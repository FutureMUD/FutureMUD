using System.Collections.Generic;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.Character.Heritage {
    public interface IEthnicity : IEditableItem, IProgVariable {
        string ChargenBlurb { get; }
        IDictionary<ICharacteristicDefinition, ICharacteristicProfile> CharacteristicChoices { get; }
        IRace ParentRace { get; }
        IFutureProg AvailabilityProg { get; }
        string EthnicGroup { get; }
        string EthnicSubgroup { get; }
        IPopulationBloodModel PopulationBloodModel { get; }
        bool ChargenAvailable(ICharacterTemplate template);
        int ResourceCost(IChargenResource resource);
        int ResourceRequirement(IChargenResource resource);
        IEnumerable<ChargenResourceCost> Costs { get; }
        IEnumerable<IChargenAdvice> ChargenAdvices { get; }
        bool ToggleAdvice(IChargenAdvice advice);
        double TolerableTemperatureFloorEffect { get; }
        double TolerableTemperatureCeilingEffect { get; }
    }
}