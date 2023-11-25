using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class RepairKitGameItemComponentProto : GameItemComponentProto
{
	public WoundSeverity MaximumSeverity { get; protected set; }
	public int RepairPoints { get; protected set; }
	public double CheckBonus { get; protected set; }

	private readonly List<IMaterial> _permittedMaterials = new();
	public IEnumerable<IMaterial> PermittedMaterials => _permittedMaterials;

	private readonly List<string> _echoes = new();
	public IEnumerable<string> Echoes => _echoes;

	private readonly List<DamageType> _damageTypes = new();
	public IEnumerable<DamageType> DamageTypes => _damageTypes;

	private readonly List<ITag> _tags = new();
	public IEnumerable<ITag> Tags => _tags;

	public ITraitDefinition CheckTrait { get; protected set; }

	public override string TypeDescription => "RepairKit";

	#region Constructors

	protected RepairKitGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"RepairKit")
	{
		MaximumSeverity = WoundSeverity.Horrifying;
		RepairPoints = 200;
		CheckBonus = 0.0;
		_damageTypes.AddRange(new[]
		{
			DamageType.Crushing,
			DamageType.Chopping,
			DamageType.Slashing,
			DamageType.Piercing,
			DamageType.ArmourPiercing,
			DamageType.Ballistic,
			DamageType.Bite,
			DamageType.Claw,
			DamageType.Shearing,
			DamageType.Shockwave
		});
	}

	protected RepairKitGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MaximumSeverity = (WoundSeverity)int.Parse(root.Element("MaximumSeverity").Value);
		RepairPoints = int.Parse(root.Element("RepairPoints").Value);
		CheckTrait = Gameworld.Traits.Get(long.Parse(root.Element("CheckTrait").Value));
		CheckBonus = double.Parse(root.Element("CheckBonus").Value);
		_echoes.AddRange(root.Element("Echoes").Elements().Select(x => x.Value));
		_permittedMaterials.AddRange(root.Element("Materials").Elements().SelectNotNull(x => Gameworld.Materials.Get(long.Parse(x.Value))));
		_damageTypes.AddRange(root.Element("DamageTypes").Elements().Select(x => (DamageType)int.Parse(x.Value)));
		_tags.AddRange(root.Element("Tags").Elements().Select(x => Gameworld.Tags.Get(long.Parse(x.Value))));
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumSeverity", (int)MaximumSeverity),
			new XElement("RepairPoints", RepairPoints),
			new XElement("CheckTrait", CheckTrait?.Id ?? 0),
			new XElement("CheckBonus", CheckBonus),
			new XElement("Echoes", from echo in _echoes select new XElement("Echo", new XCData(echo))),
			new XElement("Materials",
				from material in _permittedMaterials select new XElement("Material", material.Id)),
			new XElement("DamageTypes", from damage in _damageTypes select new XElement("DamageType", (int)damage)),
			new XElement("Tags", from tag in _tags select new XElement("Tag", tag.Id))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new RepairKitGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RepairKitGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("repairkit", true,
			(gameworld, account) => new RepairKitGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("repair kit", false,
			(gameworld, account) => new RepairKitGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("repair_kit", false,
			(gameworld, account) => new RepairKitGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("RepairKit",
			(proto, gameworld) => new RepairKitGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"RepairKit",
			$"Item can be used to repair damage to items and robots",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RepairKitGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tseverity <severity> - sets the maximum severity of damage that can be repaired\n\ttype <damage type> - toggles a damage type to be repairable\n\tpoints <amount> - sets how many repair points this kit has\n\tcheck <trait id|name> - sets the trait to test against\n\tbonus <amount> - sets the check bonus\n\tmaterial <material> - toggles a material this kit can repair\n\ttag <tag> - toggles requiring the item to have a specific tag\n\techo add <echo> - adds a new echo to the end\n\techo delete <#> - deletes the #th number\n\techo swap <#> <#> - swaps two echoes in position\n\techo edit <#> <echo> - overwrites the #th echo";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "severity":
			case "max severity":
			case "max wound":
			case "max damage":
			case "wound":
				return BuildingCommandSeverity(actor, command);
			case "damage":
			case "damagetype":
			case "damage type":
			case "dam type":
			case "type":
			case "types":
				return BuildingCommandDamageType(actor, command);
			case "points":
			case "quantity":
			case "repair_points":
			case "repair":
			case "repair points":
				return BuildingCommandRepairPoints(actor, command);
			case "bonus":
			case "check bonus":
				return BuildingCommandCheckBonus(actor, command);
			case "check":
			case "trait":
			case "skill":
				return BuildingCommandCheckTrait(actor, command);
			case "material":
			case "mat":
				return BuildingCommandMaterial(actor, command);
			case "tag":
			case "tags":
				return BuildingCommandTag(actor, command);
			case "echo":
				return BuildingCommandEcho(actor, command);

			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to toggle on or off?");
			return false;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		if (_tags.Contains(tag))
		{
			_tags.Remove(tag);
			actor.OutputHandler.Send(
				$"This repair kit will no longer repair items with the tag {tag.FullName.Colour(Telnet.Yellow)}.");
		}
		else
		{
			_tags.Add(tag);
			actor.OutputHandler.Send(
				$"This repair kit will now repair items with the tag {tag.FullName.Colour(Telnet.Yellow)}.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandDamageType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What damage type do you want to toggle on or off for this repair kit?");
			return false;
		}

		if (!WoundExtensions.TryGetDamageType(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid damage type.");
			return false;
		}

		if (_damageTypes.Contains(value))
		{
			_damageTypes.Remove(value);
			actor.OutputHandler.Send(
				$"This repair kit will no longer repair {value.Describe().Colour(Telnet.Cyan)} damage.");
		}
		else
		{
			_damageTypes.Add(value);
			actor.OutputHandler.Send($"This repair kit will now repair {value.Describe().Colour(Telnet.Cyan)} damage.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandRepairPoints(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"How many repair points should this kit have?\n{"Hint, each wound severity step takes 1 point, so repairs cost between 1-9 repair points each.".ColourCommand()}");
			return false;
		}

		if (value != -1 && value <= 0)
		{
			actor.OutputHandler.Send(
				"Repair kits can either have a positive amount of repair points, or -1 for infinite.");
			return false;
		}

		RepairPoints = value;
		Changed = true;
		actor.OutputHandler.Send($"This repair kit now has {RepairPoints.ToString("N0", actor)} repair points.");
		return true;
	}

	private bool BuildingCommandSeverity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What maximum wound severity should this kit work for?");
			return false;
		}

		if (!WoundExtensions.TryParseWoundSeverity(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid wound severity");
			return false;
		}

		MaximumSeverity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This repair kit will now only repair item damage up to a severity level of {MaximumSeverity.Describe().Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandCheckBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"How much of a bonus should this repair kit give to the repair check? Hint, every {StandardCheck.BonusesPerDifficultyLevel.ToString("N0", actor).ColourValue()} points of bonus is 1 difficulty stage down.");
			return false;
		}

		CheckBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"This repair kit now gives a bonus of {CheckBonus.ToString("N0", actor)}.");
		return true;
	}

	private bool BuildingCommandCheckTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or trait do you want the repair kit to make its tests against?");
			return false;
		}

		var trait = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.SafeRemainingArgument);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		CheckTrait = trait;
		Changed = true;
		actor.OutputHandler.Send(
			$"This repair kit now tests against the {CheckTrait.Name.Colour(Telnet.Green)} trait.");
		return true;
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a material to toggle.");
			return false;
		}

		var material = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Materials.Get(value)
			: Gameworld.Materials.GetByName(command.SafeRemainingArgument);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		if (_permittedMaterials.Contains(material))
		{
			_permittedMaterials.Remove(material);
			actor.OutputHandler.Send(
				$"This repair kit no longer repairs the {material.Name.Colour(Telnet.Green)} material.");
		}
		else
		{
			_permittedMaterials.Add(material);
			actor.OutputHandler.Send($"This repair kit now repairs the {material.Name.Colour(Telnet.Green)} material.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandEchoAdd(actor, command);
			case "del":
			case "delete":
			case "rem":
			case "remove":
				return BuildingCommandEchoDelete(actor, command);
			case "swap":
				return BuildingCommandEchoSwap(actor, command);
			case "edit":
			case "replace":
				return BuildingCommandEchoEdit(actor, command);
		}

		actor.OutputHandler.Send("Do you want to add, delete, swap or edit an echo?");
		return false;
	}

	private bool BuildingCommandEchoAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What echo do you want to add? Hint: Use $0 for the repairer, $1 for the repaired object, $2 for the kit.");
			return false;
		}

		var emote = new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceiver(),
			new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_echoes.Add(command.RemainingArgument);
		Changed = true;
		actor.OutputHandler.Send($"You add the following echo: {command.RemainingArgument.Colour(Telnet.Yellow)}");
		return true;
	}

	private bool BuildingCommandEchoDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which echo do you want to delete?");
			return false;
		}

		if (!_echoes.Any())
		{
			actor.OutputHandler.Send("There aren't any echoes to delete.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0 || value > _echoes.Count)
		{
			actor.OutputHandler.Send($"You must enter a value between 1 and {_echoes.Count.ToString("N0", actor)}.");
			return false;
		}

		_echoes.RemoveAt(value - 1);
		Changed = true;
		actor.OutputHandler.Send($"You remove the {value.ToOrdinal()} echo.");
		return true;
	}

	private bool BuildingCommandEchoSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which echo do you want to swap?");
			return false;
		}

		if (!_echoes.Any() || _echoes.Count == 1)
		{
			actor.OutputHandler.Send("There aren't enough echoes to swap.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0 || value > _echoes.Count)
		{
			actor.OutputHandler.Send($"You must enter a value between 1 and {_echoes.Count}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to swap that echo with?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value2) || value2 <= 0 || value2 > _echoes.Count)
		{
			actor.OutputHandler.Send($"You must enter a value between 1 and {_echoes.Count}.");
			return false;
		}

		if (value == value2)
		{
			actor.OutputHandler.Send("You can't swap an echo with itself.");
			return false;
		}

		_echoes.Swap(value - 1, value2 - 1);
		Changed = true;
		actor.OutputHandler.Send($"You swap the {value.ToOrdinal()} and {value2.ToOrdinal()} echoes.");
		return true;
	}

	private bool BuildingCommandEchoEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which echo do you want to edit?");
			return false;
		}

		if (!_echoes.Any())
		{
			actor.OutputHandler.Send("There aren't any echoes to edit.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0 || value > _echoes.Count)
		{
			actor.OutputHandler.Send($"You must enter a value between 1 and {_echoes.Count}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to change that echo to?");
			return false;
		}

		var emote = new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceiver(),
			new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_echoes[value - 1] = command.RemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the {value.ToOrdinal()} echo to the following echo: {command.RemainingArgument.Colour(Telnet.Yellow)}");
		return true;
	}

	public override bool CanSubmit()
	{
		return base.CanSubmit() && _echoes.Any() && _echoes.All(x =>
			new Emote(x, new DummyPerceiver(), new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable())
				.Valid);
	}

	public override string WhyCannotSubmit()
	{
		if (!_echoes.Any())
		{
			return "You must have at least one echo for using this repair kit.";
		}

		foreach (var emote in _echoes.Select(x => new Emote(x, new DummyPerceiver(), new DummyPerceiver(),
			         new DummyPerceivable(), new DummyPerceivable())))
		{
			if (!emote.Valid)
			{
				return $"Error with echo {emote.RawText.Colour(Telnet.Yellow)} -> {emote.ErrorMessage}";
			}
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is a repair kit with {4} points of usage. It can repair up to severity {5} damage. It uses the {6} trait and adds a bonus of {7} to repair checks. {8}. {9}. {10}.\n\nEchoes:{11}",
			"RepairKit Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RepairPoints.ToString("N0", actor),
			MaximumSeverity.Describe().Colour(Telnet.Green),
			CheckTrait?.Name.Colour(Telnet.Green) ?? "Not Set".Colour(Telnet.Red),
			CheckBonus.ToString("N2", actor)
			          .Colour(CheckBonus == 0.0 ? Telnet.Yellow : CheckBonus > 0.0 ? Telnet.Green : Telnet.Red),
			PermittedMaterials.Any()
				? $"It can only be used on items with the {_permittedMaterials.Select(x => x.Name.Colour(Telnet.Green)).ListToString(conjunction: "or ")} materials"
				: "It does not restrict usage to specific materials",
			!_damageTypes.Any()
				? "It can be used to repair any type of damage"
				: $"It can be used to repair the {_damageTypes.Select(x => x.Describe().Colour(Telnet.Green)).ListToString()} damage types",
			!_tags.Any()
				? "It does not require any specific tags"
				: $"It repairs items with the {_tags.Select(x => x.Name.Colour(Telnet.Yellow)).ListToString()} tags",
			Echoes.Select(x => x.Colour(Telnet.Yellow))
			      .ListToString(conjunction: "", twoItemJoiner: "", separator: "", article: "\n")
		);
	}
}