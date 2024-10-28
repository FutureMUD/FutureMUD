using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Combat;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Character;

public partial class Character
{
	#region IFutureProgVariable Implementation

	IProgVariable IProgVariable.GetProperty(string property)
	{
		IProgVariable returnVar;
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
				returnVar = new CollectionVariable(Body.ExternalItems.ToList(), ProgVariableTypes.Item);
				break;
			case "helditems":
				returnVar = new CollectionVariable(Body.HeldItems.ToList(), ProgVariableTypes.Item);
				break;
			case "wieldeditems":
				returnVar = new CollectionVariable(Body.WieldedItems.ToList(), ProgVariableTypes.Item);
				break;
			case "wornitems":
				returnVar = new CollectionVariable(Body.WornItems.ToList(), ProgVariableTypes.Item);
				break;
			case "visiblewornitems":
				returnVar =
					new CollectionVariable(
						Body.WornItems.Where(
							    x => Body.CoverInformation(x).All(y => y.Item1 != WearableItemCoverStatus.Covered))
						    .ToList(), ProgVariableTypes.Item);
				break;
			case "clans":
				returnVar = new CollectionVariable(ClanMemberships.Select(x => x.Clan).ToList(),
					ProgVariableTypes.Clan);
				break;
			case "skills":
				returnVar = new CollectionVariable(
					TraitsOfType(TraitType.Skill).Select(x => x.Definition).ToList(), ProgVariableTypes.Trait);
				break;
			case "accents":
				returnVar = new CollectionVariable(Accents.ToList(), ProgVariableTypes.Accent);
				break;
			case "languages":
				returnVar = new CollectionVariable(Languages.ToList(), ProgVariableTypes.Language);
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
						ProgVariableTypes.Character);
				break;
			case "npc":
				return new BooleanVariable(!IsPlayerCharacter);
			case "guest":
				return new BooleanVariable(IsGuest);
			case "pc":
				return new BooleanVariable(IsPlayerCharacter);
			case "isnewplayer":
				return new BooleanVariable(Effects.Any(x => x is NewPlayer));
			case "linewidth":
				return new NumberVariable(LineFormatLength);
			case "innerlinewidth":
				return new NumberVariable(InnerLineFormatLength);
			case "language":
				return CurrentLanguage;
			case "accent":
				return CurrentAccent;
			case "merits":
				return new CollectionVariable(Merits.ToList(), ProgVariableTypes.Merit);
			case "applicablemerits":
				return new CollectionVariable(Merits.Where(x => x.Applies(this)).ToList(),
					ProgVariableTypes.Merit);
			case "roles":
				return new CollectionVariable(Roles.ToList(), ProgVariableTypes.Role);
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
					ProgVariableTypes.Text);
			case "requiredintentions":
				return new CollectionVariable(
					new List<TextVariable>(CombatSettings.RequiredIntentions.GetFlags().OfType<CombatMoveIntentions>()
					                                     .Select(x => new TextVariable(x.Describe()))),
					ProgVariableTypes.Text);
			case "forbiddenintentions":
				return new CollectionVariable(
					new List<TextVariable>(CombatSettings.ForbiddenIntentions.GetFlags().OfType<CombatMoveIntentions>()
					                                     .Select(x => new TextVariable(x.Describe()))),
					ProgVariableTypes.Text);
			case "drugs":
				return new CollectionVariable(Body.ActiveDrugDosages.Select(x => x.Drug).Distinct().ToList(),
					ProgVariableTypes.Drug);
			case "drugamounts":
				return new CollectionVariable(
					Body.ActiveDrugDosages.Select(x => x.Drug).Distinct().Select(x =>
						new NumberVariable(Body.ActiveDrugDosages.Where(y => y.Drug == x).Sum(y => y.Grams))).ToList(),
					ProgVariableTypes.Number);
			case "latentdrugs":
				return new CollectionVariable(Body.LatentDrugDosages.Select(x => x.Drug).Distinct().ToList(),
					ProgVariableTypes.Drug);
			case "latentdrugamounts":
				return new CollectionVariable(
					Body.LatentDrugDosages.Select(x => x.Drug).Distinct().Select(x =>
						new NumberVariable(Body.ActiveDrugDosages.Where(y => y.Drug == x).Sum(y => y.Grams))).ToList(),
					ProgVariableTypes.Number);
			case "outfits":
				return new CollectionVariable(Outfits.ToList(), ProgVariableTypes.Outfit);
			case "layer":
				return new TextVariable(RoomLayer.DescribeEnum());
			case "special":
				return new BooleanVariable(false);
			case "simple":
				return new BooleanVariable(false);
			default:
				return base.GetProperty(property);
		}

		return returnVar;
	}

	public override ProgVariableTypes Type => ProgVariableTypes.Character;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "effects", ProgVariableTypes.Collection | ProgVariableTypes.Effect },
			{ "name", ProgVariableTypes.Text },
			{ "type", ProgVariableTypes.Text },
			{ "simplefullname", ProgVariableTypes.Text },
			{ "fullname", ProgVariableTypes.Text },
			{ "surname", ProgVariableTypes.Text },
			{ "csimplefullname", ProgVariableTypes.Text },
			{ "cfullname", ProgVariableTypes.Text },
			{ "csurname", ProgVariableTypes.Text },
			{ "cname", ProgVariableTypes.Text },
			{ "gender", ProgVariableTypes.Gender },
			{ "height", ProgVariableTypes.Number },
			{ "weight", ProgVariableTypes.Number },
			{ "location", ProgVariableTypes.Location },
			{ "age", ProgVariableTypes.Number },
			{ "agecategory", ProgVariableTypes.Text },
			{ "race", ProgVariableTypes.Race },
			{ "culture", ProgVariableTypes.Culture },
			{ "currency", ProgVariableTypes.Currency },
			{ "ethnicity", ProgVariableTypes.Ethnicity },
			{ "zone", ProgVariableTypes.Zone },
			{ "shard", ProgVariableTypes.Shard },
			{ "inventory", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "helditems", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "wieldeditems", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "wornitems", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "visiblewornitems", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "clans", ProgVariableTypes.Collection | ProgVariableTypes.Clan },
			{ "skills", ProgVariableTypes.Trait | ProgVariableTypes.Collection },
			{ "class", ProgVariableTypes.Text },
			{ "subclass", ProgVariableTypes.Text },
			{ "ingroup", ProgVariableTypes.Boolean },
			{ "groupmembers", ProgVariableTypes.Collection | ProgVariableTypes.Character },
			{ "npc", ProgVariableTypes.Boolean },
			{ "pc", ProgVariableTypes.Boolean },
			{ "accents", ProgVariableTypes.Accent | ProgVariableTypes.Collection },
			{ "languages", ProgVariableTypes.Language | ProgVariableTypes.Collection },
			{ "guest", ProgVariableTypes.Boolean },
			{ "linewidth", ProgVariableTypes.Number },
			{ "innerlinewidth", ProgVariableTypes.Number },
			{ "language", ProgVariableTypes.Language },
			{ "accent", ProgVariableTypes.Accent },
			{ "merits", ProgVariableTypes.Merit | ProgVariableTypes.Collection },
			{ "applicablemerits", ProgVariableTypes.Merit | ProgVariableTypes.Collection },
			{ "roles", ProgVariableTypes.Role | ProgVariableTypes.Collection },
			{ "playtime", ProgVariableTypes.Number },
			{ "incombat", ProgVariableTypes.Boolean },
			{ "combattarget", ProgVariableTypes.Perceiver },
			{ "combattargetchar", ProgVariableTypes.Character },
			{ "preferredintentions", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "requiredintentions", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "forbiddenintentions", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "drugs", ProgVariableTypes.Drug | ProgVariableTypes.Collection },
			{ "drugamounts", ProgVariableTypes.Number | ProgVariableTypes.Collection },
			{ "latentdrugs", ProgVariableTypes.Drug | ProgVariableTypes.Collection },
			{ "latentdrugamounts", ProgVariableTypes.Number | ProgVariableTypes.Collection },
			{ "outfits", ProgVariableTypes.Outfit | ProgVariableTypes.Collection },
			{ "layer", ProgVariableTypes.Text },
			{ "isnewplayer", ProgVariableTypes.Boolean }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The id of the character" },
			{ "effects", "A collection of all effects on the character" },
			{ "name", "Their real first name" },
			{ "simplefullname", "The simple version of their real full name" },
			{ "fullname", "Their real full name" },
			{ "surname", "Their real surname" },
			{ "csimplefullname", "The simple version of their current alias' full name" },
			{ "cfullname", "Their current alias' full name" },
			{ "csurname", "Their current alias' surname" },
			{ "cname", "Their current alias' first name" },
			{ "gender", "Their real gender" },
			{ "height", "Their height in base units (cm)" },
			{ "weight", "Their weight in base units (grams)" },
			{ "location", "The room that they are in" },
			{ "age", "Their age in years" },
			{ "agecategory", "Their age category as text" },
			{ "race", "Their race" },
			{ "culture", "Their culture" },
			{ "currency", "The currency they are using in transactions" },
			{ "ethnicity", "Their ethnicity" },
			{ "zone", "The zone that they are in" },
			{ "shard", "The shard that they are in" },
			{ "inventory", "A collection of all items in their inventory" },
			{ "helditems", "A collection of all items they're holding" },
			{ "wieldeditems", "A collection of all items they're wielding" },
			{ "wornitems", "A collection of all items they're wearing" },
			{ "visiblewornitems", "A collection of all uncovered or partially exposed worn items" },
			{ "clans", "A collection of all the clans they're a member of" },
			{ "skills", "A collection of skills that they have a value in" },
			{ "class", "Their class role, if any (can be null)" },
			{ "subclass", "Their subclass role, if any (can be null)" },
			{ "ingroup", "True if they are currently in a group" },
			{ "groupmembers", "A collection of all the member of their group" },
			{ "npc", "True if they are an NPC" },
			{ "pc", "True if they are a PC" },
			{ "accents", "A collection of all of the assets that they have familiarity with" },
			{ "languages", "A collection of all the languages that they know" },
			{ "guest", "True if they are a guest" },
			{ "linewidth", "Their account's line width setting, in characters" },
			{ "innerlinewidth", "Their account's inner line width setting, in characters" },
			{ "language", "The language they are currently speaking (can be null)" },
			{ "accent", "The accent they are currently speaking (can be null)" },
			{ "merits", "A collection of all of their merits and flaws" },
			{ "applicablemerits", "A collection of all of their merits and flaws currently active" },
			{ "roles", "A collection of all of their roles" },
			{ "playtime", "Their total playtime in minutes" },
			{ "incombat", "True if they are currently in combat" },
			{ "combattarget", "Who or what they are currently targeting in combat (can be null)" },
			{ "combattargetchar", "Who they are currently targeting in combat (can be null)" },
			{ "preferredintentions", "A collection of text representing the intentions they prefer in selecting combat moves" },
			{ "requiredintentions", "A collection of text representing the intentions they require in selecting combat moves" },
			{ "forbiddenintentions", "A collection of text representing the intentions they forbid in selecting combat moves" },
			{ "drugs", "A collection of drugs currently affecting them" },
			{ "drugamounts", "A collection of the grams of each drug. Items match order of drugs property" },
			{ "latentdrugs", "A collection of drugs not yet affecting them but in their system" },
			{ "latentdrugamounts", "A collection of the grams of each latent drug. Items match order of latentdrugs property" },
			{ "outfits", "A collection of outfits associated with this character" },
			{ "layer", "A text representation of the current layer this character is in" },
			{ "isnewplayer", "True if character has the (New Player) tag" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Character, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}