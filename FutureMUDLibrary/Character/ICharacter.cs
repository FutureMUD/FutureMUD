using System;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Combat;
using MudSharp.Commands.Trees;
using MudSharp.Communication.Language;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Editor;
using MudSharp.Effects;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.Magic;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;
using MudSharp.Work.Projects;

namespace MudSharp.Character {
	[Flags]
	public enum CharacterState {
		Irrelevant = 0x0,
		Awake = 0x1,
		Stasis = 0x2,
		/// <summary>
		///     Deprecated
		/// </summary>
		Resting = 0x4,
		Sleeping = 0x8,
		Unconscious = 0x10,
		Paralysed = 0x20,
		Dead = 0x40,
		SleepingOrBetter = Sleeping | Awake,                        //This flag state has to be checked via State.SleepingOrBetter.HasFlag(<state to check>)
		UnconsciousOrBetter = Unconscious | Sleeping | Awake,       //This flag state has to be checked via State.UnconsciousOrBetter.HasFlag(<state to check>)
		Conscious = ~(Sleeping | Unconscious | Dead),               //This flag state has to be checked via State.Conscious.HasFlag(<state to check>)
		Able = Conscious & ~Paralysed,                              //This flag state has to be checked via State.Able.HasFlag(<state to check>)
		Unable = ~Able,                                             //This flag state has to be checked via State.Unable.HasFlag(<state to check>)
		Mobile = Awake & Able,                                      //This flag state has to be checked via State.Mobile.HasFlag(<state to check>)
		Quittable = ~(Unconscious | Dead | Paralysed),
		Any = int.MaxValue
	}

	public interface ICollect {
		IEnumerable<IGameItem> Inventory { get; }

		/// <summary>
		///     Contains all IGameItems in the inventory or vicinity ("context") of the ICollect.
		/// </summary>
		IEnumerable<IGameItem> ContextualItems { get; }

		/// <summary>
		/// Contains all IGameItems in the inventory and vicinity of the ICollect, plus items removable from containers with those
		/// </summary>
		IEnumerable<IGameItem> DeepContextualItems { get; }

		void DisplayInventory();
	}

	public interface IHavePhysicalDimensions : IHaveWeight, IHaveHeight {
	}

	public interface IHaveWeight {
		/// <summary>
		///     The weight of the thing, in base units
		/// </summary>
		double Weight { get; set; }
	}

	public interface IHaveHeight {
		/// <summary>
		///     The height of the thing, in base units
		/// </summary>
		double Height { get; set; }
	}

	public interface IHaveABody : IHavePhysicalDimensions {
		IBody Body { get; }
		Alignment Handedness { get; }
	}

	public interface ICommunicate {
		void Emote(string emote, bool permitSpeech = true, OutputFlags additionalConditions = OutputFlags.Normal);
		void Say(IPerceivable target, string message, IEmote emote = null);
		void Talk(IPerceivable target, string message, IEmote emote = null);
		void Whisper(IPerceivable target, string message, IEmote emote = null);
		void Shout(IPerceivable target, string message, IEmote emote = null);
		void LoudSay(IPerceivable target, string message, IEmote emote = null);
		void Yell(IPerceivable target, string message, IEmote emote = null);
		void Sing(IPerceivable target, string message, IEmote emote = null);
		void Transmit(IGameItem target, string message, IEmote emote = null);
	}

	public interface IPerformSurgery {
		string PreferredSurgicalSchool { get; set; }
	}

	public interface ISentient : IHaveABody {
		bool Think(string thought, IEmote emote = null);
		bool Feel(string feeling);
	}

	public interface IBuilder {
		T EditingItem<T>() where T : class;
		void SetEditingItem<T>(T? item) where T : class;
	}

