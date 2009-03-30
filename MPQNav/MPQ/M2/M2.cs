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

namespace MPQNav.ADT
{
    class M2
    {
        /// <summary>
        /// AABB For the WMO
        /// </summary>
        public MPQNav.Collision._3D.AABB _AABB;
        /// <summary>
        /// The Orientated Bounding Box for this WMO
        /// </summary>
        public MPQNav.Collision._3D.OBB _OBB;

        #region Rendering Variables
        /// <summary>
        /// List of vertices used for rendering this M2 in World Space
        /// </summary>
        public List<VertexPositionNormalColored> _Vertices = new List<VertexPositionNormalColored>();
        /// <summary>
        /// List of indicies used for rendering this M2 in World Space
        /// </summary>
        public List<int> _Indices = new List<int>();
        #endregion
    }
}
