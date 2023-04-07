using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
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

public class InstallImplantProcedure : BodypartSpecificSurgicalProcedure
{
	public InstallImplantProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public InstallImplantProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	#region Overrides of SurgicalProcedure

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.InstallImplant;
	public override CheckType Check => CheckType.InstallImplantSurgery;
	public override bool RequiresInvasiveProcedureFinalisation => true;
	public override bool RequiresUnconsciousPatient => true;
	public override bool RequiresLivingPatient => false;

	protected override (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string)
		GetSpecialPhaseAction(string actionText)
	{
		if (actionText.EqualTo("checkspace"))
		{
			return CheckSpacePhaseSpecialAction();
		}

		return base.GetSpecialPhaseAction(actionText);
	}

	private static (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string) CheckSpacePhaseSpecialAction()
	{
		return ((surgeon, patient, parameters) =>
				{
					var implant = (IGameItem)parameters[0];
					var implantItem = implant.GetItemType<IImplant>();
					var bodypart = (IBodypart)parameters[1];
					var implants = patient.Body.Implants
					                      .Where(x => x.TargetBodypart == bodypart)
					                      .Sum(x => x.ImplantSpaceOccupied);
					var organs = bodypart.OrganInfo
					                     .Where(x =>
						                     x.Value.IsPrimaryInternalLocation &&
						                     patient.Body.Organs.Contains(x.Key))
					                     .Sum(x => x.Key.ImplantSpaceOccupied);
					if (implants + organs + implantItem.ImplantSpaceOccupied > bodypart.ImplantSpace)
					{
						return false;
					}

					return true;
				}
				,
				(surgeon, patient, parameters) =>
				{
					var bodypart = (IBodypart)parameters[1];
					return
						$"$1 $1|do|does not have enough space in &1's {bodypart.FullDescription()} to fit the implant.";
				},
				"Checks part for spare space"
			);
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

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		return $"{ProcedureGerund} into $1's {bodypart.FullDescription()}";
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

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the implant item being inserted
	#3{1}#0 - the description of the bodypart for the implant
".SubstituteANSIColour();

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 2)
		{
			return false;
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
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

		if (!patient.Body.Prototype.CountsAs(implant.TargetBody))
		{
			return false;
		}

		if (!(args[1] is IBodypart bodypart))
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return false;
		}

		if (implant.TargetBodypart is IOrganProto op && !bodypart.Organs.Contains(op))
		{
			return false;
		}

		if (implant.TargetBodypart != null && !(implant.TargetBodypart is IOrganProto) &&
		    implant.TargetBodypart != bodypart)
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
			return "You must specify the implant object to install and the bodypart in which you wish to install it.";
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (!(args[0] is IGameItem implantItem))
		{
			return "You are not holding any such implant to install.";
		}

		var implant = implantItem.GetItemType<IImplant>();
		if (implant == null)
		{
			return $"{implantItem.HowSeen(surgeon, true)} is not an implant.";
		}

		if (implant is ICannula)
		{
			return "This procedure is not designed to be used with cannulae; use a cannulation surgery instead.";
		}

		if (!patient.Body.Prototype.CountsAs(implant.TargetBody))
		{
			return
				$"{implantItem.HowSeen(surgeon, true)} is not designed for the same biology as {patient.HowSeen(surgeon)} possesses.";
		}

		if (!(args[1] is IBodypart bodypart))
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to work with {bodypart.FullDescription().Pluralise()}.";
		}

		if (implant.TargetBodypart is IOrganProto op && !bodypart.Organs.Contains(op))
		{
			return $"The {bodypart.FullDescription()} is not known to contain {op.FullDescription().Pluralise()}.";
		}

		if (implant.TargetBodypart != null && !(implant.TargetBodypart is IOrganProto) &&
		    implant.TargetBodypart != bodypart)
		{
			return
				$"{implantItem.HowSeen(surgeon, true)} is very specifically designed to be installed in the subject's {implant.TargetBodypart.FullDescription()}.";
		}

		return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var implantItem = (IGameItem)additionalArguments[0];
		var implant = implantItem.GetItemType<IImplant>();
		var bodypart = (IBodypart)additionalArguments[1];
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, implantItem);
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart,
			Difficulty.VeryHard.StageUp(result.FailureDegrees()));

		surgeon.Body.Take(implantItem);
		if (implant.TargetBodypart != bodypart)
		{
			implant.TargetBodypart = bodypart;
		}

		patient.Body.InstallImplant(implant);
	}

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
		return implant.InstallDifficulty;
	}
	
	/// <inheritdoc />
	protected override bool BuildingCommandPhaseSpecial(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		if (command.PeekSpeech().EqualTo("checkspace"))
		{
			var (truth, error, desc) = CheckSpacePhaseSpecialAction();
			phase.PhaseSpecialEffects = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n")
			                                                             .FluentAppend($"checkspace", true);
			phase.PhaseSuccessful += truth;
			phase.WhyPhaseNotSuccessful += error;
			phase.PhaseSpecialEffectsDescription = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n").FluentAppend(desc, true);
			Changed = true;
			actor.OutputHandler.Send($"This phase will now check whether the bodypart has space for the implant, and stop if not true.");
			return true;
		}
		return base.BuildingCommandPhaseSpecial(actor, command, phase);
	}

	protected override string SpecialActionText => $@"{base.SpecialActionText}
	#Ccheckspace#0 - checks whether there is enough space for the implant, and stops if not";

	#endregion
}