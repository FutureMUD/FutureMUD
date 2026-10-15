#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class ReturnTetherEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public ReturnTetherEffect(XElement definition, IMagicSpell spell) { _definition = new(definition); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	private double SecondsPerGrade => (double?)_definition.Element("SecondsPerGrade") ?? double.NaN;
	public string? DefinitionError => (string?)_definition.Attribute("version") != "1" || !double.IsFinite(SecondsPerGrade) ||
		SecondsPerGrade is <= 0 or > 86400 ? "Return Tether needs a finite positive delay of at most one day per grade." : null;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new ReturnTetherEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is SpellTriggers.CastingTriggerSelf;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("returntether", (xml, spell) => new ReturnTetherEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("returntether", (_, spell) =>
			(new ReturnTetherEffect(new XElement("Effect", new XAttribute("type", "returntether"), new XAttribute("version", 1), new XElement("SecondsPerGrade", 30)), spell), ""),
			"Durable delayed return to the caster's recorded position", "delay <seconds per grade>", false, true, ["self"]);
	}
	private sealed record Selection(ICharacter Caster, int Grade, SpellReturnAnchor Anchor, SpellTravelSnapshot Snapshot,
		ISpellReturnService Service, string Definition, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool Attempted { get; set; }
		public bool IsCurrent => !Attempted && Snapshot.AtCapturedPositions && Check();
	}
	private string? LiveError(ICharacter caster)
	{
		if (caster.Effects.OfType<SpellReturnTetherEffect>().Any()) return "An existing return tether must end before another is cast.";
		return SpellTravelEndpointPolicy.RelocationError(caster, caster, caster.Location, caster.Location, Spell, caster.RoomLayer);
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var stale)) throw new InvalidOperationException(stale);
			return existing;
		}
		if (!ReferenceEquals(caster, recipient) || DefinitionError is not null || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native ||
			Gameworld.SpellReturns is not { } service)
			throw new InvalidOperationException("Return Tether requires its caster, an admitted saved position and a selected grade.");
		var room = caster.Location; var grade = native.InvocationGrade.Value;
		var snapshot = new SpellTravelSnapshot(Gameworld, [caster], [room]);
		if (LiveError(caster) is { } refusal || !snapshot.AtCapturedPositions)
			throw new InvalidOperationException("The recorded return position was refused or changed during native admission.");
		var anchor = new SpellReturnAnchor(CharacterInstanceIdentityComparer.IdentityId(caster), caster.InstanceId, caster.Body.Id,
			room.Id, room.CurrentOverlay.Id, room.CurrentOverlay.Terrain.Id, caster.RoomLayer);
		SpellReturnAnchor.Parse(anchor.Save());
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var result = new Selection(caster, grade, anchor, snapshot, service, definition,
			() => native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition &&
				ReferenceEquals(Gameworld.SpellReturns, service));
		if (!result.IsCurrent) throw new InvalidOperationException("The return position changed during selection.");
		_selections.Add(recipient, result); return result;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The recorded return caster, position or binding changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(recipient, caster) ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade || !token.IsCurrent ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || LiveError(caster) is not null || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared return position is unavailable.";
		return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power,
		IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out var error)) throw new InvalidOperationException("Return Tether admission changed.");
		var token = _selections[target]; token.Attempted = true;
		var binding = token.Service.CreatePrepared(caster, Spell, token.Grade, SecondsPerGrade, token.Anchor,
			() => token.Snapshot.AtCapturedPositions && token.Check());
		return new(MagicEffectOperationStatus.Applied, new SpellReturnTetherEffect(caster, parent, binding));
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power,
		IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => $"Record the caster's exact position and attempt one journalled return after {SecondsPerGrade.ToString("N2", actor)} seconds per grade.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("delay") || !double.TryParse(command.PopSpeech(), NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) ||
			!double.IsFinite(seconds) || seconds is <= 0 or > 86400 || !command.IsFinished) return false;
		_definition.SetElementValue("SecondsPerGrade", seconds); Spell.Changed = true;
		actor.OutputHandler.Send("The delayed return interval has been updated."); return true;
	}
}
