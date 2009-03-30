using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace MPQNav.Collision._3D
{
    class AABB
    {
        public List<Vector3> vertList = new List<Vector3>();
        public List<int> indexList = new List<int>();
        public Vector3 center;
        public Vector3 extents;

        public AABB(List<Vector3> vList)
        {
            this.buildFromVertList(vList);
        }

        public AABB(List<VertexPositionNormalColored> vList)
        {
            if (vList.Count == 0)
            {
                return;
            }
            List<Vector3> tempList = new List<Vector3>();
            for (int i = 0; i < vList.Count; i++)
            {
                tempList.Add(vList[i].Position);
            }

            this.buildFromVertList(tempList);
        }

        private void buildFromVertList(List<Vector3> vList)
        {
            float low_x = vList[0].X;
            float low_y = vList[0].Y;
            float low_z = vList[0].Z;

            float high_x = vList[0].X;
            float high_y = vList[0].Y;
            float high_z = vList[0].Z;

            for (int i = 0; i < vList.Count; i++)
            {
                if (vList[i].X < low_x)
                {
                    low_x = vList[i].X;
                }
                if (vList[i].Y < low_y)
                {
                    low_y = vList[i].Y;
                }
                if (vList[i].Z < low_z)
                {
                    low_z = vList[i].Z;
                }
                if (vList[i].X > high_x)
                {
                    high_x = vList[i].X;
                }
                if (vList[i].Y > high_y)
                {
                    high_y = vList[i].Y;
                }
                if (vList[i].Z > high_z)
                {
                    high_z = vList[i].Z;
                }
            }

            Vector3 v1 = new Vector3(low_x, low_y, low_z);
            Vector3 v8 = new Vector3(high_x, high_y, high_z);

            Vector3 v2 = new Vector3(v8.X, v1.Y, v1.Z);
            Vector3 v3 = new Vector3(v1.X, v8.Y, v1.Z);
            Vector3 v4 = new Vector3(v8.X, v8.Y, v1.Z);
            Vector3 v5 = new Vector3(v1.X, v1.Y, v8.Z);
            Vector3 v6 = new Vector3(v8.X, v1.Y, v8.Z);
            Vector3 v7 = new Vector3(v1.X, v8.Y, v8.Z);

            Vector3[] tempVectorArray = new Vector3[8] { v1, v2, v3, v4, v5, v6, v7, v8 };
            this.vertList.AddRange(tempVectorArray);

            int[] tempIndexArray = new int[36] { 0, 3, 1, 0, 2, 3, 4, 7, 5, 4, 6, 7, 1, 7, 5, 1, 3, 7, 0, 6, 4, 0, 2, 6, 0, 5, 1, 0, 4, 5, 2, 7, 3, 2, 6, 7 };
            this.indexList.AddRange(tempIndexArray);

            this.center = (v8 - v1) * 0.5f;
            this.center += v1;
            this.extents = (v8 - v1) * .5f;
        }

        public AABB(Vector3 v1, Vector3 v8)
        {            
            Vector3 v2 = new Vector3(v8.X, v1.Y, v1.Z);
            Vector3 v3 = new Vector3(v1.X, v8.Y, v1.Z);
            Vector3 v4 = new Vector3(v8.X, v8.Y, v1.Z);
            Vector3 v5 = new Vector3(v1.X, v1.Y, v8.Z);
            Vector3 v6 = new Vector3(v8.X, v1.Y, v8.Z);
            Vector3 v7 = new Vector3(v1.X, v8.Y, v8.Z);

            List<Vector3> vList = new List<Vector3>();
            vList.Add(v1);
            vList.Add(v2);
            vList.Add(v3);
            vList.Add(v4);
            vList.Add(v5);
            vList.Add(v6);
            vList.Add(v7);
            vList.Add(v8);

            this.buildFromVertList(vList);
        }

        public static Boolean Intersects_AABB(AABB AABB_1, AABB AABB_2)
        {
            Vector3 firstMin = AABB_1.vertList[0];
            Vector3 firstMax = AABB_1.vertList[7];
            Vector3 secondMin = AABB_1.vertList[0];
            Vector3 secondMax = AABB_1.vertList[7];
            return  (firstMin.X < secondMax.X) && (firstMax.X > secondMin.X) &&
                    (firstMin.Y < secondMax.Y) && (firstMax.Y > secondMin.Y) &&
                    (firstMin.Z < secondMax.Z) && (firstMax.Z > secondMin.Z);
        }
    }
}
