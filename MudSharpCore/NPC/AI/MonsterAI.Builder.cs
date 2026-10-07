#nullable enable

using MudSharp.Celestial;
using MudSharp.Effects.Concrete;
using MudSharp.Magic.Powers;

namespace MudSharp.NPC.AI;

public sealed partial class MonsterAI
{
	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3motive <Scheduled|Territory|Provocation|Condition|Hunger|clear>#0 - toggle a motive; defensive precedence is fixed
	#3active times <bands|all>#0 - local Dawn, Morning, Afternoon, Dusk, Night
	#3active seasons <groups|all>#0 - local season groups (quote names with spaces)
	#3active calendar <calendar|clear>#0 - choose the game's calendar
	#3active months <aliases|all>#0 - months, including intercalary months
	#3active days <first> <last>#0 or #3active days all#0 - inclusive day-of-month range
	#3active moon <moon> <phases>#0 or #3active moon clear#0 - one locally present moon
	#3active condition <prog|clear>#0 - Boolean(Character); combined with all native restrictions
	#3defencewindow <on|off>#0 - apply the window to territory and provocation
	#3trapprovokes <on|off>#0 - allow an actual owned-trap capture to provoke pursuit
	#3targets eligibility|preference <prog|clear>#0 - Boolean/Number(Character,Character)
	#3targets include|exclude <race|clear>#0, #3targets prefer <race> <score>#0, #3targets sizes <min> <max>|any#0
	#3targets selection <Safest|Nearest|LargestManageable>#0
	#3allies same <on|off>#0 or #3allies prog <prog|clear>#0 - Boolean(Character,Character)
	#3feeding <Off|Needs|AfterKill>#0, #3feeding bites <1-100>#0, #3feeding duration <seconds>#0
	#3hunting on|off#0, #3hunting opening <Direct|Ambush|TrapWait>#0, #3hunting followup <Fight|Extract|VenomWithdrawal>#0
	#3hunting layer <layer|any>#0, #3hunting opportunity <on|off>#0, #3hunting range <1-20>#0, #3hunting timeout|lost <seconds>#0
	#3assessment cautious|balanced|bold#0, #3assessment engage|abandon|confidence <number>#0, #3assessment weight <factor> <number>#0
	#3movement <Ground|Swim|Fly|Arboreal|Amphibious>#0; #3movement range|chance|enabled|cell|emote|flyinglayer|restinglayer|preferredhabitat|toleratedhabitat#0
	#3home none|territorial|denning#0; #3home location <Location(Character) prog>#0, #3home anchor <Boolean(Character,Item) prog>#0
	#3home craft|site|enabled|territory|size|share|shareother#0 - shared shelter and territory controls
	#3awareness none|wary|wimpy|skittish|guarding|range|memory|threat|avoid|senses#0 - shared observation and refuge policy
	#3refuge none|home|den|trees|sky|water|prog|layer|cell|return#0
	#3guardrange <0-20>#0, #3warning <seconds>#0, #3warningemote <emote|clear>#0 ($0 monster, $1 target)
	#3returnhome <on|off>#0, #3cooldown <seconds>#0, #3provocation <seconds>#0
	#3engagedelay <dice milliseconds>#0, #3engageemote <emote|clear>#0
No motive enables proactive attacks by default. Times are game time; all durations are real seconds except engagedelay.
Monster AI never changes needs, equips gear or grants powers. Configure normal combat settings and loadouts separately.
Use one primary Animal/Monster AI per NPC; Monster AI is not a Wildlife group member.";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopForSwitch();
		bool Bad(string message) { actor.OutputHandler.Send(message); return false; }
		bool Toggle(Action<bool> setter)
		{
			if (!command.SafeRemainingArgument.EqualToAny("on", "off")) return Bad("Use on or off.");
			setter(command.SafeRemainingArgument.EqualTo("on")); return true;
		}
		bool Seconds(Action<TimeSpan> setter, int minimum = 0, int maximum = 86400)
		{
			if (!double.TryParse(command.SafeRemainingArgument, out var value) || !double.IsFinite(value) || value < minimum || value > maximum)
				return Bad($"Use seconds between {minimum.ToString("N0", actor)} and {maximum.ToString("N0", actor)}.");
			setter(TimeSpan.FromSeconds(value)); return true;
		}
		bool success;
		switch (verb)
		{
			case "motive":
				if (command.SafeRemainingArgument.EqualTo("clear")) Motives.Clear();
				else
				{
					if (!command.SafeRemainingArgument.TryParseEnum<MonsterMotive>(out var motive) || motive is MonsterMotive.None or MonsterMotive.SelfDefence)
						return Bad("Use Scheduled, Territory, Provocation, Condition, Hunger or clear. Each named motive toggles independently.");
					if (!Motives.Remove(motive)) Motives.Add(motive);
				}
				success = true; break;
			case "active": success = BuildingCommandActive(actor, command); break;
			case "allies":
				var option = command.PopForSwitch();
				success = option == "same" ? Toggle(x => SameRaceAllies = x) : option == "prog" && SetMonsterProg(actor, command, x => AllyProgId = x, true);
				if (!success && option is not ("same" or "prog")) return Bad("Use allies same <on|off> or allies prog <prog|clear>.");
				break;
			case "targets":
				if (command.PeekSpeech().EqualToAny("people", "classification")) return Bad("Monster targets are independent of food classification. Use eligibility, include, exclude or allies.");
				return BuildingCommandHunting(actor, new StringStack("prey " + command.RemainingArgument));
			case "hunting": case "assessment":
				if (verb == "assessment" && command.PeekSpeech().EqualTo("starvation")) return Bad("Monster assessment does not use a starvation bonus.");
				return BuildingCommandHunting(actor, command.GetUndo());
			case "movement": return BuildingCommandMovement(actor, command);
			case "home": return BuildingCommandHome(actor, command);
			case "awareness": return BuildingCommandAwareness(actor, command);
			case "refuge": return BuildingCommandRefuge(actor, command);
			case "engagedelay": return BuildingCommandEngageDelay(actor, command);
			case "engageemote": return BuildingCommandEngageEmote(actor, command);
			case "warningemote": return BuildingCommandThreatPostureEmote(actor, command);
			case "defencewindow": success = Toggle(x => DefenceUsesWindow = x); break;
			case "trapprovokes": success = Toggle(x => TrapProvokes = x); break;
			case "returnhome": success = Toggle(x => ReturnHome = x); break;
			case "cooldown": success = Seconds(x => Cooldown = x); break;
			case "provocation": success = Seconds(x => ProvocationDuration = x, 1); break;
			case "warning": success = Seconds(x => WarningDelay = x, 0, 3600); break;
			case "guardrange":
				if (!int.TryParse(command.SafeRemainingArgument, out var range) || range < 0 || range > 20) return Bad("Use a defended radius from 0 to 20 cells; zero means the home cell.");
				GuardRange = range; success = true; break;
			case "feeding":
				var feeding = command.PopForSwitch();
				if (feeding == "duration") { success = Seconds(x => FeedingDuration = x, 1, 3600); break; }
				if (feeding == "bites")
				{
					if (!int.TryParse(command.SafeRemainingArgument, out var bites) || bites < 1 || bites > 100) return Bad("Use 1 to 100 bites per after-kill episode.");
					FeedingBites = bites;
				}
				else
				{
					if (!feeding.TryParseEnum<MonsterFeedingMode>(out var mode)) return Bad("Use feeding Off, Needs, AfterKill, bites <number> or duration <seconds>.");
					Feeding = mode;
				}
				success = true; break;
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
		if (!success) return false;
		Changed = true;
		actor.OutputHandler.Send(Show(actor));
		return true;
	}

