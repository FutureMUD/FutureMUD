using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;

#nullable enable
namespace MudSharp.Effects.Concrete;

/// <summary>Saved stage state for recurring symptoms of one active drug on one body.</summary>
public sealed partial class DrugSymptomEffect : Effect, IPreventNaturalStunRecoveryEffect, ICeaseCombatOnParalysisEffect
{
	private readonly Dictionary<string, Expression> _expressions = new();
	private DateTime _lastTickUtc = DateTime.UtcNow;
	private double _pendingSeconds;
	private bool _onsetApplied;
	private bool _exposureActivated;
	private double _intensity;

	public static void InitialiseEffectType() => RegisterFactory("DrugSymptoms", (root, owner) => new DrugSymptomEffect(root, owner));

	public DrugSymptomEffect(IBody owner, long drugId, int initialStage, long sourceInfectionDrugId = 0) : base(owner)
	{
		DrugId = drugId;
		SourceInfectionDrugId = sourceInfectionDrugId;
		Stage = initialStage;
	}

	private DrugSymptomEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		SourceInfectionDrugId = (long?)root.Element("Effect")?.Element("SourceInfectionDrugId") ?? 0;
		DrugId = (long?)root.Element("Effect")?.Element("DrugId") ?? 0;
		Stage = (int?)root.Element("Effect")?.Element("Stage") ?? 0;
		_onsetApplied = (bool?)root.Element("Effect")?.Element("OnsetApplied") ?? true;
		_exposureActivated = (bool?)root.Element("Effect")?.Element("ExposureActivated") ?? true;
		_intensity = (double?)root.Element("Effect")?.Element("Intensity") ?? 0.0;
		if (!double.IsFinite(_intensity) || _intensity < 0.0) _intensity = 0.0;
	}

	protected override string SpecificEffectType => "DrugSymptoms";
	public override bool SavingEffect => true;
	public long SourceInfectionDrugId { get; }
	public long DrugId { get; }
	public int Stage { get; private set; }
	public double Intensity
	{
		get => _intensity;
		set
		{
			var bounded = double.IsFinite(value) && value > 0.0 ? value : 0.0;
			if (_intensity == bounded) return;
			_intensity = bounded;
			Changed = true;
		}
	}
	public bool PreventNaturalStunRecovery
	{
		get
		{
			if (Intensity <= 0.0 || Owner is not IBody body || !ReferenceEquals(body.Actor.Body, body)) return false;
			var info = Gameworld.Drugs.Get(DrugId)?.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
			if (info is null || info.ImmuneRaceIds.Contains(body.Actor.Race?.Id ?? 0)) return false;
			var stage = info.Stages[Math.Clamp(Stage, 0, info.Stages.Count - 1)];
			return stage.PreventNaturalStunRecovery && (!stage.RecoveryBlockOnlyWhenUnconscious || body.Actor.State.HasFlag(CharacterState.Unconscious));
		}
	}
	public bool CeaseCombatOnParalysis => Intensity > 0.0 && Owner is IBody body && ReferenceEquals(body.Actor.Body, body) &&
		Gameworld.Drugs.Get(DrugId)?.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms) is { } info &&
		!info.ImmuneRaceIds.Contains(body.Actor.Race?.Id ?? 0) && info.Stages[Math.Clamp(Stage, 0, info.Stages.Count - 1)].CeaseCombatOnParalysis;
	public override string Describe(IPerceiver voyeur) =>
		$"Recurring symptoms from {Gameworld.Drugs.Get(DrugId)?.Name ?? "an unknown drug"}, stage {Stage.ToString("N0", voyeur)}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("DrugId", DrugId), new XElement("SourceInfectionDrugId", SourceInfectionDrugId), new XElement("Stage", Stage),
		new XElement("OnsetApplied", _onsetApplied),
		new XElement("ExposureActivated", _exposureActivated), new XElement("Intensity", Intensity));

	/// <summary>Per-dose consequences happen at accepted administration, before absorption and any subsequent cure.</summary>
	public void ApplyAdministeredDose(double intensity)
	{
		if (Owner is not IBody body || !ReferenceEquals(body.Actor.Body, body) ||
			body.Actor.State.HasFlag(CharacterState.Dead) || !double.IsFinite(intensity) || intensity <= 0.0) return;
		var info = Gameworld.Drugs.Get(DrugId)?.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
		if (info?.OnsetPerDose != true) return;
		_onsetApplied = true;
		Changed = true;
		ApplyTraitChanges(body, info, info.OnsetTraitChanges, true, intensity);
	}

	public void ApplyOnset()
	{
		if (Owner is not IBody body || !ReferenceEquals(body.Actor.Body, body) || Intensity <= 0.0 ||
			body.Actor.State.HasFlag(CharacterState.Dead)) return;
		var info = Gameworld.Drugs.Get(DrugId)?.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
		if (info is null) return;
		_exposureActivated = true;
		if (info.OnsetPerDose)
		{
			_onsetApplied = true;
			Changed = true;
			return;
		}
		if (_onsetApplied) return;
		_onsetApplied = true;
		Changed = true;
		ApplyTraitChanges(body, info, info.OnsetTraitChanges, true);
	}

	/// <summary>Called only after natural metabolism removes the final raw active and latent dose.</summary>
	public void ApplyNaturalExpiry()
	{
		if (!_exposureActivated || Owner is not IBody body) return;
		_exposureActivated = false;
		var info = Gameworld.Drugs.Get(DrugId)?.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
		if (info is not null) ApplyTraitChanges(body, info, info.ExpiryTraitChanges, false);
	}

	private void ApplyTraitChanges(IBody body, DrugSymptomsAdditionalInfo info, IEnumerable<DrugPermanentTraitChange> changes,
		bool requireAttachment, double? intensity = null)
	{
		foreach (var change in changes.ToArray())
		{
			if (!ReferenceEquals(body.Actor.Body, body) || body.Actor.State.HasFlag(CharacterState.Dead)) return;
			if (requireAttachment && !body.EffectsOfType<DrugSymptomEffect>().Contains(this)) return;
			var trait = Gameworld.Traits.Get(change.TraitId);
			if (trait is null || !body.Actor.HasTrait(trait) || !Roll(change.Chance, body, info, intensity)) continue;
			var value = Math.Max(change.MinimumValue, body.Actor.TraitRawValue(trait) + change.Amount);
			if (double.IsFinite(value)) body.Actor.SetTraitValue(trait, value);
		}
	}

	/// <summary>Only the drug heartbeat advances symptoms. Reconciliation, inspection and cures cannot create extra episodes.</summary>
	public void ResetClock()
	{
		_lastTickUtc = DateTime.UtcNow;
		_pendingSeconds = 0.0;
	}

	/// <summary>Advances only elapsed active time, capped at one ordinary heartbeat.</summary>
	public void Tick()
	{
		var now = DateTime.UtcNow;
		var elapsed = Math.Clamp((now - _lastTickUtc).TotalSeconds, 0.0, 10.0);
		_lastTickUtc = now;
		Advance(elapsed);
	}

	internal void Advance(double seconds)
	{
		if (Owner is not IBody body || !ReferenceEquals(body.Actor.Body, body) ||
			body.Actor.State.HasFlag(CharacterState.Dead) || Intensity <= 0.0) return;
		var drug = Gameworld.Drugs.Get(DrugId);
		var info = drug?.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
		if (info is null || info.Stages.Count == 0) return;
		if (info.ImmuneRaceIds.Contains(body.Actor.Race?.Id ?? 0)) return;
		var boundedStage = Math.Clamp(Stage, 0, info.Stages.Count - 1);
		if (boundedStage != Stage) { Stage = boundedStage; Changed = true; }
		_pendingSeconds += Math.Clamp(seconds, 0.0, 10.0);
		var interval = Math.Max(1.0, info.TickIntervalSeconds);
		while (_pendingSeconds >= interval)
		{
			_pendingSeconds -= interval;
			if (!ReferenceEquals(body.Actor.Body, body) || body.Actor.State.HasFlag(CharacterState.Dead) ||
				!body.EffectsOfType<DrugSymptomEffect>().Contains(this)) return;
			var stage = info.Stages[Stage];
			if (stage.AwakeOnly && !body.Actor.State.HasFlag(CharacterState.Awake)) continue;
			if (Roll(stage.TransitionChance, body, info))
			{
				var next = stage.NextStages.Where(x => x >= 0 && x < info.Stages.Count).Distinct()
					.Select(x => (Stage: x, Weight: stage.NextStageWeights.TryGetValue(x, out var formula)
						? Math.Max(0.0, Evaluate(formula, body, info)) : 1.0)).ToArray();
				var totalWeight = next.Sum(x => x.Weight);
				if (totalWeight > 0.0 && double.IsFinite(totalWeight))
				{
					Stage = next.GetWeightedRandom(x => x.Weight).Stage;
					Changed = true;
					stage = info.Stages[Stage];
				}
			}
			if (stage.AwakeOnly && !body.Actor.State.HasFlag(CharacterState.Awake)) continue;
			if (!Roll(stage.Chance, body, info)) continue;
			if (SourceInfectionDrugId != 0)
			{
				body.PartInfections.OfType<IDrugSymptomInfection>().FirstOrDefault(x => x.DefinitionDrugId == SourceInfectionDrugId)?.OnSymptomEpisode();
				if (!ReferenceEquals(body.Actor.Body, body) || !body.EffectsOfType<DrugSymptomEffect>().Contains(this)) return;
			}
			if (stage.KillSleeping && body.Actor.State.HasFlag(CharacterState.Sleeping))
			{
				body.Actor.Die();
				return;
			}
			var damage = Math.Max(0.0, Evaluate(stage.Damage, body, info));
			var pain = Math.Max(0.0, Evaluate(stage.Pain, body, info));
			var stun = Math.Max(0.0, Evaluate(stage.Stun, body, info));
			var stamina = Math.Max(0.0, Evaluate(stage.Stamina, body, info));
			if (stage.SleepAtZeroStamina && body.CurrentStamina <= 0.0 && body.Actor.State.HasFlag(CharacterState.Awake))
			{
				body.Actor.SetPosition(PositionSprawled.Instance, PositionModifier.None, null, null);
				if (!ReferenceEquals(body.Actor.Body, body) || !body.EffectsOfType<DrugSymptomEffect>().Contains(this)) return;
				body.Actor.Sleep();
				if (!ReferenceEquals(body.Actor.Body, body) || body.Actor.State.HasFlag(CharacterState.Dead)) return;
				body.CheckHealthStatus();
				continue;
			}
			var maximumBonus = Evaluate(stage.MaximumHitPointsBonus, body, info, episodeDamage: damage);
			if (maximumBonus != 0.0 && double.IsFinite(stage.MaximumHitPointsDurationSeconds) &&
				stage.MaximumHitPointsDurationSeconds > 0.0 && stage.MaximumHitPointsDurationSeconds <= TimeSpan.MaxValue.TotalSeconds)
				body.AddEffect(new TimedMaximumHitPointsEffect(body, maximumBonus, DrugId, capacityOnly: stage.MaximumHitPointsCapacityOnly), TimeSpan.FromSeconds(stage.MaximumHitPointsDurationSeconds));
			if (stamina > 0.0) body.SpendStamina(stamina);
			if (!ReferenceEquals(body.Actor.Body, body) || body.Actor.State.HasFlag(CharacterState.Dead) ||
				!body.EffectsOfType<DrugSymptomEffect>().Contains(this)) return;
			if (damage > 0.0 || pain > 0.0 || stun > 0.0)
			{
				body.PassiveSufferDamage(new Damage
				{
					DamageType = stage.DamageType, Bodypart = body.RandomBodypart,
					DamageAmount = damage, PainAmount = pain, StunAmount = stun
				}).ProcessPassiveWounds();
			}
			if (!ReferenceEquals(body.Actor.Body, body) || body.Actor.State.HasFlag(CharacterState.Dead) ||
				!body.EffectsOfType<DrugSymptomEffect>().Contains(this)) return;
			if (body.Actor.State.HasFlag(CharacterState.Awake) && !ApplyOutcome(body, stage) && stage.Echoes is { Count: > 0 })
				body.Actor.OutputHandler.Send(stage.Echoes[Expression.RandomInstance.Next(stage.Echoes.Count)]);
			body.CheckHealthStatus();
		}
	}

	private bool Roll(string expression, IBody body, DrugSymptomsAdditionalInfo info, double? intensity = null) =>
		Expression.RandomInstance.NextDouble() < Math.Clamp(Evaluate(expression, body, info, intensity), 0.0, 1.0);

	private double Evaluate(string? formula, IBody body, DrugSymptomsAdditionalInfo info, double? intensity = null, double episodeDamage = 0.0)
	{
		if (string.IsNullOrWhiteSpace(formula)) return 0.0;
		if (!_expressions.TryGetValue(formula, out var expression)) _expressions[formula] = expression = new Expression(formula);
		if (expression.HasErrors()) return 0.0;
		var trait = Gameworld.Traits.Get(info.ResistanceTraitId);
		return expression.TryEvaluateDoubleWith(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
		{
			["intensity"] = intensity ?? Intensity, ["resistance"] = trait is null ? 0.0 : body.Actor.TraitValue(trait),
			["fullness"] = body.NeedsModel.FoodSatiatedHours, ["stamina"] = body.CurrentStamina,
			["unconscious"] = body.Actor.State.HasFlag(CharacterState.Unconscious) ? 1.0 : 0.0,
			["sleeping"] = body.Actor.State.HasFlag(CharacterState.Sleeping) ? 1.0 : 0.0, ["stage"] = Stage
			, ["damage"] = episodeDamage
		}, out var value, out _) ? value : 0.0;
	}
}
