using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MDDFChunkParser : ChunkParser<MDDF[]> {
		private readonly string[] _names;

		public MDDFChunkParser(uint size, string[] names)
			: base(size) {
			_names = names;
		}

	    /// <summary>
	    /// Parse MDDF element from file stream
	    /// </summary>
	    /// <param name="reader"></param>
	    public override MDDF[] Parse(BinaryReader reader) {
			var mddfs = new List<MDDF>();
			int bytesRead = 0;
			while(bytesRead < Size)
			{
			    var lMDDF = new MDDF
			    {
			        FileName = _names[(int) reader.ReadUInt32()],
			        UniqId = reader.ReadUInt32(),
			        Position = new Vector3(reader.ReadSingle(),
			            reader.ReadSingle(),
			            reader.ReadSingle()),
			        Rotation = new Vector3(reader.ReadSingle(),
			            reader.ReadSingle(),
			            reader.ReadSingle()),
			        Scale = (reader.ReadUInt32()/1024f)
			    };
				bytesRead += 36; // 36 total bytes
				mddfs.Add(lMDDF);
			}
			return mddfs.ToArray();
		}
	}
}