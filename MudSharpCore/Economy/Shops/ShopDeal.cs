#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Shops;

public class ShopDeal : LateInitialisingItem, IShopDeal
{
	public ShopDeal(IShop shop, string name)
	{
		Gameworld = shop.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		Shop = shop;
		_name = name;
		DealType = ShopDealType.Sale;
		TargetType = ShopDealTargetType.AllMerchandise;
		PriceAdjustmentPercentage = -0.1M;
		MinimumQuantity = 0;
		Applicability = ShopDealApplicability.Sell;
		Expiry = MudDateTime.Never;
		IsCumulative = true;
	}

	public ShopDeal(Models.ShopDeal deal, IShop shop, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Shop = shop;
		_id = deal.Id;
		IdInitialised = true;
		_name = deal.Name;
		DealType = (ShopDealType)deal.DealType;
		TargetType = (ShopDealTargetType)deal.TargetType;
		_targetMerchandiseId = deal.MerchandiseId;
		_targetTagId = deal.TagId;
		PriceAdjustmentPercentage = deal.PriceAdjustmentPercentage;
		MinimumQuantity = deal.MinimumQuantity ?? 0;
		Applicability = (ShopDealApplicability)deal.Applicability;
		_eligibilityProg = gameworld.FutureProgs.Get(deal.EligibilityProgId ?? 0);
		Expiry = string.IsNullOrWhiteSpace(deal.ExpiryDateTime) ? MudDateTime.Never : new MudDateTime(deal.ExpiryDateTime, gameworld);
		IsCumulative = deal.IsCumulative;
	}

	public override string FrameworkItemType => "ShopDeal";
	public IShop Shop { get; }
	public ShopDealType DealType { get; private set; }
	public ShopDealTargetType TargetType { get; private set; }

	private long? _targetMerchandiseId;
	private IMerchandise? _targetMerchandise;
	public IMerchandise? TargetMerchandise
	{
		get
		{
			_targetMerchandise ??= Shop.Merchandises.Get(_targetMerchandiseId ?? 0L);
			return _targetMerchandise;
		}
	}

	private long? _targetTagId;
	private ITag? _targetTag;
	public ITag? TargetTag
	{
		get
		{
			_targetTag ??= Gameworld.Tags.Get(_targetTagId ?? 0L);
			return _targetTag;
		}
	}

	public decimal PriceAdjustmentPercentage { get; private set; }
	public int MinimumQuantity { get; private set; }
	public ShopDealApplicability Applicability { get; private set; }
	private IFutureProg? _eligibilityProg;
	public IFutureProg? EligibilityProg => _eligibilityProg;
	public MudDateTime Expiry { get; private set; }
	public bool IsCumulative { get; private set; }

	public bool IsExpired =>
		Expiry.Date is not null &&
		Shop.EconomicZone.ZoneForTimePurposes.DateTime() >= Expiry;

	public bool AppliesToMerchandise(IMerchandise merchandise)
	{
		return TargetType switch
		{
			ShopDealTargetType.AllMerchandise => true,
			ShopDealTargetType.Merchandise => TargetMerchandise == merchandise,
			ShopDealTargetType.ItemTag => TargetTag is not null &&
			                             merchandise.Item.Tags.Any(x => x.IsA(TargetTag)),
			_ => false
		};
	}

