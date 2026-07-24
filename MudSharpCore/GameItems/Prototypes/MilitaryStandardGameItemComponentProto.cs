#nullable enable

using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class MilitaryStandardGameItemComponentProto : GameItemComponentProto, IMilitaryStandardPrototype
{
	protected MilitaryStandardGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "MilitaryStandard")
	{
		Family = MilitaryStandardFamily.InfantryColour;
		IdentityKey = "unassigned";
		IdentityName = "an unassigned military standard";
		Design = "It bears no assigned insignia.";
		RecognitionDifficulty = Difficulty.Normal;
		PlantEmote = "@ plant|plants $1 firmly in place.";
		TakeUpEmote = "@ take|takes up $1.";
		RecogniseEmote = "@ study|studies $1 and recognises its identity.";
		Changed = true;
	}

	protected MilitaryStandardGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public override string TypeDescription => "MilitaryStandard";
	public MilitaryStandardFamily Family { get; private set; }
	public string IdentityKey { get; private set; } = string.Empty;
	public string IdentityName { get; private set; } = string.Empty;
	public string Design { get; private set; } = string.Empty;
	public MilitaryStandardAssociationType AssociationType { get; private set; }
	public string AssociationKey { get; private set; } = string.Empty;
	public string AssociationName { get; private set; } = string.Empty;
	public ITraitDefinition? RecognitionTrait { get; private set; }
	public Difficulty RecognitionDifficulty { get; private set; }
	public List<string> SignalPatterns { get; } = [];
	public string PlantEmote { get; private set; } = string.Empty;
	public string TakeUpEmote { get; private set; } = string.Empty;
	public string RecogniseEmote { get; private set; } = string.Empty;
	public IFutureProg? CanBearProg { get; private set; }
	public IFutureProg? CanRecogniseProg { get; private set; }
	public IFutureProg? OnRecogniseProg { get; private set; }
	public IFutureProg? OnPlantProg { get; private set; }
	public IFutureProg? OnTakeUpProg { get; private set; }
	public IFutureProg? OnCapturedProg { get; private set; }
	public IFutureProg? OnRecoveredProg { get; private set; }
	public IFutureProg? OnCustodyChangedProg { get; private set; }
	public IFutureProg? OnSignalProg { get; private set; }

	protected override void LoadFromXml(XElement root)
	{
		Family = root.Element("Family")?.Value.TryParseEnum<MilitaryStandardFamily>(out var family) == true
			? family
			: MilitaryStandardFamily.InfantryColour;
		IdentityKey = root.Element("IdentityKey")?.Value ?? "unassigned";
		IdentityName = root.Element("IdentityName")?.Value ?? "an unassigned military standard";
		Design = root.Element("Design")?.Value ?? "It bears no assigned insignia.";
		AssociationType =
			root.Element("AssociationType")?.Value.TryParseEnum<MilitaryStandardAssociationType>(out var association) ==
			true
				? association
				: MilitaryStandardAssociationType.None;
		AssociationKey = root.Element("AssociationKey")?.Value ?? string.Empty;
		AssociationName = root.Element("AssociationName")?.Value ?? string.Empty;
		RecognitionTrait = Gameworld.Traits.Get(long.Parse(root.Element("RecognitionTrait")?.Value ?? "0"));
		RecognitionDifficulty =
			root.Element("RecognitionDifficulty")?.Value.TryParseEnum<Difficulty>(out var difficulty) == true
				? difficulty
				: Difficulty.Normal;
		SignalPatterns.Clear();
		SignalPatterns.AddRange(root.Element("Signals")?.Elements("Signal").Select(x => x.Value)
			.Distinct(StringComparer.OrdinalIgnoreCase) ?? Enumerable.Empty<string>());
		PlantEmote = root.Element("PlantEmote")?.Value ?? "@ plant|plants $1 firmly in place.";
		TakeUpEmote = root.Element("TakeUpEmote")?.Value ?? "@ take|takes up $1.";
		RecogniseEmote = root.Element("RecogniseEmote")?.Value ??
		                "@ study|studies $1 and recognises its identity.";
		CanBearProg = GetProg(root, "CanBearProg");
		CanRecogniseProg = GetProg(root, "CanRecogniseProg");
		OnRecogniseProg = GetProg(root, "OnRecogniseProg");
		OnPlantProg = GetProg(root, "OnPlantProg");
		OnTakeUpProg = GetProg(root, "OnTakeUpProg");
		OnCapturedProg = GetProg(root, "OnCapturedProg");
		OnRecoveredProg = GetProg(root, "OnRecoveredProg");
		OnCustodyChangedProg = GetProg(root, "OnCustodyChangedProg");
		OnSignalProg = GetProg(root, "OnSignalProg");
	}

	private IFutureProg? GetProg(XElement root, string name)
	{
		return Gameworld.FutureProgs.Get(long.Parse(root.Element(name)?.Value ?? "0"));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Family", Family),
			new XElement("IdentityKey", new XCData(IdentityKey)),
			new XElement("IdentityName", new XCData(IdentityName)),
			new XElement("Design", new XCData(Design)),
			new XElement("AssociationType", AssociationType),
			new XElement("AssociationKey", new XCData(AssociationKey)),
			new XElement("AssociationName", new XCData(AssociationName)),
			new XElement("RecognitionTrait", RecognitionTrait?.Id ?? 0),
			new XElement("RecognitionDifficulty", RecognitionDifficulty),
			new XElement("Signals", SignalPatterns.Select(x => new XElement("Signal", new XCData(x)))),
			new XElement("PlantEmote", new XCData(PlantEmote)),
			new XElement("TakeUpEmote", new XCData(TakeUpEmote)),
			new XElement("RecogniseEmote", new XCData(RecogniseEmote)),
			new XElement("CanBearProg", CanBearProg?.Id ?? 0),
			new XElement("CanRecogniseProg", CanRecogniseProg?.Id ?? 0),
			new XElement("OnRecogniseProg", OnRecogniseProg?.Id ?? 0),
			new XElement("OnPlantProg", OnPlantProg?.Id ?? 0),
			new XElement("OnTakeUpProg", OnTakeUpProg?.Id ?? 0),
			new XElement("OnCapturedProg", OnCapturedProg?.Id ?? 0),
			new XElement("OnRecoveredProg", OnRecoveredProg?.Id ?? 0),
			new XElement("OnCustodyChangedProg", OnCustodyChangedProg?.Id ?? 0),
			new XElement("OnSignalProg", OnSignalProg?.Id ?? 0)).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MilitaryStandardGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MilitaryStandardGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MilitaryStandardGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("militarystandard", true,
			(gameworld, account) => new MilitaryStandardGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("military standard", false,
			(gameworld, account) => new MilitaryStandardGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("MilitaryStandard",
			(proto, gameworld) => new MilitaryStandardGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("MilitaryStandard",
			"An identifiable, plantable military objective with persistent capture custody",
			BuildingHelpText);
	}

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3family <family>#0 - sets the standard family
	#3identity <key> <display name>#0 - sets its default unique identity
	#3design <description>#0 - sets the heraldic or visual design
	#3association none|unit|ship [key] [name]#0 - sets the default association
	#3recognition <trait>|automatic <difficulty>#0 - sets recognition checking
	#3signal <pattern>|clear#0 - toggles a visual signal pattern
	#3plantemote|takeupemote|recogniseemote <emote>#0 - sets action output
	#3canbear|canrecognise|onrecognise|onplant|ontakeup <prog>|clear#0
	#3oncaptured|onrecovered|oncustody|onsignal <prog>|clear#0";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopSpeech().ToLowerInvariant();
		switch (verb)
		{
			case "family":
				return SetEnum(actor, command, value => Family = value);
			case "identity":
				return BuildingCommandIdentity(actor, command);
			case "design":
				return SetText(actor, command, value => Design = value, "design");
			case "association":
				return BuildingCommandAssociation(actor, command);
			case "recognition":
				return BuildingCommandRecognition(actor, command);
			case "signal":
				return ToggleSignal(actor, command);
			case "plantemote":
				return SetText(actor, command, value => PlantEmote = value, "plant emote");
			case "takeupemote":
				return SetText(actor, command, value => TakeUpEmote = value, "take-up emote");
			case "recogniseemote":
			case "recognizeemote":
				return SetText(actor, command, value => RecogniseEmote = value, "recognition emote");
			case "canbear":
				return SetProg(actor, command, ProgVariableTypes.Boolean,
					[ProgVariableTypes.Character, ProgVariableTypes.Item], value => CanBearProg = value, "CanBear");
			case "canrecognise":
			case "canrecognize":
				return SetProg(actor, command, ProgVariableTypes.Boolean,
					[ProgVariableTypes.Character, ProgVariableTypes.Item], value => CanRecogniseProg = value,
					"CanRecognise");
			case "onrecognise":
			case "onrecognize":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item], value => OnRecogniseProg = value,
					"OnRecognise");
			case "onplant":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item], value => OnPlantProg = value, "OnPlant");
			case "ontakeup":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item], value => OnTakeUpProg = value, "OnTakeUp");
			case "oncaptured":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number],
					value => OnCapturedProg = value, "OnCaptured");
			case "onrecovered":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number],
					value => OnRecoveredProg = value, "OnRecovered");
			case "oncustody":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text,
						ProgVariableTypes.Text], value => OnCustodyChangedProg = value, "OnCustodyChanged");
			case "onsignal":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text],
					value => OnSignalProg = value, "OnSignal");
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool SetEnum(ICharacter actor, StringStack command, Action<MilitaryStandardFamily> setter)
	{
		if (!command.SafeRemainingArgument.TryParseEnum<MilitaryStandardFamily>(out var value))
		{
			actor.OutputHandler.Send("That is not a valid military standard family.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.OutputHandler.Send($"This is now a {value.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandIdentity(ICharacter actor, StringStack command)
	{
		var key = command.PopSpeech();
		if (string.IsNullOrWhiteSpace(key) || command.IsFinished)
		{
			actor.OutputHandler.Send("Specify an identity key and a display name.");
			return false;
		}

		IdentityKey = key;
		IdentityName = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The default identity is now {IdentityName.ColourName()} ({IdentityKey}).");
		return true;
	}

	private bool BuildingCommandAssociation(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().TryParseEnum<MilitaryStandardAssociationType>(out var type))
		{
			actor.OutputHandler.Send("Specify none, unit or ship.");
			return false;
		}

		AssociationType = type;
		if (type == MilitaryStandardAssociationType.None)
		{
			AssociationKey = string.Empty;
			AssociationName = string.Empty;
		}
		else
		{
			AssociationKey = command.PopSpeech();
			AssociationName = command.SafeRemainingArgument;
			if (string.IsNullOrWhiteSpace(AssociationKey) || string.IsNullOrWhiteSpace(AssociationName))
			{
				actor.OutputHandler.Send("Specify an association key and display name.");
				return false;
			}
		}

		Changed = true;
		actor.OutputHandler.Send("The default association has been updated.");
		return true;
	}

	private bool BuildingCommandRecognition(ICharacter actor, StringStack command)
	{
		var traitText = command.PopSpeech();
		var trait = traitText.EqualTo("automatic") || traitText.EqualTo("none")
			? null
			: Gameworld.Traits.GetByIdOrName(traitText);
		if (trait is not null && trait.TraitType != TraitType.Skill)
		{
			actor.OutputHandler.Send("That is not a skill.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send("Specify a valid recognition difficulty.");
			return false;
		}

		RecognitionTrait = trait;
		RecognitionDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send("The recognition check has been updated.");
		return true;
	}

	private bool ToggleSignal(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			SignalPatterns.Clear();
			Changed = true;
			actor.OutputHandler.Send("This standard has no visual signal patterns.");
			return true;
		}

		var pattern = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(pattern))
		{
			actor.OutputHandler.Send("Which signal pattern should be toggled?");
			return false;
		}

		var existing = SignalPatterns.FirstOrDefault(x => x.EqualTo(pattern));
		if (existing is null)
		{
			SignalPatterns.Add(pattern);
		}
		else
		{
			SignalPatterns.Remove(existing);
		}

		Changed = true;
		actor.OutputHandler.Send("The visual signal list has been updated.");
		return true;
	}

	private bool SetText(ICharacter actor, StringStack command, Action<string> setter, string name)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should the {name} be?");
			return false;
		}

		setter(command.SafeRemainingArgument);
		Changed = true;
		actor.OutputHandler.Send($"The {name} has been updated.");
		return true;
	}

	private bool SetProg(ICharacter actor, StringStack command, ProgVariableTypes returnType,
		ProgVariableTypes[] parameters, Action<IFutureProg?> setter, string name)
	{
		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			setter(null);
			Changed = true;
			actor.OutputHandler.Send($"The {name} prog has been cleared.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, returnType, parameters).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"The {name} prog is now {prog.MXPClickableFunctionName()}.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Military Standard Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nFamily: {Family.DescribeEnum().ColourName()}\nIdentity: {IdentityName.ColourName()} ({IdentityKey})\nDesign: {Design}\nAssociation: {AssociationType.DescribeEnum().ColourName()} / {AssociationName.ColourName()}\nRecognition: {RecognitionTrait?.Name.ColourName() ?? "Automatic".ColourValue()} at {RecognitionDifficulty.DescribeColoured()}\nSignals: {(SignalPatterns.Any() ? SignalPatterns.ListToString() : "None")}";
	}
}
