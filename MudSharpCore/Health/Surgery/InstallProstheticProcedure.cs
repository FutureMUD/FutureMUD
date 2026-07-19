using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class InstallProstheticProcedure : BodypartSpecificSurgicalProcedure
{
    public InstallProstheticProcedure(MudSharp.Models.SurgicalProcedure procedure, IFuturemud gameworld) : base(
        procedure, gameworld)
    {
    }

    public InstallProstheticProcedure(IFuturemud gameworld, string name, string gerund, IBodyPrototype body,
        string school, IKnowledge knowledge) : base(gameworld, name, gerund, body, school, knowledge)
    {
    }

    public override SurgicalProcedureType Procedure => SurgicalProcedureType.InstallProsthetic;
    public override CheckType Check => CheckType.InstallProstheticSurgery;
    public override bool RequiresUnconsciousPatient => false;
    public override bool RequiresInvasiveProcedureFinalisation => false;
    public override bool RequiresLivingPatient => true;

    protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        if (additionalArguments.Length != 1)
        {
            return new object[] { default(IGameItem), default(IProsthetic), default(IBodypart) };
        }

        IGameItem prostheticItem = additionalArguments[0] as IGameItem ??
                                   surgeon.TargetHeldItem(additionalArguments[0]?.ToString());
        IProsthetic prosthetic = prostheticItem?.GetItemType<IProsthetic>();
        return new object[] { prostheticItem, prosthetic, prosthetic?.TargetBodypart };
    }

    public override string DescribeProcedureGerund(ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        IBodypart bodypart = additionalArguments.ElementAtOrDefault(2) as IBodypart;
        return bodypart is null
            ? $"{ProcedureGerund} a prosthetic to $1"
            : $"{ProcedureGerund} a prosthetic to $1's {bodypart.FullDescription()}";
    }

    protected override List<(IGameItem Item, DesiredItemState State)> GetAdditionalInventory(ICharacter surgeon,
        ICharacter patient, object[] additionalArguments)
    {
        return new List<(IGameItem Item, DesiredItemState State)>
        {
            ((IGameItem)additionalArguments[0], DesiredItemState.Held)
        };
    }

    public override string DressPhaseEmote(string emote, ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        return string.Format(
            emote,
            ((IGameItem)additionalArguments[0]).HowSeen(surgeon),
            ((IBodypart)additionalArguments[2]).FullDescription().ToLowerInvariant());
    }

    public override int DressPhaseEmoteExtraArgumentCount => 2;

    public override string DressPhaseEmoteHelpAddendum => @"	#3{0}#0 - the description of the prosthetic item being fitted
	#3{1}#0 - the description of the bodypart for the prosthetic
