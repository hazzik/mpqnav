using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MPQNav.ADT;
using Microsoft.Xna.Framework;

namespace MPQNav.Util.ADTParser
{
    class MDDFChunkParser : ChunkParser
    {
        public MDDFChunkParser(BinaryReader br, long pAbsoluteStart)
        {
            this.br = br;
            _Name = "MDDF";
            br.BaseStream.Position = pAbsoluteStart + 4;
            _Size = br.ReadUInt32();
            _pStart = pAbsoluteStart + 8;
        }

        /// <summary>
        /// Return the chunk Name
        /// </summary>
        public override string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Return the absolute position of MDDF chunk in the file stream
        /// </summary>
        public override long AbsoluteStart
        {
            get { return _pStart; }
        }

        /// <summary>
        /// Return the size of MDDF chunk
        /// </summary>
        public override uint Size
        {
            get { return _Size; }
        }
        
        /// <summary>
        /// Parse MDDF element from file strem
        /// </summary>
        public List<MDDF> Parse(List<string> MDX)
        {
            List<MDDF> _MDDF = new List<MDDF>();
            br.BaseStream.Position = _pStart;
            int bytesRead = 0;
            while (bytesRead < _Size)
            {
                MDDF lMDDF = new MDDF();
                lMDDF.FilePath = MDX.ToArray()[(int)br.ReadUInt32()];
                lMDDF.UniqId = br.ReadUInt32(); // 4 bytes
                lMDDF.Position = new Vector3((float)br.ReadSingle(), (float)br.ReadSingle(), (float)br.ReadSingle()); // 12 Bytes
                lMDDF.OrientationA = (float)br.ReadSingle(); // 4 Bytes
                lMDDF.OrientationB = (float)br.ReadSingle(); // 4 Bytes
                lMDDF.OrientationC = (float)br.ReadSingle(); // 4 Bytes
                lMDDF.Scale = (float)(br.ReadUInt32() / 1024f); // 4 bytes
                bytesRead += 36; // 36 total bytes
                _MDDF.Add(lMDDF);
                //currentADT.addWMO(currentMODF.fileName, this._basePath, currentMODF);

            }
            return _MDDF;
        }
        


    }
}
