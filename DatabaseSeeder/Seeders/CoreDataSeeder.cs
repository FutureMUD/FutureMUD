using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Humanizer;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using TimeZoneInfo = MudSharp.Models.TimeZoneInfo;

namespace DatabaseSeeder.Seeders;

public class CoreDataSeeder : IDatabaseSeeder
{
	private static readonly Regex EmailRegex =
		new(
			@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|edu|gov|us|mil|biz|info|mobi|name|aero|asia|jobs|museum)$",
			RegexOptions.IgnoreCase);

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		var now = DateTime.UtcNow;

		var transaction = context.Database.BeginTransaction();
		// Set up account authorities
		var producer = SeedAuthorities(context);

		// God Account

		var dbaccount = new Account
		{
			Name = questionAnswers["account"],
			Salt = 8675309,
			Password = SecurityUtilities.GetPasswordHash(questionAnswers["password"], 8675309),
			FormatLength = 180,
			InnerFormatLength = 80,
			PageLength = 100,
			CultureName = "en-US",
			TimeZoneId = "Eastern Standard Time",
			UseUnicode = false,
			Email = questionAnswers["email"],
			IsRegistered = true,
			UnitPreference = "metric",
			ActiveCharactersAllowed = 100,
			CreationDate = now,
			RegistrationCode = "abcdefgh",
			AuthorityGroup = producer
		};
		context.Accounts.Add(dbaccount);

		SeedCulturesAndTimezoneInfos(context);

		// Default Boards
		context.Boards.Add(new Board { Name = "Deaths", ShowOnLogin = true });
		context.Boards.Add(new Board { Name = "Petitions", ShowOnLogin = true });
		context.Boards.Add(new Board { Name = "Typos", ShowOnLogin = true });
		context.SaveChanges();

		// Email Templates
		var gameName = questionAnswers["gamename"];
		var condensedGameName = gameName.Pascalize();
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.NewAccountVerification,
			Content =
				@$"<html><p>Hi, {{0}}!</p><p>Welcome to {gameName}. You, or someone pretending to be you has registered an account on the game <a href=\""http://www.{condensedGameName}.com\"">{gameName}</a> with this email. If this was you, and you would like to register your account, please use the following code to register:<br><b> {{1}}</b></p><p>We hope to see you in game soon!</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{condensedGameName}.com",
			Subject = $"Welcome to {gameName}"
		});
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.AccountPasswordReset,
			Content =
				@$"<html><p>Hi, {{0}}!<p>Your account password on {gameName} has been reset by our staff member {{1}}, presumably at your request, and is now <b>{{2}}</b>. You can now login to your account with these details.<p>If you did not initiate this password change request, please email <a href=\""mailto:staff@{condensedGameName}.com?Subject=False%20Account%20Password%20Change\"">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{condensedGameName}.com",
			Subject = $"{gameName} Password Reset"
		});
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.CharacterApplicationApproved,
			Content =
				@$"<html><p>Hi, {{0}}!</p><p>Congratulations, your character application to {gameName} for the character {{1}} has been approved by {{2}}! You may now login to the game and play!</p><p>They left the following comments about your character application:</p><p>\""{{3}}\"" -{{2}}</p><p>We hope to see you in game soon!</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{condensedGameName}.com",
			Subject = $"{gameName} Character Approved"
		});
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.CharacterApplicationRejected,
			Content =
				@$"<html><p>Hi, {{0}}!</p><p>Unfortunately, your character application to {gameName} for the character {{1}} has been declined by {{2}}.</p><p>They left the following comments about your character application:</p><p>\""{{3}}\"" -{{2}}</p><p>Don\'t fret, this can happen sometimes for a variety of reasons and you can resubmit once you make the changes outlined above. If you believe you have been declined in error, or have questions regarding your application, please feel free to join our guest lounge and discuss it with us.</p><p>We hope to see you in game soon.</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{condensedGameName}.com",
			Subject = $"{gameName} Character Rejected"
		});
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.AccountPasswordChanged,
			Content =
				@$"<html><p>Hi, {{0}}!<p>Your account password on {gameName} has been recently changed by someone at IP Address {{1}}, presumably you, and is now <b>{{2}}</b>. You can now login to your account with these details.<p>If you did not initiate this password change, please email <a href=\""mailto:staff@{condensedGameName}.com?Subject=False%20Account%20Password%20Change\"">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{condensedGameName}.com",
			Subject = $"{gameName} Password Change"
		});
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.AccountEmailChanged,
			Content =
				@$"<html><p>Hi, {{0}}!<p>Your account email on {gameName} has been recently changed by someone at IP Address {{1}}, presumably you, and is now <b>{{2}}</b>.<p>If you did not initiate this email change, please email <a href=\""mailto:staff@{condensedGameName}.com?Subject=False%20Account%20Email%20Change\"">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{condensedGameName}.com",
			Subject = $"{gameName} Email Change"
		});
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.AccountRecoveryCode,
			Content =
				@$"<html><p>Hi, {{0}}!<p>You (or someone claiming to be you) has initiated a request for Account Recovery on {gameName} for your account {{0}}, from IP Address {{1}}. A code has been generated against your account which you can use to recover it:</p><p><br><b>{{2}}</b><br></p><p>If you did not initiate this Account Recovery, please email <a href=\""mailto:staff@{condensedGameName}.com?Subject=False%20Account%20Email%20Change\"">The Head Administrator</a> to commence investigative action.</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{condensedGameName}.com",
			Subject = $"{gameName} Account Recovery Code"
		});
		context.EmailTemplates.Add(new EmailTemplate
		{
			TemplateType = (int)EmailTemplateTypes.CharacterDeath,
			Content =
				@$"<html><p>Hi, {{0}}!<p>Sadly, your character {{1}} has passed away at the ripe old age of {{3}}. We do hope that {{2}} had some adventures worth remembering and perhaps even reminiscing about in years to come.</p><p>When you have had your obligatory period of mourning we do encourage you to join us again and make another character, and tell new, fresh stories!</p><p>Sincerely,<br>The {gameName} Team</p></html>",
			ReturnAddress = $"noreply@{gameName}.com",
			Subject = $"{gameName} Character Death"
		});
		context.SaveChanges();

		// Progs
		SeedCoreProgs(context, out var isadminprog, out var cancreateclanprog, out var oncreateclanprog);

		// Add wiznet

		var wiznet = new Channel
		{
			AddToGuideCommandTree = false,
			AddToPlayerCommandTree = true,
			AnnounceChannelJoiners = true,
			AnnounceMissedListeners = true,
			ChannelColour = "#1",
			Mode = 0,
			ChannelName = "Wiznet",
			ChannelListenerProg = isadminprog,
			ChannelSpeakerProg = isadminprog
		};
		wiznet.ChannelCommandWords.Add(new ChannelCommandWord
		{
			Channel = wiznet,
			Word = "*"
		});
		wiznet.ChannelCommandWords.Add(new ChannelCommandWord
		{
			Channel = wiznet,
			Word = "wiz"
		});
		wiznet.ChannelCommandWords.Add(new ChannelCommandWord
		{
			Channel = wiznet,
			Word = "wiznet"
		});
		context.Channels.Add(wiznet);

		var unknownMaterial = new Material
		{
			Name = "an unknown material",
			MaterialDescription = "an unknown material",
			Density = 1000,
			Organic = false,
			Type = 0,
			BehaviourType = 10,
			ThermalConductivity = 0,
			ElectricalConductivity = 0,
			SpecificHeatCapacity = 0,
			ImpactFracture = 2520000,
			ImpactYield = 2520000,
			ImpactStrainAtYield = 50,
			ShearFracture = 2520000,
			ShearYield = 2520000,
			ShearStrainAtYield = 50,
			YoungsModulus = 1,
			SolventVolumeRatio = 1,
			ResidueColour = "white",
			Absorbency = 0
		};
		context.Materials.Add(unknownMaterial);

		SeedMaterials(context);

		// Insert Stack Decorators
		context.StackDecorators.Add(new StackDecorator
		{
			Name = "Suffix",
			Type = "Suffix",
			Definition = "<Definition/>",
			Description = "Appends (xN) after the item's description if quantity > 0, where N is the quantity"
		});
		context.StackDecorators.Add(new StackDecorator
		{
			Name = "Pile",
			Type = "Pile",
			Definition =
				@"<Definition><Range Item=""a couple of "" Min=""1"" Max=""2""/><Range Item=""a few "" Min=""2"" Max=""5""/><Range Item=""some "" Min=""5"" Max=""10""/><Range Item=""numerous "" Min=""10"" Max=""30""/><Range Item=""many "" Min=""30"" Max=""100""/><Range Item=""a great many "" Min=""100"" Max=""1000""/><Range Item=""an enormous quantity of "" Min=""1000"" Max=""10000""/><Range Item=""countless "" Min=""10000"" Max=""1000000""/></Definition>",
			Description = @"Describes stacks in abstract terms like ""some"", ""a few"", ""many"""
		});
		context.StackDecorators.Add(new StackDecorator
		{
			Name = "Bites",
			Type = "Bites",
			Definition =
				@"<Definition><Range Item=""scraps of {0}"" Min=""0"" Max=""15""/><Range Item=""a small amount of {0}"" Min=""15"" Max=""25""/><Range Item=""less than half of {0}"" Min=""25"" Max=""40""/><Range Item=""about half of {0}"" Min=""40"" Max=""60""/><Range Item=""most of {0}"" Min=""60"" Max=""90""/><Range Item=""almost all of {0}"" Min=""90"" Max=""100""/></Definition>",
			Description = "Describes bites remaining for food in general terms"
		});
		context.StackDecorators.Add(new StackDecorator
		{
			Name = "Residue",
			Type = "Range",
			Definition =
				@"<Definition><Range Item=""a tiny amount of "" Min=""0"" Max=""0.05""/><Range Item=""a small amount of "" Min=""0.05"" Max=""0.15""/><Range Item=""a patch of "" Min=""0.15"" Max=""0.5""/><Range Item=""a sizable patch of "" Min=""0.5"" Max=""1""/><Range Item=""a significant amount of "" Min=""1"" Max=""3""/><Range Item=""an enormous quantity of "" Min=""3"" Max=""10000""/></Definition>",
			Description = @"Used internally for residue effects"
		});

		context.SaveChanges();

		// Add read-only item templates
		var holdablecomp = new GameItemComponentProto
		{
			Name = "Holdable",
			Type = "Holdable",
			RevisionNumber = 0,
			Id = 1,
			Description = "This component makes an item able to be picked up",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = "<Definition/>"
		};
		context.GameItemComponentProtos.Add(holdablecomp);

		var currencycomp = new GameItemComponentProto
		{
			Name = "CurrencyPile",
			Type = "Currency Pile",
			RevisionNumber = 0,
			Id = 2,
			Description = "This is used for the readonly currency-pile template",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = $"<Definition Decorator=\"{context.StackDecorators.First(x => x.Name == "Pile").Id}\"/>"
		};
		context.GameItemComponentProtos.Add(currencycomp);

		var currencyItem = new GameItemProto
		{
			Id = 1,
			RevisionNumber = 0,
			Name = "Currency",
			Keywords = "currency",
			MaterialId = unknownMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = 3,
			Weight = 0,
			ReadOnly = true,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a currency pile object",
			FullDescription = "WARNING: Do not load this item manually"
		};
		currencyItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = currencycomp,
			GameItemProto = currencyItem
		});
		currencyItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = holdablecomp,
			GameItemProto = currencyItem
		});
		context.GameItemProtos.Add(currencyItem);

		var corpseComponent = new GameItemComponentProto
		{
			Name = "Corpse",
			Type = "Corpse",
			RevisionNumber = 0,
			Id = 3,
			Description = "This is used for the readonly corpse template",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = "<Definition/>"
		};

		context.GameItemComponentProtos.Add(corpseComponent);

		var corpseItem = new GameItemProto
		{
			Id = 2,
			RevisionNumber = 0,
			Name = "Corpse",
			Keywords = "corpse",
			MaterialId = unknownMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = 6,
			Weight = 0,
			ReadOnly = true,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a corpse",
			FullDescription = "WARNING: Do not load this item manually"
		};
		corpseItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = corpseComponent,
			GameItemProto = corpseItem
		});
		corpseItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = holdablecomp,
			GameItemProto = corpseItem
		});
		context.GameItemProtos.Add(corpseItem);

		var bodypartComponent = new GameItemComponentProto
		{
			Name = "Bodypart",
			Type = "Bodypart",
			RevisionNumber = 0,
			Id = 4,
			Description = "This is used for the readonly severed bodypart template",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = "<Definition/>"
		};

		context.GameItemComponentProtos.Add(bodypartComponent);

		var bodypartItem = new GameItemProto
		{
			Id = 3,
			RevisionNumber = 0,
			Name = "Bodypart",
			Keywords = "bodypart",
			MaterialId = unknownMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = 5,
			Weight = 0,
			ReadOnly = true,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a severed bodypart",
			FullDescription = "WARNING: Do not load this item manually"
		};
		bodypartItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = bodypartComponent,
			GameItemProto = bodypartItem
		});
		bodypartItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = holdablecomp,
			GameItemProto = bodypartItem
		});
		context.GameItemProtos.Add(bodypartItem);

		context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Name = "Stack_Pile",
			Type = "Stackable",
			RevisionNumber = 0,
			Id = 5,
			Description = "Makes an item stack using piles, e.g. a sword, a few swords, several swords, etc.",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = $"<Definition Decorator=\"{context.StackDecorators.First(x => x.Name == "Pile").Id}\"/>"
		});

		context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Name = "Stack_Number",
			Type = "Stackable",
			RevisionNumber = 0,
			Id = 6,
			Description = "Makes an item stack and uses (xN) style description after",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = $"<Definition Decorator=\"{context.StackDecorators.First(x => x.Name == "Suffix").Id}\"/>"
		});

		var activeCraftComp = new GameItemComponentProto
		{
			Name = "ActiveCraft",
			Type = "ActiveCraft",
			RevisionNumber = 0,
			Id = 7,
			Description = "This item is used only for system generated craft progress items.",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = "<Definition/>"
		};
		context.GameItemComponentProtos.Add(activeCraftComp);

		var activeCraftItem = new GameItemProto
		{
			Id = 4,
			RevisionNumber = 0,
			Name = "ActiveCraft",
			Keywords = "active craft",
			MaterialId = unknownMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = 5,
			Weight = 0,
			ReadOnly = true,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "an active craft",
			FullDescription = "WARNING: Do not load this item manually"
		};
		activeCraftItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = activeCraftComp,
			GameItemProto = activeCraftItem
		});
		context.GameItemProtos.Add(activeCraftItem);

		var pileComp = new GameItemComponentProto
		{
			Name = "Pile",
			Type = "Pile",
			RevisionNumber = 0,
			Id = 8,
			Description = "This item is used only for system generated pile items.",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition =
				$"<Definition><Decorator>{context.StackDecorators.First(x => x.Name == "Pile").Id}</Decorator></Definition>"
		};
		context.GameItemComponentProtos.Add(pileComp);

		var pileItem = new GameItemProto
		{
			Id = 5,
			RevisionNumber = 0,
			Name = "pile",
			Keywords = "pile item",
			MaterialId = unknownMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = 5,
			Weight = 0,
			ReadOnly = true,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a pile item",
			FullDescription = "WARNING: Do not load this item manually"
		};
		pileItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = pileComp,
			GameItemProto = pileItem
		});
		pileItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = holdablecomp,
			GameItemProto = pileItem
		});
		context.GameItemProtos.Add(pileItem);

		var commodityComp = new GameItemComponentProto
		{
			Name = "Commodity",
			Type = "Commodity",
			RevisionNumber = 0,
			Id = 9,
			Description = "This item is used only for system generated commodity items.",
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Definition = $"<Definition Decorator=\"{context.StackDecorators.First(x => x.Name == "Pile").Id}\"/>"
		};
		context.GameItemComponentProtos.Add(commodityComp);

		var commodityItem = new GameItemProto
		{
			Id = 6,
			RevisionNumber = 0,
			Name = "pile",
			Keywords = "commodity pile item",
			MaterialId = unknownMaterial.Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			},
			Size = 5,
			Weight = 0,
			ReadOnly = true,
			BaseItemQuality = 5,
			HighPriority = false,
			ShortDescription = "a commodity pile item",
			FullDescription = "WARNING: Do not load this item manually"
		};
		commodityItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = commodityComp,
			GameItemProto = commodityItem
		});
		commodityItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemComponent = holdablecomp,
			GameItemProto = commodityItem
		});
		context.GameItemProtos.Add(commodityItem);
		context.SaveChanges();

		// Add Universal Hearing Profile
		var hearing = new HearingProfile
		{
			Name = "Universal",
			Type = "Simple",
			SurveyDescription = "The noise level is generally low and otherwise unremarkable.",
			Definition = @"<Definition>
   <Difficulties>
	 <Difficulty Volume=""0"" Proximity=""0"">2</Difficulty>
	 <Difficulty Volume=""1"" Proximity=""0"">1</Difficulty>
	 <Difficulty Volume=""0"" Proximity=""1"">3</Difficulty>
	 <Difficulty Volume=""1"" Proximity=""1"">2</Difficulty>
	 <Difficulty Volume=""2"" Proximity=""1"">1</Difficulty>
	 <Difficulty Volume=""0"" Proximity=""2"">4</Difficulty>
	 <Difficulty Volume=""1"" Proximity=""2"">3</Difficulty>
	 <Difficulty Volume=""2"" Proximity=""2"">2</Difficulty>
	 <Difficulty Volume=""0"" Proximity=""3"">7</Difficulty>
	 <Difficulty Volume=""1"" Proximity=""3"">6</Difficulty>
	 <Difficulty Volume=""2"" Proximity=""3"">5</Difficulty>
	 <Difficulty Volume=""3"" Proximity=""3"">4</Difficulty>
	 <Difficulty Volume=""4"" Proximity=""3"">3</Difficulty>
	 <Difficulty Volume=""5"" Proximity=""3"">2</Difficulty>
	 <Difficulty Volume=""6"" Proximity=""3"">1</Difficulty>
   </Difficulties>
 </Definition>"
		};
		context.HearingProfiles.Add(hearing);

		// Default Item Group
		var itemGroup = new ItemGroup
		{
			Name = "Too Many Items",
			Keywords = "many items contents"
		};
		context.ItemGroups.Add(itemGroup);
		itemGroup.ItemGroupForms.Add(
			new ItemGroupForm
			{
				ItemGroup = itemGroup,
				Type = "Simple",
				Definition = @"<Definition>
   <Description>This location has a great many items, causing them to hardly stand out on their own without further inspection.</Description>
   <RoomDescription>There are a great many items here, too many to list individually.</RoomDescription>
   <ItemName>items</ItemName>
 </Definition>"
			});

		// Word Difficulty Model
		context.LanguageDifficultyModels.Add(new LanguageDifficultyModels
		{
			Name = "English Word List",
			Type = "WordList",
			Definition =
				@"<Model> <Name>Simple Wordlist</Name><DefaultDifficulty>7</DefaultDifficulty><Words>      <Word Text=""a"" Difficulty=""0""/><Word Text=""I"" Difficulty=""0""/><Word Text=""be"" Difficulty=""0""/><Word Text=""of"" Difficulty=""0""/><Word Text=""in"" Difficulty=""0""/><Word Text=""to"" Difficulty=""0""/><Word Text=""to"" Difficulty=""0""/><Word Text=""it"" Difficulty=""0""/><Word Text=""he"" Difficulty=""0""/><Word Text=""on"" Difficulty=""0""/><Word Text=""do"" Difficulty=""0""/><Word Text=""at"" Difficulty=""0""/><Word Text=""we"" Difficulty=""0""/><Word Text=""by"" Difficulty=""0""/><Word Text=""or"" Difficulty=""0""/><Word Text=""as"" Difficulty=""0""/><Word Text=""go"" Difficulty=""0""/><Word Text=""if"" Difficulty=""0""/><Word Text=""my"" Difficulty=""0""/><Word Text=""as"" Difficulty=""0""/><Word Text=""up"" Difficulty=""0""/><Word Text=""so"" Difficulty=""0""/><Word Text=""me"" Difficulty=""0""/><Word Text=""no"" Difficulty=""0""/><Word Text=""us"" Difficulty=""0""/><Word Text=""in"" Difficulty=""0""/><Word Text=""as"" Difficulty=""0""/><Word Text=""on"" Difficulty=""0""/><Word Text=""so"" Difficulty=""0""/><Word Text=""Mr"" Difficulty=""0""/><Word Text=""oh"" Difficulty=""0""/><Word Text=""up"" Difficulty=""0""/><Word Text=""no"" Difficulty=""0""/><Word Text=""TV"" Difficulty=""0""/><Word Text=""PM"" Difficulty=""0""/><Word Text=""ok"" Difficulty=""0""/><Word Text=""no"" Difficulty=""0""/><Word Text=""no"" Difficulty=""0""/><Word Text=""by"" Difficulty=""0""/><Word Text=""AM"" Difficulty=""0""/><Word Text=""Ms"" Difficulty=""0""/><Word Text=""ad"" Difficulty=""0""/><Word Text=""hi"" Difficulty=""0""/><Word Text=""vs"" Difficulty=""0""/><Word Text=""ie"" Difficulty=""0""/><Word Text=""in"" Difficulty=""0""/><Word Text=""OK"" Difficulty=""0""/><Word Text=""PC"" Difficulty=""0""/><Word Text=""ah"" Difficulty=""0""/><Word Text=""re"" Difficulty=""0""/><Word Text=""uh"" Difficulty=""0""/><Word Text=""no"" Difficulty=""0""/><Word Text=""ha"" Difficulty=""0""/><Word Text=""the"" Difficulty=""0""/><Word Text=""and"" Difficulty=""0""/><Word Text=""for"" Difficulty=""0""/><Word Text=""you"" Difficulty=""0""/><Word Text=""say"" Difficulty=""0""/><Word Text=""but"" Difficulty=""0""/><Word Text=""his"" Difficulty=""0""/><Word Text=""not"" Difficulty=""0""/><Word Text=""she"" Difficulty=""0""/><Word Text=""can"" Difficulty=""0""/><Word Text=""who"" Difficulty=""0""/><Word Text=""get"" Difficulty=""0""/><Word Text=""her"" Difficulty=""0""/><Word Text=""all"" Difficulty=""0""/><Word Text=""one"" Difficulty=""0""/><Word Text=""out"" Difficulty=""0""/><Word Text=""see"" Difficulty=""0""/><Word Text=""him"" Difficulty=""0""/><Word Text=""now"" Difficulty=""0""/><Word Text=""how"" Difficulty=""0""/><Word Text=""its"" Difficulty=""0""/><Word Text=""our"" Difficulty=""0""/><Word Text=""two"" Difficulty=""0""/><Word Text=""way"" Difficulty=""0""/><Word Text=""new"" Difficulty=""0""/><Word Text=""day"" Difficulty=""0""/><Word Text=""use"" Difficulty=""0""/><Word Text=""man"" Difficulty=""0""/><Word Text=""one"" Difficulty=""0""/><Word Text=""her"" Difficulty=""0""/><Word Text=""any"" Difficulty=""0""/><Word Text=""may"" Difficulty=""0""/><Word Text=""try"" Difficulty=""0""/><Word Text=""ask"" Difficulty=""0""/><Word Text=""too"" Difficulty=""0""/><Word Text=""own"" Difficulty=""0""/><Word Text=""out"" Difficulty=""0""/><Word Text=""put"" Difficulty=""0""/><Word Text=""old"" Difficulty=""0""/><Word Text=""why"" Difficulty=""0""/><Word Text=""let"" Difficulty=""0""/><Word Text=""big"" Difficulty=""0""/><Word Text=""few"" Difficulty=""0""/><Word Text=""run"" Difficulty=""0""/><Word Text=""off"" Difficulty=""0""/><Word Text=""all"" Difficulty=""0""/><Word Text=""lot"" Difficulty=""0""/><Word Text=""eye"" Difficulty=""0""/><Word Text=""job"" Difficulty=""0""/><Word Text=""far"" Difficulty=""0""/><Word Text=""yes"" Difficulty=""0""/><Word Text=""sit"" Difficulty=""0""/><Word Text=""yet"" Difficulty=""0""/><Word Text=""end"" Difficulty=""0""/><Word Text=""bad"" Difficulty=""0""/><Word Text=""pay"" Difficulty=""0""/><Word Text=""law"" Difficulty=""0""/><Word Text=""car"" Difficulty=""0""/><Word Text=""set"" Difficulty=""0""/><Word Text=""kid"" Difficulty=""0""/><Word Text=""ago"" Difficulty=""0""/><Word Text=""add"" Difficulty=""0""/><Word Text=""art"" Difficulty=""0""/><Word Text=""war"" Difficulty=""0""/><Word Text=""low"" Difficulty=""0""/><Word Text=""win"" Difficulty=""0""/><Word Text=""guy"" Difficulty=""0""/><Word Text=""air"" Difficulty=""0""/><Word Text=""boy"" Difficulty=""0""/><Word Text=""age"" Difficulty=""0""/><Word Text=""off"" Difficulty=""0""/><Word Text=""buy"" Difficulty=""0""/><Word Text=""die"" Difficulty=""0""/><Word Text=""cut"" Difficulty=""0""/><Word Text=""six"" Difficulty=""0""/><Word Text=""use"" Difficulty=""0""/><Word Text=""son"" Difficulty=""0""/><Word Text=""arm"" Difficulty=""0""/><Word Text=""tax"" Difficulty=""0""/><Word Text=""end"" Difficulty=""0""/><Word Text=""hit"" Difficulty=""0""/><Word Text=""eat"" Difficulty=""0""/><Word Text=""oil"" Difficulty=""0""/><Word Text=""red"" Difficulty=""0""/><Word Text=""per"" Difficulty=""0""/><Word Text=""top"" Difficulty=""0""/><Word Text=""bed"" Difficulty=""0""/><Word Text=""hot"" Difficulty=""0""/><Word Text=""lie"" Difficulty=""0""/><Word Text=""dog"" Difficulty=""0""/><Word Text=""cup"" Difficulty=""0""/><Word Text=""box"" Difficulty=""0""/><Word Text=""lay"" Difficulty=""0""/><Word Text=""sex"" Difficulty=""0""/><Word Text=""one"" Difficulty=""0""/><Word Text=""act"" Difficulty=""0""/><Word Text=""ten"" Difficulty=""0""/><Word Text=""gun"" Difficulty=""0""/><Word Text=""leg"" Difficulty=""0""/><Word Text=""set"" Difficulty=""0""/><Word Text=""fly"" Difficulty=""0""/><Word Text=""bit"" Difficulty=""0""/><Word Text=""top"" Difficulty=""0""/><Word Text=""far"" Difficulty=""0""/><Word Text=""sea"" Difficulty=""0""/><Word Text=""bar"" Difficulty=""0""/><Word Text=""bag"" Difficulty=""0""/><Word Text=""gas"" Difficulty=""0""/><Word Text=""Mrs"" Difficulty=""0""/><Word Text=""nor"" Difficulty=""0""/><Word Text=""key"" Difficulty=""0""/><Word Text=""own"" Difficulty=""0""/><Word Text=""act"" Difficulty=""0""/><Word Text=""sky"" Difficulty=""0""/><Word Text=""fan"" Difficulty=""0""/><Word Text=""bit"" Difficulty=""0""/><Word Text=""ice"" Difficulty=""0""/><Word Text=""sun"" Difficulty=""0""/><Word Text=""run"" Difficulty=""0""/><Word Text=""ear"" Difficulty=""0""/><Word Text=""fit"" Difficulty=""0""/><Word Text=""cry"" Difficulty=""0""/><Word Text=""egg"" Difficulty=""0""/><Word Text=""hey"" Difficulty=""0""/><Word Text=""bus"" Difficulty=""0""/><Word Text=""key"" Difficulty=""0""/><Word Text=""cut"" Difficulty=""0""/><Word Text=""tie"" Difficulty=""0""/><Word Text=""map"" Difficulty=""0""/><Word Text=""nod"" Difficulty=""0""/><Word Text=""dry"" Difficulty=""0""/><Word Text=""fat"" Difficulty=""0""/><Word Text=""lip"" Difficulty=""0""/><Word Text=""mom"" Difficulty=""0""/><Word Text=""aid"" Difficulty=""0""/><Word Text=""but"" Difficulty=""0""/><Word Text=""mix"" Difficulty=""0""/><Word Text=""cat"" Difficulty=""0""/><Word Text=""due"" Difficulty=""0""/><Word Text=""dad"" Difficulty=""0""/><Word Text=""fee"" Difficulty=""0""/><Word Text=""row"" Difficulty=""0""/><Word Text=""fix"" Difficulty=""0""/><Word Text=""his"" Difficulty=""0""/><Word Text=""era"" Difficulty=""0""/><Word Text=""fun"" Difficulty=""0""/><Word Text=""now"" Difficulty=""0""/><Word Text=""tip"" Difficulty=""0""/><Word Text=""aim"" Difficulty=""0""/><Word Text=""tie"" Difficulty=""0""/><Word Text=""Jew"" Difficulty=""0""/><Word Text=""hat"" Difficulty=""0""/><Word Text=""gay"" Difficulty=""0""/><Word Text=""fun"" Difficulty=""0""/><Word Text=""lab"" Difficulty=""0""/><Word Text=""sir"" Difficulty=""0""/><Word Text=""gap"" Difficulty=""0""/><Word Text=""sad"" Difficulty=""0""/><Word Text=""tea"" Difficulty=""0""/><Word Text=""far"" Difficulty=""0""/><Word Text=""cop"" Difficulty=""0""/><Word Text=""hit"" Difficulty=""0""/><Word Text=""pot"" Difficulty=""0""/><Word Text=""cap"" Difficulty=""0""/><Word Text=""for"" Difficulty=""0""/><Word Text=""fat"" Difficulty=""0""/><Word Text=""toy"" Difficulty=""0""/><Word Text=""due"" Difficulty=""0""/><Word Text=""dig"" Difficulty=""0""/><Word Text=""wet"" Difficulty=""0""/><Word Text=""pan"" Difficulty=""0""/><Word Text=""CEO"" Difficulty=""0""/><Word Text=""pop"" Difficulty=""0""/><Word Text=""mad"" Difficulty=""0""/><Word Text=""via"" Difficulty=""0""/><Word Text=""hip"" Difficulty=""0""/><Word Text=""bet"" Difficulty=""0""/><Word Text=""pay"" Difficulty=""0""/><Word Text=""odd"" Difficulty=""0""/><Word Text=""raw"" Difficulty=""0""/><Word Text=""top"" Difficulty=""0""/><Word Text=""joy"" Difficulty=""0""/><Word Text=""DNA"" Difficulty=""0""/><Word Text=""tap"" Difficulty=""0""/><Word Text=""lie"" Difficulty=""0""/><Word Text=""mix"" Difficulty=""0""/><Word Text=""rub"" Difficulty=""0""/><Word Text=""net"" Difficulty=""0""/><Word Text=""not"" Difficulty=""0""/><Word Text=""ill"" Difficulty=""0""/><Word Text=""can"" Difficulty=""0""/><Word Text=""owe"" Difficulty=""0""/><Word Text=""ski"" Difficulty=""0""/><Word Text=""rid"" Difficulty=""0""/><Word Text=""cow"" Difficulty=""0""/><Word Text=""etc"" Difficulty=""0""/><Word Text=""sue"" Difficulty=""0""/><Word Text=""jet"" Difficulty=""0""/><Word Text=""God"" Difficulty=""0""/><Word Text=""dry"" Difficulty=""0""/><Word Text=""nut"" Difficulty=""0""/><Word Text=""fly"" Difficulty=""0""/><Word Text=""ban"" Difficulty=""0""/><Word Text=""pet"" Difficulty=""0""/><Word Text=""sin"" Difficulty=""0""/><Word Text=""pie"" Difficulty=""0""/><Word Text=""win"" Difficulty=""0""/><Word Text=""lap"" Difficulty=""0""/><Word Text=""toe"" Difficulty=""0""/><Word Text=""fit"" Difficulty=""0""/><Word Text=""log"" Difficulty=""0""/><Word Text=""cry"" Difficulty=""0""/><Word Text=""beg"" Difficulty=""0""/><Word Text=""net"" Difficulty=""0""/><Word Text=""pig"" Difficulty=""0""/><Word Text=""pop"" Difficulty=""0""/><Word Text=""tip"" Difficulty=""0""/><Word Text=""van"" Difficulty=""0""/><Word Text=""bay"" Difficulty=""0""/><Word Text=""rat"" Difficulty=""0""/><Word Text=""rip"" Difficulty=""0""/><Word Text=""top"" Difficulty=""0""/><Word Text=""pen"" Difficulty=""0""/><Word Text=""mud"" Difficulty=""0""/><Word Text=""aim"" Difficulty=""0""/><Word Text=""pad"" Difficulty=""0""/><Word Text=""bat"" Difficulty=""0""/><Word Text=""wow"" Difficulty=""0""/><Word Text=""sum"" Difficulty=""0""/><Word Text=""pit"" Difficulty=""0""/><Word Text=""hug"" Difficulty=""0""/><Word Text=""gym"" Difficulty=""0""/><Word Text=""age"" Difficulty=""0""/><Word Text=""ban"" Difficulty=""0""/><Word Text=""huh"" Difficulty=""0""/><Word Text=""hay"" Difficulty=""0""/><Word Text=""kit"" Difficulty=""0""/><Word Text=""way"" Difficulty=""0""/><Word Text=""pro"" Difficulty=""0""/><Word Text=""bid"" Difficulty=""0""/><Word Text=""jaw"" Difficulty=""0""/><Word Text=""bow"" Difficulty=""0""/><Word Text=""bug"" Difficulty=""0""/><Word Text=""ass"" Difficulty=""0""/><Word Text=""cab"" Difficulty=""0""/><Word Text=""web"" Difficulty=""0""/><Word Text=""bee"" Difficulty=""0""/><Word Text=""jar"" Difficulty=""0""/><Word Text=""gut"" Difficulty=""0""/><Word Text=""pin"" Difficulty=""0""/><Word Text=""pro"" Difficulty=""0""/><Word Text=""dam"" Difficulty=""0""/><Word Text=""shy"" Difficulty=""0""/><Word Text=""bet"" Difficulty=""0""/><Word Text=""oak"" Difficulty=""0""/><Word Text=""tag"" Difficulty=""0""/><Word Text=""dot"" Difficulty=""0""/><Word Text=""aid"" Difficulty=""0""/><Word Text=""dip"" Difficulty=""0""/><Word Text=""pat"" Difficulty=""0""/><Word Text=""rib"" Difficulty=""0""/><Word Text=""rod"" Difficulty=""0""/><Word Text=""ash"" Difficulty=""0""/><Word Text=""rim"" Difficulty=""0""/><Word Text=""lid"" Difficulty=""0""/><Word Text=""any"" Difficulty=""0""/><Word Text=""arm"" Difficulty=""0""/><Word Text=""fur"" Difficulty=""0""/><Word Text=""opt"" Difficulty=""0""/><Word Text=""ego"" Difficulty=""0""/><Word Text=""low"" Difficulty=""0""/><Word Text=""spy"" Difficulty=""0""/><Word Text=""cue"" Difficulty=""0""/><Word Text=""bow"" Difficulty=""0""/><Word Text=""fog"" Difficulty=""0""/><Word Text=""kid"" Difficulty=""0""/><Word Text=""have"" Difficulty=""0""/><Word Text=""that"" Difficulty=""0""/><Word Text=""with"" Difficulty=""0""/><Word Text=""this"" Difficulty=""0""/><Word Text=""they"" Difficulty=""0""/><Word Text=""from"" Difficulty=""0""/><Word Text=""that"" Difficulty=""0""/><Word Text=""what"" Difficulty=""0""/><Word Text=""make"" Difficulty=""0""/><Word Text=""know"" Difficulty=""0""/><Word Text=""will"" Difficulty=""0""/><Word Text=""time"" Difficulty=""0""/><Word Text=""year"" Difficulty=""0""/><Word Text=""when"" Difficulty=""0""/><Word Text=""them"" Difficulty=""0""/><Word Text=""some"" Difficulty=""0""/><Word Text=""take"" Difficulty=""0""/><Word Text=""into"" Difficulty=""0""/><Word Text=""just"" Difficulty=""0""/><Word Text=""your"" Difficulty=""0""/><Word Text=""come"" Difficulty=""0""/><Word Text=""than"" Difficulty=""0""/><Word Text=""like"" Difficulty=""0""/><Word Text=""then"" Difficulty=""0""/><Word Text=""more"" Difficulty=""0""/><Word Text=""want"" Difficulty=""0""/><Word Text=""look"" Difficulty=""0""/><Word Text=""also"" Difficulty=""0""/><Word Text=""more"" Difficulty=""0""/><Word Text=""find"" Difficulty=""0""/><Word Text=""here"" Difficulty=""0""/><Word Text=""give"" Difficulty=""0""/><Word Text=""many"" Difficulty=""0""/><Word Text=""well"" Difficulty=""0""/><Word Text=""only"" Difficulty=""0""/><Word Text=""tell"" Difficulty=""0""/><Word Text=""very"" Difficulty=""0""/><Word Text=""even"" Difficulty=""0""/><Word Text=""back"" Difficulty=""0""/><Word Text=""good"" Difficulty=""0""/><Word Text=""life"" Difficulty=""0""/><Word Text=""work"" Difficulty=""0""/><Word Text=""down"" Difficulty=""0""/><Word Text=""call"" Difficulty=""0""/><Word Text=""over"" Difficulty=""0""/><Word Text=""last"" Difficulty=""0""/><Word Text=""need"" Difficulty=""0""/><Word Text=""feel"" Difficulty=""0""/><Word Text=""when"" Difficulty=""0""/><Word Text=""high"" Difficulty=""0""/><Word Text=""most"" Difficulty=""0""/><Word Text=""much"" Difficulty=""0""/><Word Text=""mean"" Difficulty=""0""/><Word Text=""keep"" Difficulty=""0""/><Word Text=""same"" Difficulty=""0""/><Word Text=""seem"" Difficulty=""0""/><Word Text=""help"" Difficulty=""0""/><Word Text=""talk"" Difficulty=""0""/><Word Text=""turn"" Difficulty=""0""/><Word Text=""hand"" Difficulty=""0""/><Word Text=""show"" Difficulty=""0""/><Word Text=""part"" Difficulty=""0""/><Word Text=""over"" Difficulty=""0""/><Word Text=""such"" Difficulty=""0""/><Word Text=""case"" Difficulty=""0""/><Word Text=""most"" Difficulty=""0""/><Word Text=""week"" Difficulty=""0""/><Word Text=""each"" Difficulty=""0""/><Word Text=""hear"" Difficulty=""0""/><Word Text=""work"" Difficulty=""0""/><Word Text=""play"" Difficulty=""0""/><Word Text=""move"" Difficulty=""0""/><Word Text=""like"" Difficulty=""0""/><Word Text=""live"" Difficulty=""0""/><Word Text=""hold"" Difficulty=""0""/><Word Text=""next"" Difficulty=""0""/><Word Text=""must"" Difficulty=""0""/><Word Text=""home"" Difficulty=""0""/><Word Text=""room"" Difficulty=""0""/><Word Text=""area"" Difficulty=""0""/><Word Text=""fact"" Difficulty=""0""/><Word Text=""book"" Difficulty=""0""/><Word Text=""word"" Difficulty=""0""/><Word Text=""side"" Difficulty=""0""/><Word Text=""kind"" Difficulty=""0""/><Word Text=""four"" Difficulty=""0""/><Word Text=""head"" Difficulty=""0""/><Word Text=""long"" Difficulty=""0""/><Word Text=""both"" Difficulty=""0""/><Word Text=""long"" Difficulty=""0""/><Word Text=""away"" Difficulty=""0""/><Word Text=""hour"" Difficulty=""0""/><Word Text=""game"" Difficulty=""0""/><Word Text=""line"" Difficulty=""0""/><Word Text=""ever"" Difficulty=""0""/><Word Text=""lose"" Difficulty=""0""/><Word Text=""meet"" Difficulty=""0""/><Word Text=""city"" Difficulty=""0""/><Word Text=""much"" Difficulty=""0""/><Word Text=""name"" Difficulty=""0""/><Word Text=""five"" Difficulty=""0""/><Word Text=""once"" Difficulty=""0""/><Word Text=""real"" Difficulty=""0""/><Word Text=""team"" Difficulty=""0""/><Word Text=""best"" Difficulty=""0""/><Word Text=""idea"" Difficulty=""0""/><Word Text=""body"" Difficulty=""0""/><Word Text=""lead"" Difficulty=""0""/><Word Text=""back"" Difficulty=""0""/><Word Text=""only"" Difficulty=""0""/><Word Text=""stop"" Difficulty=""0""/><Word Text=""face"" Difficulty=""0""/><Word Text=""read"" Difficulty=""0""/><Word Text=""door"" Difficulty=""0""/><Word Text=""sure"" Difficulty=""0""/><Word Text=""such"" Difficulty=""0""/><Word Text=""grow"" Difficulty=""0""/><Word Text=""open"" Difficulty=""0""/><Word Text=""walk"" Difficulty=""0""/><Word Text=""girl"" Difficulty=""0""/><Word Text=""food"" Difficulty=""0""/><Word Text=""both"" Difficulty=""0""/><Word Text=""foot"" Difficulty=""0""/><Word Text=""able"" Difficulty=""0""/><Word Text=""love"" Difficulty=""0""/><Word Text=""wait"" Difficulty=""0""/><Word Text=""send"" Difficulty=""0""/><Word Text=""home"" Difficulty=""0""/><Word Text=""stay"" Difficulty=""0""/><Word Text=""fall"" Difficulty=""0""/><Word Text=""plan"" Difficulty=""0""/><Word Text=""kill"" Difficulty=""0""/><Word Text=""yeah"" Difficulty=""0""/><Word Text=""care"" Difficulty=""0""/><Word Text=""late"" Difficulty=""0""/><Word Text=""hard"" Difficulty=""0""/><Word Text=""else"" Difficulty=""0""/><Word Text=""pass"" Difficulty=""0""/><Word Text=""sell"" Difficulty=""0""/><Word Text=""role"" Difficulty=""0""/><Word Text=""rate"" Difficulty=""0""/><Word Text=""drug"" Difficulty=""0""/><Word Text=""show"" Difficulty=""0""/><Word Text=""wife"" Difficulty=""0""/><Word Text=""mind"" Difficulty=""0""/><Word Text=""pull"" Difficulty=""0""/><Word Text=""free"" Difficulty=""0""/><Word Text=""less"" Difficulty=""0""/><Word Text=""hope"" Difficulty=""0""/><Word Text=""even"" Difficulty=""0""/><Word Text=""view"" Difficulty=""0""/><Word Text=""town"" Difficulty=""0""/><Word Text=""road"" Difficulty=""0""/><Word Text=""true"" Difficulty=""0""/><Word Text=""full"" Difficulty=""0""/><Word Text=""join"" Difficulty=""0""/><Word Text=""pick"" Difficulty=""0""/><Word Text=""wear"" Difficulty=""0""/><Word Text=""form"" Difficulty=""0""/><Word Text=""site"" Difficulty=""0""/><Word Text=""base"" Difficulty=""0""/><Word Text=""star"" Difficulty=""0""/><Word Text=""need"" Difficulty=""0""/><Word Text=""half"" Difficulty=""0""/><Word Text=""easy"" Difficulty=""0""/><Word Text=""cost"" Difficulty=""0""/><Word Text=""face"" Difficulty=""0""/><Word Text=""data"" Difficulty=""0""/><Word Text=""land"" Difficulty=""0""/><Word Text=""wall"" Difficulty=""0""/><Word Text=""news"" Difficulty=""0""/><Word Text=""test"" Difficulty=""0""/><Word Text=""love"" Difficulty=""0""/><Word Text=""open"" Difficulty=""0""/><Word Text=""step"" Difficulty=""0""/><Word Text=""baby"" Difficulty=""0""/><Word Text=""type"" Difficulty=""0""/><Word Text=""draw"" Difficulty=""0""/><Word Text=""film"" Difficulty=""0""/><Word Text=""tree"" Difficulty=""0""/><Word Text=""hair"" Difficulty=""0""/><Word Text=""look"" Difficulty=""0""/><Word Text=""soon"" Difficulty=""0""/><Word Text=""less"" Difficulty=""0""/><Word Text=""term"" Difficulty=""0""/><Word Text=""rule"" Difficulty=""0""/><Word Text=""well"" Difficulty=""0""/><Word Text=""call"" Difficulty=""0""/><Word Text=""risk"" Difficulty=""0""/><Word Text=""fire"" Difficulty=""0""/><Word Text=""bank"" Difficulty=""0""/><Word Text=""west"" Difficulty=""0""/><Word Text=""seek"" Difficulty=""0""/><Word Text=""rest"" Difficulty=""0""/><Word Text=""deal"" Difficulty=""0""/><Word Text=""past"" Difficulty=""0""/><Word Text=""goal"" Difficulty=""0""/><Word Text=""fill"" Difficulty=""0""/><Word Text=""drop"" Difficulty=""0""/><Word Text=""plan"" Difficulty=""0""/><Word Text=""upon"" Difficulty=""0""/><Word Text=""push"" Difficulty=""0""/><Word Text=""note"" Difficulty=""0""/><Word Text=""fine"" Difficulty=""0""/><Word Text=""near"" Difficulty=""0""/><Word Text=""page"" Difficulty=""0""/><Word Text=""than"" Difficulty=""0""/><Word Text=""poor"" Difficulty=""0""/><Word Text=""race"" Difficulty=""0""/><Word Text=""each"" Difficulty=""0""/><Word Text=""dead"" Difficulty=""0""/><Word Text=""rise"" Difficulty=""0""/><Word Text=""east"" Difficulty=""0""/><Word Text=""save"" Difficulty=""0""/><Word Text=""away"" Difficulty=""0""/><Word Text=""thus"" Difficulty=""0""/><Word Text=""size"" Difficulty=""0""/><Word Text=""fund"" Difficulty=""0""/><Word Text=""sign"" Difficulty=""0""/><Word Text=""list"" Difficulty=""0""/><Word Text=""hard"" Difficulty=""0""/><Word Text=""left"" Difficulty=""0""/><Word Text=""loss"" Difficulty=""0""/><Word Text=""deal"" Difficulty=""0""/><Word Text=""bill"" Difficulty=""0""/><Word Text=""fail"" Difficulty=""0""/><Word Text=""name"" Difficulty=""0""/><Word Text=""miss"" Difficulty=""0""/><Word Text=""sort"" Difficulty=""0""/><Word Text=""blue"" Difficulty=""0""/><Word Text=""song"" Difficulty=""0""/><Word Text=""dark"" Difficulty=""0""/><Word Text=""hang"" Difficulty=""0""/><Word Text=""rock"" Difficulty=""0""/><Word Text=""note"" Difficulty=""0""/><Word Text=""help"" Difficulty=""0""/><Word Text=""cold"" Difficulty=""0""/><Word Text=""form"" Difficulty=""0""/><Word Text=""main"" Difficulty=""0""/><Word Text=""card"" Difficulty=""0""/><Word Text=""seat"" Difficulty=""0""/><Word Text=""cell"" Difficulty=""0""/><Word Text=""nice"" Difficulty=""0""/><Word Text=""that"" Difficulty=""0""/><Word Text=""firm"" Difficulty=""0""/><Word Text=""care"" Difficulty=""0""/><Word Text=""huge"" Difficulty=""0""/><Word Text=""ball"" Difficulty=""0""/><Word Text=""talk"" Difficulty=""0""/><Word Text=""onto"" Difficulty=""0""/><Word Text=""head"" Difficulty=""0""/><Word Text=""base"" Difficulty=""0""/><Word Text=""pain"" Difficulty=""0""/><Word Text=""play"" Difficulty=""0""/><Word Text=""wide"" Difficulty=""0""/><Word Text=""fish"" Difficulty=""0""/><Word Text=""trip"" Difficulty=""0""/><Word Text=""unit"" Difficulty=""0""/><Word Text=""best"" Difficulty=""0""/><Word Text=""deep"" Difficulty=""0""/><Word Text=""past"" Difficulty=""0""/><Word Text=""edge"" Difficulty=""0""/><Word Text=""fear"" Difficulty=""0""/><Word Text=""sign"" Difficulty=""0""/><Word Text=""heat"" Difficulty=""0""/><Word Text=""fall"" Difficulty=""0""/><Word Text=""sing"" Difficulty=""0""/><Word Text=""whom"" Difficulty=""0""/><Word Text=""skin"" Difficulty=""0""/><Word Text=""down"" Difficulty=""0""/><Word Text=""test"" Difficulty=""0""/><Word Text=""item"" Difficulty=""0""/><Word Text=""step"" Difficulty=""0""/><Word Text=""yard"" Difficulty=""0""/><Word Text=""beat"" Difficulty=""0""/><Word Text=""tend"" Difficulty=""0""/><Word Text=""task"" Difficulty=""0""/><Word Text=""shot"" Difficulty=""0""/><Word Text=""wish"" Difficulty=""0""/><Word Text=""safe"" Difficulty=""0""/><Word Text=""rich"" Difficulty=""0""/><Word Text=""vote"" Difficulty=""0""/><Word Text=""none"" Difficulty=""0""/><Word Text=""born"" Difficulty=""0""/><Word Text=""wind"" Difficulty=""0""/><Word Text=""fast"" Difficulty=""0""/><Word Text=""cost"" Difficulty=""0""/><Word Text=""like"" Difficulty=""0""/><Word Text=""bird"" Difficulty=""0""/><Word Text=""hurt"" Difficulty=""0""/><Word Text=""hope"" Difficulty=""0""/><Word Text=""nine"" Difficulty=""0""/><Word Text=""vote"" Difficulty=""0""/><Word Text=""turn"" Difficulty=""0""/><Word Text=""once"" Difficulty=""0""/><Word Text=""camp"" Difficulty=""0""/><Word Text=""date"" Difficulty=""0""/><Word Text=""very"" Difficulty=""0""/><Word Text=""hole"" Difficulty=""0""/><Word Text=""ship"" Difficulty=""0""/><Word Text=""park"" Difficulty=""0""/><Word Text=""spot"" Difficulty=""0""/><Word Text=""lack"" Difficulty=""0""/><Word Text=""boat"" Difficulty=""0""/><Word Text=""wood"" Difficulty=""0""/><Word Text=""roll"" Difficulty=""0""/><Word Text=""gain"" Difficulty=""0""/><Word Text=""hide"" Difficulty=""0""/><Word Text=""gold"" Difficulty=""0""/><Word Text=""club"" Difficulty=""0""/><Word Text=""farm"" Difficulty=""0""/><Word Text=""band"" Difficulty=""0""/><Word Text=""ride"" Difficulty=""0""/><Word Text=""text"" Difficulty=""0""/><Word Text=""tool"" Difficulty=""0""/><Word Text=""wild"" Difficulty=""0""/><Word Text=""earn"" Difficulty=""0""/><Word Text=""tiny"" Difficulty=""0""/><Word Text=""feed"" Difficulty=""0""/><Word Text=""path"" Difficulty=""0""/><Word Text=""shop"" Difficulty=""0""/><Word Text=""folk"" Difficulty=""0""/><Word Text=""lift"" Difficulty=""0""/><Word Text=""jump"" Difficulty=""0""/><Word Text=""warm"" Difficulty=""0""/><Word Text=""soft"" Difficulty=""0""/><Word Text=""gift"" Difficulty=""0""/><Word Text=""past"" Difficulty=""0""/><Word Text=""wave"" Difficulty=""0""/><Word Text=""move"" Difficulty=""0""/><Word Text=""deny"" Difficulty=""0""/><Word Text=""suit"" Difficulty=""0""/><Word Text=""blow"" Difficulty=""0""/><Word Text=""kind"" Difficulty=""0""/><Word Text=""cook"" Difficulty=""0""/><Word Text=""burn"" Difficulty=""0""/><Word Text=""shoe"" Difficulty=""0""/><Word Text=""view"" Difficulty=""0""/><Word Text=""bone"" Difficulty=""0""/><Word Text=""wine"" Difficulty=""0""/><Word Text=""cool"" Difficulty=""0""/><Word Text=""mean"" Difficulty=""0""/><Word Text=""hell"" Difficulty=""0""/><Word Text=""fire"" Difficulty=""0""/><Word Text=""tour"" Difficulty=""0""/><Word Text=""grab"" Difficulty=""0""/><Word Text=""fair"" Difficulty=""0""/><Word Text=""pair"" Difficulty=""0""/><Word Text=""knee"" Difficulty=""0""/><Word Text=""tape"" Difficulty=""0""/><Word Text=""hire"" Difficulty=""0""/><Word Text=""will"" Difficulty=""0""/><Word Text=""next"" Difficulty=""0""/><Word Text=""lady"" Difficulty=""0""/><Word Text=""neck"" Difficulty=""0""/><Word Text=""lean"" Difficulty=""0""/><Word Text=""tall"" Difficulty=""0""/><Word Text=""hate"" Difficulty=""0""/><Word Text=""male"" Difficulty=""0""/><Word Text=""army"" Difficulty=""0""/><Word Text=""shut"" Difficulty=""0""/><Word Text=""lots"" Difficulty=""0""/><Word Text=""rain"" Difficulty=""0""/><Word Text=""fuel"" Difficulty=""0""/><Word Text=""leaf"" Difficulty=""0""/><Word Text=""pool"" Difficulty=""0""/><Word Text=""lead"" Difficulty=""0""/><Word Text=""salt"" Difficulty=""0""/><Word Text=""soul"" Difficulty=""0""/><Word Text=""bear"" Difficulty=""0""/><Word Text=""thin"" Difficulty=""0""/><Word Text=""poll"" Difficulty=""0""/><Word Text=""half"" Difficulty=""0""/><Word Text=""okay"" Difficulty=""0""/><Word Text=""code"" Difficulty=""0""/><Word Text=""jury"" Difficulty=""0""/><Word Text=""desk"" Difficulty=""0""/><Word Text=""fear"" Difficulty=""0""/><Word Text=""like"" Difficulty=""0""/><Word Text=""last"" Difficulty=""0""/><Word Text=""ring"" Difficulty=""0""/><Word Text=""mark"" Difficulty=""0""/><Word Text=""loan"" Difficulty=""0""/><Word Text=""crew"" Difficulty=""0""/><Word Text=""deep"" Difficulty=""0""/><Word Text=""male"" Difficulty=""0""/><Word Text=""meal"" Difficulty=""0""/><Word Text=""cash"" Difficulty=""0""/><Word Text=""link"" Difficulty=""0""/><Word Text=""root"" Difficulty=""0""/><Word Text=""nose"" Difficulty=""0""/><Word Text=""file"" Difficulty=""0""/><Word Text=""sick"" Difficulty=""0""/><Word Text=""duty"" Difficulty=""0""/><Word Text=""slow"" Difficulty=""0""/><Word Text=""zone"" Difficulty=""0""/><Word Text=""wake"" Difficulty=""0""/><Word Text=""warn"" Difficulty=""0""/><Word Text=""snow"" Difficulty=""0""/><Word Text=""slip"" Difficulty=""0""/><Word Text=""meat"" Difficulty=""0""/><Word Text=""soil"" Difficulty=""0""/><Word Text=""late"" Difficulty=""0""/><Word Text=""golf"" Difficulty=""0""/><Word Text=""just"" Difficulty=""0""/><Word Text=""user"" Difficulty=""0""/><Word Text=""kick"" Difficulty=""0""/><Word Text=""part"" Difficulty=""0""/><Word Text=""used"" Difficulty=""0""/><Word Text=""bowl"" Difficulty=""0""/><Word Text=""long"" Difficulty=""0""/><Word Text=""host"" Difficulty=""0""/><Word Text=""hall"" Difficulty=""0""/><Word Text=""rely"" Difficulty=""0""/><Word Text=""back"" Difficulty=""0""/><Word Text=""debt"" Difficulty=""0""/><Word Text=""rare"" Difficulty=""0""/><Word Text=""tank"" Difficulty=""0""/><Word Text=""bond"" Difficulty=""0""/><Word Text=""file"" Difficulty=""0""/><Word Text=""wing"" Difficulty=""0""/><Word Text=""mean"" Difficulty=""0""/><Word Text=""pour"" Difficulty=""0""/><Word Text=""stir"" Difficulty=""0""/><Word Text=""beer"" Difficulty=""0""/><Word Text=""tear"" Difficulty=""0""/><Word Text=""hero"" Difficulty=""0""/><Word Text=""seed"" Difficulty=""0""/><Word Text=""rest"" Difficulty=""0""/><Word Text=""busy"" Difficulty=""0""/><Word Text=""copy"" Difficulty=""0""/><Word Text=""cite"" Difficulty=""0""/><Word Text=""gray"" Difficulty=""0""/><Word Text=""dish"" Difficulty=""0""/><Word Text=""core"" Difficulty=""0""/><Word Text=""rush"" Difficulty=""0""/><Word Text=""rise"" Difficulty=""0""/><Word Text=""vast"" Difficulty=""0""/><Word Text=""lack"" Difficulty=""0""/><Word Text=""flow"" Difficulty=""0""/><Word Text=""mass"" Difficulty=""0""/><Word Text=""bomb"" Difficulty=""0""/><Word Text=""tone"" Difficulty=""0""/><Word Text=""AIDS"" Difficulty=""0""/><Word Text=""gate"" Difficulty=""0""/><Word Text=""hill"" Difficulty=""0""/><Word Text=""hand"" Difficulty=""0""/><Word Text=""land"" Difficulty=""0""/><Word Text=""milk"" Difficulty=""0""/><Word Text=""cast"" Difficulty=""0""/><Word Text=""ride"" Difficulty=""0""/><Word Text=""live"" Difficulty=""0""/><Word Text=""plus"" Difficulty=""0""/><Word Text=""mind"" Difficulty=""0""/><Word Text=""weak"" Difficulty=""0""/><Word Text=""list"" Difficulty=""0""/><Word Text=""wrap"" Difficulty=""0""/><Word Text=""mark"" Difficulty=""0""/><Word Text=""drag"" Difficulty=""0""/><Word Text=""roof"" Difficulty=""0""/><Word Text=""diet"" Difficulty=""0""/><Word Text=""wash"" Difficulty=""0""/><Word Text=""post"" Difficulty=""0""/><Word Text=""dark"" Difficulty=""0""/><Word Text=""chip"" Difficulty=""0""/><Word Text=""self"" Difficulty=""0""/><Word Text=""bike"" Difficulty=""0""/><Word Text=""slow"" Difficulty=""0""/><Word Text=""link"" Difficulty=""0""/><Word Text=""mass"" Difficulty=""0""/><Word Text=""lake"" Difficulty=""0""/><Word Text=""bend"" Difficulty=""0""/><Word Text=""gain"" Difficulty=""0""/><Word Text=""Arab"" Difficulty=""0""/><Word Text=""walk"" Difficulty=""0""/><Word Text=""sand"" Difficulty=""0""/><Word Text=""rule"" Difficulty=""0""/><Word Text=""lock"" Difficulty=""0""/><Word Text=""tear"" Difficulty=""0""/><Word Text=""pose"" Difficulty=""0""/><Word Text=""sale"" Difficulty=""0""/><Word Text=""mine"" Difficulty=""0""/><Word Text=""tale"" Difficulty=""0""/><Word Text=""joke"" Difficulty=""0""/><Word Text=""coat"" Difficulty=""0""/><Word Text=""pass"" Difficulty=""0""/><Word Text=""good"" Difficulty=""0""/><Word Text=""urge"" Difficulty=""0""/><Word Text=""dust"" Difficulty=""0""/><Word Text=""glad"" Difficulty=""0""/><Word Text=""pack"" Difficulty=""0""/><Word Text=""iron"" Difficulty=""0""/><Word Text=""gene"" Difficulty=""0""/><Word Text=""sure"" Difficulty=""0""/><Word Text=""kiss"" Difficulty=""0""/><Word Text=""boss"" Difficulty=""0""/><Word Text=""king"" Difficulty=""0""/><Word Text=""mood"" Difficulty=""0""/><Word Text=""boot"" Difficulty=""0""/><Word Text=""bean"" Difficulty=""0""/><Word Text=""peak"" Difficulty=""0""/><Word Text=""vary"" Difficulty=""0""/><Word Text=""wire"" Difficulty=""0""/><Word Text=""holy"" Difficulty=""0""/><Word Text=""ring"" Difficulty=""0""/><Word Text=""twin"" Difficulty=""0""/><Word Text=""stop"" Difficulty=""0""/><Word Text=""luck"" Difficulty=""0""/><Word Text=""race"" Difficulty=""0""/><Word Text=""toss"" Difficulty=""0""/><Word Text=""bury"" Difficulty=""0""/><Word Text=""pray"" Difficulty=""0""/><Word Text=""ally"" Difficulty=""0""/><Word Text=""pure"" Difficulty=""0""/><Word Text=""peer"" Difficulty=""0""/><Word Text=""belt"" Difficulty=""0""/><Word Text=""flag"" Difficulty=""0""/><Word Text=""corn"" Difficulty=""0""/><Word Text=""moon"" Difficulty=""0""/><Word Text=""crop"" Difficulty=""0""/><Word Text=""soon"" Difficulty=""0""/><Word Text=""line"" Difficulty=""0""/><Word Text=""date"" Difficulty=""0""/><Word Text=""pink"" Difficulty=""0""/><Word Text=""buck"" Difficulty=""0""/><Word Text=""poem"" Difficulty=""0""/><Word Text=""bind"" Difficulty=""0""/><Word Text=""mail"" Difficulty=""0""/><Word Text=""tube"" Difficulty=""0""/><Word Text=""quit"" Difficulty=""0""/><Word Text=""roll"" Difficulty=""0""/><Word Text=""jail"" Difficulty=""0""/><Word Text=""pace"" Difficulty=""0""/><Word Text=""cake"" Difficulty=""0""/><Word Text=""mine"" Difficulty=""0""/><Word Text=""drop"" Difficulty=""0""/><Word Text=""fast"" Difficulty=""0""/><Word Text=""pack"" Difficulty=""0""/><Word Text=""flat"" Difficulty=""0""/><Word Text=""wage"" Difficulty=""0""/><Word Text=""snap"" Difficulty=""0""/><Word Text=""gear"" Difficulty=""0""/><Word Text=""gang"" Difficulty=""0""/><Word Text=""wave"" Difficulty=""0""/><Word Text=""teen"" Difficulty=""0""/><Word Text=""yell"" Difficulty=""0""/><Word Text=""spin"" Difficulty=""0""/><Word Text=""bell"" Difficulty=""0""/><Word Text=""rank"" Difficulty=""0""/><Word Text=""beat"" Difficulty=""0""/><Word Text=""wind"" Difficulty=""0""/><Word Text=""lost"" Difficulty=""0""/><Word Text=""like"" Difficulty=""0""/><Word Text=""bear"" Difficulty=""0""/><Word Text=""pant"" Difficulty=""0""/><Word Text=""wipe"" Difficulty=""0""/><Word Text=""port"" Difficulty=""0""/><Word Text=""dirt"" Difficulty=""0""/><Word Text=""rice"" Difficulty=""0""/><Word Text=""flow"" Difficulty=""0""/><Word Text=""deck"" Difficulty=""0""/><Word Text=""pole"" Difficulty=""0""/><Word Text=""mode"" Difficulty=""0""/><Word Text=""bake"" Difficulty=""0""/><Word Text=""sink"" Difficulty=""0""/><Word Text=""swim"" Difficulty=""0""/><Word Text=""tire"" Difficulty=""0""/><Word Text=""free"" Difficulty=""0""/><Word Text=""hold"" Difficulty=""0""/><Word Text=""fade"" Difficulty=""0""/><Word Text=""spot"" Difficulty=""0""/><Word Text=""mask"" Difficulty=""0""/><Word Text=""easy"" Difficulty=""0""/><Word Text=""load"" Difficulty=""0""/><Word Text=""deer"" Difficulty=""0""/><Word Text=""fate"" Difficulty=""0""/><Word Text=""oven"" Difficulty=""0""/><Word Text=""poet"" Difficulty=""0""/><Word Text=""mere"" Difficulty=""0""/><Word Text=""pale"" Difficulty=""0""/><Word Text=""load"" Difficulty=""0""/><Word Text=""flee"" Difficulty=""0""/><Word Text=""lawn"" Difficulty=""0""/><Word Text=""plot"" Difficulty=""0""/><Word Text=""pipe"" Difficulty=""0""/><Word Text=""math"" Difficulty=""0""/><Word Text=""tail"" Difficulty=""0""/><Word Text=""palm"" Difficulty=""0""/><Word Text=""soup"" Difficulty=""0""/><Word Text=""pile"" Difficulty=""0""/><Word Text=""fund"" Difficulty=""0""/><Word Text=""aide"" Difficulty=""0""/><Word Text=""mall"" Difficulty=""0""/><Word Text=""heel"" Difficulty=""0""/><Word Text=""tent"" Difficulty=""0""/><Word Text=""myth"" Difficulty=""0""/><Word Text=""menu"" Difficulty=""0""/><Word Text=""rate"" Difficulty=""0""/><Word Text=""loud"" Difficulty=""0""/><Word Text=""auto"" Difficulty=""0""/><Word Text=""bite"" Difficulty=""0""/><Word Text=""pine"" Difficulty=""0""/><Word Text=""risk"" Difficulty=""0""/><Word Text=""chef"" Difficulty=""0""/><Word Text=""suit"" Difficulty=""0""/><Word Text=""boom"" Difficulty=""0""/><Word Text=""shit"" Difficulty=""0""/><Word Text=""cope"" Difficulty=""0""/><Word Text=""host"" Difficulty=""0""/><Word Text=""ease"" Difficulty=""0""/><Word Text=""wise"" Difficulty=""0""/><Word Text=""acid"" Difficulty=""0""/><Word Text=""odds"" Difficulty=""0""/><Word Text=""lung"" Difficulty=""0""/><Word Text=""firm"" Difficulty=""0""/><Word Text=""ugly"" Difficulty=""0""/><Word Text=""high"" Difficulty=""0""/><Word Text=""rope"" Difficulty=""0""/><Word Text=""sake"" Difficulty=""0""/><Word Text=""gaze"" Difficulty=""0""/><Word Text=""clue"" Difficulty=""0""/><Word Text=""dear"" Difficulty=""0""/><Word Text=""coal"" Difficulty=""0""/><Word Text=""sigh"" Difficulty=""0""/><Word Text=""dare"" Difficulty=""0""/><Word Text=""okay"" Difficulty=""0""/><Word Text=""cool"" Difficulty=""0""/><Word Text=""rose"" Difficulty=""0""/><Word Text=""rail"" Difficulty=""0""/><Word Text=""peer"" Difficulty=""0""/><Word Text=""mess"" Difficulty=""0""/><Word Text=""rank"" Difficulty=""0""/><Word Text=""norm"" Difficulty=""0""/><Word Text=""stem"" Difficulty=""0""/><Word Text=""rape"" Difficulty=""0""/><Word Text=""hunt"" Difficulty=""0""/><Word Text=""echo"" Difficulty=""0""/><Word Text=""pill"" Difficulty=""0""/><Word Text=""bare"" Difficulty=""0""/><Word Text=""rent"" Difficulty=""0""/><Word Text=""shop"" Difficulty=""0""/><Word Text=""pump"" Difficulty=""0""/><Word Text=""evil"" Difficulty=""0""/><Word Text=""slam"" Difficulty=""0""/><Word Text=""melt"" Difficulty=""0""/><Word Text=""park"" Difficulty=""0""/><Word Text=""cold"" Difficulty=""0""/><Word Text=""fold"" Difficulty=""0""/><Word Text=""beef"" Difficulty=""0""/><Word Text=""duck"" Difficulty=""0""/><Word Text=""dose"" Difficulty=""0""/><Word Text=""trap"" Difficulty=""0""/><Word Text=""lens"" Difficulty=""0""/><Word Text=""lend"" Difficulty=""0""/><Word Text=""nail"" Difficulty=""0""/><Word Text=""cave"" Difficulty=""0""/><Word Text=""herb"" Difficulty=""0""/><Word Text=""wish"" Difficulty=""0""/><Word Text=""warm"" Difficulty=""0""/><Word Text=""last"" Difficulty=""0""/><Word Text=""suck"" Difficulty=""0""/><Word Text=""leap"" Difficulty=""0""/><Word Text=""past"" Difficulty=""0""/><Word Text=""pond"" Difficulty=""0""/><Word Text=""dump"" Difficulty=""0""/><Word Text=""limb"" Difficulty=""0""/><Word Text=""tune"" Difficulty=""0""/><Word Text=""harm"" Difficulty=""0""/><Word Text=""horn"" Difficulty=""0""/><Word Text=""blue"" Difficulty=""0""/><Word Text=""grip"" Difficulty=""0""/><Word Text=""beam"" Difficulty=""0""/><Word Text=""rush"" Difficulty=""0""/><Word Text=""fork"" Difficulty=""0""/><Word Text=""disk"" Difficulty=""0""/><Word Text=""lock"" Difficulty=""0""/><Word Text=""blow"" Difficulty=""0""/><Word Text=""hook"" Difficulty=""0""/><Word Text=""exit"" Difficulty=""0""/><Word Text=""ship"" Difficulty=""0""/><Word Text=""mild"" Difficulty=""0""/><Word Text=""doll"" Difficulty=""0""/><Word Text=""noon"" Difficulty=""0""/><Word Text=""amid"" Difficulty=""0""/><Word Text=""loud"" Difficulty=""0""/><Word Text=""hers"" Difficulty=""0""/><Word Text=""jazz"" Difficulty=""0""/><Word Text=""bite"" Difficulty=""0""/><Word Text=""evil"" Difficulty=""0""/><Word Text=""oral"" Difficulty=""0""/><Word Text=""fist"" Difficulty=""0""/><Word Text=""bath"" Difficulty=""0""/><Word Text=""bold"" Difficulty=""0""/><Word Text=""tune"" Difficulty=""0""/><Word Text=""hint"" Difficulty=""0""/><Word Text=""peel"" Difficulty=""0""/><Word Text=""flip"" Difficulty=""0""/><Word Text=""bias"" Difficulty=""0""/><Word Text=""feel"" Difficulty=""0""/><Word Text=""lamp"" Difficulty=""0""/><Word Text=""chin"" Difficulty=""0""/><Word Text=""Arab"" Difficulty=""0""/><Word Text=""chop"" Difficulty=""0""/><Word Text=""pump"" Difficulty=""0""/><Word Text=""silk"" Difficulty=""0""/><Word Text=""kiss"" Difficulty=""0""/><Word Text=""rage"" Difficulty=""0""/><Word Text=""wake"" Difficulty=""0""/><Word Text=""dawn"" Difficulty=""0""/><Word Text=""hook"" Difficulty=""0""/><Word Text=""tide"" Difficulty=""0""/><Word Text=""cook"" Difficulty=""0""/><Word Text=""seal"" Difficulty=""0""/><Word Text=""sink"" Difficulty=""0""/><Word Text=""trap"" Difficulty=""0""/><Word Text=""scan"" Difficulty=""0""/><Word Text=""fool"" Difficulty=""0""/><Word Text=""rear"" Difficulty=""0""/><Word Text=""cart"" Difficulty=""0""/><Word Text=""stem"" Difficulty=""0""/><Word Text=""mate"" Difficulty=""0""/><Word Text=""slap"" Difficulty=""0""/><Word Text=""ours"" Difficulty=""0""/><Word Text=""heat"" Difficulty=""0""/><Word Text=""barn"" Difficulty=""0""/><Word Text=""tuck"" Difficulty=""0""/><Word Text=""drum"" Difficulty=""0""/><Word Text=""post"" Difficulty=""0""/><Word Text=""sail"" Difficulty=""0""/><Word Text=""nest"" Difficulty=""0""/><Word Text=""near"" Difficulty=""0""/><Word Text=""lane"" Difficulty=""0""/><Word Text=""cage"" Difficulty=""0""/><Word Text=""rack"" Difficulty=""0""/><Word Text=""wolf"" Difficulty=""0""/><Word Text=""grin"" Difficulty=""0""/><Word Text=""seal"" Difficulty=""0""/><Word Text=""aunt"" Difficulty=""0""/><Word Text=""rock"" Difficulty=""0""/><Word Text=""root"" Difficulty=""0""/><Word Text=""rent"" Difficulty=""0""/><Word Text=""calm"" Difficulty=""0""/><Word Text=""haul"" Difficulty=""0""/><Word Text=""ruin"" Difficulty=""0""/><Word Text=""bush"" Difficulty=""0""/><Word Text=""clip"" Difficulty=""0""/><Word Text=""bull"" Difficulty=""0""/><Word Text=""exam"" Difficulty=""0""/><Word Text=""star"" Difficulty=""0""/><Word Text=""ease"" Difficulty=""0""/><Word Text=""loop"" Difficulty=""0""/><Word Text=""edit"" Difficulty=""0""/><Word Text=""whip"" Difficulty=""0""/><Word Text=""boil"" Difficulty=""0""/><Word Text=""pork"" Difficulty=""0""/><Word Text=""sock"" Difficulty=""0""/><Word Text=""near"" Difficulty=""0""/><Word Text=""jump"" Difficulty=""0""/><Word Text=""sexy"" Difficulty=""0""/><Word Text=""seat"" Difficulty=""0""/><Word Text=""lion"" Difficulty=""0""/><Word Text=""cast"" Difficulty=""0""/><Word Text=""cord"" Difficulty=""0""/><Word Text=""harm"" Difficulty=""0""/><Word Text=""sort"" Difficulty=""0""/><Word Text=""soap"" Difficulty=""0""/><Word Text=""cute"" Difficulty=""0""/><Word Text=""shed"" Difficulty=""0""/><Word Text=""icon"" Difficulty=""0""/><Word Text=""heal"" Difficulty=""0""/><Word Text=""coin"" Difficulty=""0""/><Word Text=""stay"" Difficulty=""0""/><Word Text=""damn"" Difficulty=""0""/><Word Text=""case"" Difficulty=""0""/><Word Text=""gaze"" Difficulty=""0""/><Word Text=""mill"" Difficulty=""0""/><Word Text=""hike"" Difficulty=""0""/><Word Text=""sack"" Difficulty=""0""/><Word Text=""tray"" Difficulty=""0""/><Word Text=""coup"" Difficulty=""0""/><Word Text=""skip"" Difficulty=""0""/><Word Text=""sole"" Difficulty=""0""/><Word Text=""joke"" Difficulty=""0""/><Word Text=""weed"" Difficulty=""0""/><Word Text=""deem"" Difficulty=""0""/><Word Text=""pile"" Difficulty=""0""/><Word Text=""cure"" Difficulty=""0""/><Word Text=""cure"" Difficulty=""0""/><Word Text=""fame"" Difficulty=""0""/><Word Text=""atop"" Difficulty=""0""/><Word Text=""toll"" Difficulty=""0""/><Word Text=""this"" Difficulty=""0""/><Word Text=""grin"" Difficulty=""0""/><Word Text=""rain"" Difficulty=""0""/><Word Text=""chew"" Difficulty=""0""/><Word Text=""butt"" Difficulty=""0""/><Word Text=""dumb"" Difficulty=""0""/><Word Text=""bulk"" Difficulty=""0""/><Word Text=""goat"" Difficulty=""0""/><Word Text=""neat"" Difficulty=""0""/><Word Text=""part"" Difficulty=""0""/><Word Text=""poke"" Difficulty=""0""/><Word Text=""soar"" Difficulty=""0""/><Word Text=""calm"" Difficulty=""0""/><Word Text=""clay"" Difficulty=""0""/><Word Text=""fare"" Difficulty=""0""/><Word Text=""disc"" Difficulty=""0""/><Word Text=""sofa"" Difficulty=""0""/><Word Text=""bomb"" Difficulty=""0""/><Word Text=""fish"" Difficulty=""0""/><Word Text=""soak"" Difficulty=""0""/><Word Text=""slot"" Difficulty=""0""/><Word Text=""riot"" Difficulty=""0""/><Word Text=""tile"" Difficulty=""0""/><Word Text=""till"" Difficulty=""0""/><Word Text=""plea"" Difficulty=""0""/><Word Text=""bulb"" Difficulty=""0""/><Word Text=""copy"" Difficulty=""0""/><Word Text=""bolt"" Difficulty=""0""/><Word Text=""dock"" Difficulty=""0""/><Word Text=""trim"" Difficulty=""0""/><Word Text=""spit"" Difficulty=""0""/><Word Text=""till"" Difficulty=""0""/><Word Text=""their"" Difficulty=""0""/><Word Text=""would"" Difficulty=""0""/><Word Text=""about"" Difficulty=""0""/><Word Text=""there"" Difficulty=""0""/><Word Text=""think"" Difficulty=""0""/><Word Text=""which"" Difficulty=""0""/><Word Text=""could"" Difficulty=""0""/><Word Text=""other"" Difficulty=""0""/><Word Text=""these"" Difficulty=""0""/><Word Text=""first"" Difficulty=""0""/><Word Text=""thing"" Difficulty=""0""/><Word Text=""those"" Difficulty=""0""/><Word Text=""woman"" Difficulty=""0""/><Word Text=""child"" Difficulty=""0""/><Word Text=""there"" Difficulty=""0""/><Word Text=""after"" Difficulty=""0""/><Word Text=""world"" Difficulty=""0""/><Word Text=""still"" Difficulty=""0""/><Word Text=""three"" Difficulty=""0""/><Word Text=""state"" Difficulty=""0""/><Word Text=""never"" Difficulty=""0""/><Word Text=""leave"" Difficulty=""0""/><Word Text=""while"" Difficulty=""0""/><Word Text=""great"" Difficulty=""0""/><Word Text=""group"" Difficulty=""0""/><Word Text=""begin"" Difficulty=""0""/><Word Text=""where"" Difficulty=""0""/><Word Text=""every"" Difficulty=""0""/><Word Text=""start"" Difficulty=""0""/><Word Text=""might"" Difficulty=""0""/><Word Text=""about"" Difficulty=""0""/><Word Text=""place"" Difficulty=""0""/><Word Text=""again"" Difficulty=""0""/><Word Text=""where"" Difficulty=""0""/><Word Text=""right"" Difficulty=""0""/><Word Text=""small"" Difficulty=""0""/><Word Text=""night"" Difficulty=""0""/><Word Text=""point"" Difficulty=""0""/><Word Text=""today"" Difficulty=""0""/><Word Text=""bring"" Difficulty=""0""/><Word Text=""large"" Difficulty=""0""/><Word Text=""under"" Difficulty=""0""/><Word Text=""water"" Difficulty=""0""/><Word Text=""write"" Difficulty=""0""/><Word Text=""money"" Difficulty=""0""/><Word Text=""story"" Difficulty=""0""/><Word Text=""young"" Difficulty=""0""/><Word Text=""month"" Difficulty=""0""/><Word Text=""right"" Difficulty=""0""/><Word Text=""study"" Difficulty=""0""/><Word Text=""issue"" Difficulty=""0""/><Word Text=""black"" Difficulty=""0""/><Word Text=""house"" Difficulty=""0""/><Word Text=""after"" Difficulty=""0""/><Word Text=""since"" Difficulty=""0""/><Word Text=""until"" Difficulty=""0""/><Word Text=""power"" Difficulty=""0""/><Word Text=""often"" Difficulty=""0""/><Word Text=""among"" Difficulty=""0""/><Word Text=""stand"" Difficulty=""0""/><Word Text=""later"" Difficulty=""0""/><Word Text=""white"" Difficulty=""0""/><Word Text=""least"" Difficulty=""0""/><Word Text=""learn"" Difficulty=""0""/><Word Text=""right"" Difficulty=""0""/><Word Text=""watch"" Difficulty=""0""/><Word Text=""speak"" Difficulty=""0""/><Word Text=""level"" Difficulty=""0""/><Word Text=""allow"" Difficulty=""0""/><Word Text=""spend"" Difficulty=""0""/><Word Text=""party"" Difficulty=""0""/><Word Text=""early"" Difficulty=""0""/><Word Text=""force"" Difficulty=""0""/><Word Text=""offer"" Difficulty=""0""/><Word Text=""maybe"" Difficulty=""0""/><Word Text=""music"" Difficulty=""0""/><Word Text=""human"" Difficulty=""0""/><Word Text=""serve"" Difficulty=""1""/><Word Text=""sense"" Difficulty=""1""/><Word Text=""build"" Difficulty=""1""/><Word Text=""death"" Difficulty=""1""/><Word Text=""reach"" Difficulty=""1""/><Word Text=""local"" Difficulty=""1""/><Word Text=""class"" Difficulty=""1""/><Word Text=""raise"" Difficulty=""1""/><Word Text=""field"" Difficulty=""1""/><Word Text=""major"" Difficulty=""1""/><Word Text=""along"" Difficulty=""1""/><Word Text=""heart"" Difficulty=""1""/><Word Text=""light"" Difficulty=""1""/><Word Text=""voice"" Difficulty=""1""/><Word Text=""whole"" Difficulty=""1""/><Word Text=""price"" Difficulty=""1""/><Word Text=""carry"" Difficulty=""1""/><Word Text=""drive"" Difficulty=""1""/><Word Text=""break"" Difficulty=""1""/><Word Text=""thank"" Difficulty=""1""/><Word Text=""value"" Difficulty=""1""/><Word Text=""model"" Difficulty=""1""/><Word Text=""early"" Difficulty=""1""/><Word Text=""agree"" Difficulty=""1""/><Word Text=""paper"" Difficulty=""1""/><Word Text=""space"" Difficulty=""1""/><Word Text=""event"" Difficulty=""1""/><Word Text=""whose"" Difficulty=""1""/><Word Text=""table"" Difficulty=""1""/><Word Text=""court"" Difficulty=""1""/><Word Text=""teach"" Difficulty=""1""/><Word Text=""image"" Difficulty=""1""/><Word Text=""phone"" Difficulty=""1""/><Word Text=""cover"" Difficulty=""1""/><Word Text=""quite"" Difficulty=""1""/><Word Text=""clear"" Difficulty=""1""/><Word Text=""piece"" Difficulty=""1""/><Word Text=""movie"" Difficulty=""1""/><Word Text=""north"" Difficulty=""1""/><Word Text=""third"" Difficulty=""1""/><Word Text=""catch"" Difficulty=""1""/><Word Text=""cause"" Difficulty=""1""/><Word Text=""point"" Difficulty=""1""/><Word Text=""plant"" Difficulty=""1""/><Word Text=""short"" Difficulty=""1""/><Word Text=""place"" Difficulty=""1""/><Word Text=""south"" Difficulty=""1""/><Word Text=""floor"" Difficulty=""1""/><Word Text=""close"" Difficulty=""1""/><Word Text=""wrong"" Difficulty=""1""/><Word Text=""sport"" Difficulty=""1""/><Word Text=""board"" Difficulty=""1""/><Word Text=""fight"" Difficulty=""1""/><Word Text=""throw"" Difficulty=""1""/><Word Text=""order"" Difficulty=""1""/><Word Text=""focus"" Difficulty=""1""/><Word Text=""blood"" Difficulty=""1""/><Word Text=""color"" Difficulty=""1""/><Word Text=""store"" Difficulty=""1""/><Word Text=""sound"" Difficulty=""1""/><Word Text=""enter"" Difficulty=""1""/><Word Text=""share"" Difficulty=""1""/><Word Text=""other"" Difficulty=""1""/><Word Text=""shoot"" Difficulty=""1""/><Word Text=""seven"" Difficulty=""1""/><Word Text=""scene"" Difficulty=""1""/><Word Text=""stock"" Difficulty=""1""/><Word Text=""eight"" Difficulty=""1""/><Word Text=""happy"" Difficulty=""1""/><Word Text=""occur"" Difficulty=""1""/><Word Text=""media"" Difficulty=""1""/><Word Text=""ready"" Difficulty=""1""/><Word Text=""argue"" Difficulty=""1""/><Word Text=""staff"" Difficulty=""1""/><Word Text=""trade"" Difficulty=""1""/><Word Text=""glass"" Difficulty=""1""/><Word Text=""skill"" Difficulty=""1""/><Word Text=""crime"" Difficulty=""1""/><Word Text=""stage"" Difficulty=""1""/><Word Text=""state"" Difficulty=""1""/><Word Text=""force"" Difficulty=""1""/><Word Text=""truth"" Difficulty=""1""/><Word Text=""check"" Difficulty=""1""/><Word Text=""laugh"" Difficulty=""1""/><Word Text=""guess"" Difficulty=""1""/><Word Text=""study"" Difficulty=""1""/><Word Text=""prove"" Difficulty=""1""/><Word Text=""since"" Difficulty=""1""/><Word Text=""claim"" Difficulty=""1""/><Word Text=""close"" Difficulty=""1""/><Word Text=""sound"" Difficulty=""1""/><Word Text=""enjoy"" Difficulty=""1""/><Word Text=""legal"" Difficulty=""1""/><Word Text=""final"" Difficulty=""1""/><Word Text=""green"" Difficulty=""1""/><Word Text=""above"" Difficulty=""1""/><Word Text=""trial"" Difficulty=""1""/><Word Text=""radio"" Difficulty=""1""/><Word Text=""visit"" Difficulty=""1""/><Word Text=""avoid"" Difficulty=""1""/><Word Text=""close"" Difficulty=""1""/><Word Text=""peace"" Difficulty=""1""/><Word Text=""apply"" Difficulty=""1""/><Word Text=""shake"" Difficulty=""1""/><Word Text=""chair"" Difficulty=""1""/><Word Text=""treat"" Difficulty=""1""/><Word Text=""style"" Difficulty=""1""/><Word Text=""adult"" Difficulty=""1""/><Word Text=""worry"" Difficulty=""1""/><Word Text=""range"" Difficulty=""1""/><Word Text=""dream"" Difficulty=""1""/><Word Text=""stuff"" Difficulty=""1""/><Word Text=""hotel"" Difficulty=""1""/><Word Text=""heavy"" Difficulty=""1""/><Word Text=""cause"" Difficulty=""1""/><Word Text=""tough"" Difficulty=""1""/><Word Text=""exist"" Difficulty=""1""/><Word Text=""agent"" Difficulty=""1""/><Word Text=""owner"" Difficulty=""1""/><Word Text=""ahead"" Difficulty=""1""/><Word Text=""coach"" Difficulty=""1""/><Word Text=""total"" Difficulty=""1""/><Word Text=""civil"" Difficulty=""1""/><Word Text=""mouth"" Difficulty=""1""/><Word Text=""smile"" Difficulty=""1""/><Word Text=""score"" Difficulty=""1""/><Word Text=""break"" Difficulty=""1""/><Word Text=""front"" Difficulty=""1""/><Word Text=""admit"" Difficulty=""1""/><Word Text=""alone"" Difficulty=""1""/><Word Text=""fresh"" Difficulty=""1""/><Word Text=""video"" Difficulty=""1""/><Word Text=""judge"" Difficulty=""1""/><Word Text=""stare"" Difficulty=""1""/><Word Text=""troop"" Difficulty=""1""/><Word Text=""track"" Difficulty=""1""/><Word Text=""basic"" Difficulty=""1""/><Word Text=""plane"" Difficulty=""1""/><Word Text=""labor"" Difficulty=""1""/><Word Text=""refer"" Difficulty=""1""/><Word Text=""touch"" Difficulty=""1""/><Word Text=""sleep"" Difficulty=""1""/><Word Text=""press"" Difficulty=""1""/><Word Text=""brain"" Difficulty=""1""/><Word Text=""dozen"" Difficulty=""1""/><Word Text=""along"" Difficulty=""1""/><Word Text=""sorry"" Difficulty=""1""/><Word Text=""stick"" Difficulty=""1""/><Word Text=""stone"" Difficulty=""1""/><Word Text=""scale"" Difficulty=""1""/><Word Text=""drink"" Difficulty=""1""/><Word Text=""front"" Difficulty=""1""/><Word Text=""truck"" Difficulty=""1""/><Word Text=""sales"" Difficulty=""1""/><Word Text=""shape"" Difficulty=""1""/><Word Text=""crowd"" Difficulty=""1""/><Word Text=""horse"" Difficulty=""1""/><Word Text=""guard"" Difficulty=""1""/><Word Text=""terms"" Difficulty=""1""/><Word Text=""share"" Difficulty=""1""/><Word Text=""quick"" Difficulty=""1""/><Word Text=""light"" Difficulty=""1""/><Word Text=""pound"" Difficulty=""1""/><Word Text=""basis"" Difficulty=""1""/><Word Text=""guest"" Difficulty=""1""/><Word Text=""block"" Difficulty=""1""/><Word Text=""while"" Difficulty=""1""/><Word Text=""title"" Difficulty=""1""/><Word Text=""faith"" Difficulty=""1""/><Word Text=""river"" Difficulty=""1""/><Word Text=""count"" Difficulty=""1""/><Word Text=""marry"" Difficulty=""1""/><Word Text=""order"" Difficulty=""1""/><Word Text=""limit"" Difficulty=""1""/><Word Text=""claim"" Difficulty=""1""/><Word Text=""worth"" Difficulty=""1""/><Word Text=""until"" Difficulty=""1""/><Word Text=""speed"" Difficulty=""1""/><Word Text=""cross"" Difficulty=""1""/><Word Text=""youth"" Difficulty=""1""/><Word Text=""broad"" Difficulty=""1""/><Word Text=""twice"" Difficulty=""1""/><Word Text=""grade"" Difficulty=""1""/><Word Text=""focus"" Difficulty=""1""/><Word Text=""smile"" Difficulty=""1""/><Word Text=""quiet"" Difficulty=""1""/><Word Text=""dress"" Difficulty=""1""/><Word Text=""aware"" Difficulty=""1""/><Word Text=""drive"" Difficulty=""1""/><Word Text=""chief"" Difficulty=""1""/><Word Text=""below"" Difficulty=""1""/><Word Text=""voter"" Difficulty=""1""/><Word Text=""moral"" Difficulty=""1""/><Word Text=""visit"" Difficulty=""1""/><Word Text=""photo"" Difficulty=""1""/><Word Text=""daily"" Difficulty=""1""/><Word Text=""fully"" Difficulty=""1""/><Word Text=""actor"" Difficulty=""1""/><Word Text=""birth"" Difficulty=""1""/><Word Text=""front"" Difficulty=""1""/><Word Text=""clean"" Difficulty=""1""/><Word Text=""train"" Difficulty=""1""/><Word Text=""plate"" Difficulty=""1""/><Word Text=""press"" Difficulty=""1""/><Word Text=""start"" Difficulty=""1""/><Word Text=""alive"" Difficulty=""1""/><Word Text=""abuse"" Difficulty=""1""/><Word Text=""extra"" Difficulty=""1""/><Word Text=""paint"" Difficulty=""1""/><Word Text=""fight"" Difficulty=""1""/><Word Text=""climb"" Difficulty=""1""/><Word Text=""sweet"" Difficulty=""1""/><Word Text=""metal"" Difficulty=""1""/><Word Text=""urban"" Difficulty=""1""/><Word Text=""lunch"" Difficulty=""1""/><Word Text=""above"" Difficulty=""1""/><Word Text=""sugar"" Difficulty=""1""/><Word Text=""enemy"" Difficulty=""1""/><Word Text=""panel"" Difficulty=""1""/><Word Text=""alone"" Difficulty=""1""/><Word Text=""sight"" Difficulty=""1""/><Word Text=""cover"" Difficulty=""1""/><Word Text=""adopt"" Difficulty=""1""/><Word Text=""works"" Difficulty=""1""/><Word Text=""empty"" Difficulty=""1""/><Word Text=""trail"" Difficulty=""1""/><Word Text=""novel"" Difficulty=""1""/><Word Text=""Iraqi"" Difficulty=""1""/><Word Text=""human"" Difficulty=""1""/><Word Text=""theme"" Difficulty=""1""/><Word Text=""storm"" Difficulty=""1""/><Word Text=""union"" Difficulty=""1""/><Word Text=""fruit"" Difficulty=""1""/><Word Text=""under"" Difficulty=""1""/><Word Text=""prime"" Difficulty=""1""/><Word Text=""dance"" Difficulty=""1""/><Word Text=""limit"" Difficulty=""1""/><Word Text=""being"" Difficulty=""1""/><Word Text=""shift"" Difficulty=""1""/><Word Text=""train"" Difficulty=""1""/><Word Text=""trend"" Difficulty=""1""/><Word Text=""angry"" Difficulty=""1""/><Word Text=""truly"" Difficulty=""1""/><Word Text=""earth"" Difficulty=""1""/><Word Text=""chest"" Difficulty=""1""/><Word Text=""thick"" Difficulty=""1""/><Word Text=""dress"" Difficulty=""1""/><Word Text=""judge"" Difficulty=""1""/><Word Text=""sheet"" Difficulty=""1""/><Word Text=""ought"" Difficulty=""1""/><Word Text=""chief"" Difficulty=""1""/><Word Text=""brown"" Difficulty=""1""/><Word Text=""shirt"" Difficulty=""1""/><Word Text=""pilot"" Difficulty=""1""/><Word Text=""guide"" Difficulty=""1""/><Word Text=""steal"" Difficulty=""1""/><Word Text=""funny"" Difficulty=""1""/><Word Text=""blame"" Difficulty=""1""/><Word Text=""crazy"" Difficulty=""1""/><Word Text=""chain"" Difficulty=""1""/><Word Text=""solve"" Difficulty=""1""/><Word Text=""equal"" Difficulty=""1""/><Word Text=""forth"" Difficulty=""1""/><Word Text=""frame"" Difficulty=""1""/><Word Text=""trust"" Difficulty=""1""/><Word Text=""ocean"" Difficulty=""1""/><Word Text=""score"" Difficulty=""1""/><Word Text=""tooth"" Difficulty=""1""/><Word Text=""smart"" Difficulty=""1""/><Word Text=""topic"" Difficulty=""1""/><Word Text=""issue"" Difficulty=""1""/><Word Text=""range"" Difficulty=""1""/><Word Text=""nurse"" Difficulty=""1""/><Word Text=""aside"" Difficulty=""1""/><Word Text=""check"" Difficulty=""1""/><Word Text=""stand"" Difficulty=""1""/><Word Text=""clear"" Difficulty=""1""/><Word Text=""clean"" Difficulty=""1""/><Word Text=""doubt"" Difficulty=""1""/><Word Text=""grant"" Difficulty=""1""/><Word Text=""cloud"" Difficulty=""1""/><Word Text=""below"" Difficulty=""1""/><Word Text=""cheap"" Difficulty=""1""/><Word Text=""beach"" Difficulty=""1""/><Word Text=""route"" Difficulty=""1""/><Word Text=""upper"" Difficulty=""1""/><Word Text=""tired"" Difficulty=""1""/><Word Text=""dance"" Difficulty=""1""/><Word Text=""fewer"" Difficulty=""1""/><Word Text=""apart"" Difficulty=""1""/><Word Text=""match"" Difficulty=""1""/><Word Text=""black"" Difficulty=""1""/><Word Text=""proud"" Difficulty=""1""/><Word Text=""waste"" Difficulty=""1""/><Word Text=""wheel"" Difficulty=""1""/><Word Text=""cable"" Difficulty=""1""/><Word Text=""rural"" Difficulty=""1""/><Word Text=""cream"" Difficulty=""1""/><Word Text=""solid"" Difficulty=""1""/><Word Text=""noise"" Difficulty=""1""/><Word Text=""grass"" Difficulty=""1""/><Word Text=""drink"" Difficulty=""1""/><Word Text=""taste"" Difficulty=""1""/><Word Text=""sleep"" Difficulty=""1""/><Word Text=""first"" Difficulty=""1""/><Word Text=""sharp"" Difficulty=""1""/><Word Text=""lower"" Difficulty=""1""/><Word Text=""honor"" Difficulty=""1""/><Word Text=""knock"" Difficulty=""1""/><Word Text=""offer"" Difficulty=""1""/><Word Text=""asset"" Difficulty=""1""/><Word Text=""bread"" Difficulty=""1""/><Word Text=""green"" Difficulty=""1""/><Word Text=""lucky"" Difficulty=""1""/><Word Text=""brief"" Difficulty=""1""/><Word Text=""steel"" Difficulty=""1""/><Word Text=""shout"" Difficulty=""1""/><Word Text=""layer"" Difficulty=""1""/><Word Text=""later"" Difficulty=""1""/><Word Text=""slide"" Difficulty=""1""/><Word Text=""shall"" Difficulty=""1""/><Word Text=""error"" Difficulty=""1""/><Word Text=""print"" Difficulty=""1""/><Word Text=""album"" Difficulty=""1""/><Word Text=""joint"" Difficulty=""1""/><Word Text=""reply"" Difficulty=""1""/><Word Text=""cycle"" Difficulty=""1""/><Word Text=""whole"" Difficulty=""1""/><Word Text=""trust"" Difficulty=""1""/><Word Text=""grand"" Difficulty=""1""/><Word Text=""hello"" Difficulty=""1""/><Word Text=""knife"" Difficulty=""1""/><Word Text=""phase"" Difficulty=""1""/><Word Text=""quote"" Difficulty=""1""/><Word Text=""elect"" Difficulty=""1""/><Word Text=""shift"" Difficulty=""1""/><Word Text=""touch"" Difficulty=""1""/><Word Text=""sauce"" Difficulty=""1""/><Word Text=""shock"" Difficulty=""1""/><Word Text=""habit"" Difficulty=""1""/><Word Text=""juice"" Difficulty=""1""/><Word Text=""coach"" Difficulty=""1""/><Word Text=""other"" Difficulty=""1""/><Word Text=""entry"" Difficulty=""1""/><Word Text=""still"" Difficulty=""1""/><Word Text=""trade"" Difficulty=""1""/><Word Text=""maker"" Difficulty=""1""/><Word Text=""Asian"" Difficulty=""1""/><Word Text=""total"" Difficulty=""1""/><Word Text=""usual"" Difficulty=""1""/><Word Text=""anger"" Difficulty=""1""/><Word Text=""round"" Difficulty=""1""/><Word Text=""smell"" Difficulty=""1""/><Word Text=""light"" Difficulty=""1""/><Word Text=""block"" Difficulty=""1""/><Word Text=""tower"" Difficulty=""1""/><Word Text=""smoke"" Difficulty=""1""/><Word Text=""shape"" Difficulty=""1""/><Word Text=""coast"" Difficulty=""1""/><Word Text=""watch"" Difficulty=""1""/><Word Text=""inner"" Difficulty=""1""/><Word Text=""swing"" Difficulty=""1""/><Word Text=""plant"" Difficulty=""1""/><Word Text=""mayor"" Difficulty=""1""/><Word Text=""smoke"" Difficulty=""1""/><Word Text=""fifth"" Difficulty=""1""/><Word Text=""favor"" Difficulty=""1""/><Word Text=""weigh"" Difficulty=""1""/><Word Text=""false"" Difficulty=""1""/><Word Text=""Latin"" Difficulty=""1""/><Word Text=""essay"" Difficulty=""1""/><Word Text=""giant"" Difficulty=""1""/><Word Text=""count"" Difficulty=""1""/><Word Text=""depth"" Difficulty=""1""/><Word Text=""shell"" Difficulty=""1""/><Word Text=""onion"" Difficulty=""1""/><Word Text=""brand"" Difficulty=""1""/><Word Text=""award"" Difficulty=""1""/><Word Text=""arise"" Difficulty=""1""/><Word Text=""armed"" Difficulty=""1""/><Word Text=""stake"" Difficulty=""1""/><Word Text=""dream"" Difficulty=""1""/><Word Text=""fiber"" Difficulty=""1""/><Word Text=""minor"" Difficulty=""1""/><Word Text=""label"" Difficulty=""1""/><Word Text=""index"" Difficulty=""1""/><Word Text=""draft"" Difficulty=""1""/><Word Text=""rough"" Difficulty=""1""/><Word Text=""drama"" Difficulty=""1""/><Word Text=""clock"" Difficulty=""1""/><Word Text=""sweep"" Difficulty=""1""/><Word Text=""house"" Difficulty=""1""/><Word Text=""ahead"" Difficulty=""1""/><Word Text=""super"" Difficulty=""1""/><Word Text=""yield"" Difficulty=""1""/><Word Text=""fence"" Difficulty=""1""/><Word Text=""paint"" Difficulty=""1""/><Word Text=""bunch"" Difficulty=""1""/><Word Text=""found"" Difficulty=""1""/><Word Text=""react"" Difficulty=""1""/><Word Text=""taste"" Difficulty=""1""/><Word Text=""cheek"" Difficulty=""1""/><Word Text=""match"" Difficulty=""1""/><Word Text=""apple"" Difficulty=""1""/><Word Text=""track"" Difficulty=""1""/><Word Text=""virus"" Difficulty=""1""/><Word Text=""blind"" Difficulty=""1""/><Word Text=""white"" Difficulty=""1""/><Word Text=""honor"" Difficulty=""1""/><Word Text=""slave"" Difficulty=""1""/><Word Text=""elite"" Difficulty=""1""/><Word Text=""tight"" Difficulty=""1""/><Word Text=""Bible"" Difficulty=""1""/><Word Text=""chart"" Difficulty=""1""/><Word Text=""solar"" Difficulty=""1""/><Word Text=""stick"" Difficulty=""1""/><Word Text=""strip"" Difficulty=""1""/><Word Text=""salad"" Difficulty=""1""/><Word Text=""pause"" Difficulty=""1""/><Word Text=""bench"" Difficulty=""1""/><Word Text=""lover"" Difficulty=""1""/><Word Text=""newly"" Difficulty=""1""/><Word Text=""imply"" Difficulty=""1""/><Word Text=""pride"" Difficulty=""1""/><Word Text=""ideal"" Difficulty=""1""/><Word Text=""worth"" Difficulty=""1""/><Word Text=""smell"" Difficulty=""1""/><Word Text=""crash"" Difficulty=""1""/><Word Text=""craft"" Difficulty=""1""/><Word Text=""fault"" Difficulty=""1""/><Word Text=""loose"" Difficulty=""1""/><Word Text=""prior"" Difficulty=""1""/><Word Text=""relax"" Difficulty=""1""/><Word Text=""stair"" Difficulty=""1""/><Word Text=""proof"" Difficulty=""1""/><Word Text=""dirty"" Difficulty=""1""/><Word Text=""alter"" Difficulty=""1""/><Word Text=""split"" Difficulty=""1""/><Word Text=""vital"" Difficulty=""1""/><Word Text=""adapt"" Difficulty=""1""/><Word Text=""Irish"" Difficulty=""1""/><Word Text=""honey"" Difficulty=""1""/><Word Text=""round"" Difficulty=""1""/><Word Text=""tribe"" Difficulty=""1""/><Word Text=""shelf"" Difficulty=""1""/><Word Text=""buyer"" Difficulty=""1""/><Word Text=""doubt"" Difficulty=""1""/><Word Text=""guide"" Difficulty=""1""/><Word Text=""since"" Difficulty=""1""/><Word Text=""shade"" Difficulty=""1""/><Word Text=""mount"" Difficulty=""1""/><Word Text=""angle"" Difficulty=""1""/><Word Text=""store"" Difficulty=""1""/><Word Text=""crack"" Difficulty=""1""/><Word Text=""given"" Difficulty=""1""/><Word Text=""trace"" Difficulty=""1""/><Word Text=""meter"" Difficulty=""1""/><Word Text=""rapid"" Difficulty=""1""/><Word Text=""fifty"" Difficulty=""1""/><Word Text=""porch"" Difficulty=""1""/><Word Text=""waste"" Difficulty=""1""/><Word Text=""rifle"" Difficulty=""1""/><Word Text=""trick"" Difficulty=""1""/><Word Text=""nerve"" Difficulty=""1""/><Word Text=""ratio"" Difficulty=""1""/><Word Text=""humor"" Difficulty=""1""/><Word Text=""glove"" Difficulty=""1""/><Word Text=""delay"" Difficulty=""1""/><Word Text=""scope"" Difficulty=""1""/><Word Text=""badly"" Difficulty=""1""/><Word Text=""eager"" Difficulty=""1""/><Word Text=""motor"" Difficulty=""1""/><Word Text=""float"" Difficulty=""1""/><Word Text=""blade"" Difficulty=""1""/><Word Text=""print"" Difficulty=""1""/><Word Text=""cabin"" Difficulty=""1""/><Word Text=""yours"" Difficulty=""1""/><Word Text=""pitch"" Difficulty=""1""/><Word Text=""lemon"" Difficulty=""1""/><Word Text=""sense"" Difficulty=""1""/><Word Text=""naked"" Difficulty=""1""/><Word Text=""shrug"" Difficulty=""1""/><Word Text=""flame"" Difficulty=""1""/><Word Text=""wound"" Difficulty=""1""/><Word Text=""flesh"" Difficulty=""1""/><Word Text=""grain"" Difficulty=""1""/><Word Text=""brush"" Difficulty=""1""/><Word Text=""crack"" Difficulty=""1""/><Word Text=""seize"" Difficulty=""1""/><Word Text=""grant"" Difficulty=""1""/><Word Text=""shore"" Difficulty=""1""/><Word Text=""ghost"" Difficulty=""1""/><Word Text=""swing"" Difficulty=""1""/><Word Text=""awful"" Difficulty=""1""/><Word Text=""crash"" Difficulty=""1""/><Word Text=""piano"" Difficulty=""1""/><Word Text=""mouse"" Difficulty=""1""/><Word Text=""chase"" Difficulty=""1""/><Word Text=""brick"" Difficulty=""1""/><Word Text=""patch"" Difficulty=""1""/><Word Text=""swear"" Difficulty=""1""/><Word Text=""slice"" Difficulty=""1""/><Word Text=""exact"" Difficulty=""1""/><Word Text=""uncle"" Difficulty=""1""/><Word Text=""grave"" Difficulty=""1""/><Word Text=""couch"" Difficulty=""1""/><Word Text=""shine"" Difficulty=""1""/><Word Text=""upset"" Difficulty=""1""/><Word Text=""organ"" Difficulty=""1""/><Word Text=""tight"" Difficulty=""1""/><Word Text=""favor"" Difficulty=""1""/><Word Text=""magic"" Difficulty=""1""/><Word Text=""brush"" Difficulty=""1""/><Word Text=""jeans"" Difficulty=""1""/><Word Text=""flour"" Difficulty=""1""/><Word Text=""slope"" Difficulty=""1""/><Word Text=""delay"" Difficulty=""1""/><Word Text=""candy"" Difficulty=""1""/><Word Text=""final"" Difficulty=""1""/><Word Text=""medal"" Difficulty=""1""/><Word Text=""curve"" Difficulty=""1""/><Word Text=""logic"" Difficulty=""1""/><Word Text=""harsh"" Difficulty=""1""/><Word Text=""greet"" Difficulty=""1""/><Word Text=""favor"" Difficulty=""1""/><Word Text=""march"" Difficulty=""1""/><Word Text=""snake"" Difficulty=""1""/><Word Text=""pitch"" Difficulty=""1""/><Word Text=""cross"" Difficulty=""1""/><Word Text=""daily"" Difficulty=""1""/><Word Text=""flash"" Difficulty=""1""/><Word Text=""Islam"" Difficulty=""1""/><Word Text=""Roman"" Difficulty=""1""/><Word Text=""elbow"" Difficulty=""1""/><Word Text=""plead"" Difficulty=""1""/><Word Text=""sixth"" Difficulty=""1""/><Word Text=""trunk"" Difficulty=""1""/><Word Text=""rumor"" Difficulty=""1""/><Word Text=""cloth"" Difficulty=""1""/><Word Text=""reach"" Difficulty=""1""/><Word Text=""plain"" Difficulty=""1""/><Word Text=""fraud"" Difficulty=""1""/><Word Text=""array"" Difficulty=""1""/><Word Text=""burst"" Difficulty=""1""/><Word Text=""speed"" Difficulty=""1""/><Word Text=""label"" Difficulty=""1""/><Word Text=""flood"" Difficulty=""1""/><Word Text=""arena"" Difficulty=""1""/><Word Text=""laugh"" Difficulty=""1""/><Word Text=""drift"" Difficulty=""1""/><Word Text=""drain"" Difficulty=""1""/><Word Text=""hurry"" Difficulty=""1""/><Word Text=""wrist"" Difficulty=""1""/><Word Text=""guilt"" Difficulty=""1""/><Word Text=""skirt"" Difficulty=""1""/><Word Text=""hence"" Difficulty=""1""/><Word Text=""guard"" Difficulty=""1""/><Word Text=""await"" Difficulty=""1""/><Word Text=""spill"" Difficulty=""1""/><Word Text=""grace"" Difficulty=""1""/><Word Text=""slide"" Difficulty=""1""/><Word Text=""towel"" Difficulty=""1""/><Word Text=""award"" Difficulty=""1""/><Word Text=""prize"" Difficulty=""1""/><Word Text=""boost"" Difficulty=""1""/><Word Text=""alarm"" Difficulty=""1""/><Word Text=""weird"" Difficulty=""1""/><Word Text=""sweat"" Difficulty=""1""/><Word Text=""outer"" Difficulty=""1""/><Word Text=""drunk"" Difficulty=""1""/><Word Text=""stuff"" Difficulty=""1""/><Word Text=""pause"" Difficulty=""1""/><Word Text=""chaos"" Difficulty=""1""/><Word Text=""short"" Difficulty=""1""/><Word Text=""forty"" Difficulty=""1""/><Word Text=""lobby"" Difficulty=""1""/><Word Text=""trait"" Difficulty=""1""/><Word Text=""abuse"" Difficulty=""1""/><Word Text=""thumb"" Difficulty=""1""/><Word Text=""unity"" Difficulty=""1""/><Word Text=""twist"" Difficulty=""1""/><Word Text=""shame"" Difficulty=""1""/><Word Text=""rebel"" Difficulty=""1""/><Word Text=""fluid"" Difficulty=""1""/><Word Text=""click"" Difficulty=""1""/><Word Text=""carve"" Difficulty=""1""/><Word Text=""belly"" Difficulty=""1""/><Word Text=""scare"" Difficulty=""1""/><Word Text=""ankle"" Difficulty=""1""/><Word Text=""rider"" Difficulty=""1""/><Word Text=""crawl"" Difficulty=""1""/><Word Text=""magic"" Difficulty=""1""/><Word Text=""donor"" Difficulty=""1""/><Word Text=""opera"" Difficulty=""1""/><Word Text=""frame"" Difficulty=""1""/><Word Text=""giant"" Difficulty=""1""/><Word Text=""wrong"" Difficulty=""1""/><Word Text=""clerk"" Difficulty=""1""/><Word Text=""laser"" Difficulty=""1""/><Word Text=""realm"" Difficulty=""1""/><Word Text=""strip"" Difficulty=""1""/><Word Text=""blend"" Difficulty=""1""/><Word Text=""slice"" Difficulty=""1""/><Word Text=""pizza"" Difficulty=""1""/><Word Text=""worry"" Difficulty=""1""/><Word Text=""trail"" Difficulty=""1""/><Word Text=""value"" Difficulty=""1""/><Word Text=""civic"" Difficulty=""1""/><Word Text=""steep"" Difficulty=""1""/><Word Text=""alien"" Difficulty=""1""/><Word Text=""scary"" Difficulty=""1""/><Word Text=""angel"" Difficulty=""1""/><Word Text=""silly"" Difficulty=""1""/><Word Text=""ranch"" Difficulty=""1""/><Word Text=""elder"" Difficulty=""1""/><Word Text=""Dutch"" Difficulty=""1""/><Word Text=""Greek"" Difficulty=""1""/><Word Text=""quest"" Difficulty=""1""/><Word Text=""juror"" Difficulty=""1""/><Word Text=""shock"" Difficulty=""1""/><Word Text=""stiff"" Difficulty=""1""/><Word Text=""toxic"" Difficulty=""1""/><Word Text=""grief"" Difficulty=""1""/><Word Text=""buddy"" Difficulty=""1""/><Word Text=""sword"" Difficulty=""1""/><Word Text=""flash"" Difficulty=""1""/><Word Text=""glory"" Difficulty=""1""/><Word Text=""faint"" Difficulty=""1""/><Word Text=""queen"" Difficulty=""1""/><Word Text=""input"" Difficulty=""1""/><Word Text=""steam"" Difficulty=""1""/><Word Text=""unite"" Difficulty=""1""/><Word Text=""equip"" Difficulty=""1""/><Word Text=""bless"" Difficulty=""1""/><Word Text=""bonus"" Difficulty=""1""/><Word Text=""mixed"" Difficulty=""1""/><Word Text=""orbit"" Difficulty=""1""/><Word Text=""grasp"" Difficulty=""1""/><Word Text=""spite"" Difficulty=""1""/><Word Text=""Cuban"" Difficulty=""1""/><Word Text=""trace"" Difficulty=""1""/><Word Text=""wagon"" Difficulty=""1""/><Word Text=""sheer"" Difficulty=""1""/><Word Text=""prior"" Difficulty=""1""/><Word Text=""thigh"" Difficulty=""1""/><Word Text=""sheep"" Difficulty=""1""/><Word Text=""catch"" Difficulty=""1""/><Word Text=""whale"" Difficulty=""1""/><Word Text=""draft"" Difficulty=""1""/><Word Text=""skull"" Difficulty=""1""/><Word Text=""spell"" Difficulty=""1""/><Word Text=""booth"" Difficulty=""1""/><Word Text=""waist"" Difficulty=""1""/><Word Text=""royal"" Difficulty=""1""/><Word Text=""panic"" Difficulty=""1""/><Word Text=""crush"" Difficulty=""1""/><Word Text=""cliff"" Difficulty=""1""/><Word Text=""tumor"" Difficulty=""1""/><Word Text=""pulse"" Difficulty=""1""/><Word Text=""fixed"" Difficulty=""1""/><Word Text=""diary"" Difficulty=""1""/><Word Text=""irony"" Difficulty=""1""/><Word Text=""spoon"" Difficulty=""1""/><Word Text=""midst"" Difficulty=""1""/><Word Text=""alley"" Difficulty=""1""/><Word Text=""upset"" Difficulty=""1""/><Word Text=""rival"" Difficulty=""1""/><Word Text=""punch"" Difficulty=""1""/><Word Text=""known"" Difficulty=""1""/><Word Text=""purse"" Difficulty=""1""/><Word Text=""cheat"" Difficulty=""1""/><Word Text=""fever"" Difficulty=""1""/><Word Text=""dried"" Difficulty=""1""/><Word Text=""shove"" Difficulty=""1""/><Word Text=""stove"" Difficulty=""1""/><Word Text=""alike"" Difficulty=""1""/><Word Text=""dough"" Difficulty=""1""/><Word Text=""quote"" Difficulty=""1""/><Word Text=""trash"" Difficulty=""1""/><Word Text=""gross"" Difficulty=""1""/><Word Text=""spray"" Difficulty=""1""/><Word Text=""beast"" Difficulty=""1""/><Word Text=""shark"" Difficulty=""1""/><Word Text=""fleet"" Difficulty=""1""/><Word Text=""debut"" Difficulty=""1""/><Word Text=""ideal"" Difficulty=""1""/><Word Text=""scent"" Difficulty=""1""/><Word Text=""stack"" Difficulty=""1""/><Word Text=""cease"" Difficulty=""1""/><Word Text=""nasty"" Difficulty=""1""/><Word Text=""model"" Difficulty=""1""/><Word Text=""wheat"" Difficulty=""1""/><Word Text=""aisle"" Difficulty=""1""/><Word Text=""vocal"" Difficulty=""1""/><Word Text=""risky"" Difficulty=""1""/><Word Text=""pasta"" Difficulty=""1""/><Word Text=""genre"" Difficulty=""1""/><Word Text=""merit"" Difficulty=""1""/><Word Text=""chunk"" Difficulty=""1""/><Word Text=""wound"" Difficulty=""1""/><Word Text=""robot"" Difficulty=""1""/><Word Text=""flood"" Difficulty=""1""/><Word Text=""boast"" Difficulty=""1""/><Word Text=""major"" Difficulty=""1""/><Word Text=""added"" Difficulty=""1""/><Word Text=""sneak"" Difficulty=""1""/><Word Text=""blank"" Difficulty=""1""/><Word Text=""dying"" Difficulty=""1""/><Word Text=""spare"" Difficulty=""1""/><Word Text=""cling"" Difficulty=""1""/><Word Text=""blink"" Difficulty=""1""/><Word Text=""squad"" Difficulty=""1""/><Word Text=""color"" Difficulty=""1""/><Word Text=""chill"" Difficulty=""1""/><Word Text=""steer"" Difficulty=""1""/><Word Text=""rally"" Difficulty=""1""/><Word Text=""cheer"" Difficulty=""1""/><Word Text=""steak"" Difficulty=""1""/><Word Text=""awake"" Difficulty=""1""/><Word Text=""liver"" Difficulty=""1""/><Word Text=""plain"" Difficulty=""1""/><Word Text=""widow"" Difficulty=""1""/><Word Text=""beard"" Difficulty=""1""/><Word Text=""brake"" Difficulty=""1""/><Word Text=""valid"" Difficulty=""1""/><Word Text=""forum"" Difficulty=""1""/><Word Text=""enact"" Difficulty=""1""/><Word Text=""round"" Difficulty=""1""/><Word Text=""light"" Difficulty=""1""/><Word Text=""suite"" Difficulty=""1""/><Word Text=""straw"" Difficulty=""1""/><Word Text=""apart"" Difficulty=""1""/><Word Text=""globe"" Difficulty=""1""/><Word Text=""blast"" Difficulty=""1""/><Word Text=""level"" Difficulty=""1""/><Word Text=""screw"" Difficulty=""1""/><Word Text=""yield"" Difficulty=""1""/><Word Text=""drill"" Difficulty=""1""/><Word Text=""cruel"" Difficulty=""1""/><Word Text=""grape"" Difficulty=""1""/><Word Text=""charm"" Difficulty=""1""/><Word Text=""loyal"" Difficulty=""1""/><Word Text=""pound"" Difficulty=""1""/><Word Text=""radar"" Difficulty=""1""/><Word Text=""frown"" Difficulty=""1""/><Word Text=""leave"" Difficulty=""1""/><Word Text=""spark"" Difficulty=""1""/><Word Text=""blond"" Difficulty=""1""/><Word Text=""twist"" Difficulty=""1""/><Word Text=""arrow"" Difficulty=""1""/><Word Text=""ridge"" Difficulty=""1""/><Word Text=""brave"" Difficulty=""1""/><Word Text=""crowd"" Difficulty=""1""/><Word Text=""dense"" Difficulty=""1""/><Word Text=""sunny"" Difficulty=""1""/><Word Text=""swell"" Difficulty=""1""/><Word Text=""bride"" Difficulty=""1""/><Word Text=""weave"" Difficulty=""1""/><Word Text=""devil"" Difficulty=""1""/><Word Text=""cargo"" Difficulty=""1""/><Word Text=""spine"" Difficulty=""1""/><Word Text=""fatal"" Difficulty=""1""/><Word Text=""drown"" Difficulty=""1""/><Word Text=""kneel"" Difficulty=""1""/><Word Text=""naval"" Difficulty=""1""/><Word Text=""people"" Difficulty=""1""/><Word Text=""should"" Difficulty=""1""/><Word Text=""school"" Difficulty=""1""/><Word Text=""become"" Difficulty=""1""/><Word Text=""really"" Difficulty=""1""/><Word Text=""family"" Difficulty=""1""/><Word Text=""system"" Difficulty=""1""/><Word Text=""during"" Difficulty=""1""/><Word Text=""number"" Difficulty=""1""/><Word Text=""always"" Difficulty=""1""/><Word Text=""happen"" Difficulty=""1""/><Word Text=""before"" Difficulty=""1""/><Word Text=""mother"" Difficulty=""1""/><Word Text=""though"" Difficulty=""1""/><Word Text=""little"" Difficulty=""1""/><Word Text=""around"" Difficulty=""1""/><Word Text=""friend"" Difficulty=""1""/><Word Text=""father"" Difficulty=""1""/><Word Text=""member"" Difficulty=""1""/><Word Text=""almost"" Difficulty=""1""/><Word Text=""change"" Difficulty=""1""/><Word Text=""minute"" Difficulty=""1""/><Word Text=""social"" Difficulty=""1""/><Word Text=""follow"" Difficulty=""1""/><Word Text=""around"" Difficulty=""1""/><Word Text=""parent"" Difficulty=""1""/><Word Text=""create"" Difficulty=""1""/><Word Text=""public"" Difficulty=""1""/><Word Text=""others"" Difficulty=""1""/><Word Text=""office"" Difficulty=""1""/><Word Text=""health"" Difficulty=""1""/><Word Text=""person"" Difficulty=""1""/><Word Text=""within"" Difficulty=""1""/><Word Text=""result"" Difficulty=""1""/><Word Text=""change"" Difficulty=""1""/><Word Text=""reason"" Difficulty=""1""/><Word Text=""before"" Difficulty=""1""/><Word Text=""moment"" Difficulty=""1""/><Word Text=""enough"" Difficulty=""1""/><Word Text=""across"" Difficulty=""1""/><Word Text=""second"" Difficulty=""1""/><Word Text=""toward"" Difficulty=""1""/><Word Text=""policy"" Difficulty=""1""/><Word Text=""appear"" Difficulty=""1""/><Word Text=""market"" Difficulty=""2""/><Word Text=""expect"" Difficulty=""2""/><Word Text=""nation"" Difficulty=""2""/><Word Text=""course"" Difficulty=""2""/><Word Text=""behind"" Difficulty=""2""/><Word Text=""remain"" Difficulty=""2""/><Word Text=""effect"" Difficulty=""2""/><Word Text=""little"" Difficulty=""2""/><Word Text=""former"" Difficulty=""2""/><Word Text=""report"" Difficulty=""2""/><Word Text=""better"" Difficulty=""2""/><Word Text=""effort"" Difficulty=""2""/><Word Text=""decide"" Difficulty=""2""/><Word Text=""strong"" Difficulty=""2""/><Word Text=""leader"" Difficulty=""2""/><Word Text=""police"" Difficulty=""2""/><Word Text=""return"" Difficulty=""2""/><Word Text=""report"" Difficulty=""2""/><Word Text=""better"" Difficulty=""2""/><Word Text=""action"" Difficulty=""2""/><Word Text=""season"" Difficulty=""2""/><Word Text=""player"" Difficulty=""2""/><Word Text=""record"" Difficulty=""2""/><Word Text=""ground"" Difficulty=""2""/><Word Text=""matter"" Difficulty=""2""/><Word Text=""center"" Difficulty=""2""/><Word Text=""couple"" Difficulty=""2""/><Word Text=""figure"" Difficulty=""2""/><Word Text=""street"" Difficulty=""2""/><Word Text=""itself"" Difficulty=""2""/><Word Text=""either"" Difficulty=""2""/><Word Text=""recent"" Difficulty=""2""/><Word Text=""doctor"" Difficulty=""2""/><Word Text=""worker"" Difficulty=""2""/><Word Text=""simply"" Difficulty=""2""/><Word Text=""source"" Difficulty=""2""/><Word Text=""nearly"" Difficulty=""2""/><Word Text=""choose"" Difficulty=""2""/><Word Text=""window"" Difficulty=""2""/><Word Text=""listen"" Difficulty=""2""/><Word Text=""chance"" Difficulty=""2""/><Word Text=""energy"" Difficulty=""2""/><Word Text=""period"" Difficulty=""2""/><Word Text=""course"" Difficulty=""2""/><Word Text=""summer"" Difficulty=""2""/><Word Text=""likely"" Difficulty=""2""/><Word Text=""letter"" Difficulty=""2""/><Word Text=""choice"" Difficulty=""2""/><Word Text=""single"" Difficulty=""2""/><Word Text=""church"" Difficulty=""2""/><Word Text=""future"" Difficulty=""2""/><Word Text=""anyone"" Difficulty=""2""/><Word Text=""myself"" Difficulty=""2""/><Word Text=""second"" Difficulty=""2""/><Word Text=""author"" Difficulty=""2""/><Word Text=""agency"" Difficulty=""2""/><Word Text=""nature"" Difficulty=""2""/><Word Text=""reduce"" Difficulty=""2""/><Word Text=""before"" Difficulty=""2""/><Word Text=""common"" Difficulty=""2""/><Word Text=""series"" Difficulty=""2""/><Word Text=""animal"" Difficulty=""2""/><Word Text=""factor"" Difficulty=""2""/><Word Text=""decade"" Difficulty=""2""/><Word Text=""artist"" Difficulty=""2""/><Word Text=""career"" Difficulty=""2""/><Word Text=""beyond"" Difficulty=""2""/><Word Text=""simple"" Difficulty=""2""/><Word Text=""accept"" Difficulty=""2""/><Word Text=""answer"" Difficulty=""2""/><Word Text=""amount"" Difficulty=""2""/><Word Text=""growth"" Difficulty=""2""/><Word Text=""degree"" Difficulty=""2""/><Word Text=""wonder"" Difficulty=""2""/><Word Text=""attack"" Difficulty=""2""/><Word Text=""region"" Difficulty=""2""/><Word Text=""pretty"" Difficulty=""2""/><Word Text=""arrive"" Difficulty=""2""/><Word Text=""lawyer"" Difficulty=""2""/><Word Text=""answer"" Difficulty=""2""/><Word Text=""sister"" Difficulty=""2""/><Word Text=""design"" Difficulty=""2""/><Word Text=""little"" Difficulty=""2""/><Word Text=""indeed"" Difficulty=""2""/><Word Text=""public"" Difficulty=""2""/><Word Text=""rather"" Difficulty=""2""/><Word Text=""entire"" Difficulty=""2""/><Word Text=""design"" Difficulty=""2""/><Word Text=""enough"" Difficulty=""2""/><Word Text=""forget"" Difficulty=""2""/><Word Text=""remove"" Difficulty=""2""/><Word Text=""memory"" Difficulty=""2""/><Word Text=""expert"" Difficulty=""2""/><Word Text=""spring"" Difficulty=""2""/><Word Text=""finish"" Difficulty=""2""/><Word Text=""theory"" Difficulty=""2""/><Word Text=""impact"" Difficulty=""2""/><Word Text=""charge"" Difficulty=""2""/><Word Text=""reveal"" Difficulty=""2""/><Word Text=""weapon"" Difficulty=""2""/><Word Text=""manage"" Difficulty=""2""/><Word Text=""camera"" Difficulty=""2""/><Word Text=""weight"" Difficulty=""2""/><Word Text=""affect"" Difficulty=""2""/><Word Text=""inside"" Difficulty=""2""/><Word Text=""rather"" Difficulty=""2""/><Word Text=""writer"" Difficulty=""2""/><Word Text=""middle"" Difficulty=""2""/><Word Text=""detail"" Difficulty=""2""/><Word Text=""method"" Difficulty=""2""/><Word Text=""sexual"" Difficulty=""2""/><Word Text=""cancer"" Difficulty=""2""/><Word Text=""finger"" Difficulty=""2""/><Word Text=""garden"" Difficulty=""2""/><Word Text=""notice"" Difficulty=""2""/><Word Text=""modern"" Difficulty=""2""/><Word Text=""budget"" Difficulty=""2""/><Word Text=""victim"" Difficulty=""2""/><Word Text=""threat"" Difficulty=""2""/><Word Text=""dinner"" Difficulty=""2""/><Word Text=""figure"" Difficulty=""2""/><Word Text=""relate"" Difficulty=""2""/><Word Text=""travel"" Difficulty=""2""/><Word Text=""debate"" Difficulty=""2""/><Word Text=""senior"" Difficulty=""2""/><Word Text=""assume"" Difficulty=""2""/><Word Text=""suffer"" Difficulty=""2""/><Word Text=""speech"" Difficulty=""2""/><Word Text=""option"" Difficulty=""2""/><Word Text=""forest"" Difficulty=""2""/><Word Text=""global"" Difficulty=""2""/><Word Text=""Senate"" Difficulty=""2""/><Word Text=""reform"" Difficulty=""2""/><Word Text=""access"" Difficulty=""2""/><Word Text=""credit"" Difficulty=""2""/><Word Text=""corner"" Difficulty=""2""/><Word Text=""recall"" Difficulty=""2""/><Word Text=""safety"" Difficulty=""2""/><Word Text=""income"" Difficulty=""2""/><Word Text=""strike"" Difficulty=""2""/><Word Text=""nobody"" Difficulty=""2""/><Word Text=""object"" Difficulty=""2""/><Word Text=""client"" Difficulty=""2""/><Word Text=""please"" Difficulty=""2""/><Word Text=""attend"" Difficulty=""2""/><Word Text=""spirit"" Difficulty=""2""/><Word Text=""battle"" Difficulty=""2""/><Word Text=""crisis"" Difficulty=""2""/><Word Text=""define"" Difficulty=""2""/><Word Text=""easily"" Difficulty=""2""/><Word Text=""vision"" Difficulty=""2""/><Word Text=""status"" Difficulty=""2""/><Word Text=""normal"" Difficulty=""2""/><Word Text=""slowly"" Difficulty=""2""/><Word Text=""driver"" Difficulty=""2""/><Word Text=""handle"" Difficulty=""2""/><Word Text=""return"" Difficulty=""2""/><Word Text=""survey"" Difficulty=""2""/><Word Text=""winter"" Difficulty=""2""/><Word Text=""Soviet"" Difficulty=""2""/><Word Text=""refuse"" Difficulty=""2""/><Word Text=""screen"" Difficulty=""2""/><Word Text=""future"" Difficulty=""2""/><Word Text=""middle"" Difficulty=""2""/><Word Text=""reader"" Difficulty=""2""/><Word Text=""target"" Difficulty=""2""/><Word Text=""prison"" Difficulty=""2""/><Word Text=""demand"" Difficulty=""2""/><Word Text=""flight"" Difficulty=""2""/><Word Text=""inside"" Difficulty=""2""/><Word Text=""emerge"" Difficulty=""2""/><Word Text=""bright"" Difficulty=""2""/><Word Text=""sample"" Difficulty=""2""/><Word Text=""settle"" Difficulty=""2""/><Word Text=""highly"" Difficulty=""2""/><Word Text=""mostly"" Difficulty=""2""/><Word Text=""lesson"" Difficulty=""2""/><Word Text=""living"" Difficulty=""2""/><Word Text=""unless"" Difficulty=""2""/><Word Text=""border"" Difficulty=""2""/><Word Text=""gather"" Difficulty=""2""/><Word Text=""critic"" Difficulty=""2""/><Word Text=""aspect"" Difficulty=""2""/><Word Text=""result"" Difficulty=""2""/><Word Text=""insist"" Difficulty=""2""/><Word Text=""annual"" Difficulty=""2""/><Word Text=""French"" Difficulty=""2""/><Word Text=""affair"" Difficulty=""2""/><Word Text=""spread"" Difficulty=""2""/><Word Text=""ignore"" Difficulty=""2""/><Word Text=""belief"" Difficulty=""2""/><Word Text=""murder"" Difficulty=""2""/><Word Text=""review"" Difficulty=""2""/><Word Text=""editor"" Difficulty=""2""/><Word Text=""engage"" Difficulty=""2""/><Word Text=""coffee"" Difficulty=""2""/><Word Text=""anyway"" Difficulty=""2""/><Word Text=""commit"" Difficulty=""2""/><Word Text=""female"" Difficulty=""2""/><Word Text=""afraid"" Difficulty=""2""/><Word Text=""native"" Difficulty=""2""/><Word Text=""charge"" Difficulty=""2""/><Word Text=""Indian"" Difficulty=""2""/><Word Text=""active"" Difficulty=""2""/><Word Text=""extend"" Difficulty=""2""/><Word Text=""demand"" Difficulty=""2""/><Word Text=""remind"" Difficulty=""2""/><Word Text=""United"" Difficulty=""2""/><Word Text=""depend"" Difficulty=""2""/><Word Text=""direct"" Difficulty=""2""/><Word Text=""famous"" Difficulty=""2""/><Word Text=""flower"" Difficulty=""2""/><Word Text=""supply"" Difficulty=""2""/><Word Text=""search"" Difficulty=""2""/><Word Text=""circle"" Difficulty=""2""/><Word Text=""device"" Difficulty=""2""/><Word Text=""bottom"" Difficulty=""2""/><Word Text=""island"" Difficulty=""2""/><Word Text=""studio"" Difficulty=""2""/><Word Text=""damage"" Difficulty=""2""/><Word Text=""intend"" Difficulty=""2""/><Word Text=""attack"" Difficulty=""2""/><Word Text=""danger"" Difficulty=""2""/><Word Text=""desire"" Difficulty=""2""/><Word Text=""injury"" Difficulty=""2""/><Word Text=""direct"" Difficulty=""2""/><Word Text=""engine"" Difficulty=""2""/><Word Text=""fourth"" Difficulty=""2""/><Word Text=""expand"" Difficulty=""2""/><Word Text=""ticket"" Difficulty=""2""/><Word Text=""mental"" Difficulty=""2""/><Word Text=""farmer"" Difficulty=""2""/><Word Text=""planet"" Difficulty=""2""/><Word Text=""obtain"" Difficulty=""2""/><Word Text=""invite"" Difficulty=""2""/><Word Text=""repeat"" Difficulty=""2""/><Word Text=""pocket"" Difficulty=""2""/><Word Text=""breath"" Difficulty=""2""/><Word Text=""belong"" Difficulty=""2""/><Word Text=""advice"" Difficulty=""2""/><Word Text=""breast"" Difficulty=""2""/><Word Text=""record"" Difficulty=""2""/><Word Text=""thanks"" Difficulty=""2""/><Word Text=""yellow"" Difficulty=""2""/><Word Text=""shadow"" Difficulty=""2""/><Word Text=""locate"" Difficulty=""2""/><Word Text=""county"" Difficulty=""2""/><Word Text=""bridge"" Difficulty=""2""/><Word Text=""e-mail"" Difficulty=""2""/><Word Text=""profit"" Difficulty=""2""/><Word Text=""muscle"" Difficulty=""2""/><Word Text=""notion"" Difficulty=""2""/><Word Text=""prefer"" Difficulty=""2""/><Word Text=""search"" Difficulty=""2""/><Word Text=""museum"" Difficulty=""2""/><Word Text=""beauty"" Difficulty=""2""/><Word Text=""unique"" Difficulty=""2""/><Word Text=""ethnic"" Difficulty=""2""/><Word Text=""stress"" Difficulty=""2""/><Word Text=""select"" Difficulty=""2""/><Word Text=""actual"" Difficulty=""2""/><Word Text=""bottle"" Difficulty=""2""/><Word Text=""hardly"" Difficulty=""2""/><Word Text=""launch"" Difficulty=""2""/><Word Text=""defend"" Difficulty=""2""/><Word Text=""matter"" Difficulty=""2""/><Word Text=""ensure"" Difficulty=""2""/><Word Text=""extent"" Difficulty=""2""/><Word Text=""estate"" Difficulty=""2""/><Word Text=""pursue"" Difficulty=""2""/><Word Text=""Jewish"" Difficulty=""2""/><Word Text=""branch"" Difficulty=""2""/><Word Text=""relief"" Difficulty=""2""/><Word Text=""manner"" Difficulty=""2""/><Word Text=""rating"" Difficulty=""2""/><Word Text=""golden"" Difficulty=""2""/><Word Text=""motion"" Difficulty=""2""/><Word Text=""German"" Difficulty=""2""/><Word Text=""gender"" Difficulty=""2""/><Word Text=""except"" Difficulty=""2""/><Word Text=""afford"" Difficulty=""2""/><Word Text=""regime"" Difficulty=""2""/><Word Text=""appeal"" Difficulty=""2""/><Word Text=""mirror"" Difficulty=""2""/><Word Text=""length"" Difficulty=""2""/><Word Text=""secret"" Difficulty=""2""/><Word Text=""master"" Difficulty=""2""/><Word Text=""except"" Difficulty=""2""/><Word Text=""winner"" Difficulty=""2""/><Word Text=""volume"" Difficulty=""2""/><Word Text=""travel"" Difficulty=""2""/><Word Text=""pepper"" Difficulty=""2""/><Word Text=""divide"" Difficulty=""2""/><Word Text=""oppose"" Difficulty=""2""/><Word Text=""league"" Difficulty=""2""/><Word Text=""employ"" Difficulty=""2""/><Word Text=""barely"" Difficulty=""2""/><Word Text=""sector"" Difficulty=""2""/><Word Text=""beside"" Difficulty=""2""/><Word Text=""merely"" Difficulty=""2""/><Word Text=""female"" Difficulty=""2""/><Word Text=""invest"" Difficulty=""2""/><Word Text=""expose"" Difficulty=""2""/><Word Text=""narrow"" Difficulty=""2""/><Word Text=""either"" Difficulty=""2""/><Word Text=""accuse"" Difficulty=""2""/><Word Text=""useful"" Difficulty=""2""/><Word Text=""secret"" Difficulty=""2""/><Word Text=""reject"" Difficulty=""2""/><Word Text=""talent"" Difficulty=""2""/><Word Text=""escape"" Difficulty=""2""/><Word Text=""height"" Difficulty=""2""/><Word Text=""assess"" Difficulty=""2""/><Word Text=""plenty"" Difficulty=""2""/><Word Text=""behind"" Difficulty=""2""/><Word Text=""campus"" Difficulty=""2""/><Word Text=""proper"" Difficulty=""2""/><Word Text=""guilty"" Difficulty=""2""/><Word Text=""living"" Difficulty=""2""/><Word Text=""column"" Difficulty=""2""/><Word Text=""signal"" Difficulty=""2""/><Word Text=""regard"" Difficulty=""2""/><Word Text=""twenty"" Difficulty=""2""/><Word Text=""review"" Difficulty=""2""/><Word Text=""prayer"" Difficulty=""2""/><Word Text=""cheese"" Difficulty=""2""/><Word Text=""permit"" Difficulty=""2""/><Word Text=""scream"" Difficulty=""2""/><Word Text=""deeply"" Difficulty=""2""/><Word Text=""agenda"" Difficulty=""2""/><Word Text=""unable"" Difficulty=""2""/><Word Text=""arrest"" Difficulty=""2""/><Word Text=""visual"" Difficulty=""2""/><Word Text=""fairly"" Difficulty=""2""/><Word Text=""silent"" Difficulty=""2""/><Word Text=""widely"" Difficulty=""2""/><Word Text=""inform"" Difficulty=""2""/><Word Text=""bother"" Difficulty=""2""/><Word Text=""enable"" Difficulty=""2""/><Word Text=""saving"" Difficulty=""2""/><Word Text=""desert"" Difficulty=""2""/><Word Text=""double"" Difficulty=""2""/><Word Text=""formal"" Difficulty=""2""/><Word Text=""stream"" Difficulty=""2""/><Word Text=""racial"" Difficulty=""2""/><Word Text=""potato"" Difficulty=""2""/><Word Text=""online"" Difficulty=""2""/><Word Text=""jacket"" Difficulty=""2""/><Word Text=""rarely"" Difficulty=""2""/><Word Text=""priest"" Difficulty=""2""/><Word Text=""adjust"" Difficulty=""2""/><Word Text=""retire"" Difficulty=""2""/><Word Text=""attach"" Difficulty=""2""/><Word Text=""Indian"" Difficulty=""2""/><Word Text=""severe"" Difficulty=""2""/><Word Text=""impose"" Difficulty=""2""/><Word Text=""symbol"" Difficulty=""2""/><Word Text=""clinic"" Difficulty=""2""/><Word Text=""tomato"" Difficulty=""2""/><Word Text=""butter"" Difficulty=""2""/><Word Text=""surely"" Difficulty=""2""/><Word Text=""glance"" Difficulty=""2""/><Word Text=""fellow"" Difficulty=""2""/><Word Text=""smooth"" Difficulty=""2""/><Word Text=""nearby"" Difficulty=""2""/><Word Text=""silver"" Difficulty=""2""/><Word Text=""junior"" Difficulty=""2""/><Word Text=""rather"" Difficulty=""2""/><Word Text=""throat"" Difficulty=""2""/><Word Text=""salary"" Difficulty=""2""/><Word Text=""pretty"" Difficulty=""2""/><Word Text=""strike"" Difficulty=""2""/><Word Text=""unlike"" Difficulty=""2""/><Word Text=""resist"" Difficulty=""2""/><Word Text=""supply"" Difficulty=""2""/><Word Text=""assist"" Difficulty=""2""/><Word Text=""viewer"" Difficulty=""2""/><Word Text=""secure"" Difficulty=""2""/><Word Text=""recipe"" Difficulty=""2""/><Word Text=""wooden"" Difficulty=""2""/><Word Text=""honest"" Difficulty=""2""/><Word Text=""origin"" Difficulty=""2""/><Word Text=""advise"" Difficulty=""2""/><Word Text=""wealth"" Difficulty=""2""/><Word Text=""deputy"" Difficulty=""2""/><Word Text=""assure"" Difficulty=""2""/><Word Text=""dealer"" Difficulty=""2""/><Word Text=""phrase"" Difficulty=""2""/><Word Text=""Muslim"" Difficulty=""2""/><Word Text=""switch"" Difficulty=""2""/><Word Text=""killer"" Difficulty=""2""/><Word Text=""assign"" Difficulty=""2""/><Word Text=""heaven"" Difficulty=""2""/><Word Text=""wonder"" Difficulty=""2""/><Word Text=""button"" Difficulty=""2""/><Word Text=""bottom"" Difficulty=""2""/><Word Text=""burden"" Difficulty=""2""/><Word Text=""string"" Difficulty=""2""/><Word Text=""resort"" Difficulty=""2""/><Word Text=""tissue"" Difficulty=""2""/><Word Text=""broken"" Difficulty=""2""/><Word Text=""stupid"" Difficulty=""2""/><Word Text=""occupy"" Difficulty=""2""/><Word Text=""cousin"" Difficulty=""2""/><Word Text=""retain"" Difficulty=""2""/><Word Text=""latter"" Difficulty=""2""/><Word Text=""terror"" Difficulty=""2""/><Word Text=""though"" Difficulty=""2""/><Word Text=""bullet"" Difficulty=""2""/><Word Text=""square"" Difficulty=""2""/><Word Text=""gently"" Difficulty=""2""/><Word Text=""detect"" Difficulty=""2""/><Word Text=""likely"" Difficulty=""2""/><Word Text=""market"" Difficulty=""2""/><Word Text=""remote"" Difficulty=""2""/><Word Text=""mutual"" Difficulty=""2""/><Word Text=""mainly"" Difficulty=""2""/><Word Text=""freeze"" Difficulty=""2""/><Word Text=""singer"" Difficulty=""2""/><Word Text=""evolve"" Difficulty=""2""/><Word Text=""partly"" Difficulty=""2""/><Word Text=""thirty"" Difficulty=""2""/><Word Text=""treaty"" Difficulty=""2""/><Word Text=""double"" Difficulty=""2""/><Word Text=""sudden"" Difficulty=""2""/><Word Text=""tongue"" Difficulty=""2""/><Word Text=""target"" Difficulty=""2""/><Word Text=""stable"" Difficulty=""2""/><Word Text=""appeal"" Difficulty=""2""/><Word Text=""steady"" Difficulty=""2""/><Word Text=""stress"" Difficulty=""2""/><Word Text=""vessel"" Difficulty=""2""/><Word Text=""mm-hmm"" Difficulty=""2""/><Word Text=""dining"" Difficulty=""2""/><Word Text=""wisdom"" Difficulty=""2""/><Word Text=""garlic"" Difficulty=""2""/><Word Text=""poetry"" Difficulty=""2""/><Word Text=""scared"" Difficulty=""2""/><Word Text=""fellow"" Difficulty=""2""/><Word Text=""slight"" Difficulty=""2""/><Word Text=""differ"" Difficulty=""2""/><Word Text=""custom"" Difficulty=""2""/><Word Text=""damage"" Difficulty=""2""/><Word Text=""carbon"" Difficulty=""2""/><Word Text=""closer"" Difficulty=""2""/><Word Text=""scheme"" Difficulty=""2""/><Word Text=""galaxy"" Difficulty=""2""/><Word Text=""arrest"" Difficulty=""2""/><Word Text=""hunter"" Difficulty=""2""/><Word Text=""infant"" Difficulty=""2""/><Word Text=""derive"" Difficulty=""2""/><Word Text=""fabric"" Difficulty=""2""/><Word Text=""French"" Difficulty=""2""/><Word Text=""asleep"" Difficulty=""2""/><Word Text=""tennis"" Difficulty=""2""/><Word Text=""barrel"" Difficulty=""2""/><Word Text=""modest"" Difficulty=""2""/><Word Text=""stroke"" Difficulty=""2""/><Word Text=""prompt"" Difficulty=""2""/><Word Text=""absorb"" Difficulty=""2""/><Word Text=""across"" Difficulty=""2""/><Word Text=""cotton"" Difficulty=""2""/><Word Text=""flavor"" Difficulty=""2""/><Word Text=""orange"" Difficulty=""2""/><Word Text=""assert"" Difficulty=""2""/><Word Text=""valley"" Difficulty=""2""/><Word Text=""versus"" Difficulty=""2""/><Word Text=""German"" Difficulty=""2""/><Word Text=""hungry"" Difficulty=""2""/><Word Text=""wander"" Difficulty=""2""/><Word Text=""submit"" Difficulty=""2""/><Word Text=""legacy"" Difficulty=""2""/><Word Text=""shower"" Difficulty=""2""/><Word Text=""depict"" Difficulty=""2""/><Word Text=""garage"" Difficulty=""2""/><Word Text=""borrow"" Difficulty=""2""/><Word Text=""comedy"" Difficulty=""2""/><Word Text=""twelve"" Difficulty=""2""/><Word Text=""weekly"" Difficulty=""2""/><Word Text=""devote"" Difficulty=""2""/><Word Text=""ethics"" Difficulty=""2""/><Word Text=""summit"" Difficulty=""2""/><Word Text=""gifted"" Difficulty=""2""/><Word Text=""medium"" Difficulty=""2""/><Word Text=""basket"" Difficulty=""2""/><Word Text=""powder"" Difficulty=""2""/><Word Text=""cookie"" Difficulty=""2""/><Word Text=""orange"" Difficulty=""2""/><Word Text=""admire"" Difficulty=""2""/><Word Text=""exceed"" Difficulty=""2""/><Word Text=""rhythm"" Difficulty=""2""/><Word Text=""lovely"" Difficulty=""2""/><Word Text=""script"" Difficulty=""2""/><Word Text=""tactic"" Difficulty=""2""/><Word Text=""margin"" Difficulty=""2""/><Word Text=""horror"" Difficulty=""2""/><Word Text=""defeat"" Difficulty=""2""/><Word Text=""sacred"" Difficulty=""2""/><Word Text=""square"" Difficulty=""2""/><Word Text=""soccer"" Difficulty=""2""/><Word Text=""tunnel"" Difficulty=""2""/><Word Text=""virtue"" Difficulty=""2""/><Word Text=""abroad"" Difficulty=""2""/><Word Text=""makeup"" Difficulty=""2""/><Word Text=""legend"" Difficulty=""2""/><Word Text=""remark"" Difficulty=""2""/><Word Text=""resign"" Difficulty=""2""/><Word Text=""reward"" Difficulty=""2""/><Word Text=""gentle"" Difficulty=""2""/><Word Text=""invent"" Difficulty=""2""/><Word Text=""ritual"" Difficulty=""2""/><Word Text=""insect"" Difficulty=""2""/><Word Text=""salmon"" Difficulty=""2""/><Word Text=""combat"" Difficulty=""2""/><Word Text=""bitter"" Difficulty=""2""/><Word Text=""subtle"" Difficulty=""2""/><Word Text=""bishop"" Difficulty=""2""/><Word Text=""export"" Difficulty=""2""/><Word Text=""closet"" Difficulty=""2""/><Word Text=""murder"" Difficulty=""2""/><Word Text=""retail"" Difficulty=""2""/><Word Text=""excuse"" Difficulty=""2""/><Word Text=""online"" Difficulty=""2""/><Word Text=""deadly"" Difficulty=""2""/><Word Text=""Muslim"" Difficulty=""2""/><Word Text=""Korean"" Difficulty=""2""/><Word Text=""suburb"" Difficulty=""2""/><Word Text=""unlike"" Difficulty=""2""/><Word Text=""render"" Difficulty=""2""/><Word Text=""strict"" Difficulty=""2""/><Word Text=""motive"" Difficulty=""2""/><Word Text=""notice"" Difficulty=""2""/><Word Text=""temple"" Difficulty=""2""/><Word Text=""medium"" Difficulty=""2""/><Word Text=""and/or"" Difficulty=""2""/><Word Text=""random"" Difficulty=""2""/><Word Text=""domain"" Difficulty=""2""/><Word Text=""cattle"" Difficulty=""2""/><Word Text=""fiscal"" Difficulty=""2""/><Word Text=""endure"" Difficulty=""2""/><Word Text=""strain"" Difficulty=""2""/><Word Text=""guitar"" Difficulty=""2""/><Word Text=""behave"" Difficulty=""2""/><Word Text=""dancer"" Difficulty=""2""/><Word Text=""colony"" Difficulty=""2""/><Word Text=""closed"" Difficulty=""2""/><Word Text=""modify"" Difficulty=""2""/><Word Text=""glance"" Difficulty=""2""/><Word Text=""survey"" Difficulty=""2""/><Word Text=""govern"" Difficulty=""2""/><Word Text=""ballot"" Difficulty=""2""/><Word Text=""praise"" Difficulty=""2""/><Word Text=""injure"" Difficulty=""2""/><Word Text=""nearby"" Difficulty=""2""/><Word Text=""excuse"" Difficulty=""2""/><Word Text=""canvas"" Difficulty=""2""/><Word Text=""matter"" Difficulty=""2""/><Word Text=""format"" Difficulty=""2""/><Word Text=""turkey"" Difficulty=""2""/><Word Text=""convey"" Difficulty=""2""/><Word Text=""finish"" Difficulty=""2""/><Word Text=""frozen"" Difficulty=""2""/><Word Text=""desire"" Difficulty=""2""/><Word Text=""spouse"" Difficulty=""2""/><Word Text=""resume"" Difficulty=""2""/><Word Text=""sodium"" Difficulty=""2""/><Word Text=""bounce"" Difficulty=""2""/><Word Text=""signal"" Difficulty=""2""/><Word Text=""pickup"" Difficulty=""2""/><Word Text=""needle"" Difficulty=""2""/><Word Text=""timing"" Difficulty=""2""/><Word Text=""rescue"" Difficulty=""2""/><Word Text=""firmly"" Difficulty=""2""/><Word Text=""poster"" Difficulty=""2""/><Word Text=""oxygen"" Difficulty=""2""/><Word Text=""pastor"" Difficulty=""2""/><Word Text=""punish"" Difficulty=""2""/><Word Text=""equity"" Difficulty=""2""/><Word Text=""statue"" Difficulty=""2""/><Word Text=""repair"" Difficulty=""2""/><Word Text=""decent"" Difficulty=""2""/><Word Text=""rescue"" Difficulty=""2""/><Word Text=""purple"" Difficulty=""2""/><Word Text=""eating"" Difficulty=""2""/><Word Text=""parade"" Difficulty=""2""/><Word Text=""cancel"" Difficulty=""2""/><Word Text=""debate"" Difficulty=""2""/><Word Text=""candle"" Difficulty=""2""/><Word Text=""handle"" Difficulty=""2""/><Word Text=""entity"" Difficulty=""2""/><Word Text=""inside"" Difficulty=""2""/><Word Text=""vanish"" Difficulty=""2""/><Word Text=""racism"" Difficulty=""2""/><Word Text=""casual"" Difficulty=""2""/><Word Text=""enroll"" Difficulty=""2""/><Word Text=""intent"" Difficulty=""2""/><Word Text=""switch"" Difficulty=""2""/><Word Text=""repair"" Difficulty=""2""/><Word Text=""toilet"" Difficulty=""2""/><Word Text=""hidden"" Difficulty=""2""/><Word Text=""tender"" Difficulty=""2""/><Word Text=""lonely"" Difficulty=""2""/><Word Text=""shared"" Difficulty=""2""/><Word Text=""pillow"" Difficulty=""2""/><Word Text=""spread"" Difficulty=""2""/><Word Text=""ruling"" Difficulty=""2""/><Word Text=""lately"" Difficulty=""2""/><Word Text=""softly"" Difficulty=""2""/><Word Text=""verbal"" Difficulty=""2""/><Word Text=""tribal"" Difficulty=""2""/><Word Text=""import"" Difficulty=""2""/><Word Text=""spring"" Difficulty=""2""/><Word Text=""divine"" Difficulty=""2""/><Word Text=""genius"" Difficulty=""2""/><Word Text=""broker"" Difficulty=""2""/><Word Text=""credit"" Difficulty=""2""/><Word Text=""output"" Difficulty=""2""/><Word Text=""please"" Difficulty=""2""/><Word Text=""rocket"" Difficulty=""2""/><Word Text=""donate"" Difficulty=""2""/><Word Text=""inmate"" Difficulty=""2""/><Word Text=""tackle"" Difficulty=""2""/><Word Text=""senior"" Difficulty=""2""/><Word Text=""carpet"" Difficulty=""2""/><Word Text=""bubble"" Difficulty=""2""/><Word Text=""bloody"" Difficulty=""2""/><Word Text=""defeat"" Difficulty=""2""/><Word Text=""accent"" Difficulty=""2""/><Word Text=""escape"" Difficulty=""2""/><Word Text=""shrimp"" Difficulty=""2""/><Word Text=""voting"" Difficulty=""2""/><Word Text=""patrol"" Difficulty=""2""/><Word Text=""immune"" Difficulty=""2""/><Word Text=""exotic"" Difficulty=""2""/><Word Text=""secure"" Difficulty=""2""/><Word Text=""drawer"" Difficulty=""2""/><Word Text=""regard"" Difficulty=""2""/><Word Text=""runner"" Difficulty=""2""/><Word Text=""empire"" Difficulty=""2""/><Word Text=""puzzle"" Difficulty=""2""/><Word Text=""tragic"" Difficulty=""2""/><Word Text=""safely"" Difficulty=""2""/><Word Text=""eleven"" Difficulty=""2""/><Word Text=""bureau"" Difficulty=""2""/><Word Text=""breeze"" Difficulty=""2""/><Word Text=""costly"" Difficulty=""2""/><Word Text=""object"" Difficulty=""2""/><Word Text=""insert"" Difficulty=""2""/><Word Text=""helmet"" Difficulty=""2""/><Word Text=""casino"" Difficulty=""2""/><Word Text=""charge"" Difficulty=""2""/><Word Text=""hockey"" Difficulty=""2""/><Word Text=""liquid"" Difficulty=""2""/><Word Text=""foster"" Difficulty=""2""/><Word Text=""access"" Difficulty=""2""/><Word Text=""filter"" Difficulty=""2""/><Word Text=""rabbit"" Difficulty=""2""/><Word Text=""outfit"" Difficulty=""2""/><Word Text=""patent"" Difficulty=""2""/><Word Text=""pencil"" Difficulty=""2""/><Word Text=""banker"" Difficulty=""2""/><Word Text=""eighth"" Difficulty=""2""/><Word Text=""behalf"" Difficulty=""2""/><Word Text=""reward"" Difficulty=""2""/><Word Text=""stance"" Difficulty=""2""/><Word Text=""compel"" Difficulty=""2""/><Word Text=""shrink"" Difficulty=""2""/><Word Text=""fierce"" Difficulty=""2""/><Word Text=""weaken"" Difficulty=""2""/><Word Text=""openly"" Difficulty=""2""/><Word Text=""unfair"" Difficulty=""2""/><Word Text=""deploy"" Difficulty=""2""/><Word Text=""ladder"" Difficulty=""2""/><Word Text=""jungle"" Difficulty=""2""/><Word Text=""invade"" Difficulty=""2""/><Word Text=""sphere"" Difficulty=""2""/><Word Text=""unfold"" Difficulty=""2""/><Word Text=""collar"" Difficulty=""2""/><Word Text=""streak"" Difficulty=""2""/><Word Text=""monkey"" Difficulty=""2""/><Word Text=""mentor"" Difficulty=""2""/><Word Text=""sleeve"" Difficulty=""2""/><Word Text=""debris"" Difficulty=""2""/><Word Text=""parish"" Difficulty=""2""/><Word Text=""hunger"" Difficulty=""2""/><Word Text=""faster"" Difficulty=""2""/><Word Text=""regret"" Difficulty=""2""/><Word Text=""carrot"" Difficulty=""2""/><Word Text=""plunge"" Difficulty=""2""/><Word Text=""refuge"" Difficulty=""2""/><Word Text=""outlet"" Difficulty=""2""/><Word Text=""intact"" Difficulty=""2""/><Word Text=""vendor"" Difficulty=""2""/><Word Text=""thrive"" Difficulty=""2""/><Word Text=""peanut"" Difficulty=""2""/><Word Text=""comply"" Difficulty=""2""/><Word Text=""strain"" Difficulty=""2""/><Word Text=""patron"" Difficulty=""2""/><Word Text=""solely"" Difficulty=""2""/><Word Text=""banana"" Difficulty=""2""/><Word Text=""palace"" Difficulty=""2""/><Word Text=""cruise"" Difficulty=""2""/><Word Text=""mobile"" Difficulty=""2""/><Word Text=""forbid"" Difficulty=""2""/><Word Text=""brutal"" Difficulty=""2""/><Word Text=""thread"" Difficulty=""2""/><Word Text=""coming"" Difficulty=""2""/><Word Text=""remark"" Difficulty=""2""/><Word Text=""circle"" Difficulty=""2""/><Word Text=""denial"" Difficulty=""2""/><Word Text=""rental"" Difficulty=""2""/><Word Text=""warmth"" Difficulty=""2""/><Word Text=""liquid"" Difficulty=""2""/><Word Text=""battle"" Difficulty=""2""/><Word Text=""regard"" Difficulty=""2""/><Word Text=""regain"" Difficulty=""2""/><Word Text=""permit"" Difficulty=""2""/><Word Text=""rubber"" Difficulty=""2""/><Word Text=""freely"" Difficulty=""2""/><Word Text=""update"" Difficulty=""2""/><Word Text=""beyond"" Difficulty=""2""/><Word Text=""marker"" Difficulty=""2""/><Word Text=""preach"" Difficulty=""2""/><Word Text=""bucket"" Difficulty=""2""/><Word Text=""marble"" Difficulty=""2""/><Word Text=""mutter"" Difficulty=""2""/><Word Text=""depart"" Difficulty=""2""/><Word Text=""trauma"" Difficulty=""2""/><Word Text=""ribbon"" Difficulty=""2""/><Word Text=""screen"" Difficulty=""2""/><Word Text=""within"" Difficulty=""2""/><Word Text=""shorts"" Difficulty=""2""/><Word Text=""soften"" Difficulty=""2""/><Word Text=""sudden"" Difficulty=""2""/><Word Text=""hazard"" Difficulty=""2""/><Word Text=""seldom"" Difficulty=""2""/><Word Text=""launch"" Difficulty=""2""/><Word Text=""timber"" Difficulty=""2""/><Word Text=""flying"" Difficulty=""2""/><Word Text=""seller"" Difficulty=""2""/><Word Text=""public"" Difficulty=""2""/><Word Text=""marine"" Difficulty=""2""/><Word Text=""boring"" Difficulty=""2""/><Word Text=""bronze"" Difficulty=""2""/><Word Text=""praise"" Difficulty=""2""/><Word Text=""vacuum"" Difficulty=""2""/><Word Text=""sensor"" Difficulty=""2""/><Word Text=""manual"" Difficulty=""2""/><Word Text=""pistol"" Difficulty=""2""/><Word Text=""because"" Difficulty=""2""/><Word Text=""through"" Difficulty=""2""/><Word Text=""between"" Difficulty=""2""/><Word Text=""another"" Difficulty=""2""/><Word Text=""student"" Difficulty=""2""/><Word Text=""country"" Difficulty=""2""/><Word Text=""problem"" Difficulty=""2""/><Word Text=""against"" Difficulty=""2""/><Word Text=""company"" Difficulty=""2""/><Word Text=""program"" Difficulty=""2""/><Word Text=""believe"" Difficulty=""2""/><Word Text=""without"" Difficulty=""2""/><Word Text=""million"" Difficulty=""2""/><Word Text=""provide"" Difficulty=""2""/><Word Text=""service"" Difficulty=""2""/><Word Text=""however"" Difficulty=""2""/><Word Text=""include"" Difficulty=""2""/><Word Text=""several"" Difficulty=""2""/><Word Text=""nothing"" Difficulty=""2""/><Word Text=""whether"" Difficulty=""2""/><Word Text=""already"" Difficulty=""2""/><Word Text=""history"" Difficulty=""2""/><Word Text=""morning"" Difficulty=""2""/><Word Text=""himself"" Difficulty=""2""/><Word Text=""teacher"" Difficulty=""2""/><Word Text=""process"" Difficulty=""2""/><Word Text=""college"" Difficulty=""3""/><Word Text=""someone"" Difficulty=""3""/><Word Text=""suggest"" Difficulty=""3""/><Word Text=""control"" Difficulty=""3""/><Word Text=""perhaps"" Difficulty=""3""/><Word Text=""require"" Difficulty=""3""/><Word Text=""finally"" Difficulty=""3""/><Word Text=""explain"" Difficulty=""3""/><Word Text=""develop"" Difficulty=""3""/><Word Text=""federal"" Difficulty=""3""/><Word Text=""receive"" Difficulty=""3""/><Word Text=""society"" Difficulty=""3""/><Word Text=""because"" Difficulty=""3""/><Word Text=""special"" Difficulty=""3""/><Word Text=""support"" Difficulty=""3""/><Word Text=""project"" Difficulty=""3""/><Word Text=""produce"" Difficulty=""3""/><Word Text=""picture"" Difficulty=""3""/><Word Text=""product"" Difficulty=""3""/><Word Text=""patient"" Difficulty=""3""/><Word Text=""certain"" Difficulty=""3""/><Word Text=""support"" Difficulty=""3""/><Word Text=""century"" Difficulty=""3""/><Word Text=""culture"" Difficulty=""3""/><Word Text=""billion"" Difficulty=""3""/><Word Text=""brother"" Difficulty=""3""/><Word Text=""realize"" Difficulty=""3""/><Word Text=""hundred"" Difficulty=""3""/><Word Text=""husband"" Difficulty=""3""/><Word Text=""economy"" Difficulty=""3""/><Word Text=""medical"" Difficulty=""3""/><Word Text=""current"" Difficulty=""3""/><Word Text=""involve"" Difficulty=""3""/><Word Text=""defense"" Difficulty=""3""/><Word Text=""subject"" Difficulty=""3""/><Word Text=""officer"" Difficulty=""3""/><Word Text=""private"" Difficulty=""3""/><Word Text=""quickly"" Difficulty=""3""/><Word Text=""foreign"" Difficulty=""3""/><Word Text=""natural"" Difficulty=""3""/><Word Text=""concern"" Difficulty=""3""/><Word Text=""similar"" Difficulty=""3""/><Word Text=""usually"" Difficulty=""3""/><Word Text=""article"" Difficulty=""3""/><Word Text=""despite"" Difficulty=""3""/><Word Text=""central"" Difficulty=""3""/><Word Text=""exactly"" Difficulty=""3""/><Word Text=""protect"" Difficulty=""3""/><Word Text=""serious"" Difficulty=""3""/><Word Text=""thought"" Difficulty=""3""/><Word Text=""quality"" Difficulty=""3""/><Word Text=""meeting"" Difficulty=""3""/><Word Text=""prepare"" Difficulty=""3""/><Word Text=""disease"" Difficulty=""3""/><Word Text=""success"" Difficulty=""3""/><Word Text=""ability"" Difficulty=""3""/><Word Text=""herself"" Difficulty=""3""/><Word Text=""general"" Difficulty=""3""/><Word Text=""feeling"" Difficulty=""3""/><Word Text=""message"" Difficulty=""3""/><Word Text=""outside"" Difficulty=""3""/><Word Text=""benefit"" Difficulty=""3""/><Word Text=""forward"" Difficulty=""3""/><Word Text=""present"" Difficulty=""3""/><Word Text=""section"" Difficulty=""3""/><Word Text=""compare"" Difficulty=""3""/><Word Text=""station"" Difficulty=""3""/><Word Text=""clearly"" Difficulty=""3""/><Word Text=""discuss"" Difficulty=""3""/><Word Text=""example"" Difficulty=""3""/><Word Text=""various"" Difficulty=""3""/><Word Text=""manager"" Difficulty=""3""/><Word Text=""network"" Difficulty=""3""/><Word Text=""science"" Difficulty=""3""/><Word Text=""imagine"" Difficulty=""3""/><Word Text=""tonight"" Difficulty=""3""/><Word Text=""respond"" Difficulty=""3""/><Word Text=""popular"" Difficulty=""3""/><Word Text=""contain"" Difficulty=""3""/><Word Text=""control"" Difficulty=""3""/><Word Text=""measure"" Difficulty=""3""/><Word Text=""perform"" Difficulty=""3""/><Word Text=""evening"" Difficulty=""3""/><Word Text=""mention"" Difficulty=""3""/><Word Text=""trouble"" Difficulty=""3""/><Word Text=""instead"" Difficulty=""3""/><Word Text=""improve"" Difficulty=""3""/><Word Text=""soldier"" Difficulty=""3""/><Word Text=""reflect"" Difficulty=""3""/><Word Text=""surface"" Difficulty=""3""/><Word Text=""purpose"" Difficulty=""3""/><Word Text=""pattern"" Difficulty=""3""/><Word Text=""machine"" Difficulty=""3""/><Word Text=""address"" Difficulty=""3""/><Word Text=""reality"" Difficulty=""3""/><Word Text=""partner"" Difficulty=""3""/><Word Text=""kitchen"" Difficulty=""3""/><Word Text=""capital"" Difficulty=""3""/><Word Text=""instead"" Difficulty=""3""/><Word Text=""account"" Difficulty=""3""/><Word Text=""western"" Difficulty=""3""/><Word Text=""prevent"" Difficulty=""3""/><Word Text=""citizen"" Difficulty=""3""/><Word Text=""mission"" Difficulty=""3""/><Word Text=""publish"" Difficulty=""3""/><Word Text=""release"" Difficulty=""3""/><Word Text=""opinion"" Difficulty=""3""/><Word Text=""version"" Difficulty=""3""/><Word Text=""species"" Difficulty=""3""/><Word Text=""freedom"" Difficulty=""3""/><Word Text=""achieve"" Difficulty=""3""/><Word Text=""concept"" Difficulty=""3""/><Word Text=""perfect"" Difficulty=""3""/><Word Text=""conduct"" Difficulty=""3""/><Word Text=""examine"" Difficulty=""3""/><Word Text=""variety"" Difficulty=""3""/><Word Text=""nuclear"" Difficulty=""3""/><Word Text=""replace"" Difficulty=""3""/><Word Text=""British"" Difficulty=""3""/><Word Text=""feature"" Difficulty=""3""/><Word Text=""weekend"" Difficulty=""3""/><Word Text=""African"" Difficulty=""3""/><Word Text=""through"" Difficulty=""3""/><Word Text=""element"" Difficulty=""3""/><Word Text=""Chinese"" Difficulty=""3""/><Word Text=""attempt"" Difficulty=""3""/><Word Text=""village"" Difficulty=""3""/><Word Text=""express"" Difficulty=""3""/><Word Text=""willing"" Difficulty=""3""/><Word Text=""deliver"" Difficulty=""3""/><Word Text=""vehicle"" Difficulty=""3""/><Word Text=""observe"" Difficulty=""3""/><Word Text=""average"" Difficulty=""3""/><Word Text=""operate"" Difficulty=""3""/><Word Text=""collect"" Difficulty=""3""/><Word Text=""promote"" Difficulty=""3""/><Word Text=""present"" Difficulty=""3""/><Word Text=""survive"" Difficulty=""3""/><Word Text=""failure"" Difficulty=""3""/><Word Text=""comment"" Difficulty=""3""/><Word Text=""regular"" Difficulty=""3""/><Word Text=""measure"" Difficulty=""3""/><Word Text=""anybody"" Difficulty=""3""/><Word Text=""quarter"" Difficulty=""3""/><Word Text=""growing"" Difficulty=""3""/><Word Text=""destroy"" Difficulty=""3""/><Word Text=""context"" Difficulty=""3""/><Word Text=""mistake"" Difficulty=""3""/><Word Text=""clothes"" Difficulty=""3""/><Word Text=""promise"" Difficulty=""3""/><Word Text=""average"" Difficulty=""3""/><Word Text=""combine"" Difficulty=""3""/><Word Text=""victory"" Difficulty=""3""/><Word Text=""healthy"" Difficulty=""3""/><Word Text=""finding"" Difficulty=""3""/><Word Text=""contact"" Difficulty=""3""/><Word Text=""justice"" Difficulty=""3""/><Word Text=""eastern"" Difficulty=""3""/><Word Text=""primary"" Difficulty=""3""/><Word Text=""plastic"" Difficulty=""3""/><Word Text=""writing"" Difficulty=""3""/><Word Text=""chicken"" Difficulty=""3""/><Word Text=""theater"" Difficulty=""3""/><Word Text=""session"" Difficulty=""3""/><Word Text=""welcome"" Difficulty=""3""/><Word Text=""respect"" Difficulty=""3""/><Word Text=""Russian"" Difficulty=""3""/><Word Text=""strange"" Difficulty=""3""/><Word Text=""reading"" Difficulty=""3""/><Word Text=""explore"" Difficulty=""3""/><Word Text=""complex"" Difficulty=""3""/><Word Text=""athlete"" Difficulty=""3""/><Word Text=""meaning"" Difficulty=""3""/><Word Text=""married"" Difficulty=""3""/><Word Text=""predict"" Difficulty=""3""/><Word Text=""weather"" Difficulty=""3""/><Word Text=""Supreme"" Difficulty=""3""/><Word Text=""balance"" Difficulty=""3""/><Word Text=""attempt"" Difficulty=""3""/><Word Text=""connect"" Difficulty=""3""/><Word Text=""somehow"" Difficulty=""3""/><Word Text=""analyst"" Difficulty=""3""/><Word Text=""largely"" Difficulty=""3""/><Word Text=""revenue"" Difficulty=""3""/><Word Text=""package"" Difficulty=""3""/><Word Text=""obvious"" Difficulty=""3""/><Word Text=""anymore"" Difficulty=""3""/><Word Text=""propose"" Difficulty=""3""/><Word Text=""visitor"" Difficulty=""3""/><Word Text=""hearing"" Difficulty=""3""/><Word Text=""traffic"" Difficulty=""3""/><Word Text=""capture"" Difficulty=""3""/><Word Text=""feature"" Difficulty=""3""/><Word Text=""content"" Difficulty=""3""/><Word Text=""declare"" Difficulty=""3""/><Word Text=""outside"" Difficulty=""3""/><Word Text=""setting"" Difficulty=""3""/><Word Text=""outcome"" Difficulty=""3""/><Word Text=""airport"" Difficulty=""3""/><Word Text=""English"" Difficulty=""3""/><Word Text=""neither"" Difficulty=""3""/><Word Text=""surgery"" Difficulty=""3""/><Word Text=""correct"" Difficulty=""3""/><Word Text=""address"" Difficulty=""3""/><Word Text=""ancient"" Difficulty=""3""/><Word Text=""silence"" Difficulty=""3""/><Word Text=""typical"" Difficulty=""3""/><Word Text=""confirm"" Difficulty=""3""/><Word Text=""attract"" Difficulty=""3""/><Word Text=""bedroom"" Difficulty=""3""/><Word Text=""English"" Difficulty=""3""/><Word Text=""account"" Difficulty=""3""/><Word Text=""totally"" Difficulty=""3""/><Word Text=""stretch"" Difficulty=""3""/><Word Text=""fashion"" Difficulty=""3""/><Word Text=""welfare"" Difficulty=""3""/><Word Text=""opening"" Difficulty=""3""/><Word Text=""overall"" Difficulty=""3""/><Word Text=""initial"" Difficulty=""3""/><Word Text=""careful"" Difficulty=""3""/><Word Text=""holiday"" Difficulty=""3""/><Word Text=""witness"" Difficulty=""3""/><Word Text=""beneath"" Difficulty=""3""/><Word Text=""limited"" Difficulty=""3""/><Word Text=""faculty"" Difficulty=""3""/><Word Text=""liberal"" Difficulty=""3""/><Word Text=""massive"" Difficulty=""3""/><Word Text=""decline"" Difficulty=""3""/><Word Text=""promise"" Difficulty=""3""/><Word Text=""towards"" Difficulty=""3""/><Word Text=""succeed"" Difficulty=""3""/><Word Text=""fishing"" Difficulty=""3""/><Word Text=""unusual"" Difficulty=""3""/><Word Text=""closely"" Difficulty=""3""/><Word Text=""approve"" Difficulty=""3""/><Word Text=""outside"" Difficulty=""3""/><Word Text=""acquire"" Difficulty=""3""/><Word Text=""compete"" Difficulty=""3""/><Word Text=""illegal"" Difficulty=""3""/><Word Text=""forever"" Difficulty=""3""/><Word Text=""Israeli"" Difficulty=""3""/><Word Text=""display"" Difficulty=""3""/><Word Text=""musical"" Difficulty=""3""/><Word Text=""suspect"" Difficulty=""3""/><Word Text=""scholar"" Difficulty=""3""/><Word Text=""warning"" Difficulty=""3""/><Word Text=""climate"" Difficulty=""3""/><Word Text=""payment"" Difficulty=""3""/><Word Text=""request"" Difficulty=""3""/><Word Text=""emotion"" Difficulty=""3""/><Word Text=""airline"" Difficulty=""3""/><Word Text=""library"" Difficulty=""3""/><Word Text=""recover"" Difficulty=""3""/><Word Text=""factory"" Difficulty=""3""/><Word Text=""expense"" Difficulty=""3""/><Word Text=""funding"" Difficulty=""3""/><Word Text=""therapy"" Difficulty=""3""/><Word Text=""housing"" Difficulty=""3""/><Word Text=""violent"" Difficulty=""3""/><Word Text=""suppose"" Difficulty=""3""/><Word Text=""wedding"" Difficulty=""3""/><Word Text=""portion"" Difficulty=""3""/><Word Text=""abandon"" Difficulty=""3""/><Word Text=""tension"" Difficulty=""3""/><Word Text=""display"" Difficulty=""3""/><Word Text=""leading"" Difficulty=""3""/><Word Text=""consist"" Difficulty=""3""/><Word Text=""alcohol"" Difficulty=""3""/><Word Text=""release"" Difficulty=""3""/><Word Text=""Spanish"" Difficulty=""3""/><Word Text=""passage"" Difficulty=""3""/><Word Text=""arrange"" Difficulty=""3""/><Word Text=""deserve"" Difficulty=""3""/><Word Text=""benefit"" Difficulty=""3""/><Word Text=""resolve"" Difficulty=""3""/><Word Text=""present"" Difficulty=""3""/><Word Text=""Mexican"" Difficulty=""3""/><Word Text=""symptom"" Difficulty=""3""/><Word Text=""contact"" Difficulty=""3""/><Word Text=""breathe"" Difficulty=""3""/><Word Text=""suicide"" Difficulty=""3""/><Word Text=""passion"" Difficulty=""3""/><Word Text=""amazing"" Difficulty=""3""/><Word Text=""intense"" Difficulty=""3""/><Word Text=""advance"" Difficulty=""3""/><Word Text=""inspire"" Difficulty=""3""/><Word Text=""visible"" Difficulty=""3""/><Word Text=""illness"" Difficulty=""3""/><Word Text=""analyze"" Difficulty=""3""/><Word Text=""another"" Difficulty=""3""/><Word Text=""parking"" Difficulty=""3""/><Word Text=""enhance"" Difficulty=""3""/><Word Text=""mystery"" Difficulty=""3""/><Word Text=""poverty"" Difficulty=""3""/><Word Text=""monitor"" Difficulty=""3""/><Word Text=""digital"" Difficulty=""3""/><Word Text=""heavily"" Difficulty=""3""/><Word Text=""missile"" Difficulty=""3""/><Word Text=""equally"" Difficulty=""3""/><Word Text=""command"" Difficulty=""3""/><Word Text=""veteran"" Difficulty=""3""/><Word Text=""capable"" Difficulty=""3""/><Word Text=""nervous"" Difficulty=""3""/><Word Text=""tourist"" Difficulty=""3""/><Word Text=""crucial"" Difficulty=""3""/><Word Text=""deficit"" Difficulty=""3""/><Word Text=""journey"" Difficulty=""3""/><Word Text=""mixture"" Difficulty=""3""/><Word Text=""whisper"" Difficulty=""3""/><Word Text=""anxiety"" Difficulty=""3""/><Word Text=""embrace"" Difficulty=""3""/><Word Text=""testing"" Difficulty=""3""/><Word Text=""stomach"" Difficulty=""3""/><Word Text=""install"" Difficulty=""3""/><Word Text=""concert"" Difficulty=""3""/><Word Text=""channel"" Difficulty=""3""/><Word Text=""extreme"" Difficulty=""3""/><Word Text=""drawing"" Difficulty=""3""/><Word Text=""protein"" Difficulty=""3""/><Word Text=""absence"" Difficulty=""3""/><Word Text=""rapidly"" Difficulty=""3""/><Word Text=""comment"" Difficulty=""3""/><Word Text=""speaker"" Difficulty=""3""/><Word Text=""restore"" Difficulty=""3""/><Word Text=""quietly"" Difficulty=""3""/><Word Text=""general"" Difficulty=""3""/><Word Text=""utility"" Difficulty=""3""/><Word Text=""highway"" Difficulty=""3""/><Word Text=""routine"" Difficulty=""3""/><Word Text=""Islamic"" Difficulty=""3""/><Word Text=""refugee"" Difficulty=""3""/><Word Text=""barrier"" Difficulty=""3""/><Word Text=""classic"" Difficulty=""3""/><Word Text=""distant"" Difficulty=""3""/><Word Text=""Italian"" Difficulty=""3""/><Word Text=""ceiling"" Difficulty=""3""/><Word Text=""roughly"" Difficulty=""3""/><Word Text=""lawsuit"" Difficulty=""3""/><Word Text=""chamber"" Difficulty=""3""/><Word Text=""profile"" Difficulty=""3""/><Word Text=""penalty"" Difficulty=""3""/><Word Text=""advance"" Difficulty=""3""/><Word Text=""cabinet"" Difficulty=""3""/><Word Text=""proceed"" Difficulty=""3""/><Word Text=""dispute"" Difficulty=""3""/><Word Text=""fortune"" Difficulty=""3""/><Word Text=""genetic"" Difficulty=""3""/><Word Text=""adviser"" Difficulty=""3""/><Word Text=""whereas"" Difficulty=""3""/><Word Text=""Olympic"" Difficulty=""3""/><Word Text=""decline"" Difficulty=""3""/><Word Text=""process"" Difficulty=""3""/><Word Text=""fiction"" Difficulty=""3""/><Word Text=""balance"" Difficulty=""3""/><Word Text=""senator"" Difficulty=""3""/><Word Text=""hunting"" Difficulty=""3""/><Word Text=""journal"" Difficulty=""3""/><Word Text=""general"" Difficulty=""3""/><Word Text=""testify"" Difficulty=""3""/><Word Text=""founder"" Difficulty=""3""/><Word Text=""dismiss"" Difficulty=""3""/><Word Text=""finance"" Difficulty=""3""/><Word Text=""respect"" Difficulty=""3""/><Word Text=""diverse"" Difficulty=""3""/><Word Text=""working"" Difficulty=""3""/><Word Text=""unknown"" Difficulty=""3""/><Word Text=""offense"" Difficulty=""3""/><Word Text=""counter"" Difficulty=""3""/><Word Text=""justify"" Difficulty=""3""/><Word Text=""protest"" Difficulty=""3""/><Word Text=""insight"" Difficulty=""3""/><Word Text=""possess"" Difficulty=""3""/><Word Text=""episode"" Difficulty=""3""/><Word Text=""shortly"" Difficulty=""3""/><Word Text=""assault"" Difficulty=""3""/><Word Text=""license"" Difficulty=""3""/><Word Text=""shelter"" Difficulty=""3""/><Word Text=""tragedy"" Difficulty=""3""/><Word Text=""funeral"" Difficulty=""3""/><Word Text=""squeeze"" Difficulty=""3""/><Word Text=""convert"" Difficulty=""3""/><Word Text=""pretend"" Difficulty=""3""/><Word Text=""elderly"" Difficulty=""3""/><Word Text=""violate"" Difficulty=""3""/><Word Text=""neither"" Difficulty=""3""/><Word Text=""segment"" Difficulty=""3""/><Word Text=""nowhere"" Difficulty=""3""/><Word Text=""comfort"" Difficulty=""3""/><Word Text=""radical"" Difficulty=""3""/><Word Text=""storage"" Difficulty=""3""/><Word Text=""leather"" Difficulty=""3""/><Word Text=""council"" Difficulty=""3""/><Word Text=""fantasy"" Difficulty=""3""/><Word Text=""gesture"" Difficulty=""3""/><Word Text=""ongoing"" Difficulty=""3""/><Word Text=""witness"" Difficulty=""3""/><Word Text=""chapter"" Difficulty=""3""/><Word Text=""divorce"" Difficulty=""3""/><Word Text=""sustain"" Difficulty=""3""/><Word Text=""fifteen"" Difficulty=""3""/><Word Text=""satisfy"" Difficulty=""3""/><Word Text=""briefly"" Difficulty=""3""/><Word Text=""consume"" Difficulty=""3""/><Word Text=""tobacco"" Difficulty=""3""/><Word Text=""besides"" Difficulty=""3""/><Word Text=""wealthy"" Difficulty=""3""/><Word Text=""fighter"" Difficulty=""3""/><Word Text=""educate"" Difficulty=""3""/><Word Text=""painful"" Difficulty=""3""/><Word Text=""uniform"" Difficulty=""3""/><Word Text=""qualify"" Difficulty=""3""/><Word Text=""scandal"" Difficulty=""3""/><Word Text=""helpful"" Difficulty=""3""/><Word Text=""impress"" Difficulty=""3""/><Word Text=""privacy"" Difficulty=""3""/><Word Text=""contest"" Difficulty=""3""/><Word Text=""organic"" Difficulty=""3""/><Word Text=""bombing"" Difficulty=""3""/><Word Text=""suspect"" Difficulty=""3""/><Word Text=""explode"" Difficulty=""3""/><Word Text=""handful"" Difficulty=""3""/><Word Text=""horizon"" Difficulty=""3""/><Word Text=""curious"" Difficulty=""3""/><Word Text=""request"" Difficulty=""3""/><Word Text=""undergo"" Difficulty=""3""/><Word Text=""edition"" Difficulty=""3""/><Word Text=""complex"" Difficulty=""3""/><Word Text=""appoint"" Difficulty=""3""/><Word Text=""battery"" Difficulty=""3""/><Word Text=""arrival"" Difficulty=""3""/><Word Text=""cluster"" Difficulty=""3""/><Word Text=""habitat"" Difficulty=""3""/><Word Text=""actress"" Difficulty=""3""/><Word Text=""running"" Difficulty=""3""/><Word Text=""correct"" Difficulty=""3""/><Word Text=""worried"" Difficulty=""3""/><Word Text=""portray"" Difficulty=""3""/><Word Text=""carrier"" Difficulty=""3""/><Word Text=""cooking"" Difficulty=""3""/><Word Text=""miracle"" Difficulty=""3""/><Word Text=""killing"" Difficulty=""3""/><Word Text=""charity"" Difficulty=""3""/><Word Text=""venture"" Difficulty=""3""/><Word Text=""grocery"" Difficulty=""3""/><Word Text=""exhibit"" Difficulty=""3""/><Word Text=""blanket"" Difficulty=""3""/><Word Text=""recruit"" Difficulty=""3""/><Word Text=""painter"" Difficulty=""3""/><Word Text=""courage"" Difficulty=""3""/><Word Text=""formula"" Difficulty=""3""/><Word Text=""captain"" Difficulty=""3""/><Word Text=""gallery"" Difficulty=""3""/><Word Text=""fitness"" Difficulty=""3""/><Word Text=""inquiry"" Difficulty=""3""/><Word Text=""compose"" Difficulty=""3""/><Word Text=""related"" Difficulty=""3""/><Word Text=""lightly"" Difficulty=""3""/><Word Text=""trading"" Difficulty=""3""/><Word Text=""concern"" Difficulty=""3""/><Word Text=""surgeon"" Difficulty=""3""/><Word Text=""physics"" Difficulty=""3""/><Word Text=""counsel"" Difficulty=""3""/><Word Text=""excited"" Difficulty=""3""/><Word Text=""serving"" Difficulty=""3""/><Word Text=""greatly"" Difficulty=""3""/><Word Text=""finance"" Difficulty=""3""/><Word Text=""pleased"" Difficulty=""3""/><Word Text=""sponsor"" Difficulty=""3""/><Word Text=""ethical"" Difficulty=""3""/><Word Text=""entitle"" Difficulty=""3""/><Word Text=""evident"" Difficulty=""3""/><Word Text=""essence"" Difficulty=""3""/><Word Text=""exclude"" Difficulty=""3""/><Word Text=""pitcher"" Difficulty=""3""/><Word Text=""T-shirt"" Difficulty=""3""/><Word Text=""patient"" Difficulty=""3""/><Word Text=""reverse"" Difficulty=""3""/><Word Text=""missing"" Difficulty=""3""/><Word Text=""stretch"" Difficulty=""3""/><Word Text=""confuse"" Difficulty=""3""/><Word Text=""monthly"" Difficulty=""3""/><Word Text=""lecture"" Difficulty=""3""/><Word Text=""swallow"" Difficulty=""3""/><Word Text=""enforce"" Difficulty=""3""/><Word Text=""contend"" Difficulty=""3""/><Word Text=""frankly"" Difficulty=""3""/><Word Text=""hallway"" Difficulty=""3""/><Word Text=""monster"" Difficulty=""3""/><Word Text=""protest"" Difficulty=""3""/><Word Text=""crystal"" Difficulty=""3""/><Word Text=""written"" Difficulty=""3""/><Word Text=""consult"" Difficulty=""3""/><Word Text=""forgive"" Difficulty=""3""/><Word Text=""project"" Difficulty=""3""/><Word Text=""maximum"" Difficulty=""3""/><Word Text=""warrior"" Difficulty=""3""/><Word Text=""outdoor"" Difficulty=""3""/><Word Text=""curtain"" Difficulty=""3""/><Word Text=""monitor"" Difficulty=""3""/><Word Text=""subject"" Difficulty=""3""/><Word Text=""walking"" Difficulty=""3""/><Word Text=""playoff"" Difficulty=""3""/><Word Text=""minimum"" Difficulty=""3""/><Word Text=""execute"" Difficulty=""3""/><Word Text=""average"" Difficulty=""3""/><Word Text=""welcome"" Difficulty=""3""/><Word Text=""chronic"" Difficulty=""3""/><Word Text=""retired"" Difficulty=""3""/><Word Text=""trigger"" Difficulty=""3""/><Word Text=""virtual"" Difficulty=""3""/><Word Text=""convict"" Difficulty=""3""/><Word Text=""landing"" Difficulty=""3""/><Word Text=""conduct"" Difficulty=""3""/><Word Text=""driving"" Difficulty=""3""/><Word Text=""vitamin"" Difficulty=""3""/><Word Text=""endless"" Difficulty=""3""/><Word Text=""mandate"" Difficulty=""3""/><Word Text=""reserve"" Difficulty=""3""/><Word Text=""genuine"" Difficulty=""3""/><Word Text=""scatter"" Difficulty=""3""/><Word Text=""relieve"" Difficulty=""3""/><Word Text=""suspend"" Difficulty=""3""/><Word Text=""pension"" Difficulty=""3""/><Word Text=""rebuild"" Difficulty=""3""/><Word Text=""shuttle"" Difficulty=""3""/><Word Text=""exhibit"" Difficulty=""3""/><Word Text=""precise"" Difficulty=""3""/><Word Text=""anxious"" Difficulty=""3""/><Word Text=""liberty"" Difficulty=""3""/><Word Text=""primary"" Difficulty=""3""/><Word Text=""doorway"" Difficulty=""3""/><Word Text=""teenage"" Difficulty=""3""/><Word Text=""pursuit"" Difficulty=""3""/><Word Text=""Israeli"" Difficulty=""3""/><Word Text=""endorse"" Difficulty=""3""/><Word Text=""thereby"" Difficulty=""3""/><Word Text=""overall"" Difficulty=""3""/><Word Text=""program"" Difficulty=""3""/><Word Text=""picture"" Difficulty=""3""/><Word Text=""sharply"" Difficulty=""3""/><Word Text=""garbage"" Difficulty=""3""/><Word Text=""servant"" Difficulty=""3""/><Word Text=""elegant"" Difficulty=""3""/><Word Text=""confess"" Difficulty=""3""/><Word Text=""starter"" Difficulty=""3""/><Word Text=""banking"" Difficulty=""3""/><Word Text=""gravity"" Difficulty=""3""/><Word Text=""isolate"" Difficulty=""3""/><Word Text=""hostage"" Difficulty=""3""/><Word Text=""dynamic"" Difficulty=""3""/><Word Text=""content"" Difficulty=""3""/><Word Text=""Russian"" Difficulty=""3""/><Word Text=""command"" Difficulty=""3""/><Word Text=""stumble"" Difficulty=""3""/><Word Text=""descend"" Difficulty=""3""/><Word Text=""readily"" Difficulty=""3""/><Word Text=""romance"" Difficulty=""3""/><Word Text=""circuit"" Difficulty=""3""/><Word Text=""coastal"" Difficulty=""3""/><Word Text=""reserve"" Difficulty=""3""/><Word Text=""burning"" Difficulty=""3""/><Word Text=""diamond"" Difficulty=""3""/><Word Text=""oversee"" Difficulty=""3""/><Word Text=""trailer"" Difficulty=""3""/><Word Text=""o'clock"" Difficulty=""3""/><Word Text=""loyalty"" Difficulty=""3""/><Word Text=""nominee"" Difficulty=""3""/><Word Text=""alleged"" Difficulty=""3""/><Word Text=""dignity"" Difficulty=""3""/><Word Text=""seventh"" Difficulty=""3""/><Word Text=""tightly"" Difficulty=""3""/><Word Text=""dilemma"" Difficulty=""3""/><Word Text=""shallow"" Difficulty=""3""/><Word Text=""stadium"" Difficulty=""3""/><Word Text=""condemn"" Difficulty=""3""/><Word Text=""costume"" Difficulty=""3""/><Word Text=""statute"" Difficulty=""3""/><Word Text=""cartoon"" Difficulty=""3""/><Word Text=""besides"" Difficulty=""3""/><Word Text=""hostile"" Difficulty=""3""/><Word Text=""vaccine"" Difficulty=""3""/><Word Text=""opposed"" Difficulty=""3""/><Word Text=""jewelry"" Difficulty=""3""/><Word Text=""concede"" Difficulty=""3""/><Word Text=""secular"" Difficulty=""3""/><Word Text=""divorce"" Difficulty=""3""/><Word Text=""neutral"" Difficulty=""3""/><Word Text=""biology"" Difficulty=""3""/><Word Text=""whoever"" Difficulty=""3""/><Word Text=""verdict"" Difficulty=""3""/><Word Text=""subsidy"" Difficulty=""3""/><Word Text=""respect"" Difficulty=""3""/><Word Text=""dessert"" Difficulty=""3""/><Word Text=""utilize"" Difficulty=""3""/><Word Text=""rolling"" Difficulty=""3""/><Word Text=""minimal"" Difficulty=""3""/><Word Text=""cocaine"" Difficulty=""3""/><Word Text=""sibling"" Difficulty=""3""/><Word Text=""passing"" Difficulty=""3""/><Word Text=""persist"" Difficulty=""3""/><Word Text=""bicycle"" Difficulty=""3""/><Word Text=""exploit"" Difficulty=""3""/><Word Text=""minimum"" Difficulty=""3""/><Word Text=""charter"" Difficulty=""3""/><Word Text=""consent"" Difficulty=""3""/><Word Text=""workout"" Difficulty=""3""/><Word Text=""hormone"" Difficulty=""3""/><Word Text=""texture"" Difficulty=""3""/><Word Text=""counter"" Difficulty=""3""/><Word Text=""custody"" Difficulty=""3""/><Word Text=""outline"" Difficulty=""3""/><Word Text=""uncover"" Difficulty=""3""/><Word Text=""catalog"" Difficulty=""3""/><Word Text=""someday"" Difficulty=""3""/><Word Text=""instant"" Difficulty=""3""/><Word Text=""trainer"" Difficulty=""3""/><Word Text=""eyebrow"" Difficulty=""3""/><Word Text=""inherit"" Difficulty=""3""/><Word Text=""pioneer"" Difficulty=""3""/><Word Text=""kingdom"" Difficulty=""3""/><Word Text=""terrain"" Difficulty=""3""/><Word Text=""planner"" Difficulty=""3""/><Word Text=""closest"" Difficulty=""3""/><Word Text=""density"" Difficulty=""3""/><Word Text=""Persian"" Difficulty=""3""/><Word Text=""feather"" Difficulty=""3""/><Word Text=""tighten"" Difficulty=""3""/><Word Text=""partial"" Difficulty=""3""/><Word Text=""builder"" Difficulty=""3""/><Word Text=""glimpse"" Difficulty=""3""/><Word Text=""premise"" Difficulty=""3""/><Word Text=""legally"" Difficulty=""3""/><Word Text=""disturb"" Difficulty=""3""/><Word Text=""logical"" Difficulty=""3""/><Word Text=""liberal"" Difficulty=""3""/><Word Text=""slavery"" Difficulty=""3""/><Word Text=""mineral"" Difficulty=""3""/><Word Text=""halfway"" Difficulty=""3""/><Word Text=""fucking"" Difficulty=""3""/><Word Text=""sponsor"" Difficulty=""3""/><Word Text=""auction"" Difficulty=""3""/><Word Text=""triumph"" Difficulty=""3""/><Word Text=""scratch"" Difficulty=""3""/><Word Text=""harmony"" Difficulty=""3""/><Word Text=""instant"" Difficulty=""3""/><Word Text=""running"" Difficulty=""3""/><Word Text=""peasant"" Difficulty=""3""/><Word Text=""deposit"" Difficulty=""3""/><Word Text=""impulse"" Difficulty=""3""/><Word Text=""trouble"" Difficulty=""3""/><Word Text=""dancing"" Difficulty=""3""/><Word Text=""happily"" Difficulty=""3""/><Word Text=""removal"" Difficulty=""3""/><Word Text=""unhappy"" Difficulty=""3""/><Word Text=""tourism"" Difficulty=""3""/><Word Text=""exhaust"" Difficulty=""3""/><Word Text=""fragile"" Difficulty=""3""/><Word Text=""crowded"" Difficulty=""3""/><Word Text=""prevail"" Difficulty=""3""/><Word Text=""mention"" Difficulty=""3""/><Word Text=""mansion"" Difficulty=""3""/><Word Text=""cottage"" Difficulty=""3""/><Word Text=""balloon"" Difficulty=""3""/><Word Text=""sweater"" Difficulty=""3""/><Word Text=""retreat"" Difficulty=""3""/><Word Text=""veteran"" Difficulty=""3""/><Word Text=""premium"" Difficulty=""3""/><Word Text=""fatigue"" Difficulty=""3""/><Word Text=""provoke"" Difficulty=""3""/><Word Text=""harvest"" Difficulty=""3""/><Word Text=""specify"" Difficulty=""3""/><Word Text=""transit"" Difficulty=""3""/><Word Text=""seminar"" Difficulty=""3""/><Word Text=""delight"" Difficulty=""3""/><Word Text=""skilled"" Difficulty=""3""/><Word Text=""summary"" Difficulty=""3""/><Word Text=""harvest"" Difficulty=""3""/><Word Text=""dictate"" Difficulty=""3""/><Word Text=""laundry"" Difficulty=""3""/><Word Text=""apology"" Difficulty=""3""/><Word Text=""American"" Difficulty=""3""/><Word Text=""question"" Difficulty=""3""/><Word Text=""national"" Difficulty=""3""/><Word Text=""business"" Difficulty=""3""/><Word Text=""continue"" Difficulty=""3""/><Word Text=""together"" Difficulty=""3""/><Word Text=""anything"" Difficulty=""3""/><Word Text=""research"" Difficulty=""3""/><Word Text=""although"" Difficulty=""3""/><Word Text=""remember"" Difficulty=""3""/><Word Text=""consider"" Difficulty=""3""/><Word Text=""actually"" Difficulty=""3""/><Word Text=""probably"" Difficulty=""3""/><Word Text=""interest"" Difficulty=""3""/><Word Text=""economic"" Difficulty=""3""/><Word Text=""possible"" Difficulty=""3""/><Word Text=""military"" Difficulty=""3""/><Word Text=""decision"" Difficulty=""3""/><Word Text=""building"" Difficulty=""3""/><Word Text=""director"" Difficulty=""3""/><Word Text=""position"" Difficulty=""3""/><Word Text=""official"" Difficulty=""3""/><Word Text=""everyone"" Difficulty=""3""/><Word Text=""activity"" Difficulty=""3""/><Word Text=""American"" Difficulty=""3""/><Word Text=""industry"" Difficulty=""3""/><Word Text=""practice"" Difficulty=""3""/><Word Text=""describe"" Difficulty=""3""/><Word Text=""personal"" Difficulty=""3""/><Word Text=""computer"" Difficulty=""3""/><Word Text=""evidence"" Difficulty=""3""/><Word Text=""daughter"" Difficulty=""3""/><Word Text=""Congress"" Difficulty=""3""/><Word Text=""campaign"" Difficulty=""3""/><Word Text=""material"" Difficulty=""3""/><Word Text=""hospital"" Difficulty=""3""/><Word Text=""thousand"" Difficulty=""3""/><Word Text=""increase"" Difficulty=""3""/><Word Text=""security"" Difficulty=""3""/><Word Text=""behavior"" Difficulty=""3""/><Word Text=""recently"" Difficulty=""3""/><Word Text=""movement"" Difficulty=""3""/><Word Text=""language"" Difficulty=""3""/><Word Text=""response"" Difficulty=""3""/><Word Text=""approach"" Difficulty=""3""/><Word Text=""pressure"" Difficulty=""3""/><Word Text=""resource"" Difficulty=""3""/><Word Text=""identify"" Difficulty=""3""/><Word Text=""whatever"" Difficulty=""3""/><Word Text=""indicate"" Difficulty=""3""/><Word Text=""training"" Difficulty=""3""/><Word Text=""election"" Difficulty=""3""/><Word Text=""physical"" Difficulty=""3""/><Word Text=""standard"" Difficulty=""3""/><Word Text=""analysis"" Difficulty=""3""/><Word Text=""strategy"" Difficulty=""3""/><Word Text=""Democrat"" Difficulty=""3""/><Word Text=""yourself"" Difficulty=""3""/><Word Text=""maintain"" Difficulty=""3""/><Word Text=""employee"" Difficulty=""3""/><Word Text=""cultural"" Difficulty=""3""/><Word Text=""politics"" Difficulty=""3""/><Word Text=""suddenly"" Difficulty=""3""/><Word Text=""discover"" Difficulty=""3""/><Word Text=""specific"" Difficulty=""3""/><Word Text=""shoulder"" Difficulty=""3""/><Word Text=""property"" Difficulty=""3""/><Word Text=""somebody"" Difficulty=""3""/><Word Text=""magazine"" Difficulty=""3""/><Word Text=""marriage"" Difficulty=""3""/><Word Text=""violence"" Difficulty=""4""/><Word Text=""positive"" Difficulty=""4""/><Word Text=""consumer"" Difficulty=""4""/><Word Text=""painting"" Difficulty=""4""/><Word Text=""attorney"" Difficulty=""4""/><Word Text=""audience"" Difficulty=""4""/><Word Text=""majority"" Difficulty=""4""/><Word Text=""customer"" Difficulty=""4""/><Word Text=""southern"" Difficulty=""4""/><Word Text=""relation"" Difficulty=""4""/><Word Text=""critical"" Difficulty=""4""/><Word Text=""original"" Difficulty=""4""/><Word Text=""directly"" Difficulty=""4""/><Word Text=""attitude"" Difficulty=""4""/><Word Text=""powerful"" Difficulty=""4""/><Word Text=""announce"" Difficulty=""4""/><Word Text=""involved"" Difficulty=""4""/><Word Text=""conflict"" Difficulty=""4""/><Word Text=""argument"" Difficulty=""4""/><Word Text=""complete"" Difficulty=""4""/><Word Text=""solution"" Difficulty=""4""/><Word Text=""distance"" Difficulty=""4""/><Word Text=""mountain"" Difficulty=""4""/><Word Text=""supposed"" Difficulty=""4""/><Word Text=""resident"" Difficulty=""4""/><Word Text=""increase"" Difficulty=""4""/><Word Text=""European"" Difficulty=""4""/><Word Text=""presence"" Difficulty=""4""/><Word Text=""district"" Difficulty=""4""/><Word Text=""contract"" Difficulty=""4""/><Word Text=""strength"" Difficulty=""4""/><Word Text=""previous"" Difficulty=""4""/><Word Text=""reporter"" Difficulty=""4""/><Word Text=""facility"" Difficulty=""4""/><Word Text=""identity"" Difficulty=""4""/><Word Text=""tomorrow"" Difficulty=""4""/><Word Text=""approach"" Difficulty=""4""/><Word Text=""chairman"" Difficulty=""4""/><Word Text=""baseball"" Difficulty=""4""/><Word Text=""religion"" Difficulty=""4""/><Word Text=""document"" Difficulty=""4""/><Word Text=""threaten"" Difficulty=""4""/><Word Text=""slightly"" Difficulty=""4""/><Word Text=""reaction"" Difficulty=""4""/><Word Text=""location"" Difficulty=""4""/><Word Text=""neighbor"" Difficulty=""4""/><Word Text=""complete"" Difficulty=""4""/><Word Text=""function"" Difficulty=""4""/><Word Text=""learning"" Difficulty=""4""/><Word Text=""category"" Difficulty=""4""/><Word Text=""academic"" Difficulty=""4""/><Word Text=""Internet"" Difficulty=""4""/><Word Text=""negative"" Difficulty=""4""/><Word Text=""medicine"" Difficulty=""4""/><Word Text=""exercise"" Difficulty=""4""/><Word Text=""familiar"" Difficulty=""4""/><Word Text=""progress"" Difficulty=""4""/><Word Text=""exchange"" Difficulty=""4""/><Word Text=""football"" Difficulty=""4""/><Word Text=""domestic"" Difficulty=""4""/><Word Text=""northern"" Difficulty=""4""/><Word Text=""software"" Difficulty=""4""/><Word Text=""favorite"" Difficulty=""4""/><Word Text=""greatest"" Difficulty=""4""/><Word Text=""surround"" Difficulty=""4""/><Word Text=""surprise"" Difficulty=""4""/><Word Text=""proposal"" Difficulty=""4""/><Word Text=""minority"" Difficulty=""4""/><Word Text=""straight"" Difficulty=""4""/><Word Text=""teaching"" Difficulty=""4""/><Word Text=""regional"" Difficulty=""4""/><Word Text=""organize"" Difficulty=""4""/><Word Text=""struggle"" Difficulty=""4""/><Word Text=""conclude"" Difficulty=""4""/><Word Text=""generate"" Difficulty=""4""/><Word Text=""thinking"" Difficulty=""4""/><Word Text=""possibly"" Difficulty=""4""/><Word Text=""investor"" Difficulty=""4""/><Word Text=""accident"" Difficulty=""4""/><Word Text=""Japanese"" Difficulty=""4""/><Word Text=""internal"" Difficulty=""4""/><Word Text=""Catholic"" Difficulty=""4""/><Word Text=""contrast"" Difficulty=""4""/><Word Text=""standard"" Difficulty=""4""/><Word Text=""capacity"" Difficulty=""4""/><Word Text=""estimate"" Difficulty=""4""/><Word Text=""governor"" Difficulty=""4""/><Word Text=""official"" Difficulty=""4""/><Word Text=""producer"" Difficulty=""4""/><Word Text=""division"" Difficulty=""4""/><Word Text=""entirely"" Difficulty=""4""/><Word Text=""complain"" Difficulty=""4""/><Word Text=""variable"" Difficulty=""4""/><Word Text=""coverage"" Difficulty=""4""/><Word Text=""anywhere"" Difficulty=""4""/><Word Text=""pleasure"" Difficulty=""4""/><Word Text=""separate"" Difficulty=""4""/><Word Text=""struggle"" Difficulty=""4""/><Word Text=""somewhat"" Difficulty=""4""/><Word Text=""judgment"" Difficulty=""4""/><Word Text=""minister"" Difficulty=""4""/><Word Text=""separate"" Difficulty=""4""/><Word Text=""terrible"" Difficulty=""4""/><Word Text=""multiple"" Difficulty=""4""/><Word Text=""question"" Difficulty=""4""/><Word Text=""criminal"" Difficulty=""4""/><Word Text=""abortion"" Difficulty=""4""/><Word Text=""incident"" Difficulty=""4""/><Word Text=""enormous"" Difficulty=""4""/><Word Text=""engineer"" Difficulty=""4""/><Word Text=""sentence"" Difficulty=""4""/><Word Text=""convince"" Difficulty=""4""/><Word Text=""addition"" Difficulty=""4""/><Word Text=""creative"" Difficulty=""4""/><Word Text=""priority"" Difficulty=""4""/><Word Text=""creation"" Difficulty=""4""/><Word Text=""graduate"" Difficulty=""4""/><Word Text=""dramatic"" Difficulty=""4""/><Word Text=""universe"" Difficulty=""4""/><Word Text=""schedule"" Difficulty=""4""/><Word Text=""purchase"" Difficulty=""4""/><Word Text=""existing"" Difficulty=""4""/><Word Text=""perceive"" Difficulty=""4""/><Word Text=""planning"" Difficulty=""4""/><Word Text=""opponent"" Difficulty=""4""/><Word Text=""preserve"" Difficulty=""4""/><Word Text=""opposite"" Difficulty=""4""/><Word Text=""exposure"" Difficulty=""4""/><Word Text=""occasion"" Difficulty=""4""/><Word Text=""ordinary"" Difficulty=""4""/><Word Text=""numerous"" Difficulty=""4""/><Word Text=""moreover"" Difficulty=""4""/><Word Text=""employer"" Difficulty=""4""/><Word Text=""dominate"" Difficulty=""4""/><Word Text=""whenever"" Difficulty=""4""/><Word Text=""transfer"" Difficulty=""4""/><Word Text=""disaster"" Difficulty=""4""/><Word Text=""prospect"" Difficulty=""4""/><Word Text=""exercise"" Difficulty=""4""/><Word Text=""spending"" Difficulty=""4""/><Word Text=""evaluate"" Difficulty=""4""/><Word Text=""emphasis"" Difficulty=""4""/><Word Text=""creature"" Difficulty=""4""/><Word Text=""disorder"" Difficulty=""4""/><Word Text=""strongly"" Difficulty=""4""/><Word Text=""constant"" Difficulty=""4""/><Word Text=""bathroom"" Difficulty=""4""/><Word Text=""confront"" Difficulty=""4""/><Word Text=""prisoner"" Difficulty=""4""/><Word Text=""designer"" Difficulty=""4""/><Word Text=""educator"" Difficulty=""4""/><Word Text=""relative"" Difficulty=""4""/><Word Text=""teaspoon"" Difficulty=""4""/><Word Text=""birthday"" Difficulty=""4""/><Word Text=""teenager"" Difficulty=""4""/><Word Text=""recovery"" Difficulty=""4""/><Word Text=""observer"" Difficulty=""4""/><Word Text=""straight"" Difficulty=""4""/><Word Text=""estimate"" Difficulty=""4""/><Word Text=""historic"" Difficulty=""4""/><Word Text=""apparent"" Difficulty=""4""/><Word Text=""approval"" Difficulty=""4""/><Word Text=""criteria"" Difficulty=""4""/><Word Text=""clinical"" Difficulty=""4""/><Word Text=""schedule"" Difficulty=""4""/><Word Text=""normally"" Difficulty=""4""/><Word Text=""activist"" Difficulty=""4""/><Word Text=""ultimate"" Difficulty=""4""/><Word Text=""valuable"" Difficulty=""4""/><Word Text=""graduate"" Difficulty=""4""/><Word Text=""chemical"" Difficulty=""4""/><Word Text=""vacation"" Difficulty=""4""/><Word Text=""advocate"" Difficulty=""4""/><Word Text=""pregnant"" Difficulty=""4""/><Word Text=""Canadian"" Difficulty=""4""/><Word Text=""darkness"" Difficulty=""4""/><Word Text=""clothing"" Difficulty=""4""/><Word Text=""portrait"" Difficulty=""4""/><Word Text=""survival"" Difficulty=""4""/><Word Text=""ceremony"" Difficulty=""4""/><Word Text=""disagree"" Difficulty=""4""/><Word Text=""unlikely"" Difficulty=""4""/><Word Text=""stranger"" Difficulty=""4""/><Word Text=""electric"" Difficulty=""4""/><Word Text=""literary"" Difficulty=""4""/><Word Text=""overcome"" Difficulty=""4""/><Word Text=""shopping"" Difficulty=""4""/><Word Text=""chemical"" Difficulty=""4""/><Word Text=""accurate"" Difficulty=""4""/><Word Text=""champion"" Difficulty=""4""/><Word Text=""scenario"" Difficulty=""4""/><Word Text=""friendly"" Difficulty=""4""/><Word Text=""lifetime"" Difficulty=""4""/><Word Text=""innocent"" Difficulty=""4""/><Word Text=""boundary"" Difficulty=""4""/><Word Text=""withdraw"" Difficulty=""4""/><Word Text=""dialogue"" Difficulty=""4""/><Word Text=""advanced"" Difficulty=""4""/><Word Text=""aircraft"" Difficulty=""4""/><Word Text=""delivery"" Difficulty=""4""/><Word Text=""platform"" Difficulty=""4""/><Word Text=""relevant"" Difficulty=""4""/><Word Text=""shooting"" Difficulty=""4""/><Word Text=""transfer"" Difficulty=""4""/><Word Text=""external"" Difficulty=""4""/><Word Text=""entrance"" Difficulty=""4""/><Word Text=""favorite"" Difficulty=""4""/><Word Text=""practice"" Difficulty=""4""/><Word Text=""properly"" Difficulty=""4""/><Word Text=""emission"" Difficulty=""4""/><Word Text=""earnings"" Difficulty=""4""/><Word Text=""exciting"" Difficulty=""4""/><Word Text=""musician"" Difficulty=""4""/><Word Text=""instance"" Difficulty=""4""/><Word Text=""athletic"" Difficulty=""4""/><Word Text=""survivor"" Difficulty=""4""/><Word Text=""publicly"" Difficulty=""4""/><Word Text=""tendency"" Difficulty=""4""/><Word Text=""resemble"" Difficulty=""4""/><Word Text=""surprise"" Difficulty=""4""/><Word Text=""proposed"" Difficulty=""4""/><Word Text=""standing"" Difficulty=""4""/><Word Text=""purchase"" Difficulty=""4""/><Word Text=""Catholic"" Difficulty=""4""/><Word Text=""provider"" Difficulty=""4""/><Word Text=""downtown"" Difficulty=""4""/><Word Text=""taxpayer"" Difficulty=""4""/><Word Text=""detailed"" Difficulty=""4""/><Word Text=""workshop"" Difficulty=""4""/><Word Text=""romantic"" Difficulty=""4""/><Word Text=""overlook"" Difficulty=""4""/><Word Text=""sequence"" Difficulty=""4""/><Word Text=""relative"" Difficulty=""4""/><Word Text=""absolute"" Difficulty=""4""/><Word Text=""register"" Difficulty=""4""/><Word Text=""heritage"" Difficulty=""4""/><Word Text=""dominant"" Difficulty=""4""/><Word Text=""operator"" Difficulty=""4""/><Word Text=""collapse"" Difficulty=""4""/><Word Text=""mortgage"" Difficulty=""4""/><Word Text=""sanction"" Difficulty=""4""/><Word Text=""civilian"" Difficulty=""4""/><Word Text=""province"" Difficulty=""4""/><Word Text=""function"" Difficulty=""4""/><Word Text=""distinct"" Difficulty=""4""/><Word Text=""artistic"" Difficulty=""4""/><Word Text=""fighting"" Difficulty=""4""/><Word Text=""persuade"" Difficulty=""4""/><Word Text=""moderate"" Difficulty=""4""/><Word Text=""frequent"" Difficulty=""4""/><Word Text=""everyday"" Difficulty=""4""/><Word Text=""headline"" Difficulty=""4""/><Word Text=""invasion"" Difficulty=""4""/><Word Text=""military"" Difficulty=""4""/><Word Text=""adequate"" Difficulty=""4""/><Word Text=""concrete"" Difficulty=""4""/><Word Text=""document"" Difficulty=""4""/><Word Text=""changing"" Difficulty=""4""/><Word Text=""colonial"" Difficulty=""4""/><Word Text=""criminal"" Difficulty=""4""/><Word Text=""homeless"" Difficulty=""4""/><Word Text=""decrease"" Difficulty=""4""/><Word Text=""alliance"" Difficulty=""4""/><Word Text=""regulate"" Difficulty=""4""/><Word Text=""laughter"" Difficulty=""4""/><Word Text=""receiver"" Difficulty=""4""/><Word Text=""superior"" Difficulty=""4""/><Word Text=""compound"" Difficulty=""4""/><Word Text=""drinking"" Difficulty=""4""/><Word Text=""collapse"" Difficulty=""4""/><Word Text=""midnight"" Difficulty=""4""/><Word Text=""suburban"" Difficulty=""4""/><Word Text=""interior"" Difficulty=""4""/><Word Text=""corridor"" Difficulty=""4""/><Word Text=""weakness"" Difficulty=""4""/><Word Text=""humanity"" Difficulty=""4""/><Word Text=""reliable"" Difficulty=""4""/><Word Text=""Hispanic"" Difficulty=""4""/><Word Text=""airplane"" Difficulty=""4""/><Word Text=""initiate"" Difficulty=""4""/><Word Text=""sandwich"" Difficulty=""4""/><Word Text=""motivate"" Difficulty=""4""/><Word Text=""longtime"" Difficulty=""4""/><Word Text=""restrict"" Difficulty=""4""/><Word Text=""assemble"" Difficulty=""4""/><Word Text=""obstacle"" Difficulty=""4""/><Word Text=""basement"" Difficulty=""4""/><Word Text=""bacteria"" Difficulty=""4""/><Word Text=""database"" Difficulty=""4""/><Word Text=""ideology"" Difficulty=""4""/><Word Text=""railroad"" Difficulty=""4""/><Word Text=""peaceful"" Difficulty=""4""/><Word Text=""grateful"" Difficulty=""4""/><Word Text=""response"" Difficulty=""4""/><Word Text=""adoption"" Difficulty=""4""/><Word Text=""civilian"" Difficulty=""4""/><Word Text=""particle"" Difficulty=""4""/><Word Text=""festival"" Difficulty=""4""/><Word Text=""freshman"" Difficulty=""4""/><Word Text=""European"" Difficulty=""4""/><Word Text=""research"" Difficulty=""4""/><Word Text=""wherever"" Difficulty=""4""/><Word Text=""rhetoric"" Difficulty=""4""/><Word Text=""profound"" Difficulty=""4""/><Word Text=""currency"" Difficulty=""4""/><Word Text=""doctrine"" Difficulty=""4""/><Word Text=""horrible"" Difficulty=""4""/><Word Text=""commonly"" Difficulty=""4""/><Word Text=""sidewalk"" Difficulty=""4""/><Word Text=""Olympics"" Difficulty=""4""/><Word Text=""pleasant"" Difficulty=""4""/><Word Text=""delicate"" Difficulty=""4""/><Word Text=""forehead"" Difficulty=""4""/><Word Text=""traveler"" Difficulty=""4""/><Word Text=""dedicate"" Difficulty=""4""/><Word Text=""diagnose"" Difficulty=""4""/><Word Text=""theology"" Difficulty=""4""/><Word Text=""handsome"" Difficulty=""4""/><Word Text=""provided"" Difficulty=""4""/><Word Text=""Japanese"" Difficulty=""4""/><Word Text=""wildlife"" Difficulty=""4""/><Word Text=""elevator"" Difficulty=""4""/><Word Text=""guidance"" Difficulty=""4""/><Word Text=""envelope"" Difficulty=""4""/><Word Text=""generous"" Difficulty=""4""/><Word Text=""sunlight"" Difficulty=""4""/><Word Text=""feedback"" Difficulty=""4""/><Word Text=""spectrum"" Difficulty=""4""/><Word Text=""starting"" Difficulty=""4""/><Word Text=""advocate"" Difficulty=""4""/><Word Text=""hesitate"" Difficulty=""4""/><Word Text=""metaphor"" Difficulty=""4""/><Word Text=""judicial"" Difficulty=""4""/><Word Text=""addition"" Difficulty=""4""/><Word Text=""interior"" Difficulty=""4""/><Word Text=""diminish"" Difficulty=""4""/><Word Text=""minimize"" Difficulty=""4""/><Word Text=""assembly"" Difficulty=""4""/><Word Text=""equation"" Difficulty=""4""/><Word Text=""offering"" Difficulty=""4""/><Word Text=""precious"" Difficulty=""4""/><Word Text=""prohibit"" Difficulty=""4""/><Word Text=""abstract"" Difficulty=""4""/><Word Text=""hardware"" Difficulty=""4""/><Word Text=""shortage"" Difficulty=""4""/><Word Text=""annually"" Difficulty=""4""/><Word Text=""deadline"" Difficulty=""4""/><Word Text=""sexually"" Difficulty=""4""/><Word Text=""quantity"" Difficulty=""4""/><Word Text=""monument"" Difficulty=""4""/><Word Text=""accuracy"" Difficulty=""4""/><Word Text=""treasure"" Difficulty=""4""/><Word Text=""talented"" Difficulty=""4""/><Word Text=""gasoline"" Difficulty=""4""/><Word Text=""extended"" Difficulty=""4""/><Word Text=""diabetes"" Difficulty=""4""/><Word Text=""dynamics"" Difficulty=""4""/><Word Text=""parental"" Difficulty=""4""/><Word Text=""merchant"" Difficulty=""4""/><Word Text=""improved"" Difficulty=""4""/><Word Text=""ancestor"" Difficulty=""4""/><Word Text=""homeland"" Difficulty=""4""/><Word Text=""exchange"" Difficulty=""4""/><Word Text=""symbolic"" Difficulty=""4""/><Word Text=""conceive"" Difficulty=""4""/><Word Text=""combined"" Difficulty=""4""/><Word Text=""patience"" Difficulty=""4""/><Word Text=""tropical"" Difficulty=""4""/><Word Text=""position"" Difficulty=""4""/><Word Text=""intimate"" Difficulty=""4""/><Word Text=""flexible"" Difficulty=""4""/><Word Text=""casualty"" Difficulty=""4""/><Word Text=""republic"" Difficulty=""4""/><Word Text=""terrific"" Difficulty=""4""/><Word Text=""instinct"" Difficulty=""4""/><Word Text=""teammate"" Difficulty=""4""/><Word Text=""aluminum"" Difficulty=""4""/><Word Text=""ministry"" Difficulty=""4""/><Word Text=""instruct"" Difficulty=""4""/><Word Text=""mushroom"" Difficulty=""4""/><Word Text=""mechanic"" Difficulty=""4""/><Word Text=""sympathy"" Difficulty=""4""/><Word Text=""syndrome"" Difficulty=""4""/><Word Text=""ambition"" Difficulty=""4""/><Word Text=""dissolve"" Difficulty=""4""/><Word Text=""expected"" Difficulty=""4""/><Word Text=""actively"" Difficulty=""4""/><Word Text=""illusion"" Difficulty=""4""/><Word Text=""tolerate"" Difficulty=""4""/><Word Text=""scramble"" Difficulty=""4""/><Word Text=""decorate"" Difficulty=""4""/><Word Text=""donation"" Difficulty=""4""/><Word Text=""interact"" Difficulty=""4""/><Word Text=""supplier"" Difficulty=""4""/><Word Text=""momentum"" Difficulty=""4""/><Word Text=""elephant"" Difficulty=""4""/><Word Text=""mentally"" Difficulty=""4""/><Word Text=""organism"" Difficulty=""4""/><Word Text=""upstairs"" Difficulty=""4""/><Word Text=""backyard"" Difficulty=""4""/><Word Text=""comprise"" Difficulty=""4""/><Word Text=""reminder"" Difficulty=""4""/><Word Text=""disabled"" Difficulty=""4""/><Word Text=""frontier"" Difficulty=""4""/><Word Text=""disclose"" Difficulty=""4""/><Word Text=""notebook"" Difficulty=""4""/><Word Text=""vertical"" Difficulty=""4""/><Word Text=""swimming"" Difficulty=""4""/><Word Text=""outsider"" Difficulty=""4""/><Word Text=""proclaim"" Difficulty=""4""/><Word Text=""required"" Difficulty=""4""/><Word Text=""colorful"" Difficulty=""4""/><Word Text=""textbook"" Difficulty=""4""/><Word Text=""emerging"" Difficulty=""4""/><Word Text=""envision"" Difficulty=""4""/><Word Text=""rational"" Difficulty=""4""/><Word Text=""protocol"" Difficulty=""4""/><Word Text=""distract"" Difficulty=""4""/><Word Text=""discount"" Difficulty=""4""/><Word Text=""retailer"" Difficulty=""4""/><Word Text=""classify"" Difficulty=""4""/><Word Text=""stimulus"" Difficulty=""4""/><Word Text=""likewise"" Difficulty=""4""/><Word Text=""informal"" Difficulty=""4""/><Word Text=""validity"" Difficulty=""4""/><Word Text=""strictly"" Difficulty=""4""/><Word Text=""artifact"" Difficulty=""4""/><Word Text=""listener"" Difficulty=""4""/><Word Text=""socially"" Difficulty=""4""/><Word Text=""equality"" Difficulty=""4""/><Word Text=""cemetery"" Difficulty=""4""/><Word Text=""striking"" Difficulty=""4""/><Word Text=""isolated"" Difficulty=""4""/><Word Text=""eligible"" Difficulty=""4""/><Word Text=""interval"" Difficulty=""4""/><Word Text=""feminist"" Difficulty=""4""/><Word Text=""sprinkle"" Difficulty=""4""/><Word Text=""blessing"" Difficulty=""4""/><Word Text=""formerly"" Difficulty=""4""/><Word Text=""lawmaker"" Difficulty=""4""/><Word Text=""calendar"" Difficulty=""4""/><Word Text=""downtown"" Difficulty=""4""/><Word Text=""predator"" Difficulty=""4""/><Word Text=""autonomy"" Difficulty=""4""/><Word Text=""landmark"" Difficulty=""4""/><Word Text=""offender"" Difficulty=""4""/><Word Text=""fraction"" Difficulty=""4""/><Word Text=""fragment"" Difficulty=""4""/><Word Text=""headache"" Difficulty=""4""/><Word Text=""suitable"" Difficulty=""4""/><Word Text=""driveway"" Difficulty=""4""/><Word Text=""homework"" Difficulty=""4""/><Word Text=""molecule"" Difficulty=""4""/><Word Text=""steadily"" Difficulty=""4""/><Word Text=""defender"" Difficulty=""4""/><Word Text=""explicit"" Difficulty=""4""/><Word Text=""magnetic"" Difficulty=""4""/><Word Text=""meantime"" Difficulty=""4""/><Word Text=""transmit"" Difficulty=""4""/><Word Text=""nutrient"" Difficulty=""4""/><Word Text=""severely"" Difficulty=""4""/><Word Text=""lighting"" Difficulty=""4""/><Word Text=""terribly"" Difficulty=""4""/><Word Text=""honestly"" Difficulty=""4""/><Word Text=""troubled"" Difficulty=""4""/><Word Text=""balanced"" Difficulty=""4""/><Word Text=""managing"" Difficulty=""4""/><Word Text=""diplomat"" Difficulty=""4""/><Word Text=""sometime"" Difficulty=""4""/><Word Text=""epidemic"" Difficulty=""4""/><Word Text=""inherent"" Difficulty=""4""/><Word Text=""selected"" Difficulty=""4""/><Word Text=""something"" Difficulty=""4""/><Word Text=""different"" Difficulty=""4""/><Word Text=""important"" Difficulty=""4""/><Word Text=""political"" Difficulty=""4""/><Word Text=""community"" Difficulty=""4""/><Word Text=""president"" Difficulty=""4""/><Word Text=""education"" Difficulty=""4""/><Word Text=""including"" Difficulty=""4""/><Word Text=""sometimes"" Difficulty=""5""/><Word Text=""according"" Difficulty=""5""/><Word Text=""situation"" Difficulty=""5""/><Word Text=""attention"" Difficulty=""5""/><Word Text=""difficult"" Difficulty=""5""/><Word Text=""available"" Difficulty=""5""/><Word Text=""condition"" Difficulty=""5""/><Word Text=""certainly"" Difficulty=""5""/><Word Text=""represent"" Difficulty=""5""/><Word Text=""treatment"" Difficulty=""5""/><Word Text=""determine"" Difficulty=""5""/><Word Text=""recognize"" Difficulty=""5""/><Word Text=""character"" Difficulty=""5""/><Word Text=""everybody"" Difficulty=""5""/><Word Text=""professor"" Difficulty=""5""/><Word Text=""operation"" Difficulty=""5""/><Word Text=""financial"" Difficulty=""5""/><Word Text=""authority"" Difficulty=""5""/><Word Text=""knowledge"" Difficulty=""5""/><Word Text=""executive"" Difficulty=""5""/><Word Text=""religious"" Difficulty=""5""/><Word Text=""establish"" Difficulty=""5""/><Word Text=""statement"" Difficulty=""5""/><Word Text=""direction"" Difficulty=""5""/><Word Text=""interview"" Difficulty=""5""/><Word Text=""structure"" Difficulty=""5""/><Word Text=""candidate"" Difficulty=""5""/><Word Text=""necessary"" Difficulty=""5""/><Word Text=""challenge"" Difficulty=""5""/><Word Text=""beautiful"" Difficulty=""5""/><Word Text=""scientist"" Difficulty=""5""/><Word Text=""agreement"" Difficulty=""5""/><Word Text=""newspaper"" Difficulty=""5""/><Word Text=""concerned"" Difficulty=""5""/><Word Text=""effective"" Difficulty=""5""/><Word Text=""therefore"" Difficulty=""5""/><Word Text=""encourage"" Difficulty=""5""/><Word Text=""afternoon"" Difficulty=""5""/><Word Text=""insurance"" Difficulty=""5""/><Word Text=""beginning"" Difficulty=""5""/><Word Text=""generally"" Difficulty=""5""/><Word Text=""introduce"" Difficulty=""5""/><Word Text=""tradition"" Difficulty=""5""/><Word Text=""potential"" Difficulty=""5""/><Word Text=""Christian"" Difficulty=""5""/><Word Text=""apartment"" Difficulty=""5""/><Word Text=""obviously"" Difficulty=""5""/><Word Text=""advantage"" Difficulty=""5""/><Word Text=""technique"" Difficulty=""5""/><Word Text=""principle"" Difficulty=""5""/><Word Text=""equipment"" Difficulty=""5""/><Word Text=""associate"" Difficulty=""5""/><Word Text=""Christmas"" Difficulty=""5""/><Word Text=""procedure"" Difficulty=""5""/><Word Text=""influence"" Difficulty=""5""/><Word Text=""wonderful"" Difficulty=""5""/><Word Text=""committee"" Difficulty=""5""/><Word Text=""dangerous"" Difficulty=""5""/><Word Text=""following"" Difficulty=""5""/><Word Text=""classroom"" Difficulty=""5""/><Word Text=""democracy"" Difficulty=""5""/><Word Text=""yesterday"" Difficulty=""5""/><Word Text=""colleague"" Difficulty=""5""/><Word Text=""otherwise"" Difficulty=""5""/><Word Text=""disappear"" Difficulty=""5""/><Word Text=""corporate"" Difficulty=""5""/><Word Text=""somewhere"" Difficulty=""5""/><Word Text=""carefully"" Difficulty=""5""/><Word Text=""emotional"" Difficulty=""5""/><Word Text=""expensive"" Difficulty=""5""/><Word Text=""recommend"" Difficulty=""5""/><Word Text=""basically"" Difficulty=""5""/><Word Text=""currently"" Difficulty=""5""/><Word Text=""emergency"" Difficulty=""5""/><Word Text=""extremely"" Difficulty=""5""/><Word Text=""component"" Difficulty=""5""/><Word Text=""long-term"" Difficulty=""5""/><Word Text=""challenge"" Difficulty=""5""/><Word Text=""ourselves"" Difficulty=""5""/><Word Text=""influence"" Difficulty=""5""/><Word Text=""landscape"" Difficulty=""5""/><Word Text=""eliminate"" Difficulty=""5""/><Word Text=""meanwhile"" Difficulty=""5""/><Word Text=""telephone"" Difficulty=""5""/><Word Text=""reference"" Difficulty=""5""/><Word Text=""seriously"" Difficulty=""5""/><Word Text=""surprised"" Difficulty=""5""/><Word Text=""vegetable"" Difficulty=""5""/><Word Text=""essential"" Difficulty=""5""/><Word Text=""household"" Difficulty=""5""/><Word Text=""increased"" Difficulty=""5""/><Word Text=""emphasize"" Difficulty=""5""/><Word Text=""secretary"" Difficulty=""5""/><Word Text=""typically"" Difficulty=""5""/><Word Text=""celebrate"" Difficulty=""5""/><Word Text=""physician"" Difficulty=""5""/><Word Text=""virtually"" Difficulty=""5""/><Word Text=""technical"" Difficulty=""5""/><Word Text=""potential"" Difficulty=""5""/><Word Text=""immigrant"" Difficulty=""5""/><Word Text=""passenger"" Difficulty=""5""/><Word Text=""criticism"" Difficulty=""5""/><Word Text=""spiritual"" Difficulty=""5""/><Word Text=""childhood"" Difficulty=""5""/><Word Text=""cigarette"" Difficulty=""5""/><Word Text=""excellent"" Difficulty=""5""/><Word Text=""selection"" Difficulty=""5""/><Word Text=""primarily"" Difficulty=""5""/><Word Text=""regarding"" Difficulty=""5""/><Word Text=""remaining"" Difficulty=""5""/><Word Text=""territory"" Difficulty=""5""/><Word Text=""immediate"" Difficulty=""5""/><Word Text=""transform"" Difficulty=""5""/><Word Text=""existence"" Difficulty=""5""/><Word Text=""discovery"" Difficulty=""5""/><Word Text=""coalition"" Difficulty=""5""/><Word Text=""interview"" Difficulty=""5""/><Word Text=""reduction"" Difficulty=""5""/><Word Text=""substance"" Difficulty=""5""/><Word Text=""elsewhere"" Difficulty=""5""/><Word Text=""practical"" Difficulty=""5""/><Word Text=""volunteer"" Difficulty=""5""/><Word Text=""implement"" Difficulty=""5""/><Word Text=""marketing"" Difficulty=""5""/><Word Text=""complaint"" Difficulty=""5""/><Word Text=""commander"" Difficulty=""5""/><Word Text=""breakfast"" Difficulty=""5""/><Word Text=""so-called"" Difficulty=""5""/><Word Text=""exception"" Difficulty=""5""/><Word Text=""objective"" Difficulty=""5""/><Word Text=""dimension"" Difficulty=""5""/><Word Text=""personnel"" Difficulty=""5""/><Word Text=""perfectly"" Difficulty=""5""/><Word Text=""supporter"" Difficulty=""5""/><Word Text=""accompany"" Difficulty=""5""/><Word Text=""gentleman"" Difficulty=""5""/><Word Text=""permanent"" Difficulty=""5""/><Word Text=""literally"" Difficulty=""5""/><Word Text=""construct"" Difficulty=""5""/><Word Text=""sensitive"" Difficulty=""5""/><Word Text=""intention"" Difficulty=""5""/><Word Text=""diversity"" Difficulty=""5""/><Word Text=""historian"" Difficulty=""5""/><Word Text=""negotiate"" Difficulty=""5""/><Word Text=""criticize"" Difficulty=""5""/><Word Text=""precisely"" Difficulty=""5""/><Word Text=""terrorism"" Difficulty=""5""/><Word Text=""provision"" Difficulty=""5""/><Word Text=""satellite"" Difficulty=""5""/><Word Text=""chocolate"" Difficulty=""5""/><Word Text=""universal"" Difficulty=""5""/><Word Text=""testimony"" Difficulty=""5""/><Word Text=""furniture"" Difficulty=""5""/><Word Text=""mechanism"" Difficulty=""5""/><Word Text=""infection"" Difficulty=""5""/><Word Text=""strategic"" Difficulty=""5""/><Word Text=""assistant"" Difficulty=""5""/><Word Text=""encounter"" Difficulty=""5""/><Word Text=""initially"" Difficulty=""5""/><Word Text=""spokesman"" Difficulty=""5""/><Word Text=""incentive"" Difficulty=""5""/><Word Text=""translate"" Difficulty=""5""/><Word Text=""expansion"" Difficulty=""5""/><Word Text=""telescope"" Difficulty=""5""/><Word Text=""interpret"" Difficulty=""5""/><Word Text=""guarantee"" Difficulty=""5""/><Word Text=""awareness"" Difficulty=""5""/><Word Text=""similarly"" Difficulty=""5""/><Word Text=""naturally"" Difficulty=""5""/><Word Text=""regularly"" Difficulty=""5""/><Word Text=""assistant"" Difficulty=""5""/><Word Text=""terrorist"" Difficulty=""5""/><Word Text=""extensive"" Difficulty=""5""/><Word Text=""adventure"" Difficulty=""5""/><Word Text=""confident"" Difficulty=""5""/><Word Text=""violation"" Difficulty=""5""/><Word Text=""defensive"" Difficulty=""5""/><Word Text=""prominent"" Difficulty=""5""/><Word Text=""pollution"" Difficulty=""5""/><Word Text=""variation"" Difficulty=""5""/><Word Text=""evolution"" Difficulty=""5""/><Word Text=""celebrity"" Difficulty=""5""/><Word Text=""gradually"" Difficulty=""5""/><Word Text=""stability"" Difficulty=""5""/><Word Text=""framework"" Difficulty=""5""/><Word Text=""depending"" Difficulty=""5""/><Word Text=""counselor"" Difficulty=""5""/><Word Text=""economist"" Difficulty=""5""/><Word Text=""efficient"" Difficulty=""5""/><Word Text=""frequency"" Difficulty=""5""/><Word Text=""explosion"" Difficulty=""5""/><Word Text=""admission"" Difficulty=""5""/><Word Text=""calculate"" Difficulty=""5""/><Word Text=""formation"" Difficulty=""5""/><Word Text=""guideline"" Difficulty=""5""/><Word Text=""publisher"" Difficulty=""5""/><Word Text=""desperate"" Difficulty=""5""/><Word Text=""lifestyle"" Difficulty=""5""/><Word Text=""narrative"" Difficulty=""5""/><Word Text=""principal"" Difficulty=""5""/><Word Text=""temporary"" Difficulty=""5""/><Word Text=""brilliant"" Difficulty=""5""/><Word Text=""Christian"" Difficulty=""5""/><Word Text=""offensive"" Difficulty=""5""/><Word Text=""terrorist"" Difficulty=""5""/><Word Text=""economics"" Difficulty=""5""/><Word Text=""extension"" Difficulty=""5""/><Word Text=""inflation"" Difficulty=""5""/><Word Text=""dependent"" Difficulty=""5""/><Word Text=""operating"" Difficulty=""5""/><Word Text=""discourse"" Difficulty=""5""/><Word Text=""continued"" Difficulty=""5""/><Word Text=""intensity"" Difficulty=""5""/><Word Text=""principal"" Difficulty=""5""/><Word Text=""consensus"" Difficulty=""5""/><Word Text=""recording"" Difficulty=""5""/><Word Text=""pregnancy"" Difficulty=""5""/><Word Text=""reinforce"" Difficulty=""5""/><Word Text=""confusion"" Difficulty=""5""/><Word Text=""cognitive"" Difficulty=""5""/><Word Text=""attribute"" Difficulty=""5""/><Word Text=""defendant"" Difficulty=""5""/><Word Text=""container"" Difficulty=""5""/><Word Text=""architect"" Difficulty=""5""/><Word Text=""highlight"" Difficulty=""5""/><Word Text=""boyfriend"" Difficulty=""5""/><Word Text=""northwest"" Difficulty=""5""/><Word Text=""interrupt"" Difficulty=""5""/><Word Text=""sculpture"" Difficulty=""5""/><Word Text=""integrate"" Difficulty=""5""/><Word Text=""secondary"" Difficulty=""5""/><Word Text=""integrity"" Difficulty=""5""/><Word Text=""classical"" Difficulty=""5""/><Word Text=""estimated"" Difficulty=""5""/><Word Text=""developer"" Difficulty=""5""/><Word Text=""seemingly"" Difficulty=""5""/><Word Text=""inspector"" Difficulty=""5""/><Word Text=""companion"" Difficulty=""5""/><Word Text=""southwest"" Difficulty=""5""/><Word Text=""encounter"" Difficulty=""5""/><Word Text=""recession"" Difficulty=""5""/><Word Text=""ownership"" Difficulty=""5""/><Word Text=""nightmare"" Difficulty=""5""/><Word Text=""diagnosis"" Difficulty=""5""/><Word Text=""privilege"" Difficulty=""5""/><Word Text=""broadcast"" Difficulty=""5""/><Word Text=""radiation"" Difficulty=""5""/><Word Text=""amendment"" Difficulty=""5""/><Word Text=""undermine"" Difficulty=""5""/><Word Text=""southeast"" Difficulty=""5""/><Word Text=""convinced"" Difficulty=""5""/><Word Text=""apologize"" Difficulty=""5""/><Word Text=""exclusive"" Difficulty=""5""/><Word Text=""suspicion"" Difficulty=""5""/><Word Text=""residence"" Difficulty=""5""/><Word Text=""signature"" Difficulty=""5""/><Word Text=""promotion"" Difficulty=""5""/><Word Text=""detective"" Difficulty=""5""/><Word Text=""portfolio"" Difficulty=""5""/><Word Text=""invisible"" Difficulty=""5""/><Word Text=""identical"" Difficulty=""5""/><Word Text=""nonprofit"" Difficulty=""5""/><Word Text=""promising"" Difficulty=""5""/><Word Text=""conscious"" Difficulty=""5""/><Word Text=""departure"" Difficulty=""5""/><Word Text=""happiness"" Difficulty=""5""/><Word Text=""reporting"" Difficulty=""5""/><Word Text=""indicator"" Difficulty=""5""/><Word Text=""reluctant"" Difficulty=""5""/><Word Text=""expertise"" Difficulty=""5""/><Word Text=""volunteer"" Difficulty=""5""/><Word Text=""therapist"" Difficulty=""5""/><Word Text=""recipient"" Difficulty=""5""/><Word Text=""suffering"" Difficulty=""5""/><Word Text=""full-time"" Difficulty=""5""/><Word Text=""reception"" Difficulty=""5""/><Word Text=""necessity"" Difficulty=""5""/><Word Text=""performer"" Difficulty=""5""/><Word Text=""inventory"" Difficulty=""5""/><Word Text=""magnitude"" Difficulty=""5""/><Word Text=""collector"" Difficulty=""5""/><Word Text=""realistic"" Difficulty=""5""/><Word Text=""gathering"" Difficulty=""5""/><Word Text=""hopefully"" Difficulty=""5""/><Word Text=""cooperate"" Difficulty=""5""/><Word Text=""continent"" Difficulty=""5""/><Word Text=""undertake"" Difficulty=""5""/><Word Text=""automatic"" Difficulty=""5""/><Word Text=""sentiment"" Difficulty=""5""/><Word Text=""sacrifice"" Difficulty=""5""/><Word Text=""northeast"" Difficulty=""5""/><Word Text=""liability"" Difficulty=""5""/><Word Text=""courtroom"" Difficulty=""5""/><Word Text=""instantly"" Difficulty=""5""/><Word Text=""afterward"" Difficulty=""5""/><Word Text=""alongside"" Difficulty=""5""/><Word Text=""execution"" Difficulty=""5""/><Word Text=""fisherman"" Difficulty=""5""/><Word Text=""isolation"" Difficulty=""5""/><Word Text=""workplace"" Difficulty=""5""/><Word Text=""touchdown"" Difficulty=""5""/><Word Text=""ambitious"" Difficulty=""5""/><Word Text=""uncertain"" Difficulty=""5""/><Word Text=""aesthetic"" Difficulty=""5""/><Word Text=""anonymous"" Difficulty=""5""/><Word Text=""associate"" Difficulty=""5""/><Word Text=""franchise"" Difficulty=""5""/><Word Text=""correctly"" Difficulty=""5""/><Word Text=""sensation"" Difficulty=""5""/><Word Text=""partially"" Difficulty=""5""/><Word Text=""placement"" Difficulty=""5""/><Word Text=""columnist"" Difficulty=""5""/><Word Text=""associate"" Difficulty=""5""/><Word Text=""interfere"" Difficulty=""5""/><Word Text=""stimulate"" Difficulty=""5""/><Word Text=""sacrifice"" Difficulty=""5""/><Word Text=""worldwide"" Difficulty=""5""/><Word Text=""depressed"" Difficulty=""5""/><Word Text=""migration"" Difficulty=""5""/><Word Text=""breathing"" Difficulty=""5""/><Word Text=""hurricane"" Difficulty=""5""/><Word Text=""curiosity"" Difficulty=""5""/><Word Text=""perceived"" Difficulty=""5""/><Word Text=""publicity"" Difficulty=""5""/><Word Text=""ecosystem"" Difficulty=""5""/><Word Text=""specialty"" Difficulty=""5""/><Word Text=""lightning"" Difficulty=""5""/><Word Text=""excessive"" Difficulty=""5""/><Word Text=""high-tech"" Difficulty=""5""/><Word Text=""commodity"" Difficulty=""5""/><Word Text=""processor"" Difficulty=""5""/><Word Text=""elaborate"" Difficulty=""5""/><Word Text=""transport"" Difficulty=""5""/><Word Text=""allegedly"" Difficulty=""5""/><Word Text=""mortality"" Difficulty=""5""/><Word Text=""municipal"" Difficulty=""5""/><Word Text=""tolerance"" Difficulty=""5""/><Word Text=""screening"" Difficulty=""5""/><Word Text=""voluntary"" Difficulty=""5""/><Word Text=""privately"" Difficulty=""5""/><Word Text=""threshold"" Difficulty=""5""/><Word Text=""routinely"" Difficulty=""5""/><Word Text=""regulator"" Difficulty=""5""/><Word Text=""objection"" Difficulty=""5""/><Word Text=""chemistry"" Difficulty=""5""/><Word Text=""overnight"" Difficulty=""5""/><Word Text=""fantastic"" Difficulty=""5""/><Word Text=""policeman"" Difficulty=""5""/><Word Text=""authorize"" Difficulty=""5""/><Word Text=""sexuality"" Difficulty=""5""/><Word Text=""invention"" Difficulty=""5""/><Word Text=""guarantee"" Difficulty=""5""/><Word Text=""favorable"" Difficulty=""5""/><Word Text=""youngster"" Difficulty=""5""/><Word Text=""broadcast"" Difficulty=""5""/><Word Text=""overwhelm"" Difficulty=""5""/><Word Text=""one-third"" Difficulty=""5""/><Word Text=""speculate"" Difficulty=""5""/><Word Text=""transport"" Difficulty=""5""/><Word Text=""worldwide"" Difficulty=""5""/><Word Text=""frustrate"" Difficulty=""5""/><Word Text=""biography"" Difficulty=""5""/><Word Text=""twentieth"" Difficulty=""5""/><Word Text=""foreigner"" Difficulty=""5""/><Word Text=""organized"" Difficulty=""5""/><Word Text=""warehouse"" Difficulty=""5""/><Word Text=""butterfly"" Difficulty=""5""/><Word Text=""plaintiff"" Difficulty=""5""/><Word Text=""government"" Difficulty=""5""/><Word Text=""understand"" Difficulty=""5""/><Word Text=""everything"" Difficulty=""5""/><Word Text=""experience"" Difficulty=""6""/><Word Text=""themselves"" Difficulty=""6""/><Word Text=""difference"" Difficulty=""6""/><Word Text=""especially"" Difficulty=""6""/><Word Text=""technology"" Difficulty=""6""/><Word Text=""Republican"" Difficulty=""6""/><Word Text=""population"" Difficulty=""6""/><Word Text=""individual"" Difficulty=""6""/><Word Text=""television"" Difficulty=""6""/><Word Text=""democratic"" Difficulty=""6""/><Word Text=""management"" Difficulty=""6""/><Word Text=""particular"" Difficulty=""6""/><Word Text=""production"" Difficulty=""6""/><Word Text=""conference"" Difficulty=""6""/><Word Text=""individual"" Difficulty=""6""/><Word Text=""throughout"" Difficulty=""6""/><Word Text=""generation"" Difficulty=""6""/><Word Text=""commercial"" Difficulty=""6""/><Word Text=""investment"" Difficulty=""6""/><Word Text=""discussion"" Difficulty=""6""/><Word Text=""collection"" Difficulty=""6""/><Word Text=""successful"" Difficulty=""6""/><Word Text=""eventually"" Difficulty=""6""/><Word Text=""restaurant"" Difficulty=""6""/><Word Text=""absolutely"" Difficulty=""6""/><Word Text=""completely"" Difficulty=""6""/><Word Text=""researcher"" Difficulty=""6""/><Word Text=""experience"" Difficulty=""6""/><Word Text=""department"" Difficulty=""6""/><Word Text=""university"" Difficulty=""6""/><Word Text=""interested"" Difficulty=""6""/><Word Text=""leadership"" Difficulty=""6""/><Word Text=""contribute"" Difficulty=""6""/><Word Text=""protection"" Difficulty=""6""/><Word Text=""additional"" Difficulty=""6""/><Word Text=""background"" Difficulty=""6""/><Word Text=""apparently"" Difficulty=""6""/><Word Text=""connection"" Difficulty=""6""/><Word Text=""relatively"" Difficulty=""6""/><Word Text=""historical"" Difficulty=""6""/><Word Text=""expression"" Difficulty=""6""/><Word Text=""literature"" Difficulty=""6""/><Word Text=""assessment"" Difficulty=""6""/><Word Text=""importance"" Difficulty=""6""/><Word Text=""scientific"" Difficulty=""6""/><Word Text=""impossible"" Difficulty=""6""/><Word Text=""instrument"" Difficulty=""6""/><Word Text=""commitment"" Difficulty=""6""/><Word Text=""photograph"" Difficulty=""6""/><Word Text=""conclusion"" Difficulty=""6""/><Word Text=""regulation"" Difficulty=""6""/><Word Text=""appearance"" Difficulty=""6""/><Word Text=""difficulty"" Difficulty=""6""/><Word Text=""appreciate"" Difficulty=""6""/><Word Text=""ultimately"" Difficulty=""6""/><Word Text=""politician"" Difficulty=""6""/><Word Text=""percentage"" Difficulty=""6""/><Word Text=""basketball"" Difficulty=""6""/><Word Text=""frequently"" Difficulty=""6""/><Word Text=""perception"" Difficulty=""6""/><Word Text=""confidence"" Difficulty=""6""/><Word Text=""opposition"" Difficulty=""6""/><Word Text=""industrial"" Difficulty=""6""/><Word Text=""everywhere"" Difficulty=""6""/><Word Text=""resolution"" Difficulty=""6""/><Word Text=""experiment"" Difficulty=""6""/><Word Text=""definitely"" Difficulty=""6""/><Word Text=""curriculum"" Difficulty=""6""/><Word Text=""assistance"" Difficulty=""6""/><Word Text=""depression"" Difficulty=""6""/><Word Text=""journalist"" Difficulty=""6""/><Word Text=""definition"" Difficulty=""6""/><Word Text=""prosecutor"" Difficulty=""6""/><Word Text=""initiative"" Difficulty=""6""/><Word Text=""comparison"" Difficulty=""6""/><Word Text=""settlement"" Difficulty=""6""/><Word Text=""transition"" Difficulty=""6""/><Word Text=""atmosphere"" Difficulty=""6""/><Word Text=""consistent"" Difficulty=""6""/><Word Text=""resistance"" Difficulty=""6""/><Word Text=""philosophy"" Difficulty=""6""/><Word Text=""foundation"" Difficulty=""6""/><Word Text=""discipline"" Difficulty=""6""/><Word Text=""previously"" Difficulty=""6""/><Word Text=""accomplish"" Difficulty=""6""/><Word Text=""illustrate"" Difficulty=""6""/><Word Text=""evaluation"" Difficulty=""6""/><Word Text=""increasing"" Difficulty=""6""/><Word Text=""commission"" Difficulty=""6""/><Word Text=""tablespoon"" Difficulty=""6""/><Word Text=""electronic"" Difficulty=""6""/><Word Text=""reputation"" Difficulty=""6""/><Word Text=""retirement"" Difficulty=""6""/><Word Text=""phenomenon"" Difficulty=""6""/><Word Text=""convention"" Difficulty=""6""/><Word Text=""exhibition"" Difficulty=""6""/><Word Text=""consultant"" Difficulty=""6""/><Word Text=""constantly"" Difficulty=""6""/><Word Text=""enterprise"" Difficulty=""6""/><Word Text=""suggestion"" Difficulty=""6""/><Word Text=""reasonable"" Difficulty=""6""/><Word Text=""elementary"" Difficulty=""6""/><Word Text=""aggressive"" Difficulty=""6""/><Word Text=""employment"" Difficulty=""6""/><Word Text=""impression"" Difficulty=""6""/><Word Text=""respondent"" Difficulty=""6""/><Word Text=""particular"" Difficulty=""6""/><Word Text=""specialist"" Difficulty=""6""/><Word Text=""disability"" Difficulty=""6""/><Word Text=""biological"" Difficulty=""6""/><Word Text=""ingredient"" Difficulty=""6""/><Word Text=""assumption"" Difficulty=""6""/><Word Text=""developing"" Difficulty=""6""/><Word Text=""personally"" Difficulty=""6""/><Word Text=""remarkable"" Difficulty=""6""/><Word Text=""statistics"" Difficulty=""6""/><Word Text=""reflection"" Difficulty=""6""/><Word Text=""revolution"" Difficulty=""6""/><Word Text=""tournament"" Difficulty=""6""/><Word Text=""tremendous"" Difficulty=""6""/><Word Text=""surprising"" Difficulty=""6""/><Word Text=""attractive"" Difficulty=""6""/><Word Text=""originally"" Difficulty=""6""/><Word Text=""profession"" Difficulty=""6""/><Word Text=""constitute"" Difficulty=""6""/><Word Text=""regardless"" Difficulty=""6""/><Word Text=""distribute"" Difficulty=""6""/><Word Text=""vulnerable"" Difficulty=""6""/><Word Text=""capability"" Difficulty=""6""/><Word Text=""psychology"" Difficulty=""6""/><Word Text=""obligation"" Difficulty=""6""/><Word Text=""limitation"" Difficulty=""6""/><Word Text=""preference"" Difficulty=""6""/><Word Text=""incredible"" Difficulty=""6""/><Word Text=""friendship"" Difficulty=""6""/><Word Text=""efficiency"" Difficulty=""6""/><Word Text=""proportion"" Difficulty=""6""/><Word Text=""conviction"" Difficulty=""6""/><Word Text=""strengthen"" Difficulty=""6""/><Word Text=""membership"" Difficulty=""6""/><Word Text=""sufficient"" Difficulty=""6""/><Word Text=""helicopter"" Difficulty=""6""/><Word Text=""punishment"" Difficulty=""6""/><Word Text=""girlfriend"" Difficulty=""6""/><Word Text=""adjustment"" Difficulty=""6""/><Word Text=""motivation"" Difficulty=""6""/><Word Text=""assignment"" Difficulty=""6""/><Word Text=""laboratory"" Difficulty=""6""/><Word Text=""medication"" Difficulty=""6""/><Word Text=""anticipate"" Difficulty=""6""/><Word Text=""legitimate"" Difficulty=""6""/><Word Text=""instructor"" Difficulty=""6""/><Word Text=""nomination"" Difficulty=""6""/><Word Text=""permission"" Difficulty=""6""/><Word Text=""physically"" Difficulty=""6""/><Word Text=""repeatedly"" Difficulty=""6""/><Word Text=""impressive"" Difficulty=""6""/><Word Text=""competitor"" Difficulty=""6""/><Word Text=""subsequent"" Difficulty=""6""/><Word Text=""widespread"" Difficulty=""6""/><Word Text=""occupation"" Difficulty=""6""/><Word Text=""collective"" Difficulty=""6""/><Word Text=""indication"" Difficulty=""6""/><Word Text=""hypothesis"" Difficulty=""6""/><Word Text=""adolescent"" Difficulty=""6""/><Word Text=""concerning"" Difficulty=""6""/><Word Text=""counseling"" Difficulty=""6""/><Word Text=""acceptable"" Difficulty=""6""/><Word Text=""continuous"" Difficulty=""6""/><Word Text=""presidency"" Difficulty=""6""/><Word Text=""acceptance"" Difficulty=""6""/><Word Text=""excitement"" Difficulty=""6""/><Word Text=""occasional"" Difficulty=""6""/><Word Text=""allegation"" Difficulty=""6""/><Word Text=""mainstream"" Difficulty=""6""/><Word Text=""inevitable"" Difficulty=""6""/><Word Text=""unexpected"" Difficulty=""6""/><Word Text=""facilitate"" Difficulty=""6""/><Word Text=""inspection"" Difficulty=""6""/><Word Text=""supervisor"" Difficulty=""6""/><Word Text=""possession"" Difficulty=""6""/><Word Text=""prediction"" Difficulty=""6""/><Word Text=""continuing"" Difficulty=""6""/><Word Text=""innovation"" Difficulty=""6""/><Word Text=""administer"" Difficulty=""6""/><Word Text=""indigenous"" Difficulty=""6""/><Word Text=""separation"" Difficulty=""6""/><Word Text=""enthusiasm"" Difficulty=""6""/><Word Text=""wilderness"" Difficulty=""6""/><Word Text=""mechanical"" Difficulty=""6""/><Word Text=""astronomer"" Difficulty=""6""/><Word Text=""corruption"" Difficulty=""6""/><Word Text=""contractor"" Difficulty=""6""/><Word Text=""compromise"" Difficulty=""6""/><Word Text=""behavioral"" Difficulty=""6""/><Word Text=""complexity"" Difficulty=""6""/><Word Text=""meaningful"" Difficulty=""6""/><Word Text=""electrical"" Difficulty=""6""/><Word Text=""attraction"" Difficulty=""6""/><Word Text=""altogether"" Difficulty=""6""/><Word Text=""engagement"" Difficulty=""6""/><Word Text=""structural"" Difficulty=""6""/><Word Text=""accounting"" Difficulty=""6""/><Word Text=""regulatory"" Difficulty=""6""/><Word Text=""diplomatic"" Difficulty=""6""/><Word Text=""prevention"" Difficulty=""6""/><Word Text=""productive"" Difficulty=""6""/><Word Text=""popularity"" Difficulty=""6""/><Word Text=""artificial"" Difficulty=""6""/><Word Text=""processing"" Difficulty=""6""/><Word Text=""ridiculous"" Difficulty=""6""/><Word Text=""invitation"" Difficulty=""6""/><Word Text=""officially"" Difficulty=""6""/><Word Text=""mysterious"" Difficulty=""6""/><Word Text=""protective"" Difficulty=""6""/><Word Text=""specialize"" Difficulty=""6""/><Word Text=""associated"" Difficulty=""6""/><Word Text=""withdrawal"" Difficulty=""6""/><Word Text=""thoroughly"" Difficulty=""6""/><Word Text=""optimistic"" Difficulty=""6""/><Word Text=""bankruptcy"" Difficulty=""6""/><Word Text=""revelation"" Difficulty=""6""/><Word Text=""discourage"" Difficulty=""6""/><Word Text=""conspiracy"" Difficulty=""6""/><Word Text=""functional"" Difficulty=""6""/><Word Text=""manipulate"" Difficulty=""6""/><Word Text=""earthquake"" Difficulty=""6""/><Word Text=""creativity"" Difficulty=""6""/><Word Text=""underlying"" Difficulty=""6""/><Word Text=""incredibly"" Difficulty=""6""/><Word Text=""presumably"" Difficulty=""6""/><Word Text=""equivalent"" Difficulty=""6""/><Word Text=""short-term"" Difficulty=""6""/><Word Text=""accessible"" Difficulty=""6""/><Word Text=""grandchild"" Difficulty=""6""/><Word Text=""reportedly"" Difficulty=""6""/><Word Text=""well-known"" Difficulty=""6""/><Word Text=""ecological"" Difficulty=""6""/><Word Text=""attendance"" Difficulty=""6""/><Word Text=""innovative"" Difficulty=""6""/><Word Text=""ambassador"" Difficulty=""6""/><Word Text=""supportive"" Difficulty=""6""/><Word Text=""aggression"" Difficulty=""6""/><Word Text=""journalism"" Difficulty=""6""/><Word Text=""well-being"" Difficulty=""6""/><Word Text=""compliance"" Difficulty=""6""/><Word Text=""supposedly"" Difficulty=""6""/><Word Text=""two-thirds"" Difficulty=""6""/><Word Text=""harassment"" Difficulty=""6""/><Word Text=""likelihood"" Difficulty=""6""/><Word Text=""suspicious"" Difficulty=""6""/><Word Text=""wheelchair"" Difficulty=""6""/><Word Text=""legislator"" Difficulty=""6""/><Word Text=""conception"" Difficulty=""6""/><Word Text=""comparable"" Difficulty=""6""/><Word Text=""conscience"" Difficulty=""6""/><Word Text=""inevitably"" Difficulty=""6""/><Word Text=""constraint"" Difficulty=""6""/><Word Text=""expedition"" Difficulty=""6""/><Word Text=""compromise"" Difficulty=""6""/><Word Text=""similarity"" Difficulty=""6""/><Word Text=""conversion"" Difficulty=""6""/><Word Text=""projection"" Difficulty=""6""/><Word Text=""graduation"" Difficulty=""6""/><Word Text=""integrated"" Difficulty=""6""/><Word Text=""ironically"" Difficulty=""6""/><Word Text=""confession"" Difficulty=""6""/><Word Text=""disturbing"" Difficulty=""6""/><Word Text=""technician"" Difficulty=""6""/><Word Text=""republican"" Difficulty=""6""/><Word Text=""coordinate"" Difficulty=""6""/><Word Text=""articulate"" Difficulty=""6""/><Word Text=""accusation"" Difficulty=""6""/><Word Text=""photograph"" Difficulty=""6""/><Word Text=""straighten"" Difficulty=""6""/><Word Text=""compelling"" Difficulty=""6""/><Word Text=""accurately"" Difficulty=""6""/><Word Text=""missionary"" Difficulty=""6""/><Word Text=""accelerate"" Difficulty=""6""/><Word Text=""nationwide"" Difficulty=""6""/><Word Text=""stereotype"" Difficulty=""6""/><Word Text=""information"" Difficulty=""6""/><Word Text=""development"" Difficulty=""6""/><Word Text=""opportunity"" Difficulty=""6""/><Word Text=""performance"" Difficulty=""6""/><Word Text=""significant"" Difficulty=""6""/><Word Text=""environment"" Difficulty=""6""/><Word Text=""traditional"" Difficulty=""6""/><Word Text=""institution"" Difficulty=""6""/><Word Text=""interesting"" Difficulty=""6""/><Word Text=""participant"" Difficulty=""6""/><Word Text=""immediately"" Difficulty=""6""/><Word Text=""possibility"" Difficulty=""6""/><Word Text=""independent"" Difficulty=""6""/><Word Text=""competition"" Difficulty=""6""/><Word Text=""responsible"" Difficulty=""6""/><Word Text=""demonstrate"" Difficulty=""6""/><Word Text=""perspective"" Difficulty=""6""/><Word Text=""participate"" Difficulty=""6""/><Word Text=""appropriate"" Difficulty=""6""/><Word Text=""application"" Difficulty=""6""/><Word Text=""instruction"" Difficulty=""6""/><Word Text=""educational"" Difficulty=""6""/><Word Text=""temperature"" Difficulty=""6""/><Word Text=""consequence"" Difficulty=""6""/><Word Text=""acknowledge"" Difficulty=""6""/><Word Text=""comfortable"" Difficulty=""6""/><Word Text=""investigate"" Difficulty=""6""/><Word Text=""combination"" Difficulty=""6""/><Word Text=""requirement"" Difficulty=""6""/><Word Text=""expectation"" Difficulty=""6""/><Word Text=""improvement"" Difficulty=""6""/><Word Text=""necessarily"" Difficulty=""6""/><Word Text=""legislation"" Difficulty=""6""/><Word Text=""achievement"" Difficulty=""6""/><Word Text=""explanation"" Difficulty=""6""/><Word Text=""alternative"" Difficulty=""6""/><Word Text=""interaction"" Difficulty=""6""/><Word Text=""personality"" Difficulty=""6""/><Word Text=""association"" Difficulty=""6""/><Word Text=""observation"" Difficulty=""6""/><Word Text=""description"" Difficulty=""6""/><Word Text=""negotiation"" Difficulty=""6""/><Word Text=""essentially"" Difficulty=""6""/><Word Text=""enforcement"" Difficulty=""6""/><Word Text=""competitive"" Difficulty=""6""/><Word Text=""involvement"" Difficulty=""6""/><Word Text=""fundamental"" Difficulty=""6""/><Word Text=""arrangement"" Difficulty=""6""/><Word Text=""concentrate"" Difficulty=""6""/><Word Text=""engineering"" Difficulty=""6""/><Word Text=""immigration"" Difficulty=""6""/><Word Text=""implication"" Difficulty=""6""/><Word Text=""recognition"" Difficulty=""6""/><Word Text=""publication"" Difficulty=""6""/><Word Text=""grandmother"" Difficulty=""6""/><Word Text=""preparation"" Difficulty=""6""/><Word Text=""Palestinian"" Difficulty=""6""/><Word Text=""substantial"" Difficulty=""6""/><Word Text=""effectively"" Difficulty=""6""/><Word Text=""alternative"" Difficulty=""6""/><Word Text=""incorporate"" Difficulty=""6""/><Word Text=""corporation"" Difficulty=""6""/><Word Text=""advertising"" Difficulty=""6""/><Word Text=""cooperation"" Difficulty=""6""/><Word Text=""communicate"" Difficulty=""6""/><Word Text=""destruction"" Difficulty=""6""/><Word Text=""electricity"" Difficulty=""6""/><Word Text=""complicated"" Difficulty=""6""/><Word Text=""potentially"" Difficulty=""6""/><Word Text=""politically"" Difficulty=""6""/><Word Text=""controversy"" Difficulty=""6""/><Word Text=""imagination"" Difficulty=""6""/><Word Text=""partnership"" Difficulty=""6""/><Word Text=""distinction"" Difficulty=""6""/><Word Text=""grandfather"" Difficulty=""6""/><Word Text=""celebration"" Difficulty=""6""/><Word Text=""composition"" Difficulty=""6""/><Word Text=""appointment"" Difficulty=""6""/><Word Text=""scholarship"" Difficulty=""6""/><Word Text=""reservation"" Difficulty=""6""/><Word Text=""maintenance"" Difficulty=""6""/><Word Text=""examination"" Difficulty=""6""/><Word Text=""cholesterol"" Difficulty=""6""/><Word Text=""restriction"" Difficulty=""6""/><Word Text=""Palestinian"" Difficulty=""6""/><Word Text=""differently"" Difficulty=""6""/><Word Text=""consumption"" Difficulty=""6""/><Word Text=""orientation"" Difficulty=""6""/><Word Text=""furthermore"" Difficulty=""6""/><Word Text=""measurement"" Difficulty=""6""/><Word Text=""frustration"" Difficulty=""6""/><Word Text=""distinguish"" Difficulty=""6""/><Word Text=""nonetheless"" Difficulty=""6""/><Word Text=""quarterback"" Difficulty=""6""/><Word Text=""anniversary"" Difficulty=""6""/><Word Text=""correlation"" Difficulty=""6""/><Word Text=""legislative"" Difficulty=""6""/><Word Text=""integration"" Difficulty=""6""/><Word Text=""prosecution"" Difficulty=""6""/><Word Text=""replacement"" Difficulty=""6""/><Word Text=""intelligent"" Difficulty=""6""/><Word Text=""accommodate"" Difficulty=""6""/><Word Text=""uncertainty"" Difficulty=""6""/><Word Text=""theoretical"" Difficulty=""6""/><Word Text=""transaction"" Difficulty=""6""/><Word Text=""counterpart"" Difficulty=""6""/><Word Text=""residential"" Difficulty=""6""/><Word Text=""businessman"" Difficulty=""6""/><Word Text=""acquisition"" Difficulty=""6""/><Word Text=""destination"" Difficulty=""6""/><Word Text=""exploration"" Difficulty=""6""/><Word Text=""practically"" Difficulty=""6""/><Word Text=""photography"" Difficulty=""6""/><Word Text=""outstanding"" Difficulty=""6""/><Word Text=""credibility"" Difficulty=""6""/><Word Text=""inspiration"" Difficulty=""6""/><Word Text=""agriculture"" Difficulty=""6""/><Word Text=""willingness"" Difficulty=""6""/><Word Text=""spectacular"" Difficulty=""6""/><Word Text=""reliability"" Difficulty=""6""/><Word Text=""fascinating"" Difficulty=""6""/><Word Text=""coordinator"" Difficulty=""6""/><Word Text=""ideological"" Difficulty=""6""/><Word Text=""documentary"" Difficulty=""6""/><Word Text=""progressive"" Difficulty=""6""/><Word Text=""self-esteem"" Difficulty=""6""/><Word Text=""cooperative"" Difficulty=""6""/><Word Text=""mathematics"" Difficulty=""6""/><Word Text=""influential"" Difficulty=""6""/><Word Text=""translation"" Difficulty=""6""/><Word Text=""statistical"" Difficulty=""6""/><Word Text=""fortunately"" Difficulty=""6""/><Word Text=""flexibility"" Difficulty=""6""/><Word Text=""experienced"" Difficulty=""6""/><Word Text=""legislature"" Difficulty=""6""/><Word Text=""encouraging"" Difficulty=""6""/><Word Text=""surrounding"" Difficulty=""6""/><Word Text=""preliminary"" Difficulty=""6""/><Word Text=""speculation"" Difficulty=""6""/><Word Text=""desperately"" Difficulty=""6""/><Word Text=""sensitivity"" Difficulty=""6""/><Word Text=""exclusively"" Difficulty=""6""/><Word Text=""marketplace"" Difficulty=""6""/><Word Text=""embarrassed"" Difficulty=""6""/><Word Text=""demographic"" Difficulty=""6""/><Word Text=""programming"" Difficulty=""6""/><Word Text=""shareholder"" Difficulty=""6""/><Word Text=""calculation"" Difficulty=""6""/><Word Text=""emotionally"" Difficulty=""6""/><Word Text=""grandparent"" Difficulty=""6""/><Word Text=""supermarket"" Difficulty=""6""/><Word Text=""distinctive"" Difficulty=""6""/><Word Text=""theological"" Difficulty=""6""/><Word Text=""contemplate"" Difficulty=""6""/><Word Text=""devastating"" Difficulty=""6""/><Word Text=""neighboring"" Difficulty=""6""/><Word Text=""consecutive"" Difficulty=""6""/><Word Text=""citizenship"" Difficulty=""6""/><Word Text=""sovereignty"" Difficulty=""6""/><Word Text=""contributor"" Difficulty=""6""/><Word Text=""importantly"" Difficulty=""6""/><Word Text=""electronics"" Difficulty=""6""/><Word Text=""convenience"" Difficulty=""6""/><Word Text=""sustainable"" Difficulty=""6""/><Word Text=""relationship"" Difficulty=""6""/><Word Text=""organization"" Difficulty=""6""/><Word Text=""particularly"" Difficulty=""6""/><Word Text=""professional"" Difficulty=""6""/><Word Text=""neighborhood"" Difficulty=""6""/><Word Text=""conversation"" Difficulty=""6""/><Word Text=""construction"" Difficulty=""6""/><Word Text=""intelligence"" Difficulty=""6""/><Word Text=""increasingly"" Difficulty=""6""/><Word Text=""presidential"" Difficulty=""6""/><Word Text=""contribution"" Difficulty=""6""/><Word Text=""circumstance"" Difficulty=""6""/><Word Text=""conservative"" Difficulty=""6""/><Word Text=""intervention"" Difficulty=""6""/><Word Text=""contemporary"" Difficulty=""6""/><Word Text=""specifically"" Difficulty=""6""/><Word Text=""manufacturer"" Difficulty=""6""/><Word Text=""investigator"" Difficulty=""6""/><Word Text=""independence"" Difficulty=""6""/><Word Text=""championship"" Difficulty=""6""/><Word Text=""distribution"" Difficulty=""6""/><Word Text=""occasionally"" Difficulty=""6""/><Word Text=""conventional"" Difficulty=""6""/><Word Text=""professional"" Difficulty=""6""/><Word Text=""nevertheless"" Difficulty=""6""/><Word Text=""conservative"" Difficulty=""6""/><Word Text=""satisfaction"" Difficulty=""6""/><Word Text=""considerable"" Difficulty=""6""/><Word Text=""intellectual"" Difficulty=""6""/><Word Text=""headquarters"" Difficulty=""6""/><Word Text=""characterize"" Difficulty=""6""/><Word Text=""presentation"" Difficulty=""6""/><Word Text=""introduction"" Difficulty=""6""/><Word Text=""significance"" Difficulty=""6""/><Word Text=""psychologist"" Difficulty=""6""/><Word Text=""photographer"" Difficulty=""6""/><Word Text=""agricultural"" Difficulty=""6""/><Word Text=""successfully"" Difficulty=""6""/><Word Text=""prescription"" Difficulty=""6""/><Word Text=""dramatically"" Difficulty=""6""/><Word Text=""surprisingly"" Difficulty=""6""/><Word Text=""experimental"" Difficulty=""6""/><Word Text=""unemployment"" Difficulty=""6""/><Word Text=""civilization"" Difficulty=""6""/><Word Text=""architecture"" Difficulty=""6""/><Word Text=""overwhelming"" Difficulty=""6""/><Word Text=""consistently"" Difficulty=""6""/><Word Text=""announcement"" Difficulty=""6""/><Word Text=""transmission"" Difficulty=""6""/><Word Text=""respectively"" Difficulty=""6""/><Word Text=""compensation"" Difficulty=""6""/><Word Text=""historically"" Difficulty=""6""/><Word Text=""carbohydrate"" Difficulty=""6""/><Word Text=""disappointed"" Difficulty=""6""/><Word Text=""refrigerator"" Difficulty=""6""/><Word Text=""productivity"" Difficulty=""6""/><Word Text=""practitioner"" Difficulty=""6""/><Word Text=""entrepreneur"" Difficulty=""6""/><Word Text=""Christianity"" Difficulty=""6""/><Word Text=""metropolitan"" Difficulty=""6""/><Word Text=""appreciation"" Difficulty=""6""/><Word Text=""commissioner"" Difficulty=""6""/><Word Text=""consequently"" Difficulty=""6""/><Word Text=""conservation"" Difficulty=""6""/><Word Text=""installation"" Difficulty=""6""/><Word Text=""constitution"" Difficulty=""6""/><Word Text=""Thanksgiving"" Difficulty=""6""/><Word Text=""deliberately"" Difficulty=""6""/><Word Text=""intellectual"" Difficulty=""6""/><Word Text=""considerably"" Difficulty=""6""/><Word Text=""jurisdiction"" Difficulty=""6""/><Word Text=""availability"" Difficulty=""6""/><Word Text=""surveillance"" Difficulty=""6""/><Word Text=""economically"" Difficulty=""6""/><Word Text=""middle-class"" Difficulty=""6""/><Word Text=""international"" Difficulty=""6""/><Word Text=""environmental"" Difficulty=""6""/><Word Text=""investigation"" Difficulty=""6""/><Word Text=""communication"" Difficulty=""6""/><Word Text=""understanding"" Difficulty=""6""/><Word Text=""significantly"" Difficulty=""6""/><Word Text=""unfortunately"" Difficulty=""6""/><Word Text=""participation"" Difficulty=""6""/><Word Text=""congressional"" Difficulty=""6""/><Word Text=""entertainment"" Difficulty=""6""/><Word Text=""psychological"" Difficulty=""6""/><Word Text=""approximately"" Difficulty=""6""/><Word Text=""administrator"" Difficulty=""6""/><Word Text=""consideration"" Difficulty=""6""/><Word Text=""extraordinary"" Difficulty=""6""/><Word Text=""concentration"" Difficulty=""6""/><Word Text=""establishment"" Difficulty=""6""/><Word Text=""comprehensive"" Difficulty=""6""/><Word Text=""correspondent"" Difficulty=""6""/><Word Text=""sophisticated"" Difficulty=""6""/><Word Text=""controversial"" Difficulty=""6""/><Word Text=""demonstration"" Difficulty=""6""/><Word Text=""manufacturing"" Difficulty=""6""/><Word Text=""institutional"" Difficulty=""6""/><Word Text=""consciousness"" Difficulty=""6""/><Word Text=""effectiveness"" Difficulty=""6""/><Word Text=""questionnaire"" Difficulty=""6""/><Word Text=""uncomfortable"" Difficulty=""6""/><Word Text=""traditionally"" Difficulty=""6""/><Word Text=""technological"" Difficulty=""6""/><Word Text=""determination"" Difficulty=""6""/><Word Text=""automatically"" Difficulty=""6""/><Word Text=""revolutionary"" Difficulty=""6""/><Word Text=""collaboration"" Difficulty=""6""/><Word Text=""instructional"" Difficulty=""6""/><Word Text=""unprecedented"" Difficulty=""6""/><Word Text=""confrontation"" Difficulty=""6""/><Word Text=""developmental"" Difficulty=""6""/><Word Text=""philosophical"" Difficulty=""6""/><Word Text=""old-fashioned"" Difficulty=""6""/><Word Text=""undergraduate"" Difficulty=""6""/><Word Text=""substantially"" Difficulty=""6""/><Word Text=""administration"" Difficulty=""6""/><Word Text=""responsibility"" Difficulty=""6""/><Word Text=""representative"" Difficulty=""6""/><Word Text=""characteristic"" Difficulty=""6""/><Word Text=""transportation"" Difficulty=""6""/><Word Text=""interpretation"" Difficulty=""6""/><Word Text=""constitutional"" Difficulty=""6""/><Word Text=""recommendation"" Difficulty=""6""/><Word Text=""representation"" Difficulty=""6""/><Word Text=""discrimination"" Difficulty=""6""/><Word Text=""identification"" Difficulty=""6""/><Word Text=""transformation"" Difficulty=""6""/><Word Text=""administrative"" Difficulty=""6""/><Word Text=""implementation"" Difficulty=""6""/><Word Text=""infrastructure"" Difficulty=""6""/><Word Text=""simultaneously"" Difficulty=""6""/><Word Text=""representative"" Difficulty=""6""/><Word Text=""accomplishment"" Difficulty=""6""/><Word Text=""organizational"" Difficulty=""6""/><Word Text=""disappointment"" Difficulty=""6""/><Word Text=""rehabilitation"" Difficulty=""6""/><Word Text=""accountability"" Difficulty=""6""/><Word Text=""African-American"" Difficulty=""6""/></Words><Sentences><Length Difficulty=""0"" Min=""0"" Max=""3""/><Length Difficulty=""1"" Min=""3"" Max=""6""/><Length Difficulty=""2"" Min=""6"" Max=""10""/><Length Difficulty=""3"" Min=""10"" Max=""14""/><Length Difficulty=""4"" Min=""14"" Max=""20""/><Length Difficulty=""5"" Min=""20"" Max=""100""/></Sentences></Model>"
		});

		// Add Socials

		SeedSocials(context);

		SeedStaticStringsAndSettings(context, questionAnswers["gamename"], cancreateclanprog, oncreateclanprog,
			itemGroup);

		// Add Non-Cardinal Cell Exits
		context.NonCardinalExitTemplates.Add(new NonCardinalExitTemplate
		{
			Name = "Enter",
			OriginOutboundPreface = "in towards",
			OriginInboundPreface = "out from",
			DestinationOutboundPreface = "out towards",
			DestinationInboundPreface = "in from",
			OutboundVerb = "enter",
			InboundVerb = "leave"
		});
		context.NonCardinalExitTemplates.Add(new NonCardinalExitTemplate
		{
			Name = "Leave",
			OriginOutboundPreface = "out towards",
			OriginInboundPreface = "in from",
			DestinationOutboundPreface = "in towards",
			DestinationInboundPreface = "out from",
			OutboundVerb = "leave",
			InboundVerb = "enter"
		});
		context.NonCardinalExitTemplates.Add(new NonCardinalExitTemplate
		{
			Name = "Climb",
			OriginOutboundPreface = "up towards",
			OriginInboundPreface = "down from",
			DestinationOutboundPreface = "down towards",
			DestinationInboundPreface = "up from",
			OutboundVerb = "climb",
			InboundVerb = "descend"
		});
		context.NonCardinalExitTemplates.Add(new NonCardinalExitTemplate
		{
			Name = "Descend",
			OriginOutboundPreface = "down towards",
			OriginInboundPreface = "up from",
			DestinationOutboundPreface = "up towards",
			DestinationInboundPreface = "down from",
			OutboundVerb = "descend",
			InboundVerb = "climb"
		});
		context.NonCardinalExitTemplates.Add(new NonCardinalExitTemplate
		{
			Name = "StairsUp",
			OriginOutboundPreface = "up the stairs towards",
			OriginInboundPreface = "down the stairs from",
			DestinationOutboundPreface = "down the stairs towards",
			DestinationInboundPreface = "up the stairs from",
			OutboundVerb = "climb",
			InboundVerb = "descend"
		});
		context.NonCardinalExitTemplates.Add(new NonCardinalExitTemplate
		{
			Name = "StairsDown",
			OriginOutboundPreface = "down the stairs towards",
			OriginInboundPreface = "up the stairs from",
			DestinationOutboundPreface = "up the stairs towards",
			DestinationInboundPreface = "down the stairs from",
			OutboundVerb = "descend",
			InboundVerb = "climb"
		});

		context.SaveChanges();

		// Add guest lounge
		var package = new CellOverlayPackage
		{
			Name = "CoreData Autogenerated",
			Id = 1,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			}
		};
		context.CellOverlayPackages.Add(package);

		var skyTemplate = new SkyDescriptionTemplate
		{
			Id = 1,
			Name = "OOC Sky"
		};
		skyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = skyTemplate,
			LowerBound = -1000,
			UpperBound = 1000,
			Description = "There is nothing at all distinct or unique about the sky at this time."
		});
		context.SkyDescriptionTemplates.Add(skyTemplate);

		var earthSkyTemplate = new SkyDescriptionTemplate
		{
			Id = 2,
			Name = "Earth's Sky"
		};
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 0,
			UpperBound = 10,
			Description = "The sky is filled with light, no stars are visible."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 10,
			UpperBound = 16,
			Description = "The sky is filled with light. Only the very brightest of stars are visible."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 16,
			UpperBound = 17.8,
			Description = "The sky is bright and hazy with light. There are but a few stars visible."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 17.8,
			UpperBound = 18.38,
			Description = "The sky is bright and hazy with light. Some few stars and constellations are visible."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 18.38,
			UpperBound = 19.5,
			Description =
				"The sky is filled with a dim, hazy light towards the horizon. The milky way can be very faintly observed towards the zenith of the sky, and many stars are visible."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 19.5,
			UpperBound = 20.49,
			Description =
				"The sky is relatively dark except for a faint hazy glow at the horizons. The milky way can be observed cutting across the majority of the night sky except at the horizons. The Andromeda galaxy is visible and a great many stars and constellations can be observed."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 20.49,
			UpperBound = 21.51,
			Description =
				"The sky is brilliant, dominated by the milky way overhead and filled with many stars. The Andromeda galaxy is clearly visible and only the faintest glow on the horizon spoils the view of the cosmos."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 21.51,
			UpperBound = 21.89,
			Description =
				"The sky is crowded with countless stars, extending to the horizon in every direction. The milky way dominates the sky, with dark, visible lanes and a noticable bulge in the centre. The Andromeda and Triangulum galaxies are both easily visible to the naked eye. Only a very faint light is visible near to the horizons."
		});
		earthSkyTemplate.SkyDescriptionTemplatesValues.Add(new SkyDescriptionTemplatesValue
		{
			SkyDescriptionTemplate = earthSkyTemplate,
			LowerBound = 21.89,
			UpperBound = 22,
			Description =
				"The sky is crowded with countless stars, extending to the horizon in every direction. The milky way dominates the sky, with dark, visible lanes and a noticable bulge in the centre. The Andromeda and Triangulum galaxies are both easily visible to the naked eye."
		});
		context.SkyDescriptionTemplates.Add(earthSkyTemplate);

		var shard = new Shard
		{
			Name = "OOC",
			Id = 1,
			MinimumTerrestrialLux = 0,
			SphericalRadiusMetres = 6371000,
			SkyDescriptionTemplate = skyTemplate
		};
		context.Shards.Add(shard);


		var zone = new Zone
		{
			Name = "OOC",
			Id = 1,
			AmbientLightPollution = 0,
			Shard = shard,
			Elevation = 0,
			Latitude = 0,
			Longitude = 0
		};

		var room = new Room
		{
			Id = 1,
			X = 0,
			Y = 0,
			Z = 0,
			Zone = zone
		};
		context.Rooms.Add(room);

		var terrain = new Terrain
		{
			Id = 1,
			Name = "Void",
			HideDifficulty = 0,
			SpotDifficulty = 0,
			InfectionType = 0,
			InfectionVirulence = 0,
			InfectionMultiplier = 0,
			StaminaCost = 0,
			TerrainEditorColour = "#FFFFFFFF",
			TerrainBehaviourMode = "outdoors",
			DefaultTerrain = true,
		};
		context.Terrains.Add(terrain);

		var cell = new Cell
		{
			Id = 1,
			Room = room,
			Temporary = false,
			EffectData = "<Effects/>"
		};
		context.Cells.Add(cell);
		context.SaveChanges();

		var overlay = new CellOverlay
		{
			Id = 1,
			Name = "CoreData Autogenerated",
			CellName = "The Guest Lounge",
			CellDescription =
				"This is the guest lounge, where guests to the MUD can log in and chat with guides, administrators or each other as well as test out features of the engine before their first character is approved.",
			CellOverlayPackage = package,
			Terrain = terrain,
			AddedLight = 0,
			HearingProfile = hearing,
			OutdoorsType = 0,
			AmbientLightFactor = 1.0,
			CellId = cell.Id
		};
		context.CellOverlays.Add(overlay);

		cell.CurrentOverlay = overlay;

		context.SaveChanges();

		#region Relative Height
		var chardef = new CharacteristicDefinition
		{
			Name = "Relative Height",
			Type = 0,
			Pattern = "^height",
			Description = "Height descriptor relative to the viewer",
			Model = "standard"
		};
		context.CharacteristicDefinitions.Add(chardef);

		var nextId = 1L;
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "",
			Definition = chardef,
			Value = "0.9",
			Default = true,
			AdditionalValue = "1.1",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "miniscule",
			Definition = chardef,
			Value = "0",
			Default = false,
			AdditionalValue = "0.25",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "tiny",
			Definition = chardef,
			Value = "0.25",
			Default = false,
			AdditionalValue = "0.5",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "very short",
			Definition = chardef,
			Value = "0.5",
			Default = false,
			AdditionalValue = "0.7",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "short",
			Definition = chardef,
			Value = "0.7",
			Default = false,
			AdditionalValue = "0.9",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "tall",
			Definition = chardef,
			Value = "1.1",
			Default = false,
			AdditionalValue = "1.2",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "very tall",
			Definition = chardef,
			Value = "1.2",
			Default = false,
			AdditionalValue = "1.4",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "gigantic",
			Definition = chardef,
			Value = "1.4",
			Default = false,
			AdditionalValue = "3",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "colossal",
			Definition = chardef,
			Value = "3",
			Default = false,
			AdditionalValue = "8",
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++,
			Name = "titanic",
			Definition = chardef,
			Value = "8",
			Default = false,
			AdditionalValue = "100000",
			Pluralisation = 0
		});

		context.SaveChanges();
		#endregion

		SeedUnitsOfMeasure(context);
		context.SaveChanges();
		SeedColours(context);

		var itemHS = new HealthStrategy
		{
			Name = "Item Default",
			Type = "GameItem",
			Definition = @"<Definition>
   <LodgeDamageExpression>(damage - 10) / 2</LodgeDamageExpression>
   <SeverityRanges>
	 <Severity value=""0"" lower=""-1"" upper=""0""/>
	 <Severity value=""1"" lower=""0"" upper=""2""/>
	 <Severity value=""2"" lower=""2"" upper=""4""/>
	 <Severity value=""3"" lower=""4"" upper=""7""/>
	 <Severity value=""4"" lower=""7"" upper=""12""/>
	 <Severity value=""5"" lower=""12"" upper=""18""/>
	 <Severity value=""6"" lower=""18"" upper=""27""/>
	 <Severity value=""7"" lower=""27"" upper=""40""/>
	 <Severity value=""8"" lower=""40"" upper=""100""/>
   </SeverityRanges>
 </Definition>"
		};
		context.HealthStrategies.Add(itemHS);
		context.SaveChanges();
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "DefaultItemHealthStrategy",
			Definition = itemHS.Id.ToString()
		});
		context.SaveChanges();

		context.Database.CommitTransaction();
		return "Package successfully applied.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (context.Accounts.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("gamename", "What is the name of the MUD which you are setting up? ", (context, answers) => true,
				(text, context) =>
				{
					if (text.Length < 2) return (false, "You must enter an name with at least 2 characters.");

					return (true, string.Empty);
				}),
			("account", "What name do you want to use for your implementor account? ", (context, answers) => true,
				(text, context) =>
				{
					if (text.Length < 2) return (false, "You must enter an account name with at least 2 characters.");

					return (true, string.Empty);
				}),
			("password", "What password do you want to use for your implementor account? ", (context, answers) => true,
				(text, context) =>
				{
					if (text.Length < 8) return (false, "Your password must be at least 8 characters long.");

					return (true, string.Empty);
				}),
			("email", "What email address do you want to use for your implementor account? ",
				(context, answers) => true, (text, context) =>
				{
					if (!EmailRegex.IsMatch(text)) return (false, "That is not a valid email address.");

					return (true, string.Empty);
				})
		};

	public int SortOrder => 0;
	public string Name => "Core";

	public string Tagline =>
		"Should be the first package that all MUDs import except for very advanced users in specific circumstances";

	public string FullDescription =>
		@"This package contains some of the core data that must be included in all implementations of FutureMUD. Generally speaking, this should be the very first file that you import on all implementations before any other package. 

Among many other small but necessary things, it does the following:

1) Sets up an account with maximum privileges according to your specification
2) Sets up some universal progs
3) Sets up some of the fixed item prototypes like corpses, currency and commodities
4) Sets up a guest lounge
5) Sets up some placeholder static strings such as menu layouts and echoes
6) Sets up player convenience items like real world cultures and timezones
7) Sets up materials, fluids and gases
8) Sets up socials
9) Sets up units of measure";

	private static void SeedUnitsOfMeasure(FuturemudDatabaseContext context)
	{
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 1,
			Name = "gram",
			PrimaryAbbreviation = "g",
			Abbreviations = "gram g grams",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 2,
			Name = "kilogram",
			PrimaryAbbreviation = "kg",
			Abbreviations = "kilogram kg kilo kilograms kilos",
			BaseMultiplier = 1000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 3,
			Name = "tonne",
			PrimaryAbbreviation = "t",
			Abbreviations = "tonne ton t mg tonnes tons",
			BaseMultiplier = 1000000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 4,
			Name = "pound",
			PrimaryAbbreviation = "lb",
			Abbreviations = "pound lb pounds lbs",
			BaseMultiplier = 453.592,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 5,
			Name = "ounce",
			PrimaryAbbreviation = "oz",
			Abbreviations = "ounce oz ounces",
			BaseMultiplier = 28.3495,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 6,
			Name = "centimetre",
			PrimaryAbbreviation = "cm",
			Abbreviations = "centimetre cm centimeter centimeters centimetres cms",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 7,
			Name = "metre",
			PrimaryAbbreviation = "m",
			Abbreviations = "metre meter m metres meters",
			BaseMultiplier = 100,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 8,
			Name = "millimetre",
			PrimaryAbbreviation = "mm",
			Abbreviations = "millimetre millimeter mm millimeters millimetres",
			BaseMultiplier = 0.1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 9,
			Name = "foot",
			PrimaryAbbreviation = "ft",
			Abbreviations = "foot feet ft '",
			BaseMultiplier = 30.48,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 10,
			Name = "inch",
			PrimaryAbbreviation = "in",
			Abbreviations = "inch inches in \"",
			BaseMultiplier = 2.54,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 11,
			Name = "tonne",
			PrimaryAbbreviation = "t",
			Abbreviations = "tonne ton t mg tonnes tons",
			BaseMultiplier = 907184.74,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 12,
			Name = "kilotonne",
			PrimaryAbbreviation = "kt",
			Abbreviations = "kilotonne kilotonnes kt",
			BaseMultiplier = 1000000000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 13,
			Name = "stone",
			PrimaryAbbreviation = "st",
			Abbreviations = "stone stones st",
			BaseMultiplier = 6350.29,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 14,
			Name = "chain",
			PrimaryAbbreviation = "ch",
			Abbreviations = "chain chains ch",
			BaseMultiplier = 2011.68,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 15,
			Name = "mile",
			PrimaryAbbreviation = "mi",
			Abbreviations = "mile miles",
			BaseMultiplier = 160934.4,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 16,
			Name = "kilometer",
			PrimaryAbbreviation = "km",
			Abbreviations = "kilometre kilometer kilometres kilometers km kms",
			BaseMultiplier = 100000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 17,
			Name = "furlong",
			PrimaryAbbreviation = "fl",
			Abbreviations = "furlong furlongs fur",
			BaseMultiplier = 20116.8,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 18,
			Name = "league",
			PrimaryAbbreviation = "lg",
			Abbreviations = "leagues league lea",
			BaseMultiplier = 482803.2,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 19,
			Name = "link",
			PrimaryAbbreviation = "lnk",
			Abbreviations = "link links",
			BaseMultiplier = 20.1168,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 20,
			Name = "rod",
			PrimaryAbbreviation = "rd",
			Abbreviations = "rod rods",
			BaseMultiplier = 502.92,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 1,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 21,
			Name = "litre",
			PrimaryAbbreviation = "l",
			Abbreviations = "litre liter l litres liters",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 22,
			Name = "millilitre",
			PrimaryAbbreviation = "ml",
			Abbreviations = "millilitre milliletres milliliter milliliters ml mls",
			BaseMultiplier = 0.001,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 23,
			Name = "ounce",
			PrimaryAbbreviation = "oz",
			Abbreviations = "ounce ounces floz oz",
			BaseMultiplier = 0.0295735,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 24,
			Name = "dram",
			PrimaryAbbreviation = "dr",
			Abbreviations = "dram drams",
			BaseMultiplier = 0.0036966912,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 25,
			Name = "cup",
			PrimaryAbbreviation = "cp",
			Abbreviations = "cup cups cp cps",
			BaseMultiplier = 0.236588,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 26,
			Name = "pint",
			PrimaryAbbreviation = "pt",
			Abbreviations = "pint pints p pt pts",
			BaseMultiplier = 0.473176,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 27,
			Name = "quart",
			PrimaryAbbreviation = "qt",
			Abbreviations = "quart quarts qt qts",
			BaseMultiplier = 0.946352946,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 28,
			Name = "gallon",
			PrimaryAbbreviation = "gal",
			Abbreviations = "gallon gallons gal gals",
			BaseMultiplier = 3.785411784,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 29,
			Name = "kilolitre",
			PrimaryAbbreviation = "kl",
			Abbreviations = "kilolitre kilolitres kiloliter kiloliters kl",
			BaseMultiplier = 1000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 30,
			Name = "megalitre",
			PrimaryAbbreviation = "ml",
			Abbreviations = "megalitre megalitres megaliter megaliters ml",
			BaseMultiplier = 1000000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 31,
			Name = "gigalitre",
			PrimaryAbbreviation = "gl",
			Abbreviations = "gigalitre gigalitres gigaliter gigaliters gl",
			BaseMultiplier = 1000000000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 2,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 32,
			Name = "square metre",
			PrimaryAbbreviation = "m2",
			Abbreviations = "sqm sqms m2",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 3,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 33,
			Name = "square foot",
			PrimaryAbbreviation = "ft2",
			Abbreviations = "sqft sqf f2 ft2",
			BaseMultiplier = 0.092903,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 3,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 34,
			Name = "acre",
			PrimaryAbbreviation = "ac",
			Abbreviations = "acre acres ac",
			BaseMultiplier = 4046.86,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 3,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 35,
			Name = "hectare",
			PrimaryAbbreviation = "ha",
			Abbreviations = "hectare hectares ha",
			BaseMultiplier = 10000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 3,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 36,
			Name = "cubic metre",
			PrimaryAbbreviation = "m3",
			Abbreviations = "cm m3 cms cbm cbms",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 4,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 37,
			Name = "cubic foot",
			PrimaryAbbreviation = "cuft",
			Abbreviations = "cft ft3 f3 cuft",
			BaseMultiplier = 0.0283168466,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 4,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 38,
			Name = "°C",
			PrimaryAbbreviation = "°C",
			Abbreviations = "°C C deg degree degrees degs",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 5,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 39,
			Name = "°F",
			PrimaryAbbreviation = "°F",
			Abbreviations = "°F F deg degree degrees degs",
			BaseMultiplier = 0.55555555555,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 32,
			Type = 5,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 40,
			Name = "perch",
			PrimaryAbbreviation = "per",
			Abbreviations = "perch",
			BaseMultiplier = 25.29285264,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 3,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 41,
			Name = "rood",
			PrimaryAbbreviation = "rood",
			Abbreviations = "rood",
			BaseMultiplier = 1011.7141056,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 3,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 42,
			Name = "acre",
			PrimaryAbbreviation = "ac",
			Abbreviations = "acre ac",
			BaseMultiplier = 4045.8564224,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 3,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 43,
			Name = "drachm",
			PrimaryAbbreviation = "dr",
			Abbreviations = "dr drachm drachms",
			BaseMultiplier = 1.7718451953125,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 44,
			Name = "grain",
			PrimaryAbbreviation = "ggr",
			Abbreviations = "grain gr grains",
			BaseMultiplier = 0.06479891,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 45,
			Name = "thou",
			PrimaryAbbreviation = "th",
			Abbreviations = "thou th thous",
			BaseMultiplier = 0.00254,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = false,
			SpaceBetween = true,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 46,
			Name = "milligram",
			PrimaryAbbreviation = "mg",
			Abbreviations = "milligram mg milligrams",
			BaseMultiplier = 0.001,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 0,
			Describer = true,
			SpaceBetween = true,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 47,
			Name = "°C",
			PrimaryAbbreviation = "°C",
			Abbreviations = "°C C deg degree degrees degs",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 6,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 48,
			Name = "°F",
			PrimaryAbbreviation = "°F",
			Abbreviations = "°F F deg degree degrees degs",
			BaseMultiplier = 0.55555555555,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 6,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 49,
			Name = "newton",
			PrimaryAbbreviation = "N",
			Abbreviations = "N newton newtons",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 7,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 50,
			Name = "kilonewton",
			PrimaryAbbreviation = "kN",
			Abbreviations = "kN kilonewtons kilonewton",
			BaseMultiplier = 1000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 7,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 51,
			Name = "meganewton",
			PrimaryAbbreviation = "MN",
			Abbreviations = "MN meganewtons meganewton",
			BaseMultiplier = 1000000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 7,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 52,
			Name = "pound",
			PrimaryAbbreviation = "lb",
			Abbreviations = "lb lbf lbs lbf pounds pound",
			BaseMultiplier = 4.448222,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 7,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 53,
			Name = "pascal",
			PrimaryAbbreviation = "pa",
			Abbreviations = "pa pascal pascals",
			BaseMultiplier = 1,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 54,
			Name = "kilopascal",
			PrimaryAbbreviation = "kpa",
			Abbreviations = "kpa kilopascal kilopascals",
			BaseMultiplier = 1000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 55,
			Name = "megapascal",
			PrimaryAbbreviation = "mpa",
			Abbreviations = "mpa megapascal megapascals",
			BaseMultiplier = 1000000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 56,
			Name = "gigapascal",
			PrimaryAbbreviation = "gpa",
			Abbreviations = "gpa gigapascal gigapascals",
			BaseMultiplier = 1000000000,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 57,
			Name = "psi",
			PrimaryAbbreviation = "psi",
			Abbreviations = "psi lb lbs pound pounds",
			BaseMultiplier = 6894.757293168,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 58,
			Name = "kpsi",
			PrimaryAbbreviation = "kpsi",
			Abbreviations = "kpsi klb klbs kilopound kilopounds",
			BaseMultiplier = 6894757.293168,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 59,
			Name = "mpsi",
			PrimaryAbbreviation = "mpsi",
			Abbreviations = "mpsi mlb mlbs megapound megapounds",
			BaseMultiplier = 6894757293.168,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 60,
			Name = "kpsi",
			PrimaryAbbreviation = "kpsi",
			Abbreviations = "kpsi klb klbs kilopound kilopounds",
			BaseMultiplier = 6894757.293168,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 8,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = false
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 61,
			Name = "kg/m\u00b2",
			PrimaryAbbreviation = "kg/m\u00b2",
			Abbreviations = "kg kgm kgms",
			BaseMultiplier = 1.0,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 9,
			Describer = true,
			SpaceBetween = false,
			System = "Metric",
			DefaultUnitForSystem = true
		});
		context.UnitsOfMeasure.Add(new UnitOfMeasure
		{
			Id = 62,
			Name = "lb/in\u00b2",
			PrimaryAbbreviation = "lb/in\u00b2",
			Abbreviations = "lb lbin lbin2 lbsqin",
			BaseMultiplier = 0.00142247510668563300142247510669,
			PreMultiplierBaseOffset = 0,
			PostMultiplierBaseOffset = 0,
			Type = 9,
			Describer = true,
			SpaceBetween = false,
			System = "Imperial",
			DefaultUnitForSystem = true
		});
	}

	private static void SeedSocials(FuturemudDatabaseContext context)
	{
		context.Socials.Add(new Social
		{
			Name = "bow", NoTargetEcho = "@ bow|bows", OneTargetEcho = "@ bow|bows to $1",
			DirectionTargetEcho = "@ bow|bows towards {0}", MultiTargetEcho = "@ bow|bows to {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "chuckle", NoTargetEcho = "@ chuckle|chuckles", OneTargetEcho = "@ chuckle|chuckles at $1",
			DirectionTargetEcho = null, MultiTargetEcho = "@ chuckle|chuckles at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "cough", NoTargetEcho = "@ cough|coughs", OneTargetEcho = "@ cough|coughs at $1",
			DirectionTargetEcho = "@ cough|coughs towards {0}", MultiTargetEcho = "@ cough|coughs at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "frown", NoTargetEcho = "@ frown|frowns", OneTargetEcho = "@ frown|frowns at $1",
			DirectionTargetEcho = "@ frown|frowns towards {0}", MultiTargetEcho = "@ frown|frowns at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "gan", NoTargetEcho = "@ grin|grins and nod|nods", OneTargetEcho = "@ grin|grins and nod|nods at $1",
			DirectionTargetEcho = null, MultiTargetEcho = "@ grin|grins and nod|nods to {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "gasp", NoTargetEcho = "@ gasp|gasps", OneTargetEcho = "@ gasp|gasps at $1",
			DirectionTargetEcho = "@ gasp|gasps towards {0}", MultiTargetEcho = "@ gasp|gasps at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "glare", NoTargetEcho = "@ glare|glares", OneTargetEcho = "@ glare|glares at $1",
			DirectionTargetEcho = "@ glare|glares towards {0}", MultiTargetEcho = "@ glare|glares at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "grin", NoTargetEcho = "@ grin|grins", OneTargetEcho = "@ grin|grins at $1",
			DirectionTargetEcho = null, MultiTargetEcho = "@ grin|grins at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "grunt", NoTargetEcho = "@ grunt|grunts", OneTargetEcho = "@ grunt|grunts at $1",
			DirectionTargetEcho = "@ grunt|grunts towards {0}", MultiTargetEcho = "@ grunt|grunts at {0} collectively"
		});
		context.Socials.Add(new Social
		{
			Name = "hug", NoTargetEcho = "", OneTargetEcho = "@ hug|hugs $1", DirectionTargetEcho = null,
			MultiTargetEcho = null
		});
		context.Socials.Add(new Social
		{
			Name = "kiss", NoTargetEcho = "", OneTargetEcho = "@ kiss|kisses $1", DirectionTargetEcho = null,
			MultiTargetEcho = null
		});
		context.Socials.Add(new Social
		{
			Name = "laugh", NoTargetEcho = "@ laugh|laughs", OneTargetEcho = "@ laugh|laughs at $1",
			DirectionTargetEcho = null, MultiTargetEcho = "@ laugh|laughs at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "nod", NoTargetEcho = "@ nod|nods", OneTargetEcho = "@ nod|nods to $1",
			DirectionTargetEcho = "@ nod|nods towards {0}", MultiTargetEcho = "@ nod|nods to {0} each in turn"
		});
		context.Socials.Add(new Social
		{
			Name = "pet", NoTargetEcho = "", OneTargetEcho = "@ pet|pets $1 affectionately", DirectionTargetEcho = null,
			MultiTargetEcho = null
		});
		context.Socials.Add(new Social
		{
			Name = "salute", NoTargetEcho = "@ salute|salutes", OneTargetEcho = "@ salute|salutes $1",
			DirectionTargetEcho = "@ salute|salutes towards {0}", MultiTargetEcho = "@ salute|salutes {0} each in turn"
		});
		context.Socials.Add(new Social
		{
			Name = "scoff", NoTargetEcho = "@ scoff|scoffs", OneTargetEcho = "@ scoff|scoffs at $1",
			DirectionTargetEcho = null, MultiTargetEcho = "@ scoff|scoffs at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "shake", NoTargetEcho = "@ shake|shakes &0's head", OneTargetEcho = "@ shake|shakes &0's head at $1",
			DirectionTargetEcho = null, MultiTargetEcho = "@ shake|shakes &0's head at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "shrug", NoTargetEcho = "@ shrug|shrugs", OneTargetEcho = "@ shrug|shrugs at $1",
			DirectionTargetEcho = "@ shrug|shrugs towards {0}", MultiTargetEcho = "@ shrug|shrugs at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "sigh", NoTargetEcho = "@ sigh|sighs", OneTargetEcho = "@ sigh|sighs at $1",
			DirectionTargetEcho = "@ sigh|sighs and gazes towards {0}", MultiTargetEcho = "@ sigh|sighs at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "smile", NoTargetEcho = "@ smile|smiles", OneTargetEcho = "@ smile|smiles at $1",
			DirectionTargetEcho = null, MultiTargetEcho = "@ smile|smiles at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "smirk", NoTargetEcho = "@ smirk|smirks", OneTargetEcho = "@ smirk|smirks at $1",
			DirectionTargetEcho = "@ smirk|smirks towards {0}", MultiTargetEcho = "@ smirk|smirks at {0}"
		});
		context.Socials.Add(new Social
		{
			Name = "wink", NoTargetEcho = "@ wink|winks", OneTargetEcho = "@ wink|winks at $1",
			DirectionTargetEcho = "@ wink|winks towards {0}", MultiTargetEcho = "@ wink|winks at {0}"
		});
	}

	private static void SeedStaticStringsAndSettings(FuturemudDatabaseContext context, string gameName,
		FutureProg cancreateclanprog, FutureProg oncreateclanprog, ItemGroup itemGroup)
	{
		// Add static strings
		foreach (var ss in DefaultStaticSettings.DefaultStaticStrings)
			context.StaticStrings.Add(new StaticString { Id = ss.Key, Text = ss.Value });

		context.StaticStrings.Add(new StaticString { Id = "CleanPromptStatusLine", Text = @"cleaning {0}{1}{2}" });
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountCulture", Text = @"#3Culture Settings#0

Culture settings define how dates, times and certain other values appear to you in the game. The most obvious example is with dates, where certain countries will display month/day whereas others will display day/month. These cultures are the exact same ones that you would use when setting up your operating system.

The engine has the following available cultures to choose from:

{0}

Please select the number of your desired culture, or enter the code for it directly if you know it: "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountEmail", Text = @"#3Email Address#0

Please enter a valid email address for your account. This email account will be used in the event that you need to use the password recovery service, and also may be used by staff to contact you. An account verification email will be sent to you at the listed address. Accounts that do not have a validated email address cannot create characters.

Your email address: "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountGMTOffset",
			Text = @"#3Timezone Settings#0

The engine will automatically convert any real-world time you encounter in game to your local Timezone. If you know the GMT Offset of your Timezone, please enter it here (e.g. -5 for US East Coast, -8 for US West Coast). Otherwise, enter ""unknown"" to be shown a list of all Timezones. 

Please note that this game uses timezones from the following list: 
#6http://en.wikipedia.org/wiki/List_of_tz_database_time_zones#0
			
Please select your GMT Offset: "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountLineWrap", Text = @"#3Line Wrap Width#0

Your Line Wrap Width is the number of characters after which excess text on one line will be wrapped to a new line. In many cases you may have to change a setting in your MUD Client to match the value you enter here, though it is recommended that you disable client-side word wrapping. 

Recommended values are between #280#0 and #2130#0, although the game is best enjoyed at #2120#0 characters. Please note that some text will be wrapped at #280#0 characters regardless of this setting.

Please enter your desired line wrap width: "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountName", Text = @"#3Account Name Selection#0

Your account name will be what you use to log in to the game in the future, as well as being a general identifier of you as a player. Account names are not the same thing as character names, and you should not pick an account name that will be the same as a character you will make. Account names must be unique, at least 2 letters long, and contain only letters, numbers and underscores.

Please enter an account name for your new account: "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountPassword", Text = @"#3Password Selection#0

Your password will be used to access your account, and will be stored securely in the game's database as an encrypted, salted hash. Passwords must be at least 8 characters long and can use any combination of letters, numbers and symbols.

Please enter a password for your new account: "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountTimezone",
			Text =
				@"#3{0}#0: {1}
Please select the number of your desired Timezone, or enter the code for it directly if you know it:"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountUnicode", Text = @"#3Unicode Support#0

The engine can be used in unicode mode if your client supports it. If enabled, the MUD will send text in UTF-8 encoding (like web pages) rather than ASCII (like traditional MUDs). MUD Client support for Unicode is sparse. You may also need to be using a font that supports Unicode (such as Courier New).

If you can see the following Chinese characters (rather than squares, question marks or other symbols), your client supports Unicode: 这个文本的存在是为了测试Unicode支持。

Do you want to use Unicode? (y/n): "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "CreateAccountUnitPreference", Text = @"#3Unit Preference#0

When viewing any quantity units (height, weight, length, volume, etc.) in game, you will see the quantity in the style of your preference. For example, your character's height and weight could be displayed in Imperial (feet/inches and pounds), or Metric (metres and kilograms). Regardless of what system you choose, you can enter quantities in the MUD in any system, this setting only affects how they are displayed to you.

The following systems are available for selection:

{0}

Please select your preference for unit systems: "
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "DeathMessage",
			Text = @"Unfortunately, you have died. Our condolences. We hope to see you again soon with a new character."
		});
		context.StaticStrings.Add(new StaticString
			{ Id = "DefaultAlternateTextValue", Text = @"some text that you cannot read" });
		context.StaticStrings.Add(new StaticString
		{
			Id = "DefaultSecondWindEmote",
			Text = @"$0 get|gets a burst of adrenaline as #0 %0|get|gets &0's second wind."
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "DefaultTableCannotFlipTraitMessage", Text = @"@ try|tries to flip $1, but are|is not strong enough."
		});
		context.StaticStrings.Add(new StaticString { Id = "EmoteBeginClean", Text = @"@ begin|begins to clean $0" });
		context.StaticStrings.Add(new StaticString
			{ Id = "EmoteFinishClean", Text = @"$0 clean|cleans $1$?2| with $2||${0}" });
		context.StaticStrings.Add(new StaticString { Id = "EmoteStopClean", Text = @"@ stop|stops cleaning $0" });
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenu", Text = @"
Welcome, {0}!

========== Options ===========

C) Connect to a character
G) Login as a guest
N) Create a new character
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuBanned", Text = @"
		   .-......-.                Your account has been banned.
		 .'          '.              You may not log in to the game.
		/   O      O   \
	   :           `    :
	   |                |            X) Disconnect
	   :    .------.    :
		\  '        '  /
		 '.          .'
		   '-......-'"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuBothChargens", Text = @"Welcome, {0}!

========== Options ===========

C) Connect to a character
		 and/or
   Resume an application
G) Login as a guest
W) Withdraw an application
N) Create a new character
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuMaintenance", Text = @"Welcome, {0}!

The game is currently in maintenance mode and is closed
to players until further notice.

========== Options ===========

E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuMaintenanceNoChargen", Text = @"Welcome, {0}!

The game is currently in maintenance mode and character
creation is closed. You may still log in if you have an
existing character.

========== Options ===========

C) Connect to a character
G) Login as a guest
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuMaintenanceNoLogin", Text = @"Welcome, {0}!

The game is currently in maintenance mode and logging
in is disabled. However, character creation is open.

========== Options ===========

G) Login as a guest
N) Create a character
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuMaintenanceNoLoginBothChargens", Text = @"Welcome, {0}!

The game is currently in maintenance mode and logging
in is disabled. However, character creation is open.

========== Options ===========

C) Resume an application
G) Login as a guest
W) Withdraw an application
N) Create a new character
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuMaintenanceNoLoginResume", Text = @"Welcome, {0}!

The game is currently in maintenance mode and logging
in is disabled. However, character creation is open.

========== Options ===========

C) Resume an application
G) Login as a guest
N) Create a new character
E) Change your email address
P) Change your password"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuMaintenanceNoLoginSubmitted", Text = @"Welcome, {0}!

The game is currently in maintenance mode and logging
in is disabled. However, character creation is open.

========== Options ===========

G) Login as a guest
W) Withdraw an application
N) Create a new character
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuResume", Text = @"Welcome, {0}!

========== Options ===========

C) Connect to a character
		   or
   Resume an application
G) Login as a guest
N) Create a new character
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuSubmitted", Text = @"Welcome, {0}!

========== Options ===========

C) Connect to a character
G) Login as a guest
W) Withdraw an application
N) Create a new character
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "LoggedInMenuUnregistered", Text = @"Welcome, {0}!

Your account is unregistered. Please register your account.

========== Options ===========

R) Enter registration code
S) Resend registration code
E) Change your email address
P) Change your password

X) Disconnect from the game"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "MainMenu", Text = @"{2}
Powered by the FutureMUD Engine {0} (Copyright 2008-{1})

Select an Option:
C) Create a new account
L) Login to your account
R) Recover a lost account"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "MainMenuSiteBanned", Text = @"
		   .-......-.                Your IP address has been banned.
		 .'          '.              You may not log in to the game.
		/   O      O   \
	   :           `    :
	   |                |            X) Disconnect
	   :    .------.    :
		\  '        '  /
		 '.          .'
		   '-......-'"
		});
		context.StaticStrings.Add(new StaticString { Id = "MudName", Text = gameName });
		context.StaticStrings.Add(new StaticString
		{
			Id = "OnSecondWindRecoverMessage",
			Text = @"You feel as if you have recovered from the arrival of your second wind."
		});
		context.StaticStrings.Add(new StaticString { Id = "RegularDeathEmote", Text = @"@ have|has died!" });
		context.StaticStrings.Add(new StaticString
		{
			Id = "WhoText", Text = @"There are currently {0} players online.{4} Our record is {1}, last seen on {2}.{3}"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "WhoTextNoneOnline",
			Text = @"There are currently no players online.{4} Our record is {1}, last seen on {2}.{3}"
		});
		context.StaticStrings.Add(new StaticString
		{
			Id = "WhoTextOneOnline",
			Text = @"There is currently just {0} player online.{4} Our record is {1}, last seen on {2}.{3}"
		});


		// Add mandatory static configurations
		foreach (var ss in DefaultStaticSettings.DefaultStaticConfigurations)
			context.StaticConfigurations.Add(new StaticConfiguration { SettingName = ss.Key, Definition = ss.Value });

		context.StaticConfigurations.Add(new StaticConfiguration
			{ SettingName = "TooManyItemsGameItemGroup", Definition = itemGroup.Id.ToString() });
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "DefaultFoodStackDecorator",
			Definition = context.StackDecorators.First(x => x.Name == "Bites").Id.ToString()
		});
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "ResidueStackDecorator",
			Definition = context.StackDecorators.First(x => x.Name == "Residue").Id.ToString()
		});

		context.StaticConfigurations.Find("DeathsBoardId").Definition =
			context.Boards.First(x => x.Name == "Deaths").Id.ToString();
		context.StaticConfigurations.Find("PetitionsBoardId").Definition =
			context.Boards.First(x => x.Name == "Petitions").Id.ToString();
		context.StaticConfigurations.Find("TyposBoardId").Definition =
			context.Boards.First(x => x.Name == "Typos").Id.ToString();
		context.StaticConfigurations.Find("OnCreateClanProg").Definition = oncreateclanprog.Id.ToString();
		context.StaticConfigurations.Find("PlayersCanCreateClansProg").Definition = cancreateclanprog.Id.ToString();

		context.TraitExpressions.Add(new TraitExpression { Name = "Always Zero", Expression = "0" });
		context.TraitExpressions.Add(new TraitExpression { Name = "Always One", Expression = "1" });
		context.TraitExpressions.Add(new TraitExpression { Name = "1d4", Expression = "rand(1,4)" });
		context.TraitExpressions.Add(new TraitExpression { Name = "1d6", Expression = "rand(1,6)" });
		context.TraitExpressions.Add(new TraitExpression { Name = "1d8", Expression = "rand(1,8)" });
		context.TraitExpressions.Add(new TraitExpression { Name = "1d10", Expression = "rand(1,10)" });
		context.TraitExpressions.Add(new TraitExpression { Name = "1d12", Expression = "rand(1,12)" });
		context.TraitExpressions.Add(new TraitExpression { Name = "1d20", Expression = "rand(1,20)" });
		context.SaveChanges();
		var alwaysOne = context.TraitExpressions.First(x => x.Name == "Always One").Id.ToString();
		context.StaticConfigurations.Find("DefaultWeaponAttackPainExpressionId").Definition = alwaysOne;
		context.StaticConfigurations.Find("DefaultWeaponAttackStunExpressionId").Definition = alwaysOne;
		context.StaticConfigurations.Find("DefaultWeaponAttackDamageExpressionId").Definition = alwaysOne;
	}

	private static void SeedCoreProgs(FuturemudDatabaseContext context, out FutureProg isadminprog,
		out FutureProg cancreateclanprog, out FutureProg oncreateclanprog)
	{
		// Core progs
		var alwaystrueprog = new FutureProg
		{
			FunctionName = "AlwaysTrue",
			AcceptsAnyParameters = true,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts any parameters, and always returns true.",
			FunctionText = "return true",
			StaticType = 2
		};
		context.FutureProgs.Add(alwaystrueprog);
		context.SaveChanges();

		var alwaysfalseprog = new FutureProg
		{
			FunctionName = "AlwaysFalse",
			AcceptsAnyParameters = true,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts any parameters, and always returns false.",
			FunctionText = "return false",
			StaticType = 2
		};
		context.FutureProgs.Add(alwaysfalseprog);
		context.SaveChanges();

		var alwayszeroprog = new FutureProg
		{
			FunctionName = "AlwaysZero",
			AcceptsAnyParameters = true,
			ReturnType = 2,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts any parameters, and always returns zero.",
			FunctionText = "return 0",
			StaticType = 2
		};
		context.FutureProgs.Add(alwayszeroprog);
		context.SaveChanges();

		var alwaysoneprog = new FutureProg
		{
			FunctionName = "AlwaysOne",
			AcceptsAnyParameters = true,
			ReturnType = 2,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts any parameters, and always returns one.",
			FunctionText = "return 1",
			StaticType = 2
		};
		context.FutureProgs.Add(alwaysoneprog);
		context.SaveChanges();

		var alwaysonehundredprog = new FutureProg
		{
			FunctionName = "AlwaysOneHundred",
			AcceptsAnyParameters = true,
			ReturnType = 2,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts any parameters, and always returns one hundred.",
			FunctionText = "return 100",
			StaticType = 2
		};
		context.FutureProgs.Add(alwaysonehundredprog);
		context.SaveChanges();

		var prog = new FutureProg
		{
			FunctionName = "AlwaysTwo",
			AcceptsAnyParameters = true,
			ReturnType = 2,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts any parameters, and always returns two.",
			FunctionText = "return 2",
			StaticType = 2
		};
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsLessThanOne",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is less than 1.",
			FunctionText = "return @number < 1",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsLessThanOneOrEqual",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is less than or equal to 1.",
			FunctionText = "return @number <= 1",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsGreaterThanOne",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is greater than 1.",
			FunctionText = "return @number > 1",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsGreaterThanOneOrEqual",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is greater than or equal to 1.",
			FunctionText = "return @number >= 1",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsLessThanOneHundred",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is less than 100.",
			FunctionText = "return @number < 100",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsGreaterThanOneHundredOrEqual",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is greater than or equal to 100.",
			FunctionText = "return @number >= 100",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsLessThanTwoHundredForty",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is less than 240.",
			FunctionText = "return @number < 240",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		prog = new FutureProg
		{
			FunctionName = "IsGreaterThanTwoHundredFortyOrEqual",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Accepts a number and returns true if it is greater than or equal to 240.",
			FunctionText = "return @number >= 240",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		isadminprog = new FutureProg
		{
			FunctionName = "IsAdmin",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = "Returns true if the supplied character/chargen is an admin",
			FunctionText =
				@"if (isnull(@ch))
  return false
end if
return IsAdmin(@ch)",
			StaticType = 0
		};
		isadminprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isadminprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		context.FutureProgs.Add(isadminprog);
		context.SaveChanges();

		cancreateclanprog = new FutureProg
		{
			FunctionName = "CanCreateClan",
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Clans",
			Subcategory = "General",
			Public = true,
			FunctionComment = "Players can create one clan each if they have more than 24hours played",
			FunctionText =
				@"return @ch.playtime >= 1440 and not(GetRegister(@ch, ""hascreatedaclan""))",
			StaticType = 0
		};
		cancreateclanprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = cancreateclanprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8
		});
		context.FutureProgs.Add(cancreateclanprog);
		context.SaveChanges();

		oncreateclanprog = new FutureProg
		{
			FunctionName = "OnCreateClan",
			AcceptsAnyParameters = false,
			ReturnType = 0,
			Category = "Clans",
			Subcategory = "General",
			Public = true,
			FunctionComment = "Called when a player creates a clan",
			FunctionText = @"setregister @ch ""hascreatedaclan"" true",
			StaticType = 0
		};
		oncreateclanprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = oncreateclanprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8
		});

		context.FutureProgs.Add(oncreateclanprog);
		context.SaveChanges();

		context.VariableDefinitions.Add(new VariableDefinition
		{
			ContainedType = 4,
			OwnerType = 8,
			Property = "hascreatedaclan"
		});
		context.VariableDefaults.Add(new VariableDefault
		{
			OwnerType = 8,
			Property = "hascreatedaclan",
			DefaultValue = "<var>False</var>"
		});
		context.SaveChanges();
	}

	private static void SeedCulturesAndTimezoneInfos(FuturemudDatabaseContext context)
	{
		// Culture Infos
		context.CultureInfos.Add(new CultureInfo
		{
			Id = "en-AU",
			DisplayName = "English (Australia)",
			Order = 1
		});
		context.CultureInfos.Add(new CultureInfo
		{
			Id = "en-CA",
			DisplayName = "English (Canada)",
			Order = 2
		});
		context.CultureInfos.Add(new CultureInfo
		{
			Id = "en-GB",
			DisplayName = "English (Great Britain)",
			Order = 3
		});
		context.CultureInfos.Add(new CultureInfo
		{
			Id = "en-NZ",
			DisplayName = "English (New Zealand)",
			Order = 4
		});
		context.CultureInfos.Add(new CultureInfo
		{
			Id = "en-US",
			DisplayName = "English (US)",
			Order = 5
		});
		context.CultureInfos.Add(new CultureInfo
		{
			Id = "en-ZA",
			DisplayName = "English (South Africa)",
			Order = 1
		});
		context.CultureInfos.Add(new CultureInfo { Id = "es-ES", DisplayName = "Spanish (Spain)", Order = 7 });
		context.CultureInfos.Add(new CultureInfo { Id = "es-MX", DisplayName = "Spanish (Mexico)", Order = 8 });
		context.CultureInfos.Add(new CultureInfo { Id = "fr-FR", DisplayName = "French (France)", Order = 9 });
		context.CultureInfos.Add(new CultureInfo { Id = "fr-CA", DisplayName = "French (Canadian)", Order = 10 });
		context.CultureInfos.Add(new CultureInfo { Id = "de-DE", DisplayName = "German (Germany)", Order = 11 });
		context.CultureInfos.Add(new CultureInfo { Id = "el-GR", DisplayName = "Greek (Greece)", Order = 12 });
		context.CultureInfos.Add(new CultureInfo { Id = "zh-CN", DisplayName = "Chinese (P.R.C)", Order = 13 });
		context.CultureInfos.Add(new CultureInfo { Id = "zn-TW", DisplayName = "Chinese (Taiwan)", Order = 14 });
		context.CultureInfos.Add(new CultureInfo { Id = "ja-JP", DisplayName = "Japanese (Japan)", Order = 15 });
		context.CultureInfos.Add(new CultureInfo { Id = "ru-RU", DisplayName = "Russian (Russian)", Order = 16 });
		context.CultureInfos.Add(new CultureInfo { Id = "pl-PL", DisplayName = "Polish (Poland)", Order = 17 });
		context.CultureInfos.Add(new CultureInfo { Id = "id-ID", DisplayName = "Indonesian (Indonesia)", Order = 18 });
		context.CultureInfos.Add(new CultureInfo
			{ Id = "af-ZA", DisplayName = "Afrikaans (South Africa)", Order = 19 });
		context.CultureInfos.Add(new CultureInfo { Id = "he-IL", DisplayName = "Hebrew (Israel)", Order = 20 });
		context.CultureInfos.Add(new CultureInfo { Id = "it-IT", DisplayName = "Italian (Italy)", Order = 21 });
		context.CultureInfos.Add(new CultureInfo { Id = "pt-BR", DisplayName = "Portugese (Brazil)", Order = 22 });
		context.CultureInfos.Add(new CultureInfo { Id = "pt-PT", DisplayName = "Portugese (Portugal)", Order = 23 });

		// Timezones
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Dateline Standard Time", Display = "(UTC-12:00) International Date Line West", Order = -12 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "UTC-11", Display = "(UTC-11:00) Coordinated Universal Time-11", Order = -11 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Aleutian Standard Time", Display = "(UTC-10:00) Aleutian Islands", Order = -10 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Hawaiian Standard Time", Display = "(UTC-10:00) Hawaii", Order = -10 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Marquesas Standard Time", Display = "(UTC-09:30) Marquesas Islands", Order = -9.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Alaskan Standard Time", Display = "(UTC-09:00) Alaska", Order = -9 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "UTC-09", Display = "(UTC-09:00) Coordinated Universal Time-09", Order = -9 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Pacific Standard Time", Display = "(UTC-08:00) Pacific Time (US & Canada)", Order = -8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Pacific Standard Time (Mexico)", Display = "(UTC-08:00) Baja California", Order = -8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "UTC-08", Display = "(UTC-08:00) Coordinated Universal Time-08", Order = -8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Mountain Standard Time", Display = "(UTC-07:00) Mountain Time (US & Canada)", Order = -7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
		{
			Id = "Mountain Standard Time (Mexico)", Display = "(UTC-07:00) Chihuahua, La Paz, Mazatlan", Order = -7
		});
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "US Mountain Standard Time", Display = "(UTC-07:00) Arizona", Order = -7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Canada Central Standard Time", Display = "(UTC-06:00) Saskatchewan", Order = -6 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Central America Standard Time", Display = "(UTC-06:00) Central America", Order = -6 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Central Standard Time", Display = "(UTC-06:00) Central Time (US & Canada)", Order = -6 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
		{
			Id = "Central Standard Time (Mexico)", Display = "(UTC-06:00) Guadalajara, Mexico City, Monterrey",
			Order = -6
		});
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Easter Island Standard Time", Display = "(UTC-06:00) Easter Island", Order = -6 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Cuba Standard Time", Display = "(UTC-05:00) Havana", Order = -5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Eastern Standard Time", Display = "(UTC-05:00) Eastern Time (US & Canada)", Order = -5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Eastern Standard Time (Mexico)", Display = "(UTC-05:00) Chetumal", Order = -5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Haiti Standard Time", Display = "(UTC-05:00) Haiti", Order = -5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "SA Pacific Standard Time", Display = "(UTC-05:00) Bogota, Lima, Quito, Rio Branco", Order = -5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "US Eastern Standard Time", Display = "(UTC-05:00) Indiana (East)", Order = -5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Atlantic Standard Time", Display = "(UTC-04:00) Atlantic Time (Canada)", Order = -4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Central Brazilian Standard Time", Display = "(UTC-04:00) Cuiaba", Order = -4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Pacific SA Standard Time", Display = "(UTC-04:00) Santiago", Order = -4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Paraguay Standard Time", Display = "(UTC-04:00) Asuncion", Order = -4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
		{
			Id = "SA Western Standard Time", Display = "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan", Order = -4
		});
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Turks And Caicos Standard Time", Display = "(UTC-04:00) Turks and Caicos", Order = -4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Venezuela Standard Time", Display = "(UTC-04:00) Caracas", Order = -4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Newfoundland Standard Time", Display = "(UTC-03:30) Newfoundland", Order = -3.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Argentina Standard Time", Display = "(UTC-03:00) City of Buenos Aires", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Bahia Standard Time", Display = "(UTC-03:00) Salvador", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "E. South America Standard Time", Display = "(UTC-03:00) Brasilia", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Greenland Standard Time", Display = "(UTC-03:00) Greenland", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Montevideo Standard Time", Display = "(UTC-03:00) Montevideo", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "SA Eastern Standard Time", Display = "(UTC-03:00) Cayenne, Fortaleza", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Saint Pierre Standard Time", Display = "(UTC-03:00) Saint Pierre and Miquelon", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Tocantins Standard Time", Display = "(UTC-03:00) Araguaina", Order = -3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Mid-Atlantic Standard Time", Display = "(UTC-02:00) Mid-Atlantic - Old", Order = -2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "UTC-02", Display = "(UTC-02:00) Coordinated Universal Time-02", Order = -2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Azores Standard Time", Display = "(UTC-01:00) Azores", Order = -1 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Cape Verde Standard Time", Display = "(UTC-01:00) Cabo Verde Is.", Order = -1 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "GMT Standard Time", Display = "(UTC+00:00) Dublin, Edinburgh, Lisbon, London", Order = 0 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Greenwich Standard Time", Display = "(UTC+00:00) Monrovia, Reykjavik", Order = 0 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Morocco Standard Time", Display = "(UTC+00:00) Casablanca", Order = 0 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "UTC", Display = "(UTC) Coordinated Universal Time", Order = 0 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
		{
			Id = "Central Europe Standard Time",
			Display = "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague", Order = 1
		});
		context.TimeZoneInfos.Add(new TimeZoneInfo
		{
			Id = "Central European Standard Time", Display = "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb", Order = 1
		});
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Namibia Standard Time", Display = "(UTC+01:00) Windhoek", Order = 1 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Romance Standard Time", Display = "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris", Order = 1 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "W. Central Africa Standard Time", Display = "(UTC+01:00) West Central Africa", Order = 1 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
		{
			Id = "W. Europe Standard Time", Display = "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna",
			Order = 1
		});
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "E. Europe Standard Time", Display = "(UTC+02:00) Chisinau", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Egypt Standard Time", Display = "(UTC+02:00) Cairo", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
		{
			Id = "FLE Standard Time", Display = "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius", Order = 2
		});
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "GTB Standard Time", Display = "(UTC+02:00) Athens, Bucharest", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Israel Standard Time", Display = "(UTC+02:00) Jerusalem", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Jordan Standard Time", Display = "(UTC+02:00) Amman", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Kaliningrad Standard Time", Display = "(UTC+02:00) Kaliningrad", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Libya Standard Time", Display = "(UTC+02:00) Tripoli", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Middle East Standard Time", Display = "(UTC+02:00) Beirut", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "South Africa Standard Time", Display = "(UTC+02:00) Harare, Pretoria", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Syria Standard Time", Display = "(UTC+02:00) Damascus", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "West Bank Standard Time", Display = "(UTC+02:00) Gaza, Hebron", Order = 2 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Arab Standard Time", Display = "(UTC+03:00) Kuwait, Riyadh", Order = 3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Arabic Standard Time", Display = "(UTC+03:00) Baghdad", Order = 3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Belarus Standard Time", Display = "(UTC+03:00) Minsk", Order = 3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "E. Africa Standard Time", Display = "(UTC+03:00) Nairobi", Order = 3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Russian Standard Time", Display = "(UTC+03:00) Moscow, St. Petersburg, Volgograd", Order = 3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Turkey Standard Time", Display = "(UTC+03:00) Istanbul", Order = 3 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Iran Standard Time", Display = "(UTC+03:30) Tehran", Order = 3.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Arabian Standard Time", Display = "(UTC+04:00) Abu Dhabi, Muscat", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Astrakhan Standard Time", Display = "(UTC+04:00) Astrakhan, Ulyanovsk", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Azerbaijan Standard Time", Display = "(UTC+04:00) Baku", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Caucasus Standard Time", Display = "(UTC+04:00) Yerevan", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Georgian Standard Time", Display = "(UTC+04:00) Tbilisi", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Mauritius Standard Time", Display = "(UTC+04:00) Port Louis", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Russia Time Zone 3", Display = "(UTC+04:00) Izhevsk, Samara", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Saratov Standard Time", Display = "(UTC+04:00) Saratov", Order = 4 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Afghanistan Standard Time", Display = "(UTC+04:30) Kabul", Order = 4.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Ekaterinburg Standard Time", Display = "(UTC+05:00) Ekaterinburg", Order = 5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Pakistan Standard Time", Display = "(UTC+05:00) Islamabad, Karachi", Order = 5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "West Asia Standard Time", Display = "(UTC+05:00) Ashgabat, Tashkent", Order = 5 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "India Standard Time", Display = "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi", Order = 5.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Sri Lanka Standard Time", Display = "(UTC+05:30) Sri Jayawardenepura", Order = 5.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Nepal Standard Time", Display = "(UTC+05:45) Kathmandu", Order = 5.75M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Bangladesh Standard Time", Display = "(UTC+06:00) Dhaka", Order = 6 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Central Asia Standard Time", Display = "(UTC+06:00) Astana", Order = 6 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Omsk Standard Time", Display = "(UTC+06:00) Omsk", Order = 6 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Myanmar Standard Time", Display = "(UTC+06:30) Yangon (Rangoon)", Order = 6.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Altai Standard Time", Display = "(UTC+07:00) Barnaul, Gorno-Altaysk", Order = 7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "N. Central Asia Standard Time", Display = "(UTC+07:00) Novosibirsk", Order = 7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "North Asia Standard Time", Display = "(UTC+07:00) Krasnoyarsk", Order = 7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "SE Asia Standard Time", Display = "(UTC+07:00) Bangkok, Hanoi, Jakarta", Order = 7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Tomsk Standard Time", Display = "(UTC+07:00) Tomsk", Order = 7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "W. Mongolia Standard Time", Display = "(UTC+07:00) Hovd", Order = 7 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "China Standard Time", Display = "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi", Order = 8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "North Asia East Standard Time", Display = "(UTC+08:00) Irkutsk", Order = 8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Singapore Standard Time", Display = "(UTC+08:00) Kuala Lumpur, Singapore", Order = 8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Taipei Standard Time", Display = "(UTC+08:00) Taipei", Order = 8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Ulaanbaatar Standard Time", Display = "(UTC+08:00) Ulaanbaatar", Order = 8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "W. Australia Standard Time", Display = "(UTC+08:00) Perth", Order = 8 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "North Korea Standard Time", Display = "(UTC+08:30) Pyongyang", Order = 8.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Aus Central W. Standard Time", Display = "(UTC+08:45) Eucla", Order = 8.75M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Korea Standard Time", Display = "(UTC+09:00) Seoul", Order = 9 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Tokyo Standard Time", Display = "(UTC+09:00) Osaka, Sapporo, Tokyo", Order = 9 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Transbaikal Standard Time", Display = "(UTC+09:00) Chita", Order = 9 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Yakutsk Standard Time", Display = "(UTC+09:00) Yakutsk", Order = 9 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "AUS Central Standard Time", Display = "(UTC+09:30) Darwin", Order = 9.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Cen. Australia Standard Time", Display = "(UTC+09:30) Adelaide", Order = 9.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "AUS Eastern Standard Time", Display = "(UTC+10:00) Canberra, Melbourne, Sydney", Order = 10 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "E. Australia Standard Time", Display = "(UTC+10:00) Brisbane", Order = 10 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Tasmania Standard Time", Display = "(UTC+10:00) Hobart", Order = 10 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Vladivostok Standard Time", Display = "(UTC+10:00) Vladivostok", Order = 10 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "West Pacific Standard Time", Display = "(UTC+10:00) Guam, Port Moresby", Order = 10 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Lord Howe Standard Time", Display = "(UTC+10:30) Lord Howe Island", Order = 10.5M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Bougainville Standard Time", Display = "(UTC+11:00) Bougainville Island", Order = 11 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Central Pacific Standard Time", Display = "(UTC+11:00) Solomon Is., New Caledonia", Order = 11 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Magadan Standard Time", Display = "(UTC+11:00) Magadan", Order = 11 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Norfolk Standard Time", Display = "(UTC+11:00) Norfolk Island", Order = 11 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Russia Time Zone 10", Display = "(UTC+11:00) Chokurdakh", Order = 11 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Sakhalin Standard Time", Display = "(UTC+11:00) Sakhalin", Order = 11 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Fiji Standard Time", Display = "(UTC+12:00) Fiji", Order = 12 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Kamchatka Standard Time", Display = "(UTC+12:00) Petropavlovsk-Kamchatsky - Old", Order = 12 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "New Zealand Standard Time", Display = "(UTC+12:00) Auckland, Wellington", Order = 12 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Russia Time Zone 11", Display = "(UTC+12:00) Anadyr, Petropavlovsk-Kamchatsky", Order = 12 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "UTC+12", Display = "(UTC+12:00) Coordinated Universal Time+12", Order = 12 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Chatham Islands Standard Time", Display = "(UTC+12:45) Chatham Islands", Order = 12.75M });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Samoa Standard Time", Display = "(UTC+13:00) Samoa", Order = 13 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Tonga Standard Time", Display = "(UTC+13:00) Nuku'alofa", Order = 13 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "UTC+13", Display = "(UTC+13:00) Coordinated Universal Time+13", Order = 13 });
		context.TimeZoneInfos.Add(new TimeZoneInfo
			{ Id = "Line Islands Standard Time", Display = "(UTC+14:00) Kiritimati Island", Order = 14 });
	}

	private static AuthorityGroup SeedAuthorities(FuturemudDatabaseContext context)
	{
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 0,
			AuthorityLevel = 0,
			InformationLevel = 0,
			CharactersLevel = 0,
			ItemsLevel = 0,
			PlanesLevel = 0,
			RoomsLevel = 0,
			Name = "Guest",
			CharacterApprovalLevel = 0,
			CharacterApprovalRisk = 0
		});
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 1,
			AuthorityLevel = 1,
			InformationLevel = 1,
			CharactersLevel = 1,
			ItemsLevel = 1,
			PlanesLevel = 1,
			RoomsLevel = 1,
			Name = "NPC",
			CharacterApprovalLevel = 0,
			CharacterApprovalRisk = 0
		});
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 2,
			AuthorityLevel = 2,
			InformationLevel = 2,
			CharactersLevel = 2,
			ItemsLevel = 2,
			PlanesLevel = 2,
			RoomsLevel = 2,
			Name = "Player",
			CharacterApprovalLevel = 0,
			CharacterApprovalRisk = 0
		});
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 3,
			AuthorityLevel = 3,
			InformationLevel = 3,
			CharactersLevel = 3,
			ItemsLevel = 3,
			PlanesLevel = 3,
			RoomsLevel = 3,
			Name = "Guide",
			CharacterApprovalLevel = 1,
			CharacterApprovalRisk = 1
		});
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 4,
			AuthorityLevel = 4,
			InformationLevel = 4,
			CharactersLevel = 4,
			ItemsLevel = 4,
			PlanesLevel = 4,
			RoomsLevel = 4,
			Name = "Junior Admin",
			CharacterApprovalLevel = 2,
			CharacterApprovalRisk = 2
		});
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 5,
			AuthorityLevel = 5,
			InformationLevel = 5,
			CharactersLevel = 5,
			ItemsLevel = 5,
			PlanesLevel = 5,
			RoomsLevel = 5,
			Name = "Admin",
			CharacterApprovalLevel = 2,
			CharacterApprovalRisk = 3
		});
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 6,
			AuthorityLevel = 6,
			InformationLevel = 6,
			CharactersLevel = 6,
			ItemsLevel = 6,
			PlanesLevel = 6,
			RoomsLevel = 6,
			Name = "Senior Admin",
			CharacterApprovalLevel = 3,
			CharacterApprovalRisk = 8
		});
		context.AuthorityGroups.Add(new AuthorityGroup
		{
			AccountsLevel = 7,
			AuthorityLevel = 7,
			InformationLevel = 7,
			CharactersLevel = 7,
			ItemsLevel = 7,
			PlanesLevel = 7,
			RoomsLevel = 7,
			Name = "Head Admin",
			CharacterApprovalLevel = 4,
			CharacterApprovalRisk = 8
		});
		var producer = new AuthorityGroup
		{
			AccountsLevel = 8,
			AuthorityLevel = 8,
			InformationLevel = 8,
			CharactersLevel = 8,
			ItemsLevel = 8,
			PlanesLevel = 8,
			RoomsLevel = 8,
			Name = "Producer",
			CharacterApprovalLevel = 4,
			CharacterApprovalRisk = 8
		};
		context.AuthorityGroups.Add(producer);

		context.SaveChanges();
		return producer;
	}

	public void SeedColours(FuturemudDatabaseContext context)
	{
		var colourDef = new CharacteristicDefinition
		{
			Type = 2,
			Name = "Colour",
			Pattern = "colou?r",
			Description = "The base variable for all colour types",
			Model = "standard"
		};
		context.CharacteristicDefinitions.Add(colourDef);
		context.SaveChanges();

		context.CharacteristicDefinitions.Add(new CharacteristicDefinition
		{
			Type = 2,
			Name = "Colour1",
			Pattern = "colou?r1",
			Description = "A child of the Colour variable for use when an item has multiple colours",
			Model = "standard",
			ParentId = colourDef.Id
		});
		context.CharacteristicDefinitions.Add(new CharacteristicDefinition
		{
			Type = 2,
			Name = "Colour2",
			Pattern = "colou?r2",
			Description = "A child of the Colour variable for use when an item has multiple colours",
			Model = "standard",
			ParentId = colourDef.Id
		});
		context.CharacteristicDefinitions.Add(new CharacteristicDefinition
		{
			Type = 2,
			Name = "Colour3",
			Pattern = "colou?r3",
			Description = "A child of the Colour variable for use when an item has multiple colours",
			Model = "standard",
			ParentId = colourDef.Id
		});

		// Colours
		var colours = new List<Colour>();
		context.Colours.Add(new Colour
			{ Id = 1, Name = "black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "a pure shade of black" });
		context.Colours.Add(new Colour
			{ Id = 2, Name = "white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "a pure shade of white" });
		context.Colours.Add(new Colour
			{ Id = 3, Name = "grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "a pure shade of grey" });
		context.Colours.Add(new Colour
		{
			Id = 4, Name = "light grey", Basic = 2, Red = 175, Green = 175, Blue = 175,
			Fancy = "a pure shade of light grey"
		});
		context.Colours.Add(new Colour
		{
			Id = 5, Name = "dark grey", Basic = 2, Red = 75, Green = 75, Blue = 75, Fancy = "a pure shade of dark grey"
		});
		context.Colours.Add(new Colour
			{ Id = 6, Name = "red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "a pure shade of red" });
		context.Colours.Add(new Colour
		{
			Id = 7, Name = "dark red", Basic = 3, Red = 160, Green = 0, Blue = 0, Fancy = "a pure shade of dark red"
		});
		context.Colours.Add(new Colour
			{ Id = 8, Name = "blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "a pure shade of blue" });
		context.Colours.Add(new Colour
		{
			Id = 9, Name = "dark blue", Basic = 4, Red = 0, Green = 0, Blue = 160, Fancy = "a pure shade of dark blue"
		});
		context.Colours.Add(new Colour
			{ Id = 10, Name = "green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a pure shade of green" });
		context.Colours.Add(new Colour
			{ Id = 11, Name = "brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "a pure shade of brown" });
		context.Colours.Add(new Colour
		{
			Id = 12, Name = "dark green", Basic = 5, Red = 0, Green = 160, Blue = 0,
			Fancy = "a pure shade of dark green"
		});
		context.Colours.Add(new Colour
		{
			Id = 13, Name = "hazel", Basic = 10, Red = 175, Green = 255, Blue = 0,
			Fancy = "a complex mixture of brown, green and gold"
		});
		context.Colours.Add(new Colour
			{ Id = 14, Name = "pale white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "a pale white" });
		context.Colours.Add(new Colour
		{
			Id = 15, Name = "olive", Basic = 5, Red = 25, Green = 160, Blue = 0,
			Fancy = "the dark, deep brownish-green of an olive"
		});
		context.Colours.Add(new Colour
		{
			Id = 16, Name = "caramel", Basic = 10, Red = 185, Green = 175, Blue = 0,
			Fancy = "the deep, rich brown of melted caramel"
		});
		context.Colours.Add(new Colour
		{
			Id = 17, Name = "ebony", Basic = 0, Red = 10, Green = 10, Blue = 10,
			Fancy = "the deep, rich black of polished ebony"
		});
		context.Colours.Add(new Colour
		{
			Id = 18, Name = "emerald green", Basic = 5, Red = 0, Green = 255, Blue = 15,
			Fancy = "the radiant green of a cut emerald"
		});
		context.Colours.Add(new Colour
		{
			Id = 19, Name = "cerulean", Basic = 4, Red = 0, Green = 75, Blue = 255,
			Fancy = "the vibrant, bright cyan of cerulean"
		});
		context.Colours.Add(new Colour
		{
			Id = 20, Name = "violet", Basic = 8, Red = 225, Green = 0, Blue = 225,
			Fancy = "the bright pure purple colour of violet"
		});
		context.Colours.Add(new Colour
		{
			Id = 21, Name = "sandy brown", Basic = 10, Red = 125, Green = 125, Blue = 10,
			Fancy = "the light brown colour of beach sand"
		});
		context.Colours.Add(new Colour
		{
			Id = 22, Name = "light brown", Basic = 10, Red = 125, Green = 125, Blue = 0, Fancy = "a rich, light brown"
		});
		context.Colours.Add(new Colour
			{ Id = 23, Name = "dark brown", Basic = 10, Red = 225, Green = 225, Blue = 0, Fancy = "a dark brown" });
		context.Colours.Add(new Colour
		{
			Id = 24, Name = "auburn", Basic = 3, Red = 160, Green = 10, Blue = 10,
			Fancy = "the rich, reddish brown of auburn"
		});
		context.Colours.Add(new Colour
		{
			Id = 25, Name = "ebony", Basic = 0, Red = 0, Green = 0, Blue = 0,
			Fancy = "the deep, rich black of polished ebony"
		});
		context.Colours.Add(new Colour
		{
			Id = 26, Name = "onyx", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "the colour of an onyx gemstone"
		});
		context.Colours.Add(new Colour
			{ Id = 27, Name = "obsidian", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "a deep obsidian black" });
		context.Colours.Add(new Colour
		{
			Id = 28, Name = "midnight black", Basic = 0, Red = 0, Green = 0, Blue = 0,
			Fancy = "the colour of a starless midnight sky"
		});
		context.Colours.Add(new Colour
		{
			Id = 29, Name = "ink black", Basic = 0, Red = 0, Green = 0, Blue = 0,
			Fancy = "the colour of a pot of black ink"
		});
		context.Colours.Add(new Colour
			{ Id = 30, Name = "jet black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "a tenebrous, jet black" });
		context.Colours.Add(new Colour
		{
			Id = 31, Name = "pitch black", Basic = 0, Red = 0, Green = 0, Blue = 0,
			Fancy = "the colour of absolute darkness"
		});
		context.Colours.Add(new Colour
		{
			Id = 32, Name = "ivory", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of polished ivory"
		});
		context.Colours.Add(new Colour
		{
			Id = 33, Name = "seashell", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of a weathered seashell"
		});
		context.Colours.Add(new Colour
		{
			Id = 34, Name = "snow white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of fresh snow"
		});
		context.Colours.Add(new Colour
		{
			Id = 35, Name = "gleaming white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "a bright, gleaming white"
		});
		context.Colours.Add(new Colour
		{
			Id = 36, Name = "pure white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "a perfect, pure white"
		});
		context.Colours.Add(new Colour
		{
			Id = 37, Name = "pearl white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of a polished pearl"
		});
		context.Colours.Add(new Colour
		{
			Id = 38, Name = "bright white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "a bold, bright white"
		});
		context.Colours.Add(new Colour
		{
			Id = 39, Name = "bone white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of weathered bone"
		});
		context.Colours.Add(new Colour
		{
			Id = 40, Name = "ghost white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "a ghostly shade of white"
		});
		context.Colours.Add(new Colour
		{
			Id = 41, Name = "mist grey", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of a thick morning mist"
		});
		context.Colours.Add(new Colour
		{
			Id = 42, Name = "charcoal grey", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of charcoal"
		});
		context.Colours.Add(new Colour
		{
			Id = 43, Name = "thistle grey", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of a thistle bush"
		});
		context.Colours.Add(new Colour
		{
			Id = 44, Name = "smoky grey", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of wood smoke"
		});
		context.Colours.Add(new Colour
		{
			Id = 45, Name = "slate grey", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the grey-blue colour of a slab of slate"
		});
		context.Colours.Add(new Colour
		{
			Id = 46, Name = "silver grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "a rich, silvery grey"
		});
		context.Colours.Add(new Colour
			{ Id = 47, Name = "soft grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "a soft grey" });
		context.Colours.Add(new Colour
		{
			Id = 48, Name = "ash grey", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the grey colour of cold ash"
		});
		context.Colours.Add(new Colour
		{
			Id = 49, Name = "crimson", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "a strong, deep crimson red"
		});
		context.Colours.Add(new Colour
		{
			Id = 50, Name = "scarlet", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the bright, orangey-red colour of scarlet"
		});
		context.Colours.Add(new Colour
		{
			Id = 51, Name = "ruby red", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the colour of a ruby gemstone"
		});
		context.Colours.Add(new Colour
		{
			Id = 52, Name = "blood red", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the colour of arterial blood"
		});
		context.Colours.Add(new Colour
		{
			Id = 53, Name = "rose red", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the colour of a rose in bloom"
		});
		context.Colours.Add(new Colour
		{
			Id = 54, Name = "wine red", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the deep purplish-red of wine"
		});
		context.Colours.Add(new Colour
		{
			Id = 55, Name = "flame red", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the bold red of burning flame"
		});
		context.Colours.Add(new Colour
		{
			Id = 56, Name = "coral", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the dark reddish-pink of coral"
		});
		context.Colours.Add(new Colour
		{
			Id = 57, Name = "copper", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the reddish-brown of the metal copper"
		});
		context.Colours.Add(new Colour
		{
			Id = 58, Name = "fiery orange", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the bold orange of a burning flame"
		});
		context.Colours.Add(new Colour
		{
			Id = 59, Name = "ochre", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "the yellowish-brown of ochre"
		});
		context.Colours.Add(new Colour
		{
			Id = 60, Name = "sunset orange", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the brilliant yellowish-orange of a sunset"
		});
		context.Colours.Add(new Colour
		{
			Id = 61, Name = "amber", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the colour of a block of polished amber"
		});
		context.Colours.Add(new Colour
		{
			Id = 62, Name = "goldenrod", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the yellow of the goldenrod flower"
		});
		context.Colours.Add(new Colour
		{
			Id = 63, Name = "pale yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "a gentle, pale yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 64, Name = "golden yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "a rich, golden yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 65, Name = "sand yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the light brownish-yellow of beach sand"
		});
		context.Colours.Add(new Colour
		{
			Id = 66, Name = "topaz hued", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of the topaz gemstone"
		});
		context.Colours.Add(new Colour
		{
			Id = 67, Name = "gold-coloured", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "a full-bodied gold hue"
		});
		context.Colours.Add(new Colour
		{
			Id = 68, Name = "spring green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the light, bluish-green of early spring"
		});
		context.Colours.Add(new Colour
		{
			Id = 69, Name = "sea green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the deep bluish-green of the sea"
		});
		context.Colours.Add(new Colour
		{
			Id = 70, Name = "hunter green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the dark green of woodland leaves"
		});
		context.Colours.Add(new Colour
			{ Id = 71, Name = "olive green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "an olive green" });
		context.Colours.Add(new Colour
		{
			Id = 72, Name = "sage green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the light, pale green of sage leaves"
		});
		context.Colours.Add(new Colour
		{
			Id = 73, Name = "pine green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the rich, dark green of pine leaves"
		});
		context.Colours.Add(new Colour
		{
			Id = 74, Name = "bright green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a bold, bright green"
		});
		context.Colours.Add(new Colour
			{ Id = 75, Name = "rich green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a deep, rich green" });
		context.Colours.Add(new Colour
		{
			Id = 76, Name = "pale green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a pale greyish-green"
		});
		context.Colours.Add(new Colour
		{
			Id = 77, Name = "verdant green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the verdant green of summer"
		});
		context.Colours.Add(new Colour
			{ Id = 78, Name = "forest green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a forest green" });
		context.Colours.Add(new Colour
		{
			Id = 79, Name = "chartreuse", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the slight greenish-yellow colour of chartreuse"
		});
		context.Colours.Add(new Colour
		{
			Id = 80, Name = "slate blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "the greyish-blue of slate"
		});
		context.Colours.Add(new Colour
		{
			Id = 81, Name = "bright blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "a bright, vibrant blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 82, Name = "powder blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the bright cyan colour of powder snow"
		});
		context.Colours.Add(new Colour
		{
			Id = 83, Name = "sapphire blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of the sapphire gemstone"
		});
		context.Colours.Add(new Colour
		{
			Id = 84, Name = "royal blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "a dark shade of azure blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 85, Name = "ocean blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the deep, multihued blue of the ocean"
		});
		context.Colours.Add(new Colour
			{ Id = 86, Name = "teal", Basic = 11, Red = 0, Green = 75, Blue = 255, Fancy = "a dark bluish-green" });
		context.Colours.Add(new Colour
		{
			Id = 87, Name = "cornflour blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the medium blue of the sapphire gem"
		});
		context.Colours.Add(new Colour
		{
			Id = 88, Name = "sky blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the rich colour of a cloudless sky"
		});
		context.Colours.Add(new Colour
		{
			Id = 89, Name = "azure", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the blue colour of the azure gemstone"
		});
		context.Colours.Add(new Colour
		{
			Id = 90, Name = "beryl", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the bluish-green colour of the beryl gemstone"
		});
		context.Colours.Add(new Colour
		{
			Id = 91, Name = "cerulean", Basic = 11, Red = 0, Green = 75, Blue = 255,
			Fancy = "a rich, pure cerulean blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 92, Name = "cobalt", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "a medium-dark cobalt blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 93, Name = "rich indigo", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the vibrant colour of indigo dye"
		});
		context.Colours.Add(new Colour
		{
			Id = 94, Name = "deep indigo", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the deep indigo of the bottom of a rainbow"
		});
		context.Colours.Add(new Colour
		{
			Id = 95, Name = "vivid indigo", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "a vivid, bold indigo"
		});
		context.Colours.Add(new Colour
		{
			Id = 96, Name = "earthen brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the brown of rich soil"
		});
		context.Colours.Add(new Colour
		{
			Id = 97, Name = "deep brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "a deep, dark brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 98, Name = "rich brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "a rich, bold brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 99, Name = "burnt sienna", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of fired sienna earth"
		});
		context.Colours.Add(new Colour
		{
			Id = 100, Name = "chocolate", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of dark chocolate"
		});
		context.Colours.Add(new Colour
		{
			Id = 101, Name = "cinnamon", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the light pinkish-brown of cinnamon"
		});
		context.Colours.Add(new Colour
		{
			Id = 102, Name = "mahogany", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the rich, dark colour of mahogany timber"
		});
		context.Colours.Add(new Colour
		{
			Id = 103, Name = "nut brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the light brown of a chestnut shell"
		});
		context.Colours.Add(new Colour
		{
			Id = 104, Name = "umber", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the deep reddish-brown of umber"
		});
		context.Colours.Add(new Colour
		{
			Id = 105, Name = "amethyst", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the vivid purple of the amethyst gemstone"
		});
		context.Colours.Add(new Colour
		{
			Id = 106, Name = "mauve", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the greyish-magenta of the mallow flower"
		});
		context.Colours.Add(new Colour
		{
			Id = 107, Name = "mulbery", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the rich blackish-purple hue of ripe mulberries"
		});
		context.Colours.Add(new Colour
		{
			Id = 108, Name = "plum", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the rich reddish-purple hue of a plum"
		});
		context.Colours.Add(new Colour
		{
			Id = 109, Name = "lavender", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the pure light-purple of a lavender bush"
		});
		context.Colours.Add(new Colour
		{
			Id = 110, Name = "royal purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "a rich, dark purple"
		});
		context.Colours.Add(new Colour
			{ Id = 111, Name = "faded black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 112, Name = "tattered black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 113, Name = "shabby black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 114, Name = "grimy black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 115, Name = "off-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 116, Name = "dingy grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 117, Name = "blotched red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 118, Name = "dull orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 119, Name = "bland yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 120, Name = "faded green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 121, Name = "faded blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 122, Name = "faded indigo", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 123, Name = "faded purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 124, Name = "drab brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 125, Name = "dim grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 126, Name = "dusky slate grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 127, Name = "sooty grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 128, Name = "chalky pale grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 129, Name = "dull mist grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 130, Name = "ashen off-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 131, Name = "dirty bone-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 132, Name = "wan ivory", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 133, Name = "spotted white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 134, Name = "stained white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 135, Name = "blotched white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 136, Name = "dingy off-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 137, Name = "stained ivory", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 138, Name = "shabby sallow-coloured", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 139, Name = "lurid pale yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 140, Name = "dingy yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 141, Name = "gaudy mustard yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 142, Name = "sickly pale yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 143, Name = "shabby pale yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 144, Name = "murky brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 145, Name = "stained brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 146, Name = "dreary brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 147, Name = "bland brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 148, Name = "spotted muddy brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 149, Name = "dismal sand brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 150, Name = "dreary beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 151, Name = "grimy beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 152, Name = "shabby beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 153, Name = "dirty beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 154, Name = "tattered beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 155, Name = "bland wheat-coloured", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 156, Name = "drab olive", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 157, Name = "murky olive", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 158, Name = "dim olive", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 159, Name = "dingy green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 160, Name = "shabby green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 161, Name = "dull green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 162, Name = "sickly greyish-green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 163, Name = "grisly brownish-green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 164, Name = "discoloured green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 165, Name = "blotchy green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 166, Name = "grimy rust-red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 167, Name = "blotchy rust-red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 168, Name = "grimy salmon", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 169, Name = "stained salmon", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 170, Name = "blotched red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 171, Name = "dull red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 172, Name = "faded red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 173, Name = "stained red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 174, Name = "dingy red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 175, Name = "faded salmon", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 176, Name = "well-worn blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 177, Name = "faded slate blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 178, Name = "pallid blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 179, Name = "stained blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 180, Name = "grimy blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 181, Name = "dim blue-black", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 182, Name = "faded blue-black", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 183, Name = "dreary blue-black", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 184, Name = "dull orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 185, Name = "faded reddish-orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 186, Name = "tattered reddish-orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 187, Name = "discoloured orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 188, Name = "stained orange-red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 189, Name = "drab peach-coloured", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 190, Name = "lurid peach-coloured", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 191, Name = "sickly peach-coloured", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 192, Name = "tattered violet", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 193, Name = "grimy lavender", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 194, Name = "spotted lavender", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 195, Name = "discoloured purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 196, Name = "dirty purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 197, Name = "dingy purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 198, Name = "faded purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 199, Name = "stained purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
			{ Id = 200, Name = "dusty faded purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
		context.Colours.Add(new Colour
		{
			Id = 201, Name = "blonde", Basic = 6, Red = 0, Green = 125, Blue = 125, Fancy = "a fair, yellow blonde"
		});
		context.Colours.Add(new Colour
		{
			Id = 202, Name = "dirty blonde", Basic = 6, Red = 0, Green = 125, Blue = 125,
			Fancy = "a darker brownish blonde"
		});
		context.Colours.Add(new Colour
		{
			Id = 203, Name = "silver blonde", Basic = 6, Red = 255, Green = 255, Blue = 255,
			Fancy = "a light blonde with a silvery hue"
		});
		context.Colours.Add(new Colour
		{
			Id = 204, Name = "ash blonde", Basic = 6, Red = 0, Green = 55, Blue = 55, Fancy = "an ashen, greyish blonde"
		});
		context.Colours.Add(new Colour
		{
			Id = 205, Name = "strawberry blonde", Basic = 6, Red = 55, Green = 100, Blue = 100,
			Fancy = "the colour of blonde with a reddish tinge"
		});
		context.Colours.Add(new Colour
		{
			Id = 206, Name = "platinum blonde", Basic = 6, Red = 0, Green = 25, Blue = 25,
			Fancy = "an almost whitish shade of blonde"
		});
		context.Colours.Add(new Colour
		{
			Id = 207, Name = "light blonde", Basic = 6, Red = 0, Green = 55, Blue = 55,
			Fancy = "a pure, light shade of blonde"
		});
		context.Colours.Add(new Colour
		{
			Id = 208, Name = "salt-and-pepper", Basic = 0, Red = 0, Green = 0, Blue = 0,
			Fancy = "a dark greyish-black with speckles of grey"
		});
		context.Colours.Add(new Colour
			{ Id = 209, Name = "orange", Basic = 7, Red = 200, Green = 100, Blue = 100, Fancy = "a pure orange" });
		context.Colours.Add(new Colour
		{
			Id = 210, Name = "light blue", Basic = 4, Red = 50, Green = 50, Blue = 255, Fancy = "a light shade of blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 211, Name = "light green", Basic = 5, Red = 0, Green = 100, Blue = 0, Fancy = "a light shade of green"
		});
		context.Colours.Add(new Colour
		{
			Id = 212, Name = "pale blue", Basic = 4, Red = 0, Green = 0, Blue = 200, Fancy = "a pale shade of blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 213, Name = "yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "a pure shade of yellow"
		});
		context.Colours.Add(new Colour
			{ Id = 214, Name = "cyan", Basic = 11, Red = 0, Green = 75, Blue = 255, Fancy = "a light, greenish blue" });
		context.Colours.Add(new Colour
		{
			Id = 215, Name = "navy blue", Basic = 4, Red = 0, Green = 0, Blue = 180,
			Fancy = "a very dark shade of the colour blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 216, Name = "reddish brown", Basic = 10, Red = 200, Green = 155, Blue = 10,
			Fancy = "the colour of brown with a tinge of red"
		});
		context.Colours.Add(new Colour
		{
			Id = 217, Name = "beige", Basic = 10, Red = 75, Green = 75, Blue = 0,
			Fancy = "the pale brown of natural wool"
		});
		context.Colours.Add(new Colour
		{
			Id = 218, Name = "light red", Basic = 3, Red = 115, Green = 0, Blue = 0, Fancy = "a light shade of red"
		});
		context.Colours.Add(new Colour
		{
			Id = 219, Name = "purple", Basic = 8, Red = 180, Green = 180, Blue = 0, Fancy = "a pure shade of purple"
		});
		context.Colours.Add(new Colour
			{ Id = 220, Name = "pink", Basic = 7, Red = 255, Green = 245, Blue = 245, Fancy = "a pure shade of pink" });
		context.Colours.Add(new Colour
		{
			Id = 221, Name = "dark", Basic = 10, Red = 225, Green = 225, Blue = 0,
			Fancy = "a dark brown verging on black"
		});
		context.Colours.Add(new Colour
		{
			Id = 222, Name = "indian red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "the colour of indian red"
		});
		context.Colours.Add(new Colour
		{
			Id = 223, Name = "light pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of light pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 224, Name = "violet red", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the colour of violet red"
		});
		context.Colours.Add(new Colour
		{
			Id = 225, Name = "hot pink", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "the colour of hot pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 226, Name = "maroon red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "the colour of maroon red"
		});
		context.Colours.Add(new Colour
		{
			Id = 227, Name = "plum purple", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the colour of plum purple"
		});
		context.Colours.Add(new Colour
		{
			Id = 228, Name = "magenta red", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the colour of magenta red"
		});
		context.Colours.Add(new Colour
		{
			Id = 229, Name = "cobalt blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the strikingly rich, deep blue colour of cobalt"
		});
		context.Colours.Add(new Colour
		{
			Id = 230, Name = "light steel blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of light steel blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 231, Name = "slate gray", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of slate gray"
		});
		context.Colours.Add(new Colour
		{
			Id = 232, Name = "turquoise blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of turquoise blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 233, Name = "cyan blue", Basic = 11, Red = 0, Green = 75, Blue = 255, Fancy = "the colour of cyan blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 234, Name = "cobalt green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the colour of cobalt green"
		});
		context.Colours.Add(new Colour
		{
			Id = 235, Name = "lime green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "the colour of lime green"
		});
		context.Colours.Add(new Colour
		{
			Id = 236, Name = "ivory white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of ivory white"
		});
		context.Colours.Add(new Colour
		{
			Id = 237, Name = "goldenrod yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of goldenrod yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 238, Name = "dark khaki", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of dark khaki"
		});
		context.Colours.Add(new Colour
		{
			Id = 239, Name = "banana yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of banana yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 240, Name = "orange red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "the colour of orange red"
		});
		context.Colours.Add(new Colour
		{
			Id = 241, Name = "moccasin brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of moccasin brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 242, Name = "tan yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of tan yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 243, Name = "brick brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of brick brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 244, Name = "carrot orange", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the colour of carrot orange"
		});
		context.Colours.Add(new Colour
		{
			Id = 245, Name = "peachpuff pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of peachpuff pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 246, Name = "sienna brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of sienna brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 247, Name = "saddle brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of saddle brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 248, Name = "salmon pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of salmon pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 249, Name = "sepia brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of sepia brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 250, Name = "fire brick brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of fire brick brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 251, Name = "teal blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "the colour of teal blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 252, Name = "dark gray", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of dark gray"
		});
		context.Colours.Add(new Colour
		{
			Id = 253, Name = "pale violet", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the colour of pale violet"
		});
		context.Colours.Add(new Colour
		{
			Id = 254, Name = "violet red", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the colour of violet red"
		});
		context.Colours.Add(new Colour
		{
			Id = 255, Name = "lavender pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of lavender pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 256, Name = "hot pink", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "the colour of hot pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 257, Name = "deep pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of deep pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 258, Name = "maroon red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "the colour of maroon red"
		});
		context.Colours.Add(new Colour
		{
			Id = 259, Name = "orchid pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of orchid pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 260, Name = "plum purple", Basic = 8, Red = 128, Green = 0, Blue = 128,
			Fancy = "the colour of plum purple"
		});
		context.Colours.Add(new Colour
		{
			Id = 261, Name = "fuchsia pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of fuchsia pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 262, Name = "magenta red", Basic = 3, Red = 255, Green = 0, Blue = 0,
			Fancy = "the colour of magenta red"
		});
		context.Colours.Add(new Colour
		{
			Id = 263, Name = "midnight blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "darkest night-sky blue, easily mistaken for black in poor lighting"
		});
		context.Colours.Add(new Colour
		{
			Id = 264, Name = "cobalt blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the strikingly rich, deep blue colour of cobalt"
		});
		context.Colours.Add(new Colour
		{
			Id = 265, Name = "cornflower blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of cornflower blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 266, Name = "light steel blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of light steel blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 267, Name = "steel blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "the colour of steel blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 268, Name = "slate gray", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of slate gray"
		});
		context.Colours.Add(new Colour
			{ Id = 269, Name = "gray", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "the colour of gray" });
		context.Colours.Add(new Colour
		{
			Id = 270, Name = "turquoise blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of turquoise blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 271, Name = "azure blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "the colour of azure blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 272, Name = "cyan blue", Basic = 11, Red = 0, Green = 75, Blue = 255, Fancy = "the colour of cyan blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 273, Name = "aquamarine", Basic = 11, Red = 0, Green = 75, Blue = 255,
			Fancy = "the colour of aquamarine"
		});
		context.Colours.Add(new Colour
		{
			Id = 274, Name = "cobalt green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the colour of cobalt green"
		});
		context.Colours.Add(new Colour
		{
			Id = 275, Name = "mint green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "the colour of mint green"
		});
		context.Colours.Add(new Colour
		{
			Id = 276, Name = "lime green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "the colour of lime green"
		});
		context.Colours.Add(new Colour
		{
			Id = 277, Name = "chartreuse green", Basic = 5, Red = 0, Green = 255, Blue = 0,
			Fancy = "the colour of chartreuse green"
		});
		context.Colours.Add(new Colour
		{
			Id = 278, Name = "ivory white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of ivory white"
		});
		context.Colours.Add(new Colour
		{
			Id = 279, Name = "light yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of light yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 280, Name = "goldenrod yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of goldenrod yellow"
		});
		context.Colours.Add(new Colour
			{ Id = 281, Name = "khaki", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "the colour of khaki" });
		context.Colours.Add(new Colour
		{
			Id = 282, Name = "dark khaki", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of dark khaki"
		});
		context.Colours.Add(new Colour
			{ Id = 283, Name = "gold", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "the colour of gold" });
		context.Colours.Add(new Colour
		{
			Id = 284, Name = "banana yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of banana yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 285, Name = "cornsilk yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of cornsilk yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 286, Name = "orange red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "the colour of orange red"
		});
		context.Colours.Add(new Colour
		{
			Id = 287, Name = "wheat yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of wheat yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 288, Name = "moccasin brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of moccasin brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 289, Name = "eggshell white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of eggshell white"
		});
		context.Colours.Add(new Colour
		{
			Id = 290, Name = "tan yellow", Basic = 6, Red = 255, Green = 255, Blue = 0,
			Fancy = "the colour of tan yellow"
		});
		context.Colours.Add(new Colour
		{
			Id = 291, Name = "tan brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of tan brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 292, Name = "brick brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of brick brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 293, Name = "brick red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "the colour of brick red"
		});
		context.Colours.Add(new Colour
		{
			Id = 294, Name = "carrot orange", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the colour of carrot orange"
		});
		context.Colours.Add(new Colour
		{
			Id = 295, Name = "dark orange", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the colour of dark orange"
		});
		context.Colours.Add(new Colour
		{
			Id = 296, Name = "peachpuff pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of peachpuff pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 297, Name = "seashell gray", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of seashell gray"
		});
		context.Colours.Add(new Colour
		{
			Id = 298, Name = "sienna brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of sienna brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 299, Name = "chocolate brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of chocolate brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 300, Name = "saddle brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of saddle brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 301, Name = "light salmon pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of light salmon pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 302, Name = "salmon pink", Basic = 9, Red = 255, Green = 192, Blue = 203,
			Fancy = "the colour of salmon pink"
		});
		context.Colours.Add(new Colour
		{
			Id = 303, Name = "coral orange", Basic = 7, Red = 255, Green = 165, Blue = 0,
			Fancy = "the colour of coral orange"
		});
		context.Colours.Add(new Colour
		{
			Id = 304, Name = "sepia brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of sepia brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 305, Name = "orange brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of orange brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 306, Name = "fire brick brown", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the colour of fire brick brown"
		});
		context.Colours.Add(new Colour
		{
			Id = 307, Name = "beet red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "the colour of beet red"
		});
		context.Colours.Add(new Colour
		{
			Id = 308, Name = "teal blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "the colour of teal blue"
		});
		context.Colours.Add(new Colour
		{
			Id = 309, Name = "smoky white", Basic = 1, Red = 255, Green = 255, Blue = 255,
			Fancy = "the colour of smoky white"
		});
		context.Colours.Add(new Colour
		{
			Id = 310, Name = "dark gray", Basic = 2, Red = 127, Green = 127, Blue = 127,
			Fancy = "the colour of dark gray"
		});
		context.Colours.Add(new Colour
		{
			Id = 311, Name = "gray black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "the colour of gray black"
		});
		context.Colours.Add(new Colour
		{
			Id = 312, Name = "deep blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "a blue that's several shades darker than most"
		});
		context.Colours.Add(new Colour
		{
			Id = 313, Name = "winter blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of a pale, cold winter sky"
		});
		context.Colours.Add(new Colour
		{
			Id = 314, Name = "storm blue", Basic = 4, Red = 0, Green = 0, Blue = 255,
			Fancy = "the colour of a storm at sea, flat slatey blue with shadowy depths"
		});
		context.Colours.Add(new Colour
		{
			Id = 315, Name = "natural", Basic = 10, Red = 175, Green = 175, Blue = 0,
			Fancy = "the natural colour of lips, unadorned by makeup"
		});

		var nextId = context.CharacteristicValues.Select(x => x.Id).Max() + 1;
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "black", DefinitionId = colourDef.Id, Value = "1", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "white", DefinitionId = colourDef.Id, Value = "2", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grey", DefinitionId = colourDef.Id, Value = "3", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light grey", DefinitionId = colourDef.Id, Value = "4", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark grey", DefinitionId = colourDef.Id, Value = "5", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "red", DefinitionId = colourDef.Id, Value = "6", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark red", DefinitionId = colourDef.Id, Value = "7", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blue", DefinitionId = colourDef.Id, Value = "8", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark blue", DefinitionId = colourDef.Id, Value = "9", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "green", DefinitionId = colourDef.Id, Value = "10", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "brown", DefinitionId = colourDef.Id, Value = "11", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark green", DefinitionId = colourDef.Id, Value = "12", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "hazel", DefinitionId = colourDef.Id, Value = "13", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pale white", DefinitionId = colourDef.Id, Value = "14", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "olive", DefinitionId = colourDef.Id, Value = "15", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "caramel", DefinitionId = colourDef.Id, Value = "16", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ebony", DefinitionId = colourDef.Id, Value = "17", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "emerald green", DefinitionId = colourDef.Id, Value = "18", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cerulean", DefinitionId = colourDef.Id, Value = "19", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "violet", DefinitionId = colourDef.Id, Value = "20", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sandy brown", DefinitionId = colourDef.Id, Value = "21", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light brown", DefinitionId = colourDef.Id, Value = "22", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark brown", DefinitionId = colourDef.Id, Value = "23", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "auburn", DefinitionId = colourDef.Id, Value = "24", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ebony", DefinitionId = colourDef.Id, Value = "25", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "onyx", DefinitionId = colourDef.Id, Value = "26", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "obsidian", DefinitionId = colourDef.Id, Value = "27", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "midnight black", DefinitionId = colourDef.Id, Value = "28", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ink black", DefinitionId = colourDef.Id, Value = "29", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "jet black", DefinitionId = colourDef.Id, Value = "30", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pitch black", DefinitionId = colourDef.Id, Value = "31", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ivory", DefinitionId = colourDef.Id, Value = "32", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "seashell", DefinitionId = colourDef.Id, Value = "33", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "snow white", DefinitionId = colourDef.Id, Value = "34", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "gleaming white", DefinitionId = colourDef.Id, Value = "35", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pure white", DefinitionId = colourDef.Id, Value = "36", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pearl white", DefinitionId = colourDef.Id, Value = "37", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "bright white", DefinitionId = colourDef.Id, Value = "38", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "bone white", DefinitionId = colourDef.Id, Value = "39", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ghost white", DefinitionId = colourDef.Id, Value = "40", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "mist grey", DefinitionId = colourDef.Id, Value = "41", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "charcoal grey", DefinitionId = colourDef.Id, Value = "42", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "thistle grey", DefinitionId = colourDef.Id, Value = "43", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "smoky grey", DefinitionId = colourDef.Id, Value = "44", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "slate grey", DefinitionId = colourDef.Id, Value = "45", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "silver grey", DefinitionId = colourDef.Id, Value = "46", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "soft grey", DefinitionId = colourDef.Id, Value = "47", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ash grey", DefinitionId = colourDef.Id, Value = "48", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "crimson", DefinitionId = colourDef.Id, Value = "49", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "scarlet", DefinitionId = colourDef.Id, Value = "50", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ruby red", DefinitionId = colourDef.Id, Value = "51", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blood red", DefinitionId = colourDef.Id, Value = "52", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "rose red", DefinitionId = colourDef.Id, Value = "53", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "wine red", DefinitionId = colourDef.Id, Value = "54", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "flame red", DefinitionId = colourDef.Id, Value = "55", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "coral", DefinitionId = colourDef.Id, Value = "56", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "copper", DefinitionId = colourDef.Id, Value = "57", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "fiery orange", DefinitionId = colourDef.Id, Value = "58", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ochre", DefinitionId = colourDef.Id, Value = "59", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sunset orange", DefinitionId = colourDef.Id, Value = "60", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "amber", DefinitionId = colourDef.Id, Value = "61", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "goldenrod", DefinitionId = colourDef.Id, Value = "62", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pale yellow", DefinitionId = colourDef.Id, Value = "63", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "golden yellow", DefinitionId = colourDef.Id, Value = "64", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sand yellow", DefinitionId = colourDef.Id, Value = "65", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "topaz hued", DefinitionId = colourDef.Id, Value = "66", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "gold-coloured", DefinitionId = colourDef.Id, Value = "67", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "spring green", DefinitionId = colourDef.Id, Value = "68", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sea green", DefinitionId = colourDef.Id, Value = "69", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "hunter green", DefinitionId = colourDef.Id, Value = "70", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "olive green", DefinitionId = colourDef.Id, Value = "71", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sage green", DefinitionId = colourDef.Id, Value = "72", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pine green", DefinitionId = colourDef.Id, Value = "73", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "bright green", DefinitionId = colourDef.Id, Value = "74", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "rich green", DefinitionId = colourDef.Id, Value = "75", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pale green", DefinitionId = colourDef.Id, Value = "76", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "verdant green", DefinitionId = colourDef.Id, Value = "77", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "forest green", DefinitionId = colourDef.Id, Value = "78", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "chartreuse", DefinitionId = colourDef.Id, Value = "79", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "slate blue", DefinitionId = colourDef.Id, Value = "80", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "bright blue", DefinitionId = colourDef.Id, Value = "81", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "powder blue", DefinitionId = colourDef.Id, Value = "82", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sapphire blue", DefinitionId = colourDef.Id, Value = "83", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "royal blue", DefinitionId = colourDef.Id, Value = "84", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ocean blue", DefinitionId = colourDef.Id, Value = "85", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "teal", DefinitionId = colourDef.Id, Value = "86", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cornflour blue", DefinitionId = colourDef.Id, Value = "87", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sky blue", DefinitionId = colourDef.Id, Value = "88", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "azure", DefinitionId = colourDef.Id, Value = "89", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "beryl", DefinitionId = colourDef.Id, Value = "90", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cerulean", DefinitionId = colourDef.Id, Value = "91", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cobalt", DefinitionId = colourDef.Id, Value = "92", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "rich indigo", DefinitionId = colourDef.Id, Value = "93", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "deep indigo", DefinitionId = colourDef.Id, Value = "94", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "vivid indigo", DefinitionId = colourDef.Id, Value = "95", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "earthen brown", DefinitionId = colourDef.Id, Value = "96", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "deep brown", DefinitionId = colourDef.Id, Value = "97", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "rich brown", DefinitionId = colourDef.Id, Value = "98", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "burnt sienna", DefinitionId = colourDef.Id, Value = "99", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "chocolate", DefinitionId = colourDef.Id, Value = "100", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cinnamon", DefinitionId = colourDef.Id, Value = "101", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "mahogany", DefinitionId = colourDef.Id, Value = "102", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "nut brown", DefinitionId = colourDef.Id, Value = "103", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "umber", DefinitionId = colourDef.Id, Value = "104", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "amethyst", DefinitionId = colourDef.Id, Value = "105", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "mauve", DefinitionId = colourDef.Id, Value = "106", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "mulbery", DefinitionId = colourDef.Id, Value = "107", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "plum", DefinitionId = colourDef.Id, Value = "108", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "lavender", DefinitionId = colourDef.Id, Value = "109", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "royal purple", DefinitionId = colourDef.Id, Value = "110", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded black", DefinitionId = colourDef.Id, Value = "111", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "tattered black", DefinitionId = colourDef.Id, Value = "112", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "shabby black", DefinitionId = colourDef.Id, Value = "113", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grimy black", DefinitionId = colourDef.Id, Value = "114", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "off-white", DefinitionId = colourDef.Id, Value = "115", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dingy grey", DefinitionId = colourDef.Id, Value = "116", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blotched red", DefinitionId = colourDef.Id, Value = "117", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dull orange", DefinitionId = colourDef.Id, Value = "118", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "bland yellow", DefinitionId = colourDef.Id, Value = "119", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded green", DefinitionId = colourDef.Id, Value = "120", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded blue", DefinitionId = colourDef.Id, Value = "121", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded indigo", DefinitionId = colourDef.Id, Value = "122", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded purple", DefinitionId = colourDef.Id, Value = "123", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "drab brown", DefinitionId = colourDef.Id, Value = "124", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dim grey", DefinitionId = colourDef.Id, Value = "125", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dusky slate grey", DefinitionId = colourDef.Id, Value = "126", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sooty grey", DefinitionId = colourDef.Id, Value = "127", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "chalky pale grey", DefinitionId = colourDef.Id, Value = "128", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dull mist grey", DefinitionId = colourDef.Id, Value = "129", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ashen off-white", DefinitionId = colourDef.Id, Value = "130", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dirty bone-white", DefinitionId = colourDef.Id, Value = "131", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "wan ivory", DefinitionId = colourDef.Id, Value = "132", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "spotted white", DefinitionId = colourDef.Id, Value = "133", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained white", DefinitionId = colourDef.Id, Value = "134", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blotched white", DefinitionId = colourDef.Id, Value = "135", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dingy off-white", DefinitionId = colourDef.Id, Value = "136", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained ivory", DefinitionId = colourDef.Id, Value = "137", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "shabby sallow-coloured", DefinitionId = colourDef.Id, Value = "138", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "lurid pale yellow", DefinitionId = colourDef.Id, Value = "139", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dingy yellow", DefinitionId = colourDef.Id, Value = "140", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "gaudy mustard yellow", DefinitionId = colourDef.Id, Value = "141", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sickly pale yellow", DefinitionId = colourDef.Id, Value = "142", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "shabby pale yellow", DefinitionId = colourDef.Id, Value = "143", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "murky brown", DefinitionId = colourDef.Id, Value = "144", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained brown", DefinitionId = colourDef.Id, Value = "145", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dreary brown", DefinitionId = colourDef.Id, Value = "146", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "bland brown", DefinitionId = colourDef.Id, Value = "147", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "spotted muddy brown", DefinitionId = colourDef.Id, Value = "148", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dismal sand brown", DefinitionId = colourDef.Id, Value = "149", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dreary beige", DefinitionId = colourDef.Id, Value = "150", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grimy beige", DefinitionId = colourDef.Id, Value = "151", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "shabby beige", DefinitionId = colourDef.Id, Value = "152", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dirty beige", DefinitionId = colourDef.Id, Value = "153", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "tattered beige", DefinitionId = colourDef.Id, Value = "154", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "bland wheat-coloured", DefinitionId = colourDef.Id, Value = "155", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "drab olive", DefinitionId = colourDef.Id, Value = "156", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "murky olive", DefinitionId = colourDef.Id, Value = "157", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dim olive", DefinitionId = colourDef.Id, Value = "158", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dingy green", DefinitionId = colourDef.Id, Value = "159", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "shabby green", DefinitionId = colourDef.Id, Value = "160", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dull green", DefinitionId = colourDef.Id, Value = "161", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sickly greyish-green", DefinitionId = colourDef.Id, Value = "162", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grisly brownish-green", DefinitionId = colourDef.Id, Value = "163", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "discoloured green", DefinitionId = colourDef.Id, Value = "164", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blotchy green", DefinitionId = colourDef.Id, Value = "165", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grimy rust-red", DefinitionId = colourDef.Id, Value = "166", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blotchy rust-red", DefinitionId = colourDef.Id, Value = "167", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grimy salmon", DefinitionId = colourDef.Id, Value = "168", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained salmon", DefinitionId = colourDef.Id, Value = "169", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blotched red", DefinitionId = colourDef.Id, Value = "170", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dull red", DefinitionId = colourDef.Id, Value = "171", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded red", DefinitionId = colourDef.Id, Value = "172", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained red", DefinitionId = colourDef.Id, Value = "173", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dingy red", DefinitionId = colourDef.Id, Value = "174", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded salmon", DefinitionId = colourDef.Id, Value = "175", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "well-worn blue", DefinitionId = colourDef.Id, Value = "176", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded slate blue", DefinitionId = colourDef.Id, Value = "177", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pallid blue", DefinitionId = colourDef.Id, Value = "178", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained blue", DefinitionId = colourDef.Id, Value = "179", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grimy blue", DefinitionId = colourDef.Id, Value = "180", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dim blue-black", DefinitionId = colourDef.Id, Value = "181", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded blue-black", DefinitionId = colourDef.Id, Value = "182", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dreary blue-black", DefinitionId = colourDef.Id, Value = "183", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dull orange", DefinitionId = colourDef.Id, Value = "184", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded reddish-orange", DefinitionId = colourDef.Id, Value = "185", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "tattered reddish-orange", DefinitionId = colourDef.Id, Value = "186",
			Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "discoloured orange", DefinitionId = colourDef.Id, Value = "187", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained orange-red", DefinitionId = colourDef.Id, Value = "188", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "drab peach-coloured", DefinitionId = colourDef.Id, Value = "189", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "lurid peach-coloured", DefinitionId = colourDef.Id, Value = "190", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sickly peach-coloured", DefinitionId = colourDef.Id, Value = "191", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "tattered violet", DefinitionId = colourDef.Id, Value = "192", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "grimy lavender", DefinitionId = colourDef.Id, Value = "193", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "spotted lavender", DefinitionId = colourDef.Id, Value = "194", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "discoloured purple", DefinitionId = colourDef.Id, Value = "195", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dirty purple", DefinitionId = colourDef.Id, Value = "196", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dingy purple", DefinitionId = colourDef.Id, Value = "197", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "faded purple", DefinitionId = colourDef.Id, Value = "198", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "stained purple", DefinitionId = colourDef.Id, Value = "199", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dusty faded purple", DefinitionId = colourDef.Id, Value = "200", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "blonde", DefinitionId = colourDef.Id, Value = "201", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dirty blonde", DefinitionId = colourDef.Id, Value = "202", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "silver blonde", DefinitionId = colourDef.Id, Value = "203", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ash blonde", DefinitionId = colourDef.Id, Value = "204", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "strawberry blonde", DefinitionId = colourDef.Id, Value = "205", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "platinum blonde", DefinitionId = colourDef.Id, Value = "206", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light blonde", DefinitionId = colourDef.Id, Value = "207", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "salt-and-pepper", DefinitionId = colourDef.Id, Value = "208", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "orange", DefinitionId = colourDef.Id, Value = "209", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light blue", DefinitionId = colourDef.Id, Value = "210", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light green", DefinitionId = colourDef.Id, Value = "211", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pale blue", DefinitionId = colourDef.Id, Value = "212", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "yellow", DefinitionId = colourDef.Id, Value = "213", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cyan", DefinitionId = colourDef.Id, Value = "214", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "navy blue", DefinitionId = colourDef.Id, Value = "215", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "reddish brown", DefinitionId = colourDef.Id, Value = "216", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "beige", DefinitionId = colourDef.Id, Value = "217", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light red", DefinitionId = colourDef.Id, Value = "218", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "purple", DefinitionId = colourDef.Id, Value = "219", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pink", DefinitionId = colourDef.Id, Value = "220", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark", DefinitionId = colourDef.Id, Value = "221", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "indian red", DefinitionId = colourDef.Id, Value = "222", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light pink", DefinitionId = colourDef.Id, Value = "223", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "violet red", DefinitionId = colourDef.Id, Value = "224", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "hot pink", DefinitionId = colourDef.Id, Value = "225", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "maroon red", DefinitionId = colourDef.Id, Value = "226", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "plum purple", DefinitionId = colourDef.Id, Value = "227", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "magenta red", DefinitionId = colourDef.Id, Value = "228", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cobalt blue", DefinitionId = colourDef.Id, Value = "229", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light steel blue", DefinitionId = colourDef.Id, Value = "230", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "slate gray", DefinitionId = colourDef.Id, Value = "231", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "turquoise blue", DefinitionId = colourDef.Id, Value = "232", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cyan blue", DefinitionId = colourDef.Id, Value = "233", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cobalt green", DefinitionId = colourDef.Id, Value = "234", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "lime green", DefinitionId = colourDef.Id, Value = "235", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ivory white", DefinitionId = colourDef.Id, Value = "236", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "goldenrod yellow", DefinitionId = colourDef.Id, Value = "237", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark khaki", DefinitionId = colourDef.Id, Value = "238", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "banana yellow", DefinitionId = colourDef.Id, Value = "239", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "orange red", DefinitionId = colourDef.Id, Value = "240", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "moccasin brown", DefinitionId = colourDef.Id, Value = "241", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "tan yellow", DefinitionId = colourDef.Id, Value = "242", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "brick brown", DefinitionId = colourDef.Id, Value = "243", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "carrot orange", DefinitionId = colourDef.Id, Value = "244", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "peachpuff pink", DefinitionId = colourDef.Id, Value = "245", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sienna brown", DefinitionId = colourDef.Id, Value = "246", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "saddle brown", DefinitionId = colourDef.Id, Value = "247", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "salmon pink", DefinitionId = colourDef.Id, Value = "248", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sepia brown", DefinitionId = colourDef.Id, Value = "249", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "fire brick brown", DefinitionId = colourDef.Id, Value = "250", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "teal blue", DefinitionId = colourDef.Id, Value = "251", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark gray", DefinitionId = colourDef.Id, Value = "252", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "pale violet", DefinitionId = colourDef.Id, Value = "253", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "violet red", DefinitionId = colourDef.Id, Value = "254", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "lavender pink", DefinitionId = colourDef.Id, Value = "255", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "hot pink", DefinitionId = colourDef.Id, Value = "256", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "deep pink", DefinitionId = colourDef.Id, Value = "257", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "maroon red", DefinitionId = colourDef.Id, Value = "258", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "orchid pink", DefinitionId = colourDef.Id, Value = "259", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "plum purple", DefinitionId = colourDef.Id, Value = "260", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "fuchsia pink", DefinitionId = colourDef.Id, Value = "261", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "magenta red", DefinitionId = colourDef.Id, Value = "262", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "midnight blue", DefinitionId = colourDef.Id, Value = "263", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cobalt blue", DefinitionId = colourDef.Id, Value = "264", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cornflower blue", DefinitionId = colourDef.Id, Value = "265", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light steel blue", DefinitionId = colourDef.Id, Value = "266", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "steel blue", DefinitionId = colourDef.Id, Value = "267", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "slate gray", DefinitionId = colourDef.Id, Value = "268", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "gray", DefinitionId = colourDef.Id, Value = "269", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "turquoise blue", DefinitionId = colourDef.Id, Value = "270", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "azure blue", DefinitionId = colourDef.Id, Value = "271", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cyan blue", DefinitionId = colourDef.Id, Value = "272", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "aquamarine", DefinitionId = colourDef.Id, Value = "273", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cobalt green", DefinitionId = colourDef.Id, Value = "274", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "mint green", DefinitionId = colourDef.Id, Value = "275", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "lime green", DefinitionId = colourDef.Id, Value = "276", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "chartreuse green", DefinitionId = colourDef.Id, Value = "277", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "ivory white", DefinitionId = colourDef.Id, Value = "278", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light yellow", DefinitionId = colourDef.Id, Value = "279", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "goldenrod yellow", DefinitionId = colourDef.Id, Value = "280", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "khaki", DefinitionId = colourDef.Id, Value = "281", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark khaki", DefinitionId = colourDef.Id, Value = "282", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "gold", DefinitionId = colourDef.Id, Value = "283", Default = false, Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "banana yellow", DefinitionId = colourDef.Id, Value = "284", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "cornsilk yellow", DefinitionId = colourDef.Id, Value = "285", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "orange red", DefinitionId = colourDef.Id, Value = "286", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "wheat yellow", DefinitionId = colourDef.Id, Value = "287", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "moccasin brown", DefinitionId = colourDef.Id, Value = "288", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "eggshell white", DefinitionId = colourDef.Id, Value = "289", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "tan yellow", DefinitionId = colourDef.Id, Value = "290", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "tan brown", DefinitionId = colourDef.Id, Value = "291", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "brick brown", DefinitionId = colourDef.Id, Value = "292", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "brick red", DefinitionId = colourDef.Id, Value = "293", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "carrot orange", DefinitionId = colourDef.Id, Value = "294", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark orange", DefinitionId = colourDef.Id, Value = "295", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "peachpuff pink", DefinitionId = colourDef.Id, Value = "296", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "seashell gray", DefinitionId = colourDef.Id, Value = "297", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sienna brown", DefinitionId = colourDef.Id, Value = "298", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "chocolate brown", DefinitionId = colourDef.Id, Value = "299", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "saddle brown", DefinitionId = colourDef.Id, Value = "300", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "light salmon pink", DefinitionId = colourDef.Id, Value = "301", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "salmon pink", DefinitionId = colourDef.Id, Value = "302", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "coral orange", DefinitionId = colourDef.Id, Value = "303", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "sepia brown", DefinitionId = colourDef.Id, Value = "304", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "orange brown", DefinitionId = colourDef.Id, Value = "305", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "fire brick brown", DefinitionId = colourDef.Id, Value = "306", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "beet red", DefinitionId = colourDef.Id, Value = "307", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "teal blue", DefinitionId = colourDef.Id, Value = "308", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "smoky white", DefinitionId = colourDef.Id, Value = "309", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "dark gray", DefinitionId = colourDef.Id, Value = "310", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "gray black", DefinitionId = colourDef.Id, Value = "311", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "deep blue", DefinitionId = colourDef.Id, Value = "312", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "winter blue", DefinitionId = colourDef.Id, Value = "313", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "storm blue", DefinitionId = colourDef.Id, Value = "314", Default = false,
			Pluralisation = 0
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Name = "natural", DefinitionId = colourDef.Id, Value = "315", Default = false,
			Pluralisation = 0
		});

		context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All_Colours",
			Type = "all",
			Definition = "<Definition/>",
			TargetDefinitionId = colourDef.Id,
			Description = "All defined colours"
		});

		context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Basic_Colours",
			Type = "Standard",
			Definition =
				"<Values> <Value>black</Value> <Value>white</Value> <Value>grey</Value> <Value>light grey</Value> <Value>dark grey</Value> <Value>red</Value> <Value>dark red</Value> <Value>blue</Value> <Value>dark blue</Value> <Value>green</Value> <Value>brown</Value> <Value>dark green</Value> <Value>orange</Value> <Value>light blue</Value> <Value>light green</Value> <Value>yellow</Value> <Value>light red</Value> <Value>purple</Value> <Value>pink</Value> </Values>",
			TargetDefinitionId = colourDef.Id,
			Description = "Just basic colours like red, blue, brown etc"
		});

		context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Fine_Colours",
			Type = "Standard",
			Definition =
				"<Values> <Value>light grey</Value> <Value>dark grey</Value> <Value>red</Value> <Value>dark red</Value> <Value>blue</Value> <Value>dark blue</Value> <Value>green</Value> <Value>brown</Value> <Value>dark green</Value> <Value>pale white</Value> <Value>olive</Value> <Value>caramel</Value> <Value>ebony</Value> <Value>emerald green</Value> <Value>cerulean</Value> <Value>violet</Value> <Value>sandy brown</Value> <Value>light brown</Value> <Value>dark brown</Value> <Value>auburn</Value> <Value>onyx</Value> <Value>obsidian</Value> <Value>midnight black</Value> <Value>ink black</Value> <Value>jet black</Value> <Value>pitch black</Value> <Value>ivory</Value> <Value>seashell</Value> <Value>snow white</Value> <Value>gleaming white</Value> <Value>pure white</Value> <Value>pearl white</Value> <Value>bright white</Value> <Value>bone white</Value> <Value>ghost white</Value> <Value>mist grey</Value> <Value>charcoal grey</Value> <Value>thistle grey</Value> <Value>smoky grey</Value> <Value>slate grey</Value> <Value>silver grey</Value> <Value>soft grey</Value> <Value>ash grey</Value> <Value>crimson</Value> <Value>scarlet</Value> <Value>ruby red</Value> <Value>blood red</Value> <Value>rose red</Value> <Value>wine red</Value> <Value>flame red</Value> <Value>coral</Value> <Value>copper</Value> <Value>fiery orange</Value> <Value>ochre</Value> <Value>sunset orange</Value> <Value>amber</Value> <Value>goldenrod</Value> <Value>pale yellow</Value> <Value>golden yellow</Value> <Value>sand yellow</Value> <Value>topaz hued</Value> <Value>gold-coloured</Value> <Value>spring green</Value> <Value>sea green</Value> <Value>hunter green</Value> <Value>olive green</Value> <Value>sage green</Value> <Value>pine green</Value> <Value>bright green</Value> <Value>rich green</Value> <Value>pale green</Value> <Value>verdant green</Value> <Value>forest green</Value> <Value>chartreuse</Value> <Value>slate blue</Value> <Value>bright blue</Value> <Value>powder blue</Value> <Value>sapphire blue</Value> <Value>royal blue</Value> <Value>ocean blue</Value> <Value>teal</Value> <Value>cornflour blue</Value> <Value>sky blue</Value> <Value>azure</Value> <Value>beryl</Value> <Value>cobalt</Value> <Value>rich indigo</Value> <Value>deep indigo</Value> <Value>vivid indigo</Value> <Value>earthen brown</Value> <Value>deep brown</Value> <Value>rich brown</Value> <Value>burnt sienna</Value> <Value>chocolate</Value> <Value>cinnamon</Value> <Value>mahogany</Value> <Value>nut brown</Value> <Value>umber</Value> <Value>amethyst</Value> <Value>mauve</Value> <Value>mulbery</Value> <Value>plum</Value> <Value>lavender</Value> <Value>royal purple</Value> <Value>orange</Value> <Value>light blue</Value> <Value>light green</Value> <Value>pale blue</Value> <Value>yellow</Value> <Value>cyan</Value> <Value>navy blue</Value> <Value>reddish brown</Value> <Value>beige</Value> </Values>",
			TargetDefinitionId = colourDef.Id,
			Description = "All of the colours from the RPI Engine's $finecolor variable"
		});

		context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Drab_Colours",
			Type = "Standard",
			Definition =
				"<Values> <Value>faded black</Value> <Value>tattered black</Value> <Value>shabby black</Value> <Value>grimy black</Value> <Value>off-white</Value> <Value>dingy grey</Value> <Value>bland yellow</Value> <Value>faded green</Value> <Value>faded blue</Value> <Value>faded indigo</Value> <Value>drab brown</Value> <Value>dim grey</Value> <Value>dusky slate grey</Value> <Value>sooty grey</Value> <Value>chalky pale grey</Value> <Value>dull mist grey</Value> <Value>ashen off-white</Value> <Value>dirty bone-white</Value> <Value>wan ivory</Value> <Value>spotted white</Value> <Value>stained white</Value> <Value>blotched white</Value> <Value>dingy off-white</Value> <Value>stained ivory</Value> <Value>shabby sallow-coloured</Value> <Value>lurid pale yellow</Value> <Value>dingy yellow</Value> <Value>gaudy mustard yellow</Value> <Value>sickly pale yellow</Value> <Value>shabby pale yellow</Value> <Value>murky brown</Value> <Value>stained brown</Value> <Value>dreary brown</Value> <Value>bland brown</Value> <Value>spotted muddy brown</Value> <Value>dismal sand brown</Value> <Value>dreary beige</Value> <Value>grimy beige</Value> <Value>shabby beige</Value> <Value>dirty beige</Value> <Value>tattered beige</Value> <Value>bland wheat-coloured</Value> <Value>drab olive</Value> <Value>murky olive</Value> <Value>dim olive</Value> <Value>dingy green</Value> <Value>shabby green</Value> <Value>dull green</Value> <Value>sickly greyish-green</Value> <Value>grisly brownish-green</Value> <Value>discoloured green</Value> <Value>blotchy green</Value> <Value>grimy rust-red</Value> <Value>blotchy rust-red</Value> <Value>grimy salmon</Value> <Value>stained salmon</Value> <Value>blotched red</Value> <Value>dull red</Value> <Value>faded red</Value> <Value>stained red</Value> <Value>dingy red</Value> <Value>faded salmon</Value> <Value>well-worn blue</Value> <Value>faded slate blue</Value> <Value>pallid blue</Value> <Value>stained blue</Value> <Value>grimy blue</Value> <Value>dim blue-black</Value> <Value>faded blue-black</Value> <Value>dreary blue-black</Value> <Value>dull orange</Value> <Value>faded reddish-orange</Value> <Value>tattered reddish-orange</Value> <Value>discoloured orange</Value> <Value>stained orange-red</Value> <Value>drab peach-coloured</Value> <Value>lurid peach-coloured</Value> <Value>sickly peach-coloured</Value> <Value>tattered violet</Value> <Value>grimy lavender</Value> <Value>spotted lavender</Value> <Value>discoloured purple</Value> <Value>dirty purple</Value> <Value>dingy purple</Value> <Value>faded purple</Value> <Value>stained purple</Value> <Value>dusty faded purple</Value> </Values>",
			TargetDefinitionId = colourDef.Id,
			Description = "All of the colours from the RPI Engine's $drabcolor variable"
		});

		context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Most_Colours",
			Type = "Standard",
			Definition =
				"<Values> <Value>indian red</Value> <Value>light pink</Value> <Value>pink</Value> <Value>pale violet</Value> <Value>violet red</Value> <Value>lavender pink</Value> <Value>hot pink</Value> <Value>deep pink</Value> <Value>maroon red</Value> <Value>orchid pink</Value> <Value>thistle grey</Value> <Value>plum purple</Value> <Value>fuchsia pink</Value> <Value>magenta red</Value> <Value>purple</Value> <Value>slate blue</Value> <Value>blue</Value> <Value>navy blue</Value> <Value>midnight blue</Value> <Value>cobalt blue</Value> <Value>royal blue</Value> <Value>cornflower blue</Value> <Value>light steel blue</Value> <Value>steel blue</Value> <Value>slate gray</Value> <Value>gray</Value> <Value>sky blue</Value> <Value>turquoise blue</Value> <Value>azure blue</Value> <Value>cyan blue</Value> <Value>sea green</Value> <Value>green</Value> <Value>aquamarine</Value> <Value>spring green</Value> <Value>spring green</Value> <Value>emerald green</Value> <Value>cobalt green</Value> <Value>mint green</Value> <Value>pale green</Value> <Value>forest green</Value> <Value>pale blue</Value> <Value>lime green</Value> <Value>chartreuse green</Value> <Value>olive green</Value> <Value>ivory white</Value> <Value>white</Value> <Value>yellow</Value> <Value>light yellow</Value> <Value>goldenrod yellow</Value> <Value>khaki</Value> <Value>dark khaki</Value> <Value>gold</Value> <Value>banana yellow</Value> <Value>cornsilk yellow</Value> <Value>orange</Value> <Value>orange red</Value> <Value>off-white</Value> <Value>wheat yellow</Value> <Value>moccasin brown</Value> <Value>eggshell white</Value> <Value>tan yellow</Value> <Value>tan brown</Value> <Value>brick brown</Value> <Value>brick red</Value> <Value>carrot orange</Value> <Value>dark orange</Value> <Value>peachpuff pink</Value> <Value>seashell gray</Value> <Value>sandy brown</Value> <Value>sienna brown</Value> <Value>chocolate brown</Value> <Value>saddle brown</Value> <Value>burnt sienna</Value> <Value>light salmon pink</Value> <Value>salmon pink</Value> <Value>coral orange</Value> <Value>sepia brown</Value> <Value>orange brown</Value> <Value>rose red</Value> <Value>snow white</Value> <Value>light brown</Value> <Value>dark brown</Value> <Value>brown</Value> <Value>fire brick brown</Value> <Value>beet red</Value> <Value>teal blue</Value> <Value>smoky white</Value> <Value>dark gray</Value> <Value>black</Value> <Value>pitch black</Value> <Value>gray black</Value> </Values>",
			TargetDefinitionId = colourDef.Id,
			Description = "Mostly all colours without the $drabcolor inclusions"
		});

		context.SaveChanges();
	}

	private void SeedMaterials(FuturemudDatabaseContext context)
	{
		#region Tags

		var tags = new Dictionary<string, Tag>(StringComparer.InvariantCultureIgnoreCase);

		void AddTag(string name, string? parent)
		{
			var tag = new Tag
			{
				Name = name
			};
			if (parent != null) tag.Parent = tags[parent];
			context.Tags.Add(tag);
			tags[name] = tag;
			context.SaveChanges();
		}

		AddTag("Materials", null);
		AddTag("Simplified", "Materials");
		AddTag("Animal Product", "Materials");
		AddTag("Natural Materials", "Materials");
		AddTag("Manufactured Materials", "Materials");
		AddTag("Stone", "Natural Materials");
		AddTag("Vegetation", "Natural Materials");
		AddTag("Economically Useful Stone", "Stone");
		AddTag("Feldspar", "Economically Useful Stone");
		AddTag("Calcite", "Economically Useful Stone");
		AddTag("Gypsum", "Economically Useful Stone");
		AddTag("Soda Ash", "Economically Useful Stone");
		AddTag("Zeolite", "Economically Useful Stone");
		AddTag("Gemstone", "Economically Useful Stone");
		AddTag("Metal Ore", "Stone");
		AddTag("Aluminium Ore", "Metal Ore");
		AddTag("Antimony Ore", "Metal Ore");
		AddTag("Arsenic Ore", "Metal Ore");
		AddTag("Barium Ore", "Metal Ore");
		AddTag("Beryllium Ore", "Metal Ore");
		AddTag("Bismuth Ore", "Metal Ore");
		AddTag("Boron Ore", "Metal Ore");
		AddTag("Cesium Ore", "Metal Ore");
		AddTag("Chromium Ore", "Metal Ore");
		AddTag("Cobalt Ore", "Metal Ore");
		AddTag("Copper Ore", "Metal Ore");
		AddTag("Copper Oxide Ore", "Copper Ore");
		AddTag("Copper Sulphide Ore", "Copper Ore");
		AddTag("Gold Ore", "Metal Ore");
		AddTag("Hafnium Ore", "Metal Ore");
		AddTag("Iron Ore", "Metal Ore");
		AddTag("Lead Ore", "Metal Ore");
		AddTag("Lithium Ore", "Metal Ore");
		AddTag("Magnesium Ore", "Metal Ore");
		AddTag("Manganese Ore", "Metal Ore");
		AddTag("Mercury Ore", "Metal Ore");
		AddTag("Molybdenum Ore", "Metal Ore");
		AddTag("Nickel Ore", "Metal Ore");
		AddTag("Niobium Ore", "Metal Ore");
		AddTag("Palladium Ore", "Metal Ore");
		AddTag("Platinum Ore", "Metal Ore");
		AddTag("Potassium Ore", "Metal Ore");
		AddTag("Rare Earth Ore", "Metal Ore");
		AddTag("Rhodium Ore", "Metal Ore");
		AddTag("Rubidium Ore", "Metal Ore");
		AddTag("Silver Ore", "Metal Ore");
		AddTag("Sodium Ore", "Metal Ore");
		AddTag("Strontium Ore", "Metal Ore");
		AddTag("Tantalum Ore", "Metal Ore");
		AddTag("Tin Ore", "Metal Ore");
		AddTag("Titanium Ore", "Metal Ore");
		AddTag("Thorium Ore", "Metal Ore");
		AddTag("Tungsten Ore", "Metal Ore");
		AddTag("Vanadium Ore", "Metal Ore");
		AddTag("Zinc Ore", "Metal Ore");
		AddTag("Native Copper Ore", "Copper Ore");
		AddTag("Native Nickel Ore", "Nickel Ore");
		AddTag("Native Gold Ore", "Gold Ore");
		AddTag("Native Silver Ore", "Silver Ore");
		AddTag("Native Platinum Ore", "Platinum Ore");
		AddTag("Native Tin Ore", "Tin Ore");
		AddTag("Economic Stone", "Stone");
		AddTag("Elemental Metal", "Natural Materials");
		AddTag("Manufactured Metal", "Manufactured Materials");
		AddTag("Glass", "Manufactured Materials");
		AddTag("Bronze Age", "Manufactured Metal");
		AddTag("Iron Age", "Manufactured Metal");
		AddTag("Medieval Age", "Manufactured Metal");
		AddTag("Renaissance Age", "Manufactured Metal");
		AddTag("Industrial Age", "Manufactured Metal");
		AddTag("Modern Age", "Manufactured Metal");
		AddTag("Soil", "Natural Materials");
		AddTag("Wood", "Natural Materials");
		AddTag("Hardwood", "Wood");
		AddTag("Softwood", "Wood");
		AddTag("Manufactured Wood", "Wood");
		AddTag("Food", "Natural Materials");
		AddTag("Meat", "Food");
		AddTag("Vegetable", "Food");
		AddTag("Fruit", "Food");
		AddTag("Baked Good", "Food");
		AddTag("Herb", "Food");
		AddTag("Spice", "Food");
		AddTag("Hair", "Natural Materials");
		AddTag("Agricultural Crop", "Natural Materials");
		AddTag("Food Crop", "Agricultural Crop");
		AddTag("Fiber Crop", "Agricultural Crop");
		AddTag("Oil Crop", "Agricultural Crop");
		AddTag("Animal Skin", "Natural Materials");
		AddTag("Leather", "Natural Materials");
		AddTag("Fabric", "Manufactured Materials");
		AddTag("Natural Fiber Fabric", "Fabric");
		AddTag("Animal Fiber Fabric", "Fabric");
		AddTag("Synthetic Fiber Fabric", "Fabric");
		AddTag("Blended Fiber Fabric", "Fabric");
		AddTag("Ceramic", "Manufactured Materials");
		AddTag("Plastic", "Manufactured Materials");
		AddTag("Elastomer", "Materials");
		AddTag("Elemental Materials", "Materials");
		AddTag("Paper Product", "Manufactured Materials");
		AddTag("Writing Product", "Materials");

		#endregion

		var materials = new Dictionary<string, Material>(StringComparer.InvariantCultureIgnoreCase);
		var solvents = new Dictionary<Material, string>();

		void AddMaterial(string name, MaterialBehaviourType type, double relativeDensity, bool organic,
			double shearStrength, double impactStrength, double absorbency, double thermalConductivity,
			double electricalConductivity, double specificHeatCapacity, ResidueInformation? residue = null,
			params string[] materialTags)
		{
			var material = new Material
			{
				Name = name,
				MaterialDescription = name,
				BehaviourType = (int)type,
				Type = 0,
				Density = 1000 * relativeDensity,
				Organic = organic,
				Absorbency = absorbency,
				ShearYield = shearStrength,
				ImpactYield = impactStrength > 0.0 ? impactStrength : shearStrength * 2.2,
				ElectricalConductivity = electricalConductivity,
				ThermalConductivity = thermalConductivity,
				SpecificHeatCapacity = specificHeatCapacity
			};
			if (residue != null)
			{
				material.ResidueSdesc = residue.ResidueSdesc;
				material.ResidueDesc = residue.ResidueDesc;
				material.ResidueColour = residue.ResidueColour;
				material.SolventVolumeRatio = residue.SolventRatio;
				if (residue.Solvent != null) solvents[material] = residue.Solvent;
			}
			else
			{
				material.ResidueColour = "white";
				material.SolventVolumeRatio = 1.0;
			}

			materials[name] = material;
			context.Materials.Add(material);
			foreach (var tag in materialTags)
				material.MaterialsTags.Add(new MaterialsTags { Material = material, Tag = tags[tag] });
			context.SaveChanges();
		}

		#region Simplified
		AddMaterial("textile", MaterialBehaviourType.Fabric, 1.0, true, 10000, 10000, 0.3, 10.0, 0.0001, 500, null, "Simplified", "Fabric");
		AddMaterial("wood", MaterialBehaviourType.Wood, 0.5, true, 10000, 10000, 0.01, 0.15, 0.0001, 500, null, "Simplified", "Wood");
		AddMaterial("metal", MaterialBehaviourType.Metal, 7.0, false, 40000, 10000, 0.0, 18.0, 14500000, 500, null, "Simplified", "Manufactured Metal");
		AddMaterial("stone", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, null, "Simplified", "Stone");
		AddMaterial("glass", MaterialBehaviourType.Ceramic, 1.0, false, 10000, 10000, 0.0, 10.0, 14500000, 500, null, "Simplified", "Glass");
		AddMaterial("vegetation", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.01, 10.0, 0.0001, 500, null, "Simplified", "Vegetation");
		AddMaterial("ceramic", MaterialBehaviourType.Ceramic, 1.0, false, 10000, 10000, 0.0, 10.0, 14500000, 500, null, "Simplified", "Ceramic");
		AddMaterial("meat", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.1, 0.14, 0.0001, 500, null, "Simplified", "Meat");
		AddMaterial("other", MaterialBehaviourType.Mana, 1.0, false, 10000, 10000, 0.3, 0.14, 0.0001, 500, null, "Simplified");
		#endregion

		#region Metals

		AddMaterial("aluminium", MaterialBehaviourType.Metal, 2.7, false, 34500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("antimony", MaterialBehaviourType.Metal, 6.68, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("arsenic", MaterialBehaviourType.Metal, 5.7, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("arsenical bronze", MaterialBehaviourType.Metal, 7.85, false, 250000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("bell bronze", MaterialBehaviourType.Metal, 8.5, false, 250000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("beryllium", MaterialBehaviourType.Metal, 1.85, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("bismuth bronze", MaterialBehaviourType.Metal, 7.85, false, 314000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("bismuth", MaterialBehaviourType.Metal, 9.79, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("boron", MaterialBehaviourType.Metal, 2.3, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("brass", MaterialBehaviourType.Metal, 8.4, false, 207000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("bromine", MaterialBehaviourType.Metal, 6.6, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("bronze", MaterialBehaviourType.Metal, 8.7, false, 314000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("cadmium", MaterialBehaviourType.Metal, 8.69, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("carbon steel", MaterialBehaviourType.Metal, 7.85, false, 240000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("cast iron", MaterialBehaviourType.Metal, 7.1, false, 40000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Iron Age");
		AddMaterial("cesium", MaterialBehaviourType.Metal, 1.93, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("chromium", MaterialBehaviourType.Metal, 7.15, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("cobalt", MaterialBehaviourType.Metal, 8.86, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Renaissance Age");
		AddMaterial("copper", MaterialBehaviourType.Metal, 8.96, false, 68000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("crucible steel", MaterialBehaviourType.Metal, 7.85, false, 240000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Renaissance Age");
		AddMaterial("electrum", MaterialBehaviourType.Metal, 8.8, false, 85000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("gallium", MaterialBehaviourType.Metal, 5.91, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("galvanized steel", MaterialBehaviourType.Metal, 7.95, false, 320000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("germanium", MaterialBehaviourType.Metal, 5.5, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("gold", MaterialBehaviourType.Metal, 19.3, false, 120000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("hafnium", MaterialBehaviourType.Metal, 13.3, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("high tensile steel", MaterialBehaviourType.Metal, 7.85, false, 240000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("indium", MaterialBehaviourType.Metal, 7.31, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("lead", MaterialBehaviourType.Metal, 11.3, false, 131000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("magnesium", MaterialBehaviourType.Metal, 1.74, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("manganese steel", MaterialBehaviourType.Metal, 8.0, false, 605000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("manganese", MaterialBehaviourType.Metal, 7.3, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("mild bronze", MaterialBehaviourType.Metal, 8.7, false, 275000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("mild steel", MaterialBehaviourType.Metal, 7.85, false, 430000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("molybdenum", MaterialBehaviourType.Metal, 10.2, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("neodymium", MaterialBehaviourType.Metal, 7.01, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("nickel brass", MaterialBehaviourType.Metal, 8.4, false, 207000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("nickel", MaterialBehaviourType.Metal, 8.9, false, 58600, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("niobium", MaterialBehaviourType.Metal, 8.57, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("open hearth steel", MaterialBehaviourType.Metal, 7.85, false, 300000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("orichalcum", MaterialBehaviourType.Metal, 8.4, false, 207000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("osmium", MaterialBehaviourType.Metal, 22.59, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("palladium", MaterialBehaviourType.Metal, 12.0, false, 180000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("pewter", MaterialBehaviourType.Metal, 7.25, false, 30000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("phosphorus", MaterialBehaviourType.Metal, 1.82, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("pig iron", MaterialBehaviourType.Metal, 7.1, false, 75000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("platinum", MaterialBehaviourType.Metal, 21.5, false, 165000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Iron Age");
		AddMaterial("potassium", MaterialBehaviourType.Metal, 0.89, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("powder-coated steel", MaterialBehaviourType.Metal, 8.0, false, 320000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("rare earth metal", MaterialBehaviourType.Metal, 2.99, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("rhenium", MaterialBehaviourType.Metal, 20.8, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("rhodium", MaterialBehaviourType.Metal, 12.4, false, 700000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("rubidium", MaterialBehaviourType.Metal, 1.53, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("ruthenium", MaterialBehaviourType.Metal, 12.2, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("selenium", MaterialBehaviourType.Metal, 4.81, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("silicon", MaterialBehaviourType.Metal, 2.33, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("silver", MaterialBehaviourType.Metal, 10.5, false, 55700, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("sodium", MaterialBehaviourType.Metal, 0.97, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("spelter", MaterialBehaviourType.Metal, 7.85, false, 25000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("sponge iron", MaterialBehaviourType.Metal, 7.1, false, 75000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Iron Age");
		AddMaterial("stainless steel", MaterialBehaviourType.Metal, 7.9, false, 516000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("sterling silver", MaterialBehaviourType.Metal, 7.85, false, 55700, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Renaissance Age");
		AddMaterial("strontium", MaterialBehaviourType.Metal, 2.64, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("sulfur", MaterialBehaviourType.Metal, 2, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("tantalum", MaterialBehaviourType.Metal, 16.4, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("tellurium", MaterialBehaviourType.Metal, 6.24, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("thallium", MaterialBehaviourType.Metal, 11.8, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("thorium", MaterialBehaviourType.Metal, 11.7, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("tin", MaterialBehaviourType.Metal, 7.26, false, 11800, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("titanium", MaterialBehaviourType.Metal, 4.51, false, 275000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("tungsten", MaterialBehaviourType.Metal, 19.3, false, 750000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");
		AddMaterial("uranium", MaterialBehaviourType.Metal, 19.1, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("vanadium", MaterialBehaviourType.Metal, 6.0, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Modern Age");
		AddMaterial("wootz steel", MaterialBehaviourType.Metal, 7.85, false, 240000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Medieval Age");
		AddMaterial("wrought iron", MaterialBehaviourType.Metal, 7.74, false, 107000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Iron Age");
		AddMaterial("zinc", MaterialBehaviourType.Metal, 7.14, false, 124000, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Bronze Age");
		AddMaterial("zirconium", MaterialBehaviourType.Metal, 6.52, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			materialTags: "Industrial Age");

		#endregion

		#region Ore

		AddMaterial("acanthite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Silver Ore");
		AddMaterial("anglesite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Lead Ore");
		AddMaterial("argentite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Silver Ore");
		AddMaterial("arsenopyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Arsenic Ore");
		AddMaterial("azurite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Oxide Ore");
		AddMaterial("barite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Barium Ore");
		AddMaterial("bastnasite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Rare Earth Ore");
		AddMaterial("bauxite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Aluminium Ore");
		AddMaterial("bertrandite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Beryllium Ore");
		AddMaterial("bismuthinite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Bismuth Ore");
		AddMaterial("borax", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Boron Ore");
		AddMaterial("bornite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Sulphide Ore");
		AddMaterial("braunite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Manganese Ore");
		AddMaterial("brochantite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Oxide Ore");
		AddMaterial("brucite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Magnesium Ore");
		AddMaterial("calaverite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Gold Ore");
		AddMaterial("carnallite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Magnesium Ore");
		AddMaterial("cassiterite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Tin Ore");
		AddMaterial("celestite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Strontium Ore");
		AddMaterial("cerargyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Silver Ore");
		AddMaterial("cerussite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Lead Ore");
		AddMaterial("chalcopyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Sulphide Ore");
		AddMaterial("chalcocite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Sulphide Ore");
		AddMaterial("chromite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Chromium Ore");
		AddMaterial("chrysocolla", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Oxide Ore");
		AddMaterial("cinnabar", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Mercury Ore");
		AddMaterial("cobaltite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Cobalt Ore");
		AddMaterial("colemanite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Boron Ore");
		AddMaterial("columbite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Niobium Ore");
		AddMaterial("cuprite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Oxide Ore");
		AddMaterial("djurleite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Sulphide Ore");
		AddMaterial("dolomite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Magnesium Ore");
		AddMaterial("galena", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Lead Ore");
		AddMaterial("halite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Sodium Ore");
		AddMaterial("hausmannite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Manganese Ore");
		AddMaterial("hematite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Iron Ore");
		AddMaterial("ilmenite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Titanium Ore");
		AddMaterial("kernite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Boron Ore");
		AddMaterial("lepidolite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Rubidium Ore");
		AddMaterial("leucoxene", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Titanium Ore");
		AddMaterial("loparite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Rare Earth Ore");
		AddMaterial("magnesite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Magnesium Ore");
		AddMaterial("magnetite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Iron Ore");
		AddMaterial("malachite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Oxide Ore");
		AddMaterial("molybdenite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Molybdenum Ore");
		AddMaterial("monazite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Rare Earth Ore");
		AddMaterial("native copper", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Native Copper Ore");
		AddMaterial("native gold", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Native Nickel Ore");
		AddMaterial("native nickel", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Native Gold Ore");
		AddMaterial("native platinum", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Native Platinum Ore");
		AddMaterial("native silver", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Native Silver Ore");
		AddMaterial("native tin", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Native Tin Ore");
		AddMaterial("olivine", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Magnesium Ore");
		AddMaterial("pegmatite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Lithium Ore");
		AddMaterial("pentlandite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Nickel Ore");
		AddMaterial("petzite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Gold Ore");
		AddMaterial("pollucite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Cesium Ore");
		AddMaterial("proustite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Silver Ore");
		AddMaterial("pyrargyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Silver Ore");
		AddMaterial("pyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Iron Ore");
		AddMaterial("pyrolusite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Manganese Ore");
		AddMaterial("rhodochrosite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Manganese Ore");
		AddMaterial("rutile", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Titanium Ore");
		AddMaterial("scheelite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Tungsten Ore");
		AddMaterial("siderite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Iron Ore");
		AddMaterial("sperrylite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Platinum Ore");
		AddMaterial("sphalerite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Zinc Ore");
		AddMaterial("stibnite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Antimony Ore");
		AddMaterial("strontianite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Strontium Ore");
		AddMaterial("sylvanite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Gold Ore");
		AddMaterial("sylvite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Potassium Ore");
		AddMaterial("tantalite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Tantalum Ore");
		AddMaterial("tenorite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Copper Oxide Ore");
		AddMaterial("tetrahedrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Copper Sulphide Ore", "Silver Ore", "Antimony Ore");
		AddMaterial("ulexite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Boron Ore");
		AddMaterial("witherite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Barium Ore");
		AddMaterial("wolframite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Tungsten Ore");
		AddMaterial("zircon", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			materialTags: "Hafnium Ore");

		#endregion

		#region Stone

		var dustResidue = new ResidueInformation("(dusty)", "It is covered in a layer of dust");
		AddMaterial("limestone", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Calcite");
		AddMaterial("orthoclase", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Feldspar");
		AddMaterial("microcline", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Feldspar");
		AddMaterial("albite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Feldspar");
		AddMaterial("gypsum", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Gypsum");
		AddMaterial("alabaster", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Gypsum");
		AddMaterial("selenite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Gypsum");
		AddMaterial("satinspar", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Gypsum");
		AddMaterial("perlite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("soda ash", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Soda Ash");
		AddMaterial("chabazite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Zeolite");
		AddMaterial("clinoptilolite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Zeolite");
		AddMaterial("mordenite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Zeolite");
		AddMaterial("wollastonite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("vermiculite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("talc", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, dustResidue,
			"Economically Useful Stone");
		AddMaterial("pyrophyllite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("graphite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("kyanite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("andalusite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("muscovite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("phlogopite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("brimstone", MaterialBehaviourType.Stone, 2.07, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("kaolinite", MaterialBehaviourType.Stone, 2.07, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");
		AddMaterial("saltpeter", MaterialBehaviourType.Stone, 2.07, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Economically Useful Stone");

		AddMaterial("sandstone", MaterialBehaviourType.Stone, 2.4, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("siltstone", MaterialBehaviourType.Stone, 2.5, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("mudstone", MaterialBehaviourType.Stone, 2.51, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("shale", MaterialBehaviourType.Stone, 2.25, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("claystone", MaterialBehaviourType.Stone, 2.7, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("conglomerate", MaterialBehaviourType.Stone, 2.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("chert", MaterialBehaviourType.Stone, 2.65, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("chalk", MaterialBehaviourType.Stone, 2.71, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("granite", MaterialBehaviourType.Stone, 2.6, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("diorite", MaterialBehaviourType.Stone, 2.87, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("gabbro", MaterialBehaviourType.Stone, 2.92, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("rhyolite", MaterialBehaviourType.Stone, 2.6, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("basalt", MaterialBehaviourType.Stone, 2.85, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("andesite", MaterialBehaviourType.Stone, 2.43, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("dacite", MaterialBehaviourType.Stone, 2.4, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("obsidian", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("jet", MaterialBehaviourType.Stone, 1.32, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, dustResidue,
			"Stone");
		AddMaterial("quartzite", MaterialBehaviourType.Stone, 2.6, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("slate", MaterialBehaviourType.Stone, 2.75, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("phyllite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("schist", MaterialBehaviourType.Stone, 2.9, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("gneiss", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");
		AddMaterial("marble", MaterialBehaviourType.Stone, 2.78, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			dustResidue, "Stone");

		#endregion

		#region Gem Stones

		AddMaterial("diamond", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("amethyst", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("sapphire", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("turquoise", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("opal", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("diamond", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("jade", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("emerald", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("amber", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("pearl", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("lapis lazuli", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("topaz", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("nephrite", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("moonstone", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("agate", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("quartz", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("rose quartz", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("tourmaline", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("onyx", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("peridot", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("ruby", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("citrine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("alexandrite", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("spinel", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("aquamarine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("carnelian", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("jasper", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("aventurine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("chalcedony", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
			null, "Gemstone");
		AddMaterial("beryl", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("sunstone", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");
		AddMaterial("ametrine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
			"Gemstone");

		#endregion

		#region Wood

		var sawdust = new ResidueInformation("(sawdust)", "It is covered in a layer of sawdust", "yellow");
		AddMaterial("alder", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("ash", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("aspen", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("bamboo", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("beech", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("boxwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("cedar", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Softwood");
		AddMaterial("cherry", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("chestnut", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("coach", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("cork", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("cottonwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("dogwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("ebony", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("elm", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("eucalyptus", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("fir", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Softwood");
		AddMaterial("hickory", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("mahogany", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("maple", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("oak", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("particle board", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420,
			sawdust, "Manufactured Wood");
		AddMaterial("medium density fiberboard", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14,
			0.0001, 420, sawdust, "Manufactured Wood");
		AddMaterial("plywood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Manufactured Wood");
		AddMaterial("pine", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Softwood");
		AddMaterial("sandalwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("spruce", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("teak", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("walnut", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("willow", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");
		AddMaterial("yew", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
			"Hardwood");

		#endregion

		#region Soil

		AddMaterial("clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
		AddMaterial("sodic clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
		AddMaterial("kaolinite clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
		AddMaterial("fire clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
		AddMaterial("pelagic clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
		AddMaterial("silty clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 6.0), "Soil");
		AddMaterial("sandy clay", MaterialBehaviourType.Soil, 1.330, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 5.0), "Soil");
		AddMaterial("clay loam", MaterialBehaviourType.Soil, 1.32, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 5.0), "Soil");
		AddMaterial("sandy clay loam", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
		AddMaterial("silty clay loam", MaterialBehaviourType.Soil, 1.29, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
		AddMaterial("loam", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
		AddMaterial("sandy loam", MaterialBehaviourType.Soil, 1.56, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
		AddMaterial("silt loam", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
		AddMaterial("loamy sand", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(sandy)", "It is covered in a layer of dry sand", "yellow", "water", 2.0), "Soil");
		AddMaterial("silt", MaterialBehaviourType.Soil, 1.45, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(silty)", "It is covered in a layer of dry silt", "yellow", "water", 2.0), "Soil");
		AddMaterial("sand", MaterialBehaviourType.Soil, 1.71, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			new ResidueInformation("(sandy)", "It is covered in a layer of dry sand", "yellow", "water", 2.0), "Soil");
		AddMaterial("peat", MaterialBehaviourType.Soil, 0.85, false, 100, 5000, 0.0, 0.14, 0.0001, 500, null, "Soil");

		#endregion

		#region Food

		AddMaterial("beef", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("pork", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("lamb", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("chicken", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("rabbit", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("venison", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("game bird", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("camel", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("dog", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("cat", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("insect", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("fish", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("crab", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("shark", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("whale", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("shellfish", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("shrimp", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("squid", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("mollusc", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("human meat", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("game mammal", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("snake", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("lizard", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("frog", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
		AddMaterial("turtle", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
			"Meat");
		AddMaterial("snail", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");

		AddMaterial("food", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food");
		AddMaterial("apple", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("banana", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("pear", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("peach", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("plum", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("grape", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("mango", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("pineapple", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Fruit");
		AddMaterial("melon", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("pomegranate", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Fruit");
		AddMaterial("berry", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("apricot", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Fruit");
		AddMaterial("olive", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("tree nut", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Fruit");
		AddMaterial("peanut", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("lemon", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("lime", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("orange", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
		AddMaterial("fruit", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");

		AddMaterial("potato", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("tomato", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("pepper", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("pumpkin", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("sweet potato", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("pea", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("green bean", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("bean", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("corn", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("onion", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("potato", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("carrot", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("lettuce", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("spinach", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("cabbage", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("cauliflower", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("broccoli", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("radish", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("aubergene", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("avocado", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("beet", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("cucumber", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("zucchini", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("yam", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("tuber", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("turnip", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");
		AddMaterial("greens", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");

		AddMaterial("greens", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Vegetable");

		AddMaterial("herb", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
		AddMaterial("parsley", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Herb");
		AddMaterial("sage", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
		AddMaterial("rosemary", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Herb");
		AddMaterial("thyme", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
		AddMaterial("oregano", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Herb");
		AddMaterial("basil", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
		AddMaterial("dill", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
		AddMaterial("mint", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
		AddMaterial("cilantro", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Herb");
		AddMaterial("fennel", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
		AddMaterial("peppermint", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Herb");
		AddMaterial("aloe vera", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Herb");

		AddMaterial("spice", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("black pepper", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("salt", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Spice");
		AddMaterial("chilli powder", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("coriander", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("cayenne pepper", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("paprika", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("cumin", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("cinnamon", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("nutmeg", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("cloves", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("turmeric", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("saffron", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");
		AddMaterial("ginger", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Spice");

		AddMaterial("pie", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Baked Good");
		AddMaterial("pastry", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Baked Good");
		AddMaterial("bread", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Baked Good");
		AddMaterial("sourdough bread", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Baked Good");
		AddMaterial("rye bread", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Baked Good");
		AddMaterial("dough", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Baked Good");

		AddMaterial("fruit jelly", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food");
		AddMaterial("milk cream", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food", "Animal Product");
		AddMaterial("cheese", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food",
			"Animal Product");
		AddMaterial("honey", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food",
			"Animal Product");
		AddMaterial("yoghurt", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food",
			"Animal Product");

		#endregion

		#region Crops

		AddMaterial("wheat", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("sorghum", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("chickpea", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("barley", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("rye", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("rice", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("bitter vetch", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("soybean", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Food Crop");
		AddMaterial("flax", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Fiber Crop");

		#endregion

		#region Animal Skins and Leather

		AddMaterial("animal skin", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("cow hide", MaterialBehaviourType.Skin, 1.4, true, 20000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("deer hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("bear hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("dog hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("cat hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("fox hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("pig hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("wolf hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("snake hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("alligator hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("crocodile hide", MaterialBehaviourType.Skin, 1.4, true, 20000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("lion hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("tiger hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("rabbit hide", MaterialBehaviourType.Skin, 1.4, true, 12000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Animal Skin");
		AddMaterial("small mammal hide", MaterialBehaviourType.Skin, 1.4, true, 12000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Animal Skin");

		AddMaterial("leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Leather", "Simplified");
		AddMaterial("cow leather", MaterialBehaviourType.Leather, 1.4, true, 32000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Leather");
		AddMaterial("deer leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("bear leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("dog leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Leather");
		AddMaterial("cat leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Leather");
		AddMaterial("fox leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Leather");
		AddMaterial("pig leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
			"Leather");
		AddMaterial("wolf leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("snake leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("alligator leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("crocodile leather", MaterialBehaviourType.Leather, 1.4, true, 32000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("lion leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("tiger leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("rabbit leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
			null, "Leather");
		AddMaterial("small mammal leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001,
			500, null, "Leather");

		#endregion

		#region Hair

		AddMaterial("fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null, "Hair");
		AddMaterial("hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null, "Hair");
		AddMaterial("human hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("dog hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("cat hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("bear fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("fox fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("lion fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("tiger fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("rabbit fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("ermine", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
			"Hair");
		AddMaterial("vair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null, "Hair");

		#endregion

		#region Cloth

		AddMaterial("broadcloth", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("burlap", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("canvas", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("cotton", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("denim", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("felt", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Fiber Fabric");
		AddMaterial("hemp", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("hessian", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("jute", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("linen", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("silk", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Natural Fiber Fabric");
		AddMaterial("tweed", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Fiber Fabric");
		AddMaterial("wool", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Fiber Fabric");
		AddMaterial("cashmere", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Fiber Fabric");
		AddMaterial("mohair", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Fiber Fabric");

		#endregion

		#region Ceramic

		AddMaterial("bone china", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			null, "Ceramic");
		AddMaterial("brick", MaterialBehaviourType.Ceramic, 1.9, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
			"Ceramic");
		AddMaterial("concrete", MaterialBehaviourType.Ceramic, 2.4, false, 40000, 250000, 0.0, 0.002, 0.0001, 500, null,
			"Ceramic");
		AddMaterial("earthenware", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			null, "Ceramic");
		AddMaterial("fiberglass", MaterialBehaviourType.Ceramic, 2.0, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			null, "Ceramic");
		AddMaterial("fired clay", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			null, "Ceramic");
		AddMaterial("silicate glass", MaterialBehaviourType.Ceramic, 2.1, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
			"Ceramic");
		AddMaterial("soda-lime glass", MaterialBehaviourType.Ceramic, 2.1, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
			"Ceramic");
		AddMaterial("borosilicate glass", MaterialBehaviourType.Ceramic, 2.1, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
			"Ceramic");
		AddMaterial("lead glass", MaterialBehaviourType.Ceramic, 2.1, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
			"Ceramic");
		AddMaterial("reinforced concrete", MaterialBehaviourType.Ceramic, 2.9, false, 80000, 350000, 0.0, 0.002, 0.0001,
			500, null, "Ceramic");
		AddMaterial("plaster", MaterialBehaviourType.Ceramic, 0.35, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
			"Ceramic");
		AddMaterial("porcelain", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			null, "Ceramic");
		AddMaterial("stoneware", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			null, "Ceramic");
		AddMaterial("terracotta", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			null, "Ceramic");

		#endregion

		#region Plastics

		AddMaterial("acrylic", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
			"Plastic");
		AddMaterial("acrylic fiber", MaterialBehaviourType.Fabric, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500,
			null, "Plastic", "Synthetic Fiber Fabric");
		AddMaterial("glass-reinforced plastic", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14,
			0.0001, 500, null, "Plastic");
		AddMaterial("low-density polyethylene", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14,
			0.0001, 500, null, "Plastic");
		AddMaterial("high-density polyethylene", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14,
			0.0001, 500, null, "Plastic");
		AddMaterial("melamine formaldehyde", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001,
			500, null, "Plastic");
		AddMaterial("microfiber", MaterialBehaviourType.Fabric, 1.35, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
			"Plastic", "Synthetic Fiber Fabric");
		AddMaterial("nylon", MaterialBehaviourType.Fabric, 1.3, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
			"Plastic", "Synthetic Fiber Fabric");
		AddMaterial("polyethylene terephthalate", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14,
			0.0001, 500, null, "Plastic");
		AddMaterial("polyester", MaterialBehaviourType.Fabric, 1.3, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
			"Plastic", "Synthetic Fiber Fabric");
		AddMaterial("poly-cotton blend", MaterialBehaviourType.Fabric, 1.3, false, 10000, 25000, 0, 0.14, 0.0001, 500,
			null, "Plastic", "Blended Fiber Fabric");
		AddMaterial("polypropylene", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500,
			null, "Plastic");
		AddMaterial("polystyrene", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500,
			null, "Plastic");
		AddMaterial("polyurethane", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500,
			null, "Plastic");
		AddMaterial("polyvinyl chloride", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001,
			500, null, "Plastic");
		AddMaterial("spandex", MaterialBehaviourType.Fabric, 1.15, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
			"Plastic", "Synthetic Fiber Fabric");
		AddMaterial("synthetic rubber", MaterialBehaviourType.Elastomer, 0.975, false, 10000, 25000, 0, 0.14, 0.0001,
			500, null, "Plastic");

		#endregion

		#region Miscellaneous

		AddMaterial("cardboard", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Paper Product");
		AddMaterial("paper", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Writing Product");
		AddMaterial("papyrus", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Writing Product");
		AddMaterial("parchment", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
			"Writing Product", "Animal Product");
		AddMaterial("shell", MaterialBehaviourType.Shell, 1.52, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Product");
		AddMaterial("chitin", MaterialBehaviourType.Shell, 1.52, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Product");
		AddMaterial("tooth", MaterialBehaviourType.Tooth, 1.52, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Product");
		AddMaterial("tusk", MaterialBehaviourType.Tooth, 1.52, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Product");
		AddMaterial("ivory", MaterialBehaviourType.Tooth, 1.52, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
			"Animal Product");
		AddMaterial("grass", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("leaf", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("moss", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("seaweed", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("slime", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("vine", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("compost", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("mulch", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("vine", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("feces", MaterialBehaviourType.Feces, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials", "Animal Product");
		AddMaterial("feather", MaterialBehaviourType.Feather, 1.283, true, 1000, 1000, 0.05, 0.14, 0.0001, 500, null,
			"Natural Materials", "Animal Product");
		AddMaterial("soap", MaterialBehaviourType.Soap, 0.2, true, 1000, 1000, 0.05, 0.14, 0.0001, 500, null,
			"Natural Materials", "Animal Product");
		AddMaterial("beeswax", MaterialBehaviourType.Wax, 0.2, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials", "Animal Product");
		AddMaterial("paraffin wax", MaterialBehaviourType.Wax, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("cream", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Manufactured Materials");
		AddMaterial("gel", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Manufactured Materials");
		AddMaterial("jelly", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Manufactured Materials");
		AddMaterial("grease", MaterialBehaviourType.Grease, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Manufactured Materials");
		AddMaterial("lard", MaterialBehaviourType.Grease, 0.2, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Manufactured Materials");
		AddMaterial("paste", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
			"Manufactured Materials");

		AddMaterial("flame", MaterialBehaviourType.Mana, 1.0, false, 1, 1, 0.0, 0, 0, 0, null, "Elemental Materials");
		AddMaterial("mana", MaterialBehaviourType.Mana, 1.0, false, 1, 1, 0.0, 0, 0, 0, null, "Elemental Materials");
		AddMaterial("spirit energy", MaterialBehaviourType.Spirit, 1.0, false, 1, 1, 0.0, 0, 0, 0, null,
			"Elemental Materials");
		AddMaterial("natural rubber", MaterialBehaviourType.Elastomer, 0.975, true, 20000, 35000, 0, 0.14, 0.0001, 500,
			null, "Elastomer");
		AddMaterial("vulcanized rubber", MaterialBehaviourType.Elastomer, 0.975, true, 20000, 35000, 0, 0.14, 0.0001,
			500, null, "Elastomer");
		AddMaterial("calcium hydroxide", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500,
			null, "Natural Materials");
		AddMaterial("calcium oxide", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("lye", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("portland cement", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500,
			null, "Natural Materials");
		AddMaterial("pozzolanic ash", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500,
			null, "Natural Materials");
		AddMaterial("roman cement", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("slaked lime", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
			"Natural Materials");
		AddMaterial("wood ash", MaterialBehaviourType.Powder, 0.975, true, 1000, 1000, 0, 0.14, 0.0001, 500, null,
			"Natural Materials");

		#endregion

		context.SaveChanges();

		var liquids = new Dictionary<string, Liquid>(StringComparer.InvariantCultureIgnoreCase);
		var liquidCountsAs = new Dictionary<Liquid, string>();

		void AddLiquid(string name, string description, string longDescription, string taste, string? vagueTaste,
			string smell, string? vagueSmell, double tasteIntensity, double smellIntensity, double alcohol,
			double food, double calories, double water, double satiated, double viscosity, double density,
			bool organic, string displayColour, string dampDesc, string wetDesc, string drenchedDesc,
			string dampSdesc, string wetSdesc, string drenchedSdesc, double solventVolumeRatio, string? dried,
			double residueVolumePercentage, LiquidInjectionConsequence injectionConsequence,
			(string Liquid, ItemQuality Quality)? countsAs, double thermalConductivity = 0.609,
			double electricalConductivity = 0.005, double specificHeatCapacity = 4181, string? solvent = null)
		{
			var liquid = new Liquid
			{
				Name = name,
				AlcoholLitresPerLitre = alcohol,
				BoilingPoint = 100,
				CaloriesPerLitre = calories,
				DampDescription = dampDesc,
				DampShortDescription = dampSdesc,
				Density = density,
				TasteIntensity = tasteIntensity,
				SmellIntensity = smellIntensity,
				TasteText = taste,
				VagueTasteText = vagueTaste ?? taste,
				SmellText = smell,
				VagueSmellText = vagueSmell ?? smell,
				Viscosity = viscosity,
				DrinkSatiatedHoursPerLitre = satiated,
				FoodSatiatedHoursPerLitre = food,
				SolventVolumeRatio = solventVolumeRatio,
				DrenchedDescription = drenchedDesc,
				DrenchedShortDescription = drenchedSdesc,
				WetDescription = wetDesc,
				WetShortDescription = wetSdesc,
				InjectionConsequence = (int)injectionConsequence,
				WaterLitresPerLitre = water,
				Description = description,
				LongDescription = longDescription,
				ThermalConductivity = thermalConductivity,
				SpecificHeatCapacity = specificHeatCapacity,
				ElectricalConductivity = electricalConductivity,
				Organic = organic,
				DisplayColour = displayColour,
				ResidueVolumePercentage = residueVolumePercentage
			};
			context.Liquids.Add(liquid);
			liquids[name] = liquid;
			if (dried != null) liquid.DriedResidueId = materials[dried].Id;

			if (countsAs.HasValue && !string.IsNullOrEmpty(countsAs.Value.Liquid))
			{
				liquid.CountAsQuality = (int)countsAs.Value.Quality;
				liquidCountsAs[liquid] = countsAs.Value.Liquid;
			}                         
			else
			{
				liquid.CountAsQuality = 0;
			}

			if (solvent != null) liquid.Solvent = liquids[solvent];
		}

		#region Water

		AddLiquid("water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
			"It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
			false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
			0.05, LiquidInjectionConsequence.Harmful, null);
		AddLiquid("rain water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
			"It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
			false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
			0.05, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Excellent));
		AddLiquid("tap water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
			"It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
			false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
			0.05, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Excellent));
		AddLiquid("spring water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
			"It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
			false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
			0.05, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Excellent));
		AddLiquid("pool water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
			"It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
			false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
			0.05, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Excellent));
		AddLiquid("river water", "a clear liquid", "a clear, translucent liquid with some small impurities",
			"It has no real taste", "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0,
			0, 1.0, 12.0, 1.0, 1.0, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
			"(soaked)", 1.0, null, 0.05, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Good));
		AddLiquid("lake water", "a clear liquid", "a clear, translucent liquid with some small impurities",
			"It has no real taste", "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0,
			0, 1.0, 12.0, 1.0, 1.0, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
			"(soaked)", 1.0, null, 0.05, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Good));
		AddLiquid("swamp water", "a clear liquid", "a clear, translucent liquid with some small impurities",
			"It has no real taste", "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0,
			0, 1.0, 12.0, 1.0, 1.0, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
			"(soaked)", 1.0, null, 0.05, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Standard));
		AddLiquid("salt water", "a clear liquid", "a clear, translucent liquid", "It has a strong salty taste",
			"It has a strong salty taste", "It smells strongly of salt", "It smells of salt", 1000, 100, 0, 0, 0, -0.5,
			-6.0, 1.0, 1.029, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
			"(soaked)", 1.0, "salt", 0.029, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Poor));
		AddLiquid("brackish water", "a clear liquid", "a clear, translucent liquid", "It has a salty taste",
			"It has a salty taste", "It smells of salt", "It smells of salt", 500, 50, 0, 0, 0, -0.25, -3.0, 1.0, 1.015,
			false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, "salt",
			0.015, LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Standard));
		AddLiquid("saline solution", "a clear liquid", "a clear, translucent liquid",
			"It has a very, very mild salty taste", "It has no real taste", "It has no real smell",
			"It has no real smell", 100, 1, 0, 0, 0, 0.9, 9.0, 1.0, 1.009, false, "blue", "It is damp", "It is wet",
			"It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, "salt", 0.009,
			LiquidInjectionConsequence.Hydrating,
			("water", ItemQuality.Good));
		AddLiquid("dextrose solution", "a clear liquid", "a clear, translucent liquid",
			"It has a very, very mild sweet and salty taste", "It has no real taste", "It has no real smell",
			"It has no real smell", 100, 1, 0, 5.0, 200, 0.9, 9.0, 1.0, 1.009, false, "blue", "It is damp", "It is wet",
			"It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null, 0.009, LiquidInjectionConsequence.Hydrating,
			("water", ItemQuality.Good));
		AddLiquid("detergent", "a clear, soapy liquid", "a clear, soapy liquid",
			"It has a strong soapy taste", "It has a strong soapy taste", "It smells strongly of soap",
			"It smells of soap", 1000, 100, 0, 0, 0, -0.5, -6.0, 1.0, 1.029, false, "bold blue", "It is damp",
			"It is wet", "It is soaking wet", "(soap-damp)", "(soap-wet)", "(soap-soaked)", 1.0, null, 0.029,
			LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Legendary));
		AddLiquid("soapy water", "a clear liquid with soap suds", "a clear, translucent liquid with soap suds",
			"It has a strong soapy taste", "It has a strong soapy taste", "It smells strongly of soap",
			"It smells of soap", 1000, 100, 0, 0, 0, -0.5, -6.0, 1.0, 1.029, false, "bold blue", "It is damp",
			"It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null, 0.029,
			LiquidInjectionConsequence.Harmful,
			("water", ItemQuality.Legendary));

		#endregion

		context.SaveChanges();

		#region Biofluids

		var driedBlood = new Material
		{
			Name = "Dried Blood",
			MaterialDescription = "dried blood",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 4,
			ResidueDesc = "It is covered in {0}dried blood",
			ResidueColour = "red",
			Absorbency = 0
		};
		context.Materials.Add(driedBlood);
		var blood = new Liquid
		{
			Name = "Blood",
			Description = "blood",
			LongDescription = "a virtually opaque dark red fluid",
			TasteText = "It has a sharply metallic, umami taste",
			VagueTasteText = "It has a metallic taste",
			SmellText = "It has a metallic, coppery smell",
			VagueSmellText = "It has a faintly metallic smell",
			TasteIntensity = 200,
			SmellIntensity = 10,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.8,
			DrinkSatiatedHoursPerLitre = 6,
			FoodSatiatedHoursPerLitre = 4,
			CaloriesPerLitre = 800,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "bold red",
			DampDescription = "It is damp with blood",
			WetDescription = "It is wet with blood",
			DrenchedDescription = "It is drenched with blood",
			DampShortDescription = "(blood damp)",
			WetShortDescription = "(bloody)",
			DrenchedShortDescription = "(blood drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedBlood
		};
		context.Liquids.Add(blood);

		var driedSweat = new Material
		{
			Name = "Dried Sweat",
			MaterialDescription = "dried sweat",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 3,
			ResidueDesc = "It is covered in {0}dried sweat",
			ResidueColour = "yellow",
			Absorbency = 0
		};
		context.Materials.Add(driedSweat);
		var sweat = new Liquid
		{
			Name = "Sweat",
			Description = "sweat",
			LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor",
			TasteText = "It tastes like a pungent, salty lick of someone's underarms",
			VagueTasteText = "It tastes very unpleasant, like underarm stench",
			SmellText = "It has the sharp, pungent smell of body odor",
			VagueSmellText = "It has the sharp, pungent smell of body odor",
			TasteIntensity = 200,
			SmellIntensity = 200,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.95,
			DrinkSatiatedHoursPerLitre = 5,
			FoodSatiatedHoursPerLitre = 0,
			CaloriesPerLitre = 0,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "yellow",
			DampDescription = "It is damp with sweat",
			WetDescription = "It is wet and smelly with sweat",
			DrenchedDescription = "It is soaking wet and smelly with sweat",
			DampShortDescription = "(sweat-damp)",
			WetShortDescription = "(sweaty)",
			DrenchedShortDescription = "(sweat-drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedSweat
		};
		context.Liquids.Add(sweat);

		var driedVomit = new Material
		{
			Name = "Dried Vomit",
			MaterialDescription = "dried vomit",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 3,
			ResidueDesc = "It is covered in {0}dried vomit",
			ResidueColour = "yellow",
			Absorbency = 0
		};
		context.Materials.Add(driedVomit);
		var vomit = new Liquid
		{
			Name = "Vomit",
			Description = "vomit",
			LongDescription = "a stinking mixture of digestive liquids and partially digested food",
			TasteText = "It just tastes like vomit...I'm sure you don't need a description",
			VagueTasteText = "It just tastes like vomit...I'm sure you don't need a description",
			SmellText = "It smells awful, a naturally repugnant stench associated with sickness",
			VagueSmellText = "It smells awful, a naturally repugnant stench associated with sickness",
			TasteIntensity = 500,
			SmellIntensity = 500,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.6,
			DrinkSatiatedHoursPerLitre = 2,
			FoodSatiatedHoursPerLitre = 0,
			CaloriesPerLitre = 200,
			Viscosity = 5,
			Density = 1.3,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "yellow",
			DampDescription = "It is stained with vomit",
			WetDescription = "It is wet and covered with vomit",
			DrenchedDescription = "It is absolutely drenched with wet vomit",
			DampShortDescription = "(vomit-stained)",
			WetShortDescription = "(vomit-covered)",
			DrenchedShortDescription = "(vomit-drenched)",
			SolventId = 1,
			SolventVolumeRatio = 10,
			InjectionConsequence = (int)LiquidInjectionConsequence.Deadly,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedVomit
		};
		context.Liquids.Add(vomit);

		#endregion

		#region Drinks

		AddLiquid("lager", "an amber liquid", "a fairly translucent dark amber fluid",
			"It has a smooth, crisp and moderately bitter taste", "It has the bitter taste of beer",
			"It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.06, 3.7, 340, 0.92, 12.0, 1.0, 1.0,
			true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
			"(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("light lager", "an amber liquid", "a fairly translucent dark amber fluid",
			"It has a smooth and moderately bitter taste", "It has the bitter taste of beer",
			"It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.035, 2.4, 205, 0.95, 12.0, 1.0, 1.0,
			true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
			"(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("pale ale", "an amber liquid",
			"a moderately translucent dark amber fluid with a tendency to form frothy light amber foam",
			"It has a strong, rich bitter taste", "It has the bitter taste of beer", "It has a bitter, alcoholic smell",
			"It has a bitter smell", 150, 40, 0.08, 3.7, 340, 0.92, 12.0, 1.0, 1.0, true, "yellow",
			"It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)", "(wet)", "(soaked)",
			5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("amber ale", "an amber liquid",
			"a moderately translucent dark amber fluid with a tendency to form frothy light amber foam",
			"It has a smooth, crisp and moderately bitter taste", "It has the bitter taste of beer",
			"It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.08, 3.7, 340, 0.92, 12.0, 1.0, 1.0,
			true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
			"(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("dark ale", "an amber liquid", "a fairly translucent dark amber fluid",
			"It has a rich and very bitter taste", "It has the bitter taste of beer",
			"It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.08, 5.0, 500, 0.90, 10.0, 1.0, 1.0,
			true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
			"(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");

		AddLiquid("red wine", "an dark burgundy liquid", "a transparent dark burgundy fluid",
			"It has a dry and sharp taste, with a distinct after note of tannin", "It has the sharp taste of wine",
			"It has a sharp, alcoholic smell", "It has a sharp smell", 150, 40, 0.14, 2.0, 200, 0.8, 8.0, 1.0, 1.0,
			true, "magenta", "It is damp with wine", "It is wet with wine", "It is soaking wet with wine",
			"(wine-damp)", "(wine-wet)", "(wine-soaked)", 7.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("watered red wine", "a burgundy liquid", "a transparent burgundy fluid",
			"It has a faint sharp taste, with a slight after note of tannin", "It has the taste of watered-down wine",
			"It has a slightly alcoholic smell", "It has a sharp smell", 150, 40, 0.035, 1.0, 100, 0.97, 12.0, 1.0, 1.0,
			true, "magenta", "It is damp with wine", "It is wet with wine", "It is soaking wet with wine",
			"(wine-damp)", "(wine-wet)", "(wine-soaked)", 3.5, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("white wine", "a clear amber liquid", "a transparent amber fluid", "It has a dry and floral taste",
			"It has the dry, floral taste of white wine", "It has a floral, alcoholic smell",
			"It has a slightly alcoholic smell", 150, 40, 0.14, 2.0, 200, 0.8, 8.0, 1.0, 1.0, true, "yellow",
			"It is damp with wine", "It is wet with wine", "It is soaking wet with wine", "(wine-damp)", "(wine-wet)",
			"(wine-soaked)", 7.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");

		AddLiquid("vodka", "a clear liquid", "a crystal clear liquid smelling strongly of alcohol",
			"It has little taste beyond that of the very strong alcohol",
			"It has little taste beyond that of the very strong alcohol", "It smells strongly of alcohol",
			"It smells strongly of alcohol", 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("bourbon whiskey", "a translucent brown liquid",
			"a translucent, dark brown liquid smelling strongly of alcohol",
			"It tastes first and foremost of alcohol, but it is supplemented by an oakey, sweet note that contrasts with it",
			null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("scotch whiskey", "a translucent brown liquid",
			"a translucent, dark brown liquid smelling strongly of alcohol",
			"It tastes first and foremost of alcohol, but it is supplemented by an oakey, dry note that contrasts with it",
			null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("whiskey", "a translucent brown liquid",
			"a translucent, dark brown liquid smelling strongly of alcohol",
			"It tastes first and foremost of alcohol, but it is supplemented by an oakey, dry note that contrasts with it",
			null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("rum", "a translucent brown liquid", "a translucent, dark brown liquid smelling strongly of alcohol",
			"It tastes first and foremost of alcohol, but it is supplemented by a very sweet, sugary aftertaste", null,
			"It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("gin", "a clear liquid", "a clear liquid smelling strongly of alcohol",
			"It tastes first and foremost of alcohol, followed by the unmistakable and unique taste of juniper berry",
			null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");
		AddLiquid("tequila", "a transparent brown liquid", "a transparent brown liquid smelling strongly of alcohol",
			"It tastes first and foremost of alcohol, but it is supplemented by the distinctive taste of agave", null,
			"It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
			solvent: "water");

		AddLiquid("ethanol", "a clear liquid", "a crystal clear liquid smelling strongly of alcohol",
			"It has little taste beyond that of the very strong alcohol",
			"It has little taste beyond that of the very strong alcohol", "It smells strongly of alcohol",
			"It smells strongly of alcohol", 500, 500, 1.0, 5.4, 390, -0.1, -3.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Deadly, null,
			solvent: "water");
		AddLiquid("methanol", "a clear liquid", "a crystal clear liquid smelling strongly of alcohol",
			"It has little taste beyond that of the very strong alcohol",
			"It has little taste beyond that of the very strong alcohol", "It smells strongly of alcohol",
			"It smells strongly of alcohol", 500, 500, 1.0, 5.4, 390, -0.1, -3.0, 1.0, 1.0, true, "yellow",
			"It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
			"(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Deadly, null,
			solvent: "water");

		AddLiquid("orange juice", "orange juice", "a translucent orange liquid with fruit pulp",
			"It tastes like orange juice", "It tastes like orange juice", "It smells of oranges",
			"It smells of oranges", 200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "orange", "It is damp with juice",
			"It is wet with juice", "It is soaking wet with juice", "(damp)", "(wet)", "(juice-soaked)", 5.0, null,
			0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("apple juice", "apple juice", "a transparent brown liquid with fruit pulp",
			"It tastes like apple juice", "It tastes like apple juice", "It smells of apples", "It smells of apples",
			200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with juice", "It is wet with juice",
			"It is soaking wet with juice", "(damp)", "(wet)", "(juice-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("pineapple juice", "pineapple juice", "a translucent yellow liquid with fruit pulp",
			"It tastes like pineapple juice", "It tastes like pineapple juice", "It smells of pineapples",
			"It smells of pineapples", 200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "yellow",
			"It is damp with juice", "It is wet with juice", "It is soaking wet with juice", "(damp)", "(wet)",
			"(juice-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("pomegranate juice", "pomegranate juice", "a translucent purple liquid with fruit pulp",
			"It tastes like pomegranate juice", "It tastes like pomegranate juice", "It smells of pomegranates",
			"It smells of pomegranates", 200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "magenta",
			"It is damp with juice", "It is wet with juice", "It is soaking wet with juice", "(damp)", "(wet)",
			"(juice-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");

		AddLiquid("white wine vinegar", "a clear liquid", "a clear, translucent liquid", "It tastes like vinegar", null,
			"It smells of vinegar", null, 200, 100, 0, 5.4, 390, 0.5, 0.0, 1.0, 1.0, true, "magenta", "It is damp",
			"It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("balsamic vineger", "a dark brown liquid", "a translucent dark brown liquid",
			"It tastes like balsamic vinegar", null, "It smells of pomegranates", null, 200, 100, 0, 5.4, 390, 0.5, 0,
			1.0, 1.0, true, "magenta", "It is damp with vinegar", "It is wet with vinegar",
			"It is soaking wet with vinegar", "(damp)", "(wet)", "(vinegar-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");

		AddLiquid("olive oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like olive oil", null,
			"It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow", "It is damp with oil",
			"It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)", "(oil-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
		AddLiquid("vegetable oil", "a transparent yellow oil", "a transparent yellow oil",
			"It tastes like vegetable oil", null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0,
			true, "yellow", "It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)",
			"(oily)", "(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard),
			solvent: "soapy water");
		AddLiquid("canola oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like canola oil",
			null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow",
			"It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)",
			"(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
		AddLiquid("peanut oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like peanut oil",
			null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow",
			"It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)",
			"(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
		AddLiquid("sesame oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like sesame oil",
			null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow",
			"It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)",
			"(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
		AddLiquid(name: "whale oil", description: "a transparent dark yellow oil", longDescription: "a transparent dark yellow oil", taste: "It tastes like whale oil", vagueTaste: null,
			smell: "It smells of oil", vagueSmell: null, tasteIntensity: 200, smellIntensity: 100, alcohol: 0, food: 5.4, calories: 390, water: 0.5, satiated: 0, viscosity: 2.0, density: 1.0, organic: true, displayColour: "yellow", dampDesc: "It is damp with oil",
			wetDesc: "It is covered with oil", drenchedDesc: "It is soaking with oil", dampSdesc: "(oil-damp)", wetSdesc: "(oily)", drenchedSdesc: "(oil-soaked)", solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.05,
			injectionConsequence: LiquidInjectionConsequence.Harmful, countsAs: ("fuel", ItemQuality.Heroic), solvent: "soapy water");
		AddLiquid("sperm oil", "a transparent dark yellow oil", "a transparent dark yellow oil", "It tastes like sperm oil", null,
			"It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow", "It is damp with oil",
			"It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)", "(oil-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Heroic), solvent: "soapy water");
		AddLiquid("train oil", "a transparent dark yellow oil", "a transparent dark yellow oil", "It tastes like train oil", null,
			"It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow", "It is damp with oil",
			"It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)", "(oil-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Excellent), solvent: "soapy water");

		AddLiquid("milk", "a creamy white liquid", "a translucent white liquid",
			"It tastes creamy and sweet, with a full bodied flavour", null, "It smells like milk", null, 200, 100, 0,
			5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "bold white", "It is damp with milk", "It is wet with milk",
			"It is soaking wet with milk", "(damp)", "(wet)", "(milk-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("goat's milk", "a creamy white liquid", "a translucent white liquid",
			"It tastes creamy and sweet, with a full bodied flavour", null, "It smells like milk", null, 200, 100, 0,
			5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "bold white", "It is damp with milk", "It is wet with milk",
			"It is soaking wet with milk", "(damp)", "(wet)", "(milk-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("sheep's milk", "a creamy white liquid", "a translucent white liquid",
			"It tastes creamy and sweet, with a full bodied flavour", null, "It smells like milk", null, 200, 100, 0,
			5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "bold white", "It is damp with milk", "It is wet with milk",
			"It is soaking wet with milk", "(damp)", "(wet)", "(milk-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");

		AddLiquid("tea", "a clear brown liquid", "a transparent brown liquid",
			"It tastes bitter and aromatic, like black tea", null, "It smells like tea", null, 200, 100, 0, 1.0, 50,
			0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with tea", "It is wet with tea",
			"It is soaking wet with tea", "(damp)", "(wet)", "(tea-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("tea with milk", "a light brown liquid", "a translucent light brown liquid",
			"It tastes bitter and aromatic, like black tea mixed with milk", null, "It smells like tea", null, 100, 50,
			0, 1.0, 50, 0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with tea", "It is wet with tea",
			"It is soaking wet with tea", "(damp)", "(wet)", "(tea-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("green tea", "a clear brown liquid", "a translucent brown liquid",
			"It tastes bitter and aromatic, like green tea", null, "It smells like tea", null, 200, 100, 0, 1.0, 50,
			0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with tea", "It is wet with tea",
			"It is soaking wet with tea", "(damp)", "(wet)", "(tea-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("coffee", "a dark brown liquid", "a translucent dark brown liquid",
			"It tastes bitter and rich, like black coffee", null, "It smells like coffee", null, 200, 100, 0, 0, 0, 9.0,
			1.0, 1.0, 1.0, true, "yellow", "It is damp with coffee", "It is wet with coffee",
			"It is soaking wet with tea", "(damp)", "(wet)", "(coffee-soaked)", 5.0, null, 0.05,
			LiquidInjectionConsequence.Harmful, null, solvent: "water");
		AddLiquid("latte", "a light brown liquid", "a translucent light brown liquid",
			"It tastes slightly bitter and rich, like black coffee mixed with milk", null, "It smells like coffee",
			null, 100, 50, 0, 2, 200, 0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with coffee",
			"It is wet with coffee", "It is soaking wet with tea", "(damp)", "(wet)", "(coffee-soaked)", 5.0, null,
			0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water");

		#endregion

		#region Fuels
		AddLiquid(name: "fuel", description: "a clear liquid", longDescription: "a clear, translucent liquid",
			taste: "It has little taste beyond that of the very strong alcohol",
			vagueTaste: "It has little taste beyond that of the very strong alcohol",
			smell: "It smells strongly of pure alcohol",
			vagueSmell: "It smells strongly of alcohol", tasteIntensity: 1000, smellIntensity: 100, alcohol: 1.0,
			food: 5.4, calories: 390, water: -0.5, satiated: -6.0, viscosity: 1.0, density: 1.029, organic: true,
			displayColour: "yellow", dampDesc: "It is damp with alcohol",
			wetDesc: "It is soaking wet with alcohol", drenchedDesc: "It is drenched with alcohol", dampSdesc: "(damp)",
			wetSdesc: "(liquor-soaked)", drenchedSdesc: "(liquor-drenched)", solventVolumeRatio: 1.0, dried: null,
			residueVolumePercentage: 0.029,
			injectionConsequence: LiquidInjectionConsequence.Harmful, countsAs: null);
		AddLiquid(name: "ethanol", description: "a clear liquid", longDescription: "a clear, translucent liquid",
			taste: "It has little taste beyond that of the very strong alcohol",
			vagueTaste: "It has little taste beyond that of the very strong alcohol",
			smell: "It smells strongly of pure alcohol",
			vagueSmell: "It smells strongly of alcohol", tasteIntensity: 1000, smellIntensity: 100, alcohol: 1.0,
			food: 5.4, calories: 390, water: -0.5, satiated: -6.0, viscosity: 1.0, density: 1.029, organic: true,
			displayColour: "yellow", dampDesc: "It is damp with alcohol",
			wetDesc: "It is soaking wet with alcohol", drenchedDesc: "It is drenched with alcohol", dampSdesc: "(damp)",
			wetSdesc: "(liquor-soaked)", drenchedSdesc: "(liquor-drenched)", solventVolumeRatio: 1.0, dried: null,
			residueVolumePercentage: 0.029,
			injectionConsequence: LiquidInjectionConsequence.Harmful, countsAs: ("fuel", ItemQuality.VeryGood));
		AddLiquid(name: "methanol", description: "a clear liquid", longDescription: "a clear, translucent liquid",
			taste: "It has little taste beyond that of the very strong alcohol",
			vagueTaste: "It has little taste beyond that of the very strong alcohol",
			smell: "It smells strongly of pure alcohol",
			vagueSmell: "It smells strongly of alcohol", tasteIntensity: 1000, smellIntensity: 100, alcohol: 1.0,
			food: 5.4, calories: 390, water: -0.5, satiated: -6.0, viscosity: 1.0, density: 1.029, organic: true,
			displayColour: "yellow", dampDesc: "It is damp with alcohol",
			wetDesc: "It is soaking wet with alcohol", drenchedDesc: "It is drenched with alcohol", dampSdesc: "(damp)",
			wetSdesc: "(liquor-soaked)", drenchedSdesc: "(liquor-drenched)", solventVolumeRatio: 1.0, dried: null,
			residueVolumePercentage: 0.029,
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.VeryGood));
		AddLiquid(name: "kerosene", description: "a clear liquid",
			longDescription: "a transparent fluid",
			taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has the taste of kerosene fuel; YUCK!",
			smell: "It smells like kerosene",
			vagueSmell: "It smells like kerosene", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
			calories: 0.0, water: -0.5, satiated: -6.0, viscosity: 0.85, density: 0.9, organic: false,
			displayColour: "magenta", dampDesc: "It is damp with kerosene",
			wetDesc: "It is soaked with kerosene", drenchedDesc: "It is drenched with kerosene",
			dampSdesc: "(kerosene-damp)", wetSdesc: "(kerosene-soaked)", drenchedSdesc: "(kerosene-drenched)",
			solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.VeryGood));
		AddLiquid(name: "gasoline", description: "a clear liquid",
			longDescription: "a transparent, orangey-amber fluid",
			taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has the taste of gasoline fuel; YUCK!",
			smell: "It smells like gasoline",
			vagueSmell: "It smells like gasoline", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
			calories: 0.0, water: -0.5, satiated: -6.0, viscosity: 0.85, density: 0.9, organic: false,
			displayColour: "magenta", dampDesc: "It is damp with gasoline",
			wetDesc: "It is soaked with gasoline", drenchedDesc: "It is drenched with gasoline",
			dampSdesc: "(gasoline-damp)", wetSdesc: "(gasoline-soaked)", drenchedSdesc: "(gasoline-drenched)",
			solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Heroic));
		AddLiquid(name: "diesel", description: "a clear liquid",
			longDescription: "a transparent, yellowy-amber fluid",
			taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has the taste of diesel fuel; YUCK!",
			smell: "It smells like diesel",
			vagueSmell: "It smells like diesel", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
			calories: 0.0, water: -0.5, satiated: -6.0, viscosity: 0.85, density: 0.9, organic: false,
			displayColour: "magenta", dampDesc: "It is damp with diesel",
			wetDesc: "It is soaked with diesel", drenchedDesc: "It is drenched with diesel",
			dampSdesc: "(diesel-damp)", wetSdesc: "(diesel-soaked)", drenchedSdesc: "(diesel-drenched)",
			solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Heroic));
		AddLiquid(name: "crude oil", description: "a thick, black liquid",
			longDescription: "a thick yellow-black liquid",
			taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
			smell: "It smells like sulfur",
			vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
			calories: 0.0, water: -0.5, satiated: -6.0, viscosity: 10, density: 0.9, organic: false,
			displayColour: "bold magenta", dampDesc: "It is damp with crude oil",
			wetDesc: "It is soaked with crude oil", drenchedDesc: "It is drenched with crude oil",
			dampSdesc: "(petroleum-damp)", wetSdesc: "(petroleum-soaked)", drenchedSdesc: "(petroleum-drenched)",
			solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Standard));
		AddLiquid(name: "heavy crude oil", description: "a very thick, black liquid",
			longDescription: "a thick yellow-black liquid",
			taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
			smell: "It smells like sulfur",
			vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
			calories: 0.0, water: -0.5, satiated: -6.0, viscosity: 4, density: 0.8, organic: false,
			displayColour: "bold magenta", dampDesc: "It is damp with crude oil",
			wetDesc: "It is soaked with crude oil", drenchedDesc: "It is drenched with crude oil",
			dampSdesc: "(petroleum-damp)", wetSdesc: "(petroleum-soaked)", drenchedSdesc: "(petroleum-drenched)",
			solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Standard));
		AddLiquid(name: "light crude oil", description: "a sticky, black liquid",
			longDescription: "a sticky yellow-black liquid",
			taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
			smell: "It smells like sulfur",
			vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
			calories: 0.0, water: -0.5, satiated: -6.0, viscosity: 30, density: 0.9, organic: false,
			displayColour: "bold magenta", dampDesc: "It is damp with crude oil",
			wetDesc: "It is soaked with crude oil", drenchedDesc: "It is drenched with crude oil",
			dampSdesc: "(petroleum-damp)", wetSdesc: "(petroleum-soaked)", drenchedSdesc: "(petroleum-drenched)",
			solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Standard));
		AddLiquid(name: "heavy fuel oil", description: "a viscous, black liquid",
			longDescription: "a thick black liquid",
			taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
			smell: "It smells like sulfur",
			vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
			calories: 0.0, water: -0.5, satiated: -6.0, viscosity: 4, density: 0.8, organic: false,
			displayColour: "bold magenta", dampDesc: "It is damp with fuel oil",
			wetDesc: "It is soaked with fuel oil", drenchedDesc: "It is drenched with fuel oil",
			dampSdesc: "(oil-damp)", wetSdesc: "(oil-soaked)", drenchedSdesc: "(oil-drenched)",
			solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
			injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Excellent));
		#endregion

		context.SaveChanges();

		foreach (var solvent in solvents) solvent.Key.SolventId = liquids[solvent.Value].Id;

		foreach (var countAs in liquidCountsAs) countAs.Key.CountAsId = liquids[countAs.Value].Id;

		context.SaveChanges();
	}

	internal class ResidueInformation
	{
		public ResidueInformation(string sdesc, string desc, string colour = "white", string? solvent = null,
			double ratio = 1.0)
		{
			ResidueSdesc = sdesc;
			ResidueDesc = desc;
			ResidueColour = colour;
			Solvent = solvent;
			SolventRatio = ratio;
		}

		public string ResidueSdesc { get; set; }
		public string ResidueDesc { get; set; }
		public string ResidueColour { get; set; }
		public string? Solvent { get; set; }
		public double SolventRatio { get; set; }
	}
}