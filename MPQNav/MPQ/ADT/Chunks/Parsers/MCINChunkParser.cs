using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.MPQ.ADT.Chunks.Parsers {
	/// <summary>
	/// MCIN Chunk perser
	/// </summary>
	internal class MCINChunkParser : ChunkParser<MCIN[]> {
		/// <summary>
		/// MCINChunkParser Perser
		/// </summary>
		/// <param name="br">Binary Stream</param>
		/// <param name="pAbsoluteStart">The position of MCIN</param>
		public MCINChunkParser(BinaryReader br, long pAbsoluteStart)
			: base("MCIN", br, pAbsoluteStart) {
		}

		/// <summary>
		/// Parse all MCIN element from file stream
		/// </summary>
		public override MCIN[] Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			var mcins = new MCIN[256];
			for(var i = 0; i < 256; i++) {
				var mcin = new MCIN {
				                    	Offset = Reader.ReadUInt32(),
				                    	Size = Reader.ReadUInt32(),
				                    	Flags = Reader.ReadUInt32(),
				                    	AsyncId = Reader.ReadUInt32()
				                    };
				mcins[i] = mcin;
			}
			return mcins;
		}
	}
}