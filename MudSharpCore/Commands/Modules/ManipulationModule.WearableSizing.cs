#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.RPG.Checks;

namespace MudSharp.Commands.Modules;

internal partial class ManipulationModule
{
	[PlayerCommand("Tailor", "tailor")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[HelpInfo("tailor", @"The #3tailor#0 command fits a held wearable item to you, another individual, or a chosen scalar size. The garment must support sizing, and you need the appropriate skill and a suitable held sewing tool.

Syntax:
	#3tailor <held garment>#0 - fit it to yourself
	#3tailor <held garment> for <individual>#0 - fit it to an individual
	#3tailor <held garment> <scalar>#0 - set a scalar fit
	#3tailor <held garment> height <length> weight <mass>#0 - set explicit measurements

The wearer must accept tailoring unless they are you or a trusted ally.", AutoHelp.HelpArgOrNoArg)]
	protected static void Tailor(ICharacter actor, string command)
	{
		if (!actor.CanPerformManualAction(out var initialManualReason))
		{
			actor.OutputHandler.Send(initialManualReason);
			return;
		}

		var actorBody = actor.Body;
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which held garment do you want to tailor?");
			return;
		}

		var garment = actor.TargetHeldItem(ss.PopSpeech());
		if (garment is null)
		{
			actor.OutputHandler.Send("You must be holding the garment you want to tailor.");
			return;
		}

		var wearable = garment.GetItemType<IWearable>();
		if (wearable is null || !wearable.UsesSizing)
		{
			actor.OutputHandler.Send($"{garment.HowSeen(actor, true)} cannot be tailored for size.");
			return;
		}

		if (wearable.WornBy is not null || !actor.Body.HeldItems.Any(x => ReferenceEquals(x, garment)))
		{
			actor.OutputHandler.Send("You must be holding an unworn garment to tailor it.");
			return;
		}

		var traitId = actor.Gameworld.GetStaticLong("TailoringTraitId");
		var trait = traitId > 0 ? actor.Gameworld.Traits.Get(traitId) : null;
		if (trait is null)
		{
			actor.OutputHandler.Send("Tailoring is not available in this world.");
			return;
		}
		if (!actor.HasTrait(trait) && actor.TraitRawValue(trait) <= 0.0)
		{
			actor.OutputHandler.Send($"You need the {trait.Name.ColourName()} skill to tailor garments.");
			return;
		}

		var toolTagId = actor.Gameworld.GetStaticLong("TailoringToolTagId");
		var toolTag = toolTagId > 0 ? actor.Gameworld.Tags.Get(toolTagId) : null;
		if (toolTag is null)
		{
			actor.OutputHandler.Send("Tailoring tools are not available in this world.");
			return;
		}

		var tool = actor.Body.HeldItems.FirstOrDefault(x => !ReferenceEquals(x, garment) && x.IsA(toolTag));
		if (tool is null)
		{
			actor.OutputHandler.Send($"You need to be holding a tool tagged {toolTag.FullName.ColourName()} to tailor clothing.");
			return;
		}

		var fitText = ss.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(fitText)) fitText = "for self";

		ICharacter fitTarget = actor;
		IBody fitTargetBody = actorBody;
		bool CanContinue() => actor.CanPerformManualAction(out _) && ReferenceEquals(actor.Body, actorBody) &&
			CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(actor, actorBody) &&
			actorBody.HeldItems.Any(x => ReferenceEquals(x, garment)) && actorBody.HeldItems.Any(x => ReferenceEquals(x, tool)) && tool.IsA(toolTag) &&
			actor.Gameworld.GetStaticLong("TailoringToolTagId") == toolTagId &&
			ReferenceEquals(actor.Gameworld.Tags.Get(toolTagId), toolTag) &&
			(actor.HasTrait(trait) || actor.TraitRawValue(trait) > 0.0);
		bool CanContinueWithTarget() => CanContinue() && wearable.UsesSizing &&
			ReferenceEquals(garment.GetItemType<IWearable>(), wearable) && wearable.WornBy is null &&
			fitTarget.ColocatedWith(actor) && ReferenceEquals(fitTarget.Body, fitTargetBody) &&
			CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(fitTarget, fitTargetBody);

		WearableFit requestedFit;
		if (fitText.StartsWith("for ", StringComparison.OrdinalIgnoreCase))
		{
			var targetName = fitText[4..].Trim();
			var resolvedTarget = targetName.EqualTo("self") ? actor : actor.TargetActor(targetName);
			if (resolvedTarget is null)
			{
				actor.OutputHandler.Send("You cannot see that individual to tailor the garment for.");
				return;
			}

			fitTarget = resolvedTarget;
			fitTargetBody = fitTarget.Body;
			requestedFit = wearable.FitFor(fitTargetBody);
			if (!CanContinueWithTarget())
			{
				actor.OutputHandler.Send("Your circumstances changed while you were considering the fit.");
				return;
			}
		}
		else if (!WearableSizing.TryParse(fitText, actor, actor.Gameworld, out requestedFit, out var error))
		{
			actor.OutputHandler.Send(error.ColourError());
			return;
		}

		if (requestedFit.Kind is WearableFitKind.Unfitted or WearableFitKind.Universal)
		{
			actor.OutputHandler.Send("Tailoring must set a concrete measurement or scalar fit.");
			return;
		}

		if (!CanContinueWithTarget())
		{
			actor.OutputHandler.Send("Your circumstances changed before the tailoring could begin.");
			return;
		}

		void BeginTailoring()
		{
			if (!CanContinueWithTarget())
			{
				actor.OutputHandler.Send("You can no longer reach the same unworn garment, tool, and wearer to tailor it.");
				return;
			}

			StagedCharacterActionWithTarget? action = null;
			void Cancel()
			{
				actor.OutputHandler.Send("You can no longer continue tailoring that garment.");
				if (action is not null) actor.RemoveEffect(action, true);
			}

			void Stage(IPerceivable _)
			{
				if (!CanContinueWithTarget())
				{
					Cancel();
					return;
				}

				actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ continue|continues tailoring $1 for $2.",
					actor, actor, garment, fitTarget)));
				if (!CanContinueWithTarget()) Cancel();
			}

