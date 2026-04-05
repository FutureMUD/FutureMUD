using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Knowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Health.Surgery;

public abstract class BodypartSpecificSurgicalProcedure : SurgicalProcedure
{
    protected BodypartSpecificSurgicalProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(
        procedure, gameworld)
    {
    }

    public BodypartSpecificSurgicalProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
    {
    }

    private List<IBodypart> _targetedParts = new();
    private bool _targetPartsForbidden;

    protected bool IsPermissableBodypart(IBodypart bodypart)
    {
        return MatchesPermissableBodypart(_targetedParts, _targetPartsForbidden, bodypart);
    }

    internal static bool MatchesPermissableBodypart(IEnumerable<IBodypart> targetedParts, bool targetPartsForbidden,
        IBodypart bodypart)
    {
        if (targetPartsForbidden)
        {
            return !targetedParts.Any(x => bodypart.CountsAs(x));
        }

        return targetedParts.Any(x => bodypart.CountsAs(x));
    }

    protected override string SaveDefinition()
    {
        return new XElement("Definition",
            new XElement("Parts",
                new XAttribute("forbidden", _targetPartsForbidden),
                from part in _targetedParts
                select new XElement("Part", part.Id)
            )
        ).ToString();
    }

    protected override void LoadFromDB(MudSharp.Models.SurgicalProcedure procedure)
    {
        base.LoadFromDB(procedure);
        if (!string.IsNullOrEmpty(procedure.Definition))
        {
            XElement root = XElement.Parse(procedure.Definition);
            XElement partsElement = root.Element("Parts");
            if (partsElement.Attribute("forbidden")?.Value.EqualTo("true") ?? false)
            {
                _targetPartsForbidden = true;
            }

            foreach (XElement part in partsElement.Elements())
            {
                IBodypart gPart = Gameworld.BodypartPrototypes.Get(long.Parse(part.Value));
                if (gPart != null)
                {
                    _targetedParts.Add(gPart);
                }
            }
        }
        else
        {
            _targetPartsForbidden = true;
        }
    }

    protected override string AdditionalHelpText => @"	#3forbidden#0 - toggles whether the parts list is opt-in or opt-out
	#3part <which>#0 - toggles a part being a part of the list";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "target":
            case "part":
            case "parts":
            case "targetpart":
            case "targetparts":
                return BuildingCommandTargetPart(actor, command);
            case "forbidden":
                return BuildingCommandForbidden(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandForbidden(ICharacter actor, StringStack command)
    {
        _targetPartsForbidden = !_targetPartsForbidden;
        Changed = true;
        if (_targetPartsForbidden)
        {
            actor.OutputHandler.Send("The list of bodyparts for this surgery is now a list of parts which are forbidden to be targeted.");
        }
        else
        {
            actor.OutputHandler.Send("The list of bodyparts for this surgery is now an exclusive list of parts that can be targeted by this surgery.");
        }
        return true;
    }

    private bool BuildingCommandTargetPart(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which bodypart do you want to {(_targetPartsForbidden ? "forbid" : "permit")}?");
            return false;
        }

        IBodypart part = TargetBodyType.AllBodyparts.GetBodypartByName(command.SafeRemainingArgument);
        if (part is null)
        {
            actor.OutputHandler.Send($"The {TargetBodyType.Name.ColourValue()} body has no such bodypart.");
            return false;
        }

        if (_targetedParts.Contains(part))
        {
            _targetedParts.Remove(part);
            if (_targetPartsForbidden)
            {
                actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is no longer forbidden from being targeted by this surgery.");
            }
            else
            {
                actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is no longer permitted to be targeted by this surgery.");
            }
        }
        else
        {
            _targetedParts.Add(part);
            if (_targetPartsForbidden)
            {
                actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is now forbidden from being targeted by this surgery.");
            }
            else
            {
                actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is now permitted to be targeted by this surgery.");
            }
        }

        Changed = true;
        return true;
    }

    protected override bool BuildingCommandPhaseSpecial(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
    {
        if (command.PeekSpeech().EqualTo("exposed"))
        {
            (Func<ICharacter, ICharacter, object[], bool> truth, Func<ICharacter, ICharacter, object[], string> error, string desc) = ExposedPhaseSpecialAction();
            phase.PhaseSpecialEffects = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n")
                                                                         .FluentAppend($"exposed", true);
            phase.PhaseSuccessful += truth;
            phase.WhyPhaseNotSuccessful += error;
            phase.PhaseSpecialEffectsDescription = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n").FluentAppend(desc, true);
            Changed = true;
            actor.OutputHandler.Send($"This phase will now check whether the bodypart is uncovered, and stop if not true.");
            return true;
        }
        return base.BuildingCommandPhaseSpecial(actor, command, phase);
    }

    protected override string SpecialActionText => $@"{base.SpecialActionText}
	#Cexposed#0 - checks whether the target bodypart is uncovered, and stops if not";

    protected override (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string)
        GetSpecialPhaseAction(string actionText)
    {
        if (actionText.EqualTo("exposed"))
        {
            return ExposedPhaseSpecialAction();
        }

        return base.GetSpecialPhaseAction(actionText);
    }

    public abstract IBodypart GetTargetBodypart(object[] parameters);

    private (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string) ExposedPhaseSpecialAction()
    {
        return ((surgeon, patient, parameters) =>
                {
                    IBodypart bodypart = GetTargetBodypart(parameters);
                    if (patient.Body.ExposedBodyparts.Any(x => x.CountsAs(bodypart)))
                    {
                        return true;
                    }
                    return false;
                }
                ,
                (surgeon, patient, parameters) =>
                {
                    IBodypart bodypart = GetTargetBodypart(parameters);
                    return
                        $"$1's {bodypart.FullDescription()} is not exposed, and so the procedure cannot continue.";
                },
                "Checks part for being exposed"
            );
    }
}
