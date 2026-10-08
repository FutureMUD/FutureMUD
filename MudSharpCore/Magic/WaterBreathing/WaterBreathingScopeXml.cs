#nullable enable

using MudSharp.Form.Material;

namespace MudSharp.Magic.WaterBreathing;

internal static class WaterBreathingScopeXml
{
	internal static WaterBreathingFluidScope Read(XElement? root, IFuturemud world)
	{
		if (root is null || (int?)root.Attribute("version") != 1 ||
			root.Elements().Any(x => x.Name != "Liquid"))
			throw new FormatException("Water scope requires version 1 and explicit native Liquid mappings.");
		var liquids = root.Elements("Liquid").Select(x =>
			world.Liquids.Get((long?)x.Attribute("id") ?? 0) ??
			throw new FormatException("A mapped water liquid is unavailable in this world.")).ToArray();
		return new(liquids);
	}

	internal static XElement Write(WaterBreathingFluidScope scope) => new("WaterScope",
		new XAttribute("version", 1), scope.LiquidIds.Select(id => new XElement("Liquid", new XAttribute("id", id))));

	internal static bool IsCurrent(WaterBreathingFluidScope scope, IFuturemud world) =>
		scope.MatchesConfiguredLiquids(scope.LiquidIds.Select(id => world.Liquids.Get(id))!);
}
