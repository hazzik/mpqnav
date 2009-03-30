using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace MPQNav.Collision._2D
{
    class Triangle
    {
        public Vector3 point1;
        public Vector3 point2;
        public Vector3 point3;

        public rotation rot;
        public float lower_bounds_x;
        public float lower_bounds_z;
        public float upper_bounds_x;
        public float upper_bounds_z;

        public enum rotation
        {
            rotation_none,
            rotation_1,
            rotation_2,
            rotation_3,
            rotation_4
        }

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.point1 = p1;
            this.point2 = p2;
            this.point3 = p3;
            Vector3 center = this.centroid();
            int greaterX = 0;
            int greaterZ = 0;
            if (p1.X > center.X) { greaterX++; }
            if (p2.X > center.X) { greaterX++; }
            if (p3.X > center.X) { greaterX++; }

            if (p1.Z > center.Z) { greaterZ++; }
            if (p2.Z > center.Z) { greaterZ++; }
            if (p3.Z > center.Z) { greaterZ++; }

            if (greaterZ == 2 && greaterX == 1)
            {
                this.rot = rotation.rotation_1;
            }
            else if (greaterZ == 2 && greaterX == 2)
            {
                this.rot = rotation.rotation_2;
            }
            else if (greaterZ == 1 && greaterX == 2)
            {
                this.rot = rotation.rotation_3;
            }
            else if (greaterZ == 1 && greaterX == 1)
            {
                this.rot = rotation.rotation_4;
            }
            else
            {
                this.rot = rotation.rotation_none;
            }
            this.calcUpperLower();

        }

        private void calcUpperLower()
        {
            if (this.point1.X < this.point2.X)
            {
                if (this.point1.X < this.point3.X)
                {
                    this.lower_bounds_x = this.point1.X;
                }
                else
                {
                    this.lower_bounds_x = this.point3.X;
                }
            }
            else
            {
                if (this.point2.X < this.point3.X)
                {
                    this.lower_bounds_x = this.point2.X;
                }
                else
                {
                    this.lower_bounds_x = this.point3.X;
                }
            }

            if (this.point1.X > this.point2.X)
            {
                if (this.point1.X > this.point3.X)
                {
                    this.upper_bounds_x = this.point1.X;
                }
                else
                {
                    this.upper_bounds_x = this.point1.X;
                }
            }
            else
            {
                if (this.point2.X > this.point3.X)
                {
                    this.upper_bounds_x = this.point2.X;
                }
                else
                {
                    this.upper_bounds_x = this.point3.X;
                }
            }

            if (this.point1.Z < this.point2.Z)
            {
                if (this.point1.Z < this.point3.Z)
                {
                    this.lower_bounds_z = this.point1.Z;
                }
                else
                {
                    this.lower_bounds_z = this.point3.Z;
                }
            }
            else
            {
                if (this.point2.Z < this.point3.Z)
                {
                    this.lower_bounds_z = this.point2.Z;
                }
                else
                {
                    this.lower_bounds_z = this.point3.Z;
                }
            }

            if (this.point1.Z > this.point2.Z)
            {
                if (this.point1.Z > this.point3.Z)
                {
                    this.upper_bounds_z = this.point1.Z;
                }
                else
                {
                    this.upper_bounds_z = this.point1.Z;
                }
            }
            else
            {
                if (this.point2.Z > this.point3.Z)
                {
                    this.upper_bounds_z = this.point2.Z;
                }
                else
                {
                    this.upper_bounds_z = this.point3.Z;
                }
            }
        }

        public float getSlope()
        {
            Vector3 side1 = point1 - point3;
            Vector3 side2 = point1 - point2;
            Vector3 normal = Vector3.Cross(side1, side2);
            normal.Normalize();

            float angle = (float)Math.Asin(Convert.ToDouble(normal.Y));
            angle = angle * 57.2957795f;
            return Math.Abs(angle);
        }

        public Vector3 centroid()
        {
            Vector3 tempVector = point1 + point2 + point3;
            return tempVector / 3;
        }

        public float getSlop3X()
        {
            Vector3 p1 = new Vector3();
            Vector3 p2 = new Vector3();
            // p1
            if (this.point1.X < this.point2.X)
            {
                if (this.point1.X < this.point3.X)
                {
                    p1 = this.point1;
                }
                else
                {
                    p1 = this.point3;
                }
            }
            else
            {
                if (this.point2.X < this.point3.X)
                {
                    p1 = this.point2;
                }
                else
                {
                    p1 = this.point3;
                }
            }
            // p2
            if (this.point1.X > this.point2.X)
            {
                if (this.point1.X > this.point3.X)
                {
                    p2 = this.point1;
                }
                else
                {
                    p2 = this.point3;
                }
            }
            else
            {
                if (this.point2.X > this.point3.X)
                {
                    p2 = this.point2;
                }
                else
                {
                    p2 = this.point3;
                }
            }
            return ((p2.Y - p1.Y) / (p2.X - p1.X));
        }

        public float getSlopeZ()
        {
            Vector3 p1 = new Vector3();
            Vector3 p2 = new Vector3();
            // p1
            if (this.point1.Y < this.point2.Y)
            {
                if (this.point1.Y < this.point3.Y)
                {
                    p1 = this.point1;
                }
                else
                {
                    p1 = this.point3;
                }
            }
            else
            {
                if (this.point2.Y < this.point3.Y)
                {
                    p1 = this.point2;
                }
                else
                {
                    p1 = this.point3;
                }
            }
            // p2
            if (this.point1.Y > this.point2.Y)
            {
                if (this.point1.Y > this.point3.Y)
                {
                    p2 = this.point1;
                }
                else
                {
                    p2 = this.point3;
                }
            }
            else
            {
                if (this.point2.Y > this.point3.Y)
                {
                    p2 = this.point2;
                }
                else
                {
                    p2 = this.point3;
                }
            }
            return ((p2.Y - p1.Y) / (p2.Y - p1.Y));
        }

        public static bool RayTriangleIntersect(Vector3 ray_origin, Vector3 ray_direction,
                    Vector3 vert0, Vector3 vert1, Vector3 vert2,
                    out float t, out float u, out float v)
        {
            t = 0; u = 0; v = 0;

            Vector3 edge1 = vert1 - vert0;
            Vector3 edge2 = vert2 - vert0;

            Vector3 tvec, pvec, qvec;
            float det, inv_det;

            pvec = Vector3.Cross(ray_direction, edge2);

            det = Vector3.Dot(edge1, pvec);

            if (det > -0.00001f && det < 0.00001f)
                return false;

            inv_det = 1.0f / det;

            tvec = ray_origin - vert0;

            u = Vector3.Dot(tvec, pvec) * inv_det;
            if (u < -0.001f || u > 1.001f)
                return false;

            qvec = Vector3.Cross(tvec, edge1);

            v = Vector3.Dot(ray_direction, qvec) * inv_det;
            if (v < -0.001f || u + v > 1.001f)
                return false;

            t = Vector3.Dot(edge2, qvec) * inv_det;

            if (t <= 0)
                return false;

            return true;
        }
    }
}