	public bool Applies(IMerchandise merchandise, ICharacter? shopper, int quantity, ShopDealApplicability applicability,
		MudDateTime now)
	{
		if (!Applicability.HasFlag(applicability))
		{
			return false;
		}

		if (!AppliesToMerchandise(merchandise))
		{
			return false;
		}

		if (Expiry.Date is not null && now >= Expiry)
		{
			return false;
		}

		if (DealType == ShopDealType.Volume && quantity < MinimumQuantity)
		{
			return false;
		}

		if (_eligibilityProg is null)
		{
			return true;
		}

		if (shopper is null)
		{
			return false;
		}

		return _eligibilityProg.ExecuteBool(false, shopper, Shop, merchandise, quantity, now);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.ShopDeals.Find(Id);
		dbitem.ShopId = Shop.Id;
		dbitem.Name = Name;
		dbitem.DealType = (int)DealType;
		dbitem.TargetType = (int)TargetType;
		dbitem.MerchandiseId = _targetMerchandiseId;
		dbitem.TagId = _targetTagId;
		dbitem.PriceAdjustmentPercentage = PriceAdjustmentPercentage;
		dbitem.MinimumQuantity = DealType == ShopDealType.Volume ? MinimumQuantity : null;
		dbitem.Applicability = (int)Applicability;
		dbitem.EligibilityProgId = _eligibilityProg?.Id;
		dbitem.ExpiryDateTime = Expiry.Date is null ? null : Expiry.GetDateTimeString();
		dbitem.IsCumulative = IsCumulative;
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.ShopDeal
		{
			ShopId = Shop.Id,
			Name = Name,
			DealType = (int)DealType,
			TargetType = (int)TargetType,
			MerchandiseId = _targetMerchandiseId,
			TagId = _targetTagId,
			PriceAdjustmentPercentage = PriceAdjustmentPercentage,
			MinimumQuantity = DealType == ShopDealType.Volume ? MinimumQuantity : null,
			Applicability = (int)Applicability,
			EligibilityProgId = _eligibilityProg?.Id,
			ExpiryDateTime = Expiry.Date is null ? null : Expiry.GetDateTimeString(),
			IsCumulative = IsCumulative
		};
		FMDB.Context.ShopDeals.Add(dbitem);
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((Models.ShopDeal)dbitem).Id;
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			Gameworld.SaveManager.Flush();
			using (new FMDB())
			{
				var dbitem = FMDB.Context.ShopDeals.Find(Id);
				if (dbitem is not null)
				{
					FMDB.Context.ShopDeals.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "type":
				return BuildingCommandType(actor, command);
			case "target":
				return BuildingCommandTarget(actor, command);
			case "adjustment":
			case "discount":
			case "modifier":
				return BuildingCommandAdjustment(actor, command);
			case "applies":
			case "scope":
				return BuildingCommandApplies(actor, command);
			case "prog":
			case "eligibility":
				return BuildingCommandProg(actor, command);
			case "cumulative":
			case "stack":
				return BuildingCommandCumulative(actor);
			case "expires":
				return BuildingCommandExpires(actor, command);
			case "expiresin":
			case "duration":
				return BuildingCommandExpiresIn(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private string HelpText =>
		@"You can use the following subcommands with this shop deal:

	#3name <name>#0 - renames this deal
	#3type sale#0 - makes this an always-on sale deal
	#3type volume <quantity>#0 - makes this a volume deal at the specified threshold
	#3target all#0 - targets all merchandise in the shop
	#3target merchandise <record>#0 - targets a single merchandise record
	#3target tag <tag>#0 - targets item prototypes with the specified tag
	#3adjustment <signed %>#0 - sets the signed price change (negative = discount, positive = surcharge)
	#3applies sell|buy|both#0 - controls whether this affects buying from or selling to the shop
	#3prog clear|<prog>#0 - sets a shopper eligibility prog
	#3cumulative#0 - toggles whether this stacks with other cumulative deals
	#3expires never|<datetime>#0 - sets an absolute expiry
	#3expiresin <timespan>#0 - sets expiry relative to now";

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this deal?");
			return false;
		}

		_name = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This shop deal is now called {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should this deal be of type SALE or VOLUME?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "sale":
				DealType = ShopDealType.Sale;
				MinimumQuantity = 0;
				Changed = true;
				actor.OutputHandler.Send($"This deal is now a {DealType.DescribeEnum().ColourName()} deal.");
				return true;
			case "volume":
				if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var quantity) || quantity < 2)
				{
					actor.OutputHandler.Send("You must specify a quantity threshold of 2 or more for a volume deal.");
					return false;
				}

				DealType = ShopDealType.Volume;
				MinimumQuantity = quantity;
				Changed = true;
				actor.OutputHandler.Send($"This deal is now a volume deal that applies at {MinimumQuantity.ToString("N0", actor).ColourValue()} or more items.");
				return true;
			default:
				actor.OutputHandler.Send("Valid deal types are SALE and VOLUME <quantity>.");
				return false;
		}
	}

