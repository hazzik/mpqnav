using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MPQNav
{

    /// <summary>
    /// Struct outlining our custom vertex
    /// </summary>
    public struct VertexPositionNormalColorTexture : IVertexType
    {
        /// <summary>
        /// Vector3 Position for this vertex
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Normal vector for this Vertex
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Color of this vertex
        /// </summary>
        public Color Color;

        /// <summary>
        /// Constructor for a VertexPositionNormalColor
        /// </summary>
        /// <param name="position">Vector3 Position of the vertex</param>
        /// <param name="normal">Normal vector of the vertex</param>
        /// <param name="color">Color of the vertex</param>
        public VertexPositionNormalColorTexture(Vector3 position, Vector3 normal, Color color)
        {
            Position = position;
            Color = color;
            Normal = normal;
        }

        private static readonly VertexDeclaration VertexDeclaration =
            new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof (float)*3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(sizeof (float)*6, VertexElementFormat.Color, VertexElementUsage.Color, 0));

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}