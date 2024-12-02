using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using MudSharp.Accounts;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.RPG.Law;

namespace MudSharp.Commands.Modules;

public class LegalModule : Module<ICharacter>
{
	private LegalModule()
		: base("Legal")
	{
		IsNecessary = true;
	}

	public static LegalModule Instance { get; } = new();

	private const string LegalAuthorityHelpText =
		@"This command is used to edit and manage legal authorities, which are the entities which create and enforce laws. The valid options for this command are:

	#3legal list#0 - lists all legal authorities
	#3legal edit new <name> <currency>#0 - creates a new legal authority
	#3legal edit <authority>#0 - opens a legal authority for editing
	#3legal edit#0 - equivalent to the show option for your currently edited authority
	#3legal show <which>#0 - shows a legal authority in detail
	#3legal close#0 - closes your currently edited authority
	#3legal delete#0 - permanently deletes the edited legal authority
	#3legal laws [<legal authority>]#0 - shows all the laws
	#3legal classes [<legal authority>]#0 - shows all the classes
	#3legal enforcements [<legal authority>]#0 - shows all enforcer authorities
	#3legal patrols [<legal authority>]#0 - shows all patrol routes

You can also use the following options to change the properties of an authority that you are editing:

	#3legal set name <name>#0 - renames this legal authority
	#3legal set currency <currency>#0 - changes the currency that fines and such will be issued in
	#3legal set know#0 - toggles whether players know the crimes they have committed
	#3legal set zone <zone>#0 - toggles a zone as in or out of the enforcement area of this legal authority
	#3legal set class add <name>#0 - adds a new legal class
	#3legal set class delete <name>#0 - deletes a legal class
	#3legal set class <which> ...#0 - sets properties of a legal class
	#3legal set enforcement add <name>#0 - adds a new enforcer authority
	#3legal set enforcement delete <name>#0 - deletes an enforcement authority
	#3legal set enforcement <which> ...#0 - sets the properties of an enforcement authority
	#3legal set inflate <multiplier>#0 - changes all fines by th specified multiplier
	#3legal set law add <name> <type>#0 - adds a new law of the specified type
	#3legal set law delete <name>#0 - deletes a law
	#3legal set law <which> ...#0 - sets properties of a law
	#3legal set patrol add <name>#0 - creates a new patrol template
	#3legal set patrol delete <name>#0 - deletes a patrol template
	#3legal set patrol <which> ...#0 - sets properties of a patrol template
	#3legal set prepare here|<room>#0 - sets the patrol preparation room (usually an armoury)
	#3legal set marshalling here|<room>#0 - sets the patrol marshalling room (where patrols launch)
	#3legal set stow here|<room>#0 - sets the stowing location for enforcers not on duty
	#3legal set prison here|<room>#0 - sets the prison administration location for this authority
	#3legal set release here|<room>#0 - sets the prison release location for this authority
	#3legal set belongings here|<room>#0 - sets the prison belongings stowage location for this authority
	#3legal set cell here|<room>#0 - toggles a location as a holding cell for this authority
	#3legal set imprisonedprog <prog>#0 - sets the on-imprisoned prog (when convicted and sent to jail)
	#3legal set imprisonedprog none#0 - clears the on-imprisoned prog
	#3legal set heldprog <prog>#0 - sets the on-held prog (when arrested and held in a cell)
	#3legal set heldprog none#0 - clears the on-held prog
	#3legal set releasedprog <prog>#0 - sets the on-released prog
	#3legal set releasedprog none#0 - clears the on-released prog
	#3legal set jailentry here|<room>#0 - sets the entry to the custodial jail
	#3legal set jail here|<room>#0 - toggles a location as a part of the custodial jail
	#3legal set court here|<room>#0 - sets the courtroom location for this authority
	#3legal set bankaccount <code>:<accn>#0 - sets the bank account for fines paid
	#3legal set autoconvict#0 - toggles automatic application of convictions
	#3legal set autoconvicttime <timespan>#0 - sets the delay before applying auto conviction
	#3legal set discord <channelid>|none#0 - sets or clears the discord announce channel
	#3legal set bail <prog>#0 - sets the prog which determines the bail amount for a crime";

