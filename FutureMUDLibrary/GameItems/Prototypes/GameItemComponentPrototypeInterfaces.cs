using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.GameItems.Interfaces;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public interface IGameItemComponentPrototype<TComponent> : IGameItemComponentProto
	where TComponent : IGameItemComponent
{
}

public interface IExclusiveGameItemComponentPrototype<TComponent> : IGameItemComponentPrototype<TComponent>
	where TComponent : IGameItemComponent
{
}

public interface IAggregateGameItemComponentPrototype<TComponent> : IGameItemComponentPrototype<TComponent>
	where TComponent : IGameItemComponent
{
}

public interface IActiveCraftGameItemComponentPrototype : IExclusiveGameItemComponentPrototype<IActiveCraftGameItemComponent>
{
}

public interface IAffectQualityPrototype : IAggregateGameItemComponentPrototype<IAffectQuality>
{
}

public interface IAmmoPrototype : IExclusiveGameItemComponentPrototype<IAmmo>
{
}

public interface IAmmoClipPrototype : IExclusiveGameItemComponentPrototype<IAmmoClip>, IContainerPrototype
{
}

public interface IAnsweringMachinePrototype : IExclusiveGameItemComponentPrototype<IAnsweringMachine>, ITelephonePrototype
{
}

public interface IApplyPrototype : IExclusiveGameItemComponentPrototype<IApply>
{
}

public interface IArmourPrototype : IExclusiveGameItemComponentPrototype<IArmour>
{
}

public interface IAudioStorageTapePrototype : IExclusiveGameItemComponentPrototype<IAudioStorageTape>
{
}

public interface IAutomationHousingPrototype : IExclusiveGameItemComponentPrototype<IAutomationHousing>
{
}

public interface IAutomationMountablePrototype : IExclusiveGameItemComponentPrototype<IAutomationMountable>
{
}

public interface IAutomationMountHostPrototype : IExclusiveGameItemComponentPrototype<IAutomationMountHost>
{
}

public interface IBankPaymentItemPrototype : IExclusiveGameItemComponentPrototype<IBankPaymentItem>
{
}

public interface IBatteryPrototype : IExclusiveGameItemComponentPrototype<IBattery>
{
}

public interface IBeltPrototype : IExclusiveGameItemComponentPrototype<IBelt>
{
}

public interface IBeltablePrototype : IExclusiveGameItemComponentPrototype<IBeltable>
{
}

public interface IBlindfoldPrototype : IExclusiveGameItemComponentPrototype<IBlindfold>
{
}

public interface IBoardItemPrototype : IExclusiveGameItemComponentPrototype<IBoardItem>
{
}

public interface IButcherablePrototype : IExclusiveGameItemComponentPrototype<IButcherable>
{
}

public interface ICanConnectToElectricalGridPrototype : IAggregateGameItemComponentPrototype<ICanConnectToElectricalGrid>, ICanConnectToGridPrototype
{
}

public interface ICanConnectToGridPrototype : IAggregateGameItemComponentPrototype<ICanConnectToGrid>
{
}

public interface ICanConnectToLiquidGridPrototype : IAggregateGameItemComponentPrototype<ICanConnectToLiquidGrid>, ICanConnectToGridPrototype
{
}

public interface ICanConnectToTelecommunicationsGridPrototype : IAggregateGameItemComponentPrototype<ICanConnectToTelecommunicationsGrid>, ICanConnectToGridPrototype
{
}

public interface ICannulaPrototype : IExclusiveGameItemComponentPrototype<ICannula>, IConnectablePrototype, IImplantPrototype
{
}

public interface ICellPhoneTowerPrototype : IExclusiveGameItemComponentPrototype<ICellPhoneTower>, IConsumePowerPrototype, IOnOffPrototype
{
}

public interface IChairPrototype : IExclusiveGameItemComponentPrototype<IChair>
{
}

public interface IChangeCharacteristicsPrototype : IExclusiveGameItemComponentPrototype<IChangeCharacteristics>
{
}

public interface IChangeTraitsInInventoryPrototype : IExclusiveGameItemComponentPrototype<IChangeTraitsInInventory>
{
}

public interface ICommodityPrototype : IExclusiveGameItemComponentPrototype<ICommodity>
{
}

public interface IConnectablePrototype : IAggregateGameItemComponentPrototype<IConnectable>
{
}

public interface IConsumePowerPrototype : IAggregateGameItemComponentPrototype<IConsumePower>
{
}

public interface IContainerPrototype : IExclusiveGameItemComponentPrototype<IContainer>
{
}

