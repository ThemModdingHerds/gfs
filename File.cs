using BinaryWriter = ThemModdingHerds.IO.BinaryWriter;
using BinaryReader = ThemModdingHerds.IO.BinaryReader;

namespace ThemModdingHerds.GFS;
public class File(Header header,List<FileEntry> entries)
{
    public Header Header {get;} = header;
    public List<FileEntry> Entries {get;set;} = entries;
}
public static class FileExt
{
    public static File ReadGFSFile(this BinaryReader reader)
    {
        Header header = reader.ReadHeader();
        List<FileEntry> entries = reader.ReadGFSFileEntries(header);
        return new(header,entries);
    }
    public static void Write(this BinaryWriter writer,File file)
    {
        writer.Write(file.Header);
        writer.Write(file.Entries);
    }
}