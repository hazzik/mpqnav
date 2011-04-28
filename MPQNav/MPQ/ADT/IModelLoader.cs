using MPQNav.Chunks;
using MPQNav.Graphics;

namespace MPQNav.ADT
{
	internal interface IModelLoader
	{
		Model Load(IModelDescriptor modf);
	}
}