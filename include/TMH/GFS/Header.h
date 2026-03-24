#ifndef __TMH_GFS_HEADER_H
#define __TMH_GFS_HEADER_H

#include <stdint.h>

#define TMH_GFS_IDENTIFIER "Reverge Package File"
#define TMH_GFS_VERSION "1.1"

typedef struct
{
    uint32_t dataOffset;
    char* identifier;
    char* version;
    uint64_t entries;
} RevergePackageHeader;

#endif