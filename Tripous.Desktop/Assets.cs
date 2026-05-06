namespace Tripous.Desktop;

/// <summary>
/// Handles Avalonia resources.
/// </summary>
static public class Assets
{
    const string Protocol = "avares://";
    /// <summary>
    /// Returns true if an asset resource path exists, e.g. <c>avares://Tripous.Desktop/Images/MyImage.png</c>
    /// </summary>
    static public Uri FindUriByPath(string AssetPath)
    {
        if (System.Uri.TryCreate(AssetPath, UriKind.Absolute, out Uri Uri))
            if (AssetLoader.Exists(Uri))
                return Uri;
        return null;
    }
    
    /// <summary>
    /// Finds and returns the <see cref="Uri"/> of an asset, if any, else null.
    /// <para>Combines the specified folders and filename.</para>
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Uri FindUri(string[] Folders, string FileName)
    {
        void InsertToTop(Assembly A, List<Assembly> List)
        {
            List.Remove(A);
            List.Insert(0, A);
        }
        
        Uri Result = null;
        string AssetPath;

        string AssemblyName;
        
        List<Assembly> List = Sys.GetApplicationAssemblies(["SkiaSharp", "Tmds.DBus.Protocol", "HarfBuzzSharp", "FirebirdSql", "Npgsql", "MySql", "Oracle"]);
        InsertToTop(Assembly.GetEntryAssembly(), List);
        InsertToTop(Assembly.GetExecutingAssembly(), List);
        InsertToTop(typeof(Assets).Assembly, List);
        InsertToTop(Assembly.GetCallingAssembly(), List);
 
        foreach (Assembly A in List)
        {
            AssemblyName = A.GetName().Name;
            foreach (string Folder in Folders)
            {
                AssetPath = Combine(Protocol, AssemblyName, Folder, FileName);
                Result = FindUriByPath(AssetPath);
                if (Result != null)
                    return Result;
            }
        }

        return Result;
    }
    /// <summary>
    /// Finds and returns the <see cref="Uri"/> of an asset, if any, else null.
    /// <para>Combines a specified folder and filename.</para>
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Uri FindUri(string Folder, string FileName) => FindUri([Folder], FileName);
    /// <summary>
    /// Finds and returns the <see cref="Uri"/> of an asset, if any, else null.
    /// <para>Combines a number of folders and the specified filename.</para>
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Uri FindUri(string FileName) => FindUri(["Images32", "Images16", "Images", "Assets", "Binaries", "Files"], FileName);
    
    /// <summary>
    /// Combines parts into an Avalonia asset URI
    /// </summary>
    static public string Combine(string BasePath, params string[] Parts)
    {
        if (string.IsNullOrEmpty(BasePath))
            return string.Empty;

        string Combined = BasePath.IsSameText(Protocol) ? Protocol : BasePath.TrimEnd('/');

        foreach (string Part in Parts)
        {
            if (string.IsNullOrWhiteSpace(Part)) 
                continue;

            string CleanPart = Part.Trim('/');

            if (Combined.IsSameText(Protocol))
                Combined = $"{Combined}{CleanPart}";
            else
                Combined = $"{Combined}/{CleanPart}";
        }

        return Combined;
    }
    
    /// <summary>
    /// Creates an image of a specified size.
    /// </summary>
    static public Image CreateImage(ImageSizeType SizeType, Size? Size = null)
    {
        switch (SizeType)
        {
            case ImageSizeType.Icon16: return new Image() { Width = 16, Height = 16 }; 
            case ImageSizeType.Icon32: return new Image() { Width = 32, Height = 32 }; 
            case ImageSizeType.Defined:
                if (Size == null)
                    throw new TripousException("Image size not defined");
                return new Image() { Width = Size.Value.Width, Height = Size.Value.Height }; 
            default: return new Image();
        }
    }

    /// <summary>
    /// Sets the <see cref="Image.Source"/> of a specified image.
    /// </summary>
    static public bool SetImage(Image Image, Uri Uri)
    {
        if (Image != null && Uri != null)
        {
            using (Stream Stream = AssetLoader.Open(Uri))
                Image.Source = new Bitmap(Stream);
            return true;
        }

        return false;
    }
    /// <summary>
    /// Sets the <see cref="Image.Source"/> of a specified image.
    /// </summary>
    static public bool SetImage(Image Image, string FileName)
    {
        Uri Uri = FindUri(FileName);
        if (Uri != null)
           return SetImage(Image, Uri);
        return false;
    }
    
    /// <summary>
    /// Returns an image of a specified size, if found in assets, else null.
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Image FindImage(string FileName, ImageSizeType SizeType, Size? Size = null)
    {
        Image Result = null;
        
        Uri Uri = FindUri(FileName);
        if (Uri != null)
        {
            Result = CreateImage(SizeType, Size);
            SetImage(Result, Uri);
        }

        return Result;
    }
    /// <summary>
    /// Returns an image of a specified size, if found in assets, else null.
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Image FindImage16(string FileName) => FindImage(FileName, ImageSizeType.Icon16);
    /// <summary>
    /// Returns an image of a specified size, if found in assets, else null.
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Image FindImage32(string FileName) => FindImage(FileName, ImageSizeType.Icon32);
    /// <summary>
    /// Returns an image of a specified size, if found in assets, else null.
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Image FindImage(string FileName, Size Size) => FindImage(FileName, ImageSizeType.Defined, Size);
    /// <summary>
    /// Returns an image of any size, if found in assets, else null.
    /// <para>NOTE: It searches the application assemblies too.</para>
    /// </summary>
    static public Image FindImage(string FileName) => FindImage(FileName, ImageSizeType.Undefined);

    /// <summary>
    /// Finds and returns an asset as a stream, if any, else null.
    /// <para>WARNING: The returned stream should be disposed by the caller.</para>
    /// </summary>
    static public Stream FindAsset(string FileName)
    {
        Uri Uri = FindUri(FileName);
        if (Uri != null)
        {
            Stream Stream = AssetLoader.Open(Uri);
            return Stream;
        }

        return null;
    }
 
}