using System;
using System.Collections.Generic;
using System.Linq;
using MPQNav.MPQ.ADT;

namespace MPQNav.ADT {
	public static class TriangleListExtensions {
		public static ITriangleList Optimize(this ITriangleList list) {
			var vertices = list.Vertices;
			var indices = list.Indices;
			var hash = new Dictionary<VertexPositionNormalColored, int>();
			var resultIndices = new List<int>();
			for(int i = 0; i < indices.Count; i++) {
				var vertex = vertices[indices[i]];
				int index;
				if(!hash.TryGetValue(vertex, out index)) {
					index = hash.Count;
					hash.Add(vertex, index);
				}
				resultIndices.Add(index);
			}
			return new TriangleList {
				Indices = resultIndices.ToArray(),
				Vertices = hash.Keys.ToArray(),
			};
		}
	}
}