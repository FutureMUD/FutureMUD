using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.FutureProg;
using MudSharp.RPG.Law;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class WarnedByEnforcer : Effect, INoQuitEffect
{
	public WarnedByEnforcer(ICharacter owner, ILegalAuthority authority, ICrime crime, IPatrol patrol) : base(owner,
		null)
	{
		WhichAuthority = authority;
		WhichCrime = crime;
		WhichPatrol = patrol;
		CharacterOwner = owner;
		SubscribeEvents();
	}

	public WarnedByEnforcer(XElement root, IPerceivable owner) : base(root, owner)
	{
		WhichAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("Authority").Value));
		WhichCrime = Gameworld.Crimes.Get(long.Parse(root.Element("Crime").Value));
		var patrolId = long.Parse(root.Element("Patrol").Value);
		WhichPatrol = WhichAuthority.Patrols.First(x => x.Id == patrolId);
		CharacterOwner = (ICharacter)owner;
		SubscribeEvents();
	}

	public ILegalAuthority WhichAuthority { get; init; }
	public ICrime WhichCrime { get; init; }
	public IPatrol WhichPatrol { get; init; }
	public ICharacter CharacterOwner { get; init; }

	public void SubscribeEvents()
	{
		CharacterOwner.OnStartMove += Owner_OnStartMove;
		CharacterOwner.OnWantsToMove += Owner_OnWantsToMove;
		CharacterOwner.OnLocationChanged += Owner_OnLocationChanged;
		CharacterOwner.OnMovedConsensually += Owner_OnMovedConsensually;
	}

	public void UnsubscribeEvents()
	{
		CharacterOwner.OnStartMove -= Owner_OnStartMove;
		CharacterOwner.OnWantsToMove -= Owner_OnWantsToMove;
		CharacterOwner.OnLocationChanged -= Owner_OnLocationChanged;
		CharacterOwner.OnMovedConsensually -= Owner_OnMovedConsensually;
	}

	private void Owner_OnLocationChanged(Form.Shape.ILocateable locatable, Construction.Boundary.ICellExit exit)
	{
		CharacterOwner.RemoveEffect(this, true);
	}

	private void Owner_OnMovedConsensually(object sender, Movement.MoveEventArgs e)
	{
		WhichPatrol.CriminalFailedToComply(CharacterOwner, WhichCrime);
	}

	private void Owner_OnWantsToMove(IPerceivable owner, PerceivableRejectionResponse response)
	{
		if (CharacterOwner.Account.ActLawfully && CrimeTypes.ResistArrest.CheckWouldBeACrime(CharacterOwner))
		{
			response.Rejected = true;
			response.Reason =
				"You are acting lawfully and that movement would cause you to commit a crime. If you want to move anyway, please disable automatically acting lawfully.";
		}
	}

	private void Owner_OnStartMove(object sender, Movement.MoveEventArgs e)
	{
		if (e.Movement.IsConsensualMover(CharacterOwner))
		{
			WhichPatrol.CriminalStartedMoving(CharacterOwner, WhichCrime);
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Warned to submit to arrest for the crime of {WhichCrime.DescribeCrime(voyeur).ColourValue()}.";
	}

	public override bool SavingEffect => base.SavingEffect;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Authority", WhichAuthority.Id),
			new XElement("Patrol", WhichPatrol.Id),
			new XElement("Crime", WhichCrime.Id)
		);
	}

	protected override string SpecificEffectType => "WarnedByEnforcer";

	public string NoQuitReason => "You cannot quit while you are being interacted with by law enforcement.";

	public override bool Applies(object target)
	{
		if (target is ILegalAuthority authority)
		{
			return target == WhichAuthority;
		}

		return base.Applies(target);
	}

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this, false);
		UnsubscribeEvents();
		WhichPatrol.CriminalFailedToComply(CharacterOwner, WhichCrime);
	}

	public override void RemovalEffect()
	{
		UnsubscribeEvents();
	}
}