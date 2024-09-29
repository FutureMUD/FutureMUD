using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MoreLinq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits.Interfaces;
using Org.BouncyCastle.Crypto.Engines;

namespace MudSharp.Health.Surgery;

public abstract class SurgicalProcedure : SaveableItem, ISurgicalProcedure
{
	protected readonly List<SurgicalProcedurePhase> _phases = new();
	public abstract SurgicalProcedureType Procedure { get; }

	public abstract CheckType Check { get; }

	public ITraitDefinition CheckTrait { get; protected set; }

	public string ProcedureName { get; set; }

	public string ProcedureGerund { get; set; }

	public string ProcedureBeginEmote { get; set; }

	public override string FrameworkItemType => "Surgical Procedure";

	public double BaseCheckBonus { get; set; }

	public virtual bool RequiresUnconsciousPatient => false;
	public virtual bool RequiresInvasiveProcedureFinalisation => false;

	public virtual bool RequiresLivingPatient => true;

	public IFutureProg UsabilityProg { get; set; }
	public IFutureProg WhyCannotUseProg { get; set; }
	public IFutureProg CompletionProg { get; set; }
	public IFutureProg AbortProg { get; set; }
	public IEnumerable<SurgicalProcedurePhase> Phases => _phases;

	public IKnowledge KnowledgeRequired { get; set; }

	public string ProcedureDescription { get; set; }

	public string MedicalSchool { get; set; }

	public IBodyPrototype TargetBodyType { get; set; }

	public void PerformProcedure(ICharacter surgeon, ICharacter patient, params object[] additionalArguments)
	{
		var args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		var perceivableArgs = new[] { surgeon, patient }.Concat(args.OfType<IPerceivable>()).ToArray();
		surgeon.AddEffect(GetActionEffect(surgeon, patient, args), TimeSpan.FromSeconds(10));
		surgeon.OutputHandler.Handle(new EmoteOutput(new Emote(ProcedureBeginEmote, surgeon, perceivableArgs)));
	}

