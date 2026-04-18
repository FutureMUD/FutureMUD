#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character.Heritage;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;

namespace MudSharp.Body.Disfigurements;

internal readonly record struct ScarWoundContext(
	bool IsSurgery,
	DamageType DamageType,
	WoundSeverity Severity,
	SurgicalProcedureType? SurgicalProcedureType,
	int SurgeryCheckDegrees,
	Outcome BestTendedOutcome,
	bool HadInfection,
	bool WasCleaned,
	bool WasAntisepticTreated,
	bool WasClosed,
	bool WasTraumaControlled);

internal static class ScarGenerationSupport
{
	internal const string ChanceMatrixConfigurationName = "ScarGenerationChanceMatrix";
	internal const string OrientationConfigurationName = "ScarOrientationByBodypartShape";

	private static readonly ConditionalWeakTable<IFuturemud, ScarGenerationChanceMatrixCache> ChanceMatrixCache = new();
	private static readonly ConditionalWeakTable<IFuturemud, ScarOrientationMappingCache> OrientationCache = new();

	internal static ScarWoundContext GetContext(IWound wound)
	{
		return wound switch
		{
			SimpleOrganicWound organicWound => new ScarWoundContext(
				organicWound.ScarSurgicalProcedureType.HasValue,
				organicWound.DamageType,
				organicWound.Severity,
				organicWound.ScarSurgicalProcedureType,
				organicWound.ScarSurgeryCheckDegrees,
				organicWound.BestTendedOutcome,
				organicWound.HadInfection,
				organicWound.WasCleaned,
				organicWound.WasAntisepticTreated,
				organicWound.WasClosed,
				organicWound.WasTraumaControlled),
			HealingSimpleWound healingWound => new ScarWoundContext(
				healingWound.ScarSurgicalProcedureType.HasValue,
				healingWound.DamageType,
				healingWound.Severity,
				healingWound.ScarSurgicalProcedureType,
				healingWound.ScarSurgeryCheckDegrees,
				healingWound.BestTendedOutcome,
				false,
				false,
				false,
				false,
				false),
			_ => new ScarWoundContext(
				false,
				wound.DamageType,
				wound.Severity,
				null,
				0,
				Outcome.None,
				false,
				false,
				false,
				false,
				false)
		};
	}

	internal static double GetBaseChance(IFuturemud gameworld, ScarWoundContext context)
	{
		var matrix = GetChanceMatrix(gameworld);
		return context.IsSurgery
			? matrix.GetSurgeryChance(context.Severity)
			: matrix.GetDamageChance(context.DamageType, context.Severity);
	}

	internal static ScarGenerationChanceMatrix GetChanceMatrix(IFuturemud gameworld)
	{
		var cache = ChanceMatrixCache.GetOrCreateValue(gameworld);
		var rawConfiguration = gameworld.GetStaticConfiguration(ChanceMatrixConfigurationName);
		if (cache.Matrix is not null && string.Equals(cache.RawConfiguration, rawConfiguration, StringComparison.Ordinal))
		{
			return cache.Matrix;
		}

		cache.RawConfiguration = rawConfiguration;
		cache.Matrix = ScarGenerationChanceMatrix.Parse(rawConfiguration);
		return cache.Matrix;
	}

	internal static IScar CreateScar(
		IFuturemud gameworld,
		IRace race,
		IBodypart bodypart,
		ScarWoundContext context,
		MudDateTime timeOfScarring,
		int? variantSeed)
	{
		var shapeName = bodypart.Shape?.Name ?? string.Empty;
		var orientationPool = GetOrientationMapping(gameworld).ResolveProfile(shapeName, bodypart.Name, bodypart.FullDescription());
		var orientations = GetOrientationOptions(orientationPool);
		var orientation = orientations[(variantSeed ?? RandomUtilities.Random(0, orientations.Count)) % orientations.Count];
		var descriptor = GetDescriptor(context);
		var severityDescriptor = GetSeverityDescriptor(context.Severity, context.IsSurgery);
		var partText = bodypart.FullDescription();
		var shortDescription = BuildShortDescription(severityDescriptor, descriptor.ShortBase);
		var fullDescription =
			$"{severityDescriptor.FullPrefix} {descriptor.FullBase} {string.Format(orientation, partText)}; {descriptor.CausePhrase}";
		var sizeSteps = GetSizeSteps(context);
		var distinctiveness = GetDistinctiveness(context);
		var (overridePlain, overrideWith) = GetCharacteristicOverrides(shapeName, bodypart, context, distinctiveness);
		return new Scar(
			gameworld,
			race,
			bodypart,
			timeOfScarring,
			shortDescription,
			fullDescription,
			sizeSteps,
			distinctiveness,
			overridePlain,
			overrideWith,
			context.DamageType,
			context.Severity,
			context.IsSurgery,
			context.SurgicalProcedureType);
	}

