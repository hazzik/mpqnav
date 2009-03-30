using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
	internal class MDDFChunkParser : ChunkParser {
		private readonly string[] _mmdxs;

		public MDDFChunkParser(BinaryReader br, long pAbsoluteStart, string[] mmdxs) {
			this.br = br;
			_Name = "MDDF";
			br.BaseStream.Position = pAbsoluteStart + 4;
			_Size = br.ReadUInt32();
			_pStart = pAbsoluteStart + 8;
			_mmdxs = mmdxs;
		}

		/// <summary>
		/// Return the chunk Name
		/// </summary>
		public override string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Return the absolute position of MDDF chunk in the file stream
		/// </summary>
		public override long AbsoluteStart {
			get { return _pStart; }
		}

		/// <summary>
		/// Return the size of MDDF chunk
		/// </summary>
		public override uint Size {
			get { return _Size; }
		}

		/// <summary>
		/// Parse MDDF element from file strem
		/// </summary>
		public List<MDDF> Parse() {
			var _MDDF = new List<MDDF>();
			br.BaseStream.Position = _pStart;
			int bytesRead = 0;
			while(bytesRead < _Size) {
				var lMDDF = new MDDF {
					FilePath = _mmdxs[(int)br.ReadUInt32()],
					UniqId = br.ReadUInt32(),
					Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
					OrientationA = br.ReadSingle(),
					OrientationB = br.ReadSingle(),
					OrientationC = br.ReadSingle(),
					Scale = (br.ReadUInt32() / 1024f)
				};
				bytesRead += 36; // 36 total bytes
				_MDDF.Add(lMDDF);
				//currentADT.addWMO(currentMODF.fileName, this._basePath, currentMODF);
			}
			return _MDDF;
		}
	}
}