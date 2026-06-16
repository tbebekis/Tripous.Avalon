namespace Tripous.Tests;

/// <summary>
/// Tests for stream extension methods.
/// </summary>
public class StreamExtensionsTests
{
    // ● public
    /// <summary>
    /// Ensures ToArray reads all bytes and restores the original stream position.
    /// </summary>
    [Fact]
    public void ToArray_RestoresPositionForSeekableStream()
    {
        byte[] Buffer = new byte[] { 1, 2, 3, 4 };
        using MemoryStream Stream = new MemoryStream(Buffer);
        Stream.Position = 2;
        byte[] Result = Stream.ToArray();
        Assert.Equal(Buffer, Result);
        Assert.Equal(2, Stream.Position);
    }
    /// <summary>
    /// Ensures GetEncoding returns null for an empty buffer.
    /// </summary>
    [Fact]
    public void GetEncoding_ReturnsNullForEmptyBuffer()
    {
        Encoding Result = StreamExtensions.GetEncoding(Array.Empty<byte>());
        Assert.Null(Result);
    }
    /// <summary>
    /// Ensures AddPreambleTo returns null for a null buffer.
    /// </summary>
    [Fact]
    public void AddPreambleTo_ReturnsNullForNullBuffer()
    {
        byte[] Result = StreamExtensions.AddPreambleTo(null, Encoding.UTF8);
        Assert.Null(Result);
    }
    /// <summary>
    /// Ensures AddPreambleTo requires an encoding.
    /// </summary>
    [Fact]
    public void AddPreambleTo_ThrowsForNullEncoding()
    {
        Assert.Throws<ArgumentNullException>(() => StreamExtensions.AddPreambleTo(Array.Empty<byte>(), null));
    }
    /// <summary>
    /// Ensures BytesOf uses UTF-8 by default.
    /// </summary>
    [Fact]
    public void BytesOf_UsesUtf8ByDefault()
    {
        byte[] Result = StreamExtensions.BytesOf("ABC");
        Assert.Equal(Encoding.UTF8.GetBytes("ABC"), Result);
    }
    /// <summary>
    /// Ensures BytesOf returns an empty array for null text.
    /// </summary>
    [Fact]
    public void BytesOf_ReturnsEmptyArrayForNullText()
    {
        byte[] Result = StreamExtensions.BytesOf(null);
        Assert.Empty(Result);
    }
    /// <summary>
    /// Ensures StringOf uses UTF-8 when no preamble exists.
    /// </summary>
    [Fact]
    public void StringOf_UsesUtf8ByDefault()
    {
        byte[] Buffer = Encoding.UTF8.GetBytes("Καλημέρα");
        string Result = StreamExtensions.StringOf(Buffer);
        Assert.Equal("Καλημέρα", Result);
    }
}
