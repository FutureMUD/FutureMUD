using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.CharacterCreation;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Hints;
using MudSharp.RPG.Merits;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper EntityDescriptionHelper { get; } = new()
	{
		ItemName = "Description",
		ItemNamePlural = "Descriptions",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IEntityDescriptionPattern>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IEntityDescriptionPattern>(actor) { EditingItem = (IEntityDescriptionPattern)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IEntityDescriptionPattern>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.EntityDescriptionPatterns.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.EntityDescriptionPatterns.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.EntityDescriptionPatterns.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IEntityDescriptionPattern)item),
		CastToType = typeof(IEntityDescriptionPattern),
		EditableNewAction = (actor, input) =>
		{
			EntityDescriptionType type;
			switch (input.PopForSwitch())
			{
				case "sdesc":
				case "short":
				case "shortdesc":
					type = EntityDescriptionType.ShortDescription; 
					break;
				case "fdesc":
				case "full":
				case "fulldesc":
				case "desc":
					type = EntityDescriptionType.FullDescription; 
					break;
				default:
					actor.OutputHandler.Send("Is this a description for a #3sdesc#0 or a #3fdesc#0?".SubstituteANSIColour());
					return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which prog do you want to use to control this description?");
				return;
			}

			var prog = new FutureProgLookupFromBuilderInput(actor, input.PopSpeech(), FutureProgVariableTypes.Boolean, [FutureProgVariableTypes.Toon]).LookupProg();
			if (prog is null)
			{
				return;
			}

			if (input.IsFinished)
			{
				var sb = new StringBuilder();
				sb.AppendLine(EntityDescriptionPatternExtensions.GetDescriptionHelpNoTemplate());
				sb.AppendLine();
				sb.AppendLine("Enter your new pattern below: ");
				actor.EditorMode((text, handler, _) =>
					{
						var desc = new EntityDescriptionPattern(actor.Gameworld, text, type, prog);
						actor.Gameworld.Add(desc);
						actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<IEntityDescriptionPattern>>());
						actor.AddEffect(new BuilderEditingEffect<IEntityDescriptionPattern>(actor) { EditingItem = desc });
						handler.Send($"You create a new {type.DescribeEnum()} description pattern, which you are now editing.");
					},
					(handler, _) =>
					{
						handler.Send("You decide not to create a new pattern.");
					});
				return;
			}

			var pattern = input.SafeRemainingArgument;
			var desc = new EntityDescriptionPattern(actor.Gameworld, pattern, type, prog);
			actor.Gameworld.Add(desc);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<IEntityDescriptionPattern>>());
			actor.AddEffect(new BuilderEditingEffect<IEntityDescriptionPattern>(actor) { EditingItem = desc });
			actor.OutputHandler.Send($"You create a new {type.DescribeEnum()} description pattern, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which description pattern do you want to clone?");
				return;
			}

			var template = actor.Gameworld.EntityDescriptionPatterns.GetById(input.SafeRemainingArgument);
			if (template is null)
			{
				actor.OutputHandler.Send("There is no such description pattern.");
				return;
			}

			var clone = template.Clone();
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<IEntityDescriptionPattern>>());
			actor.AddEffect(new BuilderEditingEffect<IEntityDescriptionPattern>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone pattern {template.Id.ToStringN0(actor)}, which you are now editing.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Type",
			"Pattern",
			"Prog",
			"Weight"
		},

		GetListTableContentsFunc = (character, protos) =>
			from proto in protos.OfType<IEntityDescriptionPattern>()
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.Type.DescribeEnum(),
				proto.Pattern,
				proto.ApplicabilityProg?.MXPClickableFunctionName() ?? "",
				proto.RelativeWeight.ToStringN0(character)
			},

		CustomSearch = (protos, keyword, gameworld) =>
		{
			if (keyword.Length > 1)
			{
				if (keyword[0] == '+')
				{
					keyword = keyword.Substring(1);
					return protos
					       .OfType<IEntityDescriptionPattern>()
					       .Where(x => x.Pattern.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
					       .ToList<IEditableItem>();
				}

				if (keyword[0] == '-')
				{
					keyword = keyword.Substring(1);
					return protos
					       .OfType<IEntityDescriptionPattern>()
					       .Where(x => !x.Pattern.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
					       .ToList<IEditableItem>();
				}

				if (keyword.EqualToAny("sdesc", "short", "shortdesc"))
				{
					return protos
					       .OfType<IEntityDescriptionPattern>()
					       .Where(x => x.Type == EntityDescriptionType.ShortDescription)
					       .ToList<IEditableItem>();
				}

				if (keyword.EqualToAny("fdesc", "fulldesc", "desc"))
				{
					return protos
					       .OfType<IEntityDescriptionPattern>()
					       .Where(x => x.Type == EntityDescriptionType.FullDescription)
					       .ToList<IEditableItem>();
				}

				return protos
				       .OfType<IEntityDescriptionPattern>()
				       .Where(x => x.ApplicabilityProg?.FunctionName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ?? false)
				       .ToList<IEditableItem>();
			}
			return protos;
		},
		GetEditHeader = item => $"Description Pattern #{item.Id:N0}",
		DefaultCommandHelp = BuilderModule.DescriptionPatternHelp
	};

	public static EditableItemHelper CharacterIntroTemplateHelper { get; } = new()
	{
		ItemName = "Character Intro Template",
		ItemNamePlural = "Character Intro Templates",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<ICharacterIntroTemplate>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<ICharacterIntroTemplate>(actor) { EditingItem = (ICharacterIntroTemplate)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<ICharacterIntroTemplate>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.CharacterIntroTemplates.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.CharacterIntroTemplates.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.CharacterIntroTemplates.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((ICharacterIntroTemplate)item),
		CastToType = typeof(ICharacterIntroTemplate),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the character intro template?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.CharacterIntroTemplates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a character intro template with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var cit = new CharacterCreation.CharacterIntroTemplate(actor.Gameworld, name);
			actor.Gameworld.Add(cit);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<ICharacterIntroTemplate>>());
			actor.AddEffect(new BuilderEditingEffect<ICharacterIntroTemplate>(actor) { EditingItem = cit });
			actor.OutputHandler.Send($"You create a new character intro template called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which character intro template do you want to clone?");
				return;
			}

			var template = actor.Gameworld.CharacterIntroTemplates.GetByIdOrName(input.PopSpeech());
			if (template is null)
			{
				actor.OutputHandler.Send("There is no such character intro template.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new character intro template?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.CharacterIntroTemplates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a character intro template with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var cit = template.Clone(name);
			actor.Gameworld.Add(cit);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<ICharacterIntroTemplate>>());
			actor.AddEffect(new BuilderEditingEffect<ICharacterIntroTemplate>(actor) { EditingItem = cit });
			actor.OutputHandler.Send($"You create a new character intro template called {name.ColourName()} as a clone of {template.Name.ColourName()}, which you are now editing.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Priority",
			"Prog",
			"# Echoes",
			"Length"
		},

		GetListTableContentsFunc = (character, protos) =>
			from proto in protos.OfType<ICharacterIntroTemplate>()
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.Name,
				proto.ResolutionPriority.ToString("N0", character),
				proto.AppliesToCharacterProg.MXPClickableFunctionName(),
				proto.Echoes.Count.ToString("N0", character),
				TimeSpan.FromSeconds(proto.Delays.Sum(x => x.TotalSeconds)).DescribePreciseBrief(character)
			},

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Character Intro Template #{item.Id:N0}",
		DefaultCommandHelp = ChargenModule.IntroTemplateHelp
	};

	public static EditableItemHelper NewPlayerHintHelper { get; } = new()
	{
		ItemName = "New Player Hint",
		ItemNamePlural = "New Player Hints",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<INewPlayerHint>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<INewPlayerHint>(actor) { EditingItem = (INewPlayerHint)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<INewPlayerHint>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.NewPlayerHints.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.NewPlayerHints.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.NewPlayerHints.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((INewPlayerHint)item),
		CastToType = typeof(INewPlayerHint),
		EditableNewAction = (actor, input) =>
		{
			void NewItemPost(string text, IOutputHandler handler, object[] args)
			{
				var hint = new RPG.Hints.NewPlayerHint(actor.Gameworld, text);
				actor.Gameworld.Add(hint);
				actor.RemoveAllEffects<BuilderEditingEffect<INewPlayerHint>>();
				actor.AddEffect(new BuilderEditingEffect<INewPlayerHint>(actor) { EditingItem = hint });
				actor.OutputHandler.Send($"You create new player hint #{hint.Id.ToString("N0", actor)}, which you are now editing.");
			}

			void NewItemCancel(IOutputHandler handler, object[] args)
			{
				handler.Send("You decide not to create a new player hint.");
			}

			actor.OutputHandler.Send("Please enter the text that will be shown for this hint.");
			actor.EditorMode(NewItemPost, NewItemCancel, 1.0, null);
		},
		EditableCloneAction = null,
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Repeatable?",
			"Filter",
			"Priority"
		},

		GetListTableContentsFunc = (character, protos) =>
			from proto in protos.OfType<INewPlayerHint>()
			orderby proto.Priority descending
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.CanRepeat.ToColouredString(),
				proto.FilterProg?.MXPClickableFunctionName() ?? "",
				proto.Priority.ToString("N0", character)
			},

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"New Player Hint #{item.Id:N0}",
		DefaultCommandHelp = BuilderModule.NewPlayerHintHelp
	};

	public static EditableItemHelper ChargenAdviceHelper { get; } = new()
	{
		ItemName = "Chargen Advice",
		ItemNamePlural = "Chargen Advices",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IChargenAdvice>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IChargenAdvice>(actor) { EditingItem = (IChargenAdvice)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenAdvice>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ChargenAdvices.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ChargenAdvices.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ChargenAdvices.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IChargenAdvice)item),
		CastToType = typeof(IChargenAdvice),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What stage of character creation should this advice apply to?\nOptions are: {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
				return;
			}

			if (!input.PopSpeech().TryParseEnum<ChargenStage>(out var stage))
			{
				actor.OutputHandler.Send(
					$"That is not a valid character creation stage.\nOptions are: {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a title for your chargen advice.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();

			actor.OutputHandler.Send("Enter the content of the advice in the editor below.\n\n");
			actor.EditorMode((text, handler, args) =>
			{
				var advice = new ChargenAdvice(actor.Gameworld, name, text, stage);
				actor.Gameworld.Add(advice);
				actor.RemoveAllEffects<BuilderEditingEffect<IChargenAdvice>>();
				actor.AddEffect(new BuilderEditingEffect<IChargenAdvice>(actor) { EditingItem = advice });
				handler.Send(
					$"You create a new character creation advice titled {name.ColourName()}, which you are now editing.");
			}, (handler, objects) => { handler.Send("You decide not to create a character creation advice."); }, 1.0);
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which piece of character creation advice do you want to clone?");
				return;
			}

			var target = actor.Gameworld.ChargenAdvices.GetByIdOrName(input.SafeRemainingArgument);
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such character creation advice to clone.");
				return;
			}

			var clone = new ChargenAdvice(target);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IChargenAdvice>>();
			actor.AddEffect(new BuilderEditingEffect<IChargenAdvice>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You create a cloned character creation advice titled {clone.Name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Title",
			"Prog",
			"Races",
			"Cultures",
			"Ethnicities",
			"Roles"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IChargenAdvice>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.AdviceTitle,
			                                                  proto.ShouldShowAdviceProg?.MXPClickableFunctionName() ??
			                                                  "None",
			                                                  proto.Gameworld.Races
			                                                       .Count(x => x.ChargenAdvices.Contains(proto))
			                                                       .ToString("N0", character),
			                                                  proto.Gameworld.Cultures
			                                                       .Count(x => x.ChargenAdvices.Contains(proto))
			                                                       .ToString("N0", character),
			                                                  proto.Gameworld.Ethnicities
			                                                       .Count(x => x.ChargenAdvices.Contains(proto))
			                                                       .ToString("N0", character),
			                                                  proto.Gameworld.Roles
			                                                       .Count(x => x.ChargenAdvices.Contains(proto))
			                                                       .ToString("N0", character)
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Chargen Advice #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = BuilderModule.ChargenAdviceHelp
	};

	public static EditableItemHelper MeritHelper { get; } = new()
	{
		ItemName = "Merit",
		ItemNamePlural = "Merits",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMerit>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMerit>(actor) { EditingItem = (IMerit)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMerit>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Merits.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Merits.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Merits.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMerit)item),
		CastToType = typeof(IMerit),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"What type of flaw do you want to create? The valid options are {MeritFactory.Types.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			var type = input.PopSpeech();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the character intro template?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.Merits.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a merit with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var merit = MeritFactory.LoadMeritFromBuilder(actor.Gameworld, type, name);
			if (merit is null)
			{
				actor.OutputHandler.Send($"The text {type.ColourCommand()} is not a valid type of merit. The valid options are {MeritFactory.Types.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			actor.Gameworld.Add(merit);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<IMerit>>());
			actor.AddEffect(new BuilderEditingEffect<IMerit>(actor) { EditingItem = merit });
			actor.OutputHandler.Send($"You create a new merit of type {merit.DatabaseType.ColourValue()} called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which merit do you want to clone?");
				return;
			}

			var template = actor.Gameworld.Merits.GetByIdOrName(input.PopSpeech());
			if (template is null)
			{
				actor.OutputHandler.Send("There is no such merit.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new merit?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.Merits.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a merit with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var merit = template.Clone(name);
			actor.Gameworld.Add(merit);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<IMerit>>());
			actor.AddEffect(new BuilderEditingEffect<IMerit>(actor) { EditingItem = merit });
			actor.OutputHandler.Send($"You create a new merit called {name.ColourName()} as a clone of {template.Name.ColourName()}, which you are now editing.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Type",
			"Scope",
			"Flaw?",
			"Chargen Prog",
			"Applies Prog"
		},

		GetListTableContentsFunc = (character, protos) =>
			from proto in protos.OfType<IMerit>()
			let cmerit = proto as ICharacterMerit
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.Name,
				proto.DatabaseType ?? "",
				proto.MeritScope.DescribeEnum(),
				proto.MeritType.DescribeEnum(),
				cmerit?.ChargenAvailableProg?.MXPClickableFunctionName() ?? "",
				proto.ApplicabilityProg?.MXPClickableFunctionName() ?? ""
			},

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Merit #{item.Id:N0}",
		DefaultCommandHelp = ChargenModule.MeritHelpText
	};
}