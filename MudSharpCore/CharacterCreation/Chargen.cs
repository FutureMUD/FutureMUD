using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Dapper;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Resources;
using MudSharp.CharacterCreation.Roles;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Email;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;
using Newtonsoft.Json.Linq;
using Account = MudSharp.Models.Account;
using Attribute = MudSharp.Body.Traits.Subtypes.Attribute;

namespace MudSharp.CharacterCreation;

public partial class Chargen : FrameworkItem, IChargen
{
	private readonly HashSet<ChargenStage> _completedStages = new();
	private readonly List<Tuple<string, string>> _priorRejections = new();

	public Chargen(MudSharp.Models.Chargen chargen, IFuturemud gameworld, Account account)
		: this((ChargenState)chargen.Status, null, gameworld, gameworld.TryAccount(account))
	{
		Id = chargen.Id;
		_approvedById = chargen.ApprovedById;
		LoadFromXml(XElement.Parse(chargen.Definition));
	}

	public Chargen(IChargenMenu menu, IFuturemud gameworld, IAccount account)
		: this(ChargenState.InProgress, menu, gameworld, account)
	{
		Stage = Gameworld.ChargenStoryboard.FirstStage;
		CurrentScreen = Gameworld.ChargenStoryboard.StageScreenMap[Stage].GetScreen(this);
	}

	public Chargen(MudSharp.Models.Chargen chargen, IChargenMenu menu, IFuturemud gameworld, IAccount account)
		: this(ChargenState.InProgress, menu, gameworld, account)
	{
		Id = chargen.Id;
		_approvedById = chargen.ApprovedById;
		LoadFromXml(XElement.Parse(chargen.Definition));
		if (Stage == ChargenStage.Submit || Stage == ChargenStage.ConfirmQuit)
		{
			_completedStages.Remove(Stage);
			Stage = ChargenStage.Menu;
		}

		CurrentScreen = Gameworld.ChargenStoryboard.StageScreenMap[Stage].GetScreen(this);
	}

	private Chargen(ChargenState status, IChargenMenu menu, IFuturemud gameworld, IAccount account)
	{
		Menu = menu;
		Gameworld = gameworld;
		Account = account;
		State = status;
		SelectedAttributes = new List<ITrait>();
		SelectedSkills = new List<ITraitDefinition>();
		SelectedCharacteristics = new List<Tuple<ICharacteristicDefinition, ICharacteristicValue>>();
		SelectedEntityDescriptionPatterns = new List<IEntityDescriptionPattern>();
		SelectedNotes = new List<Tuple<string, string>>();
		SelectedAccents = new List<IAccent>();
		SelectedRoles = new List<IChargenRole>();
		SkillValues = new List<Tuple<ITraitDefinition, double>>();
		SelectedMerits = new List<ICharacterMerit>();
		SelectedKnowledges = new List<IKnowledge>();
		SelectedSkillBoosts = new Dictionary<ITraitDefinition, int>();
		SelectedSkillBoostCosts = new Dictionary<IChargenResource, int>();
	}

	public override string FrameworkItemType => "Chargen";

	public static IFutureProg NeedsModelProg { get; set; }

	public bool IsSpecialApplication { get; set; }
	public ApplicationType ApplicationType { get; set; }


	public static void HandleDiscordRequest(DiscordRequestType requestType, IFuturemud gameworld, long requestedChargen,
		ulong discordUserId, ulong discordChannelId, long reviewerAccountId, string message,
		Action<string> responseCallback)
	{
		Chargen chargen;
		IAccount reviewerAccount;
		using (new FMDB())
		{
			var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Id == reviewerAccountId);
			if (dbaccount == null)
			{
				responseCallback(
					$"error {discordUserId} {discordChannelId} There was an error locating your MUD Account. You should probably panic.");
				return;
			}

			reviewerAccount = gameworld.TryAccount(dbaccount);

			var dbitem = FMDB.Context.Chargens.FirstOrDefault(x => x.Id == requestedChargen);
			if (dbitem == null)
			{
				responseCallback(
					$"error {discordUserId} {discordChannelId} There is no character application with ID {requestedChargen:N0}");
				return;
			}

			switch ((CharacterStatus)dbitem.Status)
			{
				case CharacterStatus.Creating:
					responseCallback(
						$"error {discordUserId} {discordChannelId} That character application has not yet been submitted.");
					return;
				case CharacterStatus.Submitted:
					break;
				case CharacterStatus.Active:
					responseCallback(
						$"error {discordUserId} {discordChannelId} That character application has already been approved.");
					return;
				default:
					responseCallback(
						$"error {discordUserId} {discordChannelId} Character status error with that character application.");
					return;
			}

			chargen = new Chargen(dbitem, gameworld, dbitem.Account);
		}

		var reviewerAuthority = reviewerAccount.Authority?.Level ?? PermissionLevel.Guest;

		if (requestType == DiscordRequestType.Show)
		{
			responseCallback(
				$"chargeninfo {discordUserId} {discordChannelId} {chargen.DisplayForReview(reviewerAccount, reviewerAuthority).RawText()}");
			return;
		}


		if (chargen.MinimumApprovalAuthority > reviewerAuthority)
		{
			responseCallback(
				$"error {discordUserId} {discordChannelId} That character application requires the {chargen.MinimumApprovalAuthority.Describe()} level of authority to approve or reject.");
			return;
		}

		if (reviewerAuthority < PermissionLevel.HighAdmin)
		{
			var blockingRole = chargen.SelectedRoles.FirstOrDefault(x =>
				x.RequiredApprovers.Any() && x.RequiredApprovers.All(y => !y.EqualTo(reviewerAccount.Name)));
			if (blockingRole != null)
			{
				responseCallback(
					$"error {discordUserId} {discordChannelId} That character application has the role {blockingRole.Name.TitleCase()} which requires specific reviewers, and you are not one of them.");
				return;
			}
		}

		if (requestType == DiscordRequestType.Approve)
		{
			if (chargen.ApplicationCosts.Any(x => chargen.Account.AccountResources.ValueOrDefault(x.Key, 0) < x.Value))
			{
				responseCallback(
					$"error {discordUserId} {discordChannelId} That character application no longer has sufficient account resources to pay for that application.");
				return;
			}

			chargen.ApproveApplication(null, reviewerAccount, message, null);
			return;
		}

