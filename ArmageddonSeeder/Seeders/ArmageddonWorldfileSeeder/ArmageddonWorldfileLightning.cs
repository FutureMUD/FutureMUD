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
	private const string StormWindDirectionRegister = "armw_storm_winddir";
	private const string StormPressureRegister = "armw_storm_pressure";

	private void InstallLightning()
	{
		const int cloudVnum = 950;
		var cloudSource = _source.Items.SingleOrDefault(x => x.Vnum == cloudVnum && x.Programs.Contains("C pulse storm_cloud"));
		if (cloudSource is null || !_items.TryGetValue(cloudVnum, out var cloudProto) || !_rooms.TryGetValue(48000, out var controllerRoom) ||
			!_source.Resets.Any(x => x.ReferenceIssues.Length == 0 && x.Opcode is "O" or "P" or "G" or "E" &&
				x.Arguments.Length > 1 && x.Arguments[1] == cloudVnum))
		{
			return;
		}

		AddLocationRegister(StormWindDirectionRegister, "0");
		AddLocationRegister(StormPressureRegister, "1");

		var movement = BuildStormCloudMovement();
		var cloudFamily = ItemVariantIdExpression(cloudVnum).Replace("@x.proto", "@cloud.proto", StringComparison.Ordinal);
		var pulse = Prog("ArmageddonWorldfileStormCloudPulse", $$"""
			var hub = tolocation({{controllerRoom.Id}})
			var wind = getregister(@hub, "{{StormWindDirectionRegister}}")
			foreach (cloud in loadeditems())
				if ({{cloudFamily}} and not(isnull(@cloud.location)))
					if (contains(@cloud.location.items, @cloud))
						var room = @cloud.location
						var outcome = random(1, 20)
						if (@outcome > 5)
							var eligible = 0
							foreach (target in @room.characters)
								if (not(isadmin(@target)))
									eligible = @eligible + 1
								end if
							end foreach
							if (@eligible > 0)
								var selected = random(1, @eligible)
								var index = 0
								var struck = false
								foreach (target in @room.characters)
									if (not(@struck) and not(isadmin(@target)))
										index = @index + 1
										if (@index == @selected)
											var damage = random(5, 15)
											if (random(0, 10) == 0)
												damage = random(10, 35)
											end if
											foreach (observer in @room.characters)
												if (@observer != @target)
													send(@observer, "A bolt of lightning shoots from a black storm cloud, striking $0.", @target)
												end if
											end foreach
											send(@target, "A bolt of lightning shoots from a black storm cloud, striking you.")
											wound(@target, "electrical", @damage)
											struck = true
										end if
									end if
								end foreach
							end if
						end if
						if (@outcome == 1)
							sendloc(@room, "Lightning flashes from within a black storm cloud.")
						end if
						if (@outcome == 2)
							sendloc(@room, "A black storm cloud flashes and thunders angrily.")
						end if
						if (@outcome == 3)
							sendloc(@room, "A deep rumbling can be heard within a black storm cloud.")
						end if
						if (@outcome == 4)
							{{movement}}
						end if
						if (@outcome == 5)
							sendloc(@room, "A black storm cloud swirls violently.")
						end if
					end if
				end if
			end foreach
			return true
			""", ProgVariableTypes.Boolean);
		var (pulseSchedule, rate) = ScheduleSourceObjectPulse(pulse, "Armageddon worldfile storm cloud pulse");
		var weatherUpdate = Prog("ArmageddonWorldfileStormWeatherUpdate", $$"""
			var hub = tolocation({{controllerRoom.Id}})
			var wind = getregister(@hub, "{{StormWindDirectionRegister}}")
			var pressure = getregister(@hub, "{{StormPressureRegister}}") + random(50, 250) - 100
			if (@pressure < 1)
				pressure = 1
			end if
			if (@pressure >= 1000)
				var change = random(1, 3)
				wind = (@wind + @change) % 4
				pressure = 1
			end if
			setregister @hub "{{StormWindDirectionRegister}}" @wind
			setregister @hub "{{StormPressureRegister}}" @pressure
			return true
			""", ProgVariableTypes.Boolean);
		var updateSchedule = ScheduleSourceWeatherUpdate(weatherUpdate, "Armageddon worldfile storm weather update");
		Record("item-special.storm-cloud", nameof(ProgSchedule), pulseSchedule.Id, null, new
		{
			CloudSource = cloudVnum, Source = cloudSource.Source, CloudPrototype = cloudProto.Id,
			ControllerRoom = controllerRoom.Id, WindDirectionRegister = StormWindDirectionRegister,
			PressureRegister = StormPressureRegister, PulseProg = pulse.Id,
			PulseNativeMinutes = pulseSchedule.IntervalModifier, SourceClockRate = rate,
			WeatherProg = weatherUpdate.Id, WeatherSchedule = updateSchedule.Id,
			WeatherNativeMinutes = updateSchedule.IntervalModifier,
			Disposition = "Only the retained reset-backed source object 950 (C pulse storm_cloud) is driven. Native pulses retain the 1..20 source outcomes: 6..20 strike one random non-admin room character, 1 flash, 2 thunder, 3 rumble, 4 moves, 5 swirls. Strikes use the source normal 5..15 damage range with 1-in-11 high 10..35 damage, applied through native electrical wounds. Movement chooses among open cardinal exits whose destination rooms are outdoors, excluding the reverse of the saved wind direction. Setting wind direction and pressure persist as registers on the retained zone 48 controller room; source-hour weather updates change pressure by random(50..250)-100, floor it at 1, and on reaching 1000 choose a different cardinal direction and reset pressure to 1. No private historical source data, global weather engine changes, or invented climate policy."
		});
		_counts["storm cloud schedules"] = 2;

		void AddLocationRegister(string key, string defaultValue)
		{
			var owner = ProgVariableTypes.Location.ToStorageString();
			if (_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == key))
				throw new InvalidOperationException($"Register {key} already exists.");
			_db.VariableDefinitions.Add(new VariableDefinition
			{
				OwnerTypeDefinition = owner, Property = key, ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString()
			});
			_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = key, DefaultValue = $"<var>{defaultValue}</var>" });
		}
	}

	private string BuildStormCloudMovement()
	{
		return """
			var reverse = ""
			if (@wind == 0)
				reverse = "south"
			end if
			if (@wind == 1)
				reverse = "west"
			end if
			if (@wind == 2)
				reverse = "north"
			end if
			if (@wind == 3)
				reverse = "east"
			end if
			var exits = @room.exits.Where(exit, @exit.destination.outdoors == 2 and lowercase(@exit.direction) != @reverse and (lowercase(@exit.direction) == "north" or lowercase(@exit.direction) == "east" or lowercase(@exit.direction) == "south" or lowercase(@exit.direction) == "west"))
			var exitCount = @exits.Count(exit, true)
			if (@exitCount > 0)
				var passage = getindex(@exits, random(0, @exitCount - 1))
				var direction = lowercase(@passage.direction)
				var from = ""
				if (@direction == "north")
					from = "south"
				end if
				if (@direction == "east")
					from = "west"
				end if
				if (@direction == "south")
					from = "north"
				end if
				if (@direction == "west")
					from = "east"
				end if
				sendloc(@room, "A black storm cloud floats to the " + @direction + ".")
				put(@cloud, @passage.destination)
				sendloc(@cloud.location, "A black storm cloud floats in from the " + @from + ".")
			end if
			""";
	}

	private ProgSchedule ScheduleSourceWeatherUpdate(FutureProg prog, string name)
	{
		var calendar = _db.Calendars.Single(x => x.Id == _db.Cultures.Single(c => c.Id ==
			_settings.NativeBindings[ArmageddonNativeBinding.Culture]).PrimaryCalendarId);
		var clock = _db.Clocks.Single(x => x.Id == calendar.FeedClockId);
		var definition = XElement.Parse(clock.Definition);
		var rate = SourceLawClockRate();
		var secondsPerMinute = (int)definition.Element("SecondsPerMinute")!;
		if (secondsPerMinute <= 0) throw new InvalidOperationException("Weather updates require a positive clock minute length.");
		var minutes = checked((int)Math.Max(1, Math.Ceiling(300.0 * rate / secondsPerMinute)));
		var timezone = _db.Timezones.Single(x => x.Id == clock.PrimaryTimezoneId);
		var schedule = new ProgSchedule
		{
			Id = Next<ProgSchedule>(), Name = name, IntervalType = (int)IntervalType.Minutely,
			IntervalModifier = minutes, FutureProgId = prog.Id,
			ReferenceDate = $"{calendar.Id}_{calendar.Date}_{clock.Id}_{timezone.Name} {clock.Hours}:{clock.Minutes}:{clock.Seconds}",
			ReferenceTime = string.Empty
		};
		_db.ProgSchedules.Add(schedule);
		return schedule;
	}

}
