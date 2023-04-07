using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Body
    {
        public Body()
        {
            BodiesDrugDoses = new HashSet<BodyDrugDose>();
            BodiesGameItems = new HashSet<BodiesGameItems>();
            BodiesImplants = new HashSet<BodiesImplants>();
            BodiesProsthetics = new HashSet<BodiesProsthetics>();
            BodiesSeveredParts = new HashSet<BodiesSeveredParts>();
            Characteristics = new HashSet<Characteristic>();
            Characters = new HashSet<Character>();
            HooksPerceivables = new HashSet<HooksPerceivable>();
            Infections = new HashSet<Infection>();
            PerceiverMerits = new HashSet<PerceiverMerit>();
            Traits = new HashSet<Trait>();
            Wounds = new HashSet<Wound>();
        }

        public long Id { get; set; }
        public long BodyPrototypeId { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public long Position { get; set; }
        public long? CurrentSpeed { get; set; }
        public long RaceId { get; set; }
        public double CurrentStamina { get; set; }
        public double CurrentBloodVolume { get; set; }
        public long EthnicityId { get; set; }
        public long? BloodtypeId { get; set; }
        public short Gender { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public long? ShortDescriptionPatternId { get; set; }
        public long? FullDescriptionPatternId { get; set; }
        public string Tattoos { get; set; }
        public int HeldBreathLength { get; set; }
        public string EffectData { get; set; }
        public string Scars { get; set; }

        public virtual Bloodtype Bloodtype { get; set; }
        public virtual Ethnicity Ethnicity { get; set; }
        public virtual EntityDescriptionPattern FullDescriptionPattern { get; set; }
        public virtual Race Race { get; set; }
        public virtual EntityDescriptionPattern ShortDescriptionPattern { get; set; }
        public virtual ICollection<BodyDrugDose> BodiesDrugDoses { get; set; }
        public virtual ICollection<BodiesGameItems> BodiesGameItems { get; set; }
        public virtual ICollection<BodiesImplants> BodiesImplants { get; set; }
        public virtual ICollection<BodiesProsthetics> BodiesProsthetics { get; set; }
        public virtual ICollection<BodiesSeveredParts> BodiesSeveredParts { get; set; }
        public virtual ICollection<Characteristic> Characteristics { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
        public virtual ICollection<Infection> Infections { get; set; }
        public virtual ICollection<PerceiverMerit> PerceiverMerits { get; set; }
        public virtual ICollection<Trait> Traits { get; set; }
        public virtual ICollection<Wound> Wounds { get; set; }
    }
}
