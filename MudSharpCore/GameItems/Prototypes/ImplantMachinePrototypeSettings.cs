#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public sealed class ImplantMachinePrototypeSettings : IImplantMachinePrototypeSettings
{
	private readonly IFuturemud _gameworld;

	public ImplantMachinePrototypeSettings(IFuturemud gameworld)
	{
		_gameworld = gameworld;
		External = false;
		ExternalDescription = "a small portion of a high-tech device";
		TargetBody = gameworld.BodyPrototypes.Get(gameworld.GetStaticLong("DefaultBodyForImplants"))!;
		TargetBodypart = TargetBody?.AllBodypartsBonesAndOrgans.FirstOrDefault()!;
		ImplantSpaceOccupied = 1.0;
		InstallDifficulty = (Difficulty)gameworld.GetStaticInt("DefaultImplantInstallDifficulty");
		ImplantDamageFunctionGrace = gameworld.GetStaticDouble("DefaultImplantDamageGrace");
	}

	public bool External { get; private set; }
	public string ExternalDescription { get; private set; }
	public IBodyPrototype TargetBody { get; private set; } = null!;
	public IBodypart TargetBodypart { get; private set; } = null!;
	public double ImplantSpaceOccupied { get; private set; }
	public Difficulty InstallDifficulty { get; private set; }
	public double ImplantDamageFunctionGrace { get; private set; }

	public const string BuildingHelp = @"
	#3body <body>#0 - sets the body prototype
	#3bodypart <bodypart>#0 - sets the installation bodypart
	#3external#0 - toggles external visibility
	#3externaldesc <description>#0 - sets the installed external description
	#3space <number>#0 - sets occupied implant space
	#3difficulty <difficulty>#0 - sets surgical installation difficulty
	#3grace <percentage>#0 - sets damage grace before function declines";

	public void Load(XElement root)
	{
		External = bool.TryParse(root.Element("ImplantExternal")?.Value, out var external) && external;
		ExternalDescription = root.Element("ImplantExternalDescription")?.Value ?? ExternalDescription;
		TargetBody = _gameworld.BodyPrototypes.Get(long.TryParse(root.Element("ImplantTargetBody")?.Value, out var bodyId)
			? bodyId : 0L) ?? TargetBody;
		var partId = long.TryParse(root.Element("ImplantTargetBodypart")?.Value, out var parsedPart) ? parsedPart : 0L;
		TargetBodypart = TargetBody.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Id == partId) ?? TargetBodypart;
		ImplantSpaceOccupied = double.TryParse(root.Element("ImplantSpace")?.Value, out var space) ? space : 1.0;
		InstallDifficulty = Enum.TryParse<Difficulty>(root.Element("ImplantDifficulty")?.Value, out var difficulty)
			? difficulty : InstallDifficulty;
		ImplantDamageFunctionGrace = double.TryParse(root.Element("ImplantDamageGrace")?.Value, out var grace)
			? grace : ImplantDamageFunctionGrace;
	}

	public void Save(XElement root)
	{
		root.Add(new XElement("ImplantExternal", External),
			new XElement("ImplantExternalDescription", new XCData(ExternalDescription ?? string.Empty)),
			new XElement("ImplantTargetBody", TargetBody?.Id ?? 0L),
			new XElement("ImplantTargetBodypart", TargetBodypart?.Id ?? 0L),
			new XElement("ImplantSpace", ImplantSpaceOccupied),
			new XElement("ImplantDifficulty", InstallDifficulty),
			new XElement("ImplantDamageGrace", ImplantDamageFunctionGrace));
	}

	public bool? BuildingCommand(ICharacter actor, string verb, StringStack command, Action changed)
	{
		switch (verb)
		{
			case "external":
				External = !External;
				actor.Send($"This implant is {(External ? "now" : "no longer")} externally visible.");
				break;
			case "externaldesc":
			case "externaldescription":
				if (command.IsFinished) return Fail(actor, "What external description should this implant use?");
				ExternalDescription = command.SafeRemainingArgument;
				actor.Send("The implant's external description has been updated.");
				break;
			case "body":
				if (command.IsFinished) return Fail(actor, "Which body prototype is this implant for?");
				var body = long.TryParse(command.SafeRemainingArgument, out var bodyId)
					? _gameworld.BodyPrototypes.Get(bodyId)
					: _gameworld.BodyPrototypes.GetByName(command.SafeRemainingArgument);
				if (body is null) return Fail(actor, "There is no body prototype like that.");
				TargetBody = body;
				TargetBodypart = body.AllBodypartsBonesAndOrgans.FirstOrDefault()!;
				actor.Send($"This implant is now designed for the {body.Name.ColourName()} body.");
				break;
			case "bodypart":
				if (command.IsFinished) return Fail(actor, "Which bodypart should receive this implant?");
				var text = command.SafeRemainingArgument;
				var part = long.TryParse(text, out var partId)
					? TargetBody.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Id == partId)
					: TargetBody.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Name.EqualTo(text) || x.FullDescription().EqualTo(text));
				if (part is null) return Fail(actor, "There is no such bodypart on that body.");
				TargetBodypart = part;
				actor.Send($"This implant is now designed for the {part.FullDescription().ColourCommand()}.");
				break;
			case "space":
				if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var space) || space < 0.0)
					return Fail(actor, "Enter a non-negative amount of implant space.");
				ImplantSpaceOccupied = space;
				actor.Send($"This implant now occupies {space.ToString("N2", actor).ColourValue()} space.");
				break;
			case "difficulty":
			case "install":
				if (command.IsFinished || !CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
					return Fail(actor, "Specify a valid installation difficulty.");
				InstallDifficulty = difficulty;
				actor.Send($"Installation is now {difficulty.Describe().ColourValue()}.");
				break;
			case "grace":
				if (command.IsFinished || !command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var grace) ||
				    grace is < 0.0 or > 1.0) return Fail(actor, "Specify a percentage from 0% to 100%.");
				ImplantDamageFunctionGrace = grace;
				actor.Send($"Damage grace is now {grace.ToString("P2", actor).ColourValue()}.");
				break;
			default:
				return null;
		}
		changed();
		return true;
	}

	private static bool Fail(ICharacter actor, string text)
	{
		actor.Send(text);
		return false;
	}
}
