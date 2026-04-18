using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MudSharp.Body.Disfigurements;
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

public class EnforcementAuthority : SaveableItem, IEnforcementAuthority
{
    public override string FrameworkItemType => "EnforcementAuthority";

    public EnforcementAuthority(string name, ILegalAuthority authority)
    {
        Gameworld = authority.Gameworld;
        _name = name;
        LegalAuthority = authority;
        using (new FMDB())
        {
            Models.EnforcementAuthority dbitem = new();
            FMDB.Context.EnforcementAuthorities.Add(dbitem);
            dbitem.LegalAuthorityId = authority.Id;
            dbitem.Name = Name;
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public EnforcementAuthority(MudSharp.Models.EnforcementAuthority dbitem, ILegalAuthority authority,
        IFuturemud gameworld)
    {
        Gameworld = gameworld;
        LegalAuthority = authority;
        _id = dbitem.Id;
        _name = dbitem.Name;
        Priority = dbitem.Priority;
        CanAccuse = dbitem.CanAccuse;
        CanConvict = dbitem.CanConvict;
        CanForgive = dbitem.CanForgive;
        FilteringProg = Gameworld.FutureProgs.Get(dbitem.FilterProgId ?? 0);
        foreach (EnforcementAuthorityParentAuthority included in dbitem.EnforcementAuthoritiesParentAuthoritiesChild)
        {
            _includedAuthoritiesIds.Add(included.ParentId);
        }
    }

    public ILegalAuthority LegalAuthority { get; private set; }

    public IFutureProg FilteringProg { get; private set; }

    private readonly List<ILegalClass> _arrestableClasses = new();
    public IEnumerable<ILegalClass> ArrestableClasses
    {
        get
        {
            List<ILegalClass> classes = new(_arrestableClasses);
            foreach (IEnforcementAuthority other in IncludedAuthorities)
            {
                classes.AddRange(other.ArrestableClasses);
            }
            return classes.Distinct();
        }
    }

    private readonly List<ILegalClass> _accusableClasses = new();
    public IEnumerable<ILegalClass> AccusableClasses
    {
        get
        {
            List<ILegalClass> classes = new(_accusableClasses);
            foreach (IEnforcementAuthority other in IncludedAuthorities)
            {
                classes.AddRange(other.AccusableClasses);
            }
            return classes.Distinct();
        }
    }

    public bool CanAccuse { get; private set; }
    public bool CanForgive { get; private set; }
    public bool CanConvict { get; private set; }

    private readonly List<long> _includedAuthoritiesIds = new();
    private readonly List<IEnforcementAuthority> _includedAuthorities = new();

    public IEnumerable<IEnforcementAuthority> IncludedAuthorities
    {
        get
        {
            if (_includedAuthoritiesIds.Any())
            {
                _includedAuthorities.AddRange(
                    _includedAuthoritiesIds.SelectNotNull(x => Gameworld.EnforcementAuthorities.Get(x)));
                _includedAuthoritiesIds.Clear();
            }

            return _includedAuthorities;
        }
    }

    public bool AlsoIncludesAuthorityFrom(IEnforcementAuthority otherAuthority)
    {
        return AllIncludedAuthorities.Contains(otherAuthority);
    }

    public IEnumerable<IEnforcementAuthority> AllIncludedAuthorities =>
        IncludedAuthorities.SelectMany(x => x.AllIncludedAuthorities).Distinct().Append(this);

    public int Priority { get; protected set; }

    public bool HasAuthority(ICharacter actor)
    {
        return FilteringProg?.Execute<bool?>(actor) == true;
    }


    public bool CanAccuseOfCrime(ICharacter accuser, ICharacter target, ILaw law)
    {
        if (!CanAccuse)
        {
            return false;
        }

        ILegalClass targetClass = LegalAuthority.GetLegalClass(target);
        if (!AccusableClasses.Contains(targetClass))
        {
            return false;
        }

        if (!law.OffenderClasses.Contains(targetClass))
        {
            return false;
        }

        return true;
    }

    public override void Save()
    {
        Models.EnforcementAuthority dbitem = FMDB.Context.EnforcementAuthorities.Find(Id);
        dbitem.Name = Name;
        dbitem.CanAccuse = CanAccuse;
        dbitem.CanForgive = CanForgive;
        dbitem.CanConvict = CanConvict;
        dbitem.Priority = Priority;
        dbitem.FilterProgId = FilteringProg?.Id;
        FMDB.Context.EnforcementAuthoritiesAccusableClasses.RemoveRange(dbitem.EnforcementAuthoritiesAccusableClasses);
        foreach (ILegalClass @class in _accusableClasses)
        {
            dbitem.EnforcementAuthoritiesAccusableClasses.Add(new Models.EnforcementAuthoritiesAccusableClasses
            { EnforcementAuthority = dbitem, LegalClassId = @class.Id });
        }

        FMDB.Context.EnforcementAuthoritiesArrestableClasses.RemoveRange(dbitem
            .EnforcementAuthoritiesArrestableLegalClasses);
        foreach (ILegalClass @class in _arrestableClasses)
        {
            dbitem.EnforcementAuthoritiesArrestableLegalClasses.Add(
                new Models.EnforcementAuthoritiesArrestableLegalClasses
                { EnforcementAuthority = dbitem, LegalClassId = @class.Id });
        }

        FMDB.Context.EnforcementAuthoritiesParentAuthorities.RemoveRange(
            FMDB.Context.EnforcementAuthoritiesParentAuthorities.Where(x => x.ChildId == Id));
        foreach (IEnforcementAuthority authority in _includedAuthorities)
        {
            FMDB.Context.EnforcementAuthoritiesParentAuthorities.Add(new Models.EnforcementAuthorityParentAuthority
            { Child = dbitem, ParentId = authority.Id });
        }

        Changed = false;
    }

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "accuse":
                return BuildingCommandCanAccuse(actor, command);
            case "forgive":
                return BuildingCommandCanForgive(actor);
            case "arrest":
                return BuildinCommandCanArrest(actor, command);
            case "convict":
                return BuildingCommandCanConvict(actor);
            case "also":
                return BuildingCommandAlso(actor, command);
            case "filter":
                return BuildingCommandFilter(actor, command);
            case "show":
            case "view":
            case "":
                return BuildingCommandShow(actor);
        }

        actor.OutputHandler.Send(@"You can use the following sub-commands for editing enforcement authorities:

	#3show#0 - shows the enforcement authority
	#3also <other>#0 - makes this authority include all the other properties from another authority
	#3convict#0 - toggles whether this class can convict/acquit crimes
	#3forgive#0 - toggles whether this class can forgive crimes that they can accuse
	#3accuse <class>#0 - toggles whether this authority can accuse a legal class of crimes
	#3arrest <class>#0 - toggles whether this authority can arrest a member of the legal class
	#3filter <prog>#0 - sets the filter prog for membership in this class".SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandShow(ICharacter actor)
    {
        actor.OutputHandler.Send(Show(actor));
        return true;
    }

    private bool BuildinCommandCanArrest(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which legal class do you want to toggle arrestability for?");
            return false;
        }

        ILegalClass legal = long.TryParse(command.PopSpeech(), out long value)
            ? LegalAuthority.LegalClasses.FirstOrDefault(x => x.Id == value)
            : LegalAuthority.LegalClasses.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
              LegalAuthority.LegalClasses.FirstOrDefault(x =>
                  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
        if (legal == null)
        {
            actor.OutputHandler.Send("There is no such legal class.");
            return false;
        }

        if (_arrestableClasses.Contains(legal))
        {
            _arrestableClasses.Remove(legal);
            Changed = true;
            actor.OutputHandler.Send(
                $"The {Name.Colour(Telnet.Cyan)} enforcement authority can no longer arrest the {legal.Name.Colour(Telnet.Cyan)} legal class.");
            return true;
        }

        _arrestableClasses.Add(legal);
        Changed = true;
        actor.OutputHandler.Send(
            $"The {Name.Colour(Telnet.Cyan)} enforcement authority can now arrest the {legal.Name.Colour(Telnet.Cyan)} legal class.");
        return true;
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name do you want to give to this enforcement authority?");
            return false;
        }

        string newName = command.PopSpeech().TitleCase();
        if (LegalAuthority.EnforcementAuthorities.Any(x => x.Name.EqualTo(newName)))
        {
            actor.OutputHandler.Send("There is already an enforcement authority with that name. Names must be unique.");
            return false;
        }

        actor.OutputHandler.Send(
            $"You rename the {Name.Colour(Telnet.Cyan)} enforcement authority to {newName.Colour(Telnet.Cyan)}.");
        _name = newName;
        Changed = true;
        return true;
    }

    private bool BuildingCommandCanAccuse(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which legal class do you want to toggle accusability for?");
            return false;
        }

        ILegalClass legal = long.TryParse(command.PopSpeech(), out long value)
            ? LegalAuthority.LegalClasses.FirstOrDefault(x => x.Id == value)
            : LegalAuthority.LegalClasses.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
              LegalAuthority.LegalClasses.FirstOrDefault(x =>
                  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
        if (legal == null)
        {
            actor.OutputHandler.Send("There is no such legal class.");
            return false;
        }

        if (_accusableClasses.Contains(legal))
        {
            _accusableClasses.Remove(legal);
            if (!_accusableClasses.Any())
            {
                CanAccuse = false;
            }

            Changed = true;
            actor.OutputHandler.Send(
                $"The {Name.Colour(Telnet.Cyan)} enforcement authority can no longer accuse the {legal.Name.Colour(Telnet.Cyan)} legal class of crimes.");
            return true;
        }

        _accusableClasses.Add(legal);
        CanAccuse = true;
        Changed = true;
        actor.OutputHandler.Send(
            $"The {Name.Colour(Telnet.Cyan)} enforcement authority can now accuse the {legal.Name.Colour(Telnet.Cyan)} legal class of crimes.");
        return true;
    }

    private bool BuildingCommandCanForgive(ICharacter actor)
    {
        CanForgive = !CanForgive;
        Changed = true;
        actor.OutputHandler.Send(
            $"The {Name.Colour(Telnet.Cyan)} can {(CanForgive ? "now" : "no longer")} forgive crimes that they could accuse people of.");
        return true;
    }

    private bool BuildingCommandCanConvict(ICharacter actor)
    {
        CanConvict = !CanConvict;
        Changed = true;
        actor.OutputHandler.Send(
            $"The {Name.Colour(Telnet.Cyan)} can {(CanConvict ? "now" : "no longer")} convict or acquit crimes.");
        return true;
    }

    private bool BuildingCommandAlso(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which other enforcement authority do you want to toggle inclusion of?");
            return false;
        }

        IEnforcementAuthority authority = long.TryParse(command.PopSpeech(), out long value)
            ? LegalAuthority.EnforcementAuthorities.FirstOrDefault(x => x.Id == value)
            : LegalAuthority.EnforcementAuthorities.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
              LegalAuthority.EnforcementAuthorities.FirstOrDefault(x =>
                  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
        if (authority == null)
        {
            actor.OutputHandler.Send("There is no such enforcement authority.");
            return false;
        }

        if (authority == this)
        {
            actor.OutputHandler.Send("You cannot set an enforcement authority as including itself.");
            return false;
        }

        if (IncludedAuthorities.Contains(authority))
        {
            _includedAuthorities.Remove(authority);
            Changed = true;
            actor.OutputHandler.Send(
                $"The {Name.Colour(Telnet.Cyan)} enforcement authority no longer includes the {authority.Name.Colour(Telnet.Cyan)} enforcement authority as well.");
            return true;
        }

        if (authority.AllIncludedAuthorities.Contains(this))
        {
            actor.OutputHandler.Send(
                "One of the already included authorities contains a reference to this authority. Adding this new one would create a loop, which is forbidden.");
            return false;
        }

        _includedAuthorities.Add(authority);
        Changed = true;
        actor.OutputHandler.Send(
            $"The {Name.Colour(Telnet.Cyan)} enforcement authority now includes the {authority.Name.Colour(Telnet.Cyan)} enforcement authority as well.");
        return true;
    }

    public bool BuildingCommandFilter(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                "Which prog do you want to use to filter membership of this enforcement authority?");
            return false;
        }

