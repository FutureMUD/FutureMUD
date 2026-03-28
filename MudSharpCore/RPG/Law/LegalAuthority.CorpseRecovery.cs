using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.GameItems;

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
		var existing = ActiveCorpseRecoveryReport(corpse);
		if (existing != null)
		{
			return existing;
		}

		var report = new CorpseRecoveryReport(this, zone, corpse, corpse.Location ?? corpse.TrueLocations.First(), reporter);
		_corpseRecoveryReports.Add(report);
		Changed = true;
		return report;
	}
}
