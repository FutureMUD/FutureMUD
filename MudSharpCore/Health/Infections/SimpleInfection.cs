using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Linq;

namespace MudSharp.Health.Infections;

public class SimpleInfection : Infection
{
    private InfectionNausea _nauseaEffect;

    public SimpleInfection(Difficulty virulenceDifficulty, double intensity, IBody owner, IWound wound,
        IBodypart bodypart, double virulence) : base(virulenceDifficulty, intensity, owner, wound, bodypart, virulence)
    {
        InfectionStage = DetermineStage(Intensity);
        HandleInfectionSideEffects();
    }

    public SimpleInfection(MudSharp.Models.Infection infection, IBody body, IWound wound, IBodypart bodypart)
        : base(infection, body, wound, bodypart)
    {
        InfectionStage = DetermineStage(Intensity);
        HandleInfectionSideEffects();
    }

    public override InfectionType InfectionType => InfectionType.Simple;
    public override InfectionStage InfectionStage { get; set; }

    private static Expression _painExpression;

    private static Expression PainExpression
    {
        get
        {
            if (_painExpression == null)
            {
                _painExpression =
                    new Expression(Futuremud.Games.First().GetStaticConfiguration("SimpleInfectionPainFormula"));
            }

            return _painExpression;
        }
    }

    public override double Pain
    {
        get
        {
            PainExpression.Parameters["intensity"] = Intensity;
            return Convert.ToDouble(PainExpression.Evaluate());
        }
    }

    // Get the tag to stick on the end of wound listings in views like 'look', 'triage', and 'examination'.
    public override string WoundTag(WoundExaminationType examType, Outcome outcome)
    {
        string woundTag = "";
        switch (examType)
        {
            case WoundExaminationType.Glance:
                break; //Too busy to spot infections
            case WoundExaminationType.Look:
                if (Intensity > Gameworld.GetStaticDouble("SimpleInfectionVisibleThresholdLook") /
                    outcome.SuccessDegrees())
                {
                    woundTag = "(infected)".Colour(Telnet.Yellow);
                }

                break;
            case WoundExaminationType.Self:
                if (Intensity > Gameworld.GetStaticDouble("SimpleInfectionVisibleThresholdSelf") /
                    outcome.SuccessDegrees())
                {
                    woundTag = "(infected)".Colour(Telnet.Yellow);
                }

                break;
            case WoundExaminationType.Examination:
            case WoundExaminationType.Triage:
            case WoundExaminationType.SurgicalExamination:
            case WoundExaminationType.Omniscient:
                switch (InfectionStage)
                {
                    case InfectionStage.StageZero:
                        break;
                    case InfectionStage.StageOne:
                        woundTag = "(Barely Infected)".Colour(Telnet.Yellow);
                        break;
                    case InfectionStage.StageTwo:
                        woundTag = "(Minor Infection)".Colour(Telnet.BoldYellow);
                        break;
                    case InfectionStage.StageThree:
                        woundTag = "(Infected)".Colour(Telnet.BoldYellow);
                        break;
                    case InfectionStage.StageFour:
                        woundTag = "(Spreading Infection)".Colour(Telnet.Red);
                        break;
                    case InfectionStage.StageFive:
                        woundTag = "(Ghastly Infection)".Colour(Telnet.Red);
                        break;
                    case InfectionStage.StageSix:
                        woundTag = "(Rampaging Infection)".Colour(Telnet.BoldRed);
                        break;
                    default:
                        throw new ApplicationException(
                            $"Unexpected Infection Stage {(int)InfectionStage:F0} encountered in SendIntensityMessage() for wounds");
                }

                break;
            default:
                break;
        }

        return woundTag;
    }

    //Returns true if the stage changed
    protected override bool UpdateInfectionStage(double oldIntensity)
    {
        InfectionStage oldStage = InfectionStage;
        InfectionStage = DetermineStage(Intensity);

        if (oldStage != InfectionStage)
        {
            SendIntensityMessage(oldIntensity > Intensity);
            return true;
        }

        return false;
    }

    protected InfectionStage DetermineStage(double intensity)
    {
        return DetermineStandardStage(intensity);
    }