	private static ScarOrientationMapping GetOrientationMapping(IFuturemud gameworld)
	{
		var cache = OrientationCache.GetOrCreateValue(gameworld);
		var rawConfiguration = gameworld.GetStaticConfiguration(OrientationConfigurationName);
		if (cache.Mapping is not null && string.Equals(cache.RawConfiguration, rawConfiguration, StringComparison.Ordinal))
		{
			return cache.Mapping;
		}

		cache.RawConfiguration = rawConfiguration;
		cache.Mapping = ScarOrientationMapping.Parse(rawConfiguration);
		return cache.Mapping;
	}

	private static ScarDescriptor GetDescriptor(ScarWoundContext context)
	{
		if (context.IsSurgery)
		{
			return new ScarDescriptor(
				"surgical seam scar",
				"surgical seam scar",
				"where careful incisions were drawn closed and healed into a taut seam.");
		}

		return context.DamageType switch
		{
			DamageType.Slashing or DamageType.Chopping or DamageType.Claw or DamageType.Shearing => new ScarDescriptor(
				"jagged slash scar",
				"jagged slash scar",
				"where a deep cut split the flesh and healed in an uneven seam."),
			DamageType.Piercing or DamageType.Ballistic or DamageType.ArmourPiercing or DamageType.BallisticArmourPiercing => new ScarDescriptor(
				"deep puncture scar",
				"deep puncture scar",
				"where a penetrating wound healed into a tight, puckered mark."),
			DamageType.Burning or DamageType.Chemical or DamageType.Electrical => new ScarDescriptor(
				"glossy burn scar",
				"glossy burn scar",
				"where heat or caustic injury tightened the flesh into a smooth, shiny patch."),
			DamageType.Crushing or DamageType.Falling or DamageType.Shockwave => new ScarDescriptor(
				"heavy crushed scar",
				"heavy crushed scar",
				"where the flesh was mashed and healed into a warped patch."),
			DamageType.Wrenching => new ScarDescriptor(
				"twisted wrench scar",
				"twisted wrench scar",
				"where the flesh was twisted and torn under strain and healed badly out of line."),
			DamageType.Bite or DamageType.Shrapnel => new ScarDescriptor(
				"ragged clustered scar",
				"ragged clustered scar",
				"where multiple tearing wounds healed together into a broken pattern."),
			DamageType.Freezing or DamageType.Necrotic or DamageType.Cellular or DamageType.Hypoxia => new ScarDescriptor(
				"puckered scar",
				"puckered scar",
				"where damaged flesh shrank back and healed into a drawn, uneven patch."),
			DamageType.Eldritch or DamageType.Arcane or DamageType.Sonic => new ScarDescriptor(
				"unnatural scar",
				"unnatural scar",
				"where strange trauma left the flesh healed in a subtly wrong pattern."),
			_ => new ScarDescriptor(
				"visible scar",
				"visible scar",
				"where a serious wound healed and left a lasting mark.")
		};
	}

	private static SeverityDescriptor GetSeverityDescriptor(WoundSeverity severity, bool isSurgical)
	{
		return severity switch
		{
			WoundSeverity.None => new SeverityDescriptor("a faint", "A faint"),
			WoundSeverity.Superficial => new SeverityDescriptor("a faint", "A faint"),
			WoundSeverity.Minor => new SeverityDescriptor("a slight", "A slight"),
			WoundSeverity.Small => new SeverityDescriptor("a narrow", "A narrow"),
			WoundSeverity.Moderate => new SeverityDescriptor("a noticeable", "A noticeable"),
			WoundSeverity.Severe => new SeverityDescriptor(isSurgical ? "a marked" : "a heavy", isSurgical ? "A marked" : "A heavy"),
			WoundSeverity.VerySevere => new SeverityDescriptor("a heavy", "A heavy"),
			WoundSeverity.Grievous => new SeverityDescriptor("a grievous", "A grievous"),
			WoundSeverity.Horrifying => new SeverityDescriptor("a horrific", "A horrific"),
			_ => new SeverityDescriptor("a visible", "A visible")
		};
	}

	private static string BuildShortDescription(SeverityDescriptor severityDescriptor, string shortBase)
	{
		return $"{severityDescriptor.ShortPrefix} {shortBase}";
	}

