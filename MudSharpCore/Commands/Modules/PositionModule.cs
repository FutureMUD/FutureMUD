using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MudSharp.Commands.Modules;

internal class PositionModule : Module<ICharacter>
{
    private static readonly Regex _chairAddRegex =
        new(
            @"^([\w]{0,}[a-zA-Z.-]{0,}) ([\w]{0,}[a-zA-Z.-]{0,}) {0,1}(before|behind){0,1} {0,1}(\(.*\)){0,1}$",
            RegexOptions.IgnoreCase);

    private static readonly Regex _chairRemoveRegex =
        new(@"^([\w]{0,}[a-zA-Z.-]{0,}) ([\w]{0,}[a-zA-Z.-]{0,}) {0,1}(\(.*\)){0,1}$");

    private static readonly Regex _positionTableRegex =
        new(
            @"^(sit|rest|lounge|sprawl|lay down|lie down|lay|lie) ([\w]{0,}[a-zA-Z.-]{0,}) {0,}([\w]{0,}[a-zA-Z.-]{0,}) {0,1}(?:\[(.*)\]){0,1} {0,1}(?:\((.*)\)){0,1}$",
            RegexOptions.IgnoreCase);

    private static readonly Regex _positionRegex =
        new(
            @"^(stand|lean|squat|slump|sit|rest|lounge|sprawl|prone|kneel|prostrate) {0,1}(attention|easy){0,1} {0,1}(reset|normal|revert|on|in|inside|by|against|under|underneath|below|beneath|before|behind|around){0,1} {0,1}([\w]{0,}[a-zA-Z.-]{0,}) {0,1}(?:\[(.*)\]){0,1} {0,1}(?:\((.*)\)){0,1}$",
            RegexOptions.IgnoreCase);

    private static readonly Regex _positionItemRegex =
        new(
            @"^(?<command>position|lean|slump|hang) (?<item>[\w]{0,}[a-zA-Z.-]{1,})(?: (?:(?<modifier>reset|normal|revert|by|on|in|inside|under|underneath|below|beneath|before|behind|around|against|from){1}(?: {0,1}(?<target>[\w]{0,}[a-zA-Z.-]{1,})){0,1}|(?<target>[\w]{0,}[a-zA-Z.-]{1,}))){0,1} {0,1}(?:\[(?<omote>.*)\]){0,1} {0,1}(?:\((?<emote>.*)\)){0,1}$",
            RegexOptions.IgnoreCase);

    private PositionModule()
        : base("Position")
    {
        IsNecessary = true;
    }

    public static PositionModule Instance { get; } = new();

    public override int CommandsDisplayOrder => 2;

    private const string ThrustHelp = @"Syntax:
	#3thrust <direction|up|down>#0
	#3thrust stop#0

This command uses a worn zero-gravity propulsion item to push off in the specified direction.";

