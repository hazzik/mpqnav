using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace MPQNav.Collision {
	internal class QuadTree {
		private QuadTree[] children = new QuadTree[4];

		private int childCount = 0;

		public Vector3 min;
		public Vector3 max;

		private Boolean cached = false;

		private List<VertexPositionNormalColored> _vertices = new List<VertexPositionNormalColored>();
		private List<int> _indices = new List<int>();

		public List<VertexPositionNormalColored> vertices {
			get {
				if(cached) {
					return this._vertices;
				}
				else {
					this.cacheRender();
					return this._vertices;
				}
			}
		}

		public List<int> indices {
			get {
				if(cached) {
					return this._indices;
				}
				else {
					this.cacheRender();
					return this._indices;
				}
			}
		}

		public QuadTree(Vector3 min, Vector3 max) {
			this.min = min;
			this.max = max;
		}

		public Boolean addChild(QuadTree q) {
			if(childCount >= 4) {
				return false;
			}
			this.children[childCount] = q;
			childCount++;
			return true;
		}

		private void cacheRender() {
			this._vertices.Clear();
			this._indices.Clear();
			Vector3 v1 = min;
			Vector3 v8 = max;

			Vector3 v2 = new Vector3(v8.X, v1.Y, v1.Z);
			Vector3 v3 = new Vector3(v1.X, v8.Y, v1.Z);
			Vector3 v4 = new Vector3(v8.X, v8.Y, v1.Z);
			Vector3 v5 = new Vector3(v1.X, v1.Y, v8.Z);
			Vector3 v6 = new Vector3(v8.X, v1.Y, v8.Z);
			Vector3 v7 = new Vector3(v1.X, v8.Y, v8.Z);
			Vector3[] vectors = new Vector3[8] { v1, v2, v3, v4, v5, v6, v7, v8 };
			for(int v = 0; v < 8; v++) {
				this._vertices.Add(new VertexPositionNormalColored(vectors[v], Color.Aqua, Vector3.Up));
			}

			int[] indicies = new int[36] {
				0, 3, 1, 0, 2, 3, 4, 7, 5, 4, 6, 7, 1, 7, 5, 1, 3, 7, 0, 6, 4, 0, 2, 6, 0, 5, 1, 0, 4, 5, 2, 7, 3,
				2, 6, 7
			};
			this._indices.AddRange(indicies);
			this.cached = true;
		}
	}
}