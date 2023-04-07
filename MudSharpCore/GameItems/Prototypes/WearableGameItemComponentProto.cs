using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class WearableGameItemComponentProto : GameItemComponentProto
{
	protected readonly List<IWearProfile> _profiles = new();
	private bool _displayInventoryWhenWorn = true;

	protected WearableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected WearableGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type = "Wearable")
		: base(gameworld, originator, type)
	{
	}

	public IEnumerable<IWearProfile> Profiles => _profiles;

	public IWearProfile DefaultProfile { get; protected set; }

	public IFutureProg WearableProg { get; protected set; }

	public IFutureProg WhyCannotWearProg { get; protected set; }

	public bool Bulky { get; protected set; }
	public double LayerWeightConsumption { get; protected set; }

	public bool Waterproof { get; protected set; }

	public double SeeThroughDamageRatio { get; protected set; }

	public double WaterproofDamageRatio { get; protected set; }

	/// <summary>
	///     Indicates whether this item should be displayed or hidden when worn in inventory
	/// </summary>
	public bool DisplayInventoryWhenWorn
	{
		get => _displayInventoryWhenWorn;
		protected set
		{
			_displayInventoryWhenWorn = value;
			Changed = true;
		}
	}

	public override string TypeDescription => "Wearable";

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("DisplayInventoryWhenWorn");
		DisplayInventoryWhenWorn = attr == null || bool.Parse(attr.Value);

		attr = root.Attribute("Bulky");
		if (attr != null)
		{
			Bulky = bool.Parse(attr.Value);
		}

		var element = root.Element("Profiles");
		if (element != null)
		{
			foreach (var sub in element.Elements("Profile"))
			{
				_profiles.Add(Gameworld.WearProfiles.Get(long.Parse(sub.Value)));
			}

			DefaultProfile = _profiles.FirstOrDefault(x => x.Id == long.Parse(element.Attribute("Default").Value));
		}

		element = root.Element("WearableProg");
		if (element != null)
		{
			WearableProg = Gameworld.FutureProgs.Get(long.Parse(element.Value));
		}

		element = root.Element("WhyCannotWearProg");
		if (element != null)
		{
			WhyCannotWearProg = Gameworld.FutureProgs.Get(long.Parse(element.Value));
		}

		element = root.Element("SeeThroughDamageRatio");
		SeeThroughDamageRatio = element != null ? double.Parse(element.Value) : 0.5;

		element = root.Element("LayerWeightConsumption");
		LayerWeightConsumption = element != null ? double.Parse(element.Value) : 1.0;

		if (DefaultProfile == null)
		{
			DefaultProfile = _profiles.FirstOrDefault();
		}

		element = root.Element("Waterproof");
		Waterproof = element != null ? bool.Parse(element.Value) : false;
		WaterproofDamageRatio = element != null ? double.Parse(element.Attribute("ratio").Value) : 0.5;
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("DisplayInventoryWhenWorn", DisplayInventoryWhenWorn),
				new XAttribute("Bulky", Bulky),
				new XElement("WearableProg", WearableProg?.Id ?? 0),
				new XElement("WhyCannotWearProg", WhyCannotWearProg?.Id ?? 0),
				new XElement("SeeThroughDamageRatio", SeeThroughDamageRatio),
				new XElement("LayerWeightConsumption", LayerWeightConsumption),
				new XElement("Waterproof", new XAttribute("ratio", WaterproofDamageRatio),
					new XCData(Waterproof.ToString())),
				new XElement("Profiles", new XAttribute("Default", DefaultProfile?.Id ?? 0),
					from profile in _profiles
					select new XElement("Profile", profile.Id))).ToString();
	}

	public static string BuildingHelp => $@"You can use the following options:

	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	add <wearprofile> - adds a wear profile. See {"show wears".FluentTagMXP("send", "href='show wears'")} for a list.
	remove <wearprofile> - removes a wear profile.
	default <wearprofile> - sets a wear profile as the default choice.
	hidden - toggles whether this wearable shows up when you look at someone
	bulky - toggles whether this item counts as bulky.
	weight <#> - sets the layer weight consumed by this item
	waterproof - toggles waterproofing.
	waterproof [<percentage>] - sets an item to waterproof at HP remaining above specified threshold.
	damage <amount> - for a value between 0 and 1, sets the percentage of total hitpoints remaining before this item no longer hides things below it.
	prog <prog> - sets a prog that determines who can wear this item. Requires a prog that returns a bool and accepts a character and an item.
	failprog <prog> - sets a prog that determines what echo a person should get when they can't wear the item because of the other prog. Requires a prog that returns a text and accepts a character and an item.";

	public override string ShowBuildingHelp => BuildingHelp;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant().CollapseString())
		{
			case "default":
				return BuildingCommand_Default(actor, command);
			case "add":
				return BuildingCommand_Add(actor, command);
			case "remove":
				return BuildingCommand_Remove(actor, command);
			case "hidden":
				return BuildingCommand_Hidden(actor, command);
			case "prog":
				return BuildingCommand_Prog(actor, command);
			case "failprog":
				return BuildingCommand_FailProg(actor, command);
			case "bulky":
				return BuildingCommand_Bulky(actor, command);
			case "damage":
				return BuildingCommand_Damage(actor, command);
			case "waterproofing":
			case "waterproof":
			case "water":
				return BuildingCommand_Waterproof(actor, command);
			case "weight":
			case "layers":
			case "layer":
			case "layerweight":
				return BuildingCommandLayerWeight(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Waterproof(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			Waterproof = !Waterproof;
			Changed = true;
			actor.OutputHandler.Send($"This garment is {(Waterproof ? "now" : "no longer")} waterproof.");
			return true;
		}

		var arg = command.PopSpeech();
		if (arg.TryParsePercentage(out var value))
		{
			Waterproof = true;
			WaterproofDamageRatio = value;
			Changed = true;
			actor.OutputHandler.Send(
				$"This garment is now waterproof when it was more than {WaterproofDamageRatio:P3} HP remaining.");
			return true;
		}

		if (!double.TryParse(arg, out value) || value < 0.0 || value > 1.0)
		{
			actor.Send(
				"You must enter a number between 0.0 and 1.0 for this ratio, unless you use a percentage.");
			return false;
		}

		Waterproof = true;
		WaterproofDamageRatio = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This garment is now waterproof when it was more than {WaterproofDamageRatio:P3} HP remaining.");
		return true;
	}

	private bool BuildingCommand_Damage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"At what ratio of its total hit points would you like this item to no longer hide wounds and covered items?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0 || value > 1.0)
		{
			actor.Send(
				"You must enter a number between 0.0 and 1.0 for this ratio. Note - setting to 0.0 disables this functionality.");
			return false;
		}

		SeeThroughDamageRatio = value;
		Changed = true;
		actor.Send(
			$"This item will now cease hiding wounds and covered items at {SeeThroughDamageRatio:P3} of its total health.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{2:N0}r{3:N0}, {4})\n\nThis item can be worn by characters in the following profiles: {1}. This item {7} bulky. This item {5} hidden to others when worn. {8}. {6}",
			"Wearable Item Component".Colour(Telnet.Cyan),
			_profiles.Select(
				         x =>
					         x.Name.ToLowerInvariant().Colour(Telnet.Yellow) +
					         (x == DefaultProfile ? " (default)" : ""))
			         .ListToString(),
			Id,
			RevisionNumber,
			Name,
			DisplayInventoryWhenWorn ? "is not" : "is",
			WearableProg == null
				? "It does not use a prog to determine who can wear it."
				: $"It uses the {WearableProg.FunctionName} (#{WearableProg.Id:N0}) to determine who can wear it.",
			Bulky ? "is" : "is not",
			Waterproof
				? $"It is waterproof when above {WaterproofDamageRatio.ToString("P3", actor).Colour(Telnet.Green)} damage"
				: "It is not waterproof"
		);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("wearable", true,
			(gameworld, account) => new WearableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Wearable",
			(proto, gameworld) => new WearableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Wearable",
			$"Turns the item into a {"[wearable]".Colour(Telnet.BoldYellow)}",
			BuildingHelp
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new WearableGameItemComponent(this, parent, loader?.Body, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new WearableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new WearableGameItemComponentProto(proto, gameworld));
	}

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		return WearableProg == null || WhyCannotWearProg != null;
	}

	#endregion

	#region Sub Building Commands

	private bool BuildingCommand_FailProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must specify an ID or name of a prog to use.");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Text)
		{
			actor.Send("You may only use a prog that returns a text value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.Send(
				"The wearable prog you specify must be compatible with a single character and item parameter.");
			return false;
		}

		WhyCannotWearProg = prog;
		Changed = true;
		actor.Send(
			"This wearable will now use the {0} prog to echo a failure message when a prog determines that it cannot be worn.",
			$"{WhyCannotWearProg.FunctionName} (#{WhyCannotWearProg.Id:N0})");
		return true;
	}

	private bool BuildingCommand_Bulky(ICharacter actor, StringStack command)
	{
		Bulky = !Bulky;
		Changed = true;
		actor.Send($"This component {(Bulky ? "now" : "no longer")} counts as bulky when worn.");
		return true;
	}

	private bool BuildingCommand_Prog(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"You must either use {"clear".Colour(Telnet.Yellow)} to clear the current prog, or specify an ID or name of a prog to use.");
			return false;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("You clear the Wearable Prog from this component.");
			WearableProg = null;
			Changed = true;
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.Send("You may only use a prog that returns a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.Send(
				"The wearable prog you specify must be compatible with a single character and item parameter.");
			return false;
		}

		WearableProg = prog;
		Changed = true;
		actor.Send("This wearable will now use the {0} prog to determine who can wear it.",
			$"{WearableProg.FunctionName} (#{WearableProg.Id:N0})");
		actor.Send(
			$"Note: You must use the {"comp set failprog <id|name>".Colour(Telnet.Yellow)} command to set a prog to echo on failure to wear this item explaining why.");
		return true;
	}

	private bool BuildingCommand_Hidden(ICharacter actor, StringStack command)
	{
		DisplayInventoryWhenWorn = !DisplayInventoryWhenWorn;
		actor.OutputHandler.Send("This item will " + (DisplayInventoryWhenWorn ? "now" : "no longer") +
		                         " display in inventory when worn.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Default(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("Which wear profile do you wish to set as default?");
			return false;
		}

		IWearProfile profile = null;
		profile = long.TryParse(cmd, out var val)
			? Profiles.FirstOrDefault(x => x.Id == val)
			: Profiles.FirstOrDefault(x => x.Name.ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));

		if (profile == null)
		{
			actor.OutputHandler.Send("That is not a valid wear profile to set as default.");
			return false;
		}

		if (profile == DefaultProfile)
		{
			actor.OutputHandler.Send("That is already the default wear profile.");
			return false;
		}

		DefaultProfile = profile;
		actor.OutputHandler.Send("You set the default wear profile to " + profile.Name.Proper() + ".");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Add(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("Which wear profile do you wish to add?");
			return false;
		}

		IWearProfile profile = null;
		profile = long.TryParse(cmd, out var val)
			? Gameworld.WearProfiles.Get(val)
			: Gameworld.WearProfiles.FirstOrDefault(x => x.Name.ToLowerInvariant() == cmd);

		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such wear profile for you to add.");
			return false;
		}

		if (Profiles.Contains(profile))
		{
			actor.OutputHandler.Send("This component already contains that wear profile.");
			return false;
		}

		if (_profiles.Any())
		{
			DefaultProfile = profile;
		}

		_profiles.Add(profile);
		actor.OutputHandler.Send("You add the " + profile.Name.Proper() + " wear profile to this component.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Remove(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("Which wear profile do you wish to remove?");
			return false;
		}

		IWearProfile profile = null;
		profile = long.TryParse(cmd, out var val)
			? Profiles.FirstOrDefault(x => x.Id == val)
			: Profiles.FirstOrDefault(x => x.Name.ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));

		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such wear profile for you to remove.");
			return false;
		}

		_profiles.Remove(profile);
		if (DefaultProfile == profile)
		{
			DefaultProfile = _profiles.FirstOrDefault();
		}

		actor.OutputHandler.Send("You remove the " + profile.Name.Proper() + " wear profile from this component.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandLayerWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send(
				$"You must enter an amount of layer weight for this wearable to consume that is a number greater than zero.");
			return false;
		}

		LayerWeightConsumption = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This wearable will now consume {LayerWeightConsumption.ToString("N2", actor).ColourValue()} layer weight when worn.");
		return true;
	}

	#endregion
}