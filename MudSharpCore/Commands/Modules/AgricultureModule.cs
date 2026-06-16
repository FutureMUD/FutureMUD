using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.ConcreteTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Commands.Modules;

internal class AgricultureModule : Module<ICharacter>
{
	private AgricultureModule() : base("Agriculture")
	{
		IsNecessary = true;
	}

	public static AgricultureModule Instance { get; } = new();

	private const string FieldHelp = @"Syntax:
	#3field#0
	#3field look#0
	#3field start <operation> [target]#0
	#3field harvest#0
	#3field herd draw <herd> [count]#0
	#3field herd absorb <npc> <herd>#0
	#3field herd drive <herd> <direction> [count]#0

Admin Syntax:
	#3field create [profile]#0
	#3field delete#0
	#3field set <score> <0-100>#0
	#3field reset#0
	#3field tick#0
	#3field profiles|crops|herds|woodlands|operations#0
	#3field scoretype list|set|disable ...#0
	#3field profile show|create|set|delete ...#0
	#3field crop show|create|set|delete ...#0
	#3field herds show|create|set|delete ...#0
	#3field woodland show|create|set|delete ...#0
	#3field operation show|create|set|delete ...#0";

	[PlayerCommand("Field", "field")]
	[HelpInfo("field", FieldHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Field(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			FieldLook(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "look":
			case "show":
				FieldLook(actor);
				return;
			case "start":
			case "begin":
				FieldStart(actor, ss);
				return;
			case "harvest":
				FieldHarvest(actor);
				return;
			case "herd":
				FieldHerd(actor, ss);
				return;
			case "create":
				FieldCreate(actor, ss);
				return;
			case "delete":
			case "remove":
				FieldDelete(actor);
				return;
			case "set":
				FieldSet(actor, ss);
				return;
			case "reset":
				FieldReset(actor);
				return;
			case "tick":
				FieldTick(actor);
				return;
			case "profiles":
			case "profile":
				FieldProfiles(actor, ss);
				return;
			case "scoretypes":
			case "scoretype":
			case "scores":
				FieldScoreTypes(actor, ss);
				return;
			case "crops":
			case "crop":
				FieldCrops(actor, ss);
				return;
			case "herds":
			case "herddefs":
			case "herddef":
				FieldHerds(actor, ss);
				return;
			case "woodlands":
			case "woodland":
				FieldWoodlands(actor, ss);
				return;
			case "operations":
			case "operation":
				FieldOperations(actor, ss);
				return;
		}

		actor.OutputHandler.Send(FieldHelp.SubstituteANSIColour());
	}

	private static void FieldLook(ICharacter actor)
	{
		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		actor.OutputHandler.Send(field.DescribeTo(actor, actor.IsAdministrator()));
	}

	private static void FieldCreate(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can create fields.");
			return;
		}

		if (actor.Location.AgricultureField != null)
		{
			actor.OutputHandler.Send("There is already an agriculture field in this location.");
			return;
		}

		var profile = ss.IsFinished
			? actor.Location.Terrain(actor).DefaultAgricultureFieldProfile
			: actor.Gameworld.AgricultureFieldProfiles.GetByIdOrName(ss.SafeRemainingArgument);
		if (profile == null)
		{
			actor.OutputHandler.Send("You must specify an agriculture field profile, or set a terrain default first.");
			return;
		}

		var field = new AgricultureField(actor.Location, profile);
		actor.Gameworld.Add(field);
		actor.OutputHandler.Send($"You create a {profile.Name.ColourName()} agriculture field in this location.");
	}

	private static void FieldDelete(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can delete fields.");
			return;
		}

		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.AgricultureFields.Find(field.Id);
			if (dbitem != null)
			{
				FMDB.Context.AgricultureFields.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		actor.Gameworld.SaveManager.Abort(field);
		actor.Gameworld.Destroy(field);
		actor.OutputHandler.Send("You delete the agriculture field in this location.");
	}

	private static void FieldSet(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can set field scores directly.");
			return;
		}

		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which field score do you want to set?");
			return;
		}

		if (!AgricultureScoreTypeExtensions.TryParseScoreType(ss.PopSpeech(), actor.Gameworld, out var score))
		{
			actor.OutputHandler.Send($"That is not a valid field score. Valid scores are {ValidScoreTypesText(actor)}.");
			return;
		}

