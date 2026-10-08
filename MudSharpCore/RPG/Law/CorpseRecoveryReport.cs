using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.RPG.Law;

public class CorpseRecoveryReport : SaveableItem, ICorpseRecoveryReport
{
    private readonly IFuturemud _gameworld;
    private readonly long _legalAuthorityId;
    private readonly long _economicZoneId;
    private readonly long _corpseId;
    private readonly long _sourceRoomId;
    private readonly long _destinationRoomId;
    private readonly long? _reporterId;
    private long? _assignedPatrolId;

    public CorpseRecoveryReport(MudSharp.Models.CorpseRecoveryReport dbitem, IFuturemud gameworld)
    {
        _gameworld = gameworld;
        _id = dbitem.Id;
        _legalAuthorityId = dbitem.LegalAuthorityId;
        _economicZoneId = dbitem.EconomicZoneId;
        _corpseId = dbitem.CorpseId;
        _sourceRoomId = dbitem.SourceRoomId;
        _destinationRoomId = dbitem.DestinationRoomId;
        _reporterId = dbitem.ReporterId;
        _assignedPatrolId = dbitem.AssignedPatrolId;
        Status = (CorpseRecoveryReportStatus)dbitem.Status;
    }

    public CorpseRecoveryReport(ILegalAuthority authority, IEconomicZone economicZone, IGameItem corpse, IRoom sourceRoom,
        ICharacter reporter)
    {
        _gameworld = authority.Gameworld;
        _legalAuthorityId = authority.Id;
        _economicZoneId = economicZone.Id;
        _corpseId = corpse.Id;
        _sourceRoomId = sourceRoom.Id;
        _destinationRoomId = economicZone.MorgueStorageRoom.Id;
        _reporterId = reporter is null ? null : CharacterInstanceIdentityComparer.IdentityId(reporter);
        Status = CorpseRecoveryReportStatus.Pending;

        using (new FMDB())
        {
            Models.CorpseRecoveryReport dbitem = new()
            {
                LegalAuthorityId = authority.Id,
                EconomicZoneId = economicZone.Id,
                CorpseId = corpse.Id,
                SourceRoomId = sourceRoom.Id,
                DestinationRoomId = economicZone.MorgueStorageRoom.Id,
                ReporterId = reporter is null ? null : CharacterInstanceIdentityComparer.IdentityId(reporter),
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
    public IRoom SourceRoom => _gameworld.Rooms.Get(_sourceRoomId);
    public IRoom DestinationRoom => _gameworld.Rooms.Get(_destinationRoomId);
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
        Models.CorpseRecoveryReport dbitem = FMDB.Context.CorpseRecoveryReports.Find(Id);
        dbitem.Status = (int)Status;
        dbitem.AssignedPatrolId = _assignedPatrolId;
        Changed = false;
    }
}
