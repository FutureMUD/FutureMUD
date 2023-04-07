ALTER TABLE `dbo`.`Accounts` 
ADD COLUMN `CharacterNameOverlaySetting` INT NOT NULL DEFAULT 0 AFTER `CodedRoomDescriptionAdditionsOnNewLine`,
ADD COLUMN `AppendNewlinesBetweenMultipleEchoesPerPrompt` BIT NOT NULL DEFAULT b'1' AFTER `CharacterNameOverlaySetting`;
ALTER TABLE `dbo`.`Dubs` 
ADD COLUMN `IntroducedName` VARCHAR(4000) NULL AFTER `CharacterId`;
ALTER TABLE `dbo`.`FutureProgs` 
ADD COLUMN `StaticType` INT NOT NULL DEFAULT 0 AFTER `AcceptsAnyParameters`;
