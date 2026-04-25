using Dapper;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Trees;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Estates;
using MudSharp.Economy.Markets;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Property;
using MudSharp.Economy.Shoppers;
using MudSharp.Economy.Shops;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Migrations;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Butchering;
using MudSharp.Work.Crafts;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using TimeSpanParserUtil;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using AuctionBid = MudSharp.Economy.AuctionBid;

#nullable enable
#nullable disable warnings

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule : Module<ICharacter>
{
    private EconomyModule()
        : base("Economy")
    {
        IsNecessary = true;
    }

    public static EconomyModule Instance { get; } = new();

    private static void CountItem(IGameItem item, Dictionary<ICurrency, decimal> results, StringBuilder sb,
        IPerceiver voyeur, IGameItem parent = null)
    {
        ICurrencyPile currency = item.GetItemType<ICurrencyPile>();
        if (currency != null)
        {
            if (!results.ContainsKey(currency.Currency))
            {
                results[currency.Currency] = 0;
            }

            results[currency.Currency] += currency.Coins.Sum(y => y.Item1.Value * y.Item2);
            foreach (Tuple<ICoin, int> coin in currency.Coins.OrderBy(x => x.Item1.Value))
            {
                sb.AppendLine(
                    $"\t{coin.Item1.ShortDescription.Colour(Telnet.Green)} (x{coin.Item2}) = {currency.Currency.Describe(coin.Item2 * coin.Item1.Value, CurrencyDescriptionPatternType.ShortDecimal).Colour(Telnet.Green)}{(parent != null ? $" [in {parent.HowSeen(voyeur)}]" : "")}");
            }

            return;
        }

        IContainer container = item.GetItemType<IContainer>();
        if (container != null)
        {
            foreach (IGameItem contained in container.Contents)
            {
                CountItem(contained, results, sb, voyeur, item);
            }
        }
    }

    private static bool CanManageEstate(ICharacter actor, IEconomicZone zone)
    {
        if (actor.IsAdministrator())
        {
            return true;
        }

        IClan clan = zone.ControllingClan;
        return clan != null &&
               actor.ClanMemberships.Any(x =>
                   !x.IsArchivedMembership &&
                   x.Clan == clan &&
                   x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageEstates));
    }

    private static bool CanManageEstate(ICharacter actor, IEstate estate)
    {
        return CanManageEstate(actor, estate.EconomicZone);
    }

    private static bool IsEstateLocationRestrictionExempt(ICharacter actor)
    {
        if (actor.IsAdministrator())
        {
            return true;
        }

        return actor.ClanMemberships.Any(x =>
            !x.IsArchivedMembership &&
            x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageEstates) &&
            actor.Gameworld.EconomicZones.Any(y => y.ControllingClan == x.Clan));
    }

    private static bool EnsureEstateCommandLocation(ICharacter actor, IEstate estate = null)
    {
        if (actor.IsAdministrator() || (estate != null && CanManageEstate(actor, estate)) ||
            IsEstateLocationRestrictionExempt(actor))
        {
            return true;
        }

        IEconomicZone zone = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ProbateOfficeCells.Contains(actor.Location));
        if (zone != null)
        {
            return true;
        }

        actor.OutputHandler.Send("You must be at a probate office to use estate services.");
        return false;
    }

    private static IEconomicZone CurrentProbateOfficeZone(ICharacter actor)
    {
        return actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ProbateOfficeCells.Contains(actor.Location));
    }

    private static bool CanViewEstate(ICharacter actor, IEstate estate)
    {
        if (estate.EstateStatus == EstateStatus.EstateWill)
        {
            return CanManageEstate(actor, estate) ||
                   estate.Character == actor ||
                   estate.Inheritor == actor ||
                   estate.Claims.Any(x => x.Claimant == actor);
        }

        return CanManageEstate(actor, estate) ||
               estate.Character == actor ||
               estate.Inheritor == actor ||
               estate.Claims.Any(x => x.Claimant == actor) ||
               estate.EstateStatus != EstateStatus.Undiscovered;
    }

    private static IEstate GetEstateById(ICharacter actor, string text)
    {
        return long.TryParse(text, out long value)
            ? actor.Gameworld.Estates.Get(value)
            : null;
    }

    private static IEstateAsset GetEstateAssetById(IEstate estate, string text)
    {
        return long.TryParse(text, out long value)
            ? estate.Assets.FirstOrDefault(x => x.Id == value)
            : null;
    }

    private static IClan GetClanByIdOrName(ICharacter actor, string text)
    {
        if (long.TryParse(text, out long value))
        {
            return actor.Gameworld.Clans.Get(value);
        }

        return actor.Gameworld.Clans.GetByName(text) ??
               actor.Gameworld.Clans.FirstOrDefault(x => x.Alias.EqualTo(text));
    }

    private static ICharacter GetCharacterByIdOrName(ICharacter actor, string text)
    {
        if (long.TryParse(text, out long value))
        {
            return actor.Gameworld.TryGetCharacter(value, true);
        }

        return actor.TargetActor(text) ??
               actor.Gameworld.Characters.GetByIdOrName(text) ??
               actor.Gameworld.Characters.GetByPersonalName(text);
    }

    private static string DescribeFrameworkItem(ICharacter actor, IFrameworkItem item)
    {
        return item switch
        {
            null => "None".ColourError(),
            ICharacter character => character.PersonalName.GetName(NameStyle.FullName).ColourName(),
            IClan clan => clan.FullName.ColourName(),
            IGameItem gameItem => gameItem.HowSeen(actor,
                flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings),
            IBankAccount account => $"{account.Bank.Code.ColourName()}:{account.AccountNumber.ToString("N0", actor).ColourValue()}",
            IProperty property => property.Name.ColourName(),
            _ => $"{item.Name.ColourName()} ({item.FrameworkItemType.ColourValue()})"
        };
    }

    private static string DescribeOwnershipForViewer(ICharacter actor, IGameItem item)
    {
        if (!item.HasOwner)
        {
            return "Unowned".ColourError();
        }

        if (actor.IsAdministrator())
        {
            return DescribeFrameworkItem(actor, item.Owner);
        }

        return item.Owner switch
        {
            ICharacter character when character == actor => "You".ColourName(),
            IClan clan => clan.FullName.ColourName(),
            _ => "someone"
        };
    }

    private static bool CanInspectContainedItems(ICharacter actor, IGameItem item)
    {
        if (actor.IsAdministrator())
        {
            return true;
        }

        IContainer container = item.GetItemType<IContainer>();
        if (container == null)
        {
            return false;
        }

        if (container.Transparent)
        {
            return true;
        }

        return item.GetItemType<IOpenable>()?.IsOpen == true;
    }

    private static void AppendOwnershipRows(ICharacter actor, ICollection<List<string>> rows, IEnumerable<IGameItem> items,
        string rootLocation, IGameItem containingItem = null)
    {
        foreach (IGameItem item in items)
        {
            if (!actor.IsAdministrator() && !actor.CanSee(item))
            {
                continue;
            }

            rows.Add(new List<string>
            {
                containingItem == null ? rootLocation : $"In {containingItem.HowSeen(actor)}",
                item.HowSeen(actor),
                DescribeOwnershipForViewer(actor, item)
            });

            if (CanInspectContainedItems(actor, item))
            {
                foreach (IContainer container in item.GetItemTypes<IContainer>())
                {
                    AppendOwnershipRows(actor, rows, container.Contents, rootLocation, item);
                }
            }
        }
    }

    private static string DescribeEstateStatus(EstateStatus status)
    {
        return status.DescribeEnum().ColourValue();
    }

    private static string DescribeAuctionLot(ICharacter actor, AuctionItem item)
    {
        return item.Asset switch
        {
            IGameItem gameItem => gameItem.HowSeen(actor,
                flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings),
            IProperty property =>
                $"{item.PropertyShare.ToString("P2", actor).ColourValue()} ownership share in {property.Name.ColourName()}",
            _ => item.Asset.Name.ColourName()
        };
    }

    private static string DescribeEstateAsset(ICharacter actor, IEstateAsset asset)
    {
        return asset.Asset switch
        {
            IProperty property => $"{asset.OwnershipShare.ToString("P2", actor).ColourValue()} ownership share in {property.Name.ColourName()}",
            _ => DescribeFrameworkItem(actor, asset.Asset)
        };
    }

    private static AuctionItem ResolveAuctionLot(ICharacter actor, IAuctionHouse auctionHouse, string target)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            return null;
        }

        List<AuctionItem> listings = auctionHouse.ActiveAuctionItems.ToList();
        return listings.GetFromItemListByKeyword(target, actor) ??
               listings.FirstOrDefault(x => x.Asset.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase)) ??
               listings.FirstOrDefault(x => x.Asset.Name.StartsWith(target, StringComparison.InvariantCultureIgnoreCase)) ??
               listings.FirstOrDefault(x => x.Asset.Name.Contains(target, StringComparison.InvariantCultureIgnoreCase));
    }

    private static bool CanManageAuctionLot(ICharacter actor, AuctionItem item)
    {
        if (actor.IsAdministrator())
        {
            return true;
        }

        if (item.Seller.FrameworkItemEquals(actor.Id, actor.FrameworkItemType))
        {
            return true;
        }

        return item.Seller is IEstate estate && CanManageEstate(actor, estate);
    }

    private static IEnumerable<AuctionItem> ActiveEstateAuctionLots(IEstate estate)
    {
        return estate.EconomicZone.EstateAuctionHouse?.ActiveAuctionItems.Where(x => x.IsSeller(estate)) ??
               Enumerable.Empty<AuctionItem>();
    }

    private static IEnumerable<UnclaimedAuctionItem> UnclaimedEstateAuctionLots(IEstate estate)
    {
        return estate.EconomicZone.EstateAuctionHouse?.UnclaimedItems.Where(x => x.AuctionItem.IsSeller(estate)) ??
               Enumerable.Empty<UnclaimedAuctionItem>();
    }

    private static IEconomicZone CurrentMorgueOfficeZone(ICharacter actor)
    {
        return actor.Gameworld.EconomicZones.FirstOrDefault(x => x.MorgueOfficeCell == actor.Location);
    }

    private static bool EnsureMorgueOffice(ICharacter actor, out IEconomicZone zone)
    {
        zone = CurrentMorgueOfficeZone(actor);
        if (zone != null)
        {
            return true;
        }

        actor.OutputHandler.Send("You must be at a morgue office to use that command.");
        return false;
    }

    private static bool CanClaimMorgueCorpse(ICharacter actor, IEstate estate)
    {
        return actor.IsAdministrator() || CanManageEstate(actor, estate) || estate.Inheritor == actor;
    }

    private static bool CanClaimMorgueCorpse(ICharacter actor, IEconomicZone zone, MorgueStoredCorpse effect)
    {
        if (effect.EstateId > 0)
        {
            IEstate estate = actor.Gameworld.Estates.Get(effect.EstateId);
            return estate != null && CanClaimMorgueCorpse(actor, estate);
        }

        ICharacter deceased = actor.Gameworld.TryGetCharacter(effect.CharacterOwnerId, true);
        return actor.IsAdministrator() || CanManageEstate(actor, zone) || deceased?.EstateHeir == actor;
    }

    private static IEnumerable<AuctionResult> EstateAuctionResults(IEstate estate)
    {
        return estate.EconomicZone.EstateAuctionHouse?.AuctionResults.Where(x =>
                   x.SellerId == estate.Id &&
                   x.SellerType.EqualTo(estate.FrameworkItemType)) ??
               Enumerable.Empty<AuctionResult>();
    }

    private static decimal DefaultEstateReservePrice(IEstate estate, IEstateAsset asset)
    {
        return Math.Max(0.0M, asset.AssumedValue);
    }

    private static bool TryGetEstateAuctionPrices(ICharacter actor, IEstate estate, IEstateAsset asset, StringStack ss,
        out decimal reservePrice, out decimal? buyoutPrice)
    {
        reservePrice = DefaultEstateReservePrice(estate, asset);
        buyoutPrice = null;
        if (ss.IsFinished)
        {
            return reservePrice > 0.0M;
        }

        if (!estate.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out reservePrice) || reservePrice <= 0.0M)
        {
            actor.OutputHandler.Send(
                $"You must specify a positive reserve price in {estate.EconomicZone.Currency.Name.ColourName()}.");
            return false;
        }

        if (ss.IsFinished)
        {
            return true;
        }

        if (!estate.EconomicZone.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out decimal parsedBuyout) ||
            parsedBuyout <= reservePrice)
        {
            actor.OutputHandler.Send(
                $"You must specify a buyout price in {estate.EconomicZone.Currency.Name.ColourName()} that is higher than the reserve price.");
            return false;
        }

        buyoutPrice = parsedBuyout;
        return true;
    }

    [PlayerCommand("Count", "count")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [HelpInfo("count",
        "The count command is used to take stock of the physical currency that you are carrying (including in containers), to help you understand how much money you have. It will also display a list of bank accounts that you own (though you need to go to a bank to see the balances of these).",
        AutoHelp.HelpArg)]
    protected static void Count(ICharacter character, string command)
    {
        Dictionary<ICurrency, decimal> results = new();
        StringBuilder sb = new();
        sb.AppendLine("You have the following currency items on your person:");
        foreach (IGameItem item in character.Body.ExternalItems)
        {
            CountItem(item, results, sb, character);
        }

        sb.AppendLine();
        if (!results.Any())
        {
            sb.AppendLine("\tNone. You are completely broke.".ColourError());
        }
        else
        {
            sb.AppendLine("This comes to a total of " +
                          results.Select(
                              x =>
                                  x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal)
                                   .Colour(Telnet.Green)).ListToString());
        }

        List<IBankAccount> bankAccounts = character.Gameworld.BankAccounts.Where(x => x.IsAccountOwner(character)).ToList();
        if (bankAccounts.Any())
        {
            sb.AppendLine();
            sb.AppendLine("You also know that you have accounts with the following banks:");
            sb.AppendLine();
            foreach (IBankAccount account in bankAccounts)
            {
                sb.AppendLine(
                    $"\t{account.AccountReference.ColourValue()} - {account.BankAccountType.Name.ColourName()} with {account.Bank.Name.ColourName()}");
            }
        }

        character.OutputHandler.Send(sb.ToString());
    }

    [PlayerCommand("Appraise", "appraise")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [HelpInfo("appraise",
        @"The appraise command is used to quickly judge the value of an item, or a container full of items. There are a few different ways that you can use this command:

	#3appraise#0 - appraises the value of all items you are carrying and that are present in the room
	#3appraise <thing>#0 - appraises the value of a thing and its contents (if a container)",
        AutoHelp.HelpArg)]
    protected static void Appraise(ICharacter actor, string command)
    {
        if (actor.Currency is null)
        {
            actor.OutputHandler.Send("You must first set a currency to use before you can use this command.");
            return;
        }

        StringStack ss = new(command.RemoveFirstWord());
        ITraitDefinition td = actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("AppraiseCommandSkill"));
        if (!actor.IsAdministrator() &&
            actor.Gameworld.GetStaticBool("AppraiseCommandRequiresSkill") &&
            td is not null &&
            actor.TraitValue(td) <= 0.0)
        {
            actor.OutputHandler.Send(
                $"Without the {td.Name.ColourName()} skill, you have no idea how much things are worth.");
            return;
        }

        IGameItem target = null;
        if (!ss.IsFinished)
        {
            target = actor.TargetItem(ss.SafeRemainingArgument);
            if (target is null)
            {
                actor.OutputHandler.Send("There is nothing like that here for you to appraise.");
                return;
            }
        }

        decimal fuzzinessFloor = 1.0M;
        decimal fuzzinessCeiling = 1.0M;
        if (td is not null && !actor.IsAdministrator())
        {
            ICheck check = actor.Gameworld.GetCheck(CheckType.AppraiseItemCheck);
            Difficulty difficulty = target is null
                ? Difficulty.VeryHard
                : (target.IsItemType<IContainer>() ? Difficulty.Normal : Difficulty.Easy);
            CheckOutcome result = check.Check(actor, difficulty, td, target);
            decimal skew = 0.0M;
            switch (result.Outcome)
            {
                case Outcome.MajorFail:
                    skew = (decimal)RandomUtilities.DoubleRandom(-0.5, 0.5);
                    fuzzinessCeiling = 1.5M + skew;
                    fuzzinessFloor = 0.5M + skew;
                    break;
                case Outcome.Fail:
                    skew = (decimal)RandomUtilities.DoubleRandom(-0.3, 0.3);
                    fuzzinessCeiling = 1.3M + skew;
                    fuzzinessFloor = 0.7M + skew;
                    break;
                case Outcome.MinorFail:
                    skew = (decimal)RandomUtilities.DoubleRandom(-0.2, 0.2);
                    fuzzinessCeiling = 1.2M + skew;
                    fuzzinessFloor = 0.8M + skew;
                    break;
                case Outcome.MinorPass:
                    skew = (decimal)RandomUtilities.DoubleRandom(-0.1, 0.1);
                    fuzzinessCeiling = 1.1M + skew;
                    fuzzinessFloor = 0.9M + skew;
                    break;
                case Outcome.Pass:
                    skew = (decimal)RandomUtilities.DoubleRandom(-0.05, 0.05);
                    fuzzinessCeiling = 1.05M + skew;
                    fuzzinessFloor = 0.95M + skew;
                    break;
                case Outcome.MajorPass:
                    break;
            }
        }

        (decimal minimum, decimal maximum) CalculateMinimumMaximum(IGameItem item)
        {
            if (item.GetItemType<ICurrencyPile>() is { } cp)
            {
                return (
                    cp.TotalValue * cp.Currency.BaseCurrencyToGlobalBaseCurrencyConversion * fuzzinessFloor / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion,
                    cp.TotalValue * cp.Currency.BaseCurrencyToGlobalBaseCurrencyConversion * fuzzinessCeiling / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion);
            }
            return (item.Prototype.CostInBaseCurrency * fuzzinessFloor / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion,
                    item.Prototype.CostInBaseCurrency * fuzzinessCeiling / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion);
        }

        string DescribeCurrencyRange(decimal minimum, decimal maximum)
        {
            if (minimum == maximum)
            {
                return actor.Currency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
            }

            return $"{actor.Currency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to {actor.Currency.Describe(maximum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}";
        }

        void EvaluateItem(IGameItem item, List<(string ItemDescription, string ValueDescription, int Levels)> list,
            ref decimal minTotal, ref decimal maxTotal, int level, bool includeContents)
        {
            (decimal min, decimal max) = CalculateMinimumMaximum(item);
            minTotal += min;
            maxTotal += max;
            list.Add((item.HowSeen(actor), DescribeCurrencyRange(min, max), level));
            if (includeContents &&
                item.GetItemType<IContainer>() is { } container &&
                (actor.IsAdministrator() ||
                 container.Transparent ||
                 (container is IOpenable op && op.IsOpen)
                 )
                )
            {
                foreach (IGameItem content in container.Contents)
                {
                    EvaluateItem(content, list, ref minTotal, ref maxTotal, level + 1, true);
                }
            }
        }

        StringBuilder sb = new();
        if (target is null)
        {
            List<(string ItemDescription, string ValueDescription, int Levels)> results = new();
            decimal minTotal = 0.0M;
            decimal maxTotal = 0.0M;
            foreach (IGameItem item in actor.Location.LayerGameItems(actor.RoomLayer))
            {
                if (!actor.CanSee(item))
                {
                    continue;
                }

                EvaluateItem(item, results, ref minTotal, ref maxTotal, 0, true);
            }
            sb.AppendLine($"You appraise the value of the contents of the room:");
            sb.AppendLine();
            sb.AppendLine(StringUtilities.GetTextTable(from result in results
                                                       select new List<string>
            {
                new('*', result.Levels),
                result.ItemDescription,
                result.ValueDescription
            }, new List<string>
            {
                "Level",
                "Item",
                "Value"
            }, actor));
            sb.AppendLine($"Total Value: {DescribeCurrencyRange(minTotal, maxTotal)}");
        }
        else if (target.GetItemType<IContainer>() is { } targetContainer && (targetContainer.Transparent || (targetContainer is IOpenable op && op.IsOpen) || actor.IsAdministrator()))
        {
            List<(string ItemDescription, string ValueDescription, int Levels)> results = new();
            decimal minTotal = 0.0M;
            decimal maxTotal = 0.0M;
            sb.AppendLine($"You appraise the value of {target.HowSeen(actor)}:");
            sb.AppendLine();
            EvaluateItem(target, results, ref minTotal, ref maxTotal, 0, true);
            sb.AppendLine(StringUtilities.GetTextTable(from result in results
                                                       select new List<string>
            {
                new('*', result.Levels),
                result.ItemDescription,
                result.ValueDescription
            }, new List<string>
            {
                "Level",
                "Item",
                "Value"
            }, actor));
            sb.AppendLine($"Total Value: {DescribeCurrencyRange(minTotal, maxTotal)}");
            decimal topminTotal = 0.0M;
            decimal topmaxTotal = 0.0M;
            EvaluateItem(target, results, ref topminTotal, ref topmaxTotal, 0, false);
            sb.AppendLine($"Contents Value: {DescribeCurrencyRange(minTotal - topminTotal, maxTotal - topmaxTotal)}");
        }
        else
        {
            (decimal min, decimal max) = CalculateMinimumMaximum(target);
            sb.AppendLine($"You appraise the value of {target.HowSeen(actor)}:");
            sb.AppendLine();
            sb.AppendLine(StringUtilities.GetTextTable(new[] { new List<string>
            {
                "",
                target.HowSeen(actor),
                DescribeCurrencyRange(min,max)
            } }, new List<string>
            {
                "Level",
                "Item",
                "Value"
            }, actor));
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    #region Currency

    public const string CurrencyHelp = @"This command is used to create, edit and view currencies. 

See also the closely related #3COIN#0 command for editing coins for your currencies, and #3LOADCURRENCY#0 for loading some into the world.

The syntax for editing currencies is as follows:

	#3currency list#0 - lists all of the currencies (see below for filters)
	#3currency edit <which>#0 - begins editing a currency
	#3currency edit new <name> <lowest division> <lowest coin>#0 - generates a new coin
	#3currency clone <old> <new>#0 - clones an existing currency to a new one
	#3currency close#0 - stops editing a currency
	#3currency show <which>#0 - views information about a currency
	#3currency show#0 - views information about your currently editing currency
	#3currency set name <name>#0 - sets the name of this currency
	#3currency set conversion <rate>#0 - sets the global currency conversion rate (to global base currency)
	#3currency set adddivision <name> <rate>#0 - adds a new currency division
	#3currency set remdivision <id|name>#0 - removes a currency division
	#3currency set division <id|name> name <name>#0 - sets a new name for the division
	#3currency set division <id|name> base <amount>#0 - sets the amount of base currency this division is worth
	#3currency set division <id|name> ignorecase#0 - toggles ignoring case in the regular expression patterns for the division
	#3currency set division <id|name> addabbr <regex>#0 - adds a regular expression pattern for this division
	#3currency set division <id|name> remabbr <##>#0 - removes a particular pattern abbreviation for this division
	#3currency set division <id|name> abbr <##> <regex>#0 - overwrites the regular expression pattern at the specified index for this division
	#3currency set addpattern <type>#0 - adds a new pattern of the specified type
	#3currency set removepattern <id>#0 - removes a pattern
	#3currency set pattern <id|name> order <##>#0 - changes the order in which this pattern is evaluated for applicability
	#3currency set pattern <id|name> prog <which>#0 - sets the prog that controls applicability for this pattern
	#3currency set pattern <id|name> negative <prefix>#0 - sets a prefix applied to negative values for this pattern (e.g. #2-#0 or #2negative #0.) Be sure to include spaces if necessary
	#3currency set pattern <id|name> natural#0 - toggles natural aggregation style for pattern elements (commas plus ""and"") rather than just concatenation
	#3currency set pattern <id|name> addelement <division> <plural> <pattern>#0 - adds a new pattern element
	#3currency set pattern <id|name> remelement <id|##>#0 - deletes an element.
	#3currency set pattern <id|name> element <id|##order> zero#0 - toggles showing this element if it is zero
	#3currency set pattern <id|name> element <id|##order> specials#0 - toggles special values totally overriding the pattern instead of just the value part
	#3currency set pattern <id|name> element <id|##order> order <##>#0 - changes the order this element appears in the list of its pattern
	#3currency set pattern <id|name> element <id|##order> pattern <pattern>#0 - sets the pattern for the element. Use #3{0}#0 for the numerical value.
	#3currency set pattern <id|name> element <id|##order> last <pattern>#0 - sets an alternate pattern if this is the last element in the display. Use #3{0}#0 for the numerical value.
	#3currency set pattern <id|name> element <id|##order> last none#0 - clears the last alternative pattern
	#3currency set pattern <id|name> element <id|##order> plural <word>#0 - sets the word in the pattern that should be used for pluralisation
	#3currency set pattern <id|name> element <id|##order> rounding <truncate|round|noround>#0 - changes the rounding mode for this element
	#3currency set pattern <id|name> element <id|##order> addspecial <value> <text>#0 - adds or sets a special value
	#3currency set pattern <id|name> element <id|##order> remspecial <value>#0 - removes a special value";

    [PlayerCommand("Currency", "currency")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("currency", CurrencyHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Currency(ICharacter character, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "list":
                CurrencyList(character, ss);
                break;
            case "set":
            case "show":
            case "edit":
            case "close":
            case "new":
            case "clone":
                if (!character.IsAdministrator(PermissionLevel.SeniorAdmin))
                {
                    character.OutputHandler.Send("Due to the potential to break things, only Senior Admins or higher can run this specific subcommand.");
                    return;
                }

                break;
            default:
                character.OutputHandler.Send(CurrencyHelp.SubstituteANSIColour());
                return;
        }

        switch (ss.Last)
        {
            case "set":
                CurrencySet(character, ss);
                break;
            case "show":
                CurrencyShow(character, ss);
                break;
            case "edit":
                CurrencyEdit(character, ss);
                break;
            case "close":
                CurrencyClose(character, ss);
                break;
            case "new":
                CurrencyNew(character, ss);
                break;
            case "clone":
                CurrencyClone(character, ss);
                break;
        }
    }

    private static void CurrencyClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which currency do you want to clone?");
            return;
        }

        ICurrency currency = actor.Gameworld.Currencies.GetByIdOrName(ss.PopSpeech());
        if (currency is null)
        {
            actor.OutputHandler.Send("There is no such currency.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new currency?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.Currencies.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a currency called {name.ColourName()}. Names must be unique.");
            return;
        }

        ICurrency clone = currency.Clone(name);
        actor.Gameworld.Add(clone);
        actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
        actor.AddEffect(new BuilderEditingEffect<ICurrency>(actor) { EditingItem = clone });
        actor.OutputHandler.Send($"You create a new currency called {name.ColourName()} cloned from {currency.Name.ColourName()}, which you are now editing.");
    }

    private static void CurrencyNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to your new currency?");
            return;
        }

        string name = ss.PopSpeech().TitleCase();
        if (actor.Gameworld.Currencies.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a currency called {name.ColourName()}. Names must be unique.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to call the lowest currency division for your currency?");
            return;
        }

        string division = ss.PopSpeech().TitleCase();
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to call the lowest coin for your currency?");
            return;
        }

        string coin = ss.SafeRemainingArgument;
        Currency currency = new(actor.Gameworld, name, division, coin);
        actor.Gameworld.Add(currency);
        actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
        actor.AddEffect(new BuilderEditingEffect<ICurrency>(actor) { EditingItem = currency });
        actor.OutputHandler.Send($"You create a new currency called {name.ColourName()}, which you are now editing.");
    }

    private static void CurrencyEdit(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            BuilderEditingEffect<ICurrency> current = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
            if (current is null)
            {
                actor.OutputHandler.Send("Which currency did you want to edit?");
                return;
            }

            actor.OutputHandler.Send(current.EditingItem.Show(actor));
            return;
        }

        ICurrency currency = actor.Gameworld.Currencies.GetByIdOrName(command.PopSpeech());
        if (currency is null)
        {
            actor.OutputHandler.Send("There is no currency like that.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
        actor.AddEffect(new BuilderEditingEffect<ICurrency>(actor) { EditingItem = currency });
        actor.OutputHandler.Send($"You are now editing the {currency.Name.ColourName()} currency.");
    }

    private static void CurrencyClose(ICharacter actor, StringStack command)
    {
        BuilderEditingEffect<ICurrency> effect = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any currencies.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
        actor.OutputHandler.Send("You are no longer editing any currencies.");
    }

    private static void CurrencyList(ICharacter actor, StringStack command)
    {
        List<ICurrency> currencies = actor.Gameworld.Currencies.ToList();
        // TODO - filters

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from currency in currencies
            select new List<string>
            {
                currency.Id.ToString("N0", actor),
                currency.Name,
                currency.BaseCurrencyToGlobalBaseCurrencyConversion.ToString("N3", actor),
            },
            new List<string>
            {
                "Id",
                "Name",
                "Conversion"
            },
            actor,
            Telnet.BoldYellow
            ));
    }

    private static void CurrencySet(ICharacter actor, StringStack command)
    {
        BuilderEditingEffect<ICurrency> effect = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any currencies.");
            return;
        }

        effect.EditingItem.BuildingCommand(actor, command);
    }

    private static void CurrencyShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<ICurrency> current = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
            if (current is null)
            {
                actor.OutputHandler.Send("Which currency did you want to view?");
                return;
            }

            actor.OutputHandler.Send(current.EditingItem.Show(actor));
            return;
        }

        ICurrency currency = actor.Gameworld.Currencies.GetByIdOrName(ss.PopSpeech());
        if (currency is null)
        {
            actor.OutputHandler.Send("There is no currency like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(currency.Show(actor));
            return;
        }

        string switchType = ss.PopSpeech().ToLowerInvariant();
        switch (switchType)
        {
            case "pattern":
                if (!long.TryParse(ss.PopSpeech(), out long value))
                {
                    actor.OutputHandler.Send("You must enter a valid ID.");
                    return;
                }

                ICurrencyDescriptionPattern pattern = currency.PatternDictionary.Values.SelectMany(x => x).Get(value);
                if (pattern is null)
                {
                    actor.OutputHandler.Send($"The {currency.Name.ColourValue()} currency does not have any such pattern.");
                    return;
                }
                actor.OutputHandler.Send(pattern.Show(actor));
                return;
            case "division":
                ICurrencyDivision division = currency.CurrencyDivisions.GetByIdOrName(ss.PopSpeech());
                if (division is null)
                {
                    actor.OutputHandler.Send($"The {currency.Name.ColourValue()} currency does not have any such division.");
                    return;
                }
                actor.OutputHandler.Send(division.Show(actor));
                return;
            case "coin":
                ICoin coin = currency.Coins.GetByIdOrName(ss.PopSpeech());
                if (coin is null)
                {
                    actor.OutputHandler.Send($"The {currency.Name.ColourValue()} currency does not have any such coin.");
                    return;
                }
                actor.OutputHandler.Send(coin.Show(actor));
                return;
            case "element":
                ICurrencyDescriptionPatternElement element = currency.PatternDictionary.Values.SelectMany(x => x).SelectMany(x => x.Elements).GetById(ss.PopSpeech());
                if (element is null)
                {
                    actor.OutputHandler.Send(
                        $"The {currency.Name.ColourValue()} currency does not have such a pattern element.");
                    return;
                }

                actor.OutputHandler.Send(element.Show(actor));
                return;
            default:
                actor.OutputHandler.Send("You must either specify #3pattern#0, #3element#0, #3coin#0 or #3division#0.".SubstituteANSIColour());
                return;
        }
    }

    #endregion

    #region Coins
    public const string CoinHelp = @"You can use this building command to edit coins, which are virtual items that exist in currency piles and used for economic transactions. Keep in mind that your coins don't have to perfectly match your currency divisions and you can even have coins that can be loaded (perhaps by an admin or a prog) but won't be used to automatically generate change for example.

The syntax for editing coins is as follows:

	#3coin list#0 - lists all of the coins (see below for filters)
	#3coin edit <which>#0 - begins editing a coin
	#3coin edit new <currency> <name> <value>#0 - generates a new coin
	#3coin clone <old> <new>#0 - clones an existing coin to a new one
	#3coin close#0 - stops editing a coin
	#3coin show <which>#0 - views information about a coin
	#3coin show#0 - views information about your currently editing coin
	#3coin set name <name>#0 - renames the coin
	#3coin set general <general>#0 - the general form of the coin (e.g. note, coin, etc)
	#3coin set plural <word>#0 - the keyword from the sdesc to pluralise (e.g. coin, penny, bill)
	#3coin set sdesc <sdesc>#0 - sets the short description of the coin
	#3coin set desc#0 - drops you into an editor to edit the full description of the coin
	#3coin set value <##>#0 - sets the base value of the coin
	#3coin set weight <weight>#0 - sets the weight of each coin
	#3coin set change#0 - toggles this coin being used for giving change

