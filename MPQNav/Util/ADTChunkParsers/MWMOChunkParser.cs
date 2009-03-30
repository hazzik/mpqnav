using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MPQNav.Util.ADTParser {
	internal class MWMOChunkParser : ChunkParser {
		public MWMOChunkParser(BinaryReader br, long pAbsoluteStart) {
			this.br = br;
			_Name = "MWMO";
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
		public string[] Parse() {
			var lWMO = new List<string>();
			br.BaseStream.Position = _pStart; // 20 To get to where the list starts, 8 to get off the header.

			long EndPosition = _pStart + _Size;

			String wmoName = "";
			var nextByte = new byte[1];

			while(br.BaseStream.Position < EndPosition) {
				nextByte = br.ReadBytes(1);
				if(nextByte[0] == 0) {
					lWMO.Add(wmoName);
					wmoName = "";
				}
				else {
					wmoName += Encoding.ASCII.GetString(nextByte);
				}
			}
			return lWMO.ToArray();
		}
	}
}