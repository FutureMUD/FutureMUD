using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class SurgicalSettingProcedure : BoneViaBodypartProcedure
{
	public SurgicalSettingProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public SurgicalSettingProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.SurgicalSetCheck;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.SurgicalBoneSetting;

	public override bool RequiresInvasiveProcedureFinalisation => true;

	public override bool RequiresUnconsciousPatient => false;

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		// TODO - merit type to make this change
		return patient.State.HasFlag(CharacterState.Conscious) ? Difficulty.Hard : Difficulty.Easy;
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
		var bone = (IBone)additionalArguments[1];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Easy);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, bone.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var bone = (IBone)additionalArguments[1];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Normal);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, bone.Name);
	}

	protected override IEnumerable<IEnumerable<FutureProgVariableTypes>> ParametersForCompletionProg => new[] {
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

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient)));

		var bodypart = (IBodypart)additionalArguments[0];
		var bone = (IBone)additionalArguments[1];
		var wounds = patient.Wounds.Where(x => x.Bodypart == bone).OfType<BoneFracture>().ToList();
		var wound = wounds.MinBy(x => x.CanBeTreated(TreatmentType.SurgicalSet));
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, bone.Name);
		if (wound == null)
		{
			surgeon.OutputHandler.Send(
				$"It appears that {patient.HowSeen(surgeon, type: Form.Shape.DescriptionType.Possessive)} {bone.FullDescription()} no longer requires surgical setting as there is no fracture.");
			return;
		}

		if (wound.CanBeTreated(TreatmentType.SurgicalSet) == Difficulty.Impossible)
		{
			var sb = new StringBuilder();
			foreach (var wound1 in wounds)
			{
				sb.AppendLine(
					$"{wound1.Describe(WoundExaminationType.SurgicalExamination, Outcome.MajorPass)}: {wound1.WhyCannotBeTreated(TreatmentType.SurgicalSet)}");
			}

			surgeon.OutputHandler.Send(sb.ToString());
			return;
		}

		var check = Gameworld.GetCheck(CheckType.SurgicalSetCheck);
		var outcome = check.Check(surgeon, wound.CanBeTreated(TreatmentType.SurgicalSet), patient);
		wound.Treat(surgeon, TreatmentType.SurgicalSet, null, outcome.Outcome, false);
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var baseresult = base.CanPerformProcedure(surgeon, patient, additionalArguments);
		if (!baseresult)
		{
			return false;
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);

		var bodypart = (IBodypart)args[0];
		var bone = (IBone)args[1];
		var wound = patient.Wounds.Where(x => x.Bodypart == bone).OfType<BoneFracture>().MinBy(x => x.CanBeTreated(TreatmentType.SurgicalSet));
		if (wound == null)
		{
			return false;
		}

		if (wound.Stage > BoneFractureStage.Reparation)
		{
			return false;
		}

		return true;
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var baseresult = base.CanPerformProcedure(surgeon, patient, additionalArguments);
		if (!baseresult)
		{
			return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);

		var bodypart = (IBodypart)args[0];
		var bone = (IBone)args[1];
		var wound = patient.Wounds.Where(x => x.Bodypart == bone).OfType<BoneFracture>().MinBy(x => x.CanBeTreated(TreatmentType.SurgicalSet));
		if (wound == null)
		{
			return
				$"{patient.HowSeen(surgeon, true)} doesn't appear to have any bone fractures on that bone for you to set surgically.";
		}

		if (wound.Stage > BoneFractureStage.Reparation)
		{
			return
				$"The fracture of {patient.HowSeen(surgeon, type: Form.Shape.DescriptionType.Possessive)} {bone.FullDescription()} is too advanced in healing to benefit from surgical intervention.";
		}

		throw new ApplicationException("Unknown WhyCannotPerformProcedure reason in SurgicalSettingProcedure.");
	}
}