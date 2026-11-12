using MudSharp.Economy.Employment;
using MudSharp.Effects;

#nullable enable

namespace MudSharp.Effects.Concrete;

internal sealed class EmploymentStasisEffect : Effect, IEffectSubtype
{
	public EmploymentStasisEffect(IPerceivable owner) : base(owner) { }
	private EmploymentStasisEffect(XElement xml, IPerceivable owner) : base(xml, owner) { }

	public static void InitialiseEffectType() =>
		RegisterFactory("EmploymentStasis", (xml, owner) => new EmploymentStasisEffect(xml, owner));

	protected override string SpecificEffectType => "EmploymentStasis";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => "In stasis while awaiting employment.";
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

internal sealed class EmploymentWorkerHostEvaluationEffect : Effect, IEffectSubtype
{
	public EmploymentWorkerHostEvaluationEffect(IPerceivable owner, IEmploymentHost host)
		: base(owner)
	{
		HostType = host.EmploymentHostType;
		HostId = host.Id;
	}

	public EmploymentHostType HostType { get; }
	public long HostId { get; }

	public bool Matches(IEmploymentHost host)
	{
		return HostType == host.EmploymentHostType &&
		       HostId == host.Id;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Employment worker host evaluation cooldown for {HostType} #{HostId:N0}.";
	}

	protected override string SpecificEffectType => "EmploymentWorkerHostEvaluation";
}
