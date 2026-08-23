using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Shops;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.Work.Crafts;
using System.Text;

#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	private const string MenuHelp = @"The #6menu#0 command shows the menu for a cafe or restaurant in your current location.

	Use #3menu#0 to see each item, its price, dine-in/takeaway availability and current estimated wait.";

	private const string OrderHelp = @"The #6order#0 command is used to begin table service or place cafe and restaurant orders. Moving within any cell assigned to the restaurant keeps your accepted table participation; leaving the restaurant ends it.

The syntax is as follows:

	#3order table <table>#0 - begins a new table session at an unoccupied designated restaurant table, or tries to join its existing session
	#3order <item> [quantity] [for <participant>]#0 - creates an unpaid dine-in order at your accepted table
	#3order takeaway <item> [quantity] [cash|credit <account>|with <payment item>]#0 - pays up front and queues an order for collection

Party members and people whom an existing participant considers an ally join automatically; everyone else needs a current participant to #3ACCEPT#0 the transient request. The person who places a dine-in order remains liable even when ordering for someone else.";

	private const string BillHelp = @"The #6bill#0 command shows and settles the bill for your accepted restaurant table.

The syntax is as follows:

	#3bill#0 - shows the table's orders and liabilities
	#3bill pay mine [cash|credit <account>|with <payment item>]#0 - pays the lines you ordered
	#3bill pay all [cash|credit <account>|with <payment item>]#0 - pays the entire outstanding balance
	#3bill pay <amount> [cash|credit <account>|with <payment item>]#0 - makes a partial payment
	#3bill split equal#0 - shows a convenient equal payment suggestion
	#3bill close#0 - closes the table to further orders once its bill is fully paid

#6Note - paying a split bill does not change who is legally liable for the items ordered if someone dines and dashes.#0";

	private const string RestaurantHelp = @"The #6restaurant#0 command configures a restaurant shop and lets its employees operate the shared preparation and serving queue. Restaurants are also shops, so the relevant #3shop#0 and #3merchandise#0 commands remain available.

For employees, the syntax is as follows:

	#3restaurant service list#0 - reviews the service queue
	#3restaurant service prepare <order> [with <crafted item>]#0 - prepares a queued order, optionally with a completed craft
	#3restaurant service serve <order>#0 - advances a ready order through its remaining visible service steps
	#3restaurant service clear <table>#0 - collects every eligible empty serving item from a table into one bundle for return to the kitchen
	#3restaurant service cancel <order>#0 - cancels a specific order
	#3restaurant service refund <order> <amount> [cash|credit <account>|with <payment item>]#0 - issues a refund for a specific order

For restaurant managers and proprietors, the syntax is as follows:

	#3restaurant#0 - shows the configuration of the restaurant in your current location
	#3restaurant cell add|remove <service|internal|kitchen> [here|<cell>]#0 - extends the restaurant to another cell, or removes a cell
	#3restaurant table add|remove <item>#0 - adds or removes a designated restaurant table
	#3restaurant menu add|remove <item>#0 - adds or removes a shop merchandise item from the menu
	#3restaurant menu show <item>#0 - reviews a menu item's service configuration and validation
	#3restaurant menu set <item> active|dinein|takeaway#0 - toggles the corresponding availability setting
	#3restaurant menu set <item> mode <unaltered|open|craft|plate|package>#0 - sets the service mode for a menu item
	#3restaurant menu set <item> prep <timespan>#0 - sets the expected preparation time
	#3restaurant menu set <item> craft <craft|none>#0 - sets the crafting method for a menu item
	#3restaurant menu set <item> plate <prototype|none>#0 - sets the reusable plating method for a menu item
	#3restaurant menu set <item> package <prototype|none>#0 - sets the inner takeaway packaging method for a menu item
	#3restaurant menu set <item> desc <text>#0 - sets the description for a menu item
	#3restaurant storage add|remove <ingredients|tools|servingware|takeawaycontainers|takeawaybags> <container>#0 - assigns a physical kitchen container one or more storage roles
	#3restaurant set handling|batchwait|cleanup <timespan>#0 - changes service timing
	#3restaurant set takeawaybag <prototype|none>#0 - sets the restaurant-wide outer takeaway bag; each collection uses the fewest bags that fit the whole ready collection
	#3restaurant emote list#0 - reviews visible service moments
	#3restaurant emote <moment> <emote|default>#0 - sets a restaurant-specific chef or server emote
	#3restaurant chef|server <target>#0 - creates a restaurant-specific employment contract

Administrators have the following additional syntax options:

	#3restaurant create <name> <economic zone>#0 - creates a new restaurant shop in your current location
	#3restaurant set automation|simulate#0 - toggles automated service or the simulated crafting fallback

