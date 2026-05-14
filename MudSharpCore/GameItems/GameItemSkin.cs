using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Prototypes;
using MudSharp.OpenAI;
using MudSharp.PerceptionEngine;
using MudSharp.Work.Crafts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems;
#nullable enable
public class GameItemSkin : EditableItem, IGameItemSkin
{
    public GameItemSkin(Models.GameItemSkin item, IFuturemud gameworld) : base(item.EditableItem)
    {
        Gameworld = gameworld;
        _id = item.Id;
        RevisionNumber = item.RevisionNumber;
        _name = item.Name;
        _itemProtoId = item.ItemProtoId;
        CanUseSkinProg = Gameworld.FutureProgs.Get(item.CanUseSkinProgId ?? 0) ??
                         throw new ApplicationException($"Could not load CanUseSkinProg for skin {_id}");
        ShortDescription = item.ShortDescription;
        ItemName = item.ItemName;
        FullDescription = item.FullDescription;
        LongDescription = item.LongDescription;
        Quality = (ItemQuality?)item.Quality;
        IsPublic = item.IsPublic;
    }

    public GameItemSkin(IAccount originator, IFuturemud gameworld, IGameItemProto proto, string name) : base(originator)
    {
        Gameworld = gameworld;
        _id = Gameworld.ItemSkins.NextID();
        _itemProtoId = proto.Id;
        _name = name;
        CanUseSkinProg = Gameworld.FutureProgs.Get(Gameworld.GetStaticLong("AlwaysTrueProg")) ??
                         throw new ApplicationException($"Could not load CanUseSkinProg for skin {_id}");
        using (new FMDB())
        {
            Models.GameItemSkin dbitem = new()
            {
                Id = _id,
                RevisionNumber = RevisionNumber,
                Name = name,
                CanUseSkinProgId = CanUseSkinProg.Id,
                IsPublic = false,
                ItemProtoId = _itemProtoId
            };
            FMDB.Context.GameItemSkins.Add(dbitem);
            dbitem.EditableItem = new Models.EditableItem
            {
                BuilderAccountId = BuilderAccountID,
                BuilderDate = BuilderDate,
                RevisionStatus = (int)Status
            };
            FMDB.Context.EditableItems.Add(dbitem.EditableItem);
            FMDB.Context.SaveChanges();
        }
    }

    protected GameItemSkin(GameItemSkin rhs, IAccount originator) : base(originator)
    {
        Gameworld = rhs.Gameworld;
        _name = rhs.Name;
        _id = rhs.Id;
        RevisionNumber = Gameworld.ItemSkins.GetAll(_id).Max(x => x.RevisionNumber) + 1;
        _itemProtoId = rhs._itemProtoId;
        ItemName = rhs.ItemName;
        ShortDescription = rhs.ShortDescription;
        LongDescription = rhs.LongDescription;
        FullDescription = rhs.FullDescription;
        Quality = rhs.Quality;
        CanUseSkinProg = rhs.CanUseSkinProg;
        using (new FMDB())
        {
            Models.GameItemSkin dbitem = new()
            {
                Id = Id,
                RevisionNumber = RevisionNumber,
                Name = Name,
                CanUseSkinProgId = CanUseSkinProg.Id,
                IsPublic = IsPublic,
                ItemProtoId = _itemProtoId,
                ItemName = ItemName,
                ShortDescription = ShortDescription,
                LongDescription = LongDescription,
                FullDescription = FullDescription,
                Quality = (int?)Quality
            };
            FMDB.Context.GameItemSkins.Add(dbitem);
            dbitem.EditableItem = new Models.EditableItem
            {
                BuilderAccountId = BuilderAccountID,
                BuilderDate = BuilderDate,
                RevisionStatus = (int)Status
            };
            FMDB.Context.EditableItems.Add(dbitem.EditableItem);
            FMDB.Context.SaveChanges();
        }
    }

