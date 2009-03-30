using System;
using System.IO;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
	internal class MHDRChunkParser : ChunkParser {
		public MHDRChunkParser(BinaryReader br, long pAbsoluteStart) {
			this.br = br;
			_Name = "MHDR";
			br.BaseStream.Position = pAbsoluteStart + 4;
			_Size = br.ReadUInt32();
			_pStart = pAbsoluteStart + 8;
		}

		/// <summary>
		/// Return the chunk Name
		/// </summary>
		public override string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Return the absolute position of MHDR chunk in the file stream
		/// </summary>
		public override long AbsoluteStart {
			get { return _pStart; }
		}

		/// <summary>
		/// Return the size of MHDR chunk
		/// </summary>
		public override uint Size {
			get { return _Size; }
		}

		/// <summary>
		/// Parse MHDR element from file strem
		/// </summary>
		public MHDR Parse() {
			//long pMHDRData = br.BaseStream.Position;
			br.BaseStream.Position = _pStart;
			var mhdr = new MHDR {
				Base = ((UInt32)_pStart),
				Pad = br.ReadUInt32(),
				OffsInfo = br.ReadUInt32(),
				OffsTex = br.ReadUInt32(),
				OffsModels = br.ReadUInt32(),
				OffsModelsIds = br.ReadUInt32(),
				OffsMapObejcts = br.ReadUInt32(),
				OffsMapObejctsIds = br.ReadUInt32(),
				OffsDoodsDef = br.ReadUInt32(),
				OffsObjectsDef = br.ReadUInt32(),
				OffsFlightBoundary = br.ReadUInt32(),
				OffsMH2O = br.ReadUInt32(),
				Pad3 = br.ReadUInt32(),
				Pad4 = br.ReadUInt32(),
				Pad5 = br.ReadUInt32(),
				Pad6 = br.ReadUInt32(),
				Pad7 = br.ReadUInt32()
			};
			return mhdr;
		}
	}
}