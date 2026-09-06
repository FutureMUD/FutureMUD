#nullable enable

using System.Globalization;
using MudSharp.Database;
using MudSharp.Economy.Property;
using MudSharp.TimeAndDate;

namespace MudSharp.Commands.Modules;

internal partial class ProgModule
{
	public static (object? result, bool success) GetArgument(ProgVariableTypes type, string parText,
		int parNumber, ICharacter actor)
	{
		var parameterArgument = parNumber > 0 ? $" at parameter {parNumber.ToString("N0", actor)}" : "";
		if (type == ProgVariableTypes.Anything || type == ProgVariableTypes.Literal)
		{
			return ArgumentError(actor, $"{type.Describe()} is a compiler type mask, not an input value type{parameterArgument}.");
		}
		type = type.WithoutLiteral();
		if (type == ProgVariableTypes.Void || type == ProgVariableTypes.Error || type == ProgVariableTypes.Literal)
		{
			return ArgumentError(actor, $"{type.Describe()} is not an input value type{parameterArgument}.");
		}

		if (type.IsCollection || type.IsDictionary || type.IsCollectionDictionary)
		{
			return ResolveContainerArgument(type, parText, parNumber, actor);
		}

		if (type.CompatibleWith(ProgVariableTypes.ReferenceType) && parText.EqualTo("null"))
		{
			return (null, true);
		}

		if (string.IsNullOrWhiteSpace(parText) && type != ProgVariableTypes.Text)
		{
			return ArgumentError(actor, $"You must supply a {type.Describe()} value{parameterArgument}.");
		}

		if (!type.IsExactType)
		{
			return ResolveUnionArgument(type, parText, parNumber, actor);
		}

		if (type == ProgVariableTypes.AgricultureField)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.AgricultureFields.GetByIdOrName(parText), "agriculture field", parameterArgument);
		}

		if (type == ProgVariableTypes.VehicleRoute)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.VehicleRoutes.GetByIdOrName(parText), "vehicle route", parameterArgument);
		}

		if (type == ProgVariableTypes.VehicleService)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.VehicleServices.GetByIdOrName(parText), "vehicle service", parameterArgument);
		}

		if (type == ProgVariableTypes.VehicleJourney)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.VehicleJourneys.GetByIdOrName(parText), "vehicle journey", parameterArgument);
		}

		if (type == ProgVariableTypes.NPCSkillPackage)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.NpcSkillPackages.GetByIdOrName(parText), "NPC skill package", parameterArgument);
		}

		if (type == ProgVariableTypes.SignedLanguage)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.SignedLanguages.GetByIdOrName(parText), "signed language", parameterArgument);
		}

		if (type == ProgVariableTypes.SignedVariety)
		{
			return ResolveSignedVarietyArgument(parText, parNumber, actor);
		}

		if (type == ProgVariableTypes.Effect || type == ProgVariableTypes.Trap)
		{
			return ResolveEffectArgument(type, parText, parNumber, actor);
		}

		if (type == ProgVariableTypes.Outfit || type == ProgVariableTypes.OutfitItem)
		{
			return ResolveOutfitArgument(type, parText, parNumber, actor);
		}

		if (type == ProgVariableTypes.LiquidMixture)
		{
			return ResolveLiquidMixtureArgument(parText, parNumber, actor);
		}

		if (type == ProgVariableTypes.Tag)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.Tags.GetByIdOrName(parText), "tag",
				parameterArgument);
		}

		if (type == ProgVariableTypes.ItemPrototype)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.ItemProtos.GetByIdOrName(parText),
				"item prototype", parameterArgument);
		}

		if (type == ProgVariableTypes.NPCTemplate)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.NpcTemplates.GetByIdOrName(parText), "NPC template",
				parameterArgument);
		}

		if (type == ProgVariableTypes.OutfitTemplate)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.OutfitTemplates.GetByIdOrName(parText),
				"outfit template", parameterArgument);
		}

		if (type == ProgVariableTypes.Vehicle)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.Vehicles.GetByIdOrName(parText), "vehicle",
				parameterArgument);
		}

		if (type == ProgVariableTypes.CelestialObject)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.CelestialObjects.GetByIdOrName(parText),
				"celestial object", parameterArgument);
		}

		if (type == ProgVariableTypes.Grid)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.Grids.GetByIdOrName(parText), "grid",
				parameterArgument);
		}

		if (type == ProgVariableTypes.CharacteristicDefinition)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.Characteristics.GetByIdOrName(parText),
				"characteristic definition", parameterArgument);
		}

		if (type == ProgVariableTypes.CharacteristicValue)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.CharacteristicValues.GetByIdOrName(parText),
				"characteristic value", parameterArgument);
		}

		if (type == ProgVariableTypes.AgricultureFieldProfile)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.AgricultureFieldProfiles.GetByIdOrName(parText),
				"agriculture field profile", parameterArgument);
		}

		if (type == ProgVariableTypes.AgricultureCropDefinition)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.AgricultureCropDefinitions.GetByIdOrName(parText),
				"agriculture crop definition", parameterArgument);
		}

		if (type == ProgVariableTypes.AgricultureHerdDefinition)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(parText),
				"agriculture herd definition", parameterArgument);
		}

		if (type == ProgVariableTypes.AgricultureWoodlandDefinition)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(parText),
				"agriculture woodland definition", parameterArgument);
		}

		if (type == ProgVariableTypes.AgricultureOperation)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.AgricultureOperations.GetByIdOrName(parText),
				"agriculture operation", parameterArgument);
		}

		if (type == ProgVariableTypes.Property)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.Properties.GetByIdOrName(parText), "property",
				parameterArgument);
		}

		if (type == ProgVariableTypes.PropertyKey)
		{
			return ResolvePropertyChildArgument(actor, parText, parameterArgument, "property key",
				PropertyReferenceLookup.GetPropertyKey);
		}

		if (type == ProgVariableTypes.PropertyLease)
		{
			return ResolvePropertyChildArgument(actor, parText, parameterArgument, "property lease",
				PropertyReferenceLookup.GetPropertyLease);
		}

		if (type == ProgVariableTypes.PropertyLeaseOrder)
		{
			return ResolvePropertyChildArgument(actor, parText, parameterArgument, "property lease order",
				PropertyReferenceLookup.GetPropertyLeaseOrder);
		}

		if (type == ProgVariableTypes.PropertySaleOrder)
		{
			return ResolvePropertyChildArgument(actor, parText, parameterArgument, "property sale order",
				PropertyReferenceLookup.GetPropertySaleOrder);
		}

		if (type == ProgVariableTypes.EconomicZone)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.EconomicZones.GetByIdOrName(parText),
				"economic zone", parameterArgument);
		}

		if (type == ProgVariableTypes.Channel)
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.Channels.GetByIdOrName(parText), "channel",
				parameterArgument);
		}

		if (type == ProgVariableTypes.NameCulture)
		{
			var nameCulture = actor.Gameworld.NameCultures.GetByIdOrName(parText);
			if (nameCulture is null)
			{
				actor.OutputHandler.Send($"There is no such name culture{parameterArgument}.");
				return (null, false);
			}

			return (nameCulture, true);
		}

		if (type == ProgVariableTypes.RandomNameProfile)
		{
			var randomNameProfile = actor.Gameworld.RandomNameProfiles.GetByIdOrName(parText);
			if (randomNameProfile is null)
			{
				actor.OutputHandler.Send($"There is no such random name profile{parameterArgument}.");
				return (null, false);
			}

			return (randomNameProfile, true);
		}

		if (type == ProgVariableTypes.PersonalName)
		{
			if (parText.EqualTo("null"))
			{
				return (null, true);
			}

			StringStack nameStack = new(parText);
			if (nameStack.IsFinished)
			{
				actor.OutputHandler.Send(
					$"You must specify a name culture followed by a complete name{parameterArgument}.");
				return (null, false);
			}

			var nameCulture = actor.Gameworld.NameCultures.GetByIdOrName(nameStack.PopSpeech());
			if (nameCulture is null)
			{
				actor.OutputHandler.Send($"There is no such name culture{parameterArgument}.");
				return (null, false);
			}

			if (nameStack.IsFinished)
			{
				actor.OutputHandler.Send(
					$"You must specify a complete name after the name culture{parameterArgument}.");
				return (null, false);
			}

			var personalName = nameCulture.GetPersonalName(nameStack.SafeRemainingArgument, true);
			if (personalName is null)
			{
				actor.OutputHandler.Send($"That is not a valid name for the {nameCulture.Name.ColourName()} name culture{parameterArgument}.");
				return (null, false);
			}

			return (personalName, true);
		}

		if (type == ProgVariableTypes.LegalClass)
		{
			var legalClass = actor.Gameworld.LegalClasses.GetByIdOrName(parText);
			if (legalClass is null)
			{
				actor.OutputHandler.Send($"There is no such legal class{parameterArgument}");
				return (null, false);
			}

			return (legalClass, true);
		}

		switch (type.LegacyCode)
		{
			case ProgVariableTypeCode.Boolean:
				if (bool.TryParse(parText, out bool bValue))
				{
					return (bValue, true);
				}

				actor.OutputHandler.Send($"That is not a valid boolean argument to use {parameterArgument}.");
				return (null, false);
			case ProgVariableTypeCode.Chargen:
				if (!long.TryParse(parText, out long id))
				{
					actor.OutputHandler.Send($"You must supply an ID number for the chargen you wish to use.");
					return (null, false);
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.Chargens.Find(id);
					if (dbitem is null)
					{
						actor.OutputHandler.Send("There is no such chargen.");
						return (null, false);
					}

					return (new CharacterCreation.Chargen(dbitem, actor.Gameworld, dbitem.Account), true);
				}
			case ProgVariableTypeCode.Character:
				var targetActor = parText.EqualTo("self") ? actor : actor.TargetActor(parText);
				if (targetActor == null)
				{
					actor.OutputHandler.Send($"You do not see anybody like that here to target{parameterArgument}.");
					return (null, false);
				}

				return (targetActor, true);
			case ProgVariableTypeCode.Gender:
				switch (parText.ToLowerInvariant())
				{
					case "male":
						return (Gender.Male, true);
					case "female":
						return (Gender.Female, true);
					case "neuter":
						return (Gender.Neuter, true);
					case "non-binary":
					case "nb":
					case "nonbinary":
						return (Gender.NonBinary, true);
					case "indeterminate":
						return (Gender.Indeterminate, true);
					default:
						actor.OutputHandler.Send($"That is not a valid gender to use{parameterArgument}.");
						return (null, false);
				}
			case ProgVariableTypeCode.TimeSpan:
				if (!TimeSpan.TryParse(parText, actor, out TimeSpan tsValue))
				{
					actor.OutputHandler.Send($"That is not a valid timespan to use{parameterArgument}.");
					return (null, false);
				}

				return (tsValue, true);
			case ProgVariableTypeCode.DateTime:
				if (!DateTime.TryParse(parText, actor, DateTimeStyles.None, out DateTime dtValue))
				{
					actor.OutputHandler.Send($"That is not a valid datetime to use{parameterArgument}.");
					return (null, false);
				}

				return (dtValue, true);
			case ProgVariableTypeCode.MudDateTime:
				if (!MudDateTime.TryParse(parText, actor.Gameworld, out MudDateTime mdtValue))
				{
					actor.OutputHandler.Send($"That is not a valid mud datetime to use{parameterArgument}.");
					return (null, false);
				}

				return (mdtValue, true);
			case ProgVariableTypeCode.Item:
				var targetItem = actor.TargetItem(parText);
				if (targetItem == null)
				{
					actor.OutputHandler.Send($"You do not see any item like that to use{parameterArgument}.");
					return (null, false);
				}

				return (targetItem, true);
			case ProgVariableTypeCode.Location:
				if (parText.Equals("here", StringComparison.InvariantCultureIgnoreCase))
				{
					return ResolveFrameworkItemArgument(actor, actor.Location, "location", parameterArgument);
				}

				if (long.TryParse(parText, out long iValue))
				{
					var targetCell = actor.Gameworld.Cells.Get(iValue);
					return ResolveFrameworkItemArgument(actor, targetCell, "location", parameterArgument);
				}
				else
				{
					var targetCell = RoomBuilderModule.LookupCell(actor.Gameworld, parText);
					if (targetCell == null)
					{
						actor.OutputHandler.Send(
							$"There is no such location{parameterArgument}.");
						return (null, false);
					}

					return ResolveFrameworkItemArgument(actor, targetCell, "location", parameterArgument);
				}

			case ProgVariableTypeCode.Number:
				if (decimal.TryParse(parText, NumberStyles.Number, actor, out decimal dValue))
				{
					return (dValue, true);
				}

				actor.OutputHandler.Send($"That is not a valid number argument to use{parameterArgument}.");
				return (null, false);
			case ProgVariableTypeCode.Shard:
				if (long.TryParse(parText, out iValue))
				{
					var targetShard = actor.Gameworld.Shards.Get(iValue);
					return ResolveFrameworkItemArgument(actor, targetShard, "shard", parameterArgument);
				}

				actor.OutputHandler.Send($"You must specify the ID number of the shard to use{parameterArgument}.");
				return (null, false);
			case ProgVariableTypeCode.Text:
				return (parText, true);
			case ProgVariableTypeCode.Zone:
				if (long.TryParse(parText, out iValue))
				{
					var targetZone = actor.Gameworld.Zones.Get(iValue);
					return ResolveFrameworkItemArgument(actor, targetZone, "zone", parameterArgument);
				}

				actor.OutputHandler.Send($"You must specify the ID number of the zone to use{parameterArgument}.");
				return (null, false);
			case ProgVariableTypeCode.Clan:
				var targetClan = long.TryParse(parText, out iValue)
					? actor.Gameworld.Clans.Get(iValue)
					: actor.Gameworld.Clans.FirstOrDefault(
						  x => x.FullName.Equals(parText, StringComparison.InvariantCultureIgnoreCase)) ??
					  actor.Gameworld.Clans.FirstOrDefault(
						  x => x.Alias.Equals(parText, StringComparison.InvariantCultureIgnoreCase));
				if (targetClan == null)
				{
					actor.Send($"There is no such clan{parameterArgument}.");
					return (null, false);
				}

				return (targetClan, true);
			case ProgVariableTypeCode.ClanRank:
				var rank = actor.Gameworld.Clans.SelectMany(x => x.Ranks).GetByIdOrName(parText);
				if (rank is null)
				{
					actor.OutputHandler.Send($"There is no such rank{parameterArgument}.");
					return (null, false);
				}

				return (rank, true);
			case ProgVariableTypeCode.ClanPaygrade:
				var paygrade = actor.Gameworld.Clans.SelectMany(x => x.Paygrades).GetByIdOrName(parText);
				if (paygrade is null)
				{
					actor.OutputHandler.Send($"There is no such paygrade{parameterArgument}.");
					return (null, false);
				}

				return (paygrade, true);
			case ProgVariableTypeCode.ClanAppointment:
				var appointment = actor.Gameworld.Clans.SelectMany(x => x.Appointments).GetByIdOrName(parText);
				if (appointment is null)
				{
					actor.OutputHandler.Send($"There is no such appointment{parameterArgument}.");
					return (null, false);
				}

				return (appointment, true);
			case ProgVariableTypeCode.Currency:
				var targetCurrency = long.TryParse(parText, out iValue)
					? actor.Gameworld.Currencies.Get(iValue)
					: actor.Gameworld.Currencies.FirstOrDefault(
						x => x.Name.Equals(parText, StringComparison.InvariantCultureIgnoreCase));
				if (targetCurrency == null)
				{
					actor.Send($"There is no such currency{parameterArgument}.");
					return (null, false);
				}

				return (targetCurrency, true);
			case ProgVariableTypeCode.Language:
				var targetLanguages = long.TryParse(parText, out iValue)
					? actor.Gameworld.Languages.Get(iValue)
					: actor.Gameworld.Languages.GetByName(parText);
				if (targetLanguages == null)
				{
					actor.Send($"There is no such language{parameterArgument}.");
					return (null, false);
				}

				return (targetLanguages, true);
			case ProgVariableTypeCode.Script:
				var targetScripts = actor.Gameworld.Scripts.GetByIdOrName(parText);
				if (targetScripts is null)
				{
					actor.OutputHandler.Send($"There is no such script{parameterArgument}.");
					return (null, false);
				}

				return (targetScripts, true);
			case ProgVariableTypeCode.Accent:
				var targetAccent = long.TryParse(parText, out iValue)
					? actor.Gameworld.Accents.Get(iValue)
					: actor.Gameworld.Accents.GetByName(parText);
				if (targetAccent == null)
				{
					actor.Send($"There is no such accent{parameterArgument}.");
					return (null, false);
				}

				return (targetAccent, true);
			case ProgVariableTypeCode.Exit:
				var exit = actor.Location?.GetExitKeyword(parText, actor);
				if (exit == null)
				{
					actor.OutputHandler.Send($"There is no such exit here{parameterArgument}.");
					return (null, false);
				}

				return (exit, true);
			case ProgVariableTypeCode.Trait:
				var trait = actor.Gameworld.Traits.GetByIdOrName(parText);
				if (trait is null)
				{
					actor.OutputHandler.Send($"There is no such trait{parameterArgument}.");
					return (null, false);
				}

				return (trait, true);
			case ProgVariableTypeCode.Race:
				var race = actor.Gameworld.Races.GetByIdOrName(parText);
				if (race is null)
				{
					actor.OutputHandler.Send($"There is no such race{parameterArgument}");
					return (null, false);
				}

				return (race, true);
			case ProgVariableTypeCode.Culture:
				var culture = actor.Gameworld.Cultures.GetByIdOrName(parText);
				if (culture is null)
				{
					actor.OutputHandler.Send($"There is no such culture{parameterArgument}");
					return (null, false);
				}

				return (culture, true);
			case ProgVariableTypeCode.Ethnicity:
				var ethnicity = actor.Gameworld.Ethnicities.GetByIdOrName(parText);
				if (ethnicity is null)
				{
					actor.OutputHandler.Send($"There is no such ethnicity{parameterArgument}");
					return (null, false);
				}

				return (ethnicity, true);
			case ProgVariableTypeCode.Merit:
				var merit = actor.Gameworld.Merits.GetByIdOrName(parText);
				if (merit is null)
				{
					actor.OutputHandler.Send($"There is no such merit{parameterArgument}");
					return (null, false);
				}

				return (merit, true);
			case ProgVariableTypeCode.Calendar:
				var calendar = actor.Gameworld.Calendars.GetByIdOrNames(parText);
				if (calendar is null)
				{
					actor.OutputHandler.Send($"There is no such calendar{parameterArgument}");
					return (null, false);
				}

				return (calendar, true);
			case ProgVariableTypeCode.Clock:
				var clock = actor.Gameworld.Clocks.GetByIdOrNames(parText);
				if (clock is null)
				{
					actor.OutputHandler.Send($"There is no such clock{parameterArgument}");
					return (null, false);
				}

				return (clock, true);
			case ProgVariableTypeCode.Knowledge:
				var knowledge = actor.Gameworld.Knowledges.GetByIdOrName(parText);
				if (knowledge is null)
				{
					actor.OutputHandler.Send($"There is no such knowledge{parameterArgument}");
					return (null, false);
				}

				return (knowledge, true);
			case ProgVariableTypeCode.Role:
				var role = actor.Gameworld.Roles.GetByIdOrName(parText);
				if (role is null)
				{
					actor.OutputHandler.Send($"There is no such role{parameterArgument}");
					return (null, false);
				}

				return (role, true);
			case ProgVariableTypeCode.Drug:
				var drug = actor.Gameworld.Drugs.GetByIdOrName(parText);
				if (drug is null)
				{
					actor.OutputHandler.Send($"There is no such drug{parameterArgument}");
					return (null, false);
				}

				return (drug, true);
			case ProgVariableTypeCode.WeatherEvent:
				var weather = actor.Gameworld.WeatherEvents.GetByIdOrName(parText);
				if (weather is null)
				{
					actor.OutputHandler.Send($"There is no such weather event{parameterArgument}");
					return (null, false);
				}

				return (weather, true);
			case ProgVariableTypeCode.Shop:
				var shop = actor.Gameworld.Shops.GetByIdOrName(parText);
				if (shop is null)
				{
					actor.OutputHandler.Send($"There is no such shop{parameterArgument}");
					return (null, false);
				}

				return (shop, true);
			case ProgVariableTypeCode.Merchandise:
				var merch = actor.Gameworld.Shops.SelectMany(x => x.Merchandises).GetByIdOrName(parText);
				if (merch is null)
				{
					actor.OutputHandler.Send($"There is no such merchandise{parameterArgument}");
					return (null, false);
				}

				return (merch, true);
			case ProgVariableTypeCode.OverlayPackage:
				var overlay = actor.Gameworld.CellOverlayPackages.GetByIdOrName(parText);
				if (overlay is null)
				{
					actor.OutputHandler.Send($"There is no such overlay package{parameterArgument}");
					return (null, false);
				}

				return (overlay, true);
			case ProgVariableTypeCode.Terrain:
				var terrain = actor.Gameworld.Terrains.GetByIdOrName(parText);
				if (terrain is null)
				{
					actor.OutputHandler.Send($"There is no such terrain{parameterArgument}");
					return (null, false);
				}

				return (terrain, true);
			case ProgVariableTypeCode.Solid:
				var material = actor.Gameworld.Materials.GetByIdOrName(parText);
				if (material is null)
				{
					actor.OutputHandler.Send($"There is no such material{parameterArgument}");
					return (null, false);
				}

				return (material, true);
			case ProgVariableTypeCode.Liquid:
				var liquid = actor.Gameworld.Liquids.GetByIdOrName(parText);
				if (liquid is null)
				{
					actor.OutputHandler.Send($"There is no such liquid{parameterArgument}");
					return (null, false);
				}

				return (liquid, true);
			case ProgVariableTypeCode.Gas:
				var gas = actor.Gameworld.Gases.GetByIdOrName(parText);
				if (gas is null)
				{
					actor.OutputHandler.Send($"There is no such gas{parameterArgument}");
					return (null, false);
				}

				return (gas, true);
			case ProgVariableTypeCode.LegalAuthority:
				var legal = actor.Gameworld.LegalAuthorities.GetByIdOrName(parText);
				if (legal is null)
				{
					actor.OutputHandler.Send($"There is no such legal authority{parameterArgument}");
					return (null, false);
				}

				return (legal, true);
			case ProgVariableTypeCode.MagicCapability:
				var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(parText);
				if (capability is null)
				{
					actor.OutputHandler.Send($"There is no such magic capability{parameterArgument}");
					return (null, false);
				}

				return (capability, true);
			case ProgVariableTypeCode.MagicSchool:
				var school = actor.Gameworld.MagicSchools.GetByIdOrName(parText);
				if (school is null)
				{
					actor.OutputHandler.Send($"There is no such magic school{parameterArgument}");
					return (null, false);
				}

				return (school, true);
			case ProgVariableTypeCode.MagicSpell:
				var spell = actor.Gameworld.MagicSpells.GetByIdOrName(parText);
				if (spell is null)
				{
					actor.OutputHandler.Send($"There is no such magic spell{parameterArgument}");
					return (null, false);
				}

				return (spell, true);
			case ProgVariableTypeCode.Bank:
				var bank = actor.Gameworld.Banks.GetByIdOrName(parText);
				if (bank is null)
				{
					actor.OutputHandler.Send($"There is no such bank{parameterArgument}");
					return (null, false);
				}

				return (bank, true);
			case ProgVariableTypeCode.BankAccountType:
				var accountType = actor.Gameworld.BankAccountTypes.GetByIdOrName(parText);
				if (accountType is null)
				{
					actor.OutputHandler.Send($"There is no such bank account type{parameterArgument}");
					return (null, false);
				}

				return (accountType, true);
			case ProgVariableTypeCode.BankAccount:
				return ResolveBankAccountArgument(parText, actor);
			case ProgVariableTypeCode.Project:
				var project = actor.Gameworld.ActiveProjects.GetByIdOrName(parText);
				if (project is null)
				{
					actor.OutputHandler.Send($"There is no such project{parameterArgument}");
					return (null, false);
				}

				return (project, true);
			case ProgVariableTypeCode.Law:
				var law = actor.Gameworld.Laws.GetByIdOrName(parText);
				if (law is null)
				{
					actor.OutputHandler.Send($"There is no such law{parameterArgument}");
					return (null, false);
				}

				return (law, true);
			case ProgVariableTypeCode.Market:
				var market = actor.Gameworld.Markets.GetByIdOrName(parText);
				if (market is null)
				{
					actor.OutputHandler.Send($"There is no such market{parameterArgument}");
					return (null, false);
				}

				return (market, true);
			case ProgVariableTypeCode.MarketCategory:
				var category = actor.Gameworld.MarketCategories.GetByIdOrName(parText);
				if (category is null)
				{
					actor.OutputHandler.Send($"There is no such market category{parameterArgument}");
					return (null, false);
				}

				return (category, true);
			case ProgVariableTypeCode.Crime:
				var crime = actor.Gameworld.Crimes.GetByIdOrName(parText);
				if (crime is null)
				{
					actor.OutputHandler.Send($"There is no such crime{parameterArgument}");
					return (null, false);
				}
				return (crime, true);
			case ProgVariableTypeCode.Area:
				var area = actor.Gameworld.Areas.GetByIdOrName(parText);
				if (area is null)
				{
					actor.OutputHandler.Send($"There is no such area{parameterArgument}");
					return (null, false);
				}

				return (area, true);
			case ProgVariableTypeCode.Writing:
				if (!long.TryParse(parText, out iValue))
				{
					actor.OutputHandler.Send($"The text is not a valid id{parameterArgument}");
					return (null, false);
				}

				var writing = actor.Gameworld.Writings.Get(iValue);
				if (writing is null)
				{
					actor.OutputHandler.Send($"There is no such writing{parameterArgument}");
					return (null, false);
				}

				return (writing, true);
			default:
				actor.Send(
					$"The variable type {type.Describe().Colour(Telnet.VariableCyan)} is not yet supported in this command. Sorry.");
				return (null, false);
		}
	}

	private static (object? result, bool success) ResolvePropertyChildArgument(ICharacter actor, string text,
		string parameterArgument, string typeName, Func<IFuturemud, long, IFrameworkItem?> resolver)
	{
		if (!long.TryParse(text, out long id))
		{
			actor.OutputHandler.Send($"You must enter a numeric ID for the {typeName}{parameterArgument}.");
			return (null, false);
		}

		return ResolveFrameworkItemArgument(actor, resolver(actor.Gameworld, id), typeName, parameterArgument);
	}

	private static (object? result, bool success) ResolveFrameworkItemArgument(ICharacter actor, IFrameworkItem? item,
		string typeName, string parameterArgument)
	{
		if (item is not null)
		{
			return (item, true);
		}

		actor.OutputHandler.Send($"There is no such {typeName}{parameterArgument}.");
		return (null, false);
	}

}
