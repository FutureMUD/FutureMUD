using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Events.Hooks;

public class HookOnInput : HookBase, IHookWithProgs, ICommandHook, IExecuteProgHook
{
    private readonly List<IFutureProg> _futureProgs = new();
    public IEnumerable<IFutureProg> FutureProgs => _futureProgs;

    public HookOnInput(string name, string command, IFuturemud gameworld, EventType type, IFutureProg prog) : base(
        gameworld, type)
    {
        TargetCommand = command;
        _name = name;
        Category = "Uncategorised";
        _futureProgs.Add(prog);
        using (new FMDB())
        {
            Models.Hooks dbitem = new();
            FMDB.Context.Hooks.Add(dbitem);
            dbitem.Name = name;
            dbitem.Definition = new XElement("Definition", new XElement("TargetCommand", new XCData(TargetCommand)),
                new XElement("FutureProg", prog.Id)).ToString();
            dbitem.Category = "Uncategorised";
            dbitem.TargetEventType = (int)type;
            dbitem.Type = "HookOnInput";
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }

        Gameworld.Add(this);
    }

    public HookOnInput(Models.Hooks hook, IFuturemud gameworld) : base(hook, gameworld)
    {
        LoadFromXml(XElement.Parse(hook.Definition));
    }

    public string TargetCommand { get; set; }

    #region Overrides of HookBase

    public override Func<EventType, object[], bool> Function => (type, paramList) =>
                                                                         {
                                                                             if (type != EventType.CommandInput && type != EventType.SelfCommandInput)
                                                                             {
                                                                                 return false;
                                                                             }

                                                                             string cmd = (type == EventType.CommandInput ? paramList[2] : paramList[1]).ToString();
                                                                             if (!cmd.EqualTo(TargetCommand))
                                                                             {
                                                                                 return false;
                                                                             }

                                                                             StringStack ss = (StringStack)(type == EventType.CommandInput ? paramList[3] : paramList[2]);
                                                                             ss.PopSpeechAll();

                                                                             foreach (IFutureProg prog in _futureProgs)
                                                                             {
                                                                                 if (type == EventType.CommandInput)
                                                                                 {
                                                                                     prog.Execute((ICharacter)paramList[0], paramList[1], cmd, ss.Memory);
                                                                                 }
                                                                                 else
                                                                                 {
                                                                                     prog.Execute((ICharacter)paramList[0], cmd, ss.Memory);
                                                                                 }
                                                                             }

                                                                             return true;
                                                                         };

    #endregion

    public void LoadFromXml(XElement element)
    {
        TargetCommand = element.Element("TargetCommand").Value;
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
            new XElement("TargetCommand", new XCData(TargetCommand)),
            from item in _futureProgs
            select new XElement("FutureProg", item.Id)
        );
    }

    public static void RegisterLoader()
    {
        HookLoaders.Add("HookOnInput", (hook, gameworld) => new HookOnInput(hook, gameworld));
    }

    public override string InfoForHooklist =>
        $"Executes {_futureProgs.SelectNotNull(x => x.MXPClickableFunctionName()).ListToString()}";

    /// <inheritdoc />
    public string CommandText
    {
        get => TargetCommand;
        set => TargetCommand = value;
    }

    /// <inheritdoc />
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
