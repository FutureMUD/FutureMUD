using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.CharacterCreation {
    public interface ICharacterTemplate : IFutureProgVariable, IHaveFuturemud, IHaveTraits {
        List<IAccent> SelectedAccents { get; }
        List<ITrait> SelectedAttributes { get; }
        MudDate SelectedBirthday { get; }
        List<(ICharacteristicDefinition, ICharacteristicValue)> SelectedCharacteristics { get; }
        ICulture SelectedCulture { get; }
        List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns { get; }
        IEthnicity SelectedEthnicity { get; }
        string SelectedFullDesc { get; }
        Gender SelectedGender { get; }
        double SelectedHeight { get; }
        IPersonalName SelectedName { get; }
        IRace SelectedRace { get; }
        string SelectedSdesc { get; }
        List<ITraitDefinition> SelectedSkills { get; }
		List<(ITraitDefinition, double)> SkillValues { get; }
        double SelectedWeight { get; }
        ICell SelectedStartingLocation { get; }
        List<IChargenRole> SelectedRoles { get; }
        IAccount Account { get; }
        List<ICharacterMerit> SelectedMerits { get; }
        List<IKnowledge> SelectedKnowledges { get; }
        string NeedsModel { get; }
        Alignment Handedness { get; }
        List<IBodypart> MissingBodyparts { get; }
        List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)> SelectedDisfigurements { get; }
        List<IGameItemProto> SelectedProstheses { get; }

        XElement SaveToXml();
	}
}