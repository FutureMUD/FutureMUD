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
    private TimeSpan MaximumDuration()
    {
        var maximum = _prototype.ToolDurabilitySecondsExpression.EvaluateDoubleWith(("quality", (int)Parent.Quality));
        if (maximum <= 0.0)
        {
            return TimeSpan.Zero;
        }
        return TimeSpan.FromSeconds(maximum);
    }

    private TimeSpan EffectiveDurationAvailable()
    {
        var maximum = _prototype.ToolDurabilitySecondsExpression.EvaluateDoubleWith(("quality", (int)Parent.Quality));
        if (maximum <= 0.0)
        {
            return TimeSpan.Zero;
        }
        var remaining = Parent.Condition * maximum;
        return TimeSpan.FromSeconds(remaining);
    }

    public bool CountAsTool(ITag toolTag)
	{
		return Parent.Tags.Any(x => x.IsA(toolTag));
	}

	public bool CanUseTool(ITag toolTag, TimeSpan baseUsage)
	{
        if (!CountAsTool(toolTag))
        {
            return false;
        }

        if (EffectiveDurationAvailable() < baseUsage)
        {
            return false;
        }

		if (!_powered)
		{
			return false;
		}

		if (Parent.GetItemType<IProducePower>()?.CanDrawdownSpike(_prototype.Wattage * baseUsage.TotalSeconds) != true)
		{
			return false;
		}

		return true;
	}

	public double ToolTimeMultiplier(ITag toolTag)
	{
		return _prototype.BaseMultiplier - (int)Parent.Quality * _prototype.MultiplierReductionPerQuality;
	}

	public void UseTool(ITag toolTag, TimeSpan usage)
	{
		Parent.GetItemType<IProducePower>()?.DrawdownSpike(_prototype.Wattage * usage.TotalSeconds);
        var max = MaximumDuration();
        if (max <= TimeSpan.Zero || usage <= TimeSpan.Zero)
        {
            return;
        }

        Parent.Condition -= usage.TotalSeconds / max.TotalSeconds;
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
