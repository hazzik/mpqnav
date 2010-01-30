using System.IO;
using MPQNav.IO;

namespace MPQNav.ADT
{
    public class FileInfo : IFileInfo
    {
        private readonly string mpqPath;

        public FileInfo(string mpqPath)
        {
            this.mpqPath = mpqPath;
        }

        #region IFileInfo Members

        public Stream OpenRead(string file)
        {
            return File.OpenRead(mpqPath + file);
        }

        public bool Exists(string file)
        {
            return File.Exists(mpqPath + file);
        }

        public void Dispose()
        {
        }

        #endregion
    }
}