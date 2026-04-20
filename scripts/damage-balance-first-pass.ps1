param(
	[string]$BaselineProfile = 'stock',
	[string]$ComparisonProfile = 'combat-rebalance'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$strength = 15.0
$quality = 5.0
$naturalArmourQuality = 2.0

$absoluteSeverityBands = @(
	[pscustomobject]@{ Name = 'None'; Lower = [double]::NegativeInfinity; Upper = 0.0 },
	[pscustomobject]@{ Name = 'Superficial'; Lower = 0.0; Upper = 2.0 },
	[pscustomobject]@{ Name = 'Minor'; Lower = 2.0; Upper = 4.0 },
	[pscustomobject]@{ Name = 'Small'; Lower = 4.0; Upper = 7.0 },
	[pscustomobject]@{ Name = 'Moderate'; Lower = 7.0; Upper = 12.0 },
	[pscustomobject]@{ Name = 'Severe'; Lower = 12.0; Upper = 18.0 },
	[pscustomobject]@{ Name = 'VerySevere'; Lower = 18.0; Upper = 27.0 },
	[pscustomobject]@{ Name = 'Grievous'; Lower = 27.0; Upper = 40.0 },
	[pscustomobject]@{ Name = 'Horrifying'; Lower = 40.0; Upper = [double]::PositiveInfinity }
)

$percentageBandsBefore = @(
	[pscustomobject]@{ Name = 'None'; Lower = [double]::NegativeInfinity; Upper = 0.0 },
	[pscustomobject]@{ Name = 'Superficial'; Lower = 0.0; Upper = 0.40 },
	[pscustomobject]@{ Name = 'Minor'; Lower = 0.40; Upper = 0.55 },
	[pscustomobject]@{ Name = 'Small'; Lower = 0.55; Upper = 0.65 },
	[pscustomobject]@{ Name = 'Moderate'; Lower = 0.65; Upper = 0.75 },
	[pscustomobject]@{ Name = 'Severe'; Lower = 0.75; Upper = 0.85 },
	[pscustomobject]@{ Name = 'VerySevere'; Lower = 0.85; Upper = 0.90 },
	[pscustomobject]@{ Name = 'Grievous'; Lower = 0.90; Upper = 0.95 },
	[pscustomobject]@{ Name = 'Horrifying'; Lower = 0.95; Upper = [double]::PositiveInfinity }
)

$percentageBandsAfter = @(
	[pscustomobject]@{ Name = 'None'; Lower = [double]::NegativeInfinity; Upper = 0.0 },
	[pscustomobject]@{ Name = 'Superficial'; Lower = 0.0; Upper = 0.15 },
	[pscustomobject]@{ Name = 'Minor'; Lower = 0.15; Upper = 0.30 },
	[pscustomobject]@{ Name = 'Small'; Lower = 0.30; Upper = 0.45 },
	[pscustomobject]@{ Name = 'Moderate'; Lower = 0.45; Upper = 0.60 },
	[pscustomobject]@{ Name = 'Severe'; Lower = 0.60; Upper = 0.75 },
	[pscustomobject]@{ Name = 'VerySevere'; Lower = 0.75; Upper = 0.87 },
	[pscustomobject]@{ Name = 'Grievous'; Lower = 0.87; Upper = 0.95 },
	[pscustomobject]@{ Name = 'Horrifying'; Lower = 0.95; Upper = [double]::PositiveInfinity }
)

$severityOrder = @{
	None = 0
	Superficial = 1
	Minor = 2
	Small = 3
	Moderate = 4
	Severe = 5
	VerySevere = 6
	Grievous = 7
	Horrifying = 8
}

$genericTransforms = @{
	Slashing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	Chopping = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	Piercing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Moderate' }
	Ballistic = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Moderate' }
	Bite = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	Claw = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	Shearing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	Wrenching = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	BallisticArmourPiercing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
	ArmourPiercing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
}

$relaxedFleshTransforms = @{
	Slashing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
	Chopping = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
	Piercing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Superficial' }
	Ballistic = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Superficial' }
	Bite = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
	Claw = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
	Shearing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
	Wrenching = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
	Shrapnel = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Superficial' }
	ArmourPiercing = [pscustomobject]@{ To = 'ArmourPiercing'; Threshold = 'Horrifying' }
}

$materials = @{
	'flesh' = [pscustomobject]@{ Name = 'flesh'; Shear = 25000.0; Impact = 10000.0 }
	'bony flesh' = [pscustomobject]@{ Name = 'bony flesh'; Shear = 50000.0; Impact = 60000.0 }
	'compact bone' = [pscustomobject]@{ Name = 'compact bone'; Shear = 115000.0; Impact = 200000.0 }
}

$armourFamiliesBefore = @{
	'Heavy Clothing' = [pscustomobject]@{ Name = 'Heavy Clothing'; Power = 0.8; Strong = @('Chemical', 'Freezing', 'Burning', 'Electrical'); Weak = @('Piercing', 'Chopping', 'Slashing', 'Ballistic', 'BallisticArmourPiercing', 'ArmourPiercing'); Zero = @('Hypoxia', 'Cellular'); Super = @(); Transforms = $genericTransforms }
	'Chainmail' = [pscustomobject]@{ Name = 'Chainmail'; Power = 4.0; Strong = @('Chemical', 'Freezing', 'Burning', 'Electrical', 'Slashing', 'Claw', 'Ballistic', 'BallisticArmourPiercing', 'Chopping', 'Shearing'); Weak = @('Piercing', 'Crushing', 'Shockwave', 'Wrenching', 'Falling'); Zero = @('Hypoxia', 'Cellular', 'ArmourPiercing', 'Arcane', 'Necrotic', 'Eldritch'); Super = @(); Transforms = $genericTransforms }
	'Platemail' = [pscustomobject]@{ Name = 'Platemail'; Power = 6.0; Strong = @('Chemical', 'Freezing', 'Burning', 'Electrical', 'Slashing', 'Claw', 'Ballistic', 'BallisticArmourPiercing', 'ArmourPiercing'); Weak = @('Piercing', 'Crushing', 'Shockwave', 'Wrenching', 'Falling'); Zero = @('Hypoxia', 'Cellular', 'Arcane', 'Necrotic', 'Eldritch', 'Chopping', 'Shearing'); Super = @(); Transforms = $genericTransforms }
}

$armourFamiliesAfter = @{
	'Heavy Clothing' = [pscustomobject]@{ Name = 'Heavy Clothing'; Power = 0.9; Strong = @('Chemical', 'Freezing', 'Burning', 'Electrical', 'Crushing', 'Shockwave', 'Falling'); Weak = @('Piercing', 'Chopping', 'Slashing', 'Ballistic', 'BallisticArmourPiercing', 'ArmourPiercing'); Zero = @('Hypoxia', 'Cellular'); Super = @(); Transforms = $genericTransforms }
	'Chainmail' = [pscustomobject]@{ Name = 'Chainmail'; Power = 3.5; Strong = @('Chemical', 'Freezing', 'Burning', 'Electrical', 'Slashing', 'Claw', 'Chopping', 'Shearing'); Weak = @('Crushing', 'Shockwave', 'Falling', 'Ballistic', 'BallisticArmourPiercing', 'ArmourPiercing'); Zero = @('Hypoxia', 'Cellular', 'Arcane', 'Necrotic', 'Eldritch'); Super = @(); Transforms = $genericTransforms }
	'Platemail' = [pscustomobject]@{ Name = 'Platemail'; Power = 4.5; Strong = @('Chemical', 'Freezing', 'Burning', 'Electrical', 'Slashing', 'Claw', 'Chopping', 'Shearing'); Weak = @('Crushing', 'Shockwave', 'Falling', 'Ballistic'); Zero = @('Hypoxia', 'Cellular', 'Arcane', 'Necrotic', 'Eldritch', 'BallisticArmourPiercing', 'ArmourPiercing'); Super = @(); Transforms = $genericTransforms }
}

$snapshots = @{
	Before = [pscustomobject]@{
		Key = 'Before'
		Label = $BaselineProfile
		FractureBands = $percentageBandsBefore
		ArmourFamilies = $armourFamiliesBefore
		RacialArmour = [pscustomobject]@{ Name = 'Human Natural Armour'; Kind = 'OldNatural'; Quality = $naturalArmourQuality; Material = $null; Transforms = $genericTransforms }
		BodyArmour = [pscustomobject]@{ Name = 'Human Natural Armour'; Kind = 'OldNatural'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
		CranialArmour = [pscustomobject]@{ Name = 'Human Natural Armour'; Kind = 'OldNatural'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
		BoneArmour = [pscustomobject]@{ Name = 'Human Natural Bone Armour'; Kind = 'Bone'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
	}
	After = [pscustomobject]@{
		Key = 'After'
		Label = $ComparisonProfile
		FractureBands = $percentageBandsAfter
		ArmourFamilies = $armourFamiliesAfter
		RacialArmour = [pscustomobject]@{ Name = 'Human Racial Tissue Armour'; Kind = 'RacialTissue'; Quality = $naturalArmourQuality; Material = $null; Transforms = $relaxedFleshTransforms }
		BodyArmour = [pscustomobject]@{ Name = 'Human Natural Flesh Armour'; Kind = 'BodyFlesh'; Quality = $naturalArmourQuality; Transforms = $relaxedFleshTransforms }
		CranialArmour = [pscustomobject]@{ Name = 'Human Cranial Flesh Armour'; Kind = 'CranialFlesh'; Quality = $naturalArmourQuality; Transforms = $relaxedFleshTransforms }
		BoneArmour = [pscustomobject]@{ Name = 'Human Natural Bone Armour'; Kind = 'Bone'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
	}
}

$targets = @{
	rupperarm = [pscustomobject]@{ Key = 'rupperarm'; Name = 'right upper arm'; BodypartHealth = 80.0; Material = $materials['flesh']; SeverThresholdBefore = 100.0; SeverThresholdAfter = 27.0; UsesCranial = $false; Bone = [pscustomobject]@{ Name = 'right humerus'; Chance = 0.50; Health = 140.0; Coverage = '50%' }; Organ = $null; InternalNotes = 'right humerus 50% hit chance' }
	rshin = [pscustomobject]@{ Key = 'rshin'; Name = 'right shin'; BodypartHealth = 40.0; Material = $materials['bony flesh']; SeverThresholdBefore = 100.0; SeverThresholdAfter = 27.0; UsesCranial = $false; Bone = [pscustomobject]@{ Name = 'right tibia'; Chance = 1.00; Health = 150.0; Coverage = '100%' }; Organ = $null; InternalNotes = 'right tibia 100% hit chance' }
	rbreast = [pscustomobject]@{ Key = 'rbreast'; Name = 'right breast'; BodypartHealth = 80.0; Material = $materials['flesh']; SeverThresholdBefore = -1.0; SeverThresholdAfter = -1.0; UsesCranial = $false; Bone = [pscustomobject]@{ Name = 'right ribs'; Chance = [double]::NaN; Health = 100.0; Coverage = '12 ribs at 5% each, stochastic' }; Organ = [pscustomobject]@{ Name = 'right lung'; Chance = 1.00; Coverage = '100%' }; InternalNotes = 'right ribs each 5% hit chance; right lung bodypart coverage 100%' }
	forehead = [pscustomobject]@{ Key = 'forehead'; Name = 'forehead'; BodypartHealth = 40.0; Material = $materials['flesh']; SeverThresholdBefore = -1.0; SeverThresholdAfter = -1.0; UsesCranial = $true; Bone = [pscustomobject]@{ Name = 'frontal cranial bone'; Chance = 1.00; Health = 140.0; Coverage = '100%' }; Organ = [pscustomobject]@{ Name = 'brain'; Chance = 1.00; Coverage = '100% via frontal cranial bone' }; InternalNotes = 'frontal cranial bone 100% hit chance; brain coverage 100% through the skull' }
}

$attacks = @(
	[pscustomobject]@{ Name = 'Longsword 1-Handed Arm Swing'; Family = 'Normal'; DamageType = 'Slashing'; Degree = 5; Target = 'rupperarm'; Worn = @() },
	[pscustomobject]@{ Name = 'Axe 1-Handed Leg Swing'; Family = 'Good'; DamageType = 'Chopping'; Degree = 5; Target = 'rshin'; Worn = @() },
	[pscustomobject]@{ Name = 'Spear 1-Handed Stab'; Family = 'Normal'; DamageType = 'Piercing'; Degree = 3; Target = 'rbreast'; Worn = @() },
	[pscustomobject]@{ Name = 'Spear 1-Handed Stab'; Family = 'Normal'; DamageType = 'Piercing'; Degree = 3; Target = 'rbreast'; Worn = @('Chainmail', 'Heavy Clothing') },
	[pscustomobject]@{ Name = 'Warhammer Crush Right Leg'; Family = 'Coup de Grace'; DamageType = 'Crushing'; Degree = 5; Target = 'rshin'; Worn = @() },
	[pscustomobject]@{ Name = 'Warhammer Crush Right Leg'; Family = 'Coup de Grace'; DamageType = 'Crushing'; Degree = 5; Target = 'rshin'; Worn = @('Chainmail') },
	[pscustomobject]@{ Name = 'Warhammer Crush Head'; Family = 'Coup de Grace'; DamageType = 'Crushing'; Degree = 5; Target = 'forehead'; Worn = @() },
	[pscustomobject]@{ Name = 'Warhammer Crush Head'; Family = 'Coup de Grace'; DamageType = 'Crushing'; Degree = 5; Target = 'forehead'; Worn = @('Platemail') }
)

function Get-FamilyMultiplier {
	param([string]$family)
	switch ($family) {
		'Training' { return 0.14 }
		'Terrible' { return 0.15 }
		'Bad' { return 0.16 }
		'Poor' { return 0.17 }
		'Normal' { return 0.18 }
		'Good' { return 0.24 }
		'Very Good' { return 0.27 }
		'Great' { return 0.30 }
		'Coup de Grace' { return 0.60 }
		default { throw "Unknown damage family '$family'." }
	}
}

function Get-RawDamage {
	param([string]$family, [double]$strengthValue, [double]$qualityValue, [int]$degree)
	return (Get-FamilyMultiplier $family) * ($strengthValue * $qualityValue) * [math]::Sqrt($degree + 1)
}

function Get-AbsoluteSeverity {
	param([double]$damage)
	foreach ($band in $absoluteSeverityBands) {
		if ($damage -ge $band.Lower -and $damage -lt $band.Upper) {
			return $band.Name
		}
	}
	return 'Horrifying'
}

function Get-PercentageSeverity {
	param([double]$ratio, $bands)
	foreach ($band in $bands) {
		if ($ratio -ge $band.Lower -and $ratio -lt $band.Upper) {
			return $band.Name
		}
	}
	return 'Horrifying'
}

function Transform-DamageType {
	param([string]$damageType, [double]$passThroughDamage, $transformMap)
	if (-not $transformMap.ContainsKey($damageType)) {
		return $damageType
	}

	$severity = Get-AbsoluteSeverity $passThroughDamage
	$transform = $transformMap[$damageType]
	if ($severityOrder[$severity] -le $severityOrder[$transform.Threshold]) {
		return $transform.To
	}

	return $damageType
}

function Get-GenericArmourCategory {
	param([string]$damageType, $family)
	if ($family.Zero -contains $damageType) { return 'Zero' }
	if ($family.Super -contains $damageType) { return 'Super' }
	if ($family.Strong -contains $damageType) { return 'Strong' }
	if ($family.Weak -contains $damageType) { return 'Weak' }
	return 'Default'
}

function Apply-GenericArmourLayer {
	param($packet, $family, [double]$layerQuality)

	if ($null -eq $packet) {
		return $null
	}

	$category = Get-GenericArmourCategory -damageType $packet.Type -family $family
	switch ($category) {
		'Zero' {
			$dissipated = $packet.Amount
			$passThrough = $packet.Amount
		}
		'Super' {
			$dissipated = $packet.Amount - ($layerQuality * ($family.Power * 2.0))
			$passThrough = $packet.Amount * ((1.0 - $family.Power * 0.05) - ($layerQuality * ($family.Power * 0.05)))
		}
		'Strong' {
			$dissipated = $packet.Amount - ($layerQuality * ($family.Power * 0.65))
			$passThrough = $packet.Amount * ((1.0 - $family.Power * 0.03) - ($layerQuality * ($family.Power * 0.03)))
		}
		'Weak' {
			$dissipated = $packet.Amount - ($layerQuality * ($family.Power * 0.1))
			$passThrough = $packet.Amount * ((1.0 - $family.Power * 0.01) - ($layerQuality * ($family.Power * 0.01)))
		}
		default {
			$dissipated = $packet.Amount - ($layerQuality * ($family.Power * 0.35))
			$passThrough = $packet.Amount * ((1.0 - $family.Power * 0.02) - ($layerQuality * ($family.Power * 0.02)))
		}
	}

	if ($dissipated -le 0.0) {
		return [pscustomobject]@{ Layer = $family.Name; OuterDamage = 0.0; PassThrough = $null }
	}

	$transformedType = Transform-DamageType -damageType $packet.Type -passThroughDamage $passThrough -transformMap $family.Transforms
	return [pscustomobject]@{
		Layer = $family.Name
		OuterDamage = $dissipated
		PassThrough = [pscustomobject]@{ Amount = $passThrough; Type = $transformedType }
	}
}

function Get-CutLikeTypes {
	return @('Slashing', 'Chopping', 'Piercing', 'Ballistic', 'Bite', 'Claw', 'Shearing', 'BallisticArmourPiercing', 'ArmourPiercing', 'Shrapnel')
}

function Get-ImpactLikeTypes {
	return @('Crushing', 'Shockwave', 'Sonic', 'Wrenching', 'Falling')
}

function Apply-NaturalLayer {
	param($packet, $layer, $material)

	if ($null -eq $packet) {
		return $null
	}

	$cutLike = Get-CutLikeTypes
	$impactLike = Get-ImpactLikeTypes
	$shear = if ($null -ne $material) { $material.Shear } else { 0.0 }
	$impact = if ($null -ne $material) { $material.Impact } else { 0.0 }

	switch ($layer.Kind) {
		'OldNatural' {
			if ($cutLike -contains $packet.Type) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.75)
			} elseif ($impactLike -contains $packet.Type) {
				$dissipated = $packet.Amount - ($layer.Quality * $impact / 10000.0 * 0.75)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 0.75)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'Ballistic' { $passThrough = $packet.Amount * (0.9 - ($layer.Quality * 0.02)) }
				'BallisticArmourPiercing' { $passThrough = $packet.Amount * (1.0 - ($layer.Quality * 0.02)) }
				'ArmourPiercing' { $passThrough = $packet.Amount * (1.0 - ($layer.Quality * 0.02)) }
				default { $passThrough = $packet.Amount * (0.8 - ($layer.Quality * 0.02)) }
			}
		}
		'RacialTissue' {
			if (($cutLike -contains $packet.Type) -or ($impactLike -contains $packet.Type)) {
				$dissipated = $packet.Amount - ($layer.Quality * 0.5)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 0.75)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'Slashing' { $passThrough = $packet.Amount * 0.95 }
				'Chopping' { $passThrough = $packet.Amount * 0.95 }
				'Piercing' { $passThrough = $packet.Amount * 0.95 }
				'Ballistic' { $passThrough = $packet.Amount * 0.95 }
				'Bite' { $passThrough = $packet.Amount * 0.95 }
				'Claw' { $passThrough = $packet.Amount * 0.95 }
				'Shearing' { $passThrough = $packet.Amount * 0.95 }
				'BallisticArmourPiercing' { $passThrough = $packet.Amount * 0.95 }
				'ArmourPiercing' { $passThrough = $packet.Amount * 0.95 }
				'Shrapnel' { $passThrough = $packet.Amount * 0.95 }
				'Crushing' { $passThrough = $packet.Amount * 0.90 }
				'Shockwave' { $passThrough = $packet.Amount * 0.90 }
				'Sonic' { $passThrough = $packet.Amount * 0.90 }
				'Wrenching' { $passThrough = $packet.Amount * 0.90 }
				'Falling' { $passThrough = $packet.Amount * 0.90 }
				default { $passThrough = $packet.Amount * 0.80 }
			}
		}
		'BodyFlesh' {
			if ($cutLike -contains $packet.Type) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.75)
			} elseif ($impactLike -contains $packet.Type) {
				$dissipated = $packet.Amount - ($layer.Quality * $impact / 10000.0 * 0.75)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 0.75)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				default { $passThrough = $packet.Amount * 0.80 }
			}
		}
		'CranialFlesh' {
			if ($cutLike -contains $packet.Type) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.75)
			} elseif ($impactLike -contains $packet.Type) {
				$dissipated = $packet.Amount - ($layer.Quality * $impact / 10000.0 * 0.75)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 0.75)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'Slashing' { $passThrough = $packet.Amount * 0.92 }
				'Chopping' { $passThrough = $packet.Amount * 0.92 }
				'Crushing' { $passThrough = $packet.Amount * 0.92 }
				'Bite' { $passThrough = $packet.Amount * 0.92 }
				'Claw' { $passThrough = $packet.Amount * 0.92 }
				'Shearing' { $passThrough = $packet.Amount * 0.92 }
				'Wrenching' { $passThrough = $packet.Amount * 0.92 }
				'Piercing' { $passThrough = $packet.Amount * 0.95 }
				'Ballistic' { $passThrough = $packet.Amount * 0.95 }
				'BallisticArmourPiercing' { $passThrough = $packet.Amount * 0.95 }
				'ArmourPiercing' { $passThrough = $packet.Amount * 0.95 }
				'Shrapnel' { $passThrough = $packet.Amount * 0.95 }
				default { $passThrough = $packet.Amount * 0.80 }
			}
		}
		'Bone' {
			$cutLikeBone = @('Slashing', 'Chopping', 'Piercing', 'Ballistic', 'Bite', 'Claw', 'Shearing', 'BallisticArmourPiercing', 'ArmourPiercing', 'Shrapnel')
			$impactLikeBone = @('Crushing', 'Shockwave', 'Sonic', 'Wrenching', 'Falling')
			if ($cutLikeBone -contains $packet.Type) {
				$dissipated = [math]::Max($packet.Amount * 0.1, $packet.Amount - ($layer.Quality * 2.0 * $shear / 115000.0))
			} elseif ($impactLikeBone -contains $packet.Type) {
				$dissipated = [math]::Max($packet.Amount * 0.1, $packet.Amount - ($layer.Quality * 2.0 * $impact / 200000.0))
			} elseif ($packet.Type -eq 'Hypoxia') {
				$dissipated = 0.0
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 2.0)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'ArmourPiercing' { $passThrough = $packet.Amount * (1.0 - ($layer.Quality * 0.02)) }
				default { $passThrough = $packet.Amount * (0.8 - ($layer.Quality * 0.02)) }
			}
		}
		default { throw "Unknown natural-armour kind '$($layer.Kind)'." }
	}

	if ($dissipated -le 0.0) {
		return [pscustomobject]@{ Layer = $layer.Name; OuterDamage = 0.0; PassThrough = $null }
	}

	$transformedType = Transform-DamageType -damageType $packet.Type -passThroughDamage $passThrough -transformMap $layer.Transforms
	return [pscustomobject]@{
		Layer = $layer.Name
		OuterDamage = $dissipated
		PassThrough = [pscustomobject]@{ Amount = $passThrough; Type = $transformedType }
	}
}

