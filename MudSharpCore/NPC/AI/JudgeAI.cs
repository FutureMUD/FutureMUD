using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MimeKit.IO.Filters;
using MoreLinq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.NPC.AI;
public class JudgeAI : EnforcerAI
{
	public static void RegisterLoader()
	{
		RegisterAIType("Judge", (ai, gameworld) => new JudgeAI(ai, gameworld));
		RegisterAIBuilderInformation("judge", (gameworld, name) => new JudgeAI(gameworld, name), new JudgeAI().HelpText);
	}

	protected JudgeAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
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

		IntroductionDelay = TimeSpan.FromSeconds(double.Parse(root.Element("IntroductionDelay").Value));
		ChargesDelay = TimeSpan.FromSeconds(double.Parse(root.Element("ChargesDelay").Value));
		PleaDelay = TimeSpan.FromSeconds(double.Parse(root.Element("PleaDelay").Value));
		CaseDelayPerCrime = TimeSpan.FromSeconds(double.Parse(root.Element("CaseDelayPerCrime").Value));
		ClosingArgumentDelay = TimeSpan.FromSeconds(double.Parse(root.Element("ClosingArgumentDelay").Value));
		VerdictDelay = TimeSpan.FromSeconds(double.Parse(root.Element("VerdictDelay").Value));
		SentencingDelay = TimeSpan.FromSeconds(double.Parse(root.Element("SentencingDelay").Value));