You can use the following search filters:

	#6+<keyword>#0 - include coins with this keyword in the name, sdesc or desc
	#6-<keyword>#0 - exclude coins with this keyword in the name, sdesc or desc
	#6<currency>#0 - only include coins from this currency
	#6change#0 - only include coins that are used for change
	#6!change#0 - only include coins that are not used for change";

    [PlayerCommand("Coin", "coin")]
    [HelpInfo("coin", CoinHelp, AutoHelp.HelpArgOrNoArg)]
    [CommandPermission(PermissionLevel.Admin)]
    protected static void Coin(ICharacter actor, string command)
    {
        BaseBuilderModule.GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.CoinHelper);
    }

    #endregion

    [PlayerCommand("List", "list")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("list", @"The list command is used with systems like shops to show you the inventory for sale.

At its simplest, the syntax is simply #3list#0. 

If there are multiple things in the room that can accept a #3list#0 command you can specify which one you want with the syntax #3list *<thing>#0 , e.g. #3list *vending#0

If you are in a shop, you can view the list output as a specific line of credit account (which may include custom discounts) with the syntax #3list ~<account> <password>#0, e.g. #3list ~cityguard thinblueline#0

You can also view variants of a particular item of merchandise with #3list variants <item>#0.

Finally, you can filter the output of list by a keyword with #3list <keyword>#0. You can combine the variants, keyword and account parameters for shops.",
        AutoHelp.HelpArg)]
    protected static void List(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        List<IListable> listables = actor.Location
            .LayerGameItems(actor.RoomLayer)
            .SelectNotNull(x => x.GetItemType<IListable>())
            .Where(x => actor.IsAdministrator() || x.Parent.GetItemType<IOnOff>()?.SwitchedOn != false)
            .ToList();

        IListable listable;
        IShop shop = null;
        if (actor.Location.Shop is not null)
        {
            shop = actor.Location.Shop;
        }
        else
        {
            IShopStall stall = actor.Location.
            LayerGameItems(actor.RoomLayer).
            SelectNotNull(x => x.GetItemType<IShopStall>()).
            FirstOrDefault(x => x.Shop is not null);
            shop = stall?.Shop;
        }

        if (ss.IsFinished)
        {
            if (shop is not null)
            {
                shop.ShowList(actor, actor);
                return;
            }

            listable = listables.FirstOrDefault();
            if (listable == null)
            {
                actor.Send("There is nothing here for which you can view a list of stock.");
                return;
            }
        }
        else
        {
            if (shop is not null && ss.Peek()[0] != '*')
            {
                ILineOfCreditAccount account = null;
                IMerchandise merch = null;
                string keyword = null;
                while (!ss.IsFinished)
                {
                    string arg = ss.PopSpeech();
                    if (arg[0] == '~')
                    {
                        arg = arg.RemoveFirstCharacter();
                        account = shop.LineOfCreditAccounts.FirstOrDefault(x => x.Name.EqualTo(arg));
                        if (account == null)
                        {
                            // TODO - echoed by shopkeeper?
                            actor.OutputHandler.Send("There is no such line of credit account associated with this shop.");
                            return;
                        }

                        switch (account.IsAuthorisedToUse(actor, 0.0M))
                        {
                            case LineOfCreditAuthorisationFailureReason.None:
                            case LineOfCreditAuthorisationFailureReason.AccountOverbalanced:
                            case LineOfCreditAuthorisationFailureReason.UserOverbalanced:
                                break;
                            case LineOfCreditAuthorisationFailureReason.NotAuthorisedAccountUser:
                                // TODO - echoed by shopkeeper?
                                actor.OutputHandler.Send("You are not an authorised user of that account.");
                                return;

                            case LineOfCreditAuthorisationFailureReason.AccountSuspended:
                                // TODO - echoed by shopkeeper?
                                actor.OutputHandler.Send("That account has been suspended.");
                                return;
                            default:
                                actor.OutputHandler.Send("There is a problem with that account.");
                                return;
                        }

                        continue;
                    }

                    if (arg.EqualTo("variants"))
                    {
                        if (ss.IsFinished)
                        {
                            actor.OutputHandler.Send("Which item of merchandise do you want to view variants for?");
                            return;
                        }

                        merch = shop.StockedMerchandise.GetFromItemListByKeyword(ss.PopSpeech(), actor);
                        if (merch == null)
                        {
                            actor.OutputHandler.Send(
                                "There is no such merchandise for sale in this shop that you can view detailed information for.");
                            return;
                        }

                        continue;
                    }

                    keyword = arg;
                }

                if (merch is not null || keyword is null)
                {
                    shop.ShowList(actor,
                        account?.IsAccountOwner(actor) == false
                            ? actor.Gameworld.TryGetCharacter(account.AccountOwnerId, true)
                            : actor, merch);
                }
                else
                {
                    shop.ShowList(actor, account?.IsAccountOwner(actor) == false ? actor.Gameworld.TryGetCharacter(account.AccountOwnerId, true)
                        : actor, keyword);
                }

                return;
            }

            string text = ss.PopSpeech();
            if (!string.IsNullOrEmpty(text) && text[0] == '*')
            {
                text = text.RemoveFirstCharacter();
            }

            IGameItem listableItem =
                listables.Select(x => x.Parent).GetFromItemListByKeyword(text, actor);
            if (listableItem == null)
            {
                actor.Send("You do not see anything by that keyword that you can list the stock of.");
                return;
            }

            listable = listableItem.GetItemType<IListable>();
        }

        actor.Send(listable.ShowList(actor, ss.SafeRemainingArgument ?? ""));
    }

    [PlayerCommand("Deals", "deals")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("deals",
        @"The deals command shows active deals in the current shop.

The syntax for this command is as follows:

	#3deals#0 - shows all active deals in the current shop
	#3deals <item>#0 - shows the active deals for a specific merchandise item
	#3deals ~<account> [<item>]#0 - shows deals and eligibility as if using a line of credit account",
        AutoHelp.HelpArgOrNoArg)]
    protected static void Deals(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsTrading || (shop as ITransientShop)?.CurrentStall?.IsTrading == false)
        {
            actor.OutputHandler.Send("This shop is not currently trading.");
            return;
        }

        ILineOfCreditAccount account = null;
        IMerchandise merch = null;
        while (!ss.IsFinished)
        {
            string arg = ss.PopSpeech();
            if (arg[0] == '~')
            {
                arg = arg.RemoveFirstCharacter();
                account = shop.LineOfCreditAccounts.FirstOrDefault(x => x.Name.EqualTo(arg));
                if (account == null)
                {
                    actor.OutputHandler.Send("There is no such line of credit account associated with this shop.");
                    return;
                }

                switch (account.IsAuthorisedToUse(actor, 0.0M))
                {
                    case LineOfCreditAuthorisationFailureReason.None:
                    case LineOfCreditAuthorisationFailureReason.AccountOverbalanced:
                    case LineOfCreditAuthorisationFailureReason.UserOverbalanced:
                        break;
                    case LineOfCreditAuthorisationFailureReason.NotAuthorisedAccountUser:
                        actor.OutputHandler.Send("You are not an authorised user of that account.");
                        return;
                    case LineOfCreditAuthorisationFailureReason.AccountSuspended:
                        actor.OutputHandler.Send("That account has been suspended.");
                        return;
                    default:
                        actor.OutputHandler.Send("There is a problem with that account.");
                        return;
                }

                continue;
            }

            merch = shop.StockedMerchandise.GetFromItemListByKeyword(arg, actor) ??
                    shop.Merchandises.GetFromItemListByKeywordIncludingNames(arg, actor);
            if (merch is null)
            {
                actor.OutputHandler.Send("There is no such merchandise profile in this shop.");
                return;
            }
        }

        ICharacter purchaser = account?.IsAccountOwner(actor) == false
            ? actor.Gameworld.TryGetCharacter(account.AccountOwnerId, true)
            : actor;
        shop.ShowDeals(actor, purchaser, merch);
    }

    [PlayerCommand("Preview", "preview")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("preview", @"The preview command allows you to see the specific items that you would buy if you used a particular combination of syntax for buy.

The syntax is as follows:

	#3preview <thing>#0 - previews buying a specified item
	#3preview <quantity> <thing>#0- previews buying the specified quantity of the thing", AutoHelp.HelpArgOrNoArg)]
    protected static void Preview(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsTrading || (shop as ITransientShop)?.CurrentStall?.IsTrading == false)
        {
            actor.OutputHandler.Send("This shop is not currently trading.");
            return;
        }

        string firstArg = ss.PopSpeech();
        string target = firstArg;
        int quantity = 1;
        if (!ss.IsFinished && int.TryParse(firstArg, out int newquantity))
        {
            if (newquantity < 1)
            {
                actor.OutputHandler.Send("You must specify a quantity that is 1 or more.");
                return;
            }

            quantity = newquantity;
            target = ss.PopSpeech();
        }

        IMerchandise merch = shop.StockedMerchandise.OrderBy(x => x.Name).GetFromItemListByKeywordIncludingNames(target, actor);
        if (merch == null)
        {
            actor.OutputHandler.Send(
                "This shop doesn't appear to be selling anything like that.\nSee LIST for a list of merchandise for sale.");
            return;
        }

        (bool truth, string reason) = shop.CanBuy(actor, merch, quantity, null);
        if (!truth)
        {
            actor.OutputHandler.Send(
                $"You cannot buy {quantity}x {merch.Item.ShortDescription.Colour(merch.Item.CustomColour ?? Telnet.Green)} because {reason}.");
            return;
        }

        decimal price = shop.PriceForMerchandise(actor, merch, quantity);
        List<IGameItem> items = new();
        int count = 0;

        foreach (IGameItem item in shop.StockedItems(merch))
        {
            if (count + item.Quantity <= quantity)
            {
                items.Add(item);
                count += item.Quantity;
            }
            else
            {
                items.Add(item.PeekSplit(quantity - count));
                count = quantity;
            }

            if (count >= quantity)
            {
                break;
            }
        }

        StringBuilder sb = new();
        sb.AppendLine(
            $"Previewing the purchase of {quantity}x {merch.Item.ShortDescription.Colour(merch.Item.CustomColour ?? Telnet.Green)}:");
        sb.AppendLine(
            $"The price would be {shop.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).Colour(Telnet.Green)}.");
        sb.AppendLine($"You would get the following specific items:");
        sb.AppendLine();
        foreach (IGameItem item in items)
        {
            sb.AppendLine($"\t{item.HowSeen(actor)}");
        }

        if (quantity <= 1)
        {
            IGameItem item = items[0];
            sb.AppendLine();
            sb.AppendLine(item.HowSeen(actor, type: DescriptionType.Full));
            sb.AppendLine();
            sb.AppendLine(item.Evaluate(actor));
            IContainer container = item.GetItemType<IContainer>();
            if (container is not null && container.Contents.Any())
            {
                sb.AppendLine();
                sb.AppendLine("It also contains the following items:\n");
                foreach (IGameItem content in container.Contents)
                {
                    sb.AppendLine($"\t{content.HowSeen(actor)}");
                }
            }

        }

        IGameItem citem = items.First();
        List<ICraft> crafts = (actor.IsAdministrator()
                         ? actor.Gameworld.Crafts
                         : actor.Gameworld.Crafts.Where(x => x.AppearInCraftsList(actor)))
                     .Where(x => x.Status == RevisionStatus.Current)
                     .Where(x =>
                         x.Inputs.Any(y => y.IsInput(citem)) ||
                         x.Products.Any(y => y.IsItem(citem)) ||
                         x.FailProducts.Any(y => y.IsItem(citem)) ||
                         x.Tools.Any(y => y.IsTool(citem))
                     )
                     .OrderBy(x => x.Category)
                     .ThenBy(x => x.Name)
                     .ToList();
        if (crafts.Any())
        {
            sb.AppendLine($"\nYou know the following crafts that can use or produce this item:\n");
            foreach (ICraft craft in crafts)
            {
                sb.AppendLine($"\t{craft.Name.ColourName()} {craft.Category.SquareBrackets().ColourValue()}");
            }
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    [PlayerCommand("Buy", "buy")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("buy",
        @"The buy command is used in shops to buy goods from the shop. It is used in conjunction with the LIST command to see merchandise for sale and the PREVIEW command to preview items for sale.

The syntax for this command is as follows:

	#3buy <thing>#0 - buys a specified item
	#3buy <quantity> <thing>#0- buys the specified quantity of the thing
	#3buy [<quantity>] <thing> account <accountname>#0 - buys the the thing with a line of credit account
	#3buy [<quantity>] <thing> with <item>#0 - buys the thing with a payment item such as a cheque, credit card, writ, etc.",
        AutoHelp.HelpArgOrNoArg)]
    protected static void Buy(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsTrading || (shop as ITransientShop)?.CurrentStall?.IsTrading == false)
        {
            actor.OutputHandler.Send("This shop is not currently trading.");
            return;
        }

        string firstArg = ss.PopSpeech();
        string target = firstArg;
        int quantity = 1;
        if (!ss.IsFinished && int.TryParse(firstArg, out int newquantity))
        {
            if (newquantity < 1)
            {
                actor.OutputHandler.Send("You must specify a quantity that is 1 or more.");
                return;
            }

            quantity = newquantity;
            target = ss.PopSpeech();
        }

        IMerchandise merch;
        if (int.TryParse(target, out int value))
        {
            merch = shop.StockedMerchandise.OrderBy(x => x.Name).ElementAtOrDefault(value - 1);
        }
        else
        {
            merch = shop.StockedMerchandise.OrderBy(x => x.Name).GetFromItemListByKeywordIncludingNames(target, actor);
        }

        if (merch == null)
        {
            actor.OutputHandler.Send(
                "This shop doesn't appear to be selling anything like that.\nSee LIST for a list of merchandise for sale.");
            return;
        }

        IPaymentMethod payment = null;
        if (ss.IsFinished)
        {
            payment = new ShopCashPayment(shop.Currency, shop, actor);
        }
        else
        {
            switch (ss.PopSpeech().ToLowerInvariant())
            {
                case "account":
                case "credit":
                case "cred":
                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send(
                            "What is the name of the line of credit account you'd like to bill to?");
                        return;
                    }

                    string accn = ss.PopSpeech();
                    ILineOfCreditAccount loc = shop.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(accn));
                    if (loc == null)
                    {
                        // TODO - echoed by shopkeep?
                        actor.OutputHandler.Send("There is no line of credit account associated with this shop.");
                        return;
                    }

                    payment = new LineOfCreditPayment(actor, loc);
                    break;
                case "with":
                case "card":
                case "keycard":
                    if (!actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
                    {
                        goto default;
                    }

                    if (shop.BankAccount is null || shop.BankAccount.Currency != shop.Currency)
                    {
                        actor.OutputHandler.Send("This shop does not accept non-cash payment.");
                        return;
                    }

                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send("What do you want to pay with?");
                        return;
                    }

                    IGameItem item = actor.TargetPersonalItem(ss.SafeRemainingArgument);
                    if (item is null)
                    {
                        actor.OutputHandler.Send("You don't see anything like that.");
                        return;
                    }

                    IBankPaymentItem paymentItem = item.GetItemType<IBankPaymentItem>();
                    if (paymentItem is null)
                    {
                        actor.OutputHandler.Send(
                            $"{item.HowSeen(actor, true)} is not something that be used to pay for things.");
                        return;
                    }

                    payment = new BankPayment(actor, paymentItem, shop);
                    break;
                default:
                    if (actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
                    {
                        actor.OutputHandler.Send(
                            $"If you specify an argument after the thing you want to buy, it must be either CREDIT <account name> or WITH <card>.");
                    }
                    else
                    {
                        actor.OutputHandler.Send(
                            $"If you specify an argument after the thing you want to buy, it must be CREDIT <account name>.");
                    }

                    return;
            }
        }

        (bool truth, string reason) = shop.CanBuy(actor, merch, quantity, payment);
        if (!truth)
        {
            actor.OutputHandler.Send(
                $"You cannot buy {quantity}x {merch.Item.ShortDescription.Colour(merch.Item.CustomColour ?? Telnet.Green)} because {reason}");
            return;
        }

        (decimal Price, IEnumerable<IGameItem> Items) preview = shop.PreviewBuy(actor, merch, quantity, payment);
        if (actor.Account.ActLawfully &&
            preview.Items.Any(x => CrimeTypes.PossessingContraband.CheckWouldBeACrime(actor, null, x, "")))
        {
            actor.OutputHandler.Send(
                $"That action would be a crime.\n{CrimeExtensions.StandardDisableIllegalFlagText}");
            return;
        }

        if (preview.Items.Any(x => x.Prototype.Morphs &&
                                  x.Prototype.MorphTimeSpan > TimeSpan.Zero &&
                                  (x.CachedMorphTime ??
                                   (x.MorphTime == DateTime.MinValue
                                       ? x.Prototype.MorphTimeSpan
                                       : x.MorphTime - DateTime.UtcNow)).TotalSeconds /
                                  x.Prototype.MorphTimeSpan.TotalSeconds < 0.3))
        {
            actor.OutputHandler.Send(
                $"Warning: That item will morph soon.\n{Accept.StandardAcceptPhrasing}");
            actor.AddEffect(new Accept(actor, new GenericProposal
            {
                DescriptionString = "confirming near-morph purchase",
                AcceptAction = text =>
                {
                    List<IGameItem> bought = shop.Buy(actor, merch, quantity, payment).ToList();
                    foreach (IGameItem contrabandItem in bought)
                    {
                        CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.PossessingContraband, null,
                            contrabandItem, "");
                    }
                },
                RejectAction = text => { actor.OutputHandler.Send("You decide not to buy it."); },
                ExpireAction = () => { actor.OutputHandler.Send("You decide not to buy it."); },
                Keywords = new List<string> { "buy" }
            }), TimeSpan.FromSeconds(120));
            return;
        }

        List<IGameItem> boughtItems = shop.Buy(actor, merch, quantity, payment).ToList();
        foreach (IGameItem boughtContrabandItem in boughtItems)
        {
            CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.PossessingContraband, null,
                boughtContrabandItem, "");
        }

    }
    [PlayerCommand("Sell", "Sell")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("sell", @"The sell command is used to sell items that you have to a shop. Not all shops buy items, and shops have to be explicitly set to buy the items that you're selling. Finally, if you're holding a stack of something, you will be trying to sell the whole stack. Try splitting it up if you want to sell less.

The syntax for this command is as follows:

	#3sell <item>#0", AutoHelp.HelpArgOrNoArg)]
    protected static void Sell(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        IGameItem item = actor.TargetHeldItem(ss.PopSpeech());
        if (item is null)
        {
            actor.OutputHandler.Send("You aren't holding anything like that.");
            return;
        }

        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsTrading || (shop as ITransientShop)?.CurrentStall?.IsTrading == false)
        {
            actor.OutputHandler.Send("This shop is not currently trading.");
            return;
        }

        IMerchandise merch = null;
        if (!ss.IsFinished)
        {
            merch = shop.Merchandises
                        .Where(x => x.IsMerchandiseFor(item, true))
                        .GetFromItemListByKeyword(ss.PopSpeech(), actor);
            if (merch == null)
            {
                actor.OutputHandler.Send(
                    $"There is no matching merchandise profile for {item.HowSeen(actor)} with the specified keywords. The shop will not buy that item.");
                return;
            }
        }
        else
        {
            merch = shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item)) ??
                    shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item, true));
            if (merch == null)
            {
                actor.OutputHandler.Send(
                    $"There is no merchandise profile for items like {item.HowSeen(actor)}. The shop will not buy that item.");
                return;
            }
        }

        if (!merch.WillBuy)
        {
            actor.OutputHandler.Send($"This shop does not buy items like {item.HowSeen(actor)}.");
            return;
        }

        double condition = Math.Min(item.Condition, item.DamageCondition);
        if (condition < merch.MinimumConditionToBuy)
        {
            actor.OutputHandler.Send($"Unfortunately, {item.HowSeen(actor)} is in too poor condition for this shop to accept.");
            return;
        }

        IPaymentMethod payment = null;
        if (ss.IsFinished)
        {
            payment = new ShopCashPayment(shop.Currency, shop, actor);
        }
        else
        {
            switch (ss.PopSpeech().ToLowerInvariant())
            {
                case "account":
                case "credit":
                case "cred":
                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send(
                            "What is the name of the line of credit account you'd like to credit to?");
                        return;
                    }

                    string accn = ss.PopSpeech();
                    ILineOfCreditAccount loc = shop.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(accn));
                    if (loc == null)
                    {
                        // TODO - echoed by shopkeep?
                        actor.OutputHandler.Send("There is no such line of credit account associated with this shop.");
                        return;
                    }

                    payment = new LineOfCreditPayment(actor, loc);
                    break;
                case "with":
                case "card":
                case "keycard":
                    if (!actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
                    {
                        goto default;
                    }

                    if (shop.BankAccount is null || shop.BankAccount.Currency != shop.Currency)
                    {
                        actor.OutputHandler.Send("This shop does not accept non-cash payment.");
                        return;
                    }

                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send("What payment item do you want to be paid to?");
                        return;
                    }

                    IGameItem targetItem = actor.TargetPersonalItem(ss.SafeRemainingArgument);
                    if (targetItem is null)
                    {
                        actor.OutputHandler.Send("You don't see anything like that.");
                        return;
                    }

                    IBankPaymentItem paymentItem = targetItem.GetItemType<IBankPaymentItem>();
                    if (paymentItem is null)
                    {
                        actor.OutputHandler.Send(
                            $"{targetItem.HowSeen(actor, true)} is not something that be used to pay for things.");
                        return;
                    }

                    payment = new BankPayment(actor, paymentItem, shop);
                    break;
                default:
                    if (actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
                    {
                        actor.OutputHandler.Send(
                            $"If you specify an argument after the thing you want to sell, it must be either CREDIT <account name> or WITH <card>.");
                    }
                    else
                    {
                        actor.OutputHandler.Send(
                            $"If you specify an argument after the thing you want to sell, it must be CREDIT <account name>.");
                    }

                    return;
            }
        }

        (bool truth, string reason) = shop.CanSell(actor, merch, payment, item);
        if (!truth)
        {
            actor.OutputHandler.Send(
                $"You cannot sell {item.HowSeen(actor)} because {reason}.");
            return;
        }

        if (actor.Account.ActLawfully && CrimeTypes.SellingContraband.CheckWouldBeACrime(actor, null, item, ""))
        {
            actor.OutputHandler.Send(
                $"That action would be a crime.\n{CrimeExtensions.StandardDisableIllegalFlagText}");
            return;
        }

        shop.Sell(actor, merch, payment, item);
    }

    [PlayerCommand("Shop", "shop")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("shop", ShopHelpPlayers, AutoHelp.HelpArgOrNoArg, ShopHelpAdmins)]
    protected static void Shop(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "reprice":
                ShopReprice(actor, ss);
                return;
            case "clockin":
            case "clock in":
            case "clock-in":
                ShopClockIn(actor);
                return;
            case "clockout":
            case "clock out":
            case "clock-out":
                ShopClockOut(actor);
                return;
            case "employ":
                ShopEmploy(actor, ss);
                return;
            case "fire":
                ShopFire(actor, ss);
                return;
            case "manager":
                ShopManager(actor, ss);
                return;
            case "proprietor":
                ShopProprietor(actor, ss);
                return;
            case "till":
                ShopTill(actor, ss);
                return;
            case "display":
                ShopDisplay(actor, ss);
                return;
            case "stock":
                ShopStock(actor, ss);
                return;
            case "merch":
            case "merchandise":
                ShopMerchandise(actor, ss);
                return;
            case "deal":
            case "deals":
                ShopDeals(actor, ss);
                return;
            case "set":
                ShopSet(actor, ss);
                return;
            case "quit":
                ShopQuit(actor, ss);
                return;
            case "dispose":
                ShopDispose(actor, ss);
                return;
            case "ledger":
                ShopLedger(actor, ss);
                return;
            case "bank":
                ShopBank(actor, ss);
                return;
            case "account":
                ShopAccount(actor, ss);
                return;
            case "payaccount":
                ShopPayAccount(actor, ss);
                return;
            case "accountstatus":
                ShopAccountStatus(actor, ss);
                return;
            case "paytax":
            case "paytaxes":
                ShopPayTax(actor, ss);
                return;
            case "info":
                ShopInfo(actor, ss);
                return;
            case "autostock":
                ShopAutostock(actor, ss);
                return;
            case "open":
                ShopOpen(actor, ss);
                return;
            case "close":
                ShopClose(actor, ss);
                return;
            case "markup":
                ShopMarkup(actor, ss);
                return;
        }

        if (actor.IsAdministrator())
        {
            switch (ss.Last.ToLowerInvariant())
            {
                case "setupstall":
                    ShopSetupStall(actor, ss);
                    return;
                case "create":
                    ShopCreate(actor, ss);
                    return;
                case "createstall":
                    ShopCreateStall(actor, ss);
                    return;
                case "list":
                    ShopList(actor, ss);
                    return;
                case "economy":
                    ShopEconomy(actor, ss);
                    return;
                case "delete":
                    ShopDelete(actor, ss);
                    return;
                case "extend":
                    ShopExtend(actor, ss);
                    return;
                case "remove":
                    ShopRemove(actor, ss);
                    return;
                case "rezone":
                    ShopRezone(actor, ss);
                    return;
            }
        }

        actor.OutputHandler.Send((actor.IsAdministrator() ? ShopHelpAdmins : ShopHelpPlayers).SubstituteANSIColour());
    }

    #region Shop Subcommands

    private const string ShopHelpPlayers = @"You can use the following options with the shop command:

	#3shop payaccount <account> <amount>#0 - pays off a line of credit account
	#3shop paytax <amount>|all#0 - pays owing taxes out of all sources of available cash
	#3shop accountstatus <account>#0 - inquires about the status of a line of credit account
	#3shop account ...#0 - allows store managers to configure line of credit accounts. See #3SHOP ACCOUNT HELP#0.
	#3shop clockin#0 - clocks in as an on-duty employee
	#3shop clockout#0 - clocks out as an off-duty employee	
	#3shop employ <target>#0 - employs someone with the store
	#3shop quit#0 - quits employment with this store
	#3shop fire <target>|<name>#0 - fires an employee from this store
	#3shop manager <target>|<name>#0 - toggles an employee's status as a manager
	#3shop proprietor <target>|<name>#0 - toggles and employee's status as a proprietor
	#3shop till <target>#0 - toggles an item being used as a till for the store
	#3shop display <target>#0 - toggles an item being used as a display cabinet for the store
	#3shop info#0 - shows detailed information about the shop
	#3shop stock <target>#0 - adds an item as shop inventory
	#3shop dispose <target>#0 - disposes of an item from shop inventory
	#3shop ledger [<period#>]#0 - views the financial ledger for the shop
	#3shop bank <account>#0 - sets the bank account for the shop
	#3shop bank none#0 - sets this shop to no longer use a bank account
	#3shop merchandise <other args>#0 - edits merchandise. See #3SHOP MERCHANDISE HELP#0.
	#3shop deals <other args>#0 - edits shop deals. See #3SHOP DEALS HELP#0.
	#3shop open <shop>#0 - opens a shop for trading
	#3shop close <shop>#0 - closes a shop to trading
	#3shop reprice <%>#0 - reprices all merchandise by the specified percentage
	#3shop markup <%>#0 - bulk changes the markup for all merchandise to x%
	#3shop set name <name>#0 - renames a shop
	#3shop set can <prog> <whyprog>#0 - sets a prog to control who can shop here (and associated error message)
	#3shop set trading#0 - toggles whether this shop is trading
	#3shop set minfloat <amount>#0 - sets the minimum float for the shop to buy anything
	#3shop set market <which>#0 - sets a market to draw pricing multipliers from
	#3shop set market none#0 - clears the market pricing";

    private const string ShopHelpAdmins = @"You can use the following options with the shop command:

	#3shop payaccount <account> <amount>#0 - pays off a line of credit account
	#3shop paytax <amount>|all#0 - pays owing taxes out of all sources of available cash
	#3shop accountstatus <account>#0 - inquires about the status of a line of credit account
	#3shop account ...#0 - allows store managers to configure line of credit accounts. See #3SHOP ACCOUNT HELP#0.
	#3shop clockin#0 - clocks in as an on-duty employee
	#3shop clockout#0 - clocks out as an off-duty employee	
	#3shop employ <target>#0 - employs someone with the store
	#3shop quit#0 - quits employment with this store
	#3shop fire <target>|<name>#0 - fires an employee from this store
	#3shop manager <target>|<name>#0 - toggles an employee's status as a manager
	#3shop proprietor <target>|<name>#0 - toggles and employee's status as a proprietor
	#3shop till <target>#0 - toggles an item being used as a till for the store
	#3shop display <target>#0 - toggles an item being used as a display cabinet for the store
	#3shop info#0 - shows detailed information about the shop
	#3shop stock <target>#0 - adds an item as shop inventory
	#3shop dispose <target>#0 - disposes of an item from shop inventory
	#3shop ledger [<period#>]#0 - views the financial ledger for the shop
	#3shop bank <account>#0 - sets the bank account for the shop
	#3shop bank none#0 - sets this shop to no longer use a bank account
	#3shop merchandise <other args>#0 - edits merchandise. See #3SHOP MERCHANDISE HELP#0.
	#3shop deals <other args>#0 - edits shop deals. See #3SHOP DEALS HELP#0.
	#3shop open <shop>#0 - opens a shop for trading
	#3shop close <shop>#0 - closes a shop to trading
	#3shop reprice <%>#0 - reprices all merchandise by the specified percentage
	#3shop markup <%>#0 - bulk changes the markup for all merchandise to x%
	#3shop set name <name>#0 - renames a shop
	#3shop set can <prog> <whyprog>#0 - sets a prog to control who can shop here (and associated error message)
	#3shop set trading#0 - toggles whether this shop is trading
	#3shop set minfloat <amount>#0 - sets the minimum float for the shop to buy anything
	#3shop set market <which>#0 - sets a market to draw pricing multipliers from
	#3shop set market none#0 - clears the market pricing