        IFutureProg prog = long.TryParse(command.SafeRemainingArgument, out long value)
            ? Gameworld.FutureProgs.Get(value)
            : Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
        if (prog == null)
        {
            actor.OutputHandler.Send("There is no such prog.");
            return false;
        }

        if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
        {
            actor.OutputHandler.Send(
                $"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}");
            return false;
        }

        if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
        {
            actor.OutputHandler.Send(
                $"You must specify a prog that accepts a single character paramater, and {prog.MXPClickableFunctionName()} does not.");
            return false;
        }

        FilteringProg = prog;
        Changed = true;
        actor.OutputHandler.Send(
            $"The {prog.MXPClickableFunctionNameWithId()} prog will now be used to determine membership in the {Name.ColourName()} enforcement authority.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Enforcement Authority #{Id.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)}");
        sb.AppendLine($"Filter Prog: {FilteringProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Can Convict: {CanConvict.ToColouredString()}");
        sb.AppendLine($"Can Forgive: {CanForgive.ToColouredString()}");
        IEnumerable<ILegalClass> extraAccusable = AllIncludedAuthorities.SelectMany(x => x.AccusableClasses).Distinct()
                                                   .Except(_accusableClasses);
        IEnumerable<string> text = _accusableClasses.Select(x => x.Name.Colour(Telnet.Cyan))
                                   .Concat(extraAccusable.Select(x => x.Name.Colour(Telnet.BoldCyan)));
        sb.AppendLine($"Accusable: {text.ListToString()}");

