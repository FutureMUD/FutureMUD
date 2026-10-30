using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Material;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

#nullable enable
namespace MudSharp.FutureProg.Functions.GameItem;

/// <summary>Atomically transforms liquid lineages while retaining amounts, freshness and specialised instance metadata.</summary>
internal sealed class RemapLiquidsFunction(IList<IFunction> parameters, IFuturemud world) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var from = ReadIds(ParameterFunctions[1].Result?.GetObject);
		var to = ReadIds(ParameterFunctions[2].Result?.GetObject);
		Result = new BooleanVariable(TryRemap(ParameterFunctions[0].Result?.GetObject as IGameItem, from, to, world,
			ParameterFunctions.Count < 4 || (bool?)ParameterFunctions[3].Result?.GetObject == true,
			ParameterFunctions.Count < 5 || (bool?)ParameterFunctions[4].Result?.GetObject == true));
		return StatementResult.Normal;
	}

	private static long[]? ReadIds(object? value)
	{
		if (value is not IEnumerable items) return null;
		var result = new List<long>();
		foreach (var item in items)
		{
			var raw = item is IProgVariable variable ? variable.GetObject : item;
			if (raw is not decimal number || number <= 0 || number > long.MaxValue || decimal.Truncate(number) != number) return null;
			result.Add((long)number);
		}
		return result.ToArray();
	}

	internal static bool TryRemap(IGameItem? item, long[]? from, long[]? to, IFuturemud world, bool requireOpen = true, bool requireCompleteMapping = true)
	{
		if (item?.GetItemType<ILiquidContainer>() is not { LiquidMixture: { } mixture } container ||
			requireOpen && !container.IsOpen || mixture.IsEmpty || from is null || to is null || from.Length != to.Length || from.Distinct().Count() != from.Length) return false;
		var map = new Dictionary<long, ILiquid>();
		for (var i = 0; i < from.Length; i++)
		{
			var target = world.Liquids.Get(to[i]);
			if (target is null) return false;
			map.Add(from[i], target);
		}
		var replacements = new List<LiquidInstance>();
		foreach (var instance in mixture.Instances)
		{
			if (!map.TryGetValue(instance.OriginLiquid.Id, out var replacement))
			{
				if (requireCompleteMapping) return false;
				replacements.Add(instance.Copy());
				continue;
			}
			var xml = instance.SaveToXml();
			xml.SetAttributeValue("id", replacement.Id);
			replacements.Add(LiquidInstance.LoadInstance(xml, world));
		}
		container.LiquidMixture = new LiquidMixture(replacements, world);
		return true;
	}

	public static void RegisterFunctionCompiler()
	{
		for (var count = 3; count <= 5; count++)
		{
			var types = new List<ProgVariableTypes> { ProgVariableTypes.Item, ProgVariableTypes.Number | ProgVariableTypes.Collection, ProgVariableTypes.Number | ProgVariableTypes.Collection };
			var names = new List<string> { "container", "fromLiquidIds", "toLiquidIds" };
			if (count >= 4) { types.Add(ProgVariableTypes.Boolean); names.Add("requireOpen"); }
			if (count >= 5) { types.Add(ProgVariableTypes.Boolean); names.Add("requireCompleteMapping"); }
			FutureProg.RegisterBuiltInFunctionCompiler(new("remapliquids", types.ToArray(), (p, w) => new RemapLiquidsFunction(p, w), names, names,
				"Atomically replaces liquid lineages, preserving volume, freshness and specialised metadata. Defaults require an open container and a complete map; explicit false options permit closed vessels or preserve unmapped constituents.", "Items", ProgVariableTypes.Boolean));
		}
	}
}