	public interface IEat {
		bool Eat(IEdible edible, IContainer container, ITable table, double bites, IEmote playerEmote);
		bool SilentEat(IEdible edible, double bites);
		bool CanEat(IEdible edible, IContainer container, ITable table, double bites);
		bool Drink(ILiquidContainer container, ITable table, double quantity, IEmote playerEmote);
		bool SilentDrink(ILiquidContainer container, double amount);
		bool CanDrink(ILiquidContainer container, ITable table, double quantity);
		bool Swallow(ISwallowable swallowable, IContainer container, ITable table, IEmote playerEmote);
		bool SilentSwallow(ISwallowable swallowable);
		bool CanSwallow(ISwallowable swallowable, IContainer container, ITable table);
		(bool Success, string ErrorMessage) CanEat(ICorpse corpse, double bites);
		(bool Success, string ErrorMessage) CanEat(ISeveredBodypart bodypart, double bites);
		(bool Success, string ErrorMessage) CanEat(string foragableYield, double bites);
		(bool Success, string ErrorMessage) Eat(ICorpse corpse, double bites, IEmote playerEmote);
		(bool Success, string ErrorMessage) Eat(ISeveredBodypart bodypart, double bites, IEmote playerEmote);
		(bool Success, string ErrorMessage) Eat(string foragableYield, double bites, IEmote playerEmote);
	}

	public interface IMortal {
		void CheckHealthStatus();
		IGameItem Die();
		ICharacter Resurrect(ICell location);
		event PerceivableEvent OnDeath;
	}

	public interface IBreathe {
		bool NeedsToBreathe { get; }
		bool IsBreathing { get; }
		bool CanBreathe { get; }
		/// <summary>
		/// Volume (at sea level) of breathable gases consumed per breath
		/// </summary>
		TimeSpan HeldBreathTime { get; set; }
		double HeldBreathPercentage { get; }
		IFluid BreathingFluid { get; }
		IBreathingStrategy BreathingStrategy { get; }
	}

	public interface ISleep {
		void Awaken(IEmote emote = null);
		void Sleep(IEmote emote = null);
	}

	public interface IUseTools {
		Difficulty GetDifficultyForTool(IGameItem tool, Difficulty baseDifficulty);
	}

	public interface ICharacterMover : IMove
	{
		bool CanMovePosition(IPositionState whichPosition, PositionModifier whichModifier, IPerceivable target,
			bool ignorePositionTargetChangeRestrictions = false, bool ignoreMovement  = false);
		string WhyCannotMovePosition(IPositionState whichPosition, PositionModifier whichModifier,
			IPerceivable target, bool ignorePositionTargetChangeRestrictions = false, bool ignoreMovement = false);
		IPositionState MostUprightMobilePosition(bool ignoreCouldMove = false);

		bool CanMovePosition(IPositionState whichPosition, bool ignoreMovement = false);
		string WhyCannotMovePosition(IPositionState whichPosition, bool ignoreMovement = false);

		/// <summary>
		///     Handles both a call to MovePosition and specifically takes an unparsed emote string for messaging purposes
		/// </summary>
		/// <param name="whichPosition"></param>
		/// <param name="whichModifier"></param>
		/// <param name="target"></param>
		/// <param name="unparsedEmote"></param>
		void MovePosition(IPositionState whichPosition, PositionModifier whichModifier, IPositionable target,
			string unparsedEmote);

		/// <summary>
		///     Instructs the IBody to move to the new position with modifiers and target.
		/// </summary>
		/// <param name="whichPosition">The position to adopt</param>
		/// <param name="whichModifier">The desired modifier of the new position</param>
		/// <param name="target">The desired target of the new position</param>
		/// <param name="playerEmote"></param>
		/// <param name="playerPmote"></param>
		/// <param name="ignorePositionTargetChangeRestrictions"></param>
		void MovePosition(IPositionState whichPosition, PositionModifier whichModifier, IPerceivable target,
			IEmote playerEmote, IEmote playerPmote, bool ignorePositionTargetChangeRestrictions = false, bool ignoreMovement = false);

		/// <summary>
		///     Instructs the IBody to move to the new position, respecting existing targets and modifiers.
		/// </summary>
		/// <param name="whichPosition">The position to adopt</param>
		/// <param name="playerEmote"></param>
		/// <param name="playerPmote"></param>
		void MovePosition(IPositionState whichPosition, IEmote playerEmote, IEmote playerPmote);

		void ResetPositionTarget(IEmote playerEmote, IEmote playerPmote);

		Difficulty ArmourUseCheckDifficulty(ArmourUseDifficultyContext context);
	}

	public enum ArmourUseDifficultyContext
	{
		General,
		Combat,
		AidedWalking,
		Climbing,
		Swimming,
		LobItem,
		Flying
	}

