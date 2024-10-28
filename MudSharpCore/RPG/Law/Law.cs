using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy.Currency;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.RPG.Law;

public class Law : SaveableItem, ILaw
{
	public Law(string name, ILegalAuthority authority, CrimeTypes crime)
	{
		Gameworld = authority.Gameworld;
		_name = name;
		Authority = authority;
		CrimeType = crime;
		CanBeAppliedAutomatically = false;
		CanBeArrested = false;
		CanBeOfferedBail = false;
		ActivePeriod = TimeSpan.FromSeconds(Gameworld.GetStaticDouble("DefaultCrimeActiveSeconds"));
		if (Gameworld.GetStaticBool("ApplyAllLegalClassesAsVictimsByDefault"))
		{
			_victimClasses.AddRange(Authority.LegalClasses);
		}

		if (Gameworld.GetStaticBool("ApplyAllLegalClassesAsOffendersByDefault"))
		{
			_offenderClasses.AddRange(Authority.LegalClasses);
		}

		EnforcementPriority = Gameworld.GetStaticInt("DefaultEnforcementPriority");
		EnforcementStrategy = EnforcementStrategy.NoActiveEnforcement;
		DoNotAutomaticallyApplyRepeats = true;
		using (new FMDB())
		{
			var dbitem = new Models.Law();
			FMDB.Context.Laws.Add(dbitem);
			dbitem.Name = Name;
			dbitem.LegalAuthorityId = Authority.Id;
			dbitem.CrimeType = (int)CrimeType;
			dbitem.CanBeAppliedAutomatically = CanBeAppliedAutomatically;
			dbitem.CanBeArrested = CanBeArrested;
			dbitem.CanBeOfferedBail = CanBeOfferedBail;
			dbitem.ActivePeriod = ActivePeriod.TotalSeconds;
			dbitem.EnforcementStrategy = EnforcementStrategy.DescribeEnum();
			dbitem.EnforcementPriority = EnforcementPriority;
			dbitem.DoNotAutomaticallyApplyRepeats = DoNotAutomaticallyApplyRepeats;
			dbitem.PunishmentStrategy = PunishmentStrategy.SaveResult();
			foreach (var @class in _victimClasses)
			{
				dbitem.LawsVictimClasses.Add(new Models.LawsVictimClasses { Law = dbitem, LegalClassId = @class.Id });
			}

			foreach (var @class in _offenderClasses)
			{
				dbitem.LawsOffenderClasses.Add(
					new Models.LawsOffenderClasses { Law = dbitem, LegalClassId = @class.Id });
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Law(MudSharp.Models.Law dbitem, ILegalAuthority authority, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Authority = authority;
		CrimeType = (CrimeTypes)dbitem.CrimeType;
		CanBeAppliedAutomatically = dbitem.CanBeAppliedAutomatically;
		CanBeArrested = dbitem.CanBeArrested;
		CanBeOfferedBail = dbitem.CanBeOfferedBail;
		LawAppliesProg = Gameworld.FutureProgs.Get(dbitem.LawAppliesProgId ?? 0);
		ActivePeriod = TimeSpan.FromSeconds(dbitem.ActivePeriod);
		EnforcementStrategy = Enum.Parse<EnforcementStrategy>(dbitem.EnforcementStrategy);
		EnforcementPriority = dbitem.EnforcementPriority;
		DoNotAutomaticallyApplyRepeats = dbitem.DoNotAutomaticallyApplyRepeats;
		PunishmentStrategy =
			PunishmentStrategies.PunishmentStrategyFactory.LoadStrategy(Gameworld, dbitem.PunishmentStrategy,
				authority);
		foreach (var @class in dbitem.LawsVictimClasses)
		{
			_victimClasses.Add(Gameworld.LegalClasses.Get(@class.LegalClassId));
		}

		foreach (var @class in dbitem.LawsOffenderClasses)
		{
			_offenderClasses.Add(Gameworld.LegalClasses.Get(@class.LegalClassId));
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Laws.Find(Id);
		dbitem.Name = Name;
		dbitem.LegalAuthorityId = Authority.Id;
		dbitem.CrimeType = (int)CrimeType;
		dbitem.CanBeAppliedAutomatically = CanBeAppliedAutomatically;
		dbitem.CanBeArrested = CanBeArrested;
		dbitem.CanBeOfferedBail = CanBeOfferedBail;
		dbitem.ActivePeriod = ActivePeriod.TotalSeconds;
		dbitem.EnforcementStrategy = EnforcementStrategy.DescribeEnum();
		dbitem.EnforcementPriority = EnforcementPriority;
		dbitem.LawAppliesProgId = LawAppliesProg?.Id;
		dbitem.DoNotAutomaticallyApplyRepeats = DoNotAutomaticallyApplyRepeats;
		dbitem.PunishmentStrategy = PunishmentStrategy.SaveResult();
		FMDB.Context.LawsVictimClasses.RemoveRange(dbitem.LawsVictimClasses);
		foreach (var @class in _victimClasses)
		{
			dbitem.LawsVictimClasses.Add(new Models.LawsVictimClasses { Law = dbitem, LegalClassId = @class.Id });
		}

		FMDB.Context.LawsOffenderClasses.RemoveRange(dbitem.LawsOffenderClasses);
		foreach (var @class in _offenderClasses)
		{
			dbitem.LawsOffenderClasses.Add(new Models.LawsOffenderClasses { Law = dbitem, LegalClassId = @class.Id });
		}

		Changed = false;
	}

	public sealed override string FrameworkItemType => "Law";
	public ILegalAuthority Authority { get; protected set; }
	public CrimeTypes CrimeType { get; protected set; }
	public IFutureProg LawAppliesProg { get; protected set; }
	public Func<ICharacter, ICharacter, IGameItem, long, string, bool> LawAppliesInvoker { get; protected set; }
	public bool CanBeAppliedAutomatically { get; protected set; }
	public bool DoNotAutomaticallyApplyRepeats { get; protected set; }
	private readonly List<ILegalClass> _victimClasses = new();
	public IEnumerable<ILegalClass> VictimClasses => _victimClasses;

	private readonly List<ILegalClass> _offenderClasses = new();
	public IEnumerable<ILegalClass> OffenderClasses => _offenderClasses;

	public bool CanBeArrested { get; protected set; }
	public bool CanBeOfferedBail { get; protected set; }
	public TimeSpan ActivePeriod { get; protected set; }
	public int EnforcementPriority { get; protected set; }
	public EnforcementStrategy EnforcementStrategy { get; protected set; }
	public IPunishmentStrategy PunishmentStrategy { get; protected set; }

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			Gameworld.SaveManager.Flush();
			using (new FMDB())
			{
				var dbitem = FMDB.Context.Laws.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.Laws.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void RemoveAllReferencesTo(ILegalClass legalClass)
	{
		if (_victimClasses.Contains(legalClass))
		{
			_victimClasses.Remove(legalClass);
			Changed = true;
		}

		if (_offenderClasses.Contains(legalClass))
		{
			_offenderClasses.Remove(legalClass);
			Changed = true;
		}
	}

	public void ApplyInflation(decimal rate)
	{
		// TODO
		Changed = true;
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "show":
			case "view":
			case "":
				return BuildingCommandShow(actor);
			case "crime":
				return BuildingCommandCrime(actor, command);
			case "name":
				return BuildingCommandName(actor, command);
			case "auto":
			case "automatic":
				return BuildingCommandAutomatic(actor, command);
			case "arrest":
			case "arrestable":
				return BuildingCommandArrest(actor, command);
			case "bail":
				return BuildingCommandBail(actor, command);
			case "active":
			case "activeperiod":
			case "period":
			case "time":
				return BuildingCommandActivePeriod(actor, command);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "strategy":
			case "strat":
				return BuildingCommandStrategy(actor, command);
			case "punish":
			case "punishment":
			case "punishmentstrategy":
				return BuildingCommandPunishmentStrategy(actor, command);
			case "victim":
				return BuildingCommandVictim(actor, command);
			case "offender":
			case "criminal":
				return BuildingCommandOffender(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "repeat":
			case "repeats":
				return BuildingCommandRepeats(actor);
		}

		actor.OutputHandler.Send(@"You can use the following options with this building sub-command:

	#3show#0 - shows detailed information about the law
	#3crime <type>#0 - sets the hard-coded crime that the law matches
	#3name <name>#0 - renames the law
	#3auto#0 - toggles whether crimes against this law are automatically handled by the engine
	#3arrest#0 - toggles whether people will be arrested for this crime
	#3bail#0 - toggles whether people can get bail when arrested for this crime
	#3priority <number>#0 - sets the priority for this crime to be investigated
	#3victim <class>#0 - toggles a particular class being an eligable victim of this crime
	#3offender <class>#0 - toggles a particular class being an eligable perpetrator of this crime
	#3prog clear#0 - clears any filtering prog
	#3prog <prog>#0 - sets a prog as the filter prog to determine whether it applies
	#3period <time>#0 - sets how long this time will go before becoming a cold case
	#3strategy <which>#0 - sets the enforcement strategy
	#3repeats#0 - toggles whether to repeatedly apply a crime in a short period
	#3punishment type <type>#0 - changes the type of punishment strategy
	#3punishment ...#0 - edits some properties of the punishment strategy".SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandPunishmentStrategy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(PunishmentStrategy.Show(actor));
			return true;
		}

		switch (command.PopForSwitch())
		{
			case "type":
				return BuildingCommandPunishmentStrategyType(actor, command);
			default:
				var outcome = PunishmentStrategy.BuildingCommand(actor, Authority, command.GetUndo());
				if (outcome)
				{
					Changed = true;
				}
				return outcome;
		}
	}

	private bool BuildingCommandPunishmentStrategyType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type do you want to change the punishment strategy to?\nValid types: {PunishmentStrategies.PunishmentStrategyFactory.ValidTypes.Select(x => x.ColourName()).ListToString()}");
			return false;
		}

		var strategy =
			PunishmentStrategies.PunishmentStrategyFactory.GetStrategyFromBuilderInput(Gameworld, Authority,
				command.SafeRemainingArgument);
		if (strategy is null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid punishment strategy type.\nValid types: {PunishmentStrategies.PunishmentStrategyFactory.ValidTypes.Select(x => x.ColourName()).ListToString()}");
			return false;
		}

		PunishmentStrategy = strategy;
		actor.OutputHandler.Send(
			$"When convicted of violating this law, the punishment strategy is now {PunishmentStrategy.Describe(actor).ColourIncludingReset(Telnet.Red)}.");
		Changed = true;
		return true;
	}

	private bool CalculateLawAppliesInvoker(IFutureProg prog)
	{
		if (prog.MatchesParameters(new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim) == true;
			return true;
		}

		if (prog.MatchesParameters(new[]
			    { ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim, item) == true;
			return true;
		}

		if (prog.MatchesParameters(new[]
		    {
			    ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Number
		    }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim, id) == true;
			return true;
		}

		if (prog.MatchesParameters(new[]
		    {
			    ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item,
			    ProgVariableTypes.Number
		    }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim, item, id) == true;
			return true;
		}

		if (prog.MatchesParameters(new[]
		    {
			    ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Number,
			    ProgVariableTypes.Text
		    }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim, id, crime) == true;
			return true;
		}

		if (prog.MatchesParameters(new[]
		    {
			    ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item,
			    ProgVariableTypes.Number, ProgVariableTypes.Text
		    }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim, item, id, crime) == true;
			return true;
		}

		if (prog.MatchesParameters(new[]
			    { ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Text }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim, crime) == true;
			return true;
		}

		if (prog.MatchesParameters(new[]
		    {
			    ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item,
			    ProgVariableTypes.Text
		    }))
		{
			LawAppliesInvoker = (criminal, victim, item, id, crime) =>
				LawAppliesProg.Execute<bool?>(criminal, victim, item, crime) == true;
			return true;
		}

		return false;
	}

	private bool BuildingCommandRepeats(ICharacter actor)
	{
		DoNotAutomaticallyApplyRepeats = !DoNotAutomaticallyApplyRepeats;
		Changed = true;
		actor.OutputHandler.Send(
			$"When being applied automatically, this law will {(DoNotAutomaticallyApplyRepeats ? "now" : "no longer")} ignore repeated violations against the same target in a short period.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a prog that you want to control whether or not this is a crime, or use clear to reset it to none.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "reset"))
		{
			LawAppliesProg = null;
			actor.OutputHandler.Send(
				$"The {Name.Colour(Telnet.Cyan)} law no longer uses a prog to determine applicability - instead it always applies.");
			Changed = true;
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no prog with that name or ID.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		var oldInvoker = LawAppliesInvoker;
		if (!CalculateLawAppliesInvoker(prog))
		{
			LawAppliesInvoker = oldInvoker;
			actor.OutputHandler.Send(
				"The prog you specify must take one of the following patterns of parameters:\n\tcharacter character\n\tcharacter character number\n\tcharacter character number text\n\tcharacter character text\n\tcharacter character item\n\tcharacter character item number\n\tcharacter character item number text\n\tcharacter character item text");
			return false;
		}

		LawAppliesProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine applicability.");
		return true;
	}

	private bool BuildingCommandVictim(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What legal class do you want to toggle as the victim for this law?");
			return false;
		}

		var legal = long.TryParse(command.PopSpeech(), out var value)
			? Authority.LegalClasses.FirstOrDefault(x => x.Id == value)
			: Authority.LegalClasses.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
			  Authority.LegalClasses.FirstOrDefault(x =>
				  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (legal == null)
		{
			actor.OutputHandler.Send("There is no such legal class.");
			return false;
		}

		if (_victimClasses.Contains(legal))
		{
			_victimClasses.Remove(legal);
			Changed = true;
			actor.OutputHandler.Send(
				$"The {Name.Colour(Telnet.Cyan)} law no longer counts the {legal.Name.Colour(Telnet.Cyan)} legal class as victims.");
			return true;
		}

		_victimClasses.Add(legal);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law now counts the {legal.Name.Colour(Telnet.Cyan)} legal class as victims.");
		return true;
	}

	private bool BuildingCommandOffender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What legal class do you want to toggle as the offender for this law?");
			return false;
		}

		var legal = long.TryParse(command.PopSpeech(), out var value)
			? Authority.LegalClasses.FirstOrDefault(x => x.Id == value)
			: Authority.LegalClasses.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
			  Authority.LegalClasses.FirstOrDefault(x =>
				  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (legal == null)
		{
			actor.OutputHandler.Send("There is no such legal class.");
			return false;
		}

		if (_offenderClasses.Contains(legal))
		{
			_offenderClasses.Remove(legal);
			Changed = true;
			actor.OutputHandler.Send(
				$"The {Name.Colour(Telnet.Cyan)} law no longer counts the {legal.Name.Colour(Telnet.Cyan)} legal class as offenders.");
			return true;
		}

		_offenderClasses.Add(legal);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law now counts the {legal.Name.Colour(Telnet.Cyan)} legal class as offenders.");
		return true;
	}

	private bool BuildingCommandStrategy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can specify the following enforcement strategies: {Enum.GetValues(typeof(EnforcementStrategy)).OfType<EnforcementStrategy>().Select(x => x.DescribeEnum().ColourValue()).ListToLines(true)}");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<EnforcementStrategy>(out var strategy))
		{
			actor.OutputHandler.Send(
				$"That is not a valid enforcement strategy. Valid enforcement strategies are as follows:{Enum.GetValues(typeof(EnforcementStrategy)).OfType<EnforcementStrategy>().Select(x => x.DescribeEnum().ColourValue()).ListToLines(true)}");
			return false;
		}

		EnforcementStrategy = strategy;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law will now use the {EnforcementStrategy.DescribeEnum(true).Colour(Telnet.Green)} enforcement strategy.");

		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What numerical priority should the enforcement of this law carry? Higher numbers mean higher priorities.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		EnforcementPriority = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law will now carry an enforcement priority of {EnforcementPriority.ToString("N0", actor).ColourValue()}, which is {(Authority.Laws.OrderByDescending(x => x.EnforcementPriority).ToList().IndexOf(this) + 1).ToOrdinal().ColourValue()} of {Authority.Laws.Count().ToString("N0", actor).ColourValue()} laws.");
		return true;
	}

	private bool BuildingCommandActivePeriod(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How long should the investigation of this time remain active, before it becomes a cold case?");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var ts))
		{
			actor.OutputHandler.Send(
				$"That is not a valid timespan. Note that in most cases the syntax will be hh:mm:ss e.g. 24:0:0 for 24 hours.");
			return false;
		}

		ActivePeriod = ts;
		Changed = true;
		actor.OutputHandler.Send(
			$"Violations of the {Name.Colour(Telnet.Cyan)} law will now be actively investigated for {ActivePeriod.Describe(actor).ColourValue()} after they are reported.");
		return true;
	}

	private bool BuildingCommandBail(ICharacter actor, StringStack command)
	{
		CanBeOfferedBail = !CanBeOfferedBail;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law will {(CanBeOfferedBail ? "now" : "no longer")} permit offenders to post bail.");
		return true;
	}

	private bool BuildingCommandArrest(ICharacter actor, StringStack command)
	{
		CanBeArrested = !CanBeArrested;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law will {(CanBeArrested ? "now" : "no longer")} be a reason to arrest people.");
		return true;
	}

	private bool BuildingCommandAutomatic(ICharacter actor, StringStack command)
	{
		CanBeAppliedAutomatically = !CanBeAppliedAutomatically;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} law will {(CanBeAppliedAutomatically ? "now" : "no longer")} be applied automatically by the engine.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to the law?");
			return false;
		}

		var newName = command.PopSpeech().TitleCase();
		if (Authority.Laws.Any(x => x.Name.EqualTo(newName)))
		{
			actor.OutputHandler.Send("There is already a law with that name. Law names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {Name.Colour(Telnet.Cyan)} law to {newName.Colour(Telnet.Cyan)}.");
		_name = newName;
		Changed = true;
		return true;
	}

	private bool BuildingCommandCrime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What crime type do you want to change that law to codify? Valid types are: {Enum.GetValues<CrimeTypes>().Select(x => x.DescribeEnum().ColourValue()).OrderBy(x => x).ListToCommaSeparatedValues()}");
			return false;
		}

		if (!Utilities.TryParseEnum<CrimeTypes>(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid crime type. Valid types are: {Enum.GetValues<CrimeTypes>().Select(x => x.DescribeEnum().ColourValue()).OrderBy(x => x).ListToCommaSeparatedValues()}");
			return false;
		}

		actor.OutputHandler.Send(
			$"You change the type of coded crime controlled by the {Name.Colour(Telnet.Cyan)} law from {CrimeType.DescribeEnum().ColourValue()} to {value.DescribeEnum().ColourValue()}.");
		CrimeType = value;
		Changed = true;
		return true;
	}

	private bool BuildingCommandShow(ICharacter actor)
	{
		actor.OutputHandler.Send(Show(actor));
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Law #{Id.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Crime: {CrimeType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Strategy: {EnforcementStrategy.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Arrestable: {CanBeArrested.ToColouredString()}");
		sb.AppendLine($"Bail: {CanBeOfferedBail.ToColouredString()}");
		sb.AppendLine($"Automatic: {CanBeAppliedAutomatically.ToColouredString()}");
		sb.AppendLine($"Don't Repeat: {DoNotAutomaticallyApplyRepeats.ToColouredString()}");
		sb.AppendLine($"Active Period: {ActivePeriod.Describe(actor).ColourValue()}");
		sb.AppendLine($"Victims: {VictimClasses.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");
		sb.AppendLine($"Offenders: {OffenderClasses.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");
		sb.AppendLine(
			$"Prog: {(LawAppliesProg == null ? "None".Colour(Telnet.Red) : LawAppliesProg.MXPClickableFunctionNameWithId())}");
		sb.AppendLine($"Punishment Strategy: {PunishmentStrategy.Describe(actor).ColourIncludingReset(Telnet.Red)}");
		return sb.ToString();
	}

	public bool IsCrime(ICharacter criminal, ICharacter victim, IGameItem item)
	{
		var crimLegal = Authority.GetLegalClass(criminal);
		if (crimLegal == null)
		{
			return false;
		}

		if (!OffenderClasses.Contains(crimLegal))
		{
			return false;
		}

		if (victim is not null)
		{
			var victimLegal = Authority.GetLegalClass(victim);
			if (victimLegal == null)
			{
				return false;
			}

			if (!VictimClasses.Contains(victimLegal))
			{
				return false;
			}
		}

		if (LawAppliesProg != null &&
		    !LawAppliesInvoker(criminal, victim, item, Id, CrimeType.DescribeEnum()))
		{
			return false;
		}

		return true;
	}

	#region FutureProg Implementation

	public ProgVariableTypes Type => ProgVariableTypes.Law;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "authority":
				return Authority;
			case "crimename":
				return new TextVariable(CrimeType.DescribeEnum(true));
			case "crimetype":
				return new NumberVariable((int)CrimeType);
			case "isviolentcrime":
				return new BooleanVariable(CrimeType.IsViolentCrime());
			case "ismoralcrime":
				return new BooleanVariable(CrimeType.IsMoralCrime());
			case "ismajorcrime":
				return new BooleanVariable(CrimeType.IsMajorCrime());
			case "isarrestable":
				return new BooleanVariable(EnforcementStrategy.IsArrestable());
			case "iskillable":
				return new BooleanVariable(EnforcementStrategy.IsKillable());
			default:
				throw new ApplicationException($"Invalid property {property} requested in Law.GetProperty");
		}
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "authority", ProgVariableTypes.LegalAuthority },
			{ "crimename", ProgVariableTypes.Text },
			{ "crimetype", ProgVariableTypes.Number },
			{ "isviolentcrime", ProgVariableTypes.Boolean },
			{ "ismoralcrime", ProgVariableTypes.Boolean },
			{ "ismajorcrime", ProgVariableTypes.Boolean },
			{ "isarrestable", ProgVariableTypes.Boolean },
			{ "iskillable", ProgVariableTypes.Boolean }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the law" },
			{ "name", "The name of the law" },
			{ "authority", "The legal authority that this law belongs to" },
			{ "crimename", "The name of the crime that this law governs" },
			{ "crimetype", "The numerical ID of the crime that this law governs" },
			{ "isviolentcrime", "Whether or not this crime type is considered a violent crime" },
			{ "ismoralcrime", "Whether or not this crime type is considered a moral crime" },
			{ "ismajorcrime", "Whether or not this crime type is considered a major crime" },
			{ "isarrestable", "Whether or not this crime can lead to the individual being arrested" },
			{ "iskillable", "Whether or not this crime attracts a lethal force response" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Law, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}