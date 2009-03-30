using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace MPQNav.Collision._2D
{
    class Rectangle
    {
        public Vector2 _min;
        public Vector2 _max;
        public float _width;
        public float _height;

        public Rectangle(Vector2 min, Vector2 max)
        {
            if (min.X <= max.X && min.Y <= max.Y)
            {
                this._min = min;
                this._max = max;
            }
            else
            {
                this._min = max;
                this._max = min;
            }
            this._width = this._max.X - this._min.X;
            this._height = this._max.Y - this._min.Y;
        }

        public Rectangle(float min_x, float min_y, float width, float height)
        {
            this._min = new Vector2(min_x, min_y);
            this._max = new Vector2(min_x + width, min_y + height);
            this._width = width;
            this._height = height;
        }

        public bool contains(float x, float y)
        {
            return this.contains(new Vector2(x, y));
        }

        public bool contains(Vector2 v)
        {
            if (v.X >= this._min.X && v.X <= this._max.X && v.Y >= this._min.Y && v.Y <= this._max.Y)
            {
                return true;
            }
            return false;
        }

        public bool Intersects_Rectangle(Rectangle r2)
        {
            return Rectangle.Intersects_Rectangle(this, r2);
        }

        public static bool Intersects_Rectangle(Rectangle r1, Rectangle r2)
        {
            if (r1._max.X < r2._min.X) { return false; }
            if (r1._min.X > r2._max.X) { return false; }
            if (r1._max.Y < r2._min.Y) { return false; }
            if (r1._min.Y > r2._max.Y) { return false; }
            return true;
        }


    }
}
