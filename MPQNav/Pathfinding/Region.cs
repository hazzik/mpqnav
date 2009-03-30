using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace MPQNav.Pathfinding
{
    /// <summary>
    /// Represents a rectangular region that is a sectino of a map (ADT)
    /// </summary>
    class Region
    {
        /// <summary>
        /// The minimum (bottom left) Vector (2D)
        /// </summary>
        public Vector2 min;
        /// <summary>
        /// The maximum (top right) Vector (2D)
        /// </summary>
        public Vector2 max;
        /// <summary>
        /// The width of the region
        /// </summary>
        public float width;
        /// <summary>
        /// The height of the region
        /// </summary>
        public float height;

        /// <summary>
        /// The list of OBBs representing M2s in this region
        /// </summary>
        public List<MPQNav.Collision._3D.OBB> m2OBB = new List<MPQNav.Collision._3D.OBB>();
        /// <summary>
        /// The list of OBBs representing WMOs in this region
        /// </summary>
        public List<MPQNav.Collision._3D.OBB> wmoOBB = new List<MPQNav.Collision._3D.OBB>();
        /// <summary>
        /// The list of triangles for the ADT in this region
        /// </summary>
        public List<MPQNav.Collision._2D.Triangle> triangles = new List<MPQNav.Collision._2D.Triangle>();

        /// <summary>
        /// Creates a new instance of the region class
        /// </summary>
        /// <param name="my_min">Bottom Left corner of the region</param>
        /// <param name="my_max">Top Right corner of the region</param>
        public Region(Vector3 my_min, Vector3 my_max)
        {
            if (my_max.X > my_min.X && my_max.Y > my_min.Y)
            {
                this.min = new Vector2(my_min.X, my_min.Z);
                this.max = new Vector2(my_max.X, my_max.Z);
            }
            else
            {
                this.max = new Vector2(my_min.X, my_min.Z);
                this.min = new Vector2(my_max.X, my_max.Z);
            }
            this.width = this.max.X - this.min.X;
            this.height = this.max.Y - this.min.Y;
        }

        public Boolean addOBB_wmo(MPQNav.Collision._3D.OBB obb)
        {

            MPQNav.Collision._2D.Rectangle r1 = new MPQNav.Collision._2D.Rectangle(this.min.X, this.min.Y, this.width, this.height);
            float w = obb.LocalBoundingBox.Max.X - obb.LocalBoundingBox.Min.X;
            float h = obb.LocalBoundingBox.Max.Z - obb.LocalBoundingBox.Min.Z;

            MPQNav.Collision._2D.Rectangle r2 = new MPQNav.Collision._2D.Rectangle(obb.LocalBoundingBox.Min.X, obb.LocalBoundingBox.Min.Z, w, h);
            if (r1.Intersects_Rectangle(r2))
            {
                this.wmoOBB.Add(obb);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean addTriangle(MPQNav.Collision._2D.Triangle t)
        {
            MPQNav.Collision._2D.Rectangle r1 = new MPQNav.Collision._2D.Rectangle(this.min.X, this.min.Y, this.width, this.height);
            if (r1.contains(t.point1.X, t.point1.Z) && r1.contains(t.point2.X, t.point2.Z) && r1.contains(t.point3.X, t.point3.Z))
            {
                this.triangles.Add(t);
                return true;
            }
            else
            {
                return false;
            }
        }




    }
}
