using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using MudSharp.Work.Crafts;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;

namespace MudSharp.Commands.Helpers;

internal class EditableRevisableItemHelper
{
	static EditableRevisableItemHelper()
	{
		#region NPC Template

		NpcTemplateHelper = new EditableRevisableItemHelper
		{
			ItemName = "NPC Template",
			ItemNamePlural = "NPC Templates",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					var dbproto = FMDB.Context.NpcTemplates.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.NpcTemplates.Remove(dbproto);
						var dbeditable = FMDB.Context.EditableItems.Find(dbproto.EditableItemId);
						FMDB.Context.EditableItems.Remove(dbeditable);
						FMDB.Context.SaveChanges();
					}
				}

				item.Gameworld.Destroy((INPCTemplate)item);
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<INPCTemplate>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<INPCTemplate>(character)
					{ EditingItem = (INPCTemplate)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<INPCTemplate>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (character, input) =>
			{
				var cmd = input.PopSpeech().ToLowerInvariant();
				if (cmd.Length == 0)
				{
					character.OutputHandler.Send(
						$"What type of NPC Template do you wish to create? Specify either {"simple".ColourCommand()} or {"variable".ColourCommand()}.");
					return;
				}

				INPCTemplate template;

				switch (cmd)
				{
					case "simple":
						template = new SimpleNPCTemplate(character.Gameworld, character.Account);
						break;
					case "variable":
						template = new VariableNPCTemplate(character.Gameworld,
							character.Account);
						break;
					default:
						character.OutputHandler.Send(
							$"What type of NPC Template do you wish to create? Specify either {"simple".ColourCommand()} or {"variable".ColourCommand()}.");
						return;
				}

				character.Gameworld.Add(template);
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<INPCTemplate>>());
				character.AddEffect(new BuilderEditingEffect<INPCTemplate>(character) { EditingItem = template });
				character.OutputHandler.Send(
					$"You create a new {cmd} NPC Template with ID #{template.Id.ToString("N0", character)}, which you are now editing.");
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((INPCTemplate)item),
			GetAllEditableItems = character => character.Gameworld.NpcTemplates,
			GetAllEditableItemsByIdFunc = (character, id) => character.Gameworld.NpcTemplates.GetAll(id),
			GetEditableItemByIdFunc = (character, id) => character.Gameworld.NpcTemplates.Get(id),
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) => character.Gameworld.NpcTemplates.Get(id, revision),
			GetReviewTableContentsFunc = (character, items) =>
			{
				using (new FMDB())
				{
					return from item in items.OfType<INPCTemplate>()
					       select
						       new[]
						       {
							       item.Id.ToString(character), item.RevisionNumber.ToString(character),
							       item.Name.Proper(),
							       item.Keywords.ListToString(separator: ", ", conjunction: "", twoItemJoiner: ""),
							       item.NPCTemplateType,
							       FMDB.Context.Accounts.Find(item.BuilderAccountID).Name,
							       item.BuilderComment
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Keywords", "Type", "Builder", "Comment" },
			GetListTableContentsFunc = (character, items) => from item in items.OfType<INPCTemplate>()
			                                                 select
				                                                 new[]
				                                                 {
					                                                 item.Id.ToString(character),
					                                                 item.RevisionNumber.ToString(character),
					                                                 item.Name.Proper(),
					                                                 item.NPCTemplateType,
					                                                 item.Status.Describe(),
					                                                 character.Gameworld.NPCs.OfType<NPC.NPC>()
					                                                          .Count(x => x.Template == item)
					                                                          .ToString(character).MXPSend($"npc instances {item.Id}", "View the instances")
				                                                 },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Type", "Status", "Instances" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<INPCTemplate>(character, protos.Cast<INPCTemplate>().ToList())),
			CastToType = typeof(INPCTemplate),
			CustomSearch = (protos, keyword, gameworld) =>
			{
				if (keyword[0] == '*')
				{
					keyword = keyword.Substring(1);
					var ai = gameworld.AIs.GetByIdOrName(keyword);
					if (ai is null)
					{
						return protos;
					}
					return protos.OfType<INPCTemplate>()
								 .Where(x => x.ArtificialIntelligences.Contains(ai))
								 .ToList<IEditableRevisableItem>();
				}

				return protos;
			},
			DefaultCommandHelp = NPCBuilderModule.NPCHelp
		};

		#endregion

		#region Item Template

		GameItemHelper = new EditableRevisableItemHelper
		{
			ItemName = "Item Prototype",
			ItemNamePlural = "Item Prototypes",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					var dbproto = FMDB.Context.GameItemProtos.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.GameItemProtos.Remove(dbproto);
						var dbeditable = FMDB.Context.EditableItems.Find(dbproto.EditableItemId);
						FMDB.Context.EditableItems.Remove(dbeditable);
						FMDB.Context.SaveChanges();
					}

					item.Gameworld.Destroy((IGameItemProto)item);
				}
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IGameItemProto>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<IGameItemProto>(character)
					{ EditingItem = (IGameItemProto)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<IGameItemProto>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (actor, input) =>
			{
				var cmd = input.PopSpeech().ToLowerInvariant();
				if (cmd.Length == 0)
				{
					var item = new GameItemProto(actor.Gameworld, actor.Account);
					actor.Gameworld.Add(item);
					actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IGameItemProto>>());
					actor.AddEffect(new BuilderEditingEffect<IGameItemProto>(actor) { EditingItem = item });
					actor.OutputHandler.Send("You create a new item prototype with ID " + item.Id + ".");
				}
				else
				{
					// Handle "new"ing with a specified ID
					actor.OutputHandler.Send("Creating an item with a specific ID is not yet supported.");
				}
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((IGameItemProto)item),
			GetAllEditableItems = character => character.Gameworld.ItemProtos,
			GetAllEditableItemsByIdFunc = (character, id) => character.Gameworld.ItemProtos.GetAll(id),
			GetEditableItemByIdFunc = (character, id) => character.Gameworld.ItemProtos.Get(id),
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) => character.Gameworld.ItemProtos.Get(id, revision),
			GetReviewTableContentsFunc = (actor, protos) =>
			{
				using (new FMDB())
				{
					return from proto in protos.OfType<IGameItemProto>()
					       select
						       new[]
						       {
							       proto.Id.ToString(), proto.RevisionNumber.ToString(), proto.Name.Proper(),
							       proto.ShortDescription,
							       proto.Keywords.ListToString(separator: " ", conjunction: "", twoItemJoiner: ""),
							       FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment ?? ""
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Short Description", "Keywords", "Builder", "Comment" },
			GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IGameItemProto>()
			                                                  select
				                                                  new[]
				                                                  {
					                                                  proto.Id.ToString(),
					                                                  proto.RevisionNumber.ToString(),
					                                                  proto.Name.Proper(),
					                                                  proto.ShortDescription,
					                                                  proto.Status.Describe()
				                                                  },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Short Description", "Status" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<IGameItemProto>(character,
							protos.Cast<IGameItemProto>().ToList())),
			CastToType = typeof(IGameItemProto),
			CustomSearch = (protos, keyword, gameworld) =>
			{
				if (keyword[0] == '*')
				{
					keyword = keyword.Substring(1);
					var tags = gameworld.Tags.FindMatchingTags(keyword);
					return protos.OfType<IGameItemProto>()
					             .Where(x => x.Tags.Any(y => tags.Any(z => y.IsA(z))))
					             .ToList<IEditableRevisableItem>();
				}

				return protos.OfType<IGameItemProto>()
				             .Where(x => x.Components.Any(
					             y => y.TypeDescription.StartsWith(
						             keyword, StringComparison.InvariantCultureIgnoreCase)))
				             .ToList<IEditableRevisableItem>();
			}
		};

		#endregion

		#region Foragable

		ForagableHelper = new EditableRevisableItemHelper
		{
			ItemName = "Foragable",
			ItemNamePlural = "Foragables",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					var dbproto = FMDB.Context.Foragables.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.Foragables.Remove(dbproto);
						var dbeditable = FMDB.Context.EditableItems.Find(dbproto.EditableItemId);
						FMDB.Context.EditableItems.Remove(dbeditable);
						FMDB.Context.SaveChanges();
					}

					item.Gameworld.Destroy((IForagable)item);
				}
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IForagable>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<IForagable>(character) { EditingItem = (IForagable)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<IForagable>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (actor, input) =>
			{
				var foragable = new Foragable(actor.Account);
				actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IForagable>>());
				actor.AddEffect(new BuilderEditingEffect<IForagable>(actor) { EditingItem = foragable });
				actor.Gameworld.Add(foragable);
				actor.OutputHandler.Send(
					$"You create a new foragable with ID {foragable.Id.ToString("N0", actor)}, which you are now editing.");
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((IForagable)item),
			GetAllEditableItems = character => character.Gameworld.Foragables,
			GetAllEditableItemsByIdFunc = (character, id) => character.Gameworld.Foragables.GetAll(id),
			GetEditableItemByIdFunc = (character, id) => character.Gameworld.Foragables.Get(id),
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) => character.Gameworld.Foragables.Get(id, revision),
			GetReviewTableContentsFunc = (actor, protos) =>
			{
				using (new FMDB())
				{
					return from proto in protos.OfType<IForagable>()
					       select
						       new[]
						       {
							       proto.Id.ToString(), proto.RevisionNumber.ToString(), proto.Name.Proper(),
							       proto.ItemProto != null
								       ? string.Format(actor, "{0} {1:N0}r{2:N0}", proto.ItemProto.Name,
									       proto.ItemProto.Id,
									       proto.ItemProto.RevisionNumber)
								       : "None",
							       proto.QuantityDiceExpression,
							       FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment ?? ""
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Proto", "Quantity", "Builder", "Comment" },
			GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IForagable>()
			                                                  select
				                                                  new[]
				                                                  {
					                                                  proto.Id.ToString(),
					                                                  proto.RevisionNumber.ToString(),
					                                                  proto.Name.Proper(),
					                                                  proto.ItemProto != null
						                                                  ? string.Format(character,
							                                                  "{0} {1:N0}r{2:N0}", proto.ItemProto.Name,
							                                                  proto.ItemProto.Id,
							                                                  proto.ItemProto.RevisionNumber)
						                                                  : "None",
					                                                  proto.QuantityDiceExpression,
					                                                  proto.Status.Describe()
				                                                  },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Proto", "Quantity", "Status" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<IForagable>(character, protos.Cast<IForagable>().ToList())),
			CastToType = typeof(IForagable)
		};

		ForagableProfileHelper = new EditableRevisableItemHelper
		{
			ItemName = "Foragable Profile",
			ItemNamePlural = "Foragable Profiles",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					var dbproto = FMDB.Context.ForagableProfiles.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.ForagableProfiles.Remove(dbproto);
						FMDB.Context.EditableItems.Remove(dbproto.EditableItem);
						FMDB.Context.SaveChanges();
					}

					item.Gameworld.Destroy((IForagableProfile)item);
				}
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IForagableProfile>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<IForagableProfile>(character)
					{ EditingItem = (IForagableProfile)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<IForagableProfile>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (actor, input) =>
			{
				var item = new ForagableProfile(actor.Account);
				actor.Gameworld.Add(item);
				actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IForagableProfile>>());
				actor.AddEffect(new BuilderEditingEffect<IForagableProfile>(actor) { EditingItem = item });
				actor.OutputHandler.Send(
					$"You create a new foragable profile with ID {item.Id.ToString("N0", actor)}, which you are now editing.");
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((IForagableProfile)item),
			GetAllEditableItems = character => character.Gameworld.ForagableProfiles,
			GetAllEditableItemsByIdFunc = (character, id) => character.Gameworld.ForagableProfiles.GetAll(id),
			GetEditableItemByIdFunc = (character, id) => character.Gameworld.ForagableProfiles.Get(id),
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) => character.Gameworld.ForagableProfiles.Get(id, revision),
			GetReviewTableContentsFunc = (actor, protos) =>
			{
				using (new FMDB())
				{
					return from proto in protos.OfType<IForagableProfile>()
					       select
						       new[]
						       {
							       proto.Id.ToString(), proto.RevisionNumber.ToString(), proto.Name.Proper(),
							       proto.Foragables.Count().ToString("N0", actor),
							       FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment ?? ""
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Foragables", "Builder", "Comment" },
			GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IForagableProfile>()
			                                                  select
				                                                  new[]
				                                                  {
					                                                  proto.Id.ToString(),
					                                                  proto.RevisionNumber.ToString(),
					                                                  proto.Name.Proper(),
					                                                  proto.Foragables.Count()
					                                                       .ToString("N0", character),
					                                                  proto.Status.Describe()
				                                                  },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Foragables", "Status" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<IForagableProfile>(character,
							protos.Cast<IForagableProfile>().ToList())),
			CastToType = typeof(IForagableProfile),
			CustomSearch = (protos, keyword, gameworld) => protos
		};

		#endregion

		#region Crafts

		CraftHelper = new EditableRevisableItemHelper
		{
			ItemName = "Craft",
			ItemNamePlural = "Crafts",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					var dbproto = FMDB.Context.Crafts.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.Crafts.Remove(dbproto);
						var dbeditable = FMDB.Context.EditableItems.Find(dbproto.EditableItemId);
						FMDB.Context.EditableItems.Remove(dbeditable);
						FMDB.Context.SaveChanges();
					}

					item.Gameworld.Destroy((ICraft)item);
				}
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ICraft>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<ICraft>(character) { EditingItem = (ICraft)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<ICraft>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (actor, input) =>
			{
				var item = new Craft(actor.Account);
				actor.Gameworld.Add(item);
				actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ICraft>>());
				actor.AddEffect(new BuilderEditingEffect<ICraft>(actor) { EditingItem = item });
				actor.OutputHandler.Send("You create a new craft with ID " +
				                         item.Id + ".");
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((ICraft)item),
			GetAllEditableItems = character => character.Gameworld.Crafts,
			GetAllEditableItemsByIdFunc = (character, id) => character.Gameworld.Crafts.GetAll(id),
			GetEditableItemByIdFunc = (character, id) => character.Gameworld.Crafts.Get(id),
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) => character.Gameworld.Crafts.Get(id, revision),
			GetReviewTableContentsFunc = (actor, protos) =>
			{
				using (new FMDB())
				{
					return from proto in protos.OfType<ICraft>()
					       select
						       new[]
						       {
							       proto.Id.ToString("N0", actor), 
							       proto.RevisionNumber.ToString("N0", actor), 
							       proto.Name.TitleCase(),
							       proto.Category.TitleCase(),
							       FMDB.Context.Accounts.Find(proto.BuilderAccountID)?.Name ?? "Unknown", 
							       proto.BuilderComment ?? "",
								   proto.CraftIsValid.ToColouredString()
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Category", "Builder", "Comment", "Valid" },
			GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<ICraft>()
			                                                  select
				                                                  new[]
				                                                  {
					                                                  proto.Id.ToString(),
					                                                  proto.RevisionNumber.ToString(),
					                                                  proto.Name.TitleCase(),
					                                                  proto.Blurb,
																	  proto.Category.TitleCase(),
					                                                  proto.Status.Describe(),
																	  proto.CraftIsValid.ToColouredString()
				                                                  },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Blurb", "Category", "Status", "Valid" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<ICraft>(character,
							protos.Cast<ICraft>().ToList())),
			CastToType = typeof(ICraft),
			CustomSearch = (protos, keyword, gameworld) =>
			{
				if (keyword.Length > 1)
				{
					var key2 = keyword.Substring(1);
					switch (char.ToLowerInvariant(keyword[0]))
					{
						case '+':
							return protos
							       .OfType<ICraft>()
							       .Where(x =>
								       x.Name.Contains(key2, StringComparison.InvariantCultureIgnoreCase) ||
								       x.Name.Contains(key2, StringComparison.InvariantCultureIgnoreCase))
							       .ToList<IEditableRevisableItem>();
						case '-':
							return protos
							       .OfType<ICraft>()
							       .Where(x =>
								       !x.Name.Contains(key2, StringComparison.InvariantCultureIgnoreCase) &&
								       !x.Name.Contains(key2, StringComparison.InvariantCultureIgnoreCase))
							       .ToList<IEditableRevisableItem>();
						case '&':
							var tag = gameworld.Tags.GetByIdOrName(key2);
							if (tag is null)
							{
								return [];
							}

							return protos
							       .OfType<ICraft>()
								   .Where(x =>
								       x.Inputs.Any(y => y.RefersToTag(tag)) ||
								       x.Products.Any(y => y.RefersToTag(tag)) ||
								       x.FailProducts.Any(y => y.RefersToTag(tag)) ||
								       x.Tools.Any(y => y.RefersToTag(tag))
									)
								    .ToList<IEditableRevisableItem>();
						case '*':
							if (!long.TryParse(key2, out var id))
							{
								return [];
							}
							return protos
							       .OfType<ICraft>()
							       .Where(x =>
									    x.Inputs.Any(y => y.RefersToItemProto(id)) ||
									    x.Products.Any(y => y.RefersToItemProto(id)) ||
									    x.FailProducts.Any(y => y.RefersToItemProto(id)) ||
									    x.Tools.Any(y => y.RefersToItemProto(id))

									)
							       .ToList<IEditableRevisableItem>();
						case '^':
							var liquid = gameworld.Liquids.GetByIdOrName(key2);
							if (liquid is null)
							{
								return [];
							}

							return protos
							       .OfType<ICraft>()
							       .Where(x =>
								       x.Inputs.Any(y => y.RefersToLiquid(liquid)) ||
								       x.Products.Any(y => y.RefersToLiquid(liquid)) ||
								       x.FailProducts.Any(y => y.RefersToLiquid(liquid)) ||
								       x.Tools.Any(y => y.RefersToLiquid(liquid))
							       )
							       .ToList<IEditableRevisableItem>();
					}
				}
				return protos.OfType<ICraft>()
				             .Where(x => x.Category.StartsWith(keyword,
					             StringComparison.InvariantCultureIgnoreCase))
				             .ToList<IEditableRevisableItem>();
			}
		};

