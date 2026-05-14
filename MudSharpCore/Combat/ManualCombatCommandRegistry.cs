using MudSharp.Character;
using MudSharp.Accounts;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat;

public static class ManualCombatCommandRegistry
{
	private static readonly HashSet<string> _registeredWords = new(StringComparer.InvariantCultureIgnoreCase);

	private static IEnumerable<ICharacterCommandTree> PlayerTrees
	{
		get
		{
			yield return PlayerCommandTree.Instance;
			yield return GuideCommandTree.Instance;
			yield return AdminCommandTree.JuniorAdminCommandTree;
			yield return AdminCommandTree.StandardAdminCommandTree;
			yield return AdminCommandTree.SeniorAdminCommandTree;
			yield return AdminCommandTree.HighAdminCommandTree;
			yield return FounderCommandTree.Instance;
		}
	}

	private static IEnumerable<ICharacterCommandTree> AllTrees => PlayerTrees.Append(NPCCommandTree.Instance);

	public static bool IsValidCommandWord(string text)
	{
		return !string.IsNullOrWhiteSpace(text) &&
		       text.All(char.IsLetter);
	}

	public static bool HasReservedCommandCollision(string verb)
	{
		return AllTrees.Any(x => x.Commands.TCommands.ContainsKey(verb) && !_registeredWords.Contains(verb));
	}

	public static void Rebuild(IFuturemud gameworld)
	{
		foreach (var tree in AllTrees)
		{
			tree.Commands.Remove(_registeredWords, null);
		}
		_registeredWords.Clear();

		var claimedWords = new Dictionary<string, IManualCombatCommand>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var binding in gameworld.ManualCombatCommands.OrderBy(x => x.Id))
		{
			if (!BindingHasValidTarget(binding))
			{
				ConsoleUtilities.WriteLine(
					$"#9Manual combat command {binding.Name} (#{binding.Id:N0}) was not registered because it does not have a valid target action.#0");
				continue;
			}

			if (!binding.PlayerUsable && !binding.NpcUsable)
			{
				continue;
			}

			var commandWords = binding.CommandWords
			                          .Select(x => x.ToLowerInvariant())
			                          .Where(IsValidCommandWord)
			                          .Distinct(StringComparer.InvariantCultureIgnoreCase)
			                          .ToList();
			if (!commandWords.Contains(binding.PrimaryVerb, StringComparer.InvariantCultureIgnoreCase))
			{
				ConsoleUtilities.WriteLine(
					$"#9Manual combat command {binding.Name} (#{binding.Id:N0}) was not registered because its primary verb is invalid.#0");
				continue;
			}

			if (claimedWords.TryGetValue(binding.PrimaryVerb, out var existingPrimary))
			{
				ConsoleUtilities.WriteLine(
					$"#9Manual combat command {binding.Name} (#{binding.Id:N0}) primary verb {binding.PrimaryVerb} duplicates {existingPrimary.Name} (#{existingPrimary.Id:N0}); skipping it.#0");
				continue;
			}

			if (HasReservedCommandCollision(binding.PrimaryVerb))
			{
				ConsoleUtilities.WriteLine(
					$"#9Manual combat command {binding.Name} (#{binding.Id:N0}) primary verb {binding.PrimaryVerb} collides with an existing command; skipping it.#0");
				continue;
			}

			var validWords = new List<string> { binding.PrimaryVerb };
			claimedWords[binding.PrimaryVerb] = binding;
			foreach (var word in commandWords.Where(x => !x.EqualTo(binding.PrimaryVerb)))
			{
				if (claimedWords.TryGetValue(word, out var existing))
				{
					ConsoleUtilities.WriteLine(
						$"#9Manual combat command alias {word} for {binding.Name} (#{binding.Id:N0}) duplicates {existing.Name} (#{existing.Id:N0}); skipping that alias.#0");
					continue;
				}

				if (HasReservedCommandCollision(word))
				{
					ConsoleUtilities.WriteLine(
						$"#9Manual combat command alias {word} for {binding.Name} (#{binding.Id:N0}) collides with an existing command; skipping that alias.#0");
					continue;
				}

				validWords.Add(word);
				claimedWords[word] = binding;
			}

			var dynamicCommand = new Command<ICharacter>(
				CombatModule.ManualCombatGeneric,
				CharacterState.Able,
				PermissionLevel.Any,
				binding.PrimaryVerb,
				CommandDisplayOptions.DisplayCommandWords,
				new DelayBlock("general", "aim", "You must first stop {0} before you can do that."),
				noMovementCommand: true,
				moduleName: "Combat");

			if (binding.PlayerUsable)
			{
				foreach (var tree in PlayerTrees)
				{
					tree.Commands.Add(validWords, dynamicCommand);
				}
			}

			if (binding.NpcUsable)
			{
				NPCCommandTree.Instance.Commands.Add(validWords, dynamicCommand);
			}

			foreach (var word in validWords)
			{
				_registeredWords.Add(word);
			}
		}
	}

	private static bool BindingHasValidTarget(IManualCombatCommand binding)
	{
		return binding.ActionKind switch
		{
			ManualCombatActionKind.WeaponAttack => binding.WeaponAttack is not null,
			ManualCombatActionKind.AuxiliaryAction => binding.AuxiliaryAction is not null,
			_ => false
		};
	}
}
