using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.CharacterCreation
{
    public enum ChargenStage {
        None = 0,
        SelectRace = 1,
        SelectCulture = 2,
        SelectName = 3,
        SelectDescription = 4,
        SelectBirthday = 5,
        SelectAttributes = 6,
        SelectSkills = 7,
        SelectEthnicity = 8,
        SelectCharacteristics = 9,
        SelectGender = 10,
        SelectHeight = 11,
        SelectWeight = 12,
        SelectNotes = 13,
        ConfirmQuit = 14,
        Submit = 15,
        Menu = 16,
        SelectAccents = 17,
        SelectStartingLocation = 18,
        SelectRole = 19,
        Welcome = 20,
        SelectMerits = 21,
        SelectKnowledges = 22,
        SelectHandedness = 23,
        SpecialApplication = 24,
        SelectDisfigurements = 25
    }

    public enum ChargenState {
        InProgress = 0,
        Submitted = 1,
        Deleted = 2,
        Halt = 3,
        ExternallyApproved = 4,
        Approved = 5
    }

    public static class ChargenStageExtensions {
        public static string Describe(this ChargenStage stage) {
            switch (stage) {
                case ChargenStage.Welcome:
                    return "Welcome";
                case ChargenStage.ConfirmQuit:
                    return "Quit";

                case ChargenStage.Menu:
                    return "Menu";

                case ChargenStage.SelectAttributes:
                    return "Attributes";

                case ChargenStage.SelectBirthday:
                    return "Birthday";

                case ChargenStage.SelectCharacteristics:
                    return "Characteristics";

                case ChargenStage.SelectEthnicity:
                    return "Ethnicity";

                case ChargenStage.SelectCulture:
                    return "Culture";

                case ChargenStage.SelectGender:
                    return "Gender";

                case ChargenStage.SelectHeight:
                    return "Height";

                case ChargenStage.SelectName:
                    return "Name";

                case ChargenStage.SelectDescription:
                    return "Description";

                case ChargenStage.SelectRace:
                    return "Race";

                case ChargenStage.SelectSkills:
                    return "Skills";

                case ChargenStage.SelectNotes:
                    return "Notes";

                case ChargenStage.SelectWeight:
                    return "Weight";

                case ChargenStage.Submit:
                    return "Submit";

                case ChargenStage.SelectAccents:
                    return "Accents";

                case ChargenStage.SelectStartingLocation:
                    return "Location";
                case ChargenStage.SelectRole:
                    return "Role";
                case ChargenStage.SelectMerits:
                    return "Merits";
                case ChargenStage.SelectKnowledges:
                    return "Knowledges";
                case ChargenStage.SelectHandedness:
                    return "Handedness";
                case ChargenStage.SpecialApplication:
                    return "Special App";
                case ChargenStage.SelectDisfigurements:
                    return "Disfigurements";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
