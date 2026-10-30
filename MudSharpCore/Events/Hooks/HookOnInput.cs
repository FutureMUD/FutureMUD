using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Events.Hooks;

public class HookOnInput : HookBase, IHookWithProgs, IConditionalCommandHook, IExecuteProgHook
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
	public bool UseProgResultForHandling { get; set; }

    #region Overrides of HookBase

	public override Func<EventType, object[], bool> Function => HandleInput;

	private bool HandleInput(EventType type, object[] parameters)
	{
		if (type != EventType.CommandInput && type != EventType.SelfCommandInput) return false;
		var command = parameters[type == EventType.CommandInput ? 2 : 1].ToString();
		var allInput = UseProgResultForHandling && TargetCommand == "*";
		if (!allInput && !command.EqualTo(TargetCommand)) return false;
		var original = (StringStack)parameters[type == EventType.CommandInput ? 3 : 2];
		if (allInput) command += original.RemainingArgument.LeadingSpaceIfNotEmpty();
		// Declining an input must leave the caller's tokens and quotation boundaries intact.
		var arguments = new StringStack(original.RemainingArgument);
		arguments.PopSpeechAll();
		var values = type == EventType.CommandInput
			? new object[] { (ICharacter)parameters[0], parameters[1], command, arguments.Memory }
			: new object[] { (ICharacter)parameters[0], command, arguments.Memory };
		foreach (var prog in _futureProgs)
		{
			if (!UseProgResultForHandling)
			{
				prog.Execute(values);
				continue;
			}
			if (prog?.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) == true && prog.ExecuteBool(values)) return true;
		}
		return !UseProgResultForHandling;
	}

    #endregion

    public void LoadFromXml(XElement element)
    {
        TargetCommand = element.Element("TargetCommand").Value;
		UseProgResultForHandling = (bool?)element.Element("UseProgResultForHandling") ?? false;
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
			new XElement("UseProgResultForHandling", UseProgResultForHandling),
            from item in _futureProgs
            select new XElement("FutureProg", item.Id)
        );
    }

    public static void RegisterLoader()
    {
        HookLoaders.Add("HookOnInput", (hook, gameworld) => new HookOnInput(hook, gameworld));
    }

    public override string InfoForHooklist =>
        $"Executes {_futureProgs.SelectNotNull(x => x.MXPClickableFunctionName()).ListToString()}{(UseProgResultForHandling ? "; boolean results control handling" : "")}";

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
