#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Arenas;

internal sealed class ArenaEventTypeSide : SaveableItem, IArenaEventTypeSide
{
	private readonly List<ICombatantClass> _eligibleClasses = new();

	public ArenaEventTypeSide(MudSharp.Models.ArenaEventTypeSide model, ArenaEventType parent,
		Func<long, ICombatantClass?> classLookup)
	{
		Gameworld = parent.Gameworld;
		EventType = parent;
		_id = model.Id;
		_name = $"Side {ArenaSideIndexUtilities.ToDisplayIndex(model.Index)}";
		Index = model.Index;
		Capacity = model.Capacity;
		Policy = (ArenaSidePolicy)model.Policy;
		MinimumRating = model.MinimumRating;
		MaximumRating = model.MaximumRating;
		AllowNpcSignup = model.AllowNpcSignup;
		AutoFillNpc = model.AutoFillNpc;
		OutfitProg = model.OutfitProgId.HasValue ? parent.Gameworld.FutureProgs.Get(model.OutfitProgId.Value) : null;
		NpcLoaderProg = model.NpcLoaderProgId.HasValue
			? parent.Gameworld.FutureProgs.Get(model.NpcLoaderProgId.Value)
			: null;

		_eligibleClasses.AddRange(model.ArenaEventTypeSideAllowedClasses
			.Select(x => classLookup(x.ArenaCombatantClassId))
			.OfType<ICombatantClass>());
	}

