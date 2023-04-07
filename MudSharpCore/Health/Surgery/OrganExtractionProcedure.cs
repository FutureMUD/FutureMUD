using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.Body;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.GameItems;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class OrganExtractionProcedure : OrganViaBodypartProcedure
{
	public OrganExtractionProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public OrganExtractionProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.OrganExtraction;

	public override CheckType Check => CheckType.OrganExtractionCheck;

	public override bool RequiresInvasiveProcedureFinalisation => true;

	public override bool RequiresUnconsciousPatient => true;

	public override bool RequiresLivingPatient => false;

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		// TODO - merit type to make this change
		return CharacterState.Conscious.HasFlag(patient.State) ? Difficulty.VeryHard : Difficulty.Easy;
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
			(IBodypart)additionalArguments[0], Difficulty.Hard);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, organ.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var organ = (IOrganProto)additionalArguments[1];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Hard);
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
					FutureProgVariableTypes.Item
				},
				new[]
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Item
				},
			};

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var organ = (IOrganProto)additionalArguments[1];
		var severedOrgan = patient.Body.ExciseOrgan(organ);
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, organ.Name, severedOrgan);
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart,
			Difficulty.Normal.StageUp(result.FailureDegrees()));
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient)));
		
		if (severedOrgan == null)
		{
			surgeon.Send(
				$"Dissapointly, you discover that {patient.HowSeen(surgeon)} does not have {organ.FullDescription().A_An()}.");
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

		if (surgeon.Body.CanGet(severedOrgan, 0))
		{
			surgeon.Body.Get(severedOrgan);
		}
		else
		{
			severedOrgan.RoomLayer = surgeon.RoomLayer;
			surgeon.Location.Insert(severedOrgan);
		}

		if (patient.Corpse != null)
		{
			severedOrgan.GetItemType<ISeveredBodypart>().DecayPoints = patient.Corpse.DecayPoints;
		}
	}
}