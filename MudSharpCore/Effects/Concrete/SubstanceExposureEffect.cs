using MudSharp.Form.Material;
using MudSharp.Health;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.Effects.Concrete;

/// <summary>A retained spell parent owns the doses that sustain it. Dispel suppresses those doses.</summary>
public sealed class SubstanceExposureEffect : MagicSpellParent
{
	private sealed class Contribution
	{
		public double Quantity;
		public double Latent;
		public double Active;
		public double Remaining;
		public DrugVector Vector;
		public bool Surface;
		public List<SubstanceCharge> Charges = new();
	}
	private readonly List<Contribution> _doses = new();
	private readonly Dictionary<IMagicSpellEffect, int> _templateIndices = new(ReferenceEqualityComparer.Instance);
	private bool _naturalRemoval;
	private bool _updating;
	private double _absorptionElapsed;
	private double _pulseElapsed;
	private DateTime _lastTick = DateTime.UtcNow;
	public long SubstanceId { get; private set; }
	public SubstanceEffectEntry Entry { get; private set; }
	public IMagicalSubstance? Substance => Gameworld.MagicalSubstances.Get(SubstanceId);
	public double RemainingSeconds => !IsTimed ? 0 : _doses.Select(x => x.Remaining).DefaultIfEmpty().Max();
	public static new void InitialiseEffectType() => RegisterFactory("SubstanceExposure", (xml, owner) => new SubstanceExposureEffect(xml, owner));
	protected override string SpecificEffectType => "SubstanceExposure";
	public SubstanceExposureEffect(IPerceivable owner, IMagicalSubstance substance, SubstanceEffectEntry entry)
		: base(owner, substance.Gameworld.MagicSpells.Get(entry.SpellId), null!, substance.Power)
	{
		SubstanceId = substance.Id;
		Entry = MagicalSubstance.LoadEntry(MagicalSubstance.SaveEntry(entry));
	}
	private SubstanceExposureEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		SubstanceId = (long)root.Element("Substance")!;
		Entry = MagicalSubstance.LoadEntry(root.Element("Entry")!);
		var indices = root.Element("TemplateIndices")?.Elements("Index").Select(x => (int)x).ToList() ?? [];
		foreach (var pair in SpellEffects.Zip(indices)) _templateIndices[pair.First] = pair.Second;
		_pulseElapsed = (double?)root.Element("PulseElapsed") ?? 0;
		_absorptionElapsed = (double?)root.Element("AbsorptionElapsed") ?? 0;
		foreach (var d in root.Elements("Dose")) _doses.Add(new()
		{
			Quantity = (double)d.Attribute("quantity")!, Latent = (double)d.Attribute("latent")!, Active = (double)d.Attribute("active")!,
			Remaining = (double)d.Attribute("remaining")!, Vector = (DrugVector)(int)d.Attribute("vector")!, Surface = (bool)d.Attribute("surface")!,
			Charges = d.Elements("Charge").Select(SubstanceCharge.Load).ToList()
		});
	}
	protected override XElement SaveDefinition()
	{
		var root = base.SaveDefinition();
		root.Add(new XElement("TemplateIndices", SpellEffects.Select(x => new XElement("Index", _templateIndices.GetValueOrDefault(x, -1)))), new XElement("Substance", SubstanceId), MagicalSubstance.SaveEntry(Entry), new XElement("PulseElapsed", _pulseElapsed),
			new XElement("AbsorptionElapsed", _absorptionElapsed), _doses.Select(d => new XElement("Dose", new XAttribute("quantity", d.Quantity),
				new XAttribute("latent", d.Latent), new XAttribute("active", d.Active), new XAttribute("remaining", d.Remaining),
				new XAttribute("vector", (int)d.Vector), new XAttribute("surface", d.Surface), d.Charges.Select(x => x.Save()))));
		return root;
	}
	public void AddExposure(IEnumerable<(double Quantity, SubstanceCharge Charge)> parts, DrugVector vector, bool surface)
	{
		var incoming = parts.ToList();
		if (surface && !IsTimed)
			incoming.RemoveAll(x => _doses.Any(d => d.Surface && d.Charges.Any(c => c.Lot == x.Charge.Lot)));
		if (incoming.Count == 0) return;
		var quantity = incoming.Sum(x => x.Quantity);
		var dose = Math.Min(Entry.MaximumDose, quantity / Substance!.ReferenceDose);
		if (!surface && !IsTimed && Entry.Stacking == SubstanceStacking.Aggregate &&
			_doses.FirstOrDefault(x => !x.Surface && x.Vector == vector) is { } existing)
		{
			existing.Quantity += quantity; existing.Latent += quantity; Changed = true; return;
		}
		if (Entry.Stacking == SubstanceStacking.Replace) { Suppress(); _doses.Clear(); ClearChildren(); }
		var duration = Math.Min(Entry.MaximumDurationSeconds, Entry.DurationSeconds *
			(Entry.Scaling == SubstanceScaling.Duration || Entry.Lifecycle == SubstanceLifecycle.Periodic ? dose : 1));
		var contribution = new Contribution { Quantity = quantity, Latent = quantity, Vector = vector, Surface = surface,
			Charges = incoming.Select(x => x.Charge).DistinctBy(x => x.Lot).Select(x => x.Copy()).ToList(), Remaining = duration };
		if (IsTimed && Entry.Stacking == SubstanceStacking.Aggregate)
		{
			contribution.Quantity = Math.Min(Entry.MaximumDose * Substance.ReferenceDose, quantity + _doses.Sum(x => x.Quantity));
			contribution.Remaining = Math.Min(Entry.MaximumDurationSeconds, RemainingSeconds + duration);
			contribution.Charges.AddRange(_doses.SelectMany(x => x.Charges));
			// Timed doses have already spent their activation charge. Only retained liquid needs
			// provenance for later dispelling; historical internal doses share one capped reservoir.
			contribution.Charges = RetainedCharges(contribution.Charges);
			contribution.Surface = contribution.Charges.Count > 0;
			_doses.Clear();
		}
		_doses.Add(contribution);
		if (Entry.Lifecycle == SubstanceLifecycle.Activation) ReconcileChildren(CurrentDose());
		ScheduleNextTick();
		Changed = true;
	}
	private List<SubstanceCharge> RetainedCharges(IEnumerable<SubstanceCharge> charges)
	{
		var lots = MagicalExposure.RetainedLiquids(Owner)
			.Where(x => x.Instance.MagicalCharges.ContainsKey(SubstanceId))
			.Select(x => x.Instance.MagicalCharges[SubstanceId].Lot).ToHashSet();
		return charges.Where(x => lots.Contains(x.Lot)).DistinctBy(x => x.Lot).ToList();
	}
	public bool IsTimed => Entry.Lifecycle == SubstanceLifecycle.Activation ||
		Entry.Lifecycle == SubstanceLifecycle.Periodic && Entry.PulseMode == SubstancePulseMode.Timed;
	private double SurfaceQuantity(Contribution d)
	{
		return MagicalExposure.RetainedLiquids(Owner).Sum(x =>
			x.Instance.MagicalCharges.TryGetValue(SubstanceId, out var charge) && d.Charges.Any(c => c.Lot == charge.Lot) && !charge.Suppressed.Contains(Entry.Key)
				? x.Quantity * (Substance?.Bindings.FirstOrDefault(b => b.Carrier == SubstanceCarrier.Liquid && b.Id == x.Instance.Liquid.Id)?.QuantityPerUnit ?? 0) : 0);
	}
	private double CurrentDose()
	{
		if (Substance is not { } substance) return 0;
		var values = _doses
			.Select(x => IsTimed ? x.Quantity : x.Surface ? SurfaceQuantity(x) : x.Active).ToList();
		var quantity = Entry.Stacking == SubstanceStacking.Strongest ? values.DefaultIfEmpty().Max() : values.Sum();
		return Math.Min(Entry.MaximumDose, quantity / substance.ReferenceDose);
	}
	private void ClearChildren()
	{
		_updating = true;
		foreach (var child in SpellEffects.ToList()) Owner.RemoveEffect(child, true);
		_updating = false;
	}
	private void ReconcileChildren(double dose)
	{
		if (dose <= 0 || dose < Entry.MinimumDose) { ClearChildren(); return; }
		var magnitude = Entry.Lifecycle == SubstanceLifecycle.Maintained || Entry.Scaling == SubstanceScaling.Magnitude ? dose : 1.0;
		if (SpellEffects.Any())
		{
			foreach (var child in SpellEffects)
				if (_templateIndices.TryGetValue(child, out var index) && index >= 0 && index < Spell.SpellEffects.Count())
					SubstanceSpellResolver.Update(child, Spell.SpellEffects.ElementAt(index), magnitude, Power);
			return;
		}
		var context = new SubstanceResolutionContext(Owner, Substance!, dose);
		foreach (var (template, index) in Spell.SpellEffects.Select((x, i) => (x, i)).Where(x => !x.x.IsInstantaneous))
		{
			var child = SubstanceSpellResolver.Apply(template, Spell, context, this, magnitude);
			if (child is null) continue;
			Owner.AddEffect(child); AddSpellEffect(child); _templateIndices[child] = index;
		}
	}
	public override void ExpireEffect()
	{
		var now = DateTime.UtcNow;
		var elapsed = now - _lastTick;
		_lastTick = now;
		Advance(elapsed);
	}
	internal void Advance(TimeSpan interval)
	{
		if (Substance is not { } substance || SubstanceSpellResolver.Errors(Spell, Entry).Any()) { EndNaturally(); return; }
		if (_doses.Any(x => x.Surface))
			((Owner is ICharacter c ? c.Body : Owner) as ISurfaceContaminable)?.ResolveSurfaceLiquidDrying();
		var elapsed = Math.Clamp(interval.TotalSeconds, 0, 10);
		_absorptionElapsed += elapsed; _pulseElapsed += elapsed;
		foreach (var d in _doses)
		{
			if (IsTimed)
			{
				d.Remaining -= elapsed;
				if (Entry.Stacking == SubstanceStacking.Aggregate) d.Charges = RetainedCharges(d.Charges);
			}
			else if (!d.Surface && _absorptionElapsed >= 10)
				(d.Latent, d.Active) = SubstanceDose.Advance(d.Latent, d.Active, d.Vector, substance.ClearancePerTick);
		}
		if (_absorptionElapsed >= 10) _absorptionElapsed %= 10;
		_doses.RemoveAll(x => IsTimed ? x.Remaining <= 0 : x.Surface ? SurfaceQuantity(x) <= 0 : x.Latent + x.Active <= 0);
		if (_doses.Count == 0) { EndNaturally(); return; }
		var dose = CurrentDose();
		if (Entry.Lifecycle == SubstanceLifecycle.Periodic)
		{
			if (_pulseElapsed >= Entry.IntervalSeconds)
			{
				_pulseElapsed = 0;
				var context = new SubstanceResolutionContext(Owner, substance, dose);
				if (dose > 0 && dose >= Entry.MinimumDose && SubstanceSpellResolver.CanApply((MagicSpell)Spell, context))
					foreach (var template in Spell.SpellEffects) SubstanceSpellResolver.Apply(template, Spell, context, this,
						Entry.PulseMode == SubstancePulseMode.Presence ? dose : Entry.Scaling == SubstanceScaling.Magnitude ? dose : 1);
			}
		}
		else ReconcileChildren(dose);
		Changed = true;
		ScheduleNextTick();
	}
	private void ScheduleNextTick() => Owner.Reschedule(this, TimeSpan.FromSeconds(IsTimed ? Math.Clamp(RemainingSeconds, 0.001, 1) : 1));
	public void SetRemaining(TimeSpan duration)
	{
		if (!IsTimed) return;
		if (duration <= TimeSpan.Zero) { Owner.RemoveEffect(this, true); return; }
		foreach (var dose in _doses) dose.Remaining = Math.Clamp(duration.TotalSeconds, 0, Entry.MaximumDurationSeconds);
		ScheduleNextTick();
		Changed = true;
	}
	private void EndNaturally() { _naturalRemoval = true; Owner.RemoveEffect(this, true); }
	private void Suppress()
	{
		foreach (var charge in _doses.SelectMany(x => x.Charges)) charge.Suppressed.Add(Entry.Key);
		var surface = (Owner is ICharacter c ? c.Body : Owner) as ISurfaceContaminable;
		if (surface is null) return;
		foreach (var (instance, _) in MagicalExposure.RetainedLiquids(Owner))
			if (instance.MagicalCharges.TryGetValue(SubstanceId, out var charge) && _doses.Any(x => x.Charges.Any(c => c.Lot == charge.Lot))) charge.Suppressed.Add(Entry.Key);
		MagicalExposure.RetainedLiquidsChanged(Owner);
	}
	public override void RemovalEffect()
	{
		if (!_naturalRemoval) Suppress();
		_updating = true; base.RemovalEffect(); _updating = false;
	}
	public override void RemoveSpellEffect(IMagicSpellEffect effect)
	{
		if (!_updating) { Owner.RemoveEffect(this, true); return; }
		_templateIndices.Remove(effect);
		RemoveOwnedChild(effect);
	}
	public override string Describe(IPerceiver voyeur) => $"{Substance?.Name ?? "Unknown substance"}: {Entry.Lifecycle.DescribeEnum()} {Spell.Name}, dose {CurrentDose().ToString("N3", voyeur)}.";
}
