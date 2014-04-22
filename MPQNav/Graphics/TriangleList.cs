using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MPQNav.Graphics
{
    public class TriangleList
    {
        public TriangleList()
            : this(new List<int>(), new List<VertexPositionNormalColorTexture>())
        {
        }

        public TriangleList(IList<int> indices, IList<VertexPositionNormalColorTexture> vertices)
        {
            Indices = indices;
            Vertices = vertices;
        }

        public virtual IList<int> Indices { get; private set; }
        public virtual IList<VertexPositionNormalColorTexture> Vertices { get; private set; }

        public TriangleList Optimize()
        {
            IList<VertexPositionNormalColorTexture> vertices = Vertices;
            IList<int> indices = Indices;
            var hash = new Dictionary<VertexPositionNormalColorTexture, int>();
            var resultIndices = new List<int>();
            for (int i = 0; i < indices.Count; i++)
            {
                VertexPositionNormalColorTexture vertex = vertices[indices[i]];
                int index;
                if (!hash.TryGetValue(vertex, out index))
                {
                    index = hash.Count;
                    hash.Add(vertex, index);
                }
                resultIndices.Add(index);
            }
            return new TriangleList()
                       {
                           Indices = resultIndices.ToArray(),
                           Vertices = hash.Keys.ToArray(),
                       };
        }

        public TriangleList Transform(Vector3 origin, Matrix matrix)
        {
            return new TriangleList()
                       {
                           Indices = Indices,
                           Vertices = Vertices
                               .Select(v => new VertexPositionNormalColorTexture(
                                                Vector3.Transform(v.Position, matrix) + origin,
                                                Vector3.TransformNormal(v.Normal, matrix), v.Color)).ToList(),
                       };
        }
    }
}