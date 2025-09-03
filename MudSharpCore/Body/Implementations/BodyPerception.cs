using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using JetBrains.Annotations;
using MudSharp.Body.PartProtos;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	#region IPerceiver

	public override ICellOverlayPackage CurrentOverlayPackage
	{
		get => Actor.CurrentOverlayPackage;
		set => Actor.CurrentOverlayPackage = value;
	}

	public new RoomLayer RoomLayer
	{
		get => Actor.RoomLayer;
		set => Actor.RoomLayer = value;
	}

	public override bool CanHear(IPerceivable thing)
	{
		// TODO
		if (Actor.IsAdministrator())
		{
			return true;
		}

				if (AffectedBy<IDeafnessEffect>())
				{
						return false;
				}

				if (OrganFunction<EarProto>() <= 0.0)
				{
						return false;
				}

		return !Actor.State.HasFlag(CharacterState.Unconscious);
	}

	public override bool CanSense(IPerceivable thing, bool ignoreFuzzy = false)
	{
		throw new NotImplementedException();
	}

	private bool IsInInventory(IGameItem item)
	{
		return item != null && (item.InInventoryOf == this || IsInInventory(item.ContainedIn));
	}

	public override double VisionPercentage
	{
		get
		{
			var numberOfEyes = 0.0;
			var eyes = Bodyparts.OfType<EyeProto>().ToList();
			foreach (var eye in eyes)
			{
				if (WornItemsFor(eye).Any(x => x.IsItemType<IBlindfold>()))
				{
					continue;
				}

				if (AffectedBy<IBodypartIneffectiveEffect>(eye) &&
					!Prosthetics.Any(y => eye.DownstreamOfPart(y.TargetBodypart) && y.Functional))
				{
					continue;
				}

				numberOfEyes += 1.0;
			}

			if (Actor.Merits.OfType<IMyopiaMerit>().Any(x =>
					x.Applies(Actor) && (!x.CorrectedByGlasses ||
										 eyes.Any(y => !WornItemsFor(y).Any(z => z.IsItemType<ICorrectMyopia>())))))
			{
				numberOfEyes /= 2.0;
			}

			// TODO - other things that impact on eyes without totally blinding them

			return numberOfEyes >= 2.0 ? 1.0 : numberOfEyes / 2.0;
		}
	}

	public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (flags.HasFlag(PerceiveIgnoreFlags.IgnoreCanSee))
		{
			return true;
		}

		if (thing == null)
		{
			return false;
		}

		if (Actor.IsAdministrator())
		{
			return true;
		}

		if (thing is IBody body)
		{
			thing = body.Actor;
		}

		if (thing == Actor && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf))
		{
			return true;
		}

		var visionExemptThing = thing is IExit || (thing is IGameItem gi && IsInInventory(gi));
		var perceiverThing = thing as IPerceiver;

		// Require at least 1 eye to see unless things are in your inventory or are cell exits
				var eyes = Bodyparts.OfType<EyeProto>().ToList();
				if (!visionExemptThing)
				{
						if (AffectedBy<IBlindnessEffect>())
						{
								return false;
						}

						if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreDark) && (!eyes.Any() ||
																			   eyes.All(
	   AffectedBy<IBodypartIneffectiveEffect>) ||
																			   eyes.All(x => Prosthetics.Any(
	   y => x.DownstreamOfPart(
					y.TargetBodypart) &&
			!y.Functional)) ||
																			   eyes.All(x => WornItemsFor(x)
	   .Any(
			   y => y
					   .IsItemType<IBlindfold>())))
						   )
						{
								return false;
						}
				}

		if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreConsciousness) && Actor.State.IsUnconscious())
		{
			return false;
		}

		if (thing is ICelestialObject celestial)
		{
			return (Actor.GetPerception(Actor.NaturalPerceptionTypes) & PerceptionTypes.AllVisual &
					celestial.PerceivableTypes) != PerceptionTypes.None &&
				   (Location?.OutdoorsType(Actor) == CellOutdoorsType.IndoorsWithWindows ||
					Location?.OutdoorsType(Actor) == CellOutdoorsType.Outdoors);
		}

		if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreObscured) && perceiverThing != null)
		{
			if (
				!visionExemptThing &&
				!ColocatedWith(perceiverThing) &&
				Actor.Merits.OfType<IMyopiaMerit>().Any(x =>
					x.Applies(Actor) && (!x.CorrectedByGlasses ||
										 eyes.Any(y => !WornItemsFor(y).Any(z => z.IsItemType<ICorrectMyopia>()))))
			)
			{
				return false;
			}
		}

		if (flags.HasFlag(PerceiveIgnoreFlags.IgnoreDark) && thing.Location != null && Actor.IlluminationSightDifficulty() == Difficulty.Impossible)
		{
			if (!visionExemptThing && Actor.Party?.Members.Any(x => x.IsSelf(thing)) != true)
			{
				return false;
			}
		}

		return thing.HiddenFromPerception(Actor,
			Actor.GetPerception(Actor.NaturalPerceptionTypes) & PerceptionTypes.AllVisual, flags);
	}

	public bool CanSee(ICell cell, ICellExit exit, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (Actor.IsAdministrator())
		{
			return true;
		}

		return cell.IsExitVisible(Actor, exit,
			Actor.GetPerception(Actor.NaturalPerceptionTypes) & PerceptionTypes.AllVisual, flags);
	}

	public override bool CanSmell(IPerceivable thing)
	{
		throw new NotImplementedException();
	}

	public override Gendering ApparentGender(IPerceiver voyeur)
	{
		if (voyeur != null && !voyeur.CanSee(this))
		{
			return Indeterminate.Instance;
		}

		if (Actor.Corpse?.Decay >= DecayState.HeavilyDecayed)
		{
			return Indeterminate.Instance;
		}

		return Gender;
	}

	public override IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return GetKeywordsFromSDesc(HowSeen(voyeur, colour: false, flags: PerceiveIgnoreFlags.IgnoreNamesSetting));
	}

	public string ProcessDescriptionAdditions(string description, IPerceiver voyeur, bool colour, PerceiveIgnoreFlags flags)
	{
		if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreNamesSetting) && voyeur is ICharacter vch && vch.Account.CharacterNameOverlaySetting != Accounts.CharacterNameOverlaySetting.None)
		{
			var dub = vch.Dubs.FirstOrDefault(x => x.TargetId == Actor.Id && x.TargetType == Actor.FrameworkItemType);
			if (dub is not null)
			{
				description = dub.HowSeen(vch);
			}
			else if (vch.IsAdministrator())
			{
				string name;
				switch (vch.Account.CharacterNameOverlaySetting)
				{
					case Accounts.CharacterNameOverlaySetting.AppendWithBrackets:
						name = Actor.PersonalName.GetName(Character.Name.NameStyle.GivenOnly);
						if (!string.IsNullOrWhiteSpace(name))
						{
							description = $"{description} {name.Parentheses().Colour(Telnet.BoldWhite)}";
						}

						break;
					case Accounts.CharacterNameOverlaySetting.Replace:
						name = Actor.PersonalName.GetName(Character.Name.NameStyle.FullName);
						if (!string.IsNullOrWhiteSpace(name))
						{
							description = name;
						}

						break;
				}
			}
		}

		return
			CombinedEffectsOfType<ISDescAdditionEffect>()
				.Where(x => x.DescriptionAdditionApplies(voyeur))
				.DistinctBy(x => x.AddendumText)
				.Select(effect => effect.GetAddendumText(colour))
				.Aggregate(description, (current, text) => $"{current} {text}");
	}

	private string DressLongDescription(IPerceiver voyeur, string description)
	{
		var sb = new StringBuilder(description);
		if (Actor.EffectsOfType<IAdminInvisEffect>().Any())
		{
			sb.Append(" (wizinvis)".ColourBold(Telnet.Blue));
		}

		if (Actor.EffectsOfType<INewPlayerEffect>().Any())
		{
			sb.Append(" (new player)".Colour(Telnet.Green));
		}

		if (Actor.EffectsOfType<IDoorguardModeEffect>().Any() &&
			((voyeur as ICharacter)?.IsAdministrator() ?? false))
		{
			sb.Append(" (door guard)".Colour(Telnet.Cyan));
		}

		if (Actor.EffectsOfType<PatrolMemberEffect>().Any() &&
			((voyeur as ICharacter)?.IsAdministrator() ?? false))
		{
			var pme = Actor.EffectsOfType<PatrolMemberEffect>().First();
			if (pme.Patrol.PatrolStrategy.Name == "Judge")
			{
				sb.Append(" (judge)".Colour(Telnet.BoldPink));
			}
			else if (pme.Patrol.PatrolStrategy.Name == "Prosecutor")
			{
				sb.Append(" (prosecutor)".Colour(Telnet.BoldPink));
			}
			else if (pme.Patrol.PatrolStrategy.Name == "Sheriff")
			{
				sb.Append(" (sheriff)".Colour(Telnet.BoldPink));
			}
			else
			{
				sb.Append(" (enforcer)".Colour(Telnet.BoldPink));
			}
		}

		return sb.ToString();
	}

	public string GetPositionDescription(IPerceiver voyeur, bool proper, bool colour, PerceiveIgnoreFlags flags)
	{
		var parts = new List<string>();
		var stateDesc = "";
		if (Actor.State.HasFlag(CharacterState.Unconscious))
		{
			parts.Add("unconscious");
		}
		else if (Actor.State.HasFlag(CharacterState.Sleeping))
		{
			parts.Add("asleep");
		}
		else if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreHiding) && Actor.CombinedEffectsOfType<IHideEffect>().Any())
		{
			parts.Add("hiding");
		}

		if (Actor.Cover != null)
		{
			parts.Add(Actor.Cover.Cover.Describe(Actor, Actor.Cover.CoverItem?.Parent, voyeur));
		}

		if (Actor.Combat is not null)
		{
			parts.Add(Actor.Combat.LDescAddendumFor(Actor, voyeur));
		}

		if (Actor.RidingMount is not null)
		{
			parts.Add($"riding {Actor.RidingMount.HowSeen(voyeur)}");
		}
		else
		{
			parts.Add(DescribePosition(voyeur, true));
		}

		switch (Actor.Riders.Count())
		{
			case 0:
				break;
			case 1:
				parts.Add($"being ridden by {Actor.Riders.First().HowSeen(voyeur)}");
				break;
			default:
				parts.Add("being ridden by multiple riders");
				break;
		}

		var ldescEffect = Actor.CombinedEffectsOfType<ILDescSuffixEffect>()
		                       .FirstOrDefault(x => x.Applies() && x.SuffixApplies());
		if (ldescEffect != null)
		{
			parts.Add(ldescEffect.SuffixFor(voyeur));
		}

		var usePlural = voyeur.IsSelf(this) && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf) || !IsSingleEntity;
		return $"{HowSeen(voyeur, proper, DescriptionType.Short, colour, flags)} {(usePlural ? "are" : "is")} {parts.ListToCommaSeparatedValues(", ")} here";
	}

	public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (voyeur == null)
		{
			voyeur = this;
			flags = flags | PerceiveIgnoreFlags.IgnoreSelf;
		}

		if (CombinedEffectsOfType<IOverrideDescEffect>().Any(x => x.OverrideApplies(voyeur, type)) &&
			(voyeur.CanSee(this) || flags.HasFlag(PerceiveIgnoreFlags.IgnoreCanSee)))
		{
			return CombinedEffectsOfType<IOverrideDescEffect>()
				   .First(x => x.OverrideApplies(voyeur, type))
				   .Description(type, colour);
		}

		// TODO - add in obscuring effects like cloaks
		switch (type)
		{
			case DescriptionType.Short:
				return ShortDescription(voyeur, proper, type, colour, flags);
			case DescriptionType.Possessive:
				if ((voyeur == this || (voyeur is ICharacter && ((ICharacter)voyeur).Body == this)) &&
					!flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf))
				{
					return colour ? (proper ? "Your" : "your").ColourCharacter() : proper ? "Your" : "your";
				}

				return
					(this as IHaveCharacteristics).ParseCharacteristics(
						HowSeen(voyeur, proper, DescriptionType.Short, colour, flags), voyeur) + "'s";
			case DescriptionType.Long:
				return DressLongDescription(voyeur,
					GetPositionDescription(voyeur, proper, colour, flags).Fullstop());
			case DescriptionType.Full:
				return FullDescription(voyeur).SubstituteANSIColour().StripANSIColour(colour == false);
			default:
				throw new ArgumentException("Not a valid description type");
		}
	}

	private string ShortDescription(IPerceiver voyeur, bool proper, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if ((voyeur == this || (voyeur is ICharacter && ((ICharacter)voyeur).Body == this)) &&
			!flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf))
		{
			return proper ? "You" : "you";
		}

		if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreCorpse) && Actor.Corpse != null)
		{
			return Actor.Corpse.Parent.HowSeen(voyeur, proper, type, colour);
		}

		if (!voyeur.CanSee(this, flags))
		{
			if (colour)
			{
				return (proper ? "Someone" : "someone").ColourCharacter();
			}

			return proper ? "Someone" : "someone";
		}

		var identityObscurer =
			flags.HasFlag(PerceiveIgnoreFlags.IgnoreDisguises)
				? null
				: WornItems.SelectNotNull(x => x.GetItemType<IObscureIdentity>()).Reverse()
						   .FirstOrDefault(x => x.CurrentlyApplies);

		var output = ProcessDescriptionAdditions(
			(this as IHaveCharacteristics)
			.ParseCharacteristics(
				identityObscurer?.OverriddenShortDescription ?? _shortDescriptionPattern?.Pattern ?? _shortDescription,
				voyeur)
			.SubstituteWrittenLanguage(voyeur, Gameworld)
			.NormaliseSpacing()
			.Trim()
			.FluentProper(proper), voyeur, colour, flags);

		return colour ? output.ColourCharacter() : output;
	}

	private string FullDescription(IPerceiver voyeur)
	{
		if (!voyeur.CanSee(Actor))
		{
			return "You cannot make out their features.";
		}

		var identityObscurer = WornItems.SelectNotNull(x => x.GetItemType<IObscureIdentity>()).Reverse()
										.FirstOrDefault(x => x.CurrentlyApplies);

		var text = identityObscurer?.OverriddenFullDescription ?? _fullDescriptionPattern?.Pattern ?? _fullDescription;
		text = text.SubstituteWrittenLanguage(voyeur, Gameworld);
		text = (this as IHaveCharacteristics).ParseCharacteristics(text, voyeur);

		text = Effects.Concat(Actor.Effects).OfType<IDescriptionAdditionEffect>().Where(x => x.DescriptionAdditionApplies(voyeur))
					  .Aggregate(text,
						  (current, component) =>
							  $"{current}\n\t{component.GetAdditionalText(voyeur, true)}");

		return text.NormaliseSpacing().ProperSentences().Wrap(voyeur.InnerLineFormatLength);
	}

	public string LookText(bool fromMovement = false)
	{
		if (Actor.State.HasFlag(CharacterState.Sleeping))
		{
			if (!fromMovement)
			{
				return "You can see nothing but the back of your eyelids, as you are asleep.";
			}

			return string.Empty;
		}

		if (Actor.State.HasFlag(CharacterState.Unconscious))
		{
			if (!fromMovement)
			{
				return "You can see nothing but the back of your eyelids, as you are unconscious.";
			}

			return string.Empty;
		}

		if (!CanSee(Location))
		{
			return "The world is dark and you cannot see anything.";
		}

		var sb = new StringBuilder();
		sb.AppendLine(Location.HowSeen(Actor,
			type: fromMovement && Actor.BriefRoomDescs ? DescriptionType.Long : DescriptionType.Full));
		foreach (
			var move in
			Location.Characters.Where(x => x != Actor && x.Movement != null && x.RoomLayer == RoomLayer)
					.Select(x => x.Movement)
					.Distinct()
					.Where(move => move.SeenBy(Actor))
					.ToList())
		{
			sb.AppendLine(move.Describe(Actor).Wrap(InnerLineFormatLength).Colour(Telnet.Yellow));
		}

		var graffitis = Location.EffectsOfType<IGraffitiEffect>(x => x.Layer == RoomLayer).ToList();
		if (graffitis.Any())
		{
			sb.AppendLine($"There is graffiti in this location. Use LOOK GRAFFITI to view it.".Colour(Telnet.BoldCyan));
		}

		var items = Location.LayerGameItems(RoomLayer).Where(x => CanSee(x)).ToList();
		if (items.GroupBy(x => x.ItemGroup?.Forms.Any() == true ? x.ItemGroup : null).Sum(x => x.Key != null ? 1 : x.Count()) > 25 &&
			GameItemProto.TooManyItemsGroup != null)
		{
			sb.AppendLine(GameItemProto.TooManyItemsGroup.Describe(Actor, items, Actor.Location).Fullstop()
									   .Wrap(InnerLineFormatLength).Colour(Telnet.Cyan));
		}
		else
		{
			// Do puddle groups first
			var puddles = items.SelectNotNull(x => x.GetItemType<PuddleGameItemComponent>()).ToList();
			var bloodPuddles = puddles.Where(x => x.LiquidMixture.Instances.All(y => y is BloodLiquidInstance)).ToList();
			items = items.Except(puddles.Select(x => x.Parent)).ToList();
			if (bloodPuddles.Any())
			{
				sb.AppendLine(PuddleGameItemComponentProto.BloodGroup.Describe(Actor, bloodPuddles.Select(x => x.Parent), Location).Wrap(InnerLineFormatLength));
				puddles = puddles.Except(bloodPuddles).ToList();
			}

			if (puddles.Any())
			{
				sb.AppendLine(PuddleGameItemComponentProto.PuddleGroup.Describe(Actor, puddles.Select(x => x.Parent), Location).Wrap(InnerLineFormatLength));
			}

			// Next do dried residues
			var commodityTag = Gameworld.Tags.Get(Gameworld.GetStaticLong("PuddleResidueTagId"));
			if (commodityTag is not null)
			{
				var residues = items
							   .SelectNotNull(x => x.GetItemType<ICommodity>())
							   .Where(x => x.Tag == commodityTag)
							   .ToList();
				if (residues.Any())
				{
					sb.AppendLine(PuddleGameItemComponentProto.ResidueGroup.Describe(Actor, residues.Select(x => x.Parent), Location).Wrap(InnerLineFormatLength));
					items = items.Except(residues.Select(x => x.Parent)).ToList();
				}
			}
			
			// Then display everything else
			foreach (var group in items.GroupBy(x => x.ItemGroup?.Forms.Any() == true ? x.ItemGroup : null).OrderBy(x => x.Key == null))
			{
				if (group.Key != null)
				{
					sb.AppendLine(
						group.Key.Describe(Actor, group.AsEnumerable(), Location).Wrap(InnerLineFormatLength)
							 .Colour(Telnet.Cyan));
				}
				else
				{
					foreach (var item in group)
					{
						sb.AppendLine(item.HowSeen(Actor, true, DescriptionType.Long).Wrap(InnerLineFormatLength));
					}
				}
			}
		}

		foreach (var ch in Location.Characters.Where(x =>
					 x != Actor && x.Movement == null && x.RoomLayer == RoomLayer && CanSee(x)))
		{
			sb.AppendLine(ch.HowSeen(Actor, true, DescriptionType.Long).Wrap(InnerLineFormatLength));
		}

		return sb.ToString().Wrap(Account.LineFormatLength);
	}

	public string LookText(IPerceivable thing, bool fromLookCommand = false)
	{
		var sb = new StringBuilder();
		sb.AppendLine(thing.HowSeen(Actor, true, DescriptionType.Full));

		if (thing is IGameItem gi && fromLookCommand)
		{

			if (Actor.IsAdministrator() && gi.GetItemType<IDestroyable>() is { } id)
			{
				var damage = gi.Wounds.Sum(x => x.CurrentDamage);
				var hp = id.MaximumDamage;
				var currentHP = hp - damage;
				sb.AppendLine($"HP: {currentHP.ToStringN2Colour(Actor)}/{hp.ToStringN2Colour(Actor)}");
			}

			if (gi.EffectsOfType<IGraffitiEffect>().Any())
			{
				sb.AppendLine($"This item has graffiti. Use LOOK <item> GRAFFITI to view it.".Colour(Telnet.BoldCyan));
			}

			var dub = Dubs.FirstOrDefault(x => x.TargetId == thing.Id && x.TargetType == thing.FrameworkItemType);
			if (dub != null)
			{
				sb.AppendLine(
					$"You have dubbed this item {dub.Keywords.Select(x => x.Colour(Telnet.BoldWhite)).ListToString()}."
						.Wrap(InnerLineFormatLength));
			}

			if (gi.GetItemType<IWieldable>() is IWieldable iw)
			{
				var hands = _wieldLocs.Min(x => x.Hands(gi));
				sb.AppendLine(
					$"You would require {hands} {(hands == 1 ? WielderDescriptionSingular : WielderDescriptionPlural)} to wield this."
						.Colour(Telnet.Yellow));
			}

			foreach (var attached in gi.AttachedItems)
			{
				sb.AppendLine($"It has {attached.HowSeen(Actor)} attached to it.");
			}
		}
		if (thing is IMortalPerceiver mortal)
		{
			// Wounds
			var wounds = mortal.VisibleWounds(Actor,
				Actor.Combat != null ? WoundExaminationType.Glance : WoundExaminationType.Look).ToList();
			var woundDescs = wounds
							 .Select(x => (Description: x.Describe(
								 Actor.Combat != null ? WoundExaminationType.Glance : WoundExaminationType.Look,
								 Outcome.MajorPass), x.Bodypart))
							 .Where(x => !string.IsNullOrWhiteSpace(x.Description))
							 .ToList();
			var gender = mortal.ApparentGender(Actor);
			var genderWord = gender.Possessive();
			var woundsName = thing is IGameItem ? "signs of damage" : "wounds";
			if (woundDescs.Count > 5)
			{
				var maxSeverity = wounds.Max(x => x.Severity);
				var quantiyDescription = "a great many";
				if (woundDescs.Count < 8)
				{
					quantiyDescription = "several";
				}
				else if (woundDescs.Count < 15)
				{
					quantiyDescription = "quite a few";
				}
				else if (woundDescs.Count < 30)
				{
					quantiyDescription = "many";
				}

				sb.AppendLine();
				if (maxSeverity >= WoundSeverity.Severe)
				{
					sb.AppendLine(
						$"{mortal.HowSeen(Actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} has {quantiyDescription} {woundsName}, some {maxSeverity.Describe()}."
							.Wrap(InnerLineFormatLength));
				}
				else
				{
					sb.AppendLine(
						$"{mortal.HowSeen(Actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} has {quantiyDescription} {woundsName}, though none especially severe."
							.Wrap(InnerLineFormatLength));
				}
			}
			else
			{
				sb.AppendLine();
				sb.AppendLine(woundDescs.Any()
					? $"{mortal.HowSeen(Actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} has {woundDescs.Select(x => $"{x.Description}{(x.Bodypart != null ? $" on {genderWord} {x.Bodypart.FullDescription()}" : "")}").ListToCompactString()}"
					  .Fullstop().Wrap(Actor.InnerLineFormatLength)
					: $"{mortal.HowSeen(Actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} has no visible {woundsName}.\n"
						.Wrap(Actor.InnerLineFormatLength));
			}


			var lodgedWounds = wounds.Where(x => x.Lodged != null).ToList();
			if (lodgedWounds.Any())
			{
				sb.AppendLine();
				sb.AppendLine(lodgedWounds.Any(x => x.Bodypart == null)
					? $"{gender.Subjective(true)} {gender.Has()} {lodgedWounds.Select(x => x.Lodged.HowSeen(Actor)).ListToString()} in {gender.Objective()}."
						.Wrap(InnerLineFormatLength)
					: $"{gender.Subjective(true)} {gender.Has()} {lodgedWounds.Select(x => $"{x.Lodged.HowSeen(Actor)} lodged in {genderWord} {x.Bodypart.FullDescription()}").ListToString()}."
						.Wrap(InnerLineFormatLength));
			}
		}

		if (thing is ICharacter actor)
		{
			var gender = actor.ApparentGender(Actor);

			// Severed Bodyparts
			if (actor.Body.SeveredRoots.Any())
			{
				sb.AppendLine();
				var severs =
					actor.Body.SeveredRoots.Where(
							 x =>
								 !(x is IOrganProto) &&
								 x.Significant &&
								 actor.Body.WornItemsProfilesFor(x)
									  .All(
										  y =>
											  (y.Item1.GetItemType<IWearable>()?.GloballyTransparent ?? false) &&
											  !y.Item2.HidesSeveredBodyparts) &&
								 actor.Body.Prosthetics.All(y =>
									 x != y.TargetBodypart && !x.DownstreamOfPart(y.TargetBodypart)))
						 .GroupBy(x => actor.Body.GetLimbFor(x))
						 .ToList();
				foreach (var item in severs.OrderByDescending(x => x.Key != null))
				{
					if (item.Key == null)
					{
						sb.AppendLine(
							$"{gender.Subjective(true)} {gender.Is()} missing {gender.Possessive()} {item.Select(x => x.FullDescription().ToLowerInvariant()).ListToString()}."
								.Wrap(InnerLineFormatLength).Colour(Telnet.Yellow));
						continue;
					}

					sb.AppendLine(
						$"{gender.Possessive(true)} {item.Key.Name.ToLowerInvariant()} is severed at the {item.Select(x => x.FullDescription().ToLowerInvariant()).ListToString()}."
							.Wrap(InnerLineFormatLength).Colour(Telnet.Yellow));
				}

				var insignificantSevers =
					actor.Body.SeveredRoots.Where(
						x =>
							!(x is IOrganProto) &&
							!x.Significant && Prosthetics.All(y => x != y.TargetBodypart) &&
							actor.Body.WornItemsProfilesFor(x).All(y => !y.Item2.HidesSeveredBodyparts)).ToList();
				if (insignificantSevers.Any())
				{
					sb.AppendLine(
						$"{gender.Subjective(true)} {gender.Is()} missing {gender.Possessive()} {insignificantSevers.Select(x => x.FullDescription().ToLowerInvariant()).ListToString()}."
							.Wrap(InnerLineFormatLength).Colour(Telnet.Yellow));
				}

				// Prosthetics
				var prosthetics = (actor == Actor || Actor.IsAdministrator()
						? actor.Body.Prosthetics
						: actor.Body.Prosthetics.Select(
								   x => (Item: x, Profiles: x.IncludedParts
															 .Select(y => actor.Body.WornItemsProfilesFor(y).ToList())
															 .ToList()))
							   .Where(x => x.Item.Obvious && CanSee(x.Item.Parent) && (!x.Profiles.Any() ||
								   !x.Profiles.All(y => y.Any(z =>
									   z.Item1.GetItemType<IWearable>()?.GloballyTransparent != true &&
									   z.Item2.HidesSeveredBodyparts))))
							   .Select(x => x.Item))
					.ToList();
				if (prosthetics.Any())
				{
					sb.AppendLine(
						$"{gender.Subjective(true)} {gender.Has()} {prosthetics.Select(x => x.Parent.HowSeen(Actor)).ListToString()}."
							.Wrap(InnerLineFormatLength));
				}
			}

			// External Implants
			var externalImplants = actor.Body.Implants.Where(x =>
				x.External && !string.IsNullOrWhiteSpace(x.ExternalDescription)).ToList();
			var visibleExternalImplants = externalImplants.Where(x => actor.Body.ExposedBodyparts.Any(y => y.CountsAs(x.TargetBodypart))).ToList();
			if (visibleExternalImplants.Any() || ((Actor.IsAdministrator() || Actor == actor) && externalImplants.Any()))
			{
				sb.AppendLine(
					$"{gender.Subjective(true)} {gender.Has()} {externalImplants.Select(x => $"{x.ExternalDescription.Colour(Telnet.Yellow)} in {gender.Possessive()} {x.TargetBodypart.FullDescription()}{(visibleExternalImplants.Contains(x) ? "" : " [Covered]".ColourCommand())}").ListToString()}."
						.Wrap(Actor.Account.InnerLineFormatLength));
			}

			// Tattoos
			var tattoos = actor.Body.Tattoos.ToList();
			var visibleTattoos = tattoos.Where(x => actor.Body.ExposedBodyparts.Contains(x.Bodypart))
										.ToList();
			var quantityDesc = "among many others";
			switch (visibleTattoos.Count)
			{
				case 0:
					break;
				case 1:
				case 2:
				case 3:
				case 4:
					sb.AppendLine(
						$"{gender.Subjective(true)} {gender.Has()} {visibleTattoos.Select(x => $"{x.ShortDescription.Colour(Telnet.BoldOrange)} on {gender.Possessive()} {x.Bodypart.FullDescription()}").ListToString()}."
							.Wrap(InnerLineFormatLength));
					break;
				case 5:
					quantityDesc = "among a couple of others";
					goto default;
				case 6:
				case 7:
					quantityDesc = "among a few others";
					goto default;
				case 8:
				case 9:
				case 10:
					quantityDesc = "among several others";
					goto default;
				default:
					var biggest = visibleTattoos.OrderByDescending(x => x.Size)
												.ThenByDescending(x => x.CompletionPercentage).Take(3).ToList();
					sb.AppendLine(
						$"{gender.Subjective(true)} {gender.Has()} {biggest.Select(x => $"{x.ShortDescription.Colour(Telnet.BoldOrange)} on {gender.Possessive()} {x.Bodypart.FullDescription()}").ListToString()}, {quantityDesc}."
							.Wrap(InnerLineFormatLength));
					break;
			}

			// Scars
			var visibleScars = actor.Body.Scars.Where(x => actor.Body.ExposedBodyparts.Contains(x.Bodypart)).ToList();
			quantityDesc = "among many others";
			switch (visibleScars.Count)
			{
				case 0:
					break;
				case 1:
				case 2:
				case 3:
				case 4:
					sb.AppendLine(
						$"{gender.Subjective(true)} {gender.Has()} {visibleScars.Select(x => $"{x.ShortDescription.Colour(Telnet.BoldPink)} on {gender.Possessive()} {x.Bodypart.FullDescription()}").ListToString()}."
							.Wrap(InnerLineFormatLength));
					break;
				case 5:
					quantityDesc = "among a couple of others";
					goto default;
				case 6:
				case 7:
					quantityDesc = "among a few others";
					goto default;
				case 8:
				case 9:
				case 10:
					quantityDesc = "among several others";
					goto default;
				default:
					var biggest = visibleScars.OrderByDescending(x => x.Size).ThenByDescending(x => x.Distinctiveness)
											  .Take(3).ToList();
					sb.AppendLine(
						$"{gender.Subjective(true)} {gender.Has()} {biggest.Select(x => $"{x.ShortDescription.Colour(Telnet.BoldPink)} on {gender.Possessive()} {x.Bodypart.FullDescription()}").ListToString()}, {quantityDesc}."
							.Wrap(InnerLineFormatLength));
					break;
			}

			if (fromLookCommand)
			{
				if (Gameworld.GetStaticBool("ClanSimilaritiesShowInLook"))
				{
					foreach (var clan in Actor.ClanMemberships)
					{
						if (!clan.NetPrivileges.HasFlag(Community.ClanPrivilegeType.CanViewMembers))
						{
							continue;
						}

						var membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan.Clan);
						if (membership == null)
						{
							continue;
						}

						if (membership.Appointments.Any())
						{
							sb.AppendLine(
								$"{gender.Subjective(true)} is {membership.Rank.Title(actor).A_An(false, Telnet.Green)} and {membership.Appointments.Select(x => x.Title(actor).ColourValue()).ListToString()} in {membership.Clan.Name.ColourName()}");
						}
						else
						{
							sb.AppendLine(
								$"{gender.Subjective(true)} is {membership.Rank.Title(actor).A_An(false, Telnet.Green)} in {membership.Clan.Name.ColourName()}");
						}
					}
				}

				var dub = Dubs.FirstOrDefault(x => x.TargetId == thing.Id && x.TargetType == thing.FrameworkItemType);
				if (dub != null)
				{
					sb.AppendLine(
						$"You have dubbed this person {dub.Keywords.Select(x => x.Colour(Telnet.BoldWhite)).ListToString()}."
							.Wrap(InnerLineFormatLength));
				}

				var legalAuthority =
					Gameworld.LegalAuthorities.FirstOrDefault(x => x.EnforcementZones.Contains(thing.Location.Zone));
				if (legalAuthority is not null &&
					(Actor.IsAdministrator() || (legalAuthority.GetEnforcementAuthority(Actor) is not null &&
												 !actor.IdentityIsObscuredTo(Actor))))
				{
					var enforcer = legalAuthority.GetEnforcementAuthority(actor);
					if (enforcer is not null)
					{
						sb.AppendLine(
							$"{$"[Enforcement: {legalAuthority.Name}]".Colour(Telnet.BoldBlue)} {gender.Subjective(true)} is the legal class {legalAuthority.GetLegalClass(actor).Name.ColourValue()} and {enforcer.Name.A_An().ColourValue()}.");
					}
					else
					{
						sb.AppendLine(
							$"{$"[Enforcement: {legalAuthority.Name}]".Colour(Telnet.BoldBlue)} {gender.Subjective(true)} is the legal class {legalAuthority.GetLegalClass(actor).Name.ColourValue()}.");
					}
				}
			}

			// Inventory
			sb.AppendLine();

			if (!CanSee(thing.Location) && !thing.IsSelf(this))
			{
				sb.AppendLine("It is too dark for you to make out any of their equipment.");
				return sb.ToString();
			}

			sb.AppendLine(actor.Body.GetInventoryString(Actor));
		}

		return sb.ToString();
	}

	public string LookInText(IPerceivable thing)
	{
		if (thing is not IGameItem item ||
			(!item.IsItemType<IContainer>() && !item.IsItemType<ISheath>() && !item.IsItemType<ILiquidContainer>()))
		{
			return thing.HowSeen(Actor, true) + " is not something that can be looked in.";
		}

		if (item.GetItemType<IOpenable>()?.IsOpen == false)
		{
			if (item.GetItemType<IContainer>()?.Transparent != false)
			{
				return thing.HowSeen(Actor, true) + " is closed, and must be opened before you can look in it.";
			}
		}

		return thing.HowSeen(Actor, true) + "\n" + thing.HowSeen(Actor, true, DescriptionType.Contents);
	}

	public string LookTattoosText(ICharacter actor, IBodypart forBodypart = null)
	{
		if (forBodypart != null && !actor.Body.ExposedBodyparts.Contains(forBodypart))
		{
			return $"{actor.HowSeen(Actor, true)} does not have any visible tattoos on that bodypart.";
		}

		var visibleTattoos = forBodypart == null
			? actor.Body.Tattoos.Where(x => actor.Body.ExposedBodyparts.Contains(x.Bodypart)).ToList()
			: actor.Body.Tattoos.Where(x => x.Bodypart == forBodypart).ToList();
		;
		if (!visibleTattoos.Any())
		{
			return
				$"{actor.HowSeen(Actor, true)} does not have any visible tattoos{(forBodypart == null ? "" : " on that bodypart")}.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{actor.HowSeen(Actor, true)} has the following tattoos:");
		foreach (var tattoo in visibleTattoos)
		{
			sb.AppendLine(
				$"\t{tattoo.ShortDescription.SubstituteWrittenLanguage(Actor, Gameworld).Proper().Colour(Telnet.BoldOrange)} on the {tattoo.Bodypart.FullDescription()}{(tattoo.TimeOfInscription.Calendar.CurrentDateTime - tattoo.TimeOfInscription > TimeSpan.FromDays(14) ? "" : " (fresh)")}");
			if (forBodypart != null)
			{
				sb.AppendLine(
					$"\n{tattoo.FullDescription.SubstituteWrittenLanguage(Actor, Gameworld).Wrap(Actor.InnerLineFormatLength, "\t\t")}");
			}
		}

		return sb.ToString();
	}

	public string LookScarsText(ICharacter actor, IBodypart forBodypart = null)
	{
		if (forBodypart != null && !actor.Body.ExposedBodyparts.Contains(forBodypart))
		{
			return $"{actor.HowSeen(Actor, true)} does not have any visible scars on that bodypart.";
		}

		var visibleScars = forBodypart == null
			? actor.Body.Scars.Where(x => actor.Body.ExposedBodyparts.Contains(x.Bodypart)).ToList()
			: actor.Body.Scars.Where(x => x.Bodypart == forBodypart).ToList();
		;
		if (!visibleScars.Any())
		{
			return
				$"{actor.HowSeen(Actor, true)} does not have any visible scars{(forBodypart == null ? "" : " on that bodypart")}.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{actor.HowSeen(Actor, true)} has the following scars:");
		foreach (var scar in visibleScars)
		{
			sb.AppendLine(
				$"\t{scar.ShortDescription.SubstituteWrittenLanguage(Actor, Gameworld).Proper().ColourIncludingReset(Telnet.BoldPink)} on the {scar.Bodypart.FullDescription()}");
			if (forBodypart != null)
			{
				sb.AppendLine(
					$"\n{scar.FullDescription.SubstituteWrittenLanguage(Actor, Gameworld).SubstituteANSIColour().Wrap(Actor.InnerLineFormatLength, "\t\t")}");
			}
		}

		return sb.ToString();
	}

	public string LookGraffitiText(string target)
	{
		var sb = new StringBuilder();
		var graffitis = Location.EffectsOfType<IGraffitiEffect>(x => x.Layer == RoomLayer).ToList();
		if (string.IsNullOrEmpty(target))
		{
			if (!graffitis.Any())
			{
				return "There are is no graffiti in this location.";
			}

			sb.AppendLine($"There are the following items of graffiti here:\n");
			var i = 1;
			foreach (var g in graffitis)
			{
				sb.AppendLine($"\t#{i++.ToString("N0", Actor)}) {g.Writing?.DescribeInLook(Actor) ?? "unknown graffiti".ColourError()}{(string.IsNullOrEmpty(g.LocaleDescription) ? "" : $" ({g.LocaleDescription.ColourCommand()})")}");
			}
			return sb.ToString();
		}

		var graffiti = graffitis.GetFromItemListByKeyword(target, Actor);
		if (graffiti?.Writing is null)
		{
			return $"There is no graffiti here that can be targeted with the keyword {target.ColourCommand()}.";
		}

		return DescribeGraffiti(graffiti);
	}

	public string LookGraffitiThingText(IGameItem item, string target)
	{
		var sb = new StringBuilder();
		var graffitis = item.EffectsOfType<IGraffitiEffect>().ToList();
		if (string.IsNullOrEmpty(target))
		{
			if (!graffitis.Any())
			{
				return $"There is no graffiti on {item.HowSeen(Actor)}.";
			}

			sb.AppendLine($"{item.HowSeen(Actor, true)} has the following items of graffiti:\n");
			var i = 1;
			foreach (var g in graffitis)
			{
				sb.AppendLine($"\t#{i++.ToString("N0", Actor)}) {g.Writing?.DescribeInLook(Actor) ?? "unknown graffiti".ColourError()}");
			}
			return sb.ToString();
		}

		var graffiti = graffitis.GetFromItemListByKeyword(target, Actor);
		if (graffiti?.Writing is null)
		{
			return $"There is no graffiti on {item.HowSeen(Actor)} that can be targeted with the keyword {target.ColourCommand()}.";
		}

		return DescribeGraffiti(graffiti);
	}

