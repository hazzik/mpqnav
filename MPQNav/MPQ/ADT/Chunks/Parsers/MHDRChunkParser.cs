using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.MPQ.ADT.Chunks.Parsers {
	internal class MHDRChunkParser : ChunkParser<MHDR> {
		public MHDRChunkParser(BinaryReader br, long pAbsoluteStart)
			: base("MHDR", br, pAbsoluteStart) {
		}

		/// <summary>
		/// Parse MHDR element from file stream
		/// </summary>
		public override MHDR Parse() {
			//long pMHDRData = br.BaseStream.Position;
			Reader.BaseStream.Position = AbsoluteStart;
			var mhdr = new MHDR {
				Base = ((UInt32)AbsoluteStart),
				Pad = Reader.ReadUInt32(),
				OffsInfo = Reader.ReadUInt32(),
				OffsTex = Reader.ReadUInt32(),
				OffsModels = Reader.ReadUInt32(),
				OffsModelsIds = Reader.ReadUInt32(),
				OffsMapObejcts = Reader.ReadUInt32(),
				OffsMapObejctsIds = Reader.ReadUInt32(),
				OffsDoodsDef = Reader.ReadUInt32(),
				OffsObjectsDef = Reader.ReadUInt32(),
				OffsFlightBoundary = Reader.ReadUInt32(),
				OffsMH2O = Reader.ReadUInt32(),
				Pad3 = Reader.ReadUInt32(),
				Pad4 = Reader.ReadUInt32(),
				Pad5 = Reader.ReadUInt32(),
				Pad6 = Reader.ReadUInt32(),
				Pad7 = Reader.ReadUInt32()
			};
			return mhdr;
		}
	}
}