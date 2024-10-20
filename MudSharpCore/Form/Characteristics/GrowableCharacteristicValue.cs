using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Characteristics;

public class GrowableCharacteristicValue : CharacteristicValue, IGrowableCharacteristicValue
{
	public GrowableCharacteristicValue(string name, ICharacteristicDefinition definition, string basic, string fancy,
		int growthStage) : base(name, definition, basic, $"{growthStage} 0 0 {fancy}")
	{
		Basic = basic;
		Fancy = fancy;
		GrowthStage = growthStage;
		Changed = true;
	}

	public GrowableCharacteristicValue(MudSharp.Models.CharacteristicValue value, IFuturemud gameworld) : base(value,
		gameworld)
	{
		Basic = value.Value;
		var split = value.AdditionalValue.Split(new char[] { ' ' }, 4);
		GrowthStage = int.Parse(split[0]);
		StyleDifficulty = (Difficulty)int.Parse(split[1]);
		StyleToolTag = Gameworld.Tags.Get(long.Parse(split[2]));
		Fancy = split[3];
	}

	protected GrowableCharacteristicValue(GrowableCharacteristicValue rhs, string newName) : base(rhs, newName,
		rhs.Basic, $"{rhs.GrowthStage} {(int)rhs.StyleDifficulty} {rhs.StyleToolTag?.Id ?? 0} {rhs.Fancy}")
	{
		GrowthStage = rhs.GrowthStage;
		StyleDifficulty = rhs.StyleDifficulty;
		StyleToolTag = rhs.StyleToolTag;
		Fancy = rhs.Fancy;
		Basic = rhs.Basic;
	}

	public override ICharacteristicValue Clone(string newName)
	{
		return new GrowableCharacteristicValue(this, newName);
	}

	public string Basic { get; protected set; }

	public string Fancy { get; protected set; }

	public override string GetBasicValue => Basic;

	public override string GetFancyValue => Fancy;

	public int GrowthStage { get; protected set; }

	public Difficulty StyleDifficulty { get; protected set; }

	public ITag StyleToolTag { get; protected set; }

	public override void Save()
	{
		using (new FMDB())
		{
			var dbvalue = FMDB.Context.CharacteristicValues.Find(Id);
			dbvalue.Default = Definition.IsDefaultValue(this);
			dbvalue.Name = Name;
			dbvalue.Value = Basic;
			dbvalue.Pluralisation = (int)Pluralisation;
			dbvalue.AdditionalValue = $"{GrowthStage} {(int)StyleDifficulty} {StyleToolTag?.Id ?? 0} {Fancy}";
			dbvalue.FutureProgId = ChargenApplicabilityProg?.Id;
			dbvalue.OngoingValidityProgId = OngoingValidityProg?.Id;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	protected override string HelpText => $@"{base.HelpText}
	#3basic <basic form>#0 - the basic form of this variable
	#3fancy <fancy form>#0 - the fancy form of this variable
	#3tag <which>#0 - the tag required for a tool to apply this style in the STYLE command
	#3tag none#0 - only the default styling tool will be required
	#3difficulty <difficulty>#0 - the difficulty that controls who can apply the style
	#3growth <#>#0 - a numerical value representing the growth stage

Note: When using the STYLE command people can apply any style with an equal or lesser growth number essentially at will, whereas there is a timer after each style before they can apply a longer style and it must always be a style 1 larger than the current length.";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "basic":
				var basic = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(basic))
				{
					actor.OutputHandler.Send("You must supply a value for the basic form.");
					return;
				}

				Basic = basic;
				Changed = true;
				actor.OutputHandler.Send(
					$"You change the basic value of this characteristic value to {basic.ColourValue()}.");
				return;
			case "fancy":
				var fancy = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(fancy))
				{
					actor.OutputHandler.Send("You must supply a value for the fancy form.");
					return;
				}

				Fancy = fancy;
				Changed = true;
				actor.OutputHandler.Send(
					$"You change the fancy value of this characteristic value to {fancy.ColourValue()}.");
				return;
			case "tag":
				var id = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(id))
				{
					actor.OutputHandler.Send(
						"Which tag do you want to assign to this characteristic as a required tag for styling tools?");
					return;
				}

				if (id.EqualTo("none"))
				{
					StyleToolTag = null;
					Changed = true;
					actor.OutputHandler.Send("This characteristic will no longer require a tool tag.");
					return;
				}

				var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
				if (matchedtags.Count == 0)
				{
					actor.OutputHandler.Send("There is no such tag.");
					return;
				}

				if (matchedtags.Count > 1)
				{
					actor.OutputHandler.Send(
						$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
					return;
				}

				StyleToolTag = matchedtags.Single();
				Changed = true;
				actor.OutputHandler.Send(
					$"This style now requires the {StyleToolTag.FullName.Colour(Telnet.Cyan)} tag to be styled.");
				return;
			case "growthstage":
			case "growth stage":
			case "stage":
				var stage = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(stage))
				{
					actor.OutputHandler.Send("What growth stage do you want to assign to this characteristic?");
					return;
				}

				if (!int.TryParse(stage, out var value))
				{
					actor.OutputHandler.Send("You must supply a number for the growth stage.");
					return;
				}

				GrowthStage = value;
				Changed = true;
				actor.OutputHandler.Send(
					$"You change the growth stage of this characteristic to {GrowthStage.ToString("N0", actor).ColourValue()}.");

				return;
			case "difficulty":
				var difftext = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(difftext))
				{
					actor.OutputHandler.Send("What difficulty do you want to set for this style?");
					return;
				}

				if (!difftext.TryParseEnum(out Difficulty difficulty))
				{
					actor.OutputHandler.Send("That is not a valid difficulty.");
					return;
				}

				StyleDifficulty = difficulty;
				Changed = true;
				actor.OutputHandler.Send($"It is now {StyleDifficulty.Describe().ColourValue()} to set this style.");
				return;
			default:
				base.BuildingCommand(actor, command.GetUndo());
				break;
		}
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Tool Tag: {StyleToolTag?.FullName.ColourName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Style Difficulty: {StyleDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Growth Stage: {GrowthStage.ToString("N0", actor).ColourValue()}");
		return sb.ToString();
	}

	public override string ToString()
	{
		return
			$"GrowableCharacteristicValue ID {Id} Name: {Name} Basic: {Basic} Stage: {GrowthStage} Diff: {StyleDifficulty} Tag: {StyleToolTag?.Name ?? "None"} Fancy: {Fancy}";
	}
}