ALTER TABLE `dbo`.`Crafts` 
ADD COLUMN `ActiveCraftItemSDesc` VARCHAR(200) NOT NULL DEFAULT 'a craft in progress' AFTER `PhaseInformation`;