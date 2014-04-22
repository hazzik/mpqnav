using System.IO;

namespace MPQNav.IO
{
    public class DefaultFileSystem : FileSystem
    {
        private readonly string _basePath;

        public DefaultFileSystem(string basePath)
        {
            _basePath = basePath;
        }

        public override Stream OpenRead(string file)
        {
            return File.OpenRead(_basePath + file);
        }

        public override bool Exists(string file)
        {
            return File.Exists(_basePath + file);
        }
    }
}