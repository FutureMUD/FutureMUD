using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.DateTime;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Menus;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using static MudSharp.Effects.Concrete.Butchering;

namespace MudSharp.Work.Butchering;

public class RaceButcheryProfile : SaveableItem, IRaceButcheryProfile
{
	public RaceButcheryProfile(MudSharp.Models.RaceButcheryProfile profile, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = profile.Id;
		_name = profile.Name;
		Verb = (ButcheryVerb)profile.Verb;
		RequiredToolTag = gameworld.Tags.Get(profile.RequiredToolTagId ?? 0L);
		DifficultySkin = (Difficulty)profile.DifficultySkin;
		CanButcherProg = gameworld.FutureProgs.Get(profile.CanButcherProgId ?? 0L);
		WhyCannotButcherProg = gameworld.FutureProgs.Get(profile.WhyCannotButcherProgId ?? 0L);
		foreach (var product in profile.RaceButcheryProfilesButcheryProducts.SelectNotNull(item =>
			         gameworld.ButcheryProducts.Get(item.ButcheryProductId)))
		{
			_products.Add(product);
		}

		foreach (var item in profile.RaceButcheryProfilesBreakdownChecks)
		{
			_breakdownChecks.Add(
				string.IsNullOrEmpty(item.Subcageory) ? string.Empty : item.Subcageory.ToLowerInvariant(),
				(gameworld.Traits.Get(item.TraitDefinitionId), (Difficulty)item.Difficulty));
		}

		foreach (var item in profile.RaceButcheryProfilesSkinningEmotes.OrderBy(x => x.Order))
		{
			_skinEmotes.Add((item.Emote, item.Delay));
		}

		foreach (var category in profile.RaceButcheryProfilesBreakdownEmotes.GroupBy(x => x.Subcategory))
		{
			_breakdownEmotes.AddRange(
				string.IsNullOrEmpty(category.Key) ? string.Empty : category.Key.ToLowerInvariant(),
				category.OrderBy(x => x.Order).Select(item => (item.Emote, item.Delay)).ToList());
		}

		RecalculateInventoryPlan();
	}

