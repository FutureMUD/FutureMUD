using MudSharp.Framework;
using System;
using System.Linq;

namespace MudSharp.RPG.Checks {
	public enum CheckType {
		None = 0, // Default
		ExactTimeCheck = 1, // Can tell the time exactly in the Time command
		VagueTimeCheck = 2, // Can tell the time vaguely in the Time command
		GenericAttributeCheck = 3, // Such as used by the ROLL vs Strength command
		GenericSkillCheck = 4, // such as used by the Roll vs swords command
		GenericListenCheck = 5, // such as when hearing a miscellaneous noise
		LanguageListenCheck = 6, // hearing a fragment of spoken language
		SpokenLanguageSpeakCheck = 7, // speaking a spoken language
		SpokenLanguageHearCheck = 8, // hearing and understanding a spoken language
		AccentAcquireCheck = 9, // acquiring an accent that is not possessed by the individual
		AccentImproveCheck = 10, // improving an accent the individual already has
		GenericSpotCheck = 11, // such as used by the Echo -v command, or hemote
		TraitBranchCheck = 12, // Called when a check is TraitCanBranchIfMissing
		NoticeCheck = 13, // Called when EmoteOutput is sent with the NoticeCheckRequired flag
		HideCheck = 14, // Called when someone has used the HideInvis affect
		SneakCheck = 15, // Called when someone is sneaking
		PalmCheck = 16, // Called when someone uses the palm command
		SpotStealthCheck = 17, // Called to spot someone using hide or palm
		HideItemCheck = 18, // Called when someone uses the HIDE command on an item
		UninstallDoorCheck = 19, // Called when a player uses the UNINSTALL command on a door
		SpotSneakCheck = 20, // Called to determine whether a SawSneaker effect is added
		SkillTeachCheck = 21,
		SkillLearnCheck = 22,
		KnowledgeTeachCheck = 23,
		KnowledgeLearnCheck = 24,
		ForageCheck = 25,
		ForageSpecificCheck = 26,
		ForageTimeCheck = 27,
		HealingCheck = 28, // Called in HealingTick to recover HP
		StunRecoveryCheck = 29, // Called in HealingTick to recover Stun
		ShockRecoveryCheck = 30, // Called in HealingTick to recover Shock
		PainRecoveryCheck = 31, // Called in HealingTick to recover Pain
		WoundCloseCheck = 32, // Called in HealingTick for Trauma wounds to close of their own accord
		BindWoundCheck = 33,
		SutureWoundCheck = 34,
		CleanWoundCheck = 35,
		RemoveLodgedObjectCheck = 36,
		MendCheck = 37,
		TreatmentItemRecognitionCheck = 38,
		MeleeWeaponPenetrateCheck = 39,
		RangedWeaponPenetrateCheck = 40,
		PenetrationDefenseCheck = 41,
		CombatMoveCheck = 42,
		DreamCheck = 43,
		GoToSleepCheck = 44, // This check is used when first going to sleep to determine how soon someone can wake up
		CombatRecoveryCheck = 45, // This check is used to determine how much time before your next action in combat
		MedicalExaminationCheck = 46, // MedicalExaminationProcedure Surgical Procedure
		LocksmithingCheck = 47,
		NaturalWeaponAttack = 48, // Unarmed combat attacks
		DodgeCheck = 49,
		ParryCheck = 50,
		BlockCheck = 51,
		FleeMeleeCheck = 52, // Called in FleeMove to determine whether a character can escape
		OpposeFleeMeleeCheck = 53, // Called in FleeMove by opponents of someone fleeing
		Ward = 54, // Called when WardDefenseMove chosen as defense
		WardDefense = 55, // Called to oppose WardDefenseMove defense type by attacker
		WardIgnore = 56, // Called when beaten in a ward check to summon the nerve to ignore it and attack anyway
		StartClinch = 57,
		ResistClinch = 58,
		BreakClinch = 59,
		ResistBreakClinch = 60,
		MeleeWeaponCheck = 61, // use a weapon skill
		ExploratorySurgeryCheck = 62, // ExploratorySurgery Surgical Procedure
		TriageCheck = 63, // TriageProcedure Surgical Procedure
		AmputationCheck = 64, // AmputationProcedure Surgical Procedure
		ReplantationCheck = 65, // ReplantationProcedure Surgical Procedure
		InfectionHeartbeat = 66,
		InfectionSpread = 67,
		InvasiveProcedureFinalisation = 68,
		ReplantedBodypartRejectionCheck = 69,
		TraumaControlSurgery = 70,
		ThrownWeaponCheck = 71,
		AimRangedWeapon = 72,
		ScanPerceptionCheck = 73,
		RescueCheck = 74,
		OpposeRescueCheck = 75,
		Defibrillate = 76,
		PerformCPR = 77,
		ArmourUseCheck = 78,
		ReadTextImprovementCheck = 79,
		HandwritingImprovementCheck = 80,
		StaggeringBlowDefense = 82,
		FireBow = 83,
		FireCrossbow = 84,
		FireFirearm = 85,
		FireSling = 86,
		StruggleFreeFromDrag = 87,
		OpposeStruggleFreeFromDrag = 88,
		CounterGrappleCheck = 89,
		StruggleFreeFromGrapple = 90,
		OpposeStruggleFreeFromGrapple = 91,
		ExtendGrappleCheck = 92,
		InitiateGrapple = 93,
		StyleCharacteristicCapabilityCheck = 94,
		StyleCharacteristicCheck = 95,
		ScreechAttack = 96,
		CrutchWalking = 97,
		OrganExtractionCheck = 98,
		OrganTransplantCheck = 99,
		ProgSkillUseCheck = 100,
		CannulationProcedure = 101,
		DecannulationProcedure = 102,
		KeepAimTargetMoved = 103,
		QuickscanPerceptionCheck = 104,
		LongscanPerceptionCheck = 105,
		StrangleCheck = 106,
		WrenchAttackCheck = 107,
		OrganStabilisationCheck = 108,
		CraftOutcomeCheck = 109, // Used to determine whether the craft is a pass or fail
		CraftQualityCheck = 110, // Used to determine crafter effect on quality
		TendWoundCheck = 111,
		RelocateBoneCheck = 112,
		SurgicalSetCheck = 113,
		RepairItemCheck = 114,
		InstallImplantSurgery = 115,
		RemoveImplantSurgery = 116,
		ConfigureImplantPowerSurgery = 117,
		ImplantRecognitionCheck = 118,
		ButcheryCheck = 119,
		SkinningCheck = 120,
		TossItemCheck = 121,
		WatchLocation = 122,
		MagicConcentrationOnWounded = 123,
		ConnectMindPower = 124,
		PsychicLanguageHearCheck = 125,
		MindSayPower = 126,
		MindBroadcastPower = 127,
		MagicTelepathyCheck = 128,
		MindLookPower = 129,
		PassiveStealthCheck = 130, // Only used INTERNALLY in a PassivePerceptionCheck. Do not use elsewhere.
		ActiveSearchCheck = 131,
		InvisibilityPower = 132,
		MagicArmourPower = 133,
		ClimbCheck = 134,
		ConfigureImplantInterfaceSurgery = 135,
		ProjectLabourCheck = 136,
		InkTattooCheck = 137,
		FallingImpactCheck = 138,
		MagicChokePower = 139,
		ResistMagicChokePower = 140,
		MagicAnesthesiaPower = 141,
		ResistMagicAnesthesiaPower = 142,
		SwimmingCheck = 143,
		AvoidFallDueToWind = 144,
		SwimStayAfloatCheck = 145,
		FlyCheck = 146,
		CheatAtDiceCheck = 147,
		EvaluateDiceFairnessCheck = 148,
		SpillLiquidOnPerson = 149,
		DodgeSpillLiquidOnPerson = 150,
		ProjectSkillUseAction = 151,
		DrawingImprovementCheck = 152,
		MagicSensePower = 153,
		TakedownCheck = 154,
		BreakoutCheck = 155,
		OpposeBreakoutCheck = 156,
		CastSpellCheck = 157,
		ResistMagicSpellCheck = 158,
		MindBarrierPowerCheck = 159,
		MindExpelPower = 160,
		MindAuditPower = 161,
		AppraiseItemCheck = 162,
		AuxiliaryMoveCheck = 163,
		WritingComprehendCheck = 164,
		ProsecuteLegalCase = 165,
		DefendLegalCase = 166,
		ClimbTreetoTreeCheck = 167,
		CheatAtCoinFlip = 168
	}

