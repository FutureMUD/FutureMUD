using System;
using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using System.Text;
using MudSharp.Character;
using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

public class SimpleKeyGameItemComponent : GameItemComponent, IKey
{
	private SimpleKeyGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SimpleKeyGameItemComponent(this, newParent, temporary);
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemKey = newItem?.GetItemType<SimpleKeyGameItemComponent>();
		if (newItemKey != null)
		{
			newItemKey.Pattern = Pattern;
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SimpleKeyGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Pattern", Pattern)
		).ToString();
	}

	#region Constructors

	public SimpleKeyGameItemComponent(SimpleKeyGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SimpleKeyGameItemComponent(MudSharp.Models.GameItemComponent component,
		SimpleKeyGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("Pattern");
		if (element != null)
		{
			_pattern = int.Parse(element.Value);
		}
	}

	public SimpleKeyGameItemComponent(SimpleKeyGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion

	#region IKey Members

	public string LockType => _prototype.LockType;

	private int _pattern;

	public int Pattern
	{
		get => _pattern;
		set
		{
			_pattern = value;
			Changed = true;
		}
	}

	public bool Unlocks(string type, int pattern)
	{
		return Pattern != 0 && pattern != 0 &&
		       type.Equals(LockType, StringComparison.InvariantCultureIgnoreCase) &&
		       pattern == Pattern;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Full)
		{
			var sb = new StringBuilder();
			sb.AppendLine(description);
			sb.AppendLine();
			sb.AppendLine(Pattern == 0
				? "It appears to not be matched to an lock."
				: "It appears to have been setup to match some lock.");
			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public string Inspect(ICharacter actor, string description)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You identify the following information about {Parent.HowSeen(actor)}");
		sb.AppendLine(" ");
		sb.AppendLine("  This key works with " + LockType.Colour(Telnet.Green) + " style locks.");
		sb.AppendLine(Pattern == 0 ? "  This key is a blank." : "  This key has been set to match a lock.");
		if (actor.IsAdministrator())
		{
			sb.AppendLine("  The combination for this key is: " + Pattern.ToString().Colour(Telnet.Green));
		}

		return sb.ToString();
	}

	#endregion
}