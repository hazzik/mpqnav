using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.Util;

namespace MPQNav.MPQ.WMO.Chunks.Parsers {
	internal class MOVIChunkParser : ChunkParser<int[]> {
		public MOVIChunkParser(BinaryReader reader, long absoluteStart)
			: base("MOVI", reader, absoluteStart) {
		}

		public override int[] Parse() {
			Reader.BaseStream.Position = AbsoluteStart;

			var result = new List<int>();
			while(Reader.BaseStream.Position < AbsoluteStart + Size) {
				short one = Reader.ReadInt16();
				short two = Reader.ReadInt16();
				short three = Reader.ReadInt16();
				result.Add(three);
				result.Add(two);
				result.Add(one);
			}

			return result.ToArray();
		}
	}
}