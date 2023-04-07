using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;

namespace MudSharp.GameItems.Components;

public class DestroyableGameItemComponent : GameItemComponent, IDestroyable, IAffectQuality
{
	protected DestroyableGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (DestroyableGameItemComponentProto)newProto;
	}

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region Constructors

	public DestroyableGameItemComponent(DestroyableGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public DestroyableGameItemComponent(MudSharp.Models.GameItemComponent component,
		DestroyableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public DestroyableGameItemComponent(DestroyableGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new DestroyableGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region IDestroyable Implementation

	public IDamage GetActualDamage(IDamage originalDamage)
	{
		return new Damage(originalDamage)
		{
			DamageAmount = _prototype.DamageTypeMultipliers[originalDamage.DamageType] * originalDamage.DamageAmount,
			PainAmount = 0,
			StunAmount = 0,
			ShockAmount = 0
		};
	}

	public double MaximumDamage
	{
		get
		{
			_prototype.HpExpression.Parameters["quality"] = (int)Parent.Prototype.BaseItemQuality;
			_prototype.HpExpression.Parameters["size"] = (int)Parent.Size;
			return Convert.ToDouble(_prototype.HpExpression.Evaluate());
		}
	}

	public int ItemQualityStages
	{
		get
		{
			var ratio = Parent.Wounds.Sum(x => x.CurrentDamage) / MaximumDamage;
			return (int)(ratio / -0.15);
		}
	}

	#endregion
}