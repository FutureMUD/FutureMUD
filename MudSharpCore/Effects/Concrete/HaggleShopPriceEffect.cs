#nullable enable
using System.Globalization;
using MudSharp.Economy;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Effects.Concrete;

public sealed class HaggleShopPriceEffect : Effect
{
	public HaggleShopPriceEffect(ICharacter owner, long shopId, ShopDealApplicability scope,
		decimal saleAdjustment, decimal buyAdjustment, DateTime expiresAtUtc, long issuerId = 0,
		string reason = "Skill negotiation") : base(owner)
	{
		ShopId = shopId;
		Scope = scope;
		SaleAdjustment = saleAdjustment;
		BuyAdjustment = buyAdjustment;
		WindowExpiresAtUtc = expiresAtUtc;
		IssuerId = issuerId;
		Reason = reason;
	}

	private HaggleShopPriceEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var data = root.Element("Haggle")!;
		ShopId = (long)data.Attribute("Shop")!;
		Scope = (ShopDealApplicability)(int)data.Attribute("Scope")!;
		SaleAdjustment = (decimal)data.Attribute("Sale")!;
		BuyAdjustment = (decimal)data.Attribute("Buy")!;
		IssuerId = (long)data.Attribute("Issuer")!;
		Reason = data.Element("Reason")?.Value ?? "";
		WindowExpiresAtUtc = DateTime.Parse(data.Attribute("Expires")!.Value, CultureInfo.InvariantCulture,
			DateTimeStyles.RoundtripKind).ToUniversalTime();
		LoadErrors = WindowExpiresAtUtc <= RuntimeClock.UtcNow;
	}

	public long ShopId { get; }
	public ShopDealApplicability Scope { get; }
	public decimal SaleAdjustment { get; }
	public decimal BuyAdjustment { get; }
	public DateTime WindowExpiresAtUtc { get; }
	public long IssuerId { get; }
	public string Reason { get; }
	public string Origin => IssuerId == 0 ? "skill" : "employee";
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "HaggleShopPriceEffect";
	public override bool Applies() => WindowExpiresAtUtc > RuntimeClock.UtcNow;
	protected override XElement SaveDefinition() => new("Haggle",
		new XAttribute("Shop", ShopId), new XAttribute("Scope", (int)Scope),
		new XAttribute("Sale", SaleAdjustment), new XAttribute("Buy", BuyAdjustment),
		new XAttribute("Issuer", IssuerId), new XAttribute("Origin", Origin),
		new XAttribute("Expires", WindowExpiresAtUtc.ToString("O", CultureInfo.InvariantCulture)),
		new XElement("Reason", Reason));
	public static void InitialiseEffectType() => RegisterFactory("HaggleShopPriceEffect",
		(root, owner) => new HaggleShopPriceEffect(root, owner));
	public override string Describe(IPerceiver voyeur) =>
		$"Haggle shop #{ShopId.ToString("N0", voyeur)} ({Scope.DescribeEnum()}): sales {SaleAdjustment.ToString("P1", voyeur)}, buybacks {BuyAdjustment.ToString("P1", voyeur)}; {Origin}, issuer #{IssuerId.ToString("N0", voyeur)}, reason: {Reason}, expires {WindowExpiresAtUtc:O}.";
}
