using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Health {
    public interface ICorpseModel : IFrameworkItem {
        string Description { get; }
        string Describe(DescriptionType type, DecayState state, ICharacter originalCharacter, IPerceiver voyeur, double eatenPercentage);

        string DescribeSevered(DescriptionType type, DecayState state, ICharacter originalCharacter, IPerceiver voyeur,
            ISeveredBodypart part, double eatenPercentage);

        double DecayRate(ITerrain terrain);
        DecayState GetDecayState(double decayPoints);

        ISolid CorpseMaterial(double decayPoints);
        double EdiblePercentage { get; }
    }
}