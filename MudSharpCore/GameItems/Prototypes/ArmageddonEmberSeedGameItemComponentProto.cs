#nullable enable

using MudSharp.Accounts;
using System.Globalization;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public sealed class ArmageddonEmberSeedGameItemComponentProto : GameItemComponentProto, IArmageddonEmberSeedComponentPrototype
{
	public override string TypeDescription => "Armageddon Ember Seed";
	public double DamagePerGrade { get; private set; } = 3.0;

	private ArmageddonEmberSeedGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ArmageddonEmberSeed")
	{
		Changed = true;
	}

	private ArmageddonEmberSeedGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		var damage = (double?)root.Attribute("damagePerGrade") ?? 0.0;
		DamagePerGrade = double.IsFinite(damage) && damage is > 0.0 and <= 100.0 ? damage : 0.0;
	}

	protected override string SaveToXml() => new XElement("Definition", new XAttribute("damagePerGrade", DamagePerGrade)).ToString();

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"Ember Seed one-shot binding: {DamagePerGrade.ToString("N1", actor).ColourValue()} burning damage per bound invocation grade per eligible target. Grade, creator and deadline come from the live spell-created item binding.";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("armageddonemberseed", true,
			(gameworld, account) => new ArmageddonEmberSeedGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ArmageddonEmberSeed",
			(proto, gameworld) => new ArmageddonEmberSeedGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("ArmageddonEmberSeed",
			"A spell-created one-shot bounded Ember Seed blast item.",
			"Use `damagepergrade <positive amount up to 100>` to configure bounded native blast damage.");
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch() is not "damagepergrade") return base.BuildingCommand(actor, command.GetUndo());
		if (double.TryParse(command.PopSpeech(), NumberStyles.Float, actor, out var damage) && double.IsFinite(damage) && damage is > 0.0 and <= 100.0)
		{
			DamagePerGrade = damage;
			Changed = true;
			actor.Send($"Ember Seed damage per grade set to {damage.ToString("N1", actor).ColourValue()}.");
			return true;
		}

		actor.Send("Use damagepergrade <positive amount up to 100>.");
		return false;
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new ArmageddonEmberSeedGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new ArmageddonEmberSeedGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new ArmageddonEmberSeedGameItemComponentProto(proto, gameworld));
}
