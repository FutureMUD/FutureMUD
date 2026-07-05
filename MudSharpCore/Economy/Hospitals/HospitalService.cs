using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Health;
using DbHospitalService = MudSharp.Models.HospitalService;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public class HospitalService : SavableKeywordedItem, IHospitalService
{
	private sealed record EquipmentRequirementPayload(string Kind, long? Id, string? Text, int Quantity);

	private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	private string _description;
	private HospitalServiceType _serviceType;
	private decimal _price;
	private bool _isActive;
	private bool _allowDebt;
	private bool _preferOperatingTheatre;
	private int _sortOrder;
	private long? _surgicalProcedureId;
	private ISurgicalProcedure? _surgicalProcedure;
	private long? _implantItemPrototypeId;
	private int? _implantItemPrototypeRevisionNumber;
	private IGameItemProto? _implantItemPrototype;
	private long? _implantPowerProcedureId;
	private ISurgicalProcedure? _implantPowerProcedure;
	private long? _implantInterfaceProcedureId;
	private ISurgicalProcedure? _implantInterfaceProcedure;
	private long? _anesthesiaCannulationProcedureId;
	private ISurgicalProcedure? _anesthesiaCannulationProcedure;
	private long? _anesthesiaDrugId;
	private IDrug? _anesthesiaDrug;
	private string _procedureParameters;
	private readonly List<HospitalServiceEquipmentRequirement> _requiredEquipment = new();
	private double _bloodVolumeLitres;
	private bool _requiresRecovery;
	private double _anesthesiaIntensity;

	public HospitalService(IHospital hospital, string name, HospitalServiceType serviceType, decimal price)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		_name = name;
		SetKeywordsFromSDesc(name);
		_description = string.Empty;
		_serviceType = serviceType;
		_price = Math.Max(0.0M, price);
		_isActive = true;
		_allowDebt = true;
		_preferOperatingTheatre = DefaultPreferOperatingTheatre(serviceType);
		_sortOrder = hospital.Services.Any() ? hospital.Services.Max(x => x.SortOrder) + 1 : 0;
		_procedureParameters = string.Empty;
		_bloodVolumeLitres = 0.5;
		_requiresRecovery = DefaultRequiresRecovery(serviceType);
		_anesthesiaIntensity = 1.25;

		using (new FMDB())
		{
			var dbitem = new DbHospitalService
			{
				HospitalId = hospital.Id,
				Name = name,
				Keywords = string.Join(" ", Keywords),
				Description = string.Empty,
				ServiceType = (int)serviceType,
				Price = _price,
				IsActive = true,
				AllowDebt = true,
				PreferOperatingTheatre = _preferOperatingTheatre,
				SortOrder = _sortOrder,
				ProcedureParameters = string.Empty,
				RequiredEquipmentJson = "[]",
				BloodVolumeLitres = _bloodVolumeLitres,
				RequiresRecovery = _requiresRecovery,
				AnesthesiaIntensity = _anesthesiaIntensity
			};
			FMDB.Context.HospitalServices.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public HospitalService(DbHospitalService service, IHospital hospital)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		_id = service.Id;
		_name = service.Name;
		_keywords = new Lazy<List<string>>(() => service.Keywords.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList());
		_description = service.Description;
		_serviceType = (HospitalServiceType)service.ServiceType;
		_price = service.Price;
		_isActive = service.IsActive;
		_allowDebt = service.AllowDebt;
		_preferOperatingTheatre = service.PreferOperatingTheatre;
		_sortOrder = service.SortOrder;
		_surgicalProcedureId = service.SurgicalProcedureId;
		_implantItemPrototypeId = service.ImplantItemPrototypeId;
		_implantItemPrototypeRevisionNumber = service.ImplantItemPrototypeRevisionNumber;
		_implantPowerProcedureId = service.ImplantPowerProcedureId;
		_implantInterfaceProcedureId = service.ImplantInterfaceProcedureId;
		_anesthesiaCannulationProcedureId = service.AnesthesiaCannulationProcedureId;
		_anesthesiaDrugId = service.AnesthesiaDrugId;
		_procedureParameters = service.ProcedureParameters;
		_requiredEquipment.AddRange(DeserializeEquipment(service.RequiredEquipmentJson));
		_bloodVolumeLitres = service.BloodVolumeLitres > 0.0 ? service.BloodVolumeLitres : 0.5;
		_requiresRecovery = service.RequiresRecovery;
		_anesthesiaIntensity = service.AnesthesiaIntensity > 0.0 ? service.AnesthesiaIntensity : 1.25;
	}

	public override string FrameworkItemType => "HospitalService";
	public IHospital Hospital { get; }

	public void Rename(string name)
	{
		_name = name.TitleCase();
		SetKeywordsFromSDesc(_name);
		Changed = true;
	}

	public HospitalServiceType ServiceType
	{
		get => _serviceType;
		set
		{
			_serviceType = value;
			Changed = true;
		}
	}

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			Changed = true;
		}
	}

	public decimal Price
	{
		get => _price;
		set
		{
			_price = Math.Max(0.0M, value);
			Changed = true;
		}
	}

	public bool IsActive
	{
		get => _isActive;
		set
		{
			_isActive = value;
			Changed = true;
		}
	}

	public bool AllowDebt
	{
		get => _allowDebt;
		set
		{
			_allowDebt = value;
			Changed = true;
		}
	}

	public bool PreferOperatingTheatre
	{
		get => _preferOperatingTheatre;
		set
		{
			_preferOperatingTheatre = value;
			Changed = true;
		}
	}

	public int SortOrder
	{
		get => _sortOrder;
		set
		{
			_sortOrder = value;
			Changed = true;
		}
	}

	public ISurgicalProcedure? SurgicalProcedure
	{
		get
		{
			if (_surgicalProcedure is null && _surgicalProcedureId is not null)
			{
				_surgicalProcedure = Gameworld.SurgicalProcedures.Get(_surgicalProcedureId.Value);
			}

			return _surgicalProcedure;
		}
		set
		{
			_surgicalProcedure = value;
			_surgicalProcedureId = value?.Id;
			Changed = true;
		}
	}

	public IGameItemProto? ImplantItemPrototype
	{
		get
		{
			if (_implantItemPrototype is null && _implantItemPrototypeId is not null)
			{
				_implantItemPrototype = Gameworld.ItemProtos.Get(_implantItemPrototypeId.Value, _implantItemPrototypeRevisionNumber ?? 0);
			}

			return _implantItemPrototype;
		}
		set
		{
			_implantItemPrototype = value;
			_implantItemPrototypeId = value?.Id;
			_implantItemPrototypeRevisionNumber = value?.RevisionNumber;
			Changed = true;
		}
	}
	public ISurgicalProcedure? ImplantPowerProcedure
	{
		get
		{
			if (_implantPowerProcedure is null && _implantPowerProcedureId is not null)
			{
				_implantPowerProcedure = Gameworld.SurgicalProcedures.Get(_implantPowerProcedureId.Value);
			}

			return _implantPowerProcedure;
		}
		set
		{
			_implantPowerProcedure = value;
			_implantPowerProcedureId = value?.Id;
			Changed = true;
		}
	}

	public ISurgicalProcedure? ImplantInterfaceProcedure
	{
		get
		{
			if (_implantInterfaceProcedure is null && _implantInterfaceProcedureId is not null)
			{
				_implantInterfaceProcedure = Gameworld.SurgicalProcedures.Get(_implantInterfaceProcedureId.Value);
			}

			return _implantInterfaceProcedure;
		}
		set
		{
			_implantInterfaceProcedure = value;
			_implantInterfaceProcedureId = value?.Id;
			Changed = true;
		}
	}

	public ISurgicalProcedure? AnesthesiaCannulationProcedure
	{
		get
		{
			if (_anesthesiaCannulationProcedure is null && _anesthesiaCannulationProcedureId is not null)
			{
				_anesthesiaCannulationProcedure = Gameworld.SurgicalProcedures.Get(_anesthesiaCannulationProcedureId.Value);
			}

			return _anesthesiaCannulationProcedure;
		}
		set
		{
			_anesthesiaCannulationProcedure = value;
			_anesthesiaCannulationProcedureId = value?.Id;
			Changed = true;
		}
	}

	public IDrug? AnesthesiaDrug
	{
		get
		{
			if (_anesthesiaDrug is null && _anesthesiaDrugId is not null)
			{
				_anesthesiaDrug = Gameworld.Drugs.Get(_anesthesiaDrugId.Value);
			}

			return _anesthesiaDrug;
		}
		set
		{
			_anesthesiaDrug = value;
			_anesthesiaDrugId = value?.Id;
			Changed = true;
		}
	}

	public double AnesthesiaIntensity
	{
		get => _anesthesiaIntensity;
		set
		{
			_anesthesiaIntensity = Math.Max(0.0, value);
			Changed = true;
		}
	}

	public string ProcedureParameters
	{
		get => _procedureParameters;
		set
		{
			_procedureParameters = value;
			Changed = true;
		}
	}

	public IReadOnlyList<HospitalServiceEquipmentRequirement> RequiredEquipment => _requiredEquipment;

	public double BloodVolumeLitres
	{
		get => _bloodVolumeLitres;
		set
		{
			_bloodVolumeLitres = Math.Max(0.0, value);
			Changed = true;
		}
	}

	public bool RequiresRecovery
	{
		get => _requiresRecovery;
		set
		{
			_requiresRecovery = value;
			Changed = true;
		}
	}

	public void AddRequiredEquipment(HospitalServiceEquipmentRequirement requirement)
	{
		_requiredEquipment.Add(requirement with { Quantity = Math.Max(1, requirement.Quantity) });
		Changed = true;
	}

	public void RemoveRequiredEquipmentAt(int index)
	{
		if (index < 0 || index >= _requiredEquipment.Count)
		{
			return;
		}

		_requiredEquipment.RemoveAt(index);
		Changed = true;
	}

	public void ClearRequiredEquipment()
	{
		if (!_requiredEquipment.Any())
		{
			return;
		}

		_requiredEquipment.Clear();
		Changed = true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.HospitalServices.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Name = Name;
		dbitem.Keywords = string.Join(" ", Keywords);
		dbitem.Description = Description;
		dbitem.ServiceType = (int)ServiceType;
		dbitem.Price = Price;
		dbitem.IsActive = IsActive;
		dbitem.AllowDebt = AllowDebt;
		dbitem.PreferOperatingTheatre = PreferOperatingTheatre;
		dbitem.SortOrder = SortOrder;
		dbitem.SurgicalProcedureId = SurgicalProcedure?.Id;
		dbitem.ImplantItemPrototypeId = ImplantItemPrototype?.Id;
		dbitem.ImplantItemPrototypeRevisionNumber = ImplantItemPrototype?.RevisionNumber;
		dbitem.ImplantPowerProcedureId = ImplantPowerProcedure?.Id;
		dbitem.ImplantInterfaceProcedureId = ImplantInterfaceProcedure?.Id;
		dbitem.AnesthesiaCannulationProcedureId = AnesthesiaCannulationProcedure?.Id;
		dbitem.AnesthesiaDrugId = AnesthesiaDrug?.Id;
		dbitem.ProcedureParameters = ProcedureParameters;
		dbitem.RequiredEquipmentJson = SerializeEquipment(_requiredEquipment);
		dbitem.BloodVolumeLitres = BloodVolumeLitres;
		dbitem.RequiresRecovery = RequiresRecovery;
		dbitem.AnesthesiaIntensity = AnesthesiaIntensity;
		Changed = false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Hospital Service #{Id.ToString("N0", actor)} - {Name.TitleCase().ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Hospital: {Hospital.Name.ColourName()}");
		sb.AppendLine($"Type: {ServiceType.DescribeEnum().ColourName()}");
		sb.AppendLine($"Price: {HospitalServiceBilling.DescribePrice(Hospital, this, actor).ColourValue()}");
		sb.AppendLine($"Active: {IsActive.ToColouredString()}");
		var availability = HospitalServiceAvailability.Evaluate(Hospital, this, actor);
		sb.AppendLine($"Current Availability: {availability.DescribeColoured()}");
		sb.AppendLine($"Allow Debt: {AllowDebt.ToColouredString()}");
		sb.AppendLine($"Prefer Theatre: {PreferOperatingTheatre.ToColouredString()}");
		sb.AppendLine($"Requires Recovery: {RequiresRecovery.ToColouredString()}");
		sb.AppendLine($"Blood Volume: {BloodVolumeLitres.ToString("N2", actor).ColourValue()}L");
		sb.AppendLine($"Sort Order: {SortOrder.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Surgical Procedure: {(SurgicalProcedure is null ? "None".ColourError() : $"#{SurgicalProcedure.Id.ToString("N0", actor)} {SurgicalProcedure.Name}".ColourName())}");
		sb.AppendLine($"Implant Prototype: {(ImplantItemPrototype is null ? "None".ColourError() : $"#{ImplantItemPrototype.Id.ToString("N0", actor)}r{ImplantItemPrototype.RevisionNumber.ToString("N0", actor)} {ImplantItemPrototype.Name}".ColourName())}");
		sb.AppendLine($"Implant Power Procedure: {(ImplantPowerProcedure is null ? "None".ColourError() : $"#{ImplantPowerProcedure.Id.ToString("N0", actor)} {ImplantPowerProcedure.Name}".ColourName())}");
		sb.AppendLine($"Implant Interface Procedure: {(ImplantInterfaceProcedure is null ? "None".ColourError() : $"#{ImplantInterfaceProcedure.Id.ToString("N0", actor)} {ImplantInterfaceProcedure.Name}".ColourName())}");
		sb.AppendLine($"Anesthesia Cannulation: {(AnesthesiaCannulationProcedure is null ? "None".ColourError() : $"#{AnesthesiaCannulationProcedure.Id.ToString("N0", actor)} {AnesthesiaCannulationProcedure.Name}".ColourName())}");
		sb.AppendLine($"Anesthesia Drug: {(AnesthesiaDrug is null ? "None".ColourError() : $"#{AnesthesiaDrug.Id.ToString("N0", actor)} {AnesthesiaDrug.Name}".ColourName())}");
		sb.AppendLine($"Anesthesia Intensity: {AnesthesiaIntensity.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Parameters: {(string.IsNullOrWhiteSpace(ProcedureParameters) ? "None".ColourError() : ProcedureParameters.ColourCommand())}");
		sb.AppendLine("Required Equipment:");
		if (_requiredEquipment.Any())
		{
			for (var i = 0; i < _requiredEquipment.Count; i++)
			{
				var requirement = _requiredEquipment[i];
				sb.AppendLine($"\t{(i + 1).ToString("N0", actor).ColourValue()}. {requirement.Quantity.ToString("N0", actor).ColourValue()}x {EmploymentItemSelectorResolver.Describe(requirement.Selector).ColourCommand()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone.".ColourError());
		}

		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine(string.IsNullOrWhiteSpace(Description) ? "\tNone.".ColourError() : Description.Wrap(actor.InnerLineFormatLength));
		return sb.ToString();
	}

	public void Delete()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.HospitalServices.Find(Id);
			if (dbitem is not null)
			{
				FMDB.Context.HospitalServices.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	private static bool DefaultPreferOperatingTheatre(HospitalServiceType serviceType)
	{
		return serviceType is HospitalServiceType.SurgicalProcedure or HospitalServiceType.ImplantProcedure or
			HospitalServiceType.BoneSetting or HospitalServiceType.BloodDonation or HospitalServiceType.BloodTransfusion or
			HospitalServiceType.Stabilisation or HospitalServiceType.FullTreatment;
	}

	private static bool DefaultRequiresRecovery(HospitalServiceType serviceType)
	{
		return serviceType is HospitalServiceType.SurgicalProcedure or HospitalServiceType.ImplantProcedure or
			HospitalServiceType.BoneSetting or HospitalServiceType.BloodDonation or HospitalServiceType.BloodTransfusion or
			HospitalServiceType.Stabilisation or HospitalServiceType.FullTreatment;
	}

	private static string SerializeEquipment(IEnumerable<HospitalServiceEquipmentRequirement> requirements)
	{
		var payload = requirements.Select(x => new EquipmentRequirementPayload(
			x.Selector.Kind.ToString(),
			x.Selector.Id,
			x.Selector.Text,
			Math.Max(1, x.Quantity)));
		return JsonSerializer.Serialize(payload, JsonOptions);
	}

	private static IEnumerable<HospitalServiceEquipmentRequirement> DeserializeEquipment(string? json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return [];
		}

		try
		{
			return JsonSerializer.Deserialize<List<EquipmentRequirementPayload>>(json, JsonOptions)?
			                     .SelectNotNull(ToRequirement)
			                     .ToList() ?? [];
		}
		catch (JsonException)
		{
			return [];
		}
	}

	private static HospitalServiceEquipmentRequirement? ToRequirement(EquipmentRequirementPayload? payload)
	{
		if (payload is null || !payload.Kind.TryParseEnum<EmploymentItemSelectorKind>(out var kind))
		{
			return null;
		}

		var selector = kind switch
		{
			EmploymentItemSelectorKind.PrototypeId when payload.Id.HasValue => EmploymentItemSelector.ForPrototype(payload.Id.Value),
			EmploymentItemSelectorKind.ItemId when payload.Id.HasValue => EmploymentItemSelector.ForItemId(payload.Id.Value),
			EmploymentItemSelectorKind.Tag when !string.IsNullOrWhiteSpace(payload.Text) => EmploymentItemSelector.ForTag(payload.Text),
			EmploymentItemSelectorKind.Keyword when !string.IsNullOrWhiteSpace(payload.Text) => EmploymentItemSelector.ForKeyword(payload.Text),
			_ => null
		};
		return selector is null ? null : new HospitalServiceEquipmentRequirement(Math.Max(1, payload.Quantity), selector);
	}
}
