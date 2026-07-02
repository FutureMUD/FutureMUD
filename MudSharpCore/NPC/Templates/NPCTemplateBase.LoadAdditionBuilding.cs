using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Economy;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using System;
using System.Globalization;
using System.Linq;

#nullable enable
#nullable disable warnings

namespace MudSharp.NPC.Templates;

public abstract partial class NPCTemplateBase
{
    private bool BuildingCommandClan(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                return BuildingCommandClanAdd(actor, command);
            case "remove":
            case "delete":
            case "rem":
                return BuildingCommandClanRemove(actor, command);
            case "paygrade":
                return BuildingCommandClanPaygrade(actor, command);
            case "appointment":
            case "appt":
                return BuildingCommandClanAppointment(actor, command);
            case "list":
            case "":
                actor.OutputHandler.Send(ShowTemplateLoadAdditions(actor));
                return true;
            default:
                actor.OutputHandler.Send("You can use #3clan add#0, #3clan remove#0, #3clan paygrade#0 or #3clan appointment#0.".SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandClanAdd(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to add to this NPC template?");
            return false;
        }

        var clan = Gameworld.Clans.GetByIdOrName(command.PopSpeech());
        if (clan is null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which rank should this NPC have in that clan?");
            return false;
        }

        var rank = clan.Ranks.GetByIdOrName(command.PopSpeech());
        if (rank is null)
        {
            actor.OutputHandler.Send($"The {clan.FullName.ColourName()} clan has no such rank.");
            return false;
        }

        IPaygrade? paygrade = null;
        if (!command.IsFinished)
        {
            var token = command.PopSpeech();
            if (!token.EqualTo("paygrade"))
            {
                actor.OutputHandler.Send("The only optional clan membership argument is #3paygrade <paygrade>#0.".SubstituteANSIColour());
                return false;
            }

            if (command.IsFinished)
            {
                actor.OutputHandler.Send("Which paygrade do you want to use?");
                return false;
            }

            paygrade = clan.Paygrades.GetByIdOrName(command.PopSpeech());
            if (paygrade is null)
            {
                actor.OutputHandler.Send($"The {clan.FullName.ColourName()} clan has no such paygrade.");
                return false;
            }

            if (!rank.Paygrades.Contains(paygrade))
            {
                actor.OutputHandler.Send($"The {paygrade.Name.ColourName()} paygrade is not valid for the {rank.Name.ColourName()} rank.");
                return false;
            }
        }

        _templateClanMemberships.RemoveAll(x => x.ClanId == clan.Id);
        _templateClanMemberships.Add(new TemplateClanMembership
        {
            ClanId = clan.Id,
            RankId = rank.Id,
            PaygradeId = paygrade?.Id
        });
        Changed = true;
        actor.OutputHandler.Send(
            $"This NPC template will now load with membership in {clan.FullName.ColourName()} as {rank.Name.TitleCase().ColourValue()}{(paygrade is null ? string.Empty : $" on paygrade {paygrade.Name.TitleCase().ColourValue()}")}.");
        return true;
    }

    private bool BuildingCommandClanRemove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan membership do you want to remove from this NPC template?");
            return false;
        }

        var clan = Gameworld.Clans.GetByIdOrName(command.SafeRemainingArgument);
        if (clan is null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return false;
        }

        if (_templateClanMemberships.RemoveAll(x => x.ClanId == clan.Id) == 0)
        {
            actor.OutputHandler.Send("This NPC template does not have a load-time membership for that clan.");
            return false;
        }