Additionally, you can use the following shop admin subcommands:

	#3shop list#0 - lists all shops
	#3shop info <which>#0 - shows a shop you're not in
	#3shop economy#0 - a modified list that shows some economic info
	#3shop create <name> <econzone>#0 - creates a new fixed-location store with the specified name
	#3shop createstall <name> <econzone>#0 - creates a new transient (stall) store with the specified name
	#3shop delete#0 - deletes the shop you're currently in. Warning: Irreversible.
	#3shop extend <direction> [stockroom|workshop]#0 - extends the shop in the specified direction, optionally as the stockroom or workshop
	#3shop extend <shop> <direction> [stockroom|workshop]#0 - extends the specified shop in the specified direction, optionally as the stockroom or workshop
	#3shop remove#0 - removes the current location from its shop.
	#3shop autostock#0 - automatically loads and stocks items up to the minimum reorder levels for all merchandise
	#3shop setupstall <stall> <shop>#0 - sets up a stall item as belonging to a shop
	#3shop rezone <zone>#0 - changes the economic zone of a shop";

    private static void ShopMarkup(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("By what percentage should all merchandise markups in this store be adjusted?");
            return;
        }

        if (!ss.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out decimal value) || value <= -1.0M)
        {
            actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid percentage greater than {(-1.0).ToStringP2Colour(actor)}.");
            return;
        }

        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send($"You are not a manager of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        foreach (IMerchandise merch in shop.Merchandises)
        {
            merch.SalesMarkupMultiplier = (1.0M + value);
            merch.Changed = true;
        }

        actor.OutputHandler.Send($"You change the markup of all merchandise in this store to {value.ToStringP2Colour(actor)}.");
    }

    private static void ShopReprice(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("By what percentage should all prices in this store be adjusted?");
            return;
        }

        if (!ss.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out decimal value) || value <= 0.0M)
        {
            actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid percentage greater than zero.");
            return;
        }

        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send($"You are not a manager of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        foreach (IMerchandise merch in shop.Merchandises)
        {
            merch.Reprice(value);
        }

        actor.OutputHandler.Send($"You reprice all of the merchandise by {value.ToStringP2Colour(actor)}.");
    }

    private static void ShopRezone(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which Economic Zone do you want to move this shop to?");
            return;
        }

        IEconomicZone zone = actor.Gameworld.EconomicZones.GetByIdOrName(ss.SafeRemainingArgument);
        if (zone is null)
        {
            actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid economic zone.");
            return;
        }

        if (shop.EconomicZone == zone)
        {
            actor.OutputHandler.Send($"{shop.Name.TitleCase().ColourName()} is already in the {zone.Name.ColourValue()} economic zone.");
            return;
        }

        actor.OutputHandler.Send($"You move the {shop.Name.TitleCase().ColourName()} shop from the {shop.EconomicZone.Name.ColourValue()} economic zone to the {zone.Name.ColourValue()} economic zone.");
        shop.EconomicZone = zone;
    }

    private static void ShopClose(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }


        if (!actor.IsAdministrator() && !shop.IsEmployee(actor))
        {
            actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().ColourName()}.");
            return;
        }

        ITransientShop tShop = shop as ITransientShop;
        if (tShop is not null)
        {
            IShopStall stall = actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<IShopStall>()).FirstOrDefault(x => x.Shop == shop);
            if (stall is null)
            {
                actor.OutputHandler.Send($"There is no stall for {shop.Name.TitleCase().ColourName()} in this location.");
                return;
            }

            if (!stall.IsTrading)
            {
                actor.OutputHandler.Send($"{stall.Parent.HowSeen(actor, true)} is not trading.");
                return;
            }

            stall.IsTrading = false;
            if (shop.IsTrading)
            {
                shop.ToggleIsTrading();
            }
            actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ close|closes $1 from business", actor, actor, stall.Parent)));
            return;
        }

        if (!shop.IsTrading)
        {
            actor.OutputHandler.Send($"{shop.Name.TitleCase().ColourName()} is not trading.");
            return;
        }

        shop.ToggleIsTrading();
        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ close|closes the shop for business.", actor, actor)));
    }

    private static void ShopOpen(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!actor.IsAdministrator() && !shop.IsEmployee(actor))
        {
            actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().ColourName()}.");
            return;
        }

        ITransientShop tShop = shop as ITransientShop;
        if (tShop is not null)
        {
            IShopStall stall = actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<IShopStall>()).FirstOrDefault(x => x.Shop == shop);
            if (stall is null)
            {
                actor.OutputHandler.Send($"There is no stall for {shop.Name.TitleCase().ColourName()} in this location.");
                return;
            }

            if (stall.IsTrading)
            {
                actor.OutputHandler.Send($"{stall.Parent.HowSeen(actor, true)} is already trading.");
                return;
            }

            stall.IsTrading = true;
            if (!shop.IsTrading)
            {
                shop.ToggleIsTrading();
            }
            actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ open|opens $1 for business, trading as {shop.Name.TitleCase().ColourName()}.", actor, actor, stall.Parent)));
            return;
        }

        if (shop.IsTrading)
        {
            actor.OutputHandler.Send($"{shop.Name.TitleCase().ColourName()} is already trading.");
            return;
        }

        shop.ToggleIsTrading();
        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ open|opens the shop for business.", actor, actor)));
    }

    private static void ShopSetupStall(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which shop stall item did you want to set up?");
            return;
        }

        IGameItem item = actor.TargetItem(ss.PopSpeech());
        if (item == null)
        {
            actor.OutputHandler.Send("You don't see anything like that.");
            return;
        }

        IShopStall itemAsStall = item.GetItemType<IShopStall>();
        if (itemAsStall is null)
        {
            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not a shop stall.");
            return;
        }

        if (itemAsStall.IsTrading)
        {
            actor.OutputHandler.Send("You can't change the shop associated with a trading stall. Close the stall first.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What shop do you want to set this stall to be associated with?");
            return;
        }

        if (ss.SafeRemainingArgument.Equals("none"))
        {
            if (itemAsStall.Shop is not null)
            {
                ITransientShop oldShop = itemAsStall.Shop;
                itemAsStall.Shop = null;
                oldShop.CurrentStall = null;
                oldShop.StocktakeAllMerchandise();
            }

            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is no longer affiliated with any shop.");
            return;
        }

        IShop shop = actor.Gameworld.Shops.GetByIdOrName(ss.SafeRemainingArgument);
        if (shop is null)
        {
            actor.OutputHandler.Send("There is no such shop.");
            return;
        }

        ITransientShop tShop = shop as ITransientShop;
        if (tShop is null)
        {
            actor.OutputHandler.Send($"{shop.Name.TitleCase().ColourName()} is not a transient shop and so cannot be used with a stall.");
            return;
        }

        if (tShop.CurrentStall is not null)
        {
            if (tShop.CurrentStall == itemAsStall)
            {
                actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is already the stall for that shop.");
                return;
            }

            tShop.CurrentStall.Shop = null;
            tShop.CurrentStall.IsTrading = false;
        }

        itemAsStall.Shop = tShop;
        tShop.CurrentStall = itemAsStall;
        tShop.StocktakeAllMerchandise();
        actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is now affiliated with the {shop.Name.TitleCase().ColourName()} shop.");
    }

    private static bool DoShopCommandFindShop(ICharacter actor, out IShop shop)
    {
        if (actor.Location.Shop is not null)
        {
            shop = actor.Location.Shop;
            return true;
        }

        IShopStall stall = actor.Location.
            LayerGameItems(actor.RoomLayer).
            SelectNotNull(x => x.GetItemType<IShopStall>()).
            FirstOrDefault(x => x.Shop is not null);
        if (stall is null)
        {
            actor.OutputHandler.Send("You are not currently at a shop or in the presence of a market stall.");
            shop = null;
            return false;
        }

        shop = stall.Shop;
        return true;
    }

    private static void ShopBank(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send(
                "You are not a proprietor of this establishment and so cannot alter its bank account details.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either specify a bank account or use {"none".ColourCommand()} to clear one.");
            return;
        }

        if (ss.PeekSpeech().EqualTo("none"))
        {
            shop.BankAccount = null;
            actor.OutputHandler.Send("This shop no longer has a bank account.");
            return;
        }

        (IBankAccount account, string error) = Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
        if (account is null)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        if (!account.IsAuthorisedAccountUser(actor))
        {
            actor.OutputHandler.Send("You are not authorised to use that bank account.");
            return;
        }

        shop.BankAccount = account;
        actor.OutputHandler.Send(
            $"This shop will now use the {account.AccountReference.ColourValue()} bank account for its transactions.");
    }

    private static void ShopPayTax(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not a manager of this establishment and so cannot pay its taxes.");
            return;
        }

        decimal amount;
        if (ss.SafeRemainingArgument.EqualTo("all"))
        {
            amount = shop.EconomicZone.OutstandingTaxesForShop(shop);
        }
        else
        {
            amount = shop.Currency.GetBaseCurrency(ss.SafeRemainingArgument, out bool success);
            if (!success || amount <= 0.0M)
            {
                actor.OutputHandler.Send(
                    $"That is not a valid amount of the {shop.Currency.Name.ColourName()} currency.");
                return;
            }
        }

        if (amount <= 0.0M)
        {
            actor.OutputHandler.Send("This shop does not owe any money in taxes.");
            return;
        }

        decimal available = shop.AvailableCashFromAllSources();

        if (available < amount)
        {
            actor.OutputHandler.Send(
                $"This shop cannot afford to pay that much. The most it could pay is {shop.Currency.Describe(available, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
            return;
        }

        shop.TakeCashFromAllSources(amount, "Paying taxes");
        shop.EconomicZone.PayTaxesForShop(shop, amount);
        shop.AddTransaction(new TransactionRecord(ShopTransactionType.TaxPayment, shop.Currency, shop,
            shop.EconomicZone.ZoneForTimePurposes.DateTime(), actor, amount, 0.0M, null));
        actor.OutputHandler.Send(
            $"You pay {shop.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in taxes for {shop.Name.TitleCase().ColourName()}. The shop now owes {shop.Currency.Describe(shop.EconomicZone.OutstandingTaxesForShop(shop), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in taxes.");
    }
    private static void ShopAccountStatus(ICharacter actor, StringStack command)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsTrading)
        {
            actor.OutputHandler.Send("This shop is not currently trading.");
            return;
        }

        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        if (account.AccountUsers.All(x => x.Id != actor.Id) && !actor.IsAdministrator() && !shop.IsEmployee(actor))
        {
            actor.OutputHandler.Send("You are not authorised to view that account.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine(
            $"Line of Credit Account {account.AccountName.ColourName()} for {shop.Name.TitleCase().ColourName()}");
        sb.AppendLine(account.IsSuspended
            ? $"The account is currently suspended from trading.".Colour(Telnet.BoldMagenta)
            : $"The account is currently in good standing.".Colour(Telnet.BoldGreen));
        sb.AppendLine(
            $"The account has a limit of {shop.Currency.Describe(account.AccountLimit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
        sb.AppendLine(
            $"The account has an outstanding balance of {shop.Currency.Describe(account.OutstandingBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
        sb.AppendLine(
            $"You are personally authorised to spend {shop.Currency.Describe(account.MaximumAuthorisedToUse(actor), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
        if (actor.Id == account.AccountOwnerId)
        {
            sb.AppendLine();
            sb.AppendLine("The following people are authorised to use this account:");
            foreach (LineOfCreditAccountUser user in account.AccountUsers)
            {
                sb.AppendLine(
                    $"\t{user.PersonalName.GetName(NameStyle.FullName).ColourName()}{(!user.SpendingLimit.HasValue ? " [no limit]".ColourValue() : $" [{shop.Currency.Describe(user.SpendingLimit.Value, CurrencyDescriptionPatternType.ShortDecimal)}]".ColourValue())}");
            }
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ShopPayAccount(ICharacter actor, StringStack command)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsReadyToDoBusiness)
        {
            actor.OutputHandler.Send("This shop has not been properly set up to do business.");
            return;
        }

        if (!shop.IsTrading)
        {
            actor.OutputHandler.Send("This shop is not currently trading.");
            return;
        }

        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How much do you want to pay towards that account's outstanding balance?");
            return;
        }

        decimal amount = Math.Truncate(shop.Currency.GetBaseCurrency(command.SafeRemainingArgument, out bool success));
        if (!success || amount <= 0.0M)
        {
            actor.OutputHandler.Send($"That is not a valid amount of the {shop.Currency.Name.ColourName()} currency.");
            return;
        }

        ShopCashPayment payment = new(shop.Currency, shop, actor);
        if (payment.AccessibleMoneyForPayment() < amount)
        {
            actor.OutputHandler.Send("You do not have enough cash on hand to pay that amount.");
            return;
        }

        payment.TakePayment(amount);
        account.PayoffAccount(amount);
        shop.ExpectedCashBalance += amount;
        shop.AddTransaction(new TransactionRecord(ShopTransactionType.Deposit, shop.Currency, shop,
            actor.Location.DateTime(), actor, amount, 0.0M, null));
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(
            $"@ pay|pays $1 towards the {account.AccountName.ColourName()} line of credit account.", actor, actor,
            new DummyPerceivable(shop.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)))));
    }

    public const string ShopAccountHelp = @"You can use the following options with this subcommand:

	#3shop account list#0 - shows all line of credit accounts for this shop
	#3shop account show <account>#0 - shows information about a specific account
	#3shop account create <name> <owner> <spending limit>#0 - creates a new credit account
	#3shop account close <name>#0 - closes a credit account
	#3shop account authorise <account> <who> [<limit>]#0 - authorises someone to use an account with optional transaction limit
	#3shop account deauthorise <account> <who>#0 - deauthorises someone from an account
	#3shop account suspend <account>#0 - toggles trading on an account
	#3shop account owner <account> <who>#0 - sets a new account owner
	#3shop account limit <account> <amount>#0 - sets a new account spending limit
	#3shop account userlimit <account> <user> <limit>#0 - sets a new user-specific spending limit (use 0 for no limit)";

    private static void ShopAccount(ICharacter actor, StringStack command)
    {
        if (command.PeekSpeech().EqualToAny("?", "help"))
        {
            actor.OutputHandler.Send(ShopAccountHelp.SubstituteANSIColour());
            return;
        }

        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!actor.Location.Shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send($"You are not a manager of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
            case "new":
            case "create":
                ShopAccountCreate(actor, shop, command);
                return;
            case "remove":
            case "rem":
            case "delete":
            case "close":
                ShopAccountClose(actor, shop, command);
                return;
            case "authorise":
            case "authorize":
                ShopAccountAuthorise(actor, shop, command);
                return;
            case "deauthorise":
            case "deauthorize":
                ShopAccountDeauthorise(actor, shop, command);
                return;
            case "suspend":
                ShopAccountSuspend(actor, shop, command);
                return;
            case "limit":
                ShopAccountLimit(actor, shop, command);
                return;
            case "userlimit":
            case "user limit":
            case "user_limit":
                ShopAccountUserLimit(actor, shop, command);
                return;
            case "owner":
                ShopAccountOwner(actor, shop, command);
                return;
            case "list":
                ShopAccountList(actor, shop, command);
                return;
            case "show":
                ShopAccountShow(actor, shop, command);
                return;
            default:
                actor.OutputHandler.Send(ShopAccountHelp.SubstituteANSIColour());
                break;
        }
    }

    private static void ShopAccountShow(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        actor.OutputHandler.Send(account.Show(actor));
    }

    private static void ShopAccountList(ICharacter actor, IShop shop, StringStack command)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Line of Credit Accounts for {shop.Name.TitleCase().ColourName()}:");
        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from account in shop.LineOfCreditAccounts
            select new List<string>
            {
                account.AccountName,
                account.AccountOwnerName.GetName(NameStyle.FullName),
                shop.Currency.Describe(account.AccountLimit, CurrencyDescriptionPatternType.ShortDecimal),
                shop.Currency.Describe(account.OutstandingBalance, CurrencyDescriptionPatternType.ShortDecimal),
                account.AccountUsers.Count().ToString("N0", actor),
                account.IsSuspended.ToString()
            },
            new List<string> { "Name", "Owner", "Limit", "Outstanding", "# Users", "Suspended?" },
            actor.LineFormatLength,
            colour: Telnet.BoldYellow,
            unicodeTable: actor.Account.UseUnicode
        ));
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ShopAccountOwner(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Who do you want to set as the new owner of the {account.AccountName.ColourName()} account?");
            return;
        }

        ICharacter target = actor.TargetActor(command.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anyone like that here.");
            return;
        }

        account.SetAccountOwner(target);
        actor.OutputHandler.Send(
            $"You change the owner of the {account.AccountName.ColourName()} account to {target.HowSeen(actor)}.");
    }

    private static ILineOfCreditAccount GetAccount(ICharacter actor, IShop shop, string whichAccount)
    {
        if (string.IsNullOrWhiteSpace(whichAccount))
        {
            actor.OutputHandler.Send("Which line of credit account do you want to edit?");
            return null;
        }

        ILineOfCreditAccount account = shop.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(whichAccount));
        if (account == null)
        {
            actor.OutputHandler.Send("There is no such line of credit account.");
            return null;
        }

        return account;
    }

    private static void ShopAccountUserLimit(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which account user did you want to set the limit for?");
            return;
        }

        LineOfCreditAccountUser user = account.AccountUsers.GetByPersonalName(command.SafeRemainingArgument);
        if (user == null)
        {
            actor.OutputHandler.Send(
                $"There is no such account user for that account. The authorised users are as follows:\n{account.AccountUsers.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToCommaSeparatedValues("\n")}");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What limit do you want to set on that user's spending on the {account.AccountName.ColourName()} account?");
            return;
        }

        decimal amount = account.Currency.GetBaseCurrency(command.SafeRemainingArgument, out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid amount of currency.");
            return;
        }

        account.SetLimit(user, amount);
        actor.OutputHandler.Send(
            $"You set the spending limit for {user.PersonalName.GetName(NameStyle.FullName).ColourName()} on the {account.AccountName.ColourName()} account to {account.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
    }

    private static void ShopAccountLimit(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What limit do you want to put on the outstanding balance of that account?");
            return;
        }

        decimal amount = account.Currency.GetBaseCurrency(command.SafeRemainingArgument, out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid amount of currency.");
            return;
        }

        account.SetAccountLimit(amount);
        actor.OutputHandler.Send(
            $"You set the total outstanding balance limit on the {account.AccountName.ColourName()} account to {account.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
    }

    private static void ShopAccountSuspend(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        account.IsSuspended = !account.IsSuspended;
        if (account.IsSuspended)
        {
            actor.OutputHandler.Send($"You suspend trading on the {account.AccountName.ColourName()} account.");
        }
        else
        {
            actor.OutputHandler.Send(
                $"You remove the suspension of trading on the {account.AccountName.ColourName()} account.");
        }
    }

    private static void ShopAccountDeauthorise(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which account user do you want to deauthorise? The current users are as follows:\n{account.AccountUsers.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToCommaSeparatedValues("\n")}");
            return;
        }

        LineOfCreditAccountUser user = account.AccountUsers.GetByPersonalName(command.SafeRemainingArgument);
        if (user == null)
        {
            actor.OutputHandler.Send(
                $"There is no such authorised user for that account. The current users are as follows:\n{account.AccountUsers.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToCommaSeparatedValues("\n")}");
            return;
        }

        if (user.Id == account.AccountOwnerId)
        {
            actor.OutputHandler.Send(
                "You cannot deauthorise the account owner. If you want to remove this person you must first set a new account owner.");
            return;
        }

        account.RemoveAuthorisation(user);
        actor.OutputHandler.Send(
            $"You remove {user.PersonalName.GetName(NameStyle.FullName).ColourName()} as an authorised user for the {account.AccountName.ColourName()} line of credit account.");
    }

    private static void ShopAccountAuthorise(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you want to authorise to use that line of credit account?");
            return;
        }

        ICharacter target = actor.TargetActor(command.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anyone like that.");
            return;
        }

        decimal value = 0.0M;
        if (!command.IsFinished)
        {
            value = account.Currency.GetBaseCurrency(command.SafeRemainingArgument, out bool success);
            if (!success)
            {
                actor.OutputHandler.Send("That is not a valid amount of currency for the user's spending limit.");
                return;
            }
        }

        if (account.AccountUsers.Any(x => x.Id == target.Id))
        {
            actor.OutputHandler.Send(
                $"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is already an authorised user of that account.");
            return;
        }

        account.AddAuthorisation(target, value);
        actor.OutputHandler.Send(
            $"You add {target.HowSeen(actor)} as an authorised user for the {account.AccountName.ColourName()} line of credit account{(value > 0.0M ? $" with a spendling limit of {account.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}" : "")}.");
    }

    private static void ShopAccountClose(ICharacter actor, IShop shop, StringStack command)
    {
        ILineOfCreditAccount account = GetAccount(actor, shop, command.PopSpeech());
        if (account == null)
        {
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure you want to permanently close the {account.AccountName.ColourName()} line of credit account? This action is irreversible.\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                actor.OutputHandler.Send($"You close the {account.AccountName.ColourName()} line of credit account.");
                shop.RemoveLineOfCredit(account);
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to close the {account.AccountName.ColourName()} line of credit account.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to close the {account.AccountName.ColourName()} line of credit account.");
            },
            Keywords = new List<string> { "close", "credit", "account" },
            DescriptionString = "Closing a line of credit account"
        }));
        throw new NotImplementedException();
    }

    private static void ShopAccountCreate(ICharacter actor, IShop shop, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this line of credit account?");
            return;
        }

        string name = command.PopSpeech();
        if (shop.LineOfCreditAccounts.Any(x => x.AccountName.EqualTo(name)))
        {
            actor.OutputHandler.Send(
                "There is already a line of credit account with that name. Names must be unique.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you want to set as the owner of that line of credit account?");
            return;
        }

        ICharacter target = actor.TargetActor(command.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anyone like that.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What outstanding balance limit do you want to put on the new account?");
            return;
        }

        decimal amount = shop.Currency.GetBaseCurrency(command.SafeRemainingArgument, out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid amount of currency.");
            return;
        }

        LineOfCreditAccount account = new(shop, name, target, amount);
        actor.Gameworld.Add(account);
        shop.AddLineOfCredit(account);
        actor.OutputHandler.Send(
            $"You create a new line of credit account for {shop.Name.TitleCase().ColourName()} called {name.ColourName()} owned by {target.HowSeen(actor)} with an outstanding balance limit of {shop.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
    }

    private static void ShopAutostock(ICharacter actor, StringStack command)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }


        if (!actor.IsAdministrator() && !shop.IsEmployee(actor))
        {
            actor.OutputHandler.Send("You are not authorised to bring items into stock in this store.");
            return;
        }

        List<IGameItem> stocked = new();
        if (actor.IsAdministrator())
        {
            foreach (IMerchandise merchandise in shop.Merchandises)
            {
                stocked.AddRange(shop.DoAutoRestockForMerchandise(merchandise));
            }
        }

        stocked.AddRange(shop.DoAutostockAllMerchandise());

        StringBuilder sb = new();
        sb.AppendLine("You add the following items to stock:");
        foreach (IGameItem item in stocked)
        {
            sb.AppendLine($"\t{item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}");
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ShopRemove(ICharacter actor, StringStack command)
    {
        IPermanentShop shop = actor.Location.Shop;
        if (shop == null)
        {
            actor.OutputHandler.Send("You are not currently in a shop.");
            return;
        }

        if (shop.ShopfrontCells.Contains(actor.Location) && shop.ShopfrontCells.Count() == 1)
        {
            actor.OutputHandler.Send(
                "You cannot remove the last shopfront location from a shop. You must add a new one before you can remove this one.");
            return;
        }

        ICell location = actor.Location;
        actor.OutputHandler.Send(
            $"Are you sure you want to remove this location from the shop?\nType {"accept".ColourCommand()} to accept or {"decline".ColourCommand()} to change your mind.");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You remove {location.HowSeen(actor)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                if (location == shop.WorkshopCell)
                {
                    shop.WorkshopCell = null;
                    return;
                }

                if (location == shop.StockroomCell)
                {
                    shop.StockroomCell = null;
                    return;
                }

                shop.RemoveShopfrontCell(location);
                location.Shop = null;
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to remove this location from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to remove this location from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            DescriptionString = $"removing a location from {shop.Name.TitleCase()}",
            Keywords = new List<string> { "remove", "shop", "location" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopExtend(ICharacter actor, StringStack command)
    {
        IPermanentShop shop = actor.Location.Shop;
        if (shop == null)
        {
            if (!command.IsFinished)
            {
                string shopText = command.PopSpeech();
                IShop gshop = actor.Gameworld.Shops.GetByIdOrName(shopText);
                if (gshop is not null && !command.IsFinished)
                {
                    if (gshop is not IPermanentShop ps)
                    {
                        actor.OutputHandler.Send(
                            "This command can only be used with a permanent (brick and mortar) shop.");
                        return;
                    }

                    shop = ps;
                }
                else
                {
                    actor.OutputHandler.Send("You are not currently in a shop.");
                    return;
                }
            }
            else
            {
                actor.OutputHandler.Send("You are not currently in a shop.");
                return;
            }
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which direction do you want to extend this store in?");
            return;
        }

        ICellExit exit = actor.Location.GetExitKeyword(command.PopSpeech(), actor);
        if (exit == null)
        {
            actor.OutputHandler.Send("There is no such exit.");
            return;
        }

        if (exit.Destination.Shop != null)
        {
            actor.OutputHandler.Send(
                "There is already a shop in that direction. Each location can only belong to one shop.");
            return;
        }

        if (command.IsFinished)
        {
            shop.AddShopfrontCell(exit.Destination);
            exit.Destination.Shop = shop;
            actor.OutputHandler.Send(
                $"You add {exit.Destination.HowSeen(actor)} ({exit.OutboundMovementSuffix}) to the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "store":
            case "storeroom":
            case "stock":
            case "stockroom":
                shop.StockroomCell = exit.Destination;
                exit.Destination.Shop = shop;
                actor.OutputHandler.Send(
                    $"You set {exit.Destination.HowSeen(actor)} ({exit.OutboundMovementSuffix}) as the stockroom for shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                return;
            case "work":
            case "workshop":
                shop.WorkshopCell = exit.Destination;
                exit.Destination.Shop = shop;
                actor.OutputHandler.Send(
                    $"You set {exit.Destination.HowSeen(actor)} ({exit.OutboundMovementSuffix}) as the workshop for shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                return;
        }

        actor.OutputHandler.Send(
            "You must either supply no argument after the exit to set the location as part of the shopfront, or use STOCKROOM to set it as a stockroom, or WORKSHOP to set it as a workshop.");
    }

    private static void ShopSet(ICharacter actor, StringStack command)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only proprietors can edit shop properties.");
            return;
        }

        shop.BuildingCommand(actor, command);
    }

    private static void ShopQuit(ICharacter actor, StringStack command)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsEmployee(actor))
        {
            actor.OutputHandler.Send("You are not an employee of this shop.");
            return;
        }

        actor.OutputHandler.Send($"Are you sure you wish to quit {shop.Name.TitleCase().ColourName()}, and end your employment with them?");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You quit {shop.Name.TitleCase().Colour(Telnet.Cyan)}, ending your employment with them.");
                shop.RemoveEmployee(actor);
                foreach (ICharacter employee in shop.EmployeesOnDuty)
                {
                    employee.OutputHandler.Send(
                        $"{actor.HowSeen(employee, true)} has quit {actor.ApparentGender(employee).Possessive()} employment with {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
                }
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send($"You decide not to quit {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send($"You decide not to quit {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            DescriptionString = $"quitting employment with {shop.Name.TitleCase()}",
            Keywords = new List<string> { "quit", "shop", "employment" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopCreate(ICharacter actor, StringStack command)
    {
        // TODO - prevent if a stall is here too
        if (actor.Location.Shop != null)
        {
            actor.OutputHandler.Send(
                "The location you are in is already part of a shop. You must be in a non-shop location to create a new one.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this shop?");
            return;
        }

        string name = command.PopSpeech().TitleCase();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What economic zone should this shop belong to?");
            return;
        }

        IEconomicZone zone = long.TryParse(command.PopSpeech(), out long value)
            ? actor.Gameworld.EconomicZones.Get(value)
            : actor.Gameworld.EconomicZones.GetByName(command.Last);
        if (zone == null)
        {
            actor.OutputHandler.Send("There is no such economic zone.");
            return;
        }

        PermanentShop newShop = new(zone, actor.Location, name);
        actor.Gameworld.Add(newShop);
        actor.OutputHandler.Send(
            $"You create a new shop at your current location, in the {zone.Name.TitleCase().Colour(Telnet.Cyan)} economic zone called {newShop.Name.TitleCase().Colour(Telnet.Cyan)}.");
    }

    private static void ShopCreateStall(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this shop?");
            return;
        }

        string name = command.PopSpeech().TitleCase();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What economic zone should this shop belong to?");
            return;
        }

        IEconomicZone zone = long.TryParse(command.PopSpeech(), out long value)
            ? actor.Gameworld.EconomicZones.Get(value)
            : actor.Gameworld.EconomicZones.GetByName(command.Last);
        if (zone == null)
        {
            actor.OutputHandler.Send("There is no such economic zone.");
            return;
        }

        TransientShop newShop = new(zone, name);
        actor.Gameworld.Add(newShop);
        actor.OutputHandler.Send(
            $"You create a new transient shop in the {zone.Name.TitleCase().Colour(Telnet.Cyan)} economic zone called {newShop.Name.TitleCase().Colour(Telnet.Cyan)}.");
    }

    private static void ShopEconomy(ICharacter actor, StringStack command)
    {
        IEconomicZone zone = null;
        if (!command.IsFinished)
        {
            zone = long.TryParse(command.PopSpeech(), out long value)
                ? actor.Gameworld.EconomicZones.Get(value)
                : actor.Gameworld.EconomicZones.GetByName(command.Last);
            if (zone == null)
            {
                actor.OutputHandler.Send("There is no such economic zone.");
                return;
            }
        }

        List<IShop> shops = actor.Gameworld.Shops.ToList();
        if (zone != null)
        {
            shops = shops.Where(x => x.EconomicZone == zone).ToList();
        }

        actor.OutputHandler.Send(
            $"List of shops{(zone != null ? $" for economic zone {zone.Name.TitleCase().Colour(Telnet.Cyan)}" : "")}:");
        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from shop in shops
            let transactions = shop.TransactionRecords
                                   .Where(x => x.RealDateTime >=
                                               shop.EconomicZone.CurrentFinancialPeriod.FinancialPeriodStart).ToList()
            let pshop = shop as IPermanentShop
            let cash = pshop?.TillItems.RecursiveGetItems<ICurrencyPile>(false).Where(x => x.Currency == shop.Currency)
                           .Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value)) ?? 0.0M
            let merchandise = shop.StocktakeAllMerchandise()
                                  .Sum(x => x.Key.EffectivePrice * (x.Value.InStockroomCount + x.Value.OnFloorCount))
            select new[]
            {
                shop.Id.ToString("N0", actor),
                shop.Name.TitleCase(),
                shop.Currency.Describe(cash, CurrencyDescriptionPatternType.ShortDecimal),
                shop.Currency.Describe(merchandise, CurrencyDescriptionPatternType.ShortDecimal),
                shop.Currency.Describe(
                    transactions
                        .Where(x => !x.TransactionType.In(ShopTransactionType.Deposit, ShopTransactionType.Withdrawal))
                        .Sum(x => x.NetValue), CurrencyDescriptionPatternType.ShortDecimal),
                shop.Currency.Describe(transactions.Sum(x => x.Tax), CurrencyDescriptionPatternType.ShortDecimal),
                shop.Currency.Describe(cash + merchandise + (shop.BankAccount?.CurrentBalance ?? 0.0M),
                    CurrencyDescriptionPatternType.ShortDecimal)
            },
            new[]
            {
                "ID",
                "Name",
                "Cash",
                "Inventory",
                "P/L CFP",
                "Tax CFP",
                "Value"
            },
            colour: Telnet.Green,
            maxwidth: actor.LineFormatLength,
            unicodeTable: actor.Account.UseUnicode
        ));
    }

    private static void ShopList(ICharacter actor, StringStack command)
    {
        IEconomicZone zone = null;
        if (!command.IsFinished)
        {
            zone = long.TryParse(command.PopSpeech(), out long value)
                ? actor.Gameworld.EconomicZones.Get(value)
                : actor.Gameworld.EconomicZones.GetByName(command.Last);
            if (zone == null)
            {
                actor.OutputHandler.Send("There is no such economic zone.");
                return;
            }
        }

        List<IShop> shops = actor.Gameworld.Shops.ToList();
        if (zone != null)
        {
            shops = shops.Where(x => x.EconomicZone == zone).ToList();
        }

        StringBuilder sb = new();
        sb.AppendLine($"List of shops{(zone != null ? $" for economic zone {zone.Name.TitleCase().Colour(Telnet.Cyan)}" : "")}:");
        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from shop in shops
            let pshop = shop as IPermanentShop
            select new[]
            {
                shop.Id.ToString("N0", actor),
                shop.Name.TitleCase(),
                shop.IsTrading.ToString(actor),
                shop.EmployeeRecords.Count().ToString("N0", actor),
                shop.EmployeesOnDuty.Count().ToString("N0", actor),
                pshop?.ShopfrontCells.Select(x =>
                    x.GetFriendlyReference(actor).FluentTagMXP("send",
                        $"href='goto {x.Id}'")).FirstOrDefault() ?? "",
                shop.EconomicZone.Name,
                (shop is ITransientShop).ToColouredString()
            },
            new[]
            {
                "ID",
                "Name",
                "Open?",
                "Employs",
                "Working",
                "Storefront",
                "Economic Zone",
                "Transient?"
            },
            actor,
            colour: Telnet.Green
        ));

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ShopDelete(ICharacter actor, StringStack command)
    {
        IPermanentShop shop = actor.Location.Shop;
        if (shop == null)
        {
            actor.OutputHandler.Send("You are not currently in a shop.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.SeniorAdmin))
        {
            actor.OutputHandler.Send("This action can only be performed by senior administrators.");
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure you want to delete the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}? This is irreversible.\nType {"accept".ColourCommand()} to proceed or {"decline".ColourCommand()} to change your mind.");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                actor.OutputHandler.Send($"You delete the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                shop.Delete();
                actor.Location.Shop = null;
                actor.SetEditingItem<IShop>(null);
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide against deleting the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide against deleting the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            DescriptionString = "deleting a shop",
            Keywords = new List<string> { "delete", "shop" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopInfo(ICharacter actor, StringStack command)
    {
        IShop shop;
        if (!command.IsFinished && actor.IsAdministrator())
        {
            shop = long.TryParse(command.PopSpeech(), out long value)
                ? actor.Gameworld.Shops.Get(value)
                : actor.Gameworld.Shops.GetByName(command.Last);
            if (shop == null)
            {
                actor.OutputHandler.Send("There is no such shop.");
                return;
            }
        }
        else
        {
            if (!DoShopCommandFindShop(actor, out shop))
            {
                return;
            }
        }

        if (shop == null)
        {
            actor.OutputHandler.Send("You are not in a shop.");
            return;
        }

        shop.ShowInfo(actor);
    }

    private static void ShopLedger(ICharacter actor, StringStack command)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!actor.IsAdministrator() && !shop.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not authorised to view the financial ledger of this store.");
            return;
        }

        IFinancialPeriod period = default;
        if (!command.IsFinished)
        {
            if (!int.TryParse(command.SafeRemainingArgument, out int value) || value > 0)
            {
                actor.OutputHandler.Send(
                    "You must enter a valid number equal or less than 0. Zero represents the current financial period, -1 the previous financial period and so on.");
                return;
            }

            value *= -1;
            period = shop.EconomicZone.FinancialPeriods.OrderByDescending(x => x.FinancialPeriodStart)
                         .ElementAtOrDefault(value);
            if (period == null)
            {
                actor.OutputHandler.Send("There is no such financial period for that shop.");
                return;
            }
        }
        else
        {
            period = shop.EconomicZone.CurrentFinancialPeriod;
        }

        StringBuilder sb = new();
        sb.AppendLine(
            $"Transaction record for {shop.Name.ColourName()} for financial period {period.FinancialPeriodStartMUD.Date.Display(CalendarDisplayMode.Short).ColourName()} to {period.FinancialPeriodEndMUD.Date.Display(CalendarDisplayMode.Short).ColourName()}:");
        sb.AppendLine();
        List<ITransactionRecord> records = shop.TransactionRecords.Where(x => period.InPeriod(x.RealDateTime))
                          .OrderByDescending(x => x.RealDateTime).ToList();
        sb.AppendLine(
            $"Total Net for Period: {shop.Currency.Describe(records.Sum(x => x.NetValue), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        sb.AppendLine(
            $"Total Tax Collected for Period: {shop.Currency.Describe(records.Sum(x => x.Tax), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        FinancialPeriodResult result = shop.EconomicZone.FinancialPeriodResultForShop(shop, period);
        if (result is not null)
        {
            sb.AppendLine($"Gross Revenue for Period: {shop.Currency.Describe(result.GrossRevenue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
            sb.AppendLine($"Net Revenue for Period: {shop.Currency.Describe(result.NetRevenue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
            sb.AppendLine($"Profit Tax for Period: {shop.Currency.Describe(result.ProfitsTax, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        }

        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from transaction in records
            select new List<string>
            {
                transaction.MudDateTime.Date.Display(CalendarDisplayMode.Short),
                transaction.MudDateTime.Time.Display(TimeDisplayTypes.Short),
                transaction.TransactionType.DescribeEnum(),
                transaction.Currency.Describe(transaction.PretaxValue, CurrencyDescriptionPatternType.ShortDecimal),
                transaction.Currency.Describe(transaction.Tax, CurrencyDescriptionPatternType.ShortDecimal),
                transaction.Currency.Describe(transaction.NetValue, CurrencyDescriptionPatternType.ShortDecimal),
                transaction.Merchandise?.ListDescription ?? ""
            },
            new List<string>
            {
                "Date",
                "Time",
                "Type",
                "Pretax",
                "Tax",
                "Net",
                "Merch"
            },
            actor.LineFormatLength,
            colour: Telnet.BoldYellow,
            unicodeTable: actor.Account.UseUnicode
        ));
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ShopClockIn(ICharacter actor)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!actor.Location.Shop.IsEmployee(actor))
        {
            actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        if (actor.Location.Shop.EmployeesOnDuty.Contains(actor))
        {
            actor.OutputHandler.Send("You are already clocked-in and available for duty.");
            return;
        }

        actor.Location.Shop.EmployeeClockIn(actor);
    }

    private static void ShopClockOut(ICharacter actor)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!actor.Location.Shop.IsEmployee(actor))
        {
            actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        if (!actor.Location.Shop.EmployeesOnDuty.Contains(actor))
        {
            actor.OutputHandler.Send("You are already clocked-out and off duty.");
            return;
        }

        actor.Location.Shop.EmployeeClockOut(actor);
    }

    private static void ShopEmploy(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send($"You are not a manager of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Who is it that you're proposing to employ?");
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anyone like that.");
            return;
        }

        if (shop.IsEmployee(target))
        {
            actor.OutputHandler.Send(
                $"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is already an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            return;
        }

        actor.OutputHandler.Send(
            $"You propose to employ {target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} in {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
        target.OutputHandler.Send(
            $"{actor.HowSeen(target, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is proposing to make you an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.\nUse {"accept".ColourCommand()} to sign on, or {"decline".ColourCommand()} to say no.");
        target.AddEffect(new Accept(target, new GenericProposal
        {
            AcceptAction = text =>
            {
                if (shop.IsEmployee(target))
                {
                    target.OutputHandler.Send($"You are already an employee of this store.");
                    return;
                }

                shop.AddEmployee(target);
                actor.OutputHandler.Send(
                    $"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is now an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                target.OutputHandler.Send($"You are now an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} declines to join the employ of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                target.OutputHandler.Send(
                    $"You decline to become an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} declines to join the employ of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                target.OutputHandler.Send(
                    $"You decline to become an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
            },
            Keywords = new List<string> { "shop", "employ" },
            DescriptionString = "Signing up as an employee of the shop"
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopFire(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not a manager of this shop.");
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        IEmployeeRecord record = null;
        if (target != null)
        {
            record = actor.Location.Shop.EmployeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == target.Id);
        }

        record ??= actor.Location.Shop.EmployeeRecords.FirstOrDefault(x =>
            x.Name.GetName(NameStyle.FullName).EqualTo(ss.Last));

        if (record == null)
        {
            actor.OutputHandler.Send("You don't employ anyone like that.");
            return;
        }

        if (record.IsProprietor)
        {
            actor.OutputHandler.Send("You cannot fire someone who is a proprietor.");
            return;
        }

        if (record.IsManager && !shop.IsProprietor(actor))
        {
            actor.OutputHandler.Send("You can't fire other managers, only the proprietor can.");
            return;
        }


        actor.OutputHandler.Send(
            "Are you sure that you want to fire {} from {}?\nUse {} to confirm or {} to change your mind.");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                if (!shop.IsEmployee(target))
                {
                    actor.OutputHandler.Send(
                        $"{record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} is no longer an employee, so you can't fire them.");
                    return;
                }

                shop.RemoveEmployee(record);
                actor.OutputHandler.Send(
                    $"You fire {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                target = actor.Gameworld.Actors.Get(record.EmployeeCharacterId);
                target?.OutputHandler.Send($"You have been fired from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to fire {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to fire {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            Keywords = new List<string> { },
            DescriptionString = "firing an employee from the shop"
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopDisplay(ICharacter actor, StringStack ss)
    {
        if (actor.Location.Shop == null)
        {
            actor.OutputHandler.Send("You are not currently at a permanent shop.");
            return;
        }

        if (!actor.Location.Shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not a manager of this shop.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to toggle as a display cabinet?");
            return;
        }

        IGameItem item = actor.TargetLocalItem(ss.PopSpeech());
        if (item == null)
        {
            actor.OutputHandler.Send("You don't see anything like that.");
            return;
        }

        IContainer container = item.GetItemType<IContainer>();
        if (container == null)
        {
            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not suitable for use as a display cabinet.");
            return;
        }

        if (actor.Location.Shop.TillItems.Contains(item))
        {
            actor.OutputHandler.Send("An item cannot be marked as both a display cabinet and a till at the same time.");
            return;
        }

        if (actor.Location.Shop.DisplayContainers.Contains(item))
        {
            actor.Location.Shop.RemoveDisplayContainer(item);
            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} will no longer be used as a display cabinet.");
            return;
        }

        actor.Location.Shop.AddDisplayContainer(item);
        actor.OutputHandler.Send($"{item.HowSeen(actor, true)} will now be used as a display cabinet.");
    }

    private static void ShopTill(ICharacter actor, StringStack ss)
    {
        if (actor.Location.Shop == null)
        {
            actor.OutputHandler.Send("You are not currently at a permanent shop.");
            return;
        }

        if (!actor.Location.Shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not a manager of this shop.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to toggle as a till?");
            return;
        }

        IGameItem item = actor.TargetLocalItem(ss.PopSpeech());
        if (item == null)
        {
            actor.OutputHandler.Send("You don't see anything like that.");
            return;
        }

        // TODO - actual cash register type tills
        IContainer container = item.GetItemType<IContainer>();
        if (container == null)
        {
            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not suitable for use as a till.");
            return;
        }

        if (actor.Location.Shop.DisplayContainers.Contains(item))
        {
            actor.OutputHandler.Send("An item cannot be marked as both a display cabinet and a till at the same time.");
            return;
        }

        if (actor.Location.Shop.TillItems.Contains(item))
        {
            actor.Location.Shop.RemoveTillItem(item);
            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is no longer being used as a till for the shop.");
            return;
        }

        actor.Location.Shop.AddTillItem(item);
        actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is now being used as a till for the shop.");
    }

    private static void ShopManager(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }


        if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not the proprietor of this shop.");
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        IEmployeeRecord record = null;
        if (target != null)
        {
            record = shop.EmployeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == target.Id);
        }

        record ??= shop.EmployeeRecords.FirstOrDefault(x =>
            x.Name.GetName(NameStyle.FullName).EqualTo(ss.Last));

        if (record == null)
        {
            actor.OutputHandler.Send("You don't employ anyone like that.");
            return;
        }

        if (record.IsProprietor)
        {
            actor.OutputHandler.Send("You cannot remove the manager status of someone who is a proprietor.");
            return;
        }

        record.IsManager = !record.IsManager;
        shop.Changed = true;
        actor.OutputHandler.Send(
            $"You {(record.IsManager ? "promote" : "demote")} {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} {(record.IsManager ? "to the position of manager" : "to merely an employee")}.");
        target = actor.Gameworld.Actors.Get(record.EmployeeCharacterId);
        target?.OutputHandler.Send(
                $"You have been {(record.IsManager ? "promoted to the position of manager of this shop" : "demoted to merely an employee of this shop")}.");
    }

    private static void ShopProprietor(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not the proprietor of this shop.");
            return;
        }

        ICharacter target = actor.TargetActor(ss.PopSpeech());
        IEmployeeRecord record = null;
        if (target != null)
        {
            record = shop.EmployeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == target.Id);
        }

        record ??= shop.EmployeeRecords.FirstOrDefault(x => x.Name.GetName(NameStyle.FullName).EqualTo(ss.Last));

        if (record == null)
        {
            actor.OutputHandler.Send("You don't employ anyone like that.");
            return;
        }

        if (record.IsProprietor)
        {
            actor.OutputHandler.Send(
                $"{record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} is already a proprietor.");
            return;
        }

        actor.OutputHandler.Send(
            $"You are proposing to promote {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to the position of proprietor of {shop.Name.TitleCase().Colour(Telnet.Cyan)}. This decision is irreversable, and they will be a full owner with all rights unless they subsequently quit.\nTo go through with this decision, type {"accept".ColourCommand()} or type {"decline".ColourCommand()} to change your mind.");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                record.IsManager = true;
                record.IsProprietor = true;
                shop.Changed = true;
                actor.OutputHandler.Send(
                    $"You promote {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to proprietor of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
                target = actor.Gameworld.Actors.Get(record.EmployeeCharacterId);
                target?.OutputHandler.Send(
                        $"You have been promoted to the proprietor of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide against promoting {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to proprietor.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide against promoting {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to proprietor.");
            },
            Keywords = new List<string> { "proprietor", "shop", "promote" },
            DescriptionString = "Promote someone to proprietor of a shop"
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopStock(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsEmployee(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not an employee of this shop.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to put into stock for sale?");
            return;
        }

        IGameItem item = actor.TargetLocalOrHeldItem(ss.PopSpeech());
        if (item == null)
        {
            actor.OutputHandler.Send("You don't see anything like that.");
            return;
        }

        IMerchandise merch = null;
        if (!ss.IsFinished)
        {
            merch = shop.Merchandises
                        .Where(x => x.IsMerchandiseFor(item, true))
                        .GetFromItemListByKeyword(ss.PopSpeech(), actor);
            if (merch == null)
            {
                actor.OutputHandler.Send(
                    $"There is no matching merchandise profile for {item.HowSeen(actor)} with the specified keywords. A manager or proprietor must first create a merchandise profile before it can be brought into stock.");
                return;
            }
        }
        else
        {
            merch = shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item)) ??
                    shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item, true));
            if (merch == null)
            {
                actor.OutputHandler.Send(
                    $"There is no merchandise profile for items like {item.HowSeen(actor)}. A manager or proprietor must first create a merchandise profile before it can be brought into stock.");
                return;
            }
        }

        if (item.AffectedBy<ItemOnDisplayInShop>())
        {
            actor.OutputHandler.Send(
                $"{item.HowSeen(actor, true)} is already in stock. Use the dispose command if you want to take it out of inventory.");
            return;
        }

        shop.AddToStock(actor, item, merch);
    }

    private static void ShopDispose(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsEmployee(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("You are not an employee of this shop.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "Which item do you want to put dispose of from the for-sale inventory of the shop?");
            return;
        }

        IGameItem item = actor.TargetLocalOrHeldItem(ss.PopSpeech());
        if (item == null)
        {
            actor.OutputHandler.Send("You don't see anything like that.");
            return;
        }

        if (!item.AffectedBy<ItemOnDisplayInShop>())
        {
            actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not in stock.");
            return;
        }

        IMerchandise merch =
            shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item)) ??
            shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item, true));
        if (merch == null)
        {
            actor.OutputHandler.Send(
                $"There is no merchandise profile for items like {item.HowSeen(actor)}. A manager or proprietor must first create a merchandise profile before it can be disposed of.");
            return;
        }

        shop.DisposeFromStock(actor, item);
    }

    private static void ShopMerchandise(ICharacter actor, StringStack ss)
    {
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "list":
                ShopMerchandiseList(actor, ss);
                return;
            case "show":
                ShopMerchandiseShow(actor, ss);
                return;
            case "edit":
                ShopMerchandiseEdit(actor, ss);
                return;
            case "new":
                ShopMerchandiseNew(actor, ss);
                return;
            case "clone":
                ShopMerchandiseClone(actor, ss);
                return;
            case "delete":
                ShopMerchandiseDelete(actor, ss);
                return;
            case "set":
                ShopMerchandiseSet(actor, ss);
                return;
            case "close":
                ShopMerchandiseClose(actor);
                return;
            default:
                ShopMerchandiseHelp(actor);
                return;
        }
    }

    private static void ShopMerchandiseClose(ICharacter actor)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }
        BuilderEditingEffect<IMerchandise> editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
                           .FirstOrDefault(x => x.EditingItem.Shop == shop);
        if (editing is null)
        {
            actor.OutputHandler.Send("You are not editing any merchandise for your current shop.");
            return;
        }

        actor.RemoveEffect(editing);
        actor.OutputHandler.Send($"You are no longer editing any merchandise records for your current shop.");
    }

    private const string ShopMerchandiseHelpText = @"You can use the following subcommands for working with merchandise:
	#1Note: All SHOP MERCHANDISE commands must be done from a room within the shop.#0

	#3shop merch list#0 - lists all merchandise records
	#3shop merch edit <record>#0 - opens a merchandise record for editing
	#3shop merch edit#0 - equivalent to SHOW <edited record>
	#3shop merch show <record>#0 - shows information about the specified merchandise record
	#3shop merch new <name> <id>|<target> <price>|default [<custom description>]#0 - creates a new record with the specified item and price, and optional custom LIST description
	#3shop merch clone <new name>#0 - clones the currently edited record to an identical new record
	#3shop merch delete#0 - deletes the current merchandise record
	#3shop merch close#0 - closes the merchandise record you're editing
	#3shop merch set name <name>#0 - sets the name of a merchandise
	#3shop merch set proto <target>#0 - sets the item type associated with a merchandise
	#3shop merch set default#0 - toggles whether this is the default merch record for similar items
	#3shop merch set price <price>#0 - sets the pre-tax price
	#3shop merch set desc clear#0 - clears a custom list description
	#3shop merch set desc <description>#0 - sets a custom list description
	#3shop merch set container clear#0 - clears a preferred display container
	#3shop merch set container <target>#0 - sets a preferred display container";

    private const string ShopMerchandiseAdminHelpText =
        @"You can use the following subcommands for working with merchandise:
	#1Note: All SHOP MERCHANDISE commands must be done from a room within the shop.#0

	#3shop merch list#0 - lists all merchandise records
	#3shop merch edit <record>#0 - opens a merchandise record for editing
	#3shop merch edit#0 - equivalent to SHOW <edited record>
	#3shop merch show <record>#0 - shows information about the specified merchandise record
	#3shop merch new <name> <id>|<target> <price>|default [<custom description>]#0 - creates a new record with the specified item and price, and optional custom LIST description
	#3shop merch clone <new name>#0 - clones the currently edited record to an identical new record
	#3shop merch delete#0 - deletes the current merchandise record
	#3shop merch close#0 - closes the merchandise record you're editing
	#3shop merch set name <name>#0 - sets the name of a merchandise
	#3shop merch set proto <target>#0 - sets the item type associated with a merchandise
	#3shop merch set default#0 - toggles whether this is the default merch record for similar items
	#3shop merch set price <price>#0 - sets the pre-tax price
	#3shop merch set desc clear#0 - clears a custom list description
	#3shop merch set desc <description>#0 - sets a custom list description
	#3shop merch set container clear#0 - clears a preferred display container
	#3shop merch set container <target>#0 - sets a preferred display container
	#3shop merch set reorder off#0 - turns auto-reordering off
	#3shop merch set reorder <price> <quantity> [<weight>]#0 - enables auto-reordering with the specified price, quantity and optional minimum weight
	#3shop merch set preserve#0 - toggles preservation of item variables when reordering";

    private static void ShopMerchandiseHelp(ICharacter actor)
    {
        actor.OutputHandler.Send(
            actor.IsAdministrator()
                ? ShopMerchandiseAdminHelpText.SubstituteANSIColour()
                : ShopMerchandiseHelpText.SubstituteANSIColour()
        );
    }

    private static void ShopMerchandiseSet(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }
        BuilderEditingEffect<IMerchandise> editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
                           .FirstOrDefault(x => x.EditingItem.Shop == shop);
        if (editing == null)
        {
            actor.OutputHandler.Send("You are not currently editing any merchandise entries for your current shop.");
            return;
        }

        editing.EditingItem.BuildingCommand(actor, ss);
    }

    private static void ShopMerchandiseDelete(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }
        BuilderEditingEffect<IMerchandise> editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
                           .FirstOrDefault(x => x.EditingItem.Shop == shop);
        if (editing == null)
        {
            actor.OutputHandler.Send("You are not currently editing any merchandise entries for your current shop.");
            return;
        }

        IMerchandise record = editing.EditingItem;
        actor.OutputHandler.Send(
            $"Are you sure that you want to delete the merchandise record '{editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}'?\nUse {"accept".ColourCommand()} to proceed or {"decline".ColourCommand()} to change your mind.");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You delete merchandise record {editing.EditingItem.ListDescription.ColourObject()} ({editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}).");
                record.Shop.RemoveMerchandise(record);
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide against deleting merchandise record {editing.EditingItem.ListDescription.ColourObject()} ({editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}).");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide against deleting merchandise record {editing.EditingItem.ListDescription.ColourObject()} ({editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}).");
            },
            DescriptionString = "Deleting a merchandise record",
            Keywords = new List<string> { "merchandise", "delete" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopMerchandiseClone(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }
        BuilderEditingEffect<IMerchandise> editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
                           .FirstOrDefault(x => x.EditingItem.Shop == shop);
        if (editing == null)
        {
            actor.OutputHandler.Send("You are not currently editing any merchandise entries.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("You must supply a name for your cloned merchandise record.");
            return;
        }

        string name = ss.PopSpeech();

        Merchandise newMerch = new((Merchandise)editing.EditingItem, name);
        editing.EditingItem.Shop.AddMerchandise(newMerch);
        actor.OutputHandler.Send(
            $"You clone merchandise record for {editing.EditingItem.ListDescription.ColourObject()} into a new record called {name.TitleCase().Colour(Telnet.Cyan)}, which you are now editing.");
        actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IMerchandise>>());
        actor.AddEffect(new BuilderEditingEffect<IMerchandise>(actor) { EditingItem = newMerch });
    }

    private static void ShopMerchandiseNew(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }


        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only managers or proprietors of the shop can create new merchandise records.");
            return;
        }

        void HelpText()
        {
            if (actor.IsAdministrator())
            {
                actor.OutputHandler.Send(
                    "The syntax for this command is SHOP MERCHANDISE NEW <name> <id>|<target> <price>|default [<custom description>]");
                return;
            }

            actor.OutputHandler.Send(
                "The syntax for this command is SHOP MERCHANDISE NEW <name> <target> <price>|default [<custom description>]");
        }

        if (ss.IsFinished)
        {
            HelpText();
            return;
        }

        string name = ss.PopSpeech();

        if (ss.IsFinished)
        {
            HelpText();
            return;
        }

        string text = ss.PopSpeech();
        IGameItemProto proto;
        if (actor.IsAdministrator() && long.TryParse(text, out long value))
        {
            proto = actor.Gameworld.ItemProtos.Get(value);
            if (proto == null)
            {
                actor.OutputHandler.Send("There is no such item prototype.");
                return;
            }
        }
        else
        {
            IGameItem target = actor.TargetLocalOrHeldItem(text);
            if (target == null)
            {
                actor.OutputHandler.Send("You don't see anything like that.");
                return;
            }

            proto = target.Prototype;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What pre-tax price do you want to give to this merchandise entry?");
            return;
        }

        decimal price;
        if (ss.PeekSpeech().EqualTo("default"))
        {
            price = -1.0M;
            ss.PopSpeech();
        }
        else
        {
            price = shop.Currency.GetBaseCurrency(ss.PopSpeech(), out bool success);
            if (!success)
            {
                actor.OutputHandler.Send("That is not a valid price.");
                return;
            }
        }

        Merchandise newMerch = new(shop, name, proto, price, shop.Merchandises.All(x => x.Item.Id != proto.Id),
            null, ss.SafeRemainingArgument);
        shop.AddMerchandise(newMerch);
        actor.OutputHandler.Send(
            $"You create a new merchandise entry for {newMerch.ListDescription.ColourObject()} ({newMerch.Name.TitleCase().Colour(Telnet.Cyan)}), which you are now editing.");
        actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IMerchandise>>());
        actor.AddEffect(new BuilderEditingEffect<IMerchandise>(actor) { EditingItem = newMerch });
        return;
    }

    private static void ShopMerchandiseEdit(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only managers or proprietors of the shop can edit merchandise records.");
            return;
        }

        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMerchandise> editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
                               .FirstOrDefault();
            if (editing == null)
            {
                actor.OutputHandler.Send("Which merchandise record would you like to edit?");
                return;
            }

            editing.EditingItem.ShowToBuilder(actor);
            return;
        }

        string text = ss.PopSpeech();
        IMerchandise merch =
            shop.Merchandises.GetById(text) ??
            shop.Merchandises.GetFromItemListByKeywordIncludingNames(text, actor);
        if (merch == null)
        {
            actor.OutputHandler.Send("There is no merchandise record like that for you to edit.");
            return;
        }

        actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IMerchandise>>());
        actor.AddEffect(new BuilderEditingEffect<IMerchandise>(actor) { EditingItem = merch });
        actor.OutputHandler.Send(
            $"You are now editing the merchandise record {merch.Name.TitleCase().Colour(Telnet.Cyan)}.");
    }

    private static void ShopMerchandiseShow(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only managers or proprietors of the shop can view merchandise records.");
            return;
        }

        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMerchandise> editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
                               .FirstOrDefault(x => x.EditingItem.Shop == shop);
            if (editing == null)
            {
                actor.OutputHandler.Send("Which merchandise record would you like to view?");
                return;
            }

            editing.EditingItem.ShowToBuilder(actor);
            return;
        }

        string text = ss.PopSpeech();
        IMerchandise merch =
            shop.Merchandises.GetById(text) ??
            shop.Merchandises.GetFromItemListByKeywordIncludingNames(text, actor);
        if (merch == null)
        {
            actor.OutputHandler.Send("There is no merchandise record like that for you to view.");
            return;
        }

        merch.ShowToBuilder(actor);
    }

    private static void ShopMerchandiseList(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!shop.IsManager(actor) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only managers or proprietors of the shop can view merchandise records.");
            return;
        }

        Dictionary<IMerchandise, (int OnFloorCount, int InStockroomCount)> stockTake = shop.StocktakeAllMerchandise();
        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from merch in shop.Merchandises
            orderby merch.Id
            select new[]
            {
                merch.Id.ToString("N0", actor),
                merch.Name,
                merch.ListDescription,
                shop.Currency.Describe(merch.EffectivePrice, CurrencyDescriptionPatternType.Short),
                stockTake[merch].OnFloorCount.ToString("N0", actor),
                stockTake[merch].InStockroomCount.ToString("N0", actor),
                merch.DefaultMerchandiseForItem.ToColouredString()
            },
            new[] { "Id", "Name", "List Description", "Price", "On Display", "In Store", "Default?" },
            actor.LineFormatLength,
            truncatableColumnIndex: 1,
            colour: Telnet.Green,
            unicodeTable: actor.Account.UseUnicode
        ));
    }

    private static bool CanManageShopDeals(ICharacter actor, IShop shop)
    {
        return actor.IsAdministrator() || shop.IsManager(actor) || shop.IsProprietor(actor);
    }

    private static void ShopDeals(ICharacter actor, StringStack ss)
    {
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "list":
                ShopDealsList(actor, ss);
                return;
            case "show":
                ShopDealsShow(actor, ss);
                return;
            case "edit":
                ShopDealsEdit(actor, ss);
                return;
            case "new":
                ShopDealsNew(actor, ss);
                return;
            case "delete":
                ShopDealsDelete(actor, ss);
                return;
            case "set":
                ShopDealsSet(actor, ss);
                return;
            case "close":
                ShopDealsClose(actor);
                return;
            default:
                ShopDealsHelp(actor);
                return;
        }
    }

    private static void ShopDealsClose(ICharacter actor)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        BuilderEditingEffect<IShopDeal> editing = actor.EffectsOfType<BuilderEditingEffect<IShopDeal>>()
            .FirstOrDefault(x => x.EditingItem.Shop == shop);
        if (editing is null)
        {
            actor.OutputHandler.Send("You are not editing any shop deals for your current shop.");
            return;
        }

        actor.RemoveEffect(editing);
        actor.OutputHandler.Send("You are no longer editing any shop deals for your current shop.");
    }

    private const string ShopDealsHelpText = @"You can use the following subcommands for working with shop deals:
	#1Note: All SHOP DEALS commands must be done from a room within the shop.#0

	#3shop deals list#0 - lists all shop deals
	#3shop deals show <deal>#0 - shows information about the specified deal
	#3shop deals edit <deal>#0 - opens a deal for editing
	#3shop deals edit#0 - shows the currently edited deal
	#3shop deals new <name>#0 - creates a new deal
	#3shop deals delete#0 - deletes the currently edited deal
	#3shop deals close#0 - closes the deal you are editing
	#3shop deals set name <name>#0 - renames a deal
	#3shop deals set type sale|volume <quantity>#0 - sets the deal type
	#3shop deals set target all|merchandise <record>|tag <tag>#0 - sets the target
	#3shop deals set adjustment <signed %>#0 - sets the signed price change
	#3shop deals set applies sell|buy|both#0 - sets whether the deal affects buying, selling or both
	#3shop deals set prog clear|<prog>#0 - sets the shopper eligibility prog
	#3shop deals set cumulative#0 - toggles whether the deal stacks
	#3shop deals set expires never|<datetime>#0 - sets an expiry
	#3shop deals set expiresin <timespan>#0 - sets a relative expiry";

    private static void ShopDealsHelp(ICharacter actor)
    {
        actor.OutputHandler.Send(ShopDealsHelpText.SubstituteANSIColour());
    }

    private static void ShopDealsSet(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!CanManageShopDeals(actor, shop))
        {
            actor.OutputHandler.Send("Only managers, proprietors or administrators can edit shop deals.");
            return;
        }

        BuilderEditingEffect<IShopDeal> editing = actor.EffectsOfType<BuilderEditingEffect<IShopDeal>>()
            .FirstOrDefault(x => x.EditingItem.Shop == shop);
        if (editing is null)
        {
            actor.OutputHandler.Send("You are not currently editing any shop deals for your current shop.");
            return;
        }

        editing.EditingItem.BuildingCommand(actor, ss);
    }

    private static void ShopDealsDelete(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!CanManageShopDeals(actor, shop))
        {
            actor.OutputHandler.Send("Only managers, proprietors or administrators can delete shop deals.");
            return;
        }

        BuilderEditingEffect<IShopDeal> editing = actor.EffectsOfType<BuilderEditingEffect<IShopDeal>>()
            .FirstOrDefault(x => x.EditingItem.Shop == shop);
        if (editing is null)
        {
            actor.OutputHandler.Send("You are not currently editing any shop deals for your current shop.");
            return;
        }

        IShopDeal record = editing.EditingItem;
        actor.OutputHandler.Send(
            $"Are you sure that you want to delete the shop deal {record.Name.ColourName()}?\nUse {"accept".ColourCommand()} to proceed or {"decline".ColourCommand()} to change your mind.");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                actor.OutputHandler.Send($"You delete the shop deal {record.Name.ColourName()}.");
                record.Shop.RemoveDeal(record);
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send($"You decide against deleting the shop deal {record.Name.ColourName()}.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send($"You decide against deleting the shop deal {record.Name.ColourName()}.");
            },
            DescriptionString = "Deleting a shop deal",
            Keywords = new List<string> { "deal", "delete" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void ShopDealsNew(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!CanManageShopDeals(actor, shop))
        {
            actor.OutputHandler.Send("Only managers, proprietors or administrators can create shop deals.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this new shop deal?");
            return;
        }

        ShopDeal deal = new(shop, ss.SafeRemainingArgument);
        shop.AddDeal(deal);
        actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IShopDeal>>());
        actor.AddEffect(new BuilderEditingEffect<IShopDeal>(actor) { EditingItem = deal });
        actor.OutputHandler.Send($"You create a new shop deal called {deal.Name.ColourName()}, which you are now editing.");
    }

    private static void ShopDealsEdit(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!CanManageShopDeals(actor, shop))
        {
            actor.OutputHandler.Send("Only managers, proprietors or administrators can edit shop deals.");
            return;
        }

        if (ss.IsFinished)
        {
            BuilderEditingEffect<IShopDeal> editing = actor.EffectsOfType<BuilderEditingEffect<IShopDeal>>()
                .FirstOrDefault(x => x.EditingItem.Shop == shop);
            if (editing is null)
            {
                actor.OutputHandler.Send("Which shop deal would you like to edit?");
                return;
            }

            editing.EditingItem.ShowToBuilder(actor);
            return;
        }

        string text = ss.PopSpeech();
        IShopDeal deal = shop.Deals.GetById(text) ??
                   shop.Deals.FirstOrDefault(x => x.Name.EqualTo(text));
        if (deal is null)
        {
            actor.OutputHandler.Send("There is no shop deal like that for you to edit.");
            return;
        }

        actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IShopDeal>>());
        actor.AddEffect(new BuilderEditingEffect<IShopDeal>(actor) { EditingItem = deal });
        actor.OutputHandler.Send($"You are now editing the shop deal {deal.Name.ColourName()}.");
    }

    private static void ShopDealsShow(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!CanManageShopDeals(actor, shop))
        {
            actor.OutputHandler.Send("Only managers, proprietors or administrators can view shop deals.");
            return;
        }

        if (ss.IsFinished)
        {
            BuilderEditingEffect<IShopDeal> editing = actor.EffectsOfType<BuilderEditingEffect<IShopDeal>>()
                .FirstOrDefault(x => x.EditingItem.Shop == shop);
            if (editing is null)
            {
                actor.OutputHandler.Send("Which shop deal would you like to view?");
                return;
            }

            editing.EditingItem.ShowToBuilder(actor);
            return;
        }

        string text = ss.PopSpeech();
        IShopDeal deal = shop.Deals.GetById(text) ??
                   shop.Deals.FirstOrDefault(x => x.Name.EqualTo(text));
        if (deal is null)
        {
            actor.OutputHandler.Send("There is no shop deal like that for you to view.");
            return;
        }

        deal.ShowToBuilder(actor);
    }

    private static void ShopDealsList(ICharacter actor, StringStack ss)
    {
        if (!DoShopCommandFindShop(actor, out IShop shop))
        {
            return;
        }

        if (!CanManageShopDeals(actor, shop))
        {
            actor.OutputHandler.Send("Only managers, proprietors or administrators can view shop deals.");
            return;
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from deal in shop.Deals
            orderby deal.Id
            select new[]
            {
                deal.Id.ToString("N0", actor),
                deal.Name,
                deal is ShopDeal shopDeal ? shopDeal.DescribeType(actor) : deal.DealType.DescribeEnum().ColourName(),
                deal is ShopDeal targetDeal ? targetDeal.DescribeTarget(actor) : deal.TargetType.DescribeEnum().ColourName(),
                ShopDeal.DescribePercentage(deal.PriceAdjustmentPercentage, actor),
                deal.Applicability.DescribeEnum().ColourName(),
                deal.IsCumulative.ToColouredString(),
                deal.Expiry.Date is null
                    ? "Never".ColourValue()
                    : deal.Expiry.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()
            },
            new[] { "Id", "Name", "Type", "Target", "Adjustment", "Applies", "Stacks", "Expiry" },
            actor.LineFormatLength,
            truncatableColumnIndex: 1,
            colour: Telnet.Green,
            unicodeTable: actor.Account.UseUnicode
        ));
    }

    #endregion

    #region Banks

    public const string BankHelpText =
        @"The bank command is used to interact with bank accounts. All of the commands need to be done at a bank branch.

