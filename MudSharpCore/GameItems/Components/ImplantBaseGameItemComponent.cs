using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ImplantBaseGameItemComponent : GameItemComponent, IImplant
{
	protected ImplantBaseGameItemComponentProto _implantPrototype;
	public override IGameItemComponentProto Prototype => _implantPrototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_implantPrototype = (ImplantBaseGameItemComponentProto)newProto;
	}

	#region Constructors

	public ImplantBaseGameItemComponent(ImplantBaseGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_implantPrototype = proto;
	}

	public ImplantBaseGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantBaseGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_implantPrototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantBaseGameItemComponent(ImplantBaseGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_implantPrototype = rhs._implantPrototype;
	}

	protected virtual void LoadFromXml(XElement root)
	{
		var element = root.Element("OverridenBodypart");
		if (element != null)
		{
			var part = Gameworld.BodypartPrototypes.Get(long.Parse(element.Value));
			if (part != null)
			{
				OverridenBodypart = part;
			}
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantBaseGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("OverridenBodypart", OverridenBodypart?.Id ?? 0)).ToString();
	}

	protected XElement SaveToXmlNoTextConversion()
	{
		return new XElement("Definition", new XElement("OverridenBodypart", OverridenBodypart?.Id ?? 0));
	}

	#endregion

	#region IImplant Members

	public double ImplantSpaceOccupied => _implantPrototype.ImplantSpaceOccupied;
	public Difficulty InstallDifficulty => _implantPrototype.InstallDifficulty;

	public double FunctionFactor
	{
		get
		{
			if (!_powered)
			{
				return 0.0;
			}

			var health = Parent.HealthStrategy.CurrentHealthPercentage(Parent);
			if (health >= 1.0 - _implantPrototype.ImplantDamageFunctionGrace)
			{
				return 1.0;
			}

			return health * (1.0 - _implantPrototype.ImplantDamageFunctionGrace);
		}
	}

	public bool External => _implantPrototype.External;
	public string ExternalDescription => _implantPrototype.ExternalDescription;
	public IBodyPrototype TargetBody => _implantPrototype.TargetBody;
	protected IBodypart OverridenBodypart;

	public IBodypart TargetBodypart
	{
		get => OverridenBodypart ?? _implantPrototype.TargetBodypart;
		set
		{
			OverridenBodypart = value;
			Changed = true;
		}
	}

	private IBody _installedBody;

	public IBody InstalledBody
	{
		get => _installedBody;
		set
		{
			if (_installedBody == null && value != null)
			{
				Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
			}
			else if (_installedBody != null && value == null)
			{
				Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
				OverridenBodypart = null;
				Changed = true;
			}

			_installedBody = value;
		}
	}

	public void InstallImplant(IBody body)
	{
		InstalledBody = body;
		Changed = true;
	}

	public void RemoveImplant()
	{
		InstalledBody = null;
		Changed = true;
	}

	public virtual double PowerConsumptionInWatts => _implantPrototype.PowerConsumptionInWatts -
	                                                 _implantPrototype.PowerConsumptionDiscountPerQuality *
	                                                 (int)Parent.Quality;

	protected bool _powered;

	public virtual void OnPowerCutIn()
	{
		_powered = true;
	}

	public virtual void OnPowerCutOut()
	{
		_powered = false;
	}

	#endregion

	#region Overrides of GameItemComponent

	public override void Quit()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Quit();
	}

	public override void Delete()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Delete();
	}

	public override void Login()
	{
		base.Login();
		if (InstalledBody != null)
		{
			Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
		}
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short)
		{
			if (InstalledBody != null && External)
			{
				return ExternalDescription;
			}
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override int DecorationPriority => int.MaxValue;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		if (type == DescriptionType.Short)
		{
			return InstalledBody != null && External;
		}

		return base.DescriptionDecorator(type);
	}

	#endregion
}