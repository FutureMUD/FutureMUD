using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class WornTraitChangerGameItemComponent : GameItemComponent, IChangeTraitsInInventory
{
	protected WornTraitChangerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (WornTraitChangerGameItemComponentProto)newProto;
	}

	#region Constructors

	public WornTraitChangerGameItemComponent(WornTraitChangerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public WornTraitChangerGameItemComponent(MudSharp.Models.GameItemComponent component,
		WornTraitChangerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public WornTraitChangerGameItemComponent(WornTraitChangerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// Noop
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new WornTraitChangerGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	public double BonusForTrait(ITraitDefinition definition, TraitBonusContext context)
	{
		if (Parent.GetItemType<IWearable>()?.WornBy == null)
		{
			return 0.0;
		}

		return _prototype.TraitModifiers
		                 .FirstOrDefault(x => x.Trait == definition &&
		                                      (context == x.Context || x.Context == TraitBonusContext.None))
		                 .Modifier;
	}

	#region Overrides of GameItemComponent

	/// <summary>
	///     This property indicates whether this IGameItemComponent acts as a decorator for the IGameItem's description
	/// </summary>
	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var worn = Parent.GetItemType<IWearable>()?.WornBy != null;
		var sb = new StringBuilder(description);
		if (type == DescriptionType.Evaluate)
		{
			foreach (var (trait, context, modifier) in _prototype.TraitModifiers)
			{
				sb.AppendLine(
					$"This {(worn ? "is giving you" : "would give you")} a {ModifierDesc(modifier)} {(modifier >= 0.0 ? "bonus".Colour(Telnet.Green) : "penalty".Colour(Telnet.Red))} to {trait.Name.ColourValue()}{(context == TraitBonusContext.None ? "" : $"in the {context.DescribeEnum().Colour(Telnet.Cyan)} context")} when worn.");
			}

			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	private string ModifierDesc(double value)
	{
		value = Math.Abs(value);
		if (value <= Gameworld.GetStaticDouble("TraitBonusDescriptionThresholdNegligible"))
		{
			return "negligible";
		}

		if (value <= Gameworld.GetStaticDouble("TraitBonusDescriptionThresholdSmall"))
		{
			return "small";
		}

		if (value <= Gameworld.GetStaticDouble("TraitBonusDescriptionThresholdModerate"))
		{
			return "moderate";
		}

		if (value <= Gameworld.GetStaticDouble("TraitBonusDescriptionThresholdLarge"))
		{
			return "large";
		}

		if (value <= Gameworld.GetStaticDouble("TraitBonusDescriptionThresholdVeryLarge"))
		{
			return "very large";
		}

		return "extremely large";
	}

	#endregion
}