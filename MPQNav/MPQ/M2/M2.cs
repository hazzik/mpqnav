using System;
using Microsoft.Xna.Framework;
using MPQNav.MPQ.ADT;

namespace MPQNav.ADT {
	internal class M2 {
		private ITriangleList _triangleList = new TriangleList();

		public ITriangleList TriangleList {
			get { return _triangleList; }
			set { _triangleList = value; }
		}

		public void Transform(Vector3 position, Vector3 rotation, float scale) {
			_triangleList = GetTriangleList().Transform(ADTManager.CreateOrigin(position), ADTManager.CreateTransform(rotation, scale));
		}

		private ITriangleList GetTriangleList() {
			return _triangleList;
		}
	}
}