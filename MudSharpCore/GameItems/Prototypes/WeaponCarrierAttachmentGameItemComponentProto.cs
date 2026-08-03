using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class WeaponCarrierAttachmentGameItemComponentProto : GameItemComponentProto, IWeaponCarrierAttachmentPrototype
{
	public WeaponCarrierAttachmentGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "WeaponCarrierAttachment")
	{
	}

	protected WeaponCarrierAttachmentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Weapon Carrier Attachment";
	public string CompatibleProfile { get; private set; } = "any";
	public string CompatibleWeaponType { get; private set; } = "any";
	public string CompatibleTags { get; private set; } = string.Empty;
	public SizeCategory MaximumWeaponSize { get; private set; } = SizeCategory.Gigantic;
	public string AttachmentPoint { get; private set; } = "worn-or-held";
	public bool RetainsDroppedWeapon { get; private set; } = true;

	protected override void LoadFromXml(XElement root)
	{
		CompatibleProfile = root.Element("CompatibleProfile")?.Value ?? "any";
		CompatibleWeaponType = root.Element("CompatibleWeaponType")?.Value ?? "any";
		CompatibleTags = root.Element("CompatibleTags")?.Value ?? string.Empty;
		MaximumWeaponSize = root.Element("MaximumWeaponSize")?.Value.TryParseEnum<SizeCategory>(out var size) == true
			? size : SizeCategory.Gigantic;
		AttachmentPoint = root.Element("AttachmentPoint")?.Value ?? "worn-or-held";
		RetainsDroppedWeapon = (bool?)root.Element("RetainsDroppedWeapon") ?? true;
	}

	protected override string SaveToXml() => new XElement("Definition",
		new XElement("CompatibleProfile", new XCData(CompatibleProfile)),
		new XElement("CompatibleWeaponType", new XCData(CompatibleWeaponType)),
		new XElement("CompatibleTags", new XCData(CompatibleTags)),
		new XElement("MaximumWeaponSize", MaximumWeaponSize),
		new XElement("AttachmentPoint", AttachmentPoint),
		new XElement("RetainsDroppedWeapon", RetainsDroppedWeapon)).ToString();

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new WeaponCarrierAttachmentGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new WeaponCarrierAttachmentGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new WeaponCarrierAttachmentGameItemComponentProto(proto, gameworld));

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("weaponcarrier", true, (gameworld, account) => new WeaponCarrierAttachmentGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("weaponcarrierattachment", false, (gameworld, account) => new WeaponCarrierAttachmentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("WeaponCarrierAttachment", (proto, gameworld) => new WeaponCarrierAttachmentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("WeaponCarrierAttachment", "Makes an item an attachable weapon sling, loop, or lanyard", BuildingHelpText);
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3profile <any|pistol|carbine|longgun>#0 - sets the compatible weapon family
	#3weapontype <any|musket|crossbow|...>#0 - sets the compatible ranged-weapon class
	#3tags <tag[, tag ...]>#0 - requires one of the named item tags (or #3none#0)
	#3maxsize <size>#0 - sets the largest compatible weapon size
	#3point <worn-or-held|worn|held>#0 - sets where the carrier must be used
	#3retain#0 - toggles retention of a dropped attached weapon.";
	public override string ShowBuildingHelp => BuildingHelpText;
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "profile":
				if (command.IsFinished)
				{
					actor.Send("Which weapon family should this carrier accept?");
					return false;
				}
				CompatibleProfile = command.SafeRemainingArgument.ToLowerInvariant();
				Changed = true;
				return true;
			case "retain":
				RetainsDroppedWeapon = !RetainsDroppedWeapon;
				Changed = true;
				return true;
			case "weapontype":
				CompatibleWeaponType = command.SafeRemainingArgument.IfNullOrWhiteSpace("any").ToLowerInvariant();
				Changed = true;
				return true;
			case "tags":
				CompatibleTags = command.SafeRemainingArgument.EqualTo("none") ? string.Empty : command.SafeRemainingArgument;
				Changed = true;
				return true;
			case "maxsize":
				if (!command.PopSpeech().TryParseEnum<SizeCategory>(out var size)) return false;
				MaximumWeaponSize = size;
				Changed = true;
				return true;
			case "point":
				var point = command.PopForSwitch();
				if (!point.EqualToAny("worn-or-held", "worn", "held")) return false;
				AttachmentPoint = point;
				Changed = true;
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{Name.ColourName()} carries {CompatibleProfile.ColourValue()} / {CompatibleWeaponType.ColourValue()} weapons up to {MaximumWeaponSize.DescribeEnum().ColourValue()} size at {AttachmentPoint.ColourValue()} and {(RetainsDroppedWeapon ? "retains" : "does not retain").ColourValue()} dropped weapons.";
}
