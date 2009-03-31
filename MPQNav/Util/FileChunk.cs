using System;
using System.IO;
using System.Text;

namespace MPQNav.Util {
	internal class FileChunkHelper {
		public static ChunkResultReturn SearchChunk(BinaryReader br, String chunkName) {
			chunkName = ReverseString(chunkName);
			// We assume we're giving the chunk name reversed.
			// That is to say chunkName for what we call MOVI would be given as IVOM
			String frstTwoChars = chunkName.Substring(0, 2);
			String lastTwoChars = chunkName.Substring(2, 2);

			br.BaseStream.Position = 0;

			var keepSearch = true;
			var nextLine = br.ReadBytes(2);	
			try {
				while(keepSearch) {
					var nextLine2 = br.ReadBytes(2);
					if(Encoding.ASCII.GetString(nextLine) == frstTwoChars && Encoding.ASCII.GetString(nextLine2, 0, 2) == lastTwoChars) {
						keepSearch = false;
					}
					if(keepSearch) {
						nextLine = nextLine2;
					}
				}
				long pos = br.BaseStream.Position - 4;
				return new ChunkResultReturn(true, pos, br.ReadUInt32());
			}
			catch(Exception e) {
				Console.WriteLine(e.ToString());
				return new ChunkResultReturn(false, 0, 0);
			}
		}

		public static string ReverseString(string s) {
			char[] arr = s.ToCharArray();
			Array.Reverse(arr);
			return new string(arr);
		}

		#region Nested type: ChunkResultReturn

		public class ChunkResultReturn {
			public bool Found { get; private set; }
			public uint Size { get; private set; }
			public long StartPosition { get; private set; }

			public ChunkResultReturn(bool found, long startPosition, UInt32 size) {
				Found = found;
				StartPosition = startPosition;
				Size = size;
			}
		}

		#endregion
	}
}