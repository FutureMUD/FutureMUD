#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using EditableItem = MudSharp.Framework.Revision.EditableItem;
using DbLootTable = MudSharp.Models.LootTable;

namespace MudSharp.Work.Loot;

public sealed class LootTable : EditableItem, ILootTable
{
	public const int CurrentAlgorithm = LootTableDefinition.CurrentAlgorithmVersion;

	public LootTable(DbLootTable row, IFuturemud gameworld) : base(row.EditableItem)
	{
		Gameworld = gameworld;
		_id = row.Id;
		_name = row.Name;
		Definition = LootTableDefinition.Load(row.Definition);
		if (row.AlgorithmVersion != Definition.AlgorithmVersion)
		{
			throw new InvalidOperationException($"LootTable #{row.Id}r{row.RevisionNumber} algorithm column does not match its canonical definition.");
		}
	}

	public LootTable(IAccount originator, string name) : base(originator)
	{
		Gameworld = originator.Gameworld;
		_name = name.TitleCase();
		Definition = new LootTableDefinition();
		Definition.Variants.Add(new LootVariantDefinition { Key = "default" });
		using (new FMDB())
		{
			var row = new DbLootTable
			{
				Id = Gameworld.LootTables.NextID(),
				RevisionNumber = 0,
				Name = _name,
				AlgorithmVersion = AlgorithmVersion,
				Definition = Definition.ToCanonicalXml(),
				EditableItem = new Models.EditableItem
				{
					BuilderAccountId = BuilderAccountID,
					BuilderDate = BuilderDate,
					RevisionNumber = 0,
					RevisionStatus = (int)Status
				}
			};
			FMDB.Context.LootTables.Add(row);
			FMDB.Context.SaveChanges();
			_id = row.Id;
		}
	}

	public override string FrameworkItemType => "LootTable";
	public int AlgorithmVersion => Definition.AlgorithmVersion;
	public LootTableDefinition Definition { get; private set; }
	public string DefinitionHash => Definition.ComputeHash();

	public override string EditHeader() => $"Loot Table {Name} ({Id:N0}r{RevisionNumber:N0})";