	private static int GetSizeSteps(ScarWoundContext context)
	{
		var sizeSteps = context.Severity switch
		{
			WoundSeverity.None => -2,
			WoundSeverity.Superficial => -2,
			WoundSeverity.Minor => -2,
			WoundSeverity.Small => -1,
			WoundSeverity.Moderate => -1,
			WoundSeverity.Severe => 0,
			WoundSeverity.VerySevere => 0,
			WoundSeverity.Grievous => 1,
			WoundSeverity.Horrifying => 1,
			_ => 0
		};

		sizeSteps += context.DamageType switch
		{
			DamageType.Piercing or DamageType.Ballistic or DamageType.ArmourPiercing or DamageType.BallisticArmourPiercing => -1,
			DamageType.Burning or DamageType.Chemical or DamageType.Crushing or DamageType.Wrenching => 1,
			_ => 0
		};

		return Math.Max(-2, Math.Min(2, sizeSteps));
	}

	private static int GetDistinctiveness(ScarWoundContext context)
	{
		var distinctiveness = context.Severity switch
		{
			WoundSeverity.None => 1,
			WoundSeverity.Superficial => 1,
			WoundSeverity.Minor => 1,
			WoundSeverity.Small => 2,
			WoundSeverity.Moderate => 2,
			WoundSeverity.Severe => 3,
			WoundSeverity.VerySevere => 4,
			WoundSeverity.Grievous => 5,
			WoundSeverity.Horrifying => 6,
			_ => 2
		};

		distinctiveness += context.DamageType switch
		{
			DamageType.Burning or DamageType.Chemical or DamageType.Wrenching => 1,
			DamageType.Piercing => -1,
			_ => 0
		};

		return Math.Max(1, distinctiveness);
	}

	private static (string? Plain, string? With) GetCharacteristicOverrides(
		string shapeName,
		IBodypart bodypart,
		ScarWoundContext context,
		int distinctiveness)
	{
		if (distinctiveness < 4)
		{
			return (null, null);
		}

		var normalised = shapeName.ToLowerInvariant();
		if (normalised.EqualToAny("eye", "nose", "mouth", "ear", "forehead", "cheek", "chin", "jaw", "scalp"))
		{
			return ("facially-scarred", "with facial scarring");
		}

		if (bodypart.Name.Contains("face", StringComparison.InvariantCultureIgnoreCase))
		{
			return ("facially-scarred", "with facial scarring");
		}

		return (null, null);
	}

	private static List<string> GetOrientationOptions(string profile)
	{
		return profile.ToLowerInvariant() switch
		{
			"ring" => ["runs around {0}", "cuts obliquely over {0}", "cuts crosswise over {0}"],
			"broad" => ["runs lengthwise along {0}", "cuts crosswise over {0}", "slashes diagonally across {0}", "sprawls across {0}", "fans outward over {0}"],
			"joint" => ["hooks across {0}", "cuts obliquely over {0}", "runs lengthwise beside {0}"],
			"facial" => ["slashes across {0}", "cuts diagonally over {0}", "marks one side of {0}"],
			"eye" => ["scores a crescent across {0}", "branches radially across {0}", "rides one side of {0}"],
			"nose" => ["runs down the bridge of {0}", "cuts crosswise over {0}", "rides one side of {0}"],
			"mouth" => ["runs along the line of {0}", "splits vertically through one side of {0}", "hooks from one corner of {0} inward"],
			"ear" => ["runs along the rim of {0}", "cuts across {0}", "runs downward toward the lobe of {0}"],
			"breast" => ["cuts directly across {0}", "rings {0} in a tight seam", "rides one side of {0}"],
			"groin" => ["runs down one side of {0}", "cuts low across {0}", "slashes diagonally over {0}"],
			_ => ["runs lengthwise along {0}", "cuts crosswise over {0}", "slashes diagonally across {0}", "curves across {0}"]
		};
	}

	private sealed class ScarGenerationChanceMatrixCache
	{
		public string? RawConfiguration { get; set; }
		public ScarGenerationChanceMatrix? Matrix { get; set; }
	}

	private sealed class ScarOrientationMappingCache
	{
		public string? RawConfiguration { get; set; }
		public ScarOrientationMapping? Mapping { get; set; }
	}

	private sealed record SeverityDescriptor(string ShortPrefix, string FullPrefix);
	private sealed record ScarDescriptor(string ShortBase, string FullBase, string CausePhrase);
}

