using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Health.Corpses;

public abstract class CorpseModel : FrameworkItem, ICorpseModel
{
    public override string FrameworkItemType => "CorpseModel";

    #region ICorpseModel Members

    public string Description { get; init; }

    public abstract string Describe(DescriptionType type, DecayState state,
        ICharacter originalCharacter, IBody originalBody, IPerceiver voyeur, double eatenPercentage);

    public abstract string DescribeSevered(DescriptionType type, DecayState state, ICharacter originalCharacter,
        IBody originalBody, IPerceiver voyeur, ISeveredBodypart part, double eatenPercentage);

    public abstract double DecayRate(ITerrain terrain);

    public abstract DecayState GetDecayState(double decayPoints);

    public abstract ISolid CorpseMaterial(double decayPoints);

    public double EdiblePercentage { get; set; }

    public virtual bool CreateCorpse => true;
    public virtual bool RetainItems => true;

    public string EatenShortDescription(double percentage)
    {
        if (percentage <= 0.005)
        {
            return string.Empty;
        }

        if (percentage <= 0.075)
        {
            return "slightly eaten";
        }

        if (percentage <= 0.15)
        {
            return "partially eaten";
        }

        if (percentage <= 0.3)
        {
            return "substantially eaten";
        }

        if (percentage <= 0.6)
        {
            return "largely eaten";
        }

        return "mostly eaten";
    }

    public string EatenDescription(double percentage)
    {
        if (percentage <= 0.005)
        {
            return string.Empty;
        }

        if (percentage <= 0.075)
        {
            return "It has been slightly eaten.";
        }

        if (percentage <= 0.15)
        {
            return "It has been partially eaten.";
        }

        if (percentage <= 0.3)
        {
            return "It has been substantially eaten.";
        }

        if (percentage <= 0.6)
        {
            return "It has been largely eaten.";
        }

        return "It has been mostly eaten.";
    }

    #endregion
}
