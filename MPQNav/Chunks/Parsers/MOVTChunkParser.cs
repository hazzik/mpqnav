using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers
{
    internal class MOVTChunkParser : ChunkParser<IList<Vector3>>
    {
        public MOVTChunkParser(uint size)
            : base(size)
        {
        }

        public override IList<Vector3> Parse(BinaryReader reader)
        {
            var result = new List<Vector3>();
            var end = reader.BaseStream.Position + Size;
            while (reader.BaseStream.Position < end)
            {
                float vect_x = (reader.ReadSingle()*-1);
                float vect_z = reader.ReadSingle();
                float vect_y = reader.ReadSingle();
                result.Add(new Vector3(vect_x, vect_y, vect_z));
            }

            return result;
        }
    }
}