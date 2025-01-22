using MudSharp.Character;
using MudSharp.Framework;
using System.Reflection;
using System;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
using static MudSharp.CharacterCreation.Screens.CharacteristicPickerScreenStoryboard;
using MudSharp.Form.Characteristics;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Editor;
using MudSharp.PerceptionEngine;
using System.Reflection.PortableExecutable;

namespace MudSharp.CharacterCreation.Screens;

public class SimpleCharacteristicsPickerScreenStoryboard : ChargenScreenStoryboard
{
	private SimpleCharacteristicsPickerScreenStoryboard()
	{
	}

	private SimpleCharacteristicsPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
	}

	protected SimpleCharacteristicsPickerScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(gameworld,
		storyboard)
	{
		switch (storyboard)
		{
			case CharacteristicPickerScreenStoryboard picker:
				Blurb = picker.Blurb;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "SimpleCharacteristicPicker";

	public string Blurb { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectCharacteristics;

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb))
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectCharacteristics,
			new ChargenScreenStoryboardFactory("SimpleCharacteristicPicker",
				(game, dbitem) => new SimpleCharacteristicsPickerScreenStoryboard(game, dbitem),
			(game, storyboard) => new SimpleCharacteristicsPickerScreenStoryboard(game, storyboard)),
			"SimpleCharacteristicPicker",
			"Randomise characteristics, allow customisation",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen is where people choose the values of their characteristics like eye colour, hair colour and the like. Unlike the other Characteristic Picker, this screen starts with a randomised selection based on their ethnicity and then invites them to customise only what they want to."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}


	#region Building Commands

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, Blurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the blurb for this chargen screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		Blurb = text;
		Changed = true;
		handler.Send($"You set the blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new SimpleCharacteristicPickerScreen(chargen, this);
	}

	internal class SimpleCharacteristicPickerScreen : ChargenScreen
	{
		private readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> SelectedCharacteristics =
			new();

		private readonly SimpleCharacteristicsPickerScreenStoryboard _storyboard;

		private IEnumerable<ICharacteristicValue> _shownValues;
		private ICharacteristicDefinition _selectedDefinition = null;
		private readonly List<ICharacteristicDefinition> _definitions = new();
		private string _selectedBasicValue = "";

		internal SimpleCharacteristicPickerScreen(IChargen chargen, SimpleCharacteristicsPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			_storyboard = storyboard;
			_definitions.AddRange(Chargen.SelectedRace.Characteristics(Chargen.SelectedGender).Distinct());
			RandomiseCharacteristics();

		}

		private void RandomiseCharacteristics()
		{
			SelectedCharacteristics.Clear();
			foreach (var characteristic in _definitions)
			{
				SelectedCharacteristics[characteristic] = Chargen.SelectedEthnicity?.CharacteristicChoices[characteristic].GetRandomCharacteristic(Chargen);
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectCharacteristics;

		private IEnumerable<ICharacteristicValue> GetCharacteristicsFor(ICharacteristicDefinition definition)
		{
			return
				Chargen.SelectedEthnicity.CharacteristicChoices[definition].Values.Where(
					x =>
						x.ChargenApplicabilityProg == null ||
						(x.ChargenApplicabilityProg.ExecuteBool(Chargen))).Distinct();
		}

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			var index = 1;
			var column = 0;
			int columns;
			var sb = new StringBuilder();
			if (_selectedDefinition is null)
			{
				sb.AppendLine("Characteristics Selection".ColourName());
				sb.AppendLine();
				sb.AppendLine(_storyboard.Blurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength));
				sb.AppendLine();
				sb.AppendLine(
					SelectedCharacteristics
						.Select(x =>
							$"{x.Key.Name.TitleCase()}: {x.Value.Name.TitleCase().ColourValue()}"
						)
						.ArrangeStringsOntoLines((uint)Chargen.Account.LineFormatLength / 40, (uint)Chargen.Account.LineFormatLength)
				);
				sb.AppendLine($"Select a characteristic to customise, #3random#0 to randomise, or #3continue#0 to accept current selections.".SubstituteANSIColour());
				return sb.ToString();
			}

			if (string.IsNullOrEmpty(_selectedBasicValue))
			{
				switch (_selectedDefinition.ChargenDisplayType)
				{
					case CharacterGenerationDisplayType.DisplayAll:
						sb.AppendLine($"{_selectedDefinition.Name.TitleCase()} Selection".ColourName());
						sb.AppendLine();
						columns = Chargen.Account.LineFormatLength / 30;
						foreach (var value in GetCharacteristicsFor(_selectedDefinition).OrderBy(x => x.Name))
						{
							sb.Append($"{index++.ToString("F0", Account)}: {value.GetValue.ColourValue()}".RawTextPadRight(30));
							if (++column == columns)
							{
								sb.AppendLine();
								column = 0;
							}
						}
						sb.AppendLine();
						sb.AppendLine($"You are selecting {_selectedDefinition.Name.ToLowerInvariant().A_An().ColourValue()}.");
						sb.AppendLine($"Select the name or number of your desired {_selectedDefinition.Name.ToLowerInvariant()}, or {"back".ColourCommand()} to return to the main screen.");
						return sb.ToString();

					case CharacterGenerationDisplayType.GroupByBasic:
						sb.AppendLine($"{_selectedDefinition.Name.TitleCase()} Selection".ColourName());
						sb.AppendLine();
						columns = Chargen.Account.LineFormatLength / 30;
						foreach (var value in GetCharacteristicsFor(_selectedDefinition).Select(x => x.GetBasicValue).Distinct().OrderBy(x => x))
						{
							sb.Append($"{index++.ToString("F0", Account)}: {value.ColourValue()}".RawTextPadRight(30));
							if (++column == columns)
							{
								sb.AppendLine();
								column = 0;
							}
						}
						sb.AppendLine();
						sb.AppendLine();
						sb.AppendLine($"You are selecting {_selectedDefinition.Name.ToLowerInvariant().A_An().ColourValue()}. First you must select the basic form, and then you will drill down into a specific value.");
						sb.AppendLine($"Select the name or number of the general form of your desired {_selectedDefinition.Name.ToLowerInvariant()}, or {"back".ColourCommand()} to return to the main screen.");
						return sb.ToString();

					case CharacterGenerationDisplayType.DisplayTable:
						var showingLess = false;
						var fullCount = 0;
						if (_shownValues == null)
						{
							_shownValues = GetCharacteristicsFor(_selectedDefinition).ToList();
							if (_shownValues.Count() > 30)
							{
								showingLess = true;
								fullCount = _shownValues.Count();
								_shownValues = _selectedDefinition.DefaultValue != null
									? _shownValues.Except(_selectedDefinition.DefaultValue).PickRandom(29)
												  .Plus(_selectedDefinition.DefaultValue)
												  .OrderByDescending(x => _selectedDefinition.IsDefaultValue(x))
												  .ThenBy(x => x.Name).ToList()
									: _shownValues.PickRandom(25).OrderBy(x => x.Name).ToList();
								if (_shownValues.All(x => !_selectedDefinition.IsDefaultValue(x)) && _selectedDefinition.DefaultValue != null)
								{
									_shownValues = new[] { _selectedDefinition.DefaultValue }
												   .Concat(_shownValues.PickRandom(29).OrderBy(x => x.Name)).ToList();
								}
							}
						}

						var count = 1;
						sb.AppendLine($"{_selectedDefinition.Name.TitleCase()} Selection".ColourName());
						sb.AppendLine();
						sb.AppendLine(StringUtilities.GetTextTable(
							from item in _shownValues
							select
								new[]
								{
									count++.ToString(), item.GetValue, item.GetBasicValue, item.GetFancyValue
								},
							new[] { "#", "Value", "Basic", "Fancy" },
							Account.LineFormatLength,
							colour: Telnet.Green,
							unicodeTable: Account.UseUnicode
						));
						
						sb.AppendLine($"You are selecting {_selectedDefinition.Name.ToLowerInvariant().A_An().ColourValue()}.");
						if (showingLess)
						{
							sb.AppendLine($"You are seeing {30.ToStringN0Colour(Account)} of {fullCount.ToStringN0Colour(Account)} total options.");
						}
						sb.AppendLine($"Select the name or number of your desired {_selectedDefinition.Name.ToLowerInvariant()}, {"more".ColourCommand()} to see more options or {"back".ColourCommand()} to return to the main screen.");
						return sb.ToString();
					default:
						throw new NotSupportedException(
							"Invalid CharacteristicDisplayType in CharacteristicPickerScreen.Get");
				}
			}

			sb.AppendLine($"{_selectedDefinition.Name.TitleCase()} Selection".ColourName());
			sb.AppendLine();
			sb.AppendLine($"You have selected {_selectedBasicValue.ColourCommand()} as your general value. You must now select the specific value.");
			sb.AppendLine();
			columns = Chargen.Account.LineFormatLength / 30;
			foreach (var value in GetCharacteristicsFor(_selectedDefinition)
			                      .Where(x => x.GetBasicValue.EqualTo(_selectedBasicValue))
			                      .OrderBy(x => x.Name))
			{
				sb.Append($"{index++.ToString("F0", Account)}: {value.GetValue.ColourValue()}".RawTextPadRight(30));
				if (++column == columns)
				{
					sb.AppendLine();
					column = 0;
				}
			}
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine($"You are selecting {_selectedDefinition.Name.ToLowerInvariant().A_An().ColourValue()}.");
			sb.AppendLine($"Select the name or number of your desired {_selectedDefinition.Name.ToLowerInvariant()}, or {"back".ColourCommand()} to return to the main screen.");
			return sb.ToString();
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			if ("continue".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				State = ChargenScreenState.Complete;
				Chargen.SelectedCharacteristics =
					SelectedCharacteristics.Select(x => (x.Key, x.Value)).ToList();
				return "";
			}
			
			if ("reset".StartsWith(command, StringComparison.InvariantCultureIgnoreCase) || "random".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				_selectedBasicValue = "";
				_selectedDefinition = null;
				_shownValues = null;
				RandomiseCharacteristics();
				return Display();
			}

			if (_selectedDefinition is not null)
			{
				if (_selectedDefinition.ChargenDisplayType == CharacterGenerationDisplayType.DisplayTable &&
				    "more".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					_shownValues = null;
					return Display();
				}

				if ("back".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					if (!string.IsNullOrEmpty(_selectedBasicValue))
					{
						_selectedBasicValue = "";
						return Display();
					}

					_selectedDefinition = null;
					_shownValues = null;
					return Display();
				}

				if (command.StartsWith("show ", StringComparison.InvariantCultureIgnoreCase))
				{
					var pick = command.RemoveFirstWord();
					if (string.IsNullOrWhiteSpace(pick))
					{
						return "You must select an option to see more information about.";
					}

					var choices = (_selectedDefinition.ChargenDisplayType switch
					{
						CharacterGenerationDisplayType.DisplayAll => GetCharacteristicsFor(_selectedDefinition),
						CharacterGenerationDisplayType.GroupByBasic => GetCharacteristicsFor(_selectedDefinition).Where(x => x.GetBasicValue.EqualTo(_selectedBasicValue)),
						CharacterGenerationDisplayType.DisplayTable => _shownValues,
						_ => GetCharacteristicsFor(_selectedDefinition)
					}).ToList();
					_shownValues = choices;

					var choice = int.TryParse(pick, out var ivalue)
						? _shownValues.ElementAtOrDefault(ivalue - 1)
						: _shownValues.FirstOrDefault(x =>
							x.Name.StartsWith(pick, StringComparison.InvariantCultureIgnoreCase));
					if (choice == null)
					{
						return
							$"That is not a valid selection to show you more info for. Select the name or number of your desired {_selectedDefinition.Name.TitleCase().ColourValue()}.";
						;
					}

					return
						$@"Showing information for the {choice.Name.Colour(Telnet.Green)} pick:

Its default value is: {choice.GetValue.Colour(Telnet.Green)}
Its basic value is: {choice.GetBasicValue.Colour(Telnet.Green)}
Its fancy value is: {choice.GetFancyValue.Colour(Telnet.Green)}";
				}

				int value;
				var availableChoices = (_selectedDefinition.ChargenDisplayType switch
				{
					CharacterGenerationDisplayType.DisplayAll => GetCharacteristicsFor(_selectedDefinition),
					CharacterGenerationDisplayType.GroupByBasic => 
						string.IsNullOrEmpty(_selectedBasicValue) ?
							GetCharacteristicsFor(_selectedDefinition) :
							GetCharacteristicsFor(_selectedDefinition).Where(x => x.GetBasicValue.EqualTo(_selectedBasicValue)),
					CharacterGenerationDisplayType.DisplayTable => _shownValues,
					_ => GetCharacteristicsFor(_selectedDefinition)
				}).ToList();
				if (string.IsNullOrEmpty(_selectedBasicValue))
				{
					switch (_selectedDefinition.ChargenDisplayType)
					{
						case CharacterGenerationDisplayType.DisplayAll:
							var selection = int.TryParse(command, out value)
								? availableChoices.ElementAtOrDefault(value - 1)
								: availableChoices.FirstOrDefault(
									x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

							if (selection == null)
							{
								return
									$"That is not a valid selection. Select the name or number of your desired {_selectedDefinition.Name}.";
							}

							SelectedCharacteristics[_selectedDefinition] = selection;
							_selectedDefinition = null;
							break;

						case CharacterGenerationDisplayType.GroupByBasic:
							string basicSelection = null;
							basicSelection = int.TryParse(command, out value)
								? availableChoices.Select(x => x.GetBasicValue).Distinct().ElementAtOrDefault(value - 1)
								: availableChoices.Select(x => x.GetBasicValue)
												  .Distinct()
												  .FirstOrDefault(
													  x => x.StartsWith(command,
														  StringComparison.InvariantCultureIgnoreCase));

							if (string.IsNullOrEmpty(basicSelection))
							{
								return
									$"That is not a valid selection. Select the name or number of the general type of your desired {_selectedDefinition.Name}.";
							}

							_selectedBasicValue = basicSelection;
							_shownValues = GetCharacteristicsFor(_selectedDefinition)
										   .Where(x => x.GetBasicValue == _selectedBasicValue).ToList();
							return Display();
						case CharacterGenerationDisplayType.DisplayTable:
							selection = int.TryParse(command, out value)
								? _shownValues.ElementAtOrDefault(value - 1)
								: _shownValues.FirstOrDefault(
									x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

							if (selection == null)
							{
								return $"That is not a valid selection. Select the name or number of your desired {_selectedDefinition.Name}.";
							}

							SelectedCharacteristics[_selectedDefinition] = selection;
							_shownValues = null;
							_selectedDefinition = null;
							break;
						default:
							throw new NotSupportedException(
								"Invalid ChargenDisplayType in SimpleCharacteristicPickerScreen.HandleCommand");
					}
				}
				else
				{
					ICharacteristicValue selection = null;
					selection = int.TryParse(command, out value)
						? availableChoices.Where(x => x.GetBasicValue == _selectedBasicValue)
										  .ElementAtOrDefault(value - 1)
						: availableChoices.Where
									 (x => x.GetBasicValue == _selectedBasicValue)
								 .FirstOrDefault(x =>
									 x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

					if (selection == null)
					{
						return
							$"That is not a valid selection. Select the name or number of your desired {_selectedDefinition.Name}.";
					}

					SelectedCharacteristics[_selectedDefinition] = selection;
					_selectedBasicValue = "";
					_shownValues = null;
					_selectedDefinition = null;
				}

				return Display();
			}

			_selectedDefinition = _definitions.FirstOrDefault(x => x.Pattern.IsMatch(command)) ??
			                      _definitions.GetByIdOrName(command);
			if (_selectedDefinition is null)
			{
				return "That is not a valid characteristic to select.";
			}

			return Display();
		}
	}
}