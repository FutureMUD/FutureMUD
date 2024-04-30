using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.OpenAI;
using MudSharp.PerceptionEngine;
using EditableItem = MudSharp.Framework.Revision.EditableItem;
using Material = MudSharp.Form.Material.Material;

namespace MudSharp.GameItems;

public class GameItemProto : EditableItem, IGameItemProto
{
	public List<IFutureProg> OnLoadProgs = new();

	public GameItemProto(MudSharp.Models.GameItemProto proto, IFuturemud gameworld)
		: base(proto.EditableItem)
	{
		LoadFromDatabase(proto, gameworld);
	}

	public GameItemProto(IFuturemud gameworld, IAccount originator)
		: base(originator)
	{
		using (new FMDB())
		{
			var dbproto = new Models.GameItemProto
			{
				Id = gameworld.ItemProtos.NextID(),
				RevisionNumber = RevisionNumber,
				MaterialId = 0, // TODO
				Keywords = "new object",
				Name = "object",
				Size = (int)SizeCategory.Normal,
				Weight = 1.0
			};
			dbproto.MaterialId = 0;
			dbproto.BaseItemQuality = (int)ItemQuality.Standard;
			dbproto.MorphEmote = "$0 $?1|morphs into $1|decays into nothing|$.";

			dbproto.ShortDescription = "a new object";
			dbproto.FullDescription = "A new object that should not be loaded into the game.";

			if (gameworld.GetStaticBool("AutomaticHoldableItemProtos"))
			{
				var holdable = gameworld.ItemComponentProtos.First(x => x is HoldableGameItemComponentProto);
				dbproto.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
				{
					GameItemProto = dbproto,
					GameItemComponentProtoId = holdable.Id,
					GameItemComponentRevision = holdable.RevisionNumber
				});
			}

			dbproto.EditableItem = new Models.EditableItem
			{
				BuilderAccountId = BuilderAccountID,
				BuilderDate = BuilderDate,
				RevisionStatus = (int)Status
			};
			FMDB.Context.EditableItems.Add(dbproto.EditableItem);
			FMDB.Context.GameItemProtos.Add(dbproto);
			FMDB.Context.SaveChanges();

			LoadFromDatabase(dbproto, gameworld);
		}
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.ItemProtos.GetAll(Id);
	}

	public static IGameItemGroup TooManyItemsGroup { get; set; }
	public static IHealthStrategy DefaultItemHealthStrategy { get; set; }
	public string ShortDescription { get; private set; }
	public string FullDescription { get; private set; }
	public decimal CostInBaseCurrency { get; private set; }

	private readonly
		List<(IFutureProg Prog, string? ShortDescription, string? FullDescription, string? FullDescriptionAddendum)>
		_extraDescriptions = new();

	public IEnumerable<(IFutureProg Prog, string? ShortDescription, string? FullDescription, string?
		FullDescriptionAddendum)> ExtraDescriptions => _extraDescriptions;

	private bool _extraDescriptionsChanged;

	public bool ExtraDescriptionsChanged
	{
		get => _extraDescriptionsChanged;
		set
		{
			_extraDescriptionsChanged = value;
			if (value)
			{
				Changed = true;
			}
		}
	}

	public override string FrameworkItemType => "GameItemProto";

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		var maxRevision = Gameworld.ItemProtos.GetAll(Id).Max(x => x.RevisionNumber);
		sb.AppendLine($"Item Prototype #{Id.ToString("N0", actor)} - Revision {RevisionNumber.ToString("N0", actor)} / {maxRevision.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Status: {Status.DescribeColour()}");
		sb.AppendLine(
			$"Noun: {Name.Proper().ColourValue()}");
		sb.AppendLine($"Short Description: {ShortDescription.ColourObject()}");
		if (OverridesLongDescription)
		{
			sb.AppendLineFormat(actor, "Long Description: {0}", LongDescription.Proper().ColourObject());
		}
		else
		{
			sb.AppendLine($"Long Description: {"Default".ColourCommand()}");
		}

		sb.AppendLine(
			$"Item Group: {(ItemGroup is null ? "Default".ColourCommand() : $"{ItemGroup.Name.ColourValue()} ({ItemGroup.Id.ToString("N0", actor)})".ColourValue())}");
		if (_onDestroyedGameItemProto != 0)
		{
			sb.AppendLineFormat(actor, "Destroyed Item: {0}",
				_onDestroyedGameItemProto.ToString("N0", actor).ColourValue());
		}

		if (_healthStrategy == null)
		{
			sb.AppendLine($"Health Strategy: {"Default".ColourCommand()}");
		}
		else
		{
			sb.AppendLine(
				$"Health Strategy: {_healthStrategy.Name.ColourValue()} (#{_healthStrategy.Id.ToString("N0", actor)})");
		}

		if (Morphs)
		{
			sb.AppendLine(
				$"It morphs into {(_onMorphGameItemProto == 0 ? "nothing".Colour(Telnet.Red) : $"Item Proto {_onMorphGameItemProto} ({Gameworld.ItemProtos.Get(_onMorphGameItemProto)?.ShortDescription.Colour(Telnet.BoldWhite) ?? "nothing".Colour(Telnet.Red)})")} after {MorphTimeSpan.Describe(actor)} with the emote: {MorphEmote.Colour(Telnet.Cyan)}.");
		}

		sb.AppendLine($"Size: {Size.Describe().ColourValue()}");
		sb.AppendLine(
			$"Weight: {Gameworld.UnitManager.DescribeMostSignificantExact(Weight, UnitType.Mass, actor).ColourValue()}");
		sb.AppendLine(
			$"Material: {(Material != null ? Material.Name.ColourValue() : "None".Colour(Telnet.Red))}");
		sb.AppendLine($"Permit Player Skins: {PermitPlayerSkins.ToColouredString()}");

		sb.AppendLine($"Base Quality: {BaseItemQuality.Describe().Colour(Telnet.Green)}");
		sb.AppendLine($"Base Cost: {actor.Currency?.Describe(CostInBaseCurrency / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion, CurrencyDescriptionPatternType.ShortDecimal).ColourValue() ?? CostInBaseCurrency.ToString("N", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Full Description:");
		sb.AppendLine();
		sb.AppendLine(FullDescription.Wrap(actor.InnerLineFormatLength, "  "));
		sb.AppendLine();
		sb.AppendLine("Tags:");
		if (Tags.Any())
		{
			sb.AppendLine();
			foreach (var tag in Tags)
			{
				sb.AppendLine($"\t{tag.FullName.ColourName()}");
			}
		}
		else
		{
			sb.AppendLine("\n\tNone".ColourName());
		}

		sb.AppendLine();
		sb.AppendLine("Components:");
		sb.AppendLine();
		if (Components.Any())
		{
			foreach (var component in _components)
			{
				sb.AppendLine($"\t{component.EditHeader()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone");
		}

		if (ExtraDescriptions.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Extra Descriptions:");
			var count = 1;
			foreach (var desc in ExtraDescriptions)
			{
				sb.AppendLine();
				sb.AppendLine(
					$"\t#{count++.ToString("N0", actor)} - Applicability Prog {desc.Prog.MXPClickableFunctionNameWithId()}");
				sb.AppendLine(
					$"\t\tShort Description: {desc.ShortDescription?.SubstituteANSIColour().ColourIncludingReset(CustomColour ?? Telnet.Green) ?? "None".Colour(Telnet.Red)}");
				sb.AppendLine(
					$"\t\tDescription Addendum: {desc.FullDescriptionAddendum?.SubstituteANSIColour() ?? "None".Colour(Telnet.Red)}");
				sb.AppendLine(
					$"\t\tFull Description:\n\n{desc.FullDescription?.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t\t\t") ?? "None".Colour(Telnet.Red)}");
			}
		}

		if (OnLoadProgs.Any())
		{
			sb.AppendLine();
			sb.AppendLine("On Load Progs".Colour(Telnet.Cyan));
			foreach (var prog in OnLoadProgs)
			{
				sb.AppendLine($"\t{prog.MXPClickableFunctionNameWithId()}");
			}
		}

		if (DefaultVariables.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Default Register Values".Colour(Telnet.Cyan));
			foreach (var item in DefaultVariables)
			{
				sb.AppendLineFormat("\t({0}) {1} = {2}",
					Gameworld.VariableRegister.GetType(FutureProgVariableTypes.Item, item.Key).Describe()
					         .Colour(Telnet.Cyan),
					item.Key.Colour(Telnet.Yellow),
					item.Value);
			}
		}

		return sb.ToString();
	}

	public IGameItem CreateNew(ICharacter loader = null)
	{
		var newItem = new GameItem(this, loader, BaseItemQuality);
		foreach (var prog in OnLoadProgs)
		{
			if (
				prog.MatchesParameters(new[]
					{ FutureProgVariableTypes.Item, FutureProgVariableTypes.Character }))
			{
				prog.Execute(newItem, loader);
			}
			else if (prog.MatchesParameters(new[]
			         {
				         FutureProgVariableTypes.Item
			         }))
			{
				prog.Execute(newItem);
			}
			else
			{
				Console.WriteLine("Warning: OnLoadProg {0} #{1:N0} did not match correct paramaters.",
					prog.FunctionName, prog.Id);
			}
		}

		return newItem;
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbnew = new Models.GameItemProto
            {
                Id = Id,
                RevisionNumber =
                    FMDB.Context.GameItemProtos.Where(x => x.Id == Id)
                        .Select(x => x.RevisionNumber)
                        .AsEnumerable()
                        .DefaultIfEmpty(0)
                        .Max() +
                    1,
                Keywords = Keywords.ListToString(separator: " ",
                    conjunction: ""),
                MaterialId = Material?.Id ?? 0,
                EditableItemId = 0,
                Name = Name.Proper(),
                Size = (int)Size,
                Weight = Weight,
                ReadOnly = false,
                LongDescription = LongDescription,
                BaseItemQuality = (int)BaseItemQuality,
                MorphTimeSeconds = (int)MorphTimeSpan.TotalSeconds,
                ItemGroupId = ItemGroup?.Id,
                OnDestroyedGameItemProtoId = _onDestroyedGameItemProto == 0
                    ? null
                    : _onDestroyedGameItemProto,
                HealthStrategyId = HealthStrategy?.Id,
                FullDescription = FullDescription,
                ShortDescription = ShortDescription,
                MorphEmote = MorphEmote,
                PermitPlayerSkins = PermitPlayerSkins,
                MorphGameItemProtoId = _onMorphGameItemProto != 0
                    ? _onMorphGameItemProto
                    : default(long?),
                CustomColour = CustomColour?.Name.ToLowerInvariant() ?? "",
                HighPriority = HighPriority,
                CostInBaseCurrency = CostInBaseCurrency,
            };

			foreach (var tag in Tags)
			{
				dbnew.GameItemProtosTags.Add(new GameItemProtosTags { GameItemProto = dbnew, TagId = tag.Id });
			}

			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

			foreach (var component in _components)
			{
				var dbcomponent = FMDB.Context.GameItemComponentProtos.Find(component.Id, component.RevisionNumber);
				if (dbnew.GameItemProtosGameItemComponentProtos.Any(x =>
					    x.GameItemComponentProtoId == component.Id &&
					    x.GameItemComponentRevision == component.RevisionNumber))
				{
					continue;
				}

				dbnew.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
				{
					GameItemProto = dbnew, GameItemComponentProtoId = component.Id,
					GameItemComponentRevision = component.RevisionNumber
				});
			}

			foreach (var item in DefaultVariables)
			{
				var dbvar = new GameItemProtosDefaultVariable
				{
					VariableName = item.Key,
					VariableValue = item.Value
				};
				dbnew.GameItemProtosDefaultVariables.Add(dbvar);
			}

			var order = 0;
			foreach (var item in ExtraDescriptions)
			{
				dbnew.ExtraDescriptions.Add(new GameItemProtoExtraDescription
				{
					ApplicabilityProgId = item.Prog.Id, ShortDescription = item.ShortDescription,
					FullDescription = item.FullDescription, FullDescriptionAddendum = item.FullDescriptionAddendum,
					Priority = order++
				});
			}

			foreach (var prog in OnLoadProgs)
			{
				var dbprog = FMDB.Context.FutureProgs.Find(prog.Id);
				if (dbprog != null)
				{
					dbnew.GameItemProtosOnLoadProgs.Add(new GameItemProtosOnLoadProgs
						{ GameItemProto = dbnew, FutureProgId = prog.Id });
				}
			}

			FMDB.Context.GameItemProtos.Add(dbnew);
			FMDB.Context.SaveChanges();

			return new GameItemProto(dbnew, Gameworld);
		}
	}

	public IGameItemProto Clone(ICharacter builder)
	{
		using (new FMDB())
		{
			var dbnew = new Models.GameItemProto
			{
				Id = Gameworld.ItemProtos.NextID(),
				RevisionNumber = 0,
				Keywords = Keywords.ListToString(separator: " ",
					conjunction: ""),
				MaterialId = Material?.Id ?? 0,
				Name = Name.Proper(),
				Size = (int)Size,
				Weight = Weight,
				ReadOnly = ReadOnly,
				LongDescription = LongDescription,
				BaseItemQuality = (int)BaseItemQuality,
				MorphTimeSeconds = (int)MorphTimeSpan.TotalSeconds,
				MorphEmote = MorphEmote,
				ShortDescription = ShortDescription,
				FullDescription = FullDescription,
				PermitPlayerSkins = PermitPlayerSkins,
				MorphGameItemProtoId = _onMorphGameItemProto != 0
					? _onMorphGameItemProto
					: default(long?),
				CustomColour = CustomColour?.Name.ToLowerInvariant() ?? "",
				HighPriority = HighPriority,
				ItemGroupId = ItemGroup?.Id,
				HealthStrategyId = HealthStrategy?.Id,
				CostInBaseCurrency = CostInBaseCurrency
			};

			foreach (var tag in Tags)
			{
				dbnew.GameItemProtosTags.Add(new GameItemProtosTags { GameItemProto = dbnew, TagId = tag.Id });
			}

			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = builder.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

			foreach (var component in _components)
			{
				if (dbnew.GameItemProtosGameItemComponentProtos.Any(x =>
					    x.GameItemComponentProtoId == component.Id &&
					    x.GameItemComponentRevision == component.RevisionNumber))
				{
					continue;
				}

				dbnew.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
				{
					GameItemProto = dbnew, GameItemComponentProtoId = component.Id,
					GameItemComponentRevision = component.RevisionNumber
				});
			}

			foreach (var item in DefaultVariables)
			{
				var dbvar = new GameItemProtosDefaultVariable
				{
					VariableName = item.Key,
					VariableValue = item.Value
				};
				dbnew.GameItemProtosDefaultVariables.Add(dbvar);
			}

			var order = 0;
			foreach (var item in ExtraDescriptions)
			{
				dbnew.ExtraDescriptions.Add(new GameItemProtoExtraDescription
				{
					ApplicabilityProgId = item.Prog.Id, ShortDescription = item.ShortDescription,
					FullDescription = item.FullDescription, FullDescriptionAddendum = item.FullDescriptionAddendum,
					Priority = order++
				});
			}

			foreach (var prog in OnLoadProgs)
			{
				var dbprog = FMDB.Context.FutureProgs.Find(prog.Id);
				if (dbprog != null)
				{
					dbnew.GameItemProtosOnLoadProgs.Add(new GameItemProtosOnLoadProgs
						{ GameItemProto = dbnew, FutureProgId = prog.Id });
				}
			}

			FMDB.Context.GameItemProtos.Add(dbnew);
			FMDB.Context.SaveChanges();
			return new GameItemProto(dbnew, Gameworld);
		}
	}

	public override string EditHeader()
	{
		return $"Item Proto #{Id}r{RevisionNumber} ({ShortDescription})";
	}

	public override bool CanSubmit()
	{
		return Material != null;
	}

	public override string WhyCannotSubmit()
	{
		return Material == null ? "You must first set a material." : base.WhyCannotSubmit();
	}

	private void LoadFromDatabase(MudSharp.Models.GameItemProto proto, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = proto.Id;
		_name = proto.Name;
		_keywords = new Lazy<List<string>>(() => (from keyword in proto.Keywords.Split(' ') select keyword).ToList());
		foreach (var component in proto.GameItemProtosGameItemComponentProtos)
		{
			_components.Add(Gameworld.ItemComponentProtos.Get(component.GameItemComponentProtoId,
				component.GameItemComponentRevision));
		}

		ShortDescription = proto.ShortDescription;
		FullDescription = proto.FullDescription;
		LongDescription = proto.LongDescription;
		PermitPlayerSkins = proto.PermitPlayerSkins;
		Size = (SizeCategory)proto.Size;
		Weight = proto.Weight;
		_readOnly = proto.ReadOnly;
		Material = Gameworld.Materials.Get(proto.MaterialId);
		BaseItemQuality = (ItemQuality)proto.BaseItemQuality;
		ItemGroup = gameworld.ItemGroups.Get(proto.ItemGroupId ?? 0);
		HighPriority = proto.HighPriority;
		CustomColour = Telnet.GetColour(proto.CustomColour ?? string.Empty);
		CostInBaseCurrency = proto.CostInBaseCurrency;
		_onDestroyedGameItemProto = proto.OnDestroyedGameItemProtoId ?? 0;
		_healthStrategy = Gameworld.HealthStrategies.Get(proto.HealthStrategyId ?? 0) ??
		                  Gameworld.HealthStrategies.FirstOrDefault(
			                  x => x.OwnerType == HealthStrategyOwnerType.GameItem);

		foreach (var tag in proto.GameItemProtosTags)
		{
			_tags.Add(Gameworld.Tags.Get(tag.TagId));
		}

		foreach (var item in proto.GameItemProtosDefaultVariables)
		{
			_defaultVariables[item.VariableName] = item.VariableValue;
		}

		foreach (var item in proto.ExtraDescriptions.OrderBy(x => x.Priority))
		{
			_extraDescriptions.Add((Gameworld.FutureProgs.Get(item.ApplicabilityProgId), item.ShortDescription,
				item.FullDescription, item.FullDescriptionAddendum));
		}

		foreach (var prog in proto.GameItemProtosOnLoadProgs)
		{
			var fprog = Gameworld.FutureProgs.Get(prog.FutureProgId);
			if (fprog == null ||
			    (!fprog.MatchesParameters(new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Character }) &&
			     !fprog.MatchesParameters(new[] { FutureProgVariableTypes.Item })))
			{
				Console.WriteLine("Warning: OnLoadProg didn't match the right parameters and was skipped over.");
				continue;
			}

			OnLoadProgs.Add(fprog);
		}

		Morphs = proto.MorphTimeSeconds > 0;
		MorphTimeSpan = TimeSpan.FromSeconds(proto.MorphTimeSeconds);
		MorphEmote = proto.MorphEmote;
		_onMorphGameItemProto = proto.MorphGameItemProtoId ?? 0;
	}

	#region ISaveable Members

	public override void Save()
	{
		using (new FMDB())
		{
			var dbproto = FMDB.Context.GameItemProtos.Find(Id, RevisionNumber);
			dbproto.Keywords = Keywords.ListToString(separator: " ", conjunction: "");
			dbproto.Name = Name.Proper();
			dbproto.Weight = Weight;
			dbproto.ShortDescription = ShortDescription;
			dbproto.FullDescription = FullDescription;
			dbproto.PermitPlayerSkins = PermitPlayerSkins;
			dbproto.Size = (int)Size;
			dbproto.MaterialId = Material?.Id ?? 0;
			dbproto.LongDescription = string.IsNullOrEmpty(LongDescription) ? default : LongDescription;
			dbproto.ItemGroupId = ItemGroup?.Id;
			dbproto.HealthStrategyId = _healthStrategy?.Id;
			dbproto.BaseItemQuality = (int)BaseItemQuality;
			dbproto.HighPriority = HighPriority;
			dbproto.CustomColour = CustomColour?.Name.ToLowerInvariant() ?? "";
			dbproto.CostInBaseCurrency = CostInBaseCurrency;
			dbproto.OnDestroyedGameItemProtoId = _onDestroyedGameItemProto != 0
				? _onDestroyedGameItemProto
				: default;

			FMDB.Context.GameItemProtosGameItemComponentProtos.RemoveRange(
				dbproto.GameItemProtosGameItemComponentProtos);
			foreach (var component in _components)
			{
				dbproto.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
				{
					GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber,
					GameItemProto = dbproto
				});
			}

			if (_statusChanged)
			{
				Save(dbproto.EditableItem);
			}

			if (TagsChanged)
			{
				FMDB.Context.GameItemProtosTags.RemoveRange(dbproto.GameItemProtosTags);
				foreach (var tag in Tags)
				{
					dbproto.GameItemProtosTags.Add(new GameItemProtosTags { GameItemProto = dbproto, TagId = tag.Id });
				}

				_tagsChanged = false;
			}

			FMDB.Context.GameItemProtosDefaultVariables.RemoveRange(dbproto.GameItemProtosDefaultVariables);
			foreach (var item in DefaultVariables)
			{
				var dbvar = new GameItemProtosDefaultVariable
				{
					VariableName = item.Key,
					VariableValue = item.Value
				};
				dbproto.GameItemProtosDefaultVariables.Add(dbvar);
			}

			if (ExtraDescriptionsChanged)
			{
				FMDB.Context.GameItemProtoExtraDescriptions.RemoveRange(dbproto.ExtraDescriptions);
				var order = 0;
				foreach (var item in ExtraDescriptions)
				{
					dbproto.ExtraDescriptions.Add(new GameItemProtoExtraDescription
					{
						ApplicabilityProgId = item.Prog.Id, ShortDescription = item.ShortDescription,
						FullDescription = item.FullDescription, FullDescriptionAddendum = item.FullDescriptionAddendum,
						Priority = order++
					});
				}

				ExtraDescriptionsChanged = false;
			}

			FMDB.Context.GameItemProtosOnLoadProgs.RemoveRange(dbproto.GameItemProtosOnLoadProgs);
			foreach (var item in OnLoadProgs)
			{
				var dbprog = FMDB.Context.FutureProgs.Find(item.Id);
				if (dbprog != null)
				{
					dbproto.GameItemProtosOnLoadProgs.Add(new GameItemProtosOnLoadProgs
						{ GameItemProto = dbproto, FutureProgId = item.Id });
				}
			}

			dbproto.MorphTimeSeconds = (int)MorphTimeSpan.TotalSeconds;
			dbproto.MorphGameItemProtoId = _onMorphGameItemProto;
			dbproto.MorphEmote = MorphEmote;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	public override string ToString()
	{
		return $"GameItemProto {Id}r{RevisionNumber} - {ShortDescription}";
	}

	#region IGameItemProto Members

	private IHealthStrategy _healthStrategy;

	public IHealthStrategy HealthStrategy
	{
		get => _healthStrategy ?? DefaultItemHealthStrategy;
		private set
		{
			_healthStrategy = value;
			Changed = true;
		}
	}

	private long _onDestroyedGameItemProto;

	public IGameItem LoadDestroyedItem(IGameItem originalItem)
	{
		if (_onDestroyedGameItemProto == 0)
		{
			return null;
		}

		var proto = Gameworld.ItemProtos.Get(_onDestroyedGameItemProto);
		if (proto == null || proto.Status != RevisionStatus.Current)
		{
			return null;
		}

		var newItem = proto.CreateNew();
		newItem.RoomLayer = originalItem.RoomLayer;
		var varNewItem = newItem.GetItemType<IVariable>();
		if (varNewItem != null)
		{
			var varOldItem = originalItem.GetItemType<IVariable>();
			foreach (var variable in varNewItem.CharacteristicDefinitions)
			{
				var oldValue = varOldItem.GetCharacteristic(variable);
				if (oldValue != null)
				{
					varNewItem.SetCharacteristic(variable, oldValue);
				}
			}
		}

		return newItem;
	}

	private long _onMorphGameItemProto;
	public string MorphEmote { get; set; }
	public bool Morphs { get; set; }
	public TimeSpan MorphTimeSpan { get; set; }

	public IGameItem LoadMorphedItem(IGameItem originalItem)
	{
		if (_onMorphGameItemProto == 0)
		{
			return null;
		}

		var proto = Gameworld.ItemProtos.Get(_onMorphGameItemProto);
		if (proto == null || proto.Status != RevisionStatus.Current)
		{
			return null;
		}

		var newItem = proto.CreateNew();
		newItem.RoomLayer = originalItem.RoomLayer;
		return newItem;
	}

	public bool CheckForComponentPrototypeUpdates()
	{
		MudSharp.Models.GameItemProto dbitem = null;
		var updated = false;
		foreach (var comp in _components.ToList())
		{
			if (comp.Status.In(RevisionStatus.Obsolete, RevisionStatus.Revised))
			{
				var newProto =
					Gameworld.ItemComponentProtos.FirstOrDefault(
						x => x.Id == comp.Id && x.Status == RevisionStatus.Current);
				if (newProto == null)
				{
					return false;
				}

				if (dbitem == null)
				{
					dbitem = FMDB.Context.GameItemProtos.Find(Id, RevisionNumber);
				}

				var protoToRemove =
					dbitem.GameItemProtosGameItemComponentProtos.FirstOrDefault(x =>
						x.GameItemComponentProtoId == comp.Id);
				dbitem.GameItemProtosGameItemComponentProtos.Remove(protoToRemove);
				dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
				{
					GameItemProto = dbitem, GameItemComponentProtoId = newProto.Id,
					GameItemComponentRevision = newProto.RevisionNumber
				});
				_components.Remove(comp);
				_components.Add(newProto);
				updated = true;
			}
		}

		return updated;
	}

	private readonly List<IGameItemComponentProto> _components = new();

	public IEnumerable<IGameItemComponentProto> Components => _components;

	public void AddComponent(IGameItemComponentProto component)
	{
		_components.Add(component);
		Changed = true;
	}

	public bool IsItemType<T>() where T : IGameItemComponentProto
	{
		return _components.OfType<T>().Any();
	}

	public T GetItemType<T>() where T : IGameItemComponentProto
	{
		return _components.OfType<T>().FirstOrDefault();
	}

	public bool OverridesLongDescription => !string.IsNullOrEmpty(LongDescription);

	public string LongDescription { get; set; }

	public ItemQuality BaseItemQuality { get; protected set; }

	public SizeCategory Size { get; protected set; }

	public double Weight { get; set; }

	private bool _readOnly;
	public override bool ReadOnly => _readOnly;

	public ISolid Material { get; private set; }

	private readonly Dictionary<string, string> _defaultVariables = new();

	public IReadOnlyDictionary<string, string> DefaultVariables => _defaultVariables;

	private IGameItemGroup _itemGroup;

	public IGameItemGroup ItemGroup
	{
		get => _itemGroup;
		set
		{
			if (_itemGroup is not null)
			{
				_itemGroup.OnDelete -= ItemGroupOnOnDelete;
			}

			_itemGroup = value;
			if (_itemGroup is not null)
			{
				_itemGroup.OnDelete += ItemGroupOnOnDelete;
			}
		}
	}

	private void ItemGroupOnOnDelete(object sender, EventArgs e)
	{
		_itemGroup = null;
		Changed = true;
	}

	public bool HighPriority { get; protected set; }

	public ANSIColour CustomColour { get; protected set; }

	public bool PermitPlayerSkins { get; protected set; }

	#endregion

	#region Building Commands

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var subCommand = command.Pop().ToLowerInvariant();
		switch (subCommand)
		{
			case "skinnable":
			case "canskin":
			case "skin":
				return BuildingCommandSkinnable(actor);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "cost":
				return BuildingCommandCost(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
			case "submit":
				return BuildingCommandSubmit(actor, command);
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "ldesc":
				return BuildingCommandLDesc(actor, command);
			case "name":
			case "noun":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "suggestdesc":
				return BuildingCommandSuggestDesc(actor, command);
			case "keywords":
			case "keys":
				return BuildingCommandKeywords(actor, command);
			case "detach":
			case "remove":
				return BuildingCommandDetach(actor, command);
			case "attach":
			case "add":
				return BuildingCommandAttach(actor, command);
			case "size":
				return BuildingCommandSize(actor, command);
			case "weight":
				return BuildingCommandWeight(actor, command);
			case "material":
				return BuildingCommandMaterial(actor, command);
			case "register":
				return BuildingCommandRegister(actor, command);
			case "itemgroup":
			case "group":
				return BuildingCommandItemGroup(actor, command);
			case "destroyed":
				return BuildingCommandDestroyed(actor, command);
			case "strategy":
				return BuildingCommandStrategy(actor, command);
			case "onload":
				return BuildingCommandOnLoadProg(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
			case "untag":
				return BuildingCommandUntag(actor, command);
			case "quality":
				return BuildingCommandQuality(actor, command);
			case "morph":
				return BuildingCommandMorph(actor, command);
			case "extra":
				return BuildingCommandExtra(actor, command);
			default:
                actor.OutputHandler.Send(@"You can use the following options with the ITEM SET command:

	#3add <id|name>#0 - adds the specified component to this item
	#3remove <id|name>#0 - removes the specified component from this item
	#3noun <noun>#0 - sets the primary noun for this item. Single word only.
	#3sdesc <sdesc>#0 - sets the short description (e.g. a solid iron sword)
	#3ldesc <ldesc>#0 - sets an overrided long description (in room) for this item
	#3ldesc none#0 - clears an overrided long description
	#3desc#0 - drops you into an editor to enter the full description (when looked at)
	#3suggestdesc [<optional extra context>]#0 - asks your GPT model to suggest a description
	#3size <size>#0 - sets the item size
	#3weight <weight>#0 - sets the item weight
	#3material <material>#0 - sets the item material
	#3quality <quality>#0 - sets the base item quality
	#3cost <cost>#0 - sets the base item cost
	#3tag <tag>#0 - adds the specified tag to this item
	#3untag <tag>#0 - removes the specified tag from this item
	#3priority#0 - toggles this item being high priority, which means appearing at the top of the item list in the room
	#3colour <ansi colour>#0 - overrides the default green colour for this item
	#3colour none#0 - resets the item colour to the default
	#3onload <prog>#0 - toggles a particular prog to run when the item is loaded
	#3canskin#0 - toggles whether players can make skins for this item
	#3register <variable name> <default value>#0 - sets a default value for a register variable for this item
	#3register delete <variable name>#0 - deletes a default value for a register variable
	#3morph <item##|none> <seconds> [<emote>]#0 - sets item morph information. The 'none' value makes the item disappear.
	#3morph clear#0 - clears any morph info for this item
	#3group <id|name>#0 - sets this item's item group (for in-room grouping)
	#3group none#0 - clears this item's item group
	#3destroyed <id>#0 - sets an item to load up in-place of this item when it is destroyed
	#3destroyed none#0 - clears a destroyed item setting
	#3strategy <id|name>#0 - sets a custom health strategy for this item
	#3strategy none#0 - sets the item to use the default item health strategy
	#3extra add <prog>#0 - adds a new extra description slot with a specified prog
	#3extra remove <which##>#0 - removes the specified numbered extra description
	#3extra swap <first##> <second##>#0 - swaps the evaluation order of two extra descriptions
	#3extra <which##> sdesc <sdesc>#0 - sets the short description for the extra description
	#3extra <which##> clear sdesc#0 - clears the short description for the extra description
	#3extra <which##> desc <desc>#0 - sets the full description for the extra description
	#3extra <which##> clear desc#0 - clears the full description for the extra description
	#3extra <which##> addendum <text>#0 - sets an addendum text for the full description
	#3extra <which##> clear addendum#0 - clears the addendum text for the full description".SubstituteANSIColour());
                return true;
        }
	}

	private bool BuildingCommandSkinnable(ICharacter actor)
	{
		PermitPlayerSkins = !PermitPlayerSkins;
		Changed = true;
		actor.OutputHandler.Send(
			$"This item will {PermitPlayerSkins.NowNoLonger()} permit players to create skins for it.");
		return true;
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the base cost of this item?");
			return false;
		}

		if (actor.Currency is null)
		{
			actor.OutputHandler.Send(
				"You must set a currency with the CURRENCY SET command before using this building command.");
			return false;
		}

		if (!actor.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount) || amount < 0.0M)
		{
			actor.OutputHandler.Send($"That is not a valid amount of {actor.Currency.Name.ColourValue()}.");
			return false;
		}

		CostInBaseCurrency = amount * actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
		Changed = true;
		actor.OutputHandler.Send($"This item will now have a base cost of {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSuggestDesc(ICharacter actor, StringStack command)
	{
		var descModel = Futuremud.Games.First().GetStaticConfiguration("GPT_DescSuggestion_Model");
		var sb = new StringBuilder();
		sb.AppendLine(Futuremud.Games.First().GetStaticString("GPT_ItemSuggestionPrompt"));
		sb.AppendLine();
		sb.AppendLine($"The item that you are describing has a brief description of \"{ShortDescription}\". It is made mostly out of {Material?.Name ?? "an unknown material"}. Its quality is {BaseItemQuality.DescribeEnum(true)}, and it weighs {Gameworld.UnitManager.Describe(Weight, UnitType.Mass, "metric")} (but don't refer to the specific number in the description, though you can describe the weight in other non-numeric terms). It's {Size.DescribeEnum(true)} relative to a person.");
		sb.AppendLine();
		if (Tags.Any())
		{
			sb.AppendLine($"It has been tagged with the following prompts:");
			foreach (var tag in Tags)
			{
				sb.AppendLine($"\t{tag.FullName}");
			}
		}

		//sb.AppendLine();
		//sb.AppendLine(
		//	"It also has the following functional components. These components add functionality to the item in the game engine. You shouldn't describe these functional components in the description as the user will get information about this from other sources, but do use these to inform yourself about any functionality of the item:");
		//foreach (var component in _components)
		//{
		//	sb.AppendLine($"\tComponent Type: {component.TypeDescription} | Name: {component.Name} | Description: {Gameworld.GameItemComponentManager.TypeHelpInfo.FirstOrDefault(x => x.Name.EqualTo(component.TypeDescription)).Blurb}");
		//}

		var vp = _components.OfType<VariableGameItemComponentProto>().FirstOrDefault();
		if (vp is not null)
		{
			sb.AppendLine();
			sb.AppendLine($"When the engine uses this item it will substitute values for a few variables. In the description that you write, you should use only the pattern of the variable rather than any of its actual values. For example, if there is a variable called $colour then you should use that variable to refer to the colour of the item rather than writing in an actual colour. If there are multiple similar variables you can decide what part of each item each of the variables refer to. Each variation has a plain, basic and fancy variety - they refer to the same underlying variable but present the text in a different format. The following variables (and some examples of what the engine may render them as, so you can match your grammar to their usage) are available:");
			foreach (var variable in vp.CharacteristicDefinitions)
			{
				var profile = vp.ProfileFor(variable);
				var random = new List<ICharacteristicValue>
				{
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
					profile.GetRandomCharacteristic(),
				};
				sb.AppendLine($"\t${variable.Pattern.TransformRegexIntoPattern()} - Examples: {random.Select(x => x.GetValue).ListToString()}");
			}

			sb.AppendLine("You should use each variable at least once in the description.");
		}

		if (command.IsFinished)
		{
			sb.AppendLine();
			sb.AppendLine(command.SafeRemainingArgument);
		}

		if (!OpenAIHandler.MakeGPTRequest(sb.ToString(), Gameworld.GetStaticString("GPT_ItemSuggestionFinalWord"), text =>
		    {
			    var descriptions = text.Split('#', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			    var sb = new StringBuilder();
			    sb.AppendLine($"Your GPT Model has made the following suggestions for descriptions for item {ShortDescription.ColourObject()}:");
			    var i = 1;
			    foreach (var desc in descriptions)
			    {
				    sb.AppendLine();
				    sb.AppendLine($"Suggestion {i++.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
				    sb.AppendLine();
				    sb.AppendLine(desc.Wrap(actor.InnerLineFormatLength, "\t"));
			    }

			    sb.AppendLine();
			    sb.AppendLine($"You can apply one of these by using the syntax {"accept desc <n>".Colour(Telnet.Cyan)}, such as {"accept desc 1".Colour(Telnet.Cyan)}.");
			    actor.OutputHandler.Send(sb.ToString());
				actor.AddEffect(new Accept(actor, new GenericProposal
				{
					DescriptionString = "Applying a GPT Description suggestion",
					AcceptAction = text =>
					{
						if (!int.TryParse(text, out var value) || value < 1 || value > descriptions.Length)
						{
							actor.OutputHandler.Send(
								"You did not specify a valid description. If you still want to use the descriptions, you'll have to copy them in manually.");
							return;
						}
						FullDescription = descriptions[value-1];
						Changed = true;
						actor.OutputHandler.Send($"You set the description of this item based on the {value.ToOrdinal()} suggestion.");
					},
					RejectAction = text =>
					{
						actor.OutputHandler.Send("You decide not to use any of the suggestions.");
					},
					ExpireAction = () =>
					{
						actor.OutputHandler.Send("You decide not to use any of the suggestions.");
					},
					Keywords = new List<string> {"description", "suggestion"}
				}), TimeSpan.FromSeconds(120));
		    }, descModel))
		{
			actor.OutputHandler.Send("Your GPT Model is not set up correctly, so you cannot get any suggestions.");
			return false;
		}

		actor.OutputHandler.Send("You send your request off to the GPT Model.");
		return true;
	}

	private bool BuildingCommandExtra(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandExtraAdd(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandExtraRemove(actor, command);
			case "swap":
				return BuildingCommandExtraSwap(actor, command);
		}

		if (!int.TryParse(command.Last, out var value))
		{
			actor.OutputHandler.Send(@"The correct syntax for this command is as follows:

    add <prog> - adds a new extra description slot with a specified prog
    remove <which#> - removes the specified numbered extra description
    swap <first#> <second#> - swaps the evaluation order of two extra descriptions
    <which#> sdesc <sdesc> - sets the short description for the extra description
    <which#> clear sdesc - clears the short description for the extra description
    <which#> desc <desc> - sets the full description for the extra description
    <which#> clear desc - clears the full description for the extra description
    <which#> addendum <text> - sets an addendum text for the full description
    <which#> clear addenudm - clears the addendum text for the full description");
			return false;
		}

		if (value < 1 || value > _extraDescriptions.Count)
		{
			if (!_extraDescriptions.Any())
			{
				actor.OutputHandler.Send("There aren't any extra descriptions for you to edit.");
				return false;
			}

			if (_extraDescriptions.Count == 1)
			{
				actor.OutputHandler.Send(
					"There is only a single extra description so please use an index of 1 to target this item.");
				return false;
			}

			actor.OutputHandler.Send(
				$"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_extraDescriptions.Count.ToString("N0", actor).ColourValue()} for the index.");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "sdesc":
			case "short":
			case "short desc":
			case "short description":
			case "short_desc":
			case "short_description":
			case "shortdesc":
			case "shortdescription":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"What do you want to set the short description for that extra description slot to?");
					return false;
				}

				var old = _extraDescriptions[value - 1];
				_extraDescriptions[value - 1] = (old.Prog, command.SafeRemainingArgument, old.FullDescription,
					old.FullDescriptionAddendum);
				ExtraDescriptionsChanged = true;
				actor.OutputHandler.Send(
					$"You change the short description of the {value.ToOrdinal().ColourValue()} extra description to {command.SafeRemainingArgument.Colour(CustomColour ?? Telnet.Green)}.");
				return true;
			case "desc":
			case "description":
			case "fdesc":
			case "fdescription":
			case "fulldesc":
			case "full":
			case "fulldescription":
			case "full description":
			case "full_description":
			case "full desc":
			case "full_desc":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"What do you want to set the full description for that extra description slot to?");
					return false;
				}

				old = _extraDescriptions[value - 1];
				_extraDescriptions[value - 1] = (old.Prog, old.ShortDescription,
					command.SafeRemainingArgument.ProperSentences().Fullstop(), old.FullDescriptionAddendum);
				ExtraDescriptionsChanged = true;
				actor.OutputHandler.Send(
					$"You change the full description of the {value.ToOrdinal().ColourValue()} extra description to:\n\n{command.SafeRemainingArgument.Fullstop().ProperSentences().SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}");
				return true;
			case "addendum":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"What do you want to set the description addendum for that extra description slot to?");
					return false;
				}

				old = _extraDescriptions[value - 1];
				_extraDescriptions[value - 1] = (old.Prog, old.ShortDescription, old.FullDescription,
					command.SafeRemainingArgument);
				ExtraDescriptionsChanged = true;
				actor.OutputHandler.Send(
					$"You change the description addendum of the {value.ToOrdinal().ColourValue()} extra description to: \n\n{command.SafeRemainingArgument}");
				return true;
			case "prog":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"Which prog do you want to use to control whether this extra description appears?");
					return false;
				}

				var prog = long.TryParse(command.PopSpeech(), out var progid)
					? Gameworld.FutureProgs.Get(progid)
					: Gameworld.FutureProgs.GetByName(command.Last);
				if (prog == null)
				{
					actor.OutputHandler.Send("There is no such prog.");
					return false;
				}

				if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
				{
					actor.OutputHandler.Send(
						$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} does not.");
					return false;
				}

				if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }))
				{
					actor.OutputHandler.Send(
						$"You must specify a prog that accepts a single character as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
					return false;
				}

				old = _extraDescriptions[value - 1];
				_extraDescriptions[value - 1] = (prog, old.ShortDescription, old.FullDescription,
					old.FullDescriptionAddendum);
				ExtraDescriptionsChanged = true;
				actor.OutputHandler.Send(
					$"You change the applicability prog of the {value.ToOrdinal().ColourValue()} extra description to {prog.MXPClickableFunctionNameWithId()}.");
				return true;
			case "clear":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						$"Which element of the extra description do you want to clear? You can specify {new[] { "sdesc", "desc", "addendum" }.Select(x => x.ColourValue()).ListToString(conjunction: "or ")}.");
					return false;
				}

				switch (command.SafeRemainingArgument.ToLowerInvariant())
				{
					case "sdesc":
						old = _extraDescriptions[value - 1];
						_extraDescriptions[value - 1] =
							(old.Prog, null, old.FullDescription, old.FullDescriptionAddendum);
						ExtraDescriptionsChanged = true;
						actor.OutputHandler.Send(
							$"You clear the short description of the {value.ToOrdinal().ColourValue()} extra description.");
						return true;
					case "desc":
					case "fdesc":
						old = _extraDescriptions[value - 1];
						_extraDescriptions[value - 1] =
							(old.Prog, old.ShortDescription, null, old.FullDescriptionAddendum);
						ExtraDescriptionsChanged = true;
						actor.OutputHandler.Send(
							$"You clear the full description of the {value.ToOrdinal().ColourValue()} extra description.");
						return true;
					case "addendum":
						old = _extraDescriptions[value - 1];
						_extraDescriptions[value - 1] = (old.Prog, old.ShortDescription, old.FullDescription, null);
						ExtraDescriptionsChanged = true;
						actor.OutputHandler.Send(
							$"You clear the description addendum of the {value.ToOrdinal().ColourValue()} extra description.");
						return true;
				}

				actor.OutputHandler.Send(
					$"That is not a valid element of the extra description to clear. You can specify {new[] { "sdesc", "desc", "addendum" }.Select(x => x.ColourValue()).ListToString(conjunction: "or ")}.");
				return false;
		}

		return true;
	}

	private bool BuildingCommandExtraSwap(ICharacter actor, StringStack command)
	{
		if (_extraDescriptions.Count < 2)
		{
			actor.OutputHandler.Send("There must be at least 2 extra descriptions before you can swap them.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the first extra description whose order you want to swap?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index1) || index1 < 1 || index1 > _extraDescriptions.Count)
		{
			actor.OutputHandler.Send(
				$"The first index is not valid. You must supply a number between {1.ToString("N0", actor).ColourValue()} and {_extraDescriptions.Count.ToString("N0", actor).ColourValue()}.");
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the second extra description whose order you want to swap?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index2) || index2 < 1 || index2 > _extraDescriptions.Count)
		{
			actor.OutputHandler.Send(
				$"The second index is not valid. You must supply a number between {1.ToString("N0", actor).ColourValue()} and {_extraDescriptions.Count.ToString("N0", actor).ColourValue()}.");
		}

		if (index1 == index2)
		{
			actor.OutputHandler.Send("You cannot swap an element with itself.");
			return false;
		}

		_extraDescriptions.Swap(index1 - 1, index2 - 1);
		ExtraDescriptionsChanged = true;
		actor.OutputHandler.Send(
			$"You swap the order of the {index1.ToOrdinal().ColourValue()} and {index2.ToOrdinal().ColourValue()} extra descriptions.");
		return true;
	}

	private bool BuildingCommandExtraRemove(ICharacter actor, StringStack command)
	{
		if (_extraDescriptions.Count == 0)
		{
			actor.OutputHandler.Send("There aren't any extra descriptions for you to remove.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the extra description that you want to remove?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 ||
		    value > _extraDescriptions.Count)
		{
			actor.OutputHandler.Send(
				$"That is not a valid extra descripion. You must specify a value between {1.ToString("N0", actor).ColourValue()} and {_extraDescriptions.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		_extraDescriptions.RemoveAt(value - 1);
		ExtraDescriptionsChanged = true;
		actor.OutputHandler.Send($"You delete the {value.ToOrdinal().ColourValue()} extra description.");
		return true;
	}

	private bool BuildingCommandExtraAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog to control the applicability of that extra description.");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var progid)
			? Gameworld.FutureProgs.Get(progid)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		_extraDescriptions.Add((prog, null, null, null));
		ExtraDescriptionsChanged = true;
		actor.OutputHandler.Send(
			$"You add a new extra description at position {_extraDescriptions.Count.ToString("N0", actor).ColourValue()}, controlled by the {prog.MXPClickableFunctionNameWithId()} prog.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (HighPriority)
		{
			actor.Send("This item is no longer high priority in terms of ordering in rooms.");
			HighPriority = false;
			Changed = true;
			return true;
		}

		actor.Send("This item is now high priority in terms of ordering in rooms and will appear before other items.");
		HighPriority = true;
		Changed = true;
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You must either specify a telnet colour or 'none' to set it to none.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			CustomColour = null;
			Changed = true;
			actor.Send("This item will no longer have a custom colour, and will use the default item colour instead.");
			return true;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour == null)
		{
			actor.Send(
				"There is no such colour. Valid colours are red, yellow, green, blue, cyan, magenta, white and the bold versions of these.");
			return false;
		}

		CustomColour = colour;
		Changed = true;
		actor.Send(
			$"This item will now appear in the custom colour {command.SafeRemainingArgument.Colour(CustomColour)}.");
		return true;
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send($"What item quality do you want to assign to this prototype by default? Please see {"show itemquality".MXPSend("show itemquality")} for a list of item qualities.");
			return false;
		}

		if (!GameItemEnumExtensions.TryParseQuality(command.SafeRemainingArgument, out var quality))
		{
			actor.Send(
				$"That is not a valid item quality. Please see {"show itemquality".MXPSend("show itemquality")} for a list of item qualities.");
			return false;
		}

		BaseItemQuality = quality;
		actor.Send($"This item will now have a base quality of {BaseItemQuality.Describe().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What tag do you want to add to this item prototype?");
			return false;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		if (!CanAddTag(tag))
		{
			actor.OutputHandler.Send(
				$"You cannot add the {tag.FullName.Colour(Telnet.Cyan)} tag to this item prototype.");
			return false;
		}

		AddTag(tag);
		actor.OutputHandler.Send($"You add the {tag.FullName.Colour(Telnet.Cyan)} tag to this item prototype.");
		return true;
	}

	private bool BuildingCommandUntag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What tag do you want to remove from this item prototype?");
			return false;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		if (!Tags.Contains(tag))
		{
			actor.Send($"This prototype does not have the {tag.FullName.Colour(Telnet.Cyan)} tag.");
			return false;
		}

		RemoveTag(tag);
		actor.Send($"You remove the {tag.FullName.Colour(Telnet.Cyan)} tag from this item prototype.");
		return true;
	}

	private bool BuildingCommandStrategy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must either specify a health strategy to set for this item, or use {0} to remove an existing one.",
				"none".Colour(Telnet.Yellow));
			return false;
		}

		if (command.PopSpeech().Equals("none", StringComparison.InvariantCultureIgnoreCase))
		{
			_healthStrategy = DefaultItemHealthStrategy;
			Changed = true;
			actor.Send("This item will now use the default health strategy for items.");
			return false;
		}

		var strategy = long.TryParse(command.Last, out var value)
			? Gameworld.HealthStrategies.Get(value)
			: Gameworld.HealthStrategies.GetByName(command.Last);
		if (strategy == null)
		{
			actor.Send("There is no such health strategy for you to use.");
			return false;
		}

		if (strategy.OwnerType != HealthStrategyOwnerType.GameItem)
		{
			actor.Send("That strategy is not able to be used for items. Please select another");
			return false;
		}

		_healthStrategy = strategy;
		Changed = true;
		actor.Send("This item will now use the {0} (#{1:N0}) health strategy.", HealthStrategy.Name,
			HealthStrategy.Id);
		return true;
	}

	private bool BuildingCommandOnLoadProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must specify the id or function name of a prog you wish to toggle being executed OnLoad.");
			return false;
		}

		if (command.PeekSpeech().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			OnLoadProgs.Clear();
			Changed = true;
			actor.Send("You clear all OnLoad progs from this item proto.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.Send("There is no such prog for you to toggle OnLoad.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
		{
			actor.Send(
				"Only progs that accept a single Item or a Character and and Item as parameters can be used for OnLoad progs.");
			return false;
		}

		if (OnLoadProgs.Contains(prog))
		{
			OnLoadProgs.Remove(prog);
			actor.Send(
				"You remove prog {0} #{1:N0} from this item proto. It will no longer execute when the item is loaded.",
				prog.FunctionName, prog.Id);
			Changed = true;
			return true;
		}

		OnLoadProgs.Add(prog);
		actor.Send("You add prog {0} #{1:N0} to this item proto. It will now execute when the item is loaded.",
			prog.FunctionName, prog.Id);
		Changed = true;
		return true;
	}

	private bool BuildingCommandDestroyed(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must either specify another item prototype to be loaded when this item is destroyed, or use {0} to remove an existing one.",
				"none".Colour(Telnet.Yellow));
			return false;
		}

		if (command.PopSpeech().EqualToAny("clear", "none", "delete"))
		{
			_onDestroyedGameItemProto = 0;
			Changed = true;
			actor.Send("This item will no longer load any other item when destroyed.");
			return true;
		}

		var item = long.TryParse(command.Last, out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.Last);
		if (item == null)
		{
			actor.Send("There is no item proto like that which you can set as a destroyed item for another item.");
			return false;
		}

		if (item.Status != RevisionStatus.Current)
		{
			actor.Send("Only approved prototypes can be used as destroyed items.");
			return false;
		}

		_onDestroyedGameItemProto = item.Id;
		Changed = true;
		actor.Send("When destroyed, this item will now become item {0} (#{1:N0})", item.Name, item.Id);
		return true;
	}

	private bool BuildingCommandItemGroup(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"Which item group do you want to assign this item to? You can use {0} to remove an existing group.",
				"item set itemgroup delete".Colour(Telnet.Yellow));
			return false;
		}

		var text = command.PopSpeech();
		if (text.EqualToAny("clear", "none", "delete"))
		{
			if (ItemGroup == null)
			{
				actor.Send("This item does not have an item group to delete.");
				return false;
			}


			ItemGroup = null;
			Changed = true;
			actor.Send("You clear the item group from this item.");
			return true;
		}

		var itemgroup = long.TryParse(text, out var value)
			? actor.Gameworld.ItemGroups.Get(value)
			: actor.Gameworld.ItemGroups.FirstOrDefault(
				x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase));

		if (itemgroup == null)
		{
			actor.Send("There is no such item group.");
			return false;
		}

		ItemGroup = itemgroup;
		Changed = true;
		actor.Send("This item now belongs to the {0} item group (ID #{1:N0})", itemgroup.Name, itemgroup.Id);
		return true;
	}

	private bool BuildingCommandLDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.Peek().EqualToAny("clear", "none", "delete"))
		{
			LongDescription = "";
			Changed = true;
			actor.Send(
				"You clear the long description from this item. It will now use the default long description.");
			return true;
		}

		LongDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.Send("You set the long description for this item to: {0}", LongDescription.Proper().ColourObject());
		return true;
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What material do you want this item to have?");
			return false;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material == null)
		{
			actor.Send("There is no such material.");
			return false;
		}

		Material = material;
		Changed = true;
		actor.Send("This item is now made primarily from {0}.", material.Name.TitleCase().Colour(Telnet.Green));
		return true;
	}

	private bool BuildingCommandSize(ICharacter actor, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("What size do you want to set this item to?");
			return false;
		}

		var size = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>();
		SizeCategory target;
		var itemSizeCategories = size as SizeCategory[] ?? size.ToArray();
		if (itemSizeCategories.Any(x => x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal)))
		{
			target = itemSizeCategories.FirstOrDefault(x =>
				x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
		}
		else
		{
			actor.OutputHandler.Send(
				$"That is not a valid item size. See {"show itemsizes".Colour(Telnet.Yellow)} for a correct list.");
			return false;
		}

		Size = target;
		Changed = true;
		actor.OutputHandler.Send("You set the size of this object to " + target.Describe() + ".");
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a weight to set for this item.");
			return false;
		}

		var result = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid weight.");
			return false;
		}

		Weight = result;
		actor.OutputHandler.Send(
			$"You set the weight of this item to {Gameworld.UnitManager.Describe(Weight, UnitType.Mass, actor).ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		var cmd = command.SafeRemainingArgument ?? "";
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("You must specify a noun for this item.");
			return false;
		}

		_name = cmd;
		Changed = true;
		actor.OutputHandler.Send("You set the noun for " + EditHeader() + " to " + cmd);
		return true;
	}

	private bool BuildingCommandKeywords(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.Length == 0)
		{
			actor.OutputHandler.Send("You must specify keywords for this item.");
			return false;
		}

		_keywords =
			new Lazy<List<string>>(
				() => (from keyword in command.SafeRemainingArgument.Split(' ') select keyword.Trim()).ToList());
		actor.OutputHandler.Send("You set the keywords for " + EditHeader() + " to " +
		                         Keywords.Select(x => x.Colour(Telnet.Cyan)).ListToString() + ".");
		Changed = true;
		return true;
	}

	private void BuildingCommandDescPost(string description, IOutputHandler handler, object[] arguments)
	{
		FullDescription = description.Trim();
		Changed = true;
		handler.Send("You set the description for " + EditHeader() + " to:\n\n" + FullDescription.Wrap(80, "\t"));
	}

	private void BuildingCommandDescCancel(IOutputHandler handler, object[] arguments)
	{
		handler.Send("You decide not to change the description.");
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(FullDescription))
		{
			sb.AppendLine("Replacing:\n");
			sb.AppendLine(FullDescription.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		sb.AppendLine("You can use the following markup options in this description:");
		sb.AppendLine("\t@material - will be replaced with this item's material".ColourCommand());
		sb.AppendLine("\t@matdesc - will be replaced with this item's material description".ColourCommand());
		sb.AppendLine(
			"\twriting{language,script,style=...,colour=...,minskill}{text if you can understand}{text if you cant}"
				.ColourCommand());
		sb.AppendLine("\tcheck{trait,minvalue}{text if the trait is >= value}{text if not}".ColourCommand());
		var variableComp = GetItemType<VariableGameItemComponentProto>();
		if (variableComp != null)
		{
			foreach (var variable in variableComp.CharacteristicDefinitions)
			{
				sb.AppendLine($"\t${variable.Pattern} / ${variable.Pattern}fancy / ${variable.Pattern}brief"
					.ColourCommand());
			}
		}

		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(BuildingCommandDescPost, BuildingCommandDescCancel, 1.0);
		return true;
	}

	private bool BuildingCommandSubmit(ICharacter actor, StringStack command)
	{
		if (Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send("You can only submit items in the Under Design status for review.");
			return false;
		}

		ChangeStatus(RevisionStatus.PendingRevision, command.SafeRemainingArgument, actor.Account);
		actor.OutputHandler.Send(
			$"You submit Item Prototype #{Id.ToString("N0", actor)} Revision {RevisionNumber.ToString("N0", actor)} for review.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.Length == 0)
		{
			actor.OutputHandler.Send("You must specify a short description for this item.");
			return false;
		}

		ShortDescription = command.SafeRemainingArgument.Trim();
		Changed = true;
		SetKeywordsFromSDesc(ShortDescription);
		actor.OutputHandler.Send(
			$"You set the short description for Item Prototype #{Id.ToString("N0", actor)} Revision {RevisionNumber.ToString("N0", actor)} to {ShortDescription.ColourObject()}.");
		return true;
	}

	private bool BuildingCommandDetach(ICharacter actor, StringStack command)
	{
		var subCommand = command.SafeRemainingArgument;
		if (subCommand.Length == 0)
		{
			actor.OutputHandler.Send("Which Item Component do you wish to deatch?");
			return false;
		}

		if (subCommand.Equals("all", StringComparison.InvariantCultureIgnoreCase))
		{
			_components.Clear();
			actor.OutputHandler.Send("You remove all components from Item Prototype #" + Id + " Revision " +
			                         RevisionNumber + ".");
			Changed = true;
			return true;
		}

		var component = long.TryParse(subCommand, out var value)
			? _components.FirstOrDefault(x => x.Id == value)
			: _components.FirstOrDefault(x => x.Name.Equals(subCommand, StringComparison.InvariantCultureIgnoreCase));
		if (component == null)
		{
			actor.OutputHandler.Send("There is no such component for you to detach.");
			return false;
		}

		_components.Remove(component);
		actor.OutputHandler.Send("You remove the " + component.Name.Colour(Telnet.Cyan) +
		                         " component from Item Prototype #" + Id + " Revision " + RevisionNumber + ".");
		Changed = true;
		return true;
	}

	private bool BuildingCommandAttach(ICharacter actor, StringStack command)
	{
		var subCommand = command.SafeRemainingArgument;
		IGameItemComponentProto component = null;
		component = !long.TryParse(subCommand, out var number)
			? Gameworld.ItemComponentProtos.GetByName(subCommand, true)
			: Gameworld.ItemComponentProtos.Get(number);


		if (component == null)
		{
			actor.OutputHandler.Send("There is no such component prototype.");
			return false;
		}

		if (component.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("That component prototype is not current and approved for use.");
			return false;
		}

		if (_components.Contains(component))
		{
			actor.OutputHandler.Send("This Item already contains that component.");
			return false;
		}

		if (component.PreventManualLoad)
		{
			actor.OutputHandler.Send(
				$"This component can only be used by the engine internally, not in manual building.");
			return false;
		}

		_components.Add(component);
		actor.OutputHandler.Send(
			$"You attach the {component.Name.Proper().Colour(Telnet.Cyan)} component to Item Prototype #{Id.ToString("N0", actor)} Revision {RevisionNumber.ToString("N0", actor)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandRegister(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You can either delete an existing variable, or set a value for one.");
			return false;
		}

		if (command.Peek().Equals("delete", StringComparison.InvariantCultureIgnoreCase))
		{
			return BuildingCommandRegisterDelete(actor, command);
		}

		var variableName = command.Pop();
		var variableType = Gameworld.VariableRegister.GetType(FutureProgVariableTypes.Item, variableName);
		if (variableType == FutureProgVariableTypes.Error)
		{
			actor.Send("There is no GameItem variable called {0} - you will need to register it first.",
				variableName);
			return false;
		}

		if (!FutureProgVariableTypes.ValueType.CompatibleWith(variableType))
		{
			actor.Send("Only value type variables can be given default values for Game Items.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send(
				"What value do you want to set for this variable?\nNote: To delete the variable, use {0} instead",
				"item set register delete <variable>".Colour(Telnet.Yellow));
			return false;
		}

		if (!Gameworld.VariableRegister.ValidValueType(variableType, command.SafeRemainingArgument))
		{
			actor.Send("That is not a valid value for the {0} variable type.", variableType.Describe());
			return false;
		}

		_defaultVariables[variableName.ToLowerInvariant()] = command.SafeRemainingArgument;
		Changed = true;
		actor.Send("You set the default register value {0} for this item to {1}.", variableName.Colour(Telnet.Cyan),
			command.SafeRemainingArgument.Colour(Telnet.Green));
		return true;
	}

	private bool BuildingCommandRegisterDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which default register value do you want to delete for this item?");
			return false;
		}

		var whichVariable = command.Pop().ToLowerInvariant();
		if (!_defaultVariables.ContainsKey(whichVariable))
		{
			actor.Send("This item does not have a default register value of {0}.", whichVariable);
			return false;
		}

		_defaultVariables.Remove(whichVariable);
		Changed = true;
		actor.Send("You delete the default register value for {0}. It will now use the system-wide default.",
			whichVariable);
		return true;
	}

	private bool BuildingCommandMorph(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"Correct syntax is {"item set morph clear".Colour(Telnet.Yellow)} or {"item set morph <item#/none> <seconds> [<emote>]".Colour(Telnet.Yellow)}.");
			return false;
		}

		if (command.Peek().EqualTo("clear"))
		{
			Morphs = false;
			MorphEmote = "$0 $?1|morphs into $1|decays into nothing|$.";
			MorphTimeSpan = TimeSpan.Zero;
			_onMorphGameItemProto = 0;
			Changed = true;
			actor.Send("This item will no longer morph.");
			return true;
		}

		var value = 0L;
		if (command.Peek().EqualTo("none"))
		{
			command.Pop();
			if (Morphs && _onMorphGameItemProto > 0 && command.IsFinished)
			{
				_onMorphGameItemProto = 0;
				Changed = true;
				actor.Send(
					"This item will now simply disappear rather than morphing when it reaches its morph timer.");
				return false;
			}
		}
		else
		{
			if (!long.TryParse(command.Pop(), out value))
			{
				actor.Send(
					$"Correct syntax is {"item set morph clear".Colour(Telnet.Yellow)} or {"item set morph <item#/none> <seconds> [<emote>]".Colour(Telnet.Yellow)}.");
				return false;
			}

			if (Gameworld.ItemProtos.Get(value) == null)
			{
				actor.Send("There is no such item prototype for this item to morph into.");
				return false;
			}
		}

		if (command.IsFinished)
		{
			if (!Morphs)
			{
				actor.Send("How long should it be before this item morphs?");
				return false;
			}

			_onMorphGameItemProto = value;
			Changed = true;
			actor.Send(
				$"This item will now morph into item prototype #{_onMorphGameItemProto} ({Gameworld.ItemProtos.Get(_onMorphGameItemProto).ShortDescription.ColourObject()}) when it morphs.");
			return true;
		}

		if (!double.TryParse(command.PopSpeech(), out var seconds))
		{
			actor.Send("You must enter a number of seconds before this item morphs.");
			return false;
		}

		if (seconds <= 0)
		{
			actor.Send("The number of seconds must be positive and greater than zero.");
			return false;
		}

		Morphs = true;
		MorphTimeSpan = TimeSpan.FromSeconds(seconds);
		_onMorphGameItemProto = value;
		Changed = true;

		if (command.IsFinished)
		{
			actor.Send(
				$"This item will now morph after {MorphTimeSpan.Describe()} into {(_onMorphGameItemProto == 0 ? "nothing" : $"item prototype #{_onMorphGameItemProto} ({Gameworld.ItemProtos.Get(_onMorphGameItemProto).ShortDescription.ColourObject()})")}.");
			return true;
		}

		MorphEmote = command.RemainingArgument;
		actor.Send(
			$"This item will now morph after {MorphTimeSpan.Describe()} into {(_onMorphGameItemProto == 0 ? "nothing" : $"item prototype #{_onMorphGameItemProto} ({Gameworld.ItemProtos.Get(_onMorphGameItemProto).ShortDescription.ColourObject()})")} with the emote: {MorphEmote.Colour(Telnet.Yellow)}.");
		return true;
	}

	#endregion

	#region IHaveTags Members

	private bool _tagsChanged;

	private bool TagsChanged
	{
		get => _tagsChanged;
		set
		{
			if (!_tagsChanged && value)
			{
				Changed = true;
			}

			_tagsChanged = value;
		}
	}

	private readonly List<ITag> _tags = new();
	public IEnumerable<ITag> Tags => _tags;

	private bool CanAddTag(ITag tag)
	{
		return Tags.All(x => !x.IsA(tag) && !tag.IsA(x));
	}

	public bool AddTag(ITag tag)
	{
		if (!CanAddTag(tag))
		{
			return false;
		}

		_tags.Add(tag);
		TagsChanged = true;
		return true;
	}

	public bool RemoveTag(ITag tag)
	{
		_tags.Remove(tag);
		TagsChanged = true;
		return true;
	}

	public bool IsA(ITag tag)
	{
		return tag != null ? Tags.Any(x => x.IsA(tag)) : true;
	}

	#endregion
}