	public enum FailIfTraitMissingType {
		DoNotFail = 0,
		FailIfAnyMissing = 1,
		FailIfAllMissing = 2
	}

	public static class CheckExtensions {
		public static bool IsNonStaticCheck(this CheckType type) {
			switch (type) {
				case CheckType.ExactTimeCheck:
				case CheckType.VagueTimeCheck:
				case CheckType.AccentAcquireCheck:
				case CheckType.AccentImproveCheck:
				case CheckType.TraitBranchCheck:
				case CheckType.StyleCharacteristicCapabilityCheck:
					return false;
				default:
					return true;
			}
		}

		public static bool IsPhysicalActivityCheck(this CheckType type)
		{
			switch (type)
			{

				case CheckType.HideCheck:				case CheckType.SneakCheck:				case CheckType.PalmCheck:				case CheckType.HideItemCheck:				case CheckType.UninstallDoorCheck:				case CheckType.ForageCheck:				case CheckType.ForageSpecificCheck:				case CheckType.ForageTimeCheck:				case CheckType.WoundCloseCheck:				case CheckType.BindWoundCheck:				case CheckType.SutureWoundCheck:				case CheckType.CleanWoundCheck:				case CheckType.RemoveLodgedObjectCheck:				case CheckType.MendCheck:				case CheckType.MeleeWeaponPenetrateCheck:				case CheckType.RangedWeaponPenetrateCheck:				case CheckType.PenetrationDefenseCheck:				case CheckType.CombatMoveCheck:				case CheckType.CombatRecoveryCheck:				case CheckType.MedicalExaminationCheck:				case CheckType.LocksmithingCheck:				case CheckType.NaturalWeaponAttack:				case CheckType.DodgeCheck:				case CheckType.ParryCheck:				case CheckType.BlockCheck:				case CheckType.FleeMeleeCheck:				case CheckType.OpposeFleeMeleeCheck:				case CheckType.Ward:				case CheckType.WardDefense:				case CheckType.StartClinch:				case CheckType.ResistClinch:				case CheckType.BreakClinch:				case CheckType.ResistBreakClinch:				case CheckType.MeleeWeaponCheck:				case CheckType.ExploratorySurgeryCheck:				case CheckType.TriageCheck:				case CheckType.AmputationCheck:				case CheckType.ReplantationCheck:				case CheckType.InvasiveProcedureFinalisation:				case CheckType.TraumaControlSurgery:				case CheckType.ThrownWeaponCheck:				case CheckType.AimRangedWeapon:				case CheckType.RescueCheck:				case CheckType.OpposeRescueCheck:				case CheckType.Defibrillate:				case CheckType.PerformCPR:				case CheckType.StaggeringBlowDefense:				case CheckType.FireBow:				case CheckType.FireCrossbow:				case CheckType.FireFirearm:				case CheckType.FireSling:				case CheckType.StruggleFreeFromDrag:				case CheckType.OpposeStruggleFreeFromDrag:				case CheckType.CounterGrappleCheck:				case CheckType.StruggleFreeFromGrapple:				case CheckType.OpposeStruggleFreeFromGrapple:				case CheckType.ExtendGrappleCheck:				case CheckType.InitiateGrapple:				case CheckType.StyleCharacteristicCheck:				case CheckType.CrutchWalking:				case CheckType.OrganExtractionCheck:				case CheckType.OrganTransplantCheck:				case CheckType.CannulationProcedure:				case CheckType.DecannulationProcedure:				case CheckType.StrangleCheck:				case CheckType.WrenchAttackCheck:				case CheckType.OrganStabilisationCheck:				case CheckType.CraftOutcomeCheck:				case CheckType.CraftQualityCheck:				case CheckType.TendWoundCheck:				case CheckType.RelocateBoneCheck:				case CheckType.SurgicalSetCheck:				case CheckType.RepairItemCheck:				case CheckType.InstallImplantSurgery:				case CheckType.RemoveImplantSurgery:				case CheckType.ConfigureImplantPowerSurgery:				case CheckType.ButcheryCheck:				case CheckType.SkinningCheck:				case CheckType.TossItemCheck:				case CheckType.ActiveSearchCheck:				case CheckType.ClimbCheck:				case CheckType.ConfigureImplantInterfaceSurgery:				case CheckType.InkTattooCheck:				case CheckType.FallingImpactCheck:				case CheckType.SwimmingCheck:				case CheckType.AvoidFallDueToWind:				case CheckType.SwimStayAfloatCheck:				case CheckType.FlyCheck:				case CheckType.CheatAtDiceCheck:				case CheckType.SpillLiquidOnPerson:				case CheckType.DodgeSpillLiquidOnPerson:				case CheckType.TakedownCheck:				case CheckType.BreakoutCheck:				case CheckType.OpposeBreakoutCheck:				case CheckType.AuxiliaryMoveCheck:				case CheckType.ClimbTreetoTreeCheck:				case CheckType.CheatAtCoinFlip:
					return true;
			}
			return false;
		}
		
