using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.RPG.Law;

public enum CorpseRecoveryReportStatus
{
    Pending,
    Assigned,
    Completed,
    Failed,
    Cancelled
}

public interface ICorpseRecoveryReport : IFrameworkItem, ISaveable
{
    ILegalAuthority LegalAuthority { get; }
    IEconomicZone EconomicZone { get; }
    IGameItem Corpse { get; }
    ICell SourceCell { get; }
    ICell DestinationCell { get; }
    ICharacter Reporter { get; }
    CorpseRecoveryReportStatus Status { get; set; }
    IPatrol AssignedPatrol { get; }
    void AssignPatrol(IPatrol patrol);
    void MarkCompleted();
    void MarkFailed();
}
