#nullable enable

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.Work.Agriculture;

namespace MudSharp.Magic.Environment;

public sealed partial class EnvironmentalMagicCoordinator
{
	public EnvironmentalMagicStateSnapshot InspectState(IRoom room)
	{
		var state = room is Room concrete && room.Id > 0 && ReferenceEquals(room.Gameworld, _world)
			? concrete.EnvironmentState : EnvironmentalMagicState.Empty;
		return new(state, PressureAt(state, UtcNow));
	}

	private static readonly IReadOnlyDictionary<string, double> EmptyInputs =
		new ReadOnlyDictionary<string, double>(new Dictionary<string, double>());

	public EnvironmentalMagicSnapshot Inspect(IRoom room) => Inspect(room, UtcNow);

	private EnvironmentalMagicSnapshot Inspect(IRoom room, DateTimeOffset utcNow, long? onlyResourceId = null)
	{
		if (room is not Room concrete || room.Id <= 0 || !ReferenceEquals(room.Gameworld, _world))
			return new(room.Id, EnvironmentalMagicBindingMode.Disabled, null, null, EnvironmentalMagicState.Empty,
				0.0, EmptyInputs, Array.Empty<EnvironmentalResourceSnapshot>(), Array.Empty<string>());
		var id = EffectiveProfileId(concrete);
		if (concrete.PendingEnvironmentalOperationId is { } pending)
			return FailedSnapshot(concrete, id, $"Environmental operation {pending} has an unconfirmed persistence outcome; retry only that operation identity to resolve it.");
		if (!id.HasValue)
			return new(room.Id, concrete.EnvironmentBindingMode, null, null, concrete.EnvironmentState,
				PressureAt(concrete.EnvironmentState, utcNow), EmptyInputs, Array.Empty<EnvironmentalResourceSnapshot>(), Array.Empty<string>());
		if (!_evaluating.Add(room.Id))
		{
			_recursive.Add(room.Id);
			return FailedSnapshot(concrete, id, "Recursive environmental input/cap evaluation is not permitted.");
		}
		try
		{
			_lastEvaluations++;
			_totalEvaluations++;
			var profile = Profile(id.Value);
			if (profile is null) return FailedSnapshot(concrete, id, $"Environmental profile #{id} is missing or has the wrong type.");
			if (profile.ValidationErrors.Count > 0)
				return FailedSnapshot(concrete, id, string.Join("; ", profile.ValidationErrors));
			var state = concrete.EnvironmentState;
			if (state.SchemaVersion != 1 || !double.IsFinite(state.ScarDamage) || state.ScarDamage < 0 ||
				!double.IsFinite(state.RecentPressure) || state.RecentPressure < 0)
				return FailedSnapshot(concrete, id, "The persisted environmental state is invalid or has an unsupported version.");
			var pressure = PressureAt(state, utcNow);
			if (!double.IsFinite(pressure)) return FailedSnapshot(concrete, id, "Recent pressure could not be calculated.");
			var values = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
			{
				["scardamage"] = state.ScarDamage,
				["pressure"] = pressure,
				["hasdefile"] = state.LastDefileUtc.HasValue ? 1.0 : 0.0,
				["minutessincedefile"] = state.LastDefileUtc.HasValue ? Math.Max(0.0, (utcNow - state.LastDefileUtc.Value).TotalMinutes) : 0.0
			};
			IAgricultureField? field = null;
			var fieldRead = false;
			var requiredInputs = onlyResourceId.HasValue
				? profile.RequiredOutputInputNames(onlyResourceId.Value) : profile.RequiredInputNames;
			foreach (var input in profile.Inputs)
			{
				if (!requiredInputs.Contains(input.Name)) continue;
				double value;
				switch (input.Kind)
				{
					case EnvironmentalMagicInputKind.Forage:
						if (!room.HasForagableProfile) value = 0.0;
						else if (!room.TryPeekForagableYield(input.Source, out value))
							return FailedSnapshot(concrete, id, $"Forage input {input.Name} refers to missing yield '{input.Source}'.");
						break;
					case EnvironmentalMagicInputKind.Agriculture:
						if (!fieldRead) { field = FieldFor(room); fieldRead = true; }
						value = AgricultureValue(field, input.Source);
						break;
					case EnvironmentalMagicInputKind.Prog:
						if (input.Prog is null || input.Prog.StaticType != FutureProgStaticType.NotStatic)
							return FailedSnapshot(concrete, id, $"Input {input.Name} requires a non-static numeric location prog.");
						var watch = Stopwatch.StartNew();
						_lastProgExecutions++;
						_totalProgExecutions++;
						var success = input.Prog.ExecuteWithStatus(out var result, room);
						if (watch.Elapsed.TotalMilliseconds >= Options.SlowProgMilliseconds)
							RecordSlowProg(id.Value, input.Prog.Id, watch.Elapsed.TotalMilliseconds);
						if (!success || result is null)
							return FailedSnapshot(concrete, id, $"Input prog #{input.Prog.Id} failed.");
						value = Convert.ToDouble(result, CultureInfo.InvariantCulture);
						break;
					default:
						return FailedSnapshot(concrete, id, $"Input {input.Name} has an unsupported kind.");
				}
				value *= input.Scale;
				if (!double.IsFinite(value)) return FailedSnapshot(concrete, id, $"Input {input.Name} is not finite.");
				values[input.Name] = value;
			}
			var immutable = new ReadOnlyDictionary<string, double>(values);
			var outputs = new List<EnvironmentalResourceSnapshot>(profile.Outputs.Count);
			var errors = new List<string>();
			foreach (var output in profile.Outputs)
			{
				if (onlyResourceId.HasValue && output.ResourceId != onlyResourceId.Value) continue;
				var balance = output.Resource is null ? 0.0 : room.MagicResourceAmounts.GetValueOrDefault(output.Resource);
				var calculation = profile.EvaluateOutput(output, immutable, balance);
				outputs.Add(new(output.ResourceId, output.Resource?.Name ?? $"Missing #{output.ResourceId}", balance,
					calculation.IsValid, calculation.Maximum, calculation.Rate, calculation.Error));
				if (!calculation.IsValid) errors.Add(calculation.Error ?? "Invalid environmental calculation.");
			}
			if (_recursive.Contains(room.Id)) errors.Add("Recursive environmental input/cap evaluation is not permitted.");
			if (errors.Count > 0)
				outputs = outputs.Select(x => x with { IsValid = false, Maximum = double.NaN, Rate = double.NaN, Error = string.Join("; ", errors) }).ToList();
			return new(room.Id, concrete.EnvironmentBindingMode, id, profile.Name, state, pressure,
				immutable, outputs.AsReadOnly(), errors.AsReadOnly());
		}
		catch (Exception ex)
		{
			return FailedSnapshot(concrete, id, $"Environmental calculation failed: {ex.Message}");
		}
		finally
		{
			_evaluating.Remove(room.Id);
			_recursive.Remove(room.Id);
		}
	}

