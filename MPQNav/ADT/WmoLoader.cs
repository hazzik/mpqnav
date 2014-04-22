using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MPQNav.Chunks;
using MPQNav.Chunks.Parsers;
using MPQNav.Graphics;
using MPQNav.IO;

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
			MOHD mohd = null;
			var fileInfo = FileSystem.Instance;
			if (fileInfo.Exists(path) == false)
				throw new Exception(String.Format("File does not exist: {0}", path));

			using (var br = new BinaryReader(fileInfo.OpenRead(path)))
			{
			    while (br.BaseStream.Position < br.BaseStream.Length)
			    {
			        var name = br.ReadStringReversed(4);
			        var size = br.ReadUInt32();
			        var r = new BinaryReader(new MemoryStream(br.ReadBytes((int) size)));
			        switch (name)
			        {
			            case "MVER":
                            int version = new MVERChunkParser(size).Parse(r);
			                break;
			            case "MOHD":
                            mohd = new MOHDChunkParser(size).Parse(r);
			                break;
			        }
			    }
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
	    /// <param name="size"></param>
	    /// <returns></returns>
	    private static TriangleList LoadWMOSub(string fileName)
		{
			var path = fileName;
			var fileInfo = FileSystem.Instance;
			if (fileInfo.Exists(path) == false)
				throw new Exception(String.Format("File does not exist: {0}", path));

			using (var reader = new BinaryReader(fileInfo.OpenRead(path)))
			{
			    MOGP mopg = null;
			    while (reader.BaseStream.Position < reader.BaseStream.Length)
			    {
			        var name = reader.ReadStringReversed(4);
			        var size = reader.ReadUInt32();
			        var r = new BinaryReader(new MemoryStream(reader.ReadBytes((int) size)));
			        switch (name)
			        {
			            case "MVER":
			                var ver = new MVERChunkParser(size).Parse(r);
			                break;

			            case "MOGP":
			                mopg = new MOGPChunkParser(size).Parse(r);
			                break;
			            default:
			            {
			                break;
			            }
			        }
			    }
			    var vertices = new List<VertexPositionNormalColored>();
                for (var i = 0; i < mopg.vectors.Count; i++)
			    {
                    vertices.Add(new VertexPositionNormalColored(mopg.vectors[i], Color.Yellow, mopg.normals[i]));
			    }
                return new TriangleList(mopg.indices, vertices);
			}
		}
	}
}