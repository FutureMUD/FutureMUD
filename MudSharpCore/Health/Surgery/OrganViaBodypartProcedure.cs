using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public abstract class OrganViaBodypartProcedure : SurgicalProcedure
{
	protected OrganViaBodypartProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(
		procedure, gameworld)
	{
	}

	protected OrganViaBodypartProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	private readonly List<IBodypart> _targetedParts = new();
	private bool _targetPartsForbidden;

	public IOrganProto FixedOrganTarget { get; set; }

	public IBodypart FixedBodypartTarget { get; set; }

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (FixedOrganTarget != null && FixedBodypartTarget != null)
		{
			return new object[] { FixedBodypartTarget, FixedOrganTarget };
		}

		switch (additionalArguments.Length)
		{
			case 1:
				var targetText = additionalArguments[0].ToString();
				var organ = patient.Body.Organs.FirstOrDefault(x => x.FullDescription().EqualTo(targetText)) ??
				            patient.Body.Organs.FirstOrDefault(x => x.Name.EqualTo(targetText));
				if (organ != null)
				{
					return new object[]
					{
						patient.Body.Bodyparts.Where(x => x.Organs.Contains(organ)).FirstMax(x =>
							x.Alignment.FrontRearOnly() == Alignment.Front ? 10 : 1 * x.RelativeHitChance),
						organ
					};
				}

				return new object[] { default(IBodypart), default(IOrganProto) };
			case 2:
				return new object[]
				{
					patient.Body.GetTargetBodypart(additionalArguments[0].ToString()),
					patient.Body.GetTargetBodypart(additionalArguments[0].ToString())?.Organs.FirstOrDefault(x =>
						x.FullDescription().EqualTo(additionalArguments[1].ToString()) ||
						x.ShortDescription(colour: false).EqualTo(additionalArguments[1].ToString()) ||
						x.Name.EqualTo(additionalArguments[1].ToString()))
				};
			default:
				return new object[] { default(IBodypart), default(IOrganProto) };
		}
	}

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] == null && args[1] == null)
		{
			return false;
		}

		if (args[0] is not IBodypart bodypart)
		{
			return false;
		}

		if (args[1] is not IOrganProto organ)
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart, organ))
		{
			return false;
		}

		return base.CanPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] == null && args[1] == null)
		{
			return
				$"You must specify a bodypart to access your patient's innards, and an organ located in that bodypart.";
		}

		if (args[0] is not IBodypart bodypart)
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		if (args[1] is not IOrganProto organ)
		{
			return
				$"{bodypart.FullDescription(true).Pluralise()} are not generally known to contain any organ like that.";
		}

		if (!IsPermissableBodypart(bodypart, organ))
		{
			return
				$"This procedure is not designed to work with access to the {organ.FullDescription().Pluralise()} via the {bodypart.FullDescription().Pluralise()}.";
		}

		return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var organ = (IOrganProto)additionalArguments[1];
		return $"{ProcedureGerund} $1's {organ.FullDescription()} via &1's {bodypart.FullDescription()}";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return string.Format(
			emote,
			((IBodypart)additionalArguments[0]).FullDescription().ToLowerInvariant(),
			((IOrganProto)additionalArguments[1]).FullDescription().ToLowerInvariant()
		);
	}

	public override int DressPhaseEmoteExtraArgumentCount => 2;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the bodypart being accessed
	#3{1}#0 - the description of the organ being operated on
