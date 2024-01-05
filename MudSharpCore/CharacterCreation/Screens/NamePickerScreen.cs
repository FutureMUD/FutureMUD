using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Framework;

namespace MudSharp.CharacterCreation.Screens;

public class NamePickerScreenStoryboard : ChargenScreenStoryboard
{
	private NamePickerScreenStoryboard()
	{
	}

	public NamePickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		var element = definition.Element("AllowUnicodeNames");
		if (element != null)
		{
			AllowUnicodeNames = bool.Parse(element.Value);
		}
	}

	protected override string StoryboardName => "NamePicker";

	public bool AllowUnicodeNames { get; private set; }

	public override ChargenStage Stage => ChargenStage.SelectName;

	public override string HelpText => $@"{BaseHelpText}
	#3allowunicode#0 - toggles allowing unicode characters in names";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "unicode":
			case "allowunicode":
				return BuildingCommandAllowUnicodeNames(actor);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandAllowUnicodeNames(ICharacter actor)
	{
		AllowUnicodeNames = !AllowUnicodeNames;
		Changed = true;
		actor.OutputHandler.Send(
			$"Players will {(AllowUnicodeNames ? "now" : "no longer")} be allowed to use unicode characters in names.");
		return true;
	}

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("AllowUnicodeNames", AllowUnicodeNames)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectName,
			new ChargenScreenStoryboardFactory("NamePicker",
				(game, dbitem) => new NamePickerScreenStoryboard(game, dbitem)),
			"NamePicker",
			"Enter a name for each name element",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new NamePickerScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen is where characters select their names. The specifics of this screen are provided by the name culture of the character's selected culture (this screen must always come after culture selection)."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Allow Unicode Names: {AllowUnicodeNames.ToColouredString()}");
		return sb.ToString();
	}

	internal class NamePickerScreen : ChargenScreen
	{
		private static readonly Regex _nameRegex = new(@"['-]\s*['-]");
		protected IEnumerator<NameCultureElement> Enumerator;
		protected INameCulture NameCulture;

		protected Dictionary<NameUsage, List<string>> SelectedNameElements =
			new();

		protected NamePickerScreenStoryboard Storyboard;

		internal NamePickerScreen(IChargen chargen, NamePickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			NameCulture = chargen.SelectedCulture.NameCultureForGender(Chargen.SelectedGender);
			Enumerator = NameCulture.NameCultureElements.GetEnumerator();
			Enumerator.MoveNext();
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectName;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			var sb = new StringBuilder();
			sb.AppendLine((Enumerator.Current.Name + " Selection").Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine(
				Enumerator.Current.ChargenBlurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength));
			sb.AppendLine();
			sb.AppendLine();
			var rns = Chargen.Gameworld.RandomNameProfiles
			                 .Where(x => x.Culture == NameCulture)
			                 .Where(x => x.IsCompatibleGender(Chargen.SelectedGender))
			                 .Where(x => x.UseForChargenNameSuggestions(Chargen))
			                 .ToArray();
			var suggestedNames = rns
			                     .SelectMany(x =>
				                     new[]
				                     {
					                     x.GetRandomNameElement(Enumerator.Current.Usage),
					                     x.GetRandomNameElement(Enumerator.Current.Usage),
					                     x.GetRandomNameElement(Enumerator.Current.Usage),
					                     x.GetRandomNameElement(Enumerator.Current.Usage),
					                     x.GetRandomNameElement(Enumerator.Current.Usage)
				                     }
			                     )
			                     .Distinct()
			                     .PickUpToRandom(5)
			                     .ToArray();
			if (suggestedNames.Any())
			{
				sb.AppendLine($"Examples: {suggestedNames.Select(x => x.ColourName()).ListToString()}");
				sb.AppendLine();
			}

			sb.Append("Please select ");
			sb.Append(Enumerator.Current.MinimumCount == Enumerator.Current.MaximumCount
				? $"exactly {Enumerator.Current.MinimumCount} "
				: $"between {Enumerator.Current.MinimumCount} and {Enumerator.Current.MaximumCount} ");
			sb.Append(Enumerator.Current.MaximumCount > 1
				? Enumerator.Current.Name.Pluralise()
				: Enumerator.Current.Name);
			sb.AppendLine(Enumerator.Current.MaximumCount > 1 ? ", seperated by spaces" : "");

			return sb.ToString();
		}

		public override string HandleCommand(string command)
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			var ss = new StringStack(command);
			if (ss.IsFinished && Enumerator.Current.MinimumCount > 0)
			{
				return Display();
			}

			ss.PopSpeechAll();
			var names = ss.Memory.Select(x => x.ToLowerInvariant()).ToList();
			if (names.Count > Enumerator.Current.MaximumCount || names.Count < Enumerator.Current.MinimumCount)
			{
				return
					$"You must select {(Enumerator.Current.MinimumCount == Enumerator.Current.MaximumCount ? $"exactly {Enumerator.Current.MinimumCount}" : $"between {Enumerator.Current.MinimumCount} and {Enumerator.Current.MaximumCount}")} {(Enumerator.Current.MaximumCount > 1 ? Enumerator.Current.Name.Pluralise() : Enumerator.Current.Name)}.";
			}

			if (names.Distinct().Count() != names.Count)
			{
				return "Your names must not contain any duplicates.";
			}

			if (names.Any(x => x.Any(y => !char.IsLetter(y) && y != ' ' && y != '\'' && y != '-')))
			{
				return "Your names may only contain letters, spaces, apostrophes and hyphens.";
			}

			if (names.Any(x => !x.Any(char.IsLetter)))
			{
				return "Your name must include at least one letter.";
			}

			if (names.Any(x => _nameRegex.IsMatch(x)))
			{
				return "Your names may not contain consecutive apostrophes or hyphens.";
			}

			if (!Storyboard.AllowUnicodeNames && names.Any(x => x.Any(y => y > 255)))
			{
				return "Your names may not contain Unicode characters.";
			}

			SelectedNameElements.Add(Enumerator.Current.Usage, names);
			if (Enumerator.MoveNext())
			{
				return Display();
			}

			State = ChargenScreenState.Complete;
			Chargen.SelectedName =
				new PersonalName(Chargen.SelectedCulture.NameCultureForGender(Chargen.SelectedGender),
					SelectedNameElements, true);
			return "Your full name is " +
			       Chargen.SelectedName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green) +
			       ".\n";
		}
	}
}