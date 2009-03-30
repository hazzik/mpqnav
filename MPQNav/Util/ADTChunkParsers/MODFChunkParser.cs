using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
	internal class MODFChunkParser : ChunkParser {
		private readonly string[] _mwmos;

		public MODFChunkParser(BinaryReader br, long pAbsoluteStart, string[] mwmos) {
			this.br = br;
			_Name = "MODF";
			br.BaseStream.Position = pAbsoluteStart + 4;
			_Size = br.ReadUInt32();
			_pStart = pAbsoluteStart + 8;
			_mwmos = mwmos;
		}

		/// <summary>
		/// Return the chunk Name
		/// </summary>
		public override string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Return the absolute position of MODF chunk in the file stream
		/// </summary>
		public override long AbsoluteStart {
			get { return _pStart; }
		}

		/// <summary>
		/// Return the size of MODF chunk
		/// </summary>
		public override uint Size {
			get { return _Size; }
		}

		/// <summary>
		/// Parse MODF element from file strem
		/// </summary>
		public List<MODF> Parse() {
			var _MODF = new List<MODF>();
			br.BaseStream.Position = _pStart;
			int bytesRead = 0;
			while(bytesRead < _Size) {
				var lMODF = new MODF {
					FileName = _mwmos[(int)br.ReadUInt32()],
					UniqId = br.ReadUInt32(),
					Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
					OrientationA = br.ReadSingle(),
					OrientationB = br.ReadSingle(),
					OrientationC = br.ReadSingle()
				};
				br.ReadBytes(32); // 32 bytes
				bytesRead += 64; // 64 total bytes
				_MODF.Add(lMODF);
				//currentADT.addWMO(currentMODF.fileName, this._basePath, currentMODF);
			}
			return _MODF;
		}
	}
}