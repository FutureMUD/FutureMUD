using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Combat;

public class ShieldType : SaveableItem, IShieldType
{
	public ShieldType(MudSharp.Models.ShieldType type, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = type.Id;
		_name = type.Name;
		BlockTrait = gameworld.Traits.Get(type.BlockTraitId);
		EffectiveArmourType = gameworld.ArmourTypes.Get(type.EffectiveArmourTypeId ?? 0);
		StaminaPerBlock = type.StaminaPerBlock;
		BlockBonus = type.BlockBonus;
	}

	public ShieldType(IFuturemud gameworld, string name, ITraitDefinition trait, IArmourType armour)
	{
		Gameworld = gameworld;
		_name = name;
		BlockTrait = trait;
		EffectiveArmourType = armour;
		StaminaPerBlock = 3.0;
		BlockBonus = 0.0;
		DoDatabaseInsert();
	}

	private ShieldType(ShieldType rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		BlockTrait = rhs.BlockTrait;
		BlockBonus = rhs.BlockBonus;
		EffectiveArmourType = rhs.EffectiveArmourType;
		StaminaPerBlock = rhs.StaminaPerBlock;
		DoDatabaseInsert();
	}

	public IShieldType Clone(string name)
	{
		return new ShieldType(this, name);
	}

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.ShieldType
			{
				Name = Name,
				BlockTraitId = BlockTrait.Id,
				BlockBonus = BlockBonus,
				StaminaPerBlock = StaminaPerBlock,
				EffectiveArmourTypeId = EffectiveArmourType?.Id
			};
			FMDB.Context.ShieldTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	#region Overrides of Item

	public override string FrameworkItemType => "ShieldType";

	#endregion

	#region Implementation of IShieldType

	public ITraitDefinition BlockTrait { get; private set; }
	public double BlockBonus { get; private set; }
	public double StaminaPerBlock { get; private set; }
	public IArmourType EffectiveArmourType { get; private set; }

	#endregion

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.ShieldTypes.Find(Id);
		dbitem.Name = Name;
		dbitem.BlockBonus = BlockBonus;
		dbitem.BlockTraitId = BlockTrait.Id;
		dbitem.EffectiveArmourTypeId = EffectiveArmourType?.Id;
		dbitem.StaminaPerBlock = StaminaPerBlock;
		Changed = false;
	}

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "trait":
			case "skill":
				return BuildingCommandTrait(actor, command);
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "stamina":
				return BuildingCommandStamina(actor, command);
			case "armour":
			case "armor":
				return BuildingCommandArmour(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this command:

	#3name <name>#0 - renames the armour type
	#3trait <which>#0 - sets the trait used for blocking
	#3bonus <##>#0 - sets the bonus for blocking
	#3stamina <##>#0 - sets the stamina usage per block
	#3armour <which>#0 - sets the armour type for reducing damage".SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this armour type?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.ShieldTypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a shield type with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the shield type {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or trait should be used in the block check with this shield?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send($"There is no skill or trait identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		BlockTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This shield will now use the {trait.Name.ColourValue()} trait for block checks.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the bonus be for this shield type?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		BlockBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"This shield type now has a bonus of {value.ToBonusString(actor)}.");
		return true;
	}

	private bool BuildingCommandStamina(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much stamina should be used per block with this shield type?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid positive number.");
			return false;
		}

		StaminaPerBlock = value;
		Changed = true;
		actor.OutputHandler.Send($"This shield now has a base stamina per block of {value.ToStringN2Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandArmour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which armour type should this shield use for damage reduction?");
			return false;
		}

		var armour = Gameworld.ArmourTypes.GetByIdOrName(command.SafeRemainingArgument);
		if (armour is null)
		{
			actor.OutputHandler.Send($"There is no armour type identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		EffectiveArmourType = armour;
		Changed = true;
		actor.OutputHandler.Send($"This shield type now uses the {armour.Name.ColourName()} armour type.");
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Shield Type #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Block Trait: {BlockTrait.Name.ColourValue()}");
		sb.AppendLine($"Block Bonus: {BlockBonus.ToBonusString(actor)}");
		sb.AppendLine($"Stamina Per Block: {StaminaPerBlock.ToStringN2Colour(actor)}");
		sb.AppendLine($"Effective Armour: {EffectiveArmourType?.Name.ColourValue() ?? "None".ColourError()}");
		return sb.ToString();
	}
}