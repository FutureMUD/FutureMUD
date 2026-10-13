#nullable enable

using System.Globalization;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>A configurable contact barrier; the ordinary spell parent owns lifetime and dispel.</summary>
public sealed class DamagingExitBarrierEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectAdmission
{
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record Selection(ICharacter Caster, IExit Exit, int Grade, string Definition, long[] Planes,
		Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool IsCurrent => Check(); }
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("damagingexitbarrier", (xml, spell) => new DamagingExitBarrierEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("damagingexitbarrier", (_, spell) =>
			(new DamagingExitBarrierEffect(new XElement("Effect", new XAttribute("type", "damagingexitbarrier"),
				new XElement("DamageType", (int)DamageType.Burning), new XElement("DamagePerGrade", 3)), spell), ""),
			"Blocks and injures admitted physical crossing attempts", HelpText, false, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "exit").ToArray());
	}
	public DamagingExitBarrierEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		DamageType = (DamageType)(int)(xml.Element("DamageType") ?? throw new FormatException("Missing native barrier damage type."));
		DamagePerGrade = (double)(xml.Element("DamagePerGrade") ?? throw new FormatException("Missing barrier damage budget."));
		if (!Enum.IsDefined(DamageType) || !ValidAmount(DamagePerGrade)) throw new FormatException("Choose a defined native damage type and finite positive per-grade damage at most 1000000.");
	}
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public DamageType DamageType { get; private set; }
	public double DamagePerGrade { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public const string HelpText = "Options: type <native damage type>, damage <amount per external grade>. Injures one admitted physical crossing attempt after native door and guard checks, then blocks passage. Burning is the editable authored default. Duration belongs to the spell parent.";
	private static bool ValidAmount(double value) => double.IsFinite(value) && value is > 0 and <= 1000000;
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "damagingexitbarrier"), new XElement("DamageType", (int)DamageType), new XElement("DamagePerGrade", DamagePerGrade));
	public IMagicSpellEffectTemplate Clone() => new DamagingExitBarrierEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes == "exit";
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Damaging Exit Barrier",
		("Native Damage", DamageType.DescribeEnum().ColourName()), ("Damage per grade", DamagePerGrade.ToString("N2", actor).ColourValue()));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant(); var text = command.SafeRemainingArgument;
		if (field == "type" && text.TryParseEnum<DamageType>(out var type) && Enum.IsDefined(type)) DamageType = type;
		else if (field == "damage" && double.TryParse(text, NumberStyles.Float, actor, out var amount) && ValidAmount(amount)) DamagePerGrade = amount;
		else { actor.OutputHandler.Send(HelpText); return false; }
		Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{ if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error); return existing; }
		if (recipient is not IExit exit || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native ||
			caster.Body is not { } body || !ReferenceEquals(body.Actor, caster) || caster.Location is not { } origin ||
			!ReferenceEquals(caster.Gameworld, Gameworld) || !ReferenceEquals(exit.Gameworld, Gameworld) ||
			exit.RoomExitFor(origin) is not { } direction || direction.Destination is not { } destination ||
			ReferenceEquals(origin, destination) || !ReferenceEquals(origin.Gameworld, Gameworld) || !ReferenceEquals(destination.Gameworld, Gameworld))
			throw new InvalidOperationException("A contact barrier requires the caster's current body and a local reciprocal exit in this world.");
		var instance = caster.InstanceId; var identity = caster.Identity; var layer = caster.RoomLayer;
		var rooms = exit.Rooms.ToArray(); var opposite = direction.Opposite;
		var actorEffects = caster.Effects.ToArray(); var bodyEffects = body.Effects.ToArray();
		var basePlane = body.BasePlanarPresence.SaveToXml().ToString(SaveOptions.DisableFormatting);
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting); var grade = native.InvocationGrade.Value;
		bool Raw() => ReferenceEquals(caster.Body, body) && ReferenceEquals(body.Actor, caster) &&
			ReferenceEquals(caster.Gameworld, Gameworld) && ReferenceEquals(exit.Gameworld, Gameworld) &&
			body.BasePlanarPresence.SaveToXml().ToString(SaveOptions.DisableFormatting) == basePlane &&
			ReferenceEquals(origin.Gameworld, Gameworld) && ReferenceEquals(destination.Gameworld, Gameworld) &&
			ReferenceEquals(caster.Location, origin) && caster.RoomLayer == layer && caster.InstanceId == instance && ReferenceEquals(caster.Identity, identity) &&
			ReferenceEquals(exit.RoomExitFor(origin), direction) && ReferenceEquals(direction.Exit, exit) && ReferenceEquals(direction.Origin, origin) &&
			ReferenceEquals(direction.Destination, destination) && ReferenceEquals(direction.Opposite, opposite) && ReferenceEquals(opposite?.Exit, exit) &&
			ReferenceEquals(opposite?.Origin, destination) && ReferenceEquals(opposite?.Destination, origin) &&
			rooms.Length == 2 && rooms.Contains(origin) && rooms.Contains(destination) && rooms.SequenceEqual(exit.Rooms, ReferenceEqualityComparer.Instance) &&
			actorEffects.SequenceEqual(caster.Effects, ReferenceEqualityComparer.Instance) && bodyEffects.SequenceEqual(body.Effects, ReferenceEqualityComparer.Instance) &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition;
		var presence = caster.GetPlanarPresence();
		var planes = presence.InteractionPlaneIds(PlanarInteractionKind.MovementBarrier).Distinct().Order().ToArray();
		if (!Raw() || presence.SuspendsPhysicalContact || planes.Length == 0) throw new InvalidOperationException("The barrier's physical exit selection changed or has no contact plane.");
		var selection = new Selection(caster, exit, grade, definition, planes, Raw); _selections.Add(recipient, selection); return selection;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The barrier's prepared caster, body or exact exit changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Exit, recipient) || !token.IsCurrent ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade || SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "The barrier has no prepared exact exit."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power,
		TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null; error = "A contact barrier requires a positive bounded duration and a current prepared exit.";
		if (!TryConfirmPreparedSelection(caster, target, out var selectionError))
			throw new InvalidOperationException(selectionError);
		if (resolvedDuration <= TimeSpan.Zero || resolvedDuration > TimeSpan.FromDays(365)) return false;
		var token = _selections[target];
		application = new Application(this, token, RuntimeClock.UtcNow + resolvedDuration); return true;
	}
	private sealed class Application(DamagingExitBarrierEffect template, Selection selection, DateTime deadline) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			if (!selection.IsCurrent || !CommandExecutionScope.TryContinue() || !selection.IsCurrent || RuntimeClock.UtcNow >= deadline ||
				!ReferenceEquals(parent.Owner, selection.Exit)) throw new InvalidOperationException("The admitted barrier changed before creation.");
			return new SpellDamagingExitBarrierEffect(selection.Exit, parent, template.DamageType,
				template.DamagePerGrade * selection.Grade, selection.Caster.RoomLayer, selection.Planes, deadline);
		}
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null || parent is not MagicSpellParent native || !TryPrepareApplication(caster, target, outcome, power, native.ResolvedDuration, out var application, out _))
			throw new InvalidOperationException("The contact barrier was not admitted.");
		return application!.Create(parent);
	}
}
