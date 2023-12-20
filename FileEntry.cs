using BinaryWriter = ThemModdingHerds.IO.BinaryWriter;
using BinaryReader = ThemModdingHerds.IO.BinaryReader;

namespace ThemModdingHerds.GFS;
public class FileEntry(string path, ulong size, int alignment, long offset)
{
    public string Path {get;} = path;
    public ulong Size {get;} = size;
    public int Alignment {get;} = alignment;
    public byte[] Data {get;set;} = [];
    public bool HasData {get => Data.Length != 0;}
    public long Offset {get;} = (alignment - (offset % alignment)) % alignment;
    public static List<FileEntry> FromFolder(string path)
    {
        List<FileEntry> entries = [];
        if(!Directory.Exists(path))
            throw new IOException(path + " does not exist");
        string[] directories = Directory.GetDirectories(path);
        string[] files = Directory.GetFiles(path);
        foreach(string directory in directories)
            entries.AddRange(FromFolder(directory));
        foreach(string file in files)
        {
            string filepath = file.Replace(path,"");
            FileStream stream = System.IO.File.OpenRead(file);
            byte[] data = new byte[stream.Length];
            stream.Read(data);
            entries.Add(new FileEntry(filepath,(ulong)stream.Length,1,0));
        }
        return entries;
    }
}
public static class FileEntryExt
{
    public static FileEntry ReadGFSFileEntry(this BinaryReader reader, long offset)
    {
        reader.Endianness = IO.Endianness.Big;
        string path = reader.ReadPascal64String();
        ulong size = reader.ReadULong();
        int align = reader.ReadInt();
        return new FileEntry(path, size, align, offset);
    }
    public static void Write(this BinaryWriter writer,FileEntry entry)
    {
        writer.Endianness = IO.Endianness.Big;
        writer.WritePascal64String(entry.Path);
        writer.Write(entry.Size);
        writer.Write(entry.Alignment);
        if(entry.HasData)
        {
            long oldOffset = writer.Offset;
            writer.Offset = entry.Offset;
            writer.Write(entry.Data);
            writer.Offset = oldOffset;
        }
    }
    public static List<FileEntry> ReadGFSFileEntries(this BinaryReader reader, Header header)
    {
        List<FileEntry> entries = [];

        ulong runningOffset = header.DataOffset;

        for(ulong index = 0;index < header.EntryCount;index++)
        {
            FileEntry entry = ReadGFSFileEntry(reader,(long)runningOffset);
            entries.Add(entry);
            long oldOffset = reader.Offset;
            reader.Offset = entry.Offset;
            entry.Data = reader.ReadBytes((int)entry.Size);
            reader.Offset = oldOffset;
            runningOffset = (ulong)entry.Offset + entry.Size;
        }

        return entries;
    }
    public static void Write(this BinaryWriter writer,List<FileEntry> entries)
    {
        foreach (FileEntry entry in entries)
            Write(writer,entry);
    }
}
