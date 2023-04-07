using System.Collections.Generic;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine.Light
{
    public interface ILightModel
    {
        Difficulty GetSightDifficulty(double effectiveIllumination);
        string GetIlluminationDescription(double absoluteIllumination);
        string GetIlluminationRoomDescription(double absoluteIllumination);
        double GetMinimumIlluminationForDescription(string description);
        IEnumerable<string> LightDescriptions { get; }
    }
}