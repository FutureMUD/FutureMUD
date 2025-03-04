using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class BenchGameItemComponent : GameItemComponent, ITable, IFlip, IProvideCover
{
	private BenchGameItemComponentProto _prototype;

	public BenchGameItemComponent(BenchGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		if (_prototype.ChairProto == null)
		{
			return;
		}

		for (var i = 0; i < _prototype.ChairCount; i++)
		{
			var newItem = _prototype.ChairProto.CreateNew();
			if (!temporary)
			{
				Gameworld.Add(newItem);
			}

			var newChair = newItem.GetItemType<IChair>();
			_permanentChairs.Add(newChair);
			newChair.SetTable(this);
			newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
			newItem.Login();
		}

		Changed = true;
	}

	public BenchGameItemComponent(MudSharp.Models.GameItemComponent component, BenchGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BenchGameItemComponent(BenchGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Chairs.ToList())
		{
			_chairs.Remove(item);
			item.Parent.Delete();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BenchGameItemComponent(this, newParent, temporary);
	}

	public override void Quit()
	{
		foreach (var item in Chairs)
		{
			item.Parent.Quit();
		}
	}

	public override bool PreventsRepositioning()
	{
		return Chairs.Any(x => !_permanentChairs.Contains(x));
	}

	public override string WhyPreventsRepositioning()
	{
		return " currently has non-fixed chairs. They must first be removed.";
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (type == DescriptionType.Long && Flipped) || type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Long && Flipped)
		{
			return $"{description} {"(Flipped)".Colour(Telnet.BoldWhite)}";
		}

		if (_chairs.Any())
		{
			var sb = new StringBuilder(description);
			sb.AppendLine();
			sb.AppendLine("It has the following chairs:");
			foreach (var chair in Chairs)
			{
				sb.AppendLineFormat("\t{0}", chair.Parent.HowSeen(voyeur));
			}

			return sb.ToString();
		}

		return description;
	}

	public override int DecorationPriority => 1000;

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		if (!Chairs.Any())
		{
			return false;
		}

		var newItemBench = newItem?.GetItemType<BenchGameItemComponent>();
		if (newItemBench != null)
		{
			foreach (var item in Chairs)
			{
				if (_permanentChairs.Contains(item))
				{
					newItemBench._permanentChairs.Add(item);
				}

				newItemBench._chairs.Add(item);
				item.SetTable(newItemBench);
			}

			newItemBench.Changed = true;
			_permanentChairs.Clear();
			_chairs.Clear();
			return false;
		}

		var newItemTable = newItem?.GetItemType<ITable>();
		if (newItemTable != null)
		{
			foreach (var item in Chairs)
			{
				if (newItemTable.CanAddChair(null, item))
				{
					newItemTable.AddChair(null, item);
					item.SetTable(newItemTable);
				}
				else if (location != null)
				{
					location.Insert(item.Parent);
					item.SetTable(null);
				}
				else
				{
					item.Delete();
				}
			}

			newItemTable.Changed = true;
			_permanentChairs.Clear();
			_chairs.Clear();
			return false;
		}

		foreach (var item in Chairs.ToList())
		{
			if (location != null)
			{
				item.SetTable(null);
				location.Insert(item.Parent);
			}
			else
			{
				item.Delete();
			}
		}

		_permanentChairs.Clear();
		_chairs.Clear();
		return false;
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _chairs)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BenchGameItemComponentProto)newProto;
		// TODO - bench chairs changing
	}

	protected void LoadFromXml(XElement root)
	{
		var oldNoSave = _noSave;
		_noSave = true;
		foreach (var element in root.Elements("Chair"))
		{
			var newItem = Gameworld.TryGetItem(long.Parse(element.Value), true);
			var chair = newItem.GetItemType<IChair>();
			if (element.Attribute("IsFixed") != null && bool.Parse(element.Attribute("IsFixed").Value))
			{
				_permanentChairs.Add(chair);
			}

			chair.SetTable(this, true); // This causes the table to save itself, hence the save-lock outside
		}

		_flipped = bool.Parse(root.Element("Flipped")?.Value ?? "false");
		_noSave = oldNoSave;
	}

	protected override string SaveToXml()
	{
		return new XElement("Table",
			new XElement("Flipped", Flipped),
			new object[]
			{
				from chair in Chairs
				select
					new XElement("Chair", new XAttribute("IsFixed", _permanentChairs.Contains(chair)),
						new XText(chair.Parent.Id.ToString()))
			}).ToString();
	}

	#region ITable Members

	public int MaximumChairSlots => _prototype.MaximumChairSlots;

	private readonly List<IChair> _chairs = new();
	public IEnumerable<IChair> Chairs => _chairs;

	private readonly List<IChair> _permanentChairs = new();

	public void AddChair(ICharacter character, IChair chair)
	{
		_chairs.Add(chair);
		Changed = true;
	}

	public bool CanAddChair(ICharacter character, IChair chair)
	{
		return !_chairs.Contains(chair) &&
		       _chairs.Sum(x => x.ChairSlotsUsed) + chair.ChairSlotsUsed <= MaximumChairSlots && !Flipped;
	}

	public string WhyCannotAddChair(ICharacter character, IChair chair)
	{
		if (Flipped)
		{
			return
				"You cannot add any chairs to the table while it has been flipped. Flip it back up the right way first.";
		}

		if (_chairs.Contains(chair))
		{
			return "That chair is already at the table.";
		}

		return _chairs.Sum(x => x.ChairSlotsUsed) + chair.ChairSlotsUsed > MaximumChairSlots
			? "That chair will not fit at the table."
			: "You cannot add that chair to the table.";
	}

	public bool CanRemoveChair(ICharacter character, IChair chair)
	{
		// TODO - other reasons why this might be the case
		return !_permanentChairs.Contains(chair) && !chair.Occupants.Any();
	}

	public string WhyCannotRemoveChair(ICharacter character, IChair chair)
	{
		if (_permanentChairs.Contains(chair))
		{
			return "That chair cannot be removed, as it is permanently affixed.";
		}

		return chair.Occupants.Any() ? "That chair is still occupied." : "You cannot remove that chair";
	}

	public void RemoveChair(ICharacter character, IChair chair)
	{
		_chairs.Remove(chair);
		Changed = true;
	}

	#endregion

	#region IFlippable Implementation

	private bool _flipped;

	public bool Flipped
	{
		get => _flipped;

		set
		{
			if (!_flipped && value)
			{
				DoFlip();
			}

			if (_flipped != value)
			{
				if (_flipped && _prototype.CoverWhenFlipped != null && _prototype.CoverWhenNotFlipped == null)
				{
					OnNoLongerProvideCover?.Invoke(Parent);
				}
				else if (!_flipped && _prototype.CoverWhenFlipped == null && _prototype.CoverWhenNotFlipped != null)
				{
					OnNoLongerProvideCover?.Invoke(Parent);
				}
			}

			_flipped = value;
			Changed = true;
		}
	}

	private void DoFlip()
	{
		foreach (var chair in Chairs.ToList())
		{
			Parent.Location.Insert(chair.Parent);
			chair.Parent.SetPosition(PositionUndefined.Instance, PositionModifier.Behind, Parent, null);
		}

		_chairs.Clear();
		if (!_flipped && (Parent.GetItemType<IContainer>()?.Contents.Any() ?? false))
		{
			Parent.OutputHandler.Handle(new EmoteOutput(new Emote("The things on @ fall to the ground.", Parent)));
			var container = Parent.GetItemType<IContainer>();
			foreach (var item in container.Contents.ToList())
			{
				container.Take(item);
				Parent.Location.Insert(item);
				item.SetPosition(PositionUndefined.Instance, PositionModifier.Before, Parent, null);
			}
		}
	}

	public bool Flip(ICharacter flipper, IEmote playerEmote = null, bool silent = false)
	{
		if (!CanFlip(flipper))
		{
			flipper?.OutputHandler.Handle(new EmoteOutput(new Emote(WhyCannotFlip(flipper), flipper, flipper, Parent)));
			return false;
		}

		if (flipper != null)
		{
			flipper.OutputHandler.Handle(Flipped
				? new MixedEmoteOutput(new Emote("@ right|rights $0", flipper, Parent))
					.Append(playerEmote)
				: new MixedEmoteOutput(new Emote("@ flip|flips $0", flipper, Parent))
					.Append(playerEmote));
		}
		else
		{
			Parent.OutputHandler.Handle(Flipped
				? new EmoteOutput(new Emote("@ flip|flips back over.", Parent))
				: new EmoteOutput(new Emote("@ flip|flips over.", Parent)));
		}

		Flipped = true;
		return true;
	}

	public bool CanFlip(ICharacter flipper)
	{
		if (flipper == null)
		{
			return true;
		}

		if (_prototype.TraitsToFlipExpression.Evaluate(flipper) < 0.0)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotFlip(ICharacter flipper)
	{
		if (_prototype.TraitsToFlipExpression.Evaluate(flipper) < 0.0)
		{
			return _prototype.CannotFlipTraitMessage;
		}

		throw new ApplicationException("Unsupported reason that a table cannot be flipped.");
	}

	#endregion

	#region IProvideCover Implementation

	public IRangedCover Cover => Flipped ? _prototype.CoverWhenFlipped : _prototype.CoverWhenNotFlipped;

	public bool IsProvidingCover
	{
		get => Cover != null;
		set
		{
			// Do nothing
		}
	}

	public event PerceivableEvent OnNoLongerProvideCover;

	#endregion
}