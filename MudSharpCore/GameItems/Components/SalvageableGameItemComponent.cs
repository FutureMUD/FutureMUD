using MudSharp.Body.Traits;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems.Components;

public class SalvageableGameItemComponent : GameItemComponent, ISalvageable
{
	private SalvageableGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	public ITraitDefinition Trait => _prototype.Trait!;
	public Difficulty Difficulty => _prototype.Difficulty;
	public ITag? RequiredToolTag => _prototype.RequiredToolTag;
	public IInventoryPlanTemplate ToolTemplate => _prototype.ToolTemplate;
	public IEnumerable<(string Emote, double Delay)> Stages => _prototype.Stages;

	public SalvageableGameItemComponent(SalvageableGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SalvageableGameItemComponent(Models.GameItemComponent component,
		SalvageableGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	private SalvageableGameItemComponent(SalvageableGameItemComponent rhs, IGameItem newParent, bool temporary)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SalvageableGameItemComponentProto)newProto;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
		=> new SalvageableGameItemComponent(this, newParent, temporary);

	protected override string SaveToXml() => "<Definition />";

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type == DescriptionType.Full ? $"{description}\n\nIt can be salvaged." : description;
	}

	private double SourceBaseWeight => Parent.Prototype.Weight * Parent.Quantity;

	public double MaximumOutputWeight(bool success) => _prototype.MaximumOutputWeight(SourceBaseWeight, success);

	public bool CanSalvage(out string reason)
	{
		if (!_prototype.ConfigurationIsComplete(out reason))
		{
			return false;
		}

		if (Parent.Deleted)
		{
			reason = "it no longer exists";
			return false;
		}

		if (IndependentNestedItems().Any())
		{
			reason = "it still contains another item";
			return false;
		}

		if (Parent.GetItemTypes<ILiquidContainer>().Any(x => x.LiquidVolume > 1.0e-9))
		{
			reason = "it still contains liquid";
			return false;
		}

		if (MaximumOutputWeight(true) > SourceBaseWeight + 1.0e-9 ||
		    MaximumOutputWeight(false) > SourceBaseWeight + 1.0e-9)
		{
			reason = "its configured products exceed its source-base-mass budget";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private IEnumerable<IGameItem> IndependentNestedItems()
	{
		foreach (var item in Parent.DeepItems.Skip(1).Concat(Parent.AttachedAndConnectedItems))
		{
			yield return item;
		}

		foreach (var sheath in Parent.GetItemTypes<ISheath>())
		{
			if (sheath is IMultiSlotSheath multi)
			{
				foreach (var content in multi.WieldableContents)
				{
					yield return content.Parent;
				}
			}
			else if (sheath.Content is not null)
			{
				yield return sheath.Content.Parent;
			}
		}

		foreach (var item in Parent.GetItemTypes<IRangedWeaponPlatform>().SelectMany(x => x.AllContainedItems))
		{
			yield return item;
		}

		foreach (var item in Parent.GetItemTypes<IAutomationHousing>().SelectMany(x => x.ConcealedItems))
		{
			yield return item;
		}

		foreach (var mount in Parent.GetItemTypes<IArtilleryMount>())
		{
			if (mount.InstalledPiece is not null)
			{
				yield return mount.InstalledPiece;
			}
		}

		foreach (var carrier in Parent.GetItemTypes<IWeaponCarrierAttachment>())
		{
			if (carrier.AttachedWeapon is not null)
			{
				yield return carrier.AttachedWeapon;
			}
		}

		foreach (var chamber in Parent.GetItemTypes<IArtilleryChamber>())
		{
			if (chamber.LoadedAmmunition is not null)
			{
				yield return chamber.LoadedAmmunition.Parent;
			}
		}
	}

	public IReadOnlyList<IGameItem> CreateProducts(ICharacter actor, bool success)
	{
		var products = new List<IGameItem>();
		try
		{
			var plan = _prototype.CreateProductPlan(SourceBaseWeight, success);
			foreach (var (product, weight) in plan.Commodities)
			{
				if (CommodityGameItemComponentProto.ItemPrototype is null)
				{
					CommodityGameItemComponentProto.InitialiseItemType(Gameworld);
				}

				var item = CommodityGameItemComponentProto.CreateNewCommodity(product.Material, weight, product.Tag);
				products.Add(item);
				InitialiseProduct(actor, item);
			}

			foreach (var (product, quantity) in plan.Items)
			{
				var itemPrototype = product.ItemPrototype ?? throw new InvalidOperationException(
					$"Salvage item product #{product.ItemPrototypeId}r{product.ItemPrototypeRevision} was unresolved after validation.");
				if (itemPrototype.IsItemType<StackableGameItemComponentProto>())
				{
					var item = itemPrototype.CreateNew(actor);
					item.GetItemType<IStackable>().Quantity = quantity;
					products.Add(item);
					InitialiseProduct(actor, item);
					continue;
				}

				for (var i = 0; i < quantity; i++)
				{
					var item = itemPrototype.CreateNew(actor);
					products.Add(item);
					InitialiseProduct(actor, item);
				}
			}

			return products;
		}
		catch
			{
			foreach (var product in products.AsEnumerable().Reverse().Where(x => !x.Deleted))
			{
				try
				{
					product.Delete();
				}
				catch
				{
					// Preserve the original creation failure while making a best-effort rollback of every staged product.
				}
			}

			throw;
		}
	}

	private void InitialiseProduct(ICharacter actor, IGameItem item)
	{
		item.SetOwner(actor);
		Gameworld.Add(item);
		ButcherySpatialPlacement.Place(item, actor, Parent.RoomLayer, true);
		item.HandleEvent(EventType.ItemFinishedLoading, item);
		item.Login();
	}
}
