#nullable enable

using System.Reflection;
using MudSharp.Form.Characteristics;

namespace MudSharp.CharacterCreation.Screens;

public class RerollableCharacteristicsPickerScreenStoryboard : SimpleCharacteristicsPickerScreenStoryboard
{
	private RerollableCharacteristicsPickerScreenStoryboard()
	{
	}

	private RerollableCharacteristicsPickerScreenStoryboard(IFuturemud gameworld,
		Models.ChargenScreenStoryboard dbitem) : base(gameworld, dbitem)
	{
		SeparateBlurb = bool.TryParse(XElement.Parse(dbitem.StageDefinition).Element("SeparateBlurb")?.Value,
			out var separateBlurb) ? separateBlurb : true;
	}

	private RerollableCharacteristicsPickerScreenStoryboard(IFuturemud gameworld,
		IChargenScreenStoryboard storyboard) : base(gameworld, storyboard)
	{
		if (storyboard is RerollableCharacteristicsPickerScreenStoryboard rerollable)
		{
			SeparateBlurb = rerollable.SeparateBlurb;
		}
	}

	protected override string StoryboardName => "RerollableCharacteristicPicker";

	public bool SeparateBlurb { get; private set; } = true;

	public override string HelpText => $@"{base.HelpText}
	#3intro#0 - toggles the separate blurb screen (currently {SeparateBlurb.ToColouredString()})";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb ?? string.Empty)),
			new XElement("SeparateBlurb", SeparateBlurb)).ToString();
	}

	public static new void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectCharacteristics,
			new ChargenScreenStoryboardFactory("RerollableCharacteristicPicker",
				(game, dbitem) => new RerollableCharacteristicsPickerScreenStoryboard(game, dbitem),
				(game, storyboard) => new RerollableCharacteristicsPickerScreenStoryboard(game, storyboard)),
			"RerollableCharacteristicPicker",
			"Roll characteristics, lock favourites and customise individual values",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod()!.DeclaringType!, true)!)
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		return base.Show(voyeur) +
		       $"\nSeparate blurb screen: {SeparateBlurb.ToColouredString()}\n";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch() != "intro")
		{
			return base.BuildingCommand(actor, command.GetUndo());
		}

		SeparateBlurb = !SeparateBlurb;
		Changed = true;
		actor.OutputHandler.Send($"The separate blurb screen is now {SeparateBlurb.ToColouredString()}.");
		return true;
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new RerollableCharacteristicPickerScreen(chargen, this);
	}

	internal class RerollableCharacteristicPickerScreen : SimpleCharacteristicPickerScreen
	{
		protected override bool HandleGlobalCommands => false;

		internal enum LockMode
		{
			None,
			Full,
			Basic
		}

		private readonly Dictionary<ICharacteristicDefinition, LockMode> _locks = new();
		private readonly RerollableCharacteristicsPickerScreenStoryboard _rerollableStoryboard;
		private bool _shownIntro;

		internal RerollableCharacteristicPickerScreen(IChargen chargen,
			RerollableCharacteristicsPickerScreenStoryboard storyboard) : base(chargen, storyboard)
		{
			_rerollableStoryboard = storyboard;
		}

		public override string Display()
		{
			if (_rerollableStoryboard.SeparateBlurb && !_shownIntro)
			{
				return $"Characteristics Selection".ColourName() + "\n\n" +
				       (_rerollableStoryboard.Blurb ?? string.Empty).SubstituteANSIColour()
				       .Wrap(Account.InnerLineFormatLength) +
				       $"\n\nType {"continue".ColourCommand()} to see your rolled characteristics.";
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (_selectedDefinition != null)
			{
				return base.Display();
			}

			var sb = new StringBuilder();
			sb.AppendLine("Characteristics Selection".ColourName());
			sb.AppendLine();
			if (!_rerollableStoryboard.SeparateBlurb &&
			    !string.IsNullOrWhiteSpace(_rerollableStoryboard.Blurb))
			{
				sb.AppendLine(_rerollableStoryboard.Blurb.SubstituteANSIColour()
					.Wrap(Account.InnerLineFormatLength));
				sb.AppendLine();
			}

			foreach (var definition in _definitions)
			{
				var value = SelectedCharacteristics[definition];
				var mode = _locks.GetValueOrDefault(definition);
				var suffix = mode switch
				{
					LockMode.Full => " [locked]",
					LockMode.Basic => $" [basic lock: {value?.GetBasicValue}]",
					_ => string.Empty
				};
				sb.AppendLine($"{definition.Name.TitleCase()}: " +
				              (value?.Name.TitleCase().ColourValue() ?? "Not Selected".ColourError()) +
				              suffix.ColourCommand());
			}

			sb.AppendLine();
			if (_definitions.Any(x => SelectedCharacteristics[x] == null))
			{
				sb.AppendLine("Unselected characteristics have no eligible profile value. Choose a valid value before continuing."
					.ColourError());
			}

			sb.AppendLine("Select a characteristic to customise, #3reroll#0, #3lock <characteristic>#0, #3basiclock <characteristic>#0, #3unlock <characteristic>#0, or #3continue#0 to accept."
				.SubstituteANSIColour());
			return sb.ToString();
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				return Display();
			}

			if (_rerollableStoryboard.SeparateBlurb && !_shownIntro)
			{
				if (command.EqualTo("continue"))
				{
					_shownIntro = true;
				}

				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			if (_selectedDefinition != null)
			{
				if (command.EqualTo("reroll") || command.EqualTo("reset") ||
				    command.EqualTo("random") || command.EqualTo("continue") ||
				    command.StartsWith("lock ", StringComparison.InvariantCultureIgnoreCase) ||
				    command.StartsWith("basiclock ", StringComparison.InvariantCultureIgnoreCase) ||
				    command.StartsWith("unlock ", StringComparison.InvariantCultureIgnoreCase))
				{
					return "Type back to return to the main screen first.";
				}

				return base.HandleCommand(command);
			}

			if (command.EqualTo("continue"))
			{
				var missing = _definitions.Where(x => SelectedCharacteristics[x] == null).ToList();
				if (missing.Count > 0)
				{
					return $"You must select valid values for {missing.Select(x => x.Name).ListToString()} before continuing.";
				}

				Chargen.SelectedCharacteristics = SelectedCharacteristics.Select(x => (x.Key, x.Value)).ToList();
				State = ChargenScreenState.Complete;
				return string.Empty;
			}

			if (command.EqualTo("reroll"))
			{
				foreach (var definition in _definitions)
				{
					var mode = _locks.GetValueOrDefault(definition);
					if (mode == LockMode.Full)
					{
						continue;
					}

					if (mode == LockMode.Basic && SelectedCharacteristics[definition] is { } current)
					{
						var candidates = GetCharacteristicsFor(definition)
							.Where(x => x.GetBasicValue.EqualTo(current.GetBasicValue)).ToList();
						SelectedCharacteristics[definition] = candidates.GetRandomElement() ?? current;
						continue;
					}

					SelectedCharacteristics[definition] = Chargen.SelectedEthnicity != null &&
						Chargen.SelectedEthnicity.CharacteristicChoices.TryGetValue(definition, out var profile)
						? profile.GetRandomCharacteristic(Chargen)
						: null;
				}

				return Display();
			}

			var input = new StringStack(command);
			var action = input.PopForSwitch();
			if (action is "lock" or "basiclock" or "unlock")
			{
				var name = input.SafeRemainingArgument;
				var definition = FindDefinition(name);
				if (definition == null)
				{
					return "Specify a valid characteristic to lock or unlock.";
				}

				if (action == "unlock")
				{
					_locks.Remove(definition);
					return Display();
				}

				var value = SelectedCharacteristics[definition];
				if (value == null)
				{
					return $"{definition.Name} has no selected value to lock.";
				}

				if (action == "basiclock" && GetCharacteristicsFor(definition)
				    .Count(x => x.GetBasicValue.EqualTo(value.GetBasicValue)) < 2)
				{
					return $"{definition.Name} has no alternative eligible values with the basic value {value.GetBasicValue}.";
				}

				_locks[definition] = action == "lock" ? LockMode.Full : LockMode.Basic;
				return Display();
			}

			_selectedDefinition = FindDefinition(command);
			return _selectedDefinition == null
				? "That is not a valid characteristic to select."
				: Display();
		}

		private ICharacteristicDefinition? FindDefinition(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			return _definitions.FirstOrDefault(x => x.Pattern.IsMatch(name)) ?? _definitions.GetByIdOrName(name);
		}
	}
}
