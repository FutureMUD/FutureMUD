using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using Org.BouncyCastle.Math.EC.Multiplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class BoneHealthMerit : CharacterMeritBase, IBodypartHealthMerit
{
	public double Modifier { get; protected set; }

	protected BoneHealthMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		Modifier = double.Parse(root.Element("Modifier")?.Value ??
		                        throw new ApplicationException($"BoneHealthMerit {Id} was missing a Modifier element"));
	}

	protected BoneHealthMerit()
	{
	}

	protected BoneHealthMerit(IFuturemud gameworld, string name) : base(gameworld, name, "BoneHealth", "@ have|has healthy bones")
	{
		Modifier = 1.0;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Modifier", Modifier));
		return root;
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "multiplier":
			case "mult":
			case "modifier":
			case "mod":
				return BuildingCommandMultiplier(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier should be applied to bone strength?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		Modifier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now multiply bone health by {value.ToString("P2", actor).ColourValue()} when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3multiplier <%>#0 - sets the percentage multiplier for bone health";

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bone Health Multiplier: {Modifier.ToString("P2", actor).ColourValue()}");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("BoneHealth",
			(merit, gameworld) => new BoneHealthMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("BoneHealth", (gameworld, name) => new BoneHealthMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("BoneHealth", "Has a modifier for bone HP before breaks", new BoneHealthMerit().HelpText);
	}

	public bool AppliesToBodypart(IBodypart bodypart)
	{
		return bodypart is IBone;
	}

	public double MultiplierForBodypart(IBodypart bodypart)
	{
		return Modifier;
	}
}