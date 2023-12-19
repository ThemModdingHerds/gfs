using BinaryWriter = ThemModdingHerds.IO.BinaryWriter;
using BinaryReader = ThemModdingHerds.IO.BinaryReader;

namespace ThemModdingHerds.GFS;
public class File
{
    public readonly Header Header;
    public readonly List<FileEntry> Entries;
    public readonly List<ulong> Offsets;
    public File(Header header, List<FileEntry> entries)
    {
        Header = header;
        Entries = entries;
        Offsets = GetOffsets();
    }
    protected List<ulong> GetOffsets()
    {
        List<ulong> _offsets = [];

        ulong runningOffset = Header.DataOffset;

        foreach (FileEntry item in Entries)
        {
            runningOffset += ((ulong)item.Alignment - (runningOffset % (ulong)item.Alignment)) % (ulong)item.Alignment;
            _offsets.Add(runningOffset);
            runningOffset += item.Size;
        }

        return _offsets;
    }
    public void ReadData(BinaryReader reader)
    {
        for (int i = 0; i < Offsets.Count; i++)
        {
            ulong offset = Offsets[i];
            FileEntry entry = Entries[i];
            reader.Offset = (long)offset;
            entry.Data = reader.ReadBytes((int)entry.Size);
        }
    }
}
public static class FileExt
{
    public static File ReadGFSFile(this BinaryReader reader)
    {
        Header header = reader.ReadHeader();
        List<FileEntry> entries = reader.ReadGFSFileEntries(header);
        File file = new(header,entries);
        file.ReadData(reader);
        return file;
    }
    public static void Write(this BinaryWriter writer,File file)
    {
        writer.Write(file.Header);
        writer.Write(file.Entries);
        for (int i = 0; i < file.Offsets.Count; i++)
        {
            ulong offset = file.Offsets[i];
            FileEntry entry = file.Entries[i];
            writer.Offset = (long)offset;
            if(!entry.HasData)
                throw new Exception("entry has not data");
            writer.Write(entry.Data);
        }
    }
}