using System;
using Constant;

using PlanMode = Constant.CropConstants.BatchPlantMode;

namespace Extensions
{
    static class BatchPlan
    {
        public static PlanMode GetPlanMode(this BatchPlantCropUI placeholder, CropConstants.CropStatus status) => status switch
        {
            CropConstants.CropStatus.Null => PlanMode.Plant,
            CropConstants.CropStatus.Growing => PlanMode.Remove,
            CropConstants.CropStatus.Matured => PlanMode.Harvest,
            _ => throw new InvalidOperationException()
        };
    }
}
