using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Health.Infections;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class InvasiveProcedureFinalisation : BodypartSpecificSurgicalProcedure
{
	public InvasiveProcedureFinalisation(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld)
		: base(procedure, gameworld)
	{
	}

	public InvasiveProcedureFinalisation(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.InvasiveProcedureFinalisation;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.InvasiveProcedureFinalisation;

	#region Overrides of SurgicalProcedure

	public override bool RequiresUnconsciousPatient => true;

	#endregion

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		return $"{ProcedureGerund} $1's {bodypart.FullDescription()}";
	}

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		// No negative consequences to aborting the finalisation of this procedure
		var bodypart = (IBodypart)additionalArguments[0];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
				surgeon,
				surgeon, patient)));
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
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
		var bodypart = (IBodypart)additionalArguments[0];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote(
				$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
				surgeon, surgeon, patient)));
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name);

		//If you major fail, the effect does not go away and its difficulty gets bumped by 1. Repeated screwups may require
		//calling in a better surgeon to clean up the mess.
		if (result.CheckDegrees() == -3)
		{
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ have|has done a poor job of the stitch-up, and it will need to be done again.",
						surgeon, surgeon, patient)));
			var oldEffect = patient.EffectsOfType<ISurgeryFinalisationRequiredEffect>()
			                       .FirstOrDefault(x => x.Bodypart == bodypart);
			oldEffect?.BumpDifficulty();
			return;
		}

		var effect = patient.EffectsOfType<ISurgeryFinalisationRequiredEffect>()
		                    .FirstOrDefault(x => x.Bodypart == bodypart);
		if (effect != null)
		{
			patient.RemoveEffect(effect);
		}
		else
			//It's entirely possible someone finished stitching up the effect right before we did, in which case, we're done here.
		{
			return;
		}

		//Add a recovery wound
		//By default, leave severes. Merits and successes then modify from there.
		var recoverySeverity = WoundSeverity.Severe;

		//Wound gets worse or better depending on how well we did on the check.
		recoverySeverity = recoverySeverity.StageDown(result.CheckDegrees());

		//merits of surgeon could make this easier
		var merits = surgeon.Merits.OfType<ISurgeryFinalisationMerit>().Where(x => x.Applies(surgeon)).ToList();
		foreach (var merit in merits)
		{
			recoverySeverity = recoverySeverity.StageDown(merit.BonusDegrees);
		}

		if (recoverySeverity == WoundSeverity.None)
		{
			return; //No damage to apply
		}

		var damageAmount = patient.Body.HealthStrategy.GetSeverityCeiling(recoverySeverity);

		var recoveryDamage = new Damage
		{
			DamageType = DamageType.Slashing,
			DamageAmount = damageAmount,
			PainAmount = damageAmount * 2,
			ShockAmount = 0,
			StunAmount = 0,
			Bodypart = bodypart,
			ActorOrigin = surgeon,
			ToolOrigin = null,
			LodgableItem = null,
			AngleOfIncidentRadians = Math.PI / 2,
			PenetrationOutcome = Outcome.MajorFail
		};

		var recoveryWounds = patient.Body.HealthStrategy.SufferDamage(patient, recoveryDamage, bodypart).ToArray();
		foreach (var recoveryWound in recoveryWounds)
		{
			//If we got here via fail or minorfail, then the wound is not fully closed off
			recoveryWound.BleedStatus = result == Outcome.Fail ? BleedStatus.Bleeding :
				result == Outcome.MinorFail ? BleedStatus.TraumaControlled : BleedStatus.Closed;

			//Reset the damage and pain to the original amounts to negate the effect of BodyPart pain/damage modifiers.
			recoveryWound.OriginalDamage = damageAmount;
			recoveryWound.CurrentDamage = damageAmount;
			recoveryWound.CurrentPain = damageAmount * 2;

			foreach (var merit in merits)
			{
				if (merit.BonusDegrees > 0)
				{
					//If we have TidySurgeon or the equivalent, make the wound Automatically antiseptic treated.
					//Otherwise, people will need to clean the wound themselves.
					recoveryWound.BleedStatus = BleedStatus.Closed;
					recoveryWound.Treat(surgeon, TreatmentType.Antiseptic, null, Outcome.MajorPass, true);
				}

				if (merit.BonusDegrees < 0)
					//Sloppy Surgeons make a mess
				{
					if (patient.Body.PartInfections.All(x => x.Bodypart != bodypart))
					{
						var terrain = patient.Location.Terrain(patient);
						patient.Body.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection,
							terrain.InfectionVirulence.StageDown(merit.BonusDegrees), 0.0001, patient.Body, null, bodypart,
							terrain.InfectionMultiplier));
					}
				}
			}
		}
		
		patient.Body.AddWounds(recoveryWounds);
	}

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		return
			patient.EffectsOfType<ISurgeryFinalisationRequiredEffect>()
			       .FirstOrDefault(x => x.Bodypart == bodypart)?.Difficulty ?? Difficulty.Impossible;
	}

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return !additionalArguments.Any()
			? new object[] { default(IBodypart) }
			: new object[] { patient.Body.GetTargetBodypart(additionalArguments[0].ToString()) };
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return string.Format(
			emote, ((IBodypart)additionalArguments[0]).FullDescription().ToLowerInvariant());
	}

	public override int DressPhaseEmoteExtraArgumentCount => 1;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the bodypart being sutured
".SubstituteANSIColour();

	#region Overrides of SurgicalProcedure

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

		return patient.EffectsOfType<ISurgeryFinalisationRequiredEffect>().Any(x => x.Bodypart == bodypart) &&
		       base.CanPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IBodypart bodypart)
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart for you to stitch up.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to stitch up {bodypart.FullDescription().Pluralise()}.";
		}

		return patient.EffectsOfType<ISurgeryFinalisationRequiredEffect>().All(x => x.Bodypart != bodypart)
			? $"{patient.HowSeen(surgeon, true)} does not have any surgical wounds on {patient.ApparentGender(surgeon).Possessive()} {bodypart.FullDescription()} that require stitch-up."
			: base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	#endregion
}