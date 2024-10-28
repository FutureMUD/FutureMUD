using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Helpers;
using MudSharp.Community;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Property;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using TimeSpanParserUtil;

namespace MudSharp.Commands.Modules;

internal class PropertyModule : Module<ICharacter>
{
	private PropertyModule()
		: base("Property")
	{
		IsNecessary = true;
	}

	public static PropertyModule Instance { get; } = new();

	public const string PropertyHelpPlayers =
		@"The property command is used to buy, sell, lease and view real estate property. Most of the subcommands within this command need to be used at a location marked as a conveyancer or real estate agent. You will know when you are in such a location because a notification will appear below the room description.

If you are a property owner, you can can create ""Sale Orders"" and ""Lease Orders"" for your property. All property owners must consent to both of these. 

A Sale Order is an intention to fully sell all ownership interest in the property to any party that pays the listed price. All revenues from the sale will be paid into your nominated bank account.

A Lease Order is an intention to lease the property to a tenant. The Lease Order and the Lease itself are separate things; the Lease Order contains the rules for how NEW or RENEWED leases will be treated. Changing or cancelling a Lease Order does not cancel any Leases that are still valid.

Note that there are two closely related commands to this one you also need to be aware of. See the PROPERTIES command for a succinct listing of properties. Also see the CLAN PROPERTY command for some clan-related versions of these property commands.

The options you can use with this command are as follows:

	#3property preview <name>#0 - previews a property that you own, are leasing, or is for sale/lease
	#3property buy <name>#0 - buys a property that is for sale
	#3property buy <name> <bankcode>:<accn>#0 - buys a property that is for sale, paying with a bank account
	#3property lease <name>#0 - leases a property that is available for lease
	#3property lease <name> <bankcode>:<accn>#0 - leases a property that is available for lease, paying with a bank account
	#3property tenant <name> add <target>#0 - adds someone as a listed tenant on your lease
	#3property tenant <name> remove <name>#0 - removes someone as a listed tenant on your lease
	#3property pay <name> <amount>#0 - pays an amount towards your rent or owing debts
	#3property pay <name> <amount> <bankcode>:<accn>#0 - pays an amount towards your rent or owing debts (using bank account)
	#3property returnkey <keyitem>#0 - returns a key you are holding
	#3property claimkeys <name>#0 - claims all returned keys associated with the specified property
	#3property returnbond <name>#0 - returns any unclaimed bond on an expired lease
	#3property claimshops <name>#0 - claims all shops associated with the specified property

The following commands are specific to those who own a property (or who are managing a clan-owned property):

	#3property setbank <name> <bankcode>:<account number>#0 - sets the bank account associated with any revenues you make on a property
	#3property sell <name> <price>#0 - lists a property that you own for sale
	#3property delaysale <name> <timespan>#0 - delays the sale of a property for the specific amount of time from now
	#3property delaysale <name> until <date>#0 - delays the sale of a property until a specific date
	#3property approvesale <name>#0 - gives your consent to a proposed sale of your property
	#3property addkey <name> <keyitem> <keyname> <replacement cost>#0 - adds a key to a property
	#3property remkey <name> <keyname>#0 - removes a key from a property
	#3property makelease <name> <price per 14 days> <bond>#0 - lists a property you own as available for leasing
	#3property interval <name> <interval>#0 - sets the payment interval for a lease, such as '2 weeks'
	#3property approvelease <name>#0 - gives your consent to lease a property you own
	#3property leaseprog <name> <prog> [character|clan]#0 - sets an optional prog to determine who can lease your property
	#3property leaseprog <name> none character|clan#0 - clears a prog controlling a lease
	#3property leaselength <name> <minimum length> <maximum length>#0 - changes the minimum and maximum lease lengths
	#3property autorenewlease <name>#0 - toggles whether a lease will automatically renew (true by default)
	#3property autorelist <name>#0 - toggles whether a lease will automatically relist (true by default)
	#3property allownovation <name>#0 - toggles whether a lesee can novate the lease to another (true by default)
	#3property bond <name> <bond>#0 - changes the bond required for a lease
	#3property rent <name> <price>#0 - changes the rent per interval for the lease
	#3property cancellease <name>#0 - cancels a lease order for the property
	#3property terminatelease <name>#0 - terminates a lease early when in default
	#3property claimbond <name> <amount>#0 - claims a deduction from bond owing to you";

	public const string PropertyHelpAdmins =
		@"The property command allows you to create, edit and assign real estate property. This allows the property to be owned, leased, and interacts with other systems in the MUD like law enforcement.

	#3property list#0 - lists all of the properties
	#3property show <which>#0 - shows information about a property
	#3property edit new <name> <code> <economiczone>#0 - creates a new property	
	#3property edit <which>#0 - begins to edit a property
	#3property edit#0 - alias for PROPERTY SHOW <currently editing property>
	#3property close#0 - stops editing a property
	#3property set ...#0 - edits the properties of a property. See property set ? for more info.

The player version of this command is as follows:

The property command is used to buy, sell, lease and view real estate property. Most of the subcommands within this command need to be used at a location marked as a conveyancer or real estate agent. You will know when you are in such a location because a notification will appear below the room description.

If you are a property owner, you can can create ""Sale Orders"" and ""Lease Orders"" for your property. All property owners must consent to both of these. 

A Sale Order is an intention to fully sell all ownership interest in the property to any party that pays the listed price. All revenues from the sale will be paid into your nominated bank account.

A Lease Order is an intention to lease the property to a tenant. The Lease Order and the Lease itself are separate things; the Lease Order contains the rules for how NEW or RENEWED leases will be treated. Changing or cancelling a Lease Order does not cancel any Leases that are still valid.

Note that there are two closely related commands to this one you also need to be aware of. See the PROPERTIES command for a succinct listing of properties. Also see the CLAN PROPERTY command for some clan-related versions of these property commands.

The options you can use with this command are as follows:

	#3property preview <name>#0 - previews a property that you own, are leasing, or is for sale/lease
	#3property buy <name>#0 - buys a property that is for sale
	#3property lease <name>#0 - leases a property that is available for lease
	#3property tenant <name> add <target>#0 - adds someone as a listed tenant on your lease
	#3property tenant <name> remove <name>#0 - removes someone as a listed tenant on your lease
	#3property pay <name> <amount>#0 - pays an amount towards your rent or owing debts
	#3property pay <name> <amount> <bankcode>:<accn>#0 - pays an amount towards your rent or owing debts (using bank account)
	#3property returnkey <keyitem>#0 - returns a key you are holding
	#3property claimkeys <name>#0 - claims all returned keys associated with the specified property
	#3property returnbond <name>#0 - returns any unclaimed bond on an expired lease
	#3property claimshops <name>#0 - claims all shops associated with the specified property

The following commands are specific to those who own a property (or who are managing a clan-owned property):

