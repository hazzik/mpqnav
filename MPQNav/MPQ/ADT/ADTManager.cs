using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MPQNav.ADT
{
    /// <summary>
    /// The ADTManager is responsible for handling all the different ADTs that we are going to be loading up.
    /// </summary>
    class ADTManager
    {
        #region variables
        /// <summary>
        /// Boolean result stating if this manager is loaded or not.
        /// </summary>
        private Boolean loaded = false;
        /// <summary>
        /// Continent of the ADT Manager
        /// </summary>
        private continent_type _continent;
        /// <summary>
        /// Base directory for all MPQ data.
        /// </summary>
        private String _basePath = "C:\\temp\\mpq\\";
        /// <summary>
        /// Default ADT Path
        /// </summary>
        private String _adtPath = "World\\Maps\\";
        /// <summary>
        /// Enumeration of the different continents available.
        /// </summary>
        public enum continent_type
        {
            Azeroth,
            Kalimdor,
            Outland
        }
        /// <summary>
        /// List of all ADTs managed by this ADT manager
        /// </summary>
        private List<ADT> _ADTs = new List<ADT>();
        /// <summary>
        /// Boolean variable representing if all the rendering data has been cached.
        /// </summary>
        private Boolean renderCached = false;
        private List<VertexPositionNormalColored> verticesCachedADT = new List<VertexPositionNormalColored>();
        private List<int> indicesCachedADT = new List<int>();
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new instance of the ADT manager.
        /// </summary>
        /// <param name="c">Continent of the ADT</param>
        /// <param name="dataDirectory">Base directory for all MPQ data WITH TRAILING SLASHES</param>
        /// <example>ADTManager myADTManager = new ADTManager(continent.Azeroth, "C:\\mpq\\");</example>
        public ADTManager(continent_type c, String dataDirectory)
        {
            this._continent = c;
            if (Directory.Exists(dataDirectory))
            {
                this.loaded = true;
                this._basePath = dataDirectory;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid data directory entered. Please exit and update your app.CONFIG file","Invalid Data Directory");
//                throw new Exception("Invalid data directory entered.");
            }
        }
        #endregion

        /// <summary>
        /// Loads an ADT into the manager.
        /// </summary>
        /// <param name="x">X coordiate of the ADT in the 64 x 64 Grid</param>
        /// <param name="y">Y coordinate of the ADT in the 64 x 64 grid</param>
        public void loadADT(int x, int y)
        {
            if (this.loaded == false)
            {
                System.Windows.Forms.MessageBox.Show("ADT Manager not loaded, aborting loading ADT file.", "ADT Manager not loaded.");
                return;
            }
            if (Directory.Exists(this._basePath + this._adtPath + this._continent))
            {
                if (File.Exists(this._basePath + this._adtPath + this._continent + "\\" + this._continent + "_" + x.ToString().PadLeft(2, Convert.ToChar("0")) + "_" + y.ToString().PadLeft(2, Convert.ToChar("0")) + ".adt"))
                {
                    string FilePath;
                    FilePath = this._basePath + "World\\Maps\\" + this._continent + "\\" + this._continent + "_" + x.ToString().PadLeft(2, Convert.ToChar("0")) + "_" + y.ToString().PadLeft(2, Convert.ToChar("0")) + ".adt";

                    Util.ADTChunkFileParser ADTprs = new MPQNav.Util.ADTChunkFileParser();


                    ADT currentADT = ADTprs.loadADT(FilePath);
                    
                    foreach (MODF lMODF in currentADT._MODFList)
                    {
                        currentADT.WMOManager.addWMO(lMODF.fileName, this._basePath, lMODF);
                    }
                    
                    foreach (MDDF lMMDF in currentADT._MDDFList) {
                    	var fullName = _basePath + lMMDF.filePath;
                    	currentADT._M2Manager.Add(fullName);
							  try
							  {currentADT._M2Manager.Process(fullName, lMMDF);}
							  catch{}
                    }

                	this.renderCached = false;
                    currentADT.GenerateVertexAndIndices();
                    currentADT.GenerateVertexAndIndicesH2O();
                    this._ADTs.Add(currentADT);
                }
                else
                {
                    throw new Exception("ADT Doesn't exist: " + this._basePath + "\\World\\Maps\\" + this._continent + "\\" + this._continent + "_" + x.ToString().PadLeft(2, Convert.ToChar("0")) + "_" + y.ToString().PadLeft(2, Convert.ToChar("0")) + ".adt");
                }
            }
            else
            {
                throw new Exception("Continent data missing");
            }
        }

    	public List<VertexPositionNormalColored> renderingVerticies()
        {
            if (this.renderCached == true)
            {
                return this.verticesCachedADT;
            }
            this.buildVerticiesAndIndicies();
            return this.verticesCachedADT;
        }
        
        
        public List<int> renderingIndices()
        {
            if (this.renderCached == true)
            {
                return this.indicesCachedADT;
            }
            this.buildVerticiesAndIndicies();
            return this.indicesCachedADT;
        }

        public void buildVerticiesAndIndicies()
        {
            // Cycle through each ADT
            List<VertexPositionNormalColored> tempVertices = new List<VertexPositionNormalColored>();
            List<int> tempIndicies = new List<int>();
            int offset = 0;
            foreach (ADT a in this._ADTs)
            {

                // Handle the ADTs
                for (int v = 0; v < a.Vertices.Count; v++)
                {
                    tempVertices.Add(a.Vertices[v]);
                }
                for (int i = 0; i < a.Indicies.Count; i++)
                {
                    tempIndicies.Add(a.Indicies[i] + offset);
                }
                offset = tempVertices.Count;
                for (int v = 0; v < a.H2OVertices.Count; v++)
                {
                    tempVertices.Add(a.H2OVertices[v]);
                }
                for (int i = 0; i < a.H2OIndicies.Count; i++)
                {
                    tempIndicies.Add(a.H2OIndicies[i] + offset);
                }
                offset = tempVertices.Count;
                // Handle the WMOs
                foreach (MPQ.ADT.WMO w in a.WMOManager._wmos)
                {
                    for (int v = 0; v < w._Vertices.Count; v++)
                    {
                        tempVertices.Add(w._Vertices[v]);
                    }
                    for (int i = 0; i < w._Indices.Count; i++)
                    {
                        tempIndicies.Add(w._Indices[i] + offset);
                    }
                    offset = tempVertices.Count;
                }
                // Handle the M2s
                foreach (MPQNav.ADT.M2 m in a._M2Manager._m2s)
                {
                    for (int v = 0; v < m._Vertices.Count; v++)
                    {
                        tempVertices.Add(m._Vertices[v]);
                    }
                    for (int i = 0; i < m._Indices.Count; i++)
                    {
                        tempIndicies.Add(m._Indices[i] + offset);
                    }
                    offset = tempVertices.Count;
                }
                
            }
            this.indicesCachedADT.Clear();
            this.indicesCachedADT = tempIndicies;

            this.verticesCachedADT.Clear();
            this.verticesCachedADT = tempVertices;

            this.renderCached = true;   
        }
    }
}
