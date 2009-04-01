using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
		private readonly ContinentType _continent;

		/// <summary>
		/// Boolean result stating if this manager is loaded or not.
		/// </summary>
		private readonly Boolean loaded;

		private int[] indicesCachedADT;

		/// <summary>
		/// Boolean variable representing if all the rendering data has been cached.
		/// </summary>
		private Boolean renderCached;

		private VertexPositionNormalColored[] verticesCachedADT;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of the ADT manager.
		/// </summary>
		/// <param name="c">Continent of the ADT</param>
		/// <example>ADTManager myADTManager = new ADTManager(continent.Azeroth, "C:\\mpq\\");</example>
		public ADTManager(ContinentType c) {
			_continent = c;
			loaded = true;
		}

		#endregion

		/// <summary>
		/// Loads an ADT into the manager.
		/// </summary>
		/// <param name="x">X coordiate of the ADT in the 64 x 64 Grid</param>
		/// <param name="y">Y coordinate of the ADT in the 64 x 64 grid</param>
		public void loadADT(int x, int y) {
			if(loaded == false) {
				MessageBox.Show("ADT Manager not loaded, aborting loading ADT file.", "ADT Manager not loaded.");
				return;
			}
			var dir = MpqNavSettings.MpqPath + AdtPath + _continent;
			if(!Directory.Exists(dir)) {
				throw new Exception("Continent data missing");
			}
			var file = String.Format("{0}{1}{2}\\{2}_{3}_{4}.adt", MpqNavSettings.MpqPath, AdtPath, _continent, x, y);
			if(!File.Exists(file)) {
				throw new Exception(String.Format("ADT Doesn't exist: {0}", file));
			}

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

		public VertexPositionNormalColored[] renderingVerticies() {
			if(!renderCached) {
				buildVerticiesAndIndicies();
			}
			return verticesCachedADT;
		}


		public int[] renderingIndices() {
			if(!renderCached) {
				buildVerticiesAndIndicies();
			}
			return indicesCachedADT;
		}

		public void buildVerticiesAndIndicies() {
			// Cycle through each ADT
			var triangleListCollection = new TriangleListCollection();
			foreach(ADT a in _ADTs) {
				// Handle the ADTs
				triangleListCollection.Add(a.TriangeListH2O);
				triangleListCollection.Add(a.TriangeList);
				// Handle the WMOs
				foreach(WMO w in a.WMOs) {
					triangleListCollection.Add(w.TriangleList);
				}
				// Handle the M2s
				foreach(M2 m in a._M2Manager._m2s) {
					triangleListCollection.Add(m.TriangleList);
				}
			}

			Optimize(triangleListCollection, out verticesCachedADT, out indicesCachedADT);

			renderCached = true;
		}

		public static void Optimize(ITriangleList list, out VertexPositionNormalColored[] outVertices, out int[] outindices) {
			var vertices = list.Vertices;
			var indices = list.Indices;
			var hash = new Dictionary<VertexPositionNormalColored, int>();
			var resultIndices = new List<int>();
			for(int i = 0; i < indices.Count; i++) {
				var vertex = vertices[indices[i]];
				int index;
				if(!hash.TryGetValue(vertex, out index)) {
					index = hash.Count;
					hash.Add(vertex, index);
				}
				resultIndices.Add(index);
			}
			outVertices = hash.Keys.ToArray();
			outindices = resultIndices.ToArray();
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