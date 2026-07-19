using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public sealed class EmploymentCraftReservationEffect : Effect, INoGetEffect
{
	public EmploymentCraftReservationEffect(IPerceivable owner, Guid taskId, Guid correlationId, string taskName,
		string resourceDescription, DateTimeOffset expiresAt)
		: base(owner)
	{
		TaskId = taskId;
		CorrelationId = correlationId;
		TaskName = taskName;
		ResourceDescription = resourceDescription;
		ExpiresAt = expiresAt;
	}

	public Guid TaskId { get; }
	public Guid CorrelationId { get; }
	public string TaskName { get; }
	public string ResourceDescription { get; }
	public DateTimeOffset ExpiresAt { get; }
	public bool CombatRelated => false;

	protected override string SpecificEffectType => "EmploymentCraftReservation";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Reserved for employment task {TaskName.ColourName()} until {ExpiresAt.LocalDateTime.ToString("g", voyeur).ColourValue()} ({ResourceDescription}).";
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return oldItem == Owner
			? new EmploymentCraftReservationEffect(newItem, TaskId, CorrelationId, TaskName, ResourceDescription,
				ExpiresAt)
			: null;
	}

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return true;
	}
}