	private EnvironmentalMagicSnapshot FailedSnapshot(Room room, long? id, string error) =>
		new(room.Id, room.EnvironmentBindingMode, id, null, room.EnvironmentState, double.NaN, EmptyInputs,
			Array.Empty<EnvironmentalResourceSnapshot>(), new[] { error });

	private static double AgricultureValue(IAgricultureField? field, string source) => source.ToLowerInvariant() switch
	{
		"hasfield" => field is null ? 0 : 1,
		"hascrop" => field?.CurrentCrop is null ? 0 : 1,
		"haswoodland" => field?.CurrentWoodland is null ? 0 : 1,
		"crophealth" => field?.CurrentCrop is null ? 0 : field.CropHealth,
		"cropyieldpotential" => field?.CurrentCrop is null ? 0 : field.CropYieldPotential,
		"woodlandhealth" => field?.CurrentWoodland is null ? 0 : field.WoodlandHealth,
		"woodlandyieldpotential" => field?.CurrentWoodland is null ? 0 : field.WoodlandYieldPotential,
		"pasture" => field?.Pasture ?? 0,
		"fieldcondition" => field?.Condition ?? 0,
		_ => throw new ArgumentException($"Unknown agriculture input '{source}'.")
	};

	public bool TryInspectResource(IRoom room, IMagicResource resource, out EnvironmentalResourceSnapshot result)
	{
		result = null!;
		if (room is not Room concrete || EffectiveProfileId(concrete) is not { } id) return false;
		var profile = Profile(id);
		if (profile is not null && !profile.Outputs.Any(x => x.ResourceId == resource.Id)) return false;
		var snapshot = Inspect(room);
		result = snapshot.Outputs.FirstOrDefault(x => x.ResourceId == resource.Id) ??
			new EnvironmentalResourceSnapshot(resource.Id, resource.Name, room.MagicResourceAmounts.GetValueOrDefault(resource),
				false, double.NaN, double.NaN, string.Join("; ", snapshot.Errors));
		return true;
	}

