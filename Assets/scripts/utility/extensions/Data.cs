using System;
using System.Collections.Generic;
using Constant;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Models.Data.Player;

using Access = Constant.PlayerDataConstants.DataAccessibility;
using PublicData = Constant.PlayerDataConstants.PublicDataType;
using ProtectedData = Constant.PlayerDataConstants.ProtectedDataType;
using PrivateData = Constant.PlayerDataConstants.PrivateDataType;

namespace Extensions
{
    static class PlayerData
    {
        public static object DeserializeData<T>(this PlayerDataManager manager, string key, IDeserializable value) where T : struct, Enum
        {
            Enum.TryParse(key, out T result);

            return result switch
            {
                PublicData.UserID => value.GetAs<Int64>(),
                PublicData.Name => value.GetAs<string>(),
                PublicData.Lv => value.GetAs<byte>(),
                PublicData.Exp => value.GetAs<int>(),
                ProtectedData.Inventory => value.GetAs<string>(),
                ProtectedData.BalanceGold => value.GetAs<int>(),
                ProtectedData.CropData => value.GetAs<string>(),
                ProtectedData.UnlockedCrops => value.GetAs<int>(),
                _ => throw new Exception("unknown type in cloud save data detected!")
            };
        }
        
        public static SaveOptions GetSaveOptions(this PlayerDataManager manager, Access access) => access switch
        {
            Access.Public => new SaveOptions(new PublicWriteAccessClassOptions()),
            Access.Protected => new SaveOptions(new DefaultWriteAccessClassOptions()),
            _ => null
        };

        public static LoadOptions GetLoadOptions(this PlayerDataManager manager, Access access) => access switch
        {
            Access.Public => new LoadOptions(new PublicReadAccessClassOptions()),
            Access.Protected => new LoadOptions(new DefaultReadAccessClassOptions()),
            Access.Private => new LoadOptions(new ProtectedReadAccessClassOptions()),
            _ => null
        };

        public static LoadAllOptions GetLoadAllOptions(this PlayerDataManager manager, Access access) => access switch
        {
            Access.Public => new LoadAllOptions(new PublicReadAccessClassOptions()),
            Access.Protected => new LoadAllOptions(new DefaultReadAccessClassOptions()),
            Access.Private => new LoadAllOptions(new ProtectedReadAccessClassOptions()),
            _ => null
        };
        
        public static async UniTask ValidateBasePlayerData(this PlayerDataManager manager)
        {
            Dictionary<string, object> storedPublicData = PlayerDataManager.Instance.GetData(Access.Public);
            Dictionary<string, object> storedProtectedData = PlayerDataManager.Instance.GetData(Access.Protected);

            foreach (PublicData element in PlayerDataConstants.GetEnumSet<PublicData>())
            {
                if (!storedPublicData.TryGetValue(element.ToString(), out object value))
                {
                    switch (element)
                    {
                        case PublicData.UserID:
                            manager.UpdateData(Access.Public, PublicData.UserID, await CloudCodeService.Instance.CallModuleEndpointAsync<Int64>("Data", "GenerateUserID"));
                            break;
                        case PublicData.Name:
                            manager.UpdateData(Access.Public, PublicData.Name, "");
                            break;
                        case PublicData.Lv:
                            manager.UpdateData<byte, PublicData>(Access.Public, PublicData.Lv, 1);
                            break;
                        case PublicData.Exp:
                            manager.UpdateData(Access.Public, PublicData.Exp, 0);
                            break;
                    }
                }
            }

            foreach (ProtectedData element in PlayerDataConstants.GetEnumSet<ProtectedData>())
            {
                if (!storedProtectedData.TryGetValue(element.ToString(), out object value))
                {
                    switch (element)
                    {
                        case ProtectedData.BalanceGold:
                            manager.UpdateData(Access.Protected, ProtectedData.BalanceGold, 0);
                            break;
                        case ProtectedData.UnlockedCrops:
                            manager.UpdateData(Access.Protected, ProtectedData.UnlockedCrops, 20);
                            break;
                    }
                }
            }

            await manager.SaveAllData();
        }
    }

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