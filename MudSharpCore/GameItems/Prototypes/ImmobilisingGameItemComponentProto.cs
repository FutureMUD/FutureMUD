using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ImmobilisingGameItemComponentProto : WearableGameItemComponentProto
{
	public override string TypeDescription => "Immobilising";

	#region Constructors

	protected ImmobilisingGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "Immobilising")
	{
	}

	protected ImmobilisingGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImmobilisingGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImmobilisingGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Immobilising".ToLowerInvariant(), true,
			(gameworld, account) => new ImmobilisingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("sling", false,
			(gameworld, account) => new ImmobilisingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("cast", false,
			(gameworld, account) => new ImmobilisingGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Immobilising",
			(proto, gameworld) => new ImmobilisingGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Immobilising",
			$"Item is {"[wearable]".Colour(Telnet.BoldYellow)} and can be used to immobilise broken bones (e.g. splint, cast, etc)",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImmobilisingGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{2:N0}r{3:N0}, {4})\n\nThis item immobolises broken bones and can be worn by characters in the following profiles: {1}. This item {7} bulky. This item {5} hidden to others when worn. {6}",
			"Immobolising Item Component".Colour(Telnet.Cyan),
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
			Bulky ? "is" : "is not"
		);
	}
}