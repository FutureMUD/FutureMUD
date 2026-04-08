#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.RPG.Checks;

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
	private static readonly ConditionalWeakTable<IFuturemud, ScarGenerationChanceMatrixCache> ChanceMatrixCache = new();

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

	private sealed class ScarGenerationChanceMatrixCache
	{
		public string? RawConfiguration { get; set; }
		public ScarGenerationChanceMatrix? Matrix { get; set; }
	}
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

internal static class ScarTemplateIndex
{
	private static readonly ConditionalWeakTable<IFuturemud, ScarTemplateIndexCache> Cache = new();

	internal static void Invalidate(IFuturemud gameworld)
	{
		Cache.GetOrCreateValue(gameworld).IsDirty = true;
	}

	internal static ScarTemplateIndexSnapshot GetSnapshot(IFuturemud gameworld)
	{
		var cache = Cache.GetOrCreateValue(gameworld);
		if (!cache.IsDirty && cache.Snapshot is not null)
		{
			return cache.Snapshot;
		}

		cache.Snapshot = new ScarTemplateIndexSnapshot(gameworld.DisfigurementTemplates
			.OfType<IScarTemplate>()
			.Where(x => x.Status == RevisionStatus.Current)
			.ToList());
		cache.IsDirty = false;
		return cache.Snapshot;
	}

	private sealed class ScarTemplateIndexCache
	{
		public bool IsDirty { get; set; } = true;
		public ScarTemplateIndexSnapshot? Snapshot { get; set; }
	}
}

internal sealed class ScarTemplateIndexSnapshot
{
	private readonly Dictionary<long, List<IScarTemplate>> _shapeBuckets = new();
	private readonly List<IScarTemplate> _wildcardShapeTemplates = [];
	private readonly Dictionary<DamageType, List<IScarTemplate>> _damageBuckets = new();
	private readonly Dictionary<SurgicalProcedureType, List<IScarTemplate>> _surgeryBuckets = new();

	internal ScarTemplateIndexSnapshot(IEnumerable<IScarTemplate> templates)
	{
		foreach (var template in templates)
		{
			IndexTemplateByShape(template);
			IndexTemplateByDamage(template);
			IndexTemplateBySurgery(template);
		}
	}

	internal IEnumerable<IScarTemplate> GetCandidates(IBody body, IBodypart bodypart, ScarWoundContext context)
	{
		var candidates = GetShapeCandidates(bodypart)
			.Intersect(context.IsSurgery ? GetSurgeryCandidates(context) : GetDamageCandidates(context))
			.Where(x => x.CanBeAppliedToBodypart(body, bodypart))
			.Where(x => context.IsSurgery
				? context.SurgicalProcedureType.HasValue && x.CanBeAppliedFromSurgery(context.SurgicalProcedureType.Value)
				: x.CanBeAppliedFromDamage(context.DamageType, context.Severity))
			.ToList();
		return candidates;
	}

	private IEnumerable<IScarTemplate> GetShapeCandidates(IBodypart bodypart)
	{
		var results = new HashSet<IScarTemplate>(_wildcardShapeTemplates);
		if (bodypart.Shape is not null && _shapeBuckets.TryGetValue(bodypart.Shape.Id, out var templates))
		{
			results.UnionWith(templates);
		}

		return results;
	}

	private IEnumerable<IScarTemplate> GetDamageCandidates(ScarWoundContext context)
	{
		return _damageBuckets.TryGetValue(context.DamageType, out var templates)
			? templates
			: Enumerable.Empty<IScarTemplate>();
	}

	private IEnumerable<IScarTemplate> GetSurgeryCandidates(ScarWoundContext context)
	{
		if (!context.SurgicalProcedureType.HasValue)
		{
			return Enumerable.Empty<IScarTemplate>();
		}

		return _surgeryBuckets.TryGetValue(context.SurgicalProcedureType.Value, out var templates)
			? templates
			: Enumerable.Empty<IScarTemplate>();
	}

	private void IndexTemplateByShape(IScarTemplate template)
	{
		if (!template.BodypartShapes.Any())
		{
			_wildcardShapeTemplates.Add(template);
			return;
		}

		foreach (var shape in template.BodypartShapes)
		{
			if (!_shapeBuckets.TryGetValue(shape.Id, out var templates))
			{
				templates = [];
				_shapeBuckets[shape.Id] = templates;
			}

			templates.Add(template);
		}
	}

	private void IndexTemplateByDamage(IScarTemplate template)
	{
		foreach (var damageType in Enum.GetValues<DamageType>())
		{
			if (!template.CanBeAppliedFromDamage(damageType, WoundSeverity.Horrifying))
			{
				continue;
			}

			if (!_damageBuckets.TryGetValue(damageType, out var templates))
			{
				templates = [];
				_damageBuckets[damageType] = templates;
			}

			templates.Add(template);
		}
	}

	private void IndexTemplateBySurgery(IScarTemplate template)
	{
		foreach (var surgeryType in Enum.GetValues<SurgicalProcedureType>())
		{
			if (!template.CanBeAppliedFromSurgery(surgeryType))
			{
				continue;
			}

			if (!_surgeryBuckets.TryGetValue(surgeryType, out var templates))
			{
				templates = [];
				_surgeryBuckets[surgeryType] = templates;
			}

			templates.Add(template);
		}
	}
}
