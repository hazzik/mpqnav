using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MPQNav.Util.ADTParser {
	internal class MMDXChunkParser : ChunkParser<string[]> {
		public MMDXChunkParser(BinaryReader br, long pAbsoluteStart)
			: base("MMDX",br, pAbsoluteStart) {
		}

		/// <summary>
		/// Parse MMDX element from file strem
		/// </summary>
		public override string[] Parse() {
			var ret = new List<string>();
			Reader.BaseStream.Position = AbsoluteStart;

			UInt32 chunkSize = Size;
			long EndPosition = Reader.BaseStream.Position + chunkSize;

			String m2Name = "";
			var nextByte = new byte[1];
			while(Reader.BaseStream.Position < EndPosition) {
				nextByte = Reader.ReadBytes(1);
				if(nextByte[0] != 0) {
					m2Name += Encoding.ASCII.GetString(nextByte);
				}
				else {
					// Example: world\wmo\azeroth\buildings\redridge_stable\redridge_stable.wmo
					ret.Add(m2Name);
					m2Name = "";
				}
			}
			return ret.ToArray();
		}
	}
}