#nullable enable

using MudSharp.Construction.Boundary;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Health;
using MudSharp.Models;

namespace MudSharp.NPC.AI;

/// <summary>Reports visible departures privately to a configured clan.</summary>
public sealed class ClanSentryAI : ArtificialIntelligenceBase
{
	private readonly HashSet<CardinalDirection> _directions = [];
	public long ClanId { get; private set; }
	public bool HideAfterAlert { get; private set; } = true;
	public string AlertFormat { get; private set; } = "{0} just passed, heading {1}.";
	public IReadOnlyCollection<CardinalDirection> Directions => _directions;
	public override bool IsReadyToBeUsed => Gameworld.Clans.Get(ClanId) is not null && _directions.Count > 0;

	public ClanSentryAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		ClanId = (long?)root.Element("Clan") ?? 0;
		HideAfterAlert = (bool?)root.Element("HideAfterAlert") ?? true;
		AlertFormat = (string?)root.Element("AlertFormat") ?? AlertFormat;
		if (!AlertFormat.IsValidFormatString(2)) throw new FormatException("Clan sentry alert requires a valid two-argument format.");
		foreach (var direction in root.Element("Directions")?.Elements("Direction") ?? [])
			if (Enum.IsDefined((CardinalDirection)(int)direction)) _directions.Add((CardinalDirection)(int)direction);
	}

	private ClanSentryAI(IFuturemud world, string name) : base(world, name, "ClanSentry")
	{
		_directions.UnionWith([CardinalDirection.North, CardinalDirection.East, CardinalDirection.South, CardinalDirection.West]);
		DatabaseInitialise();
	}
	public static void RegisterLoader()
	{
		RegisterAIType("ClanSentry", (ai, world) => new ClanSentryAI(ai, world));
		RegisterAIBuilderInformation("clansentry", (world, name) => new ClanSentryAI(world, name),
			"Privately reports visible departures to a clan. Set clan and directions before use.");
	}
	protected override string SaveToXml() => new XElement("Definition", new XElement("Clan", ClanId),
		new XElement("HideAfterAlert", HideAfterAlert), new XElement("AlertFormat", AlertFormat),
		new XElement("Directions", _directions.Order().Select(x => new XElement("Direction", (int)x)))).ToString();
	public override bool HandlesEvent(params EventType[] types) => types.Contains(EventType.CharacterLeaveRoomWitness);
	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type != EventType.CharacterLeaveRoomWitness || arguments.Length < 4 ||
		    arguments[0] is not ICharacter traveller || arguments[2] is not IRoomExit exit ||
		    arguments[3] is not ICharacter sentry) return false;
		if (!IsReadyToBeUsed || sentry == traveller || !sentry.State.IsConscious() || sentry.Combat is not null ||
		    !ReferenceEquals(sentry.Location, exit.Origin) || !_directions.Contains(exit.OutboundDirection) ||
		    traveller.ClanMemberships.Any(x => x.Clan.Id == ClanId) || !sentry.CanSee(traveller)) return false;
		var description = traveller.HowSeen(sentry, true);
		foreach (var member in Gameworld.Actors.Where(x => x.State.IsConscious() &&
			         x.ClanMemberships.Any(m => m.Clan.Id == ClanId)).ToArray())
			member.OutputHandler?.Send(string.Format(member, AlertFormat, description, exit.OutboundDirection.DescribeEnum()));
		if (HideAfterAlert) sentry.ExecuteCommand("hide");
		return true;
	}
	protected override string TypeHelpText => @"	#3clan <id|name>#0 - selects the clan receiving private reports
	#3direction <direction>#0 - toggles a reported direction
	#3hide#0 - toggles hiding after a report
	#3format <text>#0 - sets text with {0} for traveller and {1} for direction";
	public override string Show(ICharacter actor) => base.Show(actor) +
		$"\nClan: {(Gameworld.Clans.Get(ClanId)?.Name ?? "unset").ColourName()}\nDirections: {_directions.Select(x => x.DescribeEnum()).ListToString()}\nHide: {HideAfterAlert.ToColouredString()}\nFormat: {AlertFormat.ColourCommand()}";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "clan":
				var clan = Gameworld.Clans.GetByIdOrName(command.SafeRemainingArgument);
				if (clan is null) { actor.OutputHandler.Send("There is no such clan."); return false; }
				ClanId = clan.Id; break;
			case "direction":
				if (!command.SafeRemainingArgument.TryParseEnum<CardinalDirection>(out var direction) || direction == CardinalDirection.Unknown)
				{ actor.OutputHandler.Send("Specify a cardinal direction."); return false; }
				if (!_directions.Remove(direction)) _directions.Add(direction); break;
			case "hide": HideAfterAlert = !HideAfterAlert; break;
			case "format":
				if (command.IsFinished || !command.SafeRemainingArgument.IsValidFormatString(2))
				{ actor.OutputHandler.Send("Use a valid format with {0} for traveller and {1} for direction."); return false; }
				AlertFormat = command.SafeRemainingArgument; break;
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
		Changed = true; actor.OutputHandler.Send("You update the clan sentry configuration."); return true;
	}
}
