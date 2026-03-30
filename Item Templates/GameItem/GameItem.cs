#nullable enable
using System.Xml.Linq;
using MudSharp.Framework;

namespace $rootnamespace$.Components;

public class $safeitemrootname$GameItemComponent : GameItemComponent
{
	protected $safeitemrootname$GameItemComponentProto _prototype;

	public $safeitemrootname$GameItemComponent($safeitemrootname$GameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public $safeitemrootname$GameItemComponent(MudSharp.Models.GameItemComponent component,
		$safeitemrootname$GameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public $safeitemrootname$GameItemComponent($safeitemrootname$GameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new $safeitemrootname$GameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = ($safeitemrootname$GameItemComponentProto)newProto;
	}

	protected virtual void LoadFromXml(XElement root)
	{
		// Load runtime state here if the component persists per-item data.
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
