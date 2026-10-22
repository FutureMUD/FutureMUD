using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

#nullable enable
namespace MudSharp.Magic;

public enum ConditionalMagicItemKind { SpellSelectionShrine, ConditionalBinding, ResourceRestorer, ItemIssuer }
public enum BindingActivationRange { SameRoom, SameZone, SameShard, World }

/// <summary>Native health units; a floor limits only health loss, not stun or stamina loss.</summary>
public sealed record ItemConsequence(double Health = 0, double Stun = 0, double Stamina = 0,
	double HealthFloor = 0, bool Collapse = false, bool DestroyItem = false, string Message = "",
	double HealthFraction = 0, double StunFraction = 0, double StaminaFraction = 0)
{
	public bool IsValid => new[] { Health, Stun, Stamina, HealthFloor }.All(x => double.IsFinite(x) && x >= 0) && new[] { HealthFraction, StunFraction, StaminaFraction }.All(x => double.IsFinite(x) && x is >= 0 and <= 1);
	public double HealthLoss(double remaining, double maximum = 0) => Math.Clamp(remaining - HealthFloor, 0, Health + maximum * HealthFraction);
}

public sealed record ShrineSpellChoice(long SpellId, string Category);

/// <summary>Revisioned, setting-independent content. Prog IDs are boolean(character), except the protected-target prog.</summary>
public sealed record ConditionalMagicItemPolicy
{
	public int Version { get; init; } = 1;
	public ConditionalMagicItemKind Kind { get; init; }
	public long EligibilityProgId { get; init; }
	public long ControllerProgId { get; init; }
	public long TerritoryProgId { get; init; }
	public long EscortProgId { get; init; }
	public long ProtectedTargetProgId { get; init; }
	public long IncompatibleWearerProgId { get; init; }
	public long RequiredRegaliaPrototypeId { get; init; }
	public string BindingGroup { get; init; } = "binding";
	public BindingActivationRange Range { get; init; } = BindingActivationRange.SameRoom;
	public string[] WearLocations { get; init; } = [];
	public bool PreventRemoval { get; init; } = true;
	public bool PunishOutsideTerritory { get; init; } = true;
	public double IntervalSeconds { get; init; } = 10;
	public double IntervalJitterSeconds { get; init; }
	public double ActivationChance { get; init; } = 1;
	public double ActionDelaySeconds { get; init; }
	public bool InterceptHostileSpells { get; init; }
	public long[] InterceptSpellIds { get; init; } = [];
	public string[] InterceptMoods { get; init; } = [];
	public ItemConsequence Activation { get; init; } = new();
	public ItemConsequence Removal { get; init; } = new();
	public ItemConsequence Periodic { get; init; } = new();
	public ItemConsequence IncompatibleWear { get; init; } = new();
	public long CapabilityId { get; init; }
	public int SelectionLimit { get; init; } = 3;
	public bool AllowReplacement { get; init; } = true;
	public ShrineSpellChoice[] Choices { get; init; } = [];
	public long ResourceId { get; init; }
	public int Capacity { get; init; } = 1;
	public bool RequireWorn { get; init; }
	public long[] IssuedPrototypeIds { get; init; } = [];
	public string ActivationMessage { get; init; } = "You invoke the item's power.";
	public string RechargeMessage { get; init; } = "You restore the item's power.";

	public IReadOnlyList<string> Errors()
	{
		var errors = new List<string>();
		if (Version != 1 || !Enum.IsDefined(Kind) || !Enum.IsDefined(Range)) errors.Add("Unsupported policy version or kind.");
		if (EligibilityProgId <= 0) errors.Add("An eligibility prog is required.");
		if (!double.IsFinite(IntervalSeconds) || IntervalSeconds < 1 || !double.IsFinite(IntervalJitterSeconds) || IntervalJitterSeconds < 0 || IntervalJitterSeconds >= IntervalSeconds) errors.Add("Invalid periodic interval/jitter.");
		if (!double.IsFinite(ActivationChance) || ActivationChance is < 0 or > 1 || !double.IsFinite(ActionDelaySeconds) || ActionDelaySeconds < 0) errors.Add("Invalid activation chance or delay.");
		if (!Activation.IsValid || !Removal.IsValid || !Periodic.IsValid || !IncompatibleWear.IsValid) errors.Add("Consequences require finite nonnegative amounts.");
		if (Kind == ConditionalMagicItemKind.ConditionalBinding && (string.IsNullOrWhiteSpace(BindingGroup) || ControllerProgId <= 0)) errors.Add("Bindings require a group and controller prog.");
		if (Kind == ConditionalMagicItemKind.SpellSelectionShrine && (CapabilityId <= 0 || SelectionLimit < 1 || Choices.Length == 0 || Choices.Any(x => x.SpellId <= 0 || string.IsNullOrWhiteSpace(x.Category)) || Choices.Select(x => x.SpellId).Distinct().Count() != Choices.Length)) errors.Add("Invalid shrine capability, allowance or spell choices.");
		if (Kind == ConditionalMagicItemKind.ResourceRestorer && (ResourceId <= 0 || Capacity is < 1 or > 10000 || TerritoryProgId <= 0)) errors.Add("Restorers require a resource, capacity and recharge territory.");
		if (Kind == ConditionalMagicItemKind.ItemIssuer && (IssuedPrototypeIds.Length == 0 || IssuedPrototypeIds.Any(x => x <= 0))) errors.Add("An issuer needs explicit item prototypes.");
		return errors;
	}

	public bool AcceptsSelection(IReadOnlyList<long> requested, IReadOnlyList<long> current, out string reason)
	{
		reason = "";
		if (requested.Count > SelectionLimit || requested.Distinct().Count() != requested.Count || requested.Any(x => !Choices.Any(y => y.SpellId == x))) reason = "Choose distinct spells from this shrine's list within its allowance.";
		else if (!AllowReplacement && current.Except(requested).Any()) reason = "This shrine does not permit replacing granted spells.";
		return reason.Length == 0;
	}

	public XElement Save() => new("Definition", new XElement("Policy", new XCData(JsonSerializer.Serialize(this))));
	public static ConditionalMagicItemPolicy Load(XElement root) => JsonSerializer.Deserialize<ConditionalMagicItemPolicy>(root.Element("Policy")?.Value ?? throw new FormatException("Missing conditional item policy.")) ?? throw new FormatException("Empty conditional item policy.");
}

/// <summary>Optional route-local patron rules. They never promote a character's independently acquired spell grades.</summary>
public sealed record PatronCastingPolicy(long AuthorityProgId, long TerritoryProgId, int FixedGrade,
	bool FreeWithinTerritory, bool RefillWithinTerritory, bool RequireShout, long[] PermanentSpells,
	int SelectionLimit = 3, IReadOnlyDictionary<long, ControlledSpellIncantation>? Incantations = null);
