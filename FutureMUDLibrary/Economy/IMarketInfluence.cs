#nullable enable
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
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
	IFutureProg CharacterKnowsAboutInfluenceProg { get; }
	bool CharacterKnowsAboutInfluence(ICharacter character);
	bool Applies(IMarketCategory category, MudDateTime currentDateTime);
	string TextForMarketShow(ICharacter actor);
	XElement SaveImpacts();
}