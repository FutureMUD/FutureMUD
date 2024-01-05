using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Form.Characteristics;

internal class WeightedCharacteristicProfile : CharacteristicProfile
{
	private readonly List<(long Id, double Weight)> _idsAndWeights = new();

	private readonly List<(ICharacteristicValue Value, double Weight)> _valuesAndWeights = new();

	public WeightedCharacteristicProfile(Models.CharacteristicProfile profile, IFuturemud gameworld) : base(profile,
		gameworld)
	{
	}

	public WeightedCharacteristicProfile(string name, ICharacteristicDefinition target) : base(name, target, "Weighted",
		null)
	{
	}

	protected WeightedCharacteristicProfile(WeightedCharacteristicProfile rhs, string newName) : base(rhs, newName)
	{
	}

	protected void EnsureValuesLoaded()
	{
		if (_valuesAndWeights.Any())
		{
			return;
		}

		foreach (var (id, weight) in _idsAndWeights)
		{
			_valuesAndWeights.Add((Gameworld.CharacteristicValues.Get(id), weight));
		}
	}

	#region Overrides of CharacteristicProfile

	protected override void LoadFromXml(XElement root)
	{
		foreach (var value in root.Elements("Value"))
		{
			_idsAndWeights.Add((long.Parse(value.Value), double.Parse(value.Attribute("weight").Value)));
		}
	}

	public override ICharacteristicProfile Clone(string newName)
	{
		return new WeightedCharacteristicProfile(this, newName);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
				from value in _idsAndWeights
				select new XElement("Value", new XAttribute("weight", value.Weight), value.Id))
			.ToString();
	}

	public override IEnumerable<ICharacteristicValue> Values
	{
		get
		{
			EnsureValuesLoaded();
			return _valuesAndWeights.Select(x => x.Value);
		}
	}

	public override ICharacteristicValue GetRandomCharacteristic()
	{
		EnsureValuesLoaded();
		return _valuesAndWeights.GetWeightedRandom();
	}

	public override string Type => "Weighted";

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Characteristic Profile #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Definition: {TargetDefinition.Name.ColourValue()}");
		sb.AppendLine($"Type: {Type.TitleCase().ColourValue()}");
		sb.AppendLine($"Description: {Description.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Values:");
		EnsureValuesLoaded();
		var sum = _valuesAndWeights.Sum(x => x.Weight);
		foreach (var (value, weight) in _valuesAndWeights.OrderByDescending(x => x.Weight))
		{
			sb.AppendLine(
				$"\t[{value.Id.ToString("N0", actor)}] {value.Name.ColourValue()} - {weight.ToString("N3", actor)} ({(weight / sum).ToString("P2", actor)})");
		}

		return sb.ToString();
	}

	public override string HelpText => @"You can use the following options when editing this profile:

	name <name> - changes the name of this profile
	desc <description> - changes the description of this profile
	value <which> <weight> - adds or edits a value and its weighting
    value <which> - removes a value from the profile";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "value":
				BuildingCommandValue(actor, command);
				return;
		}

		base.BuildingCommand(actor, command.GetUndo());
	}

	protected void BuildingCommandValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic value do you want to toggle for this profile?");
			return;
		}

		var value = Gameworld.CharacteristicValues.GetByIdOrName(command.SafeRemainingArgument);
		if (value == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value.");
			return;
		}

		if (!TargetDefinition.IsValue(value))
		{
			actor.OutputHandler.Send(
				$"The value {value.Name.ColourName()} is not a valid value for the {TargetDefinition.Name.ColourName()} definition.");
			return;
		}

		EnsureValuesLoaded();
		if (command.IsFinished)
		{
			if (_valuesAndWeights.Any(x => x.Value == value))
			{
				_valuesAndWeights.RemoveAll(x => x.Value == value);
				_idsAndWeights.RemoveAll(x => x.Id == value.Id);
				Changed = true;
				actor.OutputHandler.Send(
					$"The value {value.Name.ColourName()} is no longer an option for this profile.");
				return;
			}

			actor.OutputHandler.Send($"What weighting do you want to give to {value.Name.ColourName()}?");
			return;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var weight))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return;
		}

		if (_valuesAndWeights.Any(x => x.Value == value))
		{
			var index = _valuesAndWeights.FindIndex(x => x.Value == value);
			_valuesAndWeights[index] = (value, weight);
			_idsAndWeights[index] = (value.Id, weight);
		}
		else
		{
			_valuesAndWeights.Add((value, weight));
			_idsAndWeights.Add((value.Id, weight));
		}

		Changed = true;
		var sum = _valuesAndWeights.Sum(x => x.Weight);
		actor.OutputHandler.Send(
			$"The value {value.Name.ColourName()} is now an option with a weighting of {weight.ToString("N3", actor).ColourValue()} ({(weight / sum).ToString("P2", actor).ColourValue()} chance).");
	}

	public override void ExpireCharacteristic(ICharacteristicValue value)
	{
		base.ExpireCharacteristic(value);
		_valuesAndWeights.RemoveAll(x => x.Value == value);
		_idsAndWeights.RemoveAll(x => x.Id == value.Id);
	}

	#endregion
}