The syntax for using banks is as follows:

	#3bank accounts#0 - shows all the bank accounts you have access to
	#3bank open <type>#0 - opens a new bank account
	#3bank openclan <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank openshop <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank types#0 - shows what types of bank accounts this bank offers
	#3bank alias <account#>#0 - sets the alias of a bank account
	#3bank preview <type>#0 - previews the fees/interest of a bank account type
	#3bank close <account#>#0 - permanently closes a bank account
	#3bank show <account#>#0 - shows information about an account
	#3bank transactions <account#>#0 - shows transaction history for a bank account
	#3bank deposit <account#> <amount>#0 - deposits money into an account
	#3bank withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank transfer <fromaccount#>#0 <toaccount#> <amount>#0 - transfers money to another account
	#3bank transfer <fromaccount#>#0 <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank
	#3bank requestitem <account#>#0 - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank cancelitems <account#>#0 - requests that the bank cancel all issued items, e.g. if lost or stolen

Additionally, if you are the manager of a bank, you can use the following additional commands:

	#3bank manager balance#0 - shows information about the banks funds and liabilities
	#3bank manager audit [<who>]#0 - shows audit logs (optionally for a specific person)
	#3bank manager status <account> <active|suspended|locked>#0 - changes the status of a bank account
	#3bank manager close <account>#0 - permanently closes an account (can get around restrictions)
	#3bank manager credit <account> <amount> <comment>#0 - credits an existing account
	#3bank manager accounts#0 - view a list of accounts
	#3bank manager account <accn>#0 - view info about an account
	#3bank manager rollover <account> <newaccount>#0 - closes an account and rolls balance into a new one
	#3bank manager withdraw <amount>#0 - withdraws money from the cash reserves
	#3bank manager deposit <amount>#0 - deposits money into the cash reserves
	#3bank manager exchange <from> <to> <rate>#0 - sets the currency exchange rate";

    public const string BankAdminHelpText =
        @"The bank command is used to create and edit banks. The commands are as follows:

	#3bank list#0 - lists all of the banks
	#3bank show <which>#0 - shows information about a bank
	#3bank edit new <name> <code> <economiczone>#0 - creates a new bank
	#3bank clone <which> <name> <code>#0 - clones a bank into a new bank
	#3bank edit <which>#0 - begins to edit a bank
	#3bank edit#0 - alias for BANK SHOW <currently editing bank>
	#3bank close#0 - stops editing a bank
	#3bank set ...#0 - edits the properties of a bank. See bank set ? for more info.

The player version of the command is explained below. All of the commands need to be done at a bank branch.
Note: There are two different ways to access the player commands, which is necessary to avoid clashes with the admin versions. You may choose either version

Syntax Option 1:

	#3bank accounts#0 - shows all the bank accounts you have access to
	#3bank open <type>#0 - opens a new bank account
	#3bank openclan <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank openshop <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank types#0 - shows what types of bank accounts this bank offers
	#3bank alias <account#>#0 - sets the alias of a bank account
	#3bank closeaccount <account#>#0 - permanently closes a bank account
	#3bank showaccount <account#>#0 - shows information about an account
	#3bank transactions <account#>#0 - shows transaction history for a bank account
	#3bank deposit <account#> <amount>#0 - deposits money into an account
	#3bank withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank transfer <fromaccount#> <toaccount#> <amount>#0 - transfers money to another account
	#3bank transfer <fromaccount#> <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank
	#3bank requestitem <account#>#0 - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank cancelitems <account#>#0 - requests that the bank cancel all issued items, e.g. if lost or stolen

Syntax Option 2:

	#3bank accounts#0 - shows all the bank accounts you have access to
	#3bank account open <type>#0 - opens a new bank account
	#3bank account clanopen <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank account shopopen <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank account types#0 - shows what types of bank accounts this bank offers
	#3bank account alias <account#>#0 - sets the alias of a bank account
	#3bank account close <account#>#0 - permanently closes a bank account
	#3bank account show <account#>#0 - shows information about an account
	#3bank account transactions <account#>#0 - shows transaction history for a bank account
	#3bank account deposit <account#> <amount>#0 - deposits money into an account
	#3bank account withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank account transfer <fromaccount#> <toaccount#> <amount>#0 - transfers money to another account
	#3bank account transfer <fromaccount#> <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank	
	#3bank account requestitem <account#>#0 - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank account cancelitems <account#>#0 - requests that the bank cancel all issued items, e.g. if lost or stolen