    protected GameItemSkin(GameItemSkin rhs, IAccount originator, string name) : base(originator)
    {
        Gameworld = rhs.Gameworld;
        _name = name;
        _itemProtoId = rhs._itemProtoId;
        ItemName = rhs.ItemName;
        ShortDescription = rhs.ShortDescription;
        LongDescription = rhs.LongDescription;
        FullDescription = rhs.FullDescription;
        Quality = rhs.Quality;
        CanUseSkinProg = rhs.CanUseSkinProg;
        _id = Gameworld.ItemSkins.NextID();
        RevisionNumber = 0;
        using (new FMDB())
        {
            Models.GameItemSkin dbitem = new()
            {
                Id = Id,
                RevisionNumber = RevisionNumber,
                Name = Name,
                CanUseSkinProgId = CanUseSkinProg?.Id,
                IsPublic = IsPublic,
                ItemProtoId = _itemProtoId,
                ItemName = ItemName,
                ShortDescription = ShortDescription,
                LongDescription = LongDescription,
                FullDescription = FullDescription,
                Quality = (int?)Quality
            };
            FMDB.Context.GameItemSkins.Add(dbitem);
            dbitem.EditableItem = new Models.EditableItem
            {
                BuilderAccountId = BuilderAccountID,
                BuilderDate = BuilderDate,
                RevisionStatus = (int)Status
            };
            FMDB.Context.EditableItems.Add(dbitem.EditableItem);
            FMDB.Context.SaveChanges();
        }
    }

    #region Overrides of FrameworkItem

    public override string FrameworkItemType => "GameItemSkin";

    #endregion

    #region Overrides of SavableKeywordedItem

    public override void Save()
    {
        Models.GameItemSkin dbitem = FMDB.Context.GameItemSkins.Find(Id, RevisionNumber)!;
        dbitem.Name = Name;
        dbitem.ItemName = ItemName;
        dbitem.ShortDescription = ShortDescription;
        dbitem.LongDescription = LongDescription;
        dbitem.FullDescription = FullDescription;
        dbitem.Quality = (int?)Quality;
        dbitem.CanUseSkinProgId = CanUseSkinProg.Id;
        dbitem.IsPublic = IsPublic;
        base.Save(dbitem.EditableItem);
        Changed = false;
    }

    #endregion

    #region Overrides of EditableItem

    public override string EditHeader()
    {
        return $"Item Skin {Id:F0}r{RevisionNumber:F0} ({Name})";
    }

    protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
    {
        return Gameworld.ItemSkins.GetAll(Id);
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return new GameItemSkin(this, initiator.Account);
    }

    public const string BuildingHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this skin
	#3prog <prog>#0 - sets the prog that controls use of this skin
	#3itemname <name>#0 - sets an override for the item's name
	#3itemname#0 - clears an override for the item's name
	#3sdesc <sdesc>#0 - sets an override for the item's short description
	#3sdesc#0 - clears an override for the item's short description
	#3ldesc <ldesc>#0 - sets an override for the item's long description
	#3ldesc#0 - clears an override for the item's long description
	#3desc#0 - drops you into an editor to enter an override description
	#3desc clear#0 - clears an override for the item's full description
	#3quality <quality>#0 - sets an overriding quality for the base item
	#3quality#0 - clears an override for the item's quality";

    public const string AdminBuildingHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this skin
	#3public#0 - toggles this skin being public (admin only)
	#3prog <prog>#0 - sets the prog that controls use of this skin
	#3itemname <name>#0 - sets an override for the item's name
	#3itemname#0 - clears an override for the item's name
	#3sdesc <sdesc>#0 - sets an override for the item's short description
	#3sdesc#0 - clears an override for the item's short description
	#3ldesc <ldesc>#0 - sets an override for the item's long description
	#3ldesc#0 - clears an override for the item's long description
	#3desc#0 - drops you into an editor to enter an override description
	#3desc clear#0 - clears an override for the item's full description
	#3suggestdesc [<optional extra context>]#0 - asks your configured AI model to suggest a description (admin only)
	#3quality <quality>#0 - sets an overriding quality for the base item
	#3quality#0 - clears an override for the item's quality";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "itemname":
            case "iname":
                return BuildingCommandItemName(actor, command);
            case "itemsdesc":
            case "sdesc":
                return BuildingCommandSDesc(actor, command);
            case "itemldesc":
            case "ldesc":
                return BuildingCommandLDesc(actor, command);
            case "itemdesc":
            case "desc":
                return BuildingCommandFDesc(actor, command);
            case "suggestdesc":
            case "suggestdescription":
                return BuildingCommandSuggestDesc(actor, command);
            case "quality":
                return BuildingCommandQuality(actor, command);
            case "prog":
                return BuildingCommandProg(actor, command);
            case "public":
                return BuildingCommandPublic(actor, command);
            default:
                actor.OutputHandler.Send((actor.IsAdministrator() ? AdminBuildingHelp : BuildingHelp).SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this skin?");
            return false;
        }

