using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class StartingLocationPickerScreenStoryboard : ChargenScreenStoryboard
{
	private StartingLocationPickerScreenStoryboard()
	{
	}

	public StartingLocationPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem) : base(
		dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		var element = definition.Element("Blurb");
		if (element != null)
		{
			Blurb = element.Value;
		}

		element = definition.Element("Locations");
		if (element != null)
		{
			Locations = new List<StartingLocation>();
			foreach (var location in element.Elements("Location"))
			{
				Locations.Add(new StartingLocation
				{
					Name = location.Element("Name")?.Value,
					Blurb = location.Element("Blurb")?.Value,
					Location = Gameworld.Cells.Get(long.Parse(location.Element("Location")?.Value ?? "0")),
					Role = Gameworld.Roles.Get(long.Parse(location.Element("Role")?.Value ?? "0")),
					OnCommenceProg =
						Gameworld.FutureProgs.Get(long.Parse(location.Element("OnCommenceProg")?.Value ?? "0"))
				});
			}
		}

		SkipScreenIfOnlyOneChoice = bool.Parse(definition.Element("SkipScreenIfOnlyOneChoice")?.Value ?? "true");
	}

	protected override string StoryboardName => "StartingLocationPicker";

	public string Blurb { get; protected set; }
	public List<StartingLocation> Locations { get; }
	public bool SkipScreenIfOnlyOneChoice { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectStartingLocation;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("SkipScreenIfOnlyOneChoice", SkipScreenIfOnlyOneChoice),
			new XElement("Locations",
				from location in Locations
				select new XElement("Location",
					new XElement("Name", location.Name),
					new XElement("Blurb", new XCData(location.Blurb)),
					new XElement("Location", location.Location?.Id ?? 0),
					new XElement("Role", location.Role?.Id ?? 0),
					new XElement("OnCommenceProg", location.OnCommenceProg?.Id ?? 0)
				)
			)
		).ToString();
	}

	#endregion

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new StartingLocationPickerScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"On this screen, users will select their character's starting location, which is the room that they will be dropped into upon creating their character. These starting locations tie to a particular kind of role, called a starting location role, which you must also create."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Locations".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		var i = 1;
		foreach (var location in Locations)
		{
			sb.AppendLine();
			sb.AppendLine($"#{i++.ToString("N0", voyeur)}) {location.Name.Colour(Telnet.Magenta)}");
			sb.AppendLine();
			sb.AppendLine($"Location: {location.Location?.GetFriendlyReference(voyeur) ?? "None".ColourError()}");
			sb.AppendLine($"Role: {location.Role?.Name.ColourName() ?? "None".ColourError()}");
			sb.AppendLine(
				$"Filter Prog: {location.Role?.AvailabilityProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
			sb.AppendLine(
				$"Commence Prog: {location.OnCommenceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
			sb.AppendLine();
			sb.AppendLine(location.Blurb.SubstituteANSIColour().Wrap(voyeur.InnerLineFormatLength));
		}

		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectStartingLocation,
			new ChargenScreenStoryboardFactory("StartingLocationPicker",
				(game, dbitem) => new StartingLocationPickerScreenStoryboard(game, dbitem)),
			"StartingLocationPicker",
			"Select a starting location upon creation",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	internal class StartingLocationPickerScreen : ChargenScreen
	{
		protected StartingLocation SelectedLocation;
		protected IEnumerable<StartingLocation> StartingLocations;
		protected StartingLocationPickerScreenStoryboard Storyboard;

		internal StartingLocationPickerScreen(IChargen chargen, StartingLocationPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			StartingLocations = GetValidStartingLocations();
			if (Storyboard.SkipScreenIfOnlyOneChoice && StartingLocations.Count() == 1)
			{
				SelectedLocation = StartingLocations.Single();
				Chargen.StartingLocation = SelectedLocation;
				Chargen.SelectedRoles.RemoveAll(x => x.RoleType == ChargenRoleType.StartingLocation);
				Chargen.SelectedRoles.Add(SelectedLocation.Role);
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectStartingLocation;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (SelectedLocation == null)
			{
				var index = 1;
				return
					$"{"Starting Location Selection".Colour(Telnet.Cyan)}\n\n{Storyboard.Blurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength)}\n\n{StartingLocations.Select(x => $"{index++}: {x.Name}").ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}\n\nEnter the name or number of the starting location that you would like to select.";
			}

			return
				$"Starting Location: {SelectedLocation.Name}\n\n{SelectedLocation.Blurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength)}\n\nDo you want to select this starting location? Type {"yes".Colour(Telnet.Yellow)} or {"no".Colour(Telnet.Yellow)}.";
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

			if (SelectedLocation != null)
			{
				if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					Chargen.StartingLocation = SelectedLocation;
					Chargen.SelectedRoles.RemoveAll(x => x.RoleType == ChargenRoleType.StartingLocation);
					Chargen.SelectedRoles.Add(SelectedLocation.Role);
					State = ChargenScreenState.Complete;
					return "You select " + SelectedLocation.Name.Colour(Telnet.Green) +
					       " as your starting location.\n";
				}

				SelectedLocation = null;
				return Display();
			}

			SelectedLocation = int.TryParse(command, out var value)
				? StartingLocations.ElementAtOrDefault(value - 1)
				: StartingLocations.FirstOrDefault(
					x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

			return SelectedLocation == null
				? "That is not a valid starting location. Please enter the name or number of the starting location that you want to select."
				: Display();
		}

		private IEnumerable<StartingLocation> GetValidStartingLocations()
		{
			return
				Storyboard.Locations.OrderBy(x => x.Name)
				          .Where(x => x.FutureProg?.ExecuteBool(Chargen) ?? true)
				          .ToList();
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3skipone#0 - toggles skipping the screen if there is only one valid choice
	#3location add <name> <role> <prog>#0 - drops you into an editor to enter the blurb for a new location
	#3location rename <old name> <new name>#0 - renames a starting location options
	#3location role <name> <role>#0 - sets the role associated with a location
	#3location commence <name> <prog>#0 - sets the on-commencement prog for a location
	#3location room <name> <room>#0 - sets the room associated with a location
	#3location blurb <name>#0 - drops you into an editor to change the blurb for a location
	#3location remove <name>#0 - removes a location";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "skipone":
				return BuildingCommandSkipOne(actor);
			case "location":
				return BuildingCommandLocation(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandLocation(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandLocationAdd(actor, command);
			case "rename":
			case "name":
				return BuildingCommandLocationRename(actor, command);
			case "role":
				return BuildingCommandLocationRole(actor, command);
			case "commence":
			case "oncommence":
			case "oncommenceprog":
			case "commenceprog":
			case "prog":
				return BuildingCommandLocationOnCommenceProg(actor, command);
			case "blurb":
				return BuildingCommandLocationBlurb(actor, command);
			case "remove":
			case "rem":
			case "del":
			case "delete":
				return BuildingCommandLocationRemove(actor, command);
			case "room":
			case "cell":
				return BuildingCommandLocationRoom(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandLocationRoom(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which starting location do you want to change the room for?");
			return false;
		}

		var which = Locations.FirstOrDefault(x => x.Name.EqualTo(command.PopSpeech())) ??
		            Locations.FirstOrDefault(x =>
			            x.Name.StartsWith(command.PopSpeech(), StringComparison.InvariantCultureIgnoreCase));
		if (which is null)
		{
			actor.OutputHandler.Send("There is no such starting location.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which room should characters choosing this option be placed into on creation?");
			return false;
		}

		var room = Gameworld.Cells.GetByIdOrName(command.SafeRemainingArgument);
		if (room is null)
		{
			actor.OutputHandler.Send("There is no such room.");
			return false;
		}

		which.Location = room;
		Changed = true;
		actor.OutputHandler.Send(
			$"The starting location called {which.Name.ColourName()} will now place characters into the room {room.GetFriendlyReference(actor)}.");
		return true;
	}

	private bool BuildingCommandLocationRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which starting location do you want to remove?");
			return false;
		}

		var which = Locations.FirstOrDefault(x => x.Name.EqualTo(command.SafeRemainingArgument)) ??
		            Locations.FirstOrDefault(x =>
			            x.Name.StartsWith(command.SafeRemainingArgument, StringComparison.InvariantCultureIgnoreCase));
		if (which is null)
		{
			actor.OutputHandler.Send("There is no such starting location for you to remove.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to remove the starting location option called {which.Name.ColourName()}? This is permanent and cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "deleting a starting location option",
			AcceptAction = text =>
			{
				Locations.Remove(which);
				Changed = true;
				actor.OutputHandler.Send($"You delete the starting location option called {which.Name.ColourName()}.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to remove the starting location option.");
			},
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to remove the starting location option."); }
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandLocationBlurb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which starting location do you want to edit the blurb for?");
			return false;
		}

		var which = Locations.FirstOrDefault(x => x.Name.EqualTo(command.SafeRemainingArgument)) ??
		            Locations.FirstOrDefault(x =>
			            x.Name.StartsWith(command.SafeRemainingArgument, StringComparison.InvariantCultureIgnoreCase));
		if (which is null)
		{
			actor.OutputHandler.Send("There is no such starting location.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{which.Blurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostLocationBlurb, CancelLocationBlurb, 1.0, which.Blurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, which });
		return true;
	}

	private void CancelLocationBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the blurb for this location.");
	}

	private void PostLocationBlurb(string text, IOutputHandler handler, object[] args)
	{
		var which = (StartingLocation)args[1];
		which.Blurb = text;
		Changed = true;
		handler.Send(
			$"You set the location blurb for starting location {which.Name.ColourName()} to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}


	private bool BuildingCommandLocationAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new starging location?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (Locations.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a starting location with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What character role should be associated with this starting location? See the {"role".MXPSend("role help")} command for more info.");
			return false;
		}

		var role = Gameworld.Roles.GetByIdOrName(command.PopSpeech());
		if (role is null)
		{
			actor.OutputHandler.Send("There is no such character role.");
			return false;
		}

		if (role.RoleType != ChargenRoleType.StartingLocation)
		{
			actor.OutputHandler.Send(
				$"The character role {role.Name.ColourName()} (#{role.Id.ToString("N0", actor)}) is not a starting location role.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which room should characters be put into when they commence with this starting location?");
			return false;
		}

		var cell = Gameworld.Cells.GetByIdOrName(command.PopSpeech());
		if (cell is null)
		{
			actor.OutputHandler.Send("There is no such room.");
			return false;
		}

		IFutureProg prog = null;
		if (!command.IsFinished)
		{
			prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
				ProgVariableTypes.Void, new List<ProgVariableTypes>
				{
					ProgVariableTypes.Character
				}).LookupProg();
			if (prog is null)
			{
				return false;
			}
		}

		actor.OutputHandler.Send($"Enter the blurb for your starting location below:\n");
		actor.EditorMode(PostAddLocationBlurb, CancelAddLocationBlurb, 1.0, null, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, name, role, prog, cell, actor });
		return true;
	}

	private void CancelAddLocationBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to add a new starting location.");
	}

	private void PostAddLocationBlurb(string text, IOutputHandler handler, object[] args)
	{
		var name = (string)args[1];
		var role = (IChargenRole)args[2];
		var prog = (IFutureProg)args[3];
		var cell = (ICell)args[4];
		var actor = (ICharacter)args[5];
		var location = new StartingLocation
		{
			Blurb = text,
			Role = role,
			Name = name,
			OnCommenceProg = prog,
			Location = cell
		};
		Locations.Add(location);
		Changed = true;
		handler.Send($@"You create a new starting location with the following properties: 

	Name: {name.ColourName()} 
	Role: {role.Name.ColourName()} 
	Location: {cell.GetFriendlyReference(actor)}
	Commence Prog: {prog?.MXPClickableFunctionName() ?? "None".ColourError()}
	Blurb:

{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandLocationOnCommenceProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which starting location do you want to change the room for?");
			return false;
		}

		var text = command.PopSpeech();
		var which = Locations.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		            Locations.FirstOrDefault(x =>
			            x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (which is null)
		{
			actor.OutputHandler.Send("There is no such starting location.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be executed on a character upon commencement?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Void, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		which.OnCommenceProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The starting location {which.Name.ColourName()} will now have the on-commencement prog {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandLocationRole(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which starting location do you want to change the room for?");
			return false;
		}

		var text = command.PopSpeech();
		var which = Locations.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		            Locations.FirstOrDefault(x =>
			            x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (which is null)
		{
			actor.OutputHandler.Send("There is no such starting location.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What character role should be associated with this starting location? See the {"role".MXPSend("role help")} command for more info.");
			return false;
		}

		var role = Gameworld.Roles.GetByIdOrName(command.PopSpeech());
		if (role is null)
		{
			actor.OutputHandler.Send("There is no such character role.");
			return false;
		}

		if (role.RoleType != ChargenRoleType.StartingLocation)
		{
			actor.OutputHandler.Send(
				$"The character role {role.Name.ColourName()} (#{role.Id.ToString("N0", actor)}) is not a starting location role.");
			return false;
		}

		which.Role = role;
		Changed = true;
		actor.OutputHandler.Send(
			$"The starting location called {which.Name.ColourName()} will now be associated with the {role.Name.ColourName()} character role.");
		return true;
	}

	private bool BuildingCommandLocationRename(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which starting location do you want to rename?");
			return false;
		}

		var text = command.PopSpeech();
		var which = Locations.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		            Locations.FirstOrDefault(x =>
			            x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (which is null)
		{
			actor.OutputHandler.Send("There is no such starting location.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this starting location?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Locations.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a starting location called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename the starting location formerly called {which.Name.ColourName()} to {name.ColourName()}.");
		which.Name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandSkipOne(ICharacter actor)
	{
		SkipScreenIfOnlyOneChoice = !SkipScreenIfOnlyOneChoice;
		Changed = true;
		actor.OutputHandler.Send(
			$"This screen will {(SkipScreenIfOnlyOneChoice ? "now" : "no longer")} be skipped if only one valid selection is available.");
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