using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class PhotocopierGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	public PhotocopierGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Photocopier")
	{
		PaperWeightCapacity = 1.5 / Gameworld.UnitManager.BaseWeightToKilograms;
		MaximumCharactersPrintedPerCartridge = 2400000;
	}

	public PhotocopierGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("photocopier", true,
			(gameworld, account) => new PhotocopierGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Photocopier",
			(proto, gameworld) => new PhotocopierGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Photocopier",
			$"A {"[powered]".Colour(Telnet.Magenta)} machine that can copy all the text on a sheet of {"[paper]".Colour(Telnet.Yellow)}",
			BuildingHelpText
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		InkCartridgePrototypeId = long.Parse(root.Element("InkCartridgePrototypeId").Value);
		SpentInkCartridgePrototypeId = long.Parse(root.Element("SpentInkCartridgePrototypeId").Value);
		MaximumCharactersPrintedPerCartridge = int.Parse(root.Element("MaximumCharactersPrintedPerCartridge").Value);
		PaperWeightCapacity = double.Parse(root.Element("PaperWeightCapacity").Value);
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("InkCartridgePrototypeId", InkCartridgePrototypeId));
		root.Add(new XElement("SpentInkCartridgePrototypeId", SpentInkCartridgePrototypeId));
		root.Add(new XElement("MaximumCharactersPrintedPerCartridge", MaximumCharactersPrintedPerCartridge));
		root.Add(new XElement("PaperWeightCapacity", PaperWeightCapacity));
		return root;
	}

	public override string TypeDescription => "Photocopier";

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PhotocopierGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PhotocopierGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PhotocopierGameItemComponentProto(proto, gameworld));
	}

	protected override string ComponentDescriptionOLCByline => "This item is a photocopier";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(PaperWeightCapacity, Framework.Units.UnitType.Mass, actor).ColourValue()} of paper.\nIt uses {Gameworld.ItemProtos.Get(InkCartridgePrototypeId)?.EditHeader() ?? "Not yet set".Colour(Telnet.Red)} as an ink cartridge.\nIt uses {Gameworld.ItemProtos.Get(SpentInkCartridgePrototypeId)?.EditHeader() ?? "Not yet set".Colour(Telnet.Red)} as a spent ink cartridge.\nIt prints {MaximumCharactersPrintedPerCartridge.ToString("N0", actor).ColourValue()} characters per cartridge refill (approximately {(MaximumCharactersPrintedPerCartridge / 2400).ToString("N0", actor).ColourValue()} pages)";
	}

	public long InkCartridgePrototypeId { get; protected set; }

	public long SpentInkCartridgePrototypeId { get; protected set; }

	public int MaximumCharactersPrintedPerCartridge { get; protected set; }

	public double PaperWeightCapacity { get; protected set; }

	public override bool CanSubmit()
	{
		if (InkCartridgePrototypeId == 0)
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (InkCartridgePrototypeId == 0)
		{
			return "You must set an ink cartridge prototype.";
		}

		return base.WhyCannotSubmit();
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tink <proto> - sets an item proto to be the ink cartridge\nspent <proto> - sets an item proto to be loaded as the spent ink cartridge\nuses <amount> - the number of characters of text to be printed by a full cartridge\npaper <weight> - the weight of paper this photocopier can hold";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ink":
				return BuildingCommandInkCartridge(actor, command);
			case "spent":
				return BuildingCommandSpentCartridge(actor, command);
			case "uses":
				return BuildingCommandUses(actor, command);
			case "paper":
			case "capacity":
				return BuildingCommandCapacity(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
	}

	private bool BuildingCommandSpentCartridge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which item prototype should serve as the spent ink cartridge for this photocopier?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid ID number.");
			return false;
		}

		var proto = Gameworld.ItemProtos.Get(value);
		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				"There is no currently approved version of that prototype, and it is therefore invalid.");
			return false;
		}

		SpentInkCartridgePrototypeId = proto.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This photocopier will now produce instances of {proto.EditHeader()} when its spent ink cartridges are removed.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much weight of paper should this photocopier be able to hold?");
			return false;
		}

		var amount = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, Framework.Units.UnitType.Mass,
			out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid weight.");
			return false;
		}

		PaperWeightCapacity = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This photocopier can now hold {Gameworld.UnitManager.DescribeMostSignificantExact(PaperWeightCapacity, Framework.Units.UnitType.Mass, actor).ColourValue()} of paper.");
		return true;
	}

	private bool BuildingCommandUses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many characters of basic text should this photocopier be able to print before requiring an ink cartridge refill?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		MaximumCharactersPrintedPerCartridge = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This photocopier will now have {MaximumCharactersPrintedPerCartridge.ToString("N0", actor).ColourValue()} uses between ink cartridge refills (approximately {(MaximumCharactersPrintedPerCartridge / 2400).ToString("N0", actor).ColourValue()} pages)");
		return true;
	}

	private bool BuildingCommandInkCartridge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item prototype should serve as the ink cartridge for this photocopier?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid ID number.");
			return false;
		}

		var proto = Gameworld.ItemProtos.Get(value);
		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				"There is no currently approved version of that prototype, and it is therefore invalid.");
			return false;
		}

		InkCartridgePrototypeId = proto.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This photocopier will now consume instances of {proto.EditHeader()} to refill its ink.");
		return true;
	}
}