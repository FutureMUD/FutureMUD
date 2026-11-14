#nullable enable

using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Instant two-stage attunement of a local item to detection and inventory retention on a plane.</summary>
public class EtherealItemAttunementEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation,
	IMagicSpellEffectPreparedSelection
{
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record Selection(ICharacter Caster, IPerceivable Target, int Grade, string Definition, Func<bool> Check)
		: IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
	}
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("etherealitemattunement", (xml, spell) => new EtherealItemAttunementEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("etherealitemattunement", (commands, spell) =>
		{
			var plane = commands.IsFinished ? null : spell.Gameworld.Planes.GetByIdOrName(commands.PopSpeech());
			var stage = commands.IsFinished ? "auto" : commands.PopSpeech().ToLowerInvariant();
			if (plane is null || stage is not ("auto" or "ready" or "reveal") || !commands.IsFinished)
				return (null, "Use effect new etherealitemattunement <plane> [auto|ready|reveal].");
			return (new EtherealItemAttunementEffect(spell, plane.Id, stage == "reveal", stage == "auto", 3.0, 1.0), string.Empty);
		}, "Permanently attunes a selected item to ethereal travel", HelpText, true, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	protected EtherealItemAttunementEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		PlaneId = (long?)xml.Element("PlaneId") ?? throw new FormatException("Missing attunement plane.");
		RevealStage = (bool?)xml.Element("RevealStage") ?? false;
		AutoAdvance = (bool?)xml.Element("AutoAdvance") ?? false;
		MassPerGrade = (double?)xml.Element("MassPerGrade") ?? 0.0;
		RiskPercentPerGrade = (double?)xml.Element("RiskPercentPerGrade") ?? 0.0;
		CharacterPlanarDefinition = xml.Element("CharacterPlanarData") is { } characterPlanarData
			? PlanarPresenceDefinition.FromXml(characterPlanarData.Element("PlanarData") ?? characterPlanarData, spell.Gameworld)
			: null;
		Validate();
	}

	private EtherealItemAttunementEffect(IMagicSpell spell, long planeId, bool revealStage, bool autoAdvance, double massPerGrade,
		double riskPercentPerGrade) : this(new XElement("Effect", new XElement("PlaneId", planeId),
		new XElement("RevealStage", revealStage), new XElement("AutoAdvance", autoAdvance), new XElement("MassPerGrade", massPerGrade),
		new XElement("RiskPercentPerGrade", riskPercentPerGrade)), spell) { }

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => CharacterPlanarDefinition is null;
	public bool RequiresTarget => true;
	public long PlaneId { get; private set; }
	public bool RevealStage { get; private set; }
	public bool AutoAdvance { get; private set; }
	public double MassPerGrade { get; private set; }
	public double RiskPercentPerGrade { get; private set; }
	public PlanarPresenceDefinition? CharacterPlanarDefinition { get; private set; }
	protected virtual Random DestructionRiskRandom => Random.Shared;
	private void Validate()
	{
		if (PlaneId <= 0 || !double.IsFinite(MassPerGrade) || MassPerGrade <= 0 ||
			!double.IsFinite(RiskPercentPerGrade) || RiskPercentPerGrade is < 0 or > 100)
			throw new FormatException("Attunement requires a plane, positive mass per grade, and risk from 0 to 100 percent per grade.");
	}
	public const string HelpText = "Attunes an item in two stages: ready permits inventory retention; reveal also makes it visible to the plane. Items must weigh less than grade times mass-per-grade. Each successful stage risks destruction by grade times risk-per-grade percent. Configure plane, stage auto|ready|reveal, mass <base units per grade>, risk <percent per grade>, or characterstate <plane> <corporeal|noncorporeal>|none for optional mixed targeting.";
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "etherealitemattunement"),
		new XElement("PlaneId", PlaneId), new XElement("RevealStage", RevealStage), new XElement("AutoAdvance", AutoAdvance),
		new XElement("MassPerGrade", MassPerGrade.ToString("R", CultureInfo.InvariantCulture)),
		new XElement("RiskPercentPerGrade", RiskPercentPerGrade.ToString("R", CultureInfo.InvariantCulture)),
		CharacterPlanarDefinition?.SaveToXml() is { } characterData
			? new XElement("CharacterPlanarData", characterData)
			: null);
	public IMagicSpellEffectTemplate Clone() => new EtherealItemAttunementEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => IsCompatibleWithTrigger(trigger.TargetTypes);
	private static bool IsCompatibleWithTrigger(string types) => types is "item" or "items" or "character" or "characters" or "perceivable" or "perceivables";
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Ethereal Item Attunement",
		("Plane", actor.Gameworld.Planes.Get(PlaneId)?.Name ?? PlaneId.ToString()),
		("Character State", CharacterPlanarDefinition?.Describe(Gameworld) ?? "disabled"),
		("Stage", AutoAdvance ? "auto" : RevealStage ? "reveal" : "ready"), ("Mass per grade", MassPerGrade.ToString("N2", actor)),
		("Risk per grade", $"{RiskPercentPerGrade.ToString("N2", actor)}%"));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant(); var value = command.SafeRemainingArgument;
		if (field == "characterstate")
		{
			if (value.EqualTo("none")) CharacterPlanarDefinition = null;
			else
			{
				var args = new StringStack(value);
				var characterPlane = Gameworld.Planes.GetByIdOrName(args.PopSpeech());
				var state = args.PopSpeech().ToLowerInvariant();
				if (characterPlane is null || !args.IsFinished || state is not ("corporeal" or "noncorporeal"))
				{ actor.Send(HelpText); return false; }
				var data = (state == "corporeal" ? PlanarPresenceDefinition.Manifested(characterPlane) :
					PlanarPresenceDefinition.NonCorporeal(characterPlane)).SaveToXml();
				data.Element("Flags")!.SetAttributeValue("retainPlaneSpecificInventory", state == "noncorporeal");
				CharacterPlanarDefinition = PlanarPresenceDefinition.FromXml(data, Gameworld);
			}
		}
		else if (field == "plane" && actor.Gameworld.Planes.GetByIdOrName(value) is { } plane) PlaneId = plane.Id;
		else if (field == "stage" && value is "auto" or "ready" or "reveal")
		{
			AutoAdvance = value == "auto";
			RevealStage = value == "reveal";
		}
		else if (field == "mass" && double.TryParse(value, NumberStyles.Float, actor, out var mass) && double.IsFinite(mass) && mass > 0) MassPerGrade = mass;
		else if (field == "risk" && double.TryParse(value, NumberStyles.Float, actor, out var risk) && double.IsFinite(risk) && risk is >= 0 and <= 100) RiskPercentPerGrade = risk;
		else { actor.OutputHandler.Send(HelpText); return false; }
		Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var priorError))
				throw new InvalidOperationException(priorError);
			return prior;
		}
		if (Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native)
			throw new InvalidOperationException("Ethereal item attunement requires a native spell grade.");
		var grade = native.InvocationGrade.Value;
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var casterBody = caster.Body;
		var casterRoom = caster.Location;
		var casterLayer = caster.RoomLayer;
		bool Current() => ReferenceEquals(caster.Gameworld, Gameworld) && caster.Location is not null &&
			ReferenceEquals(caster.Body, casterBody) && ReferenceEquals(caster.Location, casterRoom) && caster.RoomLayer == casterLayer &&
			Spell is MagicSpell currentNative && currentNative.InvocationGrade == grade &&
			SaveToXml().ToString(SaveOptions.DisableFormatting) == definition;
		if (recipient is IGameItem item)
		{
			var accessCurrent = CaptureItemAccess(item);
			var prototype = item.Prototype; var custodian = item.InInventoryOf; var container = item.ContainedIn;
			var location = item.Location; var weight = item.Weight; var affinity = AffinityFor(item);
			var affinityReady = affinity?.IsReadyForTransition; var affinityVisible = affinity?.IsVisibleToThatPlane;
			bool ItemCurrent() => accessCurrent() && Current() && !item.Deleted && !item.Destroyed &&
				ReferenceEquals(Gameworld.Items.Get(item.Id), item) && ReferenceEquals(item.Prototype, prototype) &&
				ReferenceEquals(item.InInventoryOf, custodian) && ReferenceEquals(item.ContainedIn, container) &&
				ReferenceEquals(item.Location, location) && item.Weight == weight &&
				item.TrueLocations.Contains(caster.Location!) && ReferenceEquals(AffinityFor(item), affinity) &&
				affinity?.IsReadyForTransition == affinityReady && affinity?.IsVisibleToThatPlane == affinityVisible &&
				EligibleStage(affinity) && weight < grade * MassPerGrade;
			if (!ItemCurrent() || !AdmitsItem(caster, item) || !ItemCurrent())
				throw new InvalidOperationException("The item is not visible, reachable, light enough, or ready for this attunement stage.");
			var token = new Selection(caster, recipient, grade, definition, ItemCurrent);
			_selections[recipient] = token; return token;
		}
		if (recipient is not ICharacter character || CharacterPlanarDefinition is null)
			throw new InvalidOperationException("This effect only accepts an item or a character when CharacterPlanarData is configured.");
		var targetBody = character.Body;
		var targetLocation = character.Location;
		bool CharacterCurrent() => Current() && ReferenceEquals(character.Body, targetBody) &&
			ReferenceEquals(character.Location, targetLocation);
		var characterToken = new Selection(caster, recipient, grade, definition, CharacterCurrent);
		_selections[recipient] = characterToken; return characterToken;
	}

	private static bool AdmitsItem(ICharacter caster, IGameItem item) =>
		caster.CanReachItem(item, requireInventoryPermission: false).Truth && caster.CanSee(item);

	/// <summary>Freeze containment before live reach callbacks; raw payment fences must not evaluate access policy.</summary>
	private static Func<bool> CaptureItemAccess(IGameItem item)
	{
		var checks = new List<Func<bool>>();
		var edges = new List<Func<bool>>();
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var ancestors = new List<IGameItem>();
		for (IGameItem? current = item; current is not null; current = current.ContainedIn)
		{
			if (!seen.Add(current)) throw new InvalidOperationException("The attunement item has cyclic containment.");
			ancestors.Add(current);
		}
		foreach (var captured in ancestors)
		{
			var parent = captured.ContainedIn;
			edges.Add(() => ReferenceEquals(captured.ContainedIn, parent));
			var holder = captured.InInventoryOf;
			var location = captured.Location;
			var layer = captured.RoomLayer;
			var owner = holder?.Actor;
			var ownerLocation = owner?.Location;
			var ownerLayer = owner?.RoomLayer;
			var openable = captured.GetItemType<IOpenable>();
			var open = openable?.IsOpen;
			checks.Add(() => !captured.Deleted && !captured.Destroyed &&
				ReferenceEquals(captured.InInventoryOf, holder) &&
				ReferenceEquals(captured.Location, location) && captured.RoomLayer == layer &&
				ReferenceEquals(holder?.Actor, owner) && ReferenceEquals(owner?.Location, ownerLocation) && owner?.RoomLayer == ownerLayer &&
				ReferenceEquals(captured.GetItemType<IOpenable>(), openable) && openable?.IsOpen == open);
		}
		return () => edges.All(check => check()) && checks.All(check => check());
	}

	private EtherealItemAffinityEffect? AffinityFor(IGameItem item) =>
		item.EffectsOfType<EtherealItemAffinityEffect>().FirstOrDefault(x => x.PlaneId == PlaneId);

	private bool EligibleStage(EtherealItemAffinityEffect? affinity)
	{
		if (affinity?.IsVisibleToThatPlane == true) return false;
		if (AutoAdvance) return true;
		return RevealStage ? affinity?.IsReadyForTransition == true : affinity?.IsReadyForTransition != true;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The selected target, item custody, weight, grade or effect definition changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Target, recipient) ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || !token.IsCurrent ||
			(recipient is IGameItem item && !AdmitsItem(caster, item)) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The exact attunement target was not prepared before payment.";
		return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}

	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		string? error = null;
		if (target is null || !TryConfirmPreparedSelection(caster, target, out error))
			throw new InvalidOperationException(error ?? "The exact attunement target changed before application.");
		if (target is ICharacter character && CharacterPlanarDefinition is not null)
			return new(MagicEffectOperationStatus.Applied,
				new SpellPlanarStateEffect(character, parent, CharacterPlanarDefinition));
		if (target is not IGameItem item || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native)
			throw new InvalidOperationException("The prepared attunement target is no longer valid.");
		var affinity = AffinityFor(item);
		var reveal = AutoAdvance ? affinity?.IsReadyForTransition == true : RevealStage;
		if (reveal && affinity?.IsReadyForTransition != true)
			throw new InvalidOperationException("The reveal stage requires prior ready attunement.");
		var ready = true;
		var visible = reveal || affinity?.IsVisibleToThatPlane == true;
		var before = PlanarVisibilityEchoHelper.CaptureVisibleObservers(item);
		if (RiskPercentPerGrade > 0 && DestructionRiskRandom.NextDouble() * 100 < Math.Min(100, native.InvocationGrade.Value * RiskPercentPerGrade))
		{
			item.Delete();
			if (!item.Deleted) throw new InvalidOperationException("The item destruction risk succeeded but native item deletion did not complete.");
			_selections.Remove(item);
			return new(MagicEffectOperationStatus.Applied, null);
		}
		if (affinity is not null) item.RemoveEffect(affinity);
		item.AddEffect(new EtherealItemAffinityEffect(item, PlaneId, ready, visible));
		PlanarVisibilityEchoHelper.EchoVisibilityChanges(item, before);
		_selections.Remove(item);
		return new(MagicEffectOperationStatus.Applied, null);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		Apply(caster, target, outcome, power, parent, additionalParameters).Effect;
}
