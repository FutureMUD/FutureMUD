using MudSharp.Health;

namespace MudSharp.RPG.Merits.Interfaces;

public interface ISpecificDrugResistanceMerit : ICharacterMerit
{
    double MultiplierForDrug(IDrug drug);
}