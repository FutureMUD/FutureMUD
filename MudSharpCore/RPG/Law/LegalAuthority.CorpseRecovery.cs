using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.GameItems;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.RPG.Law;

public partial class LegalAuthority
{
    private readonly List<ICorpseRecoveryReport> _corpseRecoveryReports = new();
    public IEnumerable<ICorpseRecoveryReport> CorpseRecoveryReports => _corpseRecoveryReports;

    public ICorpseRecoveryReport ActiveCorpseRecoveryReport(IGameItem corpse)
    {
        return _corpseRecoveryReports.FirstOrDefault(x =>
            x.Corpse?.Id == corpse.Id &&
            x.Status.In(CorpseRecoveryReportStatus.Pending, CorpseRecoveryReportStatus.Assigned));
    }

    public ICorpseRecoveryReport ReportCorpse(IGameItem corpse, IEconomicZone zone, ICharacter reporter)
    {
        ICorpseRecoveryReport existing = ActiveCorpseRecoveryReport(corpse);
        if (existing != null)
        {
            return existing;
        }

        CorpseRecoveryReport report = new(this, zone, corpse, corpse.Location ?? corpse.TrueLocations.First(), reporter);
        _corpseRecoveryReports.Add(report);
        Changed = true;
        return report;
    }
}
