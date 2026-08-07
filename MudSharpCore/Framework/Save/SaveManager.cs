using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoreLinq.Extensions;
using MudSharp.Body;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using System.Diagnostics;

namespace MudSharp.Framework.Save;

public class SaveManager : ISaveManager
{
    protected readonly List<ILateInitialisingItem> _initialisationQueue = new();
    protected readonly List<ISaveable> _saveStack = new();
    protected readonly List<ISaveable> _delayedSaveStack = new();
    protected Queue<ILazyLoadDuringIdleTime> _lazyLoaders = new();
#if DEBUG
    private readonly Dictionary<string, (int Count, TimeSpan Duration)> _lazyLoadProfile = new();
    private Stopwatch _lazyLoadProfileStopwatch;
#endif

    #region ISaveManager Members

    public void AddInitialisation(ILateInitialisingItem item)
    {
        if (IsNoSave(item))
        {
            return;
        }

        _initialisationQueue.Add(item);
    }

    public void Add(ISaveable item)
    {
        if (IsNoSave(item))
        {
            return;
        }
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
        StringBuilder sb = new();
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

        foreach (ILateInitialisingItem item in _initialisationQueue.ToList())
        {
            sb.AppendLine(DescribeThing(item));
        }

        sb.AppendLine();
        sb.AppendLine("The following things are waiting to be saved: ");
        foreach (ISaveable item in _saveStack.ToList())
        {
            sb.AppendLine(DescribeThing(item));
        }

        sb.AppendLine("The following Characters are Changed without being in the queue.");
        foreach (ICharacter character in gameworld.Actors.Where(x => x.Changed && !_saveStack.Contains(x)).ToList())
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
        if (IsNoSave(item))
        {
            Abort(item);
            return;
        }

        using (new FMDB())
        {
            Action action = item.InitialiseItem();
            action();
            FMDB.Context.SaveChanges();
            Abort(item);
        }
    }

