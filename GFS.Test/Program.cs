using ThemModdingHerds;
using ThemModdingHerds.GFS;
using ThemModdingHerds.IO.Binary;

string data01 = "G:\\SteamLibrary\\steamapps\\common\\Them's Fightin' Herds\\data01";
string path = "ai-libs";
string filename = $"{path}.gfs";
string bakname = $"{filename}.bak";
RevergePackage folder = RevergePackage.Create(Path.Combine(data01,path));
RevergePackage bak = RevergePackage.Open(bakname);
folder.Metadata = bak.Metadata;
File.Delete(filename);
Writer writer = new(filename);
writer.Write(folder);