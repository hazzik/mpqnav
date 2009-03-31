using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.Util;

namespace MPQNav.MPQ.ADT {
	internal class MOVIChunkParser : ChunkParser<MOVI> {
		public MOVIChunkParser(BinaryReader reader, long absoluteStart)
			: base("MOVI", reader, absoluteStart) {
		}

		public override MOVI Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			
			var result = new List<short>();
			while(Reader.BaseStream.Position < AbsoluteStart + Size) {
				short one = Reader.ReadInt16();
				short two = Reader.ReadInt16();
				short three = Reader.ReadInt16();
				result.Add(three);
				result.Add(two);
				result.Add(one);
			}

			return new MOVI { Indices = result.ToArray() };
		}
	}
}