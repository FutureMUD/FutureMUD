using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class CannulationProcedure : BodypartSpecificSurgicalProcedure
{
	public CannulationProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public CannulationProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.CannulationProcedure;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.Cannulation;

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		// TODO - merit type to make this change
		return Difficulty.VeryEasy;
	}

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		return $"{ProcedureGerund} $1's {bodypart.FullDescription()}";
	}

	protected override List<(IGameItem Item, DesiredItemState State)> GetAdditionalInventory(ICharacter surgeon,
		ICharacter patient, object[] additionalArguments)
	{
		return new List<(IGameItem Item, DesiredItemState State)>
		{
			((IGameItem)additionalArguments[0], DesiredItemState.Held)
		};
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

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the cannula item being inserted
	#3{1}#0 - the description of the bodypart being cannulated
".SubstituteANSIColour();

	protected override IEnumerable<IEnumerable<FutureProgVariableTypes>> ParametersForCancelProg => new[]
	{
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Number,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Item
		},
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Item
		},
	};

	public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		var cannula = (IGameItem)additionalArguments[0];
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
					surgeon, surgeon, patient, cannula)));
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[1], Difficulty.ExtremelyEasy);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, cannula);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
			(IBodypart)additionalArguments[1], Difficulty.ExtremelyEasy);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, (IGameItem)additionalArguments[0]);
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var cannulaItem = (IGameItem)additionalArguments[0];
		var cannula = cannulaItem.GetItemType<ICannula>();
		var bodypart = (IBodypart)additionalArguments[1];
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, cannulaItem);

		surgeon.Body.Take(cannulaItem);
		cannula.SetBodypart(bodypart);
		patient.Body.InstallImplant(cannula);
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 2)
		{
			return false;
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		var cannulaItem = args[0] as IGameItem;

		var cannula = cannulaItem?.GetItemType<ICannula>();
		if (cannula == null)
		{
			return false;
		}

		if (!patient.Body.Prototype.CountsAs(cannula.TargetBody))
		{
			return false;
		}

		if (args[1] is not IBodypart bodypart)
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
		if (additionalArguments.Length != 2)
		{
			return "You must specify the cannula object to install and the bodypart in which you wish to install it.";
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IGameItem cannulaItem)
		{
			return "You are not holding any such cannula to install.";
		}

		var cannula = cannulaItem.GetItemType<ICannula>();
		if (cannula == null)
		{
			return $"{cannulaItem.HowSeen(surgeon, true)} is not a cannula.";
		}

		if (!patient.Body.Prototype.CountsAs(cannula.TargetBody))
		{
			return
				$"{cannulaItem.HowSeen(surgeon, true)} is not designed for the same biology as {patient.HowSeen(surgeon)} possesses.";
		}

		if (!(args[1] is IBodypart bodypart))
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to work with {bodypart.FullDescription().Pluralise()}.";
		}

		var implants = patient.Body.Implants
		                      .Where(x => x.TargetBodypart == bodypart)
		                      .Sum(x => x.ImplantSpaceOccupied);
		var organs = bodypart.OrganInfo
		                     .Where(x => x.Value.IsPrimaryInternalLocation && patient.Body.Organs.Contains(x.Key))
		                     .Sum(x => x.Key.ImplantSpaceOccupied);

		return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return additionalArguments.Length != 2
			? new object[] { default(IGameItem), default(IBodypart) }
			: new object[]
			{
				surgeon.TargetHeldItem(additionalArguments[0].ToString()),
				patient.Body.GetTargetBodypart(additionalArguments[1].ToString())
			};
	}
}