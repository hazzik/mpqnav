using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	/// <summary>
	/// MCIN Chunk perser
	/// </summary>
	internal class MCINChunkParser : ChunkParser<MCIN[]> {
	    /// <summary>
	    /// MCINChunkParser Perser
	    /// </summary>
	    /// <param name="size"></param>
	    public MCINChunkParser(uint size)
			: base(size) {
		}

	    /// <summary>
	    /// Parse all MCIN element from file stream
	    /// </summary>
	    /// <param name="reader"></param>
	    public override MCIN[] Parse(BinaryReader reader) {
			var mcins = new MCIN[256];
			for(var i = 0; i < 256; i++)
			{
			    mcins[i] = new MCIN
			    {
			        Offset = reader.ReadUInt32(),
			        Size = reader.ReadUInt32(),
			        Flags = reader.ReadUInt32(),
			        AsyncId = reader.ReadUInt32()
			    };
			}
			return mcins;
		}
	}
}