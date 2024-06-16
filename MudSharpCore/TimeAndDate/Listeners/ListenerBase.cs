using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Listeners;

public abstract class ListenerBase : FrameworkItem, ITemporalListener
{
	private static long _nextId = 1;
	private static readonly Queue<long> ReturnedIdsList = new();

	private int _repeatTimes;
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

	public int RepeatTimes
	{
		get => _repeatTimes;
		protected set
		{
			_repeatTimes = value;
			if (_repeatTimes <= 0)
			{
				UnSubscribe();
				Futuremud.Games.FirstOrDefault()?.Destroy(this);
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
		UnSubscribe();
		Futuremud.Games.First().Destroy(this);
	}

	#endregion

	private static long GetNextId()
	{
		return ReturnedIdsList.Any() ? ReturnedIdsList.Dequeue() : _nextId++;
	}

	~ListenerBase()
	{
		ReturnedIdsList.Enqueue(Id);
	}

	public abstract void UnSubscribe();
}