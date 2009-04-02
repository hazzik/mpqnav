using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MPQNav.ADT;
using MPQNav.Collision._3D;
using MPQNav.MPQ.WMO.Chunks;

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
		public OBB _OBB;

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

		public void Transform(Vector3 position, Vector3 rotation) {
			_OBB = new OBB();
			var list = new TriangleListCollection();

			float pos_x = (position.X - 17066.666666666656f) * -1;
			float pos_y = position.Y;
			float pos_z = (position.Z - 17066.666666666656f) * -1;

			var origin = new Vector3(pos_x, pos_y, pos_z);

			Matrix rotateY = Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y - 90));
			Matrix rotateZ = Matrix.CreateRotationZ(MathHelper.ToRadians(-rotation.X));
			Matrix rotateX = Matrix.CreateRotationX(MathHelper.ToRadians(rotation.Z));

			for(int i = 0; i < WmoSubList.Count; i++) {
				list.Add(Transform(WmoSubList[i], origin, rotateY));
			}

			_triangleList = list;
			// Generate the OBB
			_OBB = new OBB(AABB.center, AABB.extents, rotateY);
		}

		private static ITriangleList Transform(ITriangleList list, Vector3 origin, Matrix rotateY) {
			return new TriangleList {
				Indices = list.Indices,
				Vertices = list.Vertices
					.Select(v => new VertexPositionNormalColored(
										Vector3.Transform(v.Position, rotateY) + origin,
										Color.Yellow,
										Vector3.TransformNormal(v.Normal, rotateY) + origin)).ToList(),
			};
		}
	}
}