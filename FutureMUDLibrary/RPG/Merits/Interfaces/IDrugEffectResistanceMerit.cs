using MudSharp.Health;
using System.Collections.Generic;

namespace MudSharp.RPG.Merits.Interfaces;

public interface IDrugEffectResistanceMerit : ICharacterMerit
{
    IReadOnlyDictionary<DrugType,double> DrugResistances { get; }
    double ModifierForDrugType(DrugType drugType);
}