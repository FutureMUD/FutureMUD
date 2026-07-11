using MudSharp.Economy.Currency;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.GameItem;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.GameItems;

public partial class GameItem
{
    #region IFutureProgVariable Members

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "effects", ProgVariableTypes.Collection | ProgVariableTypes.Effect },
            { "name", ProgVariableTypes.Text },
            { "type", ProgVariableTypes.Text },
            { "proto", ProgVariableTypes.Number },
            { "quantity", ProgVariableTypes.Number },
            { "holder", ProgVariableTypes.Character },
            { "wearer", ProgVariableTypes.Character },
            { "owner", OwnershipFunctionHelpers.OwnerVariableType },
            { "hasowner", ProgVariableTypes.Boolean },
            { "ownerid", ProgVariableTypes.Number },
            { "ownertype", ProgVariableTypes.Text },
            { "weight", ProgVariableTypes.Number },
            { "contents", ProgVariableTypes.Collection | ProgVariableTypes.Item },
            { "container", ProgVariableTypes.Item },
            { "iscontainer", ProgVariableTypes.Boolean },
            { "isopenable", ProgVariableTypes.Boolean },
            { "iscurrency", ProgVariableTypes.Boolean },
            { "islock", ProgVariableTypes.Boolean },
            { "islockable", ProgVariableTypes.Boolean },
            { "iskey", ProgVariableTypes.Boolean },
            { "istable", ProgVariableTypes.Boolean },
            { "ischair", ProgVariableTypes.Boolean },
            { "isdoor", ProgVariableTypes.Boolean },
            { "isbelt", ProgVariableTypes.Boolean },
            { "isbeltable", ProgVariableTypes.Boolean },
            { "iswearable", ProgVariableTypes.Boolean },
            { "iswieldable", ProgVariableTypes.Boolean },
            { "isholdable", ProgVariableTypes.Boolean },
            { "issheath", ProgVariableTypes.Boolean },
            { "islightable", ProgVariableTypes.Boolean },
            { "ispowered", ProgVariableTypes.Boolean },
            { "ison", ProgVariableTypes.Boolean },
            { "iscover", ProgVariableTypes.Boolean },
            { "iscorpse", ProgVariableTypes.Boolean },
            { "isweapon", ProgVariableTypes.Boolean },
            { "ismeleeweapon", ProgVariableTypes.Boolean },
            { "israngedweapon", ProgVariableTypes.Boolean },
            { "providingcover", ProgVariableTypes.Boolean },
            { "lit", ProgVariableTypes.Boolean },
            { "open", ProgVariableTypes.Boolean },
            { "locked", ProgVariableTypes.Boolean },
            { "locks", ProgVariableTypes.Collection | ProgVariableTypes.Item },
            { "corpsecharacter", ProgVariableTypes.Character },
            { "corpsebodyid", ProgVariableTypes.Number },
            { "corpseremainscontext", ProgVariableTypes.Text },
            { "corpsefinaldeath", ProgVariableTypes.Boolean },
            { "location", ProgVariableTypes.Location },
            { "tags", ProgVariableTypes.Collection | ProgVariableTypes.Text },
            { "iscommodity", ProgVariableTypes.Boolean },
            { "material", ProgVariableTypes.Material },
            { "isgridconnectable", ProgVariableTypes.Boolean },
            { "iselectricgridconnectable", ProgVariableTypes.Boolean },
            { "istelecommunicationsgridconnectable", ProgVariableTypes.Boolean },
            { "istelephone", ProgVariableTypes.Boolean },
            { "iscellularphone", ProgVariableTypes.Boolean },
            { "iscellphonetower", ProgVariableTypes.Boolean },
            { "phonenumber", ProgVariableTypes.Text },
            { "grid", ProgVariableTypes.Number },
            { "layer", ProgVariableTypes.Text },
            { "isfood", ProgVariableTypes.Boolean },
            { "isliquidcontainer", ProgVariableTypes.Boolean },
            { "issealstamp", ProgVariableTypes.Boolean },
            { "issealable", ProgVariableTypes.Boolean },
            { "issealed", ProgVariableTypes.Boolean },
            { "sealbroken", ProgVariableTypes.Boolean },
            { "sealresidue", ProgVariableTypes.Boolean },
            { "sealdesign", ProgVariableTypes.Text },
            { "sealissuer", ProgVariableTypes.Text },
            { "sealowner", ProgVariableTypes.Text },
            { "sealclan", ProgVariableTypes.Text },
            { "sealoffice", ProgVariableTypes.Text },
            { "sealmaterial", ProgVariableTypes.Text },
            { "sealmedium", ProgVariableTypes.Text },
            { "sealingcharacterid", ProgVariableTypes.Number },
            { "ismeasuringinstrument", ProgVariableTypes.Boolean },
            { "measuringmode", ProgVariableTypes.Text },
            { "calibrationbias", ProgVariableTypes.Number },
            { "calibrationbiasispercent", ProgVariableTypes.Boolean },
            { "calibrationdeliberate", ProgVariableTypes.Boolean },
            { "usessincecalibration", ProgVariableTypes.Number },
            { "variables", ProgVariableTypes.Dictionary | ProgVariableTypes.Text},
            { "condition", ProgVariableTypes.Number },
            { "quality", ProgVariableTypes.Number },
            { "qualityname", ProgVariableTypes.Text  },
            { "rawquality", ProgVariableTypes.Number },
            { "rawqualityname", ProgVariableTypes.Text  }
        };
    }

    private static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "proto", "The ID number of the proto this item was loaded from" },
            { "quantity", "The quantity of this item in the stack. 1 if not stacked" },
            { "holder", "The character who is holding this item, if any" },
            { "wearer", "The character who is wearing this item, if any" },
            { "owner", "The registered owner of this item when its owner has a native FutureProg variable type." },
            { "hasowner", "True if this item has a durable registered owner reference." },
            { "ownerid", "The ID of this item's registered owner, or 0 if unowned." },
            { "ownertype", "The framework-item type of this item's registered owner, or empty text if unowned." },
            { "weight", "The weight of this item in base units" },
            { "contents", "A collection of the item contents of this item" },
            { "container", "The item that this item is contained in, if any" },
            { "iscontainer", "True if the item is a container" },
            { "isopenable", "True is the item can be opened and closed" },
            { "iscurrency", "True if the item is a currency pile" },
            { "islock", "True if the item is a lock" },
            { "islockable", "True if the item can be locked and unlocked" },
            { "iskey", "True if the item is a key" },
            { "istable", "True if the item is a table" },
            { "ischair", "True if the item is a chair" },
            { "isdoor", "True if the item is a door" },
            { "isbelt", "True if the item is a belt" },
            { "isbeltable", "True if the item can be attached to a belt" },
            { "iswearable", "True if the item can be worn" },
            { "iswieldable", "True if the item can be wielded" },
            { "isholdable", "True if the item can be held" },
            { "issheath", "True if the item is a weapon sheath" },
            { "islightable", "True if the item can be lit with the light command" },
            { "ispowered", "True if the item is electrically powered" },
            { "ison", "True if the item is on" },
            { "iscover", "True if the item can be used as ranged cover" },
            { "iscorpse", "True if the item is a corpse" },
            { "isweapon", "True if the item is a weapon of any kind" },
            { "ismeleeweapon", "True if the item is a melee weapon" },
            { "israngedweapon", "True if the item is a ranged weapon" },
            { "providingcover", "True if the item is currently providing cover" },
            { "lit", "True if the item is currently lit (e.g. by the light command)" },
            { "open", "True if the item is currently open" },
            { "locked", "True if the item is currently locked" },
            { "locks", "A collection of the locks on this item" },
            { "corpsecharacter", "If the item is a corpse, this is the original character" },
            { "corpsebodyid", "If the item is a corpse, this is the original body ID represented by the remains" },
            { "corpseremainscontext", "If the item is a corpse, this is the body-remains context" },
            { "corpsefinaldeath", "True if the item is a corpse from a final character death rather than abandoned body remains" },
            { "location", "The room that the item is in" },
            { "tags", "A collection of the tags that this item has" },
            { "iscommodity", "True if the item is a commodity pile" },
            { "material", "The primary material that this item is made from" },
            { "isgridconnectable", "True if this item can be connected to a grid" },
            { "iselectricgridconnectable", "True if this item can be connected to an electrical grid" },
            { "istelecommunicationsgridconnectable", "True if this item can be connected to a telecommunications grid" },
            { "istelephone", "True if this item is or hosts a telephone" },
            { "iscellularphone", "True if this item is or hosts a cellular phone" },
            { "iscellphonetower", "True if this item is or hosts a cell phone tower" },
            { "phonenumber", "The active telephone number assigned to this item or its connected telecom endpoint, if any" },
            { "grid", "The grid that this item is connected to, if any" },
            { "layer", "A text description of the layer this item is currently in" },
            { "isfood", "True if the item is food" },
            { "isliquidcontainer", "True if the item is a liquid container" },
            { "issealstamp", "True if the item is a seal stamp" },
            { "issealable", "True if the item can hold a seal impression" },
            { "issealed", "True if the item currently has an intact seal" },
            { "sealbroken", "True if the item has had a seal broken" },
            { "sealresidue", "True if the item has broken seal residue" },
            { "sealdesign", "The design text of the current or broken seal impression, if any" },
            { "sealissuer", "The issuer metadata of the current or broken seal impression, if any" },
            { "sealowner", "The owner metadata of the current or broken seal impression, if any" },
            { "sealclan", "The clan metadata of the current or broken seal impression, if any" },
            { "sealoffice", "The office metadata of the current or broken seal impression, if any" },
            { "sealmaterial", "The stamp material metadata of the current or broken seal impression, if any" },
            { "sealmedium", "The sealing medium description of the current or broken seal impression, if any" },
            { "sealingcharacterid", "The ID of the character who applied the current or broken seal impression, if any" },
            { "ismeasuringinstrument", "True if the item is a measuring instrument" },
            { "measuringmode", "The measuring mode of the measuring instrument, if any" },
            { "calibrationbias", "The current deliberate calibration bias in base units or percent depending on calibrationbiasispercent" },
            { "calibrationbiasispercent", "True if calibrationbias is a percentage rather than a base-unit amount" },
            { "calibrationdeliberate", "True if the current calibration has a deliberate wrong-measure bias" },
            { "usessincecalibration", "The number of measurements made since the instrument was last calibrated" },
            { "variables", "Returns a dictionary of variable names and variable values"},
            { "condition", "A value from 0.0 to 1.0 representing the current condition percentage of the item" },
            { "quality", "The quality of the item as a number between 0 (terrible) and 11 (legendary)" },
            { "qualityname", "The name of the quality level of the item (e.g. Terrible, ExtremelyBad, Bad, Poor, Substandard, Standard, Good, Very Good, Great, Excellent, Heroic, Legendary)" },
            { "rawquality", "The raw (base) quality of the item as a number between 0 (terrible) and 11 (legendary)" },
            { "rawqualityname", "The name of the raw (base) quality level of the item (e.g. Terrible, ExtremelyBad, Bad, Poor, Substandard, Standard, Good, Very Good, Great, Excellent, Heroic, Legendary)" }
        };
    }

    private static IReadOnlyDictionary<string, ProgVariableTypes> TaggedDotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "tags", ProgVariableTypes.Collection | ProgVariableTypes.Text }
        };
    }

    private static IReadOnlyDictionary<string, string> TaggedDotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "tags", "A collection of the tags that this thing has" }
        };
    }

    public new static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Item, DotReferenceHandler(),
            DotReferenceHelp());
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Tagged, TaggedDotReferenceHandler(),
            TaggedDotReferenceHelp());
    }

    public override IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "proto":
                return new NumberVariable(Prototype.Id);

            case "quantity":
                return new NumberVariable(Quantity);

            case "holder":
                IHoldable holdable = GetItemType<IHoldable>();
                return holdable?.HeldBy?.Actor;

            case "wearer":
                IWearable wearable = GetItemType<IWearable>();
                return wearable?.WornBy?.Actor;

            case "owner":
                return Owner switch
                {
                    IProgVariable progVariable => progVariable,
                    _ => new NullVariable(OwnershipFunctionHelpers.OwnerVariableType)
                };

            case "hasowner":
                return new BooleanVariable(HasOwner);

            case "ownerid":
                return new NumberVariable(OwnershipReference?.Id ?? 0L);

            case "ownertype":
                return new TextVariable(OwnershipReference?.FrameworkItemType ?? string.Empty);

            case "weight":
                return new NumberVariable(Weight);

            case "contents":
                return new CollectionVariable(
                    IsItemType<IContainer>() ? GetItemType<IContainer>().Contents.ToList() : new List<IGameItem>(),
                    ProgVariableTypes.Item
                );

            case "container":
                return ContainedIn;

            case "iscontainer":
                return new BooleanVariable(IsItemType<IContainer>());

            case "iscorpse":
                return new BooleanVariable(IsItemType<ICorpse>());

            case "isopenable":
                return new BooleanVariable(IsItemType<IOpenable>());

            case "iscurrency":
                return new BooleanVariable(IsItemType<ICurrencyPile>());

            case "islock":
                return new BooleanVariable(IsItemType<ILock>());

            case "islockable":
                return new BooleanVariable(IsItemType<ILockable>());

            case "iskey":
                return new BooleanVariable(IsItemType<IKey>());

            case "istable":
                return new BooleanVariable(IsItemType<ITable>());

            case "ischair":
                return new BooleanVariable(IsItemType<IChair>());

            case "isdoor":
                return new BooleanVariable(IsItemType<IDoor>());

            case "isbelt":
                return new BooleanVariable(IsItemType<IBelt>());

            case "isbeltable":
                return new BooleanVariable(IsItemType<IBeltable>());

            case "iswieldable":
                return new BooleanVariable(IsItemType<IWieldable>());

            case "iswearable":
                return new BooleanVariable(IsItemType<IWearable>());

            case "isholdable":
                return new BooleanVariable(IsItemType<IHoldable>());

            case "issheath":
                return new BooleanVariable(IsItemType<ISheath>());

            case "islightable":
                return new BooleanVariable(IsItemType<ILightable>());

            case "ispowered":
                return new BooleanVariable(GetItemType<IProducePower>()?.ProducingPower ?? false);

            case "ison":
                return new BooleanVariable(GetItemType<IOnOff>()?.SwitchedOn ?? false);

            case "iscover":
                return new BooleanVariable(IsItemType<IProvideCover>());

            case "isweapon":
                return new BooleanVariable(IsItemType<IRangedWeapon>() || IsItemType<IMeleeWeapon>());
            case "ismeleeweapon":
                return new BooleanVariable(IsItemType<IMeleeWeapon>());
            case "israngedweapon":
                return new BooleanVariable(IsItemType<IRangedWeapon>());

            case "providingcover":
                return new BooleanVariable(GetItemType<IProvideCover>()?.Cover != null);

            case "lit":
                ILightable lightable = GetItemType<ILightable>();
                return new BooleanVariable(lightable?.Lit == true);

            case "open":
                IOpenable openable = GetItemType<IOpenable>();
                return new BooleanVariable(openable?.IsOpen == true);

            case "locked":
                ILockable lockable = GetItemType<ILockable>();
                ILock theLock = GetItemType<ILock>();
                return new BooleanVariable(lockable?.Locks.Any(x => x.IsLocked) == true ||
                                           theLock?.IsLocked == true);

            case "locks":
                lockable = GetItemType<ILockable>();
                return new CollectionVariable(lockable?.Locks.ToList() ?? new List<ILock>(),
                    ProgVariableTypes.Item);
            case "corpsecharacter":
                return GetItemType<ICorpse>()?.OriginalCharacter;
            case "corpsebodyid":
                return new NumberVariable(GetItemType<ICorpse>()?.OriginalBodyId ?? 0);
            case "corpseremainscontext":
                return new TextVariable(GetItemType<ICorpse>()?.RemainsContext.DescribeEnum() ?? "");
            case "corpsefinaldeath":
                return new BooleanVariable(GetItemType<ICorpse>()?.RepresentsFinalCharacterDeath == true);
            case "location":
                return Location;
            case "tags":
                return new CollectionVariable(Tags.Select(x => x.Name).ToList(), ProgVariableTypes.Text);
            case "iscommodity":
                return new BooleanVariable(IsItemType<ICommodity>());
            case "material":
                return Material;
            case "isgridconnectable":
                return new BooleanVariable(IsItemType<ICanConnectToGrid>());
            case "iselectricgridconnectable":
                return new BooleanVariable(IsItemType<ICanConnectToElectricalGrid>());
            case "istelecommunicationsgridconnectable":
                return new BooleanVariable(IsItemType<ICanConnectToTelecommunicationsGrid>());
            case "istelephone":
                return new BooleanVariable(IsItemType<ITelephone>());
            case "iscellularphone":
                return new BooleanVariable(GetItemType<ITelephone>() is CellularPhoneGameItemComponent);
            case "iscellphonetower":
                return new BooleanVariable(IsItemType<ICellPhoneTower>());
            case "phonenumber":
                ITelephone phone = GetItemType<ITelephone>();
                if (phone != null)
                {
                    return phone.PhoneNumber != null
                        ? new TextVariable(phone.PhoneNumber)
                        : new NullVariable(ProgVariableTypes.Text);
                }

                ITelephoneNumberOwner owner = GetItemType<ITelephoneNumberOwner>();
                return owner?.PhoneNumber != null
                    ? new TextVariable(owner.PhoneNumber)
                    : new NullVariable(ProgVariableTypes.Text);
            case "grid":
                return new NumberVariable(GetItemType<ICanConnectToGrid>()?.Grid?.Id ?? 0);
            case "layer":
                return new TextVariable(RoomLayer.DescribeEnum());
            case "isfood":
                return new BooleanVariable(IsItemType<IEdible>());
            case "isliquidcontainer":
                return new BooleanVariable(IsItemType<ILiquidContainer>());
            case "issealstamp":
                return new BooleanVariable(IsItemType<ISealStamp>());
            case "issealable":
                return new BooleanVariable(IsItemType<ISealable>());
            case "issealed":
                return new BooleanVariable(GetItemType<ISealable>()?.IsSealed == true);
            case "sealbroken":
                return new BooleanVariable(GetItemType<ISealable>()?.SealBroken == true);
            case "sealresidue":
                return new BooleanVariable(GetItemType<ISealable>()?.HasSealResidue == true);
            case "sealdesign":
                return GetItemType<ISealable>()?.CurrentSeal is { } sealDesign
                    ? new TextVariable(sealDesign.SealDesign)
                    : new NullVariable(ProgVariableTypes.Text);
            case "sealissuer":
                return GetItemType<ISealable>()?.CurrentSeal is { } sealIssuer
                    ? new TextVariable(sealIssuer.IssuerText)
                    : new NullVariable(ProgVariableTypes.Text);
            case "sealowner":
                return GetItemType<ISealable>()?.CurrentSeal is { } sealOwner
                    ? new TextVariable(sealOwner.OwnerText)
                    : new NullVariable(ProgVariableTypes.Text);
            case "sealclan":
                return GetItemType<ISealable>()?.CurrentSeal is { } sealClan
                    ? new TextVariable(sealClan.ClanText)
                    : new NullVariable(ProgVariableTypes.Text);
            case "sealoffice":
                return GetItemType<ISealable>()?.CurrentSeal is { } sealOffice
                    ? new TextVariable(sealOffice.OfficeText)
                    : new NullVariable(ProgVariableTypes.Text);
            case "sealmaterial":
                return GetItemType<ISealable>()?.CurrentSeal is { } sealMaterial
                    ? new TextVariable(sealMaterial.StampMaterial)
                    : new NullVariable(ProgVariableTypes.Text);
            case "sealmedium":
                return GetItemType<ISealable>()?.CurrentSeal is { } sealMedium
                    ? new TextVariable(sealMedium.SealMedium)
                    : new NullVariable(ProgVariableTypes.Text);
            case "sealingcharacterid":
                return new NumberVariable(GetItemType<ISealable>()?.CurrentSeal?.SealingCharacterId ?? 0);
            case "ismeasuringinstrument":
                return new BooleanVariable(IsItemType<IMeasuringInstrument>());
            case "measuringmode":
                return GetItemType<IMeasuringInstrument>() is { } instrumentMode
                    ? new TextVariable(instrumentMode.Mode.DescribeEnum())
                    : new NullVariable(ProgVariableTypes.Text);
            case "calibrationbias":
                return new NumberVariable(GetItemType<IMeasuringInstrument>()?.CalibrationBias ?? 0.0);
            case "calibrationbiasispercent":
                return new BooleanVariable(GetItemType<IMeasuringInstrument>()?.CalibrationBiasIsPercentage == true);
            case "calibrationdeliberate":
                return new BooleanVariable(GetItemType<IMeasuringInstrument>()?.HasDeliberateBias == true);
            case "usessincecalibration":
                return new NumberVariable(GetItemType<IMeasuringInstrument>()?.UsesSinceCalibration ?? 0);
            case "variables":
                Dictionary<string, IProgVariable> dict = new(StringComparer.InvariantCultureIgnoreCase);
                foreach ((ICharacteristicDefinition definition, ICharacteristicValue value) in RawCharacteristics)
                {
                    if (value is null)
                    {
                        continue;
                    }

                    dict[definition.Name.ToLowerInvariant()] = new TextVariable(value.GetValue.ToLowerInvariant());
                }
                return new DictionaryVariable(dict, ProgVariableTypes.Text);
            case "condition":
                return new NumberVariable(Condition);
            case "quality":
                return new NumberVariable((int)Quality);
            case "qualityname":
                return new TextVariable(Quality.DescribeEnum());
            case "rawquality":
                return new NumberVariable((int)RawQuality);
            case "rawqualityname":
                return new TextVariable(RawQuality.DescribeEnum());
            default:
                return base.GetProperty(property);
        }
    }

    public override ProgVariableTypes Type => ProgVariableTypes.Item;

    #endregion
}
