using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using MPQNav.Chunks;
using MPQNav.Graphics;
using MPQNav.IO;

namespace MPQNav.ADT
{
	internal class M2Loader : IModelLoader
	{
		public Model Load(IModelDescriptor descriptor)
		{
			return LoadM2(descriptor.FileName)
				.Transform(descriptor.Position, descriptor.Rotation, descriptor.Scale);
		}

		private static Model LoadM2(string fileName)
		{
			string path = fileName;
			if (path.Substring(path.Length - 4) == ".mdx")
			{
				path = path.Substring(0, path.Length - 4) + ".m2";
			}
			var fileInfo = FileInfoFactory.Create();
			if (!fileInfo.Exists(path))
			{
				throw new Exception(String.Format("File does not exist: {0}", path));
			}

			using (var br = new BinaryReader(fileInfo.OpenRead(path)))
			{
				br.BaseStream.Position = 60; //wotlk
				uint numberOfVerts = br.ReadUInt32();
				uint vertsOffset = br.ReadUInt32();
				uint numberOfViews = br.ReadUInt32();
				//UInt32 viewsOffset = br.ReadUInt32(); //now in skins

				br.BaseStream.Position = 216; //wotlk
				uint nBoundingTriangles = br.ReadUInt32();
				uint ofsBoundingTriangles = br.ReadUInt32();
				uint nBoundingVertices = br.ReadUInt32();
				uint ofsBoundingVertices = br.ReadUInt32();
				uint nBoundingNormals = br.ReadUInt32();
				uint ofsBoundingNormals = br.ReadUInt32();

				var indices = new IndicesParser(br, ofsBoundingTriangles, nBoundingTriangles).Parse();

				var vectors = new VectorsListParser(br, ofsBoundingVertices, nBoundingVertices).Parse();

				//var normals = new VectorsListParser(br, ofsBoundingNormals, nBoundingNormals).Parse();

				var vertices = vectors
					.Select(t => new VertexPositionNormalColored(t, Color.Red, Vector3.Up))
					.ToList();

				var list = new TriangleList(indices, vertices);
				return new Model(list);
			}
		}
	}
}