Additionally, if you are the manager of a bank, you can use the following additional commands:

	#3bank manager balance#0 - shows information about the banks funds and liabilities
	#3bank manager audit [<who>]#0 - shows audit logs (optionally for a specific person)
	#3bank manager status <account> <active|suspended|locked>#0 - changes the status of a bank account
	#3bank manager close <account>#0 - permanently closes an account (can get around restrictions)
	#3bank manager accounts#0 - view a list of accounts
	#3bank manager account <accn>#0 - view info about an account
	#3bank manager credit <account> <amount> <comment>#0 - credits an existing account
	#3bank manager rollover <account> <newaccount>#0 - closes an account and rolls balance into a new one
	#3bank manager withdraw <amount>#0 - withdraws money from the cash reserves
	#3bank manager deposit <amount>#0 - deposits money into the cash reserves
	#3bank manager exchange <from> <to> <rate>#0 - sets the currency exchange rate";

    [PlayerCommand("Bank", "bank")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoHideCommand]
    [NoCombatCommand]
    [HelpInfo("bank", BankHelpText, AutoHelp.HelpArgOrNoArg, BankAdminHelpText)]
    protected static void Bank(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "edit":
            case "set":
            case "view":
            case "clone":
            case "list":
                BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.BankHelper);
                return;
            case "close":
                if (!actor.IsAdministrator())
                {
                    goto case "closeaccount";
                }

                BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.BankHelper);
                return;
            case "show":
                if (!actor.IsAdministrator())
                {
                    goto case "showaccount";
                }

                BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.BankHelper);
                return;
            case "account":
                BuildingCommandBankAccount(actor, ss);
                return;
            case "accounts":
                BuildingCommandBankAccounts(actor, ss);
                return;
            case "manager":
                BankManager(actor, ss);
                return;
            case "deposit":
                BuildingCommandBankAccount(actor,
                    new StringStack("deposit" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "withdraw":
                BuildingCommandBankAccount(actor,
                    new StringStack("withdraw" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "transactions":
            case "history":
                BuildingCommandBankAccount(actor,
                    new StringStack("transactions" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "transfer":
            case "trans":
            case "xfer":
                BuildingCommandBankAccount(actor,
                    new StringStack("transfer" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "alias":
                BuildingCommandBankAccount(actor,
                    new StringStack("alias" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "closeaccount":
                BuildingCommandBankAccount(actor,
                    new StringStack("close" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "showaccount":
                BuildingCommandBankAccount(actor,
                    new StringStack("show" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "types":
                BuildingCommandBankAccount(actor,
                    new StringStack("types" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "preview":
                BuildingCommandBankAccount(actor,
                    new StringStack("preview" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "open":
            case "openaccount":
                BuildingCommandBankAccount(actor,
                    new StringStack("open" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "openshop":
            case "openshopaccount":
                BuildingCommandBankAccount(actor,
                    new StringStack("shopopen" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "openclan":
            case "openclanaccount":
                BuildingCommandBankAccount(actor,
                    new StringStack("clanopen" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "requestitem":
                BuildingCommandBankAccount(actor,
                    new StringStack("requestitem" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            case "cancelitems":
                BuildingCommandBankAccount(actor,
                    new StringStack("cancelitems" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
                return;
            default:
                actor.OutputHandler.Send((actor.IsAdministrator() ? BankAdminHelpText : BankHelpText)
                    .SubstituteANSIColour());
                return;
        }
    }

    private static void BankManager(ICharacter actor, StringStack ss)
    {
        IBank bank = actor.Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(actor.Location));
        if (bank == null)
        {
            actor.OutputHandler.Send("You are not currently at a bank.");
            return;
        }

        if (!bank.IsManager(actor))
        {
            actor.OutputHandler.Send($"You are not a manager of {bank.Name.ColourName()}.");
            return;
        }

        bank.ManagerCommand(actor, ss);
    }

    private static void BuildingCommandBankAccounts(ICharacter actor, StringStack ss)
    {
        IBank bank = actor.Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(actor.Location));
        if (bank == null)
        {
            actor.OutputHandler.Send("You are not currently at a bank.");
            return;
        }

        List<IBankAccount> accounts = bank.BankAccounts.Where(x => x.IsAuthorisedAccountUser(actor)).ToList();
        if (!accounts.Any())
        {
            actor.OutputHandler.Send($"You don't have any bank accounts with {bank.Name.ColourName()}.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"You have access to the following bank accounts with {bank.Name.ColourName()}:");
        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from account in accounts
            select new List<string>
            {
                $"{account.Bank.Code}:{account.AccountNumber.ToString("F0",actor)}",
                account.BankAccountType.Name,
                account.Name,
                account.AccountStatus.DescribeEnum(),
                bank.EconomicZone.Currency.Describe(account.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
                account.IsAccountOwner(actor) ? "You" : account.AccountOwner switch
                {
                    ICharacter ch => ch.PersonalName.GetName(NameStyle.FullName),
                    IClan clan => clan.FullName,
                    IShop shop => shop.Name,
                    _ => "Unknown"
                }
            },
            new List<string>
            {
                "Account",
                "Account Type",
                "Alias",
                "Status",
                "Balance",
                "Owner"
            },
            actor,
            Telnet.Yellow
        ));

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void BuildingCommandBankAccount(ICharacter actor, StringStack ss)
    {
        IBank bank = actor.Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(actor.Location));
        if (bank == null)
        {
            actor.OutputHandler.Send("You are not currently at a bank.");
            return;
        }

        string text = ss.PopForSwitch();
        if (text.EqualToAny("open", "new", "create"))
        {
            BankAccountCreate(actor, ss, bank);
            return;
        }

        if (text.EqualTo("clanopen"))
        {
            BankAccountCreateClan(actor, ss, bank);
            return;
        }

        if (text.EqualTo("shopopen"))
        {
            BankAccountCreateShop(actor, ss, bank);
            return;
        }

        if (text.EqualTo("types"))
        {
            BankAccountTypes(actor, bank);
            return;
        }

        if (text.EqualTo("preview"))
        {
            BankAccountPreview(actor, ss, bank);
            return;
        }

        switch (text)
        {
            case "deposit":
            case "withdraw":
            case "transfer":
            case "close":
            case "show":
            case "transactions":
            case "requestitem":
            case "cancelitems":
            case "alias":
                break;
            default:
                actor.OutputHandler.Send(@"The valid options for this sub-command are as follows:

	#3bank account open <type>#0 - opens a new bank account
	#3bank account clanopen <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank account shopopen <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank account types#0 - shows what types of bank accounts this bank offers
	#3bank account alias <account#>#0 - sets the alias of a bank account
	#3bank account close <account#>#0 - permanently closes a bank account
	#3bank account show <account#>#0 - shows information about an account
	#3bank account transactions <account#>#0 - shows transaction history for an account
	#3bank account deposit <account#> <amount>#0 - deposits money into an account
	#3bank account preview <type>#0 - previews an account type
	#3bank account withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank account transfer <fromaccount#> <toaccount#> <amount>#0 - transfers money to another account
	#3bank account transfer <fromaccount#> <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank
	#3bank account requestitem <account#> - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank account cancelitems <account#> - requests that the bank cancel all issued items, e.g. if lost or stolen"
                    .SubstituteANSIColour());
                return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("You must supply an account number for that action.");
            return;
        }

        _ = int.TryParse(ss.PopSpeech(), out int accn);

        IBankAccount account =
            bank.BankAccounts.FirstOrDefault(x => ((accn > 0 && x.AccountNumber == accn) || x.Name.StartsWith(ss.Last, StringComparison.InvariantCultureIgnoreCase)) && x.IsAuthorisedAccountUser(actor));
        if (account == null)
        {
            actor.OutputHandler.Send("You don't have access to any bank account with that account number or alias.");
            return;
        }

        switch (text)
        {
            case "show":
                BankAccountShow(actor, ss, account, bank);
                return;
            case "transactions":
                BankAccountTransactions(actor, ss, account, bank);
                return;
        }

        switch (account.AccountStatus)
        {
            case BankAccountStatus.Locked:
                actor.OutputHandler.Send("That account has been locked and currently cannot be used.");
                return;
            case BankAccountStatus.Suspended:
                actor.OutputHandler.Send("That account has been suspended by the bank and cannot be used.");
                return;
            case BankAccountStatus.Closed:
                actor.OutputHandler.Send("That account has been closed, and cannot be used.");
                return;
        }

        switch (text)
        {
            case "deposit":
                BankAccountDeposit(actor, ss, account, bank);
                return;
            case "withdraw":
                BankAccountWithdraw(actor, ss, account, bank);
                return;
            case "transfer":
                BankAccountTransfer(actor, ss, account, bank);
                return;
            case "close":
                BankAccountClose(actor, ss, account, bank);
                return;
            case "requestitem":
                BankAccountRequestItem(actor, ss, account, bank);
                return;
            case "cancelitems":
                BankAccountCancelItems(actor, ss, account, bank);
                return;
            case "alias":
                BankAccountAlias(actor, ss, account, bank);
                return;
        }
    }

    private static void BankAccountAlias(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What alias do you want to give to that account?");
            return;
        }

        account.SetName(ss.SafeRemainingArgument);
        actor.OutputHandler.Send($"Bank account {account.AccountReference.ColourName()} now has the alias {account.Name.ColourCommand()} for use with bank commands.");
    }

    private static void BankAccountPreview(ICharacter actor, StringStack ss, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What type of bank account would you like to preview?\nYou can see the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        string typeText = ss.SafeRemainingArgument;
        IBankAccountType type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(typeText)) ??
                   bank.BankAccountTypes.FirstOrDefault(x =>
                       x.Name.StartsWith(typeText, StringComparison.InvariantCultureIgnoreCase));
        if (type == null)
        {
            actor.OutputHandler.Send(
                $"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        actor.OutputHandler.Send(type.ShowToCustomer(actor));
    }

    private static void BankAccountCreateShop(ICharacter actor, StringStack ss, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What type of bank account would you like to open?\nYou can see the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        string typeText = ss.PopSpeech();
        IBankAccountType type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(typeText)) ??
                   bank.BankAccountTypes.FirstOrDefault(x =>
                       x.Name.StartsWith(typeText, StringComparison.InvariantCultureIgnoreCase));
        if (type == null)
        {
            actor.OutputHandler.Send(
                $"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What shop are you trying to open a bank account on behalf of?");
            return;
        }

        string shopText = ss.SafeRemainingArgument;
        IShop shop = actor.Gameworld.Shops
                        .FirstOrDefault(x => x.Name.StartsWith(shopText, StringComparison.InvariantCultureIgnoreCase));
        if (shop == null)
        {
            actor.OutputHandler.Send("There is no shop by that name.");
            return;
        }

        if (!shop.IsManager(actor))
        {
            actor.OutputHandler.Send($"You are not authorised to act on behalf of {shop.Name.ColourName()}.");
            return;
        }

        (bool success, string error) = type.CanOpenAccount(shop);
        if (!success)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        IBankAccount account = type.OpenAccount(shop);
        bank.AddAccount(account);
        actor.OutputHandler.Send(
            $"You open a new {type.Name.ColourName()} account for {shop.Name.ColourName()} with {bank.Name.ColourName()}. The account number is {account.AccountNumber.ToString("N0", actor).ColourName()}.");
    }

    private static void BankAccountCreateClan(ICharacter actor, StringStack ss, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What type of bank account would you like to open?\nYou can see the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        string typeText = ss.PopSpeech();
        IBankAccountType type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(typeText)) ??
                   bank.BankAccountTypes.FirstOrDefault(x =>
                       x.Name.StartsWith(typeText, StringComparison.InvariantCultureIgnoreCase));
        if (type == null)
        {
            actor.OutputHandler.Send(
                $"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What clan are you trying to open a bank account on behalf of?");
            return;
        }

        string clanText = ss.SafeRemainingArgument;
        IClan clan = actor.Gameworld.Clans.GetByNameOrAbbreviation(clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send("There is no clan by that name.");
            return;
        }

        IClanMembership membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator() && (membership == null ||
                                         !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateBudgets)))
        {
            actor.OutputHandler.Send($"You are not authorised to act on behalf of {clan.Name.ColourName()}.");
            return;
        }

        (bool success, string error) = type.CanOpenAccount(clan);
        if (!success)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        IBankAccount account = type.OpenAccount(clan);
        bank.AddAccount(account);
        actor.OutputHandler.Send(
            $"You open a new {type.Name.ColourName()} account for {clan.Name.ColourName()} with {bank.Name.ColourName()}. The account number is {account.AccountNumber.ToString("N0", actor).ColourName()}.");
    }

    private static void BankAccountRequestItem(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        if (account.BankAccountType.NumberOfPermittedPaymentItems == 0 ||
            actor.Gameworld.ItemProtos.Get(account.BankAccountType.PaymentItemPrototype ?? 0)?.Status !=
            RevisionStatus.Current)
        {
            actor.OutputHandler.Send($"Your bank account does not permit the issue of payment items.");
            return;
        }

        if (account.NumberOfIssuedPaymentItems >= account.BankAccountType.NumberOfPermittedPaymentItems)
        {
            actor.OutputHandler.Send(
                "You have already had the maximum number of payment items issued to you. You must cancel the existing ones before you can have any more.");
            return;
        }

        IGameItem item = account.CreateNewPaymentItem();
        if (item is null)
        {
            actor.OutputHandler.Send($"Your bank account does not permit the issue of payment items.");
            return;
        }

        actor.Gameworld.Add(item);
        actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is issued $1 by the bank.", actor, actor, item)));
        if (actor.Body.CanGet(item, 0))
        {
            actor.Body.Get(item, silent: true);
        }
        else
        {
            item.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(item, true);
            item.SetPosition(PositionUndefined.Instance, PositionModifier.None, actor, null);
            actor.OutputHandler.Send(
                $"Your {actor.Body.Prototype.WielderDescriptionPlural.ToLowerInvariant()} were full so you set the item on the ground.");
        }

        item.HandleEvent(EventType.ItemFinishedLoading, item);
        item.Login();
    }

    private static void BankAccountCancelItems(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        if (account.NumberOfIssuedPaymentItems <= 0)
        {
            actor.OutputHandler.Send(
                "You don't currently have any issued payment items to cancel with that account.");
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure that you want to cancel all your existing issued payment items? They will no longer be able to be used to pay for things.\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                account.CancelPaymentItems();
                actor.OutputHandler.Send(
                    $"You cancel all your existing payment items for account {account.AccountReference.ColourName()}.");
            },
            RejectAction = text => { actor.OutputHandler.Send("You decide not to cancel your payment items."); },
            ExpireAction = () => { actor.OutputHandler.Send("You decide not to cancel your payment items."); },
            Keywords = new List<string> { "cancel", "payment" },
            DescriptionString = "cancelling your payment items from your bank account"
        }), TimeSpan.FromSeconds(120));
    }

    private static void BankAccountDeposit(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("How much money do you want to deposit?");
            return;
        }

        if (actor.Currency != bank.PrimaryCurrency)
        {
            actor.OutputHandler.Send(
                $"This bank conducts transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency. You must {$"set currency {bank.PrimaryCurrency.Name}".MXPSend()} before using its services.");
            return;
        }

        if (!bank.PrimaryCurrency.TryGetBaseCurrency(ss.SafeRemainingArgument, out decimal amount))
        {
            actor.OutputHandler.Send("That is not a valid amount of currency.");
            return;
        }

        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = bank.PrimaryCurrency.FindCurrency(
            actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
            amount);
        if (!targetCoins.Any())
        {
            actor.OutputHandler.Send("You aren't holding any currency of that type.");
            return;
        }

        decimal coinValue = targetCoins.TotalValue();
        if (coinValue < amount)
        {
            actor.OutputHandler.Send(
                $"You aren't holding enough money to make a deposit of that size.\nThe largest deposit that you could make is {bank.PrimaryCurrency.Describe(coinValue, CurrencyDescriptionPatternType.Short).ColourValue()}.");
            return;
        }

        decimal change = 0.0M;
        if (coinValue > amount)
        {
            change = coinValue - amount;
        }

        foreach (KeyValuePair<ICurrencyPile, Dictionary<ICoin, int>> item in targetCoins)
        {
            if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
            {
                actor.Body.Take(item.Key.Parent);
                item.Key.Parent.Delete();
            }
        }

        account.Deposit(amount);
        bank.CurrencyReserves[bank.PrimaryCurrency] += amount;
        bank.Changed = true;
        string moneyDescription = bank.PrimaryCurrency.Describe(amount, CurrencyDescriptionPatternType.Short)
                                   .ColourValue();
        actor.OutputHandler.Send(
            $"You deposit {moneyDescription} into account {account.NameWithAlias}.");
        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote($"@ deposits {moneyDescription} into a bank account.", actor, actor),
            flags: OutputFlags.SuppressSource));

        if (change > 0.0M)
        {
            IGameItem changeItem =
                CurrencyGameItemComponentProto.CreateNewCurrencyPile(bank.PrimaryCurrency,
                    bank.PrimaryCurrency.FindCoinsForAmount(change, out _));
            if (actor.Body.CanGet(changeItem, 0))
            {
                actor.Body.Get(changeItem, silent: true);
            }
            else
            {
                actor.Location.Insert(changeItem, true);
                actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
            }
        }
    }

    private static void BankAccountTransfer(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What account do you want to transfer money into?");
            return;
        }

        IBank bankTarget = null;
        IBankAccount accountTarget = null;
        string accTarget = ss.PopSpeech();

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("How much money do you want to transfer?");
            return;
        }

        if (actor.Currency != bank.PrimaryCurrency)
        {
            actor.OutputHandler.Send(
                $"This bank conducts transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency. You must {$"set currency {bank.PrimaryCurrency.Name}".MXPSend()} before using its services.");
            return;
        }

        if (!bank.PrimaryCurrency.TryGetBaseCurrency(ss.PopSpeech(), out decimal amount))
        {
            actor.OutputHandler.Send("That is not a valid amount of currency.");
            return;
        }

        (IBankAccount target, string accountError) = Economy.Banking.Bank.FindBankAccount(accTarget, bank, actor);
        if (target == null)
        {
            actor.OutputHandler.Send(accountError);
            return;
        }

        accountTarget = target;
        bankTarget = accountTarget.Bank;

        if (accountTarget == account)
        {
            actor.OutputHandler.Send("You can't transfer money from an account to itself.");
            return;
        }

        (bool success, string error) = account.CanWithdraw(amount, false);
        if (!success)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        decimal targetAmount = amount;
        if (bankTarget.PrimaryCurrency != bank.PrimaryCurrency)
        {
            if (bankTarget.ExchangeRates[(bank.PrimaryCurrency, bankTarget.PrimaryCurrency)] == 0.0M)
            {
                actor.OutputHandler.Send(
                    $"{bankTarget.Name.ColourName()} does not accept transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency.");
                return;
            }

            targetAmount *= bankTarget.ExchangeRates[(bank.PrimaryCurrency, bankTarget.PrimaryCurrency)];
        }

        account.WithdrawFromTransfer(amount, bankTarget.Code, accountTarget.AccountNumber, ss.SafeRemainingArgument);
        accountTarget.DepositFromTransfer(targetAmount, bank.Code, account.AccountNumber, ss.SafeRemainingArgument);
        if (bank != bankTarget)
        {
            bank.CurrencyReserves[bank.PrimaryCurrency] -= amount;
            bank.Changed = true;
            bankTarget.CurrencyReserves[bankTarget.PrimaryCurrency] += targetAmount;
            bankTarget.Changed = true;
        }

        string moneyDescription = bank.PrimaryCurrency.Describe(amount, CurrencyDescriptionPatternType.Short)
                                   .ColourValue();
        actor.OutputHandler.Send(
            $"You transfer {moneyDescription} from account {account.AccountNumber.ToString("F0", actor).ColourName()} into account {accountTarget.AccountNumber.ToString("F0", actor).ColourName()}{(bank != bankTarget ? $" of {bank.Name.ColourName()}" : "")}{(ss.SafeRemainingArgument.Length > 0 ? $", with reference {ss.SafeRemainingArgument.ColourCommand()}." : "")}.");
        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote($"@ transfers {moneyDescription} between bank accounts.", actor, actor),
            flags: OutputFlags.SuppressSource));
    }

    private static void BankAccountWithdraw(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("How much money do you want to withdraw?");
            return;
        }

        if (actor.Currency != bank.PrimaryCurrency)
        {
            actor.OutputHandler.Send(
                $"This bank conducts transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency. You must {$"set currency {bank.PrimaryCurrency.Name}".MXPSend()} before using its services.");
            return;
        }

        if (!bank.PrimaryCurrency.TryGetBaseCurrency(ss.SafeRemainingArgument, out decimal amount))
        {
            actor.OutputHandler.Send("That is not a valid amount of currency.");
            return;
        }

        (bool success, string reason) = account.CanWithdraw(amount, false);
        if (!success)
        {
            actor.OutputHandler.Send(reason);
            return;
        }

        if (amount > bank.CurrencyReserves[bank.PrimaryCurrency])
        {
            actor.OutputHandler.Send(
                $"Unfortunately, {bank.Name.ColourName()} does not have enough currency to honour your withdrawal.");
            return;
        }

        account.Withdraw(amount);
        bank.CurrencyReserves[bank.PrimaryCurrency] -= amount;
        bank.Changed = true;
        string moneyDescription = bank.PrimaryCurrency.Describe(amount, CurrencyDescriptionPatternType.Short)
                                   .ColourValue();
        actor.OutputHandler.Send(
            $"You withdraw {moneyDescription} from account {account.NameWithAlias}.");
        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote($"@ withdraws {moneyDescription} from a bank account.", actor, actor),
            flags: OutputFlags.SuppressSource));

        IGameItem currencyItem =
            CurrencyGameItemComponentProto.CreateNewCurrencyPile(bank.PrimaryCurrency,
                bank.PrimaryCurrency.FindCoinsForAmount(amount, out _));
        if (actor.Body.CanGet(currencyItem, 0))
        {
            actor.Body.Get(currencyItem, silent: true);
        }
        else
        {
            currencyItem.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(currencyItem, true);
            actor.OutputHandler.Send("You couldn't hold your money, so it is on the ground.");
        }
    }

    private static void BankAccountShow(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        actor.OutputHandler.Send(account.Show(actor));
    }

    private static void BankAccountTransactions(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        actor.OutputHandler.Send(account.ShowTransactions(actor));
    }

    private static void BankAccountClose(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
    {
        (bool truth, string reason) = account.BankAccountType.CanCloseAccount(actor, account);
        if (!truth)
        {
            actor.OutputHandler.Send(reason);
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure you want to close {account.Name.ColourName()} with {bank.Name.ColourName()}? This action cannot be undone.\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                (bool success, string error) = account.CloseAccount(actor);
                if (!success)
                {
                    actor.OutputHandler.Send(error);
                }
            },
            RejectAction = text => { actor.OutputHandler.Send("You decide not to close the bank account."); },
            ExpireAction = () => { actor.OutputHandler.Send("You decide not to close the bank account."); },
            DescriptionString = "Closing a bank account",
            Keywords = new List<string> { "close", "bank", "account" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void BankAccountTypes(ICharacter actor, IBank bank)
    {
        StringBuilder sb = new();
        List<IBankAccountType> accounts = bank.BankAccountTypes
                           .Where(x => x.CanOpenAccount(actor).Truth)
                           .ToList();
        if (!accounts.Any())
        {
            actor.OutputHandler.Send("There are no account types that you are eligible to open right now.");
            return;
        }

        sb.AppendLine($"You can open the following types of accounts with {bank.Name.ColourName()}:");

        foreach (IBankAccountType account in accounts)
        {
            sb.AppendLine();
            sb.AppendLine($"\t[{account.Name.Colour(Telnet.BoldCyan)}]");
            sb.AppendLine();
            sb.AppendLine(account.CustomerDescription.Wrap(actor.InnerLineFormatLength, "\t"));
            sb.AppendLine();
            sb.AppendLine(
                $"See {$"bank preview {account.Name.ToLowerInvariant()}".MXPSend()} for a full fee statement.");
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void BankAccountCreate(ICharacter actor, StringStack ss, IBank bank)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What type of bank account would you like to open?\nYou can see the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        IBankAccountType type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(ss.SafeRemainingArgument)) ??
                   bank.BankAccountTypes.FirstOrDefault(x =>
                       x.Name.StartsWith(ss.SafeRemainingArgument, StringComparison.InvariantCultureIgnoreCase));
        if (type == null)
        {
            actor.OutputHandler.Send(
                $"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
            return;
        }

        (bool success, string error) = bank.CanOpenAccount(actor, type);
        if (!success)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        IBankAccount account = type.OpenAccount(actor);
        bank.AddAccount(account);
        actor.OutputHandler.Send(
            $"You open a new {type.Name.ColourName()} account with {bank.Name.ColourName()}. The account number is {account.AccountNumber.ToString("N0", actor).ColourName()}.");
    }

    #endregion

    #region Auctions

    public const string AuctionHelp =
        @"The auction command is used to interact with auction houses, and it must be used at a location that is an auction house. You should also see the related command AUCTIONS.

The syntax for using this command is as follows:

	#3auction preview <lot>#0 - view an auction lot currently being auctioned
	#3auction sell <item> <price> <bank code>:<accn> [<buyout price>]#0 - lists an item for sale
	#3auction sell property <property> <price> <bank code>:<accn> [<buyout price>]#0 - lists your ownership share in a property for sale
	#3auction bid <lot> <bid>#0 - makes a bid on an auction lot
	#3auction buyout <lot>#0 - pays the buyout price on an auction lot
	#3auction claim#0 - claims all movable items won or not sold
	#3auction refund#0 - claims all money owed for unsuccessful bids
	#3auction cancel <lot>#0 - cancels an auction lot";

    public const string AuctionHelpAdmins = @"This command is used to create and edit auction houses.

The syntax for using this command is as follows:

	#3auction list#0 - lists all auction houses
	#3auction edit <which>#0 - begins editing an auction house
	#3auction close#0 - stops editing an auction house
	#3auction show <which>#0 - views an auction house
	#3auction edit new <name> <economic zone> <bank>#0 - creates a new auction house based in your current location
	#3auction set name <name>#0 - renames the auction house
	#3auction set economiczone <which>#0 - changes the economic zone
	#3auction set fee <amount>#0 - sets the flat fee for listing an item
	#3auction set rate <%>#0 - sets the percentage fee for listing an item
	#3auction set bank <bank code>:<accn>#0 - changes the bank account for revenues
	#3auction set time <time period>#0 - sets the amount of time auctions run for
	#3auction set location#0 - changes the location of the auction house to the current cell

There is also the player version of the command, which is used to interact with auction houses, and it must be used at a location that is an auction house. You should also see the related command AUCTIONS.

The syntax for using this command is as follows:

	#3auction preview <lot>#0 - view an auction lot currently being auctioned
	#3auction sell <item> <price> <bank code>:<accn> [<buyout price>]#0 - lists an item for sale
	#3auction sell property <property> <price> <bank code>:<accn> [<buyout price>]#0 - lists your ownership share in a property for sale
	#3auction bid <lot> <bid>#0 - makes a bid on an auction lot
	#3auction buyout <lot>#0 - pays the buyout price on an auction lot
	#3auction claim#0 - claims all movable items won or not sold
	#3auction refund#0 - claims all money owed for unsuccessful bids
	#3auction cancel <lot>#0 - cancels an auction lot

Note: Admins can use the #3auction cancel#0 subcommand on other people's items";

    [PlayerCommand("Auction", "auction")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("auction", AuctionHelp, AutoHelp.HelpArgOrNoArg, AuctionHelpAdmins)]
    protected static void Auction(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "edit":
            case "close":
            case "set":
            case "show":
            case "view":
            case "clone":
            case "list":
                BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.AuctionHelper);
                return;
        }

        IAuctionHouse auctionHouse = actor.Gameworld.AuctionHouses.FirstOrDefault(x => x.AuctionHouseCell == actor.Location);
        if (auctionHouse == null)
        {
            actor.OutputHandler.Send("You are not currently at an auction house.");
            return;
        }

        switch (ss.Last.ToLowerInvariant().CollapseString())
        {
            case "preview":
                AuctionPreview(actor, auctionHouse, ss);
                return;
            case "bid":
                AuctionBid(actor, auctionHouse, ss);
                return;
            case "refund":
                AuctionRefund(actor, auctionHouse, ss);
                return;
            case "buyout":
                AuctionBuyout(actor, auctionHouse, ss);
                return;
            case "sell":
                AuctionSell(actor, auctionHouse, ss);
                return;
            case "claim":
                AuctionClaim(actor, auctionHouse, ss);
                return;
            case "cancel":
                AuctionCancel(actor, auctionHouse, ss);
                return;
            default:
                actor.OutputHandler.Send(actor.IsAdministrator() ? AuctionHelpAdmins : AuctionHelp);
                return;
        }
    }

    private static void AuctionCancel(ICharacter actor, IAuctionHouse? auctionHouse, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which auction item do you want to cancel the auction of?");
            return;
        }

        AuctionItem item = ResolveAuctionLot(actor, auctionHouse, ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item currently being auctioned.");
            return;
        }

        if (!actor.IsAdministrator())
        {
            if (!CanManageAuctionLot(actor, item))
            {
                actor.OutputHandler.Send(
                    $"You are not authorised to cancel the sale of {DescribeAuctionLot(actor, item)}.");
                return;
            }

            if (auctionHouse.AuctionBids[item].Count > 0)
            {
                actor.OutputHandler.Send(
                    $"Unfortunately you cannot cancel the sale of {DescribeAuctionLot(actor, item)} as there have already been bids placed.");
                return;
            }
        }

        auctionHouse.CancelItem(item);
        actor.OutputHandler.Send(
            $"You cancel the auction of {DescribeAuctionLot(actor, item)} with {auctionHouse.Name.ColourName()}.");
        if (item.Item == null)
        {
            return;
        }

        IGameItem gameitem = item.Item;
        gameitem.Login();
        if (item.Seller.FrameworkItemEquals(actor.Id, actor.FrameworkItemType) && actor.Body.CanGet(gameitem, 0))
        {
            actor.Body.Get(gameitem, silent: true);
            return;
        }

        gameitem.RoomLayer = actor.RoomLayer;
        auctionHouse.AuctionHouseCell.Insert(gameitem);
    }

    private static void AuctionClaim(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
    {
        List<UnclaimedAuctionItem> unclaimed = auctionHouse.UnclaimedItems.Where(x =>
                                        x.AuctionItem.Item != null &&
                                        (x.WinningBid?.Bidder == actor ||
                                         (x.WinningBid == null &&
                                          x.AuctionItem.Seller.FrameworkItemEquals(actor.Id, actor.FrameworkItemType))))
                                    .ToList();
        if (!unclaimed.Any())
        {
            actor.OutputHandler.Send("You do not have any unclaimed auction items.");
            return;
        }

        foreach (UnclaimedAuctionItem item in unclaimed)
        {
            auctionHouse.ClaimItem(item.AuctionItem);
        }

        foreach (UnclaimedAuctionItem item in unclaimed)
        {
            item.AuctionItem.Item!.Login();
            item.AuctionItem.Item.SetOwner(item.WinningBid?.Bidder ?? actor);
        }

        List<IGameItem> items = unclaimed.Select(x => x.AuctionItem.Item!).ToList();

        IGameItem givenItem = null;
        if (items.Count > 1)
        {
            givenItem = PileGameItemComponentProto.CreateNewBundle(items);
            actor.Gameworld.Add(givenItem);
        }
        else
        {
            givenItem = items.Single();
        }

        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote($"@ claim|claims $1 from {auctionHouse.Name.ColourName()}.", actor, actor, givenItem)));
        if (actor.Body.CanGet(givenItem, 0))
        {
            actor.Body.Get(givenItem, silent: true);
        }
        else
        {
            givenItem.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(givenItem);
            givenItem.SetPosition(PositionUndefined.Instance, PositionModifier.Before, actor, null);
            actor.OutputHandler.Send(
                $"You were unable to pick up {givenItem.HowSeen(actor)}, so it is on the ground at your feet.");
        }
    }

    private static void AuctionSell(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What is it that you want to sell?");
            return;
        }

        string targetText = ss.PopSpeech();
        bool propertySale = targetText.EqualTo("property");
        IGameItem item = null;
        IProperty property = null;
        decimal propertyShare = 1.0M;
        if (propertySale)
        {
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("Which property do you want to list?");
                return;
            }

            List<IProperty> properties = actor.Gameworld.Properties
                .Where(x => x.EconomicZone == auctionHouse.EconomicZone)
                .Where(x => x.PropertyOwners.Any(y => y.Owner == actor && y.ShareOfOwnership > 0.0M))
                .ToList();
            property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
            if (property == null)
            {
                actor.OutputHandler.Send("You do not own any such property share in this auction house's economic zone.");
                return;
            }

            propertyShare = property.PropertyOwners
                .Where(x => x.Owner == actor)
                .Sum(x => x.ShareOfOwnership);
            if (propertyShare <= 0.0M)
            {
                actor.OutputHandler.Send("You do not own any share in that property.");
                return;
            }

            if (property.SaleOrder != null)
            {
                actor.OutputHandler.Send(
                    $"The property {property.Name.ColourName()} is already listed for sale. Cancel that sale order before listing it on the auction house.");
                return;
            }

            if (actor.Gameworld.AuctionHouses.SelectMany(x => x.ActiveAuctionItems).Any(x =>
                    x.Asset.FrameworkItemEquals(property.Id, property.FrameworkItemType) &&
                    x.Seller.FrameworkItemEquals(actor.Id, actor.FrameworkItemType)) ||
                actor.Gameworld.AuctionHouses.SelectMany(x => x.UnclaimedItems).Any(x =>
                    x.AuctionItem.Asset.FrameworkItemEquals(property.Id, property.FrameworkItemType) &&
                    x.AuctionItem.Seller.FrameworkItemEquals(actor.Id, actor.FrameworkItemType)))
            {
                actor.OutputHandler.Send(
                    $"Your ownership share in {property.Name.ColourName()} is already listed on an auction house.");
                return;
            }

            if (property.Lease is null && property.PropertyKeys.Any(x => !x.IsReturned))
            {
                actor.OutputHandler.Send(
                    "The property still has outstanding keys that must be returned before it can be listed for auction.");
                return;
            }
        }
        else
        {
            item = actor.TargetHeldItem(targetText);
            if (item == null)
            {
                actor.OutputHandler.Send("You aren't holding anything like that.");
                return;
            }
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What price do you want to list {(propertySale ? property.Name.ColourName() : item.HowSeen(actor))} for? Prices must be in the {auctionHouse.EconomicZone.Currency.Name.TitleCase().ColourName()} currency.");
            return;
        }

        if (!auctionHouse.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out decimal price))
        {
            actor.OutputHandler.Send(
                $"The value {ss.Last.ColourValue()} is not a valid amount of {auctionHouse.EconomicZone.Currency.Name.TitleCase().ColourName()}.");
            return;
        }

        if (price <= auctionHouse.AuctionListingFeeFlat)
        {
            actor.OutputHandler.Send(
                $"You must list {(propertySale ? property.Name.ColourName() : item.HowSeen(actor))} for a price greater than {auctionHouse.EconomicZone.Currency.Describe(auctionHouse.AuctionListingFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "You must specify a bank account into which any proceeds will be transferred. Use the format BANKCODE:ACCOUNT#.");
            return;
        }

        string bankString = ss.PopSpeech();
        string[] split = bankString.Split(':');
        if (split.Length != 2)
        {
            actor.OutputHandler.Send($"You must use the format BANKCODE:ACCOUNT# to specify the bank account.");
            return;
        }

        IBank bankTarget = actor.Gameworld.Banks.GetByName(split[0]) ??
                         actor.Gameworld.Banks.FirstOrDefault(x =>
                             x.Code.StartsWith(split[0], StringComparison.InvariantCultureIgnoreCase));
        if (bankTarget == null)
        {
            actor.OutputHandler.Send("There is no bank with that name or bank code.");
            return;
        }

        if (!int.TryParse(split[1], out int accn) || accn <= 0)
        {
            actor.OutputHandler.Send("The account number to transfer money into must be a number greater than zero.");
            return;
        }

        IBankAccount accountTarget = bankTarget.BankAccounts.FirstOrDefault(x =>
            x.AccountNumber == accn && x.AccountStatus == BankAccountStatus.Active);
        if (accountTarget == null)
        {
            actor.OutputHandler.Send(
                $"The supplied account number is not a valid account number for {bankTarget.Name.ColourName()}.");
            return;
        }

        decimal buyout = 0.0M;
        if (!ss.IsFinished)
        {
            if (!auctionHouse.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out buyout))
            {
                actor.OutputHandler.Send(
                    $"The value {ss.Last.ColourValue()} is not a valid amount of {auctionHouse.EconomicZone.Currency.Name.TitleCase().ColourName()}.");
                return;
            }

            if (buyout < price)
            {
                actor.OutputHandler.Send(
                    "You must specify a buyout price that is higher than the starting sale price.");
                return;
            }
        }

        string itemDesc = propertySale
            ? $"{propertyShare.ToString("P2", actor).ColourValue()} ownership share in {property.Name.ColourName()}"
            : item.HowSeen(actor);
        actor.OutputHandler.Send(
            $"Are you sure you want to list {itemDesc} for sale at a reserve price of {auctionHouse.EconomicZone.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}?\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                if (propertySale)
                {
                    decimal currentPropertyShare = property.PropertyOwners
                        .Where(x => x.Owner == actor)
                        .Sum(x => x.ShareOfOwnership);
                    if (currentPropertyShare <= 0.0M)
                    {
                        actor.OutputHandler.Send($"You no longer own {itemDesc}.");
                        return;
                    }

                    propertyShare = currentPropertyShare;
                }
                else if (!actor.Body.HeldOrWieldedItems.Contains(item) || !actor.Body.CanDrop(item, 1))
                {
                    actor.OutputHandler.Send($"You no longer have {itemDesc}.");
                    return;
                }

                if (actor.Location != auctionHouse.AuctionHouseCell)
                {
                    actor.OutputHandler.Send($"You are no longer in the auction house.");
                    return;
                }

                actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ list|lists $1 for sale on the auction house.",
                    actor, actor, propertySale ? new DummyPerceivable(property.Name.ColourName()) : item)));
                if (!propertySale)
                {
                    actor.Body.Take(item);
                    item.Drop(null);
                }

                auctionHouse.AddAuctionItem(new AuctionItem
                {
                    Asset = propertySale ? property : item,
                    Seller = actor,
                    PayoutTarget = accountTarget,
                    PropertyShare = propertyShare,
                    ListingDateTime = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
                    FinishingDateTime =
                        new MudDateTime(auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime) +
                        auctionHouse.DefaultListingTime,
                    MinimumPrice = price,
                    BuyoutPrice = buyout > 0.0M ? buyout : null
                });
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send($"You decide not to list {itemDesc} for sale on the auction house.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send($"You decide not to list {itemDesc} for sale on the auction house.");
            },
            DescriptionString = "List an item for sale on the auction house"
        }), TimeSpan.FromSeconds(120));
    }

    private static void AuctionBuyout(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which auction item do you want to buy out?");
            return;
        }

        AuctionItem item = ResolveAuctionLot(actor, auctionHouse, ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item currently being auctioned.");
            return;
        }

        if (!item.BuyoutPrice.HasValue)
        {
            actor.OutputHandler.Send(
                $"{DescribeAuctionLot(actor, item)} does not have a buyout price.");
            return;
        }

        decimal amount = item.BuyoutPrice.Value;
        ICurrency currency = auctionHouse.EconomicZone.Currency;
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = currency.FindCurrency(actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
            amount);
        if (!targetCoins.Any())
        {
            actor.OutputHandler.Send("You aren't holding any currency of that type.");
            return;
        }

        decimal coinValue = targetCoins.TotalValue();
        if (coinValue < amount)
        {
            actor.OutputHandler.Send(
                $"You aren't holding enough money to pay the {currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} buyout price.");
            return;
        }

        decimal change = 0.0M;
        if (coinValue > amount)
        {
            change = coinValue - amount;
        }

        foreach (KeyValuePair<ICurrencyPile, Dictionary<ICoin, int>> coinItem in targetCoins)
        {
            if (!coinItem.Key.RemoveCoins(coinItem.Value.Select(x => Tuple.Create(x.Key, x.Value))))
            {
                actor.Body.Take(coinItem.Key.Parent);
                coinItem.Key.Parent.Delete();
            }
        }

        auctionHouse.AddBid(item, new AuctionBid
        {
            Bidder = actor,
            Bid = amount,
            BidDateTime = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
        });
        string moneyDescription = currency.Describe(amount, CurrencyDescriptionPatternType.Short)
                                       .ColourValue();
        actor.OutputHandler.Send(
            $"You pay the buyout price of {moneyDescription} on {DescribeAuctionLot(actor, item)} at {auctionHouse.Name.ColourName()}.");

        if (change > 0.0M)
        {
            IGameItem changeItem =
                CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
                    currency.FindCoinsForAmount(change, out _));
            if (actor.Body.CanGet(changeItem, 0))
            {
                actor.Body.Get(changeItem, silent: true);
            }
            else
            {
                changeItem.RoomLayer = actor.RoomLayer;
                actor.Location.Insert(changeItem, true);
                actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
            }
        }
    }

    private static void AuctionRefund(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
    {
        decimal owed = auctionHouse.BidderRefundsOwed[actor.Id];
        if (owed <= 0.0M)
        {
            actor.OutputHandler.Send($"{auctionHouse.Name.ColourName()} does not owe you any refunds.");
            return;
        }

        if (!auctionHouse.ClaimRefund(actor))
        {
            actor.OutputHandler.Send(
                $"{auctionHouse.Name.ColourName()} does not have enough money to pay what they owe you.");
            return;
        }

        string moneyDescription = auctionHouse.EconomicZone.Currency.Describe(owed, CurrencyDescriptionPatternType.Short)
                                           .ColourValue();
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(
            $"@ claim|claims a $1 refund from {auctionHouse.Name.ColourName()}.", actor, actor,
            new DummyPerceivable(moneyDescription))));

        IGameItem currencyItem =
            CurrencyGameItemComponentProto.CreateNewCurrencyPile(auctionHouse.EconomicZone.Currency,
                auctionHouse.EconomicZone.Currency.FindCoinsForAmount(owed, out _));
        if (actor.Body.CanGet(currencyItem, 0))
        {
            actor.Body.Get(currencyItem, silent: true);
        }
        else
        {
            currencyItem.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(currencyItem, true);
            actor.OutputHandler.Send("You couldn't hold your money, so it is on the ground.");
        }
    }

    private static void AuctionBid(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which auction item do you want to bid on?");
            return;
        }

        List<string> bidParts = new StringStack(ss.RemainingArgument).PopSpeechAll().ToList();
        if (bidParts.Count < 2)
        {
            actor.OutputHandler.Send("You must specify both an auction lot and a bid amount.");
            return;
        }

        string bidText = bidParts.Last();
        string lotText = string.Join(" ", bidParts.Take(bidParts.Count - 1));
        AuctionItem item = ResolveAuctionLot(actor, auctionHouse, lotText);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item currently being auctioned.");
            return;
        }

        ICurrency currency = auctionHouse.EconomicZone.Currency;
        if (!currency.TryGetBaseCurrency(bidText, out decimal amount))
        {
            actor.OutputHandler.Send($"That is not a valid amount of {currency.Name.ColourValue()}.");
            return;
        }

        decimal currentPrice = auctionHouse.AuctionBids[item].Select(x => x.Bid).DefaultIfEmpty(item.MinimumPrice)
                                       .Max();
        decimal nextBidMinimum = !auctionHouse.AuctionBids[item].Any() ? item.MinimumPrice : currentPrice * 1.05M;
        if (amount <= nextBidMinimum)
        {
            actor.OutputHandler.Send(
                $"Your bid for {DescribeAuctionLot(actor, item)} must be higher than {currency.Describe(nextBidMinimum, CurrencyDescriptionPatternType.Short).ColourValue()}.");
            return;
        }

        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = currency.FindCurrency(actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
            amount);
        if (!targetCoins.Any())
        {
            actor.OutputHandler.Send("You aren't holding any currency of that type.");
            return;
        }

        decimal coinValue = targetCoins.TotalValue();
        if (coinValue < amount)
        {
            actor.OutputHandler.Send(
                $"You aren't holding enough money to make a bid of that size.\nThe largest bid that you could make is {currency.Describe(coinValue, CurrencyDescriptionPatternType.Short).ColourValue()}.");
            return;
        }

        decimal change = 0.0M;
        if (coinValue > amount)
        {
            change = coinValue - amount;
        }

        foreach (KeyValuePair<ICurrencyPile, Dictionary<ICoin, int>> coinItem in targetCoins)
        {
            if (!coinItem.Key.RemoveCoins(coinItem.Value.Select(x => Tuple.Create(x.Key, x.Value))))
            {
                actor.Body.Take(coinItem.Key.Parent);
                coinItem.Key.Parent.Delete();
            }
        }

        auctionHouse.AddBid(item, new AuctionBid
        {
            Bidder = actor,
            Bid = amount,
            BidDateTime = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
        });

        string moneyDescription = currency.Describe(amount, CurrencyDescriptionPatternType.Short)
                                       .ColourValue();
        actor.OutputHandler.Send(
            $"You bid {moneyDescription} on {DescribeAuctionLot(actor, item)} at {auctionHouse.Name.ColourName()}.");

        if (change > 0.0M)
        {
            IGameItem changeItem =
                CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
                    currency.FindCoinsForAmount(change, out _));
            if (actor.Body.CanGet(changeItem, 0))
            {
                actor.Body.Get(changeItem, silent: true);
            }
            else
            {
                changeItem.RoomLayer = actor.RoomLayer;
                actor.Location.Insert(changeItem, true);
                actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
            }
        }
    }

    private static void AuctionPreview(ICharacter actor, IAuctionHouse auctionHouse, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which auction item do you want to preview?");
            return;
        }

        AuctionItem item = ResolveAuctionLot(actor, auctionHouse, command.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item currently being auctioned.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Previewing {DescribeAuctionLot(actor, item)}");
        sb.AppendLine();
        if (item.Asset is IGameItem gameItem)
        {
            sb.AppendLine(gameItem.HowSeen(actor, type: DescriptionType.Full,
                flags: PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreCanSee));
        }
        else if (item.Asset is IProperty property)
        {
            sb.AppendLine(property.PreviewProperty(actor));
        }
        sb.AppendLine();
        sb.AppendLine(
            $"Current Price: {auctionHouse.EconomicZone.Currency.Describe(auctionHouse.CurrentBid(item).IfZero(item.MinimumPrice), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        if (item.BuyoutPrice.HasValue)
        {
            sb.AppendLine(
                $"Buyout Price: {auctionHouse.EconomicZone.Currency.Describe(item.BuyoutPrice.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        }

        sb.AppendLine($"# of Bids: {auctionHouse.AuctionBids[item].Count.ToString("N0", actor).ColourValue()}");
        sb.AppendLine(
            $"Time Remaining: {(item.FinishingDateTime - auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime).Describe(actor).ColourValue()}");
        actor.OutputHandler.Send(sb.ToString());
    }

    [PlayerCommand("Auctions", "auctions")]
    [RequiredCharacterState(CharacterState.Able)]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("auctions", @"", AutoHelp.HelpArg)]
    protected static void Auctions(ICharacter actor, string command)
    {
        IAuctionHouse auctionHouse = actor.Gameworld.AuctionHouses.FirstOrDefault(x => x.AuctionHouseCell == actor.Location);
        if (auctionHouse == null)
        {
            actor.OutputHandler.Send("You are not currently at an auction house.");
            return;
        }

        List<AuctionItem> listings = auctionHouse.ActiveAuctionItems.ToList();

        StringStack ss = new(command.RemoveFirstWord());
        while (!ss.IsFinished)
        {
            string cmd = ss.PopSpeech();
            listings = listings.Where(x =>
                x.Asset.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (listings.Count == 0)
            {
                actor.OutputHandler.Send($"There are no auction listings with the keyword {cmd.ColourCommand()}.");
                return;
            }
        }

        StringBuilder psb = new();

        if (auctionHouse.BidderRefundsOwed[actor.Id] > 0)
        {
            psb.AppendLine(
                $"\nThe auction house owes you {auctionHouse.EconomicZone.Currency.Describe(auctionHouse.BidderRefundsOwed[actor.Id], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in refunds.\nType {"AUCTION REFUND".MXPSend("auction refund", "Click to send AUCTION REFUND to the MUD")} to claim your money.");
        }

        List<UnclaimedAuctionItem> unclaimed = auctionHouse.UnclaimedItems.Where(x =>
                                        (x.AuctionItem.Seller.FrameworkItemEquals(actor.Id, actor.FrameworkItemType) && x.WinningBid == null) ||
                                        x.WinningBid?.BidderId == actor.Id
                                    )
                                    .ToList();
        if (unclaimed.Any())
        {
            psb.AppendLine(
                $"\nYou have unclaimed items with this auction house. Type {"AUCTION CLAIM".MXPSend("auction claim", "Click to send AUCTION CLAIM to the MUD")} to claim your items.");
        }

        if (listings.Count == 0)
        {
            actor.OutputHandler.Send(
                $"{auctionHouse.Name.TitleCase().ColourName()} currently has no listings.{psb}");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine(auctionHouse.Name.TitleCase().ColourName());
        sb.AppendLine();
        int i = 1;
        MudDateTime now = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        sb.AppendLine(StringUtilities.GetTextTable(
            from listing in listings
            orderby listing.ListingDateTime
            select new List<string>
            {
                (i++).ToString("N0", actor),
                DescribeAuctionLot(actor, listing),
                auctionHouse.AuctionBids[listing].Count.ToString("N0", actor),
                auctionHouse.EconomicZone.Currency.Describe(
                    auctionHouse.AuctionBids[listing].Select(x => x.Bid).DefaultIfEmpty(listing.MinimumPrice).Max(),
                    CurrencyDescriptionPatternType.ShortDecimal),
                (listing.FinishingDateTime - now).Describe(actor),
                listing.BuyoutPrice.HasValue
                    ? auctionHouse.EconomicZone.Currency.Describe(listing.BuyoutPrice.Value,
                        CurrencyDescriptionPatternType.ShortDecimal)
                    : "N/A"
            },
            new List<string>
            {
                "#",
                "Item",
                "# Bids",
                "Current Bid",
                "Time Remaining",
                "Buyout"
            },
            actor.LineFormatLength,
            colour: Telnet.Yellow,
            unicodeTable: actor.Account.UseUnicode
        ));
        if (psb.Length > 0)
        {
            sb.Append(psb.ToString());
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    public const string HeirHelp = @"The heir command lets you choose who should receive the remainder of your estate when you die.

The syntax for this command is as follows:

	#3heir#0 - shows your current heir
	#3heir <character>#0 - sets a character as your heir
	#3heir clan <clan>#0 - sets a clan as your heir
	#3heir clear#0 - clears your current heir";

    [PlayerCommand("Heir", "heir")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [HelpInfo("heir", HeirHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Heir(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (ss.IsFinished || ss.PeekSpeech().EqualToAny("show", "view", "current"))
        {
            actor.OutputHandler.Send(
                actor.EstateHeir == null
                    ? "You have not nominated any heir for your estate."
                    : $"Your estate heir is currently {DescribeFrameworkItem(actor, actor.EstateHeir)}.");
            return;
        }

        if (ss.PeekSpeech().EqualToAny("clear", "none", "remove"))
        {
            actor.EstateHeir = null;
            actor.OutputHandler.Send("You clear your estate heir nomination.");
            return;
        }

        if (ss.PeekSpeech().EqualTo("clan"))
        {
            ss.PopSpeech();
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("Which clan do you want to nominate as your heir?");
                return;
            }

            IClan clan = GetClanByIdOrName(actor, ss.SafeRemainingArgument);
            if (clan == null)
            {
                actor.OutputHandler.Send("There is no such clan.");
                return;
            }

            actor.EstateHeir = clan;
            actor.OutputHandler.Send($"You nominate {clan.FullName.ColourName()} as your estate heir.");
            return;
        }

        ICharacter target = actor.TargetActor(ss.SafeRemainingArgument);
        if (target == null)
        {
            actor.OutputHandler.Send("You can only nominate someone present with you as your estate heir.");
            return;
        }

        if (target == actor)
        {
            actor.OutputHandler.Send("Naming yourself as your heir would not achieve much.");
            return;
        }

        actor.EstateHeir = target;
        actor.OutputHandler.Send(
            $"You nominate {target.PersonalName.GetName(NameStyle.FullName).ColourName()} as your estate heir.");
    }

    public const string OwnershipHelp = @"The ownership command is used to inspect and manage durable ownership of items.

The syntax for this command is as follows:

	#3ownership#0 - lists visible items on your person and in the room together with their ownership status
	#3ownership show <item>#0 - shows who owns an item
	#3ownership claim <item>#0 - claims ownership of an unowned item
	#3ownership claim deep [<possessed item>]#0 - claims everything in your possession, or one possessed item, plus all contents recursively
	#3ownership clan <clan> <item>#0 - marks an item as clan-owned public property
	#3ownership clear <item>#0 - clears an item's owner (admin only)
	#3ownership set character <character> <item>#0 - sets an item's owner to a character (admin only)
	#3ownership set clan <clan> <item>#0 - sets an item's owner to a clan (admin only)";

    [PlayerCommand("Ownership", "ownership", "owner", "own")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [HelpInfo("ownership", OwnershipHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Ownership(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (ss.IsFinished)
        {
            OwnershipSummary(actor);
            return;
        }

        switch (ss.PopForSwitch())
        {
            case "show":
            case "view":
                OwnershipShow(actor, ss);
                return;
            case "claim":
                OwnershipClaim(actor, ss);
                return;
            case "clan":
                OwnershipClan(actor, ss);
                return;
            case "clear":
                OwnershipClear(actor, ss);
                return;
            case "set":
                OwnershipSet(actor, ss);
                return;
            default:
                OwnershipShow(actor, ss.GetUndo());
                return;
        }
    }

    private static void OwnershipSummary(ICharacter actor)
    {
        List<List<string>> onPersonRows = new();
        AppendOwnershipRows(actor, onPersonRows, actor.Body.ExternalItems, "Person");

        List<List<string>> roomRows = new();
        AppendOwnershipRows(actor, roomRows,
            actor.Location.LayerGameItems(actor.RoomLayer).Where(x => actor.CanSee(x)),
            "Room");

        StringBuilder sb = new();
        sb.AppendLine("Ownership on your person:");
        sb.AppendLine(onPersonRows.Any()
            ? StringUtilities.GetTextTable(onPersonRows, new List<string> { "Where", "Item", "Owner" }, actor,
                Telnet.Yellow)
            : "\tNone.");
        sb.AppendLine();
        sb.AppendLine("Ownership in the room:");
        sb.AppendLine(roomRows.Any()
            ? StringUtilities.GetTextTable(roomRows, new List<string> { "Where", "Item", "Owner" }, actor,
                Telnet.Yellow)
            : "\tNone.");
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void OwnershipShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to inspect ownership for?");
            return;
        }

        IGameItem item = actor.TargetItem(ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item here.");
            return;
        }

        actor.OutputHandler.Send(
            item.HasOwner
                ? $"{item.HowSeen(actor)} is owned by {DescribeOwnershipForViewer(actor, item)}."
                : $"{item.HowSeen(actor)} is currently unowned.");
    }

    private static void OwnershipClaim(ICharacter actor, StringStack ss)
    {
        if (ss.PeekSpeech().EqualTo("deep"))
        {
            ss.PopSpeech();
            OwnershipClaimDeep(actor, ss);
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to claim ownership of?");
            return;
        }

        IGameItem item = actor.TargetItem(ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item here.");
            return;
        }

        if (item.HasOwner)
        {
            actor.OutputHandler.Send($"{item.HowSeen(actor)} already has a registered owner.");
            return;
        }

        item.SetOwner(actor);
        actor.OutputHandler.Send($"You claim ownership of {item.HowSeen(actor)}.");
    }

    private static void OwnershipClaimDeep(ICharacter actor, StringStack ss)
    {
        List<IGameItem> rootItems = ss.IsFinished
            ? actor.Body.AllItems.ToList()
            : new List<IGameItem> { actor.Body.AllItems.GetFromItemListByKeyword(ss.SafeRemainingArgument, actor) };
        if (rootItems.Any(x => x == null))
        {
            actor.OutputHandler.Send("You do not possess anything like that.");
            return;
        }

        if (!rootItems.Any())
        {
            actor.OutputHandler.Send("You are not carrying, wearing, implanting or otherwise possessing anything that can be deep-claimed.");
            return;
        }

        List<IGameItem> claimed = new();
        List<IGameItem> alreadyOwned = new();
        foreach (IGameItem item in rootItems.SelectMany(x => x.DeepItems).Distinct())
        {
            if (item.HasOwner)
            {
                alreadyOwned.Add(item);
                continue;
            }

            item.SetOwner(actor);
            claimed.Add(item);
        }

        if (!claimed.Any())
        {
            actor.OutputHandler.Send(
                alreadyOwned.Any()
                    ? "Everything in that deep-claim selection already has a registered owner."
                    : "There was nothing available to claim.");
            return;
        }

        StringBuilder summary = new();
        summary.Append(
            $"You deep-claim {claimed.Count.ToString("N0", actor).ColourValue()} item{"s".Pluralise(claimed.Count != 1)}");
        if (alreadyOwned.Any())
        {
            summary.Append(
                $" and skip {alreadyOwned.Count.ToString("N0", actor).ColourValue()} that already had owner{"s".Pluralise(alreadyOwned.Count != 1)}");
        }

        summary.Append(".");
        actor.OutputHandler.Send(summary.ToString());
    }

    private static void OwnershipClan(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to set as the owner?");
            return;
        }

        IClan clan = GetClanByIdOrName(actor, ss.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to mark as clan-owned?");
            return;
        }

        IGameItem item = actor.TargetItem(ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item here.");
            return;
        }

        bool canManageClanProperty = actor.IsAdministrator() ||
                                    actor.ClanMemberships.Any(x =>
                                        !x.IsArchivedMembership &&
                                        x.Clan == clan &&
                                        x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty));
        if (!canManageClanProperty)
        {
            actor.OutputHandler.Send($"You do not have permission to manage property for {clan.FullName.ColourName()}.");
            return;
        }

        if (item.HasOwner && item.Owner != clan && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send($"{item.HowSeen(actor)} already has a different registered owner.");
            return;
        }

        item.SetOwner(clan);
        actor.OutputHandler.Send($"You mark {item.HowSeen(actor)} as property of {clan.FullName.ColourName()}.");
    }

    private static void OwnershipClear(ICharacter actor, StringStack ss)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only administrators can forcibly clear item ownership.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to clear ownership for?");
            return;
        }

        IGameItem item = actor.TargetItem(ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item here.");
            return;
        }

        item.ClearOwner();
        actor.OutputHandler.Send($"You clear the registered owner of {item.HowSeen(actor)}.");
    }

    private static void OwnershipSet(ICharacter actor, StringStack ss)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only administrators can forcibly set item ownership.");
            return;
        }

        switch (ss.PopForSwitch())
        {
            case "character":
            case "char":
                OwnershipSetCharacter(actor, ss);
                return;
            case "clan":
                OwnershipSetClan(actor, ss);
                return;
            default:
                actor.OutputHandler.Send("You must specify either character or clan as the ownership target type.");
                return;
        }
    }

    private static void OwnershipSetCharacter(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which character should own the item?");
            return;
        }

        ICharacter target = GetCharacterByIdOrName(actor, ss.PopSpeech());
        if (target == null)
        {
            actor.OutputHandler.Send("There is no such character.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to assign?");
            return;
        }

        IGameItem item = actor.TargetItem(ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item here.");
            return;
        }

        item.SetOwner(target);
        actor.OutputHandler.Send(
            $"You set {DescribeFrameworkItem(actor, target)} as the owner of {item.HowSeen(actor)}.");
    }

    private static void OwnershipSetClan(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which clan should own the item?");
            return;
        }

        IClan clan = GetClanByIdOrName(actor, ss.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to assign?");
            return;
        }

        IGameItem item = actor.TargetItem(ss.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item here.");
            return;
        }

        item.SetOwner(clan);
        actor.OutputHandler.Send(
            $"You set {clan.FullName.ColourName()} as the owner of {item.HowSeen(actor)}.");
    }

    public const string EstateHelp = @"The estate command is used to review probate estates, submit claims, and adjudicate them if you have authority in the relevant economic zone.

The syntax for this command is as follows:

	#3estate list#0 - lists estates you can currently see
	#3estate show <id>#0 - shows details of a particular estate
	#3estate create#0 - creates or refreshes your will for the current probate office's zone
	#3estate bequeath <estate> <asset> <character|clan> <who> [<reason>]#0 - sets a pre-approved bequest from your will
	#3estate revoke <estate> <claim>#0 - removes a prior bequest from your will
	#3estate claim <id> <amount> <reason>#0 - submits a claim against an estate
	#3estate claim <id> asset <asset> <reason>#0 - submits a claim against a specific estate asset
	#3estate payout [<estate>]#0 - collects cash payouts that probate owes you at this office
	#3estate approve <estate> <claim> [reason]#0 - approves an estate claim
	#3estate reject <estate> <claim> <reason>#0 - rejects an estate claim
	#3estate listasset <estate> <asset> [<reserve>] [<buyout>]#0 - manually lists an estate asset on the zone probate auction house
	#3estate relist <estate> <asset> [<reserve>] [<buyout>]#0 - relists an unsold estate asset
	#3estate liquidate <id>#0 - manually moves an estate into liquidation
	#3estate open <id>#0 - opens probate for an undiscovered estate immediately
	#3estate finalise <id>#0 - finalises an estate immediately";

    [PlayerCommand("Estate", "estate", "estates")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [HelpInfo("estate", EstateHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Estate(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "create":
            case "will":
                EstateCreate(actor, ss);
                return;
            case "bequeath":
                EstateBequeath(actor, ss);
                return;
            case "revoke":
            case "remove":
                EstateRevoke(actor, ss);
                return;
            case "list":
                EstateList(actor, ss);
                return;
            case "show":
            case "view":
                EstateShow(actor, ss);
                return;
            case "payout":
            case "pay":
            case "collect":
                EstateCollectPayout(actor, ss);
                return;
            case "claim":
                EstateClaim(actor, ss);
                return;
            case "approve":
                EstateAssess(actor, ss, true);
                return;
            case "reject":
                EstateAssess(actor, ss, false);
                return;
            case "listasset":
            case "auction":
                EstateAuctionAsset(actor, ss, false);
                return;
            case "relist":
                EstateAuctionAsset(actor, ss, true);
                return;
            case "liquidate":
            case "liquidation":
                EstateLiquidate(actor, ss);
                return;
            case "open":
                EstateOpen(actor, ss);
                return;
            case "finalise":
            case "finalize":
                EstateFinalise(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(EstateHelp.SubstituteANSIColour());
                return;
        }
    }

    private static void EstateCreate(ICharacter actor, StringStack ss)
    {
        if (!EnsureEstateCommandLocation(actor))
        {
            return;
        }

        IEconomicZone zone = CurrentProbateOfficeZone(actor);
        if (zone == null)
        {
            actor.OutputHandler.Send("You must be at a probate office to create a will.");
            return;
        }

        if (!zone.EstatesEnabled)
        {
            actor.OutputHandler.Send($"{zone.Name.ColourName()} does not currently use estates.");
            return;
        }

        Estate estate = actor.Gameworld.Estates
            .Where(x => x.Character == actor && x.EconomicZone == zone)
            .Where(x => x.EstateStatus != EstateStatus.Cancelled && x.EstateStatus != EstateStatus.Finalised)
            .OfType<Estate>()
            .OrderBy(x => x.EstateStartTime)
            .FirstOrDefault();
        if (estate != null && estate.EstateStatus != EstateStatus.EstateWill)
        {
            actor.OutputHandler.Send("You already have an active estate in this economic zone.");
            return;
        }

        bool created = estate == null;
        estate ??= new Estate(zone, actor, actor.EstateHeir, EstateStatus.EstateWill);
        estate.RefreshWill();
        actor.OutputHandler.Send(
            $"You {(created ? "create" : "refresh")} will estate #{estate.Id.ToString("N0", actor)} for {zone.Name.ColourName()}.");
    }

    private static void EstateBequeath(ICharacter actor, StringStack ss)
    {
        if (!EnsureEstateCommandLocation(actor))
        {
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which estate will do you want to add a bequest to?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.PopSpeech());
        if (estate == null || estate.EstateStatus != EstateStatus.EstateWill)
        {
            actor.OutputHandler.Send("That is not an active will estate.");
            return;
        }

        if (estate.Character != actor && !CanManageEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to alter that will.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which asset from that will do you want to bequeath?");
            return;
        }

        IEstateAsset asset = GetEstateAssetById(estate, ss.PopSpeech());
        if (asset == null)
        {
            actor.OutputHandler.Send("That estate has no such asset.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to bequeath that asset to a character or a clan?");
            return;
        }

        IFrameworkItem beneficiary;
        switch (ss.PopForSwitch())
        {
            case "character":
            case "char":
                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Which character should receive that bequest?");
                    return;
                }

                beneficiary = GetCharacterByIdOrName(actor, ss.PopSpeech());
                if (beneficiary == null)
                {
                    actor.OutputHandler.Send("There is no such character.");
                    return;
                }

                break;
            case "clan":
                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Which clan should receive that bequest?");
                    return;
                }

                beneficiary = GetClanByIdOrName(actor, ss.PopSpeech());
                if (beneficiary == null)
                {
                    actor.OutputHandler.Send("There is no such clan.");
                    return;
                }

                break;
            default:
                actor.OutputHandler.Send("You must specify either character or clan as the beneficiary type.");
                return;
        }

        if (estate.Claims.Any(x =>
                x.Status == ClaimStatus.Approved &&
                x.TargetItem != null &&
                x.TargetItem.FrameworkItemEquals(asset.Asset.Id, asset.Asset.FrameworkItemType)))
        {
            actor.OutputHandler.Send("There is already an approved bequest for that asset.");
            return;
        }

        string reason = ss.IsFinished
            ? $"Bequest of {DescribeEstateAsset(actor, asset)}"
            : ss.SafeRemainingArgument;
        estate.AddClaim(new EstateClaim(estate, beneficiary, Math.Max(0.0M, asset.AssumedValue), reason,
            ClaimStatus.Approved, false, asset.Asset));
        actor.OutputHandler.Send(
            $"You set up a bequest of {DescribeEstateAsset(actor, asset)} from estate #{estate.Id.ToString("N0", actor)} to {DescribeFrameworkItem(actor, beneficiary)}.");
    }

    private static void EstateRevoke(ICharacter actor, StringStack ss)
    {
        if (!EnsureEstateCommandLocation(actor))
        {
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which will estate contains the bequest you want to revoke?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.PopSpeech());
        if (estate == null || estate.EstateStatus != EstateStatus.EstateWill)
        {
            actor.OutputHandler.Send("That is not an active will estate.");
            return;
        }

        if (estate.Character != actor && !CanManageEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to alter that will.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which claim do you want to revoke?");
            return;
        }

        if (!long.TryParse(ss.PopSpeech(), out long claimId))
        {
            actor.OutputHandler.Send("You must specify a numeric claim ID.");
            return;
        }

        IEstateClaim claim = estate.Claims.FirstOrDefault(x => x.Id == claimId);
        if (claim == null)
        {
            actor.OutputHandler.Send("That estate has no such claim.");
            return;
        }

        estate.RemoveClaim(claim);
        if (claim is EstateClaim estateClaim)
        {
            estateClaim.Delete();
        }

        actor.OutputHandler.Send(
            $"You revoke claim #{claim.Id.ToString("N0", actor)} from estate #{estate.Id.ToString("N0", actor)}.");
    }

    private static void EstateCollectPayout(ICharacter actor, StringStack ss)
    {
        if (!EnsureEstateCommandLocation(actor))
        {
            return;
        }

        IEconomicZone zone = CurrentProbateOfficeZone(actor);
        if (zone == null)
        {
            actor.OutputHandler.Send("You must be at a probate office to collect estate payouts.");
            return;
        }

        IEstate specificEstate = null;
        if (!ss.IsFinished)
        {
            specificEstate = GetEstateById(actor, ss.SafeRemainingArgument);
            if (specificEstate == null)
            {
                actor.OutputHandler.Send("There is no such estate.");
                return;
            }

            if (!actor.IsAdministrator() && specificEstate.EconomicZone != zone)
            {
                actor.OutputHandler.Send("You must visit that estate's probate office to collect its payouts.");
                return;
            }
        }

        List<IEstatePayout> payouts = actor.Gameworld.Estates
            .Where(x => specificEstate == null ? x.EconomicZone == zone : x.Id == specificEstate.Id)
            .SelectMany(x => x.Payouts)
            .Where(x => !x.IsCollected && x.Recipient == actor)
            .ToList();
        if (!payouts.Any())
        {
            actor.OutputHandler.Send(specificEstate == null
                ? "There are no estate payouts waiting for you at this probate office."
                : "That estate does not owe you any uncollected cash payouts.");
            return;
        }

        IEconomicZone payoutZone = specificEstate?.EconomicZone ?? zone;
        decimal total = payouts.Sum(x => x.Amount);
        MudDateTime now = payoutZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        foreach (IEstatePayout payout in payouts)
        {
            payout.CollectedDate = now;
        }

        IGameItem currencyItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(payoutZone.Currency,
            payoutZone.Currency.FindCoinsForAmount(total, out _));
        if (actor.Body.CanGet(currencyItem, 0))
        {
            actor.Body.Get(currencyItem, silent: true);
        }
        else
        {
            currencyItem.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(currencyItem, true);
            actor.OutputHandler.Send("You cannot carry the payout, so probate sets it down at your feet.");
        }

        actor.OutputHandler.Send(
            $"Probate pays out {payoutZone.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from {payouts.Count.ToString("N0", actor).ColourValue()} estate payout(s).");
    }

    private static void EstateList(ICharacter actor, StringStack ss)
    {
        if (!EnsureEstateCommandLocation(actor))
        {
            return;
        }

        List<IEstate> estates = actor.Gameworld.Estates.Where(x => CanViewEstate(actor, x)).ToList();
        if (!estates.Any())
        {
            actor.OutputHandler.Send("There are no estates available for you to review.");
            return;
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from estate in estates
            orderby estate.EstateStatus, estate.EconomicZone.Name, estate.Id
            select new List<string>
            {
                estate.Id.ToString("N0", actor),
                estate.Character != null
                    ? estate.Character.PersonalName.GetName(NameStyle.FullName)
                    : "Unknown",
                estate.EconomicZone.Name,
                estate.EstateStatus.DescribeEnum(),
                estate.Claims.Count().ToString("N0", actor),
                estate.Assets.Count().ToString("N0", actor),
                estate.Inheritor == null ? "Zone Default" : DescribeFrameworkItem(actor, estate.Inheritor)
            },
            new List<string>
            {
                "ID",
                "Deceased",
                "Zone",
                "Status",
                "Claims",
                "Assets",
                "Heir"
            },
            actor,
            Telnet.Yellow));
    }

    private static void EstateShow(ICharacter actor, StringStack ss)
    {
        if (!EnsureEstateCommandLocation(actor))
        {
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which estate do you want to inspect?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.SafeRemainingArgument);
        if (estate == null)
        {
            actor.OutputHandler.Send("There is no such estate.");
            return;
        }

        if (!CanViewEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to view that estate.");
            return;
        }

        StringBuilder sb = new();
        IAuctionHouse auctionHouse = estate.EconomicZone.EstateAuctionHouse;
        List<AuctionItem> activeLots = ActiveEstateAuctionLots(estate).ToList();
        List<UnclaimedAuctionItem> unclaimedLots = UnclaimedEstateAuctionLots(estate).ToList();
        List<AuctionResult> auctionResults = EstateAuctionResults(estate)
            .OrderByDescending(x => x.ResultDateTime)
            .ToList();
        sb.AppendLine($"Estate #{estate.Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
        sb.AppendLine(
            $"Deceased: {(estate.Character != null ? estate.Character.PersonalName.GetName(NameStyle.FullName).ColourName() : "Unknown".ColourError())}");
        sb.AppendLine($"Economic Zone: {estate.EconomicZone.Name.ColourName()}");
        sb.AppendLine($"Status: {DescribeEstateStatus(estate.EstateStatus)}");
        sb.AppendLine($"Inheritor: {(estate.Inheritor == null ? "Zone Default".ColourValue() : DescribeFrameworkItem(actor, estate.Inheritor))}");
        sb.AppendLine($"Estate Auction House: {(auctionHouse == null ? "None Configured".ColourError() : auctionHouse.Name.ColourName())}");
        sb.AppendLine($"Estate Started: {estate.EstateStartTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
        if (estate.FinalisationDate != null)
        {
            sb.AppendLine($"Finalisation Date: {estate.FinalisationDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
        }

        if (estate.EstateStatus == EstateStatus.Liquidating)
        {
            if (auctionHouse == null)
            {
                sb.AppendLine("Liquidation Error: No probate auction house is configured for this estate's economic zone."
                    .ColourError());
            }
            else
            {
                sb.AppendLine(
                    $"Liquidation Progress: {activeLots.Count.ToString("N0", actor).ColourValue()} active lots, {unclaimedLots.Count.ToString("N0", actor).ColourValue()} unclaimed lots, {auctionResults.Count.ToString("N0", actor).ColourValue()} completed results.");
            }
        }

        sb.AppendLine();
        sb.AppendLine("Assets:");
        sb.AppendLine(StringUtilities.GetTextTable(
            from asset in estate.Assets
            select new List<string>
            {
                asset.Id.ToString("N0", actor),
                asset.Asset.FrameworkItemType,
                DescribeEstateAsset(actor, asset),
                asset.IsPresumedOwnership.ToColouredString(),
                asset.IsTransferred.ToColouredString(),
                asset.IsLiquidated.ToColouredString(),
                estate.EconomicZone.Currency.Describe(
                    asset.IsLiquidated
                        ? asset.LiquidatedValue ?? 0.0M
                        : asset.AssumedValue,
                    CurrencyDescriptionPatternType.ShortDecimal)
            },
            new List<string>
            {
                "ID",
                "Type",
                "Asset",
                "Presumed",
                "Transferred",
                "Liquidated",
                "Value"
            },
            actor,
            Telnet.Green));

        if (activeLots.Any())
        {
            sb.AppendLine();
            sb.AppendLine("Active Liquidation Lots:");
            sb.AppendLine(StringUtilities.GetTextTable(
                from lot in activeLots
                select new List<string>
                {
                    lot.Asset.Id.ToString("N0", actor),
                    lot.LotType.DescribeEnum(),
                    DescribeAuctionLot(actor, lot),
                    estate.EconomicZone.Currency.Describe(lot.MinimumPrice, CurrencyDescriptionPatternType.ShortDecimal),
                    lot.BuyoutPrice.HasValue
                        ? estate.EconomicZone.Currency.Describe(lot.BuyoutPrice.Value,
                            CurrencyDescriptionPatternType.ShortDecimal)
                        : "",
                    auctionHouse.AuctionBids[lot].Count.ToString("N0", actor),
                    lot.FinishingDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)
                },
                new List<string>
                {
                    "Asset",
                    "Type",
                    "Lot",
                    "Reserve",
                    "Buyout",
                    "Bids",
                    "Ends"
                },
                actor,
                Telnet.Magenta));
        }

        if (unclaimedLots.Any())
        {
            sb.AppendLine();
            sb.AppendLine("Unclaimed Liquidation Lots:");
            sb.AppendLine(StringUtilities.GetTextTable(
                from lot in unclaimedLots
                select new List<string>
                {
                    lot.AuctionItem.Asset.Id.ToString("N0", actor),
                    lot.AuctionItem.LotType.DescribeEnum(),
                    DescribeAuctionLot(actor, lot.AuctionItem),
                    lot.WinningBid == null
                        ? "Unsold".ColourError()
                        : estate.EconomicZone.Currency.Describe(lot.WinningBid.Bid,
                            CurrencyDescriptionPatternType.ShortDecimal)
                },
                new List<string>
                {
                    "Asset",
                    "Type",
                    "Lot",
                    "Outcome"
                },
                actor,
                Telnet.Red));
        }

        sb.AppendLine();
        sb.AppendLine("Claims:");
        sb.AppendLine(StringUtilities.GetTextTable(
            from claim in estate.Claims
            orderby claim.IsSecured descending, claim.ClaimDate
            select new List<string>
            {
                claim.Id.ToString("N0", actor),
                DescribeFrameworkItem(actor, claim.Claimant),
                estate.EconomicZone.Currency.Describe(claim.Amount, CurrencyDescriptionPatternType.ShortDecimal),
                claim.Status.DescribeEnum(),
                claim.IsSecured.ToColouredString(),
                claim.TargetItem == null
                    ? ""
                    : estate.Assets.FirstOrDefault(x => x.Asset.FrameworkItemEquals(claim.TargetItem.Id, claim.TargetItem.FrameworkItemType)) is { } targetAsset
                        ? DescribeEstateAsset(actor, targetAsset)
                        : DescribeFrameworkItem(actor, claim.TargetItem),
                claim.Reason
            },
            new List<string>
            {
                "ID",
                "Claimant",
                "Amount",
                "Status",
                "Secured",
                "Target",
                "Reason"
            },
            actor,
            Telnet.Cyan));

        List<IEstatePayout> outstandingPayouts = estate.Payouts.Where(x => !x.IsCollected).ToList();
        if (outstandingPayouts.Any())
        {
            sb.AppendLine();
            sb.AppendLine("Outstanding Cash Payouts:");
            sb.AppendLine(StringUtilities.GetTextTable(
                from payout in outstandingPayouts
                select new List<string>
                {
                    payout.Id.ToString("N0", actor),
                    DescribeFrameworkItem(actor, payout.Recipient),
                    estate.EconomicZone.Currency.Describe(payout.Amount, CurrencyDescriptionPatternType.ShortDecimal),
                    payout.CreatedDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
                    payout.Reason
                },
                new List<string>
                {
                    "ID",
                    "Recipient",
                    "Amount",
                    "Created",
                    "Reason"
                },
                actor,
                Telnet.White));
        }

        if (auctionResults.Any())
        {
            sb.AppendLine();
            sb.AppendLine("Liquidation Results:");
            sb.AppendLine(StringUtilities.GetTextTable(
                from result in auctionResults
                select new List<string>
                {
                    result.AssetId.ToString("N0", actor),
                    result.AssetType,
                    result.AssetDescription,
                    result.Sold.ToColouredString(),
                    estate.EconomicZone.Currency.Describe(result.SalePrice,
                        CurrencyDescriptionPatternType.ShortDecimal),
                    result.ResultDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
                    result.PaidOutAtTime.ToColouredString()
                },
                new List<string>
                {
                    "Asset",
                    "Type",
                    "Description",
                    "Sold",
                    "Price",
                    "Date",
                    "Paid"
                },
                actor,
                Telnet.Yellow));
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void EstateClaim(ICharacter actor, StringStack ss)
    {
        if (!EnsureEstateCommandLocation(actor))
        {
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which estate do you want to make a claim against?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.PopSpeech());
        if (estate == null)
        {
            actor.OutputHandler.Send("There is no such estate.");
            return;
        }

        if (estate.EstateStatus != EstateStatus.ClaimPhase)
        {
            actor.OutputHandler.Send("That estate is not currently accepting claims.");
            return;
        }

        if (ss.PeekSpeech().EqualToAny("asset", "item", "property", "account", "bankaccount"))
        {
            ss.PopSpeech();
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("Which asset from that estate do you want to claim?");
                return;
            }

            IEstateAsset asset = GetEstateAssetById(estate, ss.PopSpeech());
            if (asset == null)
            {
                actor.OutputHandler.Send("That estate has no such asset.");
                return;
            }

            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("What is the reason for your claim?");
                return;
            }

            string assetReason = ss.SafeRemainingArgument;
            estate.AddClaim(new EstateClaim(estate, actor, Math.Max(0.0M, asset.AssumedValue), assetReason, ClaimStatus.NotAssessed, false,
                asset.Asset));
            actor.OutputHandler.Send(
                $"You submit a claim against asset #{asset.Id.ToString("N0", actor)} on estate #{estate.Id.ToString("N0", actor)}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"How much is your claim? Amounts must be in {estate.EconomicZone.Currency.Name.ColourName()}.");
            return;
        }

        if (!estate.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out decimal amount) || amount <= 0.0M)
        {
            actor.OutputHandler.Send("That is not a valid positive claim amount.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What is the reason for your claim?");
            return;
        }

        string reason = ss.SafeRemainingArgument;
        estate.AddClaim(new EstateClaim(estate, actor, amount, reason, ClaimStatus.NotAssessed, false));
        actor.OutputHandler.Send(
            $"You submit a claim against estate #{estate.Id.ToString("N0", actor)} for {estate.EconomicZone.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
    }

    private static void EstateAssess(ICharacter actor, StringStack ss, bool approve)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send($"Which estate contains the claim you want to {(approve ? "approve" : "reject")}?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.PopSpeech());
        if (estate == null)
        {
            actor.OutputHandler.Send("There is no such estate.");
            return;
        }

        if (!EnsureEstateCommandLocation(actor, estate))
        {
            return;
        }

        if (!CanManageEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to adjudicate claims for that estate.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send($"Which claim do you want to {(approve ? "approve" : "reject")}?");
            return;
        }

        if (!long.TryParse(ss.PopSpeech(), out long claimId))
        {
            actor.OutputHandler.Send("You must specify a numeric claim ID.");
            return;
        }

        IEstateClaim claim = estate.Claims.FirstOrDefault(x => x.Id == claimId);
        if (claim == null)
        {
            actor.OutputHandler.Send("That estate has no such claim.");
            return;
        }

        if (approve && claim.TargetItem != null)
        {
            IEstateAsset targetAsset = estate.Assets.FirstOrDefault(x =>
                x.Asset.FrameworkItemEquals(claim.TargetItem.Id, claim.TargetItem.FrameworkItemType));
            if (targetAsset == null)
            {
                actor.OutputHandler.Send("That claim targets an asset that is no longer part of the estate.");
                return;
            }

            if (targetAsset.IsTransferred || targetAsset.IsLiquidated)
            {
                actor.OutputHandler.Send("That claim targets an asset that has already been transferred or liquidated.");
                return;
            }

            if (estate.Claims.Any(x =>
                    x.Id != claim.Id &&
                    x.Status == ClaimStatus.Approved &&
                    x.TargetItem != null &&
                    x.TargetItem.FrameworkItemEquals(claim.TargetItem.Id, claim.TargetItem.FrameworkItemType)))
            {
                actor.OutputHandler.Send("There is already an approved claim against that specific estate asset.");
                return;
            }
        }

        claim.Status = approve ? ClaimStatus.Approved : ClaimStatus.Rejected;
        claim.StatusReason = ss.IsFinished ? null : ss.SafeRemainingArgument;
        estate.UpdateClaim(claim);
        actor.OutputHandler.Send(
            $"You {(approve ? "approve" : "reject")} claim #{claim.Id.ToString("N0", actor)} on estate #{estate.Id.ToString("N0", actor)}.");
    }

    private static void EstateAuctionAsset(ICharacter actor, StringStack ss, bool relist)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send($"Which estate do you want to {(relist ? "relist an asset from" : "list an asset from")}?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.PopSpeech());
        if (estate == null)
        {
            actor.OutputHandler.Send("There is no such estate.");
            return;
        }

        if (!EnsureEstateCommandLocation(actor, estate))
        {
            return;
        }

        if (!CanManageEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to manage liquidation for that estate.");
            return;
        }

        if (estate.EstateStatus != EstateStatus.Liquidating)
        {
            actor.OutputHandler.Send("That estate is not currently in the liquidation phase.");
            return;
        }

        IAuctionHouse auctionHouse = estate.EconomicZone.EstateAuctionHouse;
        if (auctionHouse == null)
        {
            actor.OutputHandler.Send(
                $"{estate.EconomicZone.Name.ColourName()} does not have a probate auction house configured.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which estate asset do you want to list?");
            return;
        }

        IEstateAsset asset = GetEstateAssetById(estate, ss.PopSpeech());
        if (asset == null)
        {
            actor.OutputHandler.Send("That estate has no such asset.");
            return;
        }

        if (asset.Asset is not IGameItem && asset.Asset is not IProperty)
        {
            actor.OutputHandler.Send("Only items and properties can be listed on an auction house.");
            return;
        }

        if (asset.Asset is IProperty && asset.OwnershipShare < 1.0M)
        {
            actor.OutputHandler.Send("Partial ownership shares in properties are inherited in kind and cannot be liquidated by the estate.");
            return;
        }

        if (asset.IsTransferred || asset.IsLiquidated)
        {
            actor.OutputHandler.Send("That asset has already been transferred or liquidated.");
            return;
        }

        bool hasActiveListing = ActiveEstateAuctionLots(estate).Any(x =>
            x.Asset.FrameworkItemEquals(asset.Asset.Id, asset.Asset.FrameworkItemType)) ||
                               UnclaimedEstateAuctionLots(estate).Any(x =>
                                   x.AuctionItem.Asset.FrameworkItemEquals(asset.Asset.Id,
                                       asset.Asset.FrameworkItemType));
        if (hasActiveListing)
        {
            actor.OutputHandler.Send("That asset is already tied up in an active or unclaimed liquidation lot.");
            return;
        }

        if (relist)
        {
            bool priorUnsoldResult = EstateAuctionResults(estate).Any(x =>
                x.AssetId == asset.Asset.Id &&
                x.AssetType.EqualTo(asset.Asset.FrameworkItemType) &&
                !x.Sold);
            if (!priorUnsoldResult)
            {
                actor.OutputHandler.Send("That asset does not have a prior unsold liquidation result to relist.");
                return;
            }
        }

        if (!TryGetEstateAuctionPrices(actor, estate, asset, ss, out decimal reservePrice, out decimal? buyoutPrice))
        {
            if (ss.IsFinished && reservePrice <= 0.0M)
            {
                actor.OutputHandler.Send("That asset does not have a suitable default reserve price; specify one explicitly.");
            }

            return;
        }

        if (!estate.TryCreateAuctionListing(auctionHouse, asset, reservePrice, buyoutPrice))
        {
            actor.OutputHandler.Send("You could not create a liquidation listing for that estate asset.");
            return;
        }

        actor.OutputHandler.Send(
            $"You {(relist ? "relist" : "list")} {DescribeEstateAsset(actor, asset)} on {auctionHouse.Name.ColourName()} with a reserve of {estate.EconomicZone.Currency.Describe(reservePrice, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}{(buyoutPrice.HasValue ? $" and a buyout of {estate.EconomicZone.Currency.Describe(buyoutPrice.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}" : "")}.");
    }

    private static void EstateLiquidate(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which estate do you want to move into liquidation?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.SafeRemainingArgument);
        if (estate == null)
        {
            actor.OutputHandler.Send("There is no such estate.");
            return;
        }

        if (!EnsureEstateCommandLocation(actor, estate))
        {
            return;
        }

        if (!CanManageEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to manage liquidation for that estate.");
            return;
        }

        if (estate.EstateStatus == EstateStatus.Finalised || estate.EstateStatus == EstateStatus.Cancelled)
        {
            actor.OutputHandler.Send("That estate has already reached a terminal status.");
            return;
        }

        if (estate.EstateStatus == EstateStatus.Liquidating)
        {
            actor.OutputHandler.Send("That estate is already in the liquidation phase.");
            return;
        }

        if (!estate.StartLiquidation())
        {
            actor.OutputHandler.Send("You could not move that estate into liquidation.");
            return;
        }

        actor.OutputHandler.Send(estate.EconomicZone.EstateAuctionHouse == null
            ? $"Estate #{estate.Id.ToString("N0", actor)} is now in liquidation, but {estate.EconomicZone.Name.ColourName()} has no probate auction house configured."
            : $"Estate #{estate.Id.ToString("N0", actor)} is now in liquidation.");
    }

    private static void EstateFinalise(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which estate do you want to finalise?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.SafeRemainingArgument);
        if (estate == null)
        {
            actor.OutputHandler.Send("There is no such estate.");
            return;
        }

        if (!EnsureEstateCommandLocation(actor, estate))
        {
            return;
        }

        if (!CanManageEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to finalise that estate.");
            return;
        }

        if (estate.EstateStatus == EstateStatus.Finalised || estate.EstateStatus == EstateStatus.Cancelled)
        {
            actor.OutputHandler.Send("That estate has already reached a terminal status.");
            return;
        }

        EstateStatus originalStatus = estate.EstateStatus;
        estate.Finalise();
        if (estate.EstateStatus == EstateStatus.Finalised)
        {
            actor.OutputHandler.Send($"You finalise estate #{estate.Id.ToString("N0", actor)}.");
            return;
        }

        if (estate.EstateStatus == EstateStatus.Liquidating && originalStatus != EstateStatus.Liquidating)
        {
            actor.OutputHandler.Send(estate.EconomicZone.EstateAuctionHouse == null
                ? $"Estate #{estate.Id.ToString("N0", actor)} has entered liquidation, but {estate.EconomicZone.Name.ColourName()} has no probate auction house configured."
                : $"Estate #{estate.Id.ToString("N0", actor)} has entered liquidation and is awaiting auction completion.");
            return;
        }

        if (estate.EstateStatus == EstateStatus.Liquidating)
        {
            actor.OutputHandler.Send(estate.EconomicZone.EstateAuctionHouse == null
                ? $"Estate #{estate.Id.ToString("N0", actor)} cannot finalise because {estate.EconomicZone.Name.ColourName()} has no probate auction house configured."
                : $"Estate #{estate.Id.ToString("N0", actor)} cannot finalise yet because liquidation lots are still pending.");
            return;
        }

        actor.OutputHandler.Send(
            $"Estate #{estate.Id.ToString("N0", actor)} did not change status from {DescribeEstateStatus(originalStatus)}.");
    }

    private static void EstateOpen(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which estate do you want to open for probate?");
            return;
        }

        IEstate estate = GetEstateById(actor, ss.SafeRemainingArgument);
        if (estate == null)
        {
            actor.OutputHandler.Send("There is no such estate.");
            return;
        }

        if (!EnsureEstateCommandLocation(actor, estate))
        {
            return;
        }

        if (!CanManageEstate(actor, estate))
        {
            actor.OutputHandler.Send("You are not authorised to open probate for that estate.");
            return;
        }

        if (estate.OpenProbate())
        {
            actor.OutputHandler.Send(
                $"You open probate for estate #{estate.Id.ToString("N0", actor)}. Claims will remain open until {estate.FinalisationDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
            return;
        }

        actor.OutputHandler.Send(
            $"Estate #{estate.Id.ToString("N0", actor)} is already in {DescribeEstateStatus(estate.EstateStatus)}.");
    }

    #endregion

    public const string MorgueHelp = @"The morgue command is used to review and release corpses and claimed belongings from a morgue office.

The syntax for this command is as follows:

	#3morgue list#0 - lists corpses and reclaimable belongings held by this morgue
	#3morgue claim corpse <which>#0 - releases a corpse from morgue storage to this office
	#3morgue claim item <which>#0 - releases one of your owned items from a belongings bundle";

    [PlayerCommand("Morgue", "morgue")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [HelpInfo("morgue", MorgueHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Morgue(ICharacter actor, string command)
    {
        if (!EnsureMorgueOffice(actor, out IEconomicZone zone))
        {
            return;
        }

        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "list":
                MorgueList(actor, zone);
                return;
            case "claim":
                MorgueClaim(actor, zone, ss);
                return;
            default:
                actor.OutputHandler.Send(MorgueHelp.SubstituteANSIColour());
                return;
        }
    }

    private static void MorgueList(ICharacter actor, IEconomicZone zone)
    {
        ICell storage = zone.MorgueStorageCell;
        if (storage == null)
        {
            actor.OutputHandler.Send("This morgue does not have a storage room configured.");
            return;
        }

        List<IGameItem> corpses = storage.GameItems
            .Where(x => x.EffectsOfType<MorgueStoredCorpse>().Any(y => y.EconomicZoneId == zone.Id))
            .ToList();
        List<IGameItem> reclaimableItems = storage.GameItems
            .Where(x => x.EffectsOfType<MorgueBelongings>().Any(y => y.EconomicZoneId == zone.Id))
            .SelectMany(x => x.GetItemType<IContainer>()?.Contents ?? Enumerable.Empty<IGameItem>())
            .Where(x => x.HasOwner && x.Owner == actor)
            .ToList();

        StringBuilder sb = new();
        sb.AppendLine($"Morgue Office for {zone.Name.ColourName()}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
        sb.AppendLine("Stored Corpses:");
        if (!corpses.Any())
        {
            sb.AppendLine("\tNone");
        }
        else
        {
            sb.AppendLine(StringUtilities.GetTextTable(
                from corpse in corpses
                let effect = corpse.EffectsOfType<MorgueStoredCorpse>().First(x => x.EconomicZoneId == zone.Id)
                let deceased = actor.Gameworld.TryGetCharacter(effect.CharacterOwnerId, true)
                select new List<string>
                {
                    corpse.Id.ToString("N0", actor),
                    deceased?.PersonalName.GetName(NameStyle.FullName) ?? "Unknown",
                    corpse.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings),
                    effect.EstateId > 0 ? effect.EstateId.ToString("N0", actor) : ""
                },
                new List<string> { "Item", "Deceased", "Corpse", "Estate" },
                actor,
                Telnet.Cyan));
        }

        sb.AppendLine();
        sb.AppendLine("Reclaimable Owned Items:");
        if (!reclaimableItems.Any())
        {
            sb.AppendLine("\tNone");
        }
        else
        {
            sb.AppendLine(StringUtilities.GetTextTable(
                from item in reclaimableItems
                select new List<string>
                {
                    item.Id.ToString("N0", actor),
                    item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings),
                    item.Owner?.Name ?? ""
                },
                new List<string> { "Item", "Description", "Owner" },
                actor,
                Telnet.Green));
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void MorgueClaim(ICharacter actor, IEconomicZone zone, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to claim a corpse or an item?");
            return;
        }

        switch (ss.PopForSwitch())
        {
            case "corpse":
                MorgueClaimCorpse(actor, zone, ss);
                return;
            case "item":
                MorgueClaimItem(actor, zone, ss);
                return;
            default:
                actor.OutputHandler.Send("You must specify either CORPSE or ITEM.");
                return;
        }
    }

    private static void MorgueClaimCorpse(ICharacter actor, IEconomicZone zone, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which corpse do you want to claim?");
            return;
        }

        List<IGameItem> corpses = zone.MorgueStorageCell?.GameItems
            .Where(x => x.EffectsOfType<MorgueStoredCorpse>().Any(y => y.EconomicZoneId == zone.Id))
            .ToList() ?? new List<IGameItem>();
        IGameItem corpse = long.TryParse(ss.SafeRemainingArgument, out long value)
            ? corpses.FirstOrDefault(x => x.Id == value)
            : corpses.GetFromItemListByKeyword(ss.SafeRemainingArgument, actor);
        if (corpse == null)
        {
            actor.OutputHandler.Send("There is no such corpse in this morgue.");
            return;
        }

        MorgueStoredCorpse effect = corpse.EffectsOfType<MorgueStoredCorpse>().First(x => x.EconomicZoneId == zone.Id);
        if (!CanClaimMorgueCorpse(actor, zone, effect))
        {
            actor.OutputHandler.Send("You are not authorised to claim that corpse.");
            return;
        }

        corpse.RemoveEffect(effect, true);
        zone.MorgueStorageCell.Extract(corpse);
        corpse.RoomLayer = actor.RoomLayer;
        actor.Location.Insert(corpse, true);
        actor.OutputHandler.Send(
            $"{corpse.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings).ColourName()} has been released from storage into this office.");
    }

    private static void MorgueClaimItem(ICharacter actor, IEconomicZone zone, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which item do you want to claim?");
            return;
        }

        List<IGameItem> bundles = zone.MorgueStorageCell?.GameItems
            .Where(x => x.EffectsOfType<MorgueBelongings>().Any(y => y.EconomicZoneId == zone.Id))
            .ToList() ?? new List<IGameItem>();
        List<IGameItem> items = bundles
            .SelectMany(x => x.GetItemType<IContainer>()?.Contents ?? Enumerable.Empty<IGameItem>())
            .Where(x => x.HasOwner && x.Owner == actor)
            .ToList();
        IGameItem item = long.TryParse(ss.SafeRemainingArgument, out long value)
            ? items.FirstOrDefault(x => x.Id == value)
            : items.GetFromItemListByKeyword(ss.SafeRemainingArgument, actor);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such reclaimable item in this morgue.");
            return;
        }

        IGameItem bundle = item.ContainedIn;
        item.ContainedIn?.Take(item);
        item.RoomLayer = actor.RoomLayer;
        actor.Location.Insert(item, true);
        if (bundle?.GetItemType<IContainer>()?.Contents.Any() == false)
        {
            bundle.Delete();
        }

        actor.OutputHandler.Send(
            $"{item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings).ColourName()} has been released into this office.");
    }

    #region Jobs

    [PlayerCommand("Jobs", "jobs")]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("jobs", @"The jobs command allows you to see three bits of information:

	1) Which jobs you currently hold (or that you no longer hold but still owe you money)
	2) Which jobs you or your clans have listed
	3) Which jobs are hiring

Of these 3, only the 1st one can be done from anywhere. The other 2 items need to be done from a location flagged as a 'job market' location.

You should also see the JOB command for ways to interact with these jobs.", AutoHelp.HelpArg)]
    protected static void Jobs(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        StringBuilder sb = new();

        if (actor.ActiveJobs.Any())
        {
            sb.AppendLine("You are currently employed in the following jobs:");
            sb.AppendLine();
            sb.AppendLine(StringUtilities.GetTextTable(
                from job in actor.ActiveJobs
                select new List<string>
                {
                    job.Id.ToString("N0", actor),
                    job.Name,
                    job.JobCommenced.Date.Display(CalendarDisplayMode.Short),
                    job.JobDueToEnd?.Date.Display(CalendarDisplayMode.Short) ?? "",
                    job.Listing is IOngoingJobListing ojl
                        ? ojl.PayReference.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)
                        : "",
                    job.RevenueEarned
                       .Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
                       .DefaultIfEmpty("None").ListToString(),
                    job.BackpayOwed
                       .Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
                       .DefaultIfEmpty("None").ListToString(),
                    job.Listing.Employer is ICharacter ch
                        ? ch.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Magenta)
                        : ((IClan)job.Listing.Employer).FullName.ColourName()
                },
                new List<string>
                {
                    "Id",
                    "Name",
                    "Commenced",
                    "Ends",
                    "Payday",
                    "Earned",
                    "Owed",
                    "Employer"
                },
                actor,
                Telnet.Green
            ));

            double load = actor.ActiveJobs.Sum(x => x.FullTimeEquivalentRatio);
            sb.AppendLine(
                $"You are currently working at {load.ToString("P0", actor).ColourValue()} of a full time work load.");
        }

        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        if (ez is null)
        {
            sb.AppendLine($"\nYou are not at a location where jobs can be posted or accepted.");
            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        List<IJobListing> ownJobs = actor.Gameworld.JobListings.Where(x => x.EconomicZone == ez && x.IsAuthorisedToEdit(actor))
                           .ToList();
        if (ownJobs.Any())
        {
            sb.AppendLine($"You have the following job listings in the {ez.Name.ColourName()} economic zone:");
            sb.AppendLine();
            sb.AppendLine(StringUtilities.GetTextTable(
                from job in ownJobs
                select new List<string>
                {
                    job.Id.ToString("N0", actor),
                    job.Name,
                    job.ActiveJobs.Count(x => !x.IsJobComplete).ToString("N0", actor),
                    job.NetFinancialPosition.Select(x =>
                           x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
                       .ListToString(),
                    double.IsInfinity(job.DaysOfSolvency) ? "Forever" : job.DaysOfSolvency.ToString("N1", actor),
                    job.Employer is ICharacter ch
                        ? ch.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Magenta)
                        : ((IClan)job.Employer).FullName.ColourName()
                },
                new List<string>
                {
                    "Id",
                    "Name",
                    "Employees",
                    "Net Position",
                    "Days Solvent",
                    "Employer"
                },
                actor,
                Telnet.Green
            ));
        }

        // TODO - filters
        List<IJobListing> jobs = actor.Gameworld.JobListings
                        .Where(x => !x.IsArchived && x.IsReadyToBePosted && x.EconomicZone == ez)
                        .OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
                        .ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
                        .ThenBy(x => x.Name)
                        .ToList();
        if (jobs.Any())
        {
            sb.AppendLine($"There are the following job listings locally:");
            sb.AppendLine();
            sb.AppendLine(StringUtilities.GetTextTable(
                from job in jobs
                select new List<string>
                {
                    job.Id.ToString("N0", actor),
                    job.Name,
                    job.Employer is ICharacter ch
                        ? ch.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Magenta)
                        : ((IClan)job.Employer).FullName.ColourName(),
                    job.PayDescriptionForJobListing(),
                    job.IsEligibleForJob(actor).Truth.ToString(),
                    job.MaximumNumberOfSimultaneousEmployees == 0
                        ? "Unlimited"
                        : (job.MaximumNumberOfSimultaneousEmployees - job.ActiveJobs.Count(x => !x.IsJobComplete))
                        .ToString("N0", actor)
                },
                new List<string>
                {
                    "Id",
                    "Name",
                    "Employer",
                    "Pay",
                    "Eligible?",
                    "# Positions"
                },
                actor,
                Telnet.Green
            ));
        }
        else
        {
            sb.AppendLine($"There are currently no jobs listed locally.");
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private const string JobHelp =
        @"The job command is used to interact with job listings and jobs that you hold. Most of these commands need to be done from a location that is flagged as a job market in the area's economic zone.

You should see the closely related JOBS command for a list of jobs that you manage or that are posted in your area.

The following basic syntaxes are used to interact with jobs:

	#3job preview <job>#0 - shows details about a posted job you're interested in
	#3job apply <job>#0 - applies for a job
	#3job quit <job>#0 - quits your job #2[works anywhere]#0
	#3job payday#0 - collects all monies you've earned from jobs
	#3job bankpayday <bank>:<account>#0 - collects all money from jobs into a bank account #2[works anywhere]#0

If you're interested in posting a job rather than applying for one, you can use the following syntaxes:

	#3job show <your job>#0 - shows employer information about a job
	#3job edit [<ez>] <your job>#0 - begins to edit a job listing
	#3job close#0 - stops editing a job listing
	#3job edit#0 - an alias for doing #3job show#0 on the job you're currently editing
	#3job new [<ez>] <name>#0 - creates a new job with you as the employer
	#3job newclan <clan> [<ez>] <name>#0 - creates a new job with a clan as the employer

The following commands require you to be editing a job listing:

	#3job deposit <money>#0 - deposits money into the coffers of the job to pay for payroll
	#3job withdraw <money>#0 - withdraws money from the coffers of the job
	#3job employees#0 - lists all employees currently working on this job
	#3job fire <who>#0 - fires an employee from the job
	#3job ready#0 - toggles the readiness of this posting for the job market
	#3job finish#0 - ends the job and pays out all the employees
	#3job set name <name>#0 - renames this job listing
	#3job set desc#0 - drops you into an editor to write a description for this job
	#3job set ratio <verycasual|parttime|fulltime|overtime|punishing>#0 - sets the job effort ratio
	#3job set employees#0 - permits an unlimited number of simultaneous employees
	#3job set employees <##>#0 - sets the maximum number of simultaneous employees
	#3job set clan <name>#0 - sets a clan that employees get membership in
	#3job set clan none#0 - clears the clan from this job
	#3job set rank <name>#0 - sets a clan rank for employees
	#3job set rank none#0 - clears the rank from this job
	#3job set paygrade <name>#0 - sets a clan paygrade for employees
	#3job set paygrade none#0 - clears the paygrade from this job
	#3job set appointment <name>#0 - sets a clan appointment for employees
	#3job set appointment none#0 - clears the appointment from this job
	#3job set term <time>#0 - sets the maximum term employees can hold this job
	#3job set term#0 - clears the term limit from this job
	#3job set prog <prog>#0 - sets a prog that controls who can hold this job

Note: There may be additional properties that can be edited depending on the type of job.";

    [PlayerCommand("Job", "job")]
    [NoCombatCommand]
    [NoHideCommand]
    [HelpInfo("job", JobHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Job(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "bankpayday":
                JobPayday(actor, ss, true);
                return;
            case "quit":
                JobQuit(actor, ss);
                return;
            case "preview":
                JobPreview(actor, ss);
                return;
        }

        ss = ss.GetUndo();
        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        if (ez is null)
        {
            actor.OutputHandler.Send($"You are not at a location where jobs can be interacted with.");
            return;
        }


        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "edit":
                JobEdit(actor, ss);
                return;
            case "close":
                JobClose(actor, ss);
                return;
            case "new":
                JobNew(actor, ss);
                return;
            case "newclan":
                JobNewClan(actor, ss);
                return;
            case "set":
                JobSet(actor, ss);
                return;
            case "show":
                JobShow(actor, ss);
                return;
            case "apply":
                JobApply(actor, ss);
                return;
            case "employees":
                JobEmployees(actor, ss);
                return;
            case "fire":
                JobFire(actor, ss);
                return;
            case "deposit":
                JobDeposit(actor, ss);
                return;
            case "withdraw":
                JobWithdraw(actor, ss);
                return;
            case "payday":
                JobPayday(actor, ss, false);
                return;
            case "finish":
                JobFinish(actor, ss);
                return;
            case "ready":
                JobReady(actor, ss);
                return;
        }
    }

    private static void JobNewClan(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What clan do you want to create a job listing for?");
            return;
        }

        IClan clan = ClanModule.GetTargetClan(actor, ss.PopSpeech());
        if (clan is null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator()
                ? "There is no such clan"
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership membership = actor.ClanMemberships.FirstOrDefault(x => !x.IsArchivedMembership && x.Clan == clan);
        if (!actor.IsAdministrator() && (membership is null ||
                                         !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanJobs)))
        {
            actor.OutputHandler.Send(
                $"You do not have permission to manage job listings in the {clan.FullName.ColourName()} clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to your new job?");
            return;
        }

        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        if (ez is null)
        {
            string ezname = ss.PopSpeech();
            ez = actor.Gameworld.EconomicZones.GetByIdOrName(ezname);
        }

        if (ez is null)
        {
            actor.OutputHandler.Send($"You are not at a location where jobs can be posted.");
            return;
        }

        string name = ss.PopSpeech().TitleCase();
        OngoingJobListing job = new(actor.Gameworld, name, ez, clan, ez.Currency);
        actor.Gameworld.Add(job);
        actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
        actor.AddEffect(new BuilderEditingEffect<IJobListing>(actor) { EditingItem = job });
        actor.OutputHandler.Send(
            $"You create a new ongoing job called {name.ColourName()} with Id #{job.Id.ToString("N0", actor)}, which are you now editing.");
    }

    private static void JobReady(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any job listings.");
            return;
        }

        IJobListing job = effect.EditingItem;
        if (job.IsArchived)
        {
            actor.OutputHandler.Send(
                $"The {job.Name.ColourName()} job is already finished, so it cannot be ready for listing.");
            return;
        }

        job.IsReadyToBePosted = !job.IsReadyToBePosted;
        if (!job.IsReadyToBePosted && job.ActiveJobs.Any(x => !x.IsJobComplete))
        {
            actor.OutputHandler.Send(
                $"You withdraw the {job.Name.ColourName()} job from public advertisement to new applicants, but existing employees remain.");
        }
        else if (!job.IsReadyToBePosted)
        {
            actor.OutputHandler.Send($"You withdraw the {job.Name.ColourName()} job from public advertisement.");
        }
        else
        {
            actor.OutputHandler.Send(
                $"The {job.Name.ColourName()} job is now publicly listed and ready for applicants.");
        }
    }

    private static void JobFinish(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any job listings.");
            return;
        }

        IJobListing job = effect.EditingItem;
        if (job.IsArchived)
        {
            actor.OutputHandler.Send(
                $"The {job.Name.ColourName()} job is already finished. If you want to remove it from your list, you need to ensure that all employees are paid any backpay that they are owed.");
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure you want to finish the {job.Name.ColourName()} job? This action cannot be undone.\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                if (job.IsArchived)
                {
                    actor.OutputHandler.Send($"The {job.Name.ColourName()} job has already been finished.");
                    return;
                }

                actor.OutputHandler.Send($"You finish the {job.Name.ColourName()} job.");
                job.FinishJob();
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send($"You decide not to finish the {job.Name.ColourName()} job.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send($"You decide not to finish the {job.Name.ColourName()} job.");
            },
            Keywords = new List<string>
            {
                "finish",
                "job"
            },
            DescriptionString = "Finishing a job listing"
        }), TimeSpan.FromSeconds(120));
    }

    private static void JobPayday(ICharacter actor, StringStack ss, bool useBank)
    {
        if (!actor.ActiveJobs.Any())
        {
            actor.OutputHandler.Send("You don't have any jobs, so nobody is going to pay you anything.");
            return;
        }

        IBankAccount bankAccount = null;
        if (useBank)
        {
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("Which bank account do you want to deposit your pay into?");
                return;
            }

            (IBankAccount account, string error) = Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
            if (account is null)
            {
                actor.OutputHandler.Send(error);
                return;
            }

            if (!account.IsAuthorisedAccountUser(actor))
            {
                actor.OutputHandler.Send("You are not authorised to use that bank account.");
                return;
            }

            bankAccount = account;
        }

        DecimalCounter<ICurrency> owed = new();
        DecimalCounter<ICurrency> paid = new();
        foreach (IActiveJob job in actor.ActiveJobs)
        {
            owed.Add(job.BackpayOwed);
            paid.Add(job.RevenueEarned);
        }

        StringBuilder sb = new();
        if (paid.All(x => x.Value <= 0.0M))
        {
            sb.AppendLine($"None of your jobs have paid you anything yet.");
        }
        else
        {
            if (useBank)
            {
                foreach (KeyValuePair<ICurrency, decimal> item in paid.ToList())
                {
                    if (item.Key != bankAccount.Currency)
                    {
                        if (!bankAccount.Bank.ExchangeRates.ContainsKey((item.Key, bankAccount.Currency)))
                        {
                            sb.AppendLine(
                                $"One of your jobs pays you in {item.Key.Name.ColourValue()}, which can't be deposited into the bank account {bankAccount.AccountReference.ColourName()}. You must collect your pay in coin or specify a different bank account.");
                            return;
                        }

                        paid[bankAccount.Currency] +=
                            bankAccount.Bank.ExchangeRates[(item.Key, bankAccount.Currency)] * item.Value;
                        paid.Remove(item.Key);
                    }
                }

                foreach (KeyValuePair<ICurrency, decimal> item in paid)
                {
                    bankAccount.DepositFromTransaction(item.Value, "Deposit from payday");
                }

                sb.AppendLine(
                    $"You deposit {paid.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()} in pay into the bank account {bankAccount.AccountReference.ColourName()} from your jobs.");
            }
            else
            {
                sb.AppendLine(
                    $"You collect {paid.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()} in pay from your jobs.");
                foreach (KeyValuePair<ICurrency, decimal> item in paid)
                {
                    IGameItem pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(item.Key,
                        item.Key.FindCoinsForAmount(item.Value, out _));
                    if (actor.Body.CanGet(pile, 0))
                    {
                        actor.Body.Get(pile, 0, silent: true);
                    }
                    else
                    {
                        pile.RoomLayer = actor.RoomLayer;
                        actor.Location.Insert(pile, true);
                        pile.PositionTarget = actor;
                        sb.AppendLine($"You couldn't hold {pile.HowSeen(actor)}, so it is on the ground.");
                    }
                }
            }
        }

        foreach (IActiveJob job in actor.ActiveJobs)
        {
            if (job.RevenueEarned.Any(x => x.Value > 0.0M))
            {
                job.RevenueEarned.Clear();
                job.Changed = true;
            }

            DecimalCounter<ICurrency> jobOwed = job.BackpayOwed;
            if (jobOwed.Any())
            {
                string employer = job.Listing.Employer is ICharacter ch
                    ? ch.PersonalName.GetName(NameStyle.FullName).ColourCharacter()
                    : ((IClan)job.Listing.Employer).FullName.ColourName();
                sb.AppendLine(
                    $"You are owed {jobOwed.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()} by your employer {employer} from your {job.Name.ColourValue()} job.");
            }
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void JobWithdraw(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any job listings.");
            return;
        }

        if (actor.Currency is null)
        {
            actor.OutputHandler.Send(
                $"You have not set a currency. You must first CURRENCY SET <currency> before you can use this command.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("How much do you want to withdraw from the coffers of that job listing?");
            return;
        }

        IJobListing job = effect.EditingItem;
        if (!actor.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out decimal amount))
        {
            actor.OutputHandler.Send(
                $"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid amount of {actor.Currency.Name.ColourValue()}.");
            return;
        }

        if (job.MoneyPaidIn[actor.Currency] >= amount)
        {
            job.MoneyPaidIn[actor.Currency] -= amount;
            job.Changed = true;
            IGameItem pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(actor.Currency,
                actor.Currency.FindCoinsForAmount(amount, out _));
            actor.OutputHandler.Send(
                $"You withdraw {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from the coffers of job {job.Name.ColourName()}.");
            if (actor.Body.CanGet(pile, 0))
            {
                actor.Body.Get(pile, 0, silent: true);
            }
            else
            {
                pile.RoomLayer = actor.RoomLayer;
                actor.Location.Insert(pile, true);
                pile.PositionTarget = actor;
                actor.OutputHandler.Send($"You couldn't hold {pile.HowSeen(actor)}, so it is on the ground.");
            }

            return;
        }

        actor.OutputHandler.Send(
            $"There is not enough money in the coffers of {job.Name.ColourName()} for you to withdraw that much.");
    }

    private static void JobDeposit(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any job listings.");
            return;
        }

        if (actor.Currency is null)
        {
            actor.OutputHandler.Send(
                $"You have not set a currency. You must first CURRENCY SET <currency> before you can use this command.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("How much do you want to deposit into the coffers of that job listing?");
            return;
        }

        IJobListing job = effect.EditingItem;
        if (!actor.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out decimal amount))
        {
            actor.OutputHandler.Send(
                $"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid amount of {actor.Currency.Name.ColourValue()}.");
            return;
        }

        OtherCashPayment payment = new(actor.Currency, actor);
        decimal accessible = payment.AccessibleMoneyForPayment();
        if (accessible >= amount)
        {
            payment.TakePayment(amount);
            job.MoneyPaidIn[actor.Currency] += amount;
            job.Changed = true;
            actor.OutputHandler.Send(
                $"You deposit {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} into the coffers of the job {job.Name.ColourName()}.");
            return;
        }

        if (accessible == 0.0M)
        {
            actor.OutputHandler.Send(
                $"There is no money in the coffers of {job.Name.ColourName()} for the {actor.Currency.Name.ColourValue()} currency.");
            return;
        }

        actor.OutputHandler.Send(
            $"There is not enough money in the coffers of {job.Name.ColourName()} for you to withdraw {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.\nThe most you could withdraw is {actor.Currency.Describe(accessible, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
    }

    private static void JobFire(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any job listings.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Who do you want to fire from that job? You can view the currency employees with {"job employees".MXPSend("job employees")}.");
            return;
        }

        List<ICharacter> employees = effect.EditingItem.ActiveJobs.Where(x => !x.IsJobComplete).Select(x => x.Character)
                              .ToList();
        ICharacter employee = employees.GetFromItemListByKeywordIncludingNames(ss.SafeRemainingArgument, actor);
        if (employee is null)
        {
            actor.OutputHandler.Send(
                $"There is no one like that currency working for the {effect.EditingItem.Name.ColourName()} job.");
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure you want to fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}?\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                IActiveJob activeJob = effect.EditingItem.ActiveJobs.FirstOrDefault(x => x.Character == employee);
                if (activeJob is null || activeJob.IsJobComplete)
                {
                    actor.OutputHandler.Send(
                        $"You can't fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} because {employee.ApparentGender(actor).Subjective()} {(employee.ApparentGender(actor).UseThirdPersonVerbForms ? "are" : "is")} no longer an employee of the job {effect.EditingItem.Name.ColourName()}");
                    return;
                }

                activeJob.FireFromJob();
                actor.OutputHandler.Send(
                    $"You fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}.");
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}.");
            },
            DescriptionString = $"Firing {employee.PersonalName.GetName(NameStyle.FullName)} from their job",
            Keywords = new List<string> { "fire" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void JobEmployees(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any job listings.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Employees of the {effect.EditingItem.Name.ColourName()} job:");
        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from item in effect.EditingItem.ActiveJobs
            select new List<string>
            {
                item.Character.PersonalName.GetName(NameStyle.FullName),
                item.Character.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf),
                item.JobCommenced.Date.Display(CalendarDisplayMode.Short),
                item.JobDueToEnd?.Date.Display(CalendarDisplayMode.Short) ?? "",
                item.IsJobComplete.ToString(),
                item.BackpayOwed
                    .Where(x => x.Value >= 0.0M)
                    .Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
                    .DefaultIfEmpty("None")
                    .ListToString()
            },
            new List<string>
            {
                "Name",
                "Appearance",
                "Started",
                "Ending",
                "Complete?",
                "Owed Backpay"
            },
            actor,
            Telnet.Green
        ));
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void JobQuit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which of your jobs do you want to quit?");
            return;
        }

        IActiveJob job = actor.ActiveJobs.Where(x => !x.IsJobComplete).GetByIdOrName(ss.SafeRemainingArgument);
        if (job is null)
        {
            actor.OutputHandler.Send("You're not working any job like that.");
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure you want to quit your {job.Name.ColourName()} job?\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                IActiveJob activeJob = actor.ActiveJobs.FirstOrDefault(x => x.Listing == job.Listing);
                if (activeJob is null || activeJob.IsJobComplete)
                {
                    actor.OutputHandler.Send(
                        $"You can't quit your {job.Name.ColourName()} job because it's already complete.");
                    return;
                }

                actor.OutputHandler.Send($"You quit your {job.Name.ColourName()} job.");
                job.QuitJob();
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send($"You decide not to quit your {job.Name.ColourName()} job.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send($"You decide not to quit your {job.Name.ColourName()} job.");
            },
            DescriptionString = "Quitting your job",
            Keywords = new List<string> { "quit", "job" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void JobApply(ICharacter actor, StringStack ss)
    {
        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        List<IJobListing> jobs = actor.Gameworld.JobListings
                        .Where(x => !x.IsArchived && x.IsReadyToBePosted && x.EconomicZone == ez)
                        .OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
                        .ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
                        .ThenBy(x => x.Name)
                        .ToList();
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which job would you like to apply for? Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
            return;
        }

        IJobListing job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
        if (job is null)
        {
            actor.OutputHandler.Send(
                $"There is no job like that for you to apply for. Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
            return;
        }

        (bool truth, string error) = job.IsEligibleForJob(actor);
        if (!truth)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Are you sure that you want to apply for the {job.Name.ColourName()} job?");
        double effort = job.FullTimeEquivalentRatio +
                     actor.ActiveJobs.Where(x => !x.IsJobComplete).Sum(x => x.FullTimeEquivalentRatio);
        if (effort > 1.0)
        {
            sb.AppendLine(
                $"Warning: This job will put you over {1.0.ToString("P", actor)} equivalent of full time hours. This will impact your skill rolls due to overwork.");
        }

        sb.Append(Accept.StandardAcceptPhrasing);
        actor.OutputHandler.Send(sb.ToString());
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                if (job.IsArchived || !job.IsReadyToBePosted)
                {
                    actor.OutputHandler.Send($"Unfortunately, the {job.Name.ColourName()} job has been withdrawn.");
                    return;
                }

                if (job.ActiveJobs.Any(x => !x.IsJobComplete && x.Listing == job && x.Character == actor))
                {
                    actor.OutputHandler.Send("You already have that job.");
                    return;
                }

                (bool truth, string error) = job.IsEligibleForJob(actor);
                if (!truth)
                {
                    actor.OutputHandler.Send(error);
                    return;
                }

                job.ApplyForJob(actor);
                actor.OutputHandler.Send($"You apply for the {job.Name.ColourName()} job, and are now employed!");
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send($"You decide not to apply for the {job.Name.ColourName()} job.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send($"You decide not to apply for the {job.Name.ColourName()} job.");
            },
            DescriptionString = "Accepting a job application",
            Keywords = new List<string> { "job", "application" }
        }), TimeSpan.FromSeconds(120));
    }

    private static void JobPreview(ICharacter actor, StringStack ss)
    {
        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        List<IJobListing> jobs =
                ez is not null
                    ? actor.Gameworld.JobListings
                           .Where(x => !x.IsArchived && x.IsReadyToBePosted && x.EconomicZone == ez)
                           .OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
                           .ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
                           .ThenBy(x => x.Name)
                           .ToList()
                    : actor.ActiveJobs.Select(x => x.Listing)
                           .OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
                           .ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
                           .ThenBy(x => x.Name)
                           .ToList()
            ;

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which job would you like to preview? Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
            return;
        }

        IJobListing job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
        if (job is null)
        {
            actor.OutputHandler.Send(
                $"There is no job like that for you to preview. Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
            return;
        }

        actor.OutputHandler.Send(job.ShowToPlayer(actor));
    }

    private static void JobNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to your new job?");
            return;
        }

        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        if (ez is null)
        {
            string ezname = ss.PopSpeech();
            ez = actor.Gameworld.EconomicZones.GetByIdOrName(ezname);
        }

        if (ez is null)
        {
            actor.OutputHandler.Send($"You are not at a location where jobs can be posted.");
            return;
        }

        string name = ss.PopSpeech().TitleCase();
        OngoingJobListing job = new(actor.Gameworld, name, ez, actor, ez.Currency);
        actor.Gameworld.Add(job);
        actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
        actor.AddEffect(new BuilderEditingEffect<IJobListing>(actor) { EditingItem = job });
        actor.OutputHandler.Send(
            $"You create a new ongoing job called {name.ColourName()} with Id #{job.Id.ToString("N0", actor)}, which are you now editing.");
    }

    private static void JobSet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any job listings.");
            return;
        }

        effect.EditingItem.BuildingCommand(actor, ss);
    }

    private static void JobShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which job listing do you want to view?");
            return;
        }

        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        if (ez is null)
        {
            string ezname = ss.PopSpeech();
            ez = actor.Gameworld.EconomicZones.GetByIdOrName(ezname);
        }

        if (ez is null)
        {
            actor.OutputHandler.Send($"You are not at a location where jobs can be posted.");
            return;
        }

        List<IJobListing> jobs = actor.Gameworld.JobListings.Where(x => x.EconomicZone == ez && x.IsAuthorisedToEdit(actor))
                        .ToList();
        IJobListing job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
        if (job is null)
        {
            actor.OutputHandler.Send("There is no job listing that you have access to like that.");
            return;
        }

        actor.OutputHandler.Send(job.Show(actor));
    }

    private static void JobClose(ICharacter actor, StringStack ss)
    {
        actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
        actor.OutputHandler.Send($"You are no longer editing any job listings.");
    }

    private static void JobEdit(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IJobListing> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
        if (ss.IsFinished && effect is not null)
        {
            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which job listing would you like to edit?");
            return;
        }

        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
        if (ez is null)
        {
            actor.OutputHandler.Send($"You are not at a location where jobs can be posted.");
            return;
        }

        List<IJobListing> jobs = actor.Gameworld.JobListings.Where(x => x.EconomicZone == ez && x.IsAuthorisedToEdit(actor))
                        .ToList();
        IJobListing job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
        if (job is null)
        {
            actor.OutputHandler.Send("There is no job listing that you have access to like that.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
        actor.AddEffect(new BuilderEditingEffect<IJobListing>(actor) { EditingItem = job });
        actor.OutputHandler.Send(
            $"You open job listing #{job.Id.ToString("N0", actor)} ({job.Name.ColourName()}) for editing.");
    }

    #endregion

    #region Market Related Code

    #region Market Influence Templates
    public const string MarketInfluenceTemplateHelpText = @"This command is used to create and edit Market Influence Templates. These are templates for creating market influences which apply supply, demand, flat price or income changes in a market.

It is recommended that you use this command rather than creating market influences directly with #3MARKETINFLUENCE#0, but that is also an option.

The syntax for this command is as follows:

	#3mit list#0 - shows all market influence templates
	#3mit show <id>#0 - shows a particular market influence template
	#3mit edit <id>#0 - begins editing a market influence template
	#3mit edit#0 - an alias for #3mit show <editing id>#0
	#3mit close#0 - stops editing a market influence template
	#3mit clone <name>#0 - clones an existing template and then begins editing the clone
	#3mit new <name>#0 - creates a new market influence template
	#3mit set name <name>#0 - sets a new name
	#3mit set about#0 - drops you into an editor to write an about info for builders
	#3mit set desc#0 - drops you into an editor to write a description for players
	#3mit set know <prog>#0 - sets the prog that controls if players know about this
	#3mit set impact <category> <supply%> <demand%> [<price%>]#0 - adds or edits an impact for a category
	#3mit set remimpact <category>#0 - removes the impact for a category
	#3mit set popimpact <population> <additive%> <multiplier>#0 - adds or edits an income impact for a population
	#3mit set rempopimpact <population>#0 - removes the income impact for a population";

    [PlayerCommand("MarketInfluenceTemplate", "marketinfluencetemplate", "mit")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("MarketInfluenceTemplate", MarketInfluenceTemplateHelpText, AutoHelp.HelpArgOrNoArg)]
    protected static void MarketInfluenceTemplate(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "list":
                MarketInfluenceTemplateList(actor, ss);
                return;
            case "new":
            case "create":
                MarketInfluenceTemplateNew(actor, ss);
                return;
            case "clone":
                MarketInfluenceTemplateClone(actor, ss);
                return;
            case "set":
                MarketInfluenceTemplateSet(actor, ss);
                return;
            case "edit":
                MarketInfluenceTemplateEdit(actor, ss);
                return;
            case "close":
                MarketInfluenceTemplateClose(actor, ss);
                return;
            case "show":
            case "view":
                MarketInfluenceTemplateShow(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(MarketInfluenceTemplateHelpText.SubstituteANSIColour());
                return;
        }
    }

    private static void MarketInfluenceTemplateNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name would you like to give to this market influence template?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.MarketInfluenceTemplates.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send(
                $"There is already a market influence template called {name.ColourName()}. Names must be unique.");
            return;
        }

        MarketInfluenceTemplate template = new(actor.Gameworld, name);
        actor.Gameworld.Add(template);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketInfluenceTemplate>(actor) { EditingItem = template });
        actor.OutputHandler.Send($"You are create a new market influence template called {name.ColourValue()}, which you are now editing.");
    }

    private static void MarketInfluenceTemplateClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market influence template do you want to clone?");
            return;
        }

        IMarketInfluenceTemplate old = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.SafeRemainingArgument);
        if (old is null)
        {
            actor.OutputHandler.Send("There is no market influence template like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new market influence template?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.MarketInfluenceTemplates.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a market influence template called {name.ColourName()}. Names must be unique.");
            return;
        }

        IMarketInfluenceTemplate category = old.Clone(name);
        actor.Gameworld.Add(category);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketInfluenceTemplate>(actor) { EditingItem = category });
        actor.OutputHandler.Send($"You are clone market influence template {old.Name.ColourValue()} to a new market influence template called {category.Name.ColourName()}, which you are now editing.");
    }

    private static void MarketInfluenceTemplateSet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketInfluenceTemplate> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market influence templates.");
            return;
        }

        effect.EditingItem.BuildingCommand(actor, ss);
    }

    private static void MarketInfluenceTemplateEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketInfluenceTemplate> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market influence template would you like to edit?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketInfluenceTemplate template = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.SafeRemainingArgument);
        if (template is null)
        {
            actor.OutputHandler.Send("There is no market influence template like that.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketInfluenceTemplate>(actor) { EditingItem = template });
        actor.OutputHandler.Send($"You are now editing the {template.Name.ColourName()} market influence template.");
    }

    private static void MarketInfluenceTemplateClose(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketInfluenceTemplate> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market influence templates.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
        actor.OutputHandler.Send("You are no longer editing any market influence templates.");
    }

    private static void MarketInfluenceTemplateShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketInfluenceTemplate> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market influence template would you like to show?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketInfluenceTemplate category = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.SafeRemainingArgument);
        if (category is null)
        {
            actor.OutputHandler.Send("There is no market influence template like that.");
            return;
        }

        actor.OutputHandler.Send(category.Show(actor));
    }

    private static void MarketInfluenceTemplateList(ICharacter actor, StringStack ss)
    {
        List<IMarketInfluenceTemplate> templates = actor.Gameworld.MarketInfluenceTemplates.ToList();
        // TODO - filters
        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from item in templates
            select new List<string>
            {
                item.Id.ToString("N0", actor),
                item.Name,
                item.CharacterKnowsAboutInfluenceProg.MXPClickableFunctionName(),
                item.TemplateSummary,
                actor.Gameworld.MarketInfluences.Count(x => x.MarketInfluenceTemplate == item).ToString("N0", actor),
                actor.Gameworld.MarketInfluences.Count(x => x.MarketInfluenceTemplate == item && x.Applies(null, x.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)).ToString("N0", actor)
            },
            new List<string>
            {
                "Id",
                "Name",
                "Prog",
                "Summary",
                "# Infl",
                "# Active"
            },
            actor,
            Telnet.Cyan,
            3
        ));
    }
    #endregion

    #region Market Influences
    public const string MarketInfluenceHelpText = @"The Market Influence command is used to create and manage 

	#3mi list#0 - shows all market influences
	#3mi show <id>#0 - shows a particular market influence
	#3mi edit <id>#0 - begins editing a market influence
	#3mi edit#0 - an alias for #3mi show <editing id>#0
	#3mi close#0 - stops editing a market influence
	#3mi clone <name>#0 - clones an existing influence and then begins editing the clone
	#3mi new <market> <date> <name>#0 - creates a new market influence
	#3mi begin <market> <template> [<from>] [<until>]#0 - creates a new market influence from a template
	#3mi end <id>#0 - ends a market influence
	#3mi set name <name>#0 - sets a new name
	#3mi set desc#0 - drops you into an editor to write a description for players
	#3mi set know <prog>#0 - sets the prog that controls if players know about this
	#3mi set impact <category> <supply%> <demand%> [<price%>]#0 - adds or edits an impact for a category
	#3mi set remimpact <category>#0 - removes the impact for a category
	#3mi set popimpact <population> <additive%> <multiplier>#0 - adds or edits an income impact for a population
	#3mi set rempopimpact <population>#0 - removes the income impact for a population
	#3mi set applies <date>#0 - the date that this impact applies from
	#3mi set until <date>#0 - the date that this impact applies until
	#3mi set until always#0 - removes the expiry date for this impact
	#3mi set duration <timespan>#0 - an alternative way to set until based on duration";
    [PlayerCommand("MarketInfluence", "marketinfluence", "mi")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("MarketInfluence", MarketInfluenceHelpText, AutoHelp.HelpArgOrNoArg)]
    protected static void MarketInfluence(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "list":
                MarketInfluenceList(actor, ss);
                return;
            case "new":
            case "create":
                MarketInfluenceNew(actor, ss);
                return;
            case "begin":
                MarketInfluenceBegin(actor, ss);
                return;
            case "end":
                MarketInfluenceEnd(actor, ss);
                return;
            case "clone":
                MarketInfluenceClone(actor, ss);
                return;
            case "set":
                MarketInfluenceSet(actor, ss);
                return;
            case "edit":
                MarketInfluenceEdit(actor, ss);
                return;
            case "close":
                MarketInfluenceClose(actor, ss);
                return;
            case "show":
            case "view":
                MarketInfluenceShow(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(MarketInfluenceHelpText.SubstituteANSIColour());
                return;
        }
    }

    private static void MarketInfluenceEnd(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market influence do you want end?");
            return;
        }

        IMarketInfluence influence = actor.Gameworld.MarketInfluences.GetById(ss.SafeRemainingArgument);
        if (influence is null)
        {
            actor.OutputHandler.Send("There is no such market influence.");
            return;
        }

        if (influence.AppliesUntil is not null && influence.AppliesUntil <= influence.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
        {
            actor.OutputHandler.Send("That influence has already ended.");
            return;
        }

        StringBuilder sb = new();
        sb.Append("Are you sure that you want to end market influence #");
        sb.Append(influence.Id.ToString("N0", actor));
        sb.Append(" (");
        sb.Append(influence.Name.ColourValue());
        sb.AppendLine(")?");
        sb.Append("This influence ");
        MudDateTime now = influence.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        if (influence.AppliesFrom > now)
        {
            sb.Append("has not yet begun".Colour(Telnet.Yellow));
        }
        else
        {
            sb.Append($"began at {influence.AppliesFrom}".Colour(Telnet.Green));
        }

        sb.Append(" and ");
        if (influence.AppliesUntil is null)
        {
            sb.Append("continues until cancelled".Colour(Telnet.Cyan));
        }
        else
        {
            if (influence.AppliesUntil > now)
            {
                sb.Append("currently applies".Colour(Telnet.Green));
            }
            else
            {
                sb.Append("has already ended".Colour(Telnet.Red));
            }
        }

        sb.AppendLine(".");
        sb.AppendLine(Accept.StandardAcceptPhrasing);
        actor.OutputHandler.Send(sb.ToString());
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            DescriptionString = "ending a market influence",
            AcceptAction = text =>
            {
                influence.EndOrCancel();
                actor.OutputHandler.Send("You end the market influence.");
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send("You decide not to end the market influence.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send("You decide not to end the market influence.");
            },
            Keywords = ["influence", "end"]
        }), TimeSpan.FromSeconds(120));
    }

    private static void MarketInfluenceBegin(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market do you want to begin an influence in?");
            return;
        }

        IMarket market = actor.Gameworld.Markets.GetByIdOrName(ss.PopSpeech());
        if (market is null)
        {
            actor.OutputHandler.Send("There is no such market.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which template do you want to use for the influence?");
            return;
        }

        IMarketInfluenceTemplate template = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.PopSpeech());
        if (template is null)
        {
            actor.OutputHandler.Send("There is no such market influence template.");
            return;
        }

        MudDateTime begin = market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        if (!ss.IsFinished)
        {
            if (!MudDateTime.TryParse(ss.PopSpeech(), market.EconomicZone.FinancialPeriodReferenceCalendar, market.EconomicZone.FinancialPeriodReferenceClock, actor, out begin, out string error))
            {
                actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid date and time.\n{error}");
                return;
            }
        }

        MudDateTime end = default;
        if (!ss.IsFinished)
        {
            if (!MudDateTime.TryParse(ss.PopSpeech(), market.EconomicZone.FinancialPeriodReferenceCalendar, market.EconomicZone.FinancialPeriodReferenceClock, actor, out end, out string error))
            {
                actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid date and time.\n{error}");
                return;
            }
        }

        MarketInfluence influence = new(market, template, template.Name, begin, end);
        actor.Gameworld.Add(influence);
        market.ApplyMarketInfluence(influence);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
        actor.OutputHandler.Send($"You are create a new market influence for the {market.Name.ColourName()} market from the template {template.Name.ColourValue()}, which you are now editing.");
    }

    private static void MarketInfluenceNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market do you want to create an influence for?");
            return;
        }

        IMarket market = actor.Gameworld.Markets.GetByIdOrName(ss.PopSpeech());
        if (market is null)
        {
            actor.OutputHandler.Send("There is no such market.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What date should this influence apply from?");
            return;
        }

        if (!MudDateTime.TryParse(ss.PopSpeech(), market.EconomicZone.FinancialPeriodReferenceCalendar, market.EconomicZone.FinancialPeriodReferenceClock, actor, out MudDateTime date, out string error))
        {
            actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid date and time.\n{error}");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name would you like to give to this market influence?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.MarketInfluenceTemplates.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send(
                $"There is already a market influence template called {name.ColourName()}. Names must be unique.");
            return;
        }

        MarketInfluence influence = new(market, name, "This influence has no detailed description", date, null);
        actor.Gameworld.Add(influence);
        market.ApplyMarketInfluence(influence);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
        actor.OutputHandler.Send($"You are create a new market influence for the {market.Name.ColourName()} market called {name.ColourValue()}, which you are now editing.");
    }

    private static void MarketInfluenceClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market influence do you want to clone?");
            return;
        }

        IMarketInfluence old = actor.Gameworld.MarketInfluences.GetByIdOrName(ss.SafeRemainingArgument);
        if (old is null)
        {
            actor.OutputHandler.Send("There is no market influence like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new market influence?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();

        IMarketInfluence influence = old.Clone(name);
        actor.Gameworld.Add(influence);
        influence.Market.ApplyMarketInfluence(influence);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
        actor.OutputHandler.Send($"You are clone market influence {old.Name.ColourValue()} to a new market influence called {influence.Name.ColourName()}, which you are now editing.");
    }

    private static void MarketInfluenceSet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketInfluence> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market influences.");
            return;
        }

        effect.EditingItem.BuildingCommand(actor, ss);
    }

    private static void MarketInfluenceEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketInfluence> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market influence would you like to edit?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketInfluence influence = actor.Gameworld.MarketInfluences.GetByIdOrName(ss.SafeRemainingArgument);
        if (influence is null)
        {
            actor.OutputHandler.Send("There is no market influence like that.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
        actor.OutputHandler.Send($"You are now editing the {influence.Name.ColourName()} market influence.");
    }

    private static void MarketInfluenceClose(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketInfluence> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market influences.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
        actor.OutputHandler.Send("You are no longer editing any market influences.");
    }

    private static void MarketInfluenceShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketInfluence> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market influence would you like to show?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketInfluence category = actor.Gameworld.MarketInfluences.GetByIdOrName(ss.SafeRemainingArgument);
        if (category is null)
        {
            actor.OutputHandler.Send("There is no market influences like that.");
            return;
        }

        actor.OutputHandler.Send(category.Show(actor));
    }

    private static void MarketInfluenceList(ICharacter actor, StringStack ss)
    {
        List<IMarketInfluence> influences = actor.Gameworld.MarketInfluences.ToList();
        // TODO - filters
        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from item in influences
            select new List<string>
            {
                item.Id.ToString("N0", actor),
                item.Name,
                item.CharacterKnowsAboutInfluenceProg.MXPClickableFunctionName(),
                item.Market.Name,
                item.MarketInfluenceTemplate?.Name ?? "",
                item.AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
                item.AppliesUntil?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short) ?? "Until Removed",
                item.Applies(null, item.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime).ToColouredString()
            },
            new List<string>
            {
                "Id",
                "Name",
                "Prog",
                "Market",
                "Template",
                "From",
                "Until",
                "Active"
            },
            actor,
            Telnet.Cyan,
            3
        ));
    }
    #endregion

    #region Market Categories
    public const string MarketCategoryHelpText = @"This command allows you to edit market categories. Market categories are groupings of goods or services that have the same price multipliers in a market. 

