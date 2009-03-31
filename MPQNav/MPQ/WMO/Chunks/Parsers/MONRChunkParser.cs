using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.MPQ.ADT {
	internal class MONRChunkParser : ChunkParser<MONR> {
		public MONRChunkParser(BinaryReader reader, long absoluteStart)
			: base("MONR", reader, absoluteStart) {
		}

		public override MONR Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			
			var result = new List<Vector3>();
			while(Reader.BaseStream.Position < AbsoluteStart + Size) {
				float vect_x = (Reader.ReadSingle() * -1);
				float vect_z = Reader.ReadSingle();
				float vect_y = Reader.ReadSingle();
				result.Add(new Vector3(vect_x, vect_y, vect_z));
			}

			return new MONR { Normals = result.ToArray() };
		}
	}
}