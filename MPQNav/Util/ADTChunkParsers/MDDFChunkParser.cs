using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
	internal class MDDFChunkParser : ChunkParser<MDDF[]> {
		private readonly string[] _mmdxs;

		public MDDFChunkParser(BinaryReader br, long pAbsoluteStart, string[] mmdxs)
			: base("MDDF", br, pAbsoluteStart) {
			_mmdxs = mmdxs;
		}

		/// <summary>
		/// Parse MDDF element from file strem
		/// </summary>
		public override MDDF[] Parse() {
			var _MDDF = new List<MDDF>();
			Reader.BaseStream.Position = AbsoluteStart;
			int bytesRead = 0;
			while(bytesRead < Size) {
				var lMDDF = new MDDF {
					FilePath = _mmdxs[(int)Reader.ReadUInt32()],
					UniqId = Reader.ReadUInt32(),
					Position = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()),
					OrientationA = Reader.ReadSingle(),
					OrientationB = Reader.ReadSingle(),
					OrientationC = Reader.ReadSingle(),
					Scale = (Reader.ReadUInt32() / 1024f)
				};
				bytesRead += 36; // 36 total bytes
				_MDDF.Add(lMDDF);
				//currentADT.addWMO(currentMODF.fileName, this._basePath, currentMODF);
			}
			return _MDDF.ToArray();
		}
	}
}