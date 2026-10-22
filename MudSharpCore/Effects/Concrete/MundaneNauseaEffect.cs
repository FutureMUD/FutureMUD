#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

/// <summary>A persistent timed physical cause of nausea, using the ordinary symptom system.</summary>
public sealed class MundaneNauseaEffect : Effect, ICauseDrugEffect
{
	public MundaneNauseaEffect(IBody owner, double intensity) : base(owner)
	{
		if (!double.IsFinite(intensity) || intensity < 0.0) throw new ArgumentOutOfRangeException(nameof(intensity));
		Intensity = intensity;
	}

	private MundaneNauseaEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var value = double.Parse(root.Element("Effect")!.Element("Intensity")!.Value, CultureInfo.InvariantCulture);
		Intensity = double.IsFinite(value) ? Math.Max(0.0, value) : 0.0;
	}

	/// <summary>Intensity per unit of body mass, before ordinary nausea resistance and neutralisation.</summary>
	public double Intensity { get; }
	public IEnumerable<DrugType> AffectedDrugTypes => [DrugType.Nausea];
	public double AddedIntensity(ICharacter character, DrugType drugtype) => drugtype == DrugType.Nausea
		? Intensity * ((IBody)Owner).Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001 : 0.0;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "MundaneNausea";
	public static void InitialiseEffectType() => RegisterFactory("MundaneNausea",
		(root, owner) => new MundaneNauseaEffect(root, owner));
	protected override XElement SaveDefinition() => new("Effect",
		new XElement("Intensity", Intensity.ToString(CultureInfo.InvariantCulture)));
	public override void Login() => ((IBody)Owner).RefreshNausea();
	public override string Describe(IPerceiver voyeur) => $"Physical nausea of intensity {Intensity.ToString("N2", voyeur)}.";
}
