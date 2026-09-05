#nullable enable

using MudSharp.Character;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public enum MentalActionKind
{
	Communication,
	Investigation,
	Influence,
	Disruption,
	ResourceTransfer,
	WitnessForgetting
}

public enum MagicInvocationStatus
{
	Refused,
	Failed,
	Succeeded
}

/// <summary>A single attempted mental action; reactions must not recursively create new attempts.</summary>
public sealed record MentalActionContext(ICharacter Source, ICharacter Target, IMagicPower Power,
	MentalActionKind Kind, bool Hostile);

public readonly record struct MagicInvocationResult(MagicInvocationStatus Status, Outcome Outcome = Outcome.NotTested)
{
	public bool Succeeded => Status == MagicInvocationStatus.Succeeded;
}

/// <summary>Defences observe the original attempt rather than interpreting presentation text.</summary>
public interface IMentalActionReaction
{
	void OnMentalAction(MentalActionContext context, MagicInvocationResult result);
}

public interface IMentalActionDefence
{
	double DefensiveBonus(MentalActionContext context);
}
