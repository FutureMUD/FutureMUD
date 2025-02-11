using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Shops;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Shoppers;
internal abstract class ShopperBase : SaveableItem, IShopper
{
	public sealed override string FrameworkItemType => "Shopper";

	private static readonly Dictionary<string, Func<IFuturemud, string, IEconomicZone, IShopper>> _builderLoaders = new(StringComparer.InvariantCultureIgnoreCase);
	private static readonly Dictionary<string, Func<IFuturemud, Models.Shopper, IShopper>> _databaseLoaders = new(StringComparer.InvariantCultureIgnoreCase);
	private static readonly Dictionary<string, string> _typeHelps = new(StringComparer.InvariantCultureIgnoreCase);
	private static readonly Dictionary<string, string> _typeBlurbs = new(StringComparer.InvariantCultureIgnoreCase);

	protected static void RegisterLoader(string type, Func<IFuturemud, Models.Shopper, IShopper> databaseLoader, Func<IFuturemud, string, IEconomicZone, IShopper> builderLoader, string typeHelp, string typeBlurb)
	{
		_builderLoaders[type] = builderLoader;
		_databaseLoaders[type] = databaseLoader;
		_typeHelps[type] = typeHelp;
		_typeBlurbs[type] = typeBlurb;
	}

	public static void RegisterShopperTypes()
	{
		var pType = typeof(IShopper);
		foreach (
			var type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(pType)))
		{
			var method = type.GetMethod("RegisterLoaders", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, []);
		}
	}

	public static IEnumerable<string> Types => _typeBlurbs.Keys.ToList();

	public static (string Blurb, string Help) GetTypeInfoFor(string type)
	{
		return _typeHelps.TryGetValue(type, out var help) ? (_typeBlurbs[type], help) : ("An invalid type", "");
	}

	public static IShopper LoadShopper(Models.Shopper dbitem, IFuturemud gameworld)
	{
		return _databaseLoaders[dbitem.Type](gameworld, dbitem);
	}

	public static IShopper LoadShopper(IFuturemud gameworld, string type, string name, IEconomicZone ez)
	{
		return _builderLoaders[type](gameworld, name, ez);
	}

	protected ShopperBase(IFuturemud gameworld, string name, IEconomicZone ez)
	{
		Gameworld = gameworld;
		_name = name;
		EconomicZone = ez;
		ShoppingInterval = new RecurringInterval
		{
			IntervalAmount = 1,
			Modifier = 0,
			Type = IntervalType.Daily
		};
		NextShop = ez.FinancialPeriodReferenceCalendar.CurrentDateTime + MudTimeSpan.FromHours(2);
		SetupListener();
	}

	protected Models.Shopper DoDatabaseInsert(string type)
	{
		using (new FMDB())
		{
			var dbitem = new Models.Shopper
			{
				Name = Name,
				EconomicZoneId = EconomicZone.Id,
				NextDate = NextShop?.GetDateTimeString() ?? "now",
				Interval = ShoppingInterval.ToString(),
				Type = type,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.Shoppers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			return dbitem;
		}
	}

	protected ShopperBase(){}

	protected ShopperBase(Models.Shopper dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		EconomicZone = Gameworld.EconomicZones.Get(dbitem.EconomicZoneId);
		ShoppingInterval = RecurringInterval.Parse(dbitem.Interval);
		NextShop = new MudDateTime(dbitem.NextDate, Gameworld);
		LoadFromDefinition(XElement.Parse(dbitem.Definition));
		SetupListener();
	}

	protected ShopperBase(ShopperBase rhs, string name, string type)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		EconomicZone = rhs.EconomicZone;
		ShoppingInterval = RecurringInterval.Parse(rhs.ShoppingInterval.ToString());
		NextShop = new MudDateTime(rhs.NextShop);
		LoadFromDefinition(XElement.Parse(DoDatabaseInsert(type).Definition));
		SetupListener();
	}

	private readonly List<(string Type, string Message)> _queuedLogEntries = new();

	protected void DoLogEntry(string type, string message)
	{
		_queuedLogEntries.Add((type, message));
	}

	protected void FlushLogEntries()
	{
		var now = DateTime.UtcNow;
		var mudnow = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime.GetDateTimeString();
		using (new FMDB())
		{
			foreach (var entry in _queuedLogEntries)
			{
				FMDB.Context.ShopperLogs.Add(new ShopperLog
				{
					ShopperId = Id,
					DateTime = now,
					MudDateTime = mudnow,
					LogEntry = entry.Message,
					LogType = entry.Type
				});
			}

			FMDB.Context.SaveChanges();

		}

		_queuedLogEntries.Clear();
	}

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Shoppers.Find(Id);
		dbitem.Name = Name;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.Interval = ShoppingInterval.ToString();
		dbitem.NextDate = NextShop?.GetDateTimeString() ?? "now";
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	protected abstract XElement SaveDefinition();

	protected abstract void LoadFromDefinition(XElement root);

	#endregion

	#region Implementation of IEditableItem

	public string HelpText => $@"You can use the following options with this commnand:

	#3name <name>#0 - update the name
	#3ez <which>#0 - changes the economic zone
	#3interval every <x> hours|days|weekdays|weeks|months|years <offset>#0 - sets the interval
	#3next <date> <time>#0 - sets the next shopping date time
{SubtypeHelpText}";

	protected abstract string SubtypeHelpText { get; }

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "ez":
			case "economiczone":
				return BuildingCommandEconomicZone(actor, command);
			case "interval":
				return BuildingCommandInterval(actor, command);
			case "next":
			case "nextdate":
				return BuildingCommandNextDate(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandNextDate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the next date and time that this shopper shops?");
			return false;
		}

		if (!MudDateTime.TryParse(command.SafeRemainingArgument, EconomicZone.FinancialPeriodReferenceCalendar, EconomicZone.FinancialPeriodReferenceClock, actor, out var dt, out var error))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid date time.\n{error}");
			return false;
		}

		if (dt < EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
		{
			actor.OutputHandler.Send($"You must enter a date time in the future, whereas {dt.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()} is not.");
			return false;
		}

		if (dt == EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
		{
			dt = dt + MudTimeSpan.FromSeconds(1);
		}

		NextShop = dt;
		Changed = true;
		SetupListener();
		actor.OutputHandler.Send($"The next shop for this shopper will now be {dt.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInterval(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter an interval.\nUse the following form: #3every <x> hours|days|weekdays|weeks|months|years <offset>#0".SubstituteANSIColour());
			return false;
		}

		if (!RecurringInterval.TryParse(command.SafeRemainingArgument, out var interval))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid interval.\nUse the following form: #3every <x> hours|days|weekdays|weeks|months|years <offset>#0".SubstituteANSIColour());
			return false;
		}

		ShoppingInterval = interval;
		Changed = true;
		actor.OutputHandler.Send($"This shopper will now shop {interval.Describe(EconomicZone.FinancialPeriodReferenceCalendar).ColourValue()}. This will take effect after their next scheduled shop.");
		return true;
	}

	private bool BuildingCommandEconomicZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to move this shopper to?");
			return false;
		}

		var ez = Gameworld.EconomicZones.GetByIdOrName(command.SafeRemainingArgument);
		if (ez is null)
		{
			actor.OutputHandler.Send($"There is no economic zone identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (EconomicZone == ez)
		{
			actor.OutputHandler.Send($"This shopper already belongs to the {ez.Name.ColourValue()} economic zone.");
			return false;
		}

		if (EconomicZone.FinancialPeriodReferenceCalendar != ez.FinancialPeriodReferenceCalendar)
		{
			NextShop = NextShop.ConvertToOtherCalendar(ez.FinancialPeriodReferenceCalendar);
			EconomicZone = ez;
			SetupListener();
		}
		else
		{
			EconomicZone = ez;
		}

		Changed = true;
		actor.OutputHandler.Send($"This shopper now belongs to the {ez.Name.ColourValue()} economic zone.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a new name.");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Shoppers.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a shopper called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this shopper from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Shopper #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourValue()}");
		sb.AppendLine($"Shopper Interval: {ShoppingInterval.Describe(EconomicZone.FinancialPeriodReferenceCalendar).ColourValue()}");
		if (NextShop is null)
		{
			sb.AppendLine($"Next Shop Date: {"None".ColourError()}");
		}
		else
		{
			sb.AppendLine($"Next Shop Date: {NextShop.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()} ({(NextShop - EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime).DescribePreciseBrief(actor).ColourValue()})");
		}
		
		sb.AppendLine(ShowSubtype(actor));
		return sb.ToString();
	}

	protected abstract string ShowSubtype(ICharacter actor);
	#endregion

	#region Implementation of IShopper
	public abstract string ShopperType { get; }
	protected void ReleaseListener()
	{
		if (_listener is null)
		{
			return;
		}

		_listener.CancelListener();
		_listener = null;
	}

	protected void SetupListener()
	{
		if (_listener is not null)
		{
			ReleaseListener();
		}

		NextShop ??= ShoppingInterval.GetNextDateTime(EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
		if (NextShop <= EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
		{
			NextShop = ShoppingInterval.GetNextAdjacentToCurrent(NextShop);
		}

		_listener = ListenerFactory.CreateDateTimeListener(NextShop, _ => DoShop(), [], $"Shopper #{Id} - {Name}");
	}

	private ITemporalListener _listener;

	/// <inheritdoc />
	public IEconomicZone EconomicZone { get; private set; }

	/// <inheritdoc />
	public RecurringInterval ShoppingInterval { get; private set; }

	/// <inheritdoc />
	public MudDateTime NextShop { get; private set; }

	/// <inheritdoc />
	public abstract void DoShop();

	public abstract IShopper Clone(string name);
	#endregion
}