	public abstract void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments);

	public abstract void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments);

	public virtual string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return $"{ProcedureGerund} $1";
	}

	public virtual bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (!CharacterState.Able.HasFlag(surgeon.State))
		{
			return false;
		}

		if (patient.Corpse == null)
		{
			if (surgeon.Location != patient.Location)
			{
				return false;
			}
		}
		else
		{
			if (surgeon.Location != patient.Corpse.Parent.Location)
			{
				return false;
			}
		}


		if (patient.Movement != null || surgeon.Movement != null)
		{
			return false;
		}

		if (patient.State.HasFlag(CharacterState.Dead) && RequiresLivingPatient)
		{
			return false;
		}

		if (surgeon.Combat != null && surgeon.MeleeRange)
		{
			return false;
		}

		if (patient.Combat != null && patient.MeleeRange)
		{
			return false;
		}

		if (KnowledgeRequired != null && !surgeon.Knowledges.Contains(KnowledgeRequired))
		{
			return false;
		}

		if (UsabilityProg != null && !((bool?)UsabilityProg.Execute(surgeon, patient) ?? false))
		{
			return false;
		}

		return Phases.Where(x => x.InventoryPlanTemplate != null)
		             .Select(x => x.InventoryPlanTemplate.CreatePlan(surgeon))
		             .All(x => x.PlanIsFeasible() == InventoryPlanFeasibility.Feasible);
	}

	public virtual string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (!CharacterState.Able.HasFlag(surgeon.State))
		{
			return $"You cannot perform that procedure because you are {surgeon.State}.";
		}

		if (patient.Corpse == null)
		{
			if (surgeon.Location != patient.Location)
			{
				return "You cannot perform that procedure because your patient is not in the same place as you.";
			}
		}
		else
		{
			if (surgeon.Location != patient.Corpse.Parent.Location)
			{
				return "You cannot perform that procedure because your patient is not in the same place as you.";
			}
		}

		if (patient.Movement != null || surgeon.Movement != null)
		{
			return "You cannot perform that procedure while either you or your patient is moving.";
		}

		if (patient.State.HasFlag(CharacterState.Dead) && RequiresLivingPatient)
		{
			return
				"You cannot perform that procedure because your patient is dead and this procedure requires a living patient.";
		}

		if (surgeon.Combat != null && surgeon.MeleeRange)
		{
			return "You cannot perform that procedure while you are in melee combat!";
		}

		if (patient.Combat != null && patient.MeleeRange)
		{
			return "You cannot perform that procedure while your patient is in melee combat!";
		}

		if (KnowledgeRequired != null && !surgeon.Knowledges.Contains(KnowledgeRequired))
		{
			return "You do not have the required knowledges to perform that procedure.";
		}

		if (UsabilityProg != null && !((bool?)UsabilityProg.Execute(surgeon, patient) ?? false))
		{
			return WhyCannotUseProg?.Execute(surgeon, patient)?.ToString() ?? "You cannot perform that procedure";
		}

		if (
			Phases.Select(x => x.InventoryPlanTemplate.CreatePlan(surgeon))
			      .Any(x => x.PlanIsFeasible() != InventoryPlanFeasibility.Feasible))
		{
			switch (
				Phases.Select(x => x.InventoryPlanTemplate.CreatePlan(surgeon).PlanIsFeasible())
				      .First(x => x != InventoryPlanFeasibility.Feasible))
			{
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					return "You do not have all of the items you require to perform that procedure.";
				case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					return
						$"You do not have enough possible free {surgeon.Body.WielderDescriptionPlural.ToLowerInvariant()} to hold items required to perform that procedure.";
				case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
					return
						$"You do not have enough possible free {surgeon.Body.WielderDescriptionPlural.ToLowerInvariant()} to wield items required to perform that procedure.";
			}
		}

		throw new NotImplementedException("Got to the bottom of SurgicalProcedure.WhyCannotPerformProcedure");
	}

	public abstract Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments);

	public IEffect GetActionEffect(ICharacter surgeon, ICharacter patient, params object[] additionalArguments)
	{
		return new SurgicalProcedureEffect(surgeon, patient, this,
					from phase in Phases
					select new Action<IPerceivable>(perceivable =>
					{
						if (surgeon.State == CharacterState.Dead)
						{
							SilentAbortProcedure(surgeon, patient,
								Outcome.NotTested, additionalArguments);
							return;
						}

						if (patient.State == CharacterState.Dead &&
							RequiresLivingPatient)
						{
							surgeon.OutputHandler.Handle(
								new EmoteOutput(
									new Emote(
										$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0's patient has died.",
										surgeon, surgeon, patient)));
							surgeon.RemoveAllEffects(x =>
								x.IsEffectType<
									SurgicalProcedureEffect>());
							return;
						}

						if ((patient.Corpse?.Parent.InRoomLocation ??
							 patient.InRoomLocation) != surgeon.InRoomLocation)
						{
							surgeon.OutputHandler.Handle(
								new EmoteOutput(
									new Emote(
										$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0's patient is no longer there.",
										surgeon, surgeon, patient)));
							surgeon.RemoveAllEffects(x =>
								x.IsEffectType<
									SurgicalProcedureEffect>());
							SilentAbortProcedure(surgeon, patient,
								Outcome.NotTested, additionalArguments);
							return;
						}

						if (patient.Movement != null ||
							surgeon.Movement != null)
						{
							surgeon.OutputHandler.Handle(
								new EmoteOutput(
									new Emote(
										$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because #0 or &0's patient is moving about.",
										surgeon, surgeon, patient)));
							surgeon.RemoveAllEffects(x =>
								x.IsEffectType<
									SurgicalProcedureEffect>());
							SilentAbortProcedure(surgeon, patient,
								Outcome.NotTested, additionalArguments);
							return;
						}

						if (surgeon.Combat != null && surgeon.MeleeRange)
						{
							surgeon.OutputHandler.Handle(
								new EmoteOutput(
									new Emote(
										$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0 have|has been engaged in melee combat.",
										surgeon, surgeon, patient)));
							surgeon.RemoveAllEffects(x =>
								x.IsEffectType<
									SurgicalProcedureEffect>());
							SilentAbortProcedure(surgeon, patient,
								Outcome.NotTested, additionalArguments);
							return;
						}

						if (patient.Combat != null && patient.MeleeRange)
						{
							surgeon.OutputHandler.Handle(
								new EmoteOutput(
									new Emote(
										$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0's patient $1|have|has been engaged in melee combat.",
										surgeon, surgeon, patient)));
							surgeon.RemoveAllEffects(x =>
								x.IsEffectType<
									SurgicalProcedureEffect>());
							SilentAbortProcedure(surgeon, patient,
								Outcome.NotTested, additionalArguments);
							return;
						}

						if (!CharacterState.Able.HasFlag(surgeon.State))
						{
							surgeon.OutputHandler.Handle(
								new EmoteOutput(
									new Emote(
										$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0 are|is {surgeon.State.Describe().ToLowerInvariant()}.",
										surgeon, surgeon, patient)));
							surgeon.RemoveAllEffects(x =>
								x.IsEffectType<
									SurgicalProcedureEffect>());
							SilentAbortProcedure(surgeon, patient,
								Outcome.NotTested, additionalArguments);
							return;
						}

						var plan = phase.InventoryPlanTemplate
							?.CreatePlan(surgeon);
						if (plan != null && plan.PlanIsFeasible() !=
							InventoryPlanFeasibility.Feasible)
						{
							switch (plan.PlanIsFeasible())
							{
								case InventoryPlanFeasibility
									.NotFeasibleMissingItems:
									surgeon.OutputHandler.Handle(
										new EmoteOutput(
											new Emote(
												$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0 do|does not have all the items &0 requires to perform that procedure.",
												surgeon, surgeon,
												patient)));
									surgeon.RemoveAllEffects(x =>
										x.IsEffectType<
											SurgicalProcedureEffect>());
									SilentAbortProcedure(surgeon,
										patient, Outcome.NotTested,
										additionalArguments);
									return;
								case InventoryPlanFeasibility
									.NotFeasibleNotEnoughHands:
									surgeon.OutputHandler.Handle(
										new EmoteOutput(
											new Emote(
												$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0 do|does not have enough free {surgeon.Body.WielderDescriptionPlural.ToLowerInvariant()} to hold all the items required to perform that procedure.",
												surgeon, surgeon,
												patient)));
									surgeon.RemoveAllEffects(x =>
										x.IsEffectType<
											SurgicalProcedureEffect>());
									SilentAbortProcedure(surgeon,
										patient, Outcome.NotTested,
										additionalArguments);
									return;
								case InventoryPlanFeasibility
									.NotFeasibleNotEnoughWielders:
									surgeon.OutputHandler.Handle(
										new EmoteOutput(
											new Emote(
												$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because &0 do|does not have enough free {surgeon.Body.WielderDescriptionPlural.ToLowerInvariant()} to wield all the items required to perform that procedure.",
												surgeon, surgeon,
												patient)));
									surgeon.RemoveAllEffects(x =>
										x.IsEffectType<
											SurgicalProcedureEffect>());
									SilentAbortProcedure(surgeon,
										patient, Outcome.NotTested,
										additionalArguments);
									return;
							}
						}

						if (!phase.PhaseSuccessful(surgeon, patient,
								additionalArguments))
						{
							surgeon.OutputHandler.Handle(
								new EmoteOutput(
									new Emote(
										$"@ cease|ceases {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because {phase.WhyPhaseNotSuccessful(surgeon, patient, additionalArguments)}",
										surgeon, surgeon, patient)));
							surgeon.RemoveAllEffects(x =>
								x.IsEffectType<
									SurgicalProcedureEffect>());
							SilentAbortProcedure(surgeon, patient,
								Outcome.NotTested, additionalArguments);
							return;
						}

						plan?.ExecuteWholePlan();
						plan?.FinalisePlanNoRestore();
						surgeon.OutputHandler.Handle(new EmoteOutput(
							new Emote(
								DressPhaseEmote(phase.PhaseEmote,
									surgeon, patient,
									additionalArguments), surgeon,
								surgeon,
								patient)));
						Gameworld.GetCheck(Check).Check(surgeon,
							GetProcedureDifficulty(surgeon, patient, CheckTrait,
								additionalArguments),
							patient);
						phase.OnPhaseProg?.Execute(surgeon, patient);
					}),
					$"performing {ProcedureName} procedure",
			new[] { "general", "movement" },
			DescribeProcedureGerund(surgeon, patient, additionalArguments), _phases.Count,
			from phase in Phases select phase.BaseLength, GetAdditionalInventory(surgeon, patient, additionalArguments),
			additionalArguments);
	}

	protected virtual List<(IGameItem Item, DesiredItemState State)> GetAdditionalInventory(ICharacter surgeon,
		ICharacter patient, object[] additionalArguments)
	{
		return new List<(IGameItem Item, DesiredItemState State)>();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.SurgicalProcedures.Find(Id);
		dbitem.Name = Name;
		dbitem.ProcedureName = ProcedureName;
		dbitem.ProcedureBeginEmote = ProcedureBeginEmote;
		dbitem.ProcedureDescriptionEmote = ProcedureDescription;
		dbitem.ProcedureGerund = ProcedureGerund;
		dbitem.Procedure = (int)Procedure;
		dbitem.KnowledgeRequiredId = KnowledgeRequired.Id;
		dbitem.AbortProgId = AbortProg?.Id;
		dbitem.CompletionProgId = CompletionProg?.Id;
		dbitem.UsabilityProgId = UsabilityProg?.Id;
		dbitem.WhyCannotUseProgId = WhyCannotUseProg?.Id;
		dbitem.BaseCheckBonus = BaseCheckBonus;
		dbitem.Check = (int)Check;
		dbitem.MedicalSchool = MedicalSchool;
		dbitem.CheckTraitDefinitionId = CheckTrait?.Id;
		dbitem.Definition = SaveDefinition();
		dbitem.TargetBodyTypeId = TargetBodyType.Id;
		FMDB.Context.SurgicalProcedurePhases.RemoveRange(dbitem.SurgicalProcedurePhases);
		var phaseNumber = 1;
		foreach (var phase in _phases)
		{
			dbitem.SurgicalProcedurePhases.Add(new Models.SurgicalProcedurePhase
			{
				SurgicalProcedure = dbitem,
				PhaseNumber = phaseNumber++,
				BaseLengthInSeconds = phase.BaseLength.TotalSeconds,
				PhaseEmote = phase.PhaseEmote,
				PhaseSpecialEffects = phase.PhaseSpecialEffects,
				OnPhaseProgId = phase.OnPhaseProg?.Id,
				InventoryActionPlan = phase.InventoryPlanTemplate?.SaveToXml().ToString() ?? ""
			});
		}
		Changed = false;
	}

	protected virtual string SaveDefinition()
	{
		return "";
	}

	protected void CreateMedicalFinalisationRequiredEffect(ICharacter surgeon, ICharacter patient, Outcome result,
		IBodypart bodypartAffected, Difficulty baseDifficulty, bool worsenExisting = false)
	{
		if (!RequiresInvasiveProcedureFinalisation)
		{
			return;
		}

		if (bodypartAffected == null)
		{
			return;
		}

		// Phase up or down difficulty by result
		var difficulty = baseDifficulty.StageDown(result.CheckDegrees());

		//merits of surgeon could make this easier
		var merits = surgeon.Merits.OfType<ISurgeryFinalisationMerit>().Where(x => x.Applies(surgeon)).ToList();
		foreach (var merit in merits)
		{
			difficulty = difficulty.StageDown(merit.BonusDegrees);
		}

		// sanity check - it should never be impossible to stitch someone up
		if (difficulty == Difficulty.Impossible)
		{
			difficulty = difficulty.StageDown(1);
		}

		if (worsenExisting)
		{
			var effect = patient.CombinedEffectsOfType<SurgeryFinalisationRequired>()
			                    .FirstOrDefault(x => x.Bodypart == bodypartAffected);
			if (effect != null)
			{
				effect.Difficulty = difficulty.Highest(effect.Difficulty);
				effect.Changed = true;
				return;
			}
		}

		patient.AddEffect(new SurgeryFinalisationRequired(patient, bodypartAffected, difficulty),
			TimeSpan.FromSeconds(600));
	}

	protected abstract void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
		params object[] additionalArguments);

	protected virtual object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return additionalArguments;
	}

	public virtual string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		return emote;
	}

	public virtual string DressPhaseEmoteHelpAddendum => "";

	public virtual int DressPhaseEmoteExtraArgumentCount => 0;

	#region Constructors

	protected SurgicalProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDB(procedure);
	}

	protected SurgicalProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge)
	{
		Gameworld = gameworld;
		TargetBodyType = body;
		_name = name;
		ProcedureName = name;
		ProcedureGerund = gerund;
		ProcedureBeginEmote = $@"@ begin|begins {gerund} $1";
		ProcedureDescription = "This procedure has not been described.";
		MedicalSchool = school;
		KnowledgeRequired = knowledge;

		using (new FMDB())
		{
			var dbitem = new Models.SurgicalProcedure
			{
				Name = _name,
				ProcedureName = ProcedureName,
				Procedure = (int)Procedure,
				BaseCheckBonus = BaseCheckBonus,
				Check = (int)Check,
				MedicalSchool = MedicalSchool,
				KnowledgeRequiredId = KnowledgeRequired?.Id,
				UsabilityProgId = null,
				WhyCannotUseProgId = null,
				CompletionProgId = null,
				AbortProgId = null,
				ProcedureBeginEmote = null,
				ProcedureDescriptionEmote = ProcedureDescription,
				ProcedureGerund = ProcedureGerund,
				Definition = SaveDefinition(),
				CheckTraitDefinitionId = null,
				TargetBodyTypeId = TargetBodyType.Id,
			};
			FMDB.Context.SurgicalProcedures.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	#endregion

	protected virtual (Func<ICharacter, ICharacter, object[], bool> Success, Func<ICharacter, ICharacter, object[], string> Error, string Description)
		GetSpecialPhaseAction(string actionText)
	{
		if (actionText.StartsWith("bleeding", StringComparison.InvariantCultureIgnoreCase))
		{
			var bloodAmounts = new StringStack(actionText.RemoveFirstWord()).PopAll().Select(x => double.Parse(x)).ToList();
			var bleedingDictionary = new Dictionary<Outcome, double>();
			var outcome = Outcome.MajorFail;
			foreach (var amount in bloodAmounts)
			{
				bleedingDictionary[outcome] = amount;
				outcome.StageUp();
			}

			if (bleedingDictionary.Keys.OrderBy(x => x).SequenceEqual(new[]
			{
				Outcome.MajorFail,
				Outcome.Fail,
				Outcome.MinorFail,
				Outcome.MinorPass,
				Outcome.Pass,
				Outcome.MajorPass
			}))
			{
				return BleedingPhaseAction(bleedingDictionary);
			}
		}
		return ((surgeon, patient, paramaters) => true, (surgeon, patient, paramaters) => "of an unknown reason", "");
	}

	private (Func<ICharacter, ICharacter, object[], bool> Success, Func<ICharacter, ICharacter, object[], string> Error, string Description) BleedingPhaseAction(Dictionary<Outcome, double> bleedingDictionary)
	{
		return ((surgeon, patient, parameters) =>
				{
					var actualOutcome = Gameworld.GetCheck(Check).Check(surgeon, GetProcedureDifficulty(surgeon, patient, parameters), CheckTrait, patient);
					var bloodLoss = bleedingDictionary[actualOutcome];
					patient.Body.CurrentBloodVolumeLitres -= bloodLoss;
					var hands = surgeon.Body.HoldLocs.Concat<IExternalBodypart>(surgeon.Body.WieldLocs).Distinct();
					BodyLiquidContamination.CreateOrMergeEffect(surgeon, new LiquidMixture(patient.Body.BloodLiquid, bloodLoss, Gameworld), hands);
					return true;
				}
				,
				(surgeon, patient, parameters) =>
				{
					return $"of an unknown reason";
				},
				$"causes bleeding [{bleedingDictionary[Outcome.MajorFail].ToString("N2").Colour(Telnet.BoldRed)}|{bleedingDictionary[Outcome.Fail].ToString("N2").Colour(Telnet.Red)}|{bleedingDictionary[Outcome.MinorFail].ToString("N2").Colour(Telnet.BoldYellow)}|{bleedingDictionary[Outcome.MinorPass].ToString("N2").Colour(Telnet.Green)}|{bleedingDictionary[Outcome.Pass].ToString("N2").Colour(Telnet.BoldGreen)}|{bleedingDictionary[Outcome.MajorPass].ToString("N2").Colour(Telnet.BoldBlue)}"
			);
	}

	protected virtual void LoadFromDB(MudSharp.Models.SurgicalProcedure procedure)
	{
		_id = procedure.Id;
		_name = procedure.Name;
		ProcedureName = procedure.ProcedureName;
		ProcedureGerund = procedure.ProcedureGerund;
		ProcedureBeginEmote = procedure.ProcedureBeginEmote;
		MedicalSchool = procedure.MedicalSchool;
		BaseCheckBonus = procedure.BaseCheckBonus;
		KnowledgeRequired = Gameworld.Knowledges.Get(procedure.KnowledgeRequiredId ?? 0);
		UsabilityProg = Gameworld.FutureProgs.Get(procedure.UsabilityProgId ?? 0);
		WhyCannotUseProg = Gameworld.FutureProgs.Get(procedure.WhyCannotUseProgId ?? 0);
		CompletionProg = Gameworld.FutureProgs.Get(procedure.CompletionProgId ?? 0);
		AbortProg = Gameworld.FutureProgs.Get(procedure.AbortProgId ?? 0);
		ProcedureDescription = procedure.ProcedureDescriptionEmote;
		CheckTrait = Gameworld.Traits.Get(procedure.CheckTraitDefinitionId ?? 0);
		TargetBodyType = Gameworld.BodyPrototypes.Get(procedure.TargetBodyTypeId);
		foreach (var phase in procedure.SurgicalProcedurePhases)
		{
			var nPhase = new SurgicalProcedurePhase
			{
				PhaseEmote = phase.PhaseEmote,
				OnPhaseProg = Gameworld.FutureProgs.Get(phase.OnPhaseProgId ?? 0),
				BaseLength = TimeSpan.FromSeconds(phase.BaseLengthInSeconds),
				InventoryPlanTemplate =
					string.IsNullOrWhiteSpace(phase.InventoryActionPlan)
						? default(IInventoryPlanTemplate)
						: new InventoryPlanTemplate(XElement.Parse(phase.InventoryActionPlan), Gameworld),
				PhaseSpecialEffects = phase.PhaseSpecialEffects
			};
			if (!string.IsNullOrWhiteSpace(phase.PhaseSpecialEffects))
			{
				(nPhase.PhaseSuccessful, nPhase.WhyPhaseNotSuccessful, nPhase.PhaseSpecialEffectsDescription) =
					GetSpecialPhaseAction(phase.PhaseSpecialEffects);
			}

			_phases.Add(nPhase);
		}
	}

	protected virtual IEnumerable<IEnumerable<FutureProgVariableTypes>> ParametersForCompletionProg => new[] {
				new[]
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Number,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Item
				},
				new[]
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Item
				},
			};

	protected virtual IEnumerable<IEnumerable<FutureProgVariableTypes>> ParametersForCancelProg => new[] {
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Number,
			FutureProgVariableTypes.Text
		},
		new[]
		{
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Character,
			FutureProgVariableTypes.Text,
			FutureProgVariableTypes.Text
		},
	};

	protected string HelpText => $@"You can use the following options with this surgical procedure:

	#3name <name>#0 - renames this surgery
	#3alias <name#0 - sets a shorter alias for  this surgery
	#3school <school>#0 - sets the school of medicine this surgery belongs to
	#3gerund <gerund>#0 - sets the gerund (-ing ending word) for this surgery
	#3description#0 - drops you into an editor to set the description
	#3knowledge <knowledge#0 - sets the knowledge required for this procedure
	#3beginemote <emote>#0 - sets the emote when you begin this procedure
	#3trait <which>#0 - sets the trait or skill used for this surgery
	#3bonus <amount>#0 - sets the bonus/penalty to the skill check for this surgery
	#3useprog <prog>#0 - sets a prog that controls if you can use this
	#3whyprog <prog>#0 - sets a prog that gives an error message if useprog is false
	#3endprog <prog>#0 - sets a prog that runs when the surgery finishes
	#3cancelprog <prog>#0 - sets a prog that runs when the surgery is aborted
	#3useprog none#0 - clears the useprog (always usable)
	#3whyprog none#0 - clears the whyprog (generic error message)
	#3endprog none#0 - clears the endprog (no prog executed)
	#3cancelprog none#0 - clears the cancelprog (no prog executed){AdditionalHelpText}

