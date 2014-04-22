using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MVERChunkParser : ChunkParser<int> {
		public MVERChunkParser(uint size)
			: base(size) {
		}

		public override int Parse(BinaryReader reader) {
			return reader.ReadInt32();
		}
	}
}