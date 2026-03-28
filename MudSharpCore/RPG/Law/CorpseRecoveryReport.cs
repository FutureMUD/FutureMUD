using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.RPG.Law;

public class CorpseRecoveryReport : SaveableItem, ICorpseRecoveryReport
{
	private readonly IFuturemud _gameworld;
	private readonly long _legalAuthorityId;
	private readonly long _economicZoneId;
	private readonly long _corpseId;
	private readonly long _sourceCellId;
	private readonly long _destinationCellId;
	private readonly long? _reporterId;
	private long? _assignedPatrolId;

	public CorpseRecoveryReport(MudSharp.Models.CorpseRecoveryReport dbitem, IFuturemud gameworld)
	{
		_gameworld = gameworld;
		_id = dbitem.Id;
		_legalAuthorityId = dbitem.LegalAuthorityId;
		_economicZoneId = dbitem.EconomicZoneId;
		_corpseId = dbitem.CorpseId;
		_sourceCellId = dbitem.SourceCellId;
		_destinationCellId = dbitem.DestinationCellId;
		_reporterId = dbitem.ReporterId;
		_assignedPatrolId = dbitem.AssignedPatrolId;
		Status = (CorpseRecoveryReportStatus)dbitem.Status;
	}

	public CorpseRecoveryReport(ILegalAuthority authority, IEconomicZone economicZone, IGameItem corpse, ICell sourceCell,
		ICharacter reporter)
	{
		_gameworld = authority.Gameworld;
		_legalAuthorityId = authority.Id;
		_economicZoneId = economicZone.Id;
		_corpseId = corpse.Id;
		_sourceCellId = sourceCell.Id;
		_destinationCellId = economicZone.MorgueStorageCell.Id;
		_reporterId = reporter?.Id;
		Status = CorpseRecoveryReportStatus.Pending;

		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.CorpseRecoveryReport
			{
				LegalAuthorityId = authority.Id,
				EconomicZoneId = economicZone.Id,
				CorpseId = corpse.Id,
				SourceCellId = sourceCell.Id,
				DestinationCellId = economicZone.MorgueStorageCell.Id,
				ReporterId = reporter?.Id,
				Status = (int)Status
			};
			FMDB.Context.CorpseRecoveryReports.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "CorpseRecoveryReport";
	public ILegalAuthority LegalAuthority => _gameworld.LegalAuthorities.Get(_legalAuthorityId);
	public IEconomicZone EconomicZone => _gameworld.EconomicZones.Get(_economicZoneId);
	public IGameItem Corpse => _gameworld.Items.Get(_corpseId);
	public ICell SourceCell => _gameworld.Cells.Get(_sourceCellId);
	public ICell DestinationCell => _gameworld.Cells.Get(_destinationCellId);
	public ICharacter Reporter => _reporterId.HasValue ? _gameworld.TryGetCharacter(_reporterId.Value, true) : null;
	public CorpseRecoveryReportStatus Status { get; set; }
	public long? AssignedPatrolId => _assignedPatrolId;
	public IPatrol AssignedPatrol => _assignedPatrolId.HasValue
		? LegalAuthority.Patrols.FirstOrDefault(x => x.Id == _assignedPatrolId.Value)
		: null;

	public void AssignPatrol(IPatrol patrol)
	{
		_assignedPatrolId = patrol?.Id;
		Status = patrol == null ? CorpseRecoveryReportStatus.Pending : CorpseRecoveryReportStatus.Assigned;
		Changed = true;
	}

	public void MarkCompleted()
	{
		_assignedPatrolId = null;
		Status = CorpseRecoveryReportStatus.Completed;
		Changed = true;
	}

	public void MarkFailed()
	{
		_assignedPatrolId = null;
		Status = CorpseRecoveryReportStatus.Failed;
		Changed = true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.CorpseRecoveryReports.Find(Id);
		dbitem.Status = (int)Status;
		dbitem.AssignedPatrolId = _assignedPatrolId;
		Changed = false;
	}
}