        Changed = true;
        actor.OutputHandler.Send($"This NPC template no longer loads with membership in {clan.FullName.ColourName()}.");
        return true;
    }

    private bool BuildingCommandClanPaygrade(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan membership do you want to change the paygrade for?");
            return false;
        }

        var clan = Gameworld.Clans.GetByIdOrName(command.PopSpeech());
        if (clan is null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return false;
        }

        var membership = _templateClanMemberships.FirstOrDefault(x => x.ClanId == clan.Id);
        if (membership is null)
        {
            actor.OutputHandler.Send("This NPC template does not have a load-time membership for that clan.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which paygrade do you want to use, or #3none#0 to clear it?".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
        {
            membership.PaygradeId = null;
            Changed = true;
            actor.OutputHandler.Send($"This NPC template no longer specifies a paygrade for {clan.FullName.ColourName()}.");
            return true;
        }

        var paygrade = clan.Paygrades.GetByIdOrName(command.SafeRemainingArgument);
        if (paygrade is null)
        {
            actor.OutputHandler.Send($"The {clan.FullName.ColourName()} clan has no such paygrade.");
            return false;
        }

        var rank = clan.Ranks.FirstOrDefault(x => x.Id == membership.RankId);
        if (rank is null)
        {
            actor.OutputHandler.Send(
                $"This NPC template's stored rank for {clan.FullName.ColourName()} no longer exists. Use #3clan add {clan.Name} <rank>#0 to reset the membership.".SubstituteANSIColour());
            return false;
        }

        if (!rank.Paygrades.Contains(paygrade))
        {
            actor.OutputHandler.Send($"The {paygrade.Name.ColourName()} paygrade is not valid for the {rank.Name.ColourName()} rank.");
            return false;
        }

        membership.PaygradeId = paygrade.Id;
        Changed = true;
        actor.OutputHandler.Send($"This NPC template will use the {paygrade.Name.ColourName()} paygrade for {clan.FullName.ColourName()}.");
        return true;
    }

    private bool BuildingCommandClanAppointment(ICharacter actor, StringStack command)
    {
        var action = command.PopSpeech();
        var add = action.EqualTo("add");
        if (!add && !action.EqualToAny("remove", "delete", "rem"))
        {
            actor.OutputHandler.Send("Do you want to #3add#0 or #3remove#0 a clan appointment?".SubstituteANSIColour());
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan membership is the appointment for?");
            return false;
        }

        var clan = Gameworld.Clans.GetByIdOrName(command.PopSpeech());
        if (clan is null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return false;
        }

        var membership = _templateClanMemberships.FirstOrDefault(x => x.ClanId == clan.Id);
        if (membership is null)
        {
            actor.OutputHandler.Send("This NPC template does not have a load-time membership for that clan.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which appointment do you want to change?");
            return false;
        }

        var appointment = clan.Appointments.GetByIdOrName(command.SafeRemainingArgument);
        if (appointment is null)
        {
            actor.OutputHandler.Send($"The {clan.FullName.ColourName()} clan has no such appointment.");
            return false;
        }

        if (add)
        {
            if (membership.AppointmentIds.Contains(appointment.Id))
            {
                actor.OutputHandler.Send("This NPC template already includes that appointment.");
                return false;
            }

            membership.AppointmentIds.Add(appointment.Id);
            Changed = true;
            actor.OutputHandler.Send($"This NPC template will try to load with the {appointment.Name.ColourName()} appointment.");
            return true;
        }

        if (!membership.AppointmentIds.Remove(appointment.Id))
        {
            actor.OutputHandler.Send("This NPC template does not include that appointment.");
            return false;
        }

        Changed = true;
        actor.OutputHandler.Send($"This NPC template will no longer try to load with the {appointment.Name.ColourName()} appointment.");
        return true;
    }

    private bool BuildingCommandOutfit(ICharacter actor, StringStack command)
    {
        var action = command.PopSpeech();
        switch (action.ToLowerInvariant())
        {
            case "add":
                return BuildingCommandOutfitAdd(actor, command);
            case "remove":
            case "delete":
            case "rem":
                return BuildingCommandOutfitRemove(actor, command);
            case "list":
            case "":
                actor.OutputHandler.Send(ShowTemplateLoadAdditions(actor));
                return true;
            default:
                actor.OutputHandler.Send("You can use #3outfit add <template> [name <name>]#0 or #3outfit remove <template|#>#0.".SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandOutfitAdd(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which outfit template should be materialised when this NPC loads?");
            return false;
        }

        var outfit = Gameworld.OutfitTemplates.GetByIdOrName(command.PopSpeech());
        if (outfit is null)
        {
            actor.OutputHandler.Send("There is no such outfit template.");
            return false;
        }

        if (_templateOutfits.Any(x => x.OutfitTemplateId == outfit.Id))
        {
            actor.OutputHandler.Send("This NPC template already includes that outfit template.");
            return false;
        }

        string? name = null;
        if (!command.IsFinished)
        {
            var token = command.PopSpeech();
            if (!token.EqualTo("name") || command.IsFinished)
            {
                actor.OutputHandler.Send("The optional outfit argument is #3name <created outfit name>#0.".SubstituteANSIColour());
                return false;
            }

            name = command.SafeRemainingArgument;
        }

        _templateOutfits.Add(new TemplateOutfitLoad { OutfitTemplateId = outfit.Id, OutfitName = NullIfBlank(name) });
        Changed = true;
        actor.OutputHandler.Send($"This NPC template will materialise {$"{outfit.Name} (#{outfit.Id:N0})".ColourName()} when loaded.");
        return true;
    }

    private bool BuildingCommandOutfitRemove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which outfit template do you want to remove?");
            return false;
        }

        TemplateOutfitLoad? outfit = null;
        if (int.TryParse(command.SafeRemainingArgument, out var index) && index >= 1 && index <= _templateOutfits.Count)
        {
            outfit = _templateOutfits[index - 1];
        }
        else
        {
            var template = Gameworld.OutfitTemplates.GetByIdOrName(command.SafeRemainingArgument);
            if (template is not null)
            {
                outfit = _templateOutfits.FirstOrDefault(x => x.OutfitTemplateId == template.Id);
            }
        }

        if (outfit is null)
        {
            actor.OutputHandler.Send("This NPC template does not include that outfit template.");
            return false;
        }

        _templateOutfits.Remove(outfit);
        Changed = true;
        actor.OutputHandler.Send("This NPC template will no longer materialise that outfit template when loaded.");
        return true;
    }

    private bool BuildingCommandHook(ICharacter actor, StringStack command)
    {
        var action = command.PopSpeech();
        var add = action.EqualTo("add");
        if (!add && !action.EqualToAny("remove", "delete", "rem"))
        {
            actor.OutputHandler.Send("Do you want to #3add#0 or #3remove#0 a hook?".SubstituteANSIColour());
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which hook do you want to change?");
            return false;
        }

        var hook = Gameworld.Hooks.GetByIdOrName(command.SafeRemainingArgument);
        if (hook is null)
        {
            actor.OutputHandler.Send("There is no such hook.");
            return false;
        }

        if (add)
        {
            if (!HookIsValidForCharacter(hook))
            {
                actor.OutputHandler.Send("That hook is not valid for character/NPC installation.");
                return false;
            }

            if (_templateHookIds.Contains(hook.Id))
            {
                actor.OutputHandler.Send("This NPC template already includes that hook.");
                return false;
            }

            _templateHookIds.Add(hook.Id);
            Changed = true;
            actor.OutputHandler.Send($"This NPC template will install the {hook.Name.ColourName()} hook when loaded.");
            return true;
        }

        if (!_templateHookIds.Remove(hook.Id))
        {
            actor.OutputHandler.Send("This NPC template does not include that hook.");
            return false;
        }

        Changed = true;
        actor.OutputHandler.Send($"This NPC template will no longer install the {hook.Name.ColourName()} hook when loaded.");
        return true;
    }

    private bool BuildingCommandBank(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                return BuildingCommandBankAdd(actor, command);
            case "remove":
            case "delete":
            case "rem":
                return BuildingCommandBankRemove(actor, command);
            case "name":
                return BuildingCommandBankName(actor, command);
            case "balance":
                return BuildingCommandBankBalance(actor, command);
            case "list":
            case "":
                actor.OutputHandler.Send(ShowTemplateLoadAdditions(actor));
                return true;
            default:
                actor.OutputHandler.Send("You can use #3bank add#0, #3bank remove#0, #3bank name#0 or #3bank balance#0.".SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandBankAdd(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which bank account type should be created when this NPC loads?");
            return false;
        }

        var type = Gameworld.BankAccountTypes.GetByIdOrName(command.PopSpeech());
        if (type is null)
        {
            actor.OutputHandler.Send("There is no such bank account type.");
            return false;
        }

        var account = new TemplateBankAccount { BankAccountTypeId = type.Id };
        while (!command.IsFinished)
        {
            var token = command.PopSpeech();
            if (token.EqualTo("balance"))
            {
                if (command.IsFinished || !decimal.TryParse(command.PopSpeech(), NumberStyles.Number,
                        actor, out var balance))
                {
                    actor.OutputHandler.Send("You must specify a valid opening balance.");
                    return false;
                }

                account.OpeningBalance = balance;
                continue;
            }

            if (token.EqualTo("name"))
            {
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("What account name do you want to use?");
                    return false;
                }

                account.Name = command.SafeRemainingArgument;
                break;
            }

            actor.OutputHandler.Send("Bank account options are #3balance <amount>#0 and #3name <account name>#0.".SubstituteANSIColour());
            return false;
        }

        _templateBankAccounts.Add(account);
        Changed = true;
        actor.OutputHandler.Send($"This NPC template will create a {type.Name.ColourName()} bank account when loaded.");
        return true;
    }

    private bool BuildingCommandBankRemove(ICharacter actor, StringStack command)
    {
        var account = GetTemplateBankAccount(actor, command);
        if (account is null)
        {
            return false;
        }

        _templateBankAccounts.Remove(account);
        Changed = true;
        actor.OutputHandler.Send("This NPC template will no longer create that bank account.");
        return true;
    }

    private bool BuildingCommandBankName(ICharacter actor, StringStack command)
    {
        var account = GetTemplateBankAccount(actor, command);
        if (account is null)
        {
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What account name do you want to use, or #3none#0 to clear it?".SubstituteANSIColour());
            return false;
        }

        account.Name = command.SafeRemainingArgument.EqualToAny("none", "clear", "remove")
            ? null
            : command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send("You update the created account name for this NPC template.");
        return true;
    }

    private bool BuildingCommandBankBalance(ICharacter actor, StringStack command)
    {
        var account = GetTemplateBankAccount(actor, command);
        if (account is null)
        {
            return false;
        }

        if (command.IsFinished || !decimal.TryParse(command.PopSpeech(), NumberStyles.Number, actor, out var balance))
        {
            actor.OutputHandler.Send("What opening balance should this account have?");
            return false;
        }

        account.OpeningBalance = balance;
        Changed = true;
        actor.OutputHandler.Send($"This NPC template will credit that account with {balance.ToString("N2", actor).ColourValue()} when loaded.");
        return true;
    }

    private TemplateBankAccount? GetTemplateBankAccount(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which bank account entry do you want to change?");
            return null;
        }

        var text = command.PopSpeech();
        if (int.TryParse(text, out var index) && index >= 1 && index <= _templateBankAccounts.Count)
        {
            return _templateBankAccounts[index - 1];
        }

        var type = Gameworld.BankAccountTypes.GetByIdOrName(text);
        if (type is null)
        {
            actor.OutputHandler.Send("There is no such bank account type or entry number.");
            return null;
        }

        var matches = _templateBankAccounts
                      .Where(x => x.BankAccountTypeId == type.Id)
                      .ToList();
        if (matches.Count > 1)
        {
            actor.OutputHandler.Send(
                $"This NPC template creates multiple {type.Name.ColourName()} bank accounts. Please use the entry number from #3bank list#0.".SubstituteANSIColour());
            return null;
        }

        var account = matches.FirstOrDefault();
        if (account is null)
        {
            actor.OutputHandler.Send("This NPC template does not create that bank account type.");
        }

        return account;
    }

    private bool BuildingCommandImplant(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                return BuildingCommandBodyItemAdd(actor, command, TemplateBodyItemType.Implant);
            case "remove":
            case "delete":
            case "rem":
                return BuildingCommandBodyItemRemove(actor, command, TemplateBodyItemType.Implant);
            case "power":
                return BuildingCommandImplantPower(actor, command);
            case "neural":
            case "control":
            case "link":
                return BuildingCommandImplantNeural(actor, command);
            case "list":
            case "":
                actor.OutputHandler.Send(ShowTemplateLoadAdditions(actor));
                return true;
            default:
                actor.OutputHandler.Send("You can use #3implant add#0, #3implant remove#0, #3implant power#0 or #3implant neural#0.".SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandProsthetic(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                return BuildingCommandBodyItemAdd(actor, command, TemplateBodyItemType.Prosthetic);
            case "remove":
            case "delete":
            case "rem":
                return BuildingCommandBodyItemRemove(actor, command, TemplateBodyItemType.Prosthetic);
            case "list":
            case "":
                actor.OutputHandler.Send(ShowTemplateLoadAdditions(actor));
                return true;
            default:
                actor.OutputHandler.Send("You can use #3prosthetic add <key> <proto> [bodypart <part>]#0 or #3prosthetic remove <key>#0.".SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandBodyItemAdd(ICharacter actor, StringStack command, TemplateBodyItemType type)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What template-local key should this {type.DescribeEnum().ToLowerInvariant()} use?");
            return false;
        }

        var key = command.PopSpeech();
        if (string.IsNullOrWhiteSpace(key))
        {
            actor.OutputHandler.Send("The key cannot be blank.");
            return false;
        }

        if (_templateBodyItems.Any(x => x.Key.EqualTo(key)))
        {
            actor.OutputHandler.Send("This NPC template already has a body loadout item with that key.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which item prototype should be created?");
            return false;
        }

        var proto = Gameworld.ItemProtos.GetByIdOrUniqueNameOrName(command.PopSpeech());
        if (proto is null)
        {
            actor.OutputHandler.Send("There is no such item prototype.");
            return false;
        }

        if (type == TemplateBodyItemType.Implant && proto.Components.All(x => x is not IImplantPrototype))
        {
            actor.OutputHandler.Send("That item prototype does not have an implant component.");
            return false;
        }

        if (type == TemplateBodyItemType.Prosthetic && proto.Components.All(x => x is not IProstheticPrototype))
        {
            actor.OutputHandler.Send("That item prototype does not have a prosthetic component.");
            return false;
        }

        long? bodypartId = null;
        if (!command.IsFinished)
        {
            var token = command.PopSpeech();
            if (!token.EqualTo("bodypart") || command.IsFinished)
            {
                actor.OutputHandler.Send("The optional body loadout argument is #3bodypart <part>#0.".SubstituteANSIColour());
                return false;
            }

            var bodypart = Gameworld.BodypartPrototypes.GetByIdOrName(command.SafeRemainingArgument);
            if (bodypart is null)
            {
                actor.OutputHandler.Send("There is no such bodypart.");
                return false;
            }

            bodypartId = bodypart.Id;
        }

        _templateBodyItems.Add(new TemplateBodyItem
        {
            Type = type,
            Key = key,
            ItemProtoId = proto.Id,
            BodypartId = bodypartId
        });
        Changed = true;
        actor.OutputHandler.Send($"This NPC template will create and install {proto.EditHeader().ColourName()} as {key.ColourCommand()} when loaded.");
        return true;
    }

    private bool BuildingCommandBodyItemRemove(ICharacter actor, StringStack command, TemplateBodyItemType type)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which body loadout key do you want to remove?");
            return false;
        }

        var key = command.SafeRemainingArgument;
        var item = _templateBodyItems.FirstOrDefault(x => x.Type == type && x.Key.EqualTo(key));
        if (item is null)
        {
            actor.OutputHandler.Send("This NPC template does not have such a body loadout item.");
            return false;
        }

        _templateBodyItems.Remove(item);
        foreach (var implant in _templateBodyItems.Where(x => x.Type == TemplateBodyItemType.Implant))
        {
            if (implant.PowerSourceKey.EqualTo(key))
            {
                implant.PowerSourceKey = null;
            }

            if (implant.NeuralLinkKey.EqualTo(key))
            {
                implant.NeuralLinkKey = null;
            }
        }

        Changed = true;
        actor.OutputHandler.Send($"This NPC template will no longer create the {key.ColourCommand()} body loadout item.");
        return true;
    }

    private bool BuildingCommandImplantPower(ICharacter actor, StringStack command)
    {
        var item = GetTemplateImplant(actor, command);
        if (item is null)
        {
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which implant key should power this implant, or #3none#0 to clear it?".SubstituteANSIColour());
            return false;
        }

        var powerKey = command.SafeRemainingArgument;
        if (powerKey.EqualToAny("none", "clear", "remove"))
        {
            item.PowerSourceKey = null;
            Changed = true;
            actor.OutputHandler.Send($"The {item.Key.ColourCommand()} implant will no longer configure a power source.");
            return true;
        }

        var power = _templateBodyItems.FirstOrDefault(x => x.Type == TemplateBodyItemType.Implant && x.Key.EqualTo(powerKey));
        if (power is null)
        {
            actor.OutputHandler.Send("There is no implant body loadout item with that key.");
            return false;
        }

        item.PowerSourceKey = power.Key;
        Changed = true;
        actor.OutputHandler.Send($"The {item.Key.ColourCommand()} implant will try to use {power.Key.ColourCommand()} as its power source.");
        return true;
    }

    private bool BuildingCommandImplantNeural(ICharacter actor, StringStack command)
    {
        var item = GetTemplateImplant(actor, command);
        if (item is null)
        {
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which neural implant key should control this implant, or #3none#0 to clear it?".SubstituteANSIColour());
            return false;
        }

        var neuralKey = command.SafeRemainingArgument;
        if (neuralKey.EqualToAny("none", "clear", "remove"))
        {
            item.NeuralLinkKey = null;
            Changed = true;
            actor.OutputHandler.Send($"The {item.Key.ColourCommand()} implant will no longer configure a neural link.");
            return true;
        }

        var neural = _templateBodyItems.FirstOrDefault(x => x.Type == TemplateBodyItemType.Implant && x.Key.EqualTo(neuralKey));
        if (neural is null)
        {
            actor.OutputHandler.Send("There is no implant body loadout item with that key.");
            return false;
        }

        item.NeuralLinkKey = neural.Key;
        Changed = true;
        actor.OutputHandler.Send($"The {item.Key.ColourCommand()} implant will try to link to {neural.Key.ColourCommand()}.");
        return true;
    }

    private TemplateBodyItem? GetTemplateImplant(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which implant body loadout key do you want to change?");
            return null;
        }

        var key = command.PopSpeech();
        var item = _templateBodyItems.FirstOrDefault(x => x.Type == TemplateBodyItemType.Implant && x.Key.EqualTo(key));
        if (item is null)
        {
            actor.OutputHandler.Send("This NPC template does not have an implant body loadout item with that key.");
        }

        return item;
    }
}
