using Microsoft.Xna.Framework;

namespace MPQNav.Graphics
{
    internal class Model
    {
        private readonly TriangleList triangleList;

        public Model(TriangleList list)
        {
            triangleList = list;
        }

        public TriangleList TriangleList
        {
            get { return triangleList; }
        }

        public Model Transform(Vector3 position, Vector3 rotation, float scale)
        {
            return new Model(triangleList.Transform(CreateOrigin(position),
                                                    CreateTransform(rotation, scale)));
        }

        private static Vector3 CreateOrigin(Vector3 position)
        {
            float posX = -(position.X - 17066.666666666656f);
            float posY = position.Y;
            float posZ = -(position.Z - 17066.666666666656f);
            return new Vector3(posX, posY, posZ);
        }

        private static Matrix CreateTransform(Vector3 rotation, float scale)
        {
            return Matrix.CreateRotationX(MathHelper.ToRadians(rotation.Z))*
                   Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y - 90))*
                   Matrix.CreateRotationZ(MathHelper.ToRadians(-rotation.X))*
                   Matrix.CreateScale(scale);
        }
    }
}