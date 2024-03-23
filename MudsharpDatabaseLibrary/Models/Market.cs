using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MudSharp.Models;

public class Market
{
	public Market()
	{
		Influences = new HashSet<MarketInfluence>();
		MarketMarketCategories = new HashSet<MarketMarketCategory>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public long EconomicZoneId { get; set; }
	public string MarketPriceFormula { get; set; }

	public virtual EconomicZone EconomicZone { get; set; }
	public virtual ICollection<MarketInfluence> Influences { get; set; }
	public virtual ICollection<MarketMarketCategory> MarketMarketCategories { get; set; }
}

public class MarketInfluence
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public long MarketId { get; set; }
	public string AppliesFrom { get; set; }
	public string AppliesUntil { get; set; }
	public long CharacterKnowsAboutInfluenceProgId { get; set; }
	public long? MarketInfluenceTemplateId { get; set; }
	public string Impacts { get; set; }
	public virtual Market Market { get; set; }
	public virtual FutureProg CharacterKnowsAboutInfluenceProg { get; set; }
	public virtual MarketInfluenceTemplate MarketInfluenceTemplate { get; set; }
}

public class MarketInfluenceTemplate
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public long CharacterKnowsAboutInfluenceProgId { get; set; }
	public string Impacts { get; set; }
	public string TemplateSummary { get; set; }
	public virtual FutureProg CharacterKnowsAboutInfluenceProg { get; set; }
}

public class MarketCategory
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public double ElasticityFactorAbove { get; set; }
	public double ElasticityFactorBelow { get; set; }
	public string Tags { get; set; }
}

public class MarketMarketCategory
{
	public long MarketId { get; set; }
	public long MarketCategoryId { get; set; }
	public virtual Market Market { get; set; }
	public virtual MarketCategory MarketCategory { get; set; }
}
