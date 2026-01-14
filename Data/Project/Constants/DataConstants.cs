using System;
using System.Collections.Generic;
using System.Linq;

namespace Data;

public static class DataConstants
{
    public enum DataAccessibility
    {
        Public,
        Protected,
        Private
    }

    public enum PublicPlayerKeys
    {
        UserID, //int64
        Name,//string
        Lv, //byte
        Exp, //int
    }
    
    public enum ProtectedPlayerKeys
    {
        Inventory, //string
        Currency_Gold, //int
        CropData, //string
        UnlockedCropSlots,
        UnlockedFishSlots
    }
    
    public enum PrivatePlayerKeys
    {
        NextCropUpdateTime //DToffset
    }
    
    public enum DataOperations
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Update
    }

    public enum GameDataType // for building game data ID
    {
        Crop,
        Fish,
        Building,
        ItemCrop,
        ItemFish,
        ItemBuilding,
        NPC
    }
}