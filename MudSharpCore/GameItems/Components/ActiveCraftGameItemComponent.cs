using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts;

namespace MudSharp.GameItems.Components;

public class ActiveCraftGameItemComponent : GameItemComponent, IActiveCraftGameItemComponent
{
	protected ActiveCraftGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)> ConsumedInputs { get; } = new();
	public Dictionary<ICraftProduct, ICraftProductData> ProducedProducts { get; } = new();
	public Dictionary<ICraftTool, (ItemQuality Quality, double Weight)> UsedToolQualities { get; } = new();

	public Outcome QualityCheckOutcome { get; set; } = Outcome.NotTested;

	public ICraft Craft { get; set; }
	private int _phase = 1;

	public int Phase
	{
		get => _phase;
		set
		{
			_phase = value;
			Changed = true;
		}
	}

	private bool _hasFailed;

	public bool HasFailed
	{
		get => _hasFailed;
		set
		{
			_hasFailed = value;
			Changed = true;
		}
	}

	private Outcome _checkOutcome = Outcome.NotTested;

	public Outcome CheckOutcome
	{
		get => _checkOutcome;
		set
		{
			_checkOutcome = value;
			Changed = true;
		}
	}

	public bool HasFinished { get; private set; }

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ActiveCraftGameItemComponentProto)newProto;
	}

	public override void Delete()
	{
		if (!HasFinished)
		{
			foreach (var item in ProducedProducts)
			{
				item.Value.Delete();
			}
		}

		foreach (var item in ConsumedInputs)
		{
			item.Value.Data.Delete();
		}
	}

	public override void Quit()
	{
		foreach (var item in ProducedProducts)
		{
			item.Value.Quit();
		}

		foreach (var item in ConsumedInputs)
		{
			item.Value.Data.Quit();
		}
	}

	#region Constructors

	public ActiveCraftGameItemComponent(ActiveCraftGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ActiveCraftGameItemComponent(MudSharp.Models.GameItemComponent component,
		ActiveCraftGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ActiveCraftGameItemComponent(ActiveCraftGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		Craft = rhs.Craft;
		_phase = rhs.Phase;
		_checkOutcome = rhs._checkOutcome;
		foreach (var input in rhs.ConsumedInputs)
		{
			ConsumedInputs[input.Key] =
				(input.Value.Input, input.Key.LoadDataFromXml(new XElement("Dummy", input.Value.Data.SaveToXml()),
					Gameworld));
		}
	}

	protected void LoadFromXml(XElement root)
	{
		var revision = int.Parse(root.Element("CraftRevision")?.Value ?? "-1");
		if (revision >= 0)
		{
			Craft = Gameworld.Crafts.Get(long.Parse(root.Element("Craft").Value), revision);
		}
		else
		{
			Craft = Gameworld.Crafts.Get(long.Parse(root.Element("Craft").Value));
		}

		_phase = int.Parse(root.Element("Phase").Value);
		foreach (var item in root.Element("Consumed").Elements())
		{
			var input = Craft.Inputs.First(x => x.Id == long.Parse(item.Attribute("inputid").Value));
			var data = item.Element("Data");
			if (data != null)
			{
				var loadedData = input.LoadDataFromXml(data, Gameworld);
				ConsumedInputs[input] = (loadedData.Perceivable, loadedData);
			}
		}

		var element = root.Element("CheckOutcome");
		if (element != null)
		{
			_checkOutcome = (Outcome)int.Parse(element.Value);
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ActiveCraftGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Craft", Craft.Id),
			new XElement("CraftRevision", Craft.RevisionNumber),
			new XElement("Phase", Phase),
			new XElement("CheckOutcome", (int)CheckOutcome),
			new XElement("Consumed",
				from item in ConsumedInputs
				select new XElement("Input",
					new XAttribute("inputid", item.Key.Id),
					item.Value.Data?.SaveToXml() ?? new XElement("NoData")
				)
			)).ToString();
	}

	#endregion

	#region Overrides of GameItemComponent

	/// <summary>
	///     This property indicates whether this IGameItemComponent acts as a decorator for the IGameItem's description
	/// </summary>
	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short)
		{
			return Craft.ActiveCraftItemSDesc;
		}

		if (type == DescriptionType.Full)
		{
			return
				$"This item is {Craft.ActiveCraftItemSDesc.Colour(Telnet.Green)}, and represents a craft in progress; specifically, the craft {Craft.Name.Colour(Telnet.Green)}. This craft is current at phase {Phase} of {Craft.LastPhase}. It has consumed {ConsumedInputs.Values.Select(x => x.Data.Perceivable.HowSeen(voyeur)).ListToString().IfEmpty("nothing".Colour(Telnet.Green))}. It has produced {ProducedProducts.Values.Select(x => x.Perceivable.HowSeen(voyeur)).ListToString().IfEmpty("nothing".Colour(Telnet.Green))}.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	#endregion

	public void CraftWasInterrupted()
	{
		// Probably do nothing for now
	}

	public void ReleaseItems(ICell location, RoomLayer layer)
	{
		foreach (var item in ProducedProducts.ToList())
		{
			item.Value.ReleaseProducts(location, layer);
		}

		ProducedProducts.Clear();
	}

	public (bool Success, bool Finished) DoNextPhase(IActiveCraftEffect effect)
	{
		if (Craft.HandleCraftPhase(effect.CharacterOwner, effect, this, Phase))
		{
			if (Phase++ < Craft.LastPhase)
			{
				Changed = true;
				return (true, false);
			}
			else
			{
				ReleaseItems(effect.CharacterOwner.Location, effect.CharacterOwner.RoomLayer);
				HasFinished = true;
				Craft.OnUseProgComplete?.Execute(effect.CharacterOwner);
				return (true, true);
			}
		}

		effect.CancelEffect();
		return (false, false);
	}

	#region Overrides of GameItemComponent

	public override void FinaliseLoad()
	{
		foreach (var input in ConsumedInputs)
		{
			input.Value.Data.FinaliseLoadTimeTasks();
		}
	}

	#endregion
}