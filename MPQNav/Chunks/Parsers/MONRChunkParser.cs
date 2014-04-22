using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MONRChunkParser : ChunkParser<IList<Vector3>> {
		public MONRChunkParser(uint size)
			: base(size) {
		}

		public override IList<Vector3> Parse(BinaryReader reader) {
			var vectors = new List<Vector3>();

		    var end = reader.BaseStream.Position + Size;
		    while (reader.BaseStream.Position < end)
		    {
		        float vect_x = (reader.ReadSingle()*-1);
		        float vect_z = reader.ReadSingle();
		        float vect_y = reader.ReadSingle();
		        vectors.Add(new Vector3(vect_x, vect_y, vect_z));
		    }

		    return vectors;
		}
	}
}