using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class LocksmithingGameItemComponent : GameItemComponent, ILocksmithingTool
{
	private LocksmithingGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LocksmithingGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LocksmithingGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	#region Constructors

	public LocksmithingGameItemComponent(LocksmithingGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public LocksmithingGameItemComponent(MudSharp.Models.GameItemComponent component,
		LocksmithingGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
	}

	public LocksmithingGameItemComponent(LocksmithingGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion

	#region Implementation of ILocksmithingTool

	public bool UsableForInstallation => _prototype.UsableForInstallation;
	public bool UsableForConfiguration => _prototype.UsableForConfiguration;
	public bool UsableForFabrication => _prototype.UsableForFabrication;
	public bool Breakable => _prototype.Breakable;
	public int DifficultyAdjustment => _prototype.DifficultyAdjustment;

	#endregion
}