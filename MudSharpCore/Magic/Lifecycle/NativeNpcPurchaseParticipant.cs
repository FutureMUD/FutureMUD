#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.NPC;
using MudSharp.NPC.Templates;
using MudSharp.Framework.Revision;
using NativeNpc = MudSharp.NPC.NPC;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Stages a native NPC privately so its committed sale can publish only prepared state.</summary>
internal sealed class NativeNpcPurchaseParticipant(ICharacter buyer, ICharacter seller, INPCTemplate template) : ICurrencyTransferParticipant
{
	private readonly SpatialLocation _location = new(buyer.Location, buyer.RoomLayer, buyer.RoutePositionMetres);
	private Futuremud? _world;
	private Room? _room;
	private NativeNpc? _npc;
	private object? _inserted;
	private long _characterId;
	private long _bodyId;
	private bool _committed;
	private bool _prepared;
	private bool _disposed;
	private bool AdmissionCurrent => !_disposed && !ReferenceEquals(buyer, seller) && buyer.State.IsAble() &&
		seller.State.IsAwake() && seller.Combat is null && buyer.ColocatedWith(seller) &&
		ReferenceEquals(seller.Gameworld, buyer.Gameworld) && template.Status == RevisionStatus.Current;

	internal ICharacter? DeliveredNpc => _committed ? _npc : null;
	public bool IsCurrent => _prepared && AdmissionCurrent && _npc is not null && ReferenceEquals(buyer.Location, _location.Room) &&
		buyer.RoomLayer == _location.Layer && buyer.RoutePositionMetres == _location.RoutePositionMetres &&
		ReferenceEquals(_npc.Location, _location.Room) && _npc.RoomLayer == _location.Layer &&
		_npc.RoutePositionMetres == _location.RoutePositionMetres;

	public bool Prepare()
	{
		if (_prepared) return IsCurrent;
		if (!AdmissionCurrent || buyer.Gameworld is not Futuremud world || _location.Room is not Room room ||
			!ReferenceEquals(template.Gameworld, buyer.Gameworld) || !RouteSpatialService.Instance.TryValidateLocation(_location, out _) ||
			NativeNpcCreationEligibility.TemplateError(template, buyer.Gameworld) is not null) return false;
		_world = world;
		_room = room;
		var characterTemplate = template.GetCharacterTemplate(_location.Room);
		if (NativeNpcCreationEligibility.CharacterTemplateError(characterTemplate) is not null) return false;
		_npc = new NativeNpc(buyer.Gameworld, characterTemplate, template, deferInitialisation: true);
		_npc.SetPreparedNativeNpcLocation(_location);
		_npc.PrepareCommittedNativeNpc(characterTemplate);
		using (var authorization = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			_inserted = _npc.DatabaseInsert();
			var model = (MudSharp.Models.Character)((Tuple<object, MudSharp.Models.Npc>)_inserted).Item1;
			model.State = (int)CharacterState.Stasis;
			foreach (var instance in model.CharacterInstances.Where(x => x.IsPrimary)) instance.State = (int)CharacterState.Stasis;
			FMDB.Context.SaveChanges();
			_characterId = model.Id;
			_bodyId = model.BodyId ?? throw new InvalidOperationException("The prepared native NPC has no owned body row.");
			transaction.Commit();
		}
		_npc.BindPreparedNativeNpcIdentity(_inserted);
		_prepared = true;
		return IsCurrent;
	}

	public void Commit()
	{
		if (!IsCurrent) throw new InvalidOperationException("The prepared NPC purchase candidate is no longer current.");
		_world!.AddPreparedNativeNpc(_npc!);
		_room!.SetNativeCharacterMembership(_npc!, true);
		_committed = true;
	}

	public void Notify()
	{
		if (!_committed || _npc is null || _inserted is null) return;
		_npc.ActivatePreparedCommittedNativeNpc();
		_world!.Add(_npc, true);
		_npc.Location.Login(_npc);
		_npc.CommitPreparedNativeNpcIdentity();
		template.ApplyTemplateLoadAdditions(_npc);
		template.OnLoadProg?.Execute(_npc);
	}

	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		if (_committed) return;
		_npc?.ReleaseUnpublishedNativeNpc();
		if (_npc is not null)
		{
			RouteSpatialService.Instance.UntrackPerceivable(_npc);
			EnvironmentalExposureService.ForgetCommittedProjectionBody(buyer.Gameworld, _npc.Body);
		}
		if (_characterId == 0) return;
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		var model = FMDB.Context.Characters.SingleOrDefault(x => x.Id == _characterId);
		if (model is not null)
		{
			FMDB.Context.Characters.Remove(model);
			FMDB.Context.SaveChanges();
		}
		var body = FMDB.Context.Bodies.SingleOrDefault(x => x.Id == _bodyId);
		if (body is not null)
		{
			FMDB.Context.Bodies.Remove(body);
			FMDB.Context.SaveChanges();
		}
		transaction.Commit();
	}
}
