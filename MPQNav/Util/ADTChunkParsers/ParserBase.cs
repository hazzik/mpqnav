using System;
using System.IO;

namespace MPQNav.Util {
	internal abstract class ParserBase<T> : IParser<T> {
		protected ParserBase(BinaryReader reader) {
			Reader = reader;
		}

		public BinaryReader Reader { get; private set; }

		public abstract T Parse();
	}
}