	private bool SetMonsterProg(ICharacter actor, StringStack command, Action<long> setter, bool target)
	{
		if (command.SafeRemainingArgument.EqualTo("clear")) { setter(0); return true; }
		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			target ? [ProgVariableTypes.Character, ProgVariableTypes.Character] : [ProgVariableTypes.Character]).LookupProg();
		if (prog is null) return false;
		setter(prog.Id); return true;
	}
	private bool BuildingCommandActive(ICharacter actor, StringStack command)
	{
		bool Bad(string message) { actor.OutputHandler.Send(message); return false; }
		var option = command.PopForSwitch();
		switch (option)
		{
			case "times":
				if (command.SafeRemainingArgument.EqualTo("all")) { ActivityWindow.Times.Clear(); return true; }
				var times = new HashSet<TimeOfDay>();
				while (!command.IsFinished)
				{
					if (!command.PopSpeech().TryParseEnum<TimeOfDay>(out var time)) return Bad($"Valid time bands: {Enum.GetValues<TimeOfDay>().Select(x => x.DescribeEnum()).ListToString()}.");
					times.Add(time);
				}
				if (times.Count == 0) return Bad("Specify one or more time bands, or all.");
				ActivityWindow.Times.Clear(); ActivityWindow.Times.UnionWith(times); return true;
			case "seasons":
				if (command.SafeRemainingArgument.EqualTo("all")) { ActivityWindow.Seasons.Clear(); return true; }
				var validSeasons = Gameworld.Seasons.Select(x => x.SeasonGroup).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
				var seasons = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				while (!command.IsFinished)
				{
					var token = command.PopSpeech();
					var season = validSeasons.FirstOrDefault(x => x.EqualTo(token));
					if (season is null) return Bad($"Valid season groups: {validSeasons.ListToString()}.");
					seasons.Add(season);
				}
				if (seasons.Count == 0) return Bad("Specify one or more season groups, or all.");
				ActivityWindow.Seasons.Clear(); ActivityWindow.Seasons.UnionWith(seasons); return true;
			case "calendar":
				if (command.SafeRemainingArgument.EqualTo("clear"))
				{
					ActivityWindow.CalendarId = 0; ActivityWindow.Months.Clear(); ActivityWindow.FirstDay = ActivityWindow.LastDay = null; return true;
				}
				var calendar = Gameworld.Calendars.GetByIdOrName(command.SafeRemainingArgument);
				if (calendar is null) return Bad($"Select a calendar: {Gameworld.Calendars.Select(x => $"{x.Id}: {x.Name}").ListToString()}.");
				ActivityWindow.CalendarId = calendar.Id; ActivityWindow.Months.Clear(); return true;
			case "months":
				if (command.SafeRemainingArgument.EqualTo("all")) { ActivityWindow.Months.Clear(); return true; }
				var selected = Gameworld.Calendars.Get(ActivityWindow.CalendarId);
				if (selected is null) return Bad("Select an active calendar first.");
				var aliases = MonsterActivityWindow.MonthAliases(selected).ToList();
				var months = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				while (!command.IsFinished)
				{
					var token = command.PopSpeech();
					var alias = aliases.FirstOrDefault(x => x.EqualTo(token));
					if (alias is null) return Bad($"Valid month aliases: {aliases.ListToString()}.");
					months.Add(alias);
				}
				if (months.Count == 0) return Bad("Specify one or more month aliases, or all.");
				ActivityWindow.Months.Clear(); ActivityWindow.Months.UnionWith(months); return true;
			case "days":
				if (command.SafeRemainingArgument.EqualTo("all")) { ActivityWindow.FirstDay = ActivityWindow.LastDay = null; return true; }
				if (ActivityWindow.CalendarId == 0) return Bad("Select an active calendar first.");
				if (!int.TryParse(command.PopSpeech(), out var first) || !int.TryParse(command.PopSpeech(), out var last) || first < 1 || last < first || !command.IsFinished)
					return Bad("Specify a positive first and last day, inclusive. Days absent from a short month never match.");
				ActivityWindow.FirstDay = first; ActivityWindow.LastDay = last; return true;
			case "moon":
				if (command.SafeRemainingArgument.EqualTo("clear")) { ActivityWindow.MoonId = 0; ActivityWindow.Phases.Clear(); return true; }
				var moon = Gameworld.CelestialObjects.GetByIdOrName(command.PopSpeech());
				if (moon is not ILunarPhase) return Bad($"Select a moon: {Gameworld.CelestialObjects.Where(x => x is ILunarPhase).Select(x => $"{x.Id}: {x.Name}").ListToString()}.");
				var phases = new HashSet<MoonPhase>();
				while (!command.IsFinished)
				{
					if (!command.PopSpeech().TryParseEnum<MoonPhase>(out var phase)) return Bad($"Valid phases: {Enum.GetValues<MoonPhase>().Select(x => x.DescribeEnum()).ListToString()}.");
					phases.Add(phase);
				}
				if (phases.Count == 0) return Bad("Specify at least one lunar phase after the moon.");
				ActivityWindow.MoonId = moon.Id; ActivityWindow.Phases.Clear(); ActivityWindow.Phases.UnionWith(phases); return true;
			case "condition": return SetMonsterProg(actor, command, x => ActivityWindow.ConditionProgId = x, false);
			default: return Bad("Use active times, seasons, calendar, months, days, moon or condition. Restrictions combine with AND.");
		}
	}

	protected override string ShowHunting(IPerceiver voyeur) =>
		$"Hunting: {Hunting.Enabled.ToColouredString()}; target selection: {Hunting.Selection.DescribeEnum().ColourName()}\n" +
		$"Opening: {Hunting.Opening.DescribeEnum()}; followup: {Hunting.Followup.DescribeEnum()}; layer: {Hunting.PreferredLayer?.DescribeEnum() ?? "any"}; opportunity: {Hunting.Opportunistic.ToColouredString()}\n" +
		$"Assessment start/abandon: {Hunting.EngageThreshold.ToString("N1", voyeur)}/{Hunting.AbandonThreshold.ToString("N1", voyeur)}; confidence: {Hunting.ConfidenceBias.ToString("N1", voyeur)}\n" +
		$"Pursuit: {Hunting.PursuitRange.ToString("N0", voyeur)} cells, {Hunting.PursuitTimeout.Describe(voyeur)}; lost sight: {Hunting.LostTimeout.Describe(voyeur)}\n" +
		$"Target size differences: {Hunting.MinimumSizeDifference?.ToString(voyeur) ?? "any"} to {Hunting.MaximumSizeDifference?.ToString(voyeur) ?? "any"}; include: {Hunting.IncludedRaces.Select(x => x.ToString(voyeur)).ListToCommaSeparatedValues()}; exclude: {Hunting.ExcludedRaces.Select(x => x.ToString(voyeur)).ListToCommaSeparatedValues()}\n" +
		$"Preference lineages: {Hunting.PreferredRaces.Select(x => $"{x.Key}: {x.Value}").ListToCommaSeparatedValues()}; eligibility/preference progs: {Hunting.EligibilityProgId}/{Hunting.PreferenceProgId}\n" +
		$"Weights: {Hunting.Weights.Select(x => $"{x.Key}: {x.Value.ToString("N1", voyeur)}").ListToCommaSeparatedValues()}";

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Readiness: {(ConfigurationError() ?? "ready").ColourValue()}");
		sb.AppendLine($"Motives: {Motives.Order().Select(x => x.DescribeEnum()).ListToCommaSeparatedValues().ColourValue()}; implicit self-defence precedes provocation, territory, condition, schedule and hunger.");
		sb.AppendLine($"Active times: {(ActivityWindow.Times.Count == 0 ? "all" : ActivityWindow.Times.Select(x => x.DescribeEnum()).ListToCommaSeparatedValues())}; seasons: {(ActivityWindow.Seasons.Count == 0 ? "all" : ActivityWindow.Seasons.ListToCommaSeparatedValues())}");
		sb.AppendLine($"Calendar: {ActivityWindow.CalendarId}; months: {(ActivityWindow.Months.Count == 0 ? "all" : ActivityWindow.Months.ListToCommaSeparatedValues())}; days: {ActivityWindow.FirstDay?.ToString(actor) ?? "any"}-{ActivityWindow.LastDay?.ToString(actor) ?? "any"}");
		sb.AppendLine($"Moon: {ActivityWindow.MoonId}; phases: {ActivityWindow.Phases.Select(x => x.DescribeEnum()).ListToCommaSeparatedValues()}; condition prog: {ActivityWindow.ConditionProgId}");
		sb.AppendLine($"Defence uses window: {DefenceUsesWindow.ToColouredString()}; trap provokes: {TrapProvokes.ToColouredString()}; provocation: {ProvocationDuration.Describe(actor)}; cooldown: {Cooldown.Describe(actor)}");
		sb.AppendLine($"Allies: same race {SameRaceAllies.ToColouredString()}, prog {AllyProgId}; feeding: {Feeding.DescribeEnum()}, {FeedingBites.ToString("N0", actor)} bites within {FeedingDuration.Describe(actor)}");
		sb.AppendLine($"Home: {HomeStrategy.DescribeEnum()}, location prog {HomeLocationProg?.Id ?? 0}, anchor prog {AnchorItemProg?.Id ?? 0}, craft {_burrowCraftId}; guard radius {GuardRange.ToString("N0", actor)}; return {ReturnHome.ToColouredString()}; warning {WarningDelay.Describe(actor)}");
		sb.AppendLine($"Movement: {MovementStrategy.DescribeEnum()}, range {MovementRange.ToString("N0", actor)}, wander {WanderChancePerMinute.ToString("P1", actor)}; enabled/cell progs {MovementEnabledProg?.Id}/{MovementRoomProg?.Id}; habitats {PreferredHabitatProg?.Id}/{ToleratedHabitatProg?.Id}");
		sb.AppendLine($"Flying/resting layers: {TargetFlyingLayer.DescribeEnum()}/{TargetRestingLayer.DescribeEnum()}; tree layers: {PreferredTreeLayer.DescribeEnum()}/{SecondaryTreeLayer.DescribeEnum()}; aquatic bias {AmphibiousWaterBias.ToString("P1", actor)}");
		sb.AppendLine($"Awareness: {AwarenessStrategy.DescribeEnum()}, {AwarenessRange.ToString("N0", actor)} cells, {AwarenessMemoryMinutes.ToString("N0", actor)} minutes; senses {SensesStrategy.DescribeEnum()}; threat/avoid progs {AwarenessThreatProg?.Id}/{AwarenessAvoidRoomProg?.Id}; refuge {RefugeStrategy.DescribeEnum()}/{RefugeLayer.DescribeEnum()}, prog {RefugeRoomProg?.Id}");
		sb.AppendLine($"Engage delay (ms dice): {EngageDelayDiceExpression}; engage emote: {EngageEmote}; warning emote: {PostureEmote}");
		sb.AppendLine(ShowHunting(actor));
		sb.AppendLine("Needs, equipment, skills and combat powers are configured on the NPC separately.");
		return sb.ToString();
	}
	public string DebugSummary(ICharacter actor, ICharacter? target, IPerceiver voyeur)
	{
		var state = State(actor);
		var hunt = actor.EffectsOfType<MonsterIntentEffect>().FirstOrDefault(x => x.AiId == Id);
		var home = actor.CombinedEffectsOfType<NpcHomeBaseEffect>().FirstOrDefault();
		var sb = new StringBuilder();
		sb.AppendLine($"Monster {Name} (#{Id.ToString("N0", voyeur)}): {ControllerConflict(actor) ?? ConfigurationError() ?? "controller ready"}");
		sb.AppendLine($"Activity: {ActivityWindow.InactiveReason(actor) ?? "active"}; motive: {hunt?.Motive.DescribeEnum() ?? "none"}; last end: {state?.LastEndReason ?? "none"}");
		sb.AppendLine($"Provoker: {state?.ProvokerId ?? 0} until {state?.ProvokedUntil:O}; cooldown: {state?.CooldownUntil:O}; warning: {state?.WarningTargetId ?? 0} until {state?.WarningUntil:O}");
		sb.AppendLine($"Home: {home?.HomeRoom?.Id.ToString(voyeur) ?? "unresolved"}; anchor: {home?.AnchorItem?.Id.ToString(voyeur) ?? "unresolved"}; own paths: {actor.EffectsOfType<FollowingPath>().Count(x => ReferenceEquals(x.PathingOwner, this)).ToString(voyeur)}");
		if ((ReturnHome || Motives.Contains(MonsterMotive.Territory)) && home?.HomeRoom is null && HomeLocationProg is null)
			sb.AppendLine("Bind home location or establish a den before relying on home return or territory defence.");
		sb.AppendLine($"Needs model: {actor.NeedsModel.ModelName}; feeding: {Feeding.DescribeEnum()}; remaining episode bites: {state?.BitesRemaining ?? 0}");
		sb.AppendLine($"Weapons permitted by race: {actor.Race.CombatSettings.CanUseWeapons.ToColouredString()}; wielded items: {actor.Body.WieldedItems.Count().ToString(voyeur)}; unarmed fallback: {actor.CombatSettings.FallbackToUnarmedIfNoWeapon.ToColouredString()}");
		var powers = actor.Powers.OfType<MagicAttackPower>().ToList();
		sb.AppendLine($"Authored combat powers: {powers.Count.ToString(voyeur)}; magic/psionic combat weights: {actor.CombatSettings.MagicUsePercentage.ToString("P1", voyeur)}/{actor.CombatSettings.PsychicUsePercentage.ToString("P1", voyeur)}");
		if (powers.Count == 0 && (actor.CombatSettings.MagicUsePercentage > 0 || actor.CombatSettings.PsychicUsePercentage > 0))
			sb.AppendLine("Combat settings request powers but no compatible combat powers are attached.");
		if (target is not null && CanObserveTarget(actor, target))
			sb.AppendLine($"Currently usable powers for observed target: {powers.Count(x => x.CanInvokePower(actor, target)).ToString(voyeur)}");
		sb.AppendLine(HuntingDiagnostic(actor, target, voyeur));
		return sb.ToString();
	}
}
