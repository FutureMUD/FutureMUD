using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Groups;

public class GameItemGroupFormDeletionProposal : Proposal
{
	private readonly IGameItemGroupForm _form;
	private readonly IGameItemGroup _group;
	private readonly ICharacter _proponent;

	public GameItemGroupFormDeletionProposal(ICharacter proponent, IGameItemGroup group, IGameItemGroupForm form)
	{
		_form = form;
		_proponent = proponent;
		_group = group;
	}

	public override IEnumerable<string> Keywords => new[]
	{
		"group",
		"form",
		"delete"
	};

	public override void Accept(string message = "")
	{
		if (_form == null || !_proponent.Gameworld.ItemGroups.Any(x => x.Forms.Contains(_form)))
		{
			_proponent.Send("That item group form appears to have already been deleted.");
			return;
		}

		_proponent.Send("You permanently delete form {0:N0} from item group {1} (#{2:N0}).", _form.Id, _group.Name,
			_group.Id);

		using (new FMDB())
		{
			var dbitem = FMDB.Context.ItemGroupForms.Find(_form.Id);
			if (dbitem != null)
			{
				FMDB.Context.ItemGroupForms.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}

			_group.RemoveForm(_form);
		}
	}

	public override void Reject(string message = "")
	{
		_proponent.Send("You decide against deleting that item group form.");
	}

	public override void Expire()
	{
		_proponent.Send("You decide against deleting that item group form.");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return string.Format(voyeur, "Proposing to delete form {0:N0} from item group {1:N0}.", _form.Id, _group.Id);
	}
}