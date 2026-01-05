using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Form.Characteristics;

public class MultiformCharacteristicValue : CharacteristicValue
{
	public MultiformCharacteristicValue(string name, ICharacteristicDefinition type, string basic, string fancy)
		: base(name, type, basic, fancy)
	{
		Basic = basic ?? "";
		Fancy = fancy ?? "";
		Changed = true;
	}

	public MultiformCharacteristicValue(MudSharp.Models.CharacteristicValue value, IFuturemud gameworld)
		: base(value, gameworld)
	{
		Basic = value.Value ?? "";
		Fancy = value.AdditionalValue ?? "";
	}

	protected MultiformCharacteristicValue(MultiformCharacteristicValue rhs, string newName) : base(rhs, newName,
		rhs.Basic, rhs.Fancy)
	{
		Basic = rhs.Basic ?? "";
		Fancy = rhs.Fancy ?? "";
	}

	public override ICharacteristicValue Clone(string newName)
	{
		return new MultiformCharacteristicValue(this, newName);
	}

	public string Basic { get; protected set; }

	public string Fancy { get; protected set; }

	public override string GetBasicValue => Basic;

	public override string GetFancyValue => Fancy;

	public override void Save()
	{
		using (new FMDB())
		{
			var dbvalue = FMDB.Context.CharacteristicValues.Find(Id);
			dbvalue.Default = Definition.IsDefaultValue(this);
			dbvalue.Name = Name;
			dbvalue.Value = Basic;
			dbvalue.Pluralisation = (int)Pluralisation;
			dbvalue.AdditionalValue = Fancy;
			dbvalue.FutureProgId = ChargenApplicabilityProg?.Id;
			dbvalue.OngoingValidityProgId = OngoingValidityProg?.Id;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	protected override string HelpText => $@"{base.HelpText}
	#3basic <basic form>#0 - the basic form of this variable
	#3fancy <fancy form>#0 - the fancy form of this variable";

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
			default:
				base.BuildingCommand(actor, command.GetUndo());
				break;
		}
	}

	public override string ToString()
	{
		return $"MultiformCharacteristicValue ID {Id} Name: {Name} Basic: {Basic} Fancy: {Fancy}";
	}
}