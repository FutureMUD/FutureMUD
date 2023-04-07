using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class CharacteristicPickerScreenStoryboard : ChargenScreenStoryboard
{
	private CharacteristicPickerScreenStoryboard()
	{
	}

	public CharacteristicPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
	}

	protected override string StoryboardName => "CharacteristicPicker";

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
			new ChargenScreenStoryboardFactory("CharacteristicPicker",
				(game, dbitem) => new CharacteristicPickerScreenStoryboard(game, dbitem)),
			"CharacteristicPicker",
			"Pick each characteristic in turn",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen is where people choose the values of their characteristics like eye colour, hair colour and the like."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new CharacteristicPickerScreen(chargen, this);
	}

	internal class CharacteristicPickerScreen : ChargenScreen
	{
		private readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> SelectedCharacteristics =
			new();

		private readonly CharacteristicPickerScreenStoryboard Storyboard;

		private IEnumerable<ICharacteristicValue> _shownValues;
		private IEnumerator<ICharacteristicDefinition> CharacteristicEnumerator;
		private string SelectedBasicValue = "";
		private bool ShownInitialScreen;

		internal CharacteristicPickerScreen(IChargen chargen, CharacteristicPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			var characteristics = Chargen.SelectedRace.Characteristics(Chargen.SelectedGender).Distinct().ToList();
			SelectedCharacteristics.Clear();
			foreach (var characteristic in characteristics)
			{
				SelectedCharacteristics[characteristic] = null;
			}

			CharacteristicEnumerator = characteristics.GetEnumerator();
			if (!CharacteristicEnumerator.MoveNext())
			{
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectCharacteristics;

		private IEnumerable<ICharacteristicValue> GetCharacteristicsFor(ICharacteristicDefinition definition)
		{
			return
				Chargen.SelectedEthnicity.CharacteristicChoices[definition].Values.Where(
					x =>
						x.ChargenApplicabilityProg == null ||
						((bool?)x.ChargenApplicabilityProg.Execute(Chargen) ?? false)).Distinct();
		}

		public override string Display()
		{
			if (!ShownInitialScreen)
			{
				return
					string.Format(
						"Characteristics Selection".Colour(Telnet.Cyan) +
						"\n\n{0}\n\nPlease type {1} to begin. At any time, you may type {2} to return to the beginning.",
						Storyboard.Blurb.SubstituteANSIColour()
						          .Wrap(Chargen.Account.InnerLineFormatLength),
						"continue".Colour(Telnet.Yellow),
						"reset".Colour(Telnet.Yellow)
					);
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			var index = 1;
			if (string.IsNullOrEmpty(SelectedBasicValue))
			{
				switch (CharacteristicEnumerator.Current.ChargenDisplayType)
				{
					case CharacterGenerationDisplayType.DisplayAll:
						return
							string.Format(
								"{2} Selection".Colour(Telnet.Cyan) +
								"\n\n{0}\n\nYou are selecting {4}.\n\n{1}\n\nSelect the name or number of your desired {2}, or {3} to begin again.",
								SelectedCharacteristics.Select(
									                       x =>
										                       $"{x.Key.Name.Proper()}: {(x.Value == null ? "Not Selected".Colour(Telnet.Red) : x.Value.Name.Proper().Colour(Telnet.Cyan))}")
								                       .ArrangeStringsOntoLines(2, (uint)Account.LineFormatLength),
								GetCharacteristicsFor(CharacteristicEnumerator.Current)
									.Select(x => $"{index++}: {x.GetValue}")
									.ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30,
										(uint)Account.LineFormatLength),
								CharacteristicEnumerator.Current.Name,
								"reset".Colour(Telnet.Yellow),
								CharacteristicEnumerator.Current.Name.A_An(colour: Telnet.Green)
							);

					case CharacterGenerationDisplayType.GroupByBasic:
						return
							string.Format(
								"{2} Selection".Colour(Telnet.Cyan) +
								"\n\n{0}\n\nYou are selecting {4}. First you must select the basic form, and then you will drill down into a specific value.\n\n{1}\n\nSelect the name or number of the general type of your desired {2}, or {3} to begin again.",
								SelectedCharacteristics.Select(
									                       x =>
										                       $"{x.Key.Name.Proper()}: {(x.Value == null ? "Not Selected".Colour(Telnet.Red) : x.Value.Name.Proper().Colour(Telnet.Cyan))}")
								                       .ArrangeStringsOntoLines(2, (uint)Account.LineFormatLength),
								GetCharacteristicsFor(CharacteristicEnumerator.Current)
									.Select(x => x.GetBasicValue)
									.Distinct()
									.Select(x => $"{index++}: {x}")
									.ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30,
										(uint)Account.LineFormatLength),
								CharacteristicEnumerator.Current.Name,
								"reset".Colour(Telnet.Yellow),
								CharacteristicEnumerator.Current.Name.A_An(colour: Telnet.Green)
							);

					case CharacterGenerationDisplayType.DisplayTable:
						var showingLess = false;
						var fullCount = 0;
						if (_shownValues == null)
						{
							_shownValues = GetCharacteristicsFor(CharacteristicEnumerator.Current).ToList();
							if (_shownValues.Count() > 25)
							{
								showingLess = true;
								fullCount = _shownValues.Count();
								_shownValues = CharacteristicEnumerator.Current.DefaultValue != null
									? _shownValues.Except(CharacteristicEnumerator.Current.DefaultValue).PickRandom(24)
									              .Plus(CharacteristicEnumerator.Current.DefaultValue)
									              .OrderByDescending(x =>
										              CharacteristicEnumerator.Current.IsDefaultValue(x))
									              .ThenBy(x => x.Name).ToList()
									: _shownValues.PickRandom(25).OrderBy(x => x.Name).ToList();
								if (_shownValues.All(x => !CharacteristicEnumerator.Current.IsDefaultValue(x)) &&
								    CharacteristicEnumerator.Current.DefaultValue != null)
								{
									_shownValues = new[] { CharacteristicEnumerator.Current.DefaultValue }
									               .Concat(_shownValues.PickRandom(24).OrderBy(x => x.Name)).ToList();
								}
							}
						}

						var count = 1;
						return
							string.Format(
								"{0}\n\n{1}\n\nYou are selecting {2}.{7}\n\n{3}\n\nSelect the name or number of your desired {2}, {6} to see more info about a specific choice, {4} to see more, and {5} to begin again",
								string.Format("{0} Selection".Colour(Telnet.Cyan),
									CharacteristicEnumerator.Current.Name),
								SelectedCharacteristics.Select(
									                       x =>
										                       $"{x.Key.Name.Proper()}: {(x.Value == null ? "Not Selected".Colour(Telnet.Red) : x.Value.Name.Proper().Colour(Telnet.Cyan))}")
								                       .ArrangeStringsOntoLines(2, (uint)Account.LineFormatLength),
								CharacteristicEnumerator.Current.Name,
								StringUtilities.GetTextTable(
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
								),
								"random".Colour(Telnet.Yellow),
								"reset".Colour(Telnet.Yellow),
								"show <pick>",
								showingLess
									? $" You are seeing 25 of {fullCount.ToString("N0", Account).ColourValue()} total options."
									: ""
							);
					default:
						throw new NotSupportedException(
							"Invalid CharacteristicDisplayType in CharacteristicPickerScreen.Get");
				}
			}

			return
				string.Format(
					"{2} Selection".Colour(Telnet.Cyan) +
					"\n\n{0}\n\nYou have selected {5} as your general value for {2}. You must now select the specific value.\n\n{1}\n\nSelect the name or number of your desired {2}, {6} to see more info about a specific choice, {3} to see the previous menu, or {4} to begin again.",
					SelectedCharacteristics.OrderBy(x => x.Value == null)
					                       .ThenBy(x => x.Key.Name)
					                       .Select(
						                       x =>
							                       $"{x.Key.Name.Proper()}: {(x.Value == null ? "Not Selected".Colour(Telnet.Red) : x.Value.Name.Proper().Colour(Telnet.Cyan))}")
					                       .ArrangeStringsOntoLines(2, (uint)Account.LineFormatLength),
					GetCharacteristicsFor(CharacteristicEnumerator.Current)
						.Where(x => x.GetBasicValue == SelectedBasicValue)
						.Select(x => $"{index++}: {x.GetValue}")
						.ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength),
					CharacteristicEnumerator.Current.Name,
					"back".Colour(Telnet.Yellow),
					"reset".Colour(Telnet.Yellow),
					CharacteristicEnumerator.Current.Name.A_An(colour: Telnet.Green),
					SelectedBasicValue.Colour(Telnet.Yellow),
					"show <pick>"
				);
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (!ShownInitialScreen && "continue".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				ShownInitialScreen = true;
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			if (ShownInitialScreen && "reset".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				ShownInitialScreen = false;
				SelectedBasicValue = "";
				CharacteristicEnumerator =
					Chargen.SelectedRace.Characteristics(Chargen.SelectedGender).GetEnumerator();
				CharacteristicEnumerator.MoveNext();
				foreach (var item in SelectedCharacteristics.ToList())
				{
					SelectedCharacteristics[item.Key] = null;
				}

				_shownValues = null;
				return Display();
			}

			if (ShownInitialScreen &&
			    CharacteristicEnumerator.Current.ChargenDisplayType == CharacterGenerationDisplayType.DisplayTable &&
			    "random".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				_shownValues = null;
				return Display();
			}

			if (ShownInitialScreen && !string.IsNullOrEmpty(SelectedBasicValue) &&
			    "back".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				SelectedBasicValue = "";
				return Display();
			}

			if (ShownInitialScreen &&
			    (!string.IsNullOrEmpty(SelectedBasicValue) || CharacteristicEnumerator.Current.ChargenDisplayType ==
				    CharacterGenerationDisplayType.DisplayTable) &&
			    command.StartsWith("show ", StringComparison.InvariantCultureIgnoreCase))
			{
				var pick = command.RemoveFirstWord();
				if (string.IsNullOrWhiteSpace(pick))
				{
					return "You must select an option to see more information about.";
				}

				var choices =
					CharacteristicEnumerator.Current.ChargenDisplayType == CharacterGenerationDisplayType.DisplayTable
						? _shownValues
						: _shownValues.Where(x => x.GetBasicValue == SelectedBasicValue).ToList();

				var choice = int.TryParse(pick, out var ivalue)
					? _shownValues.ElementAtOrDefault(ivalue - 1)
					: _shownValues.FirstOrDefault(x =>
						x.Name.StartsWith(pick, StringComparison.InvariantCultureIgnoreCase));
				if (choice == null)
				{
					return
						$"That is not a valid selection to show you more info for. Select the name or number of your desired {CharacteristicEnumerator.Current.Name}.";
					;
				}

				return
					$"Showing information for the {choice.Name.Colour(Telnet.Green)} pick:\n\nIts default value is: {choice.GetValue.Colour(Telnet.Green)}\nIts basic value is: {choice.GetBasicValue.Colour(Telnet.Green)}\nIts fancy value is: {choice.GetFancyValue.Colour(Telnet.Green)}";
			}

			int value;
			var availableChoices = GetCharacteristicsFor(CharacteristicEnumerator.Current);
			if (string.IsNullOrEmpty(SelectedBasicValue))
			{
				switch (CharacteristicEnumerator.Current.ChargenDisplayType)
				{
					case CharacterGenerationDisplayType.DisplayAll:
						var selection = int.TryParse(command, out value)
							? availableChoices.ElementAtOrDefault(value - 1)
							: availableChoices.FirstOrDefault(
								x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

						if (selection == null)
						{
							return
								$"That is not a valid selection. Select the name or number of your desired {CharacteristicEnumerator.Current.Name}.";
						}

						SelectedCharacteristics[CharacteristicEnumerator.Current] = selection;
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
								$"That is not a valid selection. Select the name or number of the general type of your desired {CharacteristicEnumerator.Current.Name}.";
						}

						SelectedBasicValue = basicSelection;
						_shownValues = GetCharacteristicsFor(CharacteristicEnumerator.Current)
						               .Where(x => x.GetBasicValue == SelectedBasicValue).ToList();
						return Display();
					case CharacterGenerationDisplayType.DisplayTable:
						selection = int.TryParse(command, out value)
							? _shownValues.ElementAtOrDefault(value - 1)
							: _shownValues.FirstOrDefault(
								x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

						if (selection == null)
						{
							return
								$"That is not a valid selection. Select the name or number of your desired {CharacteristicEnumerator.Current.Name}.";
						}

						SelectedCharacteristics[CharacteristicEnumerator.Current] = selection;
						_shownValues = null;
						break;
					default:
						throw new NotSupportedException(
							"Invalid ChargenDisplayType in CharacteristicPickerScreen.HandleCommand");
				}
			}
			else
			{
				ICharacteristicValue selection = null;
				selection = int.TryParse(command, out value)
					? availableChoices.Where(x => x.GetBasicValue == SelectedBasicValue)
					                  .ElementAtOrDefault(value - 1)
					: Chargen.SelectedEthnicity.CharacteristicChoices[CharacteristicEnumerator.Current].Values.Where
						         (x => x.GetBasicValue == SelectedBasicValue)
					         .FirstOrDefault(x =>
						         x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

				if (selection == null)
				{
					return
						$"That is not a valid selection. Select the name or number of your desired {CharacteristicEnumerator.Current.Name}.";
				}

				SelectedCharacteristics[CharacteristicEnumerator.Current] = selection;
				SelectedBasicValue = "";
				_shownValues = null;
			}

			if (CharacteristicEnumerator.MoveNext())
			{
				return Display();
			}

			State = ChargenScreenState.Complete;
			Chargen.SelectedCharacteristics =
				SelectedCharacteristics.Select(x => Tuple.Create(x.Key, x.Value)).ToList();
			return "";
		}
	}

	#region Building Commands

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
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
}