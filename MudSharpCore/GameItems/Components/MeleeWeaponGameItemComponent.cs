using System;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class MeleeWeaponGameItemComponent : GameItemComponent, IMeleeWeapon
{
	protected MeleeWeaponGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MeleeWeaponGameItemComponent(this, newParent, temporary);
	}

	#region Implementation of IDamageSource

	public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
	{
		throw new NotImplementedException();
	}

	#endregion

	#region Implementation of IUseTrait

	public ITraitDefinition Trait => _prototype.WeaponType.AttackTrait;

	#endregion

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (MeleeWeaponGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0)).ToString();
	}

	#region Implementation of IMeleeWeapon

	public IWeaponType WeaponType => _prototype.WeaponType;

	public WeaponClassification Classification => _prototype.WeaponType.Classification;

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

	#endregion

	#region Constructors

	public MeleeWeaponGameItemComponent(MeleeWeaponGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public MeleeWeaponGameItemComponent(MudSharp.Models.GameItemComponent component,
		MeleeWeaponGameItemComponentProto proto,
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

	public MeleeWeaponGameItemComponent(MeleeWeaponGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion

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
		if (type == DescriptionType.Evaluate)
		{
			return
				$"This is a melee weapon of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(WeaponType.ParryTrait == WeaponType.AttackTrait ? "defense" : $"the {WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
		}

		return description;
	}

	#endregion
}