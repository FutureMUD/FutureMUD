using MudSharp.Effects;

#nullable enable

namespace MudSharp.Effects.Concrete;

internal sealed class ProjectWorkerSearchCooldownEffect : Effect, IEffectSubtype
{
	public ProjectWorkerSearchCooldownEffect(IPerceivable owner)
		: base(owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Project worker search cooldown.";
	}

	protected override string SpecificEffectType => "ProjectWorkerSearchCooldown";
}
