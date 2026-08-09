
using System.Threading;

namespace MudSharp.TimeAndDate.Listeners;

public abstract class ListenerBase : FrameworkItem, ITemporalListener
{
    private static long _nextId;

    private int _repeatTimes;
    private int _cancelled;
    protected object[] Objects;
    protected string DebuggerReference;

    protected ListenerBase(int repeatTimes, Action<object[]> payload, object[] objects, string debuggerReference = "Unspecified Payload")
    {
        _id = GetNextId();
        _repeatTimes = repeatTimes;
        Payload = payload;
        Objects = objects;
        DebuggerReference = debuggerReference;
    }

    public Action<object[]> Payload { get; protected set; }

    protected void TriggerPayload()
    {
        if (Volatile.Read(ref _cancelled) != 0)
        {
            return;
        }

        Payload?.Invoke(Objects);
        RepeatTimes--;
    }

    public int RepeatTimes
    {
        get => _repeatTimes;
        protected set
        {
            _repeatTimes = value;
            if (_repeatTimes <= 0)
            {
                CancelListener();
            }
        }
    }

    #region ITemporalListener Members

    public virtual bool PertainsTo(object item)
    {
        return Objects?.Contains(item) ?? false;
    }

    public void CancelListener()
    {
        if (Interlocked.Exchange(ref _cancelled, 1) != 0)
        {
            return;
        }

        UnSubscribe();
        Payload = null;
        Futuremud.Games.FirstOrDefault()?.Destroy(this);
    }

    #endregion

    private static long GetNextId()
    {
        return Interlocked.Increment(ref _nextId);
    }

    public abstract void UnSubscribe();
}
