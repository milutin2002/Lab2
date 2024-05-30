namespace Projekat1.Models
{
    public class FileInfo
    {
        public FileInfo(string name, string path, string extension)
        {
            Name = name;
            Path = path;
            Extension = extension;
        }

        public string Name { get;}
        public string Path { get; }
        public string Extension { get; }
    }
}