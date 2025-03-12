using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp.Body.PartProtos;

public abstract class BaseBoneProto : BodypartPrototype, IBone
{
	protected BaseBoneProto(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	protected BaseBoneProto(BodypartPrototype rhs, string newName) : base(rhs, newName)
	{
	}

	public abstract bool CriticalBone { get; }
	public abstract bool CanBeImmobilised { get; }
	public abstract double BoneHealingModifier { get; }
	public double BoneEffectiveHealthModifier => 1.0;

	private readonly List<(IOrganProto Organ, BodypartInternalInfo Info)> _coveredOrgans = new();
	public IEnumerable<(IOrganProto Organ, BodypartInternalInfo Info)> CoveredOrgans => _coveredOrgans;

	public (double OrdinaryDamage, double BoneDamage) ShouldBeBoneBreak(IDamage damage)
	{
		switch (damage.DamageType)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
			case DamageType.Crushing:
			case DamageType.Piercing:
			case DamageType.Ballistic:
			case DamageType.Shockwave:
			case DamageType.Bite:
			case DamageType.Claw:
			case DamageType.Shearing:
			case DamageType.BallisticArmourPiercing:
			case DamageType.ArmourPiercing:
			case DamageType.Wrenching:
			case DamageType.Shrapnel:
			case DamageType.Falling:
			case DamageType.Eldritch:
			case DamageType.Arcane:
				return (0.0, damage.DamageAmount);
		}

		return (damage.DamageAmount, 0.0);
	}

	public override void PostLoadProcessing(IBodyPrototype body, BodypartProto proto)
	{
		foreach (var organ in proto.BoneOrganCoveragesBone)
		{
			_coveredOrgans.Add((Gameworld.BodypartPrototypes.OfType<IOrganProto>().First(x => x.Id == organ.OrganId),
				new BodypartInternalInfo(organ.CoverageChance, false, string.Empty)));
		}
	}

	#region Building Commands

