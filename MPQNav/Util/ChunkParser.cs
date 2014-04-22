using System.IO;

namespace MPQNav.Util
{
    internal abstract class ChunkParser<T> : IParser<T>
    {
        protected readonly uint Size;

        protected ChunkParser(uint size)
        {
            Size = size;
        }

        public abstract T Parse(BinaryReader reader);
    }
}