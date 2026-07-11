#nullable enable

using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Commands.Helpers;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.GameItem;

internal static class OwnershipFunctionHelpers
{
    public static readonly ProgVariableTypes OwnerVariableType = ProgVariableTypes.Character | ProgVariableTypes.Clan |
                                                               ProgVariableTypes.Shop | ProgVariableTypes.Bank;

    public static bool IsPropertyTrusted(IGameItem item, ICharacter character, bool includeClanPrivileges)
    {
        IFrameworkItem? owner = item.Owner;
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
        Stack<IGameItem> pending = new();
        HashSet<IGameItem> seen = new();
        pending.Push(root);

        while (pending.Count > 0)
        {
            IGameItem item = pending.Pop();
            if (!seen.Add(item))
            {
                continue;
            }

            yield return item;

            foreach (IGameItem? child in item.GetItemTypes<IContainer>().SelectMany(x => x.Contents))
            {
                pending.Push(child);
            }

            foreach (IGameItem? child in item.GetItemTypes<IBelt>().SelectMany(x => x.ConnectedItems).Select(x => x.Parent))
            {
                pending.Push(child);
            }

            foreach (IGameItem? child in item.GetItemTypes<ISheath>().Select(x => x.Content?.Parent).Where(x => x is not null))
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
            ParameterFunctions[1].Result?.GetObject is not IFrameworkItem queriedOwner)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        Result = _mode switch
        {
            OwnershipQueryMode.IsOwner => new BooleanVariable(item.IsOwnedBy(queriedOwner)),
            OwnershipQueryMode.IsPropertyTrusted =>
                new BooleanVariable(queriedOwner is ICharacter character &&
                                    OwnershipFunctionHelpers.IsPropertyTrusted(item, character, false)),
            OwnershipQueryMode.IsPropertyTrustedOrClan =>
                new BooleanVariable(queriedOwner is ICharacter character &&
                                    OwnershipFunctionHelpers.IsPropertyTrusted(item, character, true)),
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
            "isowner",
            [ProgVariableTypes.Item, ProgVariableTypes.Shop],
            (pars, gameworld) => new ItemOwnershipQueryFunction(pars, OwnershipQueryMode.IsOwner),
            ["item", "shop"],
            ["The item whose ownership you want to check", "The shop whose ownership is being tested"],
            "Returns true if the specified shop is the registered owner of the item.",
            "Items",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "isowner",
            [ProgVariableTypes.Item, ProgVariableTypes.Bank],
            (pars, gameworld) => new ItemOwnershipQueryFunction(pars, OwnershipQueryMode.IsOwner),
            ["item", "bank"],
            ["The item whose ownership you want to check", "The bank whose ownership is being tested"],
            "Returns true if the specified bank is the registered owner of the item.",
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

                List<IGameItem> items = _mode == OwnershipMutationMode.SetOwnership
                    ? [item]
                    : OwnershipFunctionHelpers.EnumerateDeepOwnedItems(item).ToList();
                foreach (IGameItem? target in items)
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

        RegisterTypedOwnershipMutation("setownership", ProgVariableTypes.Shop, OwnershipMutationMode.SetOwnership,
            "shop");
        RegisterTypedOwnershipMutation("setownership", ProgVariableTypes.Bank, OwnershipMutationMode.SetOwnership,
            "bank");

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

        RegisterTypedOwnershipMutation("deepsetownership", ProgVariableTypes.Shop,
            OwnershipMutationMode.DeepSetOwnership, "shop");
        RegisterTypedOwnershipMutation("deepsetownership", ProgVariableTypes.Bank,
            OwnershipMutationMode.DeepSetOwnership, "bank");

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

    private static void RegisterTypedOwnershipMutation(string name, ProgVariableTypes ownerType,
        OwnershipMutationMode mode, string ownerName)
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            name,
            [ProgVariableTypes.Item, ownerType],
            (pars, gameworld) => new ItemOwnershipMutationFunction(pars, mode),
            ["item", ownerName],
            ["The item whose ownership should be changed", $"The {ownerName} that should own the item"],
            $"Sets the registered owner of the selected item{(mode == OwnershipMutationMode.DeepSetOwnership ? " tree" : string.Empty)} to the specified {ownerName}.",
            "Items",
            ProgVariableTypes.Boolean));
    }
}
