using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public abstract class BoneViaBodypartProcedure : SurgicalProcedure
{
	protected BoneViaBodypartProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(
		procedure, gameworld)
	{
	}

	protected BoneViaBodypartProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body,
		string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
	{
	}

	private readonly List<IBodypart> _targetedParts = new();
	private bool _targetPartsForbidden;

	public IBone FixedBoneTarget { get; set; }

	public IBodypart FixedBodypartTarget { get; set; }

	protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (FixedBoneTarget != null && FixedBodypartTarget != null)
		{
			return new object[] { FixedBodypartTarget, FixedBoneTarget };
		}

		switch (additionalArguments.Length)
		{
			case 1:
				var targetText = additionalArguments[0].ToString();
				var bone = patient.Body.Bones.FirstOrDefault(x => x.FullDescription().EqualTo(targetText)) ??
				           patient.Body.Bones.FirstOrDefault(x => x.Name.EqualTo(targetText));
				if (bone != null)
				{
					return new object[]
					{
						patient.Body.Bodyparts.Where(x => x.Bones.Contains(bone)).FirstMax(x =>
							x.Alignment.FrontRearOnly() == Alignment.Front ? 10 : 1 * x.RelativeHitChance),
						bone
					};
				}

				return new object[] { default(IBodypart), default(IBone) };
			case 2:
				return new object[]
				{
					patient.Body.GetTargetBodypart(additionalArguments[0].ToString()),
					patient.Body.GetTargetBodypart(additionalArguments[0].ToString())?.Bones.FirstOrDefault(x =>
						x.FullDescription().EqualTo(additionalArguments[1].ToString()) ||
						x.ShortDescription(colour: false).EqualTo(additionalArguments[1].ToString()) ||
						x.Name.EqualTo(additionalArguments[1].ToString()))
				};
			default:
				return new object[] { default(IBodypart), default(IBone) };
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

		if (args[1] is not IBone bone)
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart, bone))
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
			return $"You must specify a bodypart to access your patient's bones, and a bone located in that bodypart.";
		}

		if (args[0] is not IBodypart bodypart)
		{
			return $"{patient.HowSeen(surgeon, true)} does not have any such bodypart.";
		}

		if (args[1] is not IBone bone)
		{
			return
				$"{bodypart.FullDescription(true).Pluralise()} are not generally known to contain any bone like that.";
		}

		if (!IsPermissableBodypart(bodypart, bone))
		{
			return
				$"This procedure is not designed to work with access to the {bone.FullDescription().Pluralise()} via the {bodypart.FullDescription().Pluralise()}.";
		}

		return base.WhyCannotPerformProcedure(surgeon, patient, additionalArguments);
	}

	public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		var bodypart = (IBodypart)additionalArguments[0];
		var organ = (IBone)additionalArguments[1];
		if (bodypart == organ)
		{
			return $"{ProcedureGerund} $1's {organ.FullDescription()}";
		}
		return $"{ProcedureGerund} $1's {organ.FullDescription()} via &1's {bodypart.FullDescription()}";
	}

	public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return string.Format(
			emote,
			((IBodypart)additionalArguments[0]).FullDescription().ToLowerInvariant(),
			((IBone)additionalArguments[1]).FullDescription().ToLowerInvariant()
		);
	}
	public override int DressPhaseEmoteExtraArgumentCount => 2;

	public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the bodypart through which the bone is being accessed
	#3{1}#0 - the bone being operated on
