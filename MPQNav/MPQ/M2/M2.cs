using System;
using Microsoft.Xna.Framework;
using MPQNav.Collision._3D;
using MPQNav.MPQ.ADT;

namespace MPQNav.ADT {
	internal class M2 {
		private ITriangleList _triangleList = new TriangleList();

		/// <summary>
		/// AABB For the M2
		/// </summary>
		public AABB AABB { get; set; }

		/// <summary>
		/// The Orientated Bounding Box for this M2
		/// </summary>
		public OBB OBB { get; set; }

		public ITriangleList TriangleList {
			get { return _triangleList; }
			set { _triangleList = value; }
		}

		public void Transform(Vector3 position, Vector3 rotation, float scale) {
			var origin = ADTManager.CreateOrigin(position);

			Matrix rotateX = Matrix.CreateRotationX(MathHelper.ToRadians(rotation.Z));
			Matrix rotateY = Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y - 90));
			Matrix rotateZ = Matrix.CreateRotationZ(MathHelper.ToRadians(-rotation.X));
			Matrix scaleMatrix = Matrix.CreateScale(scale);

			_triangleList = _triangleList.Transform(origin, rotateX * rotateY * rotateZ * scaleMatrix);

			AABB = new AABB(TriangleList.Vertices);
			OBB = new OBB(AABB.center, AABB.extents, rotateY);
		}
	}
}