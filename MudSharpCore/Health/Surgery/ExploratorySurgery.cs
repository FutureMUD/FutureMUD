using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class ExploratorySurgery : BodypartSpecificSurgicalProcedure
{
	public ExploratorySurgery(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public ExploratorySurgery(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.ExploratorySurgeryCheck;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.ExploratorySurgery;

	public override bool RequiresInvasiveProcedureFinalisation => true;

	public override bool RequiresLivingPatient => false;

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

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the bodypart being explored
".SubstituteANSIColour();

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.", surgeon,
					surgeon, patient)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.VeryEasy, true);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.VeryEasy, true);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCompletionProg => new[] {
				new[]
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Character,
					ProgVariableTypes.Number,
					ProgVariableTypes.Text,
				},
				new[]
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Character,
					ProgVariableTypes.Text,
					ProgVariableTypes.Text,
				},
			};

	/// <inheritdoc />
	public override IBodypart GetTargetBodypart(object[] parameters)
	{
		return (IBodypart)parameters[0];
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote(
				$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.", surgeon,
				surgeon, patient)));
		var bodypart = (IBodypart)additionalArguments[0];
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name);
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart, Difficulty.VeryEasy, true);
		var sb = new StringBuilder();
		var apparentGender = patient.ApparentGender(surgeon);
		var implants = patient.Body.Implants
		                      .Where(x => x.TargetBodypart == bodypart || bodypart.Organs.Contains(x.TargetBodypart))
		                      .Where(x => surgeon.CanSee(x.Parent))
		                      .ToList();
		foreach (var organ in patient.Body.SeveredRoots.OfType<IOrganProto>().Where(x => bodypart.Organs.Contains(x)))
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} is missing {apparentGender.Possessive()} {organ.FullDescription()}.");
		}

		var recognition =
			Gameworld.GetCheck(CheckType.ImplantRecognitionCheck)
			         .Check(surgeon, Difficulty.Normal); // TODO - difficulty varies by implant
		foreach (var implant in implants)
		{
			var functions = new List<string>();
			if (recognition.Outcome.IsPass())
			{
				switch (implant)
				{
					case ImplantPowerRouterGameItemComponent _:
						functions.Add("router");
						break;
					case IImplantPowerPlant _:
						functions.Add("power");
						break;
					case IOrganImplant _:
						functions.Add("organ");
						break;
					case ILiquidContainer _:
						functions.Add("liquid");
						break;
					case IContainer _:
						functions.Add("container");
						break;
					case IImplantNeuralLink _:
						functions.Add("neural_link");
						break;
					case IImplantTraitChange iitc:
						functions.Add(iitc.ImplantFunctionReport());
						break;
				}
			}

			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has {implant.Parent.HowSeen(surgeon)}{(functions.Any() ? $" [{functions.ListToString(separator: " ", conjunction: "", twoItemJoiner: " ")}]" : "")} implanted{(implant is IOrganImplant ? "" : $" in {apparentGender.Possessive()} {implant.TargetBodypart.FullDescription()}")}.");
		}

		var wounds =
			patient.Body.VisibleWounds(surgeon, WoundExaminationType.SurgicalExamination)
			       .Where(x => x.Bodypart is IOrganProto && bodypart.Organs.Contains((IOrganProto)x.Bodypart))
			       .OrderBy(x => x.Bodypart.Name)
			       .ThenByDescending(x => x.CurrentDamage)
			       .ToList();
		if (wounds.Any())
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has the following internal injuries in {apparentGender.Possessive()} {bodypart.FullDescription()}:");
			foreach (var organ in wounds.GroupBy(x => x.Bodypart as IOrganProto))
			{
				sb.AppendLine($"\tOn {organ.Key?.FullDescription().TitleCase() ?? "Unknown"}:");
				foreach (var wound in organ)
				{
					sb.AppendLine($"\t* {wound.Describe(WoundExaminationType.SurgicalExamination, result)}.");
				}
			}
		}
		else
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has no internal injuries in {apparentGender.Possessive()} {bodypart.FullDescription()}.");
		}

		var bleeding =
			patient.Body.EffectsOfType<IInternalBleedingEffect>()
			       .Where(x => bodypart.Organs.Contains(x.Organ))
			       .ToList();
		if (bleeding.Any())
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has internal bleeding from {apparentGender.Possessive()} {bleeding.Select(x => x.Organ.FullDescription()).ListToString()}.");
			foreach (var bleed in bleeding)
			{
				sb.AppendLine(
					$"\t{bleed.Organ.FullDescription(true)}: approximately {Gameworld.UnitManager.DescribeMostSignificant(bleed.BloodlossTotal.Approximate(0.0025), UnitType.FluidVolume, surgeon).Colour(Telnet.Red)} of bleeding @ {Gameworld.UnitManager.DescribeMostSignificant(bleed.BloodlossPerTick / 10, UnitType.FluidVolume, surgeon).Colour(Telnet.Red)} per second");
			}
		}
		else
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} is not bleeding internally in {apparentGender.Possessive()} {bodypart.FullDescription()}.");
		}

		var infections = patient.Body.PartInfections
		                        .Where(x => x.Bodypart.Id == bodypart.Id ||
		                                    bodypart.Organs.Any(y => y.Id == x.Bodypart.Id)).ToList();
		if (infections.Any())
		{
			sb.AppendLine
				($"{apparentGender.Subjective(true)} has the following internal infections in {apparentGender.Possessive()} {bodypart.FullDescription()}.");
			foreach (var infection in infections)
			{
				sb.AppendLine(
					$"\t{infection.Bodypart.FullDescription(true)} {infection.WoundTag(WoundExaminationType.SurgicalExamination, result)}");
			}
		}

		if (bodypart.ImplantSpace <= 0.0)
		{
			sb.AppendLine("There is no space for implants in this bodypart.");
		}
		else
		{
			sb.AppendLine(
				$"There is {(bodypart.ImplantSpace - implants.Sum(x => x.ImplantSpaceOccupied)).ToString("N2", surgeon).ColourValue()} space for implants remaining out of a total of {bodypart.ImplantSpace.ToString("N2", surgeon).ColourValue()}.");
		}


		surgeon.OutputHandler.Send(sb.ToString(), false, true);
	}

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		// TODO - merit type to make this change
		return patient.State.HasFlag(CharacterState.Conscious) ? Difficulty.VeryHard : Difficulty.Normal;
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IBodypart bodypart)
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
		if (args[0] is not IBodypart bodypart)
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to work with {bodypart.FullDescription().Pluralise()}.";
		}

		return !bodypart.Organs.Any()
			? $"There are no organs located in the {bodypart.FullDescription()}, and so you cannot perform exploratory surgery on it."
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