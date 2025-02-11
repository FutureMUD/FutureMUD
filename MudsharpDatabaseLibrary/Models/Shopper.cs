using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MudSharp.Models;
public class Shopper
{
	public long Id { get; set; }
	public string Name { get; set; }
	public long EconomicZoneId { get; set; }
	public string Interval { get; set; }
	public string NextDate { get; set; }
	public string Type { get; set; }
	public string Definition { get; set; }

	public virtual EconomicZone EconomicZone { get; set; }
	public virtual ICollection<ShopperLog> ShopperLogs { get; set; }
}