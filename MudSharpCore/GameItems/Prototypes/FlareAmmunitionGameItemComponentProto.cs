using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class FlareAmmunitionGameItemComponentProto : AmmunitionGameItemComponentProto
{
	public override string TypeDescription => "FlareAmmunition";

	#region Constructors

	protected FlareAmmunitionGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "FlareAmmunition")
	{
		FlareDuration = TimeSpan.FromSeconds(240);
		FlareIllumination = 1000;
		FlareZoneDescription = "A bright white burning flare high in the air showers the area in light";
		FlareZoneDescriptionColour = Telnet.BoldWhite;
		FlareBeginEmote = "A bright white flare shoots up into the sky, showering the area in light.";
		FlareEndEmote = "The bright white flare in the sky finally burns out.";
		AmmoType = Gameworld.AmmunitionTypes.Get(Gameworld.GetStaticLong("DefaultFlareAmmunitionType"));
	}

	protected FlareAmmunitionGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		FlareDuration = TimeSpan.FromSeconds(double.Parse(root.Element("FlareDuration").Value));
		FlareIllumination = double.Parse(root.Element("FlareIllumination").Value);
		FlareZoneDescription = root.Element("FlareZoneDescription").Value;
		FlareZoneDescriptionColour = Telnet.GetColour(root.Element("FlareZoneDescriptionColour").Value);
		FlareBeginEmote = root.Element("FlareBeginEmote").Value;
		FlareEndEmote = root.Element("FlareEndEmote").Value;
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("AmmoType", AmmoType?.Id ?? 0),
			new XElement("CasingProto", CasingProto?.Id ?? 0),
			new XElement("BulletProto", BulletProto?.Id ?? 0),
			new XElement("FlareDuration", FlareDuration.TotalSeconds),
			new XElement("FlareIllumination", FlareIllumination),
			new XElement("FlareZoneDescription", new XCData(FlareZoneDescription)),
			new XElement("FlareZoneDescriptionColour", FlareZoneDescriptionColour.Name),
			new XElement("FlareBeginEmote", new XCData(FlareBeginEmote)),
			new XElement("FlareEndEmote", new XCData(FlareEndEmote))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new FlareAmmunitionGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new FlareAmmunitionGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("FlareAmmunition".ToLowerInvariant(), true,
			(gameworld, account) => new FlareAmmunitionGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("FlareAmmunition",
			(proto, gameworld) => new FlareAmmunitionGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"FlareAmmunition",
			$"This is a special type of ammunition fired from flare guns",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new FlareAmmunitionGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tYou can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tammo <ammo>- sets the ammo grade, for example, '9x19mm Parabellum'.\n\tbullet <proto> - sets the bullet proto for this round, which will be loaded when fired.\n\tshell <proto> - sets the shell proto for this round, which will be loaded when fired.\n\tillumination <lux> - sets the illumination of the flare\n\tduration <seconds> - sets the duration of the flare\n\tflare <description> - sets the description added to rooms when the flare is in the air\n\tcolour <colour> - sets the colour of the flare description in the air\n\tbegin <emote> - the zone echo when the flare goes up\n\tend <emote> - the zone echo when the flare ends";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "illumination":
			case "lux":
			case "lumen":
			case "light":
				return BuildingCommandIllumination(actor, command);
			case "duration":
			case "length":
			case "time":
				return BuildingCommandDuration(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
			case "flaredesc":
			case "flaredescription":
			case "flare desc":
			case "flare":
			case "flare description":
			case "flare_description":
			case "flare_desc":
				return BuildingCommandFlareDescription(actor, command);
			case "begin":
			case "beginemote":
			case "begin_emote":
			case "begin emote":
				return BuildingCommandBeginEmote(actor, command);
			case "end":
			case "endemote":
			case "end_emote":
			case "end emote":
				return BuildingCommandEndEmote(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandEndEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What text do you want to echo to outdoors areas in the zone when the flare ends?");
			return false;
		}

		FlareEndEmote = command.RemainingArgument.SubstituteANSIColour();
		Changed = true;
		actor.OutputHandler.Send($"When this flare ends it will echo the following: {FlareEndEmote}");
		return true;
	}

	private bool BuildingCommandBeginEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What text do you want to echo to outdoors areas in the zone when the flare goes up?");
			return false;
		}

		FlareBeginEmote = command.RemainingArgument.SubstituteANSIColour();
		Changed = true;
		actor.OutputHandler.Send($"When this flare goes up it will echo the following: {FlareBeginEmote}");
		return true;
	}

	private bool BuildingCommandFlareDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What description do you want to add to all outdoor rooms when this flare is in the air in a zone?");
			return false;
		}

		FlareZoneDescription = command.SafeRemainingArgument.ProperSentences().Trim();
		Changed = true;
		actor.OutputHandler.Send(
			$"When this flare is in the air the description added to rooms will now be {FlareZoneDescription.Colour(FlareZoneDescriptionColour)}.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What colour do you want the zone description when this flare is in the air to be?\nThe options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour == null)
		{
			actor.OutputHandler.Send(
				$"There is no such colour by that name.\nThe options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		FlareZoneDescriptionColour = colour;
		Changed = true;
		actor.OutputHandler.Send(
			$"The zone description when this flare in the air is now coloured {command.SafeRemainingArgument.TitleCase().Colour(colour)}.");
		return true;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many seconds should the flare illumination effect last in the zone when fired into the air?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid number of seconds that is greater than zero.");
			return false;
		}

		FlareDuration = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.OutputHandler.Send(
			$"The flare effect from this flare will now illuminate the zone for {FlareDuration.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandIllumination(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many lux of illumination should the flare provide?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid amount of illumination that is greater than zero.");
			return false;
		}

		FlareIllumination = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This flare will now provide {$"{FlareIllumination.ToString("N3", actor)} lux".ColourValue()} of illumination to the zone when fired into the sky.");
		return true;
		throw new NotImplementedException();
	}

	#endregion

	public double FlareIllumination { get; set; }
	public TimeSpan FlareDuration { get; set; }
	public string FlareZoneDescription { get; set; }
	public ANSIColour FlareZoneDescriptionColour { get; set; }
	public string FlareBeginEmote { get; set; }
	public string FlareEndEmote { get; set; }

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis is flare ammunition of type {4}.\nIt gives an illumination of {6} to the zone for {7}, with a description of {8}.\nBegin Emote: {9}\nEnd Emote: {10}{5}",
			"Flare Ammunition Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			AmmoType != null ? $"{AmmoType.Name} (#{AmmoType.Id:N0})" : "None".Colour(Telnet.Red),
			AmmoType?.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm) ?? false
				? $"\nWhen fired, it loads prototype {(CasingProto != null ? CasingProtoId.ToString(actor).Colour(Telnet.Green) : "None".Colour(Telnet.Red))} as a shell casing and {(BulletProto != null ? BulletProtoId.ToString(actor).Colour(Telnet.Green) : "None".Colour(Telnet.Red))} as a bullet."
				: "",
			FlareIllumination.ToString("N3", actor).FluentAppend(" lux", true).ColourValue(),
			FlareDuration.Describe().ColourValue(),
			FlareZoneDescription.Colour(FlareZoneDescriptionColour),
			FlareBeginEmote.ColourCommand(),
			FlareEndEmote.ColourCommand()
		);
	}
}