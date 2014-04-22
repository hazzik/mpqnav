namespace MPQNav.IO
{
    public static class FileInfoFactory
    {
        private static IFileInfo fileInfo;

        public static IFileInfo Create()
        {
            return fileInfo ?? (fileInfo = CreateInternal());
        }

        private static IFileInfo CreateInternal()
        {
            if (MpqNavSettings.UseCasc)
            {
                 return new CascFileInfo();
            }
            if (MpqNavSettings.UseMpq)
            {
                return new MpqFileInfo(MpqNavSettings.MpqPath);
            }
            return new FileInfo(MpqNavSettings.MpqPath);
        }
    }
}