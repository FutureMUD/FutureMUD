using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

#nullable enable
namespace MudSharp.Effects.Concrete;

public sealed partial class DrugSymptomEffect
{
	private bool ApplyOutcome(IBody body, DrugSymptomStage stage)
	{
		var choices = stage.Outcomes.Where(x => double.IsFinite(x.Weight) && x.Weight > 0).ToArray();
		if (choices.Length == 0 || !double.IsFinite(choices.Sum(x => x.Weight))) return false;
		var outcome = choices.GetWeightedRandom(x => x.Weight);
		ApplyOutcome(body, outcome);
		return outcome.Action != DrugSymptomAction.None || !string.IsNullOrEmpty(outcome.PrivateEcho) || !string.IsNullOrEmpty(outcome.RoomEmote);
	}

	internal static void ApplyOutcome(IBody body, DrugSymptomOutcome outcome)
	{
		var actor = body.Actor;
		if (!ReferenceEquals(actor.Body, body) || !actor.State.HasFlag(MudSharp.Character.CharacterState.Awake)) return;
		if (!string.IsNullOrWhiteSpace(outcome.PrivateEcho)) actor.OutputHandler.Send(outcome.PrivateEcho);
		if (!string.IsNullOrWhiteSpace(outcome.RoomEmote))
		{
			var emote = new Emote(outcome.RoomEmote, actor, actor);
			if (emote.Valid) actor.OutputHandler.Handle(new EmoteOutput(emote));
		}
		switch (outcome.Action)
		{
			case DrugSymptomAction.Sit:
				if (actor.PositionState?.CompareTo(PositionSitting.Instance) == PositionHeightComparison.Higher)
					actor.MovePosition(PositionSitting.Instance, PositionModifier.None, null, null, null, true, true);
				break;
			case DrugSymptomAction.DropReadiedItems:
				foreach (var item in body.WieldedItems.ToArray())
				{
					if (!ReferenceEquals(actor.Body, body) || actor.Location is null) break;
					body.Drop(item);
				}
				break;
			case DrugSymptomAction.Babble:
				if (double.IsFinite(outcome.DurationSeconds) && outcome.DurationSeconds > 0 && outcome.DurationSeconds <= 86400 &&
					!actor.CombinedEffectsOfType<IBabbleSpeechEffect>().Any(x => x.Applies()))
					actor.AddEffect(new SymptomBabbleEffect(actor), TimeSpan.FromSeconds(outcome.DurationSeconds));
				break;
			case DrugSymptomAction.Residue:
				var material = actor.Gameworld.Materials.Get(outcome.ResidueMaterialId);
				if (material is null || !double.IsFinite(outcome.ResidueKilograms) || outcome.ResidueKilograms <= 0) break;
				body.SurfaceLiquidState.DepositResidue(material, outcome.ResidueKilograms);
				if (!string.IsNullOrWhiteSpace(outcome.ClothingBodypart))
					foreach (var item in body.WornItemsFullInfo.Where(x => x.Wearloc.Name.Contains(outcome.ClothingBodypart, StringComparison.OrdinalIgnoreCase)).Select(x => x.Item).Distinct().ToArray())
						item.SurfaceLiquidState.DepositResidue(material, outcome.ResidueKilograms);
				break;
		}
		if (outcome.ReleaseGrapple || outcome.Action == DrugSymptomAction.ReleaseGrapple)
			foreach (var grapple in actor.EffectsOfType<Grappling>().ToArray()) actor.RemoveEffect(grapple, true);
	}
}
