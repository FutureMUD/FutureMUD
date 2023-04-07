using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class BreathingFilterGameItemComponent : GameItemComponent, IProvideGasForBreathing, IContainer
{
	protected BreathingFilterGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BreathingFilterGameItemComponentProto)newProto;
	}

	#region Constructors

	public BreathingFilterGameItemComponent(BreathingFilterGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BreathingFilterGameItemComponent(MudSharp.Models.GameItemComponent component,
		BreathingFilterGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BreathingFilterGameItemComponent(BreathingFilterGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		_installedFilterConsumable = Gameworld.TryGetItem(long.Parse(root.Element("Consumable").Value), true);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BreathingFilterGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Consumable", _installedFilterConsumable?.Id ?? 0)
		).ToString();
	}

	#endregion

	private IGameItem _installedFilterConsumable;

	#region IProvideGasForBreathing Implementation

	private bool Functional()
	{
		return _installedFilterConsumable != null && _installedFilterConsumable.Condition > 0.0;
	}

	public IGas Gas
	{
		get
		{
			if (Parent.InInventoryOf?.Location.Atmosphere is not IGas atmosphere)
			{
				return null;
			}

			if (Functional() && _prototype.FilteredGases.ContainsKey(atmosphere))
			{
				return _prototype.FilteredGases[atmosphere];
			}

			return atmosphere;
		}
	}

	public bool CanConsumeGas(double volume)
	{
		return true;
	}

	public bool ConsumeGas(double volume)
	{
		if (Functional())
		{
			_installedFilterConsumable.Condition -= volume / _prototype.VolumePerFilter;
		}

		return true;
	}

	public bool WaterTight => false;

	#endregion

	#region IContainer Implementation

	public IEnumerable<IGameItem> Contents => _installedFilterConsumable != null
		? new[] { _installedFilterConsumable }
		: Enumerable.Empty<IGameItem>();

	public string ContentsPreposition => "in";
	public bool Transparent => false;

	public bool CanPut(IGameItem item)
	{
		return
			_installedFilterConsumable == null &&
			item.Prototype.Id == _prototype.FilterProtoId &&
			item.Quantity == 1;
	}

	public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
	{
		_installedFilterConsumable = item;
		item.ContainedIn = Parent;
		Changed = true;
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (_installedFilterConsumable != null)
		{
			return WhyCannotPutReason.ContainerFull;
		}

		if (item.Prototype.Id != _prototype.FilterProtoId)
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		return WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity;
	}

	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return _installedFilterConsumable == item;
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		_installedFilterConsumable = null;
		Changed = true;
		return item;
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		return WhyCannotGetContainerReason.NotContained;
	}

	public int CanPutAmount(IGameItem item)
	{
		return 1;
	}

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
	{
		var location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
		var contents = Contents.ToList();
		_installedFilterConsumable = null;
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

	public override void Quit()
	{
		base.Quit();
		_installedFilterConsumable?.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		_installedFilterConsumable?.Delete();
	}

	public override void Login()
	{
		_installedFilterConsumable?.Login();
	}
}