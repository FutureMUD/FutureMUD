using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;

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
	[HelpInfo("mount", @"", AutoHelp.HelpArg)]
	protected static void Mount(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"You would be able to ride the following creatures present:");
			sb.AppendLine();
			var any = false;
			foreach (var ch in actor.Location.LayerCharacters(actor.RoomLayer))
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

		var target = actor.TargetActor(ss.PopSpeech());
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
	[HelpInfo("dismount", @"", AutoHelp.HelpArg)]
	protected static void Dismount(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

	}

	[PlayerCommand("Buck", "buck")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoHideCommand]
	[HelpInfo("buck", @"", AutoHelp.HelpArg)]
	protected static void Buck(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
	}
}
