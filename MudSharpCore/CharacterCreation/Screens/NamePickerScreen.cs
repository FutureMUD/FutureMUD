using MudSharp.Character.Name;
using System.Reflection;
using MudSharp.FutureProg;

namespace MudSharp.CharacterCreation.Screens;

public class NamePickerScreenStoryboard : ChargenScreenStoryboard
{
    private NamePickerScreenStoryboard()
    {
    }

    public NamePickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
        : base(dbitem, gameworld)
    {
        XElement definition = XElement.Parse(dbitem.StageDefinition);
        XElement element = definition.Element("AllowUnicodeNames");
        if (element != null)
        {
            AllowUnicodeNames = bool.Parse(element.Value);
        }
		NameCultureOptionsProg = Gameworld.FutureProgs.Get(long.TryParse(definition.Element("NameCultureOptionsProg")?.Value, out var id) ? id : 0);
    }

    protected override string StoryboardName => "NamePicker";

    public bool AllowUnicodeNames { get; private set; }
	public IFutureProg NameCultureOptionsProg { get; private set; }

    public override ChargenStage Stage => ChargenStage.SelectName;

    public override string HelpText => $@"{BaseHelpText}
	#3allowunicode#0 - toggles allowing unicode characters in names
	#3stylesprog <prog|none>#0 - optional name-culture collection prog taking a chargen";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
			case "stylesprog":
				if (command.SafeRemainingArgument.EqualTo("none"))
				{
					NameCultureOptionsProg = null; Changed = true;
					actor.OutputHandler.Send("Optional naming styles are disabled."); return true;
				}
				var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
					ProgVariableTypes.NameCulture | ProgVariableTypes.Collection, new List<ProgVariableTypes> { ProgVariableTypes.Chargen }).LookupProg();
				if (prog is null) return false;
				NameCultureOptionsProg = prog; Changed = true;
				actor.OutputHandler.Send($"Optional naming styles now use {prog.MXPClickableFunctionName()}."); return true;
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
			new XElement("AllowUnicodeNames", AllowUnicodeNames),
			new XElement("NameCultureOptionsProg", NameCultureOptionsProg?.Id ?? 0)
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
        StringBuilder sb = new();
        sb.Append(ShowHeader(voyeur));
        sb.AppendLine();
        sb.AppendLine(
            "This screen is where characters select their names. The specifics of this screen are provided by the name culture of the character's selected culture (this screen must always come after culture selection)."
                .Wrap(voyeur.InnerLineFormatLength).ColourCommand());
        sb.AppendLine();
        sb.AppendLine($"Allow Unicode Names: {AllowUnicodeNames.ToColouredString()}");
		sb.AppendLine($"Naming Styles Prog: {NameCultureOptionsProg?.MXPClickableFunctionName() ?? "None"}");
        return sb.ToString();
    }

    internal class NamePickerScreen : ChargenScreen
    {
        protected IEnumerator<NameCultureElement> Enumerator;
        protected INameCulture NameCulture;

        protected Dictionary<NameUsage, List<string>> SelectedNameElements =
            new();

        protected NamePickerScreenStoryboard Storyboard;
		private readonly HashSet<NameUsage> _confirmed = [];
		private readonly Dictionary<NameUsage, bool> _display = [];
		private List<string> _pending = [];
		private bool _pendingConfirmed;
		private string _phase = "entry";

		private void SaveProgress()
		{
			if (!NameCulture.UseStructuredEntry || !SelectedNameElements.ContainsKey(NameUsage.BirthName)) return;
			Chargen.SelectedName = new PersonalName(NameCulture, SelectedNameElements, true, _confirmed, _display)
			{
				EntryProgress = new XElement("EntryProgress", new XAttribute("Usage", (int)Enumerator.Current.Usage),
					new XAttribute("Phase", _phase), new XAttribute("Confirmed", _pendingConfirmed),
					_pending.Select(x => new XElement("Pending", x)))
			};
		}

		private string AcceptPending(bool? display = null)
		{
			var element = Enumerator.Current;
			SelectedNameElements[element.Usage] = _pending;
			if (_pendingConfirmed) _confirmed.Add(element.Usage);
			if (display is { } shown) _display[element.Usage] = shown;
			_pending = []; _pendingConfirmed = false; _phase = "entry";
			if (Enumerator.MoveNext()) { SaveProgress(); return Display(); }
			if (!AvailableCultures().Contains(NameCulture))
			{
				ResetEntry(AvailableCultures().First());
				return "That naming style is no longer available.\n" + Display();
			}
			Chargen.SelectedName = new PersonalName(NameCulture, SelectedNameElements, true, _confirmed, _display);
			State = ChargenScreenState.Complete;
			return "Your full name is " + Chargen.SelectedName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green) + ".";
		}

		private void ResetEntry(INameCulture culture)
		{
			NameCulture = culture; SelectedNameElements.Clear(); _confirmed.Clear(); _display.Clear();
			_pending = []; _pendingConfirmed = false; _phase = "entry"; Chargen.SelectedName = null;
			Enumerator.Dispose(); Enumerator = NameCulture.NameCultureElements.GetEnumerator(); Enumerator.MoveNext();
		}


        internal NamePickerScreen(IChargen chargen, NamePickerScreenStoryboard storyboard)
            : base(chargen, storyboard)
        {
            Storyboard = storyboard;
            NameCulture =
                chargen.SelectedEthnicity?.NameCultureForGender(Chargen.SelectedGender) ??
                chargen.SelectedCulture.NameCultureForGender(Chargen.SelectedGender);
			if (chargen.SelectedName?.Culture is { } saved && AvailableCultures().Contains(saved)) NameCulture = saved;
			Enumerator = NameCulture.NameCultureElements.GetEnumerator();
            Enumerator.MoveNext();
			if (chargen.SelectedName is { } oldName && oldName.Culture.Id != NameCulture.Id) chargen.SelectedName = null;
			if (NameCulture.UseStructuredEntry && chargen.SelectedName?.Culture.Id == NameCulture.Id && chargen.SelectedName.EntryProgress is { } progress)
			{
				var savedName = chargen.SelectedName.SaveToXml();
				var target = (NameUsage)(int)progress.Attribute("Usage");
				if (NameCulture.NameCultureElements.Any(x => x.Usage == target))
				{
					foreach (var group in savedName.Elements("Element").GroupBy(x => Enum.Parse<NameUsage>(x.Attribute("usage").Value)))
					{
						SelectedNameElements[group.Key] = group.Select(x => x.Value).ToList();
						if (group.Any(x => (bool?)x.Attribute("confirmed") == true)) _confirmed.Add(group.Key);
						if (group.First().Attribute("display") is { } display) _display[group.Key] = (bool)display;
					}
					while (Enumerator.Current.Usage != target) Enumerator.MoveNext();
					_pending = progress.Elements("Pending").Select(x => x.Value).ToList();
					_pendingConfirmed = (bool?)progress.Attribute("Confirmed") == true;
					_phase = progress.Attribute("Phase")?.Value ?? "entry";
				}
			}
        }

        public override ChargenStage AssociatedStage => ChargenStage.SelectName;

		private INameCulture[] AvailableCultures() => new[]
		{
			Chargen.SelectedEthnicity?.NameCultureForGender(Chargen.SelectedGender) ?? Chargen.SelectedCulture.NameCultureForGender(Chargen.SelectedGender)
		}.Concat(Storyboard.NameCultureOptionsProg?.ExecuteCollection<INameCulture>(Chargen) ?? [])
			.Where(x => x is not null).DistinctBy(x => x.Id).ToArray();

        public override string Display()
        {
            if (!ShownChargenAdvice && HasChargenAdvice())
            {
                return DisplayChargenAdvice();
            }

            StringBuilder sb = new();
            sb.AppendLine((Enumerator.Current.Name.TitleCase() + " Selection").Colour(Telnet.Cyan));
			sb.AppendLine();
			var cultures = AvailableCultures();
			if (cultures.Length > 1)
			{

                sb.AppendLine($"You have multiple naming styles available ({cultures.Select((x, i) => $"{i + 1}. {x.Name}").ListToColouredString()})");
                sb.AppendLine($"Currently selected: {NameCulture.Name.ColourName()}");
				sb.AppendLine($"Naming style: {NameCulture.Name.ColourName()}");
				sb.AppendLine(cultures.Select((x, i) => $"{i + 1}. {x.Name}").ListToString());
				sb.AppendLine();
			}
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(
                Enumerator.Current.ChargenBlurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength));
            sb.AppendLine();
            sb.AppendLine();
            IRandomNameProfile[] rns = Chargen.Gameworld.RandomNameProfiles
                             .Where(x => x.Culture == NameCulture)
                             .Where(x => x.IsCompatibleGender(Chargen.SelectedGender))
                             .Where(x => x.UseForChargenNameSuggestions(Chargen))
							 .Where(x => x.RandomNames.TryGetValue(Enumerator.Current.Usage, out var names) && names.Any())
                             .ToArray();
            string[] suggestedNames = rns
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
								 .Where(x => !string.IsNullOrWhiteSpace(x))
                                 .Distinct()
                                 .PickUpToRandom(5)
                                 .ToArray();
            if (suggestedNames.Any())
            {
                sb.AppendLine($"Examples: {suggestedNames.Select(x => x.ColourName()).ListToString()}");
                sb.AppendLine();
            }

			var current = Enumerator.Current;
			var supplied = current.ParentUsage is { } parent && SelectedNameElements.TryGetValue(parent, out var parents) && parents.Count == 1 &&
				current.ParentValues.TryGetValue(parents[0], out var children) ? children : current.SuggestedValues;
			if (supplied.Count > 0) sb.AppendLine("Suggested Examples: " + supplied.ListToColouredString());
			if (_phase != "entry")
			{
				sb.AppendLine(string.Join(", ", _pending));
				sb.AppendLine((_phase == "confirm" ? current.ConfirmationPrompt : current.DisplayChoicePrompt) + " Answer yes or no.");
				return sb.ToString();
			}
			if (current.MaximumCount < 0)
			{
                if (cultures.Length > 1)
                {
                    sb.AppendLine($"Enter any number of {current.Name.Pluralise()}, or leave blank. Quote each multiword value separately.\nOptional: {"/style <number>".ColourCommand()} changes the name style and restarts name entry.");
                    return sb.ToString();
                }

				sb.AppendLine($"Enter any number of {current.Name.Pluralise()}, or leave blank. Quote each multiword value separately.");
				return sb.ToString();
			}
            sb.Append("Please select ");
            sb.Append(Enumerator.Current.MinimumCount == Enumerator.Current.MaximumCount
                ? $"exactly {Enumerator.Current.MinimumCount} "
                : $"between {Enumerator.Current.MinimumCount} and {Enumerator.Current.MaximumCount} ");
            sb.Append(Enumerator.Current.MaximumCount > 1
                ? Enumerator.Current.Name.Pluralise()
                : Enumerator.Current.Name);
            sb.AppendLine(Enumerator.Current.MaximumCount > 1 ? ", seperated by spaces" : "");
			if (cultures.Length > 1)
			{
				sb.AppendLine($"Optional: {"/style <number>".ColourCommand()} changes the name style and restarts name entry.");
			}
			return sb.ToString();
        }

        public override string HandleCommand(string command)
        {
            if (!ShownChargenAdvice && HasChargenAdvice())
            {
                return HandleCommandChargenAdvice(command);
            }
			if (command.StartsWith("/style ", StringComparison.OrdinalIgnoreCase))
			{
				var cultures = AvailableCultures();
				if (!int.TryParse(command[7..].Trim(), out var choice) || choice < 1 || choice > cultures.Length)
					return "Select a naming style number from the displayed list.";
				ResetEntry(cultures[choice - 1]);
				return Display();
			}

            NameCultureElement nce = Enumerator.Current!;

			if (_phase != "entry")
			{
				if (!command.EqualTo("yes") && !command.EqualTo("no")) return "Answer yes or no.\n" + Display();
				if (_phase == "confirm")
				{
					if (command.EqualTo("no")) { _pending = []; _phase = "entry"; SaveProgress(); return Display(); }
					_pendingConfirmed = true;
					if (nce.DisplayChoicePrompt is null) return AcceptPending();
					_phase = "display"; SaveProgress(); return Display();
				}
				return AcceptPending(command.EqualTo("yes"));
			}
            StringStack ss = new(command.Normalize(NormalizationForm.FormC));
            if (ss.IsFinished && nce.MinimumCount > 0)
            {
                return Display();
            }

			List<string> names = ss.IsFinished ? [] : nce.MaximumCount == 1 ?
				[NameCulture.PreserveNameCase ? ss.SafeRemainingArgument : ss.SafeRemainingArgument.ToTitleCaseAP()] :
				ss.PopSpeechAll().Select(x => NameCulture.PreserveNameCase ? x : x.ToLowerInvariant()).ToList();
            if (!nce.AcceptsCount(names.Count))
            {
                return
                    $"You must select {(nce.MinimumCount == nce.MaximumCount ? $"exactly {nce.MinimumCount}" : $"between {Enumerator.Current.MinimumCount} and {Enumerator.Current.MaximumCount}")} {(nce.MaximumCount > 1 ? nce.Name.Pluralise() : nce.Name)}.";
            }

            if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
            {
                return "Your names must not contain any duplicates.";
            }

			if (names.Any(x => NameEntryValidation.HasInvalidCharacters(x, NameCulture.AllowTypographicPunctuation)))
            {
                return "Your names may only contain letters, spaces, apostrophes and hyphens.";
            }

            if (names.Any(x => !x.Any(char.IsLetter)))
            {
                return "Your name must include at least one letter.";
            }

			if (names.Any(x => NameEntryValidation.HasConsecutivePunctuation(x, NameCulture.AllowTypographicPunctuation)))
            {
                return "Your names may not contain consecutive apostrophes or hyphens.";
            }

            if (!Storyboard.AllowUnicodeNames && names.Any(x => x.Any(y => y > 255)))
            {
                return "Your names may not contain Unicode characters.";
            }

			if (nce.ValidateValues(names, SelectedNameElements) is { } error) return error;
			var proposed = new Dictionary<NameUsage, List<string>>(SelectedNameElements) { [nce.Usage] = names };
			foreach (var rule in NameCulture.NameCultureElements.Where(x => proposed.ContainsKey(x.Usage)))
				if (rule.ValidateValues(proposed[rule.Usage], proposed) is { } ruleError) return ruleError;
			_pending = names;
			if (names.Count > 0 && nce.ConfirmationPrompt is not null)
			{
				_phase = "confirm"; SaveProgress(); return Display();
			}
			if (names.Count > 0 && nce.DisplayChoicePrompt is not null)
			{
				_phase = "display"; SaveProgress(); return Display();
			}
			return AcceptPending();
        }
    }
}
