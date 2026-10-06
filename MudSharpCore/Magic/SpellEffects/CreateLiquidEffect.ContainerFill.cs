#nullable enable

using System.Globalization;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class CreateLiquidEffect
{
	private string? _fillLoadError;
	private XElement? _unreadableFill;
	private readonly HashSet<long> _compatibleLiquids = [];
	private long _bonusPlaneId;
	private double _planeMultiplier = 1;
	private double? _preparedAmount;
	private string? _litresFormula;
	public bool ContainerOnly { get; private set; }
	public ITraitExpression? LitresExpression { get; internal set; }
	public string? DefinitionError => _fillLoadError ?? RecipeError() ?? (!ContainerOnly ? null :
		Liquid is null ? "Configure a source liquid for container filling." :
		LitresExpression is null || LitresExpression.HasErrors() ? "Configure a valid litres formula." :
		LitresExpression.NonTraitParameters.Contains("outcome", StringComparer.OrdinalIgnoreCase) ? "Liquid quantity must be determinable before payment; outcome is not supported." :
		_compatibleLiquids.Any(x => Gameworld.Liquids.Get(x) is null) ? "A configured compatible liquid is missing." :
		_bonusPlaneId != 0 && Gameworld.Planes.Get(_bonusPlaneId) is null ? "The quantity bonus plane is missing." :
		!double.IsFinite(_planeMultiplier) || _planeMultiplier <= 0 ? "The plane multiplier must be finite and positive." : null);

	private void LoadContainerFill(XElement? root)
	{
		if (root is null) return;
		LoadRecipes(root.Element("Recipes"));
		if ((string?)root.Attribute("version") != "1" || root.Element("Litres") is null ||
			root.Elements("CompatibleLiquid").Any(x => !long.TryParse(x.Value, out var id) || id <= 0) ||
			root.Element("BonusPlane") is { } bonus &&
			(!long.TryParse(bonus.Value, out var plane) || plane <= 0 ||
			 !double.TryParse((string?)bonus.Attribute("multiplier"), NumberStyles.Float, CultureInfo.InvariantCulture, out var multiplier) ||
			 !double.IsFinite(multiplier) || multiplier <= 0))
		{ _fillLoadError = "Invalid container-fill schema or references."; _unreadableFill = new(root); return; }
		ContainerOnly = true;
		_litresFormula = root.Element("Litres")!.Value;
		LitresExpression = new TraitExpression(_litresFormula, Gameworld);
		foreach (var element in root.Elements("CompatibleLiquid")) _compatibleLiquids.Add(long.Parse(element.Value));
		if (root.Element("BonusPlane") is { } planeElement)
		{
			_bonusPlaneId = long.Parse(planeElement.Value);
			_planeMultiplier = (double)planeElement.Attribute("multiplier")!;
		}
	}

	private XElement? SaveContainerFill() => _unreadableFill is not null ? new(_unreadableFill) : !ContainerOnly ? null :
		new("ContainerFill", new XAttribute("version", 1), new XElement("Litres", _litresFormula),
			_compatibleLiquids.Order().Select(x => new XElement("CompatibleLiquid", x)),
			_bonusPlaneId == 0 ? null : new XElement("BonusPlane", new XAttribute("multiplier", _planeMultiplier), _bonusPlaneId), SaveRecipes());

	internal bool ValidateInvocation(ICharacter caster, IPerceivable target, out string? error)
	{
		error = DefinitionError;
		if (error is not null || !ContainerOnly) return error is null;
		if (!TryPrepareRecipe(caster, out error)) return false;
		if (!TryContainer(caster, target, out _, out error)) return false;
		if (_preparedAmount is not null) return true;
		if (Spell is not MagicSpell { InvocationGrade: not null } native)
		{ error = "Container filling requires a selected-grade native casting invocation."; return false; }
		try
		{
			var conversion = Gameworld.UnitManager.BaseFluidToLitres;
			var litres = LitresExpression!.Evaluate(caster, native.CastingTrait);
			if (_bonusPlaneId != 0 && caster.CurrentPlane().Id == _bonusPlaneId) litres *= _planeMultiplier;
			var amount = litres / conversion;
			if (!double.IsFinite(conversion) || conversion <= 0 || !double.IsFinite(litres) || litres <= 0 || !double.IsFinite(amount) || amount <= 0)
				error = "Liquid quantity and the native fluid conversion must be finite and positive.";
			else _preparedAmount = amount;
		}
		catch (Exception ex) { error = "Liquid quantity could not be prepared: " + ex.Message; }
		return error is null;
	}

	private bool TryContainer(ICharacter caster, IPerceivable target, out ILiquidContainer? container, out string? error)
	{
		container = (target as IGameItem)?.GetItemType<ILiquidContainer>(); error = null;
		if (!ReferenceEquals(caster.Gameworld, Gameworld) || !ReferenceEquals(target.Gameworld, Gameworld) ||
			container is null || !container.OwnsLiquidMixture || !container.IsOpen || !caster.CanReachItem(container.Parent).Truth)
		{ error = "Select an accessible open drink container that owns its liquid."; return false; }
		var volume = container.LiquidMixture?.TotalVolume ?? 0;
		if (!double.IsFinite(container.LiquidCapacity) || container.LiquidCapacity <= 0 || !double.IsFinite(volume) || volume < 0 || volume >= container.LiquidCapacity)
		{ error = "The container has no valid remaining liquid capacity."; return false; }
		if (container.LiquidMixture?.Instances.Any(x =>
			!Allowed(x.OriginLiquid.Id) || !Allowed(x.Liquid.Id)) == true)
		{ error = "The existing liquid is incompatible with this configured creation recipe."; return false; }
		return true;
	}

	private bool Allowed(long id) => id == SelectedLiquid.Id || _compatibleLiquids.Contains(id);

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		if (!ValidateInvocation(caster, target, out error)) return false;
		application = ContainerOnly ? new ContainerFill(this, caster, target, _preparedAmount!.Value, SelectedLiquid) :
			new LegacyLiquidCreation(this, caster, target, outcome, power);
		return true;
	}

	private sealed record LegacyLiquidCreation(CreateLiquidEffect Effect, ICharacter Caster, IPerceivable Target,
		OpposedOutcomeDegree Outcome, SpellPower Power) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Effect.GetOrApplyEffect(Caster, Target, Outcome, Power, parent, []);
	}

	private sealed record ContainerFill(CreateLiquidEffect Effect, ICharacter Caster, IPerceivable Target, double Amount, ILiquid Liquid) : IMagicSpellEffectApplicationOperation
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Apply(parent).Effect!;
		public MagicEffectOperation Apply(IMagicSpellEffectParent parent)
		{
			if (!Effect.TryContainer(Caster, Target, out var container, out var error)) throw new InvalidOperationException(error);
			var before = container!.LiquidMixture?.TotalVolume ?? 0;
			var mixture = new LiquidMixture(Liquid, Math.Min(Amount, container!.LiquidCapacity - (container.LiquidMixture?.TotalVolume ?? 0)), Effect.Gameworld);
			if (container.LiquidMixture is not null && !container.LiquidMixture.CanMerge(mixture))
				throw new InvalidOperationException("The current mixture refuses the configured liquid.");
			container.MergeLiquid(mixture, Caster, "spell");
			var after = container.LiquidMixture?.TotalVolume ?? 0;
			return new(!double.IsFinite(after) ? MagicEffectOperationStatus.Unknown :
				after > before ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
		}
	}

	private bool BuildingCommandContainerFill(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "containerfill":
				var on = command.PopSpeech().ToLowerInvariant();
				if (on is not ("on" or "off")) { actor.OutputHandler.Send("Specify on or off."); return false; }
				ContainerOnly = on == "on"; _fillLoadError = null; _unreadableFill = null; break;
			case "litres":
				var formula = new TraitExpression(command.SafeRemainingArgument, Gameworld);
				if (formula.HasErrors() || formula.NonTraitParameters.Contains("outcome", StringComparer.OrdinalIgnoreCase))
				{ actor.OutputHandler.Send("Specify a valid litres formula determinable before payment."); return false; }
				_litresFormula = command.SafeRemainingArgument; LitresExpression = formula; break;
			case "compatible":
				var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
				if (liquid is null) { actor.OutputHandler.Send("Specify an existing liquid."); return false; }
				if (!_compatibleLiquids.Add(liquid.Id)) _compatibleLiquids.Remove(liquid.Id); break;
			case "bonusplane":
				var name = command.PopSpeech();
				if (name.EqualTo("none")) { _bonusPlaneId = 0; _planeMultiplier = 1; break; }
				var plane = Gameworld.Planes.GetByIdOrName(name);
				if (plane is null || !double.TryParse(command.SafeRemainingArgument, out var multiplier) || !double.IsFinite(multiplier) || multiplier <= 0)
				{ actor.OutputHandler.Send("Specify a plane and a finite positive multiplier, or none."); return false; }
				_bonusPlaneId = plane.Id; _planeMultiplier = multiplier; break;
		}
		_preparedAmount = null;
		Spell.Changed = true; actor.OutputHandler.Send("Container liquid creation configuration updated."); return true;
	}
}
