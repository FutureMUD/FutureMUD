using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Knowledge;

namespace MudSharp.CharacterCreation.Screens;

internal class KnowledgePickerBySkillScreenStoryboard : ChargenScreenStoryboard
{
	private KnowledgePickerBySkillScreenStoryboard()
	{
	}

	public KnowledgePickerBySkillScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb")?.Value ?? "No blurb has been written.";

		var element = definition.Element("NumberOfPicksProg");
		NumberOfPicksProg = long.TryParse(element?.Value ?? "0", out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		if (NumberOfPicksProg == null)
		{
			throw new ApplicationException(
				"KnowledgePickerBySkillScreenStoryboard definition error - NumberOfPicksProg was not a valid prog or was missing.");
		}

		if (NumberOfPicksProg.ReturnType != FutureProgVariableTypes.Number)
		{
			throw new ApplicationException(
				$"KnowledgePickerBySkillScreenStoryboard definition error - NumberOfPicksProg was prog #{NumberOfPicksProg.Id} ({NumberOfPicksProg.FunctionName}), which does not return a number.");
		}

		if (
			!NumberOfPicksProg.MatchesParameters(new[] { FutureProgVariableTypes.Toon, FutureProgVariableTypes.Trait }))
		{
			throw new ApplicationException(
				$"KnowledgePickerBySkillScreenStoryboard definition error - NumberOfPicksProg was prog #{NumberOfPicksProg.Id} ({NumberOfPicksProg.FunctionName}), which does not match input parameter types of toon, trait.");
		}

		element = definition.Element("FreeKnowledgesProg");
		FreeKnowledgesProg = long.TryParse(element?.Value ?? "0", out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		if (FreeKnowledgesProg == null)
		{
			throw new ApplicationException(
				"KnowledgePickerBySkillScreenStoryboard definition error - FreeKnowledgesProg was not a valid prog or was missing.");
		}

		if (!FreeKnowledgesProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Collection |
		                                                  FutureProgVariableTypes.Knowledge))
		{
			throw new ApplicationException(
				$"KnowledgePickerBySkillScreenStoryboard definition error - FreeKnowledgesProg was prog #{FreeKnowledgesProg.Id} ({FreeKnowledgesProg.FunctionName}), which does not return a collection of knowledges.");
		}

		if (!FreeKnowledgesProg.MatchesParameters(new[] { FutureProgVariableTypes.Chargen }))
		{
			throw new ApplicationException(
				$"KnowledgePickerBySkillScreenStoryboard definition error - FreeKnowledgesProg was prog #{FreeKnowledgesProg.Id} ({FreeKnowledgesProg.FunctionName}), which does not match input parameter types of chargen.");
		}
	}

