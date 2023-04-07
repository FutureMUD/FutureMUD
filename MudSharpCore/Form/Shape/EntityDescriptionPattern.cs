using System.Text;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Form.Shape;

public class EntityDescriptionPattern : SaveableItem, IEntityDescriptionPattern
{
	public EntityDescriptionPattern(MudSharp.Models.EntityDescriptionPattern pattern, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = pattern.Id;
		Type = (EntityDescriptionType)pattern.Type;
		Pattern = pattern.Pattern;
		ApplicabilityProg = pattern.ApplicabilityProgId.HasValue
			? Gameworld.FutureProgs.Get(pattern.ApplicabilityProgId.Value)
			: null;
		RelativeWeight = pattern.RelativeWeight;
	}

	public override string FrameworkItemType => "EntityDescriptionPattern";

	#region ISaveable Members

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.EntityDescriptionPatterns.Find(Id);
			dbitem.ApplicabilityProgId = ApplicabilityProg?.Id;
			dbitem.Type = (int)Type;
			dbitem.Pattern = Pattern;
			dbitem.RelativeWeight = RelativeWeight;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	#region IEntityDescriptionPattern Members

	public EntityDescriptionType Type { get; protected set; }

	public IFutureProg ApplicabilityProg { get; protected set; }

	public bool IsValidSelection(ICharacterTemplate template)
	{
		return ApplicabilityProg?.Execute<bool?>(template) != false;
	}

	public bool IsValidSelection(ICharacter character)
	{
		return ApplicabilityProg?.Execute<bool?>(character) != false;
	}

	public string Pattern { get; protected set; }

	public string Show(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLineFormat("{0}", "Description Pattern".Colour(Telnet.Cyan));
		sb.AppendLine();
		sb.AppendLineFormat("Id: {0:N0} Type: {1}", Id, Type.Describe().Colour(Telnet.Green));
		sb.AppendLineFormat("Prog: {0}",
			ApplicabilityProg == null
				? "None".Colour(Telnet.Red)
				: string.Format("{0} (#{1:N0})".FluentTagMXP("send",
						$"href='show futureprog {ApplicabilityProg.Id}'"), ApplicabilityProg.FunctionName,
					ApplicabilityProg.Id));
		sb.AppendLine("Pattern: ");
		sb.AppendLine();
		sb.AppendLine(Pattern.Wrap(voyeur.InnerLineFormatLength, "\t"));

		return sb.ToString();
	}

	public int RelativeWeight { get; protected set; }

	#endregion

	#region Overrides of FrameworkItem

	public override string ToString()
	{
		return Type == EntityDescriptionType.FullDescription
			? $"FDesc Pattern #{Id:N0} - {ApplicabilityProg?.FunctionName ?? "No Prog"}"
			: $"SDesc Pattern #{Id:N0} - {Pattern} - {ApplicabilityProg?.FunctionName ?? "No Prog"}";
	}

	#endregion
}