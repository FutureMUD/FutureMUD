using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class DisfigurementPickerScreenStoryboard : ChargenScreenStoryboard
{
	private DisfigurementPickerScreenStoryboard()
	{
	}

	public DisfigurementPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem) : base(
		dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);

		SkipIfSimpleApplication = bool.Parse(definition.Element("SkipIfSimpleApplication")?.Value ?? "true");

		ScarBlurb = definition.Element("ScarBlurb")?.Value;
		AllowPickingScars = bool.Parse(definition.Element("AllowPickingScars")?.Value ?? "false");
		if (AllowPickingScars && string.IsNullOrWhiteSpace(ScarBlurb))
		{
			throw new ApplicationException(
				"DisfigurementPicker in ChargenStoryboard permitted the selection of scars but did not define a ScarBlurb.");
		}

		TattooBlurb = definition.Element("TattooBlurb")?.Value;
		AllowPickingTattoos = bool.Parse(definition.Element("AllowPickingTattoos")?.Value ?? "false");
		if (AllowPickingTattoos && string.IsNullOrWhiteSpace(TattooBlurb))
		{
			throw new ApplicationException(
				"DisfigurementPicker in ChargenStoryboard permitted the selection of tattoos but did not define a TattooBlurb.");
		}

		BodypartsBlurb = definition.Element("BodypartsBlurb")?.Value;
		AllowPickingMissingBodyparts = bool.Parse(definition.Element("AllowPickingMissingBodyparts")?.Value ?? "false");
		if (AllowPickingMissingBodyparts && string.IsNullOrWhiteSpace(BodypartsBlurb))
		{
			throw new ApplicationException(
				"DisfigurementPicker in ChargenStoryboard permitted the selection of missing bodyparts but did not define a BodypartsBlurb.");
		}

		ProstheticsBlurb = definition.Element("ProstheticsBlurb")?.Value;
		PickableProstheses = new List<ChargenProsthesisSelection>();
		foreach (var prosthesis in definition.Element("Prostheses")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			var item = Gameworld.ItemProtos.Get(long.Parse(prosthesis.Element("Item")?.Value ??
			                                               throw new ApplicationException(
				                                               "Prostheses in ChargenStoryboard did not have an Item element.")));
			if (!item.IsItemType<ProstheticGameItemComponentProto>())
			{
				throw new ApplicationException(
					$"Item {item.Id}r{item.RevisionNumber} was specified as a prosthesis in ChargenStoryboard but it is not actually a prosthetic item.");
			}

			var costs = new Counter<IChargenResource>();
			foreach (var cost in prosthesis.Element("Costs")?.Elements() ?? Enumerable.Empty<XElement>())
			{
				var resource =
					long.TryParse(
						cost.Attribute("resource")?.Value ?? throw new ApplicationException(
							$"Chargen costs was lacking a resource attribute in ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber}"),
						out var value)
						? Gameworld.ChargenResources.Get(value)
						: Gameworld.ChargenResources.GetByName(cost.Attribute("resource").Value) ??
						  Gameworld.ChargenResources.FirstOrDefault(x =>
							  x.Alias.EqualTo(cost.Attribute("resource").Value));
				if (resource == null)
				{
					throw new ApplicationException(
						$"Chargen costs in ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber} specified an invalid chargen resource.");
				}

				if (!int.TryParse(
					    cost.Attribute("amount")?.Value ?? throw new ApplicationException(
						    $"Chargen costs in ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber} did not specify an amount attribute."),
					    out var amount))
				{
					throw new ApplicationException(
						$"Chargen costs in ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber} specified an invalid amount.");
				}

				costs[resource] += amount;
			}

			var element = prosthesis.Element("CanSelectProg");
			if (element == null)
			{
				throw new ApplicationException(
					$"ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber} did not specify a CanSelectProg element.");
			}

			var prog = long.TryParse(element.Value, out var progId)
				? Gameworld.FutureProgs.Get(progId)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (prog == null)
			{
				throw new ApplicationException(
					$"ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber} specified an invalid CanSelectProg.");
			}

			if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber} specified a CanSelectProg that did not return boolean.");
			}

			if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Chargen }))
			{
				throw new ApplicationException(
					$"ChargenStoryboard for ProstheticItem {item.Id}r{item.RevisionNumber} specified a CanSelectProg that did not take a single chargen parameter.");
			}

			PickableProstheses.Add(new ChargenProsthesisSelection
			{
				Item = item,
				ResourceCosts = costs,
				CanSelectProg = prog,
				ProstheticProto = item.GetItemType<ProstheticGameItemComponentProto>()
			});
		}

		if (PickableProstheses.Any() && string.IsNullOrWhiteSpace(ProstheticsBlurb))
		{
			throw new ApplicationException(
				"DisfigurementPicker in ChargenStoryboard permitted the selection of prosthetics but did not define a ProstheticsBlurb.");
		}
	}

	protected override string StoryboardName => "DisfigurementPicker";

	public override ChargenStage Stage => ChargenStage.SelectDisfigurements;

	public string ScarBlurb { get; private set; }
	public string TattooBlurb { get; private set; }
	public string BodypartsBlurb { get; private set; }
	public string ProstheticsBlurb { get; private set; }

	public bool AllowPickingTattoos { get; private set; }
	public bool AllowPickingScars { get; private set; }
	public bool AllowPickingMissingBodyparts { get; private set; }

	public bool SkipIfSimpleApplication { get; private set; }

	internal List<ChargenProsthesisSelection> PickableProstheses { get; }

	public override string HelpText => $@"{BaseHelpText}
	#3allowtattoos#0 - toggles tattoos being selectable
	#3allowscars#0 - toggles scars being selectable
	#3allowmissing#0 - toggles missing bodyparts and prosthetics being selectable
	#3skipsimple#0 - skips this screen if ""simple"" mode is selected
	#3tattooblurb#0 - drops into an editor to edit the tattoo blurb
	#3scarblurb#0 - drops into an editor to edit the scar blurb
	#3missingblurb#0 - drops into an editor to edit the missing bodyparts blurb
	#3prostheticblurb#0 - drops into an editor to edit the prosthetics blurb
	#3prosthetic add <item> <prog> <costs>#0 - adds a selectable prosthetic item
	#3prosthetic remove <item>#0 - removes an item as a selectable prosthetic
	#3prosthetic cost <item> <new costs>#0 - changes the cost of a selectable prosthetic
	#3prosthetic prog <item> <new prog>#0 - changes the prog of a selectable prosthetic";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "allowscars":
				return BuildingCommandAllowScars(actor);
			case "allowtattoos":
				return BuildingCommandAllowTattoos(actor);
			case "allowmissing":
				return BuildingCommandAllowMissing(actor);
			case "scarblurb":
				return BuildingCommandScarBlurb(actor);
			case "tattooblurb":
				return BuildingCommandTattooBlurb(actor);
			case "missingblurb":
				return BuildingCommandMissingBlurb(actor);
			case "prostheticblurb":
			case "prostheticsblurb":
			case "prosthesisblurb":
			case "prosthesesblurb":
				return BuildingCommandProstheticBlurb(actor);
			case "prosthetic":
				return BuildingCommandProsthetic(actor, command);
			case "skipsimple":
				return BuildingCommandSkipSimple(actor);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandSkipSimple(ICharacter actor)
	{
		SkipIfSimpleApplication = !SkipIfSimpleApplication;
		Changed = true;
		actor.OutputHandler.Send($"This screen will {SkipIfSimpleApplication.NowNoLonger()} be skipped if this is a simple application.");
		return true;
	}

	private bool BuildingCommandProsthetic(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandProstheticAdd(actor, command);
			case "remove":
			case "rem":
				return BuildingCommandProstheticRemove(actor, command);
			case "cost":
			case "costs":
				return BuildingCommandProstheticCosts(actor, command);
			case "prog":
				return BuildingCommandProstheticProg(actor, command);
			default:
				return false;
		}
	}

	private bool BuildingCommandProstheticProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prosthetic item do you want to change the selectable prog for?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid ID number.");
			return false;
		}

		if (PickableProstheses.All(x => x.Item.Id != value))
		{
			actor.OutputHandler.Send("There is no such existing pickable prosthetic item.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether this prosthetic can be selected?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		var prosthetic = PickableProstheses.First(x => x.Item.Id == value);
		prosthetic.CanSelectProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prosthetic item {prosthetic.Item.Id.ToString("N0", actor)}r{prosthetic.Item.RevisionNumber.ToString("N0", actor)} ({prosthetic.Item.ShortDescription.ColourObject()}) now uses the prog {prog.MXPClickableFunctionName()} to control whether it can be selected.");
		return true;
	}

	private bool BuildingCommandProstheticCosts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prosthetic item do you want to change the cost of?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid ID number.");
			return false;
		}

		if (PickableProstheses.All(x => x.Item.Id != value))
		{
			actor.OutputHandler.Send("There is no such existing pickable prosthetic item.");
			return false;
		}

		var prosthetic = PickableProstheses.First(x => x.Item.Id == value);

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either enter a number of costs in the format <amount> <resource> (multiple costs can follow each other), or use {"none".ColourCommand()} to set no costs.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			prosthetic.ResourceCosts.Clear();
			Changed = true;
			actor.OutputHandler.Send(
				$"The prosthetic item {prosthetic.Item.Id.ToString("N0", actor)}r{prosthetic.Item.RevisionNumber.ToString("N0", actor)} ({prosthetic.Item.ShortDescription.ColourObject()}) no longer has any resource cost to select.");
			return true;
		}

		var costs = new Counter<IChargenResource>();
		while (!command.IsFinished)
		{
			if (!int.TryParse(command.PopSpeech(), out var amount))
			{
				actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
				return false;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					$"Which resource should this prosthesis cost {amount.ToString("N0", actor).ColourValue()} of?");
				;
				return false;
			}

			var resource = Gameworld.ChargenResources.GetByIdOrName(command.PopSpeech());
			if (resource is null)
			{
				actor.OutputHandler.Send("There is no such account resource.");
				return false;
			}

			costs[resource] += amount;
		}

		prosthetic.ResourceCosts.Clear();
		foreach (var cost in costs)
		{
			prosthetic.ResourceCosts[cost.Key] = cost.Value;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The prosthetic item {prosthetic.Item.Id.ToString("N0", actor)}r{prosthetic.Item.RevisionNumber.ToString("N0", actor)} ({prosthetic.Item.ShortDescription.ColourObject()}) now costs {costs.Select(x => $"{x.Value.ToString("N0", actor)} {(x.Value == 1 ? x.Key.Name : x.Key.PluralName)}".ColourValue()).DefaultIfEmpty("None".ColourValue()).ListToString()}.");
		return true;
	}

	private bool BuildingCommandProstheticRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prosthetic item do you want to remove?");
			return false;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid ID number.");
			return false;
		}

		if (PickableProstheses.All(x => x.Item.Id != value))
		{
			actor.OutputHandler.Send("There is no such existing pickable prosthetic item.");
			return false;
		}

		var prosthetic = PickableProstheses.First(x => x.Item.Id == value);

		actor.OutputHandler.Send(
			$"Are you sure that you want to permanently remove the prosthetic item {prosthetic.Item.Id.ToString("N0", actor)}r{prosthetic.Item.RevisionNumber.ToString("N0", actor)} ({prosthetic.Item.ShortDescription.ColourObject()}) as a selectable item?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Removing a prosthetic item",
			AcceptAction = text =>
			{
				actor.OutputHandler.Send(
					$"You permanently remove the prosthetic item {prosthetic.Item.Id.ToString("N0", actor)}r{prosthetic.Item.RevisionNumber.ToString("N0", actor)} ({prosthetic.Item.ShortDescription.ColourObject()}) as a selectable item.");
				PickableProstheses.Remove(prosthetic);
				Changed = true;
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to remove the selectable prosthetic."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to remove the selectable prosthetic."); }
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandProstheticAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item do you want to add a selectable prosthetic?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid ID number.");
			return false;
		}

		var proto = Gameworld.ItemProtos.Get(value);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				$"{proto.EditHeader().ColourObject()} is in status {proto.Status.DescribeColour()}, and is thus unable to be used.");
			return false;
		}

		var prostheticProto = proto.GetItemType<ProstheticGameItemComponentProto>();
		if (prostheticProto is null)
		{
			actor.OutputHandler.Send($"{proto.EditHeader().ColourObject()} is not a prosthetic item.");
			return false;
		}

		if (PickableProstheses.Any(x => x.Item.Id == proto.Id))
		{
			actor.OutputHandler.Send(
				$"{proto.EditHeader().ColourObject()} is already a pickable prosthetic. You should edit it instead.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should determine whether this prosthetic can be selected?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.PopSpeech(),
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		var costs = new Counter<IChargenResource>();
		while (!command.IsFinished)
		{
			if (!int.TryParse(command.PopSpeech(), out var amount))
			{
				actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
				return false;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					$"Which resource should this prosthesis cost {amount.ToString("N0", actor).ColourValue()} of?");
				;
				return false;
			}

			var resource = Gameworld.ChargenResources.GetByIdOrName(command.PopSpeech());
			if (resource is null)
			{
				actor.OutputHandler.Send("There is no such account resource.");
				return false;
			}

			costs[resource] += amount;
		}

		PickableProstheses.Add(new ChargenProsthesisSelection
		{
			CanSelectProg = prog,
			Item = proto,
			ProstheticProto = prostheticProto,
			ResourceCosts = costs
		});
		Changed = true;
		actor.OutputHandler.Send(
			$"{proto.EditHeader().ColourObject()} is now pickable as a prosthetic with a prog of {prog.MXPClickableFunctionName()} and costing {costs.Select(x => $"{x.Value.ToString("N0", actor)} {(x.Value == 1 ? x.Key.Name : x.Key.PluralName)}".ColourValue()).DefaultIfEmpty("nothing".ColourValue()).ListToString()}.");
		return true;
	}

	private bool BuildingCommandProstheticBlurb(ICharacter actor)
	{
		actor.OutputHandler.Send(
			$"You are replacing the following existing text:\n\n{ProstheticsBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\nEnter the new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, ProstheticsBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, "prosthetics" });
		return true;
	}

	private bool BuildingCommandMissingBlurb(ICharacter actor)
	{
		actor.OutputHandler.Send(
			$"You are replacing the following existing text:\n\n{BodypartsBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\nEnter the new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, BodypartsBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, "missing bodypart" });
		return true;
	}

	private bool BuildingCommandTattooBlurb(ICharacter actor)
	{
		actor.OutputHandler.Send(
			$"You are replacing the following existing text:\n\n{TattooBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\nEnter the new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, TattooBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, "tattoo" });
		return true;
	}

	private bool BuildingCommandScarBlurb(ICharacter actor)
	{
		actor.OutputHandler.Send(
			$"You are replacing the following existing text:\n\n{ScarBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\nEnter the new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, ScarBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, "scar" });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		var which = (string)args[1];
		handler.Send($"You decide not to change the {which} blurb for this chargen screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		var which = (string)args[1];
		switch (which)
		{
			case "tattoo":
				TattooBlurb = text;
				break;
			case "scar":
				ScarBlurb = text;
				break;
			case "missing bodypart":
				BodypartsBlurb = text;
				break;
			case "prosthetics":
				ProstheticsBlurb = text;
				break;
		}

		Changed = true;
		handler.Send(
			$"You set the {which} blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandAllowMissing(ICharacter actor)
	{
		AllowPickingMissingBodyparts = !AllowPickingMissingBodyparts;
		Changed = true;
		actor.OutputHandler.Send(AllowPickingMissingBodyparts
			? "Players are now allowed to pick missing bodyparts and prostheses for their characters."
			: "Players are no longer allowed to pick missing bodyparts and prostheses for their characters.");
		return true;
	}

	private bool BuildingCommandAllowTattoos(ICharacter actor)
	{
		AllowPickingTattoos = !AllowPickingTattoos;
		Changed = true;
		actor.OutputHandler.Send(AllowPickingTattoos
			? "Players are now allowed to pick tattoos for their characters."
			: "Players are no longer allowed to pick tattoos for their characters.");
		return true;
	}

	private bool BuildingCommandAllowScars(ICharacter actor)
	{
		AllowPickingScars = !AllowPickingScars;
		Changed = true;
		actor.OutputHandler.Send(AllowPickingScars
			? "Players are now allowed to pick pre-existing scars for their characters."
			: "Players are no longer allowed to pick pre-existing scars for their characters.");
		return true;
	}

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ScarBlurb", new XCData(ScarBlurb)),
			new XElement("TattooBlurb", new XCData(TattooBlurb)),
			new XElement("BodypartsBlurb", new XCData(BodypartsBlurb)),
			new XElement("ProstheticsBlurb", new XCData(ProstheticsBlurb)),
			new XElement("AllowPickingScars", AllowPickingScars),
			new XElement("AllowPickingTattoos", AllowPickingTattoos),
			new XElement("AllowPickingMissingBodyparts", AllowPickingMissingBodyparts),
			new XElement("Prostheses",
				from item in PickableProstheses
				select new XElement("Prosthesis",
					new XElement("Item", item.Item?.Id ?? 0),
					new XElement("Costs",
						from cost in item.ResourceCosts
						select new XElement("Cost", new XAttribute("resource", cost.Key.Id),
							new XAttribute("amount", cost.Value))
					),
					new XElement("CanSelectProg", item.CanSelectProg?.Id ?? 0)
				)
			)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectDisfigurements,
			new ChargenScreenStoryboardFactory("DisfigurementPicker",
				(game, dbitem) => new DisfigurementPickerScreenStoryboard(game, dbitem)),
			"DisfigurementPicker",
			"Pick tattoos, scars, missing bodyparts and prosthetics",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new DisfigurementPickerScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine("This screen allows people to select tattoos, scars, missing bodyparts and prosthetics."
		              .Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Skip if Simple Application: {SkipIfSimpleApplication.ToColouredString()}");
		sb.AppendLine($"Allow Picking Tattoos: {AllowPickingTattoos.ToColouredString()}");
		sb.AppendLine($"Allow Picking Scars: {AllowPickingScars.ToColouredString()}");
		sb.AppendLine($"Allow Picking Severs/Prosthetics: {AllowPickingMissingBodyparts.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Tattoo Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(TattooBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Scar Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(ScarBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Bodyparts Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(BodypartsBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Prosthetics Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(ProstheticsBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Pickable Prosthetics".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in PickableProstheses
			select new List<string>
			{
				item.Item.Id.ToString("N0", voyeur),
				item.Item.ShortDescription.ColourObject(),
				item.CanSelectProg.MXPClickableFunctionName(),
				item.ResourceCosts
				    .Select(x =>
					    $"{x.Value.ToString("N0", voyeur)} {(x.Value == 1 ? x.Key.Name : x.Key.PluralName)}"
						    .ColourValue()).DefaultIfEmpty("None".ColourValue()).ListToString()
			},
			new List<string>
			{
				"Item ID",
				"Item SDesc",
				"Can Select Prog",
				"Resource Costs"
			},
			voyeur
		));
		return sb.ToString();
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		var resources = new Counter<IChargenResource>();
		foreach (var prosthesis in chargen.SelectedProstheses)
		foreach (var resource in PickableProstheses.First(x => x.Item == prosthesis).ResourceCosts)
		{
			resources[resource.Key] += resource.Value;
		}

		foreach (var disfigurement in chargen.SelectedDisfigurements)
		foreach (var resource in disfigurement.Disfigurement.ChargenCosts)
		{
			resources[resource.Key] += resource.Value;
		}

		return resources.Select(x => (x.Key, x.Value));
	}

	internal class DisfigurementPickerScreen : ChargenScreen
	{
		private readonly List<ChargenProsthesisSelection> _chargenProsthesisSelections = new();
		private readonly List<string> _filteredKeywords = new();
		private readonly List<IBodypart> _missingBodyparts = new();
		private readonly List<IGameItemProto> _selectedProstheses = new();
		private readonly List<(IScarTemplate Scar, IBodypart Bodypart)> _selectedScars = new();
		private readonly List<(ITattooTemplate Tattoo, IBodypart Bodypart)> _selectedTattoos = new();
		private List<IBodypart> _effectiveBodyparts = new();
		private List<IScarTemplate> _filteredScars = new();
		private List<ITattooTemplate> _filteredTattoos = new();
		private List<IScarTemplate> _scarTemplates = new();
		private IScarTemplate _selectedScar;
		private ITattooTemplate _selectedTattoo;

		private List<ITattooTemplate> _tattooTemplates = new();

		internal DisfigurementPickerScreen(IChargen chargen, DisfigurementPickerScreenStoryboard storyboard) : base(
			chargen, storyboard)
		{
			Storyboard = storyboard;
			chargen.SelectedDisfigurements = new List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)>();
			chargen.SelectedProstheses = new List<IGameItemProto>();
			chargen.MissingBodyparts = new List<IBodypart>();
			PickerStage = DisfigurementPickerStage.MissingBodyparts;
			CheckCurrentStage();

			if (Storyboard.SkipIfSimpleApplication && chargen.ApplicationType == ApplicationType.Simple)
			{
				State = ChargenScreenState.Complete;
			}
		}

		public DisfigurementPickerScreenStoryboard Storyboard { get; }

		internal DisfigurementPickerStage PickerStage { get; set; }

		public override ChargenStage AssociatedStage => ChargenStage.SelectDisfigurements;

		public bool CanGoBack(DisfigurementPickerStage stage)
		{
			switch (stage)
			{
				case DisfigurementPickerStage.MissingBodyparts:
					return false;
				case DisfigurementPickerStage.Prosthetics:
					return Storyboard.AllowPickingMissingBodyparts;
				case DisfigurementPickerStage.Scars:
					return CanGoBack(DisfigurementPickerStage.Prosthetics) ||
					       Storyboard.PickableProstheses.Any(x => x.CanSelectProg.Execute<bool?>(Chargen) == true);
				case DisfigurementPickerStage.Tattoos:
					return CanGoBack(DisfigurementPickerStage.Scars) || Storyboard.AllowPickingScars;
			}

			return false;
		}

		public DisfigurementPickerStage GoBack(DisfigurementPickerStage stage)
		{
			switch (stage)
			{
				case DisfigurementPickerStage.Prosthetics:
					ResetStage(DisfigurementPickerStage.Prosthetics);
					UpdateChargen();
					if (Storyboard.AllowPickingMissingBodyparts)
					{
						return DisfigurementPickerStage.MissingBodyparts;
					}

					break;
				case DisfigurementPickerStage.Scars:
					ResetStage(DisfigurementPickerStage.Scars);
					UpdateChargen();
					if (Storyboard.PickableProstheses.Any(x => x.CanSelectProg.Execute<bool?>(Chargen) == true))
					{
						return DisfigurementPickerStage.Prosthetics;
					}

					return GoBack(DisfigurementPickerStage.Prosthetics);
				case DisfigurementPickerStage.Tattoos:
					ResetStage(DisfigurementPickerStage.Tattoos);
					UpdateChargen();
					if (Storyboard.AllowPickingScars)
					{
						return DisfigurementPickerStage.Scars;
					}

					return GoBack(DisfigurementPickerStage.Scars);
			}

			throw new ApplicationException("DisfigurementPickerScreen encountered error. This should never happen.");
		}

		public void ResetStage(DisfigurementPickerStage stage)
		{
			switch (stage)
			{
				case DisfigurementPickerStage.Tattoos:
					_selectedTattoo = null;
					_selectedTattoos.Clear();
					_filteredTattoos.Clear();
					_tattooTemplates.Clear();
					break;
				case DisfigurementPickerStage.Scars:
					_selectedScar = null;
					_selectedScars.Clear();
					_filteredScars.Clear();
					_scarTemplates.Clear();
					break;
				case DisfigurementPickerStage.MissingBodyparts:
					_missingBodyparts.Clear();
					break;
				case DisfigurementPickerStage.Prosthetics:
					_selectedProstheses.Clear();
					break;
			}
		}

		public void CheckCurrentStage()
		{
			_filteredKeywords.Clear();
			CalculateBodyparts();
			if (PickerStage == DisfigurementPickerStage.MissingBodyparts)
			{
				if (!Storyboard.AllowPickingMissingBodyparts)
				{
					PickerStage = DisfigurementPickerStage.Prosthetics;
				}
			}

			if (PickerStage == DisfigurementPickerStage.Prosthetics)
			{
				if (!Storyboard.PickableProstheses.Any(x => x.CanSelectProg.Execute<bool?>(Chargen) == true))
				{
					PickerStage = DisfigurementPickerStage.Scars;
				}
			}

			if (PickerStage == DisfigurementPickerStage.Scars)
			{
				if (!Storyboard.AllowPickingScars)
				{
					PickerStage = DisfigurementPickerStage.Tattoos;
				}
				else
				{
					SetupScars();
					if (!_scarTemplates.Any())
					{
						PickerStage = DisfigurementPickerStage.Tattoos;
					}
				}
			}

			if (PickerStage == DisfigurementPickerStage.Tattoos)
			{
				if (!Storyboard.AllowPickingTattoos)
				{
					State = ChargenScreenState.Complete;
					CompleteScreen();
					return;
				}

				SetupTattoos();
				if (!_tattooTemplates.Any())
				{
					State = ChargenScreenState.Complete;
					CompleteScreen();
				}
			}
		}

		private void CompleteScreen()
		{
			UpdateChargen();
		}

		private void SetupTattoos()
		{
			_tattooTemplates = Storyboard.Gameworld.DisfigurementTemplates
			                             .Where(x => x.Status == RevisionStatus.Current &&
			                                         x.AppearInChargenList(Chargen) &&
			                                         (!x.BodypartShapes.Any() ||
			                                          _effectiveBodyparts.Any(y => x.BodypartShapes.Contains(y.Shape))))
			                             .OfType<ITattooTemplate>()
			                             .ToList();
			_filteredTattoos = _tattooTemplates.Take(25).ToList();
		}

		private void SetupScars()
		{
			_scarTemplates = Storyboard.Gameworld.DisfigurementTemplates
			                           .Where(x => x.Status == RevisionStatus.Current &&
			                                       x.AppearInChargenList(Chargen) &&
			                                       (!x.BodypartShapes.Any() ||
			                                        _effectiveBodyparts.Any(y => x.BodypartShapes.Contains(y.Shape))))
			                           .OfType<IScarTemplate>()
			                           .ToList();
			_filteredScars = _scarTemplates.Take(25).ToList();
		}

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			switch (PickerStage)
			{
				case DisfigurementPickerStage.MissingBodyparts:
					return DisplayMissingBodyparts();
				case DisfigurementPickerStage.Prosthetics:
					return DisplayProsthetics();
				case DisfigurementPickerStage.Scars:
					return DisplayScars();
				case DisfigurementPickerStage.Tattoos:
					return DisplayTattoos();
			}

			throw new NotImplementedException();
		}

		private IEnumerable<IBodypart> BodypartsForTattoo(ITattooTemplate template)
		{
			return _effectiveBodyparts
					.Where(x =>
						(!template.BodypartShapes.Any() || template.BodypartShapes.Contains(x.Shape)) &&
						Chargen.SelectedRace.ModifiedSize(x) >= template.Size
					)
				;
		}

		private IEnumerable<IBodypart> BodypartsForScar(IScarTemplate template)
		{
			return _effectiveBodyparts
				.Where(x => !template.BodypartShapes.Any() || template.BodypartShapes.Contains(x.Shape));
		}

		private string DisplayTattoos()
		{
			var sb = new StringBuilder();
			if (_selectedTattoo != null)
			{
				sb.AppendLine($"Showing Tattoo {_selectedTattoo.ShortDescriptionForChargen.Colour(Telnet.BoldOrange)}");
				sb.AppendLine();
				sb.AppendLine(
					$"{_selectedTattoo.FullDescriptionForChargen.ProperSentences().Wrap(Account.InnerLineFormatLength, "\t")}");
				sb.AppendLine($"Size: {_selectedTattoo.Size.Describe().ColourValue()}");
				sb.AppendLine(
					$"Can Be Installed On: {BodypartsForTattoo(_selectedTattoo).Select(x => x.Name.Colour(Telnet.Yellow)).ListToString()}");
				sb.AppendLine(
					$"Costs: {_selectedTattoo.ChargenCosts.Select(x => $"{x.Value.ToString("N0", Account)} {x.Key.Name}".ColourValue()).DefaultIfEmpty("none".ColourValue()).ListToString()}");
				sb.AppendLine(
					$"Do you want to select this tattoo? Type the name of the bodypart to ink it on or {"no".ColourCommand()} to decline.");
				return sb.ToString();
			}


			sb.AppendLine("Tattoo Selection".Colour(Telnet.Cyan));
			sb.AppendLine(Storyboard.TattooBlurb.Wrap(Account.InnerLineFormatLength));
			sb.AppendLine();
			var i = 1;
			sb.AppendLine(StringUtilities.GetTextTable(
				from tattoo in _filteredTattoos
				select new[]
				{
					(i++).ToString("N0", Account),
					tattoo.ShortDescription,
					tattoo.Size.Describe(),
					tattoo.BodypartShapes.Select(x => x.Name).ListToString()
				},
				new[] { "#", "Tattoo", "Size", "Bodyparts", "Cost" },
				Account.LineFormatLength,
				colour: Telnet.Cyan,
				unicodeTable: Account.UseUnicode
			));
			sb.AppendLine(
				$"There are a total of {_tattooTemplates.Count.ToString("N0", Chargen.Account)} tattoos to choose from.");
			sb.AppendLine("Type a number to select or show more information about a tattoo.");
			if (!_filteredKeywords.Any())
			{
				sb.AppendLine(
					$"You can also type {"filter <keywords>".ColourCommand()} to filter for particular tattoos containing the mentioned keywords.");
			}
			else
			{
				sb.AppendLine(
					$"You are currently filtering by the keywords {_filteredKeywords.Select(x => x.Colour(Telnet.Cyan)).ListToString()}. Type {"clear".ColourCommand()} to reset this filter.");
			}

			if (CanGoBack(PickerStage))
			{
				sb.AppendLine($"Type {"back".ColourCommand()} to return to an earlier stage.");
			}

			if (_tattooTemplates.Count > 25 && !_filteredKeywords.Any())
			{
				sb.AppendLine(
					$"Type {"shuffle".ColourCommand()} to see another random 25 tattoos you can choose from.");
			}

			if (_selectedTattoos.Any())
			{
				sb.AppendLine(
					$"You have selected {_selectedTattoos.Count.ToString("N0", Account).ColourValue()} {(_selectedTattoos.Count == 1 ? "tattoo" : "tattoos")}. Type {"reset".ColourCommand()} to clear them or {"mine".ColourCommand()} to see the ones you already have.");
			}

			sb.AppendLine($"Type {"done".ColourCommand()} when you are done picking tattoos.");
			return sb.ToString();
		}

		private string DisplayScars()
		{
			var sb = new StringBuilder();
			if (_selectedScar != null)
			{
				sb.AppendLine($"Showing Scar {_selectedScar.ShortDescriptionForChargen.Colour(Telnet.BoldPink)}");
				sb.AppendLine();
				sb.AppendLine(
					$"{_selectedScar.FullDescriptionForChargen.ProperSentences().Wrap(Account.InnerLineFormatLength, "\t")}");
				sb.AppendLine(
					$"Can Be Installed On: {BodypartsForScar(_selectedScar).Select(x => x.Name.Colour(Telnet.Yellow)).ListToString()}");
				sb.AppendLine(
					$"Costs: {_selectedScar.ChargenCosts.Select(x => $"{x.Value.ToString("N0", Account)} {x.Key.Name}".ColourValue()).DefaultIfEmpty("none".ColourValue()).ListToString()}");
				sb.AppendLine(
					$"Do you want to select this scar? Type the name of the bodypart to inflict it on or {"no".ColourCommand()} to decline.");
				return sb.ToString();
			}


			sb.AppendLine("Scar Selection".Colour(Telnet.Cyan));
			sb.AppendLine(Storyboard.ScarBlurb.Wrap(Account.InnerLineFormatLength));
			sb.AppendLine();
			var i = 1;
			sb.AppendLine(StringUtilities.GetTextTable(
				from scar in _filteredScars
				select new[]
				{
					(i++).ToString("N0", Account),
					scar.ShortDescription,
					scar.BodypartShapes.Select(x => x.Name).ListToString()
				},
				new[] { "#", "Scar", "Bodyparts", "Cost" },
				Account.LineFormatLength,
				colour: Telnet.Cyan,
				unicodeTable: Account.UseUnicode
			));
			sb.AppendLine(
				$"There are a total of {_scarTemplates.Count.ToString("N0", Chargen.Account)} scars to choose from.");
			sb.AppendLine("Type a number to select or show more information about a scar.");
			if (!_filteredKeywords.Any())
			{
				sb.AppendLine(
					$"You can also type {"filter <keywords>".ColourCommand()} to filter for particular scars containing the mentioned keywords.");
			}
			else
			{
				sb.AppendLine(
					$"You are currently filtering by the keywords {_filteredKeywords.Select(x => x.Colour(Telnet.Cyan)).ListToString()}. Type {"clear".ColourCommand()} to reset this filter.");
			}

			if (CanGoBack(PickerStage))
			{
				sb.AppendLine($"Type {"back".ColourCommand()} to return to an earlier stage.");
			}

			if (_scarTemplates.Count > 25 && !_filteredKeywords.Any())
			{
				sb.AppendLine($"Type {"shuffle".ColourCommand()} to see another random 25 scars you can choose from.");
			}

			if (_selectedScars.Any())
			{
				sb.AppendLine(
					$"You have selected {_selectedScars.Count.ToString("N0", Account).ColourValue()} {(_selectedScars.Count == 1 ? "scar" : "scars")}. Type {"reset".ColourCommand()} to clear them or {"mine".ColourCommand()} to see the ones you already have.");
			}

			sb.AppendLine($"Type {"done".ColourCommand()} when you are done picking scars.");
			return sb.ToString();
		}

		private string DisplayProsthetics()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Starting Prosthetic Selection".Colour(Telnet.Cyan));
			sb.AppendLine(Storyboard.ProstheticsBlurb.Wrap(Account.InnerLineFormatLength));
			sb.AppendLine();
			var pickables = Storyboard.PickableProstheses
			                          .Where(x =>
				                          x.CanSelectProg.Execute<bool?>(Chargen) == true &&
				                          Chargen.SelectedRace.BaseBody.CountsAs(x.ProstheticProto.TargetBody) &&
				                          _missingBodyparts.Any(y => y.CountsAs(x.ProstheticProto.TargetBodypart)) &&
				                          !_chargenProsthesisSelections.Any(y =>
					                          y.ProstheticProto.TargetBodypart.CountsAs(
						                          x.ProstheticProto.TargetBodypart) ||
					                          x.ProstheticProto.TargetBodypart.CountsAs(
						                          y.ProstheticProto.TargetBodypart))
			                          )
			                          .OrderBy(x =>
				                          (x.ProstheticProto.TargetBodypart as IExternalBodypart)?.DisplayOrder ?? 0)
			                          .ToList();
			var i = 1;
			foreach (var pickable in pickables)
			{
				sb.AppendLine(
					$"{(i++).ToString("N0", Account)}) {$"{pickable.Item.ShortDescription,30}".Colour(pickable.Item.CustomColour ?? Telnet.Green)} {pickable.ResourceCosts.Select(x => $"{x.Value.ToString("N0", Account)} {x.Key.Alias}".ColourValue()).ListToString().SquareBrackets(),-20}");
			}

			sb.AppendLine();
			if (_selectedProstheses.Any())
			{
				sb.AppendLine(
					$"You have selected {_selectedProstheses.Select(x => x.ShortDescription.Colour(x.CustomColour ?? Telnet.Green)).ListToString()}, costing {_chargenProsthesisSelections.SelectMany(x => x.ResourceCosts).GroupBy(x => x.Key).Select(x => $"{x.Sum(y => y.Value)} {x.Key.Alias}".ColourValue()).DefaultIfEmpty("nothing".ColourValue()).ListToString()}.");
			}

			sb.AppendLine(
				$"Type the number of the option you want to select, {"view <number>".ColourCommand()}{(_selectedProstheses.Any() ? $", or {"clear <keyword>".ColourCommand()} to clear a selection" : "")}.");
			sb.AppendLine(
				$"When you are done type {"done".ColourCommand()} to proceed, or {"back".ColourCommand()} to go back.");
			return sb.ToString();
		}

		private string DisplayMissingBodyparts()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Missing Bodypart Selection".Colour(Telnet.Cyan));
			sb.AppendLine(Storyboard.BodypartsBlurb.Wrap(Account.InnerLineFormatLength));
			if (_missingBodyparts.Any())
			{
				sb.AppendLine(
					$"Your character is missing {Gendering.Get(Chargen.SelectedGender).Possessive()} {_missingBodyparts.Select(x => x.FullDescription().Colour(Telnet.Red)).ListToString()}."
						.ColourIncludingReset(Telnet.Yellow).Wrap(Account.InnerLineFormatLength));
			}
			else
			{
				sb.AppendLine("Your character will not be missing any bodyparts.".Colour(Telnet.BoldGreen));
			}

			sb.AppendLine(
				$"Type {"done".ColourCommand()} when you are done, {"parts".ColourCommand()} to list your parts, or the name of a part to toggle its presence or absence.");
			return sb.ToString();
		}

		private void UpdateChargen()
		{
			Chargen.MissingBodyparts = _missingBodyparts.ToList();
			Chargen.SelectedDisfigurements =
				_selectedTattoos
					.Select(x => ((IDisfigurementTemplate)x.Tattoo, x.Bodypart))
					.Concat(
						_selectedScars
							.Select(x => ((IDisfigurementTemplate)x.Scar, x.Bodypart))
					)
					.ToList();
			Chargen.SelectedProstheses = _selectedProstheses.ToList();
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			switch (PickerStage)
			{
				case DisfigurementPickerStage.Tattoos:
					return HandleCommandTattoos(command);
				case DisfigurementPickerStage.Scars:
					return HandleCommandScars(command);
				case DisfigurementPickerStage.MissingBodyparts:
					return HandleCommandBodyparts(command);
				case DisfigurementPickerStage.Prosthetics:
					return HandleCommandProsthetics(command);
			}

			throw new ApplicationException("Unknown PickerStage in DisfigurementPickerScreen.HandleCommand");
		}

		private string HandleCommandProsthetics(string command)
		{
			if (command.EqualTo("done"))
			{
				PickerStage = DisfigurementPickerStage.Scars;
				CheckCurrentStage();
				if (State != ChargenScreenState.Complete)
				{
					return Display();
				}

				return "";
			}

			var pickables = Storyboard.PickableProstheses
			                          .Where(x =>
				                          x.CanSelectProg.Execute<bool?>(Chargen) == true &&
				                          Chargen.SelectedRace.BaseBody.CountsAs(x.ProstheticProto.TargetBody) &&
				                          _missingBodyparts.Any(y => y.CountsAs(x.ProstheticProto.TargetBodypart)) &&
				                          !_chargenProsthesisSelections.Any(y =>
					                          y.ProstheticProto.TargetBodypart.CountsAs(
						                          x.ProstheticProto.TargetBodypart) ||
					                          x.ProstheticProto.TargetBodypart.CountsAs(
						                          y.ProstheticProto.TargetBodypart))
			                          )
			                          .OrderBy(x =>
				                          (x.ProstheticProto.TargetBodypart as IExternalBodypart)?.DisplayOrder ?? 0)
			                          .ToList();
			var ss = new StringStack(command);
			var cmd = ss.PopSpeech();
			if (cmd.EqualTo("clear"))
			{
				if (ss.IsFinished)
				{
					return "Which prosthetic selection do you want to clear?";
				}

				var targetItem = _selectedProstheses.GetFromItemListByKeyword(ss.PopSpeech(), null);
				if (targetItem == null)
				{
					return "You have not selected any prosthesis like that to clear.";
				}

				_selectedProstheses.Remove(targetItem);
				_chargenProsthesisSelections.RemoveAll(x => x.Item == targetItem);
				UpdateChargen();
				return Display();
			}

			if (cmd.EqualTo("reset"))
			{
				_selectedProstheses.Clear();
				_chargenProsthesisSelections.Clear();
				UpdateChargen();
				return Display();
			}

			if (cmd.EqualTo("back"))
			{
				if (CanGoBack(DisfigurementPickerStage.Prosthetics))
				{
					PickerStage = GoBack(DisfigurementPickerStage.Prosthetics);
					return Display();
				}

				return "You cannot go back any further.";
			}

			int value;
			ChargenProsthesisSelection target;
			if (cmd.EqualTo("view"))
			{
				if (ss.IsFinished)
				{
					return "Which pickable prosthesis do you want to view detailed information for?";
				}

				if (!int.TryParse(ss.PopSpeech(), out value))
				{
					return "You must select the number of a prosthesis that you want to view.";
				}

				target = pickables.ElementAtOrDefault(value - 1);
				if (target == null)
				{
					return "There is no such pickable prosthesis.";
				}

				var sb = new StringBuilder();
				sb.AppendLine($"Option #{value.ToString("N0", Account)}");
				sb.AppendLine(
					$"Sdesc: {target.Item.ShortDescription.Colour(target.Item.CustomColour ?? Telnet.Green)}");
				sb.AppendLine($"Quality: {target.Item.BaseItemQuality.Describe().ColourValue()}");
				sb.AppendLine($"Bodypart: {target.ProstheticProto.TargetBodypart.FullDescription().ColourValue()}");
				sb.AppendLine($"Functional: {target.ProstheticProto.Functional.ToColouredString()}");
				sb.AppendLine($"Obvious: {target.ProstheticProto.Obvious.ToColouredString()}");
				sb.AppendLine(
					$"Costs: {target.ResourceCosts.Select(x => $"{x.Value.ToString("N0", Account)} {x.Key.Alias}".ColourValue()).DefaultIfEmpty("none".ColourValue()).ListToString()}");
				sb.AppendLine(
					$"Description:\n\n{target.Item.FullDescription.Wrap(Account.InnerLineFormatLength, "\t")}");
				return sb.ToString();
			}

			if (!int.TryParse(cmd, out value))
			{
				return "That is not a valid option.";
			}

			target = pickables.ElementAtOrDefault(value - 1);
			if (target == null)
			{
				return "There is no such pickable prosthesis.";
			}

			_selectedProstheses.Add(target.Item);
			_chargenProsthesisSelections.Add(target);
			UpdateChargen();
			return Display();
		}

		private string HandleCommandTattoos(string command)
		{
			if (_selectedTattoo != null)
			{
				if (command.EqualTo("no"))
				{
					_selectedTattoo = null;
					return Display();
				}

				var bodypart = BodypartsForTattoo(_selectedTattoo).FirstOrDefault(x => x.Name.EqualTo(command));
				if (bodypart == null)
				{
					return
						$"Your character possesses no such bodypart. Please enter a valid bodypart or {"no".ColourCommand()} to clear your selection.";
				}

				_selectedTattoos.Add((_selectedTattoo, bodypart));
				var tattoo = _selectedTattoo;
				_selectedTattoo = null;
				UpdateChargen();
				return
					$"You add {tattoo.ShortDescriptionForChargen.Colour(Telnet.BoldOrange)} to your {bodypart.FullDescription().Colour(Telnet.Yellow)}.\n\n{Display()}";
			}

			if (command.EqualTo("back"))
			{
				if (!CanGoBack(DisfigurementPickerStage.Tattoos))
				{
					return "There is no prior stage to which you can go back.";
				}

				PickerStage = GoBack(PickerStage);
				CheckCurrentStage();
				return Display();
			}

			if (command.EqualTo("reset"))
			{
				_selectedTattoos.Clear();
				_selectedTattoo = null;
				_filteredTattoos.Clear();
				SetupTattoos();
				UpdateChargen();
				return Display();
			}

			if (command.EqualTo("done"))
			{
				State = ChargenScreenState.Complete;
				return "";
			}

			if (command.EqualTo("shuffle"))
			{
				if (!_filteredKeywords.Any())
				{
					_filteredTattoos = _tattooTemplates.Except(_filteredTattoos).Shuffle().Take(25).ToList();
					return Display();
				}

				var filtered = new List<ITattooTemplate>();
				foreach (var tattoo in _tattooTemplates)
				{
					if (_filteredTattoos.Contains(tattoo))
					{
						continue;
					}

					if (_filteredKeywords.All(x =>
						    tattoo.FullDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase) ||
						    tattoo.ShortDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
					{
						filtered.Add(tattoo);
						if (filtered.Count >= 25)
						{
							break;
						}
					}
				}

				_filteredTattoos = filtered;
				return Display();
			}

			if (command.EqualTo("mine"))
			{
				if (!_selectedTattoos.Any())
				{
					return "You have not selected any tattoos yet.";
				}

				return
					$"You have selected the following tattoos:\n{_selectedTattoos.Select(x => $"\t{x.Tattoo.ShortDescriptionForChargen.Colour(Telnet.BoldOrange)} on the {x.Bodypart.FullDescription().Colour(Telnet.Yellow)}").ListToCommaSeparatedValues("\n")}";
			}

			if (_filteredKeywords.Any() && command.EqualTo("clear"))
			{
				_filteredKeywords.Clear();
				_filteredTattoos = _tattooTemplates.Shuffle().Take(25).ToList();
				return Display();
			}

			var ss = new StringStack(command);
			var cmd = ss.PopSpeech();
			if (cmd.EqualTo("filter"))
			{
				if (ss.IsFinished)
				{
					return
						$"You must either specify keywords to toggle or use {"clear".ColourCommand()} to clear your existing filters.";
				}

				while (!ss.IsFinished)
				{
					cmd = ss.PopSpeech().ToLowerInvariant();
					if (!_filteredKeywords.Remove(cmd))
					{
						_filteredKeywords.Add(cmd);
					}
				}

				if (!_filteredKeywords.Any())
				{
					_filteredTattoos = _tattooTemplates.Shuffle().Take(25).ToList();
					return Display();
				}

				var filtered = new List<ITattooTemplate>();
				foreach (var tattoo in _tattooTemplates)
				{
					if (_filteredKeywords.All(x =>
						    tattoo.FullDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase) ||
						    tattoo.ShortDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
					{
						filtered.Add(tattoo);
						if (filtered.Count >= 25)
						{
							break;
						}
					}
				}

				_filteredTattoos = filtered;
				return Display();
			}

			if (!int.TryParse(command, out var value))
			{
				return "That is not a valid tattoo number or command.";
			}

			_selectedTattoo = _filteredTattoos.ElementAtOrDefault(value - 1);
			if (_selectedTattoo == null)
			{
				return "There is no such tattoo.";
			}

			return Display();
		}

		private string HandleCommandScars(string command)
		{
			if (_selectedScar != null)
			{
				if (command.EqualTo("no"))
				{
					_selectedScar = null;
					return Display();
				}

				var bodypart = BodypartsForScar(_selectedScar).FirstOrDefault(x => x.Name.EqualTo(command));
				if (bodypart == null)
				{
					return
						$"Your character possesses no such bodypart. Please enter a valid bodypart or {"no".ColourCommand()} to clear your selection.";
				}

				_selectedScars.Add((_selectedScar, bodypart));
				var scar = _selectedScar;
				_selectedScar = null;
				UpdateChargen();
				return
					$"You add {scar.ShortDescriptionForChargen.Colour(Telnet.BoldPink)} to your {bodypart.FullDescription().Colour(Telnet.Yellow)}.\n\n{Display()}";
			}

			if (command.EqualTo("back"))
			{
				if (!CanGoBack(DisfigurementPickerStage.Scars))
				{
					return "There is no prior stage to which you can go back.";
				}

				PickerStage = GoBack(PickerStage);
				CheckCurrentStage();
				return Display();
			}

			if (command.EqualTo("reset"))
			{
				_selectedScars.Clear();
				_selectedScar = null;
				_filteredScars.Clear();
				SetupScars();
				UpdateChargen();
				return Display();
			}

			if (command.EqualTo("done"))
			{
				PickerStage = DisfigurementPickerStage.Tattoos;
				CheckCurrentStage();
				if (State != ChargenScreenState.Complete)
				{
					return Display();
				}

				return "";
			}

			if (command.EqualTo("mine"))
			{
				if (!_selectedScars.Any())
				{
					return "You have not selected any scars yet.";
				}

				return
					$"You have selected the following scars:\n{_selectedScars.Select(x => $"\t{x.Scar.ShortDescriptionForChargen.Colour(Telnet.BoldPink)} on the {x.Bodypart.FullDescription().Colour(Telnet.Yellow)}").ListToCommaSeparatedValues("\n")}";
			}

			if (command.EqualTo("shuffle"))
			{
				if (!_filteredKeywords.Any())
				{
					_filteredScars = _scarTemplates.Except(_filteredScars).Shuffle().Take(25).ToList();
					return Display();
				}

				var filtered = new List<IScarTemplate>();
				foreach (var scar in _scarTemplates)
				{
					if (_filteredScars.Contains(scar))
					{
						continue;
					}

					if (_filteredKeywords.All(x =>
						    scar.FullDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase) ||
						    scar.ShortDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
					{
						filtered.Add(scar);
						if (filtered.Count >= 25)
						{
							break;
						}
					}
				}

				_filteredScars = filtered;
				return Display();
			}

			var ss = new StringStack(command);
			var cmd = ss.PopSpeech();
			if (cmd.EqualTo("filter"))
			{
				if (ss.IsFinished)
				{
					return
						$"You must either specify keywords to toggle or use {"clear".ColourCommand()} to clear your existing filters.";
				}

				while (!ss.IsFinished)
				{
					cmd = ss.PopSpeech().ToLowerInvariant();
					if (!_filteredKeywords.Remove(cmd))
					{
						_filteredKeywords.Add(cmd);
					}
				}

				if (!_filteredKeywords.Any())
				{
					_filteredScars = _scarTemplates.Shuffle().Take(25).ToList();
					return Display();
				}

				var filtered = new List<IScarTemplate>();
				foreach (var scar in _scarTemplates)
				{
					if (_filteredKeywords.All(x =>
						    scar.FullDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase) ||
						    scar.ShortDescriptionForChargen.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
					{
						filtered.Add(scar);
						if (filtered.Count >= 25)
						{
							break;
						}
					}
				}

				_filteredScars = filtered;
				return Display();
			}

			if (!int.TryParse(command, out var value))
			{
				return "That is not a valid scar number or command.";
			}

			_selectedScar = _filteredScars.ElementAtOrDefault(value - 1);
			if (_selectedScar == null)
			{
				return "There is no such scar.";
			}

			return Display();
		}

		private string HandleCommandBodyparts(string command)
		{
			if (command.EqualTo("done"))
			{
				PickerStage = DisfigurementPickerStage.Prosthetics;
				CheckCurrentStage();
				if (State != ChargenScreenState.Complete)
				{
					return Display();
				}

				return "";
			}

			if (command.EqualTo("parts"))
			{
				var parts = Chargen.SelectedRace.BaseBody
				                   .BodypartsFor(Chargen.SelectedRace, Chargen.SelectedGender)
				                   .OfType<IExternalBodypart>()
				                   .PruneBodyparts(_missingBodyparts)
				                   .ToList();
				var root = parts.First(x => x.UpstreamConnection == null);
				var sb = new StringBuilder();
				sb.AppendLine("Your current bodyparts:");

				void DescribeLevel(IBodypart proto, int level)
				{
					sb.AppendLine(new string('\t', level) + proto.FullDescription());
					foreach (var branch in parts.Where(x => x.UpstreamConnection == proto).ToList())
					{
						DescribeLevel(branch, level + 1);
					}
				}

				DescribeLevel(root, 0);
				return sb.ToString();
			}

			var allparts = Chargen.SelectedRace.BaseBody
			                      .BodypartsFor(Chargen.SelectedRace, Chargen.SelectedGender)
			                      .OfType<IExternalBodypart>()
			                      .ToList();
			var part = allparts.FirstOrDefault(x => x.Name.EqualTo(command)) ??
			           allparts.FirstOrDefault(x => x.FullDescription().EqualTo(command)) ??
			           allparts.FirstOrDefault(x =>
				           x.FullDescription().StartsWith(command, StringComparison.InvariantCultureIgnoreCase)) ??
			           allparts.FirstOrDefault(x =>
				           x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
			if (part == null)
			{
				return "You have no such bodypart.";
			}

			if (part.IsVital)
			{
				return "You cannot sever vital bodyparts.";
			}

			if (_effectiveBodyparts.Contains(part))
			{
				_missingBodyparts.Add(part);
				CalculateBodyparts();
				UpdateChargen();
				return Display();
			}

			if (_missingBodyparts.Any(x => part.DownstreamOfPart(x)))
			{
				return "You have a severed bodypart upstream of that part.";
			}

			_missingBodyparts.Remove(part);
			CalculateBodyparts();
			UpdateChargen();
			return Display();
		}

		private void CalculateBodyparts()
		{
			_effectiveBodyparts = Chargen.SelectedRace.BaseBody
			                             .BodypartsFor(Chargen.SelectedRace, Chargen.SelectedGender)
			                             .OfType<IExternalBodypart>()
			                             .PruneBodyparts(_missingBodyparts)
			                             .ToList();
		}

		internal enum DisfigurementPickerStage
		{
			Tattoos,
			Scars,
			MissingBodyparts,
			Prosthetics
		}
	}

	internal class ChargenProsthesisSelection
	{
		public IGameItemProto Item { get; set; }
		public ProstheticGameItemComponentProto ProstheticProto { get; set; }
		public Counter<IChargenResource> ResourceCosts { get; set; }
		public IFutureProg CanSelectProg { get; set; }
	}
}