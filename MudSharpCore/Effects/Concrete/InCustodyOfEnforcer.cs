using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;
#nullable enable
public class InCustodyOfEnforcer : Effect, IEffect
{
	private long _enforcerId;
	private ICharacter? _enforcer;

	public ICharacter Enforcer
	{
		get
		{
			if (_enforcer == null)
			{
				_enforcer = Gameworld.TryGetCharacter(_enforcerId, true);
			}

			return _enforcer;
		}
		set
		{
			_enforcer = value;
			_enforcerId = value.Id;
		}
	}

	public ICharacter CharacterOwner => (ICharacter)Owner;

	public ILegalAuthority LegalAuthority { get; set; }

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

	private void Owner_OnMovedConsensually(object? sender, Movement.MoveEventArgs e)
	{
		if (!e.Movement.CharacterMovers.Contains(Enforcer))
		{
			LegalAuthority.CheckPossibleCrime(CharacterOwner, CrimeTypes.EscapeCaptivity, Enforcer, null, "");
		}
	}

	private void Owner_OnWantsToMove(IPerceivable owner, PerceivableRejectionResponse response)
	{
		if (CharacterOwner.Account.ActLawfully && CrimeTypes.EscapeCaptivity.CheckWouldBeACrime(CharacterOwner))
		{
			response.Rejected = true;
			response.Reason =
				"You are acting lawfully and that movement would cause you to commit a crime. If you want to move anyway, please disable automatically acting lawfully.";
		}
	}

	private void Owner_OnStartMove(object? sender, Movement.MoveEventArgs e)
	{
		if (e.Movement.IsConsensualMover(CharacterOwner))
		{
			CharacterOwner.OutputHandler.Send(
				$"Warning: You are escaping the custody of {Enforcer.HowSeen(CharacterOwner)}, which will cause you to commit a crime if you do not stop immediately."
					.ColourIncludingReset(Telnet.BoldRed));
		}
	}

	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("InCustodyOfEnforcer", (effect, owner) => new InCustodyOfEnforcer(effect, owner));
	}

	#endregion

	#region Constructors

	public InCustodyOfEnforcer(ICharacter owner, ICharacter enforcer, ILegalAuthority authority) : base(owner, null)
	{
		Enforcer = enforcer;
		LegalAuthority = authority;
		SubscribeEvents();
	}

	protected InCustodyOfEnforcer(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		_enforcerId = long.Parse(root.Element("Enforcer").Value);
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("Authority").Value));
		SubscribeEvents();
	}

	#endregion

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Enforcer", _enforcerId),
			new XElement("Authority", LegalAuthority.Id)
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "InCustodyOfEnforcer";

	public override string Describe(IPerceiver voyeur)
	{
		return $"In the custody of {Enforcer.HowSeen(voyeur)} for authority {LegalAuthority.Name.ColourName()}";
	}

	public override bool SavingEffect => true;

	public override bool Applies(object target)
	{
		if (target is ILegalAuthority authority)
		{
			return base.Applies(target) && authority == LegalAuthority && CharacterOwner.ColocatedWith(Enforcer);
		}

		return base.Applies(target);
	}

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this, false);
		UnsubscribeEvents();
	}

	public override void RemovalEffect()
	{
		UnsubscribeEvents();
	}

	#endregion
}