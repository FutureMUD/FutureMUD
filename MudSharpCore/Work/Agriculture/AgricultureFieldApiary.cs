
namespace MudSharp.Work.Agriculture;

internal sealed class AgricultureFieldApiary : IAgricultureFieldApiary
{
	public AgricultureFieldApiary(int hiveCount, int colonyHealth, int stores, int yieldPotential, int pollinationRadius)
	{
		HiveCount = Math.Max(0, hiveCount);
		ColonyHealth = colonyHealth.ClampScore();
		Stores = stores.ClampScore();
		YieldPotential = yieldPotential.ClampScore();
		PollinationRadius = Math.Max(0, pollinationRadius);
	}

	public int HiveCount { get; private set; }
	public int ColonyHealth { get; private set; }
	public int Stores { get; private set; }
	public int YieldPotential { get; private set; }
	public int PollinationRadius { get; private set; }
	public int PollinationStrength => HiveCount <= 0
		? 0
		: Math.Clamp((ColonyHealth + Stores + YieldPotential) / 3 + Math.Max(0, HiveCount - 1) * 5, 0, 100);

	public void Adjust(int healthDelta, int storesDelta, int yieldDelta)
	{
		ColonyHealth = (ColonyHealth + healthDelta).ClampScore();
		Stores = (Stores + storesDelta).ClampScore();
		YieldPotential = (YieldPotential + yieldDelta).ClampScore();
	}

	public XElement SaveToXml()
	{
		return new XElement("Apiary",
			new XAttribute("hives", HiveCount),
			new XAttribute("health", ColonyHealth),
			new XAttribute("stores", Stores),
			new XAttribute("yield", YieldPotential),
			new XAttribute("radius", PollinationRadius));
	}

	public static AgricultureFieldApiary LoadFromXml(XElement root)
	{
		return new AgricultureFieldApiary(
			(int?)root.Attribute("hives") ?? 0,
			(int?)root.Attribute("health") ?? 50,
			(int?)root.Attribute("stores") ?? 0,
			(int?)root.Attribute("yield") ?? 0,
			(int?)root.Attribute("radius") ?? 2);
	}
}
