namespace MPQNav.Util
{


    abstract class ChunkParser
    {
        protected string _Name;
        protected long _pStart;
        protected uint _Size;
        protected System.IO.BinaryReader br;

        public abstract string Name
        {
            get;
        }

        public abstract long AbsoluteStart
        {
            get;
        }

        public abstract uint Size
        {
            get;
        }

    }
}
