#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>A selected-grade item wound operation with an exact pre-payment item and custody choice.</summary>
public sealed class BoundedItemConditionEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation,
	IMagicSpellEffectPreparedSelection
{
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record Selection(ICharacter Caster, IGameItem Item, int Grade, double DamageInput, string Definition,
		Func<bool> Check, Func<bool> StructureCheck)
		: IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
		public bool StructureCurrent => StructureCheck();
	}
	private sealed class ItemDamageGuard(Selection selection) : IDamageMutationGuard
	{
		public void Validate(IHaveWounds recipient, IDamage damage)
		{
			if (!ReferenceEquals(recipient, selection.Item) || !ReferenceEquals(damage.ActorOrigin, selection.Caster) ||
				!ReferenceEquals(damage.MutationGuard, this) || !selection.IsCurrent)
				throw new InvalidOperationException("The exact item or its custody changed before native damage mutation.");
		}
	}

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("boundeditemcondition", (xml, spell) => new BoundedItemConditionEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("boundeditemcondition", (_, spell) =>
			(new BoundedItemConditionEffect(new XElement("Effect", new XAttribute("type", "boundeditemcondition"),
				new XElement("Mode", "damage"), new XElement("BudgetPerGrade", 5),
				new XElement("DamageType", (int)DamageType.Crushing), new XElement("MaterialBehaviour", (int)MaterialBehaviourType.Stone),
				new XElement("OrganicOnly", false)), spell), string.Empty),
			"Bounded selected-grade damage or repair of a local item", HelpText, true, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	public BoundedItemConditionEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		Mode = (string?)xml.Element("Mode") ?? throw new FormatException("Missing item condition mode.");
		BudgetPerGrade = (double)(xml.Element("BudgetPerGrade") ?? throw new FormatException("Missing item condition budget."));
		DamageType = (DamageType)(int)(xml.Element("DamageType") ?? throw new FormatException("Missing item damage type."));
		MaterialBehaviour = (int)(xml.Element("MaterialBehaviour") ?? throw new FormatException("Missing material gate."));
		OrganicOnly = (bool)(xml.Element("OrganicOnly") ?? throw new FormatException("Missing organic gate."));
		Validate();
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Mode { get; private set; }
	public double BudgetPerGrade { get; private set; }
	public DamageType DamageType { get; private set; }
	public int MaterialBehaviour { get; private set; }
	public bool OrganicOnly { get; private set; }
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public const string HelpText = "Options: mode damage|repair, budget <finite amount per external grade>, type <native damage type>, material <behaviour|-1 for any>, organic true|false. Damage requires a material gate; repair changes existing repairable wounds only.";
	private void Validate()
	{
		if (Mode is not ("damage" or "repair") || !double.IsFinite(BudgetPerGrade) || BudgetPerGrade is <= 0 or > 1000000 ||
			!Enum.IsDefined(DamageType) || MaterialBehaviour != -1 && !Enum.IsDefined(typeof(MaterialBehaviourType), MaterialBehaviour) ||
			Mode == "damage" && MaterialBehaviour == -1 && !OrganicOnly)
			throw new FormatException("Choose a finite positive item budget, defined native damage type and an explicit damage material gate.");
	}
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "boundeditemcondition"),
		new XElement("Mode", Mode), new XElement("BudgetPerGrade", BudgetPerGrade),
		new XElement("DamageType", (int)DamageType), new XElement("MaterialBehaviour", MaterialBehaviour),
		new XElement("OrganicOnly", OrganicOnly));
	public IMagicSpellEffectTemplate Clone() => new BoundedItemConditionEffect(SaveToXml(), Spell);
	private static bool IsCompatibleWithTrigger(string types) => types is "item" or "items";
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => IsCompatibleWithTrigger(trigger.TargetTypes);
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Bounded Item Condition",
		("Mode", Mode.ColourName()), ("Budget per grade", BudgetPerGrade.ToString("N2", actor).ColourValue()),
		("Damage type", DamageType.DescribeEnum().ColourName()),
		("Material behaviour", (MaterialBehaviour < 0 ? "any" : ((MaterialBehaviourType)MaterialBehaviour).DescribeEnum()).ColourName()),
		("Organic only", OrganicOnly.ToColouredString()));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant();
		var value = command.SafeRemainingArgument;
		var old = SaveToXml();
		if (field == "mode" && value is "damage" or "repair") Mode = value;
		else if (field == "budget" && double.TryParse(value, NumberStyles.Float, actor, out var budget)) BudgetPerGrade = budget;
		else if (field == "type" && value.TryParseEnum<DamageType>(out var damageType)) DamageType = damageType;
		else if (field == "material" && value == "-1") MaterialBehaviour = -1;
		else if (field == "material" && value.TryParseEnum<MaterialBehaviourType>(out var behaviour)) MaterialBehaviour = (int)behaviour;
		else if (field == "organic" && bool.TryParse(value, out var organic)) OrganicOnly = organic;
		else { actor.OutputHandler.Send(HelpText); return false; }
		try { Validate(); }
		catch (FormatException error)
		{
			var prior = new BoundedItemConditionEffect(old, Spell);
			Mode = prior.Mode; BudgetPerGrade = prior.BudgetPerGrade; DamageType = prior.DamageType;
			MaterialBehaviour = prior.MaterialBehaviour; OrganicOnly = prior.OrganicOnly;
			actor.OutputHandler.Send(error.Message.ColourError()); return false;
		}
		Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
	}

	private bool Eligible(ICharacter caster, IGameItem item, int grade)
	{
		if (grade is < 1 or > 7 || item.Deleted || item.Destroyed || !ReferenceEquals(caster.Gameworld, Gameworld) ||
			!ReferenceEquals(item.Gameworld, Gameworld) || !ReferenceEquals(Gameworld.Items.Get(item.Id), item) ||
			caster.Body is not { } body || !ReferenceEquals(body.Actor, caster) || caster.Location is not { } room ||
			!item.TrueLocations.Contains(room) || item.GetItemType<IDestroyable>() is not { MaximumDamage: > 0 } destroyable)
			return false;
		if (Mode == "repair")
			return item.Wounds.Any(x => x.Repairable && x.CurrentDamage > 0 && ReferenceEquals(x.Parent, item));
		if (MaterialBehaviour >= 0 && (int)item.Material.BehaviourType != MaterialBehaviour || OrganicOnly && !item.Material.Organic ||
			item.ShallowItems.Any(x => !ReferenceEquals(x, item))) return false;
		var existing = item.Wounds.Sum(x => x.CurrentDamage);
		var budget = NativeDamageInput(caster, destroyable, grade);
		return double.IsFinite(existing) && existing >= 0 && double.IsFinite(destroyable.MaximumDamage) &&
			budget is { Expected: > 0 } && existing + budget.Value.Expected < destroyable.MaximumDamage;
	}
	private (double Input, double Expected)? NativeDamageInput(ICharacter caster, IDestroyable destroyable, int grade)
	{
		var limit = BudgetPerGrade * grade;
		var preview = destroyable.GetActualDamage(new Damage { ActorOrigin = caster, DamageType = DamageType,
			DamageAmount = limit, PainAmount = 0, ShockAmount = 0, StunAmount = 0,
			PenetrationOutcome = Outcome.NotTested });
		if (preview is null || !double.IsFinite(preview.DamageAmount) || preview.DamageAmount <= 0) return null;
		var input = Math.Min(limit, limit * limit / preview.DamageAmount);
		if (!double.IsFinite(input) || input <= 0) return null;
		var actual = destroyable.GetActualDamage(new Damage { ActorOrigin = caster, DamageType = DamageType,
			DamageAmount = input, PainAmount = 0, ShockAmount = 0, StunAmount = 0,
			PenetrationOutcome = Outcome.NotTested });
		if (actual is null || !double.IsFinite(actual.DamageAmount) || actual.DamageAmount <= 0 ||
			actual.DamageAmount > limit + 1e-7) return null;
		return (input, actual.DamageAmount);
	}

	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return prior;
		}
		if (recipient is not IGameItem item || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native ||
			!Eligible(caster, item, native.InvocationGrade.Value))
			throw new InvalidOperationException("The exact local item is ineligible for bounded damage or repair.");
		var body = caster.Body; var room = caster.Location; var layer = caster.RoomLayer;
		var casterId = caster.Id; var casterInstance = caster.InstanceId; var casterIdentity = caster.Identity;
		var itemId = item.Id;
		var prototype = item.Prototype; var material = item.Material; var custodian = item.InInventoryOf;
		var container = item.ContainedIn; var location = item.Location; var itemLayer = item.RoomLayer;
		var components = item.Components.ToArray(); var quality = item.Quality; var rawQuality = item.RawQuality;
		var condition = item.Condition; var wounds = item.Wounds.Select(x => (Wound: x, Damage: x.CurrentDamage)).ToArray();
		var grade = native.InvocationGrade.Value; var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var damageInput = Mode == "damage" ? NativeDamageInput(caster, item.GetItemType<IDestroyable>(), grade)!.Value.Input : 0;
		bool Stable() => ReferenceEquals(caster.Body, body) && ReferenceEquals(body?.Actor, caster) &&
			caster.Id == casterId && caster.InstanceId == casterInstance && ReferenceEquals(caster.Identity, casterIdentity) &&
			ReferenceEquals(caster.Location, room) && caster.RoomLayer == layer && item.Id == itemId &&
			ReferenceEquals(caster.Gameworld, Gameworld) && ReferenceEquals(item.Gameworld, Gameworld) &&
			ReferenceEquals(item.Prototype, prototype) && ReferenceEquals(item.Material, material) &&
			ReferenceEquals(item.InInventoryOf, custodian) && ReferenceEquals(item.ContainedIn, container) &&
			ReferenceEquals(item.Location, location) && item.RoomLayer == itemLayer &&
			item.Components.SequenceEqual(components, ReferenceEqualityComparer.Instance) &&
			item.Quality == quality && item.RawQuality == rawQuality && item.Condition == condition &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition &&
			!item.Deleted && !item.Destroyed && ReferenceEquals(Gameworld.Items.Get(item.Id), item) &&
			item.TrueLocations.Contains(room);
		bool Current() => Stable() && item.Wounds.Select(x => (Wound: x, Damage: x.CurrentDamage)).SequenceEqual(wounds) &&
			Eligible(caster, item, grade) &&
			(Mode != "damage" || NativeDamageInput(caster, item.GetItemType<IDestroyable>(), grade)?.Input == damageInput);
		var token = new Selection(caster, item, grade, damageInput, definition, Current, Stable);
		if (!token.IsCurrent) throw new InvalidOperationException("The item changed during condition selection.");
		_selections.Add(recipient, token); return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The prepared item, custody, material or condition changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Item, recipient) ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The exact item condition selection was not prepared.";
		return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		string? error = null;
		if (target is null || !TryConfirmPreparedSelection(caster, target, out error))
			throw new InvalidOperationException(error ?? "The item condition selection changed before application.");
		var token = _selections[target]; var item = token.Item; var budget = BudgetPerGrade * token.Grade;
		if (Mode == "repair")
		{
			var repaired = 0.0;
			foreach (var wound in item.Wounds.Where(x => x.Repairable && x.CurrentDamage > 0).ToArray())
			{
				if (!CommandExecutionScope.TryContinue() || !token.StructureCurrent || !ReferenceEquals(wound.Parent, item))
					throw new InvalidOperationException("The selected item changed before native wound repair.");
				var amount = Math.Min(budget - repaired, wound.CurrentDamage);
				if (amount <= 0) break;
				wound.CurrentDamage -= amount; repaired += amount;
				if (!token.StructureCurrent)
					throw new InvalidOperationException("The selected item changed during native wound repair.");
			}
			if (repaired > 0) item.EvaluateWounds();
			if (!token.StructureCurrent)
				throw new InvalidOperationException("The selected item changed during native wound evaluation.");
			return new(repaired > 0 ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
		}
		var before = item.Wounds.Sum(x => x.CurrentDamage);
		item.SufferDamage(new Damage { ActorOrigin = caster, DamageType = DamageType, DamageAmount = token.DamageInput,
			PainAmount = 0, ShockAmount = 0, StunAmount = 0, PenetrationOutcome = Outcome.NotTested,
			MutationGuard = new ItemDamageGuard(token) });
		if (!token.StructureCurrent)
			throw new InvalidOperationException("The selected item changed during native condition damage.");
		var after = item.Wounds.Sum(x => x.CurrentDamage);
		if (!double.IsFinite(after) || after - before > budget + 1e-7)
			throw new InvalidOperationException("Native item damage exceeded the selected finite condition budget.");
		return new(after > before ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		Apply(caster, target, outcome, power, parent, additionalParameters).Effect;
}
