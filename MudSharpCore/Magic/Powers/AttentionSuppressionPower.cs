#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;

namespace MudSharp.Magic.Powers;

public sealed class AttentionSuppressionPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "attentionsuppression";
	public static void RegisterLoader() => Register("attentionsuppression", (m,w) => new AttentionSuppressionPower(m,w), (w,s,n,t) => new AttentionSuppressionPower(w,s,n,t));
	private AttentionSuppressionPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private AttentionSuppressionPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise("begin or end to become difficult to notice.");
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("end")) { actor.RemoveAllEffects<AttentionSuppressionEffect>(); return; }
		if (!TryPrepareTarget(actor, new StringStack("self"), "", out _)) return;
		if (!Resolve(actor, actor, MentalActionKind.Influence, false, out _)) return;
		actor.RemoveAllEffects<AttentionSuppressionEffect>();
		actor.AddEffect(new AttentionSuppressionEffect(actor, SkillCheckDifficulty) { OriginPowerId = Id }, Duration);
		SendEcho("SuccessEcho", actor, actor);
		Complete(actor, actor, "attention suppression");
	}
}
