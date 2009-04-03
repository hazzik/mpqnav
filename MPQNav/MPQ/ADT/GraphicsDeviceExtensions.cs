using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MPQNav.Graphics;

namespace MPQNav.ADT {
	public static class GraphicsDeviceExtensions {
		public static void DrawTriangleList(this GraphicsDevice device, ITriangleList list) {
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