	public override bool CanSubmit() => LootTableValidator.Validate(this, Gameworld).Count == 0;
	public override string WhyCannotSubmit() =>
		string.Join("\n", LootTableValidator.Validate(this, Gameworld).Select(x => $"- {x}"));

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Loot Table #{Id.ToStringN0(actor)}r{RevisionNumber.ToStringN0(actor)} - {Name.ColourName()} - {Status.DescribeColour()}");
		sb.AppendLine($"Algorithm: {AlgorithmVersion.ToString().ColourValue()}");
		sb.AppendLine($"Definition Hash: {DefinitionHash.ColourValue()}");
		foreach (var variant in Definition.Variants)
		{
			sb.AppendLine($"Variant {variant.Key.ColourName()}");
			for (var i = 0; i < variant.Groups.Count; i++)
			{
				var group = variant.Groups[i];
				sb.AppendLine($"  {i + 1}. {group.Key.ColourName()} repeat {group.RepeatMinimum}-{group.RepeatMaximum} into {group.DestinationKey.ColourValue()}");
				var total = group.Choices.Sum(x => x.Weight);
				for (var j = 0; j < group.Choices.Count; j++)
				{
					var choice = group.Choices[j];
					sb.AppendLine($"     {j + 1}. {choice.Key.ColourName()} weight {choice.Weight:N0} ({(total > 0 ? (double)choice.Weight / total : 0.0):P2}) {DescribeChoice(choice)}");
				}
			}
		}
		var errors = LootTableValidator.Validate(this, Gameworld);
		if (errors.Count > 0)
		{
			sb.AppendLine("Validation errors:".ColourError());
			foreach (var error in errors) sb.AppendLine($"  - {error}".ColourError());
		}
		return sb.ToString();
	}

	private static string DescribeChoice(LootChoiceDefinition choice) => choice.Kind switch
	{
		LootChoiceKind.Nothing => "nothing",
		LootChoiceKind.Item => $"item #{choice.ItemPrototypeId}r{choice.ItemPrototypeRevision} x{choice.QuantityMinimum}-{choice.QuantityMaximum} quality {choice.QualityMinimum}-{choice.QualityMaximum}{(string.IsNullOrEmpty(choice.ResultKey) ? "" : $" as {choice.ResultKey}")}",
		LootChoiceKind.Commodity => $"commodity material #{choice.CommodityMaterialId}{(choice.CommodityTagId is null ? "" : $" tag #{choice.CommodityTagId}")} mass {choice.MassMinimum:R}-{choice.MassMaximum:R}",
		LootChoiceKind.LootTable => $"table #{choice.NestedTableId}r{choice.NestedTableRevision} variant {choice.NestedVariant}",
		_ => "unknown"
	};

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var revision = FMDB.Context.LootTables.Where(x => x.Id == Id).Select(x => x.RevisionNumber)
			                     .AsEnumerable().DefaultIfEmpty(-1).Max() + 1;
			var row = new DbLootTable
			{
				Id = Id,
				RevisionNumber = revision,
				Name = Name,
				AlgorithmVersion = AlgorithmVersion,
				Definition = Definition.ToCanonicalXml(),
				EditableItem = new Models.EditableItem
				{
					BuilderAccountId = initiator.Account.Id,
					BuilderDate = DateTime.UtcNow,
					RevisionNumber = revision,
					RevisionStatus = (int)RevisionStatus.UnderDesign
				}
			};
			FMDB.Context.LootTables.Add(row);
			FMDB.Context.SaveChanges();
			return new LootTable(row, Gameworld);
		}
	}

	public LootTable Clone(IAccount originator, string name)
	{
		var clone = new LootTable(originator, name) { Definition = Definition.Clone() };
		clone.Changed = true;
		return clone;
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var row = FMDB.Context.LootTables.Find(Id, RevisionNumber);
			if (row is null) return;
			if (_statusChanged) base.Save(row.EditableItem);
			row.Name = Name;
			row.AlgorithmVersion = AlgorithmVersion;
			row.Definition = Definition.ToCanonicalXml();
			FMDB.Context.SaveChanges();
		}
		Changed = false;
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId() => Gameworld.LootTables.GetAll(Id);

	private const string BuildingHelp = @"Use NAME <name>, VARIANT ADD|REMOVE|RENAME, GROUP ADD|REPEAT|DESTINATION|SWAP|REMOVE, CHOICE ADD|WEIGHT|VARIABLES|REMOVE. See HELP LOOTTABLE for syntax.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopSpeech().ToLowerInvariant();
		return verb switch
		{
			"name" => SetName(actor, command),
			"variant" => SetVariant(actor, command),
			"group" => SetGroup(actor, command),
			"choice" => SetChoice(actor, command),
			_ => SendHelp(actor)
		};
	}

	private static bool SendHelp(ICharacter actor) { actor.Send(BuildingHelp); return false; }
	private bool SetName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished) { actor.Send("What name should this loot table have?"); return false; }
		_name = command.SafeRemainingArgument.TitleCase(); Changed = true; actor.Send($"Name set to {Name.ColourName()}."); return true;
	}

	private bool SetVariant(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var key = command.PopSpeech().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(key)) return SendHelp(actor);
		var existing = Definition.Variants.FirstOrDefault(x => x.Key.EqualTo(key));
		switch (action)
		{
			case "add" when existing is null: Definition.Variants.Add(new LootVariantDefinition { Key = key }); break;
			case "remove" when existing is not null: Definition.Variants.Remove(existing); break;
			case "rename" when existing is not null && !command.IsFinished: existing.Key = command.PopSpeech().ToLowerInvariant(); break;
			default: actor.Send("That variant action is invalid or conflicts with an existing/missing variant."); return false;
		}
		Changed = true; actor.Send("Variant updated."); return true;
	}

	private bool SetGroup(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var variant = Definition.Variants.FirstOrDefault(x => x.Key.EqualTo(command.PopSpeech()));
		if (variant is null) { actor.Send("There is no such variant."); return false; }
		var key = command.PopSpeech();
		var group = variant.Groups.FirstOrDefault(x => x.Key.EqualTo(key));
		if (action == "add")
		{
			if (group is not null || string.IsNullOrWhiteSpace(key)) { actor.Send("That group already exists or has no key."); return false; }
			group = new LootRollGroupDefinition { Key = key.ToLowerInvariant() };
			variant.Groups.Add(group);
			while (!command.IsFinished)
			{
				switch (command.PopSpeech().ToLowerInvariant())
				{
					case "repeat" when int.TryParse(command.PopSpeech(), out var min) && int.TryParse(command.PopSpeech(), out var max): group.RepeatMinimum = min; group.RepeatMaximum = max; break;
					case "into" when !command.IsFinished: group.DestinationKey = command.PopSpeech().ToLowerInvariant(); break;
				}
			}
		}
		else if (group is null) { actor.Send("There is no such group."); return false; }
		else if (action == "remove") variant.Groups.Remove(group);
		else if (action == "repeat" && int.TryParse(command.PopSpeech(), out var min) && int.TryParse(command.PopSpeech(), out var max)) { group.RepeatMinimum = min; group.RepeatMaximum = max; }
		else if (action == "destination" && !command.IsFinished) group.DestinationKey = command.PopSpeech().ToLowerInvariant();
		else if (action == "swap" && int.TryParse(command.PopSpeech(), out var position) && position >= 1 && position <= variant.Groups.Count) { variant.Groups.Remove(group); variant.Groups.Insert(position - 1, group); }
		else { actor.Send("Invalid group action."); return false; }
		Changed = true; actor.Send("Group updated."); return true;
	}

	private bool SetChoice(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var variant = Definition.Variants.FirstOrDefault(x => x.Key.EqualTo(command.PopSpeech()));
		var groupKey = command.PopSpeech();
		var group = variant?.Groups.FirstOrDefault(x => x.Key.EqualTo(groupKey));
		if (group is null) { actor.Send("There is no such variant/group."); return false; }
		var key = command.PopSpeech().ToLowerInvariant();
		var choice = group.Choices.FirstOrDefault(x => x.Key.EqualTo(key));
		if (action == "remove" && choice is not null) group.Choices.Remove(choice);
		else if (action == "weight" && choice is not null && long.TryParse(command.PopSpeech(), out var weight)) choice.Weight = weight;
		else if (action == "variables" && choice is not null && choice.Kind == LootChoiceKind.Item)
		{
			choice.Characteristics.Clear();
			if (!command.PeekSpeech().EqualTo("clear"))
			{
				while (!command.IsFinished)
				{
					var pair = command.PopSpeech().Split('=', 2);
					if (pair.Length != 2 || !long.TryParse(pair[0], out var definition) || !long.TryParse(pair[1], out var value)) { actor.Send("Variables must be exact numeric definition=value pairs."); return false; }
					choice.Characteristics.Add(new LootCharacteristicValue { DefinitionId = definition, ValueId = value });
				}
			}
		}
		else if (action == "add" && choice is null && long.TryParse(command.PopSpeech(), out var newWeight))
		{
			var kind = command.PopSpeech().ToLowerInvariant();
			choice = new LootChoiceDefinition { Key = key, Weight = newWeight };
			switch (kind)
			{
				case "nothing": choice.Kind = LootChoiceKind.Nothing; break;
				case "item" when long.TryParse(command.PopSpeech(), out var proto): choice.Kind = LootChoiceKind.Item; choice.ItemPrototypeId = proto; choice.ItemPrototypeRevision = Gameworld.ItemProtos.Get(proto)?.RevisionNumber ?? 0; ParseItemOptions(choice, command); break;
				case "commodity" when long.TryParse(command.PopSpeech(), out var material): choice.Kind = LootChoiceKind.Commodity; choice.CommodityMaterialId = material; ParseCommodityOptions(choice, command); break;
				case "table" when long.TryParse(command.PopSpeech(), out var table) && int.TryParse(command.PopSpeech(), out var revision) && !command.IsFinished: choice.Kind = LootChoiceKind.LootTable; choice.NestedTableId = table; choice.NestedTableRevision = revision; choice.NestedVariant = command.PopSpeech().ToLowerInvariant(); break;
				default: actor.Send("Invalid choice type or arguments."); return false;
			}
			group.Choices.Add(choice);
		}
		else { actor.Send("Invalid choice action."); return false; }
		Changed = true; actor.Send("Choice updated."); return true;
	}

	private static void ParseItemOptions(LootChoiceDefinition choice, StringStack command)
	{
		while (!command.IsFinished)
		{
			switch (command.PopSpeech().ToLowerInvariant())
			{
				case "revision" when int.TryParse(command.PopSpeech(), out var revision): choice.ItemPrototypeRevision = revision; break;
				case "quantity" when int.TryParse(command.PopSpeech(), out var min) && int.TryParse(command.PopSpeech(), out var max): choice.QuantityMinimum = min; choice.QuantityMaximum = max; break;
				case "quality" when int.TryParse(command.PopSpeech(), out var min) && int.TryParse(command.PopSpeech(), out var max): choice.QualityMinimum = min; choice.QualityMaximum = max; break;
				case "as" when !command.IsFinished: choice.ResultKey = command.PopSpeech().ToLowerInvariant(); break;
			}
		}
	}

	private static void ParseCommodityOptions(LootChoiceDefinition choice, StringStack command)
	{
		while (!command.IsFinished)
		{
			switch (command.PopSpeech().ToLowerInvariant())
			{
				case "tag" when long.TryParse(command.PopSpeech(), out var tag): choice.CommodityTagId = tag; break;
				case "mass" when double.TryParse(command.PopSpeech(), out var min) && double.TryParse(command.PopSpeech(), out var max): choice.MassMinimum = min; choice.MassMaximum = max; break;
			}
		}
	}
}
