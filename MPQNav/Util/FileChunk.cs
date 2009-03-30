using System;
using System.IO;
using System.Text;

namespace MPQNav.Util {
	internal class FileChunkHelper {
		/// <summary>
		/// Search for a chunk in ADT File
		/// It returns ChunkResultReturn class
		/// </summary>
		/// <param name="br">Binary Reader with the ADT Loaded</param>
		/// <param name="ChunkName">Chink to search (case sensitive)</param>
		public static ChunkResultReturn _SearchChunk(BinaryReader br, String ChunkName) {
			ChunkName = ReverseString(ChunkName);
			br.BaseStream.Position = 0;
			int len = ChunkName.Length;
			bool found = false;
			for(long i = 0; (!found) && ((i + len) < br.BaseStream.Length); i++) {
				br.BaseStream.Position = i;
				string temp = Encoding.ASCII.GetString(br.ReadBytes(len));
				found = (temp == ChunkName);
			}
			if(found) {
				return new ChunkResultReturn(true, br.BaseStream.Position - 4, br.ReadUInt32());
			}
			return new ChunkResultReturn(false, 0, 0);
		}

		public static ChunkResultReturn SearchChunk(BinaryReader br, String chunkName) {
			chunkName = ReverseString(chunkName);
			// We assume we're giving the chunk name reversed.
			// That is to say chunkName for what we call MOVI would be given as IVOM
			String frstTwoChars = chunkName.Substring(0, 2);
			String lastTwoChars = chunkName.Substring(2, 2);

			br.BaseStream.Position = 0;

			byte[] nextLine; // Next two bytes
			byte[] nextLine2; // Bytes after that
			Boolean keepSearch = true;
			nextLine = br.ReadBytes(2);
			try {
				while(keepSearch) {
					nextLine2 = br.ReadBytes(2);
					if(Encoding.ASCII.GetString(nextLine) == frstTwoChars && Encoding.ASCII.GetString(nextLine2, 0, 2) == lastTwoChars) {
						keepSearch = false;
					}
					if(keepSearch) {
						nextLine = nextLine2;
					}
				}
				//br.BaseStream.Position = br.BaseStream.Position - 4;
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
			public bool ChunkFound;
			public UInt32 ChunkLen;
			public long ChunkStartPosition;

			public ChunkResultReturn(bool ChunkFound, long ChunkStartPosition, UInt32 ChunkLen) {
				this.ChunkFound = ChunkFound;
				this.ChunkStartPosition = ChunkStartPosition;
				this.ChunkLen = ChunkLen;
			}
		}

		#endregion
	}
}