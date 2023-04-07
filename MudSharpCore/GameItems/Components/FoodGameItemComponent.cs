using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class FoodGameItemComponent : GameItemComponent, IEdible
{
	private FoodGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new FoodGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		switch (type)
		{
			case DescriptionType.Short:
			case DescriptionType.Full:
				return true;
		}

		return false;
	}

	public override int DecorationPriority => 100;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return _prototype.Decorator.Describe(name, description, 100.0 * BitesRemaining / TotalBites);
			case DescriptionType.Full:
				return string.Format(voyeur, "{0}\n\nIt has {1:N0} out of {2:N0} bites remaining.", description,
					BitesRemaining, TotalBites);
		}

		return description;
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return BitesRemaining == TotalBites;
	}

	public override double ComponentWeightMultiplier
		=> BitesRemaining < _prototype.Bites ? (double)BitesRemaining / _prototype.Bites : 1.0;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (FoodGameItemComponentProto)newProto;
		if (BitesRemaining > TotalBites)
		{
			BitesRemaining = TotalBites;
		}
	}

	#region Constructors

	public FoodGameItemComponent(FoodGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		BitesRemaining = rhs.BitesRemaining;
	}

	public FoodGameItemComponent(FoodGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		BitesRemaining = TotalBites;
	}

	protected void LoadFromXml(XElement root)
	{
		var element = root.Element("Bites");
		if (element != null)
		{
			BitesRemaining = int.Parse(element.Value);
		}
	}


	protected override string SaveToXml()
	{
		return new XElement("Definition", new object[]
		{
			new XElement("Bites", BitesRemaining)
		}).ToString();
	}

	public FoodGameItemComponent(MudSharp.Models.GameItemComponent component, FoodGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	#endregion

	#region IEdible Members

	public double Calories => _prototype.Calories;

	public double SatiationPoints => _prototype.SatiationPoints;

	public double WaterLitres => _prototype.WaterLitres;

	public double ThirstPoints => _prototype.ThirstPoints;

	public double AlcoholLitres => _prototype.AlcoholLitres;

	public string TasteString => _prototype.TasteString;

	public double TotalBites => _prototype.Bites;

	private double _bitesRemaining;

	public double BitesRemaining
	{
		get => _bitesRemaining;
		set
		{
			_bitesRemaining = value;
			if (_bitesRemaining > 0)
			{
				Changed = true;
			}
			else
			{
				Parent.Delete();
			}
		}
	}

	public void Eat(IBody body, double bites)
	{
		BitesRemaining -= bites;
		_prototype.OnEatProg?.Execute(body.Actor, Parent, bites);
	}

	#endregion
}