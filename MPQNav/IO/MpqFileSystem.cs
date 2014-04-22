using System.IO;
using MpqReader;

namespace MPQNav.IO
{
    public class MpqFileSystem : FileSystem
    {
        private readonly MpqArchive mpqArchive;

        public MpqFileSystem(string mpqPath)
        {
            mpqArchive = new MpqArchive(mpqPath);
        }

        public override Stream OpenRead(string file)
        {
            return mpqArchive.OpenFile(file);
        }

        public override bool Exists(string file)
        {
            return mpqArchive.FileExists(file);
        }
    }
}