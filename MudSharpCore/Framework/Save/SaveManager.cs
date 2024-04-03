using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using System.Text;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.Construction;
using MudSharp.Body;
using System.Diagnostics;
using MudSharp.Effects;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using MudSharp.Character.Name;

namespace MudSharp.Framework.Save;

public class SaveManager : ISaveManager
{
	protected readonly List<ILateInitialisingItem> _initialisationQueue = new();
	protected readonly List<ISaveable> _saveStack = new();
	protected readonly List<ISaveable> _delayedSaveStack = new();
	protected Queue<ILazyLoadDuringIdleTime> _lazyLoaders = new();

	#region ISaveManager Members

	public void AddInitialisation(ILateInitialisingItem item)
	{
		_initialisationQueue.Add(item);
	}

	public void Add(ISaveable item)
	{
#if DEBUG
		if (MudBootingMode)
		{
			switch (item)
			{
				case ProgSchedule _:
				case IMeleeWeapon _:
				case IShield _:
				case ProstheticGameItemComponent _:
				case IContainer _:
					break;
				default:
					break;
					throw new ApplicationException(
						"An item was added to the SaveManager during the Booting sequence. This probably shouldn't happen and should be investigated.");
			}
		}
#endif
		_saveStack.Add(item);
	}

	public void Abort(ISaveable item)
	{
		_saveStack.Remove(item);
		_initialisationQueue.Remove(item as ILateInitialisingItem);
		if (_lazyLoaders.Contains(item))
		{
			_lazyLoaders = new Queue<ILazyLoadDuringIdleTime>(_lazyLoaders.ExceptCovariant(item).ToList());
		}
	}

	public void AbortLazyLoad(ILazyLoadDuringIdleTime item)
	{
		if (_lazyLoaders.Contains(item))
		{
			_lazyLoaders = new Queue<ILazyLoadDuringIdleTime>(_lazyLoaders.Except(item).ToList());
		}
	}

	public bool IsQueued(ISaveable saveable)
	{
		return _saveStack.Contains(saveable) || _initialisationQueue.Contains(saveable) ||
		       _lazyLoaders.Contains(saveable);
	}

