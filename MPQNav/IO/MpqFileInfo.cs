using System.IO;
using MpqReader;

namespace MPQNav.IO
{
    public class MpqFileInfo : IFileInfo
    {
        private readonly MpqArchive mpqArchive;

        public MpqFileInfo(string mpqPath)
        {
            mpqArchive = new MpqArchive(mpqPath);
        }

        #region IFileInfo Members

        public Stream OpenRead(string file)
        {
            return mpqArchive.OpenFile(file);
        }

        public bool Exists(string file)
        {
            return mpqArchive.FileExists(file);
        }

        #endregion
    }
}