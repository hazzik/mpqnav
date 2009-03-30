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
using MPQNav.Util;

namespace MPQNav.MPQ.ADT
{
    class WMOManager
    {
        #region variables
        /// <summary>
        /// List of filenames managed by this WMOManager
        /// </summary>
        private List<String> _names = new List<String>();
        /// <summary>
        /// List of WMOs managed by this WMOManager
        /// </summary>
        public List<WMO> _wmos = new List<WMO>();
        /// <summary>
        /// 1 degree = 0.0174532925 radians
        /// </summary>
        private float rad = 0.0174532925f;
        #endregion

        #region rendering variables
        /// <summary>
        /// List of vertices for rendering
        /// </summary>
        public List<VertexPositionNormalColored> vertices = new List<VertexPositionNormalColored>();
        /// <summary>
        /// List of indices for rendering
        /// </summary>
        public List<int> indicies = new List<int>();
        #endregion

        #region consructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public WMOManager()
        {

        }
        #endregion
        /// <summary>
        /// Adds a WMO Filename to the Manager
        /// </summary>
        /// <param name="name">The filename to add</param>
        public void addFileName(String name)
        {
            this._names.Add(name);
        }

        /// <summary>
        /// Adds a WMO to the manager
        /// </summary>
        /// <param name="fileName">Filename of the WMO from the Adt - i.e. world\wmo\azeroth\buildings\redridge_stable\redridge_stable.wmo</param>
        /// <param name="filePath">Base path to where the MPQ is extract - i.e. c:\mpq\</param>
        /// <param name="currentMODF">MODF (placement informatio for this WMO)</param>
        public void addWMO(String fileName, String filePath, MPQNav.ADT.MODF currentMODF)
        {
            if (File.Exists(filePath + fileName))
            {
                this.addFileName(fileName);

                BinaryReader br = new BinaryReader(File.OpenRead(filePath + fileName));
                WMO currentWMO = new WMO();
                currentWMO.name = fileName;
                br.ReadBytes(20); // Skip the header
                UInt32 nTextures = br.ReadUInt32();
                UInt32 nGroups = br.ReadUInt32(); // This is the number of "sub-wmos" or group files that we need to read
                UInt32 nPortals = br.ReadUInt32();
                UInt32 nLights = br.ReadUInt32();
                UInt32 nModels = br.ReadUInt32();
                UInt32 nDoodads = br.ReadUInt32();
                UInt32 nSets = br.ReadUInt32();
                UInt32 ambientColor = br.ReadUInt32();

                UInt32 WMOID = br.ReadUInt32(); // Column 2 in the WMOAreaTable.dbc

                float bb1_x = (float)(br.ReadSingle() * -1);
                float bb1_z = (float)br.ReadSingle();
                float bb1_y = (float)br.ReadSingle();
                float bb2_x = (float)(br.ReadSingle() * -1);
                float bb2_z = (float)br.ReadSingle();
                float bb2_y = (float)br.ReadSingle();

                currentWMO.createAABB(new Vector3(bb1_x, bb1_y, bb1_z), new Vector3(bb2_x, bb2_y, bb2_z));
                currentWMO.total_groups = (int)nGroups;
                for (int wmoGroup = 0; wmoGroup < nGroups; wmoGroup++)
                {
                    String currentFileName = currentWMO.name.Substring(0, currentWMO.name.Length - 4) + "_" + wmoGroup.ToString().PadLeft(3, "0".ToCharArray()[0]) + ".wmo";
                    currentWMO.addWMO_Sub(this.processWMOSub(filePath, currentFileName , wmoGroup));
                }
                this.transformWMO(new Vector3(currentMODF.OrientationA, currentMODF.OrientationB, currentMODF.OrientationC), currentMODF.Position, currentWMO);
                this._wmos.Add(currentWMO);
            }
            else
            {
                throw new Exception("File does not exist: " + filePath + fileName);
            }
        }

        public void transformWMO(Vector3 rotation, Vector3 position, WMO currentWMO)
        {
            currentWMO.clearCollisionData();

            float pos_x = (position.X - 17066.666666666656f) * -1;
            float pos_y = position.Y;
            float pos_z = (position.Z - 17066.666666666656f) * -1;

            Vector3 origin = new Vector3(pos_x, pos_y, pos_z);

            Matrix rotateY = Matrix.CreateRotationY((rotation.Y - 90) * rad);
            Matrix rotateZ = Matrix.CreateRotationZ(rotation.X * -1 * rad);
            Matrix rotateX = Matrix.CreateRotationX(rotation.Z * rad);

            int offset = 0;
            
            for (int i = 0; i < currentWMO.WMO_SubCount; i++)
            {
                WMO.WMO_Sub currentSub = currentWMO.getWMO_Sub(i);
                for (int v = 0; v < currentSub._MOVT.verticiesList.Count; v++)
                {
                    Vector3 baseVertex = currentSub._MOVT.verticiesList[v] + origin;
                    Vector3 rotatedVector = Vector3.Transform(baseVertex - origin, rotateY);
                    Vector3 finalVector = rotatedVector + origin;

                    currentWMO.addVertex(finalVector);
                    
                }
                for (int index = 0; index < currentSub._MOVI.indiciesList.Count; index++)
                {
                    currentWMO.addIndex(currentSub._MOVI.indiciesList[index] + offset);
                }
                offset = currentWMO._Vertices.Count;
            }

            // Generate the OBB
            currentWMO._OBB = new MPQNav.Collision._3D.OBB(currentWMO._AABB.center, currentWMO._AABB.extents, rotateY);
        }


