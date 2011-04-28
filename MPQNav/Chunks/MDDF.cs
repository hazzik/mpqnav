using System;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Chunks {
	/// <summary>
	/// MDDF Chunk Class - Placement information for M2 Models
	/// </summary>
	public class MDDF : IModelDescriptor {
		///<summary>
		///</summary>
		public MDDF() {
			Scale = 1;
		}

		/// <summary> Filename of the M2 </summary>
		public string FileName { get; set; }

		/// <summary> Unique ID of the M2 in this ADT </summary>
		public uint UniqId { get; set; }

		/// <summary> Position of the M2 </summary>
		public Vector3 Position { get; set; }

		/// <summary> Rotation of the M2 </summary>
		public Vector3 Rotation { get; set; }

		/// <summary> Scale factor of the M2 </summary>
		public float Scale { get; set; }
	}
}