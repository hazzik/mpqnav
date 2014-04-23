using System.IO;
using CASCExplorer;

namespace MPQNav.IO
{
    class CascFileSystem : FileSystem
    {
        private static readonly CASCHandler handler;

        static CascFileSystem()
        {
            CASCConfig.Load(CASCHandler.OnlineMode);
            CDNHandler.Initialize(CASCHandler.OnlineMode);
            handler = new CASCHandler(null);
        }

        public override Stream OpenRead(string file)
        {
            var hash = CASCHandler.Hasher.ComputeHash(file);
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
                    return handler.OpenFile(key);
            }

            throw new System.NotSupportedException();
        }

        public override bool Exists(string file)
        {
            var hash = CASCHandler.Hasher.ComputeHash(file);
            var rootInfos = handler.GetRootInfo(hash);
            return rootInfos != null && rootInfos.Count > 0;
        }
    }
}