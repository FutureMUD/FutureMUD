using MudSharp.Body;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Health.Wounds;
using MudSharp.Models;

namespace MudSharp.Health;

public static class WoundFactory
{
	internal static IWound CopyToSeveredPart(IWound source, IGameItem owner, IGameItem lodgedCopy, bool temporary)
	{
		var (type, extras, actorOrigin, toolOrigin) = source switch
		{
			SimpleWound wound => ("Simple", wound.SaveExtras(), wound.StoredActorOriginId, wound.StoredToolOriginId),
			HealingSimpleWound wound => ("HealingSimple", wound.SaveExtras(), wound.StoredActorOriginId, wound.StoredToolOriginId),
			SimpleOrganicWound wound => ("SimpleOrganic", wound.SaveExtras(), wound.StoredActorOriginId, wound.StoredToolOriginId),
			BoneFracture wound => ("BoneFracture", wound.SaveExtras(), wound.StoredActorOriginId, wound.StoredToolOriginId),
			RobotWound wound => ("Robot", wound.SaveExtras(), wound.StoredActorOriginId, wound.StoredToolOriginId),
			_ => throw new InvalidOperationException($"Unknown wound type for severed remains: {source.GetType().Name}.")
		};
		var copy = LoadWound(new Wound
		{
			WoundType = type,
			ExtraInformation = extras,
			BodypartProtoId = source.Bodypart?.Id,
			OriginalDamage = source.OriginalDamage,
			CurrentDamage = source.CurrentDamage,
			CurrentPain = source is SimpleOrganicWound organic ? organic.UnmodifiedPain : source.CurrentPain,
			CurrentShock = source.CurrentShock,
			CurrentStun = source.CurrentStun,
			DamageType = (int)source.DamageType,
			Internal = source.Internal,
			ActorOriginId = actorOrigin,
			ToolOriginId = toolOrigin,
			RealTimeOfWound = source.RealTimeOfWound
		}, owner, owner.Gameworld);
		copy.Lodged = lodgedCopy;
		copy.SeveredBodypart = source.SeveredBodypart;

		var initialising = (ILateInitialisingItem)copy;
		initialising.SetNoSave(temporary);
		if (!temporary) owner.Gameworld.SaveManager.AddInitialisation(initialising);
		return copy;
	}

    public static IWound LoadWound(Wound wound, IHaveWounds owner, IFuturemud gameworld, IBody ownerBody = null)
    {
        switch (wound.WoundType)
        {
            case "HealingSimple":
                return new HealingSimpleWound(owner, wound, gameworld, ownerBody);
            case "Simple":
                return new SimpleWound(owner, wound, gameworld, ownerBody);
            case "SimpleOrganic":
                return new SimpleOrganicWound(owner, wound, gameworld, ownerBody);
            case "BoneFracture":
                return new BoneFracture(owner, wound, gameworld, ownerBody);
            case "Robot":
                return new RobotWound(owner, wound, gameworld, ownerBody);
        }

        throw new ApplicationException("Unknown WoundType in WoundFactory.LoadWound - " + wound.WoundType);
    }
}
