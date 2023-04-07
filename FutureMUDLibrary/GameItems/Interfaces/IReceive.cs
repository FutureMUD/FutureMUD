using MudSharp.Communication.Language;

namespace MudSharp.GameItems.Interfaces {
    public interface IReceive : IGameItemComponent {
        void ReceiveTransmission(double frequency, SpokenLanguageInfo spokenLanguage, long encryption, ITransmit origin);
        void ReceiveTransmission(double frequency, string dataTransmission, long encryption, ITransmit origin);
    }
}