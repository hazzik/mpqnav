using System.Collections.Generic;
using System.IO;
using MPQNav.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers
{
    internal class StringArrayChunkParser : ChunkParser<string[]>
    {
        public StringArrayChunkParser(uint size)
            : base(size)
        {
        }

        public override string[] Parse(BinaryReader reader)
        {
            var result = new List<string>();
            long end = reader.BaseStream.Position + Size;
            while (reader.BaseStream.Position < end)
            {
                result.Add(reader.ReadCString());
            }
            return result.ToArray();
        }
    }
}