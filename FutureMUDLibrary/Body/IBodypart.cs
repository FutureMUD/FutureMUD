using System.Collections.Generic;
using JetBrains.Annotations;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.Body {
    public enum BodypartTypeEnum
    {
        Wear = 0,
        GrabbingWielding = 1,
        Grabbing = 2,
        Wielding = 3,
        Brain = 4,
        Liver = 5,
        Heart = 6,
        Standing = 7,
        Eye = 8,
        Ear = 9,
        Spleen = 10,
        Intestines = 11,
        Spine = 12,
        Stomach = 13,
        Lung = 14,
        Trachea = 15,
        Kidney = 16,
        Esophagus = 17,
        Tongue = 18,
        Mouth = 19,
        Bone = 20,
        NonImmobilisingBone = 21,
        MinorBone = 22,
        MinorNonImobilisingBone = 23,
        Wing = 24,
        PositronicBrain = 25,
        PowerCore = 26,
        SpeechSynthesizer = 27,
        Joint = 28,
        Fin = 29,
        Gill = 30,
        Blowhole = 31,
        BonyDrapeable = 32,
        BonyGrabbingWielding = 33,
        NonImmobilisingBonyDrapeable = 34,
    }
    
    public interface IBodypart : IKeywordedItem, IDescribable, IHaveFuturemud, ISaveable, ILateInitialisingItem {

        [CanBeNull]
        IBodypart UpstreamConnection { get; }

        IEnumerable<IOrganProto> Organs { get; }
        Dictionary<IOrganProto,BodypartInternalInfo> OrganInfo { get; }

        IEnumerable<IBone> Bones { get; }
        Dictionary<IBone, BodypartInternalInfo> BoneInfo { get; }
        Alignment Alignment { get; }

        Orientation Orientation { get; }

        BodypartTypeEnum BodypartType { get; }

        void SetBodyProto(IBodyPrototype proto);

        double PainModifier { get; }

        double BleedModifier { get; }

        double DamageModifier { get; }

        double StunModifier { get; }

        int Weight { get; }

        double RelativeHitChance { get; }

        bool CanSever { get; }

        uint MaxLife { get; }

        bool IsVital { get; }

        bool IsCore { get; }

        int SeveredThreshold { get; }

        bool Significant { get; }

        [NotNull]
        IBodypartShape Shape { get; }

        [NotNull]
        IBodyPrototype Body { get; }

        ISolid DefaultMaterial { get; }

        double ImplantSpace { get; }

        SizeCategory Size { get; }

        void LinkUpstream(IBodypart part);
        void LinkOrgan(IOrganProto part, BodypartInternalInfo info);
        void LinkBone(IBone bone, BodypartInternalInfo info);
        void PostLoadProcessing(IBodyPrototype body, MudSharp.Models.BodypartProto proto);

        bool DownstreamOfPart(IBodypart part);

        int HitChances();
        int HitChances(Alignment alignment, Orientation orientation);

        bool PartDamageEffects(IBody owner, CanUseBodypartResult why);

        string ShortDescription(bool proper = false, bool colour = true,
            PermissionLevel informationLevel = PermissionLevel.Any);

        string FullDescription(bool proper = false,
            PermissionLevel informationLevel = PermissionLevel.Any);

        IArmourType NaturalArmourType { get; }

        string ShowToBuilder(ICharacter builder);
        bool BuildingCommand(ICharacter builder, StringStack command);
        bool CountsAs(IBodypart otherBodypart);
        IBodypart Clone(string newName);
    }
}