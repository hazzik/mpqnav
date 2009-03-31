using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MPQNav.Util.ADTParser {
	internal class MWMOChunkParser : ChunkParser<string[]> {
		public MWMOChunkParser(BinaryReader br, long pAbsoluteStart)
			: base("MWMO", br, pAbsoluteStart) {
		}

		/// <summary>
		/// Parse MHDR element from file strem
		/// </summary>
		public override string[] Parse() {
			var lWMO = new List<string>();
			Reader.BaseStream.Position = AbsoluteStart; // 20 To get to where the list starts, 8 to get off the header.

			long EndPosition = AbsoluteStart + Size;

			String wmoName = "";
			var nextByte = new byte[1];

			while(Reader.BaseStream.Position < EndPosition) {
				nextByte = Reader.ReadBytes(1);
				if(nextByte[0] == 0) {
					lWMO.Add(wmoName);
					wmoName = "";
				}
				else {
					wmoName += Encoding.ASCII.GetString(nextByte);
				}
			}
			return lWMO.ToArray();
		}
	}
}