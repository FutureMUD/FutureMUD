#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string PlantPulseCounter = "armw_plant_pulses";
	private static readonly IReadOnlyDictionary<int, int> SourcePlantFruit = new Dictionary<int, int>
	{
		[25015] = 25309, [45383] = 45384, [65940] = 50306,
		[56101] = 56102, [56103] = 56104, [56105] = 56106, [56108] = 56109, [56110] = 56111,
		[56113] = 56114, [56115] = 56116, [56117] = 56118, [56120] = 56121, [56122] = 56123,
		[56124] = 56125, [56128] = 56129, [56131] = 56132, [56133] = 56134, [56136] = 56137, [56138] = 56139
	};

	private void InstallPlantGrowth()
	{
		var sources = _source.Items.Where(x => x.Programs.Contains("C pulse generic_plant") && _items.ContainsKey(x.Vnum) &&
			SourcePlantFruit.TryGetValue(x.Vnum, out var fruit) && _items.ContainsKey(fruit) &&
			_source.Resets.Any(r => r.ReferenceIssues.Length == 0 && r.Opcode is "O" or "P" or "G" or "E" &&
				r.Arguments.Length > 1 && r.Arguments[1] == x.Vnum)).Select(x => x.Vnum).ToHashSet();
		if (sources.Count == 0) return;
		var owner = ProgVariableTypes.Item.ToStorageString();
		if (_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == PlantPulseCounter))
			throw new InvalidOperationException($"Register {PlantPulseCounter} already exists.");
		_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = PlantPulseCounter,
			ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
		_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = PlantPulseCounter, DefaultValue = "<var>-1</var>" });
		var kilogram = _kilograms.ToString("0.########################", CultureInfo.InvariantCulture);
		var grow = Prog("ArmageddonWorldfileGrowPlant", $$"""
			var pulse = getregister(@plant, "{{PlantPulseCounter}}")
			if (@pulse < 5)
				var nextpulse = @pulse + 1
				setregister @plant "{{PlantPulseCounter}}" @nextpulse
				return false
			end if
			setregister @plant "{{PlantPulseCounter}}" 0
			if (isnull(@plant.location))
				return false
			end if
			if (not(contains(@plant.location.items, @plant)))
				return false
			end if
			var fruit = loaditem(@fruitproto)
			var weight = floor(@plant.weight / {{kilogram}})
			foreach (content in @plant.contents)
				var contentkg = @content.weight / {{kilogram}}
				var units = @contentkg * 100
				if (@units > 0 and @units <= 1)
					weight = @weight + 1
				else
					weight = floor(@weight + @units)
				end if
			end foreach
			var fruitkg = @fruit.weight / {{kilogram}}
			var fruitunits = @fruitkg * 100
			var candidateweight = @weight + @fruitunits
			if (@candidateweight >= @capacity)
				deleteitem(@fruit)
				return false
			end if
			if (not(forceput(@fruit, @plant)))
				deleteitem(@fruit)
				return false
			end if
			return true
			""", ProgVariableTypes.Boolean, ("plant", ProgVariableTypes.Item), ("fruitproto", ProgVariableTypes.Number), ("capacity", ProgVariableTypes.Number));
		var controller = new StringBuilder($"foreach (plant in loadeditems())\nif (getregister(@plant, \"{PlantPulseCounter}\") >= 0)\n");
		var profiles = _effectiveItems.Where(x => sources.Contains(x.Value.Source.Vnum)).ToArray();
		foreach (var profile in profiles)
		{
			var initial = Math.Min(5, profile.Value.Source.Physical[1]);
			var prototype = _db.GameItemProtos.Local.Single(x => x.Id == profile.Key);
			AddItemOnLoadProg(prototype, "PlantGrowthState", $"setregister @item \"{PlantPulseCounter}\" {initial}\nreturn true");
		}
		foreach (var group in profiles.GroupBy(x => (Fruit: SourcePlantFruit[x.Value.Source.Vnum], Capacity: x.Value.Values[0])))
			controller.AppendLine("if (" + string.Join(" or ", group.Select(x => $"@plant.proto == {x.Key}")) +
				$")\n@{grow.FunctionName}(@plant, {_items[group.Key.Fruit].Id}, {group.Key.Capacity})\nend if");
		controller.AppendLine("end if\nend foreach\nreturn true");
		var pulseProg = Prog("ArmageddonWorldfilePlantPulse", controller.ToString(), ProgVariableTypes.Boolean);
		var (schedule, rate) = ScheduleSourceObjectPulse(pulseProg, "Armageddon worldfile plant growth");
		Record("item-special.generic-plant", nameof(ProgSchedule), schedule.Id, null, new
		{
			Sources = sources.Order().ToArray(), NativeMinutes = schedule.IntervalModifier, NativeClockRate = rate,
			Profiles = profiles.Select(x => new { Prototype = x.Key, Source = x.Value.Source.Vnum, Fruit = SourcePlantFruit[x.Value.Source.Vnum],
				Capacity = x.Value.Values[0], InitialCounter = Math.Min(5, x.Value.Source.Physical[1]) }),
			Disposition = "Only mapped, retained reset-backed C pulse generic_plant definitions. One saved item counter starts from source price/counter phase without changing native prices; source cost>=5 attempts on pulse one, zero on pulse six, then every sixth pulse. Source truncated mixed-unit weight and strict capacity threshold are retained against native weights. Native forceput performs natural growth inside the plant after that source-specific budget, independent of player insertion capacity/open state; failed placements delete only the candidate. Counters advance while carried, but no fruit grows without direct ground placement. No offline loads, unmapped Greelza, unplaced Kalan or missing Bimbal reconstruction."
		});
		_counts["fruiting plant definitions"] = sources.Count;
	}
}
