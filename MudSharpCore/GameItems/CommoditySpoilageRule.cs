#nullable enable

using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbCommoditySpoilageRule = MudSharp.Models.CommoditySpoilageRule;

namespace MudSharp.GameItems;

public sealed class CommoditySpoilageRule : SaveableItem, ICommoditySpoilageRule
{
	public CommoditySpoilageRule(DbCommoditySpoilageRule dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Description = dbitem.Description;
		Enabled = dbitem.Enabled;
		Priority = dbitem.Priority;
		Material = dbitem.MaterialId is null ? null : gameworld.Materials.Get(dbitem.MaterialId.Value);
		MaterialTag = dbitem.MaterialTagId is null ? null : gameworld.Tags.Get(dbitem.MaterialTagId.Value);
		CommodityTag = dbitem.CommodityTagId is null ? null : gameworld.Tags.Get(dbitem.CommodityTagId.Value);
		ResultMaterial = gameworld.Materials.Get(dbitem.ResultMaterialId) ??
		                 throw new ApplicationException(
			                 $"Commodity spoilage rule #{dbitem.Id:N0} refers to missing result material #{dbitem.ResultMaterialId:N0}.");
		ResultCommodityTag = dbitem.ResultCommodityTagId is null ? null : gameworld.Tags.Get(dbitem.ResultCommodityTagId.Value);
		SecondsUntilSpoiled = TimeSpan.FromSeconds(dbitem.SecondsUntilSpoiled);
		SpoilEcho = dbitem.SpoilEcho;
	}

