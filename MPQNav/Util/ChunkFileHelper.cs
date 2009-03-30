using System;
using System.IO;

namespace MPQNav.Util {
	internal abstract class ChunkParser {
		protected string _Name;
		protected long _pStart;
		protected uint _Size;
		protected BinaryReader br;

		public abstract string Name { get; }

		public abstract long AbsoluteStart { get; }

		public abstract uint Size { get; }
	}
}