using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MPQNav.Chunks;
using MPQNav.Graphics;
using Model=MPQNav.Graphics.Model;

namespace MPQNav.ADT {
	internal class ADT {
	    /// <summary>
		/// Array of MCNK chunks which give the ADT vertex information for this ADT
		/// </summary>
		public MCNK[,] MCNKArray;

		/// <summary>
		/// List of MDDF Chunks which are placement information for M2s
		/// </summary>
		public MDDF[] MDDFList = new MDDF[0];

		/// <summary>
		/// Array of MH20 chunks which give the ADT FLUID vertex information for this ADT
		/// </summary>
		public MH2O[,] MH2OArray;

		/// <summary>
		/// List of MODF Chunks which are placement information for WMOs
		/// </summary>
		public List<MODF> MODFList = new List<MODF>();

		/// <summary>
		/// Version of the ADT
		/// </summary>
		/// <example></example>
		public Int32 Version;

	    private IList<Model> wmos = new List<Model>();
		private IList<Model> m2S = new List<Model>();

	    private TriangleList triangeListH2O;

	    private TriangleList triangeList;

	    private TriangleList GenerateVertexAndIndicesH2O() {
			var vertices = new List<VertexPositionNormalColored>();
			var indices = new List<int>();
			if(MH2OArray != null) {
				float offset_x = (533.33333f / 16) / 8;
				float offset_z = (533.33333f / 16) / 8;

				int VertexCounter = 0;
				int _TempVertexCounter = 0;
				for(int My = 0; My < 16; My++) {
					for(int Mx = 0; Mx < 16; Mx++) {
						float[,] MH2OHeightMap;
						bool[,] MH2ORenderMap;

						_TempVertexCounter = VertexCounter;
						float x = MCNKArray[Mx, My].x;
						float z = MCNKArray[Mx, My].z;
						MH2O mh2O = MH2OArray[Mx, My];

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
		    return new TriangleList(indices, vertices);
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

	    private TriangleList GenerateVertexAndIndices() {
			var vertices = new List<VertexPositionNormalColored>();
			var indices = new List<int>();


			for(int My = 0; My < 16; My++) {
				for(int Mx = 0; Mx < 16; Mx++) {
					MCNK lMCNK = MCNKArray[Mx, My];

					var HolesMap = new bool[4, 4];
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
			return new TriangleList( indices, vertices);
		}

		public void Load()
		{
			wmos = MODFList
				.Select(modf => _wmoLoader.Load(modf))
				.ToList();

			m2S = MDDFList.Select(mmdf => _m2Loader.Load(mmdf))
				.ToList();

			triangeList = GenerateVertexAndIndices();
			triangeListH2O = GenerateVertexAndIndicesH2O();
		}

		private TriangleList triangleList;
		
		private readonly IModelLoader _wmoLoader = new WmoLoader();
		private readonly IModelLoader _m2Loader = new M2Loader();

		public TriangleList TriangleList
	    {
	        get { return triangleList ?? (triangleList = BuildTriangleList()); }
	    }

	    private TriangleList BuildTriangleList()
	    {
	        var col = new TriangleListCollection();
	        // Handle the ADTs
	        col.Add(triangeList);
	        col.Add(triangeListH2O);
	        // Handle the WMOs
	        foreach (Model w in wmos)
	        {
	            col.Add(w.TriangleList);
	        }
	        // Handle the M2s
	        foreach (Model m in m2S)
	        {
	            col.Add(m.TriangleList);
	        }
	        return col.Optimize();
	    }
	}
}