using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MPQNav.Collision._3D {
	internal class AABB {
		public Vector3 center;
		public Vector3 extents;
		public List<int> indexList = new List<int>();
		public List<Vector3> vertList = new List<Vector3>();

		public AABB(IEnumerable<Vector3> vList) {
			buildFromVertList(vList);
		}

		public AABB(IEnumerable<VertexPositionNormalColored> vList)
			: this(vList.Select(x => x.Position)) {
		}

		public AABB(Vector3 v1, Vector3 v8)
			: this(CreateVectors(v1, v8)) {
		}

		private void buildFromVertList(IEnumerable<Vector3> vList) {
			var v1 = new Vector3(vList.Min(v => v.X),
			                     vList.Min(v => v.Y),
			                     vList.Min(v => v.Z));
			var v8 = new Vector3(vList.Max(v => v.X),
			                     vList.Max(v => v.Y),
			                     vList.Max(v => v.Z));

			vertList.AddRange(CreateVectors(v1, v8));

			indexList.AddRange(CreateIndexes());

			center = (v8 - v1) * 0.5f;
			center += v1;
			extents = (v8 - v1) * 0.5f;
		}

		private static int[] CreateIndexes() {
			return new[] {
				0, 3, 1, 0, 2, 3, 4, 7, 5, 4, 6, 7, 1, 7, 5, 1, 3, 7, 0, 6, 4, 0, 2, 6, 0, 5, 1, 0, 4, 5, 2, 7, 3, 2, 6, 7
			};
		}

		private static List<Vector3> CreateVectors(Vector3 v1, Vector3 v8) {
			return new List<Vector3> {
				v1,
				new Vector3(v8.X, v1.Y, v1.Z),
				new Vector3(v1.X, v8.Y, v1.Z),
				new Vector3(v8.X, v8.Y, v1.Z),
				new Vector3(v1.X, v1.Y, v8.Z),
				new Vector3(v8.X, v1.Y, v8.Z),
				new Vector3(v1.X, v8.Y, v8.Z),
				v8
			};
		}

		public static Boolean Intersects_AABB(AABB AABB_1, AABB AABB_2) {
			Vector3 firstMin = AABB_1.vertList[0];
			Vector3 firstMax = AABB_1.vertList[7];
			Vector3 secondMin = AABB_1.vertList[0];
			Vector3 secondMax = AABB_1.vertList[7];
			return (firstMin.X < secondMax.X) && (firstMax.X > secondMin.X) &&
			       (firstMin.Y < secondMax.Y) && (firstMax.Y > secondMin.Y) &&
			       (firstMin.Z < secondMax.Z) && (firstMax.Z > secondMin.Z);
		}
	}
}