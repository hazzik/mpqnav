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

namespace MPQNav.Pathfinding
{
    class PathFinder
    {
        public List<MPQNav.Pathfinding.PathNode> nodeList = new List<PathNode>();


        public Boolean exists(float x, float z)
        {
            foreach (PathNode p in this.nodeList)
            {
                if (p.position.X == x && p.position.Z == z)
                {
                    return true;
                }
            }
            return false;
        }

        public Boolean exists(float x, float y, float z)
        {
            Vector3 v = new Vector3(x,y,z);
            foreach (PathNode p in this.nodeList)
            {
                if (p.position == v)
                {
                    return true;
                }
            }
            return false;
        }

        public Vector3 findVector(float x, float z)
        {
            foreach (PathNode p in this.nodeList)
            {
                if (p.position.X == x && p.position.Z == z)
                {
                    return p.position;
                }
            }
            return Vector3.Zero;
        }

        public Vector3 findVector(float x, float y, float z)
        {
            Vector3 v = new Vector3(x, y, z);
            foreach (PathNode p in this.nodeList)
            {
                if (p.position == v)
                {
                    return p.position;
                }
            }
            return Vector3.Zero;
        }

    }
}
