using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Database;

#nullable enable
namespace MudSharp.CharacterCreation;

public sealed class ChargenSkillSelectionGroup : FrameworkItem, IChargenSkillSelectionGroup
{
	private readonly IFuturemud _gameworld;
	private readonly List<(long Id, int Order)> _members;
	public ChargenSkillSelectionGroup(Models.ChargenSkillSelectionGroup model, IFuturemud gameworld)
	{
		_gameworld = gameworld;
		_id = model.Id;
		_name = model.Name;
		StableKey = model.StableKey;
		Description = model.Description;
		DisplayOrder = model.DisplayOrder;
		Enabled = model.Enabled;
		Retired = model.Retired;
		MinimumPicks = model.MinimumPicks;
		MaximumPicks = model.MaximumPicks;
		Revision = model.Revision;
		ExistingSkillPolicy = (ExistingSkillPolicy)model.ExistingSkillPolicy;
		EligibilityProgId = model.EligibilityProgId;
		MemberEligibilityProgId = model.MemberEligibilityProgId;
		_members = model.Members.Select(x => (x.TraitDefinitionId, x.DisplayOrder)).ToList();
	}
	public override string FrameworkItemType => "ChargenSkillSelectionGroup";
	public IFuturemud Gameworld => _gameworld;
	public string Show(ICharacter actor) => Show();
	public string StableKey { get; }
	public string Description { get; private set; }
	public int DisplayOrder { get; private set; }
	public bool Enabled { get; private set; }
	public bool Retired { get; private set; }
	public int MinimumPicks { get; private set; }
	public int MaximumPicks { get; private set; }
	public int Revision { get; private set; }
	public ExistingSkillPolicy ExistingSkillPolicy { get; private set; }
	public long? EligibilityProgId { get; private set; }
	public long? MemberEligibilityProgId { get; private set; }
	public IFutureProg? EligibilityProg => _gameworld.FutureProgs.Get(EligibilityProgId ?? 0);
	public IFutureProg? MemberEligibilityProg => _gameworld.FutureProgs.Get(MemberEligibilityProgId ?? 0);
	public IReadOnlyList<ITraitDefinition> Members => _members.OrderBy(x => x.Order).ThenBy(x => x.Id)
		.Select(x => _gameworld.Traits.Get(x.Id)).OfType<ITraitDefinition>().ToList();

	public static bool ValidProg(IFutureProg? prog, bool member = false) => prog is not null &&
		prog.ReturnType == ProgVariableTypes.Boolean && !prog.AcceptsAnyParameters &&
		prog.Parameters.SequenceEqual(member ? [ProgVariableTypes.Chargen, ProgVariableTypes.Trait] : [ProgVariableTypes.Chargen]) &&
		prog.Compile();

	public static bool ValidMember(ITraitDefinition? trait) => trait is ISkillDefinition &&
		trait.TraitType is TraitType.Skill or TraitType.DerivedSkill or TraitType.TheoreticalSkill &&
		trait.OwnerScope == TraitOwnerScope.Character;

