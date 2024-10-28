using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class ConfigureImplantPowerProcedure : BodypartSpecificSurgicalProcedure
{
	public ConfigureImplantPowerProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(
		procedure, gameworld)
	{
	}

	public ConfigureImplantPowerProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	#region Overrides of SurgicalProcedure

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.ConfigureImplantPower;
	public override CheckType Check => CheckType.ConfigureImplantPowerSurgery;
	public override bool RequiresInvasiveProcedureFinalisation => true;
	public override bool RequiresUnconsciousPatient => true;
	public override bool RequiresLivingPatient => false;

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 4)
		{
			return new object[] { default(IGameItem), default(IBodypart), default(IGameItem), default(IBodypart) };
		}

		var bodypart1 = patient.Body.GetTargetBodypart(additionalArguments[0].ToString());
		var bodypart2 = patient.Body.GetTargetBodypart(additionalArguments[2].ToString());
		return new object[]
		{
			patient.Body.Implants
			       .Where(x => x.TargetBodypart == bodypart1 ||
			                   (x.TargetBodypart is IOrganProto op && bodypart1.Organs.Contains(op)))
			       .Select(x => x.Parent)
			       .GetFromItemListByKeyword(additionalArguments[1].ToString(), surgeon),
			bodypart1,
			patient.Body.Implants
			       .Where(x => x.TargetBodypart == bodypart2 ||
			                   (x.TargetBodypart is IOrganProto op && bodypart2.Organs.Contains(op)))
			       .Select(x => x.Parent)
			       .GetFromItemListByKeyword(additionalArguments[3].ToString(), surgeon),
			bodypart2
		};
	}

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart1 = (IBodypart)additionalArguments[1];
		var bodypart2 = (IBodypart)additionalArguments[3];
		if (bodypart1 != bodypart2)
		{
			return
				$"{ProcedureGerund} two implants in $1's {bodypart1.FullDescription()} and {bodypart2.FullDescription()}.";
		}

		return $"{ProcedureGerund} two implants in $1's {bodypart1.FullDescription()}.";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return string.Format(
			emote,
			((IGameItem)additionalArguments[0]).HowSeen(surgeon),
			((IBodypart)additionalArguments[1]).FullDescription().ToLowerInvariant(),
			((IGameItem)additionalArguments[2]).HowSeen(surgeon),
			((IBodypart)additionalArguments[3]).FullDescription().ToLowerInvariant()
		);
	}
	
	public override int DressPhaseEmoteExtraArgumentCount => 4;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of implant 1
	#3{1}#0 - the description of the bodypart for implant 1
	#3{2}#0 - the description of implant 2
	#3{3}#0 - the description of the bodypart for implant 2
