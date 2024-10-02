using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionEngine;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Foraging;

public class Foragable : EditableItem, IForagable
{
	#region Static Members

	public static string BaseForageTimeExpression
		=> Futuremud.Games.First().GetStaticConfiguration("BaseForageTimeExpression");

	#endregion

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Foragable #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)} - {Status.DescribeColour()}");
		sb.AppendLine();
		sb.Append(new List<string>
		{
			$"Minimum Outcome: {MinimumOutcome.DescribeColour()}",
			$"Maximum Outcome: {MaximumOutcome.DescribeColour()}"
		}.ArrangeStringsOntoLines(2, (uint)actor.LineFormatLength));
		sb.Append(new List<string>
		{
			$"Forage Difficulty: {ForageDifficulty.Describe().Colour(Telnet.Green)}",
			$"Relative Chance: {RelativeChance.ToString("N0", actor).ColourValue()}"
		}.ArrangeStringsOntoLines(2, (uint)actor.LineFormatLength));
		sb.AppendLineFormat("Item Proto: {0}",
			ItemProto != null
				? string.Format(actor, "{0:N0}r{1:N0} - {2}", ItemProto.Id, ItemProto.RevisionNumber,
					ItemProto.ShortDescription)
				: "Not Selected".Colour(Telnet.Red));
		sb.AppendLineFormat("Quantity Expression: {0}", QuantityDiceExpression.Colour(Telnet.Yellow));
		sb.AppendLineFormat("Foragable Types: {0}",
			ForagableTypes.Select(x => x.Colour(Telnet.Green)).ListToString());
		sb.Append(new List<string>
		{
			$"Can Forage Prog: {CanForageProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Yellow)}",
			$"On Forage Prog: {OnForageProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Yellow)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.LineFormatLength));

		return sb.ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbnew = new Models.Foragable
			{
				Id = Id,
				RevisionNumber = FMDB.Context.Foragables.Where(x => x.Id == Id).Select(x => x.RevisionNumber)
									 .AsEnumerable().DefaultIfEmpty(0).Max() +
								 1,
				Name = Name,
				CanForageProgId = CanForageProg?.Id,
				OnForageProgId = OnForageProg?.Id,
				ForagableTypes = ForagableTypes.ListToString(separator: ",", conjunction: "", twoItemJoiner: ","),
				ForageDifficulty = (int)ForageDifficulty,
				ItemProtoId = ItemProto?.Id ?? 0,
				MinimumOutcome = (int)MinimumOutcome,
				MaximumOutcome = (int)MaximumOutcome,
				QuantityDiceExpression = QuantityDiceExpression,
				RelativeChance = RelativeChance,
				EditableItem = new Models.EditableItem()
			};

			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

			FMDB.Context.Foragables.Add(dbnew);
			FMDB.Context.SaveChanges();

			return new Foragable(dbnew, Gameworld);
		}
	}

	public override string EditHeader()
	{
		return $"Foragable {Name} ({Id:N0}r{RevisionNumber:N0})";
	}

	public override bool CanSubmit()
	{
		return ForagableTypes.Any() && ItemProto != null;
	}

	public override string WhyCannotSubmit()
	{
		if (!ForagableTypes.Any())
		{
			return "You must set at least one foragable type.";
		}

		return ItemProto == null ? "You must set a item prototype." : "I don't know why you can't submit.";
	}

	public override string FrameworkItemType => "Foragable";

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Foragables.Find(Id, RevisionNumber);
			if (_statusChanged)
			{
				base.Save(dbitem.EditableItem);
			}

			dbitem.Name = Name;
			dbitem.CanForageProgId = CanForageProg?.Id;
			dbitem.OnForageProgId = OnForageProg?.Id;
			dbitem.ForagableTypes = ForagableTypes.ListToString(separator: ",", conjunction: "", twoItemJoiner: ",");
			dbitem.ForageDifficulty = (int)ForageDifficulty;
			dbitem.ItemProtoId = ItemProto?.Id ?? 0;
			dbitem.MinimumOutcome = (int)MinimumOutcome;
			dbitem.MaximumOutcome = (int)MaximumOutcome;
			dbitem.QuantityDiceExpression = QuantityDiceExpression ?? "1";
			dbitem.RelativeChance = RelativeChance;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#region Constructors

