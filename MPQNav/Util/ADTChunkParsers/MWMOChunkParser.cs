using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser
{
    class MWMOChunkParser : ChunkParser
    {
        public MWMOChunkParser(BinaryReader br, long pAbsoluteStart)
        {
            this.br = br;
            _Name = "MWMO";
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
        /// Return the absolute position of MHDR chunk in the file stream
        /// </summary>
        public override long AbsoluteStart
        {
            get { return _pStart; }
        }

        /// <summary>
        /// Return the size of MHDR chunk
        /// </summary>
        public override uint Size
        {
            get { return _Size; }
        }

        /// <summary>
        /// Parse MHDR element from file strem
        /// </summary>
        public List<string> Parse()
        {
            List<string> lWMO = new List<string>();
            br.BaseStream.Position = _pStart; // 20 To get to where the list starts, 8 to get off the header.

            long EndPosition = _pStart + _Size;

            String wmoName = "";
            byte[] nextByte = new byte[1];

            while (br.BaseStream.Position < EndPosition)
            {
                nextByte = br.ReadBytes(1);
                if (nextByte[0] != (byte)0)
                {
                    wmoName += System.Text.ASCIIEncoding.ASCII.GetString(nextByte);
                }
                else
                {
                    lWMO.Add(wmoName.ToString());
                    wmoName = "";
                }
            }
            return lWMO;
        }



    }
}
