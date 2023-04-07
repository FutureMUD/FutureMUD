using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health.Infections;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class ReplantedBodypartsEffect : Effect, IReplantedBodypartsEffect
{
	public ReplantedBodypartsEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXML(effect.Element("Effect"));
	}

	public ReplantedBodypartsEffect(IBody owner, IBodypart rootbodypart,
		Difficulty resistrejectiondifficulty) : base(owner)
	{
		Bodypart = rootbodypart;
		ResistRejectionDifficulty = resistrejectiondifficulty;
	}

	public int ChecksAtCurrentDifficulty { get; set; }

	protected override string SpecificEffectType => "Replanted Bodyparts";

	public Difficulty ResistRejectionDifficulty { get; set; }

	public IBodypart Bodypart { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"the {Bodypart.FullDescription()} was recently replanted, and has {20 - ChecksAtCurrentDifficulty:N0} more checks @ {ResistRejectionDifficulty.Describe()} before downgrade.";
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && target == Bodypart;
	}

	public override bool SavingEffect { get; } = true;

	public override void ExpireEffect()
	{
		if (ChecksAtCurrentDifficulty++ >= 20)
		{
			if (ResistRejectionDifficulty == Difficulty.Automatic)
			{
				base.ExpireEffect();
				var bOwner = Owner as IBody;
				var limb = bOwner.GetLimbFor(Bodypart);
				Owner.OutputHandler.Send(
					$"You feel as if your {(limb != null ? limb.Name : Bodypart.Name)} is entirely healed, and a part of your body.");
				return;
			}

			ChecksAtCurrentDifficulty = 0;
			ResistRejectionDifficulty = ResistRejectionDifficulty.StageDown(1);
		}

		Changed = true;
		CheckInfections();
		Gameworld.EffectScheduler.AddSchedule(new EffectSchedule(this, TimeSpan.FromSeconds(600)));
	}

	private void LoadFromXML(XElement root)
	{
		Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Bodypart").Value));
		ResistRejectionDifficulty = (Difficulty)int.Parse(root.Element("Difficulty").Value);
		ChecksAtCurrentDifficulty = int.Parse(root.Element("Checks").Value);
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XElement("Bodypart", Bodypart.Id),
				new XElement("Difficulty", (int)ResistRejectionDifficulty),
				new XElement("Checks", ChecksAtCurrentDifficulty));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("Replanted Bodyparts", (effect, owner) => new ReplantedBodypartsEffect(effect, owner));
	}

	private void CheckInfections()
	{
		var bodyOwner = (IBody)Owner;
		var check = Gameworld.GetCheck(CheckType.ReplantedBodypartRejectionCheck);
		var result = check.Check(bodyOwner.Actor, ResistRejectionDifficulty, default(IPerceivable), null,
			Owner.EffectsOfType<IImmuneBonusEffect>().Where(x => x.Applies()).Select(x => x.ImmuneBonus).Sum());
		if (result.Outcome == Outcome.MajorFail)
		{
			var eligableParts =
				bodyOwner.Bodyparts.Where(x => x.DownstreamOfPart(Bodypart) || x == Bodypart).ToList();
			var eligableOrgans =
				bodyOwner.Organs.Where(
					x =>
						eligableParts.Any(y => y.Organs.Contains(x)) &&
						bodyOwner.PartInfections.All(y => y.Bodypart != x)).ToList();
			eligableParts = eligableParts.Where(x => bodyOwner.PartInfections.All(y => y.Bodypart != x)).ToList();
			var terrain = bodyOwner.Location.Terrain(bodyOwner.Actor);
			if (eligableOrgans.Any() && Dice.Roll(1, 6) == 6)
			{
				bodyOwner.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection,
					ResistRejectionDifficulty, 0.0001, bodyOwner, null,
					eligableOrgans.GetWeightedRandom(x => 1.0 / x.RelativeInfectability), terrain.InfectionMultiplier));
				return;
			}

			if (eligableParts.Any())
			{
				bodyOwner.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection,
					ResistRejectionDifficulty, 0.0001, bodyOwner, null, eligableParts.GetRandomElement(),
					terrain.InfectionMultiplier));
			}
		}
	}
}