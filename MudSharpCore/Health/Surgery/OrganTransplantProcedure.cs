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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Health.Surgery;

public class OrganTransplantProcedure : BodypartSpecificSurgicalProcedure
{
    public OrganTransplantProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(procedure,
        gameworld)
    {
    }

    public OrganTransplantProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
    {
    }

    public override CheckType Check => CheckType.OrganTransplantCheck;

    public override SurgicalProcedureType Procedure => SurgicalProcedureType.OrganTransplant;

    public override bool RequiresInvasiveProcedureFinalisation => true;

    public override bool RequiresUnconsciousPatient => true;

    public override bool RequiresLivingPatient => false;

    public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        ISeveredBodypart item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
        IBodypart bodypart = item.RootPart;
        IBodypart entryPart = (IBodypart)additionalArguments[1];
        return $"{ProcedureGerund} {bodypart.FullDescription().A_An()} into $1's {entryPart.FullDescription()}";
    }

    public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        ISeveredBodypart item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
        IBodypart part = item.RootPart;
        IBodypart entryPart = (IBodypart)additionalArguments[1];
        return string.Format(
            emote,
            part.FullDescription().ToLowerInvariant(),
            item.Parent.HowSeen(surgeon),
            entryPart.FullDescription().ToLowerInvariant());
    }

    public override int DressPhaseEmoteExtraArgumentCount => 3;

    public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the organ being transplanted
	#3{1}#0 - the description of the organ as an item
	#3{2}#0 - the description of the bodypart via which the organ is being accessed
