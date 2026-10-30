#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;

namespace MudSharp.Health.Infections;

/// <summary>A body-owned illness whose definition is an Infection drug-effect profile.</summary>
public sealed class ConfiguredInfection : Infection, IDrugSymptomInfection
{
	private sealed record State(long DefinitionDrugId, string? SourceFamily, double RemainingSeconds);
	private double _spreadSeconds;
	private DateTime _lastTick = DateTime.UtcNow;
	private bool _completed;
	public long DefinitionDrugId { get; }
	public string? SourceFamily { get; }
	public double RemainingSeconds { get; private set; }
	private DrugInfectionAdditionalInfo? Definition => Gameworld.Drugs.Get(DefinitionDrugId)?
		.AdditionalInfoFor<DrugInfectionAdditionalInfo>(DrugType.Infection);
	public IDrug? SymptomDrug => Definition is { } info ? Gameworld.Drugs.Get(info.SymptomDrugId) : null;
	public double SymptomIntensity => _completed ? 0.0 : Intensity * (Definition?.SymptomIntensityMultiplier ?? 0.0);
	public bool CureWithDrug => Definition?.CureWithDrug == true;
	public override string Name => Definition?.Name ?? "an illness";
	public override InfectionType InfectionType => InfectionType.Configured;
	public override InfectionStage InfectionStage { get; set; } = InfectionStage.StageOne;
	public override double Pain => 0;

	public ConfiguredInfection(IBody owner, IDrug definitionDrug, DrugInfectionAdditionalInfo definition, string? family)
		: base((Difficulty)definition.VirulenceDifficulty, definition.InitialIntensity, owner, null!, null!, definition.VirulenceMultiplier)
	{
		DefinitionDrugId = definitionDrug.Id;
		SourceFamily = family;
		RemainingSeconds = definition.DurationSeconds;
	}

	public ConfiguredInfection(MudSharp.Models.Infection infection, IBody body) : base(infection, body, null!, null!)
	{
		var state = JsonSerializer.Deserialize<State>(infection.ExtraInformation ?? "null") ??
			throw new InvalidOperationException("A configured infection has no definition state.");
		DefinitionDrugId = state.DefinitionDrugId;
		SourceFamily = state.SourceFamily;
		RemainingSeconds = Math.Max(0, state.RemainingSeconds);
	}

	public ConfiguredInfection(ConfiguredInfection source, IBody owner)
		: base(source.VirulenceDifficulty, source.Intensity, owner, null!, null!, source.Virulence)
	{
		DefinitionDrugId = source.DefinitionDrugId;
		SourceFamily = source.SourceFamily;
		RemainingSeconds = source.RemainingSeconds;
		Immunity = source.Immunity;
		InfectionStage = source.InfectionStage;
		_completed = source._completed;
	}

	protected override string SaveExtraInformation() => JsonSerializer.Serialize(new State(DefinitionDrugId, SourceFamily, RemainingSeconds));
	public override bool InfectionHealed() => _completed || Definition?.RecoveryMode == InfectionRecoveryMode.Immune && base.InfectionHealed();
	public override void InfectionTick()
	{
		if (Definition?.RecoveryMode == InfectionRecoveryMode.Immune) base.InfectionTick();
	}

	/// <summary>Advance active time only; restart and long stalls never catch up offline disease time.</summary>
	public void ResetClock() => _lastTick = DateTime.UtcNow;

	public void Tick()
	{
		var now = DateTime.UtcNow;
		var elapsed = Math.Clamp((now - _lastTick).TotalSeconds, 0, 10);
		_lastTick = now;
		Advance(elapsed);
	}

	internal void Advance(double seconds)
	{
		if (_completed || !ReferenceEquals(Owner.Actor.Body, Owner) || Owner.Actor.State.HasFlag(CharacterState.Dead) ||
			Definition is not { IsValid: true } info || !double.IsFinite(seconds) || seconds <= 0) return;
		if (info.RecoveryMode == InfectionRecoveryMode.FixedDuration)
		{
			RemainingSeconds = Math.Max(0, RemainingSeconds - seconds);
			Changed = true;
			if (RemainingSeconds <= 0)
			{
				CompleteNaturally();
				return;
			}
		}
		if (info.SpreadOnSymptomEpisode) return;
		_spreadSeconds += seconds;
		while (_spreadSeconds >= info.SpreadIntervalSeconds)
		{
			_spreadSeconds -= info.SpreadIntervalSeconds;
			if (RandomUtilities.DoubleRandom(0, 1) < info.SpreadEpisodeChance) Spread(Outcome.NotTested);
		}
	}

