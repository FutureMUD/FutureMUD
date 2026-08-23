#nullable enable

namespace MudSharp.Body.Needs;

/// <summary>
///     An active needs model for fully aquatic creatures. It retains hunger and alcohol mechanics
///     while permanently satisfying thirst and hydration.
/// </summary>
public class ActiveNoThirstNeedsModel : ActiveNeedsModel
{
    public new const string ModelNameValue = "ActiveNoThirst";

    protected override bool TracksThirst => false;

    public override string ModelName => ModelNameValue;

    public ActiveNoThirstNeedsModel(MudSharp.Models.Character dbcharacter, ICharacter character)
        : base(dbcharacter, character)
    {
        NormaliseValues();
    }

    public ActiveNoThirstNeedsModel(ICharacter character)
        : base(character)
    {
        NormaliseValues();
    }

    public ActiveNoThirstNeedsModel(INeedsModel existingNeeds, ICharacter character)
        : base(existingNeeds, character)
    {
        NormaliseValues();
    }
}
