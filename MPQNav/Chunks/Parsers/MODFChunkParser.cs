using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MODFChunkParser : ChunkParser<List<MODF>> {
		private readonly string[] _names;

		public MODFChunkParser(string[] names, uint size)
			: base(size) {
			_names = names;
		}

	    /// <summary>
	    /// Parse MODF element from file stream
	    /// </summary>
	    /// <param name="reader"></param>
	    public override List<MODF> Parse(BinaryReader reader) {
			var modfs = new List<MODF>();
			int bytesRead = 0;
			while(bytesRead < Size)
			{
			    var modf = new MODF
			    {
			        FileName = _names[(int) reader.ReadUInt32()],
			        UniqId = reader.ReadUInt32(),
			        Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
			        Rotation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
			    };
				reader.ReadBytes(32); // 32 bytes
				bytesRead += 64; // 64 total bytes
				modfs.Add(modf);
			}
			return modfs;
		}
	}
}