		public static bool IsGeneralActivityCheck(this CheckType type) {
			if (!IsNonStaticCheck(type)) {
				return false;
			}

			switch (type) {
				case CheckType.ReplantedBodypartRejectionCheck:
				case CheckType.RangedWeaponPenetrateCheck:
				case CheckType.PenetrationDefenseCheck:
				case CheckType.PainRecoveryCheck:
				case CheckType.ShockRecoveryCheck:
				case CheckType.StunRecoveryCheck:
				case CheckType.HealingCheck:
				case CheckType.InfectionHeartbeat:
				case CheckType.InfectionSpread:
				case CheckType.GoToSleepCheck:
				case CheckType.DreamCheck:
				case CheckType.WoundCloseCheck:
				case CheckType.StyleCharacteristicCheck:
					return false;
				default:
					return true;
			}
		}

		public static bool IsHealingCheck(this CheckType type) {
			switch (type) {
				case CheckType.PainRecoveryCheck:
				case CheckType.ShockRecoveryCheck:
				case CheckType.StunRecoveryCheck:
				case CheckType.HealingCheck:
				case CheckType.InfectionHeartbeat:
				case CheckType.InfectionSpread:
				case CheckType.WoundCloseCheck:
					return true;
				default:
					return false;
			}
		}

		public static bool IsTargettedFriendlyCheck(this CheckType type) {
			switch (type) {
				case CheckType.BindWoundCheck:
				case CheckType.CleanWoundCheck:
				case CheckType.SutureWoundCheck:
				case CheckType.MedicalExaminationCheck:
				case CheckType.TriageCheck:
				case CheckType.AmputationCheck:
				case CheckType.ReplantationCheck:
				case CheckType.ExploratorySurgeryCheck:
				case CheckType.InvasiveProcedureFinalisation:
				case CheckType.PerformCPR:
				case CheckType.Defibrillate:
				case CheckType.TraumaControlSurgery:
				case CheckType.KnowledgeTeachCheck:
				case CheckType.SkillTeachCheck:
				case CheckType.KnowledgeLearnCheck:
				case CheckType.SkillLearnCheck:
				case CheckType.StyleCharacteristicCheck:
				case CheckType.InkTattooCheck:
				case CheckType.TendWoundCheck:
				case CheckType.CannulationProcedure:
				case CheckType.DecannulationProcedure:
				case CheckType.OrganStabilisationCheck:
				case CheckType.ConfigureImplantInterfaceSurgery:
				case CheckType.ConfigureImplantPowerSurgery:
				case CheckType.RescueCheck:
				case CheckType.DefendLegalCase:
					return true;
				default:
					return false;
			}
		}