internal sealed class ScarGenerationChanceMatrix
{
	private readonly IReadOnlyDictionary<DamageType, IReadOnlyDictionary<WoundSeverity, double>> _damageChanceMatrix;
	private readonly IReadOnlyDictionary<WoundSeverity, double> _surgeryChanceMatrix;

	private ScarGenerationChanceMatrix(
		IReadOnlyDictionary<DamageType, IReadOnlyDictionary<WoundSeverity, double>> damageChanceMatrix,
		IReadOnlyDictionary<WoundSeverity, double> surgeryChanceMatrix)
	{
		_damageChanceMatrix = damageChanceMatrix;
		_surgeryChanceMatrix = surgeryChanceMatrix;
	}

	internal double GetDamageChance(DamageType damageType, WoundSeverity severity)
	{
		if (_damageChanceMatrix.TryGetValue(damageType, out var values) &&
			values.TryGetValue(severity, out var chance))
		{
			return chance;
		}

		return 0.0;
	}

	internal double GetSurgeryChance(WoundSeverity severity)
	{
		return _surgeryChanceMatrix.TryGetValue(severity, out var chance) ? chance : 0.0;
	}

	internal static ScarGenerationChanceMatrix Parse(string xml)
	{
		var root = XElement.Parse(xml);
		var damageMatrix = new Dictionary<DamageType, IReadOnlyDictionary<WoundSeverity, double>>();
		foreach (var damageElement in root.Elements("Damage"))
		{
			var damageType = ParseEnumAttribute<DamageType>(damageElement, "Type");
			var severities = damageElement.Elements("Chance")
									 .ToDictionary(
										 x => ParseEnumAttribute<WoundSeverity>(x, "Severity"),
										 x => double.Parse(x.Value, CultureInfo.InvariantCulture));
			damageMatrix[damageType] = severities;
		}

		var surgeryMatrix = root.Element("Surgery")?
			.Elements("Chance")
			.ToDictionary(
				x => ParseEnumAttribute<WoundSeverity>(x, "Severity"),
				x => double.Parse(x.Value, CultureInfo.InvariantCulture)) ??
			new Dictionary<WoundSeverity, double>();

		return new ScarGenerationChanceMatrix(damageMatrix, surgeryMatrix);
	}

	private static T ParseEnumAttribute<T>(XElement element, string attributeName) where T : struct, Enum
	{
		var value = element.Attribute(attributeName)?.Value;
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ApplicationException($"Missing {attributeName} attribute in scar generation chance matrix.");
		}

		if (Enum.TryParse<T>(value, true, out var result))
		{
			return result;
		}

		if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integerValue))
		{
			return (T)Enum.ToObject(typeof(T), integerValue);
		}

		throw new ApplicationException($"The value {value} is not a valid {typeof(T).Name} in the scar generation chance matrix.");
	}
}

internal sealed class ScarOrientationMapping
{
	private readonly Dictionary<string, string> _shapeProfiles;
	private readonly string _defaultProfile;

	private ScarOrientationMapping(Dictionary<string, string> shapeProfiles, string defaultProfile)
	{
		_shapeProfiles = shapeProfiles;
		_defaultProfile = defaultProfile;
	}

	internal string ResolveProfile(string shapeName, string bodypartName, string fullDescription)
	{
		if (!string.IsNullOrWhiteSpace(shapeName) &&
			_shapeProfiles.TryGetValue(shapeName.Trim().ToLowerInvariant(), out var profile))
		{
			return profile;
		}

		var combinedText = $"{bodypartName} {fullDescription}".ToLowerInvariant();
		if (combinedText.Contains("eye"))
		{
			return "eye";
		}

		if (combinedText.Contains("nose"))
		{
			return "nose";
		}

		if (combinedText.Contains("mouth") || combinedText.Contains("lip"))
		{
			return "mouth";
		}

		if (combinedText.Contains("ear"))
		{
			return "ear";
		}

		return _defaultProfile;
	}

	internal static ScarOrientationMapping Parse(string xml)
	{
		var root = XElement.Parse(xml);
		var defaultProfile = root.Attribute("Default")?.Value ?? "linear";
		return new ScarOrientationMapping(
			root.Elements("Shape")
				.Where(x => x.Attribute("Name") is not null && x.Attribute("Profile") is not null)
				.ToDictionary(
					x => x.Attribute("Name")!.Value.Trim().ToLowerInvariant(),
					x => x.Attribute("Profile")!.Value.Trim(),
					StringComparer.InvariantCultureIgnoreCase),
			defaultProfile);
	}
}