public interface ICorpsePrototype : IExclusiveGameItemComponentPrototype<ICorpse>, IButcherablePrototype
{
}

public interface ICorrectMyopiaPrototype : IExclusiveGameItemComponentPrototype<ICorrectMyopia>
{
}

public interface ICrutchPrototype : IExclusiveGameItemComponentPrototype<ICrutch>
{
}

public interface ICurrencyPilePrototype : IExclusiveGameItemComponentPrototype<ICurrencyPile>
{
}

public interface IDefibrillatorPrototype : IExclusiveGameItemComponentPrototype<IDefibrillator>, IConsumePowerPrototype
{
}

public interface IDestroyablePrototype : IExclusiveGameItemComponentPrototype<IDestroyable>
{
}

public interface IDetonatablePrototype : IExclusiveGameItemComponentPrototype<IDetonatable>
{
}

public interface IDicePrototype : IExclusiveGameItemComponentPrototype<IDice>
{
}

public interface IDoorPrototype : IExclusiveGameItemComponentPrototype<IDoor>, IOpenablePrototype, ILockablePrototype
{
}

public interface IDragAidPrototype : IExclusiveGameItemComponentPrototype<IDragAid>
{
}

public interface IDripPrototype : IExclusiveGameItemComponentPrototype<IDrip>, IConnectablePrototype
{
}

public interface IEdiblePrototype : IExclusiveGameItemComponentPrototype<IEdible>
{
}

public interface IExternalBloodOxygenatorPrototype : IExclusiveGameItemComponentPrototype<IExternalBloodOxygenator>, IConnectablePrototype
{
}

public interface IExternalOrganFunctionPrototype : IExclusiveGameItemComponentPrototype<IExternalOrganFunction>, IConnectablePrototype
{
}

public interface IFaxMachinePrototype : IExclusiveGameItemComponentPrototype<IFaxMachine>, ITelephonePrototype
{
}

public interface IFileSignalGeneratorPrototype : IExclusiveGameItemComponentPrototype<IFileSignalGenerator>, ISignalSourceComponentPrototype
{
}

public interface IFlipPrototype : IExclusiveGameItemComponentPrototype<IFlip>
{
}

public interface IGagPrototype : IExclusiveGameItemComponentPrototype<IGag>
{
}

public interface IGasContainerPrototype : IExclusiveGameItemComponentPrototype<IGasContainer>
{
}

public interface IGasSupplyPrototype : IExclusiveGameItemComponentPrototype<IGasSupply>, IConnectablePrototype
{
}

public interface IHoldablePrototype : IExclusiveGameItemComponentPrototype<IHoldable>
{
}

public interface IImmobilisePrototype : IExclusiveGameItemComponentPrototype<IImmobilise>
{
}

public interface IImplantPrototype : IExclusiveGameItemComponentPrototype<IImplant>, IConsumePowerPrototype
{
}

public interface IImplantMeleeWeaponPrototype : IExclusiveGameItemComponentPrototype<IImplantMeleeWeapon>, IImplantPrototype, IMeleeWeaponPrototype
{
}

public interface IImplantNeuralLinkPrototype : IExclusiveGameItemComponentPrototype<IImplantNeuralLink>, IImplantPrototype
{
}

public interface IImplantPowerPlantPrototype : IExclusiveGameItemComponentPrototype<IImplantPowerPlant>, IImplantPrototype
{
}

public interface IImplantPowerSupplyPrototype : IExclusiveGameItemComponentPrototype<IImplantPowerSupply>, IProducePowerPrototype
{
}

public interface IImplantReportStatusPrototype : IExclusiveGameItemComponentPrototype<IImplantReportStatus>, IImplantPrototype
{
}

public interface IImplantRespondToCommandsPrototype : IExclusiveGameItemComponentPrototype<IImplantRespondToCommands>, IImplantPrototype
{
}

public interface IImplantTraitChangePrototype : IExclusiveGameItemComponentPrototype<IImplantTraitChange>, IImplantPrototype
{
}

public interface IInjectPrototype : IExclusiveGameItemComponentPrototype<IInject>
{
}

public interface IInsertablePrototype : IExclusiveGameItemComponentPrototype<IInsertable>
{
}

public interface IInsulatingPrototype : IExclusiveGameItemComponentPrototype<IInsulating>
{
}

public interface IJammableWeaponPrototype : IExclusiveGameItemComponentPrototype<IJammableWeapon>, IRangedWeaponPrototype
{
}

