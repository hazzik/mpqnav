using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers
{
    internal class Vector3ChunkParser : ChunkParser<IList<Vector3>>
    {
        public Vector3ChunkParser(uint size)
            : base(size)
        {
        }

        public override IList<Vector3> Parse(BinaryReader reader)
        {
            var result = new List<Vector3>();
            var end = reader.BaseStream.Position + Size;
            while (reader.BaseStream.Position < end)
            {
                float x = (reader.ReadSingle()*-1);
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                result.Add(new Vector3(x, y, z));
            }

            return result;
        }
    }
}