These commands allow you to work with the phases:

	#3phase add <length> <emote>#0 - adds a new phase
	#3phase swap <##2> <##1>#0 - swaps the order of two phases
	#3phase delete <##>#0 - deletes a phase
	#3phase <##> length <length>#0 - changes the length of an existing phase
	#3phase <##> emote <emote>#0 - changes the emote of an existing phase
	#3phase <##> prog <prog>#0 - changes the on-phase prog of an existing phase
	#3phase <##> special <special>#0 - changes the special action of an existing phase
	#3phase <##> held <tag>#0 - adds a required held tool to the phases' inventory plan
	#3phase <##> wield <tag>#0 - adds a required wielded tool to the phases' inventory plan
	#3phase <##> worn <tag>#0 - adds a required worn tool to the phases' inventory plan
	#3phase <##> inroom <tag>#0 - adds a required in-room tool to the phases' inventory plan
	#3phase <##> attached <tag> [<containertag>]#0 - adds a required tool to be attached to something else
	#3phase <##> container <tag> [<containertag>]#0 - adds a required tool to be in a container
	#3phase <##> used <tag> [<quantity>] [<containertag>]#0 - adds a required consumed material to the phases' inventory plan
	#3phase <##> liquid <tag> [<quantity>] [<containertag>]#0 - adds a required consumed material to the phases' inventory plan

