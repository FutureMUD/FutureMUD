using MudSharp.Commands.Helpers;
using MudSharp.Effects;

#nullable enable

namespace MudSharp.Effects.Concrete;

internal sealed class EmploymentTaskDraftEffect : Effect, IEffectSubtype
{
	public EmploymentTaskDraftEffect(IPerceivable owner, EmploymentTaskDraft draft)
		: base(owner)
	{
		Draft = draft;
	}

	public EmploymentTaskDraft Draft { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Drafting employment task {Draft.Name} for {Draft.Host.EmploymentHostName}.";
	}

	protected override string SpecificEffectType => "EmploymentTaskDraft";
}
