﻿using ThemModdingHerds.IO.Binary;
using ThemModdingHerds.IO;

namespace ThemModdingHerds.GFS;
public class RevergePackageEntry(string path,long size,int alignment)
{
    public static int SIZE(string path) => 8 + path.Length + 8 + 4;
    public string Path {get;set;} = path;
    public long Size {get;set;} = size;
    public int Alignment {get;set;} = alignment;
    public byte[] Data {get;set;} = [];
    public long Offset {get;set;}
    public RevergePackageEntry(string path,long size,int alignment,byte[] data) : this(path,size,alignment)
    {
        Data = data;
    }
    public static RevergePackageEntry Create(string dirpath,string filepath,int alignment = 1)
    {
        if(!Directory.Exists(dirpath))
            throw new DirectoryNotFoundException($"{dirpath} does not exist");
        if(!File.Exists(filepath))
            throw new FileNotFoundException($"{filepath} does not exist");
        string path = System.IO.Path.GetRelativePath(dirpath,filepath)
        .Replace("\\","/");
        byte[] data = File.ReadAllBytes(filepath);
        return new(path,data.Length,alignment,data);
    }
}
public static class RevergePackageEntryExt
{
    public static RevergePackageEntry ReadRevergePackageEntry(this Reader reader)
    {
        reader.Endianness = Endianness.Big;
        string path = reader.ReadPascal64String();
        long size = reader.ReadLong();
        int align = reader.ReadInt();
        return new RevergePackageEntry(path, size, align);
    }
    public static void Write(this Writer writer,RevergePackageEntry entry)
    {
        writer.Endianness = Endianness.Big;
        writer.WritePascal64String(entry.Path);
        writer.Write(entry.Size);
        writer.Write(entry.Alignment);
    }
    public static List<RevergePackageEntry> ReadRevergePackageEntries(this Reader reader, RevergePackageHeader header)
    {
        reader.Endianness = Endianness.Big;
        List<RevergePackageEntry> entries = [];

        long runningOffset = header.DataOffset;

        for(long index = 0;index < header.EntryCount;index++)
        {
            RevergePackageEntry entry = ReadRevergePackageEntry(reader);
            runningOffset += (entry.Alignment - (runningOffset % entry.Alignment)) % entry.Alignment;
            entry.Offset = runningOffset;
            
            long oldOffset = reader.Offset;
            reader.Offset = runningOffset;

            byte[] data = reader.ReadBytes((int)entry.Size);

            reader.Offset = oldOffset;

            entry.Data = data;

            runningOffset += entry.Size;
            entries.Add(entry);
        }

        return entries;
    }
    public static void Write(this Writer writer,IEnumerable<RevergePackageEntry> entries)
    {
        writer.Endianness = Endianness.Big;
        foreach (RevergePackageEntry entry in entries)
        {
            Write(writer,entry);

            long oldOffset = writer.Offset;
            writer.Offset = entry.Offset;

            writer.Write(entry.Data);

            writer.Offset = oldOffset;
        }
    }
}
