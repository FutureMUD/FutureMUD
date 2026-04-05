using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Economy.Estates;

public static class MorgueService
{
    public static IEstate EnsureEstate(IEconomicZone zone, ICharacter deceased)
    {
        IEstate estate = deceased.Gameworld.Estates
            .Where(x => x.Character == deceased &&
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
        ICorpse corpse = corpseItem.GetItemType<ICorpse>();
        if (corpse == null)
        {
            return null;
        }

        IEstate estate = EnsureEstate(zone, corpse.OriginalCharacter);

        corpseItem.ContainedIn?.Take(corpseItem);
        corpseItem.InInventoryOf?.Take(corpseItem);
        corpseItem.Location?.Extract(corpseItem);
        corpseItem.RoomLayer = RoomLayer.GroundLevel;
        zone.MorgueStorageCell.Insert(corpseItem, true);

        if (!corpseItem.AffectedBy<MorgueStoredCorpse>())
        {
            corpseItem.AddEffect(new MorgueStoredCorpse(corpseItem, corpse.OriginalCharacter, estate, zone));
        }

        if (estate != null)
        {
            List<IGameItem> items = corpse.OriginalCharacter.Body.ExternalItems.ToList();
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
                IGameItem bundle = zone.MorgueStorageCell.GameItems.FirstOrDefault(x =>
                    x.EffectsOfType<MorgueBelongings>().Any(y =>
                        y.CharacterOwnerId == corpse.OriginalCharacter.Id &&
                        y.EstateId == estate.Id &&
                        y.EconomicZoneId == zone.Id));
                if (bundle == null)
                {
                    bundle = PileGameItemComponentProto.CreateNewBundle(strippedItems);
                    zone.Gameworld.Add(bundle);
                    bundle.AddEffect(new MorgueBelongings(bundle, corpse.OriginalCharacter, estate, zone));
                    zone.MorgueStorageCell.Insert(bundle, true);
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

            estate.OpenProbate();
        }

        return estate;
    }
}
