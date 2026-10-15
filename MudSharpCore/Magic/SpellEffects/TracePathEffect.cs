#nullable enable

using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Magic.SpellTriggers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class TracePathEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public TracePathEffect(XElement xml, IMagicSpell spell) { _definition = new(xml); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	private int EdgesPerGrade => (int?)_definition.Element("EdgesPerGrade") ?? 0;
	public string? DefinitionError => (string?)_definition.Attribute("version") != "1" || EdgesPerGrade is < 1 or > 20 ? "Path queries need 1-20 edges per grade." : null;
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is CastingTriggerRecognisedCharacter;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new TracePathEffect(SaveToXml(), Spell);
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("tracepath", (xml, spell) => new TracePathEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("tracepath", (_, spell) => (new TracePathEffect(new XElement("Effect",
			new XAttribute("type", "tracepath"), new XAttribute("version", 1), new XElement("EdgesPerGrade", 10)), spell), ""),
			"Reveals a bounded permitted route to a recognised character", "edges <per grade>", true, true, ["recognisedcharacter"]);
	}
	private sealed record Selection(ICharacter Caster, ICharacter Target, int Grade, IReadOnlyList<IRoomExit> Path,
		SpellTravelSnapshot Physical, RecognisedIdentitySnapshot Recognition, string Definition, string Output, Func<bool> Raw) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool Attempted { get; set; }
		public bool IsCurrent => !Attempted && Physical.AtCapturedPositions && Recognition.Current && Raw();
	}
	private bool Live(Selection token) => CastingTriggerRecognisedCharacter.IsRecognised(token.Caster, token.Target) &&
		token.Path.All(edge => edge.Origin.ExitsFor(token.Caster).Any(x => ReferenceEquals(x.Exit, edge.Exit) && ReferenceEquals(x.Destination, edge.Destination)) &&
			SpellPermittedPath.Admitted(token.Caster, edge));
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior))
		{ if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error); return prior; }
		if (DefinitionError is not null || recipient is not ICharacter target || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } spell)
			throw new InvalidOperationException("Trace the Path requires its recognised target and selected grade.");
		var recognition = new RecognisedIdentitySnapshot(caster, target);
		var endpoints = new SpellTravelSnapshot(Gameworld, [caster, target], [caster.Location, target.Location]);
		if (!CastingTriggerRecognisedCharacter.IsRecognised(caster, target) || !recognition.Current)
			throw new InvalidOperationException("The exact recognised traveller is unavailable.");
		var grade = spell.InvocationGrade.Value;
		var path = SpellPermittedPath.Find(caster, caster.Location, target.Location, EdgesPerGrade * grade) ??
			throw new InvalidOperationException("No permitted route reaches that traveller within this grade's bound.");
		var physical = new SpellTravelSnapshot(Gameworld, [caster, target], path.SelectMany(x => new[] { x.Origin, x.Destination }).Concat([caster.Location, target.Location]));
		var checks = new List<Func<bool>>();
		checks.Add(SpellPermittedPath.CaptureRawPermissions(caster, path));
		void Value<T>(Func<T> read) { var before = read(); checks.Add(() => EqualityComparer<T>.Default.Equals(before, read())); }
		foreach (var edge in path)
		{
			var exit = edge.Exit; var door = exit.Door; var origin = edge.Origin; var destination = edge.Destination;
			checks.Add(() => ReferenceEquals(edge.Exit, exit) && ReferenceEquals(exit.Door, door) && ReferenceEquals(edge.Origin, origin) && ReferenceEquals(edge.Destination, destination));
			Value(() => exit.Id); Value(() => door?.IsOpen); Value(() => string.Join(",", origin.CurrentOverlay.ExitIDs));
			Value(() => edge.OutboundDirection); Value(() => edge.OutboundDirectionDescription);
		}
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		checks.Add(() => endpoints.AtCapturedPositions && spell.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		var output = path.Count == 0 ? "That traveller is already here." : "The permitted path leads " + string.Join(", then ", path.Select(x => x.OutboundDirectionDescription)) + ".";
		var token = new Selection(caster, target, grade, path, physical, recognition, definition, output, () => checks.All(x => x()));
		if (!token.IsCurrent || !Live(token) || !token.IsCurrent) throw new InvalidOperationException("The permitted path changed during admission.");
		_selections.Add(recipient, token); return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The exact recognised traveller or permitted path changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Target, recipient) ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade || SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition ||
			!token.IsCurrent || !Live(token) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "The permitted path was not prepared."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out _)) throw new InvalidOperationException("The prepared permitted route changed.");
		var token = _selections[target]; token.Attempted = true; caster.OutputHandler.Send(token.Output);
		return new(MagicEffectOperationStatus.Applied, null);
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => $"Reveal at most {EdgesPerGrade.ToString("N0", actor)} permitted native edges per grade toward a recognised traveller.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("edges") || !int.TryParse(command.PopSpeech(), out var edges) || edges is < 1 or > 20 || !command.IsFinished) return false;
		_definition.SetElementValue("EdgesPerGrade", edges); Spell.Changed = true; actor.OutputHandler.Send("The path edge bound has been updated."); return true;
	}
}
