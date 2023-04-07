namespace MudSharp.GameItems.Interfaces {
    public interface ILocksmithingTool : IGameItemComponent {
        bool UsableForFabrication { get; }
        bool UsableForConfiguration { get; }
        bool UsableForInstallation { get; }
        bool Breakable { get; }
        int DifficultyAdjustment { get; }
    }
}