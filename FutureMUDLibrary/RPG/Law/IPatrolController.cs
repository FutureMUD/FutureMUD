namespace MudSharp.RPG.Law
{
    public interface IPatrolController
    {
        ILegalAuthority LegalAuthority { get; init; }
        void ReportPatrolAborted(IPatrol patrol);
        void ReportPatrolComplete(IPatrol patrol);
        void PatrolOverwatchTick();
    }
}