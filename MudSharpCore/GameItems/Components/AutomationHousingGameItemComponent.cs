#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

public class AutomationHousingGameItemComponent : GameItemComponent, IAutomationHousing
{
	private AutomationHousingGameItemComponentProto _prototype;

	public AutomationHousingGameItemComponent(AutomationHousingGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public AutomationHousingGameItemComponent(MudSharp.Models.GameItemComponent component,
		AutomationHousingGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public AutomationHousingGameItemComponent(AutomationHousingGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public IEnumerable<IGameItem> ConcealedItems => Parent.GetItemType<IContainer>()?.Contents
		.Where(IsSupportedAutomationItem)
		.ToList() ?? [];

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new AutomationHousingGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		if (Parent.GetItemType<IContainer>() is not IContainer)
		{
			sb.AppendLine("Its automation housing is misconfigured and has no service cavity.");
			return sb.ToString();
		}

		if (Parent.GetItemType<IOpenable>() is IOpenable { IsOpen: false })
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

	public bool CanAccessHousing(ICharacter actor, out string error)
	{
		if (Parent.GetItemType<IContainer>() is not IContainer)
		{
			error = $"{Parent.HowSeen(actor, true)} is not configured with a service cavity.";
			return false;
		}

		if (Parent.GetItemType<IOpenable>() is IOpenable { IsOpen: false })
		{
			error = $"You need to open {Parent.HowSeen(actor, true)} before you can service the concealed automation items.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool CanConcealItem(IGameItem item, out string error)
	{
		if (Parent.GetItemType<IContainer>() is not IContainer)
		{
			error = $"{Parent.HowSeen(item, true)} is not configured with a service cavity.";
			return false;
		}

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
		_prototype = (AutomationHousingGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	private void LoadFromXml(XElement root)
	{
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
