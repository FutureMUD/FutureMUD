using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Commands.Socials;

public class Social : ISocial
{
    private readonly IExecutable<ICharacter> _command;

    public Social(MudSharp.Models.Social social, IFuturemud gameworld)
    {
        Name = social.Name;
        NoTargetEcho = social.NoTargetEcho;
        OneTargetEcho = social.OneTargetEcho;
        DirectionTargetEcho = social.DirectionTargetEcho;
        MultiTargetEcho = social.MultiTargetEcho;
        ApplicabilityProg = gameworld.FutureProgs.Get(social.FutureProgId ?? 0);
        _command = new Command<ICharacter>(SocialCommand);
    }

    public string Name { get; set; }
    public IFutureProg ApplicabilityProg { get; set; }
    public string NoTargetEcho { get; set; }
    public string OneTargetEcho { get; set; }
    public string DirectionTargetEcho { get; set; }
    public string MultiTargetEcho { get; set; }

    public bool Applies(object actor, string command, bool abbreviations)
    {
        return
            (ApplicabilityProg?.ExecuteBool(actor) ?? true) &&
            abbreviations
                ? Name.IndexOf(command, StringComparison.InvariantCultureIgnoreCase) >= 0
                : Name.Equals(command, StringComparison.InvariantCultureIgnoreCase);
    }

    public IExecutable<ICharacter> GetCommand()
    {
        return _command;
    }

    public void Execute(ICharacter actor, List<IPerceivable> targetList, ICellExit targetExit, IEmote playerEmote)
    {
        MixedEmoteOutput output = null;
        if (targetExit != null)
        {
            output = new MixedEmoteOutput(
                new Emote(string.Format(DirectionTargetEcho, targetExit.OutboundDirectionDescription), actor));
        }
        else
        {
            switch (targetList.Count)
            {
                case 1:
                    output = new MixedEmoteOutput(new Emote(NoTargetEcho, actor));
                    break;
                case 2:
                    output = new MixedEmoteOutput(new Emote(OneTargetEcho, actor, targetList.ToArray()));
                    break;
                default:
                    int index = 1;
                    output =
                        new MixedEmoteOutput(
                            new Emote(
                                string.Format(MultiTargetEcho,
                                    targetList.Skip(1).Select(x => "$" + index++).ListToString()), actor,
                                targetList.ToArray()));
                    break;
            }
        }

        output.Append(playerEmote);
        actor.OutputHandler.Handle(output);

        foreach (IPerceivable target in targetList)
        {
            target.HandleEvent(EventType.CharacterSocialTarget, actor, this, target, targetExit);
        }

        foreach (IPerceivable witness in actor.Location.Characters.Except(actor).Except(targetList))
        {
            witness.HandleEvent(EventType.CharacterSocialWitness, actor, this, targetList, targetExit, witness);
        }

        foreach (GameItems.IGameItem witness in actor.Body.ExternalItems)
        {
            witness.HandleEvent(EventType.CharacterSocialWitness, actor, this, targetList, targetExit, witness);
        }
    }

    private void SocialCommand(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        List<IPerceivable> targetList = new()
        { actor };
        ICellExit targetExit = null;

        if (!CharacterState.Able.HasFlag(actor.State))
        {
            actor.Send($"You cannot do that while you are {actor.State.Describe()}.");
            return;
        }

        string direction = string.IsNullOrEmpty(ss.PeekParentheses()) ? ss.Peek() : null;
        if (!string.IsNullOrEmpty(DirectionTargetEcho) && !string.IsNullOrEmpty(direction))
        {
            IEnumerable<ICellExit> exits = actor.Location.ExitsFor(actor);
            targetExit = exits.GetFromItemListByKeyword(direction, actor);
            if (targetExit == null &&
                Constants.DirectionStrings.Any(x => x.Equals(direction, StringComparison.InvariantCultureIgnoreCase)))
            {
                actor.OutputHandler.Send("There is no exit in that direction.");
                return;
            }
        }

        if (targetExit == null)
        {
            while (!ss.IsFinished && string.IsNullOrEmpty(ss.PeekParentheses()))
            {
                string targetText = ss.PopSpeech();
                IPerceivable target = actor.Target(targetText);
                if (target == null)
                {
                    actor.OutputHandler.Send("You do not see someone or something you can target with " +
                                             targetText.Colour(Telnet.Cyan) + ".");
                    return;
                }

                if (targetList.Contains(target))
                {
                    actor.Send("You cannot target {0} twice.", target.HowSeen(actor));
                    return;
                }

                targetList.Add(target);
            }

            if (targetList.Count > 2 && string.IsNullOrEmpty(MultiTargetEcho))
            {
                actor.Send("You can only use a single target with the {0} social.", Name);
                return;
            }

            if (targetList.Count == 1 && string.IsNullOrEmpty(NoTargetEcho))
            {
                actor.Send("Who do you wish to {0}?", Name);
                return;
            }
        }

        PlayerEmote emote = string.IsNullOrWhiteSpace(ss.PopParentheses()) ? null : new PlayerEmote(ss.Last, actor);
        if (!emote?.Valid == true)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return;
        }

        Execute(actor, targetList, targetExit, emote);
    }
}