		if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var value) || value < 0 || value > 100)
		{
			actor.OutputHandler.Send("You must specify a score from 0 to 100.");
			return;
		}

		field.SetScore(score, value);
		field.Changed = true;
		actor.OutputHandler.Send($"You set the field's {score.DescribeFor(actor.Gameworld).ColourName()} score to {value.ToStringN0Colour(actor)}.");
	}

	private static void FieldReset(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can reset field scores.");
			return;
		}

		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		foreach (var score in AgricultureScoreTypeExtensions.ActiveScoreTypes(actor.Gameworld))
		{
			field.SetScore(score, field.Profile.DefaultScores.TryGetValue(score, out var value) ? value : 50);
		}

		field.Changed = true;
		actor.OutputHandler.Send($"You reset the field scores to the {field.Profile.Name.ColourName()} profile defaults.");
	}

	private static void FieldTick(ICharacter actor)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can force a field tick.");
			return;
		}

		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		field.DailyTick();
		actor.OutputHandler.Send("You force an agriculture tick for this field.");
	}

	private static void FieldStart(ICharacter actor, StringStack ss)
	{
		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which agriculture operation do you want to start?");
			return;
		}

		var operation = actor.Gameworld.AgricultureOperations.GetByIdOrName(ss.PopSpeech());
		if (operation == null)
		{
			actor.OutputHandler.Send("There is no such agriculture operation.");
			return;
		}

		var target = ResolveOperationTarget(actor, operation, ss);
		if (operation.TargetType != AgricultureTargetType.None && target == null)
		{
			return;
		}

		StartOperation(actor, field, operation, target);
	}

	private static void FieldHarvest(ICharacter actor)
	{
		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		var operation = actor.Gameworld.AgricultureOperations
		                     .Where(x => x.OperationType is AgricultureOperationType.Harvest or AgricultureOperationType.HarvestApiary)
		                     .OrderBy(x => x.OperationType == AgricultureOperationType.Harvest ? 0 : 1)
		                     .FirstOrDefault(x => field.CanBeginOperation(actor, x, null, out _));
		if (operation == null)
		{
			actor.OutputHandler.Send("There is no harvest operation available for this field.");
			return;
		}

		StartOperation(actor, field, operation, null);
	}

	private static IFrameworkItem ResolveOperationTarget(ICharacter actor, IAgricultureOperation operation, StringStack ss)
	{
		if (operation.TargetType == AgricultureTargetType.None)
		{
			return null;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which {operation.TargetType.DescribeEnum().ToLowerInvariant()} should this operation target?");
			return null;
		}

		var targetText = ss.SafeRemainingArgument;
		IFrameworkItem target = operation.TargetType switch
		{
			AgricultureTargetType.Crop => actor.Gameworld.AgricultureCropDefinitions.GetByIdOrName(targetText),
			AgricultureTargetType.Herd => actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(targetText),
			AgricultureTargetType.Woodland => actor.Gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(targetText),
			_ => null
		};
		if (target == null)
		{
			actor.OutputHandler.Send($"There is no such {operation.TargetType.DescribeEnum().ToLowerInvariant()} definition.");
		}

		return target;
	}

	private static void StartOperation(ICharacter actor, IAgricultureField field, IAgricultureOperation operation, IFrameworkItem target)
	{
		if (!field.CanBeginOperation(actor, operation, target, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		if (operation.Project == null)
		{
			actor.OutputHandler.Send("That agriculture operation has no project template configured.");
			return;
		}

		if (!operation.Project.CanInitiateProject(actor))
		{
			actor.OutputHandler.Send(operation.Project.WhyCannotInitiateProject(actor));
			return;
		}

		var active = new ActiveLocalProject(operation.Project, actor);
		actor.Gameworld.Add(active);
		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var context = new MudSharp.Models.AgricultureProjectContext
			{
				ActiveProjectId = active.Id,
				AgricultureFieldId = field.Id,
				OperationId = operation.Id,
				TargetType = (int)operation.TargetType,
				TargetId = target?.Id,
				TargetText = target?.Name ?? string.Empty,
				ActorId = CharacterInstanceIdentityComparer.IdentityId(actor),
				Definition = "<Context />"
			};
			FMDB.Context.AgricultureProjectContexts.Add(context);
			FMDB.Context.SaveChanges();
		}

		actor.OutputHandler.Send($"You start the {operation.Project.Name.ColourName()} project for {operation.Name.ColourName()}.");
	}

	private static void FieldHerd(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to draw animals out of a herd, absorb an NPC into one, or drive a herd to another field?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "draw":
			case "drawdown":
				FieldHerdDraw(actor, ss);
				return;
			case "absorb":
				FieldHerdAbsorb(actor, ss);
				return;
			case "drive":
			case "move":
				FieldHerdDrive(actor, ss);
				return;
		}

		actor.OutputHandler.Send("Do you want to draw animals out of a herd, absorb an NPC into one, or drive a herd to another field?");
	}

	private static void FieldHerdDraw(ICharacter actor, StringStack ss)
	{
		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		if (!CanWorkField(actor, field, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which herd do you want to draw animals from?");
			return;
		}

		var herd = actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(ss.PopSpeech());
		if (herd == null)
		{
			actor.OutputHandler.Send("There is no such herd definition.");
			return;
		}

		var count = 1;
		if (!ss.IsFinished && (!int.TryParse(ss.PopSpeech(), out count) || count <= 0))
		{
			actor.OutputHandler.Send("The count must be a positive number.");
			return;
		}

		if (field.DrawDownHerd(herd, count, actor, out var result))
		{
			actor.OutputHandler.Send(result);
			return;
		}

		actor.OutputHandler.Send(result);
	}

	private static void FieldHerdAbsorb(ICharacter actor, StringStack ss)
	{
		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		if (!CanWorkField(actor, field, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which NPC do you want to absorb into the abstract herd?");
			return;
		}

		var npc = actor.TargetActor(ss.PopSpeech());
		if (npc == null)
		{
			actor.OutputHandler.Send("You do not see any such NPC here.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which herd definition should this NPC be absorbed into?");
			return;
		}

		var herd = actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(ss.SafeRemainingArgument);
		if (herd == null)
		{
			actor.OutputHandler.Send("There is no such herd definition.");
			return;
		}

		if (field.AbsorbNpcIntoHerd(npc, herd, actor, out var result))
		{
			actor.OutputHandler.Send(result);
			return;
		}

		actor.OutputHandler.Send(result);
	}

	private static void FieldHerdDrive(ICharacter actor, StringStack ss)
	{
		var field = actor.Location.AgricultureField;
		if (field == null)
		{
			actor.OutputHandler.Send("There is no agriculture field here.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which herd do you want to drive to another field?");
			return;
		}

		var herd = actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(ss.PopSpeech());
		if (herd == null)
		{
			actor.OutputHandler.Send("There is no such herd definition.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which direction do you want to drive the herd?");
			return;
		}

		var exit = actor.Location.GetExitKeyword(ss.PopSpeech(), actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no exit in that direction.");
			return;
		}

		var destination = exit.Destination.AgricultureField;
		if (destination == null)
		{
			actor.OutputHandler.Send("There is no agriculture field in that direction.");
			return;
		}

		var (canCross, failureOutput) = actor.CanCross(exit);
		if (!canCross)
		{
			if (failureOutput != null)
			{
				actor.OutputHandler.Send(failureOutput);
			}
			else
			{
				actor.OutputHandler.Send("You cannot drive the herd through that exit.");
			}

			return;
		}

		var count = 0;
		if (!ss.IsFinished)
		{
			var countText = ss.PopSpeech();
			if (!countText.EqualTo("all") && (!int.TryParse(countText, out count) || count <= 0))
			{
				actor.OutputHandler.Send("The count must be a positive number, or all.");
				return;
			}
		}

		if (!ss.IsFinished)
		{
			actor.OutputHandler.Send("Syntax: field herd drive <herd> <direction> [count]");
			return;
		}

		field.DriveHerdTo(destination, herd, count, actor, out var result);
		actor.OutputHandler.Send(result);
	}

	private static bool CanWorkField(ICharacter actor, IAgricultureField field, out string reason)
	{
		var property = actor.Gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(field.Cell));
		if (property != null && !actor.IsAdministrator() && !property.IsAuthorisedOwner(actor) && !property.IsAuthorisedLeaseHolder(actor))
		{
			reason = "You are not authorised to work this property.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static bool RequireAdministrator(ICharacter actor)
	{
		if (actor.IsAdministrator())
		{
			return true;
		}

		actor.OutputHandler.Send("Only administrators can edit agriculture definitions.");
		return false;
	}

	private static bool TryParseOnOff(string text, out bool value)
	{
		switch (text.ToLowerInvariant())
		{
			case "true":
			case "yes":
			case "on":
			case "allow":
			case "allowed":
				value = true;
				return true;
			case "false":
			case "no":
			case "off":
			case "disallow":
			case "forbid":
			case "forbidden":
				value = false;
				return true;
			default:
				value = false;
				return false;
		}
	}

	private static string ValidScoreTypesText(ICharacter actor)
	{
		return AgricultureScoreTypeExtensions.ActiveScoreTypes(actor.Gameworld)
		                                    .Select(x => x.DescribeFor(actor.Gameworld).ColourName())
		                                    .ListToString();
	}

	private static void FieldScoreTypes(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			ListScoreTypes(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
			case "show":
				ListScoreTypes(actor);
				return;
			case "set":
				SetScoreType(actor, ss);
				return;
			case "disable":
			case "remove":
				DisableScoreType(actor, ss);
				return;
		}

		actor.OutputHandler.Send("Syntax: field scoretype list|set|disable");
	}

	private static void ListScoreTypes(ICharacter actor)
	{
		var definitions = AgricultureScoreTypeExtensions.GetCustomScoreConfiguration(actor.Gameworld);
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			AgricultureScoreTypeExtensions.CustomScores.Select(x =>
			{
				var definition = definitions[x];
				return new[]
				{
					x.DescribeEnum(true),
					definition.Enabled.ToColouredString(),
					definition.Name,
					definition.HigherIsGood ? "higher is beneficial" : "higher is harmful"
				};
			}),
			new[] { "Slot", "Enabled", "Name", "Direction" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void SetScoreType(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		if (ss.IsFinished || !AgricultureScoreTypeExtensions.TryParseCustomSlot(ss.PopSpeech(), out var score))
		{
			actor.OutputHandler.Send("Which custom score slot do you want to enable? Use custom1 through custom12.");
			return;
		}

		if (ss.IsFinished || !TryParseScoreDirection(ss.PopSpeech(), out var higherIsGood))
		{
			actor.OutputHandler.Send("Should higher values be good or bad? Use good or bad.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name should this custom agriculture score use?");
			return;
		}

		var definitions = AgricultureScoreTypeExtensions.GetCustomScoreConfiguration(actor.Gameworld).ToDictionary(x => x.Key, x => x.Value);
		var name = ss.SafeRemainingArgument;
		definitions[score] = new AgricultureCustomScoreDefinition(score, true, name, higherIsGood);
		SaveCustomScoreDefinitions(actor, definitions);
		actor.OutputHandler.Send($"You enable {score.DescribeEnum(true).ColourName()} as {name.ColourName()} where higher values are {(higherIsGood ? "beneficial" : "harmful").ColourValue()}.");
	}

	private static void DisableScoreType(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		if (ss.IsFinished || !AgricultureScoreTypeExtensions.TryParseCustomSlot(ss.PopSpeech(), out var score))
		{
			actor.OutputHandler.Send("Which custom score slot do you want to disable? Use custom1 through custom12.");
			return;
		}

		var definitions = AgricultureScoreTypeExtensions.GetCustomScoreConfiguration(actor.Gameworld).ToDictionary(x => x.Key, x => x.Value);
		var current = definitions[score];
		definitions[score] = current with { Enabled = false };
		SaveCustomScoreDefinitions(actor, definitions);
		actor.OutputHandler.Send($"You disable {score.DescribeEnum(true).ColourName()}.");
	}

	private static bool TryParseScoreDirection(string text, out bool higherIsGood)
	{
		switch (text.ToLowerInvariant())
		{
			case "good":
			case "beneficial":
			case "benefit":
			case "highergood":
			case "higherisgood":
				higherIsGood = true;
				return true;
			case "bad":
			case "harmful":
			case "harm":
			case "higherbad":
			case "higherisbad":
				higherIsGood = false;
				return true;
			default:
				higherIsGood = true;
				return false;
		}
	}

	private static void SaveCustomScoreDefinitions(ICharacter actor,
		IReadOnlyDictionary<AgricultureScoreType, AgricultureCustomScoreDefinition> definitions)
	{
		var text = AgricultureScoreTypeExtensions.SaveCustomScoreConfiguration(definitions).ToString();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.StaticConfigurations.Find(AgricultureScoreTypeExtensions.CustomScoreConfigurationStaticConfiguration);
			if (dbitem == null)
			{
				FMDB.Context.StaticConfigurations.Add(new MudSharp.Models.StaticConfiguration
				{
					SettingName = AgricultureScoreTypeExtensions.CustomScoreConfigurationStaticConfiguration,
					Definition = text
				});
			}
			else
			{
				dbitem.Definition = text;
			}

			FMDB.Context.SaveChanges();
		}

		actor.Gameworld.UpdateStaticConfiguration(AgricultureScoreTypeExtensions.CustomScoreConfigurationStaticConfiguration, text);
	}

	private static void FieldProfiles(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			ListProfiles(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ListProfiles(actor);
				return;
			case "show":
				ShowProfile(actor, ss);
				return;
			case "create":
			case "new":
				CreateProfile(actor, ss);
				return;
			case "set":
				SetProfile(actor, ss);
				return;
			case "delete":
			case "remove":
				DeleteProfile(actor, ss);
				return;
		}

		actor.OutputHandler.Send("Syntax: field profile list|show|create|set|delete");
	}

	private static void ShowProfile(ICharacter actor, StringStack ss)
	{
		var profile = ss.IsFinished ? null : actor.Gameworld.AgricultureFieldProfiles.GetByIdOrName(ss.SafeRemainingArgument);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such agriculture field profile.");
			return;
		}

		actor.OutputHandler.Send($@"{profile.Name.ColourName()} (#{profile.Id.ToString("N0", actor)})
{profile.Description}
Allowed Uses: {Enum.GetValues(typeof(AgricultureFieldUse)).OfType<AgricultureFieldUse>().Where(profile.AllowsUse).Select(x => x.DescribeEnum().ColourName()).ListToString()}
Scores: {profile.DefaultScores.Where(x => x.Key.IsEnabledScore(actor.Gameworld)).OrderBy(x => x.Key).Select(x => $"{x.Key.DescribeFor(actor.Gameworld)} {x.Value.ToString("N0", actor)}").ListToString()}");
	}

	private static void CreateProfile(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should this agriculture field profile be called?");
			return;
		}

		var scores = AgricultureScoreTypeExtensions.ActiveScoreTypes(actor.Gameworld)
		                 .ToDictionary(x => x, _ => 50);
		var uses = Enum.GetValues(typeof(AgricultureFieldUse)).OfType<AgricultureFieldUse>();
		var profile = new AgricultureFieldProfile(actor.Gameworld, ss.SafeRemainingArgument, "A custom agriculture field profile.", scores, uses);
		actor.Gameworld.Add(profile);
		actor.OutputHandler.Send($"You create agriculture field profile {profile.Name.ColourName()}.");
	}

	private static void SetProfile(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var profile = ss.IsFinished ? null : actor.Gameworld.AgricultureFieldProfiles.GetByIdOrName(ss.PopSpeech());
		if (profile is not AgricultureFieldProfile concrete)
		{
			actor.OutputHandler.Send("There is no such agriculture field profile.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set name, description, score, or use?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "name":
				concrete.BuildingSetName(ss.SafeRemainingArgument);
				actor.OutputHandler.Send($"You rename the field profile to {concrete.Name.ColourName()}.");
				return;
			case "description":
			case "desc":
				concrete.BuildingSetDescription(ss.SafeRemainingArgument);
				actor.OutputHandler.Send("You update the field profile description.");
				return;
			case "score":
				if (ss.IsFinished || !AgricultureScoreTypeExtensions.TryParseScoreType(ss.PopSpeech(), actor.Gameworld, out var score))
				{
					actor.OutputHandler.Send($"Which agriculture score should be changed? Valid scores are {ValidScoreTypesText(actor)}.");
					return;
				}

				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var value) || value < 0 || value > 100)
				{
					actor.OutputHandler.Send("You must specify a value from 0 to 100.");
					return;
				}

				concrete.BuildingSetDefaultScore(score, value);
				actor.OutputHandler.Send($"You set the default {score.DescribeFor(actor.Gameworld).ColourName()} score to {value.ToStringN0Colour(actor)}.");
				return;
			case "use":
				if (ss.IsFinished || !Enum.TryParse<AgricultureFieldUse>(ss.PopSpeech(), true, out var use))
				{
					actor.OutputHandler.Send("Which field use should be allowed or forbidden?");
					return;
				}

				if (ss.IsFinished || !TryParseOnOff(ss.PopSpeech(), out var allowed))
				{
					actor.OutputHandler.Send("Should that use be on or off?");
					return;
				}

				concrete.BuildingSetAllowedUse(use, allowed);
				actor.OutputHandler.Send($"{use.DescribeEnum().ColourName()} is now allowed: {allowed.ToColouredString()}.");
				return;
		}

		actor.OutputHandler.Send("Do you want to set name, description, score, or use?");
	}

	private static void DeleteProfile(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var profile = ss.IsFinished ? null : actor.Gameworld.AgricultureFieldProfiles.GetByIdOrName(ss.SafeRemainingArgument);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such agriculture field profile.");
			return;
		}

		if (actor.Gameworld.AgricultureFields.Any(x => x.Profile?.Id == profile.Id))
		{
			actor.OutputHandler.Send("You cannot delete a profile while fields are using it.");
			return;
		}

		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.AgricultureFieldProfiles.Find(profile.Id);
			if (dbitem != null)
			{
				FMDB.Context.AgricultureFieldProfiles.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		if (profile is AgricultureFieldProfile concrete)
		{
			actor.Gameworld.SaveManager.Abort(concrete);
		}

		actor.Gameworld.Destroy(profile);
		actor.OutputHandler.Send("You delete that agriculture field profile.");
	}

	private static void FieldCrops(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			ListCrops(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ListCrops(actor);
				return;
			case "show":
				ShowCrop(actor, ss);
				return;
			case "create":
			case "new":
				CreateCrop(actor, ss);
				return;
			case "set":
				SetCrop(actor, ss);
				return;
			case "delete":
			case "remove":
				DeleteCrop(actor, ss);
				return;
		}

		actor.OutputHandler.Send("Syntax: field crop list|show|create|set|delete");
	}

	private static void ShowCrop(ICharacter actor, StringStack ss)
	{
		var crop = ss.IsFinished ? null : actor.Gameworld.AgricultureCropDefinitions.GetByIdOrName(ss.SafeRemainingArgument);
		if (crop == null)
		{
			actor.OutputHandler.Send("There is no such agriculture crop definition.");
			return;
		}

		actor.OutputHandler.Send($@"{crop.Name.ColourName()} (#{crop.Id.ToString("N0", actor)})
{crop.Description}
Category: {crop.Category.ColourValue()}
Growth Days: {crop.BaseGrowthDays.ToString("N0", actor).ColourValue()}
Perennial: {crop.IsPerennial.ToColouredString()}
Harvest Cycle Days: {crop.HarvestCycleDays.ToString("N0", actor).ColourValue()}
Harvest Window: {crop.HarvestWindowDays.ToString("N0", actor).ColourValue()}
Planting: {DescribePlantingWindows(crop.PlantingWindows, actor)}
Moisture: {crop.MinimumMoisture.ToString("N0", actor).ColourValue()} to {crop.MaximumMoisture.ToString("N0", actor).ColourValue()}
Temperature: {crop.MinimumTemperature.ToString("N0", actor).ColourValue()} to {crop.MaximumTemperature.ToString("N0", actor).ColourValue()} C
Pollination: {DescribePollination(crop, actor)}
Score Ranges: {DescribeScoreRanges(crop.ScoreRanges, actor)}
Seeds: {DescribeCommodityOutputs(crop.SeedRequirements, actor)}
Outputs: {DescribeCommodityOutputs(crop.YieldOutputs, actor)}");
	}

	private static string DescribePollination(IAgricultureCropDefinition crop, ICharacter actor)
	{
		if (crop.PollinationDependency == AgriculturePollinationDependency.None)
		{
			return "None".ColourError();
		}

		return $"{crop.PollinationDependency.DescribeEnum().ColourName()} (+{crop.PollinationHealthBonus.ToString("N0", actor).ColourValue()} health, +{crop.PollinationYieldBonus.ToString("N0", actor).ColourValue()} yield)";
	}

	private static string DescribePlantingWindows(IReadOnlyCollection<AgriculturePlantingWindow> windows, ICharacter actor)
	{
		if (windows.Count == 0)
		{
			return "Unrestricted".ColourValue();
		}

		return windows.Select(x => $"{x.Type.DescribeEnum().ColourName()} {x.Value.ColourValue()}").ListToString();
	}

	private static string DescribeCommodityOutputs(IReadOnlyCollection<AgricultureCommodityYield> outputs, ICharacter actor)
	{
		return outputs.Count == 0
			? "None".ColourError()
			: outputs.Select(x => $"{actor.Gameworld.UnitManager.DescribeMostSignificantExact(x.BaseWeight, MudSharp.Framework.Units.UnitType.Mass, actor).ColourValue()} {x.MaterialName.ColourName()}{(string.IsNullOrWhiteSpace(x.TagName) ? string.Empty : $" [{x.TagName.ColourName()}]")}").ListToString();
	}

	private static string DescribeScoreRanges(IReadOnlyCollection<AgricultureScoreRange> ranges, ICharacter actor)
	{
		var activeRanges = ranges
		                   .Where(x => x.Score.IsEnabledScore(actor.Gameworld))
		                   .OrderBy(x => x.Score)
		                   .Select(x => $"{x.Score.DescribeFor(actor.Gameworld).ColourName()} {x.Minimum.ToStringN0Colour(actor)}-{x.Maximum.ToStringN0Colour(actor)}")
		                   .ToList();
		return activeRanges.Count == 0 ? "None".ColourError() : activeRanges.ListToString();
	}

	private static void CreateCrop(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should this crop definition be called?");
			return;
		}

		var crop = new AgricultureCropDefinition(actor.Gameworld, ss.SafeRemainingArgument, "A custom agriculture crop definition.", "general", 60, 10, 35, 80, 5, 40);
		actor.Gameworld.Add(crop);
		actor.OutputHandler.Send($"You create agriculture crop definition {crop.Name.ColourName()}.");
	}

	private static void SetCrop(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var crop = ss.IsFinished ? null : actor.Gameworld.AgricultureCropDefinitions.GetByIdOrName(ss.PopSpeech());
		if (crop is not AgricultureCropDefinition concrete)
		{
			actor.OutputHandler.Send("There is no such agriculture crop definition.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set name, description, category, growth, perennial, cycle, window, planting, moisture, temperature, pollination, or score?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "name":
				concrete.BuildingSetName(ss.SafeRemainingArgument);
				actor.OutputHandler.Send($"You rename the crop definition to {concrete.Name.ColourName()}.");
				return;
			case "description":
			case "desc":
				concrete.BuildingSetDescription(ss.SafeRemainingArgument);
				actor.OutputHandler.Send("You update the crop description.");
				return;
			case "category":
				concrete.BuildingSetCategory(ss.SafeRemainingArgument);
				actor.OutputHandler.Send($"You set the crop category to {concrete.Category.ColourValue()}.");
				return;
			case "growth":
			case "establishment":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var growth))
				{
					actor.OutputHandler.Send("How many growth or establishment days should this crop need?");
					return;
				}

				concrete.BuildingSetGrowthDays(growth);
				actor.OutputHandler.Send($"You set the crop growth time to {concrete.BaseGrowthDays.ToStringN0Colour(actor)} days.");
				return;
			case "perennial":
			case "orchard":
				if (ss.IsFinished || !bool.TryParse(ss.PopSpeech(), out var perennial))
				{
					actor.OutputHandler.Send("Should this crop be perennial? True or false?");
					return;
				}

				concrete.BuildingSetPerennial(perennial);
				actor.OutputHandler.Send($"This crop is now {(concrete.IsPerennial ? "perennial".ColourValue() : "annual".ColourValue())}.");
				return;
			case "cycle":
			case "harvestcycle":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var cycle))
				{
					actor.OutputHandler.Send("How many days should this perennial crop need between harvests?");
					return;
				}

				concrete.BuildingSetHarvestCycleDays(cycle);
				actor.OutputHandler.Send($"You set the crop harvest cycle time to {concrete.HarvestCycleDays.ToStringN0Colour(actor)} days.");
				return;
			case "window":
			case "harvest":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var window))
				{
					actor.OutputHandler.Send("How many days should the harvest window last?");
					return;
				}

				concrete.BuildingSetHarvestWindowDays(window);
				actor.OutputHandler.Send($"You set the harvest window to {concrete.HarvestWindowDays.ToStringN0Colour(actor)} days.");
				return;
			case "planting":
			case "plant":
			case "seasons":
			case "season":
				SetCropPlanting(actor, concrete, ss);
				return;
			case "moisture":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var minMoisture) || ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var maxMoisture))
				{
					actor.OutputHandler.Send("You must specify minimum and maximum moisture scores.");
					return;
				}

				concrete.BuildingSetMoistureRange(minMoisture, maxMoisture);
				actor.OutputHandler.Send("You update the crop moisture range.");
				return;
			case "temperature":
			case "temp":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var minTemperature) || ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var maxTemperature))
				{
					actor.OutputHandler.Send("You must specify minimum and maximum temperatures in Celsius.");
					return;
				}

				concrete.BuildingSetTemperatureRange(minTemperature, maxTemperature);
				actor.OutputHandler.Send("You update the crop temperature range.");
				return;
			case "pollination":
			case "pollinate":
				if (ss.IsFinished || !Enum.TryParse<AgriculturePollinationDependency>(ss.PopSpeech(), true, out var dependency))
				{
					actor.OutputHandler.Send($"Valid pollination dependencies are {Enum.GetValues(typeof(AgriculturePollinationDependency)).OfType<AgriculturePollinationDependency>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return;
				}

				var healthBonus = dependency == AgriculturePollinationDependency.Beneficial ? 0 : 1;
				var yieldBonus = dependency == AgriculturePollinationDependency.Beneficial ? 1 : 2;
				if (!ss.IsFinished && (!int.TryParse(ss.PopSpeech(), out healthBonus) || healthBonus < 0 || healthBonus > 1))
				{
					actor.OutputHandler.Send("The pollination health bonus must be 0 or 1.");
					return;
				}

				if (!ss.IsFinished && (!int.TryParse(ss.PopSpeech(), out yieldBonus) || yieldBonus < 0 || yieldBonus > 2))
				{
					actor.OutputHandler.Send("The pollination yield bonus must be 0, 1, or 2.");
					return;
				}

				concrete.BuildingSetPollination(dependency, healthBonus, yieldBonus);
				actor.OutputHandler.Send($"You set crop pollination to {DescribePollination(concrete, actor)}.");
				return;
			case "score":
			case "scorerange":
			case "range":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send($"Which score should have a growing range? Valid scores are {ValidScoreTypesText(actor)}.");
					return;
				}

				var scoreText = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Do you want to specify a minimum and maximum score, or none to remove the requirement?");
					return;
				}

				if (ss.SafeRemainingArgument.EqualTo("none"))
				{
					if (!AgricultureScoreTypeExtensions.TryParseScoreType(scoreText, actor.Gameworld, out var removeScore, true))
					{
						actor.OutputHandler.Send("That is not a valid agriculture score.");
						return;
					}

					concrete.BuildingRemoveScoreRange(removeScore);
					actor.OutputHandler.Send($"You remove the {removeScore.DescribeFor(actor.Gameworld).ColourName()} growing range from this crop.");
					return;
				}

				if (!AgricultureScoreTypeExtensions.TryParseScoreType(scoreText, actor.Gameworld, out var cropScore))
				{
					actor.OutputHandler.Send($"That is not a valid enabled agriculture score. Valid scores are {ValidScoreTypesText(actor)}.");
					return;
				}

				if (!int.TryParse(ss.PopSpeech(), out var minScore) || ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var maxScore))
				{
					actor.OutputHandler.Send("You must specify minimum and maximum 0-100 scores.");
					return;
				}

				var range = new AgricultureScoreRange(cropScore, minScore, maxScore);
				concrete.BuildingSetScoreRange(cropScore, range.Minimum, range.Maximum);
				actor.OutputHandler.Send($"You set this crop's {cropScore.DescribeFor(actor.Gameworld).ColourName()} growing range to {range.Minimum.ToStringN0Colour(actor)}-{range.Maximum.ToStringN0Colour(actor)}.");
				return;
		}

		actor.OutputHandler.Send("Do you want to set name, description, category, growth, perennial, cycle, window, planting, moisture, temperature, pollination, or score?");
	}

	private static void SetCropPlanting(ICharacter actor, AgricultureCropDefinition crop, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Syntax: field crop set <crop> planting group|season <name> [<name> ...], or field crop set <crop> planting none.");
			return;
		}

		var mode = ss.PopSpeech().ToLowerInvariant();
		if (mode.EqualTo("none") || mode.EqualTo("clear"))
		{
			crop.BuildingClearPlantingWindows();
			actor.OutputHandler.Send("This crop no longer has planting season restrictions.");
			return;
		}

		var type = mode switch
		{
			"group" or "groups" => AgriculturePlantingWindowType.Group,
			"season" or "seasons" => AgriculturePlantingWindowType.Season,
			_ => (AgriculturePlantingWindowType?)null
		};
		if (type == null)
		{
			actor.OutputHandler.Send("Do you want to set planting windows by season group, exact season, or none?");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which planting window names do you want to allow?");
			return;
		}

		var windows = new List<AgriculturePlantingWindow>();
		while (!ss.IsFinished)
		{
			windows.Add(new AgriculturePlantingWindow(type.Value, ss.PopSpeech()));
		}

		crop.BuildingSetPlantingWindows(windows);
		actor.OutputHandler.Send($"You set the crop planting windows to {DescribePlantingWindows(crop.PlantingWindows, actor)}.");
	}

	private static void DeleteCrop(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var crop = ss.IsFinished ? null : actor.Gameworld.AgricultureCropDefinitions.GetByIdOrName(ss.SafeRemainingArgument);
		if (crop == null)
		{
			actor.OutputHandler.Send("There is no such agriculture crop definition.");
			return;
		}

		if (actor.Gameworld.AgricultureFields.Any(x => x.CurrentCrop?.Id == crop.Id))
		{
			actor.OutputHandler.Send("You cannot delete a crop definition while fields are growing it.");
			return;
		}

		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.AgricultureCropDefinitions.Find(crop.Id);
			if (dbitem != null)
			{
				FMDB.Context.AgricultureCropDefinitions.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		if (crop is AgricultureCropDefinition concrete)
		{
			actor.Gameworld.SaveManager.Abort(concrete);
		}

		actor.Gameworld.Destroy(crop);
		actor.OutputHandler.Send("You delete that agriculture crop definition.");
	}

	private static void FieldHerds(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			ListHerds(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ListHerds(actor);
				return;
			case "show":
				ShowHerdDefinition(actor, ss);
				return;
			case "create":
			case "new":
				CreateHerdDefinition(actor, ss);
				return;
			case "set":
				SetHerdDefinition(actor, ss);
				return;
			case "delete":
			case "remove":
				DeleteHerdDefinition(actor, ss);
				return;
		}

		actor.OutputHandler.Send("Syntax: field herds list|show|create|set|delete");
	}

	private static void ShowHerdDefinition(ICharacter actor, StringStack ss)
	{
		var herd = ss.IsFinished ? null : actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(ss.SafeRemainingArgument);
		if (herd == null)
		{
			actor.OutputHandler.Send("There is no such agriculture herd definition.");
			return;
		}

		actor.OutputHandler.Send($@"{herd.Name.ColourName()} (#{herd.Id.ToString("N0", actor)})
{herd.Description}
Animal Units: {herd.AnimalUnits.ToString("N2", actor).ColourValue()}
Daily Graze: {herd.DailyGraze.ToString("N2", actor).ColourValue()}
Maximum Condition: {herd.MaximumCondition.ToString("N0", actor).ColourValue()}
Secondary Outputs: {DescribeCommodityOutputs(herd.SecondaryOutputs, actor)}
NPC Template: {(herd.NpcTemplate?.Name ?? "None").ColourName()}");
	}

	private static void CreateHerdDefinition(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should this herd definition be called?");
			return;
		}

		var herd = new AgricultureHerdDefinition(actor.Gameworld, ss.SafeRemainingArgument, "A custom agriculture herd definition.", null, 1.0, 1.0, 100);
		actor.Gameworld.Add(herd);
		actor.OutputHandler.Send($"You create agriculture herd definition {herd.Name.ColourName()}.");
	}

	private static void SetHerdDefinition(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var herd = ss.IsFinished ? null : actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(ss.PopSpeech());
		if (herd is not AgricultureHerdDefinition concrete)
		{
			actor.OutputHandler.Send("There is no such agriculture herd definition.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set name, description, animalunits, graze, condition, output, or npc?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "name":
				concrete.BuildingSetName(ss.SafeRemainingArgument);
				actor.OutputHandler.Send($"You rename the herd definition to {concrete.Name.ColourName()}.");
				return;
			case "description":
			case "desc":
				concrete.BuildingSetDescription(ss.SafeRemainingArgument);
				actor.OutputHandler.Send("You update the herd description.");
				return;
			case "animalunits":
			case "au":
				if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), out var animalUnits))
				{
					actor.OutputHandler.Send("What animal-unit value should each head count as?");
					return;
				}

				concrete.BuildingSetAnimalUnits(animalUnits);
				actor.OutputHandler.Send($"You set animal units to {concrete.AnimalUnits.ToString("N2", actor).ColourValue()}.");
				return;
			case "graze":
			case "dailygraze":
				if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), out var dailyGraze))
				{
					actor.OutputHandler.Send("What daily graze pressure should each head apply?");
					return;
				}

				concrete.BuildingSetDailyGraze(dailyGraze);
				actor.OutputHandler.Send($"You set daily graze pressure to {concrete.DailyGraze.ToString("N2", actor).ColourValue()}.");
				return;
			case "condition":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var condition))
				{
					actor.OutputHandler.Send("What maximum condition score should this herd use?");
					return;
				}

				concrete.BuildingSetMaximumCondition(condition);
				actor.OutputHandler.Send($"You set maximum condition to {concrete.MaximumCondition.ToStringN0Colour(actor)}.");
				return;
			case "output":
			case "outputs":
			case "secondary":
				SetHerdDefinitionOutput(actor, concrete, ss);
				return;
			case "npc":
			case "template":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which NPC template should be materialised for drawdown, or none?");
					return;
				}

				if (ss.SafeRemainingArgument.EqualTo("none"))
				{
					concrete.BuildingSetNpcTemplate(null);
					actor.OutputHandler.Send("This herd will not materialise NPCs until a template is assigned.");
					return;
				}

				var template = actor.Gameworld.NpcTemplates.GetByIdOrName(ss.SafeRemainingArgument);
				if (template == null)
				{
					actor.OutputHandler.Send("There is no such NPC template.");
					return;
				}

				concrete.BuildingSetNpcTemplate(template);
				actor.OutputHandler.Send($"You set the materialised NPC template to {template.Name.ColourName()}.");
				return;
		}

		actor.OutputHandler.Send("Do you want to set name, description, animalunits, graze, condition, output, or npc?");
	}

	private static void SetHerdDefinitionOutput(ICharacter actor, AgricultureHerdDefinition herd, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Syntax: field herds set <herd> output clear|add <weight grams> <material> [tag <tag>]");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clear":
				herd.BuildingSetSecondaryOutputs(Array.Empty<AgricultureCommodityYield>());
				actor.OutputHandler.Send("You clear the secondary product outputs for this herd definition.");
				return;
			case "add":
				if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), out var weight) || weight <= 0.0)
				{
					actor.OutputHandler.Send("What positive output weight in grams should each head produce at full condition?");
					return;
				}

				var tagName = string.Empty;
				var remaining = ss.SafeRemainingArgument;
				var tagIndex = remaining.LastIndexOf(" tag ", StringComparison.InvariantCultureIgnoreCase);
				if (tagIndex >= 0)
				{
					tagName = remaining[(tagIndex + 5)..].Trim().Trim('"');
					remaining = remaining[..tagIndex].Trim();
				}

				if (string.IsNullOrWhiteSpace(remaining))
				{
					actor.OutputHandler.Send("Which material should this secondary output use?");
					return;
				}

				if (actor.Gameworld.Materials.GetByName(remaining) == null)
				{
					actor.OutputHandler.Send("There is no such material.");
					return;
				}

				if (!string.IsNullOrWhiteSpace(tagName) && actor.Gameworld.Tags.GetByName(tagName) == null)
				{
					actor.OutputHandler.Send("There is no such tag.");
					return;
				}

				herd.BuildingSetSecondaryOutputs(herd.SecondaryOutputs.Concat(new[]
				{
					new AgricultureCommodityYield(remaining, weight, tagName)
				}));
				actor.OutputHandler.Send($"You add {actor.Gameworld.UnitManager.DescribeMostSignificantExact(weight, MudSharp.Framework.Units.UnitType.Mass, actor).ColourValue()} of {remaining.ColourName()} per head to this herd's secondary outputs.");
				return;
		}

		actor.OutputHandler.Send("Syntax: field herds set <herd> output clear|add <weight grams> <material> [tag <tag>]");
	}

	private static void DeleteHerdDefinition(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var herd = ss.IsFinished ? null : actor.Gameworld.AgricultureHerdDefinitions.GetByIdOrName(ss.SafeRemainingArgument);
		if (herd == null)
		{
			actor.OutputHandler.Send("There is no such agriculture herd definition.");
			return;
		}

		if (actor.Gameworld.AgricultureFields.Any(x => x.Herds.Any(y => y.Definition.Id == herd.Id)))
		{
			actor.OutputHandler.Send("You cannot delete a herd definition while fields are using it.");
			return;
		}

		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.AgricultureHerdDefinitions.Find(herd.Id);
			if (dbitem != null)
			{
				FMDB.Context.AgricultureHerdDefinitions.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		if (herd is AgricultureHerdDefinition concrete)
		{
			actor.Gameworld.SaveManager.Abort(concrete);
		}

		actor.Gameworld.Destroy(herd);
		actor.OutputHandler.Send("You delete that agriculture herd definition.");
	}

	private static void FieldWoodlands(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			ListWoodlands(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ListWoodlands(actor);
				return;
			case "show":
				ShowWoodland(actor, ss);
				return;
			case "create":
			case "new":
				CreateWoodland(actor, ss);
				return;
			case "set":
				SetWoodland(actor, ss);
				return;
			case "delete":
			case "remove":
				DeleteWoodland(actor, ss);
				return;
		}

		actor.OutputHandler.Send("Syntax: field woodland list|show|create|set|delete");
	}

	private static void ShowWoodland(ICharacter actor, StringStack ss)
	{
		var woodland = ss.IsFinished ? null : actor.Gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(ss.SafeRemainingArgument);
		if (woodland == null)
		{
			actor.OutputHandler.Send("There is no such agriculture woodland definition.");
			return;
		}

		actor.OutputHandler.Send($@"{woodland.Name.ColourName()} (#{woodland.Id.ToString("N0", actor)})
{woodland.Description}
Type: {woodland.WoodlandType.ColourValue()}
Establishment Days: {woodland.EstablishmentDays.ToStringN0Colour(actor)}
Harvest Cycle Days: {woodland.HarvestCycleDays.ToStringN0Colour(actor)}
Outputs: {DescribeCommodityOutputs(woodland.YieldOutputs, actor)}");
	}

	private static void CreateWoodland(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should this woodland definition be called?");
			return;
		}

		var woodland = new AgricultureWoodlandDefinition(actor.Gameworld, ss.SafeRemainingArgument, "A custom agriculture woodland definition.", "managed", 180, 365);
		actor.Gameworld.Add(woodland);
		actor.OutputHandler.Send($"You create agriculture woodland definition {woodland.Name.ColourName()}.");
	}

	private static void SetWoodland(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var woodland = ss.IsFinished ? null : actor.Gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(ss.PopSpeech());
		if (woodland is not AgricultureWoodlandDefinition concrete)
		{
			actor.OutputHandler.Send("There is no such agriculture woodland definition.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set name, description, type, establishment, or cycle?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "name":
				concrete.BuildingSetName(ss.SafeRemainingArgument);
				actor.OutputHandler.Send($"You rename the woodland definition to {concrete.Name.ColourName()}.");
				return;
			case "description":
			case "desc":
				concrete.BuildingSetDescription(ss.SafeRemainingArgument);
				actor.OutputHandler.Send("You update the woodland description.");
				return;
			case "type":
			case "category":
				concrete.BuildingSetWoodlandType(ss.SafeRemainingArgument);
				actor.OutputHandler.Send($"You set the woodland type to {concrete.WoodlandType.ColourValue()}.");
				return;
			case "establishment":
			case "establish":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var establishment))
				{
					actor.OutputHandler.Send("How many establishment days should this woodland need?");
					return;
				}

				concrete.BuildingSetEstablishmentDays(establishment);
				actor.OutputHandler.Send($"You set establishment time to {concrete.EstablishmentDays.ToStringN0Colour(actor)} days.");
				return;
			case "cycle":
			case "harvest":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var cycle))
				{
					actor.OutputHandler.Send("How many days should the harvest cycle take?");
					return;
				}

				concrete.BuildingSetHarvestCycleDays(cycle);
				actor.OutputHandler.Send($"You set harvest cycle time to {concrete.HarvestCycleDays.ToStringN0Colour(actor)} days.");
				return;
		}

		actor.OutputHandler.Send("Do you want to set name, description, type, establishment, or cycle?");
	}

	private static void DeleteWoodland(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var woodland = ss.IsFinished ? null : actor.Gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(ss.SafeRemainingArgument);
		if (woodland == null)
		{
			actor.OutputHandler.Send("There is no such agriculture woodland definition.");
			return;
		}

		if (actor.Gameworld.AgricultureFields.Any(x => x.CurrentWoodland?.Id == woodland.Id))
		{
			actor.OutputHandler.Send("You cannot delete a woodland definition while fields are using it.");
			return;
		}

		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.AgricultureWoodlandDefinitions.Find(woodland.Id);
			if (dbitem != null)
			{
				FMDB.Context.AgricultureWoodlandDefinitions.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		if (woodland is AgricultureWoodlandDefinition concrete)
		{
			actor.Gameworld.SaveManager.Abort(concrete);
		}

		actor.Gameworld.Destroy(woodland);
		actor.OutputHandler.Send("You delete that agriculture woodland definition.");
	}

	private static void FieldOperations(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			ListOperations(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ListOperations(actor);
				return;
			case "show":
				ShowOperation(actor, ss);
				return;
			case "create":
			case "new":
				CreateOperation(actor, ss);
				return;
			case "set":
				SetOperation(actor, ss);
				return;
			case "delete":
			case "remove":
				DeleteOperation(actor, ss);
				return;
		}

		actor.OutputHandler.Send("Syntax: field operation list|show|create|set|delete");
	}

	private static void ShowOperation(ICharacter actor, StringStack ss)
	{
		var operation = ss.IsFinished ? null : actor.Gameworld.AgricultureOperations.GetByIdOrName(ss.SafeRemainingArgument);
		if (operation == null)
		{
			actor.OutputHandler.Send("There is no such agriculture operation.");
			return;
		}

		actor.OutputHandler.Send($@"{operation.Name.ColourName()} (#{operation.Id.ToString("N0", actor)})
{operation.Description}
Type: {operation.OperationType.DescribeEnum().ColourName()}
Target: {operation.TargetType.DescribeEnum().ColourName()}
Required Use: {operation.RequiredUse.DescribeEnum().ColourName()}
Allowed Uses: {operation.AllowedUses.OrderBy(x => (int)x).Select(x => x.DescribeEnum().ColourName()).ListToString()}
Result Use: {operation.ResultUse.DescribeEnum().ColourName()}
Project: {(operation.Project?.Name ?? "None").ColourName()}
Completion Prog: {(operation.CompletionProg?.FunctionName ?? "None").ColourName()}
Woodland Yield: x{operation.WoodlandYieldMultiplier.ToString("N2", actor).ColourValue()}, consumes {operation.WoodlandYieldCost.ToString("N0", actor).ColourValue()} yield
Herd Yield: x{operation.HerdYieldMultiplier.ToString("N2", actor).ColourValue()}, consumes {operation.HerdYieldCost.ToString("N0", actor).ColourValue()} yield
Apiary: install {operation.ApiaryInstallHiveCount.ToString("N0", actor).ColourValue()} hives, radius {operation.ApiaryPollinationRadius.ToString("N0", actor).ColourValue()}, tend {operation.ApiaryTendHealthDelta.ToString("N0", actor).ColourValue()}/{operation.ApiaryTendStoresDelta.ToString("N0", actor).ColourValue()}/{operation.ApiaryTendYieldDelta.ToString("N0", actor).ColourValue()}, harvest x{operation.ApiaryYieldMultiplier.ToString("N2", actor).ColourValue()} cost {operation.ApiaryYieldCost.ToString("N0", actor).ColourValue()} [{DescribeCommodityOutputs(operation.ApiaryYieldOutputs, actor)}]
Deltas: {operation.ScoreDeltas.Where(x => x.Key.IsEnabledScore(actor.Gameworld)).OrderBy(x => x.Key).Select(x => $"{x.Key.DescribeFor(actor.Gameworld)} {x.Value.ToString("N0", actor)}").ListToString()}");
	}

	private static void CreateOperation(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should this operation be called?");
			return;
		}

		var name = ss.PopSpeech();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which local project template should this operation use?");
			return;
		}

		var project = actor.Gameworld.Projects.GetByIdOrName(ss.SafeRemainingArgument);
		if (project == null)
		{
			actor.OutputHandler.Send("There is no such project template.");
			return;
		}

		var operation = new AgricultureOperation(actor.Gameworld, name, "A custom agriculture operation.",
			AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Fallow, project, new Dictionary<AgricultureScoreType, int>());
		actor.Gameworld.Add(operation);
		actor.OutputHandler.Send($"You create agriculture operation {operation.Name.ColourName()}.");
	}

	private static void SetOperation(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var operation = ss.IsFinished ? null : actor.Gameworld.AgricultureOperations.GetByIdOrName(ss.PopSpeech());
		if (operation is not AgricultureOperation concrete)
		{
			actor.OutputHandler.Send("There is no such agriculture operation.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set name, description, type, target, required, allowed, result, project, prog, delta, woodlandyield, or herdyield?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "name":
				concrete.BuildingSetName(ss.SafeRemainingArgument);
				actor.OutputHandler.Send($"You rename the operation to {concrete.Name.ColourName()}.");
				return;
			case "description":
			case "desc":
				concrete.BuildingSetDescription(ss.SafeRemainingArgument);
				actor.OutputHandler.Send("You update the operation description.");
				return;
			case "type":
				if (ss.IsFinished || !Enum.TryParse<AgricultureOperationType>(ss.PopSpeech(), true, out var operationType))
				{
					actor.OutputHandler.Send($"Valid operation types are {Enum.GetValues(typeof(AgricultureOperationType)).OfType<AgricultureOperationType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return;
				}

				concrete.BuildingSetOperationType(operationType);
				actor.OutputHandler.Send($"You set the operation type to {operationType.DescribeEnum().ColourName()}.");
				return;
			case "target":
				if (ss.IsFinished || !Enum.TryParse<AgricultureTargetType>(ss.PopSpeech(), true, out var targetType))
				{
					actor.OutputHandler.Send($"Valid target types are {Enum.GetValues(typeof(AgricultureTargetType)).OfType<AgricultureTargetType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return;
				}

				concrete.BuildingSetTargetType(targetType);
				actor.OutputHandler.Send($"You set the operation target type to {targetType.DescribeEnum().ColourName()}.");
				return;
			case "required":
			case "requireduse":
				if (ss.IsFinished || !Enum.TryParse<AgricultureFieldUse>(ss.PopSpeech(), true, out var requiredUse))
				{
					actor.OutputHandler.Send($"Valid field uses are {Enum.GetValues(typeof(AgricultureFieldUse)).OfType<AgricultureFieldUse>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return;
				}

				concrete.BuildingSetRequiredUse(requiredUse);
				actor.OutputHandler.Send($"You set the required field use to {requiredUse.DescribeEnum().ColourName()}.");
				return;
			case "allowed":
			case "allowuse":
			case "alloweduse":
				if (ss.IsFinished || !Enum.TryParse<AgricultureFieldUse>(ss.PopSpeech(), true, out var allowedUse))
				{
					actor.OutputHandler.Send($"Valid field uses are {Enum.GetValues(typeof(AgricultureFieldUse)).OfType<AgricultureFieldUse>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return;
				}

				var allowed = true;
				if (!ss.IsFinished && !TryParseOnOff(ss.PopSpeech(), out allowed))
				{
					actor.OutputHandler.Send("Do you want to allow or disallow that field use?");
					return;
				}

				concrete.BuildingSetAllowedUse(allowedUse, allowed);
				actor.OutputHandler.Send($"You {(allowed ? "allow" : "disallow")} this operation on {allowedUse.DescribeEnum().ColourName()} fields.");
				return;
			case "result":
			case "resultuse":
				if (ss.IsFinished || !Enum.TryParse<AgricultureFieldUse>(ss.PopSpeech(), true, out var resultUse))
				{
					actor.OutputHandler.Send($"Valid field uses are {Enum.GetValues(typeof(AgricultureFieldUse)).OfType<AgricultureFieldUse>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return;
				}

				concrete.BuildingSetResultUse(resultUse);
				actor.OutputHandler.Send($"You set the result field use to {resultUse.DescribeEnum().ColourName()}.");
				return;
			case "project":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which project template should this operation use?");
					return;
				}

				var project = actor.Gameworld.Projects.GetByIdOrName(ss.SafeRemainingArgument);
				if (project == null)
				{
					actor.OutputHandler.Send("There is no such project template.");
					return;
				}

				concrete.BuildingSetProject(project);
				actor.OutputHandler.Send($"You set the operation project to {project.Name.ColourName()}.");
				return;
			case "prog":
			case "completion":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which FutureProg should run on completion, or none?");
					return;
				}

				if (ss.SafeRemainingArgument.EqualTo("none"))
				{
					concrete.BuildingSetCompletionProg(null);
					actor.OutputHandler.Send("This operation no longer runs a completion prog.");
					return;
				}

				var prog = actor.Gameworld.FutureProgs.GetByIdOrName(ss.SafeRemainingArgument);
				if (prog == null)
				{
					actor.OutputHandler.Send("There is no such FutureProg.");
					return;
				}

				concrete.BuildingSetCompletionProg(prog);
				actor.OutputHandler.Send($"You set the completion prog to {prog.FunctionName.ColourName()}.");
				return;
			case "delta":
			case "score":
				if (ss.IsFinished || !AgricultureScoreTypeExtensions.TryParseScoreType(ss.PopSpeech(), actor.Gameworld, out var score))
				{
					actor.OutputHandler.Send($"Which field score should this operation adjust? Valid scores are {ValidScoreTypesText(actor)}.");
					return;
				}

				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var delta))
				{
					actor.OutputHandler.Send("What signed delta should be applied to that score?");
					return;
				}

				concrete.BuildingSetScoreDelta(score, delta);
				actor.OutputHandler.Send($"You set the {score.DescribeFor(actor.Gameworld).ColourName()} delta to {delta.ToString("N0", actor).ColourValue()}.");
				return;
			case "woodlandyield":
			case "yield":
				if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), out var multiplier) || ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var cost))
				{
					actor.OutputHandler.Send("You must specify a commodity multiplier and a 0-100 woodland yield cost.");
					return;
				}

				concrete.BuildingSetWoodlandYield(multiplier, cost);
				actor.OutputHandler.Send($"You set woodland product output to x{concrete.WoodlandYieldMultiplier.ToString("N2", actor).ColourValue()} and yield cost to {concrete.WoodlandYieldCost.ToString("N0", actor).ColourValue()}.");
				return;
			case "herdyield":
				if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), out var herdMultiplier) || ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var herdCost))
				{
					actor.OutputHandler.Send("You must specify a commodity multiplier and a 0-100 herd yield cost.");
					return;
				}

				concrete.BuildingSetHerdYield(herdMultiplier, herdCost);
				actor.OutputHandler.Send($"You set herd product output to x{concrete.HerdYieldMultiplier.ToString("N2", actor).ColourValue()} and yield cost to {concrete.HerdYieldCost.ToString("N0", actor).ColourValue()}.");
				return;
		}

		actor.OutputHandler.Send("Do you want to set name, description, type, target, required, allowed, result, project, prog, delta, woodlandyield, or herdyield?");
	}

	private static void DeleteOperation(ICharacter actor, StringStack ss)
	{
		if (!RequireAdministrator(actor))
		{
			return;
		}

		var operation = ss.IsFinished ? null : actor.Gameworld.AgricultureOperations.GetByIdOrName(ss.SafeRemainingArgument);
		if (operation == null)
		{
			actor.OutputHandler.Send("There is no such agriculture operation.");
			return;
		}

		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			if (FMDB.Context.AgricultureProjectContexts.Any(x => x.OperationId == operation.Id))
			{
				actor.OutputHandler.Send("You cannot delete an operation while active projects are using it.");
				return;
			}

			var dbitem = FMDB.Context.AgricultureOperations.Find(operation.Id);
			if (dbitem != null)
			{
				FMDB.Context.AgricultureOperations.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		if (operation is AgricultureOperation concrete)
		{
			actor.Gameworld.SaveManager.Abort(concrete);
		}

		actor.Gameworld.Destroy(operation);
		actor.OutputHandler.Send("You delete that agriculture operation.");
	}

	private static void ListProfiles(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AgricultureFieldProfiles.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.Description,
				Enum.GetValues(typeof(AgricultureFieldUse))
				    .OfType<AgricultureFieldUse>()
				    .Where(x.AllowsUse)
				    .Select(y => y.DescribeEnum())
				    .ListToString()
			}),
			new[] { "Id", "Name", "Description", "Uses" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			truncatableColumnIndex: 2,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void ListCrops(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AgricultureCropDefinitions.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.Category,
				x.BaseGrowthDays.ToString("N0", actor),
				x.HarvestWindowDays.ToString("N0", actor)
			}),
			new[] { "Id", "Name", "Category", "Growth", "Window" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void ListHerds(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AgricultureHerdDefinitions.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.AnimalUnits.ToString("N2", actor),
				x.DailyGraze.ToString("N2", actor),
				x.CanMaterialise.ToColouredString()
			}),
			new[] { "Id", "Name", "AU", "Graze", "NPC?" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void ListWoodlands(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AgricultureWoodlandDefinitions.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.WoodlandType,
				x.EstablishmentDays.ToString("N0", actor),
				x.HarvestCycleDays.ToString("N0", actor)
			}),
			new[] { "Id", "Name", "Category", "Establish", "Cycle" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void ListOperations(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			actor.Gameworld.AgricultureOperations.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.OperationType.DescribeEnum(),
				x.TargetType.DescribeEnum(),
				x.Project?.Name ?? "None"
			}),
			new[] { "Id", "Name", "Type", "Target", "Project" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}
}
