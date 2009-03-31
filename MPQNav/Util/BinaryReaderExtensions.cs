using System;
using System.IO;
using System.Text;

namespace MPQNav.Util {
	public static class BinaryReaderExtensions {
		public static string ReadStringReversed(this BinaryReader reader, int count) {
			var bytes = reader.ReadBytes(count);
			Array.Reverse(bytes);
			return Encoding.ASCII.GetString(bytes);
		}
	}
}