These categories can be broad or specific, for example, you could have ""Food"" as a category or separate ""Luxury Food"", ""Staple Foods"", etc.

Not every market needs to have every category, but categories themselves can be shared between different markets.

The syntax for working with categories is as follows:

	#3mc list#0 - shows all market categories
	#3mc show <id>#0 - shows a particular market category
	#3mc edit <id>#0 - begins editing a market category
	#3mc edit#0 - an alias for #3mc show <editing id>#0
	#3mc close#0 - stops editing a market category
	#3mc clone <name>#0 - clones an existing market category and then begins editing the clone
	#3mc new <tag> <name>#0 - creates a new market category with a specified default item tag
	#3mc set name <name>#0 - changes the name
	#3mc set eover <%>#0 - changes the elasticity for oversupply
	#3mc set eunder <%>#0 - changes the elasticity for undersupply
	#3mc set type <standalone|combination>#0 - sets the category type
	#3mc set component <category> <weight>#0 - adds or updates a combination component
	#3mc set remcomponent <category>#0 - removes a combination component
	#3mc set desc#0 - drops you into an editor to set the description
	#3mc set tag <tag>#0 - toggles an item tag as being a part of this category";

    [PlayerCommand("MarketCategory", "marketcategory", "mc")]
    [CommandPermission(PermissionLevel.Admin)]
    protected static void MarketCategory(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "list":
                MarketCategoryList(actor, ss);
                return;
            case "new":
            case "create":
                MarketCategoryNew(actor, ss);
                return;
            case "clone":
                MarketCategoryClone(actor, ss);
                return;
            case "set":
                MarketCategorySet(actor, ss);
                return;
            case "edit":
                MarketCategoryEdit(actor, ss);
                return;
            case "close":
                MarketCategoryClose(actor, ss);
                return;
            case "show":
            case "view":
                MarketCategoryShow(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(MarketCategoryHelpText.SubstituteANSIColour());
                return;
        }
    }

    private static void MarketCategoryNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What item tag would you like this market category to apply to?");
            return;
        }

        ITag tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
        if (tag is null)
        {
            actor.OutputHandler.Send("There is no such tag.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name would you like to give to this market category?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.MarketCategories.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send(
                $"There is already a market category called {name.ColourName()}. Names must be unique.");
            return;
        }

        MarketCategory category = new(actor.Gameworld, name, tag);
        actor.Gameworld.Add(category);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketCategory>(actor) { EditingItem = category });
        actor.OutputHandler.Send($"You are create a new market category called {name.ColourValue()} that applies to the tag {tag.FullName.ColourName()}, which you are now editing.");
    }

    private static void MarketCategoryClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market category do you want to clone?");
            return;
        }

        IMarketCategory old = actor.Gameworld.MarketCategories.GetByIdOrName(ss.SafeRemainingArgument);
        if (old is null)
        {
            actor.OutputHandler.Send("There is no market category like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new market category?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.MarketCategories.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a market category called {name.ColourName()}. Names must be unique.");
            return;
        }

        IMarketCategory category = old.Clone(name);
        actor.Gameworld.Add(category);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketCategory>(actor) { EditingItem = category });
        actor.OutputHandler.Send($"You are clone market category {old.Name.ColourValue()} to a new market category called {category.Name.ColourName()}, which you are now editing.");
    }

    private static void MarketCategorySet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketCategory> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market categories.");
            return;
        }

        effect.EditingItem.BuildingCommand(actor, ss);
    }

    private static void MarketCategoryEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketCategory> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market category would you like to edit?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketCategory category = actor.Gameworld.MarketCategories.GetByIdOrName(ss.SafeRemainingArgument);
        if (category is null)
        {
            actor.OutputHandler.Send("There is no market category like that.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketCategory>(actor) { EditingItem = category });
        actor.OutputHandler.Send($"You are now editing the {category.Name.ColourName()} market category.");
    }

    private static void MarketCategoryClose(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketCategory> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market categories.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
        actor.OutputHandler.Send("You are no longer editing any market categories.");
    }

    private static void MarketCategoryShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketCategory> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market category would you like to show?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketCategory category = actor.Gameworld.MarketCategories.GetByIdOrName(ss.SafeRemainingArgument);
        if (category is null)
        {
            actor.OutputHandler.Send("There is no market category like that.");
            return;
        }

        actor.OutputHandler.Send(category.Show(actor));
    }

    private static void MarketCategoryList(ICharacter actor, StringStack ss)
    {
        List<IMarketCategory> categories = actor.Gameworld.MarketCategories.ToList();
        // TODO - filters
        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from item in categories
            let prices = actor.Gameworld.Markets.Where(x => x.MarketCategories.Contains(item))
                              .Select(x => x.PriceMultiplierForCategory(item))
                              .DefaultIfEmpty(1.0M)
                              .ToList()
            select new List<string>
            {
                item.Id.ToString("N0", actor),
                item.Name,
                item.ElasticityFactorBelow.ToString("N3", actor),
                item.ElasticityFactorAbove.ToString("N3", actor),
                actor.Gameworld.Markets.Count(x => x.MarketCategories.Contains(item)).ToString("N0", actor),
                prices.Min().ToString("P2", actor),
                prices.Average().ToString("P2", actor),
                prices.Max().ToString("P2", actor)
            },
            new List<string>
            {
                "Id",
                "Name",
                "E(Under)",
                "E(Over)",
                "# Markets",
                "Min Price",
                "Avg Price",
                "Max Price"
            },
            actor,
            Telnet.Cyan
        ));
    }

    #endregion

    #region Markets
    public const string MarketHelpText = @"This command allows you to create and edit markets, which can be used to control prices of various goods in way that responds to supply and demand changes.

