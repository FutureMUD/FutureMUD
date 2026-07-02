using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Economy;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable
#nullable disable warnings

namespace MudSharp.NPC.Templates;

public abstract partial class NPCTemplateBase
{
    private readonly List<TemplateClanMembership> _templateClanMemberships = new();
    private readonly List<TemplateOutfitLoad> _templateOutfits = new();
    private readonly List<long> _templateHookIds = new();
    private readonly List<TemplateBankAccount> _templateBankAccounts = new();
    private readonly List<TemplateBodyItem> _templateBodyItems = new();

    private sealed class TemplateClanMembership
    {
        public long ClanId { get; init; }
        public long RankId { get; set; }
        public long? PaygradeId { get; set; }
        public List<long> AppointmentIds { get; } = new();

        public XElement SaveToXml()
        {
            return new XElement("Membership",
                new XAttribute("clan", ClanId),
                new XAttribute("rank", RankId),
                new XAttribute("paygrade", PaygradeId ?? 0L),
                from item in AppointmentIds
                select new XElement("Appointment", new XAttribute("id", item))
            );
        }
    }

    private sealed class TemplateOutfitLoad
    {
        public long OutfitTemplateId { get; init; }
        public string? OutfitName { get; set; }

        public XElement SaveToXml()
        {
            return new XElement("Outfit",
                new XAttribute("template", OutfitTemplateId),
                new XAttribute("name", OutfitName ?? string.Empty)
            );
        }
    }

    private sealed class TemplateBankAccount
    {
        public long BankAccountTypeId { get; init; }
        public string? Name { get; set; }
        public decimal OpeningBalance { get; set; }

        public XElement SaveToXml()
        {
            return new XElement("Account",
                new XAttribute("type", BankAccountTypeId),
                new XAttribute("name", Name ?? string.Empty),
                new XAttribute("balance", OpeningBalance.ToString(CultureInfo.InvariantCulture))
            );
        }
    }

    private enum TemplateBodyItemType
    {
        Implant,
        Prosthetic
    }

    private sealed class TemplateBodyItem
    {
        public TemplateBodyItemType Type { get; init; }
        public string Key { get; init; } = string.Empty;
        public long ItemProtoId { get; set; }
        public long? BodypartId { get; set; }
        public string? PowerSourceKey { get; set; }
        public string? NeuralLinkKey { get; set; }

        public XElement SaveToXml()
        {
            return new XElement(Type == TemplateBodyItemType.Implant ? "Implant" : "Prosthetic",
                new XAttribute("key", Key),
                new XAttribute("proto", ItemProtoId),
                new XAttribute("bodypart", BodypartId ?? 0L),
                new XAttribute("power", PowerSourceKey ?? string.Empty),
                new XAttribute("neural", NeuralLinkKey ?? string.Empty)
            );
        }
    }

