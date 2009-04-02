using System;
using System.Collections.Generic;
using System.Diagnostics;
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
					vectors.Select(v1 => new VertexPositionNormalColored(v1, Color.Red, Vector3.Up)).ToList();

				br.BaseStream.Position = ofsBoundingTriangles;

				List<int> tempIndices = ReadTriangleList(br, nBoundingTriangles);

				var m2 = new M2 {
					TriangleList = new TriangleList {
						Indices = tempIndices,
						Vertices = tempVertices,
					},
				};

				m2.Transform(mddf.Position, mddf.Rotation, mddf.Scale);

				_m2s.Add(m2);
			}
		}

		private static List<int> ReadTriangleList(BinaryReader br, uint nBoundingTriangles) {
			var tempIndices = new List<int>();
			for(int v = 0; v < nBoundingTriangles; v = v + 3) {
				Int16 int1 = br.ReadInt16();
				Int16 int2 = br.ReadInt16();
				Int16 int3 = br.ReadInt16();

				tempIndices.Add(int3);
				tempIndices.Add(int2);
				tempIndices.Add(int1);
			}
			return tempIndices;
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