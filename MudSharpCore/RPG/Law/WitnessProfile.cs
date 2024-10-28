using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;

namespace MudSharp.RPG.Law;

public class WitnessProfile : SaveableItem, IWitnessProfile
{
	public WitnessProfile(MudSharp.Models.WitnessProfile profile, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = profile.Id;
		_name = profile.Name;
		ReportingMultiplierProg = Gameworld.FutureProgs.Get(profile.ReportingMultiplierProgId);
		IdentityKnownProg = Gameworld.FutureProgs.Get(profile.IdentityKnownProgId);
		BaseReportingChance[TimeOfDay.Afternoon] = profile.BaseReportingChanceAfternoon;
		BaseReportingChance[TimeOfDay.Morning] = profile.BaseReportingChanceMorning;
		BaseReportingChance[TimeOfDay.Night] = profile.BaseReportingChanceNight;
		BaseReportingChance[TimeOfDay.Dawn] = profile.BaseReportingChanceDawn;
		BaseReportingChance[TimeOfDay.Dusk] = profile.BaseReportingChanceDusk;
		foreach (var legal in profile.WitnessProfilesIgnoredCriminalClasses)
		{
			IgnoredCriminalLegalClasses.Add(Gameworld.LegalAuthorities.Get(legal.LegalClass.LegalAuthorityId)
			                                         .LegalClasses.First(x => x.Id == legal.LegalClassId));
		}

		foreach (var legal in profile.WitnessProfilesIgnoredVictimClasses)
		{
			IgnoredVictimLegalClasses.Add(Gameworld.LegalAuthorities.Get(legal.LegalClass.LegalAuthorityId).LegalClasses
			                                       .First(x => x.Id == legal.LegalClassId));
		}

		foreach (var authority in profile.WitnessProfilesCooperatingAuthorities)
		{
			CooperatingAuthorities.Add(Gameworld.LegalAuthorities.Get(authority.LegalAuthorityId));
		}

		MinimumSkillToDetermineBiases = profile.MinimumSkillToDetermineBiases;
		MinimumSkillToDetermineTimeOfDay = profile.MinimumSkillToDetermineTimeOfDay;
		ReportingReliability = profile.ReportingReliability;
	}

