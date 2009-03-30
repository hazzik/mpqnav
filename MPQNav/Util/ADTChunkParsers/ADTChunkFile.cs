using System;
using System.Collections.Generic;
using System.IO;
using MPQNav.ADT;
using MPQNav.Util.ADTParser;

namespace MPQNav.Util {
	internal class ADTChunkFileParser {
		//private ADT.ADT ADTResult();

		/// <summary>
		/// Loads an ADT into the manager.
		/// </summary>
		/// <param name="FilePath">File Path to the file</param>
		public ADT.ADT loadADT(string FilePath) {
			ADT.ADT currentADT = null;
			if(File.Exists(FilePath)) {
				string FileName = FilePath.Split('\\')[FilePath.Split('\\').Length - 1];
				FileStream fs = File.OpenRead(FilePath);

				currentADT = new ADT.ADT(FileName);
				ParseADT(new BinaryReader(fs), currentADT);
			}
			else {
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
		private static void ParseADT(BinaryReader br, ADT.ADT currentADT) {
			if(!FileChunkHelper.SearchChunk(br, "MVER").ChunkFound) {
				throw new Exception("Not valid ADT File");
			}
			br.BaseStream.Position += 4;
			currentADT._Version = br.ReadInt32();

			var ret = FileChunkHelper.SearchChunk(br, "MHDR");

			if(!ret.ChunkFound) {
				throw new Exception("Not valid ADT File");
			}

			long pMHDRData = ret.ChunkStartPosition;

			var mhdr = new MHDRChunkParser(br, pMHDRData).Parse();

			var mcins = new MCINChunkParser(br, mhdr.OffsInfo + mhdr.Base).Parse();

			var mcnk = new MCNKChunkParser(br, mcins[0].Offset, mcins).Parse();

			currentADT.addMCNK(mcnk);

			var mwmos = new MWMOChunkParser(br, mhdr.OffsMapObejcts + mhdr.Base).Parse();

			currentADT._MODFList = new MODFChunkParser(br, mhdr.OffsObjectsDef + mhdr.Base, mwmos).Parse();

			var mmdxs = new MMDXChunkParser(br, mhdr.OffsModels + mhdr.Base).Parse();

			currentADT._MDDFList = new MDDFChunkParser(br, mhdr.OffsDoodsDef + mhdr.Base, mmdxs).Parse();

			long ofsMH2O = mhdr.OffsMH2O;

			if(ofsMH2O != 0) {
				ofsMH2O += mhdr.Base;
			}

			var chkH20 = new MH2OChunkParser(br, ofsMH2O);
			var mh2os = chkH20.Parse();

			currentADT.addMH2O(mh2os);
			currentADT.GenerateVertexAndIndices();
			currentADT.GenerateVertexAndIndicesH2O();
		}
	}
}