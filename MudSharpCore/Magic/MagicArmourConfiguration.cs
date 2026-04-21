#nullable enable
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

public class MagicArmourConfiguration
{
	public MagicArmourConfiguration(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	public MagicArmourConfiguration(XElement root, IFuturemud gameworld)
		: this(gameworld)
	{
		LoadFromXml(root);
	}

	public MagicArmourConfiguration(MagicArmourConfiguration rhs)
		: this(rhs.Gameworld)
	{
		ArmourType = rhs.ArmourType;
		Quality = rhs.Quality;
		ArmourMaterial = rhs.ArmourMaterial;
		ArmourAppliesProg = rhs.ArmourAppliesProg;
		MaximumDamageAbsorbed = new TraitExpression(rhs.MaximumDamageAbsorbed.OriginalFormulaText, Gameworld);
		FullDescriptionAddendum = rhs.FullDescriptionAddendum;
		ArmourCanBeObscuredByInventory = rhs.ArmourCanBeObscuredByInventory;
		foreach (var shape in rhs.CoveredShapes)
		{
			CoveredShapes.Add(shape);
		}
	}

	public IFuturemud Gameworld { get; }
	public IArmourType ArmourType { get; set; } = null!;
	public ItemQuality Quality { get; set; }
	public ISolid ArmourMaterial { get; set; } = null!;
	public IFutureProg ArmourAppliesProg { get; set; } = null!;
	public ITraitExpression MaximumDamageAbsorbed { get; set; } = null!;
	public string FullDescriptionAddendum { get; set; } = string.Empty;
	public bool ArmourCanBeObscuredByInventory { get; set; }
	public HashSet<IBodypartShape> CoveredShapes { get; } = new();

	public void LoadFromXml(XElement root)
	{
		var value = 0L;
		ArmourAppliesProg = long.TryParse(root.Element("ArmourAppliesProg")?.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(root.Element("ArmourAppliesProg")?.Value ?? "");
		Quality = (ItemQuality)int.Parse(root.Element("Quality")?.Value ?? "0");
		ArmourType =
			(long.TryParse(root.Element("ArmourType")?.Value ?? "0", out value)
				? Gameworld.ArmourTypes.Get(value)
				: Gameworld.ArmourTypes.GetByName(root.Element("ArmourType")?.Value ?? "")) ??
			throw new ApplicationException("Invalid armour type in magic armour configuration.");
		ArmourMaterial =
			(long.TryParse(root.Element("ArmourMaterial")?.Value ?? "0", out value)
				? Gameworld.Materials.Get(value) as ISolid
				: Gameworld.Materials.GetByName(root.Element("ArmourMaterial")?.Value ?? "") as ISolid) ??
			throw new ApplicationException("Invalid armour material in magic armour configuration.");
		FullDescriptionAddendum = root.Element("FullDescriptionAddendum")?.Value ?? string.Empty;
		ArmourCanBeObscuredByInventory = bool.Parse(root.Element("CanBeObscuredByInventory")?.Value ?? "false");
		MaximumDamageAbsorbed = new TraitExpression(root.Element("MaximumDamageAbsorbed")?.Value ?? "0", Gameworld);
		CoveredShapes.Clear();
		var element = root.Element("BodypartShapes");
		if (element is null)
		{
			return;
		}

		foreach (var sub in element.Elements())
		{
			var shape = long.TryParse(sub.Value, out var idValue)
				? Gameworld.BodypartShapes.Get(idValue)
				: Gameworld.BodypartShapes.GetByName(sub.Value);
			if (shape is not null)
			{
				CoveredShapes.Add(shape);
			}
		}
	}

	public void SaveToXml(XElement root)
	{
		root.Add(new XElement("ArmourAppliesProg", ArmourAppliesProg?.Id ?? 0L));
		root.Add(new XElement("Quality", (int)Quality));
		root.Add(new XElement("ArmourType", ArmourType?.Id ?? 0L));
		root.Add(new XElement("ArmourMaterial", ArmourMaterial?.Id ?? 0L));
		root.Add(new XElement("FullDescriptionAddendum", new XCData(FullDescriptionAddendum)));
		root.Add(new XElement("CanBeObscuredByInventory", ArmourCanBeObscuredByInventory));
		root.Add(new XElement("MaximumDamageAbsorbed", new XCData(MaximumDamageAbsorbed.OriginalFormulaText)));
		root.Add(new XElement("BodypartShapes",
			from shape in CoveredShapes
			select new XElement("Shape", shape.Id)
		));
	}

	public bool AppliesToBodypart(IBodypart bodypart)
	{
		if (!CoveredShapes.Any())
		{
			return true;
		}

		return CoveredShapes.Contains(bodypart.Shape);
	}

	public string Show(ICharacter actor)
	{
		return
			$"Armour Type: {ArmourType.Name.ColourValue()}, Quality: {Quality.Describe().ColourValue()}, Material: {ArmourMaterial.Name.ColourValue()}, Absorb: {MaximumDamageAbsorbed.OriginalFormulaText.ColourCommand()}";
	}
}
