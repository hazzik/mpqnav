using System;
using System.Collections.Generic;
using MPQNav.ADT;
using System.IO;
using Microsoft.Xna.Framework;

namespace MPQNav.Util.ADTParser
{
    /// <summary>
    /// MCNK Chunk perser
    /// </summary>
    class MCNKChunkParser : ChunkParser
    {
        /// <summary>
        /// MCNKChunkParser Perser
        /// </summary>
        /// <param name="br">Binary Stream</param>
        /// <param name="pAbsoluteStart">The position of MCNK</param>
        public MCNKChunkParser(BinaryReader br, long pAbsoluteStart)
        {
            this.br = br;
            _Name = "MCNK";
            br.BaseStream.Position = pAbsoluteStart + 4;
            _Size = br.ReadUInt32();
            _pStart = pAbsoluteStart + 8;
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
        /// Parse all MCNK element from file strem
        /// </summary>
        /// <param name="MCIN">MCIN List af the adt file</param>
        public MCNK[,] Parse(MCIN[] MCIN)
        {
            MCNK[,] MCNK = new MCNK[16,16];
            int count = 0;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    MCNK[x, y] = processMCNK(MCIN[count].offset);
                    count++;
                }
            }
            return MCNK;// processMH2Os();
        }

        /// <summary>
        /// Processes an individual MCNK Chunk
        /// </summary>
        /// <param name="br">Binary Reader with the ADT loaded</param>
        /// <param name="offset">Offset to the MCNK Chunk</param>
        /// <returns>MCNK Filled with information</returns>
        private MCNK processMCNK(UInt32 offset)
        {
            br.BaseStream.Position = offset + 12; // Get off the header
            //br.ReadBytes(4); // Data that's not needed
            int index_x = (int)br.ReadUInt32();
            int index_y = (int)br.ReadUInt32();
            br.BaseStream.Position = offset + 0x08 + 0x03C; // Get off the header
            uint holes = br.ReadUInt32() & 0x00FF;
            if (holes>0)
                Console.WriteLine(Convert.ToString(holes,2));


            br.BaseStream.Position = offset + 0x08 + 0x068;
            //br.BaseStream.Position += 28; // Get past the data we don't want   
            //UInt32 offsLiquid = br.ReadUInt32(); // Offset to the liquid information
            //UInt32 sizeLiquid = br.ReadUInt32(); // If there's no liquid information it will be 8

            float z = (float)br.ReadSingle();
            float x = (float)br.ReadSingle();
            float y = (float)br.ReadSingle();
            MCNK currentMCNK = new MCNK(x, y, z,(UInt16) holes);
            br.ReadBytes(20); // Read the reaming 12 bytes of data and 8 for the next header which is a MCVT chunk
            for (int i = 0; i < 145; i++)
            {
                currentMCNK._MCVT.heights[i] = (float)br.ReadSingle();
            }
            br.ReadBytes(8); // We're going to read past the next header which is for the normals

            sbyte normalZ = 0;
            sbyte normalX = 0;
            sbyte normalY = 0;
            for (int i = 0; i < 145; i++)
            {
                normalZ = br.ReadSByte();
                normalX = br.ReadSByte();
                normalY = br.ReadSByte();
                currentMCNK._MCNR.normals[i] = new Vector3(-(float)normalX / 127.0f, (float)normalY / 127.0f, -(float)normalZ / 127.0f);
            }
            return currentMCNK;
        }

    }


}