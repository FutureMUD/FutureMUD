using MudSharp.Body.Traits;
using MudSharp.Magic.SpellEffects;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed record ScrollEffectSupport(string Type, string Status, string Detail);

/// <summary>Explicit opt-in inventory. Registration alone never grants snapshot compatibility.</summary>
public static class ScrollSpellCompatibility
{
	private static readonly HashSet<string> Numeric = new(StringComparer.OrdinalIgnoreCase)
	{ "damage", "heal", "mend", "selfdamage", "staminadelta", "magicresourcedelta", "spellarmour" };
	private static readonly HashSet<string> ActorOnly = new(StringComparer.OrdinalIgnoreCase)
	{ "teleport", "teleporttarget", "forcedexitmovement" };
	private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
	{
		"boost", "glow", "invisibility", "blindness", "removeblindness", "cureblindness", "deafness", "healingrate",
		"needdelta", "needrate", "pacifism", "rage", "staminaexpendrate", "staminaregenrate", "weight", "roomlight",
		"comprehendlanguage", "curse", "detectethereal", "detectinvisible", "detectmagick", "fear", "flying", "infravision",
		"insomnia", "paralysis", "removecomprehendlanguage", "removecurse", "removedetectethereal", "removedetectinvisible",
		"removedetectmagick", "removefear", "removeflying", "removeinfravision", "removeinsomnia", "removeparalysis",
		"removesilence", "removesleep", "removewaterbreathing", "silence", "sleep", "waterbreathing",
		"removeinvisibility", "dispelinvisibility", "removepoison", "removedisease", "magictag", "removemagictag"
	};
	private static readonly IReadOnlyDictionary<string, string> Unsupported = BuildUnsupported();
	private static IReadOnlyDictionary<string, string> BuildUnsupported()
	{
		var entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		void Add(string types, string reason) { foreach (var type in types.Split(' ', StringSplitOptions.RemoveEmptyEntries)) entries.Add(type, reason); }
		Add("executeprog forcecommand reciteproxy", "Arbitrary script/command effects have no typed numerical snapshot adapter.");
		Add("changecharacteristic", "Characteristic profile/value selection and its live referenced definitions require a dedicated compatibility adapter.");
		Add("astralprojection bodybackup createclone createcopy animatecorpse possesscorpse seizebody possessbody resurrect transformform", "Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter.");
		Add("createitem createliquid createnpc corpsespawn", "Creation/template callbacks and referenced prototype lifecycle require a dedicated adapter.");
		Add("createtrap placetrap dispeltrap removetrap", "Nested prepared-payload and trap ownership require a dedicated adapter.");
		Add("exitbarrier magicaltether portal portalnetwork", "Retained topology/attachment lifecycle requires a dedicated snapshot adapter.");
		Add("deadspeak identify detectpoison telepathy phantomillusion subjectivedesc subjectivesdesc", "Information or subjective-perception policy scripts require an explicit live-context adapter.");
		Add("dispelmagic personalward roomward personaltagward roomtagward", "Active interception/dispelling policies require a dedicated compatibility adapter.");
		Add("corpseconsume corpsemark corpsepreserve destroyitem itemdamage itemenchant", "Item/corpse mutation and nested item configuration require a dedicated compatibility adapter.");
		Add("burning ignite trackmark tracktrail", "Periodic damage/tracking expressions require a dedicated retained-context adapter.");
		Add("planarstate planeshift removeplanarstate", "Planar transition and linked instance lifecycle require a dedicated adapter.");
		Add("relocate", "This legacy implementation is a healing variant with specialised wound logic; use the audited heal or mend adapter.");
		Add("roomatmosphere removeroomflag roomflag roomgravity roomtemperature weatherchange weatherchangefreeze weatherfreeze", "Environmental controller and referenced-world configuration require a dedicated adapter.");
		Add("disease poison", "Pathogen/drug payloads contain additional numerical and live-reference fields without a snapshot adapter.");
		Add("featherfall forcedpathmovement handsofwind levitate transference", "Sustained movement, path or inventory-control ownership requires a dedicated adapter.");
		return entries;
	}
	public static IReadOnlyList<ScrollEffectSupport> Inventory => Numeric.Select(x => new ScrollEffectSupport(x, "Supported", "Every trait expression is bound per field/context; dynamic opposed outcome and authored randomness remain live."))
		.Concat(Supported.Select(x => new ScrollEffectSupport(x, "Supported", "Scalar/template XML and duration are frozen; target applicability and reader ownership remain live.")))
		.Concat(ActorOnly.Select(x => new ScrollEffectSupport(x, "Actor-context-only supported", "The actual reader supplies movement/party ownership and live trigger room/exit parameters.")))
		.Concat(Unsupported.Select(x => new ScrollEffectSupport(x.Key, "Unsupported", x.Value))).OrderBy(x => x.Type).ToArray();
	public static IEnumerable<(string Field, ITraitExpression Expression, Action<ITraitExpression> Set)> Expressions(IMagicSpellEffectTemplate effect)
	{
		switch (effect)
		{
			case DamageEffect x: yield return ("DamageExpression", x.DamageExpression, e => x.DamageExpression = e); break;
			case SelfDamageEffect x: yield return ("DamageExpression", x.DamageExpression, e => x.DamageExpression = e); break;
			case HealEffect x: yield return ("HealingAmount", x.HealingAmount, e => x.HealingAmount = e); break;
			case MendEffect x: yield return ("HealingAmount", x.HealingAmount, e => x.HealingAmount = e); break;
			case StaminaDeltaSpellEffect x: yield return ("Formula", x.AmountExpression, e => x.AmountExpression = e); break;
			case MagicResourceDeltaEffect x: yield return ("Formula", x.DeltaExpression, e => x.DeltaExpression = e); break;
			case SpellArmourEffect x: yield return ("MaximumDamageAbsorbed", x.ArmourConfiguration.MaximumDamageAbsorbed, e => x.ArmourConfiguration.MaximumDamageAbsorbed = e); break;
		}
	}
	public static IReadOnlyList<string> Errors(IMagicSpell spell, bool requireOptIn = true)
	{
		var errors = new List<string>();
		if (requireOptIn && !spell.ScrollInscriptionAllowed) errors.Add("The spell has not opted in to scroll inscription (or has been revoked).");
		if (spell.Trigger is not ICastMagicTrigger) errors.Add("Only ordinary cast triggers can be stored.");
		if (!spell.ReadyForGame) errors.Add("The stored spell is missing required trigger, duration, casting trait or presentation configuration.");
		if (spell is MagicSpell { EffectDurationExpression: { } durationExpression } && (durationExpression is not TraitExpression || durationExpression.HasErrors())) errors.Add("duration: invalid or unsupported numerical expression.");
		foreach (var (effect, label) in spell.SpellEffects.Select((x, i) => (x, $"target[{i}]"))
			.Concat(spell.CasterSpellEffects.Select((x, i) => (x, $"caster[{i}]"))))
		{
			var type = (string?)effect.SaveToXml().Attribute("type") ?? "unknown";
			if (!Supported.Contains(type) && !Numeric.Contains(type) && !ActorOnly.Contains(type)) errors.Add($"{label} {type}: {Unsupported.GetValueOrDefault(type, "Unregistered compatibility adapter; new types default to unsupported.")}");
			foreach (var (field, expression, _) in Expressions(effect))
				if (expression is not TraitExpression || expression.HasErrors()) errors.Add($"{label}/{field}: unsupported or invalid expression.");
			if (effect is TraitBoostEffect { Trait: null }) errors.Add($"{label}: missing boost trait.");
			if (effect is MagicResourceDeltaEffect { Resource: null }) errors.Add($"{label}: missing magic resource.");
			if (effect is DamageEffect { BodypartId: > 0 } damage && spell.Gameworld.BodypartPrototypes.Get(damage.BodypartId) is null) errors.Add($"{label}: missing damage bodypart.");
			if (effect is SpellArmourEffect armour && (armour.ArmourConfiguration.ArmourType is null || armour.ArmourConfiguration.ArmourMaterial is null)) errors.Add($"{label}: missing armour type/material.");
			// Policy references are live predicates, never creator numerical capture or an executed production effect.
			foreach (var node in effect.SaveToXml().Descendants().Where(x => x.Name.LocalName.EndsWith("Prog", StringComparison.OrdinalIgnoreCase)))
				if (long.TryParse(node.Value, out var id) && id != 0 && spell.Gameworld.FutureProgs.Get(id) is null) errors.Add($"{label}: missing policy reference {node.Name} #{id}.");
		}
		return errors.AsReadOnly();
	}
	/// <summary>Validate retained IDs before legacy loaders can substitute null or discard an absent entry.</summary>
	internal static void ValidateReferences(XElement definition, IFuturemud gameworld)
	{
		foreach (var element in definition.Descendants())
		{
			if (!long.TryParse(element.Value, out var id) || id <= 0) continue;
			var name = element.Name.LocalName;
			var missing = name.EndsWith("Prog", StringComparison.OrdinalIgnoreCase)
				? gameworld.FutureProgs.Get(id) is null
				: name switch
				{
					"Bodypart" => gameworld.BodypartPrototypes.Get(id) is null,
					"Shape" when element.Parent?.Name == "BodypartShapes" => gameworld.BodypartShapes.Get(id) is null,
					"Resource" => gameworld.MagicResources.Get(id) is null,
					"Drug" => gameworld.Drugs.Get(id) is null,
					"ArmourType" => gameworld.ArmourTypes.Get(id) is null,
					"ArmourMaterial" => gameworld.Materials.Get(id) is null,
					_ => false
				};
			if (missing) throw new InvalidOperationException($"Stored spell has a missing {name} reference #{id}.");
		}
	}
	public static void Bind(MagicSpell spell, SpellNumericalContext context, ICharacter? creator)
	{
		if (spell.EffectDurationExpression is { } duration)
		{
			if (creator is not null) context.Capture("duration", duration, creator, spell.CastingTrait, TraitBonusContext.SpellDuration);
			else context.ValidateBinding("duration", duration, spell.CastingTrait, TraitBonusContext.SpellDuration);
			spell.EffectDurationExpression = new ContextualSpellExpression(duration, context, "duration", spell.Gameworld);
		}
		foreach (var (effect, label) in spell.SpellEffects.Select((x, i) => (x, $"target[{i}]"))
			.Concat(spell.CasterSpellEffects.Select((x, i) => (x, $"caster[{i}]"))))
			foreach (var (field, expression, set) in Expressions(effect))
			{
				var location = $"{label}/{field}";
				if (creator is not null) context.Capture(location, expression, creator);
				else context.ValidateBinding(location, expression);
				set(new ContextualSpellExpression(expression, context, location, spell.Gameworld));
			}
	}
}
