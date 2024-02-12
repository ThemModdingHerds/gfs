using ThemModdingHerds;
using ThemModdingHerds.GFS;
using ThemModdingHerds.IO.Binary;

string gfspath = "G:\\SteamLibrary\\steamapps\\common\\Them's Fightin' Herds\\data01\\ai-libs.gfs";

Reader reader = new(gfspath);
GFSFile file = reader.ReadGFSFile();
foreach(FileEntry entry in file)
{
    string? folder = Path.GetDirectoryName(entry.Path);
    if(folder == null) continue;
    string folderpath = Path.Combine(Path.GetFileNameWithoutExtension(gfspath),folder);
    string filepath = Path.Combine(Path.GetFileNameWithoutExtension(gfspath),entry.Path);
    Directory.CreateDirectory(folderpath);
    File.WriteAllBytes(filepath,entry.Data);
}