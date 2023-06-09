﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MagicArmourPower : SustainedMagicPower
{
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("armour", (power, gameworld) => new MagicArmourPower(power, gameworld));
		MagicPowerFactory.RegisterLoader("armor", (power, gameworld) => new MagicArmourPower(power, gameworld));
	}

	protected MagicArmourPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		BeginVerb = root.Element("BeginVerb")?.Value ??
		            throw new ApplicationException(
			            $"MagicArmourPower ID #{Id} ({Name}) did not have a BeginVerb element.");
		EndVerb = root.Element("EndVerb")?.Value ??
		          throw new ApplicationException(
			          $"MagicArmourPower ID #{Id} ({Name}) did not have an EndVerb element.");
		var element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"There was no EmoteText in the definition XML for power {Id} ({Name}).");
		}

		EmoteText = element.Value;

		element = root.Element("BodypartShapes");
		if (element != null)
		{
			foreach (var sub in element.Elements())
			{
				var shape = long.TryParse(sub.Value, out var idvalue)
					? Gameworld.BodypartShapes.Get(idvalue)
					: Gameworld.BodypartShapes.GetByName(sub.Value);
				if (shape != null)
				{
					_coveredShapes.Add(shape);
				}
			}
		}

		element = root.Element("FailEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no FailEmoteText in the definition XML for power {Id} ({Name}).");
		}

		FailEmoteText = element.Value;

		element = root.Element("EndPowerEmoteText");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EndPowerEmoteText in the definition XML for power {Id} ({Name}).");
		}

		EndPowerEmoteText = element.Value;

		element = root.Element("SkillCheckDifficulty");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckDifficulty in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckDifficulty = (Difficulty)int.Parse(element.Value);

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckTrait in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(element.Value));

		element = root.Element("MinimumSuccessThreshold");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no MinimumSuccessThreshold in the definition XML for power {Id} ({Name}).");
		}

		MinimumSuccessThreshold = (Outcome)int.Parse(element.Value);

		element = root.Element("ArmourAppliesProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ArmourAppliesProg in the definition XML for power {Id} ({Name}).");
		}

		ArmourAppliesProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		Quality = (ItemQuality)int.Parse(root.Element("Quality")?.Value ?? "0");
		ArmourType = (long.TryParse(root.Element("ArmourType")?.Value ?? "0", out value)
			             ? Gameworld.ArmourTypes.Get(value)
			             : Gameworld.ArmourTypes.GetByName(root.Element("ArmourType")?.Value ?? "")) ??
		             throw new ApplicationException($"Invalid armour type in MagicArmourPower #{Id} ({Name})")
			;
		ArmourMaterial = (long.TryParse(root.Element("ArmourMaterial")?.Value ?? "0", out value)
			                 ? Gameworld.Materials.Get(value) as ISolid
			                 : Gameworld.Materials.GetByName(root.Element("ArmourMaterial")?.Value ?? "") as ISolid
		                 ) ??
		                 throw new ApplicationException(
			                 $"Invalid armour material in MagicArmourPower #{Id} ({Name})");
		FullDescriptionAddendum = root.Element("FullDescriptionAddendum")?.Value ?? string.Empty;
		ArmourCanBeObscuredByInventory = bool.Parse(root.Element("CanBeObscuredByInventory")?.Value ?? "false");
	}

	#region Overrides of MagicPowerBase

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		if (verb.EqualTo(EndVerb))
		{
			if (!actor.AffectedBy<MagicArmour>(this))
			{
				actor.OutputHandler.Send("You are not currently using that power.");
				return;
			}

			actor.RemoveAllEffects<MagicArmour>(x => x.Power == this, true);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
			ConsumePowerCosts(actor, verb);
			return;
		}

		if (actor.AffectedBy<MagicArmour>(this))
		{
			actor.OutputHandler.Send("You are already using that power.");
			return;
		}

		if (CanInvokePowerProg?.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg?.Execute<string>(actor) ?? "You cannot use that power.");
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MagicArmourPower);
		var outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, null);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor)));
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor)));
		actor.AddEffect(new MagicArmour(actor, this), GetDuration(outcome.SuccessDegrees()));
		ConsumePowerCosts(actor, verb);
	}

	public string EmoteText { get; protected set; }
	public string FailEmoteText { get; protected set; }
	public string EndPowerEmoteText { get; protected set; }
	public Difficulty SkillCheckDifficulty { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }
	public IArmourType ArmourType { get; protected set; }
	public ItemQuality Quality { get; protected set; }
	public ISolid ArmourMaterial { get; protected set; }
	public string BeginVerb { get; protected set; }
	public string EndVerb { get; protected set; }

	public IFutureProg ArmourAppliesProg { get; protected set; }

	public ITraitExpression MaximumDamageAbsorbed { get; protected set; }

	public override IEnumerable<string> Verbs => new[] { BeginVerb, EndVerb };

	private readonly HashSet<IBodypartShape> _coveredShapes = new();

	public bool AppliesToBodypart(IBodypart bodypart)
	{
		if (!_coveredShapes.Any())
		{
			return true;
		}

		return _coveredShapes.Contains(bodypart.Shape);
	}

	public string FullDescriptionAddendum { get; protected set; }
	public bool ArmourCanBeObscuredByInventory { get; protected set; }

	#endregion

	#region Overrides of SustainedMagicPower

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		actor.RemoveAllEffects<MagicArmour>(x => x.Power == this, true);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
	}

	#endregion
}