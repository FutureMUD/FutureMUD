using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class RolePickerScreenStoryboard : ChargenScreenStoryboard
{
	private RolePickerScreenStoryboard()
	{
	}

	public RolePickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		var element = definition.Element("IntroductionBlurb");
		if (element != null)
		{
			IntroductionBlurb = element.Value;
		}

		TypeBlurbs = new Dictionary<ChargenRoleType, string>();
		CanSelectNone = new Dictionary<ChargenRoleType, bool>();
		TypeNames = new Dictionary<ChargenRoleType, string>();

		element = definition.Element("RoleTypes");
		if (element != null)
		{
			foreach (var sub in element.Elements("RoleType"))
			{
				var type = int.TryParse(sub.Attribute("Type").Value, out var value)
					? (ChargenRoleType)value
					: (ChargenRoleType)Enum.Parse(typeof(ChargenRoleType), sub.Attribute("Type").Value, true);
				TypeNames[type] = sub.Attribute("Name").Value;
				CanSelectNone[type] = bool.Parse(sub.Attribute("CanSelectNone").Value);
				TypeBlurbs[type] = sub.Value;
			}
		}

		foreach (var type in Enum.GetValues<ChargenRoleType>().Except(ChargenRoleType.StartingLocation))
		{
			if (TypeNames.ContainsKey(type))
			{
				continue;
			}

			TypeNames[type] = type.DescribeEnum(true);
			switch (type)
			{
				case ChargenRoleType.Class:
					TypeBlurbs[type] =
						@"Your class determines the broad capabilities of your character. It influences the skills that you have available at character creation as well as throughout your career and may also give you other special abilities.";
					CanSelectNone[type] = false;
					break;
				case ChargenRoleType.Subclass:
					TypeBlurbs[type] =
						@"Your subclass is a further refinement of your chosen class that helps round out your character, representing secondary capabilities that you have acquired.";
					CanSelectNone[type] = false;
					break;
				case ChargenRoleType.Profession:
					TypeBlurbs[type] =
						@"Your profession represents a job, employment, or vocation that your character has coming into the game. It might be used to give you additional starting skills, a starting clan, starting money, or something of that nature.";
					CanSelectNone[type] = false;
					break;
				case ChargenRoleType.Family:
					TypeBlurbs[type] =
						@"Family roles are designed to represent things related to the family origins of the character, whether they be a noble character selecting a great house to which they belong, or even just a player-sponsored role to make an in-character family relation. Typically a family role would be more likely to give you clan membership and possibly gear or money than skill boosts, for example.";
					CanSelectNone[type] = true;
					break;
				case ChargenRoleType.Story:
					TypeBlurbs[type] =
						@"Story roles represent unique opportunities for the character, or plot-driven back-stories. If you choose to take one of these roles, you have some unique role in the story and metaplot.";
					CanSelectNone[type] = true;
					break;
				case ChargenRoleType.Childhood:
					TypeBlurbs[type] =
						@"A childhood role is all about what your childhood was like. Your childhood is an important formative time for your later years, and the circumstances of your childhood may end up controlling some of your trajectory later in life.";
					CanSelectNone[type] = false;
					break;
				case ChargenRoleType.Education:
					TypeBlurbs[type] =
						@"An education role determines what kind of formal schooling your character had. This may control what other opportunities are available to you.";
					CanSelectNone[type] = false;
					break;
				case ChargenRoleType.ImmediatePast:
					TypeBlurbs[type] =
						@"Immediate Past roles are all about what happened immediately before you entered into the game. They are your reason for suddenly appearing in the game world.";
					CanSelectNone[type] = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	protected override string StoryboardName => "RolePicker";

	public string IntroductionBlurb { get; protected set; }
	public Dictionary<ChargenRoleType, string> TypeBlurbs { get; protected set; }
	public Dictionary<ChargenRoleType, bool> CanSelectNone { get; protected set; }
	public Dictionary<ChargenRoleType, string> TypeNames { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectRole;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("IntroductionBlurb", new XCData(IntroductionBlurb)),
			new XElement("RoleTypes",
				from role in TypeBlurbs.Keys
				select new XElement("RoleType",
					new XAttribute("Type", (int)role),
					new XAttribute("Name", TypeNames[role]),
					new XAttribute("CanSelectNone", CanSelectNone[role]),
					new XCData(TypeBlurbs[role])
				)
			)
		).ToString();
	}

	#endregion

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new RolePickerScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen is where people choose class/subclass (if your MUD has those) as well as other character creation roles. It will always skip screens that have no choices, so if there is a type that has no choices available they will not see the blurb screen or selection screen for that type."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Introduction Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(IntroductionBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());

		var roleTypes = new List<ChargenRoleType>
		{
			ChargenRoleType.Class,
			ChargenRoleType.Subclass,
			ChargenRoleType.Family,
			ChargenRoleType.Childhood,
			ChargenRoleType.Education,
			ChargenRoleType.Profession,
			ChargenRoleType.ImmediatePast,
			ChargenRoleType.Story
		};
		foreach (var type in roleTypes)
		{
			sb.AppendLine();
			sb.AppendLine(type.DescribeEnum(true).GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine($"Choice Name: {TypeNames[type].ColourValue()}");
			sb.AppendLine($"Can Choose None: {CanSelectNone[type].ToColouredString()}");
			sb.AppendLine(
				$"# Roles Available: {Gameworld.Roles.Count(x => x.RoleType == type && !x.Expired).ToString("N0", voyeur).ColourValue()}");
			sb.AppendLine();
			sb.AppendLine(TypeBlurbs[type].Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		}

		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectRole,
			new ChargenScreenStoryboardFactory("RolePicker",
				(game, dbitem) => new RolePickerScreenStoryboard(game, dbitem)),
			"RolePicker",
			"Select roles from each category",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		foreach (var resource in Gameworld.ChargenResources)
		{
			var sum = 0;
			foreach (var role in chargen.SelectedRoles)
			{
				sum += role.ResourceCost(resource);
			}

			yield return (resource, sum);
		}
	}

	internal class RolePickerScreen : ChargenScreen
	{
		private readonly RolePickerScreenStoryboard Storyboard;
		private List<IChargenRole> _roles;
		private IChargenRole _selectedRole;
		private ChargenRoleType CurrentStage;
		private bool ShownIntroduction;

		public RolePickerScreen(IChargen chargen, RolePickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			DoInitialSetup();
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectRole;

		protected void DoInitialSetup()
		{
			InitialStage();
			if (Enum.GetValues<ChargenRoleType>().Except(ChargenRoleType.StartingLocation)
			        .All(x => GetRoles(x).Count == 0))
			{
				State = ChargenScreenState.Complete;
			}
		}

		private List<IChargenRole> GetRoles(ChargenRoleType type)
		{
			return
				Chargen.Gameworld.Roles.OrderBy(x => x.Name)
				       .Where(x => x.RoleType == type)
				       .Where(x => x.ChargenAvailable(Chargen))
				       .ToList();
		}

		public override string Display()
		{
			if (!ShownIntroduction)
			{
				return
					$"{"Role Selection".Colour(Telnet.Cyan)}\n\n{Storyboard.IntroductionBlurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}\n\nPlease type {"continue".Colour(Telnet.Yellow)} to begin. At any time, you may type {"reset".Colour(Telnet.Yellow)} to return to the beginning.";
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (_selectedRole == null)
			{
				var index = 1;
				var longestName = _roles.Max(x => x.Name.Length);
				var columns = Math.Min(Chargen.Account.LineFormatLength / longestName, 3);
				return
					$"{$"{Storyboard.TypeNames[CurrentStage]} Selection".Colour(Telnet.Cyan)}\n\n{Storyboard.TypeBlurbs[CurrentStage].SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}\n\n{_roles.Select(x => $"{index++}: {x.Name.TitleCase()}").ArrangeStringsOntoLines((uint)columns, (uint)Chargen.Account.LineFormatLength)}\n\nType the name or number of your desired {Storyboard.TypeNames[CurrentStage]}{(Storyboard.CanSelectNone[CurrentStage] ? ", or 0 to select none" : "")}.";
			}

			return
				$"{_selectedRole.Name.TitleCase().Colour(Telnet.Cyan)}\n\n{_selectedRole.ChargenBlurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}\n\n{(Storyboard.Gameworld.ChargenResources.Any(x => _selectedRole.ResourceCost(x) > 0) ? $"This selection costs {Storyboard.Gameworld.ChargenResources.Where(x => _selectedRole.ResourceCost(x) > 0).Select(x => Tuple.Create(x, _selectedRole.ResourceCost(x))).Select(x => CommonStringUtilities.CultureFormat($"{x.Item2} {(x.Item2 == 1 ? x.Item1.Name.TitleCase() : x.Item1.PluralName.TitleCase())}", Account).Colour(Telnet.Green)).ListToString()}\n\n" : "")}Do you want to select this {Storyboard.TypeNames[CurrentStage]}? Type {"yes".Colour(Telnet.Yellow)} or {"no".Colour(Telnet.Yellow)}.";
		}

		private void InitialStage()
		{
			Chargen.SelectedRoles.RemoveAll(x => x.RoleType != ChargenRoleType.StartingLocation);
			ShownIntroduction = false;
			_selectedRole = null;
			CurrentStage = ChargenRoleType.Class;
		}

		private bool AdvanceStage()
		{
			_selectedRole = null;
			ChargenRoleType nextStage;
			switch (CurrentStage)
			{
				case ChargenRoleType.Class:
					nextStage = ChargenRoleType.Subclass;
					break;
				case ChargenRoleType.Subclass:
					nextStage = ChargenRoleType.Family;
					break;
				case ChargenRoleType.Family:
					nextStage = ChargenRoleType.Childhood;
					break;
				case ChargenRoleType.Childhood:
					nextStage = ChargenRoleType.Education;
					break;
				case ChargenRoleType.Education:
					nextStage = ChargenRoleType.Profession;
					break;
				case ChargenRoleType.Profession:
					nextStage = ChargenRoleType.ImmediatePast;
					break;
				case ChargenRoleType.ImmediatePast:
					nextStage = ChargenRoleType.Story;
					break;
				case ChargenRoleType.Story:
					_roles = null;
					return false;
				default:
					throw new NotSupportedException("Unsupported ChargenRoleType in RolePickerScreen.AdvanceStage()");
			}

			CurrentStage = nextStage;
			_roles = GetRoles(CurrentStage);
			return _roles.Any() || AdvanceStage();
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				return Display();
			}

			if ("reset".Equals(command, StringComparison.InvariantCultureIgnoreCase))
			{
				InitialStage();
				return Display();
			}

			if (!ShownIntroduction)
			{
				if ("continue".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					ShownIntroduction = true;
					_roles = GetRoles(CurrentStage);
					if (!_roles.Any())
					{
						if (!AdvanceStage())
						{
							State = ChargenScreenState.Complete;
							return "You do not have any roles from which to select.";
						}
					}
				}

				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			if (_selectedRole == null)
			{
				if (int.TryParse(command, out var value))
				{
					if (value == 0 && Storyboard.CanSelectNone[CurrentStage])
					{
						var currentStage = Storyboard.TypeNames[CurrentStage];
						if (AdvanceStage())
						{
							return $"You decline to select {currentStage.A_An()}.\n{Display()}";
						}

						State = ChargenScreenState.Complete;
						return
							$"You decline to select {currentStage.A_An()}.\nYou have finished selecting roles.\n";
					}

					_selectedRole = _roles.ElementAtOrDefault(value - 1);
				}
				else
				{
					_selectedRole =
						_roles.FirstOrDefault(
							x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
				}

				return _selectedRole == null ? "That is not a valid selection." : Display();
			}

			if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				Chargen.SelectedRoles.Add(_selectedRole);
				if (AdvanceStage())
				{
					return Display();
				}

				State = ChargenScreenState.Complete;
				return "You have finished selecting roles.\n";
			}

			_selectedRole = null;
			return Display();
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the introduction blurb
	#3<type> canskip#0 - toggles being able to skip choosing this type
	#3<type> name <name>#0 - renames the choice of this type
	#3<type> blurb#0 - drops you into an editor to change the blurb for this type

The valid types are:

	#6class#0, #6subclass#0, #6family#0, #6childhood#0, #6education#0, #6profession#0, #6immediatepast#0 and #6story#0.";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
		}

		if (command.Last.TryParseEnum<ChargenRoleType>(out var roleType))
		{
			return BuildingCommandRoleType(actor, command, roleType);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandRoleType(ICharacter actor, StringStack command, ChargenRoleType roleType)
	{
		switch (command.PopForSwitch())
		{
			case "canskip":
			case "skip":
				return BuildingCommandRoleSkip(actor, roleType);
			case "name":
			case "rename":
				return BuildingCommandRoleName(actor, roleType, command);
			case "blurb":
				return BuildingCommandRoleBlurb(actor, roleType);

			default:
				actor.OutputHandler.Send(
					"You can use the options #3canskip#0, #3name#0 or #3blurb#0 with this role type."
						.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandRoleBlurb(ICharacter actor, ChargenRoleType roleType)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{TypeBlurbs[roleType].SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostRoleBlurb, CancelRoleBlurb, 1.0, TypeBlurbs[roleType], EditorOptions.None, new object[]
		{
			actor.InnerLineFormatLength,
			roleType
		});
		return true;
	}

	private void CancelRoleBlurb(IOutputHandler handler, object[] args)
	{
		var roleType = (ChargenRoleType)args[1];
		handler.Send($"You decide not to change the blurb for the {roleType.DescribeEnum().ColourName()} role type.");
	}

	private void PostRoleBlurb(string text, IOutputHandler handler, object[] args)
	{
		var roleType = (ChargenRoleType)args[1];
		TypeBlurbs[roleType] = text;
		Changed = true;
		handler.Send(
			$"You set the {roleType.DescribeEnum().ColourName()} blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandRoleName(ICharacter actor, ChargenRoleType roleType, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What new name would you like to give the {roleType.DescribeEnum().ColourName()} role selection screen?");
			return false;
		}

		TypeNames[roleType] = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send(
			$"The {roleType.DescribeEnum().ColourName()} role selection screen is now entitled {command.SafeRemainingArgument.TitleCase().ColourName()}.");
		return true;
	}

	private bool BuildingCommandRoleSkip(ICharacter actor, ChargenRoleType roleType)
	{
		CanSelectNone[roleType] = !CanSelectNone[roleType];
		Changed = true;
		actor.OutputHandler.Send(
			$"It is {(CanSelectNone[roleType] ? "now" : "no longer")} possible to select no options and skip the {roleType.DescribeEnum().ColourName()} role selection screen.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{IntroductionBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, IntroductionBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the blurb for this chargen screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		IntroductionBlurb = text;
		Changed = true;
		handler.Send($"You set the blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion
}