There are several related commands, #3marketcategory#0, #3marketinfluencetemplate#0 and #3markettemplate#0.

The syntax for this command is as follows:

	#3market list#0 - shows all markets
	#3market show <id>#0 - shows a particular market
	#3market edit <id>#0 - begins editing a market
	#3market edit#0 - an alias for #3market show <editing id>#0
	#3market close#0 - stops editing a market
	#3market clone <name>#0 - clones an existing market and then begins editing the clone
	#3market new <ez> <name>#0 - creates a new market in an economic zone
	#3market set name <name>#0 - changes the name
	#3market set ez <zone>#0 - changes the economic zone
	#3market set category <which>#0 - toggles a category as being part of the market
	#3market set desc#0 - drops you into an editor for the market's description
	#3market set formula <formula>#0 - edits the market's price formula";

    [PlayerCommand("Market", "market")]
    [CommandPermission(PermissionLevel.Admin)]
    protected static void Market(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "list":
                MarketList(actor, ss);
                return;
            case "new":
            case "create":
                MarketNew(actor, ss);
                return;
            case "clone":
                MarketClone(actor, ss);
                return;
            case "set":
                MarketSet(actor, ss);
                return;
            case "edit":
                MarketEdit(actor, ss);
                return;
            case "close":
                MarketClose(actor, ss);
                return;
            case "show":
            case "view":
                MarketShow(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(MarketHelpText.SubstituteANSIColour());
                return;
        }
    }

    private static void MarketClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market do you want to clone?");
            return;
        }

        IMarket old = actor.Gameworld.Markets.GetByIdOrName(ss.PopSpeech());
        if (old is null)
        {
            actor.OutputHandler.Send("There is no market like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new market?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.Markets.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a market called {name.ColourName()}. Names must be unique.");
            return;
        }

        IMarket market = old.Clone(name);
        actor.Gameworld.Add(market);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
        actor.AddEffect(new BuilderEditingEffect<IMarket>(actor) { EditingItem = market });
        actor.OutputHandler.Send($"You are clone market {old.Name.ColourValue()} to a new market called {market.Name.ColourName()}, which you are now editing.");
    }

    private static void MarketShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarket> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market would you like to show?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarket market = actor.Gameworld.Markets.GetByIdOrName(ss.SafeRemainingArgument);
        if (market is null)
        {
            actor.OutputHandler.Send("There is no market like that.");
            return;
        }

        actor.OutputHandler.Send(market.Show(actor));
    }

    private static void MarketList(ICharacter actor, StringStack ss)
    {
        List<IMarket> markets = actor.Gameworld.Markets.ToList();
        // TODO - filters
        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from item in markets
            select new List<string>
            {
                item.Id.ToString("N0", actor),
                item.Name,
                item.EconomicZone.Name,
            },
            new List<string>
            {
                "ID",
                "Name",
                "Economic Zone"
            },
            actor,
            Telnet.Yellow
        ));
    }

    private static void MarketNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which economic zone do you want to create a market in?");
            return;
        }

        IEconomicZone ez = actor.Gameworld.EconomicZones.GetByIdOrName(ss.PopSpeech());
        if (ez is null)
        {
            actor.OutputHandler.Send("There is no such economic zones.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new market?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.Markets.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a market called {name.ColourName()}. Names must be unique.");
            return;
        }

        Market market = new(actor.Gameworld, name, ez);
        actor.Gameworld.Add(market);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
        actor.AddEffect(new BuilderEditingEffect<IMarket>(actor) { EditingItem = market });
        actor.OutputHandler.Send($"You are create a new market in the {ez.Name.ColourValue()} economic zone called {market.Name.ColourName()}, which you are now editing.");
    }

    private static void MarketSet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarket> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any markets.");
            return;
        }

        effect.EditingItem.BuildingCommand(actor, ss);
    }

    private static void MarketEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarket> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market would you like to edit?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarket market = actor.Gameworld.Markets.GetByIdOrName(ss.SafeRemainingArgument);
        if (market is null)
        {
            actor.OutputHandler.Send("There is no market like that.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
        actor.AddEffect(new BuilderEditingEffect<IMarket>(actor) { EditingItem = market });
        actor.OutputHandler.Send($"You are now editing the {market.Name.ColourName()} market.");
    }

    private static void MarketClose(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarket> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any markets.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
        actor.OutputHandler.Send("You are no longer editing any markets.");
    }
    #endregion Markets

    #region Market Populations
    public const string MarketPopulationHelpText = @"This command allows you to create and edit markets, which can be used to control prices of various goods in way that responds to supply and demand changes.

There are several related commands, #3marketcategory#0, #3marketinfluencetemplate#0 and #3markettemplate#0.

The syntax for this command is as follows:

	#3market list [<market>]#0 - shows all market populations
	#3market show <id>#0 - shows a particular market population
	#3market edit <id>#0 - begins editing a market population
	#3market edit#0 - an alias for #3market show <editing id>#0
	#3market close#0 - stops editing a market population
	#3market clone <name>#0 - clones an existing market population and then begins editing the clone
	#3market new <market> <name>#0 - creates a new market population in a market
	#3market set name <name>#0 - renames this market population
	#3market set desc#0 - drops you into an editor to edit the description
	#3market set scale <number>#0 - sets the number of people represented by this pop
	#3market set income <factor>#0 - sets the base income factor for this population
	#3market set savings <cycles>#0 - sets current savings in budget-cycle multiples
	#3market set savingscap <cycles>#0 - sets the savings cap in budget-cycle multiples
	#3market set flicker <%>#0 - sets the stress hysteresis threshold for this population
	#3market set need <category> <money>#0 - sets or removes (with 0) the need to spend on a category
	#3market set stress add <threshold> <name> <onstart>|none <onend>|none#0 - creates a new population stress threshold
	#3market set stress <threshold> remove#0 - permanently removes a stress threshold
	#3market set stress <threshold> name <name>#0 - renames a stress threshold
	#3market set stress <threshold> desc#0 - drops into an editor to edit the threshold's description
	#3market set stress <threshold> onstart <prog>|none#0 - sets or clears an on-start prog for the threshold
	#3market set stress <threshold> onend <prog>|none#0 - sets or clears an on-end prog for the threshold";

    [PlayerCommand("MarketPopulation", "marketpopulation", "mp")]
    [CommandPermission(PermissionLevel.Admin)]
    protected static void MarketPopulation(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        switch (ss.PopForSwitch())
        {
            case "list":
                MarketPopulationList(actor, ss);
                return;
            case "new":
            case "create":
                MarketPopulationNew(actor, ss);
                return;
            case "clone":
                MarketPopulationClone(actor, ss);
                return;
            case "set":
                MarketPopulationSet(actor, ss);
                return;
            case "edit":
                MarketPopulationEdit(actor, ss);
                return;
            case "close":
                MarketPopulationClose(actor, ss);
                return;
            case "show":
            case "view":
                MarketPopulationShow(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(MarketPopulationHelpText.SubstituteANSIColour());
                return;
        }
    }

    private static void MarketPopulationClone(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market population do you want to clone?");
            return;
        }

        IMarketPopulation old = actor.Gameworld.MarketPopulations.GetByIdOrName(ss.PopSpeech());
        if (old is null)
        {
            actor.OutputHandler.Send("There is no market population like that.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new market population?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.MarketPopulations.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a market population called {name.ColourName()}. Names must be unique.");
            return;
        }

        IMarketPopulation population = old.Clone(name);
        actor.Gameworld.Add(population);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketPopulation>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketPopulation>(actor) { EditingItem = population });
        actor.OutputHandler.Send($"You are clone market population {old.Name.ColourValue()} to a new market population called {population.Name.ColourName()}, which you are now editing.");
    }

    private static void MarketPopulationShow(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketPopulation> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketPopulation>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market population would you like to show?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketPopulation market = actor.Gameworld.MarketPopulations.GetByIdOrName(ss.SafeRemainingArgument);
        if (market is null)
        {
            actor.OutputHandler.Send("There is no market population like that.");
            return;
        }

        actor.OutputHandler.Send(market.Show(actor));
    }

    private static void MarketPopulationList(ICharacter actor, StringStack ss)
    {
        List<IMarketPopulation> populations = actor.Gameworld.MarketPopulations.ToList();

        while (!ss.IsFinished)
        {
            IMarket market = actor.Gameworld.Markets.GetByIdOrName(ss.PopSpeech());
            if (market is null)
            {
                actor.OutputHandler.Send("There is no such market.");
                return;
            }

            populations = populations.Where(x => x.Market == market).ToList();
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            from item in populations
            select new List<string>
            {
                item.Id.ToString("N0", actor),
                item.Name,
                item.Market.Name,
                item.IncomeFactor.ToString("N3", actor),
                item.Savings.ToString("N3", actor),
                item.CurrentStress.ToString("P2", actor),
                item.CurrentStressPoint?.Name ?? "",
                item.PopulationScale.ToString("N0", actor),
                item.MarketPopulationNeeds.Count().ToString("N0", actor)
            },
            new List<string>
            {
                "ID",
                "Name",
                "Market",
                "Income",
                "Savings",
                "Stress",
                "Stress Name",
                "Population",
                "# Needs"
            },
            actor,
            Telnet.Yellow
        ));
    }

    private static void MarketPopulationNew(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which market do you want to create a market population in?");
            return;
        }

        IMarket market = actor.Gameworld.Markets.GetByIdOrName(ss.PopSpeech());
        if (market is null)
        {
            actor.OutputHandler.Send("There is no such market.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new market population?");
            return;
        }

        string name = ss.SafeRemainingArgument.TitleCase();
        if (actor.Gameworld.MarketPopulations.Where(x => x.Market == market).Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a market population for the {market.Name.ColourValue()} market called {name.ColourName()}. Names must be unique.");
            return;
        }

        MarketPopulation population = new(market, name);
        actor.Gameworld.Add(population);
        actor.RemoveAllEffects<BuilderEditingEffect<IMarketPopulation>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketPopulation>(actor) { EditingItem = population });
        actor.OutputHandler.Send($"You are create a new market population in the {market.Name.ColourValue()} market called {population.Name.ColourName()}, which you are now editing.");
    }

    private static void MarketPopulationSet(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketPopulation> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketPopulation>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market populations.");
            return;
        }

        effect.EditingItem.BuildingCommand(actor, ss);
    }

    private static void MarketPopulationEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            BuilderEditingEffect<IMarketPopulation> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketPopulation>>().FirstOrDefault();
            if (effect is null)
            {
                actor.OutputHandler.Send("Which market population would you like to edit?");
                return;
            }

            actor.OutputHandler.Send(effect.EditingItem.Show(actor));
            return;
        }

        IMarketPopulation population = actor.Gameworld.MarketPopulations.GetByIdOrName(ss.SafeRemainingArgument);
        if (population is null)
        {
            actor.OutputHandler.Send("There is no market population like that.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketPopulation>>();
        actor.AddEffect(new BuilderEditingEffect<IMarketPopulation>(actor) { EditingItem = population });
        actor.OutputHandler.Send($"You are now editing the {population.Name.ColourName()} market population.");
    }

    private static void MarketPopulationClose(ICharacter actor, StringStack ss)
    {
        BuilderEditingEffect<IMarketPopulation> effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketPopulation>>().FirstOrDefault();
        if (effect is null)
        {
            actor.OutputHandler.Send("You are not editing any market populations.");
            return;
        }

        actor.RemoveAllEffects<BuilderEditingEffect<IMarketPopulation>>();
        actor.OutputHandler.Send("You are no longer editing any market populations.");
    }
    #endregion
    #endregion

    #region Shoppers

    public const string ShopperHelp = @"The #3shopper#0 command is used to build and edit shoppers, which are virtual customers that patronise stores and inject money into the economy.

The syntax for this command is as follows:

	#3shopper list#0 - lists all shoppers
	#3shopper edit <which>#0 - begins editing a shopper
	#3shopper close#0 - stops editing a shopper
	#3shopper show <which>#0 - views a shopper
	#3shopper edit new <type> <economic zone> <name>#0 - creates a shopper
	#3shopper types#0 - view a list of types
	#3shopper typehelp <type>#0 - show the help file for a type
	#3shopper log <which>#0 - view a log of activity for a particular shopper
	#3shopper set name <name>#0 - renames the shopper
	#3shopper set economiczone <which>#0 - changes the economic zone
	#3shopper set interval every <x> hours|days|weekdays|weeks|months|years <offset>#0 - sets the interval
	#3shopper set next <date> <time>#0 - sets the next shopping date time
	#3shopper set ...#0 - other specific options for specific types";

    [PlayerCommand("Shopper", "shopper")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("shopper", ShopperHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Shopper(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (ss.PeekSpeech().EqualTo("log"))
        {
            ss.PopSpeech();
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("Which shopper would you like to view the logs for?");
                return;
            }

            IShopper shopper = actor.Gameworld.Shoppers.GetByIdOrName(ss.PopSpeech());
            if (shopper is null)
            {
                actor.OutputHandler.Send($"There is no shopper identified by the text {ss.Last.ColourCommand()}.");
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine($"Shopping logs for shopper #{shopper.Id.ToStringN0(actor)} ({shopper.Name.ColourName()})");

            DateTime since = DateTime.MinValue;
            if (!ss.IsFinished)
            {
                if (!TimeSpanParser.TryParse(ss.SafeRemainingArgument, Units.Days, Units.Days, out TimeSpan ts))
                {
                    actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} not a valid time span.\n#3Note: Years and Months are not supported, use Weeks or Days in that case#0".SubstituteANSIColour());
                    return;
                }

                since = DateTime.UtcNow - ts;
                sb.AppendLine($"Since: {since.GetLocalDateString(actor, true).ColourValue()}");
            }

            sb.AppendLine();
            using (new FMDB())
            {
                List<Models.ShopperLog> logs = FMDB.Context.ShopperLogs.Where(x => x.ShopperId == shopper.Id).ToList();
                sb.AppendLine(
                    StringUtilities.GetTextTable(
                        from item in logs
                        orderby item.DateTime descending, item.Id descending
                        select new List<string>
                        {
                            item.DateTime.GetLocalDateString(actor, true),
                            new MudDateTime(item.MudDateTime, actor.Gameworld).ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
                            item.LogType,
                            item.LogEntry
                        },
                        new List<string>
                        {
                            "Date",
                            "Mud Date",
                            "Type",
                            "Entry"
                        },
                        actor,
                        Telnet.FunctionYellow
                    )
                );
            }

            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        if (ss.PeekSpeech().EqualTo("types"))
        {
            StringBuilder sb = new();
            sb.AppendLine($"There are the following types:");
            sb.AppendLine();
            sb.AppendLine(StringUtilities.GetTextTable(
                from item in ShopperBase.Types
                let help = ShopperBase.GetTypeInfoFor(item)
                select new List<string>
                {
                    item,
                    help.Blurb
                },
                new List<string>
                {
                    "Type",
                    "Blurb"
                },
                actor,
                Telnet.FunctionYellow
            ));
            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        if (ss.PeekSpeech().EqualTo("typehelp"))
        {
            ss.PopSpeech();
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("Which type do you want to see help info for?");
                return;
            }

            string type = ss.SafeRemainingArgument.ToLowerInvariant();
            if (ShopperBase.Types.All(x => !x.EqualTo(type)))
            {
                actor.OutputHandler.Send($"There is no type identified by the text {type.ColourCommand()}.");
                return;
            }

            (string blurb, string help) = ShopperBase.GetTypeInfoFor(type);
            StringBuilder sb = new();
            sb.AppendLine($"Type help for shopper type {type.ColourCommand()}:");
            sb.AppendLine();
            sb.AppendLine($"Blurb: {blurb}");
            sb.AppendLine();
            sb.AppendLine(help.SubstituteANSIColour());
            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        BaseBuilderModule.GenericBuildingCommand(actor, ss, EditableItemHelper.ShopperHelper);
    }

    #endregion
}
