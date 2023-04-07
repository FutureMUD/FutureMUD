ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `NoMercy` BIT NOT NULL DEFAULT b'0' AFTER `RoomLayer`;
