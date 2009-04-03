using System;
using System.Collections.Generic;

namespace MPQNav.Graphics {
	public interface ITriangleList {
		IList<int> Indices { get; }
		IList<VertexPositionNormalColored> Vertices { get; }
	}
}