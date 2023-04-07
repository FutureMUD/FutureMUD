using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class AmmoClipGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "AmmoClip";

	public string ClipType { get; set; }

	public string SpecificAmmoGrade { get; set; }

	public int Capacity { get; set; }

	#region Constructors

	protected AmmoClipGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"AmmoClip")
	{
		Capacity = Gameworld.GetStaticInt("DefaultGunClipCapacity");
		ClipType = Gameworld.GetStaticConfiguration("DefaultGunClipType");
		SpecificAmmoGrade = Gameworld.GetStaticConfiguration("DefaultAmmoGrade");
	}

	protected AmmoClipGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		ClipType = root.Element("ClipType").Value;
		SpecificAmmoGrade = root.Element("SpecificAmmoGrade").Value;
		Capacity = int.Parse(root.Element("Capacity").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ClipType", new XCData(ClipType)),
			new XElement("SpecificAmmoGrade", new XCData(SpecificAmmoGrade)),
			new XElement("Capacity", Capacity)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new AmmoClipGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new AmmoClipGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("AmmoClip".ToLowerInvariant(), true,
			(gameworld, account) => new AmmoClipGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("AmmoClip",
			(proto, gameworld) => new AmmoClipGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"AmmoClip",
			$"A type of {"[container]".Colour(Telnet.BoldGreen)} used to store rounds for guns",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tcapacity <number> - sets the maximum number of rounds in this ammo clip.\n\ttype <type> - sets the form factor of the ammo clip to the specified type, for example {"Glock 9mm".Colour(Telnet.Green)}. This must be matched by the firearm that uses it.\n\tammo <ammo>- sets the ammo grade that this ammo clip uses, for example, {"9x19mm Parabellum".Colour(Telnet.Green)}."
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new AmmoClipGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	public override string ShowBuildingHelp =>
		$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tcapacity <number> - sets the maximum number of rounds in this ammo clip.\n\ttype <type> - sets the form factor of the ammo clip to the specified type, for example {"Glock 9mm".Colour(Telnet.Green)}. This must be matched by the firearm that uses it.\n\tammo <ammo>- sets the ammo grade that this ammo clip uses, for example, {"9x19mm Parabellum".Colour(Telnet.Green)}.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "capacity":
			case "rounds":
				return BuildingCommandCapacity(actor, command);
			case "type":
			case "clip":
			case "cliptype":
			case "clip type":
			case "form":
			case "form factor":
			case "formfactor":
				return BuildingCommandClipType(actor, command);
			case "ammo":
			case "grade":
			case "ammo type":
			case "ammo grade":
			case "ammotype":
			case "ammograde":
				return BuildingCommandSpecificAmmoGrade(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many rounds should this clip hold?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.Send("You must enter a valid number of rounds for this ammo holder's capacity.");
			return false;
		}

		Capacity = value;
		Changed = true;
		actor.Send($"This ammo clip will now hold {Capacity:N0} rounds of ammunition.");
		return true;
	}

	private bool BuildingCommandClipType(ICharacter actor, StringStack command)
	{
		var types = Gameworld.ItemProtos.SelectNotNull(x => x.GetItemType<AmmoClipGameItemComponentProto>())
		                     .Where(x => x.Status == RevisionStatus.Current).Select(x => x.ClipType).Distinct()
		                     .OrderBy(x => x).ToList();
		if (command.IsFinished)
		{
			actor.Send("What clip type (form factor) should this ammo holder have?");
			if (types.Any())
			{
				actor.Send(
					$"Hint: the following form factors exist currently:\n{types.ListToString("\t", "\n", twoItemJoiner: "\n", conjunction: "")}");
			}

			return false;
		}

		var type = command.SafeRemainingArgument;
		ClipType = types.FirstOrDefault(x => x.EqualTo(type)) ?? type;
		Changed = true;
		actor.Send($"This ammo holder will now have a clip type (form factor) of {ClipType}.");
		if (types.All(x => !x.EqualTo(ClipType)))
		{
			actor.Send(
				"Warning: There have not been any clips of this type before. Check to see if the name is a typo.");
		}

		return true;
	}

	private bool BuildingCommandSpecificAmmoGrade(ICharacter actor, StringStack command)
	{
		var types = Gameworld.AmmunitionTypes.Where(x => x.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm))
		                     .Select(x => x.SpecificType).Distinct().OrderBy(x => x).ToList();
		if (command.IsFinished)
		{
			actor.Send("What ammunition type should this ammo holder take?");
			if (types.Any())
			{
				actor.Send(
					$"Hint: the following ammunition types exist currently:\n{types.ListToString("\t", "\n", twoItemJoiner: "\n", conjunction: "")}");
			}

			return false;
		}

		var type = command.SafeRemainingArgument;
		SpecificAmmoGrade = types.FirstOrDefault(x => x.EqualTo(type)) ?? type;
		Changed = true;
		actor.Send($"This ammo holder will now take rounds of type {SpecificAmmoGrade}.");
		if (types.All(x => !x.EqualTo(SpecificAmmoGrade)))
		{
			actor.Send("Warning: There are no ammunition types with that grade. Check to see if the name is a typo.");
		}

		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is an ammo clip of type {4} for ammo grade {5} with capacity for {6} rounds.",
			"AmmoClip Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			ClipType.Colour(Telnet.Cyan),
			SpecificAmmoGrade.Colour(Telnet.Cyan),
			Capacity.ToString("N0", actor)
		);
	}
}