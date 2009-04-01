using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MPQNav.ADT;
using MPQNav.Collision._3D;
using MPQNav.MPQ.ADT.Chunks;

namespace MPQNav.MPQ.ADT {
	internal class M2Manger {
		#region variables

		/// <summary>
		/// List of filenames managed by this M2Manager
		/// </summary>
		private readonly List<String> _names = new List<String>();

		/// <summary>
		/// List of WMOs managed by this WMOManager
		/// </summary>
		public List<M2> _m2s = new List<M2>();

		/// <summary>
		/// 1 degree = 0.0174532925 radians
		/// </summary>
		private float rad = 0.0174532925f;

		#endregion

		#region rendering variables

		/// <summary>
		/// List of indices for rendering
		/// </summary>
		public List<int> _indicies = new List<int>();

		/// <summary>
		/// List of vertices for rendering
		/// </summary>
		public List<VertexPositionNormalColored> _vertices = new List<VertexPositionNormalColored>();

		#endregion

		public void Process(String fileName, MDDF mddf) {
			using(var br = new BinaryReader(File.OpenRead(fileName))) {
				br.BaseStream.Position = 60; //wotlk
				uint numberOfVerts = br.ReadUInt32();
				uint vertsOffset = br.ReadUInt32();
				uint numberOfViews = br.ReadUInt32();
				//UInt32 viewsOffset = br.ReadUInt32(); //now in skins

				br.BaseStream.Position = 216; //wotlk
				uint nBoundingTriangles = br.ReadUInt32();
				uint ofsBoundingTriangles = br.ReadUInt32();
				uint nBoundingVertices = br.ReadUInt32();
				uint ofsBoundingVertices = br.ReadUInt32();

				br.BaseStream.Position = ofsBoundingVertices;

				List<Vector3> vectors = ReadVectors(br, nBoundingVertices);
				List<VertexPositionNormalColored> tempVertices =
					vectors.Select(v1 => new VertexPositionNormalColored(v1, Color.Pink, Vector3.Up)).ToList();

				br.BaseStream.Position = ofsBoundingTriangles;

				var tempIndices = new List<int>();
				for(int v = 0; v < nBoundingTriangles; v = v + 3) {
					Int16 int1 = br.ReadInt16();
					Int16 int2 = br.ReadInt16();
					Int16 int3 = br.ReadInt16();

					tempIndices.Add(int3);
					tempIndices.Add(int2);
					tempIndices.Add(int1);
				}

				_m2s.Add(transform(tempVertices, tempIndices, mddf));
			}
		}

		private static List<Vector3> ReadVectors(BinaryReader br, uint count) {
			var vectors = new List<Vector3>();
			for(int v = 0; v < count; v++) {
				float x = br.ReadSingle() * -1;
				float z = br.ReadSingle();
				float y = br.ReadSingle();
				vectors.Add(new Vector3(x, y, z));
			}
			return vectors;
		}

		private M2 transform(IList<VertexPositionNormalColored> vertices, IEnumerable<int> indicies, MDDF mddf) {
			var currentM2 = new M2();

			// Real world positions for a transform

			currentM2.Vertices.Clear();
			currentM2.Indices.Clear();

			// First we scale
			for(int i = 0; i < vertices.Count; i++) {
				var vertex = vertices[i];
				float pos_x = (mddf.Position.X - 17066.666666666656f) * -1;
				float pos_y = mddf.Position.Y;
				float pos_z = (mddf.Position.Z - 17066.666666666656f) * -1;
				var origin = new Vector3(pos_x, pos_y, pos_z);

				float my_x = vertex.Position.X + pos_x;
				float my_y = vertex.Position.Y + pos_y;
				float my_z = vertex.Position.Z + pos_z;
				var baseVertex = new Vector3(my_x, my_y, my_z);

				Matrix scaleMatrix = Matrix.CreateScale(mddf.Scale);

				Vector3 scaledVector = Vector3.Transform(baseVertex - origin, scaleMatrix);
				currentM2.Vertices.Add(new VertexPositionNormalColored(scaledVector, Color.Red, Vector3.Up));
			}
			currentM2.AABB = new AABB(currentM2.Vertices);
			currentM2.OBB = new OBB(currentM2.AABB.center, currentM2.AABB.extents, Matrix.CreateRotationY(mddf.OrientationB - 90));

			var tempVertices = new List<VertexPositionNormalColored>();

			for(int i = 0; i < currentM2.Vertices.Count; i++) {
				float pos_x = (mddf.Position.X - 17066.666666666656f) * -1;
				float pos_y = mddf.Position.Y;
				float pos_z = (mddf.Position.Z - 17066.666666666656f) * -1;
				var origin = new Vector3(pos_x, pos_y, pos_z);

				float my_x = vertices[i].Position.X + pos_x;
				float my_y = vertices[i].Position.Y + pos_y;
				float my_z = vertices[i].Position.Z + pos_z;
				var baseVertex = new Vector3(my_x, my_y, my_z);

				// Creation the rotations
				float a = mddf.OrientationA * -1 * rad;
				float b = (mddf.OrientationB - 90) * rad;
				float c = mddf.OrientationC * rad;

				// Fancy things to rotate our model
				Matrix rotateY = Matrix.CreateRotationY(b);
				Matrix rotateZ = Matrix.CreateRotationZ(a);
				Matrix rotateX = Matrix.CreateRotationX(c);

				Vector3 rotatedVector = Vector3.Transform(baseVertex - origin, rotateY);
				//rotatedVector = Vector3.Transform(rotatedVector, rotateZ);
				//rotatedVector = Vector3.Transform(rotatedVector, rotateX);
				Vector3 finalVector = rotatedVector + origin;
				tempVertices.Add(new VertexPositionNormalColored(finalVector, Color.Red, Vector3.Up));
			}
			currentM2.Indices.AddRange(indicies);
			currentM2.Vertices = tempVertices;
			return currentM2;
		}

		public void AddM2(MDDF mmdf) {
			string fileName = MpqNavSettings.MpqPath + mmdf.FilePath;
			if(fileName.Substring(fileName.Length - 4) == ".mdx") {
				fileName = fileName.Substring(0, fileName.Length - 4) + ".m2";
			}
			_names.Add(fileName);
			try {
				Process(fileName, mmdf);
			}
			catch {
			}
		}
	}
}