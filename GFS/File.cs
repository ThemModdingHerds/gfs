using ThemModdingHerds.IO.Binary;
using ThemModdingHerds.GFS;

namespace ThemModdingHerds;
public class GFSFile(Header header) : List<FileEntry>()
{
    public Header Header {get;} = header;
    public static GFSFile Read(string path)
    {
        if(!Directory.Exists(path))
            throw new Exception(path + " does not exist");
        if(!(File.GetAttributes(path) == FileAttributes.Directory))
            throw new Exception(path + " is not a folder");
        string[] files = Directory.GetFiles(path,"*.*",SearchOption.AllDirectories);
        List<FileEntry> entries = [];
        int dataoffset = Header.SIZE;
        foreach(string file in files)
        {
            FileEntry entry = FileEntry.Read(path,file);
            int size = FileEntry.SIZE(entry.Path);
            dataoffset += size;
        }
        Header header = new(dataoffset,entries.Count);
        long runningOffset = dataoffset;
        foreach(FileEntry entry in entries)
        {
            runningOffset += (entry.Alignment - (runningOffset % entry.Alignment)) % entry.Alignment;
            entry.Offset = runningOffset;
            runningOffset += entry.Size;
        }
        GFSFile gfs = new(header);
        gfs.AddRange(entries);
        return gfs;
    }
}
public static class FileExt
{
    public static GFSFile ReadGFSFile(this Reader reader)
    {
        Header header = reader.ReadHeader();
        List<FileEntry> entries = reader.ReadGFSFileEntries(header);
        
        GFSFile gfs = new(header);
        gfs.AddRange(entries);
        return gfs;
    }
    public static void Write(this Writer writer,GFSFile file)
    {
        writer.Write(file.Header);
        writer.Write(file.ToList());
    }
}