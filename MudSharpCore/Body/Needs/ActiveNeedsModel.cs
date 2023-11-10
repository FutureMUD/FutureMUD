using System.Linq;
using MudSharp.Character;
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
		Calories = dbcharacter.Calories;
	}

	public ActiveNeedsModel(ICharacter character)
	{
		Owner = character;
		DrinkSatiatedHours = 24;
		FoodSatiatedHours = 24;
		AlcoholLitres = 0;
		WaterLitres = 3.0;
		Calories = 5000;
	}

	public override void NeedsHeartbeat()
	{
		var ownerMerits = Owner.Merits.OfType<INeedRateChangingMerit>().Where(x => x.Applies(Owner)).ToList();
		var oldStatus = Status;
		var hoursPassed = 1 / 60.0 * RealSecondsToInGameSeconds;
		DrinkSatiatedHours -= hoursPassed * Owner.Race.ThirstRate * ownerMerits.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier);
		FoodSatiatedHours -= hoursPassed * Owner.Race.HungerRate * ownerMerits.Aggregate(1.0, (x, y) => x * y.HungerMultiplier);
		AlcoholLitres -= hoursPassed * Owner.Body.LiverAlcoholRemovalKilogramsPerHour *
		                 ownerMerits.Aggregate(1.0, (x, y) => x * y.DrunkennessMultiplier);
		// 1 Litre of Alcohol ~= 1 KG of Alcohol
		WaterLitres -= hoursPassed * Owner.Body.WaterLossLitresPerHour *
		               ownerMerits.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier);
		Calories -= hoursPassed * Owner.Body.CaloricConsumptionPerHour *
		            ownerMerits.Aggregate(1.0, (x, y) => x * y.HungerMultiplier);
		NeedsChanged(oldStatus, true, true, false);
	}
}