There are the following special phase actions available for this surgical procedure:

{SpecialActionText}";

	protected virtual string SpecialActionText => @"	#Cbleeding <MF> <F> <mF> <mP> <P> <MP>#0 - sets bleeding for different check success levels";
	protected virtual string AdditionalHelpText => @"";

	protected string PhaseHelpText => @"You must use one of the following options:

	#3phase add <length> <emote>#0 - adds a new phase
	#3phase swap <##2> <##1>#0 - swaps the order of two phases
	#3phase delete <##>#0 - deletes a phase
	#3phase <##> length <length>#0 - changes the length of an existing phase
	#3phase <##> emote <emote>#0 - changes the emote of an existing phase
	#3phase <##> prog <prog>#0 - changes the on-phase prog of an existing phase
	#3phase <##> special <special>#0 - changes the special action of an existing phase
	#3phase <##> held <tag>#0 - adds a required held tool to the phases' inventory plan
	#3phase <##> wield <tag>#0 - adds a required wielded tool to the phases' inventory plan
	#3phase <##> worn <tag>#0 - adds a required worn tool to the phases' inventory plan
	#3phase <##> inroom <tag>#0 - adds a required in-room tool to the phases' inventory plan
	#3phase <##> attached <tag> [<containertag>]#0 - adds a required tool to be attached to something else
	#3phase <##> container <tag> [<containertag>]#0 - adds a required tool to be in a container
	#3phase <##> used <tag> [<quantity>] [<containertag>]#0 - adds a required consumed material to the phases' inventory plan
	#3phase <##> liquid <tag> [<quantity>] [<containertag>]#0 - adds a required consumed material to the phases' inventory plan"
		.SubstituteANSIColour();

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "procedurename":
			case "alias":
				return BuildingCommandProcedureName(actor, command);
			case "school":
				return BuildingCommandSchool(actor, command);
			case "gerund":
				return BuildingCommandGerund(actor, command);
			case "description":
				return BuildingCommandDescription(actor, command);
			case "beginemote":
				return BuildingCommandBeginEmote(actor, command);
			case "useprog":
			case "usabilityprog":
			case "usableprog":
				return BuildingComamandUsabilityProg(actor, command);
			case "whycannotuseprog":
			case "whyprog":
			case "whycantuseprog":
			case "whyuseprog":
				return BuildingCommandWhyCannotUseProg(actor, command);
			case "completionprog":
			case "completeprog":
			case "finishprog":
			case "endprog":
				return BuildingCommandCompletionProg(actor, command);
			case "abortprog":
			case "cancelprog":
				return BuildingCommandAbortProg(actor, command);
			case "knowledge":
				return BuildingCommandKnowledge(actor, command);
			case "bonus":
			case "basebonus":
			case "checkbonus":
			case "basecheckbonus":
				return BuildingCommandBaseCheckBonus(actor, command);
			case "check":
			case "checktrait":
			case "trait":
			case "skill":
				return BuildingCommandCheckTrait(actor, command);
			case "phase":
				return BuildingCommandPhase(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandPhase(ICharacter actor, StringStack command)
	{
		var cmd = command.PopForSwitch();
		switch (cmd)
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandPhaseAdd(actor, command);
			case "swap":
			case "reorder":
			case "order":
				return BuildingCommandPhaseSwap(actor, command);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandPhaseRemove(actor, command);
		}

		if (!int.TryParse(cmd, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must either specify #3add#0, #3swap#0, #3delete#0 or the number of an existing phase with this command.".SubstituteANSIColour());
			return false;
		}

		var phase = _phases.ElementAtOrDefault(value - 1);
		if (phase is null)
		{
			actor.OutputHandler.Send(
				$"There is no such phase. There are only {_phases.Count.ToString("N0", actor).ColourValue()} phases.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(PhaseHelpText);
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "emote":
				return BuildingCommandPhaseEmote(actor, command, phase);
			case "prog":
				return BuildingCommandPhaseProg(actor, command, phase);
			case "special":
				return BuildingCommandPhaseSpecial(actor, command, phase);
			case "held":
			case "hold":
			case "wield":
			case "wielded":
			case "wear":
			case "worn":
			case "attach":
			case "attached":
			case "sheath":
			case "sheathed":
			case "drop":
			case "dropped":
			case "room":
			case "inroom":
			case "put":
			case "incontainer":
			case "container":
			case "consume":
			case "consumed":
			case "use":
			case "used":
			case "consumeliquid":
			case "consumedliquid":
			case "liquid":
				return BuildingCommandPhaseInventoryAdd(actor, command.GetUndo(), phase);
			case "removeinv":
			case "removeinventory":
			case "remove":
				return BuildingCommandPhaseInventoryRemove(actor, command, phase);
		}

		actor.OutputHandler.Send(PhaseHelpText);
		return false;
	}

	private bool BuildingCommandPhaseInventoryRemove(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which inventory action do you want to remove from that phase?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		if (phase.InventoryPlanTemplate is null)
		{
			actor.OutputHandler.Send("That phase does not have any inventory actions.");
			return false;
		}

		var action = phase.InventoryPlanTemplate.FirstPhase.Actions.ElementAtOrDefault(value - 1);
		if (action is null)
		{
			actor.OutputHandler.Send("That phase does not have so many inventory actions.");
			return false;
		}

		phase.InventoryPlanTemplate.FirstPhase.RemoveAction(action);
		if (!phase.InventoryPlanTemplate.FirstPhase.Actions.Any())
		{
			phase.InventoryPlanTemplate = null;
		}
		Changed = true;
		actor.OutputHandler.Send($"You delete the inventory action {action.Describe(actor)} from phase {(_phases.IndexOf(phase) + 1).ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPhaseInventoryAdd(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		var action = InventoryPlanTemplate.ParseActionFromBuilderInput(actor, command);
		if (action is null)
		{
			return false;
		}

		if (phase.InventoryPlanTemplate is null)
		{
			phase.InventoryPlanTemplate = new InventoryPlanTemplate(Gameworld, action);
		}
		else
		{
			phase.InventoryPlanTemplate.FirstPhase.AddAction(action);
		}

		Changed = true;
		actor.OutputHandler.Send($"You add the inventory action {action.Describe(actor)} to phase {(_phases.IndexOf(phase) + 1).ToString("N0", actor).ColourValue()}.");
		return true;
	}

	protected virtual bool BuildingCommandPhaseSpecial(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		var helpText = $"That is not a valid special action. You must either specify a valid special action (see {"surgery set ?".MXPSend("surgery set ?")}), use {"remove <#>".ColourCommand()} to remove a specific one, or use {"none".ColourCommand()} to reset special actions.";
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(helpText);
			return false;
		}

		switch (command.PopSpeech())
		{
			case "none":
				phase.PhaseSpecialEffectsDescription = "";
				phase.PhaseSpecialEffects = "";
				phase.PhaseSuccessful = (surgeon, patient, additional) => true;
				phase.WhyPhaseNotSuccessful = (surgeon, patient, additional) => "for an unknown reason";
				actor.OutputHandler.Send($"Phase {(_phases.IndexOf(phase) + 1).ToString("N0", actor).ColourValue()} will no longer have a special action.");
				Changed = true;
				return true;
			case "delete":
			case "remove":
			case "del":
			case "rem":
				if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
				{
					actor.OutputHandler.Send("You must specify which action you want to delete.");
					return false;
				}

				if ((phase.PhaseSpecialEffects ?? "").Split('\n').Length < value)
				{
					actor.OutputHandler.Send($"There are only {(phase.PhaseSpecialEffects ?? "").Split('\n').Length.ToString("N0", actor)} special actions on this phase.");
					return false;
				}

				phase.PhaseSpecialEffects = phase.PhaseSpecialEffects.Split('\n').Exclude(value - 1, 1).ListToLines();
				phase.PhaseSpecialEffectsDescription = phase.PhaseSpecialEffectsDescription.Split('\n').Exclude(value - 1, 1).ListToLines();
				
				var successes = phase.PhaseSuccessful?.GetInvocationList().Exclude(value - 1, 1).ToList();
				phase.PhaseSuccessful = null;
				foreach (var success in successes ?? Enumerable.Empty<Delegate>())
				{
					phase.PhaseSuccessful += (Func<ICharacter, ICharacter, object[], bool>)success;
				}

				var whys = phase.WhyPhaseNotSuccessful?.GetInvocationList().Exclude(value - 1, 1).ToList();
				phase.WhyPhaseNotSuccessful = null;
				foreach (var why in whys ?? Enumerable.Empty<Delegate>())
				{
					phase.WhyPhaseNotSuccessful += (Func<ICharacter, ICharacter, object[], string>)why;
				}

				Changed = true;
				actor.OutputHandler.Send($"You delete the {value.ToOrdinal().ColourValue()} special action from phase {(_phases.IndexOf(phase) + 1).ToString("N0", actor).ColourValue()}.");
				return true;
			case "bleeding":
			case "bleed":
			case "blood":
				var bloodTexts = new StringStack(command.RemainingArgument).PopSpeechAll().ToList();
				if (bloodTexts.Count != 6)
				{
					actor.OutputHandler.Send(
						"You must supply exactly 6 numbers representing the amount of bleeding to inflict.");
					return false;
				}

				var bleedingDictionary = new Dictionary<Outcome, double>();
				var outcome = Outcome.MajorFail;
				foreach (var text in bloodTexts)
				{
					if (!Gameworld.UnitManager.TryGetBaseUnits(text, UnitType.FluidVolume, out var bleeding))
					{
						actor.OutputHandler.Send($"The text {text.ColourCommand()} is not a valid fluid amount.");
						return false;
					}
					bleedingDictionary[outcome] = bleeding;
					outcome = outcome.StageUp();
				}

				var (truth, error, desc) = BleedingPhaseAction(bleedingDictionary);
				phase.PhaseSpecialEffects = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n")
				                                                             .FluentAppend($"bleeding {bleedingDictionary[Outcome.MajorFail]} {bleedingDictionary[Outcome.Fail]} {bleedingDictionary[Outcome.MinorFail]} {bleedingDictionary[Outcome.MinorPass]} {bleedingDictionary[Outcome.Pass]} {bleedingDictionary[Outcome.MajorPass]}", true);
				phase.PhaseSuccessful += truth;
				phase.WhyPhaseNotSuccessful += error;
				phase.PhaseSpecialEffectsDescription = (phase.PhaseSpecialEffects ?? "").ConcatIfNotEmpty("\n").FluentAppend(desc, true);
				Changed = true;
				actor.OutputHandler.Send($"There will now be a bleeding effect on this phase, described as \"{desc}\"");
				return true;
		}

		actor.OutputHandler.Send(helpText);
		return false;
	}

	private bool BuildingCommandPhaseProg(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a prog, or use {"none".ColourCommand()} to clear one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			phase.OnPhaseProg = null;
			Changed = true;
			actor.OutputHandler.Send($"You clear any prog execution from phase {(_phases.IndexOf(phase) + 1).ToString("N0", actor).ColourValue()}.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
			}).LookupProg();

		if (prog is null)
		{
			return false;
		}

		phase.OnPhaseProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} will now be executed at phase {(_phases.IndexOf(phase) + 1).ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private static readonly Regex PhaseEmoteRegexInvPlan = new(@"\$i(?<index>[0-9]+)", RegexOptions.IgnoreCase);
	private static readonly Regex PhaseEmoteRegexAdditionalArgs = new(@"\{(?<index>\d+)\}");

	private bool BuildingCommandPhaseEmote(ICharacter actor, StringStack command, SurgicalProcedurePhase phase)
	{
		var i = 1;
		var phaseItems = 
			(phase.InventoryPlanTemplate?.FirstPhase.Actions.Select(x => $"#3$i{i++.ToString("F0", actor)}#0 = {x.Describe(actor)}") ?? Enumerable.Empty<string>())
			.ToList();

		if (command.IsFinished)
		{
			var sb = new StringBuilder();
			sb.AppendLine("What emote would you like to set for this phase?");
			sb.AppendLine();
			sb.AppendLine("You can use the following markups in the emote:");
			sb.AppendLine("\t#3$0#0 = the surgeon".SubstituteANSIColour());
			sb.AppendLine("\t#3$1#0 = the patient".SubstituteANSIColour());
			sb.Append(DressPhaseEmoteHelpAddendum);
			foreach (var item in phaseItems)
			{
				sb.AppendLine($"\t{item}");
			}
			actor.OutputHandler.Send(sb.ToString());
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		foreach (Match match in PhaseEmoteRegexAdditionalArgs.Matches(emoteText))
		{
			var index = int.Parse(match.Groups["index"].Value);
			if (index < 0 || index >= DressPhaseEmoteExtraArgumentCount)
			{
				actor.OutputHandler.Send($"The text #3{{{index.ToString("F0", actor)}}}#0 is not a valid inclusion in this emote.".SubstituteANSIColour());
				return false;
			}
		}

		foreach (Match match in PhaseEmoteRegexInvPlan.Matches(emoteText))
		{
			var index = int.Parse(match.Groups["index"].Value);
			if (index < 0 || index > phaseItems.Count)
			{
				actor.OutputHandler.Send($"The text #3$i{index.ToString("F0", actor)}#0 is not a valid inclusion in this emote as there are only {phaseItems.Count.ToString("N0", actor).ColourValue()} items.".SubstituteANSIColour());
				return false;
			}
		}

		phase.PhaseEmote = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The emote text for this phase is now {emoteText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPhaseAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should the phase last?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid number of seconds.");
			return false;
		}

		var time = TimeSpan.FromSeconds(value);

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the emote for this phase?");
			return false;
		}

		_phases.Add(new SurgicalProcedurePhase
		{
			BaseLength = time,
			PhaseEmote = command.SafeRemainingArgument,
			OnPhaseProg = null,
			InventoryPlanTemplate = null,
			PhaseSpecialEffects = null,
			PhaseSpecialEffectsDescription = null,
			PhaseSuccessful = (_, _, _) => true,
			WhyPhaseNotSuccessful = (_, _, _) => "an unknown reason"
		});
		Changed = true;
		actor.OutputHandler.Send($"You add a new surgical procedure phase (#{_phases.Count.ToString("N0", actor).ColourValue()}) lasting {time.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPhaseSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the first phase that you want to swap the order of?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value1) || value1 < 1)
		{
			actor.OutputHandler.Send(
				"Your value for the first phase is not a valid phase number.");
			return false;
		}

		var phase1 = _phases.ElementAtOrDefault(value1 - 1);
		if (phase1 is null)
		{
			actor.OutputHandler.Send(
				$"There is no such first phase. There are only {_phases.Count.ToString("N0", actor).ColourValue()} phases.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the second phase that you want to swap the order of?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value2) || value2 < 1)
		{
			actor.OutputHandler.Send(
				"Your value for the second phase is not a valid phase number.");
			return false;
		}

		var phase2 = _phases.ElementAtOrDefault(value2 - 1);
		if (phase2 is null)
		{
			actor.OutputHandler.Send(
				$"There is no such second phase. There are only {_phases.Count.ToString("N0", actor).ColourValue()} phases.");
			return false;
		}

		if (phase1 == phase2)
		{
			actor.OutputHandler.Send("You cannot swap a phase with itself.");
			return false;
		}

		_phases.Swap(phase1, phase2);
		Changed = true;
		actor.OutputHandler.Send($"You swap the order of phases {value1.ToString("N0", actor).ColourValue()} and {value2.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPhaseRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase would you like to remove?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1)
		{
			actor.OutputHandler.Send(
				"That is not a valid number of an existing phase.");
			return false;
		}

		var phase = _phases.ElementAtOrDefault(value - 1);
		if (phase is null)
		{
			actor.OutputHandler.Send(
				$"There is no such phase. There are only {_phases.Count.ToString("N0", actor).ColourValue()} phases.");
			return false;
		}

		actor.OutputHandler.Send($"Are you sure you want to permanently delete the {value.ToOrdinal().ColourValue()} phase? This action cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Deleting a phase from a surgical procedure",
			AcceptAction = text =>
			{
				_phases.Remove(phase);
				Changed = true;
				actor.OutputHandler.Send($"You permanently delete the {value.ToOrdinal().ColourValue()} phase of the surgical procedure.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the phase.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to delete the phase.");
			}
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandCheckTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait should this surgical procedure use for the check?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		CheckTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This surgery will now use the {trait.Name.ColourValue()} trait in place of any {"variable".ColourName()} variables in the associated check.");
		return true;
	}

	private bool BuildingCommandBaseCheckBonus(ICharacter actor, StringStack command)
	{
		var bonusPerLevel = (double)StandardCheck.BonusesPerDifficultyLevel;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter a number to be the bonus.\nEvery {bonusPerLevel.ToBonusString(actor)} of bonus correlates to one difficulty level.");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"That is not a valid number to be the bonus.\nEvery {bonusPerLevel.ToBonusString(actor)} of bonus correlates to one difficulty level.");
			return false;
		}

		BaseCheckBonus = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This surgery will now have a modification bonus of {value.ToBonusString(actor)} to its difficulty.");
		return true;
	}

	private bool BuildingCommandKnowledge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which knowledge should be required to know this surgical procedure?");
			return false;
		}

		var knowledge = Gameworld.Knowledges.GetByIdOrName(command.SafeRemainingArgument);
		if (knowledge is null)
		{
			actor.OutputHandler.Send("There is no such knowledge.");
			return false;
		}

		KnowledgeRequired = knowledge;
		Changed = true;
		actor.OutputHandler.Send($"This surgical procedure is now only known to those who have the {knowledge.Name.ColourName()} knowledge.");
		return true;
	}

	private bool BuildingCommandAbortProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove", "delete"))
		{
			AbortProg = null;
			Changed = true;
			actor.OutputHandler.Send("This procedure will no longer execute any prog upon being cancelled.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void,
			ParametersForCancelProg).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AbortProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This procedure will now execute the {prog.MXPClickableFunctionName()} prog upon cancellation of the procedure.");
		return true;
	}

	private bool BuildingCommandCompletionProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove", "delete"))
		{
			CompletionProg = null;
			Changed = true;
			actor.OutputHandler.Send("This procedure will no longer execute any prog upon completion.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void,
			ParametersForCompletionProg).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CompletionProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This procedure will now execute the {prog.MXPClickableFunctionName()} prog upon completion of the procedure.");
		return true;
	}

	private bool BuildingCommandWhyCannotUseProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Text,
			new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WhyCannotUseProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This procedure will now use the {prog.MXPClickableFunctionName()} prog to generate an error message about why it can't be used.");
		return true;
	}

	private bool BuildingComamandUsabilityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, 
			FutureProgVariableTypes.Boolean, 
			new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		UsabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This procedure will now use the {prog.MXPClickableFunctionName()} prog to control usability.");
		return true;
	}

	private bool BuildingCommandBeginEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the emote when this surgery is begun? Use $0 for the surgeon and $1 for the patient.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		ProcedureBeginEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The emote when starting this surgery is now {ProcedureBeginEmote.ColourCommand().Fullstop()}");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send($@"You are replacing the following text:

{ProcedureDescription.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}

Enter your new description in the editor below:");
		actor.EditorMode(PostAction, CancelAction, 1.0, suppliedArguments: new object[]{ actor.InnerLineFormatLength});
		return true;
	}

	private void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the procedure's description.");
	}

	private void PostAction(string text, IOutputHandler handler, object[] args)
	{
		ProcedureDescription = text.Trim().Fullstop();
		Changed = true;
		handler.Send($"You change the description of this procedure to:\n\n{ProcedureDescription.SubstituteANSIColour().Wrap((int)args[0])}");
	}

	private bool BuildingCommandGerund(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set as the procedure gerund (-ing word or phrase) for describing this procedure?");
			return false;
		}

		ProcedureGerund = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"The gerund for this procedure is now {ProcedureGerund.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSchool(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var existing = Gameworld.SurgicalProcedures.Select(x => x.MedicalSchool.TitleCase().ColourName()).Distinct().ToArray();
			if (existing.Any())
			{
				actor.OutputHandler.Send($"Which school should this surgical procedure belong to?\nThere are the following existing schools: {existing.ListToString()}");
				return false;
			}
			actor.OutputHandler.Send("Which school should this surgical procedure belong to?");
			return false;
		}

		MedicalSchool = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"This surgical procedure now belongs to the {MedicalSchool.ColourName()} school.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What you like to rename this surgical procedure to?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.SurgicalProcedures.Except(this).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a surgical procedure called {name.ColourName()}, names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this procedure from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandProcedureName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What you like to set as the procedure name for this surgical procedure?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant();
		if (Gameworld.SurgicalProcedures.Except(this).Any(x => x.ProcedureName.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a surgical procedure with the procedure name {name.ColourName()}, procedure names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You set the procedure name of this procedure to {name.ColourName()}.");
		ProcedureName = name;
		Changed = true;
		return true;
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Surgical Procedure - {Name} (#{Id.ToString("N0", actor)})".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Procedure Name: {ProcedureName.ColourValue()}");
		sb.AppendLine($"Procedure Type: {Procedure.DescribeEnum(true, Telnet.Green)}");
		sb.AppendLine($"Gerund: {ProcedureGerund.ColourValue()}");
		sb.AppendLine($"Target Body: {TargetBodyType.Name.ColourValue()}");
		sb.AppendLine($"School: {MedicalSchool.ColourValue()}");
		sb.AppendLine($"Knowledge Required: {KnowledgeRequired?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Check: {Check.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Trait: {CheckTrait?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Base Bonus: {BaseCheckBonus.ToBonusString(actor)}");
		sb.AppendLine();
		sb.AppendLine($"Usability Prog: {UsabilityProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Why Can't Use Prog: {WhyCannotUseProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Complete Prog: {CompletionProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Cancel Prog: {AbortProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine($"Requires Unconscious Patient: {RequiresUnconsciousPatient.ToColouredString()}");
		sb.AppendLine($"Requires Living Patient: {RequiresLivingPatient.ToColouredString()}");
		sb.AppendLine($"Requires Invasive Finalisation: {RequiresInvasiveProcedureFinalisation.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine($"Start Emote: {ProcedureBeginEmote.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Description".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(ProcedureDescription.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Phases".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		var i = 1;
		foreach (var phase in Phases)
		{
			sb.AppendLine();
			sb.AppendLine($"Phase #{i++.ToString("N0", actor)}".Colour(Telnet.Magenta));
			sb.AppendLine($"Length: {phase.BaseLength.Describe(actor).ColourValue()}");
			sb.AppendLine($"Prog: {phase.OnPhaseProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
			sb.AppendLine($"Emote: {phase.PhaseEmote.ColourCommand()}");
			sb.AppendLine($"Special: {phase.PhaseSpecialEffectsDescription}");
			sb.AppendLine("Inventory Plan:");
			var j = 0;
			foreach (var action in phase.InventoryPlanTemplate?.Phases.First().Actions ?? Enumerable.Empty<IInventoryPlanAction>())
			{
				sb.AppendLine($"\t{(++j).ToString("N0", actor)}) {action.Describe(actor)}");
			}
		}
		return sb.ToString();
	}
}