using System;
using System.IO;
using MPQNav.Chunks;
using MPQNav.Chunks.Parsers;
using MPQNav.IO;

namespace MPQNav.Util {
	internal sealed class ADTChunkFileParser {
	    /// <summary>
	    /// Digs through an ADT and parses out all the information in it. 
	    /// </summary>
	    /// <param name="reader"></param>
	    /// <param name="adt"></param>
	    /// <param name="readMCNK"></param>
	    public static ADT.ADT Parse(BinaryReader reader, ADT.ADT adt, bool readMCNK)
		{
	        string[] mwmos = null;
		    string[] mmdxs = null;
		    int mcnkIndex = 0;
		    while (reader.BaseStream.Position < reader.BaseStream.Length)
		    {
		        var pos = reader.BaseStream.Position;
		        var name = reader.ReadStringReversed(4);
		        var size = reader.ReadUInt32();
		        var r = new BinaryReader(new MemoryStream(reader.ReadBytes((int) size)));
		        switch (name)
		        {
		            case "MVER":
                        var mver = new MVERChunkParser(size).Parse(r);
		                adt.Version = mver;
		                break;
		            case "MHDR":
                        var mhdr = new MHDRChunkParser(size).Parse(r);
		                break;
		            case "MCIN":
                        var mcins = new MCINChunkParser(size).Parse(r);
		                break;
		            case "MCNK":
                        if (readMCNK)
                            adt.MCNKArray[mcnkIndex % 16, mcnkIndex / 16] = new MCNKChunkParser(size).Parse(r);
		                mcnkIndex ++;
		                break;
		            case "MWMO":
                        mwmos = new StringArrayChunkParser(size).Parse(r);
		                break;
		            case "MODF":
                        adt.MODFList = new MODFChunkParser(mwmos, size).Parse(r);
		                break;
		            case "MMDX":
                        mmdxs = new StringArrayChunkParser(size).Parse(r);
		                break;
		            case "MDDF":
                        adt.MDDFList = new MDDFChunkParser(size, mmdxs).Parse(r);
		                break;
		            case "MH2O":
                        adt.MH2OArray = new MH2OChunkParser(size).Parse(r);
		                break;
                    default:
		            {
		                break;
		            }
		        }
		        reader.BaseStream.Position = pos + 0x08 + size;
		    }

		    return adt;
		}
	}
}