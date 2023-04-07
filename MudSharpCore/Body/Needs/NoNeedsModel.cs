namespace MudSharp.Body.Needs;

/// <summary>
///     A NoNeedsModel is a kind of Needs Model that simply does not respond to changes in needs. This would be used by,
///     for example, Undead/Constructs etc.
/// </summary>
public class NoNeedsModel : INeedsModel
{
	#region INeedsModel Members

	public NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false)
	{
		return NeedsResult.None;
	}

	public void NeedsHeartbeat()
	{
		// Do nothing
	}

	public double AlcoholLitres
	{
		get => 0;
		set { }
	}

	public double WaterLitres => 0.0;

	public double FoodSatiatedHours => double.MaxValue;

	public double DrinkSatiatedHours => double.MaxValue;

	public double Calories => 0.0;

	public bool NeedsSave => false;

	public NeedsResult Status => NeedsResult.AbsolutelyStuffed | NeedsResult.Sated | NeedsResult.Sober;

	#endregion
}