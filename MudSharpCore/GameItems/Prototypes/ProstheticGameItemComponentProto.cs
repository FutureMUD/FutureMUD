using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Statements;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ProstheticGameItemComponentProto : GameItemComponentProto
{
	public IBodypart TargetBodypart { get; protected set; }
	public IBodyPrototype TargetBody { get; protected set; }
	public bool Obvious { get; protected set; }
	public bool Functional { get; protected set; }

	public IRace Race { get; protected set; }

	public new Gendering Gender { get; protected set; }

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("prosthetic", true,
			(gameworld, account) => new ProstheticGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Prosthetic",
			(proto, gameworld) => new ProstheticGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Prosthetic",
			$"Item is a {"[prosthetic]".Colour(Telnet.BoldWhite)} limb replacement",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			$"{0} (#{1:N0}r{2:N0}\n\nThis item is a prosthetic for the {TargetBody?.Name.Colour(Telnet.Green) ?? "Unknown".Colour(Telnet.Red)} body type which restores severs of the {TargetBodypart?.FullDescription().Colour(Telnet.Green) ?? "Unknown".Colour(Telnet.Red)} bodypart. This prosthetic is {(Obvious ? "" : "not ")} obvious, and is {(Functional ? "" : "not ")} functional. It is designed for {(Race != null ? $"the {Race.Name.Colour(Telnet.Green)} race" : "no race in particular")} and the {Gender.GenderClass().Colour(Telnet.Yellow)} gender.",
			"Prosthetic Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		Obvious = bool.Parse(root.Element("Obvious")?.Value ?? "false");
		Functional = bool.Parse(root.Element("Functional")?.Value ?? "false");
		TargetBody = Gameworld.BodyPrototypes.Get(long.Parse(root.Element("TargetBody")?.Value ?? "0"));
		TargetBodypart = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("TargetBodypart")?.Value ?? "0"));
		Gender = Gendering.Get((Gender)short.Parse(root.Element("Gender")?.Value ?? "0"));
		Race = Gameworld.Races.Get(long.Parse(root.Element("Race")?.Value ?? "0"));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Obvious", Obvious),
			new XElement("Functional", Functional),
			new XElement("TargetBody", TargetBody?.Id ?? 0),
			new XElement("TargetBodypart", TargetBodypart?.Id ?? 0),
			new XElement("Gender", (int)Gender.Enum),
			new XElement("Race", Race?.Id ?? 0L)
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ProstheticGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ProstheticGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ProstheticGameItemComponentProto(proto, gameworld));
	}

	#region Overrides of GameItemComponentProto

	public override string TypeDescription { get; } = "Prosthetic";

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		return base.CanSubmit() && TargetBody != null && TargetBodypart != null;
	}

	#region Overrides of EditableItem

	public override string WhyCannotSubmit()
	{
		if (TargetBody == null)
		{
			return "You must first set a target body for this prosthetic.";
		}

		return TargetBodypart == null
			? "You must first set a target bodypart for this prosthetic."
			: base.WhyCannotSubmit();
	}

	#endregion

	#endregion

	#endregion

	#region Constructors

	protected ProstheticGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected ProstheticGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Prosthetic")
	{
		Gender = Gendering.Get(Form.Shape.Gender.Indeterminate);
		Changed = true;
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <which> - sets the body prototype that this targets\n\tbodypart <which> - sets the bodypart sever location that this goes with\n\tgender <which> - sets the gender associated with this prosthetic, if applicable for the race.\n\trace <which> - sets a race that this is designed for\n\trace none - clears a set race\n\tobvious - toggles whether this is an obvious prosthetic (can be noticed)\n\tfunctional - toggles whether this is a functional prosthetic or just ornamental";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "obvious":
				return BuildingCommandObvious(actor, command);
			case "functional":
			case "functioning":
			case "function":
			case "func":
				return BuildingCommandFunctional(actor, command);
			case "body":
			case "targetbody":
			case "target body":
			case "tbody":
				return BuildingCommandTargetBody(actor, command);
			case "part":
			case "bodypart":
			case "target":
				return BuildingCommandTargetBodypart(actor, command);
			case "gender":
			case "sex":
				return BuildingCommandGender(actor, command);
			case "race":
				return BuildingCommandRace(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which gender do you want this prosthetic to use for its parts list?");
			return false;
		}

		Gender = Gendering.Get(command.SafeRemainingArgument);
		Changed = true;
		actor.Send(
			$"This prosthetic will now use the {Gender.GenderClass().Colour(Telnet.Green)} gender to populate its parts list.");
		return true;
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"Which race do you want this prosthetic to use for its parts list? Specify none to remove any race.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			Race = null;
			Changed = true;
			actor.Send("This prosthetic will now use no particular race to determine its parts list.");
			return true;
		}

		var race = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Races.Get(value)
			: Gameworld.Races.GetByName(command.SafeRemainingArgument);
		if (race == null)
		{
			actor.Send("There is no such race. Use the argument 'none' if you want to remove an existing race.");
			return false;
		}

		Race = race;
		Changed = true;
		actor.Send(
			$"This prosthetic will now use the {Race.Name.Colour(Telnet.Green)} race to determine its parts list.");
		return true;
	}

	private bool BuildingCommandObvious(ICharacter actor, StringStack command)
	{
		Obvious = !Obvious;
		actor.Send($"This prosthetic is {(Obvious ? "now" : "no longer")} obvious when being observed.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandFunctional(ICharacter actor, StringStack command)
	{
		Functional = !Functional;
		actor.Send($"This prosthetic is {(Functional ? "now" : "no longer")} functional.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandTargetBody(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which body do you want this prosthetic to target?");
			return false;
		}

		var targetBody = long.TryParse(command.SafeRemainingArgument, out var value)
			? actor.Gameworld.BodyPrototypes.Get(value)
			: actor.Gameworld.BodyPrototypes.GetByName(command.SafeRemainingArgument);
		if (targetBody == null)
		{
			actor.Send("There is no such body for this prosthetic to target.");
			return false;
		}

		TargetBody = targetBody;
		Changed = true;
		actor.Send(
			$"This prosthetic now targets the {TargetBody.Name.Proper().Colour(Telnet.Green)} body prototype.");
		return true;
	}

	private bool BuildingCommandTargetBodypart(ICharacter actor, StringStack command)
	{
		if (TargetBody == null)
		{
			actor.Send("You must first select a target body before you can select a bodypart.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("Which bodypart do you want this prosthetic to target severs of?");
			return false;
		}

		var targetPart = TargetBody.AllExternalBodyparts.GetFromItemListByKeyword(command.SafeRemainingArgument, actor);
		if (targetPart == null)
		{
			actor.Send(
				$"There is no such bodypart on the {TargetBody.Name.Proper().Colour(Telnet.Green)} body type for this prosthetic to target.");
			return false;
		}

		if (!targetPart.CanSever)
		{
			actor.Send(
				$"The {targetPart.FullDescription().Colour(Telnet.Green)} bodypart cannot be severed, and so cannot have prosthetics.");
			return false;
		}

		TargetBodypart = targetPart;
		Changed = true;
		actor.Send(
			$"This prosthetic will now target severs of the {targetPart.FullDescription().Colour(Telnet.Green)} bodypart.");
		return true;
	}

	#endregion Building Commands
}