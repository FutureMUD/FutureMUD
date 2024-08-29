using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Groups;

public abstract class GameItemGroupForm : SaveableItem, IGameItemGroupForm
{
	private readonly IGameItemGroup _parent;
	protected readonly List<ICell> Cells = new();
	public sealed override string FrameworkItemType => "GameItemGroupForm";

	protected GameItemGroupForm(IGameItemGroup parent)
	{
		_parent = parent;
		Gameworld = parent.Gameworld;
	}

	protected GameItemGroupForm(ItemGroupForm form, IGameItemGroup parent)
	{
		_id = form.Id;
		_parent = parent;
		Gameworld = parent.Gameworld;
	}

	protected abstract string GameItemGroupFormType { get; }
	public abstract string Show(IPerceiver voyeur);

	#region IGameItemGroupForm Members

	public bool Applies(ICell cell)
	{
		return !Cells.Any() || Cells.Contains(cell);
	}

	public bool Applies(long cellId)
	{
		return !Cells.Any() || Cells.Any(x => x.Id == cellId);
	}

	public bool SpecialFormFor(long cellId)
	{
		return Cells.Any(x => x.Id == cellId);
	}

	public abstract string Describe(IPerceiver voyeur, IEnumerable<IGameItem> items);

	public virtual void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Last.ToLowerInvariant())
		{
			case "cell":
			case "room":
			case "location":
				BuildingCommandCell(actor, command);
				break;
			default:
				actor.Send("That is not a valid option for editing Item Group Forms.");
				return;
		}
	}

	private void BuildingCommandCell(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which cell do you want to add to or remove from this Item Group Form?");
			return;
		}

		if (!long.TryParse(command.Pop(), out var value))
		{
			actor.Send(
				"What is the ID number of the cell that you wish to add to or remove from this Item Group Form?");
			return;
		}

		var cell = Gameworld.Cells.Get(value);
		if (cell == null)
		{
			actor.Send("There is no such cell.");
			return;
		}

		if (Cells.Contains(cell))
		{
			Cells.Remove(cell);
			Changed = true;
			actor.Send("The Cell {0} (#{1:N0}) will no longer use that Item Group Form.", cell.Name, cell.Id);
			return;
		}

		Cells.Add(cell);
		actor.Send("The cell {0} (#{1:N0}) will now use this Item Group Form.", cell.Name, cell.Id);
		foreach (var form in _parent.Forms.Except(this).Where(x => x.Applies(cell)).Cast<GameItemGroupForm>())
		{
			form.Cells.Remove(cell);
			form.Changed = true;
			actor.Send("The cell was removed from form {0:N0}.", form.Id);
		}

		Changed = true;
	}

	public abstract string LookDescription(IPerceiver voyeur, IEnumerable<IGameItem> items);

	#endregion
}