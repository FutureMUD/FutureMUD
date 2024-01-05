using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Form.Shape;

public class BodypartShape : SaveableItem, IBodypartShape
{
	public BodypartShape(IFuturemud gameworld, string name)
	{
		_name = name;
		using (new FMDB())
		{
			var dbitem = new Models.BodypartShape
			{
				Name = name
			};
			FMDB.Context.BodypartShapes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public BodypartShape(Models.BodypartShape shape, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = shape.Id;
		_name = shape.Name;
	}

	public override string FrameworkItemType => "BodypartShape";

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.BodypartShapes.Find(Id);
		dbitem.Name = Name;
		Changed = false;
	}

	#endregion

	#region Implementation of IEditableItem

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following commands with this item:

	#3name <name>#0 - renames this bodypart shape".SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this bodypart shape?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.BodypartShapes.Except(this).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a bodypart shape called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the bodypart shape {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Bodypart Shape #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine("Usage:");
		foreach (var bodypart in Gameworld.BodypartPrototypes)
		{
			if (bodypart.Shape != this)
			{
				continue;
			}

			sb.AppendLine(
				$"\tBody #{bodypart.Body.Id.ToString("N0", actor)} ({bodypart.Body.Name.ColourName()}): {bodypart.FullDescription(true)}");
		}

		return sb.ToString();
	}

	#endregion
}