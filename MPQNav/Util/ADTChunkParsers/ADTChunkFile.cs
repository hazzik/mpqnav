using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MPQNav.Util.ADTParser;

namespace MPQNav.Util
{
    class ADTChunkFileParser
    {
        //private ADT.ADT ADTResult();

        /// <summary>
        /// Loads an ADT into the manager.
        /// </summary>
        /// <param name="FilePath">File Path to the file</param>
        public ADT.ADT loadADT(string FilePath)
        {
            ADT.ADT currentADT = null;
                if (File.Exists(FilePath))
                {
                    string FileName = FilePath.Split('\\')[FilePath.Split('\\').Length -1 ];
                    FileStream fs = File.OpenRead(FilePath);

                    currentADT = new ADT.ADT(FileName);
                    ParseADT(new BinaryReader(fs), currentADT);
                }
                else
                {
                    //throw new Exception("ADT Doesn't exist: " + this._basePath + "\\World\\Maps\\" + this._continent + "\\" + this._continent + "_" + x.ToString().PadLeft(2, Convert.ToChar("0")) + "_" + y.ToString().PadLeft(2, Convert.ToChar("0")) + ".adt");
                }
                return currentADT;
        }

        /// <summary>
        /// Digs through an ADT and parses out all the information in it. 
        /// </summary>
        /// <param name="x">X offset of the ADT in the 64 x 64 grid</param>
        /// <param name="y">Y offset of the ADT in the 64 x 64 grid</param>
        /// <param name="br">Binary reader loaded with the ADT file</param>
        /// <param name="currentADT">ADT to be processed</param>
        private void ParseADT(BinaryReader br, ADT.ADT currentADT)
        {
            if (!Util.FileChunkHelper.SearchChunk(br, "MVER").ChunkFound)
                throw new Exception("Not valid ADT File");
            br.BaseStream.Position += 4;
            currentADT._Version = br.ReadInt32();

            Util.FileChunkHelper.ChunkResultReturn ret = Util.FileChunkHelper.SearchChunk(br, "MHDR");

            if (!ret.ChunkFound)
                throw new Exception("Not valid ADT File");

            long pMHDRData = ret.ChunkStartPosition;

            MHDRChunkParser chkHRD = new MHDRChunkParser(br, pMHDRData);
            ADT.MHDR _MHDR = chkHRD.Parse();


            ADT.MCIN[] lMCIN;
            MCINChunkParser chkCIN = new MCINChunkParser(br, _MHDR.OffsInfo + _MHDR.Base);
            lMCIN = chkCIN.Parse();

            MCNKChunkParser chkCNK = new MCNKChunkParser(br, lMCIN[0].Offset);
            currentADT.addMCNK(chkCNK.Parse(lMCIN));

            List<string> lMWMOList;
            MWMOChunkParser chkWMO = new MWMOChunkParser(br, _MHDR.OffsMapObejcts + _MHDR.Base);
            lMWMOList = chkWMO.Parse();

            MODFChunkParser chkODF = new MODFChunkParser(br, _MHDR.OffsObjectsDef + _MHDR.Base);
            currentADT._MODFList = chkODF.Parse(lMWMOList);

            List<string> lMMDXList;
            MMDXChunkParser chkMMDX = new MMDXChunkParser(br, _MHDR.OffsModels + _MHDR.Base);
            lMMDXList = chkMMDX.Parse();

            MDDFChunkParser chkMMDF = new MDDFChunkParser(br, _MHDR.OffsDoodsDef + _MHDR.Base);
            currentADT._MDDFList = chkMMDF.Parse(lMMDXList);

            long ofsMH2O = _MHDR.OffsMH2O;

            if (ofsMH2O != 0)
                ofsMH2O += _MHDR.Base;

            MH2OChunkParser chkH20 = new MH2OChunkParser(br, ofsMH2O);
            currentADT.addMH2O(chkH20.Parse());

            currentADT.GenerateVertexAndIndices();
            currentADT.GenerateVertexAndIndicesH2O();
        }
    }


    
}