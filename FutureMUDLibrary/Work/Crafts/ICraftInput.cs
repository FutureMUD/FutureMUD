using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Work.Crafts
{
    public interface ICraftInput : IFrameworkItem, ISaveable {
        double ScoreInputDesirability(IPerceivable item);
        IEnumerable<IPerceivable> ScoutInput(ICharacter character);
        bool IsInput(IPerceivable item);
        void UseInput(IPerceivable item, ICraftInputData data);
        ICraftInputData ReserveInput(IPerceivable input);
        ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld);
        double InputQualityWeight { get; }
        DateTime OriginalAdditionTime { get; }
        bool BuildingCommand(ICharacter actor, StringStack command);
        void CreateNewRevision(Models.Craft dbcraft);
        bool IsValid();
        string WhyNotValid();
        string HowSeen(IPerceiver voyeur);
        bool RefersToItemProto(long id);
        bool RefersToTag(ITag tag);
        bool RefersToLiquid(ILiquid liquid);
	}
}
