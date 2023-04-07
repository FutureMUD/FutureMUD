using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp.NPC.AI.Groups
{
    public interface IGroupEmote : IXmlSavable
    {
        string EmoteText { get; set; }
        Gender? RequiredGender { get; set; }
        GroupRole? RequiredRole { get; set; }
        GroupRole? RequiredTargetRole { get; set; }
        AgeCategory? RequiredAgeCategory { get; set; }
        GroupAction? RequiredAction { get; set; }
        GroupAlertness MinimumAlertness { get; set; }
        GroupAlertness MaximumAlertness { get; set; }
        string DescribeForShow();
        void DoEmote(IGroupAI herd);
        bool Applies(IGroupAI herd);
    }
}