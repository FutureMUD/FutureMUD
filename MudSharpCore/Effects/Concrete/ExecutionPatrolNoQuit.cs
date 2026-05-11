using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Law;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class ExecutionPatrolNoQuit : Effect, INoQuitEffect
{
	private readonly long _patrolId;
	private IPatrol _patrol;

	public static void InitialiseEffectType()
	{
		RegisterFactory("ExecutionPatrolNoQuit", (effect, owner) => new ExecutionPatrolNoQuit(effect, owner));
	}

	public ExecutionPatrolNoQuit(ICharacter owner, IPatrol patrol) : base(owner, null)
	{
		_patrol = patrol;
		_patrolId = patrol.Id;
	}

	protected ExecutionPatrolNoQuit(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		if (long.TryParse(effect.Element("Effect")?.Element("Patrol")?.Value, out var patrolId))
		{
			_patrolId = patrolId;
		}
	}

	public IPatrol Patrol => _patrol ??= Gameworld.Patrols.Get(_patrolId);

	public string NoQuitReason => "You cannot quit while law enforcement is carrying out your death sentence.";

	public override bool Applies()
	{
		return base.Applies() && Patrol is not null && Patrol.LegalAuthority.Patrols.Contains(Patrol);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XElement("Patrol", _patrolId));
	}

	protected override string SpecificEffectType => "ExecutionPatrolNoQuit";

	public override string Describe(IPerceiver voyeur)
	{
		return Patrol is null
			? "Held in an expired execution patrol."
			: $"Being escorted for execution by the {Patrol.Name.ColourName()} patrol.";
	}
}
