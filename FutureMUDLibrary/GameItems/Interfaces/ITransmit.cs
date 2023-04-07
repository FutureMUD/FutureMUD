using MudSharp.Communication.Language;

namespace MudSharp.GameItems.Interfaces {
    public interface ITransmit : IGameItemComponent {
        bool ManualTransmit { get; }
        string TransmitPremote { get; }
        void Transmit(SpokenLanguageInfo spokenLanguage);
    }
}