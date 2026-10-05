#nullable enable

using System.Xml.Linq;
using MudSharp.Magic.Emotions;

namespace MudSharp.Magic;

public sealed record RousedFuryTerrainBindings(long Air, long City, long Inside, long Hills,
	long Mountain, long Thornlands, long Earth);

/// <summary>Independent editable definition; construction/paid runtime awaits main-owned hook allocation.</summary>
public static class ArmageddonRousedFuryStock
{
	public const string Key = "arm.spell.roused_fury";
	public const string Name = "Roused Fury";
	public const string PrerequisiteKey = ArmageddonUnravelEnchantmentStock.Key;
	public const double PrerequisiteRaw = 80;
	public const int Opening = 30;
	public const double RawCap = 90;
	public const double BranchRaw = 80;
	public const double MinimumEnergy = 20;
	public const string LifetimeGroup = "armageddon.roused_fury";
	public const int UnitSeconds = 600;
	public const int MaximumUnits = 36;
	public const string DurationFormula = "0";

	public static EmotionalStockProfile Profile(long attributeId, double unitsPerSourcePoint, double intensity,
		long eligibilityProgId, RousedFuryTerrainBindings terrains)
	{
		ArgumentNullException.ThrowIfNull(terrains);
		return new(EmotionalSpellKind.Fury, LifetimeGroup, UnitSeconds, MaximumUnits, eligibilityProgId, intensity,
			attributeId, unitsPerSourcePoint, new(3, 1, 0),
			[new(terrains.Air, new(2, 1, 0)), new(terrains.City, new(5, 2, 0)), new(terrains.Inside, new(5, 2, 0)),
			 new(terrains.Hills, new(7, 2, 0)), new(terrains.Mountain, new(7, 2, 0)), new(terrains.Thornlands, new(33, 10, 0)),
			 new(terrains.Earth, new(4, 1, 1))], [], false);
	}

	public static XElement Definition(long resourceId, long costExpressionId, EmotionalStockProfile profile)
	{
		if (profile.Kind != EmotionalSpellKind.Fury || resourceId <= 0 || costExpressionId <= 0)
			throw new ArgumentException("Select a Fury profile and existing resource/cost expression IDs.");
		return ArmageddonUtilityStock.Definition(Key, "character", resourceId, costExpressionId,
			profile.EligibilityProgId, Opening, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "sourcefury"), profile.SaveToXml()));
	}
}
