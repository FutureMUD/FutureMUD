using System.Globalization;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed class VancianLoadout
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public string Name { get; set; } = "";
	public List<VancianAssignment> Assignments { get; } = [];
}

public sealed class VancianSlot
{
	public Guid AllowanceKey { get; init; }
	public int AllowanceVersion { get; init; }
	public int Ordinal { get; init; }
	public int Level { get; init; }
	public VancianSlotStatus Status { get; set; }
	public VancianAssignment? Preparation { get; init; }
	public Guid? Reservation { get; set; }
	public VancianSlotStatus? PreviousStatus { get; set; }
}

/// <summary>Detached aggregate. Only the service/store may publish a mutated copy.</summary>
public sealed class VancianCapabilityState
{
	public long OwnerId { get; init; }
	public long CapabilityId { get; init; }
	public long Version { get; set; }
	public long Generation { get; set; }
	public Dictionary<Guid, List<long>> Selections { get; } = [];
	public List<VancianLoadout> Loadouts { get; } = [];
	public Guid? SelectedLoadout { get; set; }
	public List<VancianAssignment>? LastPattern { get; set; }
	public List<VancianSlot> Slots { get; } = [];
	public DateTime? LastRefreshUtc { get; set; }
	public bool SleepQualified { get; set; }
	public string? DataError { get; private set; }
	private string? _invalidXml;
	internal string OriginalDefinition => _invalidXml ?? Save().ToString();
	public VancianCapabilityState Copy() => Load(OwnerId, CapabilityId, Version, _invalidXml ?? Save().ToString(SaveOptions.DisableFormatting));

