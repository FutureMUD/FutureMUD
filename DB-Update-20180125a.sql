ALTER TABLE `dbo`.`Accounts` 
ADD COLUMN `TabRoomDescriptions` BIT NOT NULL DEFAULT b'1' AFTER `PromptType`,
ADD COLUMN `CodedRoomDescriptionAdditionsOnNewLine` BIT NOT NULL DEFAULT b'1' AFTER `TabRoomDescriptions`;
