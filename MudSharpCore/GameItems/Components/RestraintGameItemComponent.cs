using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class RestraintGameItemComponent : GameItemComponent, IRestraint
{
	protected RestraintGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (RestraintGameItemComponentProto)newProto;
	}

	#region Constructors

	public RestraintGameItemComponent(RestraintGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public RestraintGameItemComponent(MudSharp.Models.GameItemComponent component,
		RestraintGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public RestraintGameItemComponent(RestraintGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		TargetItem = Gameworld.TryGetItem(long.Parse(root.Element("TargetItem")?.Value ?? "0"), true);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new RestraintGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("TargetItem", TargetItem?.Id ?? 0)).ToString();
	}

	#endregion

	#region Implementation of IRestraint

	public IRestraintEffect Effect { get; set; }

	public IEnumerable<LimbType> Limbs => _prototype.LimbTypes;

	public RestraintType RestraintType => RestraintType.Binding;

	public bool CanRestrainCreature(ICharacter actor)
	{
		if (actor.CurrentContextualSize(SizeContext.None) > _prototype.MaximumCreatureSize)
		{
			return false;
		}

		if (actor.CurrentContextualSize(SizeContext.None) < _prototype.MinimumCreatureSize)
		{
			return false;
		}

		if (actor.Body.Limbs.All(x => !_prototype.LimbTypes.Contains(x.LimbType)))
		{
			return false;
		}

		if (Parent.GetItemType<IWearable>()?.CanWear(actor.Body) != true)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotRestrainCreature(ICharacter actor)
	{
		if (actor.CurrentContextualSize(SizeContext.None) > _prototype.MaximumCreatureSize)
		{
			return "They are much too big to be restrained with that.";
		}

		if (actor.CurrentContextualSize(SizeContext.None) < _prototype.MinimumCreatureSize)
		{
			return "They are much too small to be restrained with that.";
		}

		if (actor.Body.Limbs.All(x => !_prototype.LimbTypes.Contains(x.LimbType)))
		{
			return "They don't have any of the appropriate kinds of limbs to be restrained by that.";
		}

		if (Parent.GetItemType<IWearable>()?.CanWear(actor.Body) != true)
		{
			return "There just isn't any way you can think of to restrain them with that.";
		}

		throw new System.ApplicationException("Got to the end of WhyCannotRestrainCreature.");
	}

	public Difficulty BreakoutDifficulty => _prototype.BreakoutDifficulty;
	public Difficulty OverpowerDifficulty => _prototype.OverpowerDifficulty;
	public IGameItem TargetItem { get; set; }

	#endregion
}