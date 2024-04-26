using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law
{
    public class PunishmentResult
    {
        public decimal Fine { get; init; } = 0.0M;
        public TimeAndDate.MudTimeSpan CustodialSentence {  get; init; } = TimeAndDate.MudTimeSpan.Zero;
        public TimeAndDate.MudTimeSpan GoodBehaviourBondLength { get; init;} = TimeAndDate.MudTimeSpan.Zero;
        public bool Execution { get; set; } = false;

        public static PunishmentResult operator +(PunishmentResult r1, PunishmentResult r2)
        {
            return new PunishmentResult
            {
                Fine = r1.Fine + r2.Fine,
                CustodialSentence = r1.CustodialSentence + r2.CustodialSentence,
                GoodBehaviourBondLength = r1.GoodBehaviourBondLength + r2.GoodBehaviourBondLength
            };
        }

        public string Describe(IPerceiver voyeur, ILegalAuthority authority)
        {
            var strings = new List<string>();
            if (CustodialSentence > TimeAndDate.MudTimeSpan.Zero)
            {
                strings.Add($"{CustodialSentence.Describe(voyeur)} in prison".ColourValue());
            }
            if (Fine > 0.0M)
            {
                strings.Add($"a {authority.Currency.Describe(Fine, Economy.Currency.CurrencyDescriptionPatternType.Short)} fine".ColourValue());
            }
            if (GoodBehaviourBondLength > TimeAndDate.MudTimeSpan.Zero)
            {
                strings.Add($"a {GoodBehaviourBondLength.Describe(voyeur)} good behaviour bond".ColourValue());
            }
            return strings.ListToString();
        }
    }
}
