using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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

public class DecannulationProcedure : BodypartSpecificSurgicalProcedure
{
	public DecannulationProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
		if (!string.IsNullOrEmpty(procedure.Definition))
		{
			var root = XElement.Parse(procedure.Definition);
			DestroyCannula = root.Element("DestroyCannula")?.Value != "false";
			RequiresInvasiveProcedureFinalisation =
				bool.Parse(root.Element("RequiresFinalisation")?.Value ?? "false");
		}
	}

	public DecannulationProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.Decannulation;

	public override CheckType Check => CheckType.DecannulationProcedure;

	public override bool RequiresInvasiveProcedureFinalisation { get; }

	public override bool RequiresLivingPatient => false;

	public bool DestroyCannula { get; protected set; }

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		// TODO - merit type to make this change
		return Difficulty.VeryEasy;
	}

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		return $"{ProcedureGerund} from $1's {bodypart.FullDescription()}";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return string.Format(
			emote,
			((IGameItem)additionalArguments[1]).HowSeen(surgeon).ToLowerInvariant(),
			((IBodypart)additionalArguments[0]).FullDescription().ToLowerInvariant()
		);
	}

	public override int DressPhaseEmoteExtraArgumentCount => 2;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the cannula item being removed
	#3{1}#0 - the description of the bodypart being decannulated
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
			(IBodypart)additionalArguments[0], Difficulty.Easy);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[0], Difficulty.Hard);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name);
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var cannulaItem = (IGameItem)additionalArguments[1];
		var cannula = cannulaItem.GetItemType<ICannula>();
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, cannulaItem);
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart,
			Difficulty.Trivial.StageUp(result.FailureDegrees()));
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient)));

		patient.Body.RemoveImplant(cannula);
		if (DestroyCannula)
		{
			cannula.Delete();
		}
		else
		{
			if (surgeon.Body.CanGet(cannulaItem, 0))
			{
				surgeon.Body.Get(cannulaItem);
			}
			else
			{
				cannulaItem.RoomLayer = surgeon.RoomLayer;
				surgeon.Location.Insert(cannulaItem);
			}
		}
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 1)
		{
			return false;
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		var bodypart = args[0] as IBodypart;

		if (args[1] is not IGameItem cannulaItem)
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return false;
		}

		return base.CanPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 1)
		{
			return "You must specify a cannula item to remove from the patient, and nothing else.";
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);

		var bodypart = args[0] as IBodypart;
		if (args[1] is not IGameItem cannulaItem)
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such cannulae.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to decannulate {bodypart.FullDescription().Pluralise()}.";
		}

		return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 1)
		{
			return new object[] { };
		}

		var cannulaItem = patient.Body.Implants.Where(x => x.Parent.IsItemType<ICannula>()).Select(x => x.Parent)
		                         .GetFromItemListByKeyword(additionalArguments[0].ToString(), surgeon);
		return new object[] { cannulaItem?.GetItemType<ICannula>()?.TargetBodypart, cannulaItem };
	}
}