#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Economy;

public enum HaggleNegotiationScope
{
	Shared,
	PerDirection
}

/// <summary>Builder-owned shop policy. Percentages are signed fractions of the deal-adjusted price.</summary>
public sealed class HaggleSettings
{
	public bool Disabled { get; set; }
	public Difficulty? Difficulty { get; set; }
	public decimal SaleDiscount { get; set; } = 0.10M;
	public decimal SaleMarkup { get; set; } = 0.20M;
	public decimal BuyBonus { get; set; } = 0.10M;
	public decimal BuyPenalty { get; set; } = 0.20M;
	public HashSet<long> ExcludedMerchandise { get; } = new();
	public HashSet<long> PricingEmployees { get; } = new();
	public List<string> Audit { get; } = new();

	public decimal Clamp(decimal adjustment, ShopDealApplicability direction) =>
		direction == ShopDealApplicability.Buy
			? Math.Clamp(adjustment, -BuyPenalty, BuyBonus)
			: Math.Clamp(adjustment, -SaleDiscount, SaleMarkup);

	public XElement Save() => new("Haggling",
		new XAttribute("Disabled", Disabled),
		new XAttribute("Difficulty", Difficulty.HasValue ? (int)Difficulty.Value : -1),
		new XAttribute("SaleDiscount", SaleDiscount), new XAttribute("SaleMarkup", SaleMarkup),
		new XAttribute("BuyBonus", BuyBonus), new XAttribute("BuyPenalty", BuyPenalty),
		new XElement("Excluded", ExcludedMerchandise.Select(x => new XElement("Id", x))),
		new XElement("PricingEmployees", PricingEmployees.Select(x => new XElement("Id", x))),
		new XElement("Audit", Audit.TakeLast(100).Select(x => new XElement("Entry", x))));

	public static HaggleSettings Load(string? xml)
	{
		var result = new HaggleSettings();
		if (string.IsNullOrWhiteSpace(xml)) return result;
		var root = XElement.Parse(xml);
		result.Disabled = (bool?)root.Attribute("Disabled") ?? false;
		var difficulty = (int?)root.Attribute("Difficulty") ?? -1;
		result.Difficulty = difficulty is >= 0 and <= 10 ? (Difficulty)difficulty : null;
		result.SaleDiscount = Math.Clamp((decimal?)root.Attribute("SaleDiscount") ?? 0.10M, 0M, 0.99M);
		result.SaleMarkup = Math.Clamp((decimal?)root.Attribute("SaleMarkup") ?? 0.20M, 0M, 10M);
		result.BuyBonus = Math.Clamp((decimal?)root.Attribute("BuyBonus") ?? 0.10M, 0M, 10M);
		result.BuyPenalty = Math.Clamp((decimal?)root.Attribute("BuyPenalty") ?? 0.20M, 0M, 0.99M);
		result.ExcludedMerchandise.UnionWith(root.Element("Excluded")?.Elements("Id").Select(x => (long)x) ?? []);
		result.PricingEmployees.UnionWith(root.Element("PricingEmployees")?.Elements("Id").Select(x => (long)x) ?? []);
		result.Audit.AddRange(root.Element("Audit")?.Elements("Entry").Select(x => x.Value) ?? []);
		return result;
	}
}
