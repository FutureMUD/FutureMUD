using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Save;
using MudSharp.RPG.Checks;
using MudSharp.Framework;

namespace MudSharp.Health.Infections;

public abstract class Infection : LateInitialisingItem, IInfection
{
	public sealed override string FrameworkItemType => "Infection";
	protected double _intensity;

	protected Infection(Difficulty virulenceDifficulty, double intensity, IBody owner, IWound wound,
		IBodypart bodypart, double virulence)
	{
		Gameworld = owner.Gameworld;
		VirulenceDifficulty = virulenceDifficulty;
		_intensity = intensity;
		Owner = owner;
		Wound = wound;
		Bodypart = bodypart;
		Virulence = virulence;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	protected Infection(MudSharp.Models.Infection infection, IBody owner, IWound wound, IBodypart bodypart)
	{
		Gameworld = owner.Gameworld;
		_id = infection.Id;
		IdInitialised = true;
		Owner = owner;
		Wound = wound;
		Bodypart = bodypart;
		VirulenceDifficulty = (Difficulty)infection.Virulence;
		Virulence = 1.0; // TODO - load
		_intensity = infection.Intensity;
		_immunity = infection.Immunity;
	}

	public IBody Owner { get; set; }

	public void Delete()
	{
		Changed = false;
		using (new FMDB())
		{
			if (_id != 0)
			{
				Gameworld.SaveManager.Abort(this);
				var infection = FMDB.Context.Infections.Find(Id);
				if (infection != null)
				{
					FMDB.Context.Infections.Remove(infection);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Infections.Find(Id);
		if (dbitem != null)
		{
			dbitem.Virulence = (int)VirulenceDifficulty;
			dbitem.Intensity = Intensity;
			dbitem.Immunity = Immunity;
		}

		Changed = false;
	}

	public abstract InfectionType InfectionType { get; }
	public abstract InfectionStage InfectionStage { get; set; }
	public IWound Wound { get; set; }
	public IBodypart Bodypart { get; set; }

	public double Intensity
	{
		get => _intensity;
		set
		{
			_intensity = value;
			Changed = true;
		}
	}

	private double _immunity;

	public double Immunity
	{
		get => _immunity;
		set
		{
			_immunity = value;
			Changed = true;
		}
	}

	public double Virulence { get; }

	public Difficulty VirulenceDifficulty { get; set; }

	public abstract double Pain { get; }

	public virtual void InfectionTick()
	{
		if (Immunity >= 1.0)
		{
			var old = Intensity;
			Intensity -= Gameworld.GetStaticDouble("InfectionIntensityLossPerTickOnceImmune");
			UpdateInfectionStage(old);
			return;
		}

		var check = Gameworld.GetCheck(CheckType.InfectionHeartbeat);
		var externalBonus = 0.0;
		// Antibiotic effects
		externalBonus +=
			Owner.CombinedEffectsOfType<IInfectionResistanceEffect>()
			     .Where(x => x.Applies() && x.AppliesToType(InfectionType))
			     .Select(x => x.InfectionResistanceBonus)
			     .DefaultIfEmpty(0)
			     .Sum() +
			Owner.CombinedEffectsOfType<IImmuneBonusEffect>()
			     .Where(x => x.Applies())
			     .Select(x => x.ImmuneBonus)
			     .DefaultIfEmpty(0)
			     .Sum();

		// Having a damaged spleen or liver hurts immunity
		var spleenFunction = Owner.OrganFunction<SpleenProto>();
		if (spleenFunction < 1.0)
		{
			externalBonus += (1.0 - spleenFunction) * Gameworld.GetStaticDouble("InfectionBonusPerSpleenFunction");
		}

		var liverFunction = Owner.OrganFunction<LiverProto>();
		if (liverFunction < 1.0)
		{
			externalBonus += (1.0 - spleenFunction) * Gameworld.GetStaticDouble("InfectionBonusPerLiverFunction");
		}

		// Resting and Sleeping helps the immune system
		var checkDifficulty = VirulenceDifficulty;
		if (Owner.Actor.PositionState.CompareTo(PositionLounging.Instance) != PositionHeightComparison.Higher)
		{
			checkDifficulty = checkDifficulty.StageDown(1);
		}

		if (Owner.Actor.State.HasFlag(CharacterState.Sleeping))
		{
			checkDifficulty = checkDifficulty.StageDown(1);
		}

		var result = check.Check(Owner.Actor, checkDifficulty, default(ICharacter), null, externalBonus);

		Immunity += result.SuccessDegrees() * Gameworld.GetStaticDouble("ImmunityGainPerSuccess");
		var oldIntensity = Intensity;
		Intensity += Gameworld.GetStaticDouble("InfectionIntensityGainPerTick") * Virulence;
		UpdateInfectionStage(oldIntensity);

		if (InfectionIsDamaging())
		{
			Owner.SufferDamage(GetInfectionDamage());
		}

		if (InfectionCanSpread())
		{
			var spreadCheck = Gameworld.GetCheck(CheckType.InfectionSpread);
			var spreadResult = spreadCheck.Check(Owner.Actor, checkDifficulty, default(ICharacter), null,
				externalBonus + Owner.ImmuneFatigueBonus);
			if (spreadResult.IsPass())
			{
				return;
			}

			Spread(spreadResult);
		}
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.Infection();
		FMDB.Context.Infections.Add(dbitem);
		dbitem.OwnerId = Owner.Id;
		dbitem.BodypartId = Bodypart?.Id;
		dbitem.Wound = FMDB.Context.Wounds.Find(Wound?.Id ?? 0);
		dbitem.Virulence = (int)VirulenceDifficulty;
		dbitem.Intensity = Intensity;
		dbitem.InfectionType = (int)InfectionType;
		dbitem.Immunity = Immunity;
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		var item = (MudSharp.Models.Infection)dbitem;
		_id = item.Id;
	}

	public static IInfection LoadNewInfection(InfectionType type, Difficulty virulenceDifficulty, double intensity,
		IBody owner, IWound wound, IBodypart bodypart, double virulence)
	{
		switch (type)
		{
			case InfectionType.Simple:
				return new SimpleInfection(virulenceDifficulty, intensity, owner, wound, bodypart, virulence);
			case InfectionType.Gangrene:
				return new Gangrene(virulenceDifficulty, intensity, owner, wound, bodypart, virulence);
		}

		throw new ApplicationException("Unknown Infection Type");
	}

	public static IInfection LoadInfection(MudSharp.Models.Infection infection, IBody body)
	{
		var wound = body.Wounds.FirstOrDefault(x => x.Id == (infection.WoundId ?? 0));
		var bodypart = body.Bodyparts.FirstOrDefault(x => x.Id == (infection.BodypartId ?? 0)) ??
		               body.Organs.FirstOrDefault(x => x.Id == (infection.BodypartId ?? 0));
		switch ((InfectionType)infection.InfectionType)
		{
			case InfectionType.Simple:
				return new SimpleInfection(infection, body, wound, bodypart);
			case InfectionType.Gangrene:
				return new Gangrene(infection, body, wound, bodypart);
		}

		throw new ApplicationException("Unknown Infection Type");
	}

	//Return true if the infection is due to be cleared out due to being healed enough to go away.
	public bool InfectionHealed()
	{
		return Immunity >= 1.0 && InfectionStage == InfectionStage.StageZero;
	}

	//Call anytime the Intensity is changed
	//Returns true if the stage changed
	protected abstract bool UpdateInfectionStage(double oldIntensity);

	//Call anytime the Stage is changed
	protected abstract void SendIntensityMessage(bool improving);

	//Call to get a wound tag to append to infected wounds
	public abstract string WoundTag(WoundExaminationType examType, Outcome outcome);

	public abstract bool InfectionIsDamaging();
	public abstract bool InfectionCanSpread();
	public abstract IDamage GetInfectionDamage();

	public virtual void Spread(Outcome outcome)
	{
		if (Wound == null)
		{
			if (Bodypart == null)
			{
				SpreadFromBody(outcome);
				return;
			}

			if (Bodypart is IOrganProto)
			{
				SpreadFromOrgan(outcome);
				return;
			}

			SpreadFromBodypart(outcome);
			return;
		}

		SpreadFromWound(outcome);
	}

	protected virtual void SpreadFromWound(Outcome outcome)
	{
		switch (outcome)
		{
			case Outcome.MajorFail:
				// Could spread to the whole body
				if (!Owner.PartInfections.Any(x => x.Bodypart == null && x is SimpleInfection))
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, null, Virulence));
					return;
				}

				goto case Outcome.Fail;
			case Outcome.Fail:
				// Check parent bodypart first
				if (!Owner.PartInfections.Any(x => x.Bodypart == Wound.Bodypart && x is SimpleInfection))
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, Wound.Bodypart,
						Virulence));
					return;
				}