		public static bool IsOffensiveCombatAction(this CheckType type)
		{
			switch (type)
			{
				case CheckType.MeleeWeaponCheck:
				case CheckType.MeleeWeaponPenetrateCheck:
				case CheckType.NaturalWeaponAttack:
				case CheckType.RescueCheck:
				case CheckType.OpposeFleeMeleeCheck:
				case CheckType.AimRangedWeapon:
				case CheckType.RangedWeaponPenetrateCheck:
				case CheckType.WardIgnore:
				case CheckType.BreakClinch:
				case CheckType.StartClinch:
				case CheckType.ThrownWeaponCheck:
				case CheckType.ScreechAttack:
				case CheckType.InitiateGrapple:
				case CheckType.ExtendGrappleCheck:
				case CheckType.CounterGrappleCheck:
				case CheckType.StruggleFreeFromGrapple:
				case CheckType.FireBow:
				case CheckType.FireCrossbow:
				case CheckType.FireFirearm:
				case CheckType.FireSling:
				case CheckType.TakedownCheck:
				case CheckType.WrenchAttackCheck:
				case CheckType.AuxiliaryMoveCheck:
					return true;
				default:
					return false;
			}
		}

		public static bool IsDefensiveCombatAction(this CheckType type)
		{
			switch (type)
			{
				case CheckType.OpposeRescueCheck:
				case CheckType.FleeMeleeCheck:
				case CheckType.ParryCheck:
				case CheckType.DodgeCheck:
				case CheckType.BlockCheck:
				case CheckType.Ward:
				case CheckType.WardDefense:
				case CheckType.ResistBreakClinch:
				case CheckType.ResistClinch:
				case CheckType.OpposeStruggleFreeFromGrapple:
					return true;
				default:
					return false;
			}
		}