public interface IKeyPrototype : IExclusiveGameItemComponentPrototype<IKey>
{
}

public interface IKeyringPrototype : IExclusiveGameItemComponentPrototype<IKeyring>, IKeyPrototype
{
}

public interface ILaserPowerPackPrototype : IExclusiveGameItemComponentPrototype<ILaserPowerPack>
{
}

public interface ILightablePrototype : IExclusiveGameItemComponentPrototype<ILightable>
{
}

public interface ILiquidContainerPrototype : IExclusiveGameItemComponentPrototype<ILiquidContainer>, IOpenablePrototype
{
}

public interface ILiquidGridSupplierPrototype : IExclusiveGameItemComponentPrototype<ILiquidGridSupplier>
{
}

public interface IListablePrototype : IExclusiveGameItemComponentPrototype<IListable>
{
}

public interface ILockPrototype : IExclusiveGameItemComponentPrototype<ILock>
{
}

public interface ILockablePrototype : IExclusiveGameItemComponentPrototype<ILockable>
{
}

public interface ILocksmithingToolPrototype : IExclusiveGameItemComponentPrototype<ILocksmithingTool>
{
}

public interface ILodgeConsequencePrototype : IExclusiveGameItemComponentPrototype<ILodgeConsequence>
{
}

public interface IMarketGoodWeightItemPrototype : IExclusiveGameItemComponentPrototype<IMarketGoodWeightItem>
{
}

public interface IMeleeWeaponPrototype : IExclusiveGameItemComponentPrototype<IMeleeWeapon>, IWieldablePrototype
{
}

public interface IMicrocontrollerPrototype : IExclusiveGameItemComponentPrototype<IMicrocontroller>, ISignalSourceComponentPrototype, IConsumePowerPrototype, ISwitchablePrototype, IOnOffPrototype
{
}

public interface INaturalResistancePrototype : IExclusiveGameItemComponentPrototype<INaturalResistance>
{
}

public interface IObscureCharacteristicsPrototype : IExclusiveGameItemComponentPrototype<IObscureCharacteristics>
{
}

public interface IObscureIdentityPrototype : IExclusiveGameItemComponentPrototype<IObscureIdentity>, IObscureCharacteristicsPrototype
{
}

public interface IOnOffPrototype : IAggregateGameItemComponentPrototype<IOnOff>
{
}

public interface IOpenablePrototype : IExclusiveGameItemComponentPrototype<IOpenable>
{
}

public interface IOrganImplantPrototype : IExclusiveGameItemComponentPrototype<IOrganImplant>, IImplantPrototype
{
}

public interface IPreparedFoodPrototype : IExclusiveGameItemComponentPrototype<IPreparedFood>, IEdiblePrototype
{
}

public interface IProduceHeatPrototype : IAggregateGameItemComponentPrototype<IProduceHeat>
{
}

public interface IProduceLightPrototype : IExclusiveGameItemComponentPrototype<IProduceLight>
{
}

public interface IProducePowerPrototype : IAggregateGameItemComponentPrototype<IProducePower>
{
}

public interface IProstheticPrototype : IExclusiveGameItemComponentPrototype<IProsthetic>
{
}

public interface IProstheticMeleeWeaponPrototype : IExclusiveGameItemComponentPrototype<IProstheticMeleeWeapon>, IMeleeWeaponPrototype, IProstheticPrototype
{
}

public interface IProvideCoverPrototype : IExclusiveGameItemComponentPrototype<IProvideCover>
{
}

public interface IProvideGasForBreathingPrototype : IExclusiveGameItemComponentPrototype<IProvideGasForBreathing>
{
}

public interface IPuffablePrototype : IExclusiveGameItemComponentPrototype<IPuffable>
{
}

public interface IQualityByTagPrototype : IExclusiveGameItemComponentPrototype<IQualityByTag>
{
}

public interface IRangedWeaponPrototype : IExclusiveGameItemComponentPrototype<IRangedWeapon>, IWieldablePrototype
{
}

public interface IRangedWeaponWithUnreadyEventPrototype : IExclusiveGameItemComponentPrototype<IRangedWeaponWithUnreadyEvent>, IRangedWeaponPrototype
{
}

public interface IReadablePrototype : IExclusiveGameItemComponentPrototype<IReadable>
{
}

public interface IReceivePrototype : IExclusiveGameItemComponentPrototype<IReceive>
{
}

public interface IRepairKitPrototype : IExclusiveGameItemComponentPrototype<IRepairKit>
{
}

public interface IRespondToSignalPrototype : IExclusiveGameItemComponentPrototype<IRespondToSignal>
{
}

