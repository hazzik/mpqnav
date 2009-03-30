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
            _MHDR.pad = br.ReadUInt32();
            _MHDR.offsInfo = br.ReadUInt32();
            _MHDR.offsTex = br.ReadUInt32();
            _MHDR.offsModels = br.ReadUInt32();
            _MHDR.offsModelsIds = br.ReadUInt32();
            _MHDR.offsMapObejcts = br.ReadUInt32();
            _MHDR.offsMapObejctsIds = br.ReadUInt32();
            _MHDR.offsDoodsDef = br.ReadUInt32();
            _MHDR.offsObjectsDef = br.ReadUInt32();
            _MHDR.offsFlightBoundary = br.ReadUInt32();
            _MHDR.offsMH2O = br.ReadUInt32();
            _MHDR.pad3 = br.ReadUInt32();
            _MHDR.pad4 = br.ReadUInt32();
            _MHDR.pad5 = br.ReadUInt32();
            _MHDR.pad6 = br.ReadUInt32();
            _MHDR.pad7 = br.ReadUInt32();

            return _MHDR;
        }



    }
}
