using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp.Community;

public class Rank : SaveableItem, IRank
{
	protected Rank()
	{
		Paygrades = new List<IPaygrade>();
	}

	public Rank(MudSharp.Models.Rank rank, IClan clan)
		: this()
	{
		Gameworld = clan.Gameworld;
		_id = rank.Id;
		_name = rank.Name;
		Clan = clan;
		Privileges = (ClanPrivilegeType)rank.Privileges;
		InsigniaGameItem = rank.InsigniaGameItemId != null
			? Gameworld.ItemProtos.Get(rank.InsigniaGameItemId.Value, rank.InsigniaGameItemRevnum ?? 0)
			: null;
		RankPath = rank.RankPath;
		RankNumber = rank.RankNumber;
		FameType = (ClanFameType)rank.FameType;
		foreach (var item in rank.RanksPaygrades.OrderBy(x => x.Order))
		{
			Paygrades.Add(clan.Paygrades.First(x => x.Id == item.PaygradeId));
		}

		foreach (var item in rank.RanksTitles.OrderBy(x => x.Order))
		{
			TitlesAndProgs.Add(Tuple.Create(Gameworld.FutureProgs.Get(item.FutureProgId ?? 0), item.Title));
		}

		foreach (var item in rank.RanksAbbreviations.OrderBy(x => x.Order))
		{
			AbbreviationsAndProgs.Add(Tuple.Create(Gameworld.FutureProgs.Get(item.FutureProgId ?? 0),
				item.Abbreviation));
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Ranks.Find(Id);
			dbitem.Name = Name;
			dbitem.Privileges = (long)Privileges;
			dbitem.RankPath = string.IsNullOrEmpty(RankPath) ? null : RankPath;
			dbitem.RankNumber = RankNumber;
			dbitem.FameType = (int)FameType;
			if (InsigniaGameItem != null)
			{
				dbitem.InsigniaGameItemId = InsigniaGameItem.Id;
				dbitem.InsigniaGameItemRevnum = InsigniaGameItem.RevisionNumber;
			}
			else
			{
				dbitem.InsigniaGameItemId = null;
				dbitem.InsigniaGameItemRevnum = null;
			}

			FMDB.Context.RanksPaygrades.RemoveRange(dbitem.RanksPaygrades);
			var index = 0;
			foreach (var paygrade in Paygrades)
			{
				var pg = new RanksPaygrade
				{
					Order = index++,
					PaygradeId = paygrade.Id
				};
				dbitem.RanksPaygrades.Add(pg);
			}

			FMDB.Context.RanksAbbreviations.RemoveRange(dbitem.RanksAbbreviations);
			var order = 0;
			foreach (var item in AbbreviationsAndProgs)
			{
				var abbreviation = new RanksAbbreviations
				{
					FutureProgId = item.Item1?.Id,
					Abbreviation = item.Item2,
					Order = order++
				};
				dbitem.RanksAbbreviations.Add(abbreviation);
			}

			FMDB.Context.RanksTitles.RemoveRange(dbitem.RanksTitles);
			order = 0;
			foreach (var item in TitlesAndProgs)
			{
				var title = new RanksTitle
				{
					FutureProgId = item.Item1?.Id,
					Title = item.Item2,
					Order = order++
				};
				dbitem.RanksTitles.Add(title);
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Rank";

	#region IRank Members

	public IClan Clan { get; set; }

	public void SetName(string name)
	{
		_name = name;
		Changed = true;
	}

	public string RankPath { get; set; }

	public string Abbreviation(ICharacter character)
	{
		return character != null
			? AbbreviationsAndProgs.First(x => (bool?)x.Item1?.Execute(character) ?? true)
			                       .Item2
			: (AbbreviationsAndProgs.FirstOrDefault(x => x.Item1 == null) ?? AbbreviationsAndProgs.First()).Item2;
	}

	IEnumerable<string> IRank.Abbreviations
	{
		get { return AbbreviationsAndProgs.Select(x => x.Item2); }
	}

	public List<Tuple<IFutureProg, string>> AbbreviationsAndProgs { get; } = new();

	public string Title(ICharacter character)
	{
		return character != null
			? TitlesAndProgs.First(x => (bool?)x.Item1?.Execute(character) ?? true).Item2
			: (TitlesAndProgs.FirstOrDefault(x => x.Item1 == null) ?? TitlesAndProgs.First()).Item2;
	}

	IEnumerable<string> IRank.Titles
	{
		get { return TitlesAndProgs.Select(x => x.Item2); }
	}

	public List<Tuple<IFutureProg, string>> TitlesAndProgs { get; } = new();

	public List<IPaygrade> Paygrades { get; }

	public ClanPrivilegeType Privileges { get; set; }

	public IGameItemProto InsigniaGameItem { get; set; }

	public int RankNumber { get; set; }

	public ClanFameType FameType { get; set; }

	#endregion

	#region IFutureProgVariable Members

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "ranknumber":
				return new NumberVariable(RankNumber);
			case "paygrades":
				return new CollectionVariable(new List<IPaygrade>(Paygrades), ProgVariableTypes.ClanPaygrade);
			case "path":
				return new TextVariable(RankPath);
			default:
				throw new NotSupportedException("Invalid IFutureProgVariable request in Rank.GetProperty");
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.ClanRank;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "ranknumber", ProgVariableTypes.Text },
			{ "path", ProgVariableTypes.Text },
			{ "paygrades", ProgVariableTypes.Collection | ProgVariableTypes.ClanPaygrade }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The Id of the rank" },
			{ "name", "The name of the rank" },
			{ "ranknumber", "The number where the rank appears in the rank list" },
			{ "path", "The path of the rank (may be null)" },
			{ "paygrades", "A list of all of the paygrades associated with this rank" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.ClanRank, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}