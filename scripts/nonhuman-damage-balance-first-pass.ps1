param(
	[string]$BaselineProfile = 'stock',
	[string]$ComparisonProfile = 'combat-rebalance'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$strength = 15.0
$quality = 5.0
$naturalArmourQuality = 2.0
$robotNaturalArmourQuality = 4.0

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
	ArmourPiercing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Small' }
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

$robotPlatingTransforms = @{
	Slashing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'VerySevere' }
	Chopping = [pscustomobject]@{ To = 'Crushing'; Threshold = 'VerySevere' }
	Piercing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Moderate' }
	Ballistic = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Moderate' }
	Bite = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	Claw = [pscustomobject]@{ To = 'Crushing'; Threshold = 'VerySevere' }
	Shearing = [pscustomobject]@{ To = 'Crushing'; Threshold = 'VerySevere' }
	Wrenching = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Severe' }
	Shrapnel = [pscustomobject]@{ To = 'Crushing'; Threshold = 'Moderate' }
	ArmourPiercing = [pscustomobject]@{ To = 'ArmourPiercing'; Threshold = 'Horrifying' }
}

$materials = @{
	'Flesh' = [pscustomobject]@{ Name = 'Flesh'; Shear = 25000.0; Impact = 10000.0 }
	'Bony Flesh' = [pscustomobject]@{ Name = 'Bony Flesh'; Shear = 50000.0; Impact = 60000.0 }
	'Dense Bony Flesh' = [pscustomobject]@{ Name = 'Dense Bony Flesh'; Shear = 65000.0; Impact = 75000.0 }
	'Compact Bone' = [pscustomobject]@{ Name = 'Compact Bone'; Shear = 115000.0; Impact = 200000.0 }
	'Chassis Alloy' = [pscustomobject]@{ Name = 'Chassis Alloy'; Shear = 115000.0; Impact = 200000.0 }
}

