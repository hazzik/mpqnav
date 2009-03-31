using System;
using System.IO;
using MPQNav.ADT;
using MPQNav.Util.ADTParser;

namespace MPQNav.Util {
	internal class ADTChunkFileParser : ParserBase<ADT.ADT> {
		private readonly string _fileName;

		public ADTChunkFileParser(string fileName, BinaryReader reader)
			: base(reader) {
			_fileName = fileName;
		}

		/// <summary>
		/// Loads an ADT into the manager.
		/// </summary>
		/// <param name="filePath">File Path to the file</param>
		public static ADT.ADT LoadADT(string filePath) {
			if(!File.Exists(filePath)) {
				return null;
			}
			var fileName = Path.GetFileName(filePath);
			using(var reader = new BinaryReader(File.OpenRead(filePath))) {
				return new ADTChunkFileParser(fileName, reader).Parse();
			}
		}
		
		/// <summary>
		/// Digs through an ADT and parses out all the information in it. 
		/// </summary>
		public override ADT.ADT Parse() {
			var currentADT = new ADT.ADT(_fileName);

			var mver = new MVERChunkParser(Reader, 0).Parse();
			currentADT._Version = mver;

			var mhdr = new MHDRChunkParser(Reader, Reader.BaseStream.Position).Parse();

			var mcins = new MCINChunkParser(Reader, mhdr.OffsInfo + mhdr.Base).Parse();

			var mcnk = new MCNKChunkParser(Reader, mcins[0].Offset, mcins).Parse();

			currentADT._MCNKArray = mcnk;

			var mwmos = new MWMOChunkParser(Reader, mhdr.OffsMapObejcts + mhdr.Base).Parse();

			currentADT._MODFList = new MODFChunkParser(Reader, mhdr.OffsObjectsDef + mhdr.Base, mwmos).Parse();

			var mmdxs = new MMDXChunkParser(Reader, mhdr.OffsModels + mhdr.Base).Parse();

			currentADT._MDDFList = new MDDFChunkParser(Reader, mhdr.OffsDoodsDef + mhdr.Base, mmdxs).Parse();

			var mh2os = mhdr.OffsMH2O != 0 ? new MH2OChunkParser(Reader, mhdr.OffsMH2O + mhdr.Base).Parse() : new MH2O[0,0];

			currentADT._MH2OArray = mh2os;

			currentADT.GenerateVertexAndIndices();
			currentADT.GenerateVertexAndIndicesH2O();
			return currentADT;
		}
	}
}