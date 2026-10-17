#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockGatheringInstaller
{
	private const string Prefix = "arm.prog.gather.";
	internal static readonly (string Key, string Name, double Factor)[] TerrainClasses =
	[
		("fertile", "fertile land", 0.1), ("urban", "inside or city", 0.25), ("cavern", "cavern road or ruins", 0.5),
		("hills", "hills or mountains", 0.75), ("barren", "desert salt silt or sewer", 1.0), ("forbidden", "forbidden land", -1.0)
	];
	public static string TerrainTagKey(string key) => "arm.gather.terrain." + key;
	internal static string PolicyKey(string kind, string channel) => Prefix + kind.ToLowerInvariant() + "." + channel;
	internal const string AshVisibleKey = Prefix + "ash_visible";
	internal const string LandRatioKey = Prefix + "land.ratio";
	internal const string LandScarKey = Prefix + "land.scar";
	private static readonly (string Name, ProgVariableTypes Type)[] Context =
	[
		("actor", ProgVariableTypes.Character), ("owner", ProgVariableTypes.Character), ("capability", ProgVariableTypes.MagicCapability),
		("methodKey", ProgVariableTypes.Text), ("amount", ProgVariableTypes.Number), ("room", ProgVariableTypes.Location)
	];

	internal static IReadOnlyDictionary<string, FutureProg> Policies(long skill, string gateName,
		IReadOnlyDictionary<string, long> tags, IReadOnlyList<long> planes, long ashVisibleId)
	{
		var result = new Dictionary<string, FutureProg>(StringComparer.Ordinal);
		foreach (var land in new[] { false, true })
		{
			var kind = land ? "Land" : "Self";
			var args = "@actor, @owner, @capability, @methodKey, @amount, @room";
			var permission = $"if (@{gateName}({args}) == false)\nreturn false\nend if\n" + PricePrefix(land, skill, tags, planes, "false") +
				"\nreturn @price <= healthremaining(@actor, \"health\") and gettrait(@owner, totrait(" + skill + ")) > 0";
			result.Add(PolicyKey(kind, "permission"), Policy("ArmageddonGather" + kind + "Permission", ProgVariableTypes.Boolean, permission));
			foreach (var channel in new[] { "damage", "stun", "stamina" })
			{
				var body = PricePrefix(land, skill, tags, planes, "-1") + "\nif (@price == 0)\nreturn 0\nend if\n" +
					"if (@price > healthremaining(@actor, \"health\"))\nreturn -1\nend if\n";
				body += channel == "damage" ? "return min(@price, max(0, healthremaining(@actor, \"health\") - 1))" :
					$"return min(healthremaining(@actor, \"{channel}\"), healthcapacity(@actor, \"{channel}\") * @price / healthcapacity(@actor, \"health\"))";
				result.Add(PolicyKey(kind, channel), Policy("ArmageddonGather" + kind + char.ToUpperInvariant(channel[0]) + channel[1..], ProgVariableTypes.Number, body));
			}
			var success = land ? $$"""
var eco as number
var until as datetime
var skill as number
skill = min(100, max(0, gettrait(@owner, totrait({{skill}}))))
eco = min(100, max(-100, getregister(@owner, "{{ArmageddonGatheringRegisters.Ecology}}")))
until = getregister(@owner, "{{ArmageddonGatheringRegisters.AlignmentUntil}}")
if (@until <= now())
if (@eco > 0)
eco = max(0, @eco - 25)
else
eco = max(-100, @eco - 1)
end if
setregister @owner "{{ArmageddonGatheringRegisters.Ecology}}" @eco
setregister @owner "{{ArmageddonGatheringRegisters.AlignmentUntil}}" (now() + timespanfromseconds(600 * (min(floor(@skill / 15) + 1, 7) + random(0, 2)) - 1))
end if
var ash as datetime
ash = getregister(@room, "{{ArmageddonGatheringRegisters.AshUntil}}")
if (@ash < now())
ash = now()
end if
setregister @room "{{ArmageddonGatheringRegisters.AshUntil}}" (@ash + timespanfromseconds(270 * @amount))
adddescriptionaddition(@room, "Grey, lifeless ash collects on the ground.", {{ashVisibleId}})
""" : "";
			success += $"\ncheck(@owner, totrait({skill}), 5)\nreturn";
			result.Add(PolicyKey(kind, "success"), Policy("ArmageddonGather" + kind + "Success", ProgVariableTypes.Void, success,
				Context.Append(("operationId", ProgVariableTypes.Text)).ToArray()));
		}
		result.Add(AshVisibleKey, Policy("ArmageddonGatherAshVisible", ProgVariableTypes.Boolean,
			$"return getregister(@room, \"{ArmageddonGatheringRegisters.AshUntil}\") > now()",
			[("room", ProgVariableTypes.Location), ("viewer", ProgVariableTypes.Perceiver)]));
		result.Add(LandRatioKey, Policy("ArmageddonGatherLandRatio", ProgVariableTypes.Number, "return 1 / @amount",
			Context.Append(("sourceKey", ProgVariableTypes.Text)).ToArray()));
		result.Add(LandScarKey, Policy("ArmageddonGatherLandScar", ProgVariableTypes.Number, "return 1",
			Context.Append(("sources", ProgVariableTypes.Dictionary | ProgVariableTypes.Number)).ToArray()));
		return result;
	}

	private static string PricePrefix(bool land, long skill, IReadOnlyDictionary<string, long> tags,
		IReadOnlyList<long> planes, string failure)
	{
		var text = $$"""
if (@amount <= 0 or @amount != floor(@amount))
return {{failure}}
end if
var skill as number
var eco as number
var price as number
skill = min(100, max(0, gettrait(@owner, totrait({{skill}}))))
eco = getregister(@owner, "{{ArmageddonGatheringRegisters.Ecology}}")
if (@eco < -100 or @eco > 100)
return {{failure}}
end if
""";
		if (!land) return text + "\nif (@eco < 0)\nprice = ceiling(@amount * (2 - @skill / 100 - @eco * 0.030))\nelse\nprice = ceiling(@amount * (2 - @skill / 100 - @eco * 0.008))\nend if";
		text += "\nif (" + string.Join(" or ", planes.Select(x => $"currentplaneid(@actor) == {x}")) + $" or istagged(@room, tag({tags["forbidden"]})) or istagged(@room.terrain, tag({tags["forbidden"]})))\nreturn {failure}\nend if";
		text += "\nvar factor as number\nvar classes as number\nfactor = 1\nclasses = 0";
		foreach (var terrain in TerrainClasses.Where(x => x.Factor > 0))
			text += $"\nif (istagged(@room, tag({tags[terrain.Key]})) or istagged(@room.terrain, tag({tags[terrain.Key]})))\nclasses = @classes + 1\nfactor = {terrain.Factor.ToString(System.Globalization.CultureInfo.InvariantCulture)}\nend if";
		return text + $"\nif (@classes > 1)\nreturn {failure}\nend if\nprice = ceiling(@amount * (1.5 - @skill / 100 + @eco * 0.005)) * @factor\nif (getregister(@room, \"{ArmageddonGatheringRegisters.AshUntil}\") > now())\nprice = @price * 1.05\nend if";
	}

	private static FutureProg Policy(string name, ProgVariableTypes returns, string body,
		(string Name, ProgVariableTypes Type)[]? parameters = null)
	{
		var row = new FutureProg { FunctionName = name, FunctionText = body, ReturnTypeDefinition = returns.ToStorageString(),
			AcceptsAnyParameters = false, StaticType = 0, Category = "Magic", Subcategory = "Armageddon Gathering", Public = false,
			FunctionComment = "Builder-editable source-faithful gathering translation. Source trace and native unit adaptations: Armageddon_Gathering_Source_Translation_20261016.md. Stock availability remains independently false." };
		foreach (var (parameter, index) in (parameters ?? Context).Select((value, index) => (value, index)))
			row.FutureProgsParameters.Add(new() { ParameterIndex = index, ParameterName = parameter.Name, ParameterTypeDefinition = parameter.Type.ToStorageString() });
		return row;
	}
}
