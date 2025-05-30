﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class SheathGameItemComponent : GameItemComponent, ISheath, IContainer
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

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
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

	public bool CanSheath(IGameItem item)
	{
		if (Content is not null)
		{
			return false;
		}

		if (!item.IsItemType<IWieldable>())
		{
			return false;
		}

		if (MaximumSize < item.Size)
		{
			return false;
		}

		var rw = item.GetItemType<IRangedWeapon>();
		if (DesignedForGuns)
		{
			if (rw?.WeaponType.RangedWeaponType.IsFirearm() != true)
			{
				return false;
			}

			return true;
		}

		return true;
	}

	public string WhyCannotSheath(IGameItem item)
	{
		if (Content is not null)
		{
			return "the sheathe already has something in it";
		}

		if (!item.IsItemType<IWieldable>())
		{
			return "that is not a wieldable item";
		}

		if (MaximumSize < item.Size)
		{
			return "that is too large to fit in that sheathe";
		}

		var rw = item.GetItemType<IRangedWeapon>();
		if (DesignedForGuns)
		{
			if (rw?.WeaponType.RangedWeaponType.IsFirearm() != true)
			{
				return "only firearms can be sheathed in that sheathe";
			}
		}

		return "an unknown reason";
	}

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

	#region Implementation of IContainer

	/// <inheritdoc />
	public IEnumerable<IGameItem> Contents => _content?.Parent is not null ? [_content?.Parent] : [];

	/// <inheritdoc />
	public string ContentsPreposition => "in";

	/// <inheritdoc />
	public bool Transparent => false;

	/// <inheritdoc />
	public bool CanPut(IGameItem item)
	{
		return false;
	}

	/// <inheritdoc />
	public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
	{
		// Do nothing
	}

	/// <inheritdoc />
	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		return WhyCannotPutReason.NotContainer;
	}

	/// <inheritdoc />
	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return _content?.Parent == item;
	}

	/// <inheritdoc />
	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		Content = null;
		return item;
	}

	/// <inheritdoc />
	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		return WhyCannotGetContainerReason.NotContained;
	}

	/// <inheritdoc />
	public int CanPutAmount(IGameItem item)
	{
		return 0;
	}

	/// <inheritdoc />
	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
	{
		var location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
		var contents = Contents.ToList();
		Content = null;
		if (emptier is not null)
		{
			if (intoContainer == null)
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote("@ empty|empties $0 onto the ground.", emptier, Parent)).Append(
						playerEmote));
			}
			else
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote($"@ empty|empties $1 {intoContainer.ContentsPreposition}to $2.",
						emptier, emptier, Parent, intoContainer.Parent)).Append(playerEmote));
			}
		}

		foreach (var item in contents)
		{
			item.ContainedIn = null;
			if (intoContainer != null)
			{
				if (intoContainer.CanPut(item))
				{
					intoContainer.Put(emptier, item);
				}
				else if (location != null)
				{
					location.Insert(item);
					if (emptier != null)
					{
						emptier.OutputHandler.Handle(new EmoteOutput(new Emote(
							"@ cannot put $1 into $2, so #0 set|sets it down on the ground.", emptier, emptier, item,
							intoContainer.Parent)));
					}
				}
				else
				{
					item.Delete();
				}

				continue;
			}

			if (location != null)
			{
				location.Insert(item);
			}
			else
			{
				item.Delete();
			}
		}

		Changed = true;
	}

	#endregion
}