#6Tip: Disable a menu item before changing its fulfilment configuration, then reactivate it only when validation succeeds. Kitchen storage lets NPCs draw reusable plates, containers and bags from stores, and return clean serviceware and craft tools when finished.#0";

	[PlayerCommand("Menu", "menu")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("menu", MenuHelp, AutoHelp.HelpArg)]
	protected static void Menu(ICharacter actor, string command)
	{
		if (!TryGetRestaurant(actor, out var restaurant))
		{
			return;
		}

		actor.OutputHandler.Send(restaurant.ShowMenu(actor));
	}

	[PlayerCommand("Order", "order")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("order", OrderHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Order(ICharacter actor, string command)
	{
		if (!TryGetRestaurant(actor, out var restaurant))
		{
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(OrderHelp.SubstituteANSIColour());
			return;
		}

		var first = ss.PopSpeech();
		if (first.EqualTo("table"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which designated restaurant table do you want to use?");
				return;
			}

			var table = actor.TargetItem(ss.SafeRemainingArgument);
			if (table is null)
			{
				actor.OutputHandler.Send("You do not see anything like that here.");
				return;
			}

			actor.OutputHandler.Send(restaurant.TryJoinTable(actor, table).Message);
			return;
		}

		if (first.EqualTo("takeaway") || first.EqualTo("takeout"))
		{
			OrderTakeaway(actor, restaurant, ss);
			return;
		}

		OrderDineIn(actor, restaurant, first, ss);
	}

	[PlayerCommand("Bill", "bill")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("bill", BillHelp, AutoHelp.HelpArg)]
	protected static void Bill(ICharacter actor, string command)
	{
		if (!TryGetRestaurant(actor, out var restaurant))
		{
			return;
		}

		var session = restaurant.TableSessionFor(actor) as RestaurantTableSession;
		if (session is null)
		{
			actor.OutputHandler.Send("You are not an accepted participant at an active restaurant table.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(restaurant.ShowBill(actor, session));
			return;
		}

		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "pay":
				BillPay(actor, restaurant, session, ss);
				return;
			case "split":
				if (!ss.IsFinished && ss.PopSpeech().EqualTo("equal"))
				{
					ShowEqualSplit(actor, restaurant, session);
					return;
				}

				actor.OutputHandler.Send("The only supported bill split form is BILL SPLIT EQUAL.");
				return;
			case "close":
				if (!ss.IsFinished)
				{
					actor.OutputHandler.Send("Use BILL CLOSE with no additional arguments.");
					return;
				}

				actor.OutputHandler.Send(restaurant.TryCloseTableOrdering(actor, session).Message);
				return;
			default:
				actor.OutputHandler.Send(BillHelp.SubstituteANSIColour());
				return;
		}
	}

	[PlayerCommand("Restaurant", "restaurant", "cafe")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("restaurant", RestaurantHelp, AutoHelp.HelpArg)]
	protected static void RestaurantCommand(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var subcommand = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (subcommand == "create")
		{
			RestaurantCreate(actor, ss);
			return;
		}

		if (!TryGetRestaurant(actor, out var restaurant))
		{
			return;
		}

		if (subcommand is "" or "show" or "info")
		{
			actor.OutputHandler.Send(RestaurantShow(actor, restaurant));
			return;
		}

		if (new EmploymentCommandService().TryExecuteShortcut(actor, restaurant, "restaurant", subcommand, ss))
		{
			return;
		}

		if (subcommand is "service" or "queue")
		{
			RestaurantService(actor, restaurant, ss);
			return;
		}

		if (subcommand is "chef" or "server")
		{
			RestaurantDirectHire(actor, restaurant, ss, subcommand == "chef" ? EmploymentRole.Chef : EmploymentRole.Server);
			return;
		}

		if (!CanManageRestaurant(actor, restaurant))
		{
			actor.OutputHandler.Send("You must be a manager or proprietor of this restaurant to configure it.");
			return;
		}

		switch (subcommand)
		{
			case "cell":
				RestaurantCell(actor, restaurant, ss);
				return;
			case "table":
				RestaurantTable(actor, restaurant, ss);
				return;
			case "storage":
				RestaurantStorage(actor, restaurant, ss);
				return;
			case "menu":
				RestaurantMenu(actor, restaurant, ss);
				return;
			case "set":
				RestaurantSet(actor, restaurant, ss);
				return;
			case "emote":
			case "emotes":
				RestaurantEmote(actor, restaurant, ss);
				return;
			default:
				actor.OutputHandler.Send(RestaurantHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void OrderDineIn(ICharacter actor, Restaurant restaurant, string itemText, StringStack ss)
	{
		var menuItem = FindRestaurantMenuItem(restaurant, itemText);
		if (menuItem is null)
		{
			actor.OutputHandler.Send("There is no menu item like that. See MENU for the available dishes and drinks.");
			return;
		}

		var quantity = 1;
		if (!ss.IsFinished && int.TryParse(ss.PeekSpeech(), out var parsedQuantity))
		{
			quantity = parsedQuantity;
			ss.PopSpeech();
		}

		ICharacter? recipient = null;
		if (!ss.IsFinished)
		{
			if (!ss.PopSpeech().EqualTo("for") || ss.IsFinished)
			{
				actor.OutputHandler.Send("The only optional dine-in suffix is FOR <accepted table participant>.");
				return;
			}

			recipient = actor.TargetActor(ss.SafeRemainingArgument);
			if (recipient is null)
			{
				actor.OutputHandler.Send("You do not see anyone like that here.");
				return;
			}
		}

		actor.OutputHandler.Send(restaurant.TryOrderDineIn(actor, menuItem, quantity, recipient).Message);
	}

	private static void OrderTakeaway(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What menu item would you like to order as takeaway?");
			return;
		}

		var itemText = ss.PopSpeech();
		var menuItem = FindRestaurantMenuItem(restaurant, itemText);
		if (menuItem is null)
		{
			actor.OutputHandler.Send("There is no menu item like that. See MENU for the available dishes and drinks.");
			return;
		}

		var quantity = 1;
		if (!ss.IsFinished && int.TryParse(ss.PeekSpeech(), out var parsedQuantity))
		{
			quantity = parsedQuantity;
			ss.PopSpeech();
		}

		var payment = GetRestaurantPayment(actor, restaurant, ss);
		if (payment is null)
		{
			return;
		}

		actor.OutputHandler.Send(restaurant.TryOrderTakeaway(actor, menuItem, quantity, payment).Message);
	}

	private static void BillPay(ICharacter actor, Restaurant restaurant, RestaurantTableSession session, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to pay MINE, ALL, or a specific amount?");
			return;
		}

		var paymentTarget = ss.PopSpeech();
		IEnumerable<RestaurantOrder> orders;
		decimal amount;
		if (paymentTarget.EqualTo("mine"))
		{
			orders = session.Orders.OfType<RestaurantOrder>()
				.Where(x => x.OrdererCharacterId == CharacterInstanceIdentityComparer.IdentityId(actor));
			amount = orders.Sum(x => x.OutstandingBalance);
		}
		else if (paymentTarget.EqualTo("all"))
		{
			orders = session.Orders.OfType<RestaurantOrder>();
			amount = orders.Sum(x => x.OutstandingBalance);
		}
		else
		{
			amount = restaurant.Currency.GetBaseCurrency(paymentTarget, out var success);
			if (!success || amount <= 0.0M)
			{
				actor.OutputHandler.Send("That is not a valid positive amount in this restaurant's currency.");
				return;
			}

			orders = session.Orders.OfType<RestaurantOrder>()
				.Where(x => x.OrdererCharacterId == CharacterInstanceIdentityComparer.IdentityId(actor));
		}

		var payment = GetRestaurantPayment(actor, restaurant, ss);
		if (payment is null)
		{
			return;
		}

		actor.OutputHandler.Send(restaurant.TryPayBill(actor, session, orders, amount, payment, DescribeRestaurantPayment(payment)).Message);
	}

	private static void ShowEqualSplit(ICharacter actor, Restaurant restaurant, RestaurantTableSession session)
	{
		var suggestions = restaurant.EqualSplitSuggestion(session);
		if (!suggestions.Any())
		{
			actor.OutputHandler.Send("There are no accepted participants to include in an equal split.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from participant in session.Participants.Where(x => x.Accepted).OrderBy(x => x.CharacterName)
			select new[]
			{
				participant.CharacterName.ColourName(),
				restaurant.Currency.Describe(suggestions.GetValueOrDefault(participant.CharacterId), CurrencyDescriptionPatternType.Short).ColourValue(),
				"Their original order liabilities remain unchanged."
			},
			new[] { "Participant", "Suggested Payment", "Liability" },
			actor.LineFormatLength,
			truncatableColumnIndex: 2,
			colour: Telnet.Yellow,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void RestaurantCreate(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can create a restaurant.");
			return;
		}

		if (actor.Location.Shop is not null)
		{
			actor.OutputHandler.Send("This cell is already part of a shop.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name should the restaurant have?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone should the restaurant use?");
			return;
		}

		var zoneText = ss.PopSpeech();
		var zone = long.TryParse(zoneText, out var zoneId)
			? actor.Gameworld.EconomicZones.Get(zoneId)
			: actor.Gameworld.EconomicZones.GetByName(zoneText);
		if (zone is null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		var restaurant = new Restaurant(zone, actor.Location, name);
		actor.Gameworld.Add(restaurant);
		actor.OutputHandler.Send($"You create {restaurant.Name.ColourName()} as a restaurant shop in {zone.Name.ColourName()}.");
	}

	private static void RestaurantCell(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		var action = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (action is not ("add" or "remove" or "rem"))
		{
			actor.OutputHandler.Send("Use RESTAURANT CELL ADD|REMOVE <service|internal|kitchen> [here|<cell id>].");
			return;
		}

		if (!TryParseRestaurantCellRole(ss.PopSpeech(), out var role))
		{
			actor.OutputHandler.Send("Which restaurant cell role do you want to use? Valid roles are SERVICE, INTERNAL and KITCHEN.\n\n\tRESTAURANT CELL ADD|REMOVE <service|internal|kitchen> [here|<cell id>]");
			return;
		}

		var cell = actor.Location;
		if (!ss.IsFinished && !ss.PeekSpeech().EqualTo("here"))
		{
			var cellText = ss.PopSpeech();
			cell = long.TryParse(cellText, out var cellId)
				? actor.Gameworld.Cells.Get(cellId)
				: actor.Gameworld.Cells.GetByName(cellText);
		}
		else if (!ss.IsFinished)
		{
			ss.PopSpeech();
		}

		if (cell is null)
		{
			actor.OutputHandler.Send("There is no such cell.");
			return;
		}

		var result = action == "add"
			? restaurant.AddRestaurantCell(cell, role)
			: restaurant.RemoveRestaurantCell(cell, role);
		actor.OutputHandler.Send(result.Message);
	}

	private static void RestaurantTable(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		var action = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (action is not ("add" or "remove" or "rem"))
		{
			actor.OutputHandler.Send("Use RESTAURANT TABLE ADD|REMOVE <item>.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which physical table item do you mean?");
			return;
		}

		var table = actor.TargetItem(ss.SafeRemainingArgument);
		if (table is null)
		{
			actor.OutputHandler.Send("You do not see any such item.");
			return;
		}

		actor.OutputHandler.Send(action == "add"
			? restaurant.AddRestaurantTable(table).Message
			: restaurant.RemoveRestaurantTable(table).Message);
	}

	private static void RestaurantStorage(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		var action = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (action is not ("add" or "remove" or "rem"))
		{
			actor.OutputHandler.Send("Use RESTAURANT STORAGE ADD|REMOVE <ingredients|tools|servingware|takeawaycontainers|takeawaybags> <container>.");
			return;
		}

		if (!TryParseRestaurantStorageRole(ss.PopSpeech(), out var role))
		{
			actor.OutputHandler.Send("Which restaurant storage role do you want to use? Valid roles are INGREDIENTS, TOOLS, SERVINGWARE, TAKEAWAYCONTAINERS and TAKEAWAYBAGS.\n\n\tRESTAURANT STORAGE ADD|REMOVE <ingredients|tools|servingware|takeawaycontainers|takeawaybags> <container>");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which kitchen container do you want to configure?");
			return;
		}

		var item = actor.TargetItem(ss.SafeRemainingArgument);
		if (item is null)
		{
			actor.OutputHandler.Send("You do not see any such container.");
			return;
		}

		actor.OutputHandler.Send(restaurant.SetStorageRole(item, role, action == "add").Message);
	}

	private static void RestaurantMenu(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		var action = ss.PopSpeech().CollapseString().ToLowerInvariant();
		switch (action)
		{
			case "add":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which shop merchandise should be added to the menu?");
					return;
				}

				var merchandise = FindRestaurantMerchandise(restaurant, ss.PopSpeech());
				if (merchandise is null)
				{
					actor.OutputHandler.Send("There is no merchandise like that in this restaurant shop.");
					return;
				}

				if (restaurant.MenuItems.Any(x => x.Merchandise.Id == merchandise.Id))
				{
					actor.OutputHandler.Send("That merchandise is already on the restaurant menu.");
					return;
				}

				var added = restaurant.AddMenuItem(merchandise);
				actor.OutputHandler.Send($"You add {added.Name.ColourName()} to the menu as item #{added.Id.ToString("N0", actor)}.");
				return;
			case "remove":
			case "rem":
			case "delete":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which restaurant menu item do you want to remove?");
					return;
				}

				var remove = FindRestaurantMenuItem(restaurant, ss.PopSpeech());
				if (remove is null)
				{
					actor.OutputHandler.Send("There is no such restaurant menu item.");
					return;
				}

				actor.OutputHandler.Send(restaurant.RemoveMenuItem(remove)
					? "That menu item has been removed."
					: "That menu item could not be removed.");
				return;
			case "set":
				RestaurantMenuSet(actor, restaurant, ss);
				return;
			case "show":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which restaurant menu item do you want to review?");
					return;
				}

				var show = FindRestaurantMenuItem(restaurant, ss.PopSpeech());
				if (show is null)
				{
					actor.OutputHandler.Send("There is no such restaurant menu item.");
					return;
				}

				actor.OutputHandler.Send(RestaurantMenuShow(actor, restaurant, show));
				return;
			default:
				actor.OutputHandler.Send("The restaurant menu commands are:\n\n\tRESTAURANT MENU ADD <merchandise>\n\tRESTAURANT MENU REMOVE <item>\n\tRESTAURANT MENU SHOW <item>\n\tRESTAURANT MENU SET <item> <setting>");
				return;
		}
	}

	private static void RestaurantMenuSet(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		var menu = FindRestaurantMenuItem(restaurant, ss.PopSpeech());
		if (menu is null || ss.IsFinished)
		{
			actor.OutputHandler.Send("Use RESTAURANT MENU SET <item> ACTIVE|DINEIN|TAKEAWAY, MODE <mode>, PREP <timespan>, CRAFT <craft|none>, PLATE|PACKAGE <prototype|none>, or DESC <text>.");
			return;
		}

		var setting = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (setting != "active" && menu.IsActive)
		{
			actor.OutputHandler.Send("Deactivate this menu item before changing its fulfilment configuration, then reactivate it once validation succeeds.");
			return;
		}

		if (setting != "active" && restaurant.Orders.OfType<RestaurantOrder>().Any(x =>
			x.MenuItem.Id == menu.Id &&
			(x.Status is RestaurantOrderStatus.Queued or RestaurantOrderStatus.Preparing or RestaurantOrderStatus.ReadyForService)))
		{
			actor.OutputHandler.Send("That menu item has active orders. Finish, cancel, or refund them before changing its fulfilment configuration.");
			return;
		}

		switch (setting)
		{
			case "active":
				if (!ss.IsFinished)
				{
					actor.OutputHandler.Send("RESTAURANT MENU SET <item> ACTIVE is a toggle and takes no value.");
					return;
				}

				var active = !menu.IsActive;

				if (active && !menu.IsValid(out var reason))
				{
					actor.OutputHandler.Send($"That menu item cannot be activated because {reason}.");
					return;
				}

				menu.IsActive = active;
				actor.OutputHandler.Send($"That menu item is now {(active ? "active" : "inactive").ColourValue()}.");
				return;
			case "dinein":
			case "dine-in":
				if (!ss.IsFinished)
				{
					actor.OutputHandler.Send("RESTAURANT MENU SET <item> DINEIN is a toggle and takes no value.");
					return;
				}

				menu.DineInAvailable = !menu.DineInAvailable;
				actor.OutputHandler.Send($"Dine-in availability is now {menu.DineInAvailable.ToColouredString()}.");
				return;
			case "takeaway":
			case "take-out":
				if (!ss.IsFinished)
				{
					actor.OutputHandler.Send("RESTAURANT MENU SET <item> TAKEAWAY is a toggle and takes no value.");
					return;
				}

				menu.TakeawayAvailable = !menu.TakeawayAvailable;
				actor.OutputHandler.Send($"Takeaway availability is now {menu.TakeawayAvailable.ToColouredString()}.");
				return;
			case "mode":
				if (!TryParseFulfilmentMode(ss.PopSpeech(), out var mode))
				{
					actor.OutputHandler.Send("Use one of UNALTERED, OPEN, CRAFT, PLATE, or PACKAGE.");
					return;
				}

				menu.FulfilmentMode = mode;
				actor.OutputHandler.Send("The fulfilment mode has been updated.");
				return;
			case "prep":
			case "preptime":
				if (MudTimeSpan.TryParse(ss.SafeRemainingArgument, out var preparationTime) && preparationTime >= MudTimeSpan.Zero)
				{
					menu.PreparationTime = preparationTime;
					actor.OutputHandler.Send("The expected preparation time has been updated.");
				}
				else
				{
					actor.OutputHandler.Send("You must specify a non-negative time span.");
				}
				return;
			case "craft":
				if (ss.SafeRemainingArgument.EqualTo("none"))
				{
					menu.Craft = null;
					actor.OutputHandler.Send($"The menu item {menu.Name.ColourName()} no longer has an associated craft.");
					return;
				}

				var craft = FindCraft(actor, ss.SafeRemainingArgument);
				if (craft is null)
				{
					actor.OutputHandler.Send($"There is no such craft identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
					return;
				}
				menu.Craft = craft;
				actor.OutputHandler.Send($"The menu item {menu.Name.ColourName()} is now made from the craft {craft.Name.ColourValue()}.");
				return;
			case "plate":
				if (ss.SafeRemainingArgument.EqualTo("none"))
				{
					menu.ServingContainerPrototype = null;
					actor.OutputHandler.Send($"The menu item {menu.Name.ColourName()} no longer has an associated plate prototype.");
					return;
				}

				var servingContainerPrototype = FindItemPrototype(actor, ss.SafeRemainingArgument);
				if (servingContainerPrototype is null)
				{
					actor.OutputHandler.Send($"There is no item prototype identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
					return;
				}

				if (!servingContainerPrototype.IsItemType<IContainerPrototype>())
				{
					actor.OutputHandler.Send($"The item prototype {servingContainerPrototype.EditHeaderColour(actor)} is not a container.");
					return;
				}

				menu.ServingContainerPrototype = servingContainerPrototype;
				actor.OutputHandler.Send($"The menu item {menu.Name.ColourName()} now has the {servingContainerPrototype.EditHeaderColour(actor)} prototype as a serving plate.");
				return;
			case "package":
				if (ss.SafeRemainingArgument.EqualTo("none"))
				{
					menu.TakeawayContainerPrototype = null;
					actor.OutputHandler.Send($"The menu item {menu.Name.ColourName()} no longer has an associated takeaway container prototype.");
					return;
				}

				var takeawayContainerPrototype = FindItemPrototype(actor, ss.SafeRemainingArgument);
				if (takeawayContainerPrototype is null)
				{
					actor.OutputHandler.Send($"There is no item prototype identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
					return;
				}

				if (!takeawayContainerPrototype.IsItemType<IContainerPrototype>())
				{
					actor.OutputHandler.Send($"The item prototype {takeawayContainerPrototype.EditHeaderColour(actor)} is not a container.");
					return;
				}

				menu.TakeawayContainerPrototype = takeawayContainerPrototype;
				actor.OutputHandler.Send($"The menu item {menu.Name.ColourName()} now has the {takeawayContainerPrototype.EditHeaderColour(actor)} prototype as a takeaway container.");
				return;
			case "desc":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("What description would you like to set?");
					return;
				}

				menu.Description = ss.SafeRemainingArgument;
				actor.OutputHandler.Send($"The menu item {menu.Name.ColourName()} is now described as {menu.Description.ColourValue()}.");
				return;
			default:
				actor.OutputHandler.Send("Use RESTAURANT MENU SET <item> ACTIVE|DINEIN|TAKEAWAY, MODE <mode>, PREP <timespan>, CRAFT <craft|none>, PLATE|PACKAGE <prototype|none>, or DESC <text>.");
				return;
		}
	}

	private static void RestaurantSet(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		var setting = ss.PopSpeech().CollapseString().ToLowerInvariant();
		switch (setting)
		{
			case "automation":
			case "automated":
				if (!actor.IsAdministrator())
				{
					actor.OutputHandler.Send("Only administrators can change the automated service setting.");
					return;
				}

				if (!ss.IsFinished)
				{
					actor.OutputHandler.Send("RESTAURANT SET AUTOMATION is a toggle and takes no value.");
					return;
				}

				restaurant.AutomatedService = !restaurant.AutomatedService;
				actor.OutputHandler.Send($"Automated restaurant service is now {restaurant.AutomatedService.ToColouredString()}.");
				return;
			case "simulate":
			case "simulatecrafting":
				if (!actor.IsAdministrator())
				{
					actor.OutputHandler.Send("Only administrators can change the simulated service setting.");
					return;
				}

				if (!ss.IsFinished)
				{
					actor.OutputHandler.Send("RESTAURANT SET SIMULATE is a toggle and takes no value.");
					return;
				}

				restaurant.SimulateCrafting = !restaurant.SimulateCrafting;
				actor.OutputHandler.Send($"Simulated crafting fallback is now {restaurant.SimulateCrafting.ToColouredString()}.");
				return;
			case "handling":
				if (!MudTimeSpan.TryParse(ss.SafeRemainingArgument, out var handlingTime))
				{
					actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument} is not a valid time span.");
					return;
				}

				if (handlingTime < MudTimeSpan.Zero)
				{
					actor.OutputHandler.Send("You must specify a non-negative time span.");
					return;
				}

				restaurant.HandlingTime = handlingTime;
				actor.OutputHandler.Send($"The restaurant will now assume a handling time of {handlingTime.Describe(actor).ColourValue()}.");
				return;
			case "batchwait":
			case "batch":
				if (!MudTimeSpan.TryParse(ss.SafeRemainingArgument, out var batchTime))
				{
					actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument} is not a valid time span.");
					return;
				}

				if (batchTime < MudTimeSpan.Zero)
				{
					actor.OutputHandler.Send("You must specify a non-negative time span.");
					return;
				}

				restaurant.MaximumBatchWait = batchTime;
				actor.OutputHandler.Send($"The restaurant will now wait a maximum of {batchTime.Describe(actor).ColourValue()} before serving multiple orders to the same table.");
				return;
			case "cleanup":
			case "cleanupinterval":
				if (!MudTimeSpan.TryParse(ss.SafeRemainingArgument, out var cleanupTime))
				{
					actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument} is not a valid time span.");
					return;
				}

				if (cleanupTime < MudTimeSpan.Zero)
				{
					actor.OutputHandler.Send("You must specify a non-negative time span.");
					return;
				}

				restaurant.TableCleanupInterval = cleanupTime;
				actor.OutputHandler.Send($"The restaurant will now wait {cleanupTime.Describe(actor).ColourValue()} before automatically clearing an unoccupied table.");
				return;
			case "takeawaybag":
			case "bag":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which item prototype should be this restaurant's takeaway bag? Use NONE to disable outer takeaway bags.");
					return;
				}

				if (ss.SafeRemainingArgument.EqualTo("none"))
				{
					restaurant.TakeawayBagPrototype = null;
					actor.OutputHandler.Send("This restaurant will now hand takeaway items over without bags.");
					return;
				}

				var bagPrototype = FindItemPrototype(actor, ss.SafeRemainingArgument);
				if (bagPrototype is null)
				{
					actor.OutputHandler.Send("There is no such item prototype for a takeaway bag.");
					return;
				}

				if (bagPrototype.Components.OfType<ContainerGameItemComponentProto>().Count() != 1)
				{
					actor.OutputHandler.Send("A restaurant takeaway bag must have exactly one standard Container component so the restaurant can calculate the minimum number of bags required.");
					return;
				}

				restaurant.TakeawayBagPrototype = bagPrototype;
				actor.OutputHandler.Send($"{bagPrototype.EditHeaderColour(actor)} is now this restaurant's takeaway bag.");
				return;
			default:
				actor.OutputHandler.Send("Use RESTAURANT SET AUTOMATION|SIMULATE, HANDLING|BATCHWAIT|CLEANUP <timespan>, or TAKEAWAYBAG <prototype|none>.");
				return;
		}
	}

	private static void RestaurantEmote(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualTo("list"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			actor.OutputHandler.Send(StringUtilities.GetTextTable(
				Enum.GetValues<RestaurantServiceEmoteType>().Select(type => new[]
				{
					RestaurantServiceEmotes.Describe(type),
					restaurant.GetServiceEmote(type)
				}),
				new[] { "Moment", "Configured Emote" }, actor));
			return;
		}

		if (!RestaurantServiceEmotes.TryParse(ss.PopSpeech(), out var type))
		{
			actor.OutputHandler.Send("Valid restaurant emote moments are CHEFSTART, CHEFOPEN, CHEFPLATE, CHEFREADY, SERVERSERVE, SERVERCLEAR and SERVERRETURN.");
			return;
		}

		var emoteText = ss.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(emoteText))
		{
			actor.OutputHandler.Send("What emote should be used for that service moment? Use DEFAULT to restore the standard emote.");
			return;
		}

		if (emoteText.EqualTo("default"))
		{
			restaurant.SetServiceEmote(type, RestaurantServiceEmotes.DefaultFor(type));
			actor.OutputHandler.Send($"The {RestaurantServiceEmotes.Describe(type).ColourName()} emote has been restored to its default.");
			return;
		}

		var testEmote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable(), new DummyPerceivable());
		if (!testEmote.Valid)
		{
			actor.OutputHandler.Send(testEmote.ErrorMessage.ColourError());
			return;
		}

		restaurant.SetServiceEmote(type, emoteText);
		actor.OutputHandler.Send($"The {RestaurantServiceEmotes.Describe(type).ColourName()} emote has been updated. @ is the worker; $0, $1 and $2 are the event subjects where applicable.");
	}

	private static void RestaurantService(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		var action = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (action is not ("prepare" or "serve" or "clear" or "cancel" or "refund" or "list" or "queue"))
		{
			actor.OutputHandler.Send("Use RESTAURANT SERVICE LIST, PREPARE <order> [with <crafted item>], SERVE <order>, CLEAR <table>, CANCEL <order>, or REFUND <order> <amount> [payment].");
			return;
		}

		if (action is "list" or "queue")
		{
			if (!IsRestaurantEmployee(actor, restaurant))
			{
				actor.OutputHandler.Send("You must be a current restaurant employee to view its service queue.");
				return;
			}

			actor.OutputHandler.Send(restaurant.ShowServiceQueue(actor));
			return;
		}

		if (action == "clear")
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which designated restaurant table do you want to clear?");
				return;
			}

			var table = actor.TargetItem(ss.SafeRemainingArgument);
			actor.OutputHandler.Send(table is null
				? "You do not see any such table."
				: restaurant.TryClearTable(actor, table).Message);
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var orderId) || restaurant.Orders.OfType<RestaurantOrder>().FirstOrDefault(x => x.Id == orderId) is not { } order)
		{
			actor.OutputHandler.Send("There is no restaurant order with that number.");
			return;
		}

		if (action == "serve")
		{
			actor.OutputHandler.Send(restaurant.TryServeOrder(actor, order).Message);
			return;
		}

		if (action == "cancel")
		{
			actor.OutputHandler.Send(restaurant.CancelOrder(actor, order, "Cancelled through restaurant service.").Message);
			return;
		}

		if (action == "refund")
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("How much should be refunded?");
				return;
			}

			var amount = restaurant.Currency.GetBaseCurrency(ss.PopSpeech(), out var success);
			if (!success || amount <= 0.0M)
			{
				actor.OutputHandler.Send("That is not a valid positive refund amount in this restaurant's currency.");
				return;
			}

			var recipient = actor.Gameworld.Actors.FirstOrDefault(x =>
				CharacterInstanceIdentityComparer.IdentityId(x) == order.OrdererCharacterId && restaurant.IsWithinRestaurant(x.Location));
			if (recipient is null)
			{
				actor.OutputHandler.Send("The orderer must be present in the restaurant to receive this refund.");
				return;
			}

			var refundMethod = GetRestaurantPayment(recipient, restaurant, ss);
			if (refundMethod is null)
			{
				return;
			}

			actor.OutputHandler.Send(restaurant.RefundOrder(actor, recipient, order, amount, refundMethod).Message);
			return;
		}

		IGameItem? craftedOutput = null;
		if (!ss.IsFinished)
		{
			if (!ss.PopSpeech().EqualTo("with") || ss.IsFinished)
			{
				actor.OutputHandler.Send("Use RESTAURANT SERVICE PREPARE <order> [WITH <crafted item>].");
				return;
			}

			craftedOutput = actor.TargetItem(ss.SafeRemainingArgument);
			if (craftedOutput is null)
			{
				actor.OutputHandler.Send("You do not see any such crafted item.");
				return;
			}
		}

		actor.OutputHandler.Send(restaurant.TryPrepareOrder(actor, order, craftedOutput).Message);
	}

	private static void RestaurantDirectHire(ICharacter actor, Restaurant restaurant, StringStack ss, EmploymentRole role)
	{
		if (!CanManageRestaurant(actor, restaurant))
		{
			actor.OutputHandler.Send("You must be a manager or proprietor of this restaurant to employ staff.");
			return;
		}

		var target = actor.TargetActor(ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send($"Who do you want to employ as a {role.DescribeEnum()}?");
			return;
		}

		new EmploymentCommandService().TryHireDirectContract(actor, restaurant, target, role, out _, out var message);
		actor.OutputHandler.Send(message);
	}

	private static string RestaurantShow(ICharacter actor, Restaurant restaurant)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{restaurant.Name.ColourName()} Restaurant".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Automated Service: {restaurant.AutomatedService.ToColouredString()}");
		sb.AppendLine($"Simulated Crafting: {restaurant.SimulateCrafting.ToColouredString()}");
		sb.AppendLine($"Handling Time: {restaurant.HandlingTime.Describe(actor).ColourValue()}");
		sb.AppendLine($"Maximum Batch Wait: {restaurant.MaximumBatchWait.Describe(actor).ColourValue()}");
		sb.AppendLine($"Table Cleanup Cadence: {restaurant.TableCleanupInterval.Describe(actor).ColourValue()}");
		sb.AppendLine("Service Emotes: #3RESTAURANT EMOTE LIST#0".SubstituteANSIColour());
		sb.AppendLine($"Service Cells: {restaurant.ServiceCells.Select(x => x.GetFriendlyReference(actor)).ListToString()}");
		sb.AppendLine($"Internal Cells: {restaurant.InternalCells.Select(x => x.GetFriendlyReference(actor)).ListToString()}");
		sb.AppendLine($"Kitchen Cells: {restaurant.KitchenCells.Select(x => x.GetFriendlyReference(actor)).ListToString()}");
		sb.AppendLine($"Tables: {restaurant.RestaurantTables.Select(x => x.HowSeen(actor)).ListToString()}");
		sb.AppendLine($"Takeaway Bag: {restaurant.TakeawayBagPrototype?.EditHeader().ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Storage: {(restaurant.StorageContainers.Any()
			? restaurant.StorageContainers.Select(x =>
				$"{actor.Gameworld.TryGetItem(x.GameItemId, true)?.HowSeen(actor) ?? $"Missing item #{x.GameItemId:N0}"} ({x.Roles.DescribeEnum()})")
				.ListToString()
			: "None".ColourError())}");
		sb.AppendLine($"Menu Items: {restaurant.MenuItems.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Queued / Preparing / Ready: {restaurant.Orders.Count(x => x.Status == RestaurantOrderStatus.Queued).ToString("N0", actor)} / {restaurant.Orders.Count(x => x.Status == RestaurantOrderStatus.Preparing).ToString("N0", actor)} / {restaurant.Orders.Count(x => x.Status == RestaurantOrderStatus.ReadyForService).ToString("N0", actor)}");
		return sb.ToString();
	}

	private static string RestaurantMenuShow(ICharacter actor, Restaurant restaurant, RestaurantMenuItem menu)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Restaurant Menu Item #{menu.Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Merchandise: {menu.Merchandise.Name.ColourName()}");
		sb.AppendLine($"Description: {menu.Description}");
		sb.AppendLine($"Active: {menu.IsActive.ToColouredString()}");
		sb.AppendLine($"Dine-in / Takeaway: {menu.DineInAvailable.ToColouredString()} / {menu.TakeawayAvailable.ToColouredString()}");
		sb.AppendLine($"Fulfilment: {menu.FulfilmentMode.DescribeEnum().ColourName()}");
		sb.AppendLine($"Preparation: {menu.PreparationTime.Describe(actor).ColourValue()}");
		sb.AppendLine($"Craft: {menu.Craft?.EditHeader().ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Serving Container: {menu.ServingContainerPrototype?.EditHeader().ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Takeaway Container: {menu.TakeawayContainerPrototype?.EditHeader().ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Restaurant Takeaway Bag: {restaurant.TakeawayBagPrototype?.EditHeader().ColourName() ?? "None".ColourError()}");
		sb.AppendLine(menu.IsValid(out var reason) ? "Validation: Valid".ColourValue() : $"Validation: {reason}".ColourError());
		return sb.ToString();
	}

	private static bool TryGetRestaurant(ICharacter actor, out Restaurant restaurant)
	{
		restaurant = null!;
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return false;
		}

		if (shop is not Restaurant typedRestaurant)
		{
			actor.OutputHandler.Send("There is no cafe or restaurant service here.");
			return false;
		}

		restaurant = typedRestaurant;
		return true;
	}

	private static bool CanManageRestaurant(ICharacter actor, Restaurant restaurant)
	{
		return actor.IsAdministrator() || restaurant.IsManager(actor) || restaurant.IsProprietor(actor);
	}

	private static bool IsRestaurantEmployee(ICharacter actor, Restaurant restaurant)
	{
		if (actor.IsAdministrator())
		{
			return true;
		}

		var actorId = CharacterInstanceIdentityComparer.IdentityId(actor);
		return restaurant.Employment.EmploymentContracts.Any(x =>
			x.Status == EmploymentStatus.Active && CharacterInstanceIdentityComparer.IdentityId(x.Employee) == actorId);
	}

	private static RestaurantMenuItem? FindRestaurantMenuItem(Restaurant restaurant, string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		var items = restaurant.MenuItems.OfType<RestaurantMenuItem>().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
		if (long.TryParse(text, out var id))
		{
			return items.FirstOrDefault(x => x.Id == id) ?? items.ElementAtOrDefault((int)id - 1);
		}

		return items.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			items.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
	}

	private static IMerchandise? FindRestaurantMerchandise(Restaurant restaurant, string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		if (long.TryParse(text, out var id))
		{
			return restaurant.Merchandises.FirstOrDefault(x => x.Id == id) ??
				restaurant.Merchandises.OrderBy(x => x.Name).ElementAtOrDefault((int)id - 1);
		}

		return restaurant.Merchandises.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			restaurant.Merchandises.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
	}

	private static IPaymentMethod? GetRestaurantPayment(ICharacter actor, Restaurant restaurant, StringStack ss)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualTo("cash"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}
			return new ShopCashPayment(restaurant.Currency, restaurant, actor);
		}

		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "account":
			case "credit":
			case "cred":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which restaurant line of credit account do you want to use?");
					return null;
				}

				var account = restaurant.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(ss.PopSpeech()));
				if (account is null)
				{
					actor.OutputHandler.Send("There is no such line of credit account at this restaurant.");
					return null;
				}

				return new LineOfCreditPayment(actor, account);
			case "with":
			case "card":
				if (restaurant.BankAccount is null || restaurant.BankAccount.Currency != restaurant.Currency)
				{
					actor.OutputHandler.Send("This restaurant does not accept bank-item payment.");
					return null;
				}

				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("What payment item do you want to use?");
					return null;
				}

				var item = actor.TargetPersonalItem(ss.SafeRemainingArgument);
				var paymentItem = item?.GetItemType<IBankPaymentItem>();
				if (paymentItem is null)
				{
					actor.OutputHandler.Send("That is not a valid bank payment item.");
					return null;
				}

				return new BankPayment(actor, paymentItem, restaurant);
			default:
				actor.OutputHandler.Send("Payment must be CASH, CREDIT <account>, or WITH <payment item>.");
				return null;
		}
	}

	private static string DescribeRestaurantPayment(IPaymentMethod paymentMethod)
	{
		return paymentMethod switch
		{
			ShopCashPayment => "Cash",
			BankPayment => "Bank payment item",
			LineOfCreditPayment => "Line of credit",
			_ => paymentMethod.GetType().Name
		};
	}

	private static bool TryParseRestaurantCellRole(string text, out RestaurantCellRole role)
	{
		switch (text.CollapseString().ToLowerInvariant())
		{
			case "service":
			case "dining":
				role = RestaurantCellRole.Service;
				return true;
			case "internal":
			case "bathroom":
				role = RestaurantCellRole.Internal;
				return true;
			case "kitchen":
				role = RestaurantCellRole.Kitchen;
				return true;
			default:
				role = default;
				return false;
		}
	}

	private static bool TryParseRestaurantStorageRole(string text, out RestaurantStorageRole role)
	{
		switch (text.CollapseString().ToLowerInvariant())
		{
			case "ingredient":
			case "ingredients":
				role = RestaurantStorageRole.Ingredients;
				return true;
			case "tool":
			case "tools":
				role = RestaurantStorageRole.Tools;
				return true;
			case "plate":
			case "plates":
			case "servingware":
			case "serviceware":
				role = RestaurantStorageRole.Servingware;
				return true;
			case "takeawaycontainer":
			case "takeawaycontainers":
			case "package":
			case "packages":
				role = RestaurantStorageRole.TakeawayContainers;
				return true;
			case "takeawaybag":
			case "takeawaybags":
			case "bag":
			case "bags":
				role = RestaurantStorageRole.TakeawayBags;
				return true;
			default:
				role = RestaurantStorageRole.None;
				return false;
		}
	}

	private static bool TryParseFulfilmentMode(string text, out RestaurantFulfilmentMode mode)
	{
		switch (text.CollapseString().ToLowerInvariant())
		{
			case "unaltered":
			case "bring":
				mode = RestaurantFulfilmentMode.BringUnaltered;
				return true;
			case "open":
				mode = RestaurantFulfilmentMode.OpenAndBring;
				return true;
			case "craft":
				mode = RestaurantFulfilmentMode.CraftAndBring;
				return true;
			case "plate":
			case "plating":
				mode = RestaurantFulfilmentMode.CraftAndPlate;
				return true;
			case "package":
			case "packaging":
				mode = RestaurantFulfilmentMode.PackageTakeaway;
				return true;
			default:
				mode = default;
				return false;
		}
	}

	private static ICraft? FindCraft(ICharacter actor, string text)
	{
		if (text.EqualTo("none"))
		{
			return null;
		}

		return long.TryParse(text, out var id)
			? actor.Gameworld.Crafts.Get(id)
			: actor.Gameworld.Crafts.GetByIdOrName(text);
	}

	private static IGameItemProto? FindItemPrototype(ICharacter actor, string text)
	{
		if (text.EqualTo("none"))
		{
			return null;
		}

		return long.TryParse(text, out var id)
			? actor.Gameworld.ItemProtos.Get(id)
			: actor.Gameworld.ItemProtos.GetByIdOrName(text);
	}
}
