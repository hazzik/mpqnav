using System;

namespace MPQNav.ADT {
	/// <summary>
	/// Contains offsets (relative to 0x14) for some other chunks that appear in the file. Since the file follows a well-defined structure, this is redundant information.
	/// </summary>
	internal class MHDR {
		public uint Base { get; set; }
		/*000h*/
		public uint Pad { get; set; }
		/*004h*/
		public uint OffsInfo { get; set; }
		/*008h*/
		public uint OffsTex { get; set; }
		/*00Ch*/
		public uint OffsModels { get; set; }
		/*010h*/
		public uint OffsModelsIds { get; set; }
		/*014h*/
		public uint OffsMapObejcts { get; set; }
		/*018h*/
		public uint OffsMapObejctsIds { get; set; }
		/*01Ch*/
		public uint OffsDoodsDef { get; set; }
		/*020h*/
		public uint OffsObjectsDef { get; set; }
		/*024h*/
		public uint OffsFlightBoundary { get; set; }
		/*028h*/
		public uint OffsMH2O { get; set; }
		/*02Ch*/
		public uint Pad3 { get; set; }
		/*030h*/
		public uint Pad4 { get; set; }
		/*034h*/
		public uint Pad5 { get; set; }
		/*038h*/
		public uint Pad6 { get; set; }
		/*03Ch*/
		public uint Pad7 { get; set; }
		/*040h*/
	}
}