public interface IRestraintPrototype : IExclusiveGameItemComponentPrototype<IRestraint>
{
}

public interface IRuntimeConfigurableSignalSinkComponentPrototype : IAggregateGameItemComponentPrototype<IRuntimeConfigurableSignalSinkComponent>, ISignalSinkComponentPrototype
{
}

public interface IRuntimeProgrammableMicrocontrollerPrototype : IExclusiveGameItemComponentPrototype<IRuntimeProgrammableMicrocontroller>, IMicrocontrollerPrototype
{
}

public interface ISelectablePrototype : IExclusiveGameItemComponentPrototype<ISelectable>
{
}

public interface ISeveredBodypartPrototype : IExclusiveGameItemComponentPrototype<ISeveredBodypart>, IContainerPrototype, IButcherablePrototype
{
}

public interface ISharpenPrototype : IExclusiveGameItemComponentPrototype<ISharpen>
{
}

public interface ISheathPrototype : IExclusiveGameItemComponentPrototype<ISheath>
{
}

public interface IShieldPrototype : IExclusiveGameItemComponentPrototype<IShield>, IWieldablePrototype
{
}

public interface IShopStallPrototype : IExclusiveGameItemComponentPrototype<IShopStall>, IContainerPrototype
{
}

public interface ISignalCableSegmentPrototype : IExclusiveGameItemComponentPrototype<ISignalCableSegment>, ISignalSourceComponentPrototype
{
}

public interface ISignalSinkComponentPrototype : IAggregateGameItemComponentPrototype<ISignalSinkComponent>
{
}

public interface ISignalSourceComponentPrototype : IAggregateGameItemComponentPrototype<ISignalSourceComponent>
{
}

public interface ISmokeablePrototype : IExclusiveGameItemComponentPrototype<ISmokeable>, ILightablePrototype
{
}

public interface IStableTicketPrototype : IExclusiveGameItemComponentPrototype<IStableTicket>
{
}

public interface IStackablePrototype : IExclusiveGameItemComponentPrototype<IStackable>
{
}

public interface ISwallowablePrototype : IExclusiveGameItemComponentPrototype<ISwallowable>
{
}

public interface ISwitchablePrototype : IAggregateGameItemComponentPrototype<ISwitchable>
{
}

public interface ITablePrototype : IExclusiveGameItemComponentPrototype<ITable>
{
}

public interface ITearablePrototype : IExclusiveGameItemComponentPrototype<ITearable>
{
}

public interface ITelephonePrototype : IExclusiveGameItemComponentPrototype<ITelephone>, ITransmitPrototype, IReceivePrototype, IConsumePowerPrototype, ISwitchablePrototype, IOnOffPrototype
{
}

public interface ITelephoneNumberOwnerPrototype : IAggregateGameItemComponentPrototype<ITelephoneNumberOwner>
{
}

public interface ITimePiecePrototype : IExclusiveGameItemComponentPrototype<ITimePiece>
{
}

public interface IToolItemPrototype : IExclusiveGameItemComponentPrototype<IToolItem>
{
}

public interface ITransmitPrototype : IExclusiveGameItemComponentPrototype<ITransmit>
{
}

public interface ITreatmentPrototype : IExclusiveGameItemComponentPrototype<ITreatment>
{
}

public interface ITurnablePrototype : IExclusiveGameItemComponentPrototype<ITurnable>
{
}

public interface IVariablePrototype : IExclusiveGameItemComponentPrototype<IVariable>
{
}

public interface IVendingMachinePrototype : IExclusiveGameItemComponentPrototype<IVendingMachine>, ISelectablePrototype, IListablePrototype
{
}

public interface IWearablePrototype : IExclusiveGameItemComponentPrototype<IWearable>
{
}

public interface IWieldablePrototype : IExclusiveGameItemComponentPrototype<IWieldable>
{
}

public interface IWriteablePrototype : IExclusiveGameItemComponentPrototype<IWriteable>
{
}

public interface IWritingImplementPrototype : IExclusiveGameItemComponentPrototype<IWritingImplement>
{
}

public interface IZeroGravityAnchorItemPrototype : IExclusiveGameItemComponentPrototype<IZeroGravityAnchorItem>
{
}

public interface IZeroGravityPropulsionPrototype : IExclusiveGameItemComponentPrototype<IZeroGravityPropulsion>
{
}

public interface IZeroGravityTetherItemPrototype : IExclusiveGameItemComponentPrototype<IZeroGravityTetherItem>
{
}