		public static bool IsTargettedHostileCheck(this CheckType type) {
			switch (type) {
				case CheckType.MeleeWeaponCheck:
				case CheckType.MeleeWeaponPenetrateCheck:
				case CheckType.NaturalWeaponAttack:
				case CheckType.OpposeRescueCheck:
				case CheckType.RescueCheck:
				case CheckType.FleeMeleeCheck:
				case CheckType.OpposeFleeMeleeCheck:
				case CheckType.AimRangedWeapon:
				case CheckType.RangedWeaponPenetrateCheck:
				case CheckType.ParryCheck:
				case CheckType.DodgeCheck:
				case CheckType.BlockCheck:
				case CheckType.Ward:
				case CheckType.WardDefense:
				case CheckType.WardIgnore:
				case CheckType.BreakClinch:
				case CheckType.ResistBreakClinch:
				case CheckType.ResistClinch:
				case CheckType.StartClinch:
				case CheckType.ThrownWeaponCheck:
				case CheckType.ScreechAttack:
				case CheckType.InitiateGrapple:
				case CheckType.ExtendGrappleCheck:
				case CheckType.CounterGrappleCheck:
				case CheckType.OpposeStruggleFreeFromGrapple:
				case CheckType.StruggleFreeFromGrapple:
				case CheckType.MagicChokePower:
				case CheckType.BreakoutCheck:
				case CheckType.OpposeBreakoutCheck:
				case CheckType.FireBow:
				case CheckType.FireCrossbow:
				case CheckType.FireFirearm:
				case CheckType.FireSling:
				case CheckType.TakedownCheck:
				case CheckType.WrenchAttackCheck:
				case CheckType.AuxiliaryMoveCheck:
				case CheckType.ProsecuteLegalCase:
					return true;
				default:
					return false;
			}
		}

		public static bool IsVisionInfluencedCheck(this CheckType type)
		{
			switch (type)
			{
				case CheckType.GenericSpotCheck:
				case CheckType.NoticeCheck:
				case CheckType.HideCheck:
				case CheckType.SpotStealthCheck:
				case CheckType.HideItemCheck:
				case CheckType.UninstallDoorCheck:
				case CheckType.SpotSneakCheck:
				case CheckType.ForageCheck:
				case CheckType.ForageSpecificCheck:
				case CheckType.ForageTimeCheck:
				case CheckType.BindWoundCheck:
				case CheckType.SutureWoundCheck:
				case CheckType.CleanWoundCheck:
				case CheckType.RemoveLodgedObjectCheck:
				case CheckType.TreatmentItemRecognitionCheck:
				case CheckType.MedicalExaminationCheck:
				case CheckType.LocksmithingCheck:
				case CheckType.NaturalWeaponAttack:
				case CheckType.DodgeCheck:
				case CheckType.ParryCheck:
				case CheckType.BlockCheck:
				case CheckType.FleeMeleeCheck:
				case CheckType.OpposeFleeMeleeCheck:
				case CheckType.Ward:
				case CheckType.WardDefense:
				case CheckType.StartClinch:
				case CheckType.ResistClinch:
				case CheckType.BreakClinch:
				case CheckType.ResistBreakClinch:
				case CheckType.MeleeWeaponCheck:
				case CheckType.ExploratorySurgeryCheck:
				case CheckType.TriageCheck:
				case CheckType.AmputationCheck:
				case CheckType.ReplantationCheck:
				case CheckType.InvasiveProcedureFinalisation:
				case CheckType.TraumaControlSurgery:
				case CheckType.ThrownWeaponCheck:
				case CheckType.AimRangedWeapon:
				case CheckType.ScanPerceptionCheck:
				case CheckType.RescueCheck:
				case CheckType.OpposeRescueCheck:
				case CheckType.Defibrillate:
				case CheckType.PerformCPR:
				case CheckType.ReadTextImprovementCheck:
				case CheckType.HandwritingImprovementCheck:
				case CheckType.FireBow:
				case CheckType.FireCrossbow:
				case CheckType.FireFirearm:
				case CheckType.FireSling:
				case CheckType.StyleCharacteristicCheck:
				case CheckType.OrganExtractionCheck:
				case CheckType.OrganTransplantCheck:
				case CheckType.CannulationProcedure:
				case CheckType.DecannulationProcedure:
				case CheckType.KeepAimTargetMoved:
				case CheckType.QuickscanPerceptionCheck:
				case CheckType.LongscanPerceptionCheck:
				case CheckType.OrganStabilisationCheck:
				case CheckType.TendWoundCheck:
				case CheckType.SurgicalSetCheck:
				case CheckType.RepairItemCheck:
				case CheckType.InstallImplantSurgery:
				case CheckType.RemoveImplantSurgery:
				case CheckType.ConfigureImplantPowerSurgery:
				case CheckType.ImplantRecognitionCheck:
				case CheckType.ButcheryCheck:
				case CheckType.SkinningCheck:
				case CheckType.TossItemCheck:
				case CheckType.WatchLocation:
				case CheckType.PassiveStealthCheck:
				case CheckType.ActiveSearchCheck:
				case CheckType.ClimbCheck:
				case CheckType.ConfigureImplantInterfaceSurgery:
				case CheckType.InkTattooCheck:
				case CheckType.SwimmingCheck:
				case CheckType.FlyCheck:                    
				case CheckType.CheatAtDiceCheck:
				case CheckType.EvaluateDiceFairnessCheck:
				case CheckType.SpillLiquidOnPerson:
				case CheckType.DodgeSpillLiquidOnPerson:
				case CheckType.DrawingImprovementCheck:
				case CheckType.ClimbTreetoTreeCheck:
					return true;
			}

			return false;
		}

