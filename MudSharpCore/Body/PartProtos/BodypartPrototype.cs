using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using BodypartShape = MudSharp.Form.Shape.BodypartShape;
using Material = MudSharp.Form.Material.Material;

namespace MudSharp.Body.PartProtos;

public abstract partial class BodypartPrototype : LateKeywordedInitialisingItem, IBodypart
{

	public sealed override string FrameworkItemType => "Bodypart";

	#region Constructors

	protected BodypartPrototype(BodypartProto proto, IFuturemud game)
	{
		Gameworld = game;
		_id = proto.Id;
		IdInitialised = true;
		_name = proto.Name;
		_keywords = new Lazy<List<string>>(() => new List<string> { Name });
		Description = proto.Description;
		Shape = Gameworld.BodypartShapes.Get(proto.BodypartShapeId);
		Alignment = Enum.IsDefined(typeof(Alignment), proto.Alignment)
			? (Alignment)proto.Alignment
			: Alignment.Irrelevant;
		Orientation = Enum.IsDefined(typeof(Orientation), proto.Location)
			? (Orientation)proto.Location
			: Orientation.Irrelevant;
		PainModifier = proto.PainModifier;
		BleedModifier = proto.BleedModifier;
		DamageModifier = proto.DamageModifier;
		StunModifier = proto.StunModifier;
		SeveredThreshold = proto.SeveredThreshold;
		MaxLife = (uint)proto.MaxLife;
		RelativeHitChance = proto.RelativeHitChance;
		DefaultMaterial = Gameworld.Materials.Get(proto.DefaultMaterialId);
		Significant = proto.Significant;
		IsVital = proto.IsVital;
		IsCore = proto.IsCore;
		ImplantSpace = proto.ImplantSpace;
		NaturalArmourType = Gameworld.ArmourTypes.Get(proto.ArmourTypeId ?? 0);
		_countsAsId = proto.CountAsId;
		Size = (SizeCategory)proto.Size;
	}

	protected BodypartPrototype(BodypartPrototype rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		Body = rhs.Body;
		_name = newName;
		_keywords = new Lazy<List<string>>(() => new List<string> { Name });
		Description = newName;
		Shape = rhs.Shape;
		Alignment = rhs.Alignment;
		Orientation = rhs.Orientation;
		PainModifier = rhs.PainModifier;
		BleedModifier = rhs.BleedModifier;
		DamageModifier = rhs.DamageModifier;
		StunModifier = rhs.StunModifier;
		SeveredThreshold = rhs.SeveredThreshold;
		MaxLife = rhs.MaxLife;
		RelativeHitChance = rhs.RelativeHitChance;
		IsCore = rhs.IsCore;
		DefaultMaterial = rhs.DefaultMaterial;
		Significant = rhs.Significant;
		IsVital = rhs.IsVital;
		ImplantSpace = rhs.ImplantSpace;
		NaturalArmourType = rhs.NaturalArmourType;
		_countsAsId = rhs._countsAsId;
		Size = rhs.Size;
		UpstreamConnection = rhs.UpstreamConnection;
		Body.Limbs.FirstOrDefault(x => x.Parts.Contains(rhs))?.AddBodypart(this);
		Gameworld.SaveManager.AddInitialisation(this);
		Body.UpdateBodypartRole(this, BodypartRole.Extra);
	}

	public abstract BodypartTypeEnum BodypartType { get; }