$snapshots = @{
	AnimalBefore = [pscustomobject]@{
		Key = 'AnimalBefore'
		Label = $BaselineProfile
		FractureBands = $percentageBandsBefore
		RacialArmour = $null
		BodyArmour = [pscustomobject]@{ Name = 'Non-Human Natural Armour'; Kind = 'OldNatural'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
		BoneArmour = [pscustomobject]@{ Name = 'Non-Human Natural Bone Armour'; Kind = 'Bone'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
	}
	AnimalAfter = [pscustomobject]@{
		Key = 'AnimalAfter'
		Label = $ComparisonProfile
		FractureBands = $percentageBandsAfter
		RacialArmour = $null
		BodyArmour = [pscustomobject]@{ Name = 'Non-Human Natural Armour'; Kind = 'AnimalFlesh'; Quality = $naturalArmourQuality; Transforms = $relaxedFleshTransforms }
		BoneArmour = [pscustomobject]@{ Name = 'Non-Human Natural Bone Armour'; Kind = 'Bone'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
	}
	GriffinBefore = [pscustomobject]@{
		Key = 'GriffinBefore'
		Label = $BaselineProfile
		FractureBands = $percentageBandsBefore
		RacialArmour = [pscustomobject]@{ Name = 'Mythic Animal Race Armour'; Kind = 'OldNatural'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
		BodyArmour = [pscustomobject]@{ Name = 'Non-Human Natural Armour'; Kind = 'OldNatural'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
		BoneArmour = [pscustomobject]@{ Name = 'Non-Human Natural Bone Armour'; Kind = 'Bone'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
	}
	GriffinAfter = [pscustomobject]@{
		Key = 'GriffinAfter'
		Label = $ComparisonProfile
		FractureBands = $percentageBandsAfter
		RacialArmour = $null
		BodyArmour = [pscustomobject]@{ Name = 'Non-Human Natural Armour'; Kind = 'AnimalFlesh'; Quality = $naturalArmourQuality; Transforms = $relaxedFleshTransforms }
		BoneArmour = [pscustomobject]@{ Name = 'Non-Human Natural Bone Armour'; Kind = 'Bone'; Quality = $naturalArmourQuality; Transforms = $genericTransforms }
	}
	RobotBefore = [pscustomobject]@{
		Key = 'RobotBefore'
		Label = $BaselineProfile
		FractureBands = $percentageBandsBefore
		RacialArmour = [pscustomobject]@{ Name = 'Robot Natural Armour'; Kind = 'OldNatural'; Quality = $robotNaturalArmourQuality; Transforms = $genericTransforms }
		BodyArmour = [pscustomobject]@{ Name = 'Robot Natural Armour'; Kind = 'OldNatural'; Quality = $robotNaturalArmourQuality; Transforms = $genericTransforms }
		BodyLightArmour = [pscustomobject]@{ Name = 'Robot Light Armour'; Kind = 'OldNatural'; Quality = $robotNaturalArmourQuality; Transforms = $genericTransforms }
	}
	RobotAfter = [pscustomobject]@{
		Key = 'RobotAfter'
		Label = $ComparisonProfile
		FractureBands = $percentageBandsAfter
		RacialArmour = [pscustomobject]@{ Name = 'Robot Frame Armour'; Kind = 'RobotFrame'; Quality = $robotNaturalArmourQuality; Transforms = $relaxedFleshTransforms }
		BodyArmour = [pscustomobject]@{ Name = 'Robot Natural Armour'; Kind = 'RobotPlating'; Quality = $robotNaturalArmourQuality; Transforms = $robotPlatingTransforms }
		BodyLightArmour = [pscustomobject]@{ Name = 'Robot Light Armour'; Kind = 'RobotLightPlating'; Quality = $robotNaturalArmourQuality; Transforms = $robotPlatingTransforms }
	}
}

$targets = @{
	animal_ruforeleg = [pscustomobject]@{ Name = 'Quadruped right upper foreleg'; Material = $materials['Bony Flesh']; BodypartHealth = 80.0; SeverBefore = 100.0; SeverAfter = 27.0; Bone = [pscustomobject]@{ Name = 'right humerus'; Chance = 0.50; Health = 140.0 } }
	animal_head = [pscustomobject]@{ Name = 'Quadruped head'; Material = $materials['Bony Flesh']; BodypartHealth = 80.0; SeverBefore = -1.0; SeverAfter = -1.0; Bone = [pscustomobject]@{ Name = 'frontal skull bone'; Chance = 1.00; Health = 200.0 } }
	griffin_rwing = [pscustomobject]@{ Name = 'Griffin right wing'; Material = $materials['Flesh']; BodypartHealth = 40.0; SeverBefore = 50.0; SeverAfter = 27.0; Bone = $null }
	robot_rupperarm = [pscustomobject]@{ Name = 'Robot humanoid right upper arm'; Material = $materials['Chassis Alloy']; BodypartHealth = 80.0; SeverBefore = 27.0; SeverAfter = 27.0; Bone = [pscustomobject]@{ Name = 'right humerus'; Chance = 0.50; Health = 140.0 } }
	robot_sensorpod = [pscustomobject]@{ Name = 'Robot utility sensor pod'; Material = $materials['Chassis Alloy']; BodypartHealth = 120.0; SeverBefore = 90.0; SeverAfter = 18.0; Bone = $null }
}

$scenarios = @(
	[pscustomobject]@{ Name = 'Longsword -> quadruped foreleg'; Family = 'Normal'; DamageType = 'Slashing'; Degree = 5; Target = 'animal_ruforeleg'; Before = 'AnimalBefore'; After = 'AnimalAfter'; BodyArmourKey = 'BodyArmour' },
	[pscustomobject]@{ Name = 'Warhammer -> quadruped head'; Family = 'Coup de Grace'; DamageType = 'Crushing'; Degree = 5; Target = 'animal_head'; Before = 'AnimalBefore'; After = 'AnimalAfter'; BodyArmourKey = 'BodyArmour' },
	[pscustomobject]@{ Name = 'Longsword -> griffin wing'; Family = 'Normal'; DamageType = 'Slashing'; Degree = 5; Target = 'griffin_rwing'; Before = 'GriffinBefore'; After = 'GriffinAfter'; BodyArmourKey = 'BodyArmour' },
	[pscustomobject]@{ Name = 'Longsword -> robot upper arm'; Family = 'Normal'; DamageType = 'Slashing'; Degree = 5; Target = 'robot_rupperarm'; Before = 'RobotBefore'; After = 'RobotAfter'; BodyArmourKey = 'BodyArmour' },
	[pscustomobject]@{ Name = 'Axe -> robot utility sensor pod'; Family = 'Good'; DamageType = 'Chopping'; Degree = 5; Target = 'robot_sensorpod'; Before = 'RobotBefore'; After = 'RobotAfter'; BodyArmourKey = 'BodyLightArmour' }
)

function Get-FamilyMultiplier {
	param([string]$family)
	switch ($family) {
		'Normal' { return 0.18 }
		'Good' { return 0.24 }
		'Coup de Grace' { return 0.60 }
		default { throw "Unknown family '$family'." }
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
	if ($null -eq $transformMap -or -not $transformMap.ContainsKey($damageType)) {
		return $damageType
	}

	$severity = Get-AbsoluteSeverity $passThroughDamage
	$transform = $transformMap[$damageType]
	if ($severityOrder[$severity] -le $severityOrder[$transform.Threshold]) {
		return $transform.To
	}

	return $damageType
}

function Apply-NaturalLayer {
	param($packet, $layer, $material)

	if ($null -eq $packet -or $null -eq $layer) {
		return $null
	}

	$shear = $material.Shear
	$impact = $material.Impact
	switch ($layer.Kind) {
		'OldNatural' {
			if ($packet.Type -in @('Slashing','Chopping','Piercing','Ballistic','Bite','Claw','Shearing','BallisticArmourPiercing','ArmourPiercing','Shrapnel')) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.75)
			} elseif ($packet.Type -in @('Crushing','Shockwave','Sonic','Wrenching','Falling')) {
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
		'AnimalFlesh' {
			if ($packet.Type -in @('Slashing','Chopping','Piercing','Ballistic','Bite','Claw','Shearing','BallisticArmourPiercing','ArmourPiercing','Shrapnel')) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.75)
			} elseif ($packet.Type -in @('Crushing','Shockwave','Sonic','Wrenching','Falling')) {
				$dissipated = $packet.Amount - ($layer.Quality * $impact / 10000.0 * 0.75)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 0.75)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'Slashing' { $passThrough = $packet.Amount * 0.9 }
				'Chopping' { $passThrough = $packet.Amount * 0.9 }
				'Piercing' { $passThrough = $packet.Amount * 0.9 }
				'Ballistic' { $passThrough = $packet.Amount * 0.9 }
				'Bite' { $passThrough = $packet.Amount * 0.9 }
				'Claw' { $passThrough = $packet.Amount * 0.9 }
				'Shearing' { $passThrough = $packet.Amount * 0.9 }
				'BallisticArmourPiercing' { $passThrough = $packet.Amount * 0.9 }
				'ArmourPiercing' { $passThrough = $packet.Amount * 0.9 }
				'Shrapnel' { $passThrough = $packet.Amount * 0.9 }
				'Crushing' { $passThrough = $packet.Amount * 0.85 }
				'Shockwave' { $passThrough = $packet.Amount * 0.85 }
				'Sonic' { $passThrough = $packet.Amount * 0.85 }
				'Wrenching' { $passThrough = $packet.Amount * 0.85 }
				'Falling' { $passThrough = $packet.Amount * 0.85 }
				default { $passThrough = $packet.Amount * 0.8 }
			}
		}
		'RobotFrame' {
			if ($packet.Type -in @('Slashing','Chopping','Piercing','Ballistic','Bite','Claw','Shearing','BallisticArmourPiercing','ArmourPiercing','Shrapnel')) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.18)
			} elseif ($packet.Type -in @('Crushing','Shockwave','Sonic','Wrenching','Falling')) {
				$dissipated = $packet.Amount - ($layer.Quality * $impact / 10000.0 * 0.15)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 0.75)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'Crushing' { $passThrough = $packet.Amount * 0.9 }
				'Shockwave' { $passThrough = $packet.Amount * 0.9 }
				'Sonic' { $passThrough = $packet.Amount * 0.9 }
				'Wrenching' { $passThrough = $packet.Amount * 0.9 }
				'Falling' { $passThrough = $packet.Amount * 0.9 }
				default { $passThrough = $packet.Amount * 0.95 }
			}
		}
		'RobotPlating' {
			if ($packet.Type -in @('Slashing','Chopping','Piercing','Ballistic','Bite','Claw','Shearing','BallisticArmourPiercing','ArmourPiercing','Shrapnel')) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.45)
			} elseif ($packet.Type -in @('Crushing','Shockwave','Sonic','Wrenching','Falling')) {
				$dissipated = $packet.Amount - ($layer.Quality * $impact / 10000.0 * 0.28)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 1.0)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'Slashing' { $passThrough = $packet.Amount * 0.76 }
				'Chopping' { $passThrough = $packet.Amount * 0.76 }
				'Claw' { $passThrough = $packet.Amount * 0.76 }
				'Shearing' { $passThrough = $packet.Amount * 0.76 }
				'Piercing' { $passThrough = $packet.Amount * 0.84 }
				'Ballistic' { $passThrough = $packet.Amount * 0.84 }
				'BallisticArmourPiercing' { $passThrough = $packet.Amount * 0.84 }
				'ArmourPiercing' { $passThrough = $packet.Amount * 0.84 }
				'Shrapnel' { $passThrough = $packet.Amount * 0.84 }
				default { $passThrough = $packet.Amount * 0.88 }
			}
		}
		'RobotLightPlating' {
			if ($packet.Type -in @('Slashing','Chopping','Piercing','Ballistic','Bite','Claw','Shearing','BallisticArmourPiercing','ArmourPiercing','Shrapnel')) {
				$dissipated = $packet.Amount - ($layer.Quality * $shear / 25000.0 * 0.28)
			} elseif ($packet.Type -in @('Crushing','Shockwave','Sonic','Wrenching','Falling')) {
				$dissipated = $packet.Amount - ($layer.Quality * $impact / 10000.0 * 0.18)
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 0.85)
			}

			switch ($packet.Type) {
				'Hypoxia' { $passThrough = 0.0 }
				'Cellular' { $passThrough = 0.0 }
				'Slashing' { $passThrough = $packet.Amount * 0.84 }
				'Chopping' { $passThrough = $packet.Amount * 0.84 }
				'Claw' { $passThrough = $packet.Amount * 0.84 }
				'Shearing' { $passThrough = $packet.Amount * 0.84 }
				'Piercing' { $passThrough = $packet.Amount * 0.9 }
				'Ballistic' { $passThrough = $packet.Amount * 0.9 }
				'BallisticArmourPiercing' { $passThrough = $packet.Amount * 0.9 }
				'ArmourPiercing' { $passThrough = $packet.Amount * 0.9 }
				'Shrapnel' { $passThrough = $packet.Amount * 0.9 }
				default { $passThrough = $packet.Amount * 0.92 }
			}
		}
		'Bone' {
			if ($packet.Type -in @('Slashing','Chopping','Piercing','Ballistic','Bite','Claw','Shearing','BallisticArmourPiercing','ArmourPiercing','Shrapnel')) {
				$dissipated = [math]::Max($packet.Amount * 0.1, $packet.Amount - ($layer.Quality * 2.0 * $shear / 115000.0))
			} elseif ($packet.Type -in @('Crushing','Shockwave','Sonic','Wrenching','Falling')) {
				$dissipated = [math]::Max($packet.Amount * 0.1, $packet.Amount - ($layer.Quality * 2.0 * $impact / 200000.0))
			} else {
				$dissipated = $packet.Amount - ($layer.Quality * 2.0)
			}
			$passThrough = $packet.Amount * 0.76
		}
		default {
			throw "Unknown layer kind '$($layer.Kind)'."
		}
	}

	if ($dissipated -le 0.0) {
		return [pscustomobject]@{ OuterDamage = 0.0; PassThrough = $null }
	}

	[pscustomobject]@{
		OuterDamage = $dissipated
		PassThrough = [pscustomobject]@{
			Amount = $passThrough
			Type = Transform-DamageType -damageType $packet.Type -passThroughDamage $passThrough -transformMap $layer.Transforms
		}
	}
}

