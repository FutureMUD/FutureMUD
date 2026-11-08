#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallItemFlavour()
	{
		var sources = _source.Items.Where(x => x.Programs.Contains("C pulse vines_wiggle") && _items.ContainsKey(x.Vnum) &&
			_source.Resets.Any(r => r.ReferenceIssues.Length == 0 && r.Opcode is "O" or "P" or "G" or "E" &&
				r.Arguments.Length > 1 && r.Arguments[1] == x.Vnum)).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var families = string.Join(" or ", sources.Select(ItemVariantIdExpression));
		var pulse = Prog("ArmageddonWorldfileVinesPulse", $$"""
			foreach (x in loadeditems())
				if ({{families}})
					if (not(isnull(@x.location)))
						if (contains(@x.location.items, @x))
							var roll = random(1, 25)
							if (@roll == 1)
								sendloc(@x.location, "Something rustles against the ground.")
							end if
							if (@roll == 2)
								sendloc(@x.location, "You notice movement out of the corner of your eye.")
							end if
						end if
					end if
				end if
			end foreach
			return true
			""", ProgVariableTypes.Boolean);
		var (schedule, rate) = ScheduleSourceObjectPulse(pulse, "Armageddon worldfile tubular vines");
		Record("item-special.vines-wiggle", nameof(ProgSchedule), schedule.Id, null, new
		{
			Sources = sources, Prog = pulse.Id, NativeMinutes = schedule.IntervalModifier, NativeClockRate = rate,
			Disposition = "Only retained reset-backed C pulse vines_wiggle attachments. One native scheduled prog scans a snapshot of already loaded items, including original and reset variants. Ground instances alone roll; carried/contained/unloaded items do not emit. Source outcomes 1 and 2 of 25 retain their exact text; other outcomes stay silent. Nominal source thirty-real-second object cadence is converted to whole native clock minutes, rounded up (exactly three minutes at 6:1). No offline loads, fabricated get/break DMPL, NPC hooks or engine changes."
		});
		_counts["item flavour schedules"] = 1;
	}

	private (ProgSchedule Schedule, double ClockRate) ScheduleSourceObjectPulse(FutureProg prog, string name)
	{
		var calendar = _db.Calendars.Single(x => x.Id == _db.Cultures.Single(c => c.Id ==
			_settings.NativeBindings[ArmageddonNativeBinding.Culture]).PrimaryCalendarId);
		var clock = _db.Clocks.Single(x => x.Id == calendar.FeedClockId);
		var definition = XElement.Parse(clock.Definition);
		var rate = SourceLawClockRate();
		var secondsPerMinute = (int)definition.Element("SecondsPerMinute")!;
		if (secondsPerMinute <= 0) throw new InvalidOperationException("Item pulses require a positive clock minute length.");
		// Source PULSE_OBJECT=40 +/-5 heap units is nominally about thirty real seconds.
		// Native schedules use whole clock minutes, rounding up only when necessary.
		var minutes = checked((int)Math.Max(1, Math.Ceiling(30.0 * rate / secondsPerMinute)));
		var timezone = _db.Timezones.Single(x => x.Id == clock.PrimaryTimezoneId);
		var schedule = new ProgSchedule { Id = Next<ProgSchedule>(), Name = name,
			IntervalType = (int)IntervalType.Minutely, IntervalModifier = minutes, FutureProgId = prog.Id,
			ReferenceDate = $"{calendar.Id}_{calendar.Date}_{clock.Id}_{timezone.Name} {clock.Hours}:{clock.Minutes}:{clock.Seconds}",
			ReferenceTime = string.Empty };
		_db.ProgSchedules.Add(schedule);
		return (schedule, rate);
	}
}
