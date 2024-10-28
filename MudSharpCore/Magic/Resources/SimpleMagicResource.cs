using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Magic.Resources;

public class SimpleMagicResource : BaseMagicResource
{
	public override IMagicResource Clone(string newName, string newShortName)
	{
		return new SimpleMagicResource(this, newName, newShortName);
	}

	public SimpleMagicResource(SimpleMagicResource rhs, string newName, string newShortName) : base(rhs, newName, newShortName)
	{
		ShouldStartWithResourceCharacterProg = rhs.ShouldStartWithResourceCharacterProg;
		ShouldStartWithResourceItemProg = rhs.ShouldStartWithResourceItemProg;
		ShouldStartWithResourceLocationProg = rhs.ShouldStartWithResourceLocationProg;
		StartingResourceAmountCharacterProg = rhs.StartingResourceAmountCharacterProg;
		StartingResourceAmountItemProg = rhs.StartingResourceAmountItemProg;
		StartingResourceAmountLocationProg = rhs.StartingResourceAmountLocationProg;
		ResourceCapProg = rhs.ResourceCapProg;

		using (new FMDB())
		{
			var dbitem = new Models.MagicResource
			{
				Name = newName,
				ShortName = newShortName,
				BottomColour = BottomColour,
				MidColour = MidColour,
				TopColour = TopColour,
				MagicResourceType = (int)ResourceType,
				Type = "simple",
				Definition = SaveDefinition()
			};
			FMDB.Context.MagicResources.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public SimpleMagicResource(Models.MagicResource resource, IFuturemud gameworld) : base(resource, gameworld)
	{
		var root = XElement.Parse(resource.Definition);
		ShouldStartWithResourceCharacterProg = Gameworld.FutureProgs.GetByIdOrName(root.Element("ShouldStartWithResourceCharacterProg")?.Value ?? "0");
		ShouldStartWithResourceItemProg = Gameworld.FutureProgs.GetByIdOrName(root.Element("ShouldStartWithResourceItemProg")?.Value ?? "0");
		ShouldStartWithResourceLocationProg = Gameworld.FutureProgs.GetByIdOrName(root.Element("ShouldStartWithResourceLocationProg")?.Value ?? "0");
		StartingResourceAmountCharacterProg = Gameworld.FutureProgs.GetByIdOrName(root.Element("StartingResourceAmountCharacterProg")?.Value ?? "0");
		StartingResourceAmountItemProg = Gameworld.FutureProgs.GetByIdOrName(root.Element("StartingResourceAmountItemProg")?.Value ?? "0");
		StartingResourceAmountLocationProg = Gameworld.FutureProgs.GetByIdOrName(root.Element("StartingResourceAmountLocationProg")?.Value ?? "0");
		ResourceCapProg = Gameworld.FutureProgs.GetByIdOrName(root.Element("ResourceCapProg")?.Value ?? "0");
	}

	public SimpleMagicResource(IFuturemud gameworld, string name, string shortName) : base(gameworld, name, shortName)
	{
		ShouldStartWithResourceCharacterProg = Gameworld.AlwaysFalseProg;
		ShouldStartWithResourceItemProg = Gameworld.AlwaysFalseProg;
		ShouldStartWithResourceLocationProg = Gameworld.AlwaysFalseProg;
		StartingResourceAmountCharacterProg = Gameworld.AlwaysZeroProg;
		StartingResourceAmountItemProg = Gameworld.AlwaysZeroProg;
		StartingResourceAmountLocationProg = Gameworld.AlwaysZeroProg;
		ResourceCapProg = Gameworld.AlwaysZeroProg;

		using (new FMDB())
		{
			var dbitem = new Models.MagicResource
			{
				Name = Name,
				ShortName = shortName,
				TopColour = TopColour,
				MidColour = MidColour,
				BottomColour = BottomColour,
				Type = "simple",
				MagicResourceType = (int)ResourceType,
				Definition = SaveDefinition()
			};
			FMDB.Context.MagicResources.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	#region Overrides of BaseMagicResource

	public IFutureProg ShouldStartWithResourceCharacterProg { get; set; }
	public IFutureProg ShouldStartWithResourceItemProg { get; set; }
	public IFutureProg ShouldStartWithResourceLocationProg { get; set; }
	public IFutureProg StartingResourceAmountCharacterProg { get; set; }
	public IFutureProg StartingResourceAmountItemProg { get; set; }
	public IFutureProg StartingResourceAmountLocationProg { get; set; }
	public IFutureProg ResourceCapProg { get; set; }

	public override bool ShouldStartWithResource(IHaveMagicResource thing)
	{
		switch (thing)
		{
			case ICharacter ch:
				return ShouldStartWithResourceCharacterProg?.ExecuteBool(ch) ?? false;
			case ICell cell:
				return ShouldStartWithResourceLocationProg?.ExecuteBool(cell) ?? false;
			case IGameItem gi:
				return ShouldStartWithResourceItemProg?.ExecuteBool(gi) ?? false;
		}

		return false;
	}

	public override double StartingResourceAmount(IHaveMagicResource thing)
	{
		switch (thing)
		{
			case ICharacter ch:
				return StartingResourceAmountCharacterProg?.ExecuteDouble(ch) ?? 0.0;
			case ICell cell:
				return StartingResourceAmountLocationProg?.ExecuteDouble(cell) ?? 0.0;
			case IGameItem gi:
				return StartingResourceAmountItemProg?.ExecuteDouble(gi) ?? 0.0;
		}

		return 0.0;
	}

	public override double ResourceCap(IHaveMagicResource thing)
	{
		return ResourceCapProg?.ExecuteDouble(0.0, thing) ?? 0.0;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Simple Magic Resource #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.BoldPink, Telnet.BoldWhite));
		sb.AppendLine($"Short Name: {ShortName.ColourValue()}");
		sb.AppendLine($"Resource Type: {ResourceType.GetSingleFlags().Select(x => x.DescribeEnum().ColourName()).ListToCommaSeparatedValues(", ")}");
		sb.AppendLine($"Classic Prompt: {BottomColour}||{MidColour}||{TopColour}||{Telnet.RESET}");
		sb.AppendLine();
		sb.AppendLine("Prog Information".GetLineWithTitle(actor, Telnet.BoldPink, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Should Characters Have: {ShouldStartWithResourceCharacterProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Should Items Have: {ShouldStartWithResourceItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Should Rooms Have: {ShouldStartWithResourceLocationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Characters Starting Amount: {StartingResourceAmountCharacterProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Items Starting Amount: {StartingResourceAmountItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Rooms Starting Amount: {StartingResourceAmountLocationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Resource Cap: {ResourceCapProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("StartingResourceAmountLocationProg", StartingResourceAmountLocationProg?.Id ?? 0),
			new XElement("StartingResourceAmountItemProg", StartingResourceAmountItemProg?.Id ?? 0),
			new XElement("StartingResourceAmountCharacterProg", StartingResourceAmountCharacterProg?.Id ?? 0),
			new XElement("ShouldStartWithResourceCharacterProg", ShouldStartWithResourceCharacterProg?.Id ?? 0),
			new XElement("ShouldStartWithResourceLocationProg", ShouldStartWithResourceLocationProg?.Id ?? 0),
			new XElement("ShouldStartWithResourceItemProg", ShouldStartWithResourceItemProg?.Id ?? 0),
			new XElement("ResourceCapProg", ResourceCapProg?.Id ?? 0)
		).ToString();
	}

	protected override string SubtypeHelpText => @"	#3cap <prog>#0 - sets the prog for resource caps
	#3characterstart <prog>#0 - sets the prog for character starting amounts
	#3itemstart <prog>#0 - sets the prog for item starting amounts
	#3roomstart <prog>#0 - sets the prog for room starting amounts
	#3characterhas <prog>#0 - sets the prog for if characters have this resource at all
	#3itemhas <prog>#0 - sets the prog for if items have this resource at all
	#3roomhas <prog>#0 - sets the prog for if rooms have this resource at all";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "cap":
			case "resourcecap":
			case "resourcecapprog":
			case "capprog":
			case "resourceprog":
				return BuildingCommandResourceCapProg(actor, command);
			case "characterstart":
			case "startcharacter":
			case "characterstartprog":
			case "startcharacterprog":
				return BuildingCommandCharacterStartProg(actor, command);
			case "itemstart":
			case "startitem":
			case "itemstartprog":
			case "startitemprog":
				return BuildingCommandItemStartProg(actor, command);
			case "roomstart":
			case "startroom":
			case "roomstartprog":
			case "startroomprog":
			case "locationstart":
			case "startlocation":
			case "locationstartprog":
			case "startlocationprog":
			case "cellstart":
			case "startcell":
			case "cellstartprog":
			case "startcellprog":
				return BuildingCommandRoomStartProg(actor, command);
			case "characterhas":
			case "hascharacter":
			case "characterhasprog":
			case "hascharacterprog":
				return BuildingCommandCharacterHasProg(actor, command);
			case "itemhas":
			case "hasitem":
			case "itemhasprog":
			case "hasitemprog":
				return BuildingCommandItemHasProg(actor, command);
			case "roomhas":
			case "hasroom":
			case "roomhasprog":
			case "hasroomprog":
			case "locationhas":
			case "haslocation":
			case "locationhasprog":
			case "haslocationprog":
			case "cellhas":
			case "hascell":
			case "cellhasprog":
			case "hascellprog":
				return BuildingCommandRoomHasProg(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandRoomHasProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether rooms have this resource?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ShouldStartWithResourceLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource will now use the prog {prog.MXPClickableFunctionName()} to control whether rooms have this resource.");
		return true;
	}

	private bool BuildingCommandItemHasProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether items have this resource?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ShouldStartWithResourceItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource will now use the prog {prog.MXPClickableFunctionName()} to control whether items have this resource.");
		return true;
	}

	private bool BuildingCommandCharacterHasProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether characters have this resource?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ShouldStartWithResourceCharacterProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource will now use the prog {prog.MXPClickableFunctionName()} to control whether characters have this resource.");
		return true;
	}

	private bool BuildingCommandRoomStartProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to set the starting amount of this resource for rooms?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Number, new[] { ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		StartingResourceAmountLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource will now use the prog {prog.MXPClickableFunctionName()} to control room starting resource amounts.");
		return true;
	}

	private bool BuildingCommandItemStartProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to set the starting amount of this resource for items?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Number, new[] { ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		StartingResourceAmountItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource will now use the prog {prog.MXPClickableFunctionName()} to control item starting resource amounts.");
		return true;
	}

	private bool BuildingCommandCharacterStartProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to set the starting amount of this resource for characters?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Number, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		StartingResourceAmountCharacterProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource will now use the prog {prog.MXPClickableFunctionName()} to control character starting resource amounts.");
		return true;
	}

	private bool BuildingCommandResourceCapProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to set the maximum amount of this resource?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Number, new[] { ProgVariableTypes.MagicResourceHaver }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ResourceCapProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource will now use the prog {prog.MXPClickableFunctionName()} to control maximum resource amounts.");
		return true;
	}

	#endregion
}