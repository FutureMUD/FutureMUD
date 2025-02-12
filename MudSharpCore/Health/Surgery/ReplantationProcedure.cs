using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
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

public class ReplantationProcedure : BodypartSpecificSurgicalProcedure
{
	public ReplantationProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
		gameworld)
	{
	}

	public ReplantationProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	public override CheckType Check => CheckType.ReplantationCheck;

	public override SurgicalProcedureType Procedure => SurgicalProcedureType.Replantation;

	public override bool RequiresInvasiveProcedureFinalisation => true;

	public override bool RequiresUnconsciousPatient => true;

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
		var bodypart = item.RootPart;
		var limb = patient.Body.GetLimbFor(bodypart);
		var replantationDescription = limb != null
			? $"{limb.Name} at the {bodypart.FullDescription()}"
			: bodypart.FullDescription();
		return $"{ProcedureGerund} $1's {replantationDescription}";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
		var part = item.RootPart;
		return string.Format(
			emote, 
			part.FullDescription().ToLowerInvariant(),
			patient.Body.GetLimbFor(part)?.Name ?? part.FullDescription(), 
			item.Parent.HowSeen(surgeon));
	}

	public override int DressPhaseEmoteExtraArgumentCount => 3;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the bodypart being replanted
	#3{1}#0 - the description of the limb that the bodypart is part of
	#3{2}#0 - the item description of the severed bodypart
