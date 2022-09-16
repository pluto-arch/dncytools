using System;

namespace Dncy.Tools.Core
{
    public interface IObjectSerializeProvider
    {
        string Serialize(object obj);

        T Deserialize<T>(string objStr);

        object Deserialize(string objStr, Type type);
    }
}