	private DrugSymptomEffect? OwnedSymptoms => Owner.EffectsOfType<DrugSymptomEffect>()
		.FirstOrDefault(x => x.SourceInfectionDrugId == DefinitionDrugId);
	private void CompleteNaturally()
	{
		if (_completed) return;
		_completed = true;
		Changed = true;
		var symptoms = OwnedSymptoms;
		if (symptoms is not null)
		{
			Owner.RemoveEffect(symptoms, true);
			symptoms.ApplyNaturalExpiry();
		}
	}

	public void OnSymptomEpisode()
	{
		if (Definition is { SpreadOnSymptomEpisode: true } info && RandomUtilities.DoubleRandom(0, 1) < info.SpreadEpisodeChance) Spread(Outcome.NotTested);
	}

	public override void Spread(Outcome outcome)
	{
		if (Definition is not { IsValid: true } info || info.SpreadChance <= 0 || _completed ||
			Owner.Actor.Location is not { } room) return;
		var definitionDrug = Gameworld.Drugs.Get(DefinitionDrugId);
		if (definitionDrug is null) return;
		foreach (var target in room.Characters.ToArray())
		{
			if (_completed || ReferenceEquals(target, Owner.Actor) || target.State.HasFlag(CharacterState.Dead) ||
				target.State.HasFlag(CharacterState.Stasis) || !ReferenceEquals(Owner.Actor.Body, Owner) ||
				!ReferenceEquals(target.Location, room)) continue;
			if (RandomUtilities.DoubleRandom(0, 1) < info.SpreadChance) TryExpose(target.Body, definitionDrug, SourceFamily);
		}
	}

	public static bool TryExpose(IBody body, IDrug drug, string? family)
	{
		var info = drug.AdditionalInfoFor<DrugInfectionAdditionalInfo>(DrugType.Infection);
		if (info is not { IsValid: true } || !Enum.IsDefined(typeof(Difficulty), info.VirulenceDifficulty) ||
			body.Gameworld.Drugs.Get(info.SymptomDrugId) is not { } symptoms || symptoms.DrugTypes.Contains(DrugType.Infection) ||
			body.PartInfections.OfType<IDrugSymptomInfection>().Any(x => x.DefinitionDrugId == drug.Id) ||
			!ReferenceEquals(body.Actor.Body, body) || body.Actor.State.HasFlag(CharacterState.Dead) ||
			RandomUtilities.DoubleRandom(0, 1) >= Math.Clamp(info.ExposureChance * drug.IntensityForType(DrugType.Infection), 0, 1)) return false;
		body.AddInfection(new ConfiguredInfection(body, drug, info, family));
		body.StartHealthTick();
		body.RefreshDrugEffects();
		if (!ReferenceEquals(body.Actor.Body, body)) return true;
		body.CheckDrugTick();
		if (ReferenceEquals(body.Actor.Body, body)) body.CheckHealthStatus();
		return true;
	}

	protected override void OnDelete()
	{
		_completed = true;
		if (OwnedSymptoms is { } symptoms) Owner.RemoveEffect(symptoms, true);
	}
	protected override bool UpdateInfectionStage(double oldIntensity)
	{
		InfectionStage = Intensity <= 0 ? InfectionStage.StageZero : (InfectionStage)Math.Clamp((int)Math.Ceiling(Intensity), 1, 6);
		if (Immunity >= 1 && InfectionStage == InfectionStage.StageZero) CompleteNaturally();
		return true;
	}
	protected override void SendIntensityMessage(bool improving) { }
	public override string WoundTag(WoundExaminationType examType, Outcome outcome) => Definition?.Name ?? "an illness";
	public override bool InfectionIsDamaging() => false;
	public override bool InfectionCanSpread() => false; // Transmission uses the definition's independent active-time cadence.
	public override IDamage GetInfectionDamage() => throw new InvalidOperationException("Configured infections use drug symptoms.");
}