    protected void LoadTemplateLoadAdditions(XElement definition)
    {
        var root = definition.Element("TemplateLoadAdditions");
        if (root is null)
        {
            return;
        }

        foreach (var item in root.Element("ClanMemberships")?.Elements("Membership") ?? Enumerable.Empty<XElement>())
        {
            var membership = new TemplateClanMembership
            {
                ClanId = long.Parse(item.Attribute("clan")?.Value ?? "0"),
                RankId = long.Parse(item.Attribute("rank")?.Value ?? "0"),
                PaygradeId = LongOrNull(item.Attribute("paygrade")?.Value)
            };
            foreach (var appointment in item.Elements("Appointment"))
            {
                var id = long.Parse(appointment.Attribute("id")?.Value ?? "0");
                if (id > 0)
                {
                    membership.AppointmentIds.Add(id);
                }
            }

            if (membership.ClanId > 0 && membership.RankId > 0)
            {
                _templateClanMemberships.Add(membership);
            }
        }

        foreach (var item in root.Element("Outfits")?.Elements("Outfit") ?? Enumerable.Empty<XElement>())
        {
            var id = long.Parse(item.Attribute("template")?.Value ?? "0");
            if (id <= 0)
            {
                continue;
            }

            _templateOutfits.Add(new TemplateOutfitLoad
            {
                OutfitTemplateId = id,
                OutfitName = string.IsNullOrWhiteSpace(item.Attribute("name")?.Value)
                    ? null
                    : item.Attribute("name")!.Value
            });
        }

        foreach (var item in root.Element("Hooks")?.Elements("Hook") ?? Enumerable.Empty<XElement>())
        {
            var id = long.Parse(item.Attribute("id")?.Value ?? "0");
            if (id > 0)
            {
                _templateHookIds.Add(id);
            }
        }

        foreach (var item in root.Element("BankAccounts")?.Elements("Account") ?? Enumerable.Empty<XElement>())
        {
            var id = long.Parse(item.Attribute("type")?.Value ?? "0");
            if (id <= 0)
            {
                continue;
            }

            _templateBankAccounts.Add(new TemplateBankAccount
            {
                BankAccountTypeId = id,
                Name = string.IsNullOrWhiteSpace(item.Attribute("name")?.Value)
                    ? null
                    : item.Attribute("name")!.Value,
                OpeningBalance = decimal.Parse(item.Attribute("balance")?.Value ?? "0.0",
                    CultureInfo.InvariantCulture)
            });
        }

        foreach (var item in root.Element("BodyLoadout")?.Elements() ?? Enumerable.Empty<XElement>())
        {
            if (!item.Name.LocalName.EqualToAny("Implant", "Prosthetic"))
            {
                continue;
            }

            var key = item.Attribute("key")?.Value;
            var protoId = long.Parse(item.Attribute("proto")?.Value ?? "0");
            if (string.IsNullOrWhiteSpace(key) || protoId <= 0)
            {
                continue;
            }

            _templateBodyItems.Add(new TemplateBodyItem
            {
                Type = item.Name.LocalName.EqualTo("Implant")
                    ? TemplateBodyItemType.Implant
                    : TemplateBodyItemType.Prosthetic,
                Key = key.Trim(),
                ItemProtoId = protoId,
                BodypartId = LongOrNull(item.Attribute("bodypart")?.Value),
                PowerSourceKey = NullIfBlank(item.Attribute("power")?.Value),
                NeuralLinkKey = NullIfBlank(item.Attribute("neural")?.Value)
            });
        }
    }

    protected XElement SaveTemplateLoadAdditions()
    {
        return new XElement("TemplateLoadAdditions",
            new XElement("ClanMemberships", _templateClanMemberships.Select(x => x.SaveToXml())),
            new XElement("Outfits", _templateOutfits.Select(x => x.SaveToXml())),
            new XElement("Hooks",
                _templateHookIds.Select(x => new XElement("Hook", new XAttribute("id", x)))),
            new XElement("BankAccounts", _templateBankAccounts.Select(x => x.SaveToXml())),
            new XElement("BodyLoadout", _templateBodyItems.Select(x => x.SaveToXml()))
        );
    }

    protected string ShowTemplateLoadAdditions(ICharacter actor)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("Load-Time Additions:");

