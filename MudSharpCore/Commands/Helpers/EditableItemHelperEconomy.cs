using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper PropertyHelper { get; } = new()
	{
		ItemName = "Property",
		ItemNamePlural = "Properties",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IProperty>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IProperty>(actor) { EditingItem = (IProperty)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IProperty>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Properties.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Properties.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Properties.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IProperty)item),
		CastToType = typeof(IProperty),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new property.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which economic zone should this property be tied to?");
				return;
			}

			var zone = actor.Gameworld.EconomicZones.GetByIdOrName(input.PopSpeech());
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zone.");
				return;
			}

			if (actor.Gameworld.Properties.Any(x => x.EconomicZone == zone && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a property in the {zone.Name.ColourName()} economic zone with that name. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What default property value do you want to set for this property?");
				return;
			}

			if (!zone.Currency.TryGetBaseCurrency(input.PopSpeech(), out var value))
			{
				actor.OutputHandler.Send(
					$"The value \"{input.Last.ColourCommand()}\" is not a valid amount of {zone.Currency.Name.ColourValue()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					"You must specify a bank account for the default owner of this property. Use the format BANKCODE:ACCOUNT#");
				return;
			}

			var bankString = input.PopSpeech();
			var (accountTarget, error) = Bank.FindBankAccount(bankString, null, actor);
			if (accountTarget == null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			var owner = accountTarget.AccountOwner;

			if (owner is not ICharacter && owner is not IClan)
			{
				actor.OutputHandler.Send(
					"Only bank accounts owned by characters and clans can be used for the default owner of a property.");
				return;
			}

			var property = new Property(name, zone, actor.Location, value, owner, accountTarget);
			actor.Gameworld.Add(property);
			actor.RemoveAllEffects<BuilderEditingEffect<IProperty>>();
			actor.AddEffect(new BuilderEditingEffect<IProperty>(actor) { EditingItem = property });
			actor.OutputHandler.Send(
				$"You create a new property named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) => { actor.OutputHandler.Send("The clone action is not available for properties."); },

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Zone"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IProperty>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.EconomicZone.Name
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Property #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = PropertyModule.PropertyHelpAdmins
	};

	public static EditableItemHelper AuctionHelper { get; } = new()
	{
		ItemName = "Auction House",
		ItemNamePlural = "Auction Houses",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAuctionHouse>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IAuctionHouse>(actor) { EditingItem = (IAuctionHouse)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAuctionHouse>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AuctionHouses.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AuctionHouses.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AuctionHouses.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAuctionHouse)item),
		CastToType = typeof(IAuctionHouse),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new auction house.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which economic zone should this auction house be tied to?");
				return;
			}

			var zone = actor.Gameworld.EconomicZones.GetByIdOrName(input.PopSpeech());
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					"You must specify a bank account into which any proceeds will be transferred. Use the format BANKCODE:ACCOUNT#.");
				return;
			}

			var bankString = input.PopSpeech();
			var (accountTarget, error) = Bank.FindBankAccount(bankString, null, actor);
			if (accountTarget == null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (actor.Gameworld.AuctionHouses.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an auction house with that name. Names must be unique.");
				return;
			}

			var auctionHouse = new AuctionHouse(zone, name, actor.Location, accountTarget);
			actor.Gameworld.Add(auctionHouse);
			actor.RemoveAllEffects<BuilderEditingEffect<IAuctionHouse>>();
			actor.AddEffect(new BuilderEditingEffect<IAuctionHouse>(actor) { EditingItem = auctionHouse });
			actor.OutputHandler.Send(
				$"You create a new auction house named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) => { actor.OutputHandler.Send("The clone action is not available for auction houses."); },

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Zone"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IAuctionHouse>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.EconomicZone.Name
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Auction House #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = EconomyModule.AuctionHelpAdmins
	};

	public static EditableItemHelper BankHelper { get; } = new()
	{
		ItemName = "Bank",
		ItemNamePlural = "Banks",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IBank>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IBank>(actor) { EditingItem = (IBank)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IBank>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Banks.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Banks.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Banks.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IBank)item),
		CastToType = typeof(IBank),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new bank.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What code do you want to use for your bank in transfers?");
				return;
			}

			var code = input.PopSpeech().ToLowerInvariant();
			if (actor.Gameworld.Banks.Any(x => x.Code == code))
			{
				actor.OutputHandler.Send("There is already a bank with that code. Bank codes must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which economic zone is your bank based in?");
				return;
			}

			var zone = actor.Gameworld.EconomicZones.GetByIdOrName(input.SafeRemainingArgument);
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zones.");
				return;
			}

			if (actor.Gameworld.Banks.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a bank with that name. Names must be unique.");
				return;
			}

			var bank = new Bank(actor.Gameworld, name, code, zone);
			actor.Gameworld.Add(bank);
			actor.RemoveAllEffects<BuilderEditingEffect<IBank>>();
			actor.AddEffect(new BuilderEditingEffect<IBank>(actor) { EditingItem = bank });
			actor.OutputHandler.Send(
				$"You create a new bank named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which bank do you want to clone?");
				return;
			}

			var bank = actor.Gameworld.Banks.GetByIdOrName(input.PopSpeech());
			if (bank == null)
			{
				actor.OutputHandler.Send("There is no such bank.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned bank.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.Banks.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a bank with that name. Names must be unique.");
				return;
			}

			//var clone = new Bank((Economy.Banks.Bank) bank , name);
			//actor.Gameworld.Add(clone);
			//actor.RemoveAllEffects<BuilderEditingEffect<IBank>>();
			//actor.AddEffect(new BuilderEditingEffect<IBank>(actor) {EditingItem = clone});
			//actor.OutputHandler.Send($"You clone the bank {bank.Name.ColourValue()} to a new bank called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Zone",
			"# Accounts"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IBank>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.EconomicZone.Name,
			                                                  proto.BankAccounts.Count().ToString("N0", character)
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Bank #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = EconomyModule.BankAdminHelpText
	};

	public static EditableItemHelper CoinHelper = new()
	{
		ItemName = "Coin",
		ItemNamePlural = "Coins",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<ICoin>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<ICoin>(actor) { EditingItem = (ICoin)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<ICoin>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Coins.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Coins.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Coins.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((ICoin)item),
		CastToType = typeof(ICoin),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a currency for your new coin.");
				return;
			}

			var currency = actor.Gameworld.Currencies.GetByIdOrName(input.PopSpeech());
			if (currency is null)
			{
				actor.OutputHandler.Send("There is no such currency.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new coin.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (currency.Coins.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a coin for the {currency.Name.ColourValue()} with that name. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("How much should this new coin be worth?");
				return;
			}

			decimal coinValue;
			if (decimal.TryParse(input.SafeRemainingArgument, out var value))
			{
				if (value <= 0.0M)
				{
					actor.OutputHandler.Send("You must enter a valid number greater than zero for the value.");
					return;
				}

				coinValue = value;
			}
			else
			{
				if (!currency.TryGetBaseCurrency(input.SafeRemainingArgument, out value))
				{
					actor.OutputHandler.Send($"You must either enter a number for the base currency, or enter a currency amount, and the text {input.SafeRemainingArgument.ColourCommand()} is neither.");
					return;
				}

				coinValue = value;
			}

			ICoin coin;
			using (new FMDB())
			{
				var dbitem = new Models.Coin
				{
					CurrencyId = currency.Id,
					Name = name,
					FullDescription = "This coin has no description",
					ShortDescription = "an undescribed coin",
					GeneralForm = "coin",
					PluralWord = "coin",
					Weight = actor.Gameworld.UnitManager.GetBaseUnits("10g", UnitType.Mass, out _),
					Value = coinValue
				};
				FMDB.Context.Coins.Add(dbitem);
				FMDB.Context.SaveChanges();
				coin = new Economy.Currency.Coin(actor.Gameworld, dbitem, currency);
				currency.AddCoin(coin);
			}

			actor.Gameworld.Add(coin);
			actor.RemoveAllEffects<BuilderEditingEffect<ICoin>>();
			actor.AddEffect(new BuilderEditingEffect<ICoin>(actor) { EditingItem = coin });
			actor.OutputHandler.Send(
				$"You create a new coin for the {currency.Name.ColourValue()} currency worth {currency.Describe(coinValue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} called {name.ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which coin do you want to clone?");
				return;
			}

			var template = actor.Gameworld.Coins.GetByIdOrName(input.PopSpeech());
			if (template == null)
			{
				actor.OutputHandler.Send("There is no such coin.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new coin.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (template.Currency.Coins.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a coin with that name for the {template.Currency.Name.ColourValue()} currency. Names must be unique.");
				return;
			}

			ICoin coin;
			using (new FMDB())
			{
				var dbitem = new Models.Coin
				{
					CurrencyId = template.Currency.Id,
					Name = name,
					FullDescription = template.FullDescription,
					ShortDescription = template.ShortDescription,
					GeneralForm = template.GeneralForm,
					PluralWord = template.PluralWord,
					Weight = template.Weight,
					Value = template.Value
				};
				FMDB.Context.Coins.Add(dbitem);
				FMDB.Context.SaveChanges();
				coin = new Economy.Currency.Coin(actor.Gameworld, dbitem, template.Currency);
				template.Currency.AddCoin(coin);
			}

			actor.Gameworld.Add(coin);
			actor.RemoveAllEffects<BuilderEditingEffect<ICoin>>();
			actor.AddEffect(new BuilderEditingEffect<ICoin>(actor) { EditingItem = coin });
			actor.OutputHandler.Send($"You create a new coin for the {template.Currency.Name.ColourValue()} as a clone of {template.Name.ColourValue()}, called {name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"SDesc",
			"General",
			"Plural",
			"Value",
			"Weight",
			"Currency",
			"Change?"
		},

		GetListTableContentsFunc = (actor, protos) => from coin in protos.OfType<ICoin>()
		                                              select new List<string>
		                                              {
			                                              coin.Id.ToString("N0", actor),
			                                              coin.Name,
			                                              coin.ShortDescription,
			                                              coin.GeneralForm,
			                                              coin.PluralWord,
			                                              coin.Value.ToString("N0", actor),
			                                              actor.Gameworld.UnitManager.Describe(coin.Weight, Framework.Units.UnitType.Mass, actor).ColourValue(),
			                                              coin.Currency.Name.ColourName(),
			                                              coin.UseForChange.ToColouredString()
		                                              },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			if (keyword.Length > 1 && keyword[0] == '+')
			{
				keyword = keyword.Substring(1);
				return protos
				       .Cast<ICoin>()
				       .Where(x =>
					       x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
					       x.ShortDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
					       x.FullDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
				       .Cast<IEditableItem>()
				       .ToList();
			}

			if (keyword.Length > 1 && keyword[0] == '-')
			{
				keyword = keyword.Substring(1);
				return protos
				       .Cast<ICoin>()
				       .Where(x =>
					       !x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
					       !x.ShortDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
					       !x.FullDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
				       .Cast<IEditableItem>()
				       .ToList();
			}

			if (keyword.EqualTo("change"))
			{
				return protos
				       .Cast<ICoin>()
				       .Where(x => x.UseForChange)
				       .Cast<IEditableItem>()
				       .ToList()
					;
			}

			if (keyword.EqualTo("!change"))
			{
				return protos
				       .Cast<ICoin>()
				       .Where(x => !x.UseForChange)
				       .Cast<IEditableItem>()
				       .ToList()
					;
			}

			var currency = gameworld.Currencies.GetByIdOrName(keyword);
			if (currency is not null)
			{
				return protos
				       .Cast<ICoin>()
				       .Where(x => x.Currency == currency)
				       .Cast<IEditableItem>()
				       .ToList();
			}

			return protos;
		},

		DefaultCommandHelp = EconomyModule.CoinHelp,

		GetEditHeader = item => $"Coin #{item.Id:N0} ({item.Name})"
	};
}