	public string DebugInfo(IFuturemud gameworld)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"The save queue is currently {(Flushing ? "flushing".Colour(Telnet.Red) : "sleeping".Colour(Telnet.Green))}");
		sb.AppendLine("The following things are waiting to be initialised: ");

		string DescribeThing(ISaveable thing)
		{
			if (thing is ICharacter c)
			{
				return
					$"Character {c.Id} - {c.CurrentName.GetName(NameStyle.SimpleFull)} - {c.HowSeen(c, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)}";
			}

			if (thing is IBody b)
			{
				return
					$"Body {b.Id} - Actor {b.Actor.Id} - {b.Actor.CurrentName.GetName(NameStyle.SimpleFull)} - {b.Actor.HowSeen(b.Actor, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)}";
			}

			if (thing is IGameItem g)
			{
				return $"GameItem {g.Id} - {g.HowSeen(g, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)}";
			}

			if (thing is ICell cell)
			{
				return $"Cell {cell.Id} - {cell.Name}";
			}

			if (thing is IGameItemComponent component)
			{
				return $"GameItemComponent {component.Id} - {component.Prototype.TypeDescription} -  [{component.Prototype.Name} #{component.Prototype.Id}]";
			}

			return thing.ToString();
		}

		foreach (var item in _initialisationQueue.ToList())
		{
			sb.AppendLine(DescribeThing(item));
		}

		sb.AppendLine();
		sb.AppendLine("The following things are waiting to be saved: ");
		foreach (var item in _saveStack.ToList())
		{
			sb.AppendLine(DescribeThing(item));
		}

		sb.AppendLine("The following Characters are Changed without being in the queue.");
		foreach (var character in gameworld.Actors.Where(x => x.Changed && !_saveStack.Contains(x)).ToList())
		{
			sb.AppendLine(DescribeThing(character));
		}

		return sb.ToString();
	}

	private bool _flushingQueue;
	public bool Flushing => _flushingQueue;

	public void Flush()
	{
		if (_flushingQueue)
		{
			return;
		}

		_flushingQueue = true;
		if (_initialisationQueue.Any())
		{
			FlushInitialisation();
		}

		while (_saveStack.Count > 0)
		{
			FlushTop();
		}

		//If any items failed to save this round, queue them up to try on a future save pass
		_saveStack.AddRange(_delayedSaveStack);
		_delayedSaveStack.Clear();

		_flushingQueue = false;
	}

	public void DirectInitialise(ILateInitialisingItem item)
	{
		using (new FMDB())
		{
			var action = item.InitialiseItem();
			action();
			FMDB.Context.SaveChanges();
			Abort(item);
		}
	}

	private void FlushInitialisation()
	{
		using (new FMDB())
		{
			var actions = new List<Action>();
			var firstQueue = _initialisationQueue.Where(x => x.InitialisationPhase == InitialisationPhase.First)
			                                     .ToList();
			var secondQueue = _initialisationQueue.Where(x => x.InitialisationPhase == InitialisationPhase.Second)
			                                      .ToList();
			var thirdQueue = _initialisationQueue
			                 .Where(x => x.InitialisationPhase == InitialisationPhase.AfterFirstDatabaseHit)
			                 .ToList();
			_initialisationQueue.Clear();

			// Some items need to be sure to run before other items (parents with children for example). They may take action in their InitialiseItem() function to prime their subordinates.
			// However, they are all saved to the database at the same time.
			foreach (var item in firstQueue)
			{
				actions.Add(item.InitialiseItem());
			}

			foreach (var item in secondQueue)
			{
				actions.Add(item.InitialiseItem());
			}

			FMDB.Context.SaveChanges();
			foreach (var item in actions)
			{
				item();
			}

			actions.Clear();

			foreach (var item in thirdQueue)
			{
				actions.Add(item.InitialiseItem());
			}

			FMDB.Context.SaveChanges();
			foreach (var item in actions)
			{
				item();
			}

			actions.Clear();

			// Some items may add additional items to the queue to process (particularly things for which an entity relationship cannot be established - e.g. anything that applies to "perceivers"). These must run AFTER the first transactions have been processed.
			foreach (var item in _initialisationQueue)
			{
				actions.Add(item.InitialiseItem());
			}

			_initialisationQueue.Clear();

			FMDB.Context.SaveChanges();
			foreach (var item in actions)
			{
				item();
			}

			actions.Clear();
		}
	}

	private void FlushTop()
	{
#if DEBUG
		try
		{
#endif
			using (new FMDB())
			{
#if DEBUG
				if (_saveStack.Count > 200)
				{
					var sb = new StringBuilder();
					sb.AppendLine($"[PERF] SaveStack has over 200 items in it: {_saveStack.Count}".Colour(Telnet.Orange));
					foreach (var group in _saveStack.OfType<IFrameworkItem>().GroupBy(x => x.FrameworkItemType))
					{
						sb.AppendLine($"\t{group.Key}: {group.Count()}".Colour(Telnet.FunctionYellow));
					}
					Console.WriteLine(sb.ToString());
				}
#endif
				var tempStack = _saveStack.Take(200).ToList();
				_saveStack.RemoveRange(0, Math.Min(_saveStack.Count, 200));
				foreach (var item in tempStack)
				{
					item.Save();
					if (item.Changed == true)
					{
						_delayedSaveStack.Add(item);
#if DEBUG
						//I can't decide if this is a critical error or not. For now I'll just re-queue the item for saving on a future
						//update, but in debug, at least, we should trap this case to take a closer look at the cause. Objects should
						//always end up Changed = false after a Save() call unless there was an error encountered in saving it.
						throw new ApplicationException(
							"Save() called on object being removed from _saveStack but item still flagged as Changed.");
#endif
					}
				}

				FMDB.Context.SaveChanges();
			}

#if DEBUG
		}
		catch (DbUpdateException e)
		{
			var sb = new StringBuilder();
			sb.AppendLine(e.ToString());
			sb.AppendLine();
			sb.AppendLine($"DbUpdateException error details - {e.InnerException?.InnerException?.Message}");

			foreach (var eve in e.Entries)
			{
				sb.AppendLine($"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");
			}

			throw new ApplicationException(sb.ToString());
		}
#endif
	}

	public void AddLazyLoad(ILazyLoadDuringIdleTime item)
	{
		_lazyLoaders.Enqueue(item);
	}

	public void FlushLazyLoad(TimeSpan maximumTime)
	{
		if (!_lazyLoaders.Any())
		{
			return;
		}

		var sw = new Stopwatch();
		sw.Start();
		while (sw.Elapsed < maximumTime)
		{
			_lazyLoaders.Dequeue().DoLoad();
			if (!_lazyLoaders.Any())
			{
				break;
			}
		}

		sw.Stop();
	}

	private bool _mudBootingMode;

	public bool MudBootingMode
	{
		get => _mudBootingMode;
		set
		{
			_mudBootingMode = value;
			if (!value)
			{
				Console.WriteLine("Finalising and Sorting Lazy Load Queue...");
				var sorted = _lazyLoaders.AsEnumerable().OrderByDescending(x => x.LazyLoadPriority).ToList();
				_lazyLoaders.Clear();
				foreach (var item in sorted)
				{
					_lazyLoaders.Enqueue(item);
				}
			}
		}
	}

	#endregion
}