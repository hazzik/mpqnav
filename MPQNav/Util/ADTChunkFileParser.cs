using System;
using System.IO;
using MPQNav.Chunks;
using MPQNav.Chunks.Parsers;
using MPQNav.IO;

namespace MPQNav.Util {
	internal class ADTChunkFileParser : IParser<ADT.ADT> {
		public ADTChunkFileParser(BinaryReader reader)
			: base() {
		}

	    /// <summary>
	    /// Digs through an ADT and parses out all the information in it. 
	    /// </summary>
	    /// <param name="reader"></param>
	    public virtual ADT.ADT Parse(BinaryReader Reader)
		{

		    var adt = new ADT.ADT
		    {
		        MCNKArray = new MCNK[16, 16]
		    };
		    string[] mwmos = null;
		    string[] mmdxs = null;
		    int mcnkIndex = 0;
		    while (Reader.BaseStream.Position < Reader.BaseStream.Length)
		    {
		        var pos = Reader.BaseStream.Position;
		        var name = Reader.ReadStringReversed(4);
		        var size = Reader.ReadUInt32();
		        var reader = new BinaryReader(new MemoryStream(Reader.ReadBytes((int) size)));
		        switch (name)
		        {
		            case "MVER":
                        var mver = new MVERChunkParser(size).Parse(reader);
		                adt.Version = mver;
		                break;
		            case "MHDR":
                        var mhdr = new MHDRChunkParser(size).Parse(reader);
		                break;
		            case "MCIN":
                        var mcins = new MCINChunkParser(size).Parse(reader);
		                break;
		            case "MCNK":
                        adt.MCNKArray[mcnkIndex % 16, mcnkIndex / 16] = new MCNKChunkParser(size).Parse(reader);
		                mcnkIndex ++;
		                break;
		            case "MWMO":
                        mwmos = new StringArrayChunkParser(size).Parse(reader);
		                break;
		            case "MODF":
                        adt.MODFList = new MODFChunkParser(mwmos, size).Parse(reader);
		                break;
		            case "MMDX":
                        mmdxs = new StringArrayChunkParser(size).Parse(reader);
		                break;
		            case "MDDF":
                        adt.MDDFList = new MDDFChunkParser(size, mmdxs).Parse(reader);
		                break;
		            case "MH2O":
                        adt.MH2OArray = new MH2OChunkParser(size).Parse(reader);
		                break;
		        }
		        Reader.BaseStream.Position = pos + 0x08 + size;
		    }

		    return adt;
		}
	}
}