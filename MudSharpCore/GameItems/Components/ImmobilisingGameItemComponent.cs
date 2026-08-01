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

        foreach (var wound in WoundsCoveredByProfile(body, profile))
        {
            wound.ImmobilisingItem ??= Parent;
        }
    }

    internal static IEnumerable<IImmobilisableWound> WoundsCoveredByProfile(IBody body, IWearProfile profile)
    {
        var coveredBones = profile.Profile(body)
                                  .SelectMany(x => x.Key.BoneInfo.Keys)
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
            wound.ImmobilisingItem = null;
        }
    }

    #endregion
}
