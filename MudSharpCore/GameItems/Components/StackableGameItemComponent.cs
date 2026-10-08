using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class StackableGameItemComponent : GameItemComponent, IStackable
{
    private StackableGameItemComponentProto _prototype;

    public StackableGameItemComponent(StackableGameItemComponentProto proto, IGameItem parent,
        bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public StackableGameItemComponent(MudSharp.Models.GameItemComponent component,
        StackableGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public StackableGameItemComponent(StackableGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
        base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _quantity = rhs._quantity;
    }

    public override IGameItemComponentProto Prototype => _prototype;

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new StackableGameItemComponent(this, newParent, temporary);
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        return type == DescriptionType.Short
            ? _prototype.DescriptionDecorator.Describe(name, description, Quantity)
            : description;
    }

    public override int DecorationPriority => 0;

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Short;
    }

    public override double ComponentWeightMultiplier => Math.Max(1.0, Quantity);

    public override bool HandleDieOrMorph(IGameItem newItem, IRoom location)
    {
        StackableGameItemComponent newItemStackable = newItem?.GetItemType<StackableGameItemComponent>();
        if (newItemStackable == null)
        {
            return false;
        }

        newItemStackable.Quantity = Quantity;
        return false;
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (StackableGameItemComponentProto)newProto;
    }

    protected void LoadFromXml(XElement root)
    {
        XAttribute attribute = root.Attribute("Quantity");
        if (attribute != null)
        {
            _quantity = Convert.ToInt32(attribute.Value);
        }
    }

    protected override string SaveToXml()
    {
        return "<Definition Quantity=\"" + Quantity + "\"/>";
    }

    #region IStackable Members

    private int _quantity = 1;

	// Only the validated native Get merge may write the pair before publishing either change.
	internal void SetCommittedGetQuantity(int quantity) => _quantity = quantity;
	internal void MarkCommittedGetQuantityChanged() => Changed = true;
	internal void NotifyCommittedGetQuantityChanged() => HandleDescriptionUpdate();

    public int Quantity
    {
        get => _quantity;
        set
        {
			ForeignCustodyTransferContext.EnsureItem(Parent, destructive: true);
            _quantity = value;
            Changed = true;
            HandleDescriptionUpdate();
        }
    }

    public ItemGetResponse CanGet(int quantity)
    {
        return DropsWhole(quantity) ? ItemGetResponse.CanGet : ItemGetResponse.CanGetStack;
    }

    public IGameItem Get(int quantity)
    {
        return DropsWhole(quantity) ? Parent : Split(quantity);
    }

    public bool DropsWhole(int quantity)
    {
        return quantity == 0 || quantity >= Quantity;
    }

    public IGameItem Split(int quantity)
    {
		ForeignCustodyTransferContext.EnsureItem(Parent, destructive: true);
        // When splitting a stack, preserve the existing morph timer so that
        // partially purchased items do not reset their decay timers
        GameItem newItem = new((GameItem)Parent, temporary: false, preserveMorphTime: true);
        ((StackableGameItemComponent)newItem.GetItemType<IStackable>()).Quantity = quantity;
        ((GameItem)Parent).CopyStockDisplayToSplit(newItem);
        TransferWeaponPoisonCoatingToSplit(newItem, quantity);
        Quantity -= quantity;
        return newItem;
    }

	// Only the declared quiescent native ammunition leaf uses this prepared copy path.
	// Bind the exact copy and debit both raw quantities before any copy observer can throw.
	internal GameItem SplitPrepared(int quantity, Func<bool> factsUnchanged, Action<GameItem> capture)
	{
		var source = (GameItem)Parent;
		ForeignCustodyTransferContext.EnsureItem(source, destructive: true);
		var originalQuantity = Quantity;
		if (quantity <= 0 || quantity >= originalQuantity || !source.IsQuiescentNativeAmmoStack || !factsUnchanged())
			throw new InvalidOperationException("Prepared ammunition split changed before debit.");
		var newItem = new GameItem(source, false, true, copy =>
		{
			if (Quantity != originalQuantity || !source.IsQuiescentNativeAmmoStack || !factsUnchanged())
				throw new InvalidOperationException("Prepared ammunition split changed before debit.");
			var copyStack = copy.GetItemType<StackableGameItemComponent>();
			SetCommittedGetQuantity(originalQuantity - quantity);
			copyStack.SetCommittedGetQuantity(quantity);
			capture(copy);
			MarkCommittedGetQuantityChanged();
			copyStack.MarkCommittedGetQuantityChanged();
		});
		source.CopyStockDisplayToSplit(newItem);
		TransferWeaponPoisonCoatingToSplit(newItem, quantity, originalQuantity);
		NotifyCommittedGetQuantityChanged();
		newItem.GetItemType<StackableGameItemComponent>().NotifyCommittedGetQuantityChanged();
		return newItem;
	}

    private void TransferWeaponPoisonCoatingToSplit(GameItem newItem, int quantity, int? capturedQuantity = null)
    {
        var originalQuantity = Math.Max(1, capturedQuantity ?? Quantity);
        var proportion = Math.Clamp((double)quantity / originalQuantity, 0.0, 1.0);
        if (proportion <= 0.0)
        {
            return;
        }

        foreach (var copiedCoating in newItem.EffectsOfType<IWeaponPoisonCoatingEffect>().ToList())
        {
            newItem.RemoveEffect(copiedCoating, true);
        }

        foreach (var coating in Parent.EffectsOfType<IWeaponPoisonCoatingEffect>().ToList())
        {
            var amount = coating.ContaminatingLiquid.TotalVolume * proportion;
            if (amount <= 0.0)
            {
                continue;
            }

            var splitMixture = coating.RemovePoisonVolume(amount);
            if (splitMixture is null || splitMixture.IsEmpty)
            {
                continue;
            }

            var splitCoating = new WeaponPoisonCoating(newItem, splitMixture);
            var duration = Parent.ScheduledDuration(coating);
            newItem.AddEffect(splitCoating,
                duration > TimeSpan.Zero ? duration : WeaponPoisonCoating.EffectDuration(splitMixture));

            if (coating.ContaminatingLiquid.IsEmpty)
            {
                Parent.RemoveEffect(coating, true);
            }
        }
    }

    public IGameItem PeekSplit(int quantity)
    {
        GameItem newItem = new((GameItem)Parent, true);
        ((StackableGameItemComponent)newItem.GetItemType<IStackable>()).Quantity = Math.Min(quantity, Quantity);
        return newItem;
    }

    #endregion
}