    // As a rule of thumb, external wounds are noticeably  infected at 100 intensity. 
    // Internal organs and body parts are noticeably infected at 150 intensity
    protected override void SendIntensityMessage(bool improving)
    {
        string echoMesg = "";
        if (Wound != null && !(Bodypart is IOrganProto))
        {
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                    break; //No echos for PCs in this stage
                case InfectionStage.StageTwo: // no pain
                    if (improving)
                    {
                        echoMesg =
                            $"The {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()} no longer has a sharp sting.";
                    }

                    break;
                case InfectionStage.StageThree: // stinging
                    echoMesg = improving
                        ? $"The {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()} is no longer warm to the touch."
                        : $"The {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()} has developed a sharp stinging.";
                    break;
                case InfectionStage.StageFour: // warm
                    echoMesg = improving
                        ? $"The swelling has gone down in the {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()}"
                        : $"The {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()} is very warm to the touch.";
                    break;
                case InfectionStage.StageFive: // swollen
                    echoMesg = improving
                        ? $"The {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()} is no longer seeping pus."
                        : $"The {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()} is red and swollen.";
                    break;
                case InfectionStage.StageSix: // seeping pus
                    echoMesg =
                        $"The {Wound.Severity.Describe()} {Wound.DamageType.Describe()} on your {Wound.Bodypart.FullDescription()} has begun to seep pus.";
                    break;
                default:
                    throw new ApplicationException(
                        "Unexpected Infection Stage encountered in SendIntensityMessage() for wounds");
            }

            if (echoMesg != string.Empty)
            {
                Owner.OutputHandler.Send(echoMesg);
            }

            return;
        }

