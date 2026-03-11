#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Health.Infections;

public class FungalInfection : SimpleInfection
{
	public FungalInfection(Difficulty virulenceDifficulty, double intensity, IBody owner, IWound wound,
		IBodypart bodypart, double virulence)
		: base(virulenceDifficulty, intensity, owner, wound, bodypart, virulence)
	{
	}

	public FungalInfection(MudSharp.Models.Infection infection, IBody body, IWound wound, IBodypart bodypart)
		: base(infection, body, wound, bodypart)
	{
	}

	public override InfectionType InfectionType => InfectionType.FungalGrowth;

	protected override double GetIntensityGainMultiplier()
	{
		var multiplier = 1.0;
		if (IsRelevantBodypartWet())
		{
			multiplier *= Gameworld.GetStaticDouble("FungalInfectionWetProgressionMultiplier");
		}

		if (Owner.Actor.HealthStrategy.CurrentTemperatureStatus(Owner.Actor) >
		    BodyTemperatureStatus.NormalTemperature)
		{
			multiplier *= Gameworld.GetStaticDouble("FungalInfectionHotProgressionMultiplier");
		}

		return multiplier;
	}

	private bool IsRelevantBodypartWet()
	{
		var bodyparts = RelevantExternalBodyparts().ToList();
		if (!bodyparts.Any())
		{
			return Owner.SaturationLevel >= ItemSaturationLevel.Wet;
		}

		return Owner.SaturationLevelForLiquid(bodyparts) >= ItemSaturationLevel.Wet;
	}

	private IEnumerable<IExternalBodypart> RelevantExternalBodyparts()
	{
		if (Bodypart is IExternalBodypart externalBodypart)
		{
			yield return externalBodypart;
			yield break;
		}

		if (Bodypart is IOrganProto organ)
		{
			foreach (var item in Owner.Bodyparts.OfType<IExternalBodypart>().Where(x => x.Organs.Contains(organ)))
			{
				yield return item;
			}

			yield break;
		}

		if (Bodypart is IBone bone)
		{
			foreach (var item in Owner.Bodyparts.OfType<IExternalBodypart>().Where(x => x.Bones.Contains(bone)))
			{
				yield return item;
			}

			yield break;
		}
	}
}