	public override string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder(base.ShowToBuilder(builder));
		sb.AppendLine();
		sb.AppendLine("Contained In:");
		sb.AppendLine();
		var parts = Body.AllBodyparts.Select(x => (Part: x, Info: x.BoneInfo.ValueOrDefault(this, null))).Where(x => x.Item2 is not null).ToArray();
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
		sb.AppendLine("Organ Coverage:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from bone in CoveredOrgans
			select new List<string>
			{
				bone.Organ.FullDescription(),
				(bone.Info.HitChance/100.0).ToStringP2Colour(builder),
			},
			new List<string>
			{
				"Covered Organ",
				"Cover %",
			},
			builder,
			Telnet.Yellow));
		return sb.ToString();
	}

	protected override string HelpInfo =>
		$@"{base.HelpInfo}
	#3internal <part> <hitchance> [<group>]#0 - sets this bone to be in the target part
	#3removeinternal <part>#0 - removes this bone from a part
	#3primary <part>#0 - sets a part to be the primary bodypart for this bone
	#3cover <organ> <%>#0 - sets the bone to provide cover to an organ
	#3uncover <organ>#0 - removes this bone providing coverage";

	public override bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "primary":
				return BuildingCommandPrimary(builder, command);
			case "inside":
			case "internal":
				return BuildingCommandInside(builder, command);
			case "removeinside":
			case "removeinternal":
				return BuildingCommandRemoveInside(builder, command);
			case "cover":
				return BuildingCommandCover(builder, command);
			case "uncover":
				return BuildingCommandUncover(builder, command);
		}

		return base.BuildingCommand(builder, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
	}

	private bool BuildingCommandUncover(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which organ do you want to remove the cover this bone provides from?");
			return false;
		}

		var name = command.PopSpeech();
		var organ = Body.Organs.FirstOrDefault(x => x.Name.EqualTo(name));
		if (organ == null)
		{
			builder.OutputHandler.Send("There is no such organ.");
			return false;
		}

		if (!_coveredOrgans.Any(x => x.Organ == organ))
		{
			builder.OutputHandler.Send(
				$"That bone does not provide cover for the {organ.FullDescription().Colour(Telnet.Yellow)} organ.");
			return false;
		}

		_coveredOrgans.RemoveAll(x => x.Organ == organ);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			foreach (var cover in FMDB.Context.BoneOrganCoverages.Where(x => x.BoneId == Id).ToList())
			{
				if (cover.OrganId == organ.Id)
				{
					FMDB.Context.BoneOrganCoverages.Remove(cover);
				}
			}

			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"The {FullDescription().Colour(Telnet.Yellow)} bone will no longer cover the {organ.FullDescription().Colour(Telnet.Yellow)} organ.");
		return true;
	}

	private bool BuildingCommandCover(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which organ do you want to add or edit bone coverage for?");
			return false;
		}

		var name = command.PopSpeech();
		var organ = Body.Organs.FirstOrDefault(x => x.Name.EqualTo(name));
		if (organ == null)
		{
			builder.OutputHandler.Send("There is no such organ.");
			return false;
		}

		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What should be the chance of this bone to protect the organ below?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(builder.Account.Culture, out var hitchance) || hitchance <= 0.0)
		{
			builder.OutputHandler.Send("The hit chance must be a number greater than 0.");
			return false;
		}

		hitchance *= 100.0;

		if (_coveredOrgans.Any(x => x.Organ == organ))
		{
			_coveredOrgans[_coveredOrgans.FindIndex(x => x.Organ == organ)] =
				(organ, new BodypartInternalInfo(hitchance, false, string.Empty));
			using (new FMDB())
			{
				var dbitem = FMDB.Context.BoneOrganCoverages.First(x => x.BoneId == Id && x.OrganId == organ.Id);
				dbitem.CoverageChance = hitchance;
				FMDB.Context.SaveChanges();
			}

			builder.OutputHandler.Send(
				$"You update the coverage chance for organ {organ.FullDescription().Colour(Telnet.Yellow)} to {(hitchance/100.0).ToString("P3", builder).ColourValue()}.");
			return true;
		}

		_coveredOrgans.Add((organ, new BodypartInternalInfo(hitchance, false, string.Empty)));
		using (new FMDB())
		{
			var dbitem = new BoneOrganCoverage
			{
				BoneId = Id,
				OrganId = organ.Id
			};
			FMDB.Context.BoneOrganCoverages.Add(dbitem);
			dbitem.CoverageChance = hitchance;
			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"The {FullDescription().Colour(Telnet.Yellow)} bone will now cover the {organ.FullDescription().Colour(Telnet.Yellow)} organ {hitchance.ToString("P3", builder).ColourValue()} of the time.");
		return true;
	}

	private bool BuildingCommandRemoveInside(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which bodypart do you want to remove this bone's presence from?");
			return false;
		}

		var name = command.PopSpeech();
		var part = Body.AllBodyparts.FirstOrDefault(x => x.Name.EqualTo(name));
		if (part == null)
		{
			builder.OutputHandler.Send("There is no such bodypart.");
			return false;
		}

		if (!part.BoneInfo.ContainsKey(this))
		{
			builder.OutputHandler.Send(
				$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart does not contain the {FullDescription().Colour(Telnet.Yellow)} bone.");
			return false;
		}

		part.BoneInfo.Remove(this);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var target =
				FMDB.Context.BodypartInternalInfos.First(x => x.InternalPartId == Id && x.BodypartProtoId == part.Id);
			FMDB.Context.BodypartInternalInfos.Remove(target);
			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"You remove the {FullDescription().Colour(Telnet.Yellow)} bone from inside the {part.FullDescription().Colour(Telnet.Yellow)} bodypart.");
		return true;
	}

	private bool BuildingCommandInside(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which bodypart do you want to add or edit this bone's presence inside?");
			return false;
		}

		var name = command.PopSpeech();
		var part = Body.AllBodyparts.FirstOrDefault(x => x.Name.EqualTo(name));
		if (part == null)
		{
			builder.OutputHandler.Send("There is no such bodypart.");
			return false;
		}

		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"What should be the percentage hit chance of this bone when the parent bodypart is hit?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(builder.Account.Culture, out var hitchance) || hitchance <= 0.0)
		{
			builder.OutputHandler.Send("The hit chance must be a percentage greater than 0.");
			return false;
		}

		var group = default(string);
		if (!command.IsFinished)
		{
			group = command.PopSpeech();
		}

		Gameworld.SaveManager.Flush();
		if (part.BoneInfo.ContainsKey(this))
		{
			var primary = part.BoneInfo[this].IsPrimaryInternalLocation;
			part.BoneInfo[this] = new BodypartInternalInfo(hitchance, primary, group);
			using (new FMDB())
			{
				var dbtarget = FMDB.Context
				                   .BodypartProtos
				                   .Include(x => x.BodypartInternalInfosBodypartProto)
				                   .First(x => x.Id == part.Id);
				var dbinternal = dbtarget.BodypartInternalInfosBodypartProto.First(x => x.InternalPartId == Id);
				dbinternal.HitChance = hitchance * 100.0;
				dbinternal.ProximityGroup = group;
				dbinternal.IsPrimaryOrganLocation = primary;
				FMDB.Context.SaveChanges();
			}

			builder.OutputHandler.Send(
				$"You update the {FullDescription().Colour(Telnet.Yellow)} bone to be inside the {part.FullDescription().Colour(Telnet.Yellow)} bodypart with a hit chance of {(hitchance).ToString("P3", builder).ColourValue()}, proximity group of {group?.ColourValue() ?? "None".Colour(Telnet.Red)}.");
			return true;
		}

		part.BoneInfo[this] = new BodypartInternalInfo(hitchance, false, group);
		using (new FMDB())
		{
			var dbinternal = new BodypartInternalInfos();
			FMDB.Context.BodypartInternalInfos.Add(dbinternal);
			dbinternal.BodypartProtoId = part.Id;
			dbinternal.InternalPartId = Id;
			dbinternal.HitChance = hitchance * 100.0;
			dbinternal.ProximityGroup = group;
			dbinternal.IsPrimaryOrganLocation = false;
			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"You set the {FullDescription().Colour(Telnet.Yellow)} bone to be inside the {part.FullDescription().Colour(Telnet.Yellow)} bodypart with a hit chance of {(hitchance).ToString("P3", builder).ColourValue()}, proximity group of {group?.ColourValue() ?? "None".Colour(Telnet.Red)}.");
		return true;
	}

	private bool BuildingCommandPrimary(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which bodypart do you want to set as the primary location for this bone?");
			return false;
		}

		var name = command.PopSpeech();
		var part = Body.AllBodyparts.FirstOrDefault(x => x.Name.EqualTo(name));
		if (part == null)
		{
			builder.OutputHandler.Send("There is no such bodypart.");
			return false;
		}

		if (!part.BoneInfo.ContainsKey(this))
		{
			builder.OutputHandler.Send(
				$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart does not contain this bone.");
			return false;
		}

		if (part.BoneInfo[this].IsPrimaryInternalLocation)
		{
			builder.OutputHandler.Send(
				$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart is already the primary location for this bone.");
			return false;
		}

		foreach (var bone in part.BoneInfo)
		{
			if (bone.Key != this)
			{
				part.BoneInfo[bone.Key] =
					new BodypartInternalInfo(bone.Value.HitChance, false, bone.Value.ProximityGroup);
			}
			else
			{
				part.BoneInfo[bone.Key] =
					new BodypartInternalInfo(bone.Value.HitChance, true, bone.Value.ProximityGroup);
			}
		}

		using (new FMDB())
		{
			foreach (var dbitem in FMDB.Context.BodypartInternalInfos
			                           .Where(x => x.BodypartProtoId == part.Id && x.InternalPart.IsOrgan == 0)
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
			$"The {part.FullDescription().Colour(Telnet.Yellow)} bodypart is now the primary location for this bone.");
		return true;
	}

	#endregion
}