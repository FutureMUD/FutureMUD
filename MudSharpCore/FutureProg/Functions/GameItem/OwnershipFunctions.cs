#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal static class OwnershipFunctionHelpers
{
	public static readonly ProgVariableTypes OwnerVariableType = ProgVariableTypes.Character | ProgVariableTypes.Clan;

	public static bool IsPropertyTrusted(IGameItem item, ICharacter character, bool includeClanPrivileges)
	{
		var owner = item.Owner;
		if (owner is null)
		{
			return true;
		}

		if (item.IsOwnedBy(character))
		{
			return true;
		}

		if (owner is ICharacter ownerCharacter)
		{
			return character.IsAlly(ownerCharacter);
		}

		if (!includeClanPrivileges || owner is not IClan ownerClan)
		{
			return false;
		}

		return character.ClanMemberships.Any(x =>
			!x.IsArchivedMembership &&
			x.Clan == ownerClan &&
			(x.NetPrivileges.HasFlag(ClanPrivilegeType.UseClanProperty) ||
			 x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)));
	}

	public static IEnumerable<IGameItem> EnumerateDeepOwnedItems(IGameItem root)
	{
		var pending = new Stack<IGameItem>();
		var seen = new HashSet<IGameItem>();
		pending.Push(root);

		while (pending.Count > 0)
		{
			var item = pending.Pop();
			if (!seen.Add(item))
			{
				continue;
			}

			yield return item;

			foreach (var child in item.GetItemTypes<IContainer>().SelectMany(x => x.Contents))
			{
				pending.Push(child);
			}

			foreach (var child in item.GetItemTypes<IBelt>().SelectMany(x => x.ConnectedItems).Select(x => x.Parent))
			{
				pending.Push(child);
			}

			foreach (var child in item.GetItemTypes<ISheath>().Select(x => x.Content?.Parent).Where(x => x is not null))
			{
				pending.Push(child!);
			}
		}
	}
}

internal class ItemOwnershipQueryFunction : BuiltInFunction
{
	private readonly OwnershipQueryMode _mode;

	private enum OwnershipQueryMode
	{
		IsOwner,
		IsPropertyTrusted,
		IsPropertyTrustedOrClan
	}

	private ItemOwnershipQueryFunction(IList<IFunction> parameters, OwnershipQueryMode mode)
		: base(parameters)
	{
		_mode = mode;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item ||
		    ParameterFunctions[1].Result?.GetObject is not ICharacter character)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		Result = _mode switch
		{
			OwnershipQueryMode.IsOwner => new BooleanVariable(item.IsOwnedBy(character)),
			OwnershipQueryMode.IsPropertyTrusted =>
				new BooleanVariable(OwnershipFunctionHelpers.IsPropertyTrusted(item, character, false)),
			OwnershipQueryMode.IsPropertyTrustedOrClan =>
				new BooleanVariable(OwnershipFunctionHelpers.IsPropertyTrusted(item, character, true)),
			_ => new BooleanVariable(false)
		};

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"isowner",
			[ProgVariableTypes.Item, ProgVariableTypes.Character],
			(pars, gameworld) => new ItemOwnershipQueryFunction(pars, OwnershipQueryMode.IsOwner),
			["item", "character"],
			["The item whose ownership you want to check", "The character whose ownership is being tested"],
			"Returns true if the specified character is the registered owner of the item.",
			"Items",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ispropertytrusted",
			[ProgVariableTypes.Item, ProgVariableTypes.Character],
			(pars, gameworld) => new ItemOwnershipQueryFunction(pars, OwnershipQueryMode.IsPropertyTrusted),
			["item", "character"],
			["The item whose trust state you want to check", "The character attempting to use the item"],
			"Returns true if the item is unowned, directly owned by the character, or owned by one of the character's allies.",
			"Items",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ispropertytrustedorclan",
			[ProgVariableTypes.Item, ProgVariableTypes.Character],
			(pars, gameworld) => new ItemOwnershipQueryFunction(pars, OwnershipQueryMode.IsPropertyTrustedOrClan),
			["item", "character"],
			["The item whose trust state you want to check", "The character attempting to use the item"],
			"Returns true if the item is property-trusted for the character or is clan-owned by a clan where the character has Use Clan Property or Can Manage Clan Property privileges.",
			"Items",
			ProgVariableTypes.Boolean
		));
	}
}

internal class ItemOwnershipMutationFunction : BuiltInFunction
{
	private readonly OwnershipMutationMode _mode;

	private enum OwnershipMutationMode
	{
		SetOwnership,
		DeepSetOwnership,
		ClearOwnership
	}

	private ItemOwnershipMutationFunction(IList<IFunction> parameters, OwnershipMutationMode mode)
		: base(parameters)
	{
		_mode = mode;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		switch (_mode)
		{
			case OwnershipMutationMode.ClearOwnership:
				item.ClearOwner();
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			case OwnershipMutationMode.SetOwnership:
			case OwnershipMutationMode.DeepSetOwnership:
				if (ParameterFunctions[1].Result?.GetObject is not IFrameworkItem owner)
				{
					Result = new BooleanVariable(false);
					return StatementResult.Normal;
				}

				var items = _mode == OwnershipMutationMode.SetOwnership
					? [item]
					: OwnershipFunctionHelpers.EnumerateDeepOwnedItems(item).ToList();
				foreach (var target in items)
				{
					target.SetOwner(owner);
				}

				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			default:
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
		}
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setownership",
			[ProgVariableTypes.Item, ProgVariableTypes.Character],
			(pars, gameworld) => new ItemOwnershipMutationFunction(pars, OwnershipMutationMode.SetOwnership),
			["item", "character"],
			["The item whose ownership should be changed", "The character who should own the item"],
			"Sets the registered owner of the item to the specified character.",
			"Items",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setownership",
			[ProgVariableTypes.Item, ProgVariableTypes.Clan],
			(pars, gameworld) => new ItemOwnershipMutationFunction(pars, OwnershipMutationMode.SetOwnership),
			["item", "clan"],
			["The item whose ownership should be changed", "The clan that should own the item"],
			"Sets the registered owner of the item to the specified clan.",
			"Items",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"deepsetownership",
			[ProgVariableTypes.Item, ProgVariableTypes.Character],
			(pars, gameworld) => new ItemOwnershipMutationFunction(pars, OwnershipMutationMode.DeepSetOwnership),
			["item", "character"],
			["The root item whose ownership tree should be changed", "The character who should own the selected items"],
			"Sets the registered owner of the item, its nested contents, sheath contents, and belted attachments to the specified character.",
			"Items",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"deepsetownership",
			[ProgVariableTypes.Item, ProgVariableTypes.Clan],
			(pars, gameworld) => new ItemOwnershipMutationFunction(pars, OwnershipMutationMode.DeepSetOwnership),
			["item", "clan"],
			["The root item whose ownership tree should be changed", "The clan that should own the selected items"],
			"Sets the registered owner of the item, its nested contents, sheath contents, and belted attachments to the specified clan.",
			"Items",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"clearownership",
			[ProgVariableTypes.Item],
			(pars, gameworld) => new ItemOwnershipMutationFunction(pars, OwnershipMutationMode.ClearOwnership),
			["item"],
			["The item whose ownership should be cleared"],
			"Clears the registered owner of the specified item.",
			"Items",
			ProgVariableTypes.Boolean
		));
	}
}
