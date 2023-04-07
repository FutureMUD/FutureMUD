using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class PowerToolGameItemComponent : GameItemComponent, IToolItem, IConsumePower
{
	protected PowerToolGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PowerToolGameItemComponentProto)newProto;
	}

	#region Constructors

	public PowerToolGameItemComponent(PowerToolGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public PowerToolGameItemComponent(MudSharp.Models.GameItemComponent component,
		PowerToolGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PowerToolGameItemComponent(PowerToolGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PowerToolGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region IToolItem Implementation

	public bool CountAsTool(ITag toolTag)
	{
		return Parent.Tags.Any(x => x.IsA(toolTag));
	}

	public bool CanUseTool(ITag toolTag, TimeSpan baseUsage)
	{
		return CountAsTool(toolTag) && _powered && (Parent.GetItemType<IProducePower>()
		                                                  ?.CanDrawdownSpike(
			                                                  _prototype.Wattage * baseUsage.TotalSeconds) ?? false);
	}

	public double ToolTimeMultiplier(ITag toolTag)
	{
		return _prototype.BaseMultiplier - (int)Parent.Quality * _prototype.MultiplierReductionPerQuality;
	}

	public void UseTool(ITag toolTag, TimeSpan usage)
	{
		Parent.GetItemType<IProducePower>()?.DrawdownSpike(_prototype.Wattage * usage.TotalSeconds);
	}

	#endregion

	#region IConsumePower Implementation

	private bool _powered;
	public double PowerConsumptionInWatts => 0.0;

	public void OnPowerCutIn()
	{
		_powered = true;
	}

	public void OnPowerCutOut()
	{
		_powered = false;
	}

	#endregion

	public override void Delete()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Delete();
	}

	public override void Quit()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Quit();
	}

	public override void Login()
	{
		base.Login();
		Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
	}
}