			void Finish(IPerceivable _)
			{
				if (!CanContinueWithTarget())
				{
					Cancel();
					return;
				}

				var checkFailed = actor.Gameworld.GetCheck(CheckType.GenericSkillCheck)
					.Check(actor, Difficulty.Normal, trait, garment).IsFail();
				if (!CanContinueWithTarget())
				{
					Cancel();
					return;
				}

				if (checkFailed)
				{
					actor.OutputHandler.Send("Your tailoring work does not produce a usable fit.");
					return;
				}

				wearable.SetFit(requestedFit);
				actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ finish|finishes tailoring $1 for $2.",
					actor, actor, garment, fitTarget)));
				actor.OutputHandler.Send($"The garment is now fitted for {fitTarget.HowSeen(actor)}.");
			}

			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins tailoring $1 for $2.",
				actor, actor, garment, fitTarget)));
			if (!CanContinueWithTarget())
			{
				actor.OutputHandler.Send("Your circumstances changed before the tailoring could begin.");
				return;
			}
			action = new StagedCharacterActionWithTarget(actor, fitTarget,
				$"tailoring {garment.HowSeen(actor)}", "@ stop|stops tailoring the garment.",
				"@ cannot move because #0 are|is tailoring a garment.", ["general", "movement"],
				$"tailoring {garment.HowSeen(actor)}", [Stage, Stage, Finish], 3, TimeSpan.FromSeconds(10));
			actor.AddEffect(action, TimeSpan.FromSeconds(10));
		}

		if (CharacterInstanceIdentityComparer.SamePhysicalInstance(fitTarget, actor) || fitTarget.IsTrustedAlly(actor))
		{
			BeginTailoring();
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ propose|proposes to tailor $1 for $2.",
			actor, actor, garment, fitTarget)));
		if (!CanContinueWithTarget())
		{
			actor.OutputHandler.Send("Your circumstances changed before you could offer the tailoring.");
			return;
		}
		fitTarget.OutputHandler.Send($"{actor.HowSeen(fitTarget)} wants to tailor a garment for you. Type {"accept".Colour(Telnet.Yellow)} to proceed or {"decline".Colour(Telnet.Yellow)} to refuse.");
		if (!CanContinueWithTarget())
		{
			actor.OutputHandler.Send("Your circumstances changed before the tailoring offer could be recorded.");
			return;
		}

		fitTarget.AddEffect(new Accept(fitTarget, new GenericProposal
		{
			AcceptAction = _ => BeginTailoring(),
			DescriptionString = "have a garment tailored",
			Keywords = ["tailor"],
			RejectAction = _ => fitTarget.OutputHandler.Send("You decide not to have the garment tailored."),
			ExpireAction = () => fitTarget.OutputHandler.Send("You decide not to have the garment tailored.")
		}), TimeSpan.FromSeconds(30));
	}
}
