
namespace MudSharp.Commands.Modules;

public class RidingModule : Module<ICharacter>
{
    private RidingModule() : base("Riding")
    {
        IsNecessary = true;
    }

    public static RidingModule Instance { get; } = new();

    [PlayerCommand("Mount", "mount", "ride")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoHideCommand]
    [HelpInfo("mount", @"The #3mount#0 command mounts a rideable creature. Bareback riding may be possible, but appropriate saddles, bridles, reins and other tack improve control and make staying mounted easier.

The syntax is:

	#3mount <mount>#0", AutoHelp.HelpArg)]
    protected static void Mount(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (ss.IsFinished)
        {
            StringBuilder sb = new();
            sb.AppendLine($"You would be able to ride the following creatures present:");
            sb.AppendLine();
            bool any = false;
            foreach (ICharacter ch in actor.Location.LayerCharacters(actor.RoomLayer))
            {
                if (actor == ch)
                {
                    continue;
                }

                if (!ch.CanEverBeMounted(actor))
                {
                    continue;
                }

                if (!actor.CanSee(ch))
                {
                    continue;
                }

                any = true;

                if (ch.CanBeMountedBy(actor))
                {
                    sb.AppendLine($"\t{ch.HowSeen(actor)} - {"Mountable".Colour(Telnet.Green)}");
                    continue;
                }

                sb.AppendLine($"\t{ch.HowSeen(actor)} - {"Not Currently Mountable".Colour(Telnet.BoldYellow)}");
            }

            if (!any)
            {
                sb.AppendLine($"\tNone");
            }

            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        if (target is null)
        {
            actor.OutputHandler.Send($"You don't see anyone like that here.");
            return;
        }

        if (!target.CanBeMountedBy(actor))
        {
            actor.OutputHandler.Send(target.WhyCannotBeMountedBy(actor));
            return;
        }

        target.Mount(actor);
    }

    [PlayerCommand("Dismount", "dismount")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoHideCommand]
    [HelpInfo("dismount", @"The #3dismount#0 command gets your character off their current mount.

The syntax is:

	#3dismount#0", AutoHelp.HelpArg)]
    protected static void Dismount(ICharacter actor, string command)
    {
        if (actor.RidingMount is null)
        {
            actor.OutputHandler.Send("You are not currently riding a mount.");
            return;
        }


        actor.RidingMount.Dismount(actor);
    }

    [PlayerCommand("Buck", "buck")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoMovementCommand]
    [NoHideCommand]
    [HelpInfo("buck", @"The #3buck#0 command makes a mount attempt to throw off a rider. With no argument it targets the primary rider; name a rider to target that rider instead.

The syntax is:

	#3buck [<rider>]#0", AutoHelp.HelpArg)]
    protected static void Buck(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (!actor.Riders.Any())
        {
            actor.OutputHandler.Send("You are not currently carrying any riders.");
            return;
        }

        ICharacter target;
        if (ss.IsFinished)
        {
            target = actor.Riders.First();
        }
        else
        {
            target = actor.TargetActor(ss.SafeRemainingArgument);
            if (target is null || !actor.Riders.Contains(target))
            {
                actor.OutputHandler.Send("You are not carrying any rider like that.");
                return;
            }
        }

        if (!actor.BuckRider(target))
        {
            actor.OutputHandler.Send(new PerceptionEngine.Outputs.EmoteOutput(
                new PerceptionEngine.Parsers.Emote("$1 keep|keeps $1's seat despite $0's bucking.", actor, actor,
                    target)));
        }
    }
}
