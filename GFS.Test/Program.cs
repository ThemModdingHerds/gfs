using ThemModdingHerds;
using ThemModdingHerds.GFS;
using ThemModdingHerds.IO.Binary;

string path1 = "G:\\AntiAntiDebug";
string path2 = "G:\\SteamLibrary\\steamapps\\common\\Them's Fightin' Herds\\data01\\ai-libs.gfs";
string path = "test.gfs";

RevergePackage file1 = RevergePackage.Create(path1);
RevergePackage file2 = RevergePackage.Open(path2);
RevergePackage file = RevergePackage.Merge(file1,file2);

Writer writer = new(path);
writer.Write(file);
writer.Close();
RevergePackage gfs = RevergePackage.Open(path);
Console.WriteLine(gfs);