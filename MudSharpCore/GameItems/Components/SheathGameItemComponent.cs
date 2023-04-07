using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class SheathGameItemComponent : GameItemComponent, ISheath
{
	protected SheathGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override void Delete()
	{
		base.Delete();
		Content?.Delete();
	}

	public override void Quit()
	{
		base.Quit();
		Content?.Parent.Quit();
	}

	public override void Login()
	{
		Content?.Parent.Login();
	}

	public override bool Take(IGameItem item)
	{
		if (_content?.Parent == item)
		{
			_content = null;
			Changed = true;
			return true;
		}

		return false;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SheathGameItemComponent(this, newParent, temporary);
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return Content != null;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Contents || type == DescriptionType.Short;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return
					$"{description}{(Content != null ? $" bearing {Content.Parent.Name.ToLowerInvariant().A_An(colour: Telnet.Green)}" : "")}";
			case DescriptionType.Contents:
				return
					$"{description}{(Content != null ? $"\n\nIt contains {Content.Parent.HowSeen(voyeur)}." : "\n\nIt is currently empty.")}";
		}

		return description;
	}

	public override int DecorationPriority => -1;
	public override double ComponentWeight => Content?.Parent.Weight ?? 0.0;

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Content?.Parent.Buoyancy(fluidDensity) ?? 0.0;
	}

	public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		if (Content != null && Content == existingItem.GetItemType<IWieldable>())
		{
			Content = null;
			Content = newItem.GetItemType<IWieldable>();
			return true;
		}

		return false;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		if (Content == null)
		{
			return false;
		}

		var newItemSheath = newItem?.GetItemType<ISheath>();
		if (newItemSheath != null && newItemSheath.MaximumSize >= Content.Parent.Size)
		{
			newItemSheath.Content = Content;
			Content.Parent.ContainedIn = newItemSheath.Parent;
			Content = null;
		}
		else
		{
			if (location != null)
			{
				location.Insert(Content.Parent);
				Content.Parent.ContainedIn = null;
				Content = null;
			}
			else
			{
				Content.Parent.Delete();
			}
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SheathGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new object[]
			{
				new XElement("Content", Content?.Parent.Id.ToString() ?? "")
			}
		).ToString();
	}

	#region ISheath Members

	public SizeCategory MaximumSize => _prototype.MaximumSize;

	private IWieldable _content;

	public IWieldable Content
	{
		get => _content;
		set
		{
			if (_content != value && _content != null)
			{
				_content.Parent.ContainedIn = null;
			}

			_content = value;
			if (_content != null)
			{
				if (_noSave)
				{
					_content.Parent.LoadTimeSetContainedIn(Parent);
				}
				else
				{
					_content.Parent.ContainedIn = Parent;
				}
			}

			Changed = true;
		}
	}

	public Difficulty StealthDrawDifficulty => _prototype.StealthDrawDifficulty;

	public bool DesignedForGuns => _prototype.DesignedForGuns;

	#endregion

	#region Constructors

	public SheathGameItemComponent(SheathGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SheathGameItemComponent(MudSharp.Models.GameItemComponent component, SheathGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		var content = root.Element("Content");
		if (!string.IsNullOrEmpty(content?.Value))
		{
			var contentItem = Gameworld.TryGetItem(long.Parse(content.Value), true);
			contentItem.Get(null);
			var wieldable = contentItem.GetItemType<IWieldable>();
			if (wieldable != null)
			{
				Content = wieldable;
			}
			else
			{
				Console.WriteLine("Warning: sheath content was not wieldable.");
			}
		}
	}

	public SheathGameItemComponent(SheathGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override void FinaliseLoad()
	{
		_content?.Parent.FinaliseLoadTimeTasks();
	}

	#endregion
}