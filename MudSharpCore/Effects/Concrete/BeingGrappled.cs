using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class BeingGrappled : Effect, IBeingGrappled
{
	public BeingGrappled(ICharacter owner, IGrappling grappling) : base(owner)
	{
		Grappling = grappling;
	}

	public override bool CanBeStoppedByPlayer => false;

	public override IEnumerable<string> Blocks => new[] { "general", "combat-engage" };

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return $"being grappled by {Grappling.CharacterOwner.HowSeen(voyeur, colour: false)}";
	}

	public IGrappling Grappling { get; set; }

	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Grappling;

	protected override string SpecificEffectType => "BeingGrappled";

	public Difficulty StruggleDifficulty =>
		// TODO - effects, merits, circumstances
		Difficulty.Normal;

	public bool UnderControl
	{
		get
		{
			var uselessLimbs = Grappling.Target.Body.Limbs
			                            .Where(x => Grappling.Target.Body.CanUseLimb(x) != CanUseLimbResult.CanUse)
			                            .Except(Grappling.LimbsUnderControl).ToList();
			return Grappling.LimbsUnderControl.Count() + uselessLimbs.Count >=
			       Gameworld.GetStaticInt("MinimumLimbsUnderControlForGrapple");
		}
	}

	public bool AppliesToLimb(ILimb limb)
	{
		return Grappling.LimbsUnderControl.Contains(limb);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Being grappled by {Grappling.CharacterOwner.HowSeen(voyeur)}, who has {Grappling.LimbsUnderControl.Select(x => x.Name.Colour(Telnet.Yellow)).DefaultIfEmpty("nothing").ListToString()} under control.";
	}

	public string SuffixFor(IPerceiver voyeur)
	{
		return $"subdued by {Grappling.CharacterOwner.HowSeen(voyeur)}";
	}

	public bool SuffixApplies()
	{
		return UnderControl;
	}
}