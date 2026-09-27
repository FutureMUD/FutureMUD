#nullable enable

using MudSharp.Construction;
using MudSharp.Magic.Environment;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.Form.Shape;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellRejuvenateLandEffect : MagicSpellEffectBase, ILandRejuvenationEffect, IDescriptionAdditionEffect
{
	private readonly LandRejuvenationProgress? _initial;
	private readonly string _description;
	private readonly ANSIColour _colour;
	private ICharacter? _actingCaster;
	private bool _subscribed;
	private bool _activationRequested;
	private bool _ended;
	private string? _diagnostic;

	public static void InitialiseEffectType() => RegisterFactory("SpellRejuvenateLand", (xml, owner) => new SpellRejuvenateLandEffect(xml, owner));

	public SpellRejuvenateLandEffect(ICell cell, IMagicSpellEffectParent parent, ICharacter caster,
		LandRejuvenationProgress initial, string description, ANSIColour colour) : base(cell, parent, null!)
	{
		_initial = initial;
		TreatmentId = initial.Id;
		_actingCaster = caster;
		_description = description;
		_colour = colour;
	}

	private SpellRejuvenateLandEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var data = root.Element("Effect");
		if (!int.TryParse(data?.Attribute("version")?.Value, out var version) || version != 1 ||
			!Guid.TryParse(data?.Element("TreatmentId")?.Value, out var id))
		{
			_diagnostic = "Malformed/unknown-version treatment reference; no new budget is permitted.";
			_ended = true;
		}
		else TreatmentId = id;
		_description = data?.Element("Description")?.Value ?? string.Empty;
		_colour = Telnet.GetColour(data?.Element("Colour")?.Value ?? "green") ?? Telnet.Green;
	}

	public Guid TreatmentId { get; }
	public Guid ParentIdentity => (ParentEffect as MagicSpellParent)?.Identity ?? Guid.Empty;
	public ICell TreatmentCell => (ICell)Owner;
	public bool IsAttached => !_ended && Owner is ICell && ParentEffect is MagicSpellParent parent &&
		Owner.Effects.Contains(parent) && Owner.Effects.Contains(this) && parent.SpellEffects.Contains(this) &&
		parent.Spell is not null && Gameworld.MagicSpells.Get(parent.Spell.Id) is not null;
	private LandRejuvenationProgress? Progress => Gameworld.EnvironmentalMagic?.InspectTreatment(TreatmentCell, TreatmentId) ?? _initial;

	public void ActivateTreatment()
	{
		if (_activationRequested || _ended) return;
		_activationRequested = true;
		if (_initial is null) Gameworld.EnvironmentalMagic?.RegisterLoadedTreatment(this);
		else if (Gameworld.EnvironmentalMagic?.ActivateTreatment(this, _initial, out var error) != true)
			TreatmentEnded("Treatment activation failed; inspect environmental treatment diagnostics.");
	}

	public bool CheckMaintenance(out string? error)
	{
		error = null;
		var p = Progress;
		if (p is null || ParentEffect.Spell is null || p.SpellId != ParentEffect.Spell.Id || p.ParentId != ParentIdentity)
		{ error = "Treatment source/parent identity is unavailable."; return false; }
		if (!p.RequiresPresence && p.ContinuationProgId is null) return true;
		_actingCaster ??= Gameworld.Actors.FirstOrDefault(x => x.Id == p.CasterId && x.InstanceId == p.ActingInstanceId);
		var caster = _actingCaster;
		if (caster is null || caster.Id != p.CasterId || caster.InstanceId != p.ActingInstanceId || !Gameworld.Actors.Has(caster))
		{ error = "The original acting caster is not active."; return false; }
		if (p.RequiresPresence && (!ReferenceEquals(caster.Location, Owner) || (int)caster.RoomLayer != p.Layer ||
			!caster.State.IsConscious() || caster.State.HasFlag(CharacterState.Stasis) ||
			!caster.GetPlanarPresence().PresencePlaneIds.OrderBy(x => x).SequenceEqual(p.PlaneIds)))
		{ error = "The original caster no longer meets the captured presence requirement."; return false; }
		if (!_subscribed)
		{
			caster.OnQuit += CasterUnavailable;
			caster.OnDeleted += CasterUnavailable;
			caster.OnDeath += CasterUnavailable;
			caster.OnStateChanged += CasterStateChanged;
			caster.OnLocationChanged += CasterLocationChanged;
			_subscribed = true;
		}
		if (p.ContinuationProgId is not { } id) return true;
		var prog = Gameworld.FutureProgs.Get(id);
		if (!RejuvenateLandEffect.ValidPolicy(prog)) { error = "Continuation policy is missing or invalid."; return false; }
		return Gameworld.EnvironmentalMagic!.EvaluateRepairPolicy(TreatmentCell, caster, prog!, out error);
	}

	private void CasterUnavailable(IPerceivable _) => Gameworld.EnvironmentalMagic?.CancelTreatment(TreatmentCell, TreatmentId, "The required original caster became unavailable.");
	private void CasterLocationChanged(ILocateable _, ICellExit exit)
	{
		if (Progress is { RequiresPresence: true } p && _actingCaster is { } caster &&
			(!ReferenceEquals(caster.Location, Owner) || (int)caster.RoomLayer != p.Layer)) CasterUnavailable(caster);
	}
	private void CasterStateChanged(IPerceivable _)
	{
		if (_actingCaster is { } caster && (!caster.State.IsConscious() || caster.State.HasFlag(CharacterState.Stasis))) CasterUnavailable(caster);
	}
	private void Unsubscribe()
	{
		if (!_subscribed || _actingCaster is null) return;
		_actingCaster.OnQuit -= CasterUnavailable;
		_actingCaster.OnDeleted -= CasterUnavailable;
		_actingCaster.OnDeath -= CasterUnavailable;
		_actingCaster.OnStateChanged -= CasterStateChanged;
		_actingCaster.OnLocationChanged -= CasterLocationChanged;
		_subscribed = false;
	}

	public void ExpireTreatment() => Gameworld.EnvironmentalMagic?.ExpireTreatment(TreatmentCell, TreatmentId);
	public void CheckpointTreatment() => Gameworld.EnvironmentalMagic?.CheckpointTreatment(TreatmentCell, TreatmentId);
	public void TreatmentEnded(string diagnostic)
	{
		_diagnostic = diagnostic;
		_ended = true;
		Unsubscribe();
		if (Owner.Effects.Contains(this)) Owner.RemoveEffect(this, true);
	}
	public override void RemovalEffect()
	{
		Unsubscribe();
		if (!_ended)
		{
			_ended = true;
			Gameworld.EnvironmentalMagic?.CancelTreatment(TreatmentCell, TreatmentId, "The spell treatment was removed; uncommitted elapsed work was discarded.");
		}
		base.RemovalEffect();
	}

	protected override string SpecificEffectType => "SpellRejuvenateLand";
	protected override XElement SaveDefinition() => new("Effect", new XAttribute("version", 1),
		new XElement("TreatmentId", TreatmentId), new XElement("Description", new XCData(_description)), new XElement("Colour", _colour.Name));
	public override string Describe(IPerceiver voyeur)
	{
		var p = Owner is ICell ? Progress : null;
		return p is null ? $"Land rejuvenation {TreatmentId}: {_diagnostic ?? "missing authoritative progress"}" :
			$"Land rejuvenation {p.Id}: spell #{p.SpellId}, caster #{p.CasterId}, instance #{p.ActingInstanceId}; " +
			$"{p.Status}, rate {p.Rate.ToString("G", voyeur)}/minute, ceiling {Gameworld.EnvironmentalMagic!.InspectRepairPolicy(TreatmentCell).Ceiling?.ToString("G", voyeur) ?? "none"}, " +
			$"budget {p.RemainingBudget.ToString("G", voyeur)}/{p.InitialBudget.ToString("G", voyeur)}, repaired {p.TotalRepaired.ToString("G", voyeur)}, " +
			$"lifetime {TimeSpan.FromSeconds(p.RemainingSeconds).Describe(voyeur)}, step {p.Sequence}/{p.AcknowledgedSequence}, pending {p.PendingRequest?.OperationId.ToString() ?? "none"}. {p.Diagnostic}";
	}
	public bool PlayerSet => false;
	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		if (_ended || _description.Length == 0 || !ReferenceEquals(voyeur.Location, Owner)) return string.Empty;
		var p = Progress;
		if (p is null || p.IsTerminal || p.CancellationRequested || (int)voyeur.RoomLayer != p.Layer ||
			!((IPerceivable)voyeur).GetPlanarPresence().PresencePlaneIds.Intersect(p.PlaneIds).Any()) return string.Empty;
		return colour ? _description.Colour(_colour) : _description;
	}
}
