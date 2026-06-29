/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

using System.Diagnostics;
using System.Reflection;
using Tripous;
using Tripous.RegBuilder;

namespace RegBuilderConsole;

/// <summary>
/// Builds configured type assemblies and generates RegBuilder source files.
/// </summary>
static public class Program
{
    // ● private
    /// <summary>
    /// Displays command-line usage.
    /// </summary>
    static void DisplayHelp()
    {
        Console.WriteLine("Usage: RegBuilderConsole [--project Name] [--configuration Debug|Release] [--no-build]");
        Console.WriteLine("Without --project all configured projects are generated.");
    }
    /// <summary>
    /// Returns the value following a command-line option.
    /// </summary>
    static string GetOptionValue(string[] Args, string OptionName)
    {
        int Index = Array.FindIndex(Args, Value => string.Equals(Value, OptionName, StringComparison.OrdinalIgnoreCase));
        return Index >= 0 && Index + 1 < Args.Length ? Args[Index + 1] : string.Empty;
    }
    /// <summary>
    /// Loads the console configuration.
    /// </summary>
    static RegBuilderSettings LoadSettings()
    {
        string FilePath = Path.Combine(AppContext.BaseDirectory, "AppSettings.json");
        return RegBuilderSettings.Load(FilePath);
    }
    /// <summary>
    /// Resolves a configured path relative to the application folder.
    /// </summary>
    static string ResolvePath(string FilePath, string Configuration)
    {
        string Value = FilePath.Replace("{Configuration}", Configuration, StringComparison.OrdinalIgnoreCase);
        return Path.GetFullPath(Value, AppContext.BaseDirectory);
    }
    /// <summary>
    /// Builds all configured projects.
    /// </summary>
    static void BuildProjects(RegBuilderSettings Settings, string Configuration)
    {
        foreach (string ConfiguredPath in Settings.BuildProjectFilePaths)
        {
            string ProjectFilePath = ResolvePath(ConfiguredPath, Configuration);
            Console.WriteLine($"Building: {ProjectFilePath}");

            ProcessStartInfo StartInfo = new("dotnet")
            {
                UseShellExecute = false
            };
            StartInfo.ArgumentList.Add("build");
            StartInfo.ArgumentList.Add(ProjectFilePath);
            StartInfo.ArgumentList.Add("--configuration");
            StartInfo.ArgumentList.Add(Configuration);

            using Process Process = Process.Start(StartInfo) ?? throw new InvalidOperationException("Could not start dotnet build.");
            Process.WaitForExit();
            if (Process.ExitCode != 0)
                throw new InvalidOperationException($"Build failed with exit code {Process.ExitCode}: {ProjectFilePath}");
        }
    }
    /// <summary>
    /// Loads and registers configured assemblies.
    /// </summary>
    static void LoadAssemblies(RegBuilderSettings Settings, string Configuration)
    {
        TypeStore.RegisterLoadedAssemblies();

        foreach (string ConfiguredPath in Settings.AssemblyFilePaths)
        {
            string AssemblyFilePath = ResolvePath(ConfiguredPath, Configuration);
            if (!File.Exists(AssemblyFilePath))
                throw new FileNotFoundException("Configured assembly was not found.", AssemblyFilePath);

            Console.WriteLine($"Loading: {AssemblyFilePath}");
            Assembly Assembly = Assembly.LoadFrom(AssemblyFilePath);
            TypeStore.Register(Assembly);
        }
    }
    /// <summary>
    /// Returns the effective output targets of a project.
    /// </summary>
    static RegBuilderOutput[] GetOutputs(RegBuilderProject Project)
    {
        if (Project.Outputs.Length > 0)
            return Project.Outputs;

        throw new InvalidOperationException($"RegBuilder project has no outputs: {Project.Name}");
    }
    /// <summary>
    /// Copies generated files for an output target.
    /// </summary>
    static void CopyOutputFiles(RegBuilderProject Project, RegBuilderOutput Output, string TempFolderPath, string Configuration)
    {
        if (string.IsNullOrWhiteSpace(Output.OutputFolderPath))
            throw new InvalidOperationException($"RegBuilder output has no output folder. Project: {Project.Name}. Target: {Output.TargetName}");

        string OutputFolderPath = ResolvePath(Output.OutputFolderPath, Configuration);
        Directory.CreateDirectory(OutputFolderPath);

        foreach (string FileName in SchemaRegistrationBuilder.GetGeneratedFileNames(Project.SchemaVersion, Output))
        {
            string SourceFilePath = Path.Combine(TempFolderPath, FileName);
            string TargetFilePath = Path.Combine(OutputFolderPath, FileName);
            if (!File.Exists(SourceFilePath))
                throw new FileNotFoundException("Generated source file was not found.", SourceFilePath);
            File.Copy(SourceFilePath, TargetFilePath, true);
        }

        Console.WriteLine($"Generated: {Project.Name} [{Output.TargetName}] -> {OutputFolderPath}");
    }
    /// <summary>
    /// Generates and copies the source files of a configured project.
    /// </summary>
    static void GenerateProject(RegBuilderProject Project, string Configuration)
    {
        RegBuilderProject BuilderProject = new()
        {
            Name = Project.Name,
            SchemaFilePath = ResolvePath(Project.SchemaFilePath, Configuration),
            NamespaceName = Project.NamespaceName,
            SchemaVersion = Project.SchemaVersion,
            DuplicateChecks = Project.DuplicateChecks,
            ReferenceFilePaths = Project.ReferenceFilePaths.Select(Value => ResolvePath(Value, Configuration)).ToArray(),
            Outputs = Project.Outputs
        };

        string TempFolderPath = Path.Combine(Path.GetTempPath(), "RegBuilderConsole", Project.Name);
        if (Directory.Exists(TempFolderPath))
            Directory.Delete(TempFolderPath, true);

        Console.WriteLine($"Generating: {Project.Name}");
        SchemaParserResult Result = SchemaRegistrationBuilder.Parse(BuilderProject, TempFolderPath);
        if (Result.HasWarnings)
            Console.WriteLine(Result.GetWarnings());
        if (Result.HasErrors)
            throw new InvalidOperationException(Result.GetErrors());

        foreach (RegBuilderOutput Output in GetOutputs(Project))
            CopyOutputFiles(Project, Output, TempFolderPath, Configuration);
    }
    // ● static public
    /// <summary>
    /// Application entry point.
    /// </summary>
    static public int Main(string[] Args)
    {
        try
        {
            if (Args.Any(Value => string.Equals(Value, "--help", StringComparison.OrdinalIgnoreCase)))
            {
                DisplayHelp();
                return 0;
            }

            string Configuration = GetOptionValue(Args, "--configuration");
            Configuration = string.IsNullOrWhiteSpace(Configuration) ? "Debug" : Configuration;
            string ProjectName = GetOptionValue(Args, "--project");
            bool Build = !Args.Any(Value => string.Equals(Value, "--no-build", StringComparison.OrdinalIgnoreCase));

            RegBuilderSettings Settings = LoadSettings();
            if (Build)
                BuildProjects(Settings, Configuration);
            LoadAssemblies(Settings, Configuration);

            RegBuilderProject[] Projects = string.IsNullOrWhiteSpace(ProjectName)
                ? Settings.Projects
                : Settings.Projects.Where(Project => string.Equals(Project.Name, ProjectName, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (Projects.Length == 0)
                throw new InvalidOperationException($"RegBuilder project was not found: {ProjectName}");

            foreach (RegBuilderProject Project in Projects)
                GenerateProject(Project, Configuration);

            Console.WriteLine("RegBuilderConsole completed.");
            return 0;
        }
        catch (Exception Ex)
        {
            Console.Error.WriteLine(Ex.Message);
            return 1;
        }
    }
}
