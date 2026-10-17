using MudSharp.Construction.Boundary;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class ManipulationModule
{
	private static void OpenWithGuardChallenge(ICharacter actor, string command)
	{
		if (!HostileGuardChallenge.Enabled(actor)) return;
		var ss = new StringStack(command.RemoveFirstWord());
		var target = ss.PopSpeech();
		var selector = ss.PopSafe();
		var emote = new PlayerEmote(ss.PopParentheses(), actor);
		if (!emote.Valid) { actor.Send(emote.ErrorMessage); return; }
		if (!ss.IsFinished || actor.Combat is not null)
		{
			actor.Send("Guard challenges support only immediate open <item|exit> or open <door> <exit> actions.");
			return;
		}
		IRoomExit? exit = actor.Location.GetExitKeyword(selector.Length == 0 ? target : selector, actor);
		IGameItem? item;
		if (exit is not null)
		{
			if (!actor.Body.CanSee(actor.Location, exit)) { actor.Send("You do not see that here to open."); return; }
			item = exit.Exit.Door?.Parent;
			if (item is not null && selector.Length > 0 && new[] { item }.GetFromItemListByKeyword(target, actor) is null) item = null;
		}
		else if (selector.Length > 0)
		{
			actor.Send("Guard challenges cannot open another character's items or automation panels.");
			return;
		}
		else item = actor.Location.ExitsFor(actor).Where(x => actor.Body.CanSee(actor.Location, x))
			.SelectNotNull(x => x.Exit.Door).SelectNotNull(x => x?.Parent).GetFromItemListByKeyword(target, actor) ?? actor.TargetLocalItem(target);
		var openable = item?.GetItemType<IOpenable>();
		if (openable is null) { actor.Send("You do not see that here to open."); return; }
		using var attempt = HostileGuardChallenge.ForItem(actor, item!, null, GuardChallengeAction.Open);
		if (attempt is null) return;
		using var authority = attempt.Enter();
		var manipulation = actor.CanManipulateItem(item!);
		if (!manipulation.Truth) { actor.Send(manipulation.Message); return; }
		if (!actor.Body.CanOpen(openable))
		{
			actor.Send($"You cannot open {item!.HowSeen(actor)} because{actor.Body.WhyCannotOpen}");
			return;
		}
		if (attempt.Resolve()) actor.Body.Open(openable, null!, emote);
	}
}
