using System;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	internal class MOHDChunkParser : ChunkParser<MOHD> {
		public MOHDChunkParser(uint size)
			: base(size) {
		}

		public override MOHD Parse(BinaryReader reader)
		{
		    var mohd = new MOHD
		    {
		        TexturesCount = reader.ReadUInt32(),
		        GroupsCount = reader.ReadUInt32(),
		        // This is the number of "sub-wmos" or group files that we need to read
		        PortalsCount = reader.ReadUInt32(),
		        LightsCount = reader.ReadUInt32(),
		        ModelsCount = reader.ReadUInt32(),
		        DoodadsCount = reader.ReadUInt32(),
		        SetsCount = reader.ReadUInt32(),
		        AmbientColor = reader.ReadUInt32(),
		        WMOID = reader.ReadUInt32(), // Column 2 in the WMOAreaTable.dbc
		    };
			float bb1_x = reader.ReadSingle() * -1;
			float bb1_z = reader.ReadSingle();
			float bb1_y = reader.ReadSingle();
			mohd.BoundingBox1 = new Vector3(bb1_x, bb1_y, bb1_z);
			float bb2_x = reader.ReadSingle() * -1;
			float bb2_z = reader.ReadSingle();
			float bb2_y = reader.ReadSingle();
			mohd.BoundingBox2 = new Vector3(bb2_x, bb2_y, bb2_z);
			reader.ReadUInt32();
			return mohd;
		}
	}
}