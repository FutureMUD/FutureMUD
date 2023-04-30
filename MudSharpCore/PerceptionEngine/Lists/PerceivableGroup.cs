using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.GameItems;

namespace MudSharp.PerceptionEngine.Lists;

/// <summary>
/// A group of perceivables that are somehow related and should be grouped together in an emote. Warning, do not use this class outside the context of emotes.
/// </summary>
public class PerceivableGroup : TemporaryPerceivable
{
	public List<IPerceivable> Members { get; } = new();

	public PerceivableGroup(IEnumerable<IPerceivable> members)
	{
		Members.AddRange(members);
	}

	public override bool IsSelf(IPerceivable other)
	{
		return Members.Any(x => x.IsSelf(other));
	}

	public override bool Sentient => Members.Any(x => x.Sentient);

	public override double IlluminationProvided => Members.Sum(x => x.IlluminationProvided);

        public SizeCategory Size => Members.Max(x => x.Size).Stage(1);

        public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short, bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
        {
            switch (type)
            {
                case DescriptionType.Short:
                case DescriptionType.Possessive:
                    break;
                default:
                    throw new ApplicationException("PerceivableGroups should not be used for any types of descriptions other than Short or Possessive.");
            }

		var seenMembers = Members.Where(x => voyeur.CanSee(x, flags)).ToList();
		if (!seenMembers.Any())
		{
			return "something";
		}

		return seenMembers.Select(x => x.HowSeen(voyeur, proper, type, colour, flags)).ListToCompactString();
	}

	public override IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return Members.SelectMany(x => x.GetKeywordsFor(voyeur)).Distinct();
	}

	public override bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false)
	{
		return GetKeywordsFor(voyeur).Any(x =>
			abbreviated
				? x.StartsWith(targetKeyword, StringComparison.InvariantCultureIgnoreCase)
				: x.EqualTo(targetKeyword));
	}

	public override bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false)
	{
		return GetKeywordsFor(voyeur).Any(x =>
			abbreviated
				? targetKeywords.Any(y => x.StartsWith(y, StringComparison.InvariantCultureIgnoreCase))
				: targetKeywords.Any(y => x.EqualTo(y)));
	}

	public override bool HiddenFromPerception(IPerceiver voyeur, PerceptionTypes type,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return Members.All(x => x.HiddenFromPerception(voyeur, type, flags));
	}

	public override PerceptionTypes GetPerception(PerceptionTypes type)
	{
		return Members.Select(x => x.GetPerception(type)).Aggregate((x, y) => x | y);
	}

	public override bool HiddenFromPerception(PerceptionTypes type,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return Members.All(x => x.HiddenFromPerception(type, flags));
	}

	public override ICell Location => Members.First().Location;

	/// <inheritdoc />
	public override IEnumerable<(IPerceivable Thing, Proximity Proximity)> LocalThingsAndProximities()
	{
		var things = Members.SelectMany(x => x.LocalThingsAndProximities()).ToLookup(x => x.Thing);
		foreach (var thing in things)
		{
			yield return (thing.Key, thing.Min(x => x.Proximity));
		}
	}

	#region Overrides of TemporaryPerceivable

	/// <summary>True if this perceivable is a single entity as opposed to a group of entities</summary>
	public override bool IsSingleEntity => false;

	#endregion

	public override string FrameworkItemType => "PerceivableGroup";
}