	public XElement Save()
	{
		if (DataError is not null) throw new InvalidOperationException($"State is disabled: {DataError}. Preserve the original XML for repair: {_invalidXml?.Length ?? 0} characters.");
		return new XElement("VancianState", new XAttribute("schema", 1), new XAttribute("generation", Generation),
			new XAttribute("sleepQualified", SleepQualified), SelectedLoadout is { } selected ? new XAttribute("selected", selected) : null,
			LastRefreshUtc is { } refreshed ? new XAttribute("refreshed", refreshed.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)) : null,
			new XElement("Selections", Selections.Select(x => new XElement("Rule", new XAttribute("key", x.Key), x.Value.Select(id => new XElement("Spell", id))))),
			new XElement("Loadouts", Loadouts.Select(x => new XElement("Loadout", new XAttribute("id", x.Id), new XAttribute("name", x.Name), x.Assignments.Select(AssignmentXml)))),
			LastPattern is not null ? new XElement("LastPattern", LastPattern.Select(AssignmentXml)) : null,
			new XElement("Slots", Slots.Select(x => new XElement("Slot", new XAttribute("allowance", x.AllowanceKey),
				new XAttribute("version", x.AllowanceVersion), new XAttribute("ordinal", x.Ordinal), new XAttribute("level", x.Level),
				new XAttribute("status", x.Status), x.Reservation is { } token ? new XAttribute("reservation", token) : null,
				x.PreviousStatus.HasValue ? new XAttribute("previous", x.PreviousStatus.Value) : null,
				x.Preparation is not null ? AssignmentXml(x.Preparation) : null))));
	}

	internal static XElement AssignmentXml(VancianAssignment a) => new("Assignment", new XAttribute("allowance", a.AllowanceKey),
		new XAttribute("version", a.AllowanceVersion), new XAttribute("ordinal", a.Ordinal), new XAttribute("rule", a.RepertoireKey),
		new XAttribute("spell", a.SpellId), new XAttribute("level", a.SlotLevel), new XAttribute("base", a.SpellLevel), new XAttribute("power", (int)a.Power));

	internal static VancianAssignment ReadAssignment(XElement x)
	{
		var assignment = new VancianAssignment(Guid.Parse((string)x.Attribute("allowance")!), (int)x.Attribute("version")!,
			(int)x.Attribute("ordinal")!, Guid.Parse((string)x.Attribute("rule")!), (long)x.Attribute("spell")!,
			(int)x.Attribute("level")!, (int)x.Attribute("base")!, (SpellPower)(int)x.Attribute("power")!);
		if (assignment.AllowanceKey == Guid.Empty || assignment.RepertoireKey == Guid.Empty || assignment.Ordinal < 1 || assignment.AllowanceVersion < 1 || assignment.SpellId <= 0 || assignment.SpellLevel < 0 ||
			assignment.SlotLevel < assignment.SpellLevel || !Enum.IsDefined(assignment.Power)) throw new FormatException("Invalid persisted assignment.");
		return assignment;
	}

	public static VancianCapabilityState Load(long ownerId, long capabilityId, long version, string? xml)
	{
		var state = new VancianCapabilityState { OwnerId = ownerId, CapabilityId = capabilityId, Version = version };
		if (xml is null) return state;
		try
		{
			if (xml.Length > 8_000_000) throw new FormatException("State payload exceeds the 8 MB limit.");
			var root = XElement.Parse(xml);
			if ((int?)root.Attribute("schema") != 1) throw new FormatException("Unsupported Vancian state schema.");
			state.Generation = (long)root.Attribute("generation")!;
			state.SleepQualified = (bool)root.Attribute("sleepQualified")!;
			state.SelectedLoadout = (string?)root.Attribute("selected") is { } selected ? Guid.Parse(selected) : null;
			state.LastRefreshUtc = (string?)root.Attribute("refreshed") is { } refresh ? DateTime.Parse(refresh, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime() : null;
			foreach (var rule in root.Element("Selections")!.Elements("Rule"))
			{
				var entries = rule.Elements("Spell").Select(x => (long)x).ToList();
				if (entries.Count != entries.Distinct().Count() || entries.Any(x => x <= 0)) throw new FormatException("Duplicate/invalid selected spell IDs.");
				state.Selections.Add(Guid.Parse((string)rule.Attribute("key")!), entries);
			}
			foreach (var plan in root.Element("Loadouts")!.Elements("Loadout"))
			{
				var loadout = new VancianLoadout { Id = Guid.Parse((string)plan.Attribute("id")!), Name = (string)plan.Attribute("name")! };
				loadout.Assignments.AddRange(plan.Elements("Assignment").Select(ReadAssignment));
				state.Loadouts.Add(loadout);
			}
			state.LastPattern = root.Element("LastPattern")?.Elements("Assignment").Select(ReadAssignment).ToList();
			foreach (var slot in root.Element("Slots")!.Elements("Slot"))
			{
				var entry = new VancianSlot { AllowanceKey = Guid.Parse((string)slot.Attribute("allowance")!),
					AllowanceVersion = (int)slot.Attribute("version")!, Ordinal = (int)slot.Attribute("ordinal")!, Level = (int)slot.Attribute("level")!,
					Status = Enum.Parse<VancianSlotStatus>((string)slot.Attribute("status")!),
					Preparation = slot.Element("Assignment") is { } assignment ? ReadAssignment(assignment) : null,
					Reservation = (string?)slot.Attribute("reservation") is { } reservation ? Guid.Parse(reservation) : null,
					PreviousStatus = (string?)slot.Attribute("previous") is { } previous ? Enum.Parse<VancianSlotStatus>(previous) : null };
				if (entry.Ordinal < 1 || entry.Level < 0 || entry.AllowanceVersion < 1 || !Enum.IsDefined(entry.Status) ||
					(entry.Status == VancianSlotStatus.Reserved && (entry.Reservation is null || entry.PreviousStatus is null))) throw new FormatException("Invalid slot ledger entry.");
				if (entry.AllowanceKey == Guid.Empty || entry.Status == VancianSlotStatus.Prepared && entry.Preparation is null ||
					entry.Preparation is { } prepared && (prepared.AllowanceKey != entry.AllowanceKey || prepared.AllowanceVersion != entry.AllowanceVersion || prepared.Ordinal != entry.Ordinal || prepared.SlotLevel != entry.Level) ||
					entry.Status == VancianSlotStatus.Reserved && (entry.Reservation == Guid.Empty || entry.PreviousStatus is not (VancianSlotStatus.Prepared or VancianSlotStatus.AvailableSpontaneous)))
					throw new FormatException("Inconsistent persisted casting or reservation.");
				state.Slots.Add(entry);
			}
			if (state.Generation < 0 || version < 0 || state.Slots.Count > 100_000 || state.Loadouts.Count > 1000 ||
				state.Slots.GroupBy(x => (x.AllowanceKey, x.Ordinal)).Any(x => x.Count() > 1) ||
				state.Loadouts.GroupBy(x => x.Id).Any(x => x.Count() > 1) ||
				state.Loadouts.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1)) throw new FormatException("Invalid or duplicate aggregate entries.");
		}
		catch (Exception ex) { state.DataError = ex.Message; state._invalidXml = xml; }
		return state;
	}
}
