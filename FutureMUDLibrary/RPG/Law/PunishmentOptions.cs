using MudSharp.TimeAndDate;
using System.Collections.Generic;

namespace MudSharp.RPG.Law;

public class PunishmentOptions
{
    public MudTimeSpan GoodBehaviourBondLength { get; init; } = MudTimeSpan.Zero;
    public MudTimeSpan MinimumCustodialSentence { get; init; } = MudTimeSpan.Zero;
    public MudTimeSpan MaximumCustodialSentence { get; init; } = MudTimeSpan.Zero;
    public decimal MinimumFine { get; init; }
    public decimal MaximumFine { get; init; }
    public bool CanBeExecuted { get; init; }
    public List<(long ProgId, PunishmentKind Kinds)> OnPunishHooks { get; init; } = [];

    public PunishmentKind Kinds =>
        (MaximumFine > 0.0m ? PunishmentKind.Fine : PunishmentKind.None) |
        (MaximumCustodialSentence > MudTimeSpan.Zero ? PunishmentKind.Custody : PunishmentKind.None) |
        (GoodBehaviourBondLength > MudTimeSpan.Zero ? PunishmentKind.GoodBehaviourBond : PunishmentKind.None) |
        (CanBeExecuted ? PunishmentKind.Execution : PunishmentKind.None);

    public static PunishmentOptions operator +(PunishmentOptions r1, PunishmentOptions r2)
    {
        return new PunishmentOptions
        {
            MinimumFine = r1.MinimumFine + r2.MinimumFine,
            MaximumFine = r1.MaximumFine + r2.MaximumFine,
            MaximumCustodialSentence = r1.MaximumCustodialSentence + r2.MaximumCustodialSentence,
            MinimumCustodialSentence = r1.MinimumCustodialSentence + r2.MinimumCustodialSentence,
            GoodBehaviourBondLength = r1.GoodBehaviourBondLength + r2.GoodBehaviourBondLength,
            CanBeExecuted = r1.CanBeExecuted || r2.CanBeExecuted,
            OnPunishHooks = [.. r1.OnPunishHooks, .. r2.OnPunishHooks]
        };
    }
}