".SubstituteANSIColour();

	protected override (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string)
		GetSpecialPhaseAction(string actionText)
	{
		if (actionText.EqualTo("checkorgan"))
		{
			return CheckOrganPhaseSpecialAction();
		}

		return base.GetSpecialPhaseAction(actionText);
	}

	private static (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string) CheckOrganPhaseSpecialAction()
	{
		return ((surgeon, patient, parameters) =>
				{
					var organ = (IOrganProto)parameters[1];
					if (!patient.Body.Organs.Contains(organ))
					{
						return false;
					}

					return true;
				}
				,
				(surgeon, patient, parameters) =>
				{
					var bp = (IBodypart)parameters[0];
					var organ = (IOrganProto)parameters[1];
					return
						$"$1 $1|do|does not have {organ.FullDescription().A_An()} in &1's {bp.FullDescription()}";
				},
				"Checks for organ"
			);
	}

	protected bool IsPermissableBodypart(IBodypart bodypart, IOrganProto organ)
	{
		if (FixedBodypartTarget != null && FixedBodypartTarget == bodypart && FixedOrganTarget != null &&
		    FixedOrganTarget == organ)
		{
			return true;
		}

		if (_targetPartsForbidden)
		{
			return !_targetedParts.Contains(organ);
		}

		return _targetedParts.Contains(organ);
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Parts",
				new XAttribute("forbidden", _targetPartsForbidden),
				from part in _targetedParts
				select new XElement("Part", part.Id)
			),
			new XElement("FixedOrgan", FixedOrganTarget?.Id ?? 0),
			new XElement("FixedPart", FixedBodypartTarget?.Id ?? 0)
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

			foreach (var part in partsElement.Elements("Part"))
			{
				var gPart = Gameworld.BodypartPrototypes.Get(long.Parse(part.Value));
				if (gPart != null)
				{
					_targetedParts.Add(gPart);
				}
			}

			var element = partsElement.Element("FixedOrgan");
			if (element != null)
			{
				if (Gameworld.BodypartPrototypes.Get(long.Parse(element.Value)) is IOrganProto gPart)
				{
					FixedOrganTarget = gPart;
				}
			}

			element = partsElement.Element("FixedPart");
			if (element != null)
			{
				var gPart = Gameworld.BodypartPrototypes.Get(long.Parse(element.Value));
				if (gPart != null)
				{
					FixedBodypartTarget = gPart;
				}
			}
		}
		else
		{
			_targetPartsForbidden = true;
		}
	}

	protected override bool BuildingCommandPhaseSpecial(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		if (command.PeekSpeech().EqualTo("checkorgan"))
		{
			var (truth, error, desc) = CheckOrganPhaseSpecialAction();
			phase.PhaseSpecialEffects = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n")
			                                                             .FluentAppend($"checkorgan", true);
			phase.PhaseSuccessful += truth;
			phase.WhyPhaseNotSuccessful += error;
			phase.PhaseSpecialEffectsDescription = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n").FluentAppend(desc, true);
			Changed = true;
			actor.OutputHandler.Send($"This phase will now check whether the bodypart has the organ already, and stop if true.");
			return true;
		}
		return base.BuildingCommandPhaseSpecial(actor, command, phase);
	}

	#region Overrides of SurgicalProcedure

	/// <inheritdoc />
	protected override string AdditionalHelpText => @"	#3forbidden#0 - toggles whether the parts list is opt-in or opt-out
	#3part <which>#0 - toggles a part being a part of the list
	#3fixedpart <part>#0 - sets the procedure as being only accessed via this bodypart
	#3fixedpart none#0 - the procedure can be accessed via any bodypart
	#3fixedorgan <part>#0 - sets the procedure as targeting only the organ specified
	#3fixedorgan none#0 - the procedure can target any organ";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "fixedpart":
				return BuildingCommandFixedPart(actor, command);
			case "fixedorgan":
				return BuildingCommandFixedOrgan(actor, command);
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

	private bool BuildingCommandFixedOrgan(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify an organ that this procedure targets, or use {"none".ColourCommand()} to have it be able to target any organ.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			FixedOrganTarget = null;
			Changed = true;
			actor.OutputHandler.Send("This procedure no longer targets any particular organ and can instead be used on any of them.");
			return false;
		}

		var organ = TargetBodyType.Organs.GetBodypartByName(command.SafeRemainingArgument);
		if (organ is null)
		{
			actor.OutputHandler.Send($"The {TargetBodyType.Name.ColourValue()} body has no such organ.");
			return false;
		}

		FixedOrganTarget = (IOrganProto)organ;
		Changed = true;
		actor.OutputHandler.Send(
			$"This surgical procedure now specifically targets the {organ.FullDescription().ColourValue()} organ.");
		return true;
	}

	private bool BuildingCommandFixedPart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a bodypart that this procedure targets, or use {"none".ColourCommand()} to have it be able to target any bodypart.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			FixedBodypartTarget = null;
			Changed = true;
			actor.OutputHandler.Send("This procedure no longer targets any particular bodypart and can instead be used on any of them.");
			return false;
		}

		var bodypart = TargetBodyType.AllBodyparts.GetBodypartByName(command.SafeRemainingArgument);
		if (bodypart is null)
		{
			actor.OutputHandler.Send($"The {TargetBodyType.Name.ColourValue()} body has no such bodypart.");
			return false;
		}

		FixedBodypartTarget = bodypart;
		Changed = true;
		actor.OutputHandler.Send($"This surgical procedure now specifically targets the {bodypart.FullDescription().ColourValue()} bodypart.");
		return true;
	}

	#endregion

	protected override string SpecialActionText => $@"{base.SpecialActionText}
	#Ccheckorgan#0 - checks to see whether the organ is present, and stops if not";
}