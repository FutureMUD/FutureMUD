using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Construction.Boundary;

public class NonCardinalCellExit : CellExit, INonCardinalCellExit
{
	public NonCardinalCellExit(IExit parent, MudSharp.Models.Exit exit, bool firstCellExit)
		: base(parent, exit, firstCellExit)
	{
		InboundDescription = firstCellExit ? exit.InboundDescription2 : exit.InboundDescription1;
		InboundTarget = firstCellExit ? exit.InboundTarget1 : exit.InboundTarget2;
		OutboundDescription = firstCellExit ? exit.OutboundDescription1 : exit.OutboundDescription2;
		OutboundTarget = firstCellExit ? exit.OutboundTarget1 : exit.OutboundTarget2;
		Verb = firstCellExit ? exit.Verb1 : exit.Verb2;
		_keywords =
			new Lazy<List<string>>(() => (firstCellExit ? exit.Keywords1 : exit.Keywords2).Split(' ').ToList());
		PrimaryKeyword = firstCellExit ? exit.PrimaryKeyword1 : exit.PrimaryKeyword2;
	}

	public NonCardinalCellExit(IExit parent, NonCardinalCellExit exit, ICell origin, ICell destination)
		: base(parent, exit, origin, destination)
	{
		InboundDescription = exit.InboundDescription;
		InboundTarget = exit.InboundTarget;
		OutboundDescription = exit.OutboundDescription;
		OutboundTarget = exit.OutboundTarget;
		Verb = exit.Verb;
		_keywords =
			new Lazy<List<string>>(() => exit.Keywords.ToList());
		PrimaryKeyword = exit.PrimaryKeyword;
	}

	public override string ToString()
	{
		return $"NonCardinalCellExit {OutboundMovementSuffix} to {Destination.Name} ({Destination.Id:N0})";
	}

	public string Verb { get; protected set; }
	public string PrimaryKeyword { get; protected set; }

	public string InboundDescription { get; protected set; }
	public string InboundTarget { get; protected set; }

	/// <summary>
	///     e.g. in from the street
	/// </summary>
	public override string InboundMovementSuffix => InboundDescription + " " + InboundTarget;

	public string OutboundDescription { get; protected set; }
	public string OutboundTarget { get; protected set; }

	/// <summary>
	///     e.g. "off towards the Tavern"
	/// </summary>
	public override string OutboundMovementSuffix => OutboundDescription + " " + OutboundTarget;

	public override string OutboundDirectionSuffix => "from " + OutboundTarget;

	public override string InboundDirectionSuffix => "from " + InboundTarget;

	public override string OutboundDirectionDescription => OutboundTarget;

	public override bool IsExit(string verb)
	{
		return verb.Length > 1 && Verb.StartsWith(verb, StringComparison.InvariantCultureIgnoreCase);
	}

	public override bool IsExitKeyword(string keyword)
	{
		return Keywords.Any(x => x.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase));
	}

	public override string DescribeFor(IPerceiver voyeur, bool colour)
	{
		var startColourString = "";
		var endColourString = "";
		if (IsFallExit)
		{
			startColourString = Telnet.Red.ToString();
			endColourString = Telnet.Green.ToString();
		}
		else if (IsClimbExit)
		{
			startColourString = Telnet.Yellow.ToString();
			endColourString = Telnet.Green.ToString();
		}

		return
			$"{startColourString}'{Verb.Proper()} {PrimaryKeyword.Proper()}'{(Exit.Door != null ? $" ({Exit.Door.State.Describe().ToLowerInvariant()} {Exit.Door.InstalledExitDescription(voyeur)})" : "")}{endColourString}";
	}

	public override string BuilderInformationString(IPerceiver voyeur)
	{
		return string.Format("Non-Cardinal Exit #{0:N0} - {1} {2} to {4} at x{3:N} speed", Exit.Id, Verb,
			Keywords.ListToString(separator: "|", conjunction: "", twoItemJoiner: "|").Colour(Telnet.Cyan),
			Exit.TimeMultiplier, Destination.HowSeen(voyeur, colour: false));
	}
}