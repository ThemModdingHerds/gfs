using ThemModdingHerds.IO.Binary;

namespace ThemModdingHerds.GFS;
public class RevergePackageMetadata() : Dictionary<string,int>()
{
    public const char ENTRY_SEPERATOR = '\n';
    public const char DATA_SEPERATOR = ';';
    public static RevergePackageMetadata Parse(string content)
    {
        RevergePackageMetadata metadata = [];
        string[] lines = content.Split(ENTRY_SEPERATOR);
        foreach(string line in lines)
        {
            if(!line.Contains(DATA_SEPERATOR)) continue;
            string[] parts = line.Split(DATA_SEPERATOR);
            string epath = parts[0];
            int alignment = int.Parse(parts[1]);
            metadata.Add(epath,alignment);
        }
        return metadata;
    }
    public static RevergePackageMetadata Read(string path) => Parse(File.ReadAllText(path));
    public RevergePackageMetadata(IEnumerable<KeyValuePair<string, RevergePackageEntry>> gfs): this()
    {
        foreach(var pair in gfs)
        {
            Add(pair.Key,pair.Value.Alignment);
        }
    }
    public void Save(string output)
    {
        File.Delete(output);
        Writer writer = new(output);
        writer.Write(this);
        writer.Close();
    }
}
public static class RevergePackageMetadataExt
{
    public static void Write(this Writer writer,RevergePackageMetadata metadata)
    {
        foreach(var pair in metadata)
            writer.WriteASCII($"{pair.Key};{pair.Value}\n");
    }
}