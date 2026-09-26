using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health.Breathing;
using MudSharp.Health;
using MudSharp.Effects.Interfaces;

#nullable enable

namespace MudSharp.Form.Material;

/// <summary>Read-only hypothetical evaluation. Never starts or settles the active exposure clock.</summary>
public static class ExposureDiagnostic
{
	public static string Describe(ICharacter viewer, IPerceivable target, IFluid source, ExposureRoute route,
		ExposureSourceKind kind, double seconds, double volume)
	{
		var world = viewer.Gameworld;
		var options = EnvironmentalExposureOptions.Read(world);
		var body = (target as ICharacter)?.Body ?? target as IBody;
		var subject = body ?? target;
		var cell = subject.Location;
		var temperature = cell?.CurrentTemperature(null) ?? 20;
		var replenishing = kind == ExposureSourceKind.Immersion || source is IGas;
		var sb = new StringBuilder($"Mode: {options.Mode}; source: {source.Name}; route: {route}; context: {kind}; layer: {target.RoomLayer.DescribeEnum()}.\n");
		sb.AppendLine($"Hypothetical {seconds.ToString("N3", viewer)} seconds; {(replenishing ? "replenishing source" : $"finite source of {volume.ToString("N6", viewer)} base fluid units")}. No live source is debited.");
		if (options.Mode != EnvironmentalExposureMode.Enabled) sb.AppendLine(options.Mode == EnvironmentalExposureMode.Legacy ? "Continuous v2 damage is inactive. Only preserved/unconverted v1 finite-contact coefficients apply in Legacy." : "All environmental exposure damage and consumption are disabled.");
		if (!EnvironmentalExposureService.Physical(body?.Actor ?? target)) return sb.AppendLine("No physical contact: suspended contact or another plane.").ToString();
		if (!options.Allows(subject, route)) sb.AppendLine("The selected route/target is disabled.");
		IReadOnlyList<ExposurePatch> patches;
		if (body is not null)
		{
			if (route == ExposureRoute.Inhalation)
			{
				sb.AppendLine($"Effective breathing requirement: {body.NeedsToBreathe.ToColouredString()}; selected body: {ReferenceEquals(body.Actor.CurrentBody, body).ToColouredString()}; held breath: {body.HeldBreathTime.Describe(viewer)}.");
				sb.AppendLine($"Actual breathing fluid: {body.BreathingStrategy.BreathingFluid(body)?.Name ?? "none"}.");
				if (body.HeldBreathTime > TimeSpan.Zero) sb.AppendLine("Held breath suppresses the next unbreathable-fluid sample; this is a hypothetical inhaled dose, not proof of current inhalation.");
				if (!body.NeedsToBreathe) sb.AppendLine("Effective breathing is exempt: no automatic respiratory sample is delivered.");
				if (!body.BreathingStrategy.NeedsToBreathe) return sb.AppendLine("Non-breather: no respiratory exposure.").ToString();
				if (!BreathingStrategyHelper.HasAirflow(body)) return sb.AppendLine("No airflow: breath is stopped or the respiratory route is not functioning.").ToString();
				if (BreathingStrategyHelper.HasWorkingSupply(body)) sb.AppendLine("Working supplied air: the actual breathing sample comes from the apparatus; this surrounding-fluid prediction is not delivered through that supply.");
				var parts = EnvironmentalExposureService.RespiratoryParts(body);
				patches = parts.Select(p => new ExposurePatch(body, p, body.GetMaterial(p), 1.0 / Math.Max(1, parts.Length), 1)).ToArray();
			}
			else patches = ExposureTransport.ExternalPatches(body, kind == ExposureSourceKind.Immersion ? EnvironmentalExposureService.ImmersedParts(body) : body.Bodyparts.OfType<IExternalBodypart>(), route);
		}
		else if (target is IGameItem item)
		{
			if (route == ExposureRoute.Inhalation) return sb.AppendLine("Items and remains do not inhale.").ToString();
			patches = ExposureTransport.ItemPatches(item, route);
			if (item.GetItemType<IDestroyable>() is null && item.GetItemType<ICorpse>() is null) sb.AppendLine("No destroyable component: ordinary item health processing does not commit wounds.");
			if (item.GetItemType<IOpenable>()?.IsOpen == false) sb.AppendLine($"Closed enclosure: liquid/gas transmission {ExposureTransport.Transmission(item, ExposureRoute.LiquidContact):P0}/{ExposureTransport.Transmission(item, ExposureRoute.GasContact):P0}; susceptibility remains separate.");
			if (kind == ExposureSourceKind.ContainerInterior) sb.AppendLine("Interior prediction uses the supplied finite test volume; inspect the vessel's owned contents separately before applying it.");
		}
		else return sb.AppendLine("This target has no supported exposure surface.").ToString();
		foreach (var patch in patches.OrderBy(x => x.LayerDepth))
		{
			sb.AppendLine($"Depth {patch.LayerDepth}: {patch.Target.Name} / {patch.Part?.FullDescription() ?? "item surface"}; material {patch.Material.Name}; area {patch.Area:N4}; transmission {patch.Transmission:P2}.");
			if (patch.Transmission <= 0) sb.AppendLine("   A closed barrier excludes this inner surface.");
			var rules = EnvironmentalExposureResolver.SelectRules(source, patch.Material, route, temperature, out var errors);
			foreach (var error in errors) sb.AppendLine("   " + error);
			foreach (var rule in rules) sb.AppendLine($"   {rule.Name} [{rule.Id}]: {(rule.NoReaction ? "explicit exclusion" : rule.Channel)}; {rule.Consumption}; {(rule.ApplicabilityProg is null && rule.IntensityProg is null ? "unconditional" : "conditional progs not executed by diagnostic")}.");
			if (rules.Count == 0) sb.AppendLine("   No applicable v2 material/range rule.");
		}
		if (patches.Count == 0) return sb.AppendLine("No exposed parts under the selected source context.").ToString();
		var resolver = new EnvironmentalExposureResolver(world);
		var results = source is ILiquid liquid
			? resolver.Liquid(new LiquidMixture(liquid, volume, world), replenishing, patches, kind, "diagnostic", seconds, temperature, route, dryRun: true)
			: resolver.Gas(source, patches, route, kind, "diagnostic", 1, seconds, temperature, dryRun: true);
		foreach (var result in results)
		{
			var context = new ExposureDamageContext(route, kind, "diagnostic", result.Reaction.Category, result.Reaction.Channel, result.Reaction.Id, seconds);
			var multiplier = EnvironmentalExposureResolver.ResistanceMultiplier(result.Patch, context, dryRun: true);
			var predicted = PredictNormalModifiers(result, context, multiplier);
			sb.AppendLine($"{result.Patch.Target.Name}/{result.Patch.Part?.Name ?? "surface"}: work {result.Work:N6}; raw damage {result.RawDamage:N4}; after exposure resistance {result.RawDamage * multiplier:N4}; after normal modifiers {Math.Max(0, predicted?.DamageAmount ?? 0):N4} damage / {Math.Max(0, predicted?.PainAmount ?? 0):N4} pain / {Math.Max(0, predicted?.StunAmount ?? 0):N4} stun; consumption {result.Consumed:N6} base units.");
		}
		sb.AppendLine("Worn transmission is included once. Normal-modifier prediction evaluates current armour formulas without spending capacity or creating wounds; conditional effect progs are skipped. Wound caps, severity and changes during the hypothetical interval can lower committed injury.");
		foreach (var patch in patches)
			if (patch.Material is ISolid material) sb.AppendLine($"{material.Name}: heat threshold {material.HeatDamagePoint?.ToString(viewer) ?? "absent (no ambient injury)"}; ambient rate {ExposureArithmetic.ThermalRate(temperature, material.HeatDamagePoint, material.ExposureProperties.ThermalSlope ?? options.HeatSlope, material.ExposureProperties.ThermalCap ?? options.HeatCap):N4}/s before area/transmission.{(material.ExposureProperties.ThermalIntensityProgId is { } id ? $" Conditional intensity prog #{id} is not executed; effective injury is undetermined." : "")}");
		return sb.ToString();
	}

	public static IDamage? PredictNormalModifiers(ExposureResolution result, ExposureDamageContext context, double multiplier)
	{
		var patch = result.Patch;
		using var random = EnvironmentalExposureResolver.DamageRandom(patch, context);
		IDamage? damage = new Damage { ExposureContext = context, TargetBody = patch.TargetBody, Bodypart = patch.Part,
			DamageType = result.Reaction.DamageType, DamageAmount = result.RawDamage * multiplier, PainAmount = result.Pain * multiplier, StunAmount = result.Stun * multiplier };
		if (patch.TargetBody is { } body)
		{
			foreach (var armour in body.CombinedEffectsOfType<IMagicArmour>().Where(x => x.ApplicabilityProg is null && x.AppliesToPart(patch.Part!)))
			{
				damage = armour.PreviewDamage(damage);
				if (damage is null) return null;
			}
			var wounds = new List<IWound>();
			if (patch.Part is IExternalBodypart && body.Race.NaturalArmourType is { } racial)
				damage = racial.AbsorbDamage(damage, body.Race.NaturalArmourQuality, body.Race.NaturalArmourMaterial, body.Actor, ref wounds).PassThroughDamage;
			if (damage is not null && patch.Part?.NaturalArmourType is { } natural)
				damage = natural.AbsorbDamage(damage, body.Race.NaturalArmourQuality, patch.Material, body.Actor, ref wounds).SufferedDamage;
		}
		else if (patch.Target is IGameItem item)
		{
			if (item.GetItemType<IDestroyable>() is not { } destroyable) return null;
			var seconds = context.ReferenceSeconds;
			foreach (var armour in item.EffectsOfType<IMagicArmourEnhancementEffect>().Where(x => x.ApplicabilityProg is null))
			{
				var reduction = Math.Max(0, armour.ArmourDamageReduction) * seconds;
				damage = new Damage(damage) { DamageAmount = Math.Max(0, damage.DamageAmount - reduction),
					PainAmount = Math.Max(0, damage.PainAmount - reduction), StunAmount = Math.Max(0, damage.StunAmount - reduction) };
			}
			damage = destroyable.GetActualDamage(damage);
		}
		return damage;
	}
}
