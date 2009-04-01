using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.MPQ.ADT.Chunks;
using MPQNav.MPQ.WMO.Chunks.Parsers;
using MPQNav.Util;

namespace MPQNav.MPQ.ADT {
	internal class WMOManager {
		#region variables

		/// <summary>
		/// List of filenames managed by this WMOManager
		/// </summary>
		private readonly List<String> _names = new List<String>();

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
		/// Adds a WMO Filename to the Manager
		/// </summary>
		/// <param name="name">The filename to add</param>
		public void addFileName(String name) {
			_names.Add(name);
		}

		/// <summary>
		/// Adds a WMO to the manager
		/// </summary>
		/// <param name="modf">MODF (placement informatio for this WMO)</param>
		public void AddWMO(MODF modf) {
			if(!File.Exists(MpqNavSettings.MpqPath + modf.FileName)) {
				throw new Exception("File does not exist: " + MpqNavSettings.MpqPath + modf.FileName);
			}

			addFileName(modf.FileName);

			var br = new BinaryReader(File.OpenRead(MpqNavSettings.MpqPath + modf.FileName));
			br.ReadBytes(20); // Skip the header
			UInt32 nTextures = br.ReadUInt32();
			UInt32 groupsCount = br.ReadUInt32(); // This is the number of "sub-wmos" or group files that we need to read
			UInt32 nPortals = br.ReadUInt32();
			UInt32 nLights = br.ReadUInt32();
			UInt32 nModels = br.ReadUInt32();
			UInt32 nDoodads = br.ReadUInt32();
			UInt32 nSets = br.ReadUInt32();
			UInt32 ambientColor = br.ReadUInt32();

			UInt32 WMOID = br.ReadUInt32(); // Column 2 in the WMOAreaTable.dbc

			float bb1_x = (br.ReadSingle() * -1);
			float bb1_z = br.ReadSingle();
			float bb1_y = br.ReadSingle();
			float bb2_x = (br.ReadSingle() * -1);
			float bb2_z = br.ReadSingle();
			float bb2_y = br.ReadSingle();

			var currentWMO = new WMO();
			currentWMO.Name = modf.FileName;
			currentWMO.createAABB(new Vector3(bb1_x, bb1_y, bb1_z), new Vector3(bb2_x, bb2_y, bb2_z));
			currentWMO.TotalGroups = (int)groupsCount;
			for(int wmoGroup = 0; wmoGroup < groupsCount; wmoGroup++) {
				var currentFileName = string.Format("{0}_{1:D3}.wmo", currentWMO.Name.Substring(0, currentWMO.Name.Length - 4), wmoGroup);
				currentWMO.addWMO_Sub(processWMOSub(MpqNavSettings.MpqPath + currentFileName, wmoGroup));
			}
			var position = modf.Position;
			var rotation = new Vector3(modf.OrientationA, modf.OrientationB, modf.OrientationC);
			currentWMO.Transform(position, rotation, rad);
			_wmos.Add(currentWMO);
		}


		/// <summary>
		/// Gets a WMO_Sub from the WMO Group file
		/// </summary>
		/// <param name="group_index">Current index in the WMO Group</param>
		/// <param name="path">Full Filename of the WMO_Sub</param>
		/// <returns></returns>
		public WMO.WMO_Sub processWMOSub(string path, int group_index) {
			if(!File.Exists(path)) {
				throw new Exception("File does not exist: " + path);
			}
			var currentWMOSub = new WMO.WMO_Sub(group_index);

			using(var reader = new BinaryReader(File.OpenRead(path))) {
				currentWMOSub._MOVI = new MOVIChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVI").StartPosition).Parse();
				currentWMOSub._MOVT = new MOVTChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVT").StartPosition).Parse();
				currentWMOSub._MONR = new MONRChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MONR").StartPosition).Parse();
			}

			return currentWMOSub;
		}

		/// <summary>
		/// Gets the filename of a WMO at the given index 
		/// </summary>
		/// <param name="index">Index of the Filename</param>
		/// <returns>The filename upon success, INVALID upon failure</returns>
		public String getFilename(int index) {
			return _names.Count > index ? _names[index] : "INVALID";
		}
	}
}