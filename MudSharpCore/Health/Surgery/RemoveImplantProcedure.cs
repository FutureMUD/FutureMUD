using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class RemoveImplantProcedure : BodypartSpecificSurgicalProcedure
{
	public RemoveImplantProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public RemoveImplantProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	#region Overrides of SurgicalProcedure

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.RemoveImplant;
	public override CheckType Check => CheckType.RemoveImplantSurgery;
	public override bool RequiresInvasiveProcedureFinalisation => true;
	public override bool RequiresUnconsciousPatient => true;
	public override bool RequiresLivingPatient => false;

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 2)
		{
			return new object[] { default(IGameItem), default(IBodypart) };
		}

		var bodypart = patient.Body.GetTargetBodypart(additionalArguments[0].ToString());
		return new object[]
		{
			patient.Body.Implants
			       .Where(x => x.TargetBodypart == bodypart ||
			                   (x.TargetBodypart is IOrganProto op && bodypart.Organs.Contains(op)))
			       .Select(x => x.Parent)
			       .GetFromItemListByKeyword(additionalArguments[1].ToString(), surgeon),
			bodypart
		};
	}

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		return $"{ProcedureGerund} from $1's {bodypart.FullDescription()}";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return string.Format(
			emote,
			((IGameItem)additionalArguments[0]).HowSeen(surgeon),
			((IBodypart)additionalArguments[1]).FullDescription().ToLowerInvariant());
	}

	public override int DressPhaseEmoteExtraArgumentCount => 2;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the implant item
	#3{1}#0 - the description of the bodypart the implant sits in
".SubstituteANSIColour();

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 2)
		{
			return false;
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (!(args[1] is IBodypart bodypart))
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return false;
		}

		if (patient.EffectsOfType<SurgeryFinalisationRequired>().All(x => x.Bodypart != bodypart))
		{
			return false;
		}

		var implantItem = args[0] as IGameItem;

		var implant = implantItem?.GetItemType<IImplant>();
		if (implant == null)
		{
			return false;
		}

		if (implant is ICannula)
		{
			return false;
		}

		return base.CanPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 2)
		{
			return
				"You must specify the the bodypart through which you want to access the implant, and the implant itself.";
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (!(args[1] is IBodypart bodypart))
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to work with {bodypart.FullDescription().Pluralise()}.";
		}

		if (patient.EffectsOfType<SurgeryFinalisationRequired>().All(x => x.Bodypart != bodypart))
		{
			return
				$"You require an open surgical wound on your patient's {bodypart.FullDescription()} in order to perform this procedure.";
		}

		if (!(args[0] is IGameItem implantItem))
		{
			return
				$"{patient.HowSeen(surgeon, true)} does not have any implant like that in {patient.ApparentGender(surgeon).Possessive()} {bodypart.FullDescription()}.";
		}

		var implant = implantItem.GetItemType<IImplant>();
		if (implant == null)
		{
			return $"{implantItem.HowSeen(surgeon, true)} is not an implant.";
		}

		if (implant is ICannula)
		{
			return "This procedure is not designed to be used with cannulae; use a decannulation surgery instead.";
		}

		return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	/// <inheritdoc />
	public override IBodypart GetTargetBodypart(object[] parameters)
	{
		return (IBodypart)parameters[1];
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var implantItem = (IGameItem)additionalArguments[0];
		var implant = implantItem.GetItemType<IImplant>();
		var bodypart = (IBodypart)additionalArguments[1];
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, implantItem);
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart,
			Difficulty.Normal.StageUp(result.FailureDegrees()));

		if (!patient.Body.Implants.Contains(implant))
		{
			surgeon.Send(
				$"Dissapointly, you discover that {patient.HowSeen(surgeon)} no longer has the implant you were trying to extract.");
			return;
		}

		if (result.IsFail() && !patient.State.HasFlag(CharacterState.Dead))
		{
			var qualityDesc = "less than perfect";
			switch (result.Outcome)
			{
				case Outcome.Fail:
					qualityDesc = "sloppy";
					break;
				case Outcome.MajorFail:
					qualityDesc = "practically negligent";
					break;
			}

			if (result.IsAbjectFailure)
			{
				qualityDesc = "absolutely clueless and negligent";
			}

			surgeon.OutputHandler.Send($"Your {qualityDesc} surgery has left your patient with internal bleeding.");
			patient.Body.AddEffect(new InternalBleeding(patient.Body, null, result.FailureDegrees() * 0.005));
		}

		patient.Body.RemoveImplant(implant);
		if (surgeon.Body.CanGet(implantItem, 0))
		{
			surgeon.Body.Get(implantItem);
		}
		else
		{
			implantItem.RoomLayer = surgeon.RoomLayer;
			surgeon.Location.Insert(implantItem);
		}
	}

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		var implant = (IGameItem)additionalArguments[0];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient, implant)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[1], Difficulty.VeryHard);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, implant);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, (IBodypart)additionalArguments[1],
			Difficulty.VeryHard);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, (IGameItem)additionalArguments[0]);
	}

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var implantItem = (IGameItem)additionalArguments[0];
		var implant = implantItem.GetItemType<IImplant>();
		return patient.Corpse == null ? implant.InstallDifficulty : Difficulty.VeryEasy;
	}

	#endregion
}