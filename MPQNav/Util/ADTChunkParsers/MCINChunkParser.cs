using System;
using System.IO;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
	/// <summary>
	/// MCIN Chunk perser
	/// </summary>
	internal class MCINChunkParser : ChunkParser {
		/// <summary>
		/// MCINChunkParser Perser
		/// </summary>
		/// <param name="br">Binary Stream</param>
		/// <param name="pAbsoluteStart">The position of MCIN</param>
		public MCINChunkParser(BinaryReader br, long pAbsoluteStart) {
			this.br = br;
			_Name = "MCIN";
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
		/// Return the absolute position of MCIN chunk in the file stream
		/// </summary>
		public override long AbsoluteStart {
			get { return _pStart; }
		}

		/// <summary>
		/// Return the size of MCIN chunk
		/// </summary>
		public override uint Size {
			get { return _Size; }
		}

		/// <summary>
		/// Parse all MCIN element from file strem
		/// </summary>
		public MCIN[] Parse() {
			br.BaseStream.Position = _pStart;
			var mcins = new MCIN[256];
			for(var i = 0; i < 256; i++) {
				var mcin = new MCIN {
					Offset = br.ReadUInt32(),
					Size = br.ReadUInt32(),
					Flags = br.ReadUInt32(),
					AsyncId = br.ReadUInt32()
				};

				mcins[i] = mcin;
			}
			return mcins;
		}
	}
}