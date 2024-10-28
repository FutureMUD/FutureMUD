using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class PillGameItemComponentProto : GameItemComponentProto
{
	protected PillGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Pill")
	{
		GramsPerPill = 0.5;
		Changed = true;
	}

	protected PillGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IDrug Drug { get; set; }
	public double GramsPerPill { get; set; }
	public IFutureProg OnSwallowProg { get; set; }

	public override string TypeDescription => "Pill";

	protected override void LoadFromXml(XElement root)
	{
		Drug = Gameworld.Drugs.Get(long.Parse(root.Element("Drug").Value));
		GramsPerPill = double.Parse(root.Element("GramsPerPill").Value);
		OnSwallowProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnSwallowProg")?.Value ?? "0"));
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis is a pill item with {4} of the drug {5}. When swallowed, it {6}.",
			"Pill Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.DescribeExact(GramsPerPill, UnitType.Mass, actor).Colour(Telnet.Green),
			Drug?.Name.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show drug {Drug.Id}'") ??
			"No Drug Set".Colour(Telnet.Red),
			OnSwallowProg == null
				? "does not execute any prog"
				: $"executes the {$"{OnSwallowProg.FunctionName} (#{OnSwallowProg.Id})".FluentTagMXP("send", $"href='show futureprog {OnSwallowProg.Id}'")}"
		);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("GramsPerPill", GramsPerPill),
			new XElement("Drug", Drug?.Id ?? 0),
			new XElement("OnSwallowProg", OnSwallowProg?.Id ?? 0)
		).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("pill", true,
			(gameworld, account) => new PillGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Pill", (proto, gameworld) => new PillGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Pill",
			$"Turns the item into a pill that can be {"[swallowed]".Colour(Telnet.Yellow)} to deliver a drug dose",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PillGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PillGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PillGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tdrug <which> - sets the drug contained in this pill\n\tdose <weight> - sets the dose of drug in the pill by weight\n\tprog <which> - sets a prog to run when someone swalllows the pill\n\tprog clear - clears the on-swallow prog";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "grams":
			case "dose":
				return BuildingCommandDose(actor, command);
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "onswallow":
			case "prog":
				return BuildingCommandOnSwallow(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What drug do you want this pill to contain?");
			return false;
		}

		var drug = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Drugs.Get(value)
			: Gameworld.Drugs.GetByName(command.Last);
		if (drug == null)
		{
			actor.Send("There is no such drug.");
			return false;
		}

		if (!drug.DrugVectors.HasFlag(DrugVector.Ingested))
		{
			actor.Send(
				$"You cannot use the drug {drug.Name.Colour(Telnet.Cyan)} because it does not have the 'ingested' delivery vector.");
			return false;
		}

		Drug = drug;
		Changed = true;
		actor.Send($"This pill will now contain the drug {drug.Name.Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandDose(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much drug do you want this pill to deliver?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success) *
		            Gameworld.UnitManager.BaseWeightToKilograms * 1000;
		if (!success)
		{
			actor.Send("You must enter a valid weight of drug to deliver with each dose of this pill.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("The item must have a positive weight of drug.");
			return false;
		}

		GramsPerPill = value;
		Changed = true;
		actor.Send(
			$"You set this pill item to have {Gameworld.UnitManager.DescribeExact(value, UnitType.Mass, actor).Colour(Telnet.Green)} of drug per dose.");
		return true;
	}

	private bool BuildingCommandOnSwallow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"You must either specify a prog to use for the OnSwallow prog, or specify {"clear".Colour(Telnet.Yellow)} to clear it.");
			return false;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			OnSwallowProg = null;
			Changed = true;
			actor.Send("You clear the OnSwallow prog from this pill item.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog to use as an OnSwallow prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
			    { ProgVariableTypes.Character, ProgVariableTypes.Item }))
		{
			actor.Send("The OnSwallow prog must have a single character and item parameter.");
			return false;
		}

		OnSwallowProg = prog;
		Changed = true;
		actor.Send(
			$"This pill item will now execute the {OnSwallowProg.FunctionName} (#{OnSwallowProg.Id}) prog when swallowed.");
		return true;
	}

	#endregion
}