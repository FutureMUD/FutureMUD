using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts.Inputs;
using MudSharp.Work.Crafts.Products;
using MudSharp.Work.Crafts.Tools;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.Work.Crafts;

public class Craft : Framework.Revision.EditableItem, ICraft
{
	public override string FrameworkItemType => "Craft";

	public Craft(Craft rhs, IAccount newOriginator, string newName) : base(newOriginator)
	{
		Gameworld = newOriginator.Gameworld;
		_id = Gameworld.Crafts.NextID();
		_name = newName;
		Blurb = rhs.Blurb;
		ActionDescription = rhs.ActionDescription;
		ActiveCraftItemSDesc = rhs.ActiveCraftItemSDesc;
		Category = rhs.Category;
		QualityFormula = new TraitExpression(rhs.QualityFormula.OriginalFormulaText, Gameworld);
		FailThreshold = rhs.FailThreshold;
		CheckDifficulty = rhs.CheckDifficulty;
		CheckQualityWeighting = rhs.CheckQualityWeighting;
		InputQualityWeighting = rhs.InputQualityWeighting;
		ToolQualityWeighting = rhs.ToolQualityWeighting;
		FreeSkillChecks = rhs.FreeSkillChecks;
		IsPracticalCheck = rhs.IsPracticalCheck;
		Interruptable = rhs.Interruptable;
		CheckTrait = rhs.CheckTrait;
		CanUseProg = rhs.CanUseProg;
		WhyCannotUseProg = rhs.WhyCannotUseProg;
		OnUseProgCancel = rhs.OnUseProgCancel;
		OnUseProgComplete = rhs.OnUseProgComplete;
		OnUseProgStart = rhs.OnUseProgStart;
		AppearInCraftsListProg = rhs.AppearInCraftsListProg;
		FailPhase = rhs.FailPhase;
		_phaseEchoes = rhs.PhaseEchoes.ToList();
		_failPhaseEchoes = rhs.FailPhaseEchoes.ToList();
		_phaseLengths = rhs.PhaseLengths.ToList();
		using (new FMDB())
		{
			var dbnew = new Models.Craft();
			FMDB.Context.Crafts.Add(dbnew);
			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = 0;
			dbnew.EditableItem.BuilderAccountId = newOriginator.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

			dbnew.Id = _id;
			dbnew.RevisionNumber = 0;
			dbnew.Name = Name;
			dbnew.Blurb = Blurb;
			dbnew.ActionDescription = ActionDescription;
			dbnew.ActiveCraftItemSdesc = ActiveCraftItemSDesc;
			dbnew.Category = Category;
			dbnew.Interruptable = Interruptable;
			dbnew.ToolQualityWeighting = ToolQualityWeighting;
			dbnew.InputQualityWeighting = InputQualityWeighting;
			dbnew.CheckQualityWeighting = CheckQualityWeighting;
			dbnew.FreeSkillChecks = FreeSkillChecks;
			dbnew.FailPhase = FailPhase;
			dbnew.FailThreshold = (int)FailThreshold;
			dbnew.CheckDifficulty = (int)CheckDifficulty;
			dbnew.CheckTraitId = CheckTrait?.Id;
			dbnew.QualityFormula = QualityFormula.OriginalFormulaText;
			dbnew.AppearInCraftsListProgId = AppearInCraftsListProg?.Id;
			dbnew.CanUseProgId = CanUseProg?.Id;
			dbnew.WhyCannotUseProgId = WhyCannotUseProg?.Id;
			dbnew.OnUseProgStartId = OnUseProgStart?.Id;
			dbnew.OnUseProgCompleteId = OnUseProgComplete?.Id;
			dbnew.OnUseProgCancelId = OnUseProgCancel?.Id;

			for (var i = 0; i < _phaseLengths.Count; i++)
			{
				var dbphase = new CraftPhase();
				dbnew.CraftPhases.Add(dbphase);
				dbphase.Echo = _phaseEchoes[i];
				dbphase.FailEcho = _failPhaseEchoes[i];
				dbphase.PhaseLengthInSeconds = _phaseLengths[i].TotalSeconds;
				dbphase.PhaseNumber = i + 1;
			}

			var inputMap = new Dictionary<long, Models.CraftInput>();
			foreach (var input in _orderedInputs)
			{
				inputMap[input.Id] = input.CreateNewRevision(dbnew);
			}

			var toolMap = new Dictionary<long, Models.CraftTool>();
			foreach (var tool in _orderedTools)
			{
				toolMap[tool.Id] = tool.CreateNewRevision(dbnew);
			}
			FMDB.Context.SaveChanges();

			foreach (var product in _orderedProducts)
			{
				product.CreateNewRevision(dbnew, false, inputMap.ToDictionary(x => x.Key, x => x.Value.Id), toolMap.ToDictionary(x => x.Key, x => x.Value.Id));
			}

			foreach (var product in _orderedFailProducts)
			{
				product.CreateNewRevision(dbnew, true, inputMap.ToDictionary(x => x.Key, x => x.Value.Id), toolMap.ToDictionary(x => x.Key, x => x.Value.Id));
			}

			FMDB.Context.SaveChanges();

			foreach (var input in dbnew.CraftInputs)
			{
				var newInput = CraftInputFactory.Factory.LoadInput(input, this, Gameworld);
				_inputs[newInput.Id] = newInput;
			}

			_orderedInputs.AddRange(_inputs.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));

			foreach (var tool in dbnew.CraftTools)
			{
				var newTool = CraftToolFactory.Factory.LoadTool(tool, this, Gameworld);
				_tools[newTool.Id] = newTool;
			}

			_orderedTools.AddRange(_tools.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));

			foreach (var product in dbnew.CraftProducts)
			{
				var newProduct = CraftProductFactory.Factory.LoadProduct(product, this, Gameworld);
				if (product.IsFailProduct)
				{
					_failProducts[newProduct.Id] = newProduct;
				}
				else
				{
					_products[newProduct.Id] = newProduct;
				}
			}

