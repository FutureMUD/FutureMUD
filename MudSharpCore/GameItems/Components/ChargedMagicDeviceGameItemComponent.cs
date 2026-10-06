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
	public override IGameItemComponentProto Prototype => _prototype;
	public int Capacity => _prototype.Capacity;
	public MagicDeviceRole Role => _prototype.Role;
	public int Charges => _charges.Count;
	public long? SpellId => Snapshot?.SpellId;
	public int? Grade => Snapshot?.Numbers.Grade;
	public string? DataError { get; private set; }
	internal StoredSpellSnapshot? Snapshot { get; private set; }
	internal Guid? Reservation { get; private set; }
	internal long ProducerCapability { get; private set; }
	internal long ProducerTrait { get; private set; }
	internal double RequiredRaw { get; private set; }
	internal Guid? NextCharge => _charges.Count == 0 ? null : _charges[0];
	public ChargedMagicDeviceGameItemComponent(ChargedMagicDeviceGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{ _prototype = proto; PersistedDefinition = Export().ToString(); _persistedPrototypeId = proto.Id; _persistedPrototypeRevision = proto.RevisionNumber; }
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
			Snapshot = root.Element("StoredSpell") is { } snapshot ? StoredSpellSnapshot.Load(snapshot) : null;
			ProducerCapability = (long?)root.Attribute("capability") ?? 0;
			ProducerTrait = (long?)root.Attribute("trait") ?? 0;
			RequiredRaw = (double?)root.Attribute("raw") ?? 0;
			_charges.AddRange(root.Elements("Charge").Select(x => Guid.Parse(x.Value)));
			if (_charges.Count > 100 || _charges.Contains(Guid.Empty) || _charges.Distinct().Count() != Charges ||
				Reservation == Guid.Empty || !double.IsFinite(RequiredRaw) || RequiredRaw < 0 ||
				Snapshot is not null && (Grade is null || ProducerCapability <= 0 || ProducerTrait <= 0) || Snapshot is null && Charges != 0)
				throw new FormatException("Invalid device charge bank or provenance.");
		}
		catch (Exception ex) { DataError = ex.Message; _invalidXml = model.Definition; }
	}
	internal bool Reserve(Guid token)
	{
		if (DataError is not null || Reservation is not null || token == Guid.Empty) return false;
		Reservation = token; ClearPendingSave(); return true;
	}
	internal void Release(Guid token) { if (Reservation == token) Reservation = null; ClearPendingSave(); }
	internal void Fill(Guid token, StoredSpellSnapshot payload, int count, long capability, long trait, double raw)
	{
		if (Reservation != token || count <= 0 || count > Capacity - Charges || DataError is not null ||
			Charges != 0 && Snapshot?.PotencyFingerprint != payload.PotencyFingerprint) throw new InvalidOperationException("The reserved homogeneous charge bank changed.");
		if (Charges == 0) { Snapshot = payload; ProducerCapability = capability; ProducerTrait = trait; RequiredRaw = raw; }
		for (var i = 0; i < count; i++) _charges.Add(Guid.NewGuid());
		Reservation = null; ClearPendingSave();
	}
	internal void Consume(Guid token)
	{
		if (Reservation != token || NextCharge != token || DataError is not null) throw new InvalidOperationException("The reserved charge changed.");
		_charges.RemoveAt(0); Reservation = null; ClearPendingSave();
	}
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ChargedMagicDeviceGameItemComponent(_prototype, newParent, temporary);
	internal void AcceptPersistedDefinition(string definition) { PersistedDefinition = definition; Changed = false; }
	internal void ClearPendingSave() { Gameworld.SaveManager.Abort(this); Changed = false; }
	// Every existing-row definition write goes through this comparison, including the ordinary
	// save manager and prototype revision dispatch. Never attach a stale full row to EF.
	internal void PersistBank(ChargedMagicDeviceGameItemComponentProto? newPrototype = null)
	{
		ClearPendingSave();
		if (FMDB.WritesAreSuppressed || _noSave) throw new InvalidOperationException("Device persistence requires a writable persisted component.");
		if (DataError is not null) throw new InvalidOperationException(DataError);
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
		try { PersistBank(next); return true; }
		catch (InvalidOperationException) when (DataError is not null) { return false; }
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
			new XAttribute("trait", ProducerTrait), new XAttribute("raw", RequiredRaw), Reservation is { } token ? new XAttribute("reservation", token) : null,
			Snapshot?.Save(), _charges.Select(x => new XElement("Charge", x)));
		root.Add(new XAttribute("checksum", Checksum(root))); return root;
	}
	public override bool PreventsMerging(IGameItemComponent component) => true;
	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full;
	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags) =>
		$"{description}\n\n{_prototype.Kind}: {Role}, {Charges}/{Capacity} charges. Use magicdevice show for details.";
}
