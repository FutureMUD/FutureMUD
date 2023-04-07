using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Form.Characteristics
{
    public enum PluralisationType {
        UsePluralisationService,
        Plural,
        Singular
    }
    
    public interface ICharacteristicValue : IFrameworkItem
    {
        IFutureProg ChargenApplicabilityProg {
            get;
        }

        IFutureProg OngoingValidityProg
		{
            get;
		}

        ICharacteristicDefinition Definition { get; }

        string GetValue { get; }
        string GetBasicValue { get; }
        string GetFancyValue { get; }

        PluralisationType Pluralisation { get; }

        void BuildingCommand(ICharacter actor, StringStack command);
        string Show(ICharacter actor);

        ICharacteristicValue Clone(string newName);
    }
}