	public interface IHaveOutfits
	{
		IEnumerable<IOutfit> Outfits { get; }
		void AddOutfit(IOutfit outfit);
		void RemoveOutfit(IOutfit outfit);
		bool OutfitsChanged { get; set; }
	}

	public interface ISwim
	{
		TimeSpan DiveDelay { get; }
		double SwimSpeedMultiplier { get; }

		/// <summary>
		/// Checks whether the character can swim up a layer, and echoes if they fail as well as all consequences (stamina etc)
		/// </summary>
		/// <returns></returns>
		(bool Truth, TimeSpan Delay) DoSurfaceCheck();

		/// <summary>
		/// Checks whether a character sinks into the water. Called when in PositionSwimming on Stamina tick. Handles all echoes and consequences.
		/// </summary>
		void DoSwimHeartbeat();

		double SwimStaminaCost();

		(bool Truth, string Error) CanAscend();
		(bool Truth, string Error) CanDive();
		void Ascend(IEmote actionEmote = null);
		void Dive(IEmote actionEmote = null);
	}

	public interface IFly
	{
		void DoFlyHeartbeat();
		double FlyStaminaCost();
		(bool Truth, string Error) CanFly();
		void Fly(IEmote actionEmote = null);
		(bool Truth, string Error) CanAscend();
		(bool Truth, string Error) CanDive();
		void Ascend(IEmote actionEmote = null);
		void Dive(IEmote actionEmote = null);
		(bool Truth, string Error) CanLand();
		void Land(IEmote actionEmote = null);
		void CheckCanFly();
	}

	public interface IClimb
	{
		TimeSpan ClimbDelay { get; }
		(bool Truth, string Error) CanClimbUp();
		(bool Truth, string Error) CanClimbDown();
		void ClimbUp(IEmote actionEmote = null);
		void ClimbDown(IEmote actionEmote = null);

		/// <summary>
		/// Called when the climber is actually trying to move. Takes stamina, sends echoes, checks, can potentially fall. If false, climber didn't progress.
		/// </summary>
		/// <param name="difficulty"></param>
		/// <returns>True if the climber should progress, false if they couldn't progress or fell</returns>
		bool DoClimbMovementCheck(Difficulty difficulty);
	}

	public interface ICanBeEmployed
	{
		IEnumerable<IActiveJob> ActiveJobs { get; }
		void AddJob(IActiveJob job);
		void RemoveJob(IActiveJob job);
	}