		public static bool IsPerceptionCheck(this CheckType type) {
			switch (type) {
				case CheckType.GenericListenCheck:
				case CheckType.GenericSpotCheck:
				case CheckType.LanguageListenCheck:
				case CheckType.SpokenLanguageHearCheck:
				case CheckType.SpotSneakCheck:
				case CheckType.SpotStealthCheck:
				case CheckType.NoticeCheck:
				case CheckType.ScanPerceptionCheck:
				case CheckType.QuickscanPerceptionCheck:
				case CheckType.LongscanPerceptionCheck:
				case CheckType.WatchLocation:
				case CheckType.PassiveStealthCheck:
				case CheckType.WritingComprehendCheck:
				case CheckType.ActiveSearchCheck:
				case CheckType.EvaluateDiceFairnessCheck:
				case CheckType.KeepAimTargetMoved:
					return true;
				default:
					return false;
			}
		}

		public static bool IsLanguageCheck(this CheckType type) {
			switch (type) {
				case CheckType.LanguageListenCheck:
				case CheckType.SpokenLanguageHearCheck:
				case CheckType.SpokenLanguageSpeakCheck:
				case CheckType.WritingComprehendCheck:
				case CheckType.PsychicLanguageHearCheck:
				case CheckType.AccentAcquireCheck:
				case CheckType.AccentImproveCheck:
					return true;
				default:
					return false;
			}
		}

		public static bool IsPass(this Outcome outcome) {
			return
				(outcome == Outcome.MinorPass) ||
				(outcome == Outcome.Pass) ||
				(outcome == Outcome.MajorPass);
		}

		public static bool IsPass(this CheckOutcome outcome) {
			return outcome.Outcome.IsPass();
		}

		public static bool IsFail(this Outcome outcome) {
			return
				(outcome == Outcome.Fail) ||
				(outcome == Outcome.MinorFail) ||
				(outcome == Outcome.MajorFail);
		}

		public static bool IsFail(this CheckOutcome outcome) {
			return outcome.Outcome.IsFail();
		}

		public static int FailureDegrees(this Outcome outcome) {
			switch (outcome) {
				case Outcome.MajorFail:
					return 3;
				case Outcome.Fail:
					return 2;
				case Outcome.MinorFail:
					return 1;
				default:
					return 0;
			}
		}

		public static int FailureDegrees(this CheckOutcome outcome) {
			return outcome.Outcome.FailureDegrees();
		}

		public static int SuccessDegrees(this Outcome outcome) {
			switch (outcome) {
				case Outcome.MajorPass:
					return 3;
				case Outcome.Pass:
					return 2;
				case Outcome.MinorPass:
					return 1;
				default:
					return 0;
			}
		}

		public static int SuccessDegrees(this CheckOutcome outcome) {
			return outcome.Outcome.SuccessDegrees();
		}

		/// <summary>
		///     CheckDegrees returns a numerical value representing a check outcome. Positive numbers are passes and negative
		///     numbers are failures
		/// </summary>
		/// <param name="outcome"></param>
		/// <returns></returns>
		public static int CheckDegrees(this CheckOutcome outcome) {
			return outcome.Outcome.CheckDegrees();
		}

		public static int CheckDegrees(this Outcome outcome) {
			switch (outcome) {
				case Outcome.MajorPass:
					return 3;
				case Outcome.Pass:
					return 2;
				case Outcome.MinorPass:
					return 1;
				case Outcome.MajorFail:
					return -3;
				case Outcome.Fail:
					return -2;
				case Outcome.MinorFail:
					return -1;
				default:
					return 0;
			}
		}

		public static Outcome Stage(this Outcome outcome, Outcome otherOutcome) {
			return otherOutcome.IsFail()
				? outcome.StageDown(otherOutcome.FailureDegrees())
				: outcome.StageUp(otherOutcome.SuccessDegrees());
		}

		public static Outcome StageUp(this Outcome outcome, int degrees) {
			var newVal = (int) outcome + degrees;
			if (newVal > (int) Outcome.MajorPass) {
				return Outcome.MajorPass;
			}
			return (Outcome) newVal;
		}

		public static Outcome StageDown(this Outcome outcome, int degrees) {
			var newVal = (int) outcome - degrees;
			if (newVal < (int) Outcome.MajorFail) {
				return Outcome.MajorFail;
			}
			return (Outcome) newVal;
		}

