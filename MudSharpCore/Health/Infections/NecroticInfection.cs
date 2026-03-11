#nullable enable

using MudSharp.Body;
using MudSharp.RPG.Checks;

namespace MudSharp.Health.Infections;

public class NecroticInfection : InfectiousInfection
{
	public NecroticInfection(Difficulty virulenceDifficulty, double intensity, IBody owner, IWound wound,
		IBodypart bodypart, double virulence)
		: base(virulenceDifficulty, intensity, owner, wound, bodypart, virulence)
	{
	}

	public NecroticInfection(MudSharp.Models.Infection infection, IBody body, IWound wound, IBodypart bodypart)
		: base(infection, body, wound, bodypart)
	{
	}

	public override InfectionType InfectionType => InfectionType.Necrotic;

	public override bool InfectionIsDamaging()
	{
		return InfectionStage >= InfectionStage.StageFive && (Bodypart != null || Wound != null);
	}

	public override IDamage GetInfectionDamage()
	{
		return new Damage
		{
			Bodypart = Bodypart ?? Wound?.Bodypart,
			DamageType = DamageType.Necrotic,
			DamageAmount = Intensity * Gameworld.GetStaticDouble("NecroticInfectionDamagePerIntensity")
		};
	}
}
