using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.Disfigurements
{
    public interface IScarTemplate : IDisfigurementTemplate
    {
        /// <summary>
        /// Returns a special overriding description, e.g. "facially-disfigured", "with an eye scar" etc
        /// </summary>
        /// <param name="withForm">If true, it's in the form "with ...", otherwise it's a participle e.g. "scarred"</param>
        /// <returns>The description</returns>
        string SpecialScarCharacteristicOverride(bool withForm);
        bool HasSpecialScarCharacteristicOverride { get; }
        bool CanBeAppliedFromDamage(DamageType damagetype, WoundSeverity severity);
        bool CanBeAppliedFromSurgery(SurgicalProcedureType type);
        IScar ProduceScar(ICharacter target, IBodypart bodypart);
        int SizeSteps { get; }
        int Distinctiveness { get; }
    }
}
