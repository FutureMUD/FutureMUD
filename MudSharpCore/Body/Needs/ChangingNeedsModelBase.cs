using System;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.Needs;

public abstract class ChangingNeedsModelBase : INeedsModel
{
	public static double StaticRealSecondsToInGameSeconds { get; set; }
	protected double RealSecondsToInGameSeconds => StaticRealSecondsToInGameSeconds;

	public ICharacter Owner { get; init; }

	#region INeedsModel Members

	public NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false)
	{
		var oldStatus = Status;

		// Todo calories
		WaterLitres += fulfiller.WaterLitres;
		FoodSatiatedHours += fulfiller.SatiationPoints;
		DrinkSatiatedHours += fulfiller.ThirstPoints;
		Calories += fulfiller.Calories;
		if (!ignoreDelays && fulfiller.AlcoholLitres > 0.0)
		{
			AlcoholLitres += fulfiller.AlcoholLitres * 0.25;
			var timespan = TimeSpan.FromMinutes(Math.Max(1.0,
				15.0 * (1.0 + FoodSatiatedHours / 12.0) * RealSecondsToInGameSeconds));
			Owner.Body.AddEffect(new DelayedNeedsFulfillment(Owner.Body,
				new NeedFulfiller
				{
					AlcoholLitres = fulfiller.AlcoholLitres * 0.25
				}
			), timespan);
			Owner.Body.AddEffect(new DelayedNeedsFulfillment(Owner.Body,
				new NeedFulfiller
				{
					AlcoholLitres = fulfiller.AlcoholLitres * 0.25
				}
			), timespan + timespan);
			Owner.Body.AddEffect(new DelayedNeedsFulfillment(Owner.Body,
				new NeedFulfiller
				{
					AlcoholLitres = fulfiller.AlcoholLitres * 0.25
				}
			), timespan + timespan + timespan);
		}
		else
		{
			AlcoholLitres += fulfiller.AlcoholLitres;
		}

		NormaliseValues();

		return NeedsChanged(oldStatus, fulfiller.SatiationPoints < 0, fulfiller.ThirstPoints < 0,
			fulfiller.AlcoholLitres > 0);
	}

	protected void NormaliseValues()
	{
		if (FoodSatiatedHours > 16)
		{
			FoodSatiatedHours = 16;
		}

		if (FoodSatiatedHours < -4)
		{
			FoodSatiatedHours = -4;
		}

		if (DrinkSatiatedHours > 8)
		{
			DrinkSatiatedHours = 8;
		}

		if (DrinkSatiatedHours < -4)
		{
			DrinkSatiatedHours = -4;
		}

		if (AlcoholLitres < 0)
		{
			AlcoholLitres = 0;
		}

		if (WaterLitres > Owner.Body.CurrentBloodVolumeLitres / 6.0)
		{
			WaterLitres = Owner.Body.CurrentBloodVolumeLitres / 6.0;
		}
	}

	protected NeedsResult NeedsChanged(NeedsResult oldStatus, bool hungrier, bool thirstier, bool drunker)
	{
		var newStatus = Status;

		// Food Messages
		var foodChanged = (oldStatus & NeedsResult.HungerOnly) != (newStatus & NeedsResult.HungerOnly);
		switch (newStatus & NeedsResult.HungerOnly)
		{
			case NeedsResult.Starving:
				if (foodChanged)
				{
					Owner.Send("You are starving!".ColourBold(Telnet.Red));
				}

				break;
			case NeedsResult.Hungry:
				if (foodChanged)
				{
					Owner.Send(hungrier
						? "You are starting to feel quite hungry.".Colour(Telnet.Red)
						: "You are not starving any more, but still quite hungry.".Colour(Telnet.Red));
				}

				break;
			case NeedsResult.Peckish:
				if (foodChanged)
				{
					Owner.Send(hungrier
						? "You are starting to feel a bit peckish."
						: "You now feel merely a little peckish.");
				}

				break;
			case NeedsResult.Full:
				if (foodChanged)
				{
					Owner.Send(hungrier ? "You no longer feel absolutely stuffed." : "You feel that you are full.");
				}

				break;
			case NeedsResult.AbsolutelyStuffed:
				if (foodChanged)
				{
					Owner.Send("You feel absolutely stuffed!");
				}

				break;
		}

		// Drink Messages
		var drinkChanged = (oldStatus & NeedsResult.ThirstOnly) != (newStatus & NeedsResult.ThirstOnly);
		switch (newStatus & NeedsResult.ThirstOnly)
		{
			case NeedsResult.Parched:
				if (drinkChanged)
				{
					Owner.Send("You are extremely parched!".ColourBold(Telnet.Red));
				}

				break;
			case NeedsResult.Thirsty:
				if (drinkChanged)
				{
					Owner.Send(thirstier
						? "You are starting to feel quite thirsty.".Colour(Telnet.Red)
						: "You are not parched any more, but still quite thirsty.".Colour(Telnet.Red));
				}

				break;
			case NeedsResult.NotThirsty:
				if (drinkChanged)
				{
					Owner.Send(thirstier ? "You no longer feel totally sated." : "You no longer feel thirsty.");
				}

				break;
			case NeedsResult.Sated:
				if (drinkChanged)
				{
					Owner.Send("You feel completely sated!");
				}

				break;
		}

		// Alcohol Messages
		var alcoholChanged = (oldStatus & NeedsResult.DrunkOnly) != (newStatus & NeedsResult.DrunkOnly);
		switch (newStatus & NeedsResult.DrunkOnly)
		{
			case NeedsResult.Sober:
				if (alcoholChanged)
				{
					Owner.Send("You feel as if you are now completely sober.");
				}

				break;
			case NeedsResult.Buzzed:
				if (alcoholChanged)
				{
					Owner.Send(drunker
						? "You are starting to feel pleasantly buzzed."
						: "You no longer feel tipsy and feel only mildly buzzed.");
				}

				break;
			case NeedsResult.Tipsy:
				if (alcoholChanged)
				{
					Owner.Send(drunker
						? "You are starting to feel a little tipsy."
						: "You feel as if you have sobered up to the point where you are no longer drunk.");
				}

				break;
			case NeedsResult.Drunk:
				if (alcoholChanged)
				{
					Owner.Send(drunker
						? "You are starting to feel comfortably drunk."
						: "You feel as if you have sobered up to the point where you are no longer extremely drunk.");
				}

				break;
			case NeedsResult.VeryDrunk:
				if (alcoholChanged)
				{
					Owner.Send(drunker
						? "You are starting to feel extremely drunk."
						: "You feel as if you have sobered up to the point where you are no longer blackout drunk.");
				}

				break;
			case NeedsResult.BlackoutDrunk:
				if (alcoholChanged)
				{
					Owner.Send(drunker
						? "You are now in the range of blackout drunk, and are unlikely to remember much of what is going on."
						: "You feel as if you have sobered up to the point where you are no longer paralytic.");
				}

				break;
			case NeedsResult.Paralytic:
				if (alcoholChanged)
				{
					Owner.Send("You are absolutely paralytic!");
				}

				break;
		}

		return newStatus;
	}

	public NeedsResult Status
	{
		get
		{
			var result = NeedsResult.None;
			if (FoodSatiatedHours >= 12)
			{
				result |= NeedsResult.AbsolutelyStuffed;
			}
			else if (FoodSatiatedHours >= 8)
			{
				result |= NeedsResult.Full;
			}
			else if (FoodSatiatedHours >= 4)
			{
				result |= NeedsResult.Peckish;
			}
			else if (FoodSatiatedHours > 0)
			{
				result |= NeedsResult.Hungry;
			}
			else
			{
				result |= NeedsResult.Starving;
			}

			if (DrinkSatiatedHours >= 6)
			{
				result |= NeedsResult.Sated;
			}
			else if (DrinkSatiatedHours >= 4)
			{
				result |= NeedsResult.NotThirsty;
			}
			else if (DrinkSatiatedHours > 0)
			{
				result |= NeedsResult.Thirsty;
			}
			else
			{
				result |= NeedsResult.Parched;
			}

			var bac = 10.0 * AlcoholLitres / Owner.Body.CurrentBloodVolumeLitres;
			if (bac >= 0.25)
			{
				result |= NeedsResult.Paralytic;
			}
			else if (bac >= 0.16)
			{
				result |= NeedsResult.BlackoutDrunk;
			}
			else if (bac >= 0.12)
			{
				result |= NeedsResult.VeryDrunk;
			}
			else if (bac >= 0.08)
			{
				result |= NeedsResult.Drunk;
			}
			else if (bac >= 0.04)
			{
				result |= NeedsResult.Tipsy;
			}
			else if (bac >= 0.01)
			{
				result |= NeedsResult.Buzzed;
			}
			else
			{
				result |= NeedsResult.Sober;
			}

			return result;
		}
	}

	public abstract void NeedsHeartbeat();

	private double _alcoholLitres;

	public double AlcoholLitres
	{
		get => _alcoholLitres;
		set
		{
			_alcoholLitres = Math.Max(0.0, value);
			NeedsChanged(Status, false, false, false);
		}
	}

	public double WaterLitres { get; protected set; }

	public double FoodSatiatedHours { get; protected set; }

	public double DrinkSatiatedHours { get; protected set; }

	public double Calories { get; protected set; }

	public virtual bool NeedsSave => true;

	#endregion
}