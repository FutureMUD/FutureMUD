#nullable enable

using MudSharp.Community;

namespace MudSharp.Commands.Modules;

public partial class ClanModule
{
	private static void ClanAddressPreview(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators may preview address resolution.");
			return;
		}
		var clanText = command.PopSpeech();
		var speakerText = command.PopSpeech();
		var subjectText = command.PopSpeech();
		var clan = actor.Gameworld.Clans.GetClan(clanText);
		var speaker = speakerText.EqualTo("none") ? null : actor.TargetActor(speakerText);
		var subject = actor.TargetActor(subjectText);
		var option = command.PopSpeech();
		if (clan is null || subject is null || speaker is null && !speakerText.EqualTo("none") ||
		    option.Length > 0 && !option.EqualTo("hidden") || !command.IsFinished)
		{
			actor.OutputHandler.Send("Use clan address <clan> <speaker|none> <subject> [hidden]. Select visible local characters; hidden explicitly bypasses disclosure checks for this diagnostic only.");
			return;
		}
		var result = ClanAddressResolver.Resolve(speaker, subject, clan, diagnostic: option.EqualTo("hidden"));
		actor.OutputHandler.Send($"Address: {(result.Text.Length == 0 ? "none" : result.Text).ColourValue()}\nStatus: {result.Status.DescribeEnum().ColourName()}\nSource: {result.Source.ColourName()}\n{result.Detail}");
	}
}
