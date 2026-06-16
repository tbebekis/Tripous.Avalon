/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */
namespace Tripous
{
    using System.Text;
    
    
    /// <summary>
    /// Extensions
    /// </summary>
    static public class StreamExtensions
    {
        /* ●  copy-move content from one stream to another. NOTE: CopyTo() is added to Stream class in .Net 4 */
        /// <summary>
        /// Copies all bytes from the beginning of Source to the current Dest position.
        /// Requires a seekable Source and does not reset Dest after the copy operation is complete.
        /// </summary>
        public static void CopyAllTo(this Stream Source, Stream Dest, int BufferSize = 1024 * 1024)
        {
            Source.Position = 0;
            Source.CopyTo(Dest, BufferSize);
        }
        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the Stream Position.
        /// Restores the Stream Position when the stream is seekable.
        /// </summary>
        static public byte[] ToArray(this Stream Stream)
        {
            if (Stream is MemoryStream)
                return (Stream as MemoryStream).ToArray();

            using (MemoryStream MS = new MemoryStream())
            {
                if (Stream.CanSeek)
                {
                    long Position = Stream.Position;
                    try
                    {
                        CopyAllTo(Stream, MS);
                    }
                    finally
                    {
                        Stream.Position = Position;
                    }
                }
                else
                {
                    Stream.CopyTo(MS);
                }

                return MS.ToArray();
            }
        }

        // ● text streams
        /// <summary>
        /// Returns the Encoding if a Preamble exists in a text buffer, if any, else null.
        /// </summary>
        static public Encoding GetEncoding(byte[] Buffer)
        {
            if (Buffer == null || Buffer.Length == 0)
                return null;

            var encodings = Encoding.GetEncodings()
                            .Select(e => e.GetEncoding())
                            .Select(e => new { Encoding = e, Preamble = e.GetPreamble() })
                            .Where(e => e.Preamble.Any())
                            .ToArray();

            return encodings
                .Where(enc => enc.Preamble.SequenceEqual(Buffer.Take(enc.Preamble.Length)))
                .Select(enc => enc.Encoding)
                .FirstOrDefault();
        }
        /// <summary>
        /// Returns the Encoding if a Preamble exists in a text buffer, if any, else Encoding.Default.
        /// </summary>
        static public Encoding FindEncoding(byte[] Buffer)
        {
            return GetEncoding(Buffer) ?? Encoding.Default;
        }

        /// <summary>
        /// Adds the preamble of the Encoding in front of the Buffer
        /// </summary>
        static public byte[] AddPreambleTo(byte[] Buffer, Encoding Encoding)
        {
            if (Buffer == null)
                return null;
            if (Encoding == null)
                throw new ArgumentNullException(nameof(Encoding));

            byte[] Preamble = Encoding.GetPreamble();

            // CAUTION: Not all encodings have a preamble
            if (Preamble != null && Preamble.Length > 0)
            {
                byte[] Result = new byte[Preamble.Length + Buffer.Length];
                Array.Copy(Preamble, 0, Result, 0, Preamble.Length);
                Array.Copy(Buffer, 0, Result, Preamble.Length, Buffer.Length);

                return Result;
            }

            return Buffer;

        }
        /// <summary>
        /// Removes any preamble in front of the Buffer
        /// </summary>
        static public byte[] RemovePreambleFrom(byte[] Buffer)
        {
            Encoding Encoding = GetEncoding(Buffer);
            if (Encoding == null)
                return Buffer;

            byte[] Preamble = Encoding.GetPreamble();
            byte[] Result = new byte[Buffer.Length - Preamble.Length];

            Array.Copy(Buffer, Preamble.Length, Result, 0, Result.Length);

            return Result;
        }


        /// <summary>
        /// Encodes Text into a byte array. Text must be in SourceEncoding. 
        /// <para>If SourceEncoding is null then Encoding.UTF8 is assumed.</para>
        /// <para>If DestEncoding is not null then the result byte array is converted to that Encoding. </para>
        /// <para>If PutPreamble is true then a preamble is put in front of the result array</para>
        /// </summary>
        static public byte[] BytesOf(string Text, Encoding SourceEncoding = null, Encoding DestEncoding = null, bool PutPreamble = false)
        {
            if (Text == null)
                return Array.Empty<byte>();

            if (SourceEncoding == null)
                SourceEncoding = Encoding.UTF8;

            byte[] Buffer = SourceEncoding.GetBytes(Text);

            if (DestEncoding != null && SourceEncoding.CodePage != DestEncoding.CodePage)
                Buffer = Encoding.Convert(SourceEncoding, DestEncoding, Buffer);

            if (PutPreamble)
                Buffer = AddPreambleTo(Buffer, DestEncoding == null ? SourceEncoding : DestEncoding);

            return Buffer;
        }
        /// <summary>
        /// Decodes Buffer into a string. Buffer must be in SourceEncoding.
        /// <para>If SourceEncoding is null then a preamble is used, if any, else Encoding.UTF8 is assumed.</para>
        /// <para>If DestEncoding is not null then Buffer is first converted to that Encoding</para>
        /// </summary>
        static public string StringOf(byte[] Buffer, Encoding SourceEncoding = null, Encoding DestEncoding = null)
        {
            if (Buffer == null || Buffer.Length == 0)
                return string.Empty;

            if (SourceEncoding == null)
                SourceEncoding = GetEncoding(Buffer);

            // CAUTION: Not all encodings have a preamble
            if (SourceEncoding == null)
                SourceEncoding = Encoding.UTF8;

            Buffer = RemovePreambleFrom(Buffer);

            if (DestEncoding != null)
            {
                Buffer = Encoding.Convert(SourceEncoding, DestEncoding, Buffer);
                return DestEncoding.GetString(Buffer, 0, Buffer.Length);
            }
            else
            {
                return SourceEncoding.GetString(Buffer, 0, Buffer.Length);
            }
        }
    }
}
