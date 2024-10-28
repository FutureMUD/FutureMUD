using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.GameItems.Interfaces;
using MudSharp.Body.PartProtos;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class TriageProcedure : SurgicalProcedure
{
	public TriageProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public TriageProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.TriageCheck;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.DetailedExamination;

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return Difficulty.Normal;
	}

	protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCancelProg => new[] {
		new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Character,
			ProgVariableTypes.Number
		},
		new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Character,
			ProgVariableTypes.Text
		},
	};

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		// No negative consequences to being interrupted on a triage
		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
				surgeon, surgeon, patient)));
		AbortProg?.Execute(surgeon, patient, result);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		AbortProg?.Execute(surgeon, patient, result);
	}

	protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCompletionProg => new[] {
		new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Character,
			ProgVariableTypes.Number
		},
		new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Character,
			ProgVariableTypes.Text
		},
	};

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		CompletionProg?.Execute(surgeon, patient, result.Outcome);
		var sb = new StringBuilder();

		double fuzziness;
		switch (result.Outcome)
		{
			case Outcome.MajorFail:
				fuzziness = 0.2;
				break;
			case Outcome.Fail:
				fuzziness = 0.1;
				break;
			case Outcome.MinorFail:
				fuzziness = 0.07;
				break;
			case Outcome.MinorPass:
				fuzziness = 0.045;
				break;
			case Outcome.Pass:
				fuzziness = 0.035;
				break;
			case Outcome.MajorPass:
				fuzziness = 0.025;
				break;
			default:
				fuzziness = 0.5;
				break;
		}

		var apparentGender = patient.ApparentGender(surgeon);
		sb.AppendLine(
			$"Your patient is a {patient.Race.Name.ToLowerInvariant()} {patient.ApparentGender(surgeon).GenderClass()}.");

		var totalBlood = patient.Body.TotalBloodVolumeLitres;
		if (totalBlood > 0.0)
		{
			var bloodlossRatio = patient.Body.CurrentBloodVolumeLitres / patient.Body.TotalBloodVolumeLitres;
			string bloodlossDescription;
			if (bloodlossRatio >= 1.0)
			{
				bloodlossDescription = "no blood loss".Colour(Telnet.Green);
			}
			else if (bloodlossRatio >= 0.95)
			{
				bloodlossDescription = "very minor blood loss".Colour(Telnet.Yellow);
			}
			else if (bloodlossRatio >= 0.90)
			{
				bloodlossDescription = "minor blood loss".Colour(Telnet.Yellow);
			}
			else if (bloodlossRatio >= 0.825)
			{
				bloodlossDescription = "moderate blood loss".Colour(Telnet.Red);
			}
			else if (bloodlossRatio >= 0.75)
			{
				bloodlossDescription = "major blood loss".Colour(Telnet.Red);
			}
			else if (bloodlossRatio >= 0.675)
			{
				bloodlossDescription = "severe blood loss".Colour(Telnet.Red);
			}
			else if (bloodlossRatio >= 0.6)
			{
				bloodlossDescription = "very severe blood loss".Colour(Telnet.Red);
			}
			else if (bloodlossRatio >= 0.5)
			{
				bloodlossDescription = "critical blood loss".Colour(Telnet.Red);
			}
			else
			{
				bloodlossDescription = "life-threatening" +
				                       "" +
				                       " levels of blood loss".Colour(Telnet.Red);
			}

			sb.AppendLine($"{apparentGender.Subjective(true)} has suffered {bloodlossDescription}.");
		}

		var wounds = patient.VisibleWounds(surgeon, WoundExaminationType.Triage).ToList();
		var minBleeding = wounds.Sum(x => x.PeekBleed(totalBlood, ExertionLevel.Rest)) * (1.0 - fuzziness);
		var maxBleeding = wounds.Sum(x => x.PeekBleed(totalBlood, patient.Body.CurrentExertion)) * (1.0 + fuzziness);

		if (maxBleeding > 0.0)
		{
			var minSurvival =
				TimeSpan.FromSeconds(10.0 * (patient.Body.CurrentBloodVolumeLitres - totalBlood * 0.5) / maxBleeding);
			var maxSurvival =
				TimeSpan.FromSeconds(10.0 * (patient.Body.CurrentBloodVolumeLitres - totalBlood * 0.5) / minBleeding);

			sb.AppendLine(NumberUtilities.DifferenceRatio(minBleeding, maxBleeding) < 0.05
				? $"At the current rate of visible bloodloss, you would estimate that the patient will bleed out in approximately {minSurvival.Describe(surgeon).Colour(Telnet.Red)}."
				: $"At the current rate of visible bloodloss, you would estimate that the patient will bleed out in between approximately {minSurvival.Describe(surgeon).Colour(Telnet.Red)} to {maxSurvival.Describe(surgeon).Colour(Telnet.Red)}.");
		}

		var internalWounds = patient.Wounds.Where(x => x.Internal).ToList();
		var internalBleeding = patient.EffectsOfType<IInternalBleedingEffect>().ToList();
		var internalDamage = internalWounds.Sum(x => x.CurrentDamage);
		var worstInternalDamage = internalWounds.Select(x => x.CurrentDamage).DefaultIfEmpty(0).Max();
		if (internalWounds.Any() || internalBleeding.Any())
		{
			var showInternal = false;
			switch (result.Outcome)
			{
				case Outcome.MajorPass:
					showInternal = true;
					break;
				case Outcome.Pass:
					showInternal = internalDamage > 10.0 || internalBleeding.Any();
					break;
				case Outcome.MinorPass:
					showInternal = internalDamage > 30.0 ||
					               internalBleeding.Sum(x => x.BloodlossPerTick) > 0.005;
					break;
			}

			if (showInternal)
			{
				if (internalDamage > 100 || worstInternalDamage > 25 ||
				    internalBleeding.Sum(x => x.BloodlossPerTick) > 0.01 ||
				    internalBleeding.Sum(x => x.BloodlossTotal) > 1)
				{
					sb.AppendLine($"{apparentGender.Subjective(true)} appears to have substantial internal trauma.");
				}
				else if (internalDamage > 50 || worstInternalDamage > 15 ||
				         internalBleeding.Sum(x => x.BloodlossPerTick) > 0.005 ||
				         internalBleeding.Sum(x => x.BloodlossTotal) > 0.5)
				{
					sb.AppendLine($"{apparentGender.Subjective(true)} appears to have significant internal trauma.");
				}
				else if (internalDamage > 25 || worstInternalDamage > 8 ||
				         internalBleeding.Sum(x => x.BloodlossPerTick) > 0.0025 ||
				         internalBleeding.Sum(x => x.BloodlossTotal) > 0.25)
				{
					sb.AppendLine($"{apparentGender.Subjective(true)} appears to have some internal trauma.");
				}
				else
				{
					sb.AppendLine($"{apparentGender.Subjective(true)} may have some minor internal trauma.");
				}
			}
		}

		var brainFunction = patient.Body.Organs.OfType<BrainProto>()
		                           .Select(x => x.OrganFunctionFactor(patient.Body))
		                           .DefaultIfEmpty(0)
		                           .Sum();

		var conscious = CharacterState.Conscious.HasFlag(patient.State);
		if (conscious && brainFunction <= Gameworld.GetStaticDouble("MinorConcussionRatio"))
		{
			if (brainFunction <= Gameworld.GetStaticDouble("MajorConcussionRatio"))
			{
				sb.AppendLine($"{apparentGender.Subjective(true)} is showing signs of a major concussion.");
			}
			else if (brainFunction <= Gameworld.GetStaticDouble("ConcussionRatio"))
			{
				sb.AppendLine($"{apparentGender.Subjective(true)} is showing signs of a concussion.");
			}
			else
			{
				sb.AppendLine($"{apparentGender.Subjective(true)} is showing signs of a minor concussion.");
			}
		}

		var liverFunction = patient.Body.Organs.OfType<LiverProto>()
		                           .Select(x => x.OrganFunctionFactor(patient.Body))
		                           .DefaultIfEmpty(0)
		                           .Sum();

		if (liverFunction < 0.1)
		{
			sb.AppendLine($"{apparentGender.Subjective(true)} has signs of jaundice.");
		}

		var kidneyFunction = patient.Body.Organs.OfType<KidneyProto>()
		                            .Select(x => x.OrganFunctionFactor(patient.Body))
		                            .DefaultIfEmpty(0)
		                            .Sum();

		if (kidneyFunction < 0.2)
		{
			sb.AppendLine(
				$"{apparentGender.Subjective(true)} has fairly severe swelling indicative of kidney failure.");
		}

		var totalInfectionIntensity = patient.Body.PartInfections.Sum(x => x.Intensity);
		if (totalInfectionIntensity >= 100.0)
		{
			var modifier = "";
			if (totalInfectionIntensity >= 1000.0)
			{
				modifier = "very severe ";
			}
			else if (totalInfectionIntensity >= 500.0)
			{
				modifier = "severe ";
			}

			sb.AppendLine(
				$"{apparentGender.Subjective(true)} seems to have a {modifier}fever; a sign of a general infection.");
		}

		if (wounds.Any())
		{
			sb.AppendLine($"{apparentGender.Subjective(true)} has the following injuries:");
			sb.AppendLine(
				wounds
					.Select(
						x =>
							$"{x.Describe(WoundExaminationType.Triage, result)} on {apparentGender.Possessive()} {x.Bodypart.FullDescription()}.")
					.ListToCompactString("\n", "", "\n"));
			var lodgedItems = wounds.Where(x => x.Lodged != null).Select(x =>
				(item: x.Lodged, wound: x, lodgeinfo: x.Lodged.GetItemType<ILodgeConsequence>())).ToList();
			if (lodgedItems.Any())
			{
				sb.AppendLine();
				sb.AppendLine($"{apparentGender.Subjective(true)} has the following items lodged in wounds:");
				foreach (var lodgedItem in lodgedItems)
				{
					sb.AppendLine(
						$"{lodgedItem.item.HowSeen(surgeon, true)} lodged in {lodgedItem.wound.Describe(WoundExaminationType.Triage, result)} requiring {(lodgedItem.lodgeinfo?.RequiresSurgery == true ? "surgery" : "first aid").Colour(Telnet.Green)}");
				}
			}
		}
		else
		{
			sb.AppendLine($"{apparentGender.Subjective(true)} does not appear to have any external injuries.");
		}

		// TODO - more information in medical examination
		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote(
				$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.", surgeon,
				surgeon, patient)));
		surgeon.Send(sb.ToString());
	}
}