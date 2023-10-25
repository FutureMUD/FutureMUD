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

public class CoverGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Cover";

	public IRangedCover Cover { get; set; }

	public bool ProvideCoverByDefault {get; set; }

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Cover", Cover?.Id ?? 0), new XElement("ProvidesCoverByDefault", ProvideCoverByDefault)).ToString();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\n\nThis item provides the following cover {5}: {4}.",
			"Cover Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Cover?.Name ?? "None".Colour(Telnet.Red),
			ProvideCoverByDefault ? "by default" : "when activated"
		);
	}

	#region Constructors

	protected CoverGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Cover")
	{
		ProvideCoverByDefault = true;
	}

	protected CoverGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Cover = Gameworld.RangedCovers.Get(long.Parse(root.Element("Cover")?.Value ?? "0"));
		ProvideCoverByDefault = bool.Parse(root.Element("ProvidesCoverByDefault")?.Value ?? "true");
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CoverGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CoverGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Cover".ToLowerInvariant(), true,
			(gameworld, account) => new CoverGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Cover", (proto, gameworld) => new CoverGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Cover",
			$"Makes an item provide {"[ranged cover]".Colour(Telnet.Cyan)} when in the room",
			$@"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3default#0 - toggles whether it is on by default
	#3cover <type>#0 - sets the cover type this object provides. See {"show covers".FluentTagMXP("send", "href='show covers'")} for a list."
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new CoverGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	public override string ShowBuildingHelp =>
		$@"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3default#0 - toggles whether it is on by default
	#3cover <type>#0 - sets the cover type this object provides. See {"show covers".FluentTagMXP("send", "href='show covers'")} for a list.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "cover":
			case "type":
				return BuildingCommandCover(actor, command);
			case "default":
				return BuildingCommandDefault(actor);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandDefault(ICharacter actor)
	{
		ProvideCoverByDefault = !ProvideCoverByDefault;
		Changed = true;
		actor.OutputHandler.Send($"This item will {ProvideCoverByDefault.NowNoLonger()} provide cover by default when first loaded.");
		return true;
	}

	protected bool BuildingCommandCover(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What cover should this item provide? See SHOW COVERS for a list.");
			return false;
		}

		var cover = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.RangedCovers.Get(value)
			: Gameworld.RangedCovers.GetByName(command.SafeRemainingArgument) ??
			  Gameworld.RangedCovers.FirstOrDefault(
				  x => x.Name.StartsWith(command.SafeRemainingArgument, StringComparison.InvariantCultureIgnoreCase));
		if (cover == null)
		{
			actor.Send("There is no such ranged cover to set for this item. See SHOW COVERS for a list.");
			return false;
		}

		Cover = cover;
		Changed = true;
		actor.Send($"This item will now provide the {Cover.Name.TitleCase().Colour(Telnet.Green)} cover.");
		return true;
	}

	public override bool CanSubmit()
	{
		return Cover != null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		return Cover == null ? "You must first select a ranged cover for this item." : base.WhyCannotSubmit();
	}

	#endregion
}