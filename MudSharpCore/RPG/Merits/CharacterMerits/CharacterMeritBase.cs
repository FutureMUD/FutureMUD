using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Parsers;
using Chargen = MudSharp.CharacterCreation.Chargen;

namespace MudSharp.RPG.Merits.CharacterMerits;

public abstract class CharacterMeritBase : MeritBase, ICharacterMerit
{
	private readonly List<ChargenResourceCost> _costs = new();

	public IFutureProg ChargenAvailableProg { get; private set; }

	private long? _parentMeritId;
	private ICharacterMerit _parentMerit;
	private string _descriptionText;

	public ICharacterMerit ParentMerit
	{
		get
		{
			if (_parentMerit is null && _parentMeritId.HasValue)
			{
				_parentMerit = Gameworld.Merits.Get(_parentMeritId.Value) as ICharacterMerit;
			}

			return _parentMerit;
		}
	}

	protected CharacterMeritBase(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		DatabaseType = merit.Type;
		foreach (var resource in merit.MeritsChargenResources)
		{
			_costs.Add(new ChargenResourceCost
			{
				Amount = resource.Amount,
				RequirementOnly = resource.RequirementOnly,
				Resource = gameworld.ChargenResources.Get(resource.ChargenResourceId)
			});
		}

		_parentMeritId = merit.ParentId;
		LoadFromDb(XElement.Parse(merit.Definition), gameworld);
	}

	protected CharacterMeritBase(IFuturemud gameworld, string name, string databaseType, string descriptionText) : base(gameworld, name, MeritType.Merit, MeritScope.Character, databaseType)
	{
		ChargenAvailableProg = Gameworld.AlwaysFalseProg;
		_descriptionText = descriptionText;
		ChargenBlurb = "This merit does not have a detailed description.";
	}

	protected CharacterMeritBase()
	{

	}

	public override bool Applies(IHaveMerits owner)
	{
		if (ParentMerit is not null)
		{
			return ParentMerit.Applies(owner);
		}

		if (owner is ICharacter ownerAsCharacter)
		{
			return Applies(ownerAsCharacter);
		}

		if (owner is IBody ownerAsBody)
		{
			return Applies(ownerAsBody.Actor);
		}

		return owner is Chargen ownerAsChargen;
	}

	public virtual bool Applies(IHaveMerits owner, IPerceivable target)
	{
		return Applies(owner);
	}

	#region Implementation of IMerit

	public override string Describe(IHaveMerits owner, IPerceiver voyeur)
	{
		if (owner is ICharacter ownerAsCharacter)
		{
			return new Emote(_descriptionText, ownerAsCharacter, ownerAsCharacter, voyeur).ParseFor(voyeur);
		}

		return owner is IBody ownerAsBody
			? new Emote(_descriptionText, ownerAsBody.Actor, ownerAsBody.Actor, voyeur).ParseFor(voyeur)
			: "";
	}

	#endregion

	private void LoadFromDb(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("ChargenAvailableProg");
		if (element != null)
		{
			ChargenAvailableProg = gameworld.FutureProgs.Get(long.Parse(element.Value));
		}
		else
		{
			ChargenAvailableProg = gameworld.AlwaysTrueProg;
		}

		element = root.Element("ChargenBlurb");
		if (element != null)
		{
			ChargenBlurb = element.Value;
		}

		element = root.Element("ApplicabilityProg");
		if (element != null)
		{
			ApplicabilityProg = gameworld.FutureProgs.Get(long.Parse(element.Value));
		}
		else
		{
			ApplicabilityProg = gameworld.AlwaysTrueProg;
		}

		element = root.Element("DescriptionText");
		if (element != null)
		{
			_descriptionText = element.Value;
		}
	}

	protected virtual bool Applies(ICharacter character)
	{
		return ApplicabilityProg?.ExecuteBool(character) ?? true;
	}

	#region ICharacterMerit Members

	public bool ChargenAvailable(IChargen chargen)
	{
		return
			_costs.Where(x => x.RequirementOnly)
			      .All(x => chargen.Account.AccountResources[x.Resource] >= x.Amount) &&
			(ChargenAvailableProg?.ExecuteBool(chargen) ?? true);
	}

	public string ChargenBlurb { get; protected set; }

