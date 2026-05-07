using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Commands.Modules;

public class StealthModule : Module<ICharacter>
{
    private StealthModule()
        : base("Stealth")
    {
        IsNecessary = true;
    }

    public static StealthModule Instance { get; } = new();

    [PlayerCommand("Hide", "hide")]
    [DelayBlock("general", "You must first stop {0} before you can hide anywhere.")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoCombatCommand]
    [NoMovementCommand]
    [HelpInfo("hide", @"The #3hide#0 command is used to perform two functions; one is to begin hiding, trying to keep out of sight of everyone. The second is to hide an item.

The syntax is as follows:

	#3hide#0 - begin hiding yourself
	#3hide <item>#0 - hide an item

Note - anyone in the room at the time you hide yourself or an item will be able to see through your stealth as long as you all remain there", AutoHelp.HelpArg)]
    protected static void Hide(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        // Standard Hide
        if (ss.IsFinished)
        {
            if (actor.AffectedBy<IHideEffect>())
            {
                actor.OutputHandler.Send("You are already hidden.");
                return;
            }

            if (!actor.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers))
            {
                actor.OutputHandler.Send(actor.WhyCannotMove());
                return;
            }

            actor.OutputHandler.Handle(new EmoteOutput(
                new Emote("@ begin|begins looking for a hiding spot.", actor), flags: OutputFlags.SuppressObscured));
            actor.AddEffect(new SimpleCharacterAction(actor, character =>
                {
                    actor.OutputHandler.Handle(
                        new EmoteOutput(new Emote("@ settle|settles down into a hiding spot.", actor),
                            flags: OutputFlags.SuppressObscured));
                    CheckOutcome result = actor.Gameworld.GetCheck(CheckType.HideCheck)
                                      .Check(actor, actor.Location.Terrain(actor).HideDifficulty);
                    actor.AddEffect(new HideInvis(actor, result.TargetNumber));
                    foreach (ICharacter witness in actor.Location.Characters.Except(actor))
                    {
                        witness.AddEffect(new SawHider(witness, actor), TimeSpan.FromSeconds(300));
                    }

                    actor.HandleEvent(EventType.CharacterHidden, actor);
                    foreach (IHandleEvents witness in actor.Location.EventHandlers)
                    {
                        witness.HandleEvent(EventType.CharacterHidesWitness, actor, witness);
                    }
                }, "looking for a hiding spot", new[] { "general", "movement" }, "looking for a hiding spot"),
                TimeSpan.FromSeconds(15));
            return;
        }

        // Hide an item
        string targetText = ss.SafeRemainingArgument;
        IGameItem target = actor.TargetLocalOrHeldItem(targetText);
        if (target == null)
        {
            actor.OutputHandler.Send("You do not have or see anything like that to hide.");
            return;
        }

        if (target.AffectedBy<IItemHiddenEffect>())
        {
            actor.Send("{0} is already hidden.\n{1}", target.HowSeen(actor, true),
                "Hint: If you pick up the item, it will clear the hidden flag".Colour(Telnet.Yellow));
            return;
        }

        if (!target.IsItemType<IHoldable>() && !actor.IsAdministrator())
        {
            actor.Send($"You can only hide items that are holdable, and {target.HowSeen(actor)} is not holdable.");
            return;
        }

        if (target.InInventoryOf == actor.Body)
        {
            actor.Body.Take(target);
            target.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(target, true);
        }

        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote("@ begin|begins hiding $0.", actor, target), flags: OutputFlags.SuppressObscured));
        actor.AddEffect(new CharacterActionWithTarget(actor, target,
            character =>
            {
                actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ finish|finishes hiding $0.", actor, target),
                    flags: OutputFlags.SuppressObscured));
                CheckOutcome result = actor.Gameworld.GetCheck(CheckType.HideItemCheck)
                                  .Check(actor, actor.Location.Terrain(actor).HideDifficulty);
                ItemHidden effect = new(target, result.TargetNumber);
                target.AddEffect(effect);
                foreach (ICharacter witness in actor.Location.Characters)
                {
                    witness.AddEffect(new SawHiddenItem(witness, target), TimeSpan.FromSeconds(300));
                    effect.OriginalWitnesses.Add(witness.Id);
                }
            },
            "hiding an item",
            "@ stop|stops hiding $1",
            "@ cannot move because #0 are|is hiding $1",
            new string[] { "general", "movement" },
            "hiding an item"
        ), TimeSpan.FromSeconds(10));
    }

    [PlayerCommand("Unhide", "unhide")]
    [DelayBlock("general", "You must first stop {0} before you can reveal yourself.")]
    [RequiredCharacterState(CharacterState.Able)]
    [HelpInfo("unhide", @"The #3unhide#0 command is used to stop hiding. This command simply toggles your hiding off.

The syntax is as follows:

	#3unhide#0 - reveal yourself to everyone in the room", AutoHelp.HelpArg)]
    protected static void Unhide(ICharacter actor, string command)
    {
        if (!actor.AffectedBy<IHideEffect>())
        {
            actor.OutputHandler.Send("You are not hidden, and so do not need to stop hiding.");
            return;
        }

        StringStack ss = new(command.RemoveFirstWord());
        string emoteText = ss.PopParentheses();
        MixedEmoteOutput output = new(new Emote("@ reveal|reveals %0", actor, actor),
            flags: OutputFlags.SuppressObscured);
        if (!string.IsNullOrWhiteSpace(emoteText))
        {
            PlayerEmote emote = new(emoteText, actor);
            if (!emote.Valid)
            {
                actor.OutputHandler.Send(emote.ErrorMessage);
                return;
            }

            output.Append(emote);
        }

        actor.RemoveAllEffects(x => x.IsEffectType<IHideEffect>());
        actor.OutputHandler.Handle(output);
    }

    [PlayerCommand("Reveal", "reveal")]
    [DelayBlock("general", "You must first stop {0} before you can reveal yourself.")]
    [RequiredCharacterState(CharacterState.Able)]
    [HelpInfo("reveal", @"The #3reveal#0 command is used to reveal yourself to people in your room when you are hiding, without stopping the fact that you are hiding.

People to whom you reveal yourself will be able to see you while you remain hidden and in this location.

The syntax is as follows:

	#3reveal#0 - reveal yourself to everyone in the room
	#3reveal <target>#0 - reveal yourself only to a particular person", AutoHelp.HelpArg)]
    protected static void Reveal(ICharacter actor, string command)
    {
        if (!actor.AffectedBy<IHideEffect>())
        {
            actor.OutputHandler.Send("You are not hidden, and so do not need to reveal yourself.");
            return;
        }

        StringStack ss = new(command.RemoveFirstWord());
        string emoteText = ss.PopParentheses();

        MixedEmoteOutput output;
        Action successAction;

        if (ss.IsFinished)
        {
            successAction = () =>
            {
                foreach (ICharacter person in actor.Location.LayerCharacters(actor.RoomLayer).Except(actor).AsEnumerable())
                {
                    person.AddEffect(new SawHider(person, actor), TimeSpan.FromSeconds(600));
                }
            };
            output = new MixedEmoteOutput(new Emote("@ reveal|reveals %0 to everyone present", actor, actor),
                flags: OutputFlags.SuppressObscured);
        }
        else
        {
            string targetText = ss.PopSpeech();
            ICharacter target = actor.TargetActor(targetText);
            if (target == null)
            {
                actor.OutputHandler.Send("You do not see any such person to whom to reveal yourself.");
                return;
            }

            successAction = () => { target.AddEffect(new SawHider(target, actor), TimeSpan.FromSeconds(600)); };

            output = new MixedEmoteOutput(new Emote("@ reveal|reveals %0 to $1", actor, actor, target),
                flags: OutputFlags.SuppressObscured);

            emoteText = ss.PopParentheses();
        }

        if (!string.IsNullOrWhiteSpace(emoteText))
        {
            PlayerEmote emote = new(emoteText, actor);
            if (!emote.Valid)
            {
                actor.OutputHandler.Send(emote.ErrorMessage);
                return;
            }

            output.Append(emote);
        }

        successAction();
        actor.OutputHandler.Handle(output);
    }

    [PlayerCommand("Sneak", "sneak")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoMovementCommand]
    [HelpInfo("sneak", @"The #3sneak#0 command is used to toggle whether you want to sneak when you move, and try to hide the fact that you have moved to others around you.

If someone notices you sneaking it is obvious that you were trying to sneak, unless you use the subtle sneak option; if they notice you with subtle sneaking, they will just see a regular movement message. However, subtle sneaking is easier to notice on the whole.

The syntax is as follows:

	#3sneak#0 - toggles sneaking on or off
	#3sneak subtle#0 - toggles subtle sneaking on or off", AutoHelp.HelpArg)]
    protected static void Sneak(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        bool subtleSneak = false;
        if (!ss.IsFinished)
        {
            if (ss.PopSpeech().EqualTo("subtle"))
            {
                subtleSneak = true;
            }
            else
            {
                actor.Send("You can either use the syntax SNEAK or SNEAK SUBTLE.");
                return;
            }
        }

        if (actor.AffectedBy<ISneakEffect>())
        {
            if (subtleSneak && !actor.EffectsOfType<ISneakEffect>().First().Subtle)
            {
                actor.RemoveAllEffects(x => x.IsEffectType<ISneakEffect>());
                actor.AddEffect(new SneakSubtle(actor));
                actor.OutputHandler.Send("You will now try to subtly sneak when you move about.");
                return;
            }

            actor.RemoveAllEffects(x => x.IsEffectType<ISneakEffect>());
            actor.OutputHandler.Send("You will no longer try to sneak when you move about.");
            return;
        }

        if (subtleSneak)
        {
            actor.AddEffect(new SneakSubtle(actor));
            actor.OutputHandler.Send("You will now try to subtly sneak when you move about.");
        }
        else
        {
            actor.AddEffect(new Sneak(actor));
            actor.OutputHandler.Send("You will now try to sneak when you move about.");
        }
    }

    [PlayerCommand("Palm", "palm")]
    [DelayBlock("general", "You must first stop {0} before you can palm anything.")]
    [CommandPermission(PermissionLevel.NPC)]
    [RequiredCharacterState(CharacterState.Able)]
    [HelpInfo("palm", @"The #3palm#0 command is used to secretly move items or money without a public echo unless someone notices what you are doing.

The syntax is as follows:

	#3palm [<quantity>] <item>#0 - secretly get an item from the room
	#3palm [exactly] <currency>#0 - secretly get money from the room
	#3palm [<quantity>] <item> from <container>#0 - secretly get an item from a container
	#3palm [exactly] <currency> from <container>#0 - secretly get money from a container
	#3palm [<quantity>] <item> from <person> <container>#0 - secretly get an item from someone's container
	#3palm [exactly] <currency> from <person> <container>#0 - secretly get money from someone's container
	#3palm [<quantity>] <item> into <container>#0 - secretly put an item into a container
	#3palm [exactly] <currency> into <container>#0 - secretly put money into a container
	#3palm [<quantity>] <item> into <person> <container>#0 - secretly put an item into someone's container
	#3palm [exactly] <currency> into <person> <container>#0 - secretly put money into someone's container", AutoHelp.HelpArg)]
    protected static void Palm(ICharacter actor, string command)
    {
        if (actor.Combat != null)
        {
            actor.Send("You are too busy fighting to worry about that!");
            return;
        }

        var ss = new StringStack(command.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What do you want to palm?\nThe syntax is {"palm [<quantity>] <item>".ColourCommand()}, {"palm [<quantity>] <item> from <container>".ColourCommand()} or {"palm [<quantity>] <item> into [<person>] <container>".ColourCommand()}.");
            return;
        }

        var text = ss.SafeRemainingArgument;
        if (TrySplitOnKeyword(text, "into", out var targetText, out var destinationText))
        {
            PalmInto(actor, targetText, destinationText);
            return;
        }

        if (TrySplitOnKeyword(text, "from", out targetText, out var sourceText))
        {
            PalmFrom(actor, targetText, sourceText);
            return;
        }

        PalmFromRoom(actor, text);
    }

    [PlayerCommand("Steal", "steal")]
    [DelayBlock("general", "You must first stop {0} before you can steal anything.")]
    [CommandPermission(PermissionLevel.NPC)]
    [RequiredCharacterState(CharacterState.Able)]
    [HelpInfo("steal", @"The #3steal#0 command is used to secretly steal from another character's open worn or held containers, or cut a belted item free from one of their belts. Belt-cutting requires a held or wielded item tagged by the #6CutPurseToolTagName#0 static configuration.

The syntax is as follows:

	#3steal <person>#0 - steal a random item from an open container or belt
	#3steal <person> <container|belt>#0 - steal a random item from a specific open container or belt
	#3steal <person> from <container|belt>#0 - steal a random item from a specific open container or belt
	#3steal <person> [<quantity>] <item>#0 - steal an item from any open container
	#3steal <person> [<quantity>] <item> from <container>#0 - steal an item from a specific open container
	#3steal <person> [exactly] <currency>#0 - steal money from any open container
	#3steal <person> [exactly] <currency> from <container>#0 - steal money from a specific open container
	#3steal <person> <belted item> from <belt>#0 - cut a belted item free", AutoHelp.HelpArg)]
    protected static void Steal(ICharacter actor, string command)
    {
        if (actor.Combat != null)
        {
            actor.Send("You are too busy fighting to worry about that!");
            return;
        }

        var ss = new StringStack(command.RemoveFirstWord());
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Who do you want to steal from?\nThe syntax is {"steal <person> [[<quantity>] <item>|[exactly] <currency>|<container>]".ColourCommand()}.");
            return;
        }

        var target = actor.TargetActor(ss.PopSpeech(), PerceiveIgnoreFlags.IgnoreSelf);
        if (target is null)
        {
            actor.OutputHandler.Send("You don't see anyone like that to steal from.");
            return;
        }

        if (target == actor)
        {
            actor.OutputHandler.Send("You cannot steal from yourself.");
            return;
        }

        if (!actor.ColocatedWith(target))
        {
            actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not close enough for you to steal from.");
            return;
        }

        var remaining = ss.SafeRemainingArgument;
        if (string.IsNullOrWhiteSpace(remaining))
        {
            StealRandom(actor, target);
            return;
        }

        if (TrySplitOnKeyword(remaining, "from", out var itemText, out var sourceText))
        {
            if (string.IsNullOrWhiteSpace(sourceText))
            {
                actor.OutputHandler.Send("Which container or belt do you want to steal from?");
                return;
            }

            StealFromSource(actor, target, itemText, sourceText);
            return;
        }

        if (TryResolveStealSource(actor, target, remaining, out var source))
        {
            StealFromSource(actor, target, string.Empty, remaining);
            return;
        }

        StealSpecificFromAnyContainer(actor, target, remaining);
    }

    private enum StealthTransferKind
    {
        Item,
        Currency
    }

    private enum StealSourceKind
    {
        Container,
        Belt
    }

    private sealed class StealthTransferSpec
    {
        public StealthTransferKind Kind { get; set; }
        public IGameItem Item { get; set; }
        public int Quantity { get; set; }
        public ICurrency Currency { get; set; }
        public decimal Amount { get; set; }
        public bool Exact { get; set; }
        public string OriginalText { get; set; }

        public string Describe(ICharacter actor)
        {
            return Kind == StealthTransferKind.Currency
                ? Currency.Describe(Amount, CurrencyDescriptionPatternType.ShortDecimal)
                : Quantity > 0
                    ? $"{Quantity.ToString("N0", actor)} of {Item.HowSeen(actor)}"
                    : Item.HowSeen(actor);
        }
    }

    private sealed class StealthDetectionResult
    {
        public CheckOutcome ActorOutcome { get; set; } = CheckOutcome.NotTested(CheckType.None);
        public List<ICharacter> Noticers { get; } = new();
        public bool VictimStopped { get; set; }
        public bool ActorSucceeded => ActorOutcome.IsPass();
    }

    private sealed class StealSource
    {
        public StealSourceKind Kind { get; set; }
        public IGameItem Item { get; set; }
    }

    private sealed class StealCandidate
    {
        public StealSourceKind Kind { get; set; }
        public IGameItem Source { get; set; }
        public IGameItem Item { get; set; }
    }

    private static bool TrySplitOnKeyword(string text, string keyword, out string before, out string after)
    {
        text = text.Trim();
        var start = $"{keyword} ";
        if (text.StartsWith(start, StringComparison.InvariantCultureIgnoreCase))
        {
            before = string.Empty;
            after = text[start.Length..].Trim();
            return true;
        }

        var marker = $" {keyword} ";
        var index = text.IndexOf(marker, StringComparison.InvariantCultureIgnoreCase);
        if (index < 0)
        {
            before = text;
            after = string.Empty;
            return false;
        }

        before = text[..index].Trim();
        after = text[(index + marker.Length)..].Trim();
        return true;
    }

    private static bool TryParseCurrencyAmount(ICharacter actor, string text, out decimal amount, out bool exact)
    {
        amount = 0.0M;
        exact = false;
        text = text.Trim();
        if (text.StartsWith("exactly ", StringComparison.InvariantCultureIgnoreCase))
        {
            exact = true;
            text = text["exactly ".Length..].Trim();
        }

        text = text.Strip(x => x == '"');
        if (string.IsNullOrWhiteSpace(text) || actor.Currency is null)
        {
            return false;
        }

        amount = actor.Currency.GetBaseCurrency(text, out var success);
        return success && amount > 0.0M;
    }

    private static bool TryParseTransferSpec(ICharacter actor, string text, IEnumerable<IGameItem> items,
        string locationDescription, out StealthTransferSpec spec, out string error)
    {
        spec = null;
        error = string.Empty;
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            error = "You must specify what you want to move.";
            return false;
        }

        if (!text.StartsWith("exactly ", StringComparison.InvariantCultureIgnoreCase))
        {
            var quantity = 0;
            var itemText = text;
            var ss = new StringStack(text);
            var first = ss.PopSpeech();
            if (int.TryParse(first, out var parsedQuantity))
            {
                if (parsedQuantity <= 0)
                {
                    error = "You must specify a positive quantity.";
                    return false;
                }

                if (ss.IsFinished)
                {
                    error = "You must specify an item after the quantity.";
                    return false;
                }

                quantity = parsedQuantity;
                itemText = ss.SafeRemainingArgument;
            }

            var item = items
                       .Where(x => actor.CanSee(x))
                       .GetFromItemListByKeyword(itemText, actor);
            if (item is not null)
            {
                spec = new StealthTransferSpec
                {
                    Kind = StealthTransferKind.Item,
                    Item = item,
                    Quantity = quantity,
                    OriginalText = text
                };
                return true;
            }
        }

        if (TryParseCurrencyAmount(actor, text, out var amount, out var exact))
        {
            spec = new StealthTransferSpec
            {
                Kind = StealthTransferKind.Currency,
                Currency = actor.Currency,
                Amount = amount,
                Exact = exact,
                OriginalText = text
            };
            return true;
        }

        error =
            $"You don't see anything like {text.ColourCommand()} {locationDescription}, and that is not a valid amount of money.";
        return false;
    }

    private static bool TryResolvePalmDestination(ICharacter actor, string text, out IGameItem containerItem,
        out ICharacter containerOwner, out string error)
    {
        containerItem = null;
        containerOwner = null;
        error = string.Empty;
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            error = "Which container do you want to palm it into?";
            return false;
        }

        var ss = new StringStack(text);
        var first = ss.PopSpeech();
        if (!ss.IsFinished)
        {
            var possibleOwner = actor.TargetActor(first);
            if (possibleOwner is not null)
            {
                if (!actor.ColocatedWith(possibleOwner))
                {
                    error = $"{possibleOwner.HowSeen(actor, true)} is not close enough for you to palm anything into their belongings.";
                    return false;
                }

                containerOwner = possibleOwner;
                containerItem = possibleOwner.Body.ExternalItemsForOtherActors
                                             .Where(x => actor.CanSee(x))
                                             .GetFromItemListByKeyword(ss.SafeRemainingArgument, actor);
                if (containerItem is null)
                {
                    error = $"{possibleOwner.HowSeen(actor, true)} does not seem to have any container like that.";
                    return false;
                }

                if (!containerItem.IsItemType<IContainer>())
                {
                    error = $"{containerItem.HowSeen(actor, true)} is not a container.";
                    return false;
                }

                return true;
            }
        }

        containerItem = actor.TargetItem(text);
        if (containerItem is null)
        {
            error = "You don't see any such container.";
            return false;
        }

        if (!containerItem.IsItemType<IContainer>())
        {
            error = $"{containerItem.HowSeen(actor, true)} is not a container.";
            return false;
        }

        containerOwner = containerItem.InInventoryOf?.Actor;
        return true;
    }

    private static bool TryResolvePalmSource(ICharacter actor, string text, out IGameItem containerItem,
        out ICharacter containerOwner, out string error)
    {
        containerItem = null;
        containerOwner = null;
        error = string.Empty;
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            error = "Which container do you want to palm something from?";
            return false;
        }

        var ss = new StringStack(text);
        var first = ss.PopSpeech();
        if (!ss.IsFinished)
        {
            var possibleOwner = actor.TargetActor(first);
            if (possibleOwner is not null)
            {
                if (!actor.ColocatedWith(possibleOwner))
                {
                    error = $"{possibleOwner.HowSeen(actor, true)} is not close enough for you to palm anything from their belongings.";
                    return false;
                }

                containerOwner = possibleOwner;
                containerItem = possibleOwner.Body.ExternalItemsForOtherActors
                                             .Where(x => actor.CanSee(x))
                                             .GetFromItemListByKeyword(ss.SafeRemainingArgument, actor);
                if (containerItem is null)
                {
                    error = $"{possibleOwner.HowSeen(actor, true)} does not seem to have any container like that.";
                    return false;
                }

                if (!containerItem.IsItemType<IContainer>())
                {
                    error = $"{containerItem.HowSeen(actor, true)} is not a container.";
                    return false;
                }

                return true;
            }
        }

        containerItem = actor.TargetItem(text);
        if (containerItem is null)
        {
            error = "You don't see any such container.";
            return false;
        }

        if (!containerItem.IsItemType<IContainer>())
        {
            error = $"{containerItem.HowSeen(actor, true)} is not a container.";
            return false;
        }

        containerOwner = containerItem.InInventoryOf?.Actor;
        return true;
    }

    private static void PalmFromRoom(ICharacter actor, string targetText)
    {
        var items = actor.Location.LayerGameItems(actor.RoomLayer);
        if (!TryParseTransferSpec(actor, targetText, items, "here", out var spec, out var error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        IGameItem previewItem;
        if (spec.Kind == StealthTransferKind.Item)
        {
            previewItem = spec.Item;
            if (!actor.Body.CanGet(spec.Item, spec.Quantity))
            {
                actor.OutputHandler.Send(actor.Body.WhyCannotGet(spec.Item, spec.Quantity));
                return;
            }
        }
        else
        {
            previewItem = null;
            if (!actor.Body.CanGet(spec.Currency, spec.Amount, spec.Exact))
            {
                actor.OutputHandler.Send(actor.Body.WhyCannotGet(spec.Currency, spec.Amount, spec.Exact));
                return;
            }
        }

        if (!TryResolveStealth(actor, null, previewItem, CheckType.PalmCheck, null,
                $"palm {spec.Describe(actor)}", out var detection))
        {
            return;
        }

        var moved = spec.Kind == StealthTransferKind.Item
            ? actor.Body.Get(spec.Item, spec.Quantity, null, true, ItemCanGetIgnore.None,
                detection.Noticers.Cast<IHandleEvents>())
            : actor.Body.Get(spec.Currency, spec.Amount, spec.Exact, null, true,
                detection.Noticers.Cast<IHandleEvents>());

        if (moved is null)
        {
            return;
        }

        actor.OutputHandler.Send(
            $"You palm {moved.HowSeen(actor)} without drawing attention.{NoticedSuffix(detection)}");
        NotifyNoticers(actor, detection, witness =>
            $"You notice {actor.HowSeen(witness, true)} palm {moved.HowSeen(witness)}.");
    }

    private static void PalmFrom(ICharacter actor, string targetText, string sourceText)
    {
        if (!TryResolvePalmSource(actor, sourceText, out var sourceItem, out var owner, out var error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        var container = sourceItem.GetItemType<IContainer>();
        if (!TryParseTransferSpec(actor, targetText, container.Contents, $"in {sourceItem.HowSeen(actor)}",
                out var spec, out error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        var crime = owner is not null && owner != actor ? CrimeTypes.Theft : (CrimeTypes?)null;
        var previewItem = spec.Kind == StealthTransferKind.Item ? spec.Item : sourceItem;
        if (WouldStopLawfulAction(actor, crime, owner, previewItem))
        {
            return;
        }

        if (spec.Kind == StealthTransferKind.Item)
        {
            if (!actor.Body.CanGet(spec.Item, sourceItem, spec.Quantity))
            {
                actor.OutputHandler.Send(actor.Body.WhyCannotGet(spec.Item, sourceItem, spec.Quantity));
                return;
            }
        }
        else if (!actor.Body.CanGet(spec.Currency, sourceItem, spec.Amount, spec.Exact))
        {
            actor.OutputHandler.Send(actor.Body.WhyCannotGet(spec.Currency, sourceItem, spec.Amount, spec.Exact));
            return;
        }

        if (!TryResolveStealth(actor, owner == actor ? null : owner, previewItem, CheckType.PalmCheck, crime,
                $"palm {spec.Describe(actor)} from {sourceItem.HowSeen(actor)}", out var detection))
        {
            return;
        }

        var moved = spec.Kind == StealthTransferKind.Item
            ? actor.Body.Get(spec.Item, sourceItem, spec.Quantity, null, true, ItemCanGetIgnore.None,
                detection.Noticers.Cast<IHandleEvents>())
            : actor.Body.Get(spec.Currency, sourceItem, spec.Amount, spec.Exact, null, true,
                detection.Noticers.Cast<IHandleEvents>());

        if (moved is null)
        {
            return;
        }

        actor.OutputHandler.Send(
            $"You palm {moved.HowSeen(actor)} from {sourceItem.HowSeen(actor)} without drawing attention.{NoticedSuffix(detection)}");
        NotifyNoticers(actor, detection, witness =>
            $"You notice {actor.HowSeen(witness, true)} palm {moved.HowSeen(witness)} from {sourceItem.HowSeen(witness)}.");
        RecordStealthCrime(actor, crime, owner, moved, detection);
    }

    private static void PalmInto(ICharacter actor, string targetText, string destinationText)
    {
        if (!TryResolvePalmDestination(actor, destinationText, out var containerItem, out var containerOwner,
                out var error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        if (!TryParseTransferSpec(actor, targetText, actor.Body.ItemsInHands,
                "in your hands", out var spec, out error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        var victim = containerOwner == actor ? null : containerOwner;
        var check = victim is null ? CheckType.PalmCheck : CheckType.StealCheck;
        var crime = victim is null ? (CrimeTypes?)null : CrimeTypes.UnauthorisedDealing;
        var previewItem = spec.Kind == StealthTransferKind.Item ? spec.Item : containerItem;
        if (WouldStopLawfulAction(actor, crime, victim, previewItem))
        {
            return;
        }

        if (spec.Kind == StealthTransferKind.Item)
        {
            if (!actor.Body.CanPut(spec.Item, containerItem, containerOwner, spec.Quantity, true))
            {
                actor.OutputHandler.Send(actor.Body.WhyCannotPut(spec.Item, containerItem, containerOwner,
                    spec.Quantity, true));
                return;
            }
        }
        else if (!actor.Body.CanPut(spec.Currency, containerItem, containerOwner, spec.Amount, spec.Exact))
        {
            actor.OutputHandler.Send(actor.Body.WhyCannotPut(spec.Currency, containerItem, containerOwner,
                spec.Amount, spec.Exact));
            return;
        }

        if (!TryResolveStealth(actor, victim, previewItem, check, crime,
                $"palm {spec.Describe(actor)} into {containerItem.HowSeen(actor)}", out var detection))
        {
            return;
        }

        var moved = spec.Kind == StealthTransferKind.Item
            ? actor.Body.Put(spec.Item, containerItem, containerOwner, spec.Quantity, null, true, true,
                detection.Noticers.Cast<IHandleEvents>())
            : actor.Body.Put(spec.Currency, containerItem, containerOwner, spec.Amount, spec.Exact, null, true,
                detection.Noticers.Cast<IHandleEvents>());

        if (moved is null)
        {
            return;
        }

        actor.OutputHandler.Send(
            $"You palm {moved.HowSeen(actor)} into {containerItem.HowSeen(actor)} without drawing attention.{NoticedSuffix(detection)}");
        NotifyNoticers(actor, detection, witness =>
            $"You notice {actor.HowSeen(witness, true)} palm {moved.HowSeen(witness)} into {containerItem.HowSeen(witness)}.");
        RecordStealthCrime(actor, crime, victim, moved, detection);
    }

    private static void StealRandom(ICharacter actor, ICharacter target)
    {
        var candidates = GetStealCandidates(actor, target, true).ToList();
        if (!candidates.Any())
        {
            actor.OutputHandler.Send(
                $"{target.HowSeen(actor, true)} does not have anything visible and accessible in an open container or on a belt for you to steal.");
            return;
        }

        ExecuteStealCandidate(actor, target, candidates.GetRandomElement());
    }

    private static void StealFromSource(ICharacter actor, ICharacter target, string itemText, string sourceText)
    {
        if (!TryResolveStealSource(actor, target, sourceText, out var source))
        {
            actor.OutputHandler.Send(
                $"{target.HowSeen(actor, true)} does not have any open container or belt like {sourceText.ColourCommand()}.");
            return;
        }

        if (string.IsNullOrWhiteSpace(itemText))
        {
            var candidates = GetStealCandidates(actor, target)
                             .Where(x => x.Source == source.Item && x.Kind == source.Kind)
                             .ToList();
            if (!candidates.Any())
            {
                actor.OutputHandler.Send(
                    $"{source.Item.HowSeen(actor, true)} does not have anything visible and accessible for you to steal.");
                return;
            }

            ExecuteStealCandidate(actor, target, candidates.GetRandomElement());
            return;
        }

        if (source.Kind == StealSourceKind.Belt)
        {
            var item = source.Item.GetItemType<IBelt>().ConnectedItems
                             .Select(x => x.Parent)
                             .Where(x => actor.CanSee(x))
                             .GetFromItemListByKeyword(itemText, actor);
            if (item is null)
            {
                actor.OutputHandler.Send($"{source.Item.HowSeen(actor, true)} does not have anything like that attached to it.");
                return;
            }

            StealBeltedItem(actor, target, source.Item, item);
            return;
        }

        StealSpecificFromContainer(actor, target, itemText, source.Item);
    }

    private static void StealSpecificFromAnyContainer(ICharacter actor, ICharacter target, string itemText)
    {
        var containers = GetOpenStealableContainers(actor, target).ToList();
        if (!containers.Any())
        {
            actor.OutputHandler.Send($"{target.HowSeen(actor, true)} does not have any visible open containers you can steal from.");
            return;
        }

        if (TryParseCurrencyAmount(actor, itemText, out var amount, out var exact))
        {
            var container = containers.FirstOrDefault(x => actor.Body.CanGet(actor.Currency, x, amount, exact));
            if (container is null)
            {
                actor.OutputHandler.Send(
                    $"{target.HowSeen(actor, true)} does not have that much accessible money in any visible open container.");
                return;
            }

            StealCurrencyFromContainer(actor, target, actor.Currency, amount, exact, container);
            return;
        }

        var allItems = containers.SelectMany(x => x.GetItemType<IContainer>().Contents).ToList();
        if (!TryParseTransferSpec(actor, itemText, allItems, $"in {target.HowSeen(actor)}'s open containers",
                out var spec, out var error) || spec.Kind != StealthTransferKind.Item)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        var source = containers.FirstOrDefault(x => x.GetItemType<IContainer>().Contents.Contains(spec.Item));
        if (source is null)
        {
            actor.OutputHandler.Send("You cannot work out which container that item is in.");
            return;
        }

        StealItemFromContainer(actor, target, spec.Item, spec.Quantity, source);
    }

    private static void StealSpecificFromContainer(ICharacter actor, ICharacter target, string itemText,
        IGameItem containerItem)
    {
        var container = containerItem.GetItemType<IContainer>();
        if (TryParseCurrencyAmount(actor, itemText, out var amount, out var exact))
        {
            StealCurrencyFromContainer(actor, target, actor.Currency, amount, exact, containerItem);
            return;
        }

        if (!TryParseTransferSpec(actor, itemText, container.Contents, $"in {containerItem.HowSeen(actor)}",
                out var spec, out var error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        if (spec.Kind == StealthTransferKind.Currency)
        {
            StealCurrencyFromContainer(actor, target, spec.Currency, spec.Amount, spec.Exact, containerItem);
            return;
        }

        StealItemFromContainer(actor, target, spec.Item, spec.Quantity, containerItem);
    }

    private static void ExecuteStealCandidate(ICharacter actor, ICharacter target, StealCandidate candidate)
    {
        if (candidate.Kind == StealSourceKind.Belt)
        {
            StealBeltedItem(actor, target, candidate.Source, candidate.Item);
            return;
        }

        StealItemFromContainer(actor, target, candidate.Item, 0, candidate.Source);
    }

    private static void StealItemFromContainer(ICharacter actor, ICharacter target, IGameItem item, int quantity,
        IGameItem containerItem)
    {
        if (!actor.Body.CanGet(item, containerItem, quantity))
        {
            actor.OutputHandler.Send(actor.Body.WhyCannotGet(item, containerItem, quantity));
            return;
        }

        if (WouldStopLawfulAction(actor, CrimeTypes.Theft, target, item))
        {
            return;
        }

        if (!TryResolveStealth(actor, target, item, CheckType.StealCheck, CrimeTypes.Theft,
                $"steal {DescribeItemQuantity(actor, item, quantity)} from {target.HowSeen(actor)}", out var detection))
        {
            return;
        }

        var moved = actor.Body.Get(item, containerItem, quantity, null, true, ItemCanGetIgnore.None,
            detection.Noticers.Cast<IHandleEvents>());
        if (moved is null)
        {
            return;
        }

        actor.OutputHandler.Send(
            $"You steal {moved.HowSeen(actor)} from {containerItem.HowSeen(actor)}.{NoticedSuffix(detection)}");
        NotifyNoticers(actor, detection, witness =>
            $"You notice {actor.HowSeen(witness, true)} steal {moved.HowSeen(witness)} from {containerItem.HowSeen(witness)}.");
        RecordStealthCrime(actor, CrimeTypes.Theft, target, moved, detection);
    }

    private static void StealCurrencyFromContainer(ICharacter actor, ICharacter target, ICurrency currency,
        decimal amount, bool exact, IGameItem containerItem)
    {
        if (!actor.Body.CanGet(currency, containerItem, amount, exact))
        {
            actor.OutputHandler.Send(actor.Body.WhyCannotGet(currency, containerItem, amount, exact));
            return;
        }

        if (WouldStopLawfulAction(actor, CrimeTypes.Theft, target, containerItem))
        {
            return;
        }

        var description = currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal);
        if (!TryResolveStealth(actor, target, containerItem, CheckType.StealCheck, CrimeTypes.Theft,
                $"steal {description} from {target.HowSeen(actor)}", out var detection))
        {
            return;
        }

        var moved = actor.Body.Get(currency, containerItem, amount, exact, null, true,
            detection.Noticers.Cast<IHandleEvents>());
        if (moved is null)
        {
            return;
        }

        actor.OutputHandler.Send(
            $"You steal {moved.HowSeen(actor)} from {containerItem.HowSeen(actor)}.{NoticedSuffix(detection)}");
        NotifyNoticers(actor, detection, witness =>
            $"You notice {actor.HowSeen(witness, true)} steal {moved.HowSeen(witness)} from {containerItem.HowSeen(witness)}.");
        RecordStealthCrime(actor, CrimeTypes.Theft, target, moved, detection);
    }

    private static void StealBeltedItem(ICharacter actor, ICharacter target, IGameItem beltItem, IGameItem item)
    {
        if (!HasCutPurseTool(actor, out var error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        var belt = beltItem.GetItemType<IBelt>();
        var beltable = item.GetItemType<IBeltable>();
        if (belt is null || beltable is null || beltable.ConnectedTo != belt)
        {
            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not attached to {beltItem.HowSeen(actor)}.");
            return;
        }

        if (!actor.Body.CanGet(item, 0))
        {
            actor.OutputHandler.Send(actor.Body.WhyCannotGet(item, 0));
            return;
        }

        if (WouldStopLawfulAction(actor, CrimeTypes.Theft, target, item))
        {
            return;
        }

        if (!TryResolveStealth(actor, target, item, CheckType.StealCheck, CrimeTypes.Theft,
                $"cut {item.HowSeen(actor)} free from {target.HowSeen(actor)}", out var detection))
        {
            return;
        }

        belt.RemoveConnectedItem(beltable);
        var moved = actor.Body.Get(item, 0, null, true, ItemCanGetIgnore.None,
            detection.Noticers.Cast<IHandleEvents>());
        beltItem.InInventoryOf?.RecalculateItemHelpers();

        if (moved is null)
        {
            return;
        }

        actor.OutputHandler.Send(
            $"You cut {moved.HowSeen(actor)} free from {beltItem.HowSeen(actor)} and steal it.{NoticedSuffix(detection)}");
        NotifyNoticers(actor, detection, witness =>
            $"You notice {actor.HowSeen(witness, true)} cut {moved.HowSeen(witness)} free from {beltItem.HowSeen(witness)}.");
        RecordStealthCrime(actor, CrimeTypes.Theft, target, moved, detection);
    }

    private static IEnumerable<IGameItem> GetOpenStealableContainers(ICharacter actor, ICharacter target)
    {
        return target.Body.ExternalItemsForOtherActors
                     .Where(x => actor.CanSee(x))
                     .Where(x => x.IsItemType<IContainer>())
                     .Where(IsOpenContainer);
    }

    private static IEnumerable<IGameItem> GetStealableBelts(ICharacter actor, ICharacter target)
    {
        return target.Body.ExternalItemsForOtherActors
                     .Where(x => actor.CanSee(x))
                     .Where(x => x.IsItemType<IBelt>());
    }

    private static bool TryResolveStealSource(ICharacter actor, ICharacter target, string sourceText,
        out StealSource source)
    {
        source = null;
        var sourceItems = GetOpenStealableContainers(actor, target)
                          .Concat(GetStealableBelts(actor, target))
                          .Distinct()
                          .ToList();
        var item = sourceItems.GetFromItemListByKeyword(sourceText, actor);
        if (item is null)
        {
            return false;
        }

        source = new StealSource
        {
            Item = item,
            Kind = item.IsItemType<IContainer>() && IsOpenContainer(item)
                ? StealSourceKind.Container
                : StealSourceKind.Belt
        };
        return true;
    }

    private static IEnumerable<StealCandidate> GetStealCandidates(ICharacter actor, ICharacter target,
        bool requireCutPurseToolForBelts = false)
    {
        foreach (var containerItem in GetOpenStealableContainers(actor, target))
        {
            foreach (var item in containerItem.GetItemType<IContainer>().Contents
                                              .Where(x => actor.CanSee(x))
                                              .Where(x => actor.Body.CanGet(x, containerItem, 0)))
            {
                yield return new StealCandidate
                {
                    Kind = StealSourceKind.Container,
                    Source = containerItem,
                    Item = item
                };
            }
        }

        if (requireCutPurseToolForBelts && !HasCutPurseTool(actor, out _))
        {
            yield break;
        }

        foreach (var beltItem in GetStealableBelts(actor, target))
        {
            foreach (var item in beltItem.GetItemType<IBelt>().ConnectedItems
                                         .Select(x => x.Parent)
                                         .Where(x => actor.CanSee(x))
                                         .Where(x => actor.Body.CanGet(x, 0)))
            {
                yield return new StealCandidate
                {
                    Kind = StealSourceKind.Belt,
                    Source = beltItem,
                    Item = item
                };
            }
        }
    }

    private static bool IsOpenContainer(IGameItem item)
    {
        return item.GetItemType<IOpenable>()?.IsOpen != false;
    }

    private static bool HasCutPurseTool(ICharacter actor, out string error)
    {
        error = string.Empty;
        var tagText = actor.Gameworld.GetStaticConfiguration("CutPurseToolTagName");
        if (string.IsNullOrWhiteSpace(tagText))
        {
            error = "No cut-purse tool tag is configured in the CutPurseToolTagName static configuration.";
            return false;
        }

        var tag = long.TryParse(tagText, out var tagId)
            ? actor.Gameworld.Tags.Get(tagId)
            : actor.Gameworld.Tags.GetByName(tagText);
        if (tag is null)
        {
            error =
                $"The configured cut-purse tag {tagText.ColourCommand()} does not match any known tag.";
            return false;
        }

        if (actor.Body.HeldOrWieldedItems.Any(x => x.IsA(tag)))
        {
            return true;
        }

        error =
            $"You need to be holding or wielding something tagged {tag.FullName.ColourName()} to cut something loose.";
        return false;
    }

    private static string DescribeItemQuantity(ICharacter actor, IGameItem item, int quantity)
    {
        return quantity > 0 ? $"{quantity.ToString("N0", actor)} of {item.HowSeen(actor)}" : item.HowSeen(actor);
    }

    private static bool WouldStopLawfulAction(ICharacter actor, CrimeTypes? crime, ICharacter victim, IGameItem item)
    {
        if (crime is null || actor.IsAdministrator())
        {
            return false;
        }

        if (!crime.Value.CheckWouldBeACrime(actor, victim, item, string.Empty) || !actor.Account.ActLawfully)
        {
            return false;
        }

        actor.OutputHandler.Send($"That action would be a crime.\n{CrimeExtensions.StandardDisableIllegalFlagText}");
        return true;
    }

    private static bool TryResolveStealth(ICharacter actor, ICharacter victim, IGameItem focusItem, CheckType check,
        CrimeTypes? crime, string actionDescription, out StealthDetectionResult detection)
    {
        detection = ResolveStealthDetection(actor, victim, focusItem, check);
        if (!detection.ActorSucceeded)
        {
            actor.OutputHandler.Send($"You fumble your attempt to {actionDescription}.");
            NotifyNoticers(actor, detection, witness =>
                $"You notice {actor.HowSeen(witness, true)} trying to {actionDescription}.");
            RecordStealthCrime(actor, crime, victim, focusItem, detection);
            return false;
        }

        if (detection.VictimStopped)
        {
            actor.OutputHandler.Send($"{victim.HowSeen(actor, true)} notices what you are doing before you can finish.");
            NotifyNoticers(actor, detection, witness =>
                $"You notice {actor.HowSeen(witness, true)} trying to {actionDescription}.");
            RecordStealthCrime(actor, crime, victim, focusItem, detection);
            return false;
        }

        return true;
    }

    private static StealthDetectionResult ResolveStealthDetection(ICharacter actor, ICharacter victim,
        IGameItem focusItem, CheckType check)
    {
        IPerceivable checkTarget = victim is not null ? victim : focusItem;
        var result = new StealthDetectionResult
        {
            ActorOutcome = actor.Gameworld.GetCheck(check).Check(actor, Difficulty.Normal, checkTarget)
        };

        var observers = actor.Location.LayerCharacters(actor.RoomLayer)
                            .Where(x => x != actor)
                            .Where(x => x.CanSee(actor))
                            .Distinct()
                            .ToList();
        if (victim is not null && victim != actor && !observers.Contains(victim) && victim.CanSee(actor))
        {
            observers.Add(victim);
        }

        foreach (var observer in observers)
        {
            var observerOutcome = actor.Gameworld.GetCheck(CheckType.SpotStealthCheck)
                                       .Check(observer, observer.Location.SpotDifficulty(observer), actor);
            var opposed = new OpposedOutcome(result.ActorOutcome.Outcome, observerOutcome.Outcome);
            var notices = opposed.Outcome != OpposedOutcomeDirection.Proponent &&
                          (observerOutcome.IsPass() || result.ActorOutcome.IsFail());
            if (!notices)
            {
                continue;
            }

            result.Noticers.Add(observer);
            if (observer == victim && result.ActorOutcome.IsPass() &&
                opposed.Outcome == OpposedOutcomeDirection.Opponent &&
                opposed.Degree >= OpposedOutcomeDegree.Major)
            {
                result.VictimStopped = true;
            }
        }

        return result;
    }

    private static void NotifyNoticers(ICharacter actor, StealthDetectionResult detection,
        Func<ICharacter, string> message)
    {
        foreach (var witness in detection.Noticers.Where(x => x != actor))
        {
            witness.OutputHandler.Send(message(witness));
        }
    }

    private static string NoticedSuffix(StealthDetectionResult detection)
    {
        return detection.Noticers.Any() ? " You think someone may have noticed." : string.Empty;
    }

    private static void RecordStealthCrime(ICharacter actor, CrimeTypes? crime, ICharacter victim, IGameItem item,
        StealthDetectionResult detection)
    {
        if (crime is null || !detection.Noticers.Any())
        {
            return;
        }

        CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, crime.Value, victim, item, string.Empty,
            detection.Noticers, victim is not null && detection.Noticers.Contains(victim));
    }

    [PlayerCommand("Search", "search")]
    [DelayBlock("general", "You must first stop {0} before you can search.")]
    [CommandPermission(PermissionLevel.NPC)]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoMeleeCombatCommand]
    [HelpInfo("search", @"The #3search#0 command is used to search your surroundings for hidden things and people. This is a continuous action, and you will do it until you cancel it with the #3stop#0 command.

The syntax is simply #3search#0.", AutoHelp.HelpArg)]
    protected static void Search(ICharacter actor, string command)
    {
        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins searching the area.", actor)));
        actor.AddEffect(new Searching(actor), Searching.EffectDuration);
    }
}
