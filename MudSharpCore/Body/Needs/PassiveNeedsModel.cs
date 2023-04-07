using MudSharp.Character;

namespace MudSharp.Body.Needs;

/// <summary>
///     A passive needs model does respond to explicit changes but doesn't naturally tick down. It would normally be used
///     by NPCs who are susceptible to such effects but don't generally need to worry about it.
/// </summary>
public class PassiveNeedsModel : ChangingNeedsModelBase
{
	public PassiveNeedsModel(ICharacter character)
	{
		Owner = character;
		AlcoholLitres = 0.0;
		WaterLitres = 0.0;
		FoodSatiatedHours = 16.0;
		DrinkSatiatedHours = 8.0;
		Calories = 0.0;
	}

	public override bool NeedsSave => false;

	public override void NeedsHeartbeat()
	{
		// Do nothing
	}
}