	public int ResourceCost(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => !x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public int ResourceRequirement(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public bool DisplayInCharacterMeritsCommand(ICharacter character)
	{
		return _parentMerit == null || character.IsAdministrator();
	}

	#endregion

	/// <inheritdoc />
	protected override string SubtypeHelp => @"
	#3parent <other>#0 - sets a combination merit at this merit's parent
	#3parent none#0 - clears a merit parent
	#3applies <prog>#0 - sets a prog that controls when this merit applies
	#3chargenprog <prog>#0 - sets a prog that controls whether this can be selected in chargen
	#3description <text>#0 - sets the description that appears in the #3merits#0 command
	#3blurb#0 - drops into an editor to set the blurb shown in chargen
	#3blurb <text>#0 - sets the chargen blurb directly
	#3cost <resource> <amount>#0 - sets a cost to select this merit in chargen
	#3cost <resource> 0#0 - removes a cost
	#3cost <resource> spend#0 - toggles the resource being spent or only required";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "parent":
				return BuildingCommandParent(actor, command);
			case "appliesprog":
			case "applies":
				return BuildingCommandAppliesProg(actor, command);
			case "chargenprog":
				return BuildingCommandChargenProg(actor, command);
			case "cost":
				return BuildingCommandCost(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "blurb":
				return BuildingCommandBlurb(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which chargen resource do you want to set a cost for?");
			return false;
		}

		var resource = Gameworld.ChargenResources.GetByIdOrNames(command.PopSpeech());
		if (resource is null)
		{
			actor.OutputHandler.Send($"There is no chargen resource identified by the text {command.Last.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a number of {resource.Name.ColourValue()} to spend, or use the keyword {"spend".ColourCommand()} to toggle whether its spent or required only.");
			return false;
		}
		var cost = _costs.FirstOrDefault(x => x.Resource == resource);
		if (command.SafeRemainingArgument.EqualToAny("spend", "spent", "requireonly", "require"))
		{
			if (cost is null)
			{
				actor.OutputHandler.Send($"There is no cost set up for the {resource.Name.ColourValue()} resource.");
				return false;
			}

			_costs[_costs.IndexOf(cost)] = cost with { RequirementOnly = !cost.RequirementOnly };
			Changed = true;
			actor.OutputHandler.Send($"The {resource.Name.ColourValue()} resource cost for this merit will {(!cost.RequirementOnly).NowNoLonger()} be spent.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (value == 0)
		{
			if (_costs.RemoveAll(x => x.Resource == resource) == 0)
			{
				actor.OutputHandler.Send($"The {resource.Name.ColourValue()} resource is not currently a cost for this merit.");
				return false;
			}

			actor.OutputHandler.Send($"This merit no longer has a cost or requirement for the {resource.Name.ColourValue()} resource.");
			Changed = true;
			return true;
		}

		if (cost is null)
		{
			cost = new ChargenResourceCost
			{
				Resource = resource,
				Amount = value,
				RequirementOnly = false
			};
			_costs.Add(cost);
		}
		else
		{
			_costs[_costs.IndexOf(cost)] = cost with { Amount = value };
		}

		actor.OutputHandler.Send($"This merit now {(cost.RequirementOnly ? "requires" : "costs")} #2{value.ToString("N0", actor)} {(value == 1 ? resource.Name : resource.PluralName)}#0".SubstituteANSIColour());
		Changed = true;
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		if (!command.IsFinished)
		{
			ChargenBlurb = command.SafeRemainingArgument;
			Changed = true;
			actor.OutputHandler.Send($"You change the blurb for chargen to the following:\n\n{ChargenBlurb.Wrap(actor.InnerLineFormatLength).SubstituteANSIColour()}");
			return true;
		}

		actor.OutputHandler.Send(@$"Replacing:

{ChargenBlurb.Wrap(actor.InnerLineFormatLength, "\t").SubstituteANSIColour()}

Enter your blurb that will be visible in chargen in the editor below:");
		actor.EditorMode((text, handler, pars) =>
			{
				ChargenBlurb = text;
				Changed = true;
				handler.Send($"\n\nYou change the blurb for chargen to the following:\n\n{ChargenBlurb.Wrap(actor.InnerLineFormatLength).SubstituteANSIColour()}");
			},
		(handler, pars) =>
			{
				handler.Send("You decide not to change the chargen blurb.");
			});
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the description of this merit in the #3merits#0 command?".SubstituteANSIColour());
			return false;
		}

		var text = command.SafeRemainingArgument;
		var emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_descriptionText = text;
		Changed = true;
		actor.OutputHandler.Send($"The description of this merit in the #3merits#0 command is now #6{text}#0".SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandParent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a parent, or use the keyword #3none#0 to remove one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			if (ParentMerit is ComboMerit old)
			{
				old.RemoveChild(this);
			}
			_parentMerit = null;
			_parentMeritId = null;
			Changed = true;
			actor.OutputHandler.Send($"This merit will not have any parent merit controlling its applicability.");
			return true;
		}

		if (Gameworld.Merits.GetByIdOrName(command.SafeRemainingArgument) is not ICharacterMerit parent)
		{
			actor.OutputHandler.Send($"There is no such character merit identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (parent == this)
		{
			actor.OutputHandler.Send("A merit can't be its own parent.");
			return false;
		}

		if (parent is not ComboMerit cm)
		{
			actor.OutputHandler.Send($"Only combo merits can be parents, and {parent.Name.ColourValue()} is not.");
			return false;
		}

		if (this is ComboMerit tcm)
		{
			actor.OutputHandler.Send("Combo merits cannot themselves be children of other merits.");
			return false;
		}

		if (ParentMerit is ComboMerit cmold)
		{
			cmold.RemoveChild(this);
		}
		_parentMerit = parent;
		_parentMeritId = parent.Id;
		Changed = true;
		cm.AddChild(this);
		actor.OutputHandler.Send($"This merit is now a child of the {parent.Name.ColourValue()} merit.");
		return true;
	}

	protected virtual IEnumerable<IEnumerable<ProgVariableTypes>> AppliesProgValidTypes => [[ProgVariableTypes.Character]];

	private bool BuildingCommandAppliesProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to determine whether the effects of this merit apply?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, AppliesProgValidTypes).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ApplicabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This merit will use the {prog.MXPClickableFunctionName()} prog to determine whether it applies.");
		return true;
	}

	private bool BuildingCommandChargenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to determine its availability in chargen?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [ProgVariableTypes.Chargen]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ChargenAvailableProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This merit will use the {prog.MXPClickableFunctionName()} prog to determine whether it is available in chargen.");
		return true;
	}

	/// <inheritdoc />
	public sealed override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Parent: {ParentMerit?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Type: {DatabaseType.ColourName()}");
		sb.AppendLine($"Applies Prog: {ApplicabilityProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Chargen Prog: {ChargenAvailableProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Description: {_descriptionText.ColourCommand()}");
		sb.AppendLine($"Blurb:\n\n{ChargenBlurb.Wrap(actor.InnerLineFormatLength).SubstituteANSIColour()}");
		sb.AppendLine();
		SubtypeShow(actor, sb);
		sb.AppendLine();
		sb.AppendLine("Costs:"); 
		sb.AppendLine();
		if (_costs.Count == 0)
		{
			sb.AppendLine($"\t#3None#0".SubstituteANSIColour());
		}
		else
		{
			foreach (var item in _costs)
			{
				sb.AppendLine($"\t#2{item.Amount.ToString("N0", actor)} {(item.Amount == 1 ? item.Resource.Name : item.Resource.PluralName)}#0 {(item.RequirementOnly ? "[requirement only]".Colour(Telnet.BoldYellow) : "[spent]".Colour(Telnet.Orange))}".SubstituteANSIColour());
			}
		}
		
		return sb.ToString();
	}

	protected virtual void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		// Do nothing
	}

	/// <inheritdoc />
	public sealed override XElement SaveDefinition()
	{
		return SaveSubtypeDefinition(new XElement("Merit",
			new XElement("ChargenAvailableProg", ChargenAvailableProg.Id),
			new XElement("ApplicabilityProg", ApplicabilityProg.Id),
			new XElement("ChargenBlurb", new XCData(ChargenBlurb)),
			new XElement("DescriptionText", new XCData(_descriptionText))
		));
	}

	protected virtual XElement SaveSubtypeDefinition(XElement root)
	{
		return root;
	}
}