		public static string Describe(this Outcome outcome) {
			switch (outcome) {
				case Outcome.Fail:
					return "Failure";
				case Outcome.MajorFail:
					return "Major Failure";
				case Outcome.MajorPass:
					return "Major Success";
				case Outcome.MinorFail:
					return "Minor Failure";
				case Outcome.MinorPass:
					return "Minor Success";
				case Outcome.None:
					return "No Result";
				case Outcome.NotTested:
					return "Not Tested";
				case Outcome.Pass:
					return "Success";
				default:
					return "Unknown Result";
			}
		}

		public static string DescribeAbbreviated(this Outcome outcome)
		{
			switch (outcome)
			{
				case Outcome.Fail:
					return "F";
				case Outcome.MajorFail:
					return "MF";
				case Outcome.MajorPass:
					return "MS";
				case Outcome.MinorFail:
					return "MiF";
				case Outcome.MinorPass:
					return "MiS";
				case Outcome.None:
					return "N";
				case Outcome.NotTested:
					return "NT";
				case Outcome.Pass:
					return "S";
				default:
					return "Unknown Result";
			}
		}

		public static string DescribeColour(this Outcome outcome)
		{
			switch (outcome)
			{
				case Outcome.Fail:
					return "Failure".Colour(Telnet.Red);
				case Outcome.MajorFail:
					return "Major Failure".Colour(Telnet.BoldRed);
				case Outcome.MajorPass:
					return "Major Success".Colour(Telnet.BoldGreen);
				case Outcome.MinorFail:
					return "Minor Failure".Colour(Telnet.Red);
				case Outcome.MinorPass:
					return "Minor Success".Colour(Telnet.Green);
				case Outcome.None:
					return "No Result".Colour(Telnet.Yellow);
				case Outcome.NotTested:
					return "Not Tested".Colour(Telnet.Yellow);
				case Outcome.Pass:
					return "Success".Colour(Telnet.Green);
				default:
					return "Unknown Result".Colour(Telnet.BoldMagenta);
			}
		}

		public static bool GetDifficulty(string text, out Difficulty difficulty) {
			text = text.Trim();
			var difficulties = Enum.GetValues(typeof(Difficulty)).OfType<Difficulty>().ToList();
			if (difficulties.Any(x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase))) {
				difficulty =
					difficulties.FirstOrDefault(
						x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase));
				return true;
			}

			var diffNames = difficulties.Select(x => Tuple.Create(x, Enum.GetName(typeof(Difficulty), x))).ToList();
			if (!diffNames.Any(x => x.Item2.Equals(text, StringComparison.InvariantCultureIgnoreCase))) {
				difficulty = Difficulty.Impossible;
				return false;
			}

