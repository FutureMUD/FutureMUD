using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Climate;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Form.Audio;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.RPG.Merits;

namespace MudSharp.Body {
	public partial interface IBody : IInventory, IHaveBodyparts, IEditableNameData,
		ILanguagePerceiver, IPerceivableHaveCharacteristics, IManipulator, IHaveRace, IHavePhysicalDimensions, IHaveNeeds, IEat,
		IHaveStamina, IMortal, IMortalPerceiver, IHaveMerits, ILateInitialisingItem, IBreathe, IHaveContextualSizeCategory, IHavePositionalSizes, ITarget
	{
		void LoadInventory(MudSharp.Models.Body body);
		bool InventoryChanged { get; set; }
		IEnumerable<IEntityDescriptionPattern> EntityDescriptionPatterns { get; }
		(string ShortDescription, string FullDescription) GetRawDescriptions { get; }
		void SetFullDescription(string description);
		void SetShortDescription(string description);
		IBodyPrototype Prototype { get; }

		ICharacter Actor { get; set; }

		IController Controller { get; }

		IWearableSizeRules SizeRules { get; }

		Dictionary<IPositionState, IMoveSpeed> CurrentSpeeds { get; }
		IEnumerable<IMoveSpeed> Speeds { get; }

		double CurrentBloodVolumeLitres { get; set; }
		double TotalBloodVolumeLitres { get; set; }
		ILiquid BloodLiquid { get; }
		IBloodtype Bloodtype { get; }
		double BaseLiverAlcoholRemovalKilogramsPerHour { get; set; }
		double LiverAlcoholRemovalKilogramsPerHour { get; }
		double WaterLossLitresPerHour { get; }
		double CaloricConsumptionPerHour { get; }

		double ImmuneFatigueBonus { get; }
		IEnumerable<IInfection> PartInfections { get; }

		IEnumerable<DrugDosage> ActiveDrugDosages { get; }
		IEnumerable<DrugDosage> LatentDrugDosages { get; }

		IEnumerable<ITattoo> Tattoos { get; }
		void AddTattoo(ITattoo tattoo);
		void RemoveTattoo(ITattoo tattoo);
		bool TattoosChanged { get; set; }

		IEnumerable<IScar> Scars { get; }
		void AddScar(IScar scar);
		void RemoveScar(IScar scar);
		bool ScarsChanged { get; set; }

		void Look(bool fromMovement = false);
		void LookIn(IPerceivable thing);
		void Look(IPerceivable thing);
		void LookTattoos(ICharacter actor, IBodypart forBodypart = null);
		void LookScars(ICharacter actor, IBodypart forBodypart = null);
		void LookWounds(IMortalPerceiver thing);
		void LookGraffiti(string target);
		void LookGraffitiThing(IGameItem item, string target);

		string LookText(bool fromMovement = false);
		string LookText(IPerceivable thing, bool fromLookCommand = false);
		string LookInText(IPerceivable thing);
		string LookTattoosText(ICharacter actor, IBodypart forBodypart = null);
		string LookWoundsText(IMortalPerceiver thing);
		string LookGraffitiText(string target);
		string LookGraffitiThingText(IGameItem item, string target);

		string ReportCondition();

		void Register(IController controller);

		/// <summary>
		///     Tells the body to "Quit" the game, remove itself gracefully from the game world
		/// </summary>
		/// <returns></returns>
		void Quit();

		string GetConsiderString(IPerceiver voyeur);
		string GetPositionDescription(IPerceiver voyeur, bool proper, bool colour, PerceiveIgnoreFlags flags);
		bool CanSee(ICell thing, ICellExit exit, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
		void AddInfection(IInfection infection);
		void RemoveInfection(IInfection infection);
		void Dose(IDrug drug, DrugVector vector, double grams);
		void CheckDrugTick();
		void Sober();
		string DebugInfo();
		void Login();
		void DoOfflineHealing(TimeSpan timePassed);

		double OrganFunction<T>() where T : IOrganProto;
		double OrganFunction(IOrganProto organ);
		void GenderChanged();
		IEnumerable<IWound> InventoryExploded(IGameItem item, IExplosiveDamage damage);
		(double Lower, double Upper) TolerableTemperatures(bool includeClothing);
		IEnumerable<T> CombinedEffectsOfType<T>() where T : class, IEffect;
		void CheckPositionStillValid();
		(List<ILimb> WorkingLegs, List<ILimb> NonWorkingLegs) GetLegInformation(bool ignoreAids);
		(List<ILimb> WorkingAppendages, List<ILimb> NonWorkingAppendages) GetArmAndAppendagesInformation();
		(double Coating, double Absorb) LiquidAbsorbtionAmounts { get; }
		ItemSaturationLevel SaturationLevel { get; }
		ItemSaturationLevel SaturationLevelForLiquid(LiquidInstance instance);
		ItemSaturationLevel SaturationLevelForLiquid(double total);
		ItemSaturationLevel SaturationLevelForLiquid(IEnumerable<IExternalBodypart> bodyparts);
		void ExposeToLiquid(LiquidMixture mixture, IBodypart part, LiquidExposureDirection direction);
		void ExposeToLiquid(LiquidMixture mixture, IEnumerable<IExternalBodypart> parts, LiquidExposureDirection direction);
		void ExposeToPrecipitation(PrecipitationLevel level, ILiquid liquid);
	}
}