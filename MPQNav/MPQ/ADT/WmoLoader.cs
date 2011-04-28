using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Chunks;
using MPQNav.Chunks.Parsers;
using MPQNav.Graphics;
using MPQNav.IO;
using MPQNav.Util;

namespace MPQNav.ADT
{
	internal class WmoLoader : IModelLoader
	{
		public Model Load(IModelDescriptor modf)
		{
			return LoadWMO(modf.FileName)
				.Transform(modf.Position, modf.Rotation, modf.Scale);
		}

		/// <summary> Loads WMO from file </summary>
		/// <param name="fileName">Full name of file of the WMO</param>
		/// <returns>Loaded WMO</returns>
		private static Model LoadWMO(string fileName)
		{
			var path = fileName;
			MOHD mohd;
			var fileInfo = FileInfoFactory.Create();
			if (fileInfo.Exists(path) == false)
				throw new Exception(String.Format("File does not exist: {0}", path));

			using (var br = new BinaryReader(fileInfo.OpenRead(path)))
			{
				int version = new MVERChunkParser(br, br.BaseStream.Position).Parse();
				mohd = new MOHDChunkParser(br, br.BaseStream.Position).Parse();
			}

			var list = new TriangleListCollection();
			for (int wmoGroup = 0; wmoGroup < mohd.GroupsCount; wmoGroup++)
			{
				list.Add(LoadWMOSub(String.Format("{0}_{1:D3}.wmo", fileName.Substring(0, fileName.Length - 4), wmoGroup)));
			}

			return new Model(list);
		}

		/// <summary>
		/// Gets a WMO_Sub from the WMO Group file
		/// </summary>
		/// <param name="fileName">Full Filename of the WMO_Sub</param>
		/// <returns></returns>
		private static TriangleList LoadWMOSub(string fileName)
		{
			var path = fileName;
			var fileInfo = FileInfoFactory.Create();
			if (fileInfo.Exists(path) == false)
				throw new Exception(String.Format("File does not exist: {0}", path));

			using (var reader = new BinaryReader(fileInfo.OpenRead(path)))
			{
				var indices = new MOVIChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVI").StartPosition).Parse();
				var vectors = new MOVTChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MOVT").StartPosition).Parse();
				var normals = new MONRChunkParser(reader, FileChunkHelper.SearchChunk(reader, "MONR").StartPosition).Parse();
				var vertices = new List<VertexPositionNormalColored>();
				for (var i = 0; i < vectors.Count; i++)
				{
					vertices.Add(new VertexPositionNormalColored(vectors[i], Color.Yellow, normals[i]));
				}
				return new TriangleList(indices, vertices);
			}
		}
	}
}