#nullable enable

using System.Globalization;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Health;

namespace MudSharp.Character;

public static partial class NpcArchiveReferencePolicy
{
	private static readonly string[] ArmourExpressionContainers =
	[
		"DissipateExpressions", "DissipateExpressionsPain", "DissipateExpressionsStun",
		"AbsorbExpressions", "AbsorbExpressionsPain", "AbsorbExpressionsStun"
	];
	private static readonly HashSet<string> ArmourFormulaParameters = new(
	[
		"quality", "damage", "stun", "pain", "originaldamage", "originalstun", "originalpain", "angle",
		"density", "electrical", "thermal", "organic", "strength"
	], StringComparer.OrdinalIgnoreCase);

	// ArmourType's codec contains damage/severity enums and formulas over scalar combat inputs.
	// It has no physical identity slot; unknown XML or formula inputs retain uncertainty.
	private static bool IsArmour(XElement root) => TryValidateArmour(root, out _);

	private static bool TryValidateArmour(XElement root, out string diagnostic)
	{
		diagnostic = "Unrecognized armour root, attributes, nodes or duplicate containers";
		if (root.Name.LocalName is not ("Definition" or "ArmourType") ||
		    !HasShape(root, root.Name.LocalName, [], [.. ArmourExpressionContainers, "DamageTransformations"])) return false;
		foreach (var container in root.Elements())
		{
			diagnostic = $"Invalid armour {container.Name.LocalName} shape, values, formulas or duplicate keys";
			if (container.Name.LocalName == "DamageTransformations")
			{
				if (!(HasShape(container, "DamageTransformations", [], ["Transform"], ["Transform"]) &&
				  container.Elements().All(transform => HasShape(transform, "Transform", ["fromtype", "totype", "severity"], []) &&
					  RequiredEnumInteger<DamageType>(transform, "fromtype") &&
					  RequiredEnumInteger<DamageType>(transform, "totype") &&
					  RequiredEnumInteger<WoundSeverity>(transform, "severity")) && UniqueIntegerKeys(container, "fromtype"))) return false;
				continue;
			}
			if (!HasShape(container, container.Name.LocalName, [], ["Expression"], ["Expression"])) return false;
			foreach (var element in container.Elements())
			{
				if (!TryValidateArmourExpression(element, out var reason))
				{
					var damageType = int.TryParse((string?)element.Attribute("damagetype"), NumberStyles.Integer,
						CultureInfo.InvariantCulture, out var parsed) ? parsed.ToString(CultureInfo.InvariantCulture) : "(invalid)";
					diagnostic = $"Invalid armour {container.Name.LocalName} damage type {damageType}: {reason}";
					return false;
				}
			}
			if (!UniqueIntegerKeys(container, "damagetype")) return false;
		}
		diagnostic = string.Empty;
		return true;
	}

	private static bool TryValidateArmourExpression(XElement element, out string diagnostic)
	{
		diagnostic = "unknown shape, missing/invalid enum or empty formula";
		if (element.Name != XName.Get("Expression") ||
		    element.Attributes().Any(x => x.Name != XName.Get("damagetype")) ||
		    element.Nodes().Any(x => x is not XText and not XComment) ||
		    !RequiredEnumInteger<DamageType>(element, "damagetype") || string.IsNullOrWhiteSpace(element.Value))
		{
			return false;
		}

		// Parse only: never execute formulas or consume randomness at an archival boundary.
		var expression = new Expression(element.Value);
		diagnostic = "invalid formula syntax";
		if (expression.HasErrors()) return false;
		diagnostic = "unrecognized formula parameter or function";
		if (!expression.ParameterNames.All(ArmourFormulaParameters.Contains) ||
		    !expression.FunctionNames.All(Expression.IsSupportedFunction)) return false;
		diagnostic = string.Empty;
		return true;
	}

	private static bool RequiredEnumInteger<T>(XElement element, string name) where T : struct, Enum =>
		int.TryParse((string?)element.Attribute(name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) &&
		Enum.IsDefined(typeof(T), value);

	private static bool UniqueIntegerKeys(XElement container, string name) => container.Elements()
		.Select(x => int.Parse(x.Attribute(name)!.Value, CultureInfo.InvariantCulture))
		.Distinct().Count() == container.Elements().Count();
}
