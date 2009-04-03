using System;
using System.Collections.Generic;

namespace MPQNav.MPQ.ADT {
	public interface ITriangleList {
		IList<int> Indices { get; }
		IList<VertexPositionNormalColored> Vertices { get; }
	}
}