using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MPQNav.Collision._3D;
using MPQNav.MPQ.WMO.Chunks;

namespace MPQNav.MPQ.ADT {
	/// <summary>
	/// Class for the WMO Group File
	/// </summary>
	internal class WMO {
		private readonly TriangleList _triangleList = new TriangleList();
		private readonly List<WMO_Sub> _wmoSubList = new List<WMO_Sub>();

		/// <summary>
		/// The Orientated Bounding Box for this WMO
		/// </summary>
		public OBB _OBB;

		public WMO() {
		}

		public WMO(String name) {
			Name = name;
		}

		/// <summary>
		/// AABB For the WMO
		/// </summary>
		public AABB AABB { get; set; }

		/// <summary>
		/// Total number of groups for the WMO
		/// </summary>
		public int TotalGroups { get; set; }

		/// <summary>
		/// Name of the WMO Group file
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// List containg all the WMO Sub-Chunks for this WMO Group File
		/// </summary>
		public List<WMO_Sub> WmoSubList {
			get { return _wmoSubList; }
		}

		public TriangleList TriangleList {
			get { return _triangleList; }
		}

		public void Transform(Vector3 position, Vector3 rotation, float rad) {
			_OBB = new OBB();
			TriangleList.Vertices.Clear();
			TriangleList.Indices.Clear();

			float pos_x = (position.X - 17066.666666666656f) * -1;
			float pos_y = position.Y;
			float pos_z = (position.Z - 17066.666666666656f) * -1;

			var origin = new Vector3(pos_x, pos_y, pos_z);

			Matrix rotateY = Matrix.CreateRotationY((rotation.Y - 90) * rad);
			Matrix rotateZ = Matrix.CreateRotationZ(rotation.X * -1 * rad);
			Matrix rotateX = Matrix.CreateRotationX(rotation.Z * rad);

			int offset = 0;

			for(int i = 0; i < WmoSubList.Count; i++) {
				WMO_Sub currentSub = WmoSubList[i];
				for(int v = 0; v < currentSub._MOVT.Vertices.Count; v++) {
					Vector3 baseVertex = currentSub._MOVT.Vertices[v] + origin;
					Vector3 rotatedVector = Vector3.Transform(baseVertex - origin, rotateY);
					Vector3 finalVector = rotatedVector + origin;

					TriangleList.Vertices.Add(new VertexPositionNormalColored(finalVector, Color.Yellow, Vector3.Up));
				}
				for(int index = 0; index < currentSub._MOVI.Indices.Length; index++) {
					TriangleList.Indices.Add(currentSub._MOVI.Indices[index] + offset);
				}
				offset = TriangleList.Vertices.Count;
			}

			// Generate the OBB
			_OBB = new OBB(AABB.center, AABB.extents, rotateY);
		}

		#region Nested type: WMO_Sub

		public class WMO_Sub {
			public int _index;
			public MONR _MONR = new MONR();
			public MOVI _MOVI = new MOVI();
			public MOVT _MOVT = new MOVT();

			public WMO_Sub(int index) {
				_index = index;
			}
		}

		#endregion
	}
}