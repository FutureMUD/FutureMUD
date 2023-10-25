using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class CoverGameItemComponent : GameItemComponent, IProvideCover
{
	protected CoverGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CoverGameItemComponentProto)newProto;
	}

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("IsProvidingCover", IsProvidingCover)).ToString();
	}

	#endregion

	#region Constructors

	public CoverGameItemComponent(CoverGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		_isProvidingCover = proto.ProvideCoverByDefault;
	}

	public CoverGameItemComponent(MudSharp.Models.GameItemComponent component, CoverGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CoverGameItemComponent(CoverGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		IsProvidingCover = bool.Parse(root.Element("IsProvidingCover").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CoverGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region IProvideCover Implementation

	private bool _isProvidingCover;

	public bool IsProvidingCover
	{
		get => _isProvidingCover;
		set
		{
			if (_isProvidingCover && !value)
			{
				OnNoLongerProvideCover?.Invoke(Parent);
			}

			_isProvidingCover = value;
			Changed = true;
		}
	}

	public IRangedCover Cover => IsProvidingCover ? _prototype.Cover : null;

	public event PerceivableEvent OnNoLongerProvideCover;

	#endregion
}