using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MPQNav.Graphics
{
    public class TriangleList
    {
        public TriangleList()
            : this(new List<int>(), new List<VertexPositionNormalColored>())
        {
        }

        public TriangleList(IList<int> indices, IList<VertexPositionNormalColored> vertices)
        {
            Indices = indices;
            Vertices = vertices;
        }

        public virtual IList<int> Indices { get; private set; }
        public virtual IList<VertexPositionNormalColored> Vertices { get; private set; }

        public TriangleList Optimize()
        {
            IList<VertexPositionNormalColored> vertices = Vertices;
            IList<int> indices = Indices;
            var hash = new Dictionary<VertexPositionNormalColored, int>();
            var resultIndices = new List<int>();
            for (int i = 0; i < indices.Count; i++)
            {
                VertexPositionNormalColored vertex = vertices[indices[i]];
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
                               .Select(v => new VertexPositionNormalColored(
                                                Vector3.Transform(v.Position, matrix) + origin,
                                                v.Color,
                                                Vector3.TransformNormal(v.Normal, matrix))).ToList(),
                       };
        }
    }
}