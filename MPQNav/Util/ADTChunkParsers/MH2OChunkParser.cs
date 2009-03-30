using System;
using System.Collections.Generic;
using MPQNav.ADT;
using System.IO;

namespace MPQNav.Util.ADTParser
{
    /// <summary>
    /// MH2O Chunk perser
    /// </summary>
    class MH2OChunkParser : ChunkParser
    {
        /// <summary>
        /// MH2OChunkParser Perser
        /// </summary>
        /// <param name="br">Binary Stream</param>
        /// <param name="pAbsoluteStart">The position of header</param>
        public MH2OChunkParser(BinaryReader br, long pAbsoluteStart)
        {
            this.br = br;
            _Name = "MH2O";
            if (pAbsoluteStart == 0)
            {
                _pStart = 0;
                _Size = 0;
            }
            else
            {
                br.BaseStream.Position = pAbsoluteStart + 4;
                _Size = br.ReadUInt32();
                _pStart = pAbsoluteStart + 8;
            }
        }

        /// <summary>
        /// Return the chunk Name
        /// </summary>
        public override string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Return the absolute position of MH2O chunk in the file stream
        /// </summary>
        public override long AbsoluteStart
        {
            get { return _pStart; }
        }

        /// <summary>
        /// Return the size of MH2O chunk
        /// </summary>
        public override uint Size
        {
            get { return _Size; }
        }

        /// <summary>
        /// Parse all MH2O element from file strem
        /// </summary>
        public MH2O[,] Parse()
        {
            return processMH2Os();
        }

        private struct MH20Header
        {
            public UInt32 used;
            public UInt32 ofsData1;
            public UInt32 ofsData2;
        }

        /// <summary>
        /// Procceses each MCNK in the ADT passed to it.
        /// </summary>
        /// <returns>Return All MH2O chunks</returns>
        private MH2O[,] processMH2Os()
        {
            MH2O[,] _MH2OMatrix = new MH2O[16, 16];
            long ofsMH2O = _pStart;
            MH20Header[] _MH20Header = new MH20Header[256];
            if (ofsMH2O != 0)
            {
                //ofsMH2O += 8;
                br.BaseStream.Position = ofsMH2O;
                for (int i = 0; i < 256; i++)
                {
                    _MH20Header[i].ofsData1 = br.ReadUInt32();
                    _MH20Header[i].used = br.ReadUInt32();
                    _MH20Header[i].ofsData2 = br.ReadUInt32();
                }
            }
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
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
        private MH2O processMH20(MH20Header Header, long ofsMH20)
        {
            MH2O currentMH2O = new MH2O();
            //SearchChunk(br, "MH2O");
            if (Header.used == 0)
            {
                currentMH2O.used = false;
                return currentMH2O;
            }


            br.BaseStream.Position = ofsMH20 + Header.ofsData1 + 2;
            currentMH2O.used = true;
            currentMH2O.type = (ADT.MH2O.FluidType)br.ReadUInt16();
            currentMH2O.heightLevel1 = br.ReadSingle();
            currentMH2O.heightLevel2 = br.ReadSingle();
            currentMH2O.xOffset = br.ReadByte();
            currentMH2O.yOffset = br.ReadByte();
            currentMH2O.width = br.ReadByte();
            currentMH2O.height = br.ReadByte();

            UInt32 ofsData2a = br.ReadUInt32();
            UInt32 ofsData2b = br.ReadUInt32();

            int HeightMapLen = (currentMH2O.width + 1) * (currentMH2O.height + 1);

            currentMH2O.heights = new float[HeightMapLen];


            currentMH2O.RenderBitMap = new byte[currentMH2O.height];
            if (ofsData2a != 0)
            {
                br.BaseStream.Position = ofsMH20 + ofsData2a;
                for (int i = 0; i < currentMH2O.height; i++)
                {
                    if (i < (ofsData2b - ofsData2a))
                        currentMH2O.RenderBitMap[i] = br.ReadByte();
                    else
                        currentMH2O.RenderBitMap[i] = 0x00;
                }
            }
            else
            {
                currentMH2O.RenderBitMap = new byte[currentMH2O.height];
            }

            if (ofsData2b != 0)
            {
                br.BaseStream.Position = ofsMH20 + ofsData2b;
                for (int i = 0; i < HeightMapLen; i++)
                {
                    currentMH2O.heights[i] = br.ReadSingle();
                    if (currentMH2O.heights[i] == 0)
                        currentMH2O.heights[i] = currentMH2O.heightLevel1;
                }
            }
            else
            {
                for (int i = 0; i < HeightMapLen; i++)
                {
                    currentMH2O.heights[i] = currentMH2O.heightLevel1;
                }
            }

            return currentMH2O;
        }
    }


}