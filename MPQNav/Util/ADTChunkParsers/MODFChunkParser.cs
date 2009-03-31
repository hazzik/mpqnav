using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
	internal class MODFChunkParser : ChunkParser<List<MODF>> {
		private readonly string[] _mwmos;

		public MODFChunkParser(BinaryReader br, long pAbsoluteStart, string[] mwmos)
			: base("MODF",br, pAbsoluteStart) {
			_mwmos = mwmos;
		}

		/// <summary>
		/// Parse MODF element from file strem
		/// </summary>
		public override List<MODF> Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			var _MODF = new List<MODF>();
			int bytesRead = 0;
			while(bytesRead < Size) {
				var lMODF = new MODF {
					FileName = _mwmos[(int)Reader.ReadUInt32()],
					UniqId = Reader.ReadUInt32(),
					Position = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()),
					OrientationA = Reader.ReadSingle(),
					OrientationB = Reader.ReadSingle(),
					OrientationC = Reader.ReadSingle()
				};
				Reader.ReadBytes(32); // 32 bytes
				bytesRead += 64; // 64 total bytes
				_MODF.Add(lMODF);
				//currentADT.addWMO(currentMODF.fileName, this._basePath, currentMODF);
			}
			return _MODF;
		}
	}
}