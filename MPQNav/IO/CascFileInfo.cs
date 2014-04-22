using System;
using System.IO;
using System.Linq;
using CASCExplorer;

namespace MPQNav.IO
{
    class CascFileInfo : IFileInfo
    {
        private static readonly CASCHandler handler;

        static CascFileInfo()
        {
            CASCConfig.Load();
            CDNHandler.Initialize(CASCHandler.OnlineMode);
            CASCFolder root = new CASCFolder(CASCHandler.Hasher.ComputeHash("root"));
            handler = new CASCHandler(root, null);
        }

        public Stream OpenRead(string file)
        {
            var hash = GetHash(file);
            var rootInfos = handler.GetRootInfo(hash);

            foreach (var rootInfo in rootInfos)
            {
                // only enUS atm
                if ((rootInfo.Block.Flags & LocaleFlags.enUS) == 0)
                    continue;

                var encInfo = handler.GetEncodingInfo(rootInfo.MD5);

                if (encInfo == null)
                    continue;

                foreach (var key in encInfo.Keys)
                {
                    return handler.OpenFile(key);
                }
            }

            //new CASCFile()
            //CASCHandler.OpenFile()
            throw new System.NotImplementedException();
        }

        private static ulong GetHash(string file)
        {
            return CASCHandler.FileNames.Where(fileName => string.Equals(fileName.Value, file, StringComparison.InvariantCultureIgnoreCase))
                .Select(fileName => fileName.Key)
                .FirstOrDefault();
        }

        public bool Exists(string file)
        {
            var value = CASCHandler.FileNames.Any(fileName => string.Equals(fileName.Value, file, StringComparison.InvariantCultureIgnoreCase));
            return value;
            //CDNHandler.
        }
    }
}