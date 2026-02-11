using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Communication.Language;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.RPG.AIStorytellers;

public partial class AIStoryteller
{
	private static string BuildCustomToolCallSchema(AIStorytellerCustomToolCall toolCall)
	{
		var properties = new Dictionary<string, object>();
		var required = new List<string>();

		foreach (var (parameterType, parameterName) in toolCall.Prog.NamedParameters)
		{
			required.Add(parameterName);
			var description = toolCall.ParameterDescriptions.TryGetValue(parameterName, out var item)
				? item
				: string.Empty;
			properties[parameterName] = BuildJsonSchemaPropertyForProgType(parameterType, description);
		}

		var schema = new Dictionary<string, object>
		{
			["type"] = "object",
			["properties"] = properties,
			["required"] = required
		};

		return JsonSerializer.Serialize(schema);
	}

	private static object BuildJsonSchemaPropertyForProgType(ProgVariableTypes type, string description)
	{
		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			return new Dictionary<string, object>
			{
				["type"] = "array",
				["description"] = description,
				["items"] = BuildJsonSchemaPropertyForProgType(type ^ ProgVariableTypes.Collection,
					"Item value")
			};
		}

		return type switch
		{
			ProgVariableTypes.Boolean => new Dictionary<string, object>
			{
				["type"] = "boolean",
				["description"] = description
			},
			ProgVariableTypes.Number => new Dictionary<string, object>
			{
				["type"] = "number",
				["description"] = description
			},
			ProgVariableTypes.Effect => new Dictionary<string, object>
			{
				["type"] = "object",
				["description"] = description,
				["properties"] = new Dictionary<string, object>
				{
					["EffectId"] = new Dictionary<string, object>
					{
						["type"] = "integer",
						["description"] = "The id of the effect instance."
					},
					["OwnerType"] = new Dictionary<string, object>
					{
						["type"] = "string",
						["description"] = "Optional owner type (character, item, cell, zone, shard)."
					},
					["OwnerId"] = new Dictionary<string, object>
					{
						["type"] = "integer",
						["description"] = "Optional owner id, used with OwnerType."
					}
				},
				["required"] = new[] { "EffectId" }
			},
			ProgVariableTypes.Gender => new Dictionary<string, object>
			{
				["type"] = "string",
				["description"] =
					$"{description} Valid values are male, female, neuter, non-binary or indeterminate."
			},
			ProgVariableTypes.DateTime => new Dictionary<string, object>
			{
				["type"] = "string",
				["description"] = $"{description} Use an ISO-8601 datetime string."
			},
			ProgVariableTypes.TimeSpan => new Dictionary<string, object>
			{
				["type"] = "string",
				["description"] = $"{description} Use a TimeSpan string such as 00:30:00."
			},
			ProgVariableTypes.MudDateTime => new Dictionary<string, object>
			{
				["type"] = "string",
				["description"] = $"{description} Use a parseable mud datetime string such as never or a saved datetime value."
			},
			ProgVariableTypes.Text => new Dictionary<string, object>
			{
				["type"] = "string",
				["description"] = description
			},
			ProgVariableTypes.Outfit => new Dictionary<string, object>
			{
				["type"] = "object",
				["description"] = description,
				["properties"] = new Dictionary<string, object>
				{
					["OwnerCharacterId"] = new Dictionary<string, object>
					{
						["type"] = "integer",
						["description"] = "The id of the character who owns the outfit."
					},
					["OutfitName"] = new Dictionary<string, object>
					{
						["type"] = "string",
						["description"] = "The outfit name."
					}
				},
				["required"] = new[] { "OwnerCharacterId", "OutfitName" }
			},
			ProgVariableTypes.OutfitItem => new Dictionary<string, object>
			{
				["type"] = "object",
				["description"] = description,
				["properties"] = new Dictionary<string, object>
				{
					["OwnerCharacterId"] = new Dictionary<string, object>
					{
						["type"] = "integer",
						["description"] = "The id of the character who owns the outfit."
					},
					["OutfitName"] = new Dictionary<string, object>
					{
						["type"] = "string",
						["description"] = "Optional outfit name if item id appears in multiple outfits."
					},
					["ItemId"] = new Dictionary<string, object>
					{
						["type"] = "integer",
						["description"] = "The id of the outfit item (item id)."
					}
				},
				["required"] = new[] { "OwnerCharacterId", "ItemId" }
			},
			_ => new Dictionary<string, object>
			{
				["type"] = "integer",
				["description"] = $"Engine object id. {description}"
			}
		};
	}


	private static bool TryReadId(JsonElement element, out long id)
	{
		id = 0L;
		switch (element.ValueKind)
		{
			case JsonValueKind.Number:
				return element.TryGetInt64(out id);
			case JsonValueKind.String:
				return long.TryParse(element.GetString(), out id);
			case JsonValueKind.Object:
				foreach (var name in new[] { "Id", "ID", "id" })
				{
					if (element.TryGetProperty(name, out var property) && TryReadId(property, out id))
					{
						return true;
					}
				}

				return false;
			default:
				return false;
		}
	}

	private static bool TryGetOptionalLong(JsonElement arguments, out long value, params string[] propertyNames)
	{
		value = 0L;
		foreach (var name in propertyNames)
		{
			if (!arguments.TryGetProperty(name, out var property))
			{
				continue;
			}

			return TryReadId(property, out value);
		}

		return false;
	}

	private static string? TryGetOptionalString(JsonElement arguments, params string[] propertyNames)
	{
		foreach (var propertyName in propertyNames)
		{
			var text = TryGetOptionalString(arguments, propertyName);
			if (text is not null)
			{
				return text;
			}
		}

		return null;
	}

	private static string? TryGetOptionalString(JsonElement arguments, string propertyName)
	{
		if (!arguments.TryGetProperty(propertyName, out var property))
		{
			return null;
		}

		if (property.ValueKind == JsonValueKind.String)
		{
			return property.GetString() ?? string.Empty;
		}

		return property.ToString();
	}

	private static bool TryGetRequiredString(JsonElement arguments, string propertyName, out string value,
		out string error)
	{
		value = string.Empty;
		if (!arguments.TryGetProperty(propertyName, out var property))
		{
			error = $"Missing required property '{propertyName}'.";
			return false;
		}

		value = property.ValueKind == JsonValueKind.String
			? property.GetString() ?? string.Empty
			: property.ToString();
		error = string.Empty;
		return true;
	}

	internal static bool TryParseAttentionClassifierOutput(string? response, out bool interested, out string reason,
		out string error)
	{
		interested = false;
		reason = string.Empty;
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(response))
		{
			error = "Attention classifier returned empty output.";
			return false;
		}

		try
		{
			using var json = JsonDocument.Parse(response);
			if (json.RootElement.ValueKind != JsonValueKind.Object)
			{
				error = "Attention classifier output must be a JSON object.";
				return false;
			}

			var root = json.RootElement;
			var decision = TryGetOptionalString(root, "Decision", "decision", "Result", "result")
				?.Trim()
				.ToLowerInvariant();
			if (string.IsNullOrWhiteSpace(decision))
			{
				error = "Attention classifier output is missing required Decision property.";
				return false;
			}

			switch (decision)
			{
				case "ignore":
					interested = false;
					reason = string.Empty;
					return true;
				case "interested":
					interested = true;
					reason = TryGetOptionalString(root, "Reason", "reason")
						         ?.Trim()
						         .IfNullOrWhiteSpace("No reason provided")
					         ?? "No reason provided";
					return true;
				default:
					error =
						$"Attention classifier Decision must be interested or ignore, but was '{decision}'.";
					return false;
			}
		}
		catch (JsonException e)
		{
			error = $"Attention classifier output was not valid JSON: {e.Message}";
			return false;
		}
	}

	private bool TryInterpretAttentionClassifierOutput(string response, out bool interested, out string reason)
	{
		if (TryParseAttentionClassifierOutput(response, out interested, out reason, out var parseError))
		{
			return true;
		}

		LogStorytellerError($"Attention classifier contract violation: {parseError}");
		interested = false;
		reason = string.Empty;
		return false;
	}

	internal bool TryInterpretAttentionClassifierOutputForTesting(string response, out bool interested, out string reason)
	{
		return TryInterpretAttentionClassifierOutput(response, out interested, out reason);
	}

	private static bool TryGetRequiredLong(JsonElement arguments, string propertyName, out long value, out string error)
	{
		value = 0L;
		if (!arguments.TryGetProperty(propertyName, out var property))
		{
			error = $"Missing required property '{propertyName}'.";
			return false;
		}

		if (TryReadId(property, out value))
		{
			error = string.Empty;
			return true;
		}

		error = $"Property '{propertyName}' must contain an integer id.";
		return false;
	}


	private bool TryConvertJsonArgument(JsonElement element, ProgVariableTypes type, out object? convertedValue,
		out string error)
	{
		type &= ~ProgVariableTypes.Literal;
		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			if (element.ValueKind != JsonValueKind.Array)
			{
				convertedValue = null;
				error = $"Expected array value for parameter type {type.Describe()}.";
				return false;
			}

			var innerType = type ^ ProgVariableTypes.Collection;
			var list = new List<object?>();
			foreach (var item in element.EnumerateArray())
			{
				if (!TryConvertJsonArgument(item, innerType, out var value, out error))
				{
					convertedValue = null;
					return false;
				}

				list.Add(value);
			}

			convertedValue = list;
			error = string.Empty;
			return true;
		}

		if (element.ValueKind == JsonValueKind.Null && ProgVariableTypes.ReferenceType.HasFlag(type))
		{
			convertedValue = null;
			error = string.Empty;
			return true;
		}

		switch (type)
		{
			case ProgVariableTypes.Boolean:
				if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
				{
					convertedValue = element.GetBoolean();
					error = string.Empty;
					return true;
				}

				if (element.ValueKind == JsonValueKind.String &&
				    bool.TryParse(element.GetString(), out var boolValue))
				{
					convertedValue = boolValue;
					error = string.Empty;
					return true;
				}

				convertedValue = null;
				error = "Expected boolean argument.";
				return false;
			case ProgVariableTypes.Number:
				if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out var numberValue))
				{
					convertedValue = numberValue;
					error = string.Empty;
					return true;
				}

				if (element.ValueKind == JsonValueKind.String &&
				    decimal.TryParse(element.GetString(), out numberValue))
				{
					convertedValue = numberValue;
					error = string.Empty;
					return true;
				}

				convertedValue = null;
				error = "Expected numeric argument.";
				return false;
			case ProgVariableTypes.Text:
				convertedValue = element.ValueKind == JsonValueKind.String
					? element.GetString() ?? string.Empty
					: element.ToString();
				error = string.Empty;
				return true;
			case ProgVariableTypes.Gender:
				if (TryResolveGenderArgument(element, out var gender, out error))
				{
					convertedValue = gender;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.TimeSpan:
				if (TryResolveTimeSpanArgument(element, out var timeSpan, out error))
				{
					convertedValue = timeSpan;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.DateTime:
				if (TryResolveDateTimeArgument(element, out var dateTime, out error))
				{
					convertedValue = dateTime;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.MudDateTime:
				if (TryResolveMudDateTimeArgument(element, out var mudDateTime, out error))
				{
					convertedValue = mudDateTime;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Shard:
				if (TryResolveShardArgument(element, out var shard, out error))
				{
					convertedValue = shard;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Zone:
				if (TryResolveZoneArgument(element, out var zone, out error))
				{
					convertedValue = zone;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Race:
				if (TryResolveRaceArgument(element, out var race, out error))
				{
					convertedValue = race;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Culture:
				if (TryResolveCultureArgument(element, out var culture, out error))
				{
					convertedValue = culture;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Trait:
				if (TryResolveTraitArgument(element, out var trait, out error))
				{
					convertedValue = trait;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Clan:
				if (TryResolveClanArgument(element, out var clan, out error))
				{
					convertedValue = clan;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Ethnicity:
				if (TryResolveEthnicityArgument(element, out var ethnicity, out error))
				{
					convertedValue = ethnicity;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.ClanRank:
				if (TryResolveClanRankArgument(element, out var clanRank, out error))
				{
					convertedValue = clanRank;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.ClanAppointment:
				if (TryResolveClanAppointmentArgument(element, out var clanAppointment, out error))
				{
					convertedValue = clanAppointment;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.ClanPaygrade:
				if (TryResolveClanPaygradeArgument(element, out var clanPaygrade, out error))
				{
					convertedValue = clanPaygrade;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Currency:
				if (TryResolveCurrencyArgument(element, out var currency, out error))
				{
					convertedValue = currency;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Exit:
				if (TryResolveExitArgument(element, out var exit, out error))
				{
					convertedValue = exit;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Language:
				if (TryResolveLanguageArgument(element, out var language, out error))
				{
					convertedValue = language;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Accent:
				if (TryResolveAccentArgument(element, out var accent, out error))
				{
					convertedValue = accent;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Merit:
				if (TryResolveMeritArgument(element, out var merit, out error))
				{
					convertedValue = merit;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Calendar:
				if (TryResolveCalendarArgument(element, out var calendar, out error))
				{
					convertedValue = calendar;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Clock:
				if (TryResolveClockArgument(element, out var clock, out error))
				{
					convertedValue = clock;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Knowledge:
				if (TryResolveKnowledgeArgument(element, out var knowledge, out error))
				{
					convertedValue = knowledge;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Role:
				if (TryResolveRoleArgument(element, out var role, out error))
				{
					convertedValue = role;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Drug:
				if (TryResolveDrugArgument(element, out var drug, out error))
				{
					convertedValue = drug;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Shop:
				if (TryResolveShopArgument(element, out var shop, out error))
				{
					convertedValue = shop;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Effect:
				if (TryResolveEffectArgument(element, out var effect, out error))
				{
					convertedValue = effect;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.Outfit:
				if (TryResolveOutfitArgument(element, out var outfit, out error))
				{
					convertedValue = outfit;
					return true;
				}

				convertedValue = null;
				return false;
			case ProgVariableTypes.OutfitItem:
				if (TryResolveOutfitItemArgument(element, out var outfitItem, out error))
				{
					convertedValue = outfitItem;
					return true;
				}

				convertedValue = null;
				return false;
		}

		if (!TryReadId(element, out var id))
		{
			convertedValue = null;
			error = $"Expected an id argument for parameter type {type.Describe()}.";
			return false;
		}

		convertedValue = type switch
		{
			ProgVariableTypes.Character or ProgVariableTypes.Toon => Gameworld.TryGetCharacter(id, true),
			ProgVariableTypes.Item => Gameworld.Items.Get(id),
			ProgVariableTypes.Location => Gameworld.Cells.Get(id),
			ProgVariableTypes.WeatherEvent => Gameworld.WeatherEvents.Get(id),
			ProgVariableTypes.Merchandise => Gameworld.Shops
				.SelectMany(x => x.Merchandises)
				.FirstOrDefault(x => x.Id == id),
			ProgVariableTypes.Script => Gameworld.Scripts.Get(id),
			ProgVariableTypes.Writing => Gameworld.Writings.Get(id),
			ProgVariableTypes.OverlayPackage => Gameworld.CellOverlayPackages.Get(id),
			ProgVariableTypes.Terrain => Gameworld.Terrains.Get(id),
			ProgVariableTypes.Solid => Gameworld.Materials.Get(id),
			ProgVariableTypes.Liquid => Gameworld.Liquids.Get(id),
			ProgVariableTypes.Gas => Gameworld.Gases.Get(id),
			ProgVariableTypes.Material => (object?)Gameworld.Materials.Get(id) ?? (object?)Gameworld.Liquids.Get(id) ??
			                              Gameworld.Gases.Get(id),
			ProgVariableTypes.MagicSchool => Gameworld.MagicSchools.Get(id),
			ProgVariableTypes.MagicCapability => Gameworld.MagicCapabilities.Get(id),
			ProgVariableTypes.MagicSpell => Gameworld.MagicSpells.Get(id),
			ProgVariableTypes.Bank => Gameworld.Banks.Get(id),
			ProgVariableTypes.BankAccount => Gameworld.BankAccounts.Get(id),
			ProgVariableTypes.BankAccountType => Gameworld.BankAccountTypes.Get(id),
			ProgVariableTypes.Project => Gameworld.ActiveProjects.Get(id),
			ProgVariableTypes.Law => Gameworld.Laws.Get(id),
			ProgVariableTypes.LegalAuthority => Gameworld.LegalAuthorities.Get(id),
			ProgVariableTypes.Market => Gameworld.Markets.Get(id),
			ProgVariableTypes.MarketCategory => Gameworld.MarketCategories.Get(id),
			ProgVariableTypes.Crime => Gameworld.Crimes.Get(id),
			ProgVariableTypes.Area => Gameworld.Areas.Get(id),
			ProgVariableTypes.Tagged => (object?)Gameworld.Cells.Get(id) ?? (object?)Gameworld.Items.Get(id) ??
			                            Gameworld.Terrains.Get(id),
			ProgVariableTypes.Perceivable or ProgVariableTypes.Perceiver or ProgVariableTypes.MagicResourceHaver =>
				(object?)Gameworld.TryGetCharacter(id, true) ?? (object?)Gameworld.Items.Get(id) ??
				Gameworld.Cells.Get(id),
			_ => null
		};

		if (convertedValue is null)
		{
			error = $"No game object exists for id {id:N0} as required by {type.Describe()}.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private static bool TryResolveGenderArgument(JsonElement element, out Gender gender, out string error)
	{
		gender = Gender.Indeterminate;
		if (element.ValueKind == JsonValueKind.Object)
		{
			if (TryGetOptionalLong(element, out var numericGender, "GenderId", "Gender", "Value", "Id", "ID", "id") &&
			    Enum.IsDefined(typeof(Gender), (int)numericGender))
			{
				gender = (Gender)numericGender;
				error = string.Empty;
				return true;
			}

			var genderText = TryGetOptionalString(element, "Gender", "Value", "Name");
			if (!string.IsNullOrWhiteSpace(genderText))
			{
				return TryResolveGenderText(genderText, out gender, out error);
			}

			error = "Gender arguments must include a Gender value.";
			return false;
		}

		if (element.ValueKind == JsonValueKind.Number &&
		    element.TryGetInt32(out var numericValue) &&
		    Enum.IsDefined(typeof(Gender), numericValue))
		{
			gender = (Gender)numericValue;
			error = string.Empty;
			return true;
		}

		if (element.ValueKind == JsonValueKind.String)
		{
			return TryResolveGenderText(element.GetString() ?? string.Empty, out gender, out error);
		}

		error = "Expected a gender argument.";
		return false;
	}

	private static bool TryResolveGenderText(string value, out Gender gender, out string error)
	{
		gender = Gender.Indeterminate;
		switch (value.Trim().ToLowerInvariant())
		{
			case "male":
			case "m":
				gender = Gender.Male;
				error = string.Empty;
				return true;
			case "female":
			case "f":
				gender = Gender.Female;
				error = string.Empty;
				return true;
			case "neuter":
			case "n":
				gender = Gender.Neuter;
				error = string.Empty;
				return true;
			case "non-binary":
			case "nonbinary":
			case "nb":
				gender = Gender.NonBinary;
				error = string.Empty;
				return true;
			case "indeterminate":
			case "unknown":
				gender = Gender.Indeterminate;
				error = string.Empty;
				return true;
			default:
				error = "Expected a gender argument.";
				return false;
		}
	}

	private static bool TryResolveDateTimeArgument(JsonElement element, out DateTime dateTime, out string error)
	{
		if (element.ValueKind == JsonValueKind.Object)
		{
			if (TryGetOptionalLong(element, out var unixSeconds, "UnixSeconds", "EpochSeconds", "Timestamp"))
			{
				dateTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;
				error = string.Empty;
				return true;
			}

			var dateTimeText = TryGetOptionalString(element, "Value", "DateTime", "Timestamp", "Iso8601");
			if (!string.IsNullOrWhiteSpace(dateTimeText))
			{
				return TryResolveDateTimeText(dateTimeText, out dateTime, out error);
			}
		}

		if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var numericValue))
		{
			dateTime = DateTimeOffset.FromUnixTimeSeconds(numericValue).UtcDateTime;
			error = string.Empty;
			return true;
		}

		if (element.ValueKind == JsonValueKind.String)
		{
			return TryResolveDateTimeText(element.GetString() ?? string.Empty, out dateTime, out error);
		}

		dateTime = default;
		error = "Expected datetime text argument.";
		return false;
	}

	private static bool TryResolveDateTimeText(string text, out DateTime dateTime, out string error)
	{
		if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime) ||
		    DateTime.TryParse(text, out dateTime))
		{
			error = string.Empty;
			return true;
		}

		error = "Expected datetime text argument.";
		return false;
	}

	private static bool TryResolveTimeSpanArgument(JsonElement element, out TimeSpan timeSpan, out string error)
	{
		if (element.ValueKind == JsonValueKind.Object)
		{
			if (TryGetOptionalLong(element, out var totalSeconds, "TotalSeconds", "Seconds"))
			{
				timeSpan = TimeSpan.FromSeconds(totalSeconds);
				error = string.Empty;
				return true;
			}

			var spanText = TryGetOptionalString(element, "Value", "TimeSpan", "Duration");
			if (!string.IsNullOrWhiteSpace(spanText))
			{
				return TryResolveTimeSpanText(spanText, out timeSpan, out error);
			}
		}

		if (element.ValueKind == JsonValueKind.Number && element.TryGetDouble(out var seconds))
		{
			timeSpan = TimeSpan.FromSeconds(seconds);
			error = string.Empty;
			return true;
		}

		if (element.ValueKind == JsonValueKind.String)
		{
			return TryResolveTimeSpanText(element.GetString() ?? string.Empty, out timeSpan, out error);
		}

		timeSpan = default;
		error = "Expected timespan text argument.";
		return false;
	}

	private static bool TryResolveTimeSpanText(string text, out TimeSpan timeSpan, out string error)
	{
		if (TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out timeSpan) || TimeSpan.TryParse(text, out timeSpan))
		{
			error = string.Empty;
			return true;
		}

		error = "Expected timespan text argument.";
		return false;
	}

	private bool TryResolveMudDateTimeArgument(JsonElement element, out MudDateTime? mudDateTime, out string error)
	{
		mudDateTime = null;
		if (element.ValueKind == JsonValueKind.Object)
		{
			var mudDateTimeText = TryGetOptionalString(element, "Value", "MudDateTime", "DateTime");
			if (!string.IsNullOrWhiteSpace(mudDateTimeText))
			{
				return TryResolveMudDateTimeText(mudDateTimeText, out mudDateTime, out error);
			}
		}

		if (element.ValueKind == JsonValueKind.String)
		{
			return TryResolveMudDateTimeText(element.GetString() ?? string.Empty, out mudDateTime, out error);
		}

		error = "Expected mud datetime text argument.";
		return false;
	}

	private bool TryResolveMudDateTimeText(string text, out MudDateTime? mudDateTime, out string error)
	{
		if (MudDateTime.TryParse(text, Gameworld, out var parsed))
		{
			mudDateTime = parsed;
			error = string.Empty;
			return true;
		}

		mudDateTime = null;
		error = "Expected mud datetime text argument.";
		return false;
	}

	private bool TryResolveFrameworkItemArgument<T>(
		JsonElement element,
		string typeName,
		Func<long, T?> resolveById,
		IEnumerable<T> searchItems,
		out T? item,
		out string error,
		string[] idPropertyNames,
		string[] namePropertyNames) where T : class, IFrameworkItem
	{
		item = null;
		var searchList = searchItems?.ToList() ?? [];
		if (element.ValueKind == JsonValueKind.Object)
		{
			var allIdNames = idPropertyNames.Concat(["Id", "ID", "id"]).Distinct().ToArray();
			if (TryGetOptionalLong(element, out var idValue, allIdNames))
			{
				item = resolveById(idValue);
				if (item is not null)
				{
					error = string.Empty;
					return true;
				}

				error = $"No {typeName} with id {idValue:N0} exists.";
				return false;
			}

			var allNameNames = namePropertyNames.Concat(["Name", "name"]).Distinct().ToArray();
			var nameValue = TryGetOptionalString(element, allNameNames);
			if (!string.IsNullOrWhiteSpace(nameValue))
			{
				var matches = FindFrameworkItemMatchesByName(searchList, nameValue);
				if (matches.Count == 1)
				{
					item = matches[0];
					error = string.Empty;
					return true;
				}

				if (matches.Count == 0)
				{
					error = $"No {typeName} named '{nameValue}' exists.";
					return false;
				}

				error = $"'{nameValue}' is ambiguous for {typeName}; provide an id.";
				return false;
			}

			error =
				$"{typeName} arguments must include one of [{allIdNames.ListToCommaSeparatedValues()}] or [{allNameNames.ListToCommaSeparatedValues()}].";
			return false;
		}

		if (TryReadId(element, out var id))
		{
			item = resolveById(id);
			if (item is not null)
			{
				error = string.Empty;
				return true;
			}

			error = $"No {typeName} with id {id:N0} exists.";
			return false;
		}

		if (element.ValueKind == JsonValueKind.String)
		{
			var name = element.GetString() ?? string.Empty;
			var matches = FindFrameworkItemMatchesByName(searchList, name);
			if (matches.Count == 1)
			{
				item = matches[0];
				error = string.Empty;
				return true;
			}

			if (matches.Count == 0)
			{
				error = $"No {typeName} named '{name}' exists.";
				return false;
			}

			error = $"'{name}' is ambiguous for {typeName}; provide an id.";
			return false;
		}

		error = $"Expected {typeName} as an id, name, or object argument.";
		return false;
	}

	private static List<T> FindFrameworkItemMatchesByName<T>(IEnumerable<T> items, string name)
		where T : class, IFrameworkItem
	{
		var text = name?.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return [];
		}

		var itemList = items.ToList();
		var exactMatches = itemList
			.Where(x => x.Name.EqualTo(text) || (x is IHaveMultipleNames named && named.Names.Any(y => y.EqualTo(text))))
			.ToList();
		if (exactMatches.Any())
		{
			return exactMatches;
		}

		return itemList
			.Where(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase) ||
			            (x is IHaveMultipleNames named &&
			             named.Names.Any(y => y.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))))
			.ToList();
	}

	private bool TryResolveOptionalClanFilter(JsonElement element, out IClan? clan, out string error)
	{
		clan = null;
		if (TryGetOptionalLong(element, out var clanId, "ClanId", "ClanID", "Clan"))
		{
			clan = Gameworld.Clans.Get(clanId);
			if (clan is null)
			{
				error = $"No clan with id {clanId:N0} exists.";
				return false;
			}

			error = string.Empty;
			return true;
		}

		var clanName = TryGetOptionalString(element, "ClanName", "Clan");
		if (!string.IsNullOrWhiteSpace(clanName))
		{
			var matches = FindFrameworkItemMatchesByName(Gameworld.Clans, clanName);
			if (matches.Count == 1)
			{
				clan = matches[0];
				error = string.Empty;
				return true;
			}

			if (matches.Count == 0)
			{
				error = $"No clan named '{clanName}' exists.";
				return false;
			}

			error = $"'{clanName}' is ambiguous for clan; provide ClanId.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private bool TryResolveClanScopedArgument<T>(
		JsonElement element,
		string typeName,
		Func<IClan, IEnumerable<T>> selector,
		out T? item,
		out string error,
		string[] idPropertyNames,
		string[] namePropertyNames) where T : class, IFrameworkItem
	{
		IEnumerable<T> candidates = Gameworld.Clans.SelectMany(selector).ToList();
		if (element.ValueKind == JsonValueKind.Object)
		{
			if (!TryResolveOptionalClanFilter(element, out var clan, out error))
			{
				item = null;
				return false;
			}

			if (clan is not null)
			{
				candidates = selector(clan).ToList();
			}
		}

		var candidateList = candidates.ToList();
		return TryResolveFrameworkItemArgument(
			element,
			typeName,
			id => candidateList.FirstOrDefault(x => x.Id == id),
			candidateList,
			out item,
			out error,
			idPropertyNames,
			namePropertyNames);
	}

	private bool TryResolveShardArgument(JsonElement element, out IShard? shard, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Shard",
			id => Gameworld.Shards.Get(id),
			Gameworld.Shards,
			out shard,
			out error,
			["ShardId"],
			["ShardName", "Shard"]);
	}

	private bool TryResolveZoneArgument(JsonElement element, out IZone? zone, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Zone",
			id => Gameworld.Zones.Get(id),
			Gameworld.Zones,
			out zone,
			out error,
			["ZoneId"],
			["ZoneName", "Zone"]);
	}

	private bool TryResolveRaceArgument(JsonElement element, out IRace? race, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Race",
			id => Gameworld.Races.Get(id),
			Gameworld.Races,
			out race,
			out error,
			["RaceId"],
			["RaceName", "Race"]);
	}

	private bool TryResolveCultureArgument(JsonElement element, out ICulture? culture, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Culture",
			id => Gameworld.Cultures.Get(id),
			Gameworld.Cultures,
			out culture,
			out error,
			["CultureId"],
			["CultureName", "Culture"]);
	}

	private bool TryResolveTraitArgument(JsonElement element, out ITraitDefinition? trait, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Trait",
			id => Gameworld.Traits.Get(id),
			Gameworld.Traits,
			out trait,
			out error,
			["TraitId"],
			["TraitName", "Trait"]);
	}

	private bool TryResolveClanArgument(JsonElement element, out IClan? clan, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Clan",
			id => Gameworld.Clans.Get(id),
			Gameworld.Clans,
			out clan,
			out error,
			["ClanId"],
			["ClanName", "Clan"]);
	}

	private bool TryResolveEthnicityArgument(JsonElement element, out IEthnicity? ethnicity, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Ethnicity",
			id => Gameworld.Ethnicities.Get(id),
			Gameworld.Ethnicities,
			out ethnicity,
			out error,
			["EthnicityId"],
			["EthnicityName", "Ethnicity"]);
	}

	private bool TryResolveClanRankArgument(JsonElement element, out IRank? rank, out string error)
	{
		return TryResolveClanScopedArgument(
			element,
			"ClanRank",
			clan => clan.Ranks,
			out rank,
			out error,
			["ClanRankId", "RankId"],
			["ClanRankName", "RankName", "Rank"]);
	}

	private bool TryResolveClanAppointmentArgument(JsonElement element, out IAppointment? appointment, out string error)
	{
		return TryResolveClanScopedArgument(
			element,
			"ClanAppointment",
			clan => clan.Appointments,
			out appointment,
			out error,
			["ClanAppointmentId", "AppointmentId"],
			["ClanAppointmentName", "AppointmentName", "Appointment"]);
	}

	private bool TryResolveClanPaygradeArgument(JsonElement element, out IPaygrade? paygrade, out string error)
	{
		IEnumerable<IPaygrade> candidates = Gameworld.Clans.SelectMany(x => x.Paygrades).ToList();
		if (element.ValueKind == JsonValueKind.Object)
		{
			if (!TryResolveOptionalClanFilter(element, out var clan, out error))
			{
				paygrade = null;
				return false;
			}

			if (clan is not null)
			{
				candidates = clan.Paygrades.ToList();
			}
		}

		var candidateList = candidates.ToList();
		if (TryResolveFrameworkItemArgument(
			    element,
			    "ClanPaygrade",
			    id => candidateList.FirstOrDefault(x => x.Id == id),
			    candidateList,
			    out paygrade,
			    out error,
			    ["ClanPaygradeId", "PaygradeId"],
			    ["ClanPaygradeName", "PaygradeName", "Paygrade"]))
		{
			return true;
		}

		var paygradeText = element.ValueKind == JsonValueKind.Object
			? TryGetOptionalString(element, "PaygradeAbbreviation", "Abbreviation")
			: element.ValueKind == JsonValueKind.String
				? element.GetString()
				: null;
		if (string.IsNullOrWhiteSpace(paygradeText))
		{
			paygrade = null;
			return false;
		}

		var matches = candidateList
			.Where(x => x.Abbreviation.EqualTo(paygradeText))
			.ToList();
		if (matches.Count == 1)
		{
			paygrade = matches[0];
			error = string.Empty;
			return true;
		}

		if (matches.Count > 1)
		{
			paygrade = null;
			error = $"'{paygradeText}' is ambiguous for ClanPaygrade; provide an id.";
			return false;
		}

		paygrade = null;
		error = $"No ClanPaygrade with abbreviation '{paygradeText}' exists.";
		return false;
	}

	private bool TryResolveCurrencyArgument(JsonElement element, out ICurrency? currency, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Currency",
			id => Gameworld.Currencies.Get(id),
			Gameworld.Currencies,
			out currency,
			out error,
			["CurrencyId"],
			["CurrencyName", "Currency"]);
	}

	private bool TryResolveExitArgument(JsonElement element, out IExit? exit, out string error)
	{
		if (element.ValueKind == JsonValueKind.Object &&
		    TryGetOptionalLong(element, out var exitId, "ExitId", "ExitID", "Exit"))
		{
			exit = Gameworld.ExitManager.GetExitByID(exitId);
			if (exit is not null)
			{
				error = string.Empty;
				return true;
			}

			error = $"No exit with id {exitId:N0} exists.";
			return false;
		}

		if (TryReadId(element, out var id))
		{
			exit = Gameworld.ExitManager.GetExitByID(id);
			if (exit is not null)
			{
				error = string.Empty;
				return true;
			}

			error = $"No exit with id {id:N0} exists.";
			return false;
		}

		exit = null;
		error = "Expected Exit as an id or object containing ExitId.";
		return false;
	}

	private bool TryResolveLanguageArgument(JsonElement element, out ILanguage? language, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Language",
			id => Gameworld.Languages.Get(id),
			Gameworld.Languages,
			out language,
			out error,
			["LanguageId"],
			["LanguageName", "Language"]);
	}

	private bool TryResolveAccentArgument(JsonElement element, out IAccent? accent, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Accent",
			id => Gameworld.Accents.Get(id),
			Gameworld.Accents,
			out accent,
			out error,
			["AccentId"],
			["AccentName", "Accent"]);
	}

	private bool TryResolveMeritArgument(JsonElement element, out IMerit? merit, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Merit",
			id => Gameworld.Merits.Get(id),
			Gameworld.Merits,
			out merit,
			out error,
			["MeritId"],
			["MeritName", "Merit"]);
	}

	private bool TryResolveCalendarArgument(JsonElement element, out ICalendar? calendar, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Calendar",
			id => Gameworld.Calendars.Get(id),
			Gameworld.Calendars,
			out calendar,
			out error,
			["CalendarId"],
			["CalendarName", "Calendar", "Alias"]);
	}

	private bool TryResolveClockArgument(JsonElement element, out IClock? clock, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Clock",
			id => Gameworld.Clocks.Get(id),
			Gameworld.Clocks,
			out clock,
			out error,
			["ClockId"],
			["ClockName", "Clock", "Alias"]);
	}

	private bool TryResolveKnowledgeArgument(JsonElement element, out IKnowledge? knowledge, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Knowledge",
			id => Gameworld.Knowledges.Get(id),
			Gameworld.Knowledges,
			out knowledge,
			out error,
			["KnowledgeId"],
			["KnowledgeName", "Knowledge"]);
	}

	private bool TryResolveRoleArgument(JsonElement element, out IChargenRole? role, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Role",
			id => Gameworld.Roles.Get(id),
			Gameworld.Roles,
			out role,
			out error,
			["RoleId"],
			["RoleName", "Role"]);
	}

	private bool TryResolveDrugArgument(JsonElement element, out IDrug? drug, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Drug",
			id => Gameworld.Drugs.Get(id),
			Gameworld.Drugs,
			out drug,
			out error,
			["DrugId"],
			["DrugName", "Drug"]);
	}

	private bool TryResolveShopArgument(JsonElement element, out IShop? shop, out string error)
	{
		return TryResolveFrameworkItemArgument(
			element,
			"Shop",
			id => Gameworld.Shops.Get(id),
			Gameworld.Shops,
			out shop,
			out error,
			["ShopId"],
			["ShopName", "Shop"]);
	}

	private bool TryResolveEffectArgument(JsonElement element, out IEffect? effect, out string error)
	{
		effect = null;
		var hasOwnerInfo = false;
		long? ownerId = null;
		string? ownerType = null;
		long effectId;

		if (element.ValueKind == JsonValueKind.Object)
		{
			if (!TryGetOptionalLong(element, out effectId, "EffectId", "Id", "ID", "id"))
			{
				error = "Effect arguments must include EffectId.";
				return false;
			}

			if (TryGetOptionalLong(element, out var parsedOwnerId, "OwnerId", "ownerId", "OwnerCharacterId",
				    "CharacterId"))
			{
				ownerId = parsedOwnerId;
				hasOwnerInfo = true;
			}

			ownerType = TryGetOptionalString(element, "OwnerType", "ownerType", "Type", "type");
			hasOwnerInfo |= !string.IsNullOrWhiteSpace(ownerType);
		}
		else if (TryReadId(element, out effectId))
		{
			// Bare ids are accepted, but owner hints are preferred for disambiguation.
		}
		else
		{
			error = "Effect arguments must be an id or an object containing EffectId.";
			return false;
		}

		if (hasOwnerInfo)
		{
			var owner = ResolveEffectOwner(ownerId, ownerType);
			if (owner is null)
			{
				error =
					$"Could not resolve effect owner using OwnerType='{ownerType ?? "(unspecified)"}' and OwnerId='{ownerId?.ToString() ?? "(unspecified)"}'.";
				return false;
			}

			effect = owner.Effects.FirstOrDefault(x => x.Id == effectId);
			if (effect is null)
			{
				error = $"Owner does not have an effect with id {effectId:N0}.";
				return false;
			}

			error = string.Empty;
			return true;
		}

		var matches = EnumerateEffectOwners()
			.SelectMany(x => x.Effects)
			.Where(x => x.Id == effectId)
			.ToList();
		if (matches.Count == 0)
		{
			error = $"No effect with id {effectId:N0} exists.";
			return false;
		}

		if (matches.Count > 1)
		{
			error =
				$"Effect id {effectId:N0} is ambiguous across multiple owners. Supply OwnerType and OwnerId.";
			return false;
		}

		effect = matches[0];
		error = string.Empty;
		return true;
	}

	private IHaveEffects? ResolveEffectOwner(long? ownerId, string? ownerType)
	{
		var normalizedType = ownerType?.Trim().ToLowerInvariant();
		if (!string.IsNullOrWhiteSpace(normalizedType))
		{
			return normalizedType switch
			{
				"character" or "char" or "pc" => ownerId.HasValue ? Gameworld.TryGetCharacter(ownerId.Value, true) : null,
				"item" => ownerId.HasValue ? Gameworld.Items.Get(ownerId.Value) : null,
				"cell" or "room" or "location" => ownerId.HasValue ? Gameworld.Cells.Get(ownerId.Value) : null,
				"zone" => ownerId.HasValue ? Gameworld.Zones.Get(ownerId.Value) : null,
				"shard" => ownerId.HasValue ? Gameworld.Shards.Get(ownerId.Value) : null,
				_ => null
			};
		}

		if (!ownerId.HasValue)
		{
			return null;
		}

		return (IHaveEffects?)Gameworld.TryGetCharacter(ownerId.Value, true) ??
		       (IHaveEffects?)Gameworld.Items.Get(ownerId.Value) ??
		       (IHaveEffects?)Gameworld.Cells.Get(ownerId.Value) ??
		       (IHaveEffects?)Gameworld.Zones.Get(ownerId.Value) ??
		       Gameworld.Shards.Get(ownerId.Value);
	}

	private IEnumerable<IHaveEffects> EnumerateEffectOwners()
	{
		return Gameworld.Characters
			.Cast<object>()
			.Concat(Gameworld.Items)
			.Concat(Gameworld.Cells)
			.Concat(Gameworld.Zones)
			.Concat(Gameworld.Shards)
			.OfType<IHaveEffects>();
	}

	private bool TryResolveOutfitArgument(JsonElement element, out IOutfit? outfit, out string error)
	{
		outfit = null;
		if (element.ValueKind != JsonValueKind.Object)
		{
			error = "Outfit arguments must be an object with OwnerCharacterId and OutfitName.";
			return false;
		}

		if (!TryGetOptionalLong(element, out var ownerId, "OwnerCharacterId", "CharacterId", "OwnerId"))
		{
			error = "Outfit arguments must include OwnerCharacterId.";
			return false;
		}

		var outfitName = TryGetOptionalString(element, "OutfitName", "Name");
		if (string.IsNullOrWhiteSpace(outfitName))
		{
			error = "Outfit arguments must include OutfitName.";
			return false;
		}

		var owner = Gameworld.TryGetCharacter(ownerId, true);
		if (owner is null)
		{
			error = $"No character with id {ownerId:N0} exists.";
			return false;
		}

		outfit = owner.Outfits.FirstOrDefault(x => x.Name.EqualTo(outfitName));
		if (outfit is null)
		{
			error = $"Character {ownerId:N0} has no outfit named '{outfitName}'.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private bool TryResolveOutfitItemArgument(JsonElement element, out IOutfitItem? outfitItem, out string error)
	{
		outfitItem = null;
		if (element.ValueKind != JsonValueKind.Object)
		{
			error = "OutfitItem arguments must be an object with OwnerCharacterId and ItemId.";
			return false;
		}

		if (!TryGetOptionalLong(element, out var ownerId, "OwnerCharacterId", "CharacterId", "OwnerId"))
		{
			error = "OutfitItem arguments must include OwnerCharacterId.";
			return false;
		}

		if (!TryGetOptionalLong(element, out var itemId, "ItemId", "Id", "ID", "id"))
		{
			error = "OutfitItem arguments must include ItemId.";
			return false;
		}

		var outfitName = TryGetOptionalString(element, "OutfitName", "Name");
		var owner = Gameworld.TryGetCharacter(ownerId, true);
		if (owner is null)
		{
			error = $"No character with id {ownerId:N0} exists.";
			return false;
		}

		var outfitCandidates = string.IsNullOrWhiteSpace(outfitName)
			? owner.Outfits
			: owner.Outfits.Where(x => x.Name.EqualTo(outfitName)).ToList();
		if (!outfitCandidates.Any())
		{
			error = string.IsNullOrWhiteSpace(outfitName)
				? $"Character {ownerId:N0} has no outfits."
				: $"Character {ownerId:N0} has no outfit named '{outfitName}'.";
			return false;
		}

		var matches = outfitCandidates
			.SelectMany(x => x.Items)
			.Where(x => x.Id == itemId)
			.ToList();
		if (matches.Count == 0)
		{
			error = $"No outfit item with id {itemId:N0} was found for character {ownerId:N0}.";
			return false;
		}

		if (matches.Count > 1 && string.IsNullOrWhiteSpace(outfitName))
		{
			error =
				$"Outfit item id {itemId:N0} appears in multiple outfits for character {ownerId:N0}. Include OutfitName.";
			return false;
		}

		outfitItem = matches[0];
		error = string.Empty;
		return true;
	}


}
