#nullable enable

using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public sealed class EmploymentCraftResourceReservationEffect : EmploymentCraftReservationEffect, INoGetEffect
{
	public EmploymentCraftResourceReservationEffect(IPerceivable owner, Guid taskId, Guid correlationId,
		string taskName, string resourceDescription, DateTimeOffset expiresAt)
		: base(owner, taskId, correlationId, taskName, resourceDescription, expiresAt)
	{
	}

	public override IEffect? NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return oldItem == Owner
			? new EmploymentCraftResourceReservationEffect(newItem, TaskId, CorrelationId, TaskName,
				ResourceDescription, ExpiresAt)
			: null;
	}

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem) => true;
}
