﻿using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Character;

public partial class Character
{
	#region IFutureProgVariable Implementation

	IFutureProgVariable IFutureProgVariable.GetProperty(string property)
	{
		IFutureProgVariable returnVar;
		switch (property.ToLowerInvariant())
		{
			case "name":
				returnVar = new TextVariable(PersonalName.GetName(NameStyle.GivenOnly));
				break;
			case "cname":
				returnVar = new TextVariable(CurrentName.GetName(NameStyle.GivenOnly));
				break;
			case "simplefullname":
				returnVar = new TextVariable(PersonalName.GetName(NameStyle.SimpleFull));
				break;
			case "csimplefullname":
				returnVar = new TextVariable(CurrentName.GetName(NameStyle.SimpleFull));
				break;
			case "fullname":
				returnVar = new TextVariable(PersonalName.GetName(NameStyle.FullName));
				break;
			case "cfullname":
				returnVar = new TextVariable(CurrentName.GetName(NameStyle.FullName));
				break;
			case "surname":
				returnVar = new TextVariable(PersonalName.GetName(NameStyle.SurnameOnly));
				break;
			case "csurname":
				returnVar = new TextVariable(CurrentName.GetName(NameStyle.SurnameOnly));
				break;
			case "gender":
				returnVar = new GenderVariable(Body.Gender.Enum);
				break;
			case "height":
				returnVar = new NumberVariable(Body.Height);
				break;
			case "weight":
				returnVar = new NumberVariable(Body.Weight);
				break;
			case "id":
				returnVar = new NumberVariable(Id);
				break;
			case "location":
				returnVar = Location;
				break;
			case "age":
				returnVar = new NumberVariable(Birthday.Calendar.CurrentDate.YearsDifference(Birthday));
				break;
			case "agecategory":
				returnVar = new TextVariable(Race.AgeCategory(this).DescribeEnum());
				break;
			case "race":
				returnVar = Race;
				break;
			case "culture":
				returnVar = Culture;
				break;
			case "ethnicity":
				returnVar = Ethnicity;
				break;
			case "currency":
				returnVar = Currency;
				break;
			case "zone":
				returnVar = Location.Zone;
				break;
			case "shard":
				returnVar = Location.Shard;
				break;
			case "inventory":
				returnVar = new CollectionVariable(Body.ExternalItems.ToList(), FutureProgVariableTypes.Item);
				break;
			case "helditems":
				returnVar = new CollectionVariable(Body.HeldItems.ToList(), FutureProgVariableTypes.Item);
				break;
			case "wieldeditems":
				returnVar = new CollectionVariable(Body.WieldedItems.ToList(), FutureProgVariableTypes.Item);
				break;
			case "wornitems":
				returnVar = new CollectionVariable(Body.WornItems.ToList(), FutureProgVariableTypes.Item);
				break;
			case "visiblewornitems":
				returnVar =
					new CollectionVariable(
						Body.WornItems.Where(
							    x => Body.CoverInformation(x).All(y => y.Item1 != WearableItemCoverStatus.Covered))
						    .ToList(), FutureProgVariableTypes.Item);
				break;
			case "clans":
				returnVar = new CollectionVariable(ClanMemberships.Select(x => x.Clan).ToList(),
					FutureProgVariableTypes.Clan);
				break;
			case "skills":
				returnVar = new CollectionVariable(
					TraitsOfType(TraitType.Skill).Select(x => x.Definition).ToList(), FutureProgVariableTypes.Trait);
				break;
			case "accents":
				returnVar = new CollectionVariable(Accents.ToList(), FutureProgVariableTypes.Accent);
				break;
			case "languages":
				returnVar = new CollectionVariable(Languages.ToList(), FutureProgVariableTypes.Language);
				break;
			case "class":
				returnVar =
					new TextVariable(
						Roles.Where(x => x.RoleType == ChargenRoleType.Class)
						     .Select(x => x.Name)
						     .DefaultIfEmpty("None")
						     .First());
				break;
			case "subclass":
				returnVar =
					new TextVariable(
						Roles.Where(x => x.RoleType == ChargenRoleType.Subclass)
						     .Select(x => x.Name)
						     .DefaultIfEmpty("None")
						     .First());
				break;
			case "ingroup":
				returnVar = new BooleanVariable(Party != null);
				break;
			case "groupmembers":
				returnVar =
					new CollectionVariable(
						Party?.CharacterMembers.ToList() ?? new List<ICharacter>().ToList(),
						FutureProgVariableTypes.Character);
				break;
			case "npc":
				return new BooleanVariable(!IsPlayerCharacter);
			case "guest":
				return new BooleanVariable(IsGuest);
			case "pc":
				return new BooleanVariable(IsPlayerCharacter);
			case "linewidth":
				return new NumberVariable(LineFormatLength);
			case "innerlinewidth":
				return new NumberVariable(InnerLineFormatLength);
			case "language":
				return CurrentLanguage;
			case "accent":
				return CurrentAccent;
			case "merits":
				return new CollectionVariable(Merits.ToList(), FutureProgVariableTypes.Merit);
			case "applicablemerits":
				return new CollectionVariable(Merits.Where(x => x.Applies(this)).ToList(),
					FutureProgVariableTypes.Merit);
			case "roles":
				return new CollectionVariable(Roles.ToList(), FutureProgVariableTypes.Role);
			case "playtime":
				return new NumberVariable(TotalMinutesPlayed);
			case "incombat":
				return new BooleanVariable(Combat != null);
			case "combattarget":
				return CombatTarget;
			case "combattargetchar":
				return CombatTarget as ICharacter;
			case "preferredintentions":
				return new CollectionVariable(
					new List<TextVariable>(CombatSettings.PreferredIntentions.GetFlags().OfType<CombatMoveIntentions>()
					                                     .Select(x => new TextVariable(x.Describe()))),
					FutureProgVariableTypes.Text);
			case "requiredintentions":
				return new CollectionVariable(
					new List<TextVariable>(CombatSettings.RequiredIntentions.GetFlags().OfType<CombatMoveIntentions>()
					                                     .Select(x => new TextVariable(x.Describe()))),
					FutureProgVariableTypes.Text);
			case "forbiddenintentions":
				return new CollectionVariable(
					new List<TextVariable>(CombatSettings.ForbiddenIntentions.GetFlags().OfType<CombatMoveIntentions>()
					                                     .Select(x => new TextVariable(x.Describe()))),
					FutureProgVariableTypes.Text);
			case "drugs":
				return new CollectionVariable(Body.ActiveDrugDosages.Select(x => x.Drug).Distinct().ToList(),
					FutureProgVariableTypes.Drug);
			case "drugamounts":
				return new CollectionVariable(
					Body.ActiveDrugDosages.Select(x => x.Drug).Distinct().Select(x =>
						new NumberVariable(Body.ActiveDrugDosages.Where(y => y.Drug == x).Sum(y => y.Grams))).ToList(),
					FutureProgVariableTypes.Number);
			case "latentdrugs":
				return new CollectionVariable(Body.LatentDrugDosages.Select(x => x.Drug).Distinct().ToList(),
					FutureProgVariableTypes.Drug);
			case "latentdrugamounts":
				return new CollectionVariable(
					Body.LatentDrugDosages.Select(x => x.Drug).Distinct().Select(x =>
						new NumberVariable(Body.ActiveDrugDosages.Where(y => y.Drug == x).Sum(y => y.Grams))).ToList(),
					FutureProgVariableTypes.Number);
			case "outfits":
				return new CollectionVariable(Outfits.ToList(), FutureProgVariableTypes.Outfit);
			case "layer":
				return new TextVariable(RoomLayer.DescribeEnum());
			case "special":
				return new BooleanVariable(false);
			default:
				return base.GetProperty(property);
		}

		return returnVar;
	}

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Character;

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "effects", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Effect },
			{ "name", FutureProgVariableTypes.Text },
			{ "type", FutureProgVariableTypes.Text },
			{ "simplefullname", FutureProgVariableTypes.Text },
			{ "fullname", FutureProgVariableTypes.Text },
			{ "surname", FutureProgVariableTypes.Text },
			{ "csimplefullname", FutureProgVariableTypes.Text },
			{ "cfullname", FutureProgVariableTypes.Text },
			{ "csurname", FutureProgVariableTypes.Text },
			{ "cname", FutureProgVariableTypes.Text },
			{ "gender", FutureProgVariableTypes.Gender },
			{ "height", FutureProgVariableTypes.Number },
			{ "weight", FutureProgVariableTypes.Number },
			{ "location", FutureProgVariableTypes.Location },
			{ "age", FutureProgVariableTypes.Number },
			{ "agecategory", FutureProgVariableTypes.Text },
			{ "race", FutureProgVariableTypes.Race },
			{ "culture", FutureProgVariableTypes.Culture },
			{ "currency", FutureProgVariableTypes.Currency },
			{ "ethnicity", FutureProgVariableTypes.Ethnicity },
			{ "zone", FutureProgVariableTypes.Zone },
			{ "shard", FutureProgVariableTypes.Shard },
			{ "inventory", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "helditems", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "wieldeditems", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "wornitems", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "visiblewornitems", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "clans", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Clan },
			{ "skills", FutureProgVariableTypes.Trait | FutureProgVariableTypes.Collection },
			{ "class", FutureProgVariableTypes.Text },
			{ "subclass", FutureProgVariableTypes.Text },
			{ "ingroup", FutureProgVariableTypes.Boolean },
			{ "groupmembers", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Character },
			{ "npc", FutureProgVariableTypes.Boolean },
			{ "pc", FutureProgVariableTypes.Boolean },
			{ "accents", FutureProgVariableTypes.Accent | FutureProgVariableTypes.Collection },
			{ "languages", FutureProgVariableTypes.Language | FutureProgVariableTypes.Collection },
			{ "guest", FutureProgVariableTypes.Boolean },
			{ "linewidth", FutureProgVariableTypes.Number },
			{ "innerlinewidth", FutureProgVariableTypes.Number },
			{ "language", FutureProgVariableTypes.Language },
			{ "accent", FutureProgVariableTypes.Accent },
			{ "merits", FutureProgVariableTypes.Merit | FutureProgVariableTypes.Collection },
			{ "applicablemerits", FutureProgVariableTypes.Merit | FutureProgVariableTypes.Collection },
			{ "roles", FutureProgVariableTypes.Role | FutureProgVariableTypes.Collection },
			{ "playtime", FutureProgVariableTypes.Number },
			{ "incombat", FutureProgVariableTypes.Boolean },
			{ "combattarget", FutureProgVariableTypes.Perceiver },
			{ "combattargetchar", FutureProgVariableTypes.Character },
			{ "preferredintentions", FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection },
			{ "requiredintentions", FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection },
			{ "forbiddenintentions", FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection },
			{ "drugs", FutureProgVariableTypes.Drug | FutureProgVariableTypes.Collection },
			{ "drugamounts", FutureProgVariableTypes.Number | FutureProgVariableTypes.Collection },
			{ "latentdrugs", FutureProgVariableTypes.Drug | FutureProgVariableTypes.Collection },
			{ "latentdrugamounts", FutureProgVariableTypes.Number | FutureProgVariableTypes.Collection },
			{ "outfits", FutureProgVariableTypes.Outfit | FutureProgVariableTypes.Collection },
			{ "layer", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "effects", "" },
			{ "simplefullname", "" },
			{ "fullname", "" },
			{ "surname", "" },
			{ "csimplefullname", "" },
			{ "cfullname", "" },
			{ "csurname", "" },
			{ "cname", "" },
			{ "gender", "" },
			{ "height", "" },
			{ "weight", "" },
			{ "location", "" },
			{ "age", "" },
			{ "agecategory", "" },
			{ "race", "" },
			{ "culture", "" },
			{ "currency", "" },
			{ "ethnicity", "" },
			{ "zone", "" },
			{ "shard", "" },
			{ "inventory", "" },
			{ "helditems", "" },
			{ "wieldeditems", "" },
			{ "wornitems", "" },
			{ "visiblewornitems", "" },
			{ "clans", "" },
			{ "skills", "" },
			{ "class", "" },
			{ "subclass", "" },
			{ "ingroup", "" },
			{ "groupmembers", "" },
			{ "npc", "" },
			{ "pc", "" },
			{ "accents", "" },
			{ "languages", "" },
			{ "guest", "" },
			{ "linewidth", "" },
			{ "innerlinewidth", "" },
			{ "language", "" },
			{ "accent", "" },
			{ "merits", "" },
			{ "applicablemerits", "" },
			{ "roles", "" },
			{ "playtime", "" },
			{ "incombat", "" },
			{ "combattarget", "" },
			{ "combattargetchar", "" },
			{ "preferredintentions", "" },
			{ "requiredintentions", "" },
			{ "forbiddenintentions", "" },
			{ "drugs", "" },
			{ "drugamounts", "" },
			{ "latentdrugs", "" },
			{ "latentdrugamounts", "" },
			{ "outfits", "" },
			{ "layer", "" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Character, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}