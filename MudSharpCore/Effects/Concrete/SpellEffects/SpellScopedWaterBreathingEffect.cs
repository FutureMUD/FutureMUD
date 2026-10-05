#nullable enable

using MudSharp.Form.Material;
using MudSharp.Magic;
using MudSharp.Magic.WaterBreathing;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>Native parent owns persistence and expiry; only authored liquid definitions are granted.</summary>
public sealed class SpellScopedWaterBreathingEffect : SpellWaterBreathingEffect
{
	private readonly WaterBreathingFluidScope? _scope;
	private readonly XElement? _unreadableScope;

	public new static void InitialiseEffectType() => RegisterFactory("SpellScopedWaterBreathing",
		(root, owner) => new SpellScopedWaterBreathingEffect(root, owner));

	internal SpellScopedWaterBreathingEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		WaterBreathingFluidScope scope) : base(owner, parent) => _scope = scope;

	private SpellScopedWaterBreathingEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var element = root.Element("Effect")?.Element("WaterScope");
		try { _scope = WaterBreathingScopeXml.Read(element, Gameworld); }
		catch (Exception error) when (error is FormatException or OverflowException or ArgumentException)
		{
			_unreadableScope = element is null ? new XElement("WaterScope") : new XElement(element);
		}
	}

	internal bool MatchesScope(WaterBreathingFluidScope scope) => _scope is not null && ApplicabilityProg is null &&
		ReferenceEquals(Owner, ParentEffect?.Owner) &&
		_scope.LiquidIds.SequenceEqual(scope.LiquidIds) && WaterBreathingScopeXml.IsCurrent(_scope, Gameworld);

	public override bool AppliesToFluid(IFluid fluid) => _scope?.Allows(fluid) == true;
	protected override string SpecificEffectType => "SpellScopedWaterBreathing";
	protected override XElement SaveDefinition() => SimpleSaveDefinition(
		_scope is not null ? WaterBreathingScopeXml.Write(_scope) : new XElement(_unreadableScope!));
	public override string Describe(IPerceiver voyeur) => _scope is null
		? "Water breathing with unreadable liquid mappings (inactive)."
		: $"Magically able to breathe mapped water liquids: {string.Join(", ", _scope.LiquidIds)}.";
}
