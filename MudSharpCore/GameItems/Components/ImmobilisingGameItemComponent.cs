using MudSharp.Body;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health.Wounds;

namespace MudSharp.GameItems.Components;

public class ImmobilisingGameItemComponent : WearableGameItemComponent, IImmobilise
{
    #region Constructors

    public ImmobilisingGameItemComponent(ImmobilisingGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(proto, parent, null, temporary)
    {
    }

    public ImmobilisingGameItemComponent(MudSharp.Models.GameItemComponent component,
        ImmobilisingGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
    {
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public ImmobilisingGameItemComponent(ImmobilisingGameItemComponent rhs, IGameItem newParent, bool temporary = false)
        : base(rhs, newParent, temporary)
    {
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new ImmobilisingGameItemComponent(this, newParent, temporary);
    }

    public override void UpdateWear(IBody body, IWearProfile profile)
    {
        ClearImmobilisedWounds();
        base.UpdateWear(body, profile);

        if (body is null || profile is null)
        {
            return;
        }

        foreach (var wound in WoundsCoveredByWearLocations(
                     body,
                     body.WornItemsFullInfo
                         .Where(x => x.Item == Parent)
                         .Select(x => x.Wearloc)))
        {
            wound.ImmobilisingItem ??= Parent;
        }
    }

    internal static IEnumerable<IImmobilisableWound> WoundsCoveredByWearLocations(
        IBody body,
        IEnumerable<IWear> wearLocations)
    {
        var coveredBones = wearLocations
            .SelectMany(x => x.BoneInfo.Keys)
            .ToHashSet();
        return body.Wounds.OfType<IImmobilisableWound>()
            .Where(x => coveredBones.Contains(x.Bodypart));
    }

    private void ClearImmobilisedWounds()
    {
        if (WornBy is null)
        {
            return;
        }

        foreach (var wound in WornBy.Wounds.OfType<IImmobilisableWound>()
                                    .Where(x => x.ImmobilisingItem == Parent))
        {
            wound.ImmobilisingItem = FindReplacementImmobilisingItem(WornBy, wound, Parent);
        }
    }

    internal static IGameItem FindReplacementImmobilisingItem(
        IBody body,
        IImmobilisableWound wound,
        IGameItem removedItem)
    {
        foreach (var candidate in body.WornItemsFullInfo
                     .Where(x => x.Item != removedItem && x.Item.IsItemType<IImmobilise>())
                     .GroupBy(x => x.Item))
        {
            if (WoundsCoveredByWearLocations(body, candidate.Select(x => x.Wearloc)).Contains(wound))
            {
                return candidate.Key;
            }
        }

        return null;
    }

    #endregion
}
