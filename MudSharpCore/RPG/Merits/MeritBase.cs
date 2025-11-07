using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.RPG.Merits;

public abstract class MeritBase : SaveableItem, IMerit
{
	protected MeritBase(Merit merit, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = merit.Id;
		_name = merit.Name;
		MeritType = (MeritType)merit.MeritType;
		MeritScope = (MeritScope)merit.MeritScope;
	}

	protected MeritBase(IFuturemud gameworld, string name, MeritType type, MeritScope scope, string databaseType)
	{
		Gameworld = gameworld;
		_name = name;
		MeritType = type;
		MeritScope = scope;
		DatabaseType = databaseType;
		ApplicabilityProg = gameworld.AlwaysTrueProg;
	}

	protected MeritBase()
	{

	}

	protected void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.Merit
			{
				Name = Name,
				MeritScope = (int)MeritScope,
				MeritType = (int)MeritType,
				Type = DatabaseType,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.Merits.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IMerit Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.Merit
			{
				Name = newName,
				Type = DatabaseType,
				MeritType = (int)MeritType,
				MeritScope = (int)MeritScope,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.Merits.Add(dbitem);
			FMDB.Context.SaveChanges();
			return MeritFactory.LoadMerit(dbitem, Gameworld);
		}
	}

	public override string FrameworkItemType => "Merit";

	#region IMerit Members

	public MeritScope MeritScope { get; protected set; }

	public MeritType MeritType { get; protected set; }

	public string DatabaseType { get; protected set; }

	public IFutureProg ApplicabilityProg { get; protected set; }

	public abstract bool Applies(IHaveMerits owner);

	public abstract string Describe(IHaveMerits owner, IPerceiver voyeur);

	#endregion

	#region IFutureProgVariable Members

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Merit, DotReferenceHandler(),
			DotReferenceHelp());
	}


	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			default:
				throw new NotSupportedException("MeritBase.GetProperty requested invalid Merit Property " + property);
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.Merit;

	public object GetObject => this;

	#endregion

	public string HelpText => @$"You can use the following options with this merit:

	#3name <name>#0 - changes the name
	#3type#0 - toggles between this being a merit or flaw{SubtypeHelp}";

	protected virtual string SubtypeHelp => "";

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "type":
			case "merit":
			case "flaw":
				return BuildingCommandType(actor);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandType(ICharacter actor)
	{
		switch (MeritType)
		{
			case MeritType.Flaw:
				MeritType = MeritType.Merit;
				break;
			case MeritType.Merit:
				MeritType = MeritType.Flaw;
				break;
		}

		Changed = true;
		actor.OutputHandler.Send($"This quirk is now considered a {MeritType.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this merit?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Merits.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a merit called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this merit from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Merit #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Magenta, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {MeritType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Scope: {MeritScope.DescribeEnum().ColourValue()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	public override void Save()
	{
		// TODO _ remove after debug
		try
		{
			var definition = SaveDefinition().ToString();
		}
		catch
		{
			Changed = false;
			return;
		}
		
		var dbitem = FMDB.Context.Merits.Find(Id);
		dbitem.Name = Name;
		dbitem.MeritType = (int)MeritType;
		dbitem.MeritScope = (int)MeritScope;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	public virtual XElement SaveDefinition() => new XElement("Definition");
}