	public void SetBodyProto(IBodyPrototype proto)
	{
		Body = proto;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new BodypartProto
		{
			BodyId = Gameworld.BodyPrototypes.First(x => x.AllBodypartsBonesAndOrgans.Contains(this)).Id,
			BodypartType = (int)BodypartType,
			IsOrgan = this is IOrganProto ? 1 : 0,
			IsCore = IsCore,
			Name = Name,
			Description = Description,
			BodypartShapeId = Shape.Id,
			Alignment = (int)Alignment,
			Location = (int)Orientation,
			PainModifier = PainModifier,
			BleedModifier = BleedModifier,
			DamageModifier = DamageModifier,
			StunModifier = StunModifier,
			SeveredThreshold = SeveredThreshold,
			MaxLife = (int)MaxLife,
			RelativeHitChance = (int)RelativeHitChance,
			DefaultMaterialId = DefaultMaterial?.Id ?? 0,
			Significant = Significant,
			IsVital = IsVital,
			ImplantSpace = ImplantSpace,
			ArmourTypeId = NaturalArmourType?.Id,
			CountAsId = _countsAsId,
			Size = (int)Size
		};

		dbitem.IsCore = dbitem.IsOrgan != 1 && !(this is IBone) && Gameworld.BodyPrototypes
		                                                                    .First(x => x.AllBodypartsBonesAndOrgans
			                                                                    .Contains(this)).CoreBodyparts
		                                                                    .Contains(this);
		if (UpstreamConnection != null)
		{
			dbitem.BodypartProtoBodypartProtoUpstreamParentNavigation.Add(new BodypartProtoBodypartProtoUpstream
				{ ChildNavigation = dbitem, Parent = UpstreamConnection.Id });
		}

		var limb = Body.Limbs.FirstOrDefault(x => x.Parts.Contains(this));
		if (limb != null)
		{
			dbitem.Limbs.Add(FMDB.Context.Limbs.Find(limb.Id));
		}

		InternalSave(dbitem);
		FMDB.Context.BodypartProtos.Add(dbitem);
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((BodypartProto)dbitem).Id;
	}

	#endregion

	#region ISaveable Members

	public override void Save()
	{
		var dbitem = FMDB.Context.BodypartProtos.Find(Id);
		if (dbitem == null)
		{
			throw new ApplicationException($"Tried to save a bodypart that didn't exist with ID {Id} ({Description}).");
		}

		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.BodypartShapeId = Shape.Id;
		dbitem.Alignment = (int)Alignment;
		dbitem.Location = (int)Orientation;
		dbitem.PainModifier = PainModifier;
		dbitem.BleedModifier = BleedModifier;
		dbitem.DamageModifier = DamageModifier;
		dbitem.StunModifier = StunModifier;
		dbitem.SeveredThreshold = SeveredThreshold;
		dbitem.MaxLife = (int)MaxLife;
		dbitem.RelativeHitChance = (int)RelativeHitChance;
		dbitem.DefaultMaterialId = DefaultMaterial?.Id ?? 0;
		dbitem.Significant = Significant;
		dbitem.IsVital = IsVital;
		dbitem.ImplantSpace = ImplantSpace;
		dbitem.ArmourTypeId = NaturalArmourType?.Id;
		dbitem.CountAsId = _countsAsId;
		dbitem.Size = (int)Size;
		InternalSave(dbitem);
		Changed = false;
	}

	public abstract IBodypart Clone(string newName);

	protected virtual void InternalSave(BodypartProto dbitem)
	{
		// Do nothing
	}

