using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.RPG.Law;
using MudSharp.FutureProg.Statements.Manipulation;

namespace MudSharp.NPC.AI;

public class EnforcerAI : ArtificialIntelligenceBase
{
	public static void RegisterLoader()
	{
		RegisterAIType("Enforcer", (ai, gameworld) => new EnforcerAI(ai, gameworld));
		RegisterAIBuilderInformation("enforcer", (gameworld, name) => new EnforcerAI(gameworld, name), new EnforcerAI().HelpText);
	}

	protected EnforcerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		IdentityIsKnownProg = long.TryParse(root.Element("IdentityProg")?.Value ?? "0", out var value)
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
			new XElement("ThrowInPrisonEchoProg", ThrowInPrisonEchoProg?.Id ?? 0L)
		).ToString();
	}

	public IFutureProg IdentityIsKnownProg { get; protected set; }
	public IFutureProg WarnEchoProg { get; protected set; }
	public IFutureProg WarnStartMoveEchoProg { get; protected set; }
	public IFutureProg FailToComplyEchoProg { get; protected set; }
	public IFutureProg ThrowInPrisonEchoProg { get; protected set; }

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Identity Known Prog: {IdentityIsKnownProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Warn Echo Prog: {WarnEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Warn Start Move Prog: {WarnStartMoveEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Warn Fail Comply Prog: {FailToComplyEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Thrown In Prison Prog: {ThrowInPrisonEchoProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
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
		}
		return base.BuildingCommand(actor, command.GetUndo());
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

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Text,
			new[]
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Crime
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

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Text,
			new []
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Crime
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

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Text,
			new[]
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Crime
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

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Text,
			new[]
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Crime
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

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
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
				return TargetIncapacitated(ch, (ICharacter)arguments[0]);
			case EventType.NoLongerEngagedInMelee:
			case EventType.TargetSlain:
			case EventType.TruceOffered:
				return false;
			case EventType.WitnessedCrime:
				return WitnessedCrime((ICharacter)arguments[0], (ICharacter)arguments[1], ch,
					(ICrime)arguments[3]);
			case EventType.FiveSecondTick:
				return CharacterFiveSecondTick(ch);
		}

		return false;
	}

	private bool CharacterIncapacitatedWitness(ICharacter victim, ICharacter character)
	{
		return false;
	}

	private bool TargetIncapacitated(ICharacter victim, ICharacter character)
	{
		var patrolMember = character.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault();
		if (patrolMember == null || patrolMember.Patrol.ActiveEnforcementTarget != victim || patrolMember.Patrol
			    .ActiveEnforcementCrime?.Law.EnforcementStrategy.ShowMercyToIncapacitatedTarget() !=
		    false)
		{
			character.Combat?.TruceRequested(character);
		}

		return false;
	}

	protected EnforcerEffect EnforcerEffect(ICharacter enforcer)
	{
		return enforcer.EffectsOfType<EnforcerEffect>().FirstOrDefault();
	}

	private bool WitnessedCrime(ICharacter criminal, ICharacter victim, ICharacter enforcer, ICrime crime)
	{
		// Enforcers always report crimes whether they're on duty or off duty
		crime.LegalAuthority.ReportCrime(crime, enforcer,
			IdentityIsKnownProg?.Execute<bool?>(enforcer, criminal) == true, 1.0);
		return false;
	}

	private bool HandleGeneral(ICharacter enforcer)
	{
		// If not currently on a patrol, ignore AI
		var effect = EnforcerEffect(enforcer);
		if (effect == null)
		{
			return true;
		}

		// Pause AI while moving or in combat
		if (enforcer.Movement != null || enforcer.Combat != null)
		{
			return true;
		}

		var patrolMember = enforcer.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault();
		if (patrolMember == null && enforcer.Location == effect.LegalAuthority.EnforcerStowingLocation)
		{
			return true;
		}

		// Try to wake up if somehow put to sleep
		if (enforcer.State.IsAsleep())
		{
			enforcer.Awaken();
			return true;
		}

		// Try to stand up if knocked over
		var mobilePosition = enforcer.MostUprightMobilePosition();
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

		var patrol = patrolMember.Patrol;
		// If not the patrol leader and not in the same place as the patrol leader, try to regroup with them
		if (patrol.PatrolLeader != enforcer && !enforcer.ColocatedWith(patrol.PatrolLeader))
		{
			var path = enforcer.PathBetween(patrol.PatrolLeader, 10, PathSearch.PathIncludeUnlockableDoors(enforcer))
			                   .ToList();
			if (path.Any())
			{
				var fp = new FollowingPath(enforcer, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
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
		var effect = EnforcerEffect(enforcer);
		if (effect == null)
		{
			return false;
		}

		if (HandleGeneral(enforcer))
		{
			return false;
		}

		var patrol = enforcer.CombinedEffectsOfType<PatrolMemberEffect>().FirstOrDefault()?.Patrol;
		var fp = enforcer.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
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
			var path = enforcer.PathBetween(effect.LegalAuthority.EnforcerStowingLocation, 50,
				PathSearch.PathIncludeUnlockableDoors(enforcer)).ToList();
			if (path.Any())
			{
				fp = new FollowingPath(enforcer, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
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
			var fp = enforcer.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
			if (fp != null && enforcer.CouldMove(false, null).Success)
			{
				var major = patrol.PatrolRoute.PatrolNodes.Contains(enforcer.Location);
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
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterIncapacitatedWitness:
				case EventType.TargetIncapacitated:
				case EventType.NoLongerEngagedInMelee:
				case EventType.TargetSlain:
				case EventType.TruceOffered:
				case EventType.WitnessedCrime:
				case EventType.FiveSecondTick:
					return true;
			}
		}

		return false;
	}
}