using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements.Manipulation;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class EnforcerAI : ArtificialIntelligenceBase, IOverrideAlertEmote
{
    public static void RegisterLoader()
    {
        RegisterAIType("Enforcer", (ai, gameworld) => new EnforcerAI(ai, gameworld));
        RegisterAIBuilderInformation("enforcer", (gameworld, name) => new EnforcerAI(gameworld, name), new EnforcerAI().HelpText);
    }

    protected EnforcerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
    {
        XElement root = XElement.Parse(ai.Definition);
        IdentityIsKnownProg = long.TryParse(root.Element("IdentityProg")?.Value ?? "0", out long value)
            ? Gameworld.FutureProgs.Get(value)
            : Gameworld.FutureProgs.GetByName(root.Element("IdentityProg")!.Value);
        WarnEchoProg = long.TryParse(root.Element("WarnEchoProg")?.Value ?? "0", out value)
            ? Gameworld.FutureProgs.Get(value)
            : Gameworld.FutureProgs.GetByName(root.Element("WarnEchoProg")!.Value);
        WarnStartMoveEchoProg = long.TryParse(root.Element("WarnStartMoveEchoProg")?.Value ?? "0", out value)
            ? Gameworld.FutureProgs.Get(value)
            : Gameworld.FutureProgs.GetByName(root.Element("WarnStartMoveEchoProg")!.Value);
        FailToComplyEchoProg = long.TryParse(root.Element("FailToComplyEchoProg")?.Value ?? "0", out value)
            ? Gameworld.FutureProgs.Get(value)
            : Gameworld.FutureProgs.GetByName(root.Element("FailToComplyEchoProg")!.Value);
        ThrowInPrisonEchoProg = long.TryParse(root.Element("ThrowInPrisonEchoProg")?.Value ?? "0", out value)
            ? Gameworld.FutureProgs.Get(value)
            : Gameworld.FutureProgs.GetByName(root.Element("ThrowInPrisonEchoProg")!.Value);
        AlertEmote = root.Element("AlertEmote")?.Value;
        DistantAlertEmote = root.Element("DistantAlertEmote")?.Value;
    }

    protected EnforcerAI()
    {

    }

    private EnforcerAI(IFuturemud gameworld, string name) : base(gameworld, name, "Enforcer")
    {
        DatabaseInitialise();
    }

    protected EnforcerAI(IFuturemud gameworld, string name, string type) : base(gameworld, name, type)
    {
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("IdentityProg", IdentityIsKnownProg?.Id ?? 0L),
            new XElement("WarnEchoProg", WarnEchoProg?.Id ?? 0L),
            new XElement("WarnStartMoveEchoProg", WarnStartMoveEchoProg?.Id ?? 0L),
            new XElement("FailToComplyEchoProg", FailToComplyEchoProg?.Id ?? 0L),
            new XElement("ThrowInPrisonEchoProg", ThrowInPrisonEchoProg?.Id ?? 0L),
            new XElement("AlertEmote", AlertEmote ?? string.Empty),
            new XElement("DistantAlertEmote", DistantAlertEmote ?? string.Empty)
        ).ToString();
    }

    public IFutureProg IdentityIsKnownProg { get; protected set; }
    public IFutureProg WarnEchoProg { get; protected set; }
    public IFutureProg WarnStartMoveEchoProg { get; protected set; }
    public IFutureProg FailToComplyEchoProg { get; protected set; }
    public IFutureProg ThrowInPrisonEchoProg { get; protected set; }
    public string AlertEmote { get; private set; }
    public string DistantAlertEmote { get; private set; }

    /// <inheritdoc />
    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine($"Type: {AIType.ColourValue()}");
        sb.AppendLine();
        sb.AppendLine($"Identity Known Prog: {IdentityIsKnownProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
        sb.AppendLine($"Warn Echo Prog: {WarnEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
        sb.AppendLine($"Warn Start Move Prog: {WarnStartMoveEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
        sb.AppendLine($"Warn Fail Comply Prog: {FailToComplyEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
        sb.AppendLine($"Thrown In Prison Prog: {ThrowInPrisonEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
        sb.AppendLine($"Alert Emote: {AlertEmote?.ColourCommand() ?? "None".ColourError()}");
        sb.AppendLine($"Distant Alert Emote: {DistantAlertEmote?.ColourCommand() ?? "None".ColourError()}");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string TypeHelpText => @"	#3identity <prog>#0 - sets a prog that controls if an individual's identity is known to the enforcer
	#3warn <prog>#0 - sets the prog executed when a wanted criminal is spotted
	#3warn clear#0 - clears the warn prog
	#3warnmove <prog>#0 - sets the prog executed when a previously warned criminal begins to move
	#3warnmove clear#0 - clears the warn move prog
	#3warncomply <prog>#0 - sets the prog executed when a warned criminal doesn't comply after a time
	#3warncomply clear#0 - clears the warn comply prog
	#3prison <prog>#0 - sets the prog executed when the enforcer throws someone in a jail cell
	#3prison clear#0 - clears the prison prog
	#3alertemote <emote|clear>#0 - sets an AI-specific ALERT emote
	#3alertfar <emote|clear>#0 - sets an AI-specific distant ALERT echo; use {0} for the direction

#BNote - all the progs with this AI return commands to be executed. Enter multiple commands separated by newlines in the text the prog returns. You can do additional things in the prog too - you can just return an empty text if you want the NPCs not to execute any commands but instead handle the logic in the prog some other way.#0";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "identity":
            case "identityprog":
                return BuildingCommandIdentityProg(actor, command);
            case "warn":
            case "warnprog":
                return BuildingCommandWarnProg(actor, command);
            case "warnmove":
            case "warnmoveprog":
                return BuildingCommandWarnMoveProg(actor, command);
            case "warnfailcomply":
            case "warncomply":
            case "warnfailcomplyprog":
            case "warncomplyprog":
                return BuildingCommandWarnFailComplyProg(actor, command);
            case "prison":
            case "prisonprog":
                return BuildingCommandPrisonProg(actor, command);
            case "alert":
            case "alertemote":
            case "alertlocal":
                return BuildingCommandAlertEmote(actor, command);
            case "alertfar":
            case "alertdistant":
            case "distantalert":
            case "distantalertemote":
                return BuildingCommandDistantAlertEmote(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandAlertEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What ALERT emote should this AI use, or #3clear#0 to clear it?".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "none", "delete", "remove"))
        {
            AlertEmote = null;
            Changed = true;
            actor.OutputHandler.Send("This AI will now use its race or global default ALERT emote.");
            return true;
        }

        var emoteText = command.SafeRemainingArgument;
        if (!AlertUtilities.ValidateAlertEmote(emoteText, actor, out var error))
        {
            actor.OutputHandler.Send(error);
            return false;
        }

        AlertEmote = emoteText;
        Changed = true;
        actor.OutputHandler.Send($"This AI's ALERT emote is now {AlertEmote.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandDistantAlertEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What distant ALERT echo should this AI use, or #3clear#0 to clear it? Use #6{0}#0 for the direction text.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "none", "delete", "remove"))
        {
            DistantAlertEmote = null;
            Changed = true;
            actor.OutputHandler.Send("This AI will now use its race or global default distant ALERT echo.");
            return true;
        }

        var emoteText = command.SafeRemainingArgument;
        if (!AlertUtilities.ValidateDistantAlertEmote(emoteText, actor, out var error))
        {
            actor.OutputHandler.Send(error);
            return false;
        }

        DistantAlertEmote = emoteText;
        Changed = true;
        actor.OutputHandler.Send($"This AI's distant ALERT echo is now {DistantAlertEmote.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandPrisonProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a prog or use #3clear#0 to clear.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
        {
            ThrowInPrisonEchoProg = null;
            Changed = true;
            actor.OutputHandler.Send("This AI will no longer execute any actions when it throws someone in a cell.");
            return true;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
            ProgVariableTypes.Text,
            new[]
            {
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character
                },
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Crime
                }
            }
        ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        ThrowInPrisonEchoProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to return commands to execute when it throws someone in a cell.");
        return true;
    }

    private bool BuildingCommandWarnFailComplyProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a prog or use #3clear#0 to clear.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
        {
            FailToComplyEchoProg = null;
            Changed = true;
            actor.OutputHandler.Send("This AI will no longer execute any actions when the criminal fails to comply.");
            return true;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
            ProgVariableTypes.Text,
            new[]
            {
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character
                },
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Crime
                }
            }
            ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        FailToComplyEchoProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to return commands to execute when a criminal fails to comply.");
        return true;
    }

    private bool BuildingCommandWarnMoveProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a prog or use #3clear#0 to clear.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
        {
            WarnStartMoveEchoProg = null;
            Changed = true;
            actor.OutputHandler.Send("This AI will no longer execute any actions when the criminal starts to move.");
            return true;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
            ProgVariableTypes.Text,
            new[]
            {
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character
                },
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Crime
                }
            }
        ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        WarnStartMoveEchoProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to return commands to execute when a criminal begins moving.");
        return true;
    }

    private bool BuildingCommandWarnProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a prog or use #3clear#0 to clear.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
        {
            WarnEchoProg = null;
            Changed = true;
            actor.OutputHandler.Send("This AI will no longer execute any actions when it identifies a wanted criminal.");
            return true;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
            ProgVariableTypes.Text,
            new[]
            {
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character
                },
                new List<ProgVariableTypes>
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Crime
                }
            }
        ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        WarnEchoProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to return commands to execute when it identifies a wanted criminal.");
        return true;
    }

    private bool BuildingCommandIdentityProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a prog or use #3clear#0 to clear.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
        {
            IdentityIsKnownProg = null;
            Changed = true;
            actor.OutputHandler.Send("You clear the prog used to determine if a criminal's identity is known.");
            return true;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
            ProgVariableTypes.Boolean, new List<ProgVariableTypes>
            {
                ProgVariableTypes.Character,
                ProgVariableTypes.Character
            }).LookupProg();
        if (prog is null)
        {
            return false;
        }

        IdentityIsKnownProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to control whether it knows the identity of individuals.");
        return true;
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        ICharacter ch = null;
        switch (type)
        {
            case EventType.CharacterIncapacitatedWitness:

                ch = (ICharacter)arguments[1];
                break;
            case EventType.TargetIncapacitated:
            case EventType.NoLongerEngagedInMelee:
            case EventType.TargetSlain:
            case EventType.TruceOffered:
            case EventType.FiveSecondTick:
                ch = (ICharacter)arguments[0];
                break;
            case EventType.WitnessedCrime:
                ch = (ICharacter)arguments[2];
                break;
            case EventType.CharacterAlertHeard:
                ch = (ICharacter)arguments[1];
                break;
            case EventType.EngageInCombat:
                ch = (ICharacter)arguments[0];
                break;
            case EventType.EngagedInCombat:
                ch = (ICharacter)arguments[1];
                break;
        }

        if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
        {
            return false;
        }

        switch (type)
        {
			case EventType.CharacterIncapacitatedWitness:
				return CharacterIncapacitatedWitness((ICharacter)arguments[0], ch);
			case EventType.TargetIncapacitated:
				return TargetIncapacitated((ICharacter)arguments[1], ch);
            case EventType.NoLongerEngagedInMelee:
            case EventType.TargetSlain:
            case EventType.TruceOffered:
                return false;
            case EventType.WitnessedCrime:
                return WitnessedCrime((ICharacter)arguments[0], (ICharacter)arguments[1], ch,
                    (ICrime)arguments[3]);
            case EventType.CharacterAlertHeard:
                return CharacterAlertHeard((ICharacter)arguments[0], ch, (ICell)arguments[2]);
            case EventType.EngageInCombat:
            case EventType.EngagedInCombat:
                return EnforcerEnteredCombat(ch);
            case EventType.FiveSecondTick:
                return CharacterFiveSecondTick(ch);
        }

        return false;
    }

	private bool CharacterIncapacitatedWitness(ICharacter victim, ICharacter character)
	{
		EnforcerEffect effect = EnforcerEffect(character);
		return effect is not null && TryBeginIndependentCustody(character, victim, effect);
	}

	private bool TargetIncapacitated(ICharacter victim, ICharacter character)
	{
		PatrolMemberEffect patrolMember = character.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault();
		EnforcerEffect effect = EnforcerEffect(character);
		if (patrolMember is null && effect is not null && TryBeginIndependentCustody(character, victim, effect))
		{
			return true;
		}

		IPatrol patrol = patrolMember?.Patrol;
		if (patrol is not null &&
		    CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(victim, patrol.ActiveEnforcementTarget))
		{
			if (patrol.ActiveEnforcementCrime?.Law.EnforcementStrategy.ShowMercyToIncapacitatedTarget() == false)
			{
				return false;
			}

			character.Combat?.TruceRequested(character);
			patrol.PatrolStrategy.HandlePatrolTick(patrol);
			return true;
		}

		character.Combat?.TruceRequested(character);

		return false;
	}

    protected EnforcerEffect EnforcerEffect(ICharacter enforcer)
    {
        return enforcer.EffectsOfType<EnforcerEffect>().FirstOrDefault();
    }

    private void ReportVisibleCorpses(ICharacter enforcer)
    {
        bool reportedAny = false;
        foreach (IGameItem corpseItem in enforcer.Location.LayerGameItems(enforcer.RoomLayer)
                                             .Where(x => enforcer.CanSee(x))
                                             .Where(x => x.GetItemType<ICorpse>() is { RepresentsFinalCharacterDeath: true }))
        {
            if (MudSharp.RPG.Law.LegalAuthority.ReportCorpseToLocalAuthority(Gameworld, corpseItem, enforcer, out _) != null)
            {
                reportedAny = true;
            }
        }

        if (reportedAny)
        {
            enforcer.OutputHandler.Handle(new EmoteOutput(new Emote("@ report|reports a corpse to the authorities.",
                enforcer)));
		}
	}

	private bool TryHandleIndependentCustody(ICharacter enforcer, EnforcerEffect effect)
	{
		if (!CanActIndependently(enforcer, effect))
		{
			return false;
		}

		if (TryContinueIndependentCustody(enforcer, effect))
		{
			return true;
		}

		foreach (var criminal in enforcer.Location.LayerCharacters(enforcer.RoomLayer))
		{
			if (TryBeginIndependentCustody(enforcer, criminal, effect))
			{
				return true;
			}
		}

		return false;
	}

	private bool TryBeginIndependentCustody(ICharacter enforcer, ICharacter criminal, EnforcerEffect effect)
	{
		if (!CanActIndependently(enforcer, effect))
		{
			return false;
		}

		if (!CanTakeIndependentCustody(enforcer, criminal, effect, out ICrime crime))
		{
			return false;
		}

		enforcer.RemoveAllEffects<FollowingPath>(fireRemovalAction: true);
		if (EnforcementCustodyHelper.BeginDragging(enforcer, criminal, Enumerable.Empty<ICharacter>()) is null)
		{
			return false;
		}

		EnforcementCustodyHelper.ReleaseGrapplesAndCombatAgainst(criminal, new[] { enforcer });
		criminal.RemoveAllEffects<WarnedByEnforcer>(x => x.WhichAuthority == effect.LegalAuthority, true);
		return MoveIndependentCustodyToPrison(enforcer, effect, criminal, crime);
	}

	private bool TryContinueIndependentCustody(ICharacter enforcer, EnforcerEffect effect)
	{
		foreach (var drag in enforcer.CombinedEffectsOfType<Dragging>())
		{
			if (drag.Target is not ICharacter criminal)
			{
				continue;
			}

			ICrime crime = EnforcementCustodyHelper.SelectArrestableCrime(effect.LegalAuthority, criminal);
			if (crime is null)
			{
				continue;
			}

			return MoveIndependentCustodyToPrison(enforcer, effect, criminal, crime);
		}

		return false;
	}

	private bool CanActIndependently(ICharacter enforcer, EnforcerEffect effect)
	{
		return enforcer.State.IsAble() &&
		       !enforcer.CombinedEffectsOfType<PatrolMemberEffect>().Any() &&
		       effect.LegalAuthority.GetEnforcementAuthority(enforcer) is not null;
	}

	private bool CanTakeIndependentCustody(ICharacter enforcer, ICharacter criminal, EnforcerEffect effect, out ICrime crime)
	{
		crime = null;
		if (criminal is null ||
		    CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(enforcer, criminal) ||
		    !criminal.ColocatedWith(enforcer) ||
		    !criminal.IsHelpless ||
		    criminal.State.IsDead() ||
		    criminal.State.IsInStatis() ||
		    criminal.IdentityIsObscured ||
		    criminal.AffectedBy<InCustodyOfEnforcer>(effect.LegalAuthority) ||
		    criminal.AffectedBy<Dragging.DragTarget>(effect.LegalAuthority) ||
		    criminal.AffectedBy<OnTrial>(effect.LegalAuthority))
		{
			return false;
		}

		if (criminal.Combat is not null &&
		    criminal.MeleeRange &&
		    criminal.Combat.Combatants
		            .OfType<ICharacter>()
		            .Any(x =>
			            !CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x, criminal) &&
			            !CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(x, enforcer)))
		{
			return false;
		}

		crime = EnforcementCustodyHelper.SelectArrestableCrime(effect.LegalAuthority, criminal);
		return crime is not null;
	}

	private bool MoveIndependentCustodyToPrison(ICharacter enforcer, EnforcerEffect effect, ICharacter criminal, ICrime crime)
	{
		var authority = effect.LegalAuthority;
		if (enforcer.Location == authority.PrisonLocation)
		{
			criminal.RemoveAllEffects<Dragging.DragTarget>(fireRemovalAction: true);
			foreach (string action in (ThrowInPrisonEchoProg?.Execute<string>(enforcer, criminal, crime) ??
			                           string.Empty).Split('\n'))
			{
				enforcer.ExecuteCommand(action);
			}

			authority.IncarcerateCriminal(criminal);
			authority.OnPrisonerImprisoned?.Execute(criminal);
			return true;
		}

		FollowingPath fp = enforcer.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
		if (fp is not null && fp.Exits.LastOrDefault()?.Destination != authority.PrisonLocation)
		{
			enforcer.RemoveEffect(fp, true);
			fp = null;
		}

		if (fp is not null)
		{
			if (enforcer.CouldMove(false, null).Success)
			{
				fp.FollowPathAction();
			}

			return true;
		}

		var path = enforcer.PathBetween(authority.PrisonLocation, 50,
			PathSearch.PathIncludeUnlockableDoors(enforcer)).ToList();
		if (!path.Any())
		{
			return true;
		}

		fp = FollowingPath.CreateFullFriendlyPath(enforcer, path, closeDoorsBehind: true);
		enforcer.AddEffect(fp);
		if (enforcer.CouldMove(false, null).Success)
		{
			fp.FollowPathAction();
		}

		return true;
	}

	private bool WitnessedCrime(ICharacter criminal, ICharacter victim, ICharacter enforcer, ICrime crime)
	{
		// Enforcers always report crimes whether they're on duty or off duty
		crime.LegalAuthority.ReportCrime(crime, enforcer,
            IdentityIsKnownProg?.Execute<bool?>(enforcer, criminal) == true, 1.0);
        return false;
    }

    private bool EnforcerEnteredCombat(ICharacter enforcer)
    {
        if (EnforcerEffect(enforcer) == null)
        {
            return false;
        }

        return AlertUtilities.DoAlert(enforcer, echoFailure: false);
    }

    private bool CharacterAlertHeard(ICharacter alerter, ICharacter enforcer, ICell origin)
    {
        if (alerter == enforcer || origin is null || EnforcerEffect(enforcer) == null)
        {
            return false;
        }

        var patrolMember = enforcer.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault();
        if (patrolMember is null || patrolMember.Patrol.ActiveEnforcementTarget is not null)
        {
            return false;
        }

        if (!IsGenerallyAble(enforcer, ignoreMovement: true) || enforcer.Combat is not null)
        {
            return false;
        }

        if (enforcer.Location == origin)
        {
            return AssistNearbyEnforcerInCombat(enforcer, EnforcerEffect(enforcer));
        }

        var path = enforcer.PathBetween(origin, 20, PathSearch.PathIncludeUnlockableDoors(enforcer)).ToList();
        if (!path.Any())
        {
            return false;
        }

        enforcer.RemoveAllEffects<FollowingPath>(fireRemovalAction: true);
        var fp = FollowingPath.CreateFullFriendlyPath(enforcer, path, closeDoorsBehind: true);
        enforcer.AddEffect(fp);
        fp.FollowPathAction();
        return true;
    }

    private bool AssistNearbyEnforcerInCombat(ICharacter enforcer, EnforcerEffect enforcerEffect)
    {
        if (enforcerEffect is null || enforcer.Combat is not null)
        {
            return false;
        }

        var ally = enforcer.Location.Characters
                           .Except(enforcer)
                           .Where(x => x.Combat is not null)
                           .FirstOrDefault(x =>
                               x.EffectsOfType<EnforcerEffect>().Any(y => y.LegalAuthority == enforcerEffect.LegalAuthority) &&
                               x.CombatTarget is not null &&
                               x.CombatTarget != enforcer);
        if (ally?.CombatTarget is null || !enforcer.CanEngage(ally.CombatTarget))
        {
            return false;
        }

        enforcer.Engage(ally.CombatTarget, false);
        return true;
    }

    private bool HandleGeneral(ICharacter enforcer)
    {
        // If not currently on a patrol, ignore AI
        EnforcerEffect effect = EnforcerEffect(enforcer);
        if (effect == null)
        {
            return true;
        }

        // Pause AI while moving or in unrelated combat
        if (enforcer.Movement != null ||
            enforcer.Combat != null &&
            enforcer.CombinedEffectsOfType<PatrolMemberEffect>().All(x =>
                x.Patrol?.ActiveEnforcementTarget is null ||
                enforcer.CombatTarget != x.Patrol.ActiveEnforcementTarget))
        {
            return true;
        }

        PatrolMemberEffect patrolMember = enforcer.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault();
        if (patrolMember == null && enforcer.Location == effect.LegalAuthority.EnforcerStowingLocation)
        {
            return true;
        }

        if (patrolMember?.Patrol.PatrolLeader is null)
        {
            patrolMember?.Patrol.AbortPatrol();
            return true;
        }

        // Try to wake up if somehow put to sleep
        if (enforcer.State.IsAsleep())
        {
            enforcer.Awaken();
            return true;
        }

        // Try to stand up if knocked over
        IPositionState mobilePosition = enforcer.MostUprightMobilePosition();
        if (mobilePosition != null && enforcer.PositionState != mobilePosition)
        {
            enforcer.MovePosition(mobilePosition, null, null);
            return true;
        }

        // If already following a path, don't do other things
        if (enforcer.AffectedBy<FollowingPath>())
        {
            return false;
        }

        if (patrolMember == null)
        {
            return false;
        }

        IPatrol patrol = patrolMember.Patrol;
        // If not the patrol leader and not in the same place as the patrol leader, try to regroup with them
        if (patrol.PatrolLeader != enforcer && !enforcer.ColocatedWith(patrol.PatrolLeader))
        {
            List<ICellExit> path = enforcer.PathBetween(patrol.PatrolLeader, 10, PathSearch.PathIncludeUnlockableDoors(enforcer))
                               .ToList();
            if (path.Any())
            {
                FollowingPath fp = FollowingPath.CreateFullFriendlyPath(enforcer, path, closeDoorsBehind: true);
                enforcer.AddEffect(fp);
                fp.FollowPathAction();
                return true;
            }

            return true;
        }

        if (enforcer.Party != patrol.PatrolLeader.Party && patrol.PatrolLeader.Party != null)
        {
            enforcer.JoinParty(patrol.PatrolLeader.Party);
        }

        return false;
    }

    protected virtual bool CharacterFiveSecondTick(ICharacter enforcer)
    {
        EnforcerEffect effect = EnforcerEffect(enforcer);
        if (effect == null)
        {
            return false;
        }

		ReportVisibleCorpses(enforcer);

		if (TryHandleIndependentCustody(enforcer, effect))
		{
			return true;
		}

		if (HandleGeneral(enforcer))
		{
			return false;
		}

        if (AssistNearbyEnforcerInCombat(enforcer, effect))
        {
            return true;
        }

        IPatrol patrol = enforcer.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault()?.Patrol;
        FollowingPath fp = enforcer.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
        if (patrol != null)
        {
            if (HandlePatrolMember(enforcer, patrol))
            {
                if (fp != null)
                {
                    if (enforcer == patrol?.PatrolLeader && patrol.PatrolPhase == PatrolPhase.Patrol &&
                        patrol.ActiveEnforcementTarget is null &&
                        DateTime.UtcNow - patrol.LastArrivedTime < patrol.PatrolRoute.LingerTimeMinorNode)
                    {
                        return false;
                    }

                    if (enforcer.CouldMove(false, null).Success)
                    {
                        fp.FollowPathAction();
                    }
                }

                return true;
            }
        }

        if (fp != null)
        {
            if (enforcer == patrol?.PatrolLeader && patrol.PatrolPhase == PatrolPhase.Patrol &&
                patrol.ActiveEnforcementTarget is null &&
                DateTime.UtcNow - patrol.LastArrivedTime < patrol.PatrolRoute.LingerTimeMinorNode)
            {
                return false;
            }

            if (enforcer.CouldMove(false, null).Success)
            {
                fp.FollowPathAction();
            }

            return true;
        }

        if (patrol == null && enforcer.Location != effect.LegalAuthority.EnforcerStowingLocation)
        {
            List<ICellExit> path = enforcer.PathBetween(effect.LegalAuthority.EnforcerStowingLocation, 50,
                PathSearch.PathIncludeUnlockableDoors(enforcer)).ToList();
            if (path.Any())
            {
                fp = FollowingPath.CreateFullFriendlyPath(enforcer, path, closeDoorsBehind: true);
                enforcer.AddEffect(fp);
                fp.FollowPathAction();
            }
        }

        return false;
    }

    private bool HandlePatrolMember(ICharacter enforcer, IPatrol patrol)
    {
        if (patrol.ActiveEnforcementTarget != null)
        {
            return true;
        }

        if (patrol.PatrolLeader == enforcer)
        {
            FollowingPath fp = enforcer.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
            if (fp != null && enforcer.CouldMove(false, null).Success)
            {
                bool major = patrol.PatrolRoute.PatrolNodes.Contains(enforcer.Location);
                if ((major && DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMajorNode) ||
                    (!major && DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMinorNode))
                {
                    fp.FollowPathAction();
                    return true;
                }
            }

            if (patrol.PatrolPhase == PatrolPhase.Patrol)
            {
                return true;
            }
        }

        return false;
    }

    public override bool HandlesEvent(params EventType[] types)
    {
        foreach (EventType type in types)
        {
            switch (type)
            {
                case EventType.CharacterIncapacitatedWitness:
                case EventType.TargetIncapacitated:
                case EventType.NoLongerEngagedInMelee:
                case EventType.TargetSlain:
                case EventType.TruceOffered:
                case EventType.WitnessedCrime:
                case EventType.CharacterAlertHeard:
                case EventType.EngageInCombat:
                case EventType.EngagedInCombat:
                case EventType.FiveSecondTick:
                    return true;
            }
        }

        return false;
    }
}
