using Constant;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Models
{
    public static class ItemModel
    {
        public struct ItemData
        {
            public int ID; // ID must > 0
            public ItemConstants.Classification Classification;
            public string Name;
            public string Description;
            public int Price;
            public int Amount;
            public Sprite Icon;

            public ItemData(ItemSO baseData, int amount)
            {
                ID = baseData.ID;
                Classification = baseData.Classification;
                Name = baseData.Name;
                Description = baseData.Description;
                Price = baseData.Price;
                Amount = amount;
                Icon = baseData.Icon;
            }

            public void Add(int amount) => Amount += amount;
        }

        public struct ItemUploadData
        {
            public int ID;
            public ItemConstants.ItemCategory Category;
            public int Amount;

            public ItemUploadData(ItemConstants.ItemCategory category, int id, int amount)
            {
                ID = id;
                Category = category;
                Amount = amount;
            }
        }
    }
}
