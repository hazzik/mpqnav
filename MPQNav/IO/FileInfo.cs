using System.IO;

namespace MPQNav.IO
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

        #endregion
    }
}