function Format-Packet {
	param($packet)
	if ($null -eq $packet) { return 'none' }
	return ('{0:N2} {1}' -f $packet.Amount, $packet.Type)
}

function Can-SeverDamageType {
	param([string]$damageType)
	return $damageType -in @('Bite', 'Chopping', 'Claw', 'Slashing', 'Shearing', 'Shrapnel', 'Shockwave')
}

function Invoke-Scenario {
	param($scenario, [string]$phase)

	$target = $targets[$scenario.Target]
	$snapshot = $snapshots[$scenario.$phase]
	$rawDamage = Get-RawDamage -family $scenario.Family -strengthValue $strength -qualityValue $quality -degree $scenario.Degree
	$currentPacket = [pscustomobject]@{ Amount = $rawDamage; Type = $scenario.DamageType }
	$racialResult = Apply-NaturalLayer -packet $currentPacket -layer $snapshot.RacialArmour -material $target.Material
	$postRacial = if ($null -eq $racialResult) { $currentPacket } else { $racialResult.PassThrough }
	$bodyLayer = $snapshot.$($scenario.BodyArmourKey)
	$bodyResult = Apply-NaturalLayer -packet $postRacial -layer $bodyLayer -material $target.Material
	$internal = if ($null -eq $bodyResult) { $null } else { $bodyResult.PassThrough }
	$outerSeverity = if ($null -eq $bodyResult) { 'None' } else { Get-AbsoluteSeverity $bodyResult.OuterDamage }
	$severThreshold = if ($phase -eq 'Before') { $target.SeverBefore } else { $target.SeverAfter }
	$severed = $false
	if ($severThreshold -gt 0 -and $null -ne $bodyResult -and (Can-SeverDamageType $scenario.DamageType)) {
		$severed = $bodyResult.OuterDamage -ge $severThreshold
	}

	$boneSummary = $null
	if ($null -ne $target.Bone -and $null -ne $internal) {
		$fractureRatio = ($internal.Amount / $target.Bone.Health)
		$fractureSeverity = Get-PercentageSeverity -ratio $fractureRatio -bands $snapshot.FractureBands
		$boneLayer = if ($snapshot.PSObject.Properties.Name -contains 'BoneArmour') { $snapshot.BoneArmour } else { $null }
		$postBone = if ($null -eq $boneLayer) { $null } else { (Apply-NaturalLayer -packet $internal -layer $boneLayer -material $materials['Compact Bone']).PassThrough }
		$boneSummary = [pscustomobject]@{
			Name = $target.Bone.Name
			Chance = $target.Bone.Chance
			FractureRatio = $fractureRatio
			FractureSeverity = $fractureSeverity
			PostBone = $postBone
		}
	}

	[pscustomobject]@{
		RawDamage = $rawDamage
		PostRacial = $postRacial
		OuterDamage = if ($null -eq $bodyResult) { 0.0 } else { $bodyResult.OuterDamage }
		Internal = $internal
		OuterSeverity = $outerSeverity
		SeverThreshold = $severThreshold
		Severed = $severed
		Bone = $boneSummary
	}
}

