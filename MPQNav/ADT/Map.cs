using System;
using System.IO;
using MPQNav.Chunks;
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

        	var triangeList = adt.TriangleList;

        	triangleList.Add(triangeList);
        }

        private ADT ReadADT(int x, int y)
        {
            var fileName = GetAdtFileName(x, y);
            string mainFile = fileName + ".adt";
            IFileInfo fileInfo = FileInfoFactory.Create();
            if (fileInfo.Exists(mainFile) == false)
                throw new Exception(String.Format("ADT Doesn't exist: {0}", mainFile));

            var adt = new ADT
            {
                MCNKArray = new MCNK[16, 16]
            };

            using (var reader = new BinaryReader(fileInfo.OpenRead(mainFile)))
            {
                ADTChunkFileParser.Parse(reader, adt, true);
            }

            var additionalfiles = new[]
            {
                fileName + "_obj0.adt",
                fileName + "_obj1.adt",
                fileName + "_tex0.adt",
                fileName + "_tex1.adt",
            };

            foreach (var s in additionalfiles)
            {
                if (fileInfo.Exists(s))
                {
                    using (var reader = new BinaryReader(fileInfo.OpenRead(s)))
                    {
                        ADTChunkFileParser.Parse(reader, adt, false);
                    }
                }
            }

            return adt;
        }

        private string GetAdtFileName(int x, int y)
        {
            return String.Format("{0}{1}\\{1}_{2}_{3}", AdtPath, continent, x, y);
        }
    }
}