        _name = command.SafeRemainingArgument.TitleCase();
        Changed = true;
        actor.OutputHandler.Send($"You rename this skin to {Name.ColourName()}.");
        return true;
    }

    private bool BuildingCommandItemName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            ItemName = null;
            Changed = true;
            actor.OutputHandler.Send("This skin will no longer override the item name.");
            return true;
        }

        ItemName = command.SafeRemainingArgument.ToLowerInvariant();
        Changed = true;
        actor.OutputHandler.Send($"This skin will now override the item name to {ItemName.ColourName()}.");
        return true;
    }

    private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            ShortDescription = null;
            Changed = true;
            actor.OutputHandler.Send("This skin will no longer override the item short description.");
            return true;
        }

        ShortDescription = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send(
            $"This skin will now override the item short description to {ShortDescription.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandLDesc(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            LongDescription = null;
            Changed = true;
            actor.OutputHandler.Send("This skin will no longer override the item long description.");
            return true;
        }

        LongDescription = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send(
            $"This skin will now override the item long description to {LongDescription.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandFDesc(ICharacter actor, StringStack command)
    {
        if (command.Peek().EqualToAny("clear", "reset", "none", "remove", "delete"))
        {
            FullDescription = null;
            Changed = true;
            actor.OutputHandler.Send("This skin will no longer override the item's full description.");
            return true;
        }

        StringBuilder sb = new();
        sb.AppendLine(
            $"Base item description:\n\n{ItemProto.FullDescription.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t")}");
        sb.AppendLine();
        if (!string.IsNullOrEmpty(FullDescription))
        {
            sb.AppendLine("\nReplacing:\n");
            sb.AppendLine(FullDescription.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
            sb.AppendLine();
        }

        sb.AppendLine("Enter the description in the editor below.");
        sb.AppendLine();
        actor.OutputHandler.Send(sb.ToString());
        actor.EditorMode(PostAction, CancelAction, 1.0);
        return true;
    }

    private void CancelAction(IOutputHandler handler, object[] args)
    {
        handler.Send("You decide not to enter a full description override for this skin.");
    }

    private void PostAction(string text, IOutputHandler handler, object[] args)
    {
        FullDescription = text.Fullstop().ProperSentences();
        Changed = true;
        handler.Send($"You update the full description override for this skin to:\n\n{FullDescription.Wrap(80, "\t")}");
    }

    private bool BuildingCommandSuggestDesc(ICharacter actor, StringStack command)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only administrators can request AI description suggestions for item skins.");
            return false;
        }

        var effectiveShortDescription = ShortDescription ?? ItemProto.ShortDescription;
        var effectiveLongDescription = LongDescription ?? (ItemProto.OverridesLongDescription ? ItemProto.LongDescription : null);
        var effectiveQuality = Quality ?? ItemProto.BaseItemQuality;
        var differences = new List<string>();

        if (!string.IsNullOrWhiteSpace(ItemName))
        {
            differences.Add($"Item name override: \"{ItemName}\".");
        }

        if (!string.IsNullOrWhiteSpace(ShortDescription))
        {
            differences.Add($"Short description override: \"{ShortDescription}\".");
        }

        if (!string.IsNullOrWhiteSpace(LongDescription))
        {
            differences.Add($"Long description override: \"{LongDescription}\".");
        }

        if (!string.IsNullOrWhiteSpace(FullDescription))
        {
            differences.Add("Existing full description override is supplied below for possible refinement.");
        }

        if (Quality is not null)
        {
            differences.Add($"Quality override: {Quality.Value.DescribeEnum(true)}.");
        }

        StringBuilder sb = new();
        sb.AppendLine(Futuremud.Games.First().GetStaticString("GPT_ItemSuggestionPrompt"));
        sb.AppendLine();
        sb.AppendLine($"You are suggesting full-description overrides for an item skin named \"{Name}\". An item skin is a presentation variant for an existing item prototype; it changes how the same underlying item appears without changing its mechanics. Keep the base item's purpose and physical constraints intact, but make the description fit the skin's variant details.");
        sb.AppendLine();
        sb.AppendLine($"Base item prototype: {ItemProto.EditHeader()}.");
        sb.AppendLine($"Base short description: \"{ItemProto.ShortDescription}\".");
        sb.AppendLine($"Variant short description to match: \"{effectiveShortDescription}\".");
        sb.AppendLine($"Variant item name: \"{ItemName ?? ItemProto.Name}\".");
        if (!string.IsNullOrWhiteSpace(effectiveLongDescription))
        {
            sb.AppendLine($"Variant long description: \"{effectiveLongDescription}\".");
        }

        sb.AppendLine($"The item is made mostly out of {ItemProto.Material?.Name ?? "an unknown material"}. Its effective quality for this skin is {effectiveQuality.DescribeEnum(true)}, and it weighs {Gameworld.UnitManager.Describe(ItemProto.Weight, UnitType.Mass, "metric")} (but don't refer to the specific number in the description, though you can describe the weight in other non-numeric terms). It's {ItemProto.Size.DescribeEnum(true)} relative to a person.");
        sb.AppendLine();

        if (differences.Any())
        {
            sb.AppendLine("The skin differs from the base prototype as follows:");
            foreach (var difference in differences)
            {
                sb.AppendLine($"\t{difference}");
            }
        }
        else
        {
            sb.AppendLine("This skin does not currently override any presentation fields other than its builder-facing skin name. Use any extra builder context below as the requested variant direction.");
        }

        sb.AppendLine();
        sb.AppendLine("Base full description for context:");
        sb.AppendLine(ItemProto.FullDescription);
        if (!string.IsNullOrWhiteSpace(FullDescription))
        {
            sb.AppendLine();
            sb.AppendLine("Existing skin full description override to replace or improve:");
            sb.AppendLine(FullDescription);
        }

        if (ItemProto.Tags.Any())
        {
            sb.AppendLine();
            sb.AppendLine("It has been tagged with the following prompts:");
            foreach (var tag in ItemProto.Tags)
            {
                sb.AppendLine($"\t{tag.FullName}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("The full description you suggest may use these item-description markup options, but only where they genuinely help:");
        sb.AppendLine("\t@material - replaced with the item's material");
        sb.AppendLine("\t@matdesc - replaced with the item's material description");
        sb.AppendLine("\twriting{language,script,style=...,colour=...,skill|minskill}{text if you can understand}{text if you cant}");
        sb.AppendLine("\tcheck{trait,minvalue}{text if the trait is >= value}{text if not}");

        var variableComponent = ItemProto.Components.OfType<VariableGameItemComponentProto>().FirstOrDefault();
        if (variableComponent is not null)
        {
            sb.AppendLine();
            sb.AppendLine("When the engine uses this item it will substitute values for a few variables. In the description that you write, you should use only the pattern of the variable rather than any of its actual values. For example, if there is a variable called $colour then you should use that variable to refer to the colour of the item rather than writing in an actual colour. If there are multiple similar variables you can decide what part of each item each of the variables refer to. Each variation has a plain, basic and fancy variety - they refer to the same underlying variable but present the text in a different format. The following variables (and some examples of what the engine may render them as, so you can match your grammar to their usage) are available:");
            foreach (var variable in variableComponent.CharacteristicDefinitions)
            {
                var profile = variableComponent.ProfileFor(variable);
                var random = new List<ICharacteristicValue>
                {
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic(),
                    profile.GetRandomCharacteristic()
                };
                sb.AppendLine($"\t${variable.Pattern.TransformRegexIntoPattern()} - Examples: {random.Select(x => x.GetValue).ListToString()}");
            }

            sb.AppendLine("You should use each variable at least once in the description.");
        }

        sb.AppendLine("Your response should only use characters from the ISO-8859-1 page (i.e. Latin1).");

        if (!command.IsFinished)
        {
            sb.AppendLine();
            sb.AppendLine("Additional builder context:");
            sb.AppendLine(command.SafeRemainingArgument);
        }

        void GPTCallback(string text)
        {
            var descriptions = text.Split('#', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            StringBuilder sb = new();
            sb.AppendLine($"Your AI model has made the following suggestions for descriptions for item skin {Name.ColourName()} ({effectiveShortDescription.ColourObject()}):");
            var i = 1;
            foreach (var desc in descriptions)
            {
                sb.AppendLine();
                sb.AppendLine($"Suggestion {i++.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
                sb.AppendLine();
                sb.AppendLine(desc.Wrap(actor.InnerLineFormatLength, "\t"));
            }

            sb.AppendLine();
            sb.AppendLine($"You can apply one of these by using the syntax {"accept desc <n>".Colour(Telnet.Cyan)}, such as {"accept desc 1".Colour(Telnet.Cyan)}.");
            actor.OutputHandler.Send(sb.ToString());
            actor.AddEffect(new Accept(actor, new GenericProposal
            {
                DescriptionString = "Applying an AI description suggestion to an item skin",
                AcceptAction = text =>
                {
                    if (!int.TryParse(text, out var value) || value < 1 || value > descriptions.Length)
                    {
                        actor.OutputHandler.Send("You did not specify a valid description. If you still want to use the descriptions, you'll have to copy them in manually.");
                        return;
                    }

                    FullDescription = descriptions[value - 1];
                    Changed = true;
                    actor.OutputHandler.Send($"You set the full description override for this item skin based on the {value.ToOrdinal()} suggestion.");
                },
                RejectAction = text => { actor.OutputHandler.Send("You decide not to use any of the suggestions."); },
                ExpireAction = () => { actor.OutputHandler.Send("You decide not to use any of the suggestions."); },
                Keywords = new List<string> { "description", "suggestion", "skin" }
            }), TimeSpan.FromSeconds(120));
        }

        switch (actor.Gameworld.GetStaticConfiguration("DescSuggestionAIModel").ToLowerInvariant())
        {
            case "openai":
                var descModel = Futuremud.Games.First().GetStaticConfiguration("GPT_DescSuggestion_Model");
                if (!OpenAIHandler.MakeGPTRequest(sb.ToString(), actor.Gameworld.GetStaticString("GPT_ItemSuggestionFinalWord"), GPTCallback, descModel))
                {
                    actor.OutputHandler.Send("Your GPT Model is not set up correctly, so you cannot get any suggestions.");
                    return false;
                }

                break;
            case "anthropic":
                var anthropicModel = Futuremud.Games.First().GetStaticConfiguration("AnthropicDefaultModel");
                if (!OpenAIHandler.MakeAnthropicRequest(sb.ToString(), actor.Gameworld.GetStaticString("GPT_ItemSuggestionFinalWord"), GPTCallback, anthropicModel))
                {
                    actor.OutputHandler.Send("Your Anthropic Model is not set up correctly, so you cannot get any suggestions.");
                    return false;
                }

                break;
            default:
                actor.OutputHandler.Send($"The value of the #3DescSuggestionAIModel#0 static configuration must be either #3openai#0 or #3anthropic#0.");
                return false;
        }

        actor.OutputHandler.Send("You send your request off to the AI model.");
        return true;
    }

    private bool BuildingCommandQuality(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            Quality = null;
            Changed = true;
            actor.OutputHandler.Send("This skin will no longer override the item quality.");
            return true;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<ItemQuality>(out ItemQuality quality))
        {
            actor.OutputHandler.Send(
                $"That is not a valid quality. See {"show qualities".MXPSend("show qualities")} to see a list.");
            return false;
        }

        Quality = quality;
        Changed = true;
        actor.OutputHandler.Send(
            $"This skin will now override the item quality to {Quality.DescribeEnum().ColourName()}.");
        return true;
    }

    private bool BuildingCommandPublic(ICharacter actor, StringStack command)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only administrators can set a skin as public or private.");
            return false;
        }

        IsPublic = !IsPublic;
        Changed = true;
        actor.OutputHandler.Send($"This item skin is {(IsPublic ? "now" : "no longer")} public.");
        return true;
    }

    private bool BuildingCommandProg(ICharacter actor, StringStack command)
    {
        IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
            ProgVariableTypes.Boolean, new[]
            {
                new List<ProgVariableTypes> { ProgVariableTypes.Character },
                new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Text }
            }).LookupProg();
        if (prog is null)
        {
            return false;
        }

        CanUseSkinProg = prog;
        Changed = true;
        actor.OutputHandler.Send(
            $"This skin will now use the prog {CanUseSkinProg.MXPClickableFunctionName()} to determine who can use it.");
        return true;
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine(
            $"Item Skin {Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} ({Name.ColourName()})");
        sb.AppendLine($"For Item: {ItemProto.EditHeader().ColourName()}");
        sb.AppendLine($"Public: {IsPublic.ToColouredString()}");
        sb.AppendLine($"Can Use Prog: {CanUseSkinProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Override Name: {ItemName?.ColourValue() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Override SDesc: {ShortDescription?.ColourValue() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Override LDesc: {LongDescription?.ColourValue() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Override Quality: {Quality?.Describe().ColourValue() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine(
            $"Override Desc: {FullDescription?.ColourCommand().LeadingConcatIfNotEmpty("\n\n").Wrap(actor.InnerLineFormatLength, "\t") ?? "None".Colour(Telnet.Red)}");
        return sb.ToString();
    }

    #endregion

    #region Implementation of IGameItemSkin

    public IGameItemSkin Clone(ICharacter author, string newName)
    {
        return new GameItemSkin(this, author.Account, newName);
    }

    private long _itemProtoId;
    public IGameItemProto ItemProto => Gameworld.ItemProtos.Get(_itemProtoId);
    public string? ItemName { get; private set; }
    public string? ShortDescription { get; private set; }
    public string? FullDescription { get; private set; }
    public string? LongDescription { get; private set; }
    public ItemQuality? Quality { get; private set; }
    public bool IsPublic { get; private set; }
    public IFutureProg CanUseSkinProg { get; private set; } = null!;

    public (bool Truth, string Error) CanUseSkin(ICharacter crafter, IGameItemProto? prototype)
    {
        if (prototype is not null && prototype.Id != _itemProtoId)
        {
            return (false,
                $"This skin can only be used with the {ItemProto.EditHeader().ColourIncludingReset(Telnet.Cyan)} item prototype.");
        }

        if (crafter.IsAdministrator())
        {
            return (true, string.Empty);
        }

        if (CanUseSkinProg?.Execute<bool?>(crafter) == false)
        {
            return (false, $"You are not permitted to use that skin.");
        }

        return (true, string.Empty);
    }

    public (bool Truth, string Error) CanUseSkin(ICharacter crafter, IGameItemProto? prototype, ICraft craft)
    {
        if (prototype is not null && prototype.Id != _itemProtoId)
        {
            return (false,
                $"This skin can only be used with the {ItemProto.EditHeader().ColourIncludingReset(Telnet.Cyan)} item prototype.");
        }

        if (crafter.IsAdministrator())
        {
            return (true, string.Empty);
        }

        if (CanUseSkinProg?.Execute<bool?>(crafter, craft.Name) == false)
        {
            if (CanUseSkinProg.Execute<bool?>(crafter) == false)
            {
                return (false, $"You are not permitted to use that skin.");
            }

            return (false, $"That skin cannot be used with that craft.");
        }

        return (true, string.Empty);
    }

    #endregion
}
