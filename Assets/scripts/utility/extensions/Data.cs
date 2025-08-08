using System;
using System.Collections.Generic;
using Constant;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave.Internal.Http;

namespace Extensions
{
    static class PlayerData
    {
        public static object DeserializeData(this PlayerDataManager manager, string key, IDeserializable value)
        {
            Enum.TryParse(key, out PlayerDataConstant.PublicDataType result);

            switch (result)
            {
                case PlayerDataConstant.PublicDataType.UserID:
                    return value.GetAs<Int64>();
                case PlayerDataConstant.PublicDataType.Lv:
                    return value.GetAs<byte>();
                case PlayerDataConstant.PublicDataType.Name:
                    return value.GetAs<string>();
                default:
                    throw new Exception("unknown type in cloud save data detected!");
            }
        }

        public static async UniTask GenerateUserID(this PlayerDataManager manager)
        {
            Int64 userID = await CloudCodeService.Instance.CallModuleEndpointAsync<Int64>("Data", "GenerateUserID");
            
            await GenerateBasePlayerData(manager);
            
            manager.SetPublicData(PlayerDataConstant.PublicDataType.UserID, userID);
        }

        private static async UniTask GenerateBasePlayerData(this PlayerDataManager manager)
        {
            manager.SetPublicData<byte>(PlayerDataConstant.PublicDataType.Lv, 1);
            manager.SetPublicData(PlayerDataConstant.PublicDataType.Name, "");
            
            await manager.SaveAllData();
        }
    }
}