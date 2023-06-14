#nullable enable
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System.Collections.Generic;
using MudSharp.FutureProg;

namespace MudSharp.Economy;
#nullable enable

public interface IMarketInfluenceTemplate : ISaveable, IEditableItem
{
	IMarket Market { get; }
	string TemplateSummary { get; }
	string Description { get; }
	IEnumerable<MarketImpact> MarketImpacts { get; }
	IFutureProg CharacterKnowsAboutInfluenceProg { get; }
}