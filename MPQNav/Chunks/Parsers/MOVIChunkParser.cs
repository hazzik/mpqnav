using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MOVIChunkParser : ChunkParser<int[]> {
		public MOVIChunkParser(uint size)
			: base(size) {
		}

		public override int[] Parse(BinaryReader reader) {
			var result = new List<int>();

		    var end = reader.BaseStream.Position + Size;
		    while (reader.BaseStream.Position < end)
		    {
		        short one = reader.ReadInt16();
		        short two = reader.ReadInt16();
		        short three = reader.ReadInt16();
		        result.Add(three);
		        result.Add(two);
		        result.Add(one);
		    }

		    return result.ToArray();
		}
	}
}