using System;
using System.IO;
using MPQNav.ADT;

namespace MPQNav.Util.ADTParser {
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
		/// Parse all MH2O element from file strem
		/// </summary>
		public override MH2O[,] Parse() {
			return processMH2Os();
		}

		/// <summary>
		/// Procceses each MCNK in the ADT passed to it.
		/// </summary>
		/// <returns>Return All MH2O chunks</returns>
		private MH2O[,] processMH2Os() {
			var _MH2OMatrix = new MH2O[16,16];
			long ofsMH2O = AbsoluteStart;
			var _MH20Header = new MH20Header[256];
			if(ofsMH2O != 0) {
				//ofsMH2O += 8;
				Reader.BaseStream.Position = ofsMH2O;
				for(int i = 0; i < 256; i++) {
					_MH20Header[i].ofsData1 = Reader.ReadUInt32();
					_MH20Header[i].used = Reader.ReadUInt32();
					_MH20Header[i].ofsData2 = Reader.ReadUInt32();
				}
			}
			for(int y = 0; y < 16; y++) {
				for(int x = 0; x < 16; x++) {
					_MH2OMatrix[x, y] = processMH20(_MH20Header[y * 16 + x], ofsMH2O);
				}
			}
			return _MH2OMatrix;
		}

		/// <summary>
		/// Processes an individual MH2O Chunk
		/// </summary>
		/// <param name="Header">MH20 Header Chunk</param>
		/// <param name="ofsMH20">MH20 OFFset</param>
		/// <returns>MH2O Filled with information</returns>
		private MH2O processMH20(MH20Header Header, long ofsMH20) {
			var currentMH2O = new MH2O();
			//SearchChunk(br, "MH2O");
			if(Header.used == 0) {
				currentMH2O.used = false;
				return currentMH2O;
			}


			Reader.BaseStream.Position = ofsMH20 + Header.ofsData1 + 2;
			currentMH2O.used = true;
			currentMH2O.type = (MH2O.FluidType)Reader.ReadUInt16();
			currentMH2O.heightLevel1 = Reader.ReadSingle();
			currentMH2O.heightLevel2 = Reader.ReadSingle();
			currentMH2O.xOffset = Reader.ReadByte();
			currentMH2O.yOffset = Reader.ReadByte();
			currentMH2O.width = Reader.ReadByte();
			currentMH2O.height = Reader.ReadByte();

			UInt32 ofsData2a = Reader.ReadUInt32();
			UInt32 ofsData2b = Reader.ReadUInt32();

			int HeightMapLen = (currentMH2O.width + 1) * (currentMH2O.height + 1);

			currentMH2O.heights = new float[HeightMapLen];


			currentMH2O.RenderBitMap = new byte[currentMH2O.height];
			if(ofsData2a != 0) {
				Reader.BaseStream.Position = ofsMH20 + ofsData2a;
				for(int i = 0; i < currentMH2O.height; i++) {
					if(i < (ofsData2b - ofsData2a)) {
						currentMH2O.RenderBitMap[i] = Reader.ReadByte();
					}
					else {
						currentMH2O.RenderBitMap[i] = 0x00;
					}
				}
			}
			else {
				currentMH2O.RenderBitMap = new byte[currentMH2O.height];
			}

			if(ofsData2b != 0) {
				Reader.BaseStream.Position = ofsMH20 + ofsData2b;
				for(int i = 0; i < HeightMapLen; i++) {
					currentMH2O.heights[i] = Reader.ReadSingle();
					if(currentMH2O.heights[i] == 0) {
						currentMH2O.heights[i] = currentMH2O.heightLevel1;
					}
				}
			}
			else {
				for(int i = 0; i < HeightMapLen; i++) {
					currentMH2O.heights[i] = currentMH2O.heightLevel1;
				}
			}

			return currentMH2O;
		}

		#region Nested type: MH20Header

		private struct MH20Header {
			public UInt32 ofsData1;
			public UInt32 ofsData2;
			public UInt32 used;
		}

		#endregion
	}
}