	public IEnumerable<string> Validate()
	{
		if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(StableKey)) yield return "Name and stable key are required.";
		if (MinimumPicks < 0 || MaximumPicks < MinimumPicks || MinimumPicks > _members.Count) yield return "Invalid or unfillable pick bounds.";
		if (!Enum.IsDefined(ExistingSkillPolicy)) yield return "Invalid existing-skill policy.";
		if (!ValidProg(EligibilityProg)) yield return "Eligibility must be a compiled Boolean(chargen) prog.";
		if (MemberEligibilityProgId.HasValue && !ValidProg(MemberEligibilityProg, true)) yield return "Member eligibility must be a compiled Boolean(chargen, trait) prog.";
		if (_members.Select(x => x.Id).Distinct().Count() != _members.Count ||
			_members.Any(x => !ValidMember(_gameworld.Traits.Get(x.Id)))) yield return "Members must be distinct, resolved character-owned skills.";
	}

	public string Show() => $"#{Id} {Name} [{StableKey}] revision {Revision}\n{Description}\n" +
		$"Enabled: {Enabled}; Retired: {Retired}; Picks: {MinimumPicks}-{MaximumPicks}; Order: {DisplayOrder}; Existing: {ExistingSkillPolicy}\n" +
		$"Eligibility: {EligibilityProg?.FunctionName ?? "Missing"}; Member filter: {MemberEligibilityProg?.FunctionName ?? "None"}\n" +
		string.Join("\n", Members.Select(x => $"  {x.Id}: {x.Name}")) + "\n" + string.Join("\n", Validate());

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant();
		var value = command.SafeRemainingArgument;
		switch (field)
		{
			case "name":
				if (string.IsNullOrWhiteSpace(value) || value.Length > 200) break;
				_name = value;
				return Commit(actor);
			case "description":
				actor.EditorMode((text, handler, _) => { Description = text; Persist(); handler.Send("Description saved."); },
					(handler, _) => handler.Send("Description unchanged."), 1.0, Description);
				return true;
			case "picks":
				if (!int.TryParse(command.PopSpeech(), out var min)) break;
				var max = min;
				if (!command.IsFinished && !int.TryParse(command.PopSpeech(), out max)) break;
				if (!command.IsFinished || min < 0 || max < min) break;
				MinimumPicks = min;
				MaximumPicks = max;
				return Commit(actor);
			case "order":
				if (!int.TryParse(value, out var order)) break;
				DisplayOrder = order;
				return Commit(actor);
			case "existing":
				if (!Enum.TryParse<ExistingSkillPolicy>(value, true, out var policy) || !Enum.IsDefined(policy)) break;
				ExistingSkillPolicy = policy;
				return Commit(actor);
			case "eligibility":
			case "membereligibility":
				var member = field == "membereligibility";
				if (member && value.EqualTo("none")) { MemberEligibilityProgId = null; return Commit(actor); }
				var progs = _gameworld.FutureProgs.Where(x => x.Id.ToString() == value || x.FunctionName.EqualTo(value)).ToList();
				if (progs.Count != 1 || !ValidProg(progs[0], member)) break;
				if (member) MemberEligibilityProgId = progs[0].Id;
				else EligibilityProgId = progs[0].Id;
				return Commit(actor);
			case "enabled":
				var enabled = string.IsNullOrEmpty(value) ? !Enabled : value.EqualTo("yes");
				if (!string.IsNullOrEmpty(value) && !value.EqualTo("yes") && !value.EqualTo("no")) break;
				if (enabled && (Retired || Validate().Any())) { actor.OutputHandler.Send("Cannot enable: " + string.Join(" ", Validate())); return false; }
				Enabled = enabled;
				return Commit(actor);
			case "member":
				var action = command.PopSpeech().ToLowerInvariant();
				var name = command.SafeRemainingArgument;
				var traits = _gameworld.Traits.Where(x => x.Id.ToString() == name || x.Name.EqualTo(name)).ToList();
				if (traits.Count != 1 || !ValidMember(traits[0])) break;
				var id = traits[0].Id;
				if (action == "add" && _members.All(x => x.Id != id)) _members.Add((id, _members.Count));
				else if (action == "remove" && _members.Any(x => x.Id == id)) _members.RemoveAll(x => x.Id == id);
				else break;
				return Commit(actor);
		}
		actor.OutputHandler.Send("Invalid or ambiguous setting. Use name, description, picks <min> [max], eligibility, membereligibility, order, existing, enabled, or member add/remove <skill>. Progs must compile with the documented signature.");
		return false;
	}

	private bool Commit(ICharacter actor) { Persist(); actor.OutputHandler.Send(Show()); return true; }
	public void Retire() { Retired = true; Enabled = false; Persist(); }
	private void Persist()
	{
		using (new FMDB())
		{
			var model = FMDB.Context.ChargenSkillSelectionGroups.Find(Id)!;
			model.Name = Name; model.Description = Description; model.Enabled = Enabled; model.Retired = Retired;
			model.DisplayOrder = DisplayOrder; model.MinimumPicks = MinimumPicks; model.MaximumPicks = MaximumPicks;
			model.EligibilityProgId = EligibilityProgId; model.MemberEligibilityProgId = MemberEligibilityProgId;
			model.ExistingSkillPolicy = (int)ExistingSkillPolicy; model.Revision = ++Revision;
			foreach (var old in model.Members.Where(x => _members.All(y => y.Id != x.TraitDefinitionId)).ToList())
				FMDB.Context.ChargenSkillSelectionGroupMembers.Remove(old);
			foreach (var item in _members)
			{
				var join = model.Members.FirstOrDefault(x => x.TraitDefinitionId == item.Id);
				if (join is null) model.Members.Add(new Models.ChargenSkillSelectionGroupMember { TraitDefinitionId = item.Id, DisplayOrder = item.Order });
				else join.DisplayOrder = item.Order;
			}
			FMDB.Context.SaveChanges();
		}
	}
}
