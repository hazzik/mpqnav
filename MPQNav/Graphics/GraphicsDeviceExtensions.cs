using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MPQNav.Graphics {
	public static class GraphicsDeviceExtensions {
		public static void DrawTriangleList(this GraphicsDevice device, TriangleList list) {
			device.DrawUserIndexedPrimitives(
				PrimitiveType.TriangleList,
				list.Vertices.ToArray(),
				0,
				list.Vertices.Count,
				list.Indices.ToArray(),
				0,
				list.Indices.Count / 3);
		}
	}
}