    private void FlushInitialisation()
    {
        using (new FMDB())
        {
            List<Action> actions = new();
            List<ILateInitialisingItem> queuedItems = _initialisationQueue.Where(x => !IsNoSave(x)).ToList();
            List<ILateInitialisingItem> firstQueue = queuedItems.Where(x => x.InitialisationPhase == InitialisationPhase.First)
                                        .ToList();
            List<ILateInitialisingItem> secondQueue = queuedItems.Where(x => x.InitialisationPhase == InitialisationPhase.Second)
                                         .ToList();
            List<ILateInitialisingItem> thirdQueue = queuedItems
                            .Where(x => x.InitialisationPhase == InitialisationPhase.AfterFirstDatabaseHit)
                            .ToList();
            _initialisationQueue.Clear();

            // Some items need to be sure to run before other items (parents with children for example). They may take action in their InitialiseItem() function to prime their subordinates.
            // However, they are all saved to the database at the same time.
            foreach (ILateInitialisingItem item in firstQueue)
            {
                actions.Add(item.InitialiseItem());
            }

            foreach (ILateInitialisingItem item in secondQueue)
            {
                actions.Add(item.InitialiseItem());
            }

            FMDB.Context.SaveChanges();
            foreach (Action item in actions)
            {
                item();
            }

            actions.Clear();

            foreach (ILateInitialisingItem item in thirdQueue)
            {
                actions.Add(item.InitialiseItem());
            }

            FMDB.Context.SaveChanges();
            foreach (Action item in actions)
            {
                item();
            }

            actions.Clear();

            // Some items may add additional items to the queue to process (particularly things for which an entity relationship cannot be established - e.g. anything that applies to "perceivers"). These must run AFTER the first transactions have been processed.
            foreach (ILateInitialisingItem item in _initialisationQueue.Where(x => !IsNoSave(x)))
            {
                actions.Add(item.InitialiseItem());
            }

            _initialisationQueue.Clear();

            FMDB.Context.SaveChanges();
            foreach (Action item in actions)
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
                if (_saveStack.Count > 500)
                {
                    StringBuilder sb = new();
                    sb.AppendLine($"[PERF] SaveStack has over 500 items in it: {_saveStack.Count}".Colour(Telnet.Orange));
                    foreach (IGrouping<string, IFrameworkItem> group in _saveStack.OfType<IFrameworkItem>().GroupBy(x => x.FrameworkItemType))
                    {
                        if (group.Key == "GameItemComponent")
                        {
                            IEnumerable<IGrouping<string, IGameItemComponent>> sub = group.OfType<IGameItemComponent>().GroupBy(x => x.Prototype.TypeDescription);
                            foreach (IGrouping<string, IGameItemComponent> subgroup in sub)
                            {
                                sb.AppendLine($"\tGameItemComponent {subgroup.Key}: {subgroup.Count()}".Colour(Telnet.FunctionYellow));
                            }

                            continue;
                        }
                        sb.AppendLine($"\t{group.Key}: {group.Count()}".Colour(Telnet.FunctionYellow));
                    }
                    Console.WriteLine(sb.ToString());
                }
#endif
                List<ISaveable> tempStack = _saveStack.ToList();
                _saveStack.Clear();
                foreach (ISaveable item in tempStack)
                {
                    if (IsNoSave(item))
                    {
                        item.Changed = false;
                        continue;
                    }

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
            StringBuilder sb = new();
            sb.AppendLine(e.ToString());
            sb.AppendLine();
            sb.AppendLine($"DbUpdateException error details - {e.InnerException?.InnerException?.Message}");

            foreach (EntityEntry eve in e.Entries)
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

#if DEBUG
        _lazyLoadProfileStopwatch ??= Stopwatch.StartNew();
#endif
        Stopwatch sw = new();
        sw.Start();
        while (sw.Elapsed < maximumTime)
        {
            ILazyLoadDuringIdleTime loader = _lazyLoaders.Dequeue();
#if DEBUG
            Stopwatch loaderStopwatch = Stopwatch.StartNew();
#endif
            loader.DoLoad();
#if DEBUG
            loaderStopwatch.Stop();
            string loaderType = loader.GetType().Name;
            _lazyLoadProfile.TryGetValue(loaderType, out (int Count, TimeSpan Duration) current);
            _lazyLoadProfile[loaderType] = (current.Count + 1, current.Duration + loaderStopwatch.Elapsed);
#endif
            if (!_lazyLoaders.Any())
            {
                break;
            }
        }

        sw.Stop();
#if DEBUG
        if (_lazyLoaders.Any())
        {
            return;
        }

        _lazyLoadProfileStopwatch.Stop();
        ConsoleUtilities.WriteLine(
            $"#ECompleted { _lazyLoadProfile.Values.Sum(x => x.Count):N0} idle lazy loads in #2{_lazyLoadProfileStopwatch.ElapsedMilliseconds:N0}ms#0.");
        foreach ((string type, (int count, TimeSpan duration)) in _lazyLoadProfile
                     .OrderByDescending(x => x.Value.Duration)
                     .Take(10))
        {
            ConsoleUtilities.WriteLine($"#E...Lazy {type}: {count:N0} items in #2{duration.TotalMilliseconds:N0}ms#0");
        }

        _lazyLoadProfile.Clear();
        _lazyLoadProfileStopwatch = null;
#endif
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
                ConsoleUtilities.WriteLine("#EFinalising and Sorting Lazy Load Queue...#0");
                List<ILazyLoadDuringIdleTime> sorted = _lazyLoaders.AsEnumerable().OrderByDescending(x => x.LazyLoadPriority).ToList();
                _lazyLoaders.Clear();
                foreach (ILazyLoadDuringIdleTime item in sorted)
                {
                    _lazyLoaders.Enqueue(item);
                }
#if DEBUG
                ConsoleUtilities.WriteLine($"#EQueued {_lazyLoaders.Count:N0} idle lazy-load operations.#0");
#endif
            }
        }
    }

    private static bool IsNoSave(ISaveable item)
    {
        return item switch
        {
            SaveableItem saveableItem => saveableItem.GetNoSave(),
            SavableKeywordedItem savableKeywordedItem => savableKeywordedItem.GetNoSave(),
            LateInitialisingItem lateInitialisingItem => lateInitialisingItem.GetNoSave(),
            LateKeywordedInitialisingItem lateKeywordedInitialisingItem => lateKeywordedInitialisingItem.GetNoSave(),
            _ => false
        };
    }

    private static bool IsNoSave(ILateInitialisingItem item)
    {
        return item switch
        {
            LateInitialisingItem lateInitialisingItem => lateInitialisingItem.GetNoSave(),
            LateKeywordedInitialisingItem lateKeywordedInitialisingItem => lateKeywordedInitialisingItem.GetNoSave(),
            _ => false
        };
    }
    #endregion
}
