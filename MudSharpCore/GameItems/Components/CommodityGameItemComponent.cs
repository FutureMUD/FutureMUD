using MudSharp.Form.Material;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class CommodityGameItemComponent : GameItemComponent, ICommodity
{
    private readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> _characteristics = new();

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
        set
        {
            _useIndirectQuantityDescription = value;
            Changed = true;
        }
    }

#nullable enable
    public IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue> CommodityCharacteristics => _characteristics;

    public ICharacteristicValue? GetCommodityCharacteristic(ICharacteristicDefinition definition)
    {
        return _characteristics.TryGetValue(definition, out ICharacteristicValue? value) ? value : null;
    }

    public bool SetCommodityCharacteristic(ICharacteristicDefinition definition, ICharacteristicValue value)
    {
        if (definition is null || value is null || !definition.IsValue(value))
        {
            return false;
        }

        _characteristics[definition] = value;
        Changed = true;
        return true;
    }

    public bool RemoveCommodityCharacteristic(ICharacteristicDefinition definition)
    {
        if (!_characteristics.Remove(definition))
        {
            return false;
        }

        Changed = true;
        return true;
    }

    public void ClearCommodityCharacteristics()
    {
        if (_characteristics.Count == 0)
        {
            return;
        }

        _characteristics.Clear();
        Changed = true;
    }
#nullable restore

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
        foreach (var item in rhs._characteristics)
        {
            _characteristics[item.Key] = item.Value;
        }
    }

    protected void LoadFromXml(XElement root)
    {
        _material = Gameworld.Materials.Get(long.Parse(root.Element("Material").Value));
        _weight = double.Parse(root.Element("Weight").Value);
        _tag = Gameworld.Tags.Get(long.Parse(root.Element("Tag")?.Value ?? "0"));
        _useIndirectQuantityDescription = bool.Parse(root.Element("UseIndirect")?.Value ?? "false");
        foreach (var element in root.Element("Characteristics")?.Elements("Characteristic") ?? Enumerable.Empty<XElement>())
        {
            var definition = Gameworld.Characteristics.Get(long.Parse(element.Attribute("definition")?.Value ?? element.Attribute("Definition")?.Value ?? "0"));
            var value = Gameworld.CharacteristicValues.Get(long.Parse(element.Attribute("value")?.Value ?? element.Attribute("Value")?.Value ?? "0"));
            if (definition is null || value is null || !definition.IsValue(value))
            {
                continue;
            }

            _characteristics[definition] = value;
        }
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
            new XElement("UseIndirect", UseIndirectQuantityDescription),
            new XElement("Characteristics",
                from characteristic in _characteristics.OrderBy(x => x.Key.Id)
                select new XElement("Characteristic",
                    new XAttribute("definition", characteristic.Key.Id),
                    new XAttribute("value", characteristic.Value.Id)
                )
            )
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

    private string CharacteristicMaterialDescription => _characteristics.Count == 0
        ? Material.MaterialDescription
        : $"{string.Join(" ", _characteristics.OrderBy(x => x.Key.Name).ThenBy(x => x.Key.Id).Select(x => x.Value.GetValue.ToLowerInvariant()))} {Material.MaterialDescription}";

    private string CharacteristicMaterialName => _characteristics.Count == 0
        ? Material.Name
        : $"{string.Join(" ", _characteristics.OrderBy(x => x.Key.Name).ThenBy(x => x.Key.Id).Select(x => x.Value.GetValue.ToLowerInvariant()))} {Material.Name}";

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
                        return $"{QuantityDescription(Weight)}{CharacteristicMaterialDescription} {Tag.Name.ToLowerInvariant().Pluralise()}".Colour(Material.ResidueColour);
                    }

                    return $"{QuantityDescription(Weight)}{CharacteristicMaterialDescription}".Colour(Material.ResidueColour);
                }

                if (Tag is not null)
                {
                    return
                        $"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, voyeur)} of {CharacteristicMaterialDescription} {Tag.Name.ToLowerInvariant().Pluralise()}".Colour(Material.ResidueColour);
                }

                return
                    $"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, voyeur)} of {CharacteristicMaterialDescription}".Colour(Material.ResidueColour);
            case DescriptionType.Full:
                if (Tag is not null)
                {
                    return
                        $"This is a commoditised form of {Tag.Name.ToLowerInvariant().Pluralise().ColourName()} of {CharacteristicMaterialName.Colour(Material.ResidueColour)} material.";
                }

                return $"This is a commoditised form of the {CharacteristicMaterialName.Colour(Material.ResidueColour)} material.";
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    public override bool PreventsMerging(IGameItemComponent component)
    {
        return component is CommodityGameItemComponent cc &&
               (cc.Material != Material ||
                cc.Tag != Tag ||
                cc.UseIndirectQuantityDescription != UseIndirectQuantityDescription ||
                !CommodityCharacteristicRequirement.CommodityCharacteristicsEqual(this, cc));
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
