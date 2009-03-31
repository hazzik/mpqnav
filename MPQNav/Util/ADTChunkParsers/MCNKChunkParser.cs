using System;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
	/// <summary>
	/// MCNK Chunk perser
	/// </summary>
	internal class MCNKChunkParser : ChunkParser<MCNK[,]> {
		private readonly MCIN[] _mcins;

		/// <summary>
		/// MCNKChunkParser Perser
		/// </summary>
		/// <param name="br">Binary Stream</param>
		/// <param name="pAbsoluteStart">The position of MCNK</param>
		/// <param name="mcins"></param>
		public MCNKChunkParser(BinaryReader br, long pAbsoluteStart, MCIN[] mcins)
			: base("MCNK", br, pAbsoluteStart) {
			_mcins = mcins;
		}

		/// <summary>
		/// Parse all MCNK element from file stream
		/// </summary>
		public override MCNK[,] Parse() {
			var MCNK = new MCNK[16,16];
			int count = 0;
			for(int y = 0; y < 16; y++) {
				for(int x = 0; x < 16; x++) {
					MCNK[x, y] = processMCNK(_mcins[count].Offset);
					count++;
				}
			}
			return MCNK; // processMH2Os();
		}

		/// <summary>
		/// Processes an individual MCNK Chunk
		/// </summary>
		/// <param name="offset">Offset to the MCNK Chunk</param>
		/// <returns>MCNK Filled with information</returns>
		private MCNK processMCNK(UInt32 offset) {
			Reader.BaseStream.Position = offset + 12; // Get off the header
			//br.ReadBytes(4); // Data that's not needed
			var index_x = (int)Reader.ReadUInt32();
			var index_y = (int)Reader.ReadUInt32();
			Reader.BaseStream.Position = offset + 0x08 + 0x03C; // Get off the header
			uint holes = Reader.ReadUInt32() & 0x00FF;
			if(holes > 0) {
				Console.WriteLine(Convert.ToString(holes, 2));
			}


			Reader.BaseStream.Position = offset + 0x08 + 0x068;
			//br.BaseStream.Position += 28; // Get past the data we don't want   
			//UInt32 offsLiquid = br.ReadUInt32(); // Offset to the liquid information
			//UInt32 sizeLiquid = br.ReadUInt32(); // If there's no liquid information it will be 8

			float z = Reader.ReadSingle();
			float x = Reader.ReadSingle();
			float y = Reader.ReadSingle();
			var currentMCNK = new MCNK(x, y, z, (UInt16)holes);
			Reader.ReadBytes(20); // Read the reaming 12 bytes of data and 8 for the next header which is a MCVT chunk
			for(int i = 0; i < 145; i++) {
				currentMCNK._MCVT.heights[i] = Reader.ReadSingle();
			}
			Reader.ReadBytes(8); // We're going to read past the next header which is for the normals

			sbyte normalZ = 0;
			sbyte normalX = 0;
			sbyte normalY = 0;
			for(int i = 0; i < 145; i++) {
				normalZ = Reader.ReadSByte();
				normalX = Reader.ReadSByte();
				normalY = Reader.ReadSByte();
				currentMCNK._MCNR.normals[i] = new Vector3(-(float)normalX / 127.0f, normalY / 127.0f, -(float)normalZ / 127.0f);
			}
			return currentMCNK;
		}
	}
}