		TrialIntroductionEmote = root.Element("TrialIntroductionEmote").Value;
		TrialChargesEmote = root.Element("TrialChargesEmote").Value;
		TrialPleaEmote = root.Element("TrialPleaEmote").Value;
		TrialDefaultPleaEnteredEmote = root.Element("TrialDefaultPleaEnteredEmote").Value;
		TrialCaseEmote = root.Element("TrialCaseEmote").Value;
		TrialClosingArgumentsEmote = root.Element("TrialClosingArgumentsEmote").Value;
		TrialEndArgumentsEmote = root.Element("TrialEndArgumentsEmote").Value;
		TrialVerdictGuiltyEmote = root.Element("TrialVerdictGuiltyEmote").Value;
		TrialVerdictNotGuiltyEmote = root.Element("TrialVerdictNotGuiltyEmote").Value;
		TrialSentencingEmote = root.Element("TrialSentencingEmote").Value;
		TrialEndFreeToGo = root.Element("TrialEndFreeToGo").Value;
		TrialEndRemandedIntoCustody = root.Element("TrialEndRemandedIntoCustody").Value;
		TrialEndRemandedAwaitingExecution = root.Element("TrialEndRemandedAwaitingExecution").Value;
	}

	protected JudgeAI()
	{

	}

	protected JudgeAI(IFuturemud gameworld, string name) : base(gameworld, name, "Judge")
	{
		IntroductionDelay = TimeSpan.FromSeconds(15);
		ChargesDelay = TimeSpan.FromSeconds(15);
		PleaDelay = TimeSpan.FromSeconds(30);
		CaseDelayPerCrime = TimeSpan.FromSeconds(30);
		ClosingArgumentDelay = TimeSpan.FromSeconds(30);
		VerdictDelay = TimeSpan.FromSeconds(15);
		SentencingDelay = TimeSpan.FromSeconds(15);

		TrialIntroductionEmote = @"@ tell|tells $1, ""{4}, you stand accused of {6} {7}, being {5}. In this trial we will determine your guilt or innocence.""";
		TrialChargesEmote = @"@ tell|tells $1, ""I will now proceed to read out the charges in order, and after each you can enter a plea of guilty or innocent.""";
		TrialPleaEmote = @"@ ask|asks $1, ""The {10} charge is that on {8} you {9}. How do you plead?""";
		TrialDefaultPleaEnteredEmote = @"@ declare|declares, ""By {2} silence, the defendant has entered a plea of guilty to the {10} charge.""";
		TrialCaseEmote = @"@ say|says, ""I will now hear the cases of the prosecution and defense.""";
		TrialClosingArgumentsEmote = @"@ say|says, ""Both parties will now give their closing arguments.""";
		TrialEndArgumentsEmote = @"@ say|says, ""I have heard enough. We are now ready to move on to the verdict. I will read the verdict for each crime in turn.""";
		TrialVerdictGuiltyEmote = @"@ tell|tells $1, ""On the matter of the {10} charge, that on {8} you {9}, I judge you to be guilty.""";
		TrialVerdictNotGuiltyEmote = @"@ tell|tells $1, ""On the matter of the {10} charge, that on {8} you {9}, I judge you to be not guilty.""";
		TrialSentencingEmote = @"@ tell|tells $1, ""For the {10} crime, I sentence you to {11}.""";
		TrialEndFreeToGo = @"@ tell|tells $1, ""That concludes the trial. You are free to leave the court.""";
		TrialEndRemandedIntoCustody = @"@ tell|tells $1, ""You will now begin your custodial sentence. Please remand the prisoner into custody.""";
		TrialEndRemandedAwaitingExecution = @"@ tell|tells $1, ""You will now be returned to custody until the time of your execution.""";
		DatabaseInitialise();
	}

	/// <inheritdoc />
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("IdentityProg", IdentityIsKnownProg?.Id ?? 0L),
			new XElement("WarnEchoProg", WarnEchoProg?.Id ?? 0L),
			new XElement("WarnStartMoveEchoProg", WarnStartMoveEchoProg?.Id ?? 0L),
			new XElement("FailToComplyEchoProg", FailToComplyEchoProg?.Id ?? 0L),
			new XElement("ThrowInPrisonEchoProg", ThrowInPrisonEchoProg?.Id ?? 0L),
			new XElement("IntroductionDelay", IntroductionDelay.TotalSeconds),
			new XElement("ChargesDelay", ChargesDelay.TotalSeconds),
			new XElement("PleaDelay", PleaDelay.TotalSeconds),
			new XElement("CaseDelayPerCrime", CaseDelayPerCrime.TotalSeconds),
			new XElement("ClosingArgumentDelay", ClosingArgumentDelay.TotalSeconds),
			new XElement("VerdictDelay", VerdictDelay.TotalSeconds),
			new XElement("SentencingDelay", SentencingDelay.TotalSeconds),
			new XElement("TrialIntroductionEmote", new XCData(TrialIntroductionEmote)),
			new XElement("TrialChargesEmote", new XCData(TrialChargesEmote)),
			new XElement("TrialPleaEmote", new XCData(TrialPleaEmote)),
			new XElement("TrialDefaultPleaEnteredEmote", new XCData(TrialDefaultPleaEnteredEmote)),
			new XElement("TrialCaseEmote", new XCData(TrialCaseEmote)),
			new XElement("TrialClosingArgumentsEmote", new XCData(TrialClosingArgumentsEmote)),
			new XElement("TrialEndArgumentsEmote", new XCData(TrialEndArgumentsEmote)),
			new XElement("TrialVerdictGuiltyEmote", new XCData(TrialVerdictGuiltyEmote)),
			new XElement("TrialVerdictNotGuiltyEmote", new XCData(TrialVerdictNotGuiltyEmote)),
			new XElement("TrialSentencingEmote", new XCData(TrialSentencingEmote)),
			new XElement("TrialEndFreeToGo", new XCData(TrialEndFreeToGo)),
			new XElement("TrialEndRemandedIntoCustody", new XCData(TrialEndRemandedIntoCustody)),
			new XElement("TrialEndRemandedAwaitingExecution", new XCData(TrialEndRemandedAwaitingExecution))
		).ToString();
	}

	public TimeSpan IntroductionDelay { get; protected set; }
	public TimeSpan ChargesDelay { get; protected set; }
	public TimeSpan PleaDelay { get; protected set; }
	public TimeSpan CaseDelayPerCrime { get; protected set; }
	public TimeSpan ClosingArgumentDelay { get; protected set; }
	public TimeSpan VerdictDelay { get; protected set; }
	public TimeSpan SentencingDelay { get; protected set; }

	public string TrialIntroductionEmote { get; protected set; }
	public string TrialChargesEmote { get; protected set; }
	public string TrialPleaEmote { get; protected set; }
	public string TrialDefaultPleaEnteredEmote { get; protected set; }
	public string TrialCaseEmote { get; protected set; }
	public string TrialClosingArgumentsEmote { get; protected set; }
	public string TrialEndArgumentsEmote { get; protected set; }
	public string TrialVerdictGuiltyEmote { get; protected set; }
	public string TrialVerdictNotGuiltyEmote { get; protected set; }
	public string TrialSentencingEmote { get; protected set; }
	public string TrialEndFreeToGo { get; protected set; }
	public string TrialEndRemandedIntoCustody { get; protected set; }
	public string TrialEndRemandedAwaitingExecution { get; protected set; }

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(base.Show(actor));
		sb.AppendLine();
		sb.AppendLine("Delays Between Actions".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Introduction: {IntroductionDelay.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine($"Charges: {ChargesDelay.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine($"Plea: {PleaDelay.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine($"Case (Per Crime): {CaseDelayPerCrime.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine($"Closing Argument: {ClosingArgumentDelay.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine($"Verdict: {VerdictDelay.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine($"Sentencing: {SentencingDelay.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Emotes".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Introduction: {TrialIntroductionEmote.ColourCommand()}");
		sb.AppendLine($"Charges: {TrialChargesEmote.ColourCommand()}");
		sb.AppendLine($"Plea: {TrialPleaEmote.ColourCommand()}");
		sb.AppendLine($"Default Plea: {TrialDefaultPleaEnteredEmote.ColourCommand()}");
		sb.AppendLine($"Case: {TrialCaseEmote.ColourCommand()}");
		sb.AppendLine($"Closing Arguments: {TrialClosingArgumentsEmote.ColourCommand()}");
		sb.AppendLine($"End of Arguments: {TrialEndArgumentsEmote.ColourCommand()}");
		sb.AppendLine($"Guilty Verdict: {TrialVerdictGuiltyEmote.ColourCommand()}");
		sb.AppendLine($"Not Guilty Verdict: {TrialVerdictNotGuiltyEmote.ColourCommand()}");
		sb.AppendLine($"Sentencing: {TrialSentencingEmote.ColourCommand()}"); 
		sb.AppendLine($"End Free To Go: {TrialEndFreeToGo.ColourCommand()}");
		sb.AppendLine($"End Jail Time: {TrialEndRemandedIntoCustody.ColourCommand()}");
		sb.AppendLine($"End Execution: {TrialEndRemandedAwaitingExecution.ColourCommand()}");
		return sb.ToString();
		
	}

	/// <inheritdoc />
	protected override bool CharacterFiveSecondTick(ICharacter enforcer)
	{
		base.CharacterFiveSecondTick(enforcer);
		var enforcerEffect = EnforcerEffect(enforcer);
		if (enforcerEffect is null)
		{
			return false;
		}

		if (enforcer.Location != enforcerEffect.LegalAuthority.CourtLocation)
		{
			return false;
		}

		var defendant =
			enforcerEffect.LegalAuthority.CourtLocation.Characters.FirstOrDefault(x =>
				x.EffectsOfType<OnTrial>(y => y.LegalAuthority == enforcerEffect.LegalAuthority).Any());
		if (defendant is null)
		{
			return false;
		}
		var trialEffect = defendant.EffectsOfType<OnTrial>(x => x.LegalAuthority == enforcerEffect.LegalAuthority).First();
		return DoTrialTick(enforcer, defendant, trialEffect);
	}

	private bool DoTrialTick(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect)
	{
		var gender = defendant.ApparentGender(enforcer);
		var crimeNames = trialEffect.Crimes.Select(x => x.Law.CrimeType.DescribeEnum(true)).Distinct().ToArray();
		switch (trialEffect.Phase)
		{
			case TrialPhase.AwaitingLawyers:
				return DoTrialTickAwaitingLawyers(enforcer, defendant, trialEffect, gender, crimeNames);
			case TrialPhase.Introduction:
				return DoTrialTickIntroduction(enforcer, defendant, trialEffect, gender, crimeNames);
			case TrialPhase.Charges:
				return DoTrialTickCharges(enforcer, defendant, trialEffect, gender, crimeNames);
			case TrialPhase.Plea:
				return DoTrialTickPlea(enforcer, defendant, trialEffect, gender, crimeNames);
			case TrialPhase.Case:
				return DoTrialTickCase(enforcer, defendant, trialEffect, gender, crimeNames);
			case TrialPhase.ClosingArguments:
				return DoTrialTickClosingArguments(enforcer, defendant, trialEffect, gender, crimeNames);
			case TrialPhase.Verdict:
				return DoTrialTickVerdict(enforcer, defendant, trialEffect, gender, crimeNames);
			case TrialPhase.Sentencing:
				return DoTrialTickSentencing(enforcer, defendant, trialEffect, gender, crimeNames);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private bool DoTrialTickAwaitingLawyers(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		if (DateTime.UtcNow - trialEffect.LastTrialAction < TrialPhaseDelay(trialEffect.Phase, trialEffect.Crimes.Count()))
		{
			return false;
		}

		var court = trialEffect.LegalAuthority.CourtLocation;
		trialEffect.Prosecutor ??= enforcer;

		if (trialEffect.Defender is null)
		{
			var counselEffect = defendant.EffectsOfType<HasLegalCounsel>().FirstOrDefault();
			if (counselEffect is not null)
			{
				if (counselEffect.Lawyer is null)
				{
					defendant.RemoveEffect(counselEffect);
				}
				else
				{
					trialEffect.Defender = counselEffect.Lawyer;
					defendant.OutputHandler.Send($"#2[System Message]#0 A trial for your client #6{defendant.PersonalName.GetName(NameStyle.FullName)}#0 has started at #2{court.Name}#0.".SubstituteANSIColour());
				}
			}
			else
			{
				var courtLawyers = Gameworld.Characters
				                            .OfType<INPC>()
				                            .Select(x => (NPC: x, AI: x.AIs.OfType<LawyerAI>().FirstOrDefault()))
				                            .Where(x => x.AI?.AvailableToHire(x.NPC, trialEffect.LegalAuthority, true) == true)
				                            .ToList();
				if (courtLawyers.Count == 0)
				{
					trialEffect.Defender = defendant;
					defendant.OutputHandler.Send("#2[System Message]#0 You are defending yourself in this case.".SubstituteANSIColour());
				}
				else
				{
					var lawyer = courtLawyers.GetRandomElement();
					trialEffect.Defender = lawyer.NPC;
					lawyer.NPC.AddEffect(new Lawyering(lawyer.NPC, trialEffect.LegalAuthority));
					defendant.OutputHandler.Send("#2[System Message]#0 You have a court-appointed lawyer, who is on their way.".SubstituteANSIColour());
				}
			}
		}

		if (trialEffect.Defender.Location == court && trialEffect.Prosecutor.Location == court)
		{
			trialEffect.Phase = TrialPhase.Introduction;
			trialEffect.LastTrialAction = DateTime.UtcNow;
			return true;
		}

		// After 10 minutes, fire the lawyers that aren't there
		if (DateTime.UtcNow - trialEffect.LastTrialAction > TimeSpan.FromMinutes(10))
		{
			if (trialEffect.Defender.Location != court)
			{
				trialEffect.Defender.RemoveAllEffects(x => x.IsEffectType<Lawyering>());
				trialEffect.Defender = null;
				defendant.RemoveAllEffects(x => x.IsEffectType<HasLegalCounsel>());
			}

			if (trialEffect.Prosecutor.Location != court)
			{
				trialEffect.Prosecutor = null;
			}
		}

		return false;
	}

	private bool DoTrialTickSentencing(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		if (DateTime.UtcNow - trialEffect.LastTrialAction < TrialPhaseDelay(trialEffect.Phase, trialEffect.Crimes.Count()))
		{
			return false;
		}

		var sentenceCrime = trialEffect.NextCrime();
		while (true)
		{
			if (sentenceCrime is null)
			{
				break;
			}

			if (!trialEffect.Punishments.ContainsKey(sentenceCrime))
			{
				sentenceCrime = trialEffect.NextCrime();
				continue;
			}

			break;
		}

		if (sentenceCrime is null)
		{
			// Process all of the crimes and punishments
			foreach (var crime in trialEffect.Crimes)
			{
				if (!trialEffect.Punishments.ContainsKey(crime))
				{
					crime.Acquit(enforcer, "Found to be not guilty at trial");
					continue;
				}

				crime.Convict(enforcer, trialEffect.Punishments[crime], "Found to be guilty at trial");
			}

			// Determine what to do with them next
			if (defendant.EffectsOfType<AwaitingExecution>(x => x.LegalAuthority == trialEffect.LegalAuthority).Any())
			{
				enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
					string.Format(TrialEndRemandedAwaitingExecution,
						gender.Subjective(),
						gender.Objective(),
						gender.Possessive(),
						gender.Reflexive(),
						defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
						crimeNames.ListToString(),
						trialEffect.Crimes.Count().ToWordyNumber()
					),
					enforcer,
					enforcer,
					defendant
				)));

				// Transfer back to holding cell
				trialEffect.LegalAuthority.SendCharacterToHoldingCell(defendant);
			}
			else if (defendant.EffectsOfType<ServingCustodialSentence>(x => x.LegalAuthority == trialEffect.LegalAuthority).Any())
			{
				enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
					string.Format(TrialEndRemandedIntoCustody,
						gender.Subjective(),
						gender.Objective(),
						gender.Possessive(),
						gender.Reflexive(),
						defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
						crimeNames.ListToString(),
						trialEffect.Crimes.Count().ToWordyNumber()
					),
					enforcer,
					enforcer,
					defendant
				)));

				// Transfer to prison
				trialEffect.LegalAuthority.SendCharacterToPrison(defendant);
			}
			else
			{
				enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
					string.Format(TrialEndFreeToGo,
						gender.Subjective(),
						gender.Objective(),
						gender.Possessive(),
						gender.Reflexive(),
						defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
						crimeNames.ListToString(),
						trialEffect.Crimes.Count().ToWordyNumber()
					),
					enforcer,
					enforcer,
					defendant
				)));

				// Release
				trialEffect.LegalAuthority.ReleaseCharacterToFreedom(defendant);
			}

			defendant.RemoveAllEffects<OnTrial>(x => x.LegalAuthority == trialEffect.LegalAuthority, true);
			return true;
		}

		var result = sentenceCrime.Law.PunishmentStrategy.GetResult(defendant, sentenceCrime);
		trialEffect.Punishments[sentenceCrime] = result;
		enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
			string.Format(TrialSentencingEmote,
				gender.Subjective(),
				gender.Objective(),
				gender.Possessive(),
				gender.Reflexive(),
				defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
				crimeNames.ListToString(),
				trialEffect.Crimes.Count().ToWordyNumber(),
				trialEffect.Crimes.Count() == 1 ? "crime" : "crimes",
				sentenceCrime.TimeOfCrime.ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Long),
				sentenceCrime.DescribeCrimeAtTrial(enforcer),
				(trialEffect.Crimes.IndexBy(x => x == sentenceCrime).First().Key+1).ToWordyOrdinal(),
				result.Describe(enforcer, trialEffect.LegalAuthority)
			),
			enforcer,
			enforcer,
			defendant
		)));

		trialEffect.LastTrialAction = DateTime.UtcNow;
		return true;
	}

	private bool DoTrialTickVerdict(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		if (DateTime.UtcNow - trialEffect.LastTrialAction < TrialPhaseDelay(trialEffect.Phase, trialEffect.Crimes.Count()))
		{
			return false;
		}

		var verdictCrime = trialEffect.NextCrime();
		if (verdictCrime is null)
		{
			trialEffect.ResetCrimeQueue();
			trialEffect.Phase = TrialPhase.Sentencing;
			trialEffect.LastTrialAction = DateTime.UtcNow;
			return true;
		}

		var result = trialEffect.GetOutcome(verdictCrime);
		var guilty = false;
		var severity = 0.0;

		if (verdictCrime.Law.CrimeType.IsMajorCrime())
		{
			if (result.Outcome != OpposedOutcomeDirection.Opponent)
			{
				guilty = true;
				switch (result.Degree)
				{
					case OpposedOutcomeDegree.None:
						severity = 0.2;
						break;
					case OpposedOutcomeDegree.Marginal:
						severity = 0.4;
						break;
					case OpposedOutcomeDegree.Minor:
						severity = 0.6;
						break;
					case OpposedOutcomeDegree.Moderate:
						severity = 0.8;
						break;
					case OpposedOutcomeDegree.Major:
						severity = 1.0;
						break;
					case OpposedOutcomeDegree.Total:
						severity = 1.0;
						break;
				}
			}
			else if (result.Degree <= OpposedOutcomeDegree.Minor)
			{
				guilty = true;
				switch (result.Degree)
				{
					case OpposedOutcomeDegree.None:
						severity = 0.2;
						break;
					case OpposedOutcomeDegree.Marginal:
						severity = 0.2;
						break;
					case OpposedOutcomeDegree.Minor:
						severity = 0.0;
						break;
				}
			}
		}
		else if (verdictCrime.Law.CrimeType.IsMoralCrime() || verdictCrime.Law.CrimeType.IsViolentCrime())
		{
			if (result.Outcome != OpposedOutcomeDirection.Opponent)
			{
				guilty = true;
				switch (result.Degree)
				{
					case OpposedOutcomeDegree.None:
						severity = 0.0;
						break;
					case OpposedOutcomeDegree.Marginal:
						severity = 0.0;
						break;
					case OpposedOutcomeDegree.Minor:
						severity = 0.3;
						break;
					case OpposedOutcomeDegree.Moderate:
						severity = 0.6;
						break;
					case OpposedOutcomeDegree.Major:
						severity = 0.8;
						break;
					case OpposedOutcomeDegree.Total:
						severity = 1.0;
						break;
				}
			}
		}
		else
		{
			if (result.Outcome != OpposedOutcomeDirection.Opponent && result.Degree >= OpposedOutcomeDegree.Minor)
			{
				guilty = true;
				switch (result.Degree)
				{
					case OpposedOutcomeDegree.Minor:
						severity = 0.0;
						break;
					case OpposedOutcomeDegree.Moderate:
						severity = 0.33;
						break;
					case OpposedOutcomeDegree.Major:
						severity = 0.66;
						break;
					case OpposedOutcomeDegree.Total:
						severity = 1.0;
						break;
				}
			}
		}

		if (!trialEffect.Pleas[verdictCrime] && !guilty)
		{
			enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
				string.Format(TrialVerdictNotGuiltyEmote,
					gender.Subjective(),
					gender.Objective(),
					gender.Possessive(),
					gender.Reflexive(),
					defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
					crimeNames.ListToString(),
					trialEffect.Crimes.Count().ToWordyNumber(),
					trialEffect.Crimes.Count() == 1 ? "crime" : "crimes",
					verdictCrime.TimeOfCrime.ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Long),
					verdictCrime.DescribeCrimeAtTrial(enforcer),
					(trialEffect.Crimes.IndexBy(x => x == verdictCrime).First().Key+1).ToWordyOrdinal()
				),
				enforcer,
				enforcer,
				defendant
			)));

			trialEffect.LastTrialAction = DateTime.UtcNow;
			return true;
		}

		enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
			string.Format(TrialVerdictGuiltyEmote,
				gender.Subjective(),
				gender.Objective(),
				gender.Possessive(),
				gender.Reflexive(),
				defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
				crimeNames.ListToString(),
				trialEffect.Crimes.Count().ToWordyNumber(),
				trialEffect.Crimes.Count() == 1 ? "crime" : "crimes",
				verdictCrime.TimeOfCrime.ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Long),
				verdictCrime.DescribeCrimeAtTrial(enforcer),
				(trialEffect.Crimes.IndexBy(x => x == verdictCrime).First().Key+1).ToWordyOrdinal()
			),
			enforcer,
			enforcer,
			defendant
		)));
		trialEffect.Punishments[verdictCrime] = verdictCrime.Law.PunishmentStrategy.GetResult(defendant, verdictCrime, severity);
		trialEffect.LastTrialAction = DateTime.UtcNow;
		return true;
	}

	private bool DoTrialTickClosingArguments(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		if (DateTime.UtcNow - trialEffect.LastTrialAction < TrialPhaseDelay(trialEffect.Phase, trialEffect.Crimes.Count()))
		{
			return false;
		}

		enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
			string.Format(TrialEndArgumentsEmote,
				gender.Subjective(),
				gender.Objective(),
				gender.Possessive(),
				gender.Reflexive(),
				defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
				crimeNames.ListToString(),
				trialEffect.Crimes.Count().ToWordyNumber(),
				trialEffect.Crimes.Count() == 1 ? "crime" : "crimes"
			),
			enforcer,
			enforcer,
			defendant
		)));
		trialEffect.Phase = TrialPhase.Verdict;
		trialEffect.LastTrialAction = DateTime.UtcNow;
		trialEffect.ResetCrimeQueue();
		return true;
	}

	private bool DoTrialTickCase(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		if (trialEffect.Prosecutor == enforcer && DateTime.UtcNow - trialEffect.LastTrialAction < TimeSpan.FromSeconds(5) && trialEffect.NextProsecutionCrime() is not null)
		{
			trialEffect.HandleArgueCommand(enforcer, false);
			trialEffect.LastTrialAction = DateTime.UtcNow;
			return false;
		}

		if (DateTime.UtcNow - trialEffect.LastTrialAction < TrialPhaseDelay(trialEffect.Phase, trialEffect.Crimes.Count()))
		{
			return false;
		}

		enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
			string.Format(TrialClosingArgumentsEmote,
				gender.Subjective(),
				gender.Objective(),
				gender.Possessive(),
				gender.Reflexive(),
				defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
				crimeNames.ListToString(),
				trialEffect.Crimes.Count().ToWordyNumber(),
				trialEffect.Crimes.Count() == 1 ? "crime" : "crimes"
			),
			enforcer,
			enforcer,
			defendant
		)));
		trialEffect.Phase = TrialPhase.ClosingArguments;
		trialEffect.LastTrialAction = DateTime.UtcNow;
		return true;
	}

	private bool DoTrialTickPlea(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		if (DateTime.UtcNow - trialEffect.LastTrialAction < TrialPhaseDelay(trialEffect.Phase, trialEffect.Crimes.Count()))
		{
			return false;
		}

		var pleaEffect = defendant.EffectsOfType<ConsideringPlea>().FirstOrDefault();
		if (pleaEffect is not null)
		{
			enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
				string.Format(TrialDefaultPleaEnteredEmote,
					gender.Subjective(),
					gender.Objective(),
					gender.Possessive(),
					gender.Reflexive(),
					defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
					crimeNames.ListToString(),
					trialEffect.Crimes.Count().ToWordyNumber(),
					trialEffect.Crimes.Count() == 1 ? "crime" : "crimes",
					pleaEffect.Crime.TimeOfCrime.ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Long),
					pleaEffect.Crime.DescribeCrimeAtTrial(enforcer),
					(trialEffect.Crimes.IndexBy(x => x == pleaEffect.Crime).First().Key+1).ToWordyOrdinal()
				),
				enforcer,
				enforcer,
				defendant
			)));
			trialEffect.Pleas[pleaEffect.Crime] = true;
			trialEffect.LastTrialAction = DateTime.UtcNow;
			defendant.RemoveEffect(pleaEffect);
			return true;
		}

		var pleaCrime = trialEffect.NextCrime();
		if (pleaCrime is null)
		{
			enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
				string.Format(TrialCaseEmote,
					gender.Subjective(),
					gender.Objective(),
					gender.Possessive(),
					gender.Reflexive(),
					defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
					crimeNames.ListToString(),
					trialEffect.Crimes.Count().ToWordyNumber(),
					trialEffect.Crimes.Count() == 1 ? "crime" : "crimes"
				),
				enforcer,
				enforcer,
				defendant
			)));
			trialEffect.Phase = TrialPhase.Case;
			trialEffect.LastTrialAction = DateTime.UtcNow;
			return true;
		}

		enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
			string.Format(TrialPleaEmote,
				gender.Subjective(),
				gender.Objective(),
				gender.Possessive(),
				gender.Reflexive(),
				defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
				crimeNames.ListToString(),
				trialEffect.Crimes.Count().ToWordyNumber(),
				trialEffect.Crimes.Count() == 1 ? "crime" : "crimes",
				pleaCrime.TimeOfCrime.ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Long),
				pleaCrime.DescribeCrimeAtTrial(enforcer),
				(trialEffect.Crimes.IndexBy(x => x == pleaCrime).First().Key+1).ToWordyOrdinal()
			),
			enforcer,
			enforcer,
			defendant
		)));

		defendant.AddEffect(new ConsideringPlea(defendant, trialEffect.LegalAuthority, pleaCrime));
		defendant.OutputHandler.Send("You can #1plead guilty#0 to plead guilty or #2plead innocent#0 to plead innocent. If you do not enter a plea you will be pleading guilty by default.".SubstituteANSIColour());
		trialEffect.LastTrialAction = DateTime.UtcNow;
		return true;
	}

	private bool DoTrialTickCharges(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		if (DateTime.UtcNow - trialEffect.LastTrialAction < TrialPhaseDelay(trialEffect.Phase, trialEffect.Crimes.Count()))
		{
			return false;
		}

		enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
			string.Format(TrialChargesEmote,
				gender.Subjective(),
				gender.Objective(),
				gender.Possessive(),
				gender.Reflexive(),
				defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
				crimeNames.ListToString(),
				trialEffect.Crimes.Count().ToWordyNumber(),
				trialEffect.Crimes.Count() == 1 ? "crime" : "crimes"
			),
			enforcer,
			enforcer,
			defendant,
			new DummyPerceivable(x => defendant.PersonalName.GetName(Character.Name.NameStyle.FullName))
		)));
		trialEffect.Phase = TrialPhase.Plea;
		trialEffect.ResetCrimeQueue();
		trialEffect.LastTrialAction = DateTime.UtcNow;
		return true;
	}

	private bool DoTrialTickIntroduction(ICharacter enforcer, ICharacter defendant, OnTrial trialEffect, Gendering gender, string[] crimeNames)
	{
		enforcer.OutputHandler.Handle(new EmoteOutput(new Emote(
			string.Format(TrialIntroductionEmote,
				gender.Subjective(),
				gender.Objective(),
				gender.Possessive(),
				gender.Reflexive(),
				defendant.PersonalName.GetName(Character.Name.NameStyle.FullName),
				crimeNames.ListToString(),
				trialEffect.Crimes.Count().ToWordyNumber(),
				trialEffect.Crimes.Count() == 1 ? "crime" : "crimes"
			),
			enforcer,
			enforcer,
			defendant
		)));
		trialEffect.Phase = TrialPhase.Charges;
		trialEffect.LastTrialAction = DateTime.UtcNow;
		return true;
	}

	private TimeSpan TrialPhaseDelay(TrialPhase phase, int crimeCount)
	{
		switch (phase)
		{
			case TrialPhase.AwaitingLawyers:
				return TimeSpan.FromSeconds(30);
			case TrialPhase.Introduction:
				return IntroductionDelay;
			case TrialPhase.Charges:
				return ChargesDelay;
			case TrialPhase.Plea:
				return PleaDelay;
			case TrialPhase.Case:
				return CaseDelayPerCrime * crimeCount;
			case TrialPhase.ClosingArguments:
				return ClosingArgumentDelay;
			case TrialPhase.Verdict:
				return VerdictDelay;
			case TrialPhase.Sentencing:
				return SentencingDelay;
			default:
				throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
		}
	}
}
