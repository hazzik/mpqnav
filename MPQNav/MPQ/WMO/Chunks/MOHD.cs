using System;
using Microsoft.Xna.Framework;

namespace MPQNav.MPQ.WMO.Chunks {
	internal class MOHD {
		public uint TexturesCount { get; set; }
		public uint GroupsCount { get; set; }
		public uint PortalsCount { get; set; }
		public uint LightsCount { get; set; }
		public uint ModelsCount { get; set; }
		public uint DoodadsCount { get; set; }
		public uint SetsCount { get; set; }
		public uint AmbientColor { get; set; }
		public uint WMOID { get; set; }
		public Vector3 BoundingBox1 { get; set; }
		public Vector3 BoundingBox2 { get; set; }
	}
}