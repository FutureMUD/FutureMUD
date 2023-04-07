namespace MudSharp.GameItems.Interfaces {
    public interface IAmmoClip : IContainer {
        string ClipType { get; }
        int Capacity { get;}
        string SpecificAmmoGrade { get; }
    }
}
