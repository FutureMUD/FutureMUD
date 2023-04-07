ALTER TABLE `dbo`.`Infections` 
ADD COLUMN `Immunity` DOUBLE NOT NULL DEFAULT 0.0 AFTER `BodypartId`;
