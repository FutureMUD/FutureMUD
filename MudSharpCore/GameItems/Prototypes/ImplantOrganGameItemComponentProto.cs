using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ImplantOrganGameItemComponentProto : ImplantBaseGameItemComponentProto
{
	public IOrganProto TargetOrgan { get; protected set; }

	public override string TypeDescription => "ImplantOrgan";

	#region Constructors

	protected ImplantOrganGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantOrgan")
	{
	}

	protected ImplantOrganGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		TargetOrgan =
			TargetBody?.Organs.FirstOrDefault(x => x.Id == long.Parse(root.Element("TargetOrgan")?.Value ?? "0"));
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		var implant = SaveToXmlWithoutConvertingToString();
		implant.Add(new XElement("TargetOrgan", TargetOrgan?.Id ?? 0));
		return implant.ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantOrganGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantOrganGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantOrgan".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantOrganGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantOrgan",
			(proto, gameworld) => new ImplantOrganGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantOrgan",
			$"A basic {"[implant]".Colour(Telnet.Pink)} that functions as an artificial organ when {"[powered]".Colour(Telnet.Magenta)} and installed",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantOrganGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tpower <watts> - how many watts of power to use\n\tdiscount <watts> - how many watts of power usage to discount per point of quality\n\tgrace <percentage> - the grace percentage of hp damage before implant function reduces\n\torgan <organ> - sets the organ that this implant replaces\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficulty it is for surgeons to install this implant";

	public override string ShowBuildingHelp => BuildingHelpText;

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		if (TargetOrgan == null)
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (TargetOrgan == null)
		{
			return "You must first set a target organ.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "organ":
				return BuildingCommandOrgan(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandOrgan(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which organ should this implant provide the function of?");
			return false;
		}

		if (TargetBody == null)
		{
			actor.Send("You must first set a body for this implant.");
			return false;
		}

		var organName = command.SafeRemainingArgument;
		var organ = long.TryParse(organName, out var value)
			? TargetBody.Organs.FirstOrDefault(x => x.Id == value)
			: TargetBody.Organs.FirstOrDefault(x =>
				  x.FullDescription().EqualTo(organName) || x.Name.EqualTo(organName)) ??
			  TargetBodypart?.Organs.FirstOrDefault(x => x.Name.EqualTo(organName) || x.Name.EqualTo(organName));
		if (organ == null)
		{
			actor.Send("That body has no such organ.");
			return false;
		}

		TargetOrgan = organ;
		Changed = true;
		actor.OutputHandler.Send($"This implant is now functionally {organ.FullDescription().A_An_RespectPlurals()}.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return ComponentDescriptionOLC(actor,
			$"This is an implant version of the {TargetOrgan?.FullDescription() ?? "unknown"} organ", string.Empty);
	}
}