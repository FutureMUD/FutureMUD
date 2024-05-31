using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

public abstract class BaseInput : LateInitialisingItem, ICraftInput
{
	public sealed override string FrameworkItemType => "CraftInput";

	private ICraft _craft;

	protected BaseInput(CraftInput input, ICraft craft, IFuturemud gameworld)
	{
		_id = input.Id;
		_craft = craft;
		IdInitialised = true;
		Gameworld = gameworld;
		InputQualityWeight = input.InputQualityWeight;
		OriginalAdditionTime = input.OriginalAdditionTime;
	}

	protected BaseInput(ICraft craft, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_craft = craft;
		InputQualityWeight = 1.0;
		OriginalAdditionTime = DateTime.UtcNow;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public DateTime OriginalAdditionTime { get; set; }

	#region Implementation of ICraftInput

	public double InputQualityWeight { get; set; }

	public abstract string HowSeen(IPerceiver voyeur);

	public abstract double ScoreInputDesirability(IPerceivable item);

	public abstract IEnumerable<IPerceivable> ScoutInput(ICharacter character);

	public abstract bool IsInput(IPerceivable item);

	public abstract void UseInput(IPerceivable item, ICraftInputData data);

	public abstract ICraftInputData ReserveInput(IPerceivable input);

	public abstract ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld);

	public void CreateNewRevision(MudSharp.Models.Craft dbcraft)
	{
		var dbinput = new CraftInput();
		dbcraft.CraftInputs.Add(dbinput);
		dbinput.InputQualityWeight = InputQualityWeight;
		dbinput.InputType = InputType;
		dbinput.OriginalAdditionTime = OriginalAdditionTime;
		dbinput.Definition = SaveDefinition();
	}

	protected abstract string BuildingHelpString { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Last.ToLowerInvariant())
		{
			case "weight":
			case "quality":
			case "qualityweight":
				return BuildingCommandQualityWeight(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpString.SubstituteANSIColour());
				return false;
		}
	}

	public abstract bool IsValid();
	public abstract string WhyNotValid();

	private bool BuildingCommandQualityWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send(
				"What weight do you want to give to the quality of this input in determining the overall quality of the output?");
			return false;
		}

		InputQualityWeight = value;
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input now has a weighting of {InputQualityWeight.ToString("N2", actor).Colour(Telnet.Green)} in determining the overall quality of the product.");
		return true;
	}

	protected abstract string InputType { get; }

	#region Overrides of SaveableItem

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbitem = FMDB.Context.CraftInputs.Find(Id);
		dbitem.Definition = SaveDefinition();
		dbitem.InputQualityWeight = InputQualityWeight;
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new CraftInput();
		FMDB.Context.CraftInputs.Add(dbitem);
		dbitem.Definition = SaveDefinition();
		dbitem.InputQualityWeight = InputQualityWeight;
		dbitem.InputType = InputType;
		dbitem.OriginalAdditionTime = OriginalAdditionTime;
		dbitem.CraftId = _craft.Id;
		dbitem.CraftRevisionNumber = _craft.RevisionNumber;
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((CraftInput)dbitem).Id;
	}

	#endregion

	protected abstract string SaveDefinition();

	#endregion

	public bool InputChanged
	{
		get => Changed;
		set
		{
			Changed = value;
			_craft.CalculateCraftIsValid();
		}
	}

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

	internal class SimpleItemInputData : ICraftInputData
	{
		public SimpleItemInputData(XElement root, IFuturemud gameworld)
		{
			if (root.Element("Consumed") != null)
			{
				// Legacy item
				var item = gameworld.TryGetItem(long.Parse(root.Element("Consumed").Value));
				ConsumedItems.Add(item);
			}
			else
			{
				foreach (var element in root.Element("ConsumedItems").Elements())
				{
					var item = gameworld.TryGetItem(long.Parse(element.Value));
					ConsumedItems.Add(item);
				}
			}

			ConsumedGroup = new PerceivableGroup(ConsumedItems);
			Quantity = int.Parse(root.Element("Quantity")?.Value ?? "1");
		}

		public SimpleItemInputData(IEnumerable<IGameItem> items, int quantity)
		{
			Quantity = quantity;
			ConsumeInput(items);
		}

		public XElement SaveToXml()
		{
			return new XElement("Data",
				new XElement("ConsumedItems",
					from item in ConsumedItems
					select new XElement("ConsumedItem", item.Id)
				),
				new XElement("Quantity", Quantity)
			);
		}

		public void FinaliseLoadTimeTasks()
		{
			foreach (var item in ConsumedItems)
			{
				item.FinaliseLoadTimeTasks();
			}
		}

		public ItemQuality InputQuality =>
			ConsumedItems.Select(x => (x.Quality, (double)x.Quantity)).GetNetQuality();

		public IPerceivable Perceivable => ConsumedGroup;

		public List<IGameItem> ConsumedItems { get; } = new();

		public PerceivableGroup ConsumedGroup { get; set; }

		public int Quantity { get; set; }

		public void ConsumeInput(IEnumerable<IGameItem> items)
		{
			var cumulativeQuantity = 0;
			foreach (var item in items)
			{
				var target = item.DropsWhole(Quantity - cumulativeQuantity)
					? item
					: item.Get(null, Quantity - cumulativeQuantity);
				item.InInventoryOf?.Take(target);
				item.Location?.Extract(target);
				item.ContainedIn?.Take(target);
				var connectable = target.GetItemType<IConnectable>();
				foreach (var attached in target.AttachedAndConnectedItems)
				{
					attached.GetItemType<IConnectable>()?.RawDisconnect(connectable, true);
				}

				cumulativeQuantity += target.Quantity;
				ConsumedItems.Add(target);
			}

			ConsumedGroup = new PerceivableGroup(ConsumedItems);
		}

		public void Delete()
		{
			foreach (var item in ConsumedItems.ToList())
			{
				item.Delete();
			}
		}

		public void Quit()
		{
			foreach (var item in ConsumedItems.ToList())
			{
				item.Quit();
			}
		}
	}
}