using Dapper;
using MudSharp.Accounts;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Resources;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Editor;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.ScriptedEvents;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using System.Text.RegularExpressions;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.Commands.Modules;

internal class StorytellerModule : Module<ICharacter>
{

    private StorytellerModule()
        : base("Storyteller")
    {
        IsNecessary = true;
    }

    public static StorytellerModule Instance { get; } = new();

    public const string InstanceCommandHelp =
        @"The #3instance#0 command gives staff limited controls for secondary character instances belonging to a loaded character.

Use this command to inspect, spawn, move, retire and audit non-primary character instances. Instance owners and instances can generally be resolved by visible target, loaded character name or ID, while forms are resolved from the owner's known forms.

The #3spawn#0 options after the form can be supplied in any order. #3ai#0 chooses NPC AI for NPC identities and script AI for other identities, while #3npcai#0 and #3scriptai#0 force a specific AI mode. #3cloneinventory#0 copies inventory to the new body where supported.
Use #3audit all#0 for persisted instance row and global actor-cache diagnostics. Use #3audit <character>#0 for the loaded identity currently in memory.

The syntax is as follows:
	#3instance list <character>#0 - lists all loaded instances for a character
	#3instance spawn <character> <form> [here|room <cell id>] [persistent|temporary] [passive|focusable|ai|npcai|scriptai] [cloneinventory]#0 - spawns a secondary instance
	#3instance move <instance id|target> here|room <cell id>#0 - moves a secondary instance
	#3instance retire <instance id|target>#0 - retires a secondary instance and removes temporary rows
	#3instance despawn <instance id|target>#0 - alias for #3instance retire#0
	#3instance audit <character>#0 - audits loaded instances for a character
	#3instance audit all#0 - audits persisted instance rows and loaded actor caches";

    [PlayerCommand("Instance", "instance")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("instance", InstanceCommandHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void InstanceCommand(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(InstanceCommandHelp.SubstituteANSIColour());
            return;
        }

        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "list":
                InstanceList(actor, ss);
                return;
            case "spawn":
                InstanceSpawn(actor, ss);
                return;
            case "move":
                InstanceMove(actor, ss);
                return;
            case "retire":
            case "despawn":
                InstanceRetire(actor, ss);
                return;
            case "audit":
                InstanceAudit(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(InstanceCommandHelp.SubstituteANSIColour());
                return;
        }
    }

    private static IEnumerable<ICharacter> LoadedInstanceOwners(ICharacter actor)
    {
        return actor.Gameworld.Actors
                    .Concat(actor.Gameworld.Characters)
                    .Concat(actor.Gameworld.NPCs)
                    .Distinct();
    }

    private static ICharacter ResolveLoadedInstanceOwner(ICharacter actor, string text)
    {
        var targeted = actor.TargetActor(text);
        if (targeted?.Identity is ICharacter identity)
        {
            return identity;
        }

        return targeted ??
               actor.Gameworld.Actors.GetByIdOrName(text) ??
               actor.Gameworld.Characters.GetByIdOrName(text) ??
               actor.Gameworld.NPCs.GetByIdOrName(text) ??
               LoadedInstanceOwners(actor).GetByPersonalName(text);
    }

    private static ICharacterInstance ResolveLoadedInstance(ICharacter actor, string text)
    {
        if (long.TryParse(text, out var id))
        {
            return LoadedInstanceOwners(actor)
                   .SelectMany(x => x.Identity.Instances)
                   .FirstOrDefault(x => x.InstanceId == id || x.Id == id);
        }

        return actor.TargetActor(text) as ICharacterInstance ??
               actor.Location?.Characters.OfType<ICharacterInstance>()
                    .GetFromItemListByKeywordIncludingNames(text, actor);
    }

    private static ICharacterForm ResolveOwnedForm(ICharacter owner, string text)
    {
        if (long.TryParse(text, out var id))
        {
            return owner.Forms.FirstOrDefault(x => x.Body.Id == id);
        }

        return owner.Forms.FirstOrDefault(x => x.Alias.EqualTo(text)) ??
               owner.Forms.FirstOrDefault(x => x.Body.Prototype.Name.EqualTo(text)) ??
               owner.Forms.FirstOrDefault(x =>
                   x.Alias.StartsWith(text, StringComparison.InvariantCultureIgnoreCase) ||
                   x.Body.Prototype.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
    }

    private static bool TryPopInstanceDestination(ICharacter actor, StringStack ss, out ICell location,
        out RoomLayer layer, out string error)
    {
        location = null;
        layer = actor.RoomLayer;
        error = string.Empty;

        if (ss.IsFinished)
        {
            error = "You must specify HERE or ROOM <cell id>.";
            return false;
        }

        var keyword = ss.PopSpeech();
        if (keyword.EqualTo("here"))
        {
            location = actor.Location;
            layer = actor.RoomLayer;
            return true;
        }

        if (!keyword.EqualTo("room"))
        {
            error = "You must specify HERE or ROOM <cell id>.";
            return false;
        }

        if (ss.IsFinished)
        {
            error = "Which room cell id do you want to use?";
            return false;
        }

        location = RoomBuilderModule.LookupCell(actor.Gameworld, ss.PopSpeech());
        layer = RoomLayer.GroundLevel;
        if (location is not null)
        {
            return true;
        }

        error = "There is no such cell.";
        return false;
    }

    private static void InstanceList(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which loaded character do you want to list instances for?");
            return;
        }

        var target = ResolveLoadedInstanceOwner(actor, ss.SafeRemainingArgument);
        if (target is null)
        {
            actor.OutputHandler.Send("There is no such loaded character.");
            return;
        }

        string DetailsFor(ICharacterInstance instance, IArtificialIntelligenceControlledCharacter aiActor)
        {
            if (CharacterInstanceMetadata.TryGetAstralProjectionMetadata(instance.InstanceEffectData,
                    out var projection))
            {
                var plane = actor.Gameworld.Planes.Get(projection.PlaneId);
                return
                    $"Anchor #{projection.AnchorInstanceId.ToString("N0", actor)} / {(plane?.Name ?? $"Plane #{projection.PlaneId.ToString("N0", actor)}")} / {projection.AnchorPolicy.DescribeEnum()}";
            }

            if (CharacterInstanceMetadata.TryGetMagicalCopyMetadata(instance.InstanceEffectData, out var copy))
            {
                var plane = actor.Gameworld.Planes.Get(copy.PlaneId);
                var presence = copy.Intangible
                    ? $"Intangible {(plane?.Name ?? $"Plane #{copy.PlaneId.ToString("N0", actor)}")}"
                    : "Tangible";
                return
                    $"Anchor #{copy.AnchorInstanceId.ToString("N0", actor)} / Spell #{copy.SourceSpellId.ToString("N0", actor)} / {copy.FormKey.ColourCommand()} / {presence} / Focus {copy.PlayerFocusable.ToColouredString()} / {copy.PersistencePolicy.DescribeEnum()}";
            }

            if (CharacterInstanceMetadata.TryGetPhysicalCloneMetadata(instance.InstanceEffectData, out var clone))
            {
                return
                    $"Anchor #{clone.AnchorInstanceId.ToString("N0", actor)} / Spell #{clone.SourceSpellId.ToString("N0", actor)} / {clone.FormKey.ColourCommand()} / Clone body #{clone.CloneBodyId.ToString("N0", actor)} / Focus {clone.PlayerFocusable.ToColouredString()} / {clone.PersistencePolicy.DescribeEnum()}";
            }

            return aiActor is null
                ? string.Empty
                : $"{aiActor.AIs.Count().ToString("N0", actor)} AI / {(aiActor.CharacterController is null ? "Detached" : "Controlled")}";
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from instance in target.Identity.Instances
            let form = target.Identity.Forms.FirstOrDefault(x => ReferenceEquals(x.Body, instance.Body))
            let aiActor = instance as IArtificialIntelligenceControlledCharacter
            select new[]
            {
                instance.InstanceId.ToString("N0", actor),
                (target.Identity.FocusedInstance?.InstanceId == instance.InstanceId).ToColouredString(),
                instance.IsPrimaryInstance.ToColouredString(),
                instance.InstanceKind.DescribeEnum(),
                $"{form?.Alias ?? instance.Body.Prototype.Name} (#{instance.Body.Id.ToString("N0", actor)})",
                instance.Location is null
                    ? "None"
                    : $"{instance.Location.HowSeen(actor)} (#{instance.Location.Id.ToString("N0", actor)})",
                instance.RoomLayer.DescribeEnum(),
                $"{instance.State.DescribeEnum()} / {instance.Status.DescribeEnum()}",
                instance.ControlPolicy.DescribeEnum(),
                DetailsFor(instance, aiActor),
                instance.DeathPolicy.DescribeEnum(),
                instance.PersistencePolicy.DescribeEnum()
            },
            new[]
            {
                "Instance",
                "Focused",
                "Primary",
                "Kind",
                "Form",
                "Location",
                "Layer",
                "State",
                "Control",
                "Details",
                "Death",
                "Persistence"
            },
            actor.LineFormatLength,
            colour: Telnet.Cyan,
            unicodeTable: actor.Account.UseUnicode
        ));
    }

    private static void InstanceSpawn(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which loaded character do you want to spawn a secondary instance for?");
            return;
        }

        var targetText = ss.PopSpeech();
        var target = ResolveLoadedInstanceOwner(actor, targetText);
        if (target is null)
        {
            actor.OutputHandler.Send("There is no such loaded character.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which owned form do you want to spawn?");
            return;
        }

        var form = ResolveOwnedForm(target, ss.PopSpeech());
        if (form is null)
        {
            actor.OutputHandler.Send("That character has no such owned form.");
            return;
        }

        var location = actor.Location;
        var layer = actor.RoomLayer;
        var persistence = CharacterInstancePersistencePolicy.DespawnOnReboot;
        var mode = SecondaryCharacterInstanceSpawnMode.Passive;
        var cloneInventory = false;
        while (!ss.IsFinished)
        {
            switch (ss.PeekSpeech().ToLowerInvariant())
            {
                case "here":
                case "room":
                    if (!TryPopInstanceDestination(actor, ss, out location, out layer, out var error))
                    {
                        actor.OutputHandler.Send(error);
                        return;
                    }
                    break;
                case "persistent":
                    ss.PopSpeech();
                    persistence = CharacterInstancePersistencePolicy.Persistent;
                    break;
                case "temporary":
                    ss.PopSpeech();
                    persistence = CharacterInstancePersistencePolicy.DespawnOnReboot;
                    break;
                case "focusable":
                    ss.PopSpeech();
                    mode = SecondaryCharacterInstanceSpawnMode.PlayerFocusable;
                    break;
                case "passive":
                    ss.PopSpeech();
                    mode = SecondaryCharacterInstanceSpawnMode.Passive;
                    break;
                case "ai":
                    ss.PopSpeech();
                    mode = target is INPC || target.Identity is INPC
                        ? SecondaryCharacterInstanceSpawnMode.NpcAiControlled
                        : SecondaryCharacterInstanceSpawnMode.ScriptAiControlled;
                    break;
                case "npcai":
                    ss.PopSpeech();
                    mode = SecondaryCharacterInstanceSpawnMode.NpcAiControlled;
                    break;
                case "scriptai":
                case "script":
                    ss.PopSpeech();
                    mode = SecondaryCharacterInstanceSpawnMode.ScriptAiControlled;
                    break;
                case "cloneinventory":
                case "cloneinv":
                    ss.PopSpeech();
                    cloneInventory = true;
                    break;
                default:
                    actor.OutputHandler.Send("Invalid syntax. See #3help instance#0.".SubstituteANSIColour());
                    return;
            }
        }

        var result = CharacterInstanceService.SpawnBodyInstance(target, form, location!, layer, mode, persistence,
            cloneInventory: cloneInventory);
        if (!result.Success || result.Instance is null)
        {
            actor.OutputHandler.Send(result.Message);
            return;
        }

        actor.OutputHandler.Send(
            $"Spawned {DescribeSecondaryInstanceMode(mode)} instance #{result.Instance.InstanceId.ToString("N0", actor).ColourValue()} for {target.HowSeen(actor, true)} using form {form.Alias.ColourName()}.");
    }

    private static string DescribeSecondaryInstanceMode(SecondaryCharacterInstanceSpawnMode mode)
    {
        return mode switch
        {
            SecondaryCharacterInstanceSpawnMode.PlayerFocusable => "focusable",
            SecondaryCharacterInstanceSpawnMode.NpcAiControlled => "AI-controlled",
            SecondaryCharacterInstanceSpawnMode.ScriptAiControlled => "script-AI-controlled",
            _ => "passive"
        };
    }

    private static void InstanceMove(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which secondary instance do you want to move?");
            return;
        }

        var target = ResolveLoadedInstance(actor, ss.PopSpeech());
        if (target is null)
        {
            actor.OutputHandler.Send("There is no such loaded instance.");
            return;
        }

        if (!TryPopInstanceDestination(actor, ss, out var location, out var layer, out var error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        var result = CharacterInstanceService.Move(target, location!, layer);
        if (!result.Success)
        {
            actor.OutputHandler.Send(result.Message);
            return;
        }

        actor.OutputHandler.Send(
            $"Moved secondary instance #{target.InstanceId.ToString("N0", actor).ColourValue()} to {location!.HowSeen(actor)}.");
    }

    private static void InstanceRetire(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which secondary instance do you want to retire?");
            return;
        }

        var target = ResolveLoadedInstance(actor, ss.SafeRemainingArgument);
        if (target is null)
        {
            actor.OutputHandler.Send("There is no such loaded instance.");
            return;
        }

        if (!CharacterInstanceService.Retire(target, out var whyNot, deleteTemporaryRows: true))
        {
            actor.OutputHandler.Send(whyNot);
            return;
        }

        actor.OutputHandler.Send(
            $"Retired secondary instance #{target.InstanceId.ToString("N0", actor).ColourValue()}.");
    }

    private static void InstanceAudit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which loaded character do you want to audit, or do you want to audit ALL persisted instances?");
            return;
        }

        if (ss.SafeRemainingArgument.EqualTo("all"))
        {
            using (new FMDB())
            {
                var instances = FMDB.Context.CharacterInstances.ToList();
                var references = new CharacterInstanceReferenceSets(
                    FMDB.Context.Bodies.Select(x => x.Id).ToHashSet(),
                    FMDB.Context.Cells.Select(x => x.Id).ToHashSet());
                var persistedDiagnostics = CharacterInstanceDiagnostics.AuditPersistedInstances(instances, true,
                    references);
                var actorReferenceDiagnostics = CharacterInstanceDiagnostics.AuditPersistedActorReferences(
                    instances,
                    FMDB.Context.VehicleOccupancies.ToList(),
                    FMDB.Context.VehicleHitchLinks.ToList(),
                    FMDB.Context.ArenaSignups.ToList());
                var loadedCacheDiagnostics = CharacterInstanceDiagnostics.AuditLoadedGlobalActorCaches(
                    actor.Gameworld.Actors,
                    actor.Gameworld.Characters,
                    actor.Gameworld.NPCs,
                    actor.Gameworld.CachedActors);
                persistedDiagnostics = persistedDiagnostics
                    .Concat(actorReferenceDiagnostics)
                    .Concat(loadedCacheDiagnostics)
                    .ToList();
                actor.OutputHandler.Send(
                    $"Audited {instances.Count.ToString("N0", actor).ColourValue()} persisted character instance rows and loaded actor caches.\n\n{CharacterInstanceDiagnostics.RenderDiagnosticsTable(persistedDiagnostics, actor.LineFormatLength, actor.Account.UseUnicode)}");
            }

            return;
        }

        var target = ResolveLoadedInstanceOwner(actor, ss.SafeRemainingArgument);
        if (target is null)
        {
            actor.OutputHandler.Send("There is no such loaded character.");
            return;
        }

        var diagnostics = target.Identity.PrimaryInstance is ICharacter primary
            ? CharacterInstanceDiagnostics.AuditPrimaryInstance(primary)
            : CharacterInstanceDiagnostics.AuditLoadedIdentity(target.Identity);
        actor.OutputHandler.Send(
            $"Audited loaded instances for {target.HowSeen(actor, true)}.\n\n{CharacterInstanceDiagnostics.RenderDiagnosticsTable(diagnostics, actor.LineFormatLength, actor.Account.UseUnicode)}");
    }

    private const string SpyHelp =
        @"The #3spy#0 command toggles staff spying on one or more cells, causing you to receive output from those locations as if you were there.

Use #3spy here#0 to toggle your current cell, #3spy <cell id>#0 for a specific cell, and #3spy list#0 to review active spy locations.

Spying is stored as an effect on you. Toggling a cell you already spy on removes it, and #3spy clear#0 removes every spied cell at once.

The syntax is as follows:
	#3spy list#0 - lists all cells you are spying on
	#3spy here#0 - toggles spying on your current cell
	#3spy <cell id>#0 - toggles spying on a specific cell
	#3spy clear#0 - clears all spied cells
	#3spy none#0 - alias for #3spy clear#0
	#3spy off#0 - alias for #3spy clear#0";

    [PlayerCommand("Spy", "spy")]
    [HelpInfo("spy", SpyHelp, AutoHelp.HelpArgOrNoArg)]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    protected static void Spy(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.Peek().EqualTo("list"))
        {
            AdminSpyMaster effect = actor.EffectsOfType<AdminSpyMaster>().FirstOrDefault();
            if (effect == null)
            {
                actor.Send("You aren't currently spying on any locations.");
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine("You are currently spying on the following locations:");
            foreach (ICell location in effect.SpiedCells)
            {
                sb.AppendLine(
                    $"\t{location.HowSeen(actor)} ({location.Id}) in {location.Room.Zone.Name.Colour(Telnet.BoldWhite)}");
            }

            actor.Send(sb.ToString());
            return;
        }

        if (ss.Peek().EqualToAny("clear", "none", "off"))
        {
            AdminSpyMaster effect = actor.EffectsOfType<AdminSpyMaster>().FirstOrDefault();
            if (effect == null)
            {
                actor.Send("You aren't currently spying upon any locations.");
                return;
            }

            foreach (ICell cell in effect.SpiedCells.ToList())
            {
                effect.RemoveSpiedCell(cell);
            }

            actor.Send("All your spied upon locations have been cleared.");
            return;
        }

        ICell targetCell = ss.Peek().Equals("here")
            ? actor.Location
            : RoomBuilderModule.LookupCell(actor.Gameworld, ss.PopSpeech());

        if (targetCell == null)
        {
            actor.Send("There is no such cell to spy on.");
            return;
        }

        AdminSpyMaster smeffect = actor.EffectsOfType<AdminSpyMaster>().FirstOrDefault();
        if (smeffect == null)
        {
            smeffect = new AdminSpyMaster(actor);
            actor.AddEffect(smeffect);
        }

        if (smeffect.SpiedCells.Contains(targetCell))
        {
            smeffect.RemoveSpiedCell(targetCell);
            actor.Send($"You will no longer spy on {targetCell.HowSeen(actor)} ({targetCell.Id})");
            return;
        }

        smeffect.AddSpiedCell(targetCell);
        actor.Send($"You are now spying on {targetCell.HowSeen(actor)} ({targetCell.Id})");
    }

    private const string NewPlayerHelp =
        @"The #3newplayer#0 command adds the New Player room-description marker to a visible character.

Use this when a real new player accidentally loses their marker and asks for it back, or when staff deliberately want an NPC to present as a new player for a scene.

The command refuses targets that already have the marker. The effect expires using the normal new-player effect duration.

