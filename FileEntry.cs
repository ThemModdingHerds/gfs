using ThemModdingHerds.IO.Binary;
using ThemModdingHerds.IO;

namespace ThemModdingHerds.GFS;
public class FileEntry(string path,long size,int alignment)
{
    public static int SIZE(string path) => 8 + path.Length + 8 + 4;
    public string Path {get;set;} = path;
    public long Size {get;} = size;
    public int Alignment {get;} = alignment;
    public byte[] Data {get;set;} = [];
    public bool HasData {get => Size > 0;}
    public long Offset {get;set;}
    public FileEntry(string path,long size,int alignment,byte[] data) : this(path,size,alignment)
    {
        Data = data;
    }
    public static FileEntry Read(string dirpath,string filepath,int alignment = 1)
    {
        if(!Directory.Exists(dirpath))
            throw new Exception(dirpath + " does not exist");
        if(!File.Exists(filepath))
            throw new Exception(filepath + " does not exist");
        string path = filepath.Replace(dirpath,"");
        if(path.StartsWith('/'))
            path = path[1..];
        byte[] data = File.ReadAllBytes(filepath);
        return new(path,data.Length,alignment,data);
    }
}
public static class FileEntryExt
{
    public static FileEntry ReadGFSFileEntry(this Reader reader)
    {
        reader.Endianness = Endianness.Big;
        string path = reader.ReadPascal64String();
        long size = reader.ReadLong();
        int align = reader.ReadInt();
        return new FileEntry(path, size, align);
    }
    public static void Write(this Writer writer,FileEntry entry)
    {
        writer.Endianness = Endianness.Big;
        writer.WritePascal64String(entry.Path);
        writer.Write(entry.Size);
        writer.Write(entry.Alignment);
    }
    public static List<FileEntry> ReadGFSFileEntries(this Reader reader, Header header)
    {
        reader.Endianness = Endianness.Big;
        List<FileEntry> entries = [];

        long runningOffset = header.DataOffset;

        for(long index = 0;index < header.EntryCount;index++)
        {
            FileEntry entry = ReadGFSFileEntry(reader);
            runningOffset += (entry.Alignment - (runningOffset % entry.Alignment)) % entry.Alignment;
            entry.Offset = runningOffset;
            
            long oldOffset = reader.Offset;
            reader.Offset = oldOffset;

            byte[] data = reader.ReadBytes((int)entry.Size);

            entry.Data = data;

            runningOffset += entry.Size;
        }

        return entries;
    }
    public static void Write(this Writer writer,List<FileEntry> entries)
    {
        writer.Endianness = Endianness.Big;
        foreach (FileEntry entry in entries)
        {
            Write(writer,entry);

            long oldOffset = writer.Offset;
            writer.Offset = entry.Offset;

            writer.Write(entry.Data);

            writer.Offset = oldOffset;
        }
    }
}
