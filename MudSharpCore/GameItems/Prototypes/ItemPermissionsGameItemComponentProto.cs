#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Prototypes;

/// <summary>Opt-in ordinary interaction restrictions without changing holdability or scripted creation.</summary>
public sealed class ItemPermissionsGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Item Permissions";
	public ItemInteractionRestriction Restrictions { get; private set; }
	public long PickupAttemptProgId { get; private set; }
	public bool PickupAttemptOnceOnly { get; private set; }
	private ItemPermissionsGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "Item Permissions") { }
	private ItemPermissionsGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud world) : base(proto, world) { }
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("itempermissions", true, (world, account) => new ItemPermissionsGameItemComponentProto(world, account));
		manager.AddDatabaseLoader("Item Permissions", (proto, world) => new ItemPermissionsGameItemComponentProto(proto, world));
		manager.AddTypeHelpInfo("ItemPermissions", "Restricts pickup, dropping, giving, putting, sheathing or ordinary room listings.", Help);
	}
	private const string Help = "Use restrict <pickup|drop|give|put|sheathe|roomlisting|onehandedwield> to toggle a restriction. Use attemptprog <boolean(character,item) prog|none> for actual pickup attempts and attemptonce for a saved one-shot callback. True continues pickup; false aborts. Holdable and forced/scripted placement are unaffected.";
	public override string ShowBuildingHelp => Help;
	protected override void LoadFromXml(XElement root)
	{
		Restrictions = (ItemInteractionRestriction)((int?)root.Element("Restrictions") ?? 0);
		PickupAttemptProgId = (long?)root.Element("PickupAttemptProg") ?? 0;
		PickupAttemptOnceOnly = (bool?)root.Element("PickupAttemptOnceOnly") ?? false;
		if ((Restrictions & ~AllRestrictions) != 0) throw new FormatException("Unknown item interaction restriction.");
	}
	internal const ItemInteractionRestriction AllRestrictions = ItemInteractionRestriction.Pickup | ItemInteractionRestriction.Drop |
		ItemInteractionRestriction.Give | ItemInteractionRestriction.Put | ItemInteractionRestriction.Sheathe | ItemInteractionRestriction.RoomListing | ItemInteractionRestriction.OneHandedWield;
	protected override string SaveToXml() => new XElement("Definition", new XElement("Restrictions", (int)Restrictions),
		new XElement("PickupAttemptProg", PickupAttemptProgId), new XElement("PickupAttemptOnceOnly", PickupAttemptOnceOnly)).ToString();
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ItemPermissionsGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) => new ItemPermissionsGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (proto, world) => new ItemPermissionsGameItemComponentProto(proto, world));
	public override string ComponentDescriptionOLC(ICharacter actor) => $"Item permissions #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}\nRestricted: {Restrictions.DescribeEnum().ColourValue()}.";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("attemptonce"))
		{ command.PopSpeech(); PickupAttemptOnceOnly = !PickupAttemptOnceOnly; Changed = true; actor.Send($"One-shot pickup callback: {PickupAttemptOnceOnly.ToColouredString()}."); return true; }
		if (command.PeekSpeech().EqualTo("attemptprog"))
		{
			command.PopSpeech(); var text = command.SafeRemainingArgument;
			var prog = text.EqualTo("none") ? null : Gameworld.FutureProgs.GetByIdOrName(text);
			if (!text.EqualTo("none") && (prog is null || prog.ReturnType != MudSharp.FutureProg.ProgVariableTypes.Boolean ||
				!prog.MatchesParameters([MudSharp.FutureProg.ProgVariableTypes.Character, MudSharp.FutureProg.ProgVariableTypes.Item])))
			{ actor.Send("Specify a boolean(character,item) prog or none. True continues pickup; false aborts it."); return false; }
			PickupAttemptProgId = prog?.Id ?? 0; Changed = true; actor.Send("Pickup-attempt callback updated."); return true;
		}
		if (!command.PopSpeech().EqualTo("restrict")) return base.BuildingCommand(actor, command.GetUndo());
		if (!Enum.TryParse<ItemInteractionRestriction>(command.PopSpeech(), true, out var option) || option == ItemInteractionRestriction.None ||
			!Enum.IsDefined(option) || !command.IsFinished) { actor.Send(Help); return false; }
		Restrictions ^= option; Changed = true;
		actor.Send($"Restricted interactions: {Restrictions.DescribeEnum().ColourValue()}."); return true;
	}
}
