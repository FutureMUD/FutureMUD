using System.Runtime.InteropServices;
using System.Text;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Form.Shape;

public class EntityDescriptionPattern : SaveableItem, IEntityDescriptionPattern
{
	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.EntityDescriptionPattern
			{
				Type = (int)Type,
				ApplicabilityProgId = ApplicabilityProg?.Id,
				RelativeWeight = RelativeWeight,
				Pattern = Pattern
			};
			FMDB.Context.EntityDescriptionPatterns.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IEntityDescriptionPattern Clone()
	{
		return new EntityDescriptionPattern(this);
	}

	private EntityDescriptionPattern(EntityDescriptionPattern rhs)
	{
		Gameworld = rhs.Gameworld;
		Type = rhs.Type;
		Pattern = rhs.Pattern;
		ApplicabilityProg = rhs.ApplicabilityProg;
		RelativeWeight = rhs.RelativeWeight;
		DoDatabaseInsert();
	}

	public EntityDescriptionPattern(IFuturemud gameworld, string pattern, EntityDescriptionType type, IFutureProg prog)
	{
		Gameworld = gameworld;
		Pattern = pattern;
		Type = type;
		ApplicabilityProg = prog;
		RelativeWeight = 100;
		DoDatabaseInsert();
	}

	public EntityDescriptionPattern(MudSharp.Models.EntityDescriptionPattern pattern, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = pattern.Id;
		Type = (EntityDescriptionType)pattern.Type;
		Pattern = pattern.Pattern;
		ApplicabilityProg = pattern.ApplicabilityProgId.HasValue
			? Gameworld.FutureProgs.Get(pattern.ApplicabilityProgId.Value)
			: null;
		RelativeWeight = pattern.RelativeWeight;
	}

	public override string FrameworkItemType => "EntityDescriptionPattern";

	#region ISaveable Members

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.EntityDescriptionPatterns.Find(Id);
			dbitem.ApplicabilityProgId = ApplicabilityProg?.Id;
			dbitem.Type = (int)Type;
			dbitem.Pattern = Pattern;
			dbitem.RelativeWeight = RelativeWeight;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	#region IEntityDescriptionPattern Members

	/// <inheritdoc />
	public override string Name => Pattern;
	public EntityDescriptionType Type { get; protected set; }

	public IFutureProg ApplicabilityProg { get; protected set; }

	public bool IsValidSelection(ICharacterTemplate template)
	{
		return ApplicabilityProg?.Execute<bool?>(template) != false;
	}

	public bool IsValidSelection(ICharacter character)
	{
		return ApplicabilityProg?.Execute<bool?>(character) != false;
	}

	public string Pattern { get; protected set; }

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Description Pattern #{Id.ToStringN0(actor).GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite)}");
		sb.AppendLine();
		sb.AppendLine($"Type: {Type.Describe().ColourValue()}");
		sb.AppendLine($"Prog: {ApplicabilityProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Weight: {RelativeWeight.ToStringN0Colour(actor)}");
		sb.AppendLine("Pattern: ");
		sb.AppendLine();
		sb.AppendLine(
			Type == EntityDescriptionType.FullDescription ?
			Pattern.Wrap(actor.InnerLineFormatLength, "\t") :
			Pattern
			);
		return sb.ToString();
	}

	public int RelativeWeight { get; protected set; }

	#endregion

	#region Overrides of FrameworkItem

	public override string ToString()
	{
		return Type == EntityDescriptionType.FullDescription
			? $"FDesc Pattern #{Id:N0} - {ApplicabilityProg?.FunctionName ?? "No Prog"}"
			: $"SDesc Pattern #{Id:N0} - {Pattern} - {ApplicabilityProg?.FunctionName ?? "No Prog"}";
	}

	#endregion

	public const string BuildingCommandHelp = @"You can use the following options with this command:

	#3type#0 - toggles this being a short description or full description pattern
	#3prog <prog>#0 - sets the prog which controls if this pattern is valid for a character
	#3pattern <text>#0 - sets the pattern text
	#3pattern#0 - drops you into an editor with extended markup help info to enter the new pattern
	#3weight <##>#0 - sets the relative weight for randomly selected descriptions";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "type":
				return BuildingCommandType(actor);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "pattern":
				return BuildingCommandPattern(actor, command);
			case "weight":
				return BuildingCommandWeight(actor, command);
			default:
				actor.OutputHandler.Send(BuildingCommandHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the relative weight of this pattern against others that might be randomly chosen?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number greater than zero.");
			return false;
		}

		RelativeWeight = value;
		Changed = true;
		actor.OutputHandler.Send($"This pattern will now have a relative weight of {value.ToStringN0Colour(actor)} for random selection.");
		return true;
	}

	private bool BuildingCommandPattern(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Replacing:\n\n");
			sb.AppendLine(Pattern.Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
			sb.AppendLine(EntityDescriptionPatternExtensions.GetDescriptionHelpNoTemplate());
			sb.AppendLine();
			sb.AppendLine("Enter your new pattern below: ");
			actor.EditorMode((text, handler, _) =>
				{
					Pattern = text;
					handler.Send($"The pattern is now:\n\n{Pattern.ColourCommand()}");
					Changed = true;
				},
			(handler, _) =>
			{
				handler.Send("You decide not to change the pattern.");
			});
			return true;
		}

		Pattern = command.SafeRemainingArgument;
		actor.OutputHandler.Send($"The pattern is now:\n\n{Pattern.ColourCommand()}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether this is a valid pattern for a character?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean, [FutureProgVariableTypes.Toon]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ApplicabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This pattern will now use the {prog.MXPClickableFunctionName()} function to control whether it applies for a character.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor)
	{
		if (Type == EntityDescriptionType.FullDescription)
		{
			Type = EntityDescriptionType.ShortDescription;
			actor.OutputHandler.Send("This is now a pattern for short descriptions.");
		}
		else
		{
			Type = EntityDescriptionType.FullDescription;
			actor.OutputHandler.Send("This is now a pattern for full descriptions.");
		}

		Changed = true;
		return true;
	}
}