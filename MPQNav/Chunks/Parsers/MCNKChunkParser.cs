using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers {
    internal class MCNKChunkParser : ChunkParser<MCNK>
    {
        public MCNKChunkParser(uint size)
            : base(size)
        {
        }

        public override MCNK Parse(BinaryReader reader)
        {
            var start = reader.BaseStream.Position;
            var end = reader.BaseStream.Position + Size;

            reader.ReadBytes(4); // Data that's not needed
            var index_x = (int)reader.ReadUInt32();
            var index_y = (int)reader.ReadUInt32();
            reader.BaseStream.Position = start + 0x3c; // Get off the header
            uint holes = reader.ReadUInt32() & 0x00FF;
            if (holes > 0)
            {
                Console.WriteLine(Convert.ToString(holes, 2));
            }

            reader.BaseStream.Position = start + 0x068;
            //br.BaseStream.Position += 28; // Get past the data we don't want   
            //UInt32 offsLiquid = br.ReadUInt32(); // Offset to the liquid information
            //UInt32 sizeLiquid = br.ReadUInt32(); // If there's no liquid information it will be 8

            float z = reader.ReadSingle();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            var currentMCNK = new MCNK(x, y, z, (UInt16) holes);
            reader.ReadBytes(12);   // Read the reaming 12 bytes of data and 8 for the next header which is a MCVT chunk

            while (reader.BaseStream.Position < end)
            {
                var name = reader.ReadStringReversed(4);
                var size = reader.ReadInt32();
                var r = new BinaryReader(new MemoryStream(reader.ReadBytes(size)));
                switch (name)
                {
                    case "MCVT":
                        for (int i = 0; i < 145; i++)
                        {
                            currentMCNK._MCVT.heights[i] = r.ReadSingle();
                        }
                        break;
                    case "MCNR":
                        for (int i = 0; i < 145; i++)
                        {
                            sbyte normalZ = r.ReadSByte();
                            sbyte normalX = r.ReadSByte();
                            sbyte normalY = r.ReadSByte();
                            currentMCNK._MCNR.normals[i] = new Vector3(-(float) normalX/127.0f, normalY/127.0f, -(float) normalZ/127.0f);
                        }
                        break;
                    default:
                    {
                        break;
                    }
                }

            }

            return currentMCNK;
        }
    }
}