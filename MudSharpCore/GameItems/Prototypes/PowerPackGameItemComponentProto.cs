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

public class PowerPackGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "PowerPack";

	public double WattSecondCapacity { get; protected set; }
	public double WattSecondBonusPerQuality { get; protected set; }

	public string ClipType { get; protected set; }

	#region Constructors

	protected PowerPackGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"PowerPack")
	{
		WattSecondCapacity = 50000;
		WattSecondBonusPerQuality = 10000;
		ClipType = Gameworld.GetStaticConfiguration("DefaultLaserClipType");
	}

	protected PowerPackGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		WattSecondCapacity = double.Parse(root.Element("WattSecondCapacity").Value);
		WattSecondBonusPerQuality = double.Parse(root.Element("WattSecondBonusPerQuality").Value);
		ClipType = root.Element("ClipType").Value;
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new[]
		{
			new XElement("WattSecondCapacity", WattSecondCapacity),
			new XElement("WattSecondBonusPerQuality", WattSecondBonusPerQuality),
			new XElement("ClipType", new XCData(ClipType))
		}).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PowerPackGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PowerPackGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("PowerPack".ToLowerInvariant(), true,
			(gameworld, account) => new PowerPackGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PowerPack",
			(proto, gameworld) => new PowerPackGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"PowerPack",
			$"A special type of ammunition specifically for Laser guns",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PowerPackGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tcapacity <wattseconds> - sets the base capacity of this power pack in watt-seconds\n\tbonus <wattseconds> - sets the bonus power per quality\n\tclip <type> - sets the specific clip type that this power pack matches";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "watt":
			case "watts":
			case "capacity":
				return BuildingCommandCapacity(actor, command);
			case "bonus":
			case "quality":
				return BuildingCommandBonus(actor, command);
			case "clip":
			case "cliptype":
			case "ammo":
				return BuildingCommandClip(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandClip(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What specific clip type should this power pack match?");
			return false;
		}

		ClipType = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This power pack now matches laser guns of the specific type {ClipType.Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What is the base capacity of this power pack in watt-seconds?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("The base capacity must be a valid number greater than zero.");
			return false;
		}

		WattSecondCapacity = value;
		Changed = true;
		actor.Send($"This power pack now provides a base power capacity of {WattSecondCapacity} watt-seconds.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What is the bonus capacity per quality of this power pack in watt-seconds?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("The bonus capacity per quality must be a valid number greater than zero.");
			return false;
		}

		WattSecondBonusPerQuality = value;
		Changed = true;
		actor.Send(
			$"This power pack now provides a bonus capacity per quality of {WattSecondBonusPerQuality} watt-seconds.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a non-rechargable power pack for a laser. It provides {4:N0} + quality * {5:N0} watt-seconds of power.",
			"PowerPack Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			WattSecondCapacity,
			WattSecondBonusPerQuality
		);
	}
}