	public bool TryInspectLandResource(IRoom room, IMagicResource resource,
		out EnvironmentalResourceSnapshot result)
	{
		result = null!;
		if (room is not Room concrete || EffectiveProfileId(concrete) is not { } id) return false;
		var profile = Profile(id);
		if (profile is null || !profile.Outputs.Any(x => x.ResourceId == resource.Id)) return false;
		var snapshot = Inspect(room, UtcNow, resource.Id);
		result = snapshot.Outputs.FirstOrDefault(x => x.ResourceId == resource.Id) ??
			new EnvironmentalResourceSnapshot(resource.Id, resource.Name,
				room.MagicResourceAmounts.GetValueOrDefault(resource), false, double.NaN, double.NaN,
				string.Join("; ", snapshot.Errors));
		return true;
	}

	private double PressureAt(EnvironmentalMagicState state, DateTimeOffset utcNow)
	{
		if (state.RecentPressure == 0.0) return 0.0;
		if (state.PressureProfileId is { } id && _world.MagicResourceRegenerators.Get(id) is IEnvironmentalMagicProfile profile)
			return state.RecentPressure * Math.Pow(2.0, -Math.Max(0.0, profile.PressureDecayIntegralAt(utcNow) - state.PressureDecayAnchor));
		if (!state.PressureReferenceUtc.HasValue || !double.IsFinite(state.PressureHalfLifeSeconds) || state.PressureHalfLifeSeconds <= 0.0)
			return double.NaN;
		return state.RecentPressure * Math.Pow(2.0,
			-Math.Max(0.0, (utcNow - state.PressureReferenceUtc.Value).TotalSeconds) / state.PressureHalfLifeSeconds);
	}

	private EnvironmentalMagicState PressureAnchor(EnvironmentalMagicState state, IEnvironmentalMagicProfile? profile,
		double pressure, DateTimeOffset utcNow) => state with
	{
		RecentPressure = pressure,
		PressureReferenceUtc = utcNow,
		PressureProfileId = profile?.Id,
		PressureDecayAnchor = profile?.PressureDecayIntegralAt(utcNow) ?? 0.0,
		PressureHalfLifeSeconds = profile?.PressureHalfLifeSeconds ?? state.PressureHalfLifeSeconds
	};

	private void SettlePressure(Room room, IEnvironmentalMagicProfile? profile)
	{
		var state = room.EnvironmentState;
		if (state.RecentPressure == 0.0 || state.PressureProfileId == profile?.Id &&
			(profile is null || state.PressureHalfLifeSeconds == profile.PressureHalfLifeSeconds)) return;
		var utcNow = UtcNow;
		var pressure = PressureAt(state, utcNow);
		if (!double.IsFinite(pressure)) return;
		if (room.SetEnvironmentState(PressureAnchor(state, profile, pressure, utcNow) with { Revision = state.Revision + 1 })) CountWrite();
	}

	private void CountWrite()
	{
		_lastWrites++;
		_totalWrites++;
	}

	private void RecordSlowProg(long profileId, long progId, double milliseconds)
	{
		_totalSlowProgs++;
		_slowProg = $"Profile #{profileId}, prog #{progId}: {milliseconds:F3} ms (synchronous; not preempted)";
		var now = Now;
		if (_lastLoggedSlow.TryGetValue(profileId, out var last) && now - last < 60.0) return;
		_lastLoggedSlow[profileId] = now;
		_world.SystemMessage($"Environmental magic slow input: {_slowProg}", true);
	}
}
