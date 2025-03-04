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
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class TableGameItemComponent : GameItemComponent, ITable, IFlip, IProvideCover
{
	private TableGameItemComponentProto _prototype;

	public TableGameItemComponent(TableGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public TableGameItemComponent(MudSharp.Models.GameItemComponent component, TableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public TableGameItemComponent(TableGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
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
		return new TableGameItemComponent(this, newParent, temporary);
	}

	public override void Quit()
	{
		base.Quit();
		foreach (var item in Chairs)
		{
			item.Parent.Quit();
		}
	}

	public override void Login()
	{
		foreach (var item in Chairs)
		{
			item.Parent.Login();
		}
	}

	public override bool PreventsRepositioning()
	{
		return Chairs.Any();
	}

	public override string WhyPreventsRepositioning()
	{
		return " currently has chairs. They must first be removed.";
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (Flipped && type == DescriptionType.Long) || type == DescriptionType.Full;
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
		var newItemTable = newItem?.GetItemType<ITable>();
		if (newItemTable != null)
		{
			foreach (var item in Chairs.ToList())
			{
				if (newItemTable.CanAddChair(null, item))
				{
					newItemTable.AddChair(null, item);
					item.SetTable(newItemTable);
				}
				else
				{
					if (location != null)
					{
						location.Insert(item.Parent);
						item.SetTable(null);
					}
					else
					{
						item.Delete();
					}
				}
			}

			newItemTable.Changed = true;
			_chairs.Clear();
			return false;
		}

		foreach (var item in Chairs)
		{
			if (location != null)
			{
				location.Insert(item.Parent);
				item.SetTable(null);
			}
			else
			{
				item.Delete();
			}
		}

		_chairs.Clear();
		return false;
	}

	public override void FinaliseLoad()
	{
		foreach (var item in Chairs)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TableGameItemComponentProto)newProto;
	}

	protected void LoadFromXml(XElement root)
	{
		var oldNoSave = _noSave;
		_noSave = true;
		using (new FMDB())
		{
			foreach (var element in root.Elements("Chair"))
			{
				var newItem = Gameworld.TryGetItem(long.Parse(element.Value), true);
				var chair = newItem.GetItemType<IChair>();
				chair.SetTable(this, true); // This causes the table to save itself, hence the save-lock outside
			}
		}

		_flipped = bool.Parse(root.Element("Flipped")?.Value ?? "false");
		_noSave = oldNoSave;
	}

	protected override string SaveToXml()
	{
		return new XElement("Table",
			new XElement("Flipped", Flipped),
			from chair in Chairs
			select new XElement("Chair", chair.Parent.Id)
		).ToString();
	}

	#region ITable Members

	public int MaximumChairSlots => _prototype.MaximumChairSlots;

	private readonly List<IChair> _chairs = new();
	public IEnumerable<IChair> Chairs => _chairs;

	public void AddChair(ICharacter character, IChair chair)
	{
		_chairs.Add(chair);
		Changed = true;
	}

	public bool CanAddChair(ICharacter character, IChair chair)
	{
		return !_chairs.Contains(chair) &&
		       _chairs.Sum(x => x.ChairSlotsUsed) + chair.ChairSlotsUsed <= MaximumChairSlots &&
		       !Flipped;
	}

	public override bool Take(IGameItem item)
	{
		if (Chairs.Any(x => x.Parent == item))
		{
			item.GetItemType<IChair>().SetTable(null);
			return true;
		}

		return false;
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

		if (_chairs.Sum(x => x.ChairSlotsUsed) + chair.ChairSlotsUsed > MaximumChairSlots)
		{
			return "That chair will not fit at the table.";
		}

		return "You cannot add that chair to the table.";
	}

	public bool CanRemoveChair(ICharacter character, IChair chair)
	{
		// TODO - other reasons why this might be the case
		return !chair.Occupants.Any();
	}

	public string WhyCannotRemoveChair(ICharacter character, IChair chair)
	{
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