using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MPQNav.Util.ADTParser {
	internal class MMDXChunkParser : ChunkParser {
		public MMDXChunkParser(BinaryReader br, long pAbsoluteStart) {
			this.br = br;
			_Name = "MMDX";
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
		/// Parse MMDX element from file strem
		/// </summary>
		public string[] Parse() {
			var ret = new List<string>();
			br.BaseStream.Position = _pStart;

			UInt32 chunkSize = _Size;
			long EndPosition = br.BaseStream.Position + chunkSize;

			String m2Name = "";
			var nextByte = new byte[1];
			while(br.BaseStream.Position < EndPosition) {
				nextByte = br.ReadBytes(1);
				if(nextByte[0] != 0) {
					m2Name += Encoding.ASCII.GetString(nextByte);
				}
				else {
					// Example: world\wmo\azeroth\buildings\redridge_stable\redridge_stable.wmo
					ret.Add(m2Name);
					m2Name = "";
				}
			}
			return ret.ToArray();
		}
	}
}