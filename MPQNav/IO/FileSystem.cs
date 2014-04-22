using System.IO;

namespace MPQNav.IO
{
    public abstract class FileSystem 
    {
        private static FileSystem _instance;

        public static FileSystem Instance
        {
            get { return _instance ?? (_instance = CreateInternal()); }
        }

        private static FileSystem CreateInternal()
        {
            if (MpqNavSettings.UseCasc)
            {
                 return new CascFileSystem();
            }
            if (MpqNavSettings.UseMpq)
            {
                return new MpqFileSystem(MpqNavSettings.MpqPath);
            }
            return new DefaultFileSystem(MpqNavSettings.MpqPath);
        }

        public abstract Stream OpenRead(string file);

        public abstract bool Exists(string file);
    }
}