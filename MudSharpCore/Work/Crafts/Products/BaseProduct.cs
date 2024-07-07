using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Products;

public abstract class BaseProduct : LateInitialisingItem, ICraftProduct
{
	internal class SimpleProductData : ICraftProductDataWithItems
	{
		public List<IGameItem> Products { get; } = new();
		IEnumerable<IGameItem> ICraftProductDataWithItems.Products => Products;

		public IPerceivable Perceivable { get; }

		public void FinaliseLoadTimeTasks()
		{
			foreach (var product in Products)
			{
				product.FinaliseLoadTimeTasks();
			}
		}

		public XElement SaveToXml()
		{
			return new XElement("Data",
				from product in Products
				select new XElement("Item", product.Id)
			);
		}

		public SimpleProductData(IEnumerable<IGameItem> items)
		{
			Products.AddRange(items);
			Perceivable = new PerceivableGroup(Products);
		}

		public SimpleProductData(XElement root, IFuturemud gameworld)
		{
			foreach (var element in root.Elements("Item"))
			{
				var item = gameworld.TryGetItem(long.Parse(element.Value), true);
				if (item != null)
				{
					Products.Add(item);
				}
			}

			Perceivable = new PerceivableGroup(Products);
		}

		public void ReleaseProducts(ICell location, RoomLayer layer)
		{
			foreach (var item in Products)
			{
				item.RoomLayer = layer;
				location.Insert(item, true);
				item.HandleEvent(EventType.ItemFinishedLoading, item);
				item.Login();
			}
		}

		public void Delete()
		{
			foreach (var item in Products)
			{
				item.Delete();
			}
		}

		public void Quit()
		{
			foreach (var item in Products)
			{
				item.Quit();
			}
		}
	}

	public sealed override string FrameworkItemType => "CraftProduct";
	protected bool _failproduct;
	protected ICraft Craft { get; set; }

	protected BaseProduct(CraftProduct product, ICraft craft, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Craft = craft;
		_id = product.Id;
		IdInitialised = true;
		MaterialDefiningInputIndex = product.MaterialDefiningInputIndex;
		OriginalAdditionTime = product.OriginalAdditionTime;
		_failproduct = product.IsFailProduct;
	}

	protected BaseProduct(ICraft craft, IFuturemud gameworld, bool failproduct)
	{
		Gameworld = gameworld;
		Craft = craft;
		OriginalAdditionTime = DateTime.UtcNow;
		_failproduct = failproduct;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public DateTime OriginalAdditionTime { get; set; }

	public int? MaterialDefiningInputIndex { get; set; }

	public virtual bool RefersToItemProto(long id)
	{
		return false;
	}

	public virtual bool RefersToTag(ITag tag)
	{
		return false;
	}

	public virtual bool RefersToLiquid(ILiquid liquid)
	{
		return false;
	}

	public virtual bool IsItem(IGameItem item)
	{
		return false;
	}

	#region Implementation of ICraftProduct

	public abstract ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality);

	public abstract string HowSeen(IPerceiver voyeur);

	#endregion

	public abstract string ProductType { get; }

	#region Overrides of SaveableItem

	public bool ProductChanged
	{
		get => Changed;
		set
		{
			Changed = value;
			Craft.CalculateCraftIsValid();
		}
	}


	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbitem = FMDB.Context.CraftProducts.Find(Id);
		dbitem.Definition = SaveDefinition();
		dbitem.MaterialDefiningInputIndex = MaterialDefiningInputIndex;
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new CraftProduct();
		FMDB.Context.CraftProducts.Add(dbitem);
		dbitem.Definition = SaveDefinition();
		dbitem.ProductType = ProductType;
		dbitem.OriginalAdditionTime = OriginalAdditionTime;
		dbitem.CraftId = Craft.Id;
		dbitem.CraftRevisionNumber = Craft.RevisionNumber;
		dbitem.IsFailProduct = _failproduct;
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((CraftProduct)dbitem).Id;
	}

	#endregion

	protected abstract string SaveDefinition();

	protected virtual string BuildingHelpText => @"You can use the following items with this product:

	#3material <input index>#0 - sets a specific input to determine the material of this output";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Last.ToLowerInvariant())
		{
			case "material":
			case "mat":
			case "mats":
				return BuildingCommandMaterial(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which craft input index do you want to tie the material of this output to? Use NONE to clear it.");
			return false;
		}

		if (command.Peek().EqualToAny("none", "clear", "reset", "off"))
		{
			MaterialDefiningInputIndex = null;
			ProductChanged = true;
			actor.OutputHandler.Send(
				"This craft product will no longer use any input to determine its material, instead loading with the base material.");
			return true;
		}

		if (!int.TryParse(command.PopSpeech(), out var ivalue))
		{
			actor.OutputHandler.Send("Which input number do you want to pair this product with?");
			return false;
		}

		var input = Craft.Inputs.ElementAtOrDefault(ivalue - 1);
		if (input == null)
		{
			actor.OutputHandler.Send("There is no such input for this craft.");
			return false;
		}

		if (!(input is ICraftInputConsumesGameItem) && !(input is ICraftInputConsumesGameItemGroup))
		{
			actor.OutputHandler.Send(
				$"Input $i{ivalue} ({input.Name}) does not consume items, so cannot be used for determining material.");
			return false;
		}

		MaterialDefiningInputIndex = ivalue - 1;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now use input $i{ivalue} ({input.Name}) to determine which material it is made from.");
		return true;
	}

	public void CreateNewRevision(Models.Craft dbcraft, bool failproduct)
	{
		var dbproduct = new CraftProduct();
		dbcraft.CraftProducts.Add(dbproduct);
		dbproduct.ProductType = ProductType;
		dbproduct.OriginalAdditionTime = OriginalAdditionTime;
		dbproduct.IsFailProduct = failproduct;
		dbproduct.MaterialDefiningInputIndex = MaterialDefiningInputIndex;
		dbproduct.Definition = SaveDefinition();
	}

	public abstract bool IsValid();

	public abstract string WhyNotValid();

	protected ISolid DetermineOverrideMaterial(IActiveCraftGameItemComponent component)
	{
		if (!MaterialDefiningInputIndex.HasValue)
		{
			return null;
		}

		var input = Craft.Inputs.ElementAt(MaterialDefiningInputIndex.Value);
		var (consumed, _) = component.ConsumedInputs.ValueOrDefault(input, default);
		if (consumed == null)
		{
			return null;
		}

		if (input is ICraftInputConsumesGameItem)
		{
			return ((IGameItem)consumed).Material;
		}

		return ((PerceivableGroup)consumed).Members.OfType<IGameItem>().First().Material;
	}
}