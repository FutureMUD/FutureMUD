param(
	[string]$BaselineProfile = 'stock',
	[string]$ComparisonProfile = 'combat-rebalance'
)

$ErrorActionPreference = "Stop"

$baseAttribute = 11.5
$degree = 5
$sqrtFactor = [Math]::Sqrt($degree + 1)

$creatures = @(
	[pscustomobject]@{
		Name = "Mouse"; Group = "Animal"; Attack = "Small Bite"; Quality = 1; Coefficient = 0.5; QualityMultiplier = 2;
		BeforeStrengthBonus = 0; AfterStrengthBonus = -10; BeforeConstitutionBonus = 0; AfterConstitutionBonus = -10;
		BeforeBodypartMultiplier = 0.1; AfterBodypartMultiplier = 0.1
	},
	[pscustomobject]@{
		Name = "Cat"; Group = "Animal"; Attack = "Claw Swipe"; Quality = 2; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = -3; BeforeConstitutionBonus = 0; AfterConstitutionBonus = -4;
		BeforeBodypartMultiplier = 0.5; AfterBodypartMultiplier = 0.5
	},
	[pscustomobject]@{
		Name = "Wolf"; Group = "Animal"; Attack = "Carnivore Bite"; Quality = 5; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 5; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 4;
		BeforeBodypartMultiplier = 1.0; AfterBodypartMultiplier = 1.0
	},
	[pscustomobject]@{
		Name = "Bear"; Group = "Animal"; Attack = "Claw Swipe"; Quality = 7; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 12; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 13;
		BeforeBodypartMultiplier = 1.4; AfterBodypartMultiplier = 1.4
	},
	[pscustomobject]@{
		Name = "Hippopotamus"; Group = "Animal"; Attack = "Carnivore Bite"; Quality = 8; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 13; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 14;
		BeforeBodypartMultiplier = 1.5; AfterBodypartMultiplier = 1.5
	},
	[pscustomobject]@{
		Name = "Elephant"; Group = "Animal"; Attack = "Tusk Gore"; Quality = 8; Coefficient = 0.8; QualityMultiplier = 2;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 18; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 18;
		BeforeBodypartMultiplier = 2.0; AfterBodypartMultiplier = 2.0
	},
	[pscustomobject]@{
		Name = "Orca"; Group = "Animal"; Attack = "Shark Bite"; Quality = 10; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 15; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 14;
		BeforeBodypartMultiplier = 1.6; AfterBodypartMultiplier = 1.6
	},
	[pscustomobject]@{
		Name = "Cockatrice"; Group = "Mythic"; Attack = "Beak Peck"; Quality = 5; Coefficient = 0.45; QualityMultiplier = 2;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 0; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 0;
		BeforeBodypartMultiplier = 1.0; AfterBodypartMultiplier = 0.7
	},
	[pscustomobject]@{
		Name = "Griffin"; Group = "Mythic"; Attack = "Claw Swipe"; Quality = 6; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 7; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 6;
		BeforeBodypartMultiplier = 1.0; AfterBodypartMultiplier = 1.6
	},
	[pscustomobject]@{
		Name = "Wyvern"; Group = "Mythic"; Attack = "Carnivore Bite"; Quality = 6; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 8; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 6;
		BeforeBodypartMultiplier = 1.0; AfterBodypartMultiplier = 1.7
	},
	[pscustomobject]@{
		Name = "Dragon"; Group = "Mythic"; Attack = "Carnivore Bite"; Quality = 11; Coefficient = 1.0; QualityMultiplier = 3;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 12; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 11;
		BeforeBodypartMultiplier = 1.0; AfterBodypartMultiplier = 2.4
	},
	[pscustomobject]@{
		Name = "Centaur"; Group = "Mythic"; Attack = "Hoof Stomp"; Quality = 6; Coefficient = 0.8; QualityMultiplier = 2;
		BeforeStrengthBonus = 0; AfterStrengthBonus = 6; BeforeConstitutionBonus = 0; AfterConstitutionBonus = 5;
		BeforeBodypartMultiplier = 1.0; AfterBodypartMultiplier = 1.5
	}
)

function Get-Damage([double]$strength, [double]$coefficient, [int]$qualityMultiplier, [int]$quality) {
	return $coefficient * ($strength + ($qualityMultiplier * $quality)) * $sqrtFactor
}

function Get-ScaledHp([double]$constitution, [double]$bodypartMultiplier) {
	return (100.0 + $constitution) * $bodypartMultiplier
}

