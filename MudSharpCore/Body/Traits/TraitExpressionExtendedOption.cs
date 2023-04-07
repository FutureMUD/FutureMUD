using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Framework;
using MudSharp.RPG.Merits;

namespace MudSharp.Body.Traits;

public abstract class TraitExpressionExtendedOption
{
	public string Name { get; set; }
	public abstract double Evaluate(IHaveTraits iht);
}

public class TraitExpressionAlwaysZeroOption : TraitExpressionExtendedOption
{
	public override double Evaluate(IHaveTraits iht)
	{
		return 0.0;
	}
}

public class TraitExpressionMeritOption : TraitExpressionExtendedOption
{
	public IEnumerable<IMerit> _merits;
	public readonly IEnumerable<string> _meritOptions;

	public TraitExpressionMeritOption(string values)
	{
		_meritOptions = values.Split(',');
	}

	public override double Evaluate(IHaveTraits iht)
	{
		if (_merits == null)
		{
			var races = new List<IMerit>();
			var game = Futuremud.Games.First();
			foreach (var option in _meritOptions)
			{
				var merit = long.TryParse(option, out var value)
					? game.Merits.Get(value)
					: game.Merits.GetByName(option);
				if (merit != null)
				{
					races.Add(merit);
				}
			}

			_merits = races;
		}

		if (iht is not IHaveMerits ihm)
		{
			return 0.0;
		}

		if (_merits.Any(x => ihm.Merits.Contains(x) && x.Applies(ihm)))
		{
			return 1.0;
		}

		return 0.0;
	}
}

public class TraitExpressionRaceOption : TraitExpressionExtendedOption
{
	public IEnumerable<IRace> _races;
	public readonly IEnumerable<string> _raceOptions;

	public TraitExpressionRaceOption(string values)
	{
		_raceOptions = values.Split(',');
	}

	public override double Evaluate(IHaveTraits iht)
	{
		if (_races == null)
		{
			var races = new List<IRace>();
			var game = Futuremud.Games.First();
			foreach (var option in _raceOptions)
			{
				var race = long.TryParse(option, out var value) ? game.Races.Get(value) : game.Races.GetByName(option);
				if (race != null)
				{
					races.Add(race);
				}
			}

			_races = races;
		}

		if (iht is not ICharacter ch)
		{
			return 0.0;
		}

		if (_races.Any(x => ch.Race.SameRace(x)))
		{
			return 1.0;
		}

		return 0.0;
	}
}

public class TraitExpressionCultureOption : TraitExpressionExtendedOption
{
	public IEnumerable<ICulture> _cultures;
	public readonly IEnumerable<string> _cultureOptions;

	public TraitExpressionCultureOption(string values)
	{
		_cultureOptions = values.Split(',');
	}

	public override double Evaluate(IHaveTraits iht)
	{
		if (_cultures == null)
		{
			var cultures = new List<ICulture>();
			var game = Futuremud.Games.First();
			foreach (var option in _cultureOptions)
			{
				var culture = long.TryParse(option, out var value)
					? game.Cultures.Get(value)
					: game.Cultures.GetByName(option);
				if (culture != null)
				{
					cultures.Add(culture);
				}
			}

			_cultures = cultures;
		}

		if (iht is not ICharacter ch)
		{
			return 0.0;
		}

		if (_cultures.Any(x => ch.Culture == x))
		{
			return 1.0;
		}

		return 0.0;
	}
}

public class TraitExpressionRoleOption : TraitExpressionExtendedOption
{
	public IEnumerable<IChargenRole> _roles;
	public readonly IEnumerable<string> _roleOptions;
	public IEnumerable<ChargenRoleType> _roleTypes;

	public TraitExpressionRoleOption(string values)
	{
		_roleOptions = values.Split(',');
	}

	public override double Evaluate(IHaveTraits iht)
	{
		if (_roles == null)
		{
			var roles = new List<IChargenRole>();
			var game = Futuremud.Games.First();
			foreach (var option in _roleOptions)
			{
				var roleChoices = game.Roles.Where(x => _roleTypes.Contains(x.RoleType)).ToList();
				var role = long.TryParse(option, out var value)
					? roleChoices.FirstOrDefault(x => x.Id == value)
					: roleChoices.FirstOrDefault(x => x.Name.EqualTo(option)) ??
					  roleChoices.FirstOrDefault(x =>
						  x.Name.StartsWith(option, StringComparison.InvariantCultureIgnoreCase));
				if (role != null)
				{
					roles.Add(role);
				}
			}

			_roles = roles;
		}

		if (iht is not ICharacter ch)
		{
			return 0.0;
		}

		if (_roles.Any(x => ch.Roles.Contains(x)))
		{
			return 1.0;
		}

		return 0.0;
	}
}