			_orderedProducts.AddRange(_products.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));
			_orderedFailProducts.AddRange(_failProducts.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));

			CalculateCraftIsValid();
		}
	}

	public Craft(Models.Craft craft, IFuturemud gameworld) : base(craft.EditableItem)
	{
		Gameworld = gameworld;
		_id = craft.Id;
		_name = craft.Name;
		RevisionNumber = craft.RevisionNumber;
		Blurb = craft.Blurb;
		Category = craft.Category.Proper();
		ActionDescription = craft.ActionDescription;
		ActiveCraftItemSDesc = craft.ActiveCraftItemSdesc;
		Interruptable = craft.Interruptable;
		ToolQualityWeighting = craft.ToolQualityWeighting;
		InputQualityWeighting = craft.InputQualityWeighting;
		CheckQualityWeighting = craft.CheckQualityWeighting;
		QualityFormula = new TraitExpression(
			craft.QualityFormula == "ExpressionEngine.Expression" ? "5 + (outcome/3) + (variable/20)" : craft.QualityFormula, // TODO - remove this later, fixing a temporary bug
			Gameworld);
		FreeSkillChecks = craft.FreeSkillChecks;
		IsPracticalCheck = craft.IsPracticalCheck;
		FailThreshold = (Outcome)craft.FailThreshold;
		CheckDifficulty = (Difficulty)craft.CheckDifficulty;
		FailPhase = craft.FailPhase;
		CheckTrait = gameworld.Traits.Get(craft.CheckTraitId ?? 0);
		AppearInCraftsListProg = gameworld.FutureProgs.Get(craft.AppearInCraftsListProgId ?? 0);
		CanUseProg = gameworld.FutureProgs.Get(craft.CanUseProgId ?? 0);
		WhyCannotUseProg = gameworld.FutureProgs.Get(craft.WhyCannotUseProgId ?? 0);
		OnUseProgStart = gameworld.FutureProgs.Get(craft.OnUseProgStartId ?? 0);
		OnUseProgComplete = gameworld.FutureProgs.Get(craft.OnUseProgCompleteId ?? 0);
		OnUseProgCancel = gameworld.FutureProgs.Get(craft.OnUseProgCancelId ?? 0);
		var phaseEchoes = new List<string>();
		var failPhaseEchoes = new List<string>();
		var phaseLengths = new List<TimeSpan>();
		foreach (var phase in craft.CraftPhases.OrderBy(x => x.PhaseNumber))
		{
			phaseEchoes.Add(phase.Echo);
			failPhaseEchoes.Add(phase.FailEcho ?? phase.Echo);
			phaseLengths.Add(TimeSpan.FromSeconds(phase.PhaseLengthInSeconds));
		}

		_phaseEchoes = phaseEchoes;
		_phaseLengths = phaseLengths;
		_failPhaseEchoes = failPhaseEchoes;

		foreach (var input in craft.CraftInputs)
		{
			var newInput = CraftInputFactory.Factory.LoadInput(input, this, Gameworld);
			_inputs[newInput.Id] = newInput;
		}

		_orderedInputs.AddRange(_inputs.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));

		foreach (var tool in craft.CraftTools)
		{
			var newTool = CraftToolFactory.Factory.LoadTool(tool, this, Gameworld);
			_tools[newTool.Id] = newTool;
		}

		_orderedTools.AddRange(_tools.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));

		foreach (var product in craft.CraftProducts)
		{
			var newProduct = CraftProductFactory.Factory.LoadProduct(product, this, Gameworld);
			if (product.IsFailProduct)
			{
				_failProducts[newProduct.Id] = newProduct;
			}
			else
			{
				_products[newProduct.Id] = newProduct;
			}
		}

		_orderedProducts.AddRange(_products.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));
		_orderedFailProducts.AddRange(_failProducts.Select(x => x.Value).OrderBy(x => x.OriginalAdditionTime));

		CalculateCraftIsValid();
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.Crafts.GetAll(Id);
	}

	public Craft(IAccount originator) : base(originator)
	{
		Gameworld = originator.Gameworld;
		_id = Gameworld.Crafts.NextID();
		_name = "unnamed craft";
		Blurb = "This craft has no blurb";
		ActionDescription = "crafting something";
		ActiveCraftItemSDesc = "an item being crafted";
		Category = "general";
		QualityFormula = new TraitExpression("5 + (outcome/3) + (variable/20)", Gameworld);
		FailThreshold = Outcome.MajorFail;
		CheckDifficulty = Difficulty.Normal;
		CheckQualityWeighting = 1.0;
		InputQualityWeighting = 1.0;
		ToolQualityWeighting = 1.0;
		FreeSkillChecks = 3;
		IsPracticalCheck = true;
		Interruptable = false;
		_phaseEchoes = new List<string>();
		_failPhaseEchoes = new List<string>();
		_phaseLengths = new List<TimeSpan>();
		using (new FMDB())
		{
			var dbnew = new Models.Craft();
			FMDB.Context.Crafts.Add(dbnew);
			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = 0;
			dbnew.EditableItem.BuilderAccountId = originator.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

			dbnew.Id = _id;
			dbnew.RevisionNumber = 0;
			dbnew.Name = Name;
			dbnew.Blurb = Blurb;
			dbnew.ActionDescription = ActionDescription;
			dbnew.ActiveCraftItemSdesc = ActiveCraftItemSDesc;
			dbnew.Category = Category;
			dbnew.Interruptable = Interruptable;
			dbnew.ToolQualityWeighting = ToolQualityWeighting;
			dbnew.InputQualityWeighting = InputQualityWeighting;
			dbnew.CheckQualityWeighting = CheckQualityWeighting;
			dbnew.FreeSkillChecks = FreeSkillChecks;
			dbnew.IsPracticalCheck = true;
			dbnew.FailPhase = FailPhase;
			dbnew.FailThreshold = (int)FailThreshold;
			dbnew.CheckDifficulty = (int)CheckDifficulty;
			dbnew.CheckTraitId = CheckTrait?.Id;
			dbnew.QualityFormula = QualityFormula.OriginalFormulaText;
			dbnew.AppearInCraftsListProgId = AppearInCraftsListProg?.Id;
			dbnew.CanUseProgId = CanUseProg?.Id;
			dbnew.WhyCannotUseProgId = WhyCannotUseProg?.Id;
			dbnew.OnUseProgStartId = OnUseProgStart?.Id;
			dbnew.OnUseProgCompleteId = OnUseProgComplete?.Id;
			dbnew.OnUseProgCancelId = OnUseProgCancel?.Id;

			for (var i = 0; i < _phaseLengths.Count; i++)
			{
				var dbphase = new CraftPhase();
				dbnew.CraftPhases.Add(dbphase);
				dbphase.Echo = _phaseEchoes[i];
				dbphase.FailEcho = _failPhaseEchoes[i];
				dbphase.PhaseLengthInSeconds = _phaseLengths[i].TotalSeconds;
				dbphase.PhaseNumber = i + 1;
			}

			FMDB.Context.SaveChanges();
		}
	}

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		using (new FMDB())
		{
			var dbcraft = FMDB.Context.Crafts.Find(Id, RevisionNumber);
			if (dbcraft == null)
			{
				Changed = false;
				return;
			}

			Save(dbcraft.EditableItem);
			dbcraft.Name = Name;
			dbcraft.Blurb = Blurb;
			dbcraft.ActionDescription = ActionDescription;
			dbcraft.ActiveCraftItemSdesc = ActiveCraftItemSDesc;
			dbcraft.Category = Category.Proper();
			dbcraft.Interruptable = Interruptable;
			dbcraft.ToolQualityWeighting = ToolQualityWeighting;
			dbcraft.InputQualityWeighting = InputQualityWeighting;
			dbcraft.CheckQualityWeighting = CheckQualityWeighting;
			dbcraft.FreeSkillChecks = FreeSkillChecks;
			dbcraft.IsPracticalCheck = IsPracticalCheck;
			dbcraft.FailPhase = FailPhase;
			dbcraft.FailThreshold = (int)FailThreshold;
			dbcraft.CheckDifficulty = (int)CheckDifficulty;
			dbcraft.CheckTraitId = CheckTrait?.Id;
			dbcraft.QualityFormula = QualityFormula.OriginalFormulaText;
			dbcraft.AppearInCraftsListProgId = AppearInCraftsListProg?.Id;
			dbcraft.CanUseProgId = CanUseProg?.Id;
			dbcraft.WhyCannotUseProgId = WhyCannotUseProg?.Id;
			dbcraft.OnUseProgStartId = OnUseProgStart?.Id;
			dbcraft.OnUseProgCompleteId = OnUseProgComplete?.Id;
			dbcraft.OnUseProgCancelId = OnUseProgCancel?.Id;

			dbcraft.CraftPhases.Clear();
			for (var i = 0; i < _phaseLengths.Count; i++)
			{
				var dbphase = new CraftPhase();
				dbcraft.CraftPhases.Add(dbphase);
				dbphase.Echo = _phaseEchoes[i];
				dbphase.FailEcho = _failPhaseEchoes[i];
				dbphase.PhaseLengthInSeconds = _phaseLengths[i].TotalSeconds;
				dbphase.PhaseNumber = i + 1;
			}

			foreach (var input in dbcraft.CraftInputs.ToList())
			{
				if (_orderedInputs.All(x => x.Id != input.Id))
				{
					dbcraft.CraftInputs.Remove(input);
					FMDB.Context.CraftInputs.Remove(input);
				}
			}

			foreach (var tool in dbcraft.CraftTools.ToList())
			{
				if (_orderedTools.All(x => x.Id != tool.Id))
				{
					dbcraft.CraftTools.Remove(tool);
					FMDB.Context.CraftTools.Remove(tool);
				}
			}

			foreach (var product in dbcraft.CraftProducts.ToList())
			{
				if (_orderedProducts.All(x => x.Id != product.Id) && _orderedFailProducts.All(x => x.Id != product.Id))
				{
					dbcraft.CraftProducts.Remove(product);
					FMDB.Context.CraftProducts.Remove(product);
				}
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public ICraft Clone(IAccount originator, string newName)
	{
		return new Craft(this, originator, newName);
	}

	public string Blurb { get; set; }

	/// <summary>
	/// e.g. crafting a sword
	/// </summary>
	public string ActionDescription { get; set; }

	public string ActiveCraftItemSDesc { get; set; }

	public string Category { get; set; }

	public bool Interruptable { get; set; }

	public double ToolQualityWeighting { get; set; }
	public double InputQualityWeighting { get; set; }

	public double CheckQualityWeighting { get; set; }

	public int FreeSkillChecks { get; set; }
	public bool IsPracticalCheck { get; set; }

	private readonly Dictionary<long, ICraftTool> _tools = new();

	public IEnumerable<ICraftTool> Tools => _orderedTools;

	private readonly List<ICraftTool> _orderedTools = new();

	private readonly Dictionary<long, ICraftInput> _inputs = new();

	public IEnumerable<ICraftInput> Inputs => _orderedInputs;

	private readonly List<ICraftInput> _orderedInputs = new();

	private readonly Dictionary<long, ICraftProduct> _products = new();

	public IEnumerable<ICraftProduct> Products => _orderedProducts;

	private readonly List<ICraftProduct> _orderedProducts = new();

	private readonly Dictionary<long, ICraftProduct> _failProducts = new();

	public IEnumerable<ICraftProduct> FailProducts => _orderedFailProducts;

	private readonly List<ICraftProduct> _orderedFailProducts = new();

	private readonly Dictionary<int, IInventoryPlanTemplate> _phaseInventoryPlans = new();
	private readonly Dictionary<int, IInventoryPlanTemplate> _failPhaseInventoryPlans = new();

	public Outcome FailThreshold { get; set; }
	public ITraitDefinition CheckTrait { get; set; }
	public Difficulty CheckDifficulty { get; set; }
	public IEnumerable<string> PhaseEchoes => _phaseEchoes;
	public IEnumerable<TimeSpan> PhaseLengths => _phaseLengths;
	public IEnumerable<string> FailPhaseEchoes => _failPhaseEchoes;
	public int FailPhase { get; set; }
	public ITraitExpression QualityFormula { get; set; }
	public IFutureProg AppearInCraftsListProg { get; set; }
	public IFutureProg CanUseProg { get; set; }
	public IFutureProg WhyCannotUseProg { get; set; }
	public IFutureProg OnUseProgStart { get; set; }
	public IFutureProg OnUseProgComplete { get; set; }
	public IFutureProg OnUseProgCancel { get; set; }

	private readonly DictionaryWithDefault<long, int> _craftInputConsumedPhases = new();
	private readonly DictionaryWithDefault<long, int> _craftProductProducedPhases = new();
	private readonly DictionaryWithDefault<long, int> _craftFailProductProducedPhases = new();
	private readonly DictionaryWithDefault<int, HashSet<long>> _craftToolUsagePhases = new();
	private readonly DictionaryWithDefault<int, HashSet<long>> _craftToolUsageFailPhases = new();

	private static readonly Regex InputInEchoRegex = new(@"\$i(?<id>\d+)", RegexOptions.IgnoreCase);
	private static readonly Regex ToolInEchoRegex = new(@"\$t(?<id>\d+)", RegexOptions.IgnoreCase);
	private static readonly Regex ProductInEchoRegex = new(@"\$p(?<id>\d+)", RegexOptions.IgnoreCase);
	private static readonly Regex FailProductInEchoRegex = new(@"\$f(?<id>\d+)", RegexOptions.IgnoreCase);

	private ItemQuality GetReferenceQuality(ICharacter character, IActiveCraftGameItemComponent component)
	{
		if (component.QualityCheckOutcome == Outcome.NotTested)
		{
			var qualityCheck = Gameworld.GetCheck(CheckType.CraftQualityCheck);
			component.QualityCheckOutcome = qualityCheck.Check(character, CheckDifficulty, CheckTrait);
		}

		var outcome = component.QualityCheckOutcome;
		if (QualityFormula != null)
		{
			QualityFormula.Formula.Parameters["outcome"] = (int)outcome;
		}

		var outcomeQuality = (ItemQuality)Math.Min((int)ItemQuality.Legendary,
			Math.Max(Convert.ToInt32(QualityFormula?.Evaluate(character, CheckTrait) ?? 0), (int)ItemQuality.Terrible));
		var netToolQuality = component.UsedToolQualities.Values.GetNetQuality();
		var netInputQuality = component.ConsumedInputs
		                               .Select(x => (x.Value.Data.InputQuality, x.Key.InputQualityWeight))
		                               .GetNetQuality();
		var finalQuality = new[]
		{
			(outcomeQuality, CheckQualityWeighting),
			(netToolQuality, ToolQualityWeighting),
			(netInputQuality, InputQualityWeighting)
		}.GetNetQuality();
		Gameworld.DebugMessage($@"Craft #{Id:N0} ({Name.ColourName()}) Quality Outcome

	Quality Formula: {outcomeQuality.DescribeEnum().ColourValue()}
	Tool Quality: {netToolQuality.DescribeEnum().ColourValue()}
	Input Quality: {netInputQuality.DescribeEnum().ColourValue()}
	Final Quality: {finalQuality.DescribeEnum().ColourValue()}");
		return finalQuality;
	}

	private Outcome HandleSkillCheck(ICharacter character, IActiveCraftGameItemComponent component)
	{
		if (CheckTrait == null)
		{
			return Outcome.NotTested;
		}

		if (component.Phase == FailPhase)
		{
			var outcomeCheck = Gameworld.GetCheck(CheckType.CraftOutcomeCheck);
			var result = outcomeCheck.Check(character, CheckDifficulty, CheckTrait,
				traitUseType: IsPracticalCheck ? TraitUseType.Practical : TraitUseType.Theoretical);
			component.CheckOutcome = result;
			if (FailThreshold >= result.Outcome)
			{
				component.HasFailed = true;
			}

			for (var i = 0; i < FreeSkillChecks; i++)
			{
				outcomeCheck.Check(character, CheckDifficulty, CheckTrait,
					traitUseType: IsPracticalCheck ? TraitUseType.Practical : TraitUseType.Theoretical);
			}

			return result.Outcome;
		}

		return Outcome.NotTested;
	}

	private readonly List<(string Echo, int Phase, bool Fail)> _invalidEchoReferences = new();

	private void SetupPhaseInformation()
	{
		var phase = 1;
		var phaseNumbers = new HashSet<int>();
		_invalidEchoReferences.Clear();
		_craftIsValid = true;
		_phaseInventoryPlans.Clear();
		_failPhaseInventoryPlans.Clear();
		_craftFailProductProducedPhases.Clear();
		_craftProductProducedPhases.Clear();
		_craftToolUsageFailPhases.Clear();
		_craftInputConsumedPhases.Clear();

		// Scoop when items are required based on when they appear in echoes
		foreach (var echo in PhaseEchoes)
		{
			foreach (Match match in InputInEchoRegex.Matches(echo))
			{
				var whichInput = _orderedInputs.ElementAtOrDefault(int.Parse(match.Groups["id"].Value) - 1);
				if (whichInput == null)
				{
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, false));
					continue;
				}

				if (!_craftInputConsumedPhases.ContainsKey(whichInput.Id))
				{
					_craftInputConsumedPhases[whichInput.Id] = phase;
				}
			}

			_craftToolUsagePhases[phase] = new HashSet<long>();
			foreach (Match match in ToolInEchoRegex.Matches(echo))
			{
				var whichTool = _orderedTools.ElementAtOrDefault(int.Parse(match.Groups["id"].Value) - 1);
				if (whichTool == null)
				{
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, false));
					continue;
				}

				_craftToolUsagePhases[phase].Add(whichTool.Id);
			}

			foreach (Match match in ProductInEchoRegex.Matches(echo))
			{
				var whichProduct = _orderedProducts.ElementAtOrDefault(int.Parse(match.Groups["id"].Value) - 1);
				if (whichProduct == null)
				{
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, false));
					continue;
				}

				if (!_craftProductProducedPhases.ContainsKey(whichProduct.Id))
				{
					_craftProductProducedPhases[whichProduct.Id] = phase;
				}
			}

			foreach (Match match in FailProductInEchoRegex.Matches(echo))
			{
				// Fail products should never appear in regular echoes
				_craftIsValid = false;
				_invalidEchoReferences.Add((match.Value, phase, false));
				continue;
			}

			phaseNumbers.Add(phase++);
		}

		// Check the fail echoes for the same
		phase = 1;
		foreach (var echo in FailPhaseEchoes)
		{
			foreach (Match match in InputInEchoRegex.Matches(echo))
			{
				var whichInput = _orderedInputs.ElementAtOrDefault(int.Parse(match.Groups["id"].Value) - 1);
				if (whichInput == null)
				{
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, true));
					continue;
				}

				if (!_craftInputConsumedPhases.ContainsKey(whichInput.Id))
				{
					// Unconsumed inputs may not appear in fail phases
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, true));
				}
			}

			if (!_craftToolUsageFailPhases.ContainsKey(phase))
			{
				_craftToolUsageFailPhases[phase] = new HashSet<long>();
			}

			foreach (Match match in ToolInEchoRegex.Matches(echo))
			{
				var whichTool = _orderedTools.ElementAtOrDefault(int.Parse(match.Groups["id"].Value) - 1);
				if (whichTool == null)
				{
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, true));
					continue;
				}

				if (!_craftToolUsageFailPhases[phase].Contains(whichTool.Id))
				{
					_craftToolUsageFailPhases[phase].Add(whichTool.Id);
				}
			}

			foreach (Match match in ProductInEchoRegex.Matches(echo))
			{
				var whichProduct = _orderedProducts.ElementAtOrDefault(int.Parse(match.Groups["id"].Value) - 1);
				if (whichProduct == null)
				{
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, true));
					continue;
				}

				if (!_craftProductProducedPhases.ContainsKey(whichProduct.Id))
				{
					// If a product hasn't been produced in a fail phase, it's an error
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, true));
					continue;
				}
			}

			foreach (Match match in FailProductInEchoRegex.Matches(echo))
			{
				var whichProduct = _orderedFailProducts.ElementAtOrDefault(int.Parse(match.Groups["id"].Value) - 1);
				if (whichProduct == null)
				{
					_craftIsValid = false;
					_invalidEchoReferences.Add((match.Value, phase, true));
					continue;
				}

				if (!_craftFailProductProducedPhases.ContainsKey(whichProduct.Id))
				{
					_craftFailProductProducedPhases[whichProduct.Id] = phase;
				}
			}

			phaseNumbers.Add(phase++);
		}

		foreach (var item in _inputs.Keys)
		{
			if (!_craftInputConsumedPhases.ContainsKey(item))
			{
				_craftInputConsumedPhases[item] = 1;
			}
		}

		foreach (var item in _products.Keys)
		{
			if (!_craftProductProducedPhases.ContainsKey(item))
			{
				_craftProductProducedPhases[item] = FailPhase;
			}
		}

		foreach (var item in _failProducts.Keys)
		{
			if (!_craftFailProductProducedPhases.ContainsKey(item))
			{
				_craftFailProductProducedPhases[item] = FailPhase;
			}
		}

		foreach (var item in _tools.Keys)
		{
			if (_craftToolUsagePhases.All(x => !x.Value.Contains(item)))
			{
				foreach (var key in _craftToolUsagePhases.Keys)
				{
					_craftToolUsagePhases[key].Add(item);
				}
			}
		}

		if (_phaseEchoes.Any())
		{
			foreach (var phasenum in 1.GetIntRange(_phaseEchoes.Count))
			{
				_phaseInventoryPlans[phasenum] = new InventoryPlanTemplate(Gameworld, new[]
				{
					new InventoryPlanPhaseTemplate(1,
						from toolId in _craftToolUsagePhases[phasenum]
						let tool = _tools[toolId]
						select InventoryPlanAction.LoadAction(
							Gameworld,
							tool.DesiredState,
							0,
							0,
							tool.EvaluateToolFunction(this, phasenum),
							null,
							1,
							tool.ToolFitness,
							true,
							tool
						))
				});

				_failPhaseInventoryPlans[phasenum] = new InventoryPlanTemplate(Gameworld, new[]
				{
					new InventoryPlanPhaseTemplate(1,
						from toolId in _craftToolUsageFailPhases[phasenum]
						let tool = _tools[toolId]
						select InventoryPlanAction.LoadAction(
							Gameworld,
							tool.DesiredState,
							0,
							0,
							tool.EvaluateToolFunction(this, phasenum),
							null,
							1,
							tool.ToolFitness,
							true,
							tool
						))
				});
			}
		}
	}

	private EmoteOutput ProduceEmoteOutput(string echo, IActiveCraftGameItemComponent component,
		IEnumerable<InventoryPlanActionResult> results, ICharacter character)
	{
		var emoteText = echo;
		var perceivables = new List<IPerceivable> { character };
		var perceiverIndex = 1;
		emoteText = InputInEchoRegex.Replace(emoteText, m =>
		{
			var input = _orderedInputs[int.Parse(m.Groups["id"].Value) - 1];
			var inputData = component.ConsumedInputs[input].Data;
			perceivables.Add(inputData.Perceivable);
			return $"${perceiverIndex++}";
		});

		emoteText = ToolInEchoRegex.Replace(emoteText, m =>
		{
			var tool = _orderedTools[int.Parse(m.Groups["id"].Value) - 1];
			var actualTool = results.First(x => x.OriginalReference == tool).PrimaryTarget;
			perceivables.Add(actualTool);
			return $"${perceiverIndex++}";
		});

		if (component.HasFailed)
		{
			emoteText = FailProductInEchoRegex.Replace(emoteText, m =>
			{
				var product = _orderedFailProducts[int.Parse(m.Groups["id"].Value) - 1];
				if (!component.ProducedProducts.ContainsKey(product))
				{
					throw new ApplicationException(
						$"Craft {Id}r{RevisionNumber} ({Name}) referenced fail product {product.Id} ({product.Name}) in phase {component.Phase} fail echo {echo}, but it had not yet been loaded.");
				}

				var productPerceivable = component.ProducedProducts[product].Perceivable;
				perceivables.Add(productPerceivable);
				return $"${perceiverIndex++}";
			});
		}
		else
		{
			emoteText = ProductInEchoRegex.Replace(emoteText, m =>
			{
				var product = _orderedProducts[int.Parse(m.Groups["id"].Value) - 1];
				if (!component.ProducedProducts.ContainsKey(product))
				{
					throw new ApplicationException(
						$"Craft {Id}r{RevisionNumber} ({Name}) referenced product {product.Id} ({product.Name}) in phase {component.Phase} echo {echo}, but it had not yet been loaded.");
				}

				var productPerceivable = component.ProducedProducts[product].Perceivable;
				perceivables.Add(productPerceivable);
				return $"${perceiverIndex++}";
			});
		}

		return new EmoteOutput(new Emote(emoteText, character, perceivables.ToArray()));
	}

	private (bool Success, IEnumerable<(ICraftInput Input, IPerceivable Target)> Inputs, Dictionary<int, IInventoryPlan>
		InventoryPlan, IEnumerable<ICraftInput> missingInputs) ScoutToolsAndInputs(ICharacter character,
			IActiveCraftGameItemComponent component, int fromPhase = 1, int toPhase = int.MaxValue)
	{
		// Sanity Check Max Phase
		if (toPhase > PhaseEchoes.Count())
		{
			toPhase = PhaseEchoes.Count();
		}

		var plans = new Dictionary<int, IInventoryPlan>();
		// First check to see if the tool plan can work
		foreach (var plan in _phaseInventoryPlans)
		{
			if (plan.Key > toPhase || plan.Key < fromPhase)
			{
				continue;
			}

			var template = component?.HasFailed == true ? _failPhaseInventoryPlans[plan.Key] : plan.Value;
			var phasePlan = template.CreatePlan(character);
			plans[plan.Key] = phasePlan;
		}

		if (plans.Any(x => x.Value.PlanIsFeasible() != InventoryPlanFeasibility.Feasible))
		{
			return (false, Enumerable.Empty<(ICraftInput Input, IPerceivable Target)>(), plans, null);
		}

		var phasesChecked = fromPhase.GetIntRange(toPhase);
		var scoutedTargets = new List<(ICraftInput Input, IEnumerable<IPerceivable> Options)>();
		var missingInputs = new List<ICraftInput>();

		// Next gather a list of the potential options for each input not already consumed in those phases
		foreach (var input in Inputs)
		{
			if (component?.ConsumedInputs.ContainsKey(input) == true)
			{
				continue;
			}

			if (_craftInputConsumedPhases[input.Id] < fromPhase)
			{
				var makeup = input.ScoutInput(character).ToList();
				if (!makeup.Any())
				{
					missingInputs.Add(input);
					continue;
				}

				scoutedTargets.Add((input, makeup));
				continue;
			}

			if (phasesChecked.All(x => _craftInputConsumedPhases[(int)input.Id] != x))
			{
				continue;
			}

			var targets = input.ScoutInput(character).ToList();
			if (!targets.Any())
			{
				missingInputs.Add(input);
				continue;
			}

			scoutedTargets.Add((input, targets));
		}

		// We missed some options
		if (missingInputs.Any())
		{
			return (false, Enumerable.Empty<(ICraftInput Input, IPerceivable Target)>(), plans, missingInputs);
		}

		// Add back in already consumed options so that they can be excluded as clashes
		foreach (var consumed in component?.ConsumedInputs ??
		                         Enumerable
			                         .Empty<KeyValuePair<ICraftInput, (IPerceivable Original, ICraftInputData Data)>>())
		{
			scoutedTargets.Add((consumed.Key, new[] { consumed.Value.Data.Perceivable }));
		}

		// Use the clash solver to pick only one option for each input and make sure it is unique
		var clashSolver = new OptionSolver<ICraftInput, IPerceivable>(
			from result in scoutedTargets
			select new Choice<ICraftInput, IPerceivable>(result.Input,
				from option in result.Options
				select new PerceivableOption(option)
			)
			{
				OptionScorer = result.Input.ScoreInputDesirability
			}
		);

		var clashResults = clashSolver.SolveOptions();
		if (!clashResults.Success)
		{
			return (false, Enumerable.Empty<(ICraftInput Input, IPerceivable Target)>(), plans,
				clashResults.UnsolvableChoices);
		}

		return (true, clashResults.Solution, plans, null);
	}

	public (bool Success, string Error) CanDoCraft(ICharacter character, IActiveCraftGameItemComponent component,
		bool allowStartOnly, bool ignoreToolAndMaterialFailure)
	{
		if (!CharacterState.Able.HasFlag(character.State))
		{
			return (false, $"You can't perform any crafts while you are {character.State.Describe()}.");
		}

		if (character.Combat != null && character.IsEngagedInMelee)
		{
			return (false, "You can't perform any crafts while you are engaged in melee combat!");
		}

		if (character.Movement != null)
		{
			return (false, "You must first stop moving before you can perform any crafts.");
		}

		if ((bool?)CanUseProg?.Execute(character) == false)
		{
			return (false, WhyCannotUseProg?.Execute(character).ToString() ?? "You cannot use that craft right now.");
		}

		var (success, _, plans, missingInput) = ScoutToolsAndInputs(character, component);
		if (success)
		{
			return (true, string.Empty);
		}

		if (allowStartOnly && Interruptable)
		{
			(success, _, plans, missingInput) = ScoutToolsAndInputs(character, component, 1, 1);
			if (success)
			{
				return (true, string.Empty);
			}
		}

		if (ignoreToolAndMaterialFailure)
		{
			return (true, string.Empty);
		}

		foreach (var plan in plans)
		{
			switch (plan.Value.PlanIsFeasible())
			{
				case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					return (false,
							$"You cannot do that craft because you do not have enough {character.Body.Prototype.WielderDescriptionPlural} to hold all the tools you will need."
						);
				case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
					return (false,
							$"You cannot do that craft because you do not have enough {character.Body.Prototype.WielderDescriptionPlural} to wield all the tools you will need."
						);
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					return (false, "You cannot do that craft because you are missing some of the necessary tools.");
			}
		}

		return (false,
			$"You cannot do that craft because you cannot find the materials to do some or all of {missingInput.Select(x => x.Name.Colour(Telnet.Green)).ListToString()}.");
	}

	private bool _craftIsValid;
	public bool CraftIsValid => _craftIsValid;

	private readonly List<TimeSpan> _phaseLengths;
	private readonly List<string> _failPhaseEchoes;
	private readonly List<string> _phaseEchoes;

	public bool CraftChanged
	{
		get => Changed;
		set
		{
			Changed = value;
			CalculateCraftIsValid();
		}
	}

	public void CalculateCraftIsValid()
	{
		SetupPhaseInformation();
		if (_craftIsValid)
		{
			_craftIsValid = Inputs.All(x => x.IsValid()) && Tools.All(x => x.IsValid()) &&
			                Products.All(x => x.IsValid()) && FailProducts.All(x => x.IsValid()) &&
			                PhaseEchoes.Any() && AppearInCraftsListProg != null;
		}
	}

	public bool AppearInCraftsList(ICharacter actor)
	{
		return Status == RevisionStatus.Current &&
		       _craftIsValid &&
		       (actor.IsAdministrator() || (bool?)AppearInCraftsListProg?.Execute(actor) == true)
			;
	}

	public string GetMaterialPreview(ICharacter character)
	{
		var (success, error) = CanDoCraft(character, null, false, true);

		var (_, inputs, plans, missing) = ScoutToolsAndInputs(character, null);
		var sb = new StringBuilder();

		sb.AppendLine($"Materials preview for craft {Name.Colour(Telnet.Cyan)}:");
		if (!success)
		{
			sb.AppendLine();
			sb.AppendLine("Note: You cannot currently do this craft because of the below error.".Colour(Telnet.Red));
			sb.AppendLine(error);
		}
		sb.AppendLine();
		sb.AppendLine("Tools:");
		sb.AppendLine();
		var results = plans.SelectMany(x => x.Value.ScoutAllTargets());
		foreach (var item in _orderedTools)
		{
			var result = results.FirstOrDefault(x => x.OriginalReference == item && x.PrimaryTarget != null);
			if (result == null)
			{
				sb.AppendLine($"\t{item.HowSeen(character).Colour(Telnet.Yellow)}: {Telnet.Red}Missing{Telnet.RESET}");
			}
			else
			{
				sb.AppendLine(
					$"\t{item.HowSeen(character).Colour(Telnet.Yellow)}: {result.PrimaryTarget.HowSeen(character)}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Inputs:");
		sb.AppendLine();
		foreach (var input in _orderedInputs)
		{
			if (missing?.Contains(input) == true)
			{
				sb.AppendLine($"\t{input.HowSeen(character).Colour(Telnet.Yellow)}: {Telnet.Red}Missing{Telnet.RESET}");
			}
			else
			{
				var tInput = inputs.FirstOrDefault(x => x.Input == input).Target;
				if (tInput == null)
				{
					sb.AppendLine(
						$"\t{input.HowSeen(character).Colour(Telnet.Yellow)}: {Telnet.BoldYellow}Not Checked{Telnet.RESETALL}");
				}
				else
				{
					sb.AppendLine(
						$"\t{input.HowSeen(character).Colour(Telnet.Yellow)}: {inputs.First(x => x.Input == input).Target.HowSeen(character)}");
				}
			}
		}

		return sb.ToString();
	}

	public void BeginCraft(ICharacter character)
	{
		var craftItem = ActiveCraftGameItemComponentProto.LoadActiveCraft(this);
		craftItem.Parent.RoomLayer = character.RoomLayer;
		character.Location.Insert(craftItem.Parent);
		var effect = new ActiveCraftEffect(character)
		{
			Component = craftItem
		};

		effect.SubscribeEvents();
		character.AddEffect(effect, TimeSpan.FromSeconds(5));
		character.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ begin|begins the craft {Name.Colour(Telnet.Green)}", character)));
		OnUseProgStart?.Execute(character);
	}

	public void PauseCraft(ICharacter character, IActiveCraftGameItemComponent component, IActiveCraftEffect effect)
	{
		effect.ReleaseEvents();
		if (!Interruptable)
		{
			CancelCraft(character, component);
			return;
		}

		effect.Component.CraftWasInterrupted();
	}

	public void CancelCraft(ICharacter character, IActiveCraftGameItemComponent component)
	{
		component.ReleaseItems(character.Location, character.RoomLayer);
		OnUseProgCancel?.Execute(character);
		component.Parent.Delete();
	}

	public (bool Success, string Error) CanResumeCraft(ICharacter character, IActiveCraftGameItemComponent active)
	{
		var (success, _, plans, missingInput) = ScoutToolsAndInputs(character, active, active.Phase);
		if (!success)
		{
			if (Interruptable)
			{
				if (ScoutToolsAndInputs(character, active, active.Phase, active.Phase).Success)
				{
					return (true, string.Empty);
				}
			}

			foreach (var plan in plans.Values)
			{
				switch (plan.PlanIsFeasible())
				{
					case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
						return (false,
							$"You cannot do that craft because you do not have enough {character.Body.Prototype.WielderDescriptionPlural} to hold all the tools you will need.");
					case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
						return (false,
							$"You cannot do that craft because you do not have enough {character.Body.Prototype.WielderDescriptionPlural} to wield all the tools you will need.");
					case InventoryPlanFeasibility.NotFeasibleMissingItems:
						return (false, "You cannot do that craft because you are missing some of the necessary tools.");
				}
			}

			return (false,
				$"You cannot do that craft because you cannot find the materials to do some or all of {missingInput.Select(x => x.HowSeen(character).Colour(Telnet.Green)).ListToString()}.");
		}

		return (true, string.Empty);
	}

	public void ResumeCraft(ICharacter character, IActiveCraftGameItemComponent active)
	{
		var effect = new ActiveCraftEffect(character)
		{
			Component = active
		};

		effect.SubscribeEvents();
		character.AddEffect(effect, TimeSpan.FromSeconds(5));
		character.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ resume|resumes the craft {Name.Colour(Telnet.Green)}", character)));
	}

	public bool HandleCraftPhase(ICharacter character, IActiveCraftEffect effect,
		IActiveCraftGameItemComponent component, int phase)
	{
		var (success, inputs, plans, missing) = ScoutToolsAndInputs(character, component, phase, phase);
		var plan = plans.Single().Value;
		var results = plan.ExecuteWholePlan().ToList();
		var phaseLengthSeconds = _phaseLengths[phase - 1].TotalSeconds;
		var tools = results
		            .Select(x => (Item: x.PrimaryTarget, Tool: x.OriginalReference as ICraftTool))
		            .Where(x => x.Tool != null)
		            .ToList();

		foreach (var (item, tool) in tools)
		{
			phaseLengthSeconds *= tool.PhaseLengthMultiplier(item);
		}

		var phaseLength = TimeSpan.FromSeconds(phaseLengthSeconds);
		foreach (var (item, tool) in tools)
		{
			tool.UseTool(item, phaseLength, component.HasFailed);
		}

		plan.FinalisePlanNoRestore();
		if (!success)
		{
			if (missing != null)
			{
				character.OutputHandler.Handle(new EmoteOutput(
					new Emote(
						$"@ stop|stops {ActionDescription} because #0 cannot find some or all of {missing.Select(x => x.HowSeen(character).Colour(Telnet.Yellow)).ListToString()}.",
						character, character)));
			}
			else
			{
				character.OutputHandler.Handle(new EmoteOutput(
					new Emote(
						$"@ stop|stops {ActionDescription} because #0 cannot find the tools #0 need|needs.",
						character, character)));
			}

			PauseCraft(character, component, effect);
			return false;
		}

		if (!component.HasFailed)
		{
			foreach (var input in _inputs.Where(x =>
				         _craftInputConsumedPhases.ContainsKey(x.Key) && _craftInputConsumedPhases[x.Key] <= phase &&
				         !component.ConsumedInputs.ContainsKey(x.Value)))
			{
				var target = inputs.First(x => x.Input == input.Value).Target;
				component.ConsumedInputs[input.Value] = (target, input.Value.ReserveInput(target));
			}
		}

		HandleSkillCheck(character, component);
		var referenceQuality = GetReferenceQuality(character, component);

		string echo;
		if (component.HasFailed)
		{
			echo = FailPhaseEchoes.ElementAt(phase - 1);
			foreach (var product in _failProducts.Where(x =>
				         _craftFailProductProducedPhases.ContainsKey(x.Key) &&
				         _craftFailProductProducedPhases[x.Key] <= phase &&
				         !component.ProducedProducts.ContainsKey(x.Value)))
			{
				component.ProducedProducts[product.Value] = product.Value.ProduceProduct(component, referenceQuality);
			}
		}
		else
		{
			echo = PhaseEchoes.ElementAt(phase - 1);
			foreach (var product in _products.Where(x =>
				         _craftProductProducedPhases.ContainsKey(x.Key) &&
				         _craftProductProducedPhases[x.Key] <= phase &&
				         !component.ProducedProducts.ContainsKey(x.Value)))
			{
				component.ProducedProducts[product.Value] = product.Value.ProduceProduct(component, referenceQuality);
			}
		}

		effect.NextPhaseDuration = phaseLength;
		character.OutputHandler.Handle(ProduceEmoteOutput(echo, component, results, character));
		return true;
	}

	public string DisplayCraft(ICharacter character)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Craft: {Name?.Colour(Telnet.Cyan) ?? "Unnamed Craft".Colour(Telnet.Red)}");

		sb.AppendLineColumns((uint)character.LineFormatLength, 3,
			$"Category: {Category?.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
			$"Interruptable: {(Interruptable ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}",
			$"Total Length: {TimeSpan.FromMilliseconds(PhaseLengths.Take(PhaseLengths.Count() - 1).Sum(x => x.TotalMilliseconds)).Describe(character).ColourValue()}"
		);
		sb.AppendLineColumns((uint)character.LineFormatLength, 3,
			$"Skill: {CheckTrait?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
			$"Difficulty: {CheckDifficulty.Describe().Colour(Telnet.Green)}",
			$"Type: {(IsPracticalCheck ? "Practical" : "Theoretical").ColourValue()}"
		);
		if (CheckTrait is ISkillDefinition sd && !sd.Improver.CanImprove(character, character.GetTrait(sd),
			    CheckDifficulty, TraitUseType.Practical, true))
		{
			sb.AppendLine($"Warning: You cannot improve your skill with this craft.".Colour(Telnet.Red));
		}

		sb.AppendLine();
		sb.AppendLine($"Blurb: {Blurb.ColourCommand()}");
		if (Tools.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Required Tools:");
			foreach (var tool in Tools)
			{
				sb.AppendLine(
					$"\t{tool.HowSeen(character).ColourIncludingReset(Telnet.Yellow)} {$"[{tool.DesiredState.Describe()}]".ColourValue()}{(tool.UseToolDuration ? " [Use]".Colour(Telnet.Red) : "")}");
			}
		}

		if (Inputs.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Inputs:");
			foreach (var input in Inputs)
			{
				sb.AppendLine($"\t{input.HowSeen(character).ColourIncludingReset(Telnet.Yellow)}");
			}
		}

		if (Products.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Products:");
			foreach (var product in Products)
			{
				sb.AppendLine($"\t{product.HowSeen(character).ColourIncludingReset(Telnet.Yellow)}");
			}
		}

		if (FailProducts.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Fail Products:");
			foreach (var product in FailProducts)
			{
				sb.AppendLine($"\t{product.HowSeen(character).ColourIncludingReset(Telnet.Yellow)}");
			}
		}

		return sb.ToString();
	}

	public TimeSpan DurationForPhase(int phase)
	{
		return PhaseLengths.ElementAt(phase - 1);
	}

	public int LastPhase => PhaseLengths.Count();


	#region Overrides of EditableItem

	public string HelpText => @"You can use the following options with this command:

	#3name <name>#0 - sets the name of the craft
	#3category <category>#0 - sets the category of the craft
	#3blurb <blurb>#0 - sets the blurb that explains what the craft does and how to use it
	#3action <tag>#0 - appended to player's long description while working on the craft
	#3item <ldesc>#0 - the description of the crafting item made to track progress
	#3interupt#0 - toggles whether this craft can be interrupted and resumed
	#3appear <prog>#0 - set the prog that controls appearance in CRAFTS
	#3can <prog>#0 - set the prog that controls whether a player can use the craft
	#3why <prog>#0 - sets the prog that gives error messages about the CAN prog.
	#3onstart <prog>#0 - a prog that fires when the craft begins
	#3onfinish <prog>#0 - a prog that fires when the craft ends
	#3oncancel <prog>#0 - a prog that fires when the craft is cancelled.
	#3check <free rolls> <trait> <difficulty>#0 - sets the check for the craft
	#3practical#0 - toggles whether the skill check is a practical or theory check
	#3threshold <outcome>#0 - sets the minimum outcome required on the check to pass
	#3phase add <seconds> ""<echo>"" [""<failecho>""]#0 - adds a new craft phase
	#3phase edit <number> <seconds> ""<echo>"" [""<failecho>""]#0 - changes an existing phase
	#3phase remove <number>#0 - removes a phase
	#3phase swap <phase1> <phase2>#0 - swaps the order of two phases
	#3failphase <number>#0 - sets a phase to be where the check is made
	#3input add <type>#0 - adds a new input of the specified type
	#3input remove <number>#0 - removes an input
	#3input <number> <...>#0 - configures an input. See individual inputs for more info
	#3tool add <type>#0 - adds a new tool
	#3tool remove <number>#0 - removes a tool
	#3tool <number> <...>#0 - configures a tool. See individual tool for more info
	#3product add <type>#0 - adds a product
	#3product remove <number>#0 - removes a product
	#3product <number> <...>#0 - configures a product. See individual product for more info.
	#3failproduct add <type>#0 - adds a fail product
	#3failproduct remove <number>#0 - removes a fail product
	#3failproduct <number> <...>#0 - configures a fail product. See individual product for more info.";

	/// <summary>Handles OLC Building related commands from an Actor</summary>
	/// <param name="actor">The ICharacter requesting the edit</param>
	/// <param name="command">The command they have entered</param>
	/// <returns>True if anything was changed, false if the command was invalid or did not change anything</returns>
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "practical":
			case "theory":
			case "theoretical":
				return BuildingCommandPractical(actor);
			case "category":
				return BuildingCommandCategory(actor, command);
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "action":
				return BuildingCommandAction(actor, command);
			case "item":
			case "progress":
				return BuildingCommandItem(actor, command);
			case "fail":
			case "failphase":
			case "fail_phase":
				return BuildingCommandFailPhase(actor, command);
			case "phase":
				return BuildingCommandPhase(actor, command);
			case "input":
				return BuildingCommandInput(actor, command);
			case "tool":
				return BuildingCommandTool(actor, command);
			case "product":
				return BuildingCommandProduct(actor, command);
			case "failproduct":
			case "fail_product":
			case "fp":
				return BuildingCommandFailProduct(actor, command);
			case "interrupt":
			case "interruptable":
			case "stopable":
			case "stop":
			case "cancel":
			case "interupt":
			case "interuptable":
				return BuildingCommandInterruptable(actor, command);
			case "check":
				return BuildingCommandCheck(actor, command);
			case "threshold":
				return BuildingCommandFailThreshold(actor, command);
			case "appear":
			case "appearprog":
			case "appear_prog":
			case "show":
			case "showprog":
			case "show_prog":
				return BuildingCommandAppear(actor, command);
			case "can":
			case "canuse":
			case "canuseprog":
				return BuildingCommandCanUseProg(actor, command);
			case "why":
			case "whyprog":
			case "whycant":
			case "whycantuse":
			case "whycantuseprog":
			case "whycannot":
			case "whycannotuse":
			case "whycannotuseprog":
				return BuildingCommandWhyCannotUseProg(actor, command);
			case "onstart":
				return BuildingCommandOnStartProg(actor, command);
			case "onfinish":
			case "oncomplete":
			case "onend":
				return BuildingCommandOnFinishProg(actor, command);
			case "oncancel":
				return BuildingCommandOnCancelProg(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandPractical(ICharacter actor)
	{
		IsPracticalCheck = !IsPracticalCheck;
		CraftChanged = true;
		if (IsPracticalCheck)
		{
			actor.OutputHandler.Send(
				"The skill check associated with this craft is now practical, rather than theoretical.");
		}
		else
		{
			actor.OutputHandler.Send(
				"The skill check associated with this craft is now theoretical, rather than practical.");
		}

		return true;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What do you want the description of the item created to track progress for this craft to be? Note, it should be a passive construction like {"a sword being forged".Colour(Telnet.BoldCyan)}.");
			return false;
		}

		ActiveCraftItemSDesc = command.SafeRemainingArgument.ToLowerInvariant();
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"That craft now has the active craft item description {ActiveCraftItemSDesc.Colour(Telnet.BoldCyan)}.");
		return true;
	}

	private bool BuildingCommandFailProduct(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandFailProductAdd(actor, command);
			case "remove":
			case "delete":
			case "rem":
			case "del":
				return BuildingCommandFailProductRemove(actor, command);
		}

		if (!string.IsNullOrEmpty(command.Last) && int.TryParse(command.Last, out var value))
		{
			if (_orderedFailProducts.Count < value || value <= 0)
			{
				actor.OutputHandler.Send(
					$"There are {_orderedFailProducts.Count} fail products for you to edit. You must specify which one.");
				return false;
			}

			return _orderedFailProducts[value - 1].BuildingCommand(actor, command);
		}

		actor.OutputHandler.Send(
			"You must either add a new fail product, delete an existing one, or specify the number of the fail product you want to edit.");
		return false;
	}


	private bool BuildingCommandFailProductRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which product do you want to delete?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"There are {_orderedProducts.Count} products for you to edit. You must specify which one.");
			return false;
		}

		if (_orderedFailProducts.Count < value || value <= 0)
		{
			actor.OutputHandler.Send(
				$"There are {_orderedProducts.Count} products for you to edit. You must specify which one.");
			return false;
		}

		var product = _orderedFailProducts[value - 1];
		_failProducts.Remove(product.Id);
		_orderedFailProducts.RemoveAt(value - 1);
		CraftChanged = true;
		actor.OutputHandler.Send($"You delete the {value.ToOrdinal()} fail product, {product.Name}.");
		return true;
	}

	private bool BuildingCommandFailProductAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of product do you want to add?\nValid types are: {CraftProductFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		Gameworld.SaveManager.Flush();
		var newProduct = CraftProductFactory.Factory.LoadProduct(command.PopSpeech(), this, Gameworld, true);
		if (newProduct == null)
		{
			actor.OutputHandler.Send(
				$"There is no such product type to add.\nValid types are: {CraftProductFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		_orderedFailProducts.Add(newProduct);
		_failProducts.Add(newProduct.Id, newProduct);
		actor.OutputHandler.Send(
			$"You add a new fail product at position {_orderedFailProducts.Count} (referred to with $f{_orderedFailProducts.Count}).");
		return true;
	}

	private bool BuildingCommandProduct(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandProductAdd(actor, command);
			case "remove":
			case "delete":
			case "rem":
			case "del":
				return BuildingCommandProductRemove(actor, command);
		}

		if (!string.IsNullOrEmpty(command.Last) && int.TryParse(command.Last, out var value))
		{
			if (_orderedProducts.Count < value || value <= 0)
			{
				actor.OutputHandler.Send(
					$"There are {_orderedProducts.Count} products for you to edit. You must specify which one.");
				return false;
			}

			return _orderedProducts[value - 1].BuildingCommand(actor, command);
		}

		actor.OutputHandler.Send(
			"You must either add a new product, delete an existing one, or specify the number of the product you want to edit.");
		return false;
	}


	private bool BuildingCommandProductRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which product do you want to delete?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"There are {_orderedProducts.Count} products for you to edit. You must specify which one.");
			return false;
		}

		if (_orderedProducts.Count < value || value <= 0)
		{
			actor.OutputHandler.Send(
				$"There are {_orderedProducts.Count} products for you to edit. You must specify which one.");
			return false;
		}

		var product = _orderedProducts[value - 1];
		_products.Remove(product.Id);
		_orderedProducts.RemoveAt(value - 1);
		CraftChanged = true;
		product.Changed = false;
		Gameworld.SaveManager.Abort(product);
		actor.OutputHandler.Send($"You delete the {value.ToOrdinal()} product, {product.Name}.");
		return true;
	}

	private bool BuildingCommandProductAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of product do you want to add?\nValid types are: {CraftProductFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		Gameworld.SaveManager.Flush();
		var newProduct = CraftProductFactory.Factory.LoadProduct(command.PopSpeech(), this, Gameworld, false);
		if (newProduct == null)
		{
			actor.OutputHandler.Send(
				$"There is no such product type to add.\nValid types are: {CraftProductFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		_orderedProducts.Add(newProduct);
		_products.Add(newProduct.Id, newProduct);

		actor.OutputHandler.Send(
			$"You add a new product at position {_orderedProducts.Count} (referred to with $p{_orderedProducts.Count}).");
		return true;
	}

	private bool BuildingCommandTool(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandToolAdd(actor, command);
			case "remove":
			case "delete":
			case "rem":
			case "del":
				return BuildingCommandToolRemove(actor, command);
		}

		if (!string.IsNullOrEmpty(command.Last) && int.TryParse(command.Last, out var value))
		{
			if (_orderedTools.Count < value || value <= 0)
			{
				actor.OutputHandler.Send(
					$"There are {_orderedTools.Count} tools for you to edit. You must specify which one.");
				return false;
			}

			return _orderedTools[value - 1].BuildingCommand(actor, command);
		}

		actor.OutputHandler.Send(
			"You must either add a new tools, delete an existing one, or specify the number of the tool you want to edit.");
		return false;
	}

	private bool BuildingCommandToolRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tool do you want to delete?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"There are {_orderedTools.Count} tools for you to edit. You must specify which one.");
			return false;
		}

		if (_orderedTools.Count < value || value <= 0)
		{
			actor.OutputHandler.Send(
				$"There are {_orderedTools.Count} tools for you to edit. You must specify which one.");
			return false;
		}

		var tool = _orderedTools[value - 1];
		_tools.Remove(tool.Id);
		_orderedTools.RemoveAt(value - 1);
		foreach (var item in _craftToolUsagePhases)
		{
			item.Value.Remove(tool.Id);
		}

		foreach (var item in _craftToolUsageFailPhases)
		{
			item.Value.Remove(tool.Id);
		}

		CraftChanged = true;
		tool.Changed = false;
		Gameworld.SaveManager.Abort(tool);
		actor.OutputHandler.Send($"You delete the {value.ToOrdinal()} tool, {tool.Name}.");
		return true;
	}

	private bool BuildingCommandToolAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of tool do you want to add?\nValid types are: {CraftToolFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		Gameworld.SaveManager.Flush();
		var newTool = CraftToolFactory.Factory.LoadTool(command.PopSpeech(), this, Gameworld);
		if (newTool == null)
		{
			actor.OutputHandler.Send(
				$"There is no such tool type to add.\nValid types are: {CraftToolFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		_orderedTools.Add(newTool);
		_tools.Add(newTool.Id, newTool);
		actor.OutputHandler.Send(
			$"You add a new tool at position {_orderedTools.Count} (referred to with $t{_orderedTools.Count}).");
		return true;
	}

	private bool BuildingCommandInput(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
				return BuildingCommandInputAdd(actor, command);
			case "remove":
			case "delete":
			case "rem":
			case "del":
				return BuildingCommandInputRemove(actor, command);
		}

		if (!string.IsNullOrEmpty(command.Last) && int.TryParse(command.Last, out var value))
		{
			if (_orderedInputs.Count < value || value <= 0)
			{
				actor.OutputHandler.Send(
					$"There are {_orderedInputs.Count} inputs for you to edit. You must specify which one.");
				return false;
			}

			return _orderedInputs[value - 1].BuildingCommand(actor, command);
		}

		actor.OutputHandler.Send(
			"You must either add a new input, delete an existing one, or specify the number of the input you want to edit.");
		return false;
	}

	private bool BuildingCommandInputRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which input do you want to delete?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"There are {_orderedInputs.Count} inputs for you to edit. You must specify which one.");
			return false;
		}

		if (_orderedInputs.Count < value || value <= 0)
		{
			actor.OutputHandler.Send(
				$"There are {_orderedInputs.Count} inputs for you to edit. You must specify which one.");
			return false;
		}

		var input = _orderedInputs[value - 1];
		_inputs.Remove(input.Id);
		_orderedInputs.RemoveAt(value - 1);
		CraftChanged = true;
		input.Changed = false;
		Gameworld.SaveManager.Abort(input);
		actor.OutputHandler.Send($"You delete the {value.ToOrdinal()} input, {input.Name}.");
		return true;
	}

	private bool BuildingCommandInputAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of input do you want to add?\nValid types are: {CraftInputFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		Gameworld.SaveManager.Flush();
		var newInput = CraftInputFactory.Factory.LoadInput(command.PopSpeech(), this, Gameworld);
		if (newInput == null)
		{
			actor.OutputHandler.Send(
				$"There is no such input type to add.\nValid types are: {CraftInputFactory.Factory.ValidBuilderTypes.ListToString()}");
			return false;
		}

		_orderedInputs.Add(newInput);
		_inputs.Add(newInput.Id, newInput);
		actor.OutputHandler.Send(
			$"You add a new input at position {_orderedInputs.Count} (referred to with $i{_orderedInputs.Count}).");
		return true;
	}

	private bool BuildingCommandPhase(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to add, remove, edit or swap a phase?");
			return false;
		}

		switch (command.PopSpeech())
		{
			case "add":
			case "new":
				return BuildingCommandPhaseAdd(actor, command);
			case "remove":
			case "rem":
			case "del":
			case "delete":
				return BuildingCommandPhaseRemove(actor, command);
			case "edit":
			case "set":
				return BuildingCommandPhaseEdit(actor, command);
			case "swap":
				return BuildingCommandPhaseSwap(actor, command);
			default:
				actor.OutputHandler.Send("Do you want to add, remove, edit or swap a phase?");
				return false;
		}
	}

	private bool BuildingCommandPhaseAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a phase length, a phase echo, and optionally a fail echo.");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.OutputHandler.Send("You must specify a number of seconds for the phase to last.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a phase length, a phase echo, and optionally a fail echo.");
			return false;
		}

		var phaseText = command.PopSpeech();
		var failText = command.IsFinished ? phaseText : command.PopSpeech();
		_phaseEchoes.Add(phaseText);
		_failPhaseEchoes.Add(failText);
		_phaseLengths.Add(TimeSpan.FromSeconds(value));
		CraftChanged = true;
		actor.OutputHandler.Send("You add the specified phase to the craft.");
		return true;
	}

	private bool BuildingCommandPhaseRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value) || value <= 0 || value > LastPhase)
		{
			actor.OutputHandler.Send(
				$"You must specify a valid phase. There are currently {_phaseEchoes.Count} phases.");
			return false;
		}

		_phaseEchoes.RemoveAt(value - 1);
		_failPhaseEchoes.RemoveAt(value - 1);
		_phaseLengths.RemoveAt(value - 1);
		CraftChanged = true;
		actor.OutputHandler.Send($"You remove the {value.ToOrdinal()} phase.");
		return true;
	}

	private bool BuildingCommandPhaseEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to edit?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value) || value <= 0 || value > LastPhase)
		{
			actor.OutputHandler.Send(
				$"You must specify a valid phase. There are currently {_phaseEchoes.Count} phases.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a phase length, a phase echo, and optionally a fail echo.");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var seconds))
		{
			actor.OutputHandler.Send("You must specify a number of seconds for the phase to last.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a phase length, a phase echo, and optionally a fail echo.");
			return false;
		}

		var phaseText = command.PopSpeech();
		var failText = command.IsFinished ? phaseText : command.PopSpeech();
		_phaseEchoes[value - 1] = phaseText;
		_failPhaseEchoes[value - 1] = failText;
		_phaseLengths[value - 1] = TimeSpan.FromSeconds(seconds);
		CraftChanged = true;
		actor.OutputHandler.Send($"You edit the {value.ToOrdinal()} phase of the craft.");
		return true;
	}

	private bool BuildingCommandPhaseSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to swap?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value) || value <= 0 || value > LastPhase)
		{
			actor.OutputHandler.Send(
				$"You must specify a valid phase. There are currently {_phaseEchoes.Count} phases.");
			return false;
		}


		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which phase do you want to swap it with?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value2) || value2 <= 0 || value2 > LastPhase)
		{
			actor.OutputHandler.Send(
				$"You must specify a valid phase. There are currently {_phaseEchoes.Count} phases.");
			return false;
		}

		if (value == value2)
		{
			actor.OutputHandler.Send("You cannot swap a phase with itself.");
			return false;
		}

		_phaseEchoes.Swap(value - 1, value2 - 1);
		_failPhaseEchoes.Swap(value - 1, value2 - 1);
		_phaseLengths.Swap(value - 1, value2 - 1);
		CraftChanged = true;
		actor.OutputHandler.Send($"You swap the {value.ToOrdinal()} and {value2.ToOrdinal()} phases.");
		return true;
	}

	private bool BuildingCommandFailThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which outcome do you want to set as the fail threshold? See SHOW OUTCOMES for a list of possible outcomes.");
			return false;
		}

		if (!CheckExtensions.GetOutcome(command.SafeRemainingArgument, out var outcome))
		{
			actor.OutputHandler.Send("That is not a valid outcome. See SHOW OUTCOMES for a list of possible outcomes.");
			return false;
		}

		FailThreshold = outcome;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft will now be considered to have failed it an outcome of {FailThreshold.DescribeColour()} or worse is rolled on the check.");
		return true;
	}

	private bool BuildingCommandFailPhase(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value < 0 ||
		    value > LastPhase)
		{
			actor.OutputHandler.Send(
				"Which phase do you want to do the success check on, and split into pass/failure thereafter?");
			return false;
		}

		FailPhase = value;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft will now do its success check and split into pass/failure on the {FailPhase.ToOrdinal()} phase.");
		return true;
	}

	#region Building SubCommands

	private bool BuildingCommandOnCancelProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to execute when the craft is cancelled?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			OnUseProgCancel = null;
			CraftChanged = true;
			actor.OutputHandler.Send("This craft no longer uses a prog upon cancellation.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} does not comply.");
			return false;
		}

		OnUseProgCancel = prog;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft now executes the prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} when cancelled.");
		return true;
	}

	private bool BuildingCommandOnFinishProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to execute when the craft is finished?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			OnUseProgComplete = null;
			CraftChanged = true;
			actor.OutputHandler.Send("This craft no longer uses a prog upon finish.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} does not comply.");
			return false;
		}

		OnUseProgComplete = prog;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft now executes the prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} when finished.");
		return true;
	}

	private bool BuildingCommandOnStartProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to execute when the craft is started?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			OnUseProgStart = null;
			CraftChanged = true;
			actor.OutputHandler.Send("This craft no longer uses a prog upon start up.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} does not comply.");
			return false;
		}

		OnUseProgStart = prog;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft now executes the prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} when begun.");
		return true;
	}

	private bool BuildingCommandWhyCannotUseProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use for sending a message when people can't use this craft?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Text)
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns text. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} instead returns {prog.ReturnType.Describe().Colour(Telnet.Cyan)}.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} does not comply.");
			return false;
		}

		WhyCannotUseProg = prog;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft now uses the prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} to get an error message when the craft can't be used.");
		return true;
	}

	private bool BuildingCommandCanUseProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use for determining whether a character can use this craft?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns boolean. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} instead returns {prog.ReturnType.Describe().Colour(Telnet.Cyan)}.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} does not comply.");
			return false;
		}

		CanUseProg = prog;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft now uses the prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} to determine whether characters can use it.");
		return true;
	}

	private bool BuildingCommandAppear(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use for determining whether this craft should appear in people's craft lists?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns boolean. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} instead returns {prog.ReturnType.Describe().Colour(Telnet.Cyan)}.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter. The prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} does not comply.");
			return false;
		}

		AppearInCraftsListProg = prog;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"This craft now uses the prog {$"{prog.FunctionName}".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} to determine whether it appears in character craft lists.");
		return true;
	}

	private bool BuildingCommandCheck(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"The syntax for this building sub command is check <free check rolls for improvement> <trait> <difficulty>, e.g. check 3 blacksmithing easy.");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var checks) || checks < 0)
		{
			actor.OutputHandler.Send("You must specify a number of free skill check rolls for improvement.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"The syntax for this building sub command is check <free check rolls for improvement> <trait> <difficulty>, e.g. check 3 blacksmithing easy.");
			return false;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.Last);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"The syntax for this building sub command is check <free check rolls for improvement> <trait> <difficulty>, e.g. check 3 blacksmithing easy.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		CheckTrait = trait;
		CheckDifficulty = difficulty;
		FreeSkillChecks = checks;
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"That craft will now give {FreeSkillChecks.ToString("N0", actor).Colour(Telnet.Green)} free skill checks, and uses the trait {trait.Name.Colour(Telnet.Green)} at a difficulty of {CheckDifficulty.Describe().Colour(Telnet.Green)}");
		return true;
	}

	private bool BuildingCommandInterruptable(ICharacter actor, StringStack command)
	{
		Interruptable = !Interruptable;
		CraftChanged = true;
		actor.OutputHandler.Send($"This craft is {(Interruptable ? "now" : "no longer")} interruptable.");
		return true;
	}

	private bool BuildingCommandAction(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What do you want the action description for this craft to be? Note, it should be a gerund like {"milking a cow".Colour(Telnet.BoldCyan)}.");
			return false;
		}

		ActionDescription = command.SafeRemainingArgument.ToLowerInvariant();
		CraftChanged = true;
		actor.OutputHandler.Send(
			$"That craft now has the action description {ActionDescription.Colour(Telnet.BoldCyan)}.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want the blurb for this craft to be?");
			return false;
		}

		Blurb = command.SafeRemainingArgument.ToLowerInvariant();
		CraftChanged = true;
		actor.OutputHandler.Send($"That craft now has the blurb {Blurb.Colour(Telnet.Yellow)}.");
		return true;
	}

	private bool BuildingCommandCategory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What category do you want this craft to be in?");
			return false;
		}

		Category = command.SafeRemainingArgument.ToLowerInvariant().Proper();
		CraftChanged = true;
		actor.OutputHandler.Send($"That craft is now in the {Category.Colour(Telnet.Cyan)} category.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What name do you want this craft to go by?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.Crafts.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.Send("There is already a different craft going by that name. Craft names must be unique.");
			return false;
		}

		_name = name.ToLowerInvariant();
		CraftChanged = true;
		actor.OutputHandler.Send($"That craft is now named {_name.Colour(Telnet.Cyan)}.");
		return true;
	}

	#endregion

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(EditHeader().GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();

		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Name: {Name.Colour(Telnet.Green)}",
			$"Category: {Category.Colour(Telnet.Green)}",
			$"Valid: {(_craftIsValid ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}"
		);
		sb.AppendLine($"Blurb: {Blurb.ColourCommand()}");
		sb.AppendLine($"Status: {Status.DescribeEnum(true).ColourValue()}");
		sb.AppendLine($"Action: {ActionDescription.Colour(Telnet.Green)}");
		sb.AppendLine($"In-Progress Item Desc: {ActiveCraftItemSDesc.Colour(Telnet.Green)}");
		sb.AppendLine();
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Appear Prog: {(AppearInCraftsListProg != null ? $"{AppearInCraftsListProg.FunctionName} (#{AppearInCraftsListProg.Id})".FluentTagMXP("send", $"href='show futureprog {AppearInCraftsListProg.Id}'") : "None".Colour(Telnet.Red))}",
			$"CanUse Prog: {(CanUseProg != null ? $"{CanUseProg.FunctionName} (#{CanUseProg.Id})".FluentTagMXP("send", $"href='show futureprog {CanUseProg.Id}'") : "None".Colour(Telnet.Red))}",
			$"WhyCantUse Prog: {(WhyCannotUseProg != null ? $"{WhyCannotUseProg.FunctionName} (#{WhyCannotUseProg.Id})".FluentTagMXP("send", $"href='show futureprog {WhyCannotUseProg.Id}'") : "None".Colour(Telnet.Red))}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"OnStart Prog: {(OnUseProgStart != null ? $"{OnUseProgStart.FunctionName} (#{OnUseProgStart.Id})".FluentTagMXP("send", $"href='show futureprog {OnUseProgStart.Id}'") : "None".Colour(Telnet.Red))}",
			$"OnCancel Prog: {(OnUseProgCancel != null ? $"{OnUseProgCancel.FunctionName} (#{OnUseProgCancel.Id})".FluentTagMXP("send", $"href='show futureprog {OnUseProgCancel.Id}'") : "None".Colour(Telnet.Red))}",
			$"OnFinish Prog: {(OnUseProgComplete != null ? $"{OnUseProgComplete.FunctionName} (#{OnUseProgComplete.Id})".FluentTagMXP("send", $"href='show futureprog {OnUseProgComplete.Id}'") : "None".Colour(Telnet.Red))}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Trait: {CheckTrait?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
			$"Difficulty: {CheckDifficulty.Describe().Colour(Telnet.Cyan)}",
			$"Threshold: {FailThreshold.DescribeColour()}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Free Checks: {FreeSkillChecks.ToString("N0", actor).Colour(Telnet.Green)}",
			$"Fail Phase: {FailPhase.ToString("N0", actor).Colour(Telnet.Green)}",
			$"Interruptable: {Interruptable.ToString(actor).Colour(Telnet.Green)}"
		);
		sb.AppendLine($"Check Type: {(IsPracticalCheck ? "Practical" : "Theoretical").ColourValue()}");
		sb.AppendLine($"Quality Formula: {QualityFormula.OriginalFormulaText.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Phases".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		var sb2 = new StringBuilder();
		for (var i = 0; i < _phaseEchoes.Count; i++)
		{
			sb2.AppendLine();
			if (_phaseEchoes[i].EqualTo(_failPhaseEchoes[i]))
			{
				sb2.AppendLine(
					$"\t{i + 1}) {_phaseLengths[i].Describe().Colour(Telnet.Green)} -> {$"\"{_phaseEchoes[i]}\"".Colour(Telnet.Yellow)}");
			}
			else
			{
				sb2.AppendLine(
					$"\t{i + 1}) {_phaseLengths[i].Describe().Colour(Telnet.Green)} -> {$"\"{_phaseEchoes[i]}\"".Colour(Telnet.Yellow)}\n\n\t{new string(' ',$"{i + 1}) {_phaseLengths[i].Describe()}".RawTextLength() - 4)}{"fail".ColourError()} -> {$"\"{_failPhaseEchoes[i]}\"".Colour(Telnet.Yellow)}");
			}
		}

		sb.Append(sb2.ToString().Wrap(actor.InnerLineFormatLength));

		sb.AppendLine();
		sb.AppendLine("Inputs".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		var index = 0;
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _orderedInputs
			let i = ++index
			select new List<string>
			{
				$"$i{i}",
				item.HowSeen(actor),
				item.InputType,
				_craftInputConsumedPhases[item.Id].ToString("N0", actor)
			},
			[
				"Name",
				"Description",
				"Type",
				"Consumed Phase"
			],
			actor,
			Telnet.Yellow
		));

		sb.AppendLine();
		sb.AppendLine("Tools".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		index = 0;
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _orderedTools
			let i = ++index
			select new List<string>
			{
				$"$t{i}",
				item.HowSeen(actor),
				$"{item.DesiredState.Describe()}{(item.UseToolDuration ? ", UseTool" : "")}",
				item.ToolType,
				_craftToolUsagePhases
					.Where(x => x.Value.Contains(item.Id))
					.Select(x => x.Key.ToString("N0", actor))
					.Concat(
						_craftToolUsageFailPhases
							.Where(x => x.Value.Contains(item.Id))
							.Where(x => x.Key >= FailPhase)
							.Select(x => $"F{x.Key.ToString("N0", actor)}")
					)
					.ListToCommaSeparatedValues(", ")
			},
			[
				"Name",
				"Description",
				"Rules",
				"Type",
				"Used Phases"
			],
			actor,
			Telnet.Yellow
		));

		sb.AppendLine();
		sb.AppendLine("Products".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		index = 0;
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _orderedProducts
			let i = ++index
			select new List<string>
			{
				$"$p{i}",
				item.HowSeen(actor),
				item.ProductType,
				_craftProductProducedPhases[item.Id].ToString("N0", actor)
			},
			[
				"Name",
				"Description",
				"Type",
				"Produced Phase"
			],
			actor,
			Telnet.Yellow
		));

		sb.AppendLine();
		sb.AppendLine("Fail Products".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		index = 0;
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _orderedFailProducts
			let i = ++index
			select new List<string>
			{
				$"$f{i}",
				item.HowSeen(actor),
				item.ProductType,
				_craftFailProductProducedPhases[item.Id].ToString("N0", actor)
			},
			[
				"Name",
				"Description",
				"Type",
				"Produced Phase"
			],
			actor,
			Telnet.Yellow
		));

		if (!_craftIsValid)
		{
			sb.AppendLine();
			sb.AppendLine("Errors".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
			sb.AppendLine();
			if (FailPhase <= 0 || FailPhase > LastPhase)
			{
				sb.AppendLine("Error: You must set a valid fail phase.".Colour(Telnet.Red));
			}

			if (!PhaseEchoes.Any())
			{
				sb.AppendLine("Error: There must be at least one phase for every craft.".Colour(Telnet.Red));
			}

			if (AppearInCraftsListProg == null)
			{
				sb.AppendLine("Error: You must set an appear prog for every craft.".Colour(Telnet.Red));
			}

			foreach (var (echo, phase, fail) in _invalidEchoReferences)
			{
				sb.AppendLine($"Error: Invalid reference {echo} in {(fail ? "fail echo" : "echo")} for phase {phase}"
					.Colour(Telnet.Red));
			}

			foreach (var item in Inputs.Where(x => !x.IsValid()))
			{
				sb.AppendLine(
					$"Error: Input {_orderedInputs.IndexOf(item) + 1}: {item.WhyNotValid()}".Colour(Telnet.Red));
			}

			foreach (var item in Tools.Where(x => !x.IsValid()))
			{
				sb.AppendLine(
					$"Error: Tool {_orderedTools.IndexOf(item) + 1}: {item.WhyNotValid()}".Colour(Telnet.Red));
			}

			foreach (var item in Products.Where(x => !x.IsValid()))
			{
				sb.AppendLine(
					$"Error: Product {_orderedProducts.IndexOf(item) + 1}: {item.WhyNotValid()}".Colour(Telnet.Red));
			}

			foreach (var item in FailProducts.Where(x => !x.IsValid()))
			{
				sb.AppendLine($"Error: Fail Product {_orderedFailProducts.IndexOf(item) + 1}: {item.WhyNotValid()}"
					.Colour(Telnet.Red));
			}
		}

		return sb.ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbnew = new Models.Craft();
			FMDB.Context.Crafts.Add(dbnew);
			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

			dbnew.Id = Id;
			dbnew.RevisionNumber = FMDB.Context.Crafts.Where(x => x.Id == Id).Select(x => x.RevisionNumber)
			                           .AsEnumerable().DefaultIfEmpty(0).Max() + 1;
			dbnew.Name = Name;
			dbnew.Blurb = Blurb;
			dbnew.ActionDescription = ActionDescription;
			dbnew.ActiveCraftItemSdesc = ActiveCraftItemSDesc;
			dbnew.Category = Category;
			dbnew.Interruptable = Interruptable;
			dbnew.ToolQualityWeighting = ToolQualityWeighting;
			dbnew.InputQualityWeighting = InputQualityWeighting;
			dbnew.CheckQualityWeighting = CheckQualityWeighting;
			dbnew.FreeSkillChecks = FreeSkillChecks;
			dbnew.FailPhase = FailPhase;
			dbnew.FailThreshold = (int)FailThreshold;
			dbnew.CheckDifficulty = (int)CheckDifficulty;
			dbnew.CheckTraitId = CheckTrait?.Id;
			dbnew.QualityFormula = QualityFormula.OriginalFormulaText;
			dbnew.AppearInCraftsListProgId = AppearInCraftsListProg?.Id;
			dbnew.CanUseProgId = CanUseProg?.Id;
			dbnew.WhyCannotUseProgId = WhyCannotUseProg?.Id;
			dbnew.OnUseProgStartId = OnUseProgStart?.Id;
			dbnew.OnUseProgCompleteId = OnUseProgComplete?.Id;
			dbnew.OnUseProgCancelId = OnUseProgCancel?.Id;

			for (var i = 0; i < _phaseLengths.Count; i++)
			{
				var dbphase = new CraftPhase();
				dbnew.CraftPhases.Add(dbphase);
				dbphase.Echo = _phaseEchoes[i];
				dbphase.FailEcho = _failPhaseEchoes[i];
				dbphase.PhaseLengthInSeconds = _phaseLengths[i].TotalSeconds;
				dbphase.PhaseNumber = i + 1;
			}

			var inputMap = new Dictionary<long, Models.CraftInput>();
			foreach (var input in _orderedInputs)
			{
				inputMap[input.Id] = input.CreateNewRevision(dbnew);
			}

			var toolMap = new Dictionary<long, Models.CraftTool>();
			foreach (var tool in _orderedTools)
			{
				toolMap[tool.Id] = tool.CreateNewRevision(dbnew);
			}
			FMDB.Context.SaveChanges();

			foreach (var product in _orderedProducts)
			{
				product.CreateNewRevision(dbnew, false, inputMap.ToDictionary(x => x.Key, x => x.Value.Id), toolMap.ToDictionary(x => x.Key, x => x.Value.Id));
			}

			foreach (var product in _orderedFailProducts)
			{
				product.CreateNewRevision(dbnew, true, inputMap.ToDictionary(x => x.Key, x => x.Value.Id), toolMap.ToDictionary(x => x.Key, x => x.Value.Id));
			}

			FMDB.Context.SaveChanges();
			return new Craft(dbnew, Gameworld);
		}
	}

	public override string EditHeader()
	{
		return "Craft #" + Id + " Revision " + RevisionNumber;
	}

	public override bool CanSubmit()
	{
		return _craftIsValid;
	}

	public override string WhyCannotSubmit()
	{
		return "There are problems with this craft. See the errors at the bottom of CRAFT SHOW for more information.";
	}

	#endregion
}