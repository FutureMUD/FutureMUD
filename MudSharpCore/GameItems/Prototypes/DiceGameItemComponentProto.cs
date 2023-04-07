using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class DiceGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Dice";

	private readonly List<string> _faces = new();
	public IEnumerable<string> Faces => _faces;

	private Dictionary<int, double> _faceProbabilities = new();
	public IReadOnlyDictionary<int, double> FaceProbabilities => _faceProbabilities;

	#region Constructors

	protected DiceGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Dice")
	{
		_faces.AddRange(new[] { "1", "2", "3", "4", "5", "6" });
		_faceProbabilities.Add(0, 1.0);
		_faceProbabilities.Add(1, 1.0);
		_faceProbabilities.Add(2, 1.0);
		_faceProbabilities.Add(3, 1.0);
		_faceProbabilities.Add(4, 1.0);
		_faceProbabilities.Add(5, 1.0);
	}

	protected DiceGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		foreach (var element in root.Element("Faces").Elements())
		{
			_faces.Add(element.Value);
		}

		foreach (var element in root.Element("Weights").Elements())
		{
			_faceProbabilities[int.Parse(element.Element("Face").Value)] =
				double.Parse(element.Element("Probability").Value);
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Faces", from face in Faces select new XElement("Face", new XCData(face))),
			new XElement("Weights",
				from face in FaceProbabilities
				select new XElement("Weight", new XElement("Face", face.Key), new XElement("Probability", face.Value)))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DiceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DiceGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Dice".ToLowerInvariant(), true,
			(gameworld, account) => new DiceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Dice", (proto, gameworld) => new DiceGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Dice",
			$"Makes the item into a {"[dice]".Colour(Telnet.Yellow)} that can be rolled.",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new DiceGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttemplate d<x> - sets the dice to a dX\n\tface add <new> - adds a new face\n\tface weight <#/name> <%> - sets the fairness of the face\n\tface remove <#/name> - removes a face\n\tface swap <#/name> <#/name> - swaps the position of two faces in the list.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "face":
				return BuildingCommandFace(actor, command);
			case "template":
				return BuildingCommandTemplate(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private static Regex NumericalDiceRegex = new("d(?<sides>\\d+)");

	private bool BuildingCommandTemplate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which template do you want to set this dice to? You can use numbered dice in the form of d{"X".Colour(Telnet.BoldYellow)}.");
			return false;
		}

		var template = command.PopSpeech();
		if (NumericalDiceRegex.IsMatch(template))
		{
			var sides = int.Parse(NumericalDiceRegex.Match(template).Groups["sides"].Value);
			if (sides < 4)
			{
				actor.OutputHandler.Send("The minimum number of sides for a dice is 4.");
				return false;
			}

			if (sides > 100)
			{
				actor.OutputHandler.Send("The maximum number of sides for a dice is 100.");
				return false;
			}

			_faces.Clear();
			_faceProbabilities.Clear();
			for (var i = 1; i <= sides; i++)
			{
				_faces.Add(i.ToString("F0"));
				_faceProbabilities[i - 1] = 1.0;
			}

			Changed = true;
			actor.OutputHandler.Send($"You reset this dice to a d{sides} template.");
			return true;
		}

		actor.OutputHandler.Send("At this stage only dX style templates are supported.");
		return false;
	}

	private bool BuildingCommandFace(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandFaceAdd(actor, command);
			case "delete":
			case "remove":
			case "del":
			case "rem":
				return BuildingCommandFaceRemove(actor, command);
			case "swap":
			case "switch":
				return BuildingCommandSwap(actor, command);
		}

		int faceIndex;
		var faceText = command.Last;
		if (int.TryParse(faceText, out var value))
		{
			if (string.IsNullOrEmpty(Faces.ElementAtOrDefault(value - 1)))
			{
				actor.OutputHandler.Send("There is no face at that position.");
				return false;
			}

			faceIndex = value - 1;
		}
		else
		{
			var face = Faces.FirstOrDefault(x => x.EqualTo(command.Last)) ?? Faces.FirstOrDefault(x =>
				x.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (string.IsNullOrEmpty(face))
			{
				actor.OutputHandler.Send(
					"You can either use the add, remove or swap commands, or specify a face by its value or position to edit further.");
				return false;
			}

			faceIndex = _faces.IndexOf(face);
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "weight":
			case "probability":
			case "chance":
			case "prob":
				return BuildingCommandWeight(actor, command, faceIndex);
			case "text":
			case "face":
			case "value":
			case "set":
				return BuildingCommandText(actor, command, faceIndex);
			default:
				actor.OutputHandler.Send("You can either choose to change the value or the weight of that face.");
				return false;
		}
	}

	private bool BuildingCommandText(ICharacter actor, StringStack command, int faceIndex)
	{
		var face = _faces.ElementAt(faceIndex);
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What do you want to change the face value of {face.ColourValue()} to?");
			return false;
		}

		var faceValue = command.PopSpeech();
		actor.OutputHandler.Send($"You change the face value of {face.ColourValue()} to {faceValue.ColourValue()}.");
		_faces[faceIndex] = faceValue;
		Changed = true;
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command, int faceIndex)
	{
		var face = _faces.ElementAt(faceIndex);
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What probability of rolling {face.ColourValue()} do you want to set?");
			return false;
		}

		if (!NumberUtilities.TryParsePercentage(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		_faceProbabilities[faceIndex] = value;
		actor.OutputHandler.Send(
			$"The probability (relative to a fair dice) of rolling {face.ColourValue()} is now {value.ToString("P0", actor).ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the first face that you want to swap?");
			return false;
		}

		int index1;
		string face1;
		if (int.TryParse(command.PopSpeech(), out var value))
		{
			face1 = _faces.ElementAtOrDefault(value - 1);
			if (string.IsNullOrEmpty(face1))
			{
				actor.OutputHandler.Send("There is no face at the first position you specified.");
				return false;
			}

			index1 = value - 1;
		}
		else
		{
			face1 = _faces.FirstOrDefault(x => x.EqualTo(command.Last)) ?? _faces.FirstOrDefault(x =>
				x.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (string.IsNullOrEmpty(face1))
			{
				actor.OutputHandler.Send("There is no face with the first value you specified.");
				return false;
			}

			index1 = _faces.IndexOf(face1);
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the second face that you want to swap positions with the first?");
			return false;
		}

		int index2;
		string face2;
		if (int.TryParse(command.PopSpeech(), out value))
		{
			face2 = _faces.ElementAtOrDefault(value - 1);
			if (string.IsNullOrEmpty(face2))
			{
				actor.OutputHandler.Send("There is no face at the first position you specified.");
				return false;
			}

			index2 = value - 1;
		}
		else
		{
			face2 = _faces.FirstOrDefault(x => x.EqualTo(command.Last)) ?? _faces.FirstOrDefault(x =>
				x.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (string.IsNullOrEmpty(face2))
			{
				actor.OutputHandler.Send("There is no face with the first value you specified.");
				return false;
			}

			index2 = _faces.IndexOf(face2);
		}

		if (index1 == index2)
		{
			actor.OutputHandler.Send("You cannot swap a face with itself.");
			return false;
		}

		var prob1 = _faceProbabilities[index1];
		var prob2 = _faceProbabilities[index2];
		_faces.Swap(index1, index2);
		_faceProbabilities[index1] = prob2;
		_faceProbabilities[index2] = prob1;
		Changed = true;
		actor.OutputHandler.Send($"You swap the order of faces {face1.ColourValue()} and {face2.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFaceRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which face do you want to remove?");
			return false;
		}

		int index;
		string face;
		if (int.TryParse(command.PopSpeech(), out var value))
		{
			face = _faces.ElementAtOrDefault(value - 1);
			if (string.IsNullOrEmpty(face))
			{
				actor.OutputHandler.Send("There is no face at that position to remove.");
				return false;
			}

			index = value - 1;
		}
		else
		{
			face = _faces.FirstOrDefault(x => x.EqualTo(command.Last)) ?? _faces.FirstOrDefault(x =>
				x.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (string.IsNullOrEmpty(face))
			{
				actor.OutputHandler.Send("There is no face with the value you specified to remove.");
				return false;
			}

			index = _faces.IndexOf(face);
		}

		_faces.RemoveAt(index);
		_faceProbabilities.Remove(index);
		Changed = true;
		actor.OutputHandler.Send($"You remove the {face.ColourValue()} face from this die.");
		return true;
	}

	private bool BuildingCommandFaceAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What value do you want to give to this new face?");
			return false;
		}

		_faces.Add(command.PopSpeech());
		_faceProbabilities[_faces.Count - 1] = 1.0;
		actor.OutputHandler.Send($"You add a new face to this die with the value of {command.Last.ColourValue()}.");
		Changed = true;
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var faceWeights = new List<string>();
		for (var i = 0; i < _faces.Count; i++)
		{
			faceWeights.Add(FaceProbabilities[i].ToString("P0").ColourValue());
		}

		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is a die with the faces {4}. {5}",
			"Dice Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Faces.Select(x => x.ColourValue()).ListToString(),
			FaceProbabilities.All(x => x.Value == 1.0)
				? "It is a fair die with equal probabilities of any result."
				: $"It has the following weights for its faces: {faceWeights.ListToString()}"
		);
	}
}