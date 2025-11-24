using System;
using Constant;
using Unity.Services.CloudSave.Internal.Http;

namespace Extensions
{
    static class GameData
    {
        public static object DeserializeData(this GameDataManager manager, string key, IDeserializable value)
        {
            Enum.TryParse(key, out GameDataConstants.DataType result);

            return result switch
            {
                GameDataConstants.DataType.test => value.GetAs<int>(),
                _ => throw new Exception("unknown type in cloud save data detected!")
            };
        }
    }
}