".SubstituteANSIColour();

	protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCancelProg => new[] {
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
		// No negative consequences to being interrupted on a replantation
		var item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
				surgeon, surgeon, patient,
				item.Parent)));
		AbortProg?.Execute(surgeon, patient, result, item.RootPart.Name, item.Parent);
	}

	protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments)
	{
		var item = (ISeveredBodypart)additionalArguments[0];
		AbortProg?.Execute(surgeon, patient, result, item.RootPart.Name, item.Parent);
	}

	/// <inheritdoc />
	public override IBodypart GetTargetBodypart(object[] parameters)
	{
		return ((IGameItem)parameters[0]).GetItemType<ISeveredBodypart>().RootPart.UpstreamConnection;
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		var item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
		var rootPart = item.RootPart;

		if (!patient.Body.SeveredRoots.Contains(rootPart))
		{
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because it is no longer a valid replantation.",
						surgeon, surgeon, patient, item.Parent)));
			return;
		}

		if (item.Decay > DecayState.Recent)
		{
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}, but the part is too heavily decayed to take, and the procedure fails.",
						surgeon, surgeon, patient, item.Parent)));
			return;
		}

		surgeon.OutputHandler.Handle(
			new EmoteOutput(new Emote(
				$"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.", surgeon,
				surgeon,
				patient, item.Parent)));
		CompletionProg?.Execute(surgeon, patient, result.Outcome, rootPart.Name, item);

		var difficulty = rootPart.Significant ? Difficulty.Easy : Difficulty.ExtremelyEasy;
		if (item.Decay > DecayState.Fresh)
		{
			difficulty = difficulty.StageUp(1);
		}

		if (item.OriginalCharacter != patient)
		{
			difficulty = difficulty.StageUp(3);
		}

		if (result.IsFail())
		{
			difficulty = difficulty.StageUp(result.FailureDegrees());
		}

		difficulty = difficulty.StageDown(Math.Max(0, result.SuccessDegrees() - 1));

		patient.Body.RestoreBodypart(rootPart);
		foreach (var organ in item.Organs)
		{
			patient.Body.RestoreOrgan(organ);
		}

		foreach (var bone in item.Bones)
		{
			patient.Body.RestoreBodypart(bone);
		}

		foreach (var tattoo in item.Tattoos)
		{
			patient.Body.AddTattoo(tattoo);
		}

		foreach (var wound in item.Wounds)
		{
			patient.Body.AddWound(wound);
			wound.SetNewOwner(patient);
		}

		foreach (var implant in item.Implants)
		{
			patient.Body.InstallImplant(implant.GetItemType<IImplant>());
		}

		foreach (var content in item.Contents)
		{
			content.RoomLayer = patient.RoomLayer;
			patient.Location.Insert(content);
		}

		item.SeveredBodypartWasInstalledInABody();
		item.Parent.Delete();
		CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, rootPart.UpstreamConnection,
			Difficulty.Normal);

		if (difficulty > Difficulty.Automatic)
		{
			patient.Body.AddEffect(new ReplantedBodypartsEffect(patient.Body, rootPart, difficulty),
				TimeSpan.FromSeconds(600));
		}
	}

	public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
		var difficulty = Difficulty.Hard;
		if (item.OriginalCharacter != patient)
		{
			difficulty = difficulty.StageUp(2);
		}

		if (item.Decay > DecayState.Fresh)
		{
			difficulty = difficulty.StageUp(1);
		}

		// TODO - merits
		return difficulty;
	}

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return !additionalArguments.Any()
			? new object[] { default(IGameItem) }
			: new object[] { surgeon.TargetHeldItem(additionalArguments[0].ToString()) };
	}

	protected override List<(IGameItem Item, DesiredItemState State)> GetAdditionalInventory(ICharacter surgeon,
		ICharacter patient, object[] additionalArguments)
	{
		return new List<(IGameItem Item, DesiredItemState State)>
		{
			((IGameItem)additionalArguments[0], DesiredItemState.Held)
		};
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		var item = args[0] as IGameItem;

		var severItem = item?.GetItemType<ISeveredBodypart>();
		if (severItem == null)
		{
			return false;
		}

		if (severItem.OriginalCharacter.Body.Prototype != patient.Body.Prototype)
		{
			return false;
		}

		if (severItem.Decay > DecayState.Recent)
		{
			return false;
		}

		if (!IsPermissableBodypart(severItem.RootPart))
		{
			return false;
		}

		var replantationPartDamageThreshold = Gameworld.GetStaticDouble("ReplantationPartDamageThreshold");
		if (severItem.Wounds.GroupBy(x => x.Bodypart).Any(x =>
			    x.Sum(y => y.CurrentDamage) >=
			    patient.Body.HitpointsForBodypart(x.Key) * replantationPartDamageThreshold))
		{
			return false;
		}

		var rootPart = severItem.Parts.First(x => severItem.Parts.All(y => x.UpstreamConnection != y));
		return patient.Body.SeveredRoots.Contains(rootPart) &&
		       base.CanPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IGameItem item)
		{
			return "There is no such item to replant.";
		}

		var severItem = item.GetItemType<ISeveredBodypart>();
		if (severItem == null)
		{
			return $"{item.HowSeen(surgeon, true)} is not a severed bodypart and so cannot be replanted.";
		}

		if (severItem.OriginalCharacter.Body.Prototype != patient.Body.Prototype)
		{
			return
				$"{item.HowSeen(surgeon, true)} is too biologically incompatible with {patient.HowSeen(surgeon)}.";
		}

		if (severItem.Decay > DecayState.Recent)
		{
			return $"{item.HowSeen(surgeon, true)} is far too decayed to be successfully reattached.";
		}

		if (!IsPermissableBodypart(severItem.RootPart))
		{
			return $"This procedure is not designed to replant {severItem.RootPart.FullDescription().Pluralise()}.";
		}

		var replantationPartDamageThreshold = Gameworld.GetStaticDouble("ReplantationPartDamageThreshold");
		if (severItem.Wounds.GroupBy(x => x.Bodypart).Any(x =>
			    x.Sum(y => y.CurrentDamage) >=
			    patient.Body.HitpointsForBodypart(x.Key) * replantationPartDamageThreshold))
		{
			return
				$"{item.HowSeen(surgeon, true)} has parts that are too severely damaged for the replantation to succeed.";
		}

		var rootPart = severItem.Parts.First(x => severItem.Parts.All(y => x.UpstreamConnection != y));
		return !patient.Body.SeveredRoots.Contains(rootPart)
			? $"{patient.HowSeen(surgeon, true)} is not missing any bodyparts that match {item.HowSeen(surgeon)}."
			: base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}
}