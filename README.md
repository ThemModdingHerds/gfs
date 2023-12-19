# Reverge Package File

Reverge Package File (`.gfs`) parser library for C#

## Structure

Everything is `Big Endian`

```c
struct Pascal64String
{
    size_t length;
    char chars[length];
};

struct Header
{
    uint dataOffset;
    Pascal64String identifier;
    Pascal64String version;
    size_t entryCount;
};

struct Entry
{
    Pascal64String path;
    size_t size;
    int alignment;
};

Header header;
Entry entries[header.entryCount];

```

## Calculate Data Offset

```c
size_t runningOffset = header.dataOffset;

for(size_t i = 0;i < header.entryCount;i++)
{
    Entry entry = entries[i];
    runningOffset += (entry.alignment - (runningOffset % entry.size)) % entry.alignment
    // runningOffset is the position of the data from entries[i]
    runningOffset += entry.size;
}
```

## Usage

For reading:

```c#
using ThemModdingHerds.GFS;
using ThemModdingHerds.IO;

BinaryReader reader = new BinaryReader(someStreamOrFilePath);
File file = reader.ReadGFSFile();
Header header = file.Header; // contains file count, offset, version and file identifier
List<FileEntry> entries = file.Entries; // contains all the files and their data
```