    [PlayerCommand("Thrust", "thrust")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoCombatCommand]
    [HelpInfo("thrust", ThrustHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Thrust(ICharacter character, string input)
    {
        var ss = new StringStack(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            character.OutputHandler.Send(ThrustHelp.SubstituteANSIColour());
            return;
        }

        var directionText = ss.PopSpeech();
        if (directionText.EqualTo("stop"))
        {
            if (!character.EffectsOfType<ZeroGravityDrift>().Any())
            {
                character.OutputHandler.Send("You are not currently drifting.");
                return;
            }

            character.RemoveAllEffects<ZeroGravityDrift>(fireRemovalAction: true);
            character.OutputHandler.Handle(new EmoteOutput(new Emote("@ arrest|arrests &0's zero-gravity drift.", character, character)));
            return;
        }

        if (!ZeroGravityMovementHelper.IsZeroGravity(character.Location, character.RoomLayer, character))
        {
            character.OutputHandler.Send("You are not in zero gravity.");
            return;
        }

        var propulsor = character.Body.ExternalItems
                                 .Select(x => x.GetItemType<IZeroGravityPropulsion>())
                                 .FirstOrDefault(x => x?.CanPropel(character) == true);
        if (propulsor is null)
        {
            character.OutputHandler.Send("You are not wearing a usable zero-gravity propulsion item.");
            return;
        }

        if (!CardinalDirectionExtensions.CardinalExitStrings.TryGetValue(directionText, out var direction))
        {
            character.OutputHandler.Send("That is not a valid direction.");
            return;
        }

        var exit = character.Location.GetExit(direction, character);
        if (exit is not null)
        {
            var response = ZeroGravityMovementHelper.CanMoveInZeroGravity(character, exit);
            if (!response.Result)
            {
                character.OutputHandler.Send(response.ErrorMessage);
                return;
            }

            if (!propulsor.ConsumePropellant(character))
            {
                character.OutputHandler.Send(propulsor.WhyCannotPropel(character));
                return;
            }

            character.RemoveAllEffects<ZeroGravityDrift>(fireRemovalAction: false);
            character.Move(exit, null, true);
            return;
        }

        if (direction.In(CardinalDirection.Up, CardinalDirection.Down))
        {
            var terrainLayers = character.Location.Terrain(character).TerrainLayers.ToList();
            var possibleLayers = direction == CardinalDirection.Up
                ? terrainLayers.Where(x => x.IsHigherThan(character.RoomLayer)).OrderBy(x => x.LayerHeight()).ToList()
                : terrainLayers.Where(x => x.IsLowerThan(character.RoomLayer)).OrderByDescending(x => x.LayerHeight()).ToList();
            if (!possibleLayers.Any())
            {
                character.OutputHandler.Send("There is nowhere to thrust in that direction.");
                return;
            }

            var targetLayer = possibleLayers.First();
            if (!propulsor.ConsumePropellant(character))
            {
                character.OutputHandler.Send(propulsor.WhyCannotPropel(character));
                return;
            }

            character.RoomLayer = targetLayer;
            ZeroGravityMovementHelper.EnsureFloating(character);
            character.OutputHandler.Handle(new EmoteOutput(new Emote($"@ thrust|thrusts {direction.DescribeBrief()} through zero gravity.", character, character)));
            character.RemoveAllEffects<ZeroGravityDrift>(fireRemovalAction: false);
            character.AddEffect(new ZeroGravityDrift(character, direction), TimeSpan.FromSeconds(5));
            return;
        }

        character.OutputHandler.Send("There is nowhere to thrust in that direction.");
    }

    private const string TetherHelp = @"Syntax:
	#3tether <tether item> to <anchor>#0
	#3untether#0

The tether item must be a zero-gravity tether, and the anchor must be something you can perceive locally.";

    [PlayerCommand("Tether", "tether")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoCombatCommand]
    [HelpInfo("tether", TetherHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Tether(ICharacter character, string input)
    {
        var ss = new StringStack(input.RemoveFirstWord());
        if (ss.IsFinished)
        {
            character.OutputHandler.Send(TetherHelp.SubstituteANSIColour());
            return;
        }

        var tetherItemText = ss.PopSpeech();
        if (!ss.PopForSwitch().EqualTo("to") || ss.IsFinished)
        {
            character.OutputHandler.Send(TetherHelp.SubstituteANSIColour());
            return;
        }

        var tetherItem = character.TargetLocalOrHeldItem(tetherItemText);
        if (tetherItem is null || !tetherItem.IsItemType<IZeroGravityTetherItem>())
        {
            character.OutputHandler.Send("You are not carrying or near any such zero-gravity tether item.");
            return;
        }

        var anchor = character.TargetLocal(ss.SafeRemainingArgument);
        if (anchor is null)
        {
            character.OutputHandler.Send("You don't see anything like that to tether to.");
            return;
        }

        character.RemoveAllEffects<IZeroGravityTetherEffect>(fireRemovalAction: true);
        var tether = tetherItem.GetItemType<IZeroGravityTetherItem>();
        character.AddEffect(new ZeroGravityTether(character, anchor, tether.MaximumRooms, tetherItem));
        character.OutputHandler.Handle(new EmoteOutput(new Emote("@ tether|tethers &0 to $1 with $2.", character, character, anchor, tetherItem)));
    }

    [PlayerCommand("Untether", "untether")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoCombatCommand]
    [HelpInfo("untether", TetherHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Untether(ICharacter character, string input)
    {
        if (!character.EffectsOfType<IZeroGravityTetherEffect>().Any())
        {
            character.OutputHandler.Send("You are not tethered.");
            return;
        }

        character.RemoveAllEffects<IZeroGravityTetherEffect>(fireRemovalAction: true);
        character.OutputHandler.Handle(new EmoteOutput(new Emote("@ untether|untethers &0.", character, character)));
    }

    [PlayerCommand("Pmote", "pmote")]
    [RequiredCharacterState(CharacterState.Awake)]
    [NoCombatCommand]
    [HelpInfo("pmote", @"A #6pmote#0 is a ""Player Emote"", which is an emote that is appended to your long description (what people see when they use the #6look#0 command in a room where you are. PMotes use the same syntax and full grammar markup as other types of emotes and can target other people and things in the room.

Note that PMotes reset if you move, get into combat, or if you change your position in the room (i.e. go from standing to sitting etc.), or if any of the targets of the pmote become invalid (e.g. your pmote targets a person and that person moves away)

The syntax to use this command is as follows:

	#3pmote#0 - shows your current pmote, if any
	#3pmote clear#0 - removes your currently set pmote
	#3pmote <emote text>#0 - sets your pmote

For example, if you set your pmote to #6flipping *pen back and forth between his fingers#0, your long description might appear like this:

#5A tall, dark-haired man#0 is standing here, flipping #2a silver pen#0 back and forth between his fingers.", AutoHelp.HelpArg)]
    protected static void Pmote(ICharacter character, string input)
    {
        string cmd = new StringStack(input.RemoveFirstWord()).PopSpeech().ToLowerInvariant();

        if (cmd.Length == 0)
        {
            if (character.Body.PositionEmote != null)
            {
                character.OutputHandler.Send("Your current pmote is: " +
                                             character.Body.PositionEmote.ParseFor(character).Fullstop());
            }
            else
            {
                character.OutputHandler.Send("You do not currently have a pmote.");
            }

            return;
        }

        if (cmd == "clear" || cmd == "reset")
        {
            if (character.Body.PositionEmote == null)
            {
                character.OutputHandler.Send("You do not have a pmote to clear.");
                return;
            }

            character.Body.SetEmote(null);
            character.OutputHandler.Send("You clear your pmote.");
            return;
        }

        PlayerEmote emote = new(input.RemoveFirstWord(), character,
            permitSpeech: PermitLanguageOptions.LanguageIsError);
        if (!emote.Valid)
        {
            character.OutputHandler.Send(emote.ErrorMessage);
            return;
        }

        character.Body.SetEmote(emote);
        character.OutputHandler.Send("You set your pmote to: " + emote.ParseFor(character).Fullstop());
    }

    [PlayerCommand("Omote", "omote")]
    [NoCombatCommand]
    [RequiredCharacterState(CharacterState.Awake)]
    [HelpInfo("omote", @"An #6omote#0 is an ""Object Emote"", which is an emote that is appended to an item's long description (what people see when they use the #6look#0 command in a room where it is). OMotes use the same syntax and full grammar markup as other types of emotes and can target other people and things in the room.

Note that 

The syntax to use this command is as follows:

	#3omote <item>#0 - views the current omote for an item, if any
	#3omote <item> clear#0 - removes an item's current omote
	#3omote <item> <emote text>#0 - sets an item's omote

For example, if you set the omote of #2a white cotton shirt#0 to #6folded in a neat, crisp square#0, its long description might appear like this:

#2A white cotton shirt#0 is standing here, flipping #2a silver pen#0 back and forth between his fingers.", AutoHelp.HelpArg)]
    protected static void Omote(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        string cmd = ss.PopSpeech();
        if (cmd.Length == 0)
        {
            character.OutputHandler.Send("What do you want to set the omote for?");
            return;
        }

        IGameItem item = character.TargetLocalItem(cmd);
        if (item == null)
        {
            character.OutputHandler.Send("You do not see that item here.");
            return;
        }

        if (ss.IsFinished)
        {
            if (item.PositionEmote != null)
            {
                character.OutputHandler.Send(item.HowSeen(character, true) + " has the following omote: " +
                                             item.PositionEmote.ParseFor(character).Fullstop());
            }
            else
            {
                character.OutputHandler.Send(item.HowSeen(character, true) + " does not currently have an omote.");
            }

            return;
        }

        if (ss.Peek() == "clear" || ss.Peek() == "reset")
        {
            if (item.PositionEmote == null)
            {
                character.OutputHandler.Send(item.HowSeen(character, true) + " does not currently have an omote.");
                return;
            }

            item.SetEmote(null);
            character.OutputHandler.Send("You clear the omote of " + item.HowSeen(character).Fullstop());
            return;
        }

        PlayerEmote emote = new(ss.SafeRemainingArgument, character,
            permitSpeech: PermitLanguageOptions.LanguageIsError);
        if (!emote.Valid)
        {
            character.OutputHandler.Send(emote.ErrorMessage);
            return;
        }

        item.SetEmote(emote);
        character.OutputHandler.Send("You set the omote of " + item.HowSeen(character) + " to: " +
                                     emote.ParseFor(character).Fullstop());
    }

    [PlayerCommand("Tables", "tables")]
    [RequiredCharacterState(CharacterState.Awake)]
    protected static void Tables(ICharacter character, string input)
    {
        StringBuilder sb = new();
        sb.AppendLine("You see the following tables:");
        foreach (ITable table in character.Location.LayerGameItems(character.RoomLayer)
                                       .SelectNotNull(x => x.GetItemType<ITable>()))
        {
            sb.AppendLine(table.Parent.HowSeen(character.Body));
            foreach (ICharacter ch in character.Location.Characters.Where(x => x.PositionTarget == table.Parent))
            {
                sb.AppendLine(
                    $"  {ch.HowSeen(character.Body)} ({ch.PositionState.DefaultDescription()}{(ch.PositionModifier == PositionModifier.None ? "" : " " + ch.PositionModifier)})");
            }

            foreach (IChair chair in table.Chairs)
            {
                sb.AppendLine("  " + chair.Parent.HowSeen(character.Body));
                foreach (ICharacter ch in character.Location.Characters.Where(x => x.PositionTarget == chair.Parent))
                {
                    sb.AppendLine(
                        $"    {ch.HowSeen(character.Body)} ({ch.PositionState.DefaultDescription()}{(ch.PositionModifier == PositionModifier.None ? "" : " " + ch.PositionModifier)})");
                }
            }
        }

        character.OutputHandler.Send(sb.ToString());
    }

    private static void Chair_Add(ICharacter character, StringStack input)
    {
        Match match = _chairAddRegex.Match(input.SafeRemainingArgument);
        if (!match.Success)
        {
            character.OutputHandler.Send("Correct syntax is " +
                                         "chair add <chair> <table> [<position>]".Colour(Telnet.Yellow) + ".");
            return;
        }

        if (match.Groups[1].Value.Length == 0)
        {
            character.OutputHandler.Send("Which chair do you wish to add to a table?");
            return;
        }

        IGameItem chairitem = character.Body.TargetLocalOrHeldItem(match.Groups[1].Value);
        if (chairitem == null)
        {
            character.OutputHandler.Send("You do not see that chair here.");
            return;
        }

        IChair chair = chairitem.GetItemType<IChair>();
        if (chair == null)
        {
            character.Send("{0} is not a chair.", chairitem.HowSeen(character.Body, true));
            return;
        }

        if (match.Groups[2].Value.Length == 0)
        {
            character.OutputHandler.Send("Which table do you wish to add " + chairitem.HowSeen(character.Body) +
                                         " to?");
            return;
        }

        IGameItem tableitem = character.Body.TargetLocalItem(match.Groups[2].Value);
        if (tableitem == null)
        {
            character.OutputHandler.Send("You do not see that table here.");
            return;
        }

        ITable table = tableitem.GetItemType<ITable>();
        if (table == null)
        {
            character.Send("{0} is not a table.", tableitem.HowSeen(character.Body, true));
            return;
        }

        PositionModifier desiredModifier = PositionModifier.None;
        if (match.Groups[3].Success)
        {
            switch (match.Groups[3].Value)
            {
                case "before":
                    desiredModifier = PositionModifier.Before;
                    break;
                case "behind":
                    desiredModifier = PositionModifier.Behind;
                    break;
                default:
                    character.OutputHandler.Send(
                        "That is not a valid position for the chair to be in relation to the table.");
                    return;
            }
        }

        if (!table.CanAddChair(character, chair))
        {
            character.Send(table.WhyCannotAddChair(character, chair));
            return;
        }

        PlayerEmote emote = null;
        if (match.Groups[4].Value.Length > 0)
        {
            emote = new PlayerEmote(new StringStack(match.Groups[4].Value).PopParentheses(), character.Body);
            if (!emote.Valid)
            {
                character.OutputHandler.Send(emote.ErrorMessage);
                return;
            }
        }

        MixedEmoteOutput output = new(new Emote("@ add|adds $0 to $1", character.Body, chairitem, tableitem));
        output.Append(emote);
        if (character.Body.HeldItems.Contains(chairitem))
        {
            character.Body.Take(chairitem);
        }
        else
        {
            character.Body.Location.Extract(chairitem);
        }

        chair.SetTable(table);
        chairitem.SetModifier(desiredModifier);
        chairitem.SetTarget(tableitem);
        character.OutputHandler.Handle(output);
    }

    private static void Chair_Remove(ICharacter character, StringStack input)
    {
        Match match = _chairRemoveRegex.Match(input.SafeRemainingArgument);
        if (!match.Success)
        {
            character.OutputHandler.Send("Correct syntax is " + "chair remove <chair> <table>".Colour(Telnet.Yellow) +
                                         ".");
            return;
        }

        if (match.Groups[1].Value.Length == 0)
        {
            character.OutputHandler.Send("Which chair do you wish to remove from a table?");
            return;
        }

        if (match.Groups[2].Value.Length == 0)
        {
            character.OutputHandler.Send("Which table do you wish to remove a chair from?");
            return;
        }

        IGameItem tableitem = character.Body.TargetLocalItem(match.Groups[2].Value);
        if (tableitem == null)
        {
            character.OutputHandler.Send("You do not see that table here.");
            return;
        }

        ITable table = tableitem.GetItemType<ITable>();
        if (table == null)
        {
            character.Send("{0} is not a table.", tableitem.HowSeen(character, true));
            return;
        }

        IGameItem chairitem = table.Chairs.Select(x => x.Parent)
                             .GetFromItemListByKeyword(match.Groups[1].Value, character);
        if (chairitem == null)
        {
            character.OutputHandler.Send(tableitem.HowSeen(character.Body, true) +
                                         " does not have any such chair to remove.");
            return;
        }

        IChair chair = chairitem.GetItemType<IChair>();
        if (chair == null)
        {
            character.Send("{0} is not a chair.", chairitem.HowSeen(character, true));
            return;
        }

        if (!table.CanRemoveChair(character, chair))
        {
            character.Send(table.WhyCannotRemoveChair(character, chair));
            return;
        }

        PlayerEmote emote = null;
        if (match.Groups[3].Value.Length > 0)
        {
            emote = new PlayerEmote(new StringStack(match.Groups[3].Value).PopParentheses(), character);
            if (!emote.Valid)
            {
                character.OutputHandler.Send(emote.ErrorMessage);
                return;
            }
        }

        MixedEmoteOutput output = new(new Emote("@ remove|removes $0 from $1", character, chairitem, tableitem));
        output.Append(emote);
        chair.SetTable(null);
        chairitem.RoomLayer = character.RoomLayer;
        character.Location.Insert(chairitem, true);
        chairitem.SetModifier(PositionModifier.None);
        chairitem.SetTarget(null);
        character.OutputHandler.Handle(output);
    }

    [PlayerCommand("Chair", "chair")]
    [DelayBlock("general", "You must first stop {0} before you can interact with any chairs.")]
    [NoHideCommand]
    [NoCombatCommand]
    [NoMovementCommand]
    [HelpInfo("Chair",
        @"The chair command is used to add chairs to or remove chairs from tables. Once a chair is added to a table, using the SIT command on the table will instead sit a person at a free chair at that table rather than on the table itself.

The syntax can be either of the following:

#3chair add <chair> <table> [behind|before]#0 - Adds a chair to a table. The behind/before is optional and for flavour only.
#3chair remove <chair> <table>#0 - removes a chair from a table", AutoHelp.HelpArgOrNoArg)]
    protected static void Chair(ICharacter character, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "add":
                Chair_Add(character, ss);
                break;
            case "remove":
                Chair_Remove(character, ss);
                break;
            default:
                character.OutputHandler.Send("That is not a valid option for the Chair command.");
                return;
        }
    }

    private static void Position_Table(ICharacter character, Match match)
    {
        PositionState desiredState = null;
        switch (match.Groups[1].Value.ToLowerInvariant())
        {
            case "sit":
                desiredState = PositionSitting.Instance;
                break;
            case "lay":
            case "lie":
            case "rest":
            case "lie down":
            case "lay down":
                desiredState = PositionLyingDown.Instance;
                break;
            case "lounge":
                desiredState = PositionLounging.Instance;
                break;
            case "sprawl":
                desiredState = PositionSprawled.Instance;
                break;
            default:
                throw new NotSupportedException();
        }

        PlayerEmote emote = null;
        if (match.Groups[5].Value.Length > 0)
        {
            emote = new PlayerEmote(match.Groups[5].Value, character.Body);
            if (!emote.Valid)
            {
                character.OutputHandler.Send(emote.ErrorMessage);
                return;
            }
        }

        PlayerEmote pmote = null;
        if (match.Groups[4].Value.Length > 0)
        {
            pmote = new PlayerEmote(match.Groups[4].Value, character.Body);
            if (!pmote.Valid)
            {
                character.OutputHandler.Send(pmote.ErrorMessage);
                return;
            }
        }

        IGameItem target = character.TargetLocalItem(match.Groups[2].Value);
        if (target == null)
        {
            character.OutputHandler.Send("You do not see that here.");
            return;
        }

        ITable table = target.GetItemType<ITable>();
        if (table == null)
        {
            character.OutputHandler.Send("You may only use this option to sit at tables.");
            return;
        }

        IGameItem chair = null;
        IChair ichair = null;
        if (match.Groups[3].Value == "first" || match.Groups[3].Value == "any")
        {
            ichair = table.Chairs.FirstOrDefault(x => !x.Occupants.Any()) ??
                     table.Chairs.FirstOrDefault(x => x.Occupants.Count() < x.OccupantCapacity);
            if (ichair == null)
            {
                character.OutputHandler.Send("There are no chairs at that table at which you may sit.");
                return;
            }

            chair = ichair.Parent;
        }

        if (chair == null)
        {
            chair = table.Chairs.Select(x => x.Parent).GetFromItemListByKeyword(match.Groups[3].Value, character);
            if (chair == null)
            {
                character.Send("{0} has no such chair.", table.Parent.HowSeen(character.Body, true));
                return;
            }
        }

        ichair = chair.GetItemType<IChair>();
        if (ichair.Occupants.Count() >= ichair.OccupantCapacity)
        {
            character.Send("{0} already has too many occupants.", chair.HowSeen(character.Body, true));
            return;
        }

        character.MovePosition(desiredState, PositionModifier.On, chair, emote, pmote);
    }

    private static void Position_General(ICharacter character, Match match)
    {
        PositionState desiredState = null;
        switch (match.Groups[1].Value.ToLowerInvariant())
        {
            case "stand":
                if (match.Groups[2].Success)
                {
                    switch (match.Groups[2].Value.ToLowerInvariant())
                    {
                        case "easy":
                            desiredState = PositionStandingEasy.Instance;
                            break;
                        case "attention":
                            desiredState = PositionStandingAttention.Instance;
                            break;
                    }
                }
                else
                {
                    desiredState = PositionStanding.Instance;
                }

                break;
            case "squat":
                desiredState = PositionSquatting.Instance;
                break;
            case "sit":
                desiredState = PositionSitting.Instance;
                break;
            case "kneel":
                desiredState = PositionKneeling.Instance;
                break;
            case "prostrate":
                desiredState = PositionProstrate.Instance;
                break;
            case "lounge":
                desiredState = PositionLounging.Instance;
                break;
            case "rest":
                desiredState = PositionLyingDown.Instance;
                break;
            case "prone":
                desiredState = PositionProne.Instance;
                break;
            case "sprawl":
                desiredState = PositionSprawled.Instance;
                break;
            case "lean":
                desiredState = PositionLeaning.Instance;
                break;
            case "slump":
                desiredState = PositionSlumped.Instance;
                break;
            default:
                character.OutputHandler.Send("That is not a valid usage of this command.");
                return;
        }

        PlayerEmote emote = null;
        if (match.Groups[6].Value.Length > 0)
        {
            emote = new PlayerEmote(match.Groups[6].Value, character);
            if (!emote.Valid)
            {
                character.OutputHandler.Send(emote.ErrorMessage);
                return;
            }
        }

        PlayerEmote pmote = null;
        if (match.Groups[5].Value.Length > 0)
        {
            pmote = new PlayerEmote(match.Groups[5].Value, character);
            if (!pmote.Valid)
            {
                character.OutputHandler.Send(pmote.ErrorMessage);
                return;
            }
        }

        PositionModifier desiredModifier = PositionModifier.None;
        if (match.Groups[3].Success)
        {
            switch (match.Groups[3].Value.ToLowerInvariant())
            {
                case "reset":
                case "normal":
                case "revert":
                    if (desiredState.MoveRestrictions != MovementAbility.Free)
                    {
                        character.OutputHandler.Send("You cannot use that option with this command.");
                        return;
                    }

                    if (desiredState != character.PositionState)
                    {
                        character.OutputHandler.Send("You can only use that option with your current position.");
                        return;
                    }

                    character.ResetPositionTarget(emote, pmote);
                    return;
                case "on":
                    desiredModifier = PositionModifier.On;
                    break;
                case "in":
                case "inside":
                    desiredModifier = PositionModifier.In;
                    break;
                case "behind":
                    desiredModifier = PositionModifier.Behind;
                    break;
                case "before":
                    desiredModifier = PositionModifier.Before;
                    break;
                case "around":
                    desiredModifier = PositionModifier.Around;
                    break;
                case "by":
                case "against":
                    desiredModifier = PositionModifier.None;
                    break;
                case "underneath":
                case "below":
                case "under":
                case "beneath":
                    desiredModifier = PositionModifier.Under;
                    break;
            }
        }

        if (match.Groups[4].Value.Length != 0)
        {
            IPerceivable target = character.TargetLocal(match.Groups[4].Value);
            if (target == null)
            {
                character.OutputHandler.Send("You do not see that here.");
                return;
            }

            if (target.IsSelf(character))
            {
                if (desiredState.MoveRestrictions != MovementAbility.Free)
                {
                    character.OutputHandler.Send("You cannot use that option with this command.");
                    return;
                }

                if (desiredState != character.PositionState)
                {
                    character.OutputHandler.Send("You can only use that option with your current position.");
                    return;
                }

                character.ResetPositionTarget(emote, pmote);
                return;
            }

            if (target is IGameItem && (target as IGameItem).IsItemType<ITable>() &&
                (desiredState == PositionSitting.Instance || desiredState == PositionLounging.Instance ||
                 desiredState == PositionLyingDown.Instance || desiredState == PositionSprawled.Instance) && !match.Groups[3].Success)
            {
                Position_Table(character,
                    _positionTableRegex.Match(
                        $"{match.Groups[1].Value} {match.Groups[4].Value} first {(match.Groups[5].Value.Length > 0 ? "[" + match.Groups[5].Value + "]" : "")}{(match.Groups[6].Value.Length > 0 ? "(" + match.Groups[6].Value + ")" : "")}"));
                return;
            }

            if (target is IGameItem && (target as IGameItem).IsItemType<IChair>() &&
                (match.Groups[3].Length == 0 ||
                 match.Groups[3].Value.Equals("on", StringComparison.InvariantCultureIgnoreCase)))
            {
                IChair ichair = (target as IGameItem).GetItemType<IChair>();
                if (ichair.Occupants.Count() >= ichair.OccupantCapacity)
                {
                    character.Send("{0} already has too many occupants.", target.HowSeen(character, true));
                    return;
                }

                character.MovePosition(desiredState, PositionModifier.On, target, emote, pmote);
                return;
            }

            character.MovePosition(desiredState, desiredModifier, target, emote, pmote);
        }
        else
        {
            if (desiredState == character.PositionState && character.PositionTarget != null)
            {
                if (desiredState.MoveRestrictions != MovementAbility.Free)
                {
                    character.OutputHandler.Send("You cannot use that option with this command.");
                    return;
                }

                character.ResetPositionTarget(emote, pmote);
                return;
            }

            // Hack for standing up from chairs
            if (character.PositionTarget is IGameItem targetAsItem && targetAsItem.IsItemType<IChair>() &&
                character.PositionModifier == PositionModifier.On)
            {
                character.MovePosition(desiredState, PositionModifier.None, character.PositionTarget, emote, pmote,
                    true);
                return;
            }

            // Hack for standing on characters
            if (character.PositionTarget is ICharacter targetAsCharacter &&
                character.PositionModifier != PositionModifier.None &&
                desiredState.Upright)
            {
                character.MovePosition(desiredState, PositionModifier.None, targetAsCharacter, emote, pmote, true);
                return;
            }

            character.MovePosition(desiredState, emote, pmote);
        }
    }

    private static void Item_Position(ICharacter actor, string input)
    {
        Match match = _positionItemRegex.Match(input);
        if (!match.Success)
        {
            actor.OutputHandler.Send("That is not a valid usage of the Position command. Correct syntax is " +
                                     "position <item> <modifier> <target>".Colour(Telnet.Yellow) + " or " +
                                     "position <item> reset".Colour(Telnet.Yellow) + ".");
            return;
        }

        var commandText = match.Groups["command"].Value.ToLowerInvariant();
        var itemText = match.Groups["item"].Value;
        var modifierText = match.Groups["modifier"].Value.ToLowerInvariant();
        var targetText = match.Groups["target"].Value;
        var omoteText = match.Groups["omote"].Value;
        var emoteText = match.Groups["emote"].Value;

        PlayerEmote emote = null;
        if (emoteText.Length > 0)
        {
            emote = new PlayerEmote(emoteText, actor.Body);
            if (!emote.Valid)
            {
                actor.OutputHandler.Send(emote.ErrorMessage);
                return;
            }
        }

        IGameItem item = actor.Body.TargetLocalItem(itemText);
        if (item == null)
        {
            actor.OutputHandler.Send("You do not see that here to position.");
            return;
        }

        if (!item.AllowReposition() && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send(item.HowSeen(actor.Body, true) + item.WhyCannotReposition());
            return;
        }

        PlayerEmote imote = null;
        if (omoteText.Length > 0)
        {
            imote = new PlayerEmote(omoteText, actor.Body);
            if (!imote.Valid)
            {
                actor.OutputHandler.Send(imote.ErrorMessage);
                return;
            }
        }

        var desiredState = commandText switch
        {
            "hang" => PositionHanging.Instance,
            "lean" => PositionLeaning.Instance,
            "slump" => PositionSlumped.Instance,
            _ => item.PositionState
        };

        MixedEmoteOutput output;
        if (modifierText == "reset" || modifierText == "normal" || modifierText == "revert")
        {
            if (item.PositionTarget == null && item.PositionModifier == PositionModifier.None &&
                item.PositionState == PositionUndefined.Instance)
            {
                actor.OutputHandler.Send(item.HowSeen(actor.Body, true) + " is already in a normal position.");
                return;
            }

            if (item.PositionTarget == null)
            {
                output =
                    new MixedEmoteOutput(
                        new Emote("@ reset|resets the position of $0", actor.Body, item),
                        flags: OutputFlags.SuppressObscured);
            }
            else
            {
                var text = item.PositionModifier switch
                {
                    PositionModifier.On => " down off of ",
                    PositionModifier.Under => " out from under ",
                    PositionModifier.In => " out of ",
                    PositionModifier.Behind => " out from behind ",
                    PositionModifier.Before => " away from before ",
                    PositionModifier.Around => " away from around ",
                    PositionModifier.None when item.PositionState == PositionHanging.Instance => " down from ",
                    _ => " away from "
                };

                output =
                    new MixedEmoteOutput(
                        new Emote("@ move|moves $0" + text + "$1", actor.Body, item, item.PositionTarget),
                        flags: OutputFlags.SuppressObscured);
            }

            output.Append(emote);
            actor.OutputHandler.Handle(output);
            item.SetPosition(PositionUndefined.Instance, PositionModifier.None, null, imote);
            return;
        }

        if (modifierText.Length == 0 && targetText.Length == 0)
        {
            if (commandText == "position")
            {
                actor.OutputHandler.Send("What do you want to position " + item.HowSeen(actor.Body) + " against?");
                return;
            }

            item.SetPosition(desiredState, PositionModifier.None, null, imote);
            output =
                new MixedEmoteOutput(
                    new Emote($"@ {commandText}|{commandText}s $0", actor.Body, item),
                    flags: OutputFlags.SuppressObscured);
            output.Append(emote);
            actor.OutputHandler.Handle(output);
            return;
        }

        if (targetText.Length == 0)
        {
            actor.OutputHandler.Send("What do you want to position " + item.HowSeen(actor.Body) + " against?");
            return;
        }

        IPerceivable target = actor.Body.TargetLocal(targetText);
        if (target == null)
        {
            actor.OutputHandler.Send("You do not see that here to position " + item.HowSeen(actor.Body) +
                                     " against.");
            return;
        }

        PositionModifier desiredModifier;
        switch (modifierText)
        {
            case "":
                desiredModifier = PositionModifier.None;
                break;
            case "on":
                desiredModifier = PositionModifier.On;
                break;
            case "inside":
            case "in":
                desiredModifier = PositionModifier.In;
                break;
            case "underneath":
            case "below":
            case "beneath":
            case "under":
                desiredModifier = PositionModifier.Under;
                break;
            case "before":
                desiredModifier = PositionModifier.Before;
                break;
            case "behind":
                desiredModifier = PositionModifier.Behind;
                break;
            case "around":
                desiredModifier = PositionModifier.Around;
                break;
            case "by":
            case "against":
            case "from":
            default:
                desiredModifier = PositionModifier.None;
                break;
        }

        if (!target.CanBePositionedAgainst(desiredState, desiredModifier))
        {
            actor.OutputHandler.Send(target.HowSeen(actor.Body, true) + " cannot be positioned against in that way.");
            return;
        }

        item.SetPosition(desiredState, desiredModifier, target, imote);
        var displayModifier = modifierText switch
        {
            "" when commandText == "lean" || commandText == "slump" => "against",
            "" when commandText == "hang" => "from",
            "" => "by",
            "inside" => "in",
            "underneath" or "below" or "beneath" => "under",
            "against" when commandText == "lean" || commandText == "slump" => "against",
            "from" when commandText == "hang" => "from",
            _ => modifierText
        };
        var verb = commandText switch
        {
            "hang" => "hang|hangs",
            "lean" => "lean|leans",
            "slump" => "slump|slumps",
            _ => "position|positions"
        };
        output =
            new MixedEmoteOutput(
                new Emote("@ " + verb + " $0 " + displayModifier + " $1", actor.Body, item,
                    item.PositionTarget), flags: OutputFlags.SuppressObscured);
        output.Append(emote);
        actor.OutputHandler.Handle(output);
    }

    [PlayerCommand("Position", "position", "stand", "sit", "kneel", "prostrate", "lounge", "rest", "sprawl", "prone",
        "lean", "slump", "squat", "hang")]
    [DelayBlock("general", "You must first stop {0} before you can move anywhere.")]
    [DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [HelpInfo("Position", @"This command is used to change your position or the position of an item. It's important to know that there are three components to your position; a target, a modifier, and an emote (also known as pmote for players or omote for items).

#6Player Positioning#0

There are a number of positions available to players, which include:

	#3stand#0, #3stand easy#0, #3stand attention#0, #3sit#0, #3lounge#0, #3rest#0, #3sprawl#0, #3prone#0, #3kneel#0, #3prostrate#0, #3squat#0, #3lean#0 and #3slump#0.



There are also a number of modifiers that you can use, which are as follows:

	#3by#0, #3in#0, #3on#0, #3under#0, #3before#0, #3behind#0, #3around#0 (note - #3by#0 is the default and is implied if not specified)

The syntax pattern for using the various iterations of these positions is exactly the same so for brevity on the #3sit#0 position will be used in the examples below. 
The syntax to use these commands is as follows:

	#3sit#0 - changes your position to sitting (preserving existing target, modifier and pmote)
	#3sit <target>#0 - sits next to a person or item (implied modifier #3by#0)
	#3sit in|on|under|before|behind|around <target>#0 - sits with the target/modifier combination
	#3sit reset#0 - resets your position target / pmote while remaining sitting

All of the above can have an optional pmote set within square brackets #3[]#0, an an optional emote addendum after that with round brackets #3()#0.

Additionally, there is special syntax when sitting at a table or chair (see below).

For example, consider the following examples:

	#3sit chair (throwing herself back with a frustrated sigh)#0

Would echo: #5A short, dark-haired woman#0 sits on #2a wooden chair#0, throwing herself back with a frustrated sigh.
Look Desc: #5A short, dark-haired woman#0 is sitting on #2a wooden chair#0 here.

	#3sit under arch [with his legs crossed and palms upturned, praying]#0

Would echo: #5A tall, dark-haired man#0 sits underneath #2a stone archway#0.
Look Desc: #5A tall, dark-haired man#0 is sitting underneath #2a stone archway#0, with his legs crossed and palms upturned, praying.

	#3sit (unceremoniously) [looking bored]#0

Would echo: #5A bespectacled, dark-skinned lass#0 sits down, unceremoniously.
Look Desc: #5A bespectacled, dark-skinned lass#0 is sitting here, looking bored.

#6Tables and Chairs#0

Items that are codedly set up as tables and chairs have a slightly different interaction with the above syntax for playability purposes. This only applies with the #3sit#0, #3lay#0, #3rest#0 and #3sprawl#0 commands.

Firstly, when you don't specify a modifier with a chair it will automatically assume that you want to sit #3on#0 the chair rather than #3by#0 the chair.

Secondly, when you don't specify a modifier and target a table it will try to find a free chair at that table to put you instead.

If you explicitly want to sit #3by#0 a table or chair, use the #3by#0 modifier.

#6Positioning Other Items#0

You can also use the #3position#0, #3hang#0, #3lean#0, and #3slump#0 versions of this command to change the position of items in the room.

The syntax of this is as follows:

	#3position <item> [<modifier>] <target>#0 - positions an item relative to another target
	#3position <item> reset#0 - resets an item to just generally in the room
	#3hang|lean|slump <item> [<modifier> <target>]#0 - sets an item's visible state, optionally relative to another target

Like with the player version you can have an optional omote set within square brackets #3[]#0, an an optional emote addendum after that with round brackets #3()#0.

For example:

	#3position sack behind counter#0
	#3hang notice on noticeboard [affixed with neat iron nails]#0
	#3slump shirt [in an unceremonious heap] (discarding it without a second thought)#0", AutoHelp.HelpArg)]
    protected static void Position(ICharacter actor, string input)
    {
        var itemMatch = _positionItemRegex.Match(input);
        if (itemMatch.Success)
        {
            var commandText = itemMatch.Groups["command"].Value.ToLowerInvariant();
            if (commandText == "position" || commandText == "hang" ||
                actor.Body.TargetLocalItem(itemMatch.Groups["item"].Value) != null)
            {
                Item_Position(actor, input);
                return;
            }
        }

        Match match = _positionRegex.Match(input);
        if (!match.Success)
        {
            match = _positionTableRegex.Match(input);
            if (!match.Success)
            {
                actor.OutputHandler.Send("That is not a valid use of the " + new StringStack(input).PopSpeech() +
                                         " command.");
                return;
            }

            Position_Table(actor, match);
        }
        else
        {
            Position_General(actor, match);
        }
    }

    [PlayerCommand("Sleep", "sleep")]
    [DelayBlock("general", "You must first stop {0} before you can sleep.")]
    [RequiredCharacterState(CharacterState.Awake)]
    [NoCombatCommand]
    [NoMovementCommand]
    protected static void Sleep(ICharacter actor, string input)
    {
        if (actor.EffectsOfType<IPreventSleepEffect>().FirstOrDefault() is { } sleepBlocker)
        {
            actor.Send(sleepBlocker.SleepPreventionEcho);
            return;
        }

        if (actor.PositionState.CompareTo(actor.Race.MinimumSleepingPosition) == PositionHeightComparison.Higher)
        {
            actor.Send(
                $"You must be at least {actor.Race.MinimumSleepingPosition.DefaultDescription()} before you can go to sleep.");
            return;
        }

        string playerEmote = new StringStack(input.RemoveFirstWord()).PopParentheses();
        PlayerEmote emote = new(playerEmote, actor);
        if (!emote.Valid)
        {
            actor.Send(emote.ErrorMessage);
            return;
        }

        actor.Sleep(emote);
    }

    [PlayerCommand("Wake", "wake")]
    [NoCombatCommand]
    protected static void Wake(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        if (!actor.State.HasFlag(CharacterState.Sleeping))
        {
            actor.Send("You are not presently sleeping.");
            return;
        }

        if (actor.EffectsOfType<INoWakeEffect>().Any() && !actor.IsAdministrator())
        {
            actor.Send(actor.EffectsOfType<INoWakeEffect>().First().WakeUpEcho);
            return;
        }

        if (actor.EffectsOfType<ISleepEffect>().Any() && !actor.IsAdministrator())
        {
            actor.Send("A magical slumber holds you fast.");
            return;
        }

        string playerEmote = new StringStack(input.RemoveFirstWord()).PopParentheses();
        PlayerEmote emote = new(playerEmote, actor);
        if (!emote.Valid)
        {
            actor.Send(emote.ErrorMessage);
            return;
        }

        actor.Awaken(emote);
    }

    [PlayerCommand("Fly", "fly")]
    [DelayBlock("movement", "You cannot move until you stop {0}.")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoMeleeCombatCommand]
    protected static void Fly(ICharacter actor, string input)
    {
        Match positionMatch = _positionRegex.Match(input);
        if (actor.PositionState == PositionFlying.Instance)
        {
            if (positionMatch.Success)
            {
                Position_General(actor, positionMatch);
                return;
            }

            actor.OutputHandler.Send("You are already flying.");
            return;
        }

        string playerEmote = new StringStack(input.RemoveFirstWord()).PopParentheses();
        PlayerEmote emote = new(playerEmote, actor);
        if (!emote.Valid)
        {
            actor.Send(emote.ErrorMessage);
            return;
        }

        (bool truth, string error) = actor.CanFly();
        if (!truth)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        actor.Fly(emote);
    }

    [PlayerCommand("Land", "land")]
    [DelayBlock("movement", "You cannot move until you stop {0}.")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoMeleeCombatCommand]
    protected static void Land(ICharacter actor, string input)
    {
        string playerEmote = new StringStack(input.RemoveFirstWord()).PopParentheses();
        PlayerEmote emote = new(playerEmote, actor);
        if (!emote.Valid)
        {
            actor.Send(emote.ErrorMessage);
            return;
        }

        if (actor.RoomLayer.IsHigherThan(RoomLayer.HighInTrees) && !input.RemoveFirstWord().EqualTo("!"))
        {
            actor.OutputHandler.Send(
                $"You are flying in the air and will fall if you land. If you're sure, type {"land !".ColourCommand()} to land.");
            return;
        }

        actor.Land(emote);
    }

    [PlayerCommand("Dive", "dive", "descend")]
    [DelayBlock("movement", "You cannot move until you stop {0}.")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoMeleeCombatCommand]
    protected static void Dive(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        string playerEmote = ss.PopParentheses();
        PlayerEmote emote = new(playerEmote, actor);
        if (!emote.Valid)
        {
            actor.Send(emote.ErrorMessage);
            return;
        }

        if (actor.PositionState == PositionSwimming.Instance)
        {
            ((ISwim)actor).Dive(emote);
            return;
        }

        if (actor.PositionState == PositionFlying.Instance)
        {
            ((IFly)actor).Dive(emote);
            return;
        }

        actor.OutputHandler.Send("You can only dive when swimming or flying.");
    }

    [PlayerCommand("Ascend", "ascend")]
    [DelayBlock("movement", "You cannot move until you stop {0}.")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoMeleeCombatCommand]
    protected static void Ascend(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        string playerEmote = ss.PopParentheses();
        PlayerEmote emote = new(playerEmote, actor);
        if (!emote.Valid)
        {
            actor.Send(emote.ErrorMessage);
            return;
        }

        if (actor.PositionState == PositionSwimming.Instance)
        {
            ((ISwim)actor).Ascend(emote);
            return;
        }

        if (actor.PositionState == PositionFlying.Instance)
        {
            ((IFly)actor).Ascend(emote);
            return;
        }

        actor.OutputHandler.Send("You can only ascend when swimming or flying.");
    }

    [PlayerCommand("Swim", "swim")]
    [DelayBlock("movement", "You cannot move until you stop {0}.")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoMeleeCombatCommand]
    protected static void Swim(ICharacter actor, string command)
    {
        if (actor.PositionState == PositionSwimming.Instance)
        {
            actor.OutputHandler.Send("You are already swimming.");
            return;
        }

        if (!actor.Location.IsSwimmingLayer(actor.RoomLayer))
        {
            actor.OutputHandler.Send("You are not currently in a body of water, and so there is no need to swim.");
            return;
        }

        actor.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
    }

    [PlayerCommand("Climb", "climb", "cli")]
    [DelayBlock("movement", "You cannot move until you stop {0}.")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoMeleeCombatCommand]
    protected static void Climb(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (actor.PositionState == PositionFlying.Instance)
        {
            actor.OutputHandler.Send(
                "You can't climb while you are flying; either land, or just fly where you want to go.");
            return;
        }

        string modifier = ss.PopSpeech().ToLowerInvariant();
        string playerEmote = ss.PopParentheses();
        PlayerEmote emote = new(playerEmote, actor);
        if (!emote.Valid)
        {
            actor.Send(emote.ErrorMessage);
            return;
        }

        switch (modifier)
        {
            case "up":
                actor.ClimbUp(emote);
                return;
            case "down":
                actor.ClimbDown(emote);
                return;
            case "trees":
            case "roof":
            case "rooftop":
            case "rooftops":
                if (actor.RoomLayer == RoomLayer.GroundLevel)
                {
                    actor.ClimbUp(emote);
                    return;
                }

                goto default;
            default:
                actor.OutputHandler.Send("Do you want to climb up, or down?");
                return;
        }
    }
}
