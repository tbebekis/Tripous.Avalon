namespace Tripous;
 

/// <summary>
/// Helper for embedded resource files.
/// </summary>
static public class ResourceFiles
{
    // ● private 
    static string Combine(string Part1, string Part2, string Part3 = null)
    {
        StringBuilder SB = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(Part1)) SB.Append(Part1.Trim('.'));
        if (!string.IsNullOrWhiteSpace(Part2))
        {
            if (SB.Length > 0) SB.Append(".");
            SB.Append(Part2.Trim('.').Replace('/', '.').Replace('\\', '.'));
        }
        if (!string.IsNullOrWhiteSpace(Part3))
        {
            if (SB.Length > 0) SB.Append(".");
            SB.Append(Part3.Trim('.'));
        }
        return SB.ToString();
    }

    // ● public
    /// <summary>
    /// Returns all resource names in the assembly.
    /// </summary>
    static public string[] GetResourceFilePaths(Assembly A) => A.GetManifestResourceNames();
    /// <summary>
    /// Returns true if the resource exists.
    /// <code>ResourceFileExists(typeof(MyClass).Assembly, "Sql.Scripts", "MyFile.sql")</code>
    /// </summary>
    static public bool ResourceFileExists(Assembly A, string FolderPath, string FileName) => !string.IsNullOrWhiteSpace(FindResourcePath(A, FolderPath, FileName));

    /// <summary>
    /// Finds the full resource path.
    /// <code>FindResourcePath(Assembly, "Models/Ddl", "Schema.sql")</code>
    /// </summary>
    static public string FindResourcePath(Assembly A, string FolderPath, string FileName)
    {
        string Target = string.IsNullOrWhiteSpace(FolderPath) ? FileName : $"{FolderPath.Replace('/', '.')}.{FileName}";
        return A.GetManifestResourceNames().FirstOrDefault(f => f.EndsWith(Target, StringComparison.OrdinalIgnoreCase));
    }
    /// <summary>
    /// Finds the full resource path using a base namespace.
    /// <code>FindResourcePath(Assembly, "Tripous.Data", "Sql", "MyFile.sql")</code>
    /// </summary>
    static public string FindResourcePath(Assembly A, string BaseNamespace, string FolderPath, string FileName)
    {
        string FullPath = Combine(BaseNamespace, FolderPath, FileName);
        return A.GetManifestResourceNames().FirstOrDefault(f => f.Equals(FullPath, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Returns the text content of a resource.
    /// <code>string Sql = GetResourceFileText(A, "Tripous.Data", "Sql", "Init.sql")</code>
    /// </summary>
    static public string GetResourceFileText(Assembly A, string BaseNamespace, string FolderPath, string FileName)
    {
        byte[] Data = GetResourceFileData(A, BaseNamespace, FolderPath, FileName);
        return Data == null ? string.Empty : Encoding.UTF8.GetString(Data);
    }
    /// <summary>
    /// Returns the text content of a resource using simple path matching.
    /// </summary>
    static public string GetResourceFileText(Assembly A, string FolderPath, string FileName)
    {
        byte[] Data = GetResourceFileData(A, FolderPath, FileName);
        return Data == null ? string.Empty : Encoding.UTF8.GetString(Data);
    }

    /// <summary>
    /// Returns the raw data of a resource.
    /// </summary>
    static public byte[] GetResourceFileData(Assembly A, string BaseNamespace, string FolderPath, string FileName)
    {
        string Path = FindResourcePath(A, BaseNamespace, FolderPath, FileName);
        return GetResourceFileDataByPath(A, Path);
    }
    /// <summary>
    /// Returns the raw data of a resource using simple path matching.
    /// </summary>
    static public byte[] GetResourceFileData(Assembly A, string FolderPath, string FileName)
    {
        string Path = FindResourcePath(A, FolderPath, FileName);
        return GetResourceFileDataByPath(A, Path);
    }
    /// <summary>
    /// Internal helper to read stream data by exact path.
    /// </summary>
    static private byte[] GetResourceFileDataByPath(Assembly A, string Path)
    {
        if (string.IsNullOrWhiteSpace(Path)) return null;
        using (Stream Stream = A.GetManifestResourceStream(Path))
        {
            if (Stream == null) return null;
            byte[] Buffer = new byte[Stream.Length];
            Stream.ReadExactly(Buffer, 0, Buffer.Length);       // Stream.Read(Buffer, 0, Buffer.Length);
            return Buffer;
        }

    }

    /// <summary>
    /// Returns a stream for the specified resource.
    /// </summary>
    static public Stream GetResourceFileStream(Assembly A, string BaseNamespace, string FolderPath, string FileName)
    {
        string Path = FindResourcePath(A, BaseNamespace, FolderPath, FileName);
        return string.IsNullOrWhiteSpace(Path) ? null : A.GetManifestResourceStream(Path);
    }
}


