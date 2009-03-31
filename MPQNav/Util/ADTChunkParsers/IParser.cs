using System;
using System.IO;

namespace MPQNav.Util {
	internal interface IParser<T> {
		BinaryReader Reader { get; }

		T Parse();
	}
}