	public interface ICharacter : IArbitrarilyControllable, ISubContextController, IHaveAuthority,
		ISentient, ICollect, IEditableNameData, IBuilder, ICharacterMover, ILanguagePerceiver,
		IHaveCulture, IHaveRace, IPerceivableHaveCharacteristics, IHaveCurrency, IHaveCommunity, IHaveRoles, IHaveNeeds,
		IEat, IHaveStamina, IHaveKnowledges, IEquatable<ICharacter>, IPerformSurgery, IBreathe, IHaveContextualSizeCategory, 
		IHaveAllies, IUseTools, IStyleCharacterCharacteristics, ISleep, IMagicUser, IHaveMagicResource, IHavePositionalSizes, 
		IHaveOutfits, ISwim, IFly, IClimb, IHavePersonalProjects, ITarget, ICanBeEmployed, IMountable
	{
		ICharacterController CharacterController { get; }
		bool TryToDetermineIdentity(ICharacter observer);
		ICorpse Corpse { get; set; }
		bool IsGuest { get; }
		CharacterState State { get; set; }
		CharacterStatus Status { get; }
		ICharacterCommandTree CommandTree { get; }

		bool IsHelpless { get; }

		MudDate Birthday { get; set; }

		int TotalMinutesPlayed { get; }
		DateTime? PreviousLoginDateTime { get; set; }
		DateTime? LastLogoutDateTime { get; set; }
		DateTime LoginDateTime { get; set; }
		DateTime LastMinutesUpdate { get; set; }
		void SaveMinutes(MudSharp.Models.Character dbchar);

		bool IsPlayerCharacter { get; }
		string LongTermPlan { get; set; }
		string ShortTermPlan { get; set; }
		/// <summary>
		/// A manually and voluntarily set degree by which all difficulties are staged up, to go easy on someone
		/// </summary>
		int CombatBurdenOffense { get; set; }
		int CombatBurdenDefense { get; set; }
		double TrackingAbilityVisual { get; }
		double TrackingAbilityOlfactory { get; }
		bool BriefRoomDescs { get; set; }
		event PerceivableEvent OnStateChanged;
		bool IsAdministrator(PermissionLevel level = PermissionLevel.JuniorAdmin);
		void ChangePermissionLevel(PermissionLevel newLevel);

		double MaximumDragWeight { get; }

		void TransferTo(ICell target, RoomLayer layer);

		/// <summary>
		/// This function should be the preferred way of teleporting a character from one cell to another, handling all the consequences
		/// </summary>
		/// <param name="target">The target location</param>
		/// <param name="layer">The target layer</param>
		/// <param name="includeFollowers">Include dragging targets</param>
		/// <param name="echo">Echo everyone entering or leaving (if you handle the echo elsewhere, use false)</param>
		/// <param name="playerEchoLeave"></param>
		/// <param name="playerEchoArrive"></param>
		/// <param name="playerEchoSelf"></param>
		/// <param name="followerEchoLeave"></param>
		/// <param name="followerEchoArrive"></param>
		/// <param name="followerEchoSelf"></param>
		void Teleport(
			ICell target, 
			RoomLayer layer, 
			bool includeFollowers, 
			bool echo, 
			string playerEchoLeave = "@ leaves the area.", 
			string playerEchoArrive = "@ enters the area.", 
			string playerEchoSelf = "", 
			string followerEchoLeave = "@ leaves the area.", 
			string followerEchoArrive = "@ enters the area.", 
			string followerEchoSelf = "");

		void EditorMode(Action<string, IOutputHandler, object[]> postAction, Action<IOutputHandler, object[]> cancelAction, double characterLengthMultiplier = 1.0, string recallText = null, EditorOptions options = EditorOptions.None, object[] suppliedArguments = null);
		void EditorModeMulti(Action<IEnumerable<string>, IOutputHandler, object[]> postAction, Action<IOutputHandler, object[]> cancelAction, IEnumerable<string> editorTexts, double characterLengthMultiplier = 1.0, string recallText = null, EditorOptions options = EditorOptions.None, object[] suppliedArguments = null);

		string GetConsiderString(IPerceiver voyeur);
		
		bool Quit(bool silent = false);
		string ShowScore(IPerceiver voyeur);
		string ShowStat(IPerceiver voyeur);
		string ShowHealth(IPerceiver voyeur);
		bool Stop(bool force);

		string DebugInfo();
		void SetNoControllerTags(string text);
		void SetGender(Gender gender);

		ICharacterTemplate GetCharacterTemplate();

		void LoginCharacter();

		IEnumerable<T> CombinedEffectsOfType<T>() where T : class, IEffect;

		IEnumerable<IMortalPerceiver> SeenTargets { get; }
		void SeeTarget(IMortalPerceiver target);
		void LoseTarget(IMortalPerceiver target);
		void CheckTarget(IMortalPerceiver target);

		uint MaximumPerceptionRange { get; }
		bool WillingToPermitInventoryManipulation(ICharacter manipulator);
		bool WillingToPermitMedicalIntervention(ICharacter medic);
		bool UnableToResistInterventions(ICharacter intervenor);
		(bool Truth, string Message) CanManipulateItem(IGameItem item);

		(bool Truth, string Message) IsBlocked(params string[] blocks);

		void ConvertGrapplesToDrags();
		bool NoMercy { get; set; }
		IEnumerable<INameCulture> NameCultures { get; }
		INameCulture NameCultureForGender(Gender gender);
		Difficulty IlluminationSightDifficulty();
#nullable enable
		ICharacter? RidingMount { get; set; }
#nullable restore

		void DoCombatKnockdown();
		void DoFallOffHorse();
	}

	public interface IHavePersonalProjects
	{
		IEnumerable<IPersonalProject> PersonalProjects { get; }
		(IActiveProject Project, IProjectLabourRequirement Labour) CurrentProject { get; set; }
		double CurrentProjectHours { get; set; }
		void AddPersonalProject(IActiveProject project);
		void RemovePersonalProject(IActiveProject project);
	}
}