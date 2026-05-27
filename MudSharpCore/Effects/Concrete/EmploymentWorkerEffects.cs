using System;
using MudSharp.Economy.Employment;
using MudSharp.Effects;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Effects.Concrete;

internal sealed class EmploymentWorkerSearchCooldownEffect : Effect, IEffectSubtype
{
	public EmploymentWorkerSearchCooldownEffect(IPerceivable owner)
		: base(owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Employment worker job-search cooldown.";
	}

	protected override string SpecificEffectType => "EmploymentWorkerSearchCooldown";
}

internal sealed class EmploymentWorkerTaskContextEffect : Effect, IEffectSubtype
{
	public EmploymentWorkerTaskContextEffect(IPerceivable owner, IEmploymentHost host,
		IEmploymentActiveTask task, EmploymentTaskContext context)
		: base(owner)
	{
		HostType = host.EmploymentHostType;
		HostId = host.Id;
		TaskId = task.Id;
		Context = context;
	}

	public EmploymentHostType HostType { get; }
	public long HostId { get; }
	public Guid TaskId { get; }
	public EmploymentTaskContext Context { get; }

	public bool Matches(IEmploymentHost host, IEmploymentActiveTask task)
	{
		return HostType == host.EmploymentHostType &&
		       HostId == host.Id &&
		       TaskId == task.Id;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Employment worker task context for {HostType} #{HostId:N0}.";
	}

	protected override string SpecificEffectType => "EmploymentWorkerTaskContext";
}

internal sealed class EmploymentWorkerRejectedOpeningEffect : Effect, IEffectSubtype
{
	public EmploymentWorkerRejectedOpeningEffect(IPerceivable owner, IEmploymentHost host, IJobOpening opening)
		: base(owner)
	{
		HostType = host.EmploymentHostType;
		HostId = host.Id;
		OpeningId = opening.Id;
	}

	public EmploymentHostType HostType { get; }
	public long HostId { get; }
	public long OpeningId { get; }

	public bool Matches(IEmploymentHost host, IJobOpening opening)
	{
		return HostType == host.EmploymentHostType &&
		       HostId == host.Id &&
		       OpeningId == opening.Id;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Employment worker rejection memory for {HostType} #{HostId:N0} opening #{OpeningId:N0}.";
	}

	protected override string SpecificEffectType => "EmploymentWorkerRejectedOpening";
}
