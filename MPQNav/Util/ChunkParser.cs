using System;
using System.IO;

namespace MPQNav.Util {
	internal abstract class ChunkParser<T>:ParserBase<T> {
		protected ChunkParser(string name, BinaryReader reader, long absoluteStart)
			: base(reader) {
			Name = name;
			Reader.BaseStream.Position = absoluteStart;
			string name1 = Reader.ReadStringReversed(4);
			if(name1 != Name) {
				throw new Exception("invalide chunk");
			}
			Size = Reader.ReadUInt32();
			AbsoluteStart = Reader.BaseStream.Position;
		}

		/// <summary>
		/// Return the chunk Name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Return the absolute position of MCIN chunk in the file stream
		/// </summary>
		public long AbsoluteStart { get; private set; }

		/// <summary>
		/// Return the size of MCIN chunk
		/// </summary>
		public uint Size { get; private set; }
	}
}