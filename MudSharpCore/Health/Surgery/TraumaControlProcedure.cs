using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class TraumaControlProcedure : BodypartSpecificSurgicalProcedure
{
	public TraumaControlProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public TraumaControlProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.TraumaControlSurgery;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.TraumaControl;

	public override bool RequiresInvasiveProcedureFinalisation => true;

	public override bool RequiresUnconsciousPatient => true;

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		// TODO - merit type to make this change
		return patient.State.HasFlag(CharacterState.Conscious) ? Difficulty.VeryHard : Difficulty.Normal;
	}

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		return $"{ProcedureGerund} $1's {bodypart.FullDescription()}";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return string.Format(
			emote, ((IBodypart)additionalArguments[0]).FullDescription().ToLowerInvariant());
	}

	public override int DressPhaseEmoteExtraArgumentCount => 1;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the bodypart being repaired
".SubstituteANSIColour();

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Normal);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Normal);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	protected override IEnumerable<IEnumerable<FutureProgVariableTypes>> ParametersForCompletionProg => new[] {
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Number,
			FutureProgVariableTypes.Text
		},
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Text
		},
	};

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var wounds =
			patient.Body.Wounds.Where(
				x => x.Bodypart is IOrganProto && bodypart.Organs.Contains((IOrganProto)x.Bodypart)).ToList();

		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name);
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart, Difficulty.Normal);
		var sb = new StringBuilder();
		var apparentGender = patient.ApparentGender(surgeon);
		if (wounds.Any())
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has the following internal injuries in {apparentGender.Possessive(true)} {bodypart.FullDescription()}:");
			foreach (var organ in wounds.GroupBy(x => x.Bodypart as IOrganProto))
			{
				sb.AppendLine($"{organ.Key?.FullDescription().TitleCase() ?? "Unknown"}:");
				foreach (var wound in organ)
				{
					sb.AppendLine($"\t{wound.Describe(WoundExaminationType.SurgicalExamination, result)}");
				}
			}
		}
		else
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has no internal injuries in {apparentGender.Possessive(true)} {bodypart.FullDescription()}.");
		}

		var bleeding =
			patient.Body.EffectsOfType<IInternalBleedingEffect>()
			       .Where(x => bodypart.Organs.Contains(x.Organ))
			       .ToList();
		if (bleeding.Any())
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} had internal bleeding from {apparentGender.Possessive()} {bleeding.Select(x => x.Organ.FullDescription()).ListToString()}.");
			var bleedReductionAmount = 0.0;
			switch (result.Outcome)
			{
				case Outcome.MinorFail:
					bleedReductionAmount = 0.02;
					break;
				case Outcome.MinorPass:
					bleedReductionAmount = 0.05;
					break;
				case Outcome.Pass:
					bleedReductionAmount = 0.3;
					break;
				case Outcome.MajorPass:
					bleedReductionAmount = 1000;
					break;
			}

			foreach (var bleed in bleeding)
			{
				var newBleed = Math.Max(0, bleed.BloodlossPerTick - bleedReductionAmount);
				sb.AppendLine(
					$"\t{bleed.Organ.FullDescription(true)}: approximately {Gameworld.UnitManager.DescribeMostSignificant(bleed.BloodlossTotal.Approximate(0.0025), UnitType.FluidVolume, surgeon).Colour(Telnet.Red)} of bleeding @ {Gameworld.UnitManager.DescribeMostSignificant(bleed.BloodlossPerTick / 10, UnitType.FluidVolume, surgeon).Colour(Telnet.Red)} per second - reduced to {Gameworld.UnitManager.DescribeMostSignificant(newBleed.Approximate(0.0025), UnitType.FluidVolume, surgeon)}.");
				bleed.BloodlossPerTick = newBleed;
				if (newBleed <= 0.0)
				{
					patient.Body.RemoveEffect(bleed);
				}
			}
		}
		else
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} is not bleeding internally in {apparentGender.Possessive(true)} {bodypart.FullDescription()}.");
		}

		var infections = patient.Body.PartInfections
		                        .Where(x => x.Bodypart.Id == bodypart.Id ||
		                                    bodypart.Organs.Any(y => y.Id == x.Bodypart.Id)).ToList();
		if (infections.Any())
		{
			sb.AppendLine
				($"{apparentGender.Subjective(true)} has the following internal infections in {apparentGender.Possessive(false)} {bodypart.FullDescription()}.");
			foreach (var infection in infections)
			{
				sb.AppendLine(
					$"\t{infection.Bodypart.FullDescription(true)} {infection.WoundTag(WoundExaminationType.SurgicalExamination, result)}");
			}
		}

		surgeon.OutputHandler.Send(sb.ToString(), false, true);

		if (!bleeding.Any() || result.IsPass())
		{
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
						surgeon, surgeon, patient)));
			if (bleeding.Any(x => x.BloodlossPerTick > 0))
			{
				surgeon.OutputHandler.Send(
					"You have further work to do before your patient is fully stabilised, as some wounds are still bleeding.");
			}
		}
		else
		{
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"@ have|has failed to control the trauma in $1's {bodypart.FullDescription()}.",
						surgeon, surgeon, patient)));
		}
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (!(args[0] is IBodypart bodypart))
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return false;
		}

		return bodypart.Organs.Any() && base.CanPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (!(args[0] is IBodypart bodypart))
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to work with {bodypart.FullDescription().Pluralise()}.";
		}

		return !bodypart.Organs.Any()
			? $"There are no organs located in the {bodypart.FullDescription()}, and so you cannot perform trauma control surgery on it."
			: base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return !additionalArguments.Any()
			? new object[] { default(IBodypart) }
			: new object[] { patient.Body.GetTargetBodypart(additionalArguments[0].ToString()) };
	}
}