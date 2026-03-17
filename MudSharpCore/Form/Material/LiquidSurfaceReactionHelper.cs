using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Material;

public static class LiquidSurfaceReactionHelper
{
	public static IEnumerable<IWound> ApplyToItem(IGameItem item, LiquidMixture mixture)
	{
		var wounds = new List<IWound>();
		var tags = (item.Material as Framework.IHaveTags)?.Tags?.ToList() ?? new List<Framework.ITag>();
		if (!tags.Any())
		{
			return wounds;
		}

		foreach (var instance in mixture.Instances)
		{
			foreach (var reaction in instance.Liquid.SurfaceReactions.Where(x =>
				         x.TargetTags.Any(tag => tags.Any(y => y.IsA(tag)))))
			{
				wounds.AddRange(item.PassiveSufferDamage(new Damage
				{
					ActorOrigin = null,
					DamageAmount = reaction.DamagePerTick * instance.Amount,
					PainAmount = reaction.PainPerTick * instance.Amount,
					StunAmount = reaction.StunPerTick * instance.Amount,
					ShockAmount = 0.0,
					DamageType = reaction.DamageType,
					AngleOfIncidentRadians = System.Math.PI * 0.5,
					PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass }
				}));
			}
		}

		return wounds;
	}

	public static IEnumerable<IWound> ApplyToCharacter(ICharacter character, IEnumerable<IExternalBodypart> bodyparts,
		LiquidMixture mixture)
	{
		var wounds = new List<IWound>();
		foreach (var bodypart in bodyparts)
		{
			var tags = (character.Body.GetMaterial(bodypart) as Framework.IHaveTags)?.Tags?.ToList() ??
			           new List<Framework.ITag>();
			if (!tags.Any())
			{
				continue;
			}

			foreach (var instance in mixture.Instances)
			{
				foreach (var reaction in instance.Liquid.SurfaceReactions.Where(x =>
					         x.TargetTags.Any(tag => tags.Any(y => y.IsA(tag)))))
				{
					wounds.AddRange(character.PassiveSufferDamage(new Damage
					{
						ActorOrigin = null,
						Bodypart = bodypart,
						DamageAmount = reaction.DamagePerTick * instance.Amount,
						PainAmount = reaction.PainPerTick * instance.Amount,
						StunAmount = reaction.StunPerTick * instance.Amount,
						ShockAmount = 0.0,
						DamageType = reaction.DamageType,
						AngleOfIncidentRadians = System.Math.PI * 0.5,
						PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass }
					}));
				}
			}
		}

		return wounds;
	}
}
