using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.Chunks
{
    internal class MOGP
    {
        public int[] indices;
        public IList<Vector3> vectors;
        public IList<Vector3> normals;
        public IList<Vector2> TextureCoordinates;
        public MOPY[] materials;
    }
}