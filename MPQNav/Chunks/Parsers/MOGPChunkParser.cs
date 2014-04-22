using System.IO;
using MPQNav.ADT;
using MPQNav.IO;
using MPQNav.Util;

namespace MPQNav.Chunks.Parsers
{
    internal class MOGPChunkParser : ChunkParser<MOGP>
    {
        public MOGPChunkParser(uint size) : base(size)
        {
        }

        public override MOGP Parse(BinaryReader reader)
        {
            var result = new MOGP();
            byte[] header = reader.ReadBytes(0x44); // Do not care the header
            while (reader.BaseStream.Position < Size)
            {
                string name = reader.ReadStringReversed(4);
                uint size = reader.ReadUInt32();
                var r = new BinaryReader(new MemoryStream(reader.ReadBytes((int) size)));
                switch (name)
                {
                    case "MOVI":
                        result.indices = new MOVIChunkParser(size).Parse(r);
                        break;
                    case "MOVT":
                        result.vectors = new MOVTChunkParser(size).Parse(r);
                        break;
                    case "MONR":
                        result.normals = new MONRChunkParser(size).Parse(r);
                        break;
                }
            }
            return result;
        }
    }
}