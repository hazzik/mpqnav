using System;
using System.Collections.Generic;

namespace MPQNav.MPQ.ADT {
	public interface ITriangleList {
		IList<int> Indices { get; }
		IList<VertexPositionNormalColored> Vertices { get; }
	}

	public class TriangleList : ITriangleList {
		private IList<int> _indices = new List<int>();
		private IList<VertexPositionNormalColored> _vertices = new List<VertexPositionNormalColored>();

		public IList<int> Indices {
			get { return _indices; }
			set { _indices = value; }
		}

		public IList<VertexPositionNormalColored> Vertices {
			get { return _vertices; }
			set { _vertices = value; }
		}
	}
}