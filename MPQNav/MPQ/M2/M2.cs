using System;
using System.Collections.Generic;
using MPQNav.Collision._3D;

namespace MPQNav.ADT {
	internal class M2 {
		public M2() {
			Indices = new List<int>();
			Vertices = new List<VertexPositionNormalColored>();
		}

		/// <summary>
		/// AABB For the WMO
		/// </summary>
		public AABB AABB { get; set; }

		/// <summary>
		/// The Orientated Bounding Box for this WMO
		/// </summary>
		public OBB OBB { get; set; }

		/// <summary>
		/// List of vertices used for rendering this M2 in World Space
		/// </summary>
		public List<VertexPositionNormalColored> Vertices { get; set; }

		/// <summary>
		/// List of indicies used for rendering this M2 in World Space
		/// </summary>
		public List<int> Indices { get; set; }
	}
}