".SubstituteANSIColour();

	protected override (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string)
		GetSpecialPhaseAction(string actionText)
	{
		if (actionText.EqualTo("checkbone"))
		{
			return CheckBonePhaseSpecialAction();
		}

		return base.GetSpecialPhaseAction(actionText);
	}

	private static (Func<ICharacter, ICharacter, object[], bool>, Func<ICharacter, ICharacter, object[], string>, string) CheckBonePhaseSpecialAction()
	{
		return ((surgeon, patient, parameters) =>
				{
					var bone = (IBone)parameters[1];
					if (!patient.Body.Bones.Contains(bone))
					{
						return false;
					}

					return true;
				}
				,
				(surgeon, patient, parameters) =>
				{
					var bp = (IBodypart)parameters[0];
					var bone = (IBone)parameters[1];
					if (bp == bone)
					{
						return
							$"$1 $1|do|does not have {bone.FullDescription().A_An()}";
					}
					return
						$"$1 $1|do|does not have {bone.FullDescription().A_An()} in &1's {bp.FullDescription()}";
				},
				"Checks for Bones"
			);
	}

	protected bool IsPermissableBodypart(IBodypart bodypart, IBone bone)
	{
		if (FixedBodypartTarget != null && FixedBodypartTarget == bodypart && FixedBoneTarget != null &&
		    FixedBoneTarget == bone)
		{
			return true;
		}

		if (_targetPartsForbidden)
		{
			return !_targetedParts.Contains(bone);
		}

		return _targetedParts.Contains(bone);
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Parts",
				new XAttribute("forbidden", _targetPartsForbidden),
				from part in _targetedParts
				select new XElement("Part", part.Id)
			),
			new XElement("FixedBone", FixedBoneTarget?.Id ?? 0),
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

			var element = partsElement.Element("FixedBone");
			if (element != null)
			{
				if (Gameworld.BodypartPrototypes.Get(long.Parse(element.Value)) is IBone gPart)
				{
					FixedBoneTarget = gPart;
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

	#region Overrides of SurgicalProcedure

	/// <inheritdoc />
	protected override bool BuildingCommandPhaseSpecial(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		if (command.PeekSpeech().EqualTo("checkbone"))
		{
			var (truth, error, desc) = CheckBonePhaseSpecialAction();
			phase.PhaseSpecialEffects = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n")
			                                                             .FluentAppend($"checkbone", true);
			phase.PhaseSuccessful += truth;
			phase.WhyPhaseNotSuccessful += error;
			phase.PhaseSpecialEffectsDescription = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n").FluentAppend(desc, true);
			Changed = true;
			actor.OutputHandler.Send($"This phase will now check for the presence of the bone, and stop if not found.");
			return true;
		}
		return base.BuildingCommandPhaseSpecial(actor, command, phase);
	}

	protected override string SpecialActionText => $@"{base.SpecialActionText}
#Ccheckbone#0 - checks for the presence of the bone and stops if not found";

	protected override string AdditionalHelpText => @"	#3forbidden#0 - toggles whether the parts list is opt-in or opt-out
	#3part <which>#0 - toggles a part being a part of the list
	#3fixedpart <part>#0 - sets the procedure as being only accessed via this bodypart
	#3fixedpart none#0 - the procedure can be accessed via any bodypart
	#3fixedbone <part>#0 - sets the procedure as targeting only the organ specified
	#3fixedbone none#0 - the procedure can target any organ";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "fixedpart":
				return BuildingCommandFixedPart(actor, command);
			case "fixedbone":
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
			actor.OutputHandler.Send($"You must either specify a bone that this procedure targets, or use {"none".ColourCommand()} to have it be able to target any bone.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			FixedBoneTarget = null;
			Changed = true;
			actor.OutputHandler.Send("This procedure no longer targets any particular bone and can instead be used on any of them.");
			return false;
		}

		var bone = TargetBodyType.Organs.GetBodypartByName(command.SafeRemainingArgument);
		if (bone is null)
		{
			actor.OutputHandler.Send($"The {TargetBodyType.Name.ColourValue()} body has no such bone.");
			return false;
		}

		FixedBoneTarget = (IBone)bone;
		Changed = true;
		actor.OutputHandler.Send(
			$"This surgical procedure now specifically targets the {bone.FullDescription().ColourValue()} bone.");
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
}