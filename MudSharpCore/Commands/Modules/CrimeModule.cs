﻿using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using Humanizer.Localisation;
using MimeKit.Cryptography;
using MudSharp.Economy.Property;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using TimeSpanParserUtil;
using MudSharp.GameItems.Interfaces;
using MudSharp.Character.Name;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;

namespace MudSharp.Commands.Modules;

internal class CrimeModule : Module<ICharacter>
{
	private CrimeModule() : base("Crime")
	{
		IsNecessary = true;
	}

	public static CrimeModule Instance { get; } = new();

	private static bool EnforcerCommandAppearFunc(object och, string cmd)
	{
		if (och is not ICharacter ch)
		{
			return false;
		}

		if (ch.IsAdministrator() ||
		    ch.Gameworld.LegalAuthorities.Any(x => x.GetEnforcementAuthority(ch) is not null))
		{
			return true;
		}

		return false;
	}

	[PlayerCommand("Crimes", "crimes")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Crimes(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			var legalAuthorities = actor.Gameworld.LegalAuthorities
			                            .Where(x => x.PlayersKnowTheirCrimes)
			                            .Select(x => (Authority: x, Known: x.KnownCrimesForIndividual(actor).ToList(),
				                            Unknown: x.UnknownCrimesForIndividual(actor).ToList()))
			                            .Where(x => x.Known.Count > 0 || x.Unknown.Count > 0)
			                            .ToList();
			if (!legalAuthorities.Any())
			{
				actor.OutputHandler.Send(
					"You are a law-abiding citizen and haven't committed any crimes that you know about.");
				return;
			}

			foreach (var (authority, known, unknown) in legalAuthorities)
			{
				sb.AppendLine(
					$"You have committed the following crimes in the {authority.Name.ColourName()} jurisdiction:");
				sb.AppendLine();
				var combined = known.Concat(unknown).OrderBy(x => x.RealTimeOfCrime).ToList();
				foreach (var crime in combined)
				{
					sb.AppendLine($"\t{crime.DescribeCrime(actor)}");
				}
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			if (actor.IsAdministrator() && long.TryParse(ss.Last, out var id))
			{
				target = actor.Gameworld.TryGetCharacter(id, true);
				if (target == null)
				{
					actor.OutputHandler.Send(
						$"There is no such character with an ID of {id.ToString("N0", actor).ColourValue()}.");
					return;
				}
			}
			else
			{
				actor.OutputHandler.Send("You don't see anyone here like that.");
				return;
			}
		}

		if (target.IdentityIsObscuredTo(actor))
		{
			actor.OutputHandler.Send(
				$"You aren't sure who {target.HowSeen(actor)} is, so you can't see their crimes.");
			return;
		}

		var jurisdictions = actor.Gameworld.LegalAuthorities
		                         .Where(x => x.GetEnforcementAuthority(actor) is not null || actor.IsAdministrator())
		                         .ToList();
		if (!jurisdictions.Any())
		{
			actor.OutputHandler.Send("You are not an enforcer in any jurisdictions.");
			return;
		}

		var crimes = jurisdictions
		             .Select(x => (Authority: x, Known: x.KnownCrimesForIndividual(target).ToList(),
			             Unknown: x.UnknownCrimesForIndividual(target).ToList()))
		             .Where(x => x.Known.Count > 0 || x.Unknown.Count > 0)
		             .ToList();

		foreach (var (authority, known, unknown) in crimes)
		{
			if (!known.Any() && !actor.IsAdministrator())
			{
				continue;
			}

			sb.AppendLine($"The crimes of {target.HowSeen(actor)} in the {authority.Name.ColourName()} jurisdiction:");
			sb.AppendLine();
			var combined = known.Concat(unknown).OrderBy(x => x.RealTimeOfCrime).ToList();
			foreach (var crime in combined)
			{
				if (!crime.IsKnownCrime && !actor.IsAdministrator())
				{
					continue;
				}

				sb.AppendLine(
					$"\t{(crime.IsKnownCrime ? "[K]".Colour(Telnet.BoldCyan) : "[U]".Colour(Telnet.Red))}{crime.DescribeCrime(actor)}");
			}
		}

		if (sb.Length == 0)
		{
			actor.OutputHandler.Send($"You don't know about any crimes that {target.HowSeen(actor)} has committed.");
			return;
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Rapsheet", "rapsheet")]
	[RequiredCharacterState(CharacterState.Able)]
	[MustBeAnEnforcer]
	protected static void Rapsheet(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Whose rap sheet do you want to see?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			if (actor.IsAdministrator() && long.TryParse(ss.Last, out var id))
			{
				target = actor.Gameworld.TryGetCharacter(id, true);
				if (target == null)
				{
					actor.OutputHandler.Send(
						$"There is no such character with an ID of {id.ToString("N0", actor).ColourValue()}.");
					return;
				}
			}
			else
			{
				actor.OutputHandler.Send("You don't see anyone here like that.");
				return;
			}
		}

		if (target.IdentityIsObscuredTo(actor))
		{
			actor.OutputHandler.Send(
				$"You aren't sure who {target.HowSeen(actor)} is, so you can't lookup their rap sheet.");
			return;
		}

		var jurisdictions = actor.Gameworld.LegalAuthorities
		                         .Where(x => x.GetEnforcementAuthority(actor) is not null || actor.IsAdministrator())
		                         .ToList();
		if (!jurisdictions.Any())
		{
			actor.OutputHandler.Send("You are not an enforcer in any jurisdictions.");
			return;
		}

		var sb = new StringBuilder();
		var crimes = jurisdictions
		             .Select(x => (Authority: x, Known: x.KnownCrimesForIndividual(target).ToList(),
			             Resolved: x.ResolvedCrimesForIndividual(target).ToList()))
		             .Where(x => x.Known.Count > 0 || x.Resolved.Count > 0)
		             .ToList();

		foreach (var (authority, known, resolved) in crimes)
		{
			if (!known.Any() && !actor.IsAdministrator())
			{
				continue;
			}

			sb.AppendLine($"The crimes of {target.HowSeen(actor)} in the {authority.Name.ColourName()} jurisdiction:");
			sb.AppendLine();
			var combined = known.Concat(resolved).OrderBy(x => x.RealTimeOfCrime).ToList();
			foreach (var crime in combined)
			{
				if (!crime.IsKnownCrime && !actor.IsAdministrator())
				{
					continue;
				}

				if (crime.HasBeenConvicted)
				{
					sb.AppendLine($"\t{crime.DescribeCrime(actor)} [convicted]");
				}
				else if (crime.BailPosted)
				{
					sb.AppendLine($"\t{crime.DescribeCrime(actor)} [on bail]");
				}
				else
				{
					sb.AppendLine($"\t{crime.DescribeCrime(actor)} [unresolved]");
				}
			}
		}

		if (sb.Length == 0)
		{
			actor.OutputHandler.Send($"You don't know about any crimes that {target.HowSeen(actor)} has committed.");
			return;
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Report", "report")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Report(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		var accusableCrimes = actor.Gameworld.Crimes
		                           .Where(x => !x.IsKnownCrime && x.WitnessIds.Contains(actor.Id))
		                           .OrderByDescending(x => x.RealTimeOfCrime)
		                           .ToList();

		if (ss.IsFinished)
		{
			if (!accusableCrimes.Any())
			{
				actor.OutputHandler.Send("You aren't a witness to any unreported crimes.");
				return;
			}

			var sb = new StringBuilder();
			actor.OutputHandler.Send("You are a witness to the following unreported crimes:");
			foreach (var accusable in accusableCrimes)
			{
				sb.AppendLine(
					$"\t[#{accusable.Id.ToString("N0", actor)}] {accusable.CriminalShortDescription ?? "An unknown party".ColourCharacter()} {accusable.DescribeCrime(actor)}");
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (!long.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				"You must enter a valid ID number of a crime that you are a witness to. See REPORT with no argument for a list of crimes that you can report.");
			return;
		}

		var crime = accusableCrimes.FirstOrDefault(x => x.Id == value);
		if (crime == null)
		{
			actor.OutputHandler.Send(
				"You are not witness to any crime with that ID number. See REPORT with no argument for a list of crimes that you can report.");
			return;
		}

		var authority = crime.LegalAuthority;
		if (actor.Location != authority.PrisonLocation &&
		    actor.Location.LayerCharacters(actor.RoomLayer).All(x => !x.AffectedBy<EnforcerEffect>(authority)) &&
		    authority.GetEnforcementAuthority(actor) == null)
		{
			actor.OutputHandler.Send(
				$"You must either be an enforcer for that zone, be in the presence of an enforcer, or be at the prison location to report a crime.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ report|reports a {crime.Law.CrimeType.DescribeEnum().Colour(Telnet.BoldMagenta)} crime to the authorities.",
			actor)));
		authority.ReportCrime(crime, actor,
			actor.Dubs.Any(x => x.TargetType == "Character" && x.TargetId == crime.CriminalId), 1.0);
	}

	[PlayerCommand("Accuse", "accuse")]
	[MustBeAnEnforcer]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Accuse(ICharacter actor, string input)
	{
		var legals = actor.Gameworld.LegalAuthorities
		                  .Where(x =>
			                  x.EnforcementZones.Contains(actor.Location.Zone) &&
			                  (actor.IsAdministrator() || x.GetEnforcementAuthority(actor) is not null)
		                  )
		                  .ToList();
		var ss = new StringStack(input.RemoveFirstWord());

		if (legals.Count == 0)
		{
			actor.OutputHandler.Send("You are not in any enforcement zone for a legal authority.");
			return;
		}

		ILegalAuthority legal;
		if (legals.Count > 1)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					$"You are currently in multiple enforcement zones. You must specify which one you want to accuse someone in.\nYour options are {legals.Select(x => x.Name.ColourValue()).Humanize()}.");
				return;
			}

			legal = legals.GetByNameOrAbbreviation(ss.PopSpeech());
			if (legal == null)
			{
				actor.OutputHandler.Send(
					$"You are not an enforcer in any such zone as {ss.Last.ColourCommand()}.\nYour options are {legals.Select(x => x.Name.ColourValue()).Humanize()}.");
				return;
			}
		}
		else
		{
			legal = legals.First();
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to accuse of a crime?");
			return;
		}

		var who = actor.TargetActor(ss.PopSpeech());
		if (who == null)
		{
			if (actor.IsAdministrator() && long.TryParse(ss.Last, out var id))
			{
				who = actor.Gameworld.TryGetCharacter(id, true);
				if (who == null)
				{
					actor.OutputHandler.Send(
						$"There is no such character with an ID of {id.ToString("N0", actor).ColourValue()}.");
					return;
				}
			}
			else
			{
				actor.OutputHandler.Send("You don't see anyone here like that.");
				return;
			}
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What crime do you want to accuse {who.HowSeen(actor)} of?");
			return;
		}

		var law = legal.Laws
		               .GetByNameOrAbbreviation(ss.PopSpeech());
		if (law == null)
		{
			actor.OutputHandler.Send(
				$"There is no law on the books of the {legal.Name.ColourName()} enforcement zone called {ss.Last.ColourCommand()}.");
			return;
		}

		if (!law.OffenderClasses.Contains(legal.GetLegalClass(who)))
		{
			actor.OutputHandler.Send(
				$"{who.HowSeen(actor, true)} is not one of the legal classes that can be accused of breaking the {law.Name.ColourName()} law.");
			return;
		}

		if (!actor.IsAdministrator())
		{
			var enforcer = legal.GetEnforcementAuthority(actor);
			if (!enforcer.CanAccuse)
			{
				actor.OutputHandler.Send(
					"Your level of enforcement authority does not allow you to accuse anyone of crimes.");
				return;
			}

			if (!enforcer.AccusableClasses.Contains(legal.GetLegalClass(who)))
			{
				actor.OutputHandler.Send(
					$"Your level of enforcement authority does not allow you to accuse members of the {legal.GetLegalClass(who).Name.ColourName()} legal class.");
				return;
			}
		}

		var crime = new Crime(who, null, Enumerable.Empty<ICharacter>(), law, null)
		{
			AccuserId = actor.Id,
			CriminalIdentityIsKnown = true,
			IsKnownCrime = true,
			CrimeLocation = null
		};
		legal.AccuseCrime(crime);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ accuse|accuses $1 of the crime of $2.", actor, actor,
			who, new DummyPerceivable(voyeur => crime.DescribeCrime(voyeur)))));
	}

	[PlayerCommand("Forgive", "forgive")]
	[MustBeAnEnforcer]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Forgive(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to forgive of an active crime?");
			return;
		}

		var who = actor.TargetActor(ss.PopSpeech());
		if (who == null)
		{
			if (actor.IsAdministrator() && long.TryParse(ss.Last, out var id))
			{
				who = actor.Gameworld.TryGetCharacter(id, true);
				if (who == null)
				{
					actor.OutputHandler.Send(
						$"There is no such character with an ID of {id.ToString("N0", actor).ColourValue()}.");
					return;
				}
			}
			else
			{
				actor.OutputHandler.Send("You don't see anyone here like that.");
				return;
			}
		}

		if (who.IdentityIsObscuredTo(actor))
		{
			actor.OutputHandler.Send(
				$"You aren't sure who {who.HowSeen(actor)} is, so you can't forgive any of their crimes.");
			return;
		}

		var jurisdictions = actor.Gameworld.LegalAuthorities
		                         .Where(x => x.GetEnforcementAuthority(actor)?.CanForgive == true ||
		                                     actor.IsAdministrator()).ToList();
		if (!jurisdictions.Any())
		{
			actor.OutputHandler.Send("You are not an enforcer who can forgive crimes in any jurisdictions.");
			return;
		}

		var crimes = jurisdictions.SelectMany(x => x.KnownCrimesForIndividual(who)).ToList();
		if (actor.IsAdministrator())
		{
			crimes.AddRange(jurisdictions.SelectMany(x => x.UnknownCrimesForIndividual(who)));
		}

		if (!crimes.Any())
		{
			actor.OutputHandler.Send(
				$"{who.HowSeen(actor, true)} does not have any unresolved crimes in any jurisdiction that you are allowed to forgive crimes in.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What crime do you want to forgive {who.HowSeen(actor)} of?\nThey have the following forgivable crimes:{crimes.Select(x => $"\t{x.Id}) {x.DescribeCrime(actor)}").ListToLines()}");
			return;
		}

		if (ss.PeekSpeech().EqualTo("all"))
		{
			ss.PopSpeech();
			foreach (var crime in crimes)
			{
				crime.Forgive(actor, ss.SafeRemainingArgument);
			}

			actor.OutputHandler.Handle(new QuickEmote("@ forgive|forgives $1 of all &1's crimes.", actor, actor, who));
			foreach (var jurisdiction in jurisdictions)
			{
				jurisdiction.CheckCharacterForCustodyChanges(who);
			}

			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"You must enter a valid Id of the crime that you wish to forgive, or use the text {"all".ColourCommand()} to forgive all crimes.");
			return;
		}

		var forgivencrime = crimes.FirstOrDefault(x => x.Id == value);
		if (forgivencrime == null)
		{
			actor.OutputHandler.Send(
				$"{who.HowSeen(actor, true)} does not have any unresolved crimes with that Id.\nThey have the following forgivable crimes:{crimes.Select(x => $"\t{x.Id}) {x.DescribeCrime(actor)}").ListToLines()}");
			return;
		}

		forgivencrime.Forgive(actor, ss.SafeRemainingArgument);
		actor.OutputHandler.Handle(new QuickEmote("@ forgive|forgives $1 of the crime of $2.", actor, actor, who,
			new DummyPerceivable(x => forgivencrime.DescribeCrime(x))));
		forgivencrime.LegalAuthority.CheckCharacterForCustodyChanges(who);
	}

	[PlayerCommand("Convict", "convict")]
	[RequiredCharacterState(CharacterState.Able)]
	[MustBeAnEnforcer]
	protected static void Convict(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
	}

	[PlayerCommand("Pardon", "pardon")]
	[RequiredCharacterState(CharacterState.Able)]
	[MustBeAnEnforcer]
	protected static void Pardon(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
	}

	[PlayerCommand("TakeCustody", "takecustody")]
	[RequiredCharacterState(CharacterState.Able)]
	[MustBeAnEnforcer]
	protected static void TakeCustody(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to take custody of?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot take custody of yourself.");
			return;
		}

		if (target.IdentityIsObscuredTo(actor))
		{
			actor.OutputHandler.Send(
				$"You aren't sure who {target.HowSeen(actor)} is, so you can't take custody of them.");
			return;
		}

		var jurisdictions = actor.Gameworld.LegalAuthorities
		                         .Where(x => x.GetEnforcementAuthority(actor) is not null || actor.IsAdministrator())
		                         .ToList();
		if (!jurisdictions.Any())
		{
			actor.OutputHandler.Send("You are not an enforcer in any jurisdictions.");
			return;
		}

		jurisdictions.RemoveAll(x =>
			!x.KnownCrimesForIndividual(target).Any() && !target.AffectedBy<ServingCustodialSentence>(x));
		if (!jurisdictions.Any())
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true)} is not wanted for any crimes or serving a custodial sentence in any of your jurisdictions.");
			return;
		}

		ILegalAuthority jurisdiction;
		if (jurisdictions.Count > 1 && !ss.IsFinished)
		{
			jurisdiction = jurisdictions.GetByNameOrAbbreviation(ss.SafeRemainingArgument);
			if (jurisdiction == null)
			{
				actor.OutputHandler.Send(
					$"You are not an enforcer in any jurisdiction by the name of {ss.SafeRemainingArgument.ColourCommand()}.");
				return;
			}
		}
		else
		{
			jurisdiction = jurisdictions.OrderByDescending(x => x.EnforcementZones.Contains(actor.Location.Zone))
			                            .First();
		}

		var existing = target.CombinedEffectsOfType<InCustodyOfEnforcer>()
		                     .FirstOrDefault(x => x.LegalAuthority == jurisdiction);
		if (existing != null && existing.Enforcer.ColocatedWith(target))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true)} is already in the custody of {existing.Enforcer.HowSeen(actor)}.\n{"Hint: Ask them to transfer custody to you.".ColourCommand()}");
			return;
		}

		if (existing is null)
		{
			target.AddEffect(new InCustodyOfEnforcer(target, actor, jurisdiction));
		}
		else
		{
			existing.Enforcer = actor;
		}

		actor.OutputHandler.Handle(new QuickEmote("@ take|takes custody of $1.", actor, actor, target));
		target.OutputHandler.Send(
			"You are now in the custody of an enforcer. You could be guilty of a crime if you move away."
				.ColourCommand());
	}

	[PlayerCommand("TransferCustody", "transfercustody")]
	[RequiredCharacterState(CharacterState.Able)]
	[MustBeAnEnforcer]
	protected static void TransferCustody(ICharacter actor, string input)
	{
	}

	[PlayerCommand("EndCustody", "endcustody")]
	[RequiredCharacterState(CharacterState.Able)]
	[MustBeAnEnforcer]
	protected static void EndCustody(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to end the custody of?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		var effects = target.CombinedEffectsOfType<InCustodyOfEnforcer>()
		                    .Where(x => x.Enforcer == actor || actor.IsAdministrator()).ToList();
		if (!effects.Any())
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not in your custody.");
			return;
		}

		foreach (var effect in effects)
		{
			target.RemoveEffect(effect, true);
		}

		actor.OutputHandler.Handle(new QuickEmote("@ release|releases $1 from custody.", actor, actor, target));
	}

	[PlayerCommand("Patrols", "patrols")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Patrols(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var authorities = actor.Gameworld.LegalAuthorities.WhereNotNull(x => x.GetEnforcementAuthority(actor))
		                       .ToList();

		if (!authorities.Any())
		{
			if (!actor.IsAdministrator())
			{
				actor.OutputHandler.Send("You are not an enforcer in any legal zones.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which legal authorities do you want to see patrols for?");
				return;
			}

			var authority = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.SafeRemainingArgument);
			if (authority == null)
			{
				actor.OutputHandler.Send("There is no such legal authority.");
				return;
			}

			authorities.Add(authority);
		}

		var sb = new StringBuilder();
		foreach (var authority in authorities)
		{
			sb.AppendLine($"Patrols for the {authority.Name.ColourName()} Legal Authority:");
			sb.AppendLine();
			foreach (var patrol in authority.Patrols)
			{
				sb.AppendLine(
					$"\t{patrol.PatrolRoute.Name.ColourName()} - {patrol.PatrolPhase.DescribeEnum().ColourValue()}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("PermitWork", "permitwork", "permitworkproperty")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void PermitWork(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var property = ss.Pop().EqualTo("permitworkproperty");
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send($"You don't see anyone like that to give a permit to work.");
			return;
		}

		if (property && !actor.Gameworld.Properties.Any(x => x.PropertyLocations.Contains(actor.Location)))
		{
			actor.OutputHandler.Send("Your current location is not a part of any property.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"How long do you want to give them a permit to work?");
			return;
		}

		if (!TimeSpanParser.TryParse(ss.SafeRemainingArgument, Units.Hours, Units.Hours, out var timespan))
		{
			actor.OutputHandler.Send(
				$"That was not a valid time span.\n{"Note: Years and Months are not supported, use Weeks or Days in that case".ColourCommand()}");
			return;
		}

		if (!actor.IsAdministrator() && !actor.Gameworld.LegalAuthorities.Any(x =>
			    x.EnforcementZones.Contains(actor.Location.Zone) && x.GetEnforcementAuthority(actor) != null))
		{
			if (property)
			{
				var localProperty =
					actor.Gameworld.Properties.First(x => x.PropertyLocations.Contains(actor.Location));
				if (!localProperty.IsAuthorisedLeaseHolder(actor) || !localProperty.IsAuthorisedOwner(actor))
				{
					actor.OutputHandler.Send(
						$"You are not an authorised owner or leaseholder for this property, and so cannot permit others to work.");
					return;
				}

				target.RemoveAllEffects<PermitWork>(x => x.Property == localProperty);
				target.AddEffect(new PermitWork(target) { Property = localProperty }, timespan);
				actor.OutputHandler.Handle(new EmoteOutput(new Emote(
						"@ authorise|authorises $1 to work on the property $2 for $3.",
						actor,
						actor,
						target,
						new DummyPerceivable(localProperty.Name, customColour: Telnet.Cyan),
						new DummyPerceivable(perceiver =>
							timespan.Humanize(2, perceiver.Account.Culture, TimeUnit.Year, TimeUnit.Second))
					)
				));
				return;
			}

			actor.OutputHandler.Send("You are not authorised to permit others to work in this location.");
			return;
		}

		target.RemoveAllEffects<PermitWork>(x => x.Cell == actor.Location);
		target.AddEffect(new PermitWork(target) { Cell = actor.Location }, timespan);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ authorise|authorises $1 to work in this location for $2.", actor, actor, target,
			new DummyPerceivable(perceiver =>
				timespan.Humanize(2, perceiver.Account.Culture, TimeUnit.Year, TimeUnit.Second)))));
	}


	[PlayerCommand("BailOut", "bailout")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void BailOut(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("me"))
		{
			ss.PopSpeech();
			var effect = actor.EffectsOfType<AwaitingSentencing>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("You are not being held in custody on remand by any legal authorities.");
				return;
			}

			var authority = effect.LegalAuthority;
			var currency = authority.Currency;

			var crimes = authority.KnownCrimesForIndividual(actor).ToList();
			if (crimes.Any(x => !x.Law.CanBeOfferedBail))
			{
				actor.OutputHandler.Send("Some of the crimes you are accused of do not permit bail.");
				return;
			}

			var bailText = string.Empty;
			var bail = crimes.Sum(x => authority.BailCalculationProg?.Execute<decimal?>(actor, x) ?? 0.0M);
			if (bail > 0.0M)
			{
				if (!ss.IsFinished)
				{
					var (account, error) = Bank.FindBankAccount(ss.SafeRemainingArgument, null);
					if (account is null)
					{
						actor.OutputHandler.Send(error);
						return;
					}

					if (!account.IsAuthorisedAccountUser(actor))
					{
						actor.OutputHandler.Send($"You are not an authorised person for that bank account.");
						return;
					}

					var (truth, withdrawError) = account.CanWithdraw(bail, true);
					if (!truth)
					{
						actor.OutputHandler.Send(withdrawError);
						return;
					}

					account.WithdrawFromTransaction(bail, "Making bail");
					authority.BankAccount?.DepositFromTransaction(bail,
						$"Bail deposit for {actor.PersonalName.GetName(NameStyle.FullName)}");
					bailText = $"with funds from the bank account {account.AccountReference.ColourValue()}";
				}
				else
				{
					var bundle =
						effect.LegalAuthority.PrisonerBelongingsStorageLocation.GameItems.FirstOrDefault(x =>
							x.AffectedBy<PrisonerBelongings>(actor));
					if (bundle is null)
					{
						actor.OutputHandler.Send(
							$"You do not have enough money in your stored belongings to make bail.\nYou would require {currency.Describe(bail, CurrencyDescriptionPatternType.Short).ColourValue()}.");
						return;
					}

					var container = bundle.GetItemType<IContainer>();
					var piles = container.Contents.RecursiveGetItems<ICurrencyPile>(false).ToList();
					var found = currency.FindCurrency(piles, bail);
					var value = found.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
					if (value < bail)
					{
						actor.OutputHandler.Send(
							$"You do not have enough money in your stored belongings to make bail.\nYou would require {currency.Describe(bail, CurrencyDescriptionPatternType.Short).ColourValue()} but you only have {currency.Describe(value, CurrencyDescriptionPatternType.Short).ColourValue()}.");
						return;
					}

					var change = value - bail;

					foreach (var item in found.Where(item =>
						         !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
					{
						item.Key.Parent.Delete();
					}

					if (change > 0.0M)
					{
						container.Put(actor,
							GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
								currency.FindCoinsForAmount(change, out _)));
					}

					if (authority.BankAccount is not null)
					{
						authority.BankAccount.Bank.CurrencyReserves[currency] += bail;
						authority.BankAccount.DepositFromTransaction(bail,
							$"Bail deposit for {actor.PersonalName.GetName(NameStyle.FullName)}");
					}

					bailText = "with cash from your stored belongings";
				}

				authority.CalculateAndSetBail(actor);
			}

			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote(actor.Gameworld.GetStaticString("BailReleaseSelfInitiatedEmoteOrigin"), actor, actor),
				flags: OutputFlags.SuppressSource));
			actor.OutputHandler.Send(new EmoteOutput(new Emote(
				string.Format(actor.Gameworld.GetStaticString("BailReleaseSelfInitiatedEmoteSelf"), bailText), actor,
				actor)));
			authority.ReleaseCharacterToFreedom(actor);
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote(actor.Gameworld.GetStaticString("BailReleaseSelfInitiatedEmoteDestination"), actor, actor),
				flags: OutputFlags.SuppressSource));
			actor.AddEffect(new OnBail(actor, authority, effect.ArrestTime));
			return;
		}

		var jurisdiction = actor.Gameworld.LegalAuthorities.FirstOrDefault(x => x.PrisonLocation == actor.Location);
		if (jurisdiction is null)
		{
			actor.OutputHandler.Send($"You are not at the prison location of any legal jurisdiction.");
			return;
		}

		var prisoners = jurisdiction.CellLocations.SelectMany(x => x.Characters)
		                            .Where(x => x.AffectedBy<AwaitingSentencing>(jurisdiction)).ToList();
		var whoText = ss.PopSpeech();
		if (whoText.EqualTo("list"))
		{
			if (!prisoners.Any())
			{
				actor.OutputHandler.Send("There are no prisoners in the cells who are currently eligible for bail.");
				return;
			}

			var sb = new StringBuilder();
			sb.AppendLine("Prisoners on Remand:");
			sb.AppendLine();
			foreach (var prisoner in prisoners)
			{
				var crimes = jurisdiction.KnownCrimesForIndividual(prisoner).ToList();
				var prisonerBail = jurisdiction.KnownCrimesForIndividual(prisoner).Sum(x =>
					jurisdiction.BailCalculationProg?.Execute<decimal?>(prisoner, x) ?? 0.0M);
				if (crimes.Any(x => !x.Law.CanBeOfferedBail) || prisonerBail <= 0.0M)
				{
					sb.AppendLine(
						$"\t{prisoner.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} ({prisoner.PersonalName.GetName(NameStyle.FullName).ColourName()}) - {"Not Eligible".Colour(Telnet.Red)}");
					continue;
				}

				sb.AppendLine(
					$"\t{prisoner.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} ({prisoner.PersonalName.GetName(NameStyle.FullName).ColourName()}) - {jurisdiction.Currency.Describe(prisonerBail, CurrencyDescriptionPatternType.Short).ColourValue()}");
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var who = prisoners.GetFromItemListByKeywordIncludingNames(whoText, actor);
		if (who is null)
		{
			actor.OutputHandler.Send(
				$"There is no such prisoner eligible for bail currently on remand. Please see {"BAILOUT LIST".MXPSend("bailout list")} for a list of bail-eligible prisoners.");
			return;
		}

		var calculatedBail = jurisdiction.KnownCrimesForIndividual(who)
		                                 .Sum(x => jurisdiction.BailCalculationProg?.Execute<decimal?>(who, x) ?? 0.0M);

		string bailActionText;
		if (ss.IsFinished)
		{
			var (account, error) = Bank.FindBankAccount(ss.SafeRemainingArgument, null);
			if (account is null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (!account.IsAuthorisedAccountUser(actor))
			{
				actor.OutputHandler.Send($"You are not an authorised person for that bank account.");
				return;
			}

			var (truth, withdrawError) = account.CanWithdraw(calculatedBail, true);
			if (!truth)
			{
				actor.OutputHandler.Send(withdrawError);
				return;
			}

			account.WithdrawFromTransaction(calculatedBail, "Paying for someone else's bail");
			jurisdiction.BankAccount?.DepositFromTransaction(calculatedBail,
				$"Bail deposit for {who.PersonalName.GetName(NameStyle.FullName)}");
			bailActionText = $"with funds from the bank account {account.AccountReference.ColourValue()}";
		}
		else
		{
			var payment = new OtherCashPayment(jurisdiction.Currency, actor);
			if (payment.AccessibleMoneyForPayment() < calculatedBail)
			{
				actor.OutputHandler.Send(
					$"You aren't holding enough money to pay {jurisdiction.Currency.Describe(calculatedBail, CurrencyDescriptionPatternType.Short).ColourValue()} in bail for {who.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}\nYou are only holding {jurisdiction.Currency.Describe(payment.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()}.");
				return;
			}

			payment.TakePayment(calculatedBail);
			if (jurisdiction.BankAccount is not null)
			{
				jurisdiction.BankAccount.Bank.CurrencyReserves[jurisdiction.Currency] += calculatedBail;
				jurisdiction.BankAccount.DepositFromTransaction(calculatedBail,
					$"Bail deposit for {who.PersonalName.GetName(NameStyle.FullName)}");
			}

			bailActionText = "with cash";
		}

		var arrestTime =
			who.EffectsOfType<AwaitingSentencing>(x => x.LegalAuthority == jurisdiction).FirstOrDefault()?.ArrestTime ??
			jurisdiction.EnforcementZones.First().DateTime();
		who.OutputHandler.Handle(new EmoteOutput(
			new Emote(actor.Gameworld.GetStaticString("BailReleaseExternalPartyEmoteOrigin"), who, who),
			flags: OutputFlags.SuppressSource));
		who.OutputHandler.Send(new EmoteOutput(
			new Emote(actor.Gameworld.GetStaticString("BailReleaseExternalPartyEmoteSelf"), who, who, actor)));
		actor.OutputHandler.Send(new EmoteOutput(new Emote(
			string.Format(actor.Gameworld.GetStaticString("BailReleaseExternalPartyEmoteBailer"), bailActionText),
			actor, actor, who)));
		jurisdiction.ReleaseCharacterToFreedom(who);
		who.OutputHandler.Handle(new EmoteOutput(
			new Emote(actor.Gameworld.GetStaticString("BailReleaseExternalPartyEmoteDestination"), who, who),
			flags: OutputFlags.SuppressSource));
		actor.AddEffect(new OnBail(who, jurisdiction, arrestTime));
	}

	[PlayerCommand("ReturnBail", "returnbail")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void ReturnBail(ICharacter actor, string input)
	{
		var jurisdiction = actor.Gameworld.LegalAuthorities.FirstOrDefault(x => x.PrisonLocation == actor.Location);
		if (jurisdiction is null)
		{
			actor.OutputHandler.Send($"You are not at the prison location of any legal jurisdiction.");
			return;
		}

		var effect = actor.EffectsOfType<OnBail>(x => x.LegalAuthority == jurisdiction).FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send(
				$"You are not on bail in the {jurisdiction.Name.ColourName()} legal jurisdiction.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(actor.Gameworld.GetStaticString("ReturnFromBailEmote"),
			actor, actor)));
		var bailReturned = jurisdiction.KnownCrimesForIndividual(actor).Sum(x => x.CalculatedBail);
		if (bailReturned > 0.0M)
		{
			actor.OutputHandler.Send(
				$"Your bail amount of {jurisdiction.Currency.Describe(bailReturned, CurrencyDescriptionPatternType.Short).ColourValue()} has been added to your belongings, which will be returned when you are released from custody.");
		}

		jurisdiction.IncarcerateCriminal(actor);

		if (bailReturned > 0.0M)
		{
			var bundle =
				jurisdiction.PrisonerBelongingsStorageLocation.GameItems.FirstOrDefault(x =>
					x.AffectedBy<PrisonerBelongings>(actor));
			var money = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(jurisdiction.Currency,
				jurisdiction.Currency.FindCoinsForAmount(bailReturned, out _));
			if (bundle is not null)
			{
				bundle.GetItemType<IContainer>().Put(actor, money);
			}
			else
			{
				bundle = GameItems.Prototypes.PileGameItemComponentProto.CreateNewBundle(new[] { money });
				actor.Gameworld.Add(bundle);
				bundle.AddEffect(new PrisonerBelongings(bundle, actor));
				jurisdiction.PrisonerBelongingsStorageLocation.Insert(bundle, true);
				bundle.SetEmote(new Emote(
					$"marked as the property of {actor.CurrentName.GetName(NameStyle.FullWithNickname)}", bundle));
			}
		}
	}
}