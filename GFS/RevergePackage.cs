using ThemModdingHerds.IO.Binary;

namespace ThemModdingHerds.GFS;
public class RevergePackage(RevergePackageHeader header) : List<RevergePackageEntry>()
{
    public RevergePackageHeader Header {get;} = header;
    public static RevergePackage Create(string path,string id = RevergePackageHeader.IDENTIFIER,string ver = RevergePackageHeader.VERSION)
    {
        // check if path exists
        if(!Directory.Exists(path))
            throw new DirectoryNotFoundException($"{path} does not exist");
        // check if the path is a folder
        if(!(File.GetAttributes(path) == FileAttributes.Directory))
            throw new Exception($"{path} is not a directory");
        // get all files in that folder
        string[] files = Directory.GetFiles(path,"*.*",SearchOption.AllDirectories);
        List<RevergePackageEntry> entries = [];
        // calculate the dataoffset for the header
        int dataoffset = RevergePackageHeader.SIZE(id,ver);
        foreach(string file in files)
        {
            // create an entry from the file
            RevergePackageEntry entry = RevergePackageEntry.Create(path,file);
            // calculate the size of the entry
            int size = RevergePackageEntry.SIZE(entry.Path);
            dataoffset += size; // add to dataoffset and...
            entries.Add(entry); // add the entry to the list
        }
        // create the header from the entries and dataoffset we've calculated
        RevergePackageHeader header = new(dataoffset,entries.Count,id,ver);
        // calculate the data offset for each entry
        long runningOffset = dataoffset;
        foreach(RevergePackageEntry entry in entries)
        {
            runningOffset += (entry.Alignment - (runningOffset % entry.Alignment)) % entry.Alignment;
            entry.Offset = runningOffset;
            runningOffset += entry.Size;
        }
        // create the package
        RevergePackage gfs = new(header);
        gfs.AddRange(entries);
        return gfs;
    }
    public void SetAlignment(int align)
    {
        foreach(RevergePackageEntry entry in this)
            entry.Alignment = align;
    }
    public bool Verify(string id,string version) => Header.Verify(id,version) && Count == Header.EntryCount;
    public bool Verify() => Verify(RevergePackageHeader.IDENTIFIER,RevergePackageHeader.VERSION);
}
public static class FileExt
{
    public static RevergePackage ReadRevergePackage(this Reader reader)
    {
        RevergePackageHeader header = reader.ReadRevergePackageHeader();
        List<RevergePackageEntry> entries = reader.ReadRevergePackageEntries(header);

        RevergePackage gfs = new(header);
        gfs.AddRange(entries);
        return gfs;
    }
    public static void Write(this Writer writer,RevergePackage file)
    {
        writer.Write(file.Header);
        writer.Write(file.ToList());
    }
}