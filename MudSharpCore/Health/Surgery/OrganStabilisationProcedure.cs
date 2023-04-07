using MudSharp.Body;
using MudSharp.Body.PartProtos;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Health.Surgery;

public class OrganStabilisationProcedure : OrganViaBodypartProcedure
{
	public OrganStabilisationProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(
		procedure, gameworld)
	{
	}

	public OrganStabilisationProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	#region Overrides of OrganViaBodypartProcedure

	protected override void LoadFromDB(MudSharp.Models.SurgicalProcedure procedure)
	{
		base.LoadFromDB(procedure);
		_requiresUnconsciousPatient =
			bool.Parse(XElement.Parse(procedure.Definition).Attribute("requireunconcious")?.Value ?? "true");
	}

	#endregion

	public override CheckType Check => CheckType.OrganStabilisationCheck;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.OrganStabilisation;

	public override bool RequiresInvasiveProcedureFinalisation => true;

	private bool _requiresUnconsciousPatient;

	public override bool RequiresUnconsciousPatient => _requiresUnconsciousPatient;

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var organDifficultyModifier = 0;
		var organ = (IOrganProto)additionalArguments[1];
		switch (organ)
		{
			case BrainProto _:
				organDifficultyModifier = 3;
				break;
			case HeartProto _:
				organDifficultyModifier = 2;
				break;
			case EarProto _:
				organDifficultyModifier = 1;
				break;
			case TracheaProto _:
				organDifficultyModifier = -2;
				break;
		}

		// TODO - merit type to make this change
		return (CharacterState.Conscious.HasFlag(patient.State) ? Difficulty.ExtremelyHard : Difficulty.Normal)
		       .StageUp(organDifficultyModifier).Lowest(Difficulty.Insane);
	}

	protected override IEnumerable<IEnumerable<FutureProgVariableTypes>> ParametersForCancelProg => new[]
	{
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Number,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Text
		},
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Text
		},
	};

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var organ = (IOrganProto)additionalArguments[1];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Normal);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, organ.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var organ = (IOrganProto)additionalArguments[1];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Normal);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, organ.Name);
	}

	protected override IEnumerable<IEnumerable<FutureProgVariableTypes>> ParametersForCompletionProg => new[] {
				new[]
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Number,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text,
				},
				new[]
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text,
				},
			};

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var organ = (IOrganProto)additionalArguments[1];
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, organ.Name);
		var wounds = patient.Body.Wounds.Where(x => x.Bodypart == organ).ToList();

		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart, Difficulty.Normal);
		var sb = new StringBuilder();
		var apparentGender = patient.ApparentGender(surgeon);
		var worstStackingWound = 0.0;
		if (wounds.Any())
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has the following internal injuries to {apparentGender.Possessive(true)} {organ.FullDescription()}:");

			foreach (var wound in wounds)
			{
				sb.AppendLine($"\t{wound.Describe(WoundExaminationType.SurgicalExamination, result)}");
				wound.Treat(null, TreatmentType.Antiseptic, null, Outcome.MajorPass, true);
				wound.Treat(null, TreatmentType.Tend, null, result, true);
				if (wound.DamageType.In(DamageType.Cellular, DamageType.Hypoxia))
				{
					worstStackingWound = Math.Max(worstStackingWound, wound.CurrentDamage);
				}
			}
		}
		else
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has no internal injuries to {apparentGender.Possessive(true)} {organ.FullDescription()}.");
			return;
		}

		var bleeding =
			patient.Body.EffectsOfType<IInternalBleedingEffect>()
			       .Where(x => x.Organ == organ)
			       .ToList();
		if (bleeding.Any())
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} had internal bleeding from {apparentGender.Possessive()} {bleeding.Select(x => x.Organ.FullDescription()).ListToString()}.");
			var bleedReductionAmount = 0.0;
			switch (result.Outcome)
			{
				case Outcome.MinorFail:
					bleedReductionAmount = 0.01;
					break;
				case Outcome.MinorPass:
					bleedReductionAmount = 0.025;
					break;
				case Outcome.Pass:
					bleedReductionAmount = 0.15;
					break;
				case Outcome.MajorPass:
					bleedReductionAmount = 0.3;
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
				$"{apparentGender.Subjective(true)} is not bleeding internally in {apparentGender.Possessive(true)} {organ.FullDescription()}.");
		}

		var infections = patient.Body.PartInfections.Where(x => x.Bodypart.Id == organ.Id).ToList();
		if (infections.Any())
		{
			sb.AppendLine
				($"{apparentGender.Subjective(true)} has the following internal infections in {apparentGender.Possessive(false)} {organ.FullDescription()}.");
			foreach (var infection in infections)
			{
				sb.AppendLine(
					$"\t{infection.Bodypart.FullDescription(true)} {infection.WoundTag(WoundExaminationType.SurgicalExamination, result)}");
			}
		}

		surgeon.OutputHandler.Send(sb.ToString(), false, true);

		if (!bleeding.Any() || result.IsPass())
		{
			var organFunction = patient.Body.OrganFunction(organ);
			var maxStabilisation = 0.5 + result.SuccessDegrees() * 0.1;
			if (organFunction < maxStabilisation)
			{
				patient.Body.AddEffect(new Effects.Concrete.StablisedOrganFunction(patient.Body, organ,
					maxStabilisation, ExertionLevel.Heavy, Math.Max(1.0, worstStackingWound) * 1.1));
			}

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
					new Emote($"@ have|has failed to stablise $1's {organ.FullDescription()}.",
						surgeon, surgeon, patient)));
		}
	}
}