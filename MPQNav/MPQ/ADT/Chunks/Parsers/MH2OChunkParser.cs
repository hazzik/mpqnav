using System;
using System.IO;
using MPQNav.Util;

namespace MPQNav.MPQ.ADT.Chunks.Parsers {
	/// <summary>
	/// MH2O Chunk perser
	/// </summary>
	internal class MH2OChunkParser : ChunkParser<MH2O[,]> {
		/// <summary>
		/// MH2OChunkParser Perser
		/// </summary>
		/// <param name="br">Binary Stream</param>
		/// <param name="pAbsoluteStart">The position of header</param>
		public MH2OChunkParser(BinaryReader br, long pAbsoluteStart)
			: base("MH2O", br, pAbsoluteStart) {
		}

		/// <summary>
		/// Parse all MH2O element from file stream
		/// </summary>
		public override MH2O[,] Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			var result = new MH2O[16,16];
			var mh2oHeader = new MH2OHeader[256];
			for(int i = 0; i < 256; i++) {
				mh2oHeader[i].ofsData1 = Reader.ReadUInt32();
				mh2oHeader[i].used = Reader.ReadUInt32();
				mh2oHeader[i].ofsData2 = Reader.ReadUInt32();
			}
			for(int y = 0; y < 16; y++) {
				for(int x = 0; x < 16; x++) {
					result[x, y] = processMH20(mh2oHeader[y * 16 + x], AbsoluteStart);
				}
			}
			return result;
		}

		/// <summary>
		/// Processes an individual MH2O Chunk
		/// </summary>
		/// <param name="Header">MH20 Header Chunk</param>
		/// <param name="ofsMH20">MH20 OFFset</param>
		/// <returns>MH2O Filled with information</returns>
		private MH2O processMH20(MH2OHeader Header, long ofsMH20) {
			if(Header.used == 0) {
				return new MH2O { used = false };
			}

			Reader.BaseStream.Position = ofsMH20 + Header.ofsData1 + 2;

			var currentMH2O = new MH2O();
			currentMH2O.used = true;
			currentMH2O.type = ((MH2O.FluidType)Reader.ReadUInt16());
			currentMH2O.heightLevel1 = Reader.ReadSingle();
			currentMH2O.heightLevel2 = Reader.ReadSingle();
			currentMH2O.xOffset = Reader.ReadByte();
			currentMH2O.yOffset = Reader.ReadByte();
			currentMH2O.width = Reader.ReadByte();
			currentMH2O.height = Reader.ReadByte();

			UInt32 ofsData2a = Reader.ReadUInt32();
			UInt32 ofsData2b = Reader.ReadUInt32();


			currentMH2O.RenderBitMap = ReadRenderBitMap(currentMH2O, ofsData2a, ofsMH20 + ofsData2a, ofsData2b - ofsData2a);

			currentMH2O.heights = ReadHeights(currentMH2O, ofsData2b, (currentMH2O.width + 1) * (currentMH2O.height + 1), ofsMH20 + ofsData2b);

			return currentMH2O;
		}

		private byte[] ReadRenderBitMap(MH2O currentMH2O, uint ofsData2a, long offset, uint size) {
			var map = new byte[currentMH2O.height];
			if(ofsData2a != 0) {
				Reader.BaseStream.Position = offset;
				for(int i = 0; i < currentMH2O.height; i++) {
					map[i] = i < size ? Reader.ReadByte() : (byte)0x00;
				}
			}
			return map;
		}

		private float[] ReadHeights(MH2O currentMH2O, uint ofsData2b, int HeightMapLen, long offset) {
			var heights = new float[HeightMapLen];
			if(ofsData2b != 0) {
				Reader.BaseStream.Position = offset;
				for(int i = 0; i < HeightMapLen; i++) {
					var height = Reader.ReadSingle();
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