	public RaceButcheryProfile(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		Verb = ButcheryVerb.Butcher;
		DifficultySkin = Difficulty.Normal;
		using (new FMDB())
		{
			var dbitem = new Models.RaceButcheryProfile
			{
				Name = name,
				Verb = (int)ButcheryVerb.Butcher,
				DifficultySkin = (int)Difficulty.Normal
			};
			FMDB.Context.RaceButcheryProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		RecalculateInventoryPlan();
	}

	public IRaceButcheryProfile Clone(string newName, bool includeProducts)
	{
		using (new FMDB())
		{
			var dbnew = new Models.RaceButcheryProfile
			{
				Name = newName,
				DifficultySkin = (int)DifficultySkin,
				CanButcherProgId = CanButcherProg?.Id,
				WhyCannotButcherProgId = WhyCannotButcherProg?.Id,
				Verb = (int)Verb,
				RequiredToolTagId = RequiredToolTag?.Id
			};
			FMDB.Context.RaceButcheryProfiles.Add(dbnew);
			foreach (var check in _breakdownChecks)
			{
				dbnew.RaceButcheryProfilesBreakdownChecks.Add(new RaceButcheryProfilesBreakdownChecks
				{
					RaceButcheryProfile = dbnew,
					Subcageory = check.Key,
					Difficulty = (int)check.Value.CheckDifficulty,
					TraitDefinitionId = check.Value.Trait.Id
				});
			}

			var i = 1;
			foreach (var emote in _skinEmotes)
			{
				dbnew.RaceButcheryProfilesSkinningEmotes.Add(new RaceButcheryProfilesSkinningEmotes
				{
					RaceButcheryProfile = dbnew,
					Delay = emote.Delay,
					Subcategory = string.Empty,
					Emote = emote.Emote,
					Order = i++
				});
			}


			foreach (var category in _breakdownEmotes)
			{
				i = 1;
				foreach (var emote in category.Value)
				{
					dbnew.RaceButcheryProfilesBreakdownEmotes.Add(new RaceButcheryProfilesBreakdownEmotes()
					{
						RaceButcheryProfile = dbnew,
						Delay = emote.Delay,
						Subcategory = category.Key,
						Emote = emote.Emote,
						Order = i++
					});
				}
			}

			if (includeProducts)
			{
				foreach (var product in _products)
				{
					dbnew.RaceButcheryProfilesButcheryProducts.Add(new RaceButcheryProfilesButcheryProducts
					{
						RaceButcheryProfile = dbnew,
						ButcheryProductId = product.Id
					});
				}
			}

			FMDB.Context.SaveChanges();
			return new RaceButcheryProfile(dbnew, Gameworld);
		}
	}

	private void RecalculateInventoryPlan()
	{
		if (RequiredToolTag != null)
		{
			ToolTemplate = new InventoryPlanTemplate(Gameworld, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					new InventoryPlanActionHold(Gameworld, RequiredToolTag.Id, 0, item => true, null, 1)
				})
			});
		}
		else
		{
			ToolTemplate = new InventoryPlanTemplate(Gameworld, new[]
			{
				new InventoryPlanPhaseTemplate(1, new List<IInventoryPlanAction>())
			});
		}
	}

	#region Implementation of IRaceButcheryProfile

	/// <summary>
	/// The verb used to interact with this race, e.g. butcher vs salvage
	/// </summary>
	public ButcheryVerb Verb { get; set; }

	/// <summary>
	/// If specified, a tool tag required to be held to complete the process
	/// </summary>
	public ITag RequiredToolTag { get; set; }

	/// <summary>
	/// The difficulty to use the SKIN verb on this
	/// </summary>
	public Difficulty DifficultySkin { get; set; }

	private readonly Dictionary<string, (ITraitDefinition Trait, Difficulty CheckDifficulty)> _breakdownChecks = new();

	/// <summary>
	/// The trait and difficulty to use the BUTCHER/SALVAGE verbs on this
	/// </summary>
	/// <param name="subcategory">If specified, the subcategory for which to fetch the difficulty</param>
	/// <returns>The trait and difficulty of the breakdown</returns>
	public (ITraitDefinition Trait, Difficulty CheckDifficulty) BreakdownCheck(string subcategory)
	{
		return _breakdownChecks[subcategory];
	}

	private readonly List<(string Emote, double Delay)> _skinEmotes = new();

	/// <summary>
	/// The emotes and delays between each phase of skinning
	/// </summary>
	public IEnumerable<(string Emote, double Delay)> SkinEmotes => _skinEmotes;

	private readonly CollectionDictionary<string, (string Emote, double Delay)> _breakdownEmotes = new();

	/// <summary>
	/// The emotes and delays between each phase of breakdown
	/// </summary>
	/// <param name="subcategory">If specified, the subcategory for which to fetch the breakdown emotes</param>
	/// <returns>The emotes and delays for the phases</returns>
	public IEnumerable<(string Emote, double Delay)> BreakdownEmotes(string subcategory)
	{
		return _breakdownEmotes[subcategory];
	}

	private readonly List<IButcheryProduct> _products = new();

	/// <summary>
	/// The products to load when salvaged
	/// </summary>
	public IEnumerable<IButcheryProduct> Products => _products;

	public IInventoryPlanTemplate ToolTemplate { get; private set; }

	/// <summary>
	/// A prog accepting a character and item parameter which determines whether a character can butcher this race
	/// </summary>
	public IFutureProg CanButcherProg { get; set; }

	/// <summary>
	/// Determines whether a character can butcher the target corpse
	/// </summary>
	/// <param name="butcher">The character doing the butchering</param>
	/// <param name="targetItem">The bodypart or corpse being butchered</param>
	/// <returns>True if the butcher can butcher the item</returns>
	public bool CanButcher(ICharacter butcher, IGameItem targetItem)
	{
		if (targetItem.EffectsOfType<BeingButchered>().Any())
		{
			return false;
		}

		var plan = ToolTemplate.CreatePlan(butcher);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			return false;
		}

		return (bool?)CanButcherProg?.Execute(butcher, targetItem) ?? true;
	}

	/// <summary>
	/// A prog accepting a character and item parameter which determines an error message when a character cannot butcher this race
	/// </summary>
	public IFutureProg WhyCannotButcherProg { get; set; }

	/// <summary>
	/// Retrieves an error message when a character cannot butcher a race
	/// </summary>
	/// <param name="butcher">The character doing the butchering</param>
	/// <param name="targetItem">The bodypart or corpse being butchered</param>
	/// <returns>An error message</returns>
	public string WhyCannotButcher(ICharacter butcher, IGameItem targetItem)
	{
		if (targetItem.EffectsOfType<BeingButchered>().Any())
		{
			return
				$"You cannot {Verb.Describe(false)} {targetItem.HowSeen(butcher)} because {targetItem.EffectsOfType<BeingButchered>().First().Butcher.HowSeen(butcher)} is already {Verb.DescribeGerund()} it.";
		}

		var plan = ToolTemplate.CreatePlan(butcher);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				return
					$"You cannot {Verb.Describe(false)} {targetItem.HowSeen(butcher)} because you don't have a tool with the {RequiredToolTag.FullName.Colour(Telnet.Cyan)} tag.";
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				return
					$"You cannot {Verb.Describe(false)} {targetItem.HowSeen(butcher)} because you don't have enough free {butcher.Body.WielderDescriptionPlural} to pick up the available tools.";
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				return
					$"You cannot {Verb.Describe(false)} {targetItem.HowSeen(butcher)} because you don't have enough free {butcher.Body.WielderDescriptionPlural} to pick up the available tools.";
		}

		return (string)WhyCannotButcherProg?.Execute(butcher, targetItem) ??
		       $"You cannot {Verb.Describe(false)} that item for an unknown reason";
	}

	#endregion

	#region Overrides of Item

	public override string FrameworkItemType => "RaceButcheryProfile";

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.RaceButcheryProfiles.Find(Id);
		dbitem.Name = Name;
		dbitem.DifficultySkin = (int)DifficultySkin;
		dbitem.RequiredToolTagId = RequiredToolTag?.Id;
		dbitem.Verb = (int)Verb;
		dbitem.CanButcherProgId = CanButcherProg?.Id;
		dbitem.WhyCannotButcherProgId = WhyCannotButcherProg?.Id;
		FMDB.Context.RaceButcheryProfilesBreakdownChecks.RemoveRange(dbitem.RaceButcheryProfilesBreakdownChecks);
		foreach (var item in _breakdownChecks)
		{
			dbitem.RaceButcheryProfilesBreakdownChecks.Add(new RaceButcheryProfilesBreakdownChecks
			{
				RaceButcheryProfile = dbitem, Subcageory = item.Key, TraitDefinitionId = item.Value.Trait.Id,
				Difficulty = (int)item.Value.CheckDifficulty
			});
		}

		FMDB.Context.RaceButcheryProfilesBreakdownEmotes.RemoveRange(dbitem.RaceButcheryProfilesBreakdownEmotes);
		foreach (var emote in _breakdownEmotes)
		{
			var order = 1;
			foreach (var value in emote.Value)
			{
				dbitem.RaceButcheryProfilesBreakdownEmotes.Add(new RaceButcheryProfilesBreakdownEmotes
				{
					RaceButcheryProfile = dbitem,
					Emote = value.Emote,
					Delay = value.Delay,
					Order = order++,
					Subcategory = emote.Key
				});
			}
		}

		FMDB.Context.RaceButcheryProfilesSkinningEmotes.RemoveRange(dbitem.RaceButcheryProfilesSkinningEmotes);
		var skinOrder = 1;
		foreach (var emote in _skinEmotes)
		{
			dbitem.RaceButcheryProfilesSkinningEmotes.Add(new RaceButcheryProfilesSkinningEmotes
			{
				RaceButcheryProfile = dbitem, Emote = emote.Emote, Delay = emote.Delay, Order = skinOrder++,
				Subcategory = string.Empty
			});
		}

		Changed = false;
	}

	#endregion

	private const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this profile
	#3verb butcher|salvage#0 - changes the verb used for interacting with these corpses
	#3tool <tag>#0 - sets the tag of tools required to interact with this
	#3skindiff <difficulty>#0 - sets the difficulty of the skinning check
	#3can <prog>#0 - sets a prog to control whether someone can butcher this
	#3why <prog>#0 - sets a prog for a custom error message on can butcher failure
	#3product <which>#0 - toggles a butchery product being included in this profile

