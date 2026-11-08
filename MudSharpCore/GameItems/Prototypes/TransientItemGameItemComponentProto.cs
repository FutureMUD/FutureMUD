#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Prototypes;

public sealed class TransientItemGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Transient Item";
	public long RetirementPlaneId { get; private set; }
	public ItemRetirementTrigger Triggers { get; private set; }
	public bool RetireOnReload { get; private set; } = true;
	public bool RetireOnLogout { get; private set; } = true;
	private TransientItemGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "Transient Item") { }
	private TransientItemGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud world) : base(proto, world) { }
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("transientitem", true, (world, account) => new TransientItemGameItemComponentProto(world, account));
		manager.AddDatabaseLoader("Transient Item", (proto, world) => new TransientItemGameItemComponentProto(proto, world));
		manager.AddTypeHelpInfo("TransientItem", "Retires an item on database reload or inventory logout rather than after a timer.", Help);
	}
	private const string Help = "Use reload or logout to toggle retirement on those lifecycle boundaries. Use trigger <BulkDrop|EtherealTransition|TransientBodyDeath|HostBodyChange> to toggle explicit disposal events. Use plane <name|id|any> to restrict the ethereal-transition trigger. New items are not retired on initial creation.";
	public override string ShowBuildingHelp => Help;
	protected override void LoadFromXml(XElement root) { RetirementPlaneId = (long?)root.Element("RetirementPlane") ?? 0; Triggers = (ItemRetirementTrigger)((int?)root.Element("Triggers") ?? 0); RetireOnReload = (bool?)root.Element("RetireOnReload") ?? true; RetireOnLogout = (bool?)root.Element("RetireOnLogout") ?? true; }
	protected override string SaveToXml() => new XElement("Definition", new XElement("RetirementPlane", RetirementPlaneId), new XElement("Triggers", (int)Triggers), new XElement("RetireOnReload", RetireOnReload), new XElement("RetireOnLogout", RetireOnLogout)).ToString();
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new TransientItemGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) => new TransientItemGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (proto, world) => new TransientItemGameItemComponentProto(proto, world));
	public override string ComponentDescriptionOLC(ICharacter actor) => $"Transient item: retire on reload {RetireOnReload.ToColouredString()}, inventory logout {RetireOnLogout.ToColouredString()}, explicit events {Triggers.DescribeEnum().ColourValue()}.";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "plane":
				var text = command.SafeRemainingArgument;
				var plane = text.EqualTo("any") ? null : Gameworld.Planes.GetByIdOrName(text);
				if (plane is null && !text.EqualTo("any")) { actor.Send("Choose an existing plane or any."); return false; }
				RetirementPlaneId = plane?.Id ?? 0; break;
			case "trigger":
				if (!Enum.TryParse<ItemRetirementTrigger>(command.PopSpeech(), true, out var trigger) || trigger == ItemRetirementTrigger.None || !Enum.IsDefined(trigger))
				{ actor.Send("Choose BulkDrop, EtherealTransition, TransientBodyDeath or HostBodyChange."); return false; }
				Triggers ^= trigger; break;
			case "reload": RetireOnReload = !RetireOnReload; break;
			case "logout": RetireOnLogout = !RetireOnLogout; break;
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
		Changed = true; actor.Send(ComponentDescriptionOLC(actor)); return true;
	}
}
