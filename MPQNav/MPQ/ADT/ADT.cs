using System;
using System.Collections.Generic;
using System.IO;
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
		/// The continent of the ADT
		/// </summary>
		private ContinentType _continent;

		/// <summary>
		/// Filename of the ADT
		/// </summary>
		/// <example>Azeroth_32_32.adt</example>
		public String _FileName;

		/// <summary>
		/// The manager for all M2s used by this ADT
		/// </summary>
		public M2Manger _M2Manager = new M2Manger();

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

		/// <summary>
		/// The X offset of the map in the 64 x 64 grid
		/// </summary>
		private int _x;

		/// <summary>
		/// The Y offset of the map in the 64 x 64 grid
		/// </summary>
		private int _y;

		#endregion

		#region Rendering Variables

		private IList<int> _indices; // = new List<int>();
		private IList<VertexPositionNormalColored> _vertices; // = new List<VertexPositionNormalColored>();

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of the ADT class
		/// </summary>
		/// <param name="fName">Filename of just the ADT</param>
		/// <example>ADT myADT = new ADT("Azeroth_32_32.adt");</example>
		public ADT(String fName) {
			String[] fName_split = fName.Split(Convert.ToChar("_"));
			_continent = (ContinentType)Enum.Parse(typeof(ContinentType), fName_split[0], true);
			_FileName = fName;
			_x = Int32.Parse(fName_split[1]);
			_y = Int32.Parse(fName_split[2].Substring(0, (fName_split[2].Length - 4)));
		}

		#endregion

		private readonly IList<WMO> _wmos = new List<WMO>();

		public IList<WMO> WMOs {
			get { return _wmos; }
		}

		public TriangleList TriangeListH2O { get; set; }

		public TriangleList TriangeList { get; set; }

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

					float[,] LowResMap = lMCNK._MCVT.GetLowResMapMatrix();
					Vector3[,] LowResNormal = lMCNK._MCNR.GetLowResNormalMatrix();

					for(int r = 0; r < 9; r++) {
						for(int c = 0; c < 9; c++) {
							float x_pos = lMCNK.x - (c * offset_x);
							float z_pos = lMCNK.z - (r * offset_z);
							float y_pos = LowResMap[r, c] + lMCNK.y;

							Color _clr = Color.Green;

							Vector3 theNormal = LowResNormal[r, c];
							float cosAngle = Vector3.Dot(Vector3.Up, theNormal);
							float angle = MathHelper.ToDegrees((float)Math.Acos(cosAngle));
							if(angle > 50.0) {
								_clr = Color.Brown;
							}
							var position = new Vector3(x_pos, y_pos, z_pos);
							vertices.Add(new VertexPositionNormalColored(position, _clr, Vector3.Up));
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
				AddWMO(modf);
			}
		}

		/// <summary>
		/// Adds a WMO to the manager
		/// </summary>
		/// <param name="modf">MODF (placement informatio for this WMO)</param>
		public void AddWMO(MODF modf) {
			if(!File.Exists(MpqNavSettings.MpqPath + modf.FileName)) {
				throw new Exception("File does not exist: " + MpqNavSettings.MpqPath + modf.FileName);
			}

			var br = new BinaryReader(File.OpenRead(MpqNavSettings.MpqPath + modf.FileName));
			int version = new MVERChunkParser(br, br.BaseStream.Position).Parse();
			MOHD mohd = new MOHDChunkParser(br, br.BaseStream.Position).Parse();

			var currentWMO = new WMO();
			currentWMO.Name = modf.FileName;
			currentWMO.AABB = new AABB(mohd.BoundingBox1, mohd.BoundingBox2);
			currentWMO.TotalGroups = (int)mohd.GroupsCount;
			for(int wmoGroup = 0; wmoGroup < mohd.GroupsCount; wmoGroup++) {
				string currentFileName = String.Format("{0}_{1:D3}.wmo", currentWMO.Name.Substring(0, currentWMO.Name.Length - 4), wmoGroup);
				currentWMO.WmoSubList.Add(ProcessWMOSub(currentFileName, wmoGroup));
			}
			currentWMO.Transform(modf.Position, modf.Rotation, MathHelper.ToRadians(1));
			WMOs.Add(currentWMO);
		}

		public void LoadM2() {
			foreach(MDDF mmdf in _MDDFList) {
				_M2Manager.AddM2(mmdf);
			}
		}

		/// <summary>
		/// Gets a WMO_Sub from the WMO Group file
		/// </summary>
		/// <param name="wmoGroup">Current index in the WMO Group</param>
		/// <param name="fileName">Full Filename of the WMO_Sub</param>
		/// <returns></returns>
		public static WMO.WMO_Sub ProcessWMOSub(string fileName, int wmoGroup) {
			string path = MpqNavSettings.MpqPath + fileName;
			if(!File.Exists(path)) {
				throw new Exception("File does not exist: " + path);
			}

			var currentWMOSub = new WMO.WMO_Sub(wmoGroup);

			using(var reader = new BinaryReader(File.OpenRead(path))) {
				currentWMOSub._MOVI = new MOVIChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVI").StartPosition).Parse();
				currentWMOSub._MOVT = new MOVTChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVT").StartPosition).Parse();
				currentWMOSub._MONR = new MONRChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MONR").StartPosition).Parse();
			}

			return currentWMOSub;
		}
	}
}