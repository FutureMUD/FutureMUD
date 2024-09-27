using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Law;

namespace MudSharp.NPC.AI;
#nullable enable
public class LawyerAI : PathingAIBase
{
	public static void RegisterLoader()
	{
		RegisterAIType("Lawyer", (ai, gameworld) => new LawyerAI(ai, gameworld));
		RegisterAIBuilderInformation("lawyer", (game, name) => new LawyerAI(game, name), new LawyerAI().HelpText);
	}

	protected LawyerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		OpenDoors = bool.Parse(root.Element("OpenDoors").Value);
		UseKeys = bool.Parse(root.Element("UseKeys").Value);
		SmashLockedDoors = bool.Parse(root.Element("SmashLockedDoors").Value);
		MoveEvenIfObstructionInWay = bool.Parse(root.Element("MoveEvenIfObstructionInWay").Value);
		UseDoorguards = bool.Parse(root.Element("UseDoorguards").Value);
		CanBeEngagedAsCourtAppointedLawyer = bool.Parse(root.Element("CanBeEngagedAsCourtAppointedLawyer").Value);
		CanBeHiredProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanBeHiredProg").Value));
		FeeProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FeeProg").Value));
		HomeBaseProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("HomeBaseProg").Value));
		BankAccountProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BankAccountProg").Value));
	}

	private LawyerAI()
	{
	}

	private LawyerAI(IFuturemud gameworld, string name) : base(gameworld, name, "Lawyer")
	{
		CanBeEngagedAsCourtAppointedLawyer = true;
		CanBeHiredProg = Gameworld.AlwaysTrueProg;
		FeeProg = Gameworld.AlwaysOneProg;
		OpenDoors = true;
		UseKeys = true;
		SmashLockedDoors = false;
		MoveEvenIfObstructionInWay = true;
		UseDoorguards = true;
		DatabaseInitialise();
	}

	/// <inheritdoc />
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("CanBeEngagedAsCourtAppointedLawyer", CanBeEngagedAsCourtAppointedLawyer),
			new XElement("CanBeHiredProg", CanBeHiredProg.Id),
			new XElement("HomeBaseProg", HomeBaseProg?.Id ?? 0),
			new XElement("BankAccountProg", BankAccountProg?.Id ?? 0),
			new XElement("FeeProg", FeeProg.Id)
		).ToString();
	}

	public bool CanBeEngagedAsCourtAppointedLawyer { get; private set; }
	public IFutureProg FeeProg { get; private set; }
	public IFutureProg CanBeHiredProg { get; private set; }
	public IFutureProg? HomeBaseProg { get; private set; }
	public IFutureProg? BankAccountProg { get; private set; }

	public bool AvailableToHire(ICharacter ch, ILegalAuthority authority, bool courtAppointed)
	{
		if (ch.AffectedBy<Lawyering>())
		{
			return false;
		}

		if (!ch.PathBetween(authority.CourtLocation, 50, GetSuitabilityFunction(ch)).Any())
		{
			return false;
		}

		if (courtAppointed && !CanBeEngagedAsCourtAppointedLawyer)
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	protected override (ICell? Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var lawyering = ch.EffectsOfType<Lawyering>().FirstOrDefault();
		if (lawyering is null)
		{
			var home = HomeBaseProg?.Execute(ch) as ICell;
			if (home is null)
			{
				return (null, []);
			}

			var path = ch.PathBetween(home, 50, GetSuitabilityFunction(ch)).ToList();
			return path.Count == 0 ? (null, []) : (home, path);
		}

		if (lawyering.LegalAuthority.CourtLocation is null)
		{
			return (null, []);
		}

		if (lawyering.LegalAuthority.CourtLocation.Characters.Any(x => x.AffectedBy<OnTrial>() && (lawyering.EngagedByCharacter is null || lawyering.EngagedByCharacter == x)))
		{
			var path = ch.PathBetween(lawyering.LegalAuthority.CourtLocation, 50, GetSuitabilityFunction(ch)).ToList();
			return path.Count == 0 ? (null, []) : (lawyering.LegalAuthority.CourtLocation, path);
		}

		return (null, []);
	}

	/// <inheritdoc />
	protected override bool FiveSecondTick(ICharacter ch)
	{
		if (base.FiveSecondTick(ch))
		{
			return true;
		}

		var lawyering = ch.EffectsOfType<Lawyering>().FirstOrDefault();
		if (lawyering?.LegalAuthority.CourtLocation is null)
		{
			return false;
		}

		var trial = lawyering.EngagedByCharacter is null ? lawyering.LegalAuthority.CourtLocation.Characters.SelectMany(x => x.EffectsOfType<OnTrial>()).FirstOrDefault() : lawyering.EngagedByCharacter.EffectsOfType<OnTrial>(x => x.LegalAuthority == lawyering.LegalAuthority).FirstOrDefault();
		if (trial is null)
		{
			return false;
		}

		if (ch.Location != lawyering.LegalAuthority.CourtLocation)
		{
			return false;
		}

		if (trial.Phase.In(TrialPhase.Case, TrialPhase.ClosingArguments) && Dice.Roll(1,3) == 1)
		{
			trial.HandleArgueCommand(ch, true);
		}
		return true;
	}

	#region Building Commands
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Can Be Court Appointed: {CanBeEngagedAsCourtAppointedLawyer.ToColouredString()}");
		sb.AppendLine($"Fee Prog: {FeeProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Can Be Hired Prog: {CanBeHiredProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Home Base Prog: {HomeBaseProg?.MXPClickableFunctionName() ?? "None"}");
		sb.AppendLine($"Bank Account Prog: {BankAccountProg?.MXPClickableFunctionName() ?? "None"}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3court#0 - toggles being able to be hired as a court appointed lawyer
	#3fee <prog>#0 - sets the prog for determining the fee for hiring
	#3hire <prog>#0 - sets the prog for determine whether a person can hire
	#3home <prog>#0 - sets the prog for the home base when not on duty
	#3bank <prog>#0 - sets the prog for specifying a bank account where the fee goes";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "court":
			case "courtappointed":
				return BuildingCommandCourtAppointed(actor, command);
			case "fee":
			case "feeprog":
				return BuildingCommandFee(actor, command);
			case "hire":
			case "hireprog":
			case "hired":
			case "hiredprog":
			case "canhire":
			case "canhireprog":
				return BuildingCommandCanHireProg(actor, command);
			case "home":
			case "homebase":
			case "homeprog":
			case "homebaseprog":
			case "baseprog":
			case "base":
				return BuildingCommandHomeBaseProg(actor, command);
			case "bankaccount":
			case "bankaccountprog":
			case "bank":
			case "bankprog":
				return BuildingCommandBankAccountProg(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBankAccountProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to specify a bank account for this lawyer's fee?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, FutureProgVariableTypes.BankAccount, [FutureProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BankAccountProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to determine its bank account.");
		return true;
	}

	private bool BuildingCommandCourtAppointed(ICharacter actor, StringStack command)
	{
		CanBeEngagedAsCourtAppointedLawyer = !CanBeEngagedAsCourtAppointedLawyer;
		Changed = true;
		actor.OutputHandler.Send($"This AI can {CanBeEngagedAsCourtAppointedLawyer.NowNoLonger()} be appointed as a court appointed lawyer.");
		return true;
	}

	private bool BuildingCommandHomeBaseProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to specify where this AI returns when it isn't practicing law?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, FutureProgVariableTypes.Location, [FutureProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		HomeBaseProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to determine its home base.");
		return true;
	}

	private bool BuildingCommandCanHireProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to determine whether someone can hire this AI?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean, 
			[
				[FutureProgVariableTypes.Character],
				[FutureProgVariableTypes.Character, FutureProgVariableTypes.Character]
			]
			).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanBeHiredProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to determine whether someone can hire it.");
		return true;
	}

	private bool BuildingCommandFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to determine the fee for hiring this AI?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, FutureProgVariableTypes.Number,
			[
				[FutureProgVariableTypes.Character],
				[FutureProgVariableTypes.Character, FutureProgVariableTypes.Character]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		FeeProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to determine the fee for hiring it.");
		return true;
	}
	#endregion
}
