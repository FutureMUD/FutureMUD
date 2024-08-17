using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.NPC.Templates
{
    public interface IHeightWeightModel : IEditableItem
    {
        double BMIMultiplier { get; }
        double MeanBMI { get; }
        double MeanHeight { get; }
        double StandardDeviationBMI { get; }
        double StandardDeviationHeight { get; }
        double? MeanWeight { get; }
        double? StandardDeviationWeight { get; }
        (double,double) GetRandomHeightWeight();
        IHeightWeightModel Clone(string newName);
    }
}