using System.Collections.Generic;

namespace Data;

public static class CropConstants
{
    public enum CropStatus
    {
        Locked = -1,
        Null,
        Growing, 
        Matured, 
    }
}