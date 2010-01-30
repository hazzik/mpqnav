using System.IO;

namespace MPQNav.IO
{
    public interface IFileInfo 
    {
        Stream OpenRead(string file);
        bool Exists(string file);
    }
}