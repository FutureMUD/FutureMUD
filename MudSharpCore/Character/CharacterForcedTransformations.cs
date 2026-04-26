using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Merits.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Character;

public partial class Character
{
	private ForcedTransformationRecheckCadence _forcedTransformationHeartbeatCadence;
	private bool _reevaluatingForcedBodyTransformation;

	private sealed class ForcedTransformationDemand
	{
		public ForcedTransformationDemand(ICharacterForm form, ForcedTransformationPriorityBand priorityBand,
			int priorityOffset, long tieBreaker, string sourceDescription)
		{
			Form = form;
			PriorityBand = priorityBand;
			PriorityOffset = priorityOffset;
			TieBreaker = tieBreaker;
			SourceDescription = sourceDescription;
		}

		public ICharacterForm Form { get; }
		public IBody Body => Form.Body;
		public ForcedTransformationPriorityBand PriorityBand { get; }
		public int PriorityOffset { get; }
		public long TieBreaker { get; }
		public string SourceDescription { get; }
	}

	public bool HasActiveForcedTransformationDemand => TryGetCurrentForcedTransformationTarget(out _);

	public bool TryGetCurrentForcedTransformationTarget(out IBody body)
	{
		var demand = GetCurrentForcedTransformationDemand();
		body = demand?.Body;
		return body is not null;
	}

	public void ReevaluateForcedBodyTransformation()
	{
		if (_reevaluatingForcedBodyTransformation || _isSwitchingBodies)
		{
			return;
		}

		_reevaluatingForcedBodyTransformation = true;
		try
		{
			RefreshForcedTransformationHeartbeatRegistration();
			var demands = OrderedForcedTransformationDemands().ToList();
			var baselineEffect = EffectsOfType<ForcedTransformationBaselineEffect>().FirstOrDefault();
			if (demands.Any())
			{
				if (baselineEffect is null)
				{
					AddEffect(new ForcedTransformationBaselineEffect(this, InferBaselineBodyId()));
				}

				var winner = demands.FirstOrDefault(x => x.Body == CurrentBody ||
				                                        CanSwitchBody(x.Body, BodySwitchIntent.Forced, out _));
				if (winner is not null && winner.Body != CurrentBody)
				{
					SwitchToBody(winner.Body, BodySwitchIntent.Forced);
				}

				return;
			}

			if (baselineEffect is null)
			{
				return;
			}

			RemoveEffect(baselineEffect);
			RestoreBaselineOrFallbackForm(baselineEffect.BaselineBodyId);
		}
		finally
		{
			_reevaluatingForcedBodyTransformation = false;
		}
	}

	private long InferBaselineBodyId()
	{
		return EffectsOfType<SpellTransformFormEffect>()
		       .Where(x => x.PriorBodyId != 0)
		       .OrderBy(x => x.AppliedAtUtcTicks == 0 ? long.MaxValue : x.AppliedAtUtcTicks)
		       .Select(x => x.PriorBodyId)
		       .FirstOrDefault(CurrentBody.Id);
	}

	private void RestoreBaselineOrFallbackForm(long baselineBodyId)
	{
		if (baselineBodyId != 0 && baselineBodyId != CurrentBody.Id)
		{
			var baselineForm = Forms.FirstOrDefault(x => x.Body.Id == baselineBodyId);
			if (baselineForm is not null && CanSwitchBody(baselineForm.Body, BodySwitchIntent.Forced, out _))
			{
				SwitchToBody(baselineForm.Body, BodySwitchIntent.Forced);
				return;
			}
		}

		var fallbackForm = Forms
			.Where(x => x.Body != CurrentBody)
			.OrderBy(x => x.SortOrder)
			.ThenBy(x => x.Alias)
			.FirstOrDefault(x => CanSwitchBody(x.Body, BodySwitchIntent.Forced, out _));
		if (fallbackForm is not null)
		{
			SwitchToBody(fallbackForm.Body, BodySwitchIntent.Forced);
			return;
		}

		Gameworld.SystemMessage(
			$"Character #{Id.ToString("N0")} could not restore a fallback form after mandatory transformations ended.",
			true
		);
	}

	private ForcedTransformationDemand GetCurrentForcedTransformationDemand()
	{
		return OrderedForcedTransformationDemands()
			.FirstOrDefault(x => x.Body == CurrentBody || CanSwitchBody(x.Body, BodySwitchIntent.Forced, out _));
	}

	private IEnumerable<ForcedTransformationDemand> OrderedForcedTransformationDemands()
	{
		return GetActiveForcedTransformationDemands()
			.OrderByDescending(x => x.PriorityBand)
			.ThenByDescending(x => x.PriorityOffset)
			.ThenByDescending(x => x.TieBreaker)
			.ThenBy(x => x.Form.SortOrder)
			.ThenBy(x => x.Form.Alias);
	}