The syntax is as follows:
	#3newplayer <target>#0 - adds the New Player marker to a visible character";

    [PlayerCommand("NewPlayer", "newplayer")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("newplayer", NewPlayerHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void NewPlayer(ICharacter actor, string input)
    {
        ICharacter target = actor.TargetActor(input.RemoveFirstWord());
        if (target == null)
        {
            actor.Send("You don't see anyone like that.");
            return;
        }

        if (target.EffectsOfType<NewPlayer>().Any())
        {
            actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is already tagged as a new player.");
            return;
        }

        target.AddEffect(new NewPlayer(target), Effects.Concrete.NewPlayer.NewPlayerEffectLength);
        actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is now tagged as a new player.");
    }

    private const string RecentSpeechHelp =
        @"The #3recentspeech#0 command shows speech events remembered for your current room by the storyteller recent speech context system.

Use it without arguments to show all retained speech context for the room, or add relative time filters to narrow the result.

#3from#0 and #3since#0 set the older boundary, while #3to#0 and #3until#0 set the newer boundary. Timespans are relative to now and are parsed using your account culture, such as #301:00:00#0 for one hour ago. If both filters are used, the FROM value must be greater than or equal to the TO value.

The syntax is as follows:
	#3recentspeech#0 - shows all retained room speech context
	#3recentspeech from <timespan>#0 - shows events since that relative time
	#3recentspeech since <timespan>#0 - alias for #3from#0
	#3recentspeech to <timespan>#0 - shows events up to that relative time
	#3recentspeech until <timespan>#0 - alias for #3to#0
	#3recentspeech from <timespan> to <timespan>#0 - shows events between two relative times";

    [PlayerCommand("RecentSpeech", "recentspeech")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("recentspeech", RecentSpeechHelp, AutoHelp.HelpArg)]
    protected static void RecentSpeech(ICharacter actor, string input)
    {
        if (actor.Location is not ICell location)
        {
            actor.OutputHandler.Send("You are not currently in a room.");
            return;
        }

        StringStack ss = new(input.RemoveFirstWord());
        TimeSpan? from = null;
        TimeSpan? to = null;
        while (!ss.IsFinished)
        {
            string keyword = ss.PopSpeech().ToLowerInvariant();
            switch (keyword)
            {
                case "from":
                case "since":
                    {
                        if (ss.IsFinished)
                        {
                            actor.OutputHandler.Send("You must supply a timespan after FROM.");
                            return;
                        }

                        if (!TimeSpan.TryParse(ss.PopSpeech(), actor, out TimeSpan parsedFrom))
                        {
                            actor.OutputHandler.Send(
                                $"That is not a valid timespan. Use your account culture format ({actor.Account.Culture.Name.ColourValue()}).");
                            return;
                        }

                        if (parsedFrom < TimeSpan.Zero)
                        {
                            actor.OutputHandler.Send("The FROM timespan cannot be negative.");
                            return;
                        }

                        from = parsedFrom;
                        break;
                    }
                case "to":
                case "until":
                    {
                        if (ss.IsFinished)
                        {
                            actor.OutputHandler.Send("You must supply a timespan after TO.");
                            return;
                        }

                        if (!TimeSpan.TryParse(ss.PopSpeech(), actor, out TimeSpan parsedTo))
                        {
                            actor.OutputHandler.Send(
                                $"That is not a valid timespan. Use your account culture format ({actor.Account.Culture.Name.ColourValue()}).");
                            return;
                        }

                        if (parsedTo < TimeSpan.Zero)
                        {
                            actor.OutputHandler.Send("The TO timespan cannot be negative.");
                            return;
                        }

                        to = parsedTo;
                        break;
                    }
                default:
                    actor.OutputHandler.Send("Invalid syntax. See #3help recentspeech#0.".SubstituteANSIColour());
                    return;
            }
        }

        if (from.HasValue && to.HasValue && from.Value < to.Value)
        {
            actor.OutputHandler.Send(
                "The FROM timespan must be greater than or equal to the TO timespan (e.g. from 01:00:00 to 00:10:00).");
            return;
        }

        IRecentSpeechContextEffect contextEffect = location.EffectsOfType<IRecentSpeechContextEffect>().FirstOrDefault();
        if (contextEffect is null)
        {
            actor.OutputHandler.Send("There is no stored recent speech context for this room.");
            return;
        }

        DateTime nowUtc = DateTime.UtcNow;
        IReadOnlyCollection<RecentSpeechContextEvent> allEvents = contextEffect.GetRecentSpeechEvents(nowUtc, int.MaxValue, TimeSpan.FromDays(3650));
        DateTime earliestUtc = from.HasValue ? nowUtc - from.Value : DateTime.MinValue;
        DateTime latestUtc = to.HasValue ? nowUtc - to.Value : nowUtc;
        List<RecentSpeechContextEvent> events = allEvents
            .Where(x => x.RealTimeTimestampUtc >= earliestUtc)
            .Where(x => x.RealTimeTimestampUtc <= latestUtc)
            .OrderBy(x => x.RealTimeTimestampUtc)
            .ToList();

        string filterDescription = GetRecentSpeechFilterDescription(actor, from, to);
        if (!events.Any())
        {
            actor.OutputHandler.Send(
                $"No recent speech events were found in {location.HowSeen(actor)} (#{location.Id.ToString("N0", actor)}) with filter {filterDescription.ColourCommand()}.");
            return;
        }

        string textTable = StringUtilities.GetTextTable(
            events.Select(item => new[]
            {
                item.RealTimeTimestampUtc.GetLocalDateString(actor, true),
                item.SpeakerName,
                GetRecentSpeechTargetDescription(item, actor),
                item.Message,
                $"{item.LanguageName}/{item.AccentName}",
                item.Volume.DescribeEnum(true)
            }),
            new[] { "Time", "Speaker", "Target", "Speech", "Language/Accent", "Volume" },
            actor.LineFormatLength,
            colour: Telnet.Green,
            unicodeTable: actor.Account.UseUnicode
        );

        actor.OutputHandler.Send(
            $"Recent speech events in {location.HowSeen(actor)} (#{location.Id.ToString("N0", actor).ColourValue()})\nFilter: {filterDescription.ColourCommand()}\nShowing {events.Count.ToString("N0", actor).ColourValue()} event{(events.Count == 1 ? string.Empty : "s")}.\n{textTable}");
    }

    private static string GetRecentSpeechTargetDescription(RecentSpeechContextEvent item, ICharacter actor)
    {
        string target = item.TargetDescription.IfNullOrWhiteSpace("No direct target");
        if (item.TargetId is null)
        {
            return target;
        }

        return
            $"{target} ({item.TargetFrameworkItemType.IfNullOrWhiteSpace("unknown")} #{item.TargetId.Value.ToString("N0", actor)})";
    }

    private static string GetRecentSpeechFilterDescription(ICharacter actor, TimeSpan? from, TimeSpan? to)
    {
        string fromText = from.HasValue ? $"{from.Value.Describe(actor)} ago" : "the earliest retained event";
        string toText = to.HasValue ? $"{to.Value.Describe(actor)} ago" : "now";
        return $"from {fromText} to {toText}";
    }

    private const string FullAuditHelp =
        @"The #3fullaudit#0 command audits a chargen resource across all accounts in the database.

Use this to see every account's current amount and last award date for a specific chargen resource, ordered by amount and recency.

The resource can be supplied by name or alias. This command queries accounts directly from the database, so it is broader than #3audit#0.

The syntax is as follows:
	#3fullaudit <resource>#0 - shows every account's amount and last award date for a chargen resource";

    [PlayerCommand("FullAudit", "fullaudit")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("fullaudit", FullAuditHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void FullAudit(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.Send("Which resource do you want to audit?");
            return;
        }

        string name = ss.SafeRemainingArgument;
        IChargenResource targetResource = actor.Gameworld.ChargenResources.GetByName(name) ??
                             actor.Gameworld.ChargenResources.FirstOrDefault(x => x.Alias.EqualTo(name));
        if (targetResource == null)
        {
            actor.Send("There is no such resource to audit.");
            return;
        }

        using (new FMDB())
        {
            IEnumerable<(string Account, int Amount, DateTime LastAward)> accounts = FMDB.Connection.Query<(string Account, int Amount, DateTime LastAward)>(
                $"select a.Name, r.Amount, r.LastAwardDate from Accounts a left outer join Accounts_ChargenResources r on r.accountid = a.id and chargenresourceid = {targetResource.Id} order by Amount desc, LastAwardDate desc;");
            actor.Send(StringUtilities.GetTextTable(
                from account in accounts
                select new[]
                {
                    account.Account,
                    account.Amount.ToString("N0", actor),
                    account.LastAward.GetLocalDateString(actor)
                },
                new[] { "Account", "Amount", "Last Award" },
                actor.LineFormatLength,
                colour: Telnet.Green,
                unicodeTable: actor.Account.UseUnicode
            ));
        }
    }

    private const string AuditHelp =
        @"The #3audit#0 command audits a chargen resource for currently loaded player characters.

Use this to see which online or loaded characters' accounts currently hold a specific chargen resource, along with their last award date.

The resource can be supplied by name or alias. For a database-wide account audit, use #3fullaudit#0 instead.

The syntax is as follows:
	#3audit <resource>#0 - shows loaded characters' account values for a chargen resource";

    [PlayerCommand("Audit", "audit")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("audit", AuditHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Audit(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.Send("Which resource do you want to audit?");
            return;
        }

        string name = ss.SafeRemainingArgument;
        IChargenResource targetResource = actor.Gameworld.ChargenResources.GetByName(name) ??
                             actor.Gameworld.ChargenResources.FirstOrDefault(x => x.Alias.EqualTo(name));
        if (targetResource == null)
        {
            actor.Send("There is no such resource to audit.");
            return;
        }

        actor.Send(StringUtilities.GetTextTable(
            from ch in actor.Gameworld.Characters
            select new[]
            {
                ch.Account.Name,
                ch.PersonalName.GetName(NameStyle.FullName),
                ch.HowSeen(ch, flags: PerceiveIgnoreFlags.IgnoreSelf),
                ch.Account.AccountResources[targetResource].ToString("N1", actor),
                ch.Account.AccountResourcesLastAwarded.ValueOrDefault(targetResource, null)
                  ?.GetLocalDateString(actor, true)
            },
            new[] { "Account", "Character", "Desc", "Resource", "Last Award" },
            actor.LineFormatLength,
            colour: Telnet.Green,
            unicodeTable: actor.Account.UseUnicode
        ));
    }

    private const string ImmCommandsHelp =
        @"The #3immcommands#0 command lists staff commands available between Junior Admin level and your current permission level.

Use it as a quick staff command reference for your current command tree.

This command takes no arguments and only reports commands visible to your current permission state.

The syntax is as follows:
	#3immcommands#0 - lists available staff commands";

    [PlayerCommand("ImmCommands", "immcommands")]
    [HelpInfo("immcommands", ImmCommandsHelp, AutoHelp.HelpArg)]
    protected static void ImmCommands(ICharacter actor, string input)
    {
        actor.OutputHandler.Send(
            actor.CommandTree.Commands.ReportCommands(PermissionLevel.JuniorAdmin, actor.PermissionLevel, actor)
                 .ArrangeStringsOntoLines());
    }

    private static void AwardEditorPost(string text, IOutputHandler handler, params object[] arguments)
    {
        bool award = (bool)arguments[0];
        using (new FMDB())
        {
            Models.Account dbaccount = FMDB.Context.Accounts.Find((long)arguments[3]);
            if (dbaccount == null)
            {
                throw new ApplicationException("Account was not found in the database in AwardEditorPost.");
            }

            IChargenResource type = (IChargenResource)arguments[5];
            AccountsChargenResources dbaccountresource =
                dbaccount.AccountsChargenResources.FirstOrDefault(
                    x => x.ChargenResourceId == type.Id);
            if (dbaccountresource == null)
            {
                dbaccountresource = new AccountsChargenResources
                {
                    Amount = 0,
                    ChargenResourceId = type.Id
                };
                dbaccount.AccountsChargenResources.Add(dbaccountresource);
            }

            dbaccountresource.LastAwardDate = DateTime.UtcNow;
            int amount = (int)arguments[2];
            dbaccountresource.Amount += ((bool)arguments[0] ? 1 : -1) * amount;

            ICharacter character = (ICharacter)arguments[1];
            IAccount account = character.Gameworld.Accounts.FirstOrDefault(x => x.Id == dbaccount.Id);
            if (account != null)
            {
                account.AccountResources[type] = dbaccountresource.Amount;
                account.AccountResourcesLastAwarded[type] = dbaccountresource.LastAwardDate;
                if (account.ControllingContext != null &&
                    !string.IsNullOrEmpty(award
                        ? type.TextDisplayedToPlayerOnAward
                        : type.TextDisplayedToPlayerOnDeduct))
                {
                    account.ControllingContext.Send("{0} {1}", "[System Message]".Colour(Telnet.Green),
                        award ? type.TextDisplayedToPlayerOnAward : type.TextDisplayedToPlayerOnDeduct);
                }
            }

            AccountNote newNote = new()
            {
                AuthorId = character.Account.Id,
                Subject =
                    $"{(award ? "Award" : "Deduction")} of {amount} {(amount == 1 ? type.Name : type.PluralName).Proper()} by {character.Account.Name.Proper()}.",
                Text = text,
                TimeStamp = dbaccountresource.LastAwardDate
            };
            dbaccount.AccountNotesAccount.Add(newNote);
            character.Gameworld.SystemMessage(
                new EmoteOutput(
                    new Emote(
                        $"You|{character.Account.Name.Proper()} have|has {(award ? "awarded" : "deducted")} {(amount == 1 ? type.Name : type.PluralName)} {amount} {(award ? "to" : "from")} account {dbaccount.Name.Proper()}.",
                        character)), true);

            FMDB.Context.SaveChanges();
        }
    }

    private static void AwardEditorCancel(IOutputHandler handler, params object[] arguments)
    {
        bool award = (bool)arguments[0];
        handler.Send(
            $"You decide not to {(award ? "award" : "deduct")} {((IChargenResource)arguments[5]).PluralName} {(award ? "to" : "from")} {(string)arguments[4]}.");
    }

    private const string AwardHelp =
        @"The #3award#0 and #3deduct#0 commands add or remove chargen resources from an account and require a staff note explaining the change.

Use #3award#0 to give an account a chargen resource, and #3deduct#0 to remove one. After validation, you are placed in the editor to enter the audit note text.

If no amount is supplied, the command uses #31#0. Awarding respects the resource's per-award maximum, minimum time between awards, and permission requirements; deducting uses the same resource lookup but subtracts the amount.

The syntax is as follows:
	#3award <account> <resource> [amount]#0 - awards a chargen resource to an account
	#3deduct <account> <resource> [amount]#0 - deducts a chargen resource from an account";

    [PlayerCommand("Award", "award", "deduct")]
    [DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("award", AwardHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Award(ICharacter character, string input)
    {
        StringStack ss = new(input);
        bool award = ss.PopSpeech().Equals("award", StringComparison.InvariantCultureIgnoreCase);

        if (ss.IsFinished)
        {
            character.OutputHandler.Send(
                $"{(award ? "To" : "From")} whose account do you wish to {(award ? "award" : "deduct")}?");
            return;
        }

        string accountText = ss.PopSpeech();
        string typeText = ss.PopSpeech();
        string amountText = ss.PopSpeech();

        if (string.IsNullOrEmpty(typeText))
        {
            character.OutputHandler.Send(
                $"What type of resource do you wish to {(award ? "award to" : "deduct from")} them?");
            return;
        }

        IChargenResource type =
            character.Gameworld.ChargenResources.FirstOrDefault(
                x =>
                    x.Alias.Equals(typeText, StringComparison.InvariantCultureIgnoreCase) ||
                    x.Name.Equals(typeText, StringComparison.InvariantCultureIgnoreCase));

        if (type == null)
        {
            character.OutputHandler.Send(
                $"That is not a valid type of resource to {(award ? "award to" : "deduct from")} an account.");
            return;
        }

        if (!character.IsAdministrator(type.PermissionLevelRequiredToAward))
        {
            character.OutputHandler.Send($"You are not allowed to award {type.PluralName.TitleCase()}.");
            return;
        }

        int amount;
        if (string.IsNullOrEmpty(amountText))
        {
            amount = 1;
        }
        else
        {
            if (!int.TryParse(amountText, out amount))
            {
                character.OutputHandler.Send(
                    $"You must enter a valid number of {type.PluralName.TitleCase()} to {(award ? "award to" : "deduct from")} them, or do not include any additional text to use the default amount.");
                return;
            }
        }

        if (award && amount > type.MaximumNumberAwardedPerAward)
        {
            character.Send("It is only possible to award {0} {1} per award.", type.MaximumNumberAwardedPerAward,
                type.MaximumNumberAwardedPerAward == 1 ? type.Name : type.PluralName);
            return;
        }

        using (new FMDB())
        {
            Models.Account dbaccount =
                FMDB.Context.Accounts.FirstOrDefault(x => x.Name == accountText);
            if (dbaccount == null)
            {
                character.OutputHandler.Send("That is not a valid account.");
                return;
            }

            AccountsChargenResources dbaccountresource =
                dbaccount.AccountsChargenResources.FirstOrDefault(x => x.ChargenResourceId == type.Id);
            if (award && dbaccountresource != null &&
                DateTime.UtcNow - dbaccountresource.LastAwardDate <= type.MinimumTimeBetweenAwards &&
                !character.IsAdministrator(type.PermissionLevelRequiredToCircumventMinimumTime))
            {
                character.OutputHandler.Send(
                    string.Format(character,
                        "{0} has been awarded {1} too recently. They will become eligable for further awards on {2:f}.",
                        dbaccount.Name.Proper(),
                        type.PluralName.TitleCase(),
                        (dbaccountresource.LastAwardDate + type.MinimumTimeBetweenAwards).ToUniversalTime()
                        .GetLocalDateString(character)
                    ));
                return;
            }

            character.OutputHandler.Send(
                $"You will now be dropped into an editor where you must enter a comment about your {(award ? "award" : "deduction")} of {(amount == 1 ? type.Name : type.PluralName)} {(award ? "to" : "from")} account {dbaccount.Name.Proper()}.");
            character.EditorMode(AwardEditorPost, AwardEditorCancel, 1.0, null, EditorOptions.None,
                new object[] { award, character, amount, dbaccount.Id, dbaccount.Name, type });
        }
    }

    private const string ForceHelp =
        @"The #3force#0 command makes another character, or a filtered group of characters, execute a command.

Use individual force for a visible local target, #3force here#0 for eligible visible characters in your room, and the wider game forms only for serious staff interventions.

All staff see wiz-only output when this command is used. Targets with a NOFORCE effect are skipped or refused. Non-Implementors cannot force staff of equal or higher authority. The #3all#0, #3players#0 and #3npcs#0 forms require Senior Admin and require you to type #3ACCEPT#0 within the confirmation window.
There is no #3force local#0 form; use #3force here#0 for the current room.

The syntax is as follows:
	#3force <target> <command>#0 - forces one visible target to execute a command
	#3force here <command>#0 - forces eligible visible PCs and NPCs in your room
	#3force npcshere <command>#0 - forces eligible NPCs in your room
	#3force all <command>#0 - forces eligible PCs and NPCs in the game; Senior Admin only
	#3force players <command>#0 - forces eligible PCs in the game; Senior Admin only
	#3force npcs <command>#0 - forces eligible NPCs in the game; Senior Admin only";

    [PlayerCommand("Force", "force")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("force", ForceHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Force(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string targetText = ss.PopSpeech();
        if (string.IsNullOrEmpty(targetText))
        {
            character.OutputHandler.Send("Force who to do what?");
            return;
        }

        if (targetText.Equals("all", StringComparison.InvariantCultureIgnoreCase) &&
            character.IsAdministrator(PermissionLevel.SeniorAdmin))
        {
            character.OutputHandler.Send(
                $"Are you sure that you want to force EVERYONE (PC and NPC, including yourself) to execute the following command:\n\n\t{ss.RemainingArgument.ColourCommand()}\n\nThis should typically only be done for a VERY important reason. If you wish to proceed, type ACCEPT. Otherwise, DECLINE.");
            character.AddEffect(new Accept(character, new GenericProposal
            {
                AcceptAction = text =>
                {
                    character.Gameworld.SystemMessage(new EmoteOutput(new Emote(
                            $"@ force|forces everyone in the game to do the command '{ss.RemainingArgument}'",
                            character),
                        flags: OutputFlags.WizOnly), true);
                    foreach (ICharacter person in ForceTargetResolver.Resolve(character.Gameworld, ForceTargetScope.All)
                                                    .Where(x => !x.AffectedBy<IIgnoreForceEffect>() &&
                                                                CommandExecutionGuards.CanForceTarget(character, x))
                                                    .ToList())
                    {
                        person.ExecuteCommand(ss.RemainingArgument);
                    }
                },
                DescriptionString = "forcing everyone in the game to do something",
                ExpireAction = () =>
                {
                    character.OutputHandler.Send(
                        "You decide against forcing everyone in the game to do something.");
                },
                Keywords = new List<string> { "force", "game", "all" },
                RejectAction = text =>
                {
                    character.OutputHandler.Send(
                        "You decide against forcing everyone in the game to do something.");
                }
            }), TimeSpan.FromSeconds(30));

            return;
        }

        if (targetText.Equals("players", StringComparison.InvariantCultureIgnoreCase) &&
            character.IsAdministrator(PermissionLevel.SeniorAdmin))
        {
            character.OutputHandler.Send(
                $"Are you sure that you want to force EVERY PC IN THE GAME (including yourself) to execute the following command:\n\n\t{ss.RemainingArgument.ColourCommand()}\n\nThis should typically only be done for a VERY important reason. If you wish to proceed, type ACCEPT. Otherwise, DECLINE.");
            character.AddEffect(new Accept(character, new GenericProposal
            {
                AcceptAction = text =>
                {
                    character.Gameworld.SystemMessage(new EmoteOutput(new Emote(
                            $"@ force|forces all players in the game to do the command '{ss.RemainingArgument}'",
                            character),
                        flags: OutputFlags.WizOnly), true);
                    foreach (ICharacter person in ForceTargetResolver.Resolve(character.Gameworld, ForceTargetScope.Players)
                                                    .Where(x => !x.AffectedBy<IIgnoreForceEffect>() &&
                                                                CommandExecutionGuards.CanForceTarget(character, x))
                                                    .ToList())
                    {
                        person.ExecuteCommand(ss.RemainingArgument);
                    }
                },
                DescriptionString = "forcing every PC in the game to do something",
                ExpireAction = () =>
                {
                    character.OutputHandler.Send(
                        "You decide against forcing every PC in the game to do something.");
                },
                Keywords = new List<string> { "force", "game", "all" },
                RejectAction = text =>
                {
                    character.OutputHandler.Send(
                        "You decide against forcing every PC in the game to do something.");
                }
            }), TimeSpan.FromSeconds(30));

            return;
        }

        if (targetText.Equals("npcs", StringComparison.InvariantCultureIgnoreCase) &&
            character.IsAdministrator(PermissionLevel.SeniorAdmin))
        {
            character.OutputHandler.Send(
                $"Are you sure that you want to force EVERY NPC IN THE GAME to execute the following command:\n\n\t{ss.RemainingArgument.ColourCommand()}\n\nThis should typically only be done for a VERY important reason. If you wish to proceed, type ACCEPT. Otherwise, DECLINE.");
            character.AddEffect(new Accept(character, new GenericProposal
            {
                AcceptAction = text =>
                {
                    character.Gameworld.SystemMessage(new EmoteOutput(new Emote(
                            $"@ force|forces all NPCs in the game to do the command '{ss.RemainingArgument}'",
                            character),
                        flags: OutputFlags.WizOnly), true);
                    foreach (ICharacter person in ForceTargetResolver.Resolve(character.Gameworld, ForceTargetScope.Npcs)
                                                    .Where(x => !x.AffectedBy<IIgnoreForceEffect>() &&
                                                                CommandExecutionGuards.CanForceTarget(character, x))
                                                    .ToList())
                    {
                        person.ExecuteCommand(ss.RemainingArgument);
                    }
                },
                DescriptionString = "forcing every NPC in the game to do something",
                ExpireAction = () =>
                {
                    character.OutputHandler.Send(
                        "You decide against forcing every NPC in the game to do something.");
                },
                Keywords = new List<string> { "force", "game", "all" },
                RejectAction = text =>
                {
                    character.OutputHandler.Send(
                        "You decide against forcing every NPC in the game to do something.");
                }
            }), TimeSpan.FromSeconds(30));

            return;
        }

        if (targetText.Equals("here", StringComparison.InvariantCultureIgnoreCase))
        {
            character.OutputHandler.Handle(new EmoteOutput(new Emote(
                    $"@ force|forces everyone in the room to do the command '{ss.RemainingArgument}'", character),
                flags: OutputFlags.WizOnly));
            foreach (ICharacter person in character.Location.Characters
                                            .Where(x => !x.AffectedBy<IIgnoreForceEffect>() &&
                                                        !x.AffectedBy<IAdminInvisEffect>() &&
                                                        CommandExecutionGuards.CanForceTarget(character, x))
                                            .ToList())
            {
                person.ExecuteCommand(ss.RemainingArgument);
            }

            return;
        }

        if (targetText.Equals("npcshere", StringComparison.InvariantCultureIgnoreCase))
        {
            character.OutputHandler.Handle(new EmoteOutput(new Emote(
                    $"@ force|forces all NPCs in the room to do the command '{ss.RemainingArgument}'", character),
                flags: OutputFlags.WizOnly));
            foreach (ICharacter person in character.Location.Characters.Where(x => !x.AffectedBy<IIgnoreForceEffect>())
                                            .Where(x => x is INPC && CommandExecutionGuards.CanForceTarget(character, x))
                                            .ToList())
            {
                person.ExecuteCommand(ss.RemainingArgument);
            }

            return;
        }

        ICharacter target = character.TargetActor(targetText);
        if (target == null)
        {
            character.OutputHandler.Send("You do not see them here to force.");
            return;
        }

        if (ss.IsFinished)
        {
            character.OutputHandler.Send("What do you want to force " + target.HowSeen(character) + " to do?");
            return;
        }

        if (target.AffectedBy<IIgnoreForceEffect>())
        {
            character.OutputHandler.Send(
                $"{target.HowSeen(character, true)} is affected by a NOFORCE effect, so they will not respond to FORCE.");
            return;
        }

        if (!CommandExecutionGuards.CanForceTarget(character, target))
        {
            character.OutputHandler.Send(
                $"{target.HowSeen(character, true)} is an admin of equal or higher authority, so you cannot FORCE them.");
            return;
        }

        character.OutputHandler.Handle(new EmoteOutput(new Emote(
                $"@ force|forces $0 to do the command '{ss.RemainingArgument}'", character, target),
            flags: OutputFlags.WizOnly));
        target.ExecuteCommand(ss.RemainingArgument);
    }

    private const string AsHelp =
        @"The #3as#0 command briefly executes a command while controlling another visible character.

Use this when you need a command to run from another character's context rather than forcing their normal controller to do it.

Only Implementors may use #3as#0 on staff characters. Other admins may only use it on non-admin characters. The target must be visible in your current location, and your original controller is restored after the command runs.

The syntax is as follows:
	#3as <target> <command>#0 - executes a command as a visible target";

    [PlayerCommand("As", "as")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("as", AsHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void As(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string targetText = ss.PopSpeech();
        if (string.IsNullOrEmpty(targetText))
        {
            character.OutputHandler.Send("Force who to do what?");
            return;
        }

        ICharacter target = character.TargetActor(targetText);
        if (target == null)
        {
            character.OutputHandler.Send("You do not see them here to force.");
            return;
        }

        if (ss.IsFinished)
        {
            character.OutputHandler.Send("What do you want to force " + target.HowSeen(character) + " to do?");
            return;
        }

        if (!CommandExecutionGuards.CanUseAsTarget(character, target))
        {
            character.OutputHandler.Send(
                $"{target.HowSeen(character, true)} is an admin, so only Implementors can use AS on them.");
            return;
        }

        IController oldController = target.Controller;
        IController actorController = character.Controller;
        character.OutputHandler.Handle(new EmoteOutput(new Emote(
                $"@ force|forces $0 to do the command '{ss.RemainingArgument}'", character, target),
            flags: OutputFlags.WizOnly));
        target.SilentAssumeControl(actorController);
        try
        {
            target.ExecuteCommand(ss.RemainingArgument);
        }
        finally
        {
            target.SilentAssumeControl(oldController);
            character.SilentAssumeControl(actorController);
        }
    }

    public const string LoadCurrencyHelp = @"The #3loadcurrency#0 command creates a physical currency pile for your current staff currency.

Use normal currency text to load an amount, or use the #3coins#0 form to specify exact coin counts. The command uses the currency selected with #3set currency#0.

If the currency cannot make an exact pile for the requested amount, you receive a warning and the closest generated coin set is loaded. If your hands are full, the currency pile is placed on the ground. The exact-coin form cannot specify the same coin type twice.

The syntax is as follows:
	#3loadcurrency <amount>#0 - loads a pile worth the specified amount
	#3loadcurrency <currency text>#0 - loads a pile using that currency's amount parser
	#3loadcurrency coins <count> <coin> [<count> <coin>]...#0 - loads exact coin counts";

    [PlayerCommand("LoadCurrency", "loadcurrency")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("loadcurrency", LoadCurrencyHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void LoadCurrency(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        ICurrency currency = character.Currency;
        Dictionary<ICoin, int> coins = new();
        IGameItem newItem = null;
        if (ss.Peek().ToLowerInvariant() == "coins")
        {
            ss.PopSpeech();
            while (true)
            {
                string samount = ss.PopSpeech();
                if (string.IsNullOrEmpty(samount))
                {
                    character.OutputHandler.Send("You must enter the specific coins which you want to load.");
                    return;
                }

                if (!int.TryParse(samount, out int amount))
                {
                    character.OutputHandler.Send("You must enter a whole number of coins of each type to load.");
                    return;
                }

                string scoin = ss.PopSpeech();
                if (string.IsNullOrEmpty(scoin))
                {
                    character.OutputHandler.Send($"Which coin do you want to load {amount.ToString("N0", character)} of?");
                    return;
                }

                ICoin coin =
                    currency.Coins.FirstOrDefault(
                        x =>
                            x.Name.StartsWith(scoin, StringComparison.InvariantCultureIgnoreCase) ||
                            x.Name.Replace(x.PluralWord, x.PluralWord.Pluralise())
                             .StartsWith(scoin, StringComparison.InvariantCultureIgnoreCase));
                if (coin == null)
                {
                    character.OutputHandler.Send($"There is no such coin as \"{scoin.ColourCommand()}\" for the {currency.Name.ColourName()} currency.");
                    return;
                }

                if (coins.ContainsKey(coin))
                {
                    character.OutputHandler.Send("You cannot specify the same coin twice.");
                    return;
                }

                coins.Add(coin, amount);
                if (ss.IsFinished)
                {
                    break;
                }
            }
        }
        else
        {
            string strAmount = ss.SafeRemainingArgument;
            if (string.IsNullOrEmpty(strAmount))
            {
                character.OutputHandler.Send("What amount of currency do you wish to load?");
                return;
            }

            if (!decimal.TryParse(strAmount, out decimal decimalAmount))
            {
                decimalAmount = currency.GetBaseCurrency(strAmount, out bool success);
                if (!success || decimalAmount <= 0.0M)
                {
                    character.OutputHandler.Send("That is not a valid amount of currency to load.");
                    return;
                }
            }

            coins = currency.FindCoinsForAmount(decimalAmount, out bool exact);
            if (!exact)
            {
                character.OutputHandler.Send(
                    "Warning: Could not find an exact match for the total specified.".Colour(Telnet.Red));
            }
        }

        newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
            coins.Select(x => Tuple.Create(x.Key, x.Value)));
        character.Gameworld.Add(newItem);
        character.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $0.", character, newItem),
            flags: OutputFlags.SuppressObscured));
        if (character.Body.CanGet(newItem, 0))
        {
            character.Body.Get(newItem, silent: true);
        }
        else
        {
            newItem.RoomLayer = character.RoomLayer;
            character.OutputHandler.Send("Your hands are full, so you loaded the item to the ground.");
            character.Location.Insert(newItem);
        }
    }

    private const string LoadCommodityHelp =
        @"The #3loadcommodity#0 command creates a commodity pile, which is a raw bulk quantity of a solid material.

Specify a mass, a material, and optional commodity metadata. Materials can be supplied by ID or name, and weights are parsed using the game's unit manager.

Use #3tag <tag>#0 for multi-word tags. The legacy bare tag form is still accepted, but the explicit #3tag#0 keyword is clearer when also adding characteristics. Repeating a characteristic definition replaces the previous value for that definition. If your hands are full, the commodity is placed on the ground.

The syntax is as follows:
	#3loadcommodity <weight> <material>#0 - loads a commodity pile
	#3loadcommodity <weight> <material> tag <tag>#0 - loads a tagged commodity pile
	#3loadcommodity <weight> <material> characteristic <definition> <value>#0 - loads a commodity with a characteristic
	#3loadcommodity <weight> <material> tag <tag> characteristic <definition> <value> [characteristic <definition> <value>]...#0 - combines tag and characteristics";

    [PlayerCommand("LoadCommodity", "loadcommodity")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("loadcommodity", LoadCommodityHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void LoadCommodity(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        double amount =
            actor.Gameworld.UnitManager.GetBaseUnits(ss.PopSpeech(), Framework.Units.UnitType.Mass, out bool success);
        if (!success)
        {
            actor.Send("That is not a valid weight of commodity to load.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("Which material do you want to load as a commodity?");
            return;
        }

        ISolid material = long.TryParse(ss.PopSpeech(), out long value)
            ? actor.Gameworld.Materials.Get(value)
            : actor.Gameworld.Materials.GetByName(ss.Last);

        if (material == null)
        {
            actor.Send("There is no such material.");
            return;
        }

        ITag tag = null;
        List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> characteristics = new();
        while (!ss.IsFinished)
        {
            if (ss.PeekSpeech().EqualTo("tag"))
            {
                ss.PopSpeech();
                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Which tag do you want to use for this commodity pile?");
                    return;
                }

                List<string> tagParts = new()
                {
                    ss.PopSpeech()
                };
                while (!ss.IsFinished && !ss.PeekSpeech().EqualToAny("characteristic", "characteristics", "char"))
                {
                    tagParts.Add(ss.PopSpeech());
                }

                string tagText = string.Join(" ", tagParts);
                tag = actor.Gameworld.Tags.GetByIdOrName(tagText);
                if (tag is null)
                {
                    actor.OutputHandler.Send($"There is no such tag identified by {tagText.ColourCommand()}.");
                    return;
                }

                continue;
            }

            if (ss.PeekSpeech().EqualToAny("characteristic", "characteristics", "char"))
            {
                ss.PopSpeech();
                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Which characteristic definition do you want to set?");
                    return;
                }

                string definitionText = ss.PopSpeech();
                ICharacteristicDefinition definition = actor.Gameworld.Characteristics.GetByIdOrName(definitionText);
                if (definition is null)
                {
                    actor.OutputHandler.Send($"There is no characteristic definition identified by {definitionText.ColourCommand()}.");
                    return;
                }

                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send($"Which value do you want to set for the {definition.Name.ColourName()} characteristic?");
                    return;
                }

                List<string> valueParts = new()
                {
                    ss.PopSpeech()
                };
                while (!ss.IsFinished && !ss.PeekSpeech().EqualToAny("characteristic", "characteristics", "char"))
                {
                    valueParts.Add(ss.PopSpeech());
                }

                string valueText = string.Join(" ", valueParts);
                ICharacteristicValue characteristicValue = CommodityCharacteristicRequirement.GetCharacteristicValue(actor.Gameworld, valueText);
                if (characteristicValue is null || !definition.IsValue(characteristicValue))
                {
                    actor.OutputHandler.Send($"There is no {definition.Name.ColourName()} characteristic value identified by {valueText.ColourCommand()}.");
                    return;
                }

                characteristics.RemoveAll(x => x.Definition == definition);
                characteristics.Add((definition, characteristicValue));
                continue;
            }

            if (tag is not null)
            {
                actor.OutputHandler.Send($"The text {ss.PeekSpeech().ColourCommand()} is not a valid commodity option. Use {("characteristic <definition> <value>").ColourCommand()} to add commodity characteristics.");
                return;
            }

            string legacyTagText = ss.RemainingArgument.Contains(" characteristic ", StringComparison.InvariantCultureIgnoreCase)
                ? ss.PopSpeech()
                : ss.SafeRemainingArgument;
            tag = actor.Gameworld.Tags.GetByIdOrName(legacyTagText);
            if (tag is null)
            {
                actor.OutputHandler.Send(
                    $"There is no such tag identified by {legacyTagText.ColourCommand()}.");
                return;
            }

            if (legacyTagText.EqualTo(ss.SafeRemainingArgument))
            {
                break;
            }
        }

        IGameItem commodity = CommodityGameItemComponentProto.CreateNewCommodity(material, amount, tag, false, characteristics);
        commodity.RoomLayer = actor.RoomLayer;
        actor.Gameworld.Add(commodity);
        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $0.", actor, commodity),
            flags: OutputFlags.SuppressObscured));
        if (actor.Body.CanGet(commodity, 0))
        {
            actor.Body.Get(commodity, silent: true);
        }
        else
        {
            actor.OutputHandler.Send("Your hands are full, so you loaded the item to the ground.");
            commodity.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(commodity);
        }

        commodity.Login();
        commodity.HandleEvent(EventType.ItemFinishedLoading, commodity);
    }

    private const string LoadColourLiquidHelp =
        @"The #3loadcolourliquid#0 command fills a liquid container with a special coloured liquid such as tattoo ink.

Specify the colour-liquid type, colour, and target liquid container. You can target a container in the room or inventory, or a container held by another visible character.

The currently supported colour-liquid type is #3tattoo_ink#0. The command fills the target's remaining liquid capacity and refuses non-liquid containers or containers that are already full. #3loadcolorliquid#0 is an accepted spelling alias.

The syntax is as follows:
	#3loadcolourliquid tattoo_ink <colour> <container>#0 - loads tattoo ink into a local container
	#3loadcolourliquid tattoo_ink <colour> <holder> <container>#0 - loads tattoo ink into a container held by another character
	#3loadcolorliquid tattoo_ink <colour> <container>#0 - spelling alias";

    [PlayerCommand("LoadColourLiquid", "loadcolourliquid", "loadcolorliquid")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("loadcolourliquid", LoadColourLiquidHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void LoadColourLiquid(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            character.Send("Which type of colour liquid do you want to load? Valid types are tattoo_ink.");
            return;
        }

        string typeText = ss.PopSpeech().ToLowerInvariant();

        if (ss.IsFinished)
        {
            character.OutputHandler.Send("What colour do you want to load for that liquid?");
            return;
        }

        IColour colour = long.TryParse(ss.PopSpeech(), out long value)
            ? character.Gameworld.Colours.Get(value)
            : character.Gameworld.Colours.GetByName(ss.Last);
        if (colour == null)
        {
            character.OutputHandler.Send("There is no such colour.");
            return;
        }

        if (ss.IsFinished)
        {
            character.Send("What do you want to load that liquid into?");
            return;
        }

        string targettext = ss.PopSpeech(), chartext = null;
        if (!ss.IsFinished) // They can load something into a liquid container someone holds
        {
            chartext = targettext;
            targettext = ss.PopSpeech();
        }

        IGameItem target;
        if (!string.IsNullOrEmpty(chartext))
        {
            ICharacter charTarget = character.TargetActor(chartext);
            if (charTarget == null)
            {
                character.Send("You do not see that person here.");
                return;
            }

            target = charTarget.Inventory.GetFromItemListByKeyword(targettext, character);
        }
        else
        {
            target = character.TargetItem(targettext);
        }

        if (target == null)
        {
            character.Send("You do not see anything like that to load the liquid into.");
            return;
        }

        if (!target.IsItemType<ILiquidContainer>())
        {
            character.Send("{0} is not a liquid container, and so cannot contain liquid.", target.HowSeen(character));
            return;
        }

        ILiquidContainer lqtarget = target.GetItemType<ILiquidContainer>();
        if (lqtarget.LiquidVolume >= lqtarget.LiquidCapacity)
        {
            character.OutputHandler.Send($"{target.HowSeen(character, true)} is already full of liquid.");
            return;
        }

        LiquidMixture liquid;
        string liquidDescription;
        switch (typeText)
        {
            case "tattoo":
            case "tattooink":
            case "tattoo ink":
            case "tattoo_ink":
                liquid = new LiquidMixture(
                    new List<LiquidInstance>
                    {
                        new ColourLiquidInstance(TattooTemplate.InkLiquid, colour,
                            lqtarget.LiquidCapacity - lqtarget.LiquidVolume)
                    }, character.Gameworld);
                liquidDescription = $"{colour.Name} tattoo ink";
                break;
            default:
                character.OutputHandler.Send("That is not a valid colour liquid type. Valid types are tattoo_ink.");
                return;
        }

        character.OutputHandler.Handle(
            new EmoteOutput(new Emote($"@ load|loads {liquidDescription} into $0.", character, target),
                flags: OutputFlags.SuppressObscured));
        lqtarget.MergeLiquid(liquid, character, "loadliquid");
    }

    private const string LoadBloodHelp =
        @"The #3loadblood#0 command fills a liquid container with blood from a character.

Specify the source character, then the target liquid container. The source can be a visible character keyword or a character ID.

The command fills the target's remaining liquid capacity and refuses non-liquid containers or containers that are already full. You can target a container held by another visible character by supplying the holder before the container keyword.

The syntax is as follows:
	#3loadblood <character|id> <container>#0 - loads that character's blood into a local container
	#3loadblood <character|id> <holder> <container>#0 - loads that character's blood into a held container";

    [PlayerCommand("LoadBlood", "loadblood")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("loadblood", LoadBloodHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void LoadBlood(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            character.Send("Which character's blood do you want to load?");
            return;
        }

        ICharacter targetCharacter = long.TryParse(ss.PopSpeech(), out long value)
            ? character.Gameworld.TryGetCharacter(value, true)
            : character.TargetActor(ss.Last);
        if (targetCharacter == null)
        {
            character.OutputHandler.Send("There is no such character.");
            return;
        }

        if (ss.IsFinished)
        {
            character.Send("What do you want to load that blood into?");
            return;
        }

        string targettext = ss.PopSpeech(), chartext = null;
        if (!ss.IsFinished) // They can load something into a liquid container someone holds
        {
            chartext = targettext;
            targettext = ss.PopSpeech();
        }

        IGameItem target;
        if (!string.IsNullOrEmpty(chartext))
        {
            ICharacter charTarget = character.TargetActor(chartext);
            if (charTarget == null)
            {
                character.Send("You do not see that person here.");
                return;
            }

            target = charTarget.Inventory.GetFromItemListByKeyword(targettext, character);
        }
        else
        {
            target = character.TargetItem(targettext);
        }

        if (target == null)
        {
            character.Send("You do not see anything like that to load blood into.");
            return;
        }

        if (!target.IsItemType<ILiquidContainer>())
        {
            character.Send("{0} is not a liquid container, and so cannot contain liquid.", target.HowSeen(character));
            return;
        }

        ILiquidContainer lqtarget = target.GetItemType<ILiquidContainer>();
        if (lqtarget.LiquidVolume >= lqtarget.LiquidCapacity)
        {
            character.OutputHandler.Send($"{target.HowSeen(character, true)} is already full of liquid.");
            return;
        }

        LiquidMixture liquid =
            new(
                new List<LiquidInstance>
                    { new BloodLiquidInstance(targetCharacter, lqtarget.LiquidCapacity - lqtarget.LiquidVolume) },
                character.Gameworld);
        character.OutputHandler.Handle(
            new EmoteOutput(new Emote($"@ load|loads $1's blood into $0.", character, target, targetCharacter),
                flags: OutputFlags.SuppressObscured));

        lqtarget.MergeLiquid(liquid, character, "loadliquid");
    }

    private const string LoadLiquidHelp =
        @"The #3loadliquid#0 command fills a liquid container with a standard liquid.

Specify the liquid by ID or exact name, then the target liquid container. You can target a container in the room or inventory, or a container held by another visible character.

The command fills the target's remaining liquid capacity and refuses non-liquid containers or containers that are already full.

The syntax is as follows:
	#3loadliquid <liquid|id> <container>#0 - loads a liquid into a local container
	#3loadliquid <liquid|id> <holder> <container>#0 - loads a liquid into a held container";

    [PlayerCommand("LoadLiquid", "loadliquid")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("loadliquid", LoadLiquidHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void LoadLiquid(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            character.Send("Which liquid do you want to load?");
            return;
        }

        ILiquid liquid = long.TryParse(ss.PopSpeech(), out long value)
            ? character.Gameworld.Liquids.Get(value)
            : character.Gameworld.Liquids.FirstOrDefault(
                x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
        if (liquid == null)
        {
            character.Send("There is no such liquid for you to load.");
            return;
        }

        if (ss.IsFinished)
        {
            character.Send("What do you want to load that liquid into?");
            return;
        }

        string targettext = ss.PopSpeech(), chartext = null;
        if (!ss.IsFinished) // They can load something into a liquid container someone holds
        {
            chartext = targettext;
            targettext = ss.PopSpeech();
        }

        IGameItem target;
        if (!string.IsNullOrEmpty(chartext))
        {
            ICharacter charTarget = character.TargetActor(chartext);
            if (charTarget == null)
            {
                character.Send("You do not see that person here.");
                return;
            }

            target = charTarget.Inventory.GetFromItemListByKeyword(targettext, character);
        }
        else
        {
            target = character.TargetItem(targettext);
        }

        if (target == null)
        {
            character.Send("You do not see anything like that to load liquid into.");
            return;
        }

        if (!target.IsItemType<ILiquidContainer>())
        {
            character.Send("{0} is not a liquid container, and so cannot contain liquid.", target.HowSeen(character));
            return;
        }

        ILiquidContainer lqtarget = target.GetItemType<ILiquidContainer>();

        if (lqtarget.LiquidVolume >= lqtarget.LiquidCapacity)
        {
            character.OutputHandler.Send($"{target.HowSeen(character, true)} is already full of liquid.");
            return;
        }

        character.OutputHandler.Handle(
            new EmoteOutput(new Emote($"@ load|loads {liquid.MaterialDescription} into $0.", character, target),
                flags: OutputFlags.SuppressObscured));

        lqtarget.MergeLiquid(
            new LiquidMixture(liquid, lqtarget.LiquidCapacity - lqtarget.LiquidVolume, character.Gameworld), character,
            "loadliquid");
    }

    private const string LoadGasHelp =
        @"The #3loadgas#0 command fills a gas container with a standard gas at its one-atmosphere capacity.

Specify the gas by ID or exact name, then the target gas container. You can target a container in the room or inventory, or a container held by another visible character.

Loading gas replaces the container's current gas and fills it to capacity at one atmosphere. The command refuses non-gas containers.

The syntax is as follows:
	#3loadgas <gas|id> <container>#0 - loads a gas into a local gas container
	#3loadgas <gas|id> <holder> <container>#0 - loads a gas into a held gas container";

    [PlayerCommand("LoadGas", "loadgas")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("loadgas", LoadGasHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void LoadGas(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which gas did you want to load?");
            return;
        }

        IGas gas = long.TryParse(ss.PopSpeech(), out long value)
            ? actor.Gameworld.Gases.Get(value)
            : actor.Gameworld.Gases.FirstOrDefault(
                x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
        if (gas == null)
        {
            actor.Send("There is no such gas for you to load.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("What do you want to load that gas into?");
            return;
        }

        string targettext = ss.PopSpeech(), chartext = null;
        if (!ss.IsFinished) // They can load something into a gas container someone holds
        {
            chartext = targettext;
            targettext = ss.PopSpeech();
        }

        IGameItem target;
        if (!string.IsNullOrEmpty(chartext))
        {
            ICharacter charTarget = actor.TargetActor(chartext);
            if (charTarget == null)
            {
                actor.Send("You do not see that person here.");
                return;
            }

            target = charTarget.Inventory.GetFromItemListByKeyword(targettext, actor);
        }
        else
        {
            target = actor.TargetItem(targettext);
        }

        if (target == null)
        {
            actor.Send("You do not see anything like that to load liquid into.");
            return;
        }

        if (!target.IsItemType<IGasContainer>())
        {
            actor.Send("{0} is not a gas container, and so cannot contain gases.", target.HowSeen(actor));
            return;
        }

        IGasContainer gcTarget = target.GetItemType<IGasContainer>();
        gcTarget.Gas = gas;
        gcTarget.GasVolumeAtOneAtmosphere = gcTarget.GasCapacityAtOneAtmosphere;
        actor.OutputHandler.Handle(
            new EmoteOutput(new Emote($"@ load|loads {gas.MaterialDescription} into $0.", actor, target),
                flags: OutputFlags.SuppressObscured));
    }

    private const string NotesHelp =
        @"The #3notes#0 command lists account notes for an account.

Use this to review the note IDs, dates, authors and subjects recorded against an account.

Journal entries appear in the same table and are marked in the Journal column. Use #3note read <id>#0 to read a note's full text.

The syntax is as follows:
	#3notes <account>#0 - lists notes for an account";

    [PlayerCommand("Notes", "notes")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("notes", NotesHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Notes(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string cmd = ss.PopSpeech();

        if (string.IsNullOrEmpty(cmd))
        {
            character.OutputHandler.Send("Which account do you want to list the notes for?");
            return;
        }

        using (new FMDB())
        {
            Models.Account account =
                FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
            if (account == null)
            {
                character.OutputHandler.Send("That is not a valid account.");
                return;
            }

            List<AccountNote> notes =
                FMDB.Context.AccountNotes.Where(x => x.AccountId == account.Id)
                    .OrderByDescending(x => x.TimeStamp)
                    .ThenBy(x => x.Id)
                    .ToList();
            character.OutputHandler.Send(
                StringUtilities.GetTextTable(
                    from note in notes
                    select
                        new[]
                        {
                            note.Id.ToString(), note.TimeStamp.GetLocalDateString(character),
                            note.Author != null ? note.Author.Name.Proper() : "System",
                            note.IsJournalEntry
                                ? $"[{note.Character.Name}'s Journal]: \"{note.Subject}\""
                                : note.Subject.Proper(),
                            note.IsJournalEntry.ToString(character)
                        },
                    new[] { "Id", "Time", "Author", "Subject", "Journal?" },
                    character.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 3,
                    unicodeTable: character.Account.UseUnicode
                )
            );
        }
    }

    private static void NoteRead(ICharacter character, StringStack input)
    {
        if (!long.TryParse(input.PopSpeech(), out long value))
        {
            character.OutputHandler.Send("That is not a valid note ID.");
            return;
        }

        using (new FMDB())
        {
            AccountNote note = FMDB.Context.AccountNotes.Find(value);
            if (note == null)
            {
                character.OutputHandler.Send("There is no note with that ID number.");
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine(
                $"{$"Account Note {note.Id.ToString().Colour(Telnet.Green)}".RawTextPadRight(25)}{$"Account: {note.Account.Name.Proper().Colour(Telnet.Green)}".RawTextPadRight(25)}{string.Format($"Author: {(note.Author != null ? note.Author.Name.Proper().Colour(Telnet.Green) : "System".Colour(Telnet.Green))}", note.Author != null ? note.Author.Name.Proper().Colour(Telnet.Green) : "System".Colour(Telnet.Green))}");
            sb.AppendLine("Subject: " + note.Subject.Colour(Telnet.Green));
            if (note.CharacterId != null && note.Character?.NameInfo != null && XElement.Parse(note.Character.NameInfo) is { } pne)
            {
                sb.AppendLine();
                sb.AppendLine(
                    $"From the journal of {new PersonalName(pne.Element("PersonalName").Element("Name"), character.Gameworld).GetName(NameStyle.FullName).ColourName()}.");
                sb.AppendLine();
            }

            sb.AppendLine("Text:");
            sb.AppendLine();
            sb.AppendLine(note.Text.Wrap(80, "\t").NoWrap());

            character.OutputHandler.Send(sb.ToString());
        }
    }

    private static void NoteWritePost(string message, IOutputHandler handler, object[] arguments)
    {
        using (new FMDB())
        {
            AccountNote note = new()
            {
                Text = message,
                AccountId = (long)arguments[0],
                AuthorId = (long)arguments[2],
                Subject = (string)arguments[1],
                TimeStamp = DateTime.UtcNow
            };
            FMDB.Context.AccountNotes.Add(note);
            FMDB.Context.SaveChanges();
            handler.Send("You finish writing account note #" + note.Id + ".");
        }
    }

    private static void NoteWriteCancel(IOutputHandler handler, object[] arguments)
    {
        handler.Send("You decide not to post any account note.");
    }

    private static void NoteWrite(ICharacter character, StringStack input)
    {
        string accountName = input.PopSpeech().ToLowerInvariant();
        if (string.IsNullOrEmpty(accountName))
        {
            character.OutputHandler.Send("To which account will your note pertain?");
            return;
        }

        string subject = input.SafeRemainingArgument.Trim();
        if (string.IsNullOrEmpty(subject))
        {
            character.OutputHandler.Send("You must supply a subject for your account note.");
            return;
        }

        using (new FMDB())
        {
            Models.Account account = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == accountName);
            if (account == null)
            {
                character.OutputHandler.Send("There is no such account.");
                return;
            }

            character.OutputHandler.Send("Write your note in the text editor below.");
            character.EditorMode(NoteWritePost, NoteWriteCancel, 1.0,
                suppliedArguments: new object[] { account.Id, subject, character.Account.Id });
        }
    }

    private const string NoteHelp =
        @"The #3note#0 command reads or writes account notes.

Use #3note read#0 with a note ID from #3notes#0, or #3note write#0 to create a new note and enter its body in the text editor.

Written notes are account notes, not character journal entries. The subject is the rest of the command after the account name.

The syntax is as follows:
	#3note read <id>#0 - reads an account note
	#3note write <account> <subject>#0 - opens the editor to write a new account note";

    [PlayerCommand("Note", "note")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("note", NoteHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Note(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "read":
                NoteRead(character, ss);
                break;
            case "write":
                NoteWrite(character, ss);
                break;
            default:
                character.OutputHandler.Send("Do you want to read or write a note?");
                return;
        }
    }

    private const string MortalHelp =
        @"The #3mortal#0 command removes your admin sight effect and returns you to mortal perception.

Use this when you no longer want the enhanced staff perception granted by #3immortal#0.

This command takes no arguments. If you are already mortal, it reports that no change is needed.

The syntax is as follows:
	#3mortal#0 - removes admin sight";

    [PlayerCommand("Mortal", "mortal")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("mortal", MortalHelp, AutoHelp.HelpArg)]
    protected static void Mortal(ICharacter actor, string input)
    {
        if (!actor.AffectedBy<IAdminSightEffect>())
        {
            actor.OutputHandler.Send("You are already mortal.");
            return;
        }

        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is no longer immortal.", actor.Body),
            flags: OutputFlags.WizOnly));
        actor.RemoveAllEffects(x => x.IsEffectType<IAdminSightEffect>());
        actor.Gameworld.GameStatistics.UpdateOnlinePlayers();
    }

    private const string ImmWalkHelp =
        @"The #3immwalk#0 command toggles Imm Walk on yourself for staff movement.

Use this when staff movement needs to bypass normal movement restrictions handled by the Imm Walk effect.

This command takes no arguments and toggles the effect on or off.

The syntax is as follows:
	#3immwalk#0 - toggles Imm Walk";

    [PlayerCommand("ImmWalk", "immwalk")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("immwalk", ImmWalkHelp, AutoHelp.HelpArg)]
    protected static void ImmWalk(ICharacter actor, string input)
    {
        if (actor.AffectedBy<IImmwalkEffect>())
        {
            actor.RemoveAllEffects(x => x.IsEffectType<IImmwalkEffect>());
            actor.OutputHandler.Send("You will no longer Imm Walk.");
        }
        else
        {
            actor.AddEffect(new Immwalk(actor));
            actor.OutputHandler.Send("You will now Imm Walk.");
        }
    }

    private const string ImmortalHelp =
        @"The #3immortal#0 command gives you admin sight.

Use this to enable the enhanced staff perception associated with immortality. #3imm#0 is an alias.

This command takes no arguments. If you already have admin sight, it reports that you are already immortal.

The syntax is as follows:
	#3immortal#0 - enables admin sight
	#3imm#0 - alias for #3immortal#0";

    [PlayerCommand("Immortal", "immortal", "imm")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("immortal", ImmortalHelp, AutoHelp.HelpArg)]
    protected static void Immortal(ICharacter actor, string input)
    {
        if (actor.AffectedBy<IAdminSightEffect>())
        {
            actor.OutputHandler.Send("You are already immortal.");
            return;
        }

        actor.AddEffect(new AdminSight(actor));
        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ become|becomes immortal.", actor),
            flags: OutputFlags.WizOnly));
    }

    private const string TransferHelp =
        @"The #3transfer#0 command teleports a loaded character to your current location and room layer.

Use this to bring an online or loaded actor to you by name or keyword.

The target is resolved from loaded actors, not offline character records. The command refuses targets already colocated with you.

The syntax is as follows:
	#3transfer <target>#0 - transfers a loaded actor to your location";

    [PlayerCommand("Transfer", "transfer")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("transfer", TransferHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Transfer(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string targetText = ss.SafeRemainingArgument;
        if (string.IsNullOrEmpty(targetText))
        {
            actor.OutputHandler.Send("Who do you want to transfer?");
            return;
        }

        ICharacter target = actor.Gameworld.Actors.GetByName(targetText) ??
                     actor.Gameworld.Actors.GetFromItemListByKeywordIncludingNames(targetText, actor);
        if (target == null)
        {
            actor.OutputHandler.Send("There is no such player for you to transfer.");
            return;
        }

        if (target.ColocatedWith(actor))
        {
            actor.OutputHandler.Send("They are already in the same location as you.");
            return;
        }

        target.OutputHandler.Send(new EmoteOutput(new Emote("$0 transfers you to &0's location.", target, actor)));
        actor.OutputHandler.Send(new EmoteOutput(new Emote("You transfer $0 to your location.", actor, target)));
        target.Teleport(actor.Location, actor.RoomLayer, true, true);
    }

    public const string SkillCommandHelp =
        @"The #3skill#0 command lets staff inspect skill definitions, manage character skill values, and edit skill definitions.

Use #3skill add#0, #3skill remove#0 and #3skill level#0 for a character's skills. Use #3skill list#0, #3skill show#0 and the edit forms for the skill definitions themselves.

Skill definition editing commands require Senior Admin or higher. Character targets can be visible targets, loaded character IDs, or personal names. #3pause#0 and #3unpause#0 are recognised by the parser but currently report that they are coming soon.
When changing a skill cap formula, you will usually want #3traitexpression#0 to edit the existing expression rather than assigning a different expression.

The syntax is as follows:
	#3skill list [filter]#0 - lists skills
	#3skill add <who> <skill> [level]#0 - adds a skill to a character
	#3skill remove <who> <skill>#0 - removes a skill from a character
	#3skill level <who> <skill> [level]#0 - adds or sets a character's skill level
	#3skill pause <who> <skill>#0 - recognised but not yet implemented
	#3skill unpause <who> <skill>#0 - recognised but not yet implemented
	#3skill show <skill>#0 - shows details of a skill definition
	#3skill view <skill>#0 - alias for #3skill show#0
	#3skill edit <skill>#0 - begins editing a skill definition; Senior Admin only
	#3skill edit#0 - shows the skill definition you are editing
	#3skill edit new <name>#0 - creates and edits a new skill; Senior Admin only
	#3skill edit close#0 - stops editing a skill
	#3skill clone <cloned> <name>#0 - clones an existing skill to a new one; Senior Admin only
	#3skill set <field> <value>#0 - sends a builder command to the skill you are editing; Senior Admin only

Common #3skill set#0 fields:
	#3name <name>#0, #3expression <expression>#0, #3improver <which>#0, #3describer <which>#0, #3group <group>#0, #3branch <multiplier%>#0, #3chargen <prog>#0, #3teachable <prog>#0, #3learnable <prog>#0, #3teach <difficulty>#0, #3learn <difficulty>#0, #3hidden#0";

    [PlayerCommand("Skill", "skill")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("Skill", SkillCommandHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Skill(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string actionText = ss.PopSpeech().ToLowerInvariant();

        switch (actionText)
        {
            case "edit":
            case "set":
            case "clone":
                if (!actor.IsAdministrator(PermissionLevel.SeniorAdmin))
                {
                    actor.OutputHandler.Send("Skill editing is only available to Senior Administrators or higher.");
                    return;
                }

                break;
        }

        switch (actionText)
        {
            case "list":
                ShowModule.Show_Skills(actor, ss);
                return;
            case "edit":
                SkillEdit(actor, ss);
                return;
            case "set":
                SkillSet(actor, ss);
                return;
            case "clone":
                SkillClone(actor, ss);
                return;
            case "view":
            case "show":
                SkillView(actor, ss);
                return;
            case "add":
            case "level":
            case "remove":
            case "pause":
            case "unpause":
                break;
            default:
                actor.OutputHandler.Send(SkillCommandHelp.SubstituteANSIColour());
                return;
        }

        if (ss.IsFinished)
        {
            actor.Send("Whose skills do you want to change?");
            return;
        }

        string targetText = ss.PopSpeech();
        ICharacter target;
        if (long.TryParse(targetText, out long value))
        {
            target = actor.Gameworld.TryGetCharacter(value, true);
        }
        else
        {
            target = actor.TargetActor(targetText) ?? actor.Gameworld.Characters.GetByPersonalName(targetText);
        }

        if (target == null)
        {
            actor.Send("You do not see anybody like that whose skills you can edit.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("Which skill do you want to change for {0}?", target.HowSeen(actor));
            return;
        }

        ITraitDefinition skill = long.TryParse(ss.PopSpeech(), out value)
            ? actor.Gameworld.Traits.Get(value)
            : actor.Gameworld.Traits.GetByName(ss.Last);
        if (skill == null || skill.TraitType != TraitType.Skill)
        {
            actor.Send("There is no such skill.");
            return;
        }

        switch (actionText)
        {
            case "add":
                if (target.HasTrait(skill))
                {
                    actor.Send("{0} already has the {1} skill.", target.HowSeen(actor, true),
                        skill.Name.Proper().Colour(Telnet.Green));
                    return;
                }

                double dvalue = 1.0;
                if (!ss.IsFinished && !double.TryParse(ss.PopSpeech(), out dvalue))
                {
                    actor.Send("You must either specify a value for the new skill, or leave blank to start at 1.");
                    return;
                }

                target.AddTrait(skill, dvalue);
                actor.Send("You add the {0} skill to {1} at a value of {2:N2}.",
                    skill.Name.Proper().Colour(Telnet.Green), target.HowSeen(actor), dvalue);
                return;
            case "remove":
            case "rem":
                if (!target.HasTrait(skill))
                {
                    actor.Send("{0} does not have the {1} skill.", target.HowSeen(actor, true),
                        skill.Name.Proper().Colour(Telnet.Green));
                    return;
                }

                target.RemoveTrait(skill);
                actor.Send("You remove the {0} skill from {1}", skill.Name.Proper().Colour(Telnet.Green),
                    target.HowSeen(actor));
                return;
            case "level":
                dvalue = 1.0;
                if (!ss.IsFinished && !double.TryParse(ss.PopSpeech(), out dvalue))
                {
                    actor.Send("You must either specify a value for the new skill, or leave blank to start at 1.");
                    return;
                }

                if (!target.HasTrait(skill))
                {
                    target.AddTrait(skill, dvalue);
                    actor.Send("You add the {0} skill to {1} at a value of {2:N2}.",
                        skill.Name.Proper().Colour(Telnet.Green), target.HowSeen(actor), dvalue);
                    return;
                }

                target.SetTraitValue(skill, dvalue);
                actor.Send("You set the value of {0} on {1} to {2:N2}.", skill.Name.Proper().Colour(Telnet.Green),
                    target.HowSeen(actor), dvalue);
                return;
            case "pause":
            case "unpause":
                actor.Send("Coming soon.");
                return;
            default:
                actor.Send("Do you want to add, remove, set, pause or unpause a skill?");
                return;
        }
    }

    private static void SkillView(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which skill do you want to view?");
            return;
        }

        if ((long.TryParse(ss.PopSpeech(), out long value)
                ? actor.Gameworld.Traits.Get(value)
                : actor.Gameworld.Traits.GetByName(ss.Last)) is not ISkillDefinition skill)
        {
            actor.OutputHandler.Send("There is no such skill.");
            return;
        }

        actor.OutputHandler.Send(skill.Show(actor));
    }

    private static void SkillClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which skill do you want to clone?");
            return;
        }

        if ((long.TryParse(ss.PopSpeech(), out long value)
                ? actor.Gameworld.Traits.Get(value)
                : actor.Gameworld.Traits.GetByName(ss.Last)) is not ISkillDefinition skill)
        {
            actor.OutputHandler.Send("There is no such skill.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new skill?");
            return;
        }

        string name = ss.PopSpeech().ToLowerInvariant().TitleCase();
        if (actor.Gameworld.Traits.Any(x => x.TraitType == TraitType.Skill && x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send(
                $"There is already a skill called {name.ColourName()}. Skill names must be unique.");
            return;
        }

        using (new FMDB())
        {
            Models.TraitDefinition newSkillModel = new()
            {
                Name = name,
                Type = (int)TraitType.Skill,
                DerivedType = 0,
                TraitGroup = skill.Group,
                ChargenBlurb = string.Empty,
                Hidden = skill.Hidden,
                DecoratorId = skill.Decorator.Id,
                ImproverId = skill.Improver.Id,
                AvailabilityProgId = skill.AvailabilityProg.Id,
                TeachDifficulty = (int)skill.TeachDifficulty,
                LearnDifficulty = (int)skill.LearnDifficulty,
                TeachableProgId = skill.TeachableProg.Id,
                LearnableProgId = skill.LearnableProg.Id,
                BranchMultiplier = skill.BranchMultiplier
            };
            Models.TraitExpression expression = new()
            {
                Name = $"{name} Cap",
                Expression = skill.Cap.OriginalFormulaText
            };
            foreach (KeyValuePair<string, TraitExpressionParameter> parameter in skill.Cap.Parameters)
            {
                expression.TraitExpressionParameters.Add(new TraitExpressionParameters
                {
                    TraitExpression = expression,
                    TraitDefinitionId = parameter.Value.Trait.Id,
                    CanBranch = parameter.Value.CanBranch,
                    CanImprove = parameter.Value.CanImprove,
                    Parameter = parameter.Key
                });
            }

            newSkillModel.Expression = expression;
            FMDB.Context.TraitExpressions.Add(expression);
            FMDB.Context.TraitDefinitions.Add(newSkillModel);
            FMDB.Context.SaveChanges();

            actor.Gameworld.Add(new TraitExpression(expression, actor.Gameworld));
            SkillDefinition newSkill = new(newSkillModel, actor.Gameworld);
            actor.Gameworld.Add(newSkill);
            newSkill.Initialise(newSkillModel);
            actor.RemoveAllEffects<BuilderEditingEffect<ISkillDefinition>>();
            actor.AddEffect(new BuilderEditingEffect<ISkillDefinition>(actor) { EditingItem = newSkill });
            actor.OutputHandler.Send(
                $"You create the {newSkill.Name.TitleCase().ColourName()} skill, which you are now editing.");
        }
    }

    private static void SkillSet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<ISkillDefinition> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ISkillDefinition>>().FirstOrDefault();
        if (editing == null)
        {
            actor.OutputHandler.Send("You are not editing any skills.");
            return;
        }

        editing.EditingItem.BuildingCommand(actor, ss);
    }

    private static void SkillEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<ISkillDefinition> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ISkillDefinition>>().FirstOrDefault();
            if (editing == null)
            {
                actor.OutputHandler.Send("You are not editing any skills.");
                return;
            }

            actor.OutputHandler.Send(editing.EditingItem.Show(actor));
            return;
        }

        string actionText = ss.PopSpeech();
        if (actionText.EqualTo("new"))
        {
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("What name do you want to give to the new skill?");
                return;
            }

            string name = ss.PopSpeech().ToLowerInvariant().TitleCase();
            if (actor.Gameworld.Traits.Any(x => x.TraitType == TraitType.Skill && x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send(
                    $"There is already a skill called {name.ColourName()}. Skill name must be unique.");
                return;
            }

            using (new FMDB())
            {
                Models.TraitDefinition newSkillModel = new()
                {
                    Name = name,
                    Type = (int)TraitType.Skill,
                    DerivedType = 0,
                    TraitGroup = "General",
                    ChargenBlurb = string.Empty,
                    Hidden = false,
                    DecoratorId =
                        (actor.Gameworld.TraitDecorators.FirstOrDefault(x =>
                             x.Id == actor.Gameworld.GetStaticLong("DefaultSkillDecorator")) ??
                         actor.Gameworld.TraitDecorators.First()).Id,
                    ImproverId =
                        (actor.Gameworld.ImprovementModels.FirstOrDefault(x =>
                             x.Id == actor.Gameworld.GetStaticLong("DefaultSkillImprover")) ??
                         actor.Gameworld.ImprovementModels.First()).Id,
                    AvailabilityProgId = actor.Gameworld.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id,
                    TeachDifficulty = (int)Difficulty.VeryHard,
                    LearnDifficulty = (int)Difficulty.VeryHard,
                    TeachableProgId = actor.Gameworld.FutureProgs.First(x => x.FunctionName == "AlwaysFalse").Id,
                    LearnableProgId = actor.Gameworld.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id,
                    BranchMultiplier = 1.0,
                    Expression = new Models.TraitExpression { Name = $"{name} Cap", Expression = "70" }
                };
                FMDB.Context.TraitDefinitions.Add(newSkillModel);
                FMDB.Context.SaveChanges();

                actor.Gameworld.Add(new TraitExpression(newSkillModel.Expression, actor.Gameworld));
                SkillDefinition newSkill = new(newSkillModel, actor.Gameworld);
                actor.Gameworld.Add(newSkill);
                newSkill.Initialise(newSkillModel);
                actor.RemoveAllEffects<BuilderEditingEffect<ISkillDefinition>>();
                actor.AddEffect(new BuilderEditingEffect<ISkillDefinition>(actor) { EditingItem = newSkill });
                actor.OutputHandler.Send(
                    $"You create the {newSkill.Name.TitleCase().ColourName()} skill, which you are now editing.");
                return;
            }
        }

        if (actionText.EqualTo("close"))
        {
            BuilderEditingEffect<ISkillDefinition> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ISkillDefinition>>().FirstOrDefault();
            if (editing == null)
            {
                actor.OutputHandler.Send("You are not editing any skills.");
                return;
            }

            actor.RemoveEffect(editing);
            actor.OutputHandler.Send(
                $"You are no longer editing the {editing.EditingItem.Name.TitleCase().ColourName()} skill.");
            return;
        }

        if (actor.Gameworld.Traits.GetByIdOrName(actionText) is not ISkillDefinition skill)
        {
            actor.OutputHandler.Send("There is no such skill for you to edit.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<ISkillDefinition>>();
        actor.AddEffect(new BuilderEditingEffect<ISkillDefinition>(actor) { EditingItem = skill });
        actor.OutputHandler.Send($"You are now editing the {skill.Name.TitleCase().ColourName()} skill.");
    }

    private const string SetAttributeHelp =
        @"The #3setattribute#0 command sets the numeric value of an attribute on a visible character.

Use this for staff correction or testing when a character's attribute value needs to be set directly.

The target must currently have the attribute. Attribute names are matched by prefix from the target's attributes, and the value must be a valid number.

The syntax is as follows:
	#3setattribute <target> <attribute> <value>#0 - sets a character's attribute value";

    [PlayerCommand("SetAttribute", "setattribute")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("setattribute", SetAttributeHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void SetAttribute(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        ICharacter target = actor.TargetActor(ss.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anyone like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(new EmoteOutput(new Emote(
                $"Which attribute did you want to set a value for? $0 have|has the following: {target.TraitsOfType(TraitType.Attribute).Select(x => x.Definition.Name.ColourName()).ListToString()}.",
                target, target)));
            return;
        }

        string text = ss.PopSpeech();
        ITraitDefinition attribute = target.TraitsOfType(TraitType.Attribute).Select(x => x.Definition)
                              .FirstOrDefault(x =>
                                  x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
        if (attribute == null)
        {
            actor.OutputHandler.Send(new EmoteOutput(new Emote(
                $"$0 have|has no such attribute. $0 have|has the following: {target.TraitsOfType(TraitType.Attribute).Select(x => x.Definition.Name.ColourName()).ListToString()}.",
                target, target)));
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What value did you want to give to the attribute?");
            return;
        }

        if (!double.TryParse(ss.PopSpeech(), out double value))
        {
            actor.OutputHandler.Send("You must enter a valid number.");
            return;
        }

        target.SetTraitValue(attribute, value);
        actor.OutputHandler.Send(
            $"You set the value of {target.HowSeen(actor, type: DescriptionType.Possessive)} {attribute.Name.ColourName()} attribute to {value.ToString("N2", actor).ColourValue()} ({attribute.Decorator.Decorate(value)})");
    }

    private const string DecayHelp =
        @"The #3decay#0 command adds decay points to all corpses and severed body parts in your current room layer.

Use it without an amount to apply the default #31,000#0 decay points, or specify an amount for a stronger or weaker decay pass.

Only corpse and severed body part components in your current room layer are affected.

The syntax is as follows:
	#3decay#0 - adds 1,000 decay points
	#3decay <amount>#0 - adds a specified number of decay points";

    [PlayerCommand("Decay", "decay")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("decay", DecayHelp, AutoHelp.HelpArg)]
    protected static void Decay(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        double decay = 1000;
        if (!ss.IsFinished)
        {
            if (!double.TryParse(ss.PopSpeech(), out decay))
            {
                actor.Send("How much decay do you want to add to corpses in this room?");
                return;
            }
        }

        foreach (ICorpse item in
                 actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<ICorpse>()))
        {
            item.DecayPoints += decay;
        }

        foreach (ISeveredBodypart item in actor.Location.LayerGameItems(actor.RoomLayer)
                                  .SelectNotNull(x => x.GetItemType<ISeveredBodypart>()))
        {
            item.DecayPoints += decay;
        }

        actor.Send("You add {0:N} decay points to all corpses in the room.", decay);
    }

    private const string KillHelp =
        @"The #3kill#0 command immediately kills a visible character.

Use this only for direct staff intervention when a character should die immediately.

You must type the full #3kill#0 command word and supply a visible target. The command refuses self-targeting and active admin avatars.

The syntax is as follows:
	#3kill <target>#0 - immediately kills a visible non-admin target";

    [PlayerCommand("Kill", "kill")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("kill", KillHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Kill(ICharacter actor, string input)
    {
        StringStack ss = new(input);
        string cmd = ss.PopSpeech();
        if (!cmd.Equals("kill", StringComparison.InvariantCultureIgnoreCase))
        {
            actor.Send("You must type out kill in its entirety, to avoid accidents.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("Who do you want to kill?");
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        if (target == null)
        {
            actor.Send("There is no such person to kill.");
            return;
        }

        if (target == actor)
        {
            actor.Send("You cannot kill yourself.");
            return;
        }

        if (target.IsAdministrator())
        {
            actor.Send(StringUtilities.HMark + "You can't kill active admin avatars.");
            return;
        }

        target.Die();
    }

    private const string ResurrectHelp =
        @"The #3resurrect#0 command restores a dead player character from their final corpse or from an offline character ID.

Use the corpse form when the final corpse is present, or the #3*<id>#0 form when resurrecting an offline dead PC by character ID.

The corpse must represent the final death of the character, not ordinary remains. The offline form deletes existing final-death corpses for that character after resurrection. Player characters are unloaded after resurrection so they can log in normally.

The syntax is as follows:
	#3resurrect <corpse>#0 - resurrects the dead character represented by a final corpse
	#3resurrect *<character id>#0 - resurrects an offline dead player character by ID";

    [PlayerCommand("Resurrect", "resurrect")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("resurrect", ResurrectHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Resurrect(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.Send("Who do you want to resurrect?");
            return;
        }

        ICharacter character;
        string targetText = ss.PopSpeech();
        // Allow resurrection of offline PCs
        if (targetText[0] == '*')
        {
            if (!long.TryParse(targetText[1..], out long value))
            {
                actor.Send(
                    "You must specify a valid character ID to resurrect if you want to use this version of the command.");
                return;
            }

            character = actor.Gameworld.TryGetCharacter(value, true);
            if (character == null)
            {
                actor.OutputHandler.Send("There is no character with that ID.");
                return;
            }

            if (!character.State.HasFlag(CharacterState.Dead))
            {
                actor.OutputHandler.Send(
                    $"{character.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} ({character.PersonalName.GetName(NameStyle.FullName)}) is not dead.");
                return;
            }

            character.Resurrect(actor.Location);

            // First, check to see if there are any corpses of this character already in the world
            foreach (ICorpse corpseitem in actor.Gameworld.Items.SelectNotNull(x => x.GetItemType<ICorpse>()).ToList())
            {
                if (!corpseitem.RepresentsFinalCharacterDeath || corpseitem.OriginalCharacter.Id != value)
                {
                    continue;
                }

                corpseitem.Parent.Delete();
            }
        }
        else
        {
            IGameItem item = actor.TargetItem(targetText);
            if (item == null)
            {
                actor.Send("There is no such corpse to resurrect.");
                return;
            }

            ICorpse corpse = item.GetItemType<ICorpse>();
            if (corpse == null)
            {
                actor.Send("{0} is not a corpse.", item.HowSeen(actor, true));
                return;
            }

            if (!corpse.RepresentsFinalCharacterDeath)
            {
                actor.Send("{0} is body remains rather than the final corpse of a dead character.", item.HowSeen(actor, true));
                return;
            }

            character = corpse.OriginalCharacter.Resurrect(actor.Location);
            item.Delete();
        }

        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ resurrect|resurrects $0.", actor, character),
            flags: OutputFlags.SuppressObscured));
        if (character.Account.Id != 0)
        {
            character.Quit(); //Unload PCs so they can log in w/o a clone.
        }

        IBoard deathBoard = actor.Gameworld.Boards.Get(actor.Gameworld.GetStaticLong("DeathsBoardId"));
        if (deathBoard != null)
        {
            IFutureProg deathProg = actor.Gameworld.FutureProgs.Get(actor.Gameworld.GetStaticLong("PostToDeathsProg"));
            if (deathProg?.ExecuteBool(character) != false)
            {
                deathBoard.MakeNewPost(default(IAccount),
                    $"{CharacterInstanceIdentityComparer.IdentityId(character)} - {character.PersonalName.GetName(NameStyle.FullWithNickname)} Resurrected by {actor.Account.Name.Proper()}",
                    $"Character #{CharacterInstanceIdentityComparer.IdentityId(character)} ({character.PersonalName.GetName(NameStyle.FullWithNickname)}) was resurrected by {actor.Account.Name.Proper()}."
                );
            }
        }
    }

    private const string PossessHelp =
        @"The #3possess#0 command transfers your control into a visible NPC.

Use this when you need to directly roleplay or operate an NPC. Use #3return#0 while possessing to return to your original body.

You can only possess NPC targets, and only one staff member can possess an NPC at a time. You cannot start a new possession while already possessing. Admin telepathy, admin sight and admin spy effects are copied to the possessed NPC where relevant.

The syntax is as follows:
	#3possess <npc>#0 - takes control of a visible NPC";

    [PlayerCommand("Possess", "possess")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("possess", PossessHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Possess(ICharacter actor, string input)
    {
        if (actor.EffectsOfType<Switched>().Any())
        {
            actor.Send("You are already possessing an NPC. You must return before you can possess another.");
            return;
        }

        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.Send("Who do you want to possess?");
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        if (target == null)
        {
            actor.Send("You don't see anyone like that to possess.");
            return;
        }

        if (target == actor)
        {
            actor.Send("You cannot possess yourself.");
            return;
        }

        if (!CharacterInstanceService.CanStaffPossessNpcTarget(target))
        {
            actor.Send("You can only possess NPCs.");
            return;
        }

        if (target.EffectsOfType<Switched>().Any())
        {
            actor.Send(
                $"{target.HowSeen(actor, true)} is already being possessed by someone. Only one possessor at a time.");
            return;
        }

        // Preserve certain admin affects
        if (actor.AffectedBy<AdminTelepathy>())
        {
            target.AddEffect(new AdminTelepathy(target));
        }

        if (actor.AffectedBy<AdminSight>())
        {
            target.AddEffect(new AdminSight(target));
        }

        if (actor.AffectedBy<AdminSpyMaster>())
        {
            target.AddEffect(new AdminSpyMaster(target, actor.EffectsOfType<AdminSpyMaster>().First()));
        }

        // End admin effects

        actor.OutputHandler.Send(
            $"You take control of {target.HowSeen(actor, type: DescriptionType.Possessive)} mind.");
        actor.Controller.SetContext(target);
        target.AddEffect(new Switched(target, actor));
        actor.SetNoControllerTags(" (switched)");
    }

    private const string GiveInvisHelp =
        @"The #3giveinvis#0 command adds or removes a prog-controlled invisibility effect from a perceivable target.

Use this for targeted storyteller invisibility where a FutureProg decides whether the invisibility applies to a perceiver and target.

The prog must return boolean and accept two perceivable parameters. #3giveinvis clear#0 removes all basic invisibility effects from the target; it does not remove admin invisibility.

The syntax is as follows:
	#3giveinvis <target> <prog>#0 - adds prog-controlled invisibility to a target
	#3giveinvis clear <target>#0 - removes basic invisibility effects from a target";

    [PlayerCommand("GiveInvis", "giveinvis")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("giveinvis", GiveInvisHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void GiveInvis(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        bool clear = false;
        if (ss.Peek().EqualTo("clear"))
        {
            clear = true;
            ss.PopSpeech();
        }

        if (ss.IsFinished)
        {
            actor.Send(clear
                ? "What do you want to clear invisibility from?"
                : "What do you want to give invisibility to?");
            return;
        }

        IPerceivable target = actor.Target(ss.PopSpeech());
        if (target == null)
        {
            actor.Send("You don't see anything like that.");
            return;
        }

        if (clear)
        {
            target.RemoveAllEffects(x => x.IsEffectType<Invis>());
            actor.Send($"You remove all invisibility effects from {target.HowSeen(actor)}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send(
                "Which prog do you want to attach to this invisibility? It should return bool and accept two perceivables as parameters.");
            return;
        }

        string name = ss.SafeRemainingArgument;
        IFutureProg prog = long.TryParse(name, out long value)
            ? actor.Gameworld.FutureProgs.Get(value)
            : actor.Gameworld.FutureProgs.GetByName(name);
        if (prog == null)
        {
            actor.Send("There is no prog like that.");
            return;
        }

        if (prog.ReturnType != ProgVariableTypes.Boolean)
        {
            actor.Send("The prog you select must return boolean.");
            return;
        }

        if (!prog.MatchesParameters(new[]
            {
                ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
            }))
        {
            actor.Send("The prog must accept two perceivable parameters.");
            return;
        }

        actor.Send(
            $"You add invisibility to {target.HowSeen(actor)} with the prog {prog.FunctionName} (#{prog.Id}) controlling whether it applies.");
        target.AddEffect(new Invis(target, prog));
    }

    private const string GiveEmpathyHelp =
        @"The #3giveempathy#0 command grants a visible character the persistent empathy effect.

Use this when staff need to give a character the ability to sense feelings in and around their current room.

The command refuses targets who already have empathy. The effect is saving and remains on the target until removed by other staff tooling or effect cleanup.

The syntax is as follows:
	#3giveempathy <target>#0 - gives empathy to a visible character";

    [PlayerCommand("GiveEmpathy", "giveempathy")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("giveempathy", GiveEmpathyHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void GiveEmpathy(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Who do you want to give the gift of empathy to?");
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anyone like that here.");
            return;
        }

        if (target.EffectsOfType<Empathy>().Any())
        {
            actor.OutputHandler.Send(
                $"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} already has the gift of empathy.");
            return;
        }

        target.AddEffect(new Empathy(target));
        actor.OutputHandler.Send(
            $"You give {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} the gift of empathy.");
    }

    private const string MeritSearchHelp =
        @"The #3meritsearch#0 command searches online player characters by merit and flaw.

Specify one or more merits or flaws by ID or name. The result shows online characters who have every specified merit or flaw.

Multiple arguments are combined as an AND search, not OR. The search checks currently loaded player characters.

The syntax is as follows:
	#3meritsearch <merit|flaw> [<merit|flaw>]...#0 - finds online characters with all specified merits or flaws";

    [PlayerCommand("MeritSearch", "meritsearch")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("meritsearch", MeritSearchHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void MeritSearch(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());

        List<IMerit> merits = new();
        while (!ss.IsFinished)
        {
            IMerit merit = long.TryParse(ss.PopSpeech(), out long value)
                ? actor.Gameworld.Merits.Get(value)
                : actor.Gameworld.Merits.GetByName(ss.Last);
            if (merit == null)
            {
                actor.Send($"There is no such merit or flaw as '{ss.Last}'.");
                return;
            }

            merits.Add(merit);
        }

        List<ICharacter> characters = actor.Gameworld.Characters.Where(x => merits.All(y => x.Merits.Contains(y))).ToList();
        if (!characters.Any())
        {
            actor.Send(
                $"There aren't any online characters with all of the following merits and flaws: {merits.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine(
            $"The following online characters have the merits and flaws {merits.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}:");
        foreach (ICharacter ch in characters)
        {
            sb.AppendLine($"\t{ch.HowSeen(actor)} ({ch.PersonalName.GetName(NameStyle.FullWithNickname)})");
        }

        actor.Send(sb.ToString());
    }

    private const string RoleSearchHelp =
        @"The #3rolesearch#0 command searches online player characters by chargen role.

Specify one or more roles by ID or name. The result shows online characters who have every specified role.

Multiple arguments are combined as an AND search, not OR. The search checks currently loaded player characters.

The syntax is as follows:
	#3rolesearch <role> [<role>]...#0 - finds online characters with all specified roles";

    [PlayerCommand("RoleSearch", "rolesearch")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("rolesearch", RoleSearchHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void RoleSearch(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (ss.IsFinished || ss.Peek().EqualToAny("help", "?"))
        {
            actor.Send(
                $"The RoleSearch command is used to search for online character with particular roles. The syntax is {"rolesearch <role1> <role2> ...".ColourCommand()}. If you specify multiple roles, it will search for only those characters who have ALL of them.");
            return;
        }

        List<IChargenRole> roles = new();
        while (!ss.IsFinished)
        {
            IChargenRole role = long.TryParse(ss.PopSpeech(), out long value)
                ? actor.Gameworld.Roles.Get(value)
                : actor.Gameworld.Roles.GetByName(ss.Last);
            if (role == null)
            {
                actor.Send($"There is no such role as '{ss.Last}'.");
                return;
            }

            roles.Add(role);
        }

        List<ICharacter> characters = actor.Gameworld.Characters.Where(x => roles.All(y => x.Roles.Contains(y))).ToList();
        if (!characters.Any())
        {
            actor.Send(
                $"There aren't any online characters with all of the roles {roles.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine(
            $"The following online characters have the roles {roles.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}:");
        foreach (ICharacter ch in characters)
        {
            sb.AppendLine($"\t{ch.HowSeen(actor)} ({ch.PersonalName.GetName(NameStyle.FullWithNickname)})");
        }

        actor.Send(sb.ToString());
    }

    private const string LoadPCHelp =
        @"The #3loadpc#0 command loads an offline player character into the world at your location.

Use this when staff need an offline PC present for maintenance, recovery, or setup work such as giving them an item before their next login.

The character must be a non-guest player character and must not already be loaded. Loading them has real world effects: their health model runs, hunger and thirst can change, and they can be affected by spells or other systems. When finished, use #3force <target> quit#0 or another appropriate cleanup path to unload them.
Use #3show account <account>#0 to find character IDs.

The syntax is as follows:
	#3loadpc <character id>#0 - loads an offline player character by ID";

    [PlayerCommand("LoadPC", "loadpc")]
    [CommandPermission(PermissionLevel.SeniorAdmin)]
    [HelpInfo("loadpc", LoadPCHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void LoadPC(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());

        if (!long.TryParse(ss.PopSpeech(), out long value))
        {
            actor.OutputHandler.Send("You must enter the ID number of a character that you want to load.");
            return;
        }

        ICharacter character = actor.Gameworld.TryGetCharacter(value, true);
        if (character == null)
        {
            actor.OutputHandler.Send("There is no such character to load.");
            return;
        }

        if (!character.IsPlayerCharacter || character.IsGuest)
        {
            actor.OutputHandler.Send("You cannot use this command to load guests or NPCs.");
            return;
        }

        if (actor.Gameworld.Characters.Has(character))
        {
            actor.OutputHandler.Send(
                $"{character.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} ({character.PersonalName.GetName(NameStyle.FullName)}) is already in the gameworld at {character.Location.HowSeen(actor)} (#{character.Location.Id.ToString("N0", actor)}), so cannot be loaded.");
            return;
        }

        actor.Gameworld.SystemMessage(
            new EmoteOutput(new Emote(
                $"@ offline load|loads $0 ({character.PersonalName.GetName(NameStyle.FullName)}, account {character.Account.Name.Proper()})",
                actor, character)), true);
        character.Register(new NonPlayerOutputHandler());
        actor.Gameworld.Add(character, false);
        NPCController controller = new();
        controller.UpdateControlFocus(character);
        character.SilentAssumeControl(controller);
        actor.Location.Login(character);
    }

    private const string OverrideDescHelp =
        @"The #3overridedesc#0 command adds or clears a temporary storyteller description override on a perceivable target.

Use #3sdesc#0 to override the short description, #3desc#0 to override the full description, or #3clear#0 to remove storyteller description overrides.

For characters, the effect is applied to the body. Quote multi-word override text so it is consumed as one command argument.

The syntax is as follows:
	#3overridedesc <target> sdesc ""<description>""#0 - overrides the target's short description
	#3overridedesc <target> desc ""<description>""#0 - overrides the target's full description
	#3overridedesc <target> clear#0 - removes storyteller description overrides";

    [PlayerCommand("OverrideDesc", "overridedesc")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("overridedesc", OverrideDescHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void OverrideDesc(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        IPerceivable target = actor.Target(ss.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anything like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("You must specify SDESC, DESC or CLEAR as the second argument.");
            return;
        }

        DescriptionType type;
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "desc":
                type = DescriptionType.Full;
                break;
            case "sdesc":
                type = DescriptionType.Short;
                break;
            case "clear":
                if (target is ICharacter tch)
                {
                    tch.Body.RemoveAllEffects(x => x.IsEffectType<StorytellerDescOverride>());
                }
                else
                {
                    target.RemoveAllEffects(x => x.IsEffectType<StorytellerDescOverride>());
                }

                actor.OutputHandler.Send(
                    $"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is now back to {target.ApparentGender(actor).Possessive()} regular descriptions.");
                return;
            default:
                actor.OutputHandler.Send("You must specify SDESC, DESC or CLEAR as the second argument.");
                return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What custom override description do you want to set?");
            return;
        }

        string olddesc = target.HowSeen(actor, true, type, flags: PerceiveIgnoreFlags.IgnoreSelf);
        if (target is ICharacter ch)
        {
            ch.Body.RemoveAllEffects(x => x.GetSubtype<StorytellerDescOverride>()?.OverridenType == type);
            ch.Body.AddEffect(new StorytellerDescOverride(target, type, ss.PopSpeech()));
        }
        else
        {
            target.RemoveAllEffects(x => x.GetSubtype<StorytellerDescOverride>()?.OverridenType == type);
            target.AddEffect(new StorytellerDescOverride(target, type, ss.PopSpeech()));
        }

        if (type == DescriptionType.Short)
        {
            actor.OutputHandler.Send(
                $"You override the short description of {olddesc} to {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}");
        }
        else
        {
            actor.OutputHandler.Send(
                $"You override the full description of {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} to {ss.Last.Fullstop().ColourCommand()}");
        }
    }

    private const string RenameHelp =
        @"The #3rename#0 command changes a character's true personal name.

Use a visible target or character ID, then provide the new name in the format required by that character's naming culture.

The new name is parsed through the character's ethnicity name culture first, then their culture name culture. Invalid names for that culture are refused.

The syntax is as follows:
	#3rename <target> <new name>#0 - renames a visible character
	#3rename <character id> <new name>#0 - renames a loaded or offline character by ID";

    [PlayerCommand("Rename", "rename")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("rename", RenameHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Rename(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string targetText = ss.PopSpeech();
        ICharacter target = null;
        if (long.TryParse(targetText, out long value))
        {
            target = actor.Gameworld.TryGetCharacter(value, true);
        }
        else
        {
            target = actor.TargetActor(targetText);
        }

        if (target == null)
        {
            actor.OutputHandler.Send("There is no character like that to rename.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What should their new name be?");
            return;
        }

        INameCulture nc = target.Ethnicity.NameCultureForGender(target.Gender.Enum) ?? target.Culture.NameCultureForGender(target.Gender.Enum);
        IPersonalName newName = nc.GetPersonalName(ss.SafeRemainingArgument, true);
        if (newName == null)
        {
            actor.OutputHandler.Send($"That is not a valid name for their naming culture ({nc.Name.ColourValue()}).");
            return;
        }

        target.PersonalName = newName;
        actor.OutputHandler.Send(
            $"You rename {target.HowSeen(actor)} to {newName.GetName(NameStyle.FullWithNickname)}.");
    }

    private const string RedescHelp =
        @"The #3redesc#0 command edits a character or corpse's full description in the text editor.

Use it on a visible character or corpse, review the current description and description-pattern guidance, then enter the replacement text in the editor.

The target must be visible as a character or corpse. The entered description replaces the current full description.

The syntax is as follows:
	#3redesc <target>#0 - edits a target's full description";

    [PlayerCommand("Redesc", "redesc")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("redesc", RedescHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Redesc(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        ICharacter target = actor.TargetActorOrCorpse(ss.PopSpeech());
        if (target is null)
        {
            actor.OutputHandler.Send("You don't see anyone like that.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Editing the description for {target.HowSeen(actor)}.");
        sb.AppendLine();
        sb.AppendLine("Replacing:\n");
        sb.AppendLine(target.Body.GetRawDescriptions.FullDescription.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
        sb.AppendLine();
        sb.AppendLine(EntityDescriptionPatternExtensions.GetDescriptionHelpFor(target, actor));
        sb.AppendLine("Enter the description in the editor below.");
        sb.AppendLine();
        actor.OutputHandler.Send(sb.ToString());
        actor.EditorMode(postAction: RedescPostAction, RedescCancelAction, 1.0, null, EditorOptions.None, new object[] { target, actor });
    }

    private static void RedescCancelAction(IOutputHandler handler, object[] args)
    {
        handler.Send("You decide not to change the description.");
    }

    private static void RedescPostAction(string text, IOutputHandler handler, object[] args)
    {
        ICharacter target = (ICharacter)args[0];
        ICharacter actor = (ICharacter)args[1];
        target.Body.SetFullDescription(text);
        handler.Send($"You change the description of {target.HowSeen(actor)} to:\n\n{text.ColourCommand()}");
    }

    private const string ResdescHelp =
        @"The #3resdesc#0 command edits a character or corpse's short description in the text editor.

Use it on a visible character or corpse, review the current short description and description-pattern guidance, then enter the replacement text in the editor.

The target must be visible as a character or corpse. The entered description replaces the current short description.

The syntax is as follows:
	#3resdesc <target>#0 - edits a target's short description";

    [PlayerCommand("Resdesc", "resdesc")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("resdesc", ResdescHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Resdesc(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        ICharacter target = actor.TargetActorOrCorpse(ss.PopSpeech());
        if (target is null)
        {
            actor.OutputHandler.Send("You don't see anyone like that.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Editing the short description for {target.HowSeen(actor)}.");
        sb.AppendLine();
        sb.AppendLine("Replacing:\n");
        sb.AppendLine(target.Body.GetRawDescriptions.ShortDescription.ColourCommand());
        sb.AppendLine();
        sb.AppendLine(EntityDescriptionPatternExtensions.GetDescriptionHelpFor(target, actor));
        sb.AppendLine("Enter the description in the editor below.");
        sb.AppendLine();
        actor.OutputHandler.Send(sb.ToString());
        actor.EditorMode(postAction: ResdescPostAction, ResdescCancelAction, 1.0, null, EditorOptions.None, new object[] { target, actor, target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf) });
    }

    private static void ResdescCancelAction(IOutputHandler handler, object[] args)
    {
        handler.Send("You decide not to change the short description.");
    }

    private static void ResdescPostAction(string text, IOutputHandler handler, object[] args)
    {
        ICharacter target = (ICharacter)args[0];
        ICharacter actor = (ICharacter)args[1];
        string old = (string)args[2];
        target.Body.SetShortDescription(text);
        handler.Send($"You change the short description of {old} to {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}.");
    }

    private const string SniffHelp =
        @"The #3sniff#0 command shows debug information about world objects and locations.

Use it to inspect effects, hooks, variable-register values and other diagnostic details for rooms, zones, shards, exits, characters and items.

Use #3*<direction>#0 to inspect an exit from your current room, such as #3sniff *north#0. Character and item targets are resolved through normal targeting.

The syntax is as follows:
	#3sniff here#0 - sniffs your current cell
	#3sniff zone#0 - sniffs your current zone
	#3sniff shard#0 - sniffs your current shard
	#3sniff *<direction>#0 - sniffs an exit
	#3sniff <target>#0 - sniffs a character or item";

    [PlayerCommand("Sniff", "sniff")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("sniff", SniffHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Sniff(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string cmd = ss.SafeRemainingArgument;
        if (cmd.EqualTo("here"))
        {
            SniffRoom(actor);
            return;
        }

        if (cmd.EqualTo("zone"))
        {
            SniffZone(actor);
            return;
        }

        if (cmd.EqualTo("shard"))
        {
            SniffShard(actor);
            return;
        }

        if (cmd.Length > 1 && cmd[0] == '*')
        {
            ICellExit targetExit = actor.Location.GetExitKeyword(cmd[1..], actor);
            if (targetExit == null)
            {
                actor.Send("There is no such exit for you to sniff.");
                return;
            }

            SniffExit(actor, targetExit, ss);
            return;
        }

        IPerceivable target = actor.Target(cmd);
        if (target == null)
        {
            actor.Send("There is nothing like that for you to sniff.");
            return;
        }

        if (target is ICharacter ch)
        {
            SniffCharacter(actor, ch, ss);
            return;
        }

        if (target is IGameItem gi)
        {
            SniffGameItem(actor, gi, ss);
            return;
        }

        throw new NotImplementedException("Unknown target type in sniff.");
    }

    private static void SniffShard(ICharacter actor)
    {
        IShard shard = actor.Location.Zone.Shard;
        StringBuilder sb = new();
        sb.AppendLine($"Sniffing Shard {shard.Id}...");

        sb.AppendLine();
        sb.AppendLine("Effects:");
        sb.AppendLine();
        foreach (IEffect effect in shard.Effects)
        {
            sb.AppendLine($"\t{effect.Describe(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine("Hooks:");
        sb.AppendLine();
        foreach (IHook hook in shard.Hooks)
        {
            sb.AppendLine($"\t#{hook.Id.ToString("N0", actor)}) {hook.Name.ColourName()} | {hook.Type.DescribeEnum(colour: Telnet.Green)} | {hook.InfoForHooklist}");
        }

        sb.AppendLine();
        sb.AppendLine("Variables:");
        sb.AppendLine();
	foreach (Tuple<string, ProgVariableTypes> variable in actor.Gameworld.VariableRegister.AllVariables(ProgVariableTypes.Shard))
        {
            IProgVariable value = actor.Gameworld.VariableRegister.GetValue(shard, variable.Item1);
            sb.AppendLine(
                $"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
        }

        actor.Send(sb.ToString());
    }

    private static void SniffZone(ICharacter actor)
    {
        IZone zone = actor.Location.Zone;
        StringBuilder sb = new();
        sb.AppendLine($"Sniffing Zone {zone.Id}...");

        sb.AppendLine();
        sb.AppendLine("Effects:");
        sb.AppendLine();
        foreach (IEffect effect in zone.Effects)
        {
            sb.AppendLine($"\t{effect.Describe(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine("Hooks:");
        sb.AppendLine();
        foreach (IHook hook in zone.Hooks)
        {
            sb.AppendLine($"\t#{hook.Id.ToString("N0", actor)}) {hook.Name.ColourName()} | {hook.Type.DescribeEnum(colour: Telnet.Green)} | {hook.InfoForHooklist}");
        }

        sb.AppendLine();
        sb.AppendLine("Variables:");
        sb.AppendLine();
        foreach (Tuple<string, ProgVariableTypes> variable in actor.Gameworld.VariableRegister.AllVariables(ProgVariableTypes.Zone))
        {
            IProgVariable value = actor.Gameworld.VariableRegister.GetValue(zone, variable.Item1);
            sb.AppendLine(
                $"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
        }

        actor.Send(sb.ToString());
    }

    private static void SniffGameItem(ICharacter actor, IGameItem gi, StringStack ss)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Sniffing Item {gi.Id.ToString("N0", actor)}...");
        sb.AppendLine();
        sb.AppendLine($"Desc: {gi.HowSeen(actor)}");
        sb.AppendLine(
            $"Proto: {$"{gi.Prototype.Id.ToString("N0", actor)}r{gi.Prototype.RevisionNumber.ToString("N0", actor)}".ColourValue()}");
        sb.AppendLine($"Quality: {gi.Quality.Describe().ColourValue()}");
        sb.AppendLine($"Size: {gi.Size.Describe().ColourValue()}");
        sb.AppendLine($"Material: {gi.Material.Name.ColourValue()}");
        sb.AppendLine(
            $"Weight: {actor.Gameworld.UnitManager.DescribeMostSignificantExact(gi.Weight, Framework.Units.UnitType.Mass, actor).ColourValue()}");
        sb.AppendLine($"Quantity: {gi.Quantity.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Condition: {gi.Condition.ToString("P2", actor).ColourValue()}");
        sb.AppendLine($"Position: {gi.PositionState.GetType().Name.ColourValue()}");
        sb.AppendLine();
        sb.AppendLine($"InInventoryOf: {gi.InInventoryOf?.Actor.HowSeen(actor) ?? "Noone".Colour(Telnet.Red)}");
        sb.AppendLine($"ContainedIn: {gi.ContainedIn?.HowSeen(actor) ?? "Nothing".Colour(Telnet.Red)}");
        IFluid fluid = gi.TrueLocations.FirstOrDefault()?.Terrain(gi).WaterFluid ?? actor.Gameworld.Liquids.First();
        sb.AppendLine($"Buoyancy: {gi.Buoyancy(fluid.Density).ToString("N0", actor).ColourValue()}");
        if (gi.MorphTime != default)
        {
            sb.AppendLine($"Morphs In: {(gi.MorphTime - DateTime.UtcNow).Describe(actor).ColourValue()}");
            sb.AppendLine($"Morph Time: {gi.MorphTime.GetLocalDateString(actor).ColourValue()}");
        }
        else if (gi.CachedMorphTime is not null)
        {
            sb.AppendLine($"Cached Morph Time: {gi.CachedMorphTime.Value.DescribePreciseBrief(actor).ColourValue()}");
        }

        sb.AppendLine();
        sb.AppendLine("Components:");
        foreach (IGameItemComponent component in gi.Components)
        {
            sb.AppendLine(
                $"\t##{component.Id.ToString("N0", actor)} (proto #2{component.Prototype.Id.ToString("N0", actor)}r{component.Prototype.RevisionNumber.ToString("N0", actor)}#0) - #5{component.Prototype.Name}#0".SubstituteANSIColour());
        }

        sb.AppendLine();
        sb.AppendLine($"Attached Items:");
        sb.AppendLine();
        foreach (IGameItem item in gi.AttachedAndConnectedItems)
        {
            sb.AppendLine($"\tItem #{item.Id.ToString("N0", actor)} - {item.HowSeen(actor)}");
        }

        IVariable itemVariable = gi.GetItemType<IVariable>();
        if (itemVariable is not null)
        {
            sb.AppendLine();
            sb.AppendLine("Characteristics:");
            sb.AppendLine();
            foreach (ICharacteristicDefinition item in itemVariable.CharacteristicDefinitions)
            {
                sb.AppendLine($"\t{item.Name.ColourName()} => {itemVariable.GetCharacteristic(item).GetValue.ColourValue()}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("Effects:");
        sb.AppendLine();
        foreach (IEffect effect in gi.Effects)
        {
            sb.AppendLine($"\t{effect.Describe(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine("Hooks:");
        sb.AppendLine();
        foreach (IHook hook in gi.Hooks)
        {
            sb.AppendLine($"\t#{hook.Id.ToString("N0", actor)}) {hook.Name.ColourName()} | {hook.Type.DescribeEnum(colour: Telnet.Green)} | {hook.InfoForHooklist}");
        }

        sb.AppendLine();
        sb.AppendLine("Variables:");
        sb.AppendLine();
        foreach (Tuple<string, ProgVariableTypes> variable in actor.Gameworld.VariableRegister.AllVariables(ProgVariableTypes.Item))
        {
            IProgVariable value = actor.Gameworld.VariableRegister.GetValue(gi, variable.Item1);
            sb.AppendLine(
                $"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
        }

        actor.Send(sb.ToString());
    }

    private static void SniffCharacter(ICharacter actor, ICharacter ch, StringStack ss)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Sniffing Character #{ch.Id.ToString("N0", actor)}...");
        sb.AppendLine($"True Name: {ch.PersonalName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}");
        sb.AppendLine($"Current Name: {ch.CurrentName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}");
        sb.AppendLine(
            $"Aliases: {ch.Aliases.Where(x => x != ch.CurrentName).Select(x => x.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)).ListToString()}");
        sb.AppendLine($"Description: {ch.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}");
        sb.AppendLine($"Account: {ch.Account.Name.Colour(Telnet.Green)}");
        sb.AppendLine($"IsHelpless: {ch.IsHelpless.ToString(actor).Colour(Telnet.Green)}");
        sb.AppendLine($"State: {ch.State.Describe().Colour(Telnet.Cyan)}");
        sb.AppendLine($"Status: {ch.Status.Describe().Colour(Telnet.Cyan)}");
        sb.AppendLine($"Breathing: {ch.BreathingStrategy.GetType().Name.Colour(Telnet.Cyan)}");
        sb.AppendLine(
            $"Burden: Offense[{ch.CombatBurdenOffense.ToString("N2", actor).Colour(Telnet.Green)}]\tDefense[{ch.CombatBurdenDefense.ToString("N2", actor).Colour(Telnet.Green)}]");
        sb.AppendLine(
            $"Advantage: Offense[{ch.OffensiveAdvantage.ToString("N2", actor).Colour(Telnet.Green)}]\tDefense[{ch.DefensiveAdvantage.ToString("N2", actor).Colour(Telnet.Green)}]");
        IFluid fluid = ch.Location.Terrain(ch).WaterFluid ?? actor.Gameworld.Liquids.First();
        sb.AppendLine(
            $"Inventory Buoyancy: {ch.Body.AllItems.Sum(x => x.Buoyancy(fluid.Density)).ToString("N0", actor).ColourValue()}");
        sb.AppendLine();
        sb.AppendLine("Effects:");
        sb.AppendLine();
        foreach (IEffect effect in ch.Effects)
        {
            sb.AppendLine($"\t{effect.Describe(actor)}");
        }

        if (ch is INPC npc)
        {
            sb.AppendLine();
            sb.AppendLine("AIs:");
            sb.AppendLine();
            foreach (IArtificialIntelligence ai in npc.AIs)
            {
                sb.AppendLine($"\t#{ai.Id.ToStringN0(actor)}) {ai.Name.ColourName()}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("Hooks:");
        sb.AppendLine();
        foreach (IHook hook in ch.Hooks)
        {
            sb.AppendLine($"\t#{hook.Id.ToString("N0", actor)}) {hook.Name.ColourName()} | {hook.Type.DescribeEnum(colour: Telnet.Green)} | {hook.InfoForHooklist}");
        }

        sb.AppendLine();
        sb.AppendLine("Variables:");
        sb.AppendLine();
        foreach (Tuple<string, ProgVariableTypes> variable in actor.Gameworld.VariableRegister.AllVariables(ProgVariableTypes.Character))
        {
            IProgVariable value = actor.Gameworld.VariableRegister.GetValue(ch, variable.Item1);
            sb.AppendLine(
                $"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void SniffExit(ICharacter actor, ICellExit exit, StringStack ss)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Sniffing Exit {exit.Exit.Id.ToString("N0", actor).Colour(Telnet.Green)}...");
        sb.AppendLine(
            $"Origin: {exit.Origin.HowSeen(actor)} (#{exit.Origin.Id.ToString("N0", actor).Colour(Telnet.Green)})");
        sb.AppendLine(
            $"Destination: {exit.Destination.HowSeen(actor)} (#{exit.Destination.Id.ToString("N0", actor).Colour(Telnet.Green)})");
        sb.AppendLine($"Outbound Direction: {exit.OutboundDirection.Describe().Colour(Telnet.Yellow)}");
        sb.AppendLine($"Outbound Description: {exit.OutboundDirectionDescription.Colour(Telnet.Yellow)}");
        sb.AppendLine($"Outbound Suffix: {exit.OutboundDirectionSuffix.Colour(Telnet.Yellow)}");
        sb.AppendLine($"Outbound Movement: {exit.OutboundMovementSuffix.Colour(Telnet.Yellow)}");
        sb.AppendLine($"Inbound Direction: {exit.InboundDirection.Describe().Colour(Telnet.Yellow)}");
        sb.AppendLine($"Inbound Suffix: {exit.InboundDirectionSuffix.Colour(Telnet.Yellow)}");
        sb.AppendLine($"Inbound Movement: {exit.InboundMovementSuffix.Colour(Telnet.Yellow)}");
        sb.AppendLine();
        sb.AppendLine($"Accepts Door: {exit.Exit.AcceptsDoor.ToString(actor).Colour(Telnet.Cyan)}");
        sb.AppendLine($"Door Size: {exit.Exit.DoorSize.Describe().Colour(Telnet.Cyan)}");
        sb.AppendLine($"Door: {exit.Exit.Door?.Parent.HowSeen(actor) ?? "None"}");
        sb.AppendLine($"Max Size to Enter: {exit.Exit.MaximumSizeToEnter.Describe().Colour(Telnet.Cyan)}");
        sb.AppendLine(
            $"Max Size to Enter Upright: {exit.Exit.MaximumSizeToEnterUpright.Describe().Colour(Telnet.Cyan)}");
        sb.AppendLine();
        sb.AppendLine("Effects:");
        foreach (IEffect effect in exit.Exit.Effects)
        {
            sb.AppendLine($"\t{effect.Describe(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine("Hooks:");
        foreach (IHook hook in exit.Exit.Hooks)
        {
            sb.AppendLine($"\t#{hook.Id.ToString("N0", actor)}) {hook.Name.ColourName()} | {hook.Type.DescribeEnum(colour: Telnet.Green)} | {hook.InfoForHooklist}");
        }

        sb.AppendLine();
        sb.AppendLine("Variables:");
        foreach (Tuple<string, ProgVariableTypes> variable in actor.Gameworld.VariableRegister.AllVariables(ProgVariableTypes.Exit))
        {
            IProgVariable value = actor.Gameworld.VariableRegister.GetValue(exit, variable.Item1);
            sb.AppendLine(
                $"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
        }

        actor.Send(sb.ToString());
    }

    private static void SniffRoom(ICharacter actor)
    {
        ICell cell = actor.Location;
        StringBuilder sb = new();
        sb.AppendLine($"Sniffing Cell {cell.Id}...");
        sb.AppendLine(
            $"Room: {cell.Room.Id} - Zone: {cell.Room.Zone.Name} ({cell.Room.Zone.Id}) - Shard: {cell.Room.Zone.Shard.Name} ({cell.Room.Zone.Shard.Id})");
        sb.AppendLine(
            $"Current Overlay: {cell.CurrentOverlay.Id} from package {cell.CurrentOverlay.Package.Name.Colour(Telnet.Green)} ({cell.CurrentOverlay.Package.Id}r{cell.CurrentOverlay.Package.RevisionNumber})");
        sb.AppendLine("All overlays:");
        foreach (ICellOverlay overlay in cell.Overlays)
        {
            sb.AppendLine(
                $"\t{overlay.Id} - {overlay.Name.Colour(Telnet.Cyan)} - package {overlay.Package.Name.Colour(Telnet.Green)} ({overlay.Package.Id}r{overlay.Package.RevisionNumber})");
        }

        sb.AppendLine();
        sb.AppendLine("Exits:");
        foreach (ICellExit exit in actor.Gameworld.ExitManager.GetAllExits(cell)
                                  .OrderByDescending(x => cell.CurrentOverlay.ExitIDs.Contains(x.Exit.Id)))
        {
            sb.AppendLine(
                $"\t{exit.Exit.Id} - {exit.OutboundDirectionDescription} to {exit.Destination.CurrentOverlay.Name} ({exit.Destination.Id})");
            if (exit.Exit.AcceptsDoor)
            {
                if (exit.Exit.Door == null)
                {
                    sb.AppendLine($"\t\tAccepts {exit.Exit.DoorSize.Describe()} door");
                }
                else
                {
                    sb.AppendLine($"\t\tInstalled Door {exit.Exit.Door.Parent.HowSeen(actor)}");
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine("Tags:");
        foreach (ITag tag in cell.Tags)
        {
            sb.AppendLine($"\t[{tag.Id.ToString("N0", actor)}] {tag.FullName.Colour(Telnet.Cyan)}");
        }

        sb.AppendLine();
        sb.AppendLine("Effects:");
        foreach (IEffect effect in cell.Effects)
        {
            sb.AppendLine($"\t{effect.Describe(actor)}");
        }

        sb.AppendLine();
        sb.AppendLine("Hooks:");
        foreach (IHook hook in cell.Hooks)
        {
            sb.AppendLine($"\t#{hook.Id.ToString("N0", actor)}) {hook.Name.ColourName()} | {hook.Type.DescribeEnum(colour: Telnet.Green)} | {hook.InfoForHooklist}");
        }

        sb.AppendLine();
        sb.AppendLine("Variables:");
        foreach (Tuple<string, ProgVariableTypes> variable in actor.Gameworld.VariableRegister.AllVariables(ProgVariableTypes.Location))
        {
            IProgVariable value = actor.Gameworld.VariableRegister.GetValue(cell, variable.Item1);
            sb.AppendLine(
                $"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
        }

        actor.Send(sb.ToString());
    }

    private const string DrawingsHelp =
        @"The #3drawings#0 command lists immutable drawings in the game and can filter the result.

Use it without filters to list all drawings, or combine filters after the command to narrow by author and text.

Author filters can load character records and may be slow the first time they run. Prefer text filters first when you can reduce the set before filtering by author.

The syntax is as follows:
	#3drawings#0 - lists all drawings
	#3drawings by <character id>#0 - lists drawings by a character ID
	#3drawings by *<account>#0 - lists drawings by an account name
	#3drawings by ""<full name>""#0 - lists drawings by a character's full name
	#3drawings +<keyword>#0 - includes drawings whose short or full description contains a keyword
	#3drawings -<keyword>#0 - excludes drawings whose short or full description contains a keyword";

    [PlayerCommand("Drawings", "drawings")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("drawings", DrawingsHelp, AutoHelp.HelpArg)]
    protected static void Drawings(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        IEnumerable<IDrawing> drawings = actor.Gameworld.Drawings.AsEnumerable();
        while (!ss.IsFinished)
        {
            string text = ss.PopSpeech();

            if (text.EqualTo("by"))
            {
                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Show drawings by whom?");
                    return;
                }

                text = ss.PopSpeech();
                if (long.TryParse(text, out long value))
                {
                    drawings = drawings.Where(x => x.Author.Id == value);
                }
                else if (text[0] == '*' && text.Length > 1)
                {
                    drawings = drawings.Where(x => x.Author.Account.Name.EqualTo(text[1..]));
                }
                else
                {
                    drawings = drawings.Where(x => x.Author.PersonalName.GetName(NameStyle.FullName).EqualTo(text));
                }

                continue;
            }

            if (text[0] == '+' && text.Length > 1)
            {
                string search = text[1..];
                drawings = drawings.Where(x =>
                    x.FullDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                    x.ShortDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase));
            }

            if (text[0] == '-' && text.Length > 1)
            {
                string search = text[1..];
                drawings = drawings.Where(x =>
                    !x.FullDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase) &&
                    !x.ShortDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        actor.OutputHandler.Send(
            StringUtilities.GetTextTable(
                from drawing in drawings
                select new string[]
                {
                    drawing.Id.ToString("N0", actor),
                    drawing.Author.PersonalName.GetName(NameStyle.FullName),
                    drawing.ImplementType.Describe(),
                    drawing.DrawingSize.DescribeEnum(true),
                    drawing.ShortDescription
                },
                new string[]
                {
                    "ID",
                    "Author",
                    "Implement",
                    "Size",
                    "Short Description"
                },
                actor.LineFormatLength,
                colour: Telnet.Cyan,
                unicodeTable: actor.Account.UseUnicode)
        );
    }

    private const string WritingsHelp =
        @"The #3writings#0 command lists immutable writings in the game and can filter the result.

Use it without filters to list all writings, or combine filters after the command to narrow by author, text, language and script.

Author filters can load character records and may be slow the first time they run. Prefer text, language or script filters first when you can reduce the set before filtering by author.

The syntax is as follows:
	#3writings#0 - lists all writings
	#3writings by <character id>#0 - lists writings by a character ID
	#3writings by *<account>#0 - lists writings by an account name
	#3writings by ""<full name>""#0 - lists writings by a character's full name
	#3writings +<keyword>#0 - includes writings whose parsed text contains a keyword
	#3writings -<keyword>#0 - excludes writings whose parsed text contains a keyword
	#3writings &<language>#0 - lists only writings in a language
	#3writings $<script>#0 - lists only writings in a script";

    [PlayerCommand("Writings", "writings")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("writings", WritingsHelp, AutoHelp.HelpArg)]
    protected static void Writings(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        IEnumerable<IWriting> writings = actor.Gameworld.Writings.AsEnumerable();
        while (!ss.IsFinished)
        {
            string text = ss.PopSpeech();

            if (text.EqualTo("by"))
            {
                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Show writings by whom?");
                    return;
                }

                text = ss.PopSpeech();
                if (long.TryParse(text, out long value))
                {
                    writings = writings.Where(x => x.Author?.Id == value);
                }
                else if (text[0] == '*' && text.Length > 1)
                {
                    writings = writings.Where(x => x.Author?.Account.Name.EqualTo(text[1..]) == true);
                }
                else
                {
                    writings = writings.Where(x => x.Author?.PersonalName.GetName(NameStyle.FullName).EqualTo(text) == true);
                }

                continue;
            }

            if (text[0] == '+' && text.Length > 1)
            {
                string search = text[1..];
                writings = writings.Where(x =>
                    x.ParseFor(actor).Contains(search, StringComparison.InvariantCultureIgnoreCase));
            }

            if (text[0] == '-' && text.Length > 1)
            {
                string search = text[1..];
                writings = writings.Where(x =>
                    !x.ParseFor(actor).Contains(search, StringComparison.InvariantCultureIgnoreCase));
            }

            if (text[0] == '&' && text.Length > 1)
            {
                string search = text[1..];
                writings = writings.Where(x => x.Language.Name.EqualTo(search));
            }

            if (text[0] == '$' && text.Length > 1)
            {
                string search = text[1..];
                writings = writings.Where(x => x.Script.Name.EqualTo(search));
            }
        }

        actor.OutputHandler.Send(
            StringUtilities.GetTextTable(
                from writing in writings
                select new string[]
                {
                    writing.Id.ToString("N0", actor),
                    writing.Author?.PersonalName.GetName(NameStyle.FullName) ?? "Printed/Anonymous",
                    writing.Language.Name,
                    writing.Script.Name,
                    writing.ImplementType.Describe(),
                    writing.Style.DescribeEnum(),
                    writing.DocumentLength.ToString("N0")
                },
                new string[]
                {
                    "ID",
                    "Author",
                    "Language",
                    "Script",
                    "Implement",
                    "Style",
                    "Length"
                },
                actor.LineFormatLength,
                colour: Telnet.Cyan,
                unicodeTable: actor.Account.UseUnicode)
        );
    }

    private const string WritingHelp =
        @"The #3writing#0 command views immutable writing records and copies them onto writable items.

Use #3writing show#0 to inspect a writing's metadata and parsed text, or #3writing copy#0 to add an immutable reference to a writable item's current writable section.

Copying does not duplicate or edit the writing record; it references the existing immutable writing. The writable must have enough capacity in its current section.

The syntax is as follows:
	#3writing show <id>#0 - shows a writing record
	#3writing view <id>#0 - alias for #3writing show#0
	#3writing copy <id> <writable>#0 - adds an immutable writing reference to a writable item";

    [PlayerCommand("Writing", "writing")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("writing", WritingHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Writing(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "show":
            case "view":
                WritingShow(actor, ss);
                return;
            case "copy":
                WritingCopy(actor, ss);
                return;
            default:
                actor.OutputHandler.Send("That is not a valid argument. Please see WRITING HELP.");
                return;
        }
    }

    private static void WritingCopy(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which writing do you want to copy?");
            return;
        }

        if (!long.TryParse(ss.PopSpeech(), out long value))
        {
            actor.OutputHandler.Send("That is not a valid ID.");
            return;
        }

        IWriting writing = actor.Gameworld.Writings.Get(value);
        if (writing == null)
        {
            actor.OutputHandler.Send("There is no such writing.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What writable do you want to copy that writing onto?");
            return;
        }

        IGameItem target = actor.TargetItem(ss.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anything like that.");
            return;
        }

        IWriteable targetAsWritable = target.GetItemType<IWriteable>();
        if (targetAsWritable == null)
        {
            actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not something that can be written on.");
            return;
        }

        if (!targetAsWritable.CanAddWriting(writing))
        {
            actor.OutputHandler.Send(
                $"That writing won't fit in the current writable section of {target.HowSeen(actor)}.");
            return;
        }

        targetAsWritable.AddWriting(writing);
        actor.OutputHandler.Send($"You add an immutable reference to writing #{writing.Id:N0} to {target.HowSeen(actor)}.");
    }

    private static void WritingShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What is the ID of the writing you want to view?");
            return;
        }

        if (!long.TryParse(ss.PopSpeech(), out long value))
        {
            actor.OutputHandler.Send("That is not a valid ID.");
            return;
        }

        IWriting writing = actor.Gameworld.Writings.Get(value);
        if (writing == null)
        {
            actor.OutputHandler.Send("There is no such writing.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Writing #{writing.Id.ToString("N0", actor)}");
        sb.AppendLine(
            $"Written in the {writing.Language.Name.ColourValue()} language and the {writing.Script.Name.ColourValue()} script.");
        sb.AppendLine(
            $"Written in {writing.Style.DescribeEnum().A_An().Colour(Telnet.Yellow)} style with {(writing.WritingColour?.Name ?? "default").ColourValue()} {writing.ImplementType.Describe(writing.WritingColour).ColourValue()}.");
        if (writing.Author is null)
        {
            var provenance = writing.GetProperty("provenance")?.GetObject as string;
            sb.AppendLine($"Printed source: {provenance.IfNullOrWhiteSpace("unspecified").ColourName()}.");
        }
        else
        {
            sb.AppendLine(
                $"Written by {writing.Author.PersonalName.GetName(NameStyle.FullWithNickname).ColourName()} (ID #{writing.Author.Id.ToString("N0", actor)} - Account {writing.Author.Account.Name.ColourName()}).");
        }
        sb.AppendLine();
        sb.AppendLine(writing.ParseFor(actor));
        actor.OutputHandler.Send(sb.ToString());
    }

    private const string WritingCollectionHelp = @"The #3writingcollection#0 command manages immutable writing collections, which are page-ordered virtual books of writings and drawings.

Create or edit a collection, add writing and drawing records to pages, then apply the collection to a book item. #3wcollection#0 and #3wcoll#0 are aliases.

Most #3set#0, #3add#0, #3remove#0, #3move#0 and #3clear#0 forms operate on the collection you are currently editing. Imports open the text editor and can append to or replace the target collection. Applying a collection preflights page capacity before changing the book.

The syntax is as follows:
	#3writingcollection list [filter]#0 - lists writing collections
	#3writingcollection show [id|name]#0 - shows a collection, or your edited collection
	#3writingcollection new <name>#0 - creates and edits a new collection
	#3writingcollection edit <id|name>#0 - begins editing a collection
	#3writingcollection close#0 - stops editing a collection
	#3writingcollection set name <name>#0 - renames the edited collection
	#3writingcollection set desc <description>#0 - sets the edited collection's description
	#3writingcollection set title <title|clear>#0 - sets or clears the default book title
	#3writingcollection add writing <page> <writing id>#0 - adds a writing record to a page
	#3writingcollection add drawing <page> <drawing id>#0 - adds a drawing record to a page
	#3writingcollection remove <entry#>#0 - removes an entry by table number
	#3writingcollection move <entry#> <page> [order]#0 - moves an entry to a page and optional order
	#3writingcollection clear#0 - removes all entries from the edited collection
	#3writingcollection import markdown [collection] [append|replace]#0 - imports markdown into a collection
	#3writingcollection import json [collection] [append|replace]#0 - imports JSON into a collection
	#3writingcollection apply <collection> <book> [append|page <number>]#0 - applies a collection to a book";

    [PlayerCommand("WritingCollection", "writingcollection", "wcollection", "wcoll")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("writingcollection", WritingCollectionHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void WritingCollectionCommand(ICharacter actor, string command)
    {
        var ss = new StringStack(command.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(WritingCollectionHelp.SubstituteANSIColour());
            return;
        }

        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "list":
                WritingCollectionList(actor, ss);
                return;
            case "show":
            case "view":
                WritingCollectionShow(actor, ss);
                return;
            case "new":
                WritingCollectionNew(actor, ss);
                return;
            case "edit":
                WritingCollectionEdit(actor, ss);
                return;
            case "close":
                actor.RemoveAllEffects<BuilderEditingEffect<IWritingCollection>>();
                actor.OutputHandler.Send("You are no longer editing any writing collection.");
                return;
            case "set":
                WritingCollectionSet(actor, ss);
                return;
            case "add":
                WritingCollectionAdd(actor, ss);
                return;
            case "remove":
            case "delete":
                WritingCollectionRemove(actor, ss);
                return;
            case "move":
                WritingCollectionMove(actor, ss);
                return;
            case "clear":
                WritingCollectionClear(actor);
                return;
            case "import":
            case "upload":
                WritingCollectionImport(actor, ss);
                return;
            case "apply":
                WritingCollectionApply(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(WritingCollectionHelp.SubstituteANSIColour());
                return;
        }
    }

    private static MudSharp.Communication.WritingCollection GetEditingWritingCollection(ICharacter actor)
    {
        return actor.CombinedEffectsOfType<BuilderEditingEffect<IWritingCollection>>()
                    .FirstOrDefault()?.EditingItem as MudSharp.Communication.WritingCollection;
    }

    private static MudSharp.Communication.WritingCollection ResolveWritingCollection(ICharacter actor, string text)
    {
        return actor.Gameworld.WritingCollections.GetByIdOrName(text) as MudSharp.Communication.WritingCollection;
    }

    private static void WritingCollectionList(ICharacter actor, StringStack ss)
    {
        var collections = actor.Gameworld.WritingCollections.AsEnumerable();
        if (!ss.IsFinished)
        {
            var filter = ss.SafeRemainingArgument;
            collections = collections.Where(x => x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) || x.Description.Contains(filter, StringComparison.InvariantCultureIgnoreCase));
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from collection in collections.OrderBy(x => x.Id)
            select new[]
            {
                collection.Id.ToString("N0", actor),
                collection.Name,
                collection.PageCount.ToString("N0", actor),
                collection.Entries.Count().ToString("N0", actor),
                collection.DefaultTitle,
                collection.Description
            },
            new[] { "ID", "Name", "Pages", "Entries", "Title", "Description" },
            actor.LineFormatLength,
            colour: Telnet.Cyan,
            unicodeTable: actor.Account.UseUnicode));
    }

    private static void WritingCollectionShow(ICharacter actor, StringStack ss)
    {
        var collection = ss.IsFinished ? GetEditingWritingCollection(actor) : ResolveWritingCollection(actor, ss.SafeRemainingArgument);
        if (collection is null)
        {
            actor.OutputHandler.Send("Which writing collection do you want to show?");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Writing Collection #{collection.Id.ToString("N0", actor)} - {collection.Name.ColourName()}");
        sb.AppendLine($"Title: {collection.DefaultTitle.IfNullOrWhiteSpace("None").ColourValue()}");
        sb.AppendLine($"Description: {collection.Description.IfNullOrWhiteSpace("None")}");
        sb.AppendLine($"Pages: {collection.PageCount.ToString("N0", actor).ColourValue()}  Entries: {collection.Entries.Count().ToString("N0", actor).ColourValue()}");
        sb.AppendLine();
        if (!collection.Entries.Any())
        {
            sb.AppendLine("This collection does not have any entries.");
            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        sb.AppendLine(StringUtilities.GetTextTable(
            from item in collection.Entries.Select((x, i) => (Entry: x, Index: i + 1))
            select new[]
            {
                item.Index.ToString("N0", actor),
                item.Entry.Page.ToString("N0", actor),
                item.Entry.Order.ToString("N0", actor),
                item.Entry.Readable is IDrawing ? "Drawing" : "Writing",
                item.Entry.Readable.Id.ToString("N0", actor),
                item.Entry.Readable.DocumentLength.ToString("N0", actor),
                item.Entry.Readable.DescribeInLook(actor).RawText()
            },
            new[] { "#", "Page", "Order", "Kind", "ID", "Length", "Description" },
            actor.LineFormatLength,
            colour: Telnet.Cyan,
            unicodeTable: actor.Account.UseUnicode));
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void WritingCollectionNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new writing collection?");
            return;
        }

        var name = ss.SafeRemainingArgument;
        if (actor.Gameworld.WritingCollections.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a writing collection named {name.ColourName()}.");
            return;
        }

        var collection = new MudSharp.Communication.WritingCollection(name, actor.Gameworld);
        actor.Gameworld.Add(collection);
        actor.RemoveAllEffects<BuilderEditingEffect<IWritingCollection>>();
        actor.AddEffect(new BuilderEditingEffect<IWritingCollection>(actor) { EditingItem = collection });
        actor.OutputHandler.Send($"You create writing collection {name.ColourName()} and begin editing it.");
    }

    private static void WritingCollectionEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which writing collection do you want to edit?");
            return;
        }

        var collection = ResolveWritingCollection(actor, ss.SafeRemainingArgument);
        if (collection is null)
        {
            actor.OutputHandler.Send("There is no such writing collection.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IWritingCollection>>();
        actor.AddEffect(new BuilderEditingEffect<IWritingCollection>(actor) { EditingItem = collection });
        actor.OutputHandler.Send($"You are now editing writing collection #{collection.Id.ToString("N0", actor)} ({collection.Name.ColourName()}).");
    }

    private static void WritingCollectionSet(ICharacter actor, StringStack ss)
    {
        var collection = GetEditingWritingCollection(actor);
        if (collection is null)
        {
            actor.OutputHandler.Send("You are not editing any writing collection.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(WritingCollectionHelp.SubstituteANSIColour());
            return;
        }

        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "name":
                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("What new name should this collection have?");
                    return;
                }
                var name = ss.SafeRemainingArgument;
                if (actor.Gameworld.WritingCollections.Any(x => !ReferenceEquals(x, collection) && x.Name.EqualTo(name)))
                {
                    actor.OutputHandler.Send($"There is already a writing collection named {name.ColourName()}.");
                    return;
                }

                collection.Rename(name);
                actor.OutputHandler.Send($"You rename the collection to {collection.Name.ColourName()}.");
                return;
            case "desc":
            case "description":
                collection.SetDescription(ss.SafeRemainingArgument);
                actor.OutputHandler.Send("You update the collection description.");
                return;
            case "title":
                if (ss.IsFinished || ss.PeekSpeech().EqualToAny("clear", "none"))
                {
                    collection.SetDefaultTitle(string.Empty);
                    actor.OutputHandler.Send("This collection no longer has a default book title.");
                    return;
                }
                collection.SetDefaultTitle(ss.SafeRemainingArgument);
                actor.OutputHandler.Send($"This collection's default book title is now \"{collection.DefaultTitle.Colour(Telnet.BoldWhite)}\".");
                return;
            default:
                actor.OutputHandler.Send(WritingCollectionHelp.SubstituteANSIColour());
                return;
        }
    }

    private static void WritingCollectionAdd(ICharacter actor, StringStack ss)
    {
        var collection = GetEditingWritingCollection(actor);
        if (collection is null)
        {
            actor.OutputHandler.Send("You are not editing any writing collection.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add a WRITING or a DRAWING?");
            return;
        }

        var type = ss.PopSpeech();
        if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var page) || page < 1)
        {
            actor.OutputHandler.Send("You must specify a valid page number.");
            return;
        }

        if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var id))
        {
            actor.OutputHandler.Send("You must specify a valid readable ID.");
            return;
        }

        ICanBeRead readable;
        if (type.EqualTo("writing"))
        {
            readable = actor.Gameworld.Writings.Get(id);
        }
        else if (type.EqualTo("drawing"))
        {
            readable = actor.Gameworld.Drawings.Get(id);
        }
        else
        {
            actor.OutputHandler.Send("You can only add WRITING or DRAWING entries.");
            return;
        }

        if (readable is null)
        {
            actor.OutputHandler.Send($"There is no such {type.ToLowerInvariant()}.");
            return;
        }

        var entry = collection.AddEntry(page, readable);
        actor.OutputHandler.Send($"You add {type.ToLowerInvariant()} #{id.ToString("N0", actor)} to page {page.ToString("N0", actor)} at order {entry.Order.ToString("N0", actor)}.");
    }

    private static void WritingCollectionRemove(ICharacter actor, StringStack ss)
    {
        var collection = GetEditingWritingCollection(actor);
        if (collection is null)
        {
            actor.OutputHandler.Send("You are not editing any writing collection.");
            return;
        }

        if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var index))
        {
            actor.OutputHandler.Send("Which entry number do you want to remove?");
            return;
        }

        if (!collection.RemoveEntry(index))
        {
            actor.OutputHandler.Send("There is no such entry.");
            return;
        }

        actor.OutputHandler.Send("You remove that entry from the writing collection.");
    }

    private static void WritingCollectionMove(ICharacter actor, StringStack ss)
    {
        var collection = GetEditingWritingCollection(actor);
        if (collection is null)
        {
            actor.OutputHandler.Send("You are not editing any writing collection.");
            return;
        }

        if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var index))
        {
            actor.OutputHandler.Send("Which entry number do you want to move?");
            return;
        }

        if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var page) || page < 1)
        {
            actor.OutputHandler.Send("Which valid page should that entry move to?");
            return;
        }

        int? order = null;
        if (!ss.IsFinished && int.TryParse(ss.PopSpeech(), out var orderValue) && orderValue > 0)
        {
            order = orderValue;
        }

        if (!collection.MoveEntry(index, page, order))
        {
            actor.OutputHandler.Send("There is no such entry, or the target page was invalid.");
            return;
        }

        actor.OutputHandler.Send($"You move entry #{index.ToString("N0", actor)} to page {page.ToString("N0", actor)}.");
    }

    private static void WritingCollectionClear(ICharacter actor)
    {
        var collection = GetEditingWritingCollection(actor);
        if (collection is null)
        {
            actor.OutputHandler.Send("You are not editing any writing collection.");
            return;
        }

        collection.ClearEntries();
        actor.OutputHandler.Send("You remove all entries from the writing collection.");
    }

    private static void WritingCollectionImport(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to import MARKDOWN or JSON?");
            return;
        }

        var format = ss.PopSpeech();
        if (!format.EqualToAny("markdown", "json"))
        {
            actor.OutputHandler.Send("Import format must be MARKDOWN or JSON.");
            return;
        }

        var collection = GetEditingWritingCollection(actor);
        var replace = false;
        if (!ss.IsFinished && !ss.PeekSpeech().EqualToAny("append", "replace"))
        {
            collection = ResolveWritingCollection(actor, ss.PopSpeech());
        }

        if (collection is null)
        {
            actor.OutputHandler.Send("Which writing collection do you want to import into?");
            return;
        }

        if (!ss.IsFinished)
        {
            var mode = ss.PopSpeech();
            replace = mode.EqualTo("replace");
            if (!replace && !mode.EqualTo("append"))
            {
                actor.OutputHandler.Send("Import mode must be APPEND or REPLACE.");
                return;
            }
        }

        actor.OutputHandler.Send($"Enter the {format.ToUpperInvariant().ColourCommand()} content for writing collection {collection.Name.ColourName()}.");
        actor.EditorMode((text, handler, _) =>
        {
            var result = format.EqualTo("markdown")
                ? MudSharp.Communication.WritingCollectionImport.ImportMarkdown(actor.Gameworld, actor, text)
                : format.EqualTo("json")
                    ? MudSharp.Communication.WritingCollectionImport.ImportJson(actor.Gameworld, actor, text)
                    : new WritingCollectionImportResult(false, "Import format must be MARKDOWN or JSON.", Array.Empty<(int, ICanBeRead)>());
            if (!result.Success)
            {
                handler.Send(result.Error);
                return;
            }

            if (replace)
            {
                collection.ClearEntries();
            }

            foreach (var entry in result.Entries)
            {
                collection.AddEntry(entry.Page, entry.Readable);
            }

            if (!string.IsNullOrWhiteSpace(result.DefaultTitle))
            {
                collection.SetDefaultTitle(result.DefaultTitle);
            }

            if (!string.IsNullOrWhiteSpace(result.Description))
            {
                collection.SetDescription(result.Description);
            }

            handler.Send($"Imported {result.Entries.Count.ToString("N0", actor)} readable entries into writing collection {collection.Name.ColourName()}.");
        }, (handler, _) => handler.Send("You decide not to import anything."), 1.0);
    }

    private static void WritingCollectionApply(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which writing collection do you want to apply?");
            return;
        }

        var collection = ResolveWritingCollection(actor, ss.PopSpeech());
        if (collection is null)
        {
            actor.OutputHandler.Send("There is no such writing collection.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which book do you want to apply it to?");
            return;
        }

        var target = actor.TargetItem(ss.PopSpeech());
        var book = target?.GetItemType<BookGameItemComponent>();
        if (target is null || book is null)
        {
            actor.OutputHandler.Send("You don't see any such book.");
            return;
        }

        var startPage = book.HighestWrittenPage + 1;
        if (!ss.IsFinished)
        {
            var mode = ss.PopSpeech();
            if (mode.EqualTo("page"))
            {
                if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out startPage) || startPage < 1)
                {
                    actor.OutputHandler.Send("You must specify a valid starting page.");
                    return;
                }
            }
            else if (!mode.EqualTo("append"))
            {
                actor.OutputHandler.Send("Use APPEND or PAGE <number> after the book.");
                return;
            }
        }

        if (!TryApplyCollectionToBook(collection, book, startPage, out var error, mutate: false))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        TryApplyCollectionToBook(collection, book, startPage, out _, mutate: true);
        if (string.IsNullOrWhiteSpace(book.Title) && !string.IsNullOrWhiteSpace(collection.DefaultTitle))
        {
            book.Title = collection.DefaultTitle;
            book.Changed = true;
        }
        actor.OutputHandler.Send($"You apply writing collection {collection.Name.ColourName()} to {target.HowSeen(actor)} starting at page {startPage.ToString("N0", actor)}.");
    }

    private static bool TryApplyCollectionToBook(IWritingCollection collection, BookGameItemComponent book, int startPage, out string error, bool mutate)
    {
        error = string.Empty;
        if (!collection.Entries.Any())
        {
            error = "That writing collection does not have any entries.";
            return false;
        }

        var firstPage = collection.Entries.Min(x => x.Page);
        var mapped = collection.Entries
                               .OrderBy(x => x.Page)
                               .ThenBy(x => x.Order)
                               .Select(x => (Page: startPage + x.Page - firstPage, x.Readable))
                               .ToList();
        var pageUsage = mapped.GroupBy(x => x.Page)
                              .ToDictionary(x => x.Key, x => book.DocumentLengthUsedForPage(x.Key));
        foreach (var entry in mapped)
        {
            if (entry.Page < 1)
            {
                error = "The target page must be positive.";
                return false;
            }

            if (!book.CanAddReadable(entry.Readable, entry.Page))
            {
                error = book.WhyCannotAddReadable(entry.Readable, entry.Page);
                return false;
            }

            pageUsage[entry.Page] += entry.Readable.DocumentLength;
            if (pageUsage[entry.Page] > book.MaximumCharacterLengthOfText)
            {
                error = $"The collection would exceed the capacity of page {entry.Page:N0}.";
                return false;
            }
        }

        if (!mutate)
        {
            return true;
        }

        foreach (var entry in mapped)
        {
            book.AddReadable(entry.Readable, entry.Page);
        }

        return true;
    }
    #region Scripted Events
    public const string ScriptedEventHelpText = @"The #3scriptedevent#0 command manages scripted login events for player characters. #3sevent#0 is an alias.

Scripted events interrupt a character's next eligible login with storyteller-authored text and optional questions. Responses are recorded on the event and saved to the character's journal. Create or edit an event, add questions, assign it to a character, and mark it ready.

Templates cannot be marked ready directly. Clone or assign from a template to create character-specific events. #3scriptedevent set autoassign#0 only works on templates with a boolean character filter prog and at least one question, and it asks for confirmation before creating events.
Ready events cannot have their questions edited. Multiple-choice answers can have a boolean character filter prog and a void character on-choice prog.

The syntax is as follows:
	#3scriptedevent list [filters]#0 - lists scripted events
	#3scriptedevent show [id|name]#0 - shows a scripted event, or the one you are editing
	#3scriptedevent view [id|name]#0 - alias for #3show#0
	#3scriptedevent edit <id|name>#0 - begins editing a scripted event
	#3scriptedevent edit#0 - shows the scripted event you are editing
	#3scriptedevent close#0 - stops editing a scripted event
	#3scriptedevent new <name>#0 - creates and edits a new scripted event
	#3scriptedevent clone <id|name>#0 - clones a scripted event and edits the clone
	#3scriptedevent assign <character id>#0 - assigns the event you are editing to a player character
	#3scriptedevent set name <name>#0 - renames the edited event
	#3scriptedevent set earliest <date>#0 - sets the earliest time the event can fire
	#3scriptedevent set character <name|id>#0 - assigns the edited event by character name or ID
	#3scriptedevent set ready#0 - toggles readiness
	#3scriptedevent set template#0 - toggles template status
	#3scriptedevent set filter <prog>#0 - sets the template autoassign filter prog
	#3scriptedevent set autoassign#0 - creates events for all matching PCs from an edited template
	#3scriptedevent set addfree#0 - adds a free-text question through the editor
	#3scriptedevent set addmulti#0 - adds a multiple-choice question through the editor
	#3scriptedevent set remfree <number>#0 - removes a free-text question
	#3scriptedevent set remmulti <number>#0 - removes a multiple-choice question
	#3scriptedevent set text <number>#0 - shows a free-text question
	#3scriptedevent set text <number> question#0 - edits a free-text question
	#3scriptedevent set multi <number>#0 - shows a multiple-choice question
	#3scriptedevent set multi <number> question#0 - edits a multiple-choice question
	#3scriptedevent set multi <number> addanswer#0 - adds a multiple-choice answer
	#3scriptedevent set multi <number> removeanswer <answer>#0 - removes an answer
	#3scriptedevent set multi <number> answer <answer>#0 - shows an answer
	#3scriptedevent set multi <number> answer <answer> before#0 - edits answer text shown before choice
	#3scriptedevent set multi <number> answer <answer> after#0 - edits answer text shown after choice
	#3scriptedevent set multi <number> answer <answer> filter <prog>#0 - edits an answer filter prog
	#3scriptedevent set multi <number> answer <answer> choice <prog>#0 - edits an answer on-choice prog

List Filters:
	#3finished#0, #3!finished#0, #3notfinished#0, #3open#0
	#3ready#0, #3!ready#0, #3notready#0, #3unready#0
	#3template#0, #3!template#0, #3nottemplate#0
	#3assigned#0, #3!assigned#0, #3notassigned#0, #3unassigned#0
	#3+<keyword>#0 - events with a name containing the keyword
	#3-<keyword>#0 - events with a name not containing the keyword
	#3*<id>#0 - events assigned to a character ID
	#3*<name>#0 - events assigned to a character whose name contains the text";

    [PlayerCommand("ScriptedEvent", "scriptedevent", "sevent")]
    [CommandPermission(PermissionLevel.JuniorAdmin)]
    [HelpInfo("ScriptedEvent", ScriptedEventHelpText, AutoHelp.HelpArgOrNoArg)]
    protected static void ScriptedEvent(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "list":
                ScriptedEventList(actor, ss);
                return;
            case "edit":
                ScriptedEventEdit(actor, ss);
                return;
            case "show":
            case "view":
                ScriptedEventShow(actor, ss);
                return;
            case "close":
                ScriptedEventClose(actor, ss);
                return;
            case "new":
                ScriptedEventNew(actor, ss);
                return;
            case "assign":
                ScriptedEventAssign(actor, ss);
                return;
            case "clone":
                ScriptedEventClone(actor, ss);
                return;
            case "set":
                ScriptedEventSet(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(ScriptedEventHelpText.SubstituteANSIColour());
                return;
        }
    }

    private static void ScriptedEventSet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IScriptedEvent> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
        if (editing is null)
        {
            actor.OutputHandler.Send("You are not editing any scripted events.");
            return;
        }

        editing.EditingItem.BuildingCommand(actor, ss);
    }

    private static void ScriptedEventClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which scripted event do you want to clone?");
            return;
        }

        IScriptedEvent item = actor.Gameworld.ScriptedEvents.GetByIdOrName(ss.SafeRemainingArgument);
        if (item is null)
        {
            actor.OutputHandler.Send("There is no such scripted event.");
            return;
        }

        IScriptedEvent clone = item.Clone();
        actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
        actor.AddEffect(new BuilderEditingEffect<IScriptedEvent>(actor) { EditingItem = clone });
        actor.OutputHandler.Send($"You clone scripted event #{item.Id.ToString("N0", actor)} ({item.Name.ColourName()}) to a new item with id #{clone.Id.ToString("N0", actor)}, which you are now editing.");
    }

    private static void ScriptedEventAssign(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IScriptedEvent> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
        if (editing is null)
        {
            actor.OutputHandler.Send("You are not editing any scripted events.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which character do you want to assign that scripted event to?");
            return;
        }

        if (!long.TryParse(ss.SafeRemainingArgument, out long id))
        {
            actor.OutputHandler.Send("That is not a valid character id.");
            return;
        }

        ICharacter character = actor.Gameworld.TryGetCharacter(id, true);
        if (character is null)
        {
            actor.OutputHandler.Send("There is no such character to assign a scripted event to.");
            return;
        }

        if (character is INPC)
        {
            actor.OutputHandler.Send("You cannot assign scripted events to NPCs.");
            return;
        }

        if (character.IsGuest)
        {
            actor.OutputHandler.Send("You cannot assigned scripted events to guests.");
            return;
        }

        IScriptedEvent se = editing.EditingItem.Assign(character);
        if (se != editing.EditingItem)
        {
            actor.OutputHandler.Send($"You assign a new scripted event with id #{se.Id.ToString("N0", actor)} from template #{editing.EditingItem.Id.ToString("N0", actor)} ({editing.EditingItem.Name.ColourValue()}) to {character.PersonalName.GetName(NameStyle.FullName).ColourName()}.");
            return;
        }

        actor.OutputHandler.Send($"You assign scripted event #{editing.EditingItem.Id.ToString("N0", actor)} ({editing.EditingItem.Name.ColourValue()}) to {character.PersonalName.GetName(NameStyle.FullName).ColourName()}.");
    }

    private static void ScriptedEventNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the scripted event?");
            return;
        }

        RPG.ScriptedEvents.ScriptedEvent item = new(ss.SafeRemainingArgument, actor.Gameworld);
        actor.Gameworld.Add(item);
        actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
        actor.AddEffect(new BuilderEditingEffect<IScriptedEvent>(actor) { EditingItem = item });
        actor.OutputHandler.Send($"You are now editing newly created scripted event #{item.Id.ToString("N0", actor)} ({item.Name.ColourName()}).");
    }

    private static void ScriptedEventClose(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IScriptedEvent> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
        if (editing is null)
        {
            actor.OutputHandler.Send("You are not editing any scripted events.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
        actor.OutputHandler.Send($"You are no longer editing any scripted events.");
    }

    private static void ScriptedEventShow(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IScriptedEvent> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
        if (ss.IsFinished)
        {
            if (editing is not null)
            {
                actor.OutputHandler.Send(editing.EditingItem.Show(actor));
                return;
            }

            actor.OutputHandler.Send("Which scripted event would you like to view?");
            return;
        }

        IScriptedEvent item = actor.Gameworld.ScriptedEvents.GetByIdOrName(ss.SafeRemainingArgument);
        if (item is null)
        {
            actor.OutputHandler.Send("There is no such scripted event.");
            return;
        }

        actor.OutputHandler.Send(item.Show(actor));
    }

    private static void ScriptedEventEdit(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IScriptedEvent> editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
        if (ss.IsFinished)
        {
            if (editing is not null)
            {
                actor.OutputHandler.Send(editing.EditingItem.Show(actor));
                return;
            }

            actor.OutputHandler.Send("Which scripted event would you like to edit?");
            return;
        }

        IScriptedEvent item = actor.Gameworld.ScriptedEvents.GetByIdOrName(ss.SafeRemainingArgument);
        if (item is null)
        {
            actor.OutputHandler.Send("There is no such scripted event.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
        actor.AddEffect(new BuilderEditingEffect<IScriptedEvent>(actor) { EditingItem = item });
        actor.OutputHandler.Send($"You are now editing scripted event #{item.Id.ToString("N0", actor)} ({item.Name.ColourName()}).");
    }

    private static void ScriptedEventList(ICharacter actor, StringStack ss)
    {
        IEnumerable<IScriptedEvent> list = actor.Gameworld.ScriptedEvents.AsEnumerable();
        while (!ss.IsFinished)
        {
            switch (ss.PopForSwitch())
            {
                case "finished":
                    list = list.Where(x => x.IsFinished);
                    continue;
                case "!finished":
                case "notfinished":
                case "open":
                    list = list.Where(x => !x.IsFinished);
                    continue;
                case "ready":
                    list = list.Where(x => x.IsReady);
                    continue;
                case "!ready":
                case "notready":
                case "unready":
                    list = list.Where(x => !x.IsReady);
                    continue;
                case "template":
                    list = list.Where(x => x.IsTemplate);
                    continue;
                case "!template":
                case "nottemplate":
                    list = list.Where(x => !x.IsTemplate);
                    continue;
                case "assigned":
                    list = list.Where(x => x.Character is not null);
                    continue;
                case "!assigned":
                case "notassigned":
                case "unassigned":
                    list = list.Where(x => x.Character is null);
                    continue;
            }

            string substrText = ss.Last[1..];

            if (ss.Last.StartsWith("+") && ss.Last.Length > 1)
            {
                list = list.Where(x => x.Name.Contains(substrText, StringComparison.InvariantCultureIgnoreCase));
                continue;
            }

            if (ss.Last.StartsWith("-") && ss.Last.Length > 1)
            {
                list = list.Where(x => !x.Name.Contains(substrText, StringComparison.InvariantCultureIgnoreCase));
                continue;
            }

            if (ss.Last.StartsWith("*") && ss.Last.Length > 1)
            {
                if (long.TryParse(substrText, out long id))
                {
                    list = list.Where(x => x.Character?.Id == id);
                    continue;
                }

                list = list.Where(x => x.Character is not null && x.Character.PersonalName.GetName(NameStyle.FullWithNickname).Contains(substrText, StringComparison.InvariantCultureIgnoreCase));
                continue;
            }

            actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid filter.");
            return;
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from item in list
            select new List<string>
            {
                item.Id.ToString("N0", actor),
                item.Name,
                item.IsTemplate.ToColouredString(),
                item.IsReady.ToColouredString(),
                item.IsFinished.ToColouredString(),
                item.Character?.PersonalName.GetName(NameStyle.FullWithNickname) ?? ""
            },
        new List<string>
        {
            "Id",
            "Name",
            "Template",
            "Ready",
            "Finished",
            "Character"
        },
        actor,
        Telnet.Green
        ));
    }
    #endregion
}
