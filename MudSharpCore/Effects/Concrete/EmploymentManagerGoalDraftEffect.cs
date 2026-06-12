using MudSharp.Commands.Helpers;
using MudSharp.Effects;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Effects.Concrete;

internal sealed class EmploymentManagerGoalDraftEffect : Effect, IEffectSubtype
{
	public EmploymentManagerGoalDraftEffect(IPerceivable owner, EmploymentManagerGoalDraft draft)
		: base(owner)
	{
		Draft = draft;
	}

	public EmploymentManagerGoalDraft Draft { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Drafting manager goal {Draft.Description} for {Draft.Host.EmploymentHostName}.";
	}

	protected override string SpecificEffectType => "EmploymentManagerGoalDraft";
}
