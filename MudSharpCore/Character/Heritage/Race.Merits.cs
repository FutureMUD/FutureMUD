#nullable enable

using MudSharp.Database;
using MudSharp.RPG.Merits;

namespace MudSharp.Character.Heritage;

public partial class Race
{
	private readonly List<long> _intrinsicMeritIds = new();

	public IEnumerable<ICharacterMerit> DirectIntrinsicMerits => _intrinsicMeritIds
		.Select(x => Gameworld.Merits.Get(x))
		.OfType<ICharacterMerit>()
		.Where(x => x.MeritScope == MeritScope.Body);

	public IEnumerable<ICharacterMerit> IntrinsicMerits => DirectIntrinsicMerits
		.SelectMany(x => x is MudSharp.RPG.Merits.CharacterMerits.ComboMerit combo
			? new[] { x }.Concat(combo.CharacterMerits.Where(y => y.MeritScope == MeritScope.Body))
			: new[] { x })
		.Concat(ParentRace?.IntrinsicMerits ?? Enumerable.Empty<ICharacterMerit>())
		.DistinctBy(x => x.Id);

	private void AddIntrinsicMeritLinks(Models.Race row)
	{
		foreach (var id in _intrinsicMeritIds.Distinct())
			row.RacesMerits.Add(new Models.RacesMerits { Race = row, MeritId = id });
	}

	private void SaveIntrinsicMeritLinks(Models.Race row)
	{
		FMDB.Context.Entry(row).Collection(x => x.RacesMerits).Load();
		FMDB.Context.RacesMerits.RemoveRange(row.RacesMerits.Where(x => !_intrinsicMeritIds.Contains(x.MeritId)).ToArray());
		foreach (var id in _intrinsicMeritIds.Where(x => row.RacesMerits.All(y => y.MeritId != x)).Distinct())
			row.RacesMerits.Add(new Models.RacesMerits { Race = row, MeritId = id });
	}

	private bool BuildingCommandIntrinsicMerit(ICharacter actor, StringStack command)
	{
		var text = command.SafeRemainingArgument;
		var merit = Gameworld.Merits.GetByIdOrName(text) as ICharacterMerit;
		if (merit is null || merit.MeritScope != MeritScope.Body)
		{
			actor.OutputHandler.Send("Choose a body-scoped character merit. Use the merit's scope command to configure its scope.");
			return false;
		}
		if (_intrinsicMeritIds.Remove(merit.Id))
			actor.OutputHandler.Send($"This race no longer directly grants {merit.Name.ColourName()}. Any parent-race grant remains in effect.");
		else
		{
			_intrinsicMeritIds.Add(merit.Id);
			actor.OutputHandler.Send($"This race now intrinsically grants {merit.Name.ColourName()} to its bodies.");
		}
		Changed = true;
		return true;
	}
}