	private IEnumerable<ForcedTransformationDemand> GetActiveForcedTransformationDemands()
	{
		foreach (var merit in AutoTransformingBodyFormMerits().Where(x => x.Applies(this)))
		{
			var form = GetProvisionedFormForMerit(merit);
			if (form is null)
			{
				continue;
			}

			yield return new ForcedTransformationDemand(form, merit.ForcedTransformationPriorityBand,
				merit.ForcedTransformationPriorityOffset, merit.Id, $"merit #{merit.Id.ToString("N0")}");
		}

		foreach (var effect in EffectsOfType<SpellTransformFormEffect>().Where(x => x.Applies()))
		{
			var form = GetProvisionedFormForSpellTransform(effect);
			if (form is null)
			{
				continue;
			}

			yield return new ForcedTransformationDemand(form, effect.PriorityBand, effect.PriorityOffset,
				effect.AppliedAtUtcTicks, $"spell #{effect.Spell.Id.ToString("N0")}:{effect.FormKey}");
		}
	}

	private IEnumerable<IAdditionalBodyFormMerit> AutoTransformingBodyFormMerits()
	{
		return _merits.OfType<IAdditionalBodyFormMerit>()
		              .Where(x => x.AutoTransformWhenApplicable);
	}

	private ICharacterForm GetProvisionedFormForMerit(IAdditionalBodyFormMerit merit)
	{
		var source = new CharacterFormSource(CharacterFormSourceType.Merit, merit.Id);
		var form = GetForm(GetFormSource(source)?.Body);
		if (form is not null)
		{
			return form;
		}

		EnsureProvisionedFormFromMerit(merit);
		return GetForm(GetFormSource(source)?.Body);
	}

	private ICharacterForm GetProvisionedFormForSpellTransform(SpellTransformFormEffect effect)
	{
		var form = Forms.FirstOrDefault(x => x.Body.Id == effect.FormBodyId);
		if (form is not null)
		{
			return form;
		}

		var source = new CharacterFormSource(CharacterFormSourceType.SpellEffect, effect.Spell.Id, effect.FormKey);
		return GetForm(GetFormSource(source)?.Body);
	}

	private ForcedTransformationRecheckCadence DesiredForcedTransformationHeartbeatCadence()
	{
		var merits = AutoTransformingBodyFormMerits().ToList();
		if (!merits.Any())
		{
			return ForcedTransformationRecheckCadence.None;
		}

		if (merits.Any(x => x.ApplicabilityRecheckCadence == ForcedTransformationRecheckCadence.FuzzyMinute))
		{
			return ForcedTransformationRecheckCadence.FuzzyMinute;
		}

		if (merits.Any(x => x.ApplicabilityRecheckCadence == ForcedTransformationRecheckCadence.FuzzyHour))
		{
			return ForcedTransformationRecheckCadence.FuzzyHour;
		}

		return ForcedTransformationRecheckCadence.None;
	}

	private void RefreshForcedTransformationHeartbeatRegistration()
	{
		if (!CanRunCharacterOngoingProcesses)
		{
			ClearForcedTransformationHeartbeatRegistration();
			return;
		}

		var desired = DesiredForcedTransformationHeartbeatCadence();
		if (desired == _forcedTransformationHeartbeatCadence)
		{
			return;
		}

		ClearForcedTransformationHeartbeatRegistration();
		_forcedTransformationHeartbeatCadence = desired;
		switch (_forcedTransformationHeartbeatCadence)
		{
			case ForcedTransformationRecheckCadence.FuzzyMinute:
				Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= ForcedTransformationHeartbeat;
				Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += ForcedTransformationHeartbeat;
				break;
			case ForcedTransformationRecheckCadence.FuzzyHour:
				Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= ForcedTransformationHeartbeat;
				Gameworld.HeartbeatManager.FuzzyHourHeartbeat += ForcedTransformationHeartbeat;
				break;
		}
	}

	private void ClearForcedTransformationHeartbeatRegistration()
	{
		switch (_forcedTransformationHeartbeatCadence)
		{
			case ForcedTransformationRecheckCadence.FuzzyMinute:
				Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= ForcedTransformationHeartbeat;
				break;
			case ForcedTransformationRecheckCadence.FuzzyHour:
				Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= ForcedTransformationHeartbeat;
				break;
		}

		_forcedTransformationHeartbeatCadence = ForcedTransformationRecheckCadence.None;
	}

	private void ForcedTransformationHeartbeat()
	{
		if (!CanRunCharacterOngoingProcesses)
		{
			ClearForcedTransformationHeartbeatRegistration();
			return;
		}

		ReevaluateForcedBodyTransformation();
	}
}