#nullable enable
	private string DescribeGraffiti(IGraffitiEffect graffiti)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{graffiti.Writing!.DescribeInLook(Actor) ?? "unknown graffiti".ColourError()}{(string.IsNullOrEmpty(graffiti.LocaleDescription) ? "" : $" ({graffiti.LocaleDescription.ColourCommand()})")}");
		sb.AppendLine();
		sb.AppendLine($"Size: {graffiti.Writing.DrawingSize.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Medium: {graffiti.Writing.ImplementType.Describe().ColourValue()}");
		sb.AppendLine($"Colour: {graffiti.Writing.WritingColour.Name.ColourValue()}");
		var drawingSkill = Gameworld.Traits.Get(Gameworld.GetStaticLong("DrawingTraitId"));
		if (drawingSkill is not null)
		{
			sb.AppendLine($"Skill: {drawingSkill.Decorator.Decorate(graffiti.Writing.DrawingSkill).Strip(c => c is '(' or ')').ColourValue()}");
		}

		if (!graffiti.IsJustDrawing)
		{
			sb.AppendLine($"Language: {(CanIdentifyLanguage(graffiti.Writing.Language) ? graffiti.Writing.Language.Name.ColourValue() : graffiti.Writing.Language.UnknownLanguageSpokenDescription.ColourValue())}");
			sb.AppendLine($"Script: {(Scripts.Contains(graffiti.Writing.Script) ? graffiti.Writing.Script.KnownScriptDescription.ColourValue() : graffiti.Writing.Script.UnknownScriptDescription.ColourValue())}");
			sb.AppendLine($"Style: {(Scripts.Contains(graffiti.Writing.Script) ? graffiti.Writing.Style.Describe().ColourValue() : graffiti.Writing.Script.UnknownScriptDescription.ColourValue())}");
			var handwritingSkill = Gameworld.Traits.Get(Gameworld.GetStaticLong("HandwritingSkillId"));
			if (handwritingSkill is not null)
			{
				sb.AppendLine($"Handwriting: {handwritingSkill.Decorator.Decorate(graffiti.Writing.HandwritingSkill).Strip(c => c is '(' or ')').ColourValue()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine(new string(Actor.Account.UseUnicode ? '═' : '=', Actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine(graffiti.Writing.ParseFor(Actor));
		return sb.ToString();
	}
#nullable restore

	public string LookWoundsText(IMortalPerceiver thing)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{thing.HowSeen(Actor, true)} has the following wounds:");
		var wounds = thing.VisibleWounds(Actor,
							  Actor.Combat == null ? WoundExaminationType.Look : WoundExaminationType.Glance)
						  .OrderBy(x => (x.Bodypart as IExternalBodypart)?.DisplayOrder ?? 0)
						  .ThenByDescending(x => x.Severity);
		foreach (var wound in wounds)
		{
			if (wound.Bodypart != null)
			{
				sb.AppendLine(
					$"{wound.Describe(Actor.Combat == null ? WoundExaminationType.Look : WoundExaminationType.Glance, Outcome.MajorPass)} on the {wound.Bodypart.FullDescription()}");
			}
			else
			{
				sb.AppendLine(
					$"{wound.Describe(Actor.Combat == null ? WoundExaminationType.Look : WoundExaminationType.Glance, Outcome.MajorPass)}");
			}
		}

		return sb.ToString();
	}

	public void Look(bool fromMovement = false)
	{
		var text = LookText(fromMovement);
		if (!string.IsNullOrEmpty(text))
		{
			OutputHandler.Send(text, false, true);
		}
	}

	public void LookIn(IPerceivable thing)
	{
		OutputHandler.Send(LookInText(thing));
	}

	public void LookScars(ICharacter actor, IBodypart forBodypart = null)
	{
		OutputHandler.Send(LookScarsText(actor, forBodypart));
	}

	public void LookTattoos(ICharacter actor, IBodypart forBodypart = null)
	{
		OutputHandler.Send(LookTattoosText(actor, forBodypart));
	}

	public void LookWounds(IMortalPerceiver thing)
	{
		OutputHandler.Send(LookWoundsText(thing));
	}


	public void LookGraffiti(string target)
	{
		OutputHandler.Send(LookGraffitiText(target));
	}

	public void LookGraffitiThing(IGameItem item, string target)
	{
		OutputHandler.Send(LookGraffitiThingText(item, target));
	}

	public override PerceptionTypes NaturalPerceptionTypes => Race.NaturalPerceptionTypes;

	public void Look(IPerceivable thing)
	{
		OutputHandler.Send(LookText(thing, true), false, true);
	}

	public override bool Sentient => true;

	public override IList<IDub> Dubs => Actor.Dubs;

	public override bool HasDubFor(IKeyworded target, IEnumerable<string> keywords)
	{
		return Actor.HasDubFor(target, keywords);
	}

	public override bool HasDubFor(IKeyworded target, string keyword)
	{
		return Actor.HasDubFor(target, keyword);
	}

	#endregion

	#region ITarget

	public IPerceivable? Target(string keyword)
	{
		return Actor.Target(keyword);
	}

	public virtual IPerceivable? TargetLocal(string keyword)
	{
		return Actor.TargetLocal(keyword);
	}

	public ICharacter? TargetActor(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
	{
		return Actor.TargetActor(keyword, ignoreFlags);
	}

	public ICharacter? TargetActorOrCorpse(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
	{
		return Actor.TargetActorOrCorpse(keyword, ignoreFlags);
	}

	public ICorpse? TargetCorpse(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
	{
		return Actor.TargetCorpse(keyword, ignoreFlags);
	}

	public ICharacter? TargetAlly(string keyword)
	{
		return Actor.TargetAlly(keyword);
	}

	public virtual ICharacter? TargetNonAlly(string keyword)
	{
		return Actor.TargetNonAlly(keyword);
	}

	public IBody? TargetBody(string keyword)
	{
		return Actor.TargetBody(keyword);
	}

	public IGameItem? TargetItem(string keyword)
	{
		return Actor.TargetItem(keyword);
	}

	public IGameItem? TargetLocalItem(string keyword)
	{
		return Actor.TargetLocalItem(keyword);
	}


	public (ICharacter? Target, IEnumerable<ICellExit> Path) TargetDistantActor(string keyword, ICellExit? initialExit,
		uint maximumRange,
		bool respectDoors, bool respectCorners)
	{
		return Actor.TargetDistantActor(keyword, initialExit, maximumRange, respectDoors, respectCorners);
	}

	public (IGameItem? Target, IEnumerable<ICellExit> Path) TargetDistantItem(string keyword, ICellExit? initialExit,
		uint maximumRange,
		bool respectDoors, bool respectCorners)
	{
		return Actor.TargetDistantItem(keyword, initialExit, maximumRange, respectDoors, respectCorners);
	}

	public IGameItem? TargetPersonalItem(string keyword)
	{
		return Actor.TargetPersonalItem(keyword);
	}

	public IGameItem? TargetLocalOrHeldItem(string keyword)
	{
		return Actor.TargetLocalOrHeldItem(keyword);
	}

	public IGameItem? TargetHeldItem(string keyword)
	{
		return Actor.TargetHeldItem(keyword);
	}

	public IGameItem? TargetWornItem(string keyword)
	{
		return Actor.TargetWornItem(keyword);
	}

	public IGameItem? TargetTopLevelWornItem(string keyword)
	{
		return Actor.TargetTopLevelWornItem(keyword);
	}

	public string BestKeywordFor(IPerceivable target)
	{
		return Actor.BestKeywordFor(target);
	}

	public string BestKeywordFor(ICharacter target)
	{
		return Actor.BestKeywordFor(target);
	}

	public string BestKeywordFor(IGameItem target)
	{
		return Actor.BestKeywordFor(target);
	}

	#endregion
}