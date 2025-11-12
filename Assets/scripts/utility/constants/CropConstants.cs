using System.Collections.Generic;
using UnityEngine;

namespace Constant
{
    public static class CropConstants
    {
        public enum CropStatus
        {
            Null = 0,
            Growing = 1,
            Matured = 2
        }

        public enum BatchPlantMode
        {
            Standby, // 0 selection
            Initiated, // 1 selection
            Plant, //2 selections + first : empty slot
            Remove, //2 selections + first : growing slot
            Harvest //2 selections + first : harvest slot
        }

        public const string CropAwaitText = "Now Growing ... \n";
        public const string CropMaturedText = "Harvest Now !";

        public const string BatchPlanIconIDText = "Crop ID:\n";
        public const string BatchPlanIconAvailableText = "Ready!";
        public const string BatchPlanIconMaturedText = "Finish!";

        public static Dictionary<BatchPlantMode, string> BatchPlanCancelText { get; } = new()
        {
            { BatchPlantMode.Standby, "Harvest All" },
            { BatchPlantMode.Plant, "Cancel" },
            { BatchPlantMode.Remove, "Cancel" },
            { BatchPlantMode.Harvest, "Cancel" }
        };

        public static Dictionary<BatchPlantMode, string> BatchPlanConfirmText { get; } = new()
        {
            { BatchPlantMode.Standby, "Plant All" },
            { BatchPlantMode.Plant, "Plant" },
            { BatchPlantMode.Remove, "Remove" },
            { BatchPlantMode.Harvest, "Harvest All" }
        };
        
        public static Dictionary<BatchPlantMode, string> BatchPlanInitConfirmText { get; } = new()
        {
            { BatchPlantMode.Plant, "Plant" },
            { BatchPlantMode.Remove, "Detail" },
            { BatchPlantMode.Harvest, "Harvest All" }
        };
    }
}