".SubstituteANSIColour();

    protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCancelProg => new[]
    {
        new[]
        {
            ProgVariableTypes.Character,
            ProgVariableTypes.Character,
            ProgVariableTypes.Number,
            ProgVariableTypes.Text,
            ProgVariableTypes.Text,
            ProgVariableTypes.Item
        },
        new[]
        {
            ProgVariableTypes.Character,
            ProgVariableTypes.Character,
            ProgVariableTypes.Text,
            ProgVariableTypes.Text,
            ProgVariableTypes.Text,
            ProgVariableTypes.Item
        },
    };

    public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
        params object[] additionalArguments)
    {
        ISeveredBodypart item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
        IBodypart rootPart = item.RootPart;
        IOrganProto partOrgan = (IOrganProto)rootPart;
        IBodypart entryPart = (IBodypart)additionalArguments[1];
        surgeon.OutputHandler.Handle(
            new EmoteOutput(new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
                surgeon, surgeon, patient,
                item.Parent)));
        AbortProg?.Execute(surgeon, patient, result, partOrgan.Name, entryPart.Name, item.Parent);
        CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
            (IBodypart)additionalArguments[1], Difficulty.Hard, true);
    }

    protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
        params object[] additionalArguments)
    {
        ISeveredBodypart item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
        IBodypart rootPart = item.RootPart;
        IOrganProto partOrgan = (IOrganProto)rootPart;
        IBodypart entryPart = (IBodypart)additionalArguments[1];
        AbortProg?.Execute(surgeon, patient, result, partOrgan.Name, entryPart.Name, item.Parent);
        CreateMedicalFinalisationRequiredEffect(surgeon, patient, result,
            (IBodypart)additionalArguments[1], Difficulty.Hard, true);
    }

    /// <inheritdoc />
    public override IBodypart GetTargetBodypart(object[] parameters)
    {
        return (IBodypart)parameters[1];
    }

    public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
        params object[] additionalArguments)
    {
        ISeveredBodypart item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
        IBodypart rootPart = item.RootPart;
        IOrganProto partOrgan = (IOrganProto)rootPart;
        IBodypart entryPart = (IBodypart)additionalArguments[1];

        if (!patient.Body.SeveredRoots.Contains(partOrgan))
        {
            surgeon.OutputHandler.Handle(
                new EmoteOutput(
                    new Emote(
                        $"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because it is no longer a valid transplantation.",
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
        CompletionProg?.Execute(surgeon, patient, result.Outcome, partOrgan.Name, rootPart.Name, item);

        Difficulty difficulty = item.RootPart.Significant ? Difficulty.Normal : Difficulty.VeryEasy;
        if (item.Decay > DecayState.Fresh)
        {
            difficulty = difficulty.StageUp(1);
        }

        if (item.OriginalCharacter != patient)
        {
            difficulty = difficulty.StageUp(3);
        }

        difficulty = difficulty.StageDown(result.CheckDegrees());

        patient.Body.RestoreOrgan(partOrgan);
        item.Parent.Delete();
        CreateMedicalFinalisationRequiredEffect(surgeon, patient, result, entryPart,
            Difficulty.Normal, true);

        patient.Body.AddEffect(new ReplantedBodypartsEffect(patient.Body, partOrgan, difficulty),
            TimeSpan.FromSeconds(600));
    }

    public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        ISeveredBodypart item = ((IGameItem)additionalArguments[0]).GetItemType<ISeveredBodypart>();
        Difficulty difficulty = Difficulty.Hard;
        if (!item.RootPart.Significant)
        {
            difficulty = Difficulty.Easy;
        }

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
        return additionalArguments.Length != 2
            ? new object[] { }
            : new object[]
            {
                surgeon.TargetHeldItem(additionalArguments[0].ToString()),
                patient.Body.GetTargetBodypart(additionalArguments[1].ToString())
            };
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
        object[] args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
        if (args.Length != 2)
        {
            return false;
        }

        IGameItem item = args[0] as IGameItem;

        ISeveredBodypart severItem = item?.GetItemType<ISeveredBodypart>();

        if (!(severItem?.RootPart is IOrganProto organ))
        {
            return false;
        }

        if (!IsPermissableBodypart(organ))
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

        IBodypart bodypart = args[1] as IBodypart;

        if (bodypart?.Organs.Contains(organ) != true)
        {
            return false;
        }

        return base.CanPerformProcedure(surgeon, patient, additionalArguments);
    }

    public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        object[] args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
        if (args.Length != 2)
        {
            return "You must specify an organ to transplant and a bodypart through which to access the organ.";
        }

        if (args[0] is not IGameItem item)
        {
            return "There is no such item to transplant.";
        }

        ISeveredBodypart severItem = item.GetItemType<ISeveredBodypart>();
        if (severItem == null)
        {
            return $"{item.HowSeen(surgeon, true)} is not an excised organ and so cannot be transplanted.";
        }

        if (severItem.RootPart is not IOrganProto organ)
        {
            return $"{item.HowSeen(surgeon, true)} is not an organ, and so must be replanted rather than transplanted.";
        }

        if (!IsPermissableBodypart(organ))
        {
            return $"This procedure is not designed to transplant {organ.FullDescription().Pluralise()}.";
        }

        if (severItem.OriginalCharacter.Body.Prototype != patient.Body.Prototype)
        {
            return
                $"{item.HowSeen(surgeon, true)} is too biologically incompatible with {patient.HowSeen(surgeon)}.";
        }

        if (severItem.Decay > DecayState.Recent)
        {
            return $"{item.HowSeen(surgeon, true)} is far too decayed to be successfully transplanted.";
        }

        if (args[1] is not IBodypart bodypart)
        {
            return $"{patient.HowSeen(surgeon, true)} has no such bodypart through which you can access their organs.";
        }

        if (!bodypart.Organs.Contains(organ))
        {
            return
                $"The {bodypart.FullDescription()} is not generally known to contain {organ.FullDescription().Pluralise()}.";
        }

        return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
    }

    protected override (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string)
        GetSpecialPhaseAction(string actionText)
    {
        List<(Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string)> actions =
            new();
        foreach (string text in actionText.Split(' '))
        {
            if (actionText.EqualTo("checkspace"))
            {
                actions.Add(
                    CheckSpacePhaseSpecialAction());
                continue;
            }

            if (actionText.EqualTo("checkorgan"))
            {
                actions.Add(
                    CheckOrganPhaseSpecialAction());
            }

            actions.Add(base.GetSpecialPhaseAction(actionText));
        }

        return ((surgeon, patient, parameters) => { return actions.All(x => x.Item1(surgeon, patient, parameters)); },
                (surgeon, patient, parameters) =>
                {
                    return actions.First(x => !x.Item1(surgeon, patient, parameters))
                                  .Item2(surgeon, patient, parameters);
                },
                actions.Select(x => x.Item3).ListToString()
            );
    }

    private static (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string) CheckOrganPhaseSpecialAction()
    {
        return ((surgeon, patient, parameters) =>
                {
                    IGameItem item = parameters[0] as IGameItem;
                    ISeveredBodypart severItem = item.GetItemType<ISeveredBodypart>();
                    IOrganProto organ = severItem.RootPart as IOrganProto;
                    IBodypart bodypart = (IBodypart)parameters[1];
                    if (patient.Body.Organs.Contains(organ))
                    {
                        return false;
                    }

                    return true;
                }
                ,
                (surgeon, patient, parameters) =>
                {
                    IBodypart bp = (IBodypart)parameters[1];
                    IGameItem item = parameters[0] as IGameItem;
                    ISeveredBodypart severItem = item.GetItemType<ISeveredBodypart>();
                    IOrganProto organ = severItem.RootPart as IOrganProto;
                    return
                        $"$1 already $1|have|has {organ.FullDescription().A_An()} in &1's {bp.FullDescription()}";
                },
                "Checks for organ"
            );
    }

    private static (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string) CheckSpacePhaseSpecialAction()
    {
        return ((surgeon, patient, parameters) =>
                {
                    IGameItem item = parameters[0] as IGameItem;
                    ISeveredBodypart severItem = item.GetItemType<ISeveredBodypart>();
                    IOrganProto organ = severItem.RootPart as IOrganProto;
                    IBodypart bodypart = (IBodypart)parameters[1];
                    double implants = patient.Body.Implants
                                          .Where(x => x.TargetBodypart == bodypart)
                                          .Sum(x => x.ImplantSpaceOccupied);
                    double organs = bodypart.OrganInfo
                                         .Where(x =>
                                             x.Value.IsPrimaryInternalLocation &&
                                             patient.Body.Organs.Contains(x.Key))
                                         .Sum(x => x.Key.ImplantSpaceOccupied);
                    if (implants + organs + organ.ImplantSpaceOccupied > bodypart.ImplantSpace)
                    {
                        return false;
                    }

                    return true;
                }
                ,
                (surgeon, patient, parameters) =>
                {
                    IBodypart bodypart = (IBodypart)parameters[1];
                    return
                        $"$1 $1|do|does not have enough space in &1's {bodypart.FullDescription()} to fit the organ.";
                },
                "Checks for spare space"
            );
    }

    protected override bool BuildingCommandPhaseSpecial(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
    {
        if (command.PeekSpeech().EqualTo("checkspace"))
        {
            (Func<ICharacter, ICharacter, object[], bool> truth, Func<ICharacter, ICharacter, object[], string> error, string desc) = CheckSpacePhaseSpecialAction();
            phase.PhaseSpecialEffects = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n")
                                                                         .FluentAppend($"checkspace", true);
            phase.PhaseSuccessful += truth;
            phase.WhyPhaseNotSuccessful += error;
            phase.PhaseSpecialEffectsDescription = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n").FluentAppend(desc, true);
            Changed = true;
            actor.OutputHandler.Send($"This phase will now check whether the bodypart has space for the implant, and stop if not true.");
            return true;
        }
        if (command.PeekSpeech().EqualTo("checkorgan"))
        {
            (Func<ICharacter, ICharacter, object[], bool> truth, Func<ICharacter, ICharacter, object[], string> error, string desc) = CheckOrganPhaseSpecialAction();
            phase.PhaseSpecialEffects = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n")
                                                                         .FluentAppend($"checkorgan", true);
            phase.PhaseSuccessful += truth;
            phase.WhyPhaseNotSuccessful += error;
            phase.PhaseSpecialEffectsDescription = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n").FluentAppend(desc, true);
            Changed = true;
            actor.OutputHandler.Send($"This phase will now check whether the bodypart has the organ already, and stop if true.");
            return true;
        }
        return base.BuildingCommandPhaseSpecial(actor, command, phase);
    }

    protected override string SpecialActionText => $@"{base.SpecialActionText}
	#Ccheckspace#0 - checks whether there is enough space for the organ, and stops if not
	#Ccheckorgan#0 - checks to see whether the organ already exists, and stops if so";
}