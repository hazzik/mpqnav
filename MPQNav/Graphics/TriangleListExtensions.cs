using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MPQNav.Graphics {
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

		public static ITriangleList Transform(this ITriangleList list, Vector3 origin, Matrix matrix) {
			return new TriangleList {
			                        	Indices = list.Indices,
			                        	Vertices = list.Vertices
			                        		.Select(v => new VertexPositionNormalColored(
			                        		             	Vector3.Transform(v.Position, matrix) + origin,
			                        		             	v.Color,
			                        		             	Vector3.TransformNormal(v.Normal, matrix))).ToList(),
			                        };
		}
	}
}