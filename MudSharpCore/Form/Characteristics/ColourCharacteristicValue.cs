using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Form.Characteristics;

public class ColourCharacteristicValue : CharacteristicValue
{
	public ColourCharacteristicValue(string name, ICharacteristicDefinition type, Colour.IColour colour)
		: base(name, type, colour.Id.ToString(), string.Empty)
	{
		Colour = colour;
	}

	public ColourCharacteristicValue(MudSharp.Models.CharacteristicValue value, IFuturemud gameworld)
		: base(value, gameworld)
	{
		Colour = gameworld.Colours.Get(long.Parse(value.Value));
	}

	protected ColourCharacteristicValue(ColourCharacteristicValue rhs, string newName) : base(rhs, newName,
		rhs.Colour.Id.ToString(), string.Empty)
	{
		Colour = rhs.Colour;
	}

	public override ICharacteristicValue Clone(string newName)
	{
		return new ColourCharacteristicValue(this, newName);
	}

	public Colour.IColour Colour { get; protected set; }

	public override string GetValue => Colour.Name;

	public override string GetBasicValue => Colour.Basic.ToString().ToLowerInvariant();

	public override string GetFancyValue => Colour.Fancy;

	public override void Save()
	{
		using (new FMDB())
		{
			var dbvalue = FMDB.Context.CharacteristicValues.Find(Id);
			dbvalue.Default = Definition.IsDefaultValue(this);
			dbvalue.Name = Name;
			dbvalue.Value = Colour.Id.ToString();
			dbvalue.Pluralisation = (int)Pluralisation;
			dbvalue.FutureProgId = ChargenApplicabilityProg?.Id;
			dbvalue.OngoingValidityProgId = OngoingValidityProg?.Id;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	protected override string HelpText => $@"{base.HelpText}
    colour <colour> - sets the colour that this value represents";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "colour":
			case "color":
				var basic = command.SafeRemainingArgument;
				Colour.IColour colour = null;
				colour = long.TryParse(basic, out var value)
					? actor.Gameworld.Colours.Get(value)
					: actor.Gameworld.Colours.Get(basic).FirstOrDefault();

				if (colour == null)
				{
					actor.OutputHandler.Send(
						$"There is no such colour for you to use. See {"show colours".MXPSend("show colours")} for a list of values.");
					return;
				}

				Colour = colour;
				_name = colour.Name;
				actor.OutputHandler.Send($"You set the colour for this value to {colour.Name.Colour(colour.Basic)}.");
				Changed = true;
				return;
			default:
				base.BuildingCommand(actor, command.GetUndo());
				return;
		}
	}

	public override string Show(ICharacter actor)
	{
		return $@"{base.Show(actor)}
Colour: {Colour.Name.Colour(Colour.Basic)} (#{Colour.Id.ToString("N0", actor)})";
	}

	public override string ToString()
	{
		return $"ColourCharacteristicValue ID {Id} Name: {Name} Basic: {Colour.Basic}";
	}
}