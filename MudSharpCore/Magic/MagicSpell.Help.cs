using MudSharp.Body.Traits;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	private void AppendCastingCostHelp(ICharacter actor, StringBuilder sb, IVancianMagicCapability? selectedCapability)
	{
		if (_castingCosts.Count == 0 || Trigger is not ICastMagicTrigger trigger) return;
		sb.AppendLine();
		var capabilities = actor.Capabilities.OfType<IVancianMagicCapability>()
			.Where(x => x.School.Id == School.Id && (selectedCapability is null || x.Id == selectedCapability.Id))
			.DistinctBy(x => x.Id).ToArray();
		if (selectedCapability is null && (HasLegacyRoute(actor) || capabilities.Length == 0))
		{
			sb.AppendLine("Legacy Casting Costs:");
			foreach (var power in Enum.GetValues<SpellPower>().Where(x => x >= trigger.MinimumPower && x <= trigger.MaximumPower))
				sb.AppendLine($"\t{power.DescribeEnum().ColourName()}: {_castingCosts.Select(x => $"{x.Value.EvaluateWith(actor, CastingTrait, TraitBonusContext.SpellCost, ("self", 0), ("power", (int)power)).ToString("G", actor)} {x.Key.ShortName}".ColourValue()).ListToString()}");
		}
		if (capabilities.Length == 0) return;
		sb.AppendLine("Vancian Casting Costs:");
		var service = VancianMagicService.For(Gameworld);
		foreach (var capability in capabilities)
		{
			var shown = false;
			foreach (var allowance in capability.Allowances)
				foreach (var rule in capability.Repertoires.Where(x => allowance.RepertoireKeys.Contains(x.Key)))
				{
					if (!service.Candidates(actor, capability, rule.Key).Contains(this)) continue;
					shown = true;
					var route = service.CanCast(actor, capability, rule.Key, allowance.Key, this);
					var label = $"{capability.Name} {rule.Alias}/{allowance.Alias}";
					if (!route.Available)
					{
						sb.AppendLine($"\t{label.ColourName()}: {route.Reason} Costs unresolved: {_castingCosts.Select(x => $"{x.Value.OriginalFormulaText} ({x.Key.ShortName})").ListToString()}");
						continue;
					}
					try
					{
						var numbers = new SpellNumericalContext(SpellLevel, route.CastingLevel, service.CasterLevel(actor, capability), route.Power, capability.ReliableOutcome, false);
						string Costs(bool self) => _castingCosts.Select(x =>
						{
							var cost = numbers.Evaluate($"cost/{x.Key.Id}", x.Value, actor, CastingTrait, TraitBonusContext.SpellCost, [("self", self ? 1 : 0)]);
							if (!double.IsFinite(cost) || cost < 0) throw new InvalidOperationException("Invalid resource cost.");
							return $"{cost.ToString("G", actor)} {x.Key.ShortName}";
						}).ListToString();
						var costText = Trigger is CastingTriggerSelf ? Costs(true) : $"other target: {Costs(false)}; self: {Costs(true)}";
						sb.AppendLine($"\t{label.ColourName()}: {(allowance.Mode == VancianAllowanceMode.AtWill ? "at-will" : $"slot {route.Ordinal?.ToString("N0", actor)}")}, level {route.CastingLevel.ToString("N0", actor)}, {route.Power.DescribeEnum().ColourName()}: {costText.ColourValue()}");
					}
					catch (Exception ex) { sb.AppendLine($"\t{label.ColourName()}: costs unresolved: {ex.Message.ColourError()}"); }
				}
			if (!shown) sb.AppendLine($"\t{capability.Name.ColourName()}: no eligible casting route; costs unresolved.");
		}
	}
}