$results = foreach ($scenario in $scenarios) {
	[pscustomobject]@{
		Name = $scenario.Name
		Before = Invoke-Scenario -scenario $scenario -phase 'Before'
		After = Invoke-Scenario -scenario $scenario -phase 'After'
	}
}

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add('# Non-Human Combat Balance Profile Validation')
$lines.Add('')
$lines.Add(('Strength {0}, item quality {1}, natural-armour quality {2}, robot natural-armour quality {3}. Comparing profiles `{4}` and `{5}`.' -f $strength, $quality, $naturalArmourQuality, $robotNaturalArmourQuality, $BaselineProfile, $ComparisonProfile))
$lines.Add('Assumptions: representative stock attack families, seeded static-mode formulas, and chassis-alloy validation yields aligned to the robot seeder plating baseline.')
$lines.Add('')
$lines.Add('## Scenario Comparisons')
$lines.Add('')

foreach ($result in $results) {
	$lines.Add("### $($result.Name)")
	$lines.Add('')
	$lines.Add('| Profile | Raw | Post Racial | Outer Wound | Inward Packet | Wound Severity | Fracture | Sever |')
	$lines.Add('| --- | ---: | --- | ---: | --- | --- | --- | --- |')
	foreach ($label in @('Before', 'After')) {
		$current = $result.$label
		$profileLabel = if ($label -eq 'Before') { $BaselineProfile } else { $ComparisonProfile }
		$fracture = if ($null -eq $current.Bone) { 'n/a' } else { '{0} ({1:P2})' -f $current.Bone.FractureSeverity, $current.Bone.FractureRatio }
		$sever = if ($current.SeverThreshold -gt 0) { if ($current.Severed) { 'Yes' } else { 'No' } } else { 'N/A' }
		$lines.Add(('| {0} | {1:N2} | {2} | {3:N2} | {4} | {5} | {6} | {7} |' -f $profileLabel, $current.RawDamage, (Format-Packet $current.PostRacial), $current.OuterDamage, (Format-Packet $current.Internal), $current.OuterSeverity, $fracture, $sever))
	}
	$lines.Add('')
}

