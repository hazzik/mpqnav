using System.IO;
using CASCExplorer;

namespace MPQNav.IO
{
    class CascFileSystem : FileSystem
    {
        private static readonly CASCHandler handler;

        static CascFileSystem()
        {
            handler = CASCHandler.OpenOnlineStorage(null);
        }

        public override Stream OpenRead(string file)
        {
            return handler.OpenFile(file, LocaleFlags.enUS);
        }

        public override bool Exists(string file)
        {
            return handler.FileExis(file);
        }
    }
}