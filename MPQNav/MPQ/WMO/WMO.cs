using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MPQNav.ADT;
using MPQNav.Collision._3D;

namespace MPQNav.MPQ.ADT {
	/// <summary>
	/// Class for the WMO Group File
	/// </summary>
	internal class WMO {
		private ITriangleList _triangleList = new TriangleList();
		private readonly IList<ITriangleList> _wmoSubList = new List<ITriangleList>();

		/// <summary>
		/// The Orientated Bounding Box for this WMO
		/// </summary>
		public OBB OOB { get; set; }

		/// <summary>
		/// AABB For the WMO
		/// </summary>
		public AABB AABB { get; set; }

		/// <summary>
		/// Total number of groups for the WMO
		/// </summary>
		public int TotalGroups { get; set; }

		/// <summary>
		/// List containg all the WMO Sub-Chunks for this WMO Group File
		/// </summary>
		public IList<ITriangleList> WmoSubList {
			get { return _wmoSubList; }
		}

		public ITriangleList TriangleList {
			get { return _triangleList; }
		}

		public void Transform(Vector3 position, Vector3 rotation, float scale) {
			var origin = ADTManager.CreateOrigin(position);

			Vector3 rotation1 = rotation;
			Matrix scaleMatrix = Matrix.CreateScale(scale);
			Matrix rotateX = Matrix.CreateRotationX(MathHelper.ToRadians(rotation1.Z));
			Matrix rotateY = Matrix.CreateRotationY(MathHelper.ToRadians(rotation1.Y - 90));
			Matrix rotateZ = Matrix.CreateRotationZ(MathHelper.ToRadians(-rotation1.X));

			_triangleList = GetTriangleList().Transform(origin, rotateX * rotateY * rotateZ * scaleMatrix);

			OOB = new OBB(AABB.center, AABB.extents, rotateY);
		}

		private ITriangleList GetTriangleList() {
			var list = new TriangleListCollection();

			for(int i = 0; i < WmoSubList.Count; i++) {
				list.Add(WmoSubList[i]);
			}
			return list;
		}
	}
}