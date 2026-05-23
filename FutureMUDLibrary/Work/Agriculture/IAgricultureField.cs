using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureField : IFrameworkItem, ISaveable, IHaveFuturemud, IProgVariable
{
	ICell Cell { get; }
	IAgricultureFieldProfile Profile { get; set; }
	AgricultureFieldUse CurrentUse { get; }
	IAgricultureCropDefinition CurrentCrop { get; }
	AgricultureCropStage CropStage { get; }
	int CropGrowthDays { get; }
	int CropHarvestCount { get; }
	int CropHealth { get; }
	int CropYieldPotential { get; }
	IAgricultureWoodlandDefinition CurrentWoodland { get; }
	int WoodlandGrowthDays { get; }
	int WoodlandHealth { get; }
	int WoodlandYieldPotential { get; }
	IAgricultureFieldApiary Apiary { get; }
	bool HasActiveApiary { get; }
	bool IsApiaryHappy { get; }
	int PollinationStrength { get; }
	IEnumerable<IAgricultureFieldHerd> Herds { get; }
	int Moisture { get; set; }
	int Drainage { get; set; }
	int Nutrients { get; set; }
	int Salinity { get; set; }
	int Topsoil { get; set; }
	int Tilth { get; set; }
	int Rockiness { get; set; }
	int Weeds { get; set; }
	int Pests { get; set; }
	int Fence { get; set; }
	int Pasture { get; set; }
	int Condition { get; set; }
	int Score(AgricultureScoreType score);
	void SetScore(AgricultureScoreType score, int value);
	void AdjustScore(AgricultureScoreType score, int delta);
	string DescribeTo(ICharacter voyeur, bool exact);
	void DailyTick();
	bool CanBeginOperation(ICharacter actor, IAgricultureOperation operation, IFrameworkItem target, out string reason);
	bool ApplyOperation(IAgricultureOperation operation, IFrameworkItem target, ICharacter actor, out string result);
	bool ApplyOperation(IAgricultureOperation operation, IFrameworkItem target, ICharacter actor, bool enforceActorAccess, out string result);
	bool ApplyOperation(IAgricultureOperation operation, IFrameworkItem target, ICharacter actor, bool enforceActorAccess,
		AgricultureWorkOutcome outcome, out string result);
	bool ConsumeCropYield(int amount, out string reason);
	bool ConsumeWoodlandYield(int amount, out string reason);
	bool DrawDownHerd(IAgricultureHerdDefinition definition, int count, ICharacter actor, out string result);
	bool AbsorbNpcIntoHerd(ICharacter npc, IAgricultureHerdDefinition definition, ICharacter actor, out string result);
	bool DriveHerdTo(IAgricultureField destination, IAgricultureHerdDefinition definition, int count, ICharacter actor, out string result);
}

public interface IAgricultureFieldHerd : IFrameworkItem
{
	IAgricultureHerdDefinition Definition { get; }
	int HeadCount { get; }
	double Condition { get; }
	int SecondaryYieldPotential { get; }
}

public interface IAgricultureFieldApiary
{
	int HiveCount { get; }
	int ColonyHealth { get; }
	int Stores { get; }
	int YieldPotential { get; }
	int PollinationRadius { get; }
	int PollinationStrength { get; }
}
