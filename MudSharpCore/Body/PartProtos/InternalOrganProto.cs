using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public abstract class InternalOrganProto : BodypartPrototype, IOrganProto
{
	protected InternalOrganProto(BodypartProto proto, IFuturemud game)
		: base(proto, game)
	{
		HypoxiaDamagePerTick = proto.HypoxiaDamagePerTick;
		RelativeInfectability = proto.RelativeInfectability;
		ImplantSpaceOccupied = proto.ImplantSpaceOccupied;
	}

	protected InternalOrganProto(InternalOrganProto rhs, string newName) : base(rhs, newName)
	{
		HypoxiaDamagePerTick = rhs.HypoxiaDamagePerTick;
		RelativeInfectability = rhs.RelativeInfectability;
		ImplantSpaceOccupied = rhs.ImplantSpaceOccupied;
	}

	protected override void InternalSave(BodypartProto dbitem)
	{
		dbitem.HypoxiaDamagePerTick = HypoxiaDamagePerTick;
		dbitem.RelativeInfectability = RelativeInfectability;
		dbitem.ImplantSpaceOccupied = ImplantSpaceOccupied;
	}

	public double PainFactor => 1.0;

	protected virtual bool AffectedByBloodBuildup => false;

	protected virtual double BloodVolumeForTotalFailure => double.MaxValue;

	public virtual bool RequiresSpinalConnection => false;

	public double RelativeInfectability { get; set; }

	public double HypoxiaDamagePerTick { get; set; }

	public double ImplantSpaceOccupied { get; protected set; }

	public override string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder(base.ShowToBuilder(builder));
		sb.AppendLineColumns((uint)builder.LineFormatLength, 3,
			$"Implant Space Occupied: {ImplantSpaceOccupied.ToString("N3", builder).ColourValue()}",
			$"Hypoxia Per Tick: {HypoxiaDamagePerTick.ToString("N3", builder).ColourValue()}",
			$"Relative Infection: {RelativeInfectability.ToString("P3", builder).ColourValue()}"
		);
		sb.AppendLine();
		sb.AppendLine("Contained In:");
		sb.AppendLine();
		var parts = Body.AllBodyparts.Select(x => (Part: x, Info: x.OrganInfo.ValueOrDefault(this, null))).Where(x => x.Item2 is not null).ToArray();
		sb.AppendLine(StringUtilities.GetTextTable(
			from part in parts
			select new List<string>
				{
					part.Part.FullDescription(),
					(part.Info.HitChance/100.0).ToStringP2Colour(builder),
					part.Info.ProximityGroup ?? "",
					part.Info.IsPrimaryInternalLocation.ToColouredString()
				},
			new List<string>
			{
				"Parent Part",
				"Hit %",
				"Group",
				"Primary Location?"
			},
			builder,
			Telnet.Yellow));

		sb.AppendLine();
		sb.AppendLine("Covered By Bones:");
		sb.AppendLine();
		var bones = Body.AllBodypartsBonesAndOrgans.OfType<IBone>().Select(x => (Part: x, Info: x.CoveredOrgans.FirstOrDefault(y => y.Organ == this))).Where(x => x.Info.Organ is not null).ToArray();
		sb.AppendLine(StringUtilities.GetTextTable(
			from bone in bones
			select new List<string>
			{
				bone.Part.FullDescription(),
				(bone.Info.Info.HitChance/100.0).ToStringP2Colour(builder),
			},
			new List<string>
			{
				"Covering Bone",
				"Cover %",
			},
			builder,
			Telnet.Yellow));

		return sb.ToString();
	}

	protected override string HelpInfo =>
		$@"{base.HelpInfo}
	#3infectability <multiplier>#0 - base infection chance multiplier
	#3hypoxia <damage>#0 - hypoxia damage per 10 seconds to this organ
	#3occupied <space>#0 - how much space this organ occupies
	#3internal <part> <hitchance> [<group>]#0 - sets this bone to be in the target part
	#3removeinternal <part>#0 - removes this bone from a part
	#3primary <part>#0 - sets a part to be the primary bodypart for this bone";

	public override bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "infectability":
				return BuildingCommandInfectability(builder, command);
			case "hypoxia":
				return BuildingCommandHypoxia(builder, command);
			case "occupied":
			case "spaceoccupied":
			case "space_occupied":
			case "space occupied":
				return BuildingCommandSpaceOccupied(builder, command);
			case "primary":
				return BuildingCommandPrimary(builder, command);
			case "inside":
			case "internal":
				return BuildingCommandInside(builder, command);
			case "removeinside":
			case "removeinternal":
				return BuildingCommandRemoveInside(builder, command);
		}

		return base.BuildingCommand(builder, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
	}

	private bool BuildingCommandRemoveInside(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which bodypart do you want to remove this organ's presence from?");
			return false;
		}

		var name = command.PopSpeech();
		var part = Body.AllExternalBodyparts.FirstOrDefault(x => x.Name.EqualTo(name));
		if (part == null)
		{
			builder.OutputHandler.Send("There is no such bodypart.");
			return false;
		}

		if (!part.OrganInfo.ContainsKey(this))
		{
			builder.OutputHandler.Send(
				$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart does not contain the {FullDescription().Colour(Telnet.Yellow)} organ.");
			return false;
		}

		part.OrganInfo.Remove(this);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var target =
				FMDB.Context.BodypartInternalInfos.First(x => x.InternalPartId == Id && x.BodypartProtoId == part.Id);
			FMDB.Context.BodypartInternalInfos.Remove(target);
			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"You remove the {FullDescription().Colour(Telnet.Yellow)} organ from inside the {part.FullDescription().Colour(Telnet.Yellow)} bodypart.");
		return true;
	}

	private bool BuildingCommandInside(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which bodypart do you want to add or edit this organs presence inside?");
			return false;
		}

		var name = command.PopSpeech();
		var part = Body.AllExternalBodyparts.FirstOrDefault(x => x.Name.EqualTo(name));
		if (part == null)
		{
			builder.OutputHandler.Send("There is no such bodypart.");
			return false;
		}

		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"What should be the relative hit chance of this organ when the parent bodypart is hit?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(builder.Account.Culture, out var hitchance) || hitchance <= 0.0)
		{
			builder.OutputHandler.Send("The hit chance must be a number greater than 0.");
			return false;
		}

		hitchance *= 100.0;

		var group = default(string);
		if (!command.IsFinished)
		{
			group = command.PopSpeech();
		}

		if (part.OrganInfo.ContainsKey(this))
		{
			var primary = part.OrganInfo[this].IsPrimaryInternalLocation;
			part.OrganInfo[this] = new BodypartInternalInfo(hitchance, primary, group);
			using (new FMDB())
			{
				var dbtarget = FMDB.Context
				                   .BodypartProtos
				                   .Include(x => x.BodypartInternalInfosBodypartProto)
				                   .First(x => x.Id == part.Id);
				var dbinternal = dbtarget.BodypartInternalInfosBodypartProto.First(x => x.InternalPartId == Id);
				dbinternal.HitChance = hitchance;
				dbinternal.ProximityGroup = group;
				dbinternal.IsPrimaryOrganLocation = primary;
				FMDB.Context.SaveChanges();
			}

			builder.OutputHandler.Send($"You update the {FullDescription().Colour(Telnet.Yellow)} organ to be inside the {part.FullDescription().Colour(Telnet.Yellow)} bodypart with a hit chance of {(hitchance/100.0).ToString("P3", builder).ColourValue()}, proximity group of {group?.ColourValue() ?? "None".Colour(Telnet.Red)}.");
			return true;
		}

		part.OrganInfo[this] = new BodypartInternalInfo(hitchance, false, group);
		using (new FMDB())
		{
			var dbinternal = new BodypartInternalInfos();
			FMDB.Context.BodypartInternalInfos.Add(dbinternal);
			dbinternal.BodypartProtoId = part.Id;
			dbinternal.InternalPartId = Id;
			dbinternal.HitChance = hitchance;
			dbinternal.ProximityGroup = group;
			dbinternal.IsPrimaryOrganLocation = false;
			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"You set the {FullDescription().Colour(Telnet.Yellow)} organ to be inside the {part.FullDescription().Colour(Telnet.Yellow)} bodypart with a hit chance of {(100.0 * hitchance).ToString("P3", builder).ColourValue()}, proximity group of {group?.ColourValue() ?? "None".Colour(Telnet.Red)}.");
		return true;
	}

	private bool BuildingCommandPrimary(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which bodypart do you want to set as the primary location for this organ?");
			return false;
		}

		var name = command.PopSpeech();
		var part = Body.AllExternalBodyparts.FirstOrDefault(x => x.Name.EqualTo(name));
		if (part == null)
		{
			builder.OutputHandler.Send("There is no such bodypart.");
			return false;
		}

		if (!part.OrganInfo.ContainsKey(this))
		{
			builder.OutputHandler.Send(
				$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart does not contain this organ.");
			return false;
		}

		if (part.OrganInfo[this].IsPrimaryInternalLocation)
		{
			builder.OutputHandler.Send(
				$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart is already the primary location for this organ.");
			return false;
		}

		foreach (var organ in part.OrganInfo)
		{
			if (organ.Key != this)
			{
				part.OrganInfo[organ.Key] =
					new BodypartInternalInfo(organ.Value.HitChance, false, organ.Value.ProximityGroup);
			}
			else
			{
				part.OrganInfo[organ.Key] =
					new BodypartInternalInfo(organ.Value.HitChance, true, organ.Value.ProximityGroup);
			}
		}

		using (new FMDB())
		{
			foreach (var dbitem in FMDB.Context.BodypartInternalInfos
			                           .Where(x => x.BodypartProtoId == part.Id && x.InternalPart.IsOrgan == 1)
			                           .ToList())
			{
				if (dbitem.InternalPartId == Id)
				{
					dbitem.IsPrimaryOrganLocation = true;
				}
				else
				{
					dbitem.IsPrimaryOrganLocation = false;
				}
			}

			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart is now the primary location for this organ.");
		return true;
	}

	private bool BuildingCommandSpaceOccupied(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("How much space should this organ occupy in its containing bodypart?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			builder.OutputHandler.Send("You must enter a valid number 0.0 or greater.");
			return false;
		}

		ImplantSpaceOccupied = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This organ now occupies {ImplantSpaceOccupied.ToString("N3", builder).ColourValue()} space in its containing bodypart.");
		return true;
	}

	private bool BuildingCommandHypoxia(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"How much hypoxia damage should this organ suffer per 10 seconds without oxygenation?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			builder.OutputHandler.Send("You must enter a valid number 0.0 or greater.");
			return false;
		}

		HypoxiaDamagePerTick = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This organ now takes {HypoxiaDamagePerTick.ToString("N3", builder).ColourValue()} damage per tick without oxygen.");
		return true;
	}

	private bool BuildingCommandInfectability(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What multiplier to infection rate should this organ have?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			builder.OutputHandler.Send("You must enter a valid number 0.0 or greater.");
			return false;
		}

		RelativeInfectability = value;
		Changed = true;
		builder.OutputHandler.Send(
			$"This organ now gets infected at a {RelativeInfectability.ToString("P3", builder).ColourValue()} rate compared to base.");
		return true;
	}

	public double OrganFunctionFactor(IBody body)
	{
		var floor = 0.0;
		floor =
			body.EffectsOfType<IStablisedOrganFunction>().FirstOrDefault(x => x.Applies() && x.Organ == this)?.Floor ??
			0.0;

		if (body.EffectsOfType<IBodypartIneffectiveEffect>().Any(x => x.Applies() && x.Bodypart == this))
		{
			return floor;
		}

		var damageRatio =
			body.Wounds.Where(x => x.Bodypart == this).Select(x => x.CurrentDamage).DefaultIfEmpty(0).Sum() /
			body.HitpointsForBodypart(this);
		if (double.IsNaN(damageRatio))
		{
			return 0.0;
		}

		var bloodEffect = 0.0;
		if (AffectedByBloodBuildup)
		{
			var totalBlood =
				body.Effects.OfType<IInternalBleedingEffect>()
				    .Where(x => x.Organ == this)
				    .Select(x => x.BloodlossTotal)
				    .DefaultIfEmpty(0.0)
				    .Sum();
			bloodEffect = totalBlood / BloodVolumeForTotalFailure;
		}

		// TODO - effects, spells, merits etc
		return Math.Max(floor, 1.0 - damageRatio - bloodEffect);
	}

	public virtual void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor)
	{
		// Do nothing
	}
}