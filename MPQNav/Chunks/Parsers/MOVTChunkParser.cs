using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MOVTChunkParser : ChunkParser<IList<Vector3>> {
		public MOVTChunkParser(BinaryReader reader, long absoluteStart)
			: base("MOVT", reader, absoluteStart) {
		}

		public override IList<Vector3> Parse() {
			Reader.BaseStream.Position = AbsoluteStart;

			var result = new List<Vector3>();
			while(Reader.BaseStream.Position < AbsoluteStart + Size) {
				float vect_x = (Reader.ReadSingle() * -1);
				float vect_z = Reader.ReadSingle();
				float vect_y = Reader.ReadSingle();
				result.Add(new Vector3(vect_x, vect_y, vect_z));
			}

			return result;
		}
	}
}