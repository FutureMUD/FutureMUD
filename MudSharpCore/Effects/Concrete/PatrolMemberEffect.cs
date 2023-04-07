using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Law;

namespace MudSharp.Effects.Concrete;

public class PatrolMemberEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("PatrolMember", (effect, owner) => new PatrolMemberEffect(effect, owner));
	}

	public ICharacter CharacterOwner { get; set; }
	private long _patrolId;
	private IPatrol _patrol;

	public IPatrol Patrol
	{
		get
		{
			if (_patrol == null)
			{
				_patrol = Gameworld.Patrols.Get(_patrolId);
			}

			return _patrol;
		}
	}

	public PatrolMemberEffect(ICharacter owner, IPatrol patrol) : base(owner, null)
	{
		_patrol = patrol;
		_patrolId = patrol.Id;
		CharacterOwner = owner;
	}

	protected PatrolMemberEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		_patrolId = long.Parse(root.Element("Effect").Element("Patrol").Value);
		CharacterOwner = (ICharacter)owner;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"A member of the {Patrol.PatrolRoute.Name.ColourName()} patrol";
	}

	protected override string SpecificEffectType => "PatrolMember";
	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XElement("Patrol", Patrol.Id));
	}
}