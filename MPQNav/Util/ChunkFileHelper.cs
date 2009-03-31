﻿using System;
using System.IO;
using System.Text;
using MPQNav.ADT;

namespace MPQNav.Util {
	internal abstract class ChunkParser<T> {
		protected ChunkParser(string name, BinaryReader reader, long absoluteStart) {
			Name = name;
			Reader = reader;
			Reader.BaseStream.Position = absoluteStart;
			var bytes = Reader.ReadBytes(4);
			Array.Reverse(bytes);
			var name1 = Encoding.ASCII.GetString(bytes);
			if(name1 != Name) {
				throw new Exception("invalide chunk");
			}
			Size = Reader.ReadUInt32();
			AbsoluteStart = Reader.BaseStream.Position;
		}

		public BinaryReader Reader { get; private set; }

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

		/// <summary>
		/// Parse all {T} element from file stream
		/// </summary>
		public abstract T Parse();
	}
}