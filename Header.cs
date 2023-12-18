using ThemModdingHerds.IO;
using BinaryReader = ThemModdingHerds.IO.BinaryReader;

namespace ThemModdingHerds.GFS;
public class Header(uint offset,ulong entryCount)
{
    public readonly static string IDENTIFIER = "Reverge Package File";
    public readonly static string VERSION = "1.1";
    public uint DataOffset = offset;
    public string Identifier = IDENTIFIER;
    public string Version = VERSION;
    public ulong EntryCount = entryCount;
}
public static class HeaderExt
{
    public static Header ReadHeader(this BinaryReader reader)
    {
        uint offset = reader.ReadUInt();
        string id = reader.ReadPascal64String();
        if(id != Header.IDENTIFIER)
            throw new Exception("Header identifier mismatch");
        string version = reader.ReadPascal64String();
        if(version != Header.VERSION)
            throw new Exception("Header version mismatch");
        ulong entryCount = reader.ReadULong();
        return new Header(offset,entryCount);
    }
}