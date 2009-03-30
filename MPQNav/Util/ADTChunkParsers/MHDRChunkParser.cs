using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser
{
    class MHDRChunkParser : ChunkParser
    {
        public MHDRChunkParser(BinaryReader br, long pAbsoluteStart)
        {
            this.br = br;
            _Name = "MHDR";
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
        public ADT.MHDR Parse()
        {
            
            MHDR _MHDR = new MHDR();
            //long pMHDRData = br.BaseStream.Position;
            br.BaseStream.Position = _pStart;
            _MHDR.Base = (UInt32)_pStart;
            _MHDR.Pad = br.ReadUInt32();
            _MHDR.OffsInfo = br.ReadUInt32();
            _MHDR.OffsTex = br.ReadUInt32();
            _MHDR.OffsModels = br.ReadUInt32();
            _MHDR.OffsModelsIds = br.ReadUInt32();
            _MHDR.OffsMapObejcts = br.ReadUInt32();
            _MHDR.OffsMapObejctsIds = br.ReadUInt32();
            _MHDR.OffsDoodsDef = br.ReadUInt32();
            _MHDR.OffsObjectsDef = br.ReadUInt32();
            _MHDR.OffsFlightBoundary = br.ReadUInt32();
            _MHDR.OffsMH2O = br.ReadUInt32();
            _MHDR.Pad3 = br.ReadUInt32();
            _MHDR.Pad4 = br.ReadUInt32();
            _MHDR.Pad5 = br.ReadUInt32();
            _MHDR.Pad6 = br.ReadUInt32();
            _MHDR.Pad7 = br.ReadUInt32();

            return _MHDR;
        }



    }
}
