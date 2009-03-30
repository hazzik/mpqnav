using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MPQNav.ADT;
using Microsoft.Xna.Framework;

namespace MPQNav.Util.ADTParser
{
    class MODFChunkParser : ChunkParser
    {
        public MODFChunkParser(BinaryReader br, long pAbsoluteStart)
        {
            this.br = br;
            _Name = "MODF";
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
        /// Return the absolute position of MODF chunk in the file stream
        /// </summary>
        public override long AbsoluteStart
        {
            get { return _pStart; }
        }

        /// <summary>
        /// Return the size of MODF chunk
        /// </summary>
        public override uint Size
        {
            get { return _Size; }
        }

        /// <summary>
        /// Parse MODF element from file strem
        /// </summary>
        public List<MODF> Parse(List<string> WMO)
        {
            List<MODF> _MODF = new List<MODF>();
            br.BaseStream.Position = _pStart;
            int bytesRead = 0;
            while (bytesRead < _Size)
            {
                MODF lMODF = new MODF();
                lMODF.fileName = WMO.ToArray()[(int)br.ReadUInt32()];
                lMODF.uniqid = br.ReadUInt32(); // 4 bytes
                lMODF.position = new Vector3((float)br.ReadSingle(), (float)br.ReadSingle(), (float)br.ReadSingle()); // 12 Bytes
                lMODF.orientation_a = (float)br.ReadSingle(); // 4 Bytes
                lMODF.orientation_b = (float)br.ReadSingle(); // 4 Bytes
                lMODF.orientation_c = (float)br.ReadSingle(); // 4 Bytes
                br.ReadBytes(32); // 32 bytes
                bytesRead += 64; // 64 total bytes
                _MODF.Add(lMODF);
                //currentADT.addWMO(currentMODF.fileName, this._basePath, currentMODF);

            }
            return _MODF;
        }



    }
}
