using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MPQNav.Collision._3D;

namespace MPQNav.MPQ.ADT {
	/// <summary>
	/// Class for the WMO Group File
	/// </summary>
	internal class WMO {
		/// <summary>
		/// The Orientated Bounding Box for this WMO
		/// </summary>
		public OBB _OBB;

		public WMO() {
			Indices = new List<int>();
			Vertices = new List<VertexPositionNormalColored>();
			WmoSubList = new List<WMO_Sub>();
			TotalGroups = 1;
		}

		public WMO(String name) {
			Indices = new List<int>();
			Vertices = new List<VertexPositionNormalColored>();
			WmoSubList = new List<WMO_Sub>();
			TotalGroups = 1;
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
		public List<WMO_Sub> WmoSubList { get; set; }

		/// <summary>
		/// List of vertices used for rendering this WMO in World Space
		/// </summary>
		public List<VertexPositionNormalColored> Vertices { get; set; }

		/// <summary>
		/// List of indicies used for rendering this WMO in World Space
		/// </summary>
		public List<int> Indices { get; set; }

		public void createAABB(Vector3 v_min, Vector3 v_max) {
			AABB = new AABB(v_min, v_max);
		}

		public void addWMO_Sub(WMO_Sub wmoSub) {
			WmoSubList.Add(wmoSub);
		}

		public WMO_Sub getWMO_Sub(int index) {
			return WmoSubList[index];
		}

		public void clearCollisionData() {
			_OBB = new OBB();
			Vertices.Clear();
			Indices.Clear();
		}

		public void addVertex(Vector3 vec) {
			Vertices.Add(new VertexPositionNormalColored(vec, Color.Yellow, Vector3.Up));
		}

		public void addIndex(int index) {
			Indices.Add(index);
		}

		public void addIndex(short index) {
			Indices.Add(index);
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

			#region Nested type: MONR

			#endregion

			#region Nested type: MOVI

			#endregion

			#region Nested type: MOVT

			#endregion
		}

		#endregion

		public void Transform(Vector3 position, Vector3 rotation, float rad) {
			this.clearCollisionData();

			float pos_x = (position.X - 17066.666666666656f) * -1;
			float pos_y = position.Y;
			float pos_z = (position.Z - 17066.666666666656f) * -1;

			var origin = new Vector3(pos_x, pos_y, pos_z);

			Matrix rotateY = Matrix.CreateRotationY((rotation.Y - 90) * rad);
			Matrix rotateZ = Matrix.CreateRotationZ(rotation.X * -1 * rad);
			Matrix rotateX = Matrix.CreateRotationX(rotation.Z * rad);

			int offset = 0;

			for(int i = 0; i < WmoSubList.Count; i++) {
				WMO.WMO_Sub currentSub = getWMO_Sub(i);
				for(int v = 0; v < currentSub._MOVT.Vertices.Count; v++) {
					Vector3 baseVertex = currentSub._MOVT.Vertices[v] + origin;
					Vector3 rotatedVector = Vector3.Transform(baseVertex - origin, rotateY);
					Vector3 finalVector = rotatedVector + origin;

					this.addVertex(finalVector);
				}
				for(int index = 0; index < currentSub._MOVI.Indices.Length; index++) {
					this.addIndex(currentSub._MOVI.Indices[index] + offset);
				}
				offset = this.Vertices.Count;
			}

			// Generate the OBB
			this._OBB = new OBB(this.AABB.center, this.AABB.extents, rotateY);
		}
	}
}