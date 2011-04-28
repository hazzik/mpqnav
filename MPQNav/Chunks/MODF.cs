using System;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Chunks {
	/// <summary>
	/// MODF Class - WMO Placement Information
	/// </summary>
	public class MODF : IModelDescriptor
	{
		/// <summary>
		/// Filename of the WMO
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Unique ID of the WMO in this ADT
		/// </summary>
		public uint UniqId { get; set; }

		/// <summary>
		/// Position of the WMO
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Rotation of the WMO
		/// </summary>
		public Vector3 Rotation { get; set; }

		public float Scale
		{
			get { return 1; }
		}
	}
}