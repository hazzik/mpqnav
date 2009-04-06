using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace MPQNav.ADT {
	internal class VectorsListParser {
		private readonly BinaryReader _reader;
		private readonly uint _absoluteStart;
		private readonly uint _verticesCount;

		public VectorsListParser(BinaryReader br, uint absoluteStart, uint boundingVerticesCount) {
			_reader = br;
			_absoluteStart = absoluteStart;
			_verticesCount = boundingVerticesCount;
		}

		public BinaryReader Reader {
			get { return _reader; }
		}

		public uint AbsoluteStart {
			get { return _absoluteStart; }
		}

		public IList<Vector3> Parse() {
			Reader.BaseStream.Position = AbsoluteStart;
			var vectors = new List<Vector3>();
			for(int v = 0; v < _verticesCount; v++) {
				var x = Reader.ReadSingle() * -1;
				var z = Reader.ReadSingle();
				var y = Reader.ReadSingle();
				vectors.Add(new Vector3(x, y, z));
			}
			return vectors;
		}
	}
}