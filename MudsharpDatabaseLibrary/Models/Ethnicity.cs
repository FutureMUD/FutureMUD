using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Ethnicity
    {
        public Ethnicity()
        {
            Bodies = new HashSet<Body>();
            ChargenAdvicesEthnicities = new HashSet<ChargenAdvicesEthnicities>();
            EthnicitiesCharacteristics = new HashSet<EthnicitiesCharacteristics>();
            EthnicitiesChargenResources = new HashSet<EthnicitiesChargenResources>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string ChargenBlurb { get; set; }
        public long? AvailabilityProgId { get; set; }
        public long? ParentRaceId { get; set; }
        public string EthnicGroup { get; set; }
        public string EthnicSubgroup { get; set; }
        public long? PopulationBloodModelId { get; set; }
        public double TolerableTemperatureFloorEffect { get; set; }
        public double TolerableTemperatureCeilingEffect { get; set; }

        public virtual FutureProg AvailabilityProg { get; set; }
        public virtual Race ParentRace { get; set; }
        public virtual PopulationBloodModel PopulationBloodModel { get; set; }
        public virtual ICollection<Body> Bodies { get; set; }
        public virtual ICollection<ChargenAdvicesEthnicities> ChargenAdvicesEthnicities { get; set; }
        public virtual ICollection<EthnicitiesCharacteristics> EthnicitiesCharacteristics { get; set; }
        public virtual ICollection<EthnicitiesChargenResources> EthnicitiesChargenResources { get; set; }
    }
}
