#nullable enable
using System.Globalization;
using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;

namespace MudSharp.GameItems.Prototypes;

public sealed class EquipmentBonusesGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Equipment Bonuses";
	public bool WorksWorn { get; private set; } = true;
	public bool WorksWielded { get; private set; } = true;
	public bool WorksHeld { get; private set; }
	public HashSet<long> ExcludedProfiles { get; } = [];
	public List<EquippedItemBonus> Bonuses { get; } = [];

	private EquipmentBonusesGameItemComponentProto(IFuturemud world, IAccount account) : base(world, account, "Equipment Bonuses") { }
	private EquipmentBonusesGameItemComponentProto(MudSharp.Models.GameItemComponentProto model, IFuturemud world) : base(model, world) { }
	protected override void LoadFromXml(XElement root)
	{
		WorksWorn = (bool?)root.Element("Worn") ?? true;
		WorksWielded = (bool?)root.Element("Wielded") ?? true;
		WorksHeld = (bool?)root.Element("Held") ?? false;
		foreach (var profile in root.Element("ExcludedProfiles")?.Elements("Profile") ?? []) ExcludedProfiles.Add((long)profile);
		foreach (var bonus in root.Elements("Bonus"))
			Bonuses.Add(new(Enum.Parse<EquippedItemBonusKind>((string)bonus.Attribute("kind")!, true),
				(long?)bonus.Attribute("target") ?? 0, double.Parse((string)bonus.Attribute("value")!, CultureInfo.InvariantCulture)));
	}
	protected override string SaveToXml() => new XElement("Definition", new XElement("Worn", WorksWorn),
		new XElement("Wielded", WorksWielded), new XElement("Held", WorksHeld),
		new XElement("ExcludedProfiles", ExcludedProfiles.Select(id => new XElement("Profile", id))),
		Bonuses.Select(x => new XElement("Bonus", new XAttribute("kind", x.Kind), new XAttribute("target", x.TargetId), new XAttribute("value", x.Bonus)))).ToString();
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("equipmentbonuses", true, (world, account) => new EquipmentBonusesGameItemComponentProto(world, account));
		manager.AddDatabaseLoader("Equipment Bonuses", (model, world) => new EquipmentBonusesGameItemComponentProto(model, world));
		manager.AddTypeHelpInfo("EquipmentBonuses", "Intrinsic, additive equipment trait, capacity, combat, resistance and absorption modifiers.", Help);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new EquipmentBonusesGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent model, IGameItem parent) => new EquipmentBonusesGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter actor) => CreateNewRevision(actor, (model, world) => new EquipmentBonusesGameItemComponentProto(model, world));
	private const string Help = "worn|wielded|held - toggle activation (held excludes abstract inventory slots); exclude <wear profile ID> - toggle a worn profile exclusion; bonus <Trait|Health|Stun|Stamina|MagicResource|CombatAttack|CombatDefence|SpellResistance|PhysicalAbsorption> <target ID or 0> <signed amount> - set a modifier (zero removes it).";
	public override string ShowBuildingHelp => Help;
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "worn": WorksWorn = !WorksWorn; break;
			case "wielded": WorksWielded = !WorksWielded; break;
			case "held": WorksHeld = !WorksHeld; break;
			case "exclude":
				if (!long.TryParse(command.PopSpeech(), out var profile) || Gameworld.WearProfiles.Get(profile) is null)
				{ actor.Send("Specify an existing wear profile ID."); return false; }
				if (!ExcludedProfiles.Remove(profile)) ExcludedProfiles.Add(profile);
				break;
			case "bonus":
				if (!Enum.TryParse<EquippedItemBonusKind>(command.PopSpeech(), true, out var kind) || !Enum.IsDefined(kind) ||
				    !long.TryParse(command.PopSpeech(), out var target) || !double.TryParse(command.PopSpeech(), NumberStyles.Float, actor, out var value) || !ValidBonus(new(kind, target, value)))
				{ actor.Send(Help); return false; }
				Bonuses.RemoveAll(x => x.Kind == kind && x.TargetId == target);
				if (value != 0) Bonuses.Add(new(kind, target, value));
				break;
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
		Changed = true;
		actor.Send(ComponentDescriptionOLC(actor));
		return true;
	}
	private bool ValidBonus(EquippedItemBonus bonus) => double.IsFinite(bonus.Bonus) && Enum.IsDefined(bonus.Kind) &&
		(bonus.Kind != EquippedItemBonusKind.PhysicalAbsorption || bonus.Bonus >= 0) &&
		(bonus.Kind switch
		{
			EquippedItemBonusKind.Trait => bonus.TargetId > 0 && Gameworld.Traits.Get(bonus.TargetId) is not null,
			EquippedItemBonusKind.MagicResource => bonus.TargetId > 0 && Gameworld.MagicResources.Get(bonus.TargetId) is not null,
			_ => bonus.TargetId == 0
		});
	public override bool CanSubmit() => Bonuses.Count > 0 && Bonuses.All(ValidBonus) && ExcludedProfiles.All(id => Gameworld.WearProfiles.Get(id) is not null);
	public override string WhyCannotSubmit() => CanSubmit() ? string.Empty : "Supply valid finite bonuses and existing trait/resource/profile references.";
	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"Equipment Bonuses #{Id.ToString("N0", actor)}: worn {WorksWorn.ToColouredString()}, wielded {WorksWielded.ToColouredString()}, held {WorksHeld.ToColouredString()}. Excluded profiles: {ExcludedProfiles.Select(x => x.ToString("N0", actor)).ListToCommaSeparatedValues()}.\n" +
		string.Join("\n", Bonuses.Select(x => $"{x.Kind.DescribeEnum().ColourName()} #{x.TargetId.ToString("N0", actor)}: {x.Bonus.ToBonusString(actor)}"));
}
