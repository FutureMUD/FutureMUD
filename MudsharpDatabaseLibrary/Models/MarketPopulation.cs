using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models;

public class MarketPopulation
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int PopulationScale { get; set; }
	public long MarketId { get; set; }
	public string MarketPopulationNeeds { get; set; }
	public string MarketStressPoints { get; set; }

	public virtual Market Market { get; set; }
}