	public WitnessProfile(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var profile = new Models.WitnessProfile();
			FMDB.Context.WitnessProfiles.Add(profile);
			profile.Name = name;
			profile.MinimumSkillToDetermineBiases = 30;
			profile.MinimumSkillToDetermineTimeOfDay = 50;
			profile.ReportingMultiplierProgId = Gameworld.GetStaticLong("AlwaysOneProg");
			profile.IdentityKnownProgId = Gameworld.GetStaticLong("AlwaysTrueProg");
			profile.ReportingReliability = 1.0;
			profile.BaseReportingChanceAfternoon = 1.0;
			profile.BaseReportingChanceMorning = 1.0;
			profile.BaseReportingChanceNight = 1.0;
			profile.BaseReportingChanceDusk = 1.0;
			profile.BaseReportingChanceDawn = 1.0;
			FMDB.Context.SaveChanges();
			_id = profile.Id;
			_name = profile.Name;
			ReportingMultiplierProg = Gameworld.FutureProgs.Get(profile.ReportingMultiplierProgId);
			IdentityKnownProg = Gameworld.FutureProgs.Get(profile.IdentityKnownProgId);
			BaseReportingChance[TimeOfDay.Afternoon] = profile.BaseReportingChanceAfternoon;
			BaseReportingChance[TimeOfDay.Morning] = profile.BaseReportingChanceMorning;
			BaseReportingChance[TimeOfDay.Night] = profile.BaseReportingChanceNight;
			BaseReportingChance[TimeOfDay.Dawn] = profile.BaseReportingChanceDawn;
			BaseReportingChance[TimeOfDay.Dusk] = profile.BaseReportingChanceDusk;
			MinimumSkillToDetermineBiases = profile.MinimumSkillToDetermineBiases;
			MinimumSkillToDetermineTimeOfDay = profile.MinimumSkillToDetermineTimeOfDay;
			ReportingReliability = profile.ReportingReliability;
		}
	}

	public sealed override string FrameworkItemType => "WitnessProfile";

	private static TraitExpression _streetwiseSkillExpression;

	public static TraitExpression StreetwiseSkillExpression
	{
		get
		{
			if (_streetwiseSkillExpression == null)
			{
				_streetwiseSkillExpression = new TraitExpression(
					Futuremud.Games.First().GetStaticConfiguration("StreetwiseSkillExpression"),
					Futuremud.Games.First());
			}

			return _streetwiseSkillExpression;
		}
	}

	public IFutureProg ReportingMultiplierProg { get; protected set; }
	public Dictionary<TimeOfDay, double> BaseReportingChance { get; } = new();
	public List<ILegalClass> IgnoredVictimLegalClasses { get; } = new();
	public List<ILegalClass> IgnoredCriminalLegalClasses { get; } = new();
	public List<ILegalAuthority> CooperatingAuthorities { get; } = new();
	public IFutureProg IdentityKnownProg { get; protected set; }
	public double ReportingReliability { get; protected set; }
	public double MinimumSkillToDetermineTimeOfDay { get; protected set; }
	public double MinimumSkillToDetermineBiases { get; protected set; }

	public void WitnessCrime(ICrime crime)
	{
		if (!CooperatingAuthorities.Contains(crime.LegalAuthority))
		{
			return;
		}

		if (IgnoredCriminalLegalClasses.Contains(crime.LegalAuthority.GetLegalClass(crime.Criminal)))
		{
			return;
		}

		if (crime.VictimId.HasValue &&
		    IgnoredVictimLegalClasses.Contains(crime.LegalAuthority.GetLegalClass(crime.Victim)))
		{
			return;
		}

		var baseChance = BaseReportingChance[crime.CrimeLocation.CurrentTimeOfDay];
		baseChance *= ReportingMultiplierProg?.ExecuteDouble(crime.Criminal, crime.Victim, crime) ?? 1.0;
		if (Constants.Random.NextDouble() >= baseChance)
		{
			return;
		}

		crime.LegalAuthority.ReportCrime(crime, null, IdentityKnownProg?.Execute<bool?>(crime.Criminal) == true,
			ReportingReliability);
	}

	public string StreetwiseText(ICharacter enquirer)
	{
		var streetwise = StreetwiseSkillExpression.Evaluate(enquirer);
		if (!enquirer.IsAdministrator() && streetwise < MinimumSkillToDetermineTimeOfDay &&
		    streetwise < MinimumSkillToDetermineBiases)
		{
			return "You can't puzzle out anything about this area's willingness to cooperate with law enforcement.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"You can tell the following things about this area:");
		if (streetwise >= MinimumSkillToDetermineBiases || enquirer.IsAdministrator())
		{
			sb.AppendLine(
				$"\t* The people of this area will cooperate with {CooperatingAuthorities.Select(x => x.Name.ColourValue()).ListToString()}");
			if (IgnoredCriminalLegalClasses.Any())
			{
				sb.AppendLine(
					$"\t* The people of this area will ignore crimes committed by {IgnoredCriminalLegalClasses.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ")} classes.");
			}
			else
			{
				sb.AppendLine(
					$"\t* The people of this area will report crimes regardless of the legal class of the criminal.");
			}

			if (IgnoredVictimLegalClasses.Any())
			{
				sb.AppendLine(
					$"\t* The people of this area will ignore crimes whose victims are {IgnoredVictimLegalClasses.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ")} classes.");
			}
			else
			{
				sb.AppendLine(
					$"\t* The people of this area will report crimes regardless of the legal class of the victim.");
			}

			sb.AppendLine(
				$"\t* You think it is {ReportingReliability.DescribeAsProbability().ColourValue()} that perpetrator details would be reported accurately.");
		}

		if (streetwise >= MinimumSkillToDetermineTimeOfDay || enquirer.IsAdministrator())
		{
			var leastTime = BaseReportingChance.FirstMin(x => x.Value).Key;
			var mostTime = BaseReportingChance.FirstMax(x => x.Value).Key;
			var average = BaseReportingChance.Average(x => x.Value);

			sb.AppendLine(
				$"\t* On average, you think it is {average.DescribeAsProbability()} that crimes would be reported.");
			sb.AppendLine(
				$"\t* The time of day with the least scrutiny is {leastTime.DescribeEnum().Colour(Telnet.Magenta)}.");
			sb.AppendLine(
				$"\t* The time of day with the most scrutiny is {mostTime.DescribeEnum().Colour(Telnet.Magenta)}.");
		}

		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "reporting":
			case "reportingprog":
			case "reportprog":
			case "report":
			case "report_prog":
			case "reporting_prog":
			case "reporting prog":
			case "report prog":
				return BuildingCommandReportingMultiplierProg(actor, command);
			case "reliability":
				return BuildingCommandReliability(actor, command);
			case "identity":
			case "identityprog":
			case "identity_prog":
			case "identity prog":
			case "idprog":
			case "id_prog":
			case "id prog":
				return BuildingCommandIdentityProg(actor, command);
			case "chance":
				return BuildingCommandChance(actor, command);
			case "cooperate":
			case "cooperating":
			case "authority":
			case "authorities":
				return BuildingCommandCooperate(actor, command);
			case "victim":
				return BuildingCommandVictim(actor, command);
			case "criminal":
				return BuildingCommandCriminal(actor, command);
			case "skill":
				return BuildingCommandSkill(actor, command);
		}

		actor.OutputHandler.Send(
			$"You can use the following options with this command:\n\tname <name> - renames the profile\n\tchance <time> <percent> - the base reporting chance for a time of day\n\treliability <percent> - the reliability in describing criminals\n\tauthority <which> - toggles cooperating with a legal authority\n\tvictim <which> - toggles ignoring crimes committed against a victim of this legal class\n\tcriminal <which> - toggles ignoring crimes committed by this legal class\n\tidentity <prog> - specifies a prog which determines whether the criminal's identity is known\n\treport <prog> - specifies a prog that multiplies the chance of a crime being reported\n\tskill <bias> <levels> - specifies the level of skill required to determine reporting biases and reporting levels by time respectively");
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a new name for this witness profile.");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (Gameworld.WitnessProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a witness profile with that name, and witness profiles must have unique names.");
			return false;
		}

		_name = name.TitleCase().Trim();
		Changed = true;
		actor.OutputHandler.Send($"You rename this witness profile to {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must enter a minimum skill level to determine biases, and a minimum skill level to estimate activity levels.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var bias))
		{
			actor.OutputHandler.Send("You must enter a valid number for the minimum skill level to determine biases.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a minimum skill level to estimate activity levels.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var levels))
		{
			actor.OutputHandler.Send(
				"You must enter a valid number for the minimum skill level required to estimate activity levels.");
			return false;
		}

		MinimumSkillToDetermineBiases = bias;
		MinimumSkillToDetermineTimeOfDay = levels;
		Changed = true;
		actor.OutputHandler.Send(
			$"This witness profile will now require a skill level of {bias.ToString("N2", actor)} to determine biases, and {levels.ToString("N2", actor)} to determine activity levels.");
		return true;
	}

	private bool BuildingCommandVictim(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which legal class of victims would you like to toggle ignoring crimes against?");
			return false;
		}

		var victimClass = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.LegalClasses.Get(value)
			: Gameworld.LegalClasses.GetByName(command.Last);
		if (victimClass == null)
		{
			actor.OutputHandler.Send("There is no such legal class");
			return false;
		}

		if (IgnoredVictimLegalClasses.Contains(victimClass))
		{
			IgnoredVictimLegalClasses.Remove(victimClass);
			actor.OutputHandler.Send(
				$"This witness profile will no longer ignore crimes committed against people of the {victimClass.Name.ColourName()} legal class.");
		}
		else
		{
			IgnoredVictimLegalClasses.Add(victimClass);
			actor.OutputHandler.Send(
				$"This witness profile will now ignore crimes committed against people of the {victimClass.Name.ColourName()} legal class.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandCriminal(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which legal class of criminals would you like to toggle ignoring crimes comitted by?");
			return false;
		}

		var criminalClass = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.LegalClasses.Get(value)
			: Gameworld.LegalClasses.GetByName(command.Last);
		if (criminalClass == null)
		{
			actor.OutputHandler.Send("There is no such legal class");
			return false;
		}

		if (IgnoredCriminalLegalClasses.Contains(criminalClass))
		{
			IgnoredCriminalLegalClasses.Remove(criminalClass);
			actor.OutputHandler.Send(
				$"This witness profile will no longer ignore crimes committed by people of the {criminalClass.Name.ColourName()} legal class.");
		}
		else
		{
			IgnoredCriminalLegalClasses.Add(criminalClass);
			actor.OutputHandler.Send(
				$"This witness profile will now ignore crimes committed by people of the {criminalClass.Name.ColourName()} legal class.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandCooperate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which legal authority did you want to toggle this witness profile cooperating with?");
			return false;
		}

		var authority = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.LegalAuthorities.Get(value)
			: Gameworld.LegalAuthorities.GetByName(command.Last);
		if (authority == null)
		{
			actor.OutputHandler.Send("There is no such legal authority.");
			return false;
		}

		if (CooperatingAuthorities.Contains(authority))
		{
			CooperatingAuthorities.Remove(authority);
			actor.OutputHandler.Send(
				$"This witness profile will no longer cooperate with the {authority.Name.ColourName()} legal authority.");
		}
		else
		{
			CooperatingAuthorities.Add(authority);
			actor.OutputHandler.Send(
				$"This witness profile will now cooperate with the {authority.Name.ColourName()} legal authority.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What time of day do you want to set the base reporting chance for? Valid options are {Enum.GetValues(typeof(TimeOfDay)).OfType<TimeOfDay>().Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<TimeOfDay>(out var time))
		{
			actor.OutputHandler.Send(
				$"That is not a valid time of day. Valid options are {Enum.GetValues(typeof(TimeOfDay)).OfType<TimeOfDay>().Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the base chance that a crime is reported when witnessed by this witness profile?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("That is not a valid percentage chance.");
			return false;
		}

		BaseReportingChance[time] = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The base reporting chance for crimes committed during the {time.DescribeColour()} is now {value.ToString("P2", actor).ColourValue()}");
		return true;
	}

	private bool BuildingCommandIdentityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to set as the one used to determine whether the reporters know the identity of the criminal?");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				"The specified prog does not return a boolean value, as required by this command.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				"The specified prog does not accept just a single character as an argument, as required by this command.");
			return false;
		}

		IdentityKnownProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This witness profile will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine whether they know the identity of the criminal.");
		return true;
	}

	private bool BuildingCommandReliability(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What chance do you want to set for this witness profile to get the criminal's characteristics correct?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage value.");
			return false;
		}

		ReportingReliability = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This witness profile is now {value.ToString("P2", actor).ColourValue()} reliable at reporting the characteristics of criminals.");
		return true;
	}

	private bool BuildingCommandReportingMultiplierProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to set as the one used to determine a multiplier for reporting crimes?");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Number))
		{
			actor.OutputHandler.Send(
				"The specified prog does not return a number value, as required by this command.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				"The specified prog does not accept just a single character as an argument, as required by this command.");
			return false;
		}

		ReportingMultiplierProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This witness profile will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine a multiplier to their reporting chance.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Witness Profile #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Cooperates With: {CooperatingAuthorities.Select(x => x.Name.ColourValue()).ListToString()}");
		sb.AppendLine($"Ignored Victims: {IgnoredVictimLegalClasses.Select(x => x.Name.ColourValue()).ListToString()}");
		sb.AppendLine(
			$"Ignored Criminals: {IgnoredCriminalLegalClasses.Select(x => x.Name.ColourValue()).ListToString()}");
		sb.AppendLine($"Reporting Base Chances:");
		foreach (var value in BaseReportingChance)
		{
			sb.AppendLine($"\t{value.Key.DescribeColour()}: {value.Value.ToString("P2", actor).ColourValue()}");
		}

		sb.AppendLine($"Description Reliability: {ReportingReliability.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Known Identity Prog: {IdentityKnownProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine($"Reporting Multiplier Prog: {ReportingMultiplierProg.MXPClickableFunctionNameWithId()}");
		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.WitnessProfiles.Find(Id);
		dbitem.Name = Name;
		dbitem.ReportingReliability = ReportingReliability;
		dbitem.BaseReportingChanceAfternoon = BaseReportingChance[TimeOfDay.Afternoon];
		dbitem.BaseReportingChanceDawn = BaseReportingChance[TimeOfDay.Dawn];
		dbitem.BaseReportingChanceDusk = BaseReportingChance[TimeOfDay.Dusk];
		dbitem.BaseReportingChanceNight = BaseReportingChance[TimeOfDay.Night];
		dbitem.BaseReportingChanceMorning = BaseReportingChance[TimeOfDay.Morning];
		dbitem.IdentityKnownProgId = IdentityKnownProg.Id;
		dbitem.MinimumSkillToDetermineBiases = MinimumSkillToDetermineBiases;
		dbitem.MinimumSkillToDetermineTimeOfDay = MinimumSkillToDetermineTimeOfDay;
		dbitem.ReportingMultiplierProgId = ReportingMultiplierProg.Id;
		FMDB.Context.WitnessProfilesCooperatingAuthorities.RemoveRange(dbitem.WitnessProfilesCooperatingAuthorities);
		foreach (var item in CooperatingAuthorities)
		{
			dbitem.WitnessProfilesCooperatingAuthorities.Add(new Models.WitnessProfilesCooperatingAuthorities
				{ WitnessProfile = dbitem, LegalAuthorityId = item.Id });
		}

		FMDB.Context.WitnessProfilesIgnoredCriminalClasses.RemoveRange(dbitem.WitnessProfilesIgnoredCriminalClasses);
		foreach (var item in IgnoredCriminalLegalClasses)
		{
			dbitem.WitnessProfilesIgnoredCriminalClasses.Add(new Models.WitnessProfilesIgnoredCriminalClasses
				{ WitnessProfile = dbitem, LegalClassId = item.Id });
		}

		FMDB.Context.WitnessProfilesIgnoredVictimClasses.RemoveRange(dbitem.WitnessProfilesIgnoredVictimClasses);
		foreach (var item in IgnoredVictimLegalClasses)
		{
			dbitem.WitnessProfilesIgnoredVictimClasses.Add(new Models.WitnessProfilesIgnoredVictimClasses
				{ WitnessProfile = dbitem, LegalClassId = item.Id });
		}

		Changed = false;
	}
}