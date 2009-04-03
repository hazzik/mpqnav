using System;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MOHDChunkParser : ChunkParser<MOHD> {
		public MOHDChunkParser(BinaryReader reader, long absoluteStart)
			: base("MOHD", reader, absoluteStart) {
		}

		public override MOHD Parse() {
			Reader.BaseStream.Position = AbsoluteStart;

			var mohd = new MOHD {
			                    	TexturesCount = Reader.ReadUInt32(),
			                    	GroupsCount = Reader.ReadUInt32(),
			                    	// This is the number of "sub-wmos" or group files that we need to read
			                    	PortalsCount = Reader.ReadUInt32(),
			                    	LightsCount = Reader.ReadUInt32(),
			                    	ModelsCount = Reader.ReadUInt32(),
			                    	DoodadsCount = Reader.ReadUInt32(),
			                    	SetsCount = Reader.ReadUInt32(),
			                    	AmbientColor = Reader.ReadUInt32(),
			                    	WMOID = Reader.ReadUInt32(),
			                    	// Column 2 in the WMOAreaTable.dbc
			                    };
			float bb1_x = Reader.ReadSingle() * -1;
			float bb1_z = Reader.ReadSingle();
			float bb1_y = Reader.ReadSingle();
			mohd.BoundingBox1 = new Vector3(bb1_x, bb1_y, bb1_z);
			float bb2_x = Reader.ReadSingle() * -1;
			float bb2_z = Reader.ReadSingle();
			float bb2_y = Reader.ReadSingle();
			mohd.BoundingBox2 = new Vector3(bb2_x, bb2_y, bb2_z);
			Reader.ReadUInt32();
			return mohd;
		}
	}
}