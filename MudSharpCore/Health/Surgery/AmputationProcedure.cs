using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class AmputationProcedure : SurgicalProcedure
{
	public AmputationProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public AmputationProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.AmputationCheck;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.Amputation;

	public override bool RequiresInvasiveProcedureFinalisation => true;

	public override bool RequiresUnconsciousPatient => true;

	public override bool RequiresLivingPatient => false;

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var limb = patient.Body.GetLimbFor(bodypart);
		var amputationDescription = limb != null
			? $"{limb.Name} at the {bodypart.FullDescription()}"
			: bodypart.FullDescription();

		return $"{ProcedureGerund} $1's {amputationDescription}";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var part = (IBodypart)additionalArguments[0];
		return string.Format(
			emote, 
			part.FullDescription().ToLowerInvariant(),
			patient.Body.GetLimbFor(part)?.Name ?? part.FullDescription());
	}
	
	public override int DressPhaseEmoteExtraArgumentCount => 2;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the bodypart being amputated
	#3{1}#0 - the limb that the part belongs to
".SubstituteANSIColour();
	

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var limb = patient.Body.GetLimbFor(bodypart);
		var amputationDescription = limb != null
			? $"{limb.Name} at the {bodypart.FullDescription()}"
			: bodypart.FullDescription();
		// Amputations that aren't properly finished leave a whopping great wound
		var wounds = patient.SufferDamage(new Damage
		{
			DamageType = DamageType.Shearing,
			DamageAmount = patient.Body.HitpointsForBodypart(bodypart) - 1,
			ActorOrigin = surgeon,
			PainAmount = patient.Body.HitpointsForBodypart(bodypart) - 1,
			AngleOfIncidentRadians = Math.PI,
			Bodypart = bodypart
		});
		var woundText = "";
		if (wounds.Any())
		{
			woundText =
				$", leaving {wounds.Select(x => x.Describe(WoundExaminationType.Glance, Outcome.MajorPass)).ListToString()}";
		}

		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote(
				$"@ stop|stops &0's amputation of $1's {amputationDescription}{woundText}.", surgeon, surgeon,
				patient)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			((IBodypart)additionalArguments[0]).UpstreamConnection, Difficulty.Hard);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		// Amputations that aren't properly finished leave a whopping great wound
		var wounds = patient.SufferDamage(new Damage
		{
			DamageType = DamageType.Shearing,
			DamageAmount = patient.Body.HitpointsForBodypart(bodypart) - 1,
			ActorOrigin = surgeon,
			PainAmount = patient.Body.HitpointsForBodypart(bodypart) - 1,
			AngleOfIncidentRadians = Math.PI,
			Bodypart = bodypart
		});
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			((IBodypart)additionalArguments[0]).UpstreamConnection, Difficulty.Hard);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		if (!patient.Body.CanSeverBodypart(bodypart))
		{
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}, but the procedure is unsuccessful.",
						surgeon, surgeon, patient)));
			return;
		}

		var severedPart = patient.Body.SeverBodypart(bodypart);
		if (surgeon.Body.CanGet(severedPart, 0))
		{
			surgeon.Body.Get(severedPart, silent: true);
		}
		else
		{
			severedPart.RoomLayer = surgeon.RoomLayer;
			surgeon.Location.Insert(severedPart);
		}

		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote(
				$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
				surgeon, surgeon, patient)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Normal);
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, severedPart);
	}

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		// TODO - merit type to make this change
		return patient.State.HasFlag(CharacterState.Conscious) ? Difficulty.VeryHard : Difficulty.Normal;
	}

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return !additionalArguments.Any()
			? new object[] { default(IBodypart) }
			: new object[] { patient.Body.GetTargetBodypart(additionalArguments[0].ToString()) };
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IBodypart bodypart)
		{
			return false;
		}

		return patient.Body.CanSeverBodypart(bodypart) && base.CanPerformProcedure(surgeon, patient, args);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IBodypart bodypart)
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		return !patient.Body.CanSeverBodypart(bodypart)
			? $"The {bodypart.FullDescription()} is not something that can be severed."
			: base.WhyCannotPerformProcedure(surgeon, patient, args);
	}
}