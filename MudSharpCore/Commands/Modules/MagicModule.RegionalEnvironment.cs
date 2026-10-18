#nullable enable

using System.Globalization;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic.Environment;

namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	private const string RegionalEnvironmentHelp = @"Regional ecological life is shared by physical zones or areas; it is not personal mana.
#3magic environment regional profiles#0 - list reusable life profiles
#3magic environment regional profile create <name> <life per room> <life per online hour>#0
#3magic environment regional profile <profile> show|contribution <number>|recovery <number>|name <text>#0
#3magic environment regional profile <profile> terrain <terrain> <life|remove>#0 - override a terrain contribution
#3magic environment regional zone|area <here|id|name> show#0
#3magic environment regional zone|area <here|id|name> <profile|off|inherit> [priority]#0
#3magic environment regional here#0 - inspect this room's selected shared pool

Configured areas precede zones; higher area priority wins, then lower area ID. Off blocks regional inheritance and permits configured local fallback; inherit restores parent selection. Explicitly disabled rooms neither contribute nor gather. New pool bindings begin full once; rebinding, capacity growth and restart never refill them. Recovery occurs once per owner per online real hour. Configure Land's preference with #3magic capability set gather land <method> regional <life units|off>#0.";

	private static void EnvironmentRegional(ICharacter actor, StringStack command, IRegionalGatheringService? service)
	{
		if (service is null) { actor.OutputHandler.Send("Regional gathering is unavailable.".ColourError()); return; }
		try
		{
			var action = command.PopSpeech().ToLowerInvariant();
			if (action == "profiles")
			{
				var text = new StringBuilder("Regional Gathering Profiles\n");
				foreach (var p in service.Profiles.OrderBy(x => x.Name))
					text.AppendLine($"#{p.Id.ToString("N0", actor)} {p.Name.ColourName()}: default {p.DefaultContribution.ToString("N2", actor)} life/room, {p.RecoveryPerHour.ToString("N2", actor)} life/hour; {p.TerrainContributions.Count} terrain overrides");
				actor.OutputHandler.Send(text.ToString());
				return;
			}
			if (action == "here")
			{
				if (actor.Location is null) throw new ArgumentException("Stand in a physical room.");
				if (service.UnavailableReason(actor.Location) is { } error) throw new ArgumentException(error);
				actor.OutputHandler.Send(service.Inspect(actor.Location) is { } snapshot
					? $"Selected {snapshot.Owner.Kind.DescribeEnum()} #{snapshot.Owner.Id.ToString("N0", actor)}: {snapshot.Stock.ToString("N2", actor).ColourValue()} / {snapshot.Capacity.ToString("N2", actor)} life, {snapshot.RoomCount.ToString("N0", actor)} contributing rooms."
					: "No regional pool applies here. A configured local Land source may still be used.");
				return;
			}
			if (action == "profile") { EnvironmentRegionalProfile(actor, command, service); return; }
			if (!Enum.TryParse<RegionalGatheringOwnerKind>(action, true, out var kind) || !Enum.IsDefined(kind))
			{ actor.OutputHandler.Send(RegionalEnvironmentHelp.SubstituteANSIColour()); return; }
			var target = command.PopSpeech();
			long? id;
			if (kind == RegionalGatheringOwnerKind.Zone)
				id = (target.EqualTo("here") ? actor.Location?.OwningZone : actor.Gameworld.Zones.GetByIdOrName(target))?.Id;
			else
			{
				var areas = actor.Location?.OwningAreas.ToArray() ?? [];
				if (target.EqualTo("here") && areas.Length != 1) throw new ArgumentException("Specify an area ID or name when this room belongs to zero or multiple areas.");
				id = (target.EqualTo("here") ? areas.Single() : actor.Gameworld.Areas.GetByIdOrName(target))?.Id;
			}
			if (!id.HasValue) throw new ArgumentException("Select an existing physical zone or area.");
			var owner = new RegionalGatheringOwner(kind, id.Value);
			var value = command.PopSpeech();
			if (value.Length == 0 || value.EqualTo("show"))
			{
				var pool = service.Pools.SingleOrDefault(x => x.Owner == owner);
				actor.OutputHandler.Send(pool is null ? "No regional binding; parent selection is inherited."
					: $"{owner.Kind.DescribeEnum()} #{owner.Id.ToString("N0", actor)}: {(pool.Inherit ? "inherit" : pool.ProfileId is { } profileId ? $"profile #{profileId.ToString("N0", actor)}" : "off")}, priority {pool.Priority}; saved life {pool.Stock.ToString("N2", actor)} / {pool.Capacity.ToString("N2", actor)}.");
				return;
			}
			var priority = 0;
			if (!command.IsFinished && !int.TryParse(command.SafeRemainingArgument, NumberStyles.Integer, CultureInfo.InvariantCulture, out priority))
				throw new ArgumentException("Priority must be an integer.");
			long? profile = value.EqualToAny("off", "inherit") ? null : FindRegionalProfile(service, value).Id;
			service.SetBinding(owner, profile, priority, value.EqualTo("inherit"));
			actor.OutputHandler.Send("Updated the regional binding. Existing depletion is preserved; capacity increases do not refill stock.".ColourValue());
		}
		catch (Exception error) when (error is ArgumentException or InvalidOperationException)
		{
			actor.OutputHandler.Send(error.Message.ColourError());
		}
	}
	private static RegionalGatheringProfile FindRegionalProfile(IRegionalGatheringService service, string text) =>
		service.Profiles.FirstOrDefault(x => long.TryParse(text, out var id) ? x.Id == id : x.Name.EqualTo(text))
		?? throw new ArgumentException("Select a regional profile by exact ID or name. See regional profiles.");
	private static double RegionalNumber(string text)
	{
		if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value) || value < 0)
			throw new ArgumentException("Specify a finite non-negative number.");
		return value;
	}
	private static void EnvironmentRegionalProfile(ICharacter actor, StringStack command, IRegionalGatheringService service)
	{
		var name = command.PopSpeech();
		if (name.EqualTo("create"))
		{
			name = command.PopSpeech();
			var contribution = RegionalNumber(command.PopSpeech());
			var recovery = RegionalNumber(command.PopSpeech());
			if (!command.IsFinished) throw new ArgumentException("Unexpected profile creation arguments.");
			var created = service.CreateProfile(name, contribution, recovery);
			actor.OutputHandler.Send($"Created regional profile #{created.Id.ToString("N0", actor).ColourValue()} {created.Name.ColourName()}.");
			return;
		}
		var profile = FindRegionalProfile(service, name);
		var setting = command.PopSpeech().ToLowerInvariant();
		if (setting.Length == 0 || setting == "show")
		{
			actor.OutputHandler.Send($"{profile.Name.ColourName()}: default {profile.DefaultContribution.ToString("N2", actor)} life/room, {profile.RecoveryPerHour.ToString("N2", actor)} life per online real hour.\n" +
				string.Join("\n", profile.TerrainContributions.OrderBy(x => x.Key).Select(x => $"  {actor.Gameworld.Terrains.Get(x.Key)?.Name ?? $"terrain #{x.Key}"}: {x.Value.ToString("N2", actor)}")));
			return;
		}
		var updated = profile with { Revision = checked(profile.Revision + 1) };
		switch (setting)
		{
			case "contribution": updated = updated with { DefaultContribution = RegionalNumber(command.SafeRemainingArgument) }; break;
			case "recovery": updated = updated with { RecoveryPerHour = RegionalNumber(command.SafeRemainingArgument) }; break;
			case "name": updated = updated with { Name = command.SafeRemainingArgument }; break;
			case "terrain":
				var terrain = actor.Gameworld.Terrains.GetByIdOrName(command.PopSpeech()) ?? throw new ArgumentException("Select an existing terrain.");
				var value = command.SafeRemainingArgument;
				var weights = new Dictionary<long, double>(profile.TerrainContributions);
				if (value.EqualTo("remove")) weights.Remove(terrain.Id); else weights[terrain.Id] = RegionalNumber(value);
				updated = updated with { TerrainContributions = weights };
				break;
			default: throw new ArgumentException("Use show, name, contribution, recovery or terrain. See regional help.");
		}
		service.SetProfile(updated);
		actor.OutputHandler.Send("Updated the regional profile; existing stock is conserved and capacity reductions clamp it.".ColourValue());
	}
}
