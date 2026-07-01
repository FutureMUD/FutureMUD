using Humanizer;
using MudSharp.Accounts;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PatrolStrategies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	#3legal roster [<legal authority>|<zone>]#0 - shows the enforcer roster and patrol eligibility
	#3legal status [<legal authority>]#0 - diagnoses legal authority setup, patrol coverage and required equipment
	#3legal patrols [<legal authority>]#0 - shows all patrol routes
	#3legal cancelpatrol <legal authority> <patrol>#0 - cancels an active patrol

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
        StringStack ss = new(command.RemoveFirstWord());
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
            case "roster":
            case "enforcers":
                LegalAuthorityEnforcerRoster(actor, ss);
                return;
            case "status":
                LegalAuthorityStatus(actor, ss);
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
            case "cancelpatrol":
                LegalCancelPatrol(actor, ss);
                return;
        }

        actor.OutputHandler.Send(LegalAuthorityHelpText.SubstituteANSIColour());
    }

    private static void LegalAuthorityEnforcerRoster(ICharacter actor, StringStack ss)
    {
        ILegalAuthority editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
        ShowEnforcerRoster(actor, ss, editing);
    }

    private const string LegalStatusHelpText = @"The #3legalstatus#0 command shows setup diagnostics for legal authorities. Administrators can see all authorities and detailed room/route references. PC enforcers can see status for authorities they currently serve.

