using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class CombatMessageManager : ICombatMessageManager
{
	private readonly List<ICombatMessage> _combatMessages = new();

	public CombatMessageManager(IFuturemud gameworld)
	{
		LoadCombatMessages(gameworld);
	}

	public IEnumerable<ICombatMessage> CombatMessages => _combatMessages;

	public void AddCombatMessage(ICombatMessage message)
	{
		_combatMessages.Add(message);
	}

	public void RemoveCombatMessage(ICombatMessage message)
	{
		_combatMessages.Remove(message);
	}

	private ICombatMessage GetCombatMessageFor(ICharacter character, IPerceiver target, IGameItem weapon,
		IWeaponAttack attack, BuiltInCombatMoveType type, Outcome outcome, IBodypart bodypart)
	{
		return
			_combatMessages.Where(x => x.Applies(character, target, weapon, attack, type, outcome, bodypart))
			               .OrderByDescending(x => x.Priority)
			               .FirstOrDefault(x => Constants.Random.NextDouble() <= x.Chance);
	}

	public string GetMessageFor(ICharacter character, IPerceiver target, IGameItem weapon, IWeaponAttack attack,
		BuiltInCombatMoveType type, Outcome outcome, IBodypart bodypart)
	{
		return GetCombatMessageFor(character, target, weapon, attack, type, outcome, bodypart)?.Message ??
		       "Error - missing combat message";
	}

	public string GetFailMessageFor(ICharacter character, IPerceiver target, IGameItem weapon, IWeaponAttack attack,
		BuiltInCombatMoveType type, Outcome outcome, IBodypart bodypart)
	{
		return GetCombatMessageFor(character, target, weapon, attack, type, outcome, bodypart)?.FailMessage ??
		       "Error - missing combat message";
	}

	private void LoadCombatMessages(IFuturemud gameworld)
	{
		using (new FMDB())
		{
			var messages = FMDB.Context.CombatMessages.ToList();
			foreach (var message in messages)
			{
				_combatMessages.Add(new CombatMessage(message, gameworld));
			}
		}

		// Building Sanity Check
		var missingMessages = new List<Tuple<BuiltInCombatMoveType, Outcome>>();
		var outcomeValues =
			Enum.GetValues(typeof(Outcome))
			    .Cast<Outcome>()
			    .Where(x => x != Outcome.None && x != Outcome.NotTested)
			    .ToList();
		foreach (var typeValue in Enum.GetValues(typeof(BuiltInCombatMoveType)).Cast<BuiltInCombatMoveType>())
		{
			if (_combatMessages.Any(x =>
				    x.Type == typeValue && (!x.Outcome.HasValue || x.Outcome == Outcome.None) &&
				    (bool?)x.Prog?.Execute(null, null, null, 0, "") != false))
			{
				continue;
			}

			if (gameworld.WeaponAttacks.Where(x => x.MoveType == typeValue).All(x =>
				    outcomeValues.All(y => GetCombatMessageFor(null, null, null, x, typeValue, y, null) != null)))
			{
				continue;
			}

			missingMessages.AddRange(from outcomeValue in outcomeValues
			                         where
				                         !_combatMessages.Any(
					                         x =>
						                         x.Type == typeValue &&
						                         (!x.Outcome.HasValue || x.Outcome == outcomeValue ||
						                          x.Outcome == Outcome.None) &&
						                         (bool?)x.Prog?.Execute(null, null, null, 0, "") != false)
			                         select Tuple.Create(typeValue, outcomeValue));
		}

		if (missingMessages.Any())
		{
			Console.WriteLine("Critical Error - the following combat messages are missing a default value:");
			foreach (var message in missingMessages)
			{
				Console.WriteLine($"\t{message.Item1.Describe()} ({message.Item1}) - {message.Item2.Describe()}");
			}
		}
	}
}