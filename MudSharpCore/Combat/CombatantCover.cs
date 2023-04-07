using MudSharp.Body.Position;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Combat;

public class CombatantCover : ICombatantCover
{
	private bool _eventsReleased;

	public CombatantCover(IMortalPerceiver owner, IRangedCover cover, IProvideCover coveritem)
	{
		Owner = owner;
		Cover = cover;
		CoverItem = coveritem;
	}

	public IMortalPerceiver Owner { get; protected set; }
	public IRangedCover Cover { get; protected set; }
	public IProvideCover CoverItem { get; protected set; }

	public void RegisterEvents()
	{
		Owner.OnQuit += OwnerGone;
		Owner.OnDeleted += OwnerGone;
		Owner.OnDeath += OwnerGone;
		Owner.OnEngagedInMelee += OwnerEngagedInMelee;
		Owner.OnPositionChanged += OwnerPositionChanged;
		if (Owner is IMove ownerAsIMove)
		{
			if (!Cover.CoverStaysWhileMoving)
			{
				ownerAsIMove.OnStartMove += OwnerMoved;
			}

			ownerAsIMove.OnMoved += OwnerMoved;
		}

		if (CoverItem != null)
		{
			CoverItem.OnNoLongerProvideCover += CoverItemNoLongerSuitable;
			CoverItem.Parent.OnDeath += CoverItemDestroyed;
			CoverItem.Parent.OnQuit += CoverItemGone;
			CoverItem.Parent.OnDeleted += CoverItemGone;
			CoverItem.Parent.OnRemovedFromLocation += CoverItemGone;
		}
	}

	public void LeaveCover()
	{
		Owner.OutputHandler.Handle(new EmoteOutput(new Emote("@ have|has left cover.", Owner)));
		ReleaseEvents();
	}

	private void OwnerPositionChanged(IPerceivable target)
	{
		if (Owner.PositionTarget != CoverItem?.Parent)
		{
			Owner.OutputHandler.Handle(new EmoteOutput(new Emote("@ have|has left cover.", Owner)));
			ReleaseEvents();
			return;
		}

		if (Owner.PositionState.CompareTo(Cover.HighestPositionState) != PositionHeightComparison.Higher)
		{
			return;
		}

		Owner.OutputHandler.Handle(new EmoteOutput(new Emote("@ have|has left cover.", Owner)));
		ReleaseEvents();
	}

	private void OwnerGone(IPerceivable target)
	{
		ReleaseEvents();
	}

	private void OwnerMoved(object sender, MoveEventArgs args)
	{
		ReleaseEvents();
	}

	private void OwnerEngagedInMelee(IPerceivable target)
	{
		Owner.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ can no longer remain in cover because #0 have|has been engaged in melee!",
				Owner, Owner)));
		ReleaseEvents();
	}

	private void CoverItemGone(IPerceivable target)
	{
		Owner.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ can no longer remain in cover because $1 $1|are|is no longer there!", Owner,
				Owner, CoverItem.Parent)));
		ReleaseEvents();
	}

	private void CoverItemNoLongerSuitable(IPerceivable target)
	{
		Owner.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ can no longer remain in cover because $1 $1|are|is no longer there!", Owner,
				Owner, CoverItem.Parent)));
		ReleaseEvents();
	}

	private void CoverItemDestroyed(IPerceivable target)
	{
		Owner.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ can no longer remain in cover because $1 $1|have|has been destroyed!",
				Owner, Owner, CoverItem.Parent)));
		ReleaseEvents();
	}

	public void ReleaseEvents()
	{
		if (Owner.Cover == this)
		{
			Owner.Cover = null;
		}

		if (_eventsReleased)
		{
			return;
		}

		Owner.OnQuit -= OwnerGone;
		Owner.OnDeath -= OwnerGone;
		Owner.OnDeleted -= OwnerGone;
		Owner.OnEngagedInMelee -= OwnerEngagedInMelee;
		Owner.OnPositionChanged -= OwnerPositionChanged;
		if (Owner is IMove ownerAsIMove)
		{
			if (!Cover.CoverStaysWhileMoving)
			{
				ownerAsIMove.OnStartMove -= OwnerMoved;
			}

			ownerAsIMove.OnMoved -= OwnerMoved;
		}

		if (CoverItem != null)
		{
			CoverItem.OnNoLongerProvideCover -= CoverItemNoLongerSuitable;
			CoverItem.Parent.OnDeath -= CoverItemDestroyed;
			CoverItem.Parent.OnDeleted -= CoverItemGone;
			CoverItem.Parent.OnQuit -= CoverItemGone;
			CoverItem.Parent.OnRemovedFromLocation -= CoverItemGone;
		}

		_eventsReleased = true;
	}

	~CombatantCover()
	{
		if (!_eventsReleased)
		{
			ReleaseEvents();
		}
	}
}