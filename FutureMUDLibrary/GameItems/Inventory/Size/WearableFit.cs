#nullable enable
using System;
using System.Xml.Linq;
using MudSharp.Body;

namespace MudSharp.GameItems.Inventory.Size;

public enum WearableFitKind { Unfitted, Universal, Measurements, Scalar }

/// <summary>An item's immutable fitting, independent of its current wearer or owner.</summary>
public sealed record WearableFit
{
	public WearableFitKind Kind { get; }
	public double Height { get; }
	public double Weight { get; }
	public double Scalar { get; }
	private WearableFit(WearableFitKind kind, double height = 0, double weight = 0, double scalar = 0)
	{
		Kind = kind;
		Height = height;
		Weight = weight;
		Scalar = scalar;
	}
	public static WearableFit Unfitted { get; } = new(WearableFitKind.Unfitted);
	public static WearableFit Universal { get; } = new(WearableFitKind.Universal);
	public static WearableFit ForBody(IBody body) => ForMeasurements(body.Height, body.Weight);
	public static WearableFit ForMeasurements(double height, double weight)
	{
		if (!double.IsFinite(height) || !double.IsFinite(weight) || height <= 0 || weight <= 0)
			throw new ArgumentOutOfRangeException(nameof(height), "Measurements must be finite and positive.");
		return new(WearableFitKind.Measurements, height, weight);
	}
	public static WearableFit ForScalar(double size)
	{
		if (!double.IsFinite(size) || size < 0) throw new ArgumentOutOfRangeException(nameof(size));
		return new(WearableFitKind.Scalar, scalar: size);
	}
	public XElement SaveToXml() => new("Fit", new XAttribute("kind", Kind),
		new XAttribute("height", Height), new XAttribute("weight", Weight), new XAttribute("scalar", Scalar));
	public static WearableFit FromXml(XElement? element)
	{
		if (element is null) return Unfitted;
		return Enum.Parse<WearableFitKind>((string)element.Attribute("kind")!) switch
		{
			WearableFitKind.Measurements => ForMeasurements((double)element.Attribute("height")!, (double)element.Attribute("weight")!),
			WearableFitKind.Scalar => ForScalar((double)element.Attribute("scalar")!),
			WearableFitKind.Universal => Universal,
			_ => Unfitted
		};
	}
	public bool MatchesMeasurements(IBody body, double heightTolerance, double weightTolerance) =>
		body.Height >= Height * (1.0 - heightTolerance) && body.Height <= Height * (1.0 + heightTolerance) &&
		body.Weight >= Weight * (1.0 - weightTolerance) && body.Weight <= Weight * (1.0 + weightTolerance);
}
