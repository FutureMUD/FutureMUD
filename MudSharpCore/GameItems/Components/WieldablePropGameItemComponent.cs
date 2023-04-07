using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class WieldablePropGameItemComponent : GameItemComponent, IWieldable
{
	protected WieldablePropGameItemComponentProto _prototype;

	public WieldablePropGameItemComponent(WieldablePropGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public WieldablePropGameItemComponent(MudSharp.Models.GameItemComponent component,
		WieldablePropGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		var root = XElement.Parse(component.Definition);
		_noSave = true;
		PrimaryWieldedLocation =
			Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
		_noSave = false;
	}

	public WieldablePropGameItemComponent(WieldablePropGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new WieldablePropGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (WieldablePropGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0)).ToString();
	}

	#region Implementation of IWieldable

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
}