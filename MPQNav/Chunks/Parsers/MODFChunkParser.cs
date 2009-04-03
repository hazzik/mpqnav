using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MODFChunkParser : ChunkParser<List<MODF>> {
		private readonly string[] _names;

		public MODFChunkParser(BinaryReader br, long pAbsoluteStart, string[] names)
			: base("MODF", br, pAbsoluteStart) {
			_names = names;
		}

		/// <summary>
		/// Parse MODF element from file stream
		/// </summary>
		public override List<MODF> Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			var _MODF = new List<MODF>();
			int bytesRead = 0;
			while(bytesRead < Size) {
				var lMODF = new MODF {
				                     	FileName = _names[(int)Reader.ReadUInt32()],
				                     	UniqId = Reader.ReadUInt32(),
				                     	Position = new Vector3(Reader.ReadSingle(),
				                     	                       Reader.ReadSingle(),
				                     	                       Reader.ReadSingle()),
				                     	Rotation = new Vector3(Reader.ReadSingle(),
				                     	                       Reader.ReadSingle(),
				                     	                       Reader.ReadSingle()),
				                     };
				Reader.ReadBytes(32); // 32 bytes
				bytesRead += 64; // 64 total bytes
				_MODF.Add(lMODF);
			}
			return _MODF;
		}
	}
}