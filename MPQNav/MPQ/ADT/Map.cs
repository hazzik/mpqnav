using System;
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

        private readonly string continent;

        private readonly TriangleListCollection triangleList = new TriangleListCollection();

        /// <summary>
        /// Creates a new instance of the map.
        /// </summary>
        /// <param name="continent">Continent of the ADT</param>
        /// <example>Map map = new Map(continent.Azeroth);</example>
        public Map(string continent)
        {
            this.continent = continent;
        }

        public TriangleList TriangleList
        {
            get { return triangleList; }
        }

        /// <summary>
        /// Loads an ADT into the manager.
        /// </summary>
        /// <param name="x">X coordiate of the ADT in the 64 x 64 Grid</param>
        /// <param name="y">Y coordinate of the ADT in the 64 x 64 grid</param>
        public void LoadADT(int x, int y)
        {
            ADT adt = ReadADT(x, y);

            adt.Load();

            triangleList.Add(adt.TriangleList);
        }

        private ADT ReadADT(int x, int y)
        {
            string file = GetAdtFileName(x, y);
            IFileInfo fileInfo = FileInfoFactory.Create();
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
    }
}