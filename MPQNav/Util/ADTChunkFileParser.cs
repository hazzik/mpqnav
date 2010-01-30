using System;
using System.IO;
using MPQNav.Chunks.Parsers;

namespace MPQNav.Util {
	internal class ADTChunkFileParser : ParserBase<ADT.ADT> {
		public ADTChunkFileParser(BinaryReader reader)
			: base(reader) {
		}

		/// <summary>
		/// Digs through an ADT and parses out all the information in it. 
		/// </summary>
		public override ADT.ADT Parse() {
			var adt = new ADT.ADT();

			var mver = new MVERChunkParser(Reader, 0).Parse();
			adt.Version = mver;

			var mhdr = new MHDRChunkParser(Reader, Reader.BaseStream.Position).Parse();

			var mcins = new MCINChunkParser(Reader, mhdr.OffsInfo + mhdr.Base).Parse();

			var mcnk = new MCNKChunkParser(Reader, mcins[0].Offset, mcins).Parse();

			adt.MCNKArray = mcnk;

			var mwmos = new StringArrayChunkParser("MWMO", Reader, mhdr.OffsMapObejcts + mhdr.Base).Parse();

			adt.MODFList = new MODFChunkParser(Reader, mhdr.OffsObjectsDef + mhdr.Base, mwmos).Parse();

			var mmdxs = new StringArrayChunkParser("MMDX", Reader, mhdr.OffsModels + mhdr.Base).Parse();

			adt.MDDFList = new MDDFChunkParser(Reader, mhdr.OffsDoodsDef + mhdr.Base, mmdxs).Parse();

			var mh2os = mhdr.OffsMH2O != 0 ? new MH2OChunkParser(Reader, mhdr.OffsMH2O + mhdr.Base).Parse() : null;

			adt.MH2OArray = mh2os;

			return adt;
		}
	}
}