	public CommoditySpoilageRule(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		Description = "A commodity spoilage rule.";
		Enabled = true;
		Priority = 0;
		SecondsUntilSpoiled = TimeSpan.FromDays(1);
		ResultMaterial = gameworld.Materials.GetByIdOrName("meat") ??
		                 gameworld.Materials.FirstOrDefault() ??
		                 throw new ApplicationException(
			                 "Cannot create a commodity spoilage rule because there are no solid materials loaded.");

		using (new FMDB())
		{
			var dbitem = new DbCommoditySpoilageRule
			{
				Name = Name,
				Description = Description,
				Enabled = Enabled,
				Priority = Priority,
				ResultMaterialId = ResultMaterial.Id,
				SecondsUntilSpoiled = (long)SecondsUntilSpoiled.TotalSeconds
			};
			FMDB.Context.CommoditySpoilageRules.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private CommoditySpoilageRule(CommoditySpoilageRule rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		Description = rhs.Description;
		Enabled = rhs.Enabled;
		Priority = rhs.Priority;
		Material = rhs.Material;
		MaterialTag = rhs.MaterialTag;
		CommodityTag = rhs.CommodityTag;
		ResultMaterial = rhs.ResultMaterial;
		ResultCommodityTag = rhs.ResultCommodityTag;
		SecondsUntilSpoiled = rhs.SecondsUntilSpoiled;
		SpoilEcho = rhs.SpoilEcho;

		using (new FMDB())
		{
			var dbitem = new DbCommoditySpoilageRule
			{
				Name = Name,
				Description = Description,
				Enabled = Enabled,
				Priority = Priority,
				MaterialId = Material?.Id,
				MaterialTagId = MaterialTag?.Id,
				CommodityTagId = CommodityTag?.Id,
				ResultMaterialId = ResultMaterial.Id,
				ResultCommodityTagId = ResultCommodityTag?.Id,
				SecondsUntilSpoiled = (long)SecondsUntilSpoiled.TotalSeconds,
				SpoilEcho = SpoilEcho
			};
			FMDB.Context.CommoditySpoilageRules.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "CommoditySpoilageRule";

	public string Description { get; set; }
	public bool Enabled { get; set; }
	public int Priority { get; set; }
	public ISolid? Material { get; set; }
	public ITag? MaterialTag { get; set; }
	public ITag? CommodityTag { get; set; }
	public ISolid ResultMaterial { get; set; }
	public ITag? ResultCommodityTag { get; set; }
	public TimeSpan SecondsUntilSpoiled { get; set; }
	public string? SpoilEcho { get; set; }

	public IEnumerable<string> ValidationWarnings
	{
		get
		{
			if ((Material is null) == (MaterialTag is null))
			{
				yield return "Exactly one of material or material tag must be set.";
			}

			if (ResultMaterial is null)
			{
				yield return "A result material must be set.";
			}

			if (SecondsUntilSpoiled <= TimeSpan.Zero)
			{
				yield return "The spoilage duration must be positive.";
			}
		}
	}

	public ICommoditySpoilageRule Clone(string newName)
	{
		return new CommoditySpoilageRule(this, newName);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.CommoditySpoilageRules.Find(Id) ??
		             throw new ApplicationException($"Unable to find commodity spoilage rule #{Id:N0} to save.");
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.Enabled = Enabled;
		dbitem.Priority = Priority;
		dbitem.MaterialId = Material?.Id;
		dbitem.MaterialTagId = MaterialTag?.Id;
		dbitem.CommodityTagId = CommodityTag?.Id;
		dbitem.ResultMaterialId = ResultMaterial.Id;
		dbitem.ResultCommodityTagId = ResultCommodityTag?.Id;
		dbitem.SecondsUntilSpoiled = (long)SecondsUntilSpoiled.TotalSeconds;
		dbitem.SpoilEcho = SpoilEcho;
		Changed = false;
	}

	public int MatchSpecificity(ICommodity commodity)
	{
		if (!Enabled)
		{
			return -1;
		}

		var specificity = -1;
		if (Material is not null)
		{
			if (commodity.Material != Material)
			{
				return -1;
			}

			specificity = 2;
		}
		else if (MaterialTag is not null)
		{
			if (commodity.Material?.IsA(MaterialTag) != true)
			{
				return -1;
			}

			specificity = 1;
		}

		if (specificity < 0)
		{
			return -1;
		}

		if (CommodityTag is not null)
		{
			if (commodity.Tag?.IsA(CommodityTag) != true)
			{
				return -1;
			}

			specificity += 2;
		}

		return specificity;
	}

	public bool HasCompatibleResult(ICommoditySpoilageRule? other)
	{
		return other is null ||
		       (ResultMaterial == other.ResultMaterial && ResultCommodityTag == other.ResultCommodityTag);
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{"Commodity Spoilage Rule".ColourName()} #{Id.ToString("N0", actor).ColourValue()}: {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Enabled: {Enabled.ToColouredString()}");
		sb.AppendLine($"Priority: {Priority.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Matches: {MatchSideDescription(actor)}");
		sb.AppendLine($"Output: {ResultMaterial.Name.Colour(ResultMaterial.ResidueColour)}{(ResultCommodityTag is not null ? $" tagged {ResultCommodityTag.FullName.ColourName()}" : "")}");
		sb.AppendLine($"Duration: {SecondsUntilSpoiled.Describe(actor).ColourValue()}");
		sb.AppendLine($"Echo: {(string.IsNullOrWhiteSpace(SpoilEcho) ? "None".ColourError() : SpoilEcho.ColourCommand())}");
		var warnings = ValidationWarnings.ToList();
		if (warnings.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Validation Warnings:".ColourError());
			foreach (var warning in warnings)
			{
				sb.AppendLine($"\t{warning.ColourError()}");
			}
		}

		return sb.ToString();
	}

	private string MatchSideDescription(ICharacter actor)
	{
		var match = Material is not null
			? $"material {Material.Name.Colour(Material.ResidueColour)}"
			: MaterialTag is not null
				? $"material tagged {MaterialTag.FullName.ColourName()}"
				: "nothing".ColourError();
		if (CommodityTag is not null)
		{
			match = $"{match} and commodity tag {CommodityTag.FullName.ColourName()}";
		}

		return match;
	}

	private const string HelpText = @"You can use the following options with this rule:

	#3name <name>#0 - renames this rule
	#3desc <description>#0 - sets the builder description
	#3enabled#0 - toggles whether the rule applies
	#3priority <number>#0 - sets match priority, with lower numbers winning ties
	#3material <material|none>#0 - sets or clears the exact material side
	#3materialtag <tag|none>#0 - sets or clears the material-tag side
	#3tag <tag|none>#0 - sets or clears the optional commodity pile tag
	#3resultmaterial <material>#0 - sets the material produced on spoilage
	#3resulttag <tag|none>#0 - sets or clears the result commodity pile tag
	#3seconds <duration>#0 - sets the spoilage time
	#3echo <text|none>#0 - sets or clears the optional spoil echo";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "set":
				return BuildingCommand(actor, command);
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "enabled":
			case "enable":
				return BuildingCommandEnabled(actor, command);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "material":
				return BuildingCommandMaterial(actor, command);
			case "materialtag":
			case "material tag":
			case "mattag":
				return BuildingCommandMaterialTag(actor, command);
			case "tag":
			case "commoditytag":
			case "commodity tag":
				return BuildingCommandCommodityTag(actor, command);
			case "resultmaterial":
			case "result material":
			case "resultmat":
				return BuildingCommandResultMaterial(actor, command);
			case "resulttag":
			case "result tag":
				return BuildingCommandResultTag(actor, command);
			case "seconds":
			case "duration":
			case "time":
				return BuildingCommandSeconds(actor, command);
			case "echo":
				return BuildingCommandEcho(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this commodity spoilage rule?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.CommoditySpoilageRules.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a commodity spoilage rule named {name.ColourName()}.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This commodity spoilage rule is now called {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give this commodity spoilage rule?");
			return false;
		}

		Description = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("You change the builder description for this commodity spoilage rule.");
		return true;
	}

	private bool BuildingCommandEnabled(ICharacter actor, StringStack command)
	{
		if (!command.IsFinished && bool.TryParse(command.SafeRemainingArgument, out var value))
		{
			Enabled = value;
		}
		else if (!command.IsFinished && command.SafeRemainingArgument.EqualToAny("on", "yes", "enable", "enabled"))
		{
			Enabled = true;
		}
		else if (!command.IsFinished && command.SafeRemainingArgument.EqualToAny("off", "no", "disable", "disabled"))
		{
			Enabled = false;
		}
		else
		{
			Enabled = !Enabled;
		}

		Changed = true;
		actor.OutputHandler.Send($"This commodity spoilage rule is now {(Enabled ? "enabled" : "disabled").ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("What priority value do you want to give this rule?");
			return false;
		}

		Priority = value;
		Changed = true;
		actor.OutputHandler.Send($"This rule now has priority {Priority.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which exact material should this rule match? Use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete", "remove"))
		{
			Material = null;
			Changed = true;
			actor.OutputHandler.Send("This rule no longer matches an exact material.");
			return true;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material is null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		Material = material;
		MaterialTag = null;
		Changed = true;
		actor.OutputHandler.Send($"This rule now matches the exact material {material.Name.Colour(material.ResidueColour)}.");
		return true;
	}

	private bool BuildingCommandMaterialTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which material tag should this rule match? Use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete", "remove"))
		{
			MaterialTag = null;
			Changed = true;
			actor.OutputHandler.Send("This rule no longer matches a material tag.");
			return true;
		}

		var tag = LookupOneTag(actor, command.SafeRemainingArgument);
		if (tag is null)
		{
			return false;
		}

		MaterialTag = tag;
		Material = null;
		Changed = true;
		actor.OutputHandler.Send($"This rule now matches materials tagged {tag.FullName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandCommodityTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which commodity pile tag should narrow this rule? Use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete", "remove"))
		{
			CommodityTag = null;
			Changed = true;
			actor.OutputHandler.Send("This rule no longer narrows by commodity pile tag.");
			return true;
		}

		var tag = LookupOneTag(actor, command.SafeRemainingArgument);
		if (tag is null)
		{
			return false;
		}

		CommodityTag = tag;
		Changed = true;
		actor.OutputHandler.Send($"This rule now narrows to commodity piles tagged {tag.FullName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandResultMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material should matching commodities become when they spoil?");
			return false;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material is null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		ResultMaterial = material;
		Changed = true;
		actor.OutputHandler.Send($"This rule now spoils commodities into {material.Name.Colour(material.ResidueColour)}.");
		return true;
	}

	private bool BuildingCommandResultTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which commodity tag should spoiled commodities receive? Use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete", "remove"))
		{
			ResultCommodityTag = null;
			Changed = true;
			actor.OutputHandler.Send("Spoiled commodities will no longer receive a commodity tag.");
			return true;
		}

		var tag = LookupOneTag(actor, command.SafeRemainingArgument);
		if (tag is null)
		{
			return false;
		}

		ResultCommodityTag = tag;
		Changed = true;
		actor.OutputHandler.Send($"Spoiled commodities will now receive the {tag.FullName.ColourName()} tag.");
		return true;
	}

	private bool BuildingCommandSeconds(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should commodities take to spoil under this rule?");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value) || (TimeSpan)value <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send("That is not a valid positive duration.");
			return false;
		}

		SecondsUntilSpoiled = value;
		Changed = true;
		actor.OutputHandler.Send($"This rule now spoils commodities after {SecondsUntilSpoiled.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What echo should show when this rule spoils a commodity? Use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete", "remove"))
		{
			SpoilEcho = null;
			Changed = true;
			actor.OutputHandler.Send("This rule no longer emits a spoilage echo.");
			return true;
		}

		SpoilEcho = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This rule now emits: {SpoilEcho.ColourCommand()}");
		return true;
	}

	private ITag? LookupOneTag(ICharacter actor, string text)
	{
		var tags = Gameworld.Tags.FindMatchingTags(text);
		if (tags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return null;
		}

		if (tags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{tags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return null;
		}

		return tags.Single();
	}
}
