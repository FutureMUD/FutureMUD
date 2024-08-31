using System;
using System.Collections.Generic;
using System.Linq;
using Mscc.GenerativeAI;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Body.Grouping;

public abstract class BodypartGroupDescriber : SaveableItem, IBodypartGroupDescriber
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "BodypartGroupDescriber";

	public abstract IBodypartGroupDescriber Clone();
	public abstract void FinaliseLoad(MudSharp.Models.BodypartGroupDescriber describer, IFuturemud gameworld);

	public static IBodypartGroupDescriber LoadDescriber(MudSharp.Models.BodypartGroupDescriber describer,
		IFuturemud game)
	{
		switch (describer.Type)
		{
			case "shape":
				return new BodypartGroupShapeDescriber(describer, game);
			case "bodypart":
				return new BodypartGroupIDDescriber(describer, game);
			default:
				throw new NotImplementedException();
		}
	}

	public static string DescribeGroups<T>(IEnumerable<T> describers, IEnumerable<IBodypart> bodyparts)
		where T : IBodypartGroupDescriber
	{
		// Run the rule for the list of bodyparts and get a score. Higher scores at the start of the list.
		var results =
			describers.Select(x => x.Match(bodyparts))
			          .Where(x => x.IsMatch)
			          .OrderByDescending(x => x.MatchScore)
			          .ToList();

		// If we have no results there are no groups - use bodypart sdescs instead
		if (results.Count == 0)
		{
			return bodyparts.Select(x => x.ShortDescription(colour: false)).ListToString();
		}

		// Add the first match to the results and setup further processing
		var resultStrings = new List<string>();
		var firstResult = results.First();
		var remains = firstResult.Remains;
		resultStrings.Add(firstResult.Description);
		results.Remove(firstResult);

		// Keep adding groups while we have some remainder
		while (remains.Count > 0)
		{
			var currentResult = results.FirstOrDefault(x => x.Matches.All(remains.Contains));
			if (currentResult == null)
			{
				break;
			}

			resultStrings.Add(currentResult.Description);
			remains.RemoveAll(x => currentResult.Matches.Contains(x));
			results.Remove(currentResult);
		}

		resultStrings.AddRange(remains.Select(x => x.ShortDescription(colour: false)));
		return resultStrings.ListToString();
	}

	#region IBodypartGroupDescriber Members

	public string DescribedAs { get; protected set; }

	public string Comment { get; protected set; }
	public IBodyPrototype BodyPrototype { get; protected set; }

	/// <inheritdoc />
	public override string Name => DescribedAs;

	public abstract BodypartGroupResult Match(IEnumerable<IBodypart> parts);

	public abstract string Show(ICharacter actor);

	public string HelpText => $@"You can use the following options with this command:

	#3describedas <text>#0 - sets the substitute text for this group
	#3comment <comment>#0 - sets a comment explaining the intention of this group

{SubtypeHelpText}";

	protected abstract string SubtypeHelpText { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "describedas":
			case "name":
			case "desc":
			case "description":
				return BuildingCommandDescribedAs(actor, ss);
			case "comment":
				return BuildingCommandComment(actor, ss);
			default:
				actor.OutputHandler.Send(SubtypeHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandComment(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the comment do for this group?");
			return false;
		}

		Comment = ss.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send($"The comment for this group is now {Comment.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandDescribedAs(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should the replacement description for this group be?");
			return false;
		}

		DescribedAs = ss.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Instead of individual bodyparts, this group will now be described as {DescribedAs.ColourCommand()}.");
		return true;
	}
	#endregion
}