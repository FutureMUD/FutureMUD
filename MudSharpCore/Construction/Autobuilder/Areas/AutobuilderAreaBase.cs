using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Construction.Autobuilder.Areas;

public abstract class AutobuilderAreaBase : SaveableItem, IAutobuilderArea, IHaveFuturemud
{
	public sealed override string FrameworkItemType => "AutobuilderArea";

	protected readonly List<IAutobuilderParameter> _parameters = new();

	public IEnumerable<IAutobuilderParameter> Parameters => _parameters;

	protected AutobuilderAreaBase(string name, IFuturemud gameworld, string type)
	{
		Gameworld = gameworld;
		_name = name;
		ShowCommandByLine = "An undescribed autobuilder area template";
		using (new FMDB())
		{
			var dbitem = new Models.AutobuilderAreaTemplate
			{
				Name = name,
				TemplateType = type,
				Definition = SaveToXml().ToString()
			};
			FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		SetupParameters();
	}

	protected AutobuilderAreaBase(Models.AutobuilderAreaTemplate area, IFuturemud gameworld)
	{
		_id = area.Id;
		_name = area.Name;
		Gameworld = gameworld;
		LoadFromXml(XElement.Parse(area.Definition));
		SetupParameters();
	}

	protected abstract void LoadFromXml(XElement element);

	protected abstract XElement SaveToXml();

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.AutobuilderAreaTemplates.Find(Id);
		dbitem.Name = Name;
		dbitem.Definition = SaveToXml().ToString();
		Changed = false;
	}

	protected abstract void SetupParameters();

	public (bool Success, string ErrorMessage, IEnumerable<object> Arguments) TryArguments(ICharacter builder,
		StringStack ss)
	{
		var args = new List<object>();
		foreach (var item in Parameters)
		{
			var arg = ss.PopSpeech();
			if (string.IsNullOrEmpty(arg))
			{
				if (!item.IsOptional)
				{
					return (false, item.MissingErrorMessage, new object[] { });
				}

				args.Add(null);
				continue;
			}

			if (!item.IsValidArgument(arg, args.ToArray()))
			{
				return (false, item.WhyIsNotValidArgument(arg, args.ToArray()), new object[] { });
			}

			args.Add(item.GetArgument(arg));
		}

		return (true, string.Empty, args);
	}

	public abstract IEnumerable<ICell> ExecuteTemplate(ICharacter builder, IEnumerable<object> arguments);

	public abstract string Show(ICharacter builder);

	public string ShowCommandByLine { get; protected set; }

	public abstract string SubtypeHelpText { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "byline":
			case "summary":
				return BuildingCommandSummary(actor, command);
			default:
				actor.OutputHandler.Send($@"You can use the following options with this autobuilder area template:

	name <name> - renames the template
	summary <text> - edits the summary byline for LIST and SHOW{SubtypeHelpText}");
				return false;
		}
	}

	protected bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this autobuilder area template?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.AutobuilderAreas.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already an autobuilder area template with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename the autobuilder area template formerly known as {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	protected bool BuildingCommandSummary(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What text do you want to set for the summary for this autobuilder area template in the LIST and SHOW commands?");
			return false;
		}

		ShowCommandByLine = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This autobuilder area template will now have the following summary text in the LIST and SHOW commands:\n{ShowCommandByLine.ColourCommand()}");
		return true;
	}

	public abstract IAutobuilderArea Clone(string newName);
}