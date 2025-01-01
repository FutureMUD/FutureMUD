using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Strategies.BodyStratagies;

namespace MudSharp.Body.CommunicationStrategies;

public class HumanoidCommunicationStrategy : IBodyCommunicationStrategy
{
    protected HumanoidCommunicationStrategy()
    {
    }

    string IBodyCommunicationStrategy.Name => "Humanoid";

    public static IBodyCommunicationStrategy Instance { get; } = new HumanoidCommunicationStrategy();

    public bool IsGagged(IBody body)
    {
        return body.Bodyparts.OfType<MouthProto>().Any() && body.Bodyparts.OfType<MouthProto>()
                                                                .All(x => body.WornItemsFor(x)
                                                                              .Any(y => y.IsItemType<IGag>()));
    }

    public virtual string WhyCannotVocalise(IBody body)
    {
        if (!body.Organs.Any(x => x is TracheaProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is TracheaProto) || !x.Functional))
        {
            return $"You cannot speak because you do not have a trachea or a functional prosthetic replacement.";
        }

        if (!body.Bodyparts.Any(x => x is TongueProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is TongueProto) || !x.Functional))
        {
            return $"You cannot speak because you do not have a tongue or a functional prosthetic replacement.";
        }

        if (!body.Bodyparts.Any(x => x is MouthProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is MouthProto) || !x.Functional))
        {
            return $"You cannot speak because you do not have a mouth or a functional prosthetic replacement.";
        }

        if (body.OrganFunction<TracheaProto>() < 0.5)
        {
            return "You cannot speak because your trachea is not working well enough.";
        }

        foreach (var part in body.Bodyparts.OfType<TongueProto>().ToList())
        {
            if (body.EffectsOfType<IBodypartIneffectiveEffect>().Any(x => x.Applies() && x.Bodypart == part) &&
                body.Prosthetics.All(x => x.TargetBodypart != part || !x.Functional))
            {
                return
                    $"You cannot speak because your tongue is too damaged and you do not have a functional prosthetic in its place.";
            }

            if (body.Wounds.Where(x => x.Bodypart == part).Sum(x => Math.Max(x.CurrentDamage, x.CurrentPain)) >=
                body.HitpointsForBodypart(part) * 0.75)
            {
                return $"You cannot speak because your tongue is too damaged.";
            }
        }

        if (body.NeedsToBreathe && !body.CanBreathe)
        {
            return $"You cannot speak because you cannot breathe!";
        }

        if (body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor)))
        {
            return $"You are mute and cannot speak.";
        }

        throw new ApplicationException();
    }

    public virtual bool CanVocalise(IBody body)
    {
        if (!body.Organs.Any(x => x is TracheaProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is TracheaProto) || !x.Functional))
        {
            return false;
        }

        if (!body.Bodyparts.Any(x => x is TongueProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is TongueProto) || !x.Functional))
        {
            return false;
        }

        if (!body.Bodyparts.Any(x => x is MouthProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is MouthProto) || !x.Functional))
        {
            return false;
        }

        if (body.OrganFunction<TracheaProto>() < 0.5)
        {
            return false;
        }

        foreach (var part in body.Bodyparts.OfType<TongueProto>().ToList())
        {
            if (body.EffectsOfType<IBodypartIneffectiveEffect>().Any(x => x.Applies() && x.Bodypart == part) &&
                body.Prosthetics.All(x => x.TargetBodypart != part || !x.Functional))
            {
                return false;
            }

            if (body.Wounds.Where(x => x.Bodypart == part).Sum(x => Math.Max(x.CurrentDamage, x.CurrentPain)) >=
                body.HitpointsForBodypart(part) * 0.75)
            {
                return false;
            }
        }

        if (body.NeedsToBreathe && !body.CanBreathe)
        {
            return false;
        }

        return !body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor));
    }

    public virtual bool CanVocalise(IBody body, AudioVolume volume)
    {
        return CanVocalise(body); // TODO
    }

    public virtual string WhyCannotVocalise(IBody body, AudioVolume volume)
    {
        return WhyCannotVocalise(body); // TODO
    }

    public virtual PermitLanguageOptions VocalisationOption(IBody body, AudioVolume volume)
    {
        if (!body.Organs.Any(x => x is TracheaProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is TracheaProto) || !x.Functional))
        {
            return PermitLanguageOptions.LanguageIsGasping;
        }

        if (!body.Bodyparts.Any(x => x is TongueProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is TongueProto) || !x.Functional))
        {
            return PermitLanguageOptions.LanguageIsChoking;
        }

        if (!body.Bodyparts.Any(x => x is MouthProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is MouthProto) || !x.Functional))
        {
            return PermitLanguageOptions.LanguageIsMuffling;
        }

        if (body.OrganFunction<TracheaProto>() < 0.5)
        {
            return PermitLanguageOptions.LanguageIsGasping;
        }

        foreach (var part in body.Bodyparts.OfType<TongueProto>().ToList())
        {
            if (body.EffectsOfType<IBodypartIneffectiveEffect>().Any(x => x.Applies() && x.Bodypart == part) &&
                body.Prosthetics.All(x => x.TargetBodypart != part || !x.Functional))
            {
                return PermitLanguageOptions.LanguageIsChoking;
            }

            if (body.Wounds.Where(x => x.Bodypart == part).Sum(x => Math.Max(x.CurrentDamage, x.CurrentPain)) >=
                body.HitpointsForBodypart(part) * 0.75)
            {
                return PermitLanguageOptions.LanguageIsChoking;
            }
        }

        if (body.NeedsToBreathe && !body.CanBreathe)
        {
            return PermitLanguageOptions.LanguageIsChoking;
        }

        if (body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor)))
        {
            return body.Actor.Merits.OfType<IMuteMerit>().First(x => x.Applies(body.Actor)).LanguageOptions;
        }

        return PermitLanguageOptions.PermitLanguage;
    }

    public virtual void Emote(IBody body, string emote, bool permitSpeech = true,
        OutputFlags additionalConditions = OutputFlags.Normal)
    {
        var emoteData = new PlayerEmote(emote, body.Actor, true, VocalisationOption(body, AudioVolume.Decent)
        );
        if (emoteData.Valid)
        {
            body.OutputHandler.Handle(new EmoteOutput(emoteData, flags: additionalConditions));
        }
        else
        {
            body.OutputHandler.Send(emoteData.ErrorMessage);
        }
    }

    public virtual void LoudSay(IBody body, IPerceivable target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.Loud))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
                $"@ speak|speaks loudly but incomprehensibly into &0's gag{(target == null ? "" : $" at $1")}", body,
                body, target)).Append(emote));
            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Loud, message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        string actionText;
        if (target == null)
        {
            actionText = message.Last() == '?'
                ? "loudly ask|asks"
                : message.Last() == '!'
                    ? "loudly exclaim|exclaims"
                    : "loudly say|says";
        }
        else
        {
            actionText = message.Last() == '?'
                ? "loudly ask|asks $0"
                : message.Last() == '!'
                    ? "tell|tells $0 loudly and emphatically"
                    : "loudly tell|tells $0";
        }

        body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), langinfo,
            emote));
        HandleSpeechEvents(body, target, message, AudioVolume.Loud, body.CurrentLanguage, body.CurrentAccent);
    }

    public virtual void Talk(IBody body, IPerceivable target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.Quiet))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
                $"@ murmur|murmurs incomprehensibly into &0's gag{(target == null ? "" : $" at $1")}", body, body,
                target)).Append(emote));
            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Quiet, message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        string actionText;
        if (target == null)
        {
            actionText = message.Last() == '?'
                ? "murmur|murmurs questioningly"
                : message.Last() == '!'
                    ? "murmur|mumurs emphatically"
                    : "murmur|murmurs";
        }
        else
        {
            actionText = message.Last() == '?'
                ? "murmur|murmurs questioningly to $0"
                : message.Last() == '!'
                    ? "murmur|mumurs emphatically to $0"
                    : "murmur|murmurs to $0";
        }

        body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), langinfo,
            emote));
        HandleSpeechEvents(body, target, message, AudioVolume.Quiet, body.CurrentLanguage, body.CurrentAccent);
    }

    public void Transmit(IBody body, IGameItem target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.Quiet))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Send("You cannot transmit while you are gagged.");
            return;
        }

        ITransmit item;
        if (target is null)
        {
            item = body.ExternalItems.SelectNotNull(x => x.GetItemType<ITransmit>())
                       .FirstOrDefault(x => x.ManualTransmit);
            if (item == null)
            {
                body.OutputHandler.Send("You don't have items that allow you to transmit.");
                return;
            }
        }
        else
        {
            item = target.GetItemType<ITransmit>();
        }

        if (item is null)
        {
            body.OutputHandler.Send($"{target.HowSeen(body, true)} is not something that you can transmit with.");
            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Quiet, message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, null);

        body.OutputHandler.Handle(
            new LanguageOutput(new Emote(item.TransmitPremote, body.Actor, body.Actor, item.Parent), langinfo, emote));
        HandleSpeechEvents(body, null, message, AudioVolume.Quiet, body.CurrentLanguage, body.CurrentAccent);
        item.Transmit(langinfo);
    }

    public virtual void Yell(IBody body, IPerceivable target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.VeryLoud))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
                    $"@ yell|yells incomprehensibly into &0's gag{(target == null ? "" : $" at $1")}", body, body,
                    target))
                .Append(emote));
            foreach (var layer in body.Location.Terrain(null).TerrainLayers.Except(body.RoomLayer))
            {
                var layerText = layer.IsHigherThan(body.RoomLayer) ? "from below" : "from above";
                foreach (var character in body.Location.LayerCharacters(layer))
                {
                    character.OutputHandler.Send(new EmoteOutput(
                        new Emote($"You hear a muffled shout {layerText}.", body.Actor),
                        flags: OutputFlags.PurelyAudible));
                }
            }

            foreach (var cell in body.Location.Surrounds)
                foreach (var character in cell.Characters)
                {
                    var exit = body.Location.GetExitTo(cell, character);
                    character.OutputHandler.Send(new EmoteOutput(new Emote(
                        $"You hear a muffled yell {(exit != null ? exit.InboundDirectionSuffix : "from somewhere unknown")}.",
                        body.Actor), flags: OutputFlags.PurelyAudible | OutputFlags.NoticeCheckRequired));
                }

            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.VeryLoud,
            message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        var otherRoomLanginfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Faint,
            message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        string actionText;
        if (target == null)
        {
            actionText = message.Last() == '?'
                ? "yell|yells questioningly"
                : message.Last() == '!'
                    ? "yell|yells emphatically"
                    : "yell|yells";
        }
        else
        {
            actionText = message.Last() == '?'
                ? "yell|yells questioningly at $0"
                : message.Last() == '!'
                    ? "yell|yells emphatically at $0"
                    : "yell|yells at $0";
        }

        body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), langinfo,
            emote));
        HandleSpeechEvents(body, target, message, AudioVolume.Loud, body.CurrentLanguage, body.CurrentAccent);
        foreach (var layer in body.Location.Terrain(null).TerrainLayers.Except(body.RoomLayer))
        {
            var layerText = layer.IsHigherThan(body.RoomLayer) ? "from below" : "from above";
            foreach (var character in body.Location.LayerCharacters(layer))
            {
                character.OutputHandler.Send(new LanguageOutput(new Emote(
                    $"You hear a {body.ApparentGender(character).GenderClass()} voice {layerText} yell",
                    body.Actor), otherRoomLanginfo, emote));
            }
        }

        foreach (var cell in body.Location.Surrounds)
            foreach (var character in cell.Characters)
            {
                var exit = body.Location.GetExitTo(cell, character);
                character.OutputHandler.Send(new LanguageOutput(new Emote(
                    $"You hear a {body.Gender.GenderClass()} voice {(exit != null ? exit.InboundDirectionSuffix : "from somewhere unknown")} yell",
                    body.Actor), otherRoomLanginfo, emote));
                character.HandleEvent(EventType.CharacterSpeaksNearbyWitness, body.Actor, target, character,
                    AudioVolume.Faint, body.CurrentLanguage, body.CurrentAccent, message);
            }
    }

    public virtual void Say(IBody body, IPerceivable target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.Decent))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
                $"@ speak|speaks incomprehensibly into &0's gag{(target == null ? "" : $" at $1")}", body, body,
                target)).Append(emote));
            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Decent, message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        string actionText;
        if (target == null)
        {
            actionText = message.Last() == '?'
                ? "ask|asks"
                : message.Last() == '!'
                    ? "exclaim|exclaims"
                    : "say|says";
        }
        else
        {
            actionText = message.Last() == '?'
                ? "ask|asks $0"
                : message.Last() == '!'
                    ? "tell|tells $0 emphatically"
                    : "tell|tells $0";
        }

        body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), langinfo,
            emote));
        HandleSpeechEvents(body, target, message, AudioVolume.Decent, body.CurrentLanguage, body.CurrentAccent);
    }

    public virtual void Whisper(IBody body, IPerceivable target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.Quiet))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
                $"@ whisper|whispers quietly but incomprehensibly into &0's gag{(target == null ? "" : $" at $1")}",
                body, body, target)).Append(emote));
            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Quiet, message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        string actionText;
        if (target == null)
        {
            actionText = message.Last() == '?'
                ? "whisper|whispers questioningly"
                : message.Last() == '!'
                    ? "whisper|whispers emphatically"
                    : "whisper|whispers";
        }
        else
        {
            actionText = message.Last() == '?'
                ? "whisper|whispers questioningly to $0"
                : message.Last() == '!'
                    ? "whisper|whispers emphatically to $0"
                    : "whisper|whispers to $0";
        }

        body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), langinfo,
            emote));
        HandleSpeechEvents(body, target, message, AudioVolume.Quiet, body.CurrentLanguage, body.CurrentAccent);
    }

    public virtual void Shout(IBody body, IPerceivable target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.ExtremelyLoud))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
                $"@ shout|shouts incomprehensibly into &0's gag{(target == null ? "" : $" at $1")}", body, body,
                target)).Append(emote));
            foreach (var layer in body.Location.Terrain(null).TerrainLayers.Except(body.RoomLayer))
            {
                var layerText = layer.IsHigherThan(body.RoomLayer) ? "from below" : "from above";
                foreach (var character in body.Location.LayerCharacters(layer))
                {
                    character.OutputHandler.Send(new EmoteOutput(
                        new Emote($"You hear a muffled shout {layerText}.", body.Actor),
                        flags: OutputFlags.PurelyAudible));
                }
            }

            foreach (var cell in body.Location.Surrounds)
                foreach (var character in cell.Characters)
                {
                    var exit = body.Location.GetExitTo(cell, character);
                    character.OutputHandler.Send(new EmoteOutput(
                        new Emote(
                            $"You hear a muffled shout {(exit != null ? exit.InboundDirectionSuffix : "from somewhere unknown")}.",
                            body.Actor), flags: OutputFlags.PurelyAudible));
                }

            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.ExtremelyLoud,
            message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        var secondRoomLanginfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Decent,
            message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        var thirdRoomLanginfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Faint,
            message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        string actionText;
        if (target == null)
        {
            actionText = message.Last() == '?'
                ? "shout|shouts questioningly"
                : message.Last() == '!'
                    ? "shout|shouts emphatically"
                    : "shout|shouts";
        }
        else
        {
            actionText = message.Last() == '?'
                ? "shout|shouts questioningly at $0"
                : message.Last() == '!'
                    ? "shout|shouts emphatically at $0"
                    : "shout|shouts at $0";
        }

        body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), langinfo,
            emote));
        HandleSpeechEvents(body, target, message, AudioVolume.ExtremelyLoud, body.CurrentLanguage,
            body.CurrentAccent);
        foreach (var layer in body.Location.Terrain(null).TerrainLayers.Except(body.RoomLayer))
        {
            var layerText = layer.IsHigherThan(body.RoomLayer) ? "from below" : "from above";
            foreach (var character in body.Location.LayerCharacters(layer))
            {
                character.OutputHandler.Send(new LanguageOutput(new Emote(
                    $"You hear a {body.Gender.GenderClass()} voice {layerText} shout",
                    body.Actor), secondRoomLanginfo, emote));
            }
        }

        var allCells = body.Location.CellsInVicinity(2, exit => true, cell => true).ToList();
        var surrounds = body.Location.Surrounds.ToList();
        foreach (var cell in allCells)
        {
            if (cell == body.Location)
            {
                continue;
            }

            foreach (var character in cell.Characters)
            {
                var directionText = string.Empty;
                SpokenLanguageInfo info;
                if (surrounds.Contains(cell))
                {
                    directionText = body.Location.GetExitTo(cell, character)?.InboundDirectionSuffix ??
                                    "from somewhere unknown";
                    info = secondRoomLanginfo;
                }
                else
                {
                    directionText = cell.PathBetween(body.Actor, 2, PathSearch.IgnorePresenceOfDoors)
                                        .DescribeDirectionsToFrom();
                    info = thirdRoomLanginfo;
                }

                character.OutputHandler.Send(new LanguageOutput(new Emote(
                    $"You hear a {body.Gender.GenderClass()} voice {directionText} shout",
                    body.Actor), info, emote));
                character.HandleEvent(EventType.CharacterSpeaksNearbyWitness, body.Actor, target, character,
                    info.Volume, body.CurrentLanguage, body.CurrentAccent, message);
            }
        }
    }

    public virtual void Sing(IBody body, IPerceivable target, string message, IEmote emote = null)
    {
        if (!CanVocalise(body, AudioVolume.Loud))
        {
            body.OutputHandler.Send(WhyCannotVocalise(body));
            return;
        }

        if (IsGagged(body))
        {
            body.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
                    $"@ sing|sings incomprehensibly into &0's gag{(target == null ? "" : $" at $1")}", body, body,
                    target))
                .Append(emote));
            return;
        }

        var langinfo = new SpokenLanguageInfo(body.CurrentLanguage, body.CurrentAccent, AudioVolume.Loud, message,
            body.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
                .Check(body.Actor, Difficulty.Normal, body.CurrentLanguage.LinkedTrait), body.Actor, target);
        string actionText;
        if (target == null)
        {
            actionText = message.Last() == '?'
                ? "sing|sings questioningly"
                : message.Last() == '!'
                    ? "emphatically sing|sings"
                    : "sing|sings";
        }
        else
        {
            actionText = message.Last() == '?'
                ? "sing|sings questioningly to $0"
                : message.Last() == '!'
                    ? "emphatically sing|sings to $0"
                    : "sing|sings to $0";
        }

        body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), langinfo,
            emote));
        HandleSpeechEvents(body, target, message, AudioVolume.Loud, body.CurrentLanguage, body.CurrentAccent);
    }

    private static void HandleSpeechEvents(IBody body, IPerceivable target, string message, AudioVolume volume,
        ILanguage language, IAccent accent)
    {
        body.AccentDifficulty(accent, true);
        if (target == null)
        {
            body.Actor.HandleEvent(EventType.CharacterSpeaks, body.Actor, volume, language, accent, message);
            foreach (var witness in body.Location.EventHandlers.Except(body.Actor))
            {
                witness.HandleEvent(EventType.CharacterSpeaksWitness, body.Actor, witness, volume, language, accent,
                    message);
            }

            foreach (var witness in body.ExternalItems)
            {
                witness.HandleEvent(EventType.CharacterSpeaksWitness, body.Actor, witness, volume, language, accent,
                    message);
            }
        }
        else
        {
            body.Actor.HandleEvent(EventType.CharacterSpeaksDirect, body.Actor, target, volume, language, accent,
                message);
            target.HandleEvent(EventType.CharacterSpeaksDirectTarget, body.Actor, target, volume, language, accent,
                message);
            foreach (var witness in body.Location.EventHandlers.Except(body.Actor))
            {
                witness.HandleEvent(EventType.CharacterSpeaksDirectWitness, body.Actor, target, witness, volume,
                    language, accent,
                    message);
            }

            foreach (var witness in body.ExternalItems)
            {
                witness.HandleEvent(EventType.CharacterSpeaksDirectWitness, body.Actor, target, witness, volume,
                    language, accent,
                    message);
            }
        }
    }
}