#nullable enable

using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.Form.Material;

namespace MudSharp.Health.Breathing;

internal static class BreathingStrategyHelper
{
	public static void ExposeToMagic(IBody body, IFluid? fluid, bool supplied = false)
	{
		if (fluid is null || !HasAirflow(body)) { EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body); return; }
		EnvironmentalExposureService.For(body.Gameworld).Inhaled(body, fluid, supplied);
		if (fluid is IGas gas) MudSharp.Magic.MagicalExposure.Carrier(body, MudSharp.Magic.SubstanceCarrier.Gas,
			gas.Id, body.Race.BreathingRate(body, gas), DrugVector.Inhaled);
	}

	public static bool HasWorkingSupply(IBody body) => body.Bodyparts.OfType<MouthProto>()
		.SelectMany(body.WornItemsFor).Select(x => x.GetItemType<IProvideGasForBreathing>()).OfType<IProvideGasForBreathing>()
		.Any(x => x.Gas is not null && (body.Location?.IsUnderwaterLayer(body.RoomLayer) != true || x.WaterTight) &&
			x.CanConsumeGas(body.Race.BreathingRate(body, x.Gas!)));

	public static void UnbreathableSample(IBody body, IFluid? fluid)
	{
		if (fluid is null || body.HeldBreathTime > TimeSpan.Zero || !HasAirflow(body) || HasWorkingSupply(body))
		{ EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body); return; }
		EnvironmentalExposureService.For(body.Gameworld).Inhaled(body, fluid);
	}

	public static bool HasAirflow(IBody body)
	{
		if (!body.BreathingStrategy.NeedsToBreathe || body.CombinedEffectsOfType<IStopBreathing>().Any(x => x.Applies()) ||
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
		if (fluid is null) return false;
		if (body.Race.CanBreatheFluid(fluid).Truth) return true;
		return body.CombinedEffectsOfType<IAdditionalBreathableFluidEffect>().Any(x =>
			(includeLegacyMagic || x is MudSharp.Effects.Concrete.SpellEffects.SpellScopedWaterBreathingEffect) &&
			x.Applies() && x.AppliesToFluid(fluid));
	}
}
