#nullable enable

using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed class PsychometryPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "psychometry";
	public static void RegisterLoader() => Register("psychometry", (m,w) => new PsychometryPower(m,w), (w,s,n,t) => new PsychometryPower(w,s,n,t));
	private PsychometryPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private PsychometryPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("<here|item> to read impressions.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!PsychometricRecorder.Enabled(Gameworld)) { actor.OutputHandler.Send("Psychometric impressions are disabled in this world."); return; }
		if (!HandleGeneralUseRestrictions(actor) || !CanAffordToInvokePower(actor, Verb).Truth) return;
		var text = command.SafeRemainingArgument;
		IPerceivable? target = text.EqualToAny("here", "room", "cell") ? actor.Location : actor.TargetItem(text);
		if (target is null || MagicInterdictionHelper.GetInterdiction(actor, target, School, false) is not null ||
		    CanInvokePowerProg.ExecuteBool(actor, target) == false) { actor.OutputHandler.Send("You cannot read impressions there."); return; }
		ConsumePowerCosts(actor, Verb);
		var outcome = CheckPower(actor, actor, CheckType.MagicTelepathyCheck);
		if (outcome.Outcome < MinimumSuccessThreshold) { Complete(actor, null, "an unsuccessful psychometric reading"); SendFailure(actor, actor); return; }
		SendSuccess(actor, actor);
		var history = PsychometricRecorder.Read(target);
		var sb = new StringBuilder();
		if (history is not null)
		{
			foreach (var impression in history.Impressions.Where(x => (target is MudSharp.GameItems.IGameItem || PsychometricRecorder.IsLocal(actor, x)) &&
			         (x.Kind != ImpressionKind.Feeling || outcome.Outcome == Outcome.MajorPass)))
			{
				sb.AppendLine(FormatEcho("ImpressionEcho", impression.Text, (RuntimeClock.UtcNow - impression.CreatedUtc).Describe(actor)));
			}
			if (history.MixedProvenance) sb.AppendLine(EchoText("MixedHistoryEcho"));
			foreach (var custody in history.PreviousCarriers.Concat(history.CurrentCarrier is { } current ? [current] : []))
			{
				var carrier = Gameworld.TryGetCharacter(custody.CarrierId, true);
				var identity = carrier is not null && outcome.Outcome == Outcome.MajorPass
					? PsionicTrafficHelper.SourceDescription(carrier, actor, School) : "an indistinct person";
				sb.AppendLine(FormatEcho("CustodyEcho", identity, custody.UnknownBeginning ? "at least " : "", ((custody.UntilUtc ?? RuntimeClock.UtcNow) - custody.SinceUtc).Describe(actor)));
			}
		}
		actor.OutputHandler.Send(sb.Length == 0 ? EchoText("NoImpressionsEcho") : sb.ToString());
		Complete(actor, null, "a psychometric reading");
	}
}
