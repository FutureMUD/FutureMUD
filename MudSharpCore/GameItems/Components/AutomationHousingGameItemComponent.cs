#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

public class AutomationHousingGameItemComponent : LockingContainerGameItemComponent, IAutomationHousing
{
	private new AutomationHousingGameItemComponentProto _prototype;

	public AutomationHousingGameItemComponent(AutomationHousingGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public AutomationHousingGameItemComponent(MudSharp.Models.GameItemComponent component,
		AutomationHousingGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
	}

	public AutomationHousingGameItemComponent(AutomationHousingGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public IEnumerable<IGameItem> ConcealedItems => Contents.Where(IsSupportedAutomationItem).ToList();

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new AutomationHousingGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || base.DescriptionDecorator(type);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		var baseDescription = base.Decorate(voyeur, name, description, type, colour, flags);
		if (type != DescriptionType.Full)
		{
			return baseDescription;
		}

		var sb = new StringBuilder(baseDescription);
		sb.AppendLine();
		sb.AppendLine();
		if (!IsOpen)
		{
			sb.AppendLine("Its automation housing is sealed shut.");
			return sb.ToString();
		}

		var concealed = ConcealedItems.ToList();
		sb.AppendLine("Its automation housing is open for service.");
		if (!concealed.Any())
		{
			sb.AppendLine("No automation items are currently concealed inside.");
			return sb.ToString();
		}

		sb.AppendLine("Concealed automation items:");
		foreach (var item in concealed.OrderBy(x => x.Id))
		{
			sb.AppendLine($"\t{item.HowSeen(voyeur, true).ColourName()}");
		}

		return sb.ToString();
	}

	public override bool CanPut(IGameItem item)
	{
		return IsSupportedAutomationItem(item) && base.CanPut(item);
	}

	public override int CanPutAmount(IGameItem item)
	{
		return IsSupportedAutomationItem(item) ? base.CanPutAmount(item) : 0;
	}

	public override WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		return IsSupportedAutomationItem(item) ? base.WhyCannotPut(item) : WhyCannotPutReason.NotCorrectItemType;
	}

	public bool CanAccessHousing(ICharacter actor, out string error)
	{
		if (!IsOpen)
		{
			error = $"You need to open {Parent.HowSeen(actor, true)} before you can service the concealed automation items.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool CanConcealItem(IGameItem item, out string error)
	{
		if (IsSupportedAutomationItem(item))
		{
			error = string.Empty;
			return true;
		}

		error = $"{Parent.HowSeen(item, true)} is only configured to conceal automation modules, signal devices, and cable runs.";
		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (AutomationHousingGameItemComponentProto)newProto;
	}

	private bool IsSupportedAutomationItem(IGameItem item)
	{
		if (_prototype.AllowCableSegments && item.GetItemType<ISignalCableSegment>() is not null)
		{
			return true;
		}

		if (_prototype.AllowMountableModules && item.GetItemType<IAutomationMountable>() is not null)
		{
			return true;
		}

		if (_prototype.AllowSignalItems && item.Components.Any(x =>
			    x is ISignalSourceComponent ||
			    x is IRuntimeConfigurableSignalSinkComponent ||
			    x is IRuntimeProgrammableMicrocontroller ||
			    x is IAutomationMountHost))
		{
			return true;
		}

		return false;
	}
}
