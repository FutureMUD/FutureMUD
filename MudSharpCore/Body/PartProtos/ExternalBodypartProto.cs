using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.PartProtos;

public abstract class ExternalBodypartProto : BodypartPrototype, IExternalBodypart
{
	protected ExternalBodypartProto(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	protected ExternalBodypartProto(BodypartPrototype rhs, string newName) : base(rhs, newName)
	{
	}

	public int DisplayOrder { get; protected set; }

	public override bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "upstream":
				return BuildingCommandUpstream(builder, command);
			default:
				return base.BuildingCommand(builder,
					new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandUpstream(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"You must specify a bodypart to be upstream of this one, or use the keyword 'none' to set none.");
			return false;
		}

		var name = command.PopSpeech().ToLowerInvariant();
		if (name.EqualTo("none"))
		{
			UpstreamConnection = null;
			using (new FMDB())
			{
				var dbitem = FMDB.Context.BodypartProtos.Find(Id);
				FMDB.Context.BodypartProtoBodypartProtoUpstream.RemoveRange(dbitem
					.BodypartProtoBodypartProtoUpstreamParentNavigation);
				FMDB.Context.SaveChanges();
			}

			builder.OutputHandler.Send(
				"This bodypart no longer has any upstream connections. It will be treated as a root part.");
			return true;
		}

		var part = Body.AllExternalBodyparts.FirstOrDefault(x => x.Name.EqualTo(name));
		if (part == null)
		{
			builder.OutputHandler.Send($"The {Body.Name.Colour(Telnet.Yellow)} body has no such bodypart.");
			return false;
		}

		if (part.CountsAs(this) || CountsAs(part))
		{
			builder.OutputHandler.Send("You cannot set a part to be downstream of a part it counts as.");
			return false;
		}

		if (part.DownstreamOfPart(this))
		{
			builder.OutputHandler.Send(
				"You cannot set a part to be downstream of a part that would create a loop of downstreamness.");
			return false;
		}

		UpstreamConnection = part;
		var oldLimb = Body.Limbs.FirstOrDefault(x => x.Parts.Contains(this));
		ILimb newLimb;
		var newLimbs = Body.Limbs.Where(x => DownstreamOfPart(x.RootBodypart)).ToList();
		if (newLimbs.Count > 1)
		{
			var bodyRootLimb = newLimbs.FirstOrDefault(x => x.RootBodypart.UpstreamConnection == null);
			if (bodyRootLimb == null)
			{
				newLimb = newLimbs.First(x => x.LimbType != LimbType.Torso);
			}
			else
			{
				newLimb = newLimbs.Except(bodyRootLimb).First();
			}
		}
		else
		{
			newLimb = newLimbs.FirstOrDefault();
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodypartProtos.Find(Id);
			FMDB.Context.BodypartProtoBodypartProtoUpstream.RemoveRange(dbitem
				.BodypartProtoBodypartProtoUpstreamParentNavigation);
			dbitem.BodypartProtoBodypartProtoUpstreamParentNavigation.Add(new BodypartProtoBodypartProtoUpstream
				{ ChildNavigation = dbitem, Parent = part.Id });

			if (oldLimb != newLimb)
			{
				oldLimb?.RemoveBodypart(this);
				newLimb?.AddBodypart(this);
				FMDB.Context.LimbsBodypartProto.RemoveRange(dbitem.LimbsBodypartProto);
				if (newLimb != null)
				{
					dbitem.Limbs.Add(FMDB.Context.Limbs.Find(newLimb.Id));
				}
			}

			FMDB.Context.SaveChanges();
		}

		builder.OutputHandler.Send(
			$"The {FullDescription().Colour(Telnet.Yellow)} bodypart is now downstream of part {part.FullDescription().Colour(Telnet.Yellow)}.");
		return true;
	}

	public override string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder(base.ShowToBuilder(builder));
		sb.AppendLine();
		sb.AppendLine("Contained Organs:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from part in OrganInfo
			select new List<string>
			{
				part.Key.FullDescription(),
				(part.Value.HitChance/100.0).ToStringP2Colour(builder),
				part.Value.ProximityGroup ?? "",
				part.Value.IsPrimaryInternalLocation.ToColouredString()
			},
			new List<string>
			{
				"Organ",
				"Hit %",
				"Group",
				"Primary Location?"
			},
			builder,
			Telnet.Yellow));
		sb.AppendLine();
		sb.AppendLine("Contained Bones:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from bone in BoneInfo
			select new List<string>
			{
				bone.Key.FullDescription(),
				(bone.Value.HitChance/100.0).ToStringP2Colour(builder),
			},
			new List<string>
			{
				"Bone",
				"Cover %",
			},
			builder,
			Telnet.Yellow));
		return sb.ToString();
	}
}