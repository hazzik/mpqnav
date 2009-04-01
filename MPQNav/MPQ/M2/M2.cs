using System;
using System.Collections.Generic;
using MPQNav.Collision._3D;
using MPQNav.MPQ.ADT;

namespace MPQNav.ADT {
	internal class M2 {
		/// <summary>
		/// AABB For the WMO
		/// </summary>
		public AABB AABB { get; set; }

		/// <summary>
		/// The Orientated Bounding Box for this WMO
		/// </summary>
		public OBB OBB { get; set; }

		private readonly TriangleList _triangleList = new TriangleList();

		public TriangleList TriangleList {
			get { return _triangleList; }
		}
	}
}