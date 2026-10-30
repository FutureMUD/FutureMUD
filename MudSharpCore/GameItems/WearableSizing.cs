#nullable enable
using System.Globalization;
using System.Text.RegularExpressions;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.GameItems.Inventory.Size;

namespace MudSharp.GameItems;

/// <summary>Shared fitting syntax for commands, stock and item creation. No world entities are allocated here.</summary>
public static class WearableSizing
{
	// Consume complete options before selecting reserved keys, so quoted characteristic text stays opaque.
	private static readonly Regex LoadOption = new(@"(?<!\S)(?<key>[^\s:=]+)\s*[:=]\s*(?:""(?<value>[^""]*)""|(?<value>[^\s""]+))|(?<!\S)(?:""[^""]*""|\S+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
	private static bool IsSizing(Match match) => match.Groups["key"].Value.EqualTo("wearsize");

	public static bool TryParse(string text, ITarget? targeter, IFuturemud world, out WearableFit fit, out string error)
	{
		fit = WearableFit.Unfitted;
		error = "Specify a nonnegative size, for <person>, height <length> weight <mass>, unfitted or universal.";
		text = text.Trim();
		if (text.EqualTo("unfitted")) { error = ""; return true; }
		if (text.EqualTo("universal")) { fit = WearableFit.Universal; error = ""; return true; }
		if (text.StartsWith("for ", StringComparison.OrdinalIgnoreCase))
		{
			var name = text[4..].Trim();
			var body = name.EqualTo("self") ? (targeter as ICharacter)?.Body ?? targeter as IBody : targeter?.TargetBody(name);
			if (body is null) { error = "There is no such individual to fit this for."; return false; }
			fit = WearableFit.ForBody(body);
			error = "";
			return true;
		}
		if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var scalar) && double.IsFinite(scalar) && scalar >= 0)
		{
			fit = WearableFit.ForScalar(scalar); error = ""; return true;
		}
		// Stored snapshots use native base units and invariant culture, independent of the builder's units.
		if (text.StartsWith("measurements:", StringComparison.OrdinalIgnoreCase))
		{
			var values = text[13..].Split(',');
			if (values.Length == 2 && double.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var h) &&
			    double.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var w) &&
			    double.IsFinite(h) && double.IsFinite(w) && h > 0 && w > 0)
			{ fit = WearableFit.ForMeasurements(h, w); error = ""; return true; }
			return false;
		}
		var match = Regex.Match(text, @"^height\s+(?<height>.+?)\s+weight\s+(?<weight>.+)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		if (match.Success)
		{
			var height = world.UnitManager.GetBaseUnits(match.Groups["height"].Value, UnitType.Length, out var heightOk);
			var weight = world.UnitManager.GetBaseUnits(match.Groups["weight"].Value, UnitType.Mass, out var weightOk);
			if (heightOk && weightOk && double.IsFinite(height) && double.IsFinite(weight) && height > 0 && weight > 0)
			{ fit = WearableFit.ForMeasurements(height, weight); error = ""; return true; }
		}
		return false;
	}

	public static string ToLoadValue(WearableFit fit) => fit.Kind switch
	{
		WearableFitKind.Measurements => $"measurements:{fit.Height.ToString("R", CultureInfo.InvariantCulture)},{fit.Weight.ToString("R", CultureInfo.InvariantCulture)}",
		WearableFitKind.Scalar => fit.Scalar.ToString("R", CultureInfo.InvariantCulture),
		WearableFitKind.Universal => "universal",
		_ => "unfitted"
	};

	public static string NormaliseLoadArguments(string args, ITarget? targeter, IFuturemud world, IGameItemProto? prototype = null)
	{
		if (prototype is not null && HasLoadArguments(args) && prototype.GetItemType<WearableGameItemComponentProto>() is null)
			throw new ArgumentException("Sizing arguments require a wearable item.", nameof(args));
		return LoadOption.Replace(args ?? "", match =>
		{
			if (!IsSizing(match))
			{
				if (Regex.IsMatch(match.Value, @"^wearsize\s*[:=]", RegexOptions.IgnoreCase)) throw new ArgumentException("The wearsize option needs a complete value.", nameof(args));
				return match.Value;
			}
			if (!TryParse(match.Groups["value"].Value, targeter, world, out var fit, out var error)) throw new ArgumentException(error, nameof(args));
			if (match.Groups["value"].Value.StartsWith("for ", StringComparison.OrdinalIgnoreCase) && prototype?.GetItemType<WearableGameItemComponentProto>()?.ScalarSizeProg is { } scalarProg)
			{
				var name = match.Groups["value"].Value[4..].Trim();
				var body = name.EqualTo("self") ? (targeter as ICharacter)?.Body ?? targeter as IBody : targeter?.TargetBody(name);
				var executor = body!.Actor;
				var scalar = scalarProg.ExecuteDouble(executor);
				if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(executor) || !ReferenceEquals(body.Actor, executor))
					throw new ArgumentException("The fitting individual changed while calculating the size.", nameof(args));
				fit = WearableFit.ForScalar(scalar);
			}
			return $"wearsize={ToLoadValue(fit)}";
		});
	}

	public static void ApplyLoadArguments(IGameItem item, string args, ITarget? targeter)
	{
		var matches = LoadOption.Matches(args ?? "").Where(IsSizing).ToArray();
		if (matches.Length == 0) return;
		var wearable = item.GetItemType<IWearable>() ?? throw new ArgumentException("This item is not wearable.", nameof(args));
		var value = matches[^1].Groups["value"].Value;
		if (!TryParse(value, targeter, item.Gameworld, out var fit, out var error)) throw new ArgumentException(error, nameof(args));
		if (value.StartsWith("for ", StringComparison.OrdinalIgnoreCase))
		{
			var name = value[4..].Trim();
			var body = name.EqualTo("self") ? (targeter as ICharacter)?.Body ?? targeter as IBody : targeter?.TargetBody(name);
			fit = wearable.FitFor(body!);
		}
		wearable.SetFit(fit);
	}

	public static bool HasLoadArguments(string args) => LoadOption.Matches(args ?? "").Any(IsSizing);

	public static string RemoveLoadArguments(string args) => LoadOption.Replace(args ?? "", match => IsSizing(match) ? "" : match.Value);

	public static string Describe(IWearable wearable, IPerceiver voyeur)
	{
		var fit = wearable.Fit;
		var description = fit.Kind switch
		{
			WearableFitKind.Unfitted => "It has not been fitted.",
			WearableFitKind.Universal => "It is universally sized.",
			WearableFitKind.Scalar => $"It is size {fit.Scalar.ToString("N0", voyeur).ColourValue()}.",
			_ => $"It is fitted for someone {voyeur.Gameworld.UnitManager.DescribeMostSignificant(fit.Height, UnitType.Length, voyeur).ColourValue()} tall and {voyeur.Gameworld.UnitManager.DescribeMostSignificant(fit.Weight, UnitType.Mass, voyeur).ColourValue()} in weight."
		};
		return description + (voyeur is ICharacter ch && wearable.UsesSizing ? (wearable.Fits(ch.Body) ? " It fits you." : " It does not fit you.") : "");
	}
}
