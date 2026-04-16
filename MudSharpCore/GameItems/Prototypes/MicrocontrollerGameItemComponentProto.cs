#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public sealed class MicrocontrollerInputDefinition
{
	public MicrocontrollerInputDefinition(string variableName, string sourceComponentName)
	{
		VariableName = variableName;
		SourceComponentName = sourceComponentName;
	}

	public string VariableName { get; }
	public string SourceComponentName { get; }

	public XElement SaveToXml()
	{
		return new XElement("Input",
			new XAttribute("variable", VariableName),
			new XAttribute("source", SourceComponentName));
	}

	public static MicrocontrollerInputDefinition LoadFromXml(XElement element)
	{
		return new MicrocontrollerInputDefinition(
			element.Attribute("variable")?.Value ?? string.Empty,
			element.Attribute("source")?.Value ?? string.Empty);
	}
}

public class MicrocontrollerGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:
	All powered-machine options, plus:
	input add <variable> <sourcecomponent> - adds an input variable bound to a sibling signal source component
	input remove <variable> - removes an input binding
	logic - edits the controller logic in the multiline editor
	logic <text> - sets the controller logic directly

Notes:
	The controller logic must compile as a computer function and return a number.
	Input variable names must be valid FutureProg variable names and are automatically lower-cased.";

	private readonly List<MicrocontrollerInputDefinition> _inputs = [];
	private IFutureProg? _compiledLogic;

	public MicrocontrollerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Microcontroller")
	{
		LogicText = "return 0";
		CompileError = string.Empty;
		CompileControllerLogic();
	}

	protected MicrocontrollerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IReadOnlyList<MicrocontrollerInputDefinition> Inputs => _inputs;
	public string LogicText { get; protected set; } = string.Empty;
	public string CompileError { get; protected set; } = string.Empty;
	internal IFutureProg? CompiledLogic => _compiledLogic;
	public override string TypeDescription => "Microcontroller";

	protected override string ComponentDescriptionOLCByline => "This item is a programmable powered microcontroller";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		var inputs = !_inputs.Any()
			? "None".ColourError()
			: _inputs.Select(x =>
					$"{x.VariableName.ColourCommand()} <- {x.SourceComponentName.ColourName()}")
				.ListToString();

		var compileStatus = string.IsNullOrEmpty(CompileError)
			? "Compiled successfully".ColourValue()
			: CompileError.ColourError();

		return
			$"Inputs: {inputs}\nCompile Status: {compileStatus}\nLogic:\n{LogicText.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		_inputs.Clear();
		foreach (var element in root.Elements("Input"))
		{
			_inputs.Add(MicrocontrollerInputDefinition.LoadFromXml(element));
		}

		LogicText = root.Element("LogicText")?.Value ?? "return 0";
		CompileControllerLogic();
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(_inputs.Select(x => x.SaveToXml()));
		root.Add(new XElement("LogicText", new XCData(LogicText)));
		return root;
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "input":
			case "inputs":
				return BuildingCommandInput(actor, command);
			case "logic":
			case "code":
				return BuildingCommandLogic(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandInput(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove an input binding?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandInputAdd(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandInputRemove(actor, command);
			default:
				actor.Send("Do you want to add or remove an input binding?");
				return false;
		}
	}

	private bool BuildingCommandInputAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What variable name should this input use in the controller logic?");
			return false;
		}

		var variableName = command.PopSpeech().ToLowerInvariant();
		if (!MicrocontrollerLogicCompiler.IsValidVariableName(variableName))
		{
			actor.Send("That is not a valid variable name.");
			return false;
		}

		if (_inputs.Any(x => x.VariableName.Equals(variableName, System.StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already an input bound to that variable name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("Which sibling signal source component name should feed that input?");
			return false;
		}

		var sourceComponentName = command.SafeRemainingArgument.Trim();
		_inputs.Add(new MicrocontrollerInputDefinition(variableName, sourceComponentName));
		Changed = true;
		CompileControllerLogic();
		actor.Send(
			$"This microcontroller now binds input variable {variableName.ColourCommand()} to the sibling signal source component {sourceComponentName.ColourName()}.");
		ShowCompileResult(actor);
		return true;
	}

	private bool BuildingCommandInputRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which input variable do you want to remove?");
			return false;
		}

		var variableName = command.PopSpeech();
		var input = _inputs.FirstOrDefault(x =>
			x.VariableName.Equals(variableName, System.StringComparison.InvariantCultureIgnoreCase));
		if (input is null)
		{
			actor.Send("There is no such input binding.");
			return false;
		}

		_inputs.Remove(input);
		Changed = true;
		CompileControllerLogic();
		actor.Send($"You remove the input variable {input.VariableName.ColourCommand()}.");
		ShowCompileResult(actor);
		return true;
	}

	private bool BuildingCommandLogic(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Enter the microcontroller logic in the editor below. It must compile as a computer function and return a number.");
			actor.EditorMode((text, handler, _) =>
				{
					LogicText = text;
					Changed = true;
					CompileControllerLogic();
					handler.Send("You update the microcontroller logic.");
					handler.Send($"\nCompile Result: {(string.IsNullOrEmpty(CompileError) ? "Success".ColourValue() : CompileError.ColourError())}");
				},
				(handler, _) => { handler.Send("You decide not to change the microcontroller logic."); },
				1.0,
				recallText: LogicText);
			return true;
		}

		LogicText = command.SafeRemainingArgument;
		Changed = true;
		CompileControllerLogic();
		actor.Send("You update the microcontroller logic.");
		ShowCompileResult(actor);
		return true;
	}

	private void ShowCompileResult(ICharacter actor)
	{
		actor.Send(string.IsNullOrEmpty(CompileError)
			? "The microcontroller logic compiled successfully.".ColourValue()
			: $"The microcontroller logic did not compile: {CompileError.ColourError()}");
	}

	private void CompileControllerLogic()
	{
		(_compiledLogic, CompileError) = MicrocontrollerLogicCompiler.Compile(
			Gameworld,
			$"microcontroller_{Id}_{RevisionNumber}",
			_inputs.Select(x => x.VariableName),
			LogicText);
	}

	public override bool CanSubmit()
	{
		return string.IsNullOrEmpty(CompileError) && _compiledLogic is not null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (!string.IsNullOrEmpty(CompileError) || _compiledLogic is null)
		{
			return $"The microcontroller logic does not currently compile: {CompileError}";
		}

		return base.WhyCannotSubmit();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("microcontroller", true,
			(gameworld, account) => new MicrocontrollerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("mcu", false,
			(gameworld, account) => new MicrocontrollerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Microcontroller",
			(proto, gameworld) => new MicrocontrollerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Microcontroller",
			$"A {"[powered]".Colour(Telnet.Magenta)} controller that evaluates inline computer-function logic from sibling signal inputs",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new MicrocontrollerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MicrocontrollerGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MicrocontrollerGameItemComponentProto(proto, gameworld));
	}
}
