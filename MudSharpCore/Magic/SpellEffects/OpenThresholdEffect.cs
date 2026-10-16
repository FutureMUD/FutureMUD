#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class OpenThresholdEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectInvocationInputs, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, SpellThresholdDestinationRequest> _requests = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public OpenThresholdEffect(XElement xml, IMagicSpell spell) { _definition = new(xml); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	private long Restricted => (long?)_definition.Element("RestrictedTag") ?? 0;
	private long Lethal => (long?)_definition.Element("LethalTag") ?? 0;
	public string? DefinitionError => (int?)_definition.Attribute("version") != 1 || Gameworld.Tags.Get(Restricted) is null || Gameworld.Tags.Get(Lethal) is null ? "Bind exact restricted and lethal room tags." : null;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new OpenThresholdEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is CastingTriggerThresholdDestination;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("openthreshold", (xml, spell) => new OpenThresholdEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("openthreshold", (_, spell) => (new OpenThresholdEffect(new XElement("Effect", new XAttribute("type", "openthreshold"),
			new XAttribute("version", 1), new XElement("RestrictedTag", 0), new XElement("LethalTag", 0)), spell), ""),
			"One admitted two-endpoint native transient threshold", "restricted|lethal <tag>", false, true, ["thresholddestination"]);
	}
	public bool TryBindInvocationInputs(ICharacter caster, IPerceivable recipient, IReadOnlyList<SpellAdditionalParameter> parameters, out string? error)
	{
		error = "Select one exact Arcane Mark or recognised traveller.";
		if (parameters.Count != 1 || parameters[0].ParameterName != SpellThresholdDestinationRequest.ParameterName || parameters[0].Item is not SpellThresholdDestinationRequest request ||
			(request.Mark is null) == (request.Traveller is null)) return false;
		_requests[recipient] = request; error = null; return true;
	}
	private bool EndpointsAllowed(ICharacter caster, IRoom source, IRoom destination, RoomLayer layer) => !ReferenceEquals(source, destination) && source.SharesPlaneWith(destination) &&
		!source.IsA(Gameworld.Tags.Get(Restricted)) && !destination.IsA(Gameworld.Tags.Get(Restricted)) && !source.IsA(Gameworld.Tags.Get(Lethal)) && !destination.IsA(Gameworld.Tags.Get(Lethal)) &&
		SpellTravelEndpointPolicy.ConnectionError(caster, source, destination, Spell, layer) is null;
	private sealed record Selection(ICharacter Caster, IRoom Source, IRoom Destination, RoomLayer Layer, int Grade, SpellTravelSnapshot Physical,
		Func<bool> Environmental, Func<bool> Selector, ResolvedSpellTravelAnchor? Mark, SpellMagicTagEffect? MarkEffect, MagicSpellParent? MarkParent, int Uses,
		ISpellMarkedRoomUseService? Committer) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool Attempted { get; set; }
		public bool IsCurrent => !Attempted && Physical.AtCapturedPositions && Environmental() && Selector();
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior)) return prior;
		if (DefinitionError is not null || !ReferenceEquals(recipient, caster.Location) || !_requests.TryGetValue(recipient, out var request) || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } spell)
			throw new InvalidOperationException("A threshold needs a current exact destination and selected grade.");
		ResolvedSpellTravelAnchor? anchor = null; SpellMagicTagEffect? mark = null; MagicSpellParent? parent = null; var uses = 0;
		IRoom destination; Func<bool> selector; ISpellMarkedRoomUseService? committer = null;
		if (request.Mark is { } item)
		{
			if (!SpellTravelAnchorResolver.TryResolve(caster, item, SpellTravelAnchorMode.ArcaneMarkedRoom, out anchor, out _) || anchor!.Effect is not SpellMagicTagEffect typed ||
				typed.ParentEffect is not MagicSpellParent head || head.Identity == Guid.Empty || !caster.Body.DirectItems.Any(x => ReferenceEquals(x, item)) || Gameworld.SpellMarkedRoomUses is not { } service)
				throw new InvalidOperationException("The exact carried Arcane Mark and durable use authority are required.");
			mark = typed; parent = head; uses = typed.RemainingUses; destination = anchor.Destination; committer = service;
			var effects = item.Effects.ToArray(); var children = head.SpellEffects.ToArray(); var owner = item.OwnershipReference; var identity = head.Identity;
			var expected = anchor; var expectedUses = uses;
			selector = () => typed.RemainingUses == expectedUses && head.Identity == identity && item.OwnershipReference == owner &&
				item.Effects.SequenceEqual(effects, ReferenceEqualityComparer.Instance) && head.SpellEffects.SequenceEqual(children, ReferenceEqualityComparer.Instance) &&
				ReferenceEquals(Gameworld.SpellMarkedRoomUses, service) && expected.IsCurrentFor(caster);
		}
		else if (request.Traveller is { } traveller && CastingTriggerRecognisedCharacter.IsRecognised(caster, traveller))
		{
			destination = traveller.Location; var recognition = new RecognisedIdentitySnapshot(caster, traveller);
			selector = () => recognition.Current && ReferenceEquals(traveller.Location, destination) && CastingTriggerRecognisedCharacter.IsRecognised(caster, traveller) && recognition.Current;
		}
		else throw new InvalidOperationException("The exact recognised traveller is no longer available.");
		var source = caster.Location; var layer = caster.RoomLayer; var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var physical = new SpellTravelSnapshot(Gameworld, request.Traveller is { } other ? [caster, other] : [caster], [source, destination], requireSolo: false);
		bool Environment() => definition == SaveToXml().ToString(SaveOptions.DisableFormatting);
		var token = new Selection(caster, source, destination, layer, spell.InvocationGrade.Value, physical, Environment, selector, anchor, mark, parent, uses, committer);
		if (!token.IsCurrent || !EndpointsAllowed(caster, source, destination, layer) || !token.IsCurrent) throw new InvalidOperationException("The threshold endpoints or selector are not permitted.");
		_selections.Add(recipient, token); return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The exact threshold endpoints or selector changed.";
		if (selection is not Selection token || !ReferenceEquals(caster, token.Caster) || !ReferenceEquals(recipient, token.Source) || Spell is not MagicSpell spell || spell.InvocationGrade != token.Grade ||
			!token.IsCurrent || !EndpointsAllowed(caster, token.Source, token.Destination, token.Layer) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "Threshold inputs were not prepared."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out _)) return new(MagicEffectOperationStatus.Rejected, null);
		var token = _selections[target];
		if (token.Mark is { } mark && token.MarkEffect is { } effect && token.MarkParent is { } head)
		{
			token.Committer!.Debit(mark.Item.Id, caster.Body.Id, head.Identity, head.Spell.Id, token.Destination.Id, token.Uses, () =>
				token.IsCurrent && EndpointsAllowed(caster, token.Source, token.Destination, token.Layer) && token.IsCurrent);
			effect.AcceptCommittedMarkedRoomUse(token.Uses);
		}
		token.Attempted = true;
		if (!token.Physical.AtCapturedPositions || !token.Environmental() || !EndpointsAllowed(caster, token.Source, token.Destination, token.Layer) ||
			!token.Physical.AtCapturedPositions || token.Mark is null && !token.Selector())
			throw new InvalidOperationException("The paid threshold needs review; never retry a committed Arcane Mark use.");
		return new(MagicEffectOperationStatus.Applied, new SpellOpenThresholdEffect(token.Source, parent, token.Destination, token.Layer, token.Layer,
			Restricted, Lethal, RuntimeClock.UtcNow.AddSeconds(120 * token.Grade)));
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => "One admitted two-endpoint threshold for 120 seconds per grade, to a carried Arcane Mark or recognised traveller.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch(); if (option is not ("restricted" or "lethal") || Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument) is not { } tag) return false;
		_definition.SetElementValue(option == "restricted" ? "RestrictedTag" : "LethalTag", tag.Id); Spell.Changed = true; actor.OutputHandler.Send("The threshold room policy has been updated."); return true;
	}
}
