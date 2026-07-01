using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Estates;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.RPG.Law;

public partial class LegalAuthority
{
    private readonly List<ICorpseRecoveryReport> _corpseRecoveryReports = new();
    public IEnumerable<ICorpseRecoveryReport> CorpseRecoveryReports => _corpseRecoveryReports;

    public static ICorpseRecoveryReport ReportCorpseToLocalAuthority(IFuturemud gameworld, IGameItem corpse,
        ICharacter reporter, out string errorMessage)
    {
        errorMessage = string.Empty;
        ICorpse corpseComponent = corpse?.GetItemType<ICorpse>();
        if (corpseComponent == null)
        {
            errorMessage = "You do not see any such corpse here.";
            return null;
        }

        if (!corpseComponent.RepresentsFinalCharacterDeath)
        {
            errorMessage = "Those are body remains rather than the final corpse of a dead character, and cannot be reported to the morgue.";
            return null;
        }

        ICell sourceCell = corpse.Location ?? corpse.TrueLocations.FirstOrDefault();
        if (sourceCell == null)
        {
            errorMessage = "That corpse is not currently in a reportable location.";
            return null;
        }

        ILegalAuthority authority = gameworld.LegalAuthorities.FirstOrDefault(x => x.EnforcementZones.Contains(sourceCell.Zone));
        if (authority == null)
        {
            errorMessage = "There is no local legal authority that can respond to this corpse.";
            return null;
        }

        IEconomicZone zone = Estate.DetermineZone(gameworld, sourceCell);
        if (zone == null || zone.MorgueOfficeCell == null || zone.MorgueStorageCell == null)
        {
            errorMessage = "There is no configured morgue for the economic zone that covers this corpse.";
            return null;
        }

        if (authority.ActiveCorpseRecoveryReport(corpse) != null)
        {
            errorMessage = "That corpse has already been reported to the authorities.";
            return null;
        }

        return authority.ReportCorpse(corpse, zone, reporter);
    }

    public ICorpseRecoveryReport ActiveCorpseRecoveryReport(IGameItem corpse)
    {
        return _corpseRecoveryReports.FirstOrDefault(x =>
            x.Corpse?.Id == corpse.Id &&
            x.Status.In(CorpseRecoveryReportStatus.Pending, CorpseRecoveryReportStatus.Assigned));
    }

    public ICorpseRecoveryReport ReportCorpse(IGameItem corpse, IEconomicZone zone, ICharacter reporter)
    {
        ICorpse corpseComponent = corpse.GetItemType<ICorpse>();
        if (corpseComponent == null || !corpseComponent.RepresentsFinalCharacterDeath)
        {
            return null;
        }

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
