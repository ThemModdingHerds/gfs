using ThemModdingHerds;
using ThemModdingHerds.GFS;
using ThemModdingHerds.IO.Binary;

string path = "G:\\AntiAntiDebug";

RevergePackage file = RevergePackage.Create(path);

foreach(RevergePackageEntry entry in file)
    entry.Alignment = 0x0200;

Writer writer = new("test.gfs");

writer.Write(file);