using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework.Save;

namespace MudSharp.Framework.Revision;

public abstract class EditableItem : SavableKeywordedItem, IEditableRevisableItem
{
	protected bool _statusChanged;

	protected EditableItem(MudSharp.Models.EditableItem item)
	{
		RevisionNumber = item.RevisionNumber;
		Status = (RevisionStatus)item.RevisionStatus;
		ReviewerDate = item.ReviewerDate;
		ReviewerComment = item.ReviewerComment;
		ReviewerAccountID = item.ReviewerAccountId;
		BuilderDate = item.BuilderDate;
		BuilderAccountID = item.BuilderAccountId;
		BuilderComment = item.BuilderComment;
		ObsoleteDate = item.ObsoleteDate;
	}

	protected EditableItem(IAccount originator)
	{
		Status = RevisionStatus.UnderDesign;
		BuilderDate = DateTime.UtcNow;
		BuilderAccountID = originator?.Id ?? 1L;
		RevisionNumber = 0;
	}

	protected EditableItem(EditableItem item, IAccount originator)
	{
		Status = RevisionStatus.UnderDesign;
		BuilderDate = DateTime.UtcNow;
		BuilderAccountID = originator.Id;
		RevisionNumber = item.RevisionNumber + 1;
	}

	public virtual bool ReadOnly
	{
		get => false;
		set { }
	}

	public abstract string EditHeader();

	public void Save(MudSharp.Models.EditableItem item)
	{
		item.BuilderAccountId = BuilderAccountID;
		item.BuilderComment = BuilderComment;
		item.BuilderDate = BuilderDate;
		item.ObsoleteDate = ObsoleteDate;
		item.ReviewerAccountId = ReviewerAccountID;
		item.ReviewerComment = ReviewerComment;
		item.ReviewerDate = ReviewerDate;
		item.RevisionNumber = RevisionNumber;
		item.RevisionStatus = (int)Status;
		_statusChanged = false;
	}

	protected abstract IEnumerable<IEditableRevisableItem> GetAllSameId();

	#region IEditableItem Members

	public bool IsAssociatedBuilder(ICharacter character)
	{
		return GetAllSameId().Any(x => x.BuilderAccountID == character.Account.Id);
	}

	public int RevisionNumber { get; protected init; }

	public RevisionStatus Status { get; protected set; }

	public long BuilderAccountID { get; protected set; }

	public long? ReviewerAccountID { get; protected set; }

	public string BuilderComment { get; protected set; }

	public string ReviewerComment { get; protected set; }

	public DateTime BuilderDate { get; protected set; }

	public DateTime? ReviewerDate { get; protected set; }

	public DateTime? ObsoleteDate { get; protected set; }

	public virtual bool CanSubmit()
	{
		return true;
	}

	public virtual string WhyCannotSubmit()
	{
		return "There are problems that need correcting.";
	}

	public virtual bool ChangeStatus(RevisionStatus status, string comment, IAccount reviewer)
	{
		switch (status)
		{
			case RevisionStatus.Current:
			case RevisionStatus.Rejected:
				ReviewerComment = comment;
				ReviewerAccountID = reviewer?.Id ?? 1;
				ReviewerDate = DateTime.UtcNow;
				break;
			case RevisionStatus.PendingRevision:
				BuilderComment = comment;
				BuilderAccountID = reviewer?.Id ?? 1;
				BuilderDate = DateTime.UtcNow;
				break;
			case RevisionStatus.Revised:
			case RevisionStatus.Obsolete:
				ObsoleteDate = DateTime.UtcNow;
				break;
			case RevisionStatus.UnderDesign:
				BuilderComment = comment;
				BuilderAccountID = reviewer?.Id ?? 1;
				BuilderDate = DateTime.UtcNow;
				break;
		}

		Status = status;
		_statusChanged = true;
		Changed = true;
		return true;
	}

	public abstract bool BuildingCommand(ICharacter actor, StringStack command);

	public abstract string Show(ICharacter actor);

	public abstract IEditableRevisableItem CreateNewRevision(ICharacter initiator);

	#endregion
}