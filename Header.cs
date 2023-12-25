using ThemModdingHerds.IO.Binary;
using ThemModdingHerds.IO;
namespace ThemModdingHerds.GFS;
public class Header(int offset,long entryCount)
{
    public readonly static string IDENTIFIER = "Reverge Package File";
    public readonly static string VERSION = "1.1";
    public readonly static int SIZE = 4 + 8 + IDENTIFIER.Length + 8 + VERSION.Length + 8;
    public int DataOffset = offset;
    public string Identifier = IDENTIFIER;
    public string Version = VERSION;
    public long EntryCount = entryCount;
    public Header(int offset,long entryCount,string identifier,string version): this(offset,entryCount)
    {
        Identifier = identifier;
        Version = version;
    }
}
public static class HeaderExt
{
    public static Header ReadHeader(this Reader reader)
    {
        reader.Endianness = Endianness.Big;
        int offset = reader.ReadInt();
        string id = reader.ReadPascal64String();
        if(id != Header.IDENTIFIER)
            throw new Exception("Header identifier mismatch");
        string version = reader.ReadPascal64String();
        if(version != Header.VERSION)
            throw new Exception("Header version mismatch");
        long entryCount = reader.ReadLong();
        return new Header(offset,entryCount);
    }
    public static void Write(this Writer writer,Header header)
    {
        writer.Endianness = Endianness.Big;
        writer.Write(header.DataOffset);
        writer.WritePascal64String(header.Identifier);
        writer.WritePascal64String(header.Version);
        writer.Write(header.EntryCount);
    }
}