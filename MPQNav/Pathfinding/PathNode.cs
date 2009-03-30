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
    class PathNode
    {
        public float weight = 100;
        public enum type
        {
            PathNode_Water,
            PathNode_Road,
            PathNode_Building,
            PathNode_Normal
        }
        /// <summary>
        /// List of connected Vectors
        /// </summary>
        public List<PathNode> Connections = new List<PathNode>();
        /// <summary>
        /// Position of this node
        /// </summary>
        public Vector3 position = new Vector3();
    }
}
