using System;
using System.Collections.Generic;
using System.Linq;
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
using MPQNav.ADT;

namespace MPQNav.MPQ.ADT
{
    class M2Manger
    {
        #region variables
        /// <summary>
        /// List of filenames managed by this M2Manager
        /// </summary>
        private List<String> _names = new List<String>();
        /// <summary>
        /// List of WMOs managed by this WMOManager
        /// </summary>
        public List<MPQNav.ADT.M2> _m2s = new List<MPQNav.ADT.M2>();
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

        public M2Manger()
        {

        }

    	public void Add(string fileName) {
    		_names.Add(fileName);
    	}

    	public void Process(String fileName, MPQNav.ADT.MDDF mddf)
        {
			using(var br = new BinaryReader(File.OpenRead(fileName))) {
				br.BaseStream.Position = 60; //wotlk
				var numberOfVerts = br.ReadUInt32();
				var vertsOffset = br.ReadUInt32();
				var numberOfViews = br.ReadUInt32();
				//UInt32 viewsOffset = br.ReadUInt32(); //now in skins

				br.BaseStream.Position = 216; //wotlk
				var nBoundingTriangles = br.ReadUInt32();
				var ofsBoundingTriangles = br.ReadUInt32();
				var nBoundingVertices = br.ReadUInt32();
				var ofsBoundingVertices = br.ReadUInt32();

				br.BaseStream.Position = ofsBoundingVertices;

				var vectors = ReadVectors(br, nBoundingVertices);
				var tempVertices = vectors.Select(v1 => new VertexPositionNormalColored(v1, Color.Pink, Vector3.Up)).ToList();

				br.BaseStream.Position = ofsBoundingTriangles;

				var tempIndices = new List<int>();
				for(int v = 0; v < nBoundingTriangles; v = v + 3) {
					Int16 int1 = br.ReadInt16();
					Int16 int2 = br.ReadInt16();
					Int16 int3 = br.ReadInt16();

					tempIndices.Add((int)int3);
					tempIndices.Add((int)int2);
					tempIndices.Add((int)int1);
				}

				_m2s.Add(transform(tempVertices, tempIndices, mddf));
			}
    	}

    	private static List<Vector3> ReadVectors(BinaryReader br, uint count) {
    		var vectors = new List<Vector3>();
    		for(int v = 0; v < count; v++) {
    			var x = br.ReadSingle() * -1;
    			var z = br.ReadSingle();
    			var y = br.ReadSingle();
    			vectors.Add(new Vector3(x, y, z));
    		}
    		return vectors;
    	}

    	private MPQNav.ADT.M2 transform(IList<VertexPositionNormalColored> vertices, List<int> indicies, MPQNav.ADT.MDDF mddf)
        {
            MPQNav.ADT.M2 currentM2 = new MPQNav.ADT.M2();
            
            // Real world positions for a transform
            
            currentM2._Vertices.Clear();
            currentM2._Indices.Clear();

            // First we scale
            for (int i = 0; i < vertices.Count; i++)
            {
                float pos_x = (mddf.position.X - 17066.666666666656f) * -1;
                float pos_y = mddf.position.Y;
                float pos_z = (mddf.position.Z - 17066.666666666656f) * -1;
                Vector3 origin = new Vector3(pos_x, pos_y, pos_z);

                float my_x = (float)vertices[i].Position.X + pos_x;
                float my_y = (float)vertices[i].Position.Y + pos_y;
                float my_z = (float)vertices[i].Position.Z + pos_z;
                Vector3 baseVertex = new Vector3(my_x, my_y, my_z);

                Matrix scaleMatrix = Matrix.CreateScale(mddf.scale);

                Vector3 scaledVector = Vector3.Transform(baseVertex - origin, scaleMatrix);
                currentM2._Vertices.Add(new VertexPositionNormalColored(scaledVector, Color.Red, Vector3.Up));
            }
            currentM2._AABB = new MPQNav.Collision._3D.AABB(currentM2._Vertices);
            currentM2._OBB = new MPQNav.Collision._3D.OBB(currentM2._AABB.center, currentM2._AABB.extents, Matrix.CreateRotationY(mddf.orientation_b - 90));

            List<VertexPositionNormalColored> tempVertices = new List<VertexPositionNormalColored>();

            for (int i = 0; i < currentM2._Vertices.Count; i++)
            {

                float pos_x = (mddf.position.X - 17066.666666666656f) * -1;
                float pos_y = mddf.position.Y;
                float pos_z = (mddf.position.Z - 17066.666666666656f) * -1;
                Vector3 origin = new Vector3(pos_x, pos_y, pos_z);

                float my_x = (float)vertices[i].Position.X + pos_x;
                float my_y = (float)vertices[i].Position.Y + pos_y;
                float my_z = (float)vertices[i].Position.Z + pos_z;
                Vector3 baseVertex = new Vector3(my_x, my_y, my_z);

                // Creation the rotations
                float a = mddf.orientation_a * -1 * rad;
                float b = (mddf.orientation_b - 90) * rad;
                float c = mddf.orientation_c * rad;

                // Fancy things to rotate our model
                Matrix rotateY = Matrix.CreateRotationY(b);
                Matrix rotateZ = Matrix.CreateRotationZ(a);
                Matrix rotateX = Matrix.CreateRotationX(c);

                Vector3 rotatedVector = Vector3.Transform(baseVertex - origin, rotateY);
                //rotatedVector = Vector3.Transform(rotatedVector, rotateZ);
                //rotatedVector = Vector3.Transform(rotatedVector, rotateX);
                Vector3 finalVector = rotatedVector + origin;
                tempVertices.Add(new VertexPositionNormalColored(finalVector,Color.Red,Vector3.Up));
            }
            currentM2._Indices.AddRange(indicies);
            currentM2._Vertices = tempVertices;
            return currentM2;
        }

    }
}
