using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.MPQ.ADT;
using MPQNav.Util;

namespace MPQNav.ADT {
	/// <summary>
	/// The ADTManager is responsible for handling all the different ADTs that we are going to be loading up.
	/// </summary>
	internal class ADTManager {
		#region variables

		private const string AdtPath = "World\\Maps\\";

		/// <summary>
		/// List of all ADTs managed by this ADT manager
		/// </summary>
		private readonly List<ADT> _ADTs = new List<ADT>();

		/// <summary>
		/// Continent of the ADT Manager
		/// </summary>
		private readonly string _continent;

		private ITriangleList _triangleList;

		/// <summary>
		/// Boolean variable representing if all the rendering data has been cached.
		/// </summary>
		private Boolean renderCached;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of the ADT manager.
		/// </summary>
		/// <param name="c">Continent of the ADT</param>
		/// <example>ADTManager myADTManager = new ADTManager(continent.Azeroth, "C:\\mpq\\");</example>
		public ADTManager(string c) {
			_continent = c;
		}

		#endregion

		public ITriangleList TriangleList {
			get {
				if(!renderCached) {
					_triangleList = BuildTriangleList();
				}
				return _triangleList;
			}
		}

		/// <summary>
		/// Loads an ADT into the manager.
		/// </summary>
		/// <param name="x">X coordiate of the ADT in the 64 x 64 Grid</param>
		/// <param name="y">Y coordinate of the ADT in the 64 x 64 grid</param>
		public void loadADT(int x, int y) {
			string file = GetAdtFileName(x, y);

			ADT currentADT;
			using(var reader = new BinaryReader(File.OpenRead(file))) {
				currentADT = new ADTChunkFileParser(Path.GetFileName(file), reader).Parse();
			}

			currentADT.LoadWMO();

			currentADT.LoadM2();

			renderCached = false;
			currentADT.TriangeList = currentADT.GenerateVertexAndIndices();
			currentADT.TriangeListH2O = currentADT.GenerateVertexAndIndicesH2O();
			_ADTs.Add(currentADT);
		}

		private string GetAdtFileName(int x, int y) {
			string dir = MpqNavSettings.MpqPath + AdtPath + _continent;
			if(!Directory.Exists(dir)) {
				throw new Exception("Continent data missing");
			}
			string file = String.Format("{0}{1}{2}\\{2}_{3}_{4}.adt", MpqNavSettings.MpqPath, AdtPath, _continent, x, y);
			if(!File.Exists(file)) {
				throw new Exception(String.Format("ADT Doesn't exist: {0}", file));
			}
			return file;
		}

		public ITriangleList BuildTriangleList() {
			// Cycle through each ADT
			var triangleListCollection = new TriangleListCollection();
			foreach(ADT a in _ADTs) {
				// Handle the ADTs
				triangleListCollection.Add(a.TriangeList);
				triangleListCollection.Add(a.TriangeListH2O);
				// Handle the WMOs
				foreach(WMO w in a.WMOs) {
					triangleListCollection.Add(w.TriangleList);
				}
				// Handle the M2s
				foreach(M2 m in a._M2Manager._m2s) {
					triangleListCollection.Add(m.TriangleList);
				}
			}

			ITriangleList list = triangleListCollection.Optimize();

			renderCached = true;

			return list;
		}
	}

	/// <summary>
	/// Enumeration of the different continents available.
	/// </summary>
	public enum ContinentType {
		Azeroth,
		Kalimdor,
		Outland
	}
}