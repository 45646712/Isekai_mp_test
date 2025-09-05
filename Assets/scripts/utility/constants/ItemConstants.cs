using UnityEngine;

namespace Constant
{
    public static class ItemConstants
    {
        public enum ItemCategory
        {
            Crop, //ID 1~
            Fish, //ID 5001~
            Building //ID 10001~
        }

        public enum Classification
        {
            Null = -1,
            Materials,
            Resources,
            Valuables,
            QuestItems,
            Exotics,
            Others
        }
    }
}