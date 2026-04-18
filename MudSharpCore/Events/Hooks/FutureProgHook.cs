using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Events.Hooks;

public class FutureProgHook : HookBase, IHookWithProgs, IExecuteProgHook
{
    private readonly List<IFutureProg> _futureProgs = new();

    public FutureProgHook(Models.Hooks hook, IFuturemud gameworld)
        : base(hook, gameworld)
    {
        LoadFromXml(XElement.Parse(hook.Definition));
    }

    public IEnumerable<IFutureProg> FutureProgs => _futureProgs;

    public FutureProgHook(string name, IFuturemud gameworld, EventType type, IFutureProg prog) : base(gameworld, type)
    {
        _name = name;
        Category = "Uncategorised";
        _futureProgs.Add(prog);
        using (new FMDB())
        {
            Models.Hooks dbitem = new();
            FMDB.Context.Hooks.Add(dbitem);
            dbitem.Name = name;
            dbitem.Definition = new XElement("Definition", new XElement("FutureProg", prog.Id)).ToString();
            dbitem.Category = "Uncategorised";
            dbitem.TargetEventType = (int)type;
            dbitem.Type = "FutureProgHook";
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }

        Gameworld.Add(this);
    }

    public override Func<EventType, object[], bool> Function => (type, paramList) =>
                                                                         {
                                                                             if (type != Type)
                                                                             {
                                                                                 return false;
                                                                             }

                                                                             foreach (IFutureProg prog in _futureProgs)
                                                                             {
                                                                                 prog.Execute(paramList);
                                                                             }

                                                                             return true;
                                                                         };

    public static void RegisterLoader()
    {
        HookLoaders.Add("FutureProgHook", (hook, gameworld) => new FutureProgHook(hook, gameworld));
    }

    public void LoadFromXml(XElement element)
    {
        foreach (XElement item in element.Elements("FutureProg"))
        {
            _futureProgs.Add(long.TryParse(item.Value, out long value)
                ? Gameworld.FutureProgs.Get(value)
                : Gameworld.FutureProgs.GetByName(item.Value));
        }
    }

    /// <inheritdoc />
    protected override XElement SaveDefinition()
    {
        return new XElement("Definition",
            from item in _futureProgs
            select new XElement("FutureProg", item.Id)
        );
    }

    public override string InfoForHooklist =>
        $"Executes {_futureProgs.SelectNotNull(x => x.MXPClickableFunctionName()).ListToString()}";

    public void AddProg(IFutureProg prog)
    {
        if (!_futureProgs.Contains(prog))
        {
            _futureProgs.Add(prog);
        }
    }

    public bool RemoveProg(IFutureProg prog)
    {
        return _futureProgs.Remove(prog);
    }
}