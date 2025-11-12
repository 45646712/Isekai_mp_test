using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;
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
                _ => throw new InvalidOperationException()
            };
        }
        
        public static SaveOptions GetSaveOptions(this PlayerDataManager manager, Access access) => access switch
        {
            Access.Public => new SaveOptions(new PublicWriteAccessClassOptions()),
            Access.Protected => new SaveOptions(new DefaultWriteAccessClassOptions()),
            _ => throw new InvalidOperationException()
        };

        public static LoadOptions GetLoadOptions(this PlayerDataManager manager, Access access) => access switch
        {
            Access.Public => new LoadOptions(new PublicReadAccessClassOptions()),
            Access.Protected => new LoadOptions(new DefaultReadAccessClassOptions()),
            Access.Private => new LoadOptions(new ProtectedReadAccessClassOptions()),
            _ => throw new InvalidOperationException()
        };

        public static LoadAllOptions GetLoadAllOptions(this PlayerDataManager manager, Access access) => access switch
        {
            Access.Public => new LoadAllOptions(new PublicReadAccessClassOptions()),
            Access.Protected => new LoadAllOptions(new DefaultReadAccessClassOptions()),
            Access.Private => new LoadAllOptions(new ProtectedReadAccessClassOptions()),
            _ => throw new InvalidOperationException()
        };

        public static async UniTask<bool> UpdatePlayerStatData(this PlayerDataManager manager, Dictionary<ItemConstants.ResourceType,int> rewards, ItemConstants.ItemUpdateOperation ItemOp)
        {
            try
            {
                foreach (var (type, value) in rewards)
                {
                    switch (type, ItemOp)
                    {
                        case (ItemConstants.ResourceType.Exp, ItemConstants.ItemUpdateOperation.Add):
                            manager.UpdateData(Access.Public, PublicData.Exp, manager.GetData<int, PublicData>(Access.Public, PublicData.Exp) + value);
                            break;
                        case (ItemConstants.ResourceType.Currency_Gold, ItemConstants.ItemUpdateOperation.Add):
                            manager.UpdateData(Access.Protected, ProtectedData.BalanceGold, manager.GetData<int, ProtectedData>(Access.Protected, ProtectedData.BalanceGold) + value);
                            break;
                        case (ItemConstants.ResourceType.Currency_Gold, ItemConstants.ItemUpdateOperation.Subtract):
                            int ExistingGold = manager.GetData<int, ProtectedData>(Access.Protected, ProtectedData.BalanceGold);
                            if (ExistingGold < value)
                            {
                                Debug.LogError(LoggerConstants.InsufficientResource_Gold);
                                return false;
                            }

                            manager.UpdateData(Access.Protected, ProtectedData.BalanceGold, ExistingGold - value);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                await manager.LoadAllData();
                return false;
            }

            await manager.SaveAllData();
            return true;
        }

        public static async UniTask<bool> UpdatePlayerStatData(this PlayerDataManager manager, List<Dictionary<ItemConstants.ResourceType,int>> rewards, ItemConstants.ItemUpdateOperation ItemOp)
        {
            try
            {
                foreach (var (type, value) in rewards.SelectMany(x=>x))
                {
                    switch (type, ItemOp)
                    {
                        case (ItemConstants.ResourceType.Exp, ItemConstants.ItemUpdateOperation.Add):
                            manager.UpdateData(Access.Public, PublicData.Exp, manager.GetData<int, PublicData>(Access.Public, PublicData.Exp) + value);
                            break;
                        case (ItemConstants.ResourceType.Currency_Gold, ItemConstants.ItemUpdateOperation.Add):
                            manager.UpdateData(Access.Protected, ProtectedData.BalanceGold, manager.GetData<int, ProtectedData>(Access.Protected, ProtectedData.BalanceGold) + value);
                            break;
                        case (ItemConstants.ResourceType.Currency_Gold, ItemConstants.ItemUpdateOperation.Subtract):
                            int ExistingGold = manager.GetData<int, ProtectedData>(Access.Protected, ProtectedData.BalanceGold);
                            if (ExistingGold < value)
                            {
                                Debug.LogError(LoggerConstants.InsufficientResource_Gold);
                                return false;
                            }

                            manager.UpdateData(Access.Protected, ProtectedData.BalanceGold, ExistingGold - value);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                await manager.LoadAllData();
                return false;
            }

            await manager.SaveAllData();
            return true;
        }
        
        public static async UniTask ValidateBasePlayerData(this PlayerDataManager manager)
        {
            foreach (PublicData element in PlayerDataConstants.GetEnumSet<PublicData>())
            {
                if (!PlayerDataManager.Instance.GetData(Access.Public).TryGetValue(element.ToString(), out object value))
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
                if (!PlayerDataManager.Instance.GetData(Access.Protected).TryGetValue(element.ToString(), out object value))
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