        sb.AppendLine("\tClan Memberships:");
        if (_templateClanMemberships.Count == 0)
        {
            sb.AppendLine("\t\tNone");
        }
        else
        {
            foreach (var membership in _templateClanMemberships)
            {
                var clan = Gameworld.Clans.Get(membership.ClanId);
                var rank = clan?.Ranks.FirstOrDefault(x => x.Id == membership.RankId);
                var paygrade = clan?.Paygrades.FirstOrDefault(x => x.Id == membership.PaygradeId);
                var appointments = membership.AppointmentIds
                                             .Select(x => clan?.Appointments.FirstOrDefault(y => y.Id == x))
                                             .Where(x => x is not null)
                                             .Select(x => x!.Name.TitleCase().ColourName())
                                             .DefaultIfEmpty("None".ColourCommand())
                                             .ListToString();
                sb.AppendLine(
                    $"\t\t{clan?.FullName.ColourName() ?? $"Missing Clan #{membership.ClanId:N0}".ColourError()} - {rank?.Name.TitleCase().ColourValue() ?? $"Missing Rank #{membership.RankId:N0}".ColourError()} / {paygrade?.Name.TitleCase().ColourValue() ?? "No Paygrade".ColourCommand()} / Appointments: {appointments}");
            }
        }

        sb.AppendLine("\tOutfit Templates:");
        if (_templateOutfits.Count == 0)
        {
            sb.AppendLine("\t\tNone");
        }
        else
        {
            for (var i = 0; i < _templateOutfits.Count; i++)
            {
                var outfit = _templateOutfits[i];
                var template = Gameworld.OutfitTemplates.Get(outfit.OutfitTemplateId);
                sb.AppendLine(
                    $"\t\t{(i + 1).ToString("N0", actor).ColourValue()}: {(template is null ? $"Missing Outfit #{outfit.OutfitTemplateId:N0}".ColourError() : $"{template.Name} (#{template.Id:N0})".ColourName())}{(string.IsNullOrWhiteSpace(outfit.OutfitName) ? string.Empty : $" as {outfit.OutfitName.ColourCommand()}")}");
            }
        }

        sb.AppendLine("\tHooks:");
        if (_templateHookIds.Count == 0)
        {
            sb.AppendLine("\t\tNone");
        }
        else
        {
            foreach (var hookId in _templateHookIds)
            {
                var hook = Gameworld.Hooks.Get(hookId);
                sb.AppendLine($"\t\t{hook?.Name.ColourName() ?? $"Missing Hook #{hookId:N0}".ColourError()}");
            }
        }

        sb.AppendLine("\tBank Accounts:");
        if (_templateBankAccounts.Count == 0)
        {
            sb.AppendLine("\t\tNone");
        }
        else
        {
            for (var i = 0; i < _templateBankAccounts.Count; i++)
            {
                var account = _templateBankAccounts[i];
                var type = Gameworld.BankAccountTypes.Get(account.BankAccountTypeId);
                sb.AppendLine(
                    $"\t\t{(i + 1).ToString("N0", actor).ColourValue()}: {type?.Name.ColourName() ?? $"Missing Account Type #{account.BankAccountTypeId:N0}".ColourError()} / {(account.Name ?? "Default Name").ColourCommand()} / {account.OpeningBalance.ToString("N2", actor).ColourValue()}");
            }
        }

        sb.AppendLine("\tBody Loadout:");
        if (_templateBodyItems.Count == 0)
        {
            sb.AppendLine("\t\tNone");
        }
        else
        {
            foreach (var item in _templateBodyItems)
            {
                var proto = Gameworld.ItemProtos.Get(item.ItemProtoId);
                var bodypart = item.BodypartId.HasValue
                    ? Gameworld.BodypartPrototypes.Get(item.BodypartId.Value)
                    : null;
                sb.AppendLine(
                    $"\t\t{item.Type.DescribeEnum().ColourValue()} {item.Key.ColourCommand()}: {proto?.EditHeader().ColourName() ?? $"Missing Item Proto #{item.ItemProtoId:N0}".ColourError()}{(bodypart is null ? string.Empty : $" in {bodypart.FullDescription().ColourValue()}")}{(string.IsNullOrWhiteSpace(item.PowerSourceKey) ? string.Empty : $" / power {item.PowerSourceKey.ColourCommand()}")}{(string.IsNullOrWhiteSpace(item.NeuralLinkKey) ? string.Empty : $" / neural {item.NeuralLinkKey.ColourCommand()}")}");
            }
        }

        return sb.ToString();
    }

    public IEnumerable<string> ApplyTemplateLoadAdditions(ICharacter character, bool logWarnings = true)
    {
        var warnings = new List<string>();
        ApplyClanMemberships(character, warnings);
        ApplyHooks(character, warnings);

        var needsPostIdWork = _templateOutfits.Any() || _templateBankAccounts.Any() || _templateBodyItems.Any();
        if (needsPostIdWork && character is ILateInitialisingItem { IdHasBeenRegistered: false } lateItem)
        {
            Gameworld.SaveManager.DirectInitialise(lateItem);
        }

        ApplyBankAccounts(character, warnings);
        ApplyBodyLoadout(character, warnings);
        ApplyOutfits(character, warnings);

        if (logWarnings && warnings.Any())
        {
            foreach (var warning in warnings)
            {
                Gameworld.SystemMessage(
                    $"NPC template {EditHeader()} load warning: {warning}", true);
            }
        }

        return warnings;
    }

    private void ApplyClanMemberships(ICharacter character, List<string> warnings)
    {
        foreach (var item in _templateClanMemberships)
        {
            var clan = Gameworld.Clans.Get(item.ClanId);
            if (clan is null)
            {
                warnings.Add($"Clan #{item.ClanId:N0} no longer exists.");
                continue;
            }

            var rank = clan.Ranks.FirstOrDefault(x => x.Id == item.RankId);
            if (rank is null)
            {
                warnings.Add($"Rank #{item.RankId:N0} no longer exists in clan {clan.FullName}.");
                continue;
            }

            var paygrade = item.PaygradeId.HasValue
                ? clan.Paygrades.FirstOrDefault(x => x.Id == item.PaygradeId.Value)
                : null;
            if (item.PaygradeId.HasValue && (paygrade is null || !rank.Paygrades.Contains(paygrade)))
            {
                warnings.Add($"Paygrade #{item.PaygradeId.Value:N0} is no longer valid for {clan.FullName} rank {rank.Name}.");
                paygrade = null;
            }

            var membership = character.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
            if (membership is null)
            {
                membership = new ClanMembership(Gameworld)
                {
                    Clan = clan,
                    Rank = rank,
                    Paygrade = paygrade,
                    PersonalName = character.CurrentName,
                    JoinDate = clan.Calendar.CurrentDate
                };
                clan.Memberships.Add(membership);
                character.AddMembership(membership);
            }
            else
            {
                membership.Rank = rank;
                membership.Paygrade = paygrade;
                membership.Changed = CharacterIdHasBeenRegistered(character);
            }

            foreach (var appointmentId in item.AppointmentIds)
            {
                var appointment = clan.Appointments.FirstOrDefault(x => x.Id == appointmentId);
                if (appointment is null)
                {
                    warnings.Add($"Appointment #{appointmentId:N0} no longer exists in clan {clan.FullName}.");
                    continue;
                }

                if (appointment.IsAppointedByElection)
                {
                    warnings.Add($"Appointment {appointment.Name} in clan {clan.FullName} is now elected and was skipped.");
                    continue;
                }

                if (appointment.MinimumRankToHold is not null &&
                    appointment.MinimumRankToHold.RankNumber > membership.Rank.RankNumber)
                {
                    warnings.Add(
                        $"Appointment {appointment.Name} in clan {clan.FullName} now requires rank {appointment.MinimumRankToHold.Name} and was skipped.");
                    continue;
                }

                if (!clan.FreePosition(appointment))
                {
                    warnings.Add($"Appointment {appointment.Name} in clan {clan.FullName} has no free positions and was skipped.");
                    continue;
                }

                if (!membership.Appointments.Contains(appointment))
                {
                    membership.Appointments.Add(appointment);
                    membership.Changed = CharacterIdHasBeenRegistered(character);
                }
            }
        }
    }

    private void ApplyHooks(ICharacter character, List<string> warnings)
    {
        foreach (var hookId in _templateHookIds)
        {
            var hook = Gameworld.Hooks.Get(hookId);
            if (hook is null)
            {
                warnings.Add($"Hook #{hookId:N0} no longer exists.");
                continue;
            }

            if (!HookIsValidForCharacter(hook))
            {
                warnings.Add($"Hook {hook.Name} is not valid for character/NPC installation.");
                continue;
            }

            if (character.InstallHook(hook) && CharacterIdHasBeenRegistered(character))
            {
                character.HooksChanged = true;
            }
        }
    }

    private void ApplyBankAccounts(ICharacter character, List<string> warnings)
    {
        foreach (var item in _templateBankAccounts)
        {
            var type = Gameworld.BankAccountTypes.Get(item.BankAccountTypeId);
            if (type is null)
            {
                warnings.Add($"Bank account type #{item.BankAccountTypeId:N0} no longer exists.");
                continue;
            }

            var canOpen = type.CanOpenAccount(character);
            if (!canOpen.Truth)
            {
                warnings.Add($"Bank account type {type.Name} could not be opened: {canOpen.Reason}");
                continue;
            }

            var account = type.OpenAccount(character);
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                account.SetName(item.Name);
            }

            if (item.OpeningBalance != 0.0M)
            {
                account.DoAccountCredit(item.OpeningBalance, $"NPC template {EditHeader()} opening balance");
            }
        }
    }

    private void ApplyOutfits(ICharacter character, List<string> warnings)
    {
        foreach (var item in _templateOutfits)
        {
            var template = Gameworld.OutfitTemplates.Get(item.OutfitTemplateId);
            if (template is null)
            {
                warnings.Add($"Outfit template #{item.OutfitTemplateId:N0} no longer exists.");
                continue;
            }

            try
            {
                template.Materialise(character, item.OutfitName);
            }
            catch (Exception ex)
            {
                warnings.Add($"Outfit template {template.Name} could not be materialised: {ex.Message}");
            }
        }
    }

    private void ApplyBodyLoadout(ICharacter character, List<string> warnings)
    {
        var installedImplants = new Dictionary<string, IImplant>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var item in _templateBodyItems)
        {
            var proto = Gameworld.ItemProtos.Get(item.ItemProtoId);
            if (proto is null)
            {
                warnings.Add($"Item proto #{item.ItemProtoId:N0} for body loadout key {item.Key} no longer exists.");
                continue;
            }

            var created = proto.CreateNew(character, null, 1, string.Empty).FirstOrDefault();
            if (created is null)
            {
                warnings.Add($"Item proto {proto.EditHeader()} did not create an item for body loadout key {item.Key}.");
                continue;
            }

            var installed = false;
            switch (item.Type)
            {
                case TemplateBodyItemType.Implant:
                    installed = InstallTemplateImplant(character, item, created, installedImplants, warnings);
                    break;
                case TemplateBodyItemType.Prosthetic:
                    installed = InstallTemplateProsthetic(character, item, created, warnings);
                    break;
            }

            if (!installed)
            {
                created.Delete();
            }
        }

        foreach (var item in _templateBodyItems.Where(x => x.Type == TemplateBodyItemType.Implant))
        {
            if (!installedImplants.TryGetValue(item.Key, out var implant))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(item.PowerSourceKey))
            {
                if (!installedImplants.TryGetValue(item.PowerSourceKey, out var plantImplant) ||
                    plantImplant.Parent.GetItemType<IImplantPowerPlant>() is not IImplantPowerPlant plant)
                {
                    warnings.Add($"Implant {item.Key} could not be powered by missing or invalid implant key {item.PowerSourceKey}.");
                }
                else if (implant.Parent.GetItemType<IImplantPowerSupply>() is not IImplantPowerSupply supply)
                {
                    warnings.Add($"Implant {item.Key} is not a powered implant and cannot use power source {item.PowerSourceKey}.");
                }
                else
                {
                    supply.PowerPlant = plant;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.NeuralLinkKey))
            {
                if (implant is IImplantNeuralLink)
                {
                    warnings.Add($"Implant {item.Key} is a neural interface and cannot be linked to another neural interface.");
                }
                else if (!installedImplants.TryGetValue(item.NeuralLinkKey, out var neuralImplant) ||
                         neuralImplant is not IImplantNeuralLink neural)
                {
                    warnings.Add($"Implant {item.Key} could not be linked to missing or invalid neural key {item.NeuralLinkKey}.");
                }
                else
                {
                    foreach (var other in character.Body.Implants.OfType<IImplantNeuralLink>().Except(neural))
                    {
                        other.RemoveLink(implant);
                    }

                    neural.AddLink(implant);
                    if (implant is IImplantRespondToCommands commandImplant)
                    {
                        commandImplant.AliasForCommands = $"implant{commandImplant.Id:F0}";
                    }
                }
            }
        }
    }

    private bool InstallTemplateImplant(ICharacter character, TemplateBodyItem item, IGameItem created,
        Dictionary<string, IImplant> installedImplants, List<string> warnings)
    {
        var implant = created.GetItemType<IImplant>();
        if (implant is null)
        {
            warnings.Add($"{created.HowSeen(character, true)} for body loadout key {item.Key} is not an implant.");
            return false;
        }

        if (implant.TargetBody is null || !character.Body.Prototype.CountsAs(implant.TargetBody))
        {
            warnings.Add($"{created.HowSeen(character, true)} is not designed for {character.Body.Prototype.Name} bodies.");
            return false;
        }

        if (item.BodypartId.HasValue)
        {
            var bodypart = character.Body.Prototype.AllBodypartsBonesAndOrgans
                                    .FirstOrDefault(x => x.Id == item.BodypartId.Value);
            if (bodypart is null)
            {
                warnings.Add($"Bodypart #{item.BodypartId.Value:N0} is not present on {character.Body.Prototype.Name} bodies.");
                return false;
            }

            implant.TargetBodypart = bodypart;
        }

        if (implant.TargetBodypart is null)
        {
            warnings.Add($"{created.HowSeen(character, true)} does not have a target bodypart.");
            return false;
        }

        Gameworld.Add(created);
        if (created is ILateInitialisingItem lateItem)
        {
            Gameworld.SaveManager.DirectInitialise(lateItem);
        }

        character.Body.InstallImplant(implant);
        created.HandleEvent(EventType.ItemFinishedLoading, created);
        created.Login();
        installedImplants[item.Key] = implant;
        return true;
    }

    private bool InstallTemplateProsthetic(ICharacter character, TemplateBodyItem item, IGameItem created,
        List<string> warnings)
    {
        var prosthetic = created.GetItemType<IProsthetic>();
        if (prosthetic is null)
        {
            warnings.Add($"{created.HowSeen(character, true)} for body loadout key {item.Key} is not a prosthetic.");
            return false;
        }

        if (prosthetic.TargetBody is null || !character.Body.Prototype.CountsAs(prosthetic.TargetBody))
        {
            warnings.Add($"{created.HowSeen(character, true)} is not designed for {character.Body.Prototype.Name} bodies.");
            return false;
        }

        var bodypart = prosthetic.TargetBodypart;
        if (item.BodypartId.HasValue)
        {
            var configured = character.Body.Prototype.AllBodypartsBonesAndOrgans
                                      .FirstOrDefault(x => x.Id == item.BodypartId.Value);
            if (configured is null ||
                !(configured == bodypart || configured.CountsAs(bodypart) || bodypart.CountsAs(configured)))
            {
                warnings.Add($"Bodypart #{item.BodypartId.Value:N0} is not compatible with prosthetic {created.HowSeen(character)}.");
                return false;
            }
        }

        if (!character.Body.SeveredRoots.Any(x => x == bodypart || x.CountsAs(bodypart) || bodypart.CountsAs(x)))
        {
            warnings.Add($"{character.HowSeen(character, true)} does not have a sever suitable for prosthetic {created.HowSeen(character)}.");
            return false;
        }

        if (character.Body.Prosthetics.Any(x =>
                x.TargetBodypart == bodypart ||
                x.TargetBodypart.CountsAs(bodypart) ||
                bodypart.CountsAs(x.TargetBodypart)))
        {
            warnings.Add($"{character.HowSeen(character, true)} already has a prosthetic for {bodypart.FullDescription()}.");
            return false;
        }

        Gameworld.Add(created);
        if (created is ILateInitialisingItem lateItem)
        {
            Gameworld.SaveManager.DirectInitialise(lateItem);
        }

        character.Body.InstallProsthetic(prosthetic);
        created.HandleEvent(EventType.ItemFinishedLoading, created);
        created.Login();
        return true;
    }

    private static long? LongOrNull(string? text)
    {
        if (!long.TryParse(text, out var value) || value <= 0)
        {
            return null;
        }

        return value;
    }

    private static string? NullIfBlank(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    private static bool CharacterIdHasBeenRegistered(ICharacter character)
    {
        return character is not ILateInitialisingItem item || item.IdHasBeenRegistered;
    }

    private static bool HookIsValidForCharacter(IHook hook)
    {
        if (hook.Type is EventType.CommandInput or EventType.SelfCommandInput
            or EventType.FiveSecondTick or EventType.TenSecondTick or EventType.MinuteTick or EventType.HourTick
            or EventType.NPCOnGameLoadFinished)
        {
            return true;
        }

        return hook.Type.ToString().StartsWith("Character", StringComparison.InvariantCultureIgnoreCase);
    }
}
