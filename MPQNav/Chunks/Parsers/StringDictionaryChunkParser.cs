using System.Collections.Generic;
using System.IO;
using MPQNav.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers
{
    internal class StringDictionaryChunkParser : ChunkParser<IDictionary<uint, string>>
    {
        public StringDictionaryChunkParser(uint size)
            : base(size)
        {
        }

        public override IDictionary<uint, string> Parse(BinaryReader reader)
        {
            var result = new Dictionary<uint, string>();
            long end = reader.BaseStream.Position + Size;
            while (reader.BaseStream.Position < end)
            {
                var index = reader.BaseStream.Position;
                var str = reader.ReadCString();
                if (!string.IsNullOrEmpty(str))
                    result.Add((uint) index, str);
            }
            return result;
        }
    }
}