			difficulty = diffNames.First(x => x.Item2.Equals(text, StringComparison.InvariantCultureIgnoreCase)).Item1;
			return true;
		}

		public static bool GetOutcome(string text, out Outcome outcome) {
			text = text.Trim();
			var exploded = text.SplitCamelCase();
			var outcomes = Enum.GetValues(typeof(Outcome)).OfType<Outcome>().ToList();
			if (outcomes.Any(x => x.Describe().EqualToAny(text, exploded) || x.DescribeAbbreviated().EqualToAny(text, exploded))) {
				outcome = outcomes.First(x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase));
				return true;
			}

			var outcomeNames = outcomes.Select(x => (Enum: x, Name: Enum.GetName(typeof(Outcome), x), ExplodedName: Enum.GetName(typeof(Outcome), x).SplitCamelCase())).ToList();
			if (!outcomeNames.Any(x => x.Name.EqualToAny(text, exploded) || x.ExplodedName.EqualToAny(text, exploded))) {
				outcome = Outcome.None;
				return false;
			}

			outcome = outcomeNames.First(x => x.Name.EqualToAny(text, exploded) || x.ExplodedName.EqualToAny(text, exploded)).Enum;
			return true;
		}

		public static string Describe(this Difficulty difficulty) {
			switch (difficulty) {
				case Difficulty.Automatic:
					return "Automatic";
				case Difficulty.Insane:
					return "Insane";
				case Difficulty.Easy:
					return "Easy";
				case Difficulty.ExtremelyHard:
					return "Extremely Hard";
				case Difficulty.ExtremelyEasy:
					return "Extremely Easy";
				case Difficulty.Hard:
					return "Hard";
				case Difficulty.Normal:
					return "Normal";
				case Difficulty.Trivial:
					return "Trivial";
				case Difficulty.VeryEasy:
					return "Very Easy";
				case Difficulty.VeryHard:
					return "Very Hard";
				case Difficulty.Impossible:
					return "Impossible";
				default:
					return "Unknown Difficulty";
			}
		}

		public static string DescribeColoured(this Difficulty difficulty)
		{
			switch (difficulty) {
				case Difficulty.Automatic:
					return "Automatic".Colour(Telnet.BoldCyan);
				case Difficulty.Insane:
					return "Insane".Colour(Telnet.BoldRed);
				case Difficulty.Easy:
					return "Easy".Colour(Telnet.Green);
				case Difficulty.ExtremelyHard:
					return "Extremely Hard".Colour(Telnet.Red);
				case Difficulty.ExtremelyEasy:
					return "Extremely Easy".Colour(Telnet.BoldGreen);
				case Difficulty.Hard:
					return "Hard".Colour(Telnet.BoldOrange);
				case Difficulty.Normal:
					return "Normal".Colour(Telnet.Yellow);
				case Difficulty.Trivial:
					return "Trivial".Colour(Telnet.BoldCyan);
				case Difficulty.VeryEasy:
					return "Very Easy".Colour(Telnet.BoldGreen);
				case Difficulty.VeryHard:
					return "Very Hard".Colour(Telnet.BoldOrange);
				case Difficulty.Impossible:
					return "Impossible".Colour(Telnet.BoldRed);
				default:
					return "Unknown Difficulty";
			}
		}

		/// <summary>
		/// Gives a two letter summary of a difficulty in capital letters, e.g. EA for easy, VH for very hard, etc
		/// </summary>
		/// <param name="difficulty">The difficulty to describe</param>
		/// <param name="coloured">Whether to apply ANSI colouring based on difficulty</param>
		/// <returns></returns>
		public static string DescribeBrief(this Difficulty difficulty, bool coloured) {
			switch (difficulty) {
				case Difficulty.Automatic:
					return coloured ? "AU".Colour(Telnet.BoldCyan) : "AU";
				case Difficulty.Insane:
					return coloured ? "IN".Colour(Telnet.BoldRed) : "IN";
				case Difficulty.Easy:
					return coloured ? "EA".Colour(Telnet.Green) : "EA";
				case Difficulty.ExtremelyHard:
					return coloured ? "EH".Colour(Telnet.BoldOrange) : "EH";
				case Difficulty.ExtremelyEasy:
					return coloured ? "EE".Colour(Telnet.BoldGreen) : "EE";
				case Difficulty.Hard:
					return coloured ? "HA".Colour(Telnet.BoldYellow) : "HA";
				case Difficulty.Normal:
					return coloured ? "NO".Colour(Telnet.Yellow) : "NO";
				case Difficulty.Trivial:
					return coloured ? "TR".Colour(Telnet.BoldCyan) : "TR";
				case Difficulty.VeryEasy:
					return coloured ? "VE".Colour(Telnet.BoldGreen) : "VE";
				case Difficulty.VeryHard:
					return coloured ? "VH".Colour(Telnet.BoldOrange) : "VH";
				case Difficulty.Impossible:
					return coloured ? "IM".Colour(Telnet.BoldRed) : "IM";
				default:
					return "??";
			}
		}

		public static Difficulty StageUp(this Difficulty difficulty, int degrees) {
			var newVal = (int) difficulty + degrees;
			if (newVal > (int) Difficulty.Impossible) {
				return Difficulty.Impossible;
			}
			if (newVal < (int) Difficulty.Automatic) {
				return Difficulty.Automatic;
			}
			return (Difficulty) newVal;
		}

		public static Difficulty StageDown(this Difficulty difficulty, int degrees) {
			var newVal = (int) difficulty - degrees;
			if (newVal > (int) Difficulty.Impossible) {
				return Difficulty.Impossible;
			}
			if (newVal < (int) Difficulty.Automatic) {
				return Difficulty.Automatic;
			}
			return (Difficulty) newVal;
		}

		public static int Difference(this Difficulty difficulty, Difficulty compare) {
			return (int) difficulty - (int) compare;
		}

		public static Difficulty Highest(this Difficulty difficulty, params Difficulty[] otherDifficulties)
		{
			return (Difficulty)otherDifficulties.Cast<int>().Plus((int)difficulty).Max();
		}

		public static Difficulty Lowest(this Difficulty difficulty, params Difficulty[] otherDifficulties)
		{
			return (Difficulty)otherDifficulties.Cast<int>().Plus((int)difficulty).Min();
		}
		
		public static Outcome Best(this Outcome outcome, params Outcome[] otherOutcomes)
		{
			return (Outcome)otherOutcomes.Cast<int>().Plus((int)outcome).Max();
		}

		public static Outcome Worst(this Outcome outcome, params Outcome[] otherOutcomes)
		{
			return (Outcome)otherOutcomes.Cast<int>().Plus((int)outcome).Min();
		}
	}
}