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
	/// <summary>
	/// A summary of the template itself and what it's about
	/// </summary>
	string TemplateSummary { get; }

	/// <summary>
	/// The description parameter passed to an instance of IMarketInfluence
	/// </summary>
	string Description { get; }
	IEnumerable<MarketImpact> MarketImpacts { get; }
	IFutureProg CharacterKnowsAboutInfluenceProg { get; }
	IMarketInfluenceTemplate Clone(string newName);
}