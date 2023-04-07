using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using Org.BouncyCastle.Asn1.X509;

namespace MudSharp.Form.Characteristics;

public class BodypartSpecificCharacteristicDefinition : CharacteristicDefinition,
	IBodypartSpecificCharacteristicDefinition
{
	public BodypartSpecificCharacteristicDefinition(string name, string pattern, string description,
		CharacteristicType type, IBodypartShape shape, IFuturemud gameworld)
		: base(name, pattern, description, type, gameworld,
			definition: new XElement("Definition", new XElement("TargetShape", shape.Id),
				new XElement("OrdinaryCount", "1")))
	{
	}

	public BodypartSpecificCharacteristicDefinition(MudSharp.Models.CharacteristicDefinition definition,
		IFuturemud gameworld) : base(definition, gameworld)
	{
	}

	protected override void LoadFromDatabase(Models.CharacteristicDefinition definition)
	{
		base.LoadFromDatabase(definition);
		if (string.IsNullOrEmpty(definition.Definition))
		{
			throw new ApplicationException(
				"BodypartSpecificCharacteristicDefinitions must have a definition that defines their target shape and ordinary count.");
		}

		var root = XElement.Parse(definition.Definition);
		TargetShape = Gameworld.BodypartShapes.Get(long.Parse(root.Element("TargetShape").Value));
		OrdinaryCount = int.Parse(root.Element("OrdinaryCount").Value);
	}

	public IBodypartShape TargetShape { get; set; }
	public int OrdinaryCount { get; set; }

	public override void Save()
	{
		var dbitem = FMDB.Context.CharacteristicDefinitions.Find(Id);
		dbitem.Definition = new XElement("Definition",
			new XElement("TargetShape", TargetShape.Id),
			new XElement("OrdinaryCount", OrdinaryCount)
		).ToString();
		base.Save();
	}

	public override string HelpText => $@"{base.HelpText}
	shape <shape> - sets the bodypart shape to tie to
	count <#> - sets the expected number of bodyparts for normal circumstances";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "shape":
			case "part":
			case "bodypart":
				BuildingCommandBodypart(actor, command);
				return;
			case "count":
				BuildingCommandCount(actor, command);
				return;
		}

		base.BuildingCommand(actor, command.GetUndo());
	}

	private void BuildingCommandCount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the ordinary count of the bodypart that this characteristic is tied to?");
			return;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a valid number that is 0 or more.");
			return;
		}

		OrdinaryCount = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The ordinary count of the target bodypart is now {OrdinaryCount.ToString("N0", actor).ColourValue()}.");
	}

	private void BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What bodypart shape should this characteristic be tied to?");
			return;
		}

		var shape = Gameworld.BodypartShapes.GetByIdOrName(command.SafeRemainingArgument);
		if (shape == null)
		{
			actor.OutputHandler.Send("There is no such bodypart shape.");
			return;
		}

		TargetShape = shape;
		Changed = true;
		actor.OutputHandler.Send(
			$"This characteristic is now tied to bodyparts with the {shape.Name.ColourValue()} shape.");
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Target Shape: {TargetShape.Name.ColourValue()}");
		sb.AppendLine($"Normal Count: {OrdinaryCount.ToString("N0", actor).ColourValue()}");
		return sb.ToString();
	}
}