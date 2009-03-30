using System;
using Microsoft.Xna.Framework;

namespace MPQNav.Collision._2D {
	internal class Rectangle {
		public float _height;
		public Vector2 _max;
		public Vector2 _min;
		public float _width;

		public Rectangle(Vector2 min, Vector2 max) {
			if(min.X <= max.X && min.Y <= max.Y) {
				_min = min;
				_max = max;
			}
			else {
				_min = max;
				_max = min;
			}
			_width = _max.X - _min.X;
			_height = _max.Y - _min.Y;
		}

		public Rectangle(float min_x, float min_y, float width, float height) {
			_min = new Vector2(min_x, min_y);
			_max = new Vector2(min_x + width, min_y + height);
			_width = width;
			_height = height;
		}

		public bool Contains(float x, float y) {
			return Contains(new Vector2(x, y));
		}

		public bool Contains(Vector2 v) {
			return v.X >= _min.X && v.X <= _max.X && v.Y >= _min.Y && v.Y <= _max.Y;
		}

		public bool Intersects_Rectangle(Rectangle r2) {
			return Intersects_Rectangle(this, r2);
		}

		public static bool Intersects_Rectangle(Rectangle r1, Rectangle r2) {
			return r1._max.X >= r2._min.X &&
			       r1._min.X <= r2._max.X &&
			       r1._max.Y >= r2._min.Y &&
			       r1._min.Y <= r2._max.Y;
		}
	}
}