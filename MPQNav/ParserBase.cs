using System.IO;

namespace MPQNav {
	public abstract class ParserBase<T> : IParser<T> {
		protected ParserBase(BinaryReader reader) {
			Reader = reader;
		}

		public BinaryReader Reader { get; private set; }

		public abstract T Parse();
	}
}