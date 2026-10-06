
using MudSharp.Construction;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionConsume : InventoryPlanAction
{
    public InventoryPlanActionConsume(XElement root, IFuturemud gameworld)
        : base(root, gameworld, DesiredItemState.Consumed)
    {
        Quantity = int.Parse(root.Attribute("quantity").Value);
		CarriedOnly = bool.Parse(root.Attribute("carriedonly")?.Value ?? "false");
		if (root.Attribute("grade") is { } grade)
		{
			RequiredGrade = int.Parse(grade.Value);
			if (RequiredGrade is < 1 or > 7) throw new FormatException("A material grade must be between one and seven.");
		}
		if (root.Element("GradeRank") is { } rank) LoadGradeRanks(rank);
    }

    public InventoryPlanActionConsume(IFuturemud gameworld, int quantity, long primaryTag, long secondaryTag,
        Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector)
        : base(gameworld, DesiredItemState.Consumed, primaryTag, secondaryTag, primaryselector, secondaryselector)
    {
        Quantity = quantity;
    }

    #region Overrides of InventoryPlanAction

    public override XElement SaveToXml()
    {
        return new XElement("Action",
            new XAttribute("state", "consumed"),
            new XAttribute("tag", DesiredTagId),
            new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
            new XAttribute("quantity", Quantity),
            new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
            new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
            new XAttribute("originalreference", OriginalReference?.ToString() ?? ""),
			CarriedOnly ? new XAttribute("carriedonly", true) : null,
			RequiredGrade is { } grade ? new XAttribute("grade", grade) : null,
			_gradeRankDefinition is null ? null : new XElement(_gradeRankDefinition)
        );
    }


    public override string Describe(ICharacter voyeur)
    {
        return
            $"Consume {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"} x{Quantity.ToString("N0", voyeur)}" +
			(CarriedOnly ? " (directly carried only)" : "") +
			(RequiredGrade is { } grade ? $" (grade {grade} only)" : "") +
			(_gradeRankDefinition is null ? "" : $" (minimum rank: selected grade {_gradeRankOffset:+0;-0;0}, floor zero; {_rankTags.Count} rank tags)");
    }

    /// <inheritdoc />
    public override bool RequiresFreeHandsToExecute(ICharacter who, IGameItem item)
    {
        return false;
    }

    #endregion

    public int Quantity { get; set; }
	public bool CarriedOnly { get; set; }
	public int? RequiredGrade { get; private set; }
	private int? _boundGrade;
	public void ConfigureRequiredGrade(int? grade)
	{
		if (grade is < 1 or > 7) throw new ArgumentOutOfRangeException(nameof(grade));
		RequiredGrade = grade; _boundGrade = null;
	}
	private XElement _gradeRankDefinition;
	private int _gradeRankOffset;
	private readonly Dictionary<int, long> _rankTags = new();
	private ITag _boundRankTag;
	public bool HasPersistedSelection => CarriedOnly || _gradeRankDefinition is not null || RequiredGrade is not null;
	public bool HasGradeSelection => HasGradeRanks || RequiredGrade is not null;
	public bool HasGradeRanks => _gradeRankDefinition is not null;

	public void ConfigureGradeRanks(int offset, IReadOnlyList<ITag> tags)
	{
		var definition = new XElement("GradeRank", new XAttribute("offset", offset),
			tags.Select((tag, rank) => new XElement("Rank", new XAttribute("minimum", rank), new XAttribute("tag", tag.Id))));
		var candidateXml = SaveToXml();
		candidateXml.Element("GradeRank")?.Remove();
		candidateXml.Add(definition);
		var candidate = new InventoryPlanActionConsume(candidateXml, Gameworld);
		candidate.BindSelectedGrade(7);
		ClearGradeRanks();
		LoadGradeRanks(definition);
	}

	public void ClearGradeRanks()
	{
		_gradeRankDefinition = null;
		_rankTags.Clear();
		_gradeRankOffset = 0;
		_boundRankTag = null;
	}

	private void LoadGradeRanks(XElement root)
	{
		var offset = int.Parse(root.Attribute("offset")?.Value ?? throw new FormatException("Missing rank offset."));
		var ranks = root.Elements("Rank").Select(x => (Rank: int.Parse(x.Attribute("minimum")!.Value), Tag: long.Parse(x.Attribute("tag")!.Value))).ToArray();
		if (offset is < -6 or > 0 || ranks.Length is < 1 or > 7 ||
			!ranks.Select(x => x.Rank).SequenceEqual(Enumerable.Range(0, ranks.Length)) || ranks.Any(x => x.Tag <= 0) || ranks.Select(x => x.Tag).Distinct().Count() != ranks.Length ||
			Math.Max(7 + offset, 0) >= ranks.Length)
			throw new FormatException("Grade ranks require consecutive ranks from zero, positive tags and coverage of all seven grades.");
		_gradeRankDefinition = new XElement(root); _gradeRankOffset = offset;
		foreach (var rank in ranks) _rankTags.Add(rank.Rank, rank.Tag);
	}

	internal void BindSelectedGrade(int grade)
	{
		if (grade is < 1 or > 7) throw new InvalidOperationException("Select a supported grade before binding component rank.");
		_boundGrade = grade;
		if (HasGradeSelection && DesiredTagId > 0 && DesiredTag is null)
			throw new InvalidOperationException("The configured grade-dependent material tag is missing; repair the requirement before casting.");
		if (_gradeRankDefinition is null) return;
		var tags = _rankTags.OrderBy(x => x.Key).Select(x => Gameworld.Tags.Get(x.Value)).ToArray();
		if (DesiredTag is null || tags.Any(x => x is null) || !tags[0].IsA(DesiredTag) ||
			tags.Skip(1).Where((tag, index) => !tag.IsA(tags[index])).Any())
			throw new InvalidOperationException("Component ranks need existing tags in an ascending parent hierarchy under the Creation tag.");
		_boundRankTag = tags[Math.Max(grade + _gradeRankOffset, 0)];
	}

	internal bool MeetsPersistedSelection(ICharacter executor, IGameItem item) =>
		(!HasGradeSelection || DesiredTagId == 0 || DesiredTag is not null) &&
		(RequiredGrade is null || RequiredGrade == _boundGrade) &&
		(!CarriedOnly || ReferenceEquals(item.InInventoryOf, executor.Body) && item.ContainedIn is null) &&
		(_gradeRankDefinition is null || _boundRankTag is not null && item.IsA(_boundRankTag));

	internal void RevalidateConsumption(ICharacter executor, IGameItem item)
	{
		if (HasPersistedSelection && (item.Deleted || !MeetsPersistedSelection(executor, item) || !item.IsA(DesiredTag) ||
			!(MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(item) ?? true, false)) || (item.GetItemType<IStackable>()?.Quantity ?? 1) < Quantity))
			throw new InvalidOperationException("The selected component no longer meets its carried scope, rank or quantity requirements.");
	}

    public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
    {
        return null;
    }

    public override IGameItem ScoutTarget(ICharacter executor)
    {
        IGameItem item = null;

        // Already held items next
        item =
            executor.Body.HeldItems.FirstOrDefault(
                x =>
                    !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) &&
                    (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        if (item != null)
        {
            return item;
        }

        // Wielded items next
        item =
            executor.Body.WieldedItems.FirstOrDefault(
                x =>
                    !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) &&
                    (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        if (item != null)
        {
            return item;
        }

        // Worn items next
        item =
            executor.Body.WornItems.FirstOrDefault(
                x =>
                    !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) && executor.Body.CanRemoveItem(x) &&
                    (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        if (item != null)
        {
            return item;
        }

        // Attached to worn items next
        item =
            executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
                    .Select(
                        x =>
                            x.ConnectedItems.FirstOrDefault(
                                y =>
                                    !SpellOwnedItemValuePolicy.ContainsTemporaryValue(y.Parent) && y.Parent.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(y.Parent) ?? true, false)) && MeetsPersistedSelection(executor, y.Parent) &&
                                    (y.Parent.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity)?.Parent)
                    .FirstOrDefault(x => x != null);
        if (item != null)
        {
            return item;
        }

        // Sheathed next
        item =
            executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
                    .SelectNotNull(x => x.Content?.Parent)
                    .FirstOrDefault(
                        x =>
                            !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) &&
                            (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        if (item != null)
        {
            return item;
        }

        // In containers in inventory
        item =
            executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>())
                    .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
                    .SelectMany(x => x.Contents)
                    .FirstOrDefault(
                        x =>
                            !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) &&
                            (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        if (item != null)
        {
            return item;
        }

        // In location
        item =
            (executor.Location?.GameItemsInImmediateVicinity(executor) ?? []).FirstOrDefault(
                x =>
                    !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) &&
                    x.IsItemType<IHoldable>() &&
                    x.GetItemType<IHoldable>().IsHoldable &&
                    (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        if (item != null)
        {
            return item;
        }

        // Attached to room items next
        item =
            (executor.Location?.GameItemsInImmediateVicinity(executor) ?? []).SelectNotNull(x => x.GetItemType<IBelt>())
                    .Select(
                        x =>
                            x.ConnectedItems.FirstOrDefault(
                                y =>
                                    !SpellOwnedItemValuePolicy.ContainsTemporaryValue(y.Parent) && y.Parent.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(y.Parent) ?? true, false)) && MeetsPersistedSelection(executor, y.Parent) &&
                                    (y.Parent.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity)?.Parent)
                    .FirstOrDefault(x => x != null);
        if (item != null)
        {
            return item;
        }

        // Sheathed in room item next
        item =
            (executor.Location?.GameItemsInImmediateVicinity(executor) ?? []).SelectNotNull(x => x.GetItemType<ISheath>())
                    .SelectNotNull(x => x.Content?.Parent)
                    .FirstOrDefault(
                        x =>
                            !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) &&
                            (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        if (item != null)
        {
            return item;
        }

        // In containers in location
        item =
            (executor.Location?.GameItemsInImmediateVicinity(executor) ?? []).SelectNotNull(x => x.GetItemType<IContainer>())
                    .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
                    .SelectMany(x => x.Contents)
                    .FirstOrDefault(
                        x =>
                            !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x) && x.IsA(DesiredTag) && (MudSharp.NPC.AI.CommandExecutionScope.EvaluateCallback(() => PrimaryItemSelector?.Invoke(x) ?? true, false)) && MeetsPersistedSelection(executor, x) &&
                            (x.GetItemType<IStackable>()?.Quantity ?? 1) >= Quantity);
        return item;
    }
}
