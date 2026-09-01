using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class ArtilleryAmmunitionGameItemComponentProto : AmmunitionGameItemComponentProto, IArtilleryAmmunitionPrototype
{
	public override string TypeDescription => "Artillery Ammunition";
	public ArtilleryPayloadType PayloadType { get; private set; }
	public string ArtilleryProfile { get; private set; } = "general";

	protected ArtilleryAmmunitionGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ArtilleryAmmunition")
	{
	}

	protected ArtilleryAmmunitionGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PayloadType = root.Element("PayloadType")?.Value.TryParseEnum<ArtilleryPayloadType>(out var payload) == true
			? payload
			: ArtilleryPayloadType.SolidShot;
		ArtilleryProfile = root.Element("ArtilleryProfile")?.Value ?? "general";
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(new XElement("PayloadType", PayloadType), new XElement("ArtilleryProfile", ArtilleryProfile));
		return root.ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ArtilleryAmmunitionGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) => new ArtilleryAmmunitionGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (proto, gameworld) => new ArtilleryAmmunitionGameItemComponentProto(proto, gameworld));

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("artilleryammo", true, (gameworld, account) => new ArtilleryAmmunitionGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ArtilleryAmmunition", (proto, gameworld) => new ArtilleryAmmunitionGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("ArtilleryAmmunition", "Makes an item ammunition for a crew-served artillery piece", "Options: ammo <type>, bullet <proto>, payload <type>, profile <name>.");
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var keyword = command.PopSpeech().ToLowerInvariant();
		switch (keyword)
		{
			case "payload":
				if (!command.PopSpeech().TryParseEnum<ArtilleryPayloadType>(out var payload)) return false;
				PayloadType = payload;
				Changed = true;
				return true;
			case "profile":
				ArtilleryProfile = command.SafeRemainingArgument.ToLowerInvariant();
				Changed = !string.IsNullOrWhiteSpace(ArtilleryProfile);
				return Changed;
			default:
				return base.BuildingCommand(actor, new StringStack($"{keyword} {command.SafeRemainingArgument}"));
		}
	}
}