		chargen.RejectApplication(null, reviewerAccount, message, null);
	}

	public bool CanSubmit => !string.IsNullOrEmpty(SelectedSdesc) &&
	                         !string.IsNullOrEmpty(SelectedFullDesc) &&
	                         SelectedRace != null &&
	                         SelectedCulture != null &&
	                         SelectedEthnicity != null &&
	                         SelectedName != null &&
	                         SelectedGender != Gender.Indeterminate &&
	                         SelectedBirthday != null &&
	                         SelectedHeight > 0.0 &&
	                         SelectedWeight > 0.0 &&
	                         SelectedAttributes.Any() &&
	                         SelectedSkills.Any() &&
	                         SelectedAccents.Any() &&
	                         SelectedCharacteristics.Any() &&
	                         SelectedNotes.Any() &&
	                         SelectedStartingLocation != null &&
	                         Handedness != Alignment.Irrelevant;

	public IEnumerable<Tuple<string, string>> PriorRejections => _priorRejections;

	public IEnumerable<ChargenStage> CompletedStages => _completedStages;

	public IChargenScreen CurrentScreen { get; protected set; }

	public IChargenMenu Menu { get; set; }

	public List<Tuple<string, string>> SelectedNotes { get; set; }

	public StartingLocation StartingLocation { get; set; }

	public ChargenStage Stage { get; protected set; }

	public ChargenState State { get; protected set; }
	private long? _approvedById;
	private IAccount _approvedBy;

	public IAccount ApprovedBy
	{
		get
		{
			if (_approvedBy is null && _approvedById is not null)
			{
				_approvedBy = Gameworld.TryAccount(_approvedById.Value);
			}

			return _approvedBy;
		}
		protected set
		{
			_approvedBy = value;
			_approvedById = value?.Id;
		}
	}

	public PermissionLevel MinimumApprovalAuthority {
		get
		{
			if (SelectedRoles.Any())
			{
				return SelectedRoles.Max(x => x.MinimumPermissionToApprove);
			}
			return PermissionLevel.Guest;
		}
	}

	public IReadOnlyDictionary<IChargenResource, int> CurrentCosts => ApplicationCosts;

	public void RecalculateCurrentCosts()
	{
		ApplicationCosts.Clear();
		foreach (var screen in _completedStages.Plus(Stage).Distinct()
		                                       .Where(x => Gameworld.ChargenStoryboard.StageScreenMap.ContainsKey(x))
		                                       .Select(x => Gameworld.ChargenStoryboard.StageScreenMap[x]))
		foreach (var (resource, cost) in screen.ChargenCosts(this))
		{
			ApplicationCosts[resource] = ApplicationCosts.ValueOrDefault(resource, 0) + cost;
		}
	}

	public Dictionary<IChargenResource, int> ApplicationCosts { get; } = new();

	public bool ApplicationLocked { get; protected set; }

	public string NeedsModel => NeedsModelProg != null ? (string)NeedsModelProg.Execute(this) : "NoNeeds";

	public IAccount Account { get; set; }

	public List<ITrait> SelectedAttributes { get; set; }

	public MudDate SelectedBirthday { get; set; }

	public List<Tuple<ICharacteristicDefinition, ICharacteristicValue>> SelectedCharacteristics { get; set; }

	public ICulture SelectedCulture { get; set; }

	public List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns { get; set; }

	public IEthnicity SelectedEthnicity { get; set; }

	public string SelectedFullDesc { get; set; }

	public Gender SelectedGender { get; set; }

	public double SelectedHeight { get; set; }

	public IPersonalName SelectedName { get; set; }

	public IRace SelectedRace { get; set; }

	public string SelectedSdesc { get; set; }

	public List<IBodypart> MissingBodyparts { get; set; } = new();

	public List<ITraitDefinition> SelectedSkills { get; set; }

	public Dictionary<ITraitDefinition, int> SelectedSkillBoosts { get; set; }

	public Dictionary<IChargenResource, int> SelectedSkillBoostCosts { get; set; }

	public List<Tuple<ITraitDefinition, double>> SkillValues { get; set; }

	public double SelectedWeight { get; set; }

	public List<IAccent> SelectedAccents { get; set; }

	public ICell SelectedStartingLocation => StartingLocation?.Location;

	public List<IChargenRole> SelectedRoles { get; set; }

	public List<ICharacterMerit> SelectedMerits { get; set; }

	public List<IKnowledge> SelectedKnowledges { get; set; }

	public Alignment Handedness { get; set; }

	public List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)> SelectedDisfigurements { get; set; } =
		new();

	public List<IGameItemProto> SelectedProstheses { get; set; } = new();

	public IEnumerable<IChargenAdvice> AllAdvice => Enumerable.Empty<IChargenAdvice>(); // TODO

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion IHaveFuturemud Members

	/// <summary>
	///     Performs boot time tasks necessary for Chargen
	/// </summary>
	/// <param name="gameworld"></param>
	public static void SetupChargen(IFuturemud gameworld)
	{
		gameworld.HeartbeatManager.MinuteHeartbeat += () =>
		{
			using (new FMDB())
			{
				var applications =
					FMDB.Context.Chargens.Where(x => x.Status == (int)ChargenState.ExternallyApproved).ToList();
				foreach (var application in applications)
				{
					new Chargen(application, gameworld, application.Account).ApproveApplicationExternal();
				}

				var needsModel = gameworld.GetStaticConfiguration("ChargenNeedsModelProg");
				NeedsModelProg =
					gameworld.FutureProgs.Get(long.Parse(needsModel ?? "0"));
			}
		};
	}

	public string Display()
	{
		return CurrentScreen.Display();
	}

	/// <summary>
	///     Locks the application and prevents anybody from reviewing it
	/// </summary>
	public void LockApplication()
	{
		ApplicationLocked = true;
	}

	/// <summary>
	///     Releases any previous held application locks
	/// </summary>
	public void ReleaseApplication()
	{
		ApplicationLocked = false;
	}

	public string DisplayForReviewForDiscord(IAccount character, PermissionLevel permission)
	{
		RecalculateCurrentCosts();
		var sb = new StringBuilder();
		sb.Append(ApplicationType == ApplicationType.Special ? "__Special " : "__Character ");
		if (permission >= PermissionLevel.Guide)
		{
			sb.AppendLine($"Application #{Id.ToString("N0", character)}__");
			sb.AppendLine($"**Account:** {Account.Name.Proper()}");
			sb.AppendLine($"**Approval Level:** {MinimumApprovalAuthority.Describe().TitleCase()}");
			sb.AppendLine($"**Times Rejected:** {PriorRejections.Count().ToString("F0", character)}");
			sb.AppendLine($"**Total Cost:** {ApplicationCosts.Select(x => $"{x.Value} {x.Key.Alias}").ListToString()}");
		}
		else
		{
			sb.AppendLine("Application__");
		}

		sb.AppendLine($"**Name:** {SelectedName.GetName(NameStyle.FullName)}");
		if (!SelectedName.GetName(NameStyle.Affectionate).EqualTo(SelectedName.GetName(NameStyle.GivenOnly)))
		{
			sb.AppendLine($"**Nickname:** {SelectedName.GetName(NameStyle.Affectionate)}");
		}

		sb.AppendLine(
			$"**SDesc:** {IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedSdesc, SelectedCharacteristics, Gendering.Get(SelectedGender), Gameworld, SelectedRace, SelectedCulture, SelectedEthnicity, SelectedCulture?.PrimaryCalendar.CurrentDate.YearsDifference(SelectedBirthday ?? SelectedCulture?.PrimaryCalendar.CurrentDate) ?? 0, SelectedHeight)}");
		sb.AppendLine($"**Gender:** {Gendering.Get(SelectedGender).Name.Proper()}");
		sb.AppendLine($"**Race:** {SelectedRace.Name}");
		sb.AppendLine($"**Ethnicity:** {SelectedEthnicity.Name}");
		sb.AppendLine($"**Culture:** {SelectedCulture.Name}");
		if (SelectedRoles.Any(x => x.RoleType == ChargenRoleType.Class))
		{
			sb.AppendLine($"**Class:** {SelectedRoles.First(x => x.RoleType == ChargenRoleType.Class).Name}");
		}

		if (SelectedRoles.Any(x => x.RoleType == ChargenRoleType.Subclass))
		{
			sb.AppendLine($"**Subclass:** {SelectedRoles.First(x => x.RoleType == ChargenRoleType.Subclass).Name}");
		}

		sb.AppendLine(
			$"**Height:** {Gameworld.UnitManager.DescribeMostSignificantExact(SelectedHeight, UnitType.Length, character)}");
		sb.AppendLine(
			$"**Weight:** {Gameworld.UnitManager.DescribeMostSignificantExact(SelectedWeight, UnitType.Mass, character)}");
		var age = SelectedBirthday.Calendar.CurrentDate.YearsDifference(SelectedBirthday);
		sb.AppendLine(
			$"**Birthday:** {SelectedBirthday.Calendar.DisplayDate(SelectedBirthday, CalendarDisplayMode.Short)} ({SelectedRace.AgeCategory(age)} - {age.ToString("N0", character)} years old)");
		sb.AppendLine();
		sb.Append("**Attributes:** ");
		sb.AppendLine(SelectedAttributes.Select(x =>
			                                $"{x.Definition.Name.Proper()}: {(permission >= PermissionLevel.JuniorAdmin ? x.Value.ToString("N0", character) : x.Definition.Decorator.Decorate(x))}")
		                                .ListToString(conjunction: ""));
		sb.AppendLine();
		sb.Append($"**Skills:** ");
		sb.AppendLine(SelectedSkills
		              .Select(x =>
			              $"{x.Name}{(SelectedSkillBoosts.ContainsKey(x) && SelectedSkillBoosts[x] > 0 ? $" [{new string('+', SelectedSkillBoosts[x])}]" : "")}")
		              .ListToString(conjunction: ""));
		sb.AppendLine();
		sb.AppendLine("**Languages:** " +
		              SelectedSkills.SelectNotNull(x => Gameworld.Languages.FirstOrDefault(y => y.LinkedTrait == x))
		                            .Select(x =>
			                            $"{x.Name.Proper()} ({SelectedAccents.Where(y => y.Language == x).Select(y => y.Name.Proper()).ListToString()})")
		                            .ListToString());
		if (SelectedKnowledges.Any())
		{
			sb.AppendLine();
			sb.AppendLine($"**Knowledges:** {SelectedKnowledges.Select(x => x.Name).ListToString(conjunction: "")}");
		}

		if (SelectedRoles.Any(x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass))
		{
			sb.AppendLine();
			sb.AppendLine(
				$"**Roles:** {(permission >= PermissionLevel.JuniorAdmin ? SelectedRoles.Where(x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass).OrderBy(x => x.Id).Select(x => string.Format(character, "[#{0}] {1}", x.Id.ToString("N0", character), x.Name.TitleCase())).ListToString(conjunction: "") : SelectedRoles.Where(x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass).OrderBy(x => x.Name).Select(x => x.Name.TitleCase()).ListToString(conjunction: ""))}");
		}

		if (SelectedMerits.Any())
		{
			sb.AppendLine();
			sb.AppendLine(
				$"**Quirks:** {SelectedMerits.Select(x => x.Name.TitleCase()).ListToString(conjunction: "")}");
		}

		sb.AppendLine();
		sb.AppendLine("**Full Description:**");
		sb.AppendLine();
		sb.AppendLine(
			IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedFullDesc, SelectedCharacteristics,
				                              Gendering.Get(SelectedGender), Gameworld, SelectedRace, SelectedCulture,
				                              SelectedEthnicity,
				                              SelectedBirthday?.Calendar.CurrentDate.YearsDifference(SelectedBirthday ??
					                              SelectedCulture?.PrimaryCalendar
					                                             .CurrentDate) ?? 0, SelectedHeight)
			                              .ProperSentences());

		foreach (var note in SelectedNotes)
		{
			sb.AppendLine();
			sb.AppendLine($"**{note.Item1}:**");
			sb.AppendLine();
			sb.AppendLine(note.Item2);
		}

		return sb.ToString();
	}

	public string DisplayForReview(IAccount character, PermissionLevel permission)
	{
		RecalculateCurrentCosts();
		var sb = new StringBuilder();
		sb.Append(ApplicationType == ApplicationType.Special ? "Special " : "Character ");
		if (permission >= PermissionLevel.Guide)
		{
			sb.AppendLine(("Application #" + Id.ToString("N0", character)).Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.Append(new[]
			{
				$"Account: {Account.Name.Proper().Colour(Telnet.Green)}",
				$"Approval Level: {MinimumApprovalAuthority.Describe().Proper().Colour(Telnet.Green)}",
				$"Times Rejected: {PriorRejections.Count().ToString().Colour(Telnet.Green)}"
			}.ArrangeStringsOntoLines(3, (uint)character.LineFormatLength));
			sb.AppendLine(
				$"Total Cost: {ApplicationCosts.Where(x => x.Value > 0).Select(x => $"{x.Value} {x.Key.Alias}".Colour(Telnet.Green)).ListToString()}");
		}
		else
		{
			sb.AppendLine("Application".ColourBold(Telnet.Cyan));
			sb.AppendLine();
		}

		sb.Append(new[]
		{
			$"Name: {SelectedName?.GetName(NameStyle.FullName).Colour(Telnet.Green) ?? "None".ColourError()}",
			SelectedName is not null && SelectedName.GetName(NameStyle.Affectionate) !=
			SelectedName.GetName(NameStyle.GivenOnly)
				? $"Nickname: {SelectedName.GetName(NameStyle.Affectionate).Colour(Telnet.Green)}"
				: "",
			$"Gender: {Gendering.Get(SelectedGender).Name.Proper().Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(3, (uint)character.LineFormatLength));

		sb.Append(new[]
		{
			$"Race: {SelectedRace?.Name.Colour(Telnet.Green) ?? "None".ColourError()}",
			$"Ethnicity: {SelectedEthnicity?.Name.Colour(Telnet.Green) ?? "None".ColourError()}",
			$"Culture: {SelectedCulture?.Name.Colour(Telnet.Green) ?? "None".ColourError()}"
		}.ArrangeStringsOntoLines(3, (uint)character.LineFormatLength));

		if (SelectedRoles.Any(x => x.RoleType == ChargenRoleType.Class))
		{
			sb.Append(new[]
			{
				$"Class: {SelectedRoles.First(x => x.RoleType == ChargenRoleType.Class).Name.Colour(Telnet.Green)}",
				$"Subclass: {(SelectedRoles.Any(x => x.RoleType == ChargenRoleType.Subclass) ? SelectedRoles.First(x => x.RoleType == ChargenRoleType.Subclass).Name.Colour(Telnet.Green) : "None".Colour(Telnet.Red))}",
				""
			}.ArrangeStringsOntoLines(3, (uint)character.LineFormatLength));
		}

		sb.Append(new[]
		{
			$"Height: {Gameworld.UnitManager.Describe(SelectedHeight, UnitType.Length, character).Colour(Telnet.Green)}",
			$"Weight: {Gameworld.UnitManager.Describe(SelectedWeight, UnitType.Mass, character).Colour(Telnet.Green)}",
			""
		}.ArrangeStringsOntoLines(3, (uint)character.LineFormatLength));

		if (SelectedBirthday is not null)
		{
			var age = SelectedBirthday.Calendar.CurrentDate.YearsDifference(SelectedBirthday);

			sb.Append(new[]
			{
				$"Age: {$"{age} years old".ColourValue()}",
				$"Category: {SelectedRace?.AgeCategory(age).DescribeEnum(true).ColourValue() ?? ""}",
				$"Birthday: {SelectedBirthday.Calendar.DisplayDate(SelectedBirthday, CalendarDisplayMode.Short).ColourValue() ?? "None".ColourError()}"
			}.ArrangeStringsOntoLines(3, (uint)character.LineFormatLength));
		}

		sb.AppendLine();
		if (!string.IsNullOrEmpty(SelectedSdesc))
		{
			sb.AppendLine(
				$"Short Description: {IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedSdesc, SelectedCharacteristics, Gendering.Get(SelectedGender), Gameworld, SelectedRace, SelectedCulture, SelectedEthnicity, SelectedBirthday?.Calendar.CurrentDate.YearsDifference(SelectedBirthday ?? SelectedCulture?.PrimaryCalendar.CurrentDate) ?? 0, SelectedHeight).Colour(Telnet.Magenta)}");
		}
		else
		{
			sb.AppendLine($"Short Description: {"None".ColourError()}");
		}

		sb.AppendLine("Full Description:");
		sb.AppendLine();
		if (SelectedFullDesc is null || SelectedCharacteristics is null || SelectedCulture is null ||
		    SelectedEthnicity is null || SelectedRace is null)
		{
			sb.AppendLine($"\t{"Not Yet Set".ColourError()}");
		}
		else
		{
			sb.AppendLine(
				IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedFullDesc,
					                              SelectedCharacteristics,
					                              Gendering.Get(SelectedGender), Gameworld, SelectedRace,
					                              SelectedCulture, SelectedEthnicity,
					                              SelectedBirthday?.Calendar.CurrentDate.YearsDifference(
						                              SelectedBirthday ??
						                              SelectedCulture?.PrimaryCalendar
						                                             .CurrentDate) ?? 0, SelectedHeight)
				                              .ProperSentences()
				                              .Wrap(character.InnerLineFormatLength, "\t")
				                              .NoWrap());
		}

		sb.AppendLine();
		sb.AppendLine("Attributes:");
		sb.AppendLine();
		sb.AppendLine(SelectedAttributes.Select(x =>
			                                $"{x.Definition.Name.Proper()} [{(permission >= PermissionLevel.JuniorAdmin ? x.Value.ToString("N0", character) : x.Definition.Decorator.Decorate(x)).Colour(Telnet.Green)}]")
		                                .ListToLines(true));
		sb.AppendLine();
		sb.AppendLine("Skills:");
		sb.AppendLine();
		sb.Append(SelectedSkills.Select(x =>
			                        $"\t{x.Name}{(SelectedSkillBoosts.ContainsKey(x) && SelectedSkillBoosts[x] > 0 ? $" [{new string('+', SelectedSkillBoosts[x]).ColourValue()}]" : "")}")
		                        .ArrangeStringsOntoLines(5, (uint)character.LineFormatLength));
		sb.AppendLine();
		sb.AppendLine(
			$"Languages and Accents:\n\n{SelectedSkills.SelectNotNull(x => Gameworld.Languages.FirstOrDefault(y => y.LinkedTrait == x)).Select(x => $"{x.Name.Proper()} ({SelectedAccents.Where(y => y.Language == x).Select(y => y.Name.Proper()).ListToString()})".Colour(Telnet.Green)).ListToLines(true)}");
		if (SelectedKnowledges.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Knowledges:");
			sb.AppendLine();
			sb.Append(SelectedKnowledges.Select(x => $"\t{x.Description.ColourValue()}")
			                            .ArrangeStringsOntoLines(5, (uint)character.LineFormatLength));
		}

		if (SelectedRoles.Any(x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass))
		{
			sb.AppendLine();
			sb.AppendLine("Roles:");
			sb.AppendLine();
			sb.AppendLine(permission >= PermissionLevel.JuniorAdmin
				? SelectedRoles.Where(
					               x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass)
				               .OrderBy(x => x.Id)
				               .Select(x =>
					               string.Format(character, "\t#{0:N0}: {1}", x.Id, x.Name.TitleCase().ColourValue()))
				               .ArrangeStringsOntoLines(1, (uint)character.LineFormatLength)
				: SelectedRoles.Where(
					               x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass)
				               .OrderBy(x => x.Name)
				               .Select(x => "\t" + x.Name.TitleCase().ColourValue())
				               .ArrangeStringsOntoLines(1, (uint)character.LineFormatLength));
		}

		if (SelectedMerits.Any())
		{
			if (SelectedMerits.Any(x => x == null))
			{
				Console.WriteLine("Application {0} had null merits!", Account.Name);
			}

			sb.AppendLine();
			sb.AppendLineFormat("Merits and Flaws:");
			sb.AppendLineFormat("\tMerits: {0}",
				SelectedMerits.Where(x => x.MeritType == MeritType.Merit)
				              .Select(x => x.Name.TitleCase().Colour(Telnet.Green))
				              .ListToString());
			sb.AppendLineFormat("\tFlaws: {0}",
				SelectedMerits.Where(x => x.MeritType == MeritType.Flaw)
				              .Select(x => x.Name.TitleCase().Colour(Telnet.Green))
				              .ListToString());
		}

		foreach (var note in SelectedNotes)
		{
			sb.AppendLine();
			sb.AppendLine(note.Item1 + ":");
			sb.AppendLine();
			sb.AppendLine(note.Item2.Wrap(character.InnerLineFormatLength, "\t").NoWrap());
		}

		return sb.ToString();
	}

	public long ApproveApplicationExternal()
	{
		PerformPostCreationProcessing();
		RecalculateCurrentCosts();
		ReleaseApplication();
		// TODO this function and process in general could use some work.
		var timestamp = DateTime.UtcNow;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Chargens.Find(Id);
			var character = new Character.Character(Gameworld, this);
			Gameworld.SaveManager.Flush();
			var id = character.Id;
			lock (Gameworld.Connections)
			{
				var playerConnection =
					Gameworld.Connections.FirstOrDefault(
						x => x.ControlPuppet.Account != null && x.ControlPuppet.Account == Account);
				playerConnection?.ControlPuppet?.OutputHandler?.Send(
					$"Your character {SelectedName.GetName(NameStyle.FullWithNickname).TitleCase()} has been approved. You may now log in and play {Gendering.Get(SelectedGender).Objective()}.");
			}

			character.AddEffect(new NewPlayer(character), NewPlayer.NewPlayerEffectLength);

			dbitem.Status = (int)ChargenState.Approved;

			foreach (var item in SelectedNotes)
			{
				var note = new Models.AccountNote
				{
					AuthorId = null,
					AccountId = Account.Id,
					TimeStamp = timestamp,
					Subject = SelectedName.GetName(NameStyle.FullName) + " - " + item.Item1,
					Text = item.Item2
				};
				FMDB.Context.AccountNotes.Add(note);
			}

			FMDB.Context.SaveChanges();

			var dbaccount = FMDB.Context.Accounts.Find(Account.Id);
			if (dbaccount == null)
			{
				throw new ApplicationException("Account was not found in the database in Chargen ApproveCharacter.");
			}

			foreach (var cost in ApplicationCosts)
			{
				var dbaccountresource =
					dbaccount.AccountsChargenResources.FirstOrDefault(
						x => x.ChargenResourceId == cost.Key.Id);
				if (dbaccountresource == null)
				{
					Console.WriteLine("Deducting a resource in chargen that the character did not have.");
					continue;
				}

				dbaccountresource.Amount -= cost.Value;
				dbaccountresource.Amount = Math.Max(dbaccountresource.Amount, 0);
				Account.AccountResources[cost.Key] = dbaccountresource.Amount;
			}

			Gameworld.GameStatistics.UpdateApplicationApproved();
			FMDB.Context.SaveChanges();

			EmailHelper.Instance.SendEmail(EmailTemplateTypes.CharacterApplicationApproved, Account.EmailAddress,
				Account.Name.Proper(), SelectedName.GetName(NameStyle.FullWithNickname).TitleCase(), "System",
				"Welcome to FutureMUD");

			return id;
		}
	}

	public long ApproveApplication(ICharacter approver, IAccount approverAccount, string comment,
		IOutputHandler handler)
	{
		RecalculateCurrentCosts();
		ReleaseApplication();
		Gameworld.DiscordConnection?.NotifyCharacterApproval(Account.Name.Proper(),
			SelectedName.GetName(NameStyle.FullWithNickname), approverAccount?.Name.Proper() ?? "the system");
		var timestamp = DateTime.UtcNow;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Chargens.Find(Id);

			var newChar = new Character.Character(Gameworld, this);
			Gameworld.SaveManager.Flush();
			//var id = Character.Character.CreateCharacterFromTemplate(this, Account);
			var dbchar = FMDB.Context.Characters.Find(newChar.Id);
			var id = newChar.Id;
			if (approver != null)
			{
				Gameworld.SystemMessage(new EmoteOutput(new Emote(
					$"You|{approver.Account.Name} have|has approved {Account.Name}'s character \"{SelectedName.GetName(NameStyle.FullName)}\" (ID {id})",
					approver, permitSpeech: PermitLanguageOptions.IgnoreLanguage)), true);
			}
			else if (approverAccount is not null)
			{
				Gameworld.SystemMessage(
					$"{approverAccount.Name} has approved {Account.Name}'s character \"{SelectedName.GetName(NameStyle.FullName)}\" (ID {id})",
					true);
			}
			else
			{
				Gameworld.SystemMessage(
					$"The system has automatically approved {Account.Name}'s character \"{SelectedName.GetName(NameStyle.FullName)}\" (ID {id})",
					true);
			}

			lock (Gameworld.Connections)
			{
				var playerConnection =
					Gameworld.Connections.FirstOrDefault(
						x => x.ControlPuppet.Account != null && x.ControlPuppet.Account == Account);
				playerConnection?.ControlPuppet.OutputHandler.Send(
					$"Your character {SelectedName.GetName(NameStyle.FullWithNickname).TitleCase()} has been approved. You may now log in and play {Gendering.Get(SelectedGender).Objective()}.");
			}

			newChar.AddEffect(new NewPlayer(newChar), NewPlayer.NewPlayerEffectLength);
			dbchar.ShownIntroductionMessage = false;

			EmailHelper.Instance.SendEmail(EmailTemplateTypes.CharacterApplicationApproved, Account.EmailAddress,
				Account.Name.Proper(), SelectedName.GetName(NameStyle.FullWithNickname).TitleCase(),
				approverAccount?.Name.Proper() ?? "the system", comment.Wrap(80));
			FMDB.Context.SaveChanges();

			dbitem.ApprovalTime = timestamp;
			dbitem.ApprovedById = approverAccount?.Id;
			ApprovedBy = approverAccount;
			dbitem.Status = (int)ChargenState.Approved;

			foreach (var item in SelectedNotes)
			{
				var note = new Models.AccountNote
				{
					AuthorId = null,
					AccountId = Account.Id,
					TimeStamp = timestamp,
					Subject = SelectedName.GetName(NameStyle.FullName) + " - " + item.Item1,
					Text = item.Item2
				};
				FMDB.Context.AccountNotes.Add(note);
			}

			var approvalNote = new Models.AccountNote
			{
				AccountId = Account.Id,
				AuthorId = approverAccount?.Id,
				Subject = SelectedName.GetName(NameStyle.FullName) + " - Approval Note",
				Text = comment,
				TimeStamp = timestamp
			};
			FMDB.Context.AccountNotes.Add(approvalNote);

			FMDB.Context.SaveChanges();

			var dbaccount = FMDB.Context.Accounts.Find(Account.Id);
			if (dbaccount == null)
			{
				throw new ApplicationException("Account was not found in the database in Chargen ApproveCharacter.");
			}

			foreach (var cost in ApplicationCosts.Where(x => x.Value > 0))
			{
				var dbaccountresource =
					dbaccount.AccountsChargenResources.FirstOrDefault(
						x => x.ChargenResourceId == cost.Key.Id);
				if (dbaccountresource == null)
				{
					Console.WriteLine("Deducting a resource in chargen that the character did not have.");
					continue;
				}

				dbaccountresource.Amount -= cost.Value;
				dbaccountresource.Amount = Math.Max(dbaccountresource.Amount, 0);
				Account.AccountResources[cost.Key] = dbaccountresource.Amount;
			}

			Gameworld.GameStatistics.UpdateApplicationApproved();
			FMDB.Context.SaveChanges();
			newChar.Quit(); //Clean up the newly made character so it doesn't hang around
			return id;
		}
	}

	public void RejectApplication(ICharacter rejecter, IAccount rejecterAccount, string comment, IOutputHandler handler)
	{
		comment ??= string.Empty;
		State = ChargenState.InProgress;
		_priorRejections.Add(Tuple.Create(rejecterAccount.Name, comment));
		Save();
		Gameworld.DiscordConnection?.NotifyCharacterRejection(Account.Name.Proper(),
			SelectedName.GetName(NameStyle.FullWithNickname), rejecterAccount.Name.Proper());
		if (rejecter != null)
		{
			Gameworld.SystemMessage(new EmoteOutput(new Emote(
				$"You|{rejecter.Account.Name} have|has rejected {Account.Name}'s character application \"{SelectedName.GetName(NameStyle.FullName)}\". They have been rejected {_priorRejections.Count} time{(_priorRejections.Count == 1 ? "" : "s")}.",
				rejecter, permitSpeech: PermitLanguageOptions.IgnoreLanguage)), true);
		}
		else
		{
			Gameworld.SystemMessage(
				$"{rejecterAccount.Name} has rejected {Account.Name}'s character application \"{SelectedName.GetName(NameStyle.FullName)}\". They have been rejected {_priorRejections.Count} time{(_priorRejections.Count == 1 ? "" : "s")}.",
				true);
		}

		EmailHelper.Instance.SendEmail(EmailTemplateTypes.CharacterApplicationRejected, Account.EmailAddress,
			Account.Name.Proper(), SelectedName.GetName(NameStyle.FullWithNickname).TitleCase(),
			rejecterAccount.Name.Proper(), comment.Wrap(80));
	}

	public string HandleCommand(string command)
	{
		if (command.ToLowerInvariant() == "quit")
		{
			Stage = ChargenStage.ConfirmQuit;
			return "Do you want to save your application for later? Type " + "yes".Colour(Telnet.Yellow) +
			       " to do so, or " + "no".Colour(Telnet.Yellow) + " to delete this application.";
		}

		if (Stage == ChargenStage.ConfirmQuit)
		{
			if (!string.IsNullOrEmpty(command) &&
			    "no".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (Id != 0)
				{
					using (new FMDB())
					{
						Gameworld.SaveManager.Flush();
						var dbitem = FMDB.Context.Chargens.Find(Id);
						if (dbitem != null)
						{
							FMDB.Context.Chargens.Remove(dbitem);
							FMDB.Context.SaveChanges();
						}
					}
				}

				State = ChargenState.Deleted;
				return "You delete your application.";
			}

			Save();
			State = ChargenState.Halt;
			return "You save your application for later.";
		}

		if (command.ToLowerInvariant() == "menu")
		{
			CurrentScreen = Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.Menu].GetScreen(this);
			Stage = ChargenStage.Menu;
			return CurrentScreen.Display();
		}

		var result = CurrentScreen.HandleCommand(command);
		RecalculateCurrentCosts();
		if (CurrentScreen.State == ChargenScreenState.Complete)
		{
			FinishCurrentStage();
			return State == ChargenState.InProgress
				? result.ConcatIfNotEmpty("\n") + CurrentScreen.Display()
				: result;
		}

		return result;
	}

	public void ResetStage(ChargenStage stage)
	{
		switch (stage)
		{
			case ChargenStage.SelectAttributes:
				SelectedAttributes.Clear();
				break;

			case ChargenStage.SelectBirthday:
				SelectedBirthday = null;
				break;

			case ChargenStage.SelectCharacteristics:
				SelectedCharacteristics.Clear();
				break;

			case ChargenStage.SelectCulture:
				SelectedCulture = null;
				break;

			case ChargenStage.SelectEthnicity:
				SelectedEthnicity = null;
				break;

			case ChargenStage.SelectGender:
				SelectedGender = Gender.Indeterminate;
				break;

			case ChargenStage.SelectHeight:
				SelectedHeight = 0;
				break;

			case ChargenStage.SelectName:
				if (SelectedName != null && SelectedName.Id != 0)
				{
					using (new FMDB())
					{
						FMDB.Connection.Execute($"DELETE FROM PersonalName WHERE Id = {SelectedName.Id:G0}");
						FMDB.Context.SaveChanges();
					}
				}

				SelectedName = null;
				break;

			case ChargenStage.SelectDescription:
				SelectedSdesc = null;
				SelectedFullDesc = null;
				SelectedEntityDescriptionPatterns.Clear();
				break;

			case ChargenStage.SelectRace:
				SelectedRace = null;
				break;

			case ChargenStage.SelectSkills:
				SelectedSkills.Clear();
				SelectedSkillBoostCosts.Clear();
				SelectedSkillBoosts.Clear();
				break;

			case ChargenStage.SelectWeight:
				SelectedWeight = 0;
				break;

			case ChargenStage.SelectNotes:
				SelectedNotes.Clear();
				break;

			case ChargenStage.SelectAccents:
				SelectedAccents.Clear();
				break;

			case ChargenStage.SelectStartingLocation:
				StartingLocation = null;
				SelectedRoles.RemoveAll(x => x.RoleType == ChargenRoleType.StartingLocation);
				break;

			case ChargenStage.SelectRole:
				SelectedRoles.RemoveAll(x => x.RoleType != ChargenRoleType.StartingLocation);
				break;

			case ChargenStage.SelectMerits:
				SelectedMerits.Clear();
				break;

			case ChargenStage.SelectKnowledges:
				SelectedKnowledges.Clear();
				break;
			case ChargenStage.SelectHandedness:
				Handedness = Alignment.Irrelevant;
				break;
			case ChargenStage.SpecialApplication:
				ApplicationType = ApplicationType.Normal;
				break;
			case ChargenStage.SelectDisfigurements:
				MissingBodyparts.Clear();
				SelectedDisfigurements.Clear();
				SelectedProstheses.Clear();
				break;
		}

		_completedStages.Remove(stage);
		var invalidatedStages =
			Gameworld.ChargenStoryboard.StageDependencies.Where(
				x => _completedStages.Contains(x.Key) && x.Value.Any(y => y == stage)).ToList();
		foreach (var otherStage in invalidatedStages)
		{
			ResetStage(otherStage.Key);
		}
	}

	public void SetEditor(EditorController controller)
	{
		Menu.MenuSetContext(controller);
	}

	public void SetStage(ChargenStage stage)
	{
		Stage = stage;
		CurrentScreen = Gameworld.ChargenStoryboard.StageScreenMap[Stage].GetScreen(this);
	}

	/// <summary>
	///     Handles when control is returned to a Chargen, usually either when it is first created or when it returns from an
	///     editor.
	/// </summary>
	public void ControlReturned()
	{
		if (CurrentScreen.State == ChargenScreenState.Complete)
		{
			FinishCurrentStage();
			Menu.OutputHandler.Send(CurrentScreen.Display(), nopage: true);
			return;
		}

		Menu.OutputHandler.Send(Display(), nopage: true);
	}

	private void PerformPostCreationProcessing()
	{
		SkillValues.Clear();
		foreach (var skill in SelectedSkills)
		{
			SkillValues.Add(Tuple.Create(skill,
				Convert.ToDouble(SelectedCulture.SkillStartingValueProg.Execute(this, skill,
					SelectedSkillBoosts.TryGetValue(skill, out var value) ? value : 0))));
		}
	}

	private void FinishCurrentStage()
	{
		if (Stage == ChargenStage.ConfirmQuit)
		{
			if (CurrentScreen.NextScreen == null)
			{
				Save();
				State = ChargenState.Halt;
				return;
			}
		}

		if (Stage == ChargenStage.Submit)
		{
			State = ChargenState.Submitted;
			PerformPostCreationProcessing();
			Save();
			if (((SubmitScreenStoryboard)Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.Submit])
			    .AutomaticallyApproveAllApplicationsBelow >= MinimumApprovalAuthority)
			{
				Gameworld.SystemMessage(
					$"Account {Account.Name.Proper().Colour(Telnet.Green)} has submitted an application for character {SelectedName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}, which has been automatically approved by the system.",
					true);
				Gameworld.DiscordConnection.NotifyCharacterSubmission(Account.Name.Proper(),
					SelectedName.GetName(NameStyle.FullWithNickname), Id);
				Gameworld.GameStatistics.UpdateApplicationSubmitted();
				ApproveApplication(null, null, "Automatically approved by the system", null);
				return;
			}
			Gameworld.SystemMessage(
				$"Account {Account.Name.Proper().Colour(Telnet.Green)} has submitted an application for character {SelectedName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}, requiring the {MinimumApprovalAuthority.Describe().Colour(Telnet.Green)} level of approval.",
				true);
			Gameworld.DiscordConnection.NotifyCharacterSubmission(Account.Name.Proper(),
				SelectedName.GetName(NameStyle.FullWithNickname), Id);
			Gameworld.GameStatistics.UpdateApplicationSubmitted();
			return;
		}

		_completedStages.Add(Stage);

		while (true)
		{
			Stage = Gameworld.ChargenStoryboard.DefaultNextStage[Stage];
			if (Stage == ChargenStage.Menu || Stage == ChargenStage.Submit ||
			    Stage == ChargenStage.ConfirmQuit)
			{
				break;
			}

			if (!CompletedStages.Contains(Stage))
			{
				break;
			}
		}

		CurrentScreen = Gameworld.ChargenStoryboard.StageScreenMap[Stage].GetScreen(this);
		Save();

		if (CurrentScreen.State == ChargenScreenState.Complete)
		{
			FinishCurrentStage();
			Save();
		}
	}

	private void LoadFromXml(XElement root)
	{
		using (new FMDB())
		{
			Stage = (ChargenStage)Convert.ToInt32(root.Element("CurrentStage").Value);
			foreach (var completed in root.Element("CompletedStages").Elements("Stage"))
			{
				_completedStages.Add((ChargenStage)Convert.ToInt32(completed.Value));
			}

			SelectedSdesc = root.Element("SelectedSdesc").Value;
			SelectedFullDesc = root.Element("SelectedFullDesc").Value;
			SelectedRace = Gameworld.Races.Get(long.Parse(root.Element("SelectedRace").Value));
			SelectedCulture = Gameworld.Cultures.Get(long.Parse(root.Element("SelectedCulture").Value));
			SelectedEthnicity = Gameworld.Ethnicities.Get(long.Parse(root.Element("SelectedEthnicity").Value));
			SelectedGender = (Gender)Convert.ToInt32(root.Element("SelectedGender").Value);
			SelectedHeight = double.Parse(root.Element("SelectedHeight").Value);
			SelectedWeight = double.Parse(root.Element("SelectedWeight").Value);
			Handedness = (Alignment)int.Parse(root.Element("Handedness")?.Value ?? "3");

			var snElement = root.Element("SelectedName");
			if (snElement.Element("NotSet") != null || snElement.Value == "-1" || SelectedCulture == null ||
			    int.TryParse(snElement.Value, out _))
			{
				SelectedName = null;
			}
			else
			{
				SelectedName =
					new PersonalName(
						Gameworld.NameCultures.Get(long.Parse(snElement.Element("Name")?.Attribute("culture")?.Value ??
						                                      "0")) ?? SelectedCulture.NameCultureForGender(
							SelectedGender), snElement.Element("Name"));
			}

			try
			{
				SelectedBirthday = string.IsNullOrEmpty(root.Element("SelectedBirthday")?.Value)
					? null
					: SelectedCulture.PrimaryCalendar.GetDate(root.Element("SelectedBirthday").Value);
			}
			catch (MUDDateException)
			{
				// This should only happen when a name culture's calendar is changed after chargens have been created, and therefore rarely and only in some MUDs.
				foreach (var calendar in Gameworld.Calendars)
				{
					try
					{
						SelectedBirthday = calendar.GetDate(root.Element("SelectedBirthday").Value);
						break;
					}
					catch (MUDDateException)
					{
					}
				}
			}

			foreach (var item in root.Element("SelectedEntityDescriptionPatterns").Elements("Pattern"))
			{
				SelectedEntityDescriptionPatterns.Add(Gameworld.EntityDescriptionPatterns.Get(long.Parse(item.Value)));
			}

			foreach (var item in root.Element("SelectedAttributes").Elements("Attribute"))
			{
				SelectedAttributes.Add(TraitFactory.LoadAttribute(
					(IAttributeDefinition)Gameworld.Traits.Get(long.Parse(item.Attribute("Id").Value)), null,
					double.Parse(item.Attribute("Value").Value)));
			}

			foreach (var item in root.Element("SelectedSkills").Elements("Skill"))
			{
				var trait = Gameworld.Traits.Get(long.Parse(item.Value));
				if (trait == null)
				{
					continue;
				}

				SelectedSkills.Add(trait);
			}

			var element = root.Element("SkillValues");
			if (element != null)
			{
				foreach (var item in element.Elements("Skill"))
				{
					var skill = Gameworld.Traits.Get(long.Parse(item.Attribute("ID").Value));
					if (skill == null)
					{
						continue;
					}

					SkillValues.Add(Tuple.Create(skill, double.Parse(item.Attribute("Value").Value)));
				}
			}

			foreach (var item in root.Element("SelectedAccents").Elements("Accent"))
			{
				var accent = Gameworld.Accents.Get(long.Parse(item.Value));
				if (accent != null)
				{
					SelectedAccents.Add(accent);
				}
			}

			foreach (var item in root.Element("SelectedNotes").Elements("Note"))
			{
				SelectedNotes.Add(Tuple.Create(item.Attribute("Name").Value, item.Value));
			}

			foreach (var item in root.Element("SelectedCharacteristics").Elements("Characteristic"))
			{
				SelectedCharacteristics.Add(
					Tuple.Create(Gameworld.Characteristics.Get(long.Parse(item.Attribute("Definition").Value)),
						Gameworld.CharacteristicValues.Get(long.Parse(item.Attribute("Value").Value))));
			}

			if (root.Element("Rejections") != null)
			{
				foreach (var item in root.Element("Rejections").Elements("Rejection"))
				{
					_priorRejections.Add(Tuple.Create(item.Attribute("Rejecter").Value, item.Value));
				}
			}

			element = root.Element("SelectedStartingLocation");
			if (!string.IsNullOrEmpty(element?.Value))
			{
				StartingLocation =
					((StartingLocationPickerScreenStoryboard)
						Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.SelectStartingLocation])
					.Locations
					.FirstOrDefault(x => x.Role.Id == long.Parse(element.Value));
			}

			element = root.Element("SelectedRoles");
			if (element != null)
			{
				foreach (var role in element.Elements("Role"))
				{
					SelectedRoles.Add(Gameworld.Roles.Get(long.Parse(role.Value)));
				}
			}

			element = root.Element("SelectedMerits");
			if (element != null)
			{
				foreach (var merit in element.Elements("Merit"))
				{
					SelectedMerits.Add(Gameworld.Merits.Get(long.Parse(merit.Value)) as ICharacterMerit);
				}
			}

			element = root.Element("SelectedKnowledges");
			if (element != null)
			{
				foreach (var knowledge in element.Elements("Knowledge"))
				{
					SelectedKnowledges.Add(Gameworld.Knowledges.Get(long.Parse(knowledge.Value)));
				}
			}

			element = root.Element("SpecialApplication");
			if (!string.IsNullOrEmpty(element?.Value))
			{
				IsSpecialApplication = bool.Parse(element.Value);
			}

			element = root.Element("ApplicationType");
			if (element is not null)
			{
				ApplicationType = (ApplicationType)int.Parse(element.Value);
			}
			else
			{
				ApplicationType = IsSpecialApplication ? ApplicationType.Special : ApplicationType.Normal;
			}

			element = root.Element("SelectedBoosts");
			if (element != null)
			{
				foreach (var item in element.Elements())
				{
					SelectedSkillBoosts[Gameworld.Traits.Get(long.Parse(item.Value))] =
						int.Parse(item.Attribute("amount").Value);
				}
			}

			element = root.Element("MissingBodyparts");
			if (element != null)
			{
				foreach (var item in element.Elements())
				{
					MissingBodyparts.Add(Gameworld.BodypartPrototypes.Get(long.Parse(item.Value)));
				}
			}

			element = root.Element("SelectedDisfigurements");
			if (element != null)
			{
				foreach (var item in element.Elements())
				{
					var bodypartId = long.Parse(item.Attribute("bodypart").Value);
					SelectedDisfigurements.Add((
						Gameworld.DisfigurementTemplates.Get(long.Parse(item.Attribute("id").Value)),
						SelectedRace.BaseBody.AllExternalBodyparts.First(x => x.Id == bodypartId)));
				}
			}

			element = root.Element("SelectedProstheses");
			if (element != null)
			{
				foreach (var item in element.Elements())
				{
					SelectedProstheses.Add(Gameworld.ItemProtos.Get(long.Parse(item.Value)));
				}
			}
		}

		RecalculateCurrentCosts();
	}

	private void Save()
	{
		using (new FMDB())
		{
			MudSharp.Models.Chargen dbitem;
			if (Id == 0)
			{
				dbitem = new Models.Chargen();
				FMDB.Context.Chargens.Add(dbitem);
			}
			else
			{
				dbitem = FMDB.Context.Chargens.Find(Id);
			}

			dbitem.AccountId = Account.Id;
			dbitem.Status =
				(int)
				(State == ChargenState.Submitted
					? CharacterStatus.Submitted
					: CharacterStatus.Creating);
			dbitem.Name = SelectedName != null ? SelectedName.GetName(NameStyle.GivenOnly) : "Unnamed";
			if (State == ChargenState.Submitted)
			{
				dbitem.MinimumApprovalAuthority = (int)SelectedRoles.Select(x => x.MinimumPermissionToApprove)
				                                                    .DefaultIfEmpty(PermissionLevel.Guide).Max();
				dbitem.SubmitTime = DateTime.UtcNow;
			}

			dbitem.Definition = SaveDefinition();

			FMDB.Context.SaveChanges();
			Id = dbitem.Id;
		}
	}

	private string SaveDefinition()
	{
		return new XElement("Definition", new XElement("CurrentStage", (int)Stage), new XElement("CompletedStages",
				from stage in
					CompletedStages
				select new
					XElement(
						"Stage",
						(int)
						stage)
			), new XElement("SelectedSdesc", new XCData(SelectedSdesc ?? string.Empty)),
			new XElement("SelectedFullDesc", new XCData(SelectedFullDesc ?? string.Empty)),
			new XElement("SelectedRace", SelectedRace?.Id ?? -1),
			new XElement("SelectedCulture", SelectedCulture?.Id ?? -1),
			new XElement("SelectedEthnicity", SelectedEthnicity?.Id ?? -1),
			new XElement("SelectedName",
				SelectedName != null ? SelectedName.SaveToXml() : new XElement("NotSet")),
			new XElement("SelectedBirthday",
				SelectedBirthday != null ? SelectedBirthday.GetDateString() : ""),
			new XElement("SelectedGender", (int)SelectedGender),
			new XElement("SelectedHeight", SelectedHeight),
			new XElement("SelectedWeight", SelectedWeight),
			new XElement("Handedness", (int)Handedness), new XElement(
				"SelectedEntityDescriptionPatterns",
				from pattern in SelectedEntityDescriptionPatterns
				select new XElement("Pattern", pattern.Id)
			), new XElement("SelectedAttributes",
				from attribute in SelectedAttributes
				select
					new XElement("Attribute",
						new XAttribute("Id", attribute.Definition.Id),
						new XAttribute("Value", attribute.Value))
			), new XElement("SelectedSkills",
				from skill in SelectedSkills select new XElement("Skill", skill.Id)
			), new XElement("SelectedBoosts",
				from boost in SelectedSkillBoosts
				select new XElement("Boost",
					new XAttribute("amount", boost.Value),
					boost.Key.Id)
			),
			new XElement("SkillValues",
				from value in SkillValues
				select
					new XElement("Skill", new XAttribute("ID", value.Item1.Id),
						new XAttribute("Value", value.Item2))
			), new XElement("SelectedAccents",
				from accent in SelectedAccents select new XElement("Accent", accent.Id)
			), new XElement("SelectedNotes",
				from note in SelectedNotes
				select new XElement("Note", new XAttribute("Name", note.Item1),
					new XCData(note.Item2))
			), new XElement("SelectedCharacteristics",
				from characteristic in SelectedCharacteristics
				select
					new XElement("Characteristic",
						new XAttribute("Definition", characteristic.Item1.Id),
						new XAttribute("Value", characteristic.Item2.Id))
			),
			new XElement("Rejections",
				from rejection in PriorRejections
				select
					new XElement("Rejection", new XAttribute("Rejecter", rejection.Item1),
						new XCData(rejection.Item2))
			), new XElement("SelectedRoles",
				from role in SelectedRoles select new XElement("Role", role.Id)
			), new XElement("SelectedStartingLocation", StartingLocation?.Role.Id ?? 0),
			new XElement("SpecialApplication", IsSpecialApplication),
			new XElement("ApplicationType", (int)ApplicationType),
			new XElement("SelectedMerits",
				from merit in SelectedMerits select new XElement("Merit", merit.Id)),
			new XElement("MissingBodyparts",
				from part in MissingBodyparts select new XElement("Bodypart", part.Id)),
			new XElement("SelectedDisfigurements",
				from disfigurement in SelectedDisfigurements
				select new XElement("Disfigurement", new XAttribute("id", disfigurement.Disfigurement.Id),
					new XAttribute("bodypart", disfigurement.Bodypart.Id))),
			new XElement("SelectedProstheses",
				from prosthetic in SelectedProstheses select new XElement("Prosthetic", prosthetic.Id)),
			new XElement("SelectedKnowledges",
				from item in SelectedKnowledges select new XElement("Knowledge", item.Id))
		).ToString();
	}

	#region IHaveMerits Members

	public IEnumerable<IMerit> Merits => SelectedMerits;

	public bool AddMerit(IMerit merit)
	{
		return false;
	}

	public bool RemoveMerit(IMerit merit)
	{
		return false;
	}

	#endregion

	#region IHaveTraits Members
	public IEnumerable<ITrait> Traits => SelectedAttributes.Concat(SkillValues.Select(x => new TemporaryTrait { Definition = x.Item1, Owner = this, Value = x.Item2 }));
	public double TraitValue(ITraitDefinition trait, TraitBonusContext context = TraitBonusContext.None)
	{
		var value = GetTrait(trait);
		if (value is null)
		{
			return 0.0;
		}

		return value.Value;
	}

	public double TraitRawValue(ITraitDefinition trait) => TraitValue(trait);
	public double TraitMaxValue(ITraitDefinition trait)
	{
		if (trait is ISkillDefinition sd)
		{
			return sd.Cap.Evaluate(this);
		}

		return trait.MaxValue;
	}

	public double TraitMaxValue(ITrait trait)
	{
		if (trait.Definition is ISkillDefinition sd)
		{
			return sd.Cap.Evaluate(this);
		}

		return trait.MaxValue;
	}

	public bool HasTrait(ITraitDefinition trait) => Traits.Any(x => x.Definition == trait);

	public ITrait GetTrait(ITraitDefinition definition)
	{
		if (definition is ISkillDefinition)
		{
			if (!SelectedSkills.Contains(definition))
			{
				return null;
			}

			return new TemporaryTrait { Definition = definition, Owner = this, Value = SkillValues.First(x => x.Item1 == definition).Item2 };
		}

		return SelectedAttributes.FirstOrDefault(x => x.Definition == definition);
	}
	public string GetTraitDecorated(ITraitDefinition trait) => trait.Decorator.Decorate(GetTrait(trait));
	public IEnumerable<ITrait> TraitsOfType(TraitType type) => Traits.Where(x => x.Definition.TraitType == type);

	public bool AddTrait(ITraitDefinition trait, double value)
	{
		if (trait is ISkillDefinition)
		{
			if (SelectedSkills.Contains(trait))
			{
				return false;
			}

			SelectedSkills.Add(trait);
			SkillValues.Add(Tuple.Create(trait, value));
			return true;
		}
		if (SelectedAttributes.Any(x => x.Definition == trait))
		{
			return false;
		}

		SelectedAttributes.Add(new TemporaryTrait { Definition = trait, Owner = this, Value = value });
		return true;
	}

	public bool RemoveTrait(ITraitDefinition trait)
	{
		if (trait is ISkillDefinition)
		{
			if (!SelectedSkills.Contains(trait))
			{
				return false;
			}

			SelectedSkills.Remove(trait);
			SkillValues.RemoveAll(x => x.Item1 == trait);
			return true;
		}

		if (!SelectedAttributes.Any(x => x.Definition == trait))
		{
			return false;
		}

		SelectedAttributes.RemoveAll(x => x.Definition == trait);
		return true;
	}

	public bool SetTraitValue(ITraitDefinition trait, double value)
	{
		if (trait is ISkillDefinition)
		{
			if (!SelectedSkills.Contains(trait))
			{
				SelectedSkills.Add(trait);
			}

			SkillValues.RemoveAll(x => x.Item1 == trait);
			SkillValues.Add(Tuple.Create(trait, value));
			return true;
		}

		SelectedAttributes.RemoveAll(x => x.Definition == trait);
		SelectedAttributes.Add(new TemporaryTrait { Definition = trait, Owner = this, Value = value });
		return true;
	}
	#endregion
}