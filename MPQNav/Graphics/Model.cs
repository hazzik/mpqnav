using System;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Graphics {
	internal class Model {
		private readonly ITriangleList _triangleList = new TriangleList();

		public Model(ITriangleList list) {
			_triangleList = list;
		}

		public ITriangleList TriangleList {
			get { return _triangleList; }
		}

		public Model Transform(Vector3 position, Vector3 rotation, float scale) {
			return new Model(_triangleList.Transform(ADTManager.CreateOrigin(position), ADTManager.CreateTransform(rotation, scale)));
		}
	}
}