".SubstituteANSIColour();

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 4)
		{
			return false;
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (!(args[1] is IBodypart bodypart1))
		{
			return false;
		}

		if (!(args[3] is IBodypart bodypart2))
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart1) || !IsPermissableBodypart(bodypart2))
		{
			return false;
		}

		if (patient.EffectsOfType<SurgeryFinalisationRequired>().All(x => x.Bodypart != bodypart1) ||
		    patient.EffectsOfType<SurgeryFinalisationRequired>().All(x => x.Bodypart != bodypart2))
		{
			return false;
		}

		var implantItem1 = args[0] as IGameItem;
		var implantItem2 = args[2] as IGameItem;
		var implant1 = implantItem1?.GetItemType<IImplant>();
		var implant2 = implantItem2?.GetItemType<IImplant>();
		if (implant1 == null || implant2 == null)
		{
			return false;
		}

		if (implant1 is ICannula || implant2 is ICannula)
		{
			return false;
		}

		var powersupply = implantItem1.GetItemType<IImplantPowerSupply>();
		if (powersupply == null)
		{
			return false;
		}

		var powerplant = implantItem2.GetItemType<IImplantPowerPlant>();
		if (powerplant == null)
		{
			return false;
		}

		return base.CanPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 4)
		{
			return
				"You must specify the the bodypart through which you want to access the implant, and the implant itself; twice. The first such pair is the item to be powered and the second pair is the power plant.";
		}

		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (!(args[1] is IBodypart bodypart1))
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart as the first one you specified.";
		}

		if (!(args[3] is IBodypart bodypart2))
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart as the second one you specified.";
		}

		if (!IsPermissableBodypart(bodypart1))
		{
			return $"This procedure is not designed to work with {bodypart1.FullDescription().Pluralise()}.";
		}

		if (!IsPermissableBodypart(bodypart2))
		{
			return $"This procedure is not designed to work with {bodypart2.FullDescription().Pluralise()}.";
		}

		if (patient.EffectsOfType<SurgeryFinalisationRequired>().All(x => x.Bodypart != bodypart1))
		{
			return
				$"You require an open surgical wound on your patient's {bodypart1.FullDescription()} in order to perform this procedure.";
		}

		if (patient.EffectsOfType<SurgeryFinalisationRequired>().All(x => x.Bodypart != bodypart2))
		{
			return
				$"You require an open surgical wound on your patient's {bodypart2.FullDescription()} in order to perform this procedure.";
		}

		if (!(args[0] is IGameItem implantItem1))
		{
			return
				$"{patient.HowSeen(surgeon, true)} does not have any implant like that in {patient.ApparentGender(surgeon).Possessive()} {bodypart1.FullDescription()}.";
		}

		if (!(args[2] is IGameItem implantItem2))
		{
			return
				$"{patient.HowSeen(surgeon, true)} does not have any implant like that in {patient.ApparentGender(surgeon).Possessive()} {bodypart2.FullDescription()}.";
		}

		var implant1 = implantItem1.GetItemType<IImplant>();
		if (implant1 == null)
		{
			return $"{implantItem1.HowSeen(surgeon, true)} is not an implant.";
		}

		var implant2 = implantItem2.GetItemType<IImplant>();
		if (implant2 == null)
		{
			return $"{implantItem2.HowSeen(surgeon, true)} is not an implant.";
		}

		if (implant1 is ICannula || implant2 is ICannula)
		{
			return "This procedure is not designed to be used with cannulae; use a decannulation surgery instead.";
		}

		var powersupply = implantItem1.GetItemType<IImplantPowerSupply>();
		if (powersupply == null)
		{
			return $"{implantItem1.HowSeen(surgeon, true)} is not an implant that needs to be powered.";
		}

		var powerplant = implantItem2.GetItemType<IImplantPowerPlant>();
		if (powerplant == null)
		{
			return $"{implantItem2.HowSeen(surgeon, true)} is not an implant power plant.";
		}

		return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCompletionProg => new[] {
				new[]
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Character,
					ProgVariableTypes.Number,
					ProgVariableTypes.Text,
					ProgVariableTypes.Item,
					ProgVariableTypes.Text,
					ProgVariableTypes.Item
				},
				new[]
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Character,
					ProgVariableTypes.Text,
					ProgVariableTypes.Text,
					ProgVariableTypes.Item,
					ProgVariableTypes.Text,
					ProgVariableTypes.Item
				},
			};

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var implantItem1 = (IGameItem)additionalArguments[0];
		var implant1 = implantItem1.GetItemType<IImplant>();
		var bodypart1 = (IBodypart)additionalArguments[1];
		var implantItem2 = (IGameItem)additionalArguments[2];
		var implant2 = implantItem2.GetItemType<IImplant>();
		var bodypart2 = (IBodypart)additionalArguments[3];
		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart1.Name, implantItem1, bodypart2.Name, implantItem2);
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart1,
			Difficulty.Normal.StageUp(result.FailureDegrees()), true);
		if (bodypart1 != bodypart2)
		{
			CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, bodypart2,
				Difficulty.Normal.StageUp(result.FailureDegrees()), true);
		}

		if (!patient.Body.Implants.Contains(implant1) || !patient.Body.Implants.Contains(implant2))
		{
			surgeon.Send(
				$"Dissapointly, you discover that {patient.HowSeen(surgeon)} no longer has at least one of the implants you were trying to configure.");
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

		var powersupply = implantItem1.GetItemType<IImplantPowerSupply>();
		var powerplant = implantItem2.GetItemType<IImplantPowerPlant>();

		powersupply.PowerPlant = powerplant;
	}

	protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCancelProg => new[]
	{
		new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Character,
			ProgVariableTypes.Number,
			ProgVariableTypes.Text,
			ProgVariableTypes.Item
		},
		new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Character,
			ProgVariableTypes.Text,
			ProgVariableTypes.Text,
			ProgVariableTypes.Item
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
			(IBodypart)additionalArguments[1], Difficulty.VeryHard, true);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, implant);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[1];
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, (IBodypart)additionalArguments[1],
			Difficulty.VeryHard, true);
		AbortProg?.Execute(surgeon, patient, result, bodypart.Name, (IGameItem)additionalArguments[0]);
	}

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return patient.Corpse == null ? Difficulty.Easy : Difficulty.Trivial;
	}

	#endregion
}