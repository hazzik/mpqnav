using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MWMOChunkParser : ChunkParser<string[]> {
		public MWMOChunkParser(BinaryReader br, long pAbsoluteStart)
			: base("MWMO", br, pAbsoluteStart) {
		}

		/// <summary>
		/// Parse MHDR element from file stream
		/// </summary>
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