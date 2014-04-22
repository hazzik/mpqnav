using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
	/// <summary>
	/// MH2O Chunk perser
	/// </summary>
	internal class MH2OChunkParser : ChunkParser<MH2O[,]> {
	    /// <summary>
	    /// MH2OChunkParser Perser
	    /// </summary>
	    /// <param name="size"></param>
	    public MH2OChunkParser(uint size)
			: base(size) {
		}

	    /// <summary>
	    /// Parse all MH2O element from file stream
	    /// </summary>
	    /// <param name="reader"></param>
	    public override MH2O[,] Parse(BinaryReader reader)
	    {
	        var start = reader.BaseStream.Position;
			var result = new MH2O[16,16];
			var mh2oHeader = new MH2OHeader[256];
			for(int i = 0; i < 256; i++) {
				mh2oHeader[i].ofsData1 = reader.ReadUInt32();
				mh2oHeader[i].used = reader.ReadUInt32();
				mh2oHeader[i].ofsData2 = reader.ReadUInt32();
			}
			for(int y = 0; y < 16; y++) {
				for(int x = 0; x < 16; x++)
				{
				    result[x, y] = processMH20(mh2oHeader[y * 16 + x], start, reader);
				}
			}
			return result;
		}

	    /// <summary>
	    /// Processes an individual MH2O Chunk
	    /// </summary>
	    /// <param name="Header">MH20 Header Chunk</param>
	    /// <param name="ofsMH20">MH20 OFFset</param>
	    /// <param name="reader"></param>
	    /// <returns>MH2O Filled with information</returns>
	    private MH2O processMH20(MH2OHeader Header, long ofsMH20, BinaryReader reader) {
			if(Header.used == 0) {
				return new MH2O { used = false };
			}

			reader.BaseStream.Position = ofsMH20 + Header.ofsData1 + 2;

			var currentMH2O = new MH2O();
			currentMH2O.used = true;
			currentMH2O.type = ((MH2O.FluidType)reader.ReadUInt16());
			currentMH2O.heightLevel1 = reader.ReadSingle();
			currentMH2O.heightLevel2 = reader.ReadSingle();
			currentMH2O.xOffset = reader.ReadByte();
			currentMH2O.yOffset = reader.ReadByte();
			currentMH2O.width = reader.ReadByte();
			currentMH2O.height = reader.ReadByte();

			UInt32 ofsData2a = reader.ReadUInt32();
			UInt32 ofsData2b = reader.ReadUInt32();


			currentMH2O.RenderBitMap = ReadRenderBitMap(currentMH2O, ofsData2a, ofsMH20 + ofsData2a, ofsData2b - ofsData2a, reader);

			currentMH2O.heights = ReadHeights(currentMH2O, ofsData2b, (currentMH2O.width + 1) * (currentMH2O.height + 1), ofsMH20 + ofsData2b, reader);

			return currentMH2O;
		}

		private byte[] ReadRenderBitMap(MH2O currentMH2O, uint ofsData2a, long offset, uint size, BinaryReader reader) {
			var map = new byte[currentMH2O.height];
			if(ofsData2a != 0) {
				reader.BaseStream.Position = offset;
				for(int i = 0; i < currentMH2O.height; i++) {
					map[i] = i < size ? reader.ReadByte() : (byte)0x00;
				}
			}
			return map;
		}

		private float[] ReadHeights(MH2O currentMH2O, uint ofsData2b, int HeightMapLen, long offset, BinaryReader reader) {
			var heights = new float[HeightMapLen];
			if(ofsData2b != 0) {
				reader.BaseStream.Position = offset;
				for(int i = 0; i < HeightMapLen; i++) {
					var height = reader.ReadSingle();
					if(height == 0) {
						height = currentMH2O.heightLevel1;
					}
					heights[i] = height;
				}
			}
			else {
				for(int i = 0; i < HeightMapLen; i++) {
					heights[i] = currentMH2O.heightLevel1;
				}
			}
			return heights;
		}

		#region Nested type: MH2OHeader

		private struct MH2OHeader {
			public UInt32 ofsData1;
			public UInt32 ofsData2;
			public UInt32 used;
		}

		#endregion
	}
}