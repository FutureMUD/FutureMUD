namespace MudSharp.GameItems.Interfaces {
    public interface IOnOff : IGameItemComponent {
        bool SwitchedOn { get; set; }
    }
}