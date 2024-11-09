using ThemModdingHerds.IO.Binary;

namespace ThemModdingHerds.GFS;
public class RevergePackage(RevergePackageHeader header) : Dictionary<string,RevergePackageEntry>()
{
    public RevergePackageHeader Header { get; set; } = header;
    public static RevergePackage Merge(RevergePackage gfs,params RevergePackage[] files) => Merge(gfs.Header,files);
    public static RevergePackage Merge(RevergePackageHeader header,params RevergePackage[] files) => Merge(header.Identifier,header.Version,files);
    public static RevergePackage Merge(string id,string ver,params RevergePackage[] files)
    {
        RevergePackageHeader header = new(id,ver);
        List<RevergePackageEntry> entries = [];
        RevergePackage gfs = new(header);
        foreach(RevergePackage pak in files)
            entries.AddRange(pak.Values);
        gfs.AddRange(entries);
        RevergePackageExt.RecalculateEntries(gfs);
        return gfs;
    }
    public static RevergePackage Merge(params RevergePackage[] files) => Merge(files[0].Header.Identifier,files[0].Header.Version,files);
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
    public static RevergePackage Open(string path)
    {
        Reader reader = new(path);
        RevergePackage gfs = reader.ReadRevergePackage();
        reader.Close();
        return gfs;
    }
    public RevergePackage(): this(new RevergePackageHeader())
    {

    }
    public void SetAlignment(int align)
    {
        foreach(RevergePackageEntry entry in this.Values)
            entry.Alignment = align;
    }
    public bool Verify(string id,string version) => Header.Verify(id,version) && Count == Header.EntryCount;
    public bool Verify() => Verify(RevergePackageHeader.IDENTIFIER,RevergePackageHeader.VERSION);
    public void Add(RevergePackageEntry entry)
    {
        if(ContainsKey(entry.Path))
        {
            this[entry.Path] = entry;
            return;
        }
        Add(entry.Path,entry);
    }
    public void AddRange(IEnumerable<RevergePackageEntry> entries)
    {
        foreach(RevergePackageEntry entry in entries)
            Add(entry);
    }
}
public static class RevergePackageExt
{
    public static void RecalculateEntries(RevergePackage gfs)
    {
        gfs.Header.EntryCount = gfs.Count;
        gfs.Header.DataOffset = RevergePackageHeader.SIZE(gfs.Header.Identifier,gfs.Header.Version);
        long runningOffset = gfs.Header.DataOffset;
        foreach(RevergePackageEntry entry in gfs.Values)
        {
            int size = RevergePackageEntry.SIZE(entry.Path);
            gfs.Header.DataOffset += size;
            runningOffset += (entry.Alignment - (runningOffset % entry.Alignment)) % entry.Alignment;
            entry.Offset = runningOffset;
            runningOffset += entry.Size;
        }
    }
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
        RecalculateEntries(file);
        writer.Write(file.Header);
        writer.Write(file.Values.ToList());
    }
}