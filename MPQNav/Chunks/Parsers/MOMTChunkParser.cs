using System.IO;
using MPQNav.Util;

namespace MPQNav.ADT
{
    internal class MOMTChunkParser : ChunkParser<MOMT>
    {
        public MOMTChunkParser(uint size) : base(size)
        {
        }

        public override MOMT Parse(BinaryReader reader)
        {
            var start = reader.BaseStream.Position;
            var end = start + Size;
            return new MOMT
            {
                Flags1 = reader.ReadUInt32(),
                Shader = reader.ReadUInt32(),
                BlendMode = reader.ReadUInt32(),
                Texture1 = reader.ReadUInt32(),
                Colour1 = reader.ReadUInt32(),
                Flags_1 = reader.ReadUInt32(),
                Texture2 = reader.ReadUInt32(),
                Colour2 = reader.ReadUInt32(),
                Flags_2 = reader.ReadUInt32(),
                Texture3 = reader.ReadUInt32(),
                Colour3 = reader.ReadUInt32(),
            };
        }
    }
}