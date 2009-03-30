using System;
using System.Collections.Generic;
using MPQNav.ADT;
using System.IO;

namespace MPQNav.Util.ADTParser
{
    /// <summary>
    /// MCIN Chunk perser
    /// </summary>
    class MCINChunkParser : ChunkParser
    {
        /// <summary>
        /// MCINChunkParser Perser
        /// </summary>
        /// <param name="br">Binary Stream</param>
        /// <param name="pAbsoluteStart">The position of MCIN</param>
        public MCINChunkParser(BinaryReader br, long pAbsoluteStart)
        {
            this.br = br;
            _Name = "MCIN";
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
        /// Return the absolute position of MCIN chunk in the file stream
        /// </summary>
        public override long AbsoluteStart
        {
            get { return _pStart; }
        }

        /// <summary>
        /// Return the size of MCIN chunk
        /// </summary>
        public override uint Size
        {
            get { return _Size; }
        }

        /// <summary>
        /// Parse all MCIN element from file strem
        /// </summary>
        public MCIN[] Parse()
        {
            return processMCIN();
        }
        /// <summary>
        /// Procceses MCIN chunk.
        /// </summary>
        /// <param name="br">Binary reader loaded with the ADT set to the MCIN position</param>
        /// <param name="currentADT">ADT Containing the MCNKs</param>
        private MCIN[] processMCIN()
        {
            br.BaseStream.Position = _pStart;
            MCIN[] _MCIN = new MCIN[256];
            for (int i = 0; i < 256; i++)
            {
                MCIN lMCIN = new MCIN();

                lMCIN.Offset = br.ReadUInt32();
                lMCIN.Size = br.ReadUInt32();
                lMCIN.Flags = br.ReadUInt32();
                lMCIN.AsyncId = br.ReadUInt32();
                _MCIN[i] = lMCIN;
            }
            return _MCIN;

        }


    }


}