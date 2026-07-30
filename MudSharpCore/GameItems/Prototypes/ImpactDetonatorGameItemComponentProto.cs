using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class ImpactDetonatorGameItemComponentProto : GameItemComponentProto, IImpactDetonatorPrototype,
	IGameItemComponentPrototypeRequirementProvider
{
	private static readonly IReadOnlyCollection<GameItemComponentPrototypeRequirement> Requirements =
	[
		new(typeof(IDetonatable), "it needs an explosive payload to detonate after impact")
	];

	protected ImpactDetonatorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ImpactDetonator")
	{
	}

	protected ImpactDetonatorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "ImpactDetonator";
	public IReadOnlyCollection<GameItemComponentPrototypeRequirement> RequiredSiblingComponents => Requirements;

	protected override void LoadFromXml(XElement root)
	{
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ImpactDetonatorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImpactDetonatorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImpactDetonatorGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("impactdetonator", true,
			(gameworld, account) => new ImpactDetonatorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("impact detonator", false,
			(gameworld, account) => new ImpactDetonatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImpactDetonator",
			(proto, gameworld) => new ImpactDetonatorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("ImpactDetonator",
			"Detonates a sibling Bomb component after this item resolves as a fired ammunition projectile",
			BuildingHelpText);
	}

	private const string BuildingHelpText = @"This component has no type-specific options.

Combine it with a #3Bomb#0 component on an ammunition bullet prototype. When that projectile is fired and resolves an impact, miss, obstruction, cover strike, or scatter landing, it detonates at its resolved location.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Impact Detonator Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis component detonates a sibling Bomb component after a fired ammunition projectile resolves its impact.";
	}
}
