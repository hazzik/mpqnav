namespace MPQNav.IO
{
    public static class FileInfoFactory
    {
        private static MpqFileInfo fileInfo;

        public static IFileInfo Create()
        {
            return fileInfo ?? (fileInfo = new MpqFileInfo(MpqNavSettings.MpqPath));
        }
    }
}