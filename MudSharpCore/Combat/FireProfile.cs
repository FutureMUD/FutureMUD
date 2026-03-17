using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp.Combat;

public class FireProfile : IFireProfile
{
	private readonly IFuturemud _gameworld;
	private readonly List<ITag> _extinguishTags = new();

	public FireProfile(IFuturemud gameworld)
	{
		_gameworld = gameworld;
		Name = "Fire";
		DamageType = DamageType.Burning;
		DamagePerTick = 1.0;
		TickFrequency = System.TimeSpan.FromSeconds(10);
		MinimumOxidation = 0.1;
		SpreadChance = 0.1;
	}

	public FireProfile(XElement root, IFuturemud gameworld)
	{
		_gameworld = gameworld;
		Name = root.Element("Name")?.Value ?? "Fire";
		DamageType = (DamageType)int.Parse(root.Element("DamageType")?.Value ?? $"{(int)DamageType.Burning}");
		DamagePerTick = double.Parse(root.Element("DamagePerTick")?.Value ?? "0");
		PainPerTick = double.Parse(root.Element("PainPerTick")?.Value ?? "0");
		StunPerTick = double.Parse(root.Element("StunPerTick")?.Value ?? "0");
		ThermalLoadPerTick = double.Parse(root.Element("ThermalLoadPerTick")?.Value ?? "0");
		SpreadChance = double.Parse(root.Element("SpreadChance")?.Value ?? "0");
		MinimumOxidation = double.Parse(root.Element("MinimumOxidation")?.Value ?? "0");
		SelfOxidising = bool.Parse(root.Element("SelfOxidising")?.Value ?? "false");
		TickFrequency = System.TimeSpan.FromSeconds(double.Parse(root.Element("TickFrequencySeconds")?.Value ?? "10"));
		foreach (var tag in root.Element("ExtinguishTags")?.Elements("Tag") ?? Enumerable.Empty<XElement>())
		{
			if (!long.TryParse(tag.Value, out var id))
			{
				continue;
			}

			var loadedTag = _gameworld.Tags.Get(id);
			if (loadedTag is not null)
			{
				_extinguishTags.Add(loadedTag);
			}
		}
	}

	public string Name { get; set; }
	public DamageType DamageType { get; set; }
	public double DamagePerTick { get; set; }
	public double PainPerTick { get; set; }
	public double StunPerTick { get; set; }
	public double ThermalLoadPerTick { get; set; }
	public double SpreadChance { get; set; }
	public double MinimumOxidation { get; set; }
	public bool SelfOxidising { get; set; }
	public System.TimeSpan TickFrequency { get; set; }
	public IEnumerable<ITag> ExtinguishTags => _extinguishTags;

	public XElement SaveToXml()
	{
		return new XElement("FireProfile",
			new XElement("Name", new XCData(Name)),
			new XElement("DamageType", (int)DamageType),
			new XElement("DamagePerTick", DamagePerTick),
			new XElement("PainPerTick", PainPerTick),
			new XElement("StunPerTick", StunPerTick),
			new XElement("ThermalLoadPerTick", ThermalLoadPerTick),
			new XElement("SpreadChance", SpreadChance),
			new XElement("MinimumOxidation", MinimumOxidation),
			new XElement("SelfOxidising", SelfOxidising),
			new XElement("TickFrequencySeconds", TickFrequency.TotalSeconds),
			new XElement("ExtinguishTags", _extinguishTags.Select(x => new XElement("Tag", x.Id)))
		);
	}
}
