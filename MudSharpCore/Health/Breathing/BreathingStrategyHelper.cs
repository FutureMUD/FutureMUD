#nullable enable

using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.Form.Material;

namespace MudSharp.Health.Breathing;

internal static class BreathingStrategyHelper
{
	public static bool IsStoppedByEffect(IBody body) =>
		body.CombinedEffectsOfType<IStopBreathing>().Any(x => x.Applies());

	public static double BreathingVolume(IBody body, IFluid fluid, TimeSpan elapsed) =>
		body.Race.BreathingRate(body, fluid) * elapsed.TotalMinutes / body.Gameworld.UnitManager.BaseFluidToLitres;

	public static void ExposeToMagic(IBody body, IFluid? fluid, TimeSpan elapsed, bool supplied = false)
	{
		if (fluid is null || !HasAirflow(body)) { EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body); return; }
		EnvironmentalExposureService.For(body.Gameworld).Inhaled(body, fluid, supplied);
		if (fluid is IGas gas) MudSharp.Magic.MagicalExposure.Carrier(body, MudSharp.Magic.SubstanceCarrier.Gas,
			gas.Id, BreathingVolume(body, gas, elapsed), DrugVector.Inhaled);
	}

	public static bool HasWorkingSupply(IBody body, TimeSpan elapsed) => body.Bodyparts.OfType<MouthProto>()
		.Any(mouth => body.WornItemsFor(mouth).Select(x => x.GetItemType<IProvideGasForBreathing>())
			.OfType<IProvideGasForBreathing>().Any(x => x.Gas is not null &&
				(!TerrainImmersionHelper.IsPartImmersed(body, mouth) || x.WaterTight) &&
				x.CanConsumeGas(BreathingVolume(body, x.Gas!, elapsed))));

	public static void UnbreathableSample(IBody body, IFluid? fluid, TimeSpan elapsed)
	{
		if (fluid is null || body.HeldBreathTime > TimeSpan.Zero || !HasAirflow(body) || HasWorkingSupply(body, elapsed))
		{ EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body); return; }
		EnvironmentalExposureService.For(body.Gameworld).Inhaled(body, fluid);
	}

	public static bool HasAirflow(IBody body)
	{
		if (!body.BreathingStrategy.NeedsToBreathe || IsStoppedByEffect(body) ||
			body.EffectsOfType<Anesthesia>().Sum(x => x.IntensityPerGramMass) >= 5 * body.RespirationBreathingDriveMultiplier()) return false;
		if (body.BreathingStrategy.Name == "partless") return true;
		if (body.OrganFunction<HeartProto>() <= 0) return false;
		bool Working<T>() => body.Bodyparts.OfType<T>().OfType<IBodypart>().Any(p =>
			!body.CombinedEffectsOfType<IBodypartIneffectiveEffect>().Any(x => x.Bodypart == p));
		if (body.BreathingStrategy.Name == "gills") return Working<GillProto>();
		if (body.BreathingStrategy.Name == "blowhole" ? !Working<BlowholeProto>() : !Working<MouthProto>()) return false;
		var tolerance = body.RespirationAirwayToleranceMultiplier();
		return body.OrganFunction<LungProto>() >= 0.5 / tolerance && body.OrganFunction<TracheaProto>() > 0 &&
			body.EffectsOfType<IInternalBleedingEffect>().Where(x => x.Organ is LungProto or TracheaProto).Sum(x => x.BloodlossTotal) <= 0.3 * tolerance;
	}

	public static bool CanBreatheFluid(IBody body, IFluid? fluid, bool includeLegacyMagic = false)
	{
		if (includeLegacyMagic)
		{
			// Preserve the lung consumer's original evaluation order, including its
			// legacy applicability callbacks even when racial compatibility succeeds.
			var additional = body.CombinedEffectsOfType<IAdditionalBreathableFluidEffect>()
				.Any(x => x.Applies() && x.AppliesToFluid(fluid!));
			return (fluid is not null && body.Race.CanBreatheFluid(fluid).Truth) || additional;
		}
		if (fluid is null) return false;
		if (body.Race.CanBreatheFluid(fluid).Truth) return true;
		return body.CombinedEffectsOfType<IAdditionalBreathableFluidEffect>().Any(x =>
			x is MudSharp.Effects.Concrete.SpellEffects.SpellScopedWaterBreathingEffect &&
			x.Applies() && x.AppliesToFluid(fluid));
	}
}
