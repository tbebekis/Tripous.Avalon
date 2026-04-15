namespace Tripous.Avalon;

public class FilePathEventArgs: EventArgs
{
    public FilePathEventArgs()
    {
    }

    public string FilePath { get; set; }
}