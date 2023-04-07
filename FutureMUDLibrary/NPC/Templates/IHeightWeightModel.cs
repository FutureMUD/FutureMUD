using MudSharp.Framework;

namespace MudSharp.NPC.Templates
{
    public interface IHeightWeightModel : IFrameworkItem
    {
        double BMIMultiplier { get; }
        double MeanBMI { get; }
        double MeanHeight { get; }
        double StandardDeviationBMI { get; }
        double StandardDeviationHeight { get; }
        (double,double) GetRandomHeightWeight();
    }
}