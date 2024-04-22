using MudSharp.Economy;
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
using MudSharp.Form.Shape;

namespace MudSharp.GameItems.Components
{
	public class MarketGoodWeightGameItemComponent : GameItemComponent, IMarketGoodWeightItem
	{
		protected MarketGoodWeightGameItemComponentProto _prototype;
		public override IGameItemComponentProto Prototype => _prototype;

		protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
		{
			_prototype = (MarketGoodWeightGameItemComponentProto)newProto;
		}

		#region Constructors
		public MarketGoodWeightGameItemComponent(MarketGoodWeightGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
		{
			_prototype = proto;
		}

		public MarketGoodWeightGameItemComponent(Models.GameItemComponent component, MarketGoodWeightGameItemComponentProto proto, IGameItem parent) : base(component, parent)
		{
			_prototype = proto;
			_noSave = true;
			LoadFromXml(XElement.Parse(component.Definition));
			_noSave = false;
		}

		public MarketGoodWeightGameItemComponent(MarketGoodWeightGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
		{
			_prototype = rhs._prototype;
		}

		protected void LoadFromXml(XElement root)
		{
			// TODO
		}

		public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
		{
			return new MarketGoodWeightGameItemComponent(this, newParent, temporary);
		}
		#endregion

		#region Saving
		protected override string SaveToXml()
		{
			return new XElement("Definition").ToString();
		}
		#endregion

		public decimal MultiplierForCategory(IMarketCategory category)
		{
			return _prototype.MarketMultipliers.GetValueOrDefault(category.Id, 1.0M);
		}

		/// <inheritdoc />
		public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags)
		{
			if (type == DescriptionType.Evaluate)
			{
				return $"This item has different weights for market category need satisfaction:\n{_prototype.MarketMultipliers.Select(x => $"{Gameworld.MarketCategories.Get(x.Key)!.Name.ColourName()} = {x.Value.ToString("P2", voyeur).ColourValue()}").ListToLines(true)}";
			}
			return base.Decorate(voyeur, name, description, type, colour, flags);
		}

		/// <inheritdoc />
		public override bool DescriptionDecorator(DescriptionType type)
		{
			return type == DescriptionType.Evaluate;
		}
	}
}
