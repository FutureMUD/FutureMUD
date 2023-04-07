CREATE TABLE `dbo`.`Drawings` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `AuthorId` BIGINT NOT NULL,
  `ShortDescription` TEXT NOT NULL,
  `FullDescription` TEXT NOT NULL,
  `ImplementType` INT NOT NULL,
  `DrawingSkill` DOUBLE NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_Drawings_Characters_idx` (`AuthorId` ASC),
  CONSTRAINT `FK_Drawings_Characters`
    FOREIGN KEY (`AuthorId`)
    REFERENCES `dbo`.`Characters` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE);

	ALTER TABLE `dbo`.`Drawings` 
ADD COLUMN `DrawingSize` INT NOT NULL AFTER `DrawingSkill`;