	protected override string StoryboardName => "KnowledgePickerBySkill";
	public string Blurb { get; protected set; }
	public IFutureProg NumberOfPicksProg { get; protected set; }
	public IFutureProg FreeKnowledgesProg { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectKnowledges;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("FreeKnowledgesProg", FreeKnowledgesProg?.Id ?? 0),
			new XElement("NumberOfPicksProg", NumberOfPicksProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new KnowledgePickerBySkillScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		var freeKnowledges = ((IList<IFutureProgVariable>)FreeKnowledgesProg.Execute(chargen)).OfType<IKnowledge>()
			.ToList();
		foreach (var resource in Gameworld.ChargenResources)
		{
			var sum = 0;
			foreach (var knowledge in chargen.SelectedKnowledges)
			{
				if (freeKnowledges.Contains(knowledge))
				{
					continue;
				}

				sum += knowledge.ResourceCost(resource);
			}

			yield return (resource, sum);
		}
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectKnowledges,
			new ChargenScreenStoryboardFactory("KnowledgePickerBySkill",
				(game, dbitem) => new KnowledgePickerBySkillScreenStoryboard(game, dbitem)),
			"KnowledgePickerBySkill",
			"Select knowledges for each skill that has associated",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen gives the opportunity to select knowledges for the character on a skill-by-skill basis."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine(
			$"Free Knowledges Prog: {FreeKnowledgesProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		sb.AppendLine($"# Picks Prog: {NumberOfPicksProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class KnowledgePickerBySkillScreen : ChargenScreen
	{
		protected IReadOnlyCollection<IKnowledge> CurrentKnowledgesAvailable;
		protected List<IKnowledge> CurrentPicks = new();
		protected List<IKnowledge> FreeKnowledges;
		protected bool ShownIntroduction;
		protected IEnumerator<(ITraitDefinition Trait, IEnumerable<IKnowledge> Knowledges)> SkillEnumerator;
		protected KnowledgePickerBySkillScreenStoryboard Storyboard;
		protected bool WarnedAboutFewerPicks;

		internal KnowledgePickerBySkillScreen(IChargen chargen, KnowledgePickerBySkillScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			ResetChargenScreen();
		}

		public override ChargenStage AssociatedStage => Storyboard.Stage;

		public void ResetChargenScreen()
		{
			Chargen.SelectedKnowledges.Clear();
			CurrentPicks.Clear();
			ShownIntroduction = false;
			WarnedAboutFewerPicks = false;
			FreeKnowledges = ((IList<IFutureProgVariable>)Storyboard.FreeKnowledgesProg.Execute(Chargen))
			                 .OfType<IKnowledge>().ToList();
			Chargen.SelectedKnowledges.AddRange(FreeKnowledges);
			SkillEnumerator =
				Chargen.SelectedSkills.Select(
					       x =>
						       (x,
							       Storyboard.Gameworld.Knowledges.Where(
								                 y =>
									                 y.Learnable.HasFlag(LearnableType.LearnableAtChargen) &&
									                 !Chargen.SelectedKnowledges.Contains(y) &&
									                 ((bool?)y.CanPickChargenProg?.Execute(Chargen, x) ?? false))
							                 .ToList()
							                 .AsEnumerable()))
				       .Where(x => x.Item2.Any())
				       .GetEnumerator();
			EnumerateNext();
		}

		private bool EnumerateNext()
		{
			if (!SkillEnumerator.MoveNext())
			{
				State = ChargenScreenState.Complete;
				return false;
			}

			CurrentKnowledgesAvailable = SkillEnumerator.Current.Knowledges
			                                            .Where(x => !Chargen.SelectedKnowledges.Contains(x))
			                                            .ToList();

			if (CurrentKnowledgesAvailable.Count == 0)
			{
				return EnumerateNext();
			}

			if (CurrentKnowledgesAvailable.Count == 1)
			{
				Chargen.SelectedKnowledges.Add(CurrentKnowledgesAvailable.Single());
				return EnumerateNext();
			}

			return true;
		}

		public override string Display()
		{
			if (!ShownIntroduction)
			{
				return
					string.Format(
						"Knowledge Selection".Colour(Telnet.Cyan) +
						"\n\n{0}\n\nType {1} to begin, or {2} at any time to return to this screen.",
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

			var choice = 1;
			return
				$@"#6Knowledges for {SkillEnumerator.Current.Trait.Name}#0

You already have the following knowledges: {Chargen.SelectedKnowledges.Select(x => x.Description.Colour(Telnet.Cyan)).ListToString()}

{CurrentKnowledgesAvailable.OrderBy(x => x.Name).Select(x => $"{choice++}: {(CurrentPicks.Contains(x) ? x.Description.TitleCase().Colour(Telnet.Green).Parentheses() : x.Description.TitleCase())}").ArrangeStringsOntoLines((uint)Account.LineFormatLength / 55, (uint)Account.LineFormatLength)}

You are allowed #2{Storyboard.NumberOfPicksProg.Execute(Chargen, SkillEnumerator.Current.Trait)}#0 picks. 

Enter the name or number of the knowledge you would like to select, and #3done#0 to finish.".SubstituteANSIColour();
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (!ShownIntroduction)
			{
				if ("continue".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					ShownIntroduction = true;
					return Display();
				}

				return "Type " + "continue".Colour(Telnet.Yellow) + " to begin.";
			}

			if ("reset".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				ResetChargenScreen();
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			int value;
			IKnowledge selection;

			if (command.StartsWith("help", StringComparison.InvariantCultureIgnoreCase))
			{
				var which = command.RemoveFirstWord();
				if (string.IsNullOrWhiteSpace(which))
				{
					return "Which knowledge would you like to display help for?";
				}

				selection = int.TryParse(which, out value)
					? CurrentKnowledgesAvailable.OrderBy(x => x.Name).ElementAtOrDefault(value - 1)
					: CurrentKnowledgesAvailable.OrderBy(x => x.Name)
					                            .FirstOrDefault(
						                            x => x.Description.Contains(which,
							                            StringComparison.InvariantCultureIgnoreCase));
				return selection == null
					? "There is no such knowledge for which you can display help."
					: $"Help for {selection.Description.Colour(Telnet.Cyan)}\n\n{selection.LongDescription}";
			}


			var picks = Storyboard.NumberOfPicksProg.ExecuteInt(Chargen, SkillEnumerator.Current.Trait);
			if ("done".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (CurrentPicks.Count > picks)
				{
					return "You have picked too many knowledges. Unselect some knowledges to continue.";
				}

				if (CurrentPicks.Count < picks)
				{
					if (!WarnedAboutFewerPicks)
					{
						WarnedAboutFewerPicks = true;
						return
							CommonStringUtilities.CultureFormat(
								$"You have selected only {CurrentPicks.Count:N0} picks, but you are allowed to select {picks:N0}. Type {"done".Colour(Telnet.Yellow)} to accept less than your allowance.",
								Account.Culture);
					}
				}

				Chargen.SelectedKnowledges.AddRange(CurrentPicks);
				CurrentPicks.Clear();
				WarnedAboutFewerPicks = false;
				if (!EnumerateNext())
				{
					return "";
				}

				return Display();
			}

			selection = int.TryParse(command, out value)
				? CurrentKnowledgesAvailable.OrderBy(x => x.Name).ElementAtOrDefault(value - 1)
				: CurrentKnowledgesAvailable.OrderBy(x => x.Name)
				                            .FirstOrDefault(
					                            x => x.Description.Contains(command,
						                            StringComparison.InvariantCultureIgnoreCase));

			if (selection == null)
			{
				return "That is not a valid knowledge for you to pick.";
			}

			if (CurrentPicks.Contains(selection))
			{
				CurrentPicks.Remove(selection);
			}
			else
			{
				if (CurrentPicks.Count == picks)
				{
					return
						$"You have already selected the maximum number of knowledges for {SkillEnumerator.Current.Trait.Name.ColourValue()}.";
				}

				CurrentPicks.Add(selection);
			}

			return Display();
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3free <prog>#0 - sets the prog for free knowledges
	#3picks <#>#0 - sets the number of free picks per skill";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "free":
				return BuildingCommandFree(actor, command);
			case "picks":
				return BuildingCommandPicks(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandPicks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control the number of picks per skill?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<IEnumerable<FutureProgVariableTypes>>
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen,
					FutureProgVariableTypes.Trait
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		NumberOfPicksProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control the number of picks per skill in character creation.");
		return true;
	}

	private bool BuildingCommandFree(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control the knowledges a character gets for free?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Collection | FutureProgVariableTypes.Knowledge, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		FreeKnowledgesProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control the knowledges a character gets for free in character creation.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{Blurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
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