using Microsoft.Xna.Framework;

namespace MPQNav.ADT
{
	public interface IModelDescriptor
	{
		string FileName { get; }

		Vector3 Position { get; }

		Vector3 Rotation { get; }

		float Scale { get; }
	}
}