        /// <summary>
        /// Gets a WMO_Sub from the WMO Group file
        /// </summary>
        /// <param name="filePath">File path to the WMO_Sub WITH TRAILING SLASH</param>
        /// <param name="fileName">Filename for the WMO_Sub</param>
        /// <param name="group_index">Current index in the WMO Group</param>
        /// <returns></returns>
        public WMO.WMO_Sub processWMOSub(String filePath, String fileName, int group_index)
        {
            if (File.Exists(filePath + fileName))
            {
                WMO.WMO_Sub currentWMOSub = new WMO.WMO_Sub(group_index);
                BinaryReader br = new BinaryReader(File.OpenRead(filePath + fileName));

                int offsMOVI = (int)FileChunkHelper.SearchChunk(br, "MOVI").ChunkStartPosition;
                    //MPQ.findChunk(br, "IVOM");

                int offsMOVT = (int)FileChunkHelper.SearchChunk(br, "MOVT").ChunkStartPosition;
                    //MPQ.findChunk(br, "TVOM");

                int offsMONR = (int)FileChunkHelper.SearchChunk(br, "MONR").ChunkStartPosition;
                    //MPQ.findChunk(br, "RNOM");

                int offsMOTV = (int)FileChunkHelper.SearchChunk(br, "MOVT").ChunkStartPosition;
                    //MPQ.findChunk(br, "VTOM");
                this.processMOVI(br, offsMOVI, offsMOVT, currentWMOSub);
                this.processMOVT(br, offsMOVT, offsMONR, currentWMOSub);
                this.processMONR(br, offsMONR, offsMOTV, currentWMOSub);
                return currentWMOSub;
            }
            else
            {
                throw new Exception("File does not exist: " + filePath + fileName);
            }
        }
        /// <summary>
        /// WMO Vertex Index List
        /// </summary>
        /// <param name="br">Binary Reader with the WMO Loaded</param>
        /// <param name="start_offset">Starting offset in the reader for the MOVI Chunk</param>
        /// <param name="end_offset">Ending offset in the reader for the MOVI Chunk</param>
        /// <param name="currentWMOSUB">Current working WMO_Sub</param>
        public void processMOVI(BinaryReader br, int start_offset, int end_offset, WMO.WMO_Sub currentWMOSUB)
        {
            br.BaseStream.Position = start_offset + 8;
            while (br.BaseStream.Position < end_offset)
            {
                Int16 one, two, three;
                one = br.ReadInt16();
                two = br.ReadInt16();
                three = br.ReadInt16();

                currentWMOSUB._MOVI.indiciesList.Add(three);
                currentWMOSUB._MOVI.indiciesList.Add(two);
                currentWMOSUB._MOVI.indiciesList.Add(one);
            }
        }
        /// <summary>
        /// WMO Vertex List
        /// </summary>
        /// <param name="br">Binary reader with the WMO Loaded</param>
        /// <param name="start_offset">Starting offset in the binary reader for the MOVT Chunk</param>
        /// <param name="end_offset">Ending offset in the binary reader for the MOVT Chunk</param>
        /// <param name="currentWMOSUB">Current working WMO_Sub</param>
        public void processMOVT(BinaryReader br, int start_offset, int end_offset, WMO.WMO_Sub currentWMOSUB)
        {
            br.BaseStream.Position = start_offset + 8;
            while (br.BaseStream.Position < end_offset)
            {
                float vect_x = (float)(br.ReadSingle() * -1);
                float vect_z = (float)br.ReadSingle();
                float vect_y = (float)br.ReadSingle();
                currentWMOSUB._MOVT.verticiesList.Add(new Vector3(vect_x, vect_y, vect_z));
            }
        }
        /// <summary>
        /// WMO Normal Information
        /// </summary>
        /// <param name="br">Binary reader with the WMO loaded</param>
        /// <param name="start_offset">Starting offset for the MONR chunk</param>
        /// <param name="end_offset">Ending offset for the MONR Chunk</param>
        /// <param name="currentWMOSUB">Current working WMO Sub</param>
        public void processMONR(BinaryReader br, int start_offset, int end_offset, WMO.WMO_Sub currentWMOSUB)
        {
            br.BaseStream.Position = start_offset + 8;
            while (br.BaseStream.Position < end_offset)
            {
                float vect_x = (float)(br.ReadSingle() * -1);
                float vect_z = (float)br.ReadSingle();
                float vect_y = (float)br.ReadSingle();
                currentWMOSUB._MONR.normalsList.Add(new Vector3(vect_x, vect_y, vect_z));
            }
        }
        /// <summary>
        /// Gets the filename of a WMO at the given index 
        /// </summary>
        /// <param name="index">Index of the Filename</param>
        /// <returns>The filename upon success, INVALID upon failure</returns>
        public String getFilename(int index)
        {
            if (this._names.Count > index)
            {
                return this._names[index];
            }
            else
            {
                return "INVALID";
            }
        }

    }
}
