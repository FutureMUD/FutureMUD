ALTER TABLE `dbo`.`TraitExpression` 
ADD COLUMN `Name` VARCHAR(200) NOT NULL DEFAULT 'Unnamed Expression' AFTER `Expression`;
