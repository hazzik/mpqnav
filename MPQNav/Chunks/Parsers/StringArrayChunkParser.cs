using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class StringArrayChunkParser : ChunkParser<string[]> {
		public StringArrayChunkParser(string name, BinaryReader br, long pAbsoluteStart)
			: base(name, br, pAbsoluteStart) {
		}

		public override string[] Parse() {
			Reader.BaseStream.Position = AbsoluteStart;

			var result = new List<string>();
			long end = AbsoluteStart + Size;
			while(Reader.BaseStream.Position < end) {
				result.Add(Reader.ReadCString());
			}
			return result.ToArray();
		}
	}
}