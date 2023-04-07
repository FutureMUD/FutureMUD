using System;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ShieldGameItemComponent : GameItemComponent, IShield, IMeleeWeapon
{
	protected ShieldGameItemComponentProto _prototype;

	#region Implementation of IUseTrait

	public ITraitDefinition Trait => _prototype.ShieldType.BlockTrait;

	#endregion

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ShieldGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ShieldGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0)).ToString();
	}

	#region Implementation of IMeleeWeapon

	private IWield _primaryWieldedLocation;

	public IWield PrimaryWieldedLocation
	{
		get => _primaryWieldedLocation;
		set
		{
			_primaryWieldedLocation = value;
			Changed = true;
		}
	}

	public bool AlwaysRequiresTwoHandsToWield => false;

	public IShieldType ShieldType => _prototype.ShieldType;

	#endregion

	#region Constructors

	public ShieldGameItemComponent(ShieldGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ShieldGameItemComponent(MudSharp.Models.GameItemComponent component, ShieldGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		PrimaryWieldedLocation =
			Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
	}

	public ShieldGameItemComponent(ShieldGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion

	public override bool DesignedForOffhandUse => true;


	#region Implementation of IMeleeWeapon

	IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;

	public WeaponClassification Classification => _prototype.MeleeWeaponType.Classification;

	public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
	{
		throw new NotImplementedException();
	}

	#endregion
}