The syntax is:

	#3legalstatus#0 - shows your legal authorities, or all authorities if you are an admin
	#3legalstatus <legal authority>#0 - shows one legal authority";

    [PlayerCommand("LegalStatus", "legalstatus", "lawstatus")]
    [HelpInfo("legalstatus", LegalStatusHelpText, AutoHelp.HelpArgOrNoArg)]
    protected static void LegalStatus(ICharacter actor, string command)
    {
        LegalAuthorityStatus(actor, new StringStack(command.RemoveFirstWord()), null);
    }

    private sealed record LegalSetupIssue(string Severity, string Area, string Detail);

    private static void LegalAuthorityStatus(ICharacter actor, StringStack ss)
    {
        ILegalAuthority editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
        LegalAuthorityStatus(actor, ss, editing);
    }

    private static void LegalAuthorityStatus(ICharacter actor, StringStack ss, ILegalAuthority defaultAuthority)
    {
        List<ILegalAuthority> authorities = GetLegalAuthoritiesForLegalStatus(actor, ss, defaultAuthority);
        if (authorities is null)
        {
            return;
        }

        if (!authorities.Any())
        {
            actor.OutputHandler.Send(actor.IsAdministrator()
                ? "There are no legal authorities configured."
                : "You are not currently an enforcer for any legal authority.");
            return;
        }

        actor.OutputHandler.Send(BuildLegalStatusReport(actor, authorities, actor.IsAdministrator()));
    }

    private static List<ILegalAuthority> GetLegalAuthoritiesForLegalStatus(ICharacter actor, StringStack ss,
        ILegalAuthority defaultAuthority)
    {
        if (ss.IsFinished)
        {
            if (defaultAuthority is not null && actor.IsAdministrator())
            {
                return new List<ILegalAuthority> { defaultAuthority };
            }

            return actor.IsAdministrator()
                ? actor.Gameworld.LegalAuthorities.ToList()
                : actor.Gameworld.LegalAuthorities.Where(x => x.GetEnforcementAuthority(actor) is not null).ToList();
        }

        string targetText = ss.SafeRemainingArgument;
        ILegalAuthority legal = actor.Gameworld.LegalAuthorities.GetByIdOrName(targetText);
        if (legal is null)
        {
            actor.OutputHandler.Send($"There is no legal authority identified by {targetText.ColourCommand()}.");
            return null;
        }

        if (!actor.IsAdministrator() && legal.GetEnforcementAuthority(actor) is null)
        {
            actor.OutputHandler.Send($"You are not an enforcer for {legal.Name.ColourName()}.");
            return null;
        }

        return new List<ILegalAuthority> { legal };
    }

    private static string BuildLegalStatusReport(ICharacter actor, IEnumerable<ILegalAuthority> authorities,
        bool detailed)
    {
        StringBuilder sb = new();
        foreach (ILegalAuthority legal in authorities)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            List<ICharacter> freeEnforcers = FreePatrolEnforcers(actor, legal);
            CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts = FreeEnforcerCounts(legal, freeEnforcers);
            List<LegalSetupIssue> issues = LegalSetupIssues(actor, legal, freeEnforcers, enforcerCounts, detailed).ToList();

            sb.AppendLine($"Legal Status for {legal.Name} Authority".GetLineWithTitleInner(actor, Telnet.BoldBlue, Telnet.BoldWhite));
            sb.AppendLine();
            sb.AppendLine($"Overall: {(issues.Any(x => x.Severity == "Problem") ? "Problems Found".ColourError() : issues.Any() ? "Warnings Found".Colour(Telnet.Yellow) : "No Issues Detected".ColourValue())}");
            sb.AppendLine($"Enforcement Zones: {legal.EnforcementZones.Count().ToStringN0Colour(actor)}");
            sb.AppendLine($"Patrol Routes: {legal.PatrolRoutes.Count().ToStringN0Colour(actor)}");
            sb.AppendLine($"Active Patrols: {legal.Patrols.Count().ToStringN0Colour(actor)}");
            sb.AppendLine($"Free Patrol Enforcers: {freeEnforcers.Count.ToStringN0Colour(actor)}");
            sb.AppendLine();

            if (!issues.Any())
            {
                sb.AppendLine("No setup issues were detected for this legal authority.".ColourValue());
                continue;
            }

            sb.AppendLine(StringUtilities.GetTextTable(
                from issue in issues
                select new List<string>
                {
                    FormatLegalStatusSeverity(issue.Severity),
                    issue.Area.ColourName(),
                    issue.Detail
                },
                new List<string>
                {
                    "Severity",
                    "Area",
                    "Issue"
                },
                actor,
                Telnet.Magenta,
                2,
                true));
        }

        return sb.ToString();
    }

    private static string FormatLegalStatusSeverity(string severity)
    {
        return severity switch
        {
            "Problem" => severity.ColourError(),
            "Warning" => severity.Colour(Telnet.Yellow),
            _ => severity.ColourValue()
        };
    }

    private static List<ICharacter> FreePatrolEnforcers(ICharacter actor, ILegalAuthority legal)
    {
        return actor.Gameworld.NPCs
                    .Where(x => IsInPatrolPool(legal, x))
                    .ToList();
    }

    private static CollectionDictionary<IEnforcementAuthority, ICharacter> FreeEnforcerCounts(ILegalAuthority legal,
        IEnumerable<ICharacter> freeEnforcers)
    {
        CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts = new();
        foreach (IGrouping<IEnforcementAuthority, ICharacter> group in freeEnforcers.GroupBy(x => legal.GetEnforcementAuthority(x)))
        {
            enforcerCounts.AddRange(group.Key, group);
        }

        return enforcerCounts;
    }

    private static IEnumerable<LegalSetupIssue> LegalSetupIssues(ICharacter actor, ILegalAuthority legal,
        List<ICharacter> freeEnforcers, CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts,
        bool detailed)
    {
        foreach (LegalSetupIssue issue in CoreLegalSetupIssues(actor, legal, freeEnforcers, detailed))
        {
            yield return issue;
        }

        foreach (LegalSetupIssue issue in PatrolCoverageIssues(actor, legal, enforcerCounts, detailed))
        {
            yield return issue;
        }

        foreach (LegalSetupIssue issue in ExecutionEquipmentIssues(actor, legal, detailed))
        {
            yield return issue;
        }

        foreach (LegalSetupIssue issue in DoorDutyKeyIssues(actor, legal, detailed))
        {
            yield return issue;
        }
    }

    private static IEnumerable<LegalSetupIssue> CoreLegalSetupIssues(ICharacter actor, ILegalAuthority legal,
        List<ICharacter> freeEnforcers, bool detailed)
    {
        if (!legal.EnforcementZones.Any())
        {
            yield return new LegalSetupIssue("Problem", "Core Setup", "No enforcement zones are configured.");
        }

        if (!legal.EnforcementAuthorities.Any())
        {
            yield return new LegalSetupIssue("Problem", "Core Setup", "No enforcement authorities are configured.");
        }

        if (!legal.LegalClasses.Any())
        {
            yield return new LegalSetupIssue("Warning", "Core Setup", "No legal classes are configured.");
        }

        if (!legal.Laws.Any())
        {
            yield return new LegalSetupIssue("Warning", "Core Setup", "No laws are configured.");
        }

        List<string> missingLocations = new();
        if (legal.PreparingLocation is null)
        {
            missingLocations.Add("preparation room");
        }

        if (legal.MarshallingLocation is null)
        {
            missingLocations.Add("marshalling room");
        }

        if (legal.EnforcerStowingLocation is null)
        {
            missingLocations.Add("enforcer stow room");
        }

        if (legal.PrisonLocation is null)
        {
            missingLocations.Add("prison administration room");
        }

        if (legal.PrisonerBelongingsStorageLocation is null)
        {
            missingLocations.Add("prisoner belongings room");
        }

        if (legal.CourtLocation is null)
        {
            missingLocations.Add("courtroom");
        }

        if (!legal.CellLocations.Any())
        {
            missingLocations.Add("holding cells");
        }

        if (missingLocations.Any())
        {
            yield return new LegalSetupIssue("Problem", "Core Setup",
                $"Missing {missingLocations.ListToString()}.");
        }

        if (!freeEnforcers.Any())
        {
            yield return new LegalSetupIssue("Problem", "Enforcers",
                "No NPC enforcers are currently eligible for the patrol pool.");
        }
    }

    private static IEnumerable<LegalSetupIssue> PatrolCoverageIssues(ICharacter actor, ILegalAuthority legal,
        CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts, bool detailed)
    {
        if (!legal.PatrolRoutes.Any())
        {
            yield return new LegalSetupIssue("Problem", "Patrols", "No patrol routes are configured.");
            yield break;
        }

        if (!legal.PatrolRoutes.Any(x => x.IsReady))
        {
            yield return new LegalSetupIssue("Problem", "Patrols", "No patrol routes are marked ready.");
        }

        bool hasConvictingAuthority = legal.EnforcementAuthorities.Any(x => x.CanConvict);
        if (hasConvictingAuthority && !HasPatrolStrategy(legal, "Judge"))
        {
            yield return new LegalSetupIssue("Problem", "Trial Patrols",
                "No Judge patrol route is configured, so NPC trials cannot be judged.");
        }

        if (hasConvictingAuthority && !HasPatrolStrategy(legal, "Sheriff"))
        {
            yield return new LegalSetupIssue("Warning", "Trial Patrols",
                "No Sheriff patrol route is configured to fetch remand prisoners for trial.");
        }

        if (hasConvictingAuthority && !HasPatrolStrategy(legal, "Prosecutor"))
        {
            yield return new LegalSetupIssue("Warning", "Trial Patrols",
                "No Prosecutor patrol route is configured for automated trial prosecution.");
        }

        if (LawsCanSentenceExecution(legal) && !HasPatrolStrategy(legal, "ExecutionPatrol"))
        {
            yield return new LegalSetupIssue("Problem", "Execution Patrols",
                "At least one law can sentence execution, but no ExecutionPatrol route is configured.");
        }

        foreach (IPatrolRoute route in legal.PatrolRoutes)
        {
            string reason = route.WhyCannotBeginPatrol();
            if (IsSetupBlockingPatrolReason(reason))
            {
                yield return new LegalSetupIssue("Problem", "Patrol Route",
                    $"{RouteLabel(route, actor, detailed)} cannot begin: {reason}.");
            }

            string staffingReason = WhyCannotStaffPatrolRoute(route, enforcerCounts, actor, detailed);
            if (!string.IsNullOrEmpty(staffingReason))
            {
                yield return new LegalSetupIssue("Problem", "Patrol Staffing", staffingReason);
            }
        }
    }

    private static bool HasPatrolStrategy(ILegalAuthority legal, string strategyName)
    {
        return legal.PatrolRoutes.Any(x => x.PatrolStrategy.Name.Equals(strategyName, StringComparison.OrdinalIgnoreCase));
    }

    private static bool LawsCanSentenceExecution(ILegalAuthority legal)
    {
        return legal.Laws.Any(x => x.PunishmentStrategy.SaveResult().Contains("type=\"execute\"", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsSetupBlockingPatrolReason(string reason)
    {
        if (string.IsNullOrEmpty(reason))
        {
            return false;
        }

        return !reason.Contains("wait for", StringComparison.OrdinalIgnoreCase) &&
               !reason.Contains("current time of day", StringComparison.OrdinalIgnoreCase) &&
               !reason.Contains("no due condemned", StringComparison.OrdinalIgnoreCase) &&
               !reason.Contains("already an active", StringComparison.OrdinalIgnoreCase);
    }

    private static string WhyCannotStaffPatrolRoute(IPatrolRoute route,
        CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts, ICharacter actor, bool detailed)
    {
        if (!route.IsReady || !route.PatrollerNumbers.Any())
        {
            return string.Empty;
        }

        foreach (KeyValuePair<IEnforcementAuthority, int> requirement in route.PatrollerNumbers)
        {
            List<ICharacter> pool = enforcerCounts[requirement.Key].ToList();
            if (pool.Count < requirement.Value)
            {
                return $"{RouteLabel(route, actor, detailed)} requires {requirement.Value.ToStringN0(actor)} {requirement.Key.Name.Pluralise(requirement.Value != 1).ColourName()} enforcers, but only {pool.Count.ToStringN0Colour(actor)} are free.";
            }

            int selectable = route.PatrolStrategy.SelectEnforcers(route, pool, requirement.Value).Count();
            if (selectable < requirement.Value)
            {
                return $"{RouteLabel(route, actor, detailed)} requires {requirement.Value.ToStringN0(actor)} selectable {requirement.Key.Name.Pluralise(requirement.Value != 1).ColourName()} enforcers for the {route.PatrolStrategy.Name.ColourName()} strategy, but only {selectable.ToStringN0Colour(actor)} match the strategy selection rules.";
            }
        }

        return string.Empty;
    }

    private static string RouteLabel(IPatrolRoute route, ICharacter actor, bool detailed)
    {
        return detailed
            ? $"{route.Name.ColourName()} (#{route.Id.ToStringN0(actor)})"
            : route.Name.ColourName();
    }

    private static IEnumerable<LegalSetupIssue> ExecutionEquipmentIssues(ICharacter actor, ILegalAuthority legal,
        bool detailed)
    {
        foreach (IPatrolRoute route in legal.PatrolRoutes.Where(x => x.PatrolStrategy is ExecutionPatrolStrategy))
        {
            ExecutionPatrolStrategy strategy = (ExecutionPatrolStrategy)route.PatrolStrategy;
            ICell equipment = strategy.EquipmentLocationId > 0
                ? actor.Gameworld.Cells.Get(strategy.EquipmentLocationId)
                : legal.PreparingLocation;
            ICell executionLocation = route.PatrolNodes.FirstOrDefault();
            string routeLabel = RouteLabel(route, actor, detailed);

            if (equipment is null)
            {
                yield return new LegalSetupIssue("Problem", "Execution Equipment",
                    $"{routeLabel} has no valid equipment/preparation room.");
                continue;
            }

            switch (strategy.Method)
            {
                case ExecutionPatrolExecutionMethod.CoupDeGraceWithWeapon:
                    if (!ItemsInCell(equipment, actor).Any(IsExecutionMeleeWeapon))
                    {
                        yield return new LegalSetupIssue("Problem", "Execution Equipment",
                            $"{routeLabel} uses coup de grace but no suitable melee execution weapon is stocked{LocationSuffix(equipment, actor, detailed)}.");
                    }

                    break;
                case ExecutionPatrolExecutionMethod.AdministerDrug:
                    if (strategy.DrugId <= 0 || actor.Gameworld.Drugs.Get(strategy.DrugId) is null)
                    {
                        yield return new LegalSetupIssue("Problem", "Execution Equipment",
                            $"{routeLabel} uses administered drugs but has no valid drug configured.");
                    }

                    if (!ComponentsInCell<IInject>(equipment, actor).Any())
                    {
                        yield return new LegalSetupIssue("Problem", "Execution Equipment",
                            $"{routeLabel} uses administered drugs but no injector item is stocked{LocationSuffix(equipment, actor, detailed)}.");
                    }

                    break;
                case ExecutionPatrolExecutionMethod.FiringSquad:
                    if (!ComponentsInCell<IRangedWeapon>(equipment, actor).Any())
                    {
                        yield return new LegalSetupIssue("Problem", "Execution Equipment",
                            $"{routeLabel} uses firing squad but no ranged weapons are stocked{LocationSuffix(equipment, actor, detailed)}.");
                    }

                    break;
            }

            if (!ComponentsInCell<IRestraint>(equipment, actor).Concat(ComponentsInCell<IRestraint>(executionLocation, actor)).Any())
            {
                yield return new LegalSetupIssue("Warning", "Execution Equipment",
                    $"{routeLabel} has no restraint items stocked in the equipment or execution room; guards will need a helpless or submitted prisoner to proceed.");
            }

            if (strategy.RequireKeysForRetrieval)
            {
                foreach (LegalSetupIssue issue in ExecutionRetrievalKeyIssues(actor, legal, route, equipment, detailed))
                {
                    yield return issue;
                }
            }
        }
    }

    private static IEnumerable<LegalSetupIssue> ExecutionRetrievalKeyIssues(ICharacter actor, ILegalAuthority legal,
        IPatrolRoute route, ICell equipment, bool detailed)
    {
        List<ICell> prisonerCells = legal.CellLocations.Concat(legal.JailLocations).Distinct().ToList();
        if (!prisonerCells.Any())
        {
            yield return new LegalSetupIssue("Problem", "Execution Keys",
                $"{RouteLabel(route, actor, detailed)} requires retrieval keys but no prisoner cell or jail locations are configured.");
            yield break;
        }

        List<ILock> locks = DoorLocksForCells(prisonerCells)
                           .Concat(DoorLocksForPaths(equipment, prisonerCells))
                           .Distinct()
                           .ToList();
        if (!locks.Any())
        {
            yield break;
        }

        List<ILock> missingLocks = LocksMissingKeys(locks, KeysInCell(equipment, actor)).ToList();
        if (missingLocks.Any())
        {
            yield return new LegalSetupIssue("Problem", "Execution Keys",
                $"{RouteLabel(route, actor, detailed)} requires retrieval keys, but {missingLocks.Count.ToStringN0Colour(actor)} prison-route lock{(missingLocks.Count == 1 ? "" : "s")} lack stocked matching keys{LocationSuffix(equipment, actor, detailed)}.");
        }
    }

    private static IEnumerable<LegalSetupIssue> DoorDutyKeyIssues(ICharacter actor, ILegalAuthority legal,
        bool detailed)
    {
        foreach (IPatrolRoute route in legal.PatrolRoutes.Where(x => x.PatrolStrategy.Name.Equals("DoorDuties", StringComparison.OrdinalIgnoreCase)))
        {
            ICell dutyLocation = route.PatrolNodes.FirstOrDefault();
            if (dutyLocation is null || legal.PreparingLocation is null)
            {
                continue;
            }

            List<ILock> locks = DoorLocksForCells(new[] { dutyLocation }).ToList();
            if (!locks.Any())
            {
                yield return new LegalSetupIssue("Info", "Door Duties",
                    $"{RouteLabel(route, actor, detailed)} is configured for door duties, but the duty location has no doors with locks.");
                continue;
            }

            List<ILock> missingLocks = LocksMissingKeys(locks, KeysInCell(legal.PreparingLocation, actor)).ToList();
            if (missingLocks.Any())
            {
                yield return new LegalSetupIssue("Problem", "Door Duties",
                    $"{RouteLabel(route, actor, detailed)} needs keys for {missingLocks.Count.ToStringN0Colour(actor)} duty-door lock{(missingLocks.Count == 1 ? "" : "s")} that are not stocked in the preparation room.");
            }
        }
    }

    private static IEnumerable<IGameItem> ItemsInCell(ICell cell, ICharacter accessor)
    {
        return cell?.GameItems.SelectMany(x => AccessibleStockItems(x, accessor)).Distinct() ??
               Enumerable.Empty<IGameItem>();
    }

    private static IEnumerable<IGameItem> AccessibleStockItems(IGameItem item, ICharacter accessor)
    {
        yield return item;

        IContainer container = item.GetItemType<IContainer>();
        if (container is null)
        {
            yield break;
        }

        IOpenable openable = item.GetItemType<IOpenable>();
        if (openable is not null && !openable.IsOpen && (accessor is null || !openable.CanOpen(accessor.Body)))
        {
            yield break;
        }

        foreach (IGameItem contained in container.Contents.SelectMany(x => AccessibleStockItems(x, accessor)))
        {
            yield return contained;
        }
    }

    private static IEnumerable<T> ComponentsInCell<T>(ICell cell, ICharacter accessor) where T : class, IGameItemComponent
    {
        return ItemsInCell(cell, accessor).SelectNotNull(x => x.GetItemType<T>());
    }

    private static IEnumerable<IKey> KeysInCell(ICell cell, ICharacter accessor)
    {
        return ComponentsInCell<IKey>(cell, accessor);
    }

    private static bool IsExecutionMeleeWeapon(IGameItem item)
    {
        return item.GetItemType<IMeleeWeapon>()?.WeaponType.Attacks.OfType<IFixedBodypartWeaponAttack>().Any() == true;
    }

    private static IEnumerable<ILock> DoorLocksForCells(IEnumerable<ICell> cells)
    {
        return cells
               .Where(x => x is not null)
               .SelectMany(x => x.ExitsFor(null, true))
               .Where(x => x.Exit.Door?.Locks.Any() == true)
               .DistinctBy(x => x.Exit)
               .SelectMany(x => x.Exit.Door.Locks)
               .Distinct();
    }

    private static IEnumerable<ILock> DoorLocksForPaths(ICell origin, IEnumerable<ICell> destinations)
    {
        if (origin is null)
        {
            return Enumerable.Empty<ILock>();
        }

        return destinations
               .Where(x => x is not null)
               .SelectMany(x => origin.PathBetween(x, 50, PathSearch.IgnorePresenceOfDoors))
               .Where(x => x.Exit.Door?.Locks.Any() == true)
               .DistinctBy(x => x.Exit)
               .SelectMany(x => x.Exit.Door.Locks)
               .Distinct();
    }

    private static IEnumerable<ILock> LocksMissingKeys(IEnumerable<ILock> locks, IEnumerable<IKey> keys)
    {
        List<IKey> keyList = keys.ToList();
        return locks.Where(x => keyList.All(y => !y.Unlocks(x.LockType, x.Pattern)));
    }

    private static string LocationSuffix(ICell location, ICharacter actor, bool detailed)
    {
        return detailed && location is not null
            ? $" in {location.GetFriendlyReference(actor)}"
            : string.Empty;
    }

    private static List<ILegalAuthority> GetLegalAuthoritiesForEnforcerRoster(ICharacter actor, StringStack ss,
        ILegalAuthority defaultAuthority)
    {
        if (ss.IsFinished)
        {
            if (defaultAuthority is not null)
            {
                return new List<ILegalAuthority> { defaultAuthority };
            }

            return actor.Gameworld.LegalAuthorities.ToList();
        }

        string targetText = ss.SafeRemainingArgument;
        ILegalAuthority legal = actor.Gameworld.LegalAuthorities.GetByIdOrName(targetText);
        if (legal is not null)
        {
            return new List<ILegalAuthority> { legal };
        }

        IZone zone = actor.Gameworld.Zones.GetByIdOrName(targetText);
        if (zone is null)
        {
            actor.OutputHandler.Send(
                $"The text {targetText.ColourCommand()} is not a valid legal authority or enforcement zone.");
            return null;
        }

        List<ILegalAuthority> authorities = actor.Gameworld.LegalAuthorities
                                                 .Where(x => x.EnforcementZones.Contains(zone))
                                                 .ToList();
        if (!authorities.Any())
        {
            actor.OutputHandler.Send(
                $"The zone {zone.Name.ColourName()} is not an enforcement zone for any legal authority.");
            return null;
        }

        return authorities;
    }

    private static bool HasEnforcerAI(ICharacter enforcer)
    {
        return enforcer is INPC npc && npc.AIs.Any(x => x is EnforcerAI);
    }

    private static IPatrol ActivePatrolForEnforcer(ILegalAuthority legal, ICharacter enforcer)
    {
        return legal.Patrols.FirstOrDefault(x => x.PatrolMembers.ContainsPhysicalInstance(enforcer));
    }

    private static string WhyCannotJoinPatrolPool(ILegalAuthority legal, ICharacter enforcer)
    {
        if (enforcer is not INPC)
        {
            return "not an NPC; automatic patrols only use NPCs";
        }

        if (!HasEnforcerAI(enforcer))
        {
            return "does not have an Enforcer AI";
        }

        if (!enforcer.AffectedBy<EnforcerEffect>(legal))
        {
            return "not currently on duty for this authority";
        }

        if (legal.GetEnforcementAuthority(enforcer) is null)
        {
            return "does not currently match any enforcement authority";
        }

        IPatrol activePatrol = ActivePatrolForEnforcer(legal, enforcer);
        if (activePatrol is not null)
        {
            return $"already assigned to {activePatrol.PatrolRoute.Name} ({activePatrol.PatrolPhase.DescribeEnum()})";
        }

        return string.Empty;
    }

    private static bool IsInPatrolPool(ILegalAuthority legal, ICharacter enforcer)
    {
        return string.IsNullOrEmpty(WhyCannotJoinPatrolPool(legal, enforcer));
    }

    private static IEnumerable<ICharacter> EnforcerRosterCandidates(ICharacter actor, ILegalAuthority legal)
    {
        List<ICharacter> activePatrolMembers = legal.Patrols
                                                   .SelectMany(x => x.PatrolMembers)
                                                   .DistinctPhysicalInstances()
                                                   .ToList();

        return actor.Gameworld.Actors
                    .Where(x =>
                        x.AffectedBy<EnforcerEffect>(legal) ||
                        activePatrolMembers.ContainsPhysicalInstance(x) ||
                        legal.GetEnforcementAuthority(x) is not null)
                    .DistinctPhysicalInstances();
    }

    private static string DescribeEnforcerRosterAuthority(ILegalAuthority legal, ICharacter enforcer)
    {
        IEnforcementAuthority currentAuthority = legal.GetEnforcementAuthority(enforcer);
        if (currentAuthority is not null)
        {
            return currentAuthority.Name.ColourName();
        }

        EnforcerEffect effect = enforcer.EffectsOfType<EnforcerEffect>(x => x.LegalAuthority == legal).FirstOrDefault();
        return effect?.EnforcementAuthority?.Name.ColourError() ?? "None".ColourError();
    }

    private static string DescribeEnforcerRosterPatrol(ILegalAuthority legal, ICharacter enforcer)
    {
        IPatrol activePatrol = ActivePatrolForEnforcer(legal, enforcer);
        if (activePatrol is null)
        {
            return "None";
        }

        return $"{activePatrol.PatrolRoute.Name.ColourName()} ({activePatrol.PatrolPhase.DescribeEnum().ColourValue()})";
    }

    private static void ShowEnforcerRoster(ICharacter actor, StringStack ss, ILegalAuthority defaultAuthority = null)
    {
        List<ILegalAuthority> authorities = GetLegalAuthoritiesForEnforcerRoster(actor, ss, defaultAuthority);
        if (authorities is null)
        {
            return;
        }

        StringBuilder sb = new();
        foreach (ILegalAuthority legal in authorities)
        {
            List<(ICharacter Enforcer, bool OnDuty, bool InPool, string Authority, string Location, string Patrol, string Status)> rows =
                EnforcerRosterCandidates(actor, legal)
                    .Select(x =>
                    {
                        string whyNot = WhyCannotJoinPatrolPool(legal, x);
                        bool inPool = string.IsNullOrEmpty(whyNot);
                        return (
                            Enforcer: x,
                            OnDuty: x.AffectedBy<EnforcerEffect>(legal),
                            InPool: inPool,
                            Authority: DescribeEnforcerRosterAuthority(legal, x),
                            Location: x.Location?.GetFriendlyReference(actor) ?? "Nowhere".ColourError(),
                            Patrol: DescribeEnforcerRosterPatrol(legal, x),
                            Status: inPool ? "eligible for patrol pool".ColourValue() : whyNot.ColourError()
                        );
                    })
                    .OrderByDescending(x => x.InPool)
                    .ThenByDescending(x => x.OnDuty)
                    .ThenBy(x => x.Authority.StripANSIColour())
                    .ThenBy(x => x.Enforcer.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee).StripANSIColour())
                    .ToList();

            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine($"Enforcer Roster for {legal.Name} Authority".GetLineWithTitleInner(actor, Telnet.BoldBlue, Telnet.BoldWhite));
            sb.AppendLine();
            sb.AppendLine($"Enforcement Zones: {legal.EnforcementZones.Select(x => x.Name.ColourName()).DefaultIfEmpty("None".ColourError()).ListToString()}");
            sb.AppendLine($"Configured Authorities: {legal.EnforcementAuthorities.Select(x => x.Name.ColourName()).DefaultIfEmpty("None".ColourError()).ListToString()}");
            sb.AppendLine($"On Duty: {rows.Count(x => x.OnDuty).ToStringN0Colour(actor)}");
            sb.AppendLine($"In Patrol Pool: {rows.Count(x => x.InPool).ToStringN0Colour(actor)}");
            sb.AppendLine($"Assigned To Patrols: {rows.Count(x => x.Patrol != "None").ToStringN0Colour(actor)}");
            sb.AppendLine();
            sb.AppendLine(StringUtilities.GetTextTable(
                from item in rows
                select new List<string>
                {
                    item.Enforcer.Id.ToStringN0(actor),
                    item.Enforcer.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee),
                    item.OnDuty.ToColouredString(),
                    item.InPool.ToColouredString(),
                    item.Authority
                },
                new List<string>
                {
                    "Id",
                    "Enforcer",
                    "Duty",
                    "Pool",
                    "Authority"
                },
                actor,
                Telnet.Magenta,
                1,
                true));
            sb.AppendLine();
            sb.AppendLine("Patrol Pool Details:");
            foreach (var item in rows)
            {
                sb.AppendLine(
                    $"\t#{item.Enforcer.Id.ToStringN0(actor)} {item.Enforcer.PersonalName.GetName(NameStyle.FullName).ColourName()}: Location {item.Location}; Patrol {item.Patrol}; Status {item.Status}"
                        .Wrap(actor.InnerLineFormatLength, "\t"));
            }
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void LegalCancelPatrol(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which legal authority do you want to cancel a patrol in?");
            return;
        }

        ILegalAuthority legal = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.PopSpeech());
        if (legal is null)
        {
            actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} does not represent a valid legal authority.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send($"Which patrol in the {legal.Name.ColourValue()} legal authority do you want to cancel?");
            return;
        }

        IPatrol patrol = legal.Patrols.GetByIdOrName(ss.SafeRemainingArgument);
        if (patrol is null)
        {
            actor.OutputHandler.Send($"There is no patrol in the {legal.Name.ColourValue()} does not have a patrol identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
            return;
        }

        actor.OutputHandler.Send($"You abort the {patrol.Name.ColourValue()} patrol in the {legal.Name.ColourValue()} legal authority.");
        patrol.CompletePatrol();
    }

    private static void LegalAuthorityPatrols(ICharacter actor, StringStack ss)
    {
        ILegalAuthority editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
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

        List<IPatrolRoute> patrols = editing.PatrolRoutes.ToList();
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
        ILegalAuthority editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
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

        List<IEnforcementAuthority> enforcements = editing.EnforcementAuthorities.ToList();
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
        ILegalAuthority editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
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

        List<ILegalClass> classes = editing.LegalClasses.ToList();
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
        ILegalAuthority editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault()?.EditingItem;
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

        List<ILaw> laws = editing.Laws.ToList();
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
        BuilderEditingEffect<ILegalAuthority> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
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

            string name = ss.PopSpeech().TitleCase().Trim();
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

            ICurrency currency = long.TryParse(ss.PopSpeech(), out long currencyid)
                ? actor.Gameworld.Currencies.Get(currencyid)
                : actor.Gameworld.Currencies.GetByName(ss.Last);
            if (currency == null)
            {
                actor.OutputHandler.Send("There is no such currency.");
                return;
            }

            LegalAuthority newAuthority = new(name, currency, actor.Gameworld);
            actor.Gameworld.Add(newAuthority);
            actor.RemoveAllEffects<BuilderEditingEffect<ILegalAuthority>>();
            actor.AddEffect(new BuilderEditingEffect<ILegalAuthority>(actor) { EditingItem = newAuthority });
            actor.OutputHandler.Send(
                $"You create a new legal authority called {name.ColourName()} with ID {newAuthority.Id.ToString("N0", actor)}, which you are now editing.");
            return;
        }

        ILegalAuthority authority = long.TryParse(ss.PopSpeech(), out long value)
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
        BuilderEditingEffect<ILegalAuthority> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
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

        ILegalAuthority authority = long.TryParse(ss.PopSpeech(), out long value)
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
        BuilderEditingEffect<ILegalAuthority> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
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

        BuilderEditingEffect<ILegalAuthority> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
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
        BuilderEditingEffect<ILegalAuthority> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILegalAuthority>>().FirstOrDefault();
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
        StringStack ss = new(command.RemoveFirstWord());
        List<ILegalAuthority> legals = actor.Gameworld.LegalAuthorities.ToList();
        if (!ss.IsFinished)
        {
            ILegalAuthority legal = actor.Gameworld.LegalAuthorities.GetByIdOrName(ss.SafeRemainingArgument);
            if (legal == null)
            {
                actor.OutputHandler.Send("There is no such legal authority.");
                return;
            }

            legals.Clear();
            legals.Add(legal);
        }

        StringBuilder sb = new();
        foreach (ILegalAuthority legal in legals)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            sb.AppendLine($"Patrol Information for {legal.Name} Authority".GetLineWithTitleInner(actor, Telnet.BoldBlue, Telnet.BoldWhite));
            sb.AppendLine();
            List<ICharacter> enforcers =
            actor.Gameworld.NPCs
                          .Where(x =>
                              x.AffectedBy<EnforcerEffect>(legal))
                          .ToList();
            List<ICharacter> freeEnforcers = enforcers.Where(x => IsInPatrolPool(legal, x)).ToList();
            CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts = new();
            foreach (IGrouping<IEnforcementAuthority, ICharacter> group in freeEnforcers.GroupBy(x => legal.GetEnforcementAuthority(x)))
            {
                enforcerCounts.AddRange(group.Key, group);
            }

            sb.AppendLine($"Total Enforcers: {enforcers.Count.ToStringN0Colour(actor)}");
            sb.AppendLine($"Free Enforcers: {freeEnforcers.Count.ToStringN0Colour(actor)}");
            sb.AppendLine($"Enforcer Counts: {enforcerCounts.Select(x => $"{x.Value.Count.ToStringN0(actor)} {x.Key.Name.Pluralise(x.Value.Count != 1).ColourName()}").ListToString()}");
            IEnumerable<IPatrol> patrols = legal.Patrols;
            List<IPatrolRoute> inactivePatrols = legal.PatrolRoutes.Where(x => patrols.All(y => y.PatrolRoute != x)).ToList();
            foreach (IPatrol patrol in patrols)
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

            foreach (IPatrolRoute patrol in inactivePatrols)
            {
                sb.AppendLine();
                sb.AppendLine($"Inactive Patrol Route \"{patrol.Name}\" (#{patrol.Id.ToStringN0(actor)})".GetLineWithTitleInner(actor, Telnet.Magenta, Telnet.BoldWhite));
                sb.AppendLine();
                sb.AppendLine($"Strategy: {patrol.PatrolStrategy.Name.ColourValue()}");
                sb.AppendLine($"Required Enforcers: {patrol.PatrollerNumbers.Select(x => $"{x.Value.ToStringN0(actor)} {x.Key.Name.Pluralise(x.Value != 1).ColourName()}").ListToString()}");
                sb.AppendLine($"Is Ready: {patrol.IsReady.ToColouredString()}");
                string whyCannotBegin = patrol.WhyCannotBeginPatrol();
                sb.AppendLine($"Should Begin: {string.IsNullOrEmpty(whyCannotBegin).ToColouredString()}");
                if (!string.IsNullOrEmpty(whyCannotBegin))
                {
                    sb.AppendLine($"Reason: {whyCannotBegin.ColourError()}");
                }
            }
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    [PlayerCommand("ShowEnforcers", "showenforcers")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("showenforcers", @"The #3showenforcers#0 command shows the enforcer roster for a legal authority or enforcement zone, including who is on duty, who is currently eligible for the automatic patrol pool, where they are, and why any listed enforcer cannot join a patrol.

The syntax is:

	#3showenforcers#0 - shows all legal authorities
	#3showenforcers <legal authority>#0 - shows one legal authority
	#3showenforcers <zone>#0 - shows authorities that enforce a zone", AutoHelp.HelpArgOrNoArg)]
    protected static void ShowEnforcers(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        ShowEnforcerRoster(actor, ss);
    }
}
