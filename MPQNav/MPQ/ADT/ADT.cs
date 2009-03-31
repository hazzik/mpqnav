using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MPQNav.MPQ.ADT;
using MPQNav.MPQ.ADT.Chunks;

namespace MPQNav.ADT {
	internal class ADT {
		#region Variables

		/// <summary>
		/// The continent of the ADT
		/// </summary>
		private ADTManager.ContinentType _continent;

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
		/// The Manager for all WMOs used by this ADT
		/// </summary>
		private WMOManager _wmoManager = new WMOManager();

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

		public List<int> H2OIndicies; // = new List<int>();
		public List<VertexPositionNormalColored> H2OVertices; // = new List<VertexPositionNormalColored>();
		public List<int> Indicies; // = new List<int>();
		public List<VertexPositionNormalColored> Vertices; // = new List<VertexPositionNormalColored>();

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of the ADT class
		/// </summary>
		/// <param name="fName">Filename of just the ADT</param>
		/// <example>ADT myADT = new ADT("Azeroth_32_32.adt");</example>
		public ADT(String fName) {
			String[] fName_split = fName.Split(Convert.ToChar("_"));
			_continent = (ADTManager.ContinentType)Enum.Parse(typeof(ADTManager.ContinentType), fName_split[0], true);
			_FileName = fName;
			_x = Int32.Parse(fName_split[1]);
			_y = Int32.Parse(fName_split[2].Substring(0, (fName_split[2].Length - 4)));
		}

		#endregion

		public WMOManager WMOManager {
			get {
				if(_wmoManager == null) {
					_wmoManager = new WMOManager();
				}
				return _wmoManager;
			}
		}

		public void GenerateVertexAndIndicesH2O() {
			var vertices = new List<VertexPositionNormalColored>();
			var indices = new List<int>();
			float[,] MH2OHeightMap = null;
			bool[,] MH2ORenderMap = null;
			float offset_x = (533.33333f / 16) / 8;
			float offset_z = (533.33333f / 16) / 8;

			int VertexCounter = 0;
			int _TempVertexCounter = 0;
			for(int My = 0; My < 16; My++) {
				for(int Mx = 0; Mx < 16; Mx++) {
					_TempVertexCounter = VertexCounter;
					float x = _MCNKArray[Mx, My].x;
					float z = _MCNKArray[Mx, My].z;
					MH2O _MH2O = _MH2OArray[Mx, My];

					float y_pos = _MH2O.heightLevel1;
					if(_MH2O.used) {
						Color clr = GetColor(_MH2O.type);
						MH2OHeightMap = _MH2O.GetMapHeightsMatrix();
						MH2ORenderMap = _MH2O.GetRenderBitMapMatrix();
						for(int r = 0; r < 9; r++) {
							for(int c = 0; c < 9; c++) {
								float x_pos = x - (c * offset_x);
								float z_pos = z - (r * offset_z);
								if(((r >= _MH2O.yOffset) && ((r - _MH2O.yOffset) <= _MH2O.height)) &&
								   ((c >= _MH2O.xOffset) && ((c - _MH2O.xOffset) <= _MH2O.width))) {
									y_pos = MH2OHeightMap[r - _MH2O.yOffset, c - _MH2O.xOffset]; // +_MH2O.heightLevel1;
									var position = new Vector3(x_pos, y_pos, z_pos);

									vertices.Add(new VertexPositionNormalColored(position, clr, Vector3.Up));
									_TempVertexCounter++;
								}
							}
						}

						for(int r = _MH2O.yOffset; r < _MH2O.yOffset + _MH2O.height; r++) {
							for(int c = _MH2O.xOffset; c < _MH2O.xOffset + _MH2O.width; c++) {
								int row = r - _MH2O.yOffset;
								int col = c - _MH2O.xOffset;

								//if ((MH2ORenderMap[row, c]) || ((_MH2O.height == 8) && (_MH2O.width == 8)))
								{
									indices.Add(VertexCounter + ((row + 1) * (_MH2O.width + 1) + col));
									indices.Add(VertexCounter + (row * (_MH2O.width + 1) + col));
									indices.Add(VertexCounter + (row * (_MH2O.width + 1) + col + 1));
									indices.Add(VertexCounter + ((row + 1) * (_MH2O.width + 1) + col + 1));
									indices.Add(VertexCounter + ((row + 1) * (_MH2O.width + 1) + col));
									indices.Add(VertexCounter + (row * (_MH2O.width + 1) + col + 1));
								}
							}
						}
						VertexCounter = _TempVertexCounter;
					}
				}
			}
			H2OVertices = vertices;
			H2OIndicies = indices;
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

		public void GenerateVertexAndIndices() {
			Vertices = new List<VertexPositionNormalColored>();
			Indicies = new List<int>();


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

								Indicies.Add(Vertices.Count + ((row + 1) * (8 + 1) + col)); //9 ... 10
								Indicies.Add(Vertices.Count + (row * (8 + 1) + col)); //0 ... 1
								Indicies.Add(Vertices.Count + (row * (8 + 1) + col + 1)); //1 ... 2

								/*This 3 index add the low triangle
                                 *
                                 *0  1   2
                                 *  /|  /|
                                 * / | / |
                                 *9--10--11
                                 */

								Indicies.Add(Vertices.Count + ((row + 1) * (8 + 1) + col + 1));
								Indicies.Add(Vertices.Count + ((row + 1) * (8 + 1) + col));
								Indicies.Add(Vertices.Count + (row * (8 + 1) + col + 1));
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
							Vertices.Add(new VertexPositionNormalColored(position, _clr, Vector3.Up));
						}
					}
				}
			}
		}
	}
}