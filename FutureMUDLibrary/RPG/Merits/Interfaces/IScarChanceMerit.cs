#nullable enable

using System.Collections.Generic;
using MudSharp.Health;

namespace MudSharp.RPG.Merits.Interfaces;

public interface IScarChanceMerit : ICharacterMerit
{
	bool AppliesToWounds { get; }
	bool AppliesToSurgery { get; }
	IEnumerable<DamageType> DamageTypes { get; }
	double FlatModifier { get; }
	double Multiplier { get; }
	bool AppliesTo(IWound wound);
}