	public IArenaEventType EventType { get; }
	public override string FrameworkItemType => "ArenaEventTypeSide";
	public override string Name => $"Side {ArenaSideIndexUtilities.ToDisplayIndex(Index)}";
	public int Index { get; private set; }
	public int Capacity { get; private set; }
	public ArenaSidePolicy Policy { get; private set; }
	public decimal? MinimumRating { get; private set; }
	public decimal? MaximumRating { get; private set; }
	public IEnumerable<ICombatantClass> EligibleClasses => _eligibleClasses;
	public IFutureProg? OutfitProg { get; private set; }
	public bool AllowNpcSignup { get; private set; }
	public bool AutoFillNpc { get; private set; }
	public IFutureProg? NpcLoaderProg { get; private set; }

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"\tSide {ArenaSideIndexUtilities.ToDisplayString(actor, Index).ColourValue()} - Capacity {Capacity.ToString(actor).ColourValue()} ({Policy.DescribeEnum().ColourValue()})");
		sb.AppendLine();
		sb.AppendLine($"\t\tRating Range: {DescribeRatingRange(actor)}");
		sb.AppendLine($"\t\tAllow NPC Signup: {AllowNpcSignup.ToColouredString()}");
		sb.AppendLine($"\t\tAuto Fill NPC: {AutoFillNpc.ToColouredString()}");
		sb.AppendLine($"\t\tOutfit Prog: {OutfitProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"\t\tNPC Loader Prog: {NpcLoaderProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"\t\tEligible Classes: {(_eligibleClasses.Any() ? _eligibleClasses.Select(x => x.Name.ColourName()).ListToString() : "None".ColourError())}");
		return sb.ToString();
	}

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3capacity <number>#0 - sets the capacity for this side
	#3policy <policy>#0 - sets the signup policy
	#3rating <min> <max>|none#0 - sets the inclusive rating range for this side
	#3rating min|max <value>|none#0 - sets or clears one rating bound
	#3allownpc#0 - toggles whether NPCs may sign up
	#3autofill#0 - toggles whether NPCs auto-fill empty slots
	#3outfit <prog>|none#0 - sets an outfit prog
	#3npcloader <prog>|none#0 - sets an NPC loader prog
	#3class <class>#0 - toggles an eligible combatant class";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "capacity":
				return BuildingCommandCapacity(actor, command);
			case "policy":
				return BuildingCommandPolicy(actor, command);
			case "rating":
			case "ratings":
				return BuildingCommandRating(actor, command);
			case "npc":
			case "allownpc":
				return BuildingCommandAllowNpc(actor);
			case "autofill":
				return BuildingCommandAutoFill(actor);
			case "outfit":
			case "outfitprog":
				return BuildingCommandOutfit(actor, command);
			case "npcloader":
			case "loader":
				return BuildingCommandNpcLoader(actor, command);
			case "class":
			case "classes":
				return BuildingCommandClass(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What capacity should this side have?".SubstituteANSIColour());
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("Capacity must be a positive whole number.".ColourError());
			return false;
		}

		Capacity = value;
		Changed = true;
		actor.OutputHandler.Send($"This side now has a capacity of {Capacity.ToString(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPolicy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What policy should this side use? Valid options are {Enum.GetValues<ArenaSidePolicy>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ArenaSidePolicy>(out var policy))
		{
			actor.OutputHandler.Send(
				$"That is not a valid policy. Valid options are {Enum.GetValues<ArenaSidePolicy>().ListToColouredString()}.");
			return false;
		}

		Policy = policy;
		Changed = true;
		actor.OutputHandler.Send($"Policy is now {Policy.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRating(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Current rating range is {DescribeRatingRange(actor)}.\nUse #3rating <min> <max>#0, #3rating min|max <value>#0, or #3rating none#0."
					.SubstituteANSIColour());
			return false;
		}

		var first = command.PopForSwitch();
		switch (first)
		{
			case "none":
			case "clear":
			case "off":
			case "remove":
				MinimumRating = null;
				MaximumRating = null;
				Changed = true;
				actor.OutputHandler.Send("This side no longer has any arena rating restrictions.".Colour(Telnet.Green));
				return true;
			case "min":
			case "minimum":
				return BuildingCommandRatingBound(actor, command, true);
			case "max":
			case "maximum":
				return BuildingCommandRatingBound(actor, command, false);
		}

		if (!decimal.TryParse(first, out var min))
		{
			actor.OutputHandler.Send("You must enter numeric minimum and maximum rating values, or use #3rating none#0."
				.SubstituteANSIColour());
			return false;
		}

		if (command.IsFinished || !decimal.TryParse(command.PopSpeech(), out var max))
		{
			actor.OutputHandler.Send("You must supply both minimum and maximum rating values.".ColourError());
			return false;
		}

		if (min > max)
		{
			actor.OutputHandler.Send("Minimum rating cannot be greater than maximum rating.".ColourError());
			return false;
		}

		MinimumRating = min;
		MaximumRating = max;
		Changed = true;
		actor.OutputHandler.Send(
			$"Rating range is now {MinimumRating.Value.ToString("N2", actor).ColourValue()} to {MaximumRating.Value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRatingBound(ICharacter actor, StringStack command, bool isMinimum)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What {(isMinimum ? "minimum" : "maximum")} rating should this side require? Use #3none#0 to clear."
					.SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "off", "remove"))
		{
			if (isMinimum)
			{
				MinimumRating = null;
			}
			else
			{
				MaximumRating = null;
			}

			if (MinimumRating.HasValue && MaximumRating.HasValue && MinimumRating.Value > MaximumRating.Value)
			{
				if (isMinimum)
				{
					MaximumRating = MinimumRating;
				}
				else
				{
					MinimumRating = MaximumRating;
				}
			}

			Changed = true;
			actor.OutputHandler.Send($"Rating range is now {DescribeRatingRange(actor)}.");
			return true;
		}

		if (!decimal.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid numeric rating.".ColourError());
			return false;
		}

		var newMinimum = isMinimum ? value : MinimumRating;
		var newMaximum = isMinimum ? MaximumRating : value;
		if (newMinimum.HasValue && newMaximum.HasValue && newMinimum.Value > newMaximum.Value)
		{
			actor.OutputHandler.Send("Minimum rating cannot be greater than maximum rating.".ColourError());
			return false;
		}

		if (isMinimum)
		{
			MinimumRating = value;
		}
		else
		{
			MaximumRating = value;
		}

		Changed = true;
		actor.OutputHandler.Send($"Rating range is now {DescribeRatingRange(actor)}.");
		return true;
	}

	private bool BuildingCommandAllowNpc(ICharacter actor)
	{
		AllowNpcSignup = !AllowNpcSignup;
		Changed = true;
		actor.OutputHandler.Send($"NPC signups are now {(AllowNpcSignup ? "enabled" : "disabled")} for this side."
			.SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandAutoFill(ICharacter actor)
	{
		AutoFillNpc = !AutoFillNpc;
		Changed = true;
		actor.OutputHandler.Send($"Auto-fill NPCs is now {(AutoFillNpc ? "enabled" : "disabled")} for this side."
			.SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandOutfit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which outfit prog should be used? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			OutfitProg = null;
			Changed = true;
			actor.OutputHandler.Send("Outfit prog cleared.".SubstituteANSIColour());
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Void,
			ArenaProgParameters.SideOutfitParameterSets).LookupProg();
		if (prog == null)
		{
			return false;
		}

		OutfitProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"Outfit prog set to {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandNpcLoader(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which NPC loader prog should be used? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			NpcLoaderProg = null;
			Changed = true;
			actor.OutputHandler.Send("NPC loader prog cleared.".SubstituteANSIColour());
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
			ProgVariableTypes.Character | ProgVariableTypes.Collection, ArenaProgParameters.NpcLoaderParameterSets)
			.LookupProg();
		if (prog == null)
		{
			return false;
		}

		NpcLoaderProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"NPC loader prog set to {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandClass(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which combatant class do you want to toggle for this side?");
			return false;
		}

		var cls = EventType.Arena.CombatantClasses
			.FirstOrDefault(x => x.Name.EqualTo(command.SafeRemainingArgument));
		if (cls == null && long.TryParse(command.SafeRemainingArgument, out var id))
		{
			cls = EventType.Arena.CombatantClasses.FirstOrDefault(x => x.Id == id);
		}
		if (cls == null)
		{
			actor.OutputHandler.Send("There is no such combatant class in this arena.".ColourError());
			return false;
		}

		if (_eligibleClasses.Contains(cls))
		{
			_eligibleClasses.Remove(cls);
			Changed = true;
			actor.OutputHandler.Send($"{cls.Name.ColourName()} is no longer eligible for this side.");
		}
		else
		{
			_eligibleClasses.Add(cls);
			Changed = true;
			actor.OutputHandler.Send($"{cls.Name.ColourName()} is now eligible for this side.");
		}

		return true;
	}

	public override void Save()
	{
		if (!Changed)
		{
			return;
		}

		using (new FMDB())
		{
			var dbSide = FMDB.Context.ArenaEventTypeSides.Find(Id);
			if (dbSide == null)
			{
				return;
			}

			dbSide.Index = Index;
			dbSide.Capacity = Capacity;
			dbSide.Policy = (int)Policy;
			dbSide.MinimumRating = MinimumRating;
			dbSide.MaximumRating = MaximumRating;
			dbSide.AllowNpcSignup = AllowNpcSignup;
			dbSide.AutoFillNpc = AutoFillNpc;
			dbSide.OutfitProgId = OutfitProg?.Id;
			dbSide.NpcLoaderProgId = NpcLoaderProg?.Id;

			FMDB.Context.ArenaEventTypeSideAllowedClasses.RemoveRange(
				FMDB.Context.ArenaEventTypeSideAllowedClasses.Where(x => x.ArenaEventTypeSideId == dbSide.Id));
			foreach (var cls in _eligibleClasses)
			{
				FMDB.Context.ArenaEventTypeSideAllowedClasses.Add(new ArenaEventTypeSideAllowedClass
				{
					ArenaEventTypeSideId = dbSide.Id,
					ArenaCombatantClassId = cls.Id
				});
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	private string DescribeRatingRange(ICharacter actor)
	{
		return (MinimumRating, MaximumRating) switch
		{
			({ } min, { } max) => $"{min.ToString("N2", actor).ColourValue()} to {max.ToString("N2", actor).ColourValue()}",
			({ } min, null) => $"{min.ToString("N2", actor).ColourValue()} and above",
			(null, { } max) => $"{max.ToString("N2", actor).ColourValue()} and below",
			_ => "Any".Colour(Telnet.Green)
		};
	}
}
