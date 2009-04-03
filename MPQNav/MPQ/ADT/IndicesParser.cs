using System;
using System.Collections.Generic;
using System.IO;

namespace MPQNav.ADT {
	public class IndicesParser {
		private readonly BinaryReader _reader;
		private readonly uint _absoluteStart;
		private readonly uint _boundingTrianglesCount;

		public IndicesParser(BinaryReader br, uint absoluteStart, uint boundingTrianglesCount) {
			_reader = br;
			_absoluteStart = absoluteStart;
			_boundingTrianglesCount = boundingTrianglesCount;
		}

		public uint AbsoluteStart {
			get { return _absoluteStart; }
		}

		public BinaryReader Reader {
			get { return _reader; }
		}

		public IList<int> Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			var indices = new List<int>();
			for(int v = 0; v < _boundingTrianglesCount; v = v + 3) {
				Int16 int1 = Reader.ReadInt16();
				Int16 int2 = Reader.ReadInt16();
				Int16 int3 = Reader.ReadInt16();

				indices.Add(int3);
				indices.Add(int2);
				indices.Add(int1);
			}
			return indices;
		}
	}
}