	public Foragable(IAccount originator)
		: base(originator)
	{
		Gameworld = originator.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.Foragable
			{
				Id = Gameworld.Foragables.NextID()
			};
			FMDB.Context.Foragables.Add(dbitem);
			var dbedit = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbedit);
			dbitem.EditableItem = dbedit;
			dbedit.BuilderAccountId = BuilderAccountID;
			dbedit.BuilderDate = BuilderDate;
			dbedit.RevisionStatus = (int)Status;
			dbedit.RevisionNumber = 0;

			_name = "Unnamed Foragable";
			_forageDifficulty = Difficulty.Normal;
			_relativeChance = 100;
			_minimumOutcome = Outcome.MajorFail;
			_maximumOutcome = Outcome.MajorPass;
			_quantityDiceExpression = "1";

			dbitem.Name = _name;
			dbitem.RelativeChance = RelativeChance;
			dbitem.ForageDifficulty = (int)ForageDifficulty;
			dbitem.MinimumOutcome = (int)MinimumOutcome;
			dbitem.MaximumOutcome = (int)MaximumOutcome;
			dbitem.QuantityDiceExpression = QuantityDiceExpression;
			dbitem.ForagableTypes = "";
			FMDB.Context.SaveChanges();
			LoadFromDb(dbitem);
		}
	}

	public Foragable(MudSharp.Models.Foragable foragable, IFuturemud gameworld)
		: base(foragable.EditableItem)
	{
		Gameworld = gameworld;
		LoadFromDb(foragable);
	}

	private void LoadFromDb(MudSharp.Models.Foragable foragable)
	{
		_id = foragable.Id;
		_name = foragable.Name;
		_foragabaleTypes = foragable.ForagableTypes.Split(',').ToList();
		_forageDifficulty = (Difficulty)foragable.ForageDifficulty;
		_relativeChance = foragable.RelativeChance;
		_minimumOutcome = (Outcome)foragable.MinimumOutcome;
		_maximumOutcome = (Outcome)foragable.MaximumOutcome;
		_onForageProg = Gameworld.FutureProgs.Get(foragable.OnForageProgId ?? 0);
		_canForageProg = Gameworld.FutureProgs.Get(foragable.CanForageProgId ?? 0);
		_quantityDiceExpression = foragable.QuantityDiceExpression;
		_itemProtoId = foragable.ItemProtoId;
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.Foragables.GetAll(Id);
	}

	#endregion

	#region Building Commands

	private const string BuildingCommandHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this foragable
	#3proto <which>#0 - sets the proto for this foragable to load
	#3chance <#>#0 - the relative weight of this option being found
	#3quanity <# or dice>#0 - a number or dice expression for the quantity found
	#3difficulty <difficulty>#0 - the difficulty that the result is evaluated against for this item
	#3outcome <min> <max>#0 - the minimum and maximum check outcome that this item can appear on
	#3types <type1> [<type2>] ... [<typen>]#0 - sets the yield types that this foragable appears against
	#3canforage <prog>#0 - sets a prog that controls whether this foragable can be found
	#3canforage clear#0 - clears the can-forage prog
	#3onforage <prog>#0 - sets a prog that will run when this item is foraged
	#3onforage clear#0 - clears the on-forage prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "types":
				return BuildingCommandTypes(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "outcome":
				return BuildingCommandOutcome(actor, command);
			case "chance":
				return BuildingCommandChance(actor, command);
			case "onforage":
				return BuildingCommandOnForage(actor, command);
			case "canforage":
				return BuildingCommandCanForage(actor, command);
			case "proto":
				return BuildingCommandProto(actor, command);
			case "quantity":
				return BuildingCommandQuantity(actor, command);
			default:
				actor.OutputHandler.Send(BuildingCommandHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What dice expression do you want to use for quantity when this item is foraged?");
			return false;
		}

		var diceExpression = command.SafeRemainingArgument;
		var testExpression = new TraitExpression(diceExpression, Gameworld);
		if (testExpression.HasErrors())
		{
			actor.OutputHandler.Send($"Your formula had the following error: {testExpression.Error.ColourCommand()}");
			return false;
		}

		QuantityDiceExpression = diceExpression;
		actor.OutputHandler.Send(
			$"When foraged, this foragable will now yield {QuantityDiceExpression.Colour(Telnet.Yellow)} items.");
		return true;
	}

	private bool BuildingCommandProto(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which item prototype do you want to load when someone forages this foragable?");
			return false;
		}

		var proto = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.Last, true);

		if (proto == null)
		{
			actor.Send("There is no such item prototype for you to use.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.Send("You may only use item prototypes with a status of current.");
			return false;
		}

		if (proto.ReadOnly)
		{
			actor.Send("Read only item prototypes may not be used in foragables.");
			return false;
		}

		ItemProto = proto;
		actor.OutputHandler.Send(
			$"This foragable will now load item prototype {proto.Name} (#{proto.Id}) when foraged.");
		return true;
	}

	private bool BuildingCommandCanForage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You must either specify the prog, or {0} to clear an existing prog.",
				"clear".Colour(Telnet.Yellow));
			return false;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (CanForageProg == null)
			{
				actor.Send("That foragable does not have a CanForage Prog to clear.");
				return false;
			}

			CanForageProg = null;
			actor.Send("You clear the CanForage prog from this foragable. It will now always be foragable.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);

		if (prog == null)
		{
			actor.Send("There is no such prog for you to set as the CanForage prog for this foragable.");
			return false;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.Send("Only progs that return a boolean can be used for the CanForage prog. {0} returns {1}.",
				prog.FunctionName.Colour(Telnet.Yellow), prog.ReturnType.Describe().Colour(Telnet.Cyan));
			return false;
		}

		if (
			!prog.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Number
			}))
		{
			actor.Send(
				"The CanForage prog must accept a single Character parameter and a Number. {0} does not match that pattern.",
				prog.FunctionName);
			return false;
		}

		CanForageProg = prog;
		actor.Send("This foragable will now use the {0} prog to determine who can forage it.", prog.FunctionName);
		return true;
	}

	private bool BuildingCommandOnForage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You must either specify the prog, or {0} to clear an existing prog.",
				"clear".Colour(Telnet.Yellow));
			return false;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (OnForageProg == null)
			{
				actor.Send("That foragable does not have a OnForage Prog to clear.");
				return false;
			}

			OnForageProg = null;
			actor.Send("You clear the OnForage prog from this foragable.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);

		if (prog == null)
		{
			actor.Send("There is no such prog for you to set as the OnForage prog for this foragable.");
			return false;
		}

		if (
			!prog.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Number,
				FutureProgVariableTypes.Item,
				FutureProgVariableTypes.Number
			}))
		{
			actor.Send(
				"The OnForage prog must accept a Character, Number, Item and Number parameter. {0} does not match that pattern.",
				prog.FunctionName);
			return false;
		}

		OnForageProg = prog;
		actor.Send("This foragable will now execute the {0} prog when it is foraged.", prog.FunctionName);
		return true;
	}

	private bool BuildingCommandChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What relative chance do you want to give this foragable to be foraged?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send("You must enter a number for the relative chance.");
			return false;
		}

		if (value < 1)
		{
			actor.Send("You must enter a number greater than zero.");
			return false;
		}

		RelativeChance = value;
		actor.Send("This foragable now has a {0} relative chance to be foraged.",
			RelativeChance.ToString("N0", actor).Colour(Telnet.Green));
		return true;
	}

	private bool BuildingCommandOutcome(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must specify a minimum outcome for individuals to successfully forage this item. Use {0} to make it always foragable.",
				Outcome.MajorFail.DescribeColour());
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<Outcome>(out var minimumOutcome))
		{
			actor.Send("That is not a valid minimum outcome.");
			return false;
		}


		Outcome maximumOutcome;
		if (!command.IsFinished)
		{
			if (!command.PopSpeech().TryParseEnum<Outcome>(out maximumOutcome))
			{
				actor.Send("That is not a valid maximum outcome.");
				return false;
			}
		}
		else
		{
			maximumOutcome = minimumOutcome;
		}

		if (minimumOutcome > maximumOutcome)
		{
			actor.Send("The minimum outcome must be less than or equal to the maximum outcome.");
			return false;
		}

		MinimumOutcome = minimumOutcome;
		MaximumOutcome = maximumOutcome;

		actor.Send(
			"Foragers will now require a minimum outcome of {0} and a maximum outcome of {1} to forage this item.",
			MinimumOutcome.DescribeColour(), MaximumOutcome.DescribeColour());
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must specify a difficulty for individuals to specifically forage for this item. Use {0} if you do not wish it to be specifiable.",
				"impossible".Colour(Telnet.Cyan));
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		ForageDifficulty = difficulty;
		actor.Send("It will now be {0} to specifically forage for this item.",
			difficulty.Describe().Colour(Telnet.Cyan));
		return true;
	}

	private bool BuildingCommandTypes(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You must enter the types that you want to set, separated by spaces.");
			return false;
		}

		command = new StringStack(command.RemainingArgument);
		command.PopSpeechAll();
		var choices = command.Memory.Select(x => x.ToLowerInvariant()).ToList();
		actor.Send("This foragable can now be found using the keywords {0}",
			choices.Select(x => x.Colour(Telnet.Green)).ListToString(conjunction: "or "));
		var existing =
			Gameworld.Foragables.SelectMany(x => x.ForagableTypes)
					 .Select(x => x.ToLowerInvariant())
					 .Where(x => !string.IsNullOrWhiteSpace(x))
					 .Distinct()
					 .ToList();
		var newChoices =
			choices.Where(x => existing.Any(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase))).ToList();
		if (newChoices.Any())
		{
			actor.Send(
				"Warning: Options {0} have not been used before. Check that this is correct.".Colour(Telnet.Yellow),
				newChoices.ListToString());
		}

		_foragabaleTypes.Clear();
		_foragabaleTypes.AddRange(choices);
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You must specify a name to set for this foragabale.");
			return false;
		}

		_name = command.SafeRemainingArgument;
		Changed = true;
		actor.Send("You set the name of this foragable to {0}", Name.Colour(Telnet.Green));
		return true;
	}

	#endregion

	#region IForagable Members

	private List<string> _foragabaleTypes = new();
	public IEnumerable<string> ForagableTypes => _foragabaleTypes;

	private Difficulty _forageDifficulty;

	public Difficulty ForageDifficulty
	{
		get => _forageDifficulty;
		set
		{
			_forageDifficulty = value;
			Changed = true;
		}
	}

	private int _relativeChance;

	public int RelativeChance
	{
		get => _relativeChance;
		set
		{
			_relativeChance = value;
			Changed = true;
		}
	}

	private Outcome _minimumOutcome;

	public Outcome MinimumOutcome
	{
		get => _minimumOutcome;
		set
		{
			_minimumOutcome = value;
			Changed = true;
		}
	}

	private Outcome _maximumOutcome;

	public Outcome MaximumOutcome
	{
		get => _maximumOutcome;
		set
		{
			_maximumOutcome = value;
			Changed = true;
		}
	}

	private string _quantityDiceExpression;

	public string QuantityDiceExpression
	{
		get => _quantityDiceExpression;
		set
		{
			_quantityDiceExpression = value;
			Changed = true;
		}
	}

	private long _itemProtoId;

	public IGameItemProto ItemProto
	{
		get => Gameworld.ItemProtos.Get(_itemProtoId);
		set
		{
			_itemProtoId = value?.Id ?? 0;
			Changed = true;
		}
	}

	private IFutureProg _onForageProg;

	public IFutureProg OnForageProg
	{
		get => _onForageProg;
		set
		{
			_onForageProg = value;
			Changed = true;
		}
	}

	private IFutureProg _canForageProg;

	public IFutureProg CanForageProg
	{
		get => _canForageProg;
		set
		{
			_canForageProg = value;
			Changed = true;
		}
	}

	public bool CanForage(ICharacter character, Outcome outcome)
	{
		return
			(MaximumOutcome == Outcome.None || outcome <= MaximumOutcome) &&
			(MinimumOutcome == Outcome.None || outcome >= MinimumOutcome) &&
			(CanForageProg == null || ((bool?)CanForageProg.Execute(character, Id) ?? false));
	}

	#endregion
}