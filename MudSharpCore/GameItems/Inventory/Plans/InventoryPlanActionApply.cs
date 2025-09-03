using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionApply : InventoryPlanAction
{
    public InventoryPlanActionApply(XElement root, IFuturemud gameworld)
        : base(root, gameworld, DesiredItemState.Apply)
    {
        Grams = double.Parse(root.Attribute("grams").Value);
        Bodypart = root.Attribute("bodypart").Value;
    }

    public InventoryPlanActionApply(IFuturemud gameworld, long primaryTag, long secondaryTag,
        Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector,
        string bodypart, double grams)
        : base(gameworld, DesiredItemState.Apply, primaryTag, secondaryTag, primaryselector, secondaryselector)
    {
        Bodypart = bodypart;
        Grams = grams;
    }

    public string Bodypart { get; }
    public double Grams { get; }

    #region Overrides of InventoryPlanAction

    public override XElement SaveToXml()
    {
        return new XElement("Action",
            new XAttribute("state", "apply"),
            new XAttribute("tag", DesiredTag?.Id ?? 0),
            new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
            new XAttribute("bodypart", Bodypart),
            new XAttribute("grams", Grams),
            new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
            new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
            new XAttribute("originalreference", OriginalReference?.ToString() ?? "")
        );
    }

    public override string Describe(ICharacter voyeur)
    {
        return
            $"Apply {Grams.ToString("N2", voyeur)}g of {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"} to {Bodypart.Colour(Telnet.Yellow)}";
    }

    public override bool RequiresFreeHandsToExecute(ICharacter who, IGameItem item)
    {
        return false;
    }

    #endregion

    public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
    {
        return null;
    }

    public override IGameItem ScoutTarget(ICharacter executor)
    {
        var part = executor.Body.GetTargetBodypart(Bodypart);
        if (part == null)
        {
            return null;
        }

        IGameItem item = null;

        item = executor.Body.HeldItems.FirstOrDefault(x =>
            x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
            x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        if (item != null)
        {
            return item;
        }

        item = executor.Body.WieldedItems.FirstOrDefault(x =>
            x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
            x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        if (item != null)
        {
            return item;
        }

        item = executor.Body.WornItems.FirstOrDefault(x =>
            x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
            executor.Body.CanRemoveItem(x) &&
            x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        if (item != null)
        {
            return item;
        }

        item = executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
            .Select(x =>
                x.ConnectedItems.FirstOrDefault(y =>
                    y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
                    y.Parent.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply)?.Parent)
            .FirstOrDefault(x => x != null);
        if (item != null)
        {
            return item;
        }

        item = executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
            .SelectNotNull(x => x.Content?.Parent)
            .FirstOrDefault(x =>
                x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
                x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        if (item != null)
        {
            return item;
        }

        item = executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>())
            .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
            .SelectMany(x => x.Contents)
            .FirstOrDefault(x =>
                x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
                x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        if (item != null)
        {
            return item;
        }

        item = executor.Location.LayerGameItems(executor.RoomLayer).FirstOrDefault(x =>
            x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
            x.IsItemType<IHoldable>() &&
            x.GetItemType<IHoldable>().IsHoldable &&
            x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        if (item != null)
        {
            return item;
        }

        item = executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IBelt>())
            .Select(x =>
                x.ConnectedItems.FirstOrDefault(y =>
                    y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
                    y.Parent.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply)?.Parent)
            .FirstOrDefault(x => x != null);
        if (item != null)
        {
            return item;
        }

        item = executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<ISheath>())
            .SelectNotNull(x => x.Content?.Parent)
            .FirstOrDefault(x =>
                x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
                x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        if (item != null)
        {
            return item;
        }

        item = executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IContainer>())
            .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
            .SelectMany(x => x.Contents)
            .FirstOrDefault(x =>
                x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
                x.GetItemType<IApply>()?.CanApply(executor.Body, part) == WhyCannotApply.CanApply);
        return item;
    }
}
