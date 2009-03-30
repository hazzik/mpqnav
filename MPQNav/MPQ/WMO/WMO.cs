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

namespace MPQNav.MPQ.ADT
{
    /// <summary>
    /// Class for the WMO Group File
    /// </summary>
    class WMO
    {
        /// <summary>
        /// AABB For the WMO
        /// </summary>
        public MPQNav.Collision._3D.AABB _AABB;
        /// <summary>
        /// Total number of groups for the WMO
        /// </summary>
        public int total_groups = 1;
        /// <summary>
        /// Name of the WMO Group file
        /// </summary>
        public String name;
        /// <summary>
        /// List containg all the WMO Sub-Chunks for this WMO Group File
        /// </summary>
        private List<WMO_Sub> _wmoSubList = new List<WMO_Sub>();
        /// <summary>
        /// The Orientated Bounding Box for this WMO
        /// </summary>
        public MPQNav.Collision._3D.OBB _OBB;

        public int WMO_SubCount
        {
            get
            {
                return this._wmoSubList.Count;
            }
        }

        #region Rendering Variables
        /// <summary>
        /// List of vertices used for rendering this WMO in World Space
        /// </summary>
        public List<VertexPositionNormalColored> _Vertices = new List<VertexPositionNormalColored>();
        /// <summary>
        /// List of indicies used for rendering this WMO in World Space
        /// </summary>
        public List<int> _Indices = new List<int>();
        #endregion

        public WMO()
        {

        }

        public WMO(String name)
        {
            this.name = name;
        }

        public void createAABB(Vector3 v_min, Vector3 v_max)
        {
            this._AABB = new MPQNav.Collision._3D.AABB(v_min, v_max);
        }

        public void addWMO_Sub(WMO_Sub wmoSub)
        {
            this._wmoSubList.Add(wmoSub);
        }

        public WMO_Sub getWMO_Sub(int index)
        {
                return this._wmoSubList[index];                       
        }

        public void clearCollisionData()
        {
            this._OBB = new MPQNav.Collision._3D.OBB();
            this._Vertices.Clear();
            this._Indices.Clear();
        }

        public void addVertex(Vector3 vec)
        {
            this._Vertices.Add(new VertexPositionNormalColored(vec, Color.Yellow, Vector3.Up));
        }

        public void addIndex(int index)
        {
            this._Indices.Add(index);
        }

        public void addIndex(short index)
        {
            this._Indices.Add((int)index);
        }

        public class WMO_Sub
        {
            public MOVT _MOVT = new MOVT();
            public MONR _MONR = new MONR();
            public MOVI _MOVI = new MOVI();
            public int _index;

            public WMO_Sub(int index)
            {
                this._index = index;
            }

            public class MOVT
            {
                // Verticies
                public List<Vector3> verticiesList = new List<Vector3>();
            }
            public class MONR
            {
                // Normals
                public List<Vector3> normalsList = new List<Vector3>();
            }
            public class MOVI
            {
                // Triangle Indicies
                public List<short> indiciesList = new List<short>();
            }
        }
    }
}
