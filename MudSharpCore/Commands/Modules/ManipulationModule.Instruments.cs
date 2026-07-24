#nullable enable

using MudSharp.Commands.Trees;
using MudSharp.Effects.Concrete;

namespace MudSharp.Commands.Modules;

internal partial class ManipulationModule
{
	private const string PlayHelp = @"The #3play#0 command begins a sustained instrumental performance.

	#3play <instrument> [style]#0
	#3play <instrument> [style] (<emote>)#0

Use #3stop playing#0 to end the performance.";

	[PlayerCommand("Play", "play")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("play", PlayHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void PlayInstrument(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(PlayHelp.SubstituteANSIColour());
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		if (item is null)
		{
			actor.OutputHandler.Send("You do not see any instrument like that.");
			return;
		}

		var instrument = item.GetItemType<IInstrument>();
		if (instrument is null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not a playable instrument.");
			return;
		}

		var style = instrument.Styles.FirstOrDefault() ?? "general";
		PlayerEmote? emote = null;
		if (!ss.IsFinished && !ss.Peek().StartsWith('('))
		{
			style = ss.PopSpeech();
		}

		if (!ss.IsFinished)
		{
			var emoteText = ss.PopParentheses();
			if (string.IsNullOrEmpty(emoteText) || !ss.IsFinished)
			{
				actor.OutputHandler.Send(PlayHelp.SubstituteANSIColour());
				return;
			}

			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		instrument.Play(actor, style, emote);
	}

	private const string SignalInstrumentHelp = @"The #3signal#0 command sounds a named pattern on a signal instrument.

	#3signal <instrument> <pattern>#0
	#3signal <instrument> <pattern> (<emote>)#0";

	[PlayerCommand("Signal", "signal")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("signal", SignalInstrumentHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void SignalInstrument(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(SignalInstrumentHelp.SubstituteANSIColour());
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		if (item is null)
		{
			actor.OutputHandler.Send("You do not see any signal instrument like that.");
			return;
		}

		var instrument = item.GetItemType<ISignalInstrument>();
		if (instrument is null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not a signal instrument.");
			return;
		}

		var pattern = ss.PopSpeech();
		if (string.IsNullOrEmpty(pattern))
		{
			actor.OutputHandler.Send(
				$"Which signal? Available signals are {instrument.SignalPatterns.Select(x => x.Name).ListToString()}.");
			return;
		}

		PlayerEmote? emote = null;
		if (!ss.IsFinished)
		{
			var emoteText = ss.PopParentheses();
			if (string.IsNullOrEmpty(emoteText) || !ss.IsFinished)
			{
				actor.OutputHandler.Send(SignalInstrumentHelp.SubstituteANSIColour());
				return;
			}

			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		instrument.Signal(actor, pattern, emote);
	}
}
