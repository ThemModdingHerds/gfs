#ifndef __TMH_GFS_UTILITIES_H
#define __TMH_GFS_UTILITIES_H

#include <string>
#include <filesystem>

namespace TMH
{
    namespace GFS
    {
        const ::std::string FILE_EXTENSION = ".gfs";
        bool pack(const ::std::filesystem::path &directoryPath,const ::std::filesystem::path &outputPath);
        bool unpack(const ::std::filesystem::path &filePath,const ::std::filesystem::path &outputPath);
        static inline bool pack(const ::std::filesystem::path &directoryPath)
        {
            return pack(directoryPath,directoryPath.parent_path() / directoryPath.stem().replace_extension(FILE_EXTENSION));
        }
        static inline bool unpack(const ::std::filesystem::path &filePath)
        {
            return unpack(filePath,filePath.parent_path() / filePath.stem());
        }
        template<typename Char>
        static ::std::basic_string<Char> replaceCharacter(const ::std::basic_string<Char> &value,Char from,Char to)
        {
            ::std::basic_string<Char> string = value;
            for(typename ::std::basic_string<Char>::size_type i = 0;i < string.length();i++)
            {
                if(string[i] == from)
                    string[i] = to;
            }
            return string;
        }
    }
}

#endif