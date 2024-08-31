using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Body.Grouping;

/// <summary>
///     Describes a group of IBodyparts based on their identity, i.e. that they share the same Prototype
/// </summary>
public class BodypartGroupIDDescriber : BodypartGroupDescriber
{
	

	/// <summary>
	///     Contains the list of Prototypes and whether or not they are mandatory
	/// </summary>
	protected readonly Dictionary<IBodypart, bool> Prototypes = new();

	public BodypartGroupIDDescriber(MudSharp.Models.BodypartGroupDescriber describer, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = describer.Id;
		DescribedAs = describer.DescribedAs;
		Comment = describer.Comment;
	}

	private BodypartGroupIDDescriber(BodypartGroupIDDescriber rhs)
	{
		Gameworld = rhs.Gameworld;
		DescribedAs = rhs.DescribedAs;
		Comment = rhs.Comment;
		foreach (var item in rhs.Prototypes)
		{
			Prototypes[item.Key] = item.Value;
		}
		DoDatabaseInsert();
	}

	public BodypartGroupIDDescriber(IFuturemud gameworld, string describedAs, IBodyPrototype body)
	{
		Gameworld = gameworld;
		DescribedAs = describedAs;
		Comment = string.Empty;
		BodyPrototype = body;
		DoDatabaseInsert();
	}

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.BodypartGroupDescriber
			{
				Comment = Comment,
				DescribedAs = DescribedAs,
				Type = "bodypart",
				BodyProtoId = BodyPrototype.Id,
			};
			FMDB.Context.BodypartGroupDescribers.Add(dbitem);
			foreach (var item in Prototypes)
			{
				dbitem.BodypartGroupDescribersBodypartProtos.Add(new BodypartGroupDescribersBodypartProtos
				{
					BodypartGroupDescriber = dbitem,
					BodypartProtoId = item.Key.Id,
					Mandatory = item.Value
				});
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override IBodypartGroupDescriber Clone()
	{
		return new BodypartGroupIDDescriber(this);
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.BodypartGroupDescribers.Find(Id);
		dbitem.Comment = Comment;
		dbitem.DescribedAs = DescribedAs;
		dbitem.BodyProtoId = BodyPrototype.Id;
		FMDB.Context.BodypartGroupDescribersBodypartProtos.RemoveRange(dbitem.BodypartGroupDescribersBodypartProtos);
		foreach (var item in Prototypes)
		{
			dbitem.BodypartGroupDescribersBodypartProtos.Add(new Models.BodypartGroupDescribersBodypartProtos()
			{
				BodypartGroupDescriber = dbitem,
				Mandatory = item.Value,
				BodypartProtoId = item.Key.Id
			});
		}
		Changed = false;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Bodypart Group Describer #{Id.ToStringN0(actor)}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {"Direct".ColourValue()}");
		sb.AppendLine($"Body: {BodyPrototype.Name.ColourValue()}");
		sb.AppendLine($"Described As: {DescribedAs.ColourValue()}");
		sb.AppendLine($"Comment: {Comment.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Parts:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in Prototypes
			select new List<string>
			{
				item.Key.Id.ToStringN0(actor),
				item.Key.FullDescription(),
				item.Value.ToColouredString()
			},
			new List<string>
			{
				"Id",
				"Name",
				"Mandatory"
			},
			actor,
			Telnet.Yellow
		));
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3part <which>#0 - toggles the inclusion of a part
	#3part <which> mandatory#0 - sets a part to be a mandatory part
	#3part <which> optional#0 - sets a part to be an optional part";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "part":
				return BuildingCommandPart(actor, ss);
			default:
				return base.BuildingCommand(actor, ss.GetUndo());
		}
		
	}

	private bool BuildingCommandPart(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart do you want to toggle or edit?");
			return false;
		}

		var text = ss.PopSpeech();
		var part = 
			BodyPrototype.AllBodyparts.GetByIdOrName(text) ??
			Gameworld.BodypartPrototypes.GetByIdOrName(text);
		if (part is null)
		{
			actor.OutputHandler.Send($"The text {text.ColourCommand()} does not represent a valid bodypart.");
			return false;
		}

		if (!BodyPrototype.AllBodyparts.Contains(part))
		{
			actor.OutputHandler.Send($"The bodypart {part.FullDescription().ColourValue()} (#{part.Id.ToStringN0(actor)}) is not for the body {BodyPrototype.Name.ColourValue()}.");
			return false;
		}

		if (ss.IsFinished)
		{
			Changed = true;
			if (Prototypes.ContainsKey(part))
			{
				Prototypes.Remove(part);
				actor.OutputHandler.Send($"The part {part.FullDescription()} is no longer a part of this bodypart group describer.");
				return true;
			}

			Prototypes[part] = false;
			actor.OutputHandler.Send($"The part {part.FullDescription()} is now a non-mandatory part of this bodypart group describer.");
			return true;
		}

		if (ss.SafeRemainingArgument.EqualTo("mandatory"))
		{
			Prototypes[part] = true;
			Changed = true;
			actor.OutputHandler.Send($"The part {part.FullDescription()} is now a mandatory part of this bodypart group describer.");
			return true;
		}

		if (ss.SafeRemainingArgument.EqualTo("optional"))
		{
			Prototypes[part] = false;
			Changed = true;
			actor.OutputHandler.Send($"The part {part.FullDescription()} is now a non-mandatory part of this bodypart group describer.");
			return true;
		}

		actor.OutputHandler.Send("If you supply an argument after the bodypart name, it must be either #3mandatory#0 or #3optional#0.".SubstituteANSIColour());
		return false;
	}

	#region IBodypartGroupDescriber Members

	public override BodypartGroupResult Match(IEnumerable<IBodypart> parts)
	{
		if (!Prototypes.Any())
		{
			return new BodypartGroupResult(false, 0);
		}

		var matches = parts.Where(part => Prototypes.ContainsKey(part)).ToList();

		if (Prototypes.Where(x => x.Value).Any(x => matches.All(y => y != x.Key)))
		{
			return new BodypartGroupResult(false, 0);
		}

		return new BodypartGroupResult(true, matches.Count, DescribedAs, matches,
			parts.Where(x => !matches.Contains(x)).ToList());
	}

	#endregion

	public override void FinaliseLoad(MudSharp.Models.BodypartGroupDescriber describer, IFuturemud gameworld)
	{
		if (describer.BodyProtoId is not null)
		{
			BodyPrototype = Gameworld.BodyPrototypes.Get(describer.BodyProtoId.Value)!;
		}
		else
		{
			BodyPrototype = gameworld.BodyPrototypes.Get(describer.BodypartGroupDescribersBodyProtos.Single().BodyProtoId)!;
			describer.BodyProtoId = BodyPrototype.Id;
			Changed = true;
		}

		foreach (var part in describer.BodypartGroupDescribersBodypartProtos)
		{
			Prototypes.Add(Gameworld.BodypartPrototypes.Get(part.BodypartProtoId)!, part.Mandatory);
		}
	}
}