using System;

namespace MudSharp.Framework
{
    public interface IGameStatistics
    {
        int RecordOnlinePlayers { get; set; }
        DateTime RecordOnlinePlayersDateTime { get; set; }
        DateTime LastBootTime { get; set; }
        TimeSpan LastStartupSpan { get; set; }
        void LoadStatistics(IFuturemudLoader game);
        bool UpdateOnlinePlayers();

        void UpdateActiveAccount(Models.Account account);
        void UpdateNewAccount();
        void UpdateApplicationSubmitted();
        void UpdateApplicationApproved();
        void UpdatePlayerDeath();
        void UpdateNonPlayerDeath();
        void DoPlayerActivitySnapshot();
        bool RecordPlayersPaused { get; set; }

        Models.WeeklyStatistic GetOrCreateWeeklyStatistic();
    }
}