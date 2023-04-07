ALTER TABLE `dbo`.`Terrains` 
ADD COLUMN `TerrainEditorColour` VARCHAR(45) NOT NULL DEFAULT '#FFFFFFFF' AFTER `AtmosphereType`;