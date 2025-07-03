using System;
using System.Linq;
using System.Text;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;

namespace MudSharp.Body.Implementations;

public partial class Body : IHaveNeeds, IEat
{
	private bool _needsChanged;
	private int _needsChangedCount;

	public bool NeedsChanged
	{
		get => _needsChanged;
		set
		{
			if (!_noSave)
			{
				if (_needsChangedCount < (int)(Constants.Random.NextDouble() * 5))
				{
					//We only save HeartBeat related needs change at random intervals of 1 to 5 minutes
					//in order to avoid hammering the save manager all at once.
					_needsChangedCount++;
					return;
				}

				_needsChangedCount = 0;
				if (value && !_needsChanged)
				{
					Changed = true;
				}

				_needsChanged = value;
			}
		}
	}

	#region IHaveNeeds Members

	public INeedsModel NeedsModel => Actor.NeedsModel;

	public NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false)
	{
		if (NeedsModel.NeedsSave)
		{
			NeedsChanged = true;
			if (!Changed)
				//Unlike Heartbeat adjustments to needs, we always want to save when needs have changed
				//due to a FulfilNeeds call.
			{
				Changed = true;
			}
		}

		return NeedsModel.FulfilNeeds(fulfiller, ignoreDelays);
	}

	public void NeedsHeartbeat()
	{
		if (NeedsModel.NeedsSave)
		{
			NeedsChanged = true;
		}

		NeedsModel.NeedsHeartbeat();
	}

	public void StartNeedsHeartbeat()
	{
		// Do nothing
	}

	public void DescribeNeedsResult(NeedsResult result)
	{
		var sb = new StringBuilder();
		if (result.HasFlag(NeedsResult.Starving))
		{
			sb.AppendLine("You are starving!");
		}
		else if (result.HasFlag(NeedsResult.Hungry))
		{
			sb.AppendLine("You are feeling hungry.");
		}
		else if (result.HasFlag(NeedsResult.Peckish))
		{
			sb.AppendLine("You are feeling peckish.");
		}
		else if (result.HasFlag(NeedsResult.Full))
		{
			sb.AppendLine("You are comfortably full.");
		}
		else if (result.HasFlag(NeedsResult.AbsolutelyStuffed))
		{
			sb.AppendLine("You are absolutely stuffed.");
		}

		if (result.HasFlag(NeedsResult.Parched))
		{
			sb.AppendLine("Your throat is parched!");
		}
		else if (result.HasFlag(NeedsResult.Thirsty))
		{
			sb.AppendLine("You are thirsty.");
		}
		else if (result.HasFlag(NeedsResult.NotThirsty))
		{
			sb.AppendLine("You are not thirsty.");
		}
		else if (result.HasFlag(NeedsResult.Sated))
		{
			sb.AppendLine("You feel completely sated.");
		}

		if (result.HasFlag(NeedsResult.Sober))
		{
			sb.AppendLine("You are feeling perfectly sober.");
		}
		else if (result.HasFlag(NeedsResult.Buzzed))
		{
			sb.AppendLine("You are feeling a little buzzed.");
		}
		else if (result.HasFlag(NeedsResult.Tipsy))
		{
			sb.AppendLine("You are feeling a little tipsy.");
		}
		else if (result.HasFlag(NeedsResult.Drunk))
		{
			sb.AppendLine("You are feeling drunk.");
		}
		else if (result.HasFlag(NeedsResult.VeryDrunk))
		{
			sb.AppendLine("You are feeling very drunk.");
		}
		else if (result.HasFlag(NeedsResult.BlackoutDrunk))
		{
			sb.AppendLine("You are so drunk that you are unlikely to recall anything that is happening right now.");
		}
		else if (result.HasFlag(NeedsResult.Paralytic))
		{
			sb.AppendLine("You are blind, stinking drunk.");
		}

		if (sb.Length > 0)
		{
			OutputHandler.Send(sb.ToString());
		}
	}

	#endregion

	#region IEat Members

	private string BiteDescriber(double bites, double remaining)
	{
		if (bites >= remaining && bites == 1)
		{
			return string.Empty;
		}

		if (bites >= remaining)
		{
			return "all of ";
		}

		switch ((int)bites)
		{
			case 1:
				return "a bite of ";
			case 2:
				return "a couple of bites of ";
			case 3:
			case 4:
			case 5:
				return "a few bites of ";
			default:
				return "many bites of ";
		}
	}

	public bool Eat(IEdible edible, IContainer container, ITable table, double bites, IEmote playerEmote)
	{
		if (bites == 0)
		{
			bites = edible.BitesRemaining;
		}

		if (!CanEat(edible, container, table, bites))
		{
			Actor.Send(WhyCannotEat(edible, container, table, bites));
			return false;
		}

		// If the item is a stackable, split one off
		if (edible.Parent.IsItemType<IStackable>() && edible.Parent.GetItemType<IStackable>().Quantity > 1)
		{
			Actor.Send("You must first take a single item out of the stack before you can eat any of them.");
			return false;
		}

		// TODO - poison effects from eating

		Actor.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
			$"@ eat|eats {BiteDescriber(bites, edible.BitesRemaining)}$0$?1| from $1||$$?2| on $2||$", Actor,
			edible.Parent, container?.Parent,
			table?.Parent), flags: OutputFlags.SuppressObscured).Append(playerEmote));
		if (!string.IsNullOrEmpty(edible.TasteString))
		{
			Actor.Send(edible.TasteString.Fullstop().Colour(Telnet.Yellow)); // TODO - poison/magic taste modifiers
		}

		FulfilNeeds(new NeedFulfiller
		{
			SatiationPoints = edible.SatiationPoints * bites / edible.TotalBites,
			Calories = edible.Calories * bites / edible.TotalBites,
			ThirstPoints = edible.ThirstPoints * bites / edible.TotalBites,
			WaterLitres = edible.WaterLitres * bites / edible.TotalBites,
			AlcoholLitres = edible.AlcoholLitres * bites / edible.TotalBites
		});

		#region Event Handling

		var bitesLeft = edible.BitesRemaining - bites;
		Actor.HandleEvent(EventType.CharacterEat, Actor, edible.Parent, bites, bitesLeft);
		edible.Parent.HandleEvent(EventType.ItemEaten, Actor, edible.Parent, bites, bitesLeft);
		foreach (var witness in Actor.Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, edible.Parent, bites, bitesLeft);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, edible.Parent, bites, bitesLeft);
		}

		#endregion

                edible.Eat(this, bites);
                CrimeExtensions.CheckPossibleCrimeAllAuthorities(Actor, CrimeTypes.IllegalConsumption, null, edible.Parent, "");
                CrimeExtensions.CheckPossibleCrimeAllAuthorities(Actor, CrimeTypes.PublicIntoxication, null, edible.Parent, "");
                return true;
        }

	public bool SilentEat(IEdible edible, double bites)
	{
		// Require a working esophagus to eat
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return false;
		}

		if (bites == 0)
		{
			bites = edible.BitesRemaining;
		}

		bites = Math.Min(bites, edible.BitesRemaining);
		FulfilNeeds(new NeedFulfiller
		{
			SatiationPoints = edible.SatiationPoints * bites / edible.TotalBites,
			Calories = edible.Calories * bites / edible.TotalBites,
			ThirstPoints = edible.ThirstPoints * bites / edible.TotalBites,
			WaterLitres = edible.WaterLitres * bites / edible.TotalBites,
			AlcoholLitres = edible.AlcoholLitres * bites / edible.TotalBites
		});

		#region Event Handling

		var bitesLeft = edible.BitesRemaining - bites;
		Actor.HandleEvent(EventType.CharacterEat, Actor, edible.Parent, bites, bitesLeft);
		edible.Parent.HandleEvent(EventType.ItemEaten, Actor, edible.Parent, bites, bitesLeft);
		foreach (var witness in Actor.Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, edible.Parent, bites, bitesLeft);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, edible.Parent, bites, bitesLeft);
		}

		#endregion

		edible.Eat(this, bites);
		return true;
	}

	public (bool Success, string ErrorMessage) Eat(ICorpse corpse, double bites, IEmote playerEmote)
	{
		if (bites == 0)
		{
			bites = 10000;
		}

		var result = CanEat(corpse, bites);
		if (!result.Success)
		{
			Actor.OutputHandler.Send(result.ErrorMessage);
			return result;
		}

		if (corpse.RemainingEdibleWeight < bites * Actor.Race.BiteWeight)
		{
			bites = corpse.RemainingEdibleWeight / Actor.Race.BiteWeight;
		}

		var bitesLeft = corpse.RemainingEdibleWeight / Actor.Race.BiteWeight;
		Actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(
				string.Format(Actor.Race.EatCorpseEmoteText, BiteDescriber(bites, bitesLeft)), Actor, Actor,
				corpse.Parent)).Append(playerEmote));
		FulfilNeeds(Actor.Race.GetCorpseNeedFulfill(corpse.Parent.Material, bites));
		corpse.EatenWeight += Actor.Race.BiteWeight * bites;

		bitesLeft -= bites;
		Actor.HandleEvent(EventType.CharacterEat, Actor, corpse.Parent, bites, bitesLeft);
		corpse.Parent.HandleEvent(EventType.ItemEaten, Actor, corpse.Parent, bites, bitesLeft);
		foreach (var witness in Actor.Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, corpse.Parent, bites, bitesLeft);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, corpse.Parent, bites, bitesLeft);
		}

		if (corpse.RemainingEdibleWeight <= 0.0 && Actor.SizeStanding - corpse.OriginalCharacter.SizeStanding >= 3)
		{
			corpse.Parent.Delete();
		}

		return (true, string.Empty);
	}

	public (bool Success, string ErrorMessage) Eat(ISeveredBodypart bodypart, double bites, IEmote playerEmote)
	{
		if (bites == 0)
		{
			bites = 10000;
		}

		var result = CanEat(bodypart, bites);
		if (!result.Success)
		{
			Actor.OutputHandler.Send(result.ErrorMessage);
			return result;
		}

		if (bodypart.RemainingEdibleWeight < bites * Actor.Race.BiteWeight)
		{
			bites = bodypart.RemainingEdibleWeight / Actor.Race.BiteWeight;
		}

		var bitesLeft = bodypart.RemainingEdibleWeight / Actor.Race.BiteWeight;
		Actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(
				string.Format(Actor.Race.EatCorpseEmoteText, BiteDescriber(bites, bitesLeft)), Actor, Actor,
				bodypart.Parent)).Append(playerEmote));
		FulfilNeeds(Actor.Race.GetCorpseNeedFulfill(bodypart.Parent.Material, bites));
		bodypart.EatenWeight += Actor.Race.BiteWeight * bites;

		bitesLeft -= bites;
		Actor.HandleEvent(EventType.CharacterEat, Actor, bodypart.Parent, bites, bitesLeft);
		bodypart.Parent.HandleEvent(EventType.ItemEaten, Actor, bodypart.Parent, bites, bitesLeft);
		foreach (var witness in Actor.Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, bodypart.Parent, bites, bitesLeft);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterEatWitness, Actor, witness, bodypart.Parent, bites, bitesLeft);
		}

		if (bodypart.RemainingEdibleWeight <= 0.0 && Actor.SizeStanding - bodypart.OriginalCharacter.SizeStanding >= 2)
		{
			bodypart.Parent.Delete();
		}

		return (true, string.Empty);
	}

	public (bool Success, string ErrorMessage) Eat(string foragableYield, double bites, IEmote playerEmote)
	{
		if (bites == 0)
		{
			bites = 1000000;
		}

		var result = CanEat(foragableYield, bites);
		if (!result.Success)
		{
			Actor.OutputHandler.Send(result.ErrorMessage);
			return result;
		}

		if (Actor.Location.GetForagableYield(foragableYield) < bites * Actor.Race.BiteYield(foragableYield))
		{
			bites = Actor.Location.GetForagableYield(foragableYield) / Actor.Race.BiteYield(foragableYield);
		}

		var bitesLeft = Actor.Location.GetForagableYield(foragableYield) / Actor.Race.BiteYield(foragableYield);
		var (emote, fulfiler, yieldconsumed) = Actor.Race.EatYield(foragableYield, bites);
		Actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(string.Format(emote, BiteDescriber(bites, bitesLeft)), Actor, Actor))
				.Append(playerEmote));
		FulfilNeeds(fulfiler);
		Actor.Location.ConsumeYield(foragableYield, yieldconsumed);
		bitesLeft -= bites;

		// TODO - AI Events

		return (true, string.Empty);
	}

	public (bool Success, string ErrorMessage) CanEat()
	{
		// Require a working esophagus to eat
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return (false, "You require a working esophagus in order to eat.");
		}

		// TODO - effects that prevent eating
		return (true, string.Empty);
	}

	public bool CanEat(IEdible edible, IContainer container, ITable table, double bites)
	{
		if (!Actor.Race.CanEatFoodMaterial(edible.Parent.Material))
		{
			return false;
		}

		if (bites > edible.BitesRemaining)
		{
			return false;
		}

		if (container != null && !CanGet(edible.Parent, container.Parent, 0))
		{
			return false;
		}

		if (container == null && !HeldOrWieldedItems.Contains(edible.Parent) && !CanGet(edible.Parent, 0))
		{
			return false;
		}

		if (table != null && !Actor.InVicinity(table.Parent))
		{
			return false;
		}

		if (table == null && container != null && !Actor.InVicinity(container.Parent))
		{
			return false;
		}

		return CanEat().Success;
	}

	private string WhyCannotEat(IEdible edible, IContainer container, ITable table, double bites)
	{
		if (!Actor.Race.CanEatFoodMaterial(edible.Parent.Material))
		{
			return
				$"You aren't able to eat food that is made out of {edible.Parent.Material.Name.Colour(edible.Parent.Material.ResidueColour)}.";
		}

		if (bites > edible.BitesRemaining)
		{
			return $"You cannot eat {edible.Parent.HowSeen(Actor)} as there just isn't that much of it to eat.";
		}

		if (container != null && !CanGet(edible.Parent, container.Parent, 0))
		{
			return WhyCannotGet(edible.Parent, container.Parent, 0);
		}

		if (container == null && !HeldOrWieldedItems.Contains(edible.Parent) && !CanGet(edible.Parent, 0))
		{
			return WhyCannotGet(edible.Parent, 0);
		}

		if (table != null && !Actor.InVicinity(table.Parent))
		{
			return
				$"You must be in the vicinity of {table.Parent.HowSeen(Actor)} in order to eat anything from it.";
		}

		if (table == null && container != null && !Actor.InVicinity(container.Parent))
		{
			return
				$"You must be in the vicinity of {container.Parent.HowSeen(Actor)} in order to eat anything from it.";
		}

		return CanEat().ErrorMessage;
	}

	public (bool Success, string ErrorMessage) CanEat(ICorpse corpse, double bites)
	{
		if (!Actor.Race.CanEatCorpses)
		{
			return (false,
				"You are not savage enough to eat corpses without any preparation; you must first make at least a rudimentary effort to butcher the corpse.");
		}

		if (!Actor.Race.CanEatCorpseMaterial(corpse.Parent.Material))
		{
			return (false,
				$"You aren't able to digest {corpse.Parent.Material.Name.Colour(corpse.Parent.Material.ResidueColour)}.");
		}

		return CanEat();
	}

	public (bool Success, string ErrorMessage) CanEat(ISeveredBodypart bodypart, double bites)
	{
		if (!Actor.Race.CanEatCorpses)
		{
			return (false,
				"You are not savage enough to eat severed bodyparts without any preparation; you must first make at least a rudimentary effort to butcher the part.");
		}

		if (!Actor.Race.CanEatCorpseMaterial(bodypart.Parent.Material))
		{
			return (false,
				$"You aren't able to digest {bodypart.Parent.Material.Name.Colour(bodypart.Parent.Material.ResidueColour)}.");
		}

		return CanEat();
	}

	public (bool Success, string ErrorMessage) CanEat(string foragableYield, double bites)
	{
		if (!Actor.Race.CanEatForagableYield(foragableYield))
		{
			return (false,
				$"You are unable to gain any nutrition from eating the foragable type {foragableYield.ToLowerInvariant().Colour(Telnet.Green)} directly.");
		}

		if (Actor.Location.GetForagableYield(foragableYield) <= 0.0)
		{
			return (false,
				$"There isn't enough of the foragable type {foragableYield.ToLowerInvariant().Colour(Telnet.Green)} for you to eat here. Try another location.");
		}

		return CanEat();
	}

	public bool SilentDrink(ILiquidContainer container, double quantity)
	{
		// Require a working esophagus to drink
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return false;
		}

		Actor.Send(container.LiquidMixture.TasteString(Actor).Fullstop().Colour(Telnet.Yellow));

		var sip = new LiquidMixture(container.LiquidMixture, quantity);
		FulfilNeeds(sip.GetNeedFulfiller());

		sip.OnDraught(Actor, container);
		container.ReduceLiquidQuantity(quantity, Actor, "silentdrink");

		return true;
	}

	public bool Drink(ILiquidContainer container, ITable table, double quantity, IEmote playerEmote)
	{
		if (!CanDrink(container, table, quantity))
		{
			Actor.Send(WhyCannotDrink(container, table, quantity));
			return false;
		}

		var quantityDescriber = "";
		Actor.OutputHandler.Handle(
			new MixedEmoteOutput(
				new Emote($"@ drink|drinks {quantityDescriber}from $0$?1| off of $1||$", Actor, container.Parent,
					table?.Parent), flags: OutputFlags.SuppressObscured).Append(playerEmote));

		Actor.Send(container.LiquidMixture.TasteString(Actor).Colour(Telnet.Yellow));

		var sip = new LiquidMixture(container.LiquidMixture, quantity);
		FulfilNeeds(sip.GetNeedFulfiller());

		sip.OnDraught(Actor, container);
                container.ReduceLiquidQuantity(quantity, Actor, "drink");
                CrimeExtensions.CheckPossibleCrimeAllAuthorities(Actor, CrimeTypes.IllegalConsumption, null, container.Parent, "");
                CrimeExtensions.CheckPossibleCrimeAllAuthorities(Actor, CrimeTypes.PublicIntoxication, null, container.Parent, "");
                return true;
        }

	public bool CanDrink(ILiquidContainer container, ITable table, double quantity)
	{
		if (!container.IsOpen)
		{
			return false;
		}

		if (quantity > (container.LiquidMixture?.TotalVolume ?? 0.0))
		{
			return false;
		}

		if (table != null && !CanGet(container.Parent, table.Parent, 0))
		{
			return false;
		}

		if (table == null && !(container.Parent.Location?.CanGet(container.Parent, Actor) ?? true))
		{
			return false;
		}

		if (table != null && !Actor.InVicinity(table.Parent))
		{
			return false;
		}

		// Require a working esophagus to drink
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return false;
		}

		// TODO - effects that prevent drinking
		return true;
	}

	private string WhyCannotDrink(ILiquidContainer container, ITable table, double quantity)
	{
		if (!container.IsOpen)
		{
			return "It is not possible to drink from closed containers.";
		}

		if (quantity > (container.LiquidMixture?.TotalVolume ?? 0.0))
		{
			return "There simply is not that much to drink.";
		}

		if (table != null && !CanGet(container.Parent, table.Parent, 0))
		{
			return WhyCannotGet(container.Parent, table.Parent, 0);
		}

		if (table == null && !(container.Parent.Location?.CanGet(container.Parent, Actor) ?? true))
		{
			return container.Parent.Location.WhyCannotGet(container.Parent, Actor);
		}

		if (table != null && !Actor.InVicinity(table.Parent))
		{
			return
				$"You must be in the vicinity of {table.Parent.HowSeen(Actor)} in order to drink anything from it.";
		}

		// Require a working esophagus to drink
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return "You require a working esophagus to drink anything.";
		}

		throw new NotImplementedException("Unimplemented WhyCannotEat reason in Body.WhyCannotEat");
	}

	public bool SilentSwallow(ISwallowable swallowable)
	{
		// Require a working esophagus to swallow
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return false;
		}

		// If the item is a stackable, split one off
		if (swallowable.Parent.IsItemType<IStackable>() &&
		    swallowable.Parent.GetItemType<IStackable>().Quantity > 1)
		{
			swallowable = swallowable.Parent.Get(this, 1).GetItemType<ISwallowable>();
		}

		var edible = swallowable.Parent.GetItemType<IEdible>();
		if (edible != null)
		{
			FulfilNeeds(new NeedFulfiller
			{
				SatiationPoints = edible.SatiationPoints,
				Calories = edible.Calories,
				ThirstPoints = edible.ThirstPoints,
				WaterLitres = edible.WaterLitres,
				AlcoholLitres = edible.AlcoholLitres
			});
		}

		#region Event Handling

		Actor.HandleEvent(EventType.CharacterSwallow, Actor, swallowable.Parent);
		swallowable.Parent.HandleEvent(EventType.ItemSwallowed, Actor, swallowable.Parent);
		foreach (var witness in Actor.Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterSwallowWitness, Actor, swallowable.Parent, witness);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterSwallowWitness, Actor, swallowable.Parent, witness);
		}

		#endregion

		swallowable.Swallow(this);
		return true;
	}

	public bool Swallow(ISwallowable swallowable, IContainer container, ITable table, IEmote playerEmote)
	{
		if (!CanSwallow(swallowable, container, table))
		{
			Actor.Send(WhyCannotSwallow(swallowable, container, table));
			return false;
		}

		// If the item is a stackable, split one off
		if (swallowable.Parent.IsItemType<IStackable>() &&
		    swallowable.Parent.GetItemType<IStackable>().Quantity > 1)
		{
			swallowable = swallowable.Parent.Get(this, 1).GetItemType<ISwallowable>();
		}

		Actor.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
			$"@ swallow|swallows $0$?1| from $1||$$?2| on $2||$", Actor, swallowable.Parent, container?.Parent,
			table?.Parent), flags: OutputFlags.SuppressObscured).Append(playerEmote));

		var edible = swallowable.Parent.GetItemType<IEdible>();
		if (edible != null)
		{
			FulfilNeeds(new NeedFulfiller
			{
				SatiationPoints = edible.SatiationPoints,
				Calories = edible.Calories,
				ThirstPoints = edible.ThirstPoints,
				WaterLitres = edible.WaterLitres,
				AlcoholLitres = edible.AlcoholLitres
			});
		}

		#region Event Handling

		Actor.HandleEvent(EventType.CharacterSwallow, Actor, swallowable.Parent);
		swallowable.Parent.HandleEvent(EventType.ItemSwallowed, Actor, swallowable.Parent);
		foreach (var witness in Actor.Location.EventHandlers.Except(Actor))
		{
			witness.HandleEvent(EventType.CharacterSwallowWitness, Actor, swallowable.Parent, witness);
		}

		foreach (var witness in ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterSwallowWitness, Actor, swallowable.Parent, witness);
		}

		#endregion

		swallowable.Swallow(this);
		return true;
	}

	public bool CanSwallow(ISwallowable swallowable, IContainer container, ITable table)
	{
		if (container != null && !CanGet(swallowable.Parent, container.Parent, 0))
		{
			return false;
		}

		if (container == null && !HeldOrWieldedItems.Contains(swallowable.Parent) &&
		    !CanGet(swallowable.Parent, 0))
		{
			return false;
		}

		if (table != null && !Actor.InVicinity(table.Parent))
		{
			return false;
		}

		if (table == null && container != null && !Actor.InVicinity(container.Parent))
		{
			return false;
		}

		// Require a working esophagus to swallow
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return false;
		}

		// TODO - effects that prevent swallowing
		return true;
	}

	public string WhyCannotSwallow(ISwallowable swallowable, IContainer container, ITable table)
	{
		if (container != null && !CanGet(swallowable.Parent, container.Parent, 0))
		{
			return WhyCannotGet(swallowable.Parent, container.Parent, 0);
		}

		if (container == null && !HeldOrWieldedItems.Contains(swallowable.Parent) &&
		    !CanGet(swallowable.Parent, 0))
		{
			return WhyCannotGet(swallowable.Parent, 0);
		}

		if (table != null && !Actor.InVicinity(table.Parent))
		{
			return
				$"You must be in the vicinity of {table.Parent.HowSeen(Actor)} in order to swallow anything from it.";
		}

		if (table == null && container != null && !Actor.InVicinity(container.Parent))
		{
			return
				$"You must be in the vicinity of {container.Parent.HowSeen(Actor)} in order to swallow anything from it.";
		}

		// Require a working esophagus to eat
		if (OrganFunction<EsophagusProto>() < 0.5)
		{
			return "You require a working esophagus to swallow anything.";
		}

		throw new NotImplementedException("Unimplemented WhyCannotEat reason in Body.WhyCannotSwallow");
	}

	#endregion
}