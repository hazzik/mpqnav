using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MHDRChunkParser : ChunkParser<MHDR> {
		public MHDRChunkParser(uint size)
			: base(size) {
		}

		public override MHDR Parse(BinaryReader reader) {
		    return new MHDR
			{
			    Pad = reader.ReadUInt32(),
			    OffsInfo = reader.ReadUInt32(),
			    OffsTex = reader.ReadUInt32(),
			    OffsModels = reader.ReadUInt32(),
			    OffsModelsIds = reader.ReadUInt32(),
			    OffsMapObejcts = reader.ReadUInt32(),
			    OffsMapObejctsIds = reader.ReadUInt32(),
			    OffsDoodsDef = reader.ReadUInt32(),
			    OffsObjectsDef = reader.ReadUInt32(),
			    OffsFlightBoundary = reader.ReadUInt32(),
			    OffsMH2O = reader.ReadUInt32(),
			    Pad3 = reader.ReadUInt32(),
			    Pad4 = reader.ReadUInt32(),
			    Pad5 = reader.ReadUInt32(),
			    Pad6 = reader.ReadUInt32(),
			    Pad7 = reader.ReadUInt32()
			};
		}
	}
}