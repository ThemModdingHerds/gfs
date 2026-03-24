#include <TMH/GFS/Utilities.hpp>
#include <TMH/GFS/Header.hpp>
#include <TMH/GFS/Entry.hpp>

#include <filesystem>
#include <fstream>

namespace TMH
{
    bool GFS::pack(const ::std::filesystem::path &directoryPath,const ::std::filesystem::path &outputPath)
    {
        ::TMH::GFS::Entries entries;
        ::std::uint32_t offset = 0;
        for(const ::std::filesystem::directory_entry &fsEntry : ::std::filesystem::recursive_directory_iterator(directoryPath))
        {
            if(!fsEntry.is_regular_file())
                continue;
            ::std::filesystem::path entryPath = ::std::filesystem::proximate(fsEntry.path(),directoryPath);
            ::TMH::GFS::Entry entry = {
                replaceCharacter<::std::filesystem::path::string_type::value_type>(
                    entryPath,
                    static_cast<::std::filesystem::path::string_type::value_type>('\\'),
                    static_cast<::std::filesystem::path::string_type::value_type>('/')
                ),
                ::std::filesystem::file_size(fsEntry),
                ::TMH::GFS::ALIGNMENT
            };
            entries.push_back(entry);
            offset += ::TMH::GFS::entrySize(entry);
        }
        ::TMH::GFS::Header header = {
            offset,
            ::TMH::GFS::IDENTIFIER,
            ::TMH::GFS::VERSION,
            entries.size()
        };
        offset = header.dataOffset = offset + ::TMH::GFS::headerSize(header);
        ::std::ofstream file(outputPath);
        ::TMH::GFS::writeHeader(file,header);
        ::TMH::GFS::writeEntries(file,entries);
        for(const ::TMH::GFS::Entry &entry : entries)
        {
            offset += (entry.alignment - (offset % entry.alignment)) % entry.alignment;
            ::std::filesystem::path entrypath = replaceCharacter<::std::filesystem::path::string_type::value_type>(
                ::std::filesystem::path(directoryPath) / entry.filepath,
                ::std::filesystem::path::preferred_separator,
                static_cast<::std::filesystem::path::string_type::value_type>('/')
            );
            ::std::ifstream entryfile(entrypath,::std::ios::binary);
            char* data = new char[entry.size];
            entryfile.read(data,entry.size);
            entryfile.close();
            file.seekp(offset);
            file.write(data,entry.size);
            delete [] data;
            offset += entry.size;
    }
    }
}