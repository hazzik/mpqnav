using System.IO;

namespace MPQNav {
	public interface IParser<T> {
		BinaryReader Reader { get; }

		T Parse();
	}
}