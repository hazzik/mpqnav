using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers
{
    internal class Vector2ChunkParser : ChunkParser<IList<Vector2>>
    {
        public Vector2ChunkParser(uint size)
            : base(size)
        {
        }

        public override IList<Vector2> Parse(BinaryReader reader)
        {
            var result = new List<Vector2>();
            long end = reader.BaseStream.Position + Size;
            while (reader.BaseStream.Position < end)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                result.Add(new Vector2(x, y));
            }

            return result;
        }
    }
}