	#3property setbank <name> <bankcode>:<account number>#0 - sets the bank account associated with any revenues you make on a property
	#3property sell <name> <price>#0 - lists a property that you own for sale
	#3property delaysale <name> <timespan>#0 - delays the sale of a property for the specific amount of time from now
	#3property delaysale <name> until <date>#0 - delays the sale of a property until a specific date
	#3property approvesale <name>#0 - gives your consent to a proposed sale of your property
	#3property addkey <name> <keyitem> <keyname> <replacement cost>#0 - adds a key to a property
	#3property remkey <name> <keyname>#0 - removes a key from a property
	#3property makelease <name> <price per 14 days> <bond>#0 - lists a property you own as available for leasing
	#3property interval <name> <interval>#0 - sets the payment interval for a lease, such as '2 weeks'
	#3property approvelease <name>#0 - gives your consent to lease a property you own
	#3property leaseprog <name> <prog> [character|clan]#0 - sets an optional prog to determine who can lease your property
	#3property leaseprog <name> none character|clan#0 - clears a prog controlling a lease
	#3property leaselength <name> <minimum length> <maximum length>#0 - changes the minimum and maximum lease lengths
	#3property autorenewlease <name>#0 - toggles whether a lease will automatically renew (true by default)
	#3property autorelist <name>#0 - toggles whether a lease will automatically relist (true by default)
	#3property allownovation <name>#0 - toggles whether a lesee can novate the lease to another (true by default)
	#3property bond <name> <bond>#0 - changes the bond required for a lease
	#3property rent <name> <price>#0 - changes the rent per interval for the lease
	#3property cancellease <name>#0 - cancels a lease order for the property
	#3property terminatelease <name>#0 - terminates a lease early when in default
	#3property claimbond <name> <amount>#0 - claims a deduction from bond owing to you";

	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[PlayerCommand("Property", "property")]
	[HelpInfo("property", PropertyHelpPlayers, AutoHelp.HelpArg, PropertyHelpAdmins)]
	[CustomModuleName("Economy")]
	protected static void Property(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "edit":
			case "close":
			case "clone":
			case "set":
			case "show":
			case "view":
			case "list":
				BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.PropertyHelper);
				return;
			case "preview":
				PropertyPreview(actor, ss);
				return;
			case "buy":
				PropertyBuy(actor, ss);
				return;
			case "sell":
				PropertySell(actor, ss);
				return;
			case "delaysell":
			case "delaysale":
			case "delayedsale":
			case "delayedsell":
				PropertyDelaySell(actor, ss);
				return;
			case "makelease":
				PropertyMakeLease(actor, ss);
				return;
			case "lease":
				PropertyLease(actor, ss);
				return;
			case "setbank":
				PropertySetBank(actor, ss);
				return;
			case "addkey":
				PropertyAddKey(actor, ss);
				return;
			case "remkey":
			case "removekey":
			case "unkey":
			case "deletekey":
			case "delkey":
				PropertyRemoveKey(actor, ss);
				return;
			case "returnkey":
			case "returnkeys":
				PropertyReturnKey(actor, ss);
				return;
			case "claimkeys":
			case "claimkey":
				PropertyClaimKeys(actor, ss);
				return;
			case "interval":
				PropertyInterval(actor, ss);
				return;
			case "leaseprog":
				PropertyLeaseProg(actor, ss);
				return;
			case "leaselength":
				PropertyLeaseLength(actor, ss);
				return;
			case "autorenewlease":
				PropertyAutoRenewLease(actor, ss);
				return;
			case "autorelist":
				PropertyAutoRelist(actor, ss);
				return;
			case "allownovation":
				PropertyAllowNovation(actor, ss);
				return;
			case "bond":
				PropertyBond(actor, ss);
				return;
			case "price":
			case "rent":
			case "rental":
				PropertyRent(actor, ss);
				return;
			case "terminatelease":
				PropertyTerminateLease(actor, ss);
				return;
			case "approvesale":
				PropertyApproveSale(actor, ss);
				return;
			case "approvelease":
				PropertyApproveLease(actor, ss);
				return;
			case "tenant":
				PropertyTenant(actor, ss);
				return;
			case "cancellease":
				PropertyCancelLease(actor, ss);
				return;
			case "pay":
			case "payrent":
				PropertyPayRent(actor, ss);
				return;
			case "returnbond":
				PropertyReturnBond(actor, ss);
				return;
			case "claimbond":
				PropertyClaimBond(actor, ss);
				return;
			case "divest":
			case "divestownership":
				PropertyDivestOwnership(actor, ss);
				return;
			case "claimshops":
				PropertyClaimShops(actor, ss);
				return;
			default:
				actor.OutputHandler.Send((actor.IsAdministrator() ? PropertyHelpAdmins : PropertyHelpPlayers)
					.SubstituteANSIColour());
				return;
		}
	}

	private static void PropertyClaimShops(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property do you want to claim the shops for? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x =>
			x.EconomicZone == ez &&
			(x.IsAuthorisedOwner(actor) || x.IsAuthorisedLeaseHolder(actor))
		).ToList();

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (!property.IsAuthorisedLeaseHolder(actor) && property.Lease is not null)
		{
			actor.OutputHandler.Send(
				"You cannot claim the shops while there is a lease on the property. The leaseholder is the only one who can claim the shops.");
			return;
		}

		var shops = property.PropertyLocations.SelectNotNull(x => x.Shop).Distinct().ToList();
		if (!shops.Any())
		{
			actor.OutputHandler.Send(
				$"The {property.Name.ColourName()} property does not have any shops associated with it.");
			return;
		}

		property.ClaimShops(actor);
		actor.OutputHandler.Send(
			$"You claim ownership of the {shops.Select(x => x.Name.ColourCommand()).ListToString()} {"shop".Pluralise(shops.Count != 1)} associated with the {property.Name.ColourName()} property.");
	}

	private static void PropertyDivestOwnership(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send(
				"Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to divest ownership of? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (property.PropertyOwners.All(x => x.Owner != actor))
		{
			actor.OutputHandler.Send(
				$"You do not personally own {property.Name.ColourName()}. If you want to divest ownership of a clan property, use the CLAN DIVEST command instead.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What percentage of your ownership stake in {property.Name.ColourName()} do you want to divest?");
			return;
		}

		if (!ss.PopSpeech().TryParsePercentageDecimal(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid percentage.");
			return;
		}

		if (value <= 0.0M || value > 1.0M)
		{
			actor.OutputHandler.Send(
				$"The percentage must be greater than {0.0.ToString("P", actor).ColourValue()} and less than or equal to {1.0.ToString("P", actor).ColourValue()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"To whom do you want to divest ownership in {property.Name.ColourName()}?");
			return;
		}

		var text = ss.SafeRemainingArgument;
		if (text[0] == '*' && text.Length > 1)
		{
			text = text.Substring(1);
			var clan = actor.Gameworld.Clans.GetClan(text);
			if (clan == null)
			{
				actor.OutputHandler.Send($"There is no such clan as {text.ColourCommand()}.");
				return;
			}

			actor.OutputHandler.Send(
				$"Are you sure that you want to divest {value.ToString("P", actor).ColourValue()} of your ownership in {property.Name.ColourName()} to the clan {clan.FullName.ColourName()}?\n{Accept.StandardAcceptPhrasing}");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				AcceptAction = text =>
				{
					if (property.PropertyOwners.All(x => x.Owner != actor))
					{
						actor.OutputHandler.Send(
							$"You no longer own any part of the property {property.Name.ColourName()}.");
						return;
					}

					actor.OutputHandler.Handle(new EmoteOutput(new Emote(
						"@ divest|divests $1 of &0's ownership of $2 to $3.", actor, actor,
						new DummyPerceivable(voyeur => value.ToString("P", voyeur).ColourValue()),
						new DummyPerceivable($"the property {property.Name}", customColour: Telnet.Cyan),
						new DummyPerceivable($"the clan {clan.FullName}", customColour: Telnet.Cyan))));
					property.DivestOwnership(property.PropertyOwners.First(x => x.Owner == actor), value, clan);
				},
				RejectAction = text =>
				{
					actor.OutputHandler.Send(
						$"You decide not to divest your ownership in the property {property.Name.ColourName()}.");
				},
				ExpireAction = () =>
				{
					actor.OutputHandler.Send(
						$"You decide not to divest your ownership in the property {property.Name.ColourName()}.");
				},
				Keywords = new List<string> { "divest", "property", "ownership" },
				DescriptionString = $"divesting ownership of the {property.Name} property"
			}), 120.Seconds());
			return;
		}

		var target = actor.TargetActor(text, PerceiveIgnoreFlags.IgnoreSelf);
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure that you want to divest {value.ToString("P", actor).ColourValue()} of your ownership in {property.Name.ColourName()} to {target.HowSeen(actor)}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (property.PropertyOwners.All(x => x.Owner != actor))
				{
					actor.OutputHandler.Send(
						$"You no longer own any part of the property {property.Name.ColourName()}.");
					return;
				}

				actor.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ divest|divests $1 of &0's ownership of $2 to $3.", actor, actor,
					new DummyPerceivable(voyeur => value.ToString("P", voyeur).ColourValue()),
					new DummyPerceivable($"the property {property.Name}", customColour: Telnet.Cyan), target)));
				property.DivestOwnership(property.PropertyOwners.First(x => x.Owner == actor), value, target);
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to divest your ownership in the property {property.Name.ColourName()}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to divest your ownership in the property {property.Name.ColourName()}.");
			},
			Keywords = new List<string> { "divest", "property", "ownership" },
			DescriptionString = $"divesting ownership of the {property.Name} property"
		}), 120.Seconds());
	}

	private static void PropertyClaimKeys(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property do you want to claim the keys for? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x =>
			x.EconomicZone == ez &&
			(x.IsAuthorisedOwner(actor) || x.IsAuthorisedLeaseHolder(actor))
		).ToList();

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (!property.IsAuthorisedLeaseHolder(actor) && property.Lease is not null)
		{
			actor.OutputHandler.Send(
				"You cannot claim the keys while there is a lease on the property. The leaseholder is the only one who can claim the keys.");
			return;
		}

		var keys = property.PropertyKeys.Where(x => x.IsReturned).ToList();
		if (keys.Count == 0)
		{
			actor.OutputHandler.Send($"There are no keys to collect for the {property.Name.ColourName()} property.");
			return;
		}

		foreach (var key in keys)
		{
			key.GameItem.Login();
			key.IsReturned = false;
		}

		IGameItem givenItem;
		if (keys.Count == 1)
		{
			givenItem = keys.Single().GameItem;
		}
		else
		{
			givenItem = PileGameItemComponentProto.CreateNewBundle(keys.Select(x => x.GameItem));
			actor.Gameworld.Add(givenItem);
		}

		if (actor.Body.CanGet(givenItem, 0))
		{
			actor.Body.Get(givenItem, silent: true);
		}
		else
		{
			actor.Location.Insert(givenItem, true);
			actor.OutputHandler.Send("You couldn't hold the keys, so they have been put on the ground.");
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ claim|claims the keys for $2, and are|is given $1.",
			actor, actor, givenItem, new DummyPerceivable($"the property {property.Name.ColourName()}"))));
	}

	private static void PropertyClaimBond(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to make a claim on the bond for? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (property.Lease == null)
		{
			actor.OutputHandler.Send(
				$"The {property.Name.ColourName()} property does not have an existing lease on it.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How much bond do you want to claim back from the leaseholder of {property.Name.ColourName()}?");
			return;
		}

		if (!property.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var amount) || amount < 0.0M)
		{
			actor.OutputHandler.Send(
				$"The amount {ss.Last.ColourCommand()} is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
			return;
		}

		if (amount > property.Lease.BondPayment)
		{
			actor.OutputHandler.Send(
				$"You cannot claim more than the total bond held, which is {ez.Currency.Describe(property.Lease.BondPayment, CurrencyDescriptionPatternType.Short).ColourValue()}.");
			return;
		}

		property.Lease.BondClaimed = amount;
		actor.OutputHandler.Send(
			$"You claim an amount of {ez.Currency.Describe(property.Lease.BondPayment, CurrencyDescriptionPatternType.Short).ColourValue()} from the bond on the current lease of {property.Name.ColourName()}. This will be extracted from the bond returned to the leaseholder at the end of the bond."
				.Wrap(actor.InnerLineFormatLength));
	}

	private static void PropertyReturnBond(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which property do you want to return your bond from?");
			return;
		}

		var properties = actor.Gameworld.Properties
		                      .Where(x => x.EconomicZone == ez && x.HasUnclaimedBondPayments(actor))
		                      .ToList();
		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not have any bond owing from any such property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var lease = property.ExpiredLeases.First(y =>
			!y.BondReturned &&
			y.IsAuthorisedLeaseHolder(actor));
		if (property.PropertyKeys.Any(x => !x.IsReturned && x.AddedToPropertyOnDate < lease.LeaseStart))
		{
			actor.OutputHandler.Send(
				$"Warning: You have unreturned keys for this property. If you proceed, the cost of replacing them will be deducted from your bond. Would like to proceed?\n{Accept.StandardAcceptPhrasing}");
		}

		var keys = property.PropertyKeys.Where(x => !x.IsReturned && x.AddedToPropertyOnDate < lease.LeaseStart)
		                   .ToList();
		var keyCost = keys.Sum(x => x.CostToReplace);
		var bondReturned = lease.BondPayment + lease.PaymentBalance - lease.BondClaimed - keyCost;
		if (bondReturned <= 0.0M)
		{
			var reasons = new List<string>();
			if (lease.PaymentBalance < 0.0M)
			{
				reasons.Add(
					$"your outstanding balance owed of {property.EconomicZone.Currency.Describe(lease.PaymentBalance.InvertSign(), CurrencyDescriptionPatternType.Short).ColourValue()}");
			}

			if (lease.BondClaimed > 0.0M)
			{
				reasons.Add(
					$"your landlord's claim of {property.EconomicZone.Currency.Describe(lease.PaymentBalance.InvertSign(), CurrencyDescriptionPatternType.Short).ColourValue()}");
			}

			if (keyCost > 0.0M)
			{
				reasons.Add(
					$"a charge of {property.EconomicZone.Currency.Describe(keyCost, CurrencyDescriptionPatternType.Short).ColourValue()} for replacing missing keys");
			}

			lease.BondReturned = true;
			actor.OutputHandler.Send($"You did not have any bond to claim because of {reasons.ListToString()}.");
			return;
		}

		foreach (var key in keys)
		{
			key.GameItem = key.GameItem.DeepCopy(true);
			key.IsReturned = true;
		}

		var cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(
			property.EconomicZone.Currency, property.EconomicZone.Currency.FindCoinsForAmount(bondReturned, out _));
		actor.Gameworld.Add(cash);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ receive|receives $1 as a return of bond from &0's lease on {property.Name.ColourName()}.", actor, actor,
			new DummyPerceivable(
				property.EconomicZone.Currency.Describe(bondReturned, CurrencyDescriptionPatternType.Short)))));

		if (actor.Body.CanGet(cash, 0))
		{
			actor.Body.Get(cash, silent: true);
		}
		else
		{
			cash.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(cash, true);
			actor.OutputHandler.Send("You couldn't hold the money, so it is on the ground.");
		}
	}

	private static void PropertyPayRent(ICharacter actor, StringStack ss)
	{
		var properties = actor.Gameworld.Properties.Where(x => x.IsAuthorisedLeaseHolder(actor)).ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to pay rent on? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You are not the leaseholder of such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null && !property.PropertyLocations.Contains(actor.Location))
		{
			actor.OutputHandler.Send("You can only pay rent from the property or at a conveyancing location.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"How much rent do you want to pay towards {property.Name.ColourName()}?");
			return;
		}

		if (!property.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var amount))
		{
			actor.OutputHandler.Send(
				$"The amount {ss.Last.ColourCommand()} is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
			return;
		}

		// Cash payment
		if (ss.IsFinished)
		{
			var payment = new OtherCashPayment(property.EconomicZone.Currency, actor);
			if (payment.AccessibleMoneyForPayment() < amount)
			{
				actor.OutputHandler.Send(
					$"You aren't holding enough money to pay {property.EconomicZone.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} in rent for {property.Name.ColourValue()}.\nYou are only holding {property.EconomicZone.Currency.Describe(payment.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()}.");
				return;
			}

			payment.TakePayment(amount);
		}
		else
		{
			var (account, error) = Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
			if (account == null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (!account.IsAuthorisedAccountUser(actor))
			{
				actor.OutputHandler.Send("You are not an authorised user for that bank account.");
				return;
			}

			var (truth, accountError) = account.CanWithdraw(amount, true);
			if (!truth)
			{
				actor.OutputHandler.Send(accountError);
				return;
			}

			account.WithdrawFromTransaction(amount, $"Paid Rent for {property.Name}");
			account.Bank.CurrencyReserves[property.EconomicZone.Currency] -= amount;
		}

		foreach (var owner in property.PropertyOwners)
		{
			if (owner.RevenueAccount != null)
			{
				owner.RevenueAccount.DepositFromTransaction(amount * owner.ShareOfOwnership,
					$"Rent payment from {property.Name}");
				owner.RevenueAccount.Bank.CurrencyReserves[property.EconomicZone.Currency] +=
					amount * owner.ShareOfOwnership;
			}
		}

		property.Lease!.PaymentBalance += amount;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ pay|pays $1 towards rent on the property {property.Name.ColourName()}.", actor, actor,
			new DummyPerceivable(
				property.EconomicZone.Currency.Describe(amount, CurrencyDescriptionPatternType.Short)))));
	}

	[CanBeNull]
	private static IProperty FindPropertyWithLeaseOrder(ICharacter actor, StringStack ss, string subcommand)
	{
		string actionDescription;
		switch (subcommand)
		{
			case "length":
				actionDescription = "set the minimum and maximum lease of";
				break;
			case "renew":
				actionDescription = "toggle automatic renewing of leases of";
				break;
			case "relist":
				actionDescription = "toggle the automatic re-listing of";
				break;
			case "novation":
				actionDescription = "toggle the ability to novate leases of";
				break;
			case "bond":
				actionDescription = "set the required bond of";
				break;
			case "rent":
				actionDescription = "set the rent of";
				break;
			case "prog":
				actionDescription = "set the filtering prog of";
				break;
			case "interval":
				actionDescription = "set the rent interval of";
				break;
			case "approve":
				actionDescription = "approve the lease of";
				break;
			case "cancel":
				actionDescription = "cancel the lease order of";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(subcommand));
		}

		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return null;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to {actionDescription}? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return null;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return null;
		}

		if (property.LeaseOrder == null)
		{
			actor.OutputHandler.Send(
				$"The property {property.Name.ColourName()} is not proposed to be listed for lease.");
			return null;
		}

		return property;
	}

	private static void PropertyTenantAddCharacter(ICharacter actor, StringStack ss, IProperty property)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Who do you want to add as a tenant to the {property.Name.ColourName()} property?");
			return;
		}

		var target = actor.TargetActor(ss.SafeRemainingArgument, PerceiveIgnoreFlags.IgnoreSelf);
		if (target == null)
		{
			actor.OutputHandler.Send(
				$"You do not see anyone like that to add as a tenant to the {property.Name.ColourName()} property.");
			return;
		}

		if (property.Lease!.IsTenant(target, true))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true)} is already a declared tenant of the {property.Name.ColourName()} property.");
			return;
		}

		property.Lease.DeclareTenant(target);
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ declare|declares $1 as an authorised tenant of the {property.Name.ColourName()} property.",
				actor, actor, target), flags: OutputFlags.SuppressObscured));
	}

	private static void PropertyTenantAddClan(ICharacter actor, StringStack ss, IProperty property)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which clan do you want to add as a tenant to the {property.Name.ColourName()} property?");
			return;
		}

		actor.OutputHandler.Send("Coming soon.");
	}

	private static void PropertyTenantAddShop(ICharacter actor, StringStack ss, IProperty property)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which shop do you want to add as a tenant to the {property.Name.ColourName()} property?");
			return;
		}

		actor.OutputHandler.Send("Coming soon.");
	}

	private static void PropertyTenantRemoveClan(ICharacter actor, StringStack ss, IProperty property)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which clan you want to remove as a tenant from the {property.Name.ColourName()} property?");
			return;
		}

		var target = property.Lease!.DeclaredTenants.OfType<IClan>()
		                     .GetClan(ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send(
				$"There are no existing declared clan tenants like that to remove as a tenant from the {property.Name.ColourName()} property.");
			return;
		}

		if (!property.Lease!.DeclaredTenants.Contains(target))
		{
			actor.OutputHandler.Send(
				$"{target.FullName.ColourName()} is not a declared clan tenant of the {property.Name.ColourName()} property.");
			return;
		}

		property.Lease.RemoveTenant(target);
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ remove|removes $1 as an authorised tenant of the {property.Name.ColourName()} property.",
				actor, actor, new DummyPerceivable(target.FullName, customColour: Telnet.Cyan)),
			flags: OutputFlags.SuppressObscured));
	}

	private static void PropertyTenantRemoveShop(ICharacter actor, StringStack ss, IProperty property)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which clan you want to remove as a tenant from the {property.Name.ColourName()} property?");
			return;
		}

		var target = property.Lease!.DeclaredTenants.OfType<IShop>()
		                     .GetByNameOrAbbreviation(ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send(
				$"There are no existing declared shop tenants like that to remove as a tenant from the {property.Name.ColourName()} property.");
			return;
		}

		if (!property.Lease!.DeclaredTenants.Contains(target))
		{
			actor.OutputHandler.Send(
				$"{target.Name.ColourName()} is not a declared shop tenant of the {property.Name.ColourName()} property.");
			return;
		}

		property.Lease.RemoveTenant(target);
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ remove|removes $1 as an authorised tenant of the {property.Name.ColourName()} property.",
				actor, actor, new DummyPerceivable(target.Name, customColour: Telnet.Cyan)),
			flags: OutputFlags.SuppressObscured));
	}

	private static void PropertyTenantRemoveCharacter(ICharacter actor, StringStack ss, IProperty property)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Who do you want to remove as a tenant from the {property.Name.ColourName()} property?");
			return;
		}

		var target = property.Lease!.DeclaredTenants.OfType<ICharacter>()
		                     .GetFromItemListByKeywordIncludingNames(ss.SafeRemainingArgument, actor);
		if (target == null)
		{
			actor.OutputHandler.Send(
				$"There are no existing declared tenants like that to remove as a tenant from the {property.Name.ColourName()} property.");
			return;
		}

		if (property.Lease!.IsTenant(target, true))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true)} is not a declared tenant of the {property.Name.ColourName()} property.");
			return;
		}

		property.Lease.RemoveTenant(target);
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ remove|removes $1 as an authorised tenant of the {property.Name.ColourName()} property.",
				actor, actor, target), flags: OutputFlags.SuppressObscured));
	}

	private static void PropertyTenantList(ICharacter actor, StringStack ss, IProperty property)
	{
		var tenants = property.Lease!.DeclaredTenants.ToList();
		if (!tenants.Any())
		{
			actor.OutputHandler.Send($"The {property.Name.ColourName()} property has no declared tenants.");
			return;
		}

		var sb = new StringBuilder();
		foreach (var tenant in tenants)
		{
			if (tenant is ICharacter ch)
			{
				sb.AppendLine($"\t{ch.PersonalName.GetName(NameStyle.FullName).ColourCharacter()}");
				continue;
			}

			if (tenant is IClan clan)
			{
				sb.AppendLine($"\t{clan.FullName.ColourName()} [clan]");
				continue;
			}

			if (tenant is IShop shop)
			{
				sb.AppendLine($"\t{shop.Name.ColourName()} [shop]");
			}
		}

		actor.OutputHandler.Send($"The {property.Name.ColourName()} property has the following listed tenants:\n{sb}");
	}

	private static void PropertyTenant(ICharacter actor, StringStack ss)
	{
		var properties = actor.Gameworld.Properties.Where(x => x.IsAuthorisedLeaseHolder(actor)).ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to change declared tenants of? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You are not the leaseholder of such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var add = true;
		switch (ss.PopSpeech())
		{
			case "add":
				PropertyTenantAddCharacter(actor, ss, property);
				return;
			case "addclan":
				PropertyTenantAddClan(actor, ss, property);
				return;
			case "addshop":
				PropertyTenantAddShop(actor, ss, property);
				return;
			case "remove":
			case "delete":
			case "rem":
			case "del":
				PropertyTenantRemoveCharacter(actor, ss, property);
				return;
			case "removeclan":
			case "remclan":
			case "deleteclan":
			case "delclan":
				PropertyTenantRemoveClan(actor, ss, property);
				return;
			case "removeshop":
			case "remshop":
			case "deleteshop":
			case "delshop":
				PropertyTenantRemoveShop(actor, ss, property);
				return;
			case "list":
				PropertyTenantList(actor, ss, property);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command: 

	#3list#0 - lists the current tenants for this property
	#3add <target>#0 - adds a character that you can see as a tenant
	#3remove <target|name> - removes a character as a tenant
	#3addclan <clan>#0 - adds a clan (and their members) as a tenant
	#3removeclan <clan>#0 - removes a clan as a tenant
	#3addshop <shop>#0 - adds a shop and their employees as a tenant
	#3removeshop <shop>#0 - removes a shop as a tenant".SubstituteANSIColour());
				return;
		}
	}

	private static void PropertyLeaseLength(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "length");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What do you want to be the minimum length of lease for the property {property.Name.ColourName()}?");
			return;
		}

		if (!TimeSpan.TryParse(ss.PopSpeech(), actor, out var minimum))
		{
			actor.OutputHandler.Send(
				$"That is not a valid timespan. Generally speaking the format is days:hours:minutes:seconds and it matches the way your account culture ({actor.Account.Culture.Name.ColourValue()}) handles timespans.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What do you want to be the maximum length of lease for the property {property.Name.ColourName()}?");
			return;
		}

		if (!TimeSpan.TryParse(ss.PopSpeech(), actor, out var maximum))
		{
			actor.OutputHandler.Send(
				$"That is not a valid timespan. Generally speaking the format is days:hours:minutes:seconds and it matches the way your account culture ({actor.Account.Culture.Name.ColourValue()}) handles timespans.");
			return;
		}

		if (minimum <= TimeSpan.Zero || maximum <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send("You must have lengths of time that are positive and not zero.");
			return;
		}

		if (minimum > maximum)
		{
			actor.OutputHandler.Send("The minimum duration may not be longer than the maximum duration.");
			return;
		}

		property.LeaseOrder!.MinimumLeaseDuration = minimum;
		property.LeaseOrder.MaximumLeaseDuration = maximum;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will now accept leases of between {minimum.Describe(actor).ColourValue()} and {maximum.Describe(actor).ColourValue()} in duration.");
	}

	private static void PropertyAutoRenewLease(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "renew");
		if (property == null)
		{
			return;
		}

		property.LeaseOrder!.AllowAutoRenew = !property.LeaseOrder.AllowAutoRenew;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will {(property.LeaseOrder.AllowAutoRenew ? "now" : "no longer")} allow auto-renewal of leases at the conclusion of their term.");
	}

	private static void PropertyAutoRelist(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "relist");
		if (property == null)
		{
			return;
		}

		property.LeaseOrder!.AutomaticallyRelistAfterLeaseTerm = !property.LeaseOrder.AutomaticallyRelistAfterLeaseTerm;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will {(property.LeaseOrder.AutomaticallyRelistAfterLeaseTerm ? "now" : "no longer")} be automatically re-listed as available at the expiry of an agreed lease.");
	}

	private static void PropertyAllowNovation(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "novation");
		if (property == null)
		{
			return;
		}

		property.LeaseOrder!.AllowLeaseNovation = !property.LeaseOrder.AllowLeaseNovation;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will {(property.LeaseOrder.AllowLeaseNovation ? "now" : "no longer")} permit novation of the lease to other individuals or organisations.");
	}

	private static void PropertyBond(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "bond");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What amount of {property.EconomicZone.Currency.Name.ColourName()} do you want to set as the bond for the lease order?");
			return;
		}

		if (!property.EconomicZone.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"The quantity {ss.SafeRemainingArgument.ColourCommand()} is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
			return;
		}

		property.LeaseOrder!.BondRequired = value;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will now require a bond of {property.EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.Short).ColourValue()}.");
	}

	private static void PropertyRent(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "rent");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What amount of {property.EconomicZone.Currency.Name.ColourName()} do you want to set as the rent for the lease order?");
			return;
		}

		if (!property.EconomicZone.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"The quantity {ss.SafeRemainingArgument.ColourCommand()} is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
			return;
		}

		property.LeaseOrder!.PricePerInterval = value;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will now charge rent of {property.EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.Short).ColourValue()}.");
	}

	private static void PropertyLeaseProg(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "prog");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a prog, or use the keyword {"none".ColourCommand()} to clear an existing prog.");
			return;
		}

		var text = ss.PopSpeech();
		if (text.EqualTo("none"))
		{
			if (ss.IsFinished)
			{
				if (property.LeaseOrder!.CanLeaseProgCharacter is not null &&
				    property.LeaseOrder.CanLeaseProgClan is null)
				{
					property.LeaseOrder.CanLeaseProgCharacter = null;
					actor.OutputHandler.Send(
						$"The {property.Name.ColourName()} property will no longer use a prog to determine which characters can lease it.");
					return;
				}

				if (property.LeaseOrder.CanLeaseProgClan is not null &&
				    property.LeaseOrder.CanLeaseProgCharacter is null)
				{
					property.LeaseOrder.CanLeaseProgClan = null;
					actor.OutputHandler.Send(
						$"The {property.Name.ColourName()} property will no longer use a prog to determine which clans can lease it.");
					return;
				}

				actor.OutputHandler.Send(
					$"You must specify either {"character".ColourCommand()} or {"clan".ColourCommand()} for which prog you want to remove.");
				return;
			}

			switch (ss.PopSpeech().ToLowerInvariant())
			{
				case "character":
				case "person":
				case "char":
				case "ch":
					property.LeaseOrder!.CanLeaseProgCharacter = null;
					actor.OutputHandler.Send(
						$"The {property.Name.ColourName()} property will no longer use a prog to determine which characters can lease it.");
					return;
				case "clan":
					property.LeaseOrder!.CanLeaseProgClan = null;
					actor.OutputHandler.Send(
						$"The {property.Name.ColourName()} property will no longer use a prog to determine which clans can lease it.");
					return;
				default:
					actor.OutputHandler.Send(
						$"You must specify either {"character".ColourCommand()} or {"clan".ColourCommand()} for which prog you want to remove.");
					return;
			}
		}

		var prog = actor.Gameworld.FutureProgs.GetByIdOrName(text);
		if (prog == null)
		{
			actor.OutputHandler.Send($"There is no such prog as {text.ColourCommand()}.");
			return;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, and {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Clan }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that takes either a single character or clan, and {prog.MXPClickableFunctionName()} does not.");
			return;
		}

		if (prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
		{
			property.LeaseOrder!.CanLeaseProgCharacter = prog;
			actor.OutputHandler.Send(
				$"The {property.Name.ColourName()} property will now use the {prog.MXPClickableFunctionName()} prog to determine which characters can lease it.");
			return;
		}

		property.LeaseOrder!.CanLeaseProgClan = prog;
		actor.OutputHandler.Send(
			$"The {property.Name.ColourName()} property will now use the {prog.MXPClickableFunctionName()} prog to determine which clans can lease it.");
	}

	private static void PropertyInterval(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "interval");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What interval do you want to charge rent for this property at?");
			return;
		}

		if (!RecurringInterval.TryParse(ss.SafeRemainingArgument, out var interval))
		{
			actor.OutputHandler.Send(
				$"That is not a valid interval.\n{"Use the following form: every <x> hours|days|weekdays|weeks|months|years <offset>".ColourCommand()}");
			return;
		}

		property.LeaseOrder!.Interval = interval;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will now charge rent {interval.Describe(property.EconomicZone.FinancialPeriodReferenceCalendar).ColourValue()}.");
	}

	private static void PropertyAddKey(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to add a key for? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which item are you looking to add as a key for the {property.Name.ColourName()} property?");
			return;
		}

		var target = actor.TargetHeldItem(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You are not holding anything like that.");
			return;
		}

		if (actor.Gameworld.Properties.Any(x => x.PropertyKeys.Any(y => y.GameItem == target)))
		{
			actor.OutputHandler.Send(
				$"Unfortunately, {target.HowSeen(actor)} is already a key for another property. Items can only be keys for one property.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this property key?");
			return;
		}

		var name = ss.PopSpeech();

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"How much should it cost to replace this key if a tenant loses it?");
			return;
		}

		if (!ez.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid amount of {ez.Currency.Name.ColourName()}.");
			return;
		}

		property.AddKey(new PropertyKey(actor.Gameworld, property, name, target,
			ez.FinancialPeriodReferenceCalendar.CurrentDateTime, amount));
		actor.Body.Take(target);
		target.Quit();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ add|adds $1 as a key for the $2 property, named \"$3\" and costing $4 to replace.", actor, actor, target,
			new DummyPerceivable(property.Name), new DummyPerceivable(name),
			new DummyPerceivable(ez.Currency.Describe(amount, CurrencyDescriptionPatternType.Short)))));
	}

	private static void PropertyReturnKey(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which key do you want to return?");
			return;
		}

		var target = actor.TargetHeldItem(ss.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("You aren't holding anything like that.");
			return;
		}

		var property =
			actor.Gameworld.Properties.FirstOrDefault(x => x.PropertyKeys.Any(y => y.GameItem == target));
		if (property == null)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not a key for any property.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ return|returns $1, a key for property $2.", actor,
			actor, target, new DummyPerceivable(property.Name))));
		actor.Body.Take(target);
		target.Quit();
		property.PropertyKeys.First(x => x.GameItem == target).IsReturned = true;
	}

	private static void PropertyRemoveKey(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to remove a key from? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which key do you want to remove from the {property.Name.ColourName()} property?\nYour options are: {property.PropertyKeys.Select(x => x.Name.ColourValue()).Humanize()}");
			return;
		}

		var key = property.PropertyKeys.GetByNameOrAbbreviation(ss.SafeRemainingArgument);
		if (key == null)
		{
			actor.OutputHandler.Send(
				$"That is not the name of a key for the {property.Name.ColourName()} property?\nYour options are: {property.PropertyKeys.Select(x => x.Name.ColourValue()).Humanize()}");
			return;
		}

		property.RemoveKey(key);
		key.GameItem.Login();
		key.GameItem.RoomLayer = actor.RoomLayer;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ remove|removes the $1 key for property $2.", actor,
			actor, new DummyPerceivable(key.Name), new DummyPerceivable(property.Name))));
		if (actor.Body.CanGet(key.GameItem, 0))
		{
			actor.Body.Get(key.GameItem, silent: true);
		}
		else
		{
			actor.Location.Insert(key.GameItem, true);
			actor.OutputHandler.Send($"You were unable to hold {key.GameItem.HowSeen(actor)}, so it is on the ground.");
		}

		key.Delete();
	}

	private static void PropertySetBank(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to change the owner bank account details for? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which bank account do you want to set for your revenues with the {property.Name.ColourName()} property?");
			return;
		}

		var (accountTarget, error) = Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
		if (accountTarget == null)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!accountTarget.IsAuthorisedAccountUser(actor))
		{
			actor.OutputHandler.Send("You are not an authorised user for that bank account.");
			return;
		}

		foreach (var owner in property.PropertyOwners)
		{
			if (owner.Owner == actor || (owner.Owner is IClan clan && actor.ClanMemberships.Any(x =>
				    x.Clan == clan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty))))
			{
				owner.RevenueAccount = accountTarget;
			}
		}

		actor.OutputHandler.Send(
			$"You set your revenue account for {property.Name.ColourName()} to the account {$"{accountTarget.Bank.Code}:{accountTarget.AccountNumber}".ColourValue()}.");
	}

	private static void PropertyLease(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property do you want to lease? Please see {"PROPERTIES".MXPSend("properties", "The properties command")} for a list of properties.");
			return;
		}

		var properties = actor.Gameworld.Properties
		                      .Where(x => x.EconomicZone == ez && x.LeaseOrder?.ListedForLease == true).ToList();
		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send("There is no such property currently available for lease.");
			return;
		}

		if (property.Lease is not null)
		{
			actor.OutputHandler.Send(
				$"The {property.Name.ColourName()} property is already leased until {property.Lease.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How long do you want to lease this property for?\nYou may lease for any duration between {property.LeaseOrder!.MinimumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()} and {property.LeaseOrder!.MaximumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()}.");
			return;
		}

		if (!TimeSpanParser.TryParse(ss.PopSpeech(), Units.Weeks, Units.Weeks, out var duration))
		{
			actor.OutputHandler.Send(
				$"That was not a valid time span.\n{"Note: Years and Months are not supported, use Weeks or Days in that case".ColourCommand()}");
			return;
		}

		if (duration < property.LeaseOrder!.MinimumLeaseDuration ||
		    duration > property.LeaseOrder.MaximumLeaseDuration)
		{
			actor.OutputHandler.Send(
				$"You may only lease for a duration between {property.LeaseOrder!.MinimumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()} and {property.LeaseOrder!.MinimumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()}.");
			return;
		}

		var amount = property.LeaseOrder!.BondRequired + 2.0M * property.LeaseOrder.PricePerInterval;
		// Cash payment
		if (ss.IsFinished)
		{
			var payment = new OtherCashPayment(ez.Currency, actor);
			if (payment.AccessibleMoneyForPayment() < amount)
			{
				actor.OutputHandler.Send(
					$"You aren't holding enough money to lease {property.Name.ColourValue()} as it costs {ez.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()}\nNote: This amount is a minimum of two payment periods in rent plus bond.\nYou are only holding {ez.Currency.Describe(payment.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()}.");
				return;
			}

			payment.TakePayment(amount);
		}
		else
		{
			var (account, error) = Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
			if (account == null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (!account.IsAuthorisedAccountUser(actor))
			{
				actor.OutputHandler.Send("You are not an authorised user for that bank account.");
				return;
			}

			var (truth, accountError) = account.CanWithdraw(amount, true);
			if (!truth)
			{
				actor.OutputHandler.Send(accountError);
				return;
			}

			account.WithdrawFromTransaction(amount, $"Lease of {property.Name}");
			account.Bank.CurrencyReserves[ez.Currency] -= amount;
		}

		var depositAmount = amount - property.LeaseOrder.BondRequired;

		foreach (var owner in property.PropertyOwners)
		{
			if (owner.RevenueAccount != null)
			{
				owner.RevenueAccount.DepositFromTransaction(depositAmount * owner.ShareOfOwnership,
					$"Lease of {property.Name}");
				owner.RevenueAccount.Bank.CurrencyReserves[ez.Currency] += depositAmount * owner.ShareOfOwnership;
			}
		}

		property.Lease = property.LeaseOrder.CreateLease(actor, duration);
		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ lease|leases the property {property.Name.ColourName()}.", actor, actor)));
		if (property.PropertyKeys.Any())
		{
			IGameItem givenItem;
			if (property.PropertyKeys.Count() == 1)
			{
				givenItem = property.PropertyKeys.Single().GameItem;
				property.PropertyKeys.Single().IsReturned = false;
				givenItem.Login();
			}
			else
			{
				var list = new List<IGameItem>();
				foreach (var key in property.PropertyKeys)
				{
					list.Add(key.GameItem);
					key.IsReturned = false;
					key.GameItem.Login();
				}

				givenItem = PileGameItemComponentProto.CreateNewBundle(list);
				actor.Gameworld.Add(givenItem);
			}

			if (actor.Body.CanGet(givenItem, 0))
			{
				actor.Body.Get(givenItem, silent: true);
			}
			else
			{
				givenItem.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(givenItem, true);
				actor.OutputHandler.Send($"You couldn't hold {givenItem.HowSeen(actor)}, so it is on the ground.");
			}
		}
	}

	private static void PropertyTerminateLease(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to terminate the lease of? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (property.Lease == null)
		{
			actor.OutputHandler.Send(
				$"The {property.Name.ColourName()} property does not have an existing lease on it.");
			return;
		}

		if (property.Lease.PaymentBalance > 0.0M)
		{
			actor.OutputHandler.Send($"You may not terminate the lease of a property which is not in arrears.");
			return;
		}

		var lease = property.Lease;
		actor.OutputHandler.Send(
			$"Are you sure you want to terminate the existing lease on {property.Name.ColourName()}? Make sure that you claim any necessary bond for property damages BEFORE you terminate the lease.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Terminating a lease",
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You terminate the lease on {property.Name.ColourName()}.");
				lease.TerminateLease();
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to terminate the lease on the {property.Name.ColourName()} property.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to terminate the lease on the {property.Name.ColourName()} property.");
			},
			Keywords = new List<string> { "terminate", "lease" }
		}), 120.Seconds());
	}

	private static void PropertyPreview(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property do you want to preview? Please see {"PROPERTIES".MXPSend("properties", "The properties command")} for a list of properties.");
			return;
		}

		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		var properties = actor.Gameworld.Properties.Where(x =>
			x.EconomicZone == ez ||
			x.PropertyOwners.Any(y => y.Owner.Equals(actor)) ||
			x.Lease?.Leaseholder.Equals(actor) == true
		).ToList();

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.SafeRemainingArgument, actor);
		if (property == null)
		{
			actor.OutputHandler.Send("You do not own, lease or see listed any such property.");
			return;
		}

		actor.OutputHandler.Send(property.PreviewProperty(actor));
	}

	private static void PropertyBuy(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.SaleOrder?.ShowForSale == true)
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to buy? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"There is no such property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var amount = property.SaleOrder!.ReservePrice;
		// Cash payment
		if (ss.IsFinished)
		{
			var payment = new OtherCashPayment(ez.Currency, actor);
			if (payment.AccessibleMoneyForPayment() < amount)
			{
				actor.OutputHandler.Send(
					$"You aren't holding enough money to purchase {property.Name.ColourValue()} as it costs {ez.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()}.\nYou are only holding {ez.Currency.Describe(payment.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()}.");
				return;
			}

			payment.TakePayment(amount);
		}
		else
		{
			var (account, error) = Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
			if (account == null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (!account.IsAuthorisedAccountUser(actor))
			{
				actor.OutputHandler.Send("You are not an authorised user for that bank account.");
				return;
			}

			var (truth, accountError) = account.CanWithdraw(amount, true);
			if (!truth)
			{
				actor.OutputHandler.Send(accountError);
				return;
			}

			account.WithdrawFromTransaction(amount, $"Purchase of {property.Name}");
			account.Bank.CurrencyReserves[ez.Currency] -= amount;
		}

		foreach (var owner in property.PropertyOwners)
		{
			if (owner.RevenueAccount != null)
			{
				owner.RevenueAccount.DepositFromTransaction(amount * owner.ShareOfOwnership,
					$"Sale of {property.Name}");
				owner.RevenueAccount.Bank.CurrencyReserves[ez.Currency] += amount * owner.ShareOfOwnership;
			}
		}

		property.SellProperty(actor);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ purchase|purchases the property {property.Name.ColourName()} for $1.", actor, actor,
			new DummyPerceivable(ez.Currency.Describe(amount, CurrencyDescriptionPatternType.Short)))));

		if (property.PropertyKeys.Any(x => !x.IsReturned))
		{
			IGameItem givenItem;
			if (property.PropertyKeys.Count(x => !x.IsReturned) == 1)
			{
				givenItem = property.PropertyKeys.Single(x => !x.IsReturned).GameItem;
				property.PropertyKeys.Single(x => !x.IsReturned).IsReturned = false;
				givenItem.Login();
			}
			else
			{
				var list = new List<IGameItem>();
				foreach (var key in property.PropertyKeys.Where(x => !x.IsReturned))
				{
					list.Add(key.GameItem);
					key.IsReturned = false;
					key.GameItem.Login();
				}

				givenItem = PileGameItemComponentProto.CreateNewBundle(list);
				actor.Gameworld.Add(givenItem);
			}

			if (actor.Body.CanGet(givenItem, 0))
			{
				actor.Body.Get(givenItem, silent: true);
			}
			else
			{
				givenItem.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(givenItem, true);
				actor.OutputHandler.Send($"You couldn't hold {givenItem.HowSeen(actor)}, so it is on the ground.");
			}
		}
	}

	private static void PropertySell(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to sell? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (property.SaleOrder != null)
		{
			actor.OutputHandler.Send(
				$"The property {property.Name.ColourName()} is already for sale. Please cancel the existing sale order first.");
			return;
		}

		if (property.Lease is null && property.PropertyKeys.Any(x => !x.IsReturned))
		{
			actor.OutputHandler.Send(
				"You must return all the keys before you list the property for sale if it is not leased.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What price do you want to list {property.Name.ColourName()} for sale for?");
			return;
		}

		if (!ez.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var price))
		{
			actor.OutputHandler.Send(
				$"The amount {ss.Last.ColourValue()} is not a valid amount of {ez.Currency.Name.ColourValue()}.");
			return;
		}

		property.SaleOrder = new PropertySaleOrder(property, price);
		actor.OutputHandler.Send(
			$"You create a sale order for the property {property.Name.ColourName()} for {ez.Currency.Describe(price, CurrencyDescriptionPatternType.Short).ColourValue()}.");
	}

	private static void PropertyDelaySell(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to delay the sale of? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (property.SaleOrder == null)
		{
			actor.OutputHandler.Send(
				$"The property {property.Name.ColourName()} is not proposed to be listed for sale.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How long do you want to delay the sale of {property.Name.ColourName()} by?\nYou can use either the {$"property delaysell {ss.Last} <timespan>".ColourCommand()} or {$"property delaysell {ss.Last} until <date>".ColourCommand()} syntax.");
			return;
		}

		if (ss.PeekSpeech().EqualTo("until"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What date do you want to delay the sale of {property.Name.ColourName()} until?");
				return;
			}

			if (!ez.FinancialPeriodReferenceCalendar.TryGetDate(ss.SafeRemainingArgument, actor, out var date, out var error))
			{
				actor.OutputHandler.Send(error);
				return;
			}

			property.SaleOrder.StartOfListing = date.ToDateTime();
			actor.OutputHandler.Send(
				$"The property {property.Name.ColourName()} will now only be listed for sale on or after {property.SaleOrder.StartOfListing.GetByTimeZone(actor.Location.TimeZone(ez.FinancialPeriodReferenceClock)).ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)}");
			return;
		}

		if (!TimeSpanParser.TryParse(ss.SafeRemainingArgument, Units.Days, Units.Days, out var timespan))
		{
			actor.OutputHandler.Send(
				$"That was not a valid time span.\n{"Note: Years and Months are not supported, use Weeks or Days in that case".ColourCommand()}");
			return;
		}

		var newDate = ez.FinancialPeriodReferenceCalendar.CurrentDateTime + timespan;
		property.SaleOrder.StartOfListing = newDate;
		actor.OutputHandler.Send(
			$"The property {property.Name.ColourName()} will now only be listed for sale on or after {property.SaleOrder.StartOfListing.GetByTimeZone(actor.Location.TimeZone(ez.FinancialPeriodReferenceClock)).ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)}");
	}

	private static void PropertyMakeLease(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to lease? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (property.LeaseOrder != null)
		{
			actor.OutputHandler.Send(
				$"The property {property.Name.ColourName()} is already the subject of a lease order. Please cancel the existing lease order first.");
			return;
		}

		if (property.Lease is null && property.PropertyKeys.Any(x => !x.IsReturned))
		{
			actor.OutputHandler.Send(
				"You must return all the keys before you list the property for lease if it is not already leased.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an amount of rent to be paid per 14 day period.");
			return;
		}

		if (!ez.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var price))
		{
			actor.OutputHandler.Send(
				$"The quantity {ss.Last.ColourCommand()} is not a valid amount of {ez.Currency.Name.ColourName()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the value of the bond that needs to be put up?");
			return;
		}

		if (!ez.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var bond))
		{
			actor.OutputHandler.Send(
				$"The quantity {ss.Last.ColourCommand()} is not a valid amount of {ez.Currency.Name.ColourName()}.");
			return;
		}

		property.LeaseOrder = new PropertyLeaseOrder(property, price, bond);
		actor.OutputHandler.Send(
			$"You create a new lease order for {property.Name.ColourName()}, for {ez.Currency.Describe(price, CurrencyDescriptionPatternType.Short).ColourValue()} every 14 days with a {ez.Currency.Describe(bond, CurrencyDescriptionPatternType.Short).ColourValue()} bond.");
	}

	private static void PropertyApproveSale(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez == null)
		{
			actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
			return;
		}

		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
		                      .ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which property are you looking to approve the sale of? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
		if (property == null)
		{
			actor.OutputHandler.Send(
				$"You do not own such a property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
			return;
		}

		if (property.SaleOrder == null)
		{
			actor.OutputHandler.Send(
				$"The property {property.Name.ColourName()} is not proposed to be listed for sale.");
			return;
		}

		foreach (var owner in property.PropertyOwners)
		{
			if (owner.Owner == actor || (owner.Owner is IClan clan && actor.ClanMemberships.Any(x =>
				    x.Clan == clan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty))))
			{
				property.SaleOrder.SetConsent(owner);
			}
		}

		actor.OutputHandler.Send(
			$"You give your approval for {property.Name.ColourName()} to be sold at the proposed terms.");
	}

	private static void PropertyApproveLease(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "approve");
		if (property == null)
		{
			return;
		}

		foreach (var owner in property.PropertyOwners)
		{
			if (owner.Owner == actor || (owner.Owner is IClan clan && actor.ClanMemberships.Any(x =>
				    x.Clan == clan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty))))
			{
				property.LeaseOrder!.SetConsent(owner);
			}
		}

		actor.OutputHandler.Send(
			$"You give your approval for {property.Name.ColourName()} to be leased at the proposed terms.");
	}

	private static void PropertyCancelLease(ICharacter actor, StringStack ss)
	{
		var property = FindPropertyWithLeaseOrder(actor, ss, "cancel");
		if (property == null)
		{
			return;
		}

		var lo = property.LeaseOrder;
		actor.OutputHandler.Send(
			$"Are you sure you want to cancel the lease order for {property.Name.ColourName()}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You cancel the lease order for {property.Name.ColourName()}.");
				property.ExpireLeaseOrder(lo);
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to cancel the lease order for the property {property.Name.ColourName()}.");
			},
			DescriptionString = $"Cancelling the lease order for property {property.Name.ColourName()}",
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to cancel the lease order for the property {property.Name.ColourName()}.");
			},
			Keywords = new List<string> { "lease", "property", "cancel" }
		}), TimeSpan.FromSeconds(120));
	}

	[PlayerCommand("Properties", "properties")]
	[CustomModuleName("Economy")]
	protected static void Properties(ICharacter actor, string command)
	{
		var sb = new StringBuilder();
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		var properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez).ToList();
		if (ez != null)
		{
			sb.AppendLine("The following properties are available to buy in this economic zone:");
			sb.AppendLine(StringUtilities.GetTextTable(
				from property in properties.Where(x => x.SaleOrder?.ShowForSale == true)
				select new List<string>
				{
					property.Name,
					(property.SaleOrder!.StartOfListing + property.SaleOrder.DurationOfListing).Date.Display(
						CalendarDisplayMode.Short),
					property.LastChangeOfOwnership?.Date.Display(CalendarDisplayMode.Short) ?? "Never",
					property.Lease == null
						? "No"
						: $"{ez.Currency.Describe(property.Lease.PricePerInterval, CurrencyDescriptionPatternType.Short)} {property.Lease.Interval.Describe(ez.FinancialPeriodReferenceCalendar)} until {property.Lease.LeaseEnd.Date.Display(CalendarDisplayMode.Short)}",
					ez.Currency.Describe(property.SaleOrder.ReservePrice, CurrencyDescriptionPatternType.Short)
				},
				new List<string>
				{
					"Name",
					"Listed Until",
					"Last Sold",
					"Leased?",
					"Price"
				},
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			));
			sb.AppendLine("The following properties are available to lease in this economic zone:");
			sb.AppendLine(StringUtilities.GetTextTable(
				from property in properties.Where(x => x.LeaseOrder?.ListedForLease == true)
				select new List<string>
				{
					property.Name,
					property.LeaseOrder!.MinimumLeaseDuration.Describe(),
					property.LeaseOrder.MaximumLeaseDuration.Describe(),
					property.Lease == null ? "Now" : property.Lease.LeaseEnd.Date.Display(CalendarDisplayMode.Short),
					$"{ez.Currency.Describe(property.LeaseOrder.PricePerInterval, CurrencyDescriptionPatternType.Short)} {property.LeaseOrder.Interval.Describe(ez.FinancialPeriodReferenceCalendar)}",
					ez.Currency.Describe(property.LeaseOrder.BondRequired, CurrencyDescriptionPatternType.Short)
				},
				new List<string>
				{
					"Name",
					"Min Duration",
					"Max Duration",
					"Available",
					"Price",
					"Deposit"
				},
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			));
		}

		if (sb.Length > 0)
		{
			sb.AppendLine();
		}

		var owned = actor.Gameworld.Properties.Where(x => x.PropertyOwners.Any(y => y.Owner.Equals(actor)))
		                 .ToList();
		if (owned.Any())
		{
			sb.AppendLine("You own the following properties:");
			sb.AppendLine(StringUtilities.GetTextTable(
				from property in owned
				select new List<string>
				{
					property.Name,
					property.LastChangeOfOwnership?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short) ??
					"Never",
					property.Lease == null
						? "No"
						: $"{property.EconomicZone.Currency.Describe(property.Lease.PricePerInterval, CurrencyDescriptionPatternType.Short)} {property.Lease.Interval.Describe(property.EconomicZone.FinancialPeriodReferenceCalendar)} until {property.Lease.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)}",
					property.SaleOrder != null
						? property.EconomicZone.Currency.Describe(property.SaleOrder.ReservePrice,
							CurrencyDescriptionPatternType.Short)
						: "No",
					property.PropertyOwners.First(x => x.Owner == actor).ShareOfOwnership.ToString("P2", actor)
				},
				new List<string>
				{
					"Name",
					"Last Sold",
					"Leased?",
					"For Sale",
					"Stake"
				},
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			));
		}
		else
		{
			sb.AppendLine("You do not own any properties.");
		}

		sb.AppendLine();

		var leased = actor.Gameworld.Properties.Where(x => x.Lease?.Leaseholder.Equals(actor) == true)
		                  .ToList();
		if (leased.Any())
		{
			sb.AppendLine("You are leasing the following properties.");
			sb.AppendLine(StringUtilities.GetTextTable(
				from property in leased
				select new List<string>
				{
					property.Name,
					property.Lease!.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
					$"{property.EconomicZone.Currency.Describe(property.Lease.LeaseOrder.PricePerInterval, CurrencyDescriptionPatternType.Short)} {property.Lease.LeaseOrder.Interval.Describe(property.EconomicZone.FinancialPeriodReferenceCalendar)}",
					property.EconomicZone.Currency.Describe(property.Lease.PaymentBalance,
						CurrencyDescriptionPatternType.Short),
					property.Lease!.Interval.GetNextDateTime(property.Lease!.LastLeasePayment)
					        .ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
					(property.Lease.AutoRenew && property.LeaseOrder == property.Lease.LeaseOrder).ToString()
				},
				new List<string>
				{
					"Name",
					"Lease End",
					"Price",
					"Balance",
					"Next Due",
					"Autorenew"
				},
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			));
		}
		else
		{
			sb.AppendLine("You are not leasing any properties.");
		}

		actor.OutputHandler.Send(sb.ToString());
	}
}