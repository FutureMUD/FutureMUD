using MudSharp.Commands.Helpers;
using MudSharp.Effects;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Effects.Concrete;

internal sealed class EmploymentScheduledRuleDraftEffect : Effect, IEffectSubtype
{
	public EmploymentScheduledRuleDraftEffect(IPerceivable owner, EmploymentScheduledRuleDraft draft)
		: base(owner)
	{
		Draft = draft;
	}

	public EmploymentScheduledRuleDraft Draft { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Drafting scheduled employment rule {Draft.Name} for {Draft.Host.EmploymentHostName}.";
	}

	protected override string SpecificEffectType => "EmploymentScheduledRuleDraft";
}
