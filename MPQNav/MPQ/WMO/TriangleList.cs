using System;
using System.Collections.Generic;

namespace MPQNav.MPQ.ADT {
	public interface ITriangleList {
		IList<int> Indices { get; }
		IList<VertexPositionNormalColored> Vertices { get; }
	}

	public class TriangleList : ITriangleList {
		private readonly IList<int> _indices = new List<int>();
		private readonly IList<VertexPositionNormalColored> _vertices = new List<VertexPositionNormalColored>();

		public IList<int> Indices {
			get { return _indices; }
		}

		public IList<VertexPositionNormalColored> Vertices {
			get { return _vertices; }
		}
	}
}