For all of the below phase emote echoes, you can use #6$0#0 for the actor, #6$1#0 for the corpse, and #6$2#0 for the tool item.

#FSkin Phases#0

	#3skinemote add <seconds> <emotetext>#0 - adds a new skinning phase with specified length and emote
	#3skinemote remove <##>#0 - removes the specified skin phase
	#3skinemote swap <##1> <##2>#0 - swaps the order of two skin phases
	#3skinemote edit <##> <new text>#0 - changes the echo for a skin emote
	#3skinemote delay <##> <seconds>#0 - changes the delay for a skin emote phase

#FButcher Phases#0

	#3emote add <seconds> <emotetext>#0 - adds a new phase with specified length and emote
	#3emote remove <##>#0 - removes the specified phase
	#3emote swap <##1> <##2>#0 - swaps the order of two phases
	#3emote edit <##> <new text>#0 - changes the echo for an emote
	#3emote delay <##> <seconds>#0 - changes the delay for an emote phase

#FSub-Component Butcher Phases#0

	#3subemote <which> add <seconds> <emotetext>#0 - adds a new phase with specified length and emote
	#3subemote <which> remove <##>#0 - removes the specified phase
	#3subemote <which> swap <##1> <##2>#0 - swaps the order of two phases
	#3subemote <which> edit <##> <new text>#0 - changes the echo for an emote
	#3subemote <which> delay <##> <seconds>#0 - changes the delay for an emote phase";


	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "verb":
				return BuildingCommandVerb(actor, command);
			case "tool":
				return BuildingCommandTool(actor, command);
			case "skindiff":
			case "skindifficulty":
			case "skin_difficulty":
			case "skin difficulty":
				return BuildingCommandSkinDifficulty(actor, command);
			case "can":
			case "canprog":
			case "can prog":
			case "can_prog":
				return BuildingCommandCanProg(actor, command);
			case "why":
			case "whyprog":
			case "why prog":
			case "why_prog":
				return BuildingCommandWhyProg(actor, command);
			case "check":
				return BuildingCommandCheck(actor, command);
			case "skinemote":
			case "skin emote":
			case "skin_emote":
			case "emoteskin":
			case "emote_skin":
			case "emote skin":
				return BuildingCommandSkinEmote(actor, command);
			case "emote":
			case "mainemote":
			case "butcheremote":
			case "main emote":
			case "main_emote":
			case "butcher emote":
			case "butcher_emote":
				return BuildingCommandEmote(actor, command);
			case "subemote":
			case "sub_emote":
			case "sub emote":
			case "emotesub":
			case "emote_sub":
			case "emote sub":
				return BuildingCommandSubEmote(actor, command);
			case "product":
			case "prod":
				return BuildingCommandProduct(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this butchery profile to?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.RaceButcheryProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a butchery profile with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {_name.ColourName()} butchery profile to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which verb do you want to set for this butchery profile? The valid options are {"butcher".ColourCommand()} and {"salvage".ColourCommand()}.");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "butcher":
				Verb = ButcheryVerb.Butcher;
				break;
			case "salvage":
				Verb = ButcheryVerb.Salvage;
				break;
			default:
				actor.OutputHandler.Send(
					$"Which verb do you want to set for this butchery profile? The valid options are {"butcher".ColourCommand()} and {"salvage".ColourCommand()}.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"The verb used for this butchery profile is now {Verb.Describe().ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandTool(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which tool tag do you want to require people to have in order to {Verb.Describe().ColourCommand()} corpses and bodyparts with this profile?");
			return false;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		RequiredToolTag = tag;
		Changed = true;
		ToolTemplate = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				new InventoryPlanActionHold(Gameworld, RequiredToolTag.Id, 0, item => true, null, 1)
			})
		});
		actor.OutputHandler.Send(
			$"Players will now be required to have a tool item with the tag {RequiredToolTag.FullName.ColourName()} in order to {Verb.DescribeEnum().ColourCommand()} corpses and bodyparts with this profile.");
		return true;
	}

	private bool BuildingCommandSkinDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What difficulty should skinning corpses with this profile be?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		DifficultySkin = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"It is now {difficulty.Describe().ColourValue()} to skin corpses with this profile.");
		return true;
	}

	private bool BuildingCommandCanProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog would you like to set as the prog to control whether or not players can butcher corpses of this type?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes>
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a character and an item as arguments, and {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		CanButcherProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This butchery profile will now use the prog {prog.MXPClickableFunctionNameWithId()} to control whether people can butcher corpses with this profile.");
		return true;
	}

	private bool BuildingCommandWhyProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog would you like to set as the prog to give an error message when players cannot butcher corpses of this type?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Text))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a text value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes>
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a character and an item as arguments, and {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		WhyCannotButcherProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This butchery profile will now use the prog {prog.MXPClickableFunctionNameWithId()} to control error messages when people cannot butcher corpses with this profile.");
		return true;
	}

	private bool BuildingCommandCheck(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which subcategory do you want to specify a command check for? Use {"main".ColourValue()} for the non-subcategory version.");
			return false;
		}

		var subsystem = command.PopSpeech().ToLowerInvariant();
		if (subsystem.EqualTo("main"))
		{
			subsystem = string.Empty;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait should the check use?");
			return false;
		}

		if (!string.IsNullOrWhiteSpace(subsystem) && command.PeekSpeech().EqualToAny("none", "remove"))
		{
			_breakdownChecks.Remove(subsystem);
			_breakdownEmotes.Remove(subsystem);
			Changed = true;
			actor.OutputHandler.Send($"This profile will no longer have a {subsystem.ColourCommand()} subsystem.");
			return true;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.Last);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the check against {trait.Name.ColourName()} be?");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		_breakdownChecks[subsystem] = (trait, difficulty);
		actor.OutputHandler.Send(
			$"This profile will now impose a {difficulty.Describe().ColourValue()} check against {trait.Name.ColourValue()}{(string.IsNullOrWhiteSpace(subsystem) ? "" : $" when using the {subsystem} sub command")}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSkinEmote(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandSkinEmoteAdd(actor, command);
			case "remove":
			case "rem":
			case "del":
			case "delete":
				return BuildingCommandSkinEmoteRemove(actor, command);
			case "swap":
				return BuildingCommandSkinEmoteSwap(actor, command);
			case "edit":
			case "set":
			case "replace":
				return BuildingCommandSkinEmoteEdit(actor, command);
			case "delay":
			case "time":
			case "seconds":
			case "second":
				return BuildingCommandSkinEmoteDelay(actor, command);

			default:
				actor.OutputHandler.Send(
					$"Valid options are {new List<string> { "add", "remove", "swap", "edit", "delay" }.Select(x => x.ColourCommand()).ListToString()}\nUse $0 for the skinner, $1 for the corpse and $2 for the tool.");
				return false;
		}
	}

	private bool BuildingCommandSkinEmoteDelay(ICharacter actor, StringStack command)
	{
		if (!_skinEmotes.Any())
		{
			actor.OutputHandler.Send("There aren't any skin emotes currently set.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote do you want to edit the delay for?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value > _skinEmotes.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_skinEmotes.Count.ToString("N0", actor).ColourValue()} skin emotes to choose from.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many seconds do you want to set as the delay for that emote?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var dvalue) || dvalue <= 0.00)
		{
			actor.OutputHandler.Send("You must enter a valid number of seconds for the delay.");
			return false;
		}

		var old = _skinEmotes[value - 1].Emote;
		_skinEmotes[value - 1] = (old, dvalue);
		actor.OutputHandler.Send(
			$"The {value.ToOrdinal().ColourValue()} skinning emote now has a delay of {dvalue.ToString("N2", actor)}s.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSkinEmoteAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What delay do you want to put between this emote and the next?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"{command.Last.ColourCommand()} is not a valid number of seconds of delay for this emote.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What is the emote you want to add? Use $0 for the skinner, $1 for the corpse and $2 for the tool.");
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_skinEmotes.Add((emoteText, value));
		Changed = true;
		actor.OutputHandler.Send(
			$"You add a new skinning emote with a delay of {value.ToString("N2", actor).ColourValue()}s: {emoteText.ColourCommand().Fullstop()}");
		return true;
	}

	private bool BuildingCommandSkinEmoteRemove(ICharacter actor, StringStack command)
	{
		if (!_skinEmotes.Any())
		{
			actor.OutputHandler.Send("There aren't any skin emotes to remove.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which number skin emote do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value > _skinEmotes.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_skinEmotes.Count.ToString("N0", actor).ColourValue()} skin emotes to choose from.");
			return false;
		}

		var emote = _skinEmotes[value - 1];
		_skinEmotes.RemoveAt(value - 1);
		actor.OutputHandler.Send(
			$"You remove the {value.ToOrdinal().ColourValue()} skin emote, which was {emote.Emote.ColourCommand().Fullstop()}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSkinEmoteSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which number skin emote do you want to swap with another?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value1) || value1 < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value1 > _skinEmotes.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_skinEmotes.Count.ToString("N0", actor).ColourValue()} skin emotes to choose from.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which number skin emote do you want to swap the first one with?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value2) || value2 < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value2 > _skinEmotes.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_skinEmotes.Count.ToString("N0", actor).ColourValue()} skin emotes to choose from.");
			return false;
		}

		if (value1 == value2)
		{
			actor.OutputHandler.Send("You can't swap a skin emote with itself.");
			return false;
		}

		_skinEmotes.Swap(value1 - 1, value2 - 1);
		actor.OutputHandler.Send(
			$"You swap the positions of the {value1.ToOrdinal().ColourValue()} and {value2.ToOrdinal().ColourValue()} skin emotes.");
		return true;
	}

	private bool BuildingCommandSkinEmoteEdit(ICharacter actor, StringStack command)
	{
		if (!_skinEmotes.Any())
		{
			actor.OutputHandler.Send("There aren't any skin emotes currently set.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote do you want to edit the text of?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value > _skinEmotes.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_skinEmotes.Count.ToString("N0", actor).ColourValue()} skin emotes to choose from.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the new emote text that you want to set for that emote?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		var olddelay = _skinEmotes[value - 1].Delay;
		_skinEmotes[value - 1] = (emoteText, olddelay);
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the text of the {value.ToOrdinal().ColourValue()} skinning emote to {emoteText.ColourCommand().Fullstop()}");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandEmoteAdd(actor, command, string.Empty);
			case "remove":
			case "rem":
			case "del":
			case "delete":
				return BuildingCommandEmoteRemove(actor, command, string.Empty);
			case "swap":
				return BuildingCommandEmoteSwap(actor, command, string.Empty);
			case "edit":
			case "set":
			case "replace":
				return BuildingCommandEmoteEdit(actor, command, string.Empty);
			case "delay":
			case "time":
			case "seconds":
			case "second":
				return BuildingCommandEmoteDelay(actor, command, string.Empty);

			default:
				actor.OutputHandler.Send(
					$"Valid options are {new List<string> { "add", "remove", "swap", "edit", "delay" }.Select(x => x.ColourCommand()).ListToString()}\nUse $0 for the skinner, $1 for the corpse and $2 for the tool.");
				return false;
		}
	}

	private bool BuildingCommandSubEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which subsystem do you want to edit emotes for?");
			return false;
		}

		var subsystem = command.PopSpeech().ToLowerInvariant();
		if (!_breakdownChecks.ContainsKey(subsystem))
		{
			actor.OutputHandler.Send(
				$"There is no subsytem defined with a value of {subsystem.ColourCommand()}. You must first define a check for the subsystem before you can add emotes.");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandEmoteAdd(actor, command, subsystem);
			case "remove":
			case "rem":
			case "del":
			case "delete":
				return BuildingCommandEmoteRemove(actor, command, subsystem);
			case "swap":
				return BuildingCommandEmoteSwap(actor, command, subsystem);
			case "edit":
			case "set":
			case "replace":
				return BuildingCommandEmoteEdit(actor, command, subsystem);
			case "delay":
			case "time":
			case "seconds":
			case "second":
				return BuildingCommandEmoteDelay(actor, command, subsystem);

			default:
				actor.OutputHandler.Send(
					$"Valid options are {new List<string> { "add", "remove", "swap", "edit", "delay" }.Select(x => x.ColourCommand()).ListToString()}\nUse $0 for the skinner, $1 for the corpse and $2 for the tool.");
				return false;
		}

		return true;
	}

	private bool BuildingCommandEmoteAdd(ICharacter actor, StringStack command, string subsystem)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What delay do you want to put between this emote and the next?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"{command.Last.ColourCommand()} is not a valid number of seconds of delay for this emote.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What is the emote you want to add? Use $0 for the butcher, $1 for the corpse and $2 for the tool.");
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_breakdownEmotes.Add(subsystem, (emoteText, value));
		Changed = true;
		actor.OutputHandler.Send(
			$"You add a new breakdown emote{(string.IsNullOrWhiteSpace(subsystem) ? "" : $" for subsystem {subsystem.ColourCommand()}")} with a delay of {value.ToString("N2", actor).ColourValue()}s: {emoteText.ColourCommand().Fullstop()}");
		return true;
	}

	private bool BuildingCommandEmoteRemove(ICharacter actor, StringStack command, string subsystem)
	{
		if (!_breakdownEmotes[subsystem].Any())
		{
			actor.OutputHandler.Send("There aren't any breakdown emotes of that type to remove.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which number breakdown emote do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value > _breakdownEmotes[subsystem].Count())
		{
			actor.OutputHandler.Send(
				$"There are only {_breakdownEmotes[subsystem].Count().ToString("N0", actor).ColourValue()} emotes to choose from.");
			return false;
		}

		var emote = _breakdownEmotes[subsystem].ElementAt(value - 1);
		_breakdownEmotes.RemoveAt(subsystem, value - 1);
		actor.OutputHandler.Send(
			$"You remove the {value.ToOrdinal().ColourValue()} breakdown emote{(string.IsNullOrWhiteSpace(subsystem) ? "" : $" for subsystem {subsystem.ColourCommand()}")}, which was {emote.Emote.ColourCommand().Fullstop()}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEmoteSwap(ICharacter actor, StringStack command, string subsystem)
	{
		if (!_breakdownEmotes[subsystem].Any())
		{
			actor.OutputHandler.Send("There are no breakdown emotes of that type.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which number breakdown emote do you want to swap with another?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value1) || value1 < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value1 > _breakdownEmotes[subsystem].Count())
		{
			actor.OutputHandler.Send(
				$"There are only {_breakdownEmotes[subsystem].Count().ToString("N0", actor).ColourValue()} emotes to choose from.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which number skin emote do you want to swap the first one with?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value2) || value2 < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value2 > _breakdownEmotes[subsystem].Count())
		{
			actor.OutputHandler.Send(
				$"There are only {_breakdownEmotes[subsystem].Count().ToString("N0", actor).ColourValue()} emotes to choose from.");
			return false;
		}

		if (value1 == value2)
		{
			actor.OutputHandler.Send("You can't swap an emote with itself.");
			return false;
		}

		_breakdownEmotes.Swap(subsystem, value1 - 1, value2 - 1);
		actor.OutputHandler.Send(
			$"You swap the positions of the {value1.ToOrdinal().ColourValue()} and {value2.ToOrdinal().ColourValue()} breakdown emotes{(string.IsNullOrWhiteSpace(subsystem) ? "" : $" for subsystem {subsystem.ColourCommand()}")}.");
		return true;
	}

	private bool BuildingCommandEmoteEdit(ICharacter actor, StringStack command, string subsystem)
	{
		if (!_breakdownEmotes[subsystem].Any())
		{
			actor.OutputHandler.Send("There aren't any breakdown emotes of that type currently set.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote do you want to edit the text of?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value > _skinEmotes.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_breakdownEmotes[subsystem].Count().ToString("N0", actor).ColourValue()} emotes to choose from.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the new emote text that you want to set for that emote?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		var olddelay = _breakdownEmotes[subsystem].ElementAt(value - 1).Delay;
		_breakdownEmotes.SetValueAtIndex(subsystem, value - 1, (emoteText, olddelay));
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the text of the {value.ToOrdinal().ColourValue()} breakdown emote{(string.IsNullOrWhiteSpace(subsystem) ? "" : $" for subsystem {subsystem.ColourCommand()}")} to {emoteText.ColourCommand().Fullstop()}");
		return true;
	}

	private bool BuildingCommandEmoteDelay(ICharacter actor, StringStack command, string subsystem)
	{
		if (!_breakdownEmotes[subsystem].Any())
		{
			actor.OutputHandler.Send("There aren't any breakdown emotes of that type currently set.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote do you want to edit the delay for?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid emote number.");
			return false;
		}

		if (value > _skinEmotes.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_breakdownEmotes[subsystem].Count().ToString("N0", actor).ColourValue()} emotes to choose from.");
			return false;
		}


		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many seconds do you want to set as the delay for that emote?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var dvalue) || dvalue <= 0.00)
		{
			actor.OutputHandler.Send("You must enter a valid number of seconds for the delay.");
			return false;
		}

		var old = _breakdownEmotes[subsystem].ElementAt(value - 1).Emote;
		_breakdownEmotes.SetValueAtIndex(subsystem, value - 1, (old, dvalue));
		actor.OutputHandler.Send(
			$"The {value.ToOrdinal().ColourValue()} breakdown emote{(string.IsNullOrWhiteSpace(subsystem) ? "" : $" for subsystem {subsystem.ColourCommand()}")} now has a delay of {dvalue.ToString("N2", actor)}s.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandProduct(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which butchery product do you want to toggle as being included in this profile?");
			return false;
		}

		var product = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ButcheryProducts.Get(value)
			: Gameworld.ButcheryProducts.GetByName(command.Last);
		if (product == null)
		{
			actor.OutputHandler.Send("There is no such butchery product.");
			return false;
		}

		if (_products.Contains(product))
		{
			_products.Remove(product);
			actor.OutputHandler.Send(
				$"This butchery profile will no longer include the {product.Name.ColourName()} product.");
		}
		else
		{
			_products.Add(product);
			actor.OutputHandler.Send(
				$"This butchery profile will now include the {product.Name.ColourName()} product.");
		}

		Changed = true;
		return true;
	}

	/// <summary>
	/// Returns a builder-specific view of this race's butchery profile
	/// </summary>
	/// <param name="voyeur">The builder viewing the info</param>
	/// <returns>A textual representation of the butchery profile</returns>
	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Race Butchery Profile #{Id.ToString("N0", voyeur)} - {Name.ColourName()}");
		sb.AppendLine($"Verb: {Verb.Describe().ColourValue()}");
		sb.AppendLine($"Tool Tag: {RequiredToolTag?.Name.ColourName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Skin Difficulty: {DifficultySkin.Describe().ColourValue()}");
		sb.AppendLine(
			$"Can Butcher Prog: {CanButcherProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Why Can't Butcher Prog: {WhyCannotButcherProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		var i = 1;
		if (SkinEmotes.Any() || Products.Any(x => x.IsPelt))
		{
			sb.AppendLine();
			sb.AppendLine("Skinning".GetLineWithTitle(voyeur.LineFormatLength, voyeur.Account.UseUnicode,
				Telnet.Red,
				Telnet.BoldRed));
			sb.AppendLine();
			foreach (var emote in SkinEmotes)
			{
				sb.AppendLine(
					$"\t#{i++.ToString("N0", voyeur)}) [{emote.Delay.ToString("N2", voyeur)}s] {emote.Emote.ColourCommand()}");
			}

			sb.AppendLine();
			sb.AppendLine("Skin Products:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from item in _products.Where(x => x.IsPelt)
				select new List<string>
				{
					item.Id.ToString("N0", voyeur),
					item?.Name ?? "",
					item.CanProduceProg?.MXPClickableFunctionName() ?? "",
					item.TargetBody?.Name ?? "",
					item.RequiredBodyparts.Select(x => x.Name).ListToCommaSeparatedValues(", "),
					item.ProductItems.Select(x => $"{x.NormalQuantity.ToString("N0", voyeur)}x {x.NormalProto.EditHeader()}".ColourObject()).ListToCommaSeparatedValues(", ")
				},
				new List<string>
				{
					"Id",
					"Name",
					"Can Prog",
					"Body",
					"Bodyparts",
					"Produced"
				},
				voyeur
			));
		}

		sb.AppendLine();
		sb.AppendLine($"Main Breakdown".GetLineWithTitle(voyeur.LineFormatLength, voyeur.Account.UseUnicode, Telnet.Red,
			Telnet.BoldRed));
		sb.AppendLine();
		(ITraitDefinition Trait, Difficulty CheckDifficulty) check;
		if (_breakdownChecks.ContainsKey(string.Empty))
		{
			check = _breakdownChecks[string.Empty];
			sb.AppendLine($"Check vs {check.Trait.Name.ColourName()} @ {check.CheckDifficulty.Describe().ColourValue()}");
		}
		else
		{
			sb.AppendLine($"Check vs #1Unknown#0 @ #2Unknown#0".SubstituteANSIColour());
		}
		sb.AppendLine();
		i = 1;
		foreach (var emote in BreakdownEmotes(string.Empty))
		{
			sb.AppendLine(
				$"\t#{i++.ToString("N0", voyeur)}) [{emote.Delay.ToString("N2", voyeur)}s] {emote.Emote.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Products:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _products.Where(x => !x.IsPelt)
			select new List<string>
			{
				item.Id.ToString("N0", voyeur),
				item.Name,
				item.CanProduceProg?.MXPClickableFunctionName() ?? "",
				item.TargetBody?.Name ?? "",
				item.Subcategory ?? "",
				item.RequiredBodyparts.Select(x => x.Name).ListToCommaSeparatedValues(", "),
				item.ProductItems.Select(x => $"{x.NormalQuantity.ToString("N0", voyeur)}x {x.NormalProto.EditHeader()}".ColourObject()).ListToCommaSeparatedValues(", ")
			},
			new List<string>
			{
				"Id",
				"Name",
				"Can Prog",
				"Body",
				"Subcategory",
				"Bodyparts",
				"Produced"
			},
			voyeur
		));

		foreach (var category in _breakdownChecks.Keys)
		{
			if (string.IsNullOrWhiteSpace(category))
			{
				continue;
			}

			sb.AppendLine();
			sb.AppendLine($"Subcategory [{category}]".GetLineWithTitle(voyeur.LineFormatLength,
				voyeur.Account.UseUnicode, Telnet.Red,
				Telnet.BoldRed));
			check = _breakdownChecks[category];
			sb.AppendLine(
				$"Check vs {check.Trait.Name.ColourName()} @ {check.CheckDifficulty.Describe().ColourValue()}");
			sb.AppendLine();
			i = 1;
			foreach (var emote in BreakdownEmotes(category))
			{
				sb.AppendLine(
					$"\t#{i++.ToString("N0", voyeur)}) [{emote.Delay.ToString("N2", voyeur)}s] {emote.Emote.ColourCommand()}");
			}

			sb.AppendLine();
			sb.AppendLine("Products:");
			sb.AppendLine(StringUtilities.GetTextTable(
				from item in _products.Where(x => !x.IsPelt && x.Subcategory.EqualTo(category))
				select new List<string>
				{
					item.Id.ToString("N0", voyeur),
					item.Name,
					item.CanProduceProg.MXPClickableFunctionName(),
					item.TargetBody.Name,
					item.RequiredBodyparts.Select(x => x.Name).ListToCommaSeparatedValues(", "),
					item.ProductItems.Select(x => $"{x.NormalQuantity.ToString("N0", voyeur)}x {x.NormalProto.EditHeader()}".ColourObject()).ListToCommaSeparatedValues(", ")
				},
				new List<string>
				{
					"Id",
					"Name",
					"Can Prog",
					"Body",
					"Bodyparts",
					"Produced"
				},
				voyeur
			));
		}

		return sb.ToString();
	}
}