		#endregion

		#region Projects

		ProjectHelper = new EditableRevisableItemHelper
		{
			ItemName = "Project",
			ItemNamePlural = "Projects",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					var dbproto = FMDB.Context.Projects.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.Projects.Remove(dbproto);
						var dbeditable = FMDB.Context.EditableItems.Find(dbproto.EditableItemId);
						FMDB.Context.EditableItems.Remove(dbeditable);
						FMDB.Context.SaveChanges();
					}

					item.Gameworld.Destroy((IProject)item);
				}
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IProject>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<IProject>(character) { EditingItem = (IProject)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<IProject>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (actor, input) =>
			{
				if (input.IsFinished)
				{
					actor.OutputHandler.Send(
						$"What type of project do you want to create?\nValid options are {ProjectFactory.ValidProjectTypes.Select(x => x.ColourValue()).ListToString()}.");
					return;
				}

				var item = ProjectFactory.CreateProject(actor.Account, actor.Gameworld, input.PopSpeech());
				if (item == null)
				{
					actor.OutputHandler.Send(
						$"That is not a valid type of project.\nValid options are {ProjectFactory.ValidProjectTypes.Select(x => x.ColourValue()).ListToString()}.");
					return;
				}

				actor.Gameworld.Add(item);
				actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IProject>>());
				actor.AddEffect(new BuilderEditingEffect<IProject>(actor) { EditingItem = item });
				actor.OutputHandler.Send("You create a new project with ID " +
				                         item.Id + ".");
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((IProject)item),
			GetAllEditableItems = character => character.Gameworld.Projects,
			GetAllEditableItemsByIdFunc = (character, id) => character.Gameworld.Projects.GetAll(id),
			GetEditableItemByIdFunc = (character, id) => character.Gameworld.Projects.Get(id),
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) => character.Gameworld.Projects.Get(id, revision),
			GetReviewTableContentsFunc = (actor, protos) =>
			{
				using (new FMDB())
				{
					return from proto in protos.OfType<IProject>()
					       select
						       new[]
						       {
							       proto.Id.ToString(), proto.RevisionNumber.ToString(), proto.Name.Proper(),
							       FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment ?? ""
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Builder", "Comment" },
			GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IProject>()
			                                                  select
				                                                  new[]
				                                                  {
					                                                  proto.Id.ToString(),
					                                                  proto.RevisionNumber.ToString(),
					                                                  proto.Name.Proper(),
					                                                  proto.Tagline,
					                                                  proto.Status.Describe()
				                                                  },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Tagline", "Status" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<IProject>(character,
							protos.Cast<IProject>().ToList())),
			CastToType = typeof(IProject),
			CustomSearch = (protos, keyword, gameworld) => protos
		};

		#endregion

		#region Tattoos

		TattooHelper = new EditableRevisableItemHelper
		{
			ItemName = "Tattoo Template",
			ItemNamePlural = "Tattoo Templates",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					var dbproto = FMDB.Context.DisfigurementTemplates.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.DisfigurementTemplates.Remove(dbproto);
						var dbeditable = FMDB.Context.EditableItems.Find(dbproto.EditableItemId);
						FMDB.Context.EditableItems.Remove(dbeditable);
						FMDB.Context.SaveChanges();
					}

					item.Gameworld.Destroy((IDisfigurementTemplate)item);
				}
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IDisfigurementTemplate>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<IDisfigurementTemplate>(character)
					{ EditingItem = (IDisfigurementTemplate)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<IDisfigurementTemplate>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (actor, input) =>
			{
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a name for your new tattoo.");
					return;
				}

				var name = input.PopSpeech();
				var tattoos = actor.IsAdministrator()
					? actor.Gameworld.DisfigurementTemplates.OfType<ITattooTemplate>().ToList()
					: actor.Gameworld.DisfigurementTemplates.OfType<ITattooTemplate>()
					       .Where(x => x.CanSeeTattooInList(actor)).ToList();
				if (tattoos.Any(x => x.Name.EqualTo(name)))
				{
					actor.OutputHandler.Send(
						"There is already a tattoo with that name. You must ensure that your tattoos have unique names.");
					return;
				}

				var tattoo = new TattooTemplate(actor.Account, name);
				actor.Gameworld.Add(tattoo);
				actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IDisfigurementTemplate>>());
				actor.AddEffect(new BuilderEditingEffect<IDisfigurementTemplate>(actor) { EditingItem = tattoo });
				actor.OutputHandler.Send("You create a new tattoo template with ID " + tattoo.Id + ".");
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((ITattooTemplate)item),
			GetAllEditableItems = character => character.Gameworld.DisfigurementTemplates.OfType<ITattooTemplate>(),
			GetAllEditableItemsByIdFunc = (character, id) =>
				character.Gameworld.DisfigurementTemplates.GetAll(id).OfType<ITattooTemplate>(),
			GetEditableItemByIdFunc = (character, id) =>
				character.Gameworld.DisfigurementTemplates.Get(id) as ITattooTemplate,
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) =>
					character.Gameworld.DisfigurementTemplates.Get(id, revision) as ITattooTemplate,
			GetReviewTableContentsFunc = (actor, protos) =>
			{
				using (new FMDB())
				{
					return from proto in protos.OfType<ITattooTemplate>()
					       select
						       new[]
						       {
							       proto.Id.ToString("N0", actor), proto.RevisionNumber.ToString("N0", actor),
							       proto.Name.Proper(),
							       proto.ShortDescription,
							       FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment ?? ""
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Short Description", "Builder", "Comment" },
			GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<ITattooTemplate>()
			                                                  select
				                                                  new[]
				                                                  {
					                                                  proto.Id.ToString("N0", character),
					                                                  proto.RevisionNumber.ToString("N0", character),
					                                                  proto.Name.Proper(),
					                                                  proto.ShortDescription,
					                                                  proto.Status.Describe()
				                                                  },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Short Description", "Status" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<ITattooTemplate>(character,
							protos.Cast<ITattooTemplate>().ToList())),
			CastToType = typeof(ITattooTemplate),
			CustomSearch = (protos, keyword, gameworld) => protos
		};

		#endregion

		#region Item Skins

		ItemSkinHelper = new EditableRevisableItemHelper
		{
			ItemName = "Item Skin",
			ItemNamePlural = "Item Skins",
			DeleteEditableItemAction = item =>
			{
				using (new FMDB())
				{
					foreach (var gitem in item.Gameworld.Items)
					{
						if (gitem.Skin == item)
						{
							gitem.Skin = null;
							gitem.Changed = true;
						}
					}
					item.Gameworld.SaveManager.Flush();

					var dbproto = FMDB.Context.GameItemSkins.Find(item.Id, item.RevisionNumber);
					if (dbproto != null)
					{
						FMDB.Context.GameItemSkins.Remove(dbproto);
						var dbeditable = FMDB.Context.EditableItems.Find(dbproto.EditableItemId);
						FMDB.Context.EditableItems.Remove(dbeditable);
						FMDB.Context.SaveChanges();
					}

					item.Gameworld.Destroy((IGameItemSkin)item);
				}
			},
			SetEditableItemAction = (character, item) =>
			{
				character.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IGameItemSkin>>());
				if (item == null)
				{
					return;
				}

				character.AddEffect(new BuilderEditingEffect<IGameItemSkin>(character)
					{ EditingItem = (IGameItemSkin)item });
			},
			GetEditableItemFunc = character =>
				character.EffectsOfType<BuilderEditingEffect<IGameItemSkin>>().FirstOrDefault()?.EditingItem,
			EditableNewAction = (actor, input) =>
			{
				if (input.IsFinished)
				{
					actor.OutputHandler.Send(
						"Which item prototype do you want to make a skin for? See ITEMSKIN PROTOS for a list of available items.");
					return;
				}

				if (!long.TryParse(input.PopSpeech(), out var protoid))
				{
					actor.OutputHandler.Send("You must enter a valid id number for the item prototype.");
					return;
				}

				var proto = actor.Gameworld.ItemProtos.Get(protoid);
				if (proto is null || (!actor.IsAdministrator() && !proto.PermitPlayerSkins))
				{
					actor.OutputHandler.Send(
						"That is not a valid item prototype. See ITEMSKIN PROTOS for a list of available items.");
					return;
				}

				if (input.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a name for your new skin.");
					return;
				}

				var name = input.SafeRemainingArgument;

				var skin = new GameItemSkin(actor.Account, actor.Gameworld, proto, name);
				actor.Gameworld.Add(skin);
				actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IGameItemSkin>>());
				actor.AddEffect(new BuilderEditingEffect<IGameItemSkin>(actor) { EditingItem = skin });
				actor.OutputHandler.Send($"You create a new item skin with ID {skin.Id}.");
			},
			AddItemToGameWorldAction = item => item.Gameworld.Add((IGameItemSkin)item),
			GetAllEditableItems = character =>
				character.IsAdministrator()
					? character.Gameworld.ItemSkins
					: character.Gameworld.ItemSkins.Where(x => x.IsPublic || x.IsAssociatedBuilder(character)),
			GetAllEditableItemsByIdFunc = (character, id) =>
				(character.IsAdministrator()
					? character.Gameworld.ItemSkins
					: character.Gameworld.ItemSkins.Where(x => x.IsPublic || x.IsAssociatedBuilder(character)))
				.Where(x => x.Id == id),
			GetEditableItemByIdFunc = (character, id) => character.Gameworld.ItemSkins.Get(id,
				character.IsAdministrator()
					? item => true
					: item => item.IsPublic || item.IsAssociatedBuilder(character)),
			GetEditableItemByIdRevNumFunc =
				(character, id, revision) => character.Gameworld.ItemSkins.Get(id, revision,
					character.IsAdministrator()
						? item => true
						: item => item.IsPublic || item.IsAssociatedBuilder(character)),
			GetReviewTableContentsFunc = (actor, protos) =>
			{
				using (new FMDB())
				{
					return from proto in protos.OfType<IGameItemSkin>()
					       select
						       new[]
						       {
							       proto.Id.ToString("N0", actor), proto.RevisionNumber.ToString("N0", actor),
							       proto.Name.Proper(),
							       proto.ItemProto.EditHeader(),
							       FMDB.Context.Accounts.Find(proto.BuilderAccountID)!.Name, proto.BuilderComment ?? ""
						       };
				}
			},
			GetReviewTableHeaderFunc =
				character => new[] { "ID#", "Rev#", "Name", "Item", "Builder", "Comment" },
			GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IGameItemSkin>()
			                                                  select
				                                                  new[]
				                                                  {
					                                                  proto.Id.ToString("N0", character),
					                                                  proto.RevisionNumber.ToString("N0", character),
					                                                  proto.Name.Proper(),
					                                                  proto.ItemProto.EditHeader(),
					                                                  proto.IsPublic.ToColouredString(),
					                                                  proto.Status.Describe()
				                                                  },
			GetListTableHeaderFunc = character => new[] { "ID#", "Rev#", "Name", "Item", "Public", "Status" },
			GetReviewProposalEffectFunc =
				(protos, character) =>
					new Accept(character,
						new EditableItemReviewProposal<IGameItemSkin>(character,
							protos.Cast<IGameItemSkin>().ToList())),
			CastToType = typeof(IGameItemSkin),
			CustomSearch = (protos, keyword, gameworld) => protos,
			CanReviewFunc = x => x.IsAdministrator(),
			CanViewItemFunc = (ch, item) => ch.IsAdministrator() || ((IGameItemSkin)item).IsPublic,
			CanEditItemFunc = (ch, item) => ch.IsAdministrator() || item.IsAssociatedBuilder(ch)
		};

		#endregion
	}

	public static EditableRevisableItemHelper GameItemHelper { get; }
	public static EditableRevisableItemHelper GameItemComponentHelper { get; private set; }
	public static EditableRevisableItemHelper NpcTemplateHelper { get; }
	public static EditableRevisableItemHelper ForagableHelper { get; }
	public static EditableRevisableItemHelper ForagableProfileHelper { get; }
	public static EditableRevisableItemHelper CraftHelper { get; }
	public static EditableRevisableItemHelper ProjectHelper { get; }
	public static EditableRevisableItemHelper TattooHelper { get; }
	public static EditableRevisableItemHelper ItemSkinHelper { get; }

	public string ItemName { get; private set; }
	public string ItemNamePlural { get; private set; }
	public Action<ICharacter, IEditableRevisableItem> SetEditableItemAction { get; private set; }
	public Func<ICharacter, IEditableRevisableItem> GetEditableItemFunc { get; private set; }
	public Action<IEditableRevisableItem> DeleteEditableItemAction { get; private set; }
	public Action<ICharacter, StringStack> EditableNewAction { get; private set; }
	public Func<ICharacter, long, IEnumerable<IEditableRevisableItem>> GetAllEditableItemsByIdFunc { get; private set; }
	public Func<ICharacter, long, int, IEditableRevisableItem> GetEditableItemByIdRevNumFunc { get; private set; }
	public Func<ICharacter, long, IEditableRevisableItem> GetEditableItemByIdFunc { get; private set; }
	public Action<IEditableRevisableItem> AddItemToGameWorldAction { get; private set; }
	public Func<ICharacter, IEnumerable<IEditableRevisableItem>> GetAllEditableItems { get; private set; }

	public Func<ICharacter, IEnumerable<IEditableRevisableItem>, IEnumerable<IEnumerable<string>>>
		GetReviewTableContentsFunc { get; private set; }

	public Func<ICharacter, IEnumerable<string>> GetReviewTableHeaderFunc { get; private set; }

	public Func<ICharacter, IEnumerable<IEditableRevisableItem>, IEnumerable<IEnumerable<string>>>
		GetListTableContentsFunc { get; private set; }

	public Func<ICharacter, IEnumerable<string>> GetListTableHeaderFunc { get; private set; }

	public Func<List<IEditableRevisableItem>, ICharacter, IEffect> GetReviewProposalEffectFunc { get; private set; }

	public Func<List<IEditableRevisableItem>, string, IFuturemud, List<IEditableRevisableItem>> CustomSearch
	{
		get;
		private set;
	}

	public Func<ICharacter, bool> CanReviewFunc { get; private set; } = x => true;
	public Func<ICharacter, IEditableRevisableItem, bool> CanViewItemFunc { get; private set; } = (x, y) => true;
	public Func<ICharacter, IEditableRevisableItem, bool> CanEditItemFunc { get; private set; } = (x, y) => true;

	public Type CastToType { get; private set; }
	public string DefaultCommandHelp { get; private set; }
}