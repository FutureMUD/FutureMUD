using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Needs;

/// <summary>
///     An active needs model has hunger and thirst, like a PC
/// </summary>
public class ActiveNeedsModel : ChangingNeedsModelBase
{
	public ActiveNeedsModel(MudSharp.Models.Character dbcharacter, ICharacter character)
	{
		Owner = character;
		DrinkSatiatedHours = dbcharacter.DrinkSatiatedHours;
		FoodSatiatedHours = dbcharacter.FoodSatiatedHours;
		AlcoholLitres = dbcharacter.AlcoholLitres;
		WaterLitres = dbcharacter.WaterLitres;
		SatiationReserve = dbcharacter.SatiationReserve;
	}

	public ActiveNeedsModel(ICharacter character)
	{
		Owner = character;
		DrinkSatiatedHours = 24;
		FoodSatiatedHours = 24;
		AlcoholLitres = 0;
		WaterLitres = 3.0;
		SatiationReserve = 0.0;
	}

	public override void NeedsHeartbeat()
	{
		var ownerMerits = Owner.Merits.OfType<INeedRateChangingMerit>().Where(x => x.Applies(Owner)).ToList();
		var effects = Owner.CombinedEffectsOfType<INeedRateEffect>().Where(x => x.AppliesToPassive).ToList();
		var hungerMult = effects.Aggregate(1.0, (x, y) => x * y.HungerMultiplier);
		var thirstMult = effects.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier);
		var drunkMult = effects.Aggregate(1.0, (x, y) => x * y.DrunkennessMultiplier);
		var oldStatus = Status;
		var hoursPassed = 1 / 60.0 * RealSecondsToInGameSeconds;

		DrinkSatiatedHours -= hoursPassed * Owner.Race.ThirstRate *
		                      ownerMerits.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier) * thirstMult;
		FoodSatiatedHours -= hoursPassed * Owner.Race.HungerRate *
		                     ownerMerits.Aggregate(1.0, (x, y) => x * y.HungerMultiplier) * hungerMult;
		AlcoholLitres -= hoursPassed * Owner.Body.LiverAlcoholRemovalKilogramsPerHour *
		                 ownerMerits.Aggregate(1.0, (x, y) => x * y.DrunkennessMultiplier) * drunkMult;
		WaterLitres -= hoursPassed * Owner.Body.WaterLossLitresPerHour *
		               ownerMerits.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier) * thirstMult;

		var satiationUse = hoursPassed * Owner.Race.HungerRate *
		                   ownerMerits.Aggregate(1.0, (x, y) => x * y.HungerMultiplier) * hungerMult;
		SpendSatiationReserve(satiationUse, false);

		var starvationMultiplier = GetStarvationSatiationDeficitMultiplier(StarvationLevel);
		if (starvationMultiplier > 0.0)
		{
			SpendSatiationReserve(satiationUse * starvationMultiplier, true);
		}

		var exertionMultiplier = GetExertionSatiationBurnMultiplier(Owner.Body.LongtermExertion);
		if (exertionMultiplier > 0.0)
		{
			SpendSatiationReserve(satiationUse * exertionMultiplier, false);
		}

		NeedsChanged(oldStatus, true, true, false);
	}
}
