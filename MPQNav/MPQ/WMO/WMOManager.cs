using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.ADT;
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
		/// <param name="fileName">Filename of the WMO from the Adt - i.e. world\wmo\azeroth\buildings\redridge_stable\redridge_stable.wmo</param>
		/// <param name="filePath">Base path to where the MPQ is extract - i.e. c:\mpq\</param>
		/// <param name="currentMODF">MODF (placement informatio for this WMO)</param>
		public void addWMO(String fileName, String filePath, MODF currentMODF) {
			if(!File.Exists(filePath + fileName)) {
				throw new Exception("File does not exist: " + filePath + fileName);
			}

			addFileName(fileName);

			var br = new BinaryReader(File.OpenRead(filePath + fileName));
			var currentWMO = new WMO();
			currentWMO.Name = fileName;
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

			currentWMO.createAABB(new Vector3(bb1_x, bb1_y, bb1_z), new Vector3(bb2_x, bb2_y, bb2_z));
			currentWMO.TotalGroups = (int)groupsCount;
			for(int wmoGroup = 0; wmoGroup < groupsCount; wmoGroup++) {
				var currentFileName = string.Format("{0}_{1:D3}.wmo", currentWMO.Name.Substring(0, currentWMO.Name.Length - 4),
				                                       wmoGroup);
				currentWMO.addWMO_Sub(processWMOSub(filePath + currentFileName, wmoGroup));
			}
			var position = currentMODF.Position;
			var rotation = new Vector3(currentMODF.OrientationA, currentMODF.OrientationB, currentMODF.OrientationC);
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
			var br = new BinaryReader(File.OpenRead(path));

			var offsMOVI = (int)FileChunkHelper.SearchChunk(br, "MOVI").StartPosition;
			//MPQ.findChunk(br, "IVOM");

			var offsMOVT = (int)FileChunkHelper.SearchChunk(br, "MOVT").StartPosition;
			//MPQ.findChunk(br, "TVOM");

			var offsMONR = (int)FileChunkHelper.SearchChunk(br, "MONR").StartPosition;
			//MPQ.findChunk(br, "RNOM");

			var offsMOTV = (int)FileChunkHelper.SearchChunk(br, "MOVT").StartPosition;
			//MPQ.findChunk(br, "VTOM");
			processMOVI(br, offsMOVI, offsMOVT, currentWMOSub);
			processMOVT(br, offsMOVT, offsMONR, currentWMOSub);
			processMONR(br, offsMONR, offsMOTV, currentWMOSub);
			return currentWMOSub;
		}

		/// <summary>
		/// WMO Vertex Index List
		/// </summary>
		/// <param name="br">Binary Reader with the WMO Loaded</param>
		/// <param name="start_offset">Starting offset in the reader for the MOVI Chunk</param>
		/// <param name="end_offset">Ending offset in the reader for the MOVI Chunk</param>
		/// <param name="currentWMOSUB">Current working WMO_Sub</param>
		public void processMOVI(BinaryReader br, int start_offset, int end_offset, WMO.WMO_Sub currentWMOSUB) {
			br.BaseStream.Position = start_offset + 8;
			while(br.BaseStream.Position < end_offset) {
				short one = br.ReadInt16();
				short two = br.ReadInt16();
				short three = br.ReadInt16();

				currentWMOSUB._MOVI.IndiciesList.Add(three);
				currentWMOSUB._MOVI.IndiciesList.Add(two);
				currentWMOSUB._MOVI.IndiciesList.Add(one);
			}
		}

		/// <summary>
		/// WMO Vertex List
		/// </summary>
		/// <param name="br">Binary reader with the WMO Loaded</param>
		/// <param name="start_offset">Starting offset in the binary reader for the MOVT Chunk</param>
		/// <param name="end_offset">Ending offset in the binary reader for the MOVT Chunk</param>
		/// <param name="currentWMOSUB">Current working WMO_Sub</param>
		public void processMOVT(BinaryReader br, int start_offset, int end_offset, WMO.WMO_Sub currentWMOSUB) {
			br.BaseStream.Position = start_offset + 8;
			while(br.BaseStream.Position < end_offset) {
				float vect_x = (br.ReadSingle() * -1);
				float vect_z = br.ReadSingle();
				float vect_y = br.ReadSingle();
				currentWMOSUB._MOVT.VerticiesList.Add(new Vector3(vect_x, vect_y, vect_z));
			}
		}

		/// <summary>
		/// WMO Normal Information
		/// </summary>
		/// <param name="br">Binary reader with the WMO loaded</param>
		/// <param name="start_offset">Starting offset for the MONR chunk</param>
		/// <param name="end_offset">Ending offset for the MONR Chunk</param>
		/// <param name="currentWMOSUB">Current working WMO Sub</param>
		public void processMONR(BinaryReader br, int start_offset, int end_offset, WMO.WMO_Sub currentWMOSUB) {
			br.BaseStream.Position = start_offset + 8;
			while(br.BaseStream.Position < end_offset) {
				float vect_x = (br.ReadSingle() * -1);
				float vect_z = br.ReadSingle();
				float vect_y = br.ReadSingle();
				currentWMOSUB._MONR.NormalsList.Add(new Vector3(vect_x, vect_y, vect_z));
			}
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