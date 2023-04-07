using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using MudSharp.Work.Crafts;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;

namespace MudSharp.Framework.Revision;

public class EditableItemReviewProposal<T> : Proposal, IProposal where T : IEditableRevisableItem
{
	protected ICharacter _proponent;
	protected List<T> _reviewItems;

	public EditableItemReviewProposal(ICharacter proponent, List<T> reviewItems)
	{
		_proponent = proponent;
		_reviewItems = reviewItems;
	}

	private IEnumerable<IEditableRevisableItem> GetAppropriateAll()
	{
		if (typeof(T) == typeof(IGameItemProto))
		{
			return _proponent.Gameworld.ItemProtos;
		}

		if (typeof(T) == typeof(IGameItemComponentProto))
		{
			return _proponent.Gameworld.ItemComponentProtos;
		}

		if (typeof(T) == typeof(ICellOverlayPackage))
		{
			return _proponent.Gameworld.CellOverlayPackages;
		}

		if (typeof(T) == typeof(INPCTemplate))
		{
			return _proponent.Gameworld.NpcTemplates;
		}

		if (typeof(T) == typeof(IForagable))
		{
			return _proponent.Gameworld.Foragables;
		}

		if (typeof(T) == typeof(IForagableProfile))
		{
			return _proponent.Gameworld.ForagableProfiles;
		}

		if (typeof(T) == typeof(ICraft))
		{
			return _proponent.Gameworld.Crafts;
		}

		if (typeof(T) == typeof(IProject))
		{
			return _proponent.Gameworld.Projects;
		}

		if (typeof(T) == typeof(ITattooTemplate))
		{
			return _proponent.Gameworld.DisfigurementTemplates;
		}

		if (typeof(T) == typeof(IGameItemSkin))
		{
			return _proponent.Gameworld.ItemSkins;
		}

		throw new NotSupportedException("Not supported type in EDitableItemREviewProposal");
	}

	#region IProposal Members

	public override void Accept(string message = "")
	{
		foreach (var item in _reviewItems)
		{
			foreach (
				var olditem in
				GetAppropriateAll().Where(x => x.Id == item.Id && x.Status == RevisionStatus.Current))
			{
				olditem.ChangeStatus(RevisionStatus.Obsolete, "", _proponent.Account);
			}

			item.ChangeStatus(RevisionStatus.Current, message, _proponent.Account);
		}

		_proponent.OutputHandler.Send(
			$"You approve {(_reviewItems.Count <= 3 ? _reviewItems.Select(x => x.EditHeader().ColourName()).ListToString() : _reviewItems.Count + " submissions")}{(message.Length > 0 ? $", with the comment \"{message}\"" : ".")}");
	}

	public override void Reject(string message = "")
	{
		foreach (var item in _reviewItems)
		{
			item.ChangeStatus(RevisionStatus.Rejected, message, _proponent.Account);
		}

		_proponent.OutputHandler.Send("You reject " +
		                              (_reviewItems.Count <= 3
			                              ? _reviewItems.Select(x => x.EditHeader().ColourName()).ListToString()
			                              : _reviewItems.Count + " submissions") +
		                              (message.Length > 0 ? ", with the comment \"" + message + "\"" : "."));
	}

	public override void Expire()
	{
		_proponent.OutputHandler.Send("Your review of building work has expired.");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return (_proponent.Body == voyeur ? "You are" : _proponent.Body.HowSeen(voyeur, true) + " is") +
		       " proposing to approve " +
		       (_reviewItems.Count <= 3
			       ? _reviewItems.Select(x => x.EditHeader().ColourName()).ListToString()
			       : _reviewItems.Count + " submissions") + ".";
	}

	#endregion

	#region IKeyworded Members

	private static readonly List<string> _keywords = new() { "edit", "item", "review" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}