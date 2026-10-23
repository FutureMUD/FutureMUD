#nullable enable

namespace DatabaseSeeder.Seeders;

public static partial class IndustrialFoodResearchStock
{
	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0001() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 2,
		StableReference: "evidence_staples",
		EvidenceClass: "candidate package",
		Source: "NUT;PRICE;TRADE",
		ComparableFamily: "staples",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0002() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 3,
		StableReference: "evidence_baking",
		EvidenceClass: "candidate package",
		Source: "BAKE;NUT;PRICE",
		ComparableFamily: "baking",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0003() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 4,
		StableReference: "evidence_noodles-and-dumplings",
		EvidenceClass: "candidate package",
		Source: "GLOBAL;NUT;PRICE",
		ComparableFamily: "noodles-and-dumplings",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0004() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 5,
		StableReference: "evidence_plant-foods",
		EvidenceClass: "candidate package",
		Source: "GLOBAL;NUT;PRICE",
		ComparableFamily: "plant-foods",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0005() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 6,
		StableReference: "evidence_fruit-and-nuts",
		EvidenceClass: "candidate package",
		Source: "GLOBAL;NUT;PRICE",
		ComparableFamily: "fruit-and-nuts",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0006() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 7,
		StableReference: "evidence_dairy-and-eggs",
		EvidenceClass: "candidate package",
		Source: "DAIRY;NUT;PRICE",
		ComparableFamily: "dairy-and-eggs",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0007() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 8,
		StableReference: "evidence_meat-and-offal",
		EvidenceClass: "candidate package",
		Source: "MEAT;NUT;PRICE",
		ComparableFamily: "meat-and-offal",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0008() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 9,
		StableReference: "evidence_aquatic-foods",
		EvidenceClass: "candidate package",
		Source: "AQUATIC;NUT;PRICE",
		ComparableFamily: "aquatic-foods",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0009() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 10,
		StableReference: "evidence_composite-dishes",
		EvidenceClass: "candidate package",
		Source: "GLOBAL;NUT;PRICE",
		ComparableFamily: "composite-dishes",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0010() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 11,
		StableReference: "evidence_preserves-and-condiments",
		EvidenceClass: "candidate package",
		Source: "PRESERVE;NUT;PRICE",
		ComparableFamily: "preserves-and-condiments",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0011() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 12,
		StableReference: "evidence_processed-intermediates",
		EvidenceClass: "candidate package",
		Source: "PROCESS;NUT;PRICE",
		ComparableFamily: "processed-intermediates",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0012() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 13,
		StableReference: "evidence_desserts",
		EvidenceClass: "candidate package",
		Source: "SWEET;NUT;PRICE",
		ComparableFamily: "desserts",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0013() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 14,
		StableReference: "evidence_snacks",
		EvidenceClass: "candidate package",
		Source: "GLOBAL;NUT;PRICE",
		ComparableFamily: "snacks",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0014() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 15,
		StableReference: "evidence_rations",
		EvidenceClass: "candidate package",
		Source: "RATION;NUT;PRICE",
		ComparableFamily: "rations",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0015() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 16,
		StableReference: "evidence_infant-and-dietetic",
		EvidenceClass: "candidate package",
		Source: "NUT;CARE;PRICE",
		ComparableFamily: "infant-and-dietetic",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0016() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 17,
		StableReference: "evidence_processed-animal-feeds",
		EvidenceClass: "candidate package",
		Source: "FEED;NUT;PRICE",
		ComparableFamily: "processed-animal-feeds",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0017() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 18,
		StableReference: "evidence_non-alcoholic-drinks",
		EvidenceClass: "candidate package",
		Source: "BEVERAGE;NUT;PRICE",
		ComparableFamily: "non-alcoholic-drinks",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0018() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 19,
		StableReference: "evidence_stimulant-infusions",
		EvidenceClass: "candidate package",
		Source: "STIM;TRADE;PRICE",
		ComparableFamily: "stimulant-infusions",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0019() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 20,
		StableReference: "evidence_alcoholic-drinks",
		EvidenceClass: "candidate package",
		Source: "ALCOHOL;TRADE;PRICE",
		ComparableFamily: "alcoholic-drinks",
		ReviewState: "DependencyReviewed"
	);

	public static CatalogueFoodEvidenceCandidatesResearchRow EvidenceCandidates0020() => new(
		SourceFile: "Food/evidence-candidates.tsv", SourceLine: 21,
		StableReference: "evidence_beverage-intermediates",
		EvidenceClass: "candidate package",
		Source: "BEVERAGE;PROCESS;PRICE",
		ComparableFamily: "beverage-intermediates",
		ReviewState: "DependencyReviewed"
	);
}
