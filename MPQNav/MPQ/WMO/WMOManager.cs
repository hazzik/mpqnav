using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Collision._3D;
using MPQNav.MPQ.ADT.Chunks;
using MPQNav.MPQ.ADT.Chunks.Parsers;
using MPQNav.MPQ.WMO.Chunks.Parsers;
using MPQNav.Util;

namespace MPQNav.MPQ.ADT {
	internal class WMOManager {
		#region variables

		/// <summary>
		/// List of WMOs managed by this WMOManager
		/// </summary>
		public List<WMO> _wmos = new List<WMO>();

		/// <summary>
		/// 1 degree = 0.0174532925 radians
		/// </summary>
		private const float rad = 0.0174532925f;

		#endregion

		#region rendering variables

		/// <summary>
		/// List of indices for rendering
		/// </summary>
		public List<int> indicies = new List<int>();

		/// <summary>
		/// List of vertices for rendering
		/// </summary>
		public List<VertexPositionNormalColored> vertices = new List<VertexPositionNormalColored>();

		#endregion

		#region consructors

		#endregion

		/// <summary>
		/// Adds a WMO to the manager
		/// </summary>
		/// <param name="modf">MODF (placement informatio for this WMO)</param>
		public void AddWMO(MODF modf) {
			if(!File.Exists(MpqNavSettings.MpqPath + modf.FileName)) {
				throw new Exception("File does not exist: " + MpqNavSettings.MpqPath + modf.FileName);
			}

			var br = new BinaryReader(File.OpenRead(MpqNavSettings.MpqPath + modf.FileName));
			var version = new MVERChunkParser(br, br.BaseStream.Position).Parse();
			var mohd = new MOHDChunkParser(br, br.BaseStream.Position).Parse();

			var currentWMO = new WMO();
			currentWMO.Name = modf.FileName;
			currentWMO.AABB = new AABB(mohd.BoundingBox1, mohd.BoundingBox2);
			currentWMO.TotalGroups = (int)mohd.GroupsCount;
			for(int wmoGroup = 0; wmoGroup < mohd.GroupsCount; wmoGroup++) {
				var currentFileName = string.Format("{0}_{1:D3}.wmo", currentWMO.Name.Substring(0, currentWMO.Name.Length - 4), wmoGroup);
				currentWMO.addWMO_Sub(ProcessWMOSub(currentFileName, wmoGroup));
			}
			currentWMO.Transform(modf.Position, modf.Rotation, rad);
			_wmos.Add(currentWMO);
		}


		/// <summary>
		/// Gets a WMO_Sub from the WMO Group file
		/// </summary>
		/// <param name="wmoGroup">Current index in the WMO Group</param>
		/// <param name="fileName">Full Filename of the WMO_Sub</param>
		/// <returns></returns>
		public WMO.WMO_Sub ProcessWMOSub(string fileName, int wmoGroup) {
			var path = MpqNavSettings.MpqPath + fileName;
			if(!File.Exists(path)) {
				throw new Exception("File does not exist: " + path);
			}

			var currentWMOSub = new WMO.WMO_Sub(wmoGroup);

			using(var reader = new BinaryReader(File.OpenRead(path))) {
				currentWMOSub._MOVI = new MOVIChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVI").StartPosition).Parse();
				currentWMOSub._MOVT = new MOVTChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVT").StartPosition).Parse();
				currentWMOSub._MONR = new MONRChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MONR").StartPosition).Parse();
			}

			return currentWMOSub;
		}
	}
}