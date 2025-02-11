using System;

namespace MudSharp.Models;

public class ShopperLog
{
	public long Id { get; set; }
	public long ShopperId { get; set; }
	public DateTime DateTime { get; set; }
	public string MudDateTime { get; set; }
	public string LogType { get; set; }
	public string LogEntry { get; set; }
	public virtual Shopper Shopper { get; set; }
}