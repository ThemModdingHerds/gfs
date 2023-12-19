using BinaryWriter = ThemModdingHerds.IO.BinaryWriter;
using BinaryReader = ThemModdingHerds.IO.BinaryReader;

namespace ThemModdingHerds.GFS;
public class FileEntry(string path,ulong size,int alignment)
{
    public string Path = path;
    public ulong Size = size;
    public int Alignment = alignment;
    public byte[] Data = [];
    public bool HasData {get => Data.Length > 0;}
    public FileEntry(string path,ulong size,int alignment,byte[] data) : this(path,size,alignment)
    {
        Data = data;
    }
}
public static class FileEntryExt
{
    public static FileEntry ReadGFSFileEntry(this BinaryReader reader)
    {
        reader.Endianness = IO.Endianness.Big;
        string path = reader.ReadPascal64String();
        ulong size = reader.ReadULong();
        int align = reader.ReadInt();
        return new FileEntry(path, size, align);
    }
    public static void Write(this BinaryWriter writer,FileEntry entry)
    {
        writer.Endianness = IO.Endianness.Big;
        writer.WritePascal64String(entry.Path);
        writer.Write(entry.Size);
        writer.Write(entry.Alignment);
    }
    public static List<FileEntry> ReadGFSFileEntries(this BinaryReader reader, Header header)
    {
        List<FileEntry> entries = [];

        for(ulong index = 0;index < header.EntryCount;index++)
            entries.Add(ReadGFSFileEntry(reader));

        return entries;
    }
    public static void Write(this BinaryWriter writer,List<FileEntry> entries)
    {
        foreach (FileEntry entry in entries)
            Write(writer,entry);
    }
}
