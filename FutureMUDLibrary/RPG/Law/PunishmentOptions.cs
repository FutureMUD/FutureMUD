using System;
using MudSharp.TimeAndDate;

namespace MudSharp.RPG.Law;

public class PunishmentOptions
{
	public MudTimeSpan GoodBehaviourBondLength { get; init; }
	public MudTimeSpan MinimumCustodialSentence { get; init; }
	public MudTimeSpan MaximumCustodialSentence { get; init; }
	public decimal MinimumFine { get; init; }
	public decimal MaximumFine { get; init; }
	public bool CanBeExecuted { get; init; }

	public static PunishmentOptions operator +(PunishmentOptions r1, PunishmentOptions r2)
	{
		return new PunishmentOptions
		{
			MinimumFine = Math.Min(r1.MinimumFine, r2.MinimumFine),
			MaximumFine = Math.Max(r1.MaximumFine, r2.MaximumFine),
			MaximumCustodialSentence = r1.MaximumCustodialSentence > r2.MaximumCustodialSentence ? r1.MaximumCustodialSentence : r2.MaximumCustodialSentence,
			MinimumCustodialSentence = r1.MaximumCustodialSentence < r2.MaximumCustodialSentence ? r1.MaximumCustodialSentence : r2.MaximumCustodialSentence,
			GoodBehaviourBondLength = r1.GoodBehaviourBondLength + r2.GoodBehaviourBondLength,
			CanBeExecuted = r1.CanBeExecuted || r2.CanBeExecuted,
		};
	}
}