$animalForeleg = ($results | Where-Object Name -eq 'Longsword -> quadruped foreleg')
$animalHead = ($results | Where-Object Name -eq 'Warhammer -> quadruped head')
$griffinWing = ($results | Where-Object Name -eq 'Longsword -> griffin wing')
$robotArm = ($results | Where-Object Name -eq 'Longsword -> robot upper arm')
$robotSensor = ($results | Where-Object Name -eq 'Axe -> robot utility sensor pod')

$lines.Add('## Behavioural Shift Summary')
$lines.Add('')
$lines.Add(('- `Quadruped foreleg` severing moves from threshold `{0}` / outcome `{1}` in `{2}` to threshold `{3}` / outcome `{4}` in `{5}` at the same stock hit.' -f $animalForeleg.Before.SeverThreshold, $animalForeleg.Before.Severed, $BaselineProfile, $animalForeleg.After.SeverThreshold, $animalForeleg.After.Severed, $ComparisonProfile))
$lines.Add(('- `Quadruped head` inward packet rises from {0} in `{1}` to {2} in `{3}`, and the skull fracture read shifts from {4} to {5}.' -f (Format-Packet $animalHead.Before.Internal), $BaselineProfile, (Format-Packet $animalHead.After.Internal), $ComparisonProfile, $animalHead.Before.Bone.FractureSeverity, $animalHead.After.Bone.FractureSeverity))
$lines.Add(('- `Griffin wing` no longer gets the extra mythical race-level non-human armour layer; the inward packet rises from {0} in `{1}` to {2} in `{3}`, and the sever threshold drops from `{4}` to `{5}`.' -f (Format-Packet $griffinWing.Before.Internal), $BaselineProfile, (Format-Packet $griffinWing.After.Internal), $ComparisonProfile, $griffinWing.Before.SeverThreshold, $griffinWing.After.SeverThreshold))
$lines.Add(('- `Robot upper arm` still protects better than flesh, but the race-plus-bodypart stack leaks {0} inward in `{1}` instead of {2} in `{3}`, which is the intended less-binary result.' -f (Format-Packet $robotArm.After.Internal), $ComparisonProfile, (Format-Packet $robotArm.Before.Internal), $BaselineProfile))
$lines.Add(('- `Robot sensor pod` severing shifts from a threshold of `{0}` in `{1}` to `{2}` in `{3}`, and the same axe hit now lands as `{4}` instead of `{5}`.' -f $robotSensor.Before.SeverThreshold, $BaselineProfile, $robotSensor.After.SeverThreshold, $ComparisonProfile, $robotSensor.After.OuterSeverity, $robotSensor.Before.OuterSeverity))
$lines.Add('')
$lines.Add('## Notes')
$lines.Add('')
$lines.Add('- Ordinary animals still use bodypart natural armour as their only flesh layer; this pass did not add a new race-level animal armour layer.')
$lines.Add('- Mythical humanoid-default races inherit the human pass through `Human Racial Tissue Armour`; this harness focuses on the animal-default mythical case because that is where the extra race-layer bug existed.')
$lines.Add('- Robot validation focuses on natural-plating behavior rather than worn armour, because robot balance in this pass is primarily about race/frame/plating stacking.')

($lines -join [Environment]::NewLine)