".SubstituteANSIColour();

	public override bool CanPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 1)
		{
			return false;
		}

		object[] args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IGameItem prostheticItem)
		{
			return false;
		}

		if (args[1] is not IProsthetic prosthetic || args[2] is not IBodypart bodypart)
		{
			return false;
		}

		if (prosthetic.InstalledBody != null)
		{
			return false;
		}

		if (!patient.Body.Prototype.CountsAs(prosthetic.TargetBody))
		{
			return false;
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return false;
		}

		if (!CanInstallProstheticOnBody(patient.Body, prosthetic, bodypart))
		{
			return false;
		}

		if (!surgeon.IsAdministrator() && HasObstructingClothing(patient, bodypart))
		{
			return false;
		}

		return base.CanPerformProcedure(surgeon, patient, args);
	}

	public override string WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient,
		params object[] additionalArguments)
	{
		if (additionalArguments.Length != 1)
		{
			return "You must specify the prosthetic object you wish to fit.";
		}

		object[] args = GetProcessedAdditionalArguments(surgeon, patient, additionalArguments);
		if (args[0] is not IGameItem prostheticItem)
		{
			return "You are not holding any such prosthetic to fit.";
		}

		if (args[1] is not IProsthetic prosthetic || args[2] is not IBodypart bodypart)
		{
			return $"{prostheticItem.HowSeen(surgeon, true)} is not a prosthetic.";
		}

		if (prosthetic.InstalledBody != null)
		{
			return $"{prostheticItem.HowSeen(surgeon, true)} is already installed on someone.";
		}

		if (!patient.Body.Prototype.CountsAs(prosthetic.TargetBody))
		{
			return
				$"{prostheticItem.HowSeen(surgeon, true)} is not designed for the same biology as {patient.HowSeen(surgeon)} possesses.";
		}

		if (!IsPermissableBodypart(bodypart))
		{
			return $"This procedure is not designed to work with {bodypart.FullDescription().Pluralise()}.";
		}

		if (!HasMatchingSever(patient.Body, bodypart))
		{
			return
				$"{patient.HowSeen(surgeon, true)} {(patient == surgeon ? "do" : "does")} not have the appropriate disability to have {prostheticItem.HowSeen(surgeon)} fitted. It is designed for individuals with a sever at the {bodypart.FullDescription()}.";
		}

		if (HasExistingProsthetic(patient.Body, bodypart))
		{
			return
				$"{patient.HowSeen(surgeon, true)} already {(patient == surgeon ? "have" : "has")} a prosthetic for that location. The existing one must first be removed.";
		}

		if (!surgeon.IsAdministrator() && HasObstructingClothing(patient, bodypart))
		{
			return
				$"{patient.HowSeen(surgeon, true)} {(patient == surgeon ? "have" : "has")} clothing obstructing the fitting of {prostheticItem.HowSeen(surgeon)}. You must first remove it.";
		}

		return base.WhyCannotPerformProcedure(surgeon, patient, args);
	}

	private string WhyCannotCompleteProcedure(ICharacter surgeon, ICharacter patient, IGameItem prostheticItem,
		IProsthetic prosthetic, IBodypart bodypart)
	{
		if (prosthetic.InstalledBody != null)
		{
			return "the prosthetic is already installed on someone";
		}

		if (!patient.Body.Prototype.CountsAs(prosthetic.TargetBody))
		{
			return "the patient is no longer the right body type for that prosthetic";
		}

		if (!HasMatchingSever(patient.Body, bodypart))
		{
			return "the patient no longer has the appropriate disability";
		}

		if (HasExistingProsthetic(patient.Body, bodypart))
		{
			return "the patient already has a prosthetic fitted at that location";
		}

		if (!surgeon.IsAdministrator() && HasObstructingClothing(patient, bodypart))
		{
			return $"clothing is now obstructing the fitting of {prostheticItem.HowSeen(surgeon)}";
		}

		return "it is no longer a valid prosthetic fitting";
	}

	public override IBodypart GetTargetBodypart(object[] parameters)
	{
		return (IBodypart)parameters[2];
	}

	public override void CompleteProcedure(ICharacter surgeon, ICharacter patient, CheckOutcome result,
		params object[] additionalArguments)
	{
		IGameItem prostheticItem = (IGameItem)additionalArguments[0];
		IProsthetic prosthetic = (IProsthetic)additionalArguments[1];
		IBodypart bodypart = (IBodypart)additionalArguments[2];

		if (!CanInstallProstheticOnBody(patient.Body, prosthetic, bodypart) ||
		    (!surgeon.IsAdministrator() && HasObstructingClothing(patient, bodypart)))
		{
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)} because {WhyCannotCompleteProcedure(surgeon, patient, prostheticItem, prosthetic, bodypart)}.",
						surgeon, surgeon, patient, prostheticItem)));
			return;
		}

		if (result.IsFail())
		{
			CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, prostheticItem);
			surgeon.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"@ finish|finishes trying to fit $2 to $1's {bodypart.FullDescription()}, but the fit is not secure enough to attach.",
						surgeon, surgeon, patient, prostheticItem)));
			return;
		}

		CompletionProg?.Execute(surgeon, patient, result.Outcome, bodypart.Name, prostheticItem);
		surgeon.Body.Take(prostheticItem);
		patient.Body.InstallProsthetic(prosthetic);
		surgeon.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"@ finish|finishes fitting $2 to $1's {bodypart.FullDescription()}.",
					surgeon, surgeon, patient, prostheticItem)));
	}

    public override void AbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
        params object[] additionalArguments)
    {
        IGameItem prostheticItem = (IGameItem)additionalArguments[0];
        IBodypart bodypart = (IBodypart)additionalArguments[2];
        surgeon.OutputHandler.Handle(
            new EmoteOutput(
                new Emote($"@ stop|stops {DescribeProcedureGerund(surgeon, patient, additionalArguments)}.",
                    surgeon, surgeon, patient, prostheticItem)));
        AbortProg?.Execute(surgeon, patient, result, bodypart.Name, prostheticItem);
    }

    protected override void SilentAbortProcedure(ICharacter surgeon, ICharacter patient, Outcome result,
        params object[] additionalArguments)
    {
        IGameItem prostheticItem = (IGameItem)additionalArguments[0];
        IBodypart bodypart = (IBodypart)additionalArguments[2];
        AbortProg?.Execute(surgeon, patient, result, bodypart.Name, prostheticItem);
    }

    public override Difficulty GetProcedureDifficulty(ICharacter surgeon, ICharacter patient,
        params object[] additionalArguments)
    {
        IProsthetic prosthetic = (IProsthetic)additionalArguments[1];
        return prosthetic.Functional ? Difficulty.Normal : Difficulty.Easy;
    }

    protected override IEnumerable<IEnumerable<ProgVariableTypes>> ParametersForCancelProg => new[]
    {
        new[]
        {
            ProgVariableTypes.Character,
            ProgVariableTypes.Character,
            ProgVariableTypes.Number,
            ProgVariableTypes.Text,
            ProgVariableTypes.Item
        },
        new[]
        {
            ProgVariableTypes.Character,
            ProgVariableTypes.Character,
            ProgVariableTypes.Text,
            ProgVariableTypes.Text,
            ProgVariableTypes.Item
		},
	};

	internal static bool CanInstallProstheticOnBody(IBody body, IProsthetic prosthetic, IBodypart bodypart)
	{
		return prosthetic.InstalledBody == null &&
		       body.Prototype.CountsAs(prosthetic.TargetBody) &&
		       HasMatchingSever(body, bodypart) &&
		       !HasExistingProsthetic(body, bodypart);
	}

	private static bool HasMatchingSever(IBody body, IBodypart bodypart)
	{
		return body.SeveredRoots.Any(x => x == bodypart || x.CountsAs(bodypart) || bodypart.CountsAs(x));
	}

	private static bool HasExistingProsthetic(IBody body, IBodypart bodypart)
	{
		return body.Prosthetics.Any(x =>
			x.TargetBodypart == bodypart ||
			x.TargetBodypart.CountsAs(bodypart) ||
			bodypart.CountsAs(x.TargetBodypart));
	}

    private static bool HasObstructingClothing(ICharacter patient, IBodypart bodypart)
    {
        List<IBodypart> affectedParts =
            patient.Body.Prototype.BodypartsFor(patient.Race, patient.Gender.Enum)
                   .Where(x => x == bodypart || x.DownstreamOfPart(bodypart))
                   .ToList();

        return patient.Body.WornItems
                      .Select(x => x.GetItemType<IWearable>())
                      .Any(x =>
                          x != null &&
                          x.CurrentProfile.AllProfiles.Any(y =>
                              affectedParts.Contains(y.Key) && y.Value.PreventsRemoval));
    }
}
