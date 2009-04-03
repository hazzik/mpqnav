using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MPQNav.Collision._3D;
using MPQNav.MPQ.ADT;
using MPQNav.MPQ.ADT.Chunks;
using MPQNav.MPQ.ADT.Chunks.Parsers;
using MPQNav.MPQ.WMO.Chunks;
using MPQNav.MPQ.WMO.Chunks.Parsers;
using MPQNav.Util;

namespace MPQNav.ADT {
	internal class ADT {
		#region Variables

		/// <summary>
		/// Array of MCNK chunks which give the ADT vertex information for this ADT
		/// </summary>
		public MCNK[,] _MCNKArray;

		/// <summary>
		/// List of MDDF Chunks which are placement information for M2s
		/// </summary>
		public MDDF[] _MDDFList = new MDDF[0];

		/// <summary>
		/// Array of MH20 chunks which give the ADT FLUID vertex information for this ADT
		/// </summary>
		public MH2O[,] _MH2OArray;

		/// <summary>
		/// List of MODF Chunks which are placement information for WMOs
		/// </summary>
		public List<MODF> _MODFList = new List<MODF>();

		/// <summary>
		/// Version of the ADT
		/// </summary>
		/// <example></example>
		public Int32 _Version;

		#endregion

		private readonly IList<Model> _wmos = new List<Model>();
		private readonly IList<Model> _m2s = new List<Model>();

		public IList<Model> WMOs {
			get { return _wmos; }
		}

		public TriangleList TriangeListH2O { get; set; }

		public TriangleList TriangeList { get; set; }

		public IList<Model> M2s {
			get { return _m2s; }
		}

		public TriangleList GenerateVertexAndIndicesH2O() {
			var vertices = new List<VertexPositionNormalColored>();
			var indices = new List<int>();
			if(_MH2OArray != null) {
				float offset_x = (533.33333f / 16) / 8;
				float offset_z = (533.33333f / 16) / 8;

				int VertexCounter = 0;
				int _TempVertexCounter = 0;
				for(int My = 0; My < 16; My++) {
					for(int Mx = 0; Mx < 16; Mx++) {
						float[,] MH2OHeightMap;
						bool[,] MH2ORenderMap;

						_TempVertexCounter = VertexCounter;
						float x = _MCNKArray[Mx, My].x;
						float z = _MCNKArray[Mx, My].z;
						MH2O mh2O = _MH2OArray[Mx, My];

						float y_pos = mh2O.heightLevel1;
						if(mh2O.used) {
							Color clr = GetColor(mh2O.type);
							MH2OHeightMap = mh2O.GetMapHeightsMatrix();
							MH2ORenderMap = mh2O.GetRenderBitMapMatrix();
							for(int r = 0; r < 9; r++) {
								for(int c = 0; c < 9; c++) {
									float x_pos = x - (c * offset_x);
									float z_pos = z - (r * offset_z);
									if(((r >= mh2O.yOffset) && ((r - mh2O.yOffset) <= mh2O.height)) &&
									   ((c >= mh2O.xOffset) && ((c - mh2O.xOffset) <= mh2O.width))) {
										y_pos = MH2OHeightMap[r - mh2O.yOffset, c - mh2O.xOffset]; // +_MH2O.heightLevel1;
										var position = new Vector3(x_pos, y_pos, z_pos);

										vertices.Add(new VertexPositionNormalColored(position, clr, Vector3.Up));
										_TempVertexCounter++;
									}
								}
							}

							for(int r = mh2O.yOffset; r < mh2O.yOffset + mh2O.height; r++) {
								for(int c = mh2O.xOffset; c < mh2O.xOffset + mh2O.width; c++) {
									int row = r - mh2O.yOffset;
									int col = c - mh2O.xOffset;

									//if ((MH2ORenderMap[row, c]) || ((_MH2O.height == 8) && (_MH2O.width == 8)))
									{
										indices.Add(VertexCounter + ((row + 1) * (mh2O.width + 1) + col));
										indices.Add(VertexCounter + (row * (mh2O.width + 1) + col));
										indices.Add(VertexCounter + (row * (mh2O.width + 1) + col + 1));
										indices.Add(VertexCounter + ((row + 1) * (mh2O.width + 1) + col + 1));
										indices.Add(VertexCounter + ((row + 1) * (mh2O.width + 1) + col));
										indices.Add(VertexCounter + (row * (mh2O.width + 1) + col + 1));
									}
								}
							}
							VertexCounter = _TempVertexCounter;
						}
					}
				}
			}
			return new TriangleList {
			                        	Indices = indices,
			                        	Vertices = vertices,
			                        };
		}

		private static Color GetColor(MH2O.FluidType fluidType) {
			switch(fluidType) {
				case MH2O.FluidType.Lake:
					return Color.Blue;
				case MH2O.FluidType.Lava:
					return Color.Red;
				case MH2O.FluidType.Oceans:
					return Color.Coral;
			}
			return Color.Green;
		}

