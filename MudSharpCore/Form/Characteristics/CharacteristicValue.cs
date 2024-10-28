using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp.Form.Characteristics;

public class CharacteristicValue : FrameworkItem, ISaveable, ICharacteristicValue
{
	public CharacteristicValue(string name, ICharacteristicDefinition type, string value, string additional,
		bool nonsaving = false)
	{
		_name = name;
		_noSave = nonsaving;
		Definition = type;
		Gameworld = type.Gameworld;

		if (!_noSave)
		{
			using (new FMDB())
			{
				var dbval = new Models.CharacteristicValue
				{
					Name = base.Name,
					Value = value,
					AdditionalValue = additional,
					DefinitionId = Definition.Id,
					Default = false
				};
				FMDB.Context.CharacteristicValues.Add(dbval);
				FMDB.Context.SaveChanges();
				_id = dbval.Id;
			}
		}
	}

	public CharacteristicValue(MudSharp.Models.CharacteristicValue value, IFuturemud gameworld)
	{
		_id = value.Id;
		_name = value.Name;
		Gameworld = gameworld;
		Definition = gameworld.Characteristics.Get(value.DefinitionId);
		Pluralisation = (PluralisationType)value.Pluralisation;
		if (value.Default)
		{
			Definition.SetDefaultValue(this);
		}

		ChargenApplicabilityProg = value.FutureProgId.HasValue
			? gameworld.FutureProgs.Get(value.FutureProgId.Value)
			: null;
		OngoingValidityProg = gameworld.FutureProgs.Get(value.OngoingValidityProgId ?? 0);
	}

	protected CharacteristicValue(CharacteristicValue rhs, string newName, string value, string additional)
	{
		using (new FMDB())
		{
			var dbitem = new Models.CharacteristicValue
			{
				Name = newName,
				Value = value,
				AdditionalValue = additional,
				DefinitionId = rhs.Definition.Id,
				FutureProgId = rhs.ChargenApplicabilityProg?.Id,
				OngoingValidityProgId = rhs.OngoingValidityProg?.Id,
				Pluralisation = (int)rhs.Pluralisation
			};
			FMDB.Context.CharacteristicValues.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		_name = newName;
		ChargenApplicabilityProg = rhs.ChargenApplicabilityProg;
		OngoingValidityProg = rhs.OngoingValidityProg;
		Pluralisation = rhs.Pluralisation;
	}

	public ICharacteristicDefinition Definition { get; protected set; }

	public IFutureProg ChargenApplicabilityProg { get; protected set; }

	public IFutureProg OngoingValidityProg { get; protected set; }

	public sealed override string FrameworkItemType => "CharacteristicValue";

	public virtual string GetValue => Name;

	public virtual string GetBasicValue => Name;

	public virtual string GetFancyValue => Name;

	public PluralisationType Pluralisation { get; set; }

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	protected virtual string HelpText => @"You can use the following options to edit this characteristic value:

	#3name <name>#0 - sets the name of this value
	#3default#0 - sets this value as the 'default' value for the definition
	#3chargen <prog>#0 - sets a prog that controls if this value can be picked in chargen
	#3ongoing <prog>#0 - sets a prog that controls of this value is valid at any time for a character";

	public virtual void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
			case "value":
				var name = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(name))
				{
					actor.OutputHandler.Send("You must supply a name.");
					return;
				}

				_name = name;
				Changed = true;
				actor.OutputHandler.Send($"You change the name of this characteristic value to {name.ColourName()}.");
				return;
			case "basic":
				actor.OutputHandler.Send("This kind of characteristic value has no basic form.");
				return;
			case "fancy":
				actor.OutputHandler.Send("This kind of characteristic value has no fancy form.");
				return;
			case "default":
				Definition.SetDefaultValue(this);
				Changed = true;
				actor.OutputHandler.Send("You set this characteristic value to be the default for its definition.");
				return;
			case "prog":
			case "chargen":
			case "chargenprog":
				BuildingCommandChargenProg(actor, command);
				return;
			case "ongoing":
			case "ongoingprog":
				BuildingCommandOngoingValidityProg(actor, command);
				return;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return;
		}
	}

	private void BuildingCommandOngoingValidityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog would you like to set as the ongoing filter prog for this value?");
			return;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that is compatible with a single Character parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return;
		}

		OngoingValidityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This value now uses the {prog.MXPClickableFunctionNameWithId()} prog to control ongoing validity for a character.");
	}

	private void BuildingCommandChargenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog would you like to set as the chargen filter prog for this value?");
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Chargen }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that is compatible with a single Chargen parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return;
		}

		ChargenApplicabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This value now uses the {prog.MXPClickableFunctionNameWithId()} prog to control selection during character creation.");
	}

	public static ICharacteristicValue LoadValue(MudSharp.Models.CharacteristicValue value, IFuturemud gameworld)
	{
		var definition = gameworld.Characteristics.Get(value.DefinitionId);
		switch (definition.Type)
		{
			case CharacteristicType.Coloured:
				return new ColourCharacteristicValue(value, gameworld);
			case CharacteristicType.Multiform:
				return new MultiformCharacteristicValue(value, gameworld);
			case CharacteristicType.Growable:
				return new GrowableCharacteristicValue(value, gameworld);
			default:
				return new CharacteristicValue(value, gameworld);
		}
	}

	public override string ToString()
	{
		return $"CharacteristicValue ID {Id} Name: {Name}";
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Characteristic Value #{Id.ToString("N0", actor)}");
		sb.AppendLine($"Name: {Name.ColourValue()}");
		sb.AppendLine($"Basic: {GetBasicValue.ColourValue()}");
		sb.AppendLine($"Fancy: {GetFancyValue.ColourValue()}");
		sb.AppendLine($"Pluralisation: {Pluralisation.DescribeEnum().ColourValue()}");
		sb.AppendLine(
			$"Chargen Prog: {ChargenApplicabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Ongoing Prog: {OngoingValidityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		return sb.ToString();
	}

	public virtual ICharacteristicValue Clone(string newName)
	{
		return new CharacteristicValue(this, newName, newName, string.Empty);
	}

	#region ISaveable Members

	protected bool _noSave;
	private bool _changed;

	public bool Changed
	{
		get => _changed;
		set
		{
			if (value && !_changed && !_noSave)
			{
				Gameworld.SaveManager.Add(this);
			}

			_changed = value;
		}
	}

	public virtual void Save()
	{
		using (new FMDB())
		{
			var dbvalue = FMDB.Context.CharacteristicValues.Find(Id);
			dbvalue.Default = Definition.IsDefaultValue(this);
			dbvalue.Name = Name;
			dbvalue.Value = Name;
			dbvalue.Pluralisation = (int)Pluralisation;
			dbvalue.FutureProgId = ChargenApplicabilityProg?.Id;
			dbvalue.OngoingValidityProgId = OngoingValidityProg?.Id;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion
}