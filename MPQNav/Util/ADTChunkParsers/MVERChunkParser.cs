using System;
using System.IO;

namespace MPQNav.Util {
	internal class MVERChunkParser : ChunkParser<int> {
		public MVERChunkParser(BinaryReader reader, long absoluteStart)
			: base("MVER", reader, absoluteStart) {
		}

		public override int Parse() {
			return Reader.ReadInt32();
		}
	}
}