using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health.Infections;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class SurgeryFinalisationRequired : Effect, ISurgeryFinalisationRequiredEffect, IDescriptionAdditionEffect
{
	public SurgeryFinalisationRequired(IPerceivable owner, IBodypart bodypart, Difficulty difficulty,
		IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Bodypart = bodypart;
		Difficulty = difficulty;
	}

	public SurgeryFinalisationRequired(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	protected override string SpecificEffectType => "SurgeryFinalisationRequired";

	public string AdditionalText
		=>
			$"There is an open surgical wound on their {Bodypart.FullDescription()} that requires urgent medical attention!";

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour && Colour != null ? AdditionalText.Colour(Colour) : AdditionalText;
	}

	public ANSIColour Colour => Telnet.BoldRed;

	public bool PlayerSet => false;

	public IBodypart Bodypart { get; set; }

	public Difficulty OriginalDifficulty { get; set; }

	public Difficulty Difficulty { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Owner.HowSeen(voyeur, true)} has an open surgical wound on {Owner.ApparentGender(voyeur).Possessive()} {Bodypart.FullDescription()} that requires treatment ({Difficulty.Describe()}).";
	}

	public void BumpDifficulty()
	{
		var bumped = Difficulty.StageUp(1);
		if (bumped < Difficulty.Impossible && bumped <= OriginalDifficulty + 3)
		{
			Difficulty = bumped;
			Changed = true;
		}
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && (target == Bodypart || target is IPerceiver);
	}

	public override bool SavingEffect { get; } = true;

	public override void ExpireEffect()
	{
		Gameworld.EffectScheduler.AddSchedule(new EffectSchedule(this, TimeSpan.FromSeconds(1800)));
		GiveInfection();
	}

	private void LoadFromXml(XElement root)
	{
		Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Bodypart").Value));
		Difficulty = (Difficulty)int.Parse(root.Element("Difficulty").Value);
		OriginalDifficulty =
			(Difficulty)int.Parse(root.Element("OriginalDifficulty")?.Value ?? root.Element("Difficulty").Value);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("SurgeryFinalisationRequired",
			(effect, owner) => new SurgeryFinalisationRequired(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XElement("Bodypart", Bodypart.Id),
				new XElement("Difficulty", (int)Difficulty),
				new XElement("OriginalDifficulty", (int)OriginalDifficulty));
	}

	private void GiveInfection()
	{
		// Give them a horrid infection for not finishing up their surgery
		// First check any organs on the bodypart
		var cOwner = Owner as ICharacter;
		var organs =
			Bodypart.Organs.Where(
				x => cOwner.Body.Organs.Contains(x) & cOwner.Body.PartInfections.All(y => y.Bodypart != x)).ToList();
		var terrain = cOwner.Location.Terrain(cOwner);
		if (organs.Any())
		{
			var organ = organs.GetRandomElement();
			cOwner.Body.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection,
				terrain.InfectionVirulence.StageUp(2), 0.0001, cOwner.Body, null, organ, terrain.InfectionMultiplier));
			return;
		}

		// If no organs, the bodypart itself is likely to infected
		if (cOwner.Body.PartInfections.All(x => x.Bodypart != Bodypart))
		{
			cOwner.Body.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection,
				terrain.InfectionVirulence.StageUp(2), 0.0001, cOwner.Body, null, Bodypart,
				terrain.InfectionMultiplier));
			return;
		}

		// If no organs or uninfected organs on the bodypart, check all organs
		organs = cOwner.Body.Organs.Where(x => cOwner.Body.PartInfections.All(y => y.Bodypart != x)).ToList();
		if (organs.Any())
		{
			var organ = organs.GetWeightedRandom(x => 1.0 / x.RelativeInfectability);
			cOwner.Body.AddInfection(Infection.LoadNewInfection(terrain.PrimaryInfection,
				terrain.InfectionVirulence.StageUp(2), 0.0001, cOwner.Body, null, organ, terrain.InfectionMultiplier));
		}

		// Poor bastard is already completely riddled with infections. I suppose they get a free pass
	}
}