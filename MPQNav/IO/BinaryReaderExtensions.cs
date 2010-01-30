using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MPQNav.IO {
    public static class BinaryReaderExtensions {
        public static string ReadStringReversed(this BinaryReader reader, int count) {
            var bytes = reader.ReadBytes(count);
            Array.Reverse(bytes);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string ReadCString(this BinaryReader reader) {
            return ReadCString(reader, Encoding.UTF8);
        }

        public static string ReadCString(this BinaryReader reader, Encoding encoding) {
            byte b;
            var buff = new List<byte>();
            while((b = reader.ReadByte()) != 0) {
                buff.Add(b);
            }
            return encoding.GetString(buff.ToArray());
        }
    }
}