		public TriangleList GenerateVertexAndIndices() {
			var vertices = new List<VertexPositionNormalColored>();
			var indices = new List<int>();


			for(int My = 0; My < 16; My++) {
				for(int Mx = 0; Mx < 16; Mx++) {
					MCNK lMCNK = _MCNKArray[Mx, My];

					var HolesMap = new bool[4,4];
					if(lMCNK.holes > 0) {
						HolesMap = lMCNK.GetHolesMap();
					}

					#region indexing

					for(int row = 0; row < 8; row++) {
						for(int col = 0; col < 8; col++) {
							if(!HolesMap[row / 2, col / 2]) {
								/* The order metter*/
								/*This 3 index add the up triangle
                                *
                                *0--1--2
                                *| /| /
                                *|/ |/ 
                                *9  10 11
                                */

								indices.Add(vertices.Count + ((row + 1) * (8 + 1) + col)); //9 ... 10
								indices.Add(vertices.Count + (row * (8 + 1) + col)); //0 ... 1
								indices.Add(vertices.Count + (row * (8 + 1) + col + 1)); //1 ... 2

								/*This 3 index add the low triangle
                                 *
                                 *0  1   2
                                 *  /|  /|
                                 * / | / |
                                 *9--10--11
                                 */

								indices.Add(vertices.Count + ((row + 1) * (8 + 1) + col + 1));
								indices.Add(vertices.Count + ((row + 1) * (8 + 1) + col));
								indices.Add(vertices.Count + (row * (8 + 1) + col + 1));
							}

							#endregion
						}
					}

					//this.Indices.AddRange(triangleListIndices);
					const float offset_x = (533.33333f / 16) / 8;
					const float offset_z = (533.33333f / 16) / 8;

					var LowResMap = lMCNK._MCVT.GetLowResMapMatrix();
					var LowResNormal = lMCNK._MCNR.GetLowResNormalMatrix();

					for(int r = 0; r < 9; r++) {
						for(int c = 0; c < 9; c++) {
							float x_pos = lMCNK.x - (c * offset_x);
							float z_pos = lMCNK.z - (r * offset_z);
							float y_pos = LowResMap[r, c] + lMCNK.y;


							var normal = LowResNormal[r, c];
							float cosAngle = Vector3.Dot(Vector3.Up, normal);
							float angle = MathHelper.ToDegrees((float)Math.Acos(cosAngle));

							var position = new Vector3(x_pos, y_pos, z_pos);
							vertices.Add(new VertexPositionNormalColored(position, angle > 50.0 ? Color.Brown : Color.Green, normal));
						}
					}
				}
			}
			return new TriangleList {
			                        	Indices = indices,
			                        	Vertices = vertices,
			                        };
		}

		public void LoadWMO() {
			foreach(MODF modf in _MODFList) {
				var wmo = LoadWMO(modf.FileName);
				_wmos.Add(wmo.Transform(modf.Position, modf.Rotation, 1.0f));
			}
		}

		/// <summary> Loads WMO from file </summary>
		/// <param name="fileName">Full name of file of the WMO</param>
		/// <returns>Loaded WMO</returns>
		private static Model LoadWMO(string fileName) {
			var path = MpqNavSettings.MpqPath + fileName;
			if(!File.Exists(path)) {
				throw new Exception(String.Format("File does not exist: {0}", path));
			}

			MOHD mohd;
			using(var br = new BinaryReader(File.OpenRead(path))) {
				int version = new MVERChunkParser(br, br.BaseStream.Position).Parse();
				mohd = new MOHDChunkParser(br, br.BaseStream.Position).Parse();
			}

			var list = new TriangleListCollection();
			for(int wmoGroup = 0; wmoGroup < mohd.GroupsCount; wmoGroup++) {
				list.Add(LoadWMOSub(String.Format("{0}_{1:D3}.wmo", fileName.Substring(0, fileName.Length - 4), wmoGroup), wmoGroup));
			}

			return new Model(list);
		}

		public void LoadM2() {
			foreach(MDDF mmdf in _MDDFList) {
				var m2 = LoadM2(mmdf.FilePath);
				_m2s.Add(m2.Transform(mmdf.Position, mmdf.Rotation, mmdf.Scale));
			}
		}

		/// <summary>
		/// Gets a WMO_Sub from the WMO Group file
		/// </summary>
		/// <param name="wmoGroup">Current index in the WMO Group</param>
		/// <param name="fileName">Full Filename of the WMO_Sub</param>
		/// <returns></returns>
		public static ITriangleList LoadWMOSub(string fileName, int wmoGroup) {
			var path = MpqNavSettings.MpqPath + fileName;
			if(!File.Exists(path)) {
				throw new Exception(String.Format("File does not exist: {0}", path));
			}

			using(var reader = new BinaryReader(File.OpenRead(path))) {
				var indices = new MOVIChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVI").StartPosition).Parse();
				var vectors = new MOVTChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVT").StartPosition).Parse();
				var normals = new MONRChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MONR").StartPosition).Parse();
				var vertices = new List<VertexPositionNormalColored>();
				for(var i = 0; i < vectors.Count; i++) {
					vertices.Add(new VertexPositionNormalColored(vectors[i], Color.Yellow, normals[i]));
				}
				return new TriangleList {
					Indices = indices,
					Vertices = vertices,
				};
			}
		}

		public static Model LoadM2(string fileName) {
			string path = MpqNavSettings.MpqPath + fileName;
			if(path.Substring(path.Length - 4) == ".mdx") {
				path = path.Substring(0, path.Length - 4) + ".m2";
			}
			if(!File.Exists(path)) {
				throw new Exception(String.Format("File does not exist: {0}", path));
			}

			using(var br = new BinaryReader(File.OpenRead(path))) {
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

				var tempIndices = ReadTriangleList(br, nBoundingTriangles);

				var list = new TriangleList {
				                            	Indices = tempIndices,
				                            	Vertices = tempVertices,
				                            };
				return new Model(list);
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
	}
}