        // If we hit this, we're dealing with general organ infections rather than a specific wound infection.
        if (Bodypart is BrainProto)
        {
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                case InfectionStage.StageTwo: // no symptoms
                    break;
                case InfectionStage.StageThree: // no pain
                    if (improving)
                    {
                        echoMesg = $"The sharp pain in your head has eased off.";
                    }

                    break;
                case InfectionStage.StageFour: // headache
                    echoMesg = improving
                        ? $"Your fever dies down but your head is still experiencing sharp pain."
                        : $"A sharp pain is starting to build in your head.";
                    break;
                case InfectionStage.StageFive: // fever
                    echoMesg = improving
                        ? $"Your nausa has eased off a little but you are still fevered and your head has a sharp pain."
                        : $"Your headache has become intense and you feel fevered.";
                    break;
                case InfectionStage.StageSix: // stomach sickness
                                              //TODO - Apply an actual nausa effect here?
                    echoMesg = $"Your headache has become agonizing and you feel extremely sick to your stomach.";
                    break;
                default:
                    throw new ApplicationException(
                        "Unexpected Infection Stage encountered in SendIntensityMessage() for brain");
            }
        }
        else if (Bodypart is HeartProto)
        {
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                case InfectionStage.StageTwo: // No symptoms
                    break;
                case InfectionStage.StageThree: // chills improve
                    if (improving)
                    {
                        echoMesg = $"The chills you were filling earlier seem to have subsided.";
                    }

                    break;
                case InfectionStage.StageFour: // chills, needs to be detected by triage too
                    echoMesg = improving
                        ? $"Your muscle aches and joint pains seem to have subsided."
                        : $"You are starting to feel chills while your forehead beads with sweat.";
                    break;
                case InfectionStage.StageFive: // muscle and joint pain
                    echoMesg = improving
                        ? $"The patches of purple and red on your skin have begun to fade away."
                        : $"Your muscles and joints have begun to ache.";
                    break;
                case InfectionStage.StageSix: // red spots on the skin, needs to be detected by triage too.
                    echoMesg = $"Ghastly dark red and purple spots have begun to appear on the surface of your skin.";
                    break;
                default:
                    throw new ApplicationException(
                        "Unexpected Infection Stage encountered in SendIntensityMessage() for heart");
            }
        }
        else if (Bodypart is LiverProto)
        {
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                case InfectionStage.StageTwo: // no symptoms
                    break;
                case InfectionStage.StageThree: // 
                    if (improving)
                    {
                        echoMesg = $"The itch on the surface of your abdomen has cleared up.";
                    }

                    break;
                case InfectionStage.StageFour: // itching rash
                    echoMesg = improving
                        ? $"The tenderness in the upper right corner of your abdomen has dulled and faded."
                        : $"The skin on your abdomen has developed an itchy rash.";
                    break;
                case InfectionStage.StageFive: // tenderness
                    echoMesg = improving
                        ? $"The sense of nausea has faded but your abdomen still hurts."
                        : $"The upper right corner of your abdomen feels painfully tender.";
                    break;
                case InfectionStage.StageSix: // nausa
                                              //TODO - Apply actual nausea effect?
                    echoMesg =
                        $"The pain in the upper right corner of your abdomen has become agonizing and you feel extremely sick to your stomach.";
                    break;
                default:
                    throw new ApplicationException(
                        "Unexpected Infection Stage encountered in SendIntensityMessage() for liver");
            }
        }
        else if (Bodypart is StomachProto)
        {
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                case InfectionStage.StageTwo: // no symptoms
                    break;
                case InfectionStage.StageThree:
                    if (improving)
                    {
                        echoMesg = $"The sense of bloating you were feeling before has passed.";
                    }

                    break;
                case InfectionStage.StageFour: // bloating
                    echoMesg = improving
                        ? $"Your stomach ache has faded."
                        : $"You have begun to feel extremely bloated.";
                    break;
                case InfectionStage.StageFive: // stomach ache
                    echoMesg = improving
                        ? $"Your nausea has faded but your stomach still aches intensely."
                        : $"You feel a stomach ache best described as a dull, gnawing pain.";
                    break;
                case InfectionStage.StageSix: // nausea 
                    echoMesg = $"Your stomach ache has become agonizing and you are overcome with a sense of nausea.";
                    break;
                default:
                    throw new ApplicationException(
                        "Unexpected Infection Stage encountered in SendIntensityMessage() for stomach");
            }
        }
        else if (Bodypart is LungProto)
        {
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                case InfectionStage.StageTwo: // no symptoms
                    break;
                case InfectionStage.StageThree:
                    if (improving)
                    {
                        echoMesg = $"You have not coughed in quite some time.";
                    }

                    break;
                case InfectionStage.StageFour: // coughing
                    echoMesg = improving
                        ? $"You no longer feel quite so short of breath."
                        : $"You have developed a cough.";
                    break;
                case InfectionStage.StageFive: // shortness of breath
                    echoMesg = improving
                        ? $"The sharp, painful tightness in your chest has faded away."
                        : $"You feel short of breath.";
                    break;
                case InfectionStage.StageSix: // Pleuritic chest pain
                    echoMesg = $"You feel a sharp, aching pain in your chest and find it hard to breath.";
                    break;
                default:
                    throw new ApplicationException(
                        "Unexpected Infection Stage encountered in SendIntensityMessage() for lung.");
            }
        }
        else if (Bodypart != null)
        {
            // Figure out it's an organ and if so, display the part that contains it instead.
            IBodypart containingPart = Owner.Bodyparts.FirstOrDefault(x => x.Organs.Any(y => y.Id == Bodypart.Id)) ??
                                 Bodypart;
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                case InfectionStage.StageTwo: // no symptoms
                    break;
                case InfectionStage.StageThree:
                    if (improving)
                    {
                        echoMesg = $"The throbbing ache in your {containingPart.FullDescription()} has subsided.";
                    }

                    break;
                case InfectionStage.StageFour: // Aching
                    echoMesg = improving
                        ? $"While your {containingPart.FullDescription()} still aches, the redness has gone down."
                        : $"Your {containingPart.FullDescription()} has begun to throb with an internal ache.";
                    break;
                case InfectionStage.StageFive: // Redness
                    echoMesg = improving
                        ? $"The swelling in your {containingPart.FullDescription()} has gone down but it still throbs painfully."
                        : $"Your {containingPart.FullDescription()} is aching and streaks of red are visible beneath the skin.";
                    break;
                case InfectionStage.StageSix: // Swelling and tenderness
                                              //Needs to be visible on triage
                    echoMesg =
                        $"Your {containingPart.FullDescription()} is swollen and red and throbbing with agonizing pain.";
                    break;
                default:
                    throw new ApplicationException(
                        $"Unexpected Infection Stage encountered in SendIntensityMessage() for {containingPart.FullDescription()}.");
            }
        }
        else
        {
            switch (InfectionStage)
            {
                case InfectionStage.StageZero:
                case InfectionStage.StageOne:
                case InfectionStage.StageTwo: // No symptoms
                    break;
                case InfectionStage.StageThree:
                    if (improving)
                    {
                        echoMesg = $"Your fever has subsided.";
                    }

                    break;
                case InfectionStage.StageFour: // Mild fever
                    echoMesg = improving
                        ? $"Your aches and pains have subsided but you still feel fevered."
                        : $"You feel a slight fever.";
                    break;
                case InfectionStage.StageFive: // Severe fevere with muscle aches
                    echoMesg = improving
                        ? $"Your chills have subsided but you still feel feverish and are experiencing muscle aches."
                        : $"Your fever feels more intense and your are experiencing muscle aches and joint pain.";
                    break;
                case InfectionStage.StageSix: // Intense fever, chills, and shivering
                    echoMesg = $"You are experiencing feverish chills, muscle aches, and joint pain.";
                    break;
                default:
                    throw new ApplicationException(
                        $"Unexpected Infection Stage encountered in SendIntensityMessage() for generic.");
            }
        }

        if (echoMesg != string.Empty)
        {
            Owner.OutputHandler.Send(echoMesg);
        }
    }

    public override bool InfectionIsDamaging()
    {
        return (int)InfectionStage >= Gameworld.GetStaticInt("SimpleInfectionDamagingStageMinimum") &&
               Wound == null &&
               Bodypart is IOrganProto;
    }

    private int _infectionSpreadCounter;

    public override bool InfectionCanSpread()
    {
        if ((int)InfectionStage < Gameworld.GetStaticInt("SimpleInfectionSpreadingStageMinimum") || Immunity >= 1.0)
        {
            return false;
        }

        _infectionSpreadCounter++;
        if (_infectionSpreadCounter >= Gameworld.GetStaticInt("SimpleInfectionSpreadTicks"))
        {
            _infectionSpreadCounter = 0;
            return true;
        }

        return false;
    }

    public override IDamage GetInfectionDamage()
    {
        return new Damage
        {
            Bodypart = Bodypart,
            DamageType = DamageType.Cellular,
            DamageAmount = Intensity * Gameworld.GetStaticDouble("SimpleInfectionDamagePerIntensity")
        };
    }

    protected override void HandleInfectionSideEffects()
    {
        UpdateNauseaEffect();
    }

    protected override void OnDelete()
    {
        RemoveNauseaEffect();
    }

    private bool ShouldHaveNauseaEffect()
    {
        if (InfectionStage < InfectionStage.StageSix)
        {
            return false;
        }

        return Bodypart is BrainProto or LiverProto or StomachProto || Bodypart == null;
    }

    private double NauseaIntensity => Math.Max(0.0,
        (Intensity - Gameworld.GetStaticDouble("InfectionNauseaThreshold")) *
        Gameworld.GetStaticDouble("InfectionNauseaMultiplier"));

    private void UpdateNauseaEffect()
    {
        if (!ShouldHaveNauseaEffect())
        {
            RemoveNauseaEffect();
            return;
        }

        if (_nauseaEffect == null || !Owner.EffectsOfType<InfectionNausea>().Contains(_nauseaEffect))
        {
            _nauseaEffect = new InfectionNausea(Owner, NauseaIntensity);
            Owner.AddEffect(_nauseaEffect);
            return;
        }

        _nauseaEffect.Intensity = NauseaIntensity;
    }

    private void RemoveNauseaEffect()
    {
        if (_nauseaEffect == null)
        {
            return;
        }

        Owner.RemoveEffect(_nauseaEffect, true);
        _nauseaEffect = null;
    }
}