	[PlayerCommand("LegalAuthority", "legalauthority")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("legalauthority", LegalAuthorityHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void LegalAuthority(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				LegalAuthorityList(actor, ss);
				return;
			case "laws":
				LegalAuthorityLaws(actor, ss);
				return;
			case "classes":
				LegalAuthorityClasses(actor, ss);
				return;
			case "enforcements":
				LegalAuthorityEnforcements(actor, ss);
				return;
			case "patrols":
				LegalAuthorityPatrols(actor, ss);
				return;
			case "edit":
			case "open":
				LegalAuthorityEdit(actor, ss);
				return;
			case "show":
			case "view":
				LegalAuthorityShow(actor, ss);
				return;
			case "close":
				LegalAuthorityClose(actor, ss);
				return;
			case "delete":
				LegalAuthorityDelete(actor, ss);
				return;
			case "set":
				LegalAuthoritySet(actor, ss);
				return;
		}

		actor.OutputHandler.Send(LegalAuthorityHelpText.SubstituteANSIColour());
	}

	private static void LegalAuthorityPatrols(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
		if (ss.IsFinished)
		{
			if (editing is null)
			{
				actor.OutputHandler.Send("You must specify a legal authority if you are not editing one.");
				return;
			}
		}
		else
		{
			editing = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.PopSpeech());
			if (editing is null)
			{
				actor.OutputHandler.Send($"There is no legal authority identified by the text {ss.Last.ColourCommand()}.");
				return;
			}
		}

		var patrols = editing.PatrolRoutes.ToList();
		// TODO - filtering
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in patrols
			select new List<string>
			{
				item.Id.ToStringN0(actor),
				item.Name,
				item.Priority.ToStringN0(actor),
				item.LingerTimeMajorNode.DescribePreciseBrief(actor),
				item.LingerTimeMinorNode.DescribePreciseBrief(actor),
				item.TimeOfDays.Select(x => x.DescribeColour()).ListToString(),
				item.PatrolStrategy.Name
			},
			new List<string>
			{
				"Id",
				"Name",
				"Priority",
				"Linger Major",
				"Linger Minor",
				"Times",
				"Strategy"
			},
			actor,
			Telnet.Magenta));
	}

	private static void LegalAuthorityEnforcements(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
		if (ss.IsFinished)
		{
			if (editing is null)
			{
				actor.OutputHandler.Send("You must specify a legal authority if you are not editing one.");
				return;
			}
		}
		else
		{
			editing = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.PopSpeech());
			if (editing is null)
			{
				actor.OutputHandler.Send($"There is no legal authority identified by the text {ss.Last.ColourCommand()}.");
				return;
			}
		}

		var enforcements = editing.EnforcementAuthorities.ToList();
		// TODO - filtering
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in enforcements
			select new List<string>
			{
				item.Id.ToStringN0(actor),
				item.Name,
				item.FilteringProg?.MXPClickableFunctionName() ?? "",
				item.CanAccuse.ToColouredString(),
				item.CanConvict.ToColouredString(),
				item.CanForgive.ToColouredString(),
				item.IncludedAuthorities.Select(x => x.Name).ListToString()
			},
			new List<string>
			{
				"Id",
				"Name",
				"Prog",
				"Accuse?",
				"Convict?",
				"Forgive?",
				"Includes"
			},
			actor,
			Telnet.Magenta));
	}

	private static void LegalAuthorityClasses(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
		if (ss.IsFinished)
		{
			if (editing is null)
			{
				actor.OutputHandler.Send("You must specify a legal authority if you are not editing one.");
				return;
			}
		}
		else
		{
			editing = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.PopSpeech());
			if (editing is null)
			{
				actor.OutputHandler.Send($"There is no legal authority identified by the text {ss.Last.ColourCommand()}.");
				return;
			}
		}

		var classes = editing.LegalClasses.ToList();
		// TODO - filtering
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in classes
			select new List<string>
			{
				item.Id.ToStringN0(actor),
				item.Name,
				item.MembershipProg?.MXPClickableFunctionName() ?? "",
				item.CanBeDetainedUntilFinesPaid.ToColouredString(),
				item.LegalClassPriority.ToStringN0(actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Prog",
				"Arrest for Fines?",
				"Priority"
			},
			actor,
			Telnet.Magenta));
	}

	private static void LegalAuthorityLaws(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
		if (ss.IsFinished)
		{
			if (editing is null)
			{
				actor.OutputHandler.Send("You must specify a legal authority if you are not editing one.");
				return;
			}
		}
		else
		{
			editing = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.PopSpeech());
			if (editing is null)
			{
				actor.OutputHandler.Send($"There is no legal authority identified by the text {ss.Last.ColourCommand()}.");
				return;
			}
		}

		var laws = editing.Laws.ToList();
		// TODO - filtering
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in laws
			select new List<string>
			{
				item.Id.ToStringN0(actor),
				item.Name,
				item.CrimeType.DescribeEnum(),
				item.ActivePeriod.DescribePreciseBrief(actor),
				item.CanBeAppliedAutomatically.ToColouredString(),
				item.CanBeArrested.ToColouredString(),
				item.CanBeOfferedBail.ToColouredString(),
				item.DoNotAutomaticallyApplyRepeats.ToColouredString(),
				item.EnforcementStrategy.DescribeEnum(),
				item.EnforcementPriority.ToStringN0(actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Type",
				"Active",
				"Automatic?",
				"Arrest?",
				"Bail?",
				"No Repeat?",
				"Enforcement",
				"Priority"
			},
			actor,
			Telnet.Magenta));
	}

	private static void LegalAuthorityList(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from authority in actor.Gameworld.LegalAuthorities
			select new[]
			{
				authority.Id.ToString("N0", actor),
				authority.Name,
				authority.EnforcementZones.Select(x => x.Name).ListToString(),
				authority.Laws.Count().ToString("N0", actor),
				authority.UnknownCrimes.Count().ToString("N0", actor),
				authority.KnownCrimes.Count().ToString("N0", actor),
				authority.StaleCrimes.Count().ToString("N0", actor),
				authority.ResolvedCrimes.Count().ToString("N0", actor)
			},
			new[]
			{
				"Id",
				"Name",
				"Zones",
				"Laws",
				"Unknown Cases",
				"Unsolved Cases",
				"Cold Cases",
				"Resolved Cases"
			},
			actor.LineFormatLength,
			colour: Telnet.Cyan,
			truncatableColumnIndex: 2,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void LegalAuthorityEdit(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (editing != null)
			{
				actor.OutputHandler.Send(editing.EditingItem.Show(actor));
				return;
			}

			actor.OutputHandler.Send("Which legal authority is it that you want to edit?");
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must supply a name for your new legal authority.");
				return;
			}

			var name = ss.PopSpeech().TitleCase().Trim();
			if (actor.Gameworld.LegalAuthorities.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					"There is already a legal authority with that name. Legal authorities must have unique names.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					"You must specify a currency for the legal authority to use for its fines.");
				return;
			}

			var currency = long.TryParse(ss.PopSpeech(), out var currencyid)
				? actor.Gameworld.Currencies.Get(currencyid)
				: actor.Gameworld.Currencies.GetByName(ss.Last);
			if (currency == null)
			{
				actor.OutputHandler.Send("There is no such currency.");
				return;
			}

			var newAuthority = new LegalAuthority(name, currency, actor.Gameworld);
			actor.Gameworld.Add(newAuthority);
			actor.RemoveAllEffects<BuilderEditingEffect<ILegalAuthority>>();
			actor.AddEffect(new BuilderEditingEffect<ILegalAuthority>(actor) { EditingItem = newAuthority });
			actor.OutputHandler.Send(
				$"You create a new legal authority called {name.ColourName()} with ID {newAuthority.Id.ToString("N0", actor)}, which you are now editing.");
			return;
		}

		var authority = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.LegalAuthorities.Get(value)
			: actor.Gameworld.LegalAuthorities.GetByName(ss.Last);
		if (authority == null)
		{
			actor.OutputHandler.Send("There is no such legal authority.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILegalAuthority>>();
		actor.AddEffect(new BuilderEditingEffect<ILegalAuthority>(actor) { EditingItem = authority });
		actor.OutputHandler.Send($"You are now editing the {authority.Name.ColourName()} legal authority.");
	}

	private static void LegalAuthorityShow(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (editing == null)
			{
				actor.OutputHandler.Send("Which legal authority do you want to be shown?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var authority = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.LegalAuthorities.Get(value)
			: actor.Gameworld.LegalAuthorities.GetByName(ss.Last);
		if (authority == null)
		{
			actor.OutputHandler.Send("There is no such legal authority.");
			return;
		}

		actor.OutputHandler.Send(authority.Show(actor));
	}

	private static void LegalAuthorityClose(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any legal authorities.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILegalAuthority>>();
		actor.OutputHandler.Send("You are no longer editing any legal authorities.");
	}

	private static void LegalAuthorityDelete(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator(PermissionLevel.HighAdmin))
		{
			actor.OutputHandler.Send(
				"This command has serious enough consequences that it is restricted to high administrators only.");
			return;
		}

		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any legal authorities.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you absolutely sure that you want to delete the {editing.EditingItem.Name.ColourName()} legal authority? This action cannot be undone, and will delete all associated data, crimes, laws, etc.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send(
					$"You delete the {editing.EditingItem.Name.ColourName()} legal authority and all of its associated data.");
				editing.EditingItem.Delete();
				actor.RemoveEffect(editing);
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to delete the legal authority."); },
			ExpireAction = () => actor.OutputHandler.Send("You decide not to delete the legal authority."),
			DescriptionString = "Deleting a legal authority",
			Keywords = new List<string> { "legal", "authority", "delete" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void LegalAuthoritySet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any legal authorities.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	[PlayerCommand("ShowPatrols", "showpatrols")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void ShowPatrols(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var legals = actor.Gameworld.LegalAuthorities.ToList();
		if (!ss.IsFinished)
		{
			var legal = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.SafeRemainingArgument);
			if (legal == null)
			{
				actor.OutputHandler.Send("There is no such legal authority.");
				return;
			}

			legals.Clear();
			legals.Add(legal);
		}

		var sb = new StringBuilder();
		foreach (var legal in legals)
		{
			if (sb.Length > 0)
			{
				sb.AppendLine();
			}
			sb.AppendLine($"Patrol Information for {legal.Name} Authority".GetLineWithTitleInner(actor, Telnet.BoldBlue, Telnet.BoldWhite));
			sb.AppendLine();
			var enforcers =
			actor.Gameworld.NPCs
						  .Where(x =>
							  x.AffectedBy<EnforcerEffect>(legal))
						  .ToList();
			var freeEnforcers = enforcers.Where(x => legal.Patrols.All(y => !y.PatrolMembers.Contains(x))).ToList();
			var enforcerCounts = new CollectionDictionary<IEnforcementAuthority, ICharacter>();
			foreach (var group in freeEnforcers.GroupBy(x => legal.GetEnforcementAuthority(x)))
			{
				enforcerCounts.AddRange(group.Key, group);
			}

			sb.AppendLine($"Total Enforcers: {enforcers.Count.ToStringN0Colour(actor)}");
			sb.AppendLine($"Free Enforcers: {freeEnforcers.Count.ToStringN0Colour(actor)}");
			sb.AppendLine($"Enforcer Counts: {enforcerCounts.Select(x => $"{x.Value.Count.ToStringN0(actor)} {x.Key.Name.Pluralise(x.Value.Count != 1).ColourName()}").ListToString()}");
			var patrols = legal.Patrols;
			var inactivePatrols = legal.PatrolRoutes.Where(x => patrols.All(y => y.PatrolRoute != x)).ToList();
			foreach (var patrol in patrols)
			{
				sb.AppendLine();
				sb.AppendLine(
					$"Patrol \"{patrol.PatrolRoute.Name}\" (#{patrol.Id.ToString("N0", actor)})".GetLineWithTitleInner(actor, Telnet.BoldBlue, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"Phase: {patrol.PatrolPhase.DescribeEnum().ColourValue()}");
				sb.AppendLine($"Strategy: {patrol.PatrolStrategy.Name.ColourValue()}");
				sb.AppendLine($"Leader: {patrol.PatrolLeader?.HowSeen(actor) ?? "Noone".ColourCharacter()}");
				sb.AppendLine($"Members: {patrol.PatrolMembers.Select(x => x.HowSeen(actor)).ListToString()}");
				sb.AppendLine($"Origin: {patrol.OriginLocation.HowSeen(actor)}");
				sb.AppendLine(
					$"Leader Location: {patrol.PatrolLeader?.Location.HowSeen(actor) ?? "Nowhere".Colour(Telnet.Red)}");
				sb.AppendLine($"Last Node: {patrol.LastMajorNode?.HowSeen(actor) ?? "None".Colour(Telnet.Red)}");
				sb.AppendLine($"Next Node: {patrol.NextMajorNode?.HowSeen(actor) ?? "None".Colour(Telnet.Red)}");
				sb.AppendLine(
					$"Last Arrived: {(DateTime.UtcNow - patrol.LastArrivedTime).Humanize(2, actor.Account.Culture, minUnit: Humanizer.TimeUnit.Second).ColourValue()}");
				sb.AppendLine(
					$"Active Target: {patrol.ActiveEnforcementTarget?.HowSeen(actor) ?? "Noone".ColourCharacter()}");
				sb.AppendLine(
					$"Active Crime: {patrol.ActiveEnforcementCrime?.DescribeCrime(actor).ColourValue() ?? "Nothing".Colour(Telnet.Red)}");
			}

			foreach (var patrol in inactivePatrols)
			{
				sb.AppendLine();
				sb.AppendLine($"Inactive Patrol Route \"{patrol.Name}\" (#{patrol.Id.ToStringN0(actor)})".GetLineWithTitleInner(actor, Telnet.Magenta, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"Strategy: {patrol.PatrolStrategy.Name.ColourValue()}");
				sb.AppendLine($"Required Enforcers: {patrol.PatrollerNumbers.Select(x => $"{x.Value.ToStringN0(actor)} {x.Key.Name.Pluralise(x.Value != 1).ColourName()}").ListToString()}");
				sb.AppendLine($"Is Ready: {patrol.IsReady.ToColouredString()}");
				sb.AppendLine($"Should Begin: {patrol.ShouldBeginPatrol().ToColouredString()}");
			}
		}
		
		actor.OutputHandler.Send(sb.ToString());
	}
}