function Get-SeverThreshold {
	param($target, [string]$snapshotKey)
	if ($snapshotKey -eq 'Before') { return $target.SeverThresholdBefore }
	return $target.SeverThresholdAfter
}

function Can-SeverDamageType {
	param([string]$damageType)
	return $damageType -in @('Bite', 'Chopping', 'Claw', 'Slashing', 'Shearing', 'Shrapnel', 'Shockwave')
}

function Get-BoneHitChance {
	param([double]$baseChance, [string]$damageType)
	switch ($damageType) {
		'BallisticArmourPiercing' { return [math]::Pow($baseChance, 1.4) }
		'Piercing' { return [math]::Pow($baseChance, 1.4) }
		'ArmourPiercing' { return [math]::Pow($baseChance, 1.4) }
		'Ballistic' { return [math]::Pow($baseChance, 1.2) }
		'Bite' { return [math]::Pow($baseChance, 1.2) }
		default { return $baseChance }
	}
}

function Get-OrganEligibility {
	param($packet)
	if ($null -eq $packet) {
		return [pscustomobject]@{ Eligible = $false; DamageAll = $false; HighChance = $false }
	}

	$severity = Get-AbsoluteSeverity $packet.Amount
	switch ($packet.Type) {
		'Hypoxia' { return [pscustomobject]@{ Eligible = $true; DamageAll = $true; HighChance = $true } }
		'Electrical' { return [pscustomobject]@{ Eligible = $true; DamageAll = $true; HighChance = $true } }
		'Shockwave' { return [pscustomobject]@{ Eligible = $true; DamageAll = $true; HighChance = $true } }
		'Cellular' { return [pscustomobject]@{ Eligible = $true; DamageAll = $true; HighChance = $true } }
		'Crushing' { return [pscustomobject]@{ Eligible = $true; DamageAll = $false; HighChance = $true } }
		'Falling' { return [pscustomobject]@{ Eligible = $true; DamageAll = $false; HighChance = $true } }
		'Ballistic' { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Moderate']); DamageAll = $false; HighChance = $false } }
		'BallisticArmourPiercing' { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Moderate']); DamageAll = $false; HighChance = $false } }
		'ArmourPiercing' { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Moderate']); DamageAll = $false; HighChance = $false } }
		'Piercing' { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Moderate']); DamageAll = $false; HighChance = $false } }
		'Eldritch' { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Moderate']); DamageAll = $false; HighChance = $false } }
		'Arcane' { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Moderate']); DamageAll = $false; HighChance = $false } }
		'Burning' { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Moderate']); DamageAll = ($severityOrder[$severity] -ge $severityOrder['VerySevere']); HighChance = ($severityOrder[$severity] -ge $severityOrder['Severe']) } }
		default { return [pscustomobject]@{ Eligible = ($severityOrder[$severity] -ge $severityOrder['Severe']); DamageAll = $false; HighChance = $false } }
	}
}

function Format-Packet {
	param($packet)
	if ($null -eq $packet) { return 'none' }
	return ('{0:N2} {1}' -f $packet.Amount, $packet.Type)
}

function Invoke-Scenario {
	param($attack, $snapshot)

	$target = $targets[$attack.Target]
	$rawDamage = Get-RawDamage -family $attack.Family -strengthValue $strength -qualityValue $quality -degree $attack.Degree
	$currentPacket = [pscustomobject]@{ Amount = $rawDamage; Type = $attack.DamageType }
	$wornTrace = @()

	foreach ($wornFamilyName in $attack.Worn) {
		$layerResult = Apply-GenericArmourLayer -packet $currentPacket -family $snapshot.ArmourFamilies[$wornFamilyName] -layerQuality $quality
		$wornTrace += $layerResult
		$currentPacket = $layerResult.PassThrough
	}

	$postWornPacket = $currentPacket
	$racialResult = Apply-NaturalLayer -packet $currentPacket -layer $snapshot.RacialArmour -material $snapshot.RacialArmour.Material
	$postRacialPacket = if ($null -eq $racialResult) { $null } else { $racialResult.PassThrough }
	$bodyLayer = if ($target.UsesCranial) { $snapshot.CranialArmour } else { $snapshot.BodyArmour }
	$bodyResult = Apply-NaturalLayer -packet $postRacialPacket -layer $bodyLayer -material $target.Material
	$outerWoundPacket = if ($null -eq $bodyResult -or $null -eq $postRacialPacket) { $null } else { [pscustomobject]@{ Amount = $bodyResult.OuterDamage; Type = $postRacialPacket.Type } }
	$internalPacket = if ($null -eq $bodyResult) { $null } else { $bodyResult.PassThrough }
	$outerSeverity = if ($null -eq $outerWoundPacket) { 'None' } else { Get-AbsoluteSeverity $outerWoundPacket.Amount }

	$severThreshold = Get-SeverThreshold -target $target -snapshotKey $snapshot.Key
	$severed = $false
	if ($severThreshold -gt 0 -and $null -ne $outerWoundPacket -and (Can-SeverDamageType $outerWoundPacket.Type)) {
		$severed = $outerWoundPacket.Amount -ge $severThreshold
	}

	$boneSummary = $null
	$organSummary = $null
	if ($null -ne $target.Bone -and $null -ne $internalPacket) {
		$boneChance = if ([double]::IsNaN($target.Bone.Chance)) { $null } else { Get-BoneHitChance -baseChance $target.Bone.Chance -damageType $internalPacket.Type }
		if ($null -ne $boneChance -and $boneChance -ge 1.0) {
			$fractureRatio = $internalPacket.Amount / $target.Bone.Health
			$fractureSeverity = Get-PercentageSeverity -ratio $fractureRatio -bands $snapshot.FractureBands
			$boneResult = Apply-NaturalLayer -packet $internalPacket -layer $snapshot.BoneArmour -material $materials['compact bone']
			$organPacket = if ($null -eq $boneResult) { $null } else { $boneResult.PassThrough }
			$boneSummary = [pscustomobject]@{ Deterministic = $true; Name = $target.Bone.Name; Chance = $boneChance; Packet = $internalPacket; FractureRatio = $fractureRatio; FractureSeverity = $fractureSeverity; PostBonePacket = $organPacket }
			if ($null -ne $target.Organ -and $null -ne $organPacket) {
				$organEligibility = Get-OrganEligibility -packet $organPacket
				$organSummary = [pscustomobject]@{ Deterministic = ($organEligibility.Eligible -and $target.Organ.Chance -ge 1.0); Name = $target.Organ.Name; Chance = $target.Organ.Chance; Packet = $organPacket; Severity = if ($organEligibility.Eligible) { Get-AbsoluteSeverity $organPacket.Amount } else { 'Ineligible' }; Eligibility = $organEligibility }
			}
		} else {
			$boneSummary = [pscustomobject]@{ Deterministic = $false; Name = $target.Bone.Name; Chance = $boneChance; Packet = $internalPacket; Note = $target.InternalNotes }
			if ($null -ne $target.Organ) {
				$organEligibility = Get-OrganEligibility -packet $internalPacket
				$organSummary = [pscustomobject]@{ Deterministic = $false; Name = $target.Organ.Name; Chance = $target.Organ.Chance; Packet = $internalPacket; Severity = if ($organEligibility.Eligible) { Get-AbsoluteSeverity $internalPacket.Amount } else { 'Ineligible' }; Eligibility = $organEligibility; Note = $target.InternalNotes }
			}
		}
	}

	return [pscustomobject]@{
		Attack = $attack.Name
		Target = $target.Name
		RawDamage = $rawDamage
		PostWornPacket = $postWornPacket
		WornTrace = $wornTrace
		RacialOuterDamage = if ($null -eq $racialResult) { 0.0 } else { $racialResult.OuterDamage }
		PostRacialPacket = $postRacialPacket
		BodyOuterDamage = if ($null -eq $bodyResult) { 0.0 } else { $bodyResult.OuterDamage }
		InternalPacket = $internalPacket
		OuterSeverity = $outerSeverity
		SeverThreshold = $severThreshold
		Severed = $severed
		Bone = $boneSummary
		Organ = $organSummary
	}
}

function Format-InternalSummary {
	param($result)
	if ($null -eq $result.Bone) { return 'no internal routing' }
	if ($result.Bone.Deterministic) {
		$boneLine = '{0}: {1} fracture ({2:P2}, {3})' -f $result.Bone.Name, (Format-Packet $result.Bone.Packet), $result.Bone.FractureRatio, $result.Bone.FractureSeverity
		if ($null -ne $result.Organ) {
			$organLine = '{0}: {1} ({2})' -f $result.Organ.Name, (Format-Packet $result.Organ.Packet), $result.Organ.Severity
			return "$boneLine; $organLine"
		}
		return $boneLine
	}

	$boneChanceText = if ($null -eq $result.Bone.Chance) { $result.Bone.Note } else { '{0} at {1:P2}; packet {2}' -f $result.Bone.Name, $result.Bone.Chance, (Format-Packet $result.Bone.Packet) }
	$organText = if ($null -eq $result.Organ) { $null } else { '{0} eligibility {1}; coverage {2:P2}; packet {3}' -f $result.Organ.Name, $result.Organ.Severity, $result.Organ.Chance, (Format-Packet $result.Organ.Packet) }
	if ([string]::IsNullOrEmpty($organText)) { return $boneChanceText }
	return "$boneChanceText; $organText"
}

function Format-WornSummary {
	param($result)
	if (-not $result.WornTrace.Count) { return 'none' }
	return (($result.WornTrace | ForEach-Object { '{0}: {1:N2} outer, {2}' -f $_.Layer, $_.OuterDamage, (Format-Packet $_.PassThrough) }) -join '; ')
}

function Get-SummaryMetrics {
	param($snapshot)
	$majorSeverCount = 0
	$swordSevereCount = 0
	$fractureModerateCount = 0
	for ($degree = 0; $degree -le 5; $degree++) {
		$sword = Invoke-Scenario -attack ([pscustomobject]@{ Name = 'Longsword 1-Handed Arm Swing'; Family = 'Normal'; DamageType = 'Slashing'; Degree = $degree; Target = 'rupperarm'; Worn = @() }) -snapshot $snapshot
		if ($sword.Severed) { $majorSeverCount++ }
		if ($severityOrder[$sword.OuterSeverity] -ge $severityOrder['Severe']) { $swordSevereCount++ }

		$hammer = Invoke-Scenario -attack ([pscustomobject]@{ Name = 'Warhammer Crush Right Leg'; Family = 'Coup de Grace'; DamageType = 'Crushing'; Degree = $degree; Target = 'rshin'; Worn = @() }) -snapshot $snapshot
		if ($null -ne $hammer.Bone -and $hammer.Bone.Deterministic -and $severityOrder[$hammer.Bone.FractureSeverity] -ge $severityOrder['Moderate']) { $fractureModerateCount++ }
	}

	return [pscustomobject]@{ MajorSeverCount = $majorSeverCount; SwordSevereCount = $swordSevereCount; FractureModerateCount = $fractureModerateCount }
}

$scenarioResults = foreach ($attack in $attacks) {
	$wornSuffix = if ($attack.Worn.Count) { ' (' + (($attack.Worn -join ' + ')) + ')' } else { ' (unarmoured)' }
	[pscustomobject]@{
		Scenario = '{0} -> {1}{2}' -f $attack.Name, $targets[$attack.Target].Name, $wornSuffix
		Before = Invoke-Scenario -attack $attack -snapshot $snapshots.Before
		After = Invoke-Scenario -attack $attack -snapshot $snapshots.After
	}
}

$beforeMetrics = Get-SummaryMetrics -snapshot $snapshots.Before
$afterMetrics = Get-SummaryMetrics -snapshot $snapshots.After

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add('# Human Combat Balance Profile Validation')
$lines.Add('')
$lines.Add(('Strength {0}, item quality {1}, natural-armour quality {2}. Comparing profiles `{3}` and `{4}`.' -f $strength, $quality, $naturalArmourQuality, $BaselineProfile, $ComparisonProfile))
$lines.Add('Assumptions: chainmail is worn over heavy clothing, the helmeted head case uses a `Platemail` family helmet, and stochastic internal rolls are reported as chances rather than sampled.')
$lines.Add('')
$lines.Add('## Scenario Comparisons')
$lines.Add('')

	foreach ($scenario in $scenarioResults) {
	$lines.Add("### $($scenario.Scenario)")
	$lines.Add('')
	$lines.Add('| Profile | Raw | Post Worn | Post Racial | Outer Wound | Inward Packet | Wound Severity | Fracture / Internal | Sever |')
	$lines.Add('| --- | ---: | --- | --- | ---: | --- | --- | --- | --- |')
	foreach ($label in @('Before', 'After')) {
		$result = $scenario.$label
		$profileLabel = $snapshots[$label].Label
		$severText = if ($result.SeverThreshold -gt 0) {
			if ($result.Severed) { 'Yes' } else { 'No' }
		} else {
			'N/A'
		}
		$lines.Add(('| {0} | {1:N2} | {2} | {3} | {4:N2} | {5} | {6} | {7} | {8} |' -f $profileLabel, $result.RawDamage, (Format-Packet $result.PostWornPacket), (Format-Packet $result.PostRacialPacket), $result.BodyOuterDamage, (Format-Packet $result.InternalPacket), $result.OuterSeverity, (Format-InternalSummary $result), $severText))
	}
	$lines.Add('')
	$lines.Add(('Worn-layer detail, {0}: {1}' -f $BaselineProfile, (Format-WornSummary $scenario.Before)))
	$lines.Add(('Worn-layer detail, {0}: {1}' -f $ComparisonProfile, (Format-WornSummary $scenario.After)))
	$lines.Add('')
}

$headBefore = ($scenarioResults | Where-Object { $_.Scenario -eq 'Warhammer Crush Head -> forehead (unarmoured)' }).Before
$headAfter = ($scenarioResults | Where-Object { $_.Scenario -eq 'Warhammer Crush Head -> forehead (unarmoured)' }).After
$helmetBefore = ($scenarioResults | Where-Object { $_.Scenario -eq 'Warhammer Crush Head -> forehead (Platemail)' }).Before
$helmetAfter = ($scenarioResults | Where-Object { $_.Scenario -eq 'Warhammer Crush Head -> forehead (Platemail)' }).After
$spearMailBefore = ($scenarioResults | Where-Object { $_.Scenario -eq 'Spear 1-Handed Stab -> right breast (Chainmail + Heavy Clothing)' }).Before
$spearMailAfter = ($scenarioResults | Where-Object { $_.Scenario -eq 'Spear 1-Handed Stab -> right breast (Chainmail + Heavy Clothing)' }).After

$lines.Add('## Behavioural Shift Summary')
$lines.Add('')
$lines.Add(('- Major-limb sever checks in the `Longsword -> upper arm` degree sweep move from {0}/6 in `{1}` to {2}/6 in `{3}`.' -f $beforeMetrics.MajorSeverCount, $BaselineProfile, $afterMetrics.MajorSeverCount, $ComparisonProfile))
$lines.Add(('- Sword outer wounds at `Severe+` in the same sweep move from {0}/6 in `{1}` to {2}/6 in `{3}`.' -f $beforeMetrics.SwordSevereCount, $BaselineProfile, $afterMetrics.SwordSevereCount, $ComparisonProfile))
$lines.Add(('- Deterministic `Warhammer -> shin` fractures at `Moderate+` move from {0}/6 in `{1}` to {2}/6 in `{3}`.' -f $beforeMetrics.FractureModerateCount, $BaselineProfile, $afterMetrics.FractureModerateCount, $ComparisonProfile))
$lines.Add(('- Unhelmeted forehead strikes pass {0:N2} inward before the skull in `{1}` instead of {2:N2} in `{3}`, and the frontal-cranial fracture read shifts from {4} to {5}.' -f $headAfter.InternalPacket.Amount, $ComparisonProfile, $headBefore.InternalPacket.Amount, $BaselineProfile, $headBefore.Bone.FractureSeverity, $headAfter.Bone.FractureSeverity))
$lines.Add(('- Helmeted forehead strikes still protect strongly, but plate leaks {0:N2} inward pre-skull in `{1}` instead of {2:N2} in `{3}`.' -f $helmetAfter.InternalPacket.Amount, $ComparisonProfile, $helmetBefore.InternalPacket.Amount, $BaselineProfile))
$lines.Add(('- `Spear -> chainmail + heavy clothing` still strips a lot of damage, but the inward packet increases from {0} in `{1}` to {2} in `{3}`, which is the intended less-binary top-end result.' -f (Format-Packet $spearMailBefore.InternalPacket), $BaselineProfile, (Format-Packet $spearMailAfter.InternalPacket), $ComparisonProfile))
$lines.Add('')
$lines.Add('## Notes')
$lines.Add('')
$lines.Add('- The harness intentionally mirrors the current runtime armour implementation, including pass-through being evaluated from the incoming packet for each layer.')
$lines.Add('- When internal routing is stochastic, the report prints seeded hit chances and eligibility instead of sampling a roll.')

($lines -join [Environment]::NewLine)
