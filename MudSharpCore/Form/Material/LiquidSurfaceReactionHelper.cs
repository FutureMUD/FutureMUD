using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Material;

public static class LiquidSurfaceReactionHelper
{
    public static IEnumerable<IWound> ApplyToItem(IGameItem item, LiquidMixture mixture)
    {
        var options = EnvironmentalExposureOptions.Read(item.Gameworld);
        if (options.Mode == EnvironmentalExposureMode.Disabled || options.Mode == EnvironmentalExposureMode.Enabled && (!options.Items || !options.Liquids)) return Array.Empty<IWound>();
        List<IWound> wounds = new();
        List<ITag> tags = (item.Material as Framework.IHaveTags)?.Tags?.ToList() ?? new List<Framework.ITag>();
        if (!tags.Any())
        {
            return wounds;
        }

        foreach (LiquidInstance instance in mixture.Instances)
        {
            foreach (ILiquidSurfaceReaction reaction in LegacyRules(instance.Liquid, item.Gameworld).Where(x =>
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
        LiquidMixture mixture) => ApplyToBody(character.Body, bodyparts, mixture);

    public static IEnumerable<IWound> ApplyToBody(IBody body, IEnumerable<IExternalBodypart> bodyparts,
        LiquidMixture mixture)
    {
        var options = EnvironmentalExposureOptions.Read(body.Gameworld);
        if (options.Mode == EnvironmentalExposureMode.Disabled || options.Mode == EnvironmentalExposureMode.Enabled && (!options.Characters || !options.Liquids)) return Array.Empty<IWound>();
        List<IWound> wounds = new();
        foreach (IExternalBodypart bodypart in bodyparts)
        {
            List<ITag> tags = (body.GetMaterial(bodypart) as Framework.IHaveTags)?.Tags?.ToList() ??
                       new List<Framework.ITag>();
            if (!tags.Any())
            {
                continue;
            }

            foreach (LiquidInstance instance in mixture.Instances)
            {
                foreach (ILiquidSurfaceReaction reaction in LegacyRules(instance.Liquid, body.Gameworld).Where(x =>
                             x.TargetTags.Any(tag => tags.Any(y => y.IsA(tag)))))
                {
                    wounds.AddRange(body.PassiveSufferDamage(new Damage
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

	private static IEnumerable<ILiquidSurfaceReaction> LegacyRules(ILiquid liquid, IFuturemud world)
	{
		var mode = EnvironmentalExposureOptions.Read(world).Mode;
		foreach (var rule in liquid.SurfaceReactions)
		{
			if (rule is not LiquidSurfaceReaction reaction) { yield return rule; continue; }
			if (mode == EnvironmentalExposureMode.Legacy && reaction.Legacy is LiquidSurfaceReaction legacy)
			{
				if (!legacy.ValidationErrors(false).Any()) yield return legacy;
				continue;
			}
			if (reaction.ValidationErrors(false).Any()) continue;
			if (mode == EnvironmentalExposureMode.Enabled && reaction.Version == 1) yield return reaction;
		}
	}
}
