using System.Collections.Generic;

namespace Data;

public class CropConstants
{
    public enum CropGameData
    {
        ID,
        Name,
        Category,
        Description,
        Costs,
        Rewards,
        Appearance,
        Material,
        Icon,
        DetailBg,
        DetailImage
    }
    
    public enum CropStatus
    {
        Null = 0,
        Growing = 1,
        Matured = 2
    }
}