	private bool BuildingCommandTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to target ALL merchandise, a MERCHANDISE record, or a TAG?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "all":
				TargetType = ShopDealTargetType.AllMerchandise;
				_targetMerchandise = null;
				_targetMerchandiseId = null;
				_targetTag = null;
				_targetTagId = null;
				Changed = true;
				actor.OutputHandler.Send("This deal now targets all merchandise in the shop.");
				return true;
			case "merch":
			case "merchandise":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which merchandise record should this deal target?");
					return false;
				}

				var merch = Shop.Merchandises.GetById(command.Last) ??
				            Shop.Merchandises.GetFromItemListByKeywordIncludingNames(command.Last, actor);
				if (merch is null)
				{
					actor.OutputHandler.Send("There is no such merchandise record for this shop.");
					return false;
				}

				TargetType = ShopDealTargetType.Merchandise;
				_targetMerchandise = merch;
				_targetMerchandiseId = merch.Id;
				_targetTag = null;
				_targetTagId = null;
				Changed = true;
				actor.OutputHandler.Send($"This deal now targets the merchandise record {merch.Name.ColourName()}.");
				return true;
			case "tag":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which tag should this deal target?");
					return false;
				}

				var matchedTags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
				if (matchedTags.Count == 0)
				{
					actor.OutputHandler.Send("There is no such tag.");
					return false;
				}

				if (matchedTags.Count > 1)
				{
					actor.OutputHandler.Send(
						$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedTags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
					return false;
				}

				TargetType = ShopDealTargetType.ItemTag;
				_targetTag = matchedTags.Single();
				_targetTagId = _targetTag.Id;
				_targetMerchandise = null;
				_targetMerchandiseId = null;
				Changed = true;
				actor.OutputHandler.Send($"This deal now targets merchandise whose item prototype has the {_targetTag.FullName.ColourName()} tag.");
				return true;
			default:
				actor.OutputHandler.Send("Valid deal targets are ALL, MERCHANDISE <record>, or TAG <tag>.");
				return false;
		}
	}

	private bool BuildingCommandAdjustment(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("You must enter a signed percentage adjustment, such as -30% or 15%.");
			return false;
		}

		PriceAdjustmentPercentage = value;
		Changed = true;
		actor.OutputHandler.Send($"This deal now applies an adjustment of {DescribePercentage(value, actor)}.");
		return true;
	}

	private bool BuildingCommandApplies(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should this deal apply to SELL prices, BUY prices, or BOTH?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "sell":
				Applicability = ShopDealApplicability.Sell;
				break;
			case "buy":
				Applicability = ShopDealApplicability.Buy;
				break;
			case "both":
				Applicability = ShopDealApplicability.Both;
				break;
			default:
				actor.OutputHandler.Send("Valid options are SELL, BUY, or BOTH.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This deal now applies to {Applicability.DescribeEnum().ColourName()} pricing.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What eligibility prog should this deal use? Use CLEAR to remove any prog.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove"))
		{
			_eligibilityProg = null;
			Changed = true;
			actor.OutputHandler.Send("This deal no longer has an eligibility prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Shop,
				ProgVariableTypes.Merchandise,
				ProgVariableTypes.Number,
				ProgVariableTypes.MudDateTime
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_eligibilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This deal will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine eligibility.");
		return true;
	}

	private bool BuildingCommandCumulative(ICharacter actor)
	{
		IsCumulative = !IsCumulative;
		Changed = true;
		actor.OutputHandler.Send($"This deal will {(IsCumulative ? "now" : "no longer")} stack cumulatively with other cumulative deals.");
		return true;
	}

	private bool BuildingCommandExpires(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(MudDateTime.TryParseHelpText(actor, Shop.EconomicZone));
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("never", "none", "off", "clear"))
		{
			Expiry = MudDateTime.Never;
			Changed = true;
			actor.OutputHandler.Send("This deal will no longer expire.");
			return true;
		}

		if (!MudDateTime.TryParse(command.SafeRemainingArgument, Shop.EconomicZone.FinancialPeriodReferenceCalendar,
			    Shop.EconomicZone.FinancialPeriodReferenceClock, actor, out var value, out var error))
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		Expiry = value;
		Changed = true;
		actor.OutputHandler.Send($"This deal will now expire at {Expiry.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandExpiresIn(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid timespan, such as 30 days or 2 weeks.");
			return false;
		}

		Expiry = Shop.EconomicZone.ZoneForTimePurposes.DateTime() + value;
		Changed = true;
		actor.OutputHandler.Send($"This deal will now expire in {value.Describe(actor).ColourValue()}, at {Expiry.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
		return true;
	}

	public void ShowToBuilder(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Shop Deal #{Id.ToString("N0", actor)}: {Name.ColourName()}");
		sb.AppendLine($"Type: {DescribeType(actor)}");
		sb.AppendLine($"Target: {DescribeTarget(actor)}");
		sb.AppendLine($"Adjustment: {DescribePercentage(PriceAdjustmentPercentage, actor)}");
		sb.AppendLine($"Applies To: {Applicability.DescribeEnum().ColourName()}");
		sb.AppendLine($"Eligibility Prog: {_eligibilityProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		sb.AppendLine($"Cumulative: {IsCumulative.ToColouredString()}");
		sb.AppendLine($"Expiry: {(Expiry.Date is null ? "Never".ColourValue() : Expiry.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue())}");
		actor.OutputHandler.Send(sb.ToString());
	}

	public string DescribeType(ICharacter actor)
	{
		return DealType switch
		{
			ShopDealType.Sale => "Sale".ColourName(),
			ShopDealType.Volume => $"Volume ({MinimumQuantity.ToString("N0", actor).ColourValue()}+)".ColourName(),
			_ => DealType.DescribeEnum().ColourName()
		};
	}

	public string DescribeTarget(ICharacter actor)
	{
		return TargetType switch
		{
			ShopDealTargetType.AllMerchandise => "All Merchandise".ColourValue(),
			ShopDealTargetType.Merchandise => $"{TargetMerchandise?.Name.ColourName() ?? "Unknown Merchandise".ColourError()}",
			ShopDealTargetType.ItemTag => $"{TargetTag?.FullName.ColourName() ?? "Unknown Tag".ColourError()}",
			_ => TargetType.DescribeEnum().ColourName()
		};
	}

	public static string DescribePercentage(decimal value, IFormatProvider actor)
	{
		if (value > 0.0M)
		{
			return $"+{value.ToString("P2", actor)}".ColourError();
		}

		if (value < 0.0M)
		{
			return value.ToString("P2", actor).ColourValue();
		}

		return value.ToString("P2", actor).ColourValue();
	}
}
