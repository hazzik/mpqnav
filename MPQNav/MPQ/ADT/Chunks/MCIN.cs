using System;

namespace MPQNav.MPQ.ADT.Chunks {
	internal class MCIN {
/*000h*/
		public uint Offset { get; set; }
/*004h*/
		public uint Size { get; set; }
/*008h*/
		public uint Flags { get; set; }
/*00Ch*/
		public uint AsyncId { get; set; }
/*010h*/
	}
}