using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.Graphics;
using MPQNav.IO;
using MPQNav.Util;

namespace MPQNav.ADT
{
    /// <summary>
    /// The Map is responsible for handling all the different ADTs that we are going to be loading up.
    /// </summary>
    internal class Map
    {
        private const string AdtPath = "World\\Maps\\";

        /// <summary>
        /// List of all ADTs managed by this ADT manager
        /// </summary>
        private readonly List<ADT> adts = new List<ADT>();

        /// <summary>
        /// Continent of the ADT Manager
        /// </summary>
        private readonly string continent;

        private TriangleList triangleList;

        /// <summary>
        /// Creates a new instance of the ADT manager.
        /// </summary>
        /// <param name="continent">Continent of the ADT</param>
        /// <example>Map map = new Map(continent.Azeroth);</example>
        public Map(string continent)
        {
            this.continent = continent;
        }

        public TriangleList TriangleList
        {
            get { return triangleList ?? (triangleList = BuildTriangleList()); }
        }

        /// <summary>
        /// Loads an ADT into the manager.
        /// </summary>
        /// <param name="x">X coordiate of the ADT in the 64 x 64 Grid</param>
        /// <param name="y">Y coordinate of the ADT in the 64 x 64 grid</param>
        public void LoadADT(int x, int y)
        {
            triangleList = null;
            ADT adt = ReadADT(x, y);

            adt.Load();
            adts.Add(adt);
        }

        private ADT ReadADT(int x, int y)
        {
            string file = GetAdtFileName(x, y);
            var fileInfo = FileInfoFactory.Create();
            if (fileInfo.Exists(file) == false)
                throw new Exception(String.Format("ADT Doesn't exist: {0}", file));

            using (var reader = new BinaryReader(fileInfo.OpenRead(file)))
            {
                return new ADTChunkFileParser(reader).Parse();
            }
        }

        private string GetAdtFileName(int x, int y)
        {
            return String.Format("{0}{1}\\{1}_{2}_{3}.adt", AdtPath, continent, x, y);
        }

        private TriangleList BuildTriangleList()
        {
            // Cycle through each ADT
            var triangleListCollection = new TriangleListCollection();
            foreach (ADT a in adts)
            {
                triangleListCollection.Add(a.GetTriangleList());
            }

            return triangleListCollection.Optimize();
        }
    }
}