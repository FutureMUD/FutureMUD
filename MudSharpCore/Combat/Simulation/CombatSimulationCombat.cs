using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Combat.Simulation;

internal sealed class CombatSimulationCombat(IFuturemud gameworld) : SimpleMeleeCombat(gameworld), ICombatTargetingPolicy
{
	private readonly Dictionary<IPerceiver, string> _teams = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IPerceiver, CombatStrategyMode> _departureModes = new(ReferenceEqualityComparer.Instance);

	public void JoinCombat(IPerceiver character, string team)
	{
		_teams[character] = team;
		base.JoinCombat(character, Difficulty.Automatic);
	}

	public string? TeamFor(IPerceiver character)
	{
		return _teams.GetValueOrDefault(character);
	}

	public CombatStrategyMode? DepartureModeFor(IPerceiver character)
	{
		return _departureModes.GetValueOrDefault(character);
	}

	public override bool LeaveCombat(IPerceiver character)
	{
		if (character is not null)
		{
			_departureModes.TryAdd(character, character.CombatStrategyMode);
		}

		return base.LeaveCombat(character);
	}

	public IPerceiver? AcquireTargetFor(IPerceiver combatant)
	{
		if (!_teams.TryGetValue(combatant, out var team))
		{
			return null;
		}

		return Combatants
			.Where(x => !ReferenceEquals(x, combatant))
			.Where(x => !_teams.TryGetValue(x, out var otherTeam) ||
			            !string.Equals(team, otherTeam, StringComparison.InvariantCultureIgnoreCase))
			.Where(x => x is not ICharacter character || character.State.IsAble())
			.Where(combatant.CanEngage)
			.OrderBy(x => x.Id)
			.FirstOrDefault();
	}
}
