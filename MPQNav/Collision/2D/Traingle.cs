using System;
using Microsoft.Xna.Framework;

namespace MPQNav.Collision._2D {
	internal class Triangle {
		#region Rotation enum

		public enum Rotation {
			rotation_none,
			rotation_1,
			rotation_2,
			rotation_3,
			rotation_4
		}

		#endregion

		public float lower_bounds_x;
		public float lower_bounds_z;

		public Vector3 point1;
		public Vector3 point2;
		public Vector3 point3;

		public Rotation rot;
		public float upper_bounds_x;
		public float upper_bounds_z;

		public Triangle(Vector3 p1, Vector3 p2, Vector3 p3) {
			point1 = p1;
			point2 = p2;
			point3 = p3;
			Vector3 center = centroid();
			int greaterX = 0;
			int greaterZ = 0;
			if(p1.X > center.X) {
				greaterX++;
			}
			if(p2.X > center.X) {
				greaterX++;
			}
			if(p3.X > center.X) {
				greaterX++;
			}

			if(p1.Z > center.Z) {
				greaterZ++;
			}
			if(p2.Z > center.Z) {
				greaterZ++;
			}
			if(p3.Z > center.Z) {
				greaterZ++;
			}

			if(greaterZ == 2 && greaterX == 1) {
				rot = Rotation.rotation_1;
			}
			else if(greaterZ == 2 && greaterX == 2) {
				rot = Rotation.rotation_2;
			}
			else if(greaterZ == 1 && greaterX == 2) {
				rot = Rotation.rotation_3;
			}
			else if(greaterZ == 1 && greaterX == 1) {
				rot = Rotation.rotation_4;
			}
			else {
				rot = Rotation.rotation_none;
			}
			calcUpperLower();
		}

		private void calcUpperLower() {
			if(point1.X < point2.X) {
				lower_bounds_x = point1.X < point3.X ? point1.X : point3.X;
			}
			else {
				lower_bounds_x = point2.X < point3.X ? point2.X : point3.X;
			}

			if(point1.X > point2.X) {
				upper_bounds_x = point1.X > point3.X ? point1.X : point1.X;
			}
			else {
				upper_bounds_x = point2.X > point3.X ? point2.X : point3.X;
			}

			if(point1.Z < point2.Z) {
				lower_bounds_z = point1.Z < point3.Z ? point1.Z : point3.Z;
			}
			else {
				lower_bounds_z = point2.Z < point3.Z ? point2.Z : point3.Z;
			}

			if(point1.Z > point2.Z) {
				if(point1.Z > point3.Z) {
					upper_bounds_z = point1.Z;
				}
				else {
					upper_bounds_z = point1.Z;
				}
			}
			else {
				if(point2.Z > point3.Z) {
					upper_bounds_z = point2.Z;
				}
				else {
					upper_bounds_z = point3.Z;
				}
			}
		}

		public float getSlope() {
			Vector3 side1 = point1 - point3;
			Vector3 side2 = point1 - point2;
			Vector3 normal = Vector3.Cross(side1, side2);
			normal.Normalize();

			var angle = (float)Math.Asin(Convert.ToDouble(normal.Y));
			angle = angle * 57.2957795f;
			return Math.Abs(angle);
		}

		public Vector3 centroid() {
			Vector3 tempVector = point1 + point2 + point3;
			return tempVector / 3;
		}

		public float getSlop3X() {
			var p1 = new Vector3();
			var p2 = new Vector3();
			// p1
			if(point1.X < point2.X) {
				if(point1.X < point3.X) {
					p1 = point1;
				}
				else {
					p1 = point3;
				}
			}
			else {
				if(point2.X < point3.X) {
					p1 = point2;
				}
				else {
					p1 = point3;
				}
			}
			// p2
			if(point1.X > point2.X) {
				if(point1.X > point3.X) {
					p2 = point1;
				}
				else {
					p2 = point3;
				}
			}
			else {
				if(point2.X > point3.X) {
					p2 = point2;
				}
				else {
					p2 = point3;
				}
			}
			return ((p2.Y - p1.Y) / (p2.X - p1.X));
		}

		public float getSlopeZ() {
			var p1 = new Vector3();
			var p2 = new Vector3();
			// p1
			if(point1.Y < point2.Y) {
				if(point1.Y < point3.Y) {
					p1 = point1;
				}
				else {
					p1 = point3;
				}
			}
			else {
				if(point2.Y < point3.Y) {
					p1 = point2;
				}
				else {
					p1 = point3;
				}
			}
			// p2
			if(point1.Y > point2.Y) {
				if(point1.Y > point3.Y) {
					p2 = point1;
				}
				else {
					p2 = point3;
				}
			}
			else {
				if(point2.Y > point3.Y) {
					p2 = point2;
				}
				else {
					p2 = point3;
				}
			}
			return ((p2.Y - p1.Y) / (p2.Y - p1.Y));
		}

		public static bool RayTriangleIntersect(Vector3 ray_origin, Vector3 ray_direction,
		                                        Vector3 vert0, Vector3 vert1, Vector3 vert2,
		                                        out float t, out float u, out float v) {
			t = 0;
			u = 0;
			v = 0;

			Vector3 edge1 = vert1 - vert0;
			Vector3 edge2 = vert2 - vert0;

			Vector3 tvec, pvec, qvec;
			float det, inv_det;

			pvec = Vector3.Cross(ray_direction, edge2);

			det = Vector3.Dot(edge1, pvec);

			if(det > -0.00001f && det < 0.00001f) {
				return false;
			}

			inv_det = 1.0f / det;

			tvec = ray_origin - vert0;

			u = Vector3.Dot(tvec, pvec) * inv_det;
			if(u < -0.001f || u > 1.001f) {
				return false;
			}

			qvec = Vector3.Cross(tvec, edge1);

			v = Vector3.Dot(ray_direction, qvec) * inv_det;
			if(v < -0.001f || u + v > 1.001f) {
				return false;
			}

			t = Vector3.Dot(edge2, qvec) * inv_det;

			if(t <= 0) {
				return false;
			}

			return true;
		}
	}
}