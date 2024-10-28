using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law;

public class LegalClass : SaveableItem, ILegalClass
{
	public override string FrameworkItemType => "LegalClass";

	public LegalClass(string name, ILegalAuthority authority, IFutureProg membershipProg)
	{
		Gameworld = authority.Gameworld;
		_name = name;
		Authority = authority;
		MembershipProg = membershipProg;
		using (new FMDB())
		{
			var dbitem = new Models.LegalClass();
			FMDB.Context.LegalClasses.Add(dbitem);
			dbitem.Name = name;
			dbitem.LegalAuthorityId = authority.Id;
			dbitem.MembershipProgId = membershipProg.Id;
			dbitem.CanBeDetainedUntilFinesPaid = false;
			LegalClassPriority = Authority.LegalClasses.Select(x => x.LegalClassPriority).DefaultIfEmpty(0).Max() + 1;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public LegalClass(MudSharp.Models.LegalClass dbitem, ILegalAuthority authority, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Authority = authority;
		_id = dbitem.Id;
		_name = dbitem.Name;
		CanBeDetainedUntilFinesPaid = dbitem.CanBeDetainedUntilFinesPaid;
		MembershipProg = Gameworld.FutureProgs.Get(dbitem.MembershipProgId);
		LegalClassPriority = dbitem.LegalClassPriority;
	}

	public ILegalAuthority Authority { get; private set; }
	public int LegalClassPriority { get; private set; }
	public bool CanBeDetainedUntilFinesPaid { get; private set; }

	public IFutureProg MembershipProg { get; private set; }

	public bool IsMemberOfClass(ICharacter actor)
	{
		return MembershipProg.Execute<bool?>(actor) ?? false;
	}

	public void Delete()
	{
		if (Changed)
		{
			Gameworld.SaveManager.Abort(this);
		}

		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.LegalClasses.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.LegalClasses.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.LegalClasses.Find(Id);
		dbitem.Name = Name;
		dbitem.LegalClassPriority = LegalClassPriority;
		dbitem.CanBeDetainedUntilFinesPaid = CanBeDetainedUntilFinesPaid;
		dbitem.MembershipProgId = MembershipProg.Id;
		Changed = false;
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "detain":
				return BuildingCommandDetain(actor, command);
			case "show":
			case "view":
			case "":
				return BuildingCommandView(actor, command);
		}

		actor.OutputHandler.Send(
			"That is not a valid building command for legal classes. You can use the following subcommands:\n\tshow - shows the legal class\n\tname <name> - renames the class\n\tprog <prog> - sets the prog that determines class membership\n\tpriority <number> - sets the priority for evaluating relative to other classes. Higher is higher priority\n\tdetain - toggles whether this class can be detained in prison until their fines are paid");
		return false;
	}

	private bool BuildingCommandView(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(Show(actor));
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this legal class?");
			return false;
		}

		var newName = command.PopSpeech().TitleCase();
		if (Authority.LegalClasses.Any(x => x.Name.EqualTo(newName)))
		{
			actor.OutputHandler.Send("There is already a legal class with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename the {_name.Colour(Telnet.Cyan)} legal class to {newName.Colour(Telnet.Cyan)}");
		_name = newName;
		Changed = true;
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to set as the membership prog for this legal class?");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The {prog.MXPClickableFunctionName()} prog does not return a boolean value, which is a requirement for a membership prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"The {prog.MXPClickableFunctionName()} prog does not accept a character as a parameter, which is a requirement for a membership prog.");
			return false;
		}

		MembershipProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {_name.Colour(Telnet.Cyan)} legal class will decide its membership based on the {prog.MXPClickableFunctionNameWithId()} prog.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What priority should this legal class have in order of evaluation?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		LegalClassPriority = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {_name.Colour(Telnet.Cyan)} legal class now has an evaluation priority of {value.ToString("N0", actor).ColourValue()}, meaning it is {Authority.LegalClasses.OrderByDescending(x => x.LegalClassPriority).ToList().IndexOf(this).ToOrdinal().ColourValue()} in order of evaluation.");
		return true;
	}

	private bool BuildingCommandDetain(ICharacter actor, StringStack command)
	{
		CanBeDetainedUntilFinesPaid = !CanBeDetainedUntilFinesPaid;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {_name.Colour(Telnet.Cyan)} legal class will {(CanBeDetainedUntilFinesPaid ? "now" : "no longer")} be detained if it has unpaid fines.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Legal Class #{Id.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine(
			$"Membership Prog: {MembershipProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Detainable For Fines: {CanBeDetainedUntilFinesPaid.ToColouredString()}");
		return sb.ToString();
	}
}