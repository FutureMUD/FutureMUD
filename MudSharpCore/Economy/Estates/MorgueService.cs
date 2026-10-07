using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Economy.Estates;

public static class MorgueService
{
    public static IEstate EnsureEstate(IEconomicZone zone, ICharacter deceased)
    {
        IEstate estate = deceased.Gameworld.Estates
            .Where(x => CharacterInstanceIdentityComparer.SameIdentity(x.Character, deceased) &&
                        x.EconomicZone == zone &&
                        x.EstateStatus != EstateStatus.Cancelled &&
                        x.EstateStatus != EstateStatus.Finalised)
            .OrderBy(x => x.EstateStartTime)
            .FirstOrDefault();
        if (estate != null)
        {
            return estate;
        }

        if (!zone.EstatesEnabled)
        {
            return null;
        }

        return Estate.CreateEstatesForCharacterDeath(deceased)
            .FirstOrDefault(x => x.EconomicZone == zone);
    }

    public static IEstate IntakeCorpse(IEconomicZone zone, IGameItem corpseItem)
    {
		TryIntakeCorpse(zone, corpseItem, out var estate);
		return estate;
	}

	/// <summary>Returns true for successful custody, including intakes that do not create an estate.</summary>
	public static bool TryIntakeCorpse(IEconomicZone zone, IGameItem corpseItem, out IEstate estate)
	{
		estate = null;
        ICorpse corpse = corpseItem.GetItemType<ICorpse>();
		if (corpse is not { RepresentsFinalCharacterDeath: true } || corpse.Body is not { } body ||
		    corpse.GetOriginalCharacterWithMatchingBody() is not { } deceased)
        {
			return false;
        }

		estate = EnsureEstate(zone, deceased);

        corpseItem.ContainedIn?.Take(corpseItem);
        corpseItem.InInventoryOf?.Take(corpseItem);
        corpseItem.Location?.Extract(corpseItem);
        corpseItem.RoomLayer = RoomLayer.GroundLevel;
        zone.MorgueStorageRoom.Insert(corpseItem, true);

        if (!corpseItem.AffectedBy<MorgueStoredCorpse>())
        {
            corpseItem.AddEffect(new MorgueStoredCorpse(corpseItem, corpse.OriginalCharacter, estate, zone));
        }

        List<IGameItem> items = body.ExternalItems.ToList();
        List<IGameItem> strippedItems = new();
        foreach (IGameItem item in items)
        {
            if (!corpse.Take(item))
            {
                continue;
            }

            strippedItems.Add(item);
        }

        if (strippedItems.Any())
        {
			var estateId = estate?.Id ?? 0;
            IGameItem bundle = zone.MorgueStorageRoom.GameItems.FirstOrDefault(x =>
                x.EffectsOfType<MorgueBelongings>().Any(y =>
                    y.CharacterOwnerId == CharacterInstanceIdentityComparer.IdentityId(corpse.OriginalCharacter) &&
                    y.EstateId == estateId &&
                    y.EconomicZoneId == zone.Id));
            if (bundle == null)
            {
                bundle = PileGameItemComponentProto.CreateNewBundle(strippedItems);
                zone.Gameworld.Add(bundle);
                bundle.AddEffect(new MorgueBelongings(bundle, corpse.OriginalCharacter, estate, zone));
                zone.MorgueStorageRoom.Insert(bundle, true);
            }
            else
            {
                IContainer container = bundle.GetItemType<IContainer>();
                foreach (IGameItem item in strippedItems)
                {
                    container.Put(null, item);
                }
            }
        }

        if (estate != null)
        {
            estate.OpenProbate();
        }

		return true;
    }
}
