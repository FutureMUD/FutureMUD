#nullable enable

using System;
using System.Linq;
using CultureInfo = System.Globalization.CultureInfo;
using System.Text;
using System.Xml.Linq;
using MudSharp.Climate;
using MudSharp.FutureProg;
using MudSharp.Models;
using DatabaseSeeder.Seeders.Utilities;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallSandstorms()
	{
		var sources = _source.Npcs.Where(x => x.Programs.Contains("C pulse spec_roving_sandstorm")).Select(x => x.Vnum).ToArray();
		if (sources.Length == 0) return;
		var hub = _rooms[48000];
		var starts = new[] { 48167, 48118, 48123 };
		var dust = WeatherResidueSeeder.RequireDust(_db);
		var weather = new WeatherEvent
		{
			Id = Next<WeatherEvent>(), Name = "Worldfile Red Storm Sandstorm", WeatherEventType = "simple",
			WeatherDescription = "Howling winds drive a whirling sandstorm across the land.",
			WeatherRoomAddendum = "Howling winds whip sand and dust through the air.",
			Wind = (int)WindLevel.GaleWind, Precipitation = (int)PrecipitationLevel.Parched,
			LightLevelMultiplier = 1, ObscuresViewOfSky = true,
			PermittedAtDawn = true, PermittedAtMorning = true, PermittedAtAfternoon = true, PermittedAtDusk = true, PermittedAtNight = true,
			AdditionalInfo = new XElement("Info", new XElement("Echoes"), new XElement("Transitions", new XElement("Default", "The winds pick up without warning, triggering a whirling sandstorm."))).ToString()
		};
		_db.WeatherEvents.Add(weather);
		for (var i = 0; i < starts.Length; i++)
		{
			var key = "armw_sandstorm_" + i;
			var owner = ProgVariableTypes.Location.ToStorageString();
			if (_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == key))
				throw new InvalidOperationException($"Register {key} already exists.");
			_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = key, ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
			_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = key, DefaultValue = $"<var>{_rooms[starts[i]].Id}</var>" });
		}
		var nextCode = new StringBuilder();
		foreach (var room in _source.Rooms.Where(x => x.Zone == 48 && _rooms.ContainsKey(x.Vnum)).OrderBy(x => x.Vnum))
		{
			var direction = SandstormSourceDirection(room.Vnum);
			if (direction is null) continue;
			var alternate = direction switch { 1 => 0, 0 => 3, 3 => 2, _ => direction.Value };
			string Destination(int d) => room.Exits.FirstOrDefault(x => x.Direction == d) is { } exit && _rooms.TryGetValue(exit.Destination, out var destination)
				? $"tolocation({destination.Id})" : "@room";
			nextCode.AppendLine($"if (@room.id == {_rooms[room.Vnum].Id})\nif (@turn)\nreturn {Destination(alternate)}\nend if\nreturn {Destination(direction.Value)}\nend if");
		}
		nextCode.AppendLine("return null(\"location\")");
		var next = Prog("ArmageddonWorldfileSandstormNext", nextCode.ToString(), ProgVariableTypes.Location,
			("room", ProgVariableTypes.Location), ("turn", ProgVariableTypes.Boolean));
		var initialise = new StringBuilder($"var hub = tolocation({hub.Id})\n");
		for (var i = 0; i < starts.Length; i++)
			initialise.AppendLine($"var room{i} = tolocation(getregister(@hub, \"armw_sandstorm_{i}\"))\nif (not(isnull(@room{i})))\nsetroomweather(@room{i}, {weather.Id})\nend if");
		initialise.AppendLine("return true");
		var restore = Prog("ArmageddonWorldfileSandstormRestore", initialise.ToString(), ProgVariableTypes.Boolean);
		var pulse = new StringBuilder($"if (not(@npc.isawake) or @npc.incombat)\nreturn true\nend if\n@{restore.FunctionName}()\nif (random(1, 3) == 1)\nreturn true\nend if\nvar hub = tolocation({hub.Id})\n");
		for (var i = 0; i < starts.Length; i++)
		{
			pulse.AppendLine($$"""
				var room{{i}} = tolocation(getregister(@hub, "armw_sandstorm_{{i}}"))
				if (not(isnull(@room{{i}})))
					depositroomresidue(@room{{i}}, {{dust.Id}}, {{(0.0001 * _kilograms).ToString("R", CultureInfo.InvariantCulture)}}, {{(0.005 * _kilograms).ToString("R", CultureInfo.InvariantCulture)}})
					if (random(1, 75) == 1)
						var deposit{{i}} = loaditem({{_items[48009].Id}})
						if (not(put(@deposit{{i}}, @room{{i}})))
							deleteitem(@deposit{{i}})
						end if
					end if
					if (random(1, 100) <= 20)
						var next{{i}} = @{{next.FunctionName}}(@room{{i}}, random(1, 3) == 1)
						if (@next{{i}} != @room{{i}})
							setroomweather(@room{{i}}, 0)
							sendloc(@room{{i}}, "The howling winds decrease slightly and the storm subsides.")
							if (isnull(@next{{i}}))
								setregister @hub "armw_sandstorm_{{i}}" {{_rooms[48165].Id}}
							else
								setregister @hub "armw_sandstorm_{{i}}" @next{{i}}.id
								setroomweather(@next{{i}}, {{weather.Id}})
								sendloc(@next{{i}}, "The winds pick up without warning, triggering a whirling sandstorm.")
							end if
							return true
						end if
					end if
				end if
				""");
		}
		pulse.AppendLine("return true");
		var controller = Prog("ArmageddonWorldfileSandstormController", pulse.ToString(), ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
		InstallNpcTickHook("Roving Sandstorms", controller, sources);
		foreach (var source in sources) AppendNpcOnLoadCode(source, $"@{restore.FunctionName}()");
		// Boot maintenance restores effects even when the controller survives a restart.
		foreach (var boot in _progs.Where(x => x.FunctionName == "ArmageddonWorldfileZone48Boot"))
			boot.FunctionText = boot.FunctionText.Insert(boot.FunctionText.LastIndexOf("return true", StringComparison.Ordinal), $"@{restore.FunctionName}()\n");
		foreach (var item in _effectiveItems.Where(x => x.Value.Source.Vnum == 48009))
		{
			var prototype = _db.GameItemProtos.Local.Single(x => x.Id == item.Key);
			prototype.MorphTimeSeconds = 900;
			prototype.MorphGameItemProtoId = null;
			prototype.MorphEmote = "$0 is swept away by the wind.";
		}
		Record("npc-special.roving-sandstorms", nameof(FutureProg), controller.Id, null, new
		{
			Sources = sources, Hub = hub.Id, Starts = starts, WeatherEvent = weather.Id, DustMaterial = dust.Id, Deposit = _items[48009].Id,
			Disposition = "A source-specific prog on the native ten-second tick, gated at 2/3 for a nominal 15-second eligible cadence. Three saved room registers retain positions. Source ordered movement, paths, 1/3 turns, 20% moves and 1/75 deposits are retained. Local saved weather provides prose/gale wind; the controller directly deposits native dust on bodies/exposed belongings independently of regional weather. No invented lightning, damage, toxin or direct sight penalty. Native cleaning removes residue; authored deposition is 0.1g per reached pulse capped at 5g per recipient. First move ends the pulse as in source. Missing authored exits simply hold position. Native container morph removes deposits after approved 15 real minutes and handles contents. No source-backed sifting implementation exists; raw debug-coordinate talk is not installed."
		});
		_counts["roving sandstorm controllers"] = sources.Length;
	}

	internal static int? SandstormSourceDirection(int vnum) => vnum switch
	{
		>= 48165 and < 48170 => 1,
		48170 or >= 48134 and <= 48140 or >= 48144 and <= 48150 or >= 48154 and <= 48160 => 0,
		>= 48104 and <= 48110 or >= 48114 and <= 48120 or >= 48124 and <= 48130 => 3,
		< 48154 => 2,
		_ => null
	};
}
