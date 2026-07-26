#nullable enable

using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;


public partial class ItemSeeder
{
	private static TValue RequireKey<TValue>(
		IReadOnlyDictionary<string, TValue> dictionary,
		string key,
		string owner,
		string kind)
	{
		return dictionary.TryGetValue(key, out var value)
			? value
			: throw new InvalidOperationException($"{owner} references missing {kind} key {key}.");
	}

	private long NextVehiclePrototypeId()
	{
		var existing = _context!.VehicleProtos.Any() ? _context.VehicleProtos.Max(x => x.Id) : 0L;
		var local = _context.VehicleProtos.Local.Any() ? _context.VehicleProtos.Local.Max(x => x.Id) : 0L;
		return Math.Max(existing, local) + 1L;
	}

	private static string ComponentName(string role, string stableReference, string? childKey = null)
	{
		var value = childKey is null
			? $"VS_{role}_{stableReference}"
			: $"VS_{role}_{stableReference}_{childKey}";
		return Regex.Replace(value, "[^A-Za-z0-9_]+", "_");
	}

	private static string NormaliseKey(string value)
	{
		return Regex.Replace(value.ToLowerInvariant(), "[^a-z0-9]+", "_").Trim('_');
	}

	private static string VehicleChildMarker(string vehicleReference, string kind, string key)
	{
		return $"[{VehicleSeederMarkerPrefix}: {vehicleReference}/{kind}/{NormaliseKey(key)}]";
	}

	private static string WithVehicleSeederMarker(string description, string vehicleReference, string kind, string key)
	{
		var marker = VehicleChildMarker(vehicleReference, kind, key);
		return description.Contains(marker, StringComparison.OrdinalIgnoreCase)
			? description
			: $"{description.Trim()}\n{marker}";
	}

}
