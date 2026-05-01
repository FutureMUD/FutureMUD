#nullable enable
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace $rootnamespace$.Prototypes;

// If the runtime component implements a public capability interface, add the matching
// I...Prototype marker here so item prototypes can enforce exclusive component roles.
public class $safeitemrootname$GameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "$safeitemrootname$";

	protected $safeitemrootname$GameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "$safeitemrootname$")
	{
	}

	protected $safeitemrootname$GameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("$safeitemrootname$".ToLowerInvariant(), true,
			(gameworld, account) => new $safeitemrootname$GameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("$safeitemrootname$",
			(proto, gameworld) => new $safeitemrootname$GameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"$safeitemrootname$",
			"A short description of what this item component does.",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new $safeitemrootname$GameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new $safeitemrootname$GameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new $safeitemrootname$GameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item component needs a specific description.",
			"$safeitemrootname$ Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		// Load builder-authored definition data here.
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