	public virtual string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Bodypart #{Id.ToString("N0", builder).Colour(Telnet.Green)}: {Description.Colour(Telnet.Yellow)} [{Name}]");
		sb.AppendLine($"Type: {GetType().Name.Colour(Telnet.Cyan)}");
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Shape: {Shape.Name.Colour(Telnet.Green)}",
			$"Alignment: {Alignment.Describe().Colour(Telnet.Green)}",
			$"Orientation: {Orientation.Describe().Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Damage Mod: {DamageModifier.ToString("P2", builder).Colour(Telnet.Green)}",
			$"Stun Mod: {StunModifier.ToString("P2", builder).Colour(Telnet.Green)}",
			$"Pain Mod: {PainModifier.ToString("P2", builder).Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Max Damage: {MaxLife.ToString("N0", builder).Colour(Telnet.Green)}",
			$"Sever Threshold: {SeveredThreshold.ToString("N0", builder).Colour(Telnet.Green)}",
			$"Bleed Mod: {BleedModifier.ToString("P2", builder).Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Relative Hit: {RelativeHitChance.ToString("N0", builder).Colour(Telnet.Green)}",
			$"Vital?: {IsVital.ToString(builder).Colour(Telnet.Green)}",
			$"Significant?: {Significant.ToString(builder).Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Material: {DefaultMaterial?.Name.Colour(DefaultMaterial.ResidueColour) ?? "None".Colour(Telnet.Red)}",
			$"Armour Type: {NaturalArmourType?.Name.FluentTagMXP("send", $"href='show armour {NaturalArmourType.Id}' hint='Click to show the armour type'") ?? "None".Colour(Telnet.Red)}",
			$"Implant Space: {ImplantSpace.ToString("N2", builder).Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Size: {Size.Describe().ColourValue()}",
			$"Counts As: {(_countsAsId.HasValue ? _countsAsId.Value.ToString("N0", builder).ColourValue() : "None".Colour(Telnet.Red))}",
			$""
		);
		return sb.ToString();
	}

	public virtual bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(builder, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(builder, command);
			case "type":
				return BuildingCommandType(builder, command);
			case "shape":
				return BuildingCommandShape(builder, command);
			case "alignment":
				return BuildingCommandAlignment(builder, command);
			case "orientation":
				return BuildingCommandOrientation(builder, command);
			case "damage":
				return BuildingCommandDamageMod(builder, command);
			case "stun":
				return BuildingCommandStunMod(builder, command);
			case "pain":
				return BuildingCommandPainMod(builder, command);
			case "life":
				return BuildingCommandLife(builder, command);
			case "sever":
				return BuildingCommandSever(builder, command);
			case "bleed":
				return BuildingCommandBleed(builder, command);
			case "hit":
				return BuildingCommandHit(builder, command);
			case "vital":
				return BuildingCommandVital(builder, command);
			case "significant":
				return BuildingCommandSignificant(builder, command);
			case "material":
				return BuildingCommandMaterial(builder, command);
			case "armour":
			case "armor":
				return BuildingCommandArmour(builder, command);
			case "implant":
			case "implantspace":
			case "space":
			case "implant_space":
			case "implant space":
				return BuildingCommandImplantSpace(builder, command);
			case "size":
				return BuildingCommandSize(builder, command);
			case "countas":
			case "count as":
			case "count_as":
			case "countsas":
			case "counts_as":
			case "counts as":
				return BuildingCommandCountsAs(builder, command);
			case "core":
				return BuildingCommandCore(builder, command);
			case "extra":
				return BuildingCommandExtra(builder, command);
			case "male":
				return BuildingCommandMale(builder, command);
			case "female":
				return BuildingCommandFemale(builder, command);
			case "?":
			case "help":
			default:
				builder.OutputHandler.Send(HelpInfo);
				return false;
		}
	}


	private bool BuildingCommandFemale(ICharacter builder, StringStack command)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodypartProtos.Find(Id);
			dbitem.IsCore = false;
			FMDB.Context.BodyProtosAdditionalBodyparts.RemoveRange(dbitem.BodyProtosAdditionalBodyparts);
			var newAdditional = new BodyProtosAdditionalBodyparts
			{
				BodypartId = Id,
				BodyProtoId = Body.Id,
				Usage = "female"
			};
			dbitem.BodyProtosAdditionalBodyparts.Add(newAdditional);
			FMDB.Context.SaveChanges();
		}

		Body.UpdateBodypartRole(this, BodypartRole.FemaleAddition);
		builder.OutputHandler.Send("This bodypart will now only be added to female bodies.");
		return true;
	}

	private bool BuildingCommandMale(ICharacter builder, StringStack command)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodypartProtos.Find(Id);
			dbitem.IsCore = false;
			FMDB.Context.BodyProtosAdditionalBodyparts.RemoveRange(dbitem.BodyProtosAdditionalBodyparts);
			var newAdditional = new BodyProtosAdditionalBodyparts
			{
				BodypartId = Id,
				BodyProtoId = Body.Id,
				Usage = "male"
			};
			dbitem.BodyProtosAdditionalBodyparts.Add(newAdditional);
			FMDB.Context.SaveChanges();
		}

		Body.UpdateBodypartRole(this, BodypartRole.MaleAddition);
		builder.OutputHandler.Send("This bodypart will now only be added to male bodies.");
		return true;
	}

	private bool BuildingCommandExtra(ICharacter builder, StringStack command)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodypartProtos.Find(Id);
			dbitem.IsCore = false;
			FMDB.Context.BodyProtosAdditionalBodyparts.RemoveRange(dbitem.BodyProtosAdditionalBodyparts);
			FMDB.Context.SaveChanges();
		}

		Body.UpdateBodypartRole(this, BodypartRole.Extra);
		builder.OutputHandler.Send("This bodypart will now be only added by extra means, such as races or spells.");
		return true;
	}

	private bool BuildingCommandCore(ICharacter builder, StringStack command)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodypartProtos.Find(Id);
			dbitem.IsCore = true;
			FMDB.Context.BodyProtosAdditionalBodyparts.RemoveRange(dbitem.BodyProtosAdditionalBodyparts);
			FMDB.Context.SaveChanges();
		}

		Body.UpdateBodypartRole(this, BodypartRole.Core);
		builder.OutputHandler.Send("This bodypart is now a core bodypart for the body, being always present.");
		return true;
	}

	private bool BuildingCommandCountsAs(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"You must either enter another bodypart for this one to count as, or 'none' to clear it.");
			return false;
		}

		if (command.Peek().EqualToAny("none", "clear", "delete"))
		{
			_countsAsId = null;
			Changed = true;
			builder.OutputHandler.Send("This bodypart will no longer count as any other.");
			return true;
		}

		var target = long.TryParse(command.PopSpeech(), out var value)
			? Body.AllBodyparts.FirstOrDefault(x => x.Id == value)
			: Body.AllBodyparts.FirstOrDefault(x =>
				  x.Name.EqualTo(command.Last) || x.FullDescription().EqualTo(command.Last)) ??
			  Body.AllBodyparts.FirstOrDefault(x =>
				  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase) || x.FullDescription()
					  .StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (target == null)
		{
			builder.OutputHandler.Send($"The {Body.Name.Colour(Telnet.Yellow)} body does not have any such bodypart.");
			return false;
		}

		if (target == this)
		{
			builder.OutputHandler.Send("You cannot make a bodypart count as itself.");
			return false;
		}

		if (target.CountsAs(this))
		{
			builder.OutputHandler.Send(
				$"The {target.FullDescription().Colour(Telnet.Yellow)} bodypart already counts as this bodypart, and you may not create circular chains of counting-as.");
			return false;
		}

		_countsAsId = target.Id;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart now counts as the {target.FullDescription().Colour(Telnet.Yellow)} bodypart.");
		return true;
	}

	private bool BuildingCommandSize(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What size do you want to set this bodypart to be?");
			return false;
		}

		if (!GameItemEnumExtensions.TryParseSize(command.PopSpeech(), out var size))
		{
			builder.OutputHandler.Send(
				$"That is not a valid size. See {"show sizes".ColourCommand()} for a list of valid sizes.");
			return false;
		}

		Size = size;
		Changed = true;
		builder.OutputHandler.Send($"This bodypart is now {size.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandImplantSpace(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("How much space for implants should be available in this bodypart?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			builder.OutputHandler.Send("You must enter a valid number 0 or greater.");
			return false;
		}

		ImplantSpace = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now have {ImplantSpace.ToString("N2", builder).ColourValue()} units of space for implants.");
		var organs = OrganInfo.Where(x => x.Value.IsPrimaryInternalLocation).Sum(x => x.Key.ImplantSpaceOccupied);
		if (organs > ImplantSpace)
		{
			builder.OutputHandler.Send(
				$"Warning: This is {(organs - ImplantSpace).ToString("N2", builder).ColourValue()} units of space less than the organs take up on their own."
					.Colour(Telnet.Red));
		}

		return true;
	}

	private bool BuildingCommandArmour(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"What default natural armour should this bodypart use? Use 'none' if it shouldn't have any.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			NaturalArmourType = null;
			Changed = true;
			builder.OutputHandler.Send("This bodypart will no longer have any natural armour.");
			return true;
		}

		var armour = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ArmourTypes.Get(value)
			: Gameworld.ArmourTypes.GetByName(command.Last);
		if (armour == null)
		{
			builder.OutputHandler.Send("There is no such armour type.");
			return false;
		}

		NaturalArmourType = armour;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart now uses {NaturalArmourType.Name.Colour(Telnet.BoldWhite)} for natural armour.");
		return true;
	}

	private bool BuildingCommandMaterial(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What default material should this bodypart use (if not overridden)?");
			return false;
		}

		var material = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Materials.Get(value)
			: Gameworld.Materials.GetByName(command.Last);
		if (material == null)
		{
			builder.OutputHandler.Send("There is no such material.");
			return false;
		}

		DefaultMaterial = material;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now be made out of {material.Name.Colour(material.ResidueColour)}.");
		return true;
	}

	private bool BuildingCommandSignificant(ICharacter builder, StringStack command)
	{
		Significant = !Significant;
		Changed = true;
		builder.OutputHandler.Send($"This bodypart is {(Significant ? "now" : "no longer")} significant.");
		return true;
	}

	private bool BuildingCommandVital(ICharacter builder, StringStack command)
	{
		IsVital = !IsVital;
		Changed = true;
		builder.OutputHandler.Send($"This bodypart is {(IsVital ? "now" : "no longer")} vital.");
		return true;
	}

	private bool BuildingCommandHit(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("How many hit chances should this bodypart have for melee combat?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 0)
		{
			builder.OutputHandler.Send("You must enter a whole number that is 0 or greater.");
			return false;
		}

		RelativeHitChance = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart now has a relative hit chance of {RelativeHitChance.ToString("N0").ColourValue()}, which gives it a {((double)RelativeHitChance / Body.AllBodyparts.Sum(x => x.RelativeHitChance)).ToString("P2", builder).ColourValue()} chance to be hit overall.");
		return true;
	}

	private bool BuildingCommandBleed(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What modifier should be applied to the base bleed rate for this bodypart?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			builder.OutputHandler.Send("You must enter a valid, positive multiplier.");
			return false;
		}

		BleedModifier = value;
		Changed = true;

		builder.OutputHandler.Send(
			$"This bodypart now has a bleed modifier of {BleedModifier.ToString("N3", builder).ColourValue()}, meaning a severe wound at heavy exertion would bleed at {Gameworld.UnitManager.DescribeMostSignificantExact(((int)WoundSeverity.Severe + ((int)ExertionLevel.Heavy - 4)) * Gameworld.GetStaticDouble("") * BleedModifier, Framework.Units.UnitType.FluidVolume, builder).ColourValue()} per ten seconds.");
		return true;
	}

	private bool BuildingCommandSever(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"How much damage in a single attack should cause this bodypart to be severed? Use -1 to disable severing.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		SeveredThreshold = value;
		if (SeveredThreshold < -1)
		{
			SeveredThreshold = -1;
		}

		Changed = true;
		if (SeveredThreshold == -1)
		{
			builder.OutputHandler.Send($"This bodypart will no longer sever, no matter the damage.");
		}
		else
		{
			builder.OutputHandler.Send(
				$"This bodypart will now sever if it receives more than {SeveredThreshold.ToString("N0", builder).ColourValue()} damage in a single attack.");
		}

		return true;
	}

	private bool BuildingCommandLife(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"How much damage or pain should this bodypart be able to absorb before becoming disabled?");
			return false;
		}

		if (!uint.TryParse(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send("You must enter a valid number greater than zero.");
			return false;
		}

		MaxLife = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now be able to suffer {MaxLife.ToString("N0", builder).ColourValue()} damage or pain before becoming disabled.");
		return true;
	}

	private bool BuildingCommandPainMod(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("How much should all incoming pain be multiplied by for this bodypart?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		PainModifier = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now multiply all incoming pain by {PainModifier.ToString("N3", builder).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandStunMod(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("How much should all incoming stun be multiplied by for this bodypart?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		StunModifier = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now multiply all incoming stun by {StunModifier.ToString("N3", builder).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDamageMod(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("How much should all incoming damage be multiplied by for this bodypart?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		DamageModifier = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now multiply all incoming damage by {DamageModifier.ToString("N3", builder).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandShape(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				$"Which bodypart shape should this bodypart use? See {"show bodypartshapes".ColourCommand()} for a list.");
			return false;
		}

		var shape = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.BodypartShapes.Get(value)
			: Gameworld.BodypartShapes.GetByName(command.Last);
		if (shape == null)
		{
			builder.OutputHandler.Send("There is no such bodypart shape.");
			return false;
		}

		Shape = shape;
		Changed = true;
		builder.OutputHandler.Send($"This bodypart will now use the {shape.Name.Colour(Telnet.Yellow)} shape.");
		return true;
	}

	private bool BuildingCommandAlignment(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which alignment does this bodypart occupy?");
			return false;
		}

		if (!Utilities.TryParseEnum<Alignment>(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send(
				$"That is not a valid alignment. Valid alignments are {Enum.GetValues(typeof(Alignment)).OfType<Alignment>().Except(Alignment.Irrelevant).Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
			return false;
		}

		Alignment = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now occupy the {Alignment.Describe().ColourValue()} alignment.");
		return true;
	}

	private bool BuildingCommandOrientation(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which orientation does this bodypart occupy?");
			return false;
		}

		if (!Utilities.TryParseEnum<Orientation>(command.PopSpeech(), out var value))
		{
			builder.OutputHandler.Send(
				$"That is not a valid orientation. Valid orientations are {Enum.GetValues(typeof(Orientation)).OfType<Orientation>().Except(Orientation.Irrelevant).Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString(conjunction: "or ")}.");
			return false;
		}

		Orientation = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This bodypart will now occupy the {Orientation.Describe().ColourValue()} orientation.");
		return true;
	}

	private bool BuildingCommandType(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				$"What bodypart type do you want to change this bodypart into? The complete list is as follows: {Enum.GetNames(typeof(BodypartTypeEnum)).Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		if (!Utilities.TryParseEnum<BodypartTypeEnum>(command.PopSpeech(), out var type))
		{
			builder.OutputHandler.Send(
				$"That is not a valid bodypart type. The complete list is as follows: {Enum.GetNames(typeof(BodypartTypeEnum)).Select(x => x.Colour(Telnet.Cyan)).ListToString()}.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodypartProtos.Find(Id);
			dbitem.BodypartType = (int)type;
			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"You change the type of this bodypart to {type.DescribeEnum().Colour(Telnet.Yellow)}. Warning: this will not take affect until you reboot the MUD.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"What description do you want to set for this bodypart? The description is the full name of the bodypart, like 'right leg'.");
			return false;
		}

		Description = command.PopSpeech().ToLowerInvariant();
		Changed = true;
		builder.OutputHandler.Send($"This bodypart is now described as the {Description.Colour(Telnet.Yellow)}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"What name do you want to give to this bodypart? Names are usually the short version of the description like 'rleg'.");
			return false;
		}

		var name = command.PopSpeech().ToLowerInvariant();
		if (Body.AllBodypartsBonesAndOrgans.Any(x => x.Name.EqualTo(name)))
		{
			builder.OutputHandler.Send("There is already a bodypart with that name. Names must be unique.");
			return false;
		}

		_name = name;
		Changed = true;
		builder.OutputHandler.Send($"This bodypart is now named {name.Colour(Telnet.Cyan)}.");
		return true;
	}

	protected virtual string HelpInfo =>
		"You can use the following options with this command:\n\tname <name> - renames this bodypart\n\tdescription <desc> - sets the description\n\ttype <type> - changes the type\n\torientation <orientation> - set the orientation\n\talignment <alignment> - sets the alignment\n\tshape <shape> - sets the shape\n\tdamage <multiplier> - sets the damage multiplier\n\tpain <multiplier> - sets the pain multiplier\n\tstun <multiplier> - sets the stun multiplier\n\tlife <amount> - sets the max life of this bodypart\n\tsever <amount> - sets the threshold for severing\n\tbleed <multiplier> - sets the bleed modifier\n\thit <chances> - sets the hit chances for this bodypart\n\tvital - toggles whether this is a vital part\n\tsignificant - toggles whether this is a significant part\n\tmaterial <material> - sets the material this part is made from\n\tarmour <armour> - sets the default armour type\n\timplantspace <space> - sets the implant space\n\tsize <size> - sets the bodypart size\n\tcountas <other> - sets a bodypart for this to count as";

	#endregion

	public double ImplantSpace { get; protected set; }

	public SizeCategory Size { get; protected set; }

	protected string Description { get; set; }
	public IBodypartShape Shape { get; protected set; }

	public bool Significant { get; set; }

	public bool IsVital { get; set; }

	public bool IsCore { get; set; }

	public ISolid DefaultMaterial { get; protected set; }

	public string ShortDescription(bool proper = false, bool colour = true,
		PermissionLevel informationLevel = PermissionLevel.Any)
	{
		return colour ? Description.FluentProper(proper).Colour(Telnet.White) : Description.FluentProper(proper);
	}

	public string FullDescription(bool proper = false,
		PermissionLevel informationLevel = PermissionLevel.Any)
	{
		return Description.FluentProper(proper);
	}

	public override Gendering Gender => Neuter.Instance;

	public static IBodypart LoadFromDatabase(BodypartProto proto, IFuturemud gameworld)
	{
		switch ((BodypartTypeEnum)proto.BodypartType)
		{
			case BodypartTypeEnum.Drapeable:
				return new DrapeableBodypartProto(proto, gameworld);
			case BodypartTypeEnum.GrabbingWielding:
				return new GrabbingWieldingBodypartProto(proto, gameworld);
			case BodypartTypeEnum.Grabbing:
				return new GrabbingBodypartProto(proto, gameworld);
			case BodypartTypeEnum.Wielding:
				return new WieldingBodypartProto(proto, gameworld);
			case BodypartTypeEnum.Brain:
				return new BrainProto(proto, gameworld);
			case BodypartTypeEnum.Liver:
				return new LiverProto(proto, gameworld);
			case BodypartTypeEnum.Heart:
				return new HeartProto(proto, gameworld);
			case BodypartTypeEnum.Standing:
				return new StandingBodypartProto(proto, gameworld);
			case BodypartTypeEnum.Eye:
				return new EyeProto(proto, gameworld);
			case BodypartTypeEnum.Ear:
				return new EarProto(proto, gameworld);
			case BodypartTypeEnum.Spleen:
				return new SpleenProto(proto, gameworld);
			case BodypartTypeEnum.Intestines:
				return new IntestinesProto(proto, gameworld);
			case BodypartTypeEnum.Spine:
				return new SpineProto(proto, gameworld);
			case BodypartTypeEnum.Stomach:
				return new StomachProto(proto, gameworld);
			case BodypartTypeEnum.Lung:
				return new LungProto(proto, gameworld);
			case BodypartTypeEnum.Trachea:
				return new TracheaProto(proto, gameworld);
			case BodypartTypeEnum.Kidney:
				return new KidneyProto(proto, gameworld);
			case BodypartTypeEnum.Esophagus:
				return new EsophagusProto(proto, gameworld);
			case BodypartTypeEnum.Tongue:
				return new TongueProto(proto, gameworld);
			case BodypartTypeEnum.Mouth:
				return new MouthProto(proto, gameworld);
			case BodypartTypeEnum.Bone:
				return new BoneProto(proto, gameworld);
			case BodypartTypeEnum.NonImmobilisingBone:
				return new NonImmobilisingBoneProto(proto, gameworld);
			case BodypartTypeEnum.MinorBone:
				return new MinorBoneProto(proto, gameworld);
			case BodypartTypeEnum.MinorNonImobilisingBone:
				return new MinorNonImmobilisingBoneProto(proto, gameworld);
			case BodypartTypeEnum.Wing:
				return new WingProto(proto, gameworld);
			case BodypartTypeEnum.PositronicBrain:
				return new PositronicBrain(proto, gameworld);
			case BodypartTypeEnum.PowerCore:
				return new PowerCore(proto, gameworld);
			case BodypartTypeEnum.SpeechSynthesizer:
				return new SpeechSynthesizer(proto, gameworld);
			case BodypartTypeEnum.Joint:
				return new JointProto(proto, gameworld);
			case BodypartTypeEnum.Fin:
				return new FinProto(proto, gameworld);
			case BodypartTypeEnum.Gill:
				return new GillProto(proto, gameworld);
			case BodypartTypeEnum.Blowhole:
				return new BlowholeProto(proto, gameworld);
			case BodypartTypeEnum.BonyDrapeable:
				return new BonyDrapeableBodypartProto(proto, gameworld);
			default:
				throw new NotImplementedException();
		}
	}

	public override string ToString()
	{
		return $"Bodypart {Name} - {Id}";
	}

	#region IBodypartPrototype Members

	protected readonly Dictionary<Alignment, int> HitChanceAlignment = new();
	protected readonly Dictionary<Orientation, int> HitChanceOrientation = new();
	protected readonly List<IOrganProto> _organs = new();
	protected readonly List<IBone> _bones = new();
	public int SeveredThreshold { get; protected set; }
	public Alignment Alignment { get; protected set; }

	public Orientation Orientation { get; protected set; }

	public double RelativeHitChance { get; protected set; }

	public double PainModifier { get; protected set; }

	public double BleedModifier { get; protected set; }

	public double DamageModifier { get; protected set; }

	public double StunModifier { get; protected set; }

	public int Weight { get; protected set; }

	public Material Material { get; protected set; }

	public bool CanSever => SeveredThreshold > 0;

	public uint MaxLife { get; protected set; }

	public virtual bool PartDamageEffects(IBody owner, CanUseBodypartResult why)
	{
		switch (why)
		{
			case CanUseBodypartResult.CantUsePartDamage:
				if (owner.EffectsOfType<BodypartExcessivelyDamaged>().All(x => x.Bodypart != this) &&
				    owner.EffectsOfType<InsignificantBodypartExcessivelyDamaged>().All(x => x.Bodypart != this))
				{
					owner.AddEffect(Significant
						? (IEffect)new BodypartExcessivelyDamaged(owner, this)
						: new InsignificantBodypartExcessivelyDamaged(owner, this));
					return true;
				}

				break;
			case CanUseBodypartResult.CantUsePartPain:
				if (Significant && !owner.CombinedEffectsOfType<BodypartExcessivelyPainful>()
				                         .Any(x => x.Bodypart == this))
				{
					owner.AddEffect(new BodypartExcessivelyPainful(owner, this));
					return true;
				}

				break;
		}

		return false;
	}

	int IBodypart.HitChances()
	{
		return HitChanceAlignment[Alignment.Irrelevant] + HitChanceOrientation[Orientation.Irrelevant];
	}

	int IBodypart.HitChances(Alignment alignment, Orientation orientation)
	{
		return HitChanceAlignment[alignment] + HitChanceOrientation[orientation];
	}

	public IBodypart UpstreamConnection { get; protected set; }

	public IEnumerable<IOrganProto> Organs => _organs;

	public IEnumerable<IBone> Bones => _bones;
	public Dictionary<IOrganProto, BodypartInternalInfo> OrganInfo { get; } = new();
	public Dictionary<IBone, BodypartInternalInfo> BoneInfo { get; } = new();

	public IBodyPrototype Body { get; protected set; }

	public void LinkUpstream(IBodypart part)
	{
		UpstreamConnection = part;
	}

	public void LinkOrgan(IOrganProto part, BodypartInternalInfo info)
	{
		_organs.Add(part);
		OrganInfo[part] = info;
	}

	public void LinkBone(IBone bone, BodypartInternalInfo info)
	{
		_bones.Add(bone);
		BoneInfo[bone] = info;
	}

	public bool DownstreamOfPart(IBodypart part)
	{
		if (UpstreamConnection == null)
		{
			return false;
		}

		return UpstreamConnection.Equals(part) || UpstreamConnection.CountsAs(part) ||
		       UpstreamConnection.DownstreamOfPart(part);
	}

	public virtual void PostLoadProcessing(IBodyPrototype body, BodypartProto proto)
	{
		// Do nothing
	}

	public IArmourType NaturalArmourType { get; protected set; }

	protected long? _countsAsId;

	public bool CountsAs(IBodypart otherBodypart)
	{
		return otherBodypart != null && (otherBodypart == this || otherBodypart.Id == _countsAsId);
	}

	#endregion
}