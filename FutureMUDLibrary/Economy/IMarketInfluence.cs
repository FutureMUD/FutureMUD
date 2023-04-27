#nullable enable
using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy;
#nullable enable

public interface IMarketInfluence : ISaveable, IEditableItem
{
	IMarket Market { get; }
	IMarketInfluenceTemplate? MarketInfluenceTemplate { get; }
	string Description { get; }
	MudDateTime AppliesFrom { get; }
	MudDateTime? AppliesUntil { get; }
	IEnumerable<MarketImpact> MarketImpacts { get; }
	bool CharacterKnowsAboutInfluence(ICharacter character);
}