$results = foreach ($creature in $creatures) {
	$beforeStrength = $baseAttribute + $creature.BeforeStrengthBonus
	$afterStrength = $baseAttribute + $creature.AfterStrengthBonus
	$beforeConstitution = $baseAttribute + $creature.BeforeConstitutionBonus
	$afterConstitution = $baseAttribute + $creature.AfterConstitutionBonus

	$beforeDamage = Get-Damage -strength $beforeStrength -coefficient $creature.Coefficient -qualityMultiplier $creature.QualityMultiplier -quality $creature.Quality
	$afterDamage = Get-Damage -strength $afterStrength -coefficient $creature.Coefficient -qualityMultiplier $creature.QualityMultiplier -quality $creature.Quality
	$beforeScaledHp = Get-ScaledHp -constitution $beforeConstitution -bodypartMultiplier $creature.BeforeBodypartMultiplier
	$afterScaledHp = Get-ScaledHp -constitution $afterConstitution -bodypartMultiplier $creature.AfterBodypartMultiplier

	[pscustomobject]@{
		Group = $creature.Group
		Name = $creature.Name
		Attack = $creature.Attack
		AvgStrengthBefore = [Math]::Round($beforeStrength, 1)
		AvgStrengthAfter = [Math]::Round($afterStrength, 1)
		DamageBefore = [Math]::Round($beforeDamage, 1)
		DamageAfter = [Math]::Round($afterDamage, 1)
		BodypartScaleBefore = [Math]::Round($beforeScaledHp, 1)
		BodypartScaleAfter = [Math]::Round($afterScaledHp, 1)
		DamageRatio = [Math]::Round($afterDamage / [Math]::Max(0.1, $beforeDamage), 2)
		HpRatio = [Math]::Round($afterScaledHp / [Math]::Max(0.1, $beforeScaledHp), 2)
	}
}

Write-Host "# Non-Human Attribute and Bodypart Profile Validation"
Write-Host ""
Write-Host "Assumptions: average base attribute 11.5 from 3d6+1, degree 5, deterministic attack expressions, bodypart scale approximated as (100 + Constitution) * BodypartHealthMultiplier. Comparing profiles '$BaselineProfile' and '$ComparisonProfile'."
Write-Host ""

foreach ($group in "Animal", "Mythic") {
	Write-Host "## $group"
	$results |
		Where-Object Group -eq $group |
		Select-Object Name,
			Attack,
			@{ Name = "$BaselineProfile Strength"; Expression = { $_.AvgStrengthBefore } },
			@{ Name = "$ComparisonProfile Strength"; Expression = { $_.AvgStrengthAfter } },
			@{ Name = "$BaselineProfile Damage"; Expression = { $_.DamageBefore } },
			@{ Name = "$ComparisonProfile Damage"; Expression = { $_.DamageAfter } },
			@{ Name = "$BaselineProfile HP Scale"; Expression = { $_.BodypartScaleBefore } },
			@{ Name = "$ComparisonProfile HP Scale"; Expression = { $_.BodypartScaleAfter } } |
		Format-Table -AutoSize
	Write-Host ""
}

$mouse = $results | Where-Object Name -eq "Mouse"
$hippo = $results | Where-Object Name -eq "Hippopotamus"
$dragon = $results | Where-Object Name -eq "Dragon"
$wolf = $results | Where-Object Name -eq "Wolf"

Write-Host "## Headline checks"
Write-Host ("Mouse average bite damage: {0} {1}, {2} {3}" -f $BaselineProfile, $mouse.DamageBefore, $ComparisonProfile, $mouse.DamageAfter)
Write-Host ("Wolf average bite damage: {0} {1}, {2} {3}" -f $BaselineProfile, $wolf.DamageBefore, $ComparisonProfile, $wolf.DamageAfter)
Write-Host ("Hippo average bite damage: {0} {1}, {2} {3}" -f $BaselineProfile, $hippo.DamageBefore, $ComparisonProfile, $hippo.DamageAfter)
Write-Host ("Dragon average bite damage: {0} {1}, {2} {3}" -f $BaselineProfile, $dragon.DamageBefore, $ComparisonProfile, $dragon.DamageAfter)
Write-Host ("Hippo vs mouse damage ratio in {0}: {1}x" -f $ComparisonProfile, [Math]::Round($hippo.DamageAfter / $mouse.DamageAfter, 1))
Write-Host ("Dragon vs mouse damage ratio in {0}: {1}x" -f $ComparisonProfile, [Math]::Round($dragon.DamageAfter / $mouse.DamageAfter, 1))
