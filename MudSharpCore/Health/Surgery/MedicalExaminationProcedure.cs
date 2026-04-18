using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Health.Surgery;

public class MedicalExaminationProcedure : SurgicalProcedure
{
    public MedicalExaminationProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld)
        : base(procedure, gameworld)
    {
    }

    public MedicalExaminationProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body, string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
    {
    }

    public override CheckType Check => CheckType.MedicalExaminationCheck;

    public override SurgicalProcedureType Procedure => SurgicalProcedureType.DetailedExamination;

    public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        return Difficulty.ExtremelyEasy;
    }

    protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCancelProg => new[]
    {
        new[]
        {
            ProgVariableTypes.Character,
            ProgVariableTypes.Character,
            ProgVariableTypes.Number
        },
        new[]
        {
            ProgVariableTypes.Character,
            ProgVariableTypes.Character,
            ProgVariableTypes.Text
        },
    };

    public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
        params object[] additionalArguments)
    {
        // No negative consequences to being interrupted on an examination
        surgeon.OutputHandler.Handle(
            new EmoteOutput(new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
                surgeon, surgeon, patient)));
        AbortProg?.Execute(surgeon, patient, result);
    }

    protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
        params object[] additionalArguments)
    {
        AbortProg?.Execute(surgeon, patient, result);
    }

    protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCompletionProg => new[] {
                new[]
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Number
                },
                new[]
                {
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Text
                },
            };

    public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
        params object[] additionalArguments)
    {
        CompletionProg?.Execute(surgeon, patient, result.Outcome);
        StringBuilder sb = new();
        int age = patient.Location.Date(patient.Birthday.Calendar).YearsDifference(patient.Birthday);
        double ageFuzziness;
        switch (result.Outcome)
        {
            case Outcome.MajorFail:
                ageFuzziness = 0.2;
                break;
            case Outcome.Fail:
                ageFuzziness = 0.1;
                break;
            case Outcome.MinorFail:
                ageFuzziness = 0.07;
                break;
            case Outcome.MinorPass:
                ageFuzziness = 0.045;
                break;
            case Outcome.Pass:
                ageFuzziness = 0.035;
                break;
            case Outcome.MajorPass:
                ageFuzziness = 0.025;
                break;
            default:
                ageFuzziness = 0.5;
                break;
        }

        double randomShift = RandomUtilities.DoubleRandom(-1 * ageFuzziness * age, ageFuzziness * age);
        double apparentAge = age + randomShift;
        int minApparentAge = (int)(apparentAge * (1.0 - ageFuzziness));
        int maxApparentAge = (int)(apparentAge * (1.0 + ageFuzziness));
        string ageDescriptor = minApparentAge == maxApparentAge
            ? $"approximately {apparentAge:N0}"
            : $"between {minApparentAge:N0} and {maxApparentAge:N0}";

        Gendering apparentGender = patient.ApparentGender(surgeon);
        sb.AppendLine(
            $"Your patient is a {patient.Race.Name.ToLowerInvariant()} {patient.ApparentGender(surgeon).GenderClass()} of {ageDescriptor} years of age.");
        sb.AppendLine(
            $"{apparentGender.Subjective(true)} has {patient.Race.HealthTraits.Select(x => $"{x.Decorator.Decorate(patient.GetTrait(x)).Colour(Telnet.Green)} {x.Name}").ListToString().Fullstop()}");
        List<IWound> wounds = patient.VisibleWounds(surgeon, WoundExaminationType.Examination).ToList();
        if (wounds.Any())
        {
            sb.AppendLine($"{apparentGender.Subjective(true)} has the following injuries:");
            foreach (IWound wound in wounds)
            {
                sb.AppendLine($"\t{wound.Describe(WoundExaminationType.Examination, result).Fullstop()}");
            }
        }
        else
        {
            sb.AppendLine($"{apparentGender.Subjective(true)} does not appear to have any external injuries.");
        }


        // TODO - more information in medical examination
        surgeon.OutputHandler.Handle(
            new EmoteOutput(new Emote(
                $"@ finish|finishes {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.", surgeon,
                surgeon, patient)));
        surgeon.Send(sb.ToString());
    }
}