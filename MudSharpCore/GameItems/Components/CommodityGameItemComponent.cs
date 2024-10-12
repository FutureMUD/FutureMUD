using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class CommodityGameItemComponent : GameItemComponent, ICommodity
{
	public ISolid Material
	{
		get => _material;
		set
		{
			_material = value;
			Changed = true;
		}
	}

	public double Weight
	{
		get => _weight;
		set
		{
			_weight = value;
			Changed = true;
		}
	}
#nullable enable
	private ITag? _tag;
	public
		ITag? Tag
	{
		get => _tag;
		set
		{
			_tag = value;
			Changed = true;
		}
	}
#nullable restore

	private bool _useIndirectQuantityDescription;

	public bool UseIndirectQuantityDescription
	{
		get => _useIndirectQuantityDescription;
		set { _useIndirectQuantityDescription = value;
			Changed = true;
		}
	}

	protected CommodityGameItemComponentProto _prototype;
	private ISolid _material;
	private double _weight;


	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CommodityGameItemComponentProto)newProto;
	}

	#region Constructors

	public CommodityGameItemComponent(CommodityGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
		_material = Gameworld.Materials.First();
		_weight = 1.0;
	}

	public CommodityGameItemComponent(MudSharp.Models.GameItemComponent component,
		CommodityGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CommodityGameItemComponent(CommodityGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_material = rhs._material;
		_tag = rhs._tag;
		_useIndirectQuantityDescription = rhs._useIndirectQuantityDescription;
		_weight = rhs._weight;
	}

	protected void LoadFromXml(XElement root)
	{
		_material = Gameworld.Materials.Get(long.Parse(root.Element("Material").Value));
		_weight = double.Parse(root.Element("Weight").Value);
		_tag = Gameworld.Tags.Get(long.Parse(root.Element("Tag")?.Value ?? "0"));
		_useIndirectQuantityDescription = bool.Parse(root.Element("UseIndirect")?.Value ?? "false");
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CommodityGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Material", Material.Id),
			new XElement("Weight", Weight),
			new XElement("Tag", Tag?.Id ?? 0L),
			new XElement("UseIndirect", UseIndirectQuantityDescription)
		).ToString();
	}

	#endregion

	public override bool OverridesMaterial => true;

	public override ISolid OverridenMaterial => Material;

	public override double ComponentWeight => Weight;

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return (fluidDensity - Material.Density) * Weight;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		switch (type)
		{
			case DescriptionType.Short:
			case DescriptionType.Full:
				return true;
		}

		return base.DescriptionDecorator(type);
	}

	public string QuantityDescription(double weight)
	{
		if (weight >= Gameworld.GetStaticDouble("CommodityDescriptorColossal"))
		{
			return "a colossal quantity of ";
		}

		if (weight >= Gameworld.GetStaticDouble("CommodityDescriptorEnormous"))
		{
			return "an enormous quantity of ";
		}

		if (weight >= Gameworld.GetStaticDouble("CommodityDescriptorHuge"))
		{
			return "a huge quantity of ";
		}

		if (weight >= Gameworld.GetStaticDouble("CommodityDescriptorLarge"))
		{
			return "a large quantity of ";
		}

		if (weight >= Gameworld.GetStaticDouble("CommodityDescriptorModerate"))
		{
			return "a moderate quantity of ";
		}

		if (weight >= Gameworld.GetStaticDouble("CommodityDescriptorSmall"))
		{
			return "a small quantity of ";
		}

		if (weight >= Gameworld.GetStaticDouble("CommodityDescriptorTiny"))
		{
			return "a tiny quantity of ";
		}

		return "a miniscule quantity of ";
	}
	public override int DecorationPriority => -1;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				if (UseIndirectQuantityDescription)
				{
					if (Tag is not null)
					{
						return $"{QuantityDescription(Weight)}{Material.MaterialDescription} {Tag.Name.ToLowerInvariant().Pluralise()}".Colour(Material.ResidueColour);
					}

					return $"{QuantityDescription(Weight)}{Material.MaterialDescription}".Colour(Material.ResidueColour);
				}
				
				if (Tag is not null)
				{
					return
						$"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, voyeur)} of {Material.MaterialDescription} {Tag.Name.ToLowerInvariant().Pluralise()}".Colour(Material.ResidueColour);
				}

				return
					$"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, voyeur)} of {Material.MaterialDescription}".Colour(Material.ResidueColour);
			case DescriptionType.Full:
				if (Tag is not null)
				{
					return
						$"This is a commoditised form of {Tag.Name.ToLowerInvariant().Pluralise().ColourName()} of {Material.Name.Colour(Material.ResidueColour)} material.";
				}

				return $"This is a commoditised form of the {Material.Name.Colour(Material.ResidueColour)} material.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return component is CommodityGameItemComponent cc &&
		       (cc.Material != Material ||
		        cc.Tag != Tag);
	}

	public override bool ExposeToLiquid(LiquidMixture mixture)
	{
		if (Tag == PuddleGameItemComponent.PuddleResidueTag(Gameworld) && mixture.CountsAs(Material.Solvent).Truth)
		{
			Weight -= mixture.TotalVolume / Material.SolventRatio;
			if (Weight <= 0.0)
			{
				Delete();
				return true;
			}
			Changed = true;
		}
		return base.ExposeToLiquid(mixture);
	}
}