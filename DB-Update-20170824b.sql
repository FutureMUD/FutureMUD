CREATE TABLE `dbo`.`Bodies_Implants` (
  
`BodyId` BIGINT NOT NULL,
  
`ImplantId` BIGINT NOT NULL,
  
PRIMARY KEY (`BodyId`, `ImplantId`),
  
INDEX `FK_Bodies_Implants_GameItems_idx` (`ImplantId` ASC),
  
CONSTRAINT `FK_Bodies_Implants_Bodies`
    
FOREIGN KEY (`BodyId`)
    
REFERENCES `dbo`.`Bodies` (`Id`)
    
ON DELETE CASCADE
    
ON UPDATE CASCADE,
  
CONSTRAINT `FK_Bodies_Implants_GameItems`
    
FOREIGN KEY (`ImplantId`)
    
REFERENCES `dbo`.`GameItems` (`Id`)
    
ON DELETE CASCADE
    
ON UPDATE CASCADE);
