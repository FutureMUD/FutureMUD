using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Body;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public abstract class BodypartSpecificSurgicalProcedure : SurgicalProcedure
{
	protected BodypartSpecificSurgicalProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(
		procedure, gameworld)
	{
	}

	public BodypartSpecificSurgicalProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	private List<IBodypart> _targetedParts = new();
	private bool _targetPartsForbidden;

	protected bool IsPermissableBodypart(IBodypart bodypart)
	{
		if (_targetPartsForbidden)
		{
			return !_targetedParts.Contains(bodypart);
		}

		return _targetedParts.Contains(bodypart);
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Parts",
				new XAttribute("forbidden", _targetPartsForbidden),
				from part in _targetedParts
				select new XElement("Part", part.Id)
			)
		).ToString();
	}

	protected override void LoadFromDB(MudSharp.Models.SurgicalProcedure procedure)
	{
		base.LoadFromDB(procedure);
		if (!string.IsNullOrEmpty(procedure.Definition))
		{
			var root = XElement.Parse(procedure.Definition);
			var partsElement = root.Element("Parts");
			if (partsElement.Attribute("forbidden")?.Value.EqualTo("true") ?? false)
			{
				_targetPartsForbidden = true;
			}

			foreach (var part in partsElement.Elements())
			{
				var gPart = Gameworld.BodypartPrototypes.Get(long.Parse(part.Value));
				if (gPart != null)
				{
					_targetedParts.Add(gPart);
				}
			}
		}
		else
		{
			_targetPartsForbidden = true;
		}
	}

	protected override string AdditionalHelpText => @"	#3forbidden#0 - toggles whether the parts list is opt-in or opt-out
	#3part <which>#0 - toggles a part being a part of the list";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "target":
			case "part":
			case "parts":
			case "targetpart":
			case "targetparts":
				return BuildingCommandTargetPart(actor, command);
			case "forbidden":
				return BuildingCommandForbidden(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandForbidden(ICharacter actor, StringStack command)
	{
		_targetPartsForbidden = !_targetPartsForbidden;
		Changed = true;
		if (_targetPartsForbidden)
		{
			actor.OutputHandler.Send("The list of bodyparts for this surgery is now a list of parts which are forbidden to be targeted.");
		}
		else
		{
			actor.OutputHandler.Send("The list of bodyparts for this surgery is now an exclusive list of parts that can be targeted by this surgery.");
		}
		return true;
	}

	private bool BuildingCommandTargetPart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which bodypart do you want to {(_targetPartsForbidden ? "forbid" : "permit")}?");
			return false;
		}

		var part = TargetBodyType.AllBodyparts.GetBodypartByName(command.SafeRemainingArgument);
		if (part is null)
		{
			actor.OutputHandler.Send($"The {TargetBodyType.Name.ColourValue()} body has no such bodypart.");
			return false;
		}

		if (_targetedParts.Contains(part))
		{
			_targetedParts.Remove(part);
			if (_targetPartsForbidden)
			{
				actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is no longer forbidden from being targeted by this surgery.");
			}
			else
			{
				actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is no longer permitted to be targeted by this surgery.");
			}
		}
		else
		{
			_targetedParts.Add(part);
			if (_targetPartsForbidden)
			{
				actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is now forbidden from being targeted by this surgery.");
			}
			else
			{
				actor.OutputHandler.Send($"The {part.FullDescription().ColourValue()} part is now permitted to be targeted by this surgery.");
			}
		}

		Changed = true;
		return true;
	}
}