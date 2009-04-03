using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MVERChunkParser : ChunkParser<int> {
		public MVERChunkParser(BinaryReader reader, long absoluteStart)
			: base("MVER", reader, absoluteStart) {
		}

		public override int Parse() {
			return Reader.ReadInt32();
		}
	}
}