				goto case Outcome.MinorFail;
			case Outcome.MinorFail:
				// Then check wounds on the same bodypart
				var wound =
					Owner.Wounds.Where(x => x.Bodypart == Wound.Bodypart && x.EligableForInfection())
					     .GetRandomElement();
				if (wound != null)
				{
					wound.Infection = new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, wound, Wound.Bodypart,
						Virulence);
					return;
				}

				// Check wounds on the same limb next
				wound =
					Owner.GetWoundsForLimb(Owner.GetLimbFor(Wound.Bodypart))
					     .Where(x => x.EligableForInfection())
					     .GetRandomElement();
				if (wound != null)
				{
					wound.Infection = new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, wound, wound.Bodypart,
						Virulence);
					return;
				}

				// Any wound is up for grabs finally
				wound = Owner.Wounds.Where(x => x.EligableForInfection()).GetRandomElement();
				if (wound != null)
				{
					wound.Infection = new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, wound, wound.Bodypart,
						Virulence);
					return;
				}

				return;
		}
	}

	protected virtual void SpreadFromBodypart(Outcome outcome)
	{
		switch (outcome)
		{
			case Outcome.MajorFail:
				// Could spread to the whole body
				if (!Owner.PartInfections.Any(x => x.Bodypart == null && x is SimpleInfection))
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, null, Virulence));
					return;
				}

				// Could spread to any organ
				var organ =
					Owner.Bodyparts.SelectMany(x => x.Organs)
					     .Distinct()
					     .Where(x => Owner.PartInfections.All(y => y.Bodypart != x))
					     .GetWeightedRandom(x => x.RelativeInfectability * (Bodypart.Organs.Contains(x) ? 5 : 1));
				if (organ == null)
				{
					goto case Outcome.MinorFail;
				}

				Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, organ, Virulence));
				return;
			case Outcome.Fail:
				// Spreads to organs on the same bodypart
				organ = Bodypart.OrganInfo
				                .Select(x => x.Key)
				                .Where(x => !Owner.PartInfections.Any(y => y.Bodypart == x && y is SimpleInfection))
				                .GetWeightedRandom(x => x.RelativeInfectability);
				if (organ == null)
				{
					goto case Outcome.MinorFail;
				}

				Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, organ, Virulence));
				return;
			case Outcome.MinorFail:
				// Check adjacent bodyparts first
				var bodypart = Owner.Bodyparts
				                    .Where(x => x.UpstreamConnection?.CountsAs(Bodypart) == true ||
				                                Bodypart.UpstreamConnection?.CountsAs(x) == true)
				                    .Where(x => Owner.PartInfections.All(y => y.Bodypart != x))
				                    .GetRandomElement();
				if (bodypart != null)
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, bodypart,
						Virulence));
					return;
				}

				// Then check wounds on the same bodypart
				var wound =
					Owner.Wounds.Where(x => x.Bodypart == Bodypart && x.EligableForInfection()).GetRandomElement();
				if (wound != null)
				{
					wound.Infection =
						new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, wound, Bodypart, Virulence);
					return;
				}

				// Check wounds on the same limb next
				wound =
					Owner.GetWoundsForLimb(Owner.GetLimbFor(Bodypart))
					     .Where(x => x.EligableForInfection())
					     .GetRandomElement();
				if (wound != null)
				{
					wound.Infection = new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, wound, wound.Bodypart,
						Virulence);
					return;
				}

				// Any wound is up for grabs finally
				wound = Owner.Wounds.Where(x => x.EligableForInfection()).GetRandomElement();
				if (wound != null)
				{
					wound.Infection = new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, wound, wound.Bodypart,
						Virulence);
					return;
				}

				break;
		}
	}

	protected virtual void SpreadFromOrgan(Outcome outcome)
	{
		switch (outcome)
		{
			case Outcome.MajorFail:
				// Could spread to the whole body
				if (!Owner.PartInfections.Any(x => x.Bodypart == null && x is SimpleInfection))
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, null, Virulence));
					return;
				}

				// Could spread to any organ
				var organ =
					Owner.Bodyparts.SelectMany(x => x.Organs)
					     .Distinct()
					     .Where(x => Owner.PartInfections.All(y => y.Bodypart != x))
					     .GetWeightedRandom(x => x.RelativeInfectability);
				if (organ == null)
				{
					goto case Outcome.MinorFail;
				}

				Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, organ, Virulence));
				return;
			case Outcome.Fail:
				// Spreads to organs on the same bodypart
				organ = Owner.Bodyparts.Where(x => x.OrganInfo.Any(y => y.Key.CountsAs(Bodypart)))
				             .SelectMany(x => x.OrganInfo)
				             .Select(x => x.Key)
				             .Distinct()
				             .GetWeightedRandom(x => x.RelativeInfectability);
				if (organ == null)
				{
					goto case Outcome.MinorFail;
				}

				Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, organ, Virulence));
				return;
			case Outcome.MinorFail:
				// Spread to parent bodypart first
				var bodypart = Owner.Bodyparts.Where(x => x.OrganInfo.Any(y => y.Key.CountsAs(Bodypart)))
				                    .Where(x => Owner.PartInfections.All(y => y.Bodypart != x))
				                    .GetRandomElement();
				if (bodypart != null)
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, bodypart,
						Virulence));
					return;
				}

				// Spread to a bodypart on the same limb as a parent
				bodypart = Owner.Bodyparts.Where(x => x.OrganInfo.Any(y => y.Key.CountsAs(Bodypart)))
				                .Select(x => Owner.GetLimbFor(x))
				                .Distinct()
				                .SelectMany(x => Owner.BodypartsForLimb(x))
				                .Distinct()
				                .Where(x => Owner.PartInfections.All(y => y.Bodypart != x))
				                .GetRandomElement();
				if (bodypart != null)
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, bodypart,
						Virulence));
					return;
				}

				// They're pretty heavily infected as it is
				return;
		}
	}

	protected virtual void SpreadFromBody(Outcome outcome)
	{
		switch (outcome)
		{
			case Outcome.MajorFail:
			case Outcome.Fail:
				// Could spread to any organ
				var organ =
					Owner.Bodyparts.SelectMany(x => x.Organs)
					     .Distinct()
					     .Where(x => Owner.PartInfections.All(y => y.Bodypart != x))
					     .GetWeightedRandom(x => x.RelativeInfectability);
				if (organ == null)
				{
					goto case Outcome.MinorFail;
				}

				Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, organ, Virulence));
				return;
			case Outcome.MinorFail:
				// Could spread to any bodypart
				var bodypart = Owner.Bodyparts.Where(x => Owner.PartInfections.All(y => y.Bodypart != x))
				                    .GetRandomElement();
				if (bodypart != null)
				{
					Owner.AddInfection(new SimpleInfection(VirulenceDifficulty, 0.0001, Owner, null, bodypart,
						Virulence));
				}

				return;
		}
	}
}