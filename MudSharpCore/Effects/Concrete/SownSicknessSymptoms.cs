#nullable enable

using MudSharp.Health.Infections;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

/// <summary>Native stamina fatigue and fever symptoms owned by one live Sown Sickness infection.</summary>
public sealed class SownSicknessSymptoms : ThermalImbalance
{
	public SownSicknessSymptoms(ICharacter owner, SownSicknessInfection infection, int grade) : base(owner)
	{
		Infection = infection;
		Grade = Math.Clamp(grade, 1, 7);
		TemperatureStatus = StatusForGrade(Grade);
	}

	internal SownSicknessSymptoms(IPerceivable owner, int grade) : base(owner)
	{
		Grade = Math.Clamp(grade, 1, 7);
		TemperatureStatus = StatusForGrade(Grade);
	}

	public SownSicknessInfection? Infection { get; }
	public int Grade { get; }
	public override bool SavingEffect => false;
	public override bool IsAmbientExposureDriven => false;
	protected override string SpecificEffectType => "SownSicknessSymptoms";
	public override double Multiplier => base.Multiplier * StaminaExpenditureMultiplier(Grade);
	protected override double StaminaRegenerationRateMultiplier => base.StaminaRegenerationRateMultiplier * StaminaRegenerationMultiplier(Grade);

	public static BodyTemperatureStatus StatusForGrade(int grade) => grade switch
	{
		>= 1 and <= 3 => BodyTemperatureStatus.VeryMildHyperthermia,
		>= 4 and <= 6 => BodyTemperatureStatus.MildHyperthermia,
		7 => BodyTemperatureStatus.ModerateHyperthermia,
		_ => throw new ArgumentOutOfRangeException(nameof(grade))
	};

	public static double StaminaExpenditureMultiplier(int grade) => 1.0 + Math.Clamp(grade, 1, 7) * 0.1;
	public static double StaminaRegenerationMultiplier(int grade) => 1.0 - Math.Clamp(grade, 1, 7) * 0.05;

	public override string Describe(IPerceiver voyeur) =>
		$"Suffering grade {Grade:N0} fatigue and {TemperatureStatus.DescribeEnum()} fever from {InfectionType.SownSickness.Describe()}.";
}
