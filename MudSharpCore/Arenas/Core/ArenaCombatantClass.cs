#nullable enable

using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Database;

namespace MudSharp.Arenas;

public sealed class ArenaCombatantClass : SaveableItem, ICombatantClass
{
	public ArenaCombatantClass(MudSharp.Models.ArenaCombatantClass model, CombatArena arena)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		_id = model.Id;
		_name = model.Name;
		Description = model.Description;
		EligibilityProg = Gameworld.FutureProgs.Get(model.EligibilityProgId);
		AdminNpcLoaderProg = model.AdminNpcLoaderProgId.HasValue
			? Gameworld.FutureProgs.Get(model.AdminNpcLoaderProgId.Value)
			: null;
		ResurrectNpcOnDeath = model.ResurrectNpcOnDeath;
		DefaultStageNameTemplate = string.IsNullOrWhiteSpace(model.DefaultStageNameTemplate)
			? null
			: model.DefaultStageNameTemplate;
		DefaultSignatureColour = string.IsNullOrWhiteSpace(model.DefaultSignatureColour)
			? null
			: model.DefaultSignatureColour;
	}

	public ArenaCombatantClass(CombatArena arena, string name, IFutureProg eligibilityProg)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		_name = name;
		Description = string.Empty;
		EligibilityProg = eligibilityProg;
		ResurrectNpcOnDeath = false;

		using (new FMDB())
		{
			var dbItem = new MudSharp.Models.ArenaCombatantClass
			{
				ArenaId = arena.Id,
				Name = name,
				Description = string.Empty,
				EligibilityProgId = eligibilityProg.Id,
				AdminNpcLoaderProgId = null,
				ResurrectNpcOnDeath = false,
				DefaultStageNameTemplate = null,
				DefaultSignatureColour = null
			};
			FMDB.Context.ArenaCombatantClasses.Add(dbItem);
			FMDB.Context.SaveChanges();
			_id = dbItem.Id;
		}

		arena.AddCombatantClass(this);
	}

	public CombatArena Arena { get; }
	ICombatArena ICombatantClass.Arena => Arena;
	public IFutureProg EligibilityProg { get; private set; }
	public IFutureProg? AdminNpcLoaderProg { get; private set; }
	public bool ResurrectNpcOnDeath { get; private set; }
	public string? DefaultStageNameTemplate { get; private set; }
	public string? DefaultSignatureColour { get; private set; }
	public string Description { get; private set; } = string.Empty;

	public override string FrameworkItemType => "ArenaCombatantClass";

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Combatant Class #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Arena: {Arena.Name.ColourName()}");
		sb.AppendLine($"Eligibility Prog: {EligibilityProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Admin NPC Loader Prog: {AdminNpcLoaderProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Resurrect NPC On Death: {ResurrectNpcOnDeath.ToColouredString()}");
		sb.AppendLine(
			$"Default Stage Name: {(string.IsNullOrWhiteSpace(DefaultStageNameTemplate) ? "None".ColourError() : DefaultStageNameTemplate.ColourName())}");
		sb.AppendLine(
			$"Signature Colour: {(string.IsNullOrWhiteSpace(DefaultSignatureColour) ? "None".ColourError() : DefaultSignatureColour.ColourValue())}");
		if (!string.IsNullOrWhiteSpace(Description))
		{
			sb.AppendLine();
			sb.AppendLine(Description);
		}

		return sb.ToString();
	}

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this combatant class
	#3description <text>#0 - sets a description for this class
	#3eligibility <prog>#0 - sets the eligibility prog (boolean, character input)
	#3loader <prog>|none#0 - sets an admin NPC loader prog
	#3resurrect#0 - toggles whether NPCs are resurrected on death
	#3stagename <text>|clear#0 - sets a default stage name template
	#3sigcolour <colour>|clear#0 - sets a default signature colour";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "eligibility":
				return BuildingCommandEligibility(actor, command);
			case "loader":
				return BuildingCommandLoader(actor, command);
			case "resurrect":
				return BuildingCommandResurrect(actor);
			case "stagename":
			case "stage":
				return BuildingCommandStageName(actor, command);
			case "sigcolour":
			case "sigcolor":
			case "signature":
				return BuildingCommandSignatureColour(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this combatant class?".SubstituteANSIColour());
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Arena.CombatantClasses.Any(x => !ReferenceEquals(x, this) && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a combatant class called {name.ColourName()} in this arena.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This combatant class is now called {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should this combatant class have?".SubstituteANSIColour());
			return false;
		}

		Description = command.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send("You update the description of this combatant class.".Colour(Telnet.Green));
		return true;
	}

	private bool BuildingCommandEligibility(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control eligibility? It must return a boolean and take a character.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [ProgVariableTypes.Character]).LookupProg();
		if (prog == null)
		{
			return false;
		}

		EligibilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"Eligibility is now controlled by the prog {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandLoader(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to load admin NPCs? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			AdminNpcLoaderProg = null;
			Changed = true;
			actor.OutputHandler.Send("This combatant class will no longer use an admin NPC loader prog.".SubstituteANSIColour());
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Character, [ProgVariableTypes.Location]).LookupProg();
		if (prog == null)
		{
			return false;
		}

		AdminNpcLoaderProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"Admin NPC loader prog set to {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandResurrect(ICharacter actor)
	{
		ResurrectNpcOnDeath = !ResurrectNpcOnDeath;
		Changed = true;
		actor.OutputHandler.Send(
			$"NPCs in this combatant class will {(ResurrectNpcOnDeath ? "now" : "no longer")} be resurrected on death.");
		return true;
	}

	private bool BuildingCommandStageName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What default stage name template should this class use? Use #3clear#0 to remove.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove"))
		{
			DefaultStageNameTemplate = null;
			Changed = true;
			actor.OutputHandler.Send("This combatant class no longer has a default stage name template.".SubstituteANSIColour());
			return true;
		}

		DefaultStageNameTemplate = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Default stage name template set to {DefaultStageNameTemplate.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSignatureColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What signature colour should this class use? Use #3clear#0 to remove.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove"))
		{
			DefaultSignatureColour = null;
			Changed = true;
			actor.OutputHandler.Send("This combatant class no longer has a default signature colour.".SubstituteANSIColour());
			return true;
		}

		DefaultSignatureColour = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Signature colour set to {DefaultSignatureColour.ColourValue()}.");
		return true;
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbItem = FMDB.Context.ArenaCombatantClasses.Find(Id);
			if (dbItem == null)
			{
				return;
			}

			dbItem.Name = Name;
			dbItem.Description = Description ?? string.Empty;
			dbItem.EligibilityProgId = EligibilityProg.Id;
			dbItem.AdminNpcLoaderProgId = AdminNpcLoaderProg?.Id;
			dbItem.ResurrectNpcOnDeath = ResurrectNpcOnDeath;
			dbItem.DefaultStageNameTemplate = DefaultStageNameTemplate;
			dbItem.DefaultSignatureColour = DefaultSignatureColour;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}
}
