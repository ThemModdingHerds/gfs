#pragma endian big

struct Header
{
    u32 dataOffset;
    u64 id_len;
    char id[id_len];
    u64 ver_len;
    char ver[ver_len];
    u64 count;
};

struct Entry
{
    u64 path_len;
    char path[path_len];
    u64 size;
    u32 alignment;
};

Header header @ $;
Entry entries[header.count] @ $;