        IEnumerable<ILegalClass> extraArrestable = AllIncludedAuthorities.SelectMany(x => x.ArrestableClasses).Distinct()
                                                    .Except(_arrestableClasses);
        text = _arrestableClasses.Select(x => x.Name.Colour(Telnet.Cyan))
                                 .Concat(extraArrestable.Select(x => x.Name.Colour(Telnet.BoldCyan)));
        sb.AppendLine($"Arrestable: {text.ListToString()}");

        sb.AppendLine($"Also Includes: {IncludedAuthorities.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");

        return sb.ToString();
    }

    public void Delete()
    {
        Gameworld.SaveManager.Abort(this);
        if (_id != 0)
        {
            Gameworld.SaveManager.Flush();
            using (new FMDB())
            {
                Models.EnforcementAuthority dbitem = FMDB.Context.EnforcementAuthorities.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.EnforcementAuthorities.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    public void RemoveAllReferencesTo(ILegalClass legalClass)
    {
        if (_arrestableClasses.Contains(legalClass))
        {
            _arrestableClasses.Remove(legalClass);
            Changed = true;
        }

        if (_accusableClasses.Contains(legalClass))
        {
            _accusableClasses.Remove(legalClass);
            Changed = true;
            if (!_accusableClasses.Any())
            {
                CanAccuse = false;
            }
        }
    }
}