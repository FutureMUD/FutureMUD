
namespace MudSharp.Body.Needs;

public static class NeedsModelFactory
{
    public static INeedsModel LoadNeedsModel(MudSharp.Models.Character dbcharacter, ICharacter character)
    {
        switch (dbcharacter.NeedsModel)
        {
            case "NoNeeds":
                return new NoNeedsModel();
            case "Passive":
                return new PassiveNeedsModel(character);
            case "Active":
                return new ActiveNeedsModel(dbcharacter, character);
            case "ActiveNoThirst":
                return new ActiveNoThirstNeedsModel(dbcharacter, character);
            default:
                throw new NotSupportedException(
                    "NeedsModelFactory.LoadNeedsModel encountered an unknown type of Needs Model.");
        }
    }

    public static INeedsModel LoadNeedsModel(string whichModel, ICharacter character)
    {
        switch (whichModel)
        {
            case "NoNeeds":
                return new NoNeedsModel();
            case "Passive":
                return new PassiveNeedsModel(character);
            case "Active":
                return new ActiveNeedsModel(character);
            case "ActiveNoThirst":
                return new ActiveNoThirstNeedsModel(character);
            default:
                throw new NotSupportedException(
                    "NeedsModelFactory.LoadNeedsModel encountered an unknown type of Needs Model.");
        }
    }

    public static INeedsModel ConvertNeedsModel(string whichModel, ICharacter character, INeedsModel existingNeeds)
    {
        if (existingNeeds.NeedsSave)
        {
            return whichModel switch
            {
                ActiveNeedsModel.ModelNameValue => new ActiveNeedsModel(existingNeeds, character),
                ActiveNoThirstNeedsModel.ModelNameValue => new ActiveNoThirstNeedsModel(existingNeeds, character),
                _ => LoadNeedsModel(whichModel, character)
            };
        }

        return LoadNeedsModel(whichModel, character);
    }
}
