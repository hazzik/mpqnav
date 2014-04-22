using System.IO;

namespace MPQNav
{
    public interface IParser<out T>
    {
        T Parse(BinaryReader reader);
    }
}