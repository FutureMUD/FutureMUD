using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Vancian;
using System.Security.Cryptography;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using Microsoft.EntityFrameworkCore;

#nullable enable
namespace MudSharp.GameItems.Components;

public sealed class ChargedMagicDeviceGameItemComponent : GameItemComponent, IChargedMagicDevice
{
	private ChargedMagicDeviceGameItemComponentProto _prototype;
	private string? _invalidXml;
	internal string PersistedDefinition { get; private set; }
	private long _persistedPrototypeId;
	private int _persistedPrototypeRevision;
	private readonly List<Guid> _charges = [];
	private Guid? _reservoirReservation;
	private int _energy;
	public override IGameItemComponentProto Prototype => _prototype;
	public int Capacity => _prototype.Capacity;
	public MagicDeviceRole Role => _prototype.Role;
	public MagicDeviceMode Mode => _prototype.Mode;
	public int Energy => _energy;
	public int MaximumEnergy => _prototype.MaximumEnergy;
	public long? ResourceId => _prototype.Mode == MagicDeviceMode.EnergyReservoir ? _prototype.ReservoirResourceId : null;
	public long? ReservoirSpellId => _prototype.Mode == MagicDeviceMode.EnergyReservoir ? _prototype.ReservoirSpellId : null;
	public MagicDeviceDelivery Delivery => _prototype.Delivery;
	public IReadOnlyDictionary<int, int> GradeCosts => _prototype.GradeCosts;
	public bool AllowsWornUse => _prototype.AllowsWornUse;
	public bool RequiresOpenSky => _prototype.RequiresOpenSky;
	public int Charges => _charges.Count;
	public long? SpellId => Mode == MagicDeviceMode.EnergyReservoir ? ReservoirSpellId : Snapshot?.SpellId;
	public int? Grade => Snapshot?.Numbers.Grade;
	public string? DataError { get; private set; }
	internal StoredSpellSnapshot? Snapshot { get; private set; }
	internal Guid? Reservation { get; private set; }
	internal Guid? ReservoirReservation => _reservoirReservation;
	internal long ProducerCapability { get; private set; }
	internal long ProducerTrait { get; private set; }
	internal double RequiredRaw { get; private set; }
	internal Guid? NextCharge => _charges.Count == 0 ? null : _charges[0];
	public ChargedMagicDeviceGameItemComponent(ChargedMagicDeviceGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{ _prototype = proto; _energy = proto.Mode == MagicDeviceMode.EnergyReservoir ? proto.InitialEnergy : 0; PersistedDefinition = Export().ToString(); _persistedPrototypeId = proto.Id; _persistedPrototypeRevision = proto.RevisionNumber; }
	public ChargedMagicDeviceGameItemComponent(Models.GameItemComponent model, ChargedMagicDeviceGameItemComponentProto proto, IGameItem parent) : base(model, parent)
	{
		_prototype = proto;
		PersistedDefinition = model.Definition;
		_persistedPrototypeId = model.GameItemComponentProtoId;
		_persistedPrototypeRevision = model.GameItemComponentProtoRevision;
		try
		{
			if (model.Definition.Length > 8_000_000) throw new FormatException("Oversized device payload.");
			var root = XElement.Parse(model.Definition);
			if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported device payload.");
			var checksum = (string?)root.Attribute("checksum"); root.Attribute("checksum")?.Remove();
			if (checksum != Checksum(root)) throw new FormatException("Device charge-bank checksum mismatch.");
			Reservation = (Guid?)root.Attribute("reservation");
			_reservoirReservation = (Guid?)root.Attribute("reservoirReservation");
			_energy = (int?)root.Attribute("energy") ?? (proto.Mode == MagicDeviceMode.EnergyReservoir ? proto.InitialEnergy : 0);
			Snapshot = root.Element("StoredSpell") is { } snapshot ? StoredSpellSnapshot.Load(snapshot) : null;
			ProducerCapability = (long?)root.Attribute("capability") ?? 0;
			ProducerTrait = (long?)root.Attribute("trait") ?? 0;
			RequiredRaw = (double?)root.Attribute("raw") ?? 0;
			_charges.AddRange(root.Elements("Charge").Select(x => Guid.Parse(x.Value)));
			if (_charges.Count > 100 || _charges.Contains(Guid.Empty) || _charges.Distinct().Count() != Charges ||
				Reservation == Guid.Empty || _reservoirReservation == Guid.Empty || Reservation is not null && _reservoirReservation is not null || !double.IsFinite(RequiredRaw) || RequiredRaw < 0 ||
				Snapshot is not null && (Grade is null || ProducerCapability <= 0 || ProducerTrait <= 0) || Snapshot is null && Charges != 0)
				throw new FormatException("Invalid device charge bank or provenance.");
			if (proto.Mode == MagicDeviceMode.EnergyReservoir && (_energy < 0 || _energy > proto.MaximumEnergy || Snapshot is not null || Charges != 0 || Reservation is not null) ||
				proto.Mode == MagicDeviceMode.StoredCharges && _energy != 0 || _reservoirReservation is not null && proto.Mode != MagicDeviceMode.EnergyReservoir)
				throw new FormatException("Invalid device energy reservoir state.");
		}
		catch (Exception ex) { DataError = ex.Message; _invalidXml = model.Definition; }
	}
	internal bool Reserve(Guid token)
	{
		if (DataError is not null || Mode != MagicDeviceMode.StoredCharges || Reservation is not null || token == Guid.Empty) return false;
		Reservation = token; ClearPendingSave(); return true;
	}
	internal void Release(Guid token) { if (Reservation == token) Reservation = null; ClearPendingSave(); }
	internal void Fill(Guid token, StoredSpellSnapshot payload, int count, long capability, long trait, double raw)
	{
		if (Mode != MagicDeviceMode.StoredCharges || Reservation != token || count <= 0 || count > Capacity - Charges || DataError is not null ||
			Charges != 0 && Snapshot?.PotencyFingerprint != payload.PotencyFingerprint) throw new InvalidOperationException("The reserved homogeneous charge bank changed.");
		if (Charges == 0) { Snapshot = payload; ProducerCapability = capability; ProducerTrait = trait; RequiredRaw = raw; }
		for (var i = 0; i < count; i++) _charges.Add(Guid.NewGuid());
		Reservation = null; ClearPendingSave();
	}
	internal void Consume(Guid token)
	{
		if (Mode != MagicDeviceMode.StoredCharges || Reservation != token || NextCharge != token || DataError is not null) throw new InvalidOperationException("The reserved charge changed.");
		_charges.RemoveAt(0); Reservation = null; ClearPendingSave();
	}
	internal bool ReserveReservoir(Guid token)
	{
		if (DataError is not null || Mode != MagicDeviceMode.EnergyReservoir || Reservation is not null || _reservoirReservation is not null || token == Guid.Empty) return false;
		_reservoirReservation = token;
		ClearPendingSave();
		return true;
	}
	internal void SpendReservoir(Guid token, int cost)
	{
		if (_reservoirReservation != token || cost <= 0 || cost > Energy || DataError is not null)
			throw new InvalidOperationException("The reserved reservoir energy changed or is insufficient.");
		_energy -= cost;
		_reservoirReservation = null;
		ClearPendingSave();
	}
	internal void ReleaseReservoir(Guid token)
	{
		if (_reservoirReservation == token) _reservoirReservation = null;
		ClearPendingSave();
	}
	internal int AddEnergy(int amount)
	{
		if (DataError is not null || Mode != MagicDeviceMode.EnergyReservoir || _reservoirReservation is not null || amount <= 0)
			throw new InvalidOperationException("Reservoir recharge is unavailable.");
		if (amount > MaximumEnergy - Energy) throw new InvalidOperationException("The reservoir does not have enough capacity for that donation.");
		_energy += amount;
		ClearPendingSave();
		return amount;
	}
	internal int AddEnergy(Guid token, int amount)
	{
		if (DataError is not null || Mode != MagicDeviceMode.EnergyReservoir || _reservoirReservation != token ||
			token == Guid.Empty || amount <= 0 || amount > MaximumEnergy - Energy)
			throw new InvalidOperationException("The reserved reservoir donation is invalid.");
		_energy += amount;
		_reservoirReservation = null;
		ClearPendingSave();
		return amount;
	}
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		var copy = new ChargedMagicDeviceGameItemComponent(_prototype, newParent, temporary);
		if (Mode == MagicDeviceMode.EnergyReservoir)
		{
			copy._energy = 0;
			copy.PersistedDefinition = copy.Export().ToString();
		}
		return copy;
	}
	internal void AcceptPersistedDefinition(string definition) { PersistedDefinition = definition; Changed = false; }
	internal void ClearPendingSave() { Gameworld.SaveManager.Abort(this); Changed = false; }
	// Every existing-row definition write goes through this comparison, including the ordinary
	// save manager and prototype revision dispatch. Never attach a stale full row to EF.
	internal void PersistBank(ChargedMagicDeviceGameItemComponentProto? newPrototype = null)
	{
		ClearPendingSave();
		if (FMDB.WritesAreSuppressed || _noSave) throw new InvalidOperationException("Device persistence requires a writable persisted component.");
		if (DataError is not null) throw new InvalidOperationException(DataError);
		if (newPrototype is not null && PrototypeUpdateError(newPrototype) is { } compatibilityError)
			throw new InvalidOperationException(compatibilityError);
		var definition = SaveToXml();
		var revision = newPrototype?.RevisionNumber ?? _persistedPrototypeRevision;
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		if (Id <= 0 || FMDB.Context.Database.ExecuteSqlInterpolated($"UPDATE GameItemComponents SET Definition={definition}, GameItemComponentProtoRevision={revision} WHERE Id={Id} AND GameItemComponentProtoId={_persistedPrototypeId} AND GameItemComponentProtoRevision={_persistedPrototypeRevision} AND BINARY Definition=BINARY {PersistedDefinition}") != 1)
		{
			DataError = "The durable device bank or prototype changed concurrently; reload and staff review are required.";
			throw new InvalidOperationException(DataError);
		}
		AcceptPersistedDefinition(definition);
		_persistedPrototypeRevision = revision;
		if (newPrototype is not null) _prototype = newPrototype;
	}
	public override void Save()
	{
		ForeignCustodyTransferContext.RecordSave(this);
		if (_noSave || !Changed || DataError is not null) { ClearPendingSave(); return; }
		try { PersistBank(); }
		catch (InvalidOperationException) when (DataError is not null) { /* Preserve the winner and disable this stale host. */ }
	}
	public override bool CheckPrototypeForUpdate()
	{
		if (_noSave || DataError is not null || Prototype.Status is not (RevisionStatus.Obsolete or RevisionStatus.Revised)) return false;
		if (Gameworld.ItemComponentProtos.FirstOrDefault(x => x.Id == Prototype.Id && x.Status == RevisionStatus.Current) is not ChargedMagicDeviceGameItemComponentProto next) return false;
		if (PrototypeUpdateError(next) is not null) return false;
		try { PersistBank(next); return true; }
		catch (InvalidOperationException) when (DataError is not null) { return false; }
	}
	internal string? PrototypeUpdateError(ChargedMagicDeviceGameItemComponentProto next)
	{
		if (next.Mode != Mode)
		{
			var hasStoredCharges = Charges > 0 || Snapshot is not null || Reservation is not null;
			var hasStoredEnergy = Energy > 0 || _reservoirReservation is not null;
			if (hasStoredCharges || hasStoredEnergy)
				return "The revised device mode would reinterpret a nonempty or reserved instance; empty its stored state before revising the prototype.";
		}
		if (next.Mode == MagicDeviceMode.EnergyReservoir && next.MaximumEnergy < Energy)
			return "The revised reservoir maximum is below this instance's persisted energy.";
		if (next.Mode == MagicDeviceMode.StoredCharges && next.Capacity < Charges)
			return "The revised charge capacity is below this instance's stored charges.";
		return null;
	}
	public override void SetIDFromDatabase(object dbitem)
	{
		base.SetIDFromDatabase(dbitem);
		var row = (Models.GameItemComponent)dbitem;
		PersistedDefinition = row.Definition; _persistedPrototypeId = row.GameItemComponentProtoId; _persistedPrototypeRevision = row.GameItemComponentProtoRevision;
	}
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto proto) => _prototype = (ChargedMagicDeviceGameItemComponentProto)proto;
	protected override string SaveToXml() => _invalidXml ?? Export().ToString();
	private static string Checksum(XElement root) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(root.ToString(SaveOptions.DisableFormatting))));
	internal XElement Export()
	{
		var root = new XElement("ChargedMagicDevice", new XAttribute("version", 1), new XAttribute("capability", ProducerCapability),
			new XAttribute("trait", ProducerTrait), new XAttribute("raw", RequiredRaw), new XAttribute("energy", Energy),
			Reservation is { } token ? new XAttribute("reservation", token) : null,
			_reservoirReservation is { } reservoirToken ? new XAttribute("reservoirReservation", reservoirToken) : null,
			Snapshot?.Save(), _charges.Select(x => new XElement("Charge", x)));
		root.Add(new XAttribute("checksum", Checksum(root))); return root;
	}
	public override bool PreventsMerging(IGameItemComponent component) => true;
	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full;
	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags) =>
		$"{description}\n\n{_prototype.Kind}: {Role}, {Mode switch { MagicDeviceMode.EnergyReservoir => $"{Energy}/{